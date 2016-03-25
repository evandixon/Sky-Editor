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

        Public Overrides Function GetInstalledExtensions() As IEnumerable(Of ExtensionInfo)
            If SettingsManager.Instance.Settings.DevelopmentMode Then
                Dim out As New List(Of ExtensionInfo)
                out.AddRange(MyBase.GetInstalledExtensions)
                'Todo: load plugins from dev directory to phase out the Plugins directory
                Return out
            Else
                Return MyBase.GetInstalledExtensions
            End If
        End Function

        Public Overrides Async Function InstallExtension(Extension As ExtensionInfo, TempDir As String) As Task(Of ExtensionInstallResult)
            'Dim potentialAssemblyPaths As New List(Of String)
            'potentialAssemblyPaths.AddRange(IO.Directory.GetFiles(TempDir, "*.dll"))
            'potentialAssemblyPaths.AddRange(IO.Directory.GetFiles(TempDir, "*.exe"))
            'Dim supportedAssemblyPaths = Utilities.ReflectionHelpers.GetSupportedPlugins(potentialAssemblyPaths)

            Await MyBase.InstallExtension(Extension, TempDir)
            Return ExtensionInstallResult.RestartRequired
        End Function

    End Class

End Namespace
