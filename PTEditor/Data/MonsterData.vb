Imports System.IO
Imports System.Text.RegularExpressions

''' <summary>Carga, consulta e gravacao dos monstros (GameServer\Monster\*.inf).</summary>
Friend Module MonsterData

    Friend Const NameNotFound As String = "Name not found!"

    ''' <summary>Recarrega todos os .inf do servidor. progress(atual, total) e opcional.</summary>
    Friend Sub LoadAll(ByVal warnMissingZhoon As Boolean, Optional ByVal progress As Action(Of Integer, Integer) = Nothing)
        AppState.Monsters.Clear()
        ZhoonFile.ResetDetection()
        Dim files = FileHelper.ListFiles(AppState.ServerPath & AppState.MonsterFolder, AppState.MonsterPattern)
        Dim missingZhoon As New List(Of String)()
        For i = 0 To files.Count - 1
            Dim monster As New MonsterFile(files(i))
            monster.LoadFrom(AppState.MonsterPath(monster.FileName))
            RefreshInfo(monster, warnMissingZhoon, missingZhoon)
            AppState.Monsters.Add(monster)
            If progress IsNot Nothing Then progress(i + 1, files.Count)
        Next
        If missingZhoon.Count > 0 Then
            ErrorLog.Write("In MonsterData.LoadAll: Warn" & vbCrLf & missingZhoon.Count & " .zhoon file(s) not found:" & vbCrLf & vbCrLf &
                           String.Join(vbCrLf, missingZhoon))
            If warnMissingZhoon Then
                MsgBox(missingZhoon.Count & " monster(s) with no .zhoon file found." & vbCrLf & "Check the error log for details.")
            End If
        End If
    End Sub

    ''' <summary>Relê o arquivo do disco, descartando alteracoes em memoria.</summary>
    Friend Sub Reload(ByVal monster As MonsterFile)
        monster.LoadFrom(AppState.MonsterPath(monster.FileName))
        RefreshInfo(monster, My.Settings.WarnMoZhoon)
    End Sub

    ''' <summary>Carrega um backup (Monster\Backup\nome) para a memoria, sem gravar.</summary>
    Friend Sub LoadBackup(ByVal monster As MonsterFile, ByVal backupFileName As String)
        Try
            monster.LoadFrom(BackupHelper.BackupPath(AppState.ServerPath & AppState.MonsterFolder, backupFileName))
            RefreshInfo(monster, My.Settings.WarnMoZhoon)
        Catch ex As Exception
            ErrorLog.Report("MonsterData.LoadBackup: " & backupFileName, ex)
        End Try
    End Sub

    ''' <summary>Grava as linhas em memoria no .inf (com backup opcional).</summary>
    Friend Sub Save(ByVal monster As MonsterFile, ByVal makeBackup As Boolean)
        Try
            If makeBackup Then BackupHelper.CreateBackup(AppState.ServerPath & AppState.MonsterFolder, monster.FileName)
            monster.SaveTo(AppState.MonsterPath(monster.FileName))
        Catch ex As Exception
            ErrorLog.Report("MonsterData.Save: " & monster.FileName, ex)
        End Try
    End Sub

    Friend Function Find(ByVal fileName As String) As MonsterFile
        For Each m In AppState.Monsters
            If m.FileName = fileName Then Return m
        Next
        Return Nothing
    End Function

    ''' <summary>Monstro cujo *이름 (nome usado nos .spm) e igual ao informado.</summary>
    Friend Function FindByMapName(ByVal mapName As String) As MonsterFile
        For Each m In AppState.Monsters
            If m.MapName = mapName Then Return m
        Next
        Return Nothing
    End Function

    ''' <summary>Atualiza nome in-game, level, nome de mapa e .zhoon a partir das linhas.</summary>
    Private Sub RefreshInfo(ByVal monster As MonsterFile, ByVal warnMissingZhoon As Boolean, Optional ByVal missingZhoon As List(Of String) = Nothing)
        monster.Name = ""
        monster.MapName = ""
        monster.Level = ""
        monster.ZhoonFile = ""
        For Each line In monster.Lines
            If line.Contains(GameFields.LinkedFile) Then
                Dim zhoon As String = line.Substring(GameFields.LinkedFile.Length).Trim()
                monster.ZhoonFile = ZhoonFullPath(zhoon)
                monster.Name = ReadInGameName(zhoon, warnMissingZhoon, missingZhoon)
            End If
            If line.StartsWith(GameFields.Level) Then
                Dim level As Long
                Dim text As String = line.Substring(GameFields.Level.Length).Trim()
                monster.Level = If(Long.TryParse(text, level), level.ToString(), text)
            End If
            If line.StartsWith(GameFields.Name) Then
                monster.MapName = FileHelper.Between(line.Substring(GameFields.Name.Length), """"c)
            End If
        Next
    End Sub

    Private Function ZhoonFullPath(ByVal zhoonReference As String) As String
        Dim relative As String = zhoonReference.Trim(""""c)
        If relative.Contains(":") Then Return relative
        Return AppState.MonsterPath(relative)
    End Function

    ''' <summary>Nome in-game: linha "*X_NAME" do .zhoon referenciado em *연결파일.
    ''' Se missingZhoon for informado, arquivos ausentes sao acumulados nele em vez de abrir um MsgBox por monstro.</summary>
    Friend Function ReadInGameName(ByVal zhoonReference As String, ByVal warnMissing As Boolean, Optional ByVal missingZhoon As List(Of String) = Nothing) As String
        Dim path As String = ZhoonFullPath(zhoonReference)
        Try
            If File.Exists(path) Then
                Return If(ZhoonFile.ReadName(path), "")
            End If
            If missingZhoon IsNot Nothing Then
                missingZhoon.Add(path)
            Else
                If warnMissing Then MsgBox("No .zhoon file found! " & vbCrLf & path)
                ErrorLog.Write("In ReadInGameName:" & vbCrLf & "Cant find path:" & path)
            End If
            Return NameNotFound
        Catch ex As Exception
            ErrorLog.Report("MonsterData.ReadInGameName", ex)
            Return ""
        End Try
    End Function

    ' ---- Campos simples -----------------------------------------------------

    ''' <summary>Valor (apos a chave) da ultima linha que comeca com a chave; "" se nao houver.</summary>
    Friend Function GetValue(ByVal monster As MonsterFile, ByVal key As String) As String
        Dim result As String = ""
        For Each line In monster.Lines
            If line.StartsWith(key) Then result = line.Substring(key.Length).Trim()
        Next
        Return result
    End Function

    Friend Function GetLongValue(ByVal monster As MonsterFile, ByVal key As String) As Long
        Dim value As Long
        Long.TryParse(GetValue(monster, key), value)
        Return value
    End Function

    Friend Function GetExp(ByVal monster As MonsterFile) As Long
        Return GetLongValue(monster, GameFields.Monster.Exp)
    End Function

    Friend Function GetLevel(ByVal monster As MonsterFile) As Long
        Return GetLongValue(monster, GameFields.Level)
    End Function

    ''' <summary>Reescreve todas as linhas que comecam com a chave como "chave[TAB]valor".</summary>
    Friend Sub SetAll(ByVal monster As MonsterFile, ByVal key As String, ByVal value As String)
        For i = 0 To monster.Lines.Count - 1
            If monster.Lines(i).StartsWith(key) Then monster.Lines(i) = key & vbTab & value
        Next
    End Sub

    ''' <summary>Reescreve so a primeira linha que comeca com a chave. Retorna False se nao existir.</summary>
    Friend Function SetFirst(ByVal monster As MonsterFile, ByVal key As String, ByVal value As String) As Boolean
        Dim index As Integer = monster.IndexOf(key)
        If index < 0 Then Return False
        monster.Lines(index) = key & vbTab & value
        Return True
    End Function

    Friend Sub SetExp(ByVal monster As MonsterFile, ByVal exp As Long)
        SetAll(monster, GameFields.Monster.Exp, exp.ToString())
    End Sub

    Friend Sub SetLevel(ByVal monster As MonsterFile, ByVal level As Long)
        SetAll(monster, GameFields.Level, level.ToString())
    End Sub

    ' ---- Drops e ouro ---------------------------------------------------------

    ''' <summary>Linha "*아이템 ... 돈 ..." (primeira com ouro) ou -1.</summary>
    Private Function GoldLineIndex(ByVal monster As MonsterFile) As Integer
        For i = 0 To monster.Lines.Count - 1
            Dim line = monster.Lines(i)
            If line.StartsWith(GameFields.Monster.Drop) AndAlso line.Contains(GameFields.Monster.DropGold) Then Return i
        Next
        Return -1
    End Function

    ''' <summary>Texto apos "돈" na linha de drop com ouro (ex.: "500 1000"); "" se nao houver.</summary>
    Friend Function GetGold(ByVal monster As MonsterFile) As String
        Dim index As Integer = GoldLineIndex(monster)
        If index < 0 Then Return ""
        Dim line = monster.Lines(index)
        Dim pos As Integer = line.IndexOf(GameFields.Monster.DropGold)
        Return line.Substring(pos + GameFields.Monster.DropGold.Length).Trim(ChrW(9))
    End Function

    ''' <summary>Substitui tudo apos "돈" na linha de drop com ouro por "[TAB]valor".</summary>
    Friend Sub SetGold(ByVal monster As MonsterFile, ByVal value As String)
        Dim index As Integer = GoldLineIndex(monster)
        If index < 0 Then Return
        Dim line = monster.Lines(index)
        Dim pos As Integer = line.IndexOf(GameFields.Monster.DropGold)
        monster.Lines(index) = line.Substring(0, pos + GameFields.Monster.DropGold.Length) & vbTab & value
    End Sub

    ''' <summary>True se alguma quantidade de ouro nas linhas de drop supera o maximo.</summary>
    Friend Function HasGoldAbove(ByVal monster As MonsterFile, ByVal maxGold As Long) As Boolean
        For Each line In monster.Lines
            If Not (line.StartsWith(GameFields.Monster.Drop) AndAlso line.Contains(GameFields.Monster.DropGold)) Then Continue For
            Dim tokens = line.Substring(GameFields.Monster.Drop.Length).Trim(ChrW(9)).Replace(ChrW(9), " "c).Split(" "c)
            For i = 2 To tokens.Length - 1
                Dim amount As Long
                If Not tokens(i).Contains(GameFields.Monster.DropGold) AndAlso Long.TryParse(tokens(i), amount) AndAlso amount > maxGold Then
                    Return True
                End If
            Next
        Next
        Return False
    End Function

    ''' <summary>True se o valor da ultima linha com a chave supera o maximo (ex.: EXP).</summary>
    Friend Function HasValueAbove(ByVal monster As MonsterFile, ByVal key As String, ByVal maximum As Long) As Boolean
        Dim found As Boolean = False
        Dim above As Boolean = False
        For Each line In monster.Lines
            If line.StartsWith(key) Then
                Dim value As Long
                Long.TryParse(line.Substring(key.Length).Trim(), value)
                found = True
                above = value > maximum
            End If
        Next
        Return found AndAlso above
    End Function

    ''' <summary>True se alguma linha de drop (ou drop extra) contem o codigo do item (sem distinguir maiusculas).</summary>
    Friend Function DropsItem(ByVal monster As MonsterFile, ByVal itemCode As String) As Boolean
        Dim code As String = itemCode.Trim(""""c).ToUpper()
        For Each line In monster.Lines
            If line.StartsWith(GameFields.Monster.Drop) AndAlso line.ToUpper().Contains(code) Then Return True
        Next
        Return False
    End Function

    ''' <summary>Remove o codigo das linhas de drop/drop extra, preservando o restante da linha.</summary>
    Friend Sub RemoveItem(ByVal monster As MonsterFile, ByVal itemCode As String)
        Dim pattern As String = Regex.Escape(itemCode.Trim(""""c))
        For i = 0 To monster.Lines.Count - 1
            Dim line = monster.Lines(i)
            If (line.StartsWith(GameFields.Monster.Drop) OrElse line.StartsWith(GameFields.Monster.ExtraDrop)) _
               AndAlso line.ToUpper().Contains(itemCode.ToUpper()) Then
                monster.Lines(i) = Regex.Replace(line, pattern, "", RegexOptions.IgnoreCase)
            End If
        Next
    End Sub

    ''' <summary>Troca o valor de *아이템카운터 (quantidade de drops).</summary>
    Friend Sub SetDropCount(ByVal monster As MonsterFile, ByVal count As Long)
        For i = 0 To monster.Lines.Count - 1
            If IsDropCounterLine(monster.Lines(i)) Then
                monster.Lines(i) = GameFields.Monster.DropCounter & vbTab & count
            End If
        Next
    End Sub

    Friend Function IsDropCounterLine(ByVal line As String) As Boolean
        Return line.StartsWith(GameFields.Monster.DropCounter)
    End Function

    ''' <summary>Uma entrada de drop como exibida no editor.</summary>
    Friend Class DropEntry
        Public Property LineIndex As Integer
        ''' <summary>True para "*추가아이템": exibida mas nao regravada pelo editor.</summary>
        Public Property IsExtra As Boolean
        Public Property Rate As String = ""
        Public Property Items As String = ""
        ''' <summary>Valores como lidos do arquivo; linha so e reescrita se o usuario mudou algo.</summary>
        Public Property OriginalRate As String = ""
        Public Property OriginalItems As String = ""

        Public ReadOnly Property Changed As Boolean
            Get
                Return Rate <> OriginalRate OrElse Items <> OriginalItems
            End Get
        End Property
    End Class

    ''' <summary>Drops na ordem do arquivo (sem *아이템카운터 e *아이템모두). Gold/Nothing ja traduzidos.</summary>
    Friend Function GetDrops(ByVal monster As MonsterFile) As List(Of DropEntry)
        Dim result As New List(Of DropEntry)()
        For i = 0 To monster.Lines.Count - 1
            Dim line = monster.Lines(i)
            If line.StartsWith(GameFields.Monster.DropAll) OrElse IsDropCounterLine(line) Then Continue For
            Dim entry As DropEntry
            If line.StartsWith(GameFields.Monster.ExtraDrop) Then
                entry = ParseDrop(line.Substring(GameFields.Monster.ExtraDrop.Length))
                entry.IsExtra = True
            ElseIf line.Contains(GameFields.Monster.Drop) Then
                entry = ParseDrop(line.Substring(GameFields.Monster.Drop.Length))
            Else
                Continue For
            End If
            entry.LineIndex = i
            result.Add(entry)
        Next
        Return result
    End Function

    Private Function ParseDrop(ByVal value As String) As DropEntry
        Dim entry As New DropEntry()
        Dim tokens = value.Trim().Replace(ChrW(9), " "c).Split(" "c)
        entry.Rate = tokens(0)
        Dim items As New List(Of String)()
        For i = 1 To tokens.Length - 1
            Dim token = tokens(i)
            If token = GameFields.Monster.DropNothing Then token = "Nothing"
            If token = GameFields.Monster.DropGold Then token = "Gold"
            If token <> "" Then items.Add(token)
        Next
        entry.Items = String.Join(" ", items)
        entry.OriginalRate = entry.Rate
        entry.OriginalItems = entry.Items
        Return entry
    End Function

    ''' <summary>Regrava as linhas "*아이템" alteradas (ordem do arquivo). Entrada vazia apaga a linha.</summary>
    Friend Sub SetDrops(ByVal monster As MonsterFile, ByVal entries As IList(Of DropEntry))
        For Each entry In entries
            If entry.IsExtra OrElse Not entry.Changed Then Continue For
            Dim line As String
            If entry.Rate.Length = 0 AndAlso entry.Items.Length = 0 Then
                line = ""
            Else
                line = GameFields.Monster.Drop & vbTab & entry.Rate & " " & entry.Items
                line = line.Replace("Gold", GameFields.Monster.DropGold).Replace("Nothing", GameFields.Monster.DropNothing)
            End If
            monster.Lines(entry.LineIndex) = line
        Next
    End Sub

    ''' <summary>Insere uma nova linha de drop ("*아이템 100[TAB]Free") depois da linha informada.</summary>
    Friend Sub InsertDropLine(ByVal monster As MonsterFile, ByVal afterLineIndex As Integer)
        monster.Lines.Insert(afterLineIndex + 1, GameFields.Monster.Drop & " 100" & vbTab & "Free")
    End Sub

    ' ---- Buscas -----------------------------------------------------------------

    ''' <summary>True se alguma linha com a chave contem o texto (sem distinguir maiusculas).</summary>
    Friend Function MatchesText(ByVal monster As MonsterFile, ByVal key As String, ByVal text As String) As Boolean
        Dim lower As String = text.ToLower()
        For Each line In monster.Lines
            If line.StartsWith(key) AndAlso line.ToLower().Contains(lower) Then Return True
        Next
        Return False
    End Function

    ''' <summary>
    ''' Filtro dos combos: "Level=10-20", "Level=>50", "Level=<10", "Level=60", "Name=Hopy,Fury" (curingas implicitos).
    ''' errorMessage recebe "No Filter found" ou "Check Filter" quando o filtro e invalido.
    ''' </summary>
    Friend Function Filter(ByVal filterText As String, ByRef errorMessage As String) As List(Of MonsterFile)
        Dim result As New List(Of MonsterFile)()
        errorMessage = Nothing
        If Not filterText.Contains("=") Then
            errorMessage = "No Filter found"
            Return result
        End If
        Dim text As String = filterText.ToUpper()
        Dim value As String = text.Substring(text.IndexOf("="c) + 1)
        Try
            If text.StartsWith("LEVEL=") OrElse text.StartsWith("LVL=") Then
                If value.Contains("-") Then
                    Dim parts = value.Split("-"c)
                    Dim low As Long = CLng(parts(0)), high As Long = CLng(parts(1))
                    result.AddRange(AppState.Monsters.Where(Function(m) GetLevel(m) >= low AndAlso GetLevel(m) <= high))
                ElseIf value.StartsWith(">") Then
                    Dim limit As Long = CLng(value.Substring(1))
                    result.AddRange(AppState.Monsters.Where(Function(m) GetLevel(m) > limit))
                ElseIf value.StartsWith("<") Then
                    Dim limit As Long = CLng(value.Substring(1))
                    result.AddRange(AppState.Monsters.Where(Function(m) GetLevel(m) < limit))
                Else
                    Dim level As Long = CLng(value)
                    result.AddRange(AppState.Monsters.Where(Function(m) GetLevel(m) = level))
                End If
            ElseIf text.StartsWith("NAME=") OrElse text.StartsWith("=") Then
                Dim patterns = value.Split(","c)
                For Each monster In AppState.Monsters
                    Dim name As String = monster.Name.Trim(""""c).ToUpper()
                    If patterns.Any(Function(p) name Like "*" & p & "*") Then result.Add(monster)
                Next
            End If
        Catch
            errorMessage = "Check Filter"
        End Try
        Return result
    End Function

    ''' <summary>Monstros em que alguma linha com a chave tem exatamente o valor numerico.</summary>
    Friend Function FindByValue(ByVal key As String, ByVal value As Long) As List(Of MonsterFile)
        Dim result As New List(Of MonsterFile)()
        For Each monster In AppState.Monsters
            For Each line In monster.Lines
                Dim number As Long
                If line.StartsWith(key) AndAlso Long.TryParse(line.Substring(key.Length).Trim(), number) AndAlso number = value Then
                    result.Add(monster)
                    Exit For
                End If
            Next
        Next
        Return result
    End Function

End Module
