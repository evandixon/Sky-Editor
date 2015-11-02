Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports SkyEditorBase.Interfaces

Public Class ObjectWindow
    Dim _objectToEdit As Object
    Dim _manager As PluginManager
    Public Property ObjectToEdit As Object
        Get
            UpdateSave()
            Return _objectToEdit
        End Get
        Set(value As Object)
            _objectToEdit = value
            RefreshDisplay()
        End Set
    End Property
    Public Sub RefreshDisplay()
        tcTabs.Items.Clear()
        For Each item In _manager.GetRefreshedTabs(_objectToEdit)
            tcTabs.Items.Add(item)
        Next

        If TypeOf _objectToEdit Is iOpenableFile Then
            menuFileOpen.Visibility = Visibility.Visible
        Else
            menuFileOpen.Visibility = Visibility.Collapsed
        End If

        If TypeOf _objectToEdit Is iSavable Then
            menuFileSave.Visibility = Visibility.Visible
            menuFileSaveAs.Visibility = Visibility.Visible
        Else
            menuFileSave.Visibility = Visibility.Collapsed
            menuFileSaveAs.Visibility = Visibility.Collapsed
        End If

        menuFile.Visibility = menuFileOpen.Visibility * menuFileSave.Visibility 'The underlying values are integers, and visibile is 0.  If any of these are visible, this is visible
        menuMain.Visibility = menuFile.Visibility
    End Sub
    Public Sub UpdateSave()
        For Each item In (From t In tcTabs.Items Order By DirectCast(t.Content, ObjectTab).UpdatePriority Descending)
            DirectCast(item.Content, ObjectTab).UpdateObject()
        Next
    End Sub
    Public Sub New(Manager As PluginManager)
        InitializeComponent()
        _manager = Manager
    End Sub

    Private Sub menuFileOpen_Click(sender As Object, e As RoutedEventArgs) Handles menuFileOpen.Click
        If TypeOf _objectToEdit Is iOpenableFile Then
            Dim o As New OpenFileDialog

            If TypeOf _objectToEdit Is iSavable Then
                Dim ext As String = DirectCast(ObjectToEdit, iSavable).DefaultExtension.Trim("*").Trim(".")
                If _manager.IOFilters.ContainsKey(ext) Then
                    o.Filter = String.Format("{0} Files (*.{1})|*.{1}|All Files (*.*)|*.*", _manager.IOFilters(ext), ext)
                Else
                    o.Filter = "All Files (*.*)|*.*"
                End If
            Else
                o.Filter = "All Files (*.*)|*.*"
            End If

            If o.ShowDialog = Forms.DialogResult.OK Then
                DirectCast(_objectToEdit, iOpenableFile).OpenFile(o.FileName)
                RefreshDisplay()
            End If
        End If
    End Sub

    Private Sub menuFileSave_Click(sender As Object, e As RoutedEventArgs) Handles menuFileSave.Click
        If TypeOf ObjectToEdit Is iSavable Then
            If TypeOf ObjectToEdit Is iOnDisk Then
                If String.IsNullOrEmpty(DirectCast(ObjectToEdit, iOnDisk).Filename) Then
                    menuFileSaveAs_Click(sender, e)
                Else
                    DirectCast(ObjectToEdit, iSavable).Save()
                End If
            Else
                DirectCast(ObjectToEdit, iSavable).Save()
            End If
        End If
    End Sub

    Private Sub menuFileSaveAs_Click(sender As Object, e As RoutedEventArgs) Handles menuFileSaveAs.Click
        If TypeOf ObjectToEdit Is iSavable Then
            Dim s As New SaveFileDialog
            Dim ext As String = DirectCast(ObjectToEdit, iSavable).DefaultExtension.Trim("*").Trim(".")
            If _manager.IOFilters.ContainsKey(ext) Then
                s.Filter = String.Format("{0} Files (*.{1})|*.{1}|All Files (*.*)|*.*", _manager.IOFilters(ext), ext)
            Else
                s.Filter = "All Files (*.*)|*.*"
            End If
            If s.ShowDialog Then
                DirectCast(ObjectToEdit, iSavable).Save(s.FileName)
                If TypeOf ObjectToEdit Is iOnDisk Then
                    DirectCast(ObjectToEdit, iOnDisk).Filename = s.FileName
                End If
            End If
        End If
    End Sub

    Public ReadOnly Property TabCount As Integer
        Get
            Return tcTabs.Items.Count
        End Get
    End Property
End Class