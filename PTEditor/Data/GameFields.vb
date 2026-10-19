Imports System.Text

''' <summary>
''' Chaves dos arquivos de dados do GameServer (Monster/*.inf, OpenItem/*.txt, Field/*.spm, NPC/*.NPC).
''' Os arquivos sao CP949 mas o programa os le como CP1252 (ver AppState.FileEncoding); por isso cada
''' chave e convertida pelo helper Key() para a forma CP1252 que aparece em memoria. O texto coreano fica
''' legivel no codigo e os bytes gravados no arquivo continuam identicos aos originais.
''' </summary>
Friend Module GameFields

    Private ReadOnly Cp949 As Encoding = Encoding.GetEncoding(949)

    Friend Const EndOfFile As String = "End of File"

    ''' <summary>Converte texto coreano para a forma CP1252 usada em memoria. ASCII passa inalterado.</summary>
    Private Function Key(ByVal korean As String) As String
        Return AppState.FileEncoding.GetString(Cp949.GetBytes(korean))
    End Function

    ' ---- Comuns ---------------------------------------------------------------
    Friend ReadOnly LinkedFile As String = Key("*연결파일")      ' arquivo .zhoon com nome/chat
    Friend ReadOnly Name As String = Key("*이름")               ' monstro: nome de mapa; item: nome JP
    Friend ReadOnly Level As String = Key("*레벨")

    ' ---- Monster\*.inf --------------------------------------------------------
    Friend NotInheritable Class Monster
        Private Sub New()
        End Sub
        Friend Shared ReadOnly Boss As String = Key("*두목")
        Friend Shared ReadOnly ModelSize As String = Key("*모델크기")
        Friend Shared ReadOnly SpecialCode As String = Key("*구별코드")
        Friend Shared ReadOnly Organization As String = Key("*조직")       ' UI: "Organization"
        Friend Shared ReadOnly Intelligence As String = Key("*지능")
        Friend Shared ReadOnly Character As String = Key("*품성")
        Friend Shared ReadOnly Vision As String = Key("*시야")             ' UI: "Range"
        Friend Shared ReadOnly HP As String = Key("*생명력")
        Friend Shared ReadOnly MoveSpeed As String = Key("*이동속도")
        Friend Shared ReadOnly MoveType As String = Key("*이동타입")
        Friend Shared ReadOnly Sound As String = Key("*효과음")
        Friend Shared ReadOnly Exp As String = Key("*경험치")
        Friend Shared ReadOnly Race As String = Key("*몬스터종족")
        Friend Shared ReadOnly RaceDaemon As String = Key("디먼")
        Friend Shared ReadOnly RaceUndead As String = Key("언데드")
        Friend Shared ReadOnly RaceMutant As String = Key("뮤턴트")
        Friend Shared ReadOnly RaceMechanic As String = Key("메카닉")
        Friend Shared ReadOnly AttackSpeed As String = Key("*공격속도")
        Friend Shared ReadOnly AttackPower As String = Key("*공격력")
        Friend Shared ReadOnly SkillPower As String = Key("*기술공격력")
        Friend Shared ReadOnly SkillRange As String = Key("*기술공격거리")  ' UI: "Critical Attack"
        Friend Shared ReadOnly AttackRating As String = Key("*명중력")
        Friend Shared ReadOnly PerfectRate As String = Key("*특수공격률")  ' UI: "Perfect Rating"
        Friend Shared ReadOnly Defense As String = Key("*방어력")
        Friend Shared ReadOnly Absorb As String = Key("*흡수율")
        Friend Shared ReadOnly Block As String = Key("*블럭율")
        Friend Shared ReadOnly Organic As String = Key("*생체")
        Friend Shared ReadOnly Lightning As String = Key("*번개")
        Friend Shared ReadOnly Ice As String = Key("*얼음")
        Friend Shared ReadOnly Fire As String = Key("*불")
        Friend Shared ReadOnly Poison As String = Key("*독")
        Friend Shared ReadOnly Magic As String = Key("*매직")
        Friend Shared ReadOnly Drop As String = Key("*아이템")
        Friend Shared ReadOnly DropAll As String = Key("*아이템모두")
        Friend Shared ReadOnly DropCounter As String = Key("*아이템카운터")
        Friend Shared ReadOnly ExtraDrop As String = Key("*추가아이템")
        Friend Shared ReadOnly AllSuffix As String = Key("모두")           ' sufixo de DropAll
        Friend Shared ReadOnly DropNothing As String = Key("없음")         ' UI: "Nothing"
        Friend Shared ReadOnly DropGold As String = Key("돈")              ' UI: "Gold"
    End Class

    ' ---- OpenItem\*.txt -------------------------------------------------------
    Friend NotInheritable Class Item
        Private Sub New()
        End Sub
        Friend Shared ReadOnly Code As String = Key("*코드")
        Friend Shared ReadOnly EngName As String = "*Name"
        Friend Shared ReadOnly Weight As String = Key("*무게")
        Friend Shared ReadOnly Price As String = Key("*가격")
        Friend Shared ReadOnly Durability As String = Key("*내구력")      ' UI: "Integrity"
        Friend Shared ReadOnly Unique As String = Key("*유니크")          ' UI: "Quest"
        Friend Shared ReadOnly UniqueColor As String = Key("*유니크색상")  ' UI: "Glow"
        Friend Shared ReadOnly Organic As String = Key("*생체")
        Friend Shared ReadOnly Fire As String = Key("*불")
        Friend Shared ReadOnly Frost As String = Key("*냉기")
        Friend Shared ReadOnly Lightning As String = Key("*번개")
        Friend Shared ReadOnly Poison As String = Key("*독")
        Friend Shared ReadOnly HPRecover As String = Key("*생명력상승")
        Friend Shared ReadOnly MPRecover As String = Key("*기력상승")
        Friend Shared ReadOnly STMRecover As String = Key("*근력상승")
        Friend Shared ReadOnly Capacity As String = Key("*보유공간")      ' UI: "Pots"
        Friend Shared ReadOnly AttackPower As String = Key("*공격력")
        Friend Shared ReadOnly Range As String = Key("*사정거리")
        Friend Shared ReadOnly AttackSpeed As String = Key("*공격속도")
        Friend Shared ReadOnly AttackRating As String = Key("*명중력")
        Friend Shared ReadOnly Critical As String = Key("*크리티컬")
        Friend Shared ReadOnly Defense As String = Key("*방어력")
        Friend Shared ReadOnly Block As String = Key("*블럭율")
        Friend Shared ReadOnly Absorb As String = Key("*흡수력")
        Friend Shared ReadOnly HPRegen As String = Key("*생명력재생")
        Friend Shared ReadOnly MPRegen As String = Key("*기력재생")
        Friend Shared ReadOnly STMRegen As String = Key("*근력재생")
        Friend Shared ReadOnly HPAdd As String = Key("*생명력추가")
        Friend Shared ReadOnly MPAdd As String = Key("*기력추가")
        Friend Shared ReadOnly STMAdd As String = Key("*근력추가")
        Friend Shared ReadOnly MagicMastery As String = Key("*마법기술숙련도")  ' UI: "APT"
        Friend Shared ReadOnly Strength As String = Key("*힘")
        Friend Shared ReadOnly Spirit As String = Key("*정신력")
        Friend Shared ReadOnly Talent As String = Key("*재능")
        Friend Shared ReadOnly Agility As String = Key("*민첩성")
        Friend Shared ReadOnly Health As String = Key("*건강")            ' UI: "req HP"
        Friend Shared ReadOnly MoveSpeed As String = Key("*이동속도")
        Friend Shared ReadOnly Spec As String = Key("**특화")             ' classes primarias
        Friend Shared ReadOnly SpecRandom As String = Key("**특화랜덤")    ' classes secundarias
        Friend Shared ReadOnly SpAttackSpeed As String = Key("**공격속도")
        Friend Shared ReadOnly SpCritical As String = Key("**크리티컬")
        Friend Shared ReadOnly SpAttackPower As String = Key("**공격력")
        Friend Shared ReadOnly SpAttackRating As String = Key("**명중력")
        Friend Shared ReadOnly SpAbsorb As String = Key("**흡수력")
        Friend Shared ReadOnly SpDefense As String = Key("**방어력")
        Friend Shared ReadOnly SpBlock As String = Key("**블럭율")
        Friend Shared ReadOnly SpHPRegen As String = Key("**생명력재생")
        Friend Shared ReadOnly SpMPRegen As String = Key("**기력재생")
        Friend Shared ReadOnly SpMoveSpeed As String = Key("**이동속도")
        Friend Shared ReadOnly SpRange As String = Key("**사정거리")

        Friend Const ClassFighter As String = "Fighter"
        Friend Const ClassMechanician As String = "Mechanician"
        Friend Const ClassArcher As String = "Archer"
        Friend Const ClassPikeman As String = "Pikeman"
        Friend Const ClassAtalanta As String = "Atalanta"
        Friend Const ClassKnight As String = "Knight"
        Friend Const ClassMagician As String = "Magician"
        Friend Const ClassPriestess As String = "Priestess"
    End Class

    ' ---- Field\*.spm ----------------------------------------------------------
    Friend NotInheritable Class Map
        Private Sub New()
        End Sub
        Friend Shared ReadOnly MaxSpawn As String = Key("*최대동시출현수")
        Friend Shared ReadOnly SpawnInterval As String = Key("*출현간격")
        Friend Shared ReadOnly SpawnCount As String = Key("*출현수")
        Friend Shared ReadOnly Spawn As String = Key("*출연자")
        Friend Shared ReadOnly BossSpawn As String = Key("*출연자두목")
        Friend Shared ReadOnly HeaderTitle As String = Key("//몬스터 출현 비율") & vbCrLf
        Friend Shared ReadOnly HeaderInterval As String = Key("//출현간격 범위 9-1 1차이당 2배씩 빨라짐")
        Friend Shared ReadOnly HeaderColumns As String = Key("//" & vbTab & vbTab & "몬스터 이름          출현빈도") & vbCrLf
    End Class

    ' ---- NPC\*.NPC ------------------------------------------------------------
    Friend NotInheritable Class Npc
        Private Sub New()
        End Sub
        Friend Shared ReadOnly ModelFile As String = Key("*모양파일")
        Friend Shared ReadOnly ShopArmor As String = Key("*방어구판매")
        Friend Shared ReadOnly ShopWeapon As String = Key("*무기판매")
        Friend Shared ReadOnly ShopMisc As String = Key("*잡화판매")
        ''' <summary>Ordem igual ao combo "CBShopItem" do editor.</summary>
        Friend Shared ReadOnly ShopKeys As String() = {ShopArmor, ShopWeapon, ShopMisc}
    End Class

End Module
