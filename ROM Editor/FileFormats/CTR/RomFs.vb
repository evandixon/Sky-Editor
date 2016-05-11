Imports SkyEditor.Core
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats.CTR
    ''' <summary>
    ''' Models a RomFS file, as described at https://www.3dbrew.org/wiki/RomFS
    ''' </summary>
    Public Class RomFs
        Implements IOpenableFile
        Implements iDetectableFileType

        Protected Property Header As RomFSHeader

        Public Function OpenFile(Filename As String, Provider As IOProvider) As Task Implements IOpenableFile.OpenFile
            Using f As New SkyEditor.Core.Windows.GenericFile
                f.IsReadOnly = True
                f.OpenFile(Filename)
                Header = New RomFSHeader(f.RawData(0, &H60))



            End Using
            Return Task.CompletedTask
        End Function

        Public Sub New()

        End Sub

        Public Function IsOfType(File As GenericFile) As Boolean Implements iDetectableFileType.IsOfType
            'Check to see if the magic equals "IVFC"
            'The File.Length > 3 part make sure the whole program doesn't crash if the file isn't long enough for the check
            Return File.Length > 3 AndAlso File.RawData(0) = &H49 AndAlso File.RawData(1) = &H56 AndAlso File.RawData(2) = &H46 AndAlso File.RawData(3) = &H43
        End Function
    End Class

End Namespace
