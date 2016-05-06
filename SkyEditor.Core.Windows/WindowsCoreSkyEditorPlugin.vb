Public MustInherit Class WindowsCoreSkyEditorPlugin
    Inherits CoreSkyEditorPlugin

    Public Overrides Function GetIOProvider() As Core.IOProvider
        Return New IOProvider
    End Function
End Class
