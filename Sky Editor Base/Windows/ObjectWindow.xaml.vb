Imports System.Threading.Tasks

Public Class ObjectWindow
    Dim _objectToEdit As Object
    Dim _manager As PluginManager
    Public Property ObjectToEdit As Object
        Get
            UpdateSave()
            Return _objectToEdit
        End Get
        Set(value As Object)
            _objectToEdit = value
            RefreshDisplay()
        End Set
    End Property
    Public Sub RefreshDisplay()
        tcTabs.Items.Clear()
        For Each item In _manager.GetRefreshedTabs(_objectToEdit)
            tcTabs.Items.Add(item)
        Next
    End Sub
    Public Sub UpdateSave()
        For Each item In (From t In tcTabs.Items Order By DirectCast(t.Content, ObjectTab).UpdatePriority Descending)
            DirectCast(item.Content, ObjectTab).UpdateObject()
        Next
    End Sub
    Public Sub New(Manager As PluginManager)
        InitializeComponent()
        _manager = Manager
    End Sub
    Public ReadOnly Property TabCount As Integer
        Get
            Return tcTabs.Items.Count
        End Get
    End Property
End Class