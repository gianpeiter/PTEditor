Imports System.Diagnostics

Friend Module ProcessHelper

    ''' <summary>Abre um arquivo ou pasta com o programa informado (editor de texto ou Explorer).</summary>
    Friend Sub Open(ByVal program As String, ByVal fileOrFolder As String)
        Try
            Process.Start(program, """" & fileOrFolder & """")
        Catch ex As Exception
            ErrorLog.Report("ProcessHelper.Open" & vbCrLf & "Program:" & program & vbCrLf & "Path:" & fileOrFolder, ex)
        End Try
    End Sub

End Module
