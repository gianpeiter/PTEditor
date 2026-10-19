''' <summary>
''' Desfazer/Refazer das edicoes do .spc de um mapa (NPC Editor), que ficam em memoria ate o Save. Antes de cada alteracao
''' guarda uma copia dos 50.400 bytes; desfazer devolve essa copia (quem chama a aplica). Mesma ideia do SpawnPointFile.
''' Tambem guarda o conteudo salvo, para saber se ha alteracoes pendentes.
''' </summary>
Friend Class NpcUndoHistory

    Private Const MaxUndo As Integer = 100

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
    ''' <summary>Conteudo igual ao do disco (carregado ou ultimo Save).</summary>
    Private _saved As Byte()

    Friend Sub MarkSaved(ByVal data As Byte())
        _saved = CType(data.Clone(), Byte())
    End Sub

    ''' <summary>True se o conteudo difere do salvo (desfazer ate o estado salvo zera isso).</summary>
    Friend Function IsModified(ByVal data As Byte()) As Boolean
        Return _saved IsNot Nothing AndAlso Not data.SequenceEqual(_saved)
    End Function

    ''' <summary>Guarda o conteudo atual antes de uma alteracao; uma alteracao nova descarta o "refazer".</summary>
    Friend Sub Record(ByVal current As Byte(), ByVal description As String)
        _undo.Add(New Snapshot(current, description))
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

    ''' <summary>Troca o conteudo atual pelo anterior. Retorna os bytes a gravar e a descricao; Nothing se nao ha o que desfazer.</summary>
    Friend Function Undo(ByVal current As Byte(), ByRef description As String) As Byte()
        If _undo.Count = 0 Then Return Nothing
        Dim last As Snapshot = _undo(_undo.Count - 1)
        _undo.RemoveAt(_undo.Count - 1)
        _redo.Add(New Snapshot(current, last.Description))
        description = last.Description
        Return last.Data
    End Function

    ''' <summary>Refaz a alteracao desfeita (mesmo contrato do Undo).</summary>
    Friend Function Redo(ByVal current As Byte(), ByRef description As String) As Byte()
        If _redo.Count = 0 Then Return Nothing
        Dim last As Snapshot = _redo(_redo.Count - 1)
        _redo.RemoveAt(_redo.Count - 1)
        _undo.Add(New Snapshot(current, last.Description))
        description = last.Description
        Return last.Data
    End Function

End Class
