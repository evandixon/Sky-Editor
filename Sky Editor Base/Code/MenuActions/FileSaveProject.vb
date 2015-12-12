Imports System.Threading.Tasks

Namespace MenuActions
    Public Class FileSaveProject
        Inherits MenuAction

        Public Overrides Function SupportedTypes() As IEnumerable(Of Type)
            Return {GetType(Project)}
        End Function

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item As Project In Targets
                item.SaveProject()
            Next
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({PluginHelper.GetLanguageItem("_File"), PluginHelper.GetLanguageItem("_Save"), PluginHelper.GetLanguageItem("Save _Project")})
        End Sub
    End Class
End Namespace

