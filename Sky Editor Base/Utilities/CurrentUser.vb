Imports System.Security.Principal

Namespace Utilities
    Public Class CurrentUser
        Public Shared Function IsAdministrator() As Boolean
            Dim user = WindowsIdentity.GetCurrent
            Dim principal As New WindowsPrincipal(user)
            Return principal.IsInRole(WindowsBuiltInRole.Administrator)
        End Function
    End Class

End Namespace
