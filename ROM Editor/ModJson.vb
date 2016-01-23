Public Class ModJson
    Public Property ToAdd As List(Of String)
    Public Property ToDelete As List(Of String)
    Public Property ToRename As Dictionary(Of String, String)
    Public Property ToUpdate As List(Of String)
    Public Property Name As String
    Public Property Author As String
    Public Property Description As String
    Public Property DependenciesBefore As List(Of String)
    Public Property DependenciesAfter As List(Of String)
    Public Property UpdateUrl As String
    Public Sub New()
        ToAdd = New List(Of String)
        ToDelete = New List(Of String)
        ToRename = New Dictionary(Of String, String)
        ToUpdate = New List(Of String)
        DependenciesBefore = New List(Of String)
        DependenciesAfter = New List(Of String)
    End Sub
End Class