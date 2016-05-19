Namespace IO
    Public Class ProjectAddedEventArgs
        Inherits EventArgs

        ''' <summary>
        ''' Path of the solution folder to which the project was added
        ''' </summary>
        ''' <returns></returns>
        Public Property ParentPath As String

        ''' <summary>
        ''' The project that was added
        ''' </summary>
        ''' <returns></returns>
        Public Property Project As Project
    End Class

End Namespace
