Imports Xceed.Wpf.AvalonDock.Layout

Public Class DocumentTab
    Inherits LayoutDocument
    Implements IDisposable
    Private _manager As PluginManager
    Private WithEvents _file As GenericFile

#Region "Properties"
    Public Property File As GenericFile
        Get
            If TypeOf Me.Content Is ObjectControl Then
                DirectCast(Me.Content, ObjectControl).UpdateObject()
            ElseIf TypeOf Me.Content Is TabControl Then
                For Each item In DirectCast(Me.Content, TabControl).Items
                    If TypeOf item.Content Is ObjectTab Then
                        DirectCast(item.Content, ObjectTab).UpdateObject()
                    End If
                Next
            End If
            Return _file
        End Get
        Set(value As GenericFile)
            Dim tabs = _manager.GetRefreshedTabs(value)
            If tabs.Count > 0 Then
                    Dim tabControl As New TabControl
                    tabControl.TabStripPlacement = Controls.Dock.Left
                    For Each item In tabs
                        tabControl.Items.Add(item)
                    Next
                    Me.Content = tabControl
                Else
                    Dim control = _manager.GetObjectControl(value)
                    If control IsNot Nothing Then
                        control.EditingObject = value
                        control.RefreshDisplay()
                        Me.Content = control
                    End If
                End If
            _file = value
        End Set
    End Property

    ''' <summary>
    ''' Gets whether or not the file has been modified since it was last opened or saved.
    ''' </summary>
    ''' <returns></returns>
    Public Property IsModified As Boolean
        Get
            Return _isModified
        End Get
        Private Set(value As Boolean)
            _isModified = value
            UpdateTitle()
        End Set
    End Property
    Dim _isModified As Boolean

    Private Property DisposeOnExit As Boolean
#End Region

#Region "Events"
    Public Event FileModified(sender As Object, e As EventArgs)
    Public Event FileSaved(sender As Object, e As EventArgs)
#End Region

#Region "Event Handlers"
    Private Sub DocumentTab_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        If IsModified Then
            If MessageBox.Show("Are you sure you want to close this file?  Any unsaved changes will be lost.", "Sky Editor", MessageBoxButton.YesNo) = MessageBoxResult.No Then
                e.Cancel = True
            Else
                _file.Dispose()
            End If
        Else
            _file.Dispose()
        End If
    End Sub
    Private Sub File_FileModified(sender As Object, e As EventArgs) Handles _file.FileModified
        IsModified = True
        RaiseEvent FileModified(sender, e)
    End Sub
    Private Sub _file_FileSaved(sender As Object, e As EventArgs) Handles _file.FileSaved
        IsModified = False
        RaiseEvent FileSaved(sender, e)
    End Sub
#End Region

#Region "Methods"
    Public Sub SaveFile(Filename As String)
        File.Save(Filename)
    End Sub
    Public Sub SaveFile()
        File.Save()
    End Sub

    Private Sub UpdateTitle()
        If File IsNot Nothing Then
            Me.Title = File.Name
            If IsModified Then
                Me.Title = "* " & Me.Title
            End If
        End If
    End Sub
#End Region

#Region "Constructors"
    Public Sub New()
        Me.CanClose = True
        Me.IsModified = False
    End Sub
    Public Sub New(File As GenericFile, Manager As PluginManager, Optional DisposeOnExit As Boolean = False)
        Me.New()
        _manager = Manager
        Me.File = File
        UpdateTitle()
        Me.DisposeOnExit = DisposeOnExit
    End Sub

#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If _file IsNot Nothing Then
                    _file.Dispose()
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
