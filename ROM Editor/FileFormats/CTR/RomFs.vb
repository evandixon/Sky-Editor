Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats.CTR
    ''' <summary>
    ''' Models a RomFS file, as described at https://www.3dbrew.org/wiki/RomFS
    ''' </summary>
    Public Class RomFs
        Implements iOpenableFile
        Implements iDetectableFileType

        Protected Property Header As RomFSHeader

        Public Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            Using f As New GenericFile(Filename, True)
                Header = New RomFSHeader(f.RawData(0, &H60))



            End Using
        End Sub

        Public Sub New()

        End Sub

        Public Function IsOfType(File As GenericFile) As Boolean Implements iDetectableFileType.IsOfType
            'Check to see if the magic equals "IVFC"
            'The File.Length > 3 part make sure the whole program doesn't crash if the file isn't long enough for the check
            Return File.Length > 3 AndAlso File.RawData(0) = &H49 AndAlso File.RawData(1) = &H56 AndAlso File.RawData(2) = &H46 AndAlso File.RawData(3) = &H43
        End Function
    End Class

End Namespace
