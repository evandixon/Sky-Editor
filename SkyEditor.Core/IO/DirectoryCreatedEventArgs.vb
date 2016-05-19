Namespace IO
    Public Class DirectoryCreatedEventArgs
        Inherits EventArgs
        ''' <summary>
        ''' Name of the directory that was created.
        ''' </summary>
        ''' <returns></returns>
        Public Property DirectoryName As String

        ''' <summary>
        ''' Path of the directory that the newly created directory is inside.
        ''' </summary>
        ''' <returns></returns>
        Public Property ParentPath As String

        ''' <summary>
        ''' Full path of the new directory.
        ''' </summary>
        ''' <returns></returns>
        Public Property FullPath As String
    End Class
End Namespace

