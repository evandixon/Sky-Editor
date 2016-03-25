Imports System.Windows.Forms
Imports SkyEditorBase.Redistribution

Namespace UI
    Public Class PluginControl
        Inherits ObjectControl
        Public Overrides Sub RefreshDisplay()
            'Because this may take a moment or two, we're running it asynchronously
            Task.Run(New Action(AddressOf DoRefreshDisplay))
        End Sub

        Private Sub DoRefreshDisplay()
            Dim uiElements As New List(Of PluginUiElement)
            With GetEditingObject(Of PluginManager)()
                Dim supportedAssemblyPaths = Utilities.ReflectionHelpers.GetSupportedPlugins(PluginHelper.GetPluginAssemblies, .CoreAssemblyName)
                For Each item In .Plugins
                    Dim assemblyPath As String = item.GetType.Assembly.Location
                    uiElements.Add(New PluginUiElement With {.IsEnabled = False, .Author = item.PluginAuthor, .Name = item.PluginName, .Credits = item.Credits, .Filename = assemblyPath, .ContainedDefinition = item})
                    supportedAssemblyPaths.Remove(assemblyPath)
                Next

                ''Now to look at the plugins that aren't loaded
                'Using manager As New Utilities.AssemblyReflectionManager
                '    For Each item In supportedAssemblyPaths
                '        manager.LoadAssembly(item, "PluginControl")
                '        Dim elements = manager.Reflect(item, Function(CurrentAssembly As Assembly, Args() As Object) As List(Of PluginUiElement)
                '                                                 Dim out As New List(Of PluginUiElement)
                '                                                 For Each result In From t In CurrentAssembly.GetTypes Where Utilities.ReflectionHelpers.IsOfType(t, GetType(iSkyEditorPlugin)) AndAlso t.GetConstructor({}) IsNot Nothing
                '                                                     Dim info As iSkyEditorPlugin = result.GetConstructor({}).Invoke({})
                '                                                     out.Add(New PluginUiElement With {.IsEnabled = False, .Author = info.PluginAuthor, .Name = info.PluginName, .Credits = info.Credits, .Filename = Args(0)})
                '                                                 Next
                '                                                 Return out
                '                                             End Function, item)
                '        For Each element In elements
                '            uiElements.Add(New PluginUiElement With {.IsEnabled = False, .Author = element.Author, .Name = element.Name, .Credits = element.Credits, .Filename = element.Filename})
                '        Next
                '    Next
                'End Using

            End With

            For Each item In uiElements
                AddHandler item.Modified, AddressOf OnItemModified
            Next
            Dispatcher.Invoke(Sub()
                                  gridPlugins.ItemsSource = uiElements
                              End Sub)
            IsModified = False
        End Sub

        Private Sub OnItemModified(sender As Object, e As EventArgs)
            IsModified = True
        End Sub

        'Private Sub btnApply_Click(sender As Object, e As RoutedEventArgs) Handles btnApply.Click
        '    If gridPlugins.ItemsSource IsNot Nothing Then 'Just in case
        '        Dim plugins = SettingsManager.Instance.Settings.Plugins
        '        For Each item As PluginUiElement In gridPlugins.ItemsSource
        '            Dim filenamePart = item.Filename.Replace(PluginManager.GetInstance.PluginFolder, "").TrimStart("\")
        '            If item.IsEnabled Then
        '                If Not plugins.Contains(filenamePart) Then
        '                    plugins.Add(filenamePart)
        '                End If
        '            Else
        '                plugins.Remove(filenamePart)
        '            End If
        '        Next
        '        SettingsManager.Instance.Settings.Plugins = plugins
        '        SettingsManager.Instance.Save()
        '        If MessageBox.Show(PluginHelper.GetLanguageItem("You need to restart the program to save your changes.  Do you want to restart now?"), PluginHelper.GetLanguageItem("Sky Editor"), MessageBoxButtons.YesNo) = DialogResult.Yes Then
        '            Redistribution.RedistributionHelpers.RequestRestartProgram()
        '        End If
        '    End If
        '    IsModified = False
        'End Sub

        Private Sub SettingsEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("Plugins")
            colEnabled.Header = PluginHelper.GetLanguageItem("Enabled")
            colName.Header = PluginHelper.GetLanguageItem("Name")
            colAuthor.Header = PluginHelper.GetLanguageItem("Author")
            colFilename.Header = PluginHelper.GetLanguageItem("Filename")
            btnCreateExtension.Content = PluginHelper.GetLanguageItem("Create Extension")
            'menuSave.Header = PluginHelper.GetLanguageItem("_Save")
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(PluginManager)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 0
        End Function

        'Private Sub menuSave_Click(sender As Object, e As RoutedEventArgs) Handles menuSave.Click
        '    If gridPlugins.SelectedItem IsNot Nothing AndAlso DirectCast(gridPlugins.SelectedItem, PluginUiElement).ContainedDefinition IsNot Nothing Then
        '        Dim s As New SaveFileDialog
        '        s.Filter = $"{PluginHelper.GetLanguageItem("Zip Files")} (*.zip)|*.zip|{PluginHelper.GetLanguageItem("All Files")} (*.*)|*.*"
        '        If s.ShowDialog = DialogResult.OK Then
        '            Redistribution.RedistributionHelpers.PackPlugin(GetEditingObject(Of PluginManager), DirectCast(gridPlugins.SelectedItem, PluginUiElement).Filename, s.FileName, DirectCast(gridPlugins.SelectedItem, PluginUiElement).ContainedDefinition)
        '        End If
        '    End If
        'End Sub

        Private Async Sub btnCreateExtension_Click(sender As Object, e As RoutedEventArgs) Handles btnCreateExtension.Click
            Dim plugins As New List(Of Interfaces.iSkyEditorPlugin)
            For Each item As PluginUiElement In gridPlugins.SelectedItems
                plugins.Add(item.ContainedDefinition)
            Next
            If plugins.Count > 0 Then
                Dim info As New SkyEditorBase.Extensions.ExtensionInfo
                Dim first = plugins.First
                info.Name = first.PluginName
                info.Author = first.PluginAuthor
                info.Version = first.GetType.Assembly.GetName.Version.ToString
                Dim o As New ObjectWindow
                o.ObjectToEdit = info
                o.ShowDialog()
                Dim s As New SaveFileDialog
                    s.Filter = $"{PluginHelper.GetLanguageItem("Zip Files")} (*.zip)|*.zip|{PluginHelper.GetLanguageItem("All Files")} (*.*)|*.*"
                    If s.ShowDialog = DialogResult.OK Then
                    Await RedistributionHelpers.PackPlugins(plugins, s.FileName, o.ObjectToEdit)
                End If
                ' End If
            End If
        End Sub

        'Private Sub gridPlugins_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles gridPlugins.SelectionChanged
        '    If gridPlugins.SelectedItem IsNot Nothing Then
        '        menuSave.Visibility = Visibility.Visible
        '    Else
        '        menuSave.Visibility = Visibility.Collapsed
        '    End If
        'End Sub

    End Class

End Namespace
