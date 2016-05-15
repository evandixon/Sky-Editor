Imports System.IO
Imports System.Threading.Tasks
Imports SkyEditor.Core.Extensions
Imports SkyEditor.Core.IO

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

        Public Overrides Async Function InstallExtension(Extension As ExtensionInfo, TempDir As String) As Task(Of ExtensionInstallResult)
            Await MyBase.InstallExtension(Extension, TempDir)
            Return ExtensionInstallResult.RestartRequired
        End Function

        ''' <summary>
        ''' Uninstalls the given extension.
        ''' </summary>
        ''' <param name="Extension">Extension to uninstall</param>
        Public Overrides Function UninstallExtension(Extension As ExtensionInfo) As Task(Of ExtensionUninstallResult)
            Redistribution.RedistributionHelpers.ScheduleDelete(Path.Combine(ExtensionDirectory, Extension.ID.ToString))
            Return Task.FromResult(ExtensionUninstallResult.RestartRequired)
        End Function

    End Class

End Namespace
