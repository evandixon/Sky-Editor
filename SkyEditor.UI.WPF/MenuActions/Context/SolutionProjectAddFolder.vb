Imports System.Reflection
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI

Namespace MenuActions.Context
    Public Class SolutionProjectAddFolder
        Inherits MenuAction

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each obj In Targets
                Dim w As New NewNameWindow(My.Resources.Language.NewFolderQuestion, My.Resources.Language.NewFolder)
                If w.ShowDialog Then
                    If TypeOf obj Is Solution Then
                        DirectCast(obj, Solution).CreateDirectory("", w.SelectedName)
                    ElseIf TypeOf obj Is SolutionNode Then
                        DirectCast(obj, SolutionNode).CreateChildDirectory(w.SelectedName)
                    ElseIf TypeOf obj Is ProjectNode Then
                        DirectCast(obj, ProjectNode).CreateChildDirectory(w.SelectedName)
                    End If
                End If
            Next
            Return Task.CompletedTask
        End Function

        Public Overrides Function SupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(Solution).GetTypeInfo, GetType(SolutionNode).GetTypeInfo, GetType(ProjectNode).GetTypeInfo}
        End Function

        Public Overrides Function SupportsObject(Obj As Object) As Boolean
            If TypeOf Obj Is Solution Then
                Return DirectCast(Obj, Solution).CanCreateDirectory("")
            ElseIf TypeOf Obj Is SolutionNode Then
                Return DirectCast(Obj, SolutionNode).CanCreateChildDirectory
            ElseIf TypeOf Obj Is ProjectNode Then
                Return DirectCast(Obj, ProjectNode).CanCreateChildDirectory
            Else
                Return False
            End If
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.ContextMenuAddFolder})
            Me.IsContextBased = True
        End Sub
    End Class

End Namespace
