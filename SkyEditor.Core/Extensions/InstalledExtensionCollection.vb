Imports System.Threading.Tasks
Imports SkyEditor
Imports SkyEditor.Core.Extensions

Namespace Extensions
    Public Class InstalledExtensionCollection
        Implements IExtensionCollection

        Public ReadOnly Property Name As String Implements IExtensionCollection.Name
            Get
                Return "Installed Extensions"
            End Get
        End Property

        Public Function GetChildCollections(manager As Core.PluginManager) As IEnumerable(Of IExtensionCollection) Implements IExtensionCollection.GetChildCollections
            Return manager.GetRegisteredObjects(Of ExtensionType)
        End Function

        Public Function GetExtensions(manager As Core.PluginManager) As IEnumerable(Of ExtensionInfo) Implements IExtensionCollection.GetExtensions
            Return {}
        End Function

        Public Function InstallExtension(Info As ExtensionInfo) As Task(Of ExtensionInstallResult) Implements IExtensionCollection.InstallExtension
            Throw New NotSupportedException
        End Function

        Public Function UninstallExtension(Info As ExtensionInfo) As Task(Of ExtensionUninstallResult) Implements IExtensionCollection.UninstallExtension
            Throw New NotSupportedException
        End Function
    End Class

End Namespace
