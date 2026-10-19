Imports System.IO

''' <summary>
''' Janela do Add NPC e do Replace NPC (mesma janela; o Replace recebe o .NPC atual do slot): escolhe o NPC de origem (lista com nomes ou Browse na pasta GameServer\NPC) e se ele sera usado como esta
''' ou copiado para um NPC novo (novo .NPC e .zhoon). Valida na hora: nome, tamanho do caminho (campo de 64 bytes do .spc)
''' e arquivo ja existente.
''' </summary>
Friend Class AddNpcDialog
    Inherits Form

    ''' <summary>"GameServer\npc\" + arquivo: o campo do .spc tem 64 bytes.</summary>
    Private Const SetupFolderPrefix As String = "GameServer\npc\"
    Private Const ExternalLabel As String = "(file chosen with Browse)"

    ''' <summary>Texto da lista -> caminho completo do .NPC.</summary>
    Private ReadOnly _pathByLabel As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
    Private _externalPath As String = ""
    Private _nameEdited As Boolean

    Private ReadOnly cbSource As New ComboBox()
    Private ReadOnly btnBrowse As New Button()
    Private ReadOnly rbCopy As New RadioButton()
    Private ReadOnly rbUse As New RadioButton()
    Private ReadOnly lblName As New Label()
    Private ReadOnly tbName As New TextBox()
    Private ReadOnly lblInfo As New Label()
    Private ReadOnly lblMessage As New Label()
    Private ReadOnly btnOk As New Button()
    Private ReadOnly btnCancel As New Button()

    ''' <summary>True = criar um NPC novo copiando a origem; False = usar a origem como esta (sem criar arquivos).</summary>
    Friend ReadOnly Property CreateCopy As Boolean
        Get
            Return rbCopy.Checked
        End Get
    End Property

    ''' <summary>Caminho completo do .NPC escolhido ("" se nenhum valido).</summary>
    Friend ReadOnly Property SourcePath As String
        Get
            Dim found As String = ""
            If cbSource.Text = ExternalLabel Then Return _externalPath
            Return If(_pathByLabel.TryGetValue(cbSource.Text, found), found, "")
        End Get
    End Property

    ''' <summary>Nome do .NPC novo (sem extensao); so vale com CreateCopy.</summary>
    Friend ReadOnly Property NewName As String
        Get
            Return tbName.Text.Trim()
        End Get
    End Property

    ''' <summary>replaceFile = nome do .NPC atual do slot (modo Replace: ele vem selecionado e "usar como esta" e o padrao); Nothing = Add.</summary>
    Friend Sub New(ByVal npcs As IEnumerable(Of NpcFile), ByVal positionText As String, Optional ByVal replaceFile As String = Nothing)
        Dim replacing As Boolean = replaceFile IsNot Nothing
        Text = If(replacing, "Replace NPC", "Add NPC") & If(positionText <> "", " - " & positionText, "")
        AutoScaleMode = AutoScaleMode.None
        FormBorderStyle = FormBorderStyle.FixedDialog
        StartPosition = FormStartPosition.CenterParent
        MaximizeBox = False
        MinimizeBox = False
        ShowInTaskbar = False
        ClientSize = New Size(560, 268)
        AcceptButton = btnOk
        CancelButton = btnCancel

        Dim caption As New Label() With {.Text = If(replacing, "NPC to put in this slot (pick from the list or use Browse):", "NPC to place (pick from the list or use Browse):"), .AutoSize = True, .Location = New Point(12, 12)}
        cbSource.Location = New Point(12, 32)
        cbSource.Size = New Size(430, 21)
        cbSource.DropDownStyle = ComboBoxStyle.DropDown
        cbSource.AutoCompleteMode = AutoCompleteMode.SuggestAppend
        cbSource.AutoCompleteSource = AutoCompleteSource.ListItems
        btnBrowse.Text = "Browse..."
        btnBrowse.Location = New Point(450, 30)
        btnBrowse.Size = New Size(98, 24)

        rbCopy.Text = "Create a new NPC from it (new .NPC and .zhoon files)"
        rbCopy.Checked = True
        rbCopy.AutoSize = True
        rbCopy.Location = New Point(12, 68)
        lblName.Text = "New NPC name:"
        lblName.AutoSize = True
        lblName.Location = New Point(32, 96)
        tbName.Location = New Point(130, 92)
        tbName.Size = New Size(300, 20)
        lblInfo.Location = New Point(32, 118)
        lblInfo.Size = New Size(516, 30)
        lblInfo.ForeColor = SystemColors.GrayText

        rbUse.Text = "Use it as is (no files created: this NPC shares the .NPC and the dialog of the original)"
        rbUse.AutoSize = True
        rbUse.Location = New Point(12, 160)

        lblMessage.Location = New Point(12, 192)
        lblMessage.Size = New Size(536, 34)
        lblMessage.ForeColor = Color.Firebrick

        btnOk.Text = "OK"
        btnOk.DialogResult = DialogResult.OK
        btnOk.Location = New Point(380, 228)
        btnOk.Size = New Size(80, 26)
        btnCancel.Text = "Cancel"
        btnCancel.DialogResult = DialogResult.Cancel
        btnCancel.Location = New Point(468, 228)
        btnCancel.Size = New Size(80, 26)

        Controls.AddRange(New Control() {caption, cbSource, btnBrowse, rbCopy, lblName, tbName, lblInfo, rbUse, lblMessage, btnOk, btnCancel})

        For Each npc In npcs.OrderBy(Function(n) n.Name, StringComparer.OrdinalIgnoreCase).ThenBy(Function(n) n.FileName, StringComparer.OrdinalIgnoreCase)
            Dim label As String = If(npc.Name = "" OrElse npc.Name = NpcData.NoNameFound OrElse npc.Name = NpcData.NoZhoonFound,
                                     npc.FileName, npc.Name & "  (" & npc.FileName & ")")
            If Not _pathByLabel.ContainsKey(label) Then
                _pathByLabel(label) = AppState.NpcPath(npc.FileName)
                cbSource.Items.Add(label)
            End If
        Next

        If replacing Then
            For Each pair In _pathByLabel
                If Path.GetFileName(pair.Value).Equals(replaceFile, StringComparison.OrdinalIgnoreCase) Then
                    cbSource.Text = pair.Key
                    Exit For
                End If
            Next
            rbUse.Checked = True
        End If
        SuggestName()

        AddHandler cbSource.TextChanged, AddressOf OnSourceChanged
        AddHandler cbSource.SelectedIndexChanged, AddressOf OnSourceChanged
        AddHandler tbName.TextChanged, Sub(s, e)
                                            If tbName.Focused Then _nameEdited = True
                                            Revalidate()
                                        End Sub
        AddHandler rbCopy.CheckedChanged, AddressOf OnModeChanged
        AddHandler btnBrowse.Click, AddressOf OnBrowse
        Revalidate()
    End Sub

    ''' <summary>Sugere "&lt;origem&gt;-new" como nome do NPC novo, enquanto o usuario nao digitou outro.</summary>
    Private Sub SuggestName()
        If _nameEdited Then Return
        Dim source As String = SourcePath
        tbName.Text = If(source = "", "", Path.GetFileNameWithoutExtension(source) & "-new")
    End Sub

    Private Sub OnSourceChanged(ByVal sender As Object, ByVal e As EventArgs)
        SuggestName()
        Revalidate()
    End Sub

    Private Sub OnModeChanged(ByVal sender As Object, ByVal e As EventArgs)
        Revalidate()
    End Sub

    ''' <summary>Abre a pasta GameServer\NPC para escolher o .NPC; se for um da lista, seleciona-o la.</summary>
    Private Sub OnBrowse(ByVal sender As Object, ByVal e As EventArgs)
        Using dialog As New OpenFileDialog()
            dialog.Title = "Select the NPC (.NPC)"
            dialog.Filter = "NPC files (*.NPC)|*.NPC|All files|*.*"
            Dim folder As String = AppState.ServerPath & AppState.NpcFolder
            If Directory.Exists(folder) Then dialog.InitialDirectory = folder
            If dialog.ShowDialog(Me) <> DialogResult.OK Then Return
            For Each pair In _pathByLabel
                If pair.Value.Equals(dialog.FileName, StringComparison.OrdinalIgnoreCase) Then
                    cbSource.Text = pair.Key
                    Return
                End If
            Next
            _externalPath = dialog.FileName
            cbSource.Text = ExternalLabel
        End Using
    End Sub

    Private Sub Revalidate()
        Dim problem As String = ""
        Dim source As String = SourcePath
        tbName.Enabled = rbCopy.Checked
        lblName.Enabled = rbCopy.Checked
        lblInfo.Text = If(rbCopy.Checked, "Creates " & SetupFolderPrefix & If(NewName = "", "<name>", NewName) & ".NPC and " &
                          AppState.NpcFolder & "name\" & If(NewName = "", "<name>", NewName) & ".zhoon", "")

        If source = "" OrElse Not File.Exists(source) Then
            problem = "Pick an NPC from the list, or use Browse."
        ElseIf rbCopy.Checked Then
            If NewName = "" Then
                problem = "Enter a name for the new NPC."
            ElseIf NewName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 Then
                problem = "The name has characters that are not allowed in a file name."
            ElseIf (SetupFolderPrefix & NewName & ".NPC").Length > NpcData.TextFieldSize Then
                problem = "The name is too long: the NPC path must fit in " & NpcData.TextFieldSize & " characters."
            ElseIf File.Exists(AppState.NpcPath(NewName & ".NPC")) Then
                problem = NewName & ".NPC already exists. Choose another name, or pick it in the list and use it as is."
            End If
        Else
            If Not Path.GetDirectoryName(source).TrimEnd("\"c).Equals((AppState.ServerPath & AppState.NpcFolder).TrimEnd("\"c), StringComparison.OrdinalIgnoreCase) Then
                problem = "To use it as is, the .NPC must be inside " & AppState.NpcFolder & "."
            ElseIf (SetupFolderPrefix & Path.GetFileName(source)).Length > NpcData.TextFieldSize Then
                problem = "The file name is too long: the NPC path must fit in " & NpcData.TextFieldSize & " characters."
            End If
        End If
        lblMessage.Text = problem
        btnOk.Enabled = problem = ""
    End Sub

End Class
