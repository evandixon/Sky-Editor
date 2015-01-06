Public Interface iMainWindow
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Tab">Type must inherit from EditorTab.</param>
    ''' <remarks></remarks>
    Sub RegisterEditorTab(Tab As Type)
    Sub RegisterSaveType(SaveID As String, SaveType As Type)
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="GameID">Include the extension of the game (.nds, .gba, .exe, etc.), so other plugins can tell what system it's for.  No extension for ROMs or other files without a game.</param>
    ''' <param name="SaveID"></param>
    ''' <remarks></remarks>
    Sub RegisterGameType(GameID As String, SaveID As String)
    Sub RegisterIOFilter(FileExtension As String, FileFormatName As String)
    Sub RegisterMenuItem(Item As MenuItem)

    Delegate Sub ConsoleCommand(ByVal Target As GenericSave, ByVal Argument As String)
    Sub RegisterConsoleCommand(CommandName As String, Command As ConsoleCommand)

    Delegate Function SaveTypeDetector(SaveBytes As GenericFile) As String
    Sub RegisterSaveTypeDetector(Detector As SaveTypeDetector)

    Event OnKeyUp(sender As Object, e As KeyEventArgs)
End Interface
