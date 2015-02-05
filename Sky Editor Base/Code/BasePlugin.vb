Imports SkyEditorBase.SkyEditorWindows

Public MustInherit Class BasePlugin
    Dim _assemblyname As String
    Public ReadOnly Property AssemblyName As String
        Get
            Return _assemblyname
        End Get
    End Property
    Public Overridable ReadOnly Property PluginName As String
        Get
            Return AssemblyName
        End Get
    End Property
    Public ReadOnly Property PluginAuthor As String
        Get
            Return "Unknown"
        End Get
    End Property
    Public ReadOnly Property Credits As String
        Get
            Return ""
        End Get
    End Property
    Public Overridable Function AutoDetectSaveType(SaveBytes As Byte()) As String
        Return Nothing
    End Function
    Public MustOverride Sub Load(Window As MainWindow)
End Class
