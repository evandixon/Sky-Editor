Imports System.Windows
Imports System.Windows.Controls

Namespace ObjectControls
    Public Class ModpackInfoControl
        Inherits SkyEditorBase.ObjectControl(Of ModpackInfo)
        Public Overrides Sub RefreshDisplay()
            MyBase.RefreshDisplay()
            With EditingItem
                txtName.Text = .Name
                txtShortName.Text = .ShortName
                txtAuthor.Text = .Author
                txtVersion.Text = .Version
                txtGameCode.Text = .GameCode
            End With
        End Sub

        Public Overrides Sub UpdateObject()
            MyBase.UpdateObject()
            With EditingItem
                .Name = txtName.Text
                .ShortName = txtShortName.Text
                .Author = txtAuthor.Text
                .Version = txtVersion.Text
                .GameCode = txtGameCode.Text
            End With
        End Sub

        Private Sub ModpackInfoControl_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            SkyEditorBase.PluginHelper.TranslateForm(Me)
        End Sub

        Private Sub TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtAuthor.TextChanged, txtGameCode.TextChanged, txtName.TextChanged, txtVersion.TextChanged
            RaiseModified()
        End Sub
    End Class
End Namespace

