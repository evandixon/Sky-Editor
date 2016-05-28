Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports SkyEditor.Core.UI

Namespace ObjectControls
    Public Class GenericIList
        Private Sub lvItems_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lvItems.SelectionChanged
            menuDelete.IsEnabled = (lvItems.SelectedItems.Count > 0)
        End Sub

        Private Sub menuDelete_Click(sender As Object, e As RoutedEventArgs) Handles menuDelete.Click
            Dim list = DirectCast(Me.DataContext, IList)
            Dim selected As New List(Of Object)
            selected.AddRange(lvItems.SelectedItems)
            For Each item In selected
                list.Remove(item)
            Next
        End Sub
    End Class
End Namespace

