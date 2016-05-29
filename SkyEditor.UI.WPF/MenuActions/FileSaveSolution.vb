Imports System.Reflection
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI

Namespace MenuActions
    Public Class FileSaveSolution
        Inherits MenuAction
        Public Overrides Function SupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(Solution).GetTypeInfo}
        End Function

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item As Solution In Targets
                item.Save(CurrentPluginManager.CurrentIOProvider)
                item.SaveAllProjects(CurrentPluginManager.CurrentIOProvider)
            Next
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuFile, My.Resources.Language.MenuFileSave, My.Resources.Language.MenuFileSaveSolution})
            SortOrder = 1.33
        End Sub
    End Class
End Namespace

