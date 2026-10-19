Imports System.IO
Imports System.Text

''' <summary>Estado global: caminhos do servidor e dados carregados do GameServer.</summary>
Friend Module AppState

    ''' <summary>
    ''' Os arquivos do GameServer sao CP949, mas o programa sempre os leu/gravou como CP1252 (&amp;H4E4).
    ''' Manter assim: as chaves em GameFields dependem disso e a gravacao reproduz os bytes originais.
    ''' </summary>
    Friend ReadOnly FileEncoding As Encoding = Encoding.GetEncoding(1252)

    ''' <summary>Pasta (ao lado do executavel) onde o programa grava o que gera: config, logs e escolha de perfis.</summary>
    Friend Const OutputFolder As String = "PTEditor"
    Friend Const LogsFolder As String = "Logs"
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
            Return Path.Combine(OutputDir, ConfigFileName)
        End Get
    End Property

    Private _outputReady As Boolean

    ''' <summary>&lt;exe&gt;\PTEditor: criada no primeiro uso, trazendo os arquivos que versoes antigas deixavam ao lado do exe.</summary>
    Friend ReadOnly Property OutputDir As String
        Get
            Dim dir As String = Path.Combine(Application.StartupPath, OutputFolder)
            If Not _outputReady Then
                _outputReady = True
                Try
                    Directory.CreateDirectory(Path.Combine(dir, LogsFolder))
                    MoveLegacy(ConfigFileName, Path.Combine(dir, ConfigFileName))
                    For Each logName In {ErrorLogFileName, "AwardItems.log", "PostBox.log"}
                        MoveLegacy(logName, Path.Combine(dir, LogsFolder, logName))
                    Next
                    MoveLegacy(Path.Combine("Profiles", "DataServers.txt"), Path.Combine(dir, "DataServers.txt"))
                    MoveLegacyFolder("Profiles", Path.Combine(dir, "Profiles"))
                    MoveLegacyFolder(MapsImageFolder, Path.Combine(dir, MapsImageFolder))
                Catch
                    ' Sem permissao: os arquivos ficam onde estao.
                End Try
            End If
            Return dir
        End Get
    End Property

    ''' <summary>&lt;exe&gt;\PTEditor\Logs, criada junto com OutputDir.</summary>
    Friend ReadOnly Property LogsDir As String
        Get
            Return Path.Combine(OutputDir, LogsFolder)
        End Get
    End Property

    ''' <summary>Move os arquivos da pasta antiga (ao lado do exe) para o destino, descarta os ja existentes e remove a pasta se esvaziar.</summary>
    Private Sub MoveLegacyFolder(ByVal relativeOld As String, ByVal targetDir As String)
        Dim source As String = Path.Combine(Application.StartupPath, relativeOld)
        If Not Directory.Exists(source) Then Return
        Directory.CreateDirectory(targetDir)
        For Each f In Directory.GetFiles(source)
            Dim target As String = Path.Combine(targetDir, Path.GetFileName(f))
            If Not File.Exists(target) Then File.Move(f, target) Else File.Delete(f)
        Next
        If Directory.GetFileSystemEntries(source).Length = 0 Then Directory.Delete(source)
    End Sub

    Private Sub MoveLegacy(ByVal relativeOld As String, ByVal target As String)
        Dim source As String = Path.Combine(Application.StartupPath, relativeOld)
        If File.Exists(source) AndAlso Not File.Exists(target) Then File.Move(source, target)
    End Sub

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
        Return Path.Combine(OutputDir, MapsImageFolder, imageName)
    End Function

End Module
