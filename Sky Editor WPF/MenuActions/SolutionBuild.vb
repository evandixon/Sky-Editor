Imports System.Reflection
Imports System.Threading.Tasks
Imports SkyEditor.Core.UI

Namespace MenuActions
    Public Class SolutionBuild
        Inherits MenuAction

        Public Overrides Function SupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(SolutionOld).GetTypeInfo}
        End Function


        Public Overrides Async Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item As SolutionOld In Targets
                Try
                    Await item.Build()
                Catch ex As Exception
                    MessageBox.Show(My.Resources.Language.SolutionBuildFailedSeeOutput)
                    PluginHelper.Writeline(ex.ToString, PluginHelper.LineType.Error)
                    PluginHelper.SetLoadingStatusFailed()
                End Try
            Next
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuSolution, My.Resources.Language.MenuSolutionBuild})
            SortOrder = 2.1
        End Sub
    End Class
End Namespace

