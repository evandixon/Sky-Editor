Imports System.Reflection
Imports System.Windows.Forms
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities

Namespace MenuActions.Context
    Public Class SolutionAddExistingProject
        Inherits MenuAction

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item In Targets
                Dim ParentSolution As Solution
                Dim ParentPath As String

                If TypeOf item Is Solution Then
                    ParentSolution = item
                    ParentPath = ""
                ElseIf TypeOf item Is SolutionNode Then
                    ParentSolution = DirectCast(item, SolutionNode).ParentSolution
                    ParentPath = DirectCast(item, SolutionNode).GetCurrentPath
                Else
                    Throw New ArgumentException(String.Format(My.Resources.Language.ErrorUnsupportedType, item.GetType.Name))
                End If

                Dim w As New OpenFileDialog
                w.Filter = CurrentPluginManager.CurrentIOUIManager.GetProjectIOFilter

                If w.ShowDialog = DialogResult.OK Then
                    ParentSolution.AddExistingProject(ParentPath, w.FileName, CurrentPluginManager)
                End If
            Next
            Return Task.CompletedTask
        End Function

        Public Overrides Function SupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(Solution).GetTypeInfo, GetType(SolutionNode).GetTypeInfo}
        End Function

        Public Overrides Function SupportsObject(Obj As Object) As Boolean
            If TypeOf Obj Is Solution Then
                Return DirectCast(Obj, Solution).CanCreateProject("")
            ElseIf TypeOf Obj Is SolutionNode Then
                Return DirectCast(Obj, SolutionNode).CanCreateChildProject
            Else
                Return False
            End If
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuAddProject})
            IsContextBased = True
        End Sub
    End Class
End Namespace

