Imports System.Threading.Tasks

Partial Public MustInherit Class GenericSave
    Inherits GenericFile

    Public Sub New(Save As Byte())
        MyBase.New(Save)
    End Sub
    Public Sub New(Filename As String)
        MyBase.New(Filename)
    End Sub
    ''' <summary>
    ''' Creates a new GenericSave with a null save.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        MyBase.New({})
    End Sub

    ''' <summary>
    ''' Gets or sets the name of the file.  Used to identify specific instances of files so more than one can be open at once.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Name As String

    ''' <summary>
    ''' Fixes the checksum of the file, if applicable.
    ''' </summary>
    ''' <remarks></remarks>
    Public MustOverride Sub FixChecksum()

    ''' <summary>
    ''' Returns the raw data of the save file, after fixing the checksum.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Async Function GetBytes() As Task(Of Byte())
        Await Task.Run(Sub()
                           FixChecksum()
                       End Sub)
        Return RawData
    End Function

    ''' <summary>
    ''' A unique string that identifies the save format this is a save for.  Will be used to open appropriate editor tabs for the game.
    ''' Make it something easily recognizable be normal humans, because it will appear in a drop-down box.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public MustOverride ReadOnly Property SaveID As String

    ''' <summary>
    ''' Code to be run when a save is loaded, only if the program is in debug mode.
    ''' Useful for using PluginHelper.Writeline()
    ''' </summary>
    ''' <remarks></remarks>
    Public Overridable Sub DebugInfo()
        PluginHelper.Writeline("[Debug " & SaveID & "]")
    End Sub

    ''' <summary>
    ''' Gets the default file extension used by a particular save format.  Defaults to *.sav if not overriden.
    ''' Should be in the format "*.sav", not ".sav" or "sav". (Because "sav" will mean the file must be called sav without an extension.)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function DefaultExtension() As String
        Return "*.sav"
    End Function

End Class