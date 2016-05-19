Imports System.Reflection
Imports SkyEditor.Core
Imports SkyEditor.Core.IO

Public Class PluginDefinition
    Inherits SkyEditorPlugin
    Implements IFileTypeDetector

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
        Manager.CurrentIOUIManager.RegisterIOFilter("*.txt", My.Resources.Language.TextFiles)
        Manager.CurrentIOUIManager.RegisterIOFilter("*.lua", My.Resources.Language.LuaFiles)
    End Sub

    Public Overrides Sub PrepareForDistribution()

    End Sub

    Public Overrides Sub UnLoad(Manager As PluginManager)

    End Sub

    Public Function DetectFileType(File As GenericFile, manager As PluginManager) As Task(Of IEnumerable(Of FileTypeDetectionResult)) Implements IFileTypeDetector.DetectFileType
        Dim out As New List(Of FileTypeDetectionResult)
        If File.OriginalFilename.ToLower.EndsWith(".txt") Then
            out.Add(New FileTypeDetectionResult With {.FileType = GetType(TextFile).GetTypeInfo, .MatchChance = 0.1})
        ElseIf File.OriginalFilename.ToLower.EndsWith(".lua") Then
            out.Add(New FileTypeDetectionResult With {.FileType = GetType(LuaCodeFile).GetTypeInfo, .MatchChance = 0.1})
        End If
        Return Task.FromResult(DirectCast(out, IEnumerable(Of FileTypeDetectionResult)))
    End Function
End Class
