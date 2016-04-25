Imports System.ComponentModel.DataAnnotations

Public Class ExtensionCollection
    <Required> <Key> Public Property ID As Integer
    Public Property ParentCollection As Integer? 'Nullable
    <Required> Public Property Name As String
End Class
