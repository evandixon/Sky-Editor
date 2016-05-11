Namespace IO
    ''' <summary>
    ''' The result of a file type detection.
    ''' </summary>
    Public Class FileTypeDetectionResult

        ''' <summary>
        ''' The type of the class that can model the file in question.
        ''' </summary>
        ''' <returns></returns>
        Public Property FileType As Reflection.TypeInfo

        ''' <summary>
        ''' The percentage chance that this file type can model the file in question.
        ''' </summary>
        ''' <remarks>The FileTypeDetectionResult with the highest MatchChance will be used, and if there are duplicates (e.g. two FileTypeDetectionResult instances with a chance of 1.0), the user may be prompted.</remarks>
        ''' <returns></returns>
        Public Property MatchChance As Single
    End Class
End Namespace

