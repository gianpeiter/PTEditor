''' <summary>Avalia formulas simples ("EXP*2", "LVL*100+500") via DataTable.Compute.</summary>
Friend Module FormulaEval

    ''' <summary>Retorna False se a expressao for invalida.</summary>
    Friend Function TryEvaluate(ByVal expression As String, ByRef result As Double) As Boolean
        Try
            Using table As New DataTable()
                result = Convert.ToDouble(table.Compute(expression, Nothing))
            End Using
            Return True
        Catch
            result = 0
            Return False
        End Try
    End Function

    ''' <summary>Avalia e arredonda para inteiro (mesmo arredondamento de CInt). Erro vira 0 e vai para o log.</summary>
    Friend Function EvaluateInteger(ByVal expression As String) As Long
        Dim value As Double
        If Not TryEvaluate(expression, value) Then
            ErrorLog.Write("Invalid formula: " & expression)
            MsgBox("Error in formula: " & expression, MsgBoxStyle.Exclamation)
            Return 0
        End If
        Return CLng(Math.Round(value, MidpointRounding.ToEven))
    End Function

    ''' <summary>Substitui as variaveis (ex.: "EXP", "LVL") pelos valores, ignorando maiusculas/minusculas.</summary>
    Friend Function Substitute(ByVal formula As String, ByVal ParamArray vars() As KeyValuePair(Of String, Long)) As String
        Dim expression As String = formula.ToUpper()
        For Each v In vars
            expression = expression.Replace(v.Key.ToUpper(), v.Value.ToString())
        Next
        Return expression
    End Function

End Module
