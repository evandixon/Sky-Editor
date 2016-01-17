Imports System.Threading.Tasks

Namespace MenuActions
    Public Class ProjectBuild
        Inherits MenuAction

        Public Overrides Function SupportedTypes() As IEnumerable(Of Type)
            Return {GetType(Project)}
        End Function

        Public Overrides Function SupportsObject(Obj As Object) As Boolean
            Return (Obj IsNot Nothing AndAlso TypeOf Obj Is Project AndAlso DirectCast(Obj, Project).CanBuild)
        End Function

        Public Overrides Async Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item As Project In Targets
                Try
                    Await item.BuildAsync()
                Catch ex As Exception
                    MessageBox.Show(PluginHelper.GetLanguageItem("Failed to build project.  See output for details."))
                    PluginHelper.Writeline(ex.ToString, PluginHelper.LineType.Error)
                    PluginHelper.SetLoadingStatusFailed()
                End Try
            Next
        End Function

        Public Sub New()
            MyBase.New({PluginHelper.GetLanguageItem("_Project"), PluginHelper.GetLanguageItem("Build")})
        End Sub
    End Class
End Namespace

