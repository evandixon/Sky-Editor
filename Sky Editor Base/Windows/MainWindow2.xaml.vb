Imports System.ComponentModel
Imports System.Reflection
Imports System.Threading.Tasks
Imports System.Timers
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SkyEditorBase.Language
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

    Private Function GetSelectedDocumentObject() As Object
        Dim current = docPane.SelectedContent
        If current IsNot Nothing AndAlso TypeOf current Is DocumentTab Then
            Return DirectCast(current, DocumentTab).Document
        Else
            Return Nothing
        End If
    End Function

    Private Function GetMenuActionTargets() As IEnumerable(Of Object)
        Dim targets As New List(Of Object)

        'Add the current project to the targets if supported
        If _manager.CurrentProject IsNot Nothing Then
            targets.Add(_manager.CurrentProject)
        End If

        Dim currentDocumentObject = GetSelectedDocumentObject()
        If currentDocumentObject IsNot Nothing Then
            targets.Add(currentDocumentObject)
        End If
        Return targets
    End Function

    Private Function GetMenuActionTargets(Action As MenuAction) As IEnumerable(Of Object)
        Dim targets As New List(Of Object)

        'Add the current project to the targets if supported
        If _manager.CurrentProject IsNot Nothing AndAlso Action.SupportsObject(_manager.CurrentProject) Then
            targets.Add(_manager.CurrentProject)
        End If

        If Action.TargetAll Then
            For Each item In docPane.Children
                If TypeOf item Is DocumentTab Then
                    Dim d = DirectCast(item, DocumentTab).Document
                    If d IsNot Nothing AndAlso Action.SupportsObject(d) Then
                        targets.Add(d)
                    End If
                End If
            Next
        Else
            Dim currentDocumentObject = GetSelectedDocumentObject()
            If currentDocumentObject IsNot Nothing AndAlso Action.SupportsObject(currentDocumentObject) Then
                targets.Add(currentDocumentObject)
            End If
        End If

        Return targets
    End Function


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

    Private Async Sub MenuItemClicked(sender As Object, e As EventArgs)
        Try
            If sender IsNot Nothing AndAlso TypeOf sender Is MenuItem Then
                Dim m As MenuItem = DirectCast(sender, MenuItem)
                Dim tasks As New List(Of Task)
                If m.Tag IsNot Nothing AndAlso TypeOf m.Tag Is List(Of MenuAction) Then
                    Dim tags = DirectCast(m.Tag, List(Of MenuAction))
                    For Each t In tags
                        tasks.Add(t.DoAction(GetMenuActionTargets(t)))
                    Next
                End If
                Await Task.WhenAll(tasks)
            End If
        Catch ex As Exception
            MessageBox.Show(PluginHelper.GetLanguageItem("An error has occurred.  See output for details."))
            PluginHelper.Writeline(ex.ToString, PluginHelper.LineType.Error)
            PluginHelper.SetLoadingStatusFailed()
        End Try
    End Sub

#Region "Form"
    Private Sub MainWindow2_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        OpenFileDialog1 = New Forms.OpenFileDialog
        SaveFileDialog1 = New Forms.SaveFileDialog

        'menuFileOpenNoDetect.Visibility = Visibility.Collapsed

        _projectExplorer = New ProjectExplorer(_manager)
        toolbarPaneRight.Children.Add(_projectExplorer.ParentAnchorable)

        Me.Title = PluginHelper.GetLanguageItem("Sky Editor") & " Alpha " & Assembly.GetExecutingAssembly.GetName.Version.ToString

        _manager.RegisterIOFilter("*.skyproj", PluginHelper.GetLanguageItem("Sky Editor Project File"))

        ShowWelcomePage()

        TranslateControls()

        For Each item In _manager.GetRootMenuItems
            menuMain.Items.Add(item)
            RegisterEventMenuItemHandlers(item)
        Next

        _manager.UpdateMenuItemVisibility(GetMenuActionTargets)

        AddHandler PluginHelper.LoadingMessageChanged, AddressOf OnLoadingMessageChanged
        AddHandler PluginHelper.ConsoleLineWritten, AddressOf OnConsoleLineWritten
        AddHandler PluginHelper.FileOpenRequested, AddressOf OnFileOpenRequested
        AddHandler PluginHelper.ExceptionThrown, AddressOf OnExceptionThrown
    End Sub

    ''' <summary>
    ''' Adds MenuItemClicked(Object,EventArgs) as a handler for MenuItem.Click and its children
    ''' </summary>
    ''' <param name="MenuItem"></param>
    Private Sub RegisterEventMenuItemHandlers(MenuItem As MenuItem)
        For Each Item In MenuItem.Items
            RegisterEventMenuItemHandlers(Item)
        Next
        AddHandler MenuItem.Click, AddressOf MenuItemClicked
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

    Private Sub OnFileOpenRequested(sender As Object, e As EventArguments.FileOpenedEventArguments)
        OpenDocumentTab(e.File, e.DisposeOnExit)
    End Sub

    Private Sub OnExceptionThrown(sender As Object, e As EventArguments.ExceptionThrownEventArgs)
        MessageBox.Show(PluginHelper.GetLanguageItem("GenericErrorMessage", "An error has occurred.  See output for details."))
        PluginHelper.Writeline(e.Exception.ToString, PluginHelper.LineType.Error)
    End Sub

    Private Sub _manager_ProjectChanged(sender As Object, NewProject As Project) Handles _manager.ProjectChanged
        _manager.UpdateMenuItemVisibility(GetMenuActionTargets)
        RemoveWelcomePage()
    End Sub

    Private Sub docPane_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles docPane.PropertyChanged
        If e.PropertyName = "SelectedContent" AndAlso _manager IsNot Nothing Then 'docPane.SelectedContent 
            _manager.UpdateMenuItemVisibility(GetMenuActionTargets)
        End If
    End Sub
#End Region

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
