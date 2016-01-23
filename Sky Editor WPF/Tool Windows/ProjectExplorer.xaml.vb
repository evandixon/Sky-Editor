Imports System.Threading.Tasks
Imports SkyEditorBase.Interfaces

Public Class ProjectExplorer
    Inherits ToolWindow
    WithEvents _manager As PluginManager
    Private WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Public Event FileOpen(sender As Object, e As EventArguments.FileOpenedEventArguments)
    Private Sub ProjectExplorer_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.Title = PluginHelper.GetLanguageItem("Files")
        menuAddFile.Header = PluginHelper.GetLanguageItem("Add File")
        menuAddFolder.Header = PluginHelper.GetLanguageItem("Add Folder")
        menuOpenInExplorer.Header = PluginHelper.GetLanguageItem("Open in File Explorer")
        btnAddFile.Content = PluginHelper.GetLanguageItem("Add File")
        Refresh()
    End Sub
    Sub Refresh()
        'Dispatcher.Invoke(New Action(Sub()
        '                                 If _manager.CurrentProject Is Nothing Then
        '                                     Me.Visibility = Visibility.Collapsed
        '                                 Else
        '                                     Me.Visibility = Visibility.Visible
        '                                     tvFiles.Items.Clear()
        '                                     tvFiles.Items.Add(GetDirectoryNode(""))
        '                                 End If
        '                             End Sub))
    End Sub
    Private Function GetDirectoryNode(Directory As String) As TreeViewItem
        Dim Node As New TreeViewItem()
        Node.Header = GetNodeContents(IO.Path.GetFileName(Directory.Trim("/")), SkyEditorBase.PluginHelper.GetLanguageItem("DirPrefix", "[DIR] "))
        Node.Tag = Directory
        'For Each item In (From d In _manager.CurrentProject.GetDirectories(Directory, False) Order By d Ascending Select d)
        '    If Not String.IsNullOrEmpty(item) Then Node.Items.Add(GetDirectoryNode(item))
        'Next
        'For Each item In (From f In _manager.CurrentProject.GetFiles(Directory) Order By f Ascending Select f)
        '    Node.Items.Add(GetFileNode(item))
        'Next
        Return Node
    End Function
    Private Function GetFileNode(Filename As String) As TreeViewItem
        Dim Node As New TreeViewItem()
        Node.Header = GetNodeContents(IO.Path.GetFileName(Filename), SkyEditorBase.PluginHelper.GetLanguageItem("FilePrefix", "[FILE] "))
        Node.Tag = Filename
        Return Node
    End Function
    Private Function GetNodeContents(Name As String, Image As String) As StackPanel
        Dim p As New StackPanel
        Dim l As New Label()
        Dim img As New Label()
        l.Content = Name
        img.Content = Image

        p.Orientation = Orientation.Horizontal
        p.Children.Add(img)
        p.Children.Add(l)
        Return p
    End Function
    Private Function SelectedPath() As String
        Dim item = DirectCast(tvFiles.SelectedItem, TreeViewItem)
        If item IsNot Nothing Then
            Return item.Tag
        Else
            Return ""
        End If
    End Function
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        OpenFileDialog1 = New Forms.OpenFileDialog
    End Sub
    Public Sub New(Manager As PluginManager)
        Me.New()
        _manager = Manager
    End Sub

    Private Sub _manager_ProjectChanged(sender As Object, e As EventArguments.ProjectChangedEventArgs) Handles _manager.ProjectChanged
        Refresh()
    End Sub

    Private Sub _manager_ProjectFileAdded(sender As Object, e As EventArguments.FileAddedEventArguments) Handles _manager.ProjectFileAdded
        Refresh()
    End Sub

    Private Sub _manager_ProjectFileRemoved(sender As Object, e As EventArguments.FileRemovedEventArgs) Handles _manager.ProjectFileRemoved
        Refresh()
    End Sub

    Private Sub _manager_ProjectDirectoryCreated(sender As Object, e As EventArguments.ProjectDirectoryCreatedEventArgs) Handles _manager.ProjectDirectoryCreated
        Refresh()
    End Sub

    Private Sub _manager_ProjectModified(sender As Object, e As EventArgs) Handles _manager.ProjectModified
        Refresh()
    End Sub

    Private Sub tvFiles_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles tvFiles.MouseDoubleClick
        'If tvFiles.SelectedItem IsNot Nothing AndAlso _manager.CurrentProject.Files.ContainsKey(tvFiles.SelectedItem.Tag) Then
        '    Dim file = _manager.CurrentProject.Files(tvFiles.SelectedItem.Tag)
        '    If TypeOf file Is ExecutableFile Then
        '        Dim restartIfOpen As Boolean = (Not DirectCast(file, ExecutableFile).Started) OrElse (MessageBox.Show(PluginHelper.GetLanguageItem("This program is alredy open.  Would you like to stop the current instance?  Any unsaved data may be lost."), "Sky Editor", MessageBoxButton.YesNo) = MessageBoxResult.Yes)
        '        DirectCast(file, ExecutableFile).Start(restartIfOpen)
        '    Else
        '        RaiseEvent FileOpen(Me, New EventArguments.FileOpenedEventArguments() With {.ProjectPath = tvFiles.SelectedItem.Tag, .File = file})
        '    End If
        'End If
    End Sub

    Private Sub btnAddFile_Click(sender As Object, e As RoutedEventArgs) Handles btnAddFile.Click
        'OpenFileDialog1.Filter = _manager.IOFiltersString
        'If OpenFileDialog1.ShowDialog = System.Windows.Forms.DialogResult.OK Then
        '    _manager.CurrentProject.OpenFile(OpenFileDialog1.FileName, IO.Path.GetFileName(OpenFileDialog1.FileName))
        'End If
    End Sub

    'Private Sub btnAddFile_Copy_Click(sender As Object, e As RoutedEventArgs) Handles btnAddFile_Copy.Click
    '    _manager.CurrentProject.AddFile("Mods/TestMod.ndsmodsrc", New GenericFile({}))
    'End Sub

    Private Sub tvFiles_SelectedItemChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Object)) Handles tvFiles.SelectedItemChanged
        'If _manager.CurrentProject.CanCreateDirectory(SelectedPath) Then
        '    menuAddFolder.Visibility = Visibility.Visible
        'Else
        '    menuAddFolder.Visibility = Visibility.Collapsed
        'End If

        'If _manager.CurrentProject.CreatableFiles(SelectedPath, _manager) IsNot Nothing AndAlso _manager.CurrentProject.CreatableFiles(SelectedPath, _manager).Count > 0 Then
        '    menuAddFile.Visibility = Visibility.Visible
        'Else
        '    menuAddFolder.Visibility = Visibility.Collapsed
        'End If
    End Sub

    Private Sub menuAddFolder_Click(sender As Object, e As RoutedEventArgs) Handles menuAddFolder.Click
        'If _manager.CurrentProject.CanCreateDirectory(SelectedPath) Then
        '    Dim w As New SkyEditorWindows.NewNameWindow("What should the folder be named?", "New Folder")
        '    If w.ShowDialog Then
        '        _manager.CurrentProject.CreateDirectory(IO.Path.Combine(SelectedPath, w.SelectedName))
        '    End If
        'End If
    End Sub

    Private Async Sub menuAddFile_Click(sender As Object, e As RoutedEventArgs) Handles menuAddFile.Click
        'Dim creatableFiles = _manager.CurrentProject.CreatableFiles(SelectedPath, _manager)
        'If creatableFiles IsNot Nothing AndAlso creatableFiles.Count > 0 Then
        '    Dim w As New SkyEditorWindows.NewFileWindow()
        '    Dim games As New Dictionary(Of String, Type)
        '    For Each item In creatableFiles
        '        games.Add(PluginHelper.GetLanguageItem(item.Name), item)
        '    Next
        '    w.AddGames(games.Keys)
        '    If w.ShowDialog Then
        '        Dim file As iSavable = games(w.SelectedGame).GetConstructor({}).Invoke({})
        '        If TypeOf file Is iCreatableFile Then
        '            DirectCast(file, iCreatableFile).CreateFile(w.SelectedName)
        '        End If
        '        Dim path = SelectedPath()
        '        Dim name = w.SelectedName
        '        Await Task.Run(New Action(Sub()
        '                                      file.Save(IO.Path.Combine(IO.Path.GetDirectoryName(_manager.CurrentProject.Filename), path, name & file.DefaultExtension.Trim("*")))
        '                                      If TypeOf file Is iOnDisk Then
        '                                          DirectCast(file, iOnDisk).Filename = IO.Path.Combine(IO.Path.GetDirectoryName(_manager.CurrentProject.Filename), path, name & file.DefaultExtension.Trim("*"))
        '                                      End If
        '                                      _manager.CurrentProject.AddFile(IO.Path.Combine(path, name & file.DefaultExtension.Trim("*")), file)
        '                                  End Sub))
        '    End If
        'End If
    End Sub
End Class
