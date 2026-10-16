<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ItemSearch
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
        Me.cmdItemsFoundClear = New System.Windows.Forms.Button()
        Me.tbCommand = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cmdItemFound = New System.Windows.Forms.Button()
        Me.tbSearchText = New System.Windows.Forms.TextBox()
        Me.CMSItemFound = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.SendToShopToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LWItemsFound = New System.Windows.Forms.ListView()
        Me.CoItemCode = New System.Windows.Forms.ColumnHeader()
        Me.CoItemName = New System.Windows.Forms.ColumnHeader()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.CMSItemFound.SuspendLayout()
        Me.SuspendLayout()
        '
        'cmdItemsFoundClear
        '
        Me.cmdItemsFoundClear.Location = New System.Drawing.Point(12, 12)
        Me.cmdItemsFoundClear.Name = "cmdItemsFoundClear"
        Me.cmdItemsFoundClear.Size = New System.Drawing.Size(75, 23)
        Me.cmdItemsFoundClear.TabIndex = 2
        Me.cmdItemsFoundClear.Text = "Clear list"
        Me.cmdItemsFoundClear.UseVisualStyleBackColor = True
        '
        'tbCommand
        '
        Me.tbCommand.Location = New System.Drawing.Point(12, 314)
        Me.tbCommand.Name = "tbCommand"
        Me.tbCommand.Size = New System.Drawing.Size(242, 20)
        Me.tbCommand.TabIndex = 3
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(10, 298)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(79, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Drop command"
        '
        'cmdItemFound
        '
        Me.cmdItemFound.Location = New System.Drawing.Point(14, 55)
        Me.cmdItemFound.Name = "cmdItemFound"
        Me.cmdItemFound.Size = New System.Drawing.Size(75, 23)
        Me.cmdItemFound.TabIndex = 5
        Me.cmdItemFound.Text = "Search"
        Me.cmdItemFound.UseVisualStyleBackColor = True
        '
        'tbSearchText
        '
        Me.tbSearchText.Location = New System.Drawing.Point(93, 57)
        Me.tbSearchText.Name = "tbSearchText"
        Me.tbSearchText.Size = New System.Drawing.Size(161, 20)
        Me.tbSearchText.TabIndex = 6
        '
        'CMSItemFound
        '
        Me.CMSItemFound.Items.AddRange(New System.Windows.Forms.ToolStripItem() { Me.SendToShopToolStripMenuItem })
        Me.CMSItemFound.Name = "CMSItemFound"
        Me.CMSItemFound.Size = New System.Drawing.Size(153, 48)
        '
        'SendToShopToolStripMenuItem
        '
        Me.SendToShopToolStripMenuItem.Name = "SendToShopToolStripMenuItem"
        Me.SendToShopToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.SendToShopToolStripMenuItem.Text = "Send to shop"
        '
        'LWItemsFound
        '
        Me.LWItemsFound.Columns.AddRange(New System.Windows.Forms.ColumnHeader() { Me.CoItemCode, Me.CoItemName })
        Me.LWItemsFound.ContextMenuStrip = Me.CMSItemFound
        Me.LWItemsFound.FullRowSelect = True
        Me.LWItemsFound.Location = New System.Drawing.Point(13, 83)
        Me.LWItemsFound.Name = "LWItemsFound"
        Me.LWItemsFound.Size = New System.Drawing.Size(241, 212)
        Me.LWItemsFound.TabIndex = 73
        Me.LWItemsFound.UseCompatibleStateImageBehavior = False
        Me.LWItemsFound.View = System.Windows.Forms.View.Details
        '
        'CoItemCode
        '
        Me.CoItemCode.Text = "Code"
        '
        'CoItemName
        '
        Me.CoItemName.Text = "Name"
        Me.CoItemName.Width = 175
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(93, 38)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(92, 13)
        Me.Label2.TabIndex = 74
        Me.Label2.Text = "Use wildcards: ? *"
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(292, 346)
        '
        'Controls
        '
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.LWItemsFound)
        Me.Controls.Add(Me.tbSearchText)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cmdItemFound)
        Me.Controls.Add(Me.tbCommand)
        Me.Controls.Add(Me.cmdItemsFoundClear)
        Me.Name = "ItemSearch"
        Me.Text = "Item searcher"
        Me.CMSItemFound.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents cmdItemFound As System.Windows.Forms.Button
    Friend WithEvents cmdItemsFoundClear As System.Windows.Forms.Button
    Friend WithEvents CMSItemFound As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents CoItemCode As System.Windows.Forms.ColumnHeader
    Friend WithEvents CoItemName As System.Windows.Forms.ColumnHeader
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents LWItemsFound As System.Windows.Forms.ListView
    Friend WithEvents SendToShopToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents tbCommand As System.Windows.Forms.TextBox
    Friend WithEvents tbSearchText As System.Windows.Forms.TextBox
End Class
