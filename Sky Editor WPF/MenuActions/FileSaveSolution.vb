Imports System.Reflection
Imports SkyEditor.Core.UI

Namespace MenuActions
    Public Class FileSaveSolution
        Inherits MenuAction
        Public Overrides Function SupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(SolutionOld).GetTypeInfo}
        End Function

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item As SolutionOld In Targets
                item.Save(PluginManager.GetInstance.CurrentIOProvider)
                item.SaveAllProjects(PluginManager.GetInstance.CurrentIOProvider)
            Next
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuFile, My.Resources.Language.MenuFileSave, My.Resources.Language.MenuFileSaveSolution})
            SortOrder = 1.33
        End Sub
    End Class
End Namespace

