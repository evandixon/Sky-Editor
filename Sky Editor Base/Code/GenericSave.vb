Imports System.Threading.Tasks

Partial Public MustInherit Class GenericSave
    Inherits GenericFile

    Public Event SaveIDUpdated(sender As Object, NewSaveID As String)

#Region "Constructors"
    ''' <summary>
    ''' Creates a new GenericSave using the given Byte Array.
    ''' Filename will not be initialized if this is used.
    ''' </summary>
    ''' <param name="Save"></param>
    ''' <remarks></remarks>
    Public Sub New(Save As Byte())
        MyBase.New(Save)
    End Sub

    ''' <summary>
    ''' Creates a new GenericSave from the given file name.
    ''' RawData will not be initialized if this is used.
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <remarks></remarks>
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
#End Region

#Region "Properties"
    ''' <summary>
    ''' Gets or sets the name of the file.  Used to identify specific instances of files so more than one can be open at once.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Name As String

    ''' <summary>
    ''' A unique string that identifies the save format this is a save for.  Will be used to open appropriate editor tabs for the game.
    ''' Make it something easily recognizable be normal humans, because it will appear in a drop-down box.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property SaveID As String
        Get
            If _saveID Is Nothing Then
                _saveID = DefaultSaveID()
            End If
            Return _saveID
        End Get
        Set(value As String)
            _saveID = value
            RaiseEvent SaveIDUpdated(Me, value)
        End Set
    End Property
    Private _saveID As String
#End Region

    ''' <summary>
    ''' The original SaveID for this save type.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public MustOverride Function DefaultSaveID() As String


    ''' <summary>
    ''' Fixes the checksum of the file, if applicable.
    ''' If not overridden, nothing will happen.
    ''' </summary>
    ''' <remarks></remarks>
    Public Overridable Sub FixChecksum()

    End Sub

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
    ''' Saves the GenericSave to the given path, after fixing the checksum.
    ''' </summary>
    ''' <param name="Path"></param>
    ''' <remarks></remarks>
    Public Overridable Async Function Save(Path As String) As Task
        IO.File.WriteAllBytes(Path, Await GetBytes())
    End Function

    Public Overridable Function IsOfType(SaveID As String)
        Return Me.SaveID = SaveID
    End Function

    ''' <summary>
    ''' Code to be run when a save is loaded, only if the program is in debug mode.
    ''' Useful for using PluginHelper.Writeline()
    ''' </summary>
    ''' <remarks></remarks>
    Public Overridable Sub DebugInfo()
        PluginHelper.Writeline("[Debug " & SaveID & "]")
    End Sub

    ''' <summary>
    ''' Gets the default file extension used by a particular save format.  Defaults to *.sav if not overridden.
    ''' Should be in the format "*.sav", not ".sav" or "sav". (Because "sav" will mean the file must be called sav without an extension.)
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function DefaultExtension() As String
        Return "*.sav"
    End Function

    Public Overrides Function ToString() As String
        Return Name
    End Function

End Class