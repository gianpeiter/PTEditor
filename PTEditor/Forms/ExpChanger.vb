''' <summary>Aplica uma formula de EXP ("EXP*2", "LVL*100") a varios monstros de uma vez.</summary>
Public Class ExpChanger

    Private ReadOnly _monsters As List(Of MonsterFile)

    Friend Sub New(ByVal monsters As IEnumerable(Of MonsterFile))
        InitializeComponent()
        _monsters = New List(Of MonsterFile)(monsters)
    End Sub

    Private Sub ExpChanger_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        tbExpFormula_TextChanged(sender, e)
    End Sub

    Private Sub cmdClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdClose.Click
        Close()
    End Sub

    ''' <summary>Preview da formula com EXP=1500 e LVL=60.</summary>
    Private Sub tbExpFormula_TextChanged(ByVal sender As Object, ByVal e As EventArgs) Handles tbExpFormula.TextChanged
        Dim value As Double
        Dim expression As String = FormulaEval.Substitute(tbExpFormula.Text, Var("EXP", 1500), Var("LVL", 60))
        If FormulaEval.TryEvaluate(expression, value) AndAlso value <> 0 Then
            lblResu.Text = value.ToString()
        Else
            lblResu.Text = "0 or error in formula"
        End If
    End Sub

    Private Sub cmdStart_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdStart.Click
        Dim formula As String = tbExpFormula.Text.ToUpper()
        Dim probe As Double
        If Not FormulaEval.TryEvaluate(FormulaEval.Substitute(formula, Var("EXP", 1), Var("LVL", 1)), probe) Then
            MsgBox("Error in formula: " & tbExpFormula.Text, MsgBoxStyle.Exclamation)
            Return
        End If
        If _monsters.Count > 0 AndAlso Not formula.Contains("EXP") AndAlso Not formula.Contains("LVL") Then
            If Not Dialogs.Confirm("Warning!", "Set all selected monsters to same exp?", MsgBoxStyle.Exclamation) Then
                MsgBox("canceled")
                Return
            End If
        End If

        Dim maxExp As Double
        Double.TryParse(My.Settings.MaxExp, maxExp)
        Dim warnings As Integer = 0
        For Each monster In _monsters
            Dim expression As String = FormulaEval.Substitute(formula, Var("EXP", MonsterData.GetExp(monster)), Var("LVL", MonsterData.GetLevel(monster)))
            Dim exp As Long = FormulaEval.EvaluateInteger(expression)
            If exp > maxExp Then
                warnings += 1
                lblWarn.Text = "Warnings: " & warnings
                lblWarn.Refresh()
            End If
            MonsterData.SetExp(monster, exp)
        Next
        If warnings > 0 Then MsgBox("Warning! Some monsters have to high EXP, please check options!", MsgBoxStyle.Exclamation)

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
            MsgBox(_monsters.Count & " monster Saved!")
        Else
            MsgBox("Warning: No monster saved! Press reload button")
        End If
        Close()
    End Sub

    Private Function Var(ByVal name As String, ByVal value As Long) As KeyValuePair(Of String, Long)
        Return New KeyValuePair(Of String, Long)(name, value)
    End Function

End Class
