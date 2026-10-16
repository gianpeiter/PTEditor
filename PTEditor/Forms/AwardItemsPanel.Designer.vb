<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class AwardItemsPanel
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
        Me.gbRecipient = New System.Windows.Forms.GroupBox()
        Me.lblRecipientInfo = New System.Windows.Forms.Label()
        Me.cmdShowPending = New System.Windows.Forms.Button()
        Me.cbCharacter = New System.Windows.Forms.ComboBox()
        Me.lblCharacter = New System.Windows.Forms.Label()
        Me.gbPending = New System.Windows.Forms.GroupBox()
        Me.lblPendingInfo = New System.Windows.Forms.Label()
        Me.cmdRemovePending = New System.Windows.Forms.Button()
        Me.lvPending = New System.Windows.Forms.ListView()
        Me.gbItems = New System.Windows.Forms.GroupBox()
        Me.lblSendInfo = New System.Windows.Forms.Label()
        Me.cmdSend = New System.Windows.Forms.Button()
        Me.cmdRemoveSend = New System.Windows.Forms.Button()
        Me.lvSend = New System.Windows.Forms.ListView()
        Me.lblToSend = New System.Windows.Forms.Label()
        Me.cmdAdd = New System.Windows.Forms.Button()
        Me.lvSearch = New System.Windows.Forms.ListView()
        Me.cmdSearch = New System.Windows.Forms.Button()
        Me.tbSearch = New System.Windows.Forms.TextBox()
        Me.lblSearch = New System.Windows.Forms.Label()
        Me.pnlTop.SuspendLayout()
        Me.gbRecipient.SuspendLayout()
        Me.gbPending.SuspendLayout()
        Me.gbItems.SuspendLayout()
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
        'gbRecipient
        '
        Me.gbRecipient.Controls.Add(Me.lblRecipientInfo)
        Me.gbRecipient.Controls.Add(Me.cmdShowPending)
        Me.gbRecipient.Controls.Add(Me.cbCharacter)
        Me.gbRecipient.Controls.Add(Me.lblCharacter)
        Me.gbRecipient.Location = New System.Drawing.Point(6, 62)
        Me.gbRecipient.Name = "gbRecipient"
        Me.gbRecipient.Size = New System.Drawing.Size(320, 150)
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
        'cmdShowPending
        '
        Me.cmdShowPending.Location = New System.Drawing.Point(72, 47)
        Me.cmdShowPending.Name = "cmdShowPending"
        Me.cmdShowPending.Size = New System.Drawing.Size(140, 23)
        Me.cmdShowPending.TabIndex = 2
        Me.cmdShowPending.Text = "Show pending awards"
        Me.cmdShowPending.UseVisualStyleBackColor = True
        '
        'lblRecipientInfo
        '
        Me.lblRecipientInfo.Location = New System.Drawing.Point(8, 78)
        Me.lblRecipientInfo.Name = "lblRecipientInfo"
        Me.lblRecipientInfo.Size = New System.Drawing.Size(304, 66)
        Me.lblRecipientInfo.TabIndex = 3
        '
        'gbPending
        '
        Me.gbPending.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.gbPending.Controls.Add(Me.lblPendingInfo)
        Me.gbPending.Controls.Add(Me.cmdRemovePending)
        Me.gbPending.Controls.Add(Me.lvPending)
        Me.gbPending.Location = New System.Drawing.Point(6, 218)
        Me.gbPending.Name = "gbPending"
        Me.gbPending.Size = New System.Drawing.Size(320, 430)
        Me.gbPending.TabIndex = 2
        Me.gbPending.TabStop = False
        Me.gbPending.Text = "Pending awards (not claimed at the NPC yet)"
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
        Me.lvPending.Size = New System.Drawing.Size(304, 330)
        Me.lvPending.TabIndex = 0
        Me.lvPending.UseCompatibleStateImageBehavior = False
        Me.lvPending.View = System.Windows.Forms.View.Details
        '
        'cmdRemovePending
        '
        Me.cmdRemovePending.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdRemovePending.Location = New System.Drawing.Point(8, 356)
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
        Me.lblPendingInfo.Location = New System.Drawing.Point(8, 384)
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
        Me.gbItems.Controls.Add(Me.cmdRemoveSend)
        Me.gbItems.Controls.Add(Me.lvSend)
        Me.gbItems.Controls.Add(Me.lblToSend)
        Me.gbItems.Controls.Add(Me.cmdAdd)
        Me.gbItems.Controls.Add(Me.lvSearch)
        Me.gbItems.Controls.Add(Me.cmdSearch)
        Me.gbItems.Controls.Add(Me.tbSearch)
        Me.gbItems.Controls.Add(Me.lblSearch)
        Me.gbItems.Location = New System.Drawing.Point(332, 62)
        Me.gbItems.Name = "gbItems"
        Me.gbItems.Size = New System.Drawing.Size(696, 586)
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
        Me.lvSearch.Size = New System.Drawing.Size(680, 230)
        Me.lvSearch.TabIndex = 3
        Me.lvSearch.UseCompatibleStateImageBehavior = False
        Me.lvSearch.View = System.Windows.Forms.View.Details
        '
        'cmdAdd
        '
        Me.cmdAdd.Location = New System.Drawing.Point(8, 282)
        Me.cmdAdd.Name = "cmdAdd"
        Me.cmdAdd.Size = New System.Drawing.Size(130, 23)
        Me.cmdAdd.TabIndex = 4
        Me.cmdAdd.Text = "Add to list"
        Me.cmdAdd.UseVisualStyleBackColor = True
        '
        'lblToSend
        '
        Me.lblToSend.AutoSize = True
        Me.lblToSend.Location = New System.Drawing.Point(8, 316)
        Me.lblToSend.Name = "lblToSend"
        Me.lblToSend.Size = New System.Drawing.Size(48, 13)
        Me.lblToSend.TabIndex = 5
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
        Me.lvSend.Location = New System.Drawing.Point(8, 334)
        Me.lvSend.Name = "lvSend"
        Me.lvSend.Size = New System.Drawing.Size(680, 160)
        Me.lvSend.TabIndex = 6
        Me.lvSend.UseCompatibleStateImageBehavior = False
        Me.lvSend.View = System.Windows.Forms.View.Details
        '
        'cmdRemoveSend
        '
        Me.cmdRemoveSend.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cmdRemoveSend.Location = New System.Drawing.Point(8, 500)
        Me.cmdRemoveSend.Name = "cmdRemoveSend"
        Me.cmdRemoveSend.Size = New System.Drawing.Size(130, 23)
        Me.cmdRemoveSend.TabIndex = 7
        Me.cmdRemoveSend.Text = "Remove from list"
        Me.cmdRemoveSend.UseVisualStyleBackColor = True
        '
        'cmdSend
        '
        Me.cmdSend.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdSend.Location = New System.Drawing.Point(528, 500)
        Me.cmdSend.Name = "cmdSend"
        Me.cmdSend.Size = New System.Drawing.Size(160, 30)
        Me.cmdSend.TabIndex = 8
        Me.cmdSend.Text = "Send to character"
        Me.cmdSend.UseVisualStyleBackColor = True
        '
        'lblSendInfo
        '
        Me.lblSendInfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSendInfo.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblSendInfo.Location = New System.Drawing.Point(8, 534)
        Me.lblSendInfo.Name = "lblSendInfo"
        Me.lblSendInfo.Size = New System.Drawing.Size(680, 46)
        Me.lblSendInfo.TabIndex = 9
        '
        'AwardItemsPanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.gbItems)
        Me.Controls.Add(Me.gbPending)
        Me.Controls.Add(Me.gbRecipient)
        Me.Controls.Add(Me.pnlTop)
        Me.Name = "AwardItemsPanel"
        Me.Size = New System.Drawing.Size(1034, 654)
        Me.pnlTop.ResumeLayout(False)
        Me.pnlTop.PerformLayout()
        Me.gbRecipient.ResumeLayout(False)
        Me.gbRecipient.PerformLayout()
        Me.gbPending.ResumeLayout(False)
        Me.gbItems.ResumeLayout(False)
        Me.gbItems.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlTop As System.Windows.Forms.Panel
    Friend WithEvents lblDbStatus As System.Windows.Forms.Label
    Friend WithEvents cmdOptions As System.Windows.Forms.Button
    Friend WithEvents lblDbInfo As System.Windows.Forms.Label
    Friend WithEvents lblDatabase As System.Windows.Forms.Label
    Friend WithEvents gbRecipient As System.Windows.Forms.GroupBox
    Friend WithEvents lblRecipientInfo As System.Windows.Forms.Label
    Friend WithEvents cmdShowPending As System.Windows.Forms.Button
    Friend WithEvents cbCharacter As System.Windows.Forms.ComboBox
    Friend WithEvents lblCharacter As System.Windows.Forms.Label
    Friend WithEvents gbPending As System.Windows.Forms.GroupBox
    Friend WithEvents lblPendingInfo As System.Windows.Forms.Label
    Friend WithEvents cmdRemovePending As System.Windows.Forms.Button
    Friend WithEvents lvPending As System.Windows.Forms.ListView
    Friend WithEvents gbItems As System.Windows.Forms.GroupBox
    Friend WithEvents lblSendInfo As System.Windows.Forms.Label
    Friend WithEvents cmdSend As System.Windows.Forms.Button
    Friend WithEvents cmdRemoveSend As System.Windows.Forms.Button
    Friend WithEvents lvSend As System.Windows.Forms.ListView
    Friend WithEvents lblToSend As System.Windows.Forms.Label
    Friend WithEvents cmdAdd As System.Windows.Forms.Button
    Friend WithEvents lvSearch As System.Windows.Forms.ListView
    Friend WithEvents cmdSearch As System.Windows.Forms.Button
    Friend WithEvents tbSearch As System.Windows.Forms.TextBox
    Friend WithEvents lblSearch As System.Windows.Forms.Label

End Class
