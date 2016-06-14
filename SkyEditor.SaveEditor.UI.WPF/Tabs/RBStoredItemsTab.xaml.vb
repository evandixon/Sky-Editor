Imports SkyEditor.SaveEditor.MysteryDungeon.Rescue
Imports SkyEditor.UI.WPF

Namespace Tabs
    Public Class RBStoredItemsTab
        Inherits ObjectControl
        Public Overrides Sub RefreshDisplay()
            Dim x = GetEditingObject(Of RBSave).StoredItemCounts
            For count As Integer = 0 To 238
                If x(count) > 0 Then
                    txtDisplay.Text &= Lists.RBItems(count + 1) & ": " & x(count) & vbCrLf
                End If
            Next
        End Sub

        Private Sub RBStoredItemsTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = My.Resources.Language.StoredItems
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(RBSave)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 24
        End Function

    End Class
End Namespace