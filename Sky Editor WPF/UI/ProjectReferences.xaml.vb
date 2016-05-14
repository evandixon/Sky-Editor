Imports System.Reflection

Namespace UI
    Public Class ProjectReferences
        Inherits ObjectControl

        Public Overrides Sub RefreshDisplay()
            MyBase.RefreshDisplay()
            With GetEditingObject(Of ProjectOld)()
                listReferences.ItemsSource = .ProjectReferences
            End With
        End Sub

        Public Overrides Sub UpdateObject()
            MyBase.UpdateObject()
            With GetEditingObject(Of ProjectOld)()
                .ProjectReferences = listReferences.ItemsSource
            End With
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(SkyEditorBase.ProjectOld)}
        End Function
    End Class

End Namespace

