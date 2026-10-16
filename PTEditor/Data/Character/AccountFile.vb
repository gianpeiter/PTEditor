Imports System.IO

''' <summary>DataServer\userinfo\&lt;n&gt;\&lt;conta&gt;.dat (sPLAY_USER_DATA, 240 bytes): ID e ate 6 personagens.</summary>
Friend Class AccountFile

    Private Const IdOffset As Integer = &H10
    Private Const NamesOffset As Integer = &H30
    Private Const NameLength As Integer = 32
    Friend Const MaxCharacters As Integer = 6

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
        For i = 0 To MaxCharacters - 1
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
