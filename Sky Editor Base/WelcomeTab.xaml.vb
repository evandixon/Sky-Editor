Imports SkyEditorBase

Public Class WelcomeTab
    Inherits ObjectTab

    Private Sub WelcomeTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.Header = PluginHelper.GetLanguageItem("Sky Editor")
        PluginHelper.TranslateForm(Me)
    End Sub

    Public Overrides Sub RefreshDisplay(Save As Object)

    End Sub

    Public Overrides Function UpdateObject(Save As Object) As Object
        Return Save
    End Function
End Class