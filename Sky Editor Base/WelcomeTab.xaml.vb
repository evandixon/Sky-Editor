Imports SkyEditorBase

Public Class WelcomeTab
    Inherits EditorTab


    Public Overrides Sub RefreshDisplay(Save As GenericSave)

    End Sub

    Public Overrides ReadOnly Property SupportedGames As String()
        Get
            Return Nothing
        End Get
    End Property

    Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
        Return Save
    End Function

    Private Sub WelcomeTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.Header = PluginHelper.GetLanguageItem("Sky Editor")
        PluginHelper.TranslateForm(Me)
    End Sub
End Class
