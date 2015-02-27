Imports System.Threading.Tasks
Imports System.Text
Imports System.Reflection

Public Class PluginManager
    Implements IDisposable
    'Private Shared _domain As AppDomain
    'Private Shared Property PluginDomain As AppDomain
    '    Get
    '        If _domain Is Nothing Then
    '            Dim p = IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins")
    '            Dim s = AppDomain.CurrentDomain.SetupInformation
    '            s.ApplicationName = "SkyEditorPlugins"
    '            s.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory
    '            s.PrivateBinPath = IO.Path.GetDirectoryName(p).Substring(IO.Path.GetDirectoryName(p).LastIndexOf(IO.Path.DirectorySeparatorChar) + 1)
    '            s.CachePath = IO.Path.Combine(p, "cache" & IO.Path.DirectorySeparatorChar)
    '            s.ShadowCopyFiles = "true"
    '            s.ShadowCopyDirectories = p
    '            _domain = AppDomain.CreateDomain("SkyEditorPlugins", Nothing, s)
    '        End If
    '        Return _domain
    '    End Get
    '    Set(value As AppDomain)
    '        _domain = value
    '    End Set
    'End Property
    Private Const ShowLoadingWindow As Boolean = True

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
    Public Function IOFiltersString(Optional Filters As Dictionary(Of String, String) = Nothing, Optional IsSaveAs As Boolean = False) As String
        If Filters Is Nothing Then
            Filters = IOFilters
        End If
        Dim listFilter As New StringBuilder
        Dim supportedFilterName As String = ""
        Dim supportedFilterExt As String = ""
        If Filters IsNot Nothing Then
            For Each item In Filters
                listFilter.Append(String.Format("{0} ({1})|{1}|", item.Value, item.Key))
                supportedFilterName &= item.Value & ", "
                supportedFilterExt &= "" & item.Key & ";"
            Next
            Dim out = ""
            If Not IsSaveAs Then
                out &= String.Format("{0} ({1})|{1}", supportedFilterName.Trim(";"), supportedFilterExt.Trim(";"))
            End If
            out &= "|" & listFilter.ToString & "All Files (*.*)|*.*"
            Return out.Trim("|")
        Else
            Return "All Files (*.*)|*.*"
        End If
    End Function
    ''' <summary>
    ''' Gets the IO filters in a form the Open and Save file dialogs can use, optimized for the Save file dialog.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IOFiltersStringSaveAs() As String
        If Save IsNot Nothing Then
            Dim ext As String = Save.DefaultExtension
            If IOFilters.ContainsKey(ext) Then
                Dim filters As New Dictionary(Of String, String)
                filters.Add(ext, IOFilters(ext))
                For Each item In IOFilters
                    If Not filters.ContainsKey(item.Key) Then
                        filters.Add(item.Key, item.Value)
                    End If
                Next
                Return IOFiltersString(filters, True)
            Else
                Return IOFiltersString()
            End If
        Else
            Return IOFiltersString()
        End If
    End Function
    ''' <summary>
    ''' The currently loaded save.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Save As GenericSave
    ''' <summary>
    ''' List of all the plugins' assembly names.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Assemblies As New List(Of String)
    Public Property Plugins As New List(Of iSkyEditorPlugin)
    ''' <summary>
    ''' Dictionary containing all files needed by each plugin.
    ''' Excludes Assembly_plg.dll and /Assembly/, as these can be inferred by the assembly name of the plugin.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PluginFiles As New Dictionary(Of String, List(Of String))
    Public Property CheatManager As ARDS.Manager
    Public Property SaveTypeDetectors As New List(Of SaveTypeDetector)
    Public Property SaveTypes As New Dictionary(Of String, Type)
    Public Property EditorTabs As New List(Of Type)
    Public Property Window As iMainWindow
    Public Property PluginFolder As String

#Region "Constructors"
    Public Sub New()
        Me.New(Nothing, IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins"))
    End Sub
    ''' <summary>
    ''' Creates a new PluginManager, using the default storage location for plugins, which is Resources/Plugins, stored in the current working directory.
    ''' Plugins should end in _plg.dll or _plg.exe,  Ex. MyPlugin_plg.dll
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New(Window As iMainWindow)
        Me.New(Window, IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins"))
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
        LoadPlugins(PluginFolder)
    End Sub
    Public Sub LoadPlugins(PluginFolder As String)
        If IO.Directory.Exists(PluginFolder) Then
            Dim assemblies As New List(Of String)
            assemblies.AddRange(IO.Directory.GetFiles(PluginFolder, "*_plg.dll"))
            assemblies.AddRange(IO.Directory.GetFiles(PluginFolder, "*_plg.exe"))
            For Each plugin In assemblies
                PluginHelper.Writeline("Opening plugin " & IO.Path.GetFileName(plugin))
                Dim a As Assembly = Assembly.LoadFrom(plugin)
                Dim types As Type() = a.GetTypes
                For Each item In types
                    Dim IsPlugin As Boolean = False
                    For Each intface As Type In item.GetInterfaces
                        If intface Is GetType(iSkyEditorPlugin) Then
                            IsPlugin = True
                        End If
                    Next
                    If IsPlugin Then
                        Dim Plg As iSkyEditorPlugin = a.CreateInstance(item.ToString)
                        Plugins.Add(Plg)
                        Me.Assemblies.Add(plugin)
                    End If
                Next

            Next
            Me.CheatManager.GameIDs = Me.GameTypes
            If Window IsNot Nothing Then
                For Each item In Plugins
                    item.Load(Me)
                Next
            End If
            Me.PluginFolder = PluginFolder
        End If
    End Sub
    Public Sub ReloadPlugins()
        LoadPlugins(Me.PluginFolder)
    End Sub
    Public Sub UnloadPlugins()
        For Each item In Plugins
            item.UnLoad(Me)
        Next
        If Window IsNot Nothing Then Window.ClearTabItems()
        Me.CheatManager.GameIDs = New Dictionary(Of String, String)
        _consoleCommands = New Dictionary(Of String, ConsoleCommand)
        EditorTabs = New List(Of Type)
        IOFilters = New Dictionary(Of String, String)
        Dim toRemove As New List(Of MenuItem)
        If Window IsNot Nothing Then
            For Each item In Window.GetMenuItems
                If item.Tag = True Then
                    toRemove.Add(item)
                End If
            Next
            For Each item In toRemove
                Window.RemoveMenuItem(item)
            Next
        End If
        SaveTypes = New Dictionary(Of String, Type)
        SaveTypeDetectors = New List(Of SaveTypeDetector)
        _GameTypes = New Dictionary(Of String, String)
        Me.CheatManager.CodeDefinitions = New List(Of ARDS.CodeDefinition)
        PluginFiles = New Dictionary(Of String, List(Of String))
        Plugins = New List(Of iSkyEditorPlugin)
        'AppDomain.Unload(PluginDomain)
        'PluginDomain = Nothing
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
            Item.Tag = True
            Window.AddMenuItem(Item)
        End If
    End Sub

    Public Sub RegisterSaveType(SaveID As String, SaveType As Type)
        SaveTypes.Add(SaveID, SaveType)
    End Sub

    Public Sub RegisterSaveTypeDetector(Detector As SaveTypeDetector)
        SaveTypeDetectors.Add(Detector)
    End Sub

    Public Sub RegisterCodeGenerator(Generator As ARDS.CodeDefinition)
        Me.CheatManager.CodeDefinitions.Add(Generator)
    End Sub
    ''' <summary>
    ''' Registers the given File Path as being a resource used by the calling plugin.
    ''' This should be used for files in the same directory as the plugin, that are a strict requirement of functionality for your plugin.
    ''' Example: IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "xceed.wpf.toolkit.dll")
    ''' </summary>
    ''' <param name="FilePath">If the file is in the same directory as your plugin, use something like IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "xceed.wpf.toolkit.dll")</param>
    ''' <remarks></remarks>
    Public Sub RegisterResourceFile(FilePath As String)
        Dim plugin As String = Assembly.GetCallingAssembly.GetName.Name
        If Not PluginFiles.ContainsKey(plugin) Then
            PluginFiles.Add(plugin, New List(Of String))
        End If
        PluginFiles(plugin).Add(FilePath)
    End Sub
#End Region

#Region "Refresh and Update"
    Private Sub RefreshTabs()
        If ShowLoadingWindow Then PluginHelper.StartLoading(PluginHelper.GetLanguageItem("Refreshing tabs..."))
        If Window IsNot Nothing Then Window.ClearTabItems()
        Dim tabs = GetRefreshedTabs()
        For Each item In tabs
            If Window IsNot Nothing Then Window.AddTabItem(item)
        Next
        PluginHelper.StopLoading()
    End Sub
    Private Function GetRefreshedTabs() As List(Of TabItem)
        Dispose()
        Dim out As New List(Of TabItem)
        For Each item In EditorTabs
            Dim etab As EditorTab = item.GetConstructor({}).Invoke({})
            For Each game In etab.SupportedGames
                If game IsNot Nothing AndAlso Save IsNot Nothing AndAlso Save.CurrentSaveID = game Then
                    'add the tab because this save is one of the supported games
                    Dim t As TabItem = etab
                    Dim x As New Task(Sub()
                                          etab.RefreshDisplay(Save)
                                      End Sub)
                    x.RunSynchronously()
                    out.Add(t)
                    Exit For
                End If
            Next
        Next
        Return out
    End Function
    Public Sub RefreshDisplay()
        RefreshTabs()
        If Settings.DebugMode Then Save.DebugInfo()
    End Sub
    Public Sub UpdateSave()
        UpdateFromTabs()
    End Sub

    Private Sub UpdateFromTabs()
        If Window IsNot Nothing Then
            For Each item In Window.GetTabItems
                Save = DirectCast(item, EditorTab).UpdateSave(Save)
            Next
        End If
    End Sub
#End Region

#Region "Load Save"
    ''' <summary>
    ''' Loads the save from the given Filename.  If the save format cannot be determined, the built-in save type selector form will be shown.
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <remarks></remarks>
    Public Sub LoadSave(Filename As String)
        LoadSave(Filename, New SkyEditorWindows.GameTypeSelector)
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
        LoadSaveNoAutodetect(Filename, New SkyEditorWindows.GameTypeSelector)
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
