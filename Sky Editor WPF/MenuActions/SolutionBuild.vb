Imports System.Threading.Tasks

Namespace MenuActions
    Public Class SolutionBuild
        Inherits MenuAction

        Public Overrides Function SupportedTypes() As IEnumerable(Of Type)
            Return {GetType(Solution)}
        End Function


        Public Overrides Async Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item As Solution In Targets
                Try
                    Await item.Build()
                Catch ex As Exception
                    MessageBox.Show(PluginHelper.GetLanguageItem("Failed to build solution.  See output for details."))
                    PluginHelper.Writeline(ex.ToString, PluginHelper.LineType.Error)
                    PluginHelper.SetLoadingStatusFailed()
                End Try
            Next
        End Function

        Public Sub New()
            MyBase.New({PluginHelper.GetLanguageItem("_Solution"), PluginHelper.GetLanguageItem("_Build")})
        End Sub
    End Class
End Namespace

