Imports System.IO

''' <summary>
''' DataServer\warehouse\&lt;n&gt;\&lt;conta&gt;.war (TRANS_WAREHOUSE). Cabecalho de 48 bytes e um unico bloco
''' comprimido com o sWAREHOUSE inteiro (100 sITEM + ouro). Somente leitura.
''' O perfil e escolhido pelo codigo do cabecalho e pelo tamanho do sWAREHOUSE descomprimido.
''' </summary>
Friend Class WarehouseFile

    Private Const CodeOffset As Integer = &H4
    Private Const ChkSumOffset As Integer = &H8
    Private Const DataSizeOffset As Integer = &H2C
    Private Const DataOffset As Integer = &H30
    ' sITEM (iguais em todos os perfis conhecidos; o sItemInfo muda de posicao)
    Private Const ItemFlagOffset As Integer = &HCC
    Private Const ItemXOffset As Integer = &HD0
    Private Const ItemYOffset As Integer = &HD4
    ''' <summary>sWAREHOUSE.Money guarda o ouro + 2023.</summary>
    Private Const MoneyBias As Integer = 2023

    Public ReadOnly Property FilePath As String
    Public ReadOnly Property Profile As CharacterProfile
    Public ReadOnly Property Items As New List(Of GameItem)()
    Public ReadOnly Property Gold As Integer
    ''' <summary>dwChkSum do cabecalho confere com o conteudo (o servidor recusa abrir o bau se nao conferir).</summary>
    Public ReadOnly Property ChecksumOk As Boolean
    Public ReadOnly Property IsEmpty As Boolean

    Private Sub New(ByVal filePath As String)
        Me.FilePath = filePath
        IsEmpty = True
    End Sub

    Private Sub New(ByVal filePath As String, ByVal profile As CharacterProfile, ByVal decoded As Byte(), ByVal storedChkSum As UInteger)
        Me.FilePath = filePath
        Me.Profile = profile
        Dim money As Integer = BitConverter.ToInt32(decoded, profile.WarehouseMoneyOffset)
        Gold = If(money = 0, 0, money - MoneyBias)
        ChecksumOk = Checksum(decoded, profile.WarehouseSize) = storedChkSum
        For slot = 0 To CharacterProfile.WarehouseSlots - 1
            Dim offset As Integer = slot * profile.WarehouseItemSize
            If BitConverter.ToInt32(decoded, offset + ItemFlagOffset) = 0 Then Continue For
            Dim raw(profile.WarehouseItemSize - 1) As Byte
            Array.Copy(decoded, offset, raw, 0, raw.Length)
            Items.Add(New GameItem(raw, profile.WarehouseItemInfoOffset, profile, ItemContainer.Warehouse) With {
                .Slot = slot,
                .X = BitConverter.ToInt32(raw, ItemXOffset),
                .Y = BitConverter.ToInt32(raw, ItemYOffset)})
        Next
    End Sub

    Public Shared Function Load(ByVal filePath As String, ByVal candidates As IEnumerable(Of CharacterProfile)) As WarehouseFile
        Dim data As Byte() = File.ReadAllBytes(filePath)
        If data.Length < DataOffset + 4 OrElse BitConverter.ToInt32(data, 0) = 0 Then Return New WarehouseFile(filePath)
        Dim code As Integer = BitConverter.ToInt32(data, CodeOffset)
        Dim sameCode = candidates.Where(Function(p) p.WarehouseCode = code).ToList()
        If sameCode.Count = 0 Then
            Throw New InvalidDataException("No profile uses warehouse code 0x" & code.ToString("X") & ".")
        End If
        If BitConverter.ToInt32(data, DataSizeOffset) = 0 Then Return New WarehouseFile(filePath)
        Dim decodedLength As Integer
        Dim decoded As Byte() = ItemCompression.Decode(data, DataOffset, sameCode.Max(Function(p) p.WarehouseSize), decodedLength)
        Dim profile = sameCode.FirstOrDefault(Function(p) p.WarehouseSize = decodedLength)
        If profile Is Nothing Then
            Throw New InvalidDataException("Warehouse size " & decodedLength & " does not match " &
                                           String.Join(", ", sameCode.Select(Function(p) p.Name)) & ".")
        End If
        Return New WarehouseFile(filePath, profile, decoded, BitConverter.ToUInt32(data, ChkSumOffset))
    End Function

    ''' <summary>Soma de (char com sinal) * (posicao + 1) sobre o sWAREHOUSE (SaveWareHouse em netplay.cpp).</summary>
    Private Shared Function Checksum(ByVal data As Byte(), ByVal length As Integer) As UInteger
        Dim sum As Long = 0
        For i = 0 To length - 1
            Dim b As Long = data(i)
            If b >= 128 Then b -= 256
            sum = (sum + b * (i + 1)) And &HFFFFFFFFL
        Next
        Return CUInt(sum)
    End Function

End Class
