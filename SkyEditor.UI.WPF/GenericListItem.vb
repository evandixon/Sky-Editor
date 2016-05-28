<Obsolete> Public Class GenericListItem(Of T)
    Implements IComparable

    Public Property Text As String
    Public Property Value As T
    Public Sub New(Text As String, Value As T)
        Me.Text = Text
        Me.Value = Value
    End Sub
    Public Sub New()
        Me.Text = ""
        Me.Value = Nothing
    End Sub
    Public Overrides Function ToString() As String
        Return Text
    End Function
    Public Overrides Function Equals(obj As Object) As Boolean
        If TypeOf obj Is GenericListItem(Of T) Then
            Return DirectCast(obj, GenericListItem(Of T)).Text = Me.Text
        Else
            Return False
        End If
    End Function

    Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
        If TypeOf obj Is GenericListItem(Of T) Then
            Return Me.Text.CompareTo(DirectCast(obj, GenericListItem(Of T)).Text)
        Else
            Return 0
        End If
    End Function
End Class