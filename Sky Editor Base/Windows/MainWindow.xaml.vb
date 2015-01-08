Imports System.IO
Imports ICSharpCode.SharpZipLib.Zip
Imports ICSharpCode.SharpZipLib.Core
Imports System.Threading.Tasks
Imports System.Windows
Namespace Windows
    Class MainWindow
        Implements iMainWindow
        Dim Plugins As New List(Of iSkyEditorPlugin)
        Dim CodeDefinitions As New List(Of ARDS.CodeDefinitionV3)
        Dim EditorTabs As New List(Of Type)
        Dim SaveTypeDetectors As New List(Of iMainWindow.SaveTypeDetector)
        Dim SaveTypes As New Dictionary(Of String, Type)
        Private WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
        Private WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
        Private WithEvents ConsoleLog As IO.TextWriter

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

        ''' <summary>
        ''' The currently loaded save.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Save As GenericSave

        Public Event OnKeyPress(sender As Object, e As KeyEventArgs) Implements iMainWindow.OnKeyUp

        Private _IOFilters As Dictionary(Of String, String)

        ''' <summary>
        ''' Dictionary of (Extension, Friendly Name) used in the Open and Save file dialogs.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Property IOFilters As Dictionary(Of String, String)
            Get
                Return _IOFilters
            End Get
            Set(value As Dictionary(Of String, String))
                _IOFilters = value
                Dim listFilter As String = ""
                Dim supportedFilterName As String = ""
                Dim supportedFilterExt As String = ""
                For Each item In value
                    listFilter &= String.Format("{0} (*.{1})|*.{1}|", item.Value, item.Key)
                    supportedFilterName &= item.Value & ", "
                    supportedFilterExt &= "*." & item.Key & ";"
                Next
                OpenFileDialog1.Filter = String.Format("{0} ({1})|{1}", supportedFilterName.Trim(";"), supportedFilterExt.Trim(";")) & "|" & listFilter & "All Files (*.*)|*.*"
                SaveFileDialog1.Filter = String.Format("{0} ({1})|{1}", supportedFilterName.Trim(";"), supportedFilterExt.Trim(";")) & "|" & listFilter & "All Files (*.*)|*.*"
            End Set
        End Property

        Private _consoleCommands As Dictionary(Of String, iMainWindow.ConsoleCommand)
        Public ReadOnly Property ConsoleCommandList As Dictionary(Of String, iMainWindow.ConsoleCommand)
            Get
                Return _consoleCommands
            End Get
        End Property

        Public Sub OnWriteLine(Line As String)
            statusLblStatus.Content = Line
        End Sub

#Region "Menu Event Handlers"
        Private Sub menuNew_Click(sender As Object, e As RoutedEventArgs) Handles menuNew.Click
            Dim x As New GameTypeSelector
            x.AddGames(SaveTypes.Keys)
            If x.ShowDialog() Then
                Dim d(1048576) As Byte
                Dim gameID As String = x.SelectedGame
                If Not String.IsNullOrEmpty(gameID) Then
                    Save = SaveTypes(gameID).GetConstructor({GetType(Byte())}).Invoke({d})
                    Save.CurrentSaveID = gameID
                End If
                RefreshDisplay()
            End If
        End Sub
        Private Sub menuFileOpen_Click(sender As Object, e As RoutedEventArgs) Handles menuFileOpenAuto.Click
            If OpenFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                LoadSave(OpenFileDialog1.FileName)
            End If
        End Sub
        Private Sub menuFileOpenNoDetect_Click(sender As Object, e As RoutedEventArgs) Handles menuFileOpenNoDetect.Click
            If OpenFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                LoadSaveNoAutodetect(OpenFileDialog1.FileName)
            End If
        End Sub
        Private Async Sub menuFileSaveAs_Click(sender As Object, e As RoutedEventArgs) Handles menuFileSaveAs.Click
            If SaveFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                UpdateSave()
                'Writes the updated save, and fixes the checksum at the same time
                IO.File.WriteAllBytes(SaveFileDialog1.FileName, Await Save.GetBytes)
            End If
        End Sub
        Private Sub menuCheats_Click(sender As Object, e As RoutedEventArgs) Handles menuCheats.Click
            UpdateSave()
            Dim x As New CodeGenerator
            x.Show(Save)
        End Sub
        Private Sub menuCredits_Click(sender As Object, e As RoutedEventArgs) Handles menuCredits.Click
            Dim credits As String = ""
            credits = PluginHelper.GetLanguageItem("Credits", "Credits:\n     evandixon (lead developer)\n     Demonic722 (help with GBA cheat format)")
            For Each item In Plugins
                If Not String.IsNullOrWhiteSpace(item.Credits) Then
                    credits &= vbCrLf & PluginHelper.GetLanguageItem("CreditsSeparator", "----------") & vbCrLf
                    credits &= item.Credits & vbCrLf
                End If
            Next
            MessageBox.Show(credits.Trim)
        End Sub

        Private Sub menuDebugConsole_Click(sender As Object, e As RoutedEventArgs) Handles menuDebugConsole.Click
            ConsoleManager.Show()
            DeveloperConsole.DoCommands(Save, ConsoleCommandList)
        End Sub
#End Region

        Sub LoadSave(Filename As String)
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
            If Not found Then LoadSaveNoAutodetect(Filename)
        End Sub
        Sub LoadSaveNoAutodetect(Filename As String)
            Dim x As New GameTypeSelector
            x.AddGames(SaveTypes.Keys)
            If x.ShowDialog() Then
                Dim d() As Byte = IO.File.ReadAllBytes(Filename)
                Dim gameID As String = x.SelectedGame
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
#Region "Refresh and Update Display"
        Private Sub RefreshDisplay()
            RefreshTabs()
            If Settings.DebugMode Then Save.DebugInfo()
        End Sub
        Private Sub UpdateSave()
            UpdateFromTabs()
        End Sub
        Sub RefreshTabs()
            tcTabs.Items.Clear()
            For Each item In EditorTabs
                'Dim tabTask = Task.Run(Function()
                '                           Return item.GetConstructor({}).Invoke({})
                '                       End Function)
                Dim etab As EditorTab = item.GetConstructor({}).Invoke({}) 'tabTask.Result
                For Each game In etab.SupportedGames
                    If game IsNot Nothing AndAlso Save.CurrentSaveID = game Then
                        'add the tab because this save is one of the supported games
                        Dim t As TabItem = etab
                        Dim x As New Task(Sub()
                                              etab.RefreshDisplay(Save)
                                          End Sub)
                        x.RunSynchronously()
                        tcTabs.Items.Add(t)
                        GoTo NextTab
                    End If
                Next
NextTab:
            Next
        End Sub
        Sub UpdateFromTabs()
            For Each item In tcTabs.Items
                Save = DirectCast(item, EditorTab).UpdateSave(Save)
            Next
        End Sub
#End Region

        Private Sub MainWindow_Closed(sender As Object, e As EventArgs) Handles Me.Closed
            For Each item In Plugins
                item.UnLoad(Me)
            Next
        End Sub

        Private Sub MainWindow_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
            tcTabs.Items.Clear()
        End Sub

        Private Sub MainWindow_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
            RaiseEvent OnKeyPress(sender, e)
        End Sub


        Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            If Settings.DebugMode AndAlso Settings.ShowConsoleOnStart Then
                ConsoleManager.Show()
            End If
            If Environment.Is64BitProcess Then
                DeveloperConsole.Writeline("x64 Process")
            Else
                DeveloperConsole.Writeline("x86 Process")
            End If
            If Environment.Is64BitOperatingSystem Then
                DeveloperConsole.Writeline("x64 OS")
            Else
                DeveloperConsole.Writeline("x86 OS")
            End If

            'Load Plugins
            Const PluginFolder = "Resources/Plugins" '"Editors"

            If IO.Directory.Exists(Path.Combine(Environment.CurrentDirectory, PluginFolder)) Then
                Dim assemblies As String() = IO.Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, PluginFolder), "*_plg.dll")
                For Each plugin In assemblies
                    DeveloperConsole.Writeline("Opening plugin " & IO.Path.GetFileName(plugin))
                    Try
                        Dim a As System.Reflection.Assembly = System.Reflection.Assembly.LoadFrom(plugin)
                        Dim types As Type() = a.GetTypes
                        For Each item In types
                            Dim IsDefinition As Boolean = False
                            Dim IsPlugin As Boolean = False
                            For Each intface As Type In item.GetInterfaces
                                If intface Is GetType(iSkyEditorPlugin) Then
                                    IsPlugin = True
                                End If
                                If intface Is GetType(ARDS.CodeDefinitionV3) Then
                                    IsDefinition = True
                                End If
                            Next
                            If IsPlugin Then
                                Dim Plg As iSkyEditorPlugin = a.CreateInstance(item.ToString)
                                Plugins.Add(Plg)
                            End If
                            If IsDefinition Then
                                Dim CodeDef As ARDS.CodeDefinitionV3 = a.CreateInstance(item.ToString)
                                CodeDefinitions.Add(CodeDef)
                            End If
                        Next
                    Catch ex As System.IO.FileLoadException
                        If TypeOf ex.InnerException Is System.NotSupportedException Then
                            Dim p = IO.Path.GetFileName(plugin)
                            MessageBox.Show("Error loading plugin""" & p & """.  To solve, try going to Sky Editor's Resource directory > Plugins > " & p & " > Right Click > Properties, and clicking Unblock (at the bottom, since this plugin probably came from the internet).")
                        End If
                    Catch ex As Exception
                        If Settings.DebugMode Then
                            MessageBox.Show("Error loading plugin """ & plugin & """:" & vbCrLf & ex.ToString)
                        Else
                            MessageBox.Show("Error loading plugin """ & plugin & """.")
                        End If
                    End Try
                Next
            End If

            'Make sure there's actually a plugin loaded.
            If False AndAlso Plugins.Count = 0 Then
                MessageBox.Show("This version of Sky Editor requires at least one compatible extension in the Plugins folder to be loaded.  Sky Editor will now shut down.")
                Me.Close()
            End If

            'Initialize Dialogs
            OpenFileDialog1 = New Forms.OpenFileDialog
            'OpenFileDialog1.Filter = "Supported Files (*.sav;*.dsv)|*.sav;*.dsv|Save Files (*.sav)|*.sav|DeSmuMe Save Files (*.dsv)|*.dsv|All Files (*.*)|*.*"
            SaveFileDialog1 = New Forms.SaveFileDialog
            'SaveFileDialog1.Filter = "Supported Files (*.sav;*.dsv)|*.sav;*.dsv|Save Files (*.sav)|*.sav|DeSmuMe Save Files (*.dsv)|*.dsv|All Files (*.*)|*.*"


            For Each item In Plugins
                DeveloperConsole.Writeline("Loading plugin " & item.PluginName)
                item.Load(Me)
            Next
            'send loaded GameIDs to ARDS Manager
            ARDS.ManagerV3.GameIDs = GameTypes

            'Check command line arguments to see if a file needs to be loaded
            Dim fileToLoad As String = ""
            Dim args() As String = Environment.GetCommandLineArgs
            If args.Length > 1 Then
                'Code commented out should work if published with ClickOnce.  Should, so test before checking this in.
                'If args(1).ToLower.EndsWith(".application") Then
                '    'This is running in ClickOnce, which has a special way of loading files
                '    'TODO: use clickonce classes to get ileToLoad
                'Else
                fileToLoad = args(1)
                'Reset Current Directory. Without doing this, resources might not be read properly
                Environment.CurrentDirectory = args(0).Replace(IO.Path.GetFileName(args(0)), "")
                'End If
            End If

            'Load Language

            SetValue(MainWindow.TitleProperty, String.Format(PluginHelper.GetLanguageItem("Title", "Sky Editor v{0}"), My.Application.Info.Version.ToString(2)))
            menuFile.Header = PluginHelper.GetLanguageItem("File")
            menuNew.Header = PluginHelper.GetLanguageItem("New")
            menuFileOpen.Header = PluginHelper.GetLanguageItem("Open")
            menuFileOpenAuto.Header = PluginHelper.GetLanguageItem("OpenAutoDetect", "Open (Auto-Detect Game)")
            menuFileOpenNoDetect.Header = PluginHelper.GetLanguageItem("OpenNoAuto", "Open (Let me choose the Game)")
            menuFileSaveAs.Header = PluginHelper.GetLanguageItem("SaveAs", "Save As...")
            menuCredits.Header = PluginHelper.GetLanguageItem("CreditsLabel", "Credits")
            menuCheats.Header = PluginHelper.GetLanguageItem("Cheats")

            tcTabs.TabStripPlacement = Settings.TabStripPlacement


            'Add or remove cheats menu if there's plugins
            If CodeDefinitions.Count = 0 Then
                menuCheats.Visibility = System.Windows.Visibility.Collapsed
            Else
                ARDS.ManagerV3.CodeDefinitions = CodeDefinitions
            End If

            If Settings.DebugMode Then
                menuDebug.Visibility = System.Windows.Visibility.Visible
            End If

            RegisterConsoleCommand("distprep", AddressOf PrepareForDistribution)
            RegisterConsoleCommand("zip", AddressOf PackProgram)

            If fileToLoad.Length > 0 Then
                LoadSave(fileToLoad)
            Else
                'If no save was automatically loded...
                tcTabs.Items.Add(New WelcomeTab)
                tcTabs.SelectedIndex = 0
            End If
        End Sub

        Private Sub menuShowConsole_Click(sender As Object, e As RoutedEventArgs) Handles menuShowConsole.Click
            ConsoleManager.Show()
        End Sub
#Region "iMainWindow Support"
        Public Sub RegisterConsoleCommand(CommandName As String, Command As iMainWindow.ConsoleCommand) Implements iMainWindow.RegisterConsoleCommand
            If _consoleCommands Is Nothing Then
                _consoleCommands = New Dictionary(Of String, iMainWindow.ConsoleCommand)
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
        Public Sub RegisterEditorTab(Tab As Type) Implements iMainWindow.RegisterEditorTab
            If IsEditorTab(Tab) Then
                EditorTabs.Add(Tab)
            End If
        End Sub

        Public Sub RegisterGameType(GameID As String, SaveID As String) Implements iMainWindow.RegisterGameType
            _GameTypes.Add(GameID, SaveID)
        End Sub

        Public Sub RegisterIOFilter(FileExtension As String, FileFormatName As String) Implements iMainWindow.RegisterIOFilter
            Dim TempIOFilters As Dictionary(Of String, String) = IOFilters
            If TempIOFilters Is Nothing Then
                TempIOFilters = New Dictionary(Of String, String)
            End If
            TempIOFilters.Add(FileExtension, FileFormatName)
            IOFilters = TempIOFilters
        End Sub

        Public Sub RegisterMenuItem(Item As MenuItem) Implements iMainWindow.RegisterMenuItem
            If Item IsNot Nothing Then
                menuMain.Items.Add(Item)
            End If
        End Sub

        Public Sub RegisterSaveType(SaveID As String, SaveType As Type) Implements iMainWindow.RegisterSaveType
            SaveTypes.Add(SaveID, SaveType)
        End Sub

        Public Sub RegisterSaveTypeDetector(Detector As iMainWindow.SaveTypeDetector) Implements iMainWindow.RegisterSaveTypeDetector
            SaveTypeDetectors.Add(Detector)
        End Sub
#End Region
        Public Sub PrepareForDistribution(Target As GenericSave, Argument As String)
            DeveloperConsole.Writeline("Preparing for distribution...")
            For Each item In Plugins
                item.PrepareForDistribution()
            Next
            DeveloperConsole.Writeline("Distribution preparation complete.  The form will now close.  Type ""exit"" to exit the console.")
            Me.Close()
        End Sub
        Public Sub PackProgram(Target As GenericSave, ArchiveName As String)
            Dim blacklist As String() = {"DeSmuMe Integration_plg.pdb",
                                         "DeSmuMe Integration_plg.xml",
                                         "ICSharpCode.SharpZipLib.dll",
                                         "ROMEditor_plg.pdb",
                                         "ROMEditor_plg.xml",
                                         "SkyEditor.exe",
                                         "SkyEditor.pdb",
                                         "SkyEditor.xml",
                                         "SkyEditor_plg.dll.config",
                                         "SkyEditor_plg.pdb",
                                         "SkyEditor_plg.xml"}

            If Not IO.Directory.Exists(IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp")) Then
                IO.Directory.CreateDirectory(IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp"))
            Else
                IO.Directory.Delete(IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp"), True)
                IO.Directory.CreateDirectory(IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp"))
            End If
            If Not IO.Directory.Exists(IO.Path.Combine(Environment.CurrentDirectory, "Sky Editor Archives")) Then
                IO.Directory.CreateDirectory(IO.Path.Combine(Environment.CurrentDirectory, "Sky Editor Archives"))
            End If
            IO.File.Copy(IO.Path.Combine(Environment.CurrentDirectory, "SkyEditor.exe"), IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp", "SkyEditor.exe"), True)
            IO.File.Copy(IO.Path.Combine(Environment.CurrentDirectory, "ICSharpCode.SharpZipLib.dll"), IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp", "ICSharpCode.SharpZipLib.dll"), True)
            For Each File In IO.Directory.GetFiles(IO.Path.Combine(Environment.CurrentDirectory, "Resources"), "*", SearchOption.AllDirectories)
                If Not IO.Directory.Exists(IO.Path.GetDirectoryName(File.Replace(Environment.CurrentDirectory, IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp")))) Then
                    IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(File.Replace(Environment.CurrentDirectory, IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp"))))
                End If
                If Not blacklist.Contains(IO.Path.GetFileName(File)) Then
                    IO.File.Copy(File, File.Replace(Environment.CurrentDirectory, IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp")), True)
                End If
            Next
            Dim z As New FastZip
            z.CreateZip(IO.Path.Combine(Environment.CurrentDirectory, "Sky Editor Archives", ArchiveName & ".zip"), IO.Path.Combine(Environment.CurrentDirectory, "PackageTemp"), True, ".*", ".*")

        End Sub
    End Class
End Namespace
