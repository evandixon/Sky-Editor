Imports SkyEditor.Core.UI

Namespace MenuActions
    Public Class DevPlugins
        Inherits MenuAction

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            CurrentPluginManager.CurrentIOUIManager.OpenFile(CurrentPluginManager, False)
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuDev, My.Resources.Language.MenuDevPlugins})
            Me.AlwaysVisible = True
            Me.DevOnly = True
            SortOrder = 10.3
        End Sub
    End Class
End Namespace

