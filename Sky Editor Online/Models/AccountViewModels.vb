Imports System.Collections.Generic

' Models returned by AccountController actions.
Public Class ExternalLoginViewModel
    Public Property Name As String
    Public Property Url As String
    Public Property State As String
End Class

Public Class ManageInfoViewModel
    Public Property LocalLoginProvider As String
    Public Property Email As String
    Public Property Logins As IEnumerable(Of UserLoginInfoViewModel)
    Public Property ExternalLoginProviders As IEnumerable(Of ExternalLoginViewModel)
End Class

Public Class UserInfoViewModel
    Public Property Email As String
    Public Property HasRegistered As Boolean
    Public Property LoginProvider As String
End Class

Public Class UserLoginInfoViewModel
    Public Property LoginProvider As String
    Public Property ProviderKey As String
End Class
