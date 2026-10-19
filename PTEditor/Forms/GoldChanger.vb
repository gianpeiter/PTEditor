''' <summary>Aplica uma formula de ouro ("GOLD*2", "LVL*50") as linhas de drop com ouro dos monstros selecionados.</summary>
Public Class GoldChanger

    Private ReadOnly _monsters As List(Of MonsterFile)
    Private ReadOnly _maxGold As Double

    Friend Sub New(ByVal monsters As IEnumerable(Of MonsterFile), ByVal maxGold As Double)
        InitializeComponent()
        _monsters = New List(Of MonsterFile)(monsters)
        _maxGold = maxGold
    End Sub

    Private Sub cmdClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdClose.Click
        Close()
    End Sub

    Private Sub cmdStart_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdStart.Click
        Dim formula As String = tbGoldFormula.Text.ToUpper()
        Dim probe As Double
        If Not FormulaEval.TryEvaluate(FormulaEval.Substitute(formula, Var("GOLD", 1), Var("LVL", 1)), probe) Then
            MsgBox("Error in formula: " & tbGoldFormula.Text, MsgBoxStyle.Exclamation)
            Return
        End If
        If _monsters.Count > 0 AndAlso Not formula.Contains("GOLD") AndAlso Not formula.Contains("LVL") Then
            If Not Dialogs.Confirm("Warning!", "Set all selected monsters to the same Gold?", MsgBoxStyle.Exclamation) Then
                MsgBox("Canceled")
                Return
            End If
        End If

        Dim warnings As Integer = 0
        For Each monster In _monsters
            ' Formato no arquivo: "...돈 <min> <max>" ou "...돈 <valor>".
            Dim tokens = MonsterData.GetGold(monster).Split({" "c, ChrW(9)}, StringSplitOptions.RemoveEmptyEntries)
            Dim minGold As Long = 0, maxGold As Long = 0
            Dim singleValue As Boolean = tokens.Length = 1
            If tokens.Length >= 2 Then
                Long.TryParse(tokens(0), minGold)
                Long.TryParse(tokens(1), maxGold)
            ElseIf tokens.Length = 1 Then
                Long.TryParse(tokens(0), maxGold)
            End If
            Dim level As Long = MonsterData.GetLevel(monster)

            Dim newMax As Long = FormulaEval.EvaluateInteger(FormulaEval.Substitute(formula, Var("GOLD", maxGold), Var("LVL", level)))
            If newMax > _maxGold Then
                warnings += 1
                lblWarn.Text = "Warnings: " & warnings
                lblWarn.Refresh()
            End If
            If singleValue Then
                MonsterData.SetGold(monster, newMax.ToString())
            Else
                Dim newMin As Long = FormulaEval.EvaluateInteger(FormulaEval.Substitute(formula, Var("GOLD", minGold), Var("LVL", level)))
                MonsterData.SetGold(monster, newMin & vbTab & newMax)
            End If
        Next
        If warnings > 0 Then MsgBox("Warning! Some monsters have too high Gold, please check Options!", MsgBoxStyle.Exclamation)

        If Dialogs.Confirm("Save", "Save all monsters?") Then
            Dim done As Integer = 0
            For Each monster In _monsters
                lblSave.Text = monster.FileName
                lblSave.Refresh()
                MonsterData.Save(monster, True)
                done += 1
                pbProgress.Value = CInt(Math.Min(100, Math.Round(done * 100 / _monsters.Count)))
                pbProgress.Refresh()
            Next
            lblSave.Text = ""
            MsgBox(_monsters.Count & " monster(s) saved!")
        Else
            MsgBox("Warning: no monster saved! Press the Reload button")
        End If
        Close()
    End Sub

    Private Function Var(ByVal name As String, ByVal value As Long) As KeyValuePair(Of String, Long)
        Return New KeyValuePair(Of String, Long)(name, value)
    End Function

End Class
