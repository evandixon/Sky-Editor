Imports SkyEditorBase
Namespace Tabs
    Public Class PkmMovesTab
        Inherits ObjectTab

        Public Overrides Sub RefreshDisplay()
            With DirectCast(Me.EditingObject, Interfaces.iPkmAttack)
                Attack1.ObjectToEdit = .Attack1
                Attack2.ObjectToEdit = .Attack2
                Attack3.ObjectToEdit = .Attack3
                Attack4.ObjectToEdit = .Attack4
            End With
        End Sub

        Public Overrides Sub UpdateObject()
            With DirectCast(Me.EditingObject, Interfaces.iPkmAttack)
                .Attack1 = Attack1.ObjectToEdit
                .Attack2 = Attack2.ObjectToEdit
                .Attack3 = Attack3.ObjectToEdit
                .Attack4 = Attack4.ObjectToEdit
            End With
        End Sub
        Public Overrides ReadOnly Property SupportedTypes As Type()
            Get
                Return {GetType(Interfaces.iPkmAttack)}
            End Get
        End Property

        Private Sub PkmMovesTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("Moves")
        End Sub
        Public Overrides ReadOnly Property SortOrder As Integer
            Get
                Return 9
            End Get
        End Property

        Private Sub OnModified(sender As Object, e As EventArgs) Handles Attack1.Modified, Attack2.Modified, Attack3.Modified, Attack4.Modified
            RaiseModified()
        End Sub
    End Class

End Namespace