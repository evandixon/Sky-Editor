Imports System.Reflection
Imports System.Text
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities
Imports SkyEditor.Core.Settings
Imports System.Windows.Input
Imports System.Collections.Specialized
''' <summary>
''' Class that manages open files, solutions, and projects, and helps with the UI display them.
''' </summary>
Public Class IOUIManager
    Implements IDisposable
    Implements INotifyPropertyChanged

    Public Sub New(manager As PluginManager)
        Me.CurrentPluginManager = manager
        Me.CurrentSolution = Nothing
        Me.OpenedProjectFiles = New Dictionary(Of Object, Project)
        Me.FileDisposalSettings = New Dictionary(Of Object, Boolean)
        Me.OpenFiles = New ObservableCollection(Of AvalonDockFileWrapper)
        Me.RunningTasks = New ObservableCollection(Of Task)
        AnchorableViewModels = New ObservableCollection(Of AnchorableViewModel)
    End Sub

#Region "Events"
    Public Event SolutionChanged(sender As Object, e As EventArgs)
    Public Event CurrentProjectChanged(sender As Object, e As EventArgs)
    Public Event FileOpened(sender As Object, e As FileOpenedEventArguments)
    Public Event FileClosing(sender As Object, e As FileClosingEventArgs)
    Public Event FileClosed(sender As Object, e As FileClosedEventArgs)
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
#End Region

#Region "Event Handlers"

    '<Obsolete> Private Sub _pluginHelper_FileOpened(sender As Object, e As FileOpenedEventArguments)
    '    If Not Me.OpenedFiles.ContainsKey(e.File) Then
    '        Me.OpenedFiles.Add(e.File, e.ParentProject)
    '    End If
    'End Sub

    '<Obsolete> Private Sub _pluginHelper_FileClosed(sender As Object, e As FileClosedEventArgs)
    '    If Me.OpenedFiles.ContainsKey(e.File) Then
    '        Me.OpenedFiles.Remove(e.File)
    '    End If
    'End Sub

    Private Sub IOUIManager_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles Me.PropertyChanged
        For Each item In RootMenuItems
            UpdateMenuItemVisibility(item, GetMenuActionTargets)
        Next
    End Sub

    Private Sub _openFiles_CollectionChanged(sender As Object, e As NotifyCollectionChangedEventArgs) Handles _openFiles.CollectionChanged
        If e.NewItems IsNot Nothing Then
            For Each item As AvalonDockFileWrapper In e.NewItems
                AddHandler item.CloseCommandExecuted, AddressOf File_OnClosed
            Next
        End If

        If e.OldItems IsNot Nothing Then
            For Each item As AvalonDockFileWrapper In e.OldItems
                RemoveHandler item.CloseCommandExecuted, AddressOf File_OnClosed
            Next
        End If
    End Sub

    Private Sub File_OnClosed(sender As Object, e As EventArgs)
        Dim args As New FileClosingEventArgs
        args.File = sender

        RaiseEvent FileClosing(Me, args)

        If Not args.Cancel Then
            'Doing the directcast again in case something changed args
            CloseFile(DirectCast(sender, AvalonDockFileWrapper).File)
        End If
    End Sub
#End Region

    Public Property CurrentPluginManager As PluginManager

    ''' <summary>
    ''' The files that are currently open
    ''' </summary>
    ''' <returns></returns>
    Public Property OpenFiles As ObservableCollection(Of AvalonDockFileWrapper)
        Get
            Return _openFiles
        End Get
        Set(value As ObservableCollection(Of AvalonDockFileWrapper))
            _openFiles = value
        End Set
    End Property
    Private WithEvents _openFiles As ObservableCollection(Of AvalonDockFileWrapper)

    Public Property AnchorableViewModels As ObservableCollection(Of AnchorableViewModel)
        Get
            Return _anchorableViewModels
        End Get
        Set(value As ObservableCollection(Of AnchorableViewModel))
            _anchorableViewModels = value
        End Set
    End Property
    Dim _anchorableViewModels As ObservableCollection(Of AnchorableViewModel)

    Public Property SupportedToolWindowTypes As IEnumerable(Of Type)

    ''' <summary>
    ''' Gets or sets the selected file
    ''' </summary>
    ''' <returns></returns>
    Public Property SelectedFile As AvalonDockFileWrapper
        Get
            Return _selectedFile
        End Get
        Set(value As AvalonDockFileWrapper)
            If _selectedFile IsNot value Then
                'If we actually changed something...

                'Update the current
                _selectedFile = value

                'And report something changed
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SelectedFile)))
            End If
        End Set
    End Property
    Dim _selectedFile As AvalonDockFileWrapper

    Public Property ActiveContent As Object
        Get
            Return _activeContent
        End Get
        Set(value As Object)
            'Only update if we changed something
            If _activeContent IsNot value Then
                _activeContent = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ActiveContent)))
            End If

            'If the active content is a file, update the active file
            If TypeOf value Is AvalonDockFileWrapper Then
                SelectedFile = value
            End If
        End Set
    End Property
    Dim _activeContent As Object

    ''' <summary>
    ''' Stores whether or not to dispose of files on close
    ''' </summary>
    ''' <returns></returns>
    Private Property FileDisposalSettings As Dictionary(Of Object, Boolean)

    ''' <summary>
    ''' Matches opened files to their parent projects
    ''' </summary>
    ''' <returns></returns>
    Private Property OpenedProjectFiles As Dictionary(Of Object, Project)

    ''' <summary>
    ''' Dictionary of (Extension, Friendly Name) used in the Open and Save file dialogs.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IOFilters As New Dictionary(Of String, String)

    Public Property CurrentSolution As Solution
        Get
            Return _currentSolution
        End Get
        Set(value As Solution)
            If _currentSolution IsNot value Then
                'If we're actually changing values...

                'Dispose of the old one
                If _currentSolution IsNot Nothing Then
                    _currentSolution.Dispose()
                End If

                'Update the current
                _currentSolution = value

                'And report that we changed something
                RaiseEvent SolutionChanged(Me, New EventArgs)
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CurrentSolution)))
            End If
        End Set
    End Property
    Private WithEvents _currentSolution As Solution

    Public Property CurrentProject As Project
        Get
            Return _currentProject
        End Get
        Set(value As Project)
            If _currentProject IsNot value Then
                'If we're actually changing values...

                'Update the current
                _currentProject = value

                'And report that we changed something
                RaiseEvent CurrentProjectChanged(Me, New EventArgs)
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CurrentProject)))
            End If
        End Set
    End Property
    Private WithEvents _currentProject As Project

    Public Property RootMenuItems As ObservableCollection(Of ActionMenuItem)
        Get
            If _rootMenuItems Is Nothing Then
                _rootMenuItems = New ObservableCollection(Of ActionMenuItem)
                'Generate the menu items
                For Each item In UIHelper.GenerateLogicalMenuItems(UIHelper.GetMenuItemInfo(CurrentPluginManager, CurrentPluginManager.CurrentSettingsProvider.GetIsDevMode), Me, Nothing)
                    _rootMenuItems.Add(item)
                Next
                'Update their visibility now that all of them have been created
                'Doing this before they're all created will cause unintended behavior
                Dim targets = GetMenuActionTargets()
                For Each item In _rootMenuItems
                    UpdateMenuItemVisibility(item, targets)
                Next
            End If
            Return _rootMenuItems
        End Get
        Protected Set(value As ObservableCollection(Of ActionMenuItem))
            _rootMenuItems = value
        End Set
    End Property
    Dim _rootMenuItems As ObservableCollection(Of ActionMenuItem)

    Public Property RunningTasks As ObservableCollection(Of Task)

#Region "Functions"

#Region "IO Filters"
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
    Public Function IOFiltersStringSaveAs(DefaultExtension As String) As String
        Dim ext As String = DefaultExtension
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
    End Function

    ''' <summary>
    ''' Registers a filter for use in open and save file dialogs.
    ''' </summary>
    ''' <param name="FileExtension">Filter for the dialog.  If this is by extension, should be *.extension</param>
    ''' <param name="FileFormatName">Name of the file format</param>
    Public Sub RegisterIOFilter(FileExtension As String, FileFormatName As String)
        Dim TempIOFilters As Dictionary(Of String, String) = IOFilters
        If TempIOFilters Is Nothing Then
            TempIOFilters = New Dictionary(Of String, String)
        End If
        If Not TempIOFilters.ContainsKey(FileExtension) Then
            TempIOFilters.Add(FileExtension, FileFormatName)
        End If
        IOFilters = TempIOFilters
    End Sub

    ''' <summary>
    ''' Gets the filter for a Windows Forms Open or Save File Dialog for use with Sky Editor Projects.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetProjectIOFilter() As String
        Return $"{My.Resources.Language.SkyEditorProjects} (*.skyproj)|*.skyproj|{My.Resources.Language.AllFiles} (*.*)|*.*"
    End Function
#End Region

#Region "Open/Close File"

    ''' <summary>
    ''' Opens the given file
    ''' </summary>
    ''' <param name="File">File to open</param>
    ''' <param name="DisposeOnClose">True to call the file's dispose method (if IDisposable) when closed.</param>
    Public Sub OpenFile(File As Object, DisposeOnClose As Boolean)
        If File IsNot Nothing Then
            If Not (From o In OpenFiles Where o.File Is File).Any Then
                Dim wrapper As New AvalonDockFileWrapper
                wrapper.File = File
                OpenFiles.Add(wrapper)
                FileDisposalSettings.Add(File, DisposeOnClose)
                RaiseEvent FileOpened(Nothing, New FileOpenedEventArguments With {.File = File, .DisposeOnExit = DisposeOnClose})
            End If
            If SelectedFile Is Nothing AndAlso OpenFiles.Count > 0 Then
                SelectedFile = OpenFiles.First
            End If
        End If
    End Sub

    ''' <summary>
    ''' Opens the file
    ''' </summary>
    ''' <param name="File">File to open</param>
    ''' <param name="ParentProject">Project the file belongs to.  If the file does not belong to a project, don't use this overload.</param>
    Public Sub OpenFile(File As Object, ParentProject As Project)
        If File IsNot Nothing Then
            If Not (From o In OpenFiles Where o.File Is File).Any Then
                Dim wrapper As New AvalonDockFileWrapper
                wrapper.File = File
                OpenFiles.Add(wrapper)
                OpenedProjectFiles.Add(File, ParentProject)
                RaiseEvent FileOpened(Nothing, New FileOpenedEventArguments With {.File = File, .DisposeOnExit = False, .ParentProject = ParentProject})
            End If
            If SelectedFile Is Nothing AndAlso OpenFiles.Count > 0 Then
                SelectedFile = OpenFiles.First
            End If
        End If
    End Sub

    ''' <summary>
    ''' Closes the file
    ''' </summary>
    ''' <param name="File">File to close</param>
    Public Sub CloseFile(File As Object)
        If File IsNot Nothing Then
            Dim toDelete = (From o In OpenFiles Where o.File Is File)
            For count = OpenFiles.Count - 1 To 0 Step -1
                If OpenFiles(count).File Is File Then
                    OpenFiles.RemoveAt(count)
                End If
            Next

            Dim doDispose = (FileDisposalSettings.ContainsKey(File) AndAlso FileDisposalSettings(File))
            If doDispose Then
                If TypeOf File Is IDisposable Then
                    DirectCast(File, IDisposable).Dispose()
                End If
            End If
            RaiseEvent FileClosed(Me, New FileClosedEventArgs With {.File = File, .Disposed = doDispose})
        End If
    End Sub
#End Region

    ''' <summary>
    ''' Returns the file's parent project, if it exists.
    ''' </summary>
    ''' <param name="File">File of which to get the parent project.  Must be an open file, otherwise the function will return Nothing.</param>
    ''' <returns></returns>
    Public Function GetOpenedFileProject(File As Object) As Project
        If Me.OpenedProjectFiles.ContainsKey(File) Then
            Return Me.OpenedProjectFiles(File)
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Gets the possible targets for a menu action.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetMenuActionTargets() As IEnumerable(Of Object)
        Dim out As New List(Of Object)

        If CurrentSolution IsNot Nothing Then
            out.Add(CurrentSolution)
        End If

        If CurrentProject IsNot Nothing Then
            out.Add(CurrentProject)
        End If

        If SelectedFile IsNot Nothing Then
            out.Add(SelectedFile.File)
        End If

        Return out
    End Function

    ''' <summary>
    ''' Gets the targets for the given menu action
    ''' </summary>
    ''' <param name="action">The action for which to retrieve the targets</param>
    ''' <returns></returns>
    Public Function GetMenuActionTargets(action As MenuAction) As IEnumerable(Of Object)
        Dim targets As New List(Of Object)

        'Add the current project to the targets if supported
        If CurrentSolution IsNot Nothing AndAlso action.SupportsObject(CurrentSolution) Then
            targets.Add(CurrentSolution)
        End If

        'Add the current project if supported
        If CurrentProject IsNot Nothing AndAlso action.SupportsObject(CurrentProject) Then
            targets.Add(CurrentProject)
        End If

        'Add the selected file if supported
        If SelectedFile IsNot Nothing AndAlso action.SupportsObject(SelectedFile.File) Then
            targets.Add(SelectedFile.File)
        End If

        Return targets
    End Function

    ''' <summary>
    ''' Lets the IOUIManager keep track of the current task
    ''' </summary>
    ''' <param name="task">Task to keep track of.</param>
    Public Sub LogTask(task As Task)
        Dim watchTask = Task.Run(Async Function() As Task
                                     Await task
                                     RemoveTask(task)
                                 End Function)
    End Sub

    Private Sub RemoveTask(task As Task)
        RunningTasks.Remove(task)
    End Sub

    ''' <summary>
    ''' Updates the visibility for the given menu item and its children, and returns the updated visibility
    ''' </summary>
    ''' <param name="menuItem"></param>
    ''' <param name="targets"></param>
    ''' <returns></returns>
    Private Function UpdateMenuItemVisibility(menuItem As ActionMenuItem, targets As IEnumerable(Of Object)) As Boolean

        'Default to not visible
        Dim isVisible = False

        If menuItem.Actions IsNot Nothing Then
            'Visibility is determined by every available action
            'If any one of those actions is applicable, then this menu item is visible
            For Each item In menuItem.Actions
                If Not isVisible Then
                    If item.AlwaysVisible Then
                        'Then this action is always visible
                        isVisible = True

                        'And don't bother checking the rest
                        Exit For
                    Else
                        For Each target In targets
                            'Check to see if this target is supported
                            If item.SupportsObject(target) Then
                                'If it is, then this menu item should be visible
                                isVisible = True

                                'And don't bother checking the rest
                                Exit For
                            End If
                        Next
                    End If
                Else
                    'Then this menu item is visible, and don't bother checking the rest
                    Exit For
                End If
            Next
        End If

        'Update children
        For Each item In menuItem.Children
            If UpdateMenuItemVisibility(item, targets) Then
                isVisible = True
            End If
        Next

        'Set the visibility to the value we calculated
        menuItem.IsVisible = isVisible

        'Set this item to visible if there's a visible
        Return isVisible
    End Function

    Public Sub ShowAnchorable(model As AnchorableViewModel)
        Dim targetType = model.GetType
        If Not (From m In AnchorableViewModels Where ReflectionHelpers.IsOfType(m, targetType.GetTypeInfo, False)).Any Then
            AnchorableViewModels.Add(model)
        End If
    End Sub

#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If CurrentSolution IsNot Nothing Then
                    CurrentSolution.Dispose()
                End If
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
