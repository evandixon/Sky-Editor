Imports System.IO
Imports SkyEditor.Core

Public Class SkyEditorInfo
    Inherits SkyEditor.UI.WPF.WPFCoreSkyEditorPlugin
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

    Public Overrides ReadOnly Property Credits As String
        Get
            Return My.Resources.Language.PluginCredits
        End Get
    End Property

    Public Overrides Function IsPluginLoadingEnabled() As Boolean
        'If this is being used as a plugin, this is ignored.
        'If this assembly is launched directly, plugin loading isn't supported.
        Return False
    End Function

    Public Overrides Sub Load(Manager As PluginManager)
        Manager.LoadRequiredPlugin(New SkyEditor.SaveEditor.PluginDefinition, Me)
    End Sub

    Public Function DirectoryDetector(Directory As DirectoryInfo) As IEnumerable(Of Type)
        'Dim s As New SdfSave(Directory.FullName)
        'If s.IsValid Then
        '    Select Case s.MiniTitleId.ToLower
        '        Case GTISave.GTIMiniTitleID
        '            Return {GetType(GTISave)}
        '        Case Else
        '            Return {GetType(SdfSave)}
        '    End Select
        'Else
        Return {}
        'End If
    End Function

    Public Overrides Sub UnLoad(Manager As PluginManager)

    End Sub

    Public Overrides Sub PrepareForDistribution()

    End Sub
End Class