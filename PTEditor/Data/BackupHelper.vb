Imports System.IO

''' <summary>
''' Backups em &lt;pasta&gt;\Backup\: o primeiro backup de cada arquivo vira "Default_&lt;nome&gt;" (original intocado);
''' os seguintes recebem data/hora + "_Backup_" no nome.
''' </summary>
Friend Module BackupHelper

    Friend Sub CreateBackup(ByVal folder As String, ByVal fileName As String)
        Dim source As String = Path.Combine(folder, fileName)
        If Not File.Exists(source) Then Return

        Dim backupDir As String = Path.Combine(folder, AppState.BackupFolder)
        Directory.CreateDirectory(backupDir)

        Dim defaultCopy As String = Path.Combine(backupDir, AppState.DefaultBackupPrefix & fileName)
        If Not File.Exists(defaultCopy) Then
            File.Copy(source, defaultCopy)
        Else
            Dim stamp As String = Format(Now, "yyyy_MM_dd_HH_mm_ss")
            File.Copy(source, Path.Combine(backupDir, stamp & AppState.BackupTag & fileName), True)
        End If
    End Sub

    ''' <summary>
    ''' Copia um arquivo do DataServer para &lt;DataServer&gt;\PTEditor_Backup\ com data/hora no nome, fora das
    ''' pastas que o servidor le. Retorna o caminho da copia.
    ''' </summary>
    Friend Function CreateDataServerBackup(ByVal dataServerPath As String, ByVal filePath As String) As String
        Dim backupDir As String = Path.Combine(dataServerPath, AppState.DataServerBackupFolder)
        Directory.CreateDirectory(backupDir)
        Dim target As String = Path.Combine(backupDir, Format(Now, "yyyy-MM-dd_HH-mm-ss") & "_" & Path.GetFileName(filePath))
        File.Copy(filePath, target, True)
        Return target
    End Function

    Friend Function BackupPath(ByVal folder As String, ByVal backupFileName As String) As String
        Return Path.Combine(folder, AppState.BackupFolder, backupFileName)
    End Function

    ''' <summary>Backups existentes para o arquivo (nome termina com o nome original).</summary>
    Friend Function ListBackups(ByVal folder As String, ByVal fileName As String, ByVal pattern As String) As List(Of String)
        Dim result As New List(Of String)()
        For Each f In FileHelper.ListFiles(Path.Combine(folder, AppState.BackupFolder), pattern)
            If f.EndsWith(fileName) Then result.Add(f)
        Next
        Return result
    End Function

    ''' <summary>Apaga os backups com data/hora (mantem os "Default_").</summary>
    Friend Sub DeleteBackups(ByVal folder As String, ByVal pattern As String, Optional ByVal fileName As String = Nothing)
        For Each f In FileHelper.ListFiles(Path.Combine(folder, AppState.BackupFolder), pattern)
            If f.Contains(AppState.BackupTag) AndAlso (fileName Is Nothing OrElse f.Contains(fileName)) Then
                File.Delete(BackupPath(folder, f))
            End If
        Next
    End Sub

End Module
