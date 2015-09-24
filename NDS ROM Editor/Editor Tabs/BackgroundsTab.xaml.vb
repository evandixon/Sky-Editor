Imports ROMEditor.FileFormats
Imports SkyEditorBase
Public Class BackgroundsTab
    Inherits ObjectTab
    Dim _backgrounds As List(Of BGP)
    Public Overrides Async Sub RefreshDisplay()
        _backgrounds = Await DirectCast(MyBase.EditingObject, Roms.SkyNDSRom).GetBackgrounds
        For Each item In _backgrounds
            lvBackgrounds.Items.Add(item)
        Next
    End Sub

    Public Overrides ReadOnly Property SupportedTypes As Type()
        Get
            Return {GetType(Roms.SkyNDSRom)}
        End Get
    End Property
End Class
