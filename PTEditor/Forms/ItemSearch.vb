''' <summary>Busca itens pelo nome in-game (curingas ? e *) e gera o comando "~/@get".</summary>
Public Class ItemSearch

    ''' <summary>Decide se "Send to shop" esta disponivel (o editor de NPC precisa ter um NPC selecionado).</summary>
    Friend Property CanSendToShop As Func(Of Boolean)

    ''' <summary>Itens (codigo, nome) que o usuario pediu para enviar a loja do NPC.</summary>
    Friend Event SendToShop(ByVal items As List(Of KeyValuePair(Of String, String)))

    Friend Sub AddResult(ByVal code As String, ByVal name As String)
        LWItemsFound.Items.Add(code).SubItems.Add(name)
    End Sub

    Friend Sub Search()
        For Each item In ItemData.SearchByName(tbSearchText.Text)
            AddResult(item.Code, item.Name)
        Next
    End Sub

    Private Sub cmdItemFound_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdItemFound.Click
        Search()
    End Sub

    Private Sub tbSearchText_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles tbSearchText.KeyPress
        If e.KeyChar = ChrW(13) Then Search()
    End Sub

    Private Sub cmdItemsFoundClear_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdItemsFoundClear.Click
        LWItemsFound.Items.Clear()
    End Sub

    Private Sub LWItemsFound_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles LWItemsFound.SelectedIndexChanged
        If LWItemsFound.SelectedItems.Count >= 1 Then
            tbCommand.Text = "~/@get " & LWItemsFound.SelectedItems(0).Text.Trim(""""c)
        End If
    End Sub

    Private Sub CMSItemFound_Opening(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles CMSItemFound.Opening
        SendToShopToolStripMenuItem.Enabled = CanSendToShop IsNot Nothing AndAlso CanSendToShop.Invoke()
    End Sub

    Private Sub SendToShopToolStripMenuItem_Click(ByVal sender As Object, ByVal e As EventArgs) Handles SendToShopToolStripMenuItem.Click
        Dim items As New List(Of KeyValuePair(Of String, String))()
        For Each row As ListViewItem In LWItemsFound.SelectedItems
            items.Add(New KeyValuePair(Of String, String)(row.Text, row.SubItems(1).Text))
        Next
        RaiseEvent SendToShop(items)
    End Sub

End Class
