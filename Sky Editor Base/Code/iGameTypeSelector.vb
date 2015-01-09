Public Interface iGameTypeSelector
    Sub AddGames(Games As Dictionary(Of String, Type).KeyCollection)
    Property SelectedGame As String
    Function ShowDialog() As Boolean
End Interface
