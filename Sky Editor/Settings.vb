Public Class Settings
    Inherits SkyEditorBase.Settings
    Public Shared Function Enable255ItemCount() As Boolean
        Return Lists.Settings.ContainsKey("Enable255ItemCount") AndAlso Lists.Settings.ContainsKey("Enable255ItemCount") = "True"
    End Function
End Class