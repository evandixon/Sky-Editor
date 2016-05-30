Imports System.Reflection
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities

Namespace IO
    Public Class ProjectNode
        Implements IDisposable
        Implements IHiearchyItem
        Implements IComparable(Of ProjectNode)
        Implements INotifyPropertyChanged

        Public Sub New(Project As Project, parentNode As ProjectNode)
            Me.Children = New ObservableCollection(Of IHiearchyItem)
            Me.ParentNode = parentNode
            Me.ParentProject = Project
        End Sub

        Public Sub New(Project As Project, parentNode As ProjectNode, File As Object)
            Me.New(Project, parentNode)
            Me.File = File
            Me.AssemblyQualifiedTypeName = File.GetType.AssemblyQualifiedName
        End Sub


        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        ''' <summary>
        ''' Project to which this ProjectItem belongs.
        ''' </summary>
        ''' <returns></returns>
        Public Property ParentProject As Project

        Public Property ParentNode As ProjectNode

        ''' <summary>
        ''' Cached instance of the file.  Null if the file has not been opened or if this is a directory.
        ''' </summary>
        ''' <returns></returns>
        Private Property File As Object

        ''' <summary>
        ''' Whether or not this node is a directory.  If False, it's a file.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsDirectory As Boolean
            Get
                Return Filename Is Nothing AndAlso File Is Nothing
            End Get
        End Property

        Public ReadOnly Property Prefix As String Implements IHiearchyItem.Prefix
            Get
                If IsDirectory Then
                    Return My.Resources.Language.Directory
                Else
                    Return My.Resources.Language.File
                End If
            End Get
        End Property

        ''' <summary>
        ''' Name of the file or directory.
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String Implements IHiearchyItem.Name

        ''' <summary>
        ''' Path of the file, relative to the project directory.
        ''' </summary>
        ''' <returns></returns>
        Public Property Filename As String

        ''' <summary>
        ''' The child nodes of this node.
        ''' </summary>
        ''' <returns></returns>
        Public Property Children As ObservableCollection(Of IHiearchyItem) Implements IHiearchyItem.Children
            Get
                Return _children
            End Get
            Set(value As ObservableCollection(Of IHiearchyItem))
                If _children IsNot value Then
                    _children = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Children)))
                End If
            End Set
        End Property
        Dim _children As ObservableCollection(Of IHiearchyItem)


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

        ''' <summary>
        ''' Gets the full path of the file
        ''' </summary>
        ''' <returns></returns>
        Public Function GetFilename() As String
            Return Path.Combine(Path.GetDirectoryName(ParentProject.Filename), Filename?.TrimStart("\"))
        End Function

        Public Function CompareTo(other As ProjectNode) As Integer Implements IComparable(Of ProjectNode).CompareTo
            Return Me.Name.CompareTo(other.Name)
        End Function
        Public Function GetParentPath() As String
            If ParentNode Is Nothing Then
                Return ""
            Else
                Return ParentNode.GetParentPath & "/"
            End If
        End Function

        Public Function GetCurrentPath() As String
            Return GetParentPath() & "/" & Name
        End Function

        Public Function CanCreateChildDirectory() As Boolean
            Return Me.IsDirectory AndAlso ParentProject.CanCreateDirectory(GetCurrentPath)
        End Function

        Public Function CanCreateFile() As Boolean
            Return Me.IsDirectory AndAlso ParentProject.CanCreateFile(GetCurrentPath)
        End Function

        Public Function CanDeleteCurrentNode() As Boolean
            If Me.IsDirectory Then
                Return ParentProject.CanDeleteDirectory(GetCurrentPath)
            Else
                Return ParentProject.CanDeleteFile(GetCurrentPath)
            End If
        End Function

        Public Sub CreateChildDirectory(name As String)
            If CanCreateChildDirectory() Then
                Dim node As New ProjectNode(ParentProject, Me)
                node.Name = name
                Children.Add(node)
            End If
        End Sub

        Public Sub CreateFile(name As String, type As Type)
            If CanCreateFile() Then
                ParentProject.CreateFile(GetCurrentPath, name, type)
            End If
        End Sub

        Public Sub DeleteCurrentNode()
            If CanDeleteCurrentNode() AndAlso ParentNode IsNot Nothing Then
                ParentNode.Children.Remove(Me)
            End If
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    For Each item In Children
                        If TypeOf item Is IDisposable Then
                            DirectCast(item, IDisposable).Dispose()
                        End If
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
End Namespace