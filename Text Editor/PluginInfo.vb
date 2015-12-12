Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Public Class PluginInfo
    Implements SkyEditorBase.Interfaces.iSkyEditorPlugin

    Public ReadOnly Property Credits As String Implements iSkyEditorPlugin.Credits
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property PluginAuthor As String Implements iSkyEditorPlugin.PluginAuthor
        Get
            Return "evandixon"
        End Get
    End Property

    Public ReadOnly Property PluginName As String Implements iSkyEditorPlugin.PluginName
        Get
            Return "Text Editor"
        End Get
    End Property

    Public Sub Load(Manager As PluginManager) Implements iSkyEditorPlugin.Load
        Manager.RegisterIOFilter("*.txt", "Text Files")
        Manager.RegisterIOFilter("*.lua", "Lua Script Files")
        Manager.RegisterFileTypeDetector(AddressOf DetectFileType)
    End Sub

    Public Sub PrepareForDistribution() Implements iSkyEditorPlugin.PrepareForDistribution

    End Sub

    Public Sub UnLoad(Manager As PluginManager) Implements iSkyEditorPlugin.UnLoad

    End Sub

    Public Shared Function DetectFileType(File As GenericFile) As IEnumerable(Of Type)
        If File.OriginalFilename.ToLower.EndsWith(".txt") Then
            Return {GetType(TextFile)}
        ElseIf File.OriginalFilename.ToLower.EndsWith(".lua")
            Return {GetType(LuaCodeFile)}
        Else
            Return {}
        End If
    End Function
End Class
