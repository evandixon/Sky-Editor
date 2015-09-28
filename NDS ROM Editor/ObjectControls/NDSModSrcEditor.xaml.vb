Imports SkyEditorBase

Namespace ObjectControls
    Public Class NDSModSrcEditor
        Inherits ObjectControl
        Public Overrides Sub RefreshDisplay()
            If TypeOf Me.EditingObject() Is FileFormats.NDSModSourceContainer Then
                With DirectCast(Me.EditingObject(), FileFormats.NDSModSourceContainer)
                    txtModName.Text = .ModName
                    txtAuthor.Text = .Author
                    txtDescription.Text = .Description
                    txtUpdateUrl.Text = .UpdateURL
                    'txtDependenciesBefore.Text = .DependenciesBefore
                    'txtDependenciesAfter.Text = .DependenciesAfter
                End With
            End If
        End Sub

        Public Overrides Sub UpdateObject()
            If TypeOf Me.EditingObject() Is FileFormats.NDSModSourceContainer Then
                With DirectCast(Me.EditingObject(), FileFormats.NDSModSourceContainer)
                    .ModName = txtModName.Text
                    .Author = txtAuthor.Text
                    .Description = txtDescription.Text
                    .UpdateURL = txtUpdateUrl.Text
                    '.DependenciesBefore = txtDependenciesBefore.Text
                    '.DependenciesAfter = txtDependenciesAfter.Text
                End With
            End If
        End Sub

        Public Overrides ReadOnly Property SupportedTypes As Type()
            Get
                Return {GetType(FileFormats.NDSModSourceContainer)}
            End Get
        End Property
    End Class
End Namespace
