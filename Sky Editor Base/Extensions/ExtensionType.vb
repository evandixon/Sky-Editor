Imports System.Threading.Tasks

Namespace Extensions
    Public MustInherit Class ExtensionType
        Implements IExtensionCollection

        ''' <summary>
        ''' The user-friendly name of the extension type.
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride ReadOnly Property Name As String Implements IExtensionCollection.Name

        ''' <summary>
        ''' The internal name of the extension type used in paths.
        ''' </summary>
        ''' <returns></returns>
        Protected Overridable ReadOnly Property InternalName As String
            Get
                Return Me.GetType.Name
            End Get
        End Property

        ''' <summary>
        ''' Gets the directory that the extension files are stored in.
        ''' </summary>
        ''' <param name="Extension">The info file of the extension.</param>
        ''' <returns></returns>
        Public Overridable Function GetExtensionDirectory(Extension As ExtensionInfo)
            Return IO.Path.Combine(PluginHelper.RootResourceDirectory, "Extensions", InternalName, Extension.ID)
        End Function

        ''' <summary>
        ''' Lists the extensions that are currently installed.
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function GetInstalledExtensions() As IEnumerable(Of ExtensionInfo) Implements IExtensionCollection.GetExtensions
            Dim extDir = IO.Path.Combine(PluginHelper.RootResourceDirectory, "Extensions", InternalName)
            Dim out As New List(Of ExtensionInfo)
            If IO.Directory.Exists(extDir) Then
                For Each item In IO.Directory.GetDirectories(extDir)
                    If IO.File.Exists(IO.Path.Combine(item, "info.skyext")) Then
                        Dim e = ExtensionInfo.Open(IO.Path.Combine(item, "info.skyext"))
                        e.IsInstalled = True
                        out.Add(e)
                    End If
                Next
            End If
            Return out
        End Function

        Private Function InstallExtension(Extension As ExtensionInfo) As Task(Of ExtensionInstallResult) Implements IExtensionCollection.InstallExtension
            Throw New NotSupportedException("This IExtensionCollection lists extensions that are currently installed, not ones that can be installed, so this cannnot install extensions.")
        End Function

        ''' <summary>
        ''' Installs the extension that's stored in the given directory.
        ''' </summary>
        ''' <param name="TempDir">Temporary directory that contains the extension's files.</param>
        Public Overridable Async Function InstallExtension(Extension As ExtensionInfo, TempDir As String) As Task(Of ExtensionInstallResult)
            Await Utilities.FileSystem.CopyDirectory(TempDir, GetExtensionDirectory(Extension))
            Return ExtensionInstallResult.Success
        End Function

        ''' <summary>
        ''' Uninstalls the given extension.
        ''' </summary>
        ''' <param name="Extension">Extension to uninstall</param>
        Public Overridable Function UninstallExtension(Extension As ExtensionInfo) As Task(Of ExtensionUninstallResult) Implements IExtensionCollection.UninstallExtension
            Redistribution.RedistributionHelpers.ScheduleDelete(GetExtensionDirectory(Extension))
            Return Task.FromResult(ExtensionUninstallResult.RestartRequired)
        End Function

        Private Function GetChildCollections() As IEnumerable(Of IExtensionCollection) Implements IExtensionCollection.GetChildCollections
            Return {}
        End Function
    End Class
End Namespace

