Imports System.Threading.Tasks

Namespace Extensions
    Public Class PluginExtensionType
        Inherits ExtensionType

        Public Overrides ReadOnly Property Name As String
            Get
                Return PluginHelper.GetLanguageItem("Plugins")
            End Get
        End Property

        Protected Overrides ReadOnly Property InternalName As String
            Get
                Return "Plugins"
            End Get
        End Property

        Public Overrides Async Function InstallExtension(Extension As ExtensionInfo, TempDir As String) As Task(Of ExtensionInstallResult)
            Dim potentialAssemblyPaths As New List(Of String)
            potentialAssemblyPaths.AddRange(IO.Directory.GetFiles(TempDir, "*.dll"))
            potentialAssemblyPaths.AddRange(IO.Directory.GetFiles(TempDir, "*.exe"))
            Dim supportedAssemblyPaths = Utilities.ReflectionHelpers.GetSupportedPlugins(potentialAssemblyPaths)

            Await MyBase.InstallExtension(Extension, TempDir)
            Return ExtensionInstallResult.RestartRequired
        End Function

    End Class

End Namespace
