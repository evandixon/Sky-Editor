Namespace IO
    Public Interface ISavableAs
        Inherits ISavable
        ''' <summary>
        ''' Saves the class to the given filename.
        ''' </summary>
        ''' <param name="Filename"></param>
        Overloads Sub Save(Filename As String, provider As IOProvider)
        Function GetDefaultExtension() As String
    End Interface

End Namespace
