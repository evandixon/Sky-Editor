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
        ''' <summary>
        ''' Raised when the file is saved.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Event FileSaved(sender As Object, e As EventArgs)
    End Interface

End Namespace
