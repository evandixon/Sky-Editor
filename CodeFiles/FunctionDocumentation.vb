Public Class FunctionDocumentation
    Public Property FunctionName As String
    Public Property FunctionDescription As String
    Public Property ParameterSets As List(Of ParameterSet)
    Public Property ReturnTypeName As String
    Public Property Remarks As String
    Public Sub New()
        ParameterSets = New List(Of ParameterSet)
    End Sub
End Class