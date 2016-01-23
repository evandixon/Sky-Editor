Imports System.Threading.Tasks

Namespace MenuActions
    Public Class FileNewProject
        Inherits MenuAction

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            'Dim _manager = PluginManager.GetInstance
            'Dim newProj As New NewProjectWindow(_manager)
            'If newProj.ShowDialog() Then
            '    _manager.CurrentProject = ProjectOld.CreateProject(newProj.SelectedName, newProj.SelectedLocation, _manager.GetProjectType(newProj.SelectedProjectType), _manager)
            'End If
            Return Task.CompletedTask
        End Function

        Public Sub New()
            MyBase.New({PluginHelper.GetLanguageItem("_File"), PluginHelper.GetLanguageItem("_New"), PluginHelper.GetLanguageItem("_Project")})
            AlwaysVisible = True
        End Sub
    End Class
End Namespace

