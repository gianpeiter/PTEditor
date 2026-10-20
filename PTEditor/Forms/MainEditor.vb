Imports System.IO
Imports System.Text
Imports System.Threading.Tasks

''' <summary>Janela principal: abas de monstros, itens, mapas, NPCs e opcoes.</summary>
Public Class MainEditor

    Private Const TabMonsters As String = "Monsters"
    Private Const TabItems As String = "Items"
    Private Const TabMaps As String = "Maps"
    Private Const TabNpc As String = "NPC Editor"
    Private Const TabCharacters As String = "Characters"
    Private Const TabAwardItems As String = "Award Items"
    Private Const TabPostBox As String = "PostBox"
    Private Const TabSpawn As String = "Spawn Point"
    Private Const TabBellatra As String = "Bellatra"

    ''' <summary>Conversao do angulo do NPC (0..4096) para radianos: 2*PI/4096.</summary>
    Private Const AngleToRadians As Double = 0.0015339807878856412
    Private Const MaxAngle As Integer = 4096

    Private _monsterFields As List(Of KeyValuePair(Of String, TextBox))
    Private _itemFields As List(Of KeyValuePair(Of String, TextBox))
    Private _primaryClassBoxes As List(Of KeyValuePair(Of String, CheckBox))
    Private _secondaryClassBoxes As List(Of KeyValuePair(Of String, CheckBox))

    Private _itemSearch As ItemSearch
    Private _findMapsEnabled As Boolean
    Private _dropColumnsReady As Boolean

    ' Vista do mapa no editor de NPC: terreno do .smd (NpcCanvas) ou, sem ele, o minimapa .bmp como fundo.
    ' Como no minimapa do jogo, a imagem inteira cobre o retangulo do cenario (_mapRect).
    Private _mapImagePath As String = ""
    Private _mapImage As Bitmap
    Private _mapRect As MapRect = DefaultMapRect
    Private _npcMesh As StageMesh
    Private _npcFromCanvas As Boolean
    ''' <summary>Desfazer/Refazer das edicoes em memoria do .spc do mapa aberto (zera ao trocar de mapa ou recarregar).</summary>
    Private _npcHistory As New NpcUndoHistory()
    ''' <summary>Arquivo .spc aberto na aba NPC Editor ("" = nenhum).</summary>
    Private _npcMapName As String = ""
    Private _npcMapChanging As Boolean
    ''' <summary>Os dados foram relidos do disco (LoadAllData): a aba NPC Editor precisa reabrir o mapa.</summary>
    Private _npcMapStale As Boolean

    ''' <summary>StageMapRect do cenario em coordenadas do jogo (a mesma unidade de X/Z do .spc).</summary>
    Private Structure MapRect
        Public Left As Double, Top As Double, Right As Double, Bottom As Double
        Public Sub New(ByVal left As Double, ByVal top As Double, ByVal right As Double, ByVal bottom As Double)
            Me.Left = left : Me.Top = top : Me.Right = right : Me.Bottom = bottom
        End Sub
        Public ReadOnly Property Width As Double
            Get
                Return Right - Left
            End Get
        End Property
        Public ReadOnly Property Height As Double
            Get
                Return Bottom - Top
            End Get
        End Property
    End Structure

    ' Extraidos de image\GuideMap\StageInfo.dat do cliente (>> 8), indexados pelo nome do mapa sem ".ASE.SPC".
    Private Shared ReadOnly DefaultMapRect As New MapRect(-19847, -15714, -8376, -5083)
    Private Shared ReadOnly MapRects As New Dictionary(Of String, MapRect) From {
        {"FORE-3", New MapRect(-19847, -15714, -8376, -5083)},
        {"FORE-2", New MapRect(-8575, -15235, -157, -6926)},
        {"FORE-1", New MapRect(-399, -15123, 7767, -6223)},
        {"VILLAGE-2", New MapRect(-3499, -22374, 5584, -13195)},
        {"RUIN-4", New MapRect(-4403, -7059, 6439, 5728)},
        {"RUIN-3", New MapRect(-3583, 4559, 7860, 16882)},
        {"RUIN-2", New MapRect(1504, 16564, 13119, 27826)},
        {"RUIN-1", New MapRect(9049, 6338, 19339, 19168)},
        {"DE-1", New MapRect(11036, -6021, 21272, 6882)},
        {"VILLAGE-1", New MapRect(20265, -3714, 27129, 1627)},
        {"DE-2", New MapRect(9680, -17470, 19384, -5746)},
        {"DE-3", New MapRect(27070, -4873, 37071, 4280)},
        {"DE-4", New MapRect(30961, 3676, 45045, 14557)},
        {"DUN-1", New MapRect(-17123, -29223, -11101, -24016)},
        {"DUN-2", New MapRect(-8970, -29724, -2955, -24053)},
        {"DUN-3", New MapRect(-2481, -29159, 2213, -22816)},
        {"FOREVER-FALL-04", New MapRect(-4708, 26542, 7395, 41169)},
        {"FOREVER-FALL-03", New MapRect(-7270, 40442, 1933, 49871)},
        {"FOREVER-FALL-02", New MapRect(-5810, 49693, 3568, 59548)},
        {"FOREVER-FALL-01", New MapRect(-2516, 59370, 8107, 73163)},
        {"PILAI", New MapRect(-1879, 69251, 5981, 79459)},
        {"DUN-4", New MapRect(-16293, -45550, -8090, -37438)},
        {"DUN-5", New MapRect(-7947, -44601, 616, -34442)},
        {"TCAVE", New MapRect(118601, 21375, 126576, 27314)},
        {"MCAVE", New MapRect(118598, 32606, 125177, 37901)},
        {"DCAVE", New MapRect(157686, 16639, 165501, 22734)},
        {"IRON-1", New MapRect(43785, 11191, 51828, 22560)},
        {"IRON-2", New MapRect(33517, 18556, 46803, 30132)},
        {"ICE_URA", New MapRect(28868, 20896, 33642, 27932)},
        {"SOD-1", New MapRect(18611, 874, 26337, 7771)},
        {"ICE1", New MapRect(25247, 26747, 36190, 41286)},
        {"CASTLE", New MapRect(31095, -33925, 38460, -21011)},
        {"GREEDY", New MapRect(12706, 17322, 23889, 30287)},
        {"ICE_2", New MapRect(31245, 35747, 45722, 52827)},
        {"BOSS", New MapRect(31827, 49591, 34226, 53408)},
        {"LOSTISLAND", New MapRect(-19067, -5517, -6867, 7735)},
        {"LOST_TEMPLE", New MapRect(-17808, 7671, -6865, 16563)}}

    ' .spc cujo nome da imagem difere do nome do mapa.
    Private Shared ReadOnly MapImageNames As New Dictionary(Of String, String) From {
        {"ICE_2", "ICE2"}}

    Public Sub New()
        InitializeComponent()
        BuildFieldMaps()
        ItemFinder1.IndexProvider = AddressOf CharacterEditor1.GetIndex
        AwardItemsPanel1.IndexProvider = AddressOf CharacterEditor1.GetIndex
        PostBoxPanel1.IndexProvider = AddressOf CharacterEditor1.GetIndex
    End Sub

    ''' <summary>Chave do arquivo -> TextBox. A ordem importa so para *유니크색상 antes de *유니크 (prefixo).</summary>
    Private Sub BuildFieldMaps()
        _monsterFields = New List(Of KeyValuePair(Of String, TextBox)) From {
            Field(GameFields.Monster.ModelSize, tbMoSize), Field(GameFields.Name, tbMoMapName),
            Field(GameFields.Level, tbMoLevel), Field(GameFields.Monster.SpecialCode, tbMoSpCode),
            Field(GameFields.Monster.Organization, tbMoApp), Field(GameFields.Monster.Intelligence, tbMoInt),
            Field(GameFields.Monster.Character, tbMoChar), Field(GameFields.Monster.Vision, tbMoVision),
            Field(GameFields.Monster.HP, tbMoHp), Field(GameFields.Monster.MoveSpeed, tbMoMSpd),
            Field(GameFields.Monster.MoveType, tbMoMTyp), Field(GameFields.Monster.Sound, tbMoSnd),
            Field(GameFields.Monster.Exp, tbMoExp), Field(GameFields.Monster.AttackSpeed, tbMoAtkSpd),
            Field(GameFields.Monster.AttackPower, tbMoAtkPow), Field(GameFields.Monster.SkillPower, tbMoSKAtk),
            Field(GameFields.Monster.SkillRange, tbMoAtkCrit), Field(GameFields.Monster.AttackRating, tbMoAtkRtg),
            Field(GameFields.Monster.PerfectRate, tbMoPAtkRtg), Field(GameFields.Monster.Defense, tbMoDef),
            Field(GameFields.Monster.Absorb, tbMoAbs), Field(GameFields.Monster.Block, tbMoBlk),
            Field(GameFields.Monster.Organic, tbMoOrg), Field(GameFields.Monster.Lightning, tbMoLtg),
            Field(GameFields.Monster.Ice, tbMoIce), Field(GameFields.Monster.Fire, tbMoFire),
            Field(GameFields.Monster.Poison, tbMoPoision), Field(GameFields.Monster.Magic, tbMoMage)}

        _itemFields = New List(Of KeyValuePair(Of String, TextBox)) From {
            Field(GameFields.Item.Absorb, tbItmAbs), Field(GameFields.Name, tbItmJpName),
            Field(GameFields.Item.EngName, tbItmName), Field(GameFields.Item.UniqueColor, tbItmGlow),
            Field(GameFields.Item.Unique, tbItmQuest), Field(GameFields.Item.Durability, tbItmIntegrity),
            Field(GameFields.Item.Price, tbItmPrice), Field(GameFields.Item.Organic, tbItmOrganic),
            Field(GameFields.Item.Fire, tbItmFire), Field(GameFields.Item.Frost, tbItmFrost),
            Field(GameFields.Item.Lightning, tbItmLighting), Field(GameFields.Item.Poison, tbItmPoision),
            Field(GameFields.Item.HPRecover, tbItmHPRec), Field(GameFields.Item.MPRecover, tbItmManaRec),
            Field(GameFields.Item.STMRecover, tbItmStmRec), Field(GameFields.Item.Capacity, tbItmPots),
            Field(GameFields.Item.AttackPower, tbItmAtkPow), Field(GameFields.Item.Range, tbItmRange),
            Field(GameFields.Item.AttackSpeed, tbItmSpeed), Field(GameFields.Item.AttackRating, tbItmAtkRtg),
            Field(GameFields.Item.Critical, tbItmCrtRtg), Field(GameFields.Item.Defense, tbItmdef),
            Field(GameFields.Item.Block, tbItmBlk), Field(GameFields.Item.SpAttackSpeed, tbItmSPatkSpd),
            Field(GameFields.Item.SpCritical, tbItmSPCrt), Field(GameFields.Item.SpAttackPower, tbItmSPLvl),
            Field(GameFields.Item.SpAttackRating, tbItmSPRtg), Field(GameFields.Item.SpAbsorb, tbItmSPabs),
            Field(GameFields.Item.SpDefense, tbItmSPdef), Field(GameFields.Item.SpBlock, tbItmSPblk),
            Field(GameFields.Item.SpHPRegen, tbItmSPhp), Field(GameFields.Item.SpMPRegen, tbItmSPMp),
            Field(GameFields.Item.MoveSpeed, tbItmSPRun), Field(GameFields.Item.SpRange, tbItmSPRange),
            Field(GameFields.Item.SpMoveSpeed, tbItmRun), Field(GameFields.Item.HPRegen, tbItmHPRegen),
            Field(GameFields.Item.MPRegen, tbItmMPRegen), Field(GameFields.Item.STMRegen, tbItmSTMRegen),
            Field(GameFields.Item.HPAdd, tbItmHPAdd), Field(GameFields.Item.MPAdd, tbItmMPAdd),
            Field(GameFields.Item.STMAdd, tbItmSTMAdd), Field(GameFields.Item.MagicMastery, tbItmAPT),
            Field(GameFields.Item.Strength, tbItmStr), Field(GameFields.Item.Spirit, tbItmSpirit),
            Field(GameFields.Item.Talent, tbItmTalent), Field(GameFields.Item.Agility, tbItmAgi),
            Field(GameFields.Item.Health, tbItmreqHP)}

        ' A ordem define a sequencia em que as classes sao acrescentadas a linha ao salvar.
        _primaryClassBoxes = New List(Of KeyValuePair(Of String, CheckBox)) From {
            ClassBox(GameFields.Item.ClassMechanician, chPriMech), ClassBox(GameFields.Item.ClassArcher, chPriArcher),
            ClassBox(GameFields.Item.ClassAtalanta, chPriAta), ClassBox(GameFields.Item.ClassFighter, chPriFighter),
            ClassBox(GameFields.Item.ClassKnight, chPriKnight), ClassBox(GameFields.Item.ClassMagician, chPriMgs),
            ClassBox(GameFields.Item.ClassPikeman, chPriPike), ClassBox(GameFields.Item.ClassPriestess, chPriPrs)}
        _secondaryClassBoxes = New List(Of KeyValuePair(Of String, CheckBox)) From {
            ClassBox(GameFields.Item.ClassArcher, chSecArcher), ClassBox(GameFields.Item.ClassAtalanta, chSecAta),
            ClassBox(GameFields.Item.ClassFighter, chSecFighter), ClassBox(GameFields.Item.ClassKnight, chSecKnight),
            ClassBox(GameFields.Item.ClassMechanician, chSecMech), ClassBox(GameFields.Item.ClassMagician, chSecMgs),
            ClassBox(GameFields.Item.ClassPikeman, chSecPike), ClassBox(GameFields.Item.ClassPriestess, chSecPrs)}
    End Sub

    Private Shared Function Field(ByVal key As String, ByVal box As TextBox) As KeyValuePair(Of String, TextBox)
        Return New KeyValuePair(Of String, TextBox)(key, box)
    End Function

    Private Shared Function ClassBox(ByVal className As String, ByVal box As CheckBox) As KeyValuePair(Of String, CheckBox)
        Return New KeyValuePair(Of String, CheckBox)(className, box)
    End Function

#Region "Inicializacao e configuracao"

    Private Sub MainEditor_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        AppState.EditorExe = tbEditorPath.Text
        AppState.ServerPath = NormalizeServerPath(tbServerPath.Text)
        AddHandler ServerDatabase.StatusChanged, AddressOf ShowDatabase
        LoadConfig()
        If tbEditorPath.Text = "" Then
            Dim notepad As String = Path.Combine(Environment.GetEnvironmentVariable("windir"), "Notepad.exe")
            If File.Exists(notepad) Then
                My.Settings.Editor = notepad
                tbEditorPath.Text = notepad
                AppState.EditorExe = notepad
            Else
                MsgBox("Please check Options to assign a text editor")
                ErrorLog.Write("No texteditor found" & vbCrLf & notepad)
            End If
        End If
        UpdateTitle()
    End Sub

    Private Shared Function NormalizeServerPath(ByVal path As String) As String
        If path = "" OrElse path.EndsWith("\") Then Return path
        Return path & "\"
    End Function

    Private Sub UpdateTitle()
        Text = "PTEditor V." & AppState.Version & " ServerPath:" & My.Settings.sspath
        lblprgV.Text = "PTEditor V." & AppState.Version
    End Sub

    Private Sub LoadConfig()
        Try
            If ConfigFile.EnsureExists() Then MsgBox("No config file found!" & vbCrLf & "Created a default config file")
            Dim cfg = ConfigFile.Load()
            cbItemSelector.Items.Clear()
            cbItemSelector.Items.AddRange(cfg.ItemSelectors.ToArray())
            cbMonsterSelector.Items.Clear()
            cbMonsterSelector.Items.AddRange(cfg.MonsterSelectors.ToArray())
            cbMonsterSelector1.Items.Clear()
            cbMonsterSelector1.Items.AddRange(cfg.MonsterSelectors.ToArray())

            Dim value As String = Nothing
            If cfg.Values.TryGetValue(ConfigFile.KeyServerPath, value) Then
                AppState.ServerPath = NormalizeServerPath(value)
                tbServerPath.Text = AppState.ServerPath
                My.Settings.sspath = AppState.ServerPath
            End If
            If cfg.Values.TryGetValue(ConfigFile.KeyDataServerPath, value) Then AppState.DataServerPath = value
            If cfg.Values.TryGetValue(ConfigFile.KeyMaxExp, value) Then
                tbMaxExp.Text = value
                My.Settings.MaxExp = value
            End If
            If cfg.Values.TryGetValue(ConfigFile.KeyMaxGold, value) Then
                tbMaxGold.Text = value
                My.Settings.MaxGold = value
            End If
            If cfg.Values.TryGetValue(ConfigFile.KeyTextEditor, value) Then
                tbEditorPath.Text = value
                My.Settings.Editor = value
                AppState.EditorExe = value
            End If
            If cfg.Values.TryGetValue(ConfigFile.KeyDropWarnings, value) Then
                tbWarnItem.Text = value
                My.Settings.ItemWarn = value
            End If
            If cfg.Values.TryGetValue(ConfigFile.KeyWarnZhoon, value) Then
                cbMoZhoonWarn.Checked = CBool(value)
                My.Settings.WarnMoZhoon = CBool(value)
            End If
            If cfg.Values.TryGetValue(ConfigFile.KeyFindMaps, value) Then
                chMapEn.Checked = CBool(value)
                My.Settings.chMap = CBool(value)
            End If
            If cfg.Values.TryGetValue(ConfigFile.KeyFindItems, value) Then
                chItemInfo.Checked = CBool(value)
                My.Settings.chItemInfo = CBool(value)
            End If
            AppState.NameTagOverride = ""
            If cfg.Values.TryGetValue(ConfigFile.KeyNameTag, value) Then AppState.NameTagOverride = value.ToUpper()
            If cbItemSelector.Items.Count > 0 Then cbItemSelector.SelectedIndex = 0
            If cbMonsterSelector.Items.Count > 0 Then cbMonsterSelector.SelectedIndex = 0
            If cbMonsterSelector1.Items.Count > 0 Then cbMonsterSelector1.SelectedIndex = 0
        Catch ex As Exception
            ErrorLog.Report("LoadConfig", ex)
        End Try
        ServerDatabase.Reload()
    End Sub

    Private Sub SaveConfig()
        Try
            Dim values As New Dictionary(Of String, String) From {
                {ConfigFile.KeyServerPath, tbServerPath.Text},
                {ConfigFile.KeyMaxExp, tbMaxExp.Text},
                {ConfigFile.KeyMaxGold, tbMaxGold.Text},
                {ConfigFile.KeyTextEditor, tbEditorPath.Text},
                {ConfigFile.KeyDropWarnings, tbWarnItem.Text},
                {ConfigFile.KeyWarnZhoon, cbMoZhoonWarn.Checked.ToString()},
                {ConfigFile.KeyFindMaps, chMapEn.Checked.ToString()},
                {ConfigFile.KeyFindItems, chItemInfo.Checked.ToString()}}
            ConfigFile.SaveValues(values)
        Catch ex As Exception
            ErrorLog.Report("SaveConfig", ex)
        End Try
    End Sub

    Private Sub cmdLoadConfig_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdLoadConfig.Click, LoadFilterToolStripMenuItem.Click
        LoadConfig()
    End Sub

    Private Sub cmdSaveConfig_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSaveConfig.Click
        SaveConfig()
    End Sub

    Private Sub cmdDefaultConfig_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDefaultConfig.Click
        ConfigFile.WriteDefault()
        MsgBox("Created a default config file")
        LoadConfig()
    End Sub

    Private Sub cmdConfigEdit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdConfigEdit.Click
        If Not File.Exists(AppState.ConfigFilePath) Then
            LoadConfig()
            MsgBox("No config file found! A default file was created")
        End If
        ProcessHelper.Open(AppState.EditorExe, AppState.ConfigFilePath)
    End Sub

    Private Sub cmdShowLog_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdShowLog.Click
        ProcessHelper.Open(AppState.EditorExe, ErrorLog.LogPath)
    End Sub

    Private Sub cmdTestLog_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdTestLog.Click
        ErrorLog.Write("Test entry")
    End Sub

    Private Sub cmdEditorExe_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdEditorExe.Click
        Try
            If dlgFile.ShowDialog() <> DialogResult.Cancel Then
                tbEditorPath.Text = dlgFile.FileName
                AppState.EditorExe = tbEditorPath.Text
            End If
        Catch ex As Exception
            ErrorLog.Report("cmdEditorExe", ex)
        End Try
    End Sub

    Private Async Sub cmdPath_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdPath.Click
        Try
            If dlgFolder.ShowDialog() = DialogResult.Cancel Then Return
            AppState.ServerPath = NormalizeServerPath(dlgFolder.SelectedPath)
            My.Settings.sspath = AppState.ServerPath
        Catch ex As Exception
            ErrorLog.Report("cmdPath", ex)
            Return
        End Try
        Await LoadAllDataAsync()
        lbItemFiles.Items.Clear()
        RefreshMonsterList()
        ServerDatabase.Reload()
        CharacterEditor1.FollowServerPath()
    End Sub

    ''' <summary>Linha Database de Options: conexao lida do server.ini e status do ultimo teste/operacao.</summary>
    Private Sub ShowDatabase()
        Dim config = ServerDatabase.Config
        tbDatabase.Text = If(config Is Nothing, "(not configured)", config.Description)
        lblDbStatus.Text = ServerDatabase.StatusText
        lblDbStatus.ForeColor = ServerDatabase.StatusColor
    End Sub

    ''' <summary>Abre o server.ini (secao [Database]) no editor de texto.</summary>
    Private Sub cmdDatabase_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDatabase.Click
        Dim path As String = System.IO.Path.Combine(AppState.ServerPath, DatabaseConfig.FileName)
        If Not File.Exists(path) Then
            MsgBox(DatabaseConfig.FileName & " not found in the server folder:" & vbCrLf & path & vbCrLf & vbCrLf &
                   "Check the Server Path.", MsgBoxStyle.Exclamation, "Database")
            Return
        End If
        ProcessHelper.Open(AppState.EditorExe, path)
    End Sub

    Private Sub cmdTestDb_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdTestDb.Click
        Cursor = Cursors.WaitCursor
        lblDbStatus.Text = "Testing..."
        lblDbStatus.ForeColor = SystemColors.GrayText
        lblDbStatus.Refresh()
        Try
            ServerDatabase.Test()
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub AwardItemsPanel1_OptionsRequested() Handles AwardItemsPanel1.OptionsRequested, PostBoxPanel1.OptionsRequested, BellatraPanel1.OptionsRequested
        TabControl1.SelectedTab = TabPage4
    End Sub

    Private Async Sub cmdReload_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdReload.Click, cmdReload1.Click, cmdMapReload.Click
        If Not ConfirmDiscardNpc() Then Return
        lbFiles.Items.Clear()
        lbItemFiles.Items.Clear()
        MoFound.Text = "0"
        ItemFound.Text = "0"
        Mapsfound.Text = "0"
        Await LoadAllDataAsync()
        RefreshItemList()
        RefreshMapList()
        RefreshMonsterList()
        RefreshMonsterSource()
    End Sub

    Private Async Sub cmdNPCReload_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdNPCReload.Click
        If Not ConfirmDiscardNpc() Then Return
        Await LoadAllDataAsync()
        FillNpcMapList(True)
    End Sub

    ''' <summary>True enquanto LoadAllDataAsync esta lendo o servidor em outra thread.</summary>
    Private _loading As Boolean
    ''' <summary>Ultimo percentual enviado a barra (so atualiza quando muda; usado apenas na thread de carga).</summary>
    Private _lastProgress As Integer

    ''' <summary>
    ''' Carrega monstros, itens, mapas (.spm/.spc/.spp) e NPCs do servidor em uma thread de fundo, mantendo a janela
    ''' responsiva. As abas ficam desabilitadas durante a carga (os dados em AppState estao sendo reescritos).
    ''' </summary>
    Private Async Function LoadAllDataAsync() As Task
        If _loading Then Return
        _loading = True
        If AppState.ServerPath = "" Then AppState.ServerPath = NormalizeServerPath(My.Settings.sspath)
        _npcHistory = New NpcUndoHistory()
        _npcMapStale = True
        UpdateTitle()
        MoFound.Text = "0"
        ItemFound.Text = "0"
        Mapsfound.Text = "0"
        pbWorking.Value = 0
        _lastProgress = -1
        TabControl1.Enabled = False
        Try
            SetProgress("Loading monster files", 0)
            Dim monsterWarning As String = Await Task.Run(Function() MonsterData.LoadAll(Sub(i, n) ReportProgress(0, i, n)))
            MoFound.Text = AppState.Monsters.Count.ToString()

            SetProgress("Loading item files", 25)
            Dim itemWarning As String = Await Task.Run(Function() ItemData.LoadAll(Sub(i, n) ReportProgress(25, i, n)))
            ItemFound.Text = AppState.Items.Count.ToString()

            SetProgress("Loading map files", 50)
            Await Task.Run(Sub()
                               NpcData.LoadSpcFiles()
                               NpcData.LoadSppFiles()
                               MapData.LoadAll(Sub(i, n) ReportProgress(50, i, n))
                           End Sub)
            Mapsfound.Text = AppState.Maps.Count.ToString()
            lblTextCoding.Text = AppState.FileEncoding.HeaderName & "/" & Encoding.Default.EncodingName

            SetProgress("Loading NPC files", 75)
            Await Task.Run(Sub() NpcData.LoadNpcFiles(Sub(i, n) ReportProgress(75, i, n)))
            RefreshNpcCombos()

            pbWorking.Value = 0
            Label117.Text = "Status:"
            lblProg.Text = "Done"
            If cbMoZhoonWarn.Checked Then
                If monsterWarning <> "" Then MsgBox(monsterWarning)
                If itemWarning <> "" Then MsgBox(itemWarning)
            End If
        Catch ex As Exception
            Label117.Text = "Status:"
            lblProg.Text = "Error loading"
            ErrorLog.Report("LoadAllData", ex)
        Finally
            TabControl1.Enabled = True
            _loading = False
        End Try
    End Function

    ''' <summary>Progresso vindo da thread de carga: posta na UI so quando o percentual inteiro muda.</summary>
    Private Sub ReportProgress(ByVal basePercent As Integer, ByVal current As Integer, ByVal total As Integer)
        Dim percent As Integer = basePercent + CInt(25 * current / Math.Max(total, 1))
        If percent = _lastProgress Then Return
        _lastProgress = percent
        Dim value As Integer = Math.Max(0, Math.Min(100, percent))
        If IsHandleCreated Then BeginInvoke(Sub() pbWorking.Value = value)
    End Sub

    Private Sub SetProgress(ByVal text As String, ByVal percent As Integer)
        Label117.Text = "Loading:"
        lblProg.Text = text
        pbWorking.Value = percent
    End Sub

    Private Sub RefreshNpcCombos()
        tbNPCSetupfile.BeginUpdate()
        tbNPCModelFile.BeginUpdate()
        tbNPCSetupfileInI.BeginUpdate()
        Try
            tbNPCSetupfile.Items.Clear()
            tbNPCModelFile.Items.Clear()
            For Each npc In AppState.Npcs
                tbNPCSetupfile.Items.Add(npc.FileName)
                If npc.SetupIni <> "" Then tbNPCModelFile.Items.Add(npc.SetupIni)
            Next
            tbNPCSetupfileInI.Items.Clear()
            Dim charFolder As String = Path.Combine(AppState.ServerPath, AppState.CharFolder)
            If Directory.Exists(charFolder) Then
                For Each f In Directory.GetFiles(charFolder, "*.inx", SearchOption.AllDirectories)
                    tbNPCSetupfileInI.Items.Add(f.Substring(AppState.ServerPath.Length))
                Next
            End If
        Finally
            tbNPCSetupfile.EndUpdate()
            tbNPCModelFile.EndUpdate()
            tbNPCSetupfileInI.EndUpdate()
        End Try
    End Sub

    Private Sub TabControl1_Selected(ByVal sender As Object, ByVal e As TabControlEventArgs) Handles TabControl1.Selected
        Try
            Select Case TabControl1.SelectedTab.Text
                Case TabMonsters
                    RefreshMonsterList()
                Case TabItems
                    RefreshItemList()
                Case TabMaps
                    RefreshMapList()
                    RefreshMonsterSource()
                Case TabSpawn
                    SpawnPointPanel1.EnsureLoaded()
                Case TabNpc
                    FillNpcMapList()
                Case TabCharacters
                    CharacterEditor1.EnsureLoaded()
                Case TabAwardItems
                    AwardItemsPanel1.EnsureLoaded()
                Case TabPostBox
                    PostBoxPanel1.EnsureLoaded()
                Case TabBellatra
                    BellatraPanel1.EnsureLoaded()
            End Select
        Catch ex As Exception
            ErrorLog.Report("TabControl1_Selected", ex)
        End Try
    End Sub

    Private Sub ItemFinder1_OpenCharacterRequested(ByVal name As String, ByVal showWarehouse As Boolean) Handles ItemFinder1.OpenCharacterRequested
        TabControl1.SelectedTab = TabPageCharacters
        CharacterEditor1.SelectCharacter(name, showWarehouse)
    End Sub

    Private Sub ProfilePanel1_ProfilesReloaded() Handles ProfilePanel1.ProfilesReloaded
        CharacterEditor1.FillProfiles()
    End Sub

    Private Sub CharacterEditor1_CharacterSaved() Handles CharacterEditor1.CharacterSaved
        ItemFinder1.ClearCache()
    End Sub

    Private Sub lblMonsterFolder_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lblMonsterFolder.Click
        ProcessHelper.Open(AppState.Explorer, AppState.ServerPath & AppState.MonsterFolder)
    End Sub

    Private Sub lblItemFolder_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lblItemFolder.Click
        ProcessHelper.Open(AppState.Explorer, AppState.ServerPath & AppState.ItemFolder)
    End Sub

    Private Sub lblMapFolder_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lblMapFolder.Click
        ProcessHelper.Open(AppState.Explorer, AppState.ServerPath & AppState.FieldFolder)
    End Sub

#End Region

#Region "Aba Monsters"

    Private Function SelectedMonster() As MonsterFile
        If lbFiles.SelectedItem Is Nothing Then Return Nothing
        Return MonsterData.Find(lbFiles.SelectedItem.ToString())
    End Function

    Private Function SelectedMonsters() As List(Of MonsterFile)
        Dim result As New List(Of MonsterFile)()
        For Each name As Object In lbFiles.SelectedItems
            Dim monster = MonsterData.Find(name.ToString())
            If monster IsNot Nothing Then result.Add(monster)
        Next
        Return result
    End Function

    Private Sub UpdateMonsterCount()
        lbFound.Text = lbFiles.SelectedItems.Count & "/" & lbFiles.Items.Count
    End Sub

    ''' <summary>Nomes de arquivo (ou mensagens) para o filtro informado.</summary>
    Private Function FilterMonsterNames(ByVal filter As String) As List(Of String)
        Dim names As New List(Of String)()
        Dim errorMessage As String = Nothing
        For Each monster In MonsterData.Filter(filter, errorMessage)
            names.Add(monster.FileName)
        Next
        If errorMessage IsNot Nothing Then
            names.Add("Error")
            names.Add(errorMessage)
        End If
        If names.Count = 0 Then names.Add("Nothing found")
        Return names
    End Function

    Private Sub RefreshMonsterList()
        lbFiles.Items.Clear()
        lbFiles.Items.AddRange(FilterMonsterNames(cbMonsterSelector.Text).ToArray())
        UpdateMonsterCount()
    End Sub

    Private Sub ShowMonsterList(ByVal monsters As IEnumerable(Of MonsterFile))
        TabControl1.SelectedIndex = 0
        cbMonsterSelector.Text = "Name="
        lbFiles.BeginUpdate()
        Try
            lbFiles.Items.Clear()
            For Each monster In monsters
                lbFiles.Items.Add(monster.FileName)
            Next
        Finally
            lbFiles.EndUpdate()
        End Try
        UpdateMonsterCount()
    End Sub

    Private Sub cbMonsterSelector_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles cbMonsterSelector.KeyPress
        If e.KeyChar = ChrW(13) Then RefreshMonsterList()
    End Sub

    Private Sub cbMonsterSelector_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cbMonsterSelector.SelectedIndexChanged
        RefreshMonsterList()
    End Sub

    Private Sub cbSelectAll_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cbSelectAll.CheckedChanged
        For i = 0 To lbFiles.Items.Count - 1
            lbFiles.SetSelected(i, cbSelectAll.Checked)
        Next
    End Sub

    Private Sub lbFiles_SelectionInput(ByVal sender As Object, ByVal e As EventArgs) Handles lbFiles.KeyUp, lbFiles.MouseClick
        If lbFiles.SelectedIndex = -1 Then Return
        tbDrops.Text = ""
        tbDroprate.Text = ""
        ShowMonster()
        If _findMapsEnabled Then ShowMapsForMonster()
        UpdateMonsterCount()
    End Sub

    Private Sub lbFiles_SelectedValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles lbFiles.SelectedValueChanged
        If lbFiles.SelectedIndex <> -1 Then UpdateMonsterCount()
    End Sub

    Private Sub lbFiles_MouseDoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles lbFiles.MouseDoubleClick
        If lbFiles.SelectedIndex <> -1 Then ProcessHelper.Open(AppState.EditorExe, AppState.MonsterPath(lbFiles.SelectedItem.ToString()))
    End Sub

    ''' <summary>Preenche os campos com o monstro selecionado.</summary>
    Private Sub ShowMonster()
        Dim monster = SelectedMonster()
        For Each f In _monsterFields
            f.Value.Text = ""
        Next
        tbMoName.Text = ""
        tbMoNoDrop.Text = ""
        cbMoTyp.Text = ""
        cbMoBoss.Checked = False
        If monster Is Nothing Then
            lwItems.Items.Clear()
            Return
        End If
        Try
            tbMoLevel.Text = monster.Level
            For Each line In monster.Lines
                If Not line.StartsWith("*") Then Continue For
                If line.Contains(GameFields.Monster.Boss) Then cbMoBoss.Checked = True
                For Each f In _monsterFields
                    If line.Contains(f.Key) Then
                        f.Value.Text = line.Substring(f.Key.Length).Trim().Trim(ChrW(9))
                        Exit For
                    End If
                Next
                If line.Contains(GameFields.Monster.Race) Then
                    cbMoTyp.Text = RaceDisplayName(line.Substring(GameFields.Monster.Race.Length).Trim())
                End If
                If MonsterData.IsDropCounterLine(line) Then
                    tbMoNoDrop.Text = line.Substring(GameFields.Monster.DropCounter.Length).Trim().Trim(ChrW(9))
                End If
            Next
            tbMoName.Text = monster.Name
        Catch ex As Exception
            ErrorLog.Report("ShowMonster", ex)
        End Try
        ShowDrops(monster)
    End Sub

    Private Shared Function RaceDisplayName(ByVal value As String) As String
        If value = GameFields.Monster.RaceDaemon Then Return "Daemon"
        If value = GameFields.Monster.RaceUndead Then Return "Undead"
        If value = GameFields.Monster.RaceMutant Then Return "Mutant"
        If value = GameFields.Monster.RaceMechanic Then Return "Mechanic"
        Return "Normal"
    End Function

    ''' <summary>Valor gravado para a raca escolhida; Nothing mantem a linha como esta ("Normal").</summary>
    Private Shared Function RaceFileValue(ByVal displayName As String) As String
        Select Case displayName.Trim()
            Case "Daemon" : Return GameFields.Monster.RaceDaemon
            Case "Undead" : Return GameFields.Monster.RaceUndead
            Case "Mutant" : Return GameFields.Monster.RaceMutant
            Case "Mechanic" : Return GameFields.Monster.RaceMechanic
        End Select
        Return Nothing
    End Function

    Private Sub ShowDrops(ByVal monster As MonsterFile)
        Try
            If Not _dropColumnsReady Then
                lwItems.View = View.Details
                lwItems.FullRowSelect = True
                lwItems.Columns.Add("Rate", 80, HorizontalAlignment.Left)
                lwItems.Columns.Add("Items", 800, HorizontalAlignment.Left)
                _dropColumnsReady = True
            End If
            lwItems.BeginUpdate()
            lwItems.Items.Clear()
            For Each entry In MonsterData.GetDrops(monster)
                If entry.IsExtra Then
                    Dim marker = lwItems.Items.Add("Extra")
                    marker.SubItems.Add("Drop")
                    marker.ForeColor = Color.Gray
                End If
                Dim row = lwItems.Items.Add(entry.Rate)
                row.SubItems.Add(entry.Items)
                row.Tag = entry
                If entry.IsExtra Then row.ForeColor = Color.Gray
            Next
        Catch ex As Exception
            ErrorLog.Report("ShowDrops", ex)
        Finally
            lwItems.EndUpdate()
        End Try
    End Sub

    ''' <summary>Grava os campos e a lista de drops no .inf do monstro selecionado.</summary>
    Private Sub SaveMonster(ByVal monster As MonsterFile)
        If monster Is Nothing Then
            MsgBox("Please select a monster")
            Return
        End If
        Try
            If cbMoBoss.Checked AndAlso Not MonsterData.SetFirst(monster, GameFields.Monster.Boss, "") Then
                monster.Lines.Insert(1, GameFields.Monster.Boss)
            End If
            If Not MonsterData.SetFirst(monster, GameFields.Monster.ModelSize, tbMoSize.Text) AndAlso tbMoSize.Text <> "" Then
                monster.Lines.Insert(1, GameFields.Monster.ModelSize & vbTab & tbMoSize.Text)
            End If

            For i = 0 To monster.Lines.Count - 1
                Dim line = monster.Lines(i)
                If Not line.StartsWith("*") Then Continue For
                If line.StartsWith(GameFields.Monster.Boss) AndAlso Not cbMoBoss.Checked Then
                    monster.Lines(i) = ""
                ElseIf line.StartsWith(GameFields.Monster.ModelSize) AndAlso tbMoSize.Text.Length = 0 Then
                    monster.Lines(i) = ""
                ElseIf line.Contains(GameFields.Monster.Race) Then
                    Dim race = RaceFileValue(cbMoTyp.Text)
                    If race IsNot Nothing Then monster.Lines(i) = GameFields.Monster.Race & vbTab & race
                ElseIf MonsterData.IsDropCounterLine(line) Then
                    monster.Lines(i) = GameFields.Monster.DropCounter & vbTab & tbMoNoDrop.Text
                Else
                    For Each f In _monsterFields
                        If line.Contains(f.Key) Then
                            monster.Lines(i) = f.Key & vbTab & f.Value.Text
                            Exit For
                        End If
                    Next
                End If
            Next

            ' Drops: as linhas "*아이템" do arquivo passam a ser exatamente as linhas da lista (ordem preservada).
            Dim entries = MonsterData.GetDrops(monster).Where(Function(d) Not d.IsExtra).ToList()
            Dim rows As New List(Of ListViewItem)()
            For Each row As ListViewItem In lwItems.Items
                Dim entry = TryCast(row.Tag, MonsterData.DropEntry)
                If entry IsNot Nothing AndAlso Not entry.IsExtra Then rows.Add(row)
            Next
            For k = 0 To entries.Count - 1
                If k < rows.Count Then
                    entries(k).Rate = rows(k).Text
                    entries(k).Items = rows(k).SubItems(1).Text
                Else
                    entries(k).Rate = ""
                    entries(k).Items = ""
                End If
            Next
            MonsterData.SetDrops(monster, entries)

            MonsterData.Save(monster, True)
            MonsterData.Reload(monster)
            ShowMonster()
        Catch ex As Exception
            ErrorLog.Report("SaveMonster", ex)
        End Try
    End Sub

    Private Sub cmdSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSave.Click
        SaveMonster(SelectedMonster())
    End Sub

    Private Sub cmdMoRealName_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdMoRealName.Click
        Dim monster = SelectedMonster()
        If monster Is Nothing Then
            MsgBox("Please select a monster")
        Else
            ProcessHelper.Open(AppState.EditorExe, monster.ZhoonFile)
        End If
    End Sub

    ' ---- Lista de drops -----------------------------------------------------------

    Private Sub lwItems_ItemSelectionChanged(ByVal sender As Object, ByVal e As ListViewItemSelectionChangedEventArgs) Handles lwItems.ItemSelectionChanged
        If lbFiles.SelectedItems.Count = 0 Then
            MsgBox("Please select a monster")
            Return
        End If
        If lwItems.SelectedItems.Count <> 0 Then
            tbDroprate.Text = lwItems.SelectedItems(0).Text
            tbDrops.Text = lwItems.SelectedItems(0).SubItems(1).Text
        End If
    End Sub

    Private Sub lwItems_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles lwItems.SelectedIndexChanged
        If lbFiles.SelectedItems.Count = 0 Then
            MsgBox("Please select a monster")
            Return
        End If
        If lwItems.SelectedItems.Count <> 0 AndAlso chItemInfo.Checked Then
            ShowItemsForDrop()
            lblMoListName.Text = tbMoName.Text & " Filename:" & lbFiles.SelectedItem.ToString()
        End If
    End Sub

    Private Sub tbDroprate_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles tbDroprate.KeyPress
        If lwItems.SelectedItems.Count = 0 Then Return
        lwItems.SelectedItems(0).Text = tbDroprate.Text
        lwItems.SelectedItems(0).SubItems(1).Text = tbDrops.Text
    End Sub

    Private Sub tbDrops_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles tbDrops.KeyPress
        If e.KeyChar = ChrW(13) AndAlso lwItems.SelectedItems.Count > 0 Then
            lwItems.SelectedItems(0).SubItems(1).Text = tbDrops.Text
            lwItems.SelectedItems(0).Text = tbDroprate.Text
            ShowItemsForDrop()
        End If
    End Sub

    Private Sub AddDroplineToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AddDroplineToolStripMenuItem.Click
        Dim monster = SelectedMonster()
        If monster Is Nothing Then
            MsgBox("Please select a monster")
            Return
        End If
        Dim entry As MonsterData.DropEntry = Nothing
        If lwItems.SelectedItems.Count > 0 Then entry = TryCast(lwItems.SelectedItems(0).Tag, MonsterData.DropEntry)
        If entry Is Nothing Then
            MsgBox("Please select a drop line")
            Return
        End If
        Try
            MonsterData.InsertDropLine(monster, entry.LineIndex)
            MonsterData.Save(monster, True)
            MonsterData.Reload(monster)
            RefreshMonsterList()
            lbFiles.SelectedItem = monster.FileName
            ShowMonster()
        Catch ex As Exception
            ErrorLog.Report("Add new dropline", ex)
        End Try
    End Sub

    Private Sub DeleteLineToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DeleteLineToolStripMenuItem.Click
        If lbFiles.SelectedItems.Count = 0 Then
            MsgBox("Please select a monster")
            Return
        End If
        tbDroprate.Clear()
        tbDrops.Clear()
        If lwItems.SelectedItems.Count > 0 Then lwItems.SelectedItems(0).Remove()
    End Sub

    ''' <summary>Lista os arquivos de item dos codigos da linha de drop selecionada.</summary>
    Private Sub ShowItemsForDrop()
        Try
            lbItemFiles.BeginUpdate()
            lbItemsRealName.BeginUpdate()
            lbItemFiles.Items.Clear()
            lbItemsRealName.Items.Clear()
            If tbDrops.Text.StartsWith("Gold") OrElse tbDrops.Text.StartsWith("Nothing") Then Return
            Dim codes = tbDrops.Text.Split(" "c)
            For Each code In codes
                If code.Trim() = "" Then Continue For
                Dim item = ItemData.FindByCode("""" & code.Trim().ToUpper() & """")
                If item IsNot Nothing Then
                    lbItemFiles.Items.Add(item.FileName)
                    lbItemsRealName.Items.Add(item.Name)
                Else
                    lbItemFiles.Items.Add("Item: " & code & " not found")
                    lbItemsRealName.Items.Add("Item: " & code & " not found")
                End If
            Next
        Catch ex As Exception
            ErrorLog.Report("ShowItemsForDrop", ex)
        Finally
            lbItemFiles.EndUpdate()
            lbItemsRealName.EndUpdate()
        End Try
    End Sub

    Private Sub chItemInfo_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chItemInfo.CheckedChanged
        If chItemInfo.Checked Then
            ShowItemsForDrop()
        Else
            lbItemFiles.Items.Clear()
            lbItemsRealName.Items.Clear()
        End If
    End Sub

    Private Sub lbItemFiles_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles lbItemFiles.SelectedIndexChanged
        lbItemsRealName.SelectedIndex = lbItemFiles.SelectedIndex
    End Sub

    Private Sub lbItemsRealName_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles lbItemsRealName.SelectedIndexChanged
        lbItemFiles.SelectedIndex = lbItemsRealName.SelectedIndex
    End Sub

    Private Sub lbItemFiles_MouseDoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles lbItemFiles.MouseDoubleClick
        If lbItemFiles.SelectedItem IsNot Nothing Then ProcessHelper.Open(AppState.EditorExe, AppState.ItemPath(lbItemFiles.SelectedItem.ToString()))
    End Sub

    Private Sub cmMonsterItem_Opening(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles cmMonsterItem.Opening
        GoToItemEditorToolStripMenuItem.Enabled = lbItemFiles.SelectedItems.Count <> 0
    End Sub

    Private Sub GoToItemEditorToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles GoToItemEditorToolStripMenuItem.Click
        Dim item = ItemData.Find(lbItemFiles.SelectedItem.ToString())
        If item Is Nothing Then Return
        cbItemSelector.Text = item.PlainCode
        TabControl1.SelectTab(1)
        RefreshItemList()
        lbFileListItems.SelectedItem = item.FileName
        ShowItem(item)
    End Sub

    ' ---- Mapas onde o monstro aparece -------------------------------------------

    Private Sub chMapEn_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chMapEn.CheckedChanged
        _findMapsEnabled = chMapEn.Checked
        If _findMapsEnabled Then
            ShowMapsForMonster()
        Else
            lbMaps.Items.Clear()
        End If
    End Sub

    Private Sub chNotFound_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chNotFound.CheckedChanged
        ShowMapsForMonster()
    End Sub

    ''' <summary>Mapas cujas linhas citam o *이름 do monstro (ou, com "not in map", os que nao citam).</summary>
    Private Sub ShowMapsForMonster()
        If lbFiles.SelectedItems.Count = 0 Then Return
        lbMaps.BeginUpdate()
        lbMaps.Items.Clear()
        Dim mapName As String = tbMoMapName.Text
        Try
            For Each map In AppState.Maps
                Dim found As Boolean = map.Lines.Any(Function(l) l.StartsWith("*") AndAlso l.Contains(mapName))
                If found <> chNotFound.Checked Then lbMaps.Items.Add(map.FileName)
            Next
        Catch ex As Exception
            ErrorLog.Report("ShowMapsForMonster", ex)
        Finally
            lbMaps.EndUpdate()
        End Try
    End Sub

    Private Sub lbMaps_MouseDoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles lbMaps.MouseDoubleClick
        If lbMaps.SelectedItem IsNot Nothing Then ProcessHelper.Open(AppState.EditorExe, AppState.FieldPath(lbMaps.SelectedItem.ToString()))
    End Sub

    ' ---- Menu de contexto da lista de monstros -------------------------------------

    Private Sub cnMonsterFiles_Opening(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles cnMonsterFiles.Opening
        BackupsToolStripMenuItem.DropDownItems.Clear()
        DeleteBackupToolStripMenuItem.DropDownItems.Clear()
        Dim count As Integer = lbFiles.SelectedItems.Count
        DropCountToolStripMenuItem.Enabled = count >= 2
        ExperienceToolStripMenuItem.Enabled = count >= 2
        GoldToolStripMenuItem.Enabled = count >= 2
        If count = 0 Then Return
        If count = 1 Then
            BackupsToolStripMenuItem.Text = "Load Backup"
            DeleteBackupToolStripMenuItem.Text = "Delete Backup"
            For Each backup In BackupHelper.ListBackups(AppState.ServerPath & AppState.MonsterFolder, lbFiles.SelectedItem.ToString(), AppState.MonsterPattern)
                BackupsToolStripMenuItem.DropDownItems.Add(backup)
                If Not backup.ToUpper().Contains("DEFAULT") Then DeleteBackupToolStripMenuItem.DropDownItems.Add(backup)
            Next
        Else
            BackupsToolStripMenuItem.Text = "Load Default"
            DeleteBackupToolStripMenuItem.Text = "Delete all Backups"
        End If
    End Sub

    Private Sub BackupsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BackupsToolStripMenuItem.Click
        If lbFiles.SelectedItems.Count < 2 Then Return
        If Not Dialogs.Confirm("Load defaults", "Do you want load default for all selected monster?") Then Return
        For Each monster In SelectedMonsters()
            MonsterData.LoadBackup(monster, AppState.DefaultBackupPrefix & monster.FileName)
            MonsterData.Save(monster, True)
        Next
    End Sub

    Private Sub BackupsToolStripMenuItem_DropDownItemClicked(ByVal sender As Object, ByVal e As ToolStripItemClickedEventArgs) Handles BackupsToolStripMenuItem.DropDownItemClicked
        Dim monster = SelectedMonster()
        If monster Is Nothing Then Return
        MonsterData.LoadBackup(monster, e.ClickedItem.Text)
        ShowMonster()
    End Sub

    Private Sub DeleteBackupToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DeleteBackupToolStripMenuItem.Click
        If lbFiles.SelectedItems.Count < 2 Then Return
        If Not Dialogs.Confirm("Delete Backups", "Do you want to delete all Backupfiles in each selected monster?") Then Return
        For Each monster In SelectedMonsters()
            BackupHelper.DeleteBackups(AppState.ServerPath & AppState.MonsterFolder, AppState.MonsterPattern, monster.FileName)
        Next
    End Sub

    Private Sub DeleteBackupToolStripMenuItem_DropDownItemClicked(ByVal sender As Object, ByVal e As ToolStripItemClickedEventArgs) Handles DeleteBackupToolStripMenuItem.DropDownItemClicked
        File.Delete(BackupHelper.BackupPath(AppState.ServerPath & AppState.MonsterFolder, e.ClickedItem.Text))
        BackupsToolStripMenuItem.DropDownItems.Clear()
        DeleteBackupToolStripMenuItem.DropDownItems.Clear()
    End Sub

    ''' <summary>Search &gt; Item: seleciona na lista os monstros cujo drop contem o codigo.</summary>
    Private Sub SearchItemToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles SearchItemToolStripMenuItem.Click
        Dim code As String = InputBox("Enter Itemcode", "")
        If code = "" Then Return
        Try
            lbFiles.SelectedItems.Clear()
            For i = 0 To lbFiles.Items.Count - 1
                Dim monster = MonsterData.Find(lbFiles.Items(i).ToString())
                lbFiles.SetSelected(i, monster IsNot Nothing AndAlso MonsterData.MatchesText(monster, GameFields.Monster.Drop, code))
            Next
            UpdateMonsterCount()
        Catch ex As Exception
            ErrorLog.Report("SearchItem", ex)
        End Try
    End Sub

    Private Sub DropCountToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DropCountToolStripMenuItem.Click
        Dim count As Long
        If Not Dialogs.AskLong("Enter Dropcount", "", "", count) Then Return
        Try
            Dim monsters = SelectedMonsters()
            For Each monster In monsters
                MonsterData.SetDropCount(monster, count)
            Next
            If Dialogs.Confirm("Save monsters", "Do you want to save all selected monsters?") Then
                For Each monster In monsters
                    MonsterData.Save(monster, True)
                Next
                MsgBox("Drop count changed to " & count)
            Else
                MsgBox("No monster saved! Please press Reload!")
            End If
        Catch ex As Exception
            ErrorLog.Report("DropCount", ex)
        End Try
    End Sub

    Private Sub ExperienceToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ExperienceToolStripMenuItem.Click
        Using dialog As New ExpChanger(SelectedMonsters())
            dialog.ShowDialog(Me)
        End Using
    End Sub

    Private Sub GoldToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles GoldToolStripMenuItem.Click
        If tbMaxGold.Text = "" Then tbMaxGold.Text = My.Settings.MaxGold
        Dim maxGold As Double
        Double.TryParse(tbMaxGold.Text, maxGold)
        Using dialog As New GoldChanger(SelectedMonsters(), maxGold)
            dialog.ShowDialog(Me)
        End Using
    End Sub

#End Region

#Region "Aba Items"

    Private Function SelectedItemFile() As ItemFile
        If lbFileListItems.SelectedItem Is Nothing Then Return Nothing
        Return ItemData.Find(lbFileListItems.SelectedItem.ToString())
    End Function

    Private Sub UpdateItemCount()
        lbItemsListCount.Text = lbFileListItems.SelectedItems.Count & "/" & lbFileListItems.Items.Count
    End Sub

    ''' <summary>Filtro "Nome=da1,da2": itens cujo codigo comeca com um dos prefixos.</summary>
    Private Sub RefreshItemList()
        lbFileListItems.BeginUpdate()
        lbFileListItems.Items.Clear()
        Try
            Dim expression As String = cbItemSelector.Text.ToUpper()
            If expression.Contains("=") Then expression = expression.Substring(expression.IndexOf("="c) + 1)
            Dim prefixes = expression.Split(","c)
            For Each item In AppState.Items
                Dim code As String = item.PlainCode.ToUpper()
                If prefixes.Any(Function(p) code.StartsWith(p)) Then lbFileListItems.Items.Add(item.FileName)
            Next
        Catch ex As Exception
            ErrorLog.Report("RefreshItemList", ex)
        Finally
            lbFileListItems.EndUpdate()
        End Try
        UpdateItemCount()
    End Sub

    Private Sub cbItemSelector_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles cbItemSelector.KeyPress
        If e.KeyChar = ChrW(13) Then RefreshItemList()
    End Sub

    Private Sub cbItemSelector_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cbItemSelector.SelectedIndexChanged
        RefreshItemList()
    End Sub

    Private Sub lbFileListItems_SelectionInput(ByVal sender As Object, ByVal e As EventArgs) Handles lbFileListItems.KeyUp, lbFileListItems.MouseClick
        UpdateItemCount()
        Dim item = SelectedItemFile()
        If item IsNot Nothing Then ShowItem(item)
    End Sub

    Private Sub lbFileListItems_MouseDoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles lbFileListItems.MouseDoubleClick
        If lbFileListItems.SelectedItem IsNot Nothing Then ProcessHelper.Open(AppState.EditorExe, AppState.ItemPath(lbFileListItems.SelectedItem.ToString()))
    End Sub

    ''' <summary>Preenche os campos com o item.</summary>
    Private Sub ShowItem(ByVal item As ItemFile)
        For Each f In _itemFields
            f.Value.Text = ""
        Next
        tbItmCode.Text = ""
        tbItmWeight.Text = ""
        tbItmLevel.Text = ""
        tbItmInGameNAme.Text = ""
        For Each c In _primaryClassBoxes.Concat(_secondaryClassBoxes)
            c.Value.Checked = False
        Next
        If item Is Nothing Then Return

        tbItmWeight.Text = item.Weight
        tbItmLevel.Text = item.Level
        tbItmCode.Text = item.Code
        tbItmInGameNAme.Text = item.Name
        If cbItemMo.Checked Then
            lbItemMo.BeginUpdate()
            Try
                lbItemMo.Items.Clear()
                For Each monster In AppState.Monsters
                    If MonsterData.DropsItem(monster, item.Code) Then lbItemMo.Items.Add(monster.FileName)
                Next
            Finally
                lbItemMo.EndUpdate()
            End Try
        End If
        For Each line In item.Lines
            If Not line.StartsWith("*") Then Continue For
            If line.StartsWith(GameFields.Item.Spec) Then
                Dim boxes = If(line.StartsWith(GameFields.Item.SpecRandom), _secondaryClassBoxes, _primaryClassBoxes)
                Dim classes As String = line.Substring(GameFields.Item.Spec.Length)
                For Each c In boxes
                    If classes.Contains(c.Key) Then c.Value.Checked = True
                Next
                Continue For
            End If
            For Each f In _itemFields
                If line.StartsWith(f.Key) Then
                    f.Value.Text = ItemData.GetValue(line, f.Key)
                    Exit For
                End If
            Next
        Next
    End Sub

    ''' <summary>Grava os campos no .txt do item.</summary>
    Private Sub SaveItem(ByVal item As ItemFile)
        If item Is Nothing Then
            MsgBox("Please select an item")
            Return
        End If
        Try
            For i = 0 To item.Lines.Count - 1
                Dim line = item.Lines(i)
                If Not line.StartsWith("*") Then Continue For
                If line.StartsWith(GameFields.Item.Spec) Then
                    Dim secondary As Boolean = line.StartsWith(GameFields.Item.SpecRandom)
                    item.Lines(i) = ApplyClasses(line, If(secondary, GameFields.Item.SpecRandom, GameFields.Item.Spec),
                                                 If(secondary, _secondaryClassBoxes, _primaryClassBoxes))
                    Continue For
                End If
                For Each f In _itemFields
                    If line.StartsWith(f.Key) Then
                        item.Lines(i) = f.Key & vbTab & f.Value.Text
                        Exit For
                    End If
                Next
            Next
            ItemData.Save(item, True)
            ShowItem(item)
        Catch ex As Exception
            ErrorLog.Report("SaveItem", ex)
        End Try
    End Sub

    ''' <summary>Acrescenta/remove nomes de classe na linha "**특화"/"**특화랜덤" conforme as caixas marcadas.</summary>
    Private Shared Function ApplyClasses(ByVal line As String, ByVal key As String, ByVal boxes As List(Of KeyValuePair(Of String, CheckBox))) As String
        For Each c In boxes
            Dim present As Boolean = line.Substring(key.Length).Contains(c.Key)
            If c.Value.Checked AndAlso Not present Then
                line &= " " & c.Key
            ElseIf Not c.Value.Checked AndAlso present Then
                line = line.Replace(c.Key, "")
            End If
        Next
        Return line
    End Function

    Private Sub cmdItemSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdItemSave.Click
        SaveItem(SelectedItemFile())
    End Sub

    Private Sub cmdItemRealName_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdItemRealName.Click
        Dim item = SelectedItemFile()
        If item Is Nothing Then
            MsgBox("Please select an item")
        Else
            ProcessHelper.Open(AppState.EditorExe, item.ZhoonFile)
        End If
    End Sub

    Private Sub cbItemMo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cbItemMo.Click
        If Not cbItemMo.Checked Then
            lbItemMo.Items.Clear()
        ElseIf SelectedItemFile() IsNot Nothing Then
            ShowItem(SelectedItemFile())
            UpdateItemCount()
        End If
    End Sub

    Private Sub lbItemMo_MouseDoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles lbItemMo.MouseDoubleClick
        If lbItemMo.SelectedItem IsNot Nothing Then ProcessHelper.Open(AppState.EditorExe, AppState.MonsterPath(lbItemMo.SelectedItem.ToString()))
    End Sub

    Private Sub cmItemsMonster_Opening(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles cmItemsMonster.Opening
        Dim hasSelection As Boolean = lbItemMo.SelectedItems.Count <> 0
        RemoveItemToolStripMenuItem.Enabled = hasSelection
        EditMonsterToolStripMenuItem.Enabled = hasSelection
    End Sub

    Private Sub RemoveItemToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles RemoveItemToolStripMenuItem.Click
        Dim item = SelectedItemFile()
        If item Is Nothing Then Return
        If Not Dialogs.Confirm("Save Monster", "Do you want remove this Item in all selected monsters?") Then
            MsgBox("Monster not saved! Press Reload!")
            Return
        End If
        For Each name As Object In lbItemMo.SelectedItems
            Dim monster = MonsterData.Find(name.ToString())
            If monster Is Nothing Then Continue For
            MonsterData.RemoveItem(monster, item.PlainCode)
            MonsterData.Save(monster, True)
        Next
        MsgBox("Item " & item.PlainCode & " removed from all selected monsters!")
        RefreshItemList()
        lbFileListItems.SelectedItem = item.FileName
        ShowItem(item)
    End Sub

    Private Sub EditMonsterToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles EditMonsterToolStripMenuItem.Click
        Dim monster = MonsterData.Find(lbItemMo.SelectedItem.ToString())
        If monster Is Nothing Then Return
        cbMonsterSelector.Text = "Name=" & monster.Name.Trim(""""c)
        TabControl1.SelectTab(0)
        RefreshMonsterList()
        lbFiles.SelectedItem = monster.FileName
        ShowMonster()
    End Sub

    Private Sub cmFileListItems_Opening(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles cmFileListItems.Opening
        If lbFileListItems.SelectedItems.Count = 0 Then Return
        LoadBackupToolStripMenuItem.DropDownItems.Clear()
        DeleteItemBackupToolStripMenuItem.DropDownItems.Clear()
        For Each backup In BackupHelper.ListBackups(AppState.ServerPath & AppState.ItemFolder, lbFileListItems.SelectedItem.ToString(), AppState.ItemPattern)
            LoadBackupToolStripMenuItem.DropDownItems.Add(backup)
            If Not backup.ToUpper().Contains("DEFAULT") Then DeleteItemBackupToolStripMenuItem.DropDownItems.Add(backup)
        Next
    End Sub

    Private Sub LoadBackupToolStripMenuItem_DropDownItemClicked(ByVal sender As Object, ByVal e As ToolStripItemClickedEventArgs) Handles LoadBackupToolStripMenuItem.DropDownItemClicked
        Dim item = SelectedItemFile()
        If item Is Nothing Then Return
        ItemData.LoadBackup(item, e.ClickedItem.Text)
        ShowItem(item)
    End Sub

    Private Sub DeleteItemBackupToolStripMenuItem_DropDownItemClicked(ByVal sender As Object, ByVal e As ToolStripItemClickedEventArgs) Handles DeleteItemBackupToolStripMenuItem.DropDownItemClicked
        File.Delete(BackupHelper.BackupPath(AppState.ServerPath & AppState.ItemFolder, e.ClickedItem.Text))
        LoadBackupToolStripMenuItem.DropDownItems.Clear()
        DeleteItemBackupToolStripMenuItem.DropDownItems.Clear()
    End Sub

    Private Sub AwardItemsToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AwardItemsToolStripMenuItem.Click
        TabControl1.SelectedTab = TabPageAwardItems
        AwardItemsPanel1.AddItems(SelectedListItems())
    End Sub

    Private Sub PostBoxToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles PostBoxToolStripMenuItem.Click
        TabControl1.SelectedTab = TabPagePostBox
        PostBoxPanel1.AddItems(SelectedListItems())
    End Sub

    Private Function SelectedListItems() As List(Of ItemFile)
        Dim items As New List(Of ItemFile)()
        For Each name As Object In lbFileListItems.SelectedItems
            Dim item = ItemData.Find(name.ToString())
            If item IsNot Nothing Then items.Add(item)
        Next
        Return items
    End Function

    Private Sub SearchItemNameToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles SearchItemNameToolStripMenuItem.Click, AddItemsToolStripMenuItem.Click
        ShowItemSearch()
    End Sub

    Private Sub SendToItemSearcherToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles SendToItemSearcherToolStripMenuItem.Click
        ShowItemSearch()
        For Each name As Object In lbFileListItems.SelectedItems
            Dim item = ItemData.Find(name.ToString())
            If item IsNot Nothing Then _itemSearch.AddResult(item.Code, item.Name)
        Next
    End Sub

    Private Sub ShowItemSearch()
        If _itemSearch Is Nothing OrElse _itemSearch.IsDisposed Then
            _itemSearch = New ItemSearch()
            _itemSearch.CanSendToShop = Function() TabControl1.SelectedTab.Text = TabNpc AndAlso
                                                   lbNPCMapFileList.SelectedItems.Count > 0 AndAlso lbNPCList.SelectedItems.Count > 0
            AddHandler _itemSearch.SendToShop, AddressOf ItemSearch_SendToShop
        End If
        _itemSearch.Show()
    End Sub

    Private Sub ItemSearch_SendToShop(ByVal items As List(Of KeyValuePair(Of String, String)))
        For Each pair In items
            LWNPCshop.Items.Add(pair.Key).SubItems.Add(pair.Value)
        Next
    End Sub

    ' ---- Filtros do combo de itens -------------------------------------------------

    Private Sub CMFilterSaver_Opening(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles CMFilterSaver.Opening
        Dim exists As Boolean = cbItemSelector.Items.Cast(Of Object)().Any(Function(o) o.ToString() = cbItemSelector.Text)
        RemoveFromListToolStripMenuItem.Enabled = exists
        AddToListToolStripMenuItem.Enabled = Not exists
    End Sub

    Private Sub AddToListToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AddToListToolStripMenuItem.Click
        If cbItemSelector.Text = "" Then Return
        If Not cbItemSelector.Items.Cast(Of Object)().Any(Function(o) o.ToString().ToUpper() = cbItemSelector.Text.ToUpper()) Then
            cbItemSelector.Items.Add(cbItemSelector.Text)
        End If
    End Sub

    Private Sub RemoveFromListToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles RemoveFromListToolStripMenuItem.Click
        cbItemSelector.Items.Remove(cbItemSelector.Text)
    End Sub

    Private Sub SaveToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles SaveToolStripMenuItem.Click
        Try
            ConfigFile.SaveItemSelectors(cbItemSelector.Items.Cast(Of Object)().Select(Function(o) o.ToString()))
        Catch ex As Exception
            ErrorLog.Report("Error in SaveFilter", ex)
        End Try
    End Sub

#End Region

#Region "Aba Maps"

    Private Function SelectedMap() As MapFile
        If lbFilesMaps.SelectedItem Is Nothing Then Return Nothing
        Return MapData.Find(lbFilesMaps.SelectedItem.ToString())
    End Function

    Private Sub RefreshMapList()
        lbFilesMaps.BeginUpdate()
        Try
            lbFilesMaps.Items.Clear()
            For Each map In AppState.Maps
                lbFilesMaps.Items.Add(map.FileName)
            Next
        Finally
            lbFilesMaps.EndUpdate()
        End Try
    End Sub

    ''' <summary>Lista de monstros disponiveis para arrastar ao mapa (filtro do combo da aba).</summary>
    Private Sub RefreshMonsterSource()
        lvMonsterSource.BeginUpdate()
        Try
            lvMonsterSource.Items.Clear()
            Dim errorMessage As String = Nothing
            For Each monster In MonsterData.Filter(cbMonsterSelector1.Text, errorMessage)
                lvMonsterSource.Items.Add(monster.FileName).SubItems.Add(monster.Name)
            Next
        Finally
            lvMonsterSource.EndUpdate()
        End Try
    End Sub

    Private Sub cbMonsterSelector1_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cbMonsterSelector1.SelectedIndexChanged
        RefreshMonsterSource()
    End Sub

    Private Sub lbFilesMaps_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles lbFilesMaps.SelectedIndexChanged
        ShowMap(SelectedMap())
    End Sub

    Private Sub lbFilesMaps_MouseDoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles lbFilesMaps.MouseDoubleClick
        If lbFilesMaps.SelectedItem IsNot Nothing Then ProcessHelper.Open(AppState.EditorExe, AppState.FieldPath(lbFilesMaps.SelectedItem.ToString()))
    End Sub

    Private Sub ShowMap(ByVal map As MapFile)
        tbMapValue1.Clear()
        tbMapValue2.Clear()
        tbMapValue3.Clear()
        lvMapMonsters.Items.Clear()
        lvMapBosses.Items.Clear()
        If map Is Nothing Then Return
        lvMapMonsters.BeginUpdate()
        lvMapBosses.BeginUpdate()
        Try
            tbMapValue1.Text = MapData.GetValue(map, GameFields.Map.MaxSpawn)
            tbMapValue2.Text = MapData.GetValue(map, GameFields.Map.SpawnInterval)
            tbMapValue3.Text = MapData.GetValue(map, GameFields.Map.SpawnCount)
            For Each spawn In MapData.GetSpawns(map)
                Dim monster = MonsterData.FindByMapName(spawn.MapName)
                Dim row As ListViewItem
                If monster IsNot Nothing Then
                    row = lvMapMonsters.Items.Add(monster.FileName)
                    row.SubItems.Add(monster.Name)
                Else
                    row = lvMapMonsters.Items.Add("Monster <" & spawn.MapName & "> doesn't exist")
                    row.ForeColor = Color.Red
                    row.SubItems.Add("Monster doesn't exist")
                End If
                row.SubItems.Add(spawn.Rate)
                row.Tag = spawn
            Next
            For Each boss In MapData.GetBosses(map)
                Dim bossMonster = MonsterData.FindByMapName(boss.BossMapName)
                Dim guardMonster = MonsterData.FindByMapName(boss.GuardMapName)
                Dim row = lvMapBosses.Items.Add(If(bossMonster IsNot Nothing, bossMonster.FileName, "Monster <" & boss.BossMapName & "> doesn't exist"))
                row.SubItems.Add(If(bossMonster IsNot Nothing, bossMonster.Name, "Monster <" & boss.BossMapName & "> doesn't exist"))
                row.SubItems.Add(If(guardMonster IsNot Nothing, guardMonster.FileName, "Monster <" & boss.GuardMapName & "> doesn't exist"))
                row.SubItems.Add(If(guardMonster IsNot Nothing, guardMonster.Name, "Monster <" & boss.GuardMapName & "> doesn't exist"))
                row.SubItems.Add(boss.GuardCount)
                row.SubItems.Add(boss.SpawnTimes)
                If bossMonster Is Nothing OrElse guardMonster Is Nothing Then row.ForeColor = Color.Red
                row.Tag = boss
            Next
        Catch ex As Exception
            ErrorLog.Report("ShowMap", ex)
        Finally
            lvMapMonsters.EndUpdate()
            lvMapBosses.EndUpdate()
        End Try
    End Sub

    ''' <summary>Nome de mapa (*이름) do monstro da linha; se o arquivo nao existir mais, usa o nome que estava no .spm.</summary>
    Private Shared Function MapNameOf(ByVal fileName As String, ByVal fallback As String) As String
        Dim monster = MonsterData.Find(fileName)
        Return If(monster IsNot Nothing, monster.MapName, fallback)
    End Function

    Private Sub SaveMap(ByVal map As MapFile)
        If map Is Nothing Then
            MsgBox("Please select a map")
            Return
        End If
        Try
            Dim spawns As New List(Of MapData.SpawnEntry)()
            For Each row As ListViewItem In lvMapMonsters.Items
                Dim original = TryCast(row.Tag, MapData.SpawnEntry)
                spawns.Add(New MapData.SpawnEntry With {
                    .MapName = MapNameOf(row.Text, If(original Is Nothing, "", original.MapName)),
                    .Rate = row.SubItems(2).Text})
            Next
            Dim bosses As New List(Of MapData.BossEntry)()
            For Each row As ListViewItem In lvMapBosses.Items
                Dim original = TryCast(row.Tag, MapData.BossEntry)
                bosses.Add(New MapData.BossEntry With {
                    .BossMapName = MapNameOf(row.Text, If(original Is Nothing, "", original.BossMapName)),
                    .GuardMapName = MapNameOf(row.SubItems(2).Text, If(original Is Nothing, "", original.GuardMapName)),
                    .GuardCount = row.SubItems(4).Text,
                    .SpawnTimes = row.SubItems(5).Text})
            Next
            MapData.Save(map, tbMapValue1.Text, tbMapValue2.Text, tbMapValue3.Text, spawns, bosses)
        Catch ex As Exception
            ErrorLog.Report("SaveMap", ex)
        End Try
    End Sub

    Private Sub cmdMapSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdMapSave.Click
        SaveMap(SelectedMap())
    End Sub

    Private Sub cmMapsFiles_Opening(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles cmMapsFiles.Opening
        ChangeEXPOfMapToolStripMenuItem.Enabled = lbFilesMaps.SelectedItems.Count <> 0
    End Sub

    ' ---- Monstros disponiveis -> mapa ------------------------------------------------

    Private Sub AddToMapToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AddToMapToolStripMenuItem.Click
        If lvMonsterSource.SelectedItems.Count < 1 Then Return
        If lbFilesMaps.SelectedItems.Count < 1 Then
            MsgBox("Please select a map first")
            Return
        End If
        If Not IsNumeric(tbMapSpawnRate.Text) Then
            MsgBox("Spawn rate is not numeric!")
            Return
        End If
        tbMapSpawnRate.Text = Math.Abs(CInt(tbMapSpawnRate.Text)).ToString()
        For Each source As ListViewItem In lvMonsterSource.SelectedItems
            Dim row = lvMapMonsters.Items.Add(source.Text)
            row.SubItems.Add(source.SubItems(1).Text)
            row.SubItems.Add(tbMapSpawnRate.Text)
        Next
    End Sub

    Private Sub AddAsBossToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AddAsBossToolStripMenuItem.Click
        If lvMapBosses.SelectedItems.Count < 1 Then
            MsgBox("Please select a boss!")
            Return
        End If
        If lvMonsterSource.SelectedItems.Count < 1 Then Return
        For Each row As ListViewItem In lvMapBosses.SelectedItems
            row.Text = lvMonsterSource.SelectedItems(0).Text
            row.SubItems(1).Text = lvMonsterSource.SelectedItems(0).SubItems(1).Text
        Next
    End Sub

    Private Sub AddAsGuardToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AddAsGuardToolStripMenuItem.Click
        If lvMapBosses.SelectedItems.Count < 1 Then
            MsgBox("Please select a boss!")
            Return
        End If
        If lvMonsterSource.SelectedItems.Count < 1 Then Return
        For Each row As ListViewItem In lvMapBosses.SelectedItems
            row.SubItems(2).Text = lvMonsterSource.SelectedItems(0).Text
            row.SubItems(3).Text = lvMonsterSource.SelectedItems(0).SubItems(1).Text
        Next
    End Sub

    Private Sub AddAsNewBossToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AddAsNewBossToolStripMenuItem.Click
        If lvMonsterSource.SelectedItems.Count < 1 Then Return
        Dim row = lvMapBosses.Items.Add(lvMonsterSource.SelectedItems(0).Text)
        row.SubItems.Add(lvMonsterSource.SelectedItems(0).SubItems(1).Text)
        row.SubItems.Add("add a guard")
        row.SubItems.Add("add a guard")
        row.SubItems.Add("7")
        row.SubItems.Add("0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15 16 17 18 19 20 21 22 23")
    End Sub

    ' ---- Monstros no mapa -------------------------------------------------------------

    Private Sub RemoveMonsterToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles RemoveMonsterToolStripMenuItem.Click
        For Each row As ListViewItem In lvMapMonsters.SelectedItems.Cast(Of ListViewItem)().ToList()
            row.Remove()
        Next
    End Sub

    Private Sub RemoveAllToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles RemoveAllToolStripMenuItem.Click
        lvMapMonsters.Items.Clear()
    End Sub

    Private Sub EditSpawnRateToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles EditSpawnRateToolStripMenuItem.Click
        EditSubItem(lvMapMonsters, 2, "Enter spawn rate", True)
    End Sub

    ' ---- Chefes no mapa -----------------------------------------------------------------

    Private Sub RemoveBossToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles RemoveBossToolStripMenuItem.Click
        For Each row As ListViewItem In lvMapBosses.SelectedItems.Cast(Of ListViewItem)().ToList()
            row.Remove()
        Next
    End Sub

    Private Sub RemoveAllBossesToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles RemoveAllBossesToolStripMenuItem.Click
        lvMapBosses.Items.Clear()
    End Sub

    Private Sub EditRateToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles EditRateToolStripMenuItem.Click
        EditSubItem(lvMapBosses, 4, "Enter guard count", True)
    End Sub

    Private Sub EditSpawnTimesToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles EditSpawnTimesToolStripMenuItem.Click
        EditSubItem(lvMapBosses, 5, "Enter spawn times", False)
    End Sub

    ''' <summary>Edita uma coluna da linha selecionada via InputBox; valor invalido mantem o anterior.</summary>
    Private Shared Sub EditSubItem(ByVal list As ListView, ByVal column As Integer, ByVal prompt As String, ByVal numeric As Boolean)
        If list.SelectedItems.Count < 1 Then Return
        Dim cell = list.SelectedItems(0).SubItems(column)
        Dim previous As String = cell.Text
        Dim entered As String = InputBox(prompt, "", previous)
        If entered = "" OrElse (numeric AndAlso Not IsNumeric(entered)) Then
            cell.Text = previous
        Else
            cell.Text = entered
        End If
    End Sub

#End Region

#Region "Aba NPC Editor"

    Private Function SelectedSpc() As BinaryDataFile
        If lbNPCMapFileList.SelectedItem Is Nothing Then Return Nothing
        Return NpcData.FindSpc(lbNPCMapFileList.SelectedItem.ToString())
    End Function

    ''' <summary>
    ''' Preenche a lista de mapas mantendo o mapa aberto (com as edicoes em memoria). reopen (ou dados relidos do disco)
    ''' reabre o mapa, descartando a copia anterior.
    ''' </summary>
    Private Sub FillNpcMapList(Optional ByVal reopen As Boolean = False)
        reopen = reopen OrElse _npcMapStale
        _npcMapStale = False
        Dim keep As String = _npcMapName
        _npcMapChanging = True
        lbNPCMapFileList.BeginUpdate()
        Try
            lbNPCMapFileList.Items.Clear()
            For Each spcFile In AppState.SpcFiles
                lbNPCMapFileList.Items.Add(spcFile.FileName)
            Next
            If keep <> "" AndAlso lbNPCMapFileList.Items.Contains(keep) Then
                lbNPCMapFileList.SelectedItem = keep
            Else
                _npcMapName = ""
            End If
        Finally
            lbNPCMapFileList.EndUpdate()
            _npcMapChanging = False
        End Try
        If reopen AndAlso _npcMapName <> "" Then OpenNpcMap(SelectedSpc())
    End Sub

    Private Sub lbNPCMapFileList_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles lbNPCMapFileList.SelectedIndexChanged
        If _npcMapChanging Then Return
        Dim spc = SelectedSpc()
        If spc Is Nothing Then Return
        If _npcMapName <> "" AndAlso Not _npcMapName.Equals(spc.FileName, StringComparison.OrdinalIgnoreCase) AndAlso Not ConfirmDiscardNpc() Then
            _npcMapChanging = True
            Try
                lbNPCMapFileList.SelectedItem = _npcMapName
            Finally
                _npcMapChanging = False
            End Try
            Return
        End If
        OpenNpcMap(spc)
    End Sub

    ''' <summary>Abre o mapa: vista (terreno ou minimapa), lista de NPCs e ponto de partida do Desfazer/Save.</summary>
    Private Sub OpenNpcMap(ByVal spc As BinaryDataFile)
        _npcMapName = spc.FileName
        Dim mapName As String = spc.FileName.ToUpper().Replace(".ASE.SPC", "")
        Dim imageName As String = Nothing
        If Not MapImageNames.TryGetValue(mapName, imageName) Then imageName = mapName
        _mapImagePath = AppState.MapImagePath(imageName & ".BMP")
        If Not MapRects.TryGetValue(mapName, _mapRect) Then _mapRect = DefaultMapRect
        _npcHistory = New NpcUndoHistory()
        _npcHistory.MarkSaved(spc.Data)
        Cursor = Cursors.WaitCursor
        Try
            LoadNpcMapView(spc)
        Finally
            Cursor = Cursors.Default
        End Try
        ShowNpcList(spc)
        UpdateNpcSaveButton()
        SetNpcStatus("", False)
    End Sub

    ''' <summary>Mostra o terreno do .smd do mapa; sem ele, usa o minimapa (.bmp) como fundo.</summary>
    Private Sub LoadNpcMapView(ByVal spc As BinaryDataFile)
        _npcMesh = Nothing
        Dim meshPath As String = StageMesh.FindFile(spc.FileName)
        If meshPath <> "" Then
            Try
                _npcMesh = New StageMesh(meshPath)
            Catch ex As Exception
                ErrorLog.Write("NPC Editor: could not read " & meshPath & vbCrLf & ex.ToString())
            End Try
        End If
        If _npcMesh IsNot Nothing Then
            NpcCanvas.SetBackdrop(Nothing, 0, 0, 0, 0)
            NpcCanvas.SetMesh(_npcMesh)
            lblNpcSource.Text = "Terrain (.smd)"
        Else
            NpcCanvas.SetMesh(Nothing)
            LoadMapImage()
            NpcCanvas.SetBackdrop(_mapImage, _mapRect.Left, _mapRect.Right, _mapRect.Top, _mapRect.Bottom)
            lblNpcSource.Text = If(meshPath = "", "No .smd: minimap image", "Bad .smd: minimap image")
        End If
    End Sub

    ''' <summary>Carrega a imagem do mapa (ou error.bmp) usada como fundo quando nao ha .smd.</summary>
    Private Sub LoadMapImage()
        If _mapImage IsNot Nothing Then _mapImage.Dispose()
        _mapImage = Nothing
        Try
            _mapImage = New Bitmap(_mapImagePath)
        Catch
            Try
                _mapImage = New Bitmap(AppState.MapImagePath("error.bmp"))
            Catch
                _mapImage = New Bitmap(256, 256)
            End Try
        End Try
    End Sub

    ''' <summary>Preenche a lista de NPCs do .spc (100 slots) e marca todos no mapa.</summary>
    Private Sub ShowNpcList(ByVal spc As BinaryDataFile)
        lbNPCList.BeginUpdate()
        Try
            lbNPCList.Items.Clear()
            For i = 0 To NpcData.RecordCount - 1
                If NpcData.IsRecordUsed(spc, i) Then
                    Dim rec = NpcData.ReadRecord(spc, i)
                    Dim npc = NpcData.FindNpc(Path.GetFileName(rec.SetupFile))
                    lbNPCList.Items.Add(If(npc Is Nothing, "NPC file not found!", npc.Name))
                Else
                    lbNPCList.Items.Add(NpcData.Empty)
                End If
            Next
        Finally
            lbNPCList.EndUpdate()
        End Try
        RefreshNpcCanvas()
    End Sub

    ''' <summary>Refaz os marcadores do mapa a partir do .spc em memoria (descarta o que foi movido e nao gravado).</summary>
    Private Sub RefreshNpcCanvas()
        Dim spc = SelectedSpc()
        Dim points As New List(Of SpawnMapCanvas.CanvasPoint)()
        If spc IsNot Nothing Then
            For i = 0 To Math.Min(lbNPCList.Items.Count, NpcData.RecordCount) - 1
                If Not NpcData.IsRecordUsed(spc, i) Then Continue For
                Dim rec = NpcData.ReadRecord(spc, i)
                points.Add(New SpawnMapCanvas.CanvasPoint() With {
                    .Slot = i, .X = rec.X, .Z = rec.Z, .Angle = rec.Angle, .Label = lbNPCList.Items(i).ToString()})
            Next
        End If
        NpcCanvas.SetPoints(points)
        NpcCanvas.SelectedSlot = lbNPCList.SelectedIndex
    End Sub

    ''' <summary>Passa a posicao e a direcao digitadas (ainda nao gravadas) do NPC selecionado para o mapa.</summary>
    Private Sub DrawSelectedNpc()
        Dim x, z, angle As Integer
        If lbNPCList.SelectedIndex < 0 Then Return
        If Not Integer.TryParse(tbNPCx.Text, x) OrElse Not Integer.TryParse(tbNPCz.Text, z) Then Return
        Integer.TryParse(tbNPCangle.Text, angle)
        NpcCanvas.UpdatePoint(lbNPCList.SelectedIndex, x, z, angle)
    End Sub

    Private Sub lbNPCList_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles lbNPCList.SelectedIndexChanged
        Dim spc = SelectedSpc()
        If spc Is Nothing Then
            MsgBox("Please select a map")
            Return
        End If
        If lbNPCList.SelectedIndex < 0 Then Return
        Dim rec = NpcData.ReadRecord(spc, lbNPCList.SelectedIndex)
        cbNPCAktivated.Checked = rec.Active
        tbNPCModelFile.Text = rec.ModelFile
        tbNPCSetupfile.Text = rec.SetupFile
        tbNPCID.Text = rec.Id.ToString()
        tbNPCx.Text = rec.X.ToString()
        tbNPCy.Text = rec.Y.ToString()
        tbNPCz.Text = rec.Z.ToString()
        tbNPCangle.Text = rec.Angle.ToString()

        Dim npcPath As String = AppState.ServerPath & rec.SetupFile
        tbNPCName.Text = NpcData.ReadName(npcPath)
        Dim npc = NpcData.FindNpc(Path.GetFileName(rec.SetupFile))
        If npc IsNot Nothing Then tbNPCSetupfileInI.Text = npc.SetupIni
        tbNPCJ_Chat.Text = String.Join(vbCrLf, NpcData.ReadChat(npcPath)) & vbCrLf

        FillNpcShop()
        RefreshNpcCanvas()
        If NpcData.IsRecordUsed(spc, lbNPCList.SelectedIndex) Then
            ' Slot vazio tem posicao 0,0 (fora do mapa): so centraliza em NPC de verdade.
            If Not _npcFromCanvas Then NpcCanvas.CenterOn(rec.X, rec.Z)
        Else
            SetNpcStatus("Slot #" & lbNPCList.SelectedIndex & " is empty: use Add new NPC (right-click the list) or click the map with " &
                         "Click map to add NPCs to place an NPC here.", False)
        End If
    End Sub

#Region "Mapa do NPC Editor"

    Private Sub NpcCanvas_PointSelected(ByVal slot As Integer) Handles NpcCanvas.PointSelected
        _npcFromCanvas = True
        Try
            If slot >= 0 AndAlso slot < lbNPCList.Items.Count Then lbNPCList.SelectedIndex = slot
        Finally
            _npcFromCanvas = False
        End Try
    End Sub

    ''' <summary>Arrastou o NPC: X e Z mudam (e o Y acompanha o terreno) na copia em memoria, com Desfazer; Save NPC grava.</summary>
    Private Sub NpcCanvas_PointMoved(ByVal slot As Integer, ByVal x As Integer, ByVal z As Integer) Handles NpcCanvas.PointMoved
        If lbNPCList.SelectedIndex <> slot Then Return
        tbNPCx.Text = x.ToString()
        tbNPCz.Text = z.ToString()
        Dim height As Single
        If _npcMesh IsNot Nothing AndAlso _npcMesh.TryHeightAt(x, z, height) Then tbNPCy.Text = CInt(Math.Round(height)).ToString()
        ApplyNpcFields("Moved NPC")
    End Sub

    Private Sub NpcCanvas_AddRequested(ByVal x As Integer, ByVal z As Integer) Handles NpcCanvas.AddRequested
        CreateNpc(True, x, z)
    End Sub

    Private Sub NpcCanvas_DeleteRequested() Handles NpcCanvas.DeleteRequested
        DeleteNPCToolStripMenuItem_Click(Me, EventArgs.Empty)
    End Sub

    ''' <summary>PageUp/PageDown giram o NPC em passos de 8 (0..4096).</summary>
    Private Sub NpcCanvas_RotateRequested(ByVal direction As Integer) Handles NpcCanvas.RotateRequested
        If lbNPCList.SelectedIndex < 0 Then Return
        Dim angle As Integer
        Integer.TryParse(tbNPCangle.Text, angle)
        angle += direction * 8
        If angle < 0 Then angle = MaxAngle
        If angle > MaxAngle Then angle = 0
        tbNPCangle.Text = angle.ToString()
        ApplyNpcFields("Rotated NPC")
    End Sub

    Private Sub NpcCanvas_HoverChanged(ByVal x As Integer, ByVal z As Integer, ByVal hasHeight As Boolean, ByVal height As Single) Handles NpcCanvas.HoverChanged
        lblNpcHover.Text = "X: " & x & "   Z: " & z & If(hasHeight, "   Y: " & CInt(Math.Round(height)), "")
    End Sub

    Private Sub chkNpcAdd_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkNpcAdd.CheckedChanged
        NpcCanvas.AddOnClick = chkNpcAdd.Checked
        If chkNpcAdd.Checked Then SetNpcStatus("Click an empty spot to add an NPC.", False)
    End Sub

    Private Sub chkNpcNames_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkNpcNames.CheckedChanged
        NpcCanvas.ShowLabels = chkNpcNames.Checked
    End Sub

    ''' <summary>Clique direito no mapa: leva o NPC selecionado para o centro da vista (e para a altura do terreno ali).</summary>
    Private Sub BringBackToMapToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles BringBackToMapToolStripMenuItem.Click
        If lbNPCList.SelectedIndex < 0 Then Return
        NpcCanvas_PointMoved(lbNPCList.SelectedIndex, NpcCanvas.ViewCenterX, NpcCanvas.ViewCenterZ)
    End Sub

#End Region

    ''' <summary>Save: grava o .spc do mapa (com backup) e o nome/falas do NPC selecionado no .zhoon.</summary>
    Private Sub cmdNPCSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdNPCSave.Click
        Dim spc = SelectedSpc()
        If spc Is Nothing Then
            MsgBox("Please select a map file")
            Return
        End If
        If Not File.Exists(AppState.FieldPath(spc.FileName)) Then
            MsgBox("No NPC file found! Can't save the NPC")
            Return
        End If
        Dim index As Integer = lbNPCList.SelectedIndex
        Dim hasNpc As Boolean = index >= 0 AndAlso NpcData.IsRecordUsed(spc, index)
        If hasNpc Then
            Dim rec As NpcData.NpcRecord = Nothing
            Dim problem As String = ""
            If Not TryReadNpcFields(rec, problem) Then
                MsgBox(problem & vbCrLf & "NPC not saved!")
                Return
            End If
            ApplyNpcFields("Edited NPC")
        End If
        Try
            If hasNpc Then SaveNpcZhoon()
            NpcData.SaveSpc(spc)
            _npcHistory.MarkSaved(spc.Data)
            UpdateNpcSaveButton()
            SetNpcStatus("Saved " & spc.FileName & ". Restart the server to apply.", False)
        Catch ex As Exception
            ErrorLog.Report("SaveNpc", ex)
        End Try
    End Sub

    ''' <summary>Grava nome e falas do NPC selecionado no .zhoon (cria um se nao existir, depois de confirmar).</summary>
    Private Sub SaveNpcZhoon()
        Dim zhoonPath As String = AppState.ServerPath & AppState.NpcFolder & NpcData.ReadZhoonReference(AppState.ServerPath & tbNPCSetupfile.Text)
        If Not File.Exists(zhoonPath) Then
            If Not Dialogs.Confirm("Warning!", "No Zhoon file found!" & vbCrLf & "Create one for this NPC?") Then Return
            zhoonPath = AppState.ServerPath & AppState.NpcNameFolder & Path.GetFileNameWithoutExtension(tbNPCSetupfile.Text) & ".zhoon"
        End If
        NpcData.SaveZhoon(zhoonPath, Path.GetFileName(tbNPCSetupfile.Text), tbNPCName.Text, tbNPCJ_Chat.Lines)
    End Sub

    ' ---- Loja do NPC ------------------------------------------------------------------

    Private Sub FillNpcShop()
        LWNPCshop.BeginUpdate()
        Try
            LWNPCshop.Items.Clear()
            If CBShopItem.SelectedIndex < 0 Then Return
            Dim shopKey As String = GameFields.Npc.ShopKeys(CBShopItem.SelectedIndex)
            For Each code In NpcData.ReadShopItems(AppState.ServerPath & tbNPCSetupfile.Text, shopKey)
                If code = GameFields.Monster.DropNothing Then
                    LWNPCshop.Items.Add(NpcData.Empty)
                    Continue For
                End If
                Dim item = ItemData.FindByCode("""" & code.ToUpper() & """")
                If item Is Nothing Then
                    LWNPCshop.Items.Add("No code").SubItems.Add("Item doesn't exist")
                Else
                    LWNPCshop.Items.Add(item.Code).SubItems.Add(item.Name)
                End If
            Next
        Finally
            LWNPCshop.EndUpdate()
        End Try
    End Sub

    Private Sub CBShopItem_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles CBShopItem.SelectedIndexChanged
        FillNpcShop()
    End Sub

    Private Sub RemoveToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles RemoveToolStripMenuItem.Click
        For Each row As ListViewItem In LWNPCshop.SelectedItems.Cast(Of ListViewItem)().ToList()
            row.Remove()
        Next
    End Sub

    Private Sub CMDNPCShopSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CMDNPCShopSave.Click
        Dim npcPath As String = AppState.ServerPath & tbNPCSetupfile.Text
        If Not File.Exists(npcPath) Then
            MsgBox("File doesn't exist: " & npcPath)
            Return
        End If
        If CBShopItem.SelectedIndex < 0 Then Return
        Dim codes As New List(Of String)()
        For Each row As ListViewItem In LWNPCshop.Items
            If row.Text <> NpcData.Empty AndAlso row.Text <> "No code" AndAlso row.Text <> "" Then codes.Add(row.Text.Replace("""", ""))
        Next
        Dim index As Integer = lbNPCList.SelectedIndex
        Try
            NpcData.SaveShop(npcPath, GameFields.Npc.ShopKeys(CBShopItem.SelectedIndex), codes, tbNPCSetupfileInI.Text)
            NpcData.LoadNpcFiles()
            RefreshNpcCombos()
            If index >= 0 Then lbNPCList.SetSelected(index, True)
        Catch ex As Exception
            ErrorLog.Report("SaveNpcShop", ex)
        End Try
    End Sub

    ' ---- Menu da lista de NPCs ----------------------------------------------------------

    ' ---- Edicao em memoria e Desfazer ----------------------------------------------------
    ' Mover, girar, editar campos, adicionar, apagar e trocar de lugar so mudam a copia em memoria do .spc (spc.Data),
    ' como o Spawn Point; o botao Save NPC grava (com backup). .NPC e .zhoon (loja, nome, falas, arquivos criados pelo
    ' Add) sao outros arquivos e nao entram no Desfazer.

    ''' <summary>Ha alteracoes no .spc aberto ainda nao gravadas (desfazer ate o estado salvo zera isso).</summary>
    Private ReadOnly Property NpcIsDirty As Boolean
        Get
            Dim spc = If(_npcMapName = "", Nothing, NpcData.FindSpc(_npcMapName))
            Return spc IsNot Nothing AndAlso _npcHistory.IsModified(spc.Data)
        End Get
    End Property

    Private Sub UpdateNpcSaveButton()
        cmdNPCSave.Text = If(NpcIsDirty, "Save NPC *", "Save NPC")
    End Sub

    Private Sub SetNpcStatus(ByVal text As String, ByVal isError As Boolean)
        lblNpcStatus.Text = text
        lblNpcStatus.ForeColor = If(isError, Color.Firebrick, SystemColors.ControlText)
    End Sub

    ''' <summary>Se ha alteracoes pendentes, pergunta se pode descartar (e as descarta, relendo o .spc do disco).</summary>
    Private Function ConfirmDiscardNpc() As Boolean
        If Not NpcIsDirty Then Return True
        If Not Dialogs.Confirm("NPC Editor", "Discard the unsaved changes to " & _npcMapName & "?", MsgBoxStyle.Exclamation) Then Return False
        NpcData.ReloadSpc(_npcMapName)
        _npcHistory = New NpcUndoHistory()
        Return True
    End Function

    ''' <summary>Le os campos da tela num registro. Retorna False (com o motivo) se algum valor for invalido.</summary>
    Private Function TryReadNpcFields(ByRef rec As NpcData.NpcRecord, ByRef problem As String) As Boolean
        rec = New NpcData.NpcRecord()
        If tbNPCModelFile.Text.Length > NpcData.TextFieldSize Then
            problem = "Error: Max. 64 chars supported in NPC model file"
            Return False
        End If
        If tbNPCSetupfile.Text.Length > NpcData.TextFieldSize Then
            problem = "Error: Max. 64 chars supported in NPC setup file"
            Return False
        End If
        Dim id, x, y, z, angle As Integer
        If Not (Integer.TryParse(tbNPCID.Text, id) AndAlso Integer.TryParse(tbNPCx.Text, x) AndAlso Integer.TryParse(tbNPCy.Text, y) AndAlso
                Integer.TryParse(tbNPCz.Text, z) AndAlso Integer.TryParse(tbNPCangle.Text, angle)) Then
            problem = "Error: Please check all values, one or some are not numeric!"
            Return False
        End If
        If id < 0 OrElse id > NpcData.MaxId Then
            problem = "Error: ID value is too high!"
            Return False
        End If
        rec.Active = cbNPCAktivated.Checked
        rec.ModelFile = tbNPCModelFile.Text
        rec.SetupFile = tbNPCSetupfile.Text
        rec.Id = id : rec.X = x : rec.Y = y : rec.Z = z : rec.Angle = angle
        Return True
    End Function

    ''' <summary>Passa os campos da tela para o registro do NPC selecionado (em memoria), se algo mudou. Um passo no Desfazer.</summary>
    Private Sub ApplyNpcFields(ByVal description As String)
        Dim spc = SelectedSpc()
        Dim index As Integer = lbNPCList.SelectedIndex
        If spc Is Nothing OrElse index < 0 OrElse Not NpcData.IsRecordUsed(spc, index) Then Return
        Dim rec As NpcData.NpcRecord = Nothing
        Dim problem As String = ""
        If Not TryReadNpcFields(rec, problem) Then
            SetNpcStatus(problem, True)
            Return
        End If
        Dim current = NpcData.ReadRecord(spc, index)
        If current.Active = rec.Active AndAlso current.ModelFile = rec.ModelFile AndAlso current.SetupFile = rec.SetupFile AndAlso
           current.Id = rec.Id AndAlso current.X = rec.X AndAlso current.Y = rec.Y AndAlso current.Z = rec.Z AndAlso current.Angle = rec.Angle Then Return
        _npcHistory.Record(spc.Data, description & " #" & index)
        NpcData.WriteRecord(spc, index, rec)
        RefreshNpcCanvas()
        UpdateNpcSaveButton()
        SetNpcStatus(description & " #" & index & " (not saved yet).", False)
    End Sub

    ''' <summary>Sair de um campo (ou marcar Activated) aplica a edicao ao registro, como o Spawn Point faz com X/Z.</summary>
    Private Sub NpcField_Validated(ByVal sender As Object, ByVal e As EventArgs) Handles tbNPCID.Validated, tbNPCx.Validated, tbNPCy.Validated,
        tbNPCz.Validated, tbNPCangle.Validated, tbNPCModelFile.Validated, tbNPCSetupfile.Validated, cbNPCAktivated.Click
        ApplyNpcFields("Edited NPC")
    End Sub

    ''' <summary>Depois de uma alteracao que muda a lista (adicionar, apagar, trocar de lugar, desfazer): refaz tudo e mantem a selecao.</summary>
    Private Sub AfterNpcListChange(ByVal spc As BinaryDataFile, ByVal selectIndex As Integer, ByVal message As String)
        ShowNpcList(spc)
        If selectIndex >= 0 AndAlso selectIndex < lbNPCList.Items.Count Then lbNPCList.SetSelected(selectIndex, True)
        UpdateNpcSaveButton()
        SetNpcStatus(message, False)
    End Sub

    ''' <summary>Ctrl+Z desfaz e Ctrl+Y / Ctrl+Shift+Z refazem, na aba NPC Editor. Em campos de texto o Ctrl+Z continua sendo o do campo.</summary>
    Private Sub UndoRedoNpc(ByVal redo As Boolean)
        Dim spc = SelectedSpc()
        If spc Is Nothing Then Return
        Dim description As String = ""
        Dim data As Byte() = If(redo, _npcHistory.Redo(spc.Data, description), _npcHistory.Undo(spc.Data, description))
        If data Is Nothing Then Return
        Array.Copy(data, spc.Data, spc.Data.Length)
        AfterNpcListChange(spc, lbNPCList.SelectedIndex, If(redo, "Redid: ", "Undid: ") & description & ".")
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean
        If TabControl1.SelectedTab IsNot Nothing AndAlso TabControl1.SelectedTab.Text = TabNpc Then
            Dim focused As Control = ActiveControl
            Do While TypeOf focused Is ContainerControl AndAlso DirectCast(focused, ContainerControl).ActiveControl IsNot Nothing
                focused = DirectCast(focused, ContainerControl).ActiveControl
            Loop
            If Not (TypeOf focused Is TextBoxBase OrElse TypeOf focused Is ComboBox) Then
                Select Case keyData
                    Case Keys.Control Or Keys.Z
                        UndoRedoNpc(False)
                        Return True
                    Case Keys.Control Or Keys.Y, Keys.Control Or Keys.Shift Or Keys.Z
                        UndoRedoNpc(True)
                        Return True
                End Select
            End If
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub UpToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles UpToolStripMenuItem.Click
        Dim spc = SelectedSpc()
        Dim index As Integer = lbNPCList.SelectedIndex
        If spc Is Nothing OrElse index <= 0 Then Return
        _npcHistory.Record(spc.Data, "Moved NPC #" & index & " up")
        NpcData.SwapRecords(spc, index - 1, index)
        AfterNpcListChange(spc, index - 1, "Moved NPC #" & index & " up (not saved yet).")
    End Sub

    Private Sub DownToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DownToolStripMenuItem.Click
        Dim spc = SelectedSpc()
        Dim index As Integer = lbNPCList.SelectedIndex
        If spc Is Nothing OrElse index < 0 OrElse index >= NpcData.RecordCount - 1 Then Return
        _npcHistory.Record(spc.Data, "Moved NPC #" & index & " down")
        NpcData.SwapRecords(spc, index, index + 1)
        AfterNpcListChange(spc, index + 1, "Moved NPC #" & index & " down (not saved yet).")
    End Sub

    Private Sub DeleteNPCToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles DeleteNPCToolStripMenuItem.Click
        Dim spc = SelectedSpc()
        Dim index As Integer = lbNPCList.SelectedIndex
        If spc Is Nothing OrElse index < 0 OrElse Not NpcData.IsRecordUsed(spc, index) Then Return
        _npcHistory.Record(spc.Data, "Deleted NPC #" & index)
        NpcData.ClearRecord(spc, index)
        AfterNpcListChange(spc, -1, "Deleted NPC #" & index & " (not saved yet; Ctrl+Z undoes).")
    End Sub

    Private Sub AddNewNPCToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles AddNewNPCToolStripMenuItem.Click
        CreateNpc(False, 0, 0)
    End Sub

    ''' <summary>Copia o .NPC de origem para um .NPC novo e cria o .zhoon (nome e uma fala padrao). Retorna o caminho do .NPC para o registro.</summary>
    Private Function CreateNpcFiles(ByVal sourcePath As String, ByVal name As String) As String
        Dim newNpcPath As String = AppState.NpcPath(name & ".NPC")
        File.Copy(sourcePath, newNpcPath, False)
        NpcData.SetZhoonReference(newNpcPath, name)
        Using writer As New StreamWriter(AppState.ServerPath & AppState.NpcNameFolder & name & ".zhoon", False, AppState.FileEncoding)
            writer.WriteLine(ZhoonFile.NameLine(name) & vbCrLf & vbCrLf)
            writer.WriteLine(ZhoonFile.ChatLine("""What im talking?!?"""))
        End Using
        Return "GameServer\npc\" & name & ".NPC"
    End Function

    ''' <summary>Ctrl+clique num NPC (nao precisa de "Click map to add NPCs"): abre a janela para trocar o NPC daquele slot.</summary>
    Private Sub NpcCanvas_ReplaceRequested(ByVal slot As Integer) Handles NpcCanvas.ReplaceRequested
        ReplaceNpc(slot)
    End Sub

    ''' <summary>
    ''' Troca o NPC do slot por outro (a janela e a do Add NPC): muda o .NPC e o modelo do registro e mantem posicao, direcao,
    ''' ID e Activated. E uma edicao em memoria (Desfazer/Save); com "criar a partir de", o .NPC/.zhoon novo e gravado na hora.
    ''' </summary>
    Private Sub ReplaceNpc(ByVal slot As Integer)
        Dim spc = SelectedSpc()
        If spc Is Nothing OrElse slot < 0 OrElse slot >= NpcData.RecordCount OrElse Not NpcData.IsRecordUsed(spc, slot) Then Return
        If lbNPCList.SelectedIndex <> slot Then lbNPCList.SelectedIndex = slot
        ApplyNpcFields("Edited NPC")

        Dim current = NpcData.ReadRecord(spc, slot)
        Dim copy As Boolean, sourcePath As String, name As String
        Using dialog As New AddNpcDialog(AppState.Npcs, "slot #" & slot & " (" & lbNPCList.Items(slot).ToString() & ")", Path.GetFileName(current.SetupFile))
            If dialog.ShowDialog(Me) <> DialogResult.OK Then
                SetNpcStatus("Replace NPC canceled.", False)
                Return
            End If
            copy = dialog.CreateCopy
            sourcePath = dialog.SourcePath
            name = dialog.NewName
        End Using

        Try
            Dim setupFile As String = If(copy, CreateNpcFiles(sourcePath, name), "GameServer\npc\" & Path.GetFileName(sourcePath))
            If setupFile.Equals(current.SetupFile, StringComparison.OrdinalIgnoreCase) Then
                SetNpcStatus("Slot #" & slot & " already uses " & Path.GetFileName(setupFile) & ".", False)
                Return
            End If
            ' O modelo (char\npc\...) vem do .NPC novo; sem ele no arquivo, mantem o atual.
            Dim model As String = NpcData.ReadModelFile(sourcePath)
            Dim rec As NpcData.NpcRecord = current
            rec.SetupFile = setupFile
            If model <> "" AndAlso model.Length <= NpcData.TextFieldSize Then rec.ModelFile = model
            _npcHistory.Record(spc.Data, "Replaced NPC #" & slot)
            NpcData.WriteRecord(spc, slot, rec)
            If copy Then
                NpcData.LoadNpcFiles()
                RefreshNpcCombos()
            End If
            AfterNpcListChange(spc, slot, "Replaced NPC #" & slot & " with " & Path.GetFileName(setupFile) & If(copy, " (created)", "") &
                               " (not saved yet).")
        Catch ex As Exception
            ErrorLog.Report("ReplaceNpc", ex)
        End Try
    End Sub

    ''' <summary>
    ''' Adiciona um NPC no slot vazio selecionado (ou no primeiro livre). A janela Add NPC escolhe a origem e se ela e usada como esta (so o registro) ou
    ''' copiada para um .NPC/.zhoon novo. Com hasPosition (clique no mapa) o NPC ja nasce em (x, z), com o Y do terreno; sem ele
    ''' fica na posicao do registro-modelo. O registro entra so na copia em memoria (Desfazer/Save); os arquivos novos sao
    ''' gravados na hora e o Desfazer nao os apaga.
    ''' </summary>
    Private Sub CreateNpc(ByVal hasPosition As Boolean, ByVal x As Integer, ByVal z As Integer)
        Dim spc = SelectedSpc()
        If spc Is Nothing Then
            MsgBox("Please select a map")
            Return
        End If
        ' O slot vazio selecionado na lista e o escolhido; sem ele (ou se estiver ocupado), o primeiro livre.
        Dim slot As Integer = lbNPCList.SelectedIndex
        If slot < 0 OrElse NpcData.IsRecordUsed(spc, slot) Then slot = lbNPCList.Items.IndexOf(NpcData.Empty)
        If slot < 0 Then
            MsgBox("No empty NPC slot in this map")
            Return
        End If

        Dim copy As Boolean, sourcePath As String, name As String
        Using dialog As New AddNpcDialog(AppState.Npcs, "slot #" & slot & If(hasPosition, ", X " & x & ", Z " & z, ""))
            If dialog.ShowDialog(Me) <> DialogResult.OK Then
                SetNpcStatus("Add NPC canceled.", False)
                Return
            End If
            copy = dialog.CreateCopy
            sourcePath = dialog.SourcePath
            name = dialog.NewName
        End Using

        Try
            Dim setupFile As String = If(copy, CreateNpcFiles(sourcePath, name), "GameServer\npc\" & Path.GetFileName(sourcePath))
            _npcHistory.Record(spc.Data, "Added NPC #" & slot)
            NpcData.WriteTemplateRecord(spc, slot, setupFile)
            If hasPosition Then
                Dim y As Integer = NpcData.ReadRecord(spc, slot).Y
                Dim height As Single
                If _npcMesh IsNot Nothing AndAlso _npcMesh.TryHeightAt(x, z, height) Then y = CInt(Math.Round(height))
                NpcData.WritePosition(spc, slot, x, y, z)
            End If
            If copy Then
                NpcData.LoadNpcFiles()
                RefreshNpcCombos()
            End If
            AfterNpcListChange(spc, slot, "Added NPC #" & slot & " (" & Path.GetFileName(setupFile) & If(copy, " created", "") &
                               "; the record is not saved yet).")
        Catch ex As Exception
            ErrorLog.Report("AddNewNpc", ex)
        End Try
    End Sub

#End Region

#Region "Aba Options: verificacoes e backups"

    Private Sub cmdCheckExp_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCheckExp.Click
        If Not IsNumeric(tbMaxExp.Text) Then
            MsgBox("Error: " & tbMaxExp.Text & " is not numeric")
            ErrorLog.Write("Error in expcheck:" & vbCrLf & "Error: " & tbMaxExp.Text & " is not numeric")
            Return
        End If
        Dim maxExp As Long = CLng(tbMaxExp.Text)
        Dim found = AppState.Monsters.Where(Function(m) MonsterData.HasValueAbove(m, GameFields.Monster.Exp, maxExp)).ToList()
        ReportCheck(found, "Found " & found.Count & " Monsters! All will show now")
    End Sub

    Private Sub cmdCheckGold_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCheckGold.Click
        If Not IsNumeric(tbMaxGold.Text) Then
            MsgBox("Error: " & tbMaxGold.Text & " is not numeric")
            ErrorLog.Write("Error in goldcheck:" & vbCrLf & "Error: " & tbMaxGold.Text & " is not numeric")
            Return
        End If
        Dim maxGold As Long = CLng(tbMaxGold.Text)
        Dim found = AppState.Monsters.Where(Function(m) MonsterData.HasGoldAbove(m, maxGold)).ToList()
        ReportCheck(found, "Found " & found.Count & " Monsters! All Monsters show now!")
    End Sub

    Private Sub CmdCheckDrop_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CmdCheckDrop.Click
        Try
            Dim codes = tbWarnItem.Text.Split(","c)
            Dim found = AppState.Monsters.Where(Function(m) codes.Any(Function(c) MonsterData.MatchesText(m, GameFields.Monster.Drop, c))).ToList()
            ReportCheck(found, "Found " & found.Count & " Monsters! All are selected now")
        Catch ex As Exception
            ErrorLog.Report("CheckDrop", ex)
        End Try
    End Sub

    Private Sub ReportCheck(ByVal found As List(Of MonsterFile), ByVal message As String)
        If found.Count = 0 Then
            MsgBox("Nothing found!")
            Return
        End If
        MsgBox(message)
        ShowMonsterList(found)
    End Sub

    Private Sub cmdDelBackup_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDelBackup.Click
        If Dialogs.Confirm("Deleting!", "Delete all monster-backupfiles?", MsgBoxStyle.Exclamation) Then
            BackupHelper.DeleteBackups(AppState.ServerPath & AppState.MonsterFolder, AppState.MonsterPattern)
        End If
        If Dialogs.Confirm("Deleting!", "Delete all item-backupfiles?", MsgBoxStyle.Exclamation) Then
            BackupHelper.DeleteBackups(AppState.ServerPath & AppState.ItemFolder, AppState.ItemPattern)
        End If
    End Sub

#End Region

End Class
