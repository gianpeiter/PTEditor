<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class BellatraPanel
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
        Me.gbOwner = New System.Windows.Forms.GroupBox()
        Me.cmdRefresh = New System.Windows.Forms.Button()
        Me.lblGreetingHint = New System.Windows.Forms.Label()
        Me.cmdSaveGreeting = New System.Windows.Forms.Button()
        Me.tbGreeting = New System.Windows.Forms.TextBox()
        Me.lblGreeting = New System.Windows.Forms.Label()
        Me.lblOwnerInfo = New System.Windows.Forms.Label()
        Me.gbSettings = New System.Windows.Forms.GroupBox()
        Me.lblSettingsInfo = New System.Windows.Forms.Label()
        Me.cmdSave = New System.Windows.Forms.Button()
        Me.cmdRemoveFile = New System.Windows.Forms.Button()
        Me.cmdReloadFile = New System.Windows.Forms.Button()
        Me.cmdLoadLegacy = New System.Windows.Forms.Button()
        Me.cbLegacyTable = New System.Windows.Forms.ComboBox()
        Me.pnlRounds = New System.Windows.Forms.Panel()
        Me.lblTaxHint = New System.Windows.Forms.Label()
        Me.cbTax = New System.Windows.Forms.ComboBox()
        Me.lblTax = New System.Windows.Forms.Label()
        Me.lblFileInfo = New System.Windows.Forms.Label()
        Me.pnlTop.SuspendLayout()
        Me.gbOwner.SuspendLayout()
        Me.gbSettings.SuspendLayout()
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
        'gbOwner
        '
        Me.gbOwner.Controls.Add(Me.cmdRefresh)
        Me.gbOwner.Controls.Add(Me.lblGreetingHint)
        Me.gbOwner.Controls.Add(Me.cmdSaveGreeting)
        Me.gbOwner.Controls.Add(Me.tbGreeting)
        Me.gbOwner.Controls.Add(Me.lblGreeting)
        Me.gbOwner.Controls.Add(Me.lblOwnerInfo)
        Me.gbOwner.Location = New System.Drawing.Point(6, 62)
        Me.gbOwner.Name = "gbOwner"
        Me.gbOwner.Size = New System.Drawing.Size(1022, 150)
        Me.gbOwner.TabIndex = 1
        Me.gbOwner.TabStop = False
        Me.gbOwner.Text = "Clan that owns the Bellatra (most Bellatra points, as Karina shows it)"
        '
        'lblOwnerInfo
        '
        Me.lblOwnerInfo.Location = New System.Drawing.Point(8, 20)
        Me.lblOwnerInfo.Name = "lblOwnerInfo"
        Me.lblOwnerInfo.Size = New System.Drawing.Size(920, 58)
        Me.lblOwnerInfo.TabIndex = 0
        '
        'lblGreeting
        '
        Me.lblGreeting.AutoSize = True
        Me.lblGreeting.Location = New System.Drawing.Point(8, 82)
        Me.lblGreeting.Name = "lblGreeting"
        Me.lblGreeting.Size = New System.Drawing.Size(220, 13)
        Me.lblGreeting.TabIndex = 1
        Me.lblGreeting.Text = "Karina's greeting:"
        '
        'tbGreeting
        '
        Me.tbGreeting.Location = New System.Drawing.Point(8, 100)
        Me.tbGreeting.MaxLength = 89
        Me.tbGreeting.Name = "tbGreeting"
        Me.tbGreeting.Size = New System.Drawing.Size(600, 20)
        Me.tbGreeting.TabIndex = 2
        '
        'cmdSaveGreeting
        '
        Me.cmdSaveGreeting.Location = New System.Drawing.Point(614, 98)
        Me.cmdSaveGreeting.Name = "cmdSaveGreeting"
        Me.cmdSaveGreeting.Size = New System.Drawing.Size(64, 23)
        Me.cmdSaveGreeting.TabIndex = 3
        Me.cmdSaveGreeting.Text = "Save"
        Me.cmdSaveGreeting.UseVisualStyleBackColor = True
        '
        'lblGreetingHint
        '
        Me.lblGreetingHint.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblGreetingHint.Location = New System.Drawing.Point(8, 124)
        Me.lblGreetingHint.Name = "lblGreetingHint"
        Me.lblGreetingHint.Size = New System.Drawing.Size(900, 18)
        Me.lblGreetingHint.TabIndex = 4
        Me.lblGreetingHint.Text = "Shown in Karina's window (up to 89 characters). Saved as the clan's note (ClanDB.CL.Note)."
        '
        'cmdRefresh
        '
        Me.cmdRefresh.Location = New System.Drawing.Point(936, 18)
        Me.cmdRefresh.Name = "cmdRefresh"
        Me.cmdRefresh.Size = New System.Drawing.Size(78, 23)
        Me.cmdRefresh.TabIndex = 6
        Me.cmdRefresh.Text = "Refresh"
        Me.cmdRefresh.UseVisualStyleBackColor = True
        '
        'gbSettings
        '
        Me.gbSettings.Controls.Add(Me.lblSettingsInfo)
        Me.gbSettings.Controls.Add(Me.cmdSave)
        Me.gbSettings.Controls.Add(Me.cmdRemoveFile)
        Me.gbSettings.Controls.Add(Me.cmdReloadFile)
        Me.gbSettings.Controls.Add(Me.cmdLoadLegacy)
        Me.gbSettings.Controls.Add(Me.cbLegacyTable)
        Me.gbSettings.Controls.Add(Me.pnlRounds)
        Me.gbSettings.Controls.Add(Me.lblTaxHint)
        Me.gbSettings.Controls.Add(Me.cbTax)
        Me.gbSettings.Controls.Add(Me.lblTax)
        Me.gbSettings.Controls.Add(Me.lblFileInfo)
        Me.gbSettings.Location = New System.Drawing.Point(6, 216)
        Me.gbSettings.Name = "gbSettings"
        Me.gbSettings.Size = New System.Drawing.Size(1022, 410)
        Me.gbSettings.TabIndex = 2
        Me.gbSettings.TabStop = False
        Me.gbSettings.Text = "Tournament settings (Bellatra\bellatra.ini)"
        '
        'lblFileInfo
        '
        Me.lblFileInfo.AutoEllipsis = True
        Me.lblFileInfo.Location = New System.Drawing.Point(8, 20)
        Me.lblFileInfo.Name = "lblFileInfo"
        Me.lblFileInfo.Size = New System.Drawing.Size(1006, 16)
        Me.lblFileInfo.TabIndex = 0
        '
        'lblTax
        '
        Me.lblTax.AutoSize = True
        Me.lblTax.Location = New System.Drawing.Point(8, 46)
        Me.lblTax.Name = "lblTax"
        Me.lblTax.Size = New System.Drawing.Size(28, 13)
        Me.lblTax.TabIndex = 1
        Me.lblTax.Text = "Tax:"
        '
        'cbTax
        '
        Me.cbTax.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbTax.Location = New System.Drawing.Point(48, 42)
        Me.cbTax.Name = "cbTax"
        Me.cbTax.Size = New System.Drawing.Size(60, 21)
        Me.cbTax.TabIndex = 2
        '
        'lblTaxHint
        '
        Me.lblTaxHint.AutoSize = True
        Me.lblTaxHint.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblTaxHint.Location = New System.Drawing.Point(116, 46)
        Me.lblTaxHint.Name = "lblTaxHint"
        Me.lblTaxHint.Size = New System.Drawing.Size(500, 13)
        Me.lblTaxHint.TabIndex = 3
        Me.lblTaxHint.Text = "Taken from each player's prize, not credited to any clan (0 to 10%, as on the legacy site). Also copied to Sod2Config for Karina."
        '
        'pnlRounds
        '
        Me.pnlRounds.Location = New System.Drawing.Point(8, 70)
        Me.pnlRounds.Name = "pnlRounds"
        Me.pnlRounds.Size = New System.Drawing.Size(1006, 262)
        Me.pnlRounds.TabIndex = 4
        '
        'cbLegacyTable
        '
        Me.cbLegacyTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbLegacyTable.Location = New System.Drawing.Point(8, 342)
        Me.cbLegacyTable.Name = "cbLegacyTable"
        Me.cbLegacyTable.Size = New System.Drawing.Size(110, 21)
        Me.cbLegacyTable.TabIndex = 5
        '
        'cmdLoadLegacy
        '
        Me.cmdLoadLegacy.Location = New System.Drawing.Point(124, 341)
        Me.cmdLoadLegacy.Name = "cmdLoadLegacy"
        Me.cmdLoadLegacy.Size = New System.Drawing.Size(110, 23)
        Me.cmdLoadLegacy.TabIndex = 6
        Me.cmdLoadLegacy.Text = "Load monsters"
        Me.cmdLoadLegacy.UseVisualStyleBackColor = True
        '
        'cmdReloadFile
        '
        Me.cmdReloadFile.Location = New System.Drawing.Point(250, 341)
        Me.cmdReloadFile.Name = "cmdReloadFile"
        Me.cmdReloadFile.Size = New System.Drawing.Size(100, 23)
        Me.cmdReloadFile.TabIndex = 7
        Me.cmdReloadFile.Text = "Reload file"
        Me.cmdReloadFile.UseVisualStyleBackColor = True
        '
        'cmdRemoveFile
        '
        Me.cmdRemoveFile.Location = New System.Drawing.Point(356, 341)
        Me.cmdRemoveFile.Name = "cmdRemoveFile"
        Me.cmdRemoveFile.Size = New System.Drawing.Size(150, 23)
        Me.cmdRemoveFile.TabIndex = 8
        Me.cmdRemoveFile.Text = "Remove file..."
        Me.cmdRemoveFile.UseVisualStyleBackColor = True
        '
        'cmdSave
        '
        Me.cmdSave.Location = New System.Drawing.Point(914, 341)
        Me.cmdSave.Name = "cmdSave"
        Me.cmdSave.Size = New System.Drawing.Size(100, 23)
        Me.cmdSave.TabIndex = 9
        Me.cmdSave.Text = "Save"
        Me.cmdSave.UseVisualStyleBackColor = True
        '
        'lblSettingsInfo
        '
        Me.lblSettingsInfo.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblSettingsInfo.Location = New System.Drawing.Point(8, 370)
        Me.lblSettingsInfo.Name = "lblSettingsInfo"
        Me.lblSettingsInfo.Size = New System.Drawing.Size(1006, 34)
        Me.lblSettingsInfo.TabIndex = 10
        Me.lblSettingsInfo.Text = "The server reads this file at the start of each SoD: changes apply to the next SoD, without restarting. " &
            "Without the file, each SoD uses one of the 3 code tables at random. Monster names are the in-game names (.zhoon). " &
            "% = chance of each monster; max alive = monsters alive at once per room (the boss shows up after max - 10)."
        '
        'BellatraPanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.gbSettings)
        Me.Controls.Add(Me.gbOwner)
        Me.Controls.Add(Me.pnlTop)
        Me.Name = "BellatraPanel"
        Me.Size = New System.Drawing.Size(1034, 654)
        Me.pnlTop.ResumeLayout(False)
        Me.pnlTop.PerformLayout()
        Me.gbOwner.ResumeLayout(False)
        Me.gbOwner.PerformLayout()
        Me.gbSettings.ResumeLayout(False)
        Me.gbSettings.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlTop As System.Windows.Forms.Panel
    Friend WithEvents lblDbStatus As System.Windows.Forms.Label
    Friend WithEvents cmdOptions As System.Windows.Forms.Button
    Friend WithEvents lblDbInfo As System.Windows.Forms.Label
    Friend WithEvents lblDatabase As System.Windows.Forms.Label
    Friend WithEvents gbOwner As System.Windows.Forms.GroupBox
    Friend WithEvents cmdRefresh As System.Windows.Forms.Button
    Friend WithEvents lblGreetingHint As System.Windows.Forms.Label
    Friend WithEvents cmdSaveGreeting As System.Windows.Forms.Button
    Friend WithEvents tbGreeting As System.Windows.Forms.TextBox
    Friend WithEvents lblGreeting As System.Windows.Forms.Label
    Friend WithEvents lblOwnerInfo As System.Windows.Forms.Label
    Friend WithEvents gbSettings As System.Windows.Forms.GroupBox
    Friend WithEvents lblSettingsInfo As System.Windows.Forms.Label
    Friend WithEvents cmdSave As System.Windows.Forms.Button
    Friend WithEvents cmdRemoveFile As System.Windows.Forms.Button
    Friend WithEvents cmdReloadFile As System.Windows.Forms.Button
    Friend WithEvents cmdLoadLegacy As System.Windows.Forms.Button
    Friend WithEvents cbLegacyTable As System.Windows.Forms.ComboBox
    Friend WithEvents pnlRounds As System.Windows.Forms.Panel
    Friend WithEvents lblTaxHint As System.Windows.Forms.Label
    Friend WithEvents cbTax As System.Windows.Forms.ComboBox
    Friend WithEvents lblTax As System.Windows.Forms.Label
    Friend WithEvents lblFileInfo As System.Windows.Forms.Label

End Class
