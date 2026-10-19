Imports System.IO
Imports System.Text

''' <summary>Log de erros em &lt;exe&gt;\PTEditor\Logs\PTEditor_Errors.log.</summary>
Friend Module ErrorLog

    Friend ReadOnly Property LogPath As String
        Get
            Return Path.Combine(AppState.LogsDir, AppState.ErrorLogFileName)
        End Get
    End Property

    ''' <summary>Grava uma entrada no log.</summary>
    Friend Sub Write(ByVal text As String)
        Try
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
                writer.WriteLine("Computer and System information: ")
                writer.WriteLine("OS: " & My.Computer.Info.OSFullName)
                writer.WriteLine("OSPlattform: " & My.Computer.Info.OSPlatform)
                writer.WriteLine("Culture name: " & My.Computer.Info.InstalledUICulture.TextInfo.CultureName)
                writer.WriteLine("Culture name: " & My.Computer.Info.InstalledUICulture.NativeName)
                writer.WriteLine("OEM Codepage: " & My.Computer.Info.InstalledUICulture.TextInfo.OEMCodePage)
                writer.WriteLine("ANSI Codepage: " & My.Computer.Info.InstalledUICulture.TextInfo.ANSICodePage)
                writer.WriteLine("Letter Code: " & My.Computer.Info.InstalledUICulture.ThreeLetterISOLanguageName)
                writer.WriteLine("Network available: " & My.Computer.Network.IsAvailable)
                writer.WriteLine()
                writer.WriteLine("Error text:")
                writer.WriteLine(text)
            End Using
        Catch
            ' Sem log disponivel: nao ha mais nada a fazer.
        End Try
    End Sub

    ''' <summary>Grava a excecao no log e avisa o usuario.</summary>
    Friend Sub Report(ByVal context As String, ByVal ex As Exception)
        Dim site As String = If(ex.TargetSite Is Nothing, "", ex.TargetSite.Name)
        Write(context & vbCrLf & site & vbCrLf & ex.Message & vbCrLf & ex.Source & vbCrLf & ex.StackTrace)
        MsgBox("Error: Please check errorlog!" & vbCrLf & LogPath, MsgBoxStyle.Critical)
    End Sub

End Module
