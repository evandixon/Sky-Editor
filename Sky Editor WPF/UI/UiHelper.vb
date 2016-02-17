Imports System.Reflection
Imports SkyEditorBase.Interfaces

Namespace UI
    Public Class UiHelper

        ''' <summary>
        ''' Generates TargetedToolWindows from the given ITargetedControls.
        ''' </summary>
        ''' <param name="Info">IEnumerable of ITargetedControl used to generate TargetedToolWindows.  Currently, only UserControls implementing this interface are supported; all others will be ignored.</param>
        ''' <returns></returns>
        Public Shared Function GenerateToolWindows(Info As IEnumerable(Of ITargetedControl)) As IEnumerable(Of TargetedToolWindow)
            Dim out As New List(Of TargetedToolWindow)
            For Each item In Info
                If TypeOf item Is UserControl Then
                    Dim tool As New TargetedToolWindow
                    tool.ContainedControl = item
                    out.Add(tool)
                End If
            Next
            Return out
        End Function

        ''' <summary>
        ''' Generates ObjectTabs using the given ObjectControls
        ''' </summary>
        ''' <param name="ObjectControls"></param>
        ''' <returns></returns>
        Public Shared Function GenerateObjectTabs(ObjectControls As IEnumerable(Of iObjectControl)) As List(Of ObjectTab)
            If ObjectControls Is Nothing Then
                Throw New ArgumentNullException(NameOf(ObjectControls))
            End If

            Dim output As New List(Of ObjectTab)

            For Each item In ObjectControls
                output.Add(New ObjectTab(item))
            Next

            Return output
        End Function

        ''' <summary>
        ''' Generates MenuItems from the given IEnumerable of MenuItemInfo.
        ''' </summary>
        ''' <param name="MenuItemInfo">IEnumerable of MenuItemInfo that will be used to create the MenuItems.</param>
        ''' <returns></returns>
        Public Shared Function GenerateMenuItems(MenuItemInfo As IEnumerable(Of MenuItemInfo)) As List(Of MenuItem)
            If MenuItemInfo Is Nothing Then
                Throw New ArgumentNullException(NameOf(MenuItemInfo))
            End If

            Dim output As New List(Of MenuItem)

            For Each item In MenuItemInfo
                Dim m As New MenuItem
                m.Header = item.Header
                m.Tag = New List(Of MenuAction)
                For Each action In item.ActionTypes
                    DirectCast(m.Tag, List(Of MenuAction)).Add(action.GetConstructor({}).Invoke({}))
                Next
                For Each child In GenerateMenuItems(item.Children)
                    m.Items.Add(child)
                Next
                output.Add(m)
            Next

            Return output
        End Function

        ''' <summary>
        ''' Recursively pdates the visibility of the MenuItems based on the currently selected Types.
        ''' </summary>
        ''' <param name="SelectedObjects">IEnumerable of the currently seleted objects.  Used to determine visibility.</param>
        ''' <param name="MainMenu">Menu containing menu items of which to update the visibility.</param>
        Public Shared Sub UpdateMenuItemVisibility(SelectedObjects As IEnumerable(Of Object), MainMenu As Menu)
            If SelectedObjects Is Nothing Then
                Throw New ArgumentNullException(NameOf(SelectedObjects))
            End If
            If MainMenu Is Nothing Then
                Throw New ArgumentNullException(NameOf(MainMenu))
            End If
            For Each item As MenuItem In MainMenu.Items
                UpdateMenuItemVisibility(SelectedObjects, item)
            Next
        End Sub

        ''' <summary>
        ''' Recursively pdates the visibility of the MenuItems based on the currently selected Types.
        ''' </summary>
        ''' <param name="SelectedObjects">IEnumerable of the currently seleted objects.  Used to determine visibility.</param>
        ''' <param name="Parents">MenuItems of which to recursively update the visibility.</param>
        Public Shared Sub UpdateMenuItemVisibility(SelectedObjects As IEnumerable(Of Object), Parents As IEnumerable(Of MenuItem))
            If SelectedObjects Is Nothing Then
                Throw New ArgumentNullException(NameOf(SelectedObjects))
            End If
            If Parents Is Nothing Then
                Throw New ArgumentNullException(NameOf(Parents))
            End If

            For Each item In Parents
                UpdateMenuItemVisibility(SelectedObjects, item)
            Next
        End Sub

        ''' <summary>
        ''' Recursively pdates the visibility of the MenuItems based on the currently selected Types.
        ''' </summary>
        ''' <param name="SelectedObjects">IEnumerable of the currently seleted objects.  Used to determine visibility.</param>
        ''' <param name="Parent">Root menu item of which to recursively update the visibility.</param>
        Public Shared Sub UpdateMenuItemVisibility(SelectedObjects As IEnumerable(Of Object), Parent As MenuItem)
            If SelectedObjects Is Nothing Then
                Throw New ArgumentNullException(NameOf(SelectedObjects))
            End If
            If Parent Is Nothing Then
                Throw New ArgumentNullException(NameOf(Parent))
            End If

            For Each item In Parent.Items
                UpdateMenuItemVisibility(SelectedObjects, item)
            Next
            'If this tag has at least one action
            If Parent.Tag IsNot Nothing AndAlso TypeOf Parent.Tag Is List(Of MenuAction) AndAlso DirectCast(Parent.Tag, List(Of MenuAction)).Count > 0 Then
                Dim tags = DirectCast(Parent.Tag, List(Of MenuAction))
                Dim hasMatch As Boolean = False
                'Each menu item has one or more menu action
                For Each tag In tags

                    If Not hasMatch Then
                        'Each action can target multiple things
                        hasMatch = tag.AlwaysVisible OrElse tag.SupportsObjects(SelectedObjects)
                    Else
                        Exit For
                    End If

                Next

                If hasMatch Then
                    Parent.Visibility = Visibility.Visible
                Else
                    Parent.Visibility = Visibility.Collapsed
                End If
            End If
            'If this tag has child tags
            If Parent.HasItems Then
                'This menu item doesn't have an action.
                'Setting visibility to whether or not it has visible children.
                If MenuItemHasVisibleChildren(Parent) Then
                    Parent.Visibility = Visibility.Visible
                Else
                    Parent.Visibility = Visibility.Collapsed
                End If
            End If
        End Sub

        ''' <summary>
        ''' Determines whether or not the given menu item has visible children.
        ''' </summary>
        ''' <param name="Item"></param>
        Public Shared Function MenuItemHasVisibleChildren(Item As MenuItem) As Boolean
            Dim q = From m As MenuItem In Item.Items Where m.Visibility = Visibility.Visible

            Return q.Any
        End Function

        ''' <summary>
        ''' Translates certain properties of certain controls using PluginHelper.GetLanguageItem.
        ''' Supported controls: Label.Content, Button.Content, CheckBox.Content, MenuItem.Header, TabItem.Header.
        ''' Others may be added in the future.
        ''' </summary>
        ''' <param name="v"></param>
        ''' <param name="SearchLevel">The depth to search for controls.</param>
        ''' <remarks></remarks>
        Public Shared Sub TranslateForm(ByRef v As Visual, Optional SearchLevel As Integer = 5)
            Dim controls = (New ChildControls).GetChildren(v, 10)
            If Not controls.Contains(v) Then controls.Add(v)
            For Each item In controls
                If TypeOf item Is Label Then
                    Dim t As String = DirectCast(item, Label).Content
                    If t IsNot Nothing AndAlso Not String.IsNullOrEmpty(t) Then DirectCast(item, Label).Content = PluginHelper.GetLanguageItem(t.Trim("$"), CallingAssembly:=Assembly.GetCallingAssembly.GetName.Name)

                ElseIf TypeOf item Is Button Then
                    Dim t As String = DirectCast(item, Button).Content
                    If t IsNot Nothing AndAlso Not String.IsNullOrEmpty(t) Then DirectCast(item, Button).Content = PluginHelper.GetLanguageItem(t.Trim("$"), CallingAssembly:=Assembly.GetCallingAssembly.GetName.Name)

                ElseIf TypeOf item Is CheckBox Then
                    Dim t As String = DirectCast(item, CheckBox).Content
                    If t IsNot Nothing AndAlso Not String.IsNullOrEmpty(t) Then DirectCast(item, CheckBox).Content = PluginHelper.GetLanguageItem(t.Trim("$"), CallingAssembly:=Assembly.GetCallingAssembly.GetName.Name)

                ElseIf TypeOf item Is MenuItem Then
                    Dim t As String = DirectCast(item, MenuItem).Header
                    If t IsNot Nothing AndAlso Not String.IsNullOrEmpty(t) Then DirectCast(item, MenuItem).Header = PluginHelper.GetLanguageItem(t.Trim("$"), CallingAssembly:=Assembly.GetCallingAssembly.GetName.Name)

                ElseIf TypeOf item Is TabItem Then
                    Dim t As String = DirectCast(item, TabItem).Header
                    If t IsNot Nothing AndAlso Not String.IsNullOrEmpty(t) Then DirectCast(item, TabItem).Header = PluginHelper.GetLanguageItem(t.Trim("$"), CallingAssembly:=Assembly.GetCallingAssembly.GetName.Name)
                End If
            Next
        End Sub
    End Class

End Namespace
