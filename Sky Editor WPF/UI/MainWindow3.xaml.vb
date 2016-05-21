Imports SkyEditor.Core
Imports SkyEditor.Core.Settings
Imports SkyEditor.Core.UI
Imports SkyEditor.UI.WPF

Public Class MainWindow3
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub
    Public Property CurrentPluginManager As PluginManager
    Private Sub MainWindow3_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.Resources.Item("CurrentPluginManager") = CurrentPluginManager
        For Each item In WPFUiHelper.GenerateMenuItems(SkyEditor.Core.UI.UIHelper.GetMenuItemInfo(CurrentPluginManager, CurrentPluginManager.CurrentSettingsProvider.GetIsDevMode), CurrentPluginManager)
            menuMain.Items.Add(item)
            RegisterEventMenuItemHandlers(item)
        Next
    End Sub

    ''' <summary>
    ''' Adds MenuItemClicked(Object,EventArgs) as a handler for MenuItem.Click and its children
    ''' </summary>
    ''' <param name="MenuItem"></param>
    Private Sub RegisterEventMenuItemHandlers(MenuItem As MenuItem)
        For Each Item In MenuItem.Items
            RegisterEventMenuItemHandlers(Item)
        Next
        AddHandler MenuItem.Click, AddressOf MenuItemClicked
    End Sub

    Private Async Sub MenuItemClicked(sender As Object, e As EventArgs)
        Try
            If sender IsNot Nothing AndAlso TypeOf sender Is MenuItem Then
                Dim m As MenuItem = DirectCast(sender, MenuItem)
                Dim tasks As New List(Of Task)
                If m.Tag IsNot Nothing AndAlso TypeOf m.Tag Is List(Of MenuAction) Then
                    Dim tags = DirectCast(m.Tag, List(Of MenuAction))
                    For Each t In tags
                        tasks.Add(t.DoAction({dockingManager.ActiveContent}))
                    Next
                End If
                Await Task.WhenAll(tasks)
            End If
        Catch ex As Exception
            MessageBox.Show(My.Resources.Language.GenericErrorSeeOutput)
            PluginHelper.Writeline(ex.ToString, PluginHelper.LineType.Error)
            PluginHelper.SetLoadingStatusFailed()
        End Try
    End Sub
End Class
