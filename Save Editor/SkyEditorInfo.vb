Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SaveEditor.Saves
Public Class SkyEditorInfo
    Implements iSkyEditorPlugin
    Public ReadOnly Property PluginAuthor As String Implements iSkyEditorPlugin.PluginAuthor
        Get
            Return My.Resources.Language.PluginAuthor
        End Get
    End Property

    Public ReadOnly Property PluginName As String Implements iSkyEditorPlugin.PluginName
        Get
            Return My.Resources.Language.PluginName
        End Get
    End Property

    Public ReadOnly Property Credits As String Implements iSkyEditorPlugin.Credits
        Get
            Return My.Resources.Language.PluginCredits
        End Get
    End Property

    Public Sub Load(Manager As PluginManager) Implements iSkyEditorPlugin.Load
        'Manager.RegisterSaveTypeDetector(AddressOf DetectSaveType)
        Manager.RegisterIOFilter("*.sav", My.Resources.Language.RawSaveFile)
        Manager.RegisterIOFilter("*.dsv", My.Resources.Language.DeSmuMeSaveFile)

        Manager.RegisterDirectoryTypeDetector(AddressOf Me.DirectoryDetector)
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

    Public Sub UnLoad(Manager As PluginManager) Implements iSkyEditorPlugin.UnLoad

    End Sub

    Public Sub PrepareForDistribution() Implements iSkyEditorPlugin.PrepareForDistribution

    End Sub
End Class