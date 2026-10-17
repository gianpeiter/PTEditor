''' <summary>Gravidade de uma mensagem de validacao.</summary>
Friend Enum RuleSeverity
    Info
    Warning
    [Error]
End Enum

''' <summary>Quest de level: bit no dwLevelQuestLog, level minimo, recompensa e codigos usados enquanto esta em andamento.</summary>
Friend Class LevelQuest
    Public ReadOnly Property Bit As UInteger
    Public ReadOnly Property Level As Integer
    Public ReadOnly Property Name As String
    Public ReadOnly Property Reward As String
    ''' <summary>Bit da quest que o jogo exige antes (0 = nenhuma).</summary>
    Public ReadOnly Property Previous As UInteger
    Public ReadOnly Property QuestCodes As Integer()

    Public Sub New(ByVal bit As UInteger, ByVal level As Integer, ByVal name As String, ByVal reward As String,
                   ByVal previous As UInteger, ByVal questCodes As Integer())
        Me.Bit = bit
        Me.Level = level
        Me.Name = name
        Me.Reward = reward
        Me.Previous = previous
        Me.QuestCodes = questCodes
    End Sub
End Class

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
    Private Const QuestLevel55 As UInteger = 2
    Private Const QuestLevel70 As UInteger = 4
    Private Const QuestLevel80 As UInteger = 8
    Private Const QuestLevel85 As UInteger = 16
    Private Const QuestLevel90 As UInteger = 32
    Private Const QuestLevel80Second As UInteger = 64
    Private Const QuestLevel90Second As UInteger = 128

    ''' <summary>
    ''' Quests de level na ordem do jogo. Previous: bit que o jogo exige concluido antes (sinCheckLevelQuest*);
    ''' 80-2 e 90-2 nao dependem de outra quest.
    ''' </summary>
    Friend ReadOnly LevelQuests As LevelQuest() = {
        New LevelQuest(QuestLevel30, 30, "Lv 30", "+5 stat points", 0, {&H5}),
        New LevelQuest(QuestLevel55, 55, "Lv 55", "+1 skill point", QuestLevel30, {&H6, &H7}),
        New LevelQuest(QuestLevel70, 70, "Lv 70", "+5 stat points, +1 skill point", QuestLevel55, {&H8}),
        New LevelQuest(QuestLevel80, 80, "Lv 80", "+5 stat points, +2 skill points", QuestLevel70, {&H9}),
        New LevelQuest(QuestLevel85, 85, "Lv 85", "lower death penalty", QuestLevel80, {&HA}),
        New LevelQuest(QuestLevel90, 90, "Lv 90", "+40 HP", QuestLevel85, {&HB}),
        New LevelQuest(QuestLevel80Second, 80, "Lv 80-2", "+2 stat points per level above 79", 0, {&HC}),
        New LevelQuest(QuestLevel90Second, 90, "Lv 90-2", "+3 stat points per level above 89", 0, {&HD})}

    ''' <summary>Marca a quest e as anteriores da cadeia (como o jogador teria feito).</summary>
    Friend Function CompleteLevelQuest(ByVal questLog As UInteger, ByVal quest As LevelQuest) As UInteger
        Do While quest IsNot Nothing
            questLog = questLog Or quest.Bit
            quest = LevelQuests.FirstOrDefault(Function(q) q.Bit = quest.Previous)
        Loop
        Return questLog
    End Function

    ''' <summary>Desmarca a quest e as seguintes da cadeia.</summary>
    Friend Function UndoLevelQuest(ByVal questLog As UInteger, ByVal quest As LevelQuest) As UInteger
        Do While quest IsNot Nothing
            questLog = questLog And Not quest.Bit
            Dim bit As UInteger = quest.Bit
            quest = LevelQuests.FirstOrDefault(Function(q) q.Previous = bit)
        Loop
        Return questLog
    End Function

    ''' <summary>Nome das quests concluidas no log.</summary>
    Friend Function LevelQuestNames(ByVal questLog As UInteger) As String
        Return String.Join(", ", LevelQuests.Where(Function(q) (questLog And q.Bit) <> 0).Select(Function(q) q.Name))
    End Function

#Region "Rank de classe e skills"

    ''' <summary>Rank maximo do jogo normal (4 so existe por comando de debug).</summary>
    Friend Const MaxGameRank As Integer = 3

    ''' <summary>Level em que cada rank e alcancado (quests de troca de classe e promocao do AddExp).</summary>
    Friend Function RankLevel(ByVal rank As Integer) As Integer
        Return rank * 20
    End Function

    ''' <summary>Rank que o level permite.</summary>
    Friend Function MaxRankForLevel(ByVal level As Integer) As Integer
        Return Math.Max(0, Math.Min(MaxGameRank, level \ 20))
    End Function

    ''' <summary>Quantas skills o rank libera (ChangeJobSkillPlus - 1 em sinSkill.cpp).</summary>
    Friend Function SkillsForRank(ByVal rank As Integer) As Integer
        Return Math.Max(0, Math.Min(5, rank + 1)) * 4
    End Function

    Friend Function RankText(ByVal rank As Integer) As String
        Select Case rank
            Case 0 : Return "0 - Base class (skills 1-4)"
            Case 1 : Return "1 - 2nd class, Lv 20 (skills 1-8)"
            Case 2 : Return "2 - 3rd class, Lv 40 (skills 1-12)"
            Case 3 : Return "3 - 4th class, Lv 60 (skills 1-16)"
            Case 4 : Return "4 - 5th class (skills 1-20, not reachable in the game)"
            Case Else : Return rank & " - invalid"
        End Select
    End Function

    ''' <summary>Skills 1-12 usam os pontos normais; 13 em diante, os pontos de elite (record.cpp).</summary>
    Friend Const NormalSkillCount As Integer = 12

    ''' <summary>GetSkillPoint_LevelQuest: pontos de skill extras por quests de level.</summary>
    Friend Function QuestSkillPoints(ByVal level As Integer, ByVal questLog As UInteger) As Integer
        Dim points As Integer = 0
        If (questLog And QuestLevel55) <> 0 AndAlso level >= 55 Then points += 1
        If (questLog And QuestLevel70) <> 0 AndAlso level >= 70 Then points += 1
        If (questLog And QuestLevel80) <> 0 AndAlso level >= 80 Then points += 2
        Return points
    End Function

    ''' <summary>Pontos das skills 1-12 que o level permite: (level - 8) / 2 + quests, a partir do Lv 10.</summary>
    Friend Function NormalSkillPoints(ByVal level As Integer, ByVal questLog As UInteger) As Integer
        If level < 10 Then Return 0
        Return (level - 8) \ 2 + QuestSkillPoints(level, questLog)
    End Function

    ''' <summary>Pontos de elite (skills 13+): (level - 58) / 2, a partir do Lv 60.</summary>
    Friend Function EliteSkillPoints(ByVal level As Integer) As Integer
        If level < 60 Then Return 0
        Return (level - 58) \ 2
    End Function

    Friend Function UsedSkillPoints(ByVal c As CharacterFile, ByVal elite As Boolean) As Integer
        Dim used As Integer = 0
        For i = 0 To c.Profile.SkillCount - 1
            If (i >= NormalSkillCount) = elite Then used += c.SkillPoint(i)
        Next
        Return used
    End Function

#End Region

#Region "Quest em andamento"

    Private Const QuestChangeJob As Integer = &H1
    Private Const QuestChangeJob2M As Integer = &H2
    Private Const QuestChangeJob2D As Integer = &H3
    Private Const QuestChangeJob3 As Integer = &H4
    Private Const QuestChangeJob4 As Integer = &H10

    ''' <summary>Nome da quest (SIN_QUEST_CODE_* em sinQuest.h, HAQUEST_CODE_* em HaQuest.h).</summary>
    Friend Function QuestName(ByVal code As Integer) As String
        Select Case code
            Case QuestChangeJob : Return "Class change 1 (Tempskron)"
            Case QuestChangeJob2M, QuestChangeJob2D : Return "Class change 1 (Morion)"
            Case QuestChangeJob3 : Return "Class change 2"
            Case QuestChangeJob4 : Return "Class change 3"
            Case &H20 To &H26 : Return "Elementary quest " & ChrW(AscW("A"c) + code - &H20)
            Case &H28 : Return "Fury of Phantom"
        End Select
        Dim level = LevelQuests.FirstOrDefault(Function(q) q.QuestCodes.Contains(code))
        If level IsNot Nothing Then Return level.Name & " quest"
        Return "Quest 0x" & code.ToString("X")
    End Function

    ''' <summary>Rank que a quest de troca de classe entrega (0 = nao e quest de troca de classe).</summary>
    Friend Function QuestTargetRank(ByVal code As Integer) As Integer
        Select Case code
            Case QuestChangeJob, QuestChangeJob2M, QuestChangeJob2D : Return 1
            Case QuestChangeJob3 : Return 2
            Case QuestChangeJob4 : Return 3
            Case Else : Return 0
        End Select
    End Function

#End Region

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
        ElseIf c.ChangeJob > MaxRankForLevel(c.Level) Then
            result.Add(New RuleMessage(RuleSeverity.Warning,
                "Class rank " & c.ChangeJob & " is above the rank for level " & c.Level & " (" & MaxRankForLevel(c.Level) &
                "): the game only reaches it at level " & RankLevel(Math.Min(c.ChangeJob, MaxGameRank)) & "."))
        ElseIf c.ChangeJob < MaxRankForLevel(c.Level) Then
            result.Add(New RuleMessage(RuleSeverity.Info,
                "Class rank " & c.ChangeJob & " is below the rank for level " & c.Level & " (" & MaxRankForLevel(c.Level) &
                "): skills " & (SkillsForRank(c.ChangeJob) + 1) & "-" & SkillsForRank(MaxRankForLevel(c.Level)) &
                " stay locked at the Skill Master. Use 'Promote by level' in Skills & Quests."))
        End If

        ValidateSkillPoints(c, result)
        ValidateQuest(c, result)

        If c.Money < 0 Then
            result.Add(New RuleMessage(RuleSeverity.Error, "Negative gold."))
        End If
        Return result
    End Function

    ''' <summary>
    ''' RestoreSkill (cliente, ao carregar) zera as skills se os pontos usados passam do permitido, e CheckSkillPoint
    ''' (servidor, ao gravar) desconecta o jogador.
    ''' </summary>
    Private Sub ValidateSkillPoints(ByVal c As CharacterFile, ByVal result As List(Of RuleMessage))
        Dim normalUsed As Integer = UsedSkillPoints(c, False)
        Dim normalTotal As Integer = NormalSkillPoints(c.Level, c.LevelQuestLog)
        If normalUsed > normalTotal Then
            result.Add(New RuleMessage(RuleSeverity.Warning,
                "Skills 1-" & NormalSkillCount & " use " & normalUsed & " points but level " & c.Level & " allows " & normalTotal &
                ". The game will RESET skills 1-" & NormalSkillCount & " when loading."))
        End If
        If c.Profile.SkillCount > NormalSkillCount Then
            Dim eliteUsed As Integer = UsedSkillPoints(c, True)
            Dim eliteTotal As Integer = EliteSkillPoints(c.Level)
            If eliteUsed > eliteTotal Then
                result.Add(New RuleMessage(RuleSeverity.Warning,
                    "Skills " & (NormalSkillCount + 1) & "-" & c.Profile.SkillCount & " use " & eliteUsed & " elite points but level " &
                    c.Level & " allows " & eliteTotal & ". The game will RESET these skills when loading."))
            End If
        End If
    End Sub

    ''' <summary>A quest em andamento ocupa a unica vaga de quest e algumas travam o Skill Master (sinSkillMaster).</summary>
    Private Sub ValidateQuest(ByVal c As CharacterFile, ByVal result As List(Of RuleMessage))
        Dim code As Integer = c.QuestCode
        If code = 0 Then Return
        Dim name As String = QuestName(code)
        Dim target As Integer = QuestTargetRank(code)
        Dim level = LevelQuests.FirstOrDefault(Function(q) q.QuestCodes.Contains(code))
        If target > 0 AndAlso c.ChangeJob >= target Then
            result.Add(New RuleMessage(RuleSeverity.Warning,
                "Quest in progress '" & name & "' is for rank " & target & ", already reached. Clear it: it keeps the Skill Master " &
                "in the quest dialog and finishing it would raise the rank again."))
        ElseIf code = QuestChangeJob4 OrElse code = QuestChangeJob3 Then
            result.Add(New RuleMessage(RuleSeverity.Info,
                "Quest in progress '" & name & "': while it runs, the Skill Master may show the quest instead of the skill purchase."))
        ElseIf level IsNot Nothing AndAlso (c.LevelQuestLog And level.Bit) <> 0 Then
            result.Add(New RuleMessage(RuleSeverity.Warning,
                "Quest in progress '" & name & "' is already marked as completed. Clear it so it does not block other quests."))
        Else
            result.Add(New RuleMessage(RuleSeverity.Info,
                "Quest in progress '" & name & "': the game allows one quest at a time, so it must be finished or cleared before another starts."))
        End If
    End Sub

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
        AddChange(changes, "Class rank", original.ChangeJob, edited.ChangeJob)
        Dim added As UInteger = edited.LevelQuestLog And Not original.LevelQuestLog
        Dim undone As UInteger = original.LevelQuestLog And Not edited.LevelQuestLog
        If added <> 0 Then changes.Add("Level quests completed: " & LevelQuestNames(added))
        If undone <> 0 Then changes.Add("Level quests undone: " & LevelQuestNames(undone))
        If original.QuestCode <> 0 AndAlso edited.QuestCode = 0 Then
            changes.Add("Quest in progress cleared: " & QuestName(original.QuestCode))
        End If
        Return changes
    End Function

    Private Sub AddChange(ByVal changes As List(Of String), ByVal label As String, ByVal before As Integer, ByVal after As Integer)
        If before <> after Then changes.Add(label & ": " & before.ToString("N0") & " -> " & after.ToString("N0"))
    End Sub

End Module
