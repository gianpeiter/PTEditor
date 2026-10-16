<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ItemDistributor
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.cbSpec = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lbItemNames = New System.Windows.Forms.ListBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.lbItemCodes = New System.Windows.Forms.ListBox()
        Me.CMItemDistri = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ClearListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DeleteItemToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.cmdFindAccount = New System.Windows.Forms.Button()
        Me.cbChar = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.cmdSendItems = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.tbMessage = New System.Windows.Forms.TextBox()
        Me.cbAccount = New System.Windows.Forms.ComboBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.lblAccountsFound = New System.Windows.Forms.Label()
        Me.CMItemDistri.SuspendLayout()
        Me.SuspendLayout()
        '
        'cbSpec
        '
        Me.cbSpec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbSpec.FormattingEnabled = True
        Me.cbSpec.Items.AddRange(New Object() { "0=Nothing", "1=Fighter", "2=Mechanician", "3=Archer", "4=Pikeman", "5=Atalanta", "6=Knight", "7=Magician", "8=Priestess" })
        Me.cbSpec.Location = New System.Drawing.Point(92, 184)
        Me.cbSpec.Name = "cbSpec"
        Me.cbSpec.Size = New System.Drawing.Size(185, 21)
        Me.cbSpec.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 187)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(65, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Select Spec"
        '
        'lbItemNames
        '
        Me.lbItemNames.Enabled = False
        Me.lbItemNames.FormattingEnabled = True
        Me.lbItemNames.Location = New System.Drawing.Point(100, 29)
        Me.lbItemNames.Name = "lbItemNames"
        Me.lbItemNames.Size = New System.Drawing.Size(177, 95)
        Me.lbItemNames.TabIndex = 2
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(97, 9)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(61, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Itemnames:"
        '
        'lbItemCodes
        '
        Me.lbItemCodes.ContextMenuStrip = Me.CMItemDistri
        Me.lbItemCodes.FormattingEnabled = True
        Me.lbItemCodes.Location = New System.Drawing.Point(12, 29)
        Me.lbItemCodes.Name = "lbItemCodes"
        Me.lbItemCodes.Size = New System.Drawing.Size(82, 95)
        Me.lbItemCodes.TabIndex = 4
        '
        'CMItemDistri
        '
        Me.CMItemDistri.Items.AddRange(New System.Windows.Forms.ToolStripItem() { Me.ClearListToolStripMenuItem, Me.DeleteItemToolStripMenuItem })
        Me.CMItemDistri.Name = "CMItemDistri"
        Me.CMItemDistri.Size = New System.Drawing.Size(131, 48)
        '
        'ClearListToolStripMenuItem
        '
        Me.ClearListToolStripMenuItem.Name = "ClearListToolStripMenuItem"
        Me.ClearListToolStripMenuItem.Size = New System.Drawing.Size(130, 22)
        Me.ClearListToolStripMenuItem.Text = "Clear List"
        '
        'DeleteItemToolStripMenuItem
        '
        Me.DeleteItemToolStripMenuItem.Name = "DeleteItemToolStripMenuItem"
        Me.DeleteItemToolStripMenuItem.Size = New System.Drawing.Size(130, 22)
        Me.DeleteItemToolStripMenuItem.Text = "Delete Item"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(9, 9)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(55, 13)
        Me.Label3.TabIndex = 5
        Me.Label3.Text = "ItemCode:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(12, 133)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(50, 13)
        Me.Label4.TabIndex = 7
        Me.Label4.Text = "Account:"
        '
        'cmdFindAccount
        '
        Me.cmdFindAccount.Location = New System.Drawing.Point(202, 128)
        Me.cmdFindAccount.Name = "cmdFindAccount"
        Me.cmdFindAccount.Size = New System.Drawing.Size(75, 23)
        Me.cmdFindAccount.TabIndex = 8
        Me.cmdFindAccount.Text = "Find"
        Me.cmdFindAccount.UseVisualStyleBackColor = True
        '
        'cbChar
        '
        Me.cbChar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbChar.Enabled = False
        Me.cbChar.FormattingEnabled = True
        Me.cbChar.Location = New System.Drawing.Point(92, 157)
        Me.cbChar.Name = "cbChar"
        Me.cbChar.Size = New System.Drawing.Size(185, 21)
        Me.cbChar.TabIndex = 9
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(12, 160)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(32, 13)
        Me.Label5.TabIndex = 10
        Me.Label5.Text = "Char:"
        '
        'cmdSendItems
        '
        Me.cmdSendItems.Enabled = False
        Me.cmdSendItems.Location = New System.Drawing.Point(12, 238)
        Me.cmdSendItems.Name = "cmdSendItems"
        Me.cmdSendItems.Size = New System.Drawing.Size(75, 23)
        Me.cmdSendItems.TabIndex = 11
        Me.cmdSendItems.Text = "Send"
        Me.cmdSendItems.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(12, 213)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(53, 13)
        Me.Label6.TabIndex = 12
        Me.Label6.Text = "Message:"
        '
        'tbMessage
        '
        Me.tbMessage.Location = New System.Drawing.Point(92, 210)
        Me.tbMessage.Name = "tbMessage"
        Me.tbMessage.Size = New System.Drawing.Size(185, 20)
        Me.tbMessage.TabIndex = 13
        '
        'cbAccount
        '
        Me.cbAccount.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.cbAccount.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cbAccount.FormattingEnabled = True
        Me.cbAccount.Location = New System.Drawing.Point(92, 130)
        Me.cbAccount.Name = "cbAccount"
        Me.cbAccount.Size = New System.Drawing.Size(104, 21)
        Me.cbAccount.TabIndex = 14
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(93, 243)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(85, 13)
        Me.Label7.TabIndex = 15
        Me.Label7.Text = "Accounts found:"
        '
        'lblAccountsFound
        '
        Me.lblAccountsFound.AutoSize = True
        Me.lblAccountsFound.Location = New System.Drawing.Point(183, 243)
        Me.lblAccountsFound.Name = "lblAccountsFound"
        Me.lblAccountsFound.Size = New System.Drawing.Size(13, 13)
        Me.lblAccountsFound.TabIndex = 16
        Me.lblAccountsFound.Text = "0"
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(284, 268)
        '
        'Controls
        '
        Me.Controls.Add(Me.lblAccountsFound)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.cbAccount)
        Me.Controls.Add(Me.tbMessage)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.cmdSendItems)
        Me.Controls.Add(Me.cbChar)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.cmdFindAccount)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.lbItemCodes)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.lbItemNames)
        Me.Controls.Add(Me.cbSpec)
        Me.Controls.Add(Me.Label1)
        Me.MaximumSize = New System.Drawing.Size(300, 304)
        Me.Name = "ItemDistributor"
        Me.Text = "ItemDistributor"
        Me.CMItemDistri.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ClearListToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents cmdFindAccount As System.Windows.Forms.Button
    Friend WithEvents cmdSendItems As System.Windows.Forms.Button
    Friend WithEvents CMItemDistri As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents cbSpec As System.Windows.Forms.ComboBox
    Friend WithEvents cbChar As System.Windows.Forms.ComboBox
    Friend WithEvents cbAccount As System.Windows.Forms.ComboBox
    Friend WithEvents DeleteItemToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents lblAccountsFound As System.Windows.Forms.Label
    Friend WithEvents lbItemNames As System.Windows.Forms.ListBox
    Friend WithEvents lbItemCodes As System.Windows.Forms.ListBox
    Friend WithEvents tbMessage As System.Windows.Forms.TextBox
End Class
