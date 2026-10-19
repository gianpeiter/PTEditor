Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

''' <summary>
''' Vista de cima de um mapa (.smd) com pontos por cima (spawn points do .spp e NPCs do .spc). O terreno e rasterizado uma vez (so as faces
''' viradas para cima, o mais alto vence, colorido por altura e sombreado); zoom e arraste so mudam a transformacao.
''' Norte (Z maior) fica em cima, como no minimapa do jogo. Sem malha, mostra so os pontos.
''' </summary>
Friend Class SpawnMapCanvas
    Inherits Control

    ''' <summary>Ponto exibido: slot do arquivo e posicao em unidades de mundo.</summary>
    Friend Class CanvasPoint
        Friend Slot As Integer
        Friend X As Integer
        Friend Z As Integer
        ''' <summary>Direcao em 0..4096 (NPC): desenha uma seta. -1 = sem direcao.</summary>
        Friend Angle As Integer = -1
        ''' <summary>Texto ao lado do marcador (nome do NPC).</summary>
        Friend Label As String = ""
    End Class

    ''' <summary>Angulo do NPC (0..4096) para radianos: 2*PI/4096.</summary>
    Private Const AngleToRadians As Double = 0.0015339807878856412

    ''' <summary>Usuario clicou num ponto (selecao).</summary>
    Friend Event PointSelected(ByVal slot As Integer)
    ''' <summary>Clique em area vazia com "click to add" ligado.</summary>
    Friend Event AddRequested(ByVal x As Integer, ByVal z As Integer)
    ''' <summary>Ponto arrastado e solto numa nova posicao.</summary>
    Friend Event PointMoved(ByVal slot As Integer, ByVal x As Integer, ByVal z As Integer)
    ''' <summary>Tecla Delete com o foco no mapa.</summary>
    Friend Event DeleteRequested()
    ''' <summary>Ctrl+clique num ponto (com ReplaceOnCtrlClick ligado): pedido para trocar o NPC daquele slot.</summary>
    Friend Event ReplaceRequested(ByVal slot As Integer)
    ''' <summary>PageUp (+1) / PageDown (-1) com o foco no mapa: girar o ponto selecionado.</summary>
    Friend Event RotateRequested(ByVal direction As Integer)
    ''' <summary>Cursor sobre o mapa: posicao de mundo e altura do terreno (hasHeight False se nao ha terreno ali).</summary>
    Friend Event HoverChanged(ByVal x As Integer, ByVal z As Integer, ByVal hasHeight As Boolean, ByVal height As Single)

    Private Const RasterSize As Integer = 1600
    Private Const HitRadius As Integer = 9
    Private Const MaxScale As Double = 60.0
    Private Const DragThreshold As Integer = 4

    Private ReadOnly _noTerrain As Color = Color.FromArgb(38, 40, 46)

    Private _mesh As StageMesh
    Private _raster As Bitmap
    Private _heights As Single()
    Private _rasterW As Integer, _rasterH As Integer
    Private _cell As Single

    Private _scale As Double = 1.0
    Private _centerX As Double, _centerZ As Double

    Private _points As New List(Of CanvasPoint)()
    Private _selectedSlot As Integer = -1

    Private _backdrop As Bitmap
    Private _backdropMinX As Double, _backdropMaxX As Double, _backdropMinZ As Double, _backdropMaxZ As Double

    Private _panning As Boolean
    Private _panLast As Point
    Private _panStart As Point
    Private _panMoved As Boolean
    Private _dragPoint As CanvasPoint
    Private _pressed As Boolean
    Private _pressPos As Point
    Private _moved As Boolean

    ''' <summary>Se True, clicar em area vazia pede para adicionar um ponto.</summary>
    Friend Property AddOnClick As Boolean = True

    ''' <summary>Se True, Ctrl+clique num ponto dispara ReplaceRequested em vez de selecionar/arrastar.</summary>
    Friend Property ReplaceOnCtrlClick As Boolean

    ''' <summary>Se True, desenha o Label de todos os pontos (o do selecionado aparece sempre).</summary>
    Friend Property ShowLabels As Boolean
        Get
            Return _showLabels
        End Get
        Set(ByVal value As Boolean)
            _showLabels = value
            Invalidate()
        End Set
    End Property
    Private _showLabels As Boolean

    ''' <summary>Menu mostrado no clique direito sem arrastar (arrastar com o direito move a vista).</summary>
    Friend Property ClickMenu As ContextMenuStrip

    ''' <summary>Centro da vista, em unidades de mundo.</summary>
    Friend ReadOnly Property ViewCenterX As Integer
        Get
            Return CInt(Math.Round(_centerX))
        End Get
    End Property

    Friend ReadOnly Property ViewCenterZ As Integer
        Get
            Return CInt(Math.Round(_centerZ))
        End Get
    End Property

    Friend Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or ControlStyles.OptimizedDoubleBuffer Or
                 ControlStyles.ResizeRedraw Or ControlStyles.Selectable, True)
        TabStop = True
        BackColor = _noTerrain
    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso _raster IsNot Nothing Then _raster.Dispose()
        MyBase.Dispose(disposing)
    End Sub

#Region "Dados"

    Friend ReadOnly Property HasMesh As Boolean
        Get
            Return _mesh IsNot Nothing
        End Get
    End Property

    ''' <summary>Define a malha (ou Nothing) e enquadra a vista.</summary>
    Friend Sub SetMesh(ByVal mesh As StageMesh)
        _mesh = mesh
        If _raster IsNot Nothing Then _raster.Dispose()
        _raster = Nothing
        _heights = Nothing
        If mesh IsNot Nothing Then BuildRaster()
        FitView()
    End Sub

    Friend Sub SetPoints(ByVal points As IEnumerable(Of CanvasPoint))
        _points = New List(Of CanvasPoint)(points)
        Invalidate()
    End Sub

    Friend Property SelectedSlot As Integer
        Get
            Return _selectedSlot
        End Get
        Set(ByVal value As Integer)
            _selectedSlot = value
            Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Imagem (minimapa) usada no lugar do terreno quando nao ha malha; cobre o retangulo (minX..maxX, minZ..maxZ).
    ''' O bitmap pertence a quem chamou. Nothing remove a imagem.
    ''' </summary>
    Friend Sub SetBackdrop(ByVal image As Bitmap, ByVal minX As Double, ByVal maxX As Double, ByVal minZ As Double, ByVal maxZ As Double)
        _backdrop = image
        _backdropMinX = minX : _backdropMaxX = maxX : _backdropMinZ = minZ : _backdropMaxZ = maxZ
        FitView()
    End Sub

    ''' <summary>Atualiza posicao e direcao de um ponto ja exibido (edicao ainda nao gravada).</summary>
    Friend Sub UpdatePoint(ByVal slot As Integer, ByVal x As Integer, ByVal z As Integer, ByVal angle As Integer)
        Dim p = _points.FirstOrDefault(Function(q) q.Slot = slot)
        If p IsNot Nothing Then
            p.X = x : p.Z = z : p.Angle = angle
        End If
        _selectedSlot = slot
        Invalidate()
    End Sub

    ''' <summary>Centraliza a vista no ponto sem mudar o zoom.</summary>
    Friend Sub CenterOn(ByVal x As Integer, ByVal z As Integer)
        _centerX = x
        _centerZ = z
        Invalidate()
    End Sub

#End Region

#Region "Terreno"

    Private Sub BuildRaster()
        Dim mesh = _mesh
        Dim rangeX As Single = Math.Max(mesh.MaxX - mesh.MinX, 1.0F)
        Dim rangeZ As Single = Math.Max(mesh.MaxZ - mesh.MinZ, 1.0F)
        _cell = Math.Max(rangeX, rangeZ) / RasterSize
        _rasterW = CInt(Math.Ceiling(rangeX / _cell)) + 1
        _rasterH = CInt(Math.Ceiling(rangeZ / _cell)) + 1

        Dim heights(_rasterW * _rasterH - 1) As Single
        Dim shade(_rasterW * _rasterH - 1) As Single
        For i = 0 To heights.Length - 1
            heights(i) = Single.NegativeInfinity
        Next

        Dim tris = mesh.Triangles
        For t = 0 To tris.Length - 3 Step 3
            RasterizeTriangle(mesh, tris(t), tris(t + 1), tris(t + 2), heights, shade)
        Next

        Dim minH As Single = Single.MaxValue, maxH As Single = Single.MinValue
        For Each h In heights
            If Single.IsNegativeInfinity(h) Then Continue For
            If h < minH Then minH = h
            If h > maxH Then maxH = h
        Next
        If minH > maxH Then
            minH = 0 : maxH = 1
        ElseIf maxH - minH < 1.0F Then
            maxH = minH + 1.0F
        End If

        Dim pixels(heights.Length - 1) As Integer
        For i = 0 To heights.Length - 1
            If Single.IsNegativeInfinity(heights(i)) Then
                pixels(i) = _noTerrain.ToArgb()
            Else
                pixels(i) = HeightColor((heights(i) - minH) / (maxH - minH), shade(i)).ToArgb()
            End If
        Next

        Dim bmp As New Bitmap(_rasterW, _rasterH, PixelFormat.Format32bppArgb)
        Dim bits = bmp.LockBits(New Rectangle(0, 0, _rasterW, _rasterH), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb)
        Marshal.Copy(pixels, 0, bits.Scan0, pixels.Length)
        bmp.UnlockBits(bits)
        _raster = bmp
        _heights = heights
    End Sub

    Private Sub RasterizeTriangle(ByVal mesh As StageMesh, ByVal ia As Integer, ByVal ib As Integer, ByVal ic As Integer,
                                  ByVal heights As Single(), ByVal shade As Single())
        Dim ax = mesh.X(ia), ay = mesh.Y(ia), az = mesh.Z(ia)
        Dim ux = mesh.X(ib) - ax, uy = mesh.Y(ib) - ay, uz = mesh.Z(ib) - az
        Dim vx = mesh.X(ic) - ax, vy = mesh.Y(ic) - ay, vz = mesh.Z(ic) - az

        ' Normal da face (u x v). O jogo enrola as faces de modo que a normal "para cima" e o oposto desse produto.
        Dim nx = uy * vz - uz * vy
        Dim ny = uz * vx - ux * vz
        Dim nz = ux * vy - uy * vx
        Dim length As Single = CSng(Math.Sqrt(nx * nx + ny * ny + nz * nz))
        If length < 0.000001F Then Return
        Dim upY As Single = -ny / length
        If upY < 0.05F Then Return
        Dim light As Single = (0.35F * nx / length + 0.8F * upY - 0.5F * nz / length) * 0.99F
        Dim sh As Single = 0.45F + 0.55F * Math.Max(0.0F, Math.Min(1.0F, light))

        Dim x0 = (ax - mesh.MinX) / _cell, y0 = (mesh.MaxZ - az) / _cell
        Dim x1 = (mesh.X(ib) - mesh.MinX) / _cell, y1 = (mesh.MaxZ - mesh.Z(ib)) / _cell
        Dim x2 = (mesh.X(ic) - mesh.MinX) / _cell, y2 = (mesh.MaxZ - mesh.Z(ic)) / _cell
        Dim h0 = ay, h1 = mesh.Y(ib), h2 = mesh.Y(ic)

        Dim denom As Single = (y1 - y2) * (x0 - x2) + (x2 - x1) * (y0 - y2)
        Dim startX As Integer = Math.Max(0, CInt(Math.Floor(Math.Min(x0, Math.Min(x1, x2)))))
        Dim endX As Integer = Math.Min(_rasterW - 1, CInt(Math.Ceiling(Math.Max(x0, Math.Max(x1, x2)))))
        Dim startY As Integer = Math.Max(0, CInt(Math.Floor(Math.Min(y0, Math.Min(y1, y2)))))
        Dim endY As Integer = Math.Min(_rasterH - 1, CInt(Math.Ceiling(Math.Max(y0, Math.Max(y1, y2)))))

        Dim wrote As Boolean
        If Math.Abs(denom) > 0.000000001F Then
            For py = startY To endY
                For px = startX To endX
                    Dim cx As Single = px + 0.5F, cy As Single = py + 0.5F
                    Dim w0 As Single = ((y1 - y2) * (cx - x2) + (x2 - x1) * (cy - y2)) / denom
                    Dim w1 As Single = ((y2 - y0) * (cx - x2) + (x0 - x2) * (cy - y2)) / denom
                    Dim w2 As Single = 1.0F - w0 - w1
                    If w0 < -0.0001F OrElse w1 < -0.0001F OrElse w2 < -0.0001F Then Continue For
                    PlotPixel(heights, shade, px, py, w0 * h0 + w1 * h1 + w2 * h2, sh)
                    wrote = True
                Next
            Next
        End If

        ' Triangulo menor que um pixel: marca o pixel do centro para nao abrir buracos no terreno.
        If Not wrote Then
            Dim mx As Integer = CInt(Math.Floor((x0 + x1 + x2) / 3.0F))
            Dim my As Integer = CInt(Math.Floor((y0 + y1 + y2) / 3.0F))
            If mx >= 0 AndAlso my >= 0 AndAlso mx < _rasterW AndAlso my < _rasterH Then
                PlotPixel(heights, shade, mx, my, (h0 + h1 + h2) / 3.0F, sh)
            End If
        End If
    End Sub

    Private Sub PlotPixel(ByVal heights As Single(), ByVal shade As Single(), ByVal px As Integer, ByVal py As Integer,
                          ByVal height As Single, ByVal sh As Single)
        Dim index As Integer = py * _rasterW + px
        If height > heights(index) Then
            heights(index) = height
            shade(index) = sh
        End If
    End Sub

    ''' <summary>Gradiente de altura (azul-esverdeado, verde, areia, claro) multiplicado pelo sombreado.</summary>
    Private Shared Function HeightColor(ByVal t As Single, ByVal shade As Single) As Color
        Static stops As Color() = {Color.FromArgb(40, 70, 100), Color.FromArgb(70, 120, 80), Color.FromArgb(190, 170, 110), Color.FromArgb(240, 240, 240)}
        t = Math.Max(0.0F, Math.Min(1.0F, t)) * (stops.Length - 1)
        Dim i As Integer = Math.Min(CInt(Math.Floor(t)), stops.Length - 2)
        Dim f As Single = t - i
        ' Color.R/G/B sao Byte: converte antes de subtrair (Byte - Byte estoura quando o resultado e negativo).
        Dim a As Color = stops(i), c As Color = stops(i + 1)
        Dim r As Integer = CInt((CInt(a.R) + (CInt(c.R) - CInt(a.R)) * f) * shade)
        Dim g As Integer = CInt((CInt(a.G) + (CInt(c.G) - CInt(a.G)) * f) * shade)
        Dim b As Integer = CInt((CInt(a.B) + (CInt(c.B) - CInt(a.B)) * f) * shade)
        Return Color.FromArgb(Math.Min(255, r), Math.Min(255, g), Math.Min(255, b))
    End Function

    ''' <summary>Altura do terreno no ponto de mundo, se ha terreno ali.</summary>
    Private Function HeightAt(ByVal x As Double, ByVal z As Double, ByRef height As Single) As Boolean
        If _heights Is Nothing Then Return False
        Dim px As Integer = CInt(Math.Floor((x - _mesh.MinX) / _cell))
        Dim py As Integer = CInt(Math.Floor((_mesh.MaxZ - z) / _cell))
        If px < 0 OrElse py < 0 OrElse px >= _rasterW OrElse py >= _rasterH Then Return False
        height = _heights(py * _rasterW + px)
        Return Not Single.IsNegativeInfinity(height)
    End Function

#End Region

#Region "Vista"

    Private Function ScreenX(ByVal worldX As Double) As Single
        Return CSng((worldX - _centerX) * _scale + Width / 2.0)
    End Function

    Private Function ScreenY(ByVal worldZ As Double) As Single
        Return CSng(Height / 2.0 - (worldZ - _centerZ) * _scale)
    End Function

    Private Function WorldX(ByVal screenX As Double) As Double
        Return (screenX - Width / 2.0) / _scale + _centerX
    End Function

    Private Function WorldZ(ByVal screenY As Double) As Double
        Return _centerZ - (screenY - Height / 2.0) / _scale
    End Function

    ''' <summary>Enquadra o mapa inteiro (ou os pontos, se nao ha malha).</summary>
    Friend Sub FitView()
        Dim minX As Double, maxX As Double, minZ As Double, maxZ As Double
        If _mesh IsNot Nothing Then
            minX = _mesh.MinX : maxX = _mesh.MaxX : minZ = _mesh.MinZ : maxZ = _mesh.MaxZ
        ElseIf _backdrop IsNot Nothing Then
            minX = _backdropMinX : maxX = _backdropMaxX : minZ = _backdropMinZ : maxZ = _backdropMaxZ
        ElseIf _points.Count > 0 Then
            minX = _points.Min(Function(p) p.X) : maxX = _points.Max(Function(p) p.X)
            minZ = _points.Min(Function(p) p.Z) : maxZ = _points.Max(Function(p) p.Z)
        Else
            minX = -1000 : maxX = 1000 : minZ = -1000 : maxZ = 1000
        End If
        Dim rangeX As Double = Math.Max(maxX - minX, 500.0)
        Dim rangeZ As Double = Math.Max(maxZ - minZ, 500.0)
        _centerX = (minX + maxX) / 2
        _centerZ = (minZ + maxZ) / 2
        _scale = Math.Min(Math.Max(Width, 50) / rangeX, Math.Max(Height, 50) / rangeZ) * 0.95
        Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        Dim g = e.Graphics
        g.Clear(_noTerrain)

        If _raster IsNot Nothing Then
            Dim left As Single = ScreenX(_mesh.MinX)
            Dim top As Single = ScreenY(_mesh.MaxZ)
            Dim w As Single = CSng(_rasterW * _cell * _scale)
            Dim h As Single = CSng(_rasterH * _cell * _scale)
            g.InterpolationMode = If(_cell * _scale < 1.0, InterpolationMode.HighQualityBilinear, InterpolationMode.NearestNeighbor)
            g.PixelOffsetMode = PixelOffsetMode.Half
            Using attrs As New ImageAttributes()
                attrs.SetWrapMode(WrapMode.TileFlipXY)
                g.DrawImage(_raster, New Rectangle(CInt(left), CInt(top), CInt(Math.Ceiling(w)), CInt(Math.Ceiling(h))),
                            0, 0, _rasterW, _rasterH, GraphicsUnit.Pixel, attrs)
            End Using
        End If

        If _raster Is Nothing AndAlso _backdrop IsNot Nothing Then
            g.InterpolationMode = InterpolationMode.HighQualityBilinear
            g.DrawImage(_backdrop, ScreenX(_backdropMinX), ScreenY(_backdropMaxZ),
                        CSng((_backdropMaxX - _backdropMinX) * _scale), CSng((_backdropMaxZ - _backdropMinZ) * _scale))
        End If

        g.SmoothingMode = SmoothingMode.AntiAlias
        For Each p In _points
            If p.Slot = _selectedSlot Then Continue For
            DrawMarker(g, p, False)
        Next
        Dim selected = _points.FirstOrDefault(Function(p) p.Slot = _selectedSlot)
        If selected IsNot Nothing Then DrawMarker(g, selected, True)

        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit
        Using font As New Font(Me.Font.FontFamily, 8.0F, FontStyle.Bold)
            g.DrawString("N", font, Brushes.White, 6, 4)
            g.DrawLine(Pens.White, 11, 22, 11, 36)
            g.DrawLine(Pens.White, 11, 22, 8, 26)
            g.DrawLine(Pens.White, 11, 22, 14, 26)
        End Using
    End Sub

    Private Sub DrawMarker(ByVal g As Graphics, ByVal p As CanvasPoint, ByVal selected As Boolean)
        Dim sx As Single = ScreenX(p.X)
        Dim sy As Single = ScreenY(p.Z)
        If sx < -20 OrElse sy < -20 OrElse sx > Width + 20 OrElse sy > Height + 20 Then Return
        Dim r As Single = If(selected, 8.0F, 5.5F)
        If p.Angle >= 0 Then
            Dim length As Single = r + 10.0F
            Dim tipX As Single = sx + CSng(Math.Sin(AngleToRadians * p.Angle)) * length
            Dim tipY As Single = sy - CSng(Math.Cos(AngleToRadians * p.Angle)) * length
            Using arrow As New Pen(If(selected, Color.Gold, Color.Crimson), 2.0F)
                g.DrawLine(arrow, sx, sy, tipX, tipY)
            End Using
        End If
        Using fill As New SolidBrush(If(selected, Color.Gold, Color.Crimson)),
              outline As New Pen(If(selected, Color.Black, Color.White), If(selected, 2.0F, 1.5F))
            g.FillEllipse(fill, sx - r, sy - r, r * 2, r * 2)
            g.DrawEllipse(outline, sx - r, sy - r, r * 2, r * 2)
        End Using
        If selected OrElse (_showLabels AndAlso p.Label <> "") Then
            Using font As New Font(Me.Font.FontFamily, 8.5F, If(selected, FontStyle.Bold, FontStyle.Regular)),
                  back As New SolidBrush(Color.FromArgb(190, 0, 0, 0))
                Dim label As String = If(p.Label <> "", p.Label, "#" & p.Slot)
                Dim size As SizeF = g.MeasureString(label, font)
                g.FillRectangle(back, sx + r + 2, sy - size.Height / 2, size.Width, size.Height)
                g.DrawString(label, font, Brushes.White, sx + r + 2, sy - size.Height / 2)
            End Using
        End If
    End Sub

#End Region

#Region "Mouse e teclado"

    Private Function HitTest(ByVal pos As Point) As CanvasPoint
        For i = _points.Count - 1 To 0 Step -1
            Dim p = _points(i)
            Dim dx As Single = ScreenX(p.X) - pos.X
            Dim dy As Single = ScreenY(p.Z) - pos.Y
            If dx * dx + dy * dy <= HitRadius * HitRadius Then Return p
        Next
        Return Nothing
    End Function

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        Focus()
        If e.Button = MouseButtons.Left Then
            Dim hit = HitTest(e.Location)
            If hit IsNot Nothing AndAlso ReplaceOnCtrlClick AndAlso (ModifierKeys And Keys.Control) = Keys.Control Then
                RaiseEvent ReplaceRequested(hit.Slot)
                Return
            End If
            If hit IsNot Nothing Then
                _selectedSlot = hit.Slot
                _dragPoint = hit
                RaiseEvent PointSelected(hit.Slot)
            Else
                _pressed = True
            End If
            _pressPos = e.Location
            _moved = False
            Invalidate()
        ElseIf e.Button = MouseButtons.Right OrElse e.Button = MouseButtons.Middle Then
            _panning = True
            _panMoved = False
            _panStart = e.Location
            _panLast = e.Location
            Cursor = Cursors.SizeAll
        End If
    End Sub

    Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
        MyBase.OnMouseMove(e)
        If _panning Then
            If Math.Abs(e.X - _panStart.X) > DragThreshold OrElse Math.Abs(e.Y - _panStart.Y) > DragThreshold Then _panMoved = True
            _centerX -= (e.X - _panLast.X) / _scale
            _centerZ += (e.Y - _panLast.Y) / _scale
            _panLast = e.Location
            Invalidate()
        ElseIf _dragPoint IsNot Nothing Then
            If Not _moved AndAlso (Math.Abs(e.X - _pressPos.X) > DragThreshold OrElse Math.Abs(e.Y - _pressPos.Y) > DragThreshold) Then _moved = True
            If _moved Then
                _dragPoint.X = CInt(Math.Round(WorldX(e.X)))
                _dragPoint.Z = CInt(Math.Round(WorldZ(e.Y)))
                Invalidate()
            End If
        End If

        Dim wx As Double = WorldX(e.X), wz As Double = WorldZ(e.Y)
        Dim height As Single
        Dim hasHeight As Boolean = HeightAt(wx, wz, height)
        RaiseEvent HoverChanged(CInt(Math.Round(wx)), CInt(Math.Round(wz)), hasHeight, height)

        If Not _panning Then
            Cursor = If(HitTest(e.Location) IsNot Nothing, Cursors.Hand, If(AddOnClick, Cursors.Cross, Cursors.Default))
        End If
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
        MyBase.OnMouseUp(e)
        If e.Button = MouseButtons.Left Then
            If _dragPoint IsNot Nothing Then
                Dim moved = _dragPoint
                _dragPoint = Nothing
                If _moved Then RaiseEvent PointMoved(moved.Slot, moved.X, moved.Z)
            ElseIf _pressed Then
                If AddOnClick AndAlso Math.Abs(e.X - _pressPos.X) <= DragThreshold AndAlso Math.Abs(e.Y - _pressPos.Y) <= DragThreshold Then
                    RaiseEvent AddRequested(CInt(Math.Round(WorldX(e.X))), CInt(Math.Round(WorldZ(e.Y))))
                End If
            End If
            _pressed = False
        ElseIf _panning Then
            _panning = False
            Cursor = Cursors.Default
            If e.Button = MouseButtons.Right AndAlso Not _panMoved AndAlso ClickMenu IsNot Nothing Then ClickMenu.Show(Me, e.Location)
        End If
    End Sub

    Protected Overrides Sub OnMouseWheel(ByVal e As MouseEventArgs)
        MyBase.OnMouseWheel(e)
        Dim factor As Double = Math.Pow(1.25, e.Delta / 120.0)
        Dim beforeX As Double = WorldX(e.X), beforeZ As Double = WorldZ(e.Y)
        _scale = Math.Max(0.0001, Math.Min(MaxScale, _scale * factor))
        _centerX = beforeX - (e.X - Width / 2.0) / _scale
        _centerZ = beforeZ + (e.Y - Height / 2.0) / _scale
        Invalidate()
    End Sub

    Protected Overrides Sub OnKeyDown(ByVal e As KeyEventArgs)
        MyBase.OnKeyDown(e)
        If e.KeyCode = Keys.Delete Then
            RaiseEvent DeleteRequested()
            e.Handled = True
        ElseIf e.KeyCode = Keys.Prior Then
            RaiseEvent RotateRequested(1)
            e.Handled = True
        ElseIf e.KeyCode = Keys.Next Then
            RaiseEvent RotateRequested(-1)
            e.Handled = True
        End If
    End Sub

    Protected Overrides Function IsInputKey(ByVal keyData As Keys) As Boolean
        Return keyData = Keys.Delete OrElse keyData = Keys.Prior OrElse keyData = Keys.Next OrElse MyBase.IsInputKey(keyData)
    End Function

#End Region

End Class
