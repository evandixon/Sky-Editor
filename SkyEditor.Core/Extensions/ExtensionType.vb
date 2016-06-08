Imports System.IO
Imports System.Threading.Tasks
Imports SkyEditor
Imports SkyEditor.Core.Extensions
Imports SkyEditor.Core.IO

Namespace Extensions
    Public MustInherit Class ExtensionType
        Implements IExtensionCollection

        ''' <summary>
        ''' The user-friendly name of the extension type.
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride ReadOnly Property Name As String Implements IExtensionCollection.Name

        ''' <summary>
        ''' Gets or sets the directory the ExtensionType stores extensions in.
        ''' </summary>
        ''' <returns></returns>
        Public Property RootExtensionDirectory As String

        Public Property CurrentPluginManager As PluginManager

        ''' <summary>
        ''' The internal name of the extension type used in paths.
        ''' </summary>
        ''' <returns></returns>
        Protected Overridable ReadOnly Property InternalName As String
            Get
                Return Me.GetType.Name
            End Get
        End Property

        Public Overridable Function GetExtensionDirectory(extensionID As Guid) As String
            Return Path.Combine(RootExtensionDirectory, InternalName, extensionID.ToString)
        End Function

        ''' <summary>
        ''' Lists the extensions that are currently installed.
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function GetInstalledExtensions(manager As Core.PluginManager) As IEnumerable(Of ExtensionInfo) Implements IExtensionCollection.GetExtensions
            Dim out As New List(Of ExtensionInfo)
            If manager.CurrentIOProvider.DirectoryExists(Path.Combine(RootExtensionDirectory, InternalName)) Then
                For Each item In manager.CurrentIOProvider.GetDirectories(Path.Combine(RootExtensionDirectory, InternalName), True)
                    If manager.CurrentIOProvider.FileExists(Path.Combine(item, "info.skyext")) Then
                        Dim e = ExtensionInfo.OpenFromFile(Path.Combine(item, "info.skyext"), manager.CurrentIOProvider)
                        e.IsInstalled = True
                        out.Add(e)
                    End If
                Next
            End If
            Return out
        End Function

        Private Function InstallExtension(extensionID As Guid) As Task(Of ExtensionInstallResult) Implements IExtensionCollection.InstallExtension
            Throw New NotSupportedException("This IExtensionCollection lists extensions that are currently installed, not ones that can be installed, so this cannnot install extensions.")
        End Function

        ''' <summary>
        ''' Installs the extension that's stored in the given directory.
        ''' </summary>
        ''' <param name="TempDir">Temporary directory that contains the extension's files.</param>
        Public Overridable Async Function InstallExtension(extensionID As Guid, TempDir As String) As Task(Of ExtensionInstallResult)
            Await Core.Utilities.FileSystem.CopyDirectory(TempDir, GetExtensionDirectory(extensionID), CurrentPluginManager.CurrentIOProvider)
            Return ExtensionInstallResult.Success
        End Function

        ''' <summary>
        ''' Uninstalls the given extension.
        ''' </summary>
        ''' <param name="extensionID">ID of the extension to uninstall</param>
        Public Overridable Function UninstallExtension(extensionID As Guid) As Task(Of ExtensionUninstallResult) Implements IExtensionCollection.UninstallExtension
            CurrentPluginManager.CurrentIOProvider.DeleteDirectory(GetExtensionDirectory(extensionID))
            Return Task.FromResult(ExtensionUninstallResult.Success)
        End Function

        Private Function GetChildCollections(manager As Core.PluginManager) As IEnumerable(Of IExtensionCollection) Implements IExtensionCollection.GetChildCollections
            Return {}
        End Function
    End Class
End Namespace

