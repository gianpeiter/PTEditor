<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SpawnPointPanel
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
        Me.lblMesh = New System.Windows.Forms.Label()
        Me.cmdBrowse = New System.Windows.Forms.Button()
        Me.chkAdd = New System.Windows.Forms.CheckBox()
        Me.cbMap = New System.Windows.Forms.ComboBox()
        Me.lblMap = New System.Windows.Forms.Label()
        Me.cmdSave = New System.Windows.Forms.Button()
        Me.cmdReload = New System.Windows.Forms.Button()
        Me.pnlBottom = New System.Windows.Forms.Panel()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.lblHover = New System.Windows.Forms.Label()
        Me.pnlLeft = New System.Windows.Forms.Panel()
        Me.gbPoints = New System.Windows.Forms.GroupBox()
        Me.lvPoints = New System.Windows.Forms.ListView()
        Me.pnlButtons = New System.Windows.Forms.Panel()
        Me.cmdClear = New System.Windows.Forms.Button()
        Me.cmdDelete = New System.Windows.Forms.Button()
        Me.lblCount = New System.Windows.Forms.Label()
        Me.gbSelected = New System.Windows.Forms.GroupBox()
        Me.nudZ = New System.Windows.Forms.NumericUpDown()
        Me.nudX = New System.Windows.Forms.NumericUpDown()
        Me.lblZ = New System.Windows.Forms.Label()
        Me.lblX = New System.Windows.Forms.Label()
        Me.lblHelp = New System.Windows.Forms.Label()
        Me.Canvas = New PTEditor.SpawnMapCanvas()
        Me.pnlTop.SuspendLayout()
        Me.pnlBottom.SuspendLayout()
        Me.pnlLeft.SuspendLayout()
        Me.gbPoints.SuspendLayout()
        Me.pnlButtons.SuspendLayout()
        Me.gbSelected.SuspendLayout()
        CType(Me.nudZ, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudX, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pnlTop
        '
        Me.pnlTop.Controls.Add(Me.lblMesh)
        Me.pnlTop.Controls.Add(Me.cmdBrowse)
        Me.pnlTop.Controls.Add(Me.chkAdd)
        Me.pnlTop.Controls.Add(Me.cbMap)
        Me.pnlTop.Controls.Add(Me.lblMap)
        Me.pnlTop.Controls.Add(Me.cmdSave)
        Me.pnlTop.Controls.Add(Me.cmdReload)
        Me.pnlTop.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlTop.Location = New System.Drawing.Point(0, 0)
        Me.pnlTop.Name = "pnlTop"
        Me.pnlTop.Size = New System.Drawing.Size(1034, 62)
        Me.pnlTop.TabIndex = 0
        '
        'cmdReload
        '
        Me.cmdReload.Location = New System.Drawing.Point(6, 6)
        Me.cmdReload.Name = "cmdReload"
        Me.cmdReload.Size = New System.Drawing.Size(80, 23)
        Me.cmdReload.TabIndex = 0
        Me.cmdReload.Text = "Reload"
        Me.cmdReload.UseVisualStyleBackColor = True
        '
        'cmdSave
        '
        Me.cmdSave.Location = New System.Drawing.Point(92, 6)
        Me.cmdSave.Name = "cmdSave"
        Me.cmdSave.Size = New System.Drawing.Size(80, 23)
        Me.cmdSave.TabIndex = 1
        Me.cmdSave.Text = "Save"
        Me.cmdSave.UseVisualStyleBackColor = True
        '
        'lblMap
        '
        Me.lblMap.AutoSize = True
        Me.lblMap.Location = New System.Drawing.Point(190, 11)
        Me.lblMap.Name = "lblMap"
        Me.lblMap.Size = New System.Drawing.Size(31, 13)
        Me.lblMap.TabIndex = 2
        Me.lblMap.Text = "Map:"
        '
        'cbMap
        '
        Me.cbMap.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbMap.FormattingEnabled = True
        Me.cbMap.Location = New System.Drawing.Point(227, 7)
        Me.cbMap.MaxDropDownItems = 30
        Me.cbMap.Name = "cbMap"
        Me.cbMap.Size = New System.Drawing.Size(240, 21)
        Me.cbMap.TabIndex = 3
        '
        'chkAdd
        '
        Me.chkAdd.AutoSize = True
        Me.chkAdd.Checked = True
        Me.chkAdd.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkAdd.Location = New System.Drawing.Point(486, 9)
        Me.chkAdd.Name = "chkAdd"
        Me.chkAdd.Size = New System.Drawing.Size(121, 17)
        Me.chkAdd.TabIndex = 4
        Me.chkAdd.Text = "Click map to add points"
        Me.chkAdd.UseVisualStyleBackColor = True
        '
        'cmdBrowse
        '
        Me.cmdBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdBrowse.Location = New System.Drawing.Point(904, 32)
        Me.cmdBrowse.Name = "cmdBrowse"
        Me.cmdBrowse.Size = New System.Drawing.Size(124, 23)
        Me.cmdBrowse.TabIndex = 5
        Me.cmdBrowse.Text = "Browse map file..."
        Me.cmdBrowse.UseVisualStyleBackColor = True
        '
        'lblMesh
        '
        Me.lblMesh.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblMesh.AutoEllipsis = True
        Me.lblMesh.Location = New System.Drawing.Point(6, 37)
        Me.lblMesh.Name = "lblMesh"
        Me.lblMesh.Size = New System.Drawing.Size(892, 16)
        Me.lblMesh.TabIndex = 6
        '
        'pnlBottom
        '
        Me.pnlBottom.Controls.Add(Me.lblStatus)
        Me.pnlBottom.Controls.Add(Me.lblHover)
        Me.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlBottom.Location = New System.Drawing.Point(0, 630)
        Me.pnlBottom.Name = "pnlBottom"
        Me.pnlBottom.Size = New System.Drawing.Size(1034, 24)
        Me.pnlBottom.TabIndex = 2
        '
        'lblHover
        '
        Me.lblHover.Location = New System.Drawing.Point(6, 5)
        Me.lblHover.Name = "lblHover"
        Me.lblHover.Size = New System.Drawing.Size(330, 16)
        Me.lblHover.TabIndex = 0
        '
        'lblStatus
        '
        Me.lblStatus.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblStatus.AutoEllipsis = True
        Me.lblStatus.Location = New System.Drawing.Point(342, 5)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(686, 16)
        Me.lblStatus.TabIndex = 1
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'pnlLeft
        '
        Me.pnlLeft.Controls.Add(Me.gbPoints)
        Me.pnlLeft.Controls.Add(Me.gbSelected)
        Me.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left
        Me.pnlLeft.Location = New System.Drawing.Point(0, 62)
        Me.pnlLeft.Name = "pnlLeft"
        Me.pnlLeft.Padding = New System.Windows.Forms.Padding(4)
        Me.pnlLeft.Size = New System.Drawing.Size(270, 568)
        Me.pnlLeft.TabIndex = 1
        '
        'gbPoints
        '
        Me.gbPoints.Controls.Add(Me.lvPoints)
        Me.gbPoints.Controls.Add(Me.pnlButtons)
        Me.gbPoints.Controls.Add(Me.lblCount)
        Me.gbPoints.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gbPoints.Location = New System.Drawing.Point(4, 4)
        Me.gbPoints.Name = "gbPoints"
        Me.gbPoints.Padding = New System.Windows.Forms.Padding(6)
        Me.gbPoints.Size = New System.Drawing.Size(262, 420)
        Me.gbPoints.TabIndex = 0
        Me.gbPoints.TabStop = False
        Me.gbPoints.Text = "Spawn points"
        '
        'lvPoints
        '
        Me.lvPoints.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvPoints.FullRowSelect = True
        Me.lvPoints.GridLines = True
        Me.lvPoints.HideSelection = False
        Me.lvPoints.Location = New System.Drawing.Point(6, 39)
        Me.lvPoints.MultiSelect = False
        Me.lvPoints.Name = "lvPoints"
        Me.lvPoints.Size = New System.Drawing.Size(250, 343)
        Me.lvPoints.TabIndex = 1
        Me.lvPoints.UseCompatibleStateImageBehavior = False
        Me.lvPoints.View = System.Windows.Forms.View.Details
        '
        'pnlButtons
        '
        Me.pnlButtons.Controls.Add(Me.cmdClear)
        Me.pnlButtons.Controls.Add(Me.cmdDelete)
        Me.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlButtons.Location = New System.Drawing.Point(6, 382)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Size = New System.Drawing.Size(250, 32)
        Me.pnlButtons.TabIndex = 2
        '
        'cmdDelete
        '
        Me.cmdDelete.Location = New System.Drawing.Point(0, 6)
        Me.cmdDelete.Name = "cmdDelete"
        Me.cmdDelete.Size = New System.Drawing.Size(120, 23)
        Me.cmdDelete.TabIndex = 0
        Me.cmdDelete.Text = "Delete selected"
        Me.cmdDelete.UseVisualStyleBackColor = True
        '
        'cmdClear
        '
        Me.cmdClear.Location = New System.Drawing.Point(126, 6)
        Me.cmdClear.Name = "cmdClear"
        Me.cmdClear.Size = New System.Drawing.Size(120, 23)
        Me.cmdClear.TabIndex = 1
        Me.cmdClear.Text = "Clear all"
        Me.cmdClear.UseVisualStyleBackColor = True
        '
        'lblCount
        '
        Me.lblCount.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblCount.Location = New System.Drawing.Point(6, 19)
        Me.lblCount.Name = "lblCount"
        Me.lblCount.Size = New System.Drawing.Size(250, 20)
        Me.lblCount.TabIndex = 0
        Me.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'gbSelected
        '
        Me.gbSelected.Controls.Add(Me.lblHelp)
        Me.gbSelected.Controls.Add(Me.nudZ)
        Me.gbSelected.Controls.Add(Me.nudX)
        Me.gbSelected.Controls.Add(Me.lblZ)
        Me.gbSelected.Controls.Add(Me.lblX)
        Me.gbSelected.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.gbSelected.Location = New System.Drawing.Point(4, 400)
        Me.gbSelected.Name = "gbSelected"
        Me.gbSelected.Size = New System.Drawing.Size(262, 164)
        Me.gbSelected.TabIndex = 1
        Me.gbSelected.TabStop = False
        Me.gbSelected.Text = "Selected point"
        '
        'lblX
        '
        Me.lblX.AutoSize = True
        Me.lblX.Location = New System.Drawing.Point(8, 24)
        Me.lblX.Name = "lblX"
        Me.lblX.Size = New System.Drawing.Size(17, 13)
        Me.lblX.TabIndex = 0
        Me.lblX.Text = "X:"
        '
        'lblZ
        '
        Me.lblZ.AutoSize = True
        Me.lblZ.Location = New System.Drawing.Point(132, 24)
        Me.lblZ.Name = "lblZ"
        Me.lblZ.Size = New System.Drawing.Size(17, 13)
        Me.lblZ.TabIndex = 2
        Me.lblZ.Text = "Z:"
        '
        'nudX
        '
        Me.nudX.Location = New System.Drawing.Point(28, 21)
        Me.nudX.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
        Me.nudX.Minimum = New Decimal(New Integer() {2000000, 0, 0, -2147483648})
        Me.nudX.Name = "nudX"
        Me.nudX.Size = New System.Drawing.Size(92, 20)
        Me.nudX.TabIndex = 1
        '
        'nudZ
        '
        Me.nudZ.Location = New System.Drawing.Point(152, 21)
        Me.nudZ.Maximum = New Decimal(New Integer() {2000000, 0, 0, 0})
        Me.nudZ.Minimum = New Decimal(New Integer() {2000000, 0, 0, -2147483648})
        Me.nudZ.Name = "nudZ"
        Me.nudZ.Size = New System.Drawing.Size(92, 20)
        Me.nudZ.TabIndex = 3
        '
        'lblHelp
        '
        Me.lblHelp.Location = New System.Drawing.Point(8, 52)
        Me.lblHelp.Name = "lblHelp"
        Me.lblHelp.Size = New System.Drawing.Size(246, 106)
        Me.lblHelp.TabIndex = 4
        Me.lblHelp.Text = "Left click: add a point (or select/drag an existing one)." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) &
            "Right-drag: pan.   Wheel: zoom." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) &
            "Delete key: remove the selected point." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) &
            "Ctrl+Z: undo.   Ctrl+Y: redo." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) &
            "X/Z are game units (no height). Restart the server after saving."
        '
        'Canvas
        '
        Me.Canvas.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Canvas.Location = New System.Drawing.Point(270, 62)
        Me.Canvas.Name = "Canvas"
        Me.Canvas.Size = New System.Drawing.Size(764, 568)
        Me.Canvas.TabIndex = 3
        '
        'SpawnPointPanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Canvas)
        Me.Controls.Add(Me.pnlLeft)
        Me.Controls.Add(Me.pnlBottom)
        Me.Controls.Add(Me.pnlTop)
        Me.Name = "SpawnPointPanel"
        Me.Size = New System.Drawing.Size(1034, 654)
        Me.pnlTop.ResumeLayout(False)
        Me.pnlTop.PerformLayout()
        Me.pnlBottom.ResumeLayout(False)
        Me.pnlLeft.ResumeLayout(False)
        Me.gbPoints.ResumeLayout(False)
        Me.pnlButtons.ResumeLayout(False)
        Me.gbSelected.ResumeLayout(False)
        Me.gbSelected.PerformLayout()
        CType(Me.nudZ, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudX, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlTop As System.Windows.Forms.Panel
    Friend WithEvents lblMesh As System.Windows.Forms.Label
    Friend WithEvents cmdBrowse As System.Windows.Forms.Button
    Friend WithEvents chkAdd As System.Windows.Forms.CheckBox
    Friend WithEvents cbMap As System.Windows.Forms.ComboBox
    Friend WithEvents lblMap As System.Windows.Forms.Label
    Friend WithEvents cmdSave As System.Windows.Forms.Button
    Friend WithEvents cmdReload As System.Windows.Forms.Button
    Friend WithEvents pnlBottom As System.Windows.Forms.Panel
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents lblHover As System.Windows.Forms.Label
    Friend WithEvents pnlLeft As System.Windows.Forms.Panel
    Friend WithEvents gbPoints As System.Windows.Forms.GroupBox
    Friend WithEvents lvPoints As System.Windows.Forms.ListView
    Friend WithEvents pnlButtons As System.Windows.Forms.Panel
    Friend WithEvents cmdClear As System.Windows.Forms.Button
    Friend WithEvents cmdDelete As System.Windows.Forms.Button
    Friend WithEvents lblCount As System.Windows.Forms.Label
    Friend WithEvents gbSelected As System.Windows.Forms.GroupBox
    Friend WithEvents nudZ As System.Windows.Forms.NumericUpDown
    Friend WithEvents nudX As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblZ As System.Windows.Forms.Label
    Friend WithEvents lblX As System.Windows.Forms.Label
    Friend WithEvents lblHelp As System.Windows.Forms.Label
    Friend WithEvents Canvas As PTEditor.SpawnMapCanvas
End Class
