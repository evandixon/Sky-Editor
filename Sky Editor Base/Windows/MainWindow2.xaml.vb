Imports System.ComponentModel
Imports System.Timers

Public Class MainWindow2

#Region "Private Variables"
    Private WithEvents _manager As PluginManager
    Private WithEvents _projectExplorer As ProjectExplorer
    Private WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Private WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog
    Private _queuedConsoleLines As Queue(Of PluginHelper.ConsoleLineWrittenEventArgs)
#End Region
    Private Function IsFileTabOpen(File As GenericFile) As Boolean
        Dim out As Boolean = False
        For Each item In docPane.Children
            If DirectCast(item, DocumentTab).File Is File Then
                out = True
                Exit For
            End If
        Next
        Return out
    End Function


    Private Sub SaveProject()
        _manager.CurrentProject.SaveProject()
    End Sub


    Private Sub FileOpened(sender As Object, File As KeyValuePair(Of String, GenericFile))
        docPane.Children.Add(New DocumentTab(File.Value, _manager))
    End Sub


#Region "Event Handlers"

#Region "MenuItems"
    Private Sub menuFileOpenAuto_Click(sender As Object, e As RoutedEventArgs) Handles menuFileOpenAuto.Click
        OpenFileDialog1.Filter = _manager.IOFiltersString
        If OpenFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
            If OpenFileDialog1.FileName.ToLower.EndsWith(".skyproj") Then
                _manager.CurrentProject = Project.OpenProject(OpenFileDialog1.FileName, _manager)
            Else
                docPane.Children.Add(New DocumentTab(_manager.OpenFile(OpenFileDialog1.FileName), _manager))
            End If
        End If
    End Sub
    Private Sub menuFileSaveFile_Click(sender As Object, e As RoutedEventArgs) Handles menuFileSaveFile.Click
        If docPane.SelectedContent IsNot Nothing Then
            Dim file = DirectCast(docPane.SelectedContent, DocumentTab).File
            If Not String.IsNullOrEmpty(file.OriginalFilename) Then
                file.Save()
            Else
                SaveFileDialog1.Filter = _manager.IOFiltersStringSaveAs(IO.Path.GetExtension(file.OriginalFilename))
                If SaveFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                    file.Save(SaveFileDialog1.FileName)
                End If
            End If
        End If
    End Sub
    Private Sub menuFileSaveAs_Click(sender As Object, e As RoutedEventArgs) Handles menuFileSaveAs.Click
        If docPane.SelectedContent IsNot Nothing Then
            'Dim tab = DirectCast(docPane.SelectedContent, DocumentTab)
            Dim file = DirectCast(docPane.SelectedContent, DocumentTab).File
            SaveFileDialog1.Filter = _manager.IOFiltersStringSaveAs(IO.Path.GetExtension(file.OriginalFilename))

            If SaveFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                file.Save(SaveFileDialog1.FileName)
            End If
        End If
    End Sub
    Private Sub menuFileSaveProject_Click(sender As Object, e As RoutedEventArgs) Handles menuFileSaveProject.Click
        SaveProject()
    End Sub
    Private Sub menuNewProject_Click(sender As Object, e As RoutedEventArgs) Handles menuNewProject.Click
        Dim newProj As New NewProjectWindow(_manager)
        If newProj.ShowDialog() Then
            _manager.CurrentProject = Project.CreateProject(newProj.SelectedName, newProj.SelectedLocation, newProj.SelectedProjectType, _manager)
        End If
    End Sub

    Private Sub menuBuild_Click(sender As Object, e As RoutedEventArgs) Handles menuBuild.Click
        _manager.CurrentProject.Build()
    End Sub

    Private Sub menuRun_Click(sender As Object, e As RoutedEventArgs) Handles menuRun.Click
        _manager.CurrentProject.Run()
    End Sub

    Private Sub menuFileOpenNoDetect_Click(sender As Object, e As RoutedEventArgs) Handles menuFileOpenNoDetect.Click
        'OpenFileDialog1.Filter = _manager.IOFiltersString
        'If OpenFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
        '    If OpenFileDialog1.FileName.ToLower.EndsWith(".skyproj") Then
        '        _manager.CurrentProject = Project.OpenProject(OpenFileDialog1.FileName, _manager)
        '    Else
        '        'Todo: open auto-detect window
        '        docPane.Children.Add(New DocumentTab(_manager.OpenFile(OpenFileDialog1.FileName), _manager))
        '    End If
        'End If
    End Sub
#End Region

#Region "Form"
    Private Sub MainWindow2_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        _manager = PluginManager.GetInstance

        OpenFileDialog1 = New Forms.OpenFileDialog
        SaveFileDialog1 = New Forms.SaveFileDialog

        menuFileOpenNoDetect.Visibility = Visibility.Collapsed

        If Settings.GetSettings.Setting("SimpleMode").ToLower = "true" Then
            menuFileSaveProject.Visibility = Visibility.Collapsed
            menuNewProject.Visibility = Visibility.Collapsed
            menuBuild.Visibility = Visibility.Collapsed
        Else
            _projectExplorer = New ProjectExplorer(_manager)
            toolbarPaneRight.Children.Add(_projectExplorer.ParentAnchorable)
        End If

        _manager.RegisterIOFilter("*.skyproj", "Sky Editor Project File")

        AddHandler _manager.CurrentProject.FileAdded, AddressOf FileOpened

        AddHandler PluginHelper.LoadingMessageChanged, AddressOf OnLoadingMessageChanged
        AddHandler PluginHelper.ConsoleLineWritten, AddressOf OnConsoleLineWritten

    End Sub

    Private Sub MainWindow2_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Dim editedTabs = From t In docPane.Children Where TypeOf t Is DocumentTab AndAlso DirectCast(t, DocumentTab).IsModified = True

        Dim editsMade As Boolean = editedTabs.Any OrElse _manager.CurrentProject.IsModified
        If editsMade Then
            If MessageBox.Show("Are you sure you want to exit Sky Editor?  Any unsaved changes will be lost.", "Sky Editor", MessageBoxButton.YesNo) = MessageBoxResult.No Then
                e.Cancel = True
            End If
        End If
    End Sub
#End Region


    Private Sub _manager_MenuItemRegistered(sender As Object, Item As MenuItem) Handles _manager.MenuItemRegistered
        menuMain.Items.Add(Item)
    End Sub

    Private Sub _projectExplorer_FileOpen(sender As Object, ProjectFile As String) Handles _projectExplorer.FileOpen
        If Not IsFileTabOpen(_manager.CurrentProject.Files(ProjectFile)) Then
            If _manager.CurrentProject.Files(ProjectFile) IsNot Nothing Then
                docPane.Children.Add(New DocumentTab(_manager.CurrentProject.Files(ProjectFile), _manager))
            End If
        End If
    End Sub

    Private Sub OnLoadingMessageChanged(sender As Object, e As PluginHelper.LoadingMessageChangedEventArgs)
        Dispatcher.Invoke(New Action(Sub()
                                         lblStatus.Content = e.NewMessage
                                         progressBar.IsIndeterminate = e.IsIndeterminate
                                         progressBar.Value = e.Progress ' * 100
                                     End Sub))
    End Sub
    Private Sub OnConsoleLineWritten(sender As Object, e As PluginHelper.ConsoleLineWrittenEventArgs)
        If Not e.Type = PluginHelper.LineType.ConsoleOutput Then
            '_queuedConsoleLines.Enqueue(e)
            Dispatcher.InvokeAsync(New Action(Sub()
                                                  txtOutput.AppendText(e.Line)
                                                  txtOutput.AppendText(vbCrLf)
                                                  txtOutput.ScrollToEnd()
                                              End Sub))
        End If
    End Sub
#End Region

End Class
