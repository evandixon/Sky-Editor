Namespace IO
    Public Class DirectoryNotFoundException
        Inherits Exception

        Public Sub New()
            MyBase.New
        End Sub

        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
    End Class
End Namespace

