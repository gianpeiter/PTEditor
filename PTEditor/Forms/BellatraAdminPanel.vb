Imports System.IO

''' <summary>
''' Aba Bellatra Admin: a parte do site do Bellatra do legado que rodava sozinha (o site se perdeu), agora feita pelo
''' administrador. Entrega a renda do clan gerente ao CL.ClanMoney (Sod2DeliverRevenue), fecha o periodo e troca o gerente
''' pelo 1o do ranking (Sod2ClosePeriod), mostra o ranking (CL.Cpoint) e o historico (Sod2ConfigLog) e edita o minimo de
''' membros do clan na regra de pontos e a porcentagem do recolhido que vai ao clan gerente (Sod2Setting). As
''' procedures estao em Database\SoD2DB.sql do Servidor.
''' </summary>
Public Class BellatraAdminPanel

    ''' <summary>Pede para abrir a aba Options (onde o banco e configurado).</summary>
    Friend Event OptionsRequested()

    Private _setupDone As Boolean
    Private _loaded As Boolean
    Private _manager As BellatraManager
    ''' <summary>Parte liberada ao gerente do recolhido ainda nao entregue (-1 = nao lida).</summary>
    Private _pending As Long
    ''' <summary>Dinheiro recolhido (taxa dos premios) desde a ultima entrega.</summary>
    Private _collected As Long

    Private Sub BellatraAdminPanel_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        EnsureSetup()
    End Sub

    Private Sub EnsureSetup()
        If _setupDone Then Return
        _setupDone = True
        lvRanking.Columns.Add("#", 30)
        lvRanking.Columns.Add("Clan", 170)
        lvRanking.Columns.Add("Points", 90)
        lvRanking.Columns.Add("Clan money", 120)
        lvRanking.Columns.Add("Created", 100)
        lvHistory.Columns.Add("#", 40)
        lvHistory.Columns.Add("Clan", 170)
        lvHistory.Columns.Add("Leader", 140)
        lvHistory.Columns.Add("From", 130)
        lvHistory.Columns.Add("To", 130)
        lvHistory.Columns.Add("Tax", 50)
        lvHistory.Columns.Add("Delivered", 120)
        dtpNewEnd.Value = DefaultNewEnd()
        AddHandler ServerDatabase.StatusChanged, AddressOf ServerDatabase_StatusChanged
    End Sub

    ''' <summary>Uma semana a partir de hoje (so a data).</summary>
    Private Shared Function DefaultNewEnd() As DateTime
        Return DateTime.Today.AddDays(7)
    End Function

    ''' <summary>Chamado ao abrir a aba: mostra o banco e, na primeira vez, le tudo.</summary>
    Friend Sub EnsureLoaded()
        EnsureSetup()
        ShowDatabase()
        If Not _loaded Then
            _loaded = True
            RefreshAll(False)
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
            If showErrors Then MsgBox(ServerDatabase.LoadError & vbCrLf & vbCrLf & "Check the database in Options.", MsgBoxStyle.Exclamation, "Bellatra Admin")
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
            ErrorLog.Write("Bellatra Admin: " & what & vbCrLf & ex.ToString())
            If showErrors Then
                MsgBox(what & " failed:" & vbCrLf & ex.Message & vbCrLf & vbCrLf &
                       "The procedures and tables come from Database\SoD2DB.sql (Servidor repository); run it on the SQL Server if they are missing.",
                       MsgBoxStyle.Exclamation, "Bellatra Admin")
            End If
            Return False
        Finally
            Cursor = Cursors.Default
        End Try
    End Function

#End Region

#Region "Leitura"

    Private Sub cmdRefresh_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdRefresh.Click
        RefreshAll(True)
    End Sub

    Private Shared Function FormatDate(ByVal value As DateTime) As String
        Return value.ToString("yyyy-MM-dd HH:mm")
    End Function

    ''' <summary>Le gerencia, renda pendente, ranking, regra de pontos e historico.</summary>
    Private Sub RefreshAll(ByVal showErrors As Boolean)
        Dim repo = Repository(showErrors)
        If repo Is Nothing Then
            ShowManager(Nothing, "Database not configured. Set it in Options.")
            Return
        End If
        Dim ranking As List(Of BellatraClan) = Nothing
        Dim manager As BellatraManager = Nothing
        Dim history As List(Of BellatraPeriod) = Nothing
        Dim minSurvivors As Integer = BellatraRepository.DefaultMinSurvivors
        Dim release As Integer = BellatraRepository.DefaultReleasePercent
        If Not RunDb(Sub()
                         ranking = repo.Ranking()
                         manager = repo.Manager()
                         history = repo.History()
                         minSurvivors = repo.MinSurvivors()
                         release = repo.ReleasePercent()
                     End Sub, "Reading the Bellatra data", showErrors) Then
            ShowManager(Nothing, "Could not read the data (see the status above). SoD2DB needs Sod2Config, Sod2ConfigLog and Sod2Setting (Database\SoD2DB.sql).")
            Return
        End If
        _pending = -1
        _collected = 0
        If manager.Exists Then
            Dim collected, pending As Long
            If RunDb(Sub() repo.PendingRevenue(collected, pending), "Reading the pending revenue", False) Then
                _collected = collected
                _pending = pending
            End If
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

        lvHistory.BeginUpdate()
        lvHistory.Items.Clear()
        For Each period In history
            Dim row = lvHistory.Items.Add(period.Idx.ToString())
            row.SubItems.Add(period.ClanName)
            row.SubItems.Add(period.Leader)
            row.SubItems.Add(FormatDate(period.OperFrom))
            row.SubItems.Add(FormatDate(period.OperTo))
            row.SubItems.Add(period.TaxRates & "%")
            row.SubItems.Add(period.TotalMoney.ToString("N0"))
        Next
        lvHistory.EndUpdate()

        nudMin.Value = Math.Max(nudMin.Minimum, Math.Min(nudMin.Maximum, minSurvivors))
        nudRelease.Value = Math.Max(nudRelease.Minimum, Math.Min(nudRelease.Maximum, release))
        ShowManager(manager, "")
        ServerDatabase.SetStatus("Connected: " & ranking.Count & " clan(s) in the ranking, " & history.Count & " previous period(s).", Color.DarkGreen)
    End Sub

    Private Sub ShowManager(ByVal manager As BellatraManager, ByVal errorText As String)
        _manager = manager
        Dim hasManager As Boolean = manager IsNot Nothing AndAlso manager.Exists AndAlso manager.Idx > 0
        If manager Is Nothing Then
            lblManagerInfo.Text = errorText
        ElseIf Not manager.Exists Then
            lblManagerInfo.Text = "No management yet. Close period creates it and makes the 1st of the ranking the manager."
        Else
            Dim pendingText As String = If(_pending < 0, "(could not be read - run Database\SoD2DB.sql)", _pending.ToString("N0"))
            lblManagerInfo.Text = If(hasManager,
                                     "Clan: " & manager.ClanName & "   (leader " & manager.Leader & ")" & vbCrLf,
                                     "No manager in this period (no clan had points at the last close)." & vbCrLf) &
                                  "Period: " & FormatDate(manager.OperFrom) & " to " & FormatDate(manager.OperTo) & "   Tax: " & manager.TaxRates & "%" & vbCrLf &
                                  "Collected since the last delivery: " & _collected.ToString("N0") & "   To release to the clan: " & pendingText &
                                  "   Last delivery: " & manager.BeforeMoney.ToString("N0") &
                                  "   Delivered in the period: " & manager.TotalMoney.ToString("N0") & vbCrLf &
                                  If(hasManager, "Clan money (the leader withdraws it at Karina): " & manager.ClanMoney.ToString("N0"), "")
        End If
        cmdDeliver.Enabled = hasManager AndAlso _pending > 0
        cmdClose.Enabled = manager IsNot Nothing
    End Sub

#End Region

#Region "Acoes"

    Private Sub cmdDeliver_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdDeliver.Click
        If _manager Is Nothing OrElse _manager.Idx <= 0 OrElse _pending <= 0 Then Return
        If Not Dialogs.Confirm("Deliver revenue", "Deliver " & _pending.ToString("N0") & " gold to clan " & _manager.ClanName & "?" & vbCrLf & vbCrLf &
                               "It is added to the clan money (the leader withdraws it at Karina) and logged in ClanMoneyLog. " &
                               "The value is recalculated by the database now, so it can be slightly higher than shown.") Then Return
        Dim repo = Repository(True)
        If repo Is Nothing Then Return
        Dim delivered As Long
        If Not RunDb(Sub() delivered = repo.DeliverRevenue(), "Delivering the revenue", True) Then Return
        MsgBox("Delivered " & delivered.ToString("N0") & " gold to clan " & _manager.ClanName & ".", MsgBoxStyle.Information, "Bellatra Admin")
        RefreshAll(True)
    End Sub

    ''' <summary>Taxa do bellatra.ini do servidor (ou a padrao): so vale se ainda nao existe a linha em Sod2Config.</summary>
    Private Shared Function CurrentTax() As Integer
        Try
            If AppState.ServerPath <> "" AndAlso File.Exists(BellatraConfig.FilePath()) Then Return BellatraConfig.Load(BellatraConfig.FilePath()).Tax
        Catch ex As Exception
            ErrorLog.Write("Bellatra Admin: reading bellatra.ini" & vbCrLf & ex.ToString())
        End Try
        Return BellatraConfig.DefaultTax
    End Function

    Private Sub cmdClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdClose.Click
        If _manager Is Nothing Then Return
        Dim newEnd As DateTime = dtpNewEnd.Value.Date
        If newEnd <= DateTime.Now Then
            MsgBox("The new period must end in the future.", MsgBoxStyle.Exclamation, "Bellatra Admin")
            Return
        End If
        Dim outgoing As String = If(_manager.Exists AndAlso _manager.Idx > 0, "clan " & _manager.ClanName, "nobody")
        Dim pendingText As String = If(_pending > 0, "The revenue to release (" & _pending.ToString("N0") & " gold) goes to " & outgoing & ". ", "")
        If Not Dialogs.Confirm("Close period", "Close the period now?" & vbCrLf & vbCrLf &
                               pendingText & "The 1st of the ranking becomes the manager until " & newEnd.ToString("yyyy-MM-dd") & ", the ranking points of all " &
                               "clans are reset." & vbCrLf & vbCrLf & "This cannot be undone.",
                               MsgBoxStyle.Exclamation) Then Return
        Dim repo = Repository(True)
        If repo Is Nothing Then Return
        Dim newIdx As Integer
        Dim tax As Integer = CurrentTax()
        If Not RunDb(Sub() newIdx = repo.ClosePeriod(newEnd, tax), "Closing the period", True) Then Return
        RefreshAll(True)
        MsgBox(If(newIdx > 0, "Period closed. New manager: " & If(_manager IsNot Nothing, _manager.ClanName, "") & ".",
                  "Period closed. No clan had points, so there is no manager."), MsgBoxStyle.Information, "Bellatra Admin")
    End Sub

    Private Sub cmdSaveRelease_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSaveRelease.Click
        Dim value As Integer = CInt(nudRelease.Value)
        If Not Dialogs.Confirm("Release share", value & "% of the collected tax will go to the manager clan at each delivery." & vbCrLf & vbCrLf &
                               "It applies to the next Deliver revenue or Close period (also to what is already collected).") Then Return
        Dim repo = Repository(True)
        If repo Is Nothing Then Return
        If RunDb(Sub() repo.SaveReleasePercent(value), "Saving the release share", True) Then
            ServerDatabase.SetStatus("Release share saved: " & value & "% of the collected tax.", Color.DarkGreen)
            RefreshAll(False)
        End If
    End Sub

    Private Sub cmdSaveMin_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSaveMin.Click
        Dim value As Integer = CInt(nudMin.Value)
        If Not Dialogs.Confirm("Clan points rule", "Clan points count only when at least " & value & " members of the clan finish the same SoD." & vbCrLf & vbCrLf &
                               "Applies from the next SoD.") Then Return
        Dim repo = Repository(True)
        If repo Is Nothing Then Return
        If RunDb(Sub() repo.SaveMinSurvivors(value), "Saving the rule", True) Then
            ServerDatabase.SetStatus("Rule saved: at least " & value & " clan member(s) must finish the same SoD.", Color.DarkGreen)
        End If
    End Sub

#End Region

End Class
