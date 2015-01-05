Imports System.Threading.Tasks

Partial Public MustInherit Class GenericSave
    Inherits GenericFile
#Region "Generic/Helper Methods"

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
    Public MustOverride Sub FixChecksum()
    ''' <summary>
    ''' Returns the raw data of the save file, after fixing the checksum.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' 
    Public Overridable Async Function GetBytes() As Task(Of Byte())
        Await Task.Run(Sub()
                           FixChecksum()
                       End Sub)
        Return RawData
    End Function
#End Region
    ''' <summary>
    ''' A unique string that identifies the save format this is a save for.  Will be used to open appropriate editor tabs for the game.
    ''' Make it something easily recognizable be normal humans, because it will appear in a drop-down box.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public MustOverride ReadOnly Property SaveID As String

    'Public MustOverride Function IsSaveOfType(GameID As String, SaveBytes As Byte()) As Boolean

    Public Property CurrentSaveID As String

    'Public Function IsSaveOfCurrentType(SaveBytes As Byte()) As Boolean
    '    Return IsSaveOfType(GameID, SaveBytes)
    'End Function

    ''' <summary>
    ''' Code to be run when a save is loaded, only if the program is in debug mode.
    ''' Useful for usingDebugConsole.Writeline()
    ''' </summary>
    ''' <remarks></remarks>
    Public Overridable Sub DebugInfo()
        DeveloperConsole.Writeline("[Debug " & CurrentSaveID & "]")
    End Sub



End Class