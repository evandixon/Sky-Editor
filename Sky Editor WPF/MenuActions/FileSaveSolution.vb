﻿Namespace MenuActions
    Public Class FileSaveSolution
        Inherits MenuAction
        Public Overrides Function SupportedTypes() As IEnumerable(Of Type)
            Return {GetType(Solution)}
        End Function

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item As Solution In Targets
                item.Save()
                item.SaveAllProjects()
            Next
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({PluginHelper.GetLanguageItem("_File"), PluginHelper.GetLanguageItem("_Save"), PluginHelper.GetLanguageItem("Save _Solution")})
        End Sub
    End Class
End Namespace

