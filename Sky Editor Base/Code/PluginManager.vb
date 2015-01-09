Imports System.Threading.Tasks

Public Class PluginManager
    Implements IDisposable

    Delegate Sub ConsoleCommand(ByVal Manager As PluginManager, ByVal Argument As String)
    Delegate Function SaveTypeDetector(SaveBytes As GenericFile) As String

    Dim _GameTypes As New Dictionary(Of String, String)
    ''' <summary>
    ''' Key: Game Type
    ''' Value: Save Type
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GameTypes As Dictionary(Of String, String)
        Get
            Return _GameTypes
        End Get
    End Property
    Private _consoleCommands As Dictionary(Of String, ConsoleCommand)
    Public ReadOnly Property ConsoleCommandList As Dictionary(Of String, ConsoleCommand)
        Get
            Return _consoleCommands
        End Get
    End Property
    Private _IOFilters As Dictionary(Of String, String)

    ''' <summary>
    ''' Dictionary of (Extension, Friendly Name) used in the Open and Save file dialogs.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IOFilters As Dictionary(Of String, String)
        Get
            Return _IOFilters
        End Get
        Set(value As Dictionary(Of String, String))
            _IOFilters = value
        End Set
    End Property
    ''' <summary>
    ''' Gets the IO filters in a form Open and Save file dialogs can use.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IOFiltersString() As String
        Dim listFilter As String = ""
        Dim supportedFilterName As String = ""
        Dim supportedFilterExt As String = ""
        For Each item In IOFilters
            listFilter &= String.Format("{0} (*.{1})|*.{1}|", item.Value, item.Key)
            supportedFilterName &= item.Value & ", "
            supportedFilterExt &= "*." & item.Key & ";"
        Next
        Return String.Format("{0} ({1})|{1}", supportedFilterName.Trim(";"), supportedFilterExt.Trim(";")) & "|" & listFilter & "All Files (*.*)|*.*"
    End Function
    ' ''' <summary>
    ' ''' Gets the IO filters in a form the Open and Save file dialogs can use, optimized for the Save file dialog.
    ' ''' </summary>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Public Function IOFiltersStringSaveAs() As String
    '    If Save IsNot Nothing Then

    '    Else
    '        Return IOFiltersString()
    '    End If
    'End Function
    ''' <summary>
    ''' The currently loaded save.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Save As GenericSave
    Public Property Plugins As New List(Of iSkyEditorPlugin)
    Public Property CheatManager As ARDS.Manager
    Public Property SaveTypeDetectors As New List(Of SaveTypeDetector)
    Public Property SaveTypes As New Dictionary(Of String, Type)
    Public Property EditorTabs As New List(Of Type)
    Public Property Window As iMainWindow

#Region "Constructors"
    ''' <summary>
    ''' Creates a new PluginManager, using the default storage location for plugins, which is Resources/Plugins, stored in the current working directory.
    ''' Plugins should end in _plg.dll or _plg.exe,  Ex. MyPlugin_plg.dll
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(Window As iMainWindow)
        Me.New(Window, IO.Path.Combine(Environment.CurrentDirectory, "Resources/Plugins"))
    End Sub

    ''' <summary>
    ''' Creates a new PluginManager given the folder plugin files are stored in.
    ''' Plugins should end in _plg.dll or _plg.exe,  Ex. MyPlugin_plg.dll
    ''' </summary>
    ''' <param name="PluginFolder"></param>
    ''' <remarks></remarks>
    Public Sub New(Window As iMainWindow, PluginFolder As String)
        Me.Window = Window
        Me.CheatManager = New ARDS.Manager
        If IO.Directory.Exists(PluginFolder) Then
            Dim assemblies As New List(Of String)
            assemblies.AddRange(IO.Directory.GetFiles(PluginFolder, "*_plg.dll"))
            assemblies.AddRange(IO.Directory.GetFiles(PluginFolder, "*_plg.exe"))
            For Each plugin In assemblies
                DeveloperConsole.Writeline("Opening plugin " & IO.Path.GetFileName(plugin))
                Dim a As System.Reflection.Assembly = System.Reflection.Assembly.LoadFrom(plugin)
                Dim types As Type() = a.GetTypes
                For Each item In types
                    Dim IsDefinition As Boolean = False
                    Dim IsPlugin As Boolean = False
                    For Each intface As Type In item.GetInterfaces
                        If intface Is GetType(iSkyEditorPlugin) Then
                            IsPlugin = True
                        End If
                        If intface Is GetType(ARDS.CodeDefinition) Then
                            IsDefinition = True
                        End If
                    Next
                    If IsPlugin Then
                        Dim Plg As iSkyEditorPlugin = a.CreateInstance(item.ToString)
                        Plugins.Add(Plg)
                    End If
                    If IsDefinition Then
                        Dim CodeDef As ARDS.CodeDefinition = a.CreateInstance(item.ToString)
                        Me.CheatManager.CodeDefinitions.Add(CodeDef)
                    End If
                Next
            Next
        End If
        Me.CheatManager.GameIDs = Me.GameTypes
        For Each item In Plugins
            item.Load(Me)
        Next
    End Sub
#End Region

#Region "Registration"
    Public Sub RegisterConsoleCommand(CommandName As String, Command As ConsoleCommand)
        If _consoleCommands Is Nothing Then
            _consoleCommands = New Dictionary(Of String, ConsoleCommand)
        End If
        _consoleCommands.Add(CommandName, Command)
    End Sub

    Private Function IsEditorTab(T As Type) As Boolean
        If T.BaseType Is GetType(EditorTab) Then
            Return True
        Else
            If T.BaseType Is GetType(Object) Then
                Return False
            Else
                Return IsEditorTab(T.BaseType)
            End If
        End If
    End Function

    Public Sub RegisterEditorTab(Tab As Type)
        If IsEditorTab(Tab) Then
            EditorTabs.Add(Tab)
        End If
    End Sub

    Public Sub RegisterGameType(GameID As String, SaveID As String)
        _GameTypes.Add(GameID, SaveID)
    End Sub

    Public Sub RegisterIOFilter(FileExtension As String, FileFormatName As String)
        Dim TempIOFilters As Dictionary(Of String, String) = IOFilters
        If TempIOFilters Is Nothing Then
            TempIOFilters = New Dictionary(Of String, String)
        End If
        TempIOFilters.Add(FileExtension, FileFormatName)
        IOFilters = TempIOFilters
    End Sub

    Public Sub RegisterMenuItem(Item As MenuItem)
        If Item IsNot Nothing Then
            Window.AddMenuItem(Item)
        End If
    End Sub

    Public Sub RegisterSaveType(SaveID As String, SaveType As Type)
        SaveTypes.Add(SaveID, SaveType)
    End Sub

    Public Sub RegisterSaveTypeDetector(Detector As SaveTypeDetector)
        SaveTypeDetectors.Add(Detector)
    End Sub
#End Region

#Region "Refresh and Update"
    Private Sub RefreshTabs()
        Window.ClearTabItems()
        For Each item In EditorTabs
            Dim etab As EditorTab = item.GetConstructor({}).Invoke({})
            For Each game In etab.SupportedGames
                If game IsNot Nothing AndAlso Save.CurrentSaveID = game Then
                    'add the tab because this save is one of the supported games
                    Dim t As TabItem = etab
                    Dim x As New Task(Sub()
                                          etab.RefreshDisplay(Save)
                                      End Sub)
                    x.RunSynchronously()
                    Window.AddTabItem(t)
                    GoTo NextTab
                End If
            Next
NextTab:
        Next
    End Sub
    Public Sub RefreshDisplay()
        RefreshTabs()
        If Settings.DebugMode Then Save.DebugInfo()
    End Sub
    Public Sub UpdateSave()
        UpdateFromTabs()
    End Sub

    Private Sub UpdateFromTabs()
        For Each item In Window.GetTabItems
            Save = DirectCast(item, EditorTab).UpdateSave(Save)
        Next
    End Sub
#End Region

#Region "Load Save"
    ''' <summary>
    ''' Loads the save from the given Filename.  If the save format cannot be determined, the built-in save type selector form will be shown.
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <remarks></remarks>
    Public Sub LoadSave(Filename As String)
        LoadSave(Filename, New Windows.GameTypeSelector)
    End Sub
    ''' <summary>
    ''' Loads the save from the given Filename.  If the save format cannot be determined, the given Detector will be used to determine the save format.
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <param name="Detector"></param>
    ''' <remarks></remarks>
    Public Sub LoadSave(Filename As String, Detector As iGameTypeSelector)
        Dim d As New GenericFile(IO.File.ReadAllBytes(Filename))

        Dim saveID As String = ""
        Dim found As Boolean = False
        For Each item In SaveTypeDetectors
            saveID = item.Invoke(d)
            If Not String.IsNullOrEmpty(saveID) Then
                Dim SaveType = SaveTypes(saveID)
                Dim constructor = SaveType.GetConstructor({GetType(String)}) 'Sub New(Filename as String)
                If constructor IsNot Nothing Then
                    Save = constructor.Invoke({Filename})
                Else
                    constructor = SaveType.GetConstructor({GetType(Byte())}) 'Sub New(Bytes as Byte())
                    Save = constructor.Invoke({d.RawData})
                End If
                Save.CurrentSaveID = saveID
                RefreshDisplay()
                found = True
                Exit For
            End If
        Next
        If Not found Then LoadSaveNoAutodetect(Filename, Detector)
    End Sub
    ''' <summary>
    ''' Loads the save from the given Filename, showing the built-in save type selector to determine the save format.
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <remarks></remarks>
    Public Sub LoadSaveNoAutodetect(Filename As String)
        LoadSaveNoAutodetect(Filename, New Windows.GameTypeSelector)
    End Sub
    ''' <summary>
    ''' Loads the save from the given Filename, showing the given Detector to determine the save format.
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <param name="Detector"></param>
    ''' <remarks></remarks>
    Public Sub LoadSaveNoAutoDetect(Filename As String, Detector As iGameTypeSelector)
        Detector.AddGames(SaveTypes.Keys)
        If Detector.ShowDialog() Then
            Dim d() As Byte = IO.File.ReadAllBytes(Filename)
            Dim gameID As String = Detector.SelectedGame
            If Not String.IsNullOrEmpty(gameID) Then
                Dim constructor = SaveTypes(gameID).GetConstructor({GetType(String)}) 'Sub New (Filename as String)
                If constructor IsNot Nothing Then
                    Save = constructor.Invoke({Filename})
                Else
                    constructor = SaveTypes(gameID).GetConstructor({GetType(Byte())}) 'Sub New (Bytes as Byte())
                    Save = constructor.Invoke({d})
                End If
                Save.CurrentSaveID = gameID
            End If
            RefreshDisplay()
        End If
    End Sub
#End Region

#Region "New Save"

#End Region



#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                For Each item In Plugins
                    item.UnLoad(Me)
                Next
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
