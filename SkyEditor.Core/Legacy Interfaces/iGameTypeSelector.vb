Imports System.Reflection

Namespace Interfaces
    Public Interface iGameTypeSelector
        Sub AddGames(Games As Dictionary(Of String, TypeInfo).KeyCollection)
        Property SelectedGame As String
        Function ShowDialog() As Boolean
    End Interface
End Namespace