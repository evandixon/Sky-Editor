Imports ROMEditor.Projects
Imports SkyEditor.UI.WPF

Namespace ObjectControls
    Public Class NDSModSrcEditor
        Inherits ObjectControl

        Public Overrides Sub RefreshDisplay()
            With GetEditingObject(Of GenericModProject)()
                txtModName.Text = .ModName
                txtAuthor.Text = .ModAuthor
                txtDescription.Text = .ModDescription
                'txtUpdateUrl.Text = .ModUpdateURL
                txtVersion.Text = .ModVersion
                'txtDependenciesBefore.Text = .DependenciesBefore
                'txtDependenciesAfter.Text = .DependenciesAfter
            End With
            IsModified = False
        End Sub

        Public Overrides Sub UpdateObject()
            With GetEditingObject(Of GenericModProject)()
                .ModName = txtModName.Text
                .ModAuthor = txtAuthor.Text
                .ModDescription = txtDescription.Text
                '.UpdateURL = txtUpdateUrl.Text
                .ModVersion = txtVersion.Text
                '.DependenciesBefore = txtDependenciesBefore.Text
                '.DependenciesAfter = txtDependenciesAfter.Text
            End With
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(GenericModProject)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 1
        End Function

        Private Sub NDSModSrcEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = My.Resources.Language.ModInfo
        End Sub

    End Class
End Namespace
