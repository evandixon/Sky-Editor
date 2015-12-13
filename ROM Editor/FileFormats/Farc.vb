Imports SkyEditorBase.Interfaces

Namespace FileFormats
    Public Class Farc
        Inherits SkyEditorBase.GenericFile
        Implements iOpenableFile

        ''' <summary>
        ''' Matches offsets to lengths
        ''' </summary>
        ''' <returns></returns>
        Private Property RawOffsets As Dictionary(Of Integer, Integer)

        Public Function GetSubFile(Index As Integer) As Byte()
            Return RawData(RawOffsets.Keys(Index), RawOffsets(RawOffsets.Keys(Index)))
        End Function

        Public Function GetSubfileCount() As Integer
            Return RawOffsets.Count
        End Function

        Public Sub ExtractContents()
            Dim destDir = IO.Path.Combine(IO.Path.GetDirectoryName(Me.OriginalFilename), IO.Path.GetFileNameWithoutExtension(Me.OriginalFilename))
            If Not IO.Directory.Exists(destDir) Then
                IO.Directory.CreateDirectory(destDir)
            End If
            For count = 0 To GetSubfileCount() - 1
                IO.File.WriteAllBytes(IO.Path.Combine(destDir, count.ToString & ".bin"), GetSubFile(count))
            Next
        End Sub

        Public Sub New()
            MyBase.New
        End Sub

        Public Overrides Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            MyBase.OpenFile(Filename)
            RawOffsets = New Dictionary(Of Integer, Integer)
            Dim numEntries = BitConverter.ToInt32(RawData(&H20, 4), 0)
            Dim offsets As New List(Of Integer)
            For count = 0 To numEntries - 1
                offsets.Add(BitConverter.ToInt32(RawData(&H24 + count * 4, 4), 0))
            Next
            If offsets.Count > 0 Then
                Dim total As Integer = 0
                For count = 0 To offsets.Count - 2
                    total += offsets(count)
                    RawOffsets.Add(total, offsets(count + 1))
                Next
                RawOffsets.Add(offsets.Last, Length - offsets.Last)
            End If
        End Sub
    End Class
End Namespace

