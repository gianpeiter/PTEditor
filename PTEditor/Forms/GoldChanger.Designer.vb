<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class GoldChanger
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
        Me.lblWarn = New System.Windows.Forms.Label()
        Me.lblSave = New System.Windows.Forms.Label()
        Me.pbProgress = New System.Windows.Forms.ProgressBar()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cmdClose = New System.Windows.Forms.Button()
        Me.cmdStart = New System.Windows.Forms.Button()
        Me.tbGoldFormula = New System.Windows.Forms.TextBox()
        Me.SuspendLayout()
        '
        'lblWarn
        '
        Me.lblWarn.AutoSize = True
        Me.lblWarn.Location = New System.Drawing.Point(168, 139)
        Me.lblWarn.Name = "lblWarn"
        Me.lblWarn.Size = New System.Drawing.Size(64, 13)
        Me.lblWarn.TabIndex = 15
        Me.lblWarn.Text = "Warnings: 0"
        '
        'lblSave
        '
        Me.lblSave.AutoSize = True
        Me.lblSave.Location = New System.Drawing.Point(8, 142)
        Me.lblSave.Name = "lblSave"
        Me.lblSave.Size = New System.Drawing.Size(0, 13)
        Me.lblSave.TabIndex = 14
        '
        'pbProgress
        '
        Me.pbProgress.Location = New System.Drawing.Point(11, 158)
        Me.pbProgress.Name = "pbProgress"
        Me.pbProgress.Size = New System.Drawing.Size(279, 15)
        Me.pbProgress.TabIndex = 13
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(9, 90)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(159, 13)
        Me.Label2.TabIndex = 17
        Me.Label2.Text = "LVL = Level of selected monster"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(8, 77)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(158, 13)
        Me.Label1.TabIndex = 16
        Me.Label1.Text = "Gold = Gold of selected monster"
        '
        'cmdClose
        '
        Me.cmdClose.Location = New System.Drawing.Point(215, 12)
        Me.cmdClose.Name = "cmdClose"
        Me.cmdClose.Size = New System.Drawing.Size(75, 23)
        Me.cmdClose.TabIndex = 23
        Me.cmdClose.Text = "Cancel"
        Me.cmdClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText
        Me.cmdClose.UseVisualStyleBackColor = True
        '
        'cmdStart
        '
        Me.cmdStart.Location = New System.Drawing.Point(215, 85)
        Me.cmdStart.Name = "cmdStart"
        Me.cmdStart.Size = New System.Drawing.Size(75, 23)
        Me.cmdStart.TabIndex = 22
        Me.cmdStart.Text = "Start"
        Me.cmdStart.UseVisualStyleBackColor = True
        '
        'tbGoldFormula
        '
        Me.tbGoldFormula.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.PTEditor.My.MySettings.Default, "GoldFormel", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.tbGoldFormula.Location = New System.Drawing.Point(11, 116)
        Me.tbGoldFormula.Name = "tbGoldFormula"
        Me.tbGoldFormula.Size = New System.Drawing.Size(279, 20)
        Me.tbGoldFormula.TabIndex = 3
        Me.tbGoldFormula.Text = Global.PTEditor.My.MySettings.Default.GoldFormel
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(302, 182)
        '
        'Controls
        '
        Me.Controls.Add(Me.cmdClose)
        Me.Controls.Add(Me.cmdStart)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblWarn)
        Me.Controls.Add(Me.lblSave)
        Me.Controls.Add(Me.pbProgress)
        Me.Controls.Add(Me.tbGoldFormula)
        Me.Name = "GoldChanger"
        Me.Text = "Gold Changer"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents cmdClose As System.Windows.Forms.Button
    Friend WithEvents cmdStart As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblSave As System.Windows.Forms.Label
    Friend WithEvents lblWarn As System.Windows.Forms.Label
    Friend WithEvents pbProgress As System.Windows.Forms.ProgressBar
    Friend WithEvents tbGoldFormula As System.Windows.Forms.TextBox
End Class
