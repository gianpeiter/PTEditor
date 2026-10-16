Imports System.Globalization
Imports System.IO

''' <summary>
''' Tabela de EXP de um perfil (ExpLevelTable do servidor). Entrada N-1 = EXP no inicio do level N.
''' O arquivo aceita o formato do LevelTable.h (numeros separados por virgula, comentarios // ou ;).
''' Mesma logica de CheckLevelExp / GetLevelFromExp em playsub.cpp.
''' </summary>
Friend NotInheritable Class LevelTable

    Private ReadOnly _values As Long()

    ''' <summary>Ultimo level alcancavel: CHAR_LEVEL_MAX do servidor, limitado pela tabela (precisa do inicio do level seguinte).</summary>
    Public ReadOnly Property MaxLevel As Integer

    Public ReadOnly Property FilePath As String

    Private Sub New(ByVal filePath As String, ByVal values As Long(), ByVal maxLevel As Integer)
        Me.FilePath = filePath
        _values = values
        Me.MaxLevel = Math.Min(maxLevel, values.Length - 1)
    End Sub

    Public Shared Function Load(ByVal filePath As String, ByVal maxLevel As Integer) As LevelTable
        If Not File.Exists(filePath) Then Throw New InvalidDataException("Level table not found: " & filePath)
        Dim values As New List(Of Long)()
        For Each raw In File.ReadAllLines(filePath)
            Dim line As String = raw
            Dim comment As Integer = line.IndexOf("//", StringComparison.Ordinal)
            If comment >= 0 Then line = line.Substring(0, comment)
            If line.TrimStart().StartsWith(";") Then Continue For
            For Each token In line.Split({","c, " "c, ControlChars.Tab}, StringSplitOptions.RemoveEmptyEntries)
                Dim value As Long
                If Not Long.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, value) Then
                    Throw New InvalidDataException("Invalid value in level table " & Path.GetFileName(filePath) & ": " & token)
                End If
                values.Add(value)
            Next
        Next
        ' O LevelTable.h do servidor termina com -1 (incluido no ExpLevelTable[]): nao e um level.
        If values.Count > 0 AndAlso values(values.Count - 1) = -1 Then values.RemoveAt(values.Count - 1)
        If values.Count < 2 Then Throw New InvalidDataException("Level table has too few values: " & filePath)
        Return New LevelTable(filePath, values.ToArray(), maxLevel)
    End Function

    ''' <summary>EXP no inicio do level (1..MaxLevel).</summary>
    Public Function ExpAtLevelStart(ByVal level As Integer) As Long
        Return _values(Math.Max(1, Math.Min(level, MaxLevel)) - 1)
    End Function

    ''' <summary>Maior EXP aceita no level (limite superior inclusivo, como em CheckLevelExp).</summary>
    Public Function ExpAtLevelEnd(ByVal level As Integer) As Long
        Return _values(Math.Max(1, Math.Min(level, MaxLevel)))
    End Function

    ''' <summary>CheckLevelExp: True se a EXP pertence ao level.</summary>
    Public Function IsValid(ByVal level As Integer, ByVal exp As Long) As Boolean
        If level < 1 OrElse level > MaxLevel Then Return False
        Return exp >= _values(level - 1) AndAlso exp <= _values(level)
    End Function

    ''' <summary>GetLevelFromExp: level que o servidor atribui a essa EXP (0 se nenhum).</summary>
    Public Function LevelFromExp(ByVal exp As Long) As Integer
        For i = 0 To MaxLevel - 1
            If exp >= _values(i) AndAlso exp < _values(i + 1) Then Return i + 1
        Next
        Return 0
    End Function

End Class
