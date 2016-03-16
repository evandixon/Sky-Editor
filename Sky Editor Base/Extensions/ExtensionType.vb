Imports System.Threading.Tasks

Namespace Extensions
    Public MustInherit Class ExtensionType
        ''' <summary>
        ''' The user-friendly name of the extension type.
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride ReadOnly Property Name As String

        ''' <summary>
        ''' The internal name of the extension type used in paths.
        ''' </summary>
        ''' <returns></returns>
        Protected Overridable ReadOnly Property InternalName As String
            Get
                Return Me.GetType.Name
            End Get
        End Property

        Public Overridable Function GetExtensionDirectory(Extension As ExtensionInfo)
            Return IO.Path.Combine(PluginHelper.RootResourceDirectory, "Extensions", InternalName, Extension.ID)
        End Function

        ''' <summary>
        ''' Lists the extensions that are currently installed.
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function GetInstalledExtensions() As IEnumerable(Of ExtensionInfo)
            Dim filename = IO.Path.Combine(PluginHelper.RootResourceDirectory, "Extensions", InternalName, "info.skyextlst")
            If IO.File.Exists(filename) Then
                Return Utilities.Json.DeserializeFromFile(Of List(Of ExtensionInfo))(filename)
            Else
                Return {}
            End If
        End Function

        ''' <summary>
        ''' Installs the extension that's stored in the given directory.
        ''' </summary>
        ''' <param name="TempDir">Temporary directory that contains the extension's files.</param>
        Public Overridable Async Function InstallExtension(Extension As ExtensionInfo, TempDir As String) As Task(Of ExtensionInstallResult)
            Await Utilities.FileSystem.CopyDirectory(TempDir, GetExtensionDirectory(Extension))
            Dim filename = IO.Path.Combine(PluginHelper.RootResourceDirectory, "Extensions", InternalName, "info.skyextlst")
            If IO.File.Exists(filename) Then
                Dim installed = Utilities.Json.DeserializeFromFile(Of List(Of ExtensionInfo))(filename)
                installed.Add(Extension)
                Utilities.Json.SerializeToFile(filename, installed)
            Else
                Dim installed As New List(Of ExtensionInfo)
                installed.Add(Extension)
                If Not IO.Directory.Exists(IO.Path.GetDirectoryName(filename)) Then
                    IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(filename))
                End If
                Utilities.Json.SerializeToFile(filename, installed)
            End If
            Return ExtensionInstallResult.Success
        End Function

        ''' <summary>
        ''' Uninstalls the given extension.
        ''' </summary>
        ''' <param name="Extension">Extension to uninstall</param>
        Public Overridable Function UninstallExtension(Extension As ExtensionInfo) As Task(Of ExtensionUninstallResult)
            Redistribution.RedistributionHelpers.ScheduleDelete(GetExtensionDirectory(Extension))
            Return Task.FromResult(ExtensionUninstallResult.RestartRequired)
        End Function
    End Class
End Namespace

