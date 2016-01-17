Public Class EditorTabManager
    Public Shared Function GetEditorTabs() As List(Of EditorTab)
        Dim out As New List(Of EditorTab)
        out.Add(New GeneralTab)
        out.Add(New HeldItemsTab)
        out.Add(New EpisodeHeldItems)
        out.Add(New TDSStoredItems)
        out.Add(New RBStoredItemsTab)
        out.Add(New ActivePokemonTab)
        'out.Add(New StoredPokemonTab)
        out.Add(New RBStoredPokemonTab)
        Return out
    End Function
End Class
Public MustInherit Class EditorTab
    Inherits TabItem 'UserControl
    MustOverride Sub RefreshDisplay(ByVal Save As GenericSave)
    MustOverride Function UpdateSave(ByVal Save As GenericSave) As GenericSave
    MustOverride ReadOnly Property SupportedGames As GameType
End Class