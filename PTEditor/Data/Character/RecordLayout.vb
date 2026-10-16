''' <summary>
''' Offsets que mudam entre versoes do servidor. "Servidor" = Desktop\source\Servidor (o game.exe do Happy usa
''' sizeof(TRANS_RECORD_DATA) = 113016); "Classic" = PTClassic / ferramentas antigas (lista de itens em &amp;H6F0).
''' A deteccao usa o cabecalho do arquivo: offset dos dados + DataSize tem que dar o campo size.
''' </summary>
Friend NotInheritable Class RecordLayout

    Public ReadOnly Property Name As String

    ' TRANS_RECORD_DATA (offsets no arquivo)
    Public ReadOnly Property ItemCountOffset As Integer
    Public ReadOnly Property ItemSubStartOffset As Integer
    Public ReadOnly Property DataSizeOffset As Integer
    Public ReadOnly Property DataOffset As Integer

    ' sRECORD_ITEM / sITEMINFO
    Public ReadOnly Property RecordItemSize As Integer
    Public ReadOnly Property CreateTimeOffset As Integer

    ' sGAME_SAVE_INFO (offsets no arquivo)
    Public ReadOnly Property SkillCount As Integer
    Public ReadOnly Property SkillMasteryOffset As Integer
    Public ReadOnly Property ShortKeyOffset As Integer
    Public ReadOnly Property LevelQuestLogOffset As Integer
    Public ReadOnly Property MasterIdOffset As Integer

    ' sWAREHOUSE (descomprimido)
    Public ReadOnly Property WarehouseItemSize As Integer
    Public ReadOnly Property WarehouseSize As Integer
    Public ReadOnly Property WarehouseMoneyOffset As Integer

    Private Sub New(ByVal name As String, ByVal dataOffset As Integer, ByVal recordItemSize As Integer,
                    ByVal createTimeOffset As Integer, ByVal skillCount As Integer, ByVal levelQuestLogOffset As Integer,
                    ByVal masterIdOffset As Integer, ByVal warehouseItemSize As Integer, ByVal warehouseMoneyOffset As Integer)
        Me.Name = name
        Me.DataOffset = dataOffset
        DataSizeOffset = dataOffset - 4
        ItemSubStartOffset = dataOffset - 8
        ItemCountOffset = dataOffset - 12
        Me.RecordItemSize = recordItemSize
        Me.CreateTimeOffset = createTimeOffset
        Me.SkillCount = skillCount
        ' RECORD_SKILL: bSkillPoint[n], wSkillMastery[n], bShortKey[n] (...)
        SkillMasteryOffset = CharOffsets.SkillInfo + skillCount
        ShortKeyOffset = SkillMasteryOffset + skillCount * 2
        Me.LevelQuestLogOffset = CharOffsets.GameSaveInfo + levelQuestLogOffset
        Me.MasterIdOffset = CharOffsets.GameSaveInfo + masterIdOffset
        Me.WarehouseItemSize = warehouseItemSize
        WarehouseSize = warehouseItemSize * 100 + 16
        Me.WarehouseMoneyOffset = warehouseMoneyOffset
    End Sub

    Public Shared ReadOnly Servidor As New RecordLayout("Servidor", &H718, 556, &H1F4, 20, &HB4, &H108, 796, &H136F0)
    Public Shared ReadOnly Classic As New RecordLayout("Classic", &H6F0, 548, &H1EC, 16, &H90, &HE0, 788, &H133D0)

    Public Shared ReadOnly All As RecordLayout() = {Servidor, Classic}

    ''' <summary>Layout cujo offset de dados + DataSize = size do cabecalho, ou Nothing.</summary>
    Public Shared Function Detect(ByVal data As Byte()) As RecordLayout
        If data.Length < 16 Then Return Nothing
        Dim size As Integer = BitConverter.ToInt32(data, 0)
        For Each candidate In All
            If data.Length < candidate.DataOffset Then Continue For
            Dim dataSize As Integer = BitConverter.ToInt32(data, candidate.DataSizeOffset)
            If size > 0 AndAlso dataSize >= 0 AndAlso candidate.DataOffset + dataSize = size Then Return candidate
        Next
        Return Nothing
    End Function

    ''' <summary>Layout do bau pelo tamanho do sWAREHOUSE descomprimido, ou Nothing.</summary>
    Public Shared Function FromWarehouseSize(ByVal decodedSize As Integer) As RecordLayout
        For Each candidate In All
            If candidate.WarehouseSize = decodedSize Then Return candidate
        Next
        Return Nothing
    End Function

End Class

''' <summary>Offsets iguais nos dois layouts (arquivo de personagem).</summary>
Friend NotInheritable Class CharOffsets

    Private Sub New()
    End Sub

    Friend Const Size As Integer = &H0
    Friend Const Code As Integer = &H4
    Friend Const Header As Integer = &H8
    Friend Const HeaderLength As Integer = 8

    ' smCHAR_INFO (comeca em &H10)
    Friend Const CharInfo As Integer = &H10
    Friend Const CharInfoSize As Integer = 464
    Friend Const Name As Integer = CharInfo + &H0
    Friend Const NameLength As Integer = 32
    Friend Const JobCode As Integer = CharInfo + &HB4
    Friend Const Level As Integer = CharInfo + &HB8
    Friend Const Strength As Integer = CharInfo + &HBC
    Friend Const Spirit As Integer = CharInfo + &HC0
    Friend Const Talent As Integer = CharInfo + &HC4
    Friend Const Dexterity As Integer = CharInfo + &HC8
    Friend Const Health As Integer = CharInfo + &HCC
    Friend Const Weight As Integer = CharInfo + &H100
    Friend Const Life As Integer = CharInfo + &H124
    Friend Const Mana As Integer = CharInfo + &H128
    Friend Const Stamina As Integer = CharInfo + &H12C
    Friend Const Exp As Integer = CharInfo + &H13C
    Friend Const Money As Integer = CharInfo + &H144
    Friend Const StatePoint As Integer = CharInfo + &H150
    Friend Const ChangeJob As Integer = CharInfo + &H174
    Friend Const ExpHigh As Integer = CharInfo + &H184

    ' sGAME_SAVE_INFO (comeca em &H1E0)
    Friend Const GameSaveInfo As Integer = &H1E0
    Friend Const SaveHead As Integer = GameSaveInfo + &H0
    Friend Const PlayStageNum As Integer = GameSaveInfo + &H4
    Friend Const PosX As Integer = GameSaveInfo + &HC
    Friend Const PosZ As Integer = GameSaveInfo + &H10
    Friend Const LastMoney As Integer = GameSaveInfo + &H14
    Friend Const SkillInfo As Integer = GameSaveInfo + &H1C

    Friend Const RecordCode As Integer = &H48470081
    ''' <summary>O servidor grava no minimo 16384 bytes (record.cpp: max(size, srRECORD_DEFAULT_SIZE)).</summary>
    Friend Const MinFileLength As Integer = 16384
End Class
