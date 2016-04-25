Imports System.Net
Imports System.Web.Http

Namespace Controllers
    Public Class ExtensionsController
        Inherits ApiController

        Private Property Context As ApplicationDbContext

        <Route("RootCollections")>
        Public Function GetRootCollections() As IEnumerable(Of ExtensionCollection)
            Return From c In Context.ExtensionCollections Where Not c.ParentCollection.HasValue
        End Function

        Protected Overrides Sub Dispose(disposing As Boolean)
            If disposing AndAlso Context IsNot Nothing Then
                Context.Dispose()
            End If

            MyBase.Dispose(disposing)
        End Sub

        Public Sub New()
            Context = New ApplicationDbContext
        End Sub
    End Class
End Namespace