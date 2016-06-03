Imports System.Reflection
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities

Namespace MenuActions.Context
    Public Class ProjectNewFile
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
                Dim w As New NewFileWindow
                Dim types As New Dictionary(Of String, Type)
                For Each supported In ParentProject.GetSupportedFileTypes(CurrentPath, CurrentPluginManager)
                    types.Add(ReflectionHelpers.GetTypeFriendlyName(supported), supported)
                Next
                w.SetGames(types)
                If w.ShowDialog Then
                    ParentProject.CreateFile(CurrentPath, w.SelectedName, w.SelectedType)
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
                Return Not DirectCast(Obj, SolutionNode).IsDirectory AndAlso DirectCast(Obj, SolutionNode).Project.CanCreateFile("")
            Else
                Return False
            End If
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuCreateFile})
            IsContextBased = True
        End Sub
    End Class
End Namespace

