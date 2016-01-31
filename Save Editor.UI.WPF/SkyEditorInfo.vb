Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SaveEditor.Saves
Public Class SkyEditorInfo
    Implements iSkyEditorPlugin
    Public ReadOnly Property PluginAuthor As String Implements iSkyEditorPlugin.PluginAuthor
        Get
            Return "evandixon"
        End Get
    End Property

    Public ReadOnly Property PluginName As String Implements iSkyEditorPlugin.PluginName
        Get
            Return "Sky Editor"
        End Get
    End Property

    Public ReadOnly Property Credits As String Implements iSkyEditorPlugin.Credits
        Get
            Return PluginHelper.GetLanguageItem("SkyEditorPlgCredits", "Sky Editor Credits:\n     evandixon (General Research)\n     matix2267 (Pokemon Structure, code for interacting with bits)\n     Grovyle91 (Item Structure, IDs of Pokemon/Items/etc)\n     Prof. 9 (Team Name character encoding)\n     Demonic722 (Misc RAM and save addresses)")
        End Get
    End Property

    Public Sub Load(Manager As PluginManager) Implements iSkyEditorPlugin.Load
        'Manager.RegisterMenuActionType(GetType(MenuActions.ImportSdf))
        'Manager.RegisterMenuActionType(GetType(MenuActions.OpenSdfSave))
        Manager.RegisterResourceFile(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "Xceed.Wpf.AvalonDock.dll"))
        Manager.RegisterResourceFile(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "Xceed.Wpf.AvalonDock.Aero.dll"))
        Manager.RegisterResourceFile(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "Xceed.Wpf.AvalonDock.Metro.dll"))
        Manager.RegisterResourceFile(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "Xceed.Wpf.AvalonDock.VS2010.dll"))
        Manager.RegisterResourceFile(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "Xceed.Wpf.DataGrid.dll"))
        Manager.RegisterResourceFile(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "Xceed.Wpf.Toolkit.dll"))
    End Sub

    Public Function DetectSaveType(File As GenericFile) As String
        Try
            If File.Length > &HD Then
                Select Case File.RawData(&HD)
                    Case &H54
                        Return GameStrings.TDSave
                    Case &H53
                        Return GameStrings.SkySave
                    Case Else
                        If (File.RawData(&H404) = &H50 AndAlso File.RawData(&H405) = &H4F AndAlso File.RawData(&H406) = &H4B AndAlso File.RawData(&H407) = &H45) Then
                            Return GameStrings.RBSave
                        ElseIf File.OriginalFilename.ToLower.EndsWith("00000ba8\game_data") Then
                            Return GameStrings.MDGatesData
                        Else
                            Return Nothing
                        End If
                End Select
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function DirectoryDetector(Directory As IO.DirectoryInfo) As IEnumerable(Of Type)
        Dim s As New SdfSave(Directory.FullName)
        If s.IsValid Then
            Select Case s.MiniTitleId.ToLower
                Case GameStrings.GTIMiniTitleID
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