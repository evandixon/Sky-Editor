Imports SkyEditorBase

Public Class MyPlugin
    Implements iSkyEditorPlugin

    'Public Function AutoDetectSaveType(SaveBytes() As Byte) As String Implements SkyEditorPlugin.AutoDetectSaveType
    '    'Run code to detect the GameID of the given save.  Return nothing if it cannot be determined, and the user will get to choose what game it's for.
    '    Return Nothing
    'End Function

    'Public ReadOnly Property IOFilters As Dictionary(Of String, String) Implements SkyEditorPlugin.IOFilters
    '    Get
    '        Return Nothing
    '    End Get
    'End Property

    'Public ReadOnly Property ConsoleCommandList As Dictionary(Of String, SkyEditorPlugin.ConsoleCommand) Implements SkyEditorPlugin.ConsoleCommandList
    '    Get
    '        Dim out As New Dictionary(Of String, SkyEditorPlugin.ConsoleCommand)

    '        Return out
    '    End Get
    'End Property

    'Public Function GetMenuItem() As Windows.Controls.MenuItem Implements SkyEditorPlugin.GetMenuItem
    '    Return Nothing
    'End Function

    Public ReadOnly Property Credits As String Implements iSkyEditorPlugin.Credits
        Get
            'In credits, you can use \n for a line break
            Return "My Plugin\nMade by me."
        End Get
    End Property

    Public Sub Load(ByRef Manager As PluginManager) Implements iSkyEditorPlugin.Load
        'Register any custom Editor Tabs you've made.  Be sure to pass the TYPE of the tab, not an instance of the tab.
        Manager.RegisterEditorTab(GetType(MyTab))

        'Register any save file types
        Manager.RegisterSaveType(GameConstants.MySaveGameID, GetType(MySave))

        'Register game types for each save type, because some games share the same save type, while having different cheat codes.
        'If you only have one game per save structure, make the key and value of each entry be the same.
        'If you two or more games with different cheat code addresses, sharing the same save file structure,
        'The key (first argument) should be the specific game, and the value (second argument) should be the save file structure ID.
        'Both should be human-readable.
        Manager.RegisterGameType(GameConstants.MySaveGameID, GameConstants.MySaveGameID)
    End Sub

    Public ReadOnly Property PluginAuthor As String Implements iSkyEditorPlugin.PluginAuthor
        Get
            Return "Me!"
        End Get
    End Property

    Public ReadOnly Property PluginName As String Implements iSkyEditorPlugin.PluginName
        Get
            Return "My Plugin!"
        End Get
    End Property

    Public Sub UnLoad(ByRef Manager As PluginManager) Implements iSkyEditorPlugin.UnLoad
        'Delete any temporary files
    End Sub

    Public Sub PrepareForDistribution() Implements iSkyEditorPlugin.PrepareForDistribution
        'Delete any files that are not needed for distribution, including ROMs and decompressed files.
    End Sub
End Class
