Imports System.IO

''' <summary>
''' Aba Characters: lista contas/personagens do DataServer, mostra ficha, inventario, bau e skills e edita
''' level/EXP, atributos, ouro e remocao de itens. Toda leitura/gravacao fica em Data\Character.
''' </summary>
Public Class CharacterEditor

    Private Const AllClasses As String = "All classes"

    ''' <summary>Um personagem foi gravado (o Item Finder precisa reler os itens).</summary>
    Friend Event CharacterSaved()

    ''' <summary>Arquivo como esta no disco (para comparar) e a copia que recebe as edicoes.</summary>
    Private _original As CharacterFile
    Private _working As CharacterFile
    Private _entry As CharacterEntry
    Private _loadedWriteTime As DateTime
    ''' <summary>Evita que o preenchimento dos campos dispare os eventos de edicao.</summary>
    Private _filling As Boolean
    Private _initialized As Boolean

    Private _setupDone As Boolean

    Private Sub CharacterEditor_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        EnsureSetup()
    End Sub

    ''' <summary>Colunas, filtros e perfis. A aba pode chamar EnsureLoaded antes do evento Load do controle.</summary>
    Private Sub EnsureSetup()
        If _setupDone Then Return
        _setupDone = True
        SetupItemColumns(lvInventory, True)
        SetupItemColumns(lvWarehouse, False)
        cbClassFilter.Items.Add(AllClasses)
        For Each pair In GameTables.ClassNames
            cbClassFilter.Items.Add(pair.Value)
        Next
        cbClassFilter.SelectedIndex = 0
        FillProfiles()
        ShowCharacter(Nothing)
    End Sub

    Private Shared Sub SetupItemColumns(ByVal list As ListView, ByVal withPlace As Boolean)
        list.Columns.Clear()
        list.Columns.Add("Slot", 40)
        If withPlace Then list.Columns.Add("Place", 110)
        list.Columns.Add("Code", 60)
        list.Columns.Add("Name", 170)
        list.Columns.Add("Name in item", 130)
        list.Columns.Add("Aging", 45)
        list.Columns.Add("Durab.", 60)
        list.Columns.Add("Head", 75)
        list.Columns.Add("CheckSum", 75)
        list.Columns.Add("Status", 200)
    End Sub

#Region "Carga do DataServer"

    ''' <summary>Chamado quando a aba e aberta: carrega na primeira vez.</summary>
    Friend Sub EnsureLoaded()
        If _initialized Then Return
        _initialized = True
        EnsureSetup()
        tbDataServer.Text = DefaultDataServerPath()
        SelectProfileFor(tbDataServer.Text)
        If CharacterIndex.ResolveDataServer(tbDataServer.Text) IsNot Nothing Then
            LoadIndex()
        Else
            lblStatus.Text = "Select the DataServer folder and click Load."
        End If
    End Sub

    ''' <summary>Indice atual, carregando se necessario (usado pelo Item Finder).</summary>
    Friend Function GetIndex() As CharacterIndex
        EnsureLoaded()
        Return AppState.CharacterIndex
    End Function

    Private Shared Function DefaultDataServerPath() As String
        If AppState.DataServerPath <> "" Then Return AppState.DataServerPath
        Return ServerDataServerPath()
    End Function

    ''' <summary>DataServer dentro do Server Path atual (subpasta DataServer* valida, ou &lt;ServerPath&gt;\DataServer).</summary>
    Private Shared Function ServerDataServerPath() As String
        If AppState.ServerPath = "" Then Return ""
        Return If(CharacterIndex.ResolveDataServer(AppState.ServerPath), Path.Combine(AppState.ServerPath, AppState.DataServerFolder))
    End Function

    ''' <summary>
    ''' O Server Path mudou: descarta a pasta DataServer escolhida a mao (era do servidor anterior) e volta a seguir o
    ''' Server Path. Se a aba ja foi aberta, recarrega a partir da nova pasta.
    ''' </summary>
    Friend Sub FollowServerPath()
        SaveDataServerOverride("")
        If Not _initialized Then Return
        tbDataServer.Text = DefaultDataServerPath()
        SelectProfileFor(tbDataServer.Text)
        If CharacterIndex.ResolveDataServer(tbDataServer.Text) IsNot Nothing Then
            LoadIndex()
        Else
            lblStatus.Text = "Select the DataServer folder and click Load."
        End If
    End Sub

    ''' <summary>Grava a pasta DataServer escolhida; vazio = seguir o Server Path.</summary>
    Private Shared Sub SaveDataServerOverride(ByVal folder As String)
        If String.Equals(AppState.DataServerPath, folder, StringComparison.OrdinalIgnoreCase) Then Return
        AppState.DataServerPath = folder
        ConfigFile.SaveValues(New Dictionary(Of String, String) From {{ConfigFile.KeyDataServerPath, folder}})
    End Sub

    Private Sub cmdBrowse_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdBrowse.Click
        dlgFolder.Description = "Select the DataServer folder (the one containing userdata, userinfo and warehouse)"
        If Directory.Exists(tbDataServer.Text) Then dlgFolder.SelectedPath = tbDataServer.Text
        If dlgFolder.ShowDialog() = DialogResult.OK Then
            tbDataServer.Text = dlgFolder.SelectedPath
            SelectProfileFor(tbDataServer.Text)
            LoadIndex()
        End If
    End Sub

    Private Sub cmdLoad_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdLoad.Click
        LoadIndex()
    End Sub

    Private Sub LoadIndex()
        If Not ConfirmDiscard() Then Return
        Dim typed As String = tbDataServer.Text.Trim()
        Dim folder As String = CharacterIndex.ResolveDataServer(typed)
        If folder Is Nothing Then
            MsgBox("No DataServer found in this folder." & vbCrLf & typed & vbCrLf & vbCrLf &
                   "Select the DataServer folder (the one containing userdata, userinfo and warehouse) " &
                   "or the server folder that contains it.", MsgBoxStyle.Exclamation)
            Return
        End If
        If folder <> typed Then
            tbDataServer.Text = folder
            SelectProfileFor(folder)
        End If
        If ProfileStore.Profiles.Count = 0 Then
            MsgBox("No character profile found in " & ProfileStore.Folder & "." & vbCrLf &
                   "The profiles (*.ini) describe the file format of each server source.", MsgBoxStyle.Exclamation)
            Return
        End If
        Dim chosen As String = SelectedProfileName()
        Dim candidates As List(Of CharacterProfile) =
            If(chosen = ProfileStore.AutoDetect, ProfileStore.Profiles, New List(Of CharacterProfile) From {ProfileStore.Find(chosen)})
        Cursor = Cursors.WaitCursor
        Try
            Dim index = CharacterIndex.Build(folder, candidates, Sub(i, n)
                                                                     If i Mod 25 = 0 OrElse i = n Then
                                                                         lblStatus.Text = "Reading characters: " & i & " of " & n
                                                                         lblStatus.Refresh()
                                                                     End If
                                                                 End Sub)
            AppState.CharacterIndex = index
            ' So guarda a pasta quando difere da derivada do Server Path; assim ela acompanha mudancas do Server Path
            Dim followsServer As Boolean = String.Equals(folder.TrimEnd("\"c), ServerDataServerPath().TrimEnd("\"c), StringComparison.OrdinalIgnoreCase)
            SaveDataServerOverride(If(followsServer, "", folder))
            ProfileStore.SaveSelection(folder, chosen)
            Dim failed As Integer = index.Characters.Where(Function(c) c.LoadError <> "").Count()
            lblStatus.Text = index.Characters.Count & " characters, " & index.Accounts.Count & " accounts" &
                             If(failed > 0, " | " & failed & " file(s) could not be read (marked with !)", "")
            ShowDetectedProfiles(index, chosen)
            ShowCharacter(Nothing)
            FillTree()
        Catch ex As Exception
            lblStatus.Text = "Error reading the DataServer"
            ErrorLog.Report("CharacterEditor.LoadIndex", ex)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

#End Region

#Region "Perfis"

    Private _fillingProfiles As Boolean

    ''' <summary>Combo de perfis: "Auto-detect" + perfis de Profiles\*.ini.</summary>
    Friend Sub FillProfiles()
        _fillingProfiles = True
        Dim current As String = SelectedProfileName()
        cbProfile.Items.Clear()
        cbProfile.Items.Add(AutoDetectText)
        For Each p In ProfileStore.Profiles
            cbProfile.Items.Add(p.Name)
        Next
        SelectProfileName(current)
        _fillingProfiles = False
    End Sub

    Private Const AutoDetectText As String = "Auto-detect"

    Private Function SelectedProfileName() As String
        If cbProfile.SelectedIndex <= 0 Then Return ProfileStore.AutoDetect
        Return CStr(cbProfile.SelectedItem)
    End Function

    Private Sub SelectProfileName(ByVal name As String)
        Dim index As Integer = If(name = ProfileStore.AutoDetect, -1, cbProfile.Items.IndexOf(name))
        cbProfile.SelectedIndex = Math.Max(0, index)
    End Sub

    ''' <summary>Seleciona o perfil gravado para a pasta (ou Auto-detect).</summary>
    Private Sub SelectProfileFor(ByVal folder As String)
        _fillingProfiles = True
        SelectProfileName(ProfileStore.SelectionFor(folder))
        _fillingProfiles = False
    End Sub

    Private Sub cbProfile_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cbProfile.SelectedIndexChanged
        If _fillingProfiles OrElse AppState.CharacterIndex Is Nothing Then Return
        LoadIndex()
    End Sub

    ''' <summary>Resultado da deteccao: qual perfil leu quantos arquivos.</summary>
    Private Sub ShowDetectedProfiles(ByVal index As CharacterIndex, ByVal chosen As String)
        Dim counts = index.ProfileCounts()
        Dim read As Integer = counts.Sum(Function(p) p.Value)
        Dim total As Integer = index.Characters.Count
        Dim files As String = " (" & read & "/" & total & " files)"
        If total = 0 Then
            lblDetected.Text = "No character files in userdata."
            lblDetected.ForeColor = SystemColors.GrayText
        ElseIf read = 0 Then
            lblDetected.Text = If(chosen = ProfileStore.AutoDetect, "No profile matches these files", "Profile " & chosen & " does not match these files") &
                               files & " - read-only"
            lblDetected.ForeColor = Color.Firebrick
        ElseIf counts.Count = 1 Then
            lblDetected.Text = If(chosen = ProfileStore.AutoDetect, "Detected: ", "Profile: ") & counts(0).Key & files
            lblDetected.ForeColor = If(read = total, Color.DarkGreen, Color.DarkOrange)
        Else
            lblDetected.Text = "Detected: " & String.Join(", ", counts.Select(Function(p) p.Key & " " & p.Value)) & files &
                               " - mixed formats in this DataServer"
            lblDetected.ForeColor = Color.DarkOrange
        End If
        Dim broken As Integer = ProfileStore.Errors.Count
        If broken > 0 Then lblDetected.Text &= " | " & broken & " profile file(s) with errors (see Options)"
    End Sub

#End Region

#Region "Lista e filtros"

    Private Sub Filters_Changed(ByVal sender As Object, ByVal e As EventArgs) Handles tbSearch.TextChanged,
            cbClassFilter.SelectedIndexChanged, nudLevelMin.ValueChanged, nudLevelMax.ValueChanged
        FillTree()
    End Sub

    Private Function MatchesFilter(ByVal entry As CharacterEntry) As Boolean
        Dim text As String = tbSearch.Text.Trim()
        If text <> "" AndAlso
           entry.Name.IndexOf(text, StringComparison.OrdinalIgnoreCase) < 0 AndAlso
           entry.Account.IndexOf(text, StringComparison.OrdinalIgnoreCase) < 0 Then Return False
        If cbClassFilter.SelectedIndex > 0 AndAlso entry.ClassName <> CStr(cbClassFilter.SelectedItem) Then Return False
        If entry.LoadError = "" AndAlso (entry.Level < nudLevelMin.Value OrElse entry.Level > nudLevelMax.Value) Then Return False
        Return True
    End Function

    ''' <summary>Arvore Conta -> Personagens com os filtros aplicados.</summary>
    Private Sub FillTree()
        Dim index = AppState.CharacterIndex
        tvCharacters.BeginUpdate()
        tvCharacters.Nodes.Clear()
        If index Is Nothing Then
            tvCharacters.EndUpdate()
            lblCount.Text = ""
            Return
        End If
        Dim shown As Integer = 0
        Dim groups = index.Characters.Where(AddressOf MatchesFilter).
            GroupBy(Function(c) c.Account, StringComparer.OrdinalIgnoreCase).
            OrderBy(Function(g) If(g.Key = CharacterIndex.NoAccount, "~", g.Key), StringComparer.OrdinalIgnoreCase)
        For Each group In groups
            Dim accountNode = tvCharacters.Nodes.Add(group.Key & "  (" & group.Count() & ")")
            For Each entry In group
                Dim node = accountNode.Nodes.Add(NodeText(entry))
                node.Tag = entry
                If entry.LoadError <> "" Then node.ForeColor = Color.Firebrick
                If _entry IsNot Nothing AndAlso entry.FilePath = _entry.FilePath Then tvCharacters.SelectedNode = node
                shown += 1
            Next
        Next
        If tbSearch.Text.Trim() <> "" OrElse tvCharacters.Nodes.Count <= 20 Then tvCharacters.ExpandAll()
        tvCharacters.EndUpdate()
        lblCount.Text = " " & shown & " of " & index.Characters.Count & " characters"
    End Sub

    Private Shared Function NodeText(ByVal entry As CharacterEntry) As String
        If entry.LoadError <> "" Then Return "! " & entry.Name & "  (" & entry.LoadError & ")"
        Return entry.Name & "  -  " & entry.ClassName & " Lv " & entry.Level
    End Function

    Private Sub tvCharacters_BeforeSelect(ByVal sender As Object, ByVal e As TreeViewCancelEventArgs) Handles tvCharacters.BeforeSelect
        If e.Node.Tag Is Nothing OrElse _entry Is Nothing Then Return
        If DirectCast(e.Node.Tag, CharacterEntry).FilePath <> _entry.FilePath AndAlso Not ConfirmDiscard() Then e.Cancel = True
    End Sub

    Private Sub tvCharacters_AfterSelect(ByVal sender As Object, ByVal e As TreeViewEventArgs) Handles tvCharacters.AfterSelect
        Dim entry = TryCast(e.Node.Tag, CharacterEntry)
        If entry Is Nothing OrElse (_entry IsNot Nothing AndAlso entry.FilePath = _entry.FilePath) Then Return
        OpenCharacter(entry)
    End Sub

    ''' <summary>Seleciona um personagem pelo nome (usado pelo Item Finder). showWarehouse abre a aba Bau.</summary>
    Friend Sub SelectCharacter(ByVal name As String, ByVal showWarehouse As Boolean)
        EnsureLoaded()
        If AppState.CharacterIndex Is Nothing Then Return
        tbSearch.Text = ""
        cbClassFilter.SelectedIndex = 0
        nudLevelMin.Value = 0
        nudLevelMax.Value = nudLevelMax.Maximum
        For Each accountNode As TreeNode In tvCharacters.Nodes
            For Each node As TreeNode In accountNode.Nodes
                Dim entry = DirectCast(node.Tag, CharacterEntry)
                If entry.Name.Equals(name, StringComparison.OrdinalIgnoreCase) Then
                    tvCharacters.SelectedNode = node
                    node.EnsureVisible()
                    If showWarehouse Then tabs.SelectedTab = tpWarehouse
                    Return
                End If
            Next
        Next
    End Sub

#End Region

#Region "Exibicao"

    Private Sub OpenCharacter(ByVal entry As CharacterEntry)
        Try
            If entry.LoadError <> "" Then
                ShowCharacter(Nothing)
                lblTitle.Text = entry.Name
                lblSubtitle.Text = entry.FilePath
                ShowMessages({New RuleMessage(RuleSeverity.Error, "File cannot be read: " & entry.LoadError)})
                _entry = entry
                cmdOpenFolder.Enabled = True
                Return
            End If
            _original = CharacterFile.Load(entry.FilePath, AppState.CharacterIndex.Candidates)
            _working = CharacterFile.Load(entry.FilePath, AppState.CharacterIndex.Candidates)
            _loadedWriteTime = File.GetLastWriteTime(entry.FilePath)
            _entry = entry
            ShowCharacter(_working)
        Catch ex As Exception
            ShowCharacter(Nothing)
            ShowMessages({New RuleMessage(RuleSeverity.Error, "Error opening " & entry.FilePath & ": " & ex.Message)})
        End Try
    End Sub

    ''' <summary>Preenche todas as abas a partir do arquivo (Nothing limpa a tela).</summary>
    Private Sub ShowCharacter(ByVal c As CharacterFile)
        _filling = True
        Try
            Dim hasChar As Boolean = c IsNot Nothing
            tabs.Enabled = hasChar
            cmdOpenFolder.Enabled = hasChar
            If Not hasChar Then
                _original = Nothing
                _working = Nothing
                _entry = Nothing
                lblTitle.Text = "Select a character"
                lblSubtitle.Text = ""
                lvGeneral.Items.Clear()
                lvInventory.Items.Clear()
                lvWarehouse.Items.Clear()
                lvSkills.Items.Clear()
                lvMessages.Items.Clear()
                lblWarehouse.Text = ""
                lblInventory.Text = ""
                UpdateDirtyState()
                Return
            End If

            lblTitle.Text = c.Name & "  -  " & c.ClassName & " Lv " & c.Level
            lblSubtitle.Text = c.FilePath & "   |   profile " & c.Profile.Name & ", header '" & c.Header & "'"
            FillGeneral(c)
            nudLevel.Maximum = c.Profile.LevelTable.MaxLevel
            lblLevelNote.Text = LevelNote(c.Profile)
            nudLevel.Value = Math.Max(nudLevel.Minimum, Math.Min(nudLevel.Maximum, c.Level))
            tbExp.Text = c.Exp.ToString()
            nudStrength.Value = Clamp(nudStrength, c.Strength)
            nudSpirit.Value = Clamp(nudSpirit, c.Spirit)
            nudTalent.Value = Clamp(nudTalent, c.Talent)
            nudDexterity.Value = Clamp(nudDexterity, c.Dexterity)
            nudHealth.Value = Clamp(nudHealth, c.Health)
            nudMoney.Value = Clamp(nudMoney, c.EffectiveMoney)
            lblGoldNote.Text = If(c.LastMoney > 0,
                "The file has LastMoeny = " & c.LastMoney.ToString("N0") & ": the game uses this value - 1 as gold (stored Money: " &
                c.Money.ToString("N0") & "). On save, gold is written to Money and LastMoeny is set to 0.",
                "On save, gold is written to Money and LastMoeny is set to 0 (if LastMoeny > 0 the game would use it instead).")
            FillInventory(c)
            FillWarehouse(c)
            FillSkills(c)
        Finally
            _filling = False
        End Try
        RefreshDerived()
    End Sub

    Private Shared Function LevelNote(ByVal profile As CharacterProfile) As String
        Dim text As String = "Changing the level moves the EXP to the start of the new level (" & profile.Name &
                             " table, max level " & profile.LevelTable.MaxLevel & "). "
        If profile.FixLevelFromExp Then
            text &= "This server checks Level x EXP when loading the character and fixes the level if they do not match."
        Else
            text &= "This server does not check Level x EXP: the level is kept even if the EXP is outside its range."
        End If
        Return text & " Free stat points are recalculated on save."
    End Function

    Private Shared Function Clamp(ByVal box As NumericUpDown, ByVal value As Integer) As Decimal
        Return Math.Max(box.Minimum, Math.Min(box.Maximum, value))
    End Function

    Private Sub FillGeneral(ByVal c As CharacterFile)
        lvGeneral.BeginUpdate()
        lvGeneral.Items.Clear()
        AddField("Name", c.Name)
        AddField("Class", c.ClassName & " (JOB_CODE " & c.JobCode & ")" & If(c.ChangeJob > 0, ", class change " & c.ChangeJob, ""))
        AddField("Account (userinfo)", If(_entry Is Nothing, "", _entry.Account))
        AddField("Account stored in character", c.MasterId)
        If _entry IsNot Nothing AndAlso c.MasterId <> "" AndAlso _entry.Account <> CharacterIndex.NoAccount AndAlso
           Not c.MasterId.Equals(_entry.Account, StringComparison.OrdinalIgnoreCase) Then
            AddField("Warning", "Stored account differs from the account that lists this character (the server writes a hack log entry)")
        End If
        AddField("Level", c.Level.ToString())
        AddField("EXP", c.Exp.ToString("N0"))
        AddField("Gold", c.EffectiveMoney.ToString("N0"))
        AddField("HP / MP / STM", c.Life & "   " & c.Mana & "   " & c.Stamina)
        AddField("Weight", c.Weight)
        AddField("Map", GameTables.MapName(c.MapCode))
        AddField("Position (X, Z)", c.PosX & ", " & c.PosZ)
        AddField("Items", c.Items.Count.ToString())
        AddField("Folder (name hash)", "userdata\" & UserCode.Compute(c.Name))
        AddField("Save version (Head)", c.SaveHead.ToString())
        AddField("Profile", c.Profile.Name & "  (" & c.Profile.FilePath & ")")
        AddField("File", c.FilePath)
        lvGeneral.EndUpdate()
    End Sub

    Private Sub AddField(ByVal name As String, ByVal value As String)
        lvGeneral.Items.Add(name).SubItems.Add(value)
    End Sub

    Private Sub FillInventory(ByVal c As CharacterFile)
        lvInventory.BeginUpdate()
        lvInventory.Items.Clear()
        For Each item In c.Items.OrderBy(Function(i) i.SecondInventory).ThenBy(Function(i) If(i.Position = 0, 100, i.Position)).ThenBy(Function(i) i.Slot)
            lvInventory.Items.Add(ItemRow(item, True))
        Next
        lvInventory.EndUpdate()
        Dim problems As Integer = c.Items.Where(Function(i) i.HasProblem).Count()
        lblInventory.Text = c.Items.Count & " items" & If(problems > 0, " | " & problems & " with checksum problems", "")
        lblInventory.ForeColor = If(problems > 0, Color.Firebrick, SystemColors.ControlText)
    End Sub

    Private Shared Function ItemRow(ByVal item As GameItem, ByVal withPlace As Boolean) As ListViewItem
        Dim row As New ListViewItem(item.Slot.ToString()) With {.Tag = item}
        If withPlace Then row.SubItems.Add(item.PlaceName)
        row.SubItems.Add(item.CodeText)
        row.SubItems.Add(item.DisplayName)
        row.SubItems.Add(item.StoredName)
        row.SubItems.Add(If(item.Aging > 0, "+" & item.Aging, ""))
        row.SubItems.Add(item.Durability)
        row.SubItems.Add(item.Head.ToString("X8"))
        row.SubItems.Add(item.ChkSum.ToString("X8"))
        row.SubItems.Add(item.Status)
        If item.HasProblem Then
            row.ForeColor = Color.Firebrick
        ElseIf item.IsTwoHandedCopy Then
            row.ForeColor = SystemColors.GrayText
        End If
        Return row
    End Function

    Private Sub FillWarehouse(ByVal c As CharacterFile)
        lvWarehouse.BeginUpdate()
        lvWarehouse.Items.Clear()
        Try
            Dim account As String = If(_entry Is Nothing OrElse _entry.Account = CharacterIndex.NoAccount, c.MasterId, _entry.Account)
            Dim warehousePath As String = If(account = "", Nothing, AppState.CharacterIndex.WarehousePath(account))
            If warehousePath Is Nothing Then
                lblWarehouse.Text = "Account '" & account & "' has no warehouse (" & CharacterIndex.WarehouseFolder & "\" &
                                    If(account = "", "?", UserCode.Compute(account).ToString()) & "\" & account & ".war does not exist)."
                lblWarehouse.ForeColor = SystemColors.GrayText
                Return
            End If
            Dim warehouse = WarehouseFile.Load(warehousePath, AppState.CharacterIndex.Candidates)
            If warehouse.IsEmpty Then
                lblWarehouse.Text = "Empty warehouse: " & warehousePath
                lblWarehouse.ForeColor = SystemColors.GrayText
                Return
            End If
            For Each item In warehouse.Items
                lvWarehouse.Items.Add(ItemRow(item, False))
            Next
            lblWarehouse.Text = "Warehouse of account " & account & "  |  Gold: " & warehouse.Gold.ToString("N0") & "  |  " &
                                warehouse.Items.Count & " items  |  Warehouse checksum: " &
                                If(warehouse.ChecksumOk, "OK", "INVALID (the server will not open this warehouse)") & vbCrLf &
                                warehousePath & "   (read-only)"
            lblWarehouse.ForeColor = If(warehouse.ChecksumOk, SystemColors.ControlText, Color.Firebrick)
        Catch ex As Exception
            lblWarehouse.Text = "Error reading the warehouse: " & ex.Message
            lblWarehouse.ForeColor = Color.Firebrick
        Finally
            lvWarehouse.EndUpdate()
        End Try
    End Sub

    Private Sub FillSkills(ByVal c As CharacterFile)
        lvSkills.BeginUpdate()
        lvSkills.Items.Clear()
        For i = 0 To c.Profile.SkillCount - 1
            Dim row = lvSkills.Items.Add((i + 1).ToString())
            row.SubItems.Add(c.SkillPoint(i).ToString())
            row.SubItems.Add(c.SkillMastery(i).ToString())
            row.SubItems.Add(c.SkillShortKey(i).ToString())
            If c.SkillPoint(i) = 0 Then row.ForeColor = SystemColors.GrayText
        Next
        lvSkills.EndUpdate()
    End Sub

#End Region

#Region "Edicao"

    Private Sub nudLevel_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles nudLevel.ValueChanged
        If _filling OrElse _working Is Nothing Then Return
        Dim level As Integer = CInt(nudLevel.Value)
        _working.Level = level
        Dim table As LevelTable = _working.Profile.LevelTable
        If Not table.IsValid(level, _working.Exp) Then SetExp(table.ExpAtLevelStart(level))
        RefreshDerived()
    End Sub

    Private Sub cmdExpStart_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdExpStart.Click
        If _working Is Nothing Then Return
        SetExp(_working.Profile.LevelTable.ExpAtLevelStart(CInt(nudLevel.Value)))
        RefreshDerived()
    End Sub

    Private Sub SetExp(ByVal exp As Long)
        _working.Exp = exp
        _filling = True
        tbExp.Text = exp.ToString()
        _filling = False
    End Sub

    Private Sub tbExp_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles tbExp.TextChanged
        If _filling OrElse _working Is Nothing Then Return
        Dim exp As Long
        If Long.TryParse(tbExp.Text.Trim(), exp) Then
            tbExp.BackColor = SystemColors.Window
            _working.Exp = exp
        Else
            tbExp.BackColor = Color.MistyRose
        End If
        RefreshDerived()
    End Sub

    Private Sub Stats_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles nudStrength.ValueChanged,
            nudSpirit.ValueChanged, nudTalent.ValueChanged, nudDexterity.ValueChanged, nudHealth.ValueChanged
        If _filling OrElse _working Is Nothing Then Return
        _working.Strength = CInt(nudStrength.Value)
        _working.Spirit = CInt(nudSpirit.Value)
        _working.Talent = CInt(nudTalent.Value)
        _working.Dexterity = CInt(nudDexterity.Value)
        _working.Health = CInt(nudHealth.Value)
        RefreshDerived()
    End Sub

    Private Sub nudMoney_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles nudMoney.ValueChanged
        If _filling OrElse _working Is Nothing Then Return
        _working.Money = CInt(nudMoney.Value)
        _working.LastMoney = 0
        RefreshDerived()
    End Sub

    Private Sub cmdRemoveItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRemoveItem.Click
        If _working Is Nothing OrElse lvInventory.SelectedItems.Count = 0 Then
            MsgBox("Select an item in the list.", MsgBoxStyle.Information)
            Return
        End If
        Dim item = DirectCast(lvInventory.SelectedItems(0).Tag, GameItem)
        Dim text As String = "Remove " & item.CodeText & " - " & item.DisplayName & " (" & item.PlaceName & ")?" &
                             If(item.Position = 1, vbCrLf & "If it is a two-handed weapon, the left-hand copy is removed too.", "") &
                             vbCrLf & vbCrLf & "The removal only takes effect after 'Save changes'."
        If Not Dialogs.Confirm("Remove item", text) Then Return
        _working.RemoveItem(item)
        FillInventory(_working)
        RefreshDerived()
    End Sub

    ''' <summary>Atualiza faixas de EXP, pontos livres, verificacoes e o estado dos botoes.</summary>
    Private Sub RefreshDerived()
        If _working Is Nothing Then Return
        Dim c = _working
        Dim level As Integer = c.Level
        Dim table As LevelTable = c.Profile.LevelTable
        lblExpRange.Text = "Level " & level & " range in the " & c.Profile.Name & " table: " &
                           table.ExpAtLevelStart(level).ToString("N0") & " to " & table.ExpAtLevelEnd(level).ToString("N0")
        If c.Exp >= 0 AndAlso table.IsValid(level, c.Exp) Then
            lblServerLevel.Text = "OK: the EXP belongs to level " & level & "."
            lblServerLevel.ForeColor = Color.DarkGreen
        ElseIf c.Profile.FixLevelFromExp Then
            lblServerLevel.Text = "The server will change the level to " & table.LevelFromExp(c.Exp) & " because of the EXP."
            lblServerLevel.ForeColor = Color.Firebrick
        Else
            lblServerLevel.Text = "EXP outside the level range; this server does not correct the level, so it stays " & level & "."
            lblServerLevel.ForeColor = Color.DarkOrange
        End If

        Dim bonus As Integer = CharacterRules.QuestBonusPoints(level, c.LevelQuestLog)
        Dim total As Integer = CharacterRules.TotalStatPoints(level, c.LevelQuestLog)
        Dim free As Integer = CharacterRules.FreeStatPoints(level, c.LevelQuestLog, c.Strength, c.Spirit, c.Talent, c.Dexterity, c.Health)
        lblStatTotal.Text = "Allowed total for level " & level & ": " & total & "  (99 + " & (level - 1) & " x 5" &
                            If(bonus > 0, " + " & bonus & " from quests", "") & ")"
        If free <= CharacterRules.StatResetLimit Then
            lblStatFree.Text = "Free points: " & free & "  ->  the server will RESET the stats"
            lblStatFree.ForeColor = Color.Firebrick
        ElseIf free < 0 Then
            lblStatFree.Text = "Free points: " & free & "  (tolerated; the server sets 0)"
            lblStatFree.ForeColor = Color.DarkOrange
        Else
            lblStatFree.Text = "Free points: " & free
            lblStatFree.ForeColor = Color.DarkGreen
        End If

        Dim messages = CharacterRules.Validate(c)
        If messages.Count = 0 Then messages.Add(New RuleMessage(RuleSeverity.Info, "No problems: the server loads the character as it is."))
        ShowMessages(messages)
        UpdateDirtyState()
    End Sub

    Private Sub ShowMessages(ByVal messages As IEnumerable(Of RuleMessage))
        lvMessages.BeginUpdate()
        lvMessages.Items.Clear()
        For Each m In messages
            Dim prefix As String = If(m.Severity = RuleSeverity.Error, "ERROR: ", If(m.Severity = RuleSeverity.Warning, "WARNING: ", ""))
            Dim row = lvMessages.Items.Add(prefix & m.Text)
            row.ForeColor = If(m.Severity = RuleSeverity.Error, Color.Firebrick,
                               If(m.Severity = RuleSeverity.Warning, Color.DarkOrange, SystemColors.ControlText))
        Next
        lvMessages.EndUpdate()
    End Sub

    Private Function PendingChanges() As List(Of String)
        If _original Is Nothing OrElse _working Is Nothing Then Return New List(Of String)()
        Return CharacterRules.Describe(_original, _working)
    End Function

    Private Sub UpdateDirtyState()
        Dim dirty As Boolean = PendingChanges().Count > 0
        cmdSave.Enabled = dirty
        cmdDiscard.Enabled = dirty
        If _working IsNot Nothing Then
            lblTitle.Text = If(dirty, "* ", "") & _working.Name & "  -  " & _working.ClassName & " Lv " & _original.Level
        End If
    End Sub

    ''' <summary>Pergunta antes de perder alteracoes nao salvas. True = pode continuar.</summary>
    Private Function ConfirmDiscard() As Boolean
        Dim changes = PendingChanges()
        If changes.Count = 0 Then Return True
        Return Dialogs.Confirm("Unsaved changes", "Discard the changes to " & _working.Name & "?" & vbCrLf & vbCrLf &
                               String.Join(vbCrLf, changes))
    End Function

    Private Sub cmdDiscard_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDiscard.Click
        If _entry IsNot Nothing AndAlso ConfirmDiscard() Then OpenCharacter(_entry)
    End Sub

    Private Sub cmdOpenFolder_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdOpenFolder.Click
        If _entry Is Nothing Then Return
        Process.Start(AppState.Explorer, "/select,""" & _entry.FilePath & """")
    End Sub

    Private Sub cmdSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSave.Click
        If _working Is Nothing Then Return
        If tbExp.BackColor <> SystemColors.Window Then
            MsgBox("The EXP entered is not a valid number.", MsgBoxStyle.Exclamation)
            Return
        End If

        ' O servidor recalcula os pontos livres ao carregar; grava o mesmo valor para o arquivo ja ficar coerente.
        Dim free As Integer = CharacterRules.FreeStatPoints(_working.Level, _working.LevelQuestLog, _working.Strength,
                                                            _working.Spirit, _working.Talent, _working.Dexterity, _working.Health)
        _working.StatePoint = Math.Max(0, free)

        Dim messages = CharacterRules.Validate(_working)
        Dim errors = messages.Where(Function(m) m.Severity = RuleSeverity.Error).ToList()
        If errors.Count > 0 Then
            MsgBox("Cannot save:" & vbCrLf & vbCrLf & String.Join(vbCrLf, errors.Select(Function(m) "- " & m.Text)),
                   MsgBoxStyle.Exclamation, "Save character")
            Return
        End If

        Dim text As String = "Changes to " & _working.Name & ":" & vbCrLf &
                             String.Join(vbCrLf, PendingChanges().Select(Function(c) "  - " & c))
        Dim warnings = messages.Where(Function(m) m.Severity = RuleSeverity.Warning).ToList()
        If warnings.Count > 0 Then
            text &= vbCrLf & vbCrLf & "Warnings:" & vbCrLf & String.Join(vbCrLf, warnings.Select(Function(m) "  ! " & m.Text))
        End If
        If File.GetLastWriteTime(_entry.FilePath) <> _loadedWriteTime Then
            text &= vbCrLf & vbCrLf & "WARNING: the file was modified by another program (the server?) after it was opened here. " &
                    "Saving will overwrite that version."
        End If
        text &= vbCrLf & vbCrLf & "The character must be LOGGED OUT: if it is online, the server rewrites the file " &
                "on logout and the edit is lost." & vbCrLf & vbCrLf & "A backup will be created first. Save?"
        If Not Dialogs.Confirm("Save character", text, If(warnings.Count > 0, MsgBoxStyle.Exclamation, MsgBoxStyle.Question)) Then Return

        Try
            Dim backup As String = BackupHelper.CreateDataServerBackup(AppState.CharacterIndex.DataServerPath, _entry.FilePath)
            _working.Save()
            Dim saved = CharacterFile.Load(_entry.FilePath, AppState.CharacterIndex.Candidates)
            _entry.Level = saved.Level
            _entry.Exp = saved.Exp
            OpenCharacter(_entry)
            FillTree()
            lblStatus.Text = "Saved " & saved.Name & ". Backup: " & backup
            RaiseEvent CharacterSaved()
        Catch ex As Exception
            ErrorLog.Report("CharacterEditor.Save: " & _entry.FilePath, ex)
        End Try
    End Sub

#End Region

End Class
