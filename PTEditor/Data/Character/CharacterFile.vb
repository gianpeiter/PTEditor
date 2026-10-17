Imports System.IO

''' <summary>
''' DataServer\userdata\&lt;n&gt;\&lt;personagem&gt;.dat (TRANS_RECORD_DATA em Servidor\record.h).
''' Os campos sao lidos/escritos direto nos bytes do arquivo; os itens ficam descomprimidos em Items e sao
''' recomprimidos so ao gravar.
''' </summary>
Friend Class CharacterFile

    Public ReadOnly Property FilePath As String
    Public ReadOnly Property Profile As CharacterProfile
    Public ReadOnly Property Items As New List(Of GameItem)()

    Private ReadOnly _data As Byte()

    Private Sub New(ByVal filePath As String, ByVal data As Byte(), ByVal profile As CharacterProfile)
        Me.FilePath = filePath
        _data = data
        Me.Profile = profile
    End Sub

    ''' <summary>
    ''' Le o arquivo com o primeiro perfil que o reconhece. Lanca InvalidDataException com a explicacao quando
    ''' nenhum dos perfis serve.
    ''' </summary>
    Public Shared Function Load(ByVal filePath As String, ByVal candidates As IEnumerable(Of CharacterProfile)) As CharacterFile
        Return Parse(filePath, File.ReadAllBytes(filePath), candidates)
    End Function

    Public Shared Function Parse(ByVal filePath As String, ByVal data As Byte(), ByVal candidates As IEnumerable(Of CharacterProfile)) As CharacterFile
        If data.Length < CharOffsets.GameSaveInfo Then
            Throw New InvalidDataException("File too small (" & data.Length & " bytes).")
        End If
        Dim size As Integer = BitConverter.ToInt32(data, CharOffsets.Size)
        If size = 0 Then Throw New InvalidDataException("Empty file (size = 0): character was never saved.")
        Dim profile = CharacterProfile.DetectCharacter(data, candidates)
        If profile Is Nothing Then
            Dim code As Integer = BitConverter.ToInt32(data, CharOffsets.Code)
            Dim sameCode = candidates.Where(Function(p) p.RecordCode = code).Select(Function(p) p.Name).ToList()
            If sameCode.Count = 0 Then
                Throw New InvalidDataException("No profile uses record code 0x" & code.ToString("X") & " (" &
                                               String.Join(", ", candidates.Select(Function(p) p.Name)) & " tested).")
            End If
            Throw New InvalidDataException("Record code matches " & String.Join(", ", sameCode) &
                                           " but the item data does not fit (size = " & size & ").")
        End If
        Dim result As New CharacterFile(filePath, data, profile)
        result.ReadItems()
        Return result
    End Function

    Private Sub ReadItems()
        Dim count As Integer = ReadInt(Profile.ItemCountOffset)
        Dim subStart As Integer = ReadInt(Profile.ItemSubStartOffset)
        Dim dataSize As Integer = ReadInt(Profile.DataSizeOffset)
        Dim position As Integer = 0
        For i = 0 To count - 1
            If position + 4 > dataSize Then
                Throw New InvalidDataException("Item list ends early (item " & i & " of " & count & ").")
            End If
            Dim offset As Integer = Profile.DataOffset + position
            Dim length As Integer = ItemCompression.BlockLength(_data, offset)
            If length <= 4 OrElse position + length > dataSize Then
                Throw New InvalidDataException("Item " & i & " has an invalid size (" & length & ").")
            End If
            Dim record As Byte() = ItemCompression.Decode(_data, offset, Profile.RecordItemSize)
            Dim item As New GameItem(record, 16, Profile, ItemContainer.Inventory) With {
                .Slot = BitConverter.ToInt32(record, 0),
                .X = BitConverter.ToInt32(record, 4),
                .Y = BitConverter.ToInt32(record, 8),
                .Position = BitConverter.ToInt32(record, 12),
                .SecondInventory = i >= subStart}
            Items.Add(item)
            position += length
        Next
    End Sub

#Region "Acesso aos bytes"

    Private Function ReadInt(ByVal offset As Integer) As Integer
        Return BitConverter.ToInt32(_data, offset)
    End Function

    Private Sub WriteInt(ByVal offset As Integer, ByVal value As Integer)
        BitConverter.GetBytes(value).CopyTo(_data, offset)
    End Sub

    Private Function ReadText(ByVal offset As Integer, ByVal maxLength As Integer) As String
        Dim length As Integer = 0
        Do While length < maxLength AndAlso _data(offset + length) <> 0
            length += 1
        Loop
        Return AppState.FileEncoding.GetString(_data, offset, length)
    End Function

    ''' <summary>Par (atual, maximo) de shorts, como Life/Mana/Stamina.</summary>
    Private Function ReadPair(ByVal offset As Integer) As String
        Return BitConverter.ToInt16(_data, offset) & "/" & BitConverter.ToInt16(_data, offset + 2)
    End Function

#End Region

#Region "Campos"

    Public ReadOnly Property Header As String
        Get
            Return ReadText(CharOffsets.Header, CharOffsets.HeaderLength)
        End Get
    End Property

    Public ReadOnly Property Name As String
        Get
            Return ReadText(CharOffsets.Name, CharOffsets.NameLength)
        End Get
    End Property

    Public ReadOnly Property JobCode As Integer
        Get
            Return ReadInt(CharOffsets.JobCode)
        End Get
    End Property

    Public ReadOnly Property ClassName As String
        Get
            Return GameTables.ClassName(JobCode)
        End Get
    End Property

    ''' <summary>Conta dona gravada no personagem (GameSaveInfo.szMasterID).</summary>
    Public ReadOnly Property MasterId As String
        Get
            Return ReadText(Profile.MasterIdOffset, Profile.MasterIdLength)
        End Get
    End Property

    Public Property Level As Integer
        Get
            Return ReadInt(CharOffsets.Level)
        End Get
        Set(ByVal value As Integer)
            WriteInt(CharOffsets.Level, value)
        End Set
    End Property

    ''' <summary>EXP de 64 bits: Exp (32 bits baixos) + Exp_High (_MODE_EXP64).</summary>
    Public Property Exp As Long
        Get
            Dim low As Long = BitConverter.ToUInt32(_data, CharOffsets.Exp)
            Dim high As Long = ReadInt(CharOffsets.ExpHigh)
            Return (high << 32) Or low
        End Get
        Set(ByVal value As Long)
            BitConverter.GetBytes(CUInt(value And &HFFFFFFFFL)).CopyTo(_data, CharOffsets.Exp)
            WriteInt(CharOffsets.ExpHigh, CInt(value >> 32))
        End Set
    End Property

    Public Property Strength As Integer
        Get
            Return ReadInt(CharOffsets.Strength)
        End Get
        Set(ByVal value As Integer)
            WriteInt(CharOffsets.Strength, value)
        End Set
    End Property

    Public Property Spirit As Integer
        Get
            Return ReadInt(CharOffsets.Spirit)
        End Get
        Set(ByVal value As Integer)
            WriteInt(CharOffsets.Spirit, value)
        End Set
    End Property

    Public Property Talent As Integer
        Get
            Return ReadInt(CharOffsets.Talent)
        End Get
        Set(ByVal value As Integer)
            WriteInt(CharOffsets.Talent, value)
        End Set
    End Property

    Public Property Dexterity As Integer
        Get
            Return ReadInt(CharOffsets.Dexterity)
        End Get
        Set(ByVal value As Integer)
            WriteInt(CharOffsets.Dexterity, value)
        End Set
    End Property

    Public Property Health As Integer
        Get
            Return ReadInt(CharOffsets.Health)
        End Get
        Set(ByVal value As Integer)
            WriteInt(CharOffsets.Health, value)
        End Set
    End Property

    Public Property StatePoint As Integer
        Get
            Return ReadInt(CharOffsets.StatePoint)
        End Get
        Set(ByVal value As Integer)
            WriteInt(CharOffsets.StatePoint, value)
        End Set
    End Property

    ''' <summary>Rank de classe (smCHAR_INFO.ChangeJob): libera os grupos de 4 skills no Skill Master.</summary>
    Public Property ChangeJob As Integer
        Get
            Return ReadInt(CharOffsets.ChangeJob)
        End Get
        Set(ByVal value As Integer)
            WriteInt(CharOffsets.ChangeJob, value)
        End Set
    End Property

    Public Property Money As Integer
        Get
            Return ReadInt(CharOffsets.Money)
        End Get
        Set(ByVal value As Integer)
            WriteInt(CharOffsets.Money, value)
        End Set
    End Property

    ''' <summary>Se &gt; 0, cliente e servidor usam LastMoeny - 1 como ouro (record.cpp). Zerar ao editar o ouro.</summary>
    Public Property LastMoney As Integer
        Get
            Return ReadInt(CharOffsets.LastMoney)
        End Get
        Set(ByVal value As Integer)
            WriteInt(CharOffsets.LastMoney, value)
        End Set
    End Property

    ''' <summary>Ouro que o jogo vai considerar ao carregar.</summary>
    Public ReadOnly Property EffectiveMoney As Integer
        Get
            Return If(LastMoney > 0, LastMoney - 1, Money)
        End Get
    End Property

    ''' <summary>Quests de level concluidas (GameSaveInfo.dwLevelQuestLog): pontos extras de atributo e de skill.</summary>
    Public Property LevelQuestLog As UInteger
        Get
            Return BitConverter.ToUInt32(_data, Profile.LevelQuestLogOffset)
        End Get
        Set(ByVal value As UInteger)
            BitConverter.GetBytes(value).CopyTo(_data, Profile.LevelQuestLogOffset)
        End Set
    End Property

    ''' <summary>sQUEST_INFO: wQuestCode[2] + Data[7].</summary>
    Private Const QuestInfoSize As Integer = 32
    ''' <summary>Data[] comeca com CODE, BackUpCode, Kind, State (sQUEST_CHANGEJOB / sQUEST_CHANGEJOB3 / sQUEST_ELEMENTARY).</summary>
    Private Const QuestStateOffset As Integer = 4 + 12

    ''' <summary>O perfil informa onde fica a quest em andamento.</summary>
    Public ReadOnly Property HasQuestInfo As Boolean
        Get
            Return Profile.QuestInfoOffset >= 0
        End Get
    End Property

    ''' <summary>Codigo da quest em andamento (QuestInfo.wQuestCode[0]); 0 = nenhuma.</summary>
    Public ReadOnly Property QuestCode As Integer
        Get
            If Not HasQuestInfo Then Return 0
            Return BitConverter.ToUInt16(_data, Profile.QuestInfoOffset)
        End Get
    End Property

    Public ReadOnly Property QuestState As Integer
        Get
            If Not HasQuestInfo Then Return 0
            Return ReadInt(Profile.QuestInfoOffset + QuestStateOffset)
        End Get
    End Property

    ''' <summary>Abandona a quest em andamento (o jogo carrega a quest so se wQuestCode[0] &lt;&gt; 0).</summary>
    Public Sub ClearQuest()
        If Not HasQuestInfo Then Return
        Array.Clear(_data, Profile.QuestInfoOffset, QuestInfoSize)
    End Sub

    Public ReadOnly Property MapCode As Integer
        Get
            Return ReadInt(CharOffsets.PlayStageNum)
        End Get
    End Property

    ''' <summary>Posicao em unidades do mapa (o arquivo guarda em ponto fixo x256).</summary>
    Public ReadOnly Property PosX As Integer
        Get
            Return ReadInt(CharOffsets.PosX) \ 256
        End Get
    End Property

    Public ReadOnly Property PosZ As Integer
        Get
            Return ReadInt(CharOffsets.PosZ) \ 256
        End Get
    End Property

    Public ReadOnly Property Life As String
        Get
            Return ReadPair(CharOffsets.Life)
        End Get
    End Property

    Public ReadOnly Property Mana As String
        Get
            Return ReadPair(CharOffsets.Mana)
        End Get
    End Property

    Public ReadOnly Property Stamina As String
        Get
            Return ReadPair(CharOffsets.Stamina)
        End Get
    End Property

    Public ReadOnly Property Weight As String
        Get
            Return ReadPair(CharOffsets.Weight)
        End Get
    End Property

    Public ReadOnly Property SaveHead As Integer
        Get
            Return ReadInt(CharOffsets.SaveHead)
        End Get
    End Property

    Public Function SkillPoint(ByVal index As Integer) As Integer
        Return _data(CharOffsets.SkillInfo + index)
    End Function

    Public Function SkillMastery(ByVal index As Integer) As Integer
        Return BitConverter.ToUInt16(_data, Profile.SkillMasteryOffset + index * 2)
    End Function

    Public Function SkillShortKey(ByVal index As Integer) As Integer
        Return _data(Profile.ShortKeyOffset + index)
    End Function

#End Region

#Region "Itens e gravacao"

    ''' <summary>Remove o item; uma arma de duas maos leva junto a copia da mao esquerda.</summary>
    Public Function RemoveItem(ByVal item As GameItem) As List(Of GameItem)
        Dim removed As New List(Of GameItem) From {item}
        If item.Position = 1 AndAlso Not item.IsTwoHandedCopy Then
            removed.AddRange(Items.Where(Function(i) i.IsTwoHandedCopy AndAlso i.Code = item.Code AndAlso
                                                       i.SecondInventory = item.SecondInventory))
        End If
        For Each r In removed
            Items.Remove(r)
        Next
        Return removed
    End Function

    ''' <summary>
    ''' Bytes do arquivo com os itens atuais recomprimidos: ItemCount, ItemSubStart, DataSize e size sao
    ''' recalculados e o espaco apos os dados e zerado (o servidor grava no minimo 16384 bytes).
    ''' </summary>
    Public Function BuildBytes() As Byte()
        Dim blob As New List(Of Byte)()
        For Each item In Items.Where(Function(i) Not i.SecondInventory)
            blob.AddRange(ItemCompression.Encode(item.Raw))
        Next
        Dim subStart As Integer = Items.Where(Function(i) Not i.SecondInventory).Count()
        For Each item In Items.Where(Function(i) i.SecondInventory)
            blob.AddRange(ItemCompression.Encode(item.Raw))
        Next
        Dim size As Integer = Profile.DataOffset + blob.Count
        Dim result(Math.Max(Math.Max(_data.Length, size), CharOffsets.MinFileLength) - 1) As Byte
        Array.Copy(_data, result, Profile.DataOffset)
        blob.CopyTo(result, Profile.DataOffset)
        BitConverter.GetBytes(size).CopyTo(result, CharOffsets.Size)
        BitConverter.GetBytes(Items.Count).CopyTo(result, Profile.ItemCountOffset)
        BitConverter.GetBytes(subStart).CopyTo(result, Profile.ItemSubStartOffset)
        BitConverter.GetBytes(blob.Count).CopyTo(result, Profile.DataSizeOffset)
        Return result
    End Function

    ''' <summary>Grava no proprio arquivo (escreve num temporario e substitui).</summary>
    Public Sub Save()
        Dim temp As String = FilePath & ".tmp"
        File.WriteAllBytes(temp, BuildBytes())
        File.Copy(temp, FilePath, True)
        File.Delete(temp)
    End Sub

#End Region

End Class
