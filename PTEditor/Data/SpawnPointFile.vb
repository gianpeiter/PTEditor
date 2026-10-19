Imports System.IO

''' <summary>
''' Pontos de nascimento de um mapa (GameServer\Field\*.spp): 200 registros de 12 bytes (state, x, z; Int32).
''' state 1 = ponto ativo (a "flag" do GM no jogo); x/z em unidades de mundo, sem altura.
''' Mantem os bytes originais: adicionar/remover so mexe no registro afetado, como o servidor faz.
''' Cada alteracao guarda uma copia dos 2.400 bytes antes de mudar, para Desfazer/Refazer.
''' </summary>
Friend Class SpawnPointFile

    Friend Const MaxPoints As Integer = 200
    Private Const RecordSize As Integer = 12
    Private Const MaxUndo As Integer = 100

    ''' <summary>Copia do conteudo antes (ou depois) de uma alteracao, com a descricao dela.</summary>
    Private Class Snapshot
        Friend ReadOnly Data As Byte()
        Friend ReadOnly Description As String

        Friend Sub New(ByVal data As Byte(), ByVal description As String)
            Me.Data = CType(data.Clone(), Byte())
            Me.Description = description
        End Sub
    End Class

    Private ReadOnly _undo As New List(Of Snapshot)()
    Private ReadOnly _redo As New List(Of Snapshot)()
    ''' <summary>Conteudo igual ao do disco (carregado ou ultimo Save), para saber se ha alteracoes pendentes.</summary>
    Private _saved As Byte()

    Friend ReadOnly Property FileName As String
    Friend ReadOnly Property Data As Byte()
    ''' <summary>False se o arquivo ainda nao existe (criado no primeiro Save).</summary>
    Friend ReadOnly Property Exists As Boolean

    Friend Sub New(ByVal fileName As String)
        Me.FileName = fileName
        Dim path As String = AppState.FieldPath(fileName)
        _Exists = File.Exists(path)
        If _Exists Then
            Dim bytes As Byte() = File.ReadAllBytes(path)
            If bytes.Length <> MaxPoints * RecordSize Then
                Throw New InvalidDataException(fileName & " has " & bytes.Length & " bytes; expected " & MaxPoints * RecordSize & ".")
            End If
            _Data = bytes
        Else
            _Data = New Byte(MaxPoints * RecordSize - 1) {}
        End If
        _saved = CType(_Data.Clone(), Byte())
    End Sub

    ''' <summary>True se o conteudo difere do que esta no disco (desfazer ate o estado salvo zera isso).</summary>
    Friend ReadOnly Property IsModified As Boolean
        Get
            Return Not Data.SequenceEqual(_saved)
        End Get
    End Property

    Friend Function IsActive(ByVal slot As Integer) As Boolean
        Return BitConverter.ToInt32(Data, slot * RecordSize) <> 0
    End Function

    Friend Function GetX(ByVal slot As Integer) As Integer
        Return BitConverter.ToInt32(Data, slot * RecordSize + 4)
    End Function

    Friend Function GetZ(ByVal slot As Integer) As Integer
        Return BitConverter.ToInt32(Data, slot * RecordSize + 8)
    End Function

    ''' <summary>Slots ativos, em ordem.</summary>
    Friend Function ActiveSlots() As List(Of Integer)
        Dim result As New List(Of Integer)()
        For slot = 0 To MaxPoints - 1
            If IsActive(slot) Then result.Add(slot)
        Next
        Return result
    End Function

    Friend ReadOnly Property ActiveCount As Integer
        Get
            Return ActiveSlots().Count
        End Get
    End Property

    ''' <summary>Slot ativo nessa posicao exata, ou -1.</summary>
    Friend Function Find(ByVal x As Integer, ByVal z As Integer) As Integer
        For slot = 0 To MaxPoints - 1
            If IsActive(slot) AndAlso GetX(slot) = x AndAlso GetZ(slot) = z Then Return slot
        Next
        Return -1
    End Function

    ''' <summary>Usa o primeiro slot livre (igual ao servidor). Retorna o slot, ou -1 se cheio ou se ja existe ponto ali.</summary>
    Friend Function Add(ByVal x As Integer, ByVal z As Integer) As Integer
        If Find(x, z) >= 0 Then Return -1
        For slot = 0 To MaxPoints - 1
            If Not IsActive(slot) Then
                RecordChange("Added point #" & slot)
                SetRecord(slot, 1, x, z)
                Return slot
            End If
        Next
        Return -1
    End Function

    ''' <summary>Move o ponto. False se ja existe outro ponto na posicao nova.</summary>
    Friend Function Move(ByVal slot As Integer, ByVal x As Integer, ByVal z As Integer) As Boolean
        Dim other As Integer = Find(x, z)
        If other >= 0 AndAlso other <> slot Then Return False
        If IsActive(slot) AndAlso GetX(slot) = x AndAlso GetZ(slot) = z Then Return True
        RecordChange("Moved point #" & slot)
        SetRecord(slot, 1, x, z)
        Return True
    End Function

    Friend Sub Remove(ByVal slot As Integer)
        RecordChange("Removed point #" & slot)
        SetRecord(slot, 0, 0, 0)
    End Sub

    Friend Sub Clear()
        RecordChange("Removed all points")
        Array.Clear(Data, 0, Data.Length)
    End Sub

    Private Sub SetRecord(ByVal slot As Integer, ByVal state As Integer, ByVal x As Integer, ByVal z As Integer)
        BitConverter.GetBytes(state).CopyTo(Data, slot * RecordSize)
        BitConverter.GetBytes(x).CopyTo(Data, slot * RecordSize + 4)
        BitConverter.GetBytes(z).CopyTo(Data, slot * RecordSize + 8)
    End Sub

#Region "Desfazer / Refazer"

    ''' <summary>Guarda o estado atual antes de uma alteracao; uma alteracao nova descarta o "refazer".</summary>
    Private Sub RecordChange(ByVal description As String)
        _undo.Add(New Snapshot(Data, description))
        If _undo.Count > MaxUndo Then _undo.RemoveAt(0)
        _redo.Clear()
    End Sub

    Friend ReadOnly Property CanUndo As Boolean
        Get
            Return _undo.Count > 0
        End Get
    End Property

    Friend ReadOnly Property CanRedo As Boolean
        Get
            Return _redo.Count > 0
        End Get
    End Property

    ''' <summary>Desfaz a ultima alteracao. Retorna a descricao dela, ou "" se nao havia nada a desfazer.</summary>
    Friend Function Undo() As String
        If _undo.Count = 0 Then Return ""
        Dim last As Snapshot = _undo(_undo.Count - 1)
        _undo.RemoveAt(_undo.Count - 1)
        _redo.Add(New Snapshot(Data, last.Description))
        Array.Copy(last.Data, Data, Data.Length)
        Return last.Description
    End Function

    ''' <summary>Refaz a alteracao desfeita. Retorna a descricao dela, ou "" se nao havia nada a refazer.</summary>
    Friend Function Redo() As String
        If _redo.Count = 0 Then Return ""
        Dim last As Snapshot = _redo(_redo.Count - 1)
        _redo.RemoveAt(_redo.Count - 1)
        _undo.Add(New Snapshot(Data, last.Description))
        Array.Copy(last.Data, Data, Data.Length)
        Return last.Description
    End Function

#End Region

    ''' <summary>Faz backup do arquivo existente e grava. Atualiza a copia em memoria de AppState.SppFiles.</summary>
    Friend Sub Save()
        Dim folder As String = AppState.ServerPath & AppState.FieldFolder
        BackupHelper.CreateBackup(folder, FileName)
        File.WriteAllBytes(folder & FileName, Data)
        _Exists = True
        _saved = CType(Data.Clone(), Byte())
        Dim loaded = AppState.SppFiles.FirstOrDefault(Function(f) f.FileName.Equals(FileName, StringComparison.OrdinalIgnoreCase))
        If loaded IsNot Nothing Then loaded.Data = CType(Data.Clone(), Byte())
    End Sub

    ''' <summary>Nome do .spp de um mapa: o do .spm com a extensao trocada ("Dun-6a.ase.spm" vira "Dun-6a.ase.spp").</summary>
    Friend Shared Function NameForMap(ByVal mapFileName As String) As String
        Return Path.ChangeExtension(mapFileName, ".spp")
    End Function

End Class
