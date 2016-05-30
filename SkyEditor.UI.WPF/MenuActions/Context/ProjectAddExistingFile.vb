Imports System.Reflection
Imports System.Windows.Forms
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities

Namespace MenuActions.Context
    Public Class ProjectAddExistingFile
        Inherits MenuAction

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item In Targets
                Dim CurrentPath As String
                Dim ParentProject As Project
                If TypeOf item Is SolutionNode Then
                    CurrentPath = ""
                    ParentProject = DirectCast(item, SolutionNode).Project
                ElseIf TypeOf item Is ProjectNode Then
                    CurrentPath = DirectCast(item, ProjectNode).GetCurrentPath
                    ParentProject = DirectCast(item, ProjectNode).ParentProject
                Else
                    Throw New ArgumentException(String.Format(My.Resources.Language.ErrorUnsupportedType, item.GetType.Name))
                End If

                Dim w As New OpenFileDialog
                w.Filter = ParentProject.GetImportIOFilter(CurrentPath, CurrentPluginManager)

                If w.ShowDialog = DialogResult.OK Then
                    ParentProject.AddExistingFile(CurrentPath, w.FileName, CurrentPluginManager.CurrentIOProvider)
                End If
            Next
            Return Task.CompletedTask
        End Function

        Public Overrides Function SupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(SolutionNode).GetTypeInfo, GetType(ProjectNode).GetTypeInfo}
        End Function

        Public Overrides Function SupportsObject(Obj As Object) As Boolean
            If TypeOf Obj Is ProjectNode Then
                Return DirectCast(Obj, ProjectNode).IsDirectory AndAlso DirectCast(Obj, ProjectNode).CanCreateFile
            ElseIf TypeOf Obj Is SolutionNode Then
                Return Not DirectCast(Obj, SolutionNode).IsDirectory AndAlso DirectCast(Obj, SolutionNode).Project.CanAddExistingFile("")
            Else
                Return False
            End If
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuAddFile})
            IsContextBased = True
        End Sub
    End Class
End Namespace

