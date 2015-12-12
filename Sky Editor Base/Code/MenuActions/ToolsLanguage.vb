Imports System.Threading.Tasks

Namespace MenuActions
    Public Class ToolsLanguage
        Inherits MenuAction
        Public Overrides Function SupportsObject(Obj As Object) As Boolean
            Return True
        End Function

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            PluginHelper.RequestFileOpen(Language.LanguageManager.Instance, False)
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({PluginHelper.GetLanguageItem("_Tools"), PluginHelper.GetLanguageItem("_Language Editor")})
            AlwaysVisible = True
        End Sub
    End Class
End Namespace

