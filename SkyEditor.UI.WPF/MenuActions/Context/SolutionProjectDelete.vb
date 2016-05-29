Imports System.Reflection
Imports System.Windows
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI

Namespace MenuActions
    Public Class SolutionProjectDelete
        Inherits MenuAction

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item In Targets
                If MessageBox.Show(My.Resources.Language.DeleteItemConfirmation, My.Resources.Language.MainTitle, MessageBoxButton.YesNo) = MessageBoxResult.Yes Then
                    If TypeOf item Is SolutionNode Then
                        DirectCast(item, SolutionNode).DeleteCurrentNode()
                    ElseIf TypeOf item Is ProjectNode Then
                        DirectCast(item, ProjectNode).DeleteCurrentNode()
                    End If
                End If
            Next
            Return Task.CompletedTask
        End Function

        Public Overrides Function SupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(SolutionNode).GetTypeInfo, GetType(ProjectNode).GetTypeInfo}
        End Function

        Public Overrides Function SupportsObject(Obj As Object) As Boolean
            If TypeOf Obj Is SolutionNode Then
                Return DirectCast(Obj, SolutionNode).CanDeleteCurrentNode
            ElseIf TypeOf Obj Is ProjectNode Then
                Return DirectCast(Obj, ProjectNode).CanDeleteCurrentNode
            Else
                Return False
            End If
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuDelete})
            IsContextBased = True
        End Sub
    End Class
End Namespace