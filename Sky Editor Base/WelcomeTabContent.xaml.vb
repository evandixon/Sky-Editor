Imports SkyEditorBase

Public Class WelcomeTabContent
    Inherits UserControl

    Private Sub WelcomeTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        PluginHelper.TranslateForm(Me)
    End Sub
End Class