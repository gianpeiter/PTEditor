''' <summary>
''' Subpasta (0..255) onde o servidor guarda os arquivos de um nome: GetUserCode em Servidor\record.cpp.
''' userdata\&lt;code(personagem)&gt;\, userinfo\&lt;code(conta)&gt;\ e warehouse\&lt;code(conta)&gt;\.
''' </summary>
Friend Module UserCode

    ''' <summary>Soma dos bytes do nome (a..z contados como maiusculas), modulo 256.</summary>
    Friend Function Compute(ByVal name As String) As Integer
        Dim sum As Integer = 0
        For Each b In AppState.FileEncoding.GetBytes(name)
            sum += If(b >= &H61 AndAlso b <= &H7A, b - &H20, CInt(b))
        Next
        Return sum And &HFF
    End Function

End Module
