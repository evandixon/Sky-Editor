Public MustInherit Class EditorTab
    Inherits ObjectTab
    MustOverride Overloads Sub RefreshDisplay(ByVal Save As GenericSave)
    MustOverride Function UpdateSave(ByVal Save As GenericSave) As GenericSave
    Overridable ReadOnly Property SupportedGames As String()
        Get
            Return {}
        End Get
    End Property

    Public Overrides Sub RefreshDisplay(Save As Object)
        Me.RefreshDisplay(DirectCast(Save, GenericSave))
    End Sub

    Public Overrides Function UpdateObject(Save As Object) As Object
        Return Me.UpdateSave(DirectCast(Save, GenericSave))
    End Function
End Class