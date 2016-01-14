Namespace ConsoleCommands
    Public Class BatchCteConvert
        Inherits SkyEditorBase.ConsoleCommand

        Public Overrides Sub Main(Arguments() As String)
            Dim SourceDir = Arguments(0)
            If Not IO.Directory.Exists(SourceDir) Then
                Console.WriteLine($"Invalid dir ""{SourceDir}""")
                Exit Sub
            End If
            For Each item In IO.Directory.GetFiles(SourceDir)
                Try
                    Using c As New FileFormats.CteImage(item)
                        c.ContainedImage.Save(item & ".png", Drawing.Imaging.ImageFormat.Png)
                        Console.WriteLine("Converted " & item)
                    End Using
                Catch ex As Exception
                    Console.WriteLine("Failed " & item)
                    Console.WriteLine(ex)
                End Try
            Next
        End Sub
    End Class

End Namespace
