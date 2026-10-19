Imports System.IO
Imports System.Text

''' <summary>Estado global: caminhos do servidor e dados carregados do GameServer.</summary>
Friend Module AppState

    ''' <summary>
    ''' Os arquivos do GameServer sao CP949, mas o programa sempre os leu/gravou como CP1252 (&amp;H4E4).
    ''' Manter assim: as chaves em GameFields dependem disso e a gravacao reproduz os bytes originais.
    ''' </summary>
    Friend ReadOnly FileEncoding As Encoding = Encoding.GetEncoding(1252)

    Friend Const ConfigFileName As String = "PTEditor.Config.txt"
    Friend Const ErrorLogFileName As String = "PTEditor_Errors.log"
    Friend Const MapsImageFolder As String = "Maps"

    Friend Const MonsterFolder As String = "GameServer\Monster\"
    Friend Const ItemFolder As String = "GameServer\OpenItem\"
    Friend Const FieldFolder As String = "GameServer\Field\"
    Friend Const MeshFolder As String = "Field\"
    Friend Const NpcFolder As String = "GameServer\NPC\"
    Friend Const NpcNameFolder As String = "GameServer\NPC\name\"
    Friend Const CharFolder As String = "char"
    Friend Const DataServerFolder As String = "DataServer"
    Friend Const DataServerBackupFolder As String = "PTEditor_Backup"

    Friend Const MonsterPattern As String = "*.inf"
    Friend Const ItemPattern As String = "*.txt"
    Friend Const MapPattern As String = "*.spm"
    Friend Const NpcPattern As String = "*.NPC"
    Friend Const SpcPattern As String = "*.spc"
    Friend Const SppPattern As String = "*.spp"

    Friend Const BackupFolder As String = "backup"
    Friend Const BackupTag As String = "_Backup_"
    Friend Const DefaultBackupPrefix As String = "Default_"

    Friend Const Explorer As String = "Explorer.exe"

    ''' <summary>Pasta raiz do servidor, sempre terminada em "\".</summary>
    Friend ServerPath As String = ""
    ''' <summary>Editor de texto usado para abrir os arquivos.</summary>
    Friend EditorExe As String = Explorer

    ''' <summary>Tag de idioma forcada pelo config (Name-Tag=); vazio = auto-detectar.</summary>
    Friend NameTagOverride As String = ""

    ''' <summary>Letra do idioma dos .zhoon ("B" em *B_NAME): override do config, senao a mais comum nos arquivos lidos, senao "J".</summary>
    Friend ReadOnly Property NameTag As String
        Get
            If NameTagOverride <> "" Then Return NameTagOverride
            Dim detected As String = ZhoonFile.DetectedTag
            Return If(detected = "", ZhoonFile.DefaultTag, detected)
        End Get
    End Property

    ''' <summary>Pasta DataServer (personagens, contas e baus); vazio = &lt;ServerPath&gt;\DataServer.</summary>
    Friend DataServerPath As String = ""
    ''' <summary>Personagens/contas carregados pela aba Characters (Nothing ate o primeiro carregamento).</summary>
    Friend CharacterIndex As CharacterIndex

    Friend ReadOnly Monsters As New List(Of MonsterFile)()
    Friend ReadOnly Items As New List(Of ItemFile)()
    Friend ReadOnly Maps As New List(Of MapFile)()
    Friend ReadOnly Npcs As New List(Of NpcFile)()
    Friend ReadOnly SpcFiles As New List(Of BinaryDataFile)()
    Friend ReadOnly SppFiles As New List(Of BinaryDataFile)()

    Friend ReadOnly Property Version As String
        Get
            Return My.Application.Info.Version.ToString(2)
        End Get
    End Property

    Friend ReadOnly Property ConfigFilePath As String
        Get
            Return Path.Combine(Application.StartupPath, ConfigFileName)
        End Get
    End Property

    Friend Function MonsterPath(ByVal fileName As String) As String
        Return ServerPath & MonsterFolder & fileName
    End Function

    Friend Function ItemPath(ByVal fileName As String) As String
        Return ServerPath & ItemFolder & fileName
    End Function

    Friend Function FieldPath(ByVal fileName As String) As String
        Return ServerPath & FieldFolder & fileName
    End Function

    Friend Function NpcPath(ByVal fileName As String) As String
        Return ServerPath & NpcFolder & fileName
    End Function

    Friend Function MapImagePath(ByVal imageName As String) As String
        Return Path.Combine(Application.StartupPath, MapsImageFolder, imageName)
    End Function

End Module
