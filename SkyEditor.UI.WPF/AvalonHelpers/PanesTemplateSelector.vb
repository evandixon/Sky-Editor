Imports System.Windows
Imports System.Windows.Controls
Imports SkyEditor.Core.UI

Namespace AvalonHelpers
    Public Class PanesTemplateSelector
        Inherits DataTemplateSelector

        Public Property DocumentTemplate As DataTemplate

        Public Property AnchorableTemplate As DataTemplate

        Public Overrides Function SelectTemplate(item As Object, container As DependencyObject) As DataTemplate
            If TypeOf item Is AvalonDockFileWrapper Then
                Return DocumentTemplate
            ElseIf TypeOf item Is AnchorableViewModel Then
                Return AnchorableTemplate
            Else
                Return MyBase.SelectTemplate(item, container)
            End If
        End Function
    End Class
End Namespace

