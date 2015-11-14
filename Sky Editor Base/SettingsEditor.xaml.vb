Public Class SettingsEditor
    Inherits ObjectControl(Of SettingsManager)

    Public Overrides Sub RefreshDisplay()
        MyBase.RefreshDisplay()
        comboBox.Items.Clear()
        SkyEditorBase.Language.LanguageManager.Instance.LoadAllLanguages()

        For Each item In SkyEditorBase.Language.LanguageManager.Instance.Languages.Keys
            comboBox.Items.Add(item)
        Next
        comboBox.SelectedItem = EditingItem.Settings.CurrentLanguage

        chbDebugLanguagePlaceholders.IsChecked = EditingItem.Settings.DebugLanguagePlaceholders
        chbVerbose.IsChecked = EditingItem.Settings.VerboseOutput
    End Sub

    Public Overrides Sub UpdateObject()
        MyBase.UpdateObject()

        EditingItem.Settings.DebugLanguagePlaceholders = chbDebugLanguagePlaceholders.IsChecked
        EditingItem.Settings.CurrentLanguage = comboBox.SelectedItem
        EditingItem.Settings.VerboseOutput = chbVerbose.IsChecked
    End Sub

    Private Sub chbDebugLanguagePlaceholders_Checked(sender As Object, e As RoutedEventArgs) Handles chbDebugLanguagePlaceholders.Checked
        RaiseModified()
    End Sub

    Private Sub comboBox_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles comboBox.SelectionChanged
        RaiseModified()
    End Sub

    Private Sub SettingsEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        PluginHelper.TranslateForm(Me)
    End Sub
End Class
