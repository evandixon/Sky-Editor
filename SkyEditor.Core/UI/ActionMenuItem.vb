Imports System.Windows.Input
Imports SkyEditor.Core.UI

Public Class ActionMenuItem
    Implements INotifyPropertyChanged

    Public Sub New()
        Me.Actions = New List(Of MenuAction)
        Me.Children = New ObservableCollection(Of ActionMenuItem)
        Command = New RelayCommand(AddressOf RunActions)
    End Sub

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property CurrentIOUIManager As IOUIManager
    Public Property Header As String
        Get
            Return _header
        End Get
        Set(value As String)
            If Not _header = value Then
                _header = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Header)))
            End If
        End Set
    End Property
    Dim _header As String

    Public Property Actions As List(Of MenuAction)

    Public Property IsVisible As Boolean
        Get
            Return _isVisible
        End Get
        Set(value As Boolean)
            If Not _isVisible = value Then
                _isVisible = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(IsVisible)))
            End If
        End Set
    End Property
    Dim _isVisible As Boolean

    Public Property Children As ObservableCollection(Of ActionMenuItem)

    Public Property Command As ICommand

    Public Property ContextTargets As IEnumerable(Of Object)

    Private Async Sub RunActions()
        'Dim tasks As New List(Of Task) 
        For Each t In Actions
            Await t.DoAction(GetTargets(t))
        Next
    End Sub

    Private Function GetTargets(t As MenuAction) As IEnumerable(Of Object)
        If ContextTargets IsNot Nothing Then
            Return ContextTargets
        Else
            Return CurrentIOUIManager.GetMenuActionTargets(t)
        End If
    End Function

End Class
