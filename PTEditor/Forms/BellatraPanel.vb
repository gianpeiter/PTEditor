Imports System.IO

''' <summary>
''' Aba Bellatra: faz o papel do site de gerencia do Bellatra do legado (priston.com.br/bellatra). Mostra o clan gerente
''' da semana e a renda ([SoD2DB].[dbo].[Sod2Config]), o ranking da semana ([ClanDB].[dbo].[CL]), edita a saudacao da
''' Karina (Sod2Config.Note), o dia/hora do fechamento (server.ini [Bellatra]) e grava Bellatra\bellatra.ini (taxa,
''' monstros e chefes de cada round), que o servidor rele no inicio de cada SoD. Os pontos (SQL.dll), os fechamentos do
''' dia e da semana e o repasse do imposto (Bellatra::Tick/AddClanTax) continuam automaticos no servidor.
''' </summary>
Public Class BellatraPanel

    ''' <summary>Pede para abrir a aba Options (onde o banco e configurado).</summary>
    Friend Event OptionsRequested()

    ''' <summary>Opcao dos combos de monstro: o *이름 que vai para o arquivo e o texto mostrado.</summary>
    Private NotInheritable Class MonsterChoice
        Public ReadOnly MapName As String
        Public ReadOnly Display As String

        Public Sub New(ByVal mapName As String, ByVal display As String)
            Me.MapName = mapName
            Me.Display = display
        End Sub

        Public Overrides Function ToString() As String
            Return Display
        End Function
    End Class

    ''' <summary>Controles de um round.</summary>
    Private NotInheritable Class RoundRow
        Public ReadOnly Monsters(BellatraConfig.SlotCount - 1) As ComboBox
        Public ReadOnly Percents(BellatraConfig.SlotCount - 1) As NumericUpDown
        Public MaxMonster As NumericUpDown
        ''' <summary>Nothing nos rounds sem chefe.</summary>
        Public Boss As ComboBox
    End Class

    Private Const NoMonster As String = "(none)"
    Private Const RowTop As Integer = 18
    Private Const RowHeight As Integer = 30

    Private ReadOnly _rows As New List(Of RoundRow)()
    Private _choices As MonsterChoice() = {}
    Private ReadOnly _knownNames As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
    ''' <summary>Quantidade de monstros carregados quando a lista dos combos foi montada.</summary>
    Private _choicesFrom As Integer = -1
    Private _config As New BellatraConfig()
    Private _loadedPath As String = ""
    Private _dirty As Boolean
    Private _filling As Boolean
    Private _setupDone As Boolean
    Private _ownerChecked As Boolean
    ''' <summary>Gerencia da semana na ultima consulta (Nothing = nao lida).</summary>
    Private _manager As BellatraManager

    Private Sub BellatraPanel_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        EnsureSetup()
    End Sub

    Private Sub EnsureSetup()
        If _setupDone Then Return
        _setupDone = True
        lvRanking.Columns.Add("#", 30)
        lvRanking.Columns.Add("Clan", 130)
        lvRanking.Columns.Add("Points (week)", 80)
        lvRanking.Columns.Add("Clan money", 100)
        lvRanking.Columns.Add("Created", 76)
        For tax = 0 To BellatraConfig.MaxTax
            cbTax.Items.Add(tax & "%")
        Next
        For table = 1 To BellatraConfig.LegacyTableCount
            cbLegacyTable.Items.Add("Code table " & table)
        Next
        cbLegacyTable.SelectedIndex = 0
        cbCloseDay.Items.AddRange(Globalization.CultureInfo.InvariantCulture.DateTimeFormat.DayNames)
        BuildRows()
        AddHandler ServerDatabase.StatusChanged, AddressOf ServerDatabase_StatusChanged
    End Sub

    ''' <summary>Chamado ao abrir a aba: lista de monstros, arquivo do servidor atual e, na primeira vez, o banco.</summary>
    Friend Sub EnsureLoaded()
        EnsureSetup()
        ShowDatabase()
        If _choicesFrom <> AppState.Monsters.Count Then FillChoices()
        If Not _dirty AndAlso Not BellatraConfig.FilePath().Equals(_loadedPath, StringComparison.OrdinalIgnoreCase) Then LoadFile()
        LoadSchedule()
        If Not _ownerChecked Then
            _ownerChecked = True
            RefreshOwner(False)
        End If
    End Sub

#Region "Banco"

    Private Sub ShowDatabase()
        Dim config = ServerDatabase.Config
        lblDbInfo.Text = If(config Is Nothing, "(not configured)", config.Description) & "   -   set and tested in Options"
        lblDbStatus.Text = ServerDatabase.StatusText
        lblDbStatus.ForeColor = ServerDatabase.StatusColor
    End Sub

    Private Sub ServerDatabase_StatusChanged()
        If IsHandleCreated Then ShowDatabase()
    End Sub

    Private Sub cmdOptions_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdOptions.Click
        RaiseEvent OptionsRequested()
    End Sub

    ''' <summary>Repositorio pronto, ou Nothing (com a mensagem mostrada se showErrors).</summary>
    Private Function Repository(ByVal showErrors As Boolean) As BellatraRepository
        Dim config = ServerDatabase.Config
        If config Is Nothing Then
            If showErrors Then MsgBox(ServerDatabase.LoadError & vbCrLf & vbCrLf & "Check the database in Options.", MsgBoxStyle.Exclamation, "Bellatra")
            Return Nothing
        End If
        Return New BellatraRepository(config)
    End Function

    ''' <summary>Executa uma operacao no banco mostrando o erro de forma legivel. False se falhou.</summary>
    Private Function RunDb(ByVal action As Action, ByVal what As String, ByVal showErrors As Boolean) As Boolean
        Cursor = Cursors.WaitCursor
        Try
            action()
            Return True
        Catch ex As Exception
            ServerDatabase.SetStatus(what & " failed: " & ex.Message, Color.Firebrick)
            ErrorLog.Write("Bellatra: " & what & vbCrLf & ex.ToString())
            If showErrors Then MsgBox(what & " failed:" & vbCrLf & ex.Message, MsgBoxStyle.Exclamation, "Bellatra")
            Return False
        Finally
            Cursor = Cursors.Default
        End Try
    End Function

    Private Sub cmdRefresh_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRefresh.Click
        RefreshOwner(True)
    End Sub

    ''' <summary>Le o ranking da semana e a gerencia (Sod2Config): gerente, periodo, renda e saudacao da Karina.</summary>
    Private Sub RefreshOwner(ByVal showErrors As Boolean)
        Dim repo = Repository(showErrors)
        If repo Is Nothing Then
            ShowManager(Nothing, "Database not configured. Set it in Options.")
            Return
        End If
        Dim ranking As List(Of BellatraClan) = Nothing
        Dim manager As BellatraManager = Nothing
        If Not RunDb(Sub()
                         ranking = repo.Ranking()
                         manager = repo.Manager()
                     End Sub, "Reading the Bellatra ranking", showErrors) Then
            ShowManager(Nothing, "Could not read the ranking (see the status above). SoD2DB needs the Sod2Config table (Database\SoD2DB.sql).")
            Return
        End If
        lvRanking.BeginUpdate()
        lvRanking.Items.Clear()
        For i = 0 To ranking.Count - 1
            Dim clan = ranking(i)
            Dim row = lvRanking.Items.Add((i + 1).ToString())
            row.SubItems.Add(clan.ClanName)
            row.SubItems.Add(clan.Points.ToString("N0"))
            row.SubItems.Add(clan.ClanMoney.ToString("N0"))
            row.SubItems.Add(clan.RegiDate)
        Next
        lvRanking.EndUpdate()
        ShowManager(manager, "")
        ServerDatabase.SetStatus("Connected: " & ranking.Count & " clan(s) in this week's ranking.", Color.DarkGreen)
    End Sub

    Private Shared Function FormatDate(ByVal value As DateTime) As String
        Return value.ToString("yyyy-MM-dd HH:mm")
    End Function

    Private Sub ShowManager(ByVal manager As BellatraManager, ByVal errorText As String)
        _manager = manager
        Dim hasManager As Boolean = manager IsNot Nothing AndAlso manager.Exists AndAlso manager.Idx > 0
        If manager Is Nothing Then
            lblOwnerInfo.Text = errorText
        ElseIf Not manager.Exists Then
            lblOwnerInfo.Text = "No management yet: the server creates it on the first SoD tick (Bellatra enabled), " &
                                "and the 1st of the weekly ranking becomes the manager."
        ElseIf Not hasManager Then
            lblOwnerInfo.Text = "No manager this week (no clan had points at the last close)." & vbCrLf &
                                "Period: " & FormatDate(manager.OperFrom) & " to " & FormatDate(manager.OperTo) & vbCrLf &
                                "The 1st of the weekly ranking takes over at the next close."
        Else
            lblOwnerInfo.Text = "Clan: " & manager.ClanName & "   (leader " & manager.Leader & ")" & vbCrLf &
                                "Period: " & FormatDate(manager.OperFrom) & " to " & FormatDate(manager.OperTo) & "   Tax: " & manager.TaxRates & "%" & vbCrLf &
                                "Tax today (paid at the daily close): " & manager.TodayMoney.ToString("N0") &
                                "   Yesterday: " & manager.BeforeMoney.ToString("N0") & "   Period: " & manager.TotalMoney.ToString("N0") & vbCrLf &
                                "Clan money (the leader withdraws it at Karina): " & manager.ClanMoney.ToString("N0")
        End If
        tbGreeting.Text = If(hasManager, manager.Note, "")
        tbGreeting.Enabled = hasManager
        cmdSaveGreeting.Enabled = hasManager
    End Sub

    Private Sub cmdSaveGreeting_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSaveGreeting.Click
        If _manager Is Nothing OrElse _manager.Idx <= 0 Then Return
        Dim note As String = tbGreeting.Text.Trim()
        Dim problem As String = BellatraRepository.ValidateNote(note)
        If problem <> "" Then
            MsgBox(problem, MsgBoxStyle.Exclamation, "Bellatra")
            Return
        End If
        If Not Dialogs.Confirm("Karina's greeting", "Save the greeting of clan " & _manager.ClanName & "?" & vbCrLf & vbCrLf & note) Then Return
        Dim repo = Repository(True)
        If repo Is Nothing Then Return
        Dim saved As Boolean
        If Not RunDb(Sub() saved = repo.SaveNote(_manager.Idx, note), "Saving Karina's greeting", True) Then Return
        If Not saved Then MsgBox("Clan " & _manager.ClanName & " is no longer the manager (the week closed).", MsgBoxStyle.Exclamation, "Bellatra")
        RefreshOwner(True)
    End Sub

    ''' <summary>Dia e hora do fechamento lidos do server.ini do servidor atual.</summary>
    Private Sub LoadSchedule()
        Dim day, hour As Integer
        Try
            BellatraConfig.ReadSchedule(day, hour)
        Catch ex As Exception
            ErrorLog.Write("Bellatra: reading server.ini" & vbCrLf & ex.ToString())
            day = BellatraConfig.DefaultCloseDay
            hour = BellatraConfig.DefaultCloseHour
        End Try
        cbCloseDay.SelectedIndex = day
        nudCloseHour.Value = hour
    End Sub

    Private Sub cmdSaveSchedule_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSaveSchedule.Click
        If AppState.ServerPath = "" OrElse Not File.Exists(Path.Combine(AppState.ServerPath, DatabaseConfig.FileName)) Then
            MsgBox("server.ini not found in the server folder. Set it in Options.", MsgBoxStyle.Exclamation, "Bellatra")
            Return
        End If
        Dim day As Integer = cbCloseDay.SelectedIndex
        Dim hour As Integer = CInt(nudCloseHour.Value)
        If Not Dialogs.Confirm("Bellatra", "Close the week every " & cbCloseDay.Text & " at " & hour & "h and the day every day at " & hour & "h?" & vbCrLf & vbCrLf &
                               "Saved in server.ini [Bellatra]. The server reads it within a minute. The current period still ends on its date; " &
                               "the next periods follow the new day.") Then Return
        Try
            BellatraConfig.WriteSchedule(day, hour)
        Catch ex As Exception
            ErrorLog.Report("BellatraPanel.SaveSchedule", ex)
            Return
        End Try
        LoadSchedule()
    End Sub

#End Region

#Region "Rounds"

    ''' <summary>Cria a grade dos 8 rounds: 4 x (monstro, %), maximo e chefe (so nos rounds 3, 6 e 8).</summary>
    Private Sub BuildRows()
        pnlRounds.SuspendLayout()
        AddHeader("Round", 0)
        For slot = 0 To BellatraConfig.SlotCount - 1
            AddHeader("Monster " & (slot + 1), SlotLeft(slot))
            AddHeader("%", SlotLeft(slot) + 143)
        Next
        AddHeader("Max alive", MaxLeft())
        AddHeader("Boss", BossLeft())
        For i = 0 To BellatraConfig.RoundCount - 1
            Dim round As Integer = i + 1
            Dim top As Integer = RowTop + i * RowHeight
            Dim row As New RoundRow()
            Dim label As New Label With {.Text = round.ToString(), .Location = New Point(8, top + 4), .AutoSize = True}
            pnlRounds.Controls.Add(label)
            For slot = 0 To BellatraConfig.SlotCount - 1
                row.Monsters(slot) = NewMonsterCombo(SlotLeft(slot), top, 140)
                row.Percents(slot) = NewNumber(SlotLeft(slot) + 143, top, 44, 0, 100)
            Next
            row.MaxMonster = NewNumber(MaxLeft(), top, 50, BellatraConfig.MinMaxMonster, BellatraConfig.MaxMaxMonster)
            If BellatraConfig.IsBossRound(round) Then
                row.Boss = NewMonsterCombo(BossLeft(), top, 144)
            Else
                pnlRounds.Controls.Add(New Label With {.Text = "-", .Location = New Point(BossLeft() + 4, top + 4), .AutoSize = True,
                                                       .ForeColor = SystemColors.GrayText})
            End If
            _rows.Add(row)
        Next
        pnlRounds.ResumeLayout()
    End Sub

    Private Shared Function SlotLeft(ByVal slot As Integer) As Integer
        Return 34 + slot * 192
    End Function

    Private Shared Function MaxLeft() As Integer
        Return SlotLeft(BellatraConfig.SlotCount)
    End Function

    Private Shared Function BossLeft() As Integer
        Return MaxLeft() + 58
    End Function

    Private Sub AddHeader(ByVal text As String, ByVal left As Integer)
        pnlRounds.Controls.Add(New Label With {.Text = text, .Location = New Point(left, 0), .AutoSize = True,
                                               .ForeColor = SystemColors.GrayText})
    End Sub

    Private Function NewMonsterCombo(ByVal left As Integer, ByVal top As Integer, ByVal width As Integer) As ComboBox
        Dim combo As New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList, .Location = New Point(left, top),
                                        .Width = width, .DropDownWidth = 300, .MaxDropDownItems = 20}
        AddHandler combo.SelectedIndexChanged, AddressOf Field_Changed
        pnlRounds.Controls.Add(combo)
        Return combo
    End Function

    Private Function NewNumber(ByVal left As Integer, ByVal top As Integer, ByVal width As Integer, ByVal min As Integer, ByVal max As Integer) As NumericUpDown
        Dim number As New NumericUpDown With {.Location = New Point(left, top + 1), .Width = width, .Minimum = min, .Maximum = max}
        AddHandler number.ValueChanged, AddressOf Field_Changed
        pnlRounds.Controls.Add(number)
        Return number
    End Function

    ''' <summary>Monta a lista de monstros (*이름 dos .inf carregados) e reaplica o que estava escolhido.</summary>
    Private Sub FillChoices()
        _choicesFrom = AppState.Monsters.Count
        _knownNames.Clear()
        Dim list As New List(Of MonsterChoice)()
        For Each monster In AppState.Monsters
            If monster.MapName = "" OrElse Not _knownNames.Add(monster.MapName) Then Continue For
            Dim korean As String = BellatraConfig.Readable(monster.MapName)
            Dim name As String = monster.Name.Trim(""""c)
            list.Add(New MonsterChoice(monster.MapName,
                                       If(name = "" OrElse name = MonsterData.NameNotFound, korean, name & "  [" & korean & "]")))
        Next
        _choices = list.OrderBy(Function(c) c.Display, StringComparer.CurrentCultureIgnoreCase).ToArray()
        Dim current As BellatraConfig = If(_rows.Count > 0, ReadForm(), _config)
        ShowConfig(current)
    End Sub

    ''' <summary>Seleciona o monstro no combo; nomes que nao existem entram marcados como "(not found)".</summary>
    Private Sub SelectMonster(ByVal combo As ComboBox, ByVal mapName As String)
        combo.BeginUpdate()
        combo.Items.Clear()
        combo.Items.Add(New MonsterChoice("", NoMonster))
        combo.Items.AddRange(_choices)
        Dim index As Integer = 0
        If mapName <> "" Then
            index = Array.FindIndex(_choices, Function(c) c.MapName.Equals(mapName, StringComparison.OrdinalIgnoreCase)) + 1
            If index = 0 Then index = combo.Items.Add(New MonsterChoice(mapName, "(not found) " & BellatraConfig.Readable(mapName)))
        End If
        combo.SelectedIndex = index
        combo.EndUpdate()
    End Sub

    Private Shared Function SelectedName(ByVal combo As ComboBox) As String
        Dim choice = TryCast(combo.SelectedItem, MonsterChoice)
        Return If(choice Is Nothing, "", choice.MapName)
    End Function

    Private Shared Sub SetNumber(ByVal number As NumericUpDown, ByVal value As Integer)
        number.Value = Math.Max(number.Minimum, Math.Min(number.Maximum, value))
    End Sub

    ''' <summary>Mostra a configuracao nos controles (sem marcar alteracao).</summary>
    Private Sub ShowConfig(ByVal config As BellatraConfig)
        _filling = True
        Try
            If config.Tax >= 0 AndAlso config.Tax <= BellatraConfig.MaxTax Then
                cbTax.SelectedIndex = config.Tax
            Else
                cbTax.SelectedIndex = -1
            End If
            For i = 0 To BellatraConfig.RoundCount - 1
                Dim r = config.Rounds(i)
                Dim row = _rows(i)
                For slot = 0 To BellatraConfig.SlotCount - 1
                    SelectMonster(row.Monsters(slot), r.Monsters(slot))
                    SetNumber(row.Percents(slot), If(r.Monsters(slot) = "", 0, r.Percents(slot)))
                Next
                SetNumber(row.MaxMonster, r.MaxMonster)
                If row.Boss IsNot Nothing Then SelectMonster(row.Boss, r.Boss)
            Next
        Finally
            _filling = False
        End Try
    End Sub

    ''' <summary>Configuracao a partir dos controles, com as linhas guardadas do arquivo lido.</summary>
    Private Function ReadForm() As BellatraConfig
        Dim config As New BellatraConfig With {.Tax = If(cbTax.SelectedIndex >= 0, cbTax.SelectedIndex, _config.Tax),
                                               .MonsterLines = _config.MonsterLines}
        config.KeptLines.AddRange(_config.KeptLines)
        For i = 0 To BellatraConfig.RoundCount - 1
            Dim r = config.Rounds(i)
            Dim row = _rows(i)
            For slot = 0 To BellatraConfig.SlotCount - 1
                r.Monsters(slot) = SelectedName(row.Monsters(slot))
                r.Percents(slot) = CInt(row.Percents(slot).Value)
            Next
            r.MaxMonster = CInt(row.MaxMonster.Value)
            r.Boss = If(row.Boss Is Nothing, "", SelectedName(row.Boss))
        Next
        Return config
    End Function

    Private Sub Field_Changed(ByVal sender As Object, ByVal e As EventArgs) Handles cbTax.SelectedIndexChanged
        If _filling Then Return
        SetDirty(True)
    End Sub

    Private Sub SetDirty(ByVal dirty As Boolean)
        _dirty = dirty
        cmdSave.Text = If(dirty, "Save *", "Save")
    End Sub

#End Region

#Region "Arquivo"

    ''' <summary>Le Bellatra\bellatra.ini do servidor atual; sem o arquivo, mostra a tabela 1 do codigo como ponto de partida.</summary>
    Private Sub LoadFile()
        Dim path As String = BellatraConfig.FilePath()
        _loadedPath = path
        Try
            If File.Exists(path) Then
                _config = BellatraConfig.Load(path)
            Else
                _config = New BellatraConfig()
                _config.ApplyLegacyTable(1)
            End If
        Catch ex As Exception
            ErrorLog.Report("BellatraPanel.LoadFile: " & path, ex)
            _config = New BellatraConfig()
        End Try
        ShowConfig(_config)
        SetDirty(False)
        ShowFileInfo()
    End Sub

    ''' <summary>Diz se o servidor vai usar o arquivo e o que ele faz sem ele.</summary>
    Private Sub ShowFileInfo()
        Dim path As String = BellatraConfig.FilePath()
        If AppState.ServerPath = "" Then
            lblFileInfo.Text = "Set the server folder in Options."
            lblFileInfo.ForeColor = Color.Firebrick
        ElseIf Not File.Exists(path) Then
            lblFileInfo.Text = path & " not found: the server uses its 3 code tables (random each SoD) and a 5% tax. " &
                               "The form shows code table 1; Save creates the file."
            lblFileInfo.ForeColor = Color.DarkOrange
        ElseIf _config.MonsterLines < BellatraConfig.RoundCount Then
            lblFileInfo.Text = path & " has " & _config.MonsterLines & " *BELATRA_MONSTER line(s); the server ignores the whole file " &
                               "(tax included) without 8. Save writes all 8."
            lblFileInfo.ForeColor = Color.Firebrick
        Else
            Dim problems = _config.Validate(_knownNames)
            lblFileInfo.Text = path & If(problems.Count = 0, "  -  in use by the server from the next SoD.",
                                         "  -  " & problems.Count & " problem(s): " & problems(0))
            lblFileInfo.ForeColor = If(problems.Count = 0, Color.DarkGreen, Color.Firebrick)
        End If
        cmdRemoveFile.Enabled = File.Exists(path)
    End Sub

    Private Function ConfirmDiscard() As Boolean
        Return Not _dirty OrElse Dialogs.Confirm("Bellatra", "Discard the unsaved changes?")
    End Function

    Private Sub cmdReloadFile_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdReloadFile.Click
        If Not ConfirmDiscard() Then Return
        If _choicesFrom <> AppState.Monsters.Count Then FillChoices()
        LoadFile()
    End Sub

    Private Sub cmdLoadLegacy_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdLoadLegacy.Click
        Dim table As Integer = cbLegacyTable.SelectedIndex + 1
        If Not Dialogs.Confirm("Bellatra", "Replace the monsters of all rounds with code table " & table & "?" & vbCrLf &
                               "The tax is kept. Nothing is saved until you click Save.") Then Return
        Dim config = ReadForm()
        config.ApplyLegacyTable(table)
        ShowConfig(config)
        SetDirty(True)
    End Sub

    Private Sub cmdSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSave.Click
        If AppState.ServerPath = "" Then
            MsgBox("Set the server folder in Options.", MsgBoxStyle.Exclamation, "Bellatra")
            Return
        End If
        If AppState.Monsters.Count = 0 Then
            MsgBox("No monsters loaded (GameServer\Monster). Reload the server files first: the names cannot be checked.",
                   MsgBoxStyle.Exclamation, "Bellatra")
            Return
        End If
        Dim config = ReadForm()
        Dim problems = config.Validate(_knownNames)
        If problems.Count > 0 Then
            MsgBox("Not saved:" & vbCrLf & vbCrLf & String.Join(vbCrLf, problems.Take(15)) &
                   If(problems.Count > 15, vbCrLf & "... and " & (problems.Count - 15) & " more.", ""),
                   MsgBoxStyle.Exclamation, "Bellatra")
            Return
        End If
        Dim path As String = BellatraConfig.FilePath()
        If Not Dialogs.Confirm("Bellatra", "Save " & path & "?" & vbCrLf & vbCrLf &
                               "Tax " & config.Tax & "%. The server uses it from the next SoD.") Then Return
        Try
            config.Save(path)
        Catch ex As Exception
            ErrorLog.Report("BellatraPanel.Save: " & path, ex)
            Return
        End Try
        _config = config
        _config.MonsterLines = BellatraConfig.RoundCount
        _loadedPath = path
        SetDirty(False)
        ShowFileInfo()
        ' Como o site: a taxa tambem fica em Sod2Config.TaxRates, com historico em Sod2TaxLog
        Dim repo = Repository(False)
        If repo IsNot Nothing AndAlso RunDb(Sub() repo.SaveTax(config.Tax), "Saving the tax in SoD2DB (the file was saved)", True) Then RefreshOwner(False)
    End Sub

    ''' <summary>Apaga o arquivo (com backup): o servidor volta as 3 tabelas do codigo, como no legado sem o site.</summary>
    Private Sub cmdRemoveFile_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRemoveFile.Click
        Dim path As String = BellatraConfig.FilePath()
        If Not File.Exists(path) Then Return
        If Not Dialogs.Confirm("Bellatra", "Remove " & path & "?" & vbCrLf & vbCrLf &
                               "A copy goes to " & BellatraConfig.Folder & "\" & AppState.BackupFolder & ". From the next SoD the server " &
                               "uses its 3 code tables again. The tax keeps the last value read until the server restarts (then 5%).",
                               MsgBoxStyle.Exclamation) Then Return
        Try
            BackupHelper.CreateBackup(BellatraConfig.FolderPath(), BellatraConfig.FileName)
            File.Delete(path)
        Catch ex As Exception
            ErrorLog.Report("BellatraPanel.Remove: " & path, ex)
            Return
        End Try
        LoadFile()
    End Sub

#End Region

End Class
