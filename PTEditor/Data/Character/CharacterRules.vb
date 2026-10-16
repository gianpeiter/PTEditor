''' <summary>Gravidade de uma mensagem de validacao.</summary>
Friend Enum RuleSeverity
    Info
    Warning
    [Error]
End Enum

Friend Class RuleMessage
    Public ReadOnly Property Severity As RuleSeverity
    Public ReadOnly Property Text As String

    Public Sub New(ByVal severity As RuleSeverity, ByVal text As String)
        Me.Severity = severity
        Me.Text = text
    End Sub
End Class

''' <summary>
''' O que o servidor faz com o personagem ao carregar (SendRecordDataToClient em Servidor\record.cpp), para avisar
''' antes de gravar em vez de o jogador descobrir depois.
''' </summary>
Friend Module CharacterRules

    ''' <summary>Bits de GameSaveInfo.dwLevelQuestLog (sinQuest.h).</summary>
    Private Const QuestLevel30 As UInteger = 1
    Private Const QuestLevel70 As UInteger = 4
    Private Const QuestLevel80 As UInteger = 8
    Private Const QuestLevel80Second As UInteger = 64
    Private Const QuestLevel90Second As UInteger = 128

    ''' <summary>GetStatePoint_LevelQuest: pontos extras por quests de level.</summary>
    Friend Function QuestBonusPoints(ByVal level As Integer, ByVal questLog As UInteger) As Integer
        Dim points As Integer = 0
        If (questLog And QuestLevel30) <> 0 AndAlso level >= 30 Then points += 5
        If (questLog And QuestLevel70) <> 0 AndAlso level >= 70 Then points += 5
        If (questLog And QuestLevel80) <> 0 AndAlso level >= 80 Then points += 5
        If (questLog And QuestLevel80Second) <> 0 AndAlso level >= 80 Then points += (level - 79) * 2
        If (questLog And QuestLevel90Second) <> 0 AndAlso level >= 90 Then points += (level - 89) * 3
        Return points
    End Function

    ''' <summary>Total de pontos (atributos + livres) que o servidor permite: 99 + (level - 1) * 5 + quests.</summary>
    Friend Function TotalStatPoints(ByVal level As Integer, ByVal questLog As UInteger) As Integer
        Return 99 + (level - 1) * 5 + QuestBonusPoints(level, questLog)
    End Function

    ''' <summary>Pontos livres que o servidor vai calcular (ReformCharStatePoint). Abaixo de -10 ele reseta os atributos.</summary>
    Friend Function FreeStatPoints(ByVal level As Integer, ByVal questLog As UInteger, ByVal strength As Integer,
                                   ByVal spirit As Integer, ByVal talent As Integer, ByVal dexterity As Integer,
                                   ByVal health As Integer) As Integer
        Return TotalStatPoints(level, questLog) -
               (Math.Abs(strength) + Math.Abs(spirit) + Math.Abs(talent) + Math.Abs(dexterity) + Math.Abs(health))
    End Function

    Friend Const StatResetLimit As Integer = -10

    ''' <summary>Confere o personagem como o servidor fara. Erros impedem a gravacao; avisos pedem confirmacao.</summary>
    Friend Function Validate(ByVal c As CharacterFile) As List(Of RuleMessage)
        Dim result As New List(Of RuleMessage)()
        Dim table As LevelTable = c.Profile.LevelTable
        If c.Level < 1 OrElse c.Level > table.MaxLevel Then
            result.Add(New RuleMessage(RuleSeverity.Error, "Level " & c.Level & " out of range 1.." & table.MaxLevel & "."))
        End If
        If c.Exp < 0 Then
            result.Add(New RuleMessage(RuleSeverity.Error, "Negative EXP: the server resets EXP to 0."))
        ElseIf c.Level >= 1 AndAlso c.Level <= table.MaxLevel AndAlso Not table.IsValid(c.Level, c.Exp) Then
            Dim range As String = "EXP " & c.Exp.ToString("N0") & " does not belong to level " & c.Level & " (" &
                table.ExpAtLevelStart(c.Level).ToString("N0") & " to " & table.ExpAtLevelEnd(c.Level).ToString("N0") & ")"
            If c.Profile.FixLevelFromExp Then
                result.Add(New RuleMessage(RuleSeverity.Warning,
                    range & ". The server will change the level to " & table.LevelFromExp(c.Exp) & "."))
            Else
                result.Add(New RuleMessage(RuleSeverity.Info,
                    range & ". This server (" & c.Profile.Name & ") does not correct the level from the EXP."))
            End If
        End If

        Dim free As Integer = FreeStatPoints(c.Level, c.LevelQuestLog, c.Strength, c.Spirit, c.Talent, c.Dexterity, c.Health)
        If free <= StatResetLimit Then
            result.Add(New RuleMessage(RuleSeverity.Warning,
                "Stats are " & -free & " points above the allowed total for level " & c.Level &
                ". The server will RESET the stats."))
        ElseIf free < 0 Then
            result.Add(New RuleMessage(RuleSeverity.Info,
                "Stats are " & -free & " point(s) above the allowed total; the server tolerates up to 9 and sets free points to 0."))
        End If

        If c.Level < 20 AndAlso c.ChangeJob > 0 Then
            result.Add(New RuleMessage(RuleSeverity.Info, "Level below 20 with a class change: the server resets the class change to 0."))
        End If
        If c.Money < 0 Then
            result.Add(New RuleMessage(RuleSeverity.Error, "Negative gold."))
        End If
        Return result
    End Function

    ''' <summary>Diferencas legiveis entre o arquivo original e o editado.</summary>
    Friend Function Describe(ByVal original As CharacterFile, ByVal edited As CharacterFile) As List(Of String)
        Dim changes As New List(Of String)()
        AddChange(changes, "Level", original.Level, edited.Level)
        If original.Exp <> edited.Exp Then changes.Add("EXP: " & original.Exp.ToString("N0") & " -> " & edited.Exp.ToString("N0"))
        AddChange(changes, "Strength", original.Strength, edited.Strength)
        AddChange(changes, "Spirit", original.Spirit, edited.Spirit)
        AddChange(changes, "Talent", original.Talent, edited.Talent)
        AddChange(changes, "Agility", original.Dexterity, edited.Dexterity)
        AddChange(changes, "Health", original.Health, edited.Health)
        AddChange(changes, "Free points", original.StatePoint, edited.StatePoint)
        If original.EffectiveMoney <> edited.EffectiveMoney OrElse original.LastMoney <> edited.LastMoney Then
            changes.Add("Gold: " & original.EffectiveMoney.ToString("N0") & " -> " & edited.EffectiveMoney.ToString("N0"))
        End If
        Dim removed = original.Items.Count - edited.Items.Count
        If removed > 0 Then changes.Add("Items removed: " & removed)
        Return changes
    End Function

    Private Sub AddChange(ByVal changes As List(Of String), ByVal label As String, ByVal before As Integer, ByVal after As Integer)
        If before <> after Then changes.Add(label & ": " & before.ToString("N0") & " -> " & after.ToString("N0"))
    End Sub

End Module
