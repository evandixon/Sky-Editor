Imports SkyEditor.Core

Public Class ObjectWindow

        Dim _manager As PluginManager
        Public Property ObjectToEdit As Object
            Get
                Return placeholder.ObjectToEdit
            End Get
            Set(value As Object)
                placeholder.ObjectToEdit = value
            End Set
        End Property

        'Public Sub RefreshDisplay()
        '    tcTabs.Items.Clear()
        '    For Each item In WPFUiHelper.GenerateObjectTabs(SkyEditor.Core.UI.UIHelper.GetRefreshedTabs(_objectToEdit, {GetType(UserControl)}, _manager))
        '        tcTabs.Items.Add(item)
        '    Next

        '    If TypeOf _objectToEdit Is IOpenableFile Then
        '        menuFileOpen.Visibility = Visibility.Visible
        '    Else
        '        menuFileOpen.Visibility = Visibility.Collapsed
        '    End If

        '    If TypeOf _objectToEdit Is ISavable Then
        '        menuFileSave.Visibility = Visibility.Visible
        '        If TypeOf _objectToEdit Is ISavableAs Then
        '            menuFileSaveAs.Visibility = Visibility.Visible
        '        Else
        '            menuFileSaveAs.Visibility = Visibility.Collapsed
        '        End If
        '    Else
        '        menuFileSave.Visibility = Visibility.Collapsed
        '        menuFileSaveAs.Visibility = Visibility.Collapsed
        '    End If

        '    If TypeOf _objectToEdit Is iNamed Then
        '        Me.Title = DirectCast(_objectToEdit, iNamed).Name
        '    Else
        '        Me.Title = My.Resources.Language.Edit
        '    End If

        '    menuFile.Visibility = menuFileOpen.Visibility * menuFileSave.Visibility 'The underlying values are integers, and visibile is 0.  If any of these are visible, this is visible
        '    menuMain.Visibility = menuFile.Visibility
        'End Sub
        'Public Sub UpdateSave()
        '    For Each item In tcTabs.Items
        '        'Here, we want to force the ObjectControl to apply its GUI changes to the underlying model.
        '        'Calling should accomplish this goal
        '        Dim x = DirectCast(item.containedobjectcontrol, IObjectControl).EditingObject
        '    Next
        'End Sub

        ''' <summary>
        ''' This sub New is for the designer only
        ''' </summary>
        Protected Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
        End Sub
        Public Sub New(manager As PluginManager)
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            _manager = manager
            placeholder.CurrentPluginManager = manager
        End Sub

        'Private Async Sub menuFileOpen_Click(sender As Object, e As RoutedEventArgs) Handles menuFileOpen.Click
        '    If TypeOf _objectToEdit Is IOpenableFile Then
        '        Dim o As New OpenFileDialog

        '        If TypeOf _objectToEdit Is ISavableAs Then
        '            Dim ext As String = DirectCast(ObjectToEdit, ISavableAs).GetDefaultExtension.Trim("*").Trim(".")
        '            If _manager.CurrentIOUIManager.IOFilters.ContainsKey(ext) Then
        '                o.Filter = String.Format("{0} Files (*.{1})|*.{1}|All Files (*.*)|*.*", _manager.CurrentIOUIManager.IOFilters(ext), ext)
        '            Else
        '                o.Filter = "All Files (*.*)|*.*"
        '            End If
        '        Else
        '            o.Filter = "All Files (*.*)|*.*"
        '        End If

        '        If o.ShowDialog = Forms.DialogResult.OK Then
        '            Await DirectCast(_objectToEdit, IOpenableFile).OpenFile(o.FileName, _manager.CurrentIOProvider)
        '            RefreshDisplay()
        '        End If
        '    End If
        'End Sub

        'Private Sub menuFileSave_Click(sender As Object, e As RoutedEventArgs) Handles menuFileSave.Click
        '    If TypeOf ObjectToEdit Is ISavable Then
        '        If TypeOf ObjectToEdit Is IOnDisk Then
        '            If String.IsNullOrEmpty(DirectCast(ObjectToEdit, IOnDisk).Filename) Then
        '                menuFileSaveAs_Click(sender, e)
        '            Else
        '                DirectCast(ObjectToEdit, ISavable).Save(_manager.CurrentIOProvider)
        '            End If
        '        Else
        '            DirectCast(ObjectToEdit, ISavable).Save(_manager.CurrentIOProvider)
        '        End If
        '    End If
        'End Sub

        'Private Sub menuFileSaveAs_Click(sender As Object, e As RoutedEventArgs) Handles menuFileSaveAs.Click
        '    If TypeOf ObjectToEdit Is ISavableAs Then
        '        Dim s As New SaveFileDialog
        '        Dim ext As String = DirectCast(ObjectToEdit, ISavableAs).GetDefaultExtension.Trim("*").Trim(".")
        '        If _manager.CurrentIOUIManager.IOFilters.ContainsKey(ext) Then
        '            s.Filter = String.Format("{0} Files (*.{1})|*.{1}|All Files (*.*)|*.*", _manager.CurrentIOUIManager.IOFilters(ext), ext)
        '        Else
        '            s.Filter = "All Files (*.*)|*.*"
        '        End If
        '        If s.ShowDialog Then
        '            DirectCast(ObjectToEdit, ISavableAs).Save(s.FileName, _manager.CurrentIOProvider)
        '            If TypeOf ObjectToEdit Is IOnDisk Then
        '                DirectCast(ObjectToEdit, IOnDisk).Filename = s.FileName
        '            End If
        '        End If
        '    End If
        'End Sub

        Public Shadows Function ShowDialog() As Boolean?
            Return MyBase.ShowDialog
        End Function
End Class