Imports System.Reflection
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI

Namespace MenuActions
    Public Class SolutionProjectProperties
        Inherits MenuAction

        Public Overrides Function DoAction(Targets As IEnumerable(Of Object)) As Task
            For Each item In Targets
                If TypeOf item Is Solution Then
                    CurrentPluginManager.CurrentIOUIManager.OpenFile(item, False)
                ElseIf TypeOf item Is SolutionNode Then
                    If Not DirectCast(item, SolutionNode).IsDirectory Then
                        CurrentPluginManager.CurrentIOUIManager.OpenFile(DirectCast(item, SolutionNode).Project, False)
                    End If
                End If
            Next
            Return Task.CompletedTask
        End Function

        Public Overrides Function SupportedTypes() As IEnumerable(Of TypeInfo)
            Return {GetType(Solution).GetTypeInfo, GetType(SolutionNode).GetTypeInfo}
        End Function

        Public Overrides Function SupportsObject(Obj As Object) As Boolean
            If TypeOf Obj Is Solution Then
                Return True
            ElseIf TypeOf Obj Is SolutionNode Then
                Return Not DirectCast(Obj, SolutionNode).IsDirectory 'Is this a project?
            Else
                Return False
            End If
        End Function

        Public Sub New()
            MyBase.New({My.Resources.Language.MenuProperties})
            Me.IsContextBased = True
        End Sub
    End Class
End Namespace

