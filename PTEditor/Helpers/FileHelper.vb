Imports System.IO

''' <summary>Leitura/gravacao de arquivos do GameServer com o encoding do programa.</summary>
Friend Module FileHelper

    ''' <summary>Nomes (sem pasta) dos arquivos que casam com o padrao. Pasta inexistente retorna lista vazia.</summary>
    Friend Function ListFiles(ByVal folder As String, ByVal pattern As String) As List(Of String)
        Dim result As New List(Of String)()
        If Directory.Exists(folder) Then
            For Each f In Directory.GetFiles(folder, pattern)
                result.Add(Path.GetFileName(f))
            Next
        End If
        Return result
    End Function

    Friend Function ReadLines(ByVal path As String) As List(Of String)
        Dim trailing As Boolean
        Return ReadLines(path, trailing)
    End Function

    ''' <summary>Le as linhas e informa se o arquivo terminava com quebra de linha.</summary>
    Friend Function ReadLines(ByVal path As String, ByRef trailingNewline As Boolean) As List(Of String)
        Dim lines As New List(Of String)()
        Dim text As String = File.ReadAllText(path, AppState.FileEncoding)
        trailingNewline = text.Length = 0 OrElse text.EndsWith(vbLf) OrElse text.EndsWith(vbCr)
        Using reader As New StringReader(text)
            Do While reader.Peek() <> -1
                lines.Add(reader.ReadLine())
            Loop
        End Using
        Return lines
    End Function

    Friend Sub WriteLines(ByVal path As String, ByVal lines As IEnumerable(Of String), Optional ByVal trailingNewline As Boolean = True)
        Using writer As New StreamWriter(path, False, AppState.FileEncoding)
            Dim first As Boolean = True
            For Each line In lines
                If Not first Then writer.Write(vbCrLf)
                writer.Write(line)
                first = False
            Next
            If trailingNewline AndAlso Not first Then writer.Write(vbCrLf)
        End Using
    End Sub

    ''' <summary>Grava bytes a partir do offset sem alterar o tamanho do arquivo.</summary>
    Friend Sub WriteBytes(ByVal path As String, ByVal offset As Integer, ByVal data As Byte(), ByVal count As Integer)
        Using stream As New FileStream(path, FileMode.Open, FileAccess.ReadWrite)
            stream.Position = offset
            stream.Write(data, 0, count)
        End Using
    End Sub

    ''' <summary>Copia um trecho de um buffer em memoria.</summary>
    Friend Function Slice(ByVal source As Byte(), ByVal offset As Integer, ByVal count As Integer) As Byte()
        Dim buffer(count - 1) As Byte
        Array.Copy(source, offset, buffer, 0, count)
        Return buffer
    End Function

    ''' <summary>Texto fixo gravado em binario: preenche com ChrW(0) ate o tamanho.</summary>
    Friend Function FixedBytes(ByVal text As String, ByVal size As Integer) As Byte()
        Return AppState.FileEncoding.GetBytes(text.PadRight(size, ChrW(0)))
    End Function

    ''' <summary>Texto lido de binario: corta no primeiro ChrW(0).</summary>
    Friend Function FixedText(ByVal data As Byte(), ByVal offset As Integer, ByVal size As Integer) As String
        Dim text As String = AppState.FileEncoding.GetString(data, offset, size)
        Dim zero As Integer = text.IndexOf(ChrW(0))
        Return If(zero < 0, text, text.Substring(0, zero))
    End Function

    ''' <summary>Texto entre o primeiro par de delimitadores (ex.: aspas). Retorna "" se nao houver.</summary>
    Friend Function Between(ByVal text As String, ByVal delimiter As Char) As String
        Dim first As Integer = text.IndexOf(delimiter)
        If first < 0 Then Return ""
        Dim second As Integer = text.IndexOf(delimiter, first + 1)
        If second < 0 Then Return text.Substring(first + 1)
        Return text.Substring(first + 1, second - first - 1)
    End Function

End Module
