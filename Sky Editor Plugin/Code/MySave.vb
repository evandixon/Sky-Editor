Imports SkyEditorBase

Public Class MySave
    Inherits GenericSave

    Public Overrides Sub FixChecksum()
        'Run code here to fix the checksum of your save file.
        'If your save doesn't need its checksum fixed, do nothing.  Lucky you.
    End Sub

    Public Overrides Function DefaultSaveID() As String
        'In order to make detecting the save format for a save easier,
        'each save format has its own ID, which is also the friendly name for the game (one that the user can select from a list)

        'It is recommended to keep these in constants in a separate file, to avoid typos
        Return GameStrings.MySaveSaveID
    End Function

    Public Overrides Function DefaultExtension() As String
        'If you override this, then when you save a file, this filter will automatically be selected.
        'In order for this filter to function, it must be registered in your plugin definition.
        Return "*.ext"
    End Function

    Public Overrides Sub DebugInfo()
        'Need to get some information from the save, but don't want to add a UI just yet?
        'If you're in debug mode (settable in the settings file of Sky Editor), this code will run.  It's perfect for using to PluginHelper.WriteLine!
        MyBase.DebugInfo()
    End Sub

    'Public Sub New(Save As Byte())
    '    'For some reason the parent constructor is not inherited.
    '    'This takes care of that.
    '    MyBase.New(Save)
    'End Sub

    ''If you'd prefer, you can use a filename constructor instead.
    ''If you provide one, it will be called before the one giving a Byte array.
    'Public Sub New(Filename As String)
    '
    'End Sub

    '
    'In order to do anything in the future, you will need to make some properties (or functions if you prefer) that interact with the save.
    'You can use the property RawData to access the array of byte given by GenericSave
    '

End Class