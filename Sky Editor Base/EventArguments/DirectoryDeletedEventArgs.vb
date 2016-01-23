Namespace EventArguments
    Public Class DirectoryDeletedEventArgs
        Inherits EventArgs
        Public Property ParentPath As String
        Public Property DirectoryName As String
        Public Property FullPath As String
    End Class
End Namespace