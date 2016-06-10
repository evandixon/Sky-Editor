Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Windows
Imports SkyEditor.Core.Windows.Processes

Namespace FileFormats.Explorers
    <Obsolete("Needs rewriting")> Public Class DecomressedFile
        Inherits GenericFile
        'Public Property RawData As Byte()
        Public Shared Async Function RunDecompress(Filename As String) As Task
            Dim romDirectory As String = EnvironmentPaths.GetResourceDirectory
            If Not IO.Directory.Exists(IO.Path.GetDirectoryName(Filename) & "\Decompressed") Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(Filename) & "\Decompressed")
            End If
            Await ConsoleApp.RunProgram(IO.Path.Combine(romDirectory, "ppmd_unpx.exe"),
                                                  String.Format("""{0}"" ""{1}""", Filename, IO.Path.GetDirectoryName(Filename) & "\Decompressed\" & IO.Path.GetFileName(Filename)))
        End Function
        Public Shared Async Function RunCompress(Filename As String) As Task
            Dim romDirectory As String = EnvironmentPaths.GetResourceDirectory
            Await ConsoleApp.RunProgram(IO.Path.Combine(romDirectory, "ppmd_pxcomp.exe"),
                                                  String.Format("""{0}"" ""{1}""", IO.Path.GetDirectoryName(Filename) & "\Decompressed\" & IO.Path.GetFileName(Filename), Filename))
            'Cleanup
            If IO.File.Exists(IO.Path.GetDirectoryName(Filename) & "\Decompressed\" & IO.Path.GetFileName(Filename)) Then
                IO.File.Delete(IO.Path.GetDirectoryName(Filename) & "\Decompressed\" & IO.Path.GetFileName(Filename))
            End If
            If IO.Directory.Exists(IO.Path.GetDirectoryName(Filename) & "\Decompressed\") AndAlso IO.Directory.GetFiles(IO.Path.GetDirectoryName(Filename) & "\Decompressed\").Length = 0 Then
                IO.Directory.Delete(IO.Path.GetDirectoryName(Filename) & "\Decompressed\")
            End If
            If IO.File.Exists(Filename.Replace(IO.Path.GetExtension(Filename), ".pkdpx")) Then
                IO.File.Delete(Filename)
                IO.File.Copy(Filename.Replace(IO.Path.GetExtension(Filename), ".pkdpx"), Filename)
                IO.File.Delete(Filename.Replace(IO.Path.GetExtension(Filename), ".pkdpx"))
            End If
        End Function

        ''' <summary>
        ''' Saves and compresses the DecompressedFile.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Save(Path As String, provider As IOProvider)
            If Not IO.Directory.Exists(IO.Path.GetDirectoryName(Path) & "\Decompressed") Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(Path) & "\Decompressed")
            End If
            MyBase.Save(Path.Replace(IO.Path.GetDirectoryName(Path), IO.Path.GetDirectoryName(Path) & "\Decompressed"), provider)
            'Await RunCompress(Path)
        End Sub

        Public Overrides Async Function OpenFile(Filename As String, Provider As IOProvider) As Task
            Await RunDecompress(Filename)
            Await MyBase.OpenFile(Filename, Provider)
        End Function

        Public Sub New()

        End Sub
    End Class
End Namespace