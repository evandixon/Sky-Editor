Imports Xceed.Wpf.AvalonDock.Layout

Public Class DocumentTab
    Inherits LayoutDocument
    Private _manager As PluginManager
    Private _file As GenericFile
    Public Property File As GenericFile
        Get
            If TypeOf Me.Content Is ObjectControl Then
                _file = DirectCast(Me.Content, ObjectControl).UpdateObject(_file)
            ElseIf TypeOf Me.Content Is TabControl Then
                For Each item In DirectCast(Me.Content, TabControl).Items
                    If TypeOf item.Content Is EditorTab Then
                        _file = DirectCast(item.Content, EditorTab).UpdateSave(_file)
                    ElseIf TypeOf item.Content Is ObjectTab Then
                        _file = DirectCast(item.Content, ObjectTab).UpdateObject(_file)
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
                        control.RefreshDisplay(value)
                        Me.Content = control
                    End If
                End If
            End If
            _file = value
        End Set
    End Property
    Public Sub New()
        Me.CanClose = True
    End Sub
    Public Sub New(File As GenericFile, Manager As PluginManager)
        Me.New()
        _manager = Manager
        Me.File = File
        Me.Title = IO.Path.GetFileName(File.OriginalFilename)
    End Sub

    Private Sub DocumentTab_Closing(sender As Object, e As ComponentModel.CancelEventArgs) Handles Me.Closing
        If MessageBox.Show("Are you sure you want to close this file?  Any unsaved changes will be lost.", "Sky Editor", MessageBoxButton.YesNo) = MessageBoxResult.No Then
            e.Cancel = True
        End If
    End Sub
End Class
