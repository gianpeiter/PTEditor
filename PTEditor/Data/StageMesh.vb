Imports System.IO

''' <summary>
''' Malha 3D de um mapa (Field\*.smd). O arquivo e o dump binario do smSTAGE3D do jogo (x86):
''' cabecalho "SMD Stage data Ver 0.72" + o proprio objeto + materiais + vertices (28 bytes) + faces (28 bytes).
''' So geometria e lida (sem texturas). Coordenadas em ponto fixo de 8 bits, a mesma unidade do .spp.
''' </summary>
Friend Class StageMesh

    Private Const Signature As String = "SMD Stage data Ver 0.72"
    Private Const HeaderSize As Integer = 556
    ''' <summary>sizeof(smSTAGE3D) no jogo (x86): o objeto gravado logo apos o cabecalho.</summary>
    Private Const StageObjectSize As Integer = 262260
    Private Const StageCountsOffset As Integer = HeaderSize + 4 + 256 * 256 * 4 + 4 * 12
    Private Const VertexSize As Integer = 28
    Private Const FaceSize As Integer = 28
    Private Const MaxCount As Integer = 2000000

    Friend ReadOnly Property FilePath As String

    ''' <summary>Posicao dos vertices, em unidades de mundo (X e Z no plano, Y e a altura).</summary>
    Friend ReadOnly Property X As Single()
    Friend ReadOnly Property Y As Single()
    Friend ReadOnly Property Z As Single()
    ''' <summary>Indices dos vertices, tres por triangulo.</summary>
    Friend ReadOnly Property Triangles As Integer()

    Friend ReadOnly Property MinX As Single
    Friend ReadOnly Property MaxX As Single
    Friend ReadOnly Property MinZ As Single
    Friend ReadOnly Property MaxZ As Single
    Friend ReadOnly Property MinY As Single
    Friend ReadOnly Property MaxY As Single

    Friend ReadOnly Property VertexCount As Integer
        Get
            Return X.Length
        End Get
    End Property

    Friend ReadOnly Property TriangleCount As Integer
        Get
            Return Triangles.Length \ 3
        End Get
    End Property

    ''' <summary>Le o .smd. Gera Exception com mensagem legivel se o arquivo nao for um mapa valido.</summary>
    Friend Sub New(ByVal filePath As String)
        Me.FilePath = filePath
        Dim data As Byte() = File.ReadAllBytes(filePath)
        If data.Length < StageCountsOffset + 16 Then Throw New InvalidDataException("File is too small to be a map (.smd).")
        If FileHelper.FixedText(data, 0, 24) <> Signature Then
            Throw New InvalidDataException("Not a supported .smd (expected header """ & Signature & """).")
        End If

        Dim vertexCount As Integer = BitConverter.ToInt32(data, StageCountsOffset)
        Dim faceCount As Integer = BitConverter.ToInt32(data, StageCountsOffset + 4)
        If vertexCount <= 0 OrElse vertexCount > MaxCount OrElse faceCount <= 0 OrElse faceCount > MaxCount Then
            Throw New InvalidDataException("Unexpected vertex/face counts (" & vertexCount & "/" & faceCount & ").")
        End If

        ' Os materiais ficam entre o objeto e os vertices; First_ObjInfoPoint (offset 36) marca onde terminam.
        Dim materialEnd As Integer = BitConverter.ToInt32(data, 36)
        Dim vertexStart As Long = StageObjectSize + materialEnd
        Dim faceStart As Long = vertexStart + CLng(vertexCount) * VertexSize
        If materialEnd < HeaderSize OrElse faceStart + CLng(faceCount) * FaceSize > data.Length Then
            Throw New InvalidDataException("The vertex/face data does not fit in the file.")
        End If

        Dim xs(vertexCount - 1) As Single, ys(vertexCount - 1) As Single, zs(vertexCount - 1) As Single
        Dim minX As Single = Single.MaxValue, maxX As Single = Single.MinValue
        Dim minY As Single = Single.MaxValue, maxY As Single = Single.MinValue
        Dim minZ As Single = Single.MaxValue, maxZ As Single = Single.MinValue
        For i = 0 To vertexCount - 1
            Dim p As Integer = CInt(vertexStart) + i * VertexSize + 8
            xs(i) = BitConverter.ToInt32(data, p) / 256.0F
            ys(i) = BitConverter.ToInt32(data, p + 4) / 256.0F
            zs(i) = BitConverter.ToInt32(data, p + 8) / 256.0F
            If xs(i) < minX Then minX = xs(i)
            If xs(i) > maxX Then maxX = xs(i)
            If ys(i) < minY Then minY = ys(i)
            If ys(i) > maxY Then maxY = ys(i)
            If zs(i) < minZ Then minZ = zs(i)
            If zs(i) > maxZ Then maxZ = zs(i)
        Next

        Dim tris As New List(Of Integer)(faceCount * 3)
        For i = 0 To faceCount - 1
            Dim p As Integer = CInt(faceStart) + i * FaceSize + 8
            Dim a As Integer = BitConverter.ToUInt16(data, p)
            Dim b As Integer = BitConverter.ToUInt16(data, p + 2)
            Dim c As Integer = BitConverter.ToUInt16(data, p + 4)
            If a < vertexCount AndAlso b < vertexCount AndAlso c < vertexCount Then
                tris.Add(a) : tris.Add(b) : tris.Add(c)
            End If
        Next

        _X = xs : _Y = ys : _Z = zs
        _Triangles = tris.ToArray()
        _MinX = minX : _MaxX = maxX : _MinY = minY : _MaxY = maxY : _MinZ = minZ : _MaxZ = maxZ
    End Sub

    ''' <summary>
    ''' Altura do terreno em (x, z): a face virada para cima mais alta que cobre o ponto (a mesma regra da vista de cima).
    ''' O Y dos NPCs gravados nos .spc e essa altura, na mesma unidade. False se nao ha terreno ali.
    ''' </summary>
    Friend Function TryHeightAt(ByVal px As Double, ByVal pz As Double, ByRef height As Single) As Boolean
        Dim found As Boolean
        Dim best As Double = Double.NegativeInfinity
        For t = 0 To Triangles.Length - 3 Step 3
            Dim ia = Triangles(t), ib = Triangles(t + 1), ic = Triangles(t + 2)
            Dim ax As Double = X(ia), az As Double = Z(ia)
            Dim bx As Double = X(ib), bz As Double = Z(ib)
            Dim cx As Double = X(ic), cz As Double = Z(ic)
            If px < Math.Min(ax, Math.Min(bx, cx)) - 0.5 OrElse px > Math.Max(ax, Math.Max(bx, cx)) + 0.5 Then Continue For
            If pz < Math.Min(az, Math.Min(bz, cz)) - 0.5 OrElse pz > Math.Max(az, Math.Max(bz, cz)) + 0.5 Then Continue For
            Dim denom As Double = (bz - cz) * (ax - cx) + (cx - bx) * (az - cz)
            If Math.Abs(denom) < 0.000000001 Then Continue For
            Dim w0 As Double = ((bz - cz) * (px - cx) + (cx - bx) * (pz - cz)) / denom
            Dim w1 As Double = ((cz - az) * (px - cx) + (ax - cx) * (pz - cz)) / denom
            Dim w2 As Double = 1.0 - w0 - w1
            If w0 < -0.0001 OrElse w1 < -0.0001 OrElse w2 < -0.0001 Then Continue For
            ' Mesma normal da vista de cima (SpawnMapCanvas): so faces viradas para cima contam.
            Dim ux As Double = bx - ax, uy As Double = Y(ib) - Y(ia), uz As Double = bz - az
            Dim vx As Double = cx - ax, vy As Double = Y(ic) - Y(ia), vz As Double = cz - az
            Dim nx As Double = uy * vz - uz * vy
            Dim ny As Double = uz * vx - ux * vz
            Dim nz As Double = ux * vy - uy * vx
            Dim length As Double = Math.Sqrt(nx * nx + ny * ny + nz * nz)
            If length < 0.000001 OrElse -ny / length < 0.05 Then Continue For
            Dim h As Double = w0 * Y(ia) + w1 * Y(ib) + w2 * Y(ic)
            If h > best Then
                best = h
                found = True
            End If
        Next
        If found Then height = CSng(best)
        Return found
    End Function

    ''' <summary>
    ''' Procura o .smd do mapa em &lt;servidor&gt;\Field (qualquer subpasta) pelo nome base do .spp/.spm
    ''' ("Dun-6a.ASE.spp" procura Dun-6a.smd). Retorna "" se nao achar.
    ''' </summary>
    Friend Shared Function FindFile(ByVal fieldFileName As String) As String
        Dim dot As Integer = fieldFileName.IndexOf("."c)
        Dim baseName As String = If(dot < 0, fieldFileName, fieldFileName.Substring(0, dot))
        Dim folder As String = AppState.ServerPath & AppState.MeshFolder
        If baseName = "" OrElse Not Directory.Exists(folder) Then Return ""
        Dim found = Directory.GetFiles(folder, baseName & ".smd", SearchOption.AllDirectories)
        Return If(found.Length = 0, "", found(0))
    End Function

End Class
