Imports System.IO
Imports System.Text

''' <summary>Um round de Bellatra: ate 4 monstros com a % de cada, o chefe e o maximo de monstros vivos por sala.</summary>
Friend Class BellatraRound
    ''' <summary>Nome em execucao dos monstros (forma CP1252 de AppState.FileEncoding); "" = posicao vazia.</summary>
    Public ReadOnly Property Monsters As String() = {"", "", "", ""}
    Public ReadOnly Property Percents As Integer() = {0, 0, 0, 0}
    Public Property Boss As String = ""
    Public Property MaxMonster As Integer = 40
End Class

''' <summary>
''' Bellatra\bellatra.ini, o arquivo que o site de gerencia do Bellatra gravava. O servidor o rele no inicio de cada SoD
''' (rsOpen_SODConfig -> Open_SODConfig em Servidor\fileread.cpp) e a mudanca vale no SoD seguinte, sem reiniciar.
''' Sem o arquivo, ou sem as 8 linhas *BELATRA_MONSTER, o arquivo inteiro e ignorado (inclusive a taxa) e cada SoD
''' sorteia uma das 3 tabelas do codigo (sHCoreEvent1/2/3).
''' Os nomes sao os nomes dos monstros EM EXECUCAO: no build Brasil o ConvertMonsterNames (abertura do servidor) troca o
''' *이름 do .inf pelo *B_NAME do .zhoon, e o arquivo e lido depois disso (lstrcmpi, o primeiro na ordem de carga).
''' </summary>
Friend NotInheritable Class BellatraConfig

    Friend Const Folder As String = "Bellatra"
    Friend Const FileName As String = "bellatra.ini"
    ''' <summary>rsHARDCORE_COUNT_END: o servidor exige 8 linhas *BELATRA_MONSTER.</summary>
    Friend Const RoundCount As Integer = 8
    Friend Const SlotCount As Integer = 4
    ''' <summary>O site deixava a taxa entre 0 e 10%; o servidor aceita 0 a 99.</summary>
    Friend Const MaxTax As Integer = 10
    ''' <summary>rsSOD_Tax ate o servidor ler um arquivo valido.</summary>
    Friend Const DefaultTax As Integer = 5
    ''' <summary>O chefe nasce depois de (maximo - 10) monstros, entao o maximo precisa passar de 10.</summary>
    Friend Const MinMaxMonster As Integer = 11
    Friend Const MaxMaxMonster As Integer = 50
    ''' <summary>_sHCORE_MONSTER.szName[32].</summary>
    Friend Const MaxNameBytes As Integer = 31
    ''' <summary>Open_SODConfig le cada linha com fgets(line, 255).</summary>
    Friend Const MaxLineBytes As Integer = 254
    ''' <summary>Rounds com chefe no site e nas tabelas do codigo.</summary>
    Friend Shared ReadOnly BossRounds As Integer() = {3, 6, 8}

    Private Const KeyTax As String = "*BELATRA_TAX"
    Private Const KeyText As String = "*BELATRA_TEXT"
    Private Const KeyDivScore As String = "*BELATRA_DIVSCORE"
    Private Const KeyMonster As String = "*BELATRA_MONSTER"

    Private Shared ReadOnly Cp949 As Encoding = Encoding.GetEncoding(949)

    Public Property Tax As Integer = DefaultTax
    Public ReadOnly Property Rounds As New List(Of BellatraRound)()
    ''' <summary>Linhas *BELATRA_DIVSCORE e *BELATRA_TEXT do arquivo lido: o site nao as mostrava, entao sao regravadas como estao.</summary>
    Public ReadOnly Property KeptLines As New List(Of String)()
    ''' <summary>Quantas linhas *BELATRA_MONSTER o arquivo lido tinha.</summary>
    Public Property MonsterLines As Integer

    Friend Shared Function FolderPath() As String
        Return Path.Combine(AppState.ServerPath, Folder)
    End Function

    Friend Shared Function FilePath() As String
        Return Path.Combine(FolderPath(), FileName)
    End Function

    ''' <summary>Configuracao vazia com 8 rounds.</summary>
    Public Sub New()
        For i = 1 To RoundCount
            Rounds.Add(New BellatraRound())
        Next
    End Sub

    Friend Shared Function IsBossRound(ByVal round As Integer) As Boolean
        Return Array.IndexOf(BossRounds, round) >= 0
    End Function

    ''' <summary>Texto legivel (coreano incluido) a partir da forma em memoria (bytes CP949 lidos como CP1252).</summary>
    Friend Shared Function Readable(ByVal name As String) As String
        Return Cp949.GetString(AppState.FileEncoding.GetBytes(name))
    End Function

    Private Shared Function FromKorean(ByVal name As String) As String
        Return AppState.FileEncoding.GetString(Cp949.GetBytes(name))
    End Function

    ''' <summary>
    ''' Nome do monstro em execucao, como o ConvertMonsterNames do servidor: o *B_NAME do .zhoon quando existe, senao o *이름.
    ''' </summary>
    Friend Shared Function RuntimeName(ByVal monster As MonsterFile) As String
        Dim name As String = monster.Name.Trim().Trim(""""c)
        If name = "" OrElse name = MonsterData.NameNotFound Then Return monster.MapName
        Return name
    End Function

#Region "Leitura e gravacao"

    ''' <summary>Le o arquivo como Open_SODConfig: chave, depois palavras ou textos entre aspas. Sem *BELATRA_TAX a taxa e 0.</summary>
    Public Shared Function Load(ByVal path As String) As BellatraConfig
        Dim config As New BellatraConfig With {.Tax = 0}
        Dim round As Integer = 0
        For Each line In FileHelper.ReadLines(path)
            Dim tokens = Tokenize(line)
            If tokens.Count = 0 Then Continue For
            Select Case tokens(0).ToUpperInvariant()
                Case KeyTax
                    Dim tax As Integer
                    If tokens.Count > 1 AndAlso Integer.TryParse(tokens(1), tax) Then config.Tax = tax
                Case KeyText, KeyDivScore
                    config.KeptLines.Add(line)
                Case KeyMonster
                    config.MonsterLines += 1
                    If round >= RoundCount Then Continue For
                    Dim r = config.Rounds(round)
                    For slot = 0 To SlotCount - 1
                        r.Monsters(slot) = Token(tokens, 1 + slot * 2)
                        r.Percents(slot) = Number(Token(tokens, 2 + slot * 2))
                    Next
                    r.Boss = Token(tokens, 9)
                    r.MaxMonster = Number(Token(tokens, 10))
                    round += 1
            End Select
        Next
        Return config
    End Function

    Private Shared Function Token(ByVal tokens As List(Of String), ByVal index As Integer) As String
        Return If(index < tokens.Count, tokens(index), "")
    End Function

    Private Shared Function Number(ByVal text As String) As Integer
        Dim value As Integer
        Integer.TryParse(text, value)
        Return value
    End Function

    ''' <summary>GetWord/GetString do servidor: separa por espaco ou tab; texto entre aspas vira uma palavra.</summary>
    Private Shared Function Tokenize(ByVal line As String) As List(Of String)
        Dim result As New List(Of String)()
        Dim i As Integer = 0
        Do While i < line.Length
            Dim c As Char = line(i)
            If c = " "c OrElse c = vbTab Then
                i += 1
            ElseIf c = """"c Then
                Dim close As Integer = line.IndexOf(""""c, i + 1)
                If close < 0 Then close = line.Length
                result.Add(line.Substring(i + 1, close - i - 1))
                i = close + 1
            Else
                Dim start As Integer = i
                Do While i < line.Length AndAlso line(i) <> " "c AndAlso line(i) <> vbTab
                    i += 1
                Loop
                result.Add(line.Substring(start, i - start))
            End If
        Loop
        Return result
    End Function

    Private Function MonsterLine(ByVal r As BellatraRound) As String
        Dim parts As New List(Of String) From {KeyMonster}
        For slot = 0 To SlotCount - 1
            parts.Add("""" & r.Monsters(slot) & """")
            parts.Add(If(r.Monsters(slot) = "", 0, r.Percents(slot)).ToString())
        Next
        parts.Add("""" & r.Boss & """")
        parts.Add(r.MaxMonster.ToString())
        Return String.Join(" ", parts)
    End Function

    ''' <summary>Grava o arquivo inteiro (backup em Bellatra\backup antes).</summary>
    Public Sub Save(ByVal path As String)
        Dim folder As String = IO.Path.GetDirectoryName(path)
        Directory.CreateDirectory(folder)
        BackupHelper.CreateBackup(folder, IO.Path.GetFileName(path))
        Dim lines As New List(Of String) From {
            "// Bellatra (SoD) - saved by PTEditor on " & DateTime.Now.ToString("yyyy-MM-dd HH:mm") & ".",
            "// The server reads this file at the start of each SoD (Open_SODConfig).",
            "// *BELATRA_TAX: % taken from each player's prize (not credited to any clan).",
            "// *BELATRA_MONSTER ""monster 1"" % ""monster 2"" % ""monster 3"" % ""monster 4"" % ""boss"" max-alive-per-room",
            "// Monster names = in-game names (*B_NAME of the .zhoon), as the server knows them after loading.",
            "",
            KeyTax & " " & Tax}
        lines.AddRange(KeptLines)
        For i = 0 To RoundCount - 1
            lines.Add("")
            lines.Add("//Round " & (i + 1))
            lines.Add(MonsterLine(Rounds(i)))
        Next
        FileHelper.WriteLines(path, lines)
    End Sub

#End Region

#Region "Validacao"

    ''' <summary>
    ''' Problemas que fariam o servidor ignorar o arquivo ou gerar monstros errados; lista vazia = ok.
    ''' knownNames: nomes em execucao dos monstros carregados (RuntimeName).
    ''' </summary>
    Public Function Validate(ByVal knownNames As ICollection(Of String)) As List(Of String)
        Dim problems As New List(Of String)()
        If Tax < 0 OrElse Tax > MaxTax Then problems.Add("Tax must be between 0 and " & MaxTax & "%.")
        For i = 0 To RoundCount - 1
            Dim r = Rounds(i)
            Dim label As String = "Round " & (i + 1) & ": "
            If r.Monsters(0) = "" Then problems.Add(label & "monster 1 is required (an empty monster 1 ends the event).")
            Dim total As Integer = 0
            Dim gap As Boolean = False
            For slot = 0 To SlotCount - 1
                Dim name As String = r.Monsters(slot)
                If name = "" Then
                    gap = True
                    Continue For
                End If
                If gap Then problems.Add(label & "fill the monsters in order (monster " & (slot + 1) & " comes after an empty one).")
                If r.Percents(slot) <= 0 Then problems.Add(label & "monster " & (slot + 1) & " needs a % above 0.")
                total += r.Percents(slot)
                CheckName(problems, label & "monster " & (slot + 1), name, knownNames)
            Next
            If total <> 100 Then problems.Add(label & "the % add up to " & total & "; they must add up to 100.")
            If r.Boss <> "" Then
                If Not IsBossRound(i + 1) Then problems.Add(label & "only rounds 3, 6 and 8 have a boss.")
                CheckName(problems, label & "boss", r.Boss, knownNames)
            End If
            If r.MaxMonster < MinMaxMonster OrElse r.MaxMonster > MaxMaxMonster Then
                problems.Add(label & "max alive must be between " & MinMaxMonster & " and " & MaxMaxMonster & ".")
            End If
            If AppState.FileEncoding.GetByteCount(MonsterLine(r)) + 2 > MaxLineBytes Then
                problems.Add(label & "the line is longer than " & MaxLineBytes & " bytes (the server would cut it). Use shorter names.")
            End If
        Next
        Return problems
    End Function

    Private Shared Sub CheckName(ByVal problems As List(Of String), ByVal what As String, ByVal name As String, ByVal knownNames As ICollection(Of String))
        If name.Contains("""") Then problems.Add(what & ": the name cannot contain quotes.")
        If AppState.FileEncoding.GetByteCount(name) > MaxNameBytes Then problems.Add(what & ": '" & Readable(name) & "' is longer than " & MaxNameBytes & " bytes.")
        If Not knownNames.Contains(name) Then problems.Add(what & ": '" & Readable(name) & "' is not the in-game name of a monster of GameServer\Monster (the server would skip it).")
    End Sub

#End Region

#Region "Tabelas do codigo"

    Friend Const LegacyTableCount As Integer = 3

    ''' <summary>
    ''' sHCoreEvent1/2/3 de Servidor\Legacy\SrcServer\OnSever.cpp (iguais ao PTClassic): por round,
    ''' 4 x (monstro, %), chefe e maximo. E o que o servidor usa quando nao ha bellatra.ini.
    ''' Os nomes do codigo sao o *이름 (o servidor os resolve antes do ConvertMonsterNames); ApplyLegacyTable os troca pelo
    ''' nome em execucao.
    ''' </summary>
    Private Shared ReadOnly LegacyTables As String()()() = {
        New String()() {
            New String() {"비독", "40", "코크리스", "40", "디코이", "20", "", "0", "", "40"},
            New String() {"바곤", "35", "스켈레톤아처", "20", "크립트", "45", "", "0", "", "40"},
            New String() {"싸이클롭스 나이트", "35", "스켈레톤아처", "30", "스켈레톤워리어", "35", "", "0", "언홀리 나이트", "40"},
            New String() {"헤드커터", "45", "스켈레톤레인저", "25", "이블 메이플", "20", "웹", "10", "", "35"},
            New String() {"일루젼나이트", "30", "파이곤", "20", "둠 가드", "40", "그리븐", "10", "", "30"},
            New String() {"헐크", "20", "파이곤", "20", "슬러터", "40", "쉐도우", "20", "베르문 악타룬", "25"},
            New String() {"오미크론", "30", "스티지언", "40", "헐크", "20", "둠 가드", "10", "", "20"},
            New String() {"아이언 가드", "20", "킹호피", "30", "인큐버스", "30", "하이퍼 머신", "20", "둠 로드", "15"}
        },
        New String()() {
            New String() {"메핏", "40", "노쓰고블린", "40", "데벌리쉬 트리", "20", "", "0", "", "40"},
            New String() {"바곤", "25", "메핏", "40", "부마", "35", "", "0", "", "40"},
            New String() {"싸이클롭스 나이트", "30", "스켈레톤아처", "30", "스켈레톤워리어", "40", "", "0", "언홀리 나이트", "40"},
            New String() {"스켈레톤나이트", "45", "스켈레톤레인저", "25", "마이어 키퍼", "20", "웹", "10", "", "35"},
            New String() {"슬러터", "30", "파이곤", "20", "다크 나이트", "40", "그리븐", "10", "", "30"},
            New String() {"헐크", "20", "슬리버", "20", "아벨리스크-L", "40", "쉐도우", "20", "베르문 악타룬", "25"},
            New String() {"다우린", "30", "스티지언", "40", "헐크", "20", "둠 가드", "10", "", "20"},
            New String() {"아이언 가드", "20", "킹호피", "30", "아벨리스크 로드", "30", "하이퍼 머신", "20", "둠 로드", "15"}
        },
        New String()() {
            New String() {"메핏", "40", "좀비", "40", "홉고블린 칩", "20", "", "0", "", "40"},
            New String() {"구울", "25", "스켈레톤아처", "40", "싸이클롭스", "35", "", "0", "", "40"},
            New String() {"마이티고블린", "30", "스켈레톤아처", "30", "아머드 비틀", "40", "", "0", "언홀리 나이트", "40"},
            New String() {"타이탄", "45", "스켈레톤레인저", "25", "머디 고렘", "20", "웹", "10", "", "35"},
            New String() {"나이트메어", "30", "파이곤", "20", "비블", "40", "그리븐", "10", "", "30"},
            New String() {"헐크", "20", "슬리버", "20", "아벨리스크-L", "40", "쉐도우", "20", "베르문 악타룬", "25"},
            New String() {"오미크론", "30", "스티지언", "40", "헐크", "20", "둠 가드", "10", "", "20"},
            New String() {"아이언 가드", "20", "체인 고렘", "30", "위치", "30", "하이퍼 머신", "20", "둠 로드", "15"}
        }
    }

    ''' <summary>
    ''' Rounds da tabela do codigo (1 a 3), mantendo a taxa e as linhas guardadas da configuracao atual. Cada *이름 vira o
    ''' nome em execucao do primeiro monstro carregado com esse *이름; sem monstro, fica o *이름 (marcado como nao encontrado).
    ''' </summary>
    Public Sub ApplyLegacyTable(ByVal table As Integer)
        Dim rows = LegacyTables(table - 1)
        For i = 0 To RoundCount - 1
            Dim r = Rounds(i)
            For slot = 0 To SlotCount - 1
                r.Monsters(slot) = LegacyName(rows(i)(slot * 2))
                r.Percents(slot) = Integer.Parse(rows(i)(slot * 2 + 1))
            Next
            r.Boss = LegacyName(rows(i)(8))
            r.MaxMonster = Integer.Parse(rows(i)(9))
        Next
    End Sub

    Private Shared Function LegacyName(ByVal korean As String) As String
        If korean = "" Then Return ""
        Dim mapName As String = FromKorean(korean)
        For Each monster In AppState.Monsters
            If monster.MapName.Equals(mapName, StringComparison.OrdinalIgnoreCase) Then Return RuntimeName(monster)
        Next
        Return mapName
    End Function

#End Region

End Class
