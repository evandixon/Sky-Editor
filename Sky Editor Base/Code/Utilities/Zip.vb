Namespace Utilities
    Public Class Zip
        Public Shared Sub UnZip(ZipFilename As String, OutputDirectory As String)
            PluginHelper.Writeline(String.Format("Unzipping ""{0}"" to ""{1}""", ZipFilename, OutputDirectory))
            If IO.File.Exists(ZipFilename) Then
                Dim x As New ICSharpCode.SharpZipLib.Zip.FastZip()
                x.ExtractZip(ZipFilename, OutputDirectory, ".*")
                PluginHelper.Writeline("Unzip complete.")
            Else
                PluginHelper.Writeline("Unzip failed, file does not exist.")
            End If

        End Sub
        Public Shared Sub Zip(SourceDirectory As String, OutputFilename As String)
            Dim x As New ICSharpCode.SharpZipLib.Zip.FastZip
            x.CreateZip(OutputFilename, SourceDirectory, True, ".*", ".*")
        End Sub
    End Class
End Namespace