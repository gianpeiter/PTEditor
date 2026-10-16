''' <summary>Arquivo de texto do GameServer carregado em memoria (uma entrada por linha).</summary>
Friend Class TextDataFile
    Public ReadOnly Property FileName As String
    Public ReadOnly Property Lines As New List(Of String)()
    ''' <summary>Se o arquivo original terminava com quebra de linha (preservado ao gravar).</summary>
    Public Property TrailingNewline As Boolean = True

    Public Sub New(ByVal fileName As String)
        Me.FileName = fileName
    End Sub

    ''' <summary>Substitui as linhas pelo conteudo do arquivo.</summary>
    Public Sub LoadFrom(ByVal path As String)
        Dim trailing As Boolean
        Dim content = FileHelper.ReadLines(path, trailing)
        Lines.Clear()
        Lines.AddRange(content)
        TrailingNewline = trailing
    End Sub

    Public Sub SaveTo(ByVal path As String)
        FileHelper.WriteLines(path, Lines, TrailingNewline)
    End Sub

    ''' <summary>Indice da primeira linha que comeca com a chave, ou -1.</summary>
    Public Function IndexOf(ByVal key As String) As Integer
        For i = 0 To Lines.Count - 1
            If Lines(i).StartsWith(key) Then Return i
        Next
        Return -1
    End Function

    Public Function ContainsText(ByVal text As String) As Boolean
        Return Lines.Any(Function(l) l.Contains(text))
    End Function
End Class

''' <summary>GameServer\Monster\*.inf</summary>
Friend Class MonsterFile
    Inherits TextDataFile

    ''' <summary>Nome in-game lido do .zhoon (*J_Name).</summary>
    Public Property Name As String = ""
    ''' <summary>Valor de *이름; e o nome referenciado pelos mapas (.spm).</summary>
    Public Property MapName As String = ""
    Public Property Level As String = ""
    ''' <summary>Caminho completo do .zhoon vinculado (*연결파일).</summary>
    Public Property ZhoonFile As String = ""

    Public Sub New(ByVal fileName As String)
        MyBase.New(fileName)
    End Sub
End Class

''' <summary>GameServer\OpenItem\*.txt</summary>
Friend Class ItemFile
    Inherits TextDataFile

    Public Const NoName As String = "No name found"
    Public Const NoZhoon As String = "No Zhoon found"

    ''' <summary>Nome in-game lido do .zhoon (*J_Name).</summary>
    Public Property Name As String = NoName
    ''' <summary>Valor de *코드 em maiusculas, com as aspas do arquivo (ex.: "DA101").</summary>
    Public Property Code As String = ""
    Public Property Weight As String = ""
    Public Property Level As String = ""
    Public Property ZhoonFile As String = ""

    Public Sub New(ByVal fileName As String)
        MyBase.New(fileName)
    End Sub

    ''' <summary>Codigo sem aspas (ex.: DA101).</summary>
    Public ReadOnly Property PlainCode As String
        Get
            Return Code.Trim(""""c)
        End Get
    End Property
End Class

''' <summary>GameServer\Field\*.spm</summary>
Friend Class MapFile
    Inherits TextDataFile

    Public Sub New(ByVal fileName As String)
        MyBase.New(fileName)
    End Sub
End Class

''' <summary>GameServer\NPC\*.NPC</summary>
Friend Class NpcFile
    Inherits TextDataFile

    ''' <summary>Nome lido do .zhoon (*J_NAME).</summary>
    Public Property Name As String = ""
    ''' <summary>Valor de *모양파일 em maiusculas (arquivo .inx do modelo).</summary>
    Public Property SetupIni As String = ""

    Public Sub New(ByVal fileName As String)
        MyBase.New(fileName)
    End Sub
End Class

''' <summary>Arquivo binario (Field\*.spc posicao de NPC, Field\*.spp spawn) carregado em memoria.</summary>
Friend Class BinaryDataFile
    Public ReadOnly Property FileName As String
    Public Property Data As Byte()

    Public Sub New(ByVal fileName As String, ByVal data As Byte())
        Me.FileName = fileName
        Me.Data = data
    End Sub
End Class
