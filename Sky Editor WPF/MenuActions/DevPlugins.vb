Namespace MenuActions
    Public Class DevPlugins
        Inherits MenuAction

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            PluginHelper.RequestFileOpen(PluginManager.GetInstance, False)
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New("_Development/_Plugins", TranslateItems:=True)
            Me.AlwaysVisible = True
            Me.DevOnly = False
            SortOrder = 10.3
        End Sub
    End Class
End Namespace

