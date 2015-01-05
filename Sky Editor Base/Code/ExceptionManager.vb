''' <summary>
''' In the future, it would be nice to log exceptions here, instead of displaying them to the user
''' </summary>
''' <remarks></remarks>
Public Class ExceptionManager
    Public Shared Sub LogException(Ex As Exception, Optional Comment As String = "")
        Throw Ex
    End Sub
End Class
