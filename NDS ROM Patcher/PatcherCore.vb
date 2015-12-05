Public MustInherit Class PatcherCore
    Public MustOverride Sub Browse()
    Public MustOverride Function Unpack() As Task
    Public MustOverride Function Repack() As Task
    Public MustOverride Sub Cleanup()
End Class
