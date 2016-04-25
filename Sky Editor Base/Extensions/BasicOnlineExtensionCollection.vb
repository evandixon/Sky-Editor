Imports System.Threading.Tasks

Namespace Extensions
    Public Class BasicOnlineExtensionCollection
        Implements IExtensionCollection

        Public ReadOnly Property Name As String Implements IExtensionCollection.Name
            Get
                Throw New NotImplementedException()
            End Get
        End Property

        Public Function GetChildCollections() As IEnumerable(Of IExtensionCollection) Implements IExtensionCollection.GetChildCollections
            Throw New NotImplementedException()
        End Function

        Public Function GetExtensions() As IEnumerable(Of ExtensionInfo) Implements IExtensionCollection.GetExtensions
            Throw New NotImplementedException()
        End Function

        Public Function InstallExtension(Info As ExtensionInfo) As Task(Of ExtensionInstallResult) Implements IExtensionCollection.InstallExtension
            Throw New NotImplementedException()
        End Function

        Public Function UninstallExtension(Info As ExtensionInfo) As Task(Of ExtensionUninstallResult) Implements IExtensionCollection.UninstallExtension
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace

