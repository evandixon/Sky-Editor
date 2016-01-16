Imports SkyEditorBase.Interfaces
Public Class ObjectControlPlaceholder
    Inherits UserControl
    ''' <summary>
    ''' Raised when the contained object raises its Modified event, if it implements iModifiable
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>

    Public Event Modified(sender As Object, e As EventArgs)

    Dim _object As Object
    Public Property ObjectToEdit As Object
        Get
            If _object IsNot Nothing Then
                DirectCast(Content, ObjectControl).UpdateObject()
            End If
            Return _object
        End Get
        Set(value As Object)
            If _object IsNot Nothing AndAlso TypeOf _object Is iModifiable Then
                RemoveHandler DirectCast(_object, iModifiable).Modified, AddressOf OnModified
            End If
            _object = value
            If TypeOf value Is iModifiable Then
                AddHandler DirectCast(value, iModifiable).Modified, AddressOf OnModified
            End If
            Content = PluginHelper.PluginManagerInstance.GetObjectControl(value)
            With DirectCast(Content, ObjectControl)
                .VerticalAlignment = VerticalAlignment.Top
                .HorizontalAlignment = HorizontalAlignment.Left
                .EditingObject = value
                .RefreshDisplay()
            End With
        End Set
    End Property

    Private Sub OnModified(sender As Object, e As EventArgs)
        RaiseEvent Modified(sender, e)
    End Sub
End Class