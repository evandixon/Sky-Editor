Public Class GameCodeRegistry
    Public Shared Property Registry As Dictionary(Of String, String)
    Public Shared Sub RegisterGameCode(GameTitle As String, GameCode As String)
        If Not Registry.ContainsKey(GameTitle) Then
            Registry.Add(GameTitle, GameCode)
        End If
    End Sub
    Shared Sub New()
        Registry = New Dictionary(Of String, String)
    End Sub
End Class
