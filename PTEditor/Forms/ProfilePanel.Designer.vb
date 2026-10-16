<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ProfilePanel
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
        Me.gbProfiles = New System.Windows.Forms.GroupBox()
        Me.lblInfo = New System.Windows.Forms.Label()
        Me.cmdTest = New System.Windows.Forms.Button()
        Me.cmdReload = New System.Windows.Forms.Button()
        Me.cmdOpenFolder = New System.Windows.Forms.Button()
        Me.cmdOpenFile = New System.Windows.Forms.Button()
        Me.lvProfiles = New System.Windows.Forms.ListView()
        Me.gbProfiles.SuspendLayout()
        Me.SuspendLayout()
        '
        'gbProfiles
        '
        Me.gbProfiles.Controls.Add(Me.lblInfo)
        Me.gbProfiles.Controls.Add(Me.cmdTest)
        Me.gbProfiles.Controls.Add(Me.cmdReload)
        Me.gbProfiles.Controls.Add(Me.cmdOpenFolder)
        Me.gbProfiles.Controls.Add(Me.cmdOpenFile)
        Me.gbProfiles.Controls.Add(Me.lvProfiles)
        Me.gbProfiles.Dock = System.Windows.Forms.DockStyle.Fill
        Me.gbProfiles.Location = New System.Drawing.Point(0, 0)
        Me.gbProfiles.Name = "gbProfiles"
        Me.gbProfiles.Size = New System.Drawing.Size(703, 260)
        Me.gbProfiles.TabIndex = 0
        Me.gbProfiles.TabStop = False
        Me.gbProfiles.Text = "Character profiles (file format of each server source)"
        '
        'lvProfiles
        '
        Me.lvProfiles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvProfiles.FullRowSelect = True
        Me.lvProfiles.GridLines = True
        Me.lvProfiles.HideSelection = False
        Me.lvProfiles.Location = New System.Drawing.Point(8, 20)
        Me.lvProfiles.MultiSelect = False
        Me.lvProfiles.Name = "lvProfiles"
        Me.lvProfiles.Size = New System.Drawing.Size(560, 210)
        Me.lvProfiles.TabIndex = 0
        Me.lvProfiles.UseCompatibleStateImageBehavior = False
        Me.lvProfiles.View = System.Windows.Forms.View.Details
        '
        'cmdOpenFile
        '
        Me.cmdOpenFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdOpenFile.Location = New System.Drawing.Point(576, 20)
        Me.cmdOpenFile.Name = "cmdOpenFile"
        Me.cmdOpenFile.Size = New System.Drawing.Size(119, 23)
        Me.cmdOpenFile.TabIndex = 1
        Me.cmdOpenFile.Text = "Open profile file"
        Me.cmdOpenFile.UseVisualStyleBackColor = True
        '
        'cmdOpenFolder
        '
        Me.cmdOpenFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdOpenFolder.Location = New System.Drawing.Point(576, 49)
        Me.cmdOpenFolder.Name = "cmdOpenFolder"
        Me.cmdOpenFolder.Size = New System.Drawing.Size(119, 23)
        Me.cmdOpenFolder.TabIndex = 2
        Me.cmdOpenFolder.Text = "Open Profiles folder"
        Me.cmdOpenFolder.UseVisualStyleBackColor = True
        '
        'cmdReload
        '
        Me.cmdReload.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdReload.Location = New System.Drawing.Point(576, 78)
        Me.cmdReload.Name = "cmdReload"
        Me.cmdReload.Size = New System.Drawing.Size(119, 23)
        Me.cmdReload.TabIndex = 3
        Me.cmdReload.Text = "Reload profiles"
        Me.cmdReload.UseVisualStyleBackColor = True
        '
        'cmdTest
        '
        Me.cmdTest.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdTest.Location = New System.Drawing.Point(576, 117)
        Me.cmdTest.Name = "cmdTest"
        Me.cmdTest.Size = New System.Drawing.Size(119, 23)
        Me.cmdTest.TabIndex = 4
        Me.cmdTest.Text = "Test on DataServer"
        Me.cmdTest.UseVisualStyleBackColor = True
        '
        'lblInfo
        '
        Me.lblInfo.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblInfo.AutoEllipsis = True
        Me.lblInfo.ForeColor = System.Drawing.SystemColors.GrayText
        Me.lblInfo.Location = New System.Drawing.Point(8, 236)
        Me.lblInfo.Name = "lblInfo"
        Me.lblInfo.Size = New System.Drawing.Size(687, 18)
        Me.lblInfo.TabIndex = 5
        '
        'ProfilePanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.gbProfiles)
        Me.Name = "ProfilePanel"
        Me.Size = New System.Drawing.Size(703, 260)
        Me.gbProfiles.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents gbProfiles As System.Windows.Forms.GroupBox
    Friend WithEvents lblInfo As System.Windows.Forms.Label
    Friend WithEvents cmdTest As System.Windows.Forms.Button
    Friend WithEvents cmdReload As System.Windows.Forms.Button
    Friend WithEvents cmdOpenFolder As System.Windows.Forms.Button
    Friend WithEvents cmdOpenFile As System.Windows.Forms.Button
    Friend WithEvents lvProfiles As System.Windows.Forms.ListView

End Class
