Namespace IO
    Public Interface IDirectoryTypeDetector
        Function DetectDirectoryType(Path As String) As Task(Of IEnumerable(Of FileTypeDetectionResult))
    End Interface
End Namespace

