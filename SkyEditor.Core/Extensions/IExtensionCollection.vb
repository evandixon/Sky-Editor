Imports System.Threading.Tasks
Imports SkyEditor.Core.IO

Namespace Extensions
    Public Interface IExtensionCollection
        ReadOnly Property Name As String
        Function GetChildCollections(manager As PluginManager) As IEnumerable(Of IExtensionCollection)
        Function GetExtensions(manager As PluginManager) As IEnumerable(Of ExtensionInfo)
        Function InstallExtension(Info As ExtensionInfo) As Task(Of ExtensionInstallResult)
        Function UninstallExtension(Info As ExtensionInfo) As Task(Of ExtensionUninstallResult)
    End Interface
End Namespace

