Imports System.Threading.Tasks
Imports SkyEditor.Core.IO

Namespace Extensions
    Public Interface IExtensionCollection
        ReadOnly Property Name As String
        Function GetChildCollections(manager As PluginManager) As IEnumerable(Of IExtensionCollection)
        Function GetExtensions(manager As PluginManager) As IEnumerable(Of ExtensionInfo)
        Function InstallExtension(extensionID As Guid) As Task(Of ExtensionInstallResult)
        Function UninstallExtension(extensionID As Guid) As Task(Of ExtensionUninstallResult)
    End Interface
End Namespace

