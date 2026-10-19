Imports System.IO

''' <summary>
''' NPCs: definicoes de texto (GameServer\NPC\*.NPC + name\*.zhoon) e posicoes binarias por mapa
''' (GameServer\Field\*.spc, 100 registros de 504 bytes).
''' </summary>
Friend Module NpcData

    Friend Const RecordSize As Integer = &H1F8
    Friend Const RecordCount As Integer = 100
    ''' <summary>Valor gravado no offset 4 quando o NPC esta ativo.</summary>
    Friend Const ActiveMagic As Integer = &H48470070
    Friend Const TextFieldSize As Integer = &H40
    Friend Const MaxId As Integer = &HFFFFFF

    Private Const OffsetActive As Integer = 4
    Private Const OffsetModelFile As Integer = 40
    Private Const OffsetSetupFile As Integer = &H68
    Private Const OffsetId As Integer = &H1D8
    Private Const OffsetX As Integer = &H1DD
    Private Const OffsetY As Integer = &H1E1
    Private Const OffsetZ As Integer = &H1E5
    Private Const OffsetAngle As Integer = &H1EC

    Friend Const NoNameFound As String = "No name found"
    Friend Const NoZhoonFound As String = "No Zhoon found"
    Friend Const Empty As String = "Empty"

    ''' <summary>Registro-modelo usado ao criar um NPC novo (copiado de um .spc original).</summary>
    Private Const TemplateHex As String =
        "f8 01 00 00 70 00 47 48 c0 e2 c8 ad c1 a1 20 ba f1 be c8 c6 ae 00 c6 00 d5 c6 ae 00 00 00 00 00 00 00 00 00 00 00 00 00 " &
        "63 68 61 72 5c 6e 70 63 5c 54 4e 2d 30 30 35 5c 54 4e 2d 30 30 35 2e 69 6e 69 00 6e 57 6f 6c 76 65 72 69 6e 2d 30 33 2e 69 6e 69 " &
        "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 " &
        "47 61 6d 65 53 65 72 76 65 72 5c 6e 70 63 5c 6e 61 76 69 73 6b 6f 2d 73 74 6f 72 65 2e 6e 70 63 " &
        "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 " &
        "ed 20 00 50 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00 08 00 00 00 64 00 00 00 29 00 00 00 e0 00 00 00 57 00 00 00 " &
        "0f 00 00 00 7a 00 00 00 00 00 00 00 f4 01 00 00 40 00 00 00 59 00 00 00 08 00 00 00 b4 00 00 00 00 00 00 00 a6 01 00 00 " &
        "00 00 00 00 17 00 00 00 03 00 00 00 b4 00 00 00 a7 01 f9 01 0f 00 00 00 06 00 0b 00 0b 00 10 00 00 00 00 00 00 00 00 00 " &
        "00 00 00 00 00 00 00 00 00 00 00 00 64 00 64 00 64 00 af 03 bc 02 bc 02 b8 1e 85 3f 47 e1 ca 40 00 00 00 00 7c e4 a0 01 " &
        "ef fe fc 6f 9c e7 00 00 00 00 00 00 00 00 00 00 69 00 00 00 00 07 00 00 01 00 03 00 16 00 00 00 03 00 00 00 01 00 00 00 " &
        "03 00 00 00 02 00 02 00 22 b2 ff ff 01 00 00 00 00 00 04 00 00 00 00 00 00 00 00 00 14 a4 b5 6e 00 00 00 00 00 00 00 00 " &
        "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 " &
        "00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 " &
        "01 00 00 00 b2 5d 04 30 95 55 56 00 11 10 03 00 c0 18 f6 ff 00 00 00 00 5f 0e 00 00 00 00 00 00 00 00 00 00"

    Friend Class NpcRecord
        ''' <summary>Slot ocupado (primeiro Int32 diferente de zero).</summary>
        Public Property Used As Boolean
        Public Property Active As Boolean
        Public Property ModelFile As String = ""
        ''' <summary>Caminho relativo do .NPC (ex.: GameServer\npc\xxx.npc).</summary>
        Public Property SetupFile As String = ""
        Public Property Id As Integer
        Public Property X As Integer
        Public Property Y As Integer
        Public Property Z As Integer
        Public Property Angle As Integer
    End Class

    ' ---- Arquivos .NPC ------------------------------------------------------------

    Friend Sub LoadNpcFiles(Optional ByVal progress As Action(Of Integer, Integer) = Nothing)
        AppState.Npcs.Clear()
        Dim files = FileHelper.ListFiles(AppState.ServerPath & AppState.NpcFolder, AppState.NpcPattern)
        For i = 0 To files.Count - 1
            Dim npc As New NpcFile(files(i).ToUpper())
            Dim path As String = AppState.NpcPath(npc.FileName)
            npc.LoadFrom(path)
            For Each line In npc.Lines
                If line.StartsWith(GameFields.Npc.ModelFile) Then
                    npc.SetupIni = FileHelper.Between(line, """"c).ToUpper()
                    Exit For
                End If
            Next
            npc.Name = ReadName(path)
            AppState.Npcs.Add(npc)
            If progress IsNot Nothing Then progress(i + 1, files.Count)
        Next
    End Sub

    Friend Function FindNpc(ByVal fileName As String) As NpcFile
        Dim upper As String = fileName.ToUpper()
        For Each n In AppState.Npcs
            If n.FileName = upper Then Return n
        Next
        Return Nothing
    End Function

    ''' <summary>Referencia ao .zhoon (*연결파일) dentro do .NPC, sem aspas.</summary>
    Friend Function ReadZhoonReference(ByVal npcPath As String) As String
        Try
            For Each line In FileHelper.ReadLines(npcPath)
                If line.StartsWith(GameFields.LinkedFile) Then
                    Return line.Substring(GameFields.LinkedFile.Length).Replace(ChrW(9), "").Replace("""", "").Trim()
                End If
            Next
            Return NoZhoonFound
        Catch
            Return "Not found"
        End Try
    End Function

    Private Function ZhoonPath(ByVal npcPath As String) As String
        For Each line In FileHelper.ReadLines(npcPath)
            If line.Contains(GameFields.LinkedFile) Then
                Return Path.GetDirectoryName(npcPath) & "\" & line.Replace(GameFields.LinkedFile, "").Replace("""", "").Trim()
            End If
        Next
        Return Nothing
    End Function

    ''' <summary>Nome do NPC (*X_NAME do .zhoon, sem aspas).</summary>
    Friend Function ReadName(ByVal npcPath As String) As String
        Dim failure As String = "Cant find " & Path.GetFileName(npcPath)
        Try
            Dim zhoon As String = ZhoonPath(npcPath)
            If zhoon Is Nothing Then Return NoNameFound
            failure = "Cant find .zhoon file"
            Dim name As String = ZhoonFile.ReadName(zhoon)
            If name Is Nothing Then Return NoNameFound
            Return name.Replace("""", "").Trim()
        Catch
            Return failure
        End Try
    End Function

    ''' <summary>Falas do NPC (*X_CHAT do .zhoon). Em erro retorna uma unica mensagem.</summary>
    Friend Function ReadChat(ByVal npcPath As String) As List(Of String)
        Dim result As New List(Of String)()
        Dim failure As String = "Cant find " & Path.GetFileName(npcPath)
        Try
            Dim zhoon As String = ZhoonPath(npcPath)
            If zhoon Is Nothing Then
                result.Add(NoNameFound)
                Return result
            End If
            failure = "Cant find .zhoon file"
            result.AddRange(ZhoonFile.ReadChat(zhoon))
            If result.Count = 0 Then result.Add("No chat found")
            Return result
        Catch
            result.Clear()
            result.Add(failure)
            Return result
        End Try
    End Function

    ''' <summary>Grava o .zhoon com nome e falas (formato original, com linhas em branco) usando a tag de idioma atual.</summary>
    Friend Sub SaveZhoon(ByVal zhoonPath As String, ByVal setupFileName As String, ByVal name As String, ByVal chatLines As IEnumerable(Of String))
        Using writer As New StreamWriter(zhoonPath, False, AppState.FileEncoding)
            writer.WriteLine("//" & setupFileName & vbCrLf)
            writer.WriteLine(vbCrLf & ZhoonFile.NameLine(name) & vbCrLf & vbCrLf)
            For Each chat In chatLines
                If chat <> "" Then writer.WriteLine(ZhoonFile.ChatLine(chat) & vbCrLf)
            Next
        End Using
    End Sub

    ''' <summary>Codigos (sem aspas) da lista de venda; "없음" aparece como token.</summary>
    Friend Function ReadShopItems(ByVal npcPath As String, ByVal shopKey As String) As List(Of String)
        Dim result As New List(Of String)()
        If Not File.Exists(npcPath) Then Return result
        For Each line In FileHelper.ReadLines(npcPath)
            If line.StartsWith(shopKey) Then
                result.AddRange(line.Replace(shopKey, "").Trim().Replace(ChrW(9), " "c).Split({" "c}, StringSplitOptions.RemoveEmptyEntries))
                Exit For
            End If
        Next
        Return result
    End Function

    ''' <summary>Reescreve a lista de venda e o *모양파일 do .NPC.</summary>
    Friend Sub SaveShop(ByVal npcPath As String, ByVal shopKey As String, ByVal codes As IEnumerable(Of String), ByVal modelIni As String)
        Dim npc As New TextDataFile(Path.GetFileName(npcPath))
        npc.LoadFrom(npcPath)
        Dim list As String = String.Join(" ", codes)
        If list = "" Then list = GameFields.Monster.DropNothing
        For i = 0 To npc.Lines.Count - 1
            If npc.Lines(i).StartsWith(shopKey) Then npc.Lines(i) = shopKey & vbTab & list
            If npc.Lines(i).StartsWith(GameFields.Npc.ModelFile) Then npc.Lines(i) = GameFields.Npc.ModelFile & vbTab & """" & modelIni & """"
        Next
        npc.SaveTo(npcPath)
    End Sub

    ''' <summary>Troca a referencia *연결파일 do .NPC copiado para name\&lt;nome&gt;.zhoon.</summary>
    Friend Sub SetZhoonReference(ByVal npcPath As String, ByVal npcName As String)
        Dim npc As New TextDataFile(Path.GetFileName(npcPath))
        npc.LoadFrom(npcPath)
        For i = 0 To npc.Lines.Count - 1
            If npc.Lines(i).StartsWith(GameFields.LinkedFile) Then
                npc.Lines(i) = GameFields.LinkedFile & vbTab & """name\" & npcName & ".zhoon"""
            End If
        Next
        npc.SaveTo(npcPath)
    End Sub

    ' ---- Arquivos binarios .spc / .spp ----------------------------------------------

    Friend Sub LoadSpcFiles()
        AppState.SpcFiles.Clear()
        For Each f In FileHelper.ListFiles(AppState.ServerPath & AppState.FieldFolder, AppState.SpcPattern)
            AppState.SpcFiles.Add(New BinaryDataFile(f, File.ReadAllBytes(AppState.FieldPath(f))))
        Next
    End Sub

    Friend Sub LoadSppFiles()
        AppState.SppFiles.Clear()
        For Each f In FileHelper.ListFiles(AppState.ServerPath & AppState.FieldFolder, AppState.SppPattern)
            AppState.SppFiles.Add(New BinaryDataFile(f, File.ReadAllBytes(AppState.FieldPath(f))))
        Next
    End Sub

    Friend Function FindSpc(ByVal fileName As String) As BinaryDataFile
        Dim name As String = Path.GetFileName(fileName).ToUpper()
        For Each s In AppState.SpcFiles
            If s.FileName.ToUpper() = name Then Return s
        Next
        Return Nothing
    End Function

    ''' <summary>Relê o .spc do disco para a memoria.</summary>
    Friend Sub ReloadSpc(ByVal fileName As String)
        Dim spc = FindSpc(fileName)
        If spc IsNot Nothing Then spc.Data = File.ReadAllBytes(AppState.FieldPath(spc.FileName))
    End Sub

    Friend Function IsRecordUsed(ByVal spc As BinaryDataFile, ByVal index As Integer) As Boolean
        Return BitConverter.ToInt32(spc.Data, index * RecordSize) <> 0
    End Function

    ''' <summary>Inteiro de 24 bits sem sinal (3 bytes uteis).</summary>
    Private Function ReadUInt24(ByVal data As Byte(), ByVal offset As Integer) As Integer
        Dim buffer = FileHelper.Slice(data, offset, 4)
        buffer(3) = 0
        Return BitConverter.ToInt32(buffer, 0)
    End Function

    ''' <summary>Coordenada de 24 bits com sinal (3 bytes uteis).</summary>
    Private Function ReadCoordinate(ByVal data As Byte(), ByVal offset As Integer) As Integer
        Dim raw As Integer = ReadUInt24(data, offset)
        Return If(raw >= 8388608, raw - 16777216, raw)
    End Function

    Private Function CoordinateBytes(ByVal value As Integer) As Byte()
        Dim bytes = BitConverter.GetBytes(value)
        bytes(3) = 0
        Return bytes
    End Function

    Friend Function ReadRecord(ByVal spc As BinaryDataFile, ByVal index As Integer) As NpcRecord
        Dim base As Integer = index * RecordSize
        Dim rec As New NpcRecord()
        rec.Used = BitConverter.ToInt32(spc.Data, base) <> 0
        rec.Active = BitConverter.ToInt32(spc.Data, base + OffsetActive) = ActiveMagic
        rec.ModelFile = FileHelper.FixedText(spc.Data, base + OffsetModelFile, TextFieldSize)
        rec.SetupFile = FileHelper.FixedText(spc.Data, base + OffsetSetupFile, TextFieldSize)
        rec.Id = ReadUInt24(spc.Data, base + OffsetId)
        rec.X = ReadCoordinate(spc.Data, base + OffsetX)
        rec.Y = ReadCoordinate(spc.Data, base + OffsetY)
        rec.Z = ReadCoordinate(spc.Data, base + OffsetZ)
        rec.Angle = BitConverter.ToInt32(spc.Data, base + OffsetAngle)
        Return rec
    End Function

    ' ---- Edicao em memoria: nada vai para o disco ate SaveSpc ---------------------------------------

    Private Sub PutBytes(ByVal spc As BinaryDataFile, ByVal offset As Integer, ByVal bytes As Byte(), ByVal count As Integer)
        Array.Copy(bytes, 0, spc.Data, offset, count)
    End Sub

    ''' <summary>Altera os campos editaveis do registro na copia em memoria (nao mexe no restante do registro).</summary>
    Friend Sub WriteRecord(ByVal spc As BinaryDataFile, ByVal index As Integer, ByVal rec As NpcRecord)
        Dim base As Integer = index * RecordSize
        PutBytes(spc, base + OffsetActive, BitConverter.GetBytes(If(rec.Active, ActiveMagic, 0)), 4)
        PutBytes(spc, base + OffsetModelFile, FileHelper.FixedBytes(rec.ModelFile, TextFieldSize), TextFieldSize)
        PutBytes(spc, base + OffsetSetupFile, FileHelper.FixedBytes(rec.SetupFile, TextFieldSize), TextFieldSize)
        PutBytes(spc, base + OffsetId, BitConverter.GetBytes(rec.Id), 3)
        PutBytes(spc, base + OffsetX, CoordinateBytes(rec.X), 4)
        PutBytes(spc, base + OffsetY, CoordinateBytes(rec.Y), 4)
        PutBytes(spc, base + OffsetZ, CoordinateBytes(rec.Z), 4)
        PutBytes(spc, base + OffsetAngle, CoordinateBytes(rec.Angle), 4)
    End Sub

    ''' <summary>Altera so a posicao (X, Y, Z) do registro na copia em memoria.</summary>
    Friend Sub WritePosition(ByVal spc As BinaryDataFile, ByVal index As Integer, ByVal x As Integer, ByVal y As Integer, ByVal z As Integer)
        Dim base As Integer = index * RecordSize
        PutBytes(spc, base + OffsetX, CoordinateBytes(x), 4)
        PutBytes(spc, base + OffsetY, CoordinateBytes(y), 4)
        PutBytes(spc, base + OffsetZ, CoordinateBytes(z), 4)
    End Sub

    ''' <summary>Zera o registro (slot livre) na copia em memoria.</summary>
    Friend Sub ClearRecord(ByVal spc As BinaryDataFile, ByVal index As Integer)
        Array.Clear(spc.Data, index * RecordSize, RecordSize)
    End Sub

    Friend Sub SwapRecords(ByVal spc As BinaryDataFile, ByVal first As Integer, ByVal second As Integer)
        Dim a = FileHelper.Slice(spc.Data, first * RecordSize, RecordSize)
        Dim b = FileHelper.Slice(spc.Data, second * RecordSize, RecordSize)
        PutBytes(spc, first * RecordSize, b, RecordSize)
        PutBytes(spc, second * RecordSize, a, RecordSize)
    End Sub

    ''' <summary>Grava o registro-modelo no slot (em memoria) apontando para o .NPC informado (caminho relativo).</summary>
    Friend Sub WriteTemplateRecord(ByVal spc As BinaryDataFile, ByVal index As Integer, ByVal setupFileRelativePath As String)
        Dim tokens = TemplateHex.Split({" "c}, StringSplitOptions.RemoveEmptyEntries)
        Dim record(RecordSize - 1) As Byte
        For i = 0 To RecordSize - 1
            record(i) = Convert.ToByte(tokens(i), 16)
        Next
        Array.Copy(FileHelper.FixedBytes(setupFileRelativePath, TextFieldSize), 0, record, OffsetSetupFile, TextFieldSize)
        PutBytes(spc, index * RecordSize, record, RecordSize)
    End Sub

    ''' <summary>Faz backup do .spc existente (Field\Backup) e grava a copia em memoria no disco.</summary>
    Friend Sub SaveSpc(ByVal spc As BinaryDataFile)
        Dim folder As String = AppState.ServerPath & AppState.FieldFolder
        BackupHelper.CreateBackup(folder, spc.FileName)
        File.WriteAllBytes(folder & spc.FileName, spc.Data)
    End Sub

End Module
