Imports System.Windows.Forms

Namespace UI
    Public Class ExtensionManager
        Inherits ObjectControl

        Private Function GetTVItem(Collection As SkyEditorBase.Extensions.IExtensionCollection)
            Dim item As New TreeViewItem
            item.Header = Collection.Name
            item.Tag = Collection
            For Each child In Collection.GetChildCollections
                item.Items.Add(GetTVItem(child))
            Next
            Return item
        End Function

        Private Sub ExtensionManager_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("Extension Manager")
            tvCategories.Items.Clear()
            tvCategories.Items.Add(GetTVItem(New SkyEditorBase.Extensions.InstalledExtensionCollection))
        End Sub

        Private Sub tvCategories_SelectedItemChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Object)) Handles tvCategories.SelectedItemChanged
            RefreshCurrentExtensionList()
        End Sub

        Sub RefreshCurrentExtensionList()
            If tvCategories.SelectedItem IsNot Nothing Then
                lvExtensions.ItemsSource = DirectCast(tvCategories.SelectedItem.Tag, SkyEditorBase.Extensions.IExtensionCollection).GetExtensions
            End If
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(SkyEditorBase.Extensions.ExtensionHelper)}
        End Function

        Private Sub lvExtensions_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lvExtensions.SelectionChanged
            If lvExtensions.SelectedItem IsNot Nothing Then
                btnToggleInstall.Visibility = Visibility.Visible
                Dim info = DirectCast(lvExtensions.SelectedItem, SkyEditorBase.Extensions.ExtensionInfo)
                If info.IsInstalled Then
                    btnToggleInstall.Content = PluginHelper.GetLanguageItem("Uninstall")
                Else
                    btnToggleInstall.Content = PluginHelper.GetLanguageItem("Install")
                End If
            Else
                btnToggleInstall.Visibility = Visibility.Collapsed
            End If
        End Sub

        Private Async Sub btnBrowse_Click(sender As Object, e As RoutedEventArgs) Handles btnBrowse.Click
            Dim o As New OpenFileDialog
            o.Filter = $"{PluginHelper.GetLanguageItem("Zip Files")} (*.zip)|*.zip|{PluginHelper.GetLanguageItem("All Files")} (*.*)|*.*"
            If o.ShowDialog = DialogResult.OK Then
                Dim result = Await SkyEditorBase.Extensions.ExtensionHelper.InstallExtension(o.FileName)
                DisplayInstallResultMessage(result)
                RefreshCurrentExtensionList()
            End If
        End Sub

        Private Async Sub btnToggleInstall_Click(sender As Object, e As RoutedEventArgs) Handles btnToggleInstall.Click
            If tvCategories.SelectedItem IsNot Nothing AndAlso lvExtensions.SelectedItem IsNot Nothing Then
                Dim repo = DirectCast(tvCategories.SelectedItem.Tag, SkyEditorBase.Extensions.IExtensionCollection)
                Dim info = DirectCast(lvExtensions.SelectedItem, SkyEditorBase.Extensions.ExtensionInfo)
                If info.IsInstalled Then
                    DisplayUninstallResultMessage(Await repo.UninstallExtension(info))
                Else
                    DisplayInstallResultMessage(Await repo.InstallExtension(info))
                End If
            End If
        End Sub

        Private Sub DisplayInstallResultMessage(Result As SkyEditorBase.Extensions.ExtensionInstallResult)
            Select Case Result
                Case SkyEditorBase.Extensions.ExtensionInstallResult.Success
                    MessageBox.Show(PluginHelper.GetLanguageItem("Extension was installed successfully."))
                Case SkyEditorBase.Extensions.ExtensionInstallResult.RestartRequired
                    If MessageBox.Show(PluginHelper.GetLanguageItem("The application must be restarted to complete installation.  Would you like to restart now?"), PluginHelper.GetLanguageItem("Extentensions"), MessageBoxButtons.YesNo) = DialogResult.Yes Then
                        Redistribution.RedistributionHelpers.RequestRestartProgram()
                    End If
                Case SkyEditorBase.Extensions.ExtensionInstallResult.InvalidFormat
                    MessageBox.Show(PluginHelper.GetLanguageItem("The provided file is not a valid extension."))
                Case SkyEditorBase.Extensions.ExtensionInstallResult.UnsupportedFormat
                    MessageBox.Show(PluginHelper.GetLanguageItem("The provided extension is not supported.  This may be an extension to another extension that's not currently installed."))
                Case Else
                    Console.WriteLine("Unknown error installing extension.")
            End Select
        End Sub

        Private Sub DisplayUninstallResultMessage(Result As SkyEditorBase.Extensions.ExtensionUninstallResult)
            Select Case Result
                Case SkyEditorBase.Extensions.ExtensionUninstallResult.Success
                    MessageBox.Show(PluginHelper.GetLanguageItem("Extension was uninstalled successfully."))
                Case SkyEditorBase.Extensions.ExtensionUninstallResult.RestartRequired
                    If MessageBox.Show(PluginHelper.GetLanguageItem("The application must be restarted to complete uninstallation.  Would you like to restart now?"), PluginHelper.GetLanguageItem("Extentensions"), MessageBoxButtons.YesNo) = DialogResult.Yes Then
                        Redistribution.RedistributionHelpers.RequestRestartProgram()
                    End If
                Case Else
                    Console.WriteLine("Unknown error uninstalling extension.")
            End Select
        End Sub
    End Class
End Namespace

