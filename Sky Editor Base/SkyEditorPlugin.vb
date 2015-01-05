Public Interface SkyEditorPlugin
    ReadOnly Property PluginName As String
    ReadOnly Property PluginAuthor As String
    ReadOnly Property Credits As String
    Function GetEditorTabs() As List(Of EditorTab)
    ''' <summary>
    ''' Returns the GameID of the coresponding save, or Nothing if it cannot be detected.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function AutoDetectSaveType(SaveBytes As Byte()) As String
    ''' <summary>
    ''' Dictionary that links the GameID for a save format to the underlying class that models the save format.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property SaveTypes As Dictionary(Of String, Type)
    ''' <summary>
    ''' Dictionary that links the GameID for a save format to an actual Game, because while two or more games can have the same save format, they may have different cheat formats.
    ''' The value should be the save type and key should be specific game.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property GameTypes As Dictionary(Of String, String)
    ''' <summary>
    ''' Dictionary of (File extension, File format name) for use in open and save file dialogs.
    ''' Return nothing or an empty dictionary to rely on file extensions used in other plugins.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property IOFilters As Dictionary(Of String, String)


    Delegate Sub ConsoleCommand(ByVal Target As GenericSave, ByVal Argument As String)
    ''' <summary>
    ''' Dictionary containing ConsoleCommands for use in the debug console.
    ''' Key: name of command
    ''' Value: Subroutine that can be used as a ConsoleCommand
    ''' 
    ''' Only evaluated if in debug mode.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property ConsoleCommandList As Dictionary(Of String, ConsoleCommand)

    ''' <summary>
    ''' Function that returns a MenuItem to be added to the menu bar.
    ''' Return nothing to not add one.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetMenuItem() As MenuItem
End Interface
