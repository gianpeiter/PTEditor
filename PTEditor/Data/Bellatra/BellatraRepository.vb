Imports System.Data
Imports System.Data.SqlClient

''' <summary>Um clan do ranking da semana ([ClanDB].[dbo].[CL]).</summary>
Friend Class BellatraClan
    Public Property Idx As Integer
    Public Property ClanName As String = ""
    Public Property Points As Integer
    Public Property ClanMoney As Long
    Public Property RegiDate As String = ""
End Class

''' <summary>Gerencia da semana ([SoD2DB].[dbo].[Sod2Config], tabela do site de gerencia do legado).</summary>
Friend Class BellatraManager
    ''' <summary>False = o servidor ainda nao criou a linha (cria no primeiro tick do SoD).</summary>
    Public Property Exists As Boolean
    ''' <summary>CL.IDX do clan gerente; 0 = sem gerente nesta semana.</summary>
    Public Property Idx As Integer
    Public Property ClanName As String = ""
    Public Property Leader As String = ""
    Public Property OperFrom As DateTime
    Public Property OperTo As DateTime
    Public Property TaxRates As Integer
    ''' <summary>Taxa do dia, entregue ao ClanMoney no proximo fechamento do dia.</summary>
    Public Property TodayMoney As Long
    ''' <summary>Entregue no ultimo fechamento do dia ("ontem" na janela da Karina).</summary>
    Public Property BeforeMoney As Long
    ''' <summary>Entregue ao gerente no periodo.</summary>
    Public Property TotalMoney As Long
    Public Property ClanMoney As Long
    ''' <summary>Sod2Config.Note: saudacao da Karina.</summary>
    Public Property Note As String = ""
End Class

''' <summary>
''' Dados do Bellatra no banco, com os mesmos criterios do servidor:
''' - o ranking da semana e CL.Cpoint (&gt; 0), gravado pelo SQL.dll a cada SoD e zerado no fechamento da semana;
''' - o gerente da semana fica em SoD2DB.Sod2Config (Bellatra::Tick o troca no fechamento da semana e entrega a
'''   renda do dia ao CL.ClanMoney no fechamento do dia); a janela da Karina mostra Sod2Config.Note.
''' Todas as consultas sao parametrizadas.
''' </summary>
Friend NotInheritable Class BellatraRepository

    Friend Const ClanDatabase As String = "ClanDB"
    Friend Const SodDatabase As String = "SoD2DB"
    ''' <summary>CLAN_NOTE_LEN_MAX (ClanService.cpp): o cliente guarda a nota em ClanReport[90].</summary>
    Friend Const MaxNoteLength As Integer = 89
    ''' <summary>A janela da Karina lista os 10 primeiros (ClanService::Sod2Info, indice 3).</summary>
    Friend Const RankingSize As Integer = 10

    Private ReadOnly _config As DatabaseConfig

    Public Sub New(ByVal config As DatabaseConfig)
        _config = config
    End Sub

    Private Function Open(ByVal database As String) As SqlConnection
        Dim connection As New SqlConnection(_config.ConnectionString(database))
        connection.Open()
        Return connection
    End Function

    ''' <summary>Ranking da semana: os 10 clans com Cpoint &gt; 0, na ordem do servidor.</summary>
    Public Function Ranking() As List(Of BellatraClan)
        Dim result As New List(Of BellatraClan)()
        Using connection = Open(ClanDatabase)
            Using command As New SqlCommand(
                "SELECT TOP (@top) c.IDX, c.ClanName, ISNULL(c.Cpoint, 0), ISNULL(c.ClanMoney, 0), CONVERT(varchar(10), c.RegiDate, 120) " &
                "FROM [dbo].[CL] c WHERE ISNULL(c.Cpoint, 0) > 0 ORDER BY c.Cpoint DESC, c.IDX", connection)
                command.Parameters.Add("@top", SqlDbType.Int).Value = RankingSize
                Using reader = command.ExecuteReader()
                    Do While reader.Read()
                        result.Add(New BellatraClan With {
                            .Idx = reader.GetInt32(0),
                            .ClanName = Text(reader, 1),
                            .Points = Convert.ToInt32(reader.GetValue(2)),
                            .ClanMoney = Convert.ToInt64(reader.GetValue(3)),
                            .RegiDate = Text(reader, 4)})
                    Loop
                End Using
            End Using
        End Using
        Return result
    End Function

    ''' <summary>Gerencia da semana (linha de Sod2Config), com o nome e o dinheiro atuais do clan.</summary>
    Public Function Manager() As BellatraManager
        Using connection = Open(SodDatabase)
            Using command As New SqlCommand(
                "SELECT TOP 1 ISNULL(g.MIDX, 0), ISNULL(c.ClanName, ''), ISNULL(c.ClanZang, ''), g.OperFrom, g.OperTo, g.TaxRates, " &
                "ISNULL(g.TodayMoney, 0), ISNULL(g.BeforeMoney, 0), ISNULL(g.TotalMoney, 0), ISNULL(c.ClanMoney, 0), " &
                "ISNULL(CAST(g.Note AS varchar(128)), '') " &
                "FROM [dbo].[Sod2Config] g LEFT JOIN [" & ClanDatabase & "].[dbo].[CL] c ON c.IDX = g.MIDX", connection)
                Using reader = command.ExecuteReader()
                    If Not reader.Read() Then Return New BellatraManager()
                    Return New BellatraManager With {
                        .Exists = True,
                        .Idx = Convert.ToInt32(reader.GetValue(0)),
                        .ClanName = Text(reader, 1),
                        .Leader = Text(reader, 2),
                        .OperFrom = reader.GetDateTime(3),
                        .OperTo = reader.GetDateTime(4),
                        .TaxRates = reader.GetInt32(5),
                        .TodayMoney = Convert.ToInt64(reader.GetValue(6)),
                        .BeforeMoney = Convert.ToInt64(reader.GetValue(7)),
                        .TotalMoney = Convert.ToInt64(reader.GetValue(8)),
                        .ClanMoney = Convert.ToInt64(reader.GetValue(9)),
                        .Note = Text(reader, 10)}
                End Using
            End Using
        End Using
    End Function

    Private Shared Function Text(ByVal reader As SqlDataReader, ByVal index As Integer) As String
        Return If(reader.IsDBNull(index), "", reader.GetValue(index).ToString().Trim())
    End Function

    ''' <summary>Grava a saudacao da Karina (Sod2Config.Note) do gerente. False se o gerente mudou nesse meio tempo.</summary>
    Public Function SaveNote(ByVal managerIdx As Integer, ByVal note As String) As Boolean
        Using connection = Open(SodDatabase)
            Using command As New SqlCommand("UPDATE [dbo].[Sod2Config] SET Note = @note WHERE MIDX = @idx", connection)
                command.Parameters.Add("@note", SqlDbType.VarChar, MaxNoteLength).Value = note
                command.Parameters.Add("@idx", SqlDbType.Int).Value = managerIdx
                Return command.ExecuteNonQuery() > 0
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Registra a taxa gravada no bellatra.ini em Sod2Config.TaxRates e, se mudou, uma linha em Sod2TaxLog (como o site).
    ''' Sem a linha de Sod2Config (servidor ainda nao rodou o tick) nao faz nada; o servidor a cria com a taxa em uso.
    ''' </summary>
    Public Sub SaveTax(ByVal tax As Integer)
        Using connection = Open(SodDatabase)
            Using transaction = connection.BeginTransaction()
                Using command As New SqlCommand(
                    "IF EXISTS (SELECT 1 FROM [dbo].[Sod2Config] WITH (UPDLOCK, HOLDLOCK) WHERE TaxRates <> @tax) BEGIN " &
                    "INSERT INTO [dbo].[Sod2TaxLog] (IDX, MIDX, ClanName, MCount, UserID, CharName, SelServer, TaxRates, OperFrom, OperTo, RegiDate) " &
                    "SELECT (SELECT ISNULL(MAX(IDX), 0) + 1 FROM [dbo].[Sod2TaxLog]), MIDX, ClanName, 0, UserID, CharName, SelServer, @tax, " &
                    "OperFrom, OperTo, GETDATE() FROM [dbo].[Sod2Config]; " &
                    "UPDATE [dbo].[Sod2Config] SET TaxRates = @tax; END", connection, transaction)
                    command.Parameters.Add("@tax", SqlDbType.Int).Value = tax
                    command.ExecuteNonQuery()
                End Using
                transaction.Commit()
            End Using
        End Using
    End Sub

    ''' <summary>Problema que impede gravar a saudacao, ou "" se ok.</summary>
    Public Shared Function ValidateNote(ByVal note As String) As String
        If note.Trim() = "" Then Return "The greeting cannot be empty."
        If note.Length > MaxNoteLength Then Return "The greeting is longer than " & MaxNoteLength & " characters."
        ' ClanService::Sod2Info separa os campos da resposta com '|' e troca o '|' do texto por espaco
        If note.Contains("|") Then Return "The greeting cannot contain '|'."
        Return ""
    End Function

End Class
