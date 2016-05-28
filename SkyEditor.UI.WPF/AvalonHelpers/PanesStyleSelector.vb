Imports System.Windows
Imports System.Windows.Controls
Imports SkyEditor.Core.UI

Namespace AvalonHelpers
    Public Class PanesStyleSelector
        Inherits StyleSelector

        Public Property DocumentStyle As Style

        Public Property AnchorableStyle As Style

        Public Overrides Function SelectStyle(item As Object, container As DependencyObject) As Style
            If TypeOf item Is AvalonDockFileWrapper Then
                Return DocumentStyle
            ElseIf TypeOf item Is AnchorableViewModel Then
                Return AnchorableStyle
            Else
                Return MyBase.SelectStyle(item, container)
            End If
        End Function
    End Class
End Namespace

