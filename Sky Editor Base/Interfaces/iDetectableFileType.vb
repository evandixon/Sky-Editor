Namespace Interfaces
    ''' <summary>
    ''' Represents a class that can detect whether or not it is the same type as the given file.
    ''' </summary>
    Public Interface iDetectableFileType
        ''' <summary>
        ''' Returns whether or not the given file is of the type that the class represents.
        ''' </summary>
        ''' <param name="File">File to check.</param>
        ''' <returns></returns>
        Function IsOfType(File As GenericFile) As Boolean
    End Interface

End Namespace
