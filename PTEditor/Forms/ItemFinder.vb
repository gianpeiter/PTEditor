Imports System.IO
Imports System.Text

''' <summary>
''' Aba Item Finder: busca itens em todos os personagens e baus, lista copias (mesmo CODE + Head + CheckSum)
''' e itens com checksum invalido. Substitui o "Procura item pelo nome" e a busca do ControllerUser.
''' </summary>
Public Class ItemFinder

    ''' <summary>Fornece o indice de personagens (a aba Characters carrega sob demanda).</summary>
    Friend Property IndexProvider As Func(Of CharacterIndex)

    ''' <summary>Pedido para abrir um personagem (nome, abrir aba do bau).</summary>
    Friend Event OpenCharacterRequested(ByVal name As String, ByVal showWarehouse As Boolean)

    Private _items As List(Of ItemLocation)
    Private _scannedIndex As CharacterIndex
    Private _scannedWarehouses As Boolean
    Private _scanErrors As New List(Of String)()

    Private Sub ItemFinder_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        cbMode.SelectedIndex = 0
        SetColumns(False)
    End Sub

    Private Sub SetColumns(ByVal withGroup As Boolean)
        lvResults.Columns.Clear()
        If withGroup Then lvResults.Columns.Add("Group", 50)
        lvResults.Columns.Add("Owner", 130)
        lvResults.Columns.Add("Account", 100)
        lvResults.Columns.Add("Place", 120)
        lvResults.Columns.Add("Code", 60)
        lvResults.Columns.Add("Name", 170)
        lvResults.Columns.Add("Aging", 45)
        lvResults.Columns.Add("Head", 75)
        lvResults.Columns.Add("CheckSum", 75)
        lvResults.Columns.Add("Status", 200)
    End Sub

    ''' <summary>Le (ou reaproveita) todos os itens do DataServer.</summary>
    Private Function LoadItems() As Boolean
        Dim index As CharacterIndex = If(IndexProvider Is Nothing, AppState.CharacterIndex, IndexProvider.Invoke())
        If index Is Nothing Then
            MsgBox("Load the DataServer in the Characters tab first.", MsgBoxStyle.Information)
            Return False
        End If
        If _items IsNot Nothing AndAlso index Is _scannedIndex AndAlso _scannedWarehouses = chkWarehouses.Checked Then Return True
        Cursor = Cursors.WaitCursor
        Try
            _scanErrors = New List(Of String)()
            _items = index.ScanItems(chkWarehouses.Checked, _scanErrors, Sub(i, n)
                                                                              If i Mod 25 = 0 OrElse i = n Then
                                                                                  lblStatus.Text = "Reading files: " & i & " of " & n
                                                                                  lblStatus.Refresh()
                                                                              End If
                                                                          End Sub)
            _scannedIndex = index
            _scannedWarehouses = chkWarehouses.Checked
            Return True
        Catch ex As Exception
            ErrorLog.Report("ItemFinder.LoadItems", ex)
            Return False
        Finally
            Cursor = Cursors.Default
        End Try
    End Function

    ''' <summary>Itens lidos novamente apos salvar um personagem.</summary>
    Friend Sub ClearCache()
        _items = Nothing
    End Sub

    Private Sub cmdSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSearch.Click
        Search()
    End Sub

    Private Sub tbQuery_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles tbQuery.KeyPress
        If e.KeyChar = ChrW(13) Then
            e.Handled = True
            Search()
        End If
    End Sub

    Private Sub Search()
        Dim query As String = tbQuery.Text.Trim()
        If Not LoadItems() Then Return
        Dim found As List(Of ItemLocation)
        If query = "" Then
            found = _items.ToList()
        Else
            Dim byCode As Boolean = cbMode.SelectedIndex = 0
            found = _items.Where(Function(l)
                                      If byCode Then Return l.Item.CodeText.StartsWith(query, StringComparison.OrdinalIgnoreCase)
                                      Return l.Item.DisplayName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 OrElse
                                             l.Item.StoredName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0
                                  End Function).ToList()
        End If
        ShowResults(found, Nothing)
        Dim suffix As String = If(query = "", "(all items)", "for '" & query & "'")
        ReportStatus(found.Count & " item(s) found " & suffix & " in " & found.Select(Function(l) l.Owner).Distinct().Count() & " owner(s).")
    End Sub

    Private Sub cmdDuplicates_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDuplicates.Click
        If Not LoadItems() Then Return
        ' Mesmo criterio do servidor: pocoes empilham e nao contam; itens sem cabecalho nao sao identificaveis.
        Dim groups = _items.Where(Function(l) l.Item.Head <> 0 AndAlso l.Item.ChkSum <> 0 AndAlso Not GameTables.IsPotion(l.Item.Code)).
            GroupBy(Function(l) l.Item.Identity).Where(Function(g) g.Count() > 1).
            OrderByDescending(Function(g) g.Count()).ToList()
        Dim rows As New List(Of ItemLocation)()
        Dim groupOf As New Dictionary(Of ItemLocation, Integer)()
        For i = 0 To groups.Count - 1
            For Each l In groups(i)
                rows.Add(l)
                groupOf(l) = i + 1
            Next
        Next
        ShowResults(rows, groupOf)
        ReportStatus(If(groups.Count = 0, "No duplicated items found.",
                        groups.Count & " item(s) with copies (" & rows.Count & " occurrences). Same group = same CODE + Head + CheckSum."))
    End Sub

    Private Sub cmdProblems_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdProblems.Click
        If Not LoadItems() Then Return
        Dim found = _items.Where(Function(l) l.Item.HasProblem).ToList()
        ShowResults(found, Nothing)
        ReportStatus(If(found.Count = 0, "No items with an invalid checksum.",
                        found.Count & " item(s) with problems: the client drops items with an invalid checksum on login."))
    End Sub

    Private Sub ShowResults(ByVal rows As List(Of ItemLocation), ByVal groupOf As Dictionary(Of ItemLocation, Integer))
        lvResults.BeginUpdate()
        SetColumns(groupOf IsNot Nothing)
        lvResults.Items.Clear()
        For Each l In rows
            Dim row As New ListViewItem() With {.Tag = l}
            If groupOf IsNot Nothing Then
                row.Text = groupOf(l).ToString()
                row.SubItems.Add(l.Owner)
            Else
                row.Text = l.Owner
            End If
            row.SubItems.Add(l.Account)
            row.SubItems.Add(l.Item.PlaceName)
            row.SubItems.Add(l.Item.CodeText)
            row.SubItems.Add(l.Item.DisplayName)
            row.SubItems.Add(If(l.Item.Aging > 0, "+" & l.Item.Aging, ""))
            row.SubItems.Add(l.Item.Head.ToString("X8"))
            row.SubItems.Add(l.Item.ChkSum.ToString("X8"))
            row.SubItems.Add(l.Item.Status)
            If l.Item.HasProblem Then row.ForeColor = Color.Firebrick
            If groupOf IsNot Nothing AndAlso groupOf(l) Mod 2 = 0 Then row.BackColor = Color.FromArgb(242, 242, 248)
            lvResults.Items.Add(row)
        Next
        lvResults.EndUpdate()
    End Sub

    Private Sub ReportStatus(ByVal text As String)
        If _scanErrors.Count > 0 Then text &= "  |  " & _scanErrors.Count & " file(s) skipped due to read errors"
        lblStatus.Text = text
    End Sub

    Private Sub lvResults_DoubleClick(ByVal sender As Object, ByVal e As EventArgs) Handles lvResults.DoubleClick
        If lvResults.SelectedItems.Count = 0 Then Return
        Dim l = DirectCast(lvResults.SelectedItems(0).Tag, ItemLocation)
        If l.Item.Container = ItemContainer.Inventory Then
            RaiseEvent OpenCharacterRequested(l.Owner, False)
            Return
        End If
        Dim index = AppState.CharacterIndex
        Dim first = index?.Characters.FirstOrDefault(Function(c) c.Account.Equals(l.Account, StringComparison.OrdinalIgnoreCase))
        If first Is Nothing Then
            MsgBox("No character of account " & l.Account & " found to show the warehouse.", MsgBoxStyle.Information)
        Else
            RaiseEvent OpenCharacterRequested(first.Name, True)
        End If
    End Sub

    Private Sub cmdExport_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdExport.Click
        If lvResults.Items.Count = 0 Then
            MsgBox("Nothing to export: run a search first.", MsgBoxStyle.Information)
            Return
        End If
        dlgSave.FileName = "items.csv"
        If dlgSave.ShowDialog() <> DialogResult.OK Then Return
        Try
            Dim sb As New StringBuilder()
            sb.AppendLine(String.Join(";", lvResults.Columns.Cast(Of ColumnHeader)().Select(Function(c) CsvField(c.Text))))
            For Each row As ListViewItem In lvResults.Items
                sb.AppendLine(String.Join(";", row.SubItems.Cast(Of ListViewItem.ListViewSubItem)().Select(Function(s) CsvField(s.Text))))
            Next
            File.WriteAllText(dlgSave.FileName, sb.ToString(), Encoding.UTF8)
            lblStatus.Text = "Exported: " & dlgSave.FileName
        Catch ex As Exception
            ErrorLog.Report("ItemFinder.Export", ex)
        End Try
    End Sub

    Private Shared Function CsvField(ByVal text As String) As String
        If text.Contains(";") OrElse text.Contains("""") Then Return """" & text.Replace("""", """""") & """"
        Return text
    End Function

End Class
