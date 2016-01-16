Imports System.Threading.Tasks

Namespace MenuActions
    Public Class ToolsSettings
        Inherits MenuAction

        Public Overrides Function SupportsObject(Obj As Object) As Boolean
            Return True
        End Function

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            PluginHelper.RequestFileOpen(SettingsManager.Instance, False)
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({PluginHelper.GetLanguageItem("_Tools"), PluginHelper.GetLanguageItem("_Settings")})
            AlwaysVisible = True
        End Sub
    End Class
End Namespace

