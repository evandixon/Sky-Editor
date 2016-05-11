Namespace IO
    ''' <summary>
    ''' A type that can analyze a file and detect its file type.
    ''' </summary>
    Public Interface IFileTypeDetector
        Function DetectFileType(File As GenericFile, Manager As PluginManager) As Task(Of IEnumerable(Of FileTypeDetectionResult))
    End Interface

End Namespace
