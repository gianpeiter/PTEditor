''' <summary>Mapas de spawn (GameServer\Field\*.spm).</summary>
Friend Module MapData

    Friend Sub LoadAll(Optional ByVal progress As Action(Of Integer, Integer) = Nothing)
        AppState.Maps.Clear()
        Dim files = FileHelper.ListFiles(AppState.ServerPath & AppState.FieldFolder, AppState.MapPattern)
        For i = 0 To files.Count - 1
            Dim map As New MapFile(files(i))
            map.LoadFrom(AppState.FieldPath(map.FileName))
            AppState.Maps.Add(map)
            If progress IsNot Nothing Then progress(i + 1, files.Count)
        Next
    End Sub

    Friend Sub Reload(ByVal map As MapFile)
        map.LoadFrom(AppState.FieldPath(map.FileName))
    End Sub

    Friend Function Find(ByVal fileName As String) As MapFile
        For Each m In AppState.Maps
            If m.FileName = fileName Then Return m
        Next
        Return Nothing
    End Function

    ''' <summary>Valor de uma chave simples (*최대동시출현수 etc.), com tabs convertidos em espacos.</summary>
    Friend Function GetValue(ByVal map As MapFile, ByVal key As String) As String
        Dim result As String = ""
        For Each line In map.Lines
            If line.StartsWith(key) Then result = line.Substring(key.Length).Trim(ChrW(9)).Replace(ChrW(9), " "c)
        Next
        Return result
    End Function

    ''' <summary>Linha "*출연자 "nome" rate".</summary>
    Friend Class SpawnEntry
        Public Property MapName As String = ""
        Public Property Rate As String = ""
    End Class

    ''' <summary>Linha "*출연자두목 "chefe" "guarda" quantidade horarios".</summary>
    Friend Class BossEntry
        Public Property BossMapName As String = ""
        Public Property GuardMapName As String = ""
        Public Property GuardCount As String = ""
        Public Property SpawnTimes As String = ""
    End Class

    ''' <summary>Le o proximo token entre aspas a partir de position; position avanca para depois da aspa final.</summary>
    Private Function NextQuoted(ByVal text As String, ByRef position As Integer) As String
        Dim open As Integer = text.IndexOf(""""c, position)
        If open < 0 Then
            position = text.Length
            Return ""
        End If
        Dim close As Integer = text.IndexOf(""""c, open + 1)
        If close < 0 Then
            position = text.Length
            Return text.Substring(open + 1)
        End If
        position = close + 1
        Return text.Substring(open + 1, close - open - 1)
    End Function

    ''' <summary>Linhas "*출연자 "nome" rate" (as de chefe, "*출연자두목", ficam em GetBosses).</summary>
    Friend Function GetSpawns(ByVal map As MapFile) As List(Of SpawnEntry)
        Dim result As New List(Of SpawnEntry)()
        For Each line In map.Lines
            If Not line.StartsWith(GameFields.Map.Spawn) OrElse line.StartsWith(GameFields.Map.BossSpawn) Then Continue For
            Dim entry As New SpawnEntry()
            Dim position As Integer = GameFields.Map.Spawn.Length
            entry.MapName = NextQuoted(line, position)
            entry.Rate = line.Substring(position).Trim()
            result.Add(entry)
        Next
        Return result
    End Function

    ''' <summary>Linhas "*출연자두목 "chefe" "guarda" quantidade horarios".</summary>
    Friend Function GetBosses(ByVal map As MapFile) As List(Of BossEntry)
        Dim result As New List(Of BossEntry)()
        For Each line In map.Lines
            If Not line.StartsWith(GameFields.Map.BossSpawn) Then Continue For
            Dim entry As New BossEntry()
            Dim position As Integer = GameFields.Map.BossSpawn.Length
            entry.BossMapName = NextQuoted(line, position)
            entry.GuardMapName = NextQuoted(line, position)
            Dim rest As String = line.Substring(position).Replace(ChrW(9), " "c).Trim()
            Dim space As Integer = rest.IndexOf(" "c)
            If space < 0 Then
                entry.GuardCount = rest
            Else
                entry.GuardCount = rest.Substring(0, space)
                entry.SpawnTimes = rest.Substring(space).Trim()
            End If
            result.Add(entry)
        Next
        Return result
    End Function

    ''' <summary>Regrava o .spm inteiro com os valores e listas informados e recarrega a memoria.</summary>
    Friend Sub Save(ByVal map As MapFile, ByVal maxSpawn As String, ByVal spawnInterval As String, ByVal spawnCount As String,
                    ByVal spawns As IEnumerable(Of SpawnEntry), ByVal bosses As IEnumerable(Of BossEntry))
        Dim lines As New List(Of String)()
        lines.Add(GameFields.Map.HeaderTitle)
        lines.Add(GameFields.Map.MaxSpawn & vbTab & maxSpawn)
        lines.Add(GameFields.Map.HeaderInterval)
        lines.Add(GameFields.Map.SpawnInterval & vbTab & spawnInterval)
        lines.Add(GameFields.Map.SpawnCount & vbTab & spawnCount & vbCrLf)
        lines.Add(GameFields.Map.HeaderColumns)

        Dim spawnText As New System.Text.StringBuilder()
        For Each s In spawns
            spawnText.Append(GameFields.Map.Spawn & vbTab & """" & s.MapName & """" & vbTab & vbTab & s.Rate & vbCrLf)
        Next
        lines.Add(spawnText.ToString())

        Dim bossText As New System.Text.StringBuilder()
        For Each b In bosses
            bossText.Append(GameFields.Map.BossSpawn & vbTab & """" & b.BossMapName & """" & vbTab & """" & b.GuardMapName & """" &
                            vbTab & b.GuardCount & vbTab & b.SpawnTimes & vbCrLf)
        Next
        lines.Add(bossText.ToString())

        FileHelper.WriteLines(AppState.FieldPath(map.FileName), lines)
        Reload(map)
    End Sub

End Module
