Imports System.IO
Imports System.Text

''' <summary>Log de erros em &lt;exe&gt;\PTEditor\Logs\PTEditor_Errors.log.</summary>
Friend Module ErrorLog

    ''' <summary>Acima deste tamanho o log atual vira .old.log e um novo comeca (nao cresce sem limite).</summary>
    Private Const MaxLogBytes As Long = 262144

    Private ReadOnly _writeLock As New Object()

    ''' <summary>Bloco de informacoes do sistema, montado uma vez por sessao (OSFullName consulta WMI e e lento).</summary>
    Private ReadOnly _systemInfo As New Lazy(Of String)(
        Function()
            Dim sb As New StringBuilder()
            sb.AppendLine("Computer and System information: ")
            sb.AppendLine("OS: " & My.Computer.Info.OSFullName)
            sb.AppendLine("OSPlattform: " & My.Computer.Info.OSPlatform)
            sb.AppendLine("Culture name: " & My.Computer.Info.InstalledUICulture.TextInfo.CultureName)
            sb.AppendLine("Culture name: " & My.Computer.Info.InstalledUICulture.NativeName)
            sb.AppendLine("OEM Codepage: " & My.Computer.Info.InstalledUICulture.TextInfo.OEMCodePage)
            sb.AppendLine("ANSI Codepage: " & My.Computer.Info.InstalledUICulture.TextInfo.ANSICodePage)
            sb.AppendLine("Letter Code: " & My.Computer.Info.InstalledUICulture.ThreeLetterISOLanguageName)
            Return sb.ToString()
        End Function)

    Friend ReadOnly Property LogPath As String
        Get
            Return Path.Combine(AppState.LogsDir, AppState.ErrorLogFileName)
        End Get
    End Property

    ''' <summary>Grava uma entrada no log. Pode ser chamado de qualquer thread.</summary>
    Friend Sub Write(ByVal text As String)
        Try
            SyncLock _writeLock
                RotateIfTooBig()
                If Not File.Exists(LogPath) Then
                    Using writer As StreamWriter = File.CreateText(LogPath)
                        writer.WriteLine("PTEditor error log! Please post this file into forum on errors!")
                        writer.WriteLine("----------------------------------------------------------------------")
                    End Using
                End If
                Using writer As StreamWriter = File.AppendText(LogPath)
                    writer.WriteLine()
                    writer.WriteLine("----------------------------------------------------------------------")
                    writer.WriteLine()
                    writer.WriteLine(DateTime.Now.ToLongDateString() & " - " & DateTime.Now.ToLongTimeString())
                    writer.WriteLine("PTEditor Version:" & AppState.Version)
                    writer.WriteLine("Textencoding: " & Encoding.Default.EncodingName)
                    writer.WriteLine()
                    writer.WriteLine(_systemInfo.Value)
                    writer.WriteLine("Error text:")
                    writer.WriteLine(text)
                End Using
            End SyncLock
        Catch
            ' Sem log disponivel: nao ha mais nada a fazer.
        End Try
    End Sub

    ''' <summary>Se o log passou do limite, guarda-o como .old.log (substituindo o anterior) e recomeca.</summary>
    Private Sub RotateIfTooBig()
        Dim info As New FileInfo(LogPath)
        If Not info.Exists OrElse info.Length <= MaxLogBytes Then Return
        Dim oldPath As String = Path.Combine(Path.GetDirectoryName(LogPath),
                                             Path.GetFileNameWithoutExtension(LogPath) & ".old.log")
        If File.Exists(oldPath) Then File.Delete(oldPath)
        File.Move(LogPath, oldPath)
    End Sub

    ''' <summary>Grava a excecao no log e avisa o usuario.</summary>
    Friend Sub Report(ByVal context As String, ByVal ex As Exception)
        Dim site As String = If(ex.TargetSite Is Nothing, "", ex.TargetSite.Name)
        Write(context & vbCrLf & site & vbCrLf & ex.Message & vbCrLf & ex.Source & vbCrLf & ex.StackTrace)
        MsgBox("Error: please check the error log!" & vbCrLf & LogPath, MsgBoxStyle.Critical)
    End Sub

End Module
