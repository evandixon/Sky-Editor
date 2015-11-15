Imports System.ComponentModel
Imports System.Reflection
Imports System.Threading.Tasks
Imports System.Timers
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SkyEditorBase.Redistribution
Imports Xceed.Wpf.AvalonDock.Layout

Public Class MainWindow2

#Region "Private Variables"
    Private WithEvents _manager As PluginManager
    Private WithEvents _projectExplorer As ProjectExplorer
    Private WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Private WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Private _queuedConsoleLines As Queue(Of PluginHelper.ConsoleLineWrittenEventArgs)
#End Region
    Private Function IsFileTabOpen(File As Object) As Boolean
        Dim out As Boolean = False
        For Each item In docPane.Children
            If TypeOf item Is DocumentTab AndAlso DirectCast(item, DocumentTab).Document Is File Then
                out = True
                Exit For
            End If
        Next
        Return out
    End Function

    ''' <summary>
    ''' Opens the given object in a document tab, if it is not already open.
    ''' </summary>
    ''' <param name="Document"></param>
    Private Sub OpenDocumentTab(Document As Object, DisposeOnExit As Boolean)
        If Not IsFileTabOpen(Document) Then
            docPane.Children.Add(New DocumentTab(Document, _manager, DisposeOnExit))
            RemoveWelcomePage()
        End If
    End Sub


    Private Sub SaveProject()
        _manager.CurrentProject.SaveProject()
    End Sub


    Private Sub _projectExplorer_FileOpened(sender As Object, e As EventArguments.FileOpenedEventArguments) Handles _projectExplorer.FileOpen
        Dispatcher.Invoke(Sub()
                              docPane.Children.Add(New DocumentTab(e.File, _manager))
                              RemoveWelcomePage()
                          End Sub)
    End Sub


#Region "Event Handlers"

#Region "MenuItems"
    Private Sub menuNew_Click(sender As Object, e As RoutedEventArgs) Handles menuNew.Click
        'Get what kind of file to create.
        Dim w As New SkyEditorWindows.NewFileWindow()
        Dim games As New Dictionary(Of String, Type)
        For Each item In _manager.CreatableFiles
            games.Add(PluginHelper.GetLanguageItem(item.Name), item)
        Next
        w.AddGames(games.Keys)
        If w.ShowDialog Then
            Dim file As Object = _manager.CreateNewFile(w.SelectedName, games(w.SelectedGame))
            docPane.Children.Add(New DocumentTab(file, _manager, True))
            RemoveWelcomePage()
        End If
    End Sub
    Private Sub menuFileOpenAuto_Click(sender As Object, e As RoutedEventArgs) Handles menuFileOpenAuto.Click
        OpenFileDialog1.Filter = _manager.IOFiltersString
        If OpenFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
            If OpenFileDialog1.FileName.ToLower.EndsWith(".skyproj") Then
                _manager.CurrentProject = Project.OpenProject(OpenFileDialog1.FileName, _manager)
            Else
                docPane.Children.Add(New DocumentTab(_manager.OpenFile(OpenFileDialog1.FileName), _manager, True))
            End If
            RemoveWelcomePage()
        End If
    End Sub
    Private Sub menuFileOpenNoDetect_Click(sender As Object, e As RoutedEventArgs) Handles menuFileOpenNoDetect.Click
        OpenFileDialog1.Filter = _manager.IOFiltersString
        If OpenFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
            Dim w As New SkyEditorWindows.GameTypeSelector()
            Dim games As New Dictionary(Of String, Type)
            For Each item In _manager.OpenableFiles
                games.Add(PluginHelper.GetLanguageItem(item.Name), item)
            Next
            w.AddGames(games.Keys)
            If w.ShowDialog Then
                OpenDocumentTab(_manager.OpenFile(OpenFileDialog1.FileName, games(w.SelectedGame)), True)
            End If
        End If
    End Sub
    Private Sub menuFileSaveFile_Click(sender As Object, e As RoutedEventArgs) Handles menuFileSaveFile.Click
        If docPane.SelectedContent IsNot Nothing Then
            Dim file = DirectCast(docPane.SelectedContent, DocumentTab).Document
            If TypeOf file Is iOnDisk AndAlso String.IsNullOrEmpty(DirectCast(file, iOnDisk).Filename) Then
                If TypeOf file Is iSavable Then
                    If TypeOf file Is iOnDisk Then
                        SaveFileDialog1.Filter = _manager.IOFiltersStringSaveAs(IO.Path.GetExtension(DirectCast(file, iOnDisk).Filename))
                    Else
                        SaveFileDialog1.Filter = _manager.IOFiltersString(IsSaveAs:=True) 'Todo: use default extension
                    End If
                    If SaveFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                        DirectCast(file, iSavable).Save(SaveFileDialog1.FileName)
                    End If
                End If
            Else
                If TypeOf file Is iSavable Then
                    DirectCast(file, iSavable).Save()
                End If
            End If
        End If
    End Sub
    Private Sub menuFileSaveAs_Click(sender As Object, e As RoutedEventArgs) Handles menuFileSaveAs.Click
        If docPane.SelectedContent IsNot Nothing Then
            'Dim tab = DirectCast(docPane.SelectedContent, DocumentTab)
            Dim file = DirectCast(docPane.SelectedContent, DocumentTab).Document
            If TypeOf file Is iSavable Then
                SaveFileDialog1.Filter = _manager.IOFiltersStringSaveAs(IO.Path.GetExtension(DirectCast(file, iSavable).DefaultExtension))
                If SaveFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                    DirectCast(file, iSavable).Save(SaveFileDialog1.FileName)
                End If
            End If
        End If
    End Sub
    Private Sub menuFileSaveProject_Click(sender As Object, e As RoutedEventArgs) Handles menuFileSaveProject.Click
        SaveProject()
    End Sub
    Private Sub menuFileSaveAll_Click(sender As Object, e As RoutedEventArgs) Handles menuFileSaveAll.Click
        SaveProject()
        For Each item In docPane.Children
            If TypeOf item Is DocumentTab Then
                Dim file = DirectCast(item, DocumentTab).Document
                If Not String.IsNullOrEmpty(file.OriginalFilename) Then
                    If TypeOf file Is iSavable Then
                        DirectCast(file, iSavable).Save()
                    End If
                Else
                    SaveFileDialog1.Filter = _manager.IOFiltersStringSaveAs(IO.Path.GetExtension(file.OriginalFilename))
                    If SaveFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                        If TypeOf file Is iSavable Then
                            DirectCast(file, iSavable).Save(SaveFileDialog1.FileName)
                        End If
                    End If
                End If
            End If
        Next
    End Sub
    Private Sub menuNewProject_Click(sender As Object, e As RoutedEventArgs) Handles menuNewProject.Click
        Dim newProj As New NewProjectWindow(_manager)
        If newProj.ShowDialog() Then
            _manager.CurrentProject = Project.CreateProject(newProj.SelectedName, newProj.SelectedLocation, _manager.ProjectTypes(newProj.SelectedProjectType), _manager)
            RemoveWelcomePage()
        End If
    End Sub

    Private Sub menuLanguageEditor_Click(sender As Object, e As RoutedEventArgs) Handles menuLanguageEditor.Click
        OpenDocumentTab(SkyEditorBase.Language.LanguageManager.Instance, False)
    End Sub

    Private Sub MenuSettings_Click(sender As Object, e As RoutedEventArgs) Handles MenuSettings.Click
        OpenDocumentTab(SettingsManager.Instance, False)
    End Sub

    Private Async Sub menuBuild_Click(sender As Object, e As RoutedEventArgs) Handles menuBuild.Click
        Try
            Await _manager.CurrentProject.BuildAsync()
        Catch ex As Exception
            MessageBox.Show(PluginHelper.GetLanguageItem("Failed to build project.  See output for details."))
            PluginHelper.Writeline(ex.ToString, PluginHelper.LineType.Error)
            PluginHelper.SetLoadingStatusFailed
        End Try
    End Sub

    Private Sub menuRun_Click(sender As Object, e As RoutedEventArgs) Handles menuRun.Click
        _manager.CurrentProject.Run()
    End Sub

#End Region

#Region "Form"
    Private Sub MainWindow2_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        OpenFileDialog1 = New Forms.OpenFileDialog
        SaveFileDialog1 = New Forms.SaveFileDialog

        'menuFileOpenNoDetect.Visibility = Visibility.Collapsed

        _projectExplorer = New ProjectExplorer(_manager)
        toolbarPaneRight.Children.Add(_projectExplorer.ParentAnchorable)

        Me.Title = PluginHelper.GetLanguageItem("Sky Editor") & " Alpha " & Assembly.GetExecutingAssembly.GetName.Version.ToString

        _manager.RegisterIOFilter("*.skyproj", PluginHelper.GetLanguageItem("Sky Editor Project File"))

        RefreshBuildRunVisibility()

        ShowWelcomePage()

        TranslateControls()

        AddHandler PluginHelper.LoadingMessageChanged, AddressOf OnLoadingMessageChanged
        AddHandler PluginHelper.ConsoleLineWritten, AddressOf OnConsoleLineWritten

    End Sub
    Private Sub TranslateControls()
        PluginHelper.TranslateForm(menuMain)
        'toolbarOutput.Title = PluginHelper.GetLanguageItem("Output")
        lblStatus.Content = PluginHelper.GetLanguageItem("Ready")
    End Sub

    Private Sub MainWindow2_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Dim editedTabs = From t In docPane.Children Where TypeOf t Is DocumentTab AndAlso DirectCast(t, DocumentTab).IsModified = True

        Dim editsMade As Boolean = editedTabs.Any OrElse (_manager.CurrentProject IsNot Nothing AndAlso _manager.CurrentProject.IsModified)
        If editsMade Then
            If MessageBox.Show(PluginHelper.GetLanguageItem("Unsaved File Close Confirmation", "Are you sure you want to exit Sky Editor?  Any unsaved changes will be lost."), PluginHelper.GetLanguageItem("Sky Editor"), MessageBoxButton.YesNo) = MessageBoxResult.No Then
                e.Cancel = True
            End If
        End If
        For count As Integer = docPane.ChildrenCount - 1 To 0 Step -1
            Dim item = docPane.Children(count)
            If TypeOf item Is DocumentTab Then
                DirectCast(item, DocumentTab).Dispose()
            End If
        Next
        _manager.Dispose()
    End Sub
#End Region


    Private Sub _manager_MenuItemRegistered(sender As Object, Item As MenuItem) Handles _manager.MenuItemRegistered
        menuMain.Items.Add(Item)
    End Sub

    Private Sub OnLoadingMessageChanged(sender As Object, e As PluginHelper.LoadingMessageChangedEventArgs)
        Dispatcher.Invoke(New Action(Sub()
                                         lblStatus.Content = e.NewMessage
                                         progressBar.IsIndeterminate = e.IsIndeterminate
                                         progressBar.Value = e.Progress ' * 100
                                     End Sub))
    End Sub
    Private Sub OnConsoleLineWritten(sender As Object, e As PluginHelper.ConsoleLineWrittenEventArgs)
        If Not e.Type = PluginHelper.LineType.ConsoleOutput OrElse SettingsManager.Instance.Settings.VerboseOutput Then
            '_queuedConsoleLines.Enqueue(e)
            Dispatcher.InvokeAsync(New Action(Sub()
                                                  txtOutput.AppendText(e.Line)
                                                  txtOutput.AppendText(vbCrLf)
                                                  txtOutput.ScrollToEnd()
                                              End Sub))
        End If
    End Sub

    Private Sub _manager_ProjectChanged(sender As Object, NewProject As Project) Handles _manager.ProjectChanged
        RefreshBuildRunVisibility()
    End Sub
#End Region
    Private Sub RefreshBuildRunVisibility()
        If _manager.CurrentProject IsNot Nothing AndAlso _manager.CurrentProject.CanBuild Then
            menuBuild.Visibility = Visibility.Visible
        Else
            menuBuild.Visibility = Visibility.Collapsed
        End If
        If _manager.CurrentProject IsNot Nothing AndAlso _manager.CurrentProject.CanRun Then
            menuRun.Visibility = Visibility.Visible
        Else
            menuRun.Visibility = Visibility.Collapsed
        End If
    End Sub
    Private Sub ShowWelcomePage()
        Dim l As New LayoutDocument
        l.Content = New WelcomeTabContent
        l.Title = PluginHelper.GetLanguageItem("Welcome")
        docPane.Children.Add(l)
    End Sub
    Private Sub RemoveWelcomePage()
        Dim tabs As New List(Of LayoutDocument)
        For Each item In docPane.Children
            If TypeOf item.Content Is WelcomeTabContent Then
                tabs.Add(item)
            End If
        Next
        For Each item In tabs
            docPane.Children.Remove(item)
        Next
    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _manager = PluginManager.GetInstance
    End Sub
    Public Sub New(Manager As PluginManager)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _manager = Manager
    End Sub
End Class
