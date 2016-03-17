Namespace UI
    Public Class ExtensionInfoControl
        Inherits ObjectControl

        Public Overrides Sub RefreshDisplay()
            MyBase.RefreshDisplay()
            With GetEditingObject(Of SkyEditorBase.Extensions.ExtensionInfo)()
                txtAuthor.Text = .Author
                txtDescription.Text = .Description
                txtName.Text = .Name
                txtVersion.Text = .Version
            End With
        End Sub

        Public Overrides Sub UpdateObject()
            MyBase.UpdateObject()
            With GetEditingObject(Of SkyEditorBase.Extensions.ExtensionInfo)()
                .Author = txtAuthor.Text
                .Description = txtDescription.Text
                .Name = txtName.Text
                .Version = txtVersion.Text
            End With
        End Sub

        Private Sub ExtensionInfoControl_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            lblAuthor.Content = PluginHelper.GetLanguageItem("Author")
            lblName.Content = PluginHelper.GetLanguageItem("Name")
            lblDescription.Content = PluginHelper.GetLanguageItem("Description")
            lblVersion.Content = PluginHelper.GetLanguageItem("Version")
            Me.Header = PluginHelper.GetLanguageItem("Extension Info")
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(SkyEditorBase.Extensions.ExtensionInfo)}
        End Function
    End Class
End Namespace