Imports ROMEditor.FileFormats
Imports SkyEditorBase
Public Class BackgroundsTab
    Inherits EditorTab
    Dim _backgrounds As List(Of BGP)
    Public Overloads Overrides Async Sub RefreshDisplay(Save As GenericSave)
        _backgrounds = Await DirectCast(Save, Roms.SkyNDSRom).GetBackgrounds
        For Each item In _backgrounds
            lvBackgrounds.Items.Add(item)
        Next
    End Sub

    Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
        Return Save
    End Function

    Public Overrides ReadOnly Property SupportedGames As String()
        Get
            Return {GameStrings.SkyNDSRom}
        End Get
    End Property
End Class
