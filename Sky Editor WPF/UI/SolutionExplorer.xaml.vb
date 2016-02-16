Imports SkyEditorBase.Interfaces
Imports SkyEditorWPF
Namespace UI
    Public Class SolutionExplorer
        Implements ITargetedControl

        Private Class NodeTag
            Public Property IsRoot As Boolean
            Public Property IsProjectRoot As Boolean
            Public Property IsDirectory As Boolean
            Public Property ParentProject As Project
            Public Property ParentSolution As Solution
            Public Property ParentPath As String
            Public Property Name As String
            Public Sub New()
                IsRoot = False
                IsProjectRoot = False
            End Sub
        End Class

        Private Class TreeViewComparer
            Implements IComparer(Of TreeViewItem)

            Public Function Compare(x As TreeViewItem, y As TreeViewItem) As Integer Implements IComparer(Of TreeViewItem).Compare
                Dim t1 = DirectCast(x.Tag, NodeTag)
                Dim t2 = DirectCast(y.Tag, NodeTag)
                If t1.IsDirectory = t2.IsDirectory Then
                    Return t1.Name.CompareTo(t2.Name)
                Else
                    Return t2.IsDirectory.CompareTo(t1.IsDirectory)
                End If
            End Function
        End Class

        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            ProjectRegistry = New Dictionary(Of Project, TreeViewItem)
        End Sub

        Private Sub SolutionExplorer_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("Solution Explorer")
            menuAddFolder.Visibility = Visibility.Collapsed
            menuCreateProject.Visibility = Visibility.Collapsed
            menuCreateFile.Visibility = Visibility.Collapsed
            menuDelete.Visibility = Visibility.Collapsed
            menuContext.Visibility = Visibility.Collapsed
            menuOpen.Visibility = Visibility.Collapsed
            menuProperties.Visibility = Visibility.Collapsed
            menuAddExistingFile.Visibility = Visibility.Collapsed

            menuAddFolder.Header = PluginHelper.GetLanguageItem("Add Folder")
            menuCreateProject.Header = PluginHelper.GetLanguageItem("Create Project")
            menuCreateFile.Header = PluginHelper.GetLanguageItem("Create File")
            menuDelete.Header = PluginHelper.GetLanguageItem("Delete")
            menuOpen.Header = PluginHelper.GetLanguageItem("Open")
            menuProperties.Header = PluginHelper.GetLanguageItem("Properties")
            menuAddExistingFile.Header = PluginHelper.GetLanguageItem("Add Existing File")
        End Sub

        Public Property Header As String Implements ITargetedControl.Header
            Get
                Return _header
            End Get
            Set(value As String)
                Dim old = _header
                _header = value
                RaiseEvent HeaderChanged(Me, New EventArguments.HeaderUpdatedEventArgs With {.NewValue = value, .OldValue = old})
            End Set
        End Property
        Dim _header As String

        Private Property ITargetedControl_IsVisible As Boolean Implements ITargetedControl.IsVisible
            Get
                Return _isVisible
            End Get
            Set(value As Boolean)
                _isVisible = value
                RaiseEvent VisibilityChanged(Me, New EventArguments.VisibilityUpdatedEventArgs With {.IsVisible = value})
            End Set
        End Property
        Dim _isVisible As Boolean

        Private Property ProjectRegistry As Dictionary(Of Project, TreeViewItem)

        Public Event HeaderChanged As ITargetedControl.HeaderChangedEventHandler Implements ITargetedControl.HeaderChanged
        Public Event VisibilityChanged As ITargetedControl.VisibilityChangedEventHandler Implements ITargetedControl.VisibilityChanged

        Public Sub UpdateTargets(Targets As IEnumerable(Of Object)) Implements ITargetedControl.UpdateTargets
            Dim supported As Integer = 0

            For Each item As TreeViewItem In tvSolution.Items
                Dim tag = DirectCast(item.Tag, NodeTag)
                RemoveHandler tag.ParentSolution.DirectoryCreated, AddressOf Solution_DirectoryCreated
                RemoveHandler tag.ParentSolution.DirectoryDeleted, AddressOf Solution_DirectoryDeleted
                RemoveHandler tag.ParentSolution.ProjectAdded, AddressOf Solution_ProjectAdded
                RemoveHandler tag.ParentSolution.ProjectRemoving, AddressOf Solution_ProjectRemoving
                RemoveHandler tag.ParentSolution.ProjectRemoved, AddressOf Solution_ProjectRemoved
            Next

            'Todo: somehow remove handlers for child projects
            If ProjectRegistry IsNot Nothing Then
                For Each item In ProjectRegistry.Keys
                    RemoveHandler item.DirectoryCreated, AddressOf Project_DirectoryCreated
                    RemoveHandler item.DirectoryDeleted, AddressOf Project_DirectoryDeleted
                    RemoveHandler item.FileAdded, AddressOf Project_FileAdded
                    RemoveHandler item.FileRemoved, AddressOf Project_FileRemoved
                Next
                ProjectRegistry.Clear()
            End If

            tvSolution.Items.Clear()

            For Each item In Targets
                If TypeOf item Is Solution Then
                    Dim sol = DirectCast(item, Solution)

                    Dim n As New TreeViewItem
                    Dim t As New NodeTag
                    t.ParentPath = ""
                    t.ParentSolution = sol
                    t.IsDirectory = False
                    t.Name = sol.Name
                    t.IsRoot = True
                    n.Header = "[Solution] " & sol.Name
                    n.Tag = t
                    n.ExpandSubtree()

                    For Each solItem In sol.GetDirectoryContents("")
                        n.Items.Add(GetNode(sol, solItem, ""))
                    Next

                    tvSolution.Items.Add(n)

                    AddHandler sol.DirectoryCreated, AddressOf Solution_DirectoryCreated
                    AddHandler sol.DirectoryDeleted, AddressOf Solution_DirectoryDeleted
                    AddHandler sol.ProjectAdded, AddressOf Solution_ProjectAdded
                    AddHandler sol.ProjectRemoving, AddressOf Solution_ProjectRemoving
                    AddHandler sol.ProjectRemoved, AddressOf Solution_ProjectRemoved

                    supported += 1
                End If
            Next

            ITargetedControl_IsVisible = (supported > 0)
        End Sub

        Private Function GetNode(Solution As Solution, Item As Solution.SolutionItem, Path As String) As TreeViewItem
            Dim n As New TreeViewItem
            If Item.IsDirectory Then
                n.Header = "[Dir] " & Item.Name
                Item.Children.Sort()

                For Each child In Item.Children
                    n.Items.Add(GetNode(Solution, child, Path & "/" & Item.Name))
                Next
                Dim t As New NodeTag
                t.IsDirectory = True
                t.ParentProject = Nothing
                t.ParentSolution = Solution
                t.ParentPath = Path
                t.Name = Item.Name
                n.Tag = t
            Else
                n.Header = "[Project] " & Item.Name
                For Each projectItem In Item.Project.GetDirectoryContents("")
                    n.Items.Add(GetNode(Solution, Item.Project, projectItem, ""))
                Next
                Dim t As New NodeTag
                t.IsDirectory = False
                t.ParentProject = Item.Project
                t.IsProjectRoot = True
                t.ParentSolution = Solution
                t.ParentPath = Path
                t.Name = Item.Name
                n.Tag = t

                If Not ProjectRegistry.ContainsKey(Item.Project) Then
                    ProjectRegistry.Add(Item.Project, n)
                Else
                    PluginHelper.Writeline("Almost added duplicate reference of a project to the dictionary.  Something may be corrupt.", PluginHelper.LineType.Error)
                End If

                AddHandler Item.Project.DirectoryCreated, AddressOf Project_DirectoryCreated
                AddHandler Item.Project.DirectoryDeleted, AddressOf Project_DirectoryDeleted
                AddHandler Item.Project.FileAdded, AddressOf Project_FileAdded
                AddHandler Item.Project.FileRemoved, AddressOf Project_FileRemoved
            End If
            Return n
        End Function

        Private Function GetNode(Solution As Solution, Project As Project, Item As Project.ProjectItem, Path As String) As TreeViewItem
            Dim n As New TreeViewItem
            If Item.IsDirectory Then
                n.Header = "[Dir] " & Item.Name
                Item.Children.Sort()
                For Each child In Item.Children
                    n.Items.Add(GetNode(Solution, Project, child, Path & "/" & Item.Name))
                Next
                Dim t As New NodeTag
                t.IsDirectory = True
                t.ParentProject = Project
                t.ParentSolution = Solution
                t.ParentPath = Path
                t.Name = Item.Name
                n.Tag = t
            Else
                n.Header = "[File] " & Item.Name
                Dim t As New NodeTag
                t.IsDirectory = False
                t.ParentProject = Project
                t.ParentSolution = Solution
                t.ParentPath = Path
                t.Name = Item.Name
                n.Tag = t
            End If
            Return n
        End Function

        Public Function GetDefaultPane() As ITargetedControl.Pane Implements ITargetedControl.GetDefaultPane
            Return ITargetedControl.Pane.Right
        End Function

        Public Function StartCollapsed() As Boolean Implements ITargetedControl.StartCollapsed
            Return False
        End Function

        Private Sub tvSolution_SelectedItemChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Object)) Handles tvSolution.SelectedItemChanged
            'To simplify logic, we don't want any actions visible unless they're supported by the currently selected item
            menuOpen.Visibility = Visibility.Collapsed
            menuAddFolder.Visibility = Visibility.Collapsed
            menuCreateProject.Visibility = Visibility.Collapsed
            menuCreateFile.Visibility = Visibility.Collapsed
            menuAddExistingFile.Visibility = Visibility.Collapsed
            menuDelete.Visibility = Visibility.Collapsed
            menuProperties.Visibility = Visibility.Collapsed

            'Detect supported actions
            If e.NewValue IsNot Nothing AndAlso TypeOf e.NewValue Is TreeViewItem Then
                Dim node = DirectCast(e.NewValue, TreeViewItem)
                If TypeOf node.Tag Is NodeTag Then
                    Dim tag = DirectCast(node.Tag, NodeTag)
                    If tag.ParentProject Is Nothing AndAlso tag.ParentSolution IsNot Nothing Then
                        'Then we're at the solution level

                        If tag.IsDirectory AndAlso tag.ParentSolution.CanCreateDirectory(tag.ParentPath) Then
                            menuAddFolder.Visibility = Visibility.Visible
                        End If

                        If Not tag.IsProjectRoot AndAlso tag.ParentSolution.CanCreateProject(tag.ParentPath) Then
                            menuCreateProject.Visibility = Visibility.Visible
                        End If

                        If tag.ParentSolution.CanDeleteDirectory(tag.ParentPath & "/" & tag.Name) Then
                            menuDelete.Visibility = Visibility.Visible
                        End If

                        If tag.IsRoot Then
                            'Then we've selected the project
                            menuProperties.Visibility = Visibility.Visible
                        End If

                    ElseIf tag.ParentProject IsNot Nothing AndAlso tag.IsProjectRoot Then
                        'Then we're at the root of the project
                        If tag.ParentSolution.CanDeleteProject(tag.ParentPath & "/" & tag.Name) Then
                            menuDelete.Visibility = Visibility.Visible
                        End If

                        If tag.ParentProject.CanCreateFile(tag.ParentPath) Then
                            menuCreateFile.Visibility = Visibility.Visible
                        End If

                        If tag.ParentProject.CanCreateFile(tag.ParentPath) Then
                            menuAddExistingFile.Visibility = Visibility.Visible
                        End If

                        If tag.ParentProject.CanAddExistingFile(tag.ParentPath) Then
                            menuAddExistingFile.Visibility = Visibility.Visible
                        End If

                        menuProperties.Visibility = Visibility.Visible

                        'Update the current project
                        PluginManager.GetInstance.CurrentProject = tag.ParentProject

                    ElseIf tag.ParentProject IsNot Nothing AndAlso Not tag.IsProjectRoot Then
                        'Then we're at the project level

                        If Not tag.IsDirectory Then
                            menuOpen.Visibility = Visibility.Visible
                        End If

                        If tag.IsDirectory AndAlso tag.ParentProject.CanCreateDirectory(tag.ParentPath) Then
                            menuAddFolder.Visibility = Visibility.Visible
                        End If

                        If tag.IsDirectory AndAlso tag.ParentProject.CanCreateFile(tag.ParentPath) Then
                            menuCreateFile.Visibility = Visibility.Visible
                        End If

                        If tag.IsDirectory AndAlso tag.ParentProject.CanAddExistingFile(tag.ParentPath) Then
                            menuAddExistingFile.Visibility = Visibility.Visible
                        End If

                        If tag.ParentProject.CanDeleteDirectory(tag.ParentPath & "/" & tag.Name) Then
                            menuDelete.Visibility = Visibility.Visible
                        End If

                        'Update the current project
                        PluginManager.GetInstance.CurrentProject = tag.ParentProject

                    Else
                        'Then we're somewhere else?
                    End If
                End If
            End If

            menuContext.Visibility = menuAddFolder.Visibility * menuCreateFile.Visibility * menuCreateProject.Visibility * menuDelete.Visibility * menuOpen.Visibility
        End Sub

        Private Sub menuAddFolder_Click(sender As Object, e As RoutedEventArgs) Handles menuAddFolder.Click
            If tvSolution.SelectedItem IsNot Nothing Then
                Dim node = DirectCast(tvSolution.SelectedItem, TreeViewItem)
                If TypeOf node.Tag Is NodeTag Then
                    Dim tag = DirectCast(node.Tag, NodeTag)
                    Dim w As New UI.NewNameWindow(PluginHelper.GetLanguageItem("What should the folder be named?"), PluginHelper.GetLanguageItem("New Folder"))
                    If w.ShowDialog Then

                        If tag.ParentProject Is Nothing AndAlso tag.ParentSolution IsNot Nothing Then
                            'Then we're at the solution level
                            If tag.IsRoot Then
                                tag.ParentSolution.CreateDirectory("", w.SelectedName)
                            Else
                                tag.ParentSolution.CreateDirectory(tag.ParentPath & "/" & tag.Name, w.SelectedName)
                            End If
                        ElseIf tag.ParentProject IsNot Nothing Then
                            'Then we're at the project level
                            If tag.IsProjectRoot Then
                                tag.ParentProject.CreateDirectory("", w.SelectedName)
                            Else
                                tag.ParentProject.CreateDirectory(tag.ParentPath & "/" & tag.Name, w.SelectedName)
                            End If
                        Else
                            'Then we're somewhere else?
                        End If
                    End If

                End If
            End If

        End Sub

        Private Sub menuCreateProject_Click(sender As Object, e As RoutedEventArgs) Handles menuCreateProject.Click
            If tvSolution.SelectedItem IsNot Nothing Then
                Dim node = DirectCast(tvSolution.SelectedItem, TreeViewItem)
                If TypeOf node.Tag Is NodeTag Then
                    Dim tag = DirectCast(node.Tag, NodeTag)
                    Dim w As New UI.NewFileWindow
                    Dim types As New Dictionary(Of String, Type)
                    For Each item In tag.ParentSolution.GetSupportedProjectTypes(tag.ParentPath)
                        types.Add(PluginHelper.GetLanguageItem(item.Name), item)
                    Next
                    w.AddGames(types.Keys)
                    If w.ShowDialog Then

                        If tag.ParentProject Is Nothing AndAlso tag.ParentSolution IsNot Nothing Then
                            'Then we're at the solution level
                            If tag.IsRoot Then
                                tag.ParentSolution.CreateProject("", w.SelectedName, types(w.SelectedGame))
                            Else
                                tag.ParentSolution.CreateProject(tag.ParentPath & "/" & tag.Name, w.SelectedName, types(w.SelectedGame))
                            End If

                        Else
                            'Then we're somewhere else?
                        End If
                    End If

                End If
            End If
        End Sub

        Private Sub menuCreateFile_Click(sender As Object, e As RoutedEventArgs) Handles menuCreateFile.Click
            If tvSolution.SelectedItem IsNot Nothing Then
                Dim node = DirectCast(tvSolution.SelectedItem, TreeViewItem)
                If TypeOf node.Tag Is NodeTag Then
                    Dim tag = DirectCast(node.Tag, NodeTag)
                    Dim w As New UI.NewFileWindow
                    Dim types As New Dictionary(Of String, Type)
                    For Each item In tag.ParentProject.GetSupportedFileTypes(tag.ParentPath)
                        types.Add(PluginHelper.GetLanguageItem(item.Name), item)
                    Next
                    w.AddGames(types.Keys)
                    If w.ShowDialog Then

                        If tag.ParentProject IsNot Nothing Then
                            'Then we're at the project level
                            If tag.IsProjectRoot Then
                                tag.ParentProject.CreateFile("", w.SelectedName, types(w.SelectedGame))
                            Else
                                tag.ParentProject.CreateFile(tag.ParentPath & "/" & tag.Name, w.SelectedName, types(w.SelectedGame))
                            End If

                        Else
                            'Then we're somewhere else?
                        End If
                    End If

                End If
            End If
        End Sub

        Private Sub menuAddExistingFile_Click(sender As Object, e As RoutedEventArgs) Handles menuAddExistingFile.Click
            If tvSolution.SelectedItem IsNot Nothing Then
                Dim node = DirectCast(tvSolution.SelectedItem, TreeViewItem)
                If TypeOf node.Tag Is NodeTag Then
                    Dim t = DirectCast(node.Tag, NodeTag)
                    Dim w As New Forms.OpenFileDialog
                    w.Filter = t.ParentProject.GetImportIOFilter(t.ParentPath)
                    If w.ShowDialog = Forms.DialogResult.OK Then
                        If t.ParentProject IsNot Nothing Then
                            'Then we're at the project level
                            t.ParentProject.AddExistingFile(t.ParentPath, w.FileName)
                        Else
                            'Then we're somewhere else?
                        End If
                    End If

                End If
            End If
        End Sub

        Private Sub menuDelete_Click(sender As Object, e As RoutedEventArgs) Handles menuDelete.Click
            If tvSolution.SelectedItem IsNot Nothing Then
                Dim node = DirectCast(tvSolution.SelectedItem, TreeViewItem)
                If TypeOf node.Tag Is NodeTag Then
                    Dim tag = DirectCast(node.Tag, NodeTag)
                    If MessageBox.Show(PluginHelper.GetLanguageItem("Are you sure you want to delete this?"), PluginHelper.GetLanguageItem("Sky Editor"), MessageBoxButton.YesNo) = MessageBoxResult.Yes Then
                        If tag.ParentProject Is Nothing AndAlso tag.ParentSolution IsNot Nothing Then
                            'Then we're at the solution level
                            tag.ParentSolution.DeleteDirectory(tag.ParentPath & "/" & tag.Name)
                        ElseIf tag.ParentProject IsNot Nothing AndAlso tag.IsProjectRoot Then
                            'Then we're at the root of the project
                            tag.ParentSolution.DeleteProject(tag.ParentPath & "/" & tag.Name)
                        ElseIf tag.ParentProject IsNot Nothing AndAlso Not tag.IsProjectRoot Then
                            'Then we're at the project level
                            tag.ParentProject.DeleteFile(tag.ParentPath & "/" & tag.Name)
                        Else
                            'Then we're somewhere else?
                        End If
                    End If
                End If
            End If
        End Sub

        Private Sub Solution_DirectoryCreated(sender As Object, e As EventArguments.DirectoryCreatedEventArgs)
            Dim parent = GetSolutionNode(sender, e.ParentPath)
            If parent IsNot Nothing Then
                Dim t As New TreeViewItem
                t.Header = "[Dir] " & e.DirectoryName
                Dim n As New NodeTag
                n.Name = e.DirectoryName
                n.IsDirectory = True
                n.ParentSolution = sender
                n.ParentPath = e.ParentPath
                t.Tag = n
                'Sort the items when we add them
                Dim items As New List(Of TreeViewItem)
                For Each item In parent.Items
                    items.Add(item)
                Next
                items.Add(t)
                items.Sort(New TreeViewComparer)

                parent.Items.Clear()
                For Each item In items
                    parent.Items.Add(item)
                Next
            End If
        End Sub

        Private Sub Solution_ProjectAdded(sender As Object, e As EventArguments.ProjectAddedEventArgs)
            Dim parent = GetSolutionNode(sender, e.ParentPath)
            If parent IsNot Nothing Then
                Dim t As New TreeViewItem
                t.Header = "[Project] " & e.Project.Name
                Dim n As New NodeTag
                n.Name = e.Project.Name
                n.IsRoot = False
                n.IsProjectRoot = True
                n.IsDirectory = False
                n.ParentSolution = sender
                n.ParentPath = e.ParentPath
                n.ParentProject = e.Project
                t.Tag = n
                'Sort the items when we add them
                Dim items As New List(Of TreeViewItem)
                For Each item In parent.Items
                    items.Add(item)
                Next
                items.Add(t)
                items.Sort(New TreeViewComparer)

                parent.Items.Clear()
                For Each item In items
                    parent.Items.Add(item)
                Next

                ProjectRegistry.Add(e.Project, t)

                AddHandler e.Project.DirectoryCreated, AddressOf Project_DirectoryCreated
                AddHandler e.Project.DirectoryDeleted, AddressOf Project_DirectoryDeleted
                AddHandler e.Project.FileAdded, AddressOf Project_FileAdded
                AddHandler e.Project.FileRemoved, AddressOf Project_FileRemoved
            End If
        End Sub

        Private Sub Solution_DirectoryDeleted(sender As Object, e As EventArguments.DirectoryDeletedEventArgs)
            Dim parent = GetSolutionNode(sender, e.ParentPath)
            If parent IsNot Nothing Then
                Dim child = (From c As TreeViewItem In parent.Items Where DirectCast(c.Tag, NodeTag).Name.ToLower = e.DirectoryName.ToLower).FirstOrDefault
                If child IsNot Nothing Then
                    parent.Items.Remove(child)
                End If
            End If
        End Sub

        Private Sub Solution_ProjectRemoving(sender As Object, e As EventArguments.ProjectRemovingEventArgs)
            RemoveHandler e.Project.DirectoryCreated, AddressOf Project_DirectoryCreated
            RemoveHandler e.Project.DirectoryDeleted, AddressOf Project_DirectoryDeleted
            RemoveHandler e.Project.FileAdded, AddressOf Project_FileAdded
            RemoveHandler e.Project.FileRemoved, AddressOf Project_FileRemoved
            ProjectRegistry.Remove(e.Project)
        End Sub

        Private Sub Solution_ProjectRemoved(sender As Object, e As EventArguments.ProjectRemovedEventArgs)
            Dim parent = GetSolutionNode(sender, e.ParentPath)
            If parent IsNot Nothing Then
                Dim child = (From c As TreeViewItem In parent.Items Where DirectCast(c.Tag, NodeTag).Name.ToLower = e.DirectoryName.ToLower).FirstOrDefault
                If child IsNot Nothing Then
                    parent.Items.Remove(child)
                End If
            End If
        End Sub

        Private Function GetSolutionNode(Solution As Solution, Path As String) As TreeViewItem
            Dim node As TreeViewItem = Nothing

            For Each item As TreeViewItem In tvSolution.Items
                Dim itemTag = DirectCast(item.Tag, NodeTag)
                If itemTag.ParentSolution Is Solution Then
                    node = item
                    Exit For
                End If
            Next

            If node IsNot Nothing Then
                Dim pathArr = Path.Replace("\", "/").TrimStart("/").Split("/")
                For count = 0 To pathArr.Length - 1
                    Dim i = count
                    Dim q = (From c In node.Items Where DirectCast(c.Tag, NodeTag).Name.ToLower = pathArr(i).ToLower).FirstOrDefault
                    If q IsNot Nothing Then
                        node = q
                    Else
                        'In this case, there is not a node at the given path.
                        Exit For
                    End If
                Next
            End If

            Return node
        End Function

        Private Function GetProjectRootNode(Project As Project) As TreeViewItem
            Return ProjectRegistry(Project)
        End Function

        Private Function GetProjectNode(Project As Project, Path As String) As TreeViewItem
            Dim node As TreeViewItem = GetProjectRootNode(Project)

            If node IsNot Nothing AndAlso Not String.IsNullOrEmpty(Path) Then
                Dim pathArr = Path.Replace("\", "/").TrimStart("/").Split("/")
                For count = 0 To pathArr.Length - 1
                    Dim i = count
                    Dim q = (From c In node.Items Where DirectCast(c.Tag, NodeTag).Name.ToLower = pathArr(i).ToLower).FirstOrDefault
                    If q IsNot Nothing Then
                        node = q
                    Else
                        'In this case, there is not a node at the given path.
                        'So we'll add one

                        Dim t As New TreeViewItem
                        t.Header = "[Dir] " & pathArr(i)
                        Dim n As New NodeTag
                        n.Name = pathArr(i)
                        n.IsDirectory = True
                        n.ParentSolution = DirectCast(node.Tag, NodeTag).ParentSolution
                        n.ParentPath = pathArr(i)
                        n.ParentProject = Project
                        t.Tag = n
                        'Sort the items when we add them
                        Dim items As New List(Of TreeViewItem)
                        For Each item In node.Items
                            items.Add(item)
                        Next
                        items.Add(t)
                        items.Sort(New TreeViewComparer)

                        node.Items.Clear()
                        For Each item In items
                            node.Items.Add(item)
                        Next

                        node = t
                    End If
                Next
            End If

            Return node
        End Function

        Private Sub Project_DirectoryCreated(sender As Object, e As EventArguments.DirectoryCreatedEventArgs)
            Dispatcher.Invoke(Sub()
                                  Dim parent = GetProjectNode(sender, e.ParentPath)
                                  If parent IsNot Nothing Then
                                      Dim t As New TreeViewItem
                                      t.Header = "[Dir] " & e.DirectoryName
                                      Dim n As New NodeTag
                                      n.Name = e.DirectoryName
                                      n.IsDirectory = True
                                      n.ParentSolution = DirectCast(parent.Tag, NodeTag).ParentSolution
                                      n.ParentPath = e.ParentPath
                                      n.ParentProject = sender
                                      t.Tag = n
                                      'Sort the items when we add them
                                      Dim items As New List(Of TreeViewItem)
                                      For Each item In parent.Items
                                          items.Add(item)
                                      Next
                                      items.Add(t)
                                      items.Sort(New TreeViewComparer)

                                      parent.Items.Clear()
                                      For Each item In items
                                          parent.Items.Add(item)
                                      Next
                                  End If
                              End Sub)
        End Sub

        Private Sub Project_FileAdded(sender As Object, e As EventArguments.ProjectFileAddedEventArgs)
            Dispatcher.Invoke(Sub()
                                  Dim parent = GetProjectNode(sender, e.ParentPath)
                                  If parent IsNot Nothing Then
                                      Dim t As New TreeViewItem
                                      t.Header = "[File] " & e.Filename
                                      Dim n As New NodeTag
                                      n.Name = e.Filename
                                      n.IsDirectory = False
                                      n.ParentSolution = DirectCast(parent.Tag, NodeTag).ParentSolution
                                      n.ParentPath = e.ParentPath
                                      n.ParentProject = sender
                                      t.Tag = n

                                      'Sort the items when we add them
                                      Dim items As New List(Of TreeViewItem)
                                      For Each item In parent.Items
                                          items.Add(item)
                                      Next
                                      items.Add(t)
                                      items.Sort(New TreeViewComparer)

                                      parent.Items.Clear()
                                      For Each item In items
                                          parent.Items.Add(item)
                                      Next
                                  End If
                              End Sub)
        End Sub

        Private Sub Project_DirectoryDeleted(sender As Object, e As EventArguments.DirectoryDeletedEventArgs)
            Dim parent = GetProjectNode(sender, e.ParentPath)
            If parent IsNot Nothing Then
                Dim child = (From c As TreeViewItem In parent.Items Where DirectCast(c.Tag, NodeTag).Name.ToLower = e.DirectoryName.ToLower).FirstOrDefault
                If child IsNot Nothing Then
                    parent.Items.Remove(child)
                End If
            End If
        End Sub

        Private Sub Project_FileRemoved(sender As Object, e As EventArguments.ProjectFileRemovedEventArgs)
            Dim parent = GetProjectNode(sender, e.ParentPath)
            If parent IsNot Nothing Then
                Dim child = (From c As TreeViewItem In parent.Items Where DirectCast(c.Tag, NodeTag).Name.ToLower = e.FileName.ToLower).FirstOrDefault
                If child IsNot Nothing Then
                    parent.Items.Remove(child)
                    'Because parent.Items.Remove(child) doesn't work in some cases
                    'For count = parent.Items.Count - 1 To 0 Step -1
                    '    Dim item = parent.Items(count)
                    '    If DirectCast(item.Tag, NodeTag).Name = DirectCast(child.Tag, NodeTag).Name Then
                    '        parent.Items.Remove(item)
                    '    End If
                    'Next
                End If
            End If
        End Sub

        Private Sub tvSolution_MouseDoubleClick(sender As Object, e As EventArgs) Handles tvSolution.MouseDoubleClick, menuOpen.Click
            If tvSolution.SelectedItem IsNot Nothing Then
                Dim tag As NodeTag = DirectCast(tvSolution.SelectedItem, TreeViewItem).Tag
                If tag.ParentProject IsNot Nothing AndAlso Not tag.IsProjectRoot Then
                    Dim projItem = tag.ParentProject.GetProjectItemByPath(tag.ParentPath & "/" & tag.Name)
                    If projItem IsNot Nothing Then
                        Dim obj = projItem?.GetFile
                        If obj Is Nothing Then
                            Dim f = IO.Path.Combine(IO.Path.GetDirectoryName(tag.ParentProject.Filename), projItem.Filename)
                            If Not IO.File.Exists(f) Then
                                MessageBox.Show(String.Format(PluginHelper.GetLanguageItem("Unable to find file at ""{0}""."), f))
                            End If
                        End If
                        PluginHelper.RequestFileOpen(obj, tag.ParentProject)
                    End If
                End If
            End If
        End Sub

        Private Sub menuProperties_Click(sender As Object, e As RoutedEventArgs) Handles menuProperties.Click
            If tvSolution.SelectedItem IsNot Nothing Then
                Dim tag As NodeTag = DirectCast(tvSolution.SelectedItem, TreeViewItem).Tag
                If tag.IsRoot Then
                    PluginHelper.RequestFileOpen(tag.ParentSolution, False)
                ElseIf tag.IsProjectRoot Then
                    PluginHelper.RequestFileOpen(tag.ParentProject, tag.ParentProject)
                End If
            End If
        End Sub
    End Class

End Namespace
