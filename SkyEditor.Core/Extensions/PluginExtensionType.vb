Imports System.IO
Imports System.Threading.Tasks
Imports SkyEditor.Core.Extensions
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.settings

Namespace Extensions
    Public Class PluginExtensionType
        Inherits ExtensionType

        Public Overrides ReadOnly Property Name As String
            Get
                Return "Plugins"
            End Get
        End Property

        Protected Overrides ReadOnly Property InternalName As String
            Get
                Return "Plugins"
            End Get
        End Property

        Public Overrides Async Function InstallExtension(extensionID As Guid, TempDir As String) As Task(Of ExtensionInstallResult)
            Await MyBase.InstallExtension(extensionID, TempDir)
            Return ExtensionInstallResult.RestartRequired
        End Function

        Public Overrides Function GetExtensionDirectory(extensionID As Guid) As String
            If extensionID = Guid.Empty Then
                Return Path.Combine(RootExtensionDirectory, InternalName, "Development")
            Else
                Return MyBase.GetExtensionDirectory(extensionID)
            End If
        End Function

        Public Overrides Function GetInstalledExtensions(manager As PluginManager) As IEnumerable(Of ExtensionInfo)
            Dim extensions As New List(Of ExtensionInfo)
            extensions.AddRange(MyBase.GetInstalledExtensions(manager))
            If manager.CurrentSettingsProvider.GetIsDevMode Then
                'Load the development plugins
                Dim devDir = Path.Combine(RootExtensionDirectory, InternalName, "Development")
                Dim info As New ExtensionInfo
                info.ID = Guid.Empty
                info.Name = My.Resources.Language.PluginDevExtName
                info.Description = My.Resources.Language.PluginDevExtDescription
                info.Author = My.Resources.Language.PluginDevExtAuthor
                info.IsInstalled = True
                info.IsEnabled = True
                info.Version = My.Resources.Language.PluginDevExtVersion
                For Each item In manager.CurrentIOProvider.GetFiles(devDir, "*.dll", True)
                    info.ExtensionFiles.Add(Path.GetFileName(item))
                Next
                ''While exe files can technically be loaded as plugins, it can cause some other problems, especially if we try to load another copy of the executing assembly
                'For Each item In manager.CurrentIOProvider.GetFiles(devDir, "*.exe", True)
                '    info.ExtensionFiles.Add(Path.GetFileName(item))
                'Next
                extensions.Add(info)
            End If
            Return extensions
        End Function

        ''' <summary>
        ''' Uninstalls the given extension.
        ''' </summary>
        ''' <param name="extensionID">ID of the extension to uninstall</param>
        Public Overrides Function UninstallExtension(extensionID As Guid) As Task(Of ExtensionUninstallResult)
            CurrentPluginManager.CurrentSettingsProvider.ScheduleDirectoryForDeletion(Path.Combine(RootExtensionDirectory, extensionID.ToString))
            Return Task.FromResult(ExtensionUninstallResult.RestartRequired)
        End Function

    End Class

End Namespace
