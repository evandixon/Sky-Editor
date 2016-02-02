Public Class FunctionDocumentation
    Public Property FunctionName As String
    Public Property FunctionDescription As String
    Public Property ParameterSets As List(Of ParameterSet)
    Public Property ReturnTypeName As String
    Public Property Remarks As String
    Public Overrides Function ToString() As String
        If String.IsNullOrEmpty(FunctionName) Then
            Return MyBase.ToString
        Else
            Return FunctionName
        End If
    End Function
    Public Sub New()
        ParameterSets = New List(Of ParameterSet)
    End Sub
End Class