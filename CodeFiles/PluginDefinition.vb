Imports System.Reflection
Imports SkyEditor.Core

Public Class PluginDefinition
    Inherits SkyEditorPlugin

    Public Overrides ReadOnly Property Credits As String
        Get
            Return My.Resources.Language.PluginCredits
        End Get
    End Property

    Public Overrides ReadOnly Property PluginAuthor As String
        Get
            Return My.Resources.Language.PluginAuthor
        End Get
    End Property

    Public Overrides ReadOnly Property PluginName As String
        Get
            Return My.Resources.Language.PluginName
        End Get
    End Property

    Public Overrides Sub Load(Manager As PluginManager)
        Manager.RegisterIOFilter("*.txt", My.Resources.Language.TextFiles)
        Manager.RegisterIOFilter("*.lua", My.Resources.Language.LuaFiles)
        Manager.RegisterFileTypeDetector(AddressOf DetectFileType)
    End Sub

    Public Overrides Sub PrepareForDistribution()

    End Sub

    Public Overrides Sub UnLoad(Manager As PluginManager)

    End Sub

    Public Shared Function DetectFileType(File As GenericFile) As IEnumerable(Of TypeInfo)
        If File.OriginalFilename.ToLower.EndsWith(".txt") Then
            Return {GetType(TextFile).GetTypeInfo}
        ElseIf File.OriginalFilename.ToLower.EndsWith(".lua") Then
            Return {GetType(LuaCodeFile).GetTypeInfo}
        Else
            Return {}
        End If
    End Function
End Class
