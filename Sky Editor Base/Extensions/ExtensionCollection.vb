Imports System.Threading.Tasks

Namespace Extensions
    Public Interface IExtensionCollection
        ReadOnly Property Name As String
        Function GetChildCollections() As IEnumerable(Of IExtensionCollection)
        Function GetExtensions() As IEnumerable(Of ExtensionInfo)
        Function InstallExtension(Info As ExtensionInfo) As Task(Of ExtensionInstallResult)
        Function UninstallExtension(Info As ExtensionInfo) As Task(Of ExtensionUninstallResult)
    End Interface
End Namespace

