Namespace SkyEditorWindows
    Public Class CodeGenerator
        Dim m As PluginManager
        Private Sub CodeGenerator_Loaded(sender As Object, e As EventArgs) Handles Me.ContentRendered
            'Load Language
            lbActivator.Content = PluginHelper.GetLanguageItem("Activator")
            lbRegion.Content = PluginHelper.GetLanguageItem("Region")
            lbCategory.Content = PluginHelper.GetLanguageItem("Category")
            lbProperty.Content = PluginHelper.GetLanguageItem("Property")
            lbAuthor.Content = PluginHelper.GetLanguageItem("Author")
            lblCodeType.Content = PluginHelper.GetLanguageItem("CodeType", "Code Type")
            lblGame.Content = PluginHelper.GetLanguageItem("Game")
            Me.Title = PluginHelper.GetLanguageItem("CodeGeneratorTitle", "Cheat Code Generator")
            btnGenerate.Content = PluginHelper.GetLanguageItem("Generate")
            If False Then 'm.CheatManager.CodeDefinitions.Count > 0 Then
                LoadCodeTypes()
            Else
                MessageBox.Show(PluginHelper.GetLanguageItem("Error_NoCheats", "You don't have any code generator plugins installed.  To use the code generator, put a supported plugin into ~/Resources/ and restart the program."))
                Me.Close()
            End If
        End Sub
        Sub LoadCodeTypes()
            'If m.Save IsNot Nothing Then
            '    cbCodeType.Items.Clear()
            '    For Each t In m.CheatManager.GetCodeTypes
            '        cbCodeType.Items.Add(t)
            '    Next
            '    If cbCodeType.Items.Count > 0 Then cbCodeType.SelectedIndex = 0
            'Else
            '    MessageBox.Show(PluginHelper.GetLanguageItem("Error_NoGames", "In order to generate codes, you need to have a save file loaded.  Use File -> New if you don't have one, otherwise, use File-> Open."))
            'End If
        End Sub
        Private Sub cbCodeType_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbCodeType.SelectionChanged
            cbRegion.Items.Clear()
            For Each r In {} 'm.CheatManager.GetRegions(cbCodeType.SelectedItem)
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
            'cbGame.Items.Clear()
            'For Each g In m.CheatManager.GetGames(cbCodeType.SelectedItem, cbRegion.SelectedItem, m.Save.SaveID)
            '    cbGame.Items.Add(IO.Path.GetFileNameWithoutExtension(g))
            'Next
            'If cbGame.Items.Count > 0 Then cbGame.SelectedIndex = 0
        End Sub
        Private Sub cbGame_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbGame.SelectionChanged
            cbCategory.Items.Clear()
            For Each c In {} ' m.CheatManager.GetCategories(cbCodeType.SelectedItem, cbRegion.SelectedItem, cbGame.SelectedItem)
                cbCategory.Items.Add(c)
            Next
            If cbCategory.Items.Count > 0 Then cbCategory.SelectedIndex = 0
        End Sub
        Private Sub cbCategory_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbCategory.SelectionChanged
            cbProperty.Items.Clear()
            For Each p In {} 'm.CheatManager.GetCode(cbCodeType.SelectedItem, cbRegion.SelectedItem, cbGame.SelectedItem, cbCategory.SelectedItem)
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
                'txtOut.Text = DirectCast(cbProperty.SelectedItem, ARDS.CodeDefinition).GenerateCode(m.Save, cbRegion.SelectedItem, activator, cbCodeType.SelectedItem).ToString
            End If
        End Sub

        Private Sub cbProperty_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbProperty.SelectionChanged
            If cbProperty.SelectedIndex > -1 Then
                lbAuthorDynamic.Content = cbProperty.SelectedItem.Author
            End If
        End Sub
        Public Overloads Sub Show()
            MyBase.Show()
            cbGame.Items.Clear()
        End Sub
        Public Sub New(Manager As PluginManager)
            MyBase.New()
            InitializeComponent()
            m = Manager
        End Sub
    End Class
End Namespace