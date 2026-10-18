''' <summary>
''' Aba PostBox: envia itens para a caixa do NPC com *경품배달 (Item Distributor), que le PostBox\&lt;code&gt;\&lt;conta&gt;.dat.
''' Mesmo fluxo da aba Award Items, que usa outro sistema (tabela AwardItem_List); os dois podem coexistir no servidor.
''' </summary>
Public Class PostBoxPanel

    ''' <summary>Fornece o indice de personagens (para a lista de nomes, conta e classe).</summary>
    Friend Property IndexProvider As Func(Of CharacterIndex)

    ''' <summary>Pede para abrir a aba Options (onde a pasta do servidor e configurada).</summary>
    Friend Event OptionsRequested()

    Private _setupDone As Boolean
    ''' <summary>Conta cujos pendentes estao na lista.</summary>
    Private _pendingFor As String = ""

    Private Sub PostBoxPanel_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
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
        lvSend.Columns.Add("Name", 260)
        lvSend.Columns.Add("Spec", 130)
        lvSend.Columns.Add("Weight", 60)
        lvPending.Columns.Add("For", 70)
        lvPending.Columns.Add("Code", 55)
        lvPending.Columns.Add("Item name", 110)
        lvPending.Columns.Add("Spec", 45)
        lvPending.Columns.Add("Message", 150)
        tbMessage.MaxLength = PostBoxRepository.MaxDescriptionLength
        UpdateSendInfo()
    End Sub

    ''' <summary>Chamado ao abrir a aba: mostra a pasta e a lista de personagens.</summary>
    Friend Sub EnsureLoaded()
        EnsureSetup()
        ShowFolder()
        FillCharacters()
    End Sub

#Region "Pasta"

    Private Sub ShowFolder()
        If AppState.ServerPath = "" Then
            lblFolderInfo.Text = "(server folder not set)   -   set in Options"
            lblFolderInfo.ForeColor = Color.Firebrick
        Else
            Dim exists As Boolean = IO.Directory.Exists(PostBoxRepository.Folder)
            lblFolderInfo.Text = PostBoxRepository.Folder & If(exists, "", "   (does not exist yet: created on the first send)")
            lblFolderInfo.ForeColor = If(exists, SystemColors.ControlText, Color.DarkOrange)
        End If
    End Sub

    Private Sub cmdOptions_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdOptions.Click
        RaiseEvent OptionsRequested()
    End Sub

    Private Sub SetStatus(ByVal text As String, ByVal color As Color)
        lblStatus.Text = text
        lblStatus.ForeColor = color
    End Sub

    ''' <summary>Executa uma operacao no arquivo mostrando o erro de forma legivel. False se falhou.</summary>
    Private Function RunFile(ByVal action As Action, ByVal what As String) As Boolean
        Cursor = Cursors.WaitCursor
        Try
            action()
            Return True
        Catch ex As Exception
            SetStatus(what & " failed: " & ex.Message, Color.Firebrick)
            ErrorLog.Write("PostBox: " & what & vbCrLf & ex.ToString())
            MsgBox(what & " failed:" & vbCrLf & ex.Message, MsgBoxStyle.Exclamation, "PostBox")
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

    Private Function FindCharacter(ByVal name As String) As CharacterEntry
        Dim index As CharacterIndex = AppState.CharacterIndex
        If index Is Nothing OrElse name = "" Then Return Nothing
        Return index.Characters.FirstOrDefault(Function(c) c.LoadError = "" AndAlso c.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
    End Function

    ''' <summary>Personagem escolhido, se existe no DataServer e tem conta (o PostBox e por conta).</summary>
    Private Function Recipient() As CharacterEntry
        Dim entry = FindCharacter(cbCharacter.Text.Trim())
        If entry Is Nothing OrElse entry.Account = "" OrElse entry.Account = CharacterIndex.NoAccount Then Return Nothing
        Return entry
    End Function

    ''' <summary>Valor do primeiro campo da linha: o nome do personagem ou "*".</summary>
    Private Function TargetName(ByVal entry As CharacterEntry) As String
        Return If(chkAnyCharacter.Checked, "*", entry.Name)
    End Function

    Private Sub cbCharacter_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles cbCharacter.TextChanged, cbCharacter.SelectedIndexChanged
        UpdateRecipientInfo()
        Dim entry = Recipient()
        If entry IsNot Nothing Then nudSpec.Value = Math.Min(nudSpec.Maximum, Math.Max(0, entry.JobCode))
        If entry Is Nothing OrElse Not entry.Account.Equals(_pendingFor, StringComparison.OrdinalIgnoreCase) Then
            lvPending.Items.Clear()
            lblPendingInfo.Text = ""
            _pendingFor = ""
        End If
    End Sub

    Private Sub chkAnyCharacter_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) Handles chkAnyCharacter.CheckedChanged
        UpdateRecipientInfo()
    End Sub

    Private Sub UpdateRecipientInfo()
        Dim name As String = cbCharacter.Text.Trim()
        If name = "" Then
            lblRecipientInfo.Text = "Pick a character: the PostBox is one file per account, so the character must exist in the loaded DataServer."
            lblRecipientInfo.ForeColor = SystemColors.GrayText
            Return
        End If
        Dim found = FindCharacter(name)
        Dim entry = Recipient()
        If entry IsNot Nothing Then
            lblRecipientInfo.Text = "Account: " & entry.Account & "   (" & entry.ClassName & " Lv " & entry.Level & ")" & vbCrLf &
                                    If(chkAnyCharacter.Checked, "Any character of this account can claim the items.",
                                       "Only " & entry.Name & " can claim the items.") & vbCrLf &
                                    "File: " & PostBoxRepository.FilePath(entry.Account)
            lblRecipientInfo.ForeColor = SystemColors.ControlText
        ElseIf found IsNot Nothing Then
            lblRecipientInfo.Text = "No account lists " & found.Name & " in userinfo: there is no PostBox file to write to."
            lblRecipientInfo.ForeColor = Color.Firebrick
        ElseIf AppState.CharacterIndex Is Nothing Then
            lblRecipientInfo.Text = "Load the DataServer in the Characters tab first: the account comes from it."
            lblRecipientInfo.ForeColor = SystemColors.GrayText
        Else
            lblRecipientInfo.Text = "Character not found in the loaded DataServer."
            lblRecipientInfo.ForeColor = Color.DarkOrange
        End If
    End Sub

    Private Sub cmdShowPending_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdShowPending.Click
        RefreshPending(True)
    End Sub

    Private Sub RefreshPending(ByVal showErrors As Boolean)
        Dim entry = Recipient()
        If entry Is Nothing Then
            If showErrors Then MsgBox("Pick a character of the loaded DataServer first.", MsgBoxStyle.Information)
            Return
        End If
        Dim pending As List(Of PostBoxEntry) = Nothing
        If Not RunFile(Sub() pending = PostBoxRepository.ListPending(entry.Account), "Reading the PostBox") Then Return
        _pendingFor = entry.Account
        Dim invalid As Integer = 0
        lvPending.BeginUpdate()
        lvPending.Items.Clear()
        For Each p In pending
            Dim row = lvPending.Items.Add(p.Character)
            row.SubItems.Add(p.Code)
            row.SubItems.Add(ItemName(p.Code))
            row.SubItems.Add(p.Spec.ToString())
            row.SubItems.Add(If(p.HasPassword, "[password] ", "") & p.Description)
            row.Tag = p
            If IsSpecialCode(p.Code) Then Continue For
            If PostBoxRepository.ValidateCode(p.Code) <> "" Then
                row.ForeColor = Color.Firebrick
                invalid += 1
            End If
        Next
        lvPending.EndUpdate()
        Dim text As String = pending.Count & " pending line(s) in the PostBox of account " & entry.Account & "."
        If invalid > 0 Then text &= " " & invalid & " in red: the NPC never offers them (unknown or unique code)."
        lblPendingInfo.Text = text
        lblPendingInfo.ForeColor = If(invalid > 0, Color.Firebrick, SystemColors.ControlText)
        SetStatus(pending.Count & " pending line(s) for account " & entry.Account & ".", Color.DarkGreen)
    End Sub

    ''' <summary>MONEY e EXP sao linhas validas sem item no OpenItem.</summary>
    Private Shared Function IsSpecialCode(ByVal code As String) As Boolean
        Return code.Equals("MONEY", StringComparison.OrdinalIgnoreCase) OrElse code.Equals("EXP", StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Function ItemName(ByVal code As String) As String
        If IsSpecialCode(code) Then Return code.ToUpperInvariant()
        Dim item = ItemData.FindByCode("""" & code.ToUpperInvariant() & """")
        Return If(item Is Nothing, "?", GameItem.CleanName(item.Name))
    End Function

    Private Sub cmdRemovePending_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRemovePending.Click
        If lvPending.SelectedItems.Count = 0 Then
            MsgBox("Select a pending line.", MsgBoxStyle.Information)
            Return
        End If
        Dim entries = lvPending.SelectedItems.Cast(Of ListViewItem)().Select(Function(r) DirectCast(r.Tag, PostBoxEntry)).ToList()
        If Not Dialogs.Confirm("Remove pending items", "Remove " & entries.Count & " line(s) from the PostBox of account " & _pendingFor & "?" & vbCrLf & vbCrLf &
                               String.Join(vbCrLf, entries.Select(Function(p) "  " & p.Character & "  " & p.Code & "  " & ItemName(p.Code)))) Then Return
        Dim account As String = _pendingFor
        Dim removed As Integer = 0
        If Not RunFile(Sub() removed = PostBoxRepository.Remove(account, entries), "Removing pending items") Then Return
        PostBoxLog.Write("REMOVE", account, entries)
        RefreshPending(False)
        If removed < entries.Count Then
            MsgBox((entries.Count - removed) & " line(s) were no longer in the file (already claimed at the NPC).", MsgBoxStyle.Information)
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

    ''' <summary>Busca por codigo ou nome nos itens do OpenItem ja carregados (vazio lista todos).</summary>
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
            If PostBoxRepository.ValidateCode(item.PlainCode) <> "" Then row.ForeColor = SystemColors.GrayText
        Next
        lvSearch.EndUpdate()
    End Sub

    Private Sub cmdAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdAdd.Click, lvSearch.DoubleClick
        AddItems(lvSearch.SelectedItems.Cast(Of ListViewItem)().Select(Function(r) DirectCast(r.Tag, ItemFile)).ToList())
    End Sub

    ''' <summary>Acrescenta itens a lista de envio (tambem usado pelo menu "Send to PostBox" da aba Items).</summary>
    Friend Sub AddItems(ByVal items As IEnumerable(Of ItemFile))
        EnsureSetup()
        Dim rejected As New List(Of String)()
        For Each item In items
            Dim problem As String = PostBoxRepository.ValidateCode(item.PlainCode)
            If problem <> "" Then
                rejected.Add(problem)
                Continue For
            End If
            Dim code As String = item.PlainCode.ToUpperInvariant()
            Dim potion As Boolean = PostBoxRepository.IsPotion(code)
            Dim entry As New PostBoxEntry With {.Code = code, .Spec = CInt(If(potion, nudQuantity.Value, nudSpec.Value))}
            Dim row = lvSend.Items.Add(entry.Code)
            row.SubItems.Add(GameItem.CleanName(item.Name))
            row.SubItems.Add(SpecText(entry.Spec, potion))
            row.SubItems.Add(item.Weight)
            row.Tag = entry
        Next
        UpdateSendInfo()
        If rejected.Count > 0 Then MsgBox("Not added:" & vbCrLf & String.Join(vbCrLf, rejected), MsgBoxStyle.Exclamation, "PostBox")
    End Sub

    Private Shared Function SpecText(ByVal spec As Integer, ByVal potion As Boolean) As String
        If potion Then Return "Quantity " & spec
        If spec = 0 Then Return "0 (random class)"
        Return spec & " (" & GameTables.ClassName(spec) & ")"
    End Function

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
            If Double.TryParse(row.SubItems(3).Text, weight) Then total += weight
        Next
        Return total
    End Function

    Private Sub UpdateSendInfo()
        lblSendInfo.Text = lvSend.Items.Count & " item(s), total weight " & TotalWeight().ToString("0.#") & ". " &
            "The NPC offers one item at a time; the client checks space and weight before accepting, so the player can claim later." &
            " Spec is fixed when sending: weapons/armor use the class spec, potions the quantity."
        cmdSend.Enabled = lvSend.Items.Count > 0
    End Sub

    Private Sub cmdSend_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSend.Click
        Dim entry = Recipient()
        If entry Is Nothing Then
            MsgBox("Pick a character of the loaded DataServer (the PostBox file is chosen by its account).", MsgBoxStyle.Information)
            Return
        End If
        If AppState.ServerPath = "" Then
            MsgBox("Set the server folder in Options first.", MsgBoxStyle.Information)
            Return
        End If
        Dim target As String = TargetName(entry)
        Dim description As String = PostBoxRepository.CleanDescription(tbMessage.Text)
        Dim items = lvSend.Items.Cast(Of ListViewItem)().Select(Function(r)
                                                                   Dim p = DirectCast(r.Tag, PostBoxEntry)
                                                                   Return New PostBoxEntry With {.Character = target, .Code = p.Code,
                                                                                                 .Spec = p.Spec, .Description = description}
                                                               End Function).ToList()
        If items.Count = 0 Then Return

        Dim text As String = "Send " & items.Count & " item(s) to the PostBox of account " & entry.Account & ":" & vbCrLf &
                             String.Join(vbCrLf, lvSend.Items.Cast(Of ListViewItem)().Select(Function(r) "  " & r.Text & "  " & r.SubItems(1).Text & "  [" & r.SubItems(2).Text & "]")) & vbCrLf & vbCrLf &
                             "Claimable by: " & If(target = "*", "any character of the account", entry.Name) & vbCrLf &
                             "Message: " & If(description = "", "(empty)", description) & vbCrLf & vbCrLf &
                             "If " & entry.Account & " is online and already opened the PostBox this session, the logout may overwrite this change. Prefer sending with the player offline."
        If Not Dialogs.Confirm("Send to PostBox", text) Then Return

        If Not RunFile(Sub() PostBoxRepository.Add(entry.Account, items), "Sending items") Then Return
        PostBoxLog.Write("SEND", entry.Account, items)
        lvSend.Items.Clear()
        UpdateSendInfo()
        ShowFolder()
        RefreshPending(False)
        SetStatus("Sent " & items.Count & " item(s) to account " & entry.Account & ". Log: " & PostBoxLog.FilePath, Color.DarkGreen)
    End Sub

#End Region

End Class
