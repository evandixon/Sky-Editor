Imports System.Threading.Tasks

Namespace Extensions
    Public Class ExtensionHelper
        Public Shared Async Function InstallExtension(ExtensionZip As String) As Task(Of ExtensionInstallResult)
            Dim result As ExtensionInstallResult
            Dim tempDir = IO.Path.Combine(PluginHelper.RootResourceDirectory, "Temp", IO.Path.GetFileNameWithoutExtension(ExtensionZip))
            Await Utilities.FileSystem.ReCreateDirectory(tempDir)
            Utilities.Zip.UnZip(ExtensionZip, tempDir)
            Dim infoFilename As String = IO.Path.Combine(tempDir, "info.skyext")
            If IO.File.Exists(infoFilename) Then
                Dim info = ExtensionInfo.Open(infoFilename)
                Dim extType = Utilities.ReflectionHelpers.GetTypeFromName(info.ExtensionTypeName)
                If extType Is Nothing OrElse extType.GetConstructor({}) Is Nothing Then
                    result = ExtensionInstallResult.UnsupportedFormat
                Else
                    Dim extInst As ExtensionType = extType.GetConstructor({}).Invoke({})
                    result = Await extInst.InstallExtension(info, tempDir)
                End If
            Else
                result = ExtensionInstallResult.InvalidFormat
            End If
            Await Utilities.FileSystem.DeleteDirectory(tempDir)
            Return result
        End Function

        Public Shared Function GetExtensions(ExtensionType As Type) As IEnumerable(Of ExtensionInfo)
            Dim bank As ExtensionType = ExtensionType.GetConstructor({}).Invoke({})
            Return bank.GetInstalledExtensions
        End Function
    End Class
End Namespace