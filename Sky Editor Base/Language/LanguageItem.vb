Namespace Language
    Public Class LanguageItem
        Public Property Key As String
        Public Property Value As String
        Public Property PluginName As String
        Public Property Accessed As Boolean
        Public Sub New(Key As String, Value As String, PluginName As String, Accessed As Boolean)
            Me.Key = Key
            Me.Value = Value
            Me.PluginName = PluginName
            Me.Accessed = Accessed
        End Sub
        Public Sub New()

        End Sub
        Public Overrides Function Equals(obj As Object) As Boolean
            If TypeOf obj Is LanguageItem Then
                With DirectCast(obj, LanguageItem)
                    Return .Key = Me.Key AndAlso .PluginName = Me.PluginName
                End With
            Else
                Return False
            End If
        End Function

        Public Shared Operator =(Item1 As LanguageItem, Item2 As LanguageItem) As Boolean
            Return Item1.Equals(Item2)
        End Operator
        Public Shared Operator <>(Item1 As LanguageItem, Item2 As LanguageItem) As Boolean
            Return Not Item1.Equals(Item2)
        End Operator

        Public Overrides Function ToString() As String
            Return Value
        End Function

        Public Function Clone() As LanguageItem
            Return New LanguageItem(Key, Value, PluginName, False)
        End Function
    End Class
End Namespace