Imports SkyEditorBase

Namespace ObjectControls
    Public Class NDSModSrcEditor
        Inherits ObjectControl
        Public Overrides Sub RefreshDisplay(Save As Object)
            If TypeOf Save Is FileFormats.NDSModSource Then
                With DirectCast(Save, FileFormats.NDSModSource)
                    txtModName.Text = .ModName
                    txtAuthor.Text = .Author
                    txtDescription.Text = .Description
                    txtUpdateUrl.Text = .UpdateURL
                    txtDependenciesBefore.Text = .DependenciesBefore
                    txtDependenciesAfter.Text = .DependenciesAfter
                End With
            End If
        End Sub

        Public Overrides Function UpdateObject(Obj As Object) As Object
            If TypeOf Obj Is FileFormats.NDSModSource Then
                With DirectCast(Obj, FileFormats.NDSModSource)
                    .ModName = txtModName.Text
                    .Author = txtAuthor.Text
                    .Description = txtDescription.Text
                    .UpdateURL = txtUpdateUrl.Text
                    .DependenciesBefore = txtDependenciesBefore.Text
                    .DependenciesAfter = txtDependenciesAfter.Text
                End With
            End If
            Return Obj
        End Function

        Public Overrides ReadOnly Property SupportedTypes As Type()
            Get
                Return {GetType(FileFormats.NDSModSource)}
            End Get
        End Property
    End Class
End Namespace
