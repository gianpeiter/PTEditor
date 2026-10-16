Imports System.IO

''' <summary>
''' DataServer\warehouse\&lt;n&gt;\&lt;conta&gt;.war (TRANS_WAREHOUSE). Cabecalho de 48 bytes e um unico bloco
''' comprimido com o sWAREHOUSE inteiro (100 sITEM + ouro). Somente leitura: o formato ainda nao foi conferido
''' com um bau real deste servidor.
''' </summary>
Friend Class WarehouseFile

    Private Const WarehouseCode As Integer = &H48470047
    Private Const ChkSumOffset As Integer = &H8
    Private Const DataSizeOffset As Integer = &H2C
    Private Const DataOffset As Integer = &H30
    Private Const SlotCount As Integer = 100
    Private Const ItemFlagOffset As Integer = &HCC
    Private Const ItemXOffset As Integer = &HD0
    Private Const ItemYOffset As Integer = &HD4
    Private Const ItemInfoOffset As Integer = &H100
    ''' <summary>sWAREHOUSE.Money guarda o ouro + 2023.</summary>
    Private Const MoneyBias As Integer = 2023

    Public ReadOnly Property FilePath As String
    Public ReadOnly Property Layout As RecordLayout
    Public ReadOnly Property Items As New List(Of GameItem)()
    Public ReadOnly Property Gold As Integer
    ''' <summary>dwChkSum do cabecalho confere com o conteudo (o servidor recusa abrir o bau se nao conferir).</summary>
    Public ReadOnly Property ChecksumOk As Boolean
    Public ReadOnly Property IsEmpty As Boolean

    Private Sub New(ByVal filePath As String)
        Me.FilePath = filePath
        IsEmpty = True
    End Sub

    Private Sub New(ByVal filePath As String, ByVal layout As RecordLayout, ByVal decoded As Byte(), ByVal storedChkSum As UInteger)
        Me.FilePath = filePath
        Me.Layout = layout
        Dim money As Integer = BitConverter.ToInt32(decoded, layout.WarehouseMoneyOffset)
        Gold = If(money = 0, 0, money - MoneyBias)
        ChecksumOk = Checksum(decoded, layout.WarehouseSize) = storedChkSum
        For slot = 0 To SlotCount - 1
            Dim offset As Integer = slot * layout.WarehouseItemSize
            If BitConverter.ToInt32(decoded, offset + ItemFlagOffset) = 0 Then Continue For
            Dim raw(layout.WarehouseItemSize - 1) As Byte
            Array.Copy(decoded, offset, raw, 0, raw.Length)
            Items.Add(New GameItem(raw, ItemInfoOffset, layout, ItemContainer.Warehouse) With {
                .Slot = slot,
                .X = BitConverter.ToInt32(raw, ItemXOffset),
                .Y = BitConverter.ToInt32(raw, ItemYOffset)})
        Next
    End Sub

    Public Shared Function Load(ByVal filePath As String) As WarehouseFile
        Dim data As Byte() = File.ReadAllBytes(filePath)
        If data.Length < DataOffset + 4 OrElse BitConverter.ToInt32(data, 0) = 0 Then Return New WarehouseFile(filePath)
        If BitConverter.ToInt32(data, 4) <> WarehouseCode Then
            Throw New InvalidDataException("Not a warehouse file (code 0x" & BitConverter.ToInt32(data, 4).ToString("X8") & ").")
        End If
        If BitConverter.ToInt32(data, DataSizeOffset) = 0 Then Return New WarehouseFile(filePath)
        Dim decodedLength As Integer
        Dim decoded As Byte() = ItemCompression.Decode(data, DataOffset, RecordLayout.Servidor.WarehouseSize, decodedLength)
        Dim layout As RecordLayout = RecordLayout.FromWarehouseSize(decodedLength)
        If layout Is Nothing Then
            Throw New InvalidDataException("Warehouse with unknown size (" & decodedLength & " decompressed bytes).")
        End If
        Return New WarehouseFile(filePath, layout, decoded, BitConverter.ToUInt32(data, ChkSumOffset))
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
