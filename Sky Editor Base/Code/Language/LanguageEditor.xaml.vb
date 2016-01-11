Namespace Language
    Public Class LanguageEditor
        Inherits ObjectControl(Of Language.LanguageManager)
        Public Overrides Sub RefreshDisplay()
            MyBase.RefreshDisplay()
            cbLanguages.Items.Clear()

            EditingItem.LoadAllLanguages()

            For Each item In EditingItem.Languages.Keys
                cbLanguages.Items.Add(item)
            Next

            If cbLanguages.Items.Count > 0 Then
                cbLanguages.SelectedIndex = 0
            End If
        End Sub

        Private Sub cbLanguages_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbLanguages.SelectionChanged
            'Save existing values
            If e.RemovedItems IsNot Nothing AndAlso e.RemovedItems.Count > 0 AndAlso EditingItem.Languages.ContainsKey(e.RemovedItems(0)) Then
                EditingItem.Languages(e.RemovedItems(0)).ContainedObject.Items.Clear()
                For Each item In lbItems.Items
                    EditingItem.Languages(e.RemovedItems(0)).ContainedObject.Items.Add(item)
                Next
            End If

            'Load new values
            If EditingItem.Languages.ContainsKey(cbLanguages.SelectedItem) Then
                lbItems.Items.Clear()
                For Each item In EditingItem.Languages(cbLanguages.SelectedItem).ContainedObject.Items
                    lbItems.Items.Add(item)
                Next
            End If
        End Sub

        Private Sub lbItems_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lbItems.SelectionChanged
            'Save existing values
            If e.RemovedItems IsNot Nothing AndAlso e.RemovedItems.Count > 0 Then
                With DirectCast(e.RemovedItems(0), Language.LanguageItem)
                    If Not .Value = txtValue.Text Then
                        .Value = txtValue.Text
                        RaiseModified()
                    End If
                End With
            End If

            'Load new values
            If lbItems.SelectedItem IsNot Nothing Then
                With DirectCast(lbItems.SelectedItem, Language.LanguageItem)
                    txtKey.Text = .Key
                    txtValue.Text = .Value
                End With
            End If
        End Sub

        Public Overrides Sub UpdateObject()
            MyBase.UpdateObject()

            If lbItems.SelectedItem IsNot Nothing Then
                With DirectCast(lbItems.SelectedItem, Language.LanguageItem)
                    .Value = txtValue.Text
                End With
            End If

            If cbLanguages.SelectedItem IsNot Nothing AndAlso EditingItem.Languages.ContainsKey(cbLanguages.SelectedItem) Then
                EditingItem.Languages(cbLanguages.SelectedItem).ContainedObject.Items.Clear()
                For Each item In lbItems.Items
                    EditingItem.Languages(cbLanguages.SelectedItem).ContainedObject.Items.Add(item)
                Next
            End If
        End Sub

        Private Sub txtValue_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtValue.TextChanged
            If lbItems.SelectedValue IsNot Nothing AndAlso Not DirectCast(lbItems.SelectedItem, LanguageItem).Value = txtValue.Text Then
                DirectCast(lbItems.SelectedItem, LanguageItem).Value = txtValue.Text
                RaiseModified()
            End If
        End Sub

        Private Sub btnNewLang_Click(sender As Object, e As RoutedEventArgs) Handles btnNewLang.Click
            Dim newDialog As New SkyEditorWindows.NewNameWindow(PluginHelper.GetLanguageItem("NewLanguageMessage", "What is the name of the language you want to add?"), PluginHelper.GetLanguageItem("NewLanguageTitle"))
            If newDialog.ShowDialog Then
                If Not EditingItem.Languages.ContainsKey(newDialog.SelectedName) Then
                    EditingItem.EnsureLanguageLoaded(newDialog.SelectedName)
                    cbLanguages.Items.Add(newDialog.SelectedName)
                End If
            End If
        End Sub

        Private Sub btnAddLangItem_Click(sender As Object, e As RoutedEventArgs) Handles btnAddLangItem.Click
            Dim addDialog As New AddLanguageItem
            For Each item In lbItems.Items
                addDialog.CurrentLanguageItems.Add(item)
            Next
            addDialog.DefaultLanguageItems.AddRange(EditingItem.Languages(SettingsManager.Instance.Settings.DefaultLanguage).ContainedObject.Items)
            If addDialog.ShowDialog Then
                Dim newItem = addDialog.SelectedItem.Clone
                lbItems.Items.Add(newItem)
                lbItems.SelectedItem = newItem
            End If
        End Sub
    End Class
End Namespace