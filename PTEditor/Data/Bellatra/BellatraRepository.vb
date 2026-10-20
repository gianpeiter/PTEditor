Imports System.Data
Imports System.Data.SqlClient

''' <summary>Clan dono do Bellatra: o maior [ClanDB].[dbo].[CL].Cpoint &gt; 0 (desempate pelo IDX).</summary>
Friend Class BellatraOwner
    Public Property Idx As Integer
    Public Property ClanName As String = ""
    ''' <summary>CL.ClanZang: o lider.</summary>
    Public Property Leader As String = ""
    Public Property Points As Integer
    ''' <summary>CL.ClanMoney: o que o lider saca na Karina.</summary>
    Public Property ClanMoney As Long
    ''' <summary>CL.Note: a saudacao que a Karina mostra.</summary>
    Public Property Note As String = ""
End Class

''' <summary>Linha de [SoD2DB].[dbo].[Sod2Config] cujo periodo inclui a data atual.</summary>
Friend Class BellatraPeriodConfig
    Public Property TaxRates As Integer
    ''' <summary>Rendimento (TotalEMoney na Karina).</summary>
    Public Property TotalEnterMoney As Long
    ''' <summary>Recompensas (TotalMoney na Karina).</summary>
    Public Property TotalMoney As Long
    Public Property OperFrom As DateTime
    Public Property OperTo As DateTime
End Class

''' <summary>
''' Dados do Bellatra no banco, com os mesmos criterios do servidor (ClanService::ReadSod2Info, janela da Karina):
''' - clan dono = maior CL.Cpoint &gt; 0 (desempate pelo IDX); a Karina mostra CL.Note como saudacao;
''' - CTax, TotalEMoney e TotalMoney da Karina vem da linha de SoD2DB.Sod2Config cujo periodo (OperFrom..OperTo) inclui a
'''   data atual. A taxa real do evento e a *BELATRA_TAX do bellatra.ini; Sod2Config.TaxRates e so a exibida.
''' Os periodos de Sod2Config e o Cpoint sao da aba Bellatra Admin. Todas as consultas sao parametrizadas.
''' </summary>
Friend NotInheritable Class BellatraRepository

    Friend Const ClanDatabase As String = "ClanDB"
    Friend Const SodDatabase As String = "SoD2DB"
    ''' <summary>CLAN_NOTE_LEN_MAX (ClanService.cpp): o cliente guarda a nota em ClanReport[90].</summary>
    Friend Const MaxNoteLength As Integer = 89

    Private Const CurrentPeriod As String = "GETDATE() BETWEEN OperFrom AND OperTo"

    Private ReadOnly _config As DatabaseConfig

    Public Sub New(ByVal config As DatabaseConfig)
        _config = config
    End Sub

    Private Function Open(ByVal database As String) As SqlConnection
        Dim connection As New SqlConnection(_config.ConnectionString(database))
        connection.Open()
        Return connection
    End Function

    Private Shared Function Text(ByVal reader As SqlDataReader, ByVal index As Integer) As String
        Return If(reader.IsDBNull(index), "", reader.GetValue(index).ToString().Trim())
    End Function

    ''' <summary>Clan dono, ou Nothing se nenhum clan tem Cpoint &gt; 0.</summary>
    Public Function Owner() As BellatraOwner
        Using connection = Open(ClanDatabase)
            Using command As New SqlCommand(
                "SELECT TOP 1 IDX, ClanName, ClanZang, ISNULL(Cpoint, 0), ISNULL(ClanMoney, 0), CAST(Note AS varchar(128)) " &
                "FROM [dbo].[CL] WHERE ISNULL(Cpoint, 0) > 0 ORDER BY Cpoint DESC, IDX", connection)
                Using reader = command.ExecuteReader()
                    If Not reader.Read() Then Return Nothing
                    Return New BellatraOwner With {
                        .Idx = Convert.ToInt32(reader.GetValue(0)),
                        .ClanName = Text(reader, 1),
                        .Leader = Text(reader, 2),
                        .Points = Convert.ToInt32(reader.GetValue(3)),
                        .ClanMoney = Convert.ToInt64(reader.GetValue(4)),
                        .Note = Text(reader, 5)}
                End Using
            End Using
        End Using
    End Function

    ''' <summary>Sod2Config do periodo atual, ou Nothing sem linha (a Karina mostra 0).</summary>
    Public Function CurrentConfig() As BellatraPeriodConfig
        Using connection = Open(SodDatabase)
            Using command As New SqlCommand(
                "SELECT TOP 1 TaxRates, ISNULL(TotalEnterMoney, 0), ISNULL(TotalMoney, 0), OperFrom, OperTo " &
                "FROM [dbo].[Sod2Config] WHERE " & CurrentPeriod & " ORDER BY OperFrom DESC", connection)
                Using reader = command.ExecuteReader()
                    If Not reader.Read() Then Return Nothing
                    Return New BellatraPeriodConfig With {
                        .TaxRates = reader.GetInt32(0),
                        .TotalEnterMoney = Convert.ToInt64(reader.GetValue(1)),
                        .TotalMoney = Convert.ToInt64(reader.GetValue(2)),
                        .OperFrom = reader.GetDateTime(3),
                        .OperTo = reader.GetDateTime(4)}
                End Using
            End Using
        End Using
    End Function

    ''' <summary>Grava a saudacao da Karina (CL.Note) do clan. False se o clan nao existe mais.</summary>
    Public Function SaveNote(ByVal clanIdx As Integer, ByVal note As String) As Boolean
        Using connection = Open(ClanDatabase)
            Using command As New SqlCommand("UPDATE [dbo].[CL] SET Note = @note WHERE IDX = @idx", connection)
                command.Parameters.Add("@note", SqlDbType.VarChar, MaxNoteLength).Value = note
                command.Parameters.Add("@idx", SqlDbType.Int).Value = clanIdx
                Return command.ExecuteNonQuery() > 0
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Copia a taxa gravada no bellatra.ini para Sod2Config.TaxRates do periodo atual (o CTax da Karina).
    ''' False se nao ha periodo atual (os periodos sao criados na aba Bellatra Admin).
    ''' </summary>
    Public Function SaveTax(ByVal tax As Integer) As Boolean
        Using connection = Open(SodDatabase)
            Using command As New SqlCommand(
                "UPDATE g SET TaxRates = @tax, UpdateDate = GETDATE() FROM (SELECT TOP 1 TaxRates, UpdateDate " &
                "FROM [dbo].[Sod2Config] WHERE " & CurrentPeriod & " ORDER BY OperFrom DESC) g", connection)
                command.Parameters.Add("@tax", SqlDbType.Int).Value = tax
                Return command.ExecuteNonQuery() > 0
            End Using
        End Using
    End Function

    ''' <summary>Problema que impede gravar a saudacao, ou "" se ok.</summary>
    Public Shared Function ValidateNote(ByVal note As String) As String
        If note.Trim() = "" Then Return "The greeting cannot be empty."
        If note.Length > MaxNoteLength Then Return "The greeting is longer than " & MaxNoteLength & " characters."
        ' ClanService::ReadSod2Info separa os campos da resposta com '|' e troca o '|' do texto por espaco
        If note.Contains("|") Then Return "The greeting cannot contain '|'."
        ' ClanService::IsClanMember manda a nota numa resposta separada por CR
        If note.Contains(vbCr) OrElse note.Contains(vbLf) Then Return "The greeting must be a single line."
        Return ""
    End Function

End Class
