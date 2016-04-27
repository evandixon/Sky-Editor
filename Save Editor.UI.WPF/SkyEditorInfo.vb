Imports SaveEditor.Saves
Imports SkyEditor.Core.Extensions.Plugins

Public Class SkyEditorInfo
    Implements ISkyEditorPlugin
    Public ReadOnly Property PluginAuthor As String Implements ISkyEditorPlugin.PluginAuthor
        Get
            Return My.Resources.Language.PluginAuthor
        End Get
    End Property

    Public ReadOnly Property PluginName As String Implements ISkyEditorPlugin.PluginName
        Get
            Return My.Resources.Language.PluginName
        End Get
    End Property

    Public ReadOnly Property Credits As String Implements ISkyEditorPlugin.Credits
        Get
            Return My.Resources.Language.PluginCredits
        End Get
    End Property

    Public Sub Load(Manager As PluginManager) Implements ISkyEditorPlugin.Load
        Manager.LoadPlugin(New SaveEditor.SkyEditorInfo)
    End Sub

    Public Function DirectoryDetector(Directory As IO.DirectoryInfo) As IEnumerable(Of Type)
        Dim s As New SdfSave(Directory.FullName)
        If s.IsValid Then
            Select Case s.MiniTitleId.ToLower
                Case GTISave.GTIMiniTitleID
                    Return {GetType(GTISave)}
                Case Else
                    Return {GetType(SdfSave)}
            End Select
        Else
            Return {}
        End If
    End Function

    Public Sub UnLoad(Manager As PluginManager) Implements ISkyEditorPlugin.UnLoad

    End Sub

    Public Sub PrepareForDistribution() Implements ISkyEditorPlugin.PrepareForDistribution

    End Sub
End Class