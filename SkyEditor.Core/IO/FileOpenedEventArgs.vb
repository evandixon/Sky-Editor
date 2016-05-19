
Namespace IO
    Public Class FileOpenedEventArguments
        Inherits EventArgs
        Public Property File As Object
        Public Property DisposeOnExit As Boolean

        ''' <summary>
        ''' The project the file was opened from.
        ''' Null if the file is not in a project.
        ''' </summary>
        ''' <returns></returns>
        Public Property ParentProject As Project
        Public Sub New()
            DisposeOnExit = False
        End Sub
    End Class

End Namespace