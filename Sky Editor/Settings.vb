Public Class Settings
    Inherits SkyEditorBase.Settings
    Public Shared Function Enable255ItemCount() As Boolean
        Return Settings.ContainsKey("Enable255ItemCount") AndAlso Settings.ContainsKey("Enable255ItemCount") = "True"
    End Function
End Class