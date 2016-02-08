Imports System.Threading.Tasks
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SkyEditorBase.PluginHelper

Public Class Solution
    Implements IDisposable
    Implements iSavable

#Region "Child Classes"
    Private Class SettingValue
        Public Property AssemblyQualifiedTypeName As String
        Public Property ValueJson As String
    End Class
    Private Class SolutionFile
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

    Public Class SolutionItem
        Implements IDisposable
        Implements IComparable(Of SolutionItem)
        Public Property IsDirectory As Boolean
        Public Property Name As String
        Public Property Project As Project
        Public Property Children As List(Of SolutionItem)
        Public Sub New()
            Children = New List(Of SolutionItem)
        End Sub

        Public Function CompareTo(other As SolutionItem) As Integer Implements IComparable(Of SolutionItem).CompareTo
            Return Me.Name.CompareTo(other.Name)
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    For Each item In Children
                        item.Dispose()
                    Next
                    If Project IsNot Nothing Then
                        Project.Dispose()
                    End If
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
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

#Region "Properties"
    Private Property SolutionNode As SolutionItem

    ''' <summary>
    ''' The name of the solution.
    ''' </summary>
    ''' <returns></returns>
    Public Property Name As String

    ''' <summary>
    ''' The filename of the solution file.
    ''' </summary>
    ''' <returns></returns>
    Public Property Filename As String

    Public Property Settings As Dictionary(Of String, Object)

    Public Property IsModified As Boolean

    Public Property Setting(SettingName As String) As Object
        Get
            If Settings.ContainsKey(SettingName) Then
                Return Settings(SettingName)
            Else
                Return Nothing
            End If
        End Get
        Set(value As Object)
            If Settings.ContainsKey(SettingName) Then
                Settings(SettingName) = value
            Else
                Settings.Add(SettingName, value)
            End If
        End Set
    End Property
#End Region

#Region "Events"
    Public Event DirectoryCreated(sender As Object, e As EventArguments.DirectoryCreatedEventArgs)
    Public Event DirectoryDeleted(sender As Object, e As EventArguments.DirectoryDeletedEventArgs)
    Public Event ProjectAdded(sender As Object, e As EventArguments.ProjectAddedEventArgs)
    Public Event ProjectRemoving(sender As Object, e As EventArguments.ProjectRemovingEventArgs)
    Public Event ProjectRemoved(sender As Object, e As EventArguments.ProjectRemovedEventArgs)

    ''' <summary>
    ''' Raised when the solution has been created.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Public Event Created(sender As Object, e As EventArgs)
    Public Event FileSaved(sender As Object, e As EventArgs) Implements iSavable.FileSaved
    Public Event SolutionBuildStarted(sender As Object, e As EventArgs)
    Public Event SolutionBuildCompleted(sender As Object, e As EventArgs)
#End Region

    Private Sub RaiseCreated()
        RaiseEvent Created(Me, New EventArgs)
    End Sub

    ''' <summary>
    ''' Gets the solution items at the given logical path in the solution.
    ''' </summary>
    ''' <param name="Path">Logical path to get the contents for.  Pass in String.Empty or Nothing to get the root.</param>
    ''' <returns></returns>
    Public Function GetDirectoryContents(Path As String) As IEnumerable(Of SolutionItem)
        If Path Is Nothing OrElse Path = String.Empty Then
            Return SolutionNode.Children
        Else
            Dim pathArray = Path.Replace("\", "/").Split("/")

            Dim current As SolutionItem = SolutionNode
            Dim index As Integer = 0
            For count = 0 To pathArray.Length - 1
                current = (From i In current.Children Where i.Name.ToLower = pathArray(index).ToLower Select i).FirstOrDefault
                If current Is Nothing Then
                    Throw New IO.DirectoryNotFoundException("The given path does not exist in the solution.")
                End If
            Next
            current.Children.Sort()
            Return current.Children
        End If
    End Function

    ''' <summary>
    ''' Gets the solution item at the given path.
    ''' Returns Nothing if there is no solution item at that path.
    ''' </summary>
    ''' <param name="ItemPath">Path to look for a solution item.</param>
    ''' <returns></returns>
    Public Function GetSolutionItemByPath(ItemPath As String) As SolutionItem
        If ItemPath Is Nothing OrElse ItemPath = "" Then
            Return SolutionNode
        Else
            Dim path = ItemPath.Replace("\", "/").TrimStart("/").Split("/")
            Dim current = Me.SolutionNode
            For count = 0 To path.Length - 2
                Dim i = count 'I got a warning about using an iterator variable in the line below
                Dim child = (From c In current.Children Where c.Name.ToLower = path(i).ToLower).FirstOrDefault

                If child Is Nothing Then
                    Dim newNode As New SolutionItem
                    newNode.IsDirectory = True
                    newNode.Name = path(count)
                    current.Children.Add(newNode)
                    current = newNode
                Else
                    current = child
                End If

            Next
            Dim proj As SolutionItem = (From c In current.Children Where c.Name.ToLower = path.Last.ToLower).FirstOrDefault
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
    ''' Returns whether or not a directory can be made inside the given path.
    ''' </summary>
    ''' <param name="Path">Path to put the directory.</param>
    ''' <returns></returns>
    Public Overridable Function CanCreateDirectory(Path As String) As Boolean
        Return (GetSolutionItemByPath(Path) IsNot Nothing)
    End Function

    ''' <summary>
    ''' Creates a directory at the given location if it does not exist.
    ''' </summary>
    ''' <param name="Path">Path to put the new directory in.</param>
    ''' <param name="DirectoryName">Name of the new directory.</param>
    Public Overridable Sub CreateDirectory(Path As String, DirectoryName As String)
        Dim item = GetSolutionItemByPath(Path)
        If item IsNot Nothing Then
            Dim q = (From c In item.Children Where c.Name.ToLower = DirectoryName.ToLower AndAlso c.IsDirectory = True).FirstOrDefault
            If q Is Nothing Then
                item.Children.Add(New SolutionItem With {.IsDirectory = True, .Name = DirectoryName})
                RaiseEvent DirectoryCreated(Me, New EventArguments.DirectoryCreatedEventArgs With {.DirectoryName = DirectoryName, .ParentPath = Path, .FullPath = Path & "/" & DirectoryName})
            Else
                'There's already a directory here.
                'Do nothing.
            End If
        Else
            Throw New IO.DirectoryNotFoundException("Cannot create a solution directory at the given path: " & Path)
        End If
    End Sub

    ''' <summary>
    ''' Returns whether or not the directory at the given path can be deleted.
    ''' </summary>
    ''' <param name="Path"></param>
    ''' <returns></returns>
    Public Overridable Function CanDeleteDirectory(Path As String) As Boolean
        Return (GetSolutionItemByPath(Path) IsNot Nothing)
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
        Dim parent = GetSolutionItemByPath(parentPathString)
        Dim child = (From c In parent.Children Where c.Name.ToLower = pathParts.Last.ToLower AndAlso c.IsDirectory = True).FirstOrDefault
        If child IsNot Nothing Then
            parent.Children.Remove(child)
            child.Dispose()
            RaiseEvent DirectoryDeleted(Me, New EventArguments.DirectoryDeletedEventArgs With {.DirectoryName = pathParts.Last, .ParentPath = parentPathString, .FullPath = Path})
        End If
    End Sub

    Public Overridable Function CanCreateProject(Path As String) As Boolean
        Return CanCreateDirectory(Path)
    End Function

    Public Overridable Function GetSupportedProjectTypes(Path As String) As IEnumerable(Of Type)
        If CanCreateDirectory(Path) Then
            Return PluginManager.GetInstance.GetRegisteredTypes(GetType(Project))
        Else
            Return {}
        End If
    End Function

    Public Overridable Sub CreateProject(ParentPath As String, ProjectName As String, ProjectType As Type)
        Dim item = GetSolutionItemByPath(ParentPath)
        If item IsNot Nothing Then
            Dim q = (From c In item.Children Where c.Name.ToLower = ProjectName.ToLower AndAlso c.IsDirectory = False).FirstOrDefault
            If q Is Nothing Then
                Dim p = Project.CreateProject(IO.Path.GetDirectoryName(Me.Filename), ProjectName, ProjectType)
                item.Children.Add(New SolutionItem With {.IsDirectory = False, .Name = ProjectName, .Project = p})
                RaiseEvent ProjectAdded(Me, New EventArguments.ProjectAddedEventArgs With {.ParentPath = ParentPath, .Project = p})
            Else
                'There's already a project here
                Throw New ProjectAlreadyExistsException("A project with the name """ & ProjectName & """ already exists in the given path: " & ParentPath)
            End If
        Else
            Throw New IO.DirectoryNotFoundException("Cannot create a project at the given path: " & ParentPath)
        End If
    End Sub

    Public Overridable Function CanDeleteProject(ProjectPath As String) As Boolean
        Return (GetSolutionItemByPath(ProjectPath) IsNot Nothing)
    End Function

    Public Overridable Sub DeleteProject(ProjectPath As String)
        Dim pathParts = ProjectPath.Replace("\", "/").TrimStart("/").Split("/")
        Dim parentPath As New Text.StringBuilder
        For count = 0 To pathParts.Length - 2
            parentPath.Append(pathParts(count))
            parentPath.Append("/")
        Next
        Dim parentPathString = parentPath.ToString.TrimEnd("/")
        Dim parent = GetSolutionItemByPath(parentPathString)
        Dim child = (From c In parent.Children Where c.Name.ToLower = pathParts.Last.ToLower AndAlso c.IsDirectory = False).FirstOrDefault
        If child IsNot Nothing Then
            RaiseEvent ProjectRemoving(Me, New EventArguments.ProjectRemovingEventArgs With {.Project = child.Project})
            parent.Children.Remove(child)
            child.Dispose()
            RaiseEvent ProjectRemoved(Me, New EventArguments.ProjectRemovedEventArgs With {.DirectoryName = pathParts.Last, .ParentPath = parentPathString, .FullPath = ProjectPath})
        End If
    End Sub

    Public Function GetAllProjects() As IEnumerable(Of Project)
        Return GetAllProjects(SolutionNode)
    End Function

    Public Overridable Function GetAllProjects(Root As SolutionItem) As IEnumerable(Of Project)
        Dim output As New List(Of Project)
        If Root.Children.Count > 0 Then
            For Each item In Root.Children
                output.AddRange(GetAllProjects(item))
            Next
        End If

        If Root.IsDirectory = False AndAlso Root.Project IsNot Nothing Then
            output.Add(Root.Project)
        End If

        Return output
    End Function

    Public Overridable Sub SaveAllProjects()
        For Each item In GetAllProjects()
            item.Save()
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

    Public Overridable Function GetProjectsToBuild() As IEnumerable(Of Project)
        Return From p In Me.GetAllProjects Where p.CanBuild(Me)
    End Function

    Public Overridable Async Function Build() As Task
        RaiseEvent SolutionBuildStarted(Me, New EventArgs)
        Dim toBuild As New Dictionary(Of Project, Boolean)
        PluginHelper.SetLoadingStatus(PluginHelper.GetLanguageItem("Building projects..."))

        For Each item In GetProjectsToBuild()
            If Not item.HasCircularReferences(Me) Then
                toBuild.Add(item, False)
            Else
                PluginHelper.Writeline("Project " & item.Name & " has a circular reference.  Skipping its compilation.", LineType.Error)
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

        PluginHelper.SetLoadingStatusFinished()
        RaiseEvent SolutionBuildCompleted(Me, New EventArgs)
    End Function

    Private Async Function BuildProjects(ToBuild As Dictionary(Of Project, Boolean), CurrentProject As Project) As Task
        Dim buildTasks As New List(Of Task)
        For Each item In From p In CurrentProject.GetReferences(Me) Where p.CanBuild(Me)
            buildTasks.Add(BuildProjects(ToBuild, item))
        Next
        Await Task.WhenAll(buildTasks)

        If Not ToBuild(CurrentProject) Then
            'Todo: make sure we won't get here twice, with all the async stuff going on
            ToBuild(CurrentProject) = True
            UpdateBuildLoadingStatus(ToBuild)
            Await CurrentProject.Build(Me)
        End If
    End Function

    Private Sub UpdateBuildLoadingStatus(toBuild As Dictionary(Of Project, Boolean))
        Dim built As Integer = (From v In toBuild.Values Where v = True).Count
        PluginHelper.SetLoadingStatus(String.Format(PluginHelper.GetLanguageItem("Building projects... ({0} of {1})"), built, toBuild.Count), built / toBuild.Count)
    End Sub


#Region "Create New"
    ''' <summary>
    ''' Creates and returns a new solution.
    ''' </summary>
    ''' <param name="SolutionDirectory">Directory to store the solution.  Solution will be stored in a sub directory of the one given.</param>
    ''' <param name="SolutionName">Name of the solution.</param>
    ''' <returns></returns>
    Public Shared Function CreateSolution(SolutionDirectory As String, SolutionName As String) As Solution
        Return CreateSolution(SolutionDirectory, SolutionName, GetType(Solution))
    End Function

    ''' <summary>
    ''' Creates and returns a new solution.
    ''' </summary>
    ''' <param name="SolutionDirectory">Directory to store the solution.  Solution will be stored in a sub directory of the one given.</param>
    ''' <param name="SolutionName">Name of the solution.</param>
    ''' <param name="SolutionType">Type of the solution to create.  Must inherit from Solution.</param>
    ''' <returns></returns>
    Public Shared Function CreateSolution(SolutionDirectory As String, SolutionName As String, SolutionType As Type) As Solution
        If SolutionDirectory Is Nothing Then
            Throw New ArgumentNullException(NameOf(SolutionDirectory))
        End If
        If SolutionName Is Nothing Then
            Throw New ArgumentNullException(NameOf(SolutionName))
        End If
        If SolutionType Is Nothing Then
            Throw New ArgumentNullException(NameOf(SolutionType))
        End If
        If Not Utilities.ReflectionHelpers.IsOfType(SolutionType, GetType(Solution)) Then
            Throw New ArgumentException("SolutionType must inherit from Solution.", NameOf(SolutionType))
        End If

        Dim dir = IO.Path.Combine(SolutionDirectory, SolutionName)
        If Not IO.Directory.Exists(dir) Then
            IO.Directory.CreateDirectory(dir)
        End If

        Dim output As Solution = SolutionType.GetConstructor({}).Invoke({})
        output.Filename = IO.Path.Combine(dir, SolutionName & ".skysln")
        output.Name = SolutionName

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
    Public Shared Function OpenSolutionFile(Filename As String) As Solution
        If Filename Is Nothing Then
            Throw New ArgumentNullException(NameOf(Filename))
        End If
        If Not IO.File.Exists(Filename) Then
            Throw New IO.FileNotFoundException("Could not find a file at the given filename.", Filename)
        End If
        Dim solutionInfo As SolutionFile = Utilities.Json.DeserializeFromFile(Of SolutionFile)(Filename)
        Dim type As Type = Utilities.ReflectionHelpers.GetTypeFromName(solutionInfo.AssemblyQualifiedTypeName)
        If type Is Nothing Then
            PluginHelper.Writeline($"Could not find solution type ""{solutionInfo.AssemblyQualifiedTypeName}"".  Substituting a generic solution.", LineType.Error)
            type = GetType(Solution)
        End If
        Dim out As Solution = type.GetConstructor({}).Invoke({})
        out.Filename = Filename
        out.LoadSolutionFile(solutionInfo)

        Return out
    End Function

    Private Sub LoadSolutionFile(File As SolutionFile)
        Me.Name = File.Name

        'Load Settings
        For Each item In File.Settings
            If Not Settings.ContainsKey(item.Key) Then
                Dim type = Utilities.ReflectionHelpers.GetTypeFromName(item.Value.AssemblyQualifiedTypeName)
                If type Is Nothing Then
                    PluginHelper.Writeline($"Could not find setting type ""{item.Value.AssemblyQualifiedTypeName}"".  Treating setting as an Object.")
                    type = GetType(Object)
                End If
                Settings.Add(item.Key, Utilities.Json.Deserialize(type, item.Value.ValueJson))
            End If
        Next

        'Load Projects
        For Each item In File.Projects
            Dim path = item.Key.Replace("\", "/").TrimStart("/").Split("/")
            Dim current = Me.SolutionNode
            For count = 0 To path.Length - 2
                Dim i = count 'I got a warning about using an iterator variable in the line below
                Dim child = (From c In current.Children Where c.Name.ToLower = path(i).ToLower).FirstOrDefault

                If child Is Nothing Then
                    Dim newNode As New SolutionItem
                    newNode.IsDirectory = True
                    newNode.Name = path(count)
                    current.Children.Add(newNode)
                    current = newNode
                Else
                    current = child
                End If

            Next
            Dim proj = (From c In current.Children Where c.Name.ToLower = path.Last.ToLower).FirstOrDefault
            If proj Is Nothing Then
                Dim newNode As New SolutionItem
                newNode.Name = path.Last
                If item.Value IsNot Nothing Then
                    newNode.IsDirectory = False
                    newNode.Project = Project.OpenProjectFile(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), item.Value.Replace("/", "\").TrimStart("\")))
                Else
                    newNode.IsDirectory = True
                    newNode.Project = Nothing
                End If
                current.Children.Add(newNode)
            Else
                'There's already a project with the same name.
                PluginHelper.Writeline("Duplicate project detected: " & path.Last & ".  Not loading it.")
            End If
        Next

    End Sub
#End Region

#Region "Save"
    Public Sub Save() Implements iSavable.Save
        Dim file As New SolutionFile
        file.AssemblyQualifiedTypeName = Me.GetType.AssemblyQualifiedName
        file.Name = Me.Name
        For Each item In Me.Settings
            If item.Value IsNot Nothing Then
                Dim value As New SettingValue
                value.AssemblyQualifiedTypeName = item.Value.GetType.AssemblyQualifiedName
                value.ValueJson = Utilities.Json.Serialize(item.Value)
                file.Settings.Add(item.Key, value)
            End If
        Next
        file.Projects = GetProjectDictionary(SolutionNode, "")
        Utilities.Json.SerializeToFile(Filename, file)
        RaiseEvent FileSaved(Me, New EventArgs)
    End Sub

    Private Function GetProjectDictionary(ProjectNode As SolutionItem, CurrentPath As String) As Dictionary(Of String, String)
        If Not ProjectNode.IsDirectory Then
            Dim out As New Dictionary(Of String, String)
            out.Add(CurrentPath, ProjectNode.Project.Filename.Replace(IO.Path.GetDirectoryName(Filename), ""))
            Return out
        ElseIf ProjectNode.IsDirectory AndAlso ProjectNode.Children.Count = 0 AndAlso Not CurrentPath = "" Then
            Dim out As New Dictionary(Of String, String)
            out.Add(CurrentPath, Nothing)
            Return out
        Else
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

    ''' <summary>
    ''' Creates a new instance of Solution.
    ''' Note: to create a new solution file, use Solution.CreateSolutionFile instead.
    ''' </summary>
    Public Sub New()
        Settings = New Dictionary(Of String, Object)
        SolutionNode = New SolutionItem With {.Name = "Solution", .IsDirectory = True}
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                SolutionNode.Dispose()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
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

    Public Overrides Function ToString() As String
        Return PluginHelper.GetLanguageItem(Me.GetType.FullName)
    End Function

    Public Function DefaultExtension() As String Implements iSavable.DefaultExtension
        Return ".skysln"
    End Function
End Class
