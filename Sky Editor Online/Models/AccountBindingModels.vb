Imports System.ComponentModel.DataAnnotations
Imports Newtonsoft.Json

' Models used as parameters to AccountController actions.

Public Class AddExternalLoginBindingModel
    <Required>
    <Display(Name := "External access token")>
    Public Property ExternalAccessToken As String
End Class

Public Class ChangePasswordBindingModel
    <Required>
    <DataType(DataType.Password)>
    <Display(Name := "Current password")>
    Public Property OldPassword As String

    <Required>
    <StringLength(100, ErrorMessage := "The {0} must be at least {2} characters long.", MinimumLength := 6)>
    <DataType(DataType.Password)>
    <Display(Name := "New password")>
    Public Property NewPassword As String

    <DataType(DataType.Password)>
    <Display(Name := "Confirm new password")>
    <Compare("NewPassword", ErrorMessage := "The new password and confirmation password do not match.")>
    Public Property ConfirmPassword As String
End Class

Public Class RegisterBindingModel
    <Required>
    <Display(Name := "Email")>
    Public Property Email As String

    <Required>
    <StringLength(100, ErrorMessage := "The {0} must be at least {2} characters long.", MinimumLength := 6)>
    <DataType(DataType.Password)>
    <Display(Name := "Password")>
    Public Property Password As String

    <DataType(DataType.Password)>
    <Display(Name := "Confirm password")>
    <Compare("Password", ErrorMessage := "The password and confirmation password do not match.")>
    Public Property ConfirmPassword As String
End Class

Public Class RegisterExternalBindingModel
    <Required>
    <Display(Name := "Email")>
    Public Property Email As String
End Class

Public Class RemoveLoginBindingModel
    <Required>
    <Display(Name := "Login provider")>
    Public Property LoginProvider As String

    <Required>
    <Display(Name := "Provider key")>
    Public Property ProviderKey As String
End Class

Public Class SetPasswordBindingModel
    <Required>
    <StringLength(100, ErrorMessage := "The {0} must be at least {2} characters long.", MinimumLength := 6)>
    <DataType(DataType.Password)>
    <Display(Name := "New password")>
    Public Property NewPassword As String

    <DataType(DataType.Password)>
    <Display(Name := "Confirm new password")>
    <Compare("NewPassword", ErrorMessage := "The new password and confirmation password do not match.")>
    Public Property ConfirmPassword As String
End Class
