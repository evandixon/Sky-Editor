Namespace Utilities
    Public Class Zip
        ''' <summary>
        ''' Unzips the given zip file to the given directory.
        ''' </summary>
        ''' <param name="fileName">Filename of the zip file to unzip.</param>
        ''' <param name="outputDirectory">Directory to which to extract the zip.</param>
        Public Shared Sub Unzip(fileName As String, outputDirectory As String)
            Dim x As New ICSharpCode.SharpZipLib.Zip.FastZip()
            x.ExtractZip(fileName, outputDirectory, ".*")
        End Sub

        ''' <summary>
        ''' Zips the given directory into a zip file at the given path.
        ''' </summary>
        ''' <param name="sourceDirectory">Directory to zip.</param>
        ''' <param name="outputFileName">Filename of the output zip file.</param>
        Public Shared Sub Zip(sourceDirectory As String, outputFileName As String)
            Dim x As New ICSharpCode.SharpZipLib.Zip.FastZip
            x.CreateZip(outputFileName, sourceDirectory, True, ".*", ".*")
        End Sub
        Private Sub New()

        End Sub
    End Class
End Namespace