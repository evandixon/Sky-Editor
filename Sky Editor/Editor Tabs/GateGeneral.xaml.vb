Imports SkyEditorBase

Public Class GateGeneral
    Inherits EditorTab

    Public Overrides Sub RefreshDisplay(Save As GenericSave)

    End Sub

    Public Overrides ReadOnly Property SupportedGames As String()
        Get
            Return {GameStrings.GateSave}
        End Get
    End Property

    Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
        Return Save
    End Function
End Class