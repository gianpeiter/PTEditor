Imports System.Data
Imports System.Data.SqlClient

Friend Enum ClanRole
    Member
    SubChief
    Leader
End Enum

''' <summary>
''' Clan do personagem em [ClanDB] (UL = membros, CL = clans), com o mesmo criterio de ClanService::IsClanMember
''' (Servidor\Gameplay\Social\ClanService.cpp): lider = CL.ClanZang, subchefe = UL.Permi '2'. O cliente consulta isso
''' antes de excluir um personagem e bloqueia a exclusao de membro, subchefe e lider (HoLogin.cpp).
''' </summary>
Friend NotInheritable Class ClanMembership

    Friend Const DatabaseName As String = "ClanDB"

    Public Property ClanName As String = ""
    Public Property Role As ClanRole
    Public Property MemberCount As Integer

    ''' <summary>Clan do personagem; Nothing se nao estiver em nenhum. Lanca excecao se nao conseguir consultar.</summary>
    Public Shared Function Lookup(ByVal config As DatabaseConfig, ByVal character As String) As ClanMembership
        Using connection As New SqlConnection(config.ConnectionString(DatabaseName))
            connection.Open()
            Using command As New SqlCommand(
                "SELECT TOP 1 c.ClanName, c.ClanZang, u.Permi, c.MemCnt FROM [dbo].[UL] u " &
                "INNER JOIN [dbo].[CL] c ON c.ClanName = u.ClanName WHERE u.ChName = @name", connection)
                command.Parameters.Add("@name", SqlDbType.VarChar, 32).Value = character
                Using reader = command.ExecuteReader()
                    If Not reader.Read() Then Return Nothing
                    Dim leader As String = If(reader.IsDBNull(1), "", reader.GetValue(1).ToString().Trim())
                    Dim permission As String = If(reader.IsDBNull(2), "", reader.GetValue(2).ToString().Trim())
                    Dim role As ClanRole = If(leader.Equals(character, StringComparison.OrdinalIgnoreCase), ClanRole.Leader,
                                              If(permission = "2", ClanRole.SubChief, ClanRole.Member))
                    Return New ClanMembership With {
                        .ClanName = reader.GetValue(0).ToString().Trim(),
                        .Role = role,
                        .MemberCount = If(reader.IsDBNull(3), 0, Convert.ToInt32(reader.GetValue(3)))}
                End Using
            End Using
        End Using
    End Function

    ''' <summary>Por que o jogo nao deixa excluir e o que fazer antes.</summary>
    Public ReadOnly Property BlockReason As String
        Get
            Select Case Role
                Case ClanRole.Leader
                    Return "is the LEADER of clan " & ClanName & " (" & MemberCount & " members)." & vbCrLf & vbCrLf &
                           "The game does not delete clan leaders. In the game, pass the leadership to another member " &
                           "or dissolve the clan first."
                Case ClanRole.SubChief
                    Return "is the SUB-CHIEF of clan " & ClanName & "." & vbCrLf & vbCrLf &
                           "The game does not delete clan members. In the game, leave the clan (or have the leader " &
                           "remove the character) first."
                Case Else
                    Return "is a member of clan " & ClanName & "." & vbCrLf & vbCrLf &
                           "The game does not delete clan members. In the game, leave the clan (or have the leader " &
                           "remove the character) first."
            End Select
        End Get
    End Property

End Class
