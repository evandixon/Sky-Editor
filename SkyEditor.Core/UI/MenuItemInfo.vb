Imports System.Reflection

Namespace UI
    ''' <summary>
    ''' Contains all the information needed to make a MenuItem that runs one or more MenuAction.
    ''' </summary>
    Public Class MenuItemInfo
        Public Property Header As String
        Public Property ActionTypes As List(Of TypeInfo)
        Public Property Children As List(Of MenuItemInfo)
        Public Property SortOrder As Decimal
        Public Sub New()
            ActionTypes = New List(Of TypeInfo)
            Children = New List(Of MenuItemInfo)
            SortOrder = 0
        End Sub
    End Class

End Namespace
