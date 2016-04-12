Imports System.Windows
Imports System.Windows.Controls
Imports ROMEditor
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SkyEditorWPF.UI

Namespace ObjectControls
    Public Class ModpackInfoControl
        Inherits ObjectControl
        Public Overrides Sub RefreshDisplay()
            With GetEditingObject(Of ModpackInfo)()
                txtName.Text = .Name
                txtShortName.Text = .ShortName
                txtAuthor.Text = .Author
                txtVersion.Text = .Version
                txtGameCode.Text = .GameCode
            End With
            IsModified = False
        End Sub

        Public Overrides Sub UpdateObject()
            With GetEditingObject(Of ModpackInfo)()
                .Name = txtName.Text
                .ShortName = txtShortName.Text
                .Author = txtAuthor.Text
                .Version = txtVersion.Text
                .GameCode = txtGameCode.Text
            End With
        End Sub

        Private Sub ModpackInfoControl_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = My.Resources.Language.ModInfo
        End Sub

        Private Sub TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtAuthor.TextChanged, txtGameCode.TextChanged, txtName.TextChanged, txtVersion.TextChanged
            IsModified = True
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(ModpackInfo)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 0
        End Function

    End Class
End Namespace

