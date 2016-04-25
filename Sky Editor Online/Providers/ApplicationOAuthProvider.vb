Imports System.Security.Claims
Imports System.Threading.Tasks
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.EntityFramework
Imports Microsoft.AspNet.Identity.Owin
Imports Microsoft.Owin.Security
Imports Microsoft.Owin.Security.Cookies
Imports Microsoft.Owin.Security.OAuth

Public Class ApplicationOAuthProvider
    Inherits OAuthAuthorizationServerProvider
    Private ReadOnly _publicClientId As String

    Public Sub New(publicClientId As String)
        If publicClientId Is Nothing Then
            Throw New ArgumentNullException("publicClientId")
        End If

        _publicClientId = publicClientId
    End Sub

    Public Overrides Async Function GrantResourceOwnerCredentials(context As OAuthGrantResourceOwnerCredentialsContext) As Task
        Dim userManager As ApplicationUserManager = context.OwinContext.GetUserManager(Of ApplicationUserManager)()
        Dim user As ApplicationUser = Await userManager.FindAsync(context.UserName, context.Password)

        If user Is Nothing Then
            context.SetError("invalid_grant", "The user name or password is incorrect.")
            Return
        End If

        Dim oAuthIdentity As ClaimsIdentity = Await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType)
        Dim cookiesIdentity As ClaimsIdentity = Await user.GenerateUserIdentityAsync(userManager, CookieAuthenticationDefaults.AuthenticationType)

        Dim properties As AuthenticationProperties = CreateProperties(user.UserName)
        Dim ticket As New AuthenticationTicket(oAuthIdentity, properties)
        context.Validated(ticket)
        context.Request.Context.Authentication.SignIn(cookiesIdentity)
    End Function

    Public Overrides Function TokenEndpoint(context As OAuthTokenEndpointContext) As Task
        For Each [property] As KeyValuePair(Of String, String) In context.Properties.Dictionary
            context.AdditionalResponseParameters.Add([property].Key, [property].Value)
        Next

        Return Task.FromResult(Of Object)(Nothing)
    End Function

    Public Overrides Function ValidateClientAuthentication(context As OAuthValidateClientAuthenticationContext) As Task
        ' Resource owner password credentials does not provide a client ID.
        If context.ClientId Is Nothing Then
            context.Validated()
        End If

        Return Task.FromResult(Of Object)(Nothing)
    End Function

    Public Overrides Function ValidateClientRedirectUri(context As OAuthValidateClientRedirectUriContext) As Task
      If context.ClientId = _publicClientId Then
          Dim expectedRootUri As New Uri(context.Request.Uri, "/")

          If expectedRootUri.AbsoluteUri = context.RedirectUri Then
              context.Validated()
          End If
      End If

      Return Task.FromResult(Of Object)(Nothing)
    End Function

    Public Shared Function CreateProperties(userName As String) As AuthenticationProperties
      Dim data As IDictionary(Of String, String) = New Dictionary(Of String, String)() From {
          {"userName", userName}
      }
      Return New AuthenticationProperties(data)
    End Function
End Class
