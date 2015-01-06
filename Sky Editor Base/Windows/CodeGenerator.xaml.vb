Namespace Windows
    Public Class CodeGenerator
        Dim s As GenericSave
        Private Sub CodeGenerator_Loaded(sender As Object, e As EventArgs) Handles Me.ContentRendered
            'Load Language
            lbActivator.Content = Lists.LanguageText("Activator")
            lbRegion.Content = Lists.LanguageText("Region")
            lbCategory.Content = Lists.LanguageText("Category")
            lbProperty.Content = Lists.LanguageText("Property")
            lbAuthor.Content = Lists.LanguageText("Author")
            Me.Title = Lists.LanguageText("CodeGeneratorTitle")
            btnGenerate.Content = Lists.LanguageText("Generate")
            If ARDS.ManagerV3.CodeDefinitions.Count > 0 Then
                LoadCodeTypes()
            Else
                MessageBox.Show(Lists.LanguageText("Error_NoCheats"))
                Me.Close()
            End If
        End Sub
        Sub LoadCodeTypes()
            If s IsNot Nothing Then
                cbCodeType.Items.Clear()
                For Each t In ARDS.ManagerV3.GetCodeTypes
                    cbCodeType.Items.Add(t)
                Next
                If cbCodeType.Items.Count > 0 Then cbCodeType.SelectedIndex = 0
            Else
                MessageBox.Show(Lists.LanguageText("Error_NoGames"))
            End If
        End Sub
        Private Sub cbCodeType_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbCodeType.SelectionChanged
            cbRegion.Items.Clear()
            For Each r In ARDS.ManagerV3.GetRegions(cbCodeType.SelectedItem)
                cbRegion.Items.Add(r)
            Next
            If cbRegion.Items.Count > 0 Then cbRegion.SelectedIndex = 0
            ReloadButtons()
        End Sub
        Sub ReloadButtons()
            listActivators.Items.Clear()
            For Each item In DirectCast(cbCodeType.SelectedItem, ARDS.CheatFormat).SupportedButtons
                listActivators.Items.Add(item)
            Next
        End Sub
        Private Sub cbRegion_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbRegion.SelectionChanged
            cbGame.Items.Clear()
            For Each g In ARDS.ManagerV3.GetGames(cbCodeType.SelectedItem, cbRegion.SelectedItem, s.SaveID)
                cbGame.Items.Add(IO.Path.GetFileNameWithoutExtension(g))
            Next
            If cbGame.Items.Count > 0 Then cbGame.SelectedIndex = 0
        End Sub
        Private Sub cbGame_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbGame.SelectionChanged
            cbCategory.Items.Clear()
            For Each c In ARDS.ManagerV3.GetCategories(cbCodeType.SelectedItem, cbRegion.SelectedItem, cbGame.SelectedItem)
                cbCategory.Items.Add(c)
            Next
            If cbCategory.Items.Count > 0 Then cbCategory.SelectedIndex = 0
        End Sub
        Private Sub cbCategory_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbCategory.SelectionChanged
            cbProperty.Items.Clear()
            For Each p In ARDS.ManagerV3.GetCode(cbCodeType.SelectedItem, cbRegion.SelectedItem, cbGame.SelectedItem, cbCategory.SelectedItem)
                cbProperty.Items.Add(p)
            Next
            If cbProperty.Items.Count > 0 Then cbProperty.SelectedIndex = 0
        End Sub

        Private Sub btnGenerate_Click(sender As Object, e As RoutedEventArgs) Handles btnGenerate.Click
            If cbProperty.SelectedIndex > -1 Then
                Dim activator As UInt16
                If cbCodeType.SelectedItem = ARDS.CheatFormat.ARDS Then
                    activator = &HFFFF
                    For Each a In listActivators.SelectedItems
                        activator = activator Xor (DirectCast(a, ARDS.Button).ButtonValue)
                    Next
                ElseIf cbCodeType.SelectedItem = ARDS.CheatFormat.ARGBA OrElse cbCodeType.SelectedItem = ARDS.CheatFormat.GamesharkGBA OrElse cbCodeType.SelectedItem = ARDS.CheatFormat.CBA Then
                    activator = &H3FF
                    For Each a In listActivators.SelectedItems
                        If Not (a = ARDS.Button.X OrElse a = ARDS.Button.Y) Then
                            activator = activator Xor DirectCast(a, ARDS.Button).ButtonValue
                        End If
                    Next
                End If
                txtOut.Text = DirectCast(cbProperty.SelectedItem, ARDS.CodeDefinitionV3).GenerateCode(s, cbRegion.SelectedItem, activator, cbCodeType.SelectedItem).ToString
            End If
        End Sub

        Private Sub cbProperty_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbProperty.SelectionChanged
            If cbProperty.SelectedIndex > -1 Then
                lbAuthorDynamic.Content = cbProperty.SelectedItem.Author
            End If
        End Sub
        Public Overloads Sub Show(ByRef SaveFile As GenericSave)
            MyBase.Show()
            'ARDS.ManagerV3.ResetCodeDefinitions() 'Guarentees that every time this is shown, the code definitions will be new.
            s = SaveFile
            cbGame.Items.Clear()
        End Sub
    End Class
End Namespace