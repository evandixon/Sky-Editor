Public Class GateSave
    Inherits SkyEditorBase.GenericSave
    Public Sub New(Data As Byte())
        MyBase.New(Data)
    End Sub

    Public Overrides Sub FixChecksum()

    End Sub

   Public Overrides Function DefaultSaveID() As String
        Return GameStrings.GateSave
    End Function
End Class