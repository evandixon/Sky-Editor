Imports System.Threading.Tasks

Namespace Extensions
    Public Class InstalledExtensionCollection
        Implements IExtensionCollection

        Public ReadOnly Property Name As String Implements IExtensionCollection.Name
            Get
                Return PluginHelper.GetLanguageItem("Installed")
            End Get
        End Property

        Public Function GetChildCollections() As IEnumerable(Of IExtensionCollection) Implements IExtensionCollection.GetChildCollections
            Dim out As New List(Of IExtensionCollection)

            For Each item In PluginManager.GetInstance.GetRegisteredTypes(GetType(ExtensionType))
                If item.GetConstructor({}) IsNot Nothing Then
                    out.Add(item.GetConstructor({}).Invoke({}))
                End If
            Next

            Return out
        End Function

        Public Function GetExtensions() As IEnumerable(Of ExtensionInfo) Implements IExtensionCollection.GetExtensions
            Return {}
        End Function

        Public Function InstallExtension(Info As ExtensionInfo) As Task(Of ExtensionInstallResult) Implements IExtensionCollection.InstallExtension
            Throw New NotSupportedException()
        End Function

        Public Function UninstallExtension(Info As ExtensionInfo) As Task(Of ExtensionUninstallResult) Implements IExtensionCollection.UninstallExtension
            Throw New NotSupportedException()
        End Function
    End Class

End Namespace
