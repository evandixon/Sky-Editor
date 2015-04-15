Imports SkyEditorBase

Public Class RBStoredItemsTab
    Inherits EditorTab
    Public Overrides Sub RefreshDisplay(Save As GenericSave)
        'debug stored items
        Dim x = DirectCast(Save, RBSave).StoredItemCounts
        For count As Integer = 0 To 238
            If x(count) > 0 Then
                txtDisplay.Text &= Lists.RBItemNames(count + 1) & ": " & x(count) & vbCrLf
            End If
        Next
    End Sub

    Public Overrides ReadOnly Property SupportedGames As String()
        Get
            Return {GameStrings.RBSave}
        End Get
    End Property

    Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
        Return Save
    End Function

    Private Sub RBStoredItemsTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.Header = PluginHelper.GetLanguageItem("Stored Items")
    End Sub
End Class