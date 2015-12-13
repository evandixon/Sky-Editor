Imports System.Windows.Media
Imports SkyEditorBase

Namespace ObjectControls
    Public Class CteImageControl
        Inherits ObjectControl(Of FileFormats.CteImage)

        Public Overrides Sub RefreshDisplay()
            Dim c As New ImageConverter
            imgPreview.Source = c.Convert(EditingItem.ContainedImage, GetType(ImageSource), Nothing, Globalization.CultureInfo.InvariantCulture)
        End Sub

        Private Sub CteImageControl_EditingItemChanging(sender As Object, e As EventArgs) Handles Me.EditingObjectChanging
            If EditingItem IsNot Nothing Then
                RemoveHandler EditingItem.ContainedImageUpdated, AddressOf EditingItem_ContainedImageChanged
            End If
        End Sub

        Private Sub CteImageControl_EditingItemChanged(sender As Object, e As EventArgs) Handles Me.EditingObjectChanged
            If EditingItem IsNot Nothing Then
                AddHandler EditingItem.ContainedImageUpdated, AddressOf EditingItem_ContainedImageChanged
            End If
        End Sub

        Private Sub EditingItem_ContainedImageChanged(sender As Object, e As EventArgs)
            RefreshDisplay()
        End Sub
    End Class
End Namespace

