<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class PostBoxPanel
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
        Me.lblWarning = New System.Windows.Forms.Label()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.cmdOptions = New System.Windows.Forms.Button()
        Me.lblFolderInfo = New System.Windows.Forms.Label()
        Me.lblFolder = New System.Windows.Forms.Label()
        Me.gbRecipient = New System.Windows.Forms.GroupBox()
        Me.lblRecipientInfo = New System.Windows.Forms.Label()
        Me.cmdShowPending = New System.Windows.Forms.Button()
        Me.chkAnyCharacter = New System.Windows.Forms.CheckBox()
        Me.cbCharacter = New System.Windows.Forms.ComboBox()
        Me.lblCharacter = New System.Windows.Forms.Label()
        Me.gbPending = New System.Windows.Forms.GroupBox()
        Me.lblPendingInfo = New System.Windows.Forms.Label()
        Me.cmdRemovePending = New System.Windows.Forms.Button()
        Me.lvPending = New System.Windows.Forms.ListView()
        Me.gbItems = New System.Windows.Forms.GroupBox()
        Me.lblSendInfo = New System.Windows.Forms.Label()
        Me.cmdSend = New System.Windows.Forms.Button()
        Me.tbMessage = New System.Windows.Forms.TextBox()
        Me.lblMessage = New System.Windows.Forms.Label()
        Me.cmdRemoveSend = New System.Windows.Forms.Button()
        Me.lvSend = New System.Windows.Forms.ListView()
        Me.lblToSend = New System.Windows.Forms.Label()
        Me.nudQuantity = New System.Windows.Forms.NumericUpDown()
        Me.lblQuantity = New System.Windows.Forms.Label()
        Me.nudSpec = New System.Windows.Forms.NumericUpDown()
        Me.lblSpec = New System.Windows.Forms.Label()
        Me.cmdAdd = New System.Windows.Forms.Button()
        Me.lvSearch = New System.Windows.Forms.ListView()
        Me.cmdSearch = New System.Windows.Forms.Button()
        Me.tbSearch = New System.Windows.Forms.TextBox()
        Me.lblSearch = New System.Windows.Forms.Label()
        Me.pnlTop.SuspendLayout()
        Me.gbRecipient.SuspendLayout()
        Me.gbPending.SuspendLayout()
        Me.gbItems.SuspendLayout()
        CType(Me.nudQuantity, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudSpec, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pnlTop
        '
        Me.pnlTop.Controls.Add(Me.lblWarning)
        Me.pnlTop.Controls.Add(Me.lblStatus)
        Me.pnlTop.Controls.Add(Me.cmdOptions)
        Me.pnlTop.Controls.Add(Me.lblFolderInfo)
        Me.pnlTop.Controls.Add(Me.lblFolder)
        Me.pnlTop.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlTop.Location = New System.Drawing.Point(0, 0)
        Me.pnlTop.Name = "pnlTop"
        Me.pnlTop.Size = New System.Drawing.Size(1034, 78)
        Me.pnlTop.TabIndex = 0
        '
        'lblFolder
        '
        Me.lblFolder.AutoSize = True
        Me.lblFolder.Location = New System.Drawing.Point(6, 10)
        Me.lblFolder.Name = "lblFolder"
        Me.lblFolder.Size = New System.Drawing.Size(48, 13)
        Me.lblFolder.TabIndex = 0
        Me.lblFolder.Text = "PostBox:"
        '
        'lblFolderInfo
        '
        Me.lblFolderInfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblFolderInfo.AutoEllipsis = True
        Me.lblFolderInfo.Location = New System.Drawing.Point(68, 10)
        Me.lblFolderInfo.Name = "lblFolderInfo"
        Me.lblFolderInfo.Size = New System.Drawing.Size(830, 16)
        Me.lblFolderInfo.TabIndex = 1
        '
        'cmdOptions
        '
        Me.cmdOptions.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdOptions.Location = New System.Drawing.Point(904, 5)
        Me.cmdOptions.Name = "cmdOptions"
        Me.cmdOptions.Size = New System.Drawing.Size(124, 23)
        Me.cmdOptions.TabIndex = 2
        Me.cmdOptions.Text = "Server options..."
        Me.cmdOptions.UseVisualStyleBackColor = True
        '
        'lblStatus
        '
        Me.lblStatus.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblStatus.AutoEllipsis = True
        Me.lblStatus.Location = New System.Drawing.Point(68, 34)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(960, 16)
        Me.lblStatus.TabIndex = 3
        '
        'lblWarning
        '
        Me.lblWarning.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblWarning.AutoEllipsis = True
        Me.lblWarning.ForeColor = System.Drawing.Color.DarkOrange
        Me.lblWarning.Location = New System.Drawing.Point(6, 56)
        Me.lblWarning.Name = "lblWarning"
        Me.lblWarning.Size = New System.Drawing.Size(1022, 16)
        Me.lblWarning.TabIndex = 4
        Me.lblWarning.Text = "Send with the player offline: if the account already opened the PostBox this session, the logout rewrites the " &
            "file from memory and may overwrite changes made here."
        '
        'gbRecipient
        '
        Me.gbRecipient.Controls.Add(Me.lblRecipientInfo)
        Me.gbRecipient.Controls.Add(Me.cmdShowPending)
        Me.gbRecipient.Controls.Add(Me.chkAnyCharacter)
        Me.gbRecipient.Controls.Add(Me.cbCharacter)
        Me.gbRecipient.Controls.Add(Me.lblCharacter)
        Me.gbRecipient.Location = New System.Drawing.Point(6, 82)
        Me.gbRecipient.Name = "gbRecipient"
        Me.gbRecipient.Size = New System.Drawing.Size(320, 182)
        Me.gbRecipient.TabIndex = 1
        Me.gbRecipient.TabStop = False
        Me.gbRecipient.Text = "Recipient"
        '
        'lblCharacter
        '
        Me.lblCharacter.AutoSize = True
        Me.lblCharacter.Location = New System.Drawing.Point(8, 24)
        Me.lblCharacter.Name = "lblCharacter"
        Me.lblCharacter.Size = New System.Drawing.Size(56, 13)
        Me.lblCharacter.TabIndex = 0
        Me.lblCharacter.Text = "Character:"
        '
        'cbCharacter
        '
        Me.cbCharacter.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.cbCharacter.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cbCharacter.Location = New System.Drawing.Point(72, 20)
        Me.cbCharacter.MaxLength = 31
        Me.cbCharacter.Name = "cbCharacter"
        Me.cbCharacter.Size = New System.Drawing.Size(240, 21)
        Me.cbCharacter.TabIndex = 1
        '
        'chkAnyCharacter
        '
        Me.chkAnyCharacter.AutoSize = True
        Me.chkAnyCharacter.Location = New System.Drawing.Point(72, 47)
        Me.chkAnyCharacter.Name = "chkAnyCharacter"
        Me.chkAnyCharacter.Size = New System.Drawing.Size(196, 17)
        Me.chkAnyCharacter.TabIndex = 2
        Me.chkAnyCharacter.Text = "Any character of the account (*)"
        Me.chkAnyCharacter.UseVisualStyleBackColor = True
        '
        'cmdShowPending
        '
        Me.cmdShowPending.Location = New System.Drawing.Point(72, 70)
        Me.cmdShowPending.Name = "cmdShowPending"
        Me.cmdShowPending.Size = New System.Drawing.Size(140, 23)
        Me.cmdShowPending.TabIndex = 3
        Me.cmdShowPending.Text = "Show pending items"
        Me.cmdShowPending.UseVisualStyleBackColor = True
        '
        'lblRecipientInfo
        '
        Me.lblRecipientInfo.Location = New System.Drawing.Point(8, 100)
        Me.lblRecipientInfo.Name = "lblRecipientInfo"
        Me.lblRecipientInfo.Size = New System.Drawing.Size(304, 76)
        Me.lblRecipientInfo.TabIndex = 4
        '
        'gbPending
        '
        Me.gbPending.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.gbPending.Controls.Add(Me.lblPendingInfo)
        Me.gbPending.Controls.Add(Me.cmdRemovePending)
        Me.gbPending.Controls.Add(Me.lvPending)
        Me.gbPending.Location = New System.Drawing.Point(6, 270)
        Me.gbPending.Name = "gbPending"
        Me.gbPending.Size = New System.Drawing.Size(320, 378)
        Me.gbPending.TabIndex = 2
        Me.gbPending.TabStop = False
        Me.gbPending.Text = "Pending in the account's PostBox"
        '
        'lvPending
        '
        Me.lvPending.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvPending.FullRowSelect = True
        Me.lvPending.GridLines = True
        Me.lvPending.HideSelection = False
        Me.lvPending.Location = New System.Drawing.Point(8, 20)
        Me.lvPending.Name = "lvPending"
        Me.lvPending.Size = New System.Drawing.Size(304, 278)
        Me.lvPending.TabIndex = 0
        Me.lvPending.UseCompatibleStateImageBehavior = False
        Me.lvPending.View = System.Windows.Forms.View.Details
        '
        'cmdRemovePending
        '
        Me.cmdRemovePending.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdRemovePending.Location = New System.Drawing.Point(8, 304)
        Me.cmdRemovePending.Name = "cmdRemovePending"
        Me.cmdRemovePending.Size = New System.Drawing.Size(130, 23)
        Me.cmdRemovePending.TabIndex = 1
        Me.cmdRemovePending.Text = "Remove selected"
        Me.cmdRemovePending.UseVisualStyleBackColor = True
        '
        'lblPendingInfo
        '
        Me.lblPendingInfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblPendingInfo.Location = New System.Drawing.Point(8, 332)
        Me.lblPendingInfo.Name = "lblPendingInfo"
        Me.lblPendingInfo.Size = New System.Drawing.Size(304, 40)
        Me.lblPendingInfo.TabIndex = 2
        '
        'gbItems
        '
        Me.gbItems.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.gbItems.Controls.Add(Me.lblSendInfo)
        Me.gbItems.Controls.Add(Me.cmdSend)
        Me.gbItems.Controls.Add(Me.tbMessage)
        Me.gbItems.Controls.Add(Me.lblMessage)
        Me.gbItems.Controls.Add(Me.cmdRemoveSend)
        Me.gbItems.Controls.Add(Me.lvSend)
        Me.gbItems.Controls.Add(Me.lblToSend)
        Me.gbItems.Controls.Add(Me.nudQuantity)
        Me.gbItems.Controls.Add(Me.lblQuantity)
        Me.gbItems.Controls.Add(Me.nudSpec)
        Me.gbItems.Controls.Add(Me.lblSpec)
        Me.gbItems.Controls.Add(Me.cmdAdd)
        Me.gbItems.Controls.Add(Me.lvSearch)
        Me.gbItems.Controls.Add(Me.cmdSearch)
        Me.gbItems.Controls.Add(Me.tbSearch)
        Me.gbItems.Controls.Add(Me.lblSearch)
        Me.gbItems.Location = New System.Drawing.Point(332, 82)
        Me.gbItems.Name = "gbItems"
        Me.gbItems.Size = New System.Drawing.Size(696, 566)
        Me.gbItems.TabIndex = 3
        Me.gbItems.TabStop = False
        Me.gbItems.Text = "Items to send"
        '
        'lblSearch
        '
        Me.lblSearch.AutoSize = True
        Me.lblSearch.Location = New System.Drawing.Point(8, 24)
        Me.lblSearch.Name = "lblSearch"
        Me.lblSearch.Size = New System.Drawing.Size(44, 13)
        Me.lblSearch.TabIndex = 0
        Me.lblSearch.Text = "Search:"
        '
        'tbSearch
        '
        Me.tbSearch.Location = New System.Drawing.Point(58, 20)
        Me.tbSearch.Name = "tbSearch"
        Me.tbSearch.Size = New System.Drawing.Size(220, 20)
        Me.tbSearch.TabIndex = 1
        '
        'cmdSearch
        '
        Me.cmdSearch.Location = New System.Drawing.Point(284, 18)
        Me.cmdSearch.Name = "cmdSearch"
        Me.cmdSearch.Size = New System.Drawing.Size(80, 23)
        Me.cmdSearch.TabIndex = 2
        Me.cmdSearch.Text = "Search"
        Me.cmdSearch.UseVisualStyleBackColor = True
        '
        'lvSearch
        '
        Me.lvSearch.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvSearch.FullRowSelect = True
        Me.lvSearch.GridLines = True
        Me.lvSearch.HideSelection = False
        Me.lvSearch.Location = New System.Drawing.Point(8, 46)
        Me.lvSearch.Name = "lvSearch"
        Me.lvSearch.Size = New System.Drawing.Size(680, 210)
        Me.lvSearch.TabIndex = 3
        Me.lvSearch.UseCompatibleStateImageBehavior = False
        Me.lvSearch.View = System.Windows.Forms.View.Details
        '
        'cmdAdd
        '
        Me.cmdAdd.Location = New System.Drawing.Point(8, 262)
        Me.cmdAdd.Name = "cmdAdd"
        Me.cmdAdd.Size = New System.Drawing.Size(130, 23)
        Me.cmdAdd.TabIndex = 4
        Me.cmdAdd.Text = "Add to list"
        Me.cmdAdd.UseVisualStyleBackColor = True
        '
        'lblSpec
        '
        Me.lblSpec.AutoSize = True
        Me.lblSpec.Location = New System.Drawing.Point(156, 267)
        Me.lblSpec.Name = "lblSpec"
        Me.lblSpec.Size = New System.Drawing.Size(122, 13)
        Me.lblSpec.TabIndex = 5
        Me.lblSpec.Text = "Class spec (0 = random):"
        '
        'nudSpec
        '
        Me.nudSpec.Location = New System.Drawing.Point(284, 264)
        Me.nudSpec.Maximum = New Decimal(New Integer() {99, 0, 0, 0})
        Me.nudSpec.Name = "nudSpec"
        Me.nudSpec.Size = New System.Drawing.Size(50, 20)
        Me.nudSpec.TabIndex = 6
        '
        'lblQuantity
        '
        Me.lblQuantity.AutoSize = True
        Me.lblQuantity.Location = New System.Drawing.Point(352, 267)
        Me.lblQuantity.Name = "lblQuantity"
        Me.lblQuantity.Size = New System.Drawing.Size(84, 13)
        Me.lblQuantity.TabIndex = 7
        Me.lblQuantity.Text = "Potion quantity:"
        '
        'nudQuantity
        '
        Me.nudQuantity.Location = New System.Drawing.Point(440, 264)
        Me.nudQuantity.Maximum = New Decimal(New Integer() {9999, 0, 0, 0})
        Me.nudQuantity.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudQuantity.Name = "nudQuantity"
        Me.nudQuantity.Size = New System.Drawing.Size(60, 20)
        Me.nudQuantity.TabIndex = 8
        Me.nudQuantity.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'lblToSend
        '
        Me.lblToSend.AutoSize = True
        Me.lblToSend.Location = New System.Drawing.Point(8, 296)
        Me.lblToSend.Name = "lblToSend"
        Me.lblToSend.Size = New System.Drawing.Size(48, 13)
        Me.lblToSend.TabIndex = 9
        Me.lblToSend.Text = "To send:"
        '
        'lvSend
        '
        Me.lvSend.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvSend.FullRowSelect = True
        Me.lvSend.GridLines = True
        Me.lvSend.HideSelection = False
        Me.lvSend.Location = New System.Drawing.Point(8, 314)
        Me.lvSend.Name = "lvSend"
        Me.lvSend.Size = New System.Drawing.Size(680, 140)
        Me.lvSend.TabIndex = 10
        Me.lvSend.UseCompatibleStateImageBehavior = False
        Me.lvSend.View = System.Windows.Forms.View.Details
        '
        'cmdRemoveSend
        '
        Me.cmdRemoveSend.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdRemoveSend.Location = New System.Drawing.Point(8, 460)
        Me.cmdRemoveSend.Name = "cmdRemoveSend"
        Me.cmdRemoveSend.Size = New System.Drawing.Size(130, 23)
        Me.cmdRemoveSend.TabIndex = 11
        Me.cmdRemoveSend.Text = "Remove from list"
        Me.cmdRemoveSend.UseVisualStyleBackColor = True
        '
        'lblMessage
        '
        Me.lblMessage.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lblMessage.AutoSize = True
        Me.lblMessage.Location = New System.Drawing.Point(156, 465)
        Me.lblMessage.Name = "lblMessage"
        Me.lblMessage.Size = New System.Drawing.Size(53, 13)
        Me.lblMessage.TabIndex = 12
        Me.lblMessage.Text = "Message:"
        '
        'tbMessage
        '
        Me.tbMessage.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbMessage.Location = New System.Drawing.Point(214, 462)
        Me.tbMessage.Name = "tbMessage"
        Me.tbMessage.Size = New System.Drawing.Size(306, 20)
        Me.tbMessage.TabIndex = 13
        '
        'cmdSend
        '
        Me.cmdSend.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdSend.Location = New System.Drawing.Point(528, 458)
        Me.cmdSend.Name = "cmdSend"
        Me.cmdSend.Size = New System.Drawing.Size(160, 30)
        Me.cmdSend.TabIndex = 14
        Me.cmdSend.Text = "Send to PostBox"
        Me.cmdSend.UseVisualStyleBackColor = True
        '
        'lblSendInfo
        '
        Me.lblSendInfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSendInfo.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblSendInfo.Location = New System.Drawing.Point(8, 494)
        Me.lblSendInfo.Name = "lblSendInfo"
        Me.lblSendInfo.Size = New System.Drawing.Size(680, 66)
        Me.lblSendInfo.TabIndex = 15
        '
        'PostBoxPanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.gbItems)
        Me.Controls.Add(Me.gbPending)
        Me.Controls.Add(Me.gbRecipient)
        Me.Controls.Add(Me.pnlTop)
        Me.Name = "PostBoxPanel"
        Me.Size = New System.Drawing.Size(1034, 654)
        Me.pnlTop.ResumeLayout(False)
        Me.pnlTop.PerformLayout()
        Me.gbRecipient.ResumeLayout(False)
        Me.gbRecipient.PerformLayout()
        Me.gbPending.ResumeLayout(False)
        Me.gbItems.ResumeLayout(False)
        Me.gbItems.PerformLayout()
        CType(Me.nudQuantity, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudSpec, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlTop As System.Windows.Forms.Panel
    Friend WithEvents lblWarning As System.Windows.Forms.Label
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents cmdOptions As System.Windows.Forms.Button
    Friend WithEvents lblFolderInfo As System.Windows.Forms.Label
    Friend WithEvents lblFolder As System.Windows.Forms.Label
    Friend WithEvents gbRecipient As System.Windows.Forms.GroupBox
    Friend WithEvents lblRecipientInfo As System.Windows.Forms.Label
    Friend WithEvents cmdShowPending As System.Windows.Forms.Button
    Friend WithEvents chkAnyCharacter As System.Windows.Forms.CheckBox
    Friend WithEvents cbCharacter As System.Windows.Forms.ComboBox
    Friend WithEvents lblCharacter As System.Windows.Forms.Label
    Friend WithEvents gbPending As System.Windows.Forms.GroupBox
    Friend WithEvents lblPendingInfo As System.Windows.Forms.Label
    Friend WithEvents cmdRemovePending As System.Windows.Forms.Button
    Friend WithEvents lvPending As System.Windows.Forms.ListView
    Friend WithEvents gbItems As System.Windows.Forms.GroupBox
    Friend WithEvents lblSendInfo As System.Windows.Forms.Label
    Friend WithEvents cmdSend As System.Windows.Forms.Button
    Friend WithEvents tbMessage As System.Windows.Forms.TextBox
    Friend WithEvents lblMessage As System.Windows.Forms.Label
    Friend WithEvents cmdRemoveSend As System.Windows.Forms.Button
    Friend WithEvents lvSend As System.Windows.Forms.ListView
    Friend WithEvents lblToSend As System.Windows.Forms.Label
    Friend WithEvents nudQuantity As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblQuantity As System.Windows.Forms.Label
    Friend WithEvents nudSpec As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblSpec As System.Windows.Forms.Label
    Friend WithEvents cmdAdd As System.Windows.Forms.Button
    Friend WithEvents lvSearch As System.Windows.Forms.ListView
    Friend WithEvents cmdSearch As System.Windows.Forms.Button
    Friend WithEvents tbSearch As System.Windows.Forms.TextBox
    Friend WithEvents lblSearch As System.Windows.Forms.Label

End Class
