<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ItemFinder
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
        Me.cmdExport = New System.Windows.Forms.Button()
        Me.cmdProblems = New System.Windows.Forms.Button()
        Me.cmdDuplicates = New System.Windows.Forms.Button()
        Me.cmdSearch = New System.Windows.Forms.Button()
        Me.chkWarehouses = New System.Windows.Forms.CheckBox()
        Me.cbMode = New System.Windows.Forms.ComboBox()
        Me.tbQuery = New System.Windows.Forms.TextBox()
        Me.lblQuery = New System.Windows.Forms.Label()
        Me.lvResults = New System.Windows.Forms.ListView()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.dlgSave = New System.Windows.Forms.SaveFileDialog()
        Me.pnlTop.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlTop
        '
        Me.pnlTop.Controls.Add(Me.cmdExport)
        Me.pnlTop.Controls.Add(Me.cmdProblems)
        Me.pnlTop.Controls.Add(Me.cmdDuplicates)
        Me.pnlTop.Controls.Add(Me.cmdSearch)
        Me.pnlTop.Controls.Add(Me.chkWarehouses)
        Me.pnlTop.Controls.Add(Me.cbMode)
        Me.pnlTop.Controls.Add(Me.tbQuery)
        Me.pnlTop.Controls.Add(Me.lblQuery)
        Me.pnlTop.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlTop.Location = New System.Drawing.Point(0, 0)
        Me.pnlTop.Name = "pnlTop"
        Me.pnlTop.Size = New System.Drawing.Size(1034, 36)
        Me.pnlTop.TabIndex = 0
        '
        'lblQuery
        '
        Me.lblQuery.AutoSize = True
        Me.lblQuery.Location = New System.Drawing.Point(6, 11)
        Me.lblQuery.Name = "lblQuery"
        Me.lblQuery.Size = New System.Drawing.Size(43, 13)
        Me.lblQuery.TabIndex = 0
        Me.lblQuery.Text = "Search:"
        '
        'tbQuery
        '
        Me.tbQuery.Location = New System.Drawing.Point(55, 8)
        Me.tbQuery.Name = "tbQuery"
        Me.tbQuery.Size = New System.Drawing.Size(200, 20)
        Me.tbQuery.TabIndex = 1
        '
        'cbMode
        '
        Me.cbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbMode.Items.AddRange(New Object() {"by code (e.g. WA1, DA102)", "by name (part of the name)"})
        Me.cbMode.Location = New System.Drawing.Point(261, 8)
        Me.cbMode.Name = "cbMode"
        Me.cbMode.Size = New System.Drawing.Size(175, 21)
        Me.cbMode.TabIndex = 2
        '
        'chkWarehouses
        '
        Me.chkWarehouses.AutoSize = True
        Me.chkWarehouses.Checked = True
        Me.chkWarehouses.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkWarehouses.Location = New System.Drawing.Point(444, 10)
        Me.chkWarehouses.Name = "chkWarehouses"
        Me.chkWarehouses.Size = New System.Drawing.Size(120, 17)
        Me.chkWarehouses.TabIndex = 3
        Me.chkWarehouses.Text = "Include warehouses"
        Me.chkWarehouses.UseVisualStyleBackColor = True
        '
        'cmdSearch
        '
        Me.cmdSearch.Location = New System.Drawing.Point(584, 7)
        Me.cmdSearch.Name = "cmdSearch"
        Me.cmdSearch.Size = New System.Drawing.Size(80, 23)
        Me.cmdSearch.TabIndex = 4
        Me.cmdSearch.Text = "Search"
        Me.cmdSearch.UseVisualStyleBackColor = True
        '
        'cmdDuplicates
        '
        Me.cmdDuplicates.Location = New System.Drawing.Point(676, 7)
        Me.cmdDuplicates.Name = "cmdDuplicates"
        Me.cmdDuplicates.Size = New System.Drawing.Size(120, 23)
        Me.cmdDuplicates.TabIndex = 5
        Me.cmdDuplicates.Text = "Duplicated items"
        Me.cmdDuplicates.UseVisualStyleBackColor = True
        '
        'cmdProblems
        '
        Me.cmdProblems.Location = New System.Drawing.Point(802, 7)
        Me.cmdProblems.Name = "cmdProblems"
        Me.cmdProblems.Size = New System.Drawing.Size(130, 23)
        Me.cmdProblems.TabIndex = 6
        Me.cmdProblems.Text = "Items with problems"
        Me.cmdProblems.UseVisualStyleBackColor = True
        '
        'cmdExport
        '
        Me.cmdExport.Location = New System.Drawing.Point(938, 7)
        Me.cmdExport.Name = "cmdExport"
        Me.cmdExport.Size = New System.Drawing.Size(90, 23)
        Me.cmdExport.TabIndex = 7
        Me.cmdExport.Text = "Export CSV"
        Me.cmdExport.UseVisualStyleBackColor = True
        '
        'lvResults
        '
        Me.lvResults.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvResults.FullRowSelect = True
        Me.lvResults.GridLines = True
        Me.lvResults.HideSelection = False
        Me.lvResults.Location = New System.Drawing.Point(0, 36)
        Me.lvResults.Name = "lvResults"
        Me.lvResults.Size = New System.Drawing.Size(1034, 598)
        Me.lvResults.TabIndex = 1
        Me.lvResults.UseCompatibleStateImageBehavior = False
        Me.lvResults.View = System.Windows.Forms.View.Details
        '
        'lblStatus
        '
        Me.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.lblStatus.Location = New System.Drawing.Point(0, 634)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(1034, 20)
        Me.lblStatus.TabIndex = 2
        Me.lblStatus.Text = "Double-click a result to open the character (or warehouse) in the Characters tab."
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'dlgSave
        '
        Me.dlgSave.DefaultExt = "csv"
        Me.dlgSave.Filter = "CSV (*.csv)|*.csv"
        '
        'ItemFinder
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lvResults)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.pnlTop)
        Me.Name = "ItemFinder"
        Me.Size = New System.Drawing.Size(1034, 654)
        Me.pnlTop.ResumeLayout(False)
        Me.pnlTop.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlTop As System.Windows.Forms.Panel
    Friend WithEvents cmdExport As System.Windows.Forms.Button
    Friend WithEvents cmdProblems As System.Windows.Forms.Button
    Friend WithEvents cmdDuplicates As System.Windows.Forms.Button
    Friend WithEvents cmdSearch As System.Windows.Forms.Button
    Friend WithEvents chkWarehouses As System.Windows.Forms.CheckBox
    Friend WithEvents cbMode As System.Windows.Forms.ComboBox
    Friend WithEvents tbQuery As System.Windows.Forms.TextBox
    Friend WithEvents lblQuery As System.Windows.Forms.Label
    Friend WithEvents lvResults As System.Windows.Forms.ListView
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents dlgSave As System.Windows.Forms.SaveFileDialog

End Class
