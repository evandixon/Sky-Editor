Public Class GameTypeRegistry
    Public Class GameType
        Public Property GameID As String
        Public Property Checker As IsSaveOfType
        Public Delegate Function IsSaveOfType(GameID As String, Save As Byte()) As Boolean
        Public Sub New(GameID As String, SaveClass As IsSaveOfType)
            Me.GameID = GameID
            Me.Checker = SaveClass
        End Sub
    End Class
    Public Property GameTypes As New List(Of GameType)
    Private Shared reg As GameTypeRegistry
    Private Sub New()

    End Sub
    Public Shared Property Registry As GameTypeRegistry
        Get
            If reg Is Nothing Then
                reg = New GameTypeRegistry
            End If
            Return reg
        End Get
        Set(value As GameTypeRegistry)
            reg = value
        End Set
    End Property
    Public Function GetCheckerMethod(GameID As String) As GameType.IsSaveOfType
        Dim out = Nothing
        For Each item In Registry.GameTypes
            If item.GameID = GameID Then
                out = item.Checker
                Exit For
            End If
        Next
        Return out
    End Function
End Class
