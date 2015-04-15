Public Class GateSave
    Inherits SkyEditorBase.GenericSave
    Public Sub New(Data As Byte())
        MyBase.New(Data)
    End Sub

    Public Overrides Sub FixChecksum()

    End Sub

    Public Overrides ReadOnly Property SaveID As String
        Get
            Return GameStrings.GateSave
        End Get
    End Property
End Class