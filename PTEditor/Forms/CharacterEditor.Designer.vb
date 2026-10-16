<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class CharacterEditor
    Inherits System.Windows.Forms.UserControl

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.pnlTop = New System.Windows.Forms.Panel()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.cmdLoad = New System.Windows.Forms.Button()
        Me.cmdBrowse = New System.Windows.Forms.Button()
        Me.tbDataServer = New System.Windows.Forms.TextBox()
        Me.lblDataServer = New System.Windows.Forms.Label()
        Me.split = New System.Windows.Forms.SplitContainer()
        Me.tvCharacters = New System.Windows.Forms.TreeView()
        Me.lblCount = New System.Windows.Forms.Label()
        Me.pnlFilter = New System.Windows.Forms.Panel()
        Me.lblLevelTo = New System.Windows.Forms.Label()
        Me.nudLevelMax = New System.Windows.Forms.NumericUpDown()
        Me.nudLevelMin = New System.Windows.Forms.NumericUpDown()
        Me.lblLevelFilter = New System.Windows.Forms.Label()
        Me.cbClassFilter = New System.Windows.Forms.ComboBox()
        Me.lblClassFilter = New System.Windows.Forms.Label()
        Me.tbSearch = New System.Windows.Forms.TextBox()
        Me.lblSearch = New System.Windows.Forms.Label()
        Me.tabs = New System.Windows.Forms.TabControl()
        Me.tpGeneral = New System.Windows.Forms.TabPage()
        Me.lvGeneral = New System.Windows.Forms.ListView()
        Me.colField = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colValue = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.tpLevel = New System.Windows.Forms.TabPage()
        Me.lblLevelNote = New System.Windows.Forms.Label()
        Me.lblServerLevel = New System.Windows.Forms.Label()
        Me.lblExpRange = New System.Windows.Forms.Label()
        Me.tbExp = New System.Windows.Forms.TextBox()
        Me.lblExp = New System.Windows.Forms.Label()
        Me.cmdExpStart = New System.Windows.Forms.Button()
        Me.nudLevel = New System.Windows.Forms.NumericUpDown()
        Me.lblLevel = New System.Windows.Forms.Label()
        Me.tpStats = New System.Windows.Forms.TabPage()
        Me.lblStatsNote = New System.Windows.Forms.Label()
        Me.lblStatFree = New System.Windows.Forms.Label()
        Me.lblStatTotal = New System.Windows.Forms.Label()
        Me.nudHealth = New System.Windows.Forms.NumericUpDown()
        Me.nudDexterity = New System.Windows.Forms.NumericUpDown()
        Me.nudTalent = New System.Windows.Forms.NumericUpDown()
        Me.nudSpirit = New System.Windows.Forms.NumericUpDown()
        Me.nudStrength = New System.Windows.Forms.NumericUpDown()
        Me.lblHealth = New System.Windows.Forms.Label()
        Me.lblDexterity = New System.Windows.Forms.Label()
        Me.lblTalent = New System.Windows.Forms.Label()
        Me.lblSpirit = New System.Windows.Forms.Label()
        Me.lblStrength = New System.Windows.Forms.Label()
        Me.tpGold = New System.Windows.Forms.TabPage()
        Me.lblGoldNote = New System.Windows.Forms.Label()
        Me.nudMoney = New System.Windows.Forms.NumericUpDown()
        Me.lblMoney = New System.Windows.Forms.Label()
        Me.tpInventory = New System.Windows.Forms.TabPage()
        Me.lvInventory = New System.Windows.Forms.ListView()
        Me.pnlInventory = New System.Windows.Forms.Panel()
        Me.lblInventory = New System.Windows.Forms.Label()
        Me.cmdRemoveItem = New System.Windows.Forms.Button()
        Me.tpWarehouse = New System.Windows.Forms.TabPage()
        Me.lvWarehouse = New System.Windows.Forms.ListView()
        Me.lblWarehouse = New System.Windows.Forms.Label()
        Me.tpSkills = New System.Windows.Forms.TabPage()
        Me.lvSkills = New System.Windows.Forms.ListView()
        Me.colSkill = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colSkillPoint = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colSkillMastery = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.colSkillKey = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.pnlBottom = New System.Windows.Forms.Panel()
        Me.lvMessages = New System.Windows.Forms.ListView()
        Me.colMessage = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.pnlButtons = New System.Windows.Forms.Panel()
        Me.cmdOpenFolder = New System.Windows.Forms.Button()
        Me.cmdDiscard = New System.Windows.Forms.Button()
        Me.cmdSave = New System.Windows.Forms.Button()
        Me.lblSubtitle = New System.Windows.Forms.Label()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.dlgFolder = New System.Windows.Forms.FolderBrowserDialog()
        Me.pnlTop.SuspendLayout()
        CType(Me.split, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.split.Panel1.SuspendLayout()
        Me.split.Panel2.SuspendLayout()
        Me.split.SuspendLayout()
        Me.pnlFilter.SuspendLayout()
        CType(Me.nudLevelMax, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudLevelMin, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabs.SuspendLayout()
        Me.tpGeneral.SuspendLayout()
        Me.tpLevel.SuspendLayout()
        CType(Me.nudLevel, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpStats.SuspendLayout()
        CType(Me.nudHealth, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudDexterity, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudTalent, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudSpirit, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudStrength, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpGold.SuspendLayout()
        CType(Me.nudMoney, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tpInventory.SuspendLayout()
        Me.pnlInventory.SuspendLayout()
        Me.tpWarehouse.SuspendLayout()
        Me.tpSkills.SuspendLayout()
        Me.pnlBottom.SuspendLayout()
        Me.pnlButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlTop
        '
        Me.pnlTop.Controls.Add(Me.lblStatus)
        Me.pnlTop.Controls.Add(Me.cmdLoad)
        Me.pnlTop.Controls.Add(Me.cmdBrowse)
        Me.pnlTop.Controls.Add(Me.tbDataServer)
        Me.pnlTop.Controls.Add(Me.lblDataServer)
        Me.pnlTop.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlTop.Location = New System.Drawing.Point(0, 0)
        Me.pnlTop.Name = "pnlTop"
        Me.pnlTop.Size = New System.Drawing.Size(1034, 34)
        Me.pnlTop.TabIndex = 0
        '
        'lblDataServer
        '
        Me.lblDataServer.AutoSize = True
        Me.lblDataServer.Location = New System.Drawing.Point(6, 10)
        Me.lblDataServer.Name = "lblDataServer"
        Me.lblDataServer.Size = New System.Drawing.Size(63, 13)
        Me.lblDataServer.TabIndex = 0
        Me.lblDataServer.Text = "DataServer:"
        '
        'tbDataServer
        '
        Me.tbDataServer.Location = New System.Drawing.Point(75, 7)
        Me.tbDataServer.Name = "tbDataServer"
        Me.tbDataServer.Size = New System.Drawing.Size(420, 20)
        Me.tbDataServer.TabIndex = 1
        '
        'cmdBrowse
        '
        Me.cmdBrowse.Location = New System.Drawing.Point(499, 6)
        Me.cmdBrowse.Name = "cmdBrowse"
        Me.cmdBrowse.Size = New System.Drawing.Size(28, 22)
        Me.cmdBrowse.TabIndex = 2
        Me.cmdBrowse.Text = "..."
        Me.cmdBrowse.UseVisualStyleBackColor = True
        '
        'cmdLoad
        '
        Me.cmdLoad.Location = New System.Drawing.Point(531, 6)
        Me.cmdLoad.Name = "cmdLoad"
        Me.cmdLoad.Size = New System.Drawing.Size(90, 22)
        Me.cmdLoad.TabIndex = 3
        Me.cmdLoad.Text = "Load"
        Me.cmdLoad.UseVisualStyleBackColor = True
        '
        'lblStatus
        '
        Me.lblStatus.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblStatus.AutoEllipsis = True
        Me.lblStatus.Location = New System.Drawing.Point(630, 10)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(398, 16)
        Me.lblStatus.TabIndex = 4
        '
        'split
        '
        Me.split.Dock = System.Windows.Forms.DockStyle.Fill
        Me.split.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.split.Location = New System.Drawing.Point(0, 34)
        Me.split.Name = "split"
        '
        'split.Panel1
        '
        Me.split.Panel1.Controls.Add(Me.tvCharacters)
        Me.split.Panel1.Controls.Add(Me.lblCount)
        Me.split.Panel1.Controls.Add(Me.pnlFilter)
        '
        'split.Panel2
        '
        Me.split.Panel2.Controls.Add(Me.tabs)
        Me.split.Panel2.Controls.Add(Me.pnlBottom)
        Me.split.Panel2.Controls.Add(Me.lblSubtitle)
        Me.split.Panel2.Controls.Add(Me.lblTitle)
        Me.split.Size = New System.Drawing.Size(1034, 620)
        Me.split.SplitterDistance = 270
        Me.split.TabIndex = 1
        '
        'pnlFilter
        '
        Me.pnlFilter.Controls.Add(Me.lblLevelTo)
        Me.pnlFilter.Controls.Add(Me.nudLevelMax)
        Me.pnlFilter.Controls.Add(Me.nudLevelMin)
        Me.pnlFilter.Controls.Add(Me.lblLevelFilter)
        Me.pnlFilter.Controls.Add(Me.cbClassFilter)
        Me.pnlFilter.Controls.Add(Me.lblClassFilter)
        Me.pnlFilter.Controls.Add(Me.tbSearch)
        Me.pnlFilter.Controls.Add(Me.lblSearch)
        Me.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlFilter.Location = New System.Drawing.Point(0, 0)
        Me.pnlFilter.Name = "pnlFilter"
        Me.pnlFilter.Size = New System.Drawing.Size(270, 84)
        Me.pnlFilter.TabIndex = 0
        '
        'lblSearch
        '
        Me.lblSearch.AutoSize = True
        Me.lblSearch.Location = New System.Drawing.Point(4, 8)
        Me.lblSearch.Name = "lblSearch"
        Me.lblSearch.Size = New System.Drawing.Size(43, 13)
        Me.lblSearch.TabIndex = 0
        Me.lblSearch.Text = "Search:"
        '
        'tbSearch
        '
        Me.tbSearch.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbSearch.Location = New System.Drawing.Point(56, 5)
        Me.tbSearch.Name = "tbSearch"
        Me.tbSearch.Size = New System.Drawing.Size(208, 20)
        Me.tbSearch.TabIndex = 1
        '
        'lblClassFilter
        '
        Me.lblClassFilter.AutoSize = True
        Me.lblClassFilter.Location = New System.Drawing.Point(4, 34)
        Me.lblClassFilter.Name = "lblClassFilter"
        Me.lblClassFilter.Size = New System.Drawing.Size(41, 13)
        Me.lblClassFilter.TabIndex = 2
        Me.lblClassFilter.Text = "Class:"
        '
        'cbClassFilter
        '
        Me.cbClassFilter.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cbClassFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbClassFilter.Location = New System.Drawing.Point(56, 31)
        Me.cbClassFilter.Name = "cbClassFilter"
        Me.cbClassFilter.Size = New System.Drawing.Size(208, 21)
        Me.cbClassFilter.TabIndex = 3
        '
        'lblLevelFilter
        '
        Me.lblLevelFilter.AutoSize = True
        Me.lblLevelFilter.Location = New System.Drawing.Point(4, 61)
        Me.lblLevelFilter.Name = "lblLevelFilter"
        Me.lblLevelFilter.Size = New System.Drawing.Size(36, 13)
        Me.lblLevelFilter.TabIndex = 4
        Me.lblLevelFilter.Text = "Level:"
        '
        'nudLevelMin
        '
        Me.nudLevelMin.Location = New System.Drawing.Point(56, 58)
        Me.nudLevelMin.Maximum = New Decimal(New Integer() {999, 0, 0, 0})
        Me.nudLevelMin.Name = "nudLevelMin"
        Me.nudLevelMin.Size = New System.Drawing.Size(60, 20)
        Me.nudLevelMin.TabIndex = 5
        '
        'lblLevelTo
        '
        Me.lblLevelTo.AutoSize = True
        Me.lblLevelTo.Location = New System.Drawing.Point(122, 61)
        Me.lblLevelTo.Name = "lblLevelTo"
        Me.lblLevelTo.Size = New System.Drawing.Size(13, 13)
        Me.lblLevelTo.TabIndex = 6
        Me.lblLevelTo.Text = "to"
        '
        'nudLevelMax
        '
        Me.nudLevelMax.Location = New System.Drawing.Point(141, 58)
        Me.nudLevelMax.Maximum = New Decimal(New Integer() {999, 0, 0, 0})
        Me.nudLevelMax.Name = "nudLevelMax"
        Me.nudLevelMax.Size = New System.Drawing.Size(60, 20)
        Me.nudLevelMax.TabIndex = 7
        Me.nudLevelMax.Value = New Decimal(New Integer() {999, 0, 0, 0})
        '
        'tvCharacters
        '
        Me.tvCharacters.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tvCharacters.HideSelection = False
        Me.tvCharacters.Location = New System.Drawing.Point(0, 84)
        Me.tvCharacters.Name = "tvCharacters"
        Me.tvCharacters.Size = New System.Drawing.Size(270, 516)
        Me.tvCharacters.TabIndex = 1
        '
        'lblCount
        '
        Me.lblCount.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.lblCount.Location = New System.Drawing.Point(0, 600)
        Me.lblCount.Name = "lblCount"
        Me.lblCount.Size = New System.Drawing.Size(270, 20)
        Me.lblCount.TabIndex = 2
        Me.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblTitle
        '
        Me.lblTitle.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(0, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Padding = New System.Windows.Forms.Padding(4, 0, 0, 0)
        Me.lblTitle.Size = New System.Drawing.Size(760, 26)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "Select a character"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblSubtitle
        '
        Me.lblSubtitle.AutoEllipsis = True
        Me.lblSubtitle.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblSubtitle.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblSubtitle.Location = New System.Drawing.Point(0, 26)
        Me.lblSubtitle.Name = "lblSubtitle"
        Me.lblSubtitle.Padding = New System.Windows.Forms.Padding(4, 0, 0, 0)
        Me.lblSubtitle.Size = New System.Drawing.Size(760, 20)
        Me.lblSubtitle.TabIndex = 1
        '
        'tabs
        '
        Me.tabs.Controls.Add(Me.tpGeneral)
        Me.tabs.Controls.Add(Me.tpLevel)
        Me.tabs.Controls.Add(Me.tpStats)
        Me.tabs.Controls.Add(Me.tpGold)
        Me.tabs.Controls.Add(Me.tpInventory)
        Me.tabs.Controls.Add(Me.tpWarehouse)
        Me.tabs.Controls.Add(Me.tpSkills)
        Me.tabs.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabs.Location = New System.Drawing.Point(0, 46)
        Me.tabs.Name = "tabs"
        Me.tabs.SelectedIndex = 0
        Me.tabs.Size = New System.Drawing.Size(760, 454)
        Me.tabs.TabIndex = 2
        '
        'tpGeneral
        '
        Me.tpGeneral.Controls.Add(Me.lvGeneral)
        Me.tpGeneral.Location = New System.Drawing.Point(4, 22)
        Me.tpGeneral.Name = "tpGeneral"
        Me.tpGeneral.Padding = New System.Windows.Forms.Padding(3)
        Me.tpGeneral.Size = New System.Drawing.Size(752, 428)
        Me.tpGeneral.TabIndex = 0
        Me.tpGeneral.Text = "General"
        Me.tpGeneral.UseVisualStyleBackColor = True
        '
        'lvGeneral
        '
        Me.lvGeneral.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colField, Me.colValue})
        Me.lvGeneral.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvGeneral.FullRowSelect = True
        Me.lvGeneral.GridLines = True
        Me.lvGeneral.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lvGeneral.Location = New System.Drawing.Point(3, 3)
        Me.lvGeneral.Name = "lvGeneral"
        Me.lvGeneral.Size = New System.Drawing.Size(746, 422)
        Me.lvGeneral.TabIndex = 0
        Me.lvGeneral.UseCompatibleStateImageBehavior = False
        Me.lvGeneral.View = System.Windows.Forms.View.Details
        '
        'colField
        '
        Me.colField.Text = "Field"
        Me.colField.Width = 180
        '
        'colValue
        '
        Me.colValue.Text = "Value"
        Me.colValue.Width = 520
        '
        'tpLevel
        '
        Me.tpLevel.Controls.Add(Me.lblLevelNote)
        Me.tpLevel.Controls.Add(Me.lblServerLevel)
        Me.tpLevel.Controls.Add(Me.lblExpRange)
        Me.tpLevel.Controls.Add(Me.tbExp)
        Me.tpLevel.Controls.Add(Me.lblExp)
        Me.tpLevel.Controls.Add(Me.cmdExpStart)
        Me.tpLevel.Controls.Add(Me.nudLevel)
        Me.tpLevel.Controls.Add(Me.lblLevel)
        Me.tpLevel.Location = New System.Drawing.Point(4, 22)
        Me.tpLevel.Name = "tpLevel"
        Me.tpLevel.Padding = New System.Windows.Forms.Padding(3)
        Me.tpLevel.Size = New System.Drawing.Size(752, 428)
        Me.tpLevel.TabIndex = 1
        Me.tpLevel.Text = "Level & EXP"
        Me.tpLevel.UseVisualStyleBackColor = True
        '
        'lblLevel
        '
        Me.lblLevel.AutoSize = True
        Me.lblLevel.Location = New System.Drawing.Point(16, 22)
        Me.lblLevel.Name = "lblLevel"
        Me.lblLevel.Size = New System.Drawing.Size(36, 13)
        Me.lblLevel.TabIndex = 0
        Me.lblLevel.Text = "Level:"
        '
        'nudLevel
        '
        Me.nudLevel.Location = New System.Drawing.Point(120, 19)
        Me.nudLevel.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudLevel.Name = "nudLevel"
        Me.nudLevel.Size = New System.Drawing.Size(80, 20)
        Me.nudLevel.TabIndex = 1
        Me.nudLevel.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'cmdExpStart
        '
        Me.cmdExpStart.Location = New System.Drawing.Point(330, 49)
        Me.cmdExpStart.Name = "cmdExpStart"
        Me.cmdExpStart.Size = New System.Drawing.Size(160, 22)
        Me.cmdExpStart.TabIndex = 4
        Me.cmdExpStart.Text = "EXP at level start"
        Me.cmdExpStart.UseVisualStyleBackColor = True
        '
        'lblExp
        '
        Me.lblExp.AutoSize = True
        Me.lblExp.Location = New System.Drawing.Point(16, 53)
        Me.lblExp.Name = "lblExp"
        Me.lblExp.Size = New System.Drawing.Size(77, 13)
        Me.lblExp.TabIndex = 2
        Me.lblExp.Text = "EXP (64 bits):"
        '
        'tbExp
        '
        Me.tbExp.Location = New System.Drawing.Point(120, 50)
        Me.tbExp.Name = "tbExp"
        Me.tbExp.Size = New System.Drawing.Size(200, 20)
        Me.tbExp.TabIndex = 3
        '
        'lblExpRange
        '
        Me.lblExpRange.AutoSize = True
        Me.lblExpRange.Location = New System.Drawing.Point(16, 86)
        Me.lblExpRange.Name = "lblExpRange"
        Me.lblExpRange.Size = New System.Drawing.Size(0, 13)
        Me.lblExpRange.TabIndex = 5
        '
        'lblServerLevel
        '
        Me.lblServerLevel.AutoSize = True
        Me.lblServerLevel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblServerLevel.Location = New System.Drawing.Point(16, 108)
        Me.lblServerLevel.Name = "lblServerLevel"
        Me.lblServerLevel.Size = New System.Drawing.Size(0, 13)
        Me.lblServerLevel.TabIndex = 6
        '
        'lblLevelNote
        '
        Me.lblLevelNote.Location = New System.Drawing.Point(16, 140)
        Me.lblLevelNote.Name = "lblLevelNote"
        Me.lblLevelNote.Size = New System.Drawing.Size(700, 60)
        Me.lblLevelNote.TabIndex = 7
        Me.lblLevelNote.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblLevelNote.Text = "Changing the level moves the EXP to the start of the new level. The server checks Level x EXP when loading " &
            "the character (server LevelTable.h) and fixes the level if they do not match. Free stat points are " &
            "recalculated on save."
        '
        'tpStats
        '
        Me.tpStats.Controls.Add(Me.lblStatsNote)
        Me.tpStats.Controls.Add(Me.lblStatFree)
        Me.tpStats.Controls.Add(Me.lblStatTotal)
        Me.tpStats.Controls.Add(Me.nudHealth)
        Me.tpStats.Controls.Add(Me.nudDexterity)
        Me.tpStats.Controls.Add(Me.nudTalent)
        Me.tpStats.Controls.Add(Me.nudSpirit)
        Me.tpStats.Controls.Add(Me.nudStrength)
        Me.tpStats.Controls.Add(Me.lblHealth)
        Me.tpStats.Controls.Add(Me.lblDexterity)
        Me.tpStats.Controls.Add(Me.lblTalent)
        Me.tpStats.Controls.Add(Me.lblSpirit)
        Me.tpStats.Controls.Add(Me.lblStrength)
        Me.tpStats.Location = New System.Drawing.Point(4, 22)
        Me.tpStats.Name = "tpStats"
        Me.tpStats.Padding = New System.Windows.Forms.Padding(3)
        Me.tpStats.Size = New System.Drawing.Size(752, 428)
        Me.tpStats.TabIndex = 2
        Me.tpStats.Text = "Stats"
        Me.tpStats.UseVisualStyleBackColor = True
        '
        'lblStrength
        '
        Me.lblStrength.AutoSize = True
        Me.lblStrength.Location = New System.Drawing.Point(16, 22)
        Me.lblStrength.Name = "lblStrength"
        Me.lblStrength.Size = New System.Drawing.Size(50, 13)
        Me.lblStrength.TabIndex = 0
        Me.lblStrength.Text = "Strength:"
        '
        'lblSpirit
        '
        Me.lblSpirit.AutoSize = True
        Me.lblSpirit.Location = New System.Drawing.Point(16, 50)
        Me.lblSpirit.Name = "lblSpirit"
        Me.lblSpirit.Size = New System.Drawing.Size(33, 13)
        Me.lblSpirit.TabIndex = 2
        Me.lblSpirit.Text = "Spirit:"
        '
        'lblTalent
        '
        Me.lblTalent.AutoSize = True
        Me.lblTalent.Location = New System.Drawing.Point(16, 78)
        Me.lblTalent.Name = "lblTalent"
        Me.lblTalent.Size = New System.Drawing.Size(40, 13)
        Me.lblTalent.TabIndex = 4
        Me.lblTalent.Text = "Talent:"
        '
        'lblDexterity
        '
        Me.lblDexterity.AutoSize = True
        Me.lblDexterity.Location = New System.Drawing.Point(16, 106)
        Me.lblDexterity.Name = "lblDexterity"
        Me.lblDexterity.Size = New System.Drawing.Size(37, 13)
        Me.lblDexterity.TabIndex = 6
        Me.lblDexterity.Text = "Agility:"
        '
        'lblHealth
        '
        Me.lblHealth.AutoSize = True
        Me.lblHealth.Location = New System.Drawing.Point(16, 134)
        Me.lblHealth.Name = "lblHealth"
        Me.lblHealth.Size = New System.Drawing.Size(41, 13)
        Me.lblHealth.TabIndex = 8
        Me.lblHealth.Text = "Health:"
        '
        'nudStrength
        '
        Me.nudStrength.Location = New System.Drawing.Point(120, 19)
        Me.nudStrength.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
        Me.nudStrength.Name = "nudStrength"
        Me.nudStrength.Size = New System.Drawing.Size(80, 20)
        Me.nudStrength.TabIndex = 1
        '
        'nudSpirit
        '
        Me.nudSpirit.Location = New System.Drawing.Point(120, 47)
        Me.nudSpirit.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
        Me.nudSpirit.Name = "nudSpirit"
        Me.nudSpirit.Size = New System.Drawing.Size(80, 20)
        Me.nudSpirit.TabIndex = 3
        '
        'nudTalent
        '
        Me.nudTalent.Location = New System.Drawing.Point(120, 75)
        Me.nudTalent.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
        Me.nudTalent.Name = "nudTalent"
        Me.nudTalent.Size = New System.Drawing.Size(80, 20)
        Me.nudTalent.TabIndex = 5
        '
        'nudDexterity
        '
        Me.nudDexterity.Location = New System.Drawing.Point(120, 103)
        Me.nudDexterity.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
        Me.nudDexterity.Name = "nudDexterity"
        Me.nudDexterity.Size = New System.Drawing.Size(80, 20)
        Me.nudDexterity.TabIndex = 7
        '
        'nudHealth
        '
        Me.nudHealth.Location = New System.Drawing.Point(120, 131)
        Me.nudHealth.Maximum = New Decimal(New Integer() {100000, 0, 0, 0})
        Me.nudHealth.Name = "nudHealth"
        Me.nudHealth.Size = New System.Drawing.Size(80, 20)
        Me.nudHealth.TabIndex = 9
        '
        'lblStatTotal
        '
        Me.lblStatTotal.AutoSize = True
        Me.lblStatTotal.Location = New System.Drawing.Point(16, 170)
        Me.lblStatTotal.Name = "lblStatTotal"
        Me.lblStatTotal.Size = New System.Drawing.Size(0, 13)
        Me.lblStatTotal.TabIndex = 10
        '
        'lblStatFree
        '
        Me.lblStatFree.AutoSize = True
        Me.lblStatFree.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblStatFree.Location = New System.Drawing.Point(16, 192)
        Me.lblStatFree.Name = "lblStatFree"
        Me.lblStatFree.Size = New System.Drawing.Size(0, 13)
        Me.lblStatFree.TabIndex = 11
        '
        'lblStatsNote
        '
        Me.lblStatsNote.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblStatsNote.Location = New System.Drawing.Point(16, 222)
        Me.lblStatsNote.Name = "lblStatsNote"
        Me.lblStatsNote.Size = New System.Drawing.Size(700, 60)
        Me.lblStatsNote.TabIndex = 12
        Me.lblStatsNote.Text = "When loading the character the server recalculates the free points (ReformCharStatePoint): " &
            "99 + (level - 1) x 5 + quest bonus, minus the sum of the stats. If the result is -10 or lower, it " &
            "RESETS the stats."
        '
        'tpGold
        '
        Me.tpGold.Controls.Add(Me.lblGoldNote)
        Me.tpGold.Controls.Add(Me.nudMoney)
        Me.tpGold.Controls.Add(Me.lblMoney)
        Me.tpGold.Location = New System.Drawing.Point(4, 22)
        Me.tpGold.Name = "tpGold"
        Me.tpGold.Padding = New System.Windows.Forms.Padding(3)
        Me.tpGold.Size = New System.Drawing.Size(752, 428)
        Me.tpGold.TabIndex = 3
        Me.tpGold.Text = "Gold"
        Me.tpGold.UseVisualStyleBackColor = True
        '
        'lblMoney
        '
        Me.lblMoney.AutoSize = True
        Me.lblMoney.Location = New System.Drawing.Point(16, 22)
        Me.lblMoney.Name = "lblMoney"
        Me.lblMoney.Size = New System.Drawing.Size(33, 13)
        Me.lblMoney.TabIndex = 0
        Me.lblMoney.Text = "Gold:"
        '
        'nudMoney
        '
        Me.nudMoney.Location = New System.Drawing.Point(120, 19)
        Me.nudMoney.Maximum = New Decimal(New Integer() {2147483647, 0, 0, 0})
        Me.nudMoney.Name = "nudMoney"
        Me.nudMoney.Size = New System.Drawing.Size(140, 20)
        Me.nudMoney.TabIndex = 1
        Me.nudMoney.ThousandsSeparator = True
        '
        'lblGoldNote
        '
        Me.lblGoldNote.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblGoldNote.Location = New System.Drawing.Point(16, 56)
        Me.lblGoldNote.Name = "lblGoldNote"
        Me.lblGoldNote.Size = New System.Drawing.Size(700, 60)
        Me.lblGoldNote.TabIndex = 2
        '
        'tpInventory
        '
        Me.tpInventory.Controls.Add(Me.lvInventory)
        Me.tpInventory.Controls.Add(Me.pnlInventory)
        Me.tpInventory.Location = New System.Drawing.Point(4, 22)
        Me.tpInventory.Name = "tpInventory"
        Me.tpInventory.Padding = New System.Windows.Forms.Padding(3)
        Me.tpInventory.Size = New System.Drawing.Size(752, 428)
        Me.tpInventory.TabIndex = 4
        Me.tpInventory.Text = "Inventory"
        Me.tpInventory.UseVisualStyleBackColor = True
        '
        'pnlInventory
        '
        Me.pnlInventory.Controls.Add(Me.lblInventory)
        Me.pnlInventory.Controls.Add(Me.cmdRemoveItem)
        Me.pnlInventory.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlInventory.Location = New System.Drawing.Point(3, 3)
        Me.pnlInventory.Name = "pnlInventory"
        Me.pnlInventory.Size = New System.Drawing.Size(746, 30)
        Me.pnlInventory.TabIndex = 0
        '
        'cmdRemoveItem
        '
        Me.cmdRemoveItem.Location = New System.Drawing.Point(0, 3)
        Me.cmdRemoveItem.Name = "cmdRemoveItem"
        Me.cmdRemoveItem.Size = New System.Drawing.Size(130, 23)
        Me.cmdRemoveItem.TabIndex = 0
        Me.cmdRemoveItem.Text = "Remove item"
        Me.cmdRemoveItem.UseVisualStyleBackColor = True
        '
        'lblInventory
        '
        Me.lblInventory.AutoSize = True
        Me.lblInventory.Location = New System.Drawing.Point(140, 8)
        Me.lblInventory.Name = "lblInventory"
        Me.lblInventory.Size = New System.Drawing.Size(0, 13)
        Me.lblInventory.TabIndex = 1
        '
        'lvInventory
        '
        Me.lvInventory.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvInventory.FullRowSelect = True
        Me.lvInventory.GridLines = True
        Me.lvInventory.HideSelection = False
        Me.lvInventory.Location = New System.Drawing.Point(3, 33)
        Me.lvInventory.Name = "lvInventory"
        Me.lvInventory.Size = New System.Drawing.Size(746, 392)
        Me.lvInventory.TabIndex = 1
        Me.lvInventory.UseCompatibleStateImageBehavior = False
        Me.lvInventory.View = System.Windows.Forms.View.Details
        '
        'tpWarehouse
        '
        Me.tpWarehouse.Controls.Add(Me.lvWarehouse)
        Me.tpWarehouse.Controls.Add(Me.lblWarehouse)
        Me.tpWarehouse.Location = New System.Drawing.Point(4, 22)
        Me.tpWarehouse.Name = "tpWarehouse"
        Me.tpWarehouse.Padding = New System.Windows.Forms.Padding(3)
        Me.tpWarehouse.Size = New System.Drawing.Size(752, 428)
        Me.tpWarehouse.TabIndex = 5
        Me.tpWarehouse.Text = "Warehouse"
        Me.tpWarehouse.UseVisualStyleBackColor = True
        '
        'lblWarehouse
        '
        Me.lblWarehouse.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblWarehouse.Location = New System.Drawing.Point(3, 3)
        Me.lblWarehouse.Name = "lblWarehouse"
        Me.lblWarehouse.Size = New System.Drawing.Size(746, 44)
        Me.lblWarehouse.TabIndex = 0
        '
        'lvWarehouse
        '
        Me.lvWarehouse.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvWarehouse.FullRowSelect = True
        Me.lvWarehouse.GridLines = True
        Me.lvWarehouse.HideSelection = False
        Me.lvWarehouse.Location = New System.Drawing.Point(3, 47)
        Me.lvWarehouse.Name = "lvWarehouse"
        Me.lvWarehouse.Size = New System.Drawing.Size(746, 378)
        Me.lvWarehouse.TabIndex = 1
        Me.lvWarehouse.UseCompatibleStateImageBehavior = False
        Me.lvWarehouse.View = System.Windows.Forms.View.Details
        '
        'tpSkills
        '
        Me.tpSkills.Controls.Add(Me.lvSkills)
        Me.tpSkills.Location = New System.Drawing.Point(4, 22)
        Me.tpSkills.Name = "tpSkills"
        Me.tpSkills.Padding = New System.Windows.Forms.Padding(3)
        Me.tpSkills.Size = New System.Drawing.Size(752, 428)
        Me.tpSkills.TabIndex = 6
        Me.tpSkills.Text = "Skills"
        Me.tpSkills.UseVisualStyleBackColor = True
        '
        'lvSkills
        '
        Me.lvSkills.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colSkill, Me.colSkillPoint, Me.colSkillMastery, Me.colSkillKey})
        Me.lvSkills.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvSkills.FullRowSelect = True
        Me.lvSkills.GridLines = True
        Me.lvSkills.Location = New System.Drawing.Point(3, 3)
        Me.lvSkills.Name = "lvSkills"
        Me.lvSkills.Size = New System.Drawing.Size(746, 422)
        Me.lvSkills.TabIndex = 0
        Me.lvSkills.UseCompatibleStateImageBehavior = False
        Me.lvSkills.View = System.Windows.Forms.View.Details
        '
        'colSkill
        '
        Me.colSkill.Text = "Skill #"
        Me.colSkill.Width = 70
        '
        'colSkillPoint
        '
        Me.colSkillPoint.Text = "Points"
        Me.colSkillPoint.Width = 80
        '
        'colSkillMastery
        '
        Me.colSkillMastery.Text = "Mastery"
        Me.colSkillMastery.Width = 90
        '
        'colSkillKey
        '
        Me.colSkillKey.Text = "Shortcut"
        Me.colSkillKey.Width = 80
        '
        'pnlBottom
        '
        Me.pnlBottom.Controls.Add(Me.lvMessages)
        Me.pnlBottom.Controls.Add(Me.pnlButtons)
        Me.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlBottom.Location = New System.Drawing.Point(0, 500)
        Me.pnlBottom.Name = "pnlBottom"
        Me.pnlBottom.Size = New System.Drawing.Size(760, 120)
        Me.pnlBottom.TabIndex = 3
        '
        'lvMessages
        '
        Me.lvMessages.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colMessage})
        Me.lvMessages.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvMessages.FullRowSelect = True
        Me.lvMessages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.lvMessages.Location = New System.Drawing.Point(0, 0)
        Me.lvMessages.Name = "lvMessages"
        Me.lvMessages.Size = New System.Drawing.Size(610, 120)
        Me.lvMessages.TabIndex = 0
        Me.lvMessages.UseCompatibleStateImageBehavior = False
        Me.lvMessages.View = System.Windows.Forms.View.Details
        '
        'colMessage
        '
        Me.colMessage.Text = "Check (what the server will do when loading)"
        Me.colMessage.Width = 590
        '
        'pnlButtons
        '
        Me.pnlButtons.Controls.Add(Me.cmdOpenFolder)
        Me.pnlButtons.Controls.Add(Me.cmdDiscard)
        Me.pnlButtons.Controls.Add(Me.cmdSave)
        Me.pnlButtons.Dock = System.Windows.Forms.DockStyle.Right
        Me.pnlButtons.Location = New System.Drawing.Point(610, 0)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Size = New System.Drawing.Size(150, 120)
        Me.pnlButtons.TabIndex = 1
        '
        'cmdSave
        '
        Me.cmdSave.Enabled = False
        Me.cmdSave.Location = New System.Drawing.Point(8, 4)
        Me.cmdSave.Name = "cmdSave"
        Me.cmdSave.Size = New System.Drawing.Size(136, 30)
        Me.cmdSave.TabIndex = 0
        Me.cmdSave.Text = "Save changes"
        Me.cmdSave.UseVisualStyleBackColor = True
        '
        'cmdDiscard
        '
        Me.cmdDiscard.Enabled = False
        Me.cmdDiscard.Location = New System.Drawing.Point(8, 40)
        Me.cmdDiscard.Name = "cmdDiscard"
        Me.cmdDiscard.Size = New System.Drawing.Size(136, 26)
        Me.cmdDiscard.TabIndex = 1
        Me.cmdDiscard.Text = "Discard"
        Me.cmdDiscard.UseVisualStyleBackColor = True
        '
        'cmdOpenFolder
        '
        Me.cmdOpenFolder.Enabled = False
        Me.cmdOpenFolder.Location = New System.Drawing.Point(8, 72)
        Me.cmdOpenFolder.Name = "cmdOpenFolder"
        Me.cmdOpenFolder.Size = New System.Drawing.Size(136, 26)
        Me.cmdOpenFolder.TabIndex = 2
        Me.cmdOpenFolder.Text = "Open file folder"
        Me.cmdOpenFolder.UseVisualStyleBackColor = True
        '
        'CharacterEditor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.split)
        Me.Controls.Add(Me.pnlTop)
        Me.Name = "CharacterEditor"
        Me.Size = New System.Drawing.Size(1034, 654)
        Me.pnlTop.ResumeLayout(False)
        Me.pnlTop.PerformLayout()
        Me.split.Panel1.ResumeLayout(False)
        Me.split.Panel2.ResumeLayout(False)
        CType(Me.split, System.ComponentModel.ISupportInitialize).EndInit()
        Me.split.ResumeLayout(False)
        Me.pnlFilter.ResumeLayout(False)
        Me.pnlFilter.PerformLayout()
        CType(Me.nudLevelMax, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudLevelMin, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabs.ResumeLayout(False)
        Me.tpGeneral.ResumeLayout(False)
        Me.tpLevel.ResumeLayout(False)
        Me.tpLevel.PerformLayout()
        CType(Me.nudLevel, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpStats.ResumeLayout(False)
        Me.tpStats.PerformLayout()
        CType(Me.nudHealth, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudDexterity, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudTalent, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudSpirit, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudStrength, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpGold.ResumeLayout(False)
        Me.tpGold.PerformLayout()
        CType(Me.nudMoney, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tpInventory.ResumeLayout(False)
        Me.pnlInventory.ResumeLayout(False)
        Me.pnlInventory.PerformLayout()
        Me.tpWarehouse.ResumeLayout(False)
        Me.tpSkills.ResumeLayout(False)
        Me.pnlBottom.ResumeLayout(False)
        Me.pnlButtons.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlTop As System.Windows.Forms.Panel
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents cmdLoad As System.Windows.Forms.Button
    Friend WithEvents cmdBrowse As System.Windows.Forms.Button
    Friend WithEvents tbDataServer As System.Windows.Forms.TextBox
    Friend WithEvents lblDataServer As System.Windows.Forms.Label
    Friend WithEvents split As System.Windows.Forms.SplitContainer
    Friend WithEvents tvCharacters As System.Windows.Forms.TreeView
    Friend WithEvents lblCount As System.Windows.Forms.Label
    Friend WithEvents pnlFilter As System.Windows.Forms.Panel
    Friend WithEvents lblLevelTo As System.Windows.Forms.Label
    Friend WithEvents nudLevelMax As System.Windows.Forms.NumericUpDown
    Friend WithEvents nudLevelMin As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblLevelFilter As System.Windows.Forms.Label
    Friend WithEvents cbClassFilter As System.Windows.Forms.ComboBox
    Friend WithEvents lblClassFilter As System.Windows.Forms.Label
    Friend WithEvents tbSearch As System.Windows.Forms.TextBox
    Friend WithEvents lblSearch As System.Windows.Forms.Label
    Friend WithEvents tabs As System.Windows.Forms.TabControl
    Friend WithEvents tpGeneral As System.Windows.Forms.TabPage
    Friend WithEvents lvGeneral As System.Windows.Forms.ListView
    Friend WithEvents colField As System.Windows.Forms.ColumnHeader
    Friend WithEvents colValue As System.Windows.Forms.ColumnHeader
    Friend WithEvents tpLevel As System.Windows.Forms.TabPage
    Friend WithEvents lblLevelNote As System.Windows.Forms.Label
    Friend WithEvents lblServerLevel As System.Windows.Forms.Label
    Friend WithEvents lblExpRange As System.Windows.Forms.Label
    Friend WithEvents tbExp As System.Windows.Forms.TextBox
    Friend WithEvents lblExp As System.Windows.Forms.Label
    Friend WithEvents cmdExpStart As System.Windows.Forms.Button
    Friend WithEvents nudLevel As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblLevel As System.Windows.Forms.Label
    Friend WithEvents tpStats As System.Windows.Forms.TabPage
    Friend WithEvents lblStatsNote As System.Windows.Forms.Label
    Friend WithEvents lblStatFree As System.Windows.Forms.Label
    Friend WithEvents lblStatTotal As System.Windows.Forms.Label
    Friend WithEvents nudHealth As System.Windows.Forms.NumericUpDown
    Friend WithEvents nudDexterity As System.Windows.Forms.NumericUpDown
    Friend WithEvents nudTalent As System.Windows.Forms.NumericUpDown
    Friend WithEvents nudSpirit As System.Windows.Forms.NumericUpDown
    Friend WithEvents nudStrength As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblHealth As System.Windows.Forms.Label
    Friend WithEvents lblDexterity As System.Windows.Forms.Label
    Friend WithEvents lblTalent As System.Windows.Forms.Label
    Friend WithEvents lblSpirit As System.Windows.Forms.Label
    Friend WithEvents lblStrength As System.Windows.Forms.Label
    Friend WithEvents tpGold As System.Windows.Forms.TabPage
    Friend WithEvents lblGoldNote As System.Windows.Forms.Label
    Friend WithEvents nudMoney As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblMoney As System.Windows.Forms.Label
    Friend WithEvents tpInventory As System.Windows.Forms.TabPage
    Friend WithEvents lvInventory As System.Windows.Forms.ListView
    Friend WithEvents pnlInventory As System.Windows.Forms.Panel
    Friend WithEvents lblInventory As System.Windows.Forms.Label
    Friend WithEvents cmdRemoveItem As System.Windows.Forms.Button
    Friend WithEvents tpWarehouse As System.Windows.Forms.TabPage
    Friend WithEvents lvWarehouse As System.Windows.Forms.ListView
    Friend WithEvents lblWarehouse As System.Windows.Forms.Label
    Friend WithEvents tpSkills As System.Windows.Forms.TabPage
    Friend WithEvents lvSkills As System.Windows.Forms.ListView
    Friend WithEvents colSkill As System.Windows.Forms.ColumnHeader
    Friend WithEvents colSkillPoint As System.Windows.Forms.ColumnHeader
    Friend WithEvents colSkillMastery As System.Windows.Forms.ColumnHeader
    Friend WithEvents colSkillKey As System.Windows.Forms.ColumnHeader
    Friend WithEvents pnlBottom As System.Windows.Forms.Panel
    Friend WithEvents lvMessages As System.Windows.Forms.ListView
    Friend WithEvents colMessage As System.Windows.Forms.ColumnHeader
    Friend WithEvents pnlButtons As System.Windows.Forms.Panel
    Friend WithEvents cmdOpenFolder As System.Windows.Forms.Button
    Friend WithEvents cmdDiscard As System.Windows.Forms.Button
    Friend WithEvents cmdSave As System.Windows.Forms.Button
    Friend WithEvents lblSubtitle As System.Windows.Forms.Label
    Friend WithEvents lblTitle As System.Windows.Forms.Label
    Friend WithEvents dlgFolder As System.Windows.Forms.FolderBrowserDialog

End Class
