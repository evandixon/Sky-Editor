Imports System.Windows.Controls
Imports ROMEditor
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SkyEditorWPF.UI

Namespace ObjectControls
    Public Class NDSModSrcEditor
        Inherits ObjectControl

        Public Overrides Sub RefreshDisplay()
            With GetEditingObject() '(Of Mods.ModSourceContainer)()
                txtModName.Text = .ModName
                txtAuthor.Text = .Author
                txtDescription.Text = .Description
                txtUpdateUrl.Text = .UpdateURL
                txtVersion.Text = .Version
                'txtDependenciesBefore.Text = .DependenciesBefore
                'txtDependenciesAfter.Text = .DependenciesAfter
            End With
            IsModified = False
        End Sub

        Public Overrides Sub UpdateObject()
            With GetEditingObject() '(Of Mods.ModSourceContainer)()
                .ModName = txtModName.Text
                .Author = txtAuthor.Text
                .Description = txtDescription.Text
                .UpdateURL = txtUpdateUrl.Text
                .Version = txtVersion.Text
                '.DependenciesBefore = txtDependenciesBefore.Text
                '.DependenciesAfter = txtDependenciesAfter.Text
            End With
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {} '{GetType(Mods.ModSourceContainer)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 1
        End Function

        Private Sub NDSModSrcEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("Modpack Info")
        End Sub

    End Class
End Namespace
