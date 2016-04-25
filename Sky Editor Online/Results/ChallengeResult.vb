Imports System.Net
Imports System.Net.Http
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Web.Http
Imports Microsoft.Owin.Security

Public Class ChallengeResult
    Implements IHttpActionResult
    Public Sub New(loginProvider As String, controller As ApiController)
        Me.LoginProvider = loginProvider
        Me.Request = controller.Request
    End Sub

    Public Property LoginProvider As String
    Public Property Request As HttpRequestMessage

    Public Function ExecuteAsync(cancellationToken As CancellationToken) As Task(Of HttpResponseMessage) Implements IHttpActionResult.ExecuteAsync
        Request.GetOwinContext().Authentication.Challenge(LoginProvider)

        Dim response As New HttpResponseMessage(HttpStatusCode.Unauthorized)
        response.RequestMessage = Request
        Return Task.FromResult(response)
    End Function
End Class
