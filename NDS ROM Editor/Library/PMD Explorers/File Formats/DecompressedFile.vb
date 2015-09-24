Imports SkyEditorBase

Namespace FileFormats
    Public Class DecomressedFile
        Public Property RawData As Byte()
        Public Shared Async Function RunDecompress(Filename As String) As Task(Of Boolean)
            Dim romDirectory As String = PluginHelper.GetResourceDirectory
            If Not IO.Directory.Exists(IO.Path.GetDirectoryName(Filename) & "\Decompressed") Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(Filename) & "\Decompressed")
            End If
            Return Await SkyEditorBase.PluginHelper.RunProgram(IO.Path.Combine(romDirectory, "ppmd_unpx.exe"),
                                                  String.Format("""{0}"" ""{1}""", Filename, IO.Path.GetDirectoryName(Filename) & "\Decompressed\" & IO.Path.GetFileNameWithoutExtension(Filename) & ".decompressed"))
        End Function
        Public Shared Async Function RunCompress(Filename As String) As Task
            Dim romDirectory As String = PluginHelper.GetResourceDirectory
            Await SkyEditorBase.PluginHelper.RunProgram(IO.Path.Combine(romDirectory, "ppmd_pxcomp.exe"),
                                                  String.Format("""{0}"" ""{1}""", IO.Path.GetDirectoryName(Filename) & "\Decompressed\" & IO.Path.GetFileName(Filename), Filename))
            'If Not IO.File.Exists(Filename) AndAlso IO.File.Exists(Filename.Replace(IO.Path.GetExtension(Filename), ".pkdpx")) Then
            IO.File.Delete(Filename)
            IO.File.Move(Filename.Replace(IO.Path.GetExtension(Filename), ".pkdpx"), Filename)
            'End If
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
        Public Async Function Save(Filename As String) As Task
            If Not IO.Directory.Exists(IO.Path.GetDirectoryName(Filename) & "\Decompressed") Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(Filename) & "\Decompressed")
            End If
            IO.File.WriteAllBytes(Filename.Replace(IO.Path.GetDirectoryName(Filename), IO.Path.GetDirectoryName(Filename) & "\Decompressed"), RawData)
            Await RunCompress(Filename)
        End Function
        ''' <summary>
        ''' Creates a new instance of the decompressed file, given the decompressed data
        ''' </summary>
        ''' <param name="OriginalFilename"></param>
        ''' <remarks></remarks>
        Protected Sub New(OriginalFilename As String)
            Me.RawData = IO.File.ReadAllBytes(OriginalFilename.Replace("/", "\").Replace(".bgp", ".decompressed").Replace(IO.Path.GetDirectoryName(OriginalFilename), IO.Path.GetDirectoryName(OriginalFilename) & "\Decompressed"))
        End Sub
        ''' <summary>
        ''' Creates a new instance of the decompressed file, but it must already be decompressed
        ''' </summary>
        ''' <param name="RawData"></param>
        ''' <remarks></remarks>
        Public Sub New(RawData As Byte())
            Me.RawData = RawData
        End Sub
    End Class
End Namespace