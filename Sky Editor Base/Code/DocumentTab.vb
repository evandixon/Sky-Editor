Imports Xceed.Wpf.AvalonDock.Layout

Public Class DocumentTab
    Inherits LayoutDocument
    Private _manager As PluginManager
    Private WithEvents _file As GenericFile

#Region "Properties"
    Public Property File As GenericFile
        Get
            If TypeOf Me.Content Is ObjectControl Then
                DirectCast(Me.Content, ObjectControl).UpdateObject
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
            If TypeOf value Is GenericSave Then
                Dim tabControl As New TabControl
                tabControl.TabStripPlacement = Controls.Dock.Left
                For Each item In _manager.GetRefreshedTabs(value)
                    tabControl.Items.Add(item)
                Next
                Me.Content = tabControl
            Else
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
            Return _ismodified
        End Get
        Private Set(value As Boolean)
            _isModified = value
            UpdateTitle()
        End Set
    End Property
    Dim _isModified As Boolean
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
            End If
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
            Me.Title = IO.Path.GetFileName(File.OriginalFilename)
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
    Public Sub New(File As GenericFile, Manager As PluginManager)
        Me.New()
        _manager = Manager
        Me.File = File
        UpdateTitle
    End Sub
#End Region

End Class
