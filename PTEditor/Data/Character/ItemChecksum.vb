''' <summary>
''' Checksums gravados no cabecalho de cada item (sITEMINFO.ItemHeader). Os bytes entram como char com sinal (MSVC).
''' - dwChkSum = GetRefromItemCode (playsub.cpp): o cliente confere ao carregar o personagem e descarta o item se falhar.
''' - dwTime = rsGetItem_ServerForm (Gameplay\Items\ItemForm2.h, constantes do Brasil): o servidor confere ao
'''   vender/trocar/usar o item.
''' Os campos usados ficam antes de &amp;H1D4 e tem os mesmos offsets nos dois layouts.
''' </summary>
Friend Module ItemChecksum

    Private Const Mask As Long = &HFFFFFFFFL

    ' (offset, tamanho) dentro do sITEMINFO
    Private ReadOnly ClientFields As Integer(,) = {
        {&H18, 4}, {&H1C, 32}, {&H3C, 4}, {&H40, 4}, {&H4C, 16}, {&H64, 4}, {&H78, 4}, {&H7C, 4}, {&H80, 4},
        {&H68, 4}, {&H6C, 4}, {&H70, 4}, {&H74, 4}, {&H84, 4}, {&H88, 4}, {&H8C, 4}, {&H90, 4}, {&H94, 4},
        {&H98, 4}, {&H9C, 4}, {&HA0, 4}, {&HA4, 4}, {&HA8, 4}, {&HAC, 4}, {&HB0, 4}, {&HB4, 4}, {&HB8, 4},
        {&HBC, 4}, {&HC0, 4}, {&HC4, 4}, {&HC8, 4}, {&HCC, 4}, {&HF4, 4}, {&HF8, 220}, {&HE0, 4}, {&HE4, 4},
        {&HE8, 8}, {&HDC, 4}, {&HF0, 4}}

    Private ReadOnly ServerFields As Integer(,) = {
        {&H8C, 4}, {&H90, 4}, {&H94, 4}, {&H98, 4}, {&H9C, 4}, {&HA0, 4}, {&HA4, 4}, {&HC0, 4}, {&HC4, 4},
        {&HC8, 4}, {&HCC, 4}, {&HF4, 4}, {&HF8, 220}, {&HE0, 4}, {&HE4, 4}, {&HE8, 8}, {&HDC, 4}, {&HF0, 4},
        {&H3C, 4}, {&H40, 4}, {&H78, 4}, {&H7C, 4}, {&H80, 4}, {&H68, 4}, {&H6C, 4}, {&H70, 4}, {&H74, 4},
        {&H84, 4}, {&H88, 4}, {&H4C, 16}, {&H64, 4}, {&HBC, 4}, {&HA8, 4}, {&HAC, 4}, {&HB0, 4}, {&HB4, 4},
        {&HB8, 4}, {&H18, 4}, {&H1C, 32}}

    ' ItemForm2.h (_LANGUAGE_BRAZIL)
    Private Const FormAdd1 As Long = 1
    Private Const FormAdd2 As Long = 5
    Private Const FormMult As Long = 43
    Private Const FormStartMask As Long = &HF5F5
    Private Const FormCodeMask As Long = &HFCF
    Private Const FormCodeShift As Integer = 14

    Friend Const HeadOffset As Integer = &H4
    Friend Const FormCodeOffset As Integer = &HC
    Friend Const ChkSumOffset As Integer = &H10
    Friend Const CodeOffset As Integer = &H18
    Friend Const AgingProtectOffset As Integer = &H1E4

    Private Function SignedByte(ByVal b As Byte) As Long
        Return If(b >= 128, CLng(b) - 256, CLng(b))
    End Function

    Private Function U32(ByVal data As Byte(), ByVal offset As Integer) As Long
        Return BitConverter.ToUInt32(data, offset)
    End Function

    ''' <summary>GetRefromItemCode(item, Head): valor esperado em ItemHeader.dwChkSum.</summary>
    Friend Function ClientChecksum(ByVal data As Byte(), ByVal infoOffset As Integer) As UInteger
        Dim code As Long = 0
        Dim count As Long = U32(data, infoOffset + HeadOffset)
        For f = 0 To ClientFields.GetLength(0) - 1
            Dim start As Integer = infoOffset + ClientFields(f, 0)
            For i = 0 To ClientFields(f, 1) - 1
                code = (code + SignedByte(data(start + i)) * ((count And &H1FFFL) + 1)) And Mask
                count = (count + 1) And Mask
            Next
        Next
        Return CUInt(code)
    End Function

    ''' <summary>rsGetItem_ServerForm: valor esperado em ItemHeader.dwTime.</summary>
    Friend Function ServerForm(ByVal data As Byte(), ByVal infoOffset As Integer, ByVal profile As CharacterProfile) As UInteger
        Dim key As Long = U32(data, infoOffset + ChkSumOffset)
        Dim itemCode As Long = U32(data, infoOffset + CodeOffset)
        ' So os 16 bits baixos do produto sobrevivem a mascara.
        Dim code As Long = ((key And &HFFFFL) * (itemCode And &HFFFFL)) And FormStartMask
        Dim count As Long = key
        For f = 0 To ServerFields.GetLength(0) - 1
            Dim start As Integer = infoOffset + ServerFields(f, 0)
            For i = 0 To ServerFields(f, 1) - 1
                Dim v As Long = SignedByte(data(start + i))
                code = (code + ((v + FormAdd1) And Mask) * ((count And FormCodeMask) + FormAdd2)) And Mask
                count = (count + (i + v + 1) * FormMult) And Mask
            Next
        Next
        Dim protect0 As Long = BitConverter.ToInt16(data, infoOffset + AgingProtectOffset)
        Dim protect1 As Long = BitConverter.ToInt16(data, infoOffset + AgingProtectOffset + 2)
        code = code Xor (((protect0 * 7) Xor (protect1 * 9)) And Mask)
        Dim createTime As Long = U32(data, infoOffset + profile.CreateTimeOffset)
        code = (code + ((createTime And &HFFFFL) << 2)) And Mask
        Dim head As Long = U32(data, infoOffset + HeadOffset)
        code = code Xor ((head Xor (((key + code) And Mask) << FormCodeShift)) And Mask)
        Return CUInt(code And Mask)
    End Function

End Module
