Imports System.Globalization
Imports System.IO
Imports System.Text

''' <summary>
''' Perfil de um servidor (Profiles\*.ini ao lado do executavel): offsets que mudam entre sources, codigos de pacote,
''' tabela de EXP e regras do servidor. Cada source e um fork com structs diferentes; o perfil descreve uma delas.
''' </summary>
Friend NotInheritable Class CharacterProfile

    Public ReadOnly Property Name As String
    Public ReadOnly Property FilePath As String

    ' Codigos gravados no campo code (smTRANSCODE_* ou enum OpCode, conforme a source)
    Public ReadOnly Property RecordCode As Integer
    Public ReadOnly Property WarehouseCode As Integer

    ' TRANS_RECORD_DATA (offsets no arquivo)
    Public ReadOnly Property DataOffset As Integer
    Public ReadOnly Property ItemCountOffset As Integer
    Public ReadOnly Property ItemSubStartOffset As Integer
    Public ReadOnly Property DataSizeOffset As Integer

    ' sRECORD_ITEM / sITEMINFO
    Public ReadOnly Property RecordItemSize As Integer
    Public ReadOnly Property CreateTimeOffset As Integer
    Public ReadOnly Property NameEncoding As Encoding

    ' sGAME_SAVE_INFO (offsets no arquivo)
    Public ReadOnly Property SkillCount As Integer
    Public ReadOnly Property SkillMasteryOffset As Integer
    Public ReadOnly Property ShortKeyOffset As Integer
    Public ReadOnly Property LevelQuestLogOffset As Integer
    ''' <summary>GameSaveInfo.QuestInfo (sQUEST_INFO, quest em andamento); -1 se o perfil nao informa.</summary>
    Public ReadOnly Property QuestInfoOffset As Integer
    Public ReadOnly Property MasterIdOffset As Integer
    Public ReadOnly Property MasterIdLength As Integer

    ' sWAREHOUSE (descomprimido): 100 sITEM + Money, Weight[2], BuyAreaCount, FakeMoney
    Public ReadOnly Property WarehouseItemSize As Integer
    Public ReadOnly Property WarehouseItemInfoOffset As Integer
    Public ReadOnly Property WarehouseSize As Integer
    Public ReadOnly Property WarehouseMoneyOffset As Integer

    ' Regras do servidor
    Public ReadOnly Property LevelTable As LevelTable
    ''' <summary>CheckLevelExp/GetLevelFromExp ativos: o servidor muda o level se a EXP nao pertencer a ele.</summary>
    Public ReadOnly Property FixLevelFromExp As Boolean

    Friend Const WarehouseSlots As Integer = 100

    Private Sub New(ByVal filePath As String, ByVal v As Dictionary(Of String, String))
        Me.FilePath = filePath
        Name = Text(v, "Name")
        RecordCode = Number(v, "RecordCode")
        WarehouseCode = Number(v, "WarehouseCode")
        DataOffset = Number(v, "ItemDataOffset")
        DataSizeOffset = DataOffset - 4
        ItemSubStartOffset = DataOffset - 8
        ItemCountOffset = DataOffset - 12
        RecordItemSize = Number(v, "RecordItemSize")
        CreateTimeOffset = Number(v, "CreateTimeOffset")
        NameEncoding = Encoding.GetEncoding(Number(v, "ItemNameEncoding"))
        SkillCount = Number(v, "SkillCount")
        ' RECORD_SKILL: bSkillPoint[n], wSkillMastery[n], bShortKey[n] (...)
        SkillMasteryOffset = CharOffsets.SkillInfo + SkillCount
        ShortKeyOffset = SkillMasteryOffset + SkillCount * 2
        LevelQuestLogOffset = Number(v, "LevelQuestLogOffset")
        QuestInfoOffset = If(v.ContainsKey("QuestInfoOffset"), Number(v, "QuestInfoOffset"), -1)
        MasterIdOffset = Number(v, "MasterIdOffset")
        MasterIdLength = Number(v, "MasterIdLength")
        WarehouseItemSize = Number(v, "WarehouseItemSize")
        WarehouseItemInfoOffset = Number(v, "WarehouseItemInfoOffset")
        WarehouseMoneyOffset = WarehouseItemSize * WarehouseSlots
        WarehouseSize = WarehouseMoneyOffset + 16
        FixLevelFromExp = Bool(v, "FixLevelFromExp")
        Dim tablePath As String = Path.Combine(Path.GetDirectoryName(filePath), Text(v, "LevelTable"))
        LevelTable = LevelTable.Load(tablePath, Number(v, "MaxLevel"))
    End Sub

    Public Overrides Function ToString() As String
        Return Name
    End Function

#Region "Leitura do .ini"

    ''' <summary>Le o perfil. Lanca InvalidDataException dizendo qual chave falta ou esta invalida.</summary>
    Public Shared Function Load(ByVal filePath As String) As CharacterProfile
        Dim values As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        For Each raw In File.ReadAllLines(filePath)
            Dim line As String = raw.Trim()
            If line = "" OrElse line.StartsWith(";") OrElse line.StartsWith("#") Then Continue For
            Dim equals As Integer = line.IndexOf("="c)
            If equals <= 0 Then Continue For
            values(line.Substring(0, equals).Trim()) = line.Substring(equals + 1).Trim()
        Next
        Return New CharacterProfile(filePath, values)
    End Function

    Private Shared Function Text(ByVal v As Dictionary(Of String, String), ByVal key As String) As String
        Dim value As String = Nothing
        If Not v.TryGetValue(key, value) OrElse value = "" Then Throw New InvalidDataException("Missing key '" & key & "'.")
        Return value
    End Function

    ''' <summary>Inteiro decimal ou hexadecimal (0x...).</summary>
    Private Shared Function Number(ByVal v As Dictionary(Of String, String), ByVal key As String) As Integer
        Dim value As String = Text(v, key)
        Dim result As Integer
        Dim ok As Boolean
        If value.StartsWith("0x", StringComparison.OrdinalIgnoreCase) Then
            ok = Integer.TryParse(value.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, result)
        Else
            ok = Integer.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, result)
        End If
        If Not ok Then Throw New InvalidDataException("Key '" & key & "' is not a number: " & value)
        Return result
    End Function

    Private Shared Function Bool(ByVal v As Dictionary(Of String, String), ByVal key As String) As Boolean
        Dim value As String = Text(v, key).ToUpperInvariant()
        If value = "TRUE" Then Return True
        If value = "FALSE" Then Return False
        Throw New InvalidDataException("Key '" & key & "' must be TRUE or FALSE: " & value)
    End Function

#End Region

#Region "Deteccao"

    ''' <summary>True se o arquivo de personagem bate com o perfil: codigo e offset dos dados + DataSize = size.</summary>
    Public Function MatchesCharacter(ByVal data As Byte()) As Boolean
        If data.Length < DataOffset Then Return False
        If BitConverter.ToInt32(data, CharOffsets.Code) <> RecordCode Then Return False
        Dim size As Integer = BitConverter.ToInt32(data, CharOffsets.Size)
        Dim dataSize As Integer = BitConverter.ToInt32(data, DataSizeOffset)
        Return size > 0 AndAlso dataSize >= 0 AndAlso DataOffset + dataSize = size
    End Function

    ''' <summary>Primeiro perfil que reconhece o arquivo de personagem, ou Nothing.</summary>
    Public Shared Function DetectCharacter(ByVal data As Byte(), ByVal candidates As IEnumerable(Of CharacterProfile)) As CharacterProfile
        Return candidates.FirstOrDefault(Function(p) p.MatchesCharacter(data))
    End Function

#End Region

End Class

''' <summary>Offsets iguais em todos os perfis conhecidos (arquivo de personagem).</summary>
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

    ''' <summary>O servidor grava no minimo 16384 bytes (record.cpp: max(size, srRECORD_DEFAULT_SIZE)).</summary>
    Friend Const MinFileLength As Integer = 16384
End Class
