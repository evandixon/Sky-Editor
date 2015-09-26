Namespace Interfaces
    ''' <summary>
    ''' Marks a class that supports saving.
    ''' </summary>
    Public Interface iSavable
        ''' <summary>
        ''' Saves the class to the last filename.
        ''' </summary>
        Sub Save()
        ''' <summary>
        ''' Saves the class to the given filename.
        ''' </summary>
        ''' <param name="Filename"></param>
        Sub Save(Filename As String)
    End Interface

End Namespace
