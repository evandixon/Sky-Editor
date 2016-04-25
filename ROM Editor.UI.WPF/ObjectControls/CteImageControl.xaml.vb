Imports System.Windows.Controls
Imports System.Windows.Media
Imports ROMEditor
Imports ROMEditor.FileFormats
Imports ROMEditor.FileFormats.PSMD
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SkyEditorWPF.UI

Namespace ObjectControls
    Public Class CteImageControl
        Inherits ObjectControl

        Public Overrides Sub RefreshDisplay()
            Dim c As New ImageConverter
            imgPreview.Source = c.Convert(GetEditingObject(Of CteImage).ContainedImage, GetType(ImageSource), Nothing, Globalization.CultureInfo.InvariantCulture)
        End Sub

        Private Sub EditingItem_ContainedImageChanged(sender As Object, e As EventArgs)
            RefreshDisplay()
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(CteImage)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 0
        End Function

        ''' <summary>
        ''' The way to get the EditingObject from outside this class.  Refreshes the display on set, and updates the object on get.
        ''' Calling this from inside this class could result in a stack overflow, especially if called from UpdateObject, so use GetEditingObject or GetEditingObject(Of T) instead.
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Property EditingObject As Object
            Get
                Return MyBase.EditingObject
            End Get
            Set(value As Object)
                If GetEditingObject(Of CteImage)() IsNot Nothing Then
                    RemoveHandler GetEditingObject(Of CteImage).ContainedImageUpdated, AddressOf EditingItem_ContainedImageChanged
                End If

                MyBase.EditingObject = value

                If GetEditingObject(Of CteImage)() IsNot Nothing Then
                    AddHandler GetEditingObject(Of CteImage).ContainedImageUpdated, AddressOf EditingItem_ContainedImageChanged
                End If
            End Set
        End Property
        Dim _editingObject As Object

    End Class
End Namespace

