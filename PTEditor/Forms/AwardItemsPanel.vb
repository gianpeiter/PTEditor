''' <summary>
''' Aba Award Items: envia itens para a fila do NPC com *AwardItem ([PristonDB].[dbo].[AwardItem_List]), mostra e
''' remove os pendentes de um personagem. A conexao vem do server.ini da pasta do servidor.
''' Substitui o antigo ItemDistributor, que gravava no PostBox (o NPC do servidor nao le o PostBox).
''' </summary>
Public Class AwardItemsPanel

    ''' <summary>Fornece o indice de personagens (para a lista de nomes e classe/level).</summary>
    Friend Property IndexProvider As Func(Of CharacterIndex)

    Private _config As DatabaseConfig
    Private _repository As AwardItemRepository
    Private _setupDone As Boolean
    ''' <summary>Personagem cujos pendentes estao na lista.</summary>
    Private _pendingFor As String = ""

    Private Sub AwardItemsPanel_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        EnsureSetup()
    End Sub

    Private Sub EnsureSetup()
        If _setupDone Then Return
        _setupDone = True
        lvSearch.Columns.Add("Code", 70)
        lvSearch.Columns.Add("Name", 260)
        lvSearch.Columns.Add("Level", 50)
        lvSearch.Columns.Add("Weight", 60)
        lvSearch.Columns.Add("File", 150)
        lvSend.Columns.Add("Code", 70)
        lvSend.Columns.Add("Name shown at the NPC", 260)
        lvSend.Columns.Add("Weight", 60)
        lvPending.Columns.Add("Id", 50)
        lvPending.Columns.Add("Code", 60)
        lvPending.Columns.Add("Item name", 180)
        UpdateSendInfo()
    End Sub

    ''' <summary>Chamado ao abrir a aba: le o server.ini e a lista de personagens.</summary>
    Friend Sub EnsureLoaded()
        EnsureSetup()
        LoadConfig()
        FillCharacters()
    End Sub

#Region "Banco"

    Private Sub LoadConfig()
        Try
            _config = DatabaseConfig.Load(AppState.ServerPath)
            _repository = New AwardItemRepository(_config)
            lblDbInfo.Text = _config.Description
            If lblDbStatus.Text = "" Then SetStatus("Not tested yet. Click 'Test connection'.", SystemColors.GrayText)
        Catch ex As Exception
            _config = Nothing
            _repository = Nothing
            lblDbInfo.Text = "(not configured)"
            SetStatus(ex.Message & "  Set the server folder in Options.", Color.Firebrick)
        End Try
    End Sub

    Private Sub SetStatus(ByVal text As String, ByVal color As Color)
        lblDbStatus.Text = text
        lblDbStatus.ForeColor = color
    End Sub

    ''' <summary>Repositorio pronto, ou Nothing com a mensagem ja mostrada.</summary>
    Private Function Repository() As AwardItemRepository
        If _repository Is Nothing Then LoadConfig()
        If _repository Is Nothing Then MsgBox(lblDbStatus.Text, MsgBoxStyle.Exclamation, "Award Items")
        Return _repository
    End Function

    Private Sub cmdTest_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdTest.Click
        LoadConfig()
        If _repository Is Nothing Then Return
        Cursor = Cursors.WaitCursor
        Try
            _repository.Test()
            SetStatus("Connected: database " & AwardItemRepository.DatabaseName & ", table AwardItem_List OK.", Color.DarkGreen)
        Catch ex As Exception
            SetStatus("Connection failed: " & ex.Message, Color.Firebrick)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>Executa uma operacao no banco mostrando o erro de forma legivel. False se falhou.</summary>
    Private Function RunDb(ByVal action As Action, ByVal what As String) As Boolean
        Cursor = Cursors.WaitCursor
        Try
            action()
            Return True
        Catch ex As Exception
            SetStatus(what & " failed: " & ex.Message, Color.Firebrick)
            ErrorLog.Write("AwardItems: " & what & vbCrLf & ex.ToString())
            MsgBox(what & " failed:" & vbCrLf & ex.Message, MsgBoxStyle.Exclamation, "Award Items")
            Return False
        Finally
            Cursor = Cursors.Default
        End Try
    End Function

#End Region

#Region "Destinatario e pendentes"

    Private Sub FillCharacters()
        Dim index As CharacterIndex = AppState.CharacterIndex
        If index Is Nothing AndAlso IndexProvider IsNot Nothing Then index = IndexProvider.Invoke()
        Dim current As String = cbCharacter.Text
        cbCharacter.BeginUpdate()
        cbCharacter.Items.Clear()
        If index IsNot Nothing Then
            For Each entry In index.Characters.Where(Function(c) c.LoadError = "")
                cbCharacter.Items.Add(entry.Name)
            Next
        End If
        cbCharacter.EndUpdate()
        cbCharacter.Text = current
        UpdateRecipientInfo()
    End Sub

    Private Function RecipientName() As String
        Return cbCharacter.Text.Trim()
    End Function

    Private Function FindCharacter(ByVal name As String) As CharacterEntry
        Dim index As CharacterIndex = AppState.CharacterIndex
        If index Is Nothing OrElse name = "" Then Return Nothing
        Return index.Characters.FirstOrDefault(Function(c) c.LoadError = "" AndAlso c.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
    End Function

    Private Sub cbCharacter_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cbCharacter.TextChanged, cbCharacter.SelectedIndexChanged
        UpdateRecipientInfo()
        If Not RecipientName().Equals(_pendingFor, StringComparison.OrdinalIgnoreCase) Then
            lvPending.Items.Clear()
            lblPendingInfo.Text = ""
            _pendingFor = ""
        End If
    End Sub

    Private Sub UpdateRecipientInfo()
        Dim name As String = RecipientName()
        If name = "" Then
            lblRecipientInfo.Text = "Type or pick the character name. The NPC matches the exact character name."
            lblRecipientInfo.ForeColor = SystemColors.GrayText
            Return
        End If
        Dim entry = FindCharacter(name)
        If entry IsNot Nothing Then
            lblRecipientInfo.Text = "Account: " & entry.Account & vbCrLf & entry.ClassName & " Lv " & entry.Level & vbCrLf &
                                    "Items are created with the " & entry.ClassName & " spec (the server uses the current class)."
            lblRecipientInfo.ForeColor = SystemColors.ControlText
        ElseIf AppState.CharacterIndex Is Nothing Then
            lblRecipientInfo.Text = "Load the DataServer in the Characters tab to check the name, class and level."
            lblRecipientInfo.ForeColor = SystemColors.GrayText
        Else
            lblRecipientInfo.Text = "Character not found in the loaded DataServer. Check the spelling: items for a wrong name are never claimed."
            lblRecipientInfo.ForeColor = Color.DarkOrange
        End If
    End Sub

    Private Sub cmdShowPending_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdShowPending.Click
        RefreshPending(True)
    End Sub

    Private Sub RefreshPending(ByVal showErrors As Boolean)
        Dim name As String = RecipientName()
        If name = "" Then
            If showErrors Then MsgBox("Type the character name first.", MsgBoxStyle.Information)
            Return
        End If
        Dim repo = Repository()
        If repo Is Nothing Then Return
        Dim pending As List(Of AwardEntry) = Nothing
        If Not RunDb(Sub() pending = repo.ListPending(name), "Reading pending awards") Then Return
        _pendingFor = name
        lvPending.BeginUpdate()
        lvPending.Items.Clear()
        For Each entry In pending
            Dim row = lvPending.Items.Add(entry.Id.ToString())
            row.SubItems.Add(entry.Code)
            row.SubItems.Add(entry.ItemName)
            row.Tag = entry
            If AwardItemRepository.ValidateCode(entry.Code) <> "" Then row.ForeColor = Color.Firebrick
        Next
        lvPending.EndUpdate()
        Dim invalid As Integer = pending.Where(Function(p) AwardItemRepository.ValidateCode(p.Code) <> "").Count()
        Dim text As String = pending.Count & " pending award(s) for " & name & "."
        If pending.Count > AwardItemRepository.NpcListLimit Then
            text &= " The NPC shows " & AwardItemRepository.NpcListLimit & " per visit; the rest appear after claiming."
        End If
        If invalid > 0 Then text &= " " & invalid & " with an invalid code (red): the server deletes them without delivering."
        lblPendingInfo.Text = text
        lblPendingInfo.ForeColor = If(invalid > 0, Color.Firebrick, SystemColors.ControlText)
        SetStatus("Connected: " & pending.Count & " pending award(s) for " & name & ".", Color.DarkGreen)
    End Sub

    Private Sub cmdRemovePending_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRemovePending.Click
        If lvPending.SelectedItems.Count = 0 Then
            MsgBox("Select a pending award.", MsgBoxStyle.Information)
            Return
        End If
        Dim entries = lvPending.SelectedItems.Cast(Of ListViewItem)().Select(Function(r) DirectCast(r.Tag, AwardEntry)).ToList()
        If Not Dialogs.Confirm("Remove pending awards", "Remove " & entries.Count & " pending award(s) of " & _pendingFor & "?" & vbCrLf & vbCrLf &
                               String.Join(vbCrLf, entries.Select(Function(a) "  " & a.Code & "  " & a.ItemName))) Then Return
        Dim repo = Repository()
        If repo Is Nothing Then Return
        Dim removed As New List(Of AwardEntry)()
        If Not RunDb(Sub()
                         For Each entry In entries
                             If repo.Remove(entry.Id, _pendingFor) Then removed.Add(entry)
                         Next
                     End Sub, "Removing pending awards") Then Return
        AwardLog.Write("REMOVE", _pendingFor, removed)
        RefreshPending(False)
        If removed.Count < entries.Count Then
            MsgBox((entries.Count - removed.Count) & " award(s) were no longer pending (already claimed at the NPC).", MsgBoxStyle.Information)
        End If
    End Sub

#End Region

#Region "Itens"

    Private Sub cmdSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSearch.Click
        Search()
    End Sub

    Private Sub tbSearch_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs) Handles tbSearch.KeyPress
        If e.KeyChar = ChrW(13) Then
            e.Handled = True
            Search()
        End If
    End Sub

    ''' <summary>Busca por codigo ou nome nos itens do OpenItem ja carregados.</summary>
    Private Sub Search()
        Dim query As String = tbSearch.Text.Trim()
        If AppState.Items.Count = 0 Then
            MsgBox("No OpenItem files loaded. Set the server folder in Options and reload.", MsgBoxStyle.Information)
            Return
        End If
        lvSearch.BeginUpdate()
        lvSearch.Items.Clear()
        For Each item In AppState.Items
            Dim name As String = GameItem.CleanName(item.Name)
            If query <> "" AndAlso item.PlainCode.IndexOf(query, StringComparison.OrdinalIgnoreCase) < 0 AndAlso
               name.IndexOf(query, StringComparison.OrdinalIgnoreCase) < 0 Then Continue For
            Dim row = lvSearch.Items.Add(item.PlainCode)
            row.SubItems.Add(name)
            row.SubItems.Add(item.Level)
            row.SubItems.Add(item.Weight)
            row.SubItems.Add(item.FileName)
            row.Tag = item
            If AwardItemRepository.ValidateCode(item.PlainCode) <> "" Then row.ForeColor = SystemColors.GrayText
        Next
        lvSearch.EndUpdate()
    End Sub

    Private Sub cmdAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdAdd.Click, lvSearch.DoubleClick
        AddItems(lvSearch.SelectedItems.Cast(Of ListViewItem)().Select(Function(r) DirectCast(r.Tag, ItemFile)).ToList())
    End Sub

    ''' <summary>Acrescenta itens a lista de envio (tambem usado pelo menu "Send to Award Items" da aba Items).</summary>
    Friend Sub AddItems(ByVal items As IEnumerable(Of ItemFile))
        EnsureSetup()
        Dim rejected As New List(Of String)()
        For Each item In items
            Dim problem As String = AwardItemRepository.ValidateCode(item.PlainCode)
            If problem <> "" Then
                rejected.Add(problem)
                Continue For
            End If
            Dim entry As New AwardEntry With {.Code = item.PlainCode.ToUpperInvariant(),
                                              .ItemName = AwardItemRepository.DisplayName(GameItem.CleanName(item.Name))}
            Dim row = lvSend.Items.Add(entry.Code)
            row.SubItems.Add(entry.ItemName)
            row.SubItems.Add(item.Weight)
            row.Tag = entry
        Next
        UpdateSendInfo()
        If rejected.Count > 0 Then MsgBox("Not added:" & vbCrLf & String.Join(vbCrLf, rejected), MsgBoxStyle.Exclamation, "Award Items")
    End Sub

    Private Sub cmdRemoveSend_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRemoveSend.Click
        For Each row In lvSend.SelectedItems.Cast(Of ListViewItem)().ToList()
            lvSend.Items.Remove(row)
        Next
        UpdateSendInfo()
    End Sub

    Private Function TotalWeight() As Double
        Dim total As Double = 0
        For Each row As ListViewItem In lvSend.Items
            Dim weight As Double
            If Double.TryParse(row.SubItems(2).Text, weight) Then total += weight
        Next
        Return total
    End Function

    Private Sub UpdateSendInfo()
        lblSendInfo.Text = lvSend.Items.Count & " item(s), total weight " & TotalWeight().ToString("0.#") & ". " &
            "The player claims them at the NPC. If the inventory is full or over weight at that moment, the item drops to the ground." &
            " The NPC lists " & AwardItemRepository.NpcListLimit & " items per visit."
        cmdSend.Enabled = lvSend.Items.Count > 0
    End Sub

    Private Sub cmdSend_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSend.Click
        Dim name As String = RecipientName()
        If name = "" Then
            MsgBox("Type the character name.", MsgBoxStyle.Information)
            Return
        End If
        Dim entries = lvSend.Items.Cast(Of ListViewItem)().Select(Function(r) DirectCast(r.Tag, AwardEntry)).ToList()
        If entries.Count = 0 Then Return

        Dim text As String = "Send " & entries.Count & " item(s) to " & name & ":" & vbCrLf &
                             String.Join(vbCrLf, entries.Select(Function(a) "  " & a.Code & "  " & a.ItemName)) & vbCrLf & vbCrLf &
                             "Total weight: " & TotalWeight().ToString("0.#") & vbCrLf
        Dim character = FindCharacter(name)
        If character IsNot Nothing Then
            text &= "Items will have the " & character.ClassName & " spec (current class)." & vbCrLf
        ElseIf AppState.CharacterIndex IsNot Nothing Then
            text &= vbCrLf & "WARNING: '" & name & "' was not found in the loaded DataServer. Items for a wrong name are never claimed." & vbCrLf
        End If
        text &= vbCrLf & "If the inventory is full or over weight when claiming, the item drops to the ground."
        If Not Dialogs.Confirm("Send award items", text, If(character Is Nothing, MsgBoxStyle.Exclamation, MsgBoxStyle.Question)) Then Return

        Dim repo = Repository()
        If repo Is Nothing Then Return
        If Not RunDb(Sub() repo.Add(name, entries), "Sending items") Then Return
        AwardLog.Write("SEND", name, entries)
        lvSend.Items.Clear()
        UpdateSendInfo()
        RefreshPending(False)
        SetStatus("Sent " & entries.Count & " item(s) to " & name & ". Log: " & AwardLog.FilePath, Color.DarkGreen)
    End Sub

#End Region

End Class
