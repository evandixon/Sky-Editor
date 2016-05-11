Imports SkyEditor.Core.IO

Namespace IO
    ''' <summary>
    ''' Represents a class that can detect whether or not it is the same type as the given file.
    ''' </summary>
    Public Interface IDetectableFileType
        ''' <summary>
        ''' Returns whether or not the given file is of the type that the class represents.
        ''' </summary>
        ''' <param name="File">File to check.</param>
        ''' <returns></returns>
        Function IsOfType(File As GenericFile) As Task(Of Boolean)
    End Interface

End Namespace
