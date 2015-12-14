Public Class GameCodeRegistry
    Public Shared Property Registry As Dictionary(Of String, String)
    Public Shared Property GameTypes As Dictionary(Of String, Type)
    Public Shared Sub RegisterGameCode(GameTitle As String, GameCode As String, GameType As Type)
        Registry.Add(GameTitle, GameCode)
        GameTypes.Add(GameCode, GameType)
    End Sub
    Shared Sub New()
        Registry = New Dictionary(Of String, String)
        GameTypes = New Dictionary(Of String, Type)
    End Sub
End Class
