Imports System.IO

''' <summary>PTEditor.Config.txt (ao lado do executavel): filtros dos combos e opcoes gerais.</summary>
Friend Module ConfigFile

    Private Const ItemSelectorSection As String = "ItemSelector:"
    Private Const MonsterSelectorSection As String = "MonsterSelector:"
    Private Const SectionEnd As String = "<End>"

    Friend Const KeyServerPath As String = "Server-Path="
    Friend Const KeyMaxExp As String = "Max-Exp="
    Friend Const KeyMaxGold As String = "Max-Gold="
    Friend Const KeyTextEditor As String = "Text-Editor="
    Friend Const KeyDropWarnings As String = "Drop-warnings="
    Friend Const KeyWarnZhoon As String = "Monster.zhoon not found="
    Friend Const KeyFindMaps As String = "Monstereditor: Find maps="
    Friend Const KeyFindItems As String = "Monstereditor: Find Items="
    ''' <summary>Letra do idioma dos .zhoon (*B_NAME -> "B"). Ausente = auto-detectar.</summary>
    Friend Const KeyNameTag As String = "Name-Tag="

    Friend Class ConfigData
        Public ReadOnly Property ItemSelectors As New List(Of String)()
        Public ReadOnly Property MonsterSelectors As New List(Of String)()
        ''' <summary>Chaves simples presentes no arquivo (valor ja validado). Ausentes nao constam.</summary>
        Public ReadOnly Property Values As New Dictionary(Of String, String)()
    End Class

    ''' <summary>Cria o arquivo padrao (conteudo de My.Settings.Config) se nao existir. Retorna True se criou.</summary>
    Friend Function EnsureExists() As Boolean
        If File.Exists(AppState.ConfigFilePath) Then Return False
        Using writer As New StreamWriter(AppState.ConfigFilePath, False)
            writer.WriteLine(My.Settings.Config)
        End Using
        Return True
    End Function

    Friend Sub WriteDefault()
        Using writer As New StreamWriter(AppState.ConfigFilePath, False)
            writer.WriteLine(My.Settings.Config)
        End Using
    End Sub

    ''' <summary>Le o arquivo. Valores invalidos sao avisados, registrados no log e ignorados.</summary>
    Friend Function Load() As ConfigData
        Dim data As New ConfigData()
        Using reader As New StreamReader(AppState.ConfigFilePath)
            Do While reader.Peek() <> -1
                Dim item As String = reader.ReadLine()
                If item.ToUpper().StartsWith(ItemSelectorSection.ToUpper()) Then
                    Do While item <> SectionEnd AndAlso reader.Peek() <> -1
                        item = reader.ReadLine()
                        If item <> SectionEnd Then data.ItemSelectors.Add(item)
                    Loop
                End If
                If item.ToUpper().StartsWith(MonsterSelectorSection.ToUpper()) Then
                    Do While item <> SectionEnd AndAlso reader.Peek() <> -1
                        item = reader.ReadLine()
                        If item <> SectionEnd Then data.MonsterSelectors.Add(item)
                    Loop
                End If
                ReadValue(data, item, KeyServerPath)
                ReadNumeric(data, item, KeyMaxExp)
                ReadNumeric(data, item, KeyMaxGold)
                ReadEditor(data, item)
                ReadValue(data, item, KeyDropWarnings)
                ReadBoolean(data, item, KeyWarnZhoon)
                ReadBoolean(data, item, KeyFindMaps)
                ReadBoolean(data, item, KeyFindItems)
                ReadNameTag(data, item)
            Loop
        End Using
        Return data
    End Function

    Private Function ValueOf(ByVal line As String, ByVal key As String) As String
        Return line.Substring(key.Length).Trim()
    End Function

    Private Function HasKey(ByVal line As String, ByVal key As String) As Boolean
        Return line.ToUpper().StartsWith(key.ToUpper())
    End Function

    Private Sub ReadValue(ByVal data As ConfigData, ByVal line As String, ByVal key As String)
        If HasKey(line, key) Then data.Values(key) = ValueOf(line, key)
    End Sub

    Private Sub ReadNumeric(ByVal data As ConfigData, ByVal line As String, ByVal key As String)
        If Not HasKey(line, key) Then Return
        Dim value As String = ValueOf(line, key)
        If IsNumeric(value) Then
            data.Values(key) = value
        Else
            Warn("Error in loading config file at command: <" & key & ">" & vbCrLf & "Not numberic", value)
        End If
    End Sub

    Private Sub ReadEditor(ByVal data As ConfigData, ByVal line As String)
        If Not HasKey(line, KeyTextEditor) Then Return
        Dim value As String = ValueOf(line, KeyTextEditor)
        If File.Exists(value) Then
            data.Values(KeyTextEditor) = value
        Else
            Warn("Error in loading config file at command: <" & KeyTextEditor & ">" & vbCrLf & "Programm not found", value)
        End If
    End Sub

    Private Sub ReadBoolean(ByVal data As ConfigData, ByVal line As String, ByVal key As String)
        If Not HasKey(line, key) Then Return
        Dim value As String = ValueOf(line, key).ToUpper()
        If value = "TRUE" OrElse value = "FALSE" Then
            data.Values(key) = value
        Else
            Warn("Error in loading config file at command: <" & key & ">", value)
        End If
    End Sub

    Private Sub ReadNameTag(ByVal data As ConfigData, ByVal line As String)
        If Not HasKey(line, KeyNameTag) Then Return
        Dim value As String = ValueOf(line, KeyNameTag)
        If value = "" Then Return
        If value.Length = 1 AndAlso Char.IsLetter(value(0)) Then
            data.Values(KeyNameTag) = value
        Else
            Warn("Error in loading config file at command: <" & KeyNameTag & ">" & vbCrLf & "Expected one letter (ex.: B for *B_NAME)", value)
        End If
    End Sub

    Private Sub Warn(ByVal message As String, ByVal value As String)
        MsgBox(message)
        ErrorLog.Write(message & ": " & value)
    End Sub

    ''' <summary>Atualiza (ou acrescenta) as chaves simples, preservando o restante do arquivo.</summary>
    Friend Sub SaveValues(ByVal values As IDictionary(Of String, String))
        Dim lines As New List(Of String)(File.ReadAllLines(AppState.ConfigFilePath, AppState.FileEncoding))
        For Each configKey In values.Keys
            If Not lines.Any(Function(l) HasKey(l, configKey)) Then lines.Add(configKey)
        Next
        For i = 0 To lines.Count - 1
            For Each pair In values
                If HasKey(lines(i), pair.Key) Then lines(i) = pair.Key & pair.Value
            Next
        Next
        Using writer As New StreamWriter(AppState.ConfigFilePath, False)
            For Each line In lines
                writer.WriteLine(line)
            Next
        End Using
    End Sub

    ''' <summary>Substitui a secao ItemSelector pelos filtros informados.</summary>
    Friend Sub SaveItemSelectors(ByVal filters As IEnumerable(Of String))
        Dim content As String = ""
        For Each line In FileHelper.ReadLines(AppState.ConfigFilePath)
            content &= vbCrLf & line
        Next
        Dim start As Integer = content.ToUpper().IndexOf(ItemSelectorSection.ToUpper()) + ItemSelectorSection.Length
        Dim count As Integer = content.ToUpper().IndexOf(SectionEnd.ToUpper()) - start
        content = content.Remove(start, count)
        Dim section As String = ""
        For Each f In filters
            section &= vbCrLf & f
        Next
        content = content.Insert(start, section & vbCrLf)
        File.WriteAllText(AppState.ConfigFilePath, content, AppState.FileEncoding)
    End Sub

End Module
