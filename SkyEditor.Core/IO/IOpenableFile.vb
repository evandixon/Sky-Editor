Namespace IO
    Public Interface IOpenableFile
        Function OpenFile(Filename As String, Provider As IOProvider) As Task
    End Interface

End Namespace
