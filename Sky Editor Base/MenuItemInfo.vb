''' <summary>
''' Contains all the information needed to make a MenuItem that runs one or more MenuAction.
''' </summary>
Public Class MenuItemInfo
    Public Property Header As String
    Public Property ActionTypes As List(Of Type)
    Public Property Children As List(Of MenuItemInfo)
    Public Sub New()
        ActionTypes = New List(Of Type)
        Children = New List(Of MenuItemInfo)
    End Sub
End Class
