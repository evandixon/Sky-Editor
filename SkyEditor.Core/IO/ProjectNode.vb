﻿Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities

Namespace IO
    Public Class ProjectNode
        Implements IDisposable
        Implements IHiearchyItem
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
            Children = New ObservableCollection(Of IHiearchyItem)
            ParentProject = Project
            IsDirectory = True
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
                        DirectCast(item, Project).Dispose()
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