Imports System.IO

''' <summary>
''' Compressao dos itens nos .dat/.war (EecodeCompress/DecodeCompress em Servidor\playsub.cpp).
''' Bloco = Int32 com o tamanho total do bloco (incluindo esses 4 bytes) seguido de sequencias:
''' byte com bit 7 ligado = (b And &amp;H7F) bytes zero; senao b bytes literais em seguida.
''' </summary>
Friend Module ItemCompression

    Private Const ZeroRunFlag As Integer = &H80
    Private Const MaxRun As Integer = &H7F

    ''' <summary>Tamanho do bloco que comeca em offset (primeiro Int32).</summary>
    Friend Function BlockLength(ByVal source As Byte(), ByVal offset As Integer) As Integer
        Return BitConverter.ToInt32(source, offset)
    End Function

    ''' <summary>
    ''' Descomprime o bloco em offset para um buffer de outputSize bytes (o restante fica zerado).
    ''' decodedLength recebe quantos bytes o bloco realmente produziu.
    ''' </summary>
    Friend Function Decode(ByVal source As Byte(), ByVal offset As Integer, ByVal outputSize As Integer,
                           Optional ByRef decodedLength As Integer = 0) As Byte()
        Dim length As Integer = BlockLength(source, offset)
        If length < 4 OrElse offset + length > source.Length Then
            Throw New InvalidDataException("Invalid compressed block at offset 0x" & offset.ToString("X") &
                                           " (size " & length & ").")
        End If
        Dim output(outputSize - 1) As Byte
        Dim written As Integer = 0
        Dim i As Integer = offset + 4
        Dim blockEnd As Integer = offset + length
        Do While i < blockEnd
            Dim b As Integer = source(i)
            i += 1
            Dim count As Integer = b And MaxRun
            If (b And ZeroRunFlag) <> 0 Then
                written += count
            Else
                If i + count > blockEnd Then
                    Throw New InvalidDataException("Truncated compressed block at offset 0x" & offset.ToString("X") & ".")
                End If
                Dim take As Integer = Math.Max(0, Math.Min(count, outputSize - written))
                If take > 0 Then Array.Copy(source, i, output, written, take)
                written += count
                i += count
            End If
        Loop
        decodedLength = written
        Return output
    End Function

    ''' <summary>Comprime exatamente como EecodeCompress (bytes identicos aos gravados pelo jogo).</summary>
    Friend Function Encode(ByVal source As Byte()) As Byte()
        Dim size As Integer = source.Length
        Dim last As Integer = size - 1
        Dim output As New List(Of Byte)(size \ 2 + 8) From {0, 0, 0, 0}
        Dim counter As Integer = 0
        Do While counter < size
            Dim c1 As Byte = source(counter)
            Dim c2 As Byte = If(counter < last, source(counter + 1), c1)
            If c1 = 0 AndAlso c2 = 0 Then
                Dim count As Integer = 0
                Do While count < MaxRun AndAlso counter < size AndAlso source(counter) = 0
                    counter += 1
                    count += 1
                Loop
                output.Add(CByte(count Or ZeroRunFlag))
            Else
                Dim countIndex As Integer = output.Count
                output.Add(0)
                Dim count As Integer = 0
                Do While count < MaxRun AndAlso counter < size
                    c1 = source(counter)
                    c2 = If(counter < last, source(counter + 1), c1)
                    If c1 = 0 AndAlso c2 = 0 Then Exit Do
                    output.Add(c1)
                    counter += 1
                    count += 1
                Loop
                output(countIndex) = CByte(count)
            End If
        Loop
        Dim result As Byte() = output.ToArray()
        BitConverter.GetBytes(result.Length).CopyTo(result, 0)
        Return result
    End Function

End Module
