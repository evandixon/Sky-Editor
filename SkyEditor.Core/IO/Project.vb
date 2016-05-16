Imports System.Reflection
Imports SkyEditor.Core.Utilities

Namespace IO
    Public Class Project
        Implements INotifyPropertyChanged
        Implements INotifyModified
        Implements IDisposable
        Implements ISavable


#Region "Child Classes"
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
            Public Property InternalSettings As String
            Public Sub New()
                Files = New Dictionary(Of String, FileValue)
            End Sub
        End Class

        Public Class ProjectNode
            Implements IDisposable
            Implements IComparable(Of ProjectNode)

            ''' <summary>
            ''' Project to which this ProjectItem belongs.
            ''' </summary>
            ''' <returns></returns>
            Private Property ParentProject As Project

            ''' <summary>
            ''' Cached instance of the file.  Null if the file has not been opened or if this is a directory.
            ''' </summary>
            ''' <returns></returns>
            Private Property File As Object

            ''' <summary>
            ''' Whether or not this node is a directory.  If False, it's a file.
            ''' </summary>
            ''' <returns></returns>
            Public Property IsDirectory As Boolean

            ''' <summary>
            ''' Name of the file or directory.
            ''' </summary>
            ''' <returns></returns>
            Public Property Name As String

            ''' <summary>
            ''' Path of the file, relative to the project directory.
            ''' </summary>
            ''' <returns></returns>
            Public Property Filename As String

            ''' <summary>
            ''' The child nodes of this node.
            ''' </summary>
            ''' <returns></returns>
            Public Property Children As List(Of ProjectNode)

            ''' <summary>
            ''' Assembly qualified name of the type of the file, if this is node is a file.
            ''' </summary>
            ''' <returns></returns>
            Public Property AssemblyQualifiedTypeName As String

            ''' <summary>
            ''' Gets the file at this node, opening it if it hasn't already been.
            ''' </summary>
            ''' <returns></returns>
            Public Async Function GetFile(manager As PluginManager) As Task(Of Object)

                If File Is Nothing Then
                    Dim f = GetFilename()
                    If String.IsNullOrEmpty(AssemblyQualifiedTypeName) Then
                        Return Await IOHelper.OpenObject(f, AddressOf IOHelper.PickFirstDuplicateMatchSelector, manager)
                    Else
                        Dim t = ReflectionHelpers.GetTypeByName(AssemblyQualifiedTypeName, manager)
                        If t Is Nothing Then
                            Return Await IOHelper.OpenObject(f, AddressOf IOHelper.PickFirstDuplicateMatchSelector, manager)
                        Else
                            Return IOHelper.OpenFile(f, t, manager)
                        End If
                    End If
                Else
                    Return File
                End If

            End Function

            Public Function GetFilename() As String
                Return Path.Combine(Path.GetDirectoryName(ParentProject.Filename), Filename?.TrimStart("\"))
            End Function

            Public Function CompareTo(other As ProjectNode) As Integer Implements IComparable(Of ProjectNode).CompareTo
                Return Me.Name.CompareTo(other.Name)
            End Function

            Public Sub New(Project As Project)
                Children = New List(Of ProjectNode)
                ParentProject = Project
            End Sub

            Public Sub New(Project As Project, File As Object)
                Me.New(Project)
                Me.File = File
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
                        If _File IsNot Nothing AndAlso TypeOf _File Is IDisposable Then
                            DirectCast(_File, IDisposable).Dispose()
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
#End Region

        Public Sub RaiseModified() Implements INotifyModified.RaiseModified
            RaiseEvent Modified(Me, New EventArgs)
        End Sub

#Region "Properties"
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
#End Region

#Region "Create New"
        ''' <summary>
        ''' Creates and returns a new Project.
        ''' </summary>
        ''' <param name="ProjectDirectory">Directory to store the Project.  Project will be stored in a sub directory of the one given.</param>
        ''' <param name="ProjectName">Name of the Project.</param>
        ''' <returns></returns>
        Public Shared Function CreateProject(ProjectDirectory As String, ProjectName As String, manager As PluginManager) As Project
            Return CreateProject(ProjectDirectory, ProjectName, GetType(Project), manager)
        End Function

        ''' <summary>
        ''' Creates and returns a new Project.
        ''' </summary>
        ''' <param name="ProjectDirectory">Directory to store the Project.  Project will be stored in a sub directory of the one given.</param>
        ''' <param name="ProjectName">Name of the Project.</param>
        ''' <param name="ProjectType">Type of the Project to create.  Must inherit from Project.</param>
        ''' <returns></returns>
        Public Shared Function CreateProject(ProjectDirectory As String, ProjectName As String, ProjectType As Type, manager As PluginManager) As Project
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

            Dim projFile As New ProjectFile With {.Name = ProjectName, .AssemblyQualifiedTypeName = ProjectType.AssemblyQualifiedName}
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
        Public Shared Function OpenProjectFile(Filename As String, manager As PluginManager) As Project
            If Filename Is Nothing Then
                Throw New ArgumentNullException(NameOf(Filename))
            End If
            If Not manager.CurrentIOProvider.FileExists(Filename) Then
                Throw New FileNotFoundException("Could not find a file at the given filename.", Filename)
            End If

            Dim projectInfo As ProjectFile = Json.DeserializeFromFile(Of ProjectFile)(Filename, manager.CurrentIOProvider)
            Dim type As TypeInfo = ReflectionHelpers.GetTypeByName(projectInfo.AssemblyQualifiedTypeName, manager)
            If type Is Nothing Then
                'Default to Project if the saved type cannot be found
                type = GetType(Project).GetTypeInfo
            End If

            Dim out As Project = ReflectionHelpers.CreateInstance(type)
            out.Filename = Filename
            out.LoadProjectFile(projectInfo, manager)

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
                        Dim newNode As New ProjectNode(Me)
                        newNode.IsDirectory = True
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
                    Dim newNode As New ProjectNode(Me)
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

