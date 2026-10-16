Imports System.IO

''' <summary>
''' DataServer\userinfo\&lt;n&gt;\&lt;conta&gt;.dat (sPLAY_USER_DATA): cabecalho de 16 bytes, ID e N nomes de 32 bytes.
''' N depende da source (6 = 240 bytes; 11 no perfil Custom = 400 bytes) e e deduzido do tamanho do arquivo.
''' </summary>
Friend Class AccountFile

    Private Const IdOffset As Integer = &H10
    Private Const NamesOffset As Integer = &H30
    Private Const NameLength As Integer = 32

    Public ReadOnly Property FilePath As String
    Public ReadOnly Property Id As String
    Public ReadOnly Property CharacterNames As New List(Of String)()

    Private Sub New(ByVal filePath As String, ByVal id As String)
        Me.FilePath = filePath
        Me.Id = id
    End Sub

    Public Shared Function Load(ByVal filePath As String) As AccountFile
        Dim data As Byte() = File.ReadAllBytes(filePath)
        Dim id As String = ReadText(data, IdOffset)
        If id = "" Then id = Path.GetFileNameWithoutExtension(filePath)
        Dim account As New AccountFile(filePath, id)
        Dim slots As Integer = Math.Max(0, (data.Length - NamesOffset) \ NameLength)
        For i = 0 To slots - 1
            Dim name As String = ReadText(data, NamesOffset + i * NameLength)
            If name <> "" Then account.CharacterNames.Add(name)
        Next
        Return account
    End Function

    Private Shared Function ReadText(ByVal data As Byte(), ByVal offset As Integer) As String
        If offset >= data.Length Then Return ""
        Dim length As Integer = 0
        Do While length < NameLength AndAlso offset + length < data.Length AndAlso data(offset + length) <> 0
            length += 1
        Loop
        Return AppState.FileEncoding.GetString(data, offset, length).Trim()
    End Function

End Class
