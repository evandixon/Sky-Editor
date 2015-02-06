Public Class GameStrings
    ''' <summary>
    ''' Gets the game id of MySave.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function MySaveGameID() As String
        'We could just return a constant string, but these strings are visible to the user, so we translate it.
        'Game IDs should include a relevant extension so other plugins can launch the appropriate game.
        'If you're editing a file that's not part of a game, don't include the extension.
        Return SkyEditorBase.PluginHelper.GetLanguageItem("MySave.ext")
    End Function
    ''' <summary>
    ''' Gets the save id of MySave.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function MySaveSaveID() As String
        'There is a difference between games and saves.  Saves can work on multiple games, but cheats sometimes work only on particular games, so the distinction needs to be made.
        'This will be shown to the user, so make it human readable.
        Return SkyEditorBase.PluginHelper.GetLanguageItem("MySave", "My Save")
    End Function

End Class
