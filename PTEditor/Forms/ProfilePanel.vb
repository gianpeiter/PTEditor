Imports System.IO

''' <summary>
''' Aba Options: lista os perfis de Profiles\*.ini, abre os arquivos e testa cada perfil nos arquivos do DataServer
''' (quantos personagens/baus cada um consegue ler).
''' </summary>
Public Class ProfilePanel

    ''' <summary>Os perfis foram recarregados (a aba Characters precisa atualizar o combo).</summary>
    Friend Event ProfilesReloaded()

    Private Sub ProfilePanel_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        lvProfiles.Columns.Add("Profile", 90)
        lvProfiles.Columns.Add("Max level", 65)
        lvProfiles.Columns.Add("Fixes level", 70)
        lvProfiles.Columns.Add("Record code", 80)
        lvProfiles.Columns.Add("Items at", 60)
        lvProfiles.Columns.Add("Test result", 180)
        FillList()
    End Sub

    Private Sub FillList()
        lvProfiles.BeginUpdate()
        lvProfiles.Items.Clear()
        For Each p In ProfileStore.Profiles
            Dim row = lvProfiles.Items.Add(p.Name)
            row.Tag = p.FilePath
            row.SubItems.Add(p.LevelTable.MaxLevel.ToString())
            row.SubItems.Add(If(p.FixLevelFromExp, "Yes", "No"))
            row.SubItems.Add("0x" & p.RecordCode.ToString("X"))
            row.SubItems.Add("0x" & p.DataOffset.ToString("X"))
            row.SubItems.Add("")
        Next
        For Each pair In ProfileStore.Errors
            Dim row = lvProfiles.Items.Add(Path.GetFileName(pair.Key))
            row.Tag = pair.Key
            row.ForeColor = Color.Firebrick
            For i = 1 To 4
                row.SubItems.Add("")
            Next
            row.SubItems.Add("ERROR: " & pair.Value)
        Next
        lvProfiles.EndUpdate()
        lblInfo.Text = ProfileStore.Profiles.Count & " profile(s) in " & ProfileStore.Folder &
                       If(ProfileStore.Errors.Count > 0, " | " & ProfileStore.Errors.Count & " file(s) with errors", "")
    End Sub

    Private Function SelectedFile() As String
        If lvProfiles.SelectedItems.Count = 0 Then Return Nothing
        Return CStr(lvProfiles.SelectedItems(0).Tag)
    End Function

    Private Sub cmdOpenFile_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdOpenFile.Click
        Dim file As String = SelectedFile()
        If file Is Nothing Then
            MsgBox("Select a profile in the list.", MsgBoxStyle.Information)
            Return
        End If
        ProcessHelper.Open(AppState.EditorExe, file)
    End Sub

    Private Sub cmdOpenFolder_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdOpenFolder.Click
        Directory.CreateDirectory(ProfileStore.Folder)
        ProcessHelper.Open(AppState.Explorer, ProfileStore.Folder)
    End Sub

    Private Sub cmdReload_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdReload.Click
        ProfileStore.Reload()
        FillList()
        RaiseEvent ProfilesReloaded()
    End Sub

    ''' <summary>Tenta ler os personagens e baus do DataServer atual com cada perfil, isoladamente.</summary>
    Private Sub cmdTest_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdTest.Click
        Dim folder As String = If(AppState.CharacterIndex Is Nothing, "", AppState.CharacterIndex.DataServerPath)
        If Not CharacterIndex.IsDataServer(folder) Then
            MsgBox("Load a DataServer in the Characters tab first.", MsgBoxStyle.Information)
            Return
        End If
        Cursor = Cursors.WaitCursor
        Try
            Dim chars = HashedFiles(Path.Combine(folder, CharacterIndex.UserDataFolder), "*.dat")
            Dim wars = HashedFiles(Path.Combine(folder, CharacterIndex.WarehouseFolder), "*.war")
            For Each row As ListViewItem In lvProfiles.Items
                Dim profile = ProfileStore.Profiles.FirstOrDefault(Function(p) p.FilePath = CStr(row.Tag))
                If profile Is Nothing Then Continue For
                Dim onlyThis = New List(Of CharacterProfile) From {profile}
                Dim charOk As Integer = chars.Where(Function(f) CanRead(Function() CharacterFile.Load(f, onlyThis))).Count()
                Dim warOk As Integer = wars.Where(Function(f) CanRead(Function() WarehouseFile.Load(f, onlyThis))).Count()
                row.SubItems(5).Text = charOk & "/" & chars.Count & " characters, " & warOk & "/" & wars.Count & " warehouses"
                row.ForeColor = If(charOk > 0 AndAlso charOk = chars.Count, Color.DarkGreen, If(charOk > 0, Color.DarkOrange, SystemColors.GrayText))
            Next
            lblInfo.Text = "Tested on " & folder & " (empty files count as not read)."
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Shared Function CanRead(ByVal load As Func(Of Object)) As Boolean
        Try
            load()
            Return True
        Catch
            Return False
        End Try
    End Function

    Private Shared Function HashedFiles(ByVal folder As String, ByVal pattern As String) As List(Of String)
        Dim result As New List(Of String)()
        If Not Directory.Exists(folder) Then Return result
        For Each subFolder In Directory.GetDirectories(folder)
            Dim number As Integer
            If Integer.TryParse(Path.GetFileName(subFolder), number) AndAlso number >= 0 AndAlso number <= 255 Then
                result.AddRange(Directory.GetFiles(subFolder, pattern))
            End If
        Next
        Return result
    End Function

End Class
