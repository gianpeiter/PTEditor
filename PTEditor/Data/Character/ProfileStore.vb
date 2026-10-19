Imports System.IO

''' <summary>
''' Perfis em &lt;exe&gt;\PTEditor\Profiles\*.ini e a escolha de perfil de cada DataServer (PTEditor\DataServers.txt,
''' uma linha "pasta|perfil"; sem linha = deteccao automatica).
''' </summary>
Friend NotInheritable Class ProfileStore

    Friend Const FolderName As String = "Profiles"
    Private Const SelectionFileName As String = "DataServers.txt"
    Friend Const AutoDetect As String = ""

    Private Shared _profiles As New List(Of CharacterProfile)()
    Private Shared _errors As New Dictionary(Of String, String)()
    Private Shared _loaded As Boolean

    Private Sub New()
    End Sub

    Public Shared ReadOnly Property Folder As String
        Get
            Return Path.Combine(AppState.OutputDir, FolderName)
        End Get
    End Property

    ''' <summary>Perfis validos (carrega na primeira chamada).</summary>
    Public Shared ReadOnly Property Profiles As List(Of CharacterProfile)
        Get
            If Not _loaded Then Reload()
            Return _profiles
        End Get
    End Property

    ''' <summary>Arquivos .ini que nao puderam ser lidos (arquivo -> motivo).</summary>
    Public Shared ReadOnly Property Errors As Dictionary(Of String, String)
        Get
            If Not _loaded Then Reload()
            Return _errors
        End Get
    End Property

    Public Shared Sub Reload()
        Dim profiles As New List(Of CharacterProfile)()
        Dim errors As New Dictionary(Of String, String)()
        If Directory.Exists(Folder) Then
            For Each f In Directory.GetFiles(Folder, "*.ini").OrderBy(Function(x) x, StringComparer.OrdinalIgnoreCase)
                Try
                    Dim profile = CharacterProfile.Load(f)
                    If profiles.Any(Function(p) p.Name.Equals(profile.Name, StringComparison.OrdinalIgnoreCase)) Then
                        errors(f) = "Duplicate profile name '" & profile.Name & "'."
                    Else
                        profiles.Add(profile)
                    End If
                Catch ex As Exception
                    errors(f) = ex.Message
                End Try
            Next
        End If
        _profiles = profiles
        _errors = errors
        _loaded = True
    End Sub

    Public Shared Function Find(ByVal name As String) As CharacterProfile
        Return Profiles.FirstOrDefault(Function(p) p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
    End Function

#Region "Escolha por DataServer"

    Private Shared ReadOnly Property SelectionPath As String
        Get
            Return Path.Combine(AppState.OutputDir, SelectionFileName)
        End Get
    End Property

    Private Shared Function ReadSelections() As Dictionary(Of String, String)
        Dim result As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        If Not File.Exists(SelectionPath) Then Return result
        For Each line In File.ReadAllLines(SelectionPath)
            Dim bar As Integer = line.LastIndexOf("|"c)
            If bar > 0 Then result(line.Substring(0, bar).Trim()) = line.Substring(bar + 1).Trim()
        Next
        Return result
    End Function

    ''' <summary>Perfil escolhido para a pasta, ou AutoDetect.</summary>
    Public Shared Function SelectionFor(ByVal dataServerPath As String) As String
        Dim name As String = Nothing
        Return If(ReadSelections().TryGetValue(NormalizePath(dataServerPath), name), name, AutoDetect)
    End Function

    Public Shared Sub SaveSelection(ByVal dataServerPath As String, ByVal profileName As String)
        Dim selections = ReadSelections()
        Dim key As String = NormalizePath(dataServerPath)
        If profileName = AutoDetect Then
            If Not selections.Remove(key) Then Return
        Else
            Dim current As String = Nothing
            If selections.TryGetValue(key, current) AndAlso current = profileName Then Return
            selections(key) = profileName
        End If
        Directory.CreateDirectory(AppState.OutputDir)
        File.WriteAllLines(SelectionPath, selections.Select(Function(p) p.Key & "|" & p.Value))
    End Sub

    Private Shared Function NormalizePath(ByVal folder As String) As String
        Return folder.Trim().TrimEnd("\"c)
    End Function

#End Region

End Class
