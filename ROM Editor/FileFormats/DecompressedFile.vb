Imports SkyEditorBase

Namespace FileFormats
    Public Class DecomressedFile
        Inherits GenericFile
        'Public Property RawData As Byte()
        Public Shared Async Function RunDecompress(Filename As String) As Task(Of Boolean)
            Dim romDirectory As String = PluginHelper.GetResourceDirectory
            If Not IO.Directory.Exists(IO.Path.GetDirectoryName(Filename) & "\Decompressed") Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(Filename) & "\Decompressed")
            End If
            Return Await SkyEditorBase.PluginHelper.RunProgram(IO.Path.Combine(romDirectory, "ppmd_unpx.exe"),
                                                  String.Format("""{0}"" ""{1}""", Filename, IO.Path.GetDirectoryName(Filename) & "\Decompressed\" & IO.Path.GetFileName(Filename)))
        End Function
        Public Shared Sub RunDecompressSync(Filename As String)
            Dim romDirectory As String = PluginHelper.GetResourceDirectory
            If Not IO.Directory.Exists(IO.Path.GetDirectoryName(Filename) & "\Decompressed") Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(Filename) & "\Decompressed")
            End If
            SkyEditorBase.PluginHelper.RunProgramSync(IO.Path.Combine(romDirectory, "ppmd_unpx.exe"),
                                                  String.Format("""{0}"" ""{1}""", Filename, IO.Path.GetDirectoryName(Filename) & "\Decompressed\" & IO.Path.GetFileName(Filename)), False)
        End Sub
        Public Shared Async Function RunCompress(Filename As String) As Task
            Dim romDirectory As String = PluginHelper.GetResourceDirectory
            Await SkyEditorBase.PluginHelper.RunProgram(IO.Path.Combine(romDirectory, "ppmd_pxcomp.exe"),
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
        ''' Returns a DecompressedFile after decompressing the data.
        ''' </summary>
        ''' <param name="Filename"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Async Function FromFilename(Filename As String) As Task(Of DecomressedFile)
            Await RunDecompress(Filename)
            Return New DecomressedFile(Filename)
        End Function
        ''' <summary>
        ''' Saves and compresses the DecompressedFile.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Save(Path As String)
            If Not IO.Directory.Exists(IO.Path.GetDirectoryName(Path) & "\Decompressed") Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(Path) & "\Decompressed")
            End If
            MyBase.Save(Path.Replace(IO.Path.GetDirectoryName(Path), IO.Path.GetDirectoryName(Path) & "\Decompressed"))
            'Await RunCompress(Path)
        End Sub
        Public Sub New(OriginalFilename As String)
            Dim Filename As String = OriginalFilename.Replace("/", "\").Replace(IO.Path.GetDirectoryName(OriginalFilename), IO.Path.GetDirectoryName(OriginalFilename) & "\Decompressed")
            RunDecompressSync(OriginalFilename)

            MyBase.OpenFile(Filename)
        End Sub
        Public Sub New(RawData As Byte())
            Throw New NotImplementedException
            '_tempname = Guid.NewGuid.ToString()
            'IO.File.WriteAllBytes(PluginHelper.GetResourceName(_tempname & ".tmp"), RawData)
            'Me.Filename = PluginHelper.GetResourceName(_tempname & ".tmp")
            'Me.OriginalFilename = Filename
        End Sub
        ' ''' <summary>
        ' ''' Creates a new instance of the decompressed file, given the decompressed data
        ' ''' </summary>
        ' ''' <param name="OriginalFilename"></param>
        ' ''' <remarks></remarks>
        'Protected Sub New(OriginalFilename As String)
        '    Me.RawData = IO.File.ReadAllBytes(OriginalFilename.Replace("/", "\").Replace(".bgp", ".decompressed").Replace(IO.Path.GetDirectoryName(OriginalFilename), IO.Path.GetDirectoryName(OriginalFilename) & "\Decompressed"))
        'End Sub
        ' ''' <summary>
        ' ''' Creates a new instance of the decompressed file, but it must already be decompressed
        ' ''' </summary>
        ' ''' <param name="RawData"></param>
        ' ''' <remarks></remarks>
        'Public Sub New(RawData As Byte())
        '    Me.RawData = RawData
        'End Sub
    End Class
End Namespace