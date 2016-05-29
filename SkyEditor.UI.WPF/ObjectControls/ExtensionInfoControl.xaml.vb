Imports System.Reflection
Imports System.Windows
Imports SkyEditor.Core.Extensions
Imports SkyEditor.UI.WPF

Namespace ObjectControls
    Public Class ExtensionInfoControl
        Inherits ObjectControl

        Public Overrides Sub RefreshDisplay()
            MyBase.RefreshDisplay()
            With GetEditingObject(Of ExtensionInfo)()
                txtAuthor.Text = .Author
                txtDescription.Text = .Description
                txtName.Text = .Name
                txtVersion.Text = .Version
            End With
        End Sub

        Public Overrides Sub UpdateObject()
            MyBase.UpdateObject()
            With GetEditingObject(Of ExtensionInfo)()
                .Author = txtAuthor.Text
                .Description = txtDescription.Text
                .Name = txtName.Text
                .Version = txtVersion.Text
            End With
        End Sub

        Private Sub ExtensionInfoControl_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            lblAuthor.Content = My.Resources.Language.Author
            lblName.Content = My.Resources.Language.Name
            lblDescription.Content = My.Resources.Language.Description
            lblVersion.Content = My.Resources.Language.Version
            Me.Header = My.Resources.Language.ExtensionInfo
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(ExtensionInfo)}
        End Function
    End Class
End Namespace