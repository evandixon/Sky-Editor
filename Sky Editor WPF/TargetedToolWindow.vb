Imports SkyEditorBase.EventArguments
Imports SkyEditorBase.Interfaces
Imports Xceed.Wpf.AvalonDock.Layout

Public Class TargetedToolWindow
    Inherits LayoutAnchorable

    Public Property ContainedControl As ITargetedControl
        Get
            Return _containedControl
        End Get
        Set(value As ITargetedControl)
            _containedControl = value
            Me.Title = value.Header
            Me.IsVisible = value.IsVisible
            Me.Content = value
        End Set
    End Property
    Private WithEvents _containedControl As ITargetedControl

    Private Sub _containedControl_HeaderChanged(sender As Object, e As HeaderUpdatedEventArgs) Handles _containedControl.HeaderChanged
        Me.Title = e.NewValue
    End Sub

    Private Sub _containedControl_VisibilityChanged(sender As Object, e As VisibilityUpdatedEventArgs) Handles _containedControl.VisibilityChanged
        Me.IsVisible = e.IsVisible
    End Sub
End Class
