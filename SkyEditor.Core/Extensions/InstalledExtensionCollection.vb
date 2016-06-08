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
            Dim out = manager.GetRegisteredObjects(Of ExtensionType)
            For Each item In out
                item.CurrentPluginManager = manager
                item.RootExtensionDirectory = manager.ExtensionDirectory
            Next
            Return out
        End Function

        Public Function GetExtensions(manager As Core.PluginManager) As IEnumerable(Of ExtensionInfo) Implements IExtensionCollection.GetExtensions
            Return {}
        End Function

        Public Function InstallExtension(extensionID As Guid) As Task(Of ExtensionInstallResult) Implements IExtensionCollection.InstallExtension
            Throw New NotSupportedException
        End Function

        Public Function UninstallExtension(extensionID As Guid) As Task(Of ExtensionUninstallResult) Implements IExtensionCollection.UninstallExtension
            Throw New NotSupportedException
        End Function
    End Class

End Namespace
