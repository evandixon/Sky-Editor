Imports SkyEditorBase

Public Class WelcomeTab
    Inherits ObjectTab

    Private Sub WelcomeTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.Header = PluginHelper.GetLanguageItem("Sky Editor")
        PluginHelper.TranslateForm(Me)
    End Sub
End Class