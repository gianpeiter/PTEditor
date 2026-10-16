Imports System.Text

''' <summary>Onde o item foi encontrado.</summary>
Friend Enum ItemContainer
    Inventory
    Warehouse
End Enum

''' <summary>
''' Um item lido de personagem (sRECORD_ITEM descomprimido) ou bau (sITEM). Guarda os bytes originais para
''' regravar sem alteracao; as propriedades leem o sITEMINFO a partir de InfoOffset.
''' </summary>
Friend Class GameItem

    Public ReadOnly Property Raw As Byte()
    Public ReadOnly Property InfoOffset As Integer
    Public ReadOnly Property Profile As CharacterProfile
    Public ReadOnly Property Container As ItemContainer

    ''' <summary>Indice do slot no inventario do jogo (sRECORD_ITEM.ItemCount) ou no bau.</summary>
    Public Property Slot As Integer
    Public Property X As Integer
    Public Property Y As Integer
    ''' <summary>sRECORD_ITEM.ItemPosition (0 = bolsa, 1..16 = equipado, -1 = mouse).</summary>
    Public Property Position As Integer
    ''' <summary>Item do segundo inventario (indice &gt;= ItemSubStart no arquivo).</summary>
    Public Property SecondInventory As Boolean

    Public Sub New(ByVal raw As Byte(), ByVal infoOffset As Integer, ByVal profile As CharacterProfile, ByVal container As ItemContainer)
        Me.Raw = raw
        Me.InfoOffset = infoOffset
        Me.Profile = profile
        Me.Container = container
    End Sub

    Public ReadOnly Property Code As UInteger
        Get
            Return BitConverter.ToUInt32(Raw, InfoOffset + ItemChecksum.CodeOffset)
        End Get
    End Property

    ''' <summary>Codigo do OpenItem (ex.: WA101).</summary>
    Public ReadOnly Property CodeText As String
        Get
            Return GameTables.ItemCodeText(Code)
        End Get
    End Property

    ''' <summary>Nome gravado no proprio item (encoding do perfil: CP949 coreano, CP1252 nas sources traduzidas).</summary>
    Public ReadOnly Property StoredName As String
        Get
            Dim start As Integer = InfoOffset + &H1C
            Dim length As Integer = 0
            Do While length < 32 AndAlso Raw(start + length) <> 0
                length += 1
            Loop
            Return Profile.NameEncoding.GetString(Raw, start, length)
        End Get
    End Property

    ''' <summary>Nome do OpenItem carregado no editor; senao o nome gravado no item.</summary>
    Public ReadOnly Property DisplayName As String
        Get
            Dim item = ItemData.FindByCode("""" & CodeText & """")
            If item IsNot Nothing AndAlso item.Name <> ItemFile.NoName Then Return CleanName(item.Name)
            Return StoredName
        End Get
    End Property

    Private Shared ReadOnly StrictUtf8 As New UTF8Encoding(False, True)

    ''' <summary>Nome do .zhoon sem aspas; se o .zhoon era UTF-8 (lido como CP1252, ex.: "BuquÃª"), decodifica de volta.</summary>
    Private Shared Function CleanName(ByVal name As String) As String
        name = name.Trim().Trim(""""c)
        If name.IndexOf("Ã"c) >= 0 OrElse name.IndexOf("Â"c) >= 0 Then
            Try
                Return StrictUtf8.GetString(AppState.FileEncoding.GetBytes(name))
            Catch ex As DecoderFallbackException
                ' Nao era UTF-8: mantem como esta.
            End Try
        End If
        Return name
    End Function

    Public ReadOnly Property Head As UInteger
        Get
            Return BitConverter.ToUInt32(Raw, InfoOffset + ItemChecksum.HeadOffset)
        End Get
    End Property

    Public ReadOnly Property ChkSum As UInteger
        Get
            Return BitConverter.ToUInt32(Raw, InfoOffset + ItemChecksum.ChkSumOffset)
        End Get
    End Property

    Public ReadOnly Property FormCode As UInteger
        Get
            Return BitConverter.ToUInt32(Raw, InfoOffset + ItemChecksum.FormCodeOffset)
        End Get
    End Property

    Public ReadOnly Property Durability As String
        Get
            Return BitConverter.ToInt16(Raw, InfoOffset + &H14) & "/" & BitConverter.ToInt16(Raw, InfoOffset + &H16)
        End Get
    End Property

    ''' <summary>ItemAgingNum[0]: nivel de aging (+N).</summary>
    Public ReadOnly Property Aging As Integer
        Get
            Return BitConverter.ToInt16(Raw, InfoOffset + &H1DC)
        End Get
    End Property

    ''' <summary>Copia na mao esquerda de uma arma de duas maos (o jogo grava sem cabecalho).</summary>
    Public ReadOnly Property IsTwoHandedCopy As Boolean
        Get
            Return Container = ItemContainer.Inventory AndAlso Position = 2 AndAlso Head = 0 AndAlso ChkSum = 0
        End Get
    End Property

    ''' <summary>Chave usada para achar copias: mesmo CODE + Head + ChkSum (como o servidor compara).</summary>
    Public ReadOnly Property Identity As String
        Get
            Return Code.ToString("X8") & "-" & Head.ToString("X8") & "-" & ChkSum.ToString("X8")
        End Get
    End Property

    Public ReadOnly Property ClientChecksumOk As Boolean
        Get
            Return ItemChecksum.ClientChecksum(Raw, InfoOffset) = ChkSum
        End Get
    End Property

    Public ReadOnly Property ServerFormOk As Boolean
        Get
            Return ItemChecksum.ServerForm(Raw, InfoOffset, Profile) = FormCode
        End Get
    End Property

    ''' <summary>Resumo para a interface: o que o jogo/servidor vai achar do item.</summary>
    Public ReadOnly Property Status As String
        Get
            If IsTwoHandedCopy Then Return "Copy (two-handed)"
            If Head = 0 OrElse ChkSum = 0 Then Return "No header"
            Dim client As Boolean = ClientChecksumOk
            Dim server As Boolean = ServerFormOk
            If client AndAlso server Then Return "OK"
            If Not client Then Return "Invalid checksum (client drops it)"
            Return "Invalid form (server refuses sell/trade)"
        End Get
    End Property

    Public ReadOnly Property HasProblem As Boolean
        Get
            Return Not IsTwoHandedCopy AndAlso Status <> "OK"
        End Get
    End Property

    Public ReadOnly Property PlaceName As String
        Get
            If Container = ItemContainer.Warehouse Then Return "Warehouse"
            Return If(SecondInventory, "Inv. 2 / ", "") & GameTables.PositionName(Position)
        End Get
    End Property

End Class
