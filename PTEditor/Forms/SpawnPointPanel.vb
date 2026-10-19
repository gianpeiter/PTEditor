Imports System.IO

''' <summary>
''' Aba Spawn Point: edita os pontos de nascimento (GameServer\Field\*.spp) sobre a vista de cima do mapa (Field\*.smd).
''' Mesma ideia da ferramenta de GM do jogo (adicionar/remover "flag" numa posicao); o .spm define quais monstros nascem.
''' </summary>
Public Class SpawnPointPanel

    Private _setupDone As Boolean
    Private _updating As Boolean
    Private _fromCanvas As Boolean
    Private _file As SpawnPointFile
    Private _mesh As StageMesh
    Private _currentName As String = ""
    Private _serverPath As String = ""

    ''' <summary>Arquivos .smd escolhidos a mao (Browse) para mapas cujo .smd nao esta na pasta do servidor; vale ate fechar o programa.</summary>
    Private ReadOnly _manualMesh As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

    Private Sub EnsureSetup()
        If _setupDone Then Return
        _setupDone = True
        lvPoints.Columns.Add("#", 40)
        lvPoints.Columns.Add("X", 85)
        lvPoints.Columns.Add("Z", 85)
    End Sub

    ''' <summary>Chamado ao abrir a aba: carrega a lista de mapas (e o mapa escolhido, na primeira vez).</summary>
    Friend Sub EnsureLoaded()
        EnsureSetup()
        If AppState.ServerPath = "" Then
            SetStatus("Server folder not set - set it in Options.", Color.Firebrick)
            Return
        End If
        If _serverPath <> AppState.ServerPath Then
            _serverPath = AppState.ServerPath
            _currentName = ""
            FillMaps()
            If cbMap.Items.Count > 0 Then cbMap.SelectedIndex = 0
        End If
    End Sub

#Region "Lista de mapas e carregamento"

    ''' <summary>Mapas = todo .spp existente mais o .spp esperado de cada .spm que ainda nao tem (criado ao salvar).</summary>
    Private Sub FillMaps()
        Dim names As New SortedDictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        For Each f In FileHelper.ListFiles(AppState.ServerPath & AppState.FieldFolder, AppState.SppPattern)
            names(f) = f
        Next
        For Each map In AppState.Maps
            Dim spp As String = SpawnPointFile.NameForMap(map.FileName)
            If Not names.ContainsKey(spp) Then names(spp) = spp
        Next
        _updating = True
        cbMap.BeginUpdate()
        cbMap.Items.Clear()
        For Each sppName In names.Values
            cbMap.Items.Add(sppName)
        Next
        cbMap.EndUpdate()
        _updating = False
    End Sub

    Private Sub cbMap_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cbMap.SelectedIndexChanged
        If _updating OrElse cbMap.SelectedItem Is Nothing Then Return
        Dim name As String = cbMap.SelectedItem.ToString()
        If name = _currentName Then Return
        If IsDirty AndAlso Not Dialogs.Confirm("Spawn Point", "Discard the unsaved changes to " & _currentName & "?", MsgBoxStyle.Exclamation) Then
            _updating = True
            cbMap.SelectedItem = _currentName
            _updating = False
            Return
        End If
        LoadMap(name)
    End Sub

    Private Sub LoadMap(ByVal name As String)
        Cursor = Cursors.WaitCursor
        Try
            _file = New SpawnPointFile(name)
            _currentName = name
            RefreshPoints(-1)
            LoadMesh()
            If _file.Exists Then
                SetStatus("Loaded " & name & ".", SystemColors.ControlText)
            Else
                SetStatus(name & " does not exist yet: it is created on Save.", Color.DarkOrange)
            End If
        Catch ex As Exception
            _file = Nothing
            _currentName = ""
            lvPoints.Items.Clear()
            Canvas.SetPoints(New List(Of SpawnMapCanvas.CanvasPoint)())
            Canvas.SetMesh(Nothing)
            lblCount.Text = ""
            SetStatus("Could not load " & name & ": " & ex.Message, Color.Firebrick)
            ErrorLog.Write("Spawn Point: load " & name & vbCrLf & ex.ToString())
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>Carrega o .smd do mapa atual (achado na pasta do servidor ou escolhido no Browse).</summary>
    Private Sub LoadMesh()
        Dim path As String = ""
        If Not _manualMesh.TryGetValue(_currentName, path) Then path = StageMesh.FindFile(_currentName)
        _mesh = Nothing
        If path = "" Then
            lblMesh.Text = "Map file (.smd) not found in " & AppState.ServerPath & AppState.MeshFolder & " - use Browse to pick it. Points can still be edited without it."
            lblMesh.ForeColor = Color.DarkOrange
        Else
            Try
                _mesh = New StageMesh(path)
                lblMesh.Text = "Map: " & path & "   (" & _mesh.VertexCount & " vertices, " & _mesh.TriangleCount & " triangles)"
                lblMesh.ForeColor = SystemColors.ControlText
            Catch ex As Exception
                lblMesh.Text = "Could not read " & path & ": " & ex.Message
                lblMesh.ForeColor = Color.Firebrick
                ErrorLog.Write("Spawn Point: mesh " & path & vbCrLf & ex.ToString())
            End Try
        End If
        Canvas.SetMesh(_mesh)
    End Sub

    Private Sub cmdBrowse_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdBrowse.Click
        If _file Is Nothing Then Return
        Using dialog As New OpenFileDialog()
            dialog.Title = "Map file for " & _currentName
            dialog.Filter = "Map files (*.smd)|*.smd"
            Dim folder As String = AppState.ServerPath & AppState.MeshFolder
            If Directory.Exists(folder) Then dialog.InitialDirectory = folder
            If dialog.ShowDialog(Me) <> DialogResult.OK Then Return
            _manualMesh(_currentName) = dialog.FileName
        End Using
        Cursor = Cursors.WaitCursor
        Try
            LoadMesh()
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub cmdReload_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdReload.Click
        If AppState.ServerPath = "" Then
            SetStatus("Server folder not set - set it in Options.", Color.Firebrick)
            Return
        End If
        If IsDirty AndAlso Not Dialogs.Confirm("Spawn Point", "Discard the unsaved changes to " & _currentName & "?", MsgBoxStyle.Exclamation) Then Return
        Dim name As String = _currentName
        _serverPath = AppState.ServerPath
        FillMaps()
        _updating = True
        If name <> "" AndAlso cbMap.Items.Contains(name) Then
            cbMap.SelectedItem = name
        ElseIf cbMap.Items.Count > 0 Then
            cbMap.SelectedIndex = 0
            name = cbMap.SelectedItem.ToString()
        End If
        _updating = False
        _currentName = ""
        If cbMap.SelectedItem IsNot Nothing Then LoadMap(cbMap.SelectedItem.ToString())
    End Sub

#End Region

#Region "Pontos"

    ''' <summary>Refaz a lista e o mapa a partir do arquivo em memoria; selectSlot = ponto a manter selecionado (-1 = nenhum).</summary>
    Private Sub RefreshPoints(ByVal selectSlot As Integer)
        If _file Is Nothing Then Return
        _updating = True
        lvPoints.BeginUpdate()
        lvPoints.Items.Clear()
        Dim canvasPoints As New List(Of SpawnMapCanvas.CanvasPoint)()
        For Each slot In _file.ActiveSlots()
            Dim x As Integer = _file.GetX(slot), z As Integer = _file.GetZ(slot)
            Dim item As New ListViewItem(slot.ToString())
            item.SubItems.Add(x.ToString())
            item.SubItems.Add(z.ToString())
            item.Tag = slot
            lvPoints.Items.Add(item)
            If slot = selectSlot Then item.Selected = True
            canvasPoints.Add(New SpawnMapCanvas.CanvasPoint() With {.Slot = slot, .X = x, .Z = z})
        Next
        lvPoints.EndUpdate()
        Canvas.SetPoints(canvasPoints)
        Canvas.SelectedSlot = selectSlot
        _updating = False
        UpdateSelectedBox()
        UpdateCount()
    End Sub

    Private Sub UpdateCount()
        Dim count As Integer = If(_file Is Nothing, 0, _file.ActiveCount)
        lblCount.Text = count & " / " & SpawnPointFile.MaxPoints & " points" & If(IsDirty, "   (unsaved changes)", "")
        lblCount.ForeColor = If(count >= SpawnPointFile.MaxPoints, Color.Firebrick, If(IsDirty, Color.DarkOrange, SystemColors.ControlText))
        cmdSave.Text = If(IsDirty, "Save *", "Save")
    End Sub

    Private Function SelectedSlot() As Integer
        If lvPoints.SelectedItems.Count = 0 Then Return -1
        Return CInt(lvPoints.SelectedItems(0).Tag)
    End Function

    Private Sub UpdateSelectedBox()
        Dim slot As Integer = SelectedSlot()
        _updating = True
        If slot >= 0 AndAlso _file IsNot Nothing Then
            nudX.Value = _file.GetX(slot)
            nudZ.Value = _file.GetZ(slot)
            gbSelected.Text = "Selected point #" & slot
        Else
            nudX.Value = 0
            nudZ.Value = 0
            gbSelected.Text = "Selected point"
        End If
        nudX.Enabled = slot >= 0
        nudZ.Enabled = slot >= 0
        cmdDelete.Enabled = slot >= 0
        _updating = False
    End Sub

    ''' <summary>Ha alteracoes ainda nao gravadas (desfazer ate o estado salvo zera isso).</summary>
    Private ReadOnly Property IsDirty As Boolean
        Get
            Return _file IsNot Nothing AndAlso _file.IsModified
        End Get
    End Property

    Private Sub MarkDirty()
        UpdateCount()
    End Sub

    Private Sub UndoChange()
        If _file Is Nothing OrElse Not _file.CanUndo Then Return
        Dim keep As Integer = SelectedSlot()
        Dim what As String = _file.Undo()
        AfterUndoRedo("Undid: " & what, keep)
    End Sub

    Private Sub RedoChange()
        If _file Is Nothing OrElse Not _file.CanRedo Then Return
        Dim keep As Integer = SelectedSlot()
        Dim what As String = _file.Redo()
        AfterUndoRedo("Redid: " & what, keep)
    End Sub

    Private Sub AfterUndoRedo(ByVal message As String, ByVal keepSlot As Integer)
        RefreshPoints(If(keepSlot >= 0 AndAlso _file.IsActive(keepSlot), keepSlot, -1))
        SetStatus(message & ".", SystemColors.ControlText)
    End Sub

    ''' <summary>Ctrl+Z desfaz; Ctrl+Y ou Ctrl+Shift+Z refaz. Nos campos X/Z o Ctrl+Z continua sendo o do proprio campo.</summary>
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean
        If Not TypeOf ActiveControl Is NumericUpDown Then
            Select Case keyData
                Case Keys.Control Or Keys.Z
                    UndoChange()
                    Return True
                Case Keys.Control Or Keys.Y, Keys.Control Or Keys.Shift Or Keys.Z
                    RedoChange()
                    Return True
            End Select
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub lvPoints_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles lvPoints.SelectedIndexChanged
        If _updating Then Return
        Dim slot As Integer = SelectedSlot()
        Canvas.SelectedSlot = slot
        UpdateSelectedBox()
        If slot >= 0 AndAlso Not _fromCanvas AndAlso _file IsNot Nothing Then Canvas.CenterOn(_file.GetX(slot), _file.GetZ(slot))
    End Sub

    Private Sub Canvas_PointSelected(ByVal slot As Integer) Handles Canvas.PointSelected
        _fromCanvas = True
        Try
            For Each item As ListViewItem In lvPoints.Items
                If CInt(item.Tag) = slot Then
                    item.Selected = True
                    item.EnsureVisible()
                    Exit For
                End If
            Next
        Finally
            _fromCanvas = False
        End Try
    End Sub

    Private Sub Canvas_AddRequested(ByVal x As Integer, ByVal z As Integer) Handles Canvas.AddRequested
        If _file Is Nothing Then Return
        If _file.ActiveCount >= SpawnPointFile.MaxPoints Then
            SetStatus("This map already has the maximum of " & SpawnPointFile.MaxPoints & " spawn points.", Color.Firebrick)
            Return
        End If
        Dim slot As Integer = _file.Add(x, z)
        If slot < 0 Then
            SetStatus("There is already a spawn point at X " & x & ", Z " & z & ".", Color.Firebrick)
            Return
        End If
        MarkDirty()
        RefreshPoints(slot)
        SetStatus("Added point #" & slot & " at X " & x & ", Z " & z & ".", SystemColors.ControlText)
    End Sub

    Private Sub Canvas_PointMoved(ByVal slot As Integer, ByVal x As Integer, ByVal z As Integer) Handles Canvas.PointMoved
        If _file Is Nothing Then Return
        If _file.Move(slot, x, z) Then
            MarkDirty()
            SetStatus("Moved point #" & slot & " to X " & x & ", Z " & z & ".", SystemColors.ControlText)
        Else
            SetStatus("There is already a spawn point at X " & x & ", Z " & z & ".", Color.Firebrick)
        End If
        RefreshPoints(slot)
    End Sub

    Private Sub nudXZ_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) Handles nudX.ValueChanged, nudZ.ValueChanged
        If _updating OrElse _file Is Nothing Then Return
        Dim slot As Integer = SelectedSlot()
        If slot < 0 Then Return
        If _file.Move(slot, CInt(nudX.Value), CInt(nudZ.Value)) Then
            MarkDirty()
        Else
            SetStatus("There is already a spawn point at that position.", Color.Firebrick)
        End If
        RefreshPoints(slot)
        Canvas.CenterOn(_file.GetX(slot), _file.GetZ(slot))
    End Sub

    Private Sub cmdDelete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDelete.Click
        RemoveSelected()
    End Sub

    Private Sub Canvas_DeleteRequested() Handles Canvas.DeleteRequested
        RemoveSelected()
    End Sub

    Private Sub RemoveSelected()
        Dim slot As Integer = SelectedSlot()
        If _file Is Nothing OrElse slot < 0 Then Return
        _file.Remove(slot)
        MarkDirty()
        RefreshPoints(-1)
        SetStatus("Removed point #" & slot & ".", SystemColors.ControlText)
    End Sub

    Private Sub cmdClear_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdClear.Click
        If _file Is Nothing OrElse _file.ActiveCount = 0 Then Return
        If Not Dialogs.Confirm("Spawn Point", "Remove all " & _file.ActiveCount & " spawn points of " & _currentName & "?" & vbCrLf &
                               "Nothing is written until you click Save.", MsgBoxStyle.Exclamation) Then Return
        _file.Clear()
        MarkDirty()
        RefreshPoints(-1)
        SetStatus("Removed all points (not saved yet).", SystemColors.ControlText)
    End Sub

    Private Sub chkAdd_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkAdd.CheckedChanged
        Canvas.AddOnClick = chkAdd.Checked
    End Sub

    Private Sub Canvas_HoverChanged(ByVal x As Integer, ByVal z As Integer, ByVal hasHeight As Boolean, ByVal height As Single) Handles Canvas.HoverChanged
        lblHover.Text = "X: " & x & "   Z: " & z & If(hasHeight, "   Height: " & CInt(height), If(Canvas.HasMesh, "   (no terrain)", ""))
    End Sub

#End Region

#Region "Salvar"

    Private Sub cmdSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSave.Click
        If _file Is Nothing Then Return
        Cursor = Cursors.WaitCursor
        Try
            _file.Save()
            UpdateCount()
            SetStatus("Saved " & _currentName & " (" & _file.ActiveCount & " points). Restart the server to apply.", Color.ForestGreen)
        Catch ex As Exception
            SetStatus("Save failed: " & ex.Message, Color.Firebrick)
            ErrorLog.Write("Spawn Point: save " & _currentName & vbCrLf & ex.ToString())
            MsgBox("Save failed:" & vbCrLf & ex.Message, MsgBoxStyle.Exclamation, "Spawn Point")
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

#End Region

    Private Sub SetStatus(ByVal text As String, ByVal color As Color)
        lblStatus.Text = text
        lblStatus.ForeColor = color
    End Sub

End Class
