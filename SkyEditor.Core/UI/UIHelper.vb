Imports System.Reflection
Imports SkyEditor.Core.Utilities

Namespace UI
    Public Class UIHelper

        ''' <summary>
        ''' Gets the currently registered MenuActions in heiarchy form.
        ''' </summary>
        ''' <param name="pluginManager">Instance of the current plugin manager.</param>
        ''' <param name="isDevMode">Whether or not to get the dev-only menu items.</param>
        ''' <returns></returns>
        Public Shared Function GetMenuItemInfo(pluginManager As PluginManager, isDevMode As Boolean) As List(Of MenuItemInfo)
            Dim menuItems As New List(Of MenuItemInfo)
            For Each ActionType In pluginManager.GetRegisteredTypes(GetType(MenuAction).GetTypeInfo)
                'While we're registering the type, we need an instance to get extra information, like where to put it
                Dim ActionInstance As MenuAction = ReflectionHelpers.CreateInstance(ActionType)

                'DevOnly menu actions are only supported if we're in dev mode.
                If isDevMode OrElse Not ActionInstance.DevOnly Then

                    'Generate the MenuItem

                    If ActionInstance.ActionPath.Count >= 1 Then
                        'Create parent menu items
                        Dim parent = From m In menuItems Where m.Header = ActionInstance.ActionPath(0)

                        Dim current As MenuItemInfo
                        If parent.Any Then
                            current = parent.First
                            If current.ActionTypes.Count = 0 Then
                                current.SortOrder = Math.Min(current.SortOrder, ActionInstance.SortOrder)
                            End If
                        Else
                            Dim m As New MenuItemInfo
                            m.Header = ActionInstance.ActionPath(0)
                            m.Children = New List(Of MenuItemInfo)
                            m.ActionTypes = New List(Of TypeInfo)
                            m.SortOrder = ActionInstance.SortOrder
                            If ActionInstance.ActionPath.Count = 1 Then
                                m.ActionTypes.Add(ActionType)
                            End If
                            menuItems.Add(m)
                            current = m
                        End If


                        For count = 1 To ActionInstance.ActionPath.Count - 2
                            Dim index = count 'To avoid potential issues with using the below linq expression.  Might not be needed, but it's probably best to avoid potential issues.
                            parent = From m As MenuItemInfo In current.Children Where m.Header = ActionInstance.ActionPath(index)
                            If parent.Any Then
                                current = parent.First
                                If current.ActionTypes.Count = 0 Then
                                    current.SortOrder = Math.Min(current.SortOrder, ActionInstance.SortOrder)
                                End If
                            Else
                                Dim m As New MenuItemInfo
                                m.Header = ActionInstance.ActionPath(count)
                                m.Children = New List(Of MenuItemInfo)
                                m.SortOrder = ActionInstance.SortOrder
                                If count = 0 Then
                                    menuItems.Add(m)
                                Else
                                    current.Children.Add(m)
                                End If
                                current = m
                            End If
                        Next


                        If ActionInstance.ActionPath.Count > 1 Then
                            'Check to see if the menu item exists
                            parent = From m As MenuItemInfo In current.Children Where m.Header = ActionInstance.ActionPath.Last

                            If parent.Any Then
                                Dim m = DirectCast(parent.First, MenuItemInfo)
                                m.ActionTypes = New List(Of TypeInfo)
                                m.ActionTypes.Add(ActionType)
                            Else
                                'Add the menu item, and give it a proper tag
                                Dim m As New MenuItemInfo
                                m.Children = New List(Of MenuItemInfo)
                                m.Header = ActionInstance.ActionPath.Last
                                m.SortOrder = ActionInstance.SortOrder
                                m.ActionTypes = New List(Of TypeInfo)
                                m.ActionTypes.Add(ActionType)
                                current.Children.Add(m)
                            End If
                        End If

                    Else 'Count=0
                        Throw New ArgumentException(My.Resources.Language.ErrorMenuActionEmptyActionPath)
                    End If
                End If
            Next
            Return menuItems
        End Function
    End Class
End Namespace

