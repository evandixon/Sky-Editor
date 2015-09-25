Public Class JsonContainer
    Public Property TypeName As String
        Get
            If _value Is Nothing Then
                Return Me.GetType.FullName
            Else
                Return _value
            End If
        End Get
        Set(value As String)
            _value = value
        End Set
    End Property
    Dim _value As String
End Class
