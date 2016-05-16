Imports System.Reflection
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities

Namespace IO
    Public Class Solution
        Implements INotifyModified

#Region "Child Classes"
        Private Class SolutionFile
            Public Property AssemblyQualifiedTypeName As String
            Public Property Name As String
            Public Property InternalSettings As String 'Serialized settings provider
            Public Property Projects As Dictionary(Of String, String)
        End Class

        ''' <summary>
        ''' Models a node in the solution's logical heiarchy.
        ''' </summary>
        Public Class SolutionNode
            Implements IDisposable
            Implements IComparable(Of SolutionNode)

            Public ReadOnly Property IsDirectory As Boolean
                Get
                    Return (Project Is Nothing)
                End Get
            End Property

            Public Property Name As String
                Get
                    If IsDirectory Then
                        Return _name
                    Else
                        Return Project.Name
                    End If
                End Get
                Set(value As String)
                    If IsDirectory Then
                        Project.Name = value
                    Else
                        _name = value
                    End If
                End Set
            End Property
            Dim _name As String

            ''' <summary>
            ''' The node's contained project, if the node is not a solution directory.
            ''' </summary>
            ''' <returns></returns>
            Public Property Project As Project

            ''' <summary>
            ''' The solution-level children.
            ''' </summary>
            ''' <returns></returns>
            Public Property SolutionChildren As ObservableCollection(Of SolutionNode)

            Public Sub New()
                SolutionChildren = New ObservableCollection(Of SolutionNode)
            End Sub

            Public Function CompareTo(other As SolutionNode) As Integer Implements IComparable(Of SolutionNode).CompareTo
                Return Me.Name.CompareTo(other.Name)
            End Function

#Region "IDisposable Support"
            Private disposedValue As Boolean ' To detect redundant calls

            ' IDisposable
            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not disposedValue Then
                    If disposing Then
                        ' TODO: dispose managed state (managed objects).
                        For Each item In SolutionChildren
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
        ''' The root of the solution's logical heiarchy.
        ''' </summary>
        ''' <returns></returns>
        Public Property Root As SolutionNode

        Public Property UnsavedChanges As Boolean

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

#End Region

        Private Sub RaiseCreated()
            RaiseEvent Created(Me, New EventArgs)
        End Sub

        Public Sub RaiseModified() Implements INotifyModified.RaiseModified
            RaiseEvent Modified(Me, New EventArgs)
        End Sub

        Private Sub Project_Modified(sender As Object, e As EventArgs)
            RaiseModified()
        End Sub

#Region "Create"
        ''' <summary>
        ''' Creates and returns a new solution.
        ''' </summary>
        ''' <param name="SolutionDirectory">Directory to store the solution.  Solution will be stored in a sub directory of the one given.</param>
        ''' <param name="SolutionName">Name of the solution.</param>
        ''' <returns></returns>
        Public Shared Function CreateSolution(SolutionDirectory As String, SolutionName As String, provider As IOProvider) As Solution
            Return CreateSolution(SolutionDirectory, SolutionName, GetType(Solution), provider)
        End Function

        ''' <summary>
        ''' Creates and returns a new solution.
        ''' </summary>
        ''' <param name="SolutionDirectory">Directory to store the solution.  Solution will be stored in a sub directory of the one given.</param>
        ''' <param name="SolutionName">Name of the solution.</param>
        ''' <param name="SolutionType">Type of the solution to create.  Must inherit from Solution.</param>
        ''' <returns></returns>
        Public Shared Function CreateSolution(SolutionDirectory As String, SolutionName As String, SolutionType As Type, provider As IOProvider) As Solution
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
            If Not provider.DirectoryExists(dir) Then
                provider.CreateDirectory(dir)
            End If

            Dim output As Solution = ReflectionHelpers.CreateInstance(SolutionType.GetTypeInfo)
            output.Filename = Path.Combine(dir, SolutionName & ".skysln")
            output.Name = SolutionName
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
            'Get the solution type
            Dim type As TypeInfo = ReflectionHelpers.GetTypeByName(solutionInfo.AssemblyQualifiedTypeName, manager)
            If type Is Nothing Then
                'Default to a generic Solution
                type = GetType(Solution).GetTypeInfo
            End If

            Dim out As Solution = ReflectionHelpers.CreateInstance(type)
            out.Filename = Filename
            out.LoadSolutionFile(solutionInfo, manager)

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
                    Dim child = (From c In current.SolutionChildren Where c.Name.ToLower = projectPath(i).ToLower).FirstOrDefault

                    If child Is Nothing Then
                        'Create it if it doesn't exist
                        Dim newNode As New SolutionNode
                        newNode.Name = projectPath(count)
                        current.SolutionChildren.Add(newNode)
                        current = newNode
                    Else
                        'Otherwise select it
                        current = child
                    End If

                Next
                'Try to find the project node
                Dim proj = (From c In current.SolutionChildren Where c.Name.ToLower = projectPath.Last.ToLower).FirstOrDefault
                If proj Is Nothing Then
                    'If it doesn't exist, create it
                    Dim newNode As New SolutionNode
                    newNode.Name = projectPath.Last
                    If item.Value IsNot Nothing Then
                        newNode.Project = Project.OpenProjectFile(Path.Combine(Path.GetDirectoryName(Filename), item.Value.Replace("/", "\").TrimStart("\")), manager)
                        AddHandler newNode.Project.Modified, AddressOf Project_Modified
                    Else
                        newNode.Project = Nothing
                    End If
                    current.SolutionChildren.Add(newNode)
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
        End Sub

        Private Function GetProjectDictionary(ProjectNode As SolutionNode, CurrentPath As String) As Dictionary(Of String, String)
            If Not ProjectNode.IsDirectory Then
                'If it's a project
                Dim out As New Dictionary(Of String, String)
                out.Add(CurrentPath, ProjectNode.Project.Filename.Replace(Path.GetDirectoryName(Filename), ""))
                Return out
            ElseIf ProjectNode.IsDirectory AndAlso ProjectNode.SolutionChildren.Count = 0 AndAlso Not CurrentPath = "" Then
                'If it's a directory with no children
                Dim out As New Dictionary(Of String, String)
                out.Add(CurrentPath, Nothing)
                Return out
            Else
                'Otherwise, merge with a recursive call
                Dim out As New Dictionary(Of String, String)
                For Each item In ProjectNode.SolutionChildren
                    Dim toMerge = GetProjectDictionary(item, CurrentPath & "/" & item.Name)
                    For Each entry In toMerge
                        out.Add(entry.Key, entry.Value)
                    Next
                Next
                Return out
            End If
        End Function
#End Region
    End Class
End Namespace

