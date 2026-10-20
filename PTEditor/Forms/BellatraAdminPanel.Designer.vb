<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class BellatraAdminPanel
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
        Me.lblDbStatus = New System.Windows.Forms.Label()
        Me.cmdOptions = New System.Windows.Forms.Button()
        Me.lblDbInfo = New System.Windows.Forms.Label()
        Me.lblDatabase = New System.Windows.Forms.Label()
        Me.gbManager = New System.Windows.Forms.GroupBox()
        Me.lblDeliverHint = New System.Windows.Forms.Label()
        Me.cmdDeliver = New System.Windows.Forms.Button()
        Me.cmdRefresh = New System.Windows.Forms.Button()
        Me.lblManagerInfo = New System.Windows.Forms.Label()
        Me.gbClose = New System.Windows.Forms.GroupBox()
        Me.lblCloseHint = New System.Windows.Forms.Label()
        Me.cmdClose = New System.Windows.Forms.Button()
        Me.dtpNewEnd = New System.Windows.Forms.DateTimePicker()
        Me.lblNewEnd = New System.Windows.Forms.Label()
        Me.gbRanking = New System.Windows.Forms.GroupBox()
        Me.lvRanking = New System.Windows.Forms.ListView()
        Me.gbRules = New System.Windows.Forms.GroupBox()
        Me.lblRulesHint = New System.Windows.Forms.Label()
        Me.cmdSaveMin = New System.Windows.Forms.Button()
        Me.nudMin = New System.Windows.Forms.NumericUpDown()
        Me.lblMin = New System.Windows.Forms.Label()
        Me.cmdSaveRelease = New System.Windows.Forms.Button()
        Me.nudRelease = New System.Windows.Forms.NumericUpDown()
        Me.lblRelease = New System.Windows.Forms.Label()
        Me.gbHistory = New System.Windows.Forms.GroupBox()
        Me.lvHistory = New System.Windows.Forms.ListView()
        Me.pnlTop.SuspendLayout()
        Me.gbManager.SuspendLayout()
        Me.gbClose.SuspendLayout()
        Me.gbRanking.SuspendLayout()
        Me.gbRules.SuspendLayout()
        CType(Me.nudMin, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudRelease, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gbHistory.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlTop
        '
        Me.pnlTop.Controls.Add(Me.lblDbStatus)
        Me.pnlTop.Controls.Add(Me.cmdOptions)
        Me.pnlTop.Controls.Add(Me.lblDbInfo)
        Me.pnlTop.Controls.Add(Me.lblDatabase)
        Me.pnlTop.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlTop.Location = New System.Drawing.Point(0, 0)
        Me.pnlTop.Name = "pnlTop"
        Me.pnlTop.Size = New System.Drawing.Size(1034, 58)
        Me.pnlTop.TabIndex = 0
        '
        'lblDatabase
        '
        Me.lblDatabase.AutoSize = True
        Me.lblDatabase.Location = New System.Drawing.Point(6, 10)
        Me.lblDatabase.Name = "lblDatabase"
        Me.lblDatabase.Size = New System.Drawing.Size(56, 13)
        Me.lblDatabase.TabIndex = 0
        Me.lblDatabase.Text = "Database:"
        '
        'lblDbInfo
        '
        Me.lblDbInfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblDbInfo.AutoEllipsis = True
        Me.lblDbInfo.Location = New System.Drawing.Point(68, 10)
        Me.lblDbInfo.Name = "lblDbInfo"
        Me.lblDbInfo.Size = New System.Drawing.Size(830, 16)
        Me.lblDbInfo.TabIndex = 1
        '
        'cmdOptions
        '
        Me.cmdOptions.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdOptions.Location = New System.Drawing.Point(904, 5)
        Me.cmdOptions.Name = "cmdOptions"
        Me.cmdOptions.Size = New System.Drawing.Size(124, 23)
        Me.cmdOptions.TabIndex = 2
        Me.cmdOptions.Text = "Database options..."
        Me.cmdOptions.UseVisualStyleBackColor = True
        '
        'lblDbStatus
        '
        Me.lblDbStatus.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblDbStatus.AutoEllipsis = True
        Me.lblDbStatus.Location = New System.Drawing.Point(68, 34)
        Me.lblDbStatus.Name = "lblDbStatus"
        Me.lblDbStatus.Size = New System.Drawing.Size(960, 16)
        Me.lblDbStatus.TabIndex = 3
        '
        'gbManager
        '
        Me.gbManager.Controls.Add(Me.lblDeliverHint)
        Me.gbManager.Controls.Add(Me.cmdDeliver)
        Me.gbManager.Controls.Add(Me.cmdRefresh)
        Me.gbManager.Controls.Add(Me.lblManagerInfo)
        Me.gbManager.Location = New System.Drawing.Point(6, 62)
        Me.gbManager.Name = "gbManager"
        Me.gbManager.Size = New System.Drawing.Size(1022, 134)
        Me.gbManager.TabIndex = 1
        Me.gbManager.TabStop = False
        Me.gbManager.Text = "Manager and revenue (SoD2DB.Sod2Config)"
        '
        'lblManagerInfo
        '
        Me.lblManagerInfo.Location = New System.Drawing.Point(8, 20)
        Me.lblManagerInfo.Name = "lblManagerInfo"
        Me.lblManagerInfo.Size = New System.Drawing.Size(920, 72)
        Me.lblManagerInfo.TabIndex = 0
        '
        'cmdRefresh
        '
        Me.cmdRefresh.Location = New System.Drawing.Point(936, 18)
        Me.cmdRefresh.Name = "cmdRefresh"
        Me.cmdRefresh.Size = New System.Drawing.Size(78, 23)
        Me.cmdRefresh.TabIndex = 1
        Me.cmdRefresh.Text = "Refresh"
        Me.cmdRefresh.UseVisualStyleBackColor = True
        '
        'cmdDeliver
        '
        Me.cmdDeliver.Location = New System.Drawing.Point(8, 98)
        Me.cmdDeliver.Name = "cmdDeliver"
        Me.cmdDeliver.Size = New System.Drawing.Size(150, 23)
        Me.cmdDeliver.TabIndex = 2
        Me.cmdDeliver.Text = "Deliver revenue..."
        Me.cmdDeliver.UseVisualStyleBackColor = True
        '
        'lblDeliverHint
        '
        Me.lblDeliverHint.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblDeliverHint.Location = New System.Drawing.Point(166, 96)
        Me.lblDeliverHint.Name = "lblDeliverHint"
        Me.lblDeliverHint.Size = New System.Drawing.Size(848, 32)
        Me.lblDeliverHint.TabIndex = 3
        Me.lblDeliverHint.Text = "Collected = the tax taken from the prizes paid since the last delivery (SOD2Record points x tax). " &
            "Delivering adds the released share (see Rules) to the clan money of the manager; the leader withdraws it at Karina."
        '
        'gbClose
        '
        Me.gbClose.Controls.Add(Me.lblCloseHint)
        Me.gbClose.Controls.Add(Me.cmdClose)
        Me.gbClose.Controls.Add(Me.dtpNewEnd)
        Me.gbClose.Controls.Add(Me.lblNewEnd)
        Me.gbClose.Location = New System.Drawing.Point(6, 200)
        Me.gbClose.Name = "gbClose"
        Me.gbClose.Size = New System.Drawing.Size(1022, 74)
        Me.gbClose.TabIndex = 2
        Me.gbClose.TabStop = False
        Me.gbClose.Text = "Close the period"
        '
        'lblNewEnd
        '
        Me.lblNewEnd.AutoSize = True
        Me.lblNewEnd.Location = New System.Drawing.Point(8, 25)
        Me.lblNewEnd.Name = "lblNewEnd"
        Me.lblNewEnd.Size = New System.Drawing.Size(98, 13)
        Me.lblNewEnd.TabIndex = 0
        Me.lblNewEnd.Text = "New period ends:"
        '
        'dtpNewEnd
        '
        Me.dtpNewEnd.CustomFormat = "yyyy-MM-dd HH:mm"
        Me.dtpNewEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtpNewEnd.Location = New System.Drawing.Point(112, 21)
        Me.dtpNewEnd.Name = "dtpNewEnd"
        Me.dtpNewEnd.Size = New System.Drawing.Size(150, 20)
        Me.dtpNewEnd.TabIndex = 1
        '
        'cmdClose
        '
        Me.cmdClose.Location = New System.Drawing.Point(272, 19)
        Me.cmdClose.Name = "cmdClose"
        Me.cmdClose.Size = New System.Drawing.Size(150, 23)
        Me.cmdClose.TabIndex = 2
        Me.cmdClose.Text = "Close period..."
        Me.cmdClose.UseVisualStyleBackColor = True
        '
        'lblCloseHint
        '
        Me.lblCloseHint.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblCloseHint.Location = New System.Drawing.Point(8, 46)
        Me.lblCloseHint.Name = "lblCloseHint"
        Me.lblCloseHint.Size = New System.Drawing.Size(1006, 26)
        Me.lblCloseHint.TabIndex = 3
        Me.lblCloseHint.Text = "Delivers the pending revenue to the outgoing manager, saves the period in Sod2ConfigLog, makes the 1st of the " &
            "ranking the new manager (2nd and 3rd are recorded) and resets every clan's ranking points."
        '
        'gbRanking
        '
        Me.gbRanking.Controls.Add(Me.lvRanking)
        Me.gbRanking.Location = New System.Drawing.Point(6, 278)
        Me.gbRanking.Name = "gbRanking"
        Me.gbRanking.Size = New System.Drawing.Size(560, 196)
        Me.gbRanking.TabIndex = 3
        Me.gbRanking.TabStop = False
        Me.gbRanking.Text = "Ranking of the period (ClanDB.CL.Cpoint)"
        '
        'lvRanking
        '
        Me.lvRanking.FullRowSelect = True
        Me.lvRanking.GridLines = True
        Me.lvRanking.HideSelection = False
        Me.lvRanking.Location = New System.Drawing.Point(8, 18)
        Me.lvRanking.MultiSelect = False
        Me.lvRanking.Name = "lvRanking"
        Me.lvRanking.Size = New System.Drawing.Size(544, 170)
        Me.lvRanking.TabIndex = 0
        Me.lvRanking.UseCompatibleStateImageBehavior = False
        Me.lvRanking.View = System.Windows.Forms.View.Details
        '
        'gbRules
        '
        Me.gbRules.Controls.Add(Me.cmdSaveRelease)
        Me.gbRules.Controls.Add(Me.nudRelease)
        Me.gbRules.Controls.Add(Me.lblRelease)
        Me.gbRules.Controls.Add(Me.lblRulesHint)
        Me.gbRules.Controls.Add(Me.cmdSaveMin)
        Me.gbRules.Controls.Add(Me.nudMin)
        Me.gbRules.Controls.Add(Me.lblMin)
        Me.gbRules.Location = New System.Drawing.Point(572, 278)
        Me.gbRules.Name = "gbRules"
        Me.gbRules.Size = New System.Drawing.Size(456, 196)
        Me.gbRules.TabIndex = 4
        Me.gbRules.TabStop = False
        Me.gbRules.Text = "Rules (SoD2DB.Sod2Setting)"
        '
        'lblMin
        '
        Me.lblMin.AutoSize = True
        Me.lblMin.Location = New System.Drawing.Point(8, 25)
        Me.lblMin.Name = "lblMin"
        Me.lblMin.Size = New System.Drawing.Size(230, 13)
        Me.lblMin.TabIndex = 0
        Me.lblMin.Text = "Minimum clan members finishing the same SoD:"
        '
        'nudMin
        '
        Me.nudMin.Location = New System.Drawing.Point(262, 22)
        Me.nudMin.Maximum = New Decimal(New Integer() {20, 0, 0, 0})
        Me.nudMin.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudMin.Name = "nudMin"
        Me.nudMin.Size = New System.Drawing.Size(50, 20)
        Me.nudMin.TabIndex = 1
        Me.nudMin.Value = New Decimal(New Integer() {3, 0, 0, 0})
        '
        'cmdSaveMin
        '
        Me.cmdSaveMin.Location = New System.Drawing.Point(320, 20)
        Me.cmdSaveMin.Name = "cmdSaveMin"
        Me.cmdSaveMin.Size = New System.Drawing.Size(64, 23)
        Me.cmdSaveMin.TabIndex = 2
        Me.cmdSaveMin.Text = "Save"
        Me.cmdSaveMin.UseVisualStyleBackColor = True
        '
        'lblRulesHint
        '
        Me.lblRulesHint.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblRulesHint.Location = New System.Drawing.Point(8, 84)
        Me.lblRulesHint.Name = "lblRulesHint"
        Me.lblRulesHint.Size = New System.Drawing.Size(440, 106)
        Me.lblRulesHint.TabIndex = 6
        Me.lblRulesHint.Text = "Members: at the end of each SoD the database adds up the points of the clan members who finished it; " &
            "the clan's ranking points are raised only when that sum beats its current points and at least this many " &
            "members finished (3 in the 2004 guide and the PT guide). Applies from the next SoD." & Global.Microsoft.VisualBasic.ControlChars.CrLf &
            "Release: the tax (set in the Bellatra tab) is taken from the players' prizes; this is the share of it that " &
            "goes to the manager clan when you deliver the revenue."
        '
        'lblRelease
        '
        Me.lblRelease.AutoSize = True
        Me.lblRelease.Location = New System.Drawing.Point(8, 57)
        Me.lblRelease.Name = "lblRelease"
        Me.lblRelease.Size = New System.Drawing.Size(250, 13)
        Me.lblRelease.TabIndex = 3
        Me.lblRelease.Text = "Share of the collected tax released to the clan (%):"
        '
        'nudRelease
        '
        Me.nudRelease.Location = New System.Drawing.Point(262, 54)
        Me.nudRelease.Name = "nudRelease"
        Me.nudRelease.Size = New System.Drawing.Size(50, 20)
        Me.nudRelease.TabIndex = 4
        Me.nudRelease.Value = New Decimal(New Integer() {100, 0, 0, 0})
        '
        'cmdSaveRelease
        '
        Me.cmdSaveRelease.Location = New System.Drawing.Point(320, 52)
        Me.cmdSaveRelease.Name = "cmdSaveRelease"
        Me.cmdSaveRelease.Size = New System.Drawing.Size(64, 23)
        Me.cmdSaveRelease.TabIndex = 5
        Me.cmdSaveRelease.Text = "Save"
        Me.cmdSaveRelease.UseVisualStyleBackColor = True
        '
        'gbHistory
        '
        Me.gbHistory.Controls.Add(Me.lvHistory)
        Me.gbHistory.Location = New System.Drawing.Point(6, 478)
        Me.gbHistory.Name = "gbHistory"
        Me.gbHistory.Size = New System.Drawing.Size(1022, 172)
        Me.gbHistory.TabIndex = 5
        Me.gbHistory.TabStop = False
        Me.gbHistory.Text = "Previous periods (SoD2DB.Sod2ConfigLog)"
        '
        'lvHistory
        '
        Me.lvHistory.FullRowSelect = True
        Me.lvHistory.GridLines = True
        Me.lvHistory.HideSelection = False
        Me.lvHistory.Location = New System.Drawing.Point(8, 18)
        Me.lvHistory.MultiSelect = False
        Me.lvHistory.Name = "lvHistory"
        Me.lvHistory.Size = New System.Drawing.Size(1006, 146)
        Me.lvHistory.TabIndex = 0
        Me.lvHistory.UseCompatibleStateImageBehavior = False
        Me.lvHistory.View = System.Windows.Forms.View.Details
        '
        'BellatraAdminPanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.gbHistory)
        Me.Controls.Add(Me.gbRules)
        Me.Controls.Add(Me.gbRanking)
        Me.Controls.Add(Me.gbClose)
        Me.Controls.Add(Me.gbManager)
        Me.Controls.Add(Me.pnlTop)
        Me.Name = "BellatraAdminPanel"
        Me.Size = New System.Drawing.Size(1034, 654)
        Me.pnlTop.ResumeLayout(False)
        Me.pnlTop.PerformLayout()
        Me.gbManager.ResumeLayout(False)
        Me.gbClose.ResumeLayout(False)
        Me.gbClose.PerformLayout()
        Me.gbRanking.ResumeLayout(False)
        CType(Me.nudMin, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudRelease, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gbRules.ResumeLayout(False)
        Me.gbRules.PerformLayout()
        Me.gbHistory.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlTop As System.Windows.Forms.Panel
    Friend WithEvents lblDbStatus As System.Windows.Forms.Label
    Friend WithEvents cmdOptions As System.Windows.Forms.Button
    Friend WithEvents lblDbInfo As System.Windows.Forms.Label
    Friend WithEvents lblDatabase As System.Windows.Forms.Label
    Friend WithEvents gbManager As System.Windows.Forms.GroupBox
    Friend WithEvents lblDeliverHint As System.Windows.Forms.Label
    Friend WithEvents cmdDeliver As System.Windows.Forms.Button
    Friend WithEvents cmdRefresh As System.Windows.Forms.Button
    Friend WithEvents lblManagerInfo As System.Windows.Forms.Label
    Friend WithEvents gbClose As System.Windows.Forms.GroupBox
    Friend WithEvents lblCloseHint As System.Windows.Forms.Label
    Friend WithEvents cmdClose As System.Windows.Forms.Button
    Friend WithEvents dtpNewEnd As System.Windows.Forms.DateTimePicker
    Friend WithEvents lblNewEnd As System.Windows.Forms.Label
    Friend WithEvents gbRanking As System.Windows.Forms.GroupBox
    Friend WithEvents lvRanking As System.Windows.Forms.ListView
    Friend WithEvents gbRules As System.Windows.Forms.GroupBox
    Friend WithEvents lblRulesHint As System.Windows.Forms.Label
    Friend WithEvents cmdSaveMin As System.Windows.Forms.Button
    Friend WithEvents nudMin As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblMin As System.Windows.Forms.Label
    Friend WithEvents cmdSaveRelease As System.Windows.Forms.Button
    Friend WithEvents nudRelease As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblRelease As System.Windows.Forms.Label
    Friend WithEvents gbHistory As System.Windows.Forms.GroupBox
    Friend WithEvents lvHistory As System.Windows.Forms.ListView

End Class
