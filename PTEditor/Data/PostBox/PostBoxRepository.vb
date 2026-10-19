Imports System.IO
Imports System.Text

''' <summary>Uma linha de PostBox\&lt;code&gt;\&lt;conta&gt;.dat (item esperando ser retirado no NPC com *경품배달).</summary>
Friend Class PostBoxEntry
    ''' <summary>Personagem que pode receber, ou "*" (qualquer personagem da conta).</summary>
    Public Property Character As String = ""
    Public Property Code As String = ""
    ''' <summary>Equipamento: codigo da classe (0 = aleatoria). Pocao: quantidade. MONEY/EXP: valor.</summary>
    Public Property Spec As Integer
    Public Property Description As String = ""
    ''' <summary>Linha tem senha (o jogador precisa digita-la no NPC).</summary>
    Public Property HasPassword As Boolean
    ''' <summary>Texto original da linha; usado para remover sem reescrever as demais.</summary>
    Public Property RawLine As String = ""
End Class

''' <summary>
''' Caixa do NPC com *경품배달 (GiftExpress): Servidor\record.cpp rsLoadPostBox/rsSavePostBox e
''' OnSever.cpp SendOpenGiftExpress. Um arquivo-texto por conta, relido a cada clique no NPC.
''' </summary>
Friend NotInheritable Class PostBoxRepository

    Friend Const FolderName As String = "PostBox"
    ''' <summary>POST_ITEM_MAX (Shared\smPacket.h): linhas alem disso sao ignoradas pelo servidor.</summary>
    Friend Const MaxLines As Integer = 500
    ''' <summary>szCharName/szItemCode[32]: 31 caracteres + terminador.</summary>
    Friend Const MaxTextLength As Integer = 31
    ''' <summary>szDoc[128] (texto da caixa de confirmacao).</summary>
    Friend Const MaxDescriptionLength As Integer = 127

    Private Sub New()
    End Sub

    Friend Shared ReadOnly Property Folder As String
        Get
            Return Path.Combine(AppState.ServerPath, FolderName)
        End Get
    End Property

    ''' <summary>
    ''' Caminho como GetPostBoxFile: PostBox\&lt;GetUserCode(conta)&gt;\&lt;conta&gt;.dat, com o prefixo "＃" (bytes A3 A3) nos
    ''' nomes que o servidor trata como reservados. A condicao do CON nao checa o tamanho (precedencia de &amp;&amp;/|| no
    ''' original), entao toda conta que comeca com "con" usa o prefixo; replicado para cair no mesmo arquivo.
    ''' </summary>
    Friend Shared Function FilePath(ByVal account As String) As String
        Dim prefix As String = If(UsesReservedPrefix(account), Encoding.Default.GetString({&HA3, &HA3}), "")
        Return Path.Combine(Folder, UserCode.Compute(account).ToString(), prefix & account & ".dat")
    End Function

    Private Shared Function UsesReservedPrefix(ByVal id As String) As Boolean
        Dim u As String = id.ToUpperInvariant()
        If u.Length = 4 AndAlso Char.IsDigit(u(3)) AndAlso (u.StartsWith("COM") OrElse u.StartsWith("LPT")) Then Return True
        If u = "PRN" Then Return True
        Return u.StartsWith("CON")
    End Function

    ''' <summary>Itens pendentes da conta, na ordem do arquivo. Sem arquivo = nenhum.</summary>
    Public Shared Function ListPending(ByVal account As String) As List(Of PostBoxEntry)
        Dim result As New List(Of PostBoxEntry)()
        Dim file As String = FilePath(account)
        If Not IO.File.Exists(file) Then Return result
        For Each line In IO.File.ReadAllLines(file, AppState.FileEncoding)
            If line = "" Then Continue For
            result.Add(ParseLine(line))
        Next
        Return result
    End Function

    ''' <summary>Campos como rsLoadPostBox: separados por espaco/tab; campo que comeca com aspas vai ate a proxima aspa.</summary>
    Private Shared Function ParseLine(ByVal line As String) As PostBoxEntry
        Dim fields As New List(Of String)()
        Dim i As Integer = 0
        Do While fields.Count < 6
            Do While i < line.Length AndAlso Char.IsWhiteSpace(line(i))
                i += 1
            Loop
            If i >= line.Length Then Exit Do
            Dim start As Integer
            If line(i) = """"c Then
                start = i + 1
                Dim close As Integer = line.IndexOf(""""c, start)
                If close < 0 Then close = line.Length
                fields.Add(line.Substring(start, close - start))
                i = close + 1
            Else
                start = i
                Do While i < line.Length AndAlso Not Char.IsWhiteSpace(line(i))
                    i += 1
                Loop
                fields.Add(line.Substring(start, i - start))
            End If
        Loop
        Dim spec As Integer
        Integer.TryParse(Field(fields, 2), spec)
        Return New PostBoxEntry With {
            .Character = Field(fields, 0), .Code = Field(fields, 1), .Spec = spec,
            .Description = Field(fields, 3), .HasPassword = Field(fields, 5) <> "", .RawLine = line}
    End Function

    Private Shared Function Field(ByVal fields As List(Of String), ByVal index As Integer) As String
        Return If(index < fields.Count, fields(index), "")
    End Function

    ''' <summary>Acrescenta as linhas no formato de rsAddPostBoxItem (sem senha). Falha se passar de MaxLines.</summary>
    Public Shared Sub Add(ByVal account As String, ByVal items As IList(Of PostBoxEntry))
        Dim file As String = FilePath(account)
        Dim existing As Integer = ListPending(account).Count
        If existing + items.Count > MaxLines Then
            Throw New InvalidOperationException("The PostBox of " & account & " already has " & existing & " line(s); the server reads at most " & MaxLines & ".")
        End If
        Dim sb As New StringBuilder()
        If IO.File.Exists(file) AndAlso Not EndsWithNewline(file) Then sb.Append(vbCrLf)
        For Each item In items
            sb.Append(item.Character & vbTab & vbTab & item.Code & vbTab & vbTab & item.Spec & vbTab & vbTab &
                      """" & item.Description & """" & vbCrLf)
        Next
        Directory.CreateDirectory(Path.GetDirectoryName(file))
        IO.File.AppendAllText(file, sb.ToString(), AppState.FileEncoding)
    End Sub

    Private Shared Function EndsWithNewline(ByVal file As String) As Boolean
        Using stream = IO.File.OpenRead(file)
            If stream.Length = 0 Then Return True
            stream.Seek(-1, SeekOrigin.End)
            Return stream.ReadByte() = 10
        End Using
    End Function

    ''' <summary>Remove as linhas (pelo texto original) e regrava o resto como estava. Retorna quantas ainda existiam.</summary>
    Public Shared Function Remove(ByVal account As String, ByVal entries As IEnumerable(Of PostBoxEntry)) As Integer
        Dim file As String = FilePath(account)
        If Not IO.File.Exists(file) Then Return 0
        Dim lines = IO.File.ReadAllLines(file, AppState.FileEncoding).Where(Function(l) l <> "").ToList()
        Dim removed As Integer = 0
        For Each entry In entries
            Dim index As Integer = lines.IndexOf(entry.RawLine)
            If index >= 0 Then
                lines.RemoveAt(index)
                removed += 1
            End If
        Next
        If lines.Count = 0 Then
            IO.File.Delete(file)
        Else
            IO.File.WriteAllText(file, String.Join(vbCrLf, lines) & vbCrLf, AppState.FileEncoding)
        End If
        Return removed
    End Function

    ''' <summary>Problema que impede enviar o codigo, ou "" se ok.</summary>
    Public Shared Function ValidateCode(ByVal code As String) As String
        If code = "" Then Return "Empty item code."
        If code.Length > MaxTextLength Then Return "Code " & code & " is longer than " & MaxTextLength & " characters."
        Dim item = ItemData.FindByCode("""" & code.ToUpperInvariant() & """")
        If item Is Nothing Then Return "Code " & code & " does not exist in OpenItem (the NPC would never offer it)."
        If IsUnique(item) Then Return "Code " & code & " is a unique (Quest) item: the NPC never offers unique items."
        Return ""
    End Function

    ''' <summary>*유니크 preenchido: o servidor pula o item ao oferecer e ao entregar.</summary>
    Friend Shared Function IsUnique(ByVal item As ItemFile) As Boolean
        Dim line = item.Lines.FirstOrDefault(Function(l) l.StartsWith(GameFields.Item.Unique) AndAlso
                                                         Not l.StartsWith(GameFields.Item.UniqueColor))
        If line Is Nothing Then Return False
        Dim value As String = ItemData.GetValue(line, GameFields.Item.Unique)
        Return value <> "" AndAlso value <> "0"
    End Function

    ''' <summary>Pocoes (PL/PM/PS): o Spec vira a quantidade entregue.</summary>
    Friend Shared Function IsPotion(ByVal code As String) As Boolean
        Dim u As String = code.ToUpperInvariant()
        Return u.Length > 2 AndAlso u(0) = "P"c AndAlso (u(1) = "L"c OrElse u(1) = "M"c OrElse u(1) = "S"c)
    End Function

    ''' <summary>Texto da caixa de confirmacao: sem aspas nem quebras (o servidor le ate a proxima aspa).</summary>
    Friend Shared Function CleanDescription(ByVal text As String) As String
        Dim clean As String = text.Replace("""", "'").Replace(vbCr, " ").Replace(vbLf, " ").Replace(vbTab, " ").Trim()
        Return If(clean.Length > MaxDescriptionLength, clean.Substring(0, MaxDescriptionLength), clean)
    End Function

End Class

''' <summary>Registro local dos envios (&lt;exe&gt;\PTEditor\Logs\PostBox.log).</summary>
Friend NotInheritable Class PostBoxLog

    Private Sub New()
    End Sub

    Friend Shared ReadOnly Property FilePath As String
        Get
            Return Path.Combine(AppState.LogsDir, "PostBox.log")
        End Get
    End Property

    Friend Shared Sub Write(ByVal action As String, ByVal account As String, ByVal items As IEnumerable(Of PostBoxEntry))
        Try
            Dim stamp As String = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            File.AppendAllLines(FilePath, items.Select(Function(i) stamp & vbTab & action & vbTab & account & vbTab & i.Character & vbTab &
                                                                   i.Code & vbTab & i.Spec & vbTab & i.Description))
        Catch ex As Exception
            ErrorLog.Write("PostBoxLog.Write: " & ex.Message)
        End Try
    End Sub

End Class
