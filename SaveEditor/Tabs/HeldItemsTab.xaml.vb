Imports SkyEditorBase
Imports SaveEditor.Interfaces
Imports SkyEditorBase.Utilities
Imports SkyEditorBase.Interfaces

Namespace Tabs
    Public Class HeldItemsTab
        Inherits UserControl
        Implements iObjectControl
        Dim slots As ItemSlot()
        Dim storage As iItemStorage

        Public Sub RefreshDisplay()
            storage = GetEditingObject(Of iItemStorage)()
            slots = storage.HeldItemSlots
            RefreshSlotDisplay()
            If lbInventorySlots.SelectedIndex = -1 Then
                isRefresh = True
                lbInventorySlots.SelectedIndex = 0
            End If
            ShowSlot(lbInventorySlots.SelectedIndex)
        End Sub
        Private Sub RefreshSlotDisplay()
            Dim index As Integer = lbInventorySlots.SelectedIndex
            lbInventorySlots.Items.Clear()
            For Each item In slots
                lbInventorySlots.Items.Add(item.SlotName & String.Format(" ({0}/{1})", item.Getter.Invoke.Count, item.MaxItemCount))
            Next
            lbInventorySlots.SelectedIndex = index
        End Sub
        Private Sub ShowSlot(Index As Integer)
            LoadHeldItemsDropDowns()
            lbHeldItems.Items.Clear()
            For Each i In slots(Index).Getter.Invoke
                lbHeldItems.Items.Add(i)
            Next
        End Sub
        Private Sub SaveSlot(Index As Integer)
            Dim heldItems As New Generic.List(Of iItem)
            'heldItems.Add(From i In lbHeldItems.Items Select i)
            For Each item In lbHeldItems.Items
                heldItems.Add(item)
            Next
            slots(Index).Setter.Invoke(heldItems.ToArray)
        End Sub
        Public Sub UpdateObject()
            SaveSlot(lbInventorySlots.SelectedIndex)

            Dim s = GetEditingObject(Of iItemStorage)()

            For count = 0 To s.HeldItemSlots.Length - 1
                s.HeldItemSlots(count).Setter.Invoke(slots(count).Getter.Invoke)
            Next

        End Sub
        Sub LoadHeldItemsDropDowns()
            cbHeldItems.Items.Clear()
            cbHeldItemsBoxContents.Items.Clear()

            If storage.SupportsBoxes Then
                gbHeldBoxContent.Visibility = Visibility.Visible
            Else
                gbHeldBoxContent.Visibility = Visibility.Collapsed
            End If

            Dim dictionary As Dictionary(Of Integer, String) = storage.GetItemDictionary

            For Each item In From n In dictionary Select n Where Not n.Value = "$$$" Order By n.Value
                Dim i As New GenericListItem(Of Integer)(item.Value, item.Key)
                cbHeldItems.Items.Add(i)
                If gbHeldBoxContent.Visibility = Visibility.Visible Then
                    cbHeldItemsBoxContents.Items.Add(i)
                End If
            Next

            If cbHeldItems.Items.Count > 0 Then
                cbHeldItems.SelectedIndex = 0
            End If

            If cbHeldItemsBoxContents.Items.Count > 0 Then
                cbHeldItemsBoxContents.SelectedIndex = 0
            End If
        End Sub
        Private Sub btnHeldItemsAdd_Click(sender As Object, e As RoutedEventArgs) Handles btnHeldItemsAdd.Click
            If lbHeldItems.Items.Count < slots(lbInventorySlots.SelectedIndex).MaxItemCount Then
                Dim id As Integer = DirectCast(cbHeldItems.LastSafeValue, GenericListItem(Of Integer)).Value
                Dim i = slots(lbInventorySlots.SelectedIndex).Creator.Invoke(id, numHeldItemsAddCount.Value)
                'If i.IsBox Then
                '    i.BoxContents = New SkyItem(cbHeldItemsBoxContents.SelectedItem.ItemID, numHeldItemsBoxContentsAddCount.Value)
                'End If
                lbHeldItems.Items.Add(i)
                IsModified = True
            Else
                MessageBox.Show(PluginHelper.GetLanguageItem("Error_TooManyHeldItems", "You are holding the maximum number of items.  To add another, one must be deleted first."))
            End If
            'Me.Header = PluginHelper.GetLanguageItem("Inventory")
        End Sub

        Private Sub cbHeldItems_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbHeldItems.SelectionChanged
            If gbHeldBoxContent.Visibility = Visibility.Visible AndAlso cbHeldItems.SelectedIndex > -1 Then
                gbHeldBoxContent.IsEnabled = storage.IsBox(DirectCast(cbHeldItems.SelectedItem, GenericListItem(Of Integer)).Value) 'slots(lbInventorySlots.SelectedIndex).Creator.Invoke(DirectCast(cbHeldItems.SelectedItem, GenericListItem(Of Integer)).Value, 0).IsBox
            End If
        End Sub

        Private Sub lbHeldItemsDelete_Click(sender As Object, e As RoutedEventArgs) Handles lbHeldItemsDelete.Click
            If lbHeldItems.SelectedItems.Count > 0 Then
                For x As Integer = lbHeldItems.SelectedItems.Count - 1 To 0 Step -1
                    lbHeldItems.Items.Remove(lbHeldItems.SelectedItems(x))
                    IsModified = True
                Next
                ' Me.Header = PluginHelper.GetLanguageItem("Inventory")
            End If
        End Sub

        Private Sub HeldItemsTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("Inventory")
            btnHeldItemsAdd.Content = PluginHelper.GetLanguageItem("Add")
            lbHeldItemsDelete.Header = PluginHelper.GetLanguageItem("Delete")
            gbHeldBoxContent.Header = PluginHelper.GetLanguageItem("BoxContents")
        End Sub
        Dim oldSlot As Integer = -1
        Dim isRefresh As Boolean = False
        Private Sub lbInventorySlots_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lbInventorySlots.SelectionChanged
            If lbInventorySlots.Items.Count > 0 Then
                If Not isRefresh Then
                    If oldSlot > -1 Then
                        SaveSlot(oldSlot)
                    End If
                    ShowSlot(lbInventorySlots.SelectedIndex)
                    oldSlot = lbInventorySlots.SelectedIndex
                    isRefresh = True
                    RefreshSlotDisplay()
                Else
                    isRefresh = False
                End If
            End If
        End Sub

        Public Function GetSupportedTypes() As IEnumerable(Of Type) Implements iObjectControl.GetSupportedTypes
            Return {GetType(Interfaces.iItemStorage)}
        End Function

        Public Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer Implements iObjectControl.GetSortOrder
            Return 1
        End Function

#Region "IObjectControl Support"
        ''' <summary>
        ''' Called when Header is changed.
        ''' </summary>
        Public Event HeaderUpdated As iObjectControl.HeaderUpdatedEventHandler Implements iObjectControl.HeaderUpdated

        ''' <summary>
        ''' Called when IsModified is changed.
        ''' </summary>
        Public Event IsModifiedChanged As iObjectControl.IsModifiedChangedEventHandler Implements iObjectControl.IsModifiedChanged

        ''' <summary>
        ''' Returns the value of the Header.  Only used when the iObjectControl is behaving as a tab.
        ''' </summary>
        ''' <returns></returns>
        Public Property Header As String Implements iObjectControl.Header
            Get
                Return _header
            End Get
            Set(value As String)
                Dim oldValue = _header
                _header = value
                RaiseEvent HeaderUpdated(Me, New EventArguments.HeaderUpdatedEventArgs(oldValue, value))
            End Set
        End Property
        Dim _header As String

        ''' <summary>
        ''' Returns the current EditingObject, after casting it to type T.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <returns></returns>
        Protected Function GetEditingObject(Of T)() As T
            Return PluginHelper.Cast(Of T)(_editingObject)
        End Function

        ''' <summary>
        ''' Returns the current EditingObject.
        ''' It is recommended to use GetEditingObject(Of T), since it returns iContainter(Of T).Item if the EditingObject implements that interface.
        ''' </summary>
        ''' <returns></returns>
        Protected Function GetEditingObject() As Object
            Return _editingObject
        End Function

        ''' <summary>
        ''' The way to get the EditingObject from outside this class.  Refreshes the display on set, and updates the object on get.
        ''' Calling this from inside this class could result in a stack overflow, especially if called from UpdateObject, so use GetEditingObject or GetEditingObject(Of T) instead.
        ''' </summary>
        ''' <returns></returns>
        Public Property EditingObject As Object Implements iObjectControl.EditingObject
            Get
                UpdateObject()
                Return _editingObject
            End Get
            Set(value As Object)
                _editingObject = value
                RefreshDisplay()
            End Set
        End Property
        Dim _editingObject As Object

        ''' <summary>
        ''' Whether or not the EditingObject has been modified without saving.
        ''' Set to true when the user changes anything in the GUI.
        ''' Set to false when the object is saved, or if the user undoes every change.
        ''' </summary>
        ''' <returns></returns>
        Public Property IsModified As Boolean Implements iObjectControl.IsModified
            Get
                Return _isModified
            End Get
            Set(value As Boolean)
                Dim oldValue As Boolean = _isModified
                _isModified = value
                If Not oldValue = _isModified Then
                    RaiseEvent IsModifiedChanged(Me, New EventArgs)
                End If
            End Set
        End Property
        Dim _isModified As Boolean
#End Region
    End Class

End Namespace