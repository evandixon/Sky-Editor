Imports System.IO
Imports ICSharpCode.SharpZipLib.Zip
Imports ICSharpCode.SharpZipLib.Core
Imports System.Threading.Tasks
Imports System.Windows
Imports SkyEditorBase.Redistribution

Namespace SkyEditorWindows
    Class MainWindow
        Implements iMainWindow
        Private WithEvents Manager As PluginManager
        Private WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
        Private WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
        Public Event OnKeyPress(sender As Object, e As KeyEventArgs) Implements iMainWindow.OnKeyPress
        Dim tabs As New Dictionary(Of String, List(Of EditorTab))

#Region "Menu Event Handlers"
        Private Sub menuNew_Click(sender As Object, e As RoutedEventArgs) Handles menuNew.Click
            menuMain.IsEnabled = False
            Dim x As New GameTypeSelector
            x.AddGames(Manager.SaveTypes.Keys)
            If x.ShowDialog() Then
                Dim d(1048576) As Byte
                Dim gameID As String = x.SelectedGame
                If Not String.IsNullOrEmpty(gameID) Then
                    Dim s As GenericSave = Manager.SaveTypes(gameID).GetConstructor({GetType(Byte())}).Invoke({d})
                    s.Name = "Pokemon Mystery Dungeon Explorers of Sky - 1.nds"
                    Manager.Saves.Add(s)
                    Manager.CurrentSave = s.Name
                    Manager.RefreshDisplay("Pokemon Mystery Dungeon Explorers of Sky - 1.nds")
                    Manager_SaveAdded(Me, New PluginManager.SaveAddedEventArgs(s.Name, s))
                End If
            End If
            menuMain.IsEnabled = True
        End Sub
        Private Sub menuFileOpen_Click(sender As Object, e As RoutedEventArgs) Handles menuFileOpenAuto.Click
            menuMain.IsEnabled = False
            OpenFileDialog1.Filter = Manager.IOFiltersString
            If OpenFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                Manager.LoadSave(OpenFileDialog1.FileName)
            End If
            menuMain.IsEnabled = True
        End Sub
        Private Sub menuFileOpenNoDetect_Click(sender As Object, e As RoutedEventArgs) Handles menuFileOpenNoDetect.Click
            menuMain.IsEnabled = False
            OpenFileDialog1.Filter = Manager.IOFiltersString
            If OpenFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                Manager.LoadSaveNoAutodetect(OpenFileDialog1.FileName)
            End If
            menuMain.IsEnabled = True
        End Sub
        Private Async Sub menuFileSaveAs_Click(sender As Object, e As RoutedEventArgs) Handles menuFileSaveAs.Click
            menuMain.IsEnabled = False
            SaveFileDialog1.Filter = Manager.IOFiltersStringSaveAs()
            If SaveFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                Manager.UpdateSave()
                'Writes the updated save, and fixes the checksum at the same time
                IO.File.WriteAllBytes(SaveFileDialog1.FileName, Await Manager.Save.GetBytes)
            End If
            menuMain.IsEnabled = True
        End Sub
        Private Sub menuCheats_Click(sender As Object, e As RoutedEventArgs) Handles menuCheats.Click
            menuMain.IsEnabled = False
            Manager.UpdateSave()
            Dim x As New CodeGenerator(Manager)
            x.Show()
            menuMain.IsEnabled = True
        End Sub
        Private Sub menuCredits_Click(sender As Object, e As RoutedEventArgs) Handles menuCredits.Click
            Dim credits As String = ""
            credits = PluginHelper.GetLanguageItem("BaseCredits", "Credits:\n     evandixon (lead developer)\n     Demonic722 (help with GBA cheat format)")
            For Each item In Manager.Plugins
                If Not String.IsNullOrWhiteSpace(item.Credits) Then
                    credits &= vbCrLf & PluginHelper.GetLanguageItem("CreditsSeparator", "----------") & vbCrLf
                    credits &= item.Credits
                End If
            Next
            MessageBox.Show(credits.Trim, PluginHelper.GetLanguageItem("CreditsTitle"))
        End Sub

        Private Sub menuDebugConsole_Click(sender As Object, e As RoutedEventArgs) Handles menuDebugConsole.Click
            Internal.ConsoleManager.Show()
            PluginHelper.DoCommands(Manager, Manager.ConsoleCommandList)
        End Sub

        Private Sub menuShowConsole_Click(sender As Object, e As RoutedEventArgs) Handles menuShowConsole.Click
            Internal.ConsoleManager.Show()
        End Sub
#End Region

        'Todo: Figure out why this causes freesing on closing
        'Private Sub MainWindow_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        '    For Each item In Manager.Plugins
        '        item.UnLoad(Manager)
        '    Next
        'End Sub

        Private Sub MainWindow_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
            tcTabs.Items.Clear()
        End Sub

        Private Sub MainWindow_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
            RaiseEvent OnKeyPress(sender, e)
        End Sub

        Private Async Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            If Not IO.File.Exists(IO.Path.Combine(PluginHelper.RootResourceDirectory, "settings.txt")) Then
                IO.File.WriteAllText(IO.Path.Combine(PluginHelper.RootResourceDirectory, "settings.txt"), My.Resources.Settings)
            End If
            If Settings.DebugMode AndAlso Settings.ShowConsoleOnStart Then
                Internal.ConsoleManager.Show()
            End If
            If Environment.Is64BitProcess Then
                PluginHelper.Writeline("x64 Process")
            Else
                PluginHelper.Writeline("x86 Process")
            End If
            If Environment.Is64BitOperatingSystem Then
                PluginHelper.Writeline("x64 OS")
            Else
                PluginHelper.Writeline("x86 OS")
            End If

            Manager = New PluginManager(Me)

            Try
                PluginHelper.StartLoading("Updating plugins...")
                If Await Task.Run(Function() As Boolean
                                      Return RedistributionHelpers.DownloadAllPlugins(Manager, "http://dl.uniquegeeks.net/SkyEditorPluginsBeta/plugins.json")
                                  End Function) Then
                    PluginHelper.StopLoading()
                    RedistributionHelpers.RestartProgram()
                End If
                PluginHelper.StopLoading()
            Catch ex As Exception
                PluginHelper.StopLoading()
                PluginHelper.Writeline("Unable to update plugins.  Error: " & ex.ToString, PluginHelper.LineType.Error)
            End Try


            'Make sure there's actually a plugin loaded.
            If False AndAlso Manager.Plugins.Count = 0 Then
                MessageBox.Show("This version of Sky Editor requires at least one compatible extension in the Plugins folder to be loaded.  Sky Editor will now shut down.")
                Me.Close()
            End If

            'Initialize Dialogs
            OpenFileDialog1 = New Forms.OpenFileDialog
            OpenFileDialog1.Filter = Manager.IOFiltersString
            SaveFileDialog1 = New Forms.SaveFileDialog
            SaveFileDialog1.Filter = OpenFileDialog1.Filter


            'Check command line arguments to see if a file needs to be loaded
            Dim fileToLoad As String = ""
            Dim args() As String = Environment.GetCommandLineArgs
            If args.Length > 1 Then
                'Code commented out should work if published with ClickOnce.  Should, so test before checking this in.
                'If args(1).ToLower.EndsWith(".application") Then
                '    'This is running in ClickOnce, which has a special way of loading files
                '    'TODO: use clickonce classes to get FileToLoad
                'Else
                fileToLoad = args(1)
                'End If
            End If

            'Load Language

            Me.Title = String.Format(PluginHelper.GetLanguageItem("Title", "Sky Editor Beta v{0}"), My.Application.Info.Version.ToString(2))
            menuFile.Header = PluginHelper.GetLanguageItem("File")
            menuNew.Header = PluginHelper.GetLanguageItem("New")
            menuFileOpen.Header = PluginHelper.GetLanguageItem("Open")
            menuFileOpenAuto.Header = PluginHelper.GetLanguageItem("OpenAutoDetect", "Open (Auto-Detect Game)")
            menuFileOpenNoDetect.Header = PluginHelper.GetLanguageItem("OpenNoAuto", "Open (Let me choose the Game)")
            menuFileSaveAs.Header = PluginHelper.GetLanguageItem("SaveAs", "Save As...")
            menuCredits.Header = PluginHelper.GetLanguageItem("CreditsLabel", "Credits")
            menuCheats.Header = PluginHelper.GetLanguageItem("Cheats")
            menuDebug.Header = PluginHelper.GetLanguageItem("Debug")
            menuShowConsole.Header = PluginHelper.GetLanguageItem("DebugConsole", "Show Console")
            menuDebugConsole.Header = PluginHelper.GetLanguageItem("RunCommand", "Run Command...")

            tcTabs.TabStripPlacement = Settings.TabStripPlacement


            'Add or remove cheats menu if there's plugins
            If Manager.CheatManager.CodeDefinitions.Count = 0 Then
                menuCheats.Visibility = System.Windows.Visibility.Collapsed
            End If

            If Settings.DebugMode Then
                menuDebug.Visibility = System.Windows.Visibility.Visible
            Else
                menuDebug.Visibility = Windows.Visibility.Collapsed
            End If

            Manager.RegisterConsoleCommand("distprep", AddressOf RedistributionHelpers.PrepareForDistribution)
            Manager.RegisterConsoleCommand("zip", AddressOf RedistributionHelpers.PackProgram)
            Manager.RegisterConsoleCommand("packplug", AddressOf RedistributionHelpers.PackPlugins)
            'Manager.RegisterConsoleCommand("unplug", AddressOf RedistributionHelpers.UnpackPlugins)
            Manager.RegisterConsoleCommand("delplug", AddressOf RedistributionHelpers.DeletePlugin)
            'Manager.RegisterConsoleCommand("updateplug", AddressOf RedistributionHelpers.InstallUnknownPlugins)
            Manager.RegisterConsoleCommand("generateinfo", AddressOf RedistributionHelpers.GeneratePluginDownloadDir)
            Manager.RegisterConsoleCommand("updateall", AddressOf RedistributionHelpers.DownloadAllPlugins)

            If fileToLoad.Length > 0 Then
                Manager.LoadSave(fileToLoad)
            Else
                'If no save was automatically loded...
                tcTabs.Items.Add(New WelcomeTab)
                tcTabs.SelectedIndex = 0
            End If
        End Sub




#Region "iMainWindow Support"
        Public Sub AddMenuItem(Menu As MenuItem) Implements iMainWindow.AddMenuItem
            menuMain.Items.Add(Menu)
        End Sub

        Public Sub AddTabItem(SaveName As String, Tab As TabItem) Implements iMainWindow.AddTabItem
            If Not tabs.ContainsKey(SaveName) Then
                tabs.Add(SaveName, New List(Of EditorTab))
            End If
            tabs(SaveName).Add(Tab)
        End Sub

        Public Sub ClearTabItems() Implements iMainWindow.ClearTabItems
            tcTabs.Items.Clear()
        End Sub

        Public Function GetTabItems() As ItemCollection Implements iMainWindow.GetTabItems
            Return tcTabs.Items
        End Function
#End Region

        Public Function GetMenuItems() As ItemCollection Implements iMainWindow.GetMenuItems
            Return menuMain.Items
        End Function

        Public Sub RemoveMenuItem(Menu As MenuItem) Implements iMainWindow.RemoveMenuItem
            menuMain.Items.Remove(Menu)
        End Sub

        Private Sub lbSaves_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lbSaves.SelectionChanged
            Manager.CurrentSave = DirectCast(lbSaves.SelectedItem, GenericSave).Name
        End Sub
        Dim oldSelectedSaveName As String = ""
        Private Sub Manager_CurrentSaveChanged(sender As Object, e As PluginManager.SaveChangedEventArgs) Handles Manager.CurrentSaveChanged
            If Not String.IsNullOrEmpty(oldSelectedSaveName) Then
                tabs(oldSelectedSaveName).Clear()
                For Each item In tcTabs.Items
                    tabs(oldSelectedSaveName).Add(item)
                Next
            End If
            tcTabs.Items.Clear()
            If lbSaves.SelectedItem IsNot Nothing Then
                oldSelectedSaveName = DirectCast(lbSaves.SelectedItem, GenericSave).Name
                For Each item In tabs(oldSelectedSaveName)
                    tcTabs.Items.Add(item)
                Next
            End If
        End Sub

        Private Sub Manager_SaveAdded(sender As Object, e As PluginManager.SaveAddedEventArgs) Handles Manager.SaveAdded
            lbSaves.Items.Add(e.Save)
            If lbSaves.SelectedIndex < 0 Then
                lbSaves.SelectedIndex = 0
            End If
        End Sub
    End Class
End Namespace
