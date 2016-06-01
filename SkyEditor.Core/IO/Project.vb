Imports System.Reflection
Imports SkyEditor.Core.Utilities

Namespace IO
    Public Class Project
        Implements INotifyPropertyChanged
        Implements INotifyModified
        Implements IDisposable
        Implements ISavable

        Public Sub New()
            RootNode = New ProjectNode(Me, Nothing)
            Settings = New SettingsProvider
        End Sub


#Region "Child Classes"
        Private Class SettingValue
            Public Property AssemblyQualifiedTypeName As String
            Public Property ValueJson As String
        End Class

        Private Class FileValue
            Public Property AssemblyQualifiedTypeName As String
            Public Property Filename As String
        End Class

        Private Class ProjectFile
            Public Const CurrentVersion As String = "v2"
            Public Property FileFormat As String
            Public Property AssemblyQualifiedTypeName As String
            Public Property Name As String
            ''' <summary>
            ''' Matches project paths to project files, which are relative to the project directory.
            ''' </summary>
            ''' <returns></returns>
            Public Property Files As Dictionary(Of String, FileValue)
            Public Property InternalSettings As String
            Public Sub New()
                Files = New Dictionary(Of String, FileValue)
            End Sub
        End Class

        Private Class ProjectFileLegacy
            Public Property AssemblyQualifiedTypeName As String
            Public Property Name As String
            ''' <summary>
            ''' Matches project paths to project files, which are relative to the project directory.
            ''' </summary>
            ''' <returns></returns>
            Public Property Files As Dictionary(Of String, FileValue)
            Public Property Settings As Dictionary(Of String, SettingValue)
            Public Sub New()
                Files = New Dictionary(Of String, FileValue)
                Settings = New Dictionary(Of String, SettingValue)
            End Sub
        End Class

        Public Class AddExistingFileBatchOperation
            Public Property ParentPath As String
            Public Property ActualFilename As String
        End Class

#End Region

#Region "Events"
        ''' <summary>
        ''' Raised when the project has been opened.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Public Event ProjectOpened(sender As Object, e As EventArgs)
        Public Event FileSaved(sender As Object, e As EventArgs) Implements ISavable.FileSaved
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Public Event BuildStatusChanged(sender As Object, e As ProjectBuildStatusChanged)
        Public Event Modified(sender As Object, e As EventArgs) Implements INotifyModified.Modified
        Public Event DirectoryCreated(sender As Object, e As DirectoryCreatedEventArgs)
        Public Event DirectoryDeleted(sender As Object, e As DirectoryDeletedEventArgs)
        Public Event FileAdded(sender As Object, e As ProjectFileAddedEventArgs)
        Public Event FileRemoved(sender As Object, e As ProjectFileRemovedEventArgs)
#End Region

        Private Sub _rootNode_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles _rootNode.PropertyChanged
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RootNode)))
        End Sub

#Region "Properties"
        Public Property ParentSolution As Solution

        ''' <summary>
        ''' Name of the project
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String

        ''' <summary>
        ''' Full path of the project file
        ''' </summary>
        ''' <returns></returns>
        Public Property Filename As String

        ''' <summary>
        ''' The root node of the logical project heiarchy
        ''' </summary>
        ''' <returns></returns>
        Public Property RootNode As ProjectNode
            Get
                Return _rootNode
            End Get
            Set(value As ProjectNode)
                If _rootNode IsNot value Then
                    _rootNode = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RootNode)))
                End If
            End Set
        End Property
        Private WithEvents _rootNode As ProjectNode

        ''' <summary>
        ''' The project's settings
        ''' </summary>
        ''' <returns></returns>
        Public Property Settings As SettingsProvider

        ''' <summary>
        ''' Gets or sets an individual setting in the project's settings.
        ''' </summary>
        ''' <param name="SettingName"></param>
        ''' <returns></returns>
        Protected Property Setting(SettingName As String) As Object
            Get
                Return Settings.GetSetting(SettingName)
            End Get
            Set(value As Object)
                Settings.SetSetting(SettingName, value)
            End Set
        End Property

        ''' <summary>
        ''' List of the names of all projects in the current solution this project references.
        ''' </summary>
        ''' <returns></returns>
        Public Property ProjectReferences As List(Of String)
            Get
                If Setting("ProjectReferences") Is Nothing Then
                    Setting("ProjectReferences") = New List(Of String)
                End If
                Return Setting("ProjectReferences")
            End Get
            Set(value As List(Of String))
                Setting("ProjectReferences") = value
            End Set
        End Property

        ''' <summary>
        ''' The progress of the current project's build.
        ''' </summary>
        ''' <returns></returns>
        Public Property BuildProgress As Single
            Get
                Return _buildProgress
            End Get
            Set(value As Single)
                _buildProgress = value
                RaiseEvent BuildStatusChanged(Me, New ProjectBuildStatusChanged With {.Progress = BuildProgress, .StatusMessage = BuildStatusMessage})
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BuildProgress)))
            End Set
        End Property
        Private _buildProgress As Single

        ''' <summary>
        ''' The current task in the current project's build.
        ''' </summary>
        ''' <returns></returns>
        Public Property BuildStatusMessage As String
            Get
                Return _buildStatusMessage
            End Get
            Set(value As String)
                _buildStatusMessage = value
                RaiseEvent BuildStatusChanged(Me, New ProjectBuildStatusChanged With {.Progress = BuildProgress, .StatusMessage = BuildStatusMessage})
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BuildStatusMessage)))
            End Set
        End Property
        Private _buildStatusMessage As String

        Public Property IsBuildProgressIndeterminate As Boolean
            Get
                Return _isBuildProgressIndeterminate
            End Get
            Set(value As Boolean)
                _isBuildProgressIndeterminate = value
                RaiseEvent BuildStatusChanged(Me, New ProjectBuildStatusChanged With {.Progress = BuildProgress, .StatusMessage = BuildStatusMessage})
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(IsBuildProgressIndeterminate)))
            End Set
        End Property
        Dim _isBuildProgressIndeterminate As Boolean

        Public Property CurrentPluginManager As PluginManager
#End Region

#Region "Functions"
        ''' <summary>
        ''' Gets the solution items at the given logical path in the solution.
        ''' </summary>
        ''' <param name="Path">Logical path to get the contents for.  Pass in String.Empty or Nothing to get the root.</param>
        ''' <returns></returns>
        Public Function GetDirectoryContents(Path As String) As IEnumerable(Of ProjectNode)
            If Path Is Nothing OrElse Path = String.Empty Then
                Return From c In RootNode.Children Order By c.Name
            Else
                Dim pathArray = Path.Replace("\", "/").Split("/")

                Dim current As ProjectNode = RootNode
                Dim index As Integer = 0
                For count = 0 To pathArray.Length - 1
                    current = (From i In current.Children Where i.Name.ToLower = pathArray(index).ToLower Select i).FirstOrDefault
                    If current Is Nothing Then
                        Throw New DirectoryNotFoundException("The given path does not exist in the project.")
                    End If
                Next
                Return From c In current.Children Order By c.Name
            End If
        End Function

        ''' <summary>
        ''' Gets the project item at the given path.
        ''' Returns Nothing if there is no project item at that path.
        ''' </summary>
        ''' <param name="ItemPath">Path to look for a project item.</param>
        ''' <returns></returns>
        Public Function GetProjectItemByPath(ItemPath As String) As ProjectNode
            Return GetProjectItemByPath(RootNode, ItemPath)
        End Function

        ''' <summary>
        ''' Gets the project item at the given path.
        ''' Returns Nothing if there is no project item at that path.
        ''' </summary>
        ''' <param name="ItemPath">Path to look for a project item.</param>
        ''' <returns></returns>
        Public Function GetProjectItemByPath(rootNode As ProjectNode, ItemPath As String) As ProjectNode
            If ItemPath Is Nothing OrElse ItemPath = "" Then
                Return rootNode
            Else
                Dim path = ItemPath.Replace("\", "/").TrimStart("/").Split("/")
                Dim current = rootNode
                For count = 0 To path.Length - 2
                    Dim i = count 'I got a warning about using an iterator variable in the line below
                    Dim child = (From c In current.Children Where c.Name.ToLower = path(i).ToLower).FirstOrDefault

                    If child Is Nothing Then
                        Dim newNode As New ProjectNode(Me, current)
                        'newNode.IsDirectory = True
                        newNode.Name = path(count)
                        current.Children.Add(newNode)
                        current = newNode
                    Else
                        current = child
                    End If

                Next
                Dim proj As ProjectNode = (From c In current.Children Where c.Name.ToLower = path.Last.ToLower).FirstOrDefault
                If proj IsNot Nothing Then
                    Return proj
                Else
                    Return Nothing
                End If
            End If
        End Function

        ''' <summary>
        ''' Gets the file at the given path.
        ''' Returns nothing if there is no file at that path.
        ''' </summary>
        ''' <param name="Path">Path to look for a file.</param>
        ''' <returns></returns>
        Public Async Function GetFileByPath(Path As String, manager As PluginManager) As Task(Of Object)
            Return Await GetProjectItemByPath(Path)?.GetFile(manager)
        End Function

        ''' <summary>
        ''' Returns whether or not a directory can be made inside the given path.
        ''' </summary>
        ''' <param name="Path">Path to put the directory.</param>
        ''' <returns></returns>
        Public Overridable Function CanCreateDirectory(Path As String) As Boolean
            Return (GetProjectItemByPath(Path) IsNot Nothing)
        End Function

        ''' <summary>
        ''' Creates a directory at the given location if it does not exist.
        ''' </summary>
        ''' <param name="Path">Path to put the new directory in.</param>
        ''' <param name="DirectoryName">Name of the new directory.</param>
        Public Overridable Sub CreateDirectory(Path As String, DirectoryName As String)
            CreateDirectory(RootNode, Path, DirectoryName)
        End Sub

        ''' <summary>
        ''' Creates a directory at the given location if it does not exist.
        ''' </summary>
        ''' <param name="Path">Path to put the new directory in.</param>
        ''' <param name="DirectoryName">Name of the new directory.</param>
        Public Overridable Sub CreateDirectory(rootNode As ProjectNode, Path As String, DirectoryName As String)
            Dim item = GetProjectItemByPath(rootNode, Path)
            If item Is Nothing Then
                'Throw New IO.DirectoryNotFoundException("Cannot create a solution directory at the given path: " & Path)
                Dim pathParts = Path.Replace("\", "/").TrimStart("/").Split("/")
                Dim parentPath As New Text.StringBuilder
                For count = 0 To pathParts.Length - 2
                    parentPath.Append(pathParts(count))
                    parentPath.Append("/")
                Next
                Dim parentPathString = parentPath.ToString.TrimEnd("/")
                CreateDirectory(rootNode, parentPathString, pathParts.Last)
                item = GetProjectItemByPath(rootNode, Path)
            End If
            Dim q = (From c In item.Children Where TypeOf c Is ProjectNode AndAlso c.Name.ToLower = DirectoryName.ToLower AndAlso DirectCast(c, ProjectNode).IsDirectory = True).FirstOrDefault
            If q Is Nothing Then
                item.Children.Add(New ProjectNode(Me, item) With {.Name = DirectoryName})
                RaiseEvent DirectoryCreated(Me, New DirectoryCreatedEventArgs With {.DirectoryName = DirectoryName, .ParentPath = Path, .FullPath = Path & "/" & DirectoryName})
            Else
                'There's already a directory here.
                'Do nothing.
            End If
        End Sub

        ''' <summary>
        ''' Creates a directory with the given full path.
        ''' </summary>
        ''' <param name="FullPath"></param>
        Public Overridable Sub CreateDirectory(FullPath As String)
            CreateDirectory(RootNode, FullPath)
        End Sub

        ''' <summary>
        ''' Creates a directory with the given full path.
        ''' </summary>
        ''' <param name="FullPath"></param>
        Public Overridable Sub CreateDirectory(rootNode As ProjectNode, FullPath As String)
            Dim pathParts = FullPath.Replace("\", "/").TrimStart("/").Split("/")
            Dim parentPath As New Text.StringBuilder
            For count = 0 To pathParts.Length - 2
                parentPath.Append(pathParts(count))
                parentPath.Append("/")
            Next
            Dim parentPathString = parentPath.ToString.TrimEnd("/")
            CreateDirectory(rootNode, parentPathString, pathParts.Last)
        End Sub

        ''' <summary>
        ''' Returns whether or not the directory at the given path can be deleted.
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <returns></returns>
        Public Overridable Function CanDeleteDirectory(Path As String) As Boolean
            Return (GetProjectItemByPath(Path) IsNot Nothing)
        End Function

        ''' <summary>
        ''' Deletes the directory at the given path, and disposes of everything inside it.
        ''' Unless overridden, and contained projects will remain on the hard drive.
        ''' </summary>
        ''' <param name="Path"></param>
        Public Overridable Sub DeleteDirectory(Path As String)
            Dim pathParts = Path.Replace("\", "/").TrimStart("/").Split("/")
            Dim parentPath As New Text.StringBuilder
            For count = 0 To pathParts.Length - 2
                parentPath.Append(pathParts(count))
                parentPath.Append("/")
            Next
            Dim parentPathString = parentPath.ToString.TrimEnd("/")
            Dim parent = GetProjectItemByPath(parentPathString)
            Dim child = (From c In parent.Children Where TypeOf c Is ProjectNode AndAlso c.Name.ToLower = pathParts.Last.ToLower AndAlso DirectCast(c, ProjectNode).IsDirectory = True Select DirectCast(c, ProjectNode)).FirstOrDefault
            If child IsNot Nothing Then
                parent.Children.Remove(child)
                child.Dispose()
                RaiseEvent DirectoryDeleted(Me, New DirectoryDeletedEventArgs With {.DirectoryName = pathParts.Last, .ParentPath = parentPathString, .FullPath = Path})
            End If
        End Sub

        Public Overridable Function CanCreateFile(Path As String) As Boolean
            Return CanCreateDirectory(Path)
        End Function

        Public Overridable Function CanAddExistingFile(Path As String) As Boolean
            Return CanCreateFile(Path)
        End Function

        Public Overridable Function GetSupportedFileTypes(Path As String, manager As PluginManager) As IEnumerable(Of TypeInfo)
            If CanCreateDirectory(Path) Then
                Return IOHelper.GetCreatableFileTypes(manager)
            Else
                Return {}
            End If
        End Function

        Public Overridable Sub CreateFile(ParentPath As String, FileName As String, FileType As Type)
            Dim item = GetProjectItemByPath(ParentPath)
            If item IsNot Nothing Then
                FileName = FileName.Replace("\", "/").TrimStart("/")
                Dim q = (From c In item.Children Where TypeOf c Is ProjectNode AndAlso c.Name.ToLower = FileName.ToLower AndAlso DirectCast(c, ProjectNode).IsDirectory = False).FirstOrDefault
                If q Is Nothing Then
                    Dim fileObj As ICreatableFile = ReflectionHelpers.CreateInstance(FileType.GetTypeInfo)
                    fileObj.CreateFile(FileName)
                    fileObj.Filename = Path.Combine(Path.GetDirectoryName(Me.Filename), ParentPath.Replace("/", "\").TrimStart("\"), FileName)

                    Dim projItem As New ProjectNode(Me, item, fileObj)
                    projItem.Filename = ParentPath & "/" & FileName
                    projItem.Name = FileName
                    item.Children.Add(projItem)
                    RaiseEvent FileAdded(Me, New ProjectFileAddedEventArgs With {.ParentPath = ParentPath, .File = fileObj, .Filename = FileName, .FullFilename = fileObj.Filename})
                Else
                    'There's already a project here
                    'Todo: throw better exception
                    Throw New Exception("A file with the given already exists in the given path: " & ParentPath)
                End If
            Else
                Throw New IO.DirectoryNotFoundException("Cannot create a file at the given path: " & ParentPath)
            End If
        End Sub

        Public Overridable Function IsFileSupported(ParentProjectPath As String, Filename As String)
            Return CanAddExistingFile(ParentProjectPath)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ParentProjectPath">Directory to put the imported file.</param>
        ''' <param name="FullFilename">Full path of the file to import.</param>
        ''' <returns></returns>
        Protected Overridable Function GetImportedFilePath(ParentProjectPath As String, FullFilename As String)
            Return Path.Combine(ParentProjectPath, Path.GetFileName(FullFilename))
        End Function

        Public Overridable Function GetImportIOFilter(ParentProjectPath As String, manager As PluginManager) As String
            Return "All Files (*.*)|*.*" 'manager.IOFiltersString
        End Function

        Public Overridable Async Function AddExistingFile(ParentProjectPath As String, FilePath As String, provider As IOProvider) As Task
            Await AddExistingFile(RootNode, ParentProjectPath, FilePath, provider)
        End Function

        Protected Overridable Async Function AddExistingFile(rootProjectNode As ProjectNode, projectPath As String, FilePath As String, provider As IOProvider) As Task
            Dim item = GetProjectItemByPath(rootProjectNode, projectPath)
            If item IsNot Nothing Then
                Dim filename = Path.GetFileName(FilePath)
                filename = filename.Replace("\", "/").TrimStart("/")
                Dim q = (From c In item.Children Where TypeOf c Is ProjectNode AndAlso c.Name.ToLower = filename.ToLower AndAlso DirectCast(c, ProjectNode).IsDirectory = False).FirstOrDefault
                If q Is Nothing Then
                    Dim projItem As New ProjectNode(Me, item)
                    projItem.Filename = GetImportedFilePath(projectPath, FilePath)

                    Dim source = FilePath
                    Dim dest = Path.Combine(Path.GetDirectoryName(Me.Filename), projItem.Filename.Replace("/", "\").TrimStart("\"))

                    Await Task.Run(New Action(Sub()
                                                  If Not source.Replace("\", "/").ToLower = dest.Replace("\", "/").ToLower Then
                                                      provider.CopyFile(FilePath, dest)
                                                  End If
                                              End Sub))

                    projItem.Name = Path.GetFileName(projItem.Filename)
                    item.Children.Add(projItem)
                    RaiseEvent FileAdded(Me, New ProjectFileAddedEventArgs With {.Filename = projItem.Name, .FullFilename = dest})
                Else
                    'There's already a project here
                    'Todo: throw exception
                    'Throw New ProjectAlreadyExistsException("A project with the name """ & ProjectName & """ already exists in the given path: " & ParentPath)
                End If
            End If
        End Function

        Public Overridable Async Function RecreateRootWithExistingFiles(files As IEnumerable(Of AddExistingFileBatchOperation), provider As IOProvider) As Task
            Dim newRoot As New ProjectNode(Me, Nothing)
            For Each item In files
                If Not String.IsNullOrEmpty(item.ParentPath) Then
                    CreateDirectory(newRoot, item.ParentPath)
                End If
                Await AddExistingFile(newRoot, item.ParentPath, item.ActualFilename, provider)
            Next
            RootNode.Children = newRoot.Children
        End Function

        Public Overridable Function CanDeleteFile(FilePath As String) As Boolean
            Return (GetProjectItemByPath(FilePath) IsNot Nothing)
        End Function

        Public Overridable Sub DeleteFile(FilePath As String)
            Dim pathParts = FilePath.Replace("\", "/").TrimStart("/").Split("/")
            Dim parentPath As New Text.StringBuilder
            For count = 0 To pathParts.Length - 2
                parentPath.Append(pathParts(count))
                parentPath.Append("/")
            Next
            Dim parentPathString = parentPath.ToString.TrimEnd("/")
            Dim parent = GetProjectItemByPath(parentPathString)
            Dim child = (From c In parent.Children Where TypeOf c Is ProjectNode AndAlso c.Name.ToLower = pathParts.Last.ToLower AndAlso DirectCast(c, ProjectNode).IsDirectory = False Select DirectCast(c, ProjectNode)).FirstOrDefault
            If child IsNot Nothing Then
                parent.Children.Remove(child)
                child.Dispose()
                RaiseEvent FileRemoved(Me, New ProjectFileRemovedEventArgs With {.FileName = pathParts.Last, .ParentPath = parentPathString, .FullPath = FilePath})
            End If
        End Sub

        Public Overridable Function Build() As Task
            Return Task.FromResult(0)
        End Function

        Public Overridable Function CanBuild() As Boolean
            Return False
        End Function

        Public Overridable Function GetRootDirectory() As String
            Return Path.GetDirectoryName(Me.Filename)
        End Function

        Public Function GetReferences(Solution As Solution) As IEnumerable(Of Project)
            Dim out As New List(Of Project)
            For Each item In ProjectReferences
                Dim p = Solution.GetProjectsByName(item).FirstOrDefault
                If p IsNot Nothing Then
                    out.Add(p)
                End If
            Next
            Return out
        End Function

        ''' <summary>
        ''' Returns whether or not this project contains a circular reference back to itself.
        ''' It does not detect whether other projects this one references have their own circular references.
        ''' </summary>
        ''' <param name="Solution"></param>
        ''' <returns></returns>
        Public Function HasCircularReferences(Solution As Solution) As Boolean
            Dim tree As New List(Of Project)
            FillReferenceTree(Solution, tree, Me)
            Return tree.Contains(Me)
        End Function

        ''' <summary>
        ''' Fills Tree with all the references of the current item.
        ''' Stops if the last item added is the current instance of project.
        ''' </summary>
        ''' <param name="Solution"></param>
        ''' <param name="Tree"></param>
        ''' <param name="CurrentItem"></param>
        Private Sub FillReferenceTree(Solution As Solution, Tree As List(Of Project), CurrentItem As Project)
            For Each item In CurrentItem.GetReferences(Solution)
                Tree.Add(item)
                If item Is Me Then
                    Exit Sub
                Else
                    If Not item.HasCircularReferences(Solution) Then
                        FillReferenceTree(Solution, Tree, item)
                    End If
                End If
            Next
        End Sub
#End Region

#Region "Create New"
        ''' <summary>
        ''' Creates and returns a new Project.
        ''' </summary>
        ''' <param name="ProjectDirectory">Directory to store the Project.  Project will be stored in a sub directory of the one given.</param>
        ''' <param name="ProjectName">Name of the Project.</param>
        ''' <returns></returns>
        Public Shared Function CreateProject(ProjectDirectory As String, ProjectName As String, parent As Solution, manager As PluginManager) As Project
            Return CreateProject(ProjectDirectory, ProjectName, GetType(Project), parent, manager)
        End Function

        ''' <summary>
        ''' Creates and returns a new Project.
        ''' </summary>
        ''' <param name="ProjectDirectory">Directory to store the Project.  Project will be stored in a sub directory of the one given.</param>
        ''' <param name="ProjectName">Name of the Project.</param>
        ''' <param name="ProjectType">Type of the Project to create.  Must inherit from Project.</param>
        ''' <returns></returns>
        Public Shared Function CreateProject(ProjectDirectory As String, ProjectName As String, ProjectType As Type, parent As Solution, manager As PluginManager) As Project
            If ProjectDirectory Is Nothing Then
                Throw New ArgumentNullException(NameOf(ProjectDirectory))
            End If
            If ProjectName Is Nothing Then
                Throw New ArgumentNullException(NameOf(ProjectName))
            End If
            If ProjectType Is Nothing Then
                Throw New ArgumentNullException(NameOf(ProjectType))
            End If
            If Not ReflectionHelpers.IsOfType(ProjectType, GetType(Project).GetTypeInfo) Then
                Throw New ArgumentException("ProjectType must inherit from Project.", NameOf(ProjectType))
            End If

            Dim dir = Path.Combine(ProjectDirectory, ProjectName)
            If Not manager.CurrentIOProvider.DirectoryExists(dir) Then
                manager.CurrentIOProvider.CreateDirectory(dir)
            End If

            Dim output As Project = ReflectionHelpers.CreateInstance(ProjectType.GetTypeInfo)
            output.Filename = Path.Combine(dir, ProjectName & ".skyproj")
            output.Name = ProjectName
            output.CurrentPluginManager = manager
            output.ParentSolution = parent

            Dim projFile As New ProjectFile With {.Name = ProjectName, .AssemblyQualifiedTypeName = ProjectType.AssemblyQualifiedName}
            projFile.FileFormat = ProjectFile.CurrentVersion
            output.LoadProjectFile(projFile, manager)

            Return output
        End Function
#End Region

#Region "Open"
        ''' <summary>
        ''' Opens and returns the solution at the given filename.
        ''' </summary>
        ''' <param name="Filename"></param>
        ''' <returns></returns>
        Public Shared Function OpenProjectFile(Filename As String, parent As Solution, manager As PluginManager) As Project
            If Filename Is Nothing Then
                Throw New ArgumentNullException(NameOf(Filename))
            End If
            If Not manager.CurrentIOProvider.FileExists(Filename) Then
                Throw New FileNotFoundException("Could not find a file at the given filename.", Filename)
            End If

            Dim projectInfo As ProjectFile = Json.DeserializeFromFile(Of ProjectFile)(Filename, manager.CurrentIOProvider)
            'Legacy support
            If String.IsNullOrEmpty(projectInfo.FileFormat) Then
                Dim legacy As ProjectFileLegacy = Json.DeserializeFromFile(Of ProjectFileLegacy)(Filename, manager.CurrentIOProvider)
                projectInfo.FileFormat = "1"

                'Read the settings
                If legacy.Settings IsNot Nothing Then
                    Dim s As New SettingsProvider
                    For Each item In legacy.Settings
                        Dim valueType = ReflectionHelpers.GetTypeByName(item.Value.AssemblyQualifiedTypeName, manager)
                        If valueType IsNot Nothing Then
                            s.SetSetting(item.Key, Json.Deserialize(valueType.AsType, item.Value.ValueJson))
                        End If
                        'If the valueType IS nothing, then the type can't be found, and we won't save the setting
                    Next
                    projectInfo.InternalSettings = s.Serialize
                End If

            End If
            Dim type As TypeInfo = ReflectionHelpers.GetTypeByName(projectInfo.AssemblyQualifiedTypeName, manager)
            If type Is Nothing Then
                'Default to Project if the saved type cannot be found
                type = GetType(Project).GetTypeInfo
            End If

            Dim out As Project = ReflectionHelpers.CreateInstance(type)
            out.Filename = Filename
            out.LoadProjectFile(projectInfo, manager)
            out.CurrentPluginManager = manager
            out.ParentSolution = parent

            Return out
        End Function

        Private Sub LoadProjectFile(File As ProjectFile, manager As PluginManager)
            Me.Name = File.Name

            'Load Settings
            Settings = SettingsProvider.Deserialize(File.InternalSettings, manager)

            'Load Files
            For Each item In File.Files
                Dim path = item.Key.Replace("\", "/").TrimStart("/").Split("/")
                Dim current = Me.RootNode
                'Create the directory nodes
                For count = 0 To path.Length - 2
                    'Try to get the directory node we're looking for
                    Dim i = count 'I got a warning about using an iterator variable in the line below
                    Dim child = (From c In current.Children Where c.Name.ToLower = path(i).ToLower).FirstOrDefault

                    If child Is Nothing Then
                        'Create it if it doesn't exist
                        Dim newNode As New ProjectNode(Me, current)
                        newNode.Name = path(count)
                        current.Children.Add(newNode)
                        current = newNode
                    Else
                        'Select it otherwise
                        current = child
                    End If

                Next
                'Create the file node
                Dim proj = (From c In current.Children Where c.Name.ToLower = path.Last.ToLower).FirstOrDefault
                If proj Is Nothing Then
                    'If it doesn't exist, create it
                    Dim newNode As New ProjectNode(Me, current)
                    newNode.Name = path.Last
                    If item.Value IsNot Nothing Then
                        newNode.Filename = item.Value.Filename 'IO.Path.Combine(IO.Path.GetDirectoryName(Me.Filename), item.Value.Filename.Replace("/", "\").TrimStart("\"))
                        newNode.AssemblyQualifiedTypeName = item.Value.AssemblyQualifiedTypeName
                    Else
                        newNode.Filename = Nothing
                    End If
                    current.Children.Add(newNode)
                Else
                    'If it does exist, there's already a file with the same name.
                    'Todo: replace with better exception
                    Throw New Exception("Duplicate file detected: " & path.Last & ".")
                End If
            Next

            RaiseEvent ProjectOpened(Me, New EventArgs)
        End Sub
#End Region

#Region "Save"
        Public Sub Save(provider As IOProvider) Implements ISavable.Save
            Dim file As New ProjectFile
            file.FileFormat = ProjectFile.CurrentVersion
            file.AssemblyQualifiedTypeName = Me.GetType.AssemblyQualifiedName
            file.Name = Me.Name
            file.InternalSettings = Me.Settings.Serialize
            file.Files = GetProjectDictionary(RootNode, "")
            Json.SerializeToFile(Filename, file, provider)
            RaiseEvent FileSaved(Me, New EventArgs)
        End Sub

        Private Function GetProjectDictionary(ProjectNode As ProjectNode, CurrentPath As String) As Dictionary(Of String, FileValue)
            If Not ProjectNode.IsDirectory Then
                'It's a file
                Dim out As New Dictionary(Of String, FileValue)
                out.Add(CurrentPath, New FileValue With {.Filename = ProjectNode.Filename.Replace(Path.GetDirectoryName(Filename), ""), .AssemblyQualifiedTypeName = ProjectNode.AssemblyQualifiedTypeName})
                Return out
            ElseIf ProjectNode.IsDirectory AndAlso ProjectNode.Children.Count = 0 AndAlso Not CurrentPath = "" Then
                'It's a directory with no children
                Dim out As New Dictionary(Of String, FileValue)
                out.Add(CurrentPath, Nothing)
                Return out
            Else
                'Otherwise, merge with a recursive call
                Dim out As New Dictionary(Of String, FileValue)
                For Each item In ProjectNode.Children
                    Dim toMerge = GetProjectDictionary(item, CurrentPath & "/" & item.Name)
                    For Each entry In toMerge
                        out.Add(entry.Key, entry.Value)
                    Next
                Next
                Return out
            End If
        End Function
#End Region


#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
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
End Namespace

