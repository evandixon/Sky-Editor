Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SkyEditorWPF.UI
Imports SkyEditor.SaveEditor

Namespace Tabs
    Public Class PkmMovesTab
        Inherits ObjectControl

        Public Overrides Sub RefreshDisplay()
            With GetEditingObject(Of Interfaces.iPkmAttack)()
                Attack1.ObjectToEdit = .Attack1
                Attack2.ObjectToEdit = .Attack2
                Attack3.ObjectToEdit = .Attack3
                Attack4.ObjectToEdit = .Attack4
            End With
        End Sub

        Public Overrides Sub UpdateObject()
            With GetEditingObject(Of Interfaces.iPkmAttack)()
                .Attack1 = Attack1.ObjectToEdit
                .Attack2 = Attack2.ObjectToEdit
                .Attack3 = Attack3.ObjectToEdit
                .Attack4 = Attack4.ObjectToEdit
            End With
        End Sub

        Private Sub PkmMovesTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = My.Resources.Language.Moves
        End Sub

        Private Sub OnModified(sender As Object, e As EventArgs) Handles Attack1.Modified, Attack2.Modified, Attack3.Modified, Attack4.Modified
            IsModified = True
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(Interfaces.iPkmAttack)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 1
        End Function

    End Class

End Namespace