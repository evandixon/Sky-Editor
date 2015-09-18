Public Class ObjectControlPlaceholder
    Inherits UserControl
    Dim _object As Object
    Public Property ObjectToEdit As Object
        Get
            If _object IsNot Nothing Then
                DirectCast(Content, ObjectControl).UpdateObject()
            End If
            Return _object
        End Get
        Set(value As Object)
            _object = value
            Content = PluginHelper.PluginManagerInstance.GetObjectControl(value)
            With DirectCast(Content, ObjectControl)
                .VerticalAlignment = VerticalAlignment.Top
                .HorizontalAlignment = HorizontalAlignment.Left
                .ContainedObject = value
                .RefreshDisplay()
            End With
        End Set
    End Property
End Class