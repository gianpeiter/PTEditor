Imports System.Data.SqlClient

''' <summary>
''' Banco do servidor compartilhado pelas abas (Award Items, exclusao de personagem em Characters). A configuracao
''' vem do server.ini do Server Path e e editada/testada em Options; o status mais recente e avisado por StatusChanged.
''' </summary>
Friend Module ServerDatabase

    ''' <summary>Configuracao lida do server.ini; Nothing se faltar o arquivo ou o Host (motivo em LoadError).</summary>
    Friend ReadOnly Property Config As DatabaseConfig
        Get
            If Not _loaded Then Reload()
            Return _config
        End Get
    End Property

    Friend ReadOnly Property LoadError As String
        Get
            If Not _loaded Then Reload()
            Return _loadError
        End Get
    End Property

    Friend ReadOnly Property StatusText As String = ""
    Friend ReadOnly Property StatusColor As Color = SystemColors.GrayText

    Friend Event StatusChanged()

    Private _config As DatabaseConfig
    Private _loadError As String = ""
    Private _loaded As Boolean

    ''' <summary>Rele o server.ini (Server Path trocado ou arquivo editado).</summary>
    Friend Sub Reload()
        _loaded = True
        Try
            _config = DatabaseConfig.Load(AppState.ServerPath)
            _loadError = ""
            SetStatus("Read from " & _config.FilePath & ". Not tested yet.", SystemColors.GrayText)
        Catch ex As Exception
            _config = Nothing
            _loadError = ex.Message
            SetStatus(ex.Message, Color.Firebrick)
        End Try
    End Sub

    ''' <summary>Configuracao pronta; lanca excecao com o motivo se nao houver.</summary>
    Friend Function RequireConfig() As DatabaseConfig
        If Config Is Nothing Then Throw New InvalidOperationException(LoadError & " Check the database in Options.")
        Return Config
    End Function

    Friend Sub SetStatus(ByVal text As String, ByVal color As Color)
        _StatusText = text
        _StatusColor = color
        RaiseEvent StatusChanged()
    End Sub

    ''' <summary>
    ''' Rele o server.ini, conecta e confere o que as abas usam: PristonDB.AwardItem_List e ClanDB (UL/CL).
    ''' O resultado fica no status (verde = tudo ok, laranja = conectou mas falta algo, vermelho = falhou).
    ''' </summary>
    Friend Sub Test()
        Reload()
        If _config Is Nothing Then Return
        Dim problems As New List(Of String)()
        Try
            Try
                Dim awards As New AwardItemRepository(_config)
                awards.Test()
            Catch ex As SqlException When ex.Number <> 4060
                Throw
            Catch ex As Exception
                ' 4060 = banco inexistente ou sem acesso: conectou no servidor, so falta o banco
                problems.Add("Award Items: " & ex.Message)
            End Try
            Try
                ClanMembership.Test(_config)
            Catch ex As SqlException When ex.Number <> 4060
                Throw
            Catch ex As Exception
                problems.Add("Clan check: " & ex.Message)
            End Try
        Catch ex As Exception
            SetStatus("Connection failed: " & ex.Message, Color.Firebrick)
            Return
        End Try
        If problems.Count = 0 Then
            SetStatus("Connected: " & AwardItemRepository.DatabaseName & " (AwardItem_List) and " &
                      ClanMembership.DatabaseName & " (UL, CL) OK.", Color.DarkGreen)
        Else
            SetStatus("Connected, but: " & String.Join("  |  ", problems), Color.DarkOrange)
        End If
    End Sub

End Module
