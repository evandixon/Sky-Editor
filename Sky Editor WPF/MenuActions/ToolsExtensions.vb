﻿Namespace MenuActions
    Public Class ToolsExtensions
        Inherits MenuAction

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            PluginHelper.RequestFileOpen(New SkyEditorBase.Extensions.ExtensionHelper, False)
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuTools, My.Resources.Language.MenuToolsExtensions})
            AlwaysVisible = True
            SortOrder = 3.2
        End Sub
    End Class
End Namespace

