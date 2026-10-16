''' <summary>
''' Tabela de EXP do servidor (ExpLevelTable, copiada de Servidor\Gameplay\Character\LevelTable.h).
''' Entrada N-1 = EXP no inicio do level N. O servidor confere Level x EXP ao carregar o personagem
''' (CheckLevelExp / GetLevelFromExp em playsub.cpp) e corrige o level se nao baterem.
''' </summary>
Friend Module LevelTable

    ''' <summary>Ultimo level alcancavel: GetLevelFromExp so encontra levels ate Values.Length - 1 (CHAR_LEVEL_MAX 171 no Brasil).</summary>
    Friend ReadOnly Property MaxLevel As Integer
        Get
            Return Values.Length - 1
        End Get
    End Property

    Private ReadOnly Values As Long() = {
        0L, 1000L, 2500L, 5000L, 9500L, 17100L,
        29925L, 51471L, 87500L, 140001L, 212801L, 306434L,
        416750L, 537608L, 672010L, 833293L, 1024950L, 1250439L,
        1515533L, 1824702L, 2182343L, 2592624L, 3059296L, 3591614L,
        4195005L, 4874596L, 5635033L, 6480288L, 7413449L, 8443919L,
        9575404L, 10820207L, 12194373L, 13706475L, 15378665L, 17224105L,
        19273774L, 21548079L, 24069204L, 26861232L, 29950274L, 33364605L,
        37134805L, 41293904L, 45836233L, 50878219L, 56474823L, 62687054L,
        69582630L, 77236719L, 85809995L, 95334904L, 105917079L, 117673875L,
        130735675L, 145312702L, 161515069L, 179523999L, 199540925L, 221789738L,
        246630189L, 274252770L, 304969080L, 339125617L, 377107687L, 419532302L,
        466729685L, 519236775L, 577650912L, 642636640L, 715254581L, 796078348L,
        886035202L, 986157179L, 1097592941L, 1222169740L, 1360886005L, 1515346567L,
        1687338402L, 1878851311L, 2104313468L, 2356831084L, 2639650815L, 2956408912L,
        3311177982L, 3708519340L, 4153541661L, 4651966660L, 5210202659L, 6252243191L,
        7502691829L, 9003230195L, 10803876234L, 12964651481L, 15557581778L, 18669098134L,
        22402917760L, 26883501313L, 32260201575L, 38712241890L, 46454690268L, 55745628322L,
        66894753987L, 80273704784L, 96328445741L, 115594134890L, 138712961868L, 166455554241L,
        199746665090L, 239695998108L, 287635197729L, 345162237275L, 414194684730L, 497033621676L,
        596440346012L, 715728415214L, 858874098257L, 1030648917908L, 1236778701490L, 1484134441788L,
        1780961330146L, 2137153596175L, 2564584315410L, 3077501178492L, 3693001414190L, 4431601697028L,
        5317922036434L, 6381506443721L, 7657807732465L, 9189369278958L, 11027243134749L, 13232691761699L,
        15879230114039L, 19055076136847L, 22866091364216L, 27439309637060L, 32927171564472L, 39512605877366L,
        47415127052839L, 56898152463407L, 67803631685560L, 80344932791036L, 94767429062334L, 111353299774326L,
        130427051093117L, 152361865109727L, 177586901228828L, 206595692765795L, 239955803033306L, 278319929840944L,
        316684056648582L, 355048183456220L, 393412310263858L, 431776437071496L, 470140563879134L, 508504690686772L,
        546868817494410L, 585232944302048L, 623597071109686L, 661961197917324L, 710325324724962L, 720325324724963L,
        730325324724964L, 740325324724965L, 750325324724966L, 760325324724967L, 770325324724968L, 780325324724969L,
        790325324724970L, 800325324724971L, 810325324724972L
    }

    ''' <summary>EXP no inicio do level (1..MaxLevel).</summary>
    Friend Function ExpAtLevelStart(ByVal level As Integer) As Long
        Return Values(Math.Max(1, Math.Min(level, MaxLevel)) - 1)
    End Function

    ''' <summary>Maior EXP que o servidor ainda aceita para o level (limite superior inclusivo, como em CheckLevelExp).</summary>
    Friend Function ExpAtLevelEnd(ByVal level As Integer) As Long
        Return Values(Math.Max(1, Math.Min(level, MaxLevel)))
    End Function

    ''' <summary>CheckLevelExp: True se o servidor aceita a combinacao sem mexer no level.</summary>
    Friend Function IsValid(ByVal level As Integer, ByVal exp As Long) As Boolean
        If level < 1 OrElse level > MaxLevel Then Return False
        Return exp >= Values(level - 1) AndAlso exp <= Values(level)
    End Function

    ''' <summary>GetLevelFromExp: level que o servidor atribui a essa EXP (0 se nenhum).</summary>
    Friend Function LevelFromExp(ByVal exp As Long) As Integer
        For i = 0 To MaxLevel - 1
            If exp >= Values(i) AndAlso exp < Values(i + 1) Then Return i + 1
        Next
        Return 0
    End Function

End Module
