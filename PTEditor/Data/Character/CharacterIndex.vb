Imports System.IO

''' <summary>Resumo de um personagem para listas e filtros (o arquivo completo e lido so ao abrir).</summary>
Friend Class CharacterEntry
    Public Property Name As String = ""
    Public Property FilePath As String = ""
    ''' <summary>Conta pelo userinfo; se nenhum userinfo lista o personagem, a conta gravada no proprio arquivo.</summary>
    Public Property Account As String = ""
    Public Property MasterId As String = ""
    Public Property JobCode As Integer
    Public Property Level As Integer
    Public Property Exp As Long
    Public Property LayoutName As String = ""
    ''' <summary>Motivo quando o arquivo nao pode ser lido; vazio se ok.</summary>
    Public Property LoadError As String = ""

    Public ReadOnly Property ClassName As String
        Get
            Return If(LoadError = "", GameTables.ClassName(JobCode), "?")
        End Get
    End Property
End Class

''' <summary>Onde um item foi encontrado na busca global.</summary>
Friend Class ItemLocation
    Public Property Owner As String = ""
    Public Property Account As String = ""
    Public Property FilePath As String = ""
    Public Property Item As GameItem
End Class

''' <summary>
''' Varre DataServer\userdata, userinfo e warehouse. Arquivos ilegiveis entram na lista com LoadError preenchido,
''' sem interromper a carga dos demais.
''' </summary>
Friend Class CharacterIndex

    Friend Const UserDataFolder As String = "userdata"
    Friend Const UserInfoFolder As String = "userinfo"
    Friend Const WarehouseFolder As String = "warehouse"
    Friend Const NoAccount As String = "(no account)"

    Public ReadOnly Property DataServerPath As String
    Public ReadOnly Property Characters As New List(Of CharacterEntry)()
    Public ReadOnly Property Accounts As New Dictionary(Of String, AccountFile)(StringComparer.OrdinalIgnoreCase)
    ''' <summary>Arquivos de conta ilegiveis (caminho -> motivo).</summary>
    Public ReadOnly Property AccountErrors As New Dictionary(Of String, String)()

    Private Sub New(ByVal dataServerPath As String)
        Me.DataServerPath = dataServerPath
    End Sub

    ''' <summary>True se a pasta parece um DataServer (tem userdata).</summary>
    Public Shared Function IsDataServer(ByVal folder As String) As Boolean
        Return folder <> "" AndAlso Directory.Exists(Path.Combine(folder, UserDataFolder))
    End Function

    Public Shared Function Build(ByVal dataServerPath As String, Optional ByVal progress As Action(Of Integer, Integer) = Nothing) As CharacterIndex
        Dim index As New CharacterIndex(dataServerPath)
        index.LoadAccounts()
        Dim owners As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        For Each account In index.Accounts.Values
            For Each charName In account.CharacterNames
                If Not owners.ContainsKey(charName) Then owners(charName) = account.Id
            Next
        Next

        Dim files = HashedFiles(Path.Combine(dataServerPath, UserDataFolder), "*.dat")
        For i = 0 To files.Count - 1
            index.Characters.Add(ReadEntry(files(i), owners))
            If progress IsNot Nothing Then progress(i + 1, files.Count)
        Next
        index.Characters.Sort(Function(a, b) String.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase))
        Return index
    End Function

    Private Sub LoadAccounts()
        For Each f In HashedFiles(Path.Combine(DataServerPath, UserInfoFolder), "*.dat")
            Try
                Dim account = AccountFile.Load(f)
                If Not Accounts.ContainsKey(account.Id) Then Accounts(account.Id) = account
            Catch ex As Exception
                AccountErrors(f) = ex.Message
            End Try
        Next
    End Sub

    ''' <summary>Arquivos em &lt;pasta&gt;\0..255\ (as subpastas de hash do servidor; outras pastas, como Backup, sao ignoradas).</summary>
    Private Shared Function HashedFiles(ByVal folder As String, ByVal pattern As String) As List(Of String)
        Dim result As New List(Of String)()
        If Not Directory.Exists(folder) Then Return result
        For Each subFolder In Directory.GetDirectories(folder)
            Dim number As Integer
            If Integer.TryParse(Path.GetFileName(subFolder), number) AndAlso number >= 0 AndAlso number <= 255 Then
                result.AddRange(Directory.GetFiles(subFolder, pattern))
            End If
        Next
        Return result
    End Function

    Private Shared Function ReadEntry(ByVal filePath As String, ByVal owners As Dictionary(Of String, String)) As CharacterEntry
        Dim entry As New CharacterEntry With {.Name = Path.GetFileNameWithoutExtension(filePath), .FilePath = filePath}
        Try
            Dim c = CharacterFile.Load(filePath)
            entry.JobCode = c.JobCode
            entry.Level = c.Level
            entry.Exp = c.Exp
            entry.MasterId = c.MasterId
            entry.LayoutName = c.Layout.Name
        Catch ex As Exception
            entry.LoadError = ex.Message
        End Try
        Dim owner As String = Nothing
        If owners.TryGetValue(entry.Name, owner) Then
            entry.Account = owner
        ElseIf entry.MasterId <> "" Then
            entry.Account = entry.MasterId
        Else
            entry.Account = NoAccount
        End If
        Return entry
    End Function

    ''' <summary>Caminho do bau da conta: warehouse\&lt;code(conta)&gt;\&lt;conta&gt;.war; se nao existir, procura pelo nome nas subpastas.</summary>
    Public Function WarehousePath(ByVal account As String) As String
        Dim folder As String = Path.Combine(DataServerPath, WarehouseFolder)
        Dim expected As String = Path.Combine(folder, UserCode.Compute(account).ToString(), account & ".war")
        If File.Exists(expected) Then Return expected
        Dim wanted As String = account & ".war"
        For Each f In HashedFiles(folder, "*.war")
            If Path.GetFileName(f).Equals(wanted, StringComparison.OrdinalIgnoreCase) Then Return f
        Next
        Return Nothing
    End Function

    ''' <summary>Todos os baus (conta -> caminho) presentes no DataServer.</summary>
    Public Function WarehouseFiles() As Dictionary(Of String, String)
        Dim result As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        For Each f In HashedFiles(Path.Combine(DataServerPath, WarehouseFolder), "*.war")
            Dim account As String = Path.GetFileNameWithoutExtension(f)
            If Not result.ContainsKey(account) Then result(account) = f
        Next
        Return result
    End Function

    ''' <summary>
    ''' Le todos os itens de personagens e (opcionalmente) baus. errors recebe os arquivos que nao puderam ser lidos.
    ''' </summary>
    Public Function ScanItems(ByVal includeWarehouses As Boolean, ByVal errors As List(Of String),
                              Optional ByVal progress As Action(Of Integer, Integer) = Nothing) As List(Of ItemLocation)
        Dim result As New List(Of ItemLocation)()
        Dim warehouses = If(includeWarehouses, WarehouseFiles(), New Dictionary(Of String, String)())
        Dim total As Integer = Characters.Count + warehouses.Count
        Dim done As Integer = 0
        For Each entry In Characters
            done += 1
            If entry.LoadError <> "" Then
                errors.Add(entry.FilePath & ": " & entry.LoadError)
                Continue For
            End If
            Try
                For Each item In CharacterFile.Load(entry.FilePath).Items
                    If item.IsTwoHandedCopy Then Continue For
                    result.Add(New ItemLocation With {.Owner = entry.Name, .Account = entry.Account, .FilePath = entry.FilePath, .Item = item})
                Next
            Catch ex As Exception
                errors.Add(entry.FilePath & ": " & ex.Message)
            End Try
            If progress IsNot Nothing Then progress(done, total)
        Next
        For Each pair In warehouses
            done += 1
            Try
                For Each item In WarehouseFile.Load(pair.Value).Items
                    result.Add(New ItemLocation With {.Owner = "Warehouse of " & pair.Key, .Account = pair.Key, .FilePath = pair.Value, .Item = item})
                Next
            Catch ex As Exception
                errors.Add(pair.Value & ": " & ex.Message)
            End Try
            If progress IsNot Nothing Then progress(done, total)
        Next
        Return result
    End Function

End Class
