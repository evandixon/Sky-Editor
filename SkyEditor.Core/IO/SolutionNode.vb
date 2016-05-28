Imports SkyEditor.Core.UI

Namespace IO
    ''' <summary>
    ''' Models a node in the solution's logical heiarchy.
    ''' </summary>
    Public Class SolutionNode
        Implements IDisposable
        Implements IHiearchyItem
        Implements IComparable(Of SolutionNode)

        Public ReadOnly Property IsDirectory As Boolean
            Get
                Return (Project Is Nothing)
            End Get
        End Property

        Public ReadOnly Property Prefix As String Implements IHiearchyItem.Prefix
            Get
                If IsDirectory Then
                    Return My.Resources.Language.Directory
                Else
                    Return My.Resources.Language.Project
                End If
            End Get
        End Property

        Public Property Name As String Implements IHiearchyItem.Name
            Get
                If IsDirectory Then
                    Return _name
                Else
                    Return Project.Name
                End If
            End Get
            Set(value As String)
                If IsDirectory Then
                    _name = value
                Else
                    Project.Name = value
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
        Public Property Children As ObservableCollection(Of IHiearchyItem) Implements IHiearchyItem.Children
            Get
                If IsDirectory Then
                    Return _children
                Else
                    Return Project.RootNode.Children
                End If
            End Get
            Set(value As ObservableCollection(Of IHiearchyItem))
                _children = value
            End Set
        End Property

        Dim _children As ObservableCollection(Of IHiearchyItem)

        Public Sub New()
            _name = ""
            _children = New ObservableCollection(Of IHiearchyItem)
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
                    For Each item In Children
                        If TypeOf item Is IDisposable Then
                            DirectCast(item, IDisposable).Dispose()
                        End If
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
End Namespace