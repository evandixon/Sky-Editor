Imports System.Collections.ObjectModel
Imports System.Windows
Imports SkyEditor.Core.Utilities


Public Class OverlappingStyleSelector
    Inherits Windows.Controls.StyleSelector

    Public Property Styles As ObservableCollection(Of Style)

    Public Overrides Function SelectStyle(item As Object, container As DependencyObject) As Style
        Dim output As New Style
        output.TargetType = item.GetType
        For Each selector In Styles
            If ReflectionHelpers.IsOfType(item, selector.TargetType) Then
                For Each setter In selector.Setters
                    output.Setters.Add(setter)
                Next
            End If
        Next
        Return output
    End Function

    Public Sub New()
        Styles = New ObservableCollection(Of Style)
    End Sub
End Class


