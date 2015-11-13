Imports SkyEditorBase

Namespace ObjectControls
    Public Class NDSModSrcEditor
        Inherits ObjectControl(Of Mods.ModSourceContainer)
        Public Overrides Sub RefreshDisplay()
            With Me.EditingItem
                txtModName.Text = .ModName
                txtAuthor.Text = .Author
                txtDescription.Text = .Description
                txtUpdateUrl.Text = .UpdateURL
                txtVersion.Text = .Version
                'txtDependenciesBefore.Text = .DependenciesBefore
                'txtDependenciesAfter.Text = .DependenciesAfter
            End With
        End Sub

        Public Overrides Sub UpdateObject()
            With Me.EditingItem
                .ModName = txtModName.Text
                .Author = txtAuthor.Text
                .Description = txtDescription.Text
                .UpdateURL = txtUpdateUrl.Text
                .Version = txtVersion.Text
                '.DependenciesBefore = txtDependenciesBefore.Text
                '.DependenciesAfter = txtDependenciesAfter.Text
            End With
        End Sub
    End Class
End Namespace
