Public MustInherit Class EditorTab
    Inherits TabItem 'UserControl
    MustOverride Sub RefreshDisplay(ByVal Save As GenericSave)
    MustOverride Function UpdateSave(ByVal Save As GenericSave) As GenericSave
    MustOverride ReadOnly Property SupportedGames As String()
End Class