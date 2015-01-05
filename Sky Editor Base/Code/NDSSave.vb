Public MustInherit Class NDSSave
    Inherits GenericSave
    Public MustOverride Overrides Sub FixChecksum()

    Public MustOverride Overrides ReadOnly Property SaveID As String

End Class
