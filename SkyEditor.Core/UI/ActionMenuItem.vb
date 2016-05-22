Imports System.Windows.Input
Imports SkyEditor.Core.UI

Public Class ActionMenuItem
    Public Property CurrentIOUIManager As IOUIManager
    Public Property Header As String
    Public Property Actions As List(Of MenuAction)
    Public Property IsVisible As Boolean
    Public Property Children As ObservableCollection(Of ActionMenuItem)
    Public Property Command As ICommand

    Private Async Sub RunActions()
        Dim tasks As New List(Of Task)
        For Each t In Actions
            tasks.Add(t.DoAction(CurrentIOUIManager.GetMenuActionTargets(t)))
        Next
        Await Task.WhenAll(tasks)
    End Sub

    Public Sub New()
        Me.Actions = New List(Of MenuAction)
        Me.Children = New ObservableCollection(Of ActionMenuItem)
        Command = New RelayCommand(AddressOf RunActions)
    End Sub
End Class
