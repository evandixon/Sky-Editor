Imports System.ComponentModel
Imports System.Threading.Tasks
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SkyEditorBase.PluginHelper

Public Class Project
    Implements IDisposable
    Implements iSavable
    Implements iModifiable
    Implements INotifyPropertyChanged
    Implements iNamed

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

    Public Class ProjectItem
        Implements IDisposable
        Implements IComparable(Of ProjectItem)

        Dim _p As Project
        Public Property IsDirectory As Boolean
        Public Property Name As String
        Public Property Filename As String
        Public Property Children As List(Of ProjectItem)
        Public Property AssemblyQualifiedTypeName As String

        Dim _file As Object
        Public Function GetFile() As Object
            If _file Is Nothing Then
                Dim f = GetFilename()

                If IO.File.Exists(f) Then
                    If String.IsNullOrEmpty(AssemblyQualifiedTypeName) Then
                        _file = PluginManager.GetInstance.OpenObject(f)
                    Else
                        Dim t = Utilities.ReflectionHelpers.GetTypeFromName(AssemblyQualifiedTypeName)
                        If t Is Nothing Then
                            _file = PluginManager.GetInstance.OpenObject(f)
                        Else
                            _file = PluginManager.GetInstance.OpenFile(f, t)
                        End If
                    End If
                End If
            End If

            Return _file
        End Function

        Public Function GetFilename() As String
            Return IO.Path.Combine(IO.Path.GetDirectoryName(_p.Filename), Filename?.TrimStart("\"))
        End Function

        Public Function CompareTo(other As ProjectItem) As Integer Implements IComparable(Of ProjectItem).CompareTo
            Return Me.Name.CompareTo(other.Name)
        End Function

        Public Sub New(Project As Project)
            Children = New List(Of ProjectItem)
            _p = Project
        End Sub

        Public Sub New(Project As Project, File As Object)
            Me.New(Project)
            _file = File
            IsDirectory = False
            AssemblyQualifiedTypeName = File.GetType.AssemblyQualifiedName
        End Sub

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
                    If _file IsNot Nothing AndAlso TypeOf _file Is IDisposable Then
                        DirectCast(_file, IDisposable).Dispose()
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
#End Region

#Region "Properties"
    Private Property ProjectNode As ProjectItem

    ''' <summary>
    ''' The name of the solution.
    ''' </summary>
    ''' <returns></returns>
    Public Property Name As String Implements iNamed.Name

    ''' <summary>
    ''' The filename of the solution file.
    ''' </summary>
    ''' <returns></returns>
    Public Property Filename As String

    Protected Property Settings As Dictionary(Of String, Object)

    Protected Property Setting(SettingName As String) As Object
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

    ''' <summary>
    ''' List of all projects in the current solution this project references.
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
            RaiseEvent BuildStatusChanged(Me, New EventArguments.ProjectBuildStatusChanged With {.Progress = BuildProgress, .StatusMessage = BuildStatusMessage})
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
            RaiseEvent BuildStatusChanged(Me, New EventArguments.ProjectBuildStatusChanged With {.Progress = BuildProgress, .StatusMessage = BuildStatusMessage})
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BuildStatusMessage)))
        End Set
    End Property

    Private _buildStatusMessage As String
#End Region

#Region "Events"
    Public Event DirectoryCreated(sender As Object, e As EventArguments.DirectoryCreatedEventArgs)
    Public Event DirectoryDeleted(sender As Object, e As EventArguments.DirectoryDeletedEventArgs)
    Public Event FileAdded(sender As Object, e As EventArguments.ProjectFileAddedEventArgs)
    Public Event FileRemoved(sender As Object, e As EventArguments.ProjectFileRemovedEventArgs)
    Public Event FileSaved(sender As Object, e As EventArgs) Implements iSavable.FileSaved
    Public Event BuildStatusChanged(sender As Object, e As EventArguments.ProjectBuildStatusChanged)
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    ''' <summary>
    ''' Raised when the project has been opened.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Public Event ProjectOpened(sender As Object, e As EventArgs)
    Public Event Modified(sender As Object, e As EventArgs) Implements iModifiable.Modified
#End Region

#Region "Create New"
    ''' <summary>
    ''' Creates and returns a new Project.
    ''' </summary>
    ''' <param name="ProjectDirectory">Directory to store the Project.  Project will be stored in a sub directory of the one given.</param>
    ''' <param name="ProjectName">Name of the Project.</param>
    ''' <returns></returns>
    Public Shared Function CreateProject(ProjectDirectory As String, ProjectName As String) As Project
        Return CreateProject(ProjectDirectory, ProjectName, GetType(Project))
    End Function

    ''' <summary>
    ''' Creates and returns a new Project.
    ''' </summary>
    ''' <param name="ProjectDirectory">Directory to store the Project.  Project will be stored in a sub directory of the one given.</param>
    ''' <param name="ProjectName">Name of the Project.</param>
    ''' <param name="ProjectType">Type of the Project to create.  Must inherit from Project.</param>
    ''' <returns></returns>
    Public Shared Function CreateProject(ProjectDirectory As String, ProjectName As String, ProjectType As Type) As Project
        If ProjectDirectory Is Nothing Then
            Throw New ArgumentNullException(NameOf(ProjectDirectory))
        End If
        If ProjectName Is Nothing Then
            Throw New ArgumentNullException(NameOf(ProjectName))
        End If
        If ProjectType Is Nothing Then
            Throw New ArgumentNullException(NameOf(ProjectType))
        End If
        If Not Utilities.ReflectionHelpers.IsOfType(ProjectType, GetType(Project)) Then
            Throw New ArgumentException("ProjectType must inherit from Project.", NameOf(ProjectType))
        End If

        Dim dir = IO.Path.Combine(ProjectDirectory, ProjectName)
        If Not IO.Directory.Exists(dir) Then
            IO.Directory.CreateDirectory(dir)
        End If

        Dim output As Project = ProjectType.GetConstructor({}).Invoke({})
        output.Filename = IO.Path.Combine(dir, ProjectName & ".skyproj")
        output.Name = ProjectName

        Dim projFile As New ProjectFile With {.Name = ProjectName, .AssemblyQualifiedTypeName = ProjectType.AssemblyQualifiedName}
        output.LoadProjectFile(projFile)

        Return output
    End Function
#End Region


    ''' <summary>
    ''' Gets the solution items at the given logical path in the solution.
    ''' </summary>
    ''' <param name="Path">Logical path to get the contents for.  Pass in String.Empty or Nothing to get the root.</param>
    ''' <returns></returns>
    Public Function GetDirectoryContents(Path As String) As IEnumerable(Of ProjectItem)
        If Path Is Nothing OrElse Path = String.Empty Then
            Return ProjectNode.Children
        Else
            Dim pathArray = Path.Replace("\", "/").Split("/")

            Dim current As ProjectItem = ProjectNode
            Dim index As Integer = 0
            For count = 0 To pathArray.Length - 1
                current = (From i In current.Children Where i.Name.ToLower = pathArray(index).ToLower Select i).FirstOrDefault
                If current Is Nothing Then
                    Throw New IO.DirectoryNotFoundException("The given path does not exist in the project.")
                End If
            Next
            current.Children.Sort()
            Return current.Children
        End If
    End Function

    ''' <summary>
    ''' Gets the project item at the given path.
    ''' Returns Nothing if there is no project item at that path.
    ''' </summary>
    ''' <param name="ItemPath">Path to look for a project item.</param>
    ''' <returns></returns>
    Public Function GetProjectItemByPath(ItemPath As String) As ProjectItem
        If ItemPath Is Nothing OrElse ItemPath = "" Then
            Return ProjectNode
        Else
            Dim path = ItemPath.Replace("\", "/").TrimStart("/").Split("/")
            Dim current = Me.ProjectNode
            For count = 0 To path.Length - 2
                Dim i = count 'I got a warning about using an iterator variable in the line below
                Dim child = (From c In current.Children Where c.Name.ToLower = path(i).ToLower).FirstOrDefault

                If child Is Nothing Then
                    Dim newNode As New ProjectItem(Me)
                    newNode.IsDirectory = True
                    newNode.Name = path(count)
                    current.Children.Add(newNode)
                    current = newNode
                Else
                    current = child
                End If

            Next
            Dim proj As ProjectItem = (From c In current.Children Where c.Name.ToLower = path.Last.ToLower).FirstOrDefault
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
    Public Function GetProjectByPath(Path As String) As Object
        Return GetProjectItemByPath(Path)?.GetFile
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
        Dim item = GetProjectItemByPath(Path)
        If item Is Nothing Then
            'Throw New IO.DirectoryNotFoundException("Cannot create a solution directory at the given path: " & Path)
            Dim pathParts = Path.Replace("\", "/").TrimStart("/").Split("/")
            Dim parentPath As New Text.StringBuilder
            For count = 0 To pathParts.Length - 2
                parentPath.Append(pathParts(count))
                parentPath.Append("/")
            Next
            Dim parentPathString = parentPath.ToString.TrimEnd("/")
            CreateDirectory(parentPathString, pathParts.Last)
            item = GetProjectItemByPath(Path)
        End If
        Dim q = (From c In item.Children Where c.Name.ToLower = DirectoryName.ToLower AndAlso c.IsDirectory = True).FirstOrDefault
        If q Is Nothing Then
            item.Children.Add(New ProjectItem(Me) With {.IsDirectory = True, .Name = DirectoryName})
            RaiseEvent DirectoryCreated(Me, New EventArguments.DirectoryCreatedEventArgs With {.DirectoryName = DirectoryName, .ParentPath = Path, .FullPath = Path & "/" & DirectoryName})
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
        Dim pathParts = FullPath.Replace("\", "/").TrimStart("/").Split("/")
        Dim parentPath As New Text.StringBuilder
        For count = 0 To pathParts.Length - 2
            parentPath.Append(pathParts(count))
            parentPath.Append("/")
        Next
        Dim parentPathString = parentPath.ToString.TrimEnd("/")
        CreateDirectory(parentPathString, pathParts.Last)
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
        Dim child = (From c In parent.Children Where c.Name.ToLower = pathParts.Last.ToLower AndAlso c.IsDirectory = True).FirstOrDefault
        If child IsNot Nothing Then
            parent.Children.Remove(child)
            child.Dispose()
            RaiseEvent DirectoryDeleted(Me, New EventArguments.DirectoryDeletedEventArgs With {.DirectoryName = pathParts.Last, .ParentPath = parentPathString, .FullPath = Path})
        End If
    End Sub

    Public Overridable Function CanCreateFile(Path As String) As Boolean
        Return CanCreateDirectory(Path)
    End Function

    Public Overridable Function CanAddExistingFile(Path As String) As Boolean
        Return CanCreateFile(Path)
    End Function

    Public Overridable Function GetSupportedFileTypes(Path As String) As IEnumerable(Of Type)
        If CanCreateDirectory(Path) Then
            Return PluginManager.GetInstance.GetCreatableFiles
        Else
            Return {}
        End If
    End Function

    Public Overridable Sub CreateFile(ParentPath As String, FileName As String, FileType As Type)
        Dim item = GetProjectItemByPath(ParentPath)
        If item IsNot Nothing Then
            FileName = FileName.Replace("\", "/").TrimStart("/")
            Dim q = (From c In item.Children Where c.Name.ToLower = FileName.ToLower AndAlso c.IsDirectory = False).FirstOrDefault
            If q Is Nothing Then
                Dim fileObj As Interfaces.iCreatableFile = FileType.GetConstructor({}).Invoke({})
                fileObj.CreateFile(FileName)
                fileObj.Filename = IO.Path.Combine(IO.Path.GetDirectoryName(Me.Filename), ParentPath.Replace("/", "\").TrimStart("\"), FileName)

                Dim projItem As New ProjectItem(Me, fileObj)
                projItem.Filename = ParentPath & "/" & FileName
                projItem.Name = FileName
                item.Children.Add(projItem)
                RaiseEvent FileAdded(Me, New EventArguments.ProjectFileAddedEventArgs With {.ParentPath = ParentPath, .File = fileObj, .Filename = FileName, .FullFilename = fileObj.Filename})
            Else
                'There's already a project here
                'Throw New ProjectAlreadyExistsException("A project with the name """ & ProjectName & """ already exists in the given path: " & ParentPath)
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
        Return IO.Path.Combine(ParentProjectPath, IO.Path.GetFileName(FullFilename))
    End Function

    Public Overridable Function GetImportIOFilter(ParentProjectPath As String) As String
        Return PluginManager.GetInstance.IOFiltersString
    End Function

    Public Overridable Async Function AddExistingFile(ParentProjectPath As String, FilePath As String, Optional ShowLoadingStatus As Boolean = True) As Task
        Dim item = GetProjectItemByPath(ParentProjectPath)
        Dim filename = IO.Path.GetFileName(FilePath)
        If item IsNot Nothing Then
            filename = filename.Replace("\", "/").TrimStart("/")
            Dim q = (From c In item.Children Where c.Name.ToLower = filename.ToLower AndAlso c.IsDirectory = False).FirstOrDefault
            If q Is Nothing Then
                Dim projItem As New ProjectItem(Me)
                projItem.Filename = GetImportedFilePath(ParentProjectPath, FilePath)

                If ShowLoadingStatus Then PluginHelper.SetLoadingStatus(My.Resources.Language.LoadingCopyingFile)

                Dim source = FilePath
                Dim dest = IO.Path.Combine(IO.Path.GetDirectoryName(Me.Filename), projItem.Filename.Replace("/", "\").TrimStart("\"))

                Await Task.Run(New Action(Sub()
                                              If Not source.Replace("\", "/").ToLower = dest.Replace("\", "/").ToLower Then
                                                  IO.File.Copy(FilePath, dest, True)
                                              End If
                                          End Sub))

                If ShowLoadingStatus Then PluginHelper.SetLoadingStatusFinished()

                projItem.Name = IO.Path.GetFileName(projItem.Filename)
                item.Children.Add(projItem)
                RaiseEvent FileAdded(Me, New EventArguments.ProjectFileAddedEventArgs With {.ParentPath = ParentProjectPath, .Filename = projItem.Name, .FullFilename = dest})
            Else
                'There's already a project here
                'Todo: throw exception
                'Throw New ProjectAlreadyExistsException("A project with the name """ & ProjectName & """ already exists in the given path: " & ParentPath)
            End If
        Else
            Throw New IO.DirectoryNotFoundException(String.Format(My.Resources.Language.ErrorCantAddFile, ParentProjectPath))
        End If
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
        Dim child = (From c In parent.Children Where c.Name.ToLower = pathParts.Last.ToLower AndAlso c.IsDirectory = False).FirstOrDefault
        If child IsNot Nothing Then
            parent.Children.Remove(child)
            child.Dispose()
            RaiseEvent FileRemoved(Me, New EventArguments.ProjectFileRemovedEventArgs With {.FileName = pathParts.Last, .ParentPath = parentPathString, .FullPath = FilePath})
        End If
    End Sub

    Public Overridable Function Build(Solution As Solution) As Task
        Return Task.CompletedTask
    End Function

    Public Overridable Function CanBuild(Solution As Solution) As Boolean
        Return False
    End Function

    Public Overridable Function GetRootDirectory() As String
        Return IO.Path.GetDirectoryName(Me.Filename)
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

#Region "Open"
    ''' <summary>
    ''' Opens and returns the solution at the given filename.
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <returns></returns>
    Public Shared Function OpenProjectFile(Filename As String) As Project
        If Filename Is Nothing Then
            Throw New ArgumentNullException(NameOf(Filename))
        End If
        If Not IO.File.Exists(Filename) Then
            Throw New IO.FileNotFoundException("Could not find a file at the given filename.", Filename)
        End If
        Dim solutionInfo As ProjectFile = Utilities.Json.DeserializeFromFile(Of ProjectFile)(Filename)
        Dim type As Type = Utilities.ReflectionHelpers.GetTypeFromName(solutionInfo.AssemblyQualifiedTypeName)
        If type Is Nothing Then
            PluginHelper.Writeline($"Could not find project type ""{solutionInfo.AssemblyQualifiedTypeName}"".  Substituting a generic project.", LineType.Error)
            type = GetType(Project)
        End If
        Dim out As Project = type.GetConstructor({}).Invoke({})
        out.Filename = Filename
        out.LoadProjectFile(solutionInfo)

        Return out
    End Function

    Private Sub LoadProjectFile(File As ProjectFile)
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

        'Load Files
        For Each item In File.Files
            Dim path = item.Key.Replace("\", "/").TrimStart("/").Split("/")
            Dim current = Me.ProjectNode
            For count = 0 To path.Length - 2
                Dim i = count 'I got a warning about using an iterator variable in the line below
                Dim child = (From c In current.Children Where c.Name.ToLower = path(i).ToLower).FirstOrDefault

                If child Is Nothing Then
                    Dim newNode As New ProjectItem(Me)
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
                Dim newNode As New ProjectItem(Me)
                newNode.Name = path.Last
                If item.Value IsNot Nothing Then
                    newNode.IsDirectory = False
                    newNode.Filename = item.Value.Filename 'IO.Path.Combine(IO.Path.GetDirectoryName(Me.Filename), item.Value.Filename.Replace("/", "\").TrimStart("\"))
                    newNode.AssemblyQualifiedTypeName = item.Value.AssemblyQualifiedTypeName
                Else
                    newNode.IsDirectory = True
                    newNode.Filename = Nothing
                End If
                current.Children.Add(newNode)
            Else
                'There's already a project with the same name.
                PluginHelper.Writeline("Duplicate project detected: " & path.Last & ".  Not loading it.")
            End If
        Next

        RaiseEvent ProjectOpened(Me, New EventArgs)
    End Sub
#End Region

#Region "Save"
    Public Sub Save() Implements iSavable.Save
        Dim file As New ProjectFile
        file.AssemblyQualifiedTypeName = Me.GetType.AssemblyQualifiedName
        file.Name = Me.Name
        For Each item In Me.Settings
            Dim value As New SettingValue
            value.AssemblyQualifiedTypeName = item.Value.GetType.AssemblyQualifiedName
            value.ValueJson = Utilities.Json.Serialize(item.Value)
            file.Settings.Add(item.Key, value)
        Next
        file.Files = GetProjectDictionary(ProjectNode, "")
        Utilities.Json.SerializeToFile(Filename, file)
        RaiseEvent FileSaved(Me, New EventArgs)
    End Sub

    Private Function GetProjectDictionary(ProjectNode As ProjectItem, CurrentPath As String) As Dictionary(Of String, FileValue)
        If Not ProjectNode.IsDirectory Then
            Dim out As New Dictionary(Of String, FileValue)
            out.Add(CurrentPath, New FileValue With {.Filename = ProjectNode.Filename.Replace(IO.Path.GetDirectoryName(Filename), ""), .AssemblyQualifiedTypeName = ProjectNode.AssemblyQualifiedTypeName})
            Return out
        ElseIf ProjectNode.IsDirectory AndAlso ProjectNode.Children.Count = 0 AndAlso Not CurrentPath = "" Then
            Dim out As New Dictionary(Of String, FileValue)
            out.Add(CurrentPath, Nothing)
            Return out
        Else
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

    Public Function DefaultExtension() As String ' Implements iSavableAs.DefaultExtension
        Return ".skyproj"
    End Function

    Public Sub RaiseModified() Implements iModifiable.RaiseModified
        RaiseEvent Modified(Me, New EventArgs)
    End Sub

    Public Sub New()
        Settings = New Dictionary(Of String, Object)
        ProjectNode = New ProjectItem(Me) With {.Name = "Project", .IsDirectory = True}
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                ProjectNode.Dispose()
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
