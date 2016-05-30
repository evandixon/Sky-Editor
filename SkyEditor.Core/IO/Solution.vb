Imports System.IO
Imports System.Reflection
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities

Namespace IO
    Public Class Solution
        Implements INotifyModified
        Implements IDisposable

#Region "Child Classes"
        Private Class SolutionFile
            Public Property FileFormat As String
            Public Property AssemblyQualifiedTypeName As String
            Public Property Name As String
            Public Property InternalSettings As String 'Serialized settings provider
            Public Property Projects As Dictionary(Of String, String)
            Public Sub New()
                FileFormat = "v2"
            End Sub
        End Class

        Private Class SettingValue
            Public Property AssemblyQualifiedTypeName As String
            Public Property ValueJson As String
        End Class

        Private Class SolutionFileLegacy
            Public Property AssemblyQualifiedTypeName As String
            Public Property Name As String
            ''' <summary>
            ''' Matches solution paths to project files, which are relative to the solution directory.
            ''' </summary>
            ''' <returns></returns>
            Public Property Projects As Dictionary(Of String, String)
            Public Property Settings As Dictionary(Of String, SettingValue)
            Public Sub New()
                Projects = New Dictionary(Of String, String)
                Settings = New Dictionary(Of String, SettingValue)
            End Sub
        End Class

        Public Class ProjectAlreadyExistsException
            Inherits Exception
            Public Sub New()
                MyBase.New
            End Sub
            Public Sub New(Message As String)
                MyBase.New(Message)
            End Sub
        End Class
#End Region

        Public Sub New()
            Root = New SolutionNode(Me, Nothing)
        End Sub


        ''' <summary>
        ''' Name of the solution
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String

        ''' <summary>
        ''' Full path of the solution file
        ''' </summary>
        ''' <returns></returns>
        Public Property Filename As String

        ''' <summary>
        ''' The solution settings
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
        ''' The root of the solution's logical heiarchy.
        ''' </summary>
        ''' <returns></returns>
        Public Property Root As SolutionNode

        ''' <summary>
        ''' Gets or sets whether or not there are unsaved changes
        ''' </summary>
        ''' <returns></returns>
        Public Property UnsavedChanges As Boolean
            Get
                Return _unsavedChanges
            End Get
            Set(value As Boolean)
                _unsavedChanges = value
                If value Then
                    RaiseEvent Modified(Me, New EventArgs)
                End If
            End Set
        End Property
        Dim _unsavedChanges As Boolean

        Public Property CurrentPluginManager As PluginManager


#Region "Events"
        ''' <summary>
        ''' Raised when the solution is saved.
        ''' </summary>
        Public Event FileSaved(sender As Object, e As EventArgs)

        ''' <summary>
        ''' Raised when the solution has been created.
        ''' </summary>
        Public Event Created(sender As Object, e As EventArgs)
        Public Event Modified(sender As Object, e As EventArgs) Implements INotifyModified.Modified
        Public Event SolutionBuildStarted(sender As Object, e As EventArgs)
        Public Event SolutionBuildCompleted(sender As Object, e As EventArgs)
        Public Event DirectoryCreated(sender As Object, e As DirectoryCreatedEventArgs)
        Public Event DirectoryDeleted(sender As Object, e As DirectoryDeletedEventArgs)
        Public Event ProjectAdded(sender As Object, e As ProjectAddedEventArgs)
        Public Event ProjectRemoving(sender As Object, e As ProjectRemovingEventArgs)
        Public Event ProjectRemoved(sender As Object, e As ProjectRemovedEventArgs)

#End Region

#Region "Functions"
        ''' <summary>
        ''' Raises the Created event
        ''' </summary>
        Private Sub RaiseCreated()
            RaiseEvent Created(Me, New EventArgs)
        End Sub

        Private Sub Project_Modified(sender As Object, e As EventArgs)
            UnsavedChanges = True
            RaiseEvent Modified(Me, New EventArgs)
        End Sub

        ''' <summary>
        ''' Gets the types of projects that can be added in a particular directory.
        ''' </summary>
        ''' <param name="path">Logical solution path in question</param>
        ''' <param name="manager">Instance of the currrent plugin manager</param>
        ''' <returns></returns>
        Public Overridable Function GetSupportedProjectTypes(path As String, manager As PluginManager) As IEnumerable(Of TypeInfo)
            If CanCreateDirectory(path) Then
                Return manager.GetRegisteredTypes(GetType(Project).GetTypeInfo)
            Else
                Return {}
            End If
        End Function

        ''' <summary>
        ''' Gets all projects in the solution, regardless of their parent directory.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetAllProjects() As IEnumerable(Of Project)
            Return GetAllProjects(Root)
        End Function

        ''' <summary>
        ''' Gets all projects that are in the given solution node
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function GetAllProjects(node As SolutionNode) As IEnumerable(Of Project)
            Dim output As New List(Of Project)
            If node.Children.Count > 0 Then
                If node.IsDirectory Then
                    For Each item In node.Children
                        output.AddRange(GetAllProjects(item))
                    Next
                End If
            End If

            If node.IsDirectory = False AndAlso node.Project IsNot Nothing Then
                output.Add(node.Project)
            End If

            Return output
        End Function

        ''' <summary>
        ''' Saves all the projects in the solution
        ''' </summary>
        ''' <param name="provider"></param>
        Public Overridable Sub SaveAllProjects(provider As IOProvider)
            For Each item In GetAllProjects()
                item.Save(provider)
            Next
        End Sub

        ''' <summary>
        ''' Returns all projects in the solution with the given name, regardless of directories.
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        Public Overridable Function GetProjectsByName(Name As String) As IEnumerable(Of Project)
            Return From p In GetAllProjects() Where p.Name.ToLower = Name.ToLower
        End Function

#Region "Solution Logical Filesystem"
        ''' <summary>
        ''' Gets the solution items at the given logical path in the solution.
        ''' </summary>
        ''' <param name="Path">Logical path to get the contents for.  Pass in String.Empty or Nothing to get the root.</param>
        ''' <returns></returns>
        Public Function GetDirectoryContents(Path As String) As IEnumerable(Of SolutionNode)
            If Path Is Nothing OrElse Path = String.Empty Then
                Return From c In Root.Children Order By c.Name
            Else
                Dim pathArray = Path.Replace("\", "/").Split("/")

                Dim current As SolutionNode = Root
                Dim index As Integer = 0
                For count = 0 To pathArray.Length - 1
                    current = (From i In current.Children Where i.Name.ToLower = pathArray(index).ToLower Select i).FirstOrDefault
                    If current Is Nothing Then
                        Throw New DirectoryNotFoundException("The given path does not exist in the solution.")
                    End If
                Next
                Return From c In current.Children Order By c.Name
            End If
        End Function

        ''' <summary>
        ''' Gets the solution item at the given path.
        ''' Returns Nothing if there is no solution item at that path.
        ''' </summary>
        ''' <param name="ItemPath">Path to look for a solution item.</param>
        ''' <returns></returns>
        Public Function GetSolutionItemByPath(ItemPath As String) As SolutionNode
            If ItemPath Is Nothing OrElse ItemPath = "" Then
                Return Root
            Else
                Dim path = ItemPath.Replace("\", "/").TrimStart("/").Split("/")
                Dim current = Me.Root
                For count = 0 To path.Length - 2
                    Dim i = count 'I got a warning about using an iterator variable in the line below
                    Dim child = (From c In current.Children Where c.Name.ToLower = path(i).ToLower).FirstOrDefault

                    If child Is Nothing Then
                        Dim newNode As New SolutionNode(Me, current)
                        newNode.Name = path(count)
                        current.Children.Add(newNode)
                        current = newNode
                    Else
                        current = child
                    End If

                Next
                Dim proj As SolutionNode = (From c In current.Children Where c.Name.ToLower = path.Last.ToLower).FirstOrDefault
                If proj IsNot Nothing Then
                    Return proj
                Else
                    Return Nothing
                End If
            End If
        End Function

        ''' <summary>
        ''' Gets the project at the given path.
        ''' Returns nothing if there is no project at that path.
        ''' </summary>
        ''' <param name="Path">Path to look for a project.</param>
        ''' <returns></returns>
        Public Function GetProjectByPath(Path As String) As Project
            Return GetSolutionItemByPath(Path)?.Project
        End Function

        ''' <summary>
        ''' Creates a directory at the given location if it does not exist.
        ''' </summary>
        ''' <param name="Path">Path to put the new directory in.</param>
        ''' <param name="DirectoryName">Name of the new directory.</param>
        Public Overridable Sub CreateDirectory(Path As String, DirectoryName As String)
            Dim item = GetSolutionItemByPath(Path)
            If item IsNot Nothing Then
                Dim q = (From c In item.Children Where TypeOf c Is SolutionNode AndAlso c.Name.ToLower = DirectoryName.ToLower AndAlso DirectCast(c, SolutionNode).IsDirectory = True).FirstOrDefault
                If q Is Nothing Then
                    item.Children.Add(New SolutionNode(Me, item) With {.Name = DirectoryName})
                    RaiseEvent DirectoryCreated(Me, New DirectoryCreatedEventArgs With {.DirectoryName = DirectoryName, .ParentPath = Path, .FullPath = Path & "/" & DirectoryName})
                Else
                    'There's already a directory here.
                    'Do nothing.
                End If
            Else
                Throw New DirectoryNotFoundException("Cannot create a solution directory at the given path: " & Path)
            End If
        End Sub

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
            Dim parent = GetSolutionItemByPath(parentPathString)
            Dim child = (From c In parent.Children Where TypeOf c Is SolutionNode AndAlso c.Name.ToLower = pathParts.Last.ToLower AndAlso DirectCast(c, SolutionNode).IsDirectory = True Select DirectCast(c, SolutionNode)).FirstOrDefault
            If child IsNot Nothing Then
                parent.Children.Remove(child)
                child.Dispose()
                RaiseEvent DirectoryDeleted(Me, New DirectoryDeletedEventArgs With {.DirectoryName = pathParts.Last, .ParentPath = parentPathString, .FullPath = Path})
            End If
        End Sub

        Public Overridable Async Function CreateProject(ParentPath As String, ProjectName As String, ProjectType As Type, manager As PluginManager) As Task
            Dim item = GetSolutionItemByPath(ParentPath)
            If item IsNot Nothing Then
                Dim q = (From c In item.Children Where TypeOf c Is SolutionNode AndAlso c.Name.ToLower = ProjectName.ToLower AndAlso DirectCast(c, SolutionNode).IsDirectory = False).FirstOrDefault
                If q Is Nothing Then
                    Dim p = Project.CreateProject(Path.GetDirectoryName(Me.Filename), ProjectName, ProjectType, Me, manager)
                    item.Children.Add(New SolutionNode(Me, item) With {.Name = ProjectName, .Project = p})
                    AddHandler p.Modified, AddressOf Project_Modified
                    RaiseEvent Modified(Me, New EventArgs)
                    Await Task.Run(Sub()
                                       RaiseEvent ProjectAdded(Me, New ProjectAddedEventArgs With {.ParentPath = ParentPath, .Project = p})
                                   End Sub)
                Else
                    'There's already a project here
                    Throw New ProjectAlreadyExistsException("A project with the name """ & ProjectName & """ already exists in the given path: " & ParentPath)
                End If
            Else
                Throw New DirectoryNotFoundException("Cannot create a project at the given path: " & ParentPath)
            End If
        End Function

        Public Overridable Sub AddExistingProject(ParentPath As String, ProjectFilename As String, manager As PluginManager)
            Dim item = GetSolutionItemByPath(ParentPath)
            If item IsNot Nothing Then
                Dim p = Project.OpenProjectFile(ProjectFilename, Me, manager)
                Dim q = (From c In item.Children Where TypeOf c Is SolutionNode AndAlso c.Name.ToLower = p.Name.ToLower AndAlso DirectCast(c, SolutionNode).IsDirectory = False).FirstOrDefault
                If q Is Nothing Then
                    'Dim p = Project.CreateProject(IO.Path.GetDirectoryName(Me.Filename), ProjectName, ProjectType)
                    item.Children.Add(New SolutionNode(Me, item) With {.Name = p.Name, .Project = p})
                    AddHandler p.Modified, AddressOf Project_Modified
                    RaiseEvent Modified(Me, New EventArgs)
                    RaiseEvent ProjectAdded(Me, New ProjectAddedEventArgs With {.ParentPath = ParentPath, .Project = p})
                Else
                    'There's already a project here
                    Throw New ProjectAlreadyExistsException("A project with the name """ & p.Name & """ already exists in the given path: " & ParentPath)
                End If
            Else
                Throw New DirectoryNotFoundException("Cannot create a project at the given path: " & ParentPath)
            End If
        End Sub

        Public Overridable Sub DeleteProject(ProjectPath As String)
            Dim pathParts = ProjectPath.Replace("\", "/").TrimStart("/").Split("/")
            Dim parentPath As New Text.StringBuilder
            For count = 0 To pathParts.Length - 2
                parentPath.Append(pathParts(count))
                parentPath.Append("/")
            Next
            Dim parentPathString = parentPath.ToString.TrimEnd("/")
            Dim parent = GetSolutionItemByPath(parentPathString)
            Dim child = (From c In parent.Children Where TypeOf c Is SolutionNode AndAlso c.Name.ToLower = pathParts.Last.ToLower AndAlso DirectCast(c, SolutionNode).IsDirectory = False Select DirectCast(c, SolutionNode)).FirstOrDefault
            If child IsNot Nothing Then
                RaiseEvent ProjectRemoving(Me, New ProjectRemovingEventArgs With {.Project = child.Project})
                RemoveHandler child.Project.Modified, AddressOf Project_Modified
                RaiseEvent Modified(Me, New EventArgs)
                parent.Children.Remove(child)
                child.Dispose()
                RaiseEvent ProjectRemoved(Me, New ProjectRemovedEventArgs With {.DirectoryName = pathParts.Last, .ParentPath = parentPathString, .FullPath = ProjectPath})
            End If
        End Sub
#End Region

#Region "Can do X"
        ''' <summary>
        ''' Returns whether or not a directory can be made inside the given path.
        ''' </summary>
        ''' <param name="Path">Path to put the directory.</param>
        ''' <returns></returns>
        Public Overridable Function CanCreateDirectory(Path As String) As Boolean
            Return (GetSolutionItemByPath(Path) IsNot Nothing)
        End Function

        ''' <summary>
        ''' Returns whether or not the directory at the given path can be deleted.
        ''' </summary>
        ''' <param name="Path"></param>
        ''' <returns></returns>
        Public Overridable Function CanDeleteDirectory(Path As String) As Boolean
            Return (GetSolutionItemByPath(Path) IsNot Nothing)
        End Function

        Public Overridable Function CanCreateProject(Path As String) As Boolean
            Return CanCreateDirectory(Path)
        End Function

        Public Overridable Function CanDeleteProject(ProjectPath As String) As Boolean
            Return (GetSolutionItemByPath(ProjectPath) IsNot Nothing)
        End Function
#End Region

#Region "Building"
        Public Overridable Function GetProjectsToBuild() As IEnumerable(Of Project)
            Return From p In Me.GetAllProjects Where p.CanBuild
        End Function

        Public Overridable Async Function Build() As Task
            Await Build(GetProjectsToBuild)
        End Function

        Public Overridable Async Function Build(projects As IEnumerable(Of Project)) As Task
            RaiseEvent SolutionBuildStarted(Me, New EventArgs)
            Dim toBuild As New Dictionary(Of Project, Boolean)

            For Each item In projects
                If Not item.HasCircularReferences(Me) Then
                    toBuild.Add(item, False)
                Else
                    Throw New Exception("Circular reference detected")
                End If
            Next

            For count = 0 To toBuild.Keys.Count - 1
                Dim key = toBuild.Keys(count)
                'If this project has not been built
                If Not toBuild(key) Then
                    'Then build the project, but build its dependencies first
                    Await BuildProjects(toBuild, key)
                End If
            Next

            RaiseEvent SolutionBuildCompleted(Me, New EventArgs)
        End Function

        Private Async Function BuildProjects(ToBuild As Dictionary(Of Project, Boolean), CurrentProject As Project) As Task
            Dim buildTasks As New List(Of Task)
            For Each item In From p In CurrentProject.GetReferences(Me) Where p.CanBuild
                buildTasks.Add(BuildProjects(ToBuild, item))
            Next
            Await Task.WhenAll(buildTasks)

            If Not ToBuild(CurrentProject) Then
                'Todo: make sure we won't get here twice, with all the async stuff going on
                ToBuild(CurrentProject) = True
                UpdateBuildLoadingStatus(ToBuild)
                Await CurrentProject.Build
            End If
        End Function

        <Obsolete> Private Sub UpdateBuildLoadingStatus(toBuild As Dictionary(Of Project, Boolean))
            'Dim built As Integer = (From v In toBuild.Values Where v = True).Count
            'PluginHelper.SetLoadingStatus(String.Format(My.Resources.Language.LoadingBuildingProjectsXofY, built, toBuild.Count), built / toBuild.Count)
        End Sub
#End Region

#End Region

#Region "Create"
        ''' <summary>
        ''' Creates and returns a new solution.
        ''' </summary>
        ''' <param name="SolutionDirectory">Directory to store the solution.  Solution will be stored in a sub directory of the one given.</param>
        ''' <param name="SolutionName">Name of the solution.</param>
        ''' <returns></returns>
        Public Shared Function CreateSolution(SolutionDirectory As String, SolutionName As String, manager As PluginManager) As Solution
            Return CreateSolution(SolutionDirectory, SolutionName, GetType(Solution), manager)
        End Function

        ''' <summary>
        ''' Creates and returns a new solution.
        ''' </summary>
        ''' <param name="SolutionDirectory">Directory to store the solution.  Solution will be stored in a sub directory of the one given.</param>
        ''' <param name="SolutionName">Name of the solution.</param>
        ''' <param name="SolutionType">Type of the solution to create.  Must inherit from Solution.</param>
        ''' <returns></returns>
        Public Shared Function CreateSolution(SolutionDirectory As String, SolutionName As String, SolutionType As Type, manager As PluginManager) As Solution
            If SolutionDirectory Is Nothing Then
                Throw New ArgumentNullException(NameOf(SolutionDirectory))
            End If
            If SolutionName Is Nothing Then
                Throw New ArgumentNullException(NameOf(SolutionName))
            End If
            If SolutionType Is Nothing Then
                Throw New ArgumentNullException(NameOf(SolutionType))
            End If
            If Not ReflectionHelpers.IsOfType(SolutionType, GetType(Solution).GetTypeInfo) Then
                Throw New ArgumentException("SolutionType must inherit from Solution.", NameOf(SolutionType))
            End If

            Dim dir = Path.Combine(SolutionDirectory, SolutionName)
            If Not manager.CurrentIOProvider.DirectoryExists(dir) Then
                manager.CurrentIOProvider.CreateDirectory(dir)
            End If

            Dim output As Solution = ReflectionHelpers.CreateInstance(SolutionType.GetTypeInfo)
            output.CurrentPluginManager = manager
            output.Filename = Path.Combine(dir, SolutionName & ".skysln")
            output.Name = SolutionName
            output.Settings = New SettingsProvider
            output.UnsavedChanges = True
            output.RaiseCreated()

            Return output
        End Function
#End Region

#Region "Open"
        ''' <summary>
        ''' Opens and returns the solution at the given filename.
        ''' </summary>
        ''' <param name="Filename"></param>
        ''' <returns></returns>
        Public Shared Function OpenSolutionFile(Filename As String, manager As PluginManager) As Solution
            If Filename Is Nothing Then
                Throw New ArgumentNullException(NameOf(Filename))
            End If
            If Not manager.CurrentIOProvider.FileExists(Filename) Then
                Throw New FileNotFoundException("Could not find a file at the given filename.", Filename)
            End If
            'Open the file
            Dim solutionInfo As SolutionFile = Json.DeserializeFromFile(Of SolutionFile)(Filename, manager.CurrentIOProvider)

            'Legacy support
            If String.IsNullOrEmpty(solutionInfo.FileFormat) Then
                Dim legacy As SolutionFileLegacy = Json.DeserializeFromFile(Of SolutionFileLegacy)(Filename, manager.CurrentIOProvider)
                solutionInfo.FileFormat = "1"

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
                    solutionInfo.InternalSettings = s.Serialize
                End If

            End If

            'Get the solution type
            Dim type As TypeInfo = ReflectionHelpers.GetTypeByName(solutionInfo.AssemblyQualifiedTypeName, manager)
            If type Is Nothing Then
                'Default to a generic Solution
                type = GetType(Solution).GetTypeInfo
            End If

            Dim out As Solution = ReflectionHelpers.CreateInstance(type)
            out.Filename = Filename
            out.LoadSolutionFile(solutionInfo, manager)
            out.CurrentPluginManager = manager

            Return out
        End Function

        Private Sub LoadSolutionFile(File As SolutionFile, manager As PluginManager)
            Me.Name = File.Name

            'Load Settings
            Me.Settings = SettingsProvider.Deserialize(File.InternalSettings, manager)

            'Load Projects
            For Each item In File.Projects
                Dim projectPath = item.Key.Replace("\", "/").TrimStart("/").Split("/")
                Dim current = Me.Root
                'Create the directory nodes
                For count = 0 To projectPath.Length - 2
                    'Try to get the directory node we're expecting
                    Dim i = count 'I got a warning about using an iterator variable in the line below
                    Dim child = (From c In current.Children Where c.Name.ToLower = projectPath(i).ToLower).FirstOrDefault

                    If child Is Nothing Then
                        'Create it if it doesn't exist
                        Dim newNode As New SolutionNode(Me, current)
                        newNode.Name = projectPath(count)
                        current.Children.Add(newNode)
                        current = newNode
                    Else
                        'Otherwise select it
                        current = child
                    End If

                Next
                'Try to find the project node
                Dim proj = (From c In current.Children Where c.Name.ToLower = projectPath.Last.ToLower).FirstOrDefault
                If proj Is Nothing Then
                    'If it doesn't exist, create it
                    Dim newNode As New SolutionNode(Me, current)
                    newNode.Name = projectPath.Last
                    If item.Value IsNot Nothing Then
                        newNode.Project = Project.OpenProjectFile(Path.Combine(Path.GetDirectoryName(Filename), item.Value.Replace("/", "\").TrimStart("\")), Me, manager)
                        AddHandler newNode.Project.Modified, AddressOf Project_Modified
                    Else
                        newNode.Project = Nothing
                    End If
                    current.Children.Add(newNode)
                Else
                    'If it does exist, there's already a project with the same name.
                    'Todo: replace with better exception
                    Throw New Exception("Duplicate project detected: " & projectPath.Last & ".")
                End If
            Next

        End Sub
#End Region

#Region "Save"
        Public Sub Save(provider As IOProvider)
            Dim file As New SolutionFile
            file.AssemblyQualifiedTypeName = Me.GetType.AssemblyQualifiedName
            file.Name = Me.Name
            file.InternalSettings = Me.Settings.Serialize
            file.Projects = GetProjectDictionary(Root, "")
            Json.SerializeToFile(Filename, file, provider)
            RaiseEvent FileSaved(Me, New EventArgs)
            UnsavedChanges = False
        End Sub

        Private Function GetProjectDictionary(ProjectNode As SolutionNode, CurrentPath As String) As Dictionary(Of String, String)
            If Not ProjectNode.IsDirectory Then
                'If it's a project
                Dim out As New Dictionary(Of String, String)
                out.Add(CurrentPath, ProjectNode.Project.Filename.Replace(Path.GetDirectoryName(Filename), ""))
                Return out
            ElseIf ProjectNode.IsDirectory AndAlso ProjectNode.Children.Count = 0 AndAlso Not CurrentPath = "" Then
                'If it's a directory with no children
                Dim out As New Dictionary(Of String, String)
                out.Add(CurrentPath, Nothing)
                Return out
            Else
                'Otherwise, merge with a recursive call
                Dim out As New Dictionary(Of String, String)
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
                    If Root IsNot Nothing Then
                        Root.Dispose()
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
End Namespace

