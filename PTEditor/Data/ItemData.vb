Imports System.IO

''' <summary>Carga, consulta e gravacao dos itens (GameServer\OpenItem\*.txt).</summary>
Friend Module ItemData

    Friend Sub LoadAll(ByVal warnMissingZhoon As Boolean, Optional ByVal progress As Action(Of Integer, Integer) = Nothing)
        AppState.Items.Clear()
        Dim files = FileHelper.ListFiles(AppState.ServerPath & AppState.ItemFolder, AppState.ItemPattern)
        For i = 0 To files.Count - 1
            Dim item As New ItemFile(files(i))
            item.LoadFrom(AppState.ItemPath(item.FileName))
            RefreshInfo(item, warnMissingZhoon)
            AppState.Items.Add(item)
            If progress IsNot Nothing Then progress(i + 1, files.Count)
        Next
    End Sub

    ''' <summary>Carrega um backup (OpenItem\Backup\nome) para a memoria, sem gravar.</summary>
    Friend Sub LoadBackup(ByVal item As ItemFile, ByVal backupFileName As String)
        Try
            item.LoadFrom(BackupHelper.BackupPath(AppState.ServerPath & AppState.ItemFolder, backupFileName))
            RefreshInfo(item, My.Settings.WarnMoZhoon)
        Catch ex As Exception
            ErrorLog.Report("ItemData.LoadBackup: " & backupFileName, ex)
        End Try
    End Sub

    Friend Sub Save(ByVal item As ItemFile, ByVal makeBackup As Boolean)
        Try
            If makeBackup Then BackupHelper.CreateBackup(AppState.ServerPath & AppState.ItemFolder, item.FileName)
            item.SaveTo(AppState.ItemPath(item.FileName))
        Catch ex As Exception
            ErrorLog.Report("ItemData.Save: " & item.FileName, ex)
        End Try
    End Sub

    Friend Function Find(ByVal fileName As String) As ItemFile
        For Each i In AppState.Items
            If i.FileName = fileName Then Return i
        Next
        Return Nothing
    End Function

    ''' <summary>Item cujo codigo (com aspas, maiusculas) e igual ao informado.</summary>
    Friend Function FindByCode(ByVal quotedUpperCode As String) As ItemFile
        For Each i In AppState.Items
            If i.Code = quotedUpperCode Then Return i
        Next
        Return Nothing
    End Function

    ''' <summary>Itens cujo nome in-game casa com o padrao (curingas ? e *), sem distinguir maiusculas.</summary>
    Friend Function SearchByName(ByVal pattern As String) As List(Of ItemFile)
        Dim result As New List(Of ItemFile)()
        For Each i In AppState.Items
            If i.Name.ToUpper() Like pattern.ToUpper() Then result.Add(i)
        Next
        Return result
    End Function

    ''' <summary>Valor apos a chave, com tabs convertidos em espacos (formato usado nos campos de item).</summary>
    Friend Function GetValue(ByVal line As String, ByVal key As String) As String
        Return line.Substring(key.Length).Trim().Replace(ChrW(9), " "c)
    End Function

    Private Sub RefreshInfo(ByVal item As ItemFile, ByVal warnMissingZhoon As Boolean)
        item.Name = ItemFile.NoName
        item.ZhoonFile = ""
        For Each line In item.Lines
            If line.Contains(GameFields.LinkedFile) Then
                ReadZhoon(item, line.Substring(GameFields.LinkedFile.Length).Trim().Trim(""""c), warnMissingZhoon)
            End If
            If line.StartsWith(GameFields.Item.Code) Then item.Code = GetValue(line, GameFields.Item.Code).ToUpper()
            If line.StartsWith(GameFields.Item.Weight) Then item.Weight = GetValue(line, GameFields.Item.Weight)
            If line.StartsWith(GameFields.Level) Then item.Level = GetValue(line, GameFields.Level)
        Next
    End Sub

    ''' <summary>Nome in-game: linha "*X_NAME" do .zhoon referenciado em *연결파일.</summary>
    Private Sub ReadZhoon(ByVal item As ItemFile, ByVal zhoonReference As String, ByVal warnMissing As Boolean)
        Dim path As String = If(zhoonReference.Contains(":"), zhoonReference, AppState.ItemPath(zhoonReference))
        If Not File.Exists(path) Then
            item.ZhoonFile = ItemFile.NoZhoon
            If warnMissing Then MsgBox("No .zhoon File Found! " & path)
            ErrorLog.Write("In ItemData.LoadAll: Warn" & vbCrLf & "No .zhoon file found! " & vbCrLf &
                           "Itemfile: " & item.FileName & vbCrLf & "Item zhoon Path: " & path)
            Return
        End If
        item.ZhoonFile = path
        Dim name As String = ZhoonFile.ReadName(path)
        If Not String.IsNullOrEmpty(name) Then item.Name = name
    End Sub

End Module
