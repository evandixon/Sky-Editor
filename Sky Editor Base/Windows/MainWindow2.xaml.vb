Public Class MainWindow2
    Private WithEvents _manager As PluginManager
    Private WithEvents _projectExplorer As ProjectExplorer
    Private WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Private WithEvents SaveFileDialog1 As System.Windows.Forms.SaveFileDialog

    Private Sub MainWindow2_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        _manager = PluginManager.GetInstance
        _projectExplorer = New ProjectExplorer(_manager)

        OpenFileDialog1 = New Forms.OpenFileDialog
        SaveFileDialog1 = New Forms.SaveFileDialog

        toolbarPaneRight.Children.Add(_projectExplorer.ParentAnchorable)

        AddHandler _manager.CurrentProject.FileAdded, AddressOf FileOpened
    End Sub

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
            Dim file = DirectCast(docPane.SelectedContent, DocumentTab).File
            SaveFileDialog1.Filter = _manager.IOFiltersStringSaveAs(IO.Path.GetExtension(file.OriginalFilename))
            If SaveFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                file.Save(SaveFileDialog1.FileName)
            End If
        End If
    End Sub
    Private Sub SaveProject()
        _manager.CurrentProject.SaveProject()
    End Sub
    Private Sub menuFileSaveProject_Click(sender As Object, e As RoutedEventArgs) Handles menuFileSaveProject.Click
        SaveProject()
    End Sub

    Private Sub FileOpened(sender As Object, File As KeyValuePair(Of String, GenericFile))
        docPane.Children.Add(New DocumentTab(File.Value, _manager))
    End Sub

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

    Private Sub menuNewProject_Click(sender As Object, e As RoutedEventArgs) Handles menuNewProject.Click
        Dim newProj As New NewProjectWindow(_manager)
        If newProj.ShowDialog() Then
            _manager.CurrentProject = Project.CreateProject(newProj.SelectedName, newProj.SelectedLocation, newProj.SelectedProjectType, _manager)
        End If
    End Sub

    Private Sub menuBuild_Click(sender As Object, e As RoutedEventArgs) Handles menuBuild.Click
        _manager.CurrentProject.Build()
    End Sub
End Class
