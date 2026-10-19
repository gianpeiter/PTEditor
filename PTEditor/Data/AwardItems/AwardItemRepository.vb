Imports System.Data
Imports System.Data.SqlClient
Imports System.IO

''' <summary>Uma linha de [PristonDB].[dbo].[AwardItem_List] (item esperando ser retirado no NPC).</summary>
Friend Class AwardEntry
    Public Property Id As Integer
    Public Property Character As String = ""
    Public Property ItemName As String = ""
    Public Property Code As String = ""
End Class

''' <summary>
''' Fila do NPC com *AwardItem (Servidor\Gameplay\Items\AwardItem.cpp): o servidor lista ate 10 linhas pelo nome do
''' personagem, cria o item com SendItemToPlayer (spec = classe atual) e apaga a linha. Todas as consultas daqui
''' sao parametrizadas.
''' </summary>
Friend NotInheritable Class AwardItemRepository

    Friend Const DatabaseName As String = "PristonDB"
    Private Const Table As String = "[dbo].[AwardItem_List]"
    ''' <summary>Colunas varchar(32) (Database\PristonDB.sql) e AWARD_ITEM.Name[32]: 31 caracteres + terminador.</summary>
    Friend Const MaxTextLength As Integer = 31
    ''' <summary>
    ''' AWARD_ITEM.Code e char[4]: o codigo transborda para o campo seguinte do pacote (Image, 4 bytes).
    ''' Ate 7 caracteres + terminador ainda chegam inteiros ao cliente; acima disso corrompem o pacote.
    ''' </summary>
    Friend Const MaxCodeLength As Integer = 7
    ''' <summary>AwardItem::SendList envia no maximo 10 linhas por visita ao NPC.</summary>
    Friend Const NpcListLimit As Integer = 10

    Private ReadOnly _config As DatabaseConfig

    Public Sub New(ByVal config As DatabaseConfig)
        _config = config
    End Sub

    Private Function Open() As SqlConnection
        Dim connection As New SqlConnection(_config.ConnectionString(DatabaseName))
        connection.Open()
        Return connection
    End Function

    Private Shared Function Text(ByVal command As SqlCommand, ByVal name As String, ByVal value As String) As SqlParameter
        Dim p = command.Parameters.Add(name, SqlDbType.VarChar, 32)
        p.Value = value
        Return p
    End Function

    ''' <summary>Conecta e confere se a tabela tem as colunas que o servidor usa. Lanca excecao com o motivo.</summary>
    Public Sub Test()
        Using connection = Open()
            Using command As New SqlCommand(
                "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'AwardItem_List'", connection)
                Dim columns As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                Using reader = command.ExecuteReader()
                    Do While reader.Read()
                        columns.Add(reader.GetString(0))
                    Loop
                End Using
                If columns.Count = 0 Then Throw New InvalidDataException("Table " & DatabaseName & ".dbo.AwardItem_List not found.")
                Dim missing = {"Id", "Character", "ItemName", "Code"}.Where(Function(c) Not columns.Contains(c)).ToList()
                If missing.Count > 0 Then
                    Throw New InvalidDataException("Table AwardItem_List is missing column(s): " & String.Join(", ", missing))
                End If
            End Using
        End Using
    End Sub

    ''' <summary>Itens pendentes do personagem, na ordem em que foram enviados.</summary>
    Public Function ListPending(ByVal character As String) As List(Of AwardEntry)
        Dim result As New List(Of AwardEntry)()
        Using connection = Open()
            Using command As New SqlCommand("SELECT Id, Character, ItemName, Code FROM " & Table &
                                            " WHERE Character = @character ORDER BY Id", connection)
                Text(command, "@character", character)
                Using reader = command.ExecuteReader()
                    Do While reader.Read()
                        result.Add(New AwardEntry With {
                            .Id = reader.GetInt32(0),
                            .Character = If(reader.IsDBNull(1), "", reader.GetString(1)),
                            .ItemName = If(reader.IsDBNull(2), "", reader.GetString(2)),
                            .Code = If(reader.IsDBNull(3), "", reader.GetString(3))})
                    Loop
                End Using
            End Using
        End Using
        Return result
    End Function

    ''' <summary>Insere todos os itens numa transacao (ou entram todos, ou nenhum).</summary>
    Public Sub Add(ByVal character As String, ByVal items As IEnumerable(Of AwardEntry))
        Using connection = Open()
            Using transaction = connection.BeginTransaction()
                For Each item In items
                    Using command As New SqlCommand("INSERT INTO " & Table & " (Character, ItemName, Code) VALUES (@character, @name, @code)",
                                                    connection, transaction)
                        Text(command, "@character", character)
                        Text(command, "@name", item.ItemName)
                        Text(command, "@code", item.Code)
                        command.ExecuteNonQuery()
                    End Using
                Next
                transaction.Commit()
            End Using
        End Using
    End Sub

    ''' <summary>Remove uma linha pendente. False se ela ja nao existia (retirada no NPC nesse meio tempo).</summary>
    Public Function Remove(ByVal id As Integer, ByVal character As String) As Boolean
        Using connection = Open()
            Using command As New SqlCommand("DELETE FROM " & Table & " WHERE Id = @id AND Character = @character", connection)
                command.Parameters.Add("@id", SqlDbType.Int).Value = id
                Text(command, "@character", character)
                Return command.ExecuteNonQuery() > 0
            End Using
        End Using
    End Function

    ''' <summary>Problema que impede enviar o codigo, ou "" se ok.</summary>
    Public Shared Function ValidateCode(ByVal code As String) As String
        If code = "" Then Return "Empty item code."
        If code.Length > MaxCodeLength Then Return "Code " & code & " is longer than " & MaxCodeLength & " characters (the NPC packet would be corrupted)."
        If ItemData.FindByCode("""" & code.ToUpperInvariant() & """") Is Nothing Then Return "Code " & code & " does not exist in OpenItem (the server would delete it without delivering)."
        Return ""
    End Function

    ''' <summary>Nome mostrado na janela do NPC, cortado no tamanho da coluna/pacote.</summary>
    Public Shared Function DisplayName(ByVal name As String) As String
        Return If(name.Length > MaxTextLength, name.Substring(0, MaxTextLength), name)
    End Function

End Class

''' <summary>Registro local dos envios (&lt;exe&gt;\PTEditor\Logs\AwardItems.log): o servidor nao registra esse fluxo.</summary>
Friend NotInheritable Class AwardLog

    Private Sub New()
    End Sub

    Friend Shared ReadOnly Property FilePath As String
        Get
            Return Path.Combine(AppState.LogsDir, "AwardItems.log")
        End Get
    End Property

    Friend Shared Sub Write(ByVal action As String, ByVal character As String, ByVal items As IEnumerable(Of AwardEntry))
        Try
            Dim stamp As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            File.AppendAllLines(FilePath, items.Select(Function(i) stamp & vbTab & action & vbTab & character & vbTab &
                                                                   i.Code & vbTab & i.ItemName & If(i.Id > 0, vbTab & "Id " & i.Id, "")))
        Catch ex As Exception
            ErrorLog.Write("AwardLog.Write: " & ex.Message)
        End Try
    End Sub

End Class
