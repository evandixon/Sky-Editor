Imports System.IO
Imports System.Reflection
Imports System.Threading.Tasks
Imports SkyEditor
Imports SkyEditor.Core.Extensions
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities

Namespace Extensions
    Public Class ExtensionHelper
        Implements iNamed

        Public ReadOnly Property Name As String Implements iNamed.Name
            Get
                Return "Extensions"
            End Get
        End Property

        Public Shared Async Function InstallExtension(ExtensionZip As String, manager As PluginManager) As Task(Of ExtensionInstallResult)
            Dim provider = manager.CurrentIOProvider
            Dim result As ExtensionInstallResult

            'Get the temporary directory
            Dim tempDir = Path.Combine(PluginHelper.RootResourceDirectory, "Temp", Path.GetFileNameWithoutExtension(ExtensionZip))

            'Get the extension directory
            Dim extDir = Path.Combine(PluginHelper.RootResourceDirectory, "Extensions")

            'Ensure it contains no files
            Await Core.Utilities.FileSystem.ReCreateDirectory(tempDir, provider)

            'Extract the given zip file to it
            Core.Utilities.Zip.Unzip(ExtensionZip, tempDir)

            'Open the info file
            Dim infoFilename As String = IO.Path.Combine(tempDir, "info.skyext")
            If IO.File.Exists(infoFilename) Then
                'Open the file itself
                Dim info = ExtensionInfo.Open(infoFilename, provider)
                'Get the type
                Dim extType = ReflectionHelpers.GetTypeByName(info.ExtensionTypeName, manager)
                'Determine if the type is supported
                If extType Is Nothing OrElse Not ReflectionHelpers.HasDefaultConstructor(extType) Then
                    result = ExtensionInstallResult.UnsupportedFormat
                Else
                    'Create an instance of the extension type and install it
                    Dim extInst As ExtensionType = ReflectionHelpers.CreateInstance(extType)
                    extInst.ExtensionDirectory = extDir
                    extInst.CurrentIOProvider = provider
                    result = Await extInst.InstallExtension(info, tempDir)
                End If
            Else
                result = ExtensionInstallResult.InvalidFormat
            End If

            'Cleanup
            Await Core.Utilities.FileSystem.DeleteDirectory(tempDir, provider)

            Return result
        End Function

        Public Shared Function GetExtensions(ExtensionType As Type, manager As PluginManager) As IEnumerable(Of ExtensionInfo)
            Dim bank As IExtensionCollection = ReflectionHelpers.CreateInstance(ExtensionType.GetTypeInfo)
            Return bank.GetExtensions(manager)
        End Function
    End Class
End Namespace