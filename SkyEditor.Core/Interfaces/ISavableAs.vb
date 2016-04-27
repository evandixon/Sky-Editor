Namespace Interfaces
    Public Interface ISavableAs
        Inherits iSavable
        ''' <summary>
        ''' Saves the class to the given filename.
        ''' </summary>
        ''' <param name="Filename"></param>
        Overloads Sub Save(Filename As String)
        Function GetDefaultExtension() As String
    End Interface

End Namespace
