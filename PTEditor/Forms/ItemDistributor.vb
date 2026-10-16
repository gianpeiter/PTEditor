Imports System.IO
Imports System.Text

''' <summary>
''' Envia itens para um personagem via PostBox: le as contas em DataServer\userinfo e grava
''' PostBox\&lt;pasta da conta&gt;\&lt;conta&gt;.dat com uma linha por item.
''' </summary>
Public Class ItemDistributor

    Private Const CharNameOffset As Integer = &H30
    Private Const CharRecordSize As Integer = &H20
    Private Const CharNameSize As Integer = 15

    Private _accountFiles As String() = {}
    Private _accountFile As String

    Private ReadOnly Property UserInfoPath As String
        Get
            Return Path.Combine(AppState.ServerPath, AppState.UserInfoFolder)
        End Get
    End Property

    Private Sub ItemDistributor_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If Directory.Exists(UserInfoPath) Then
            _accountFiles = Directory.GetFiles(UserInfoPath, "*", SearchOption.AllDirectories)
        Else
            MsgBox("Cant find account folder: " & UserInfoPath, MsgBoxStyle.Exclamation)
        End If
    End Sub

    Private Sub ItemDistributor_Activated(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Activated
        cbSpec.Enabled = False
        cbChar.Enabled = False
        lblAccountsFound.Text = _accountFiles.Length.ToString()
        FillAccountList()
    End Sub

    Friend Sub AddItem(ByVal name As String, ByVal code As String)
        lbItemNames.Items.Add(name)
        lbItemCodes.Items.Add(code)
    End Sub

    Private Sub FillAccountList()
        cbAccount.Items.Clear()
        For Each f In _accountFiles
            cbAccount.Items.Add(Path.GetFileNameWithoutExtension(f))
        Next
    End Sub

    Private Sub ClearListToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ClearListToolStripMenuItem.Click
        lbItemNames.Items.Clear()
        lbItemCodes.Items.Clear()
    End Sub

    Private Sub DeleteItemToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DeleteItemToolStripMenuItem.Click
        Dim index As Integer = lbItemCodes.SelectedIndex
        If index < 0 Then Return
        lbItemNames.Items.RemoveAt(index)
        lbItemCodes.Items.RemoveAt(index)
    End Sub

    Private Sub cbAccount_Changed(ByVal sender As Object, ByVal e As EventArgs) Handles cbAccount.TextChanged, cbAccount.SelectedIndexChanged
        cbSpec.Enabled = False
        cbChar.Enabled = False
        cmdSendItems.Enabled = False
    End Sub

    Private Sub lbItemCodes_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles lbItemCodes.SelectedIndexChanged
        lbItemNames.SelectedIndex = lbItemCodes.SelectedIndex
        tbMessage.Text = "Total items weight: " & TotalWeight()
    End Sub

    Private Function TotalWeight() As Integer
        Dim total As Double = 0
        For Each code As Object In lbItemCodes.Items
            Dim item = ItemData.FindByCode("""" & code.ToString().ToUpper() & """")
            Dim weight As Double
            If item IsNot Nothing AndAlso Double.TryParse(item.Weight, weight) Then total = Math.Round(total + weight)
        Next
        Return CInt(total)
    End Function

    Private Sub cmdFindAccount_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdFindAccount.Click
        If cbAccount.Text = "" Then Return
        tbMessage.Text = "Total items weight: " & TotalWeight()
        _accountFile = FindAccountFile(cbAccount.Text)
        If _accountFile Is Nothing Then
            cbChar.Enabled = False
            cbSpec.Enabled = False
            cmdSendItems.Enabled = False
            MsgBox("Cant find the Account!")
            Return
        End If
        cbChar.Enabled = True
        cbSpec.Enabled = True
        cbChar.Items.Clear()
        For Each charName In ReadCharNames(_accountFile)
            cbChar.Items.Add(charName)
        Next
        cmdSendItems.Enabled = True
    End Sub

    Private Function FindAccountFile(ByVal account As String) As String
        Dim wanted As String = account.ToUpper() & ".DAT"
        For Each f In _accountFiles
            If Path.GetFileName(f).ToUpper() = wanted Then Return f
        Next
        Return Nothing
    End Function

    ''' <summary>Nomes dos personagens: a partir do offset 0x30, um nome ASCII de 15 bytes a cada 0x20 bytes.</summary>
    Private Function ReadCharNames(ByVal accountDatFile As String) As List(Of String)
        Dim names As New List(Of String)()
        Dim data As Byte() = File.ReadAllBytes(accountDatFile)
        Dim offset As Integer = CharNameOffset
        Do While offset <= data.Length - &H10
            Dim name As String = Encoding.ASCII.GetString(data, offset, CharNameSize).Replace(ChrW(0), "").Trim()
            If name <> "" Then names.Add(name)
            offset += CharRecordSize
        Loop
        Return names
    End Function

    Private Sub cmdSendItems_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSendItems.Click
        WritePostBox(_accountFile)
    End Sub

    Private Sub WritePostBox(ByVal accountDatFile As String)
        Try
            If cbChar.Text = "" Then
                MsgBox("Please select a Char")
                Return
            End If
            If cbSpec.Text = "" Then
                MsgBox("Please select a Spec")
                Return
            End If
            Dim accountFolder As String = Path.GetFileName(Path.GetDirectoryName(accountDatFile))
            Dim postBoxDir As String = Path.Combine(AppState.ServerPath, AppState.PostBoxFolder, accountFolder)
            Dim postBoxFile As String = Path.Combine(postBoxDir, Path.GetFileName(accountDatFile))
            If File.Exists(postBoxFile) Then
                If Dialogs.Confirm("Warning", "There is a gift alreay in this account, delete it?") Then
                    File.Delete(postBoxFile)
                Else
                    If Dialogs.Confirm("Sending caneled", "Load the data into ItemDistributor?") Then
                        MsgBox("Loading")
                    End If
                    Return
                End If
            End If
            Directory.CreateDirectory(postBoxDir)
            Using writer As New StreamWriter(postBoxFile, True)
                For Each code As Object In lbItemCodes.Items
                    writer.Write(cbChar.Text & " " & code.ToString() & " " & cbSpec.Text.Chars(0) & " " & """" & tbMessage.Text & """" & vbCrLf)
                Next
            End Using
        Catch ex As Exception
            ErrorLog.Report("Write ItemDistributor Data:" & vbCrLf & "Path:" & _accountFile, ex)
        End Try
    End Sub

End Class
