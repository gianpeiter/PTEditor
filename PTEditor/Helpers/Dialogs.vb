Friend Module Dialogs

    ''' <summary>Pergunta Sim/Nao com "Nao" como botao padrao. Retorna True se o usuario confirmou.</summary>
    Friend Function Confirm(ByVal title As String, ByVal text As String,
                            Optional ByVal icon As MsgBoxStyle = MsgBoxStyle.Question) As Boolean
        Return MsgBox(text, MsgBoxStyle.YesNo Or MsgBoxStyle.DefaultButton2 Or icon, title) = MsgBoxResult.Yes
    End Function

    ''' <summary>Pede um numero inteiro ao usuario. Retorna False se cancelou ou digitou algo invalido.</summary>
    Friend Function AskLong(ByVal prompt As String, ByVal title As String, ByVal defaultValue As String,
                            ByRef value As Long) As Boolean
        Dim text As String = InputBox(prompt, title, defaultValue)
        Return Long.TryParse(text.Trim(), value)
    End Function

End Module
