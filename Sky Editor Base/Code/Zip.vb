Public Class Zip
    Public Shared Sub UnZip(ZipFilename As String, OutputDirectory As String)
        DeveloperConsole.Writeline(String.Format("Unzipping ""{0}"" to ""{1}""", ZipFilename, OutputDirectory))
        If IO.File.Exists(ZipFilename) Then
            Dim x As New ICSharpCode.SharpZipLib.Zip.FastZip()
            x.ExtractZip(ZipFilename, OutputDirectory, ".*")
            DeveloperConsole.Writeline("Unzip complete.")
        Else
            DeveloperConsole.Writeline("Unzip failed, file does not exist.")
        End If

    End Sub
End Class
