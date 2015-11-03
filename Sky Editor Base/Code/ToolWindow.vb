Imports Xceed.Wpf.AvalonDock.Layout

Public Class ToolWindow
    Inherits UserControl
    Dim _header As String
    Public Property Title As String
        Get
            If ParentAnchorable Is Nothing Then
                Return _header
            Else
                Return ParentAnchorable.Title
            End If
        End Get
        Set(value As String)
            If ParentAnchorable Is Nothing Then
                _header = value
            Else
                ParentAnchorable.Title = value
            End If
        End Set
    End Property

    Dim _parentAnchorable As LayoutAnchorable
    Public Property ParentAnchorable As LayoutAnchorable
        Get
            If _parentAnchorable Is Nothing Then
                Dim a As New LayoutAnchorable
                Me.Margin = New Thickness(0, 0, 0, 0)
                a.Content = Me
                Me.ParentAnchorable = a
            End If
            Return _parentAnchorable
        End Get
        Set(value As LayoutAnchorable)
            _parentAnchorable = value
            _parentAnchorable.Title = _header
        End Set
    End Property
    Public Shadows Property Visibility As Visibility
        Get
            Return MyBase.Visibility
        End Get
        Set(value As Visibility)
            ParentAnchorable.IsVisible = (value = Visibility.Visible)
            MyBase.Visibility = value
        End Set
    End Property

End Class
