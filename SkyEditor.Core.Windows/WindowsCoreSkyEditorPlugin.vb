Public MustInherit Class WindowsCoreSkyEditorPlugin
    Inherits CoreSkyEditorPlugin

    Public Overrides Function GetIOProvider() As SkyEditor.Core.IO.IOProvider
        Return New IOProvider
    End Function
End Class
