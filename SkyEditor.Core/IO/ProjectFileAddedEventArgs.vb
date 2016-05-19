Namespace IO
    Public Class ProjectFileAddedEventArgs
        Inherits EventArgs
        Public Property ParentPath As String
        Public Property File As Object

        ''' <summary>
        ''' Name of the file.
        ''' </summary>
        ''' <returns></returns>
        Public Property Filename As String

        ''' <summary>
        ''' Physical path of the newly added file.
        ''' </summary>
        ''' <returns></returns>
        Public Property FullFilename As String
    End Class
End Namespace

