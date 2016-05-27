Imports System.ComponentModel
Imports System.Windows.Controls
Imports SkyEditor.Core
Imports SkyEditor.Core.UI

Public Class DataboundToolWindow
    Inherits UserControl
    Implements ITargetedControl
    Implements INotifyPropertyChanged
    Public Property CurrentPluginManager As PluginManager

    Public Property Header As String Implements ITargetedControl.Header
        Get
            Return _header
        End Get
        Set(value As String)
            If Not _header = value Then
                Dim old = _header
                _header = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Header)))
                RaiseEvent HeaderChanged(Me, New HeaderUpdatedEventArgs(old, value))
            End If
        End Set
    End Property
    Dim _header As String

    Private Property ITargetedControl_IsVisible As Boolean Implements ITargetedControl.IsVisible
        Get
            Return _isVisible
        End Get
        Set(value As Boolean)
            If Not _isVisible = value Then
                _isVisible = value
                RaiseEvent VisibilityChanged(Me, New VisibilityUpdatedEventArgs())
            End If
        End Set
    End Property
    Dim _isVisible As Boolean

    Public Event HeaderChanged As ITargetedControl.HeaderChangedEventHandler Implements ITargetedControl.HeaderChanged
    Public Event VisibilityChanged As ITargetedControl.VisibilityChangedEventHandler Implements ITargetedControl.VisibilityChanged
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub SetPluginManager(manager As PluginManager) Implements ITargetedControl.SetPluginManager
        CurrentPluginManager = manager
    End Sub

    Public Sub UpdateTargets(Targets As IEnumerable(Of Object)) Implements ITargetedControl.UpdateTargets
        Throw New NotImplementedException()
    End Sub

    Public Function GetDefaultPane() As ITargetedControl.Pane Implements ITargetedControl.GetDefaultPane
        Throw New NotImplementedException()
    End Function

    Public Function GetStartCollapsed() As Boolean Implements ITargetedControl.GetStartCollapsed
        Throw New NotImplementedException()
    End Function
End Class
