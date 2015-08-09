Public Class ObjectControlPlaceholder
    Inherits UserControl
    Dim _object As Object
    Public Property ObjectToEdit As Object
        Get
            If _object IsNot Nothing Then _object = DirectCast(Content, ObjectControl).UpdateObject(_object)
            Return _object
        End Get
        Set(value As Object)
            _object = value
            Content = PluginHelper.PluginManagerInstance.GetObjectControl(value)
            With DirectCast(Content, ObjectControl)
                .VerticalAlignment = VerticalAlignment.Top
                .HorizontalAlignment = HorizontalAlignment.Left
                .RefreshDisplay(value)
            End With
        End Set
    End Property
End Class