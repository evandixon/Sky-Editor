Imports System.Reflection
Imports SkyEditor.Core.IO

Namespace UI
    Public Class ProjectReferences
        Inherits ObjectControl

        Public Overrides Sub RefreshDisplay()
            MyBase.RefreshDisplay()
            With GetEditingObject(Of Project)()
                listReferences.ItemsSource = .ProjectReferences
            End With
        End Sub

        Public Overrides Sub UpdateObject()
            MyBase.UpdateObject()
            With GetEditingObject(Of Project)()
                .ProjectReferences = listReferences.ItemsSource
            End With
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(Project)}
        End Function
    End Class

End Namespace

