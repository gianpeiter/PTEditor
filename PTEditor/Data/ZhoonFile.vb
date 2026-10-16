Imports System.IO
Imports System.Text.RegularExpressions

''' <summary>
''' Arquivos .zhoon (nome e falas por idioma). As linhas sao "*X_NAME" / "*X_CHAT", onde X e a letra do idioma
''' compilado no servidor (Servidor\fileread.cpp: C, J, T, E, V, B, A). O PriTaTor original so lia "*J_";
''' aqui qualquer tag e aceita, com preferencia pela tag em AppState.NameTag.
''' </summary>
Friend Module ZhoonFile

    Friend Const DefaultTag As String = "J"

    Private ReadOnly TagPattern As New Regex("^\*([A-Za-z])_(NAME|CHAT)", RegexOptions.IgnoreCase Or RegexOptions.Compiled)
    Private ReadOnly TagCounts As New Dictionary(Of String, Integer)()

    ''' <summary>Tag mais frequente nos .zhoon lidos desde o ultimo ResetDetection; "" se nenhum.</summary>
    Friend ReadOnly Property DetectedTag As String
        Get
            If TagCounts.Count = 0 Then Return ""
            Return TagCounts.OrderByDescending(Function(p) p.Value).First().Key
        End Get
    End Property

    Friend Sub ResetDetection()
        TagCounts.Clear()
    End Sub

    Private Sub CountTag(ByVal tag As String)
        Dim n As Integer
        TagCounts.TryGetValue(tag, n)
        TagCounts(tag) = n + 1
    End Sub

    ''' <summary>Linhas do tipo pedido ("NAME" ou "CHAT"); a da tag preferida vem primeiro.</summary>
    Private Function Entries(ByVal path As String, ByVal kind As String) As List(Of String)
        Dim preferred As New List(Of String)()
        Dim others As New List(Of String)()
        For Each line In FileHelper.ReadLines(path)
            Dim m = TagPattern.Match(line)
            If Not m.Success Then Continue For
            Dim tag As String = m.Groups(1).Value.ToUpper()
            If m.Groups(2).Value.ToUpper() <> kind Then Continue For
            If kind = "NAME" Then CountTag(tag)
            If tag = AppState.NameTag.ToUpper() Then preferred.Add(line) Else others.Add(line)
        Next
        preferred.AddRange(others)
        Return preferred
    End Function

    ''' <summary>Valor apos "*X_NAME" (mantem aspas, como o original). Nothing se nao houver linha de nome.</summary>
    Friend Function ReadName(ByVal path As String) As String
        Dim lines = Entries(path, "NAME")
        If lines.Count = 0 Then Return Nothing
        Return lines(0).Substring(7).Trim()
    End Function

    ''' <summary>Falas "*X_CHAT" (sem tabs), na ordem do arquivo.</summary>
    Friend Function ReadChat(ByVal path As String) As List(Of String)
        Dim result As New List(Of String)()
        For Each line In Entries(path, "CHAT")
            result.Add(line.Substring(7).Replace(ChrW(9), "").Trim())
        Next
        Return result
    End Function

    Friend Function NameLine(ByVal name As String) As String
        Return "*" & AppState.NameTag & "_Name" & vbTab & """" & name & """"
    End Function

    Friend Function ChatLine(ByVal chat As String) As String
        Return "*" & AppState.NameTag & "_Chat" & vbTab & chat
    End Function

End Module
