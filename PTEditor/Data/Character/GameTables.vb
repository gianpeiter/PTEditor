''' <summary>Nomes fixos do jogo usados para exibir os dados dos personagens (fonte: Servidor\*).</summary>
Friend Module GameTables

    ''' <summary>JOB_CODE -> classe (smCHAR_INFO.JOB_CODE).</summary>
    Friend ReadOnly ClassNames As New Dictionary(Of Integer, String) From {
        {1, "Fighter"}, {2, "Mechanician"}, {3, "Archer"}, {4, "Pikeman"}, {5, "Atalanta"}, {6, "Knight"},
        {7, "Magician"}, {8, "Priestess"}, {9, "Assassin"}, {10, "Shaman"}, {11, "Martial"}}

    Friend Function ClassName(ByVal jobCode As Integer) As String
        Dim name As String = Nothing
        Return If(ClassNames.TryGetValue(jobCode, name), name, "Class " & jobCode)
    End Function

    ''' <summary>FieldCode (GameSaveInfo.PlayStageNum) -> mapa, na ordem de registro em Servidor\field.cpp.</summary>
    Private ReadOnly MapNames As String() = {
        "fore-3", "fore-2", "fore-1", "village-2 (Ricarten)", "ruin-4", "ruin-3", "ruin-2", "ruin-1", "De-1",
        "village-1 (Pillai)", "De-2", "De-3", "De-4", "dun-1", "dun-2", "dun-3", "office", "forever-fall-04",
        "forever-fall-03", "forever-fall-02", "forever-fall-01", "pilai", "dun-4", "dun-5", "Tcave", "Mcave",
        "Dcave", "iron-1", "iron-2", "ice_ura", "sod-1", "ice1", "quest_IV", "castle", "Greedy", "ice2", "Boss",
        "lost", "Losttemple", "fall_game", "dun-7", "dun-8", "dun-6", "Condado-Pecador", "mine-1", "Slab",
        "AncientW", "SeaA", "dark_boss", "ba1", "HeartOfFire", "iron3", "dun-9"}

    Friend Function MapName(ByVal fieldCode As Integer) As String
        If fieldCode >= 0 AndAlso fieldCode < MapNames.Length Then Return fieldCode & " - " & MapNames(fieldCode)
        Return fieldCode.ToString()
    End Function

    ''' <summary>sRECORD_ITEM.ItemPosition -> onde o item esta.</summary>
    Friend Function PositionName(ByVal position As Integer) As String
        Select Case position
            Case -1 : Return "Mouse"
            Case 0 : Return "Bag"
            Case 1 : Return "Right hand"
            Case 2 : Return "Left hand"
            Case 3 : Return "Armor"
            Case 4 : Return "Amulet"
            Case 5 : Return "Ring 1"
            Case 6 : Return "Ring 2"
            Case 7 : Return "Sheltom"
            Case 8 : Return "Armlet"
            Case 9 : Return "Gauntlets"
            Case 10 : Return "Boots"
            Case 11, 12, 13 : Return "Potion " & (position - 10)
            Case 14 : Return "Costume"
            Case 15, 16 : Return "Earring " & (position - 14)
            Case Else : Return "Position " & position
        End Select
    End Function

    ''' <summary>Grupo do CODE (CODE And &amp;HFFFF0000) -> prefixo do codigo texto (#define sinXX1 em sinItem.h).</summary>
    Private ReadOnly ItemGroups As New Dictionary(Of UInteger, String) From {
        {&H1010000UI, "WA1"}, {&H1020000UI, "WC1"}, {&H1030000UI, "WH1"}, {&H1040000UI, "WM1"}, {&H1050000UI, "WP1"},
        {&H1060000UI, "WS1"}, {&H1070000UI, "WS2"}, {&H1080000UI, "WT1"}, {&H1090000UI, "WN1"}, {&H10A0000UI, "WD1"},
        {&H10B0000UI, "WV1"},
        {&H2010000UI, "DA1"}, {&H2020000UI, "DB1"}, {&H2030000UI, "DG1"}, {&H2040000UI, "DS1"}, {&H2050000UI, "DA2"},
        {&H2070000UI, "CA1"}, {&H2080000UI, "CA2"}, {&H2100000UI, "CA5"}, {&H2110000UI, "CA6"}, {&H2120000UI, "DA3"},
        {&H2130000UI, "DA4"}, {&H2350000UI, "OS1"},
        {&H3010000UI, "OA1"}, {&H3020000UI, "OA2"}, {&H3030000UI, "OM1"}, {&H3040000UI, "OR1"}, {&H3050000UI, "OR2"},
        {&H3060000UI, "FO1"}, {&H3120000UI, "FO2"}, {&H3070000UI, "SE1"}, {&H3080000UI, "PR1"}, {&H3090000UI, "PR2"},
        {&H3100000UI, "PR3"}, {&H3110000UI, "PR4"}, {&H3130000UI, "PR5"}, {&H30A0000UI, "OE1"},
        {&H4010000UI, "PM1"}, {&H4020000UI, "PL1"}, {&H4030000UI, "PS1"},
        {&H5010000UI, "GG1"}, {&H5020000UI, "BS1"}, {&H6010000UI, "EC1"}, {&H7010000UI, "QT1"},
        {&H8010000UI, "SP1"}, {&H80E0000UI, "SP2"}, {&H8020000UI, "GP1"}, {&H8030000UI, "QW1"}, {&H8040000UI, "GF1"},
        {&H8060000UI, "PZ1"}, {&H8070000UI, "PZ2"}, {&H8080000UI, "CH1"}, {&H8090000UI, "SD2"}, {&H80A0000UI, "BC1"},
        {&H8200000UI, "BC2"}, {&H80B0000UI, "BI1"}, {&H80C0000UI, "BI2"}, {&H80D0000UI, "GP2"}, {&H8050000UI, "GP3"},
        {&H9030000UI, "BI3"}, {&H9010000UI, "MA1"}, {&H9020000UI, "MA2"}, {&H9040000UI, "EV1"}, {&H9060000UI, "EV2"},
        {&HA010000UI, "WR1"}, {&HA030000UI, "WR2"}, {&HA020000UI, "DR1"}, {&HA040000UI, "DR2"}}

    ''' <summary>CODE numerico -> codigo texto do OpenItem (ex.: &amp;H01010100 -> "WA101"). Grupo desconhecido: hexadecimal.</summary>
    Friend Function ItemCodeText(ByVal code As UInteger) As String
        Dim group As String = Nothing
        If ItemGroups.TryGetValue(code And &HFFFF0000UI, group) Then
            Return group & ((code >> 8) And &HFFUI).ToString("00")
        End If
        Return code.ToString("X8")
    End Function

    ''' <summary>Pocoes (PM/PL/PS) se empilham com o mesmo cabecalho: o servidor nao as trata como copia.</summary>
    Friend Function IsPotion(ByVal code As UInteger) As Boolean
        Dim group As UInteger = code And &HFFFF0000UI
        Return group = &H4010000UI OrElse group = &H4020000UI OrElse group = &H4030000UI
    End Function

End Module
