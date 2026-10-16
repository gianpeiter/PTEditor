Imports System.Data.SqlClient
Imports System.IO

''' <summary>
''' Conexao com o SQL Server lida de &lt;servidor&gt;\server.ini, secao [Database] (Host, User, Pass), a mesma que o
''' Servidor usa (Database\sqlserver.cpp). User vazio = autenticacao do Windows.
''' </summary>
Friend NotInheritable Class DatabaseConfig

    Friend Const FileName As String = "server.ini"
    Private Const Section As String = "Database"

    Public ReadOnly Property FilePath As String
    Public ReadOnly Property Host As String
    Public ReadOnly Property User As String
    Private ReadOnly _password As String

    Private Sub New(ByVal filePath As String, ByVal host As String, ByVal user As String, ByVal password As String)
        Me.FilePath = filePath
        Me.Host = host
        Me.User = user
        _password = password
    End Sub

    ''' <summary>Le o server.ini da pasta do servidor. Lanca excecao com a explicacao se faltar arquivo ou Host.</summary>
    Public Shared Function Load(ByVal serverPath As String) As DatabaseConfig
        Dim path As String = System.IO.Path.Combine(serverPath, FileName)
        If Not File.Exists(path) Then Throw New FileNotFoundException("server.ini not found in the server folder: " & path)
        Dim values = ReadSection(path, Section)
        Dim host As String = Nothing
        If Not values.TryGetValue("Host", host) OrElse host = "" Then
            Throw New InvalidDataException("server.ini has no Host in the [Database] section: " & path)
        End If
        Dim user As String = Nothing
        Dim pass As String = Nothing
        values.TryGetValue("User", user)
        values.TryGetValue("Pass", pass)
        Return New DatabaseConfig(path, host, If(user, ""), If(pass, ""))
    End Function

    Private Shared Function ReadSection(ByVal path As String, ByVal name As String) As Dictionary(Of String, String)
        Dim result As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        Dim inSection As Boolean = False
        For Each raw In File.ReadAllLines(path, AppState.FileEncoding)
            Dim line As String = raw.Trim()
            If line.StartsWith("[") AndAlso line.EndsWith("]") Then
                inSection = line.Substring(1, line.Length - 2).Trim().Equals(name, StringComparison.OrdinalIgnoreCase)
                Continue For
            End If
            If Not inSection OrElse line = "" OrElse line.StartsWith(";") Then Continue For
            Dim equals As Integer = line.IndexOf("="c)
            If equals > 0 Then result(line.Substring(0, equals).Trim()) = line.Substring(equals + 1).Trim()
        Next
        Return result
    End Function

    ''' <summary>Texto para a interface (sem a senha).</summary>
    Public ReadOnly Property Description As String
        Get
            Return Host & "  (" & If(User = "", "Windows login", "user " & User) & ")"
        End Get
    End Property

    Public Function ConnectionString(ByVal database As String) As String
        Dim builder As New SqlConnectionStringBuilder With {
            .DataSource = Host,
            .InitialCatalog = database,
            .ConnectTimeout = 5,
            .ApplicationName = "PTEditor"}
        If User = "" Then
            builder.IntegratedSecurity = True
        Else
            builder.UserID = User
            builder.Password = _password
        End If
        Return builder.ConnectionString
    End Function

End Class
