Namespace MenuActions
    Public Class ToolsPlugins
        Inherits MenuAction

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            PluginHelper.RequestFileOpen(PluginManager.GetInstance, False)
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New("_Tools/_Plugins", TranslateItems:=True)
            Me.AlwaysVisible = True
            SortOrder = 9
        End Sub
    End Class
End Namespace

