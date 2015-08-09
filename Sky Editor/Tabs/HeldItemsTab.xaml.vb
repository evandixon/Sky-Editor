Imports SkyEditorBase
Imports SkyEditor.Interfaces
Imports SkyEditorBase.Utilities

Namespace Tabs
    Public Class HeldItemsTab
        Inherits EditorTab
        Dim slots As ItemSlot()
        Dim storage As iItemStorage

        Public Overrides ReadOnly Property SupportedTypes As Type()
            Get
                Return {GetType(iItemStorage)}
            End Get
        End Property

        Public Overrides Sub RefreshDisplay(Save As GenericSave)
            storage = Save.Convert(Of iItemStorage)()
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
        Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
            SaveSlot(lbInventorySlots.SelectedIndex)

            'Dim s = Save.Convert(Of iItemStorage)()
            'For count = 0 To s.HeldItemSlots.Length - 1
            '    s.HeldItemSlots(count).Setter.Invoke(slots(count).)
            'Next

            Return Save
        End Function
        Sub LoadHeldItemsDropDowns()
            cbHeldItems.Items.Clear()
            cbHeldItemsBoxContents.Items.Clear()

            gbHeldBoxContent.IsEnabled = storage.SupportsBoxes

            Dim dictionary As Dictionary(Of Integer, String) = storage.GetItemDictionary

            For Each item In From n In dictionary Select n Where Not n.Value = "$$$" Order By n.Value
                Dim i As New GenericListItem(Of Integer)(item.Value, item.Key)
                cbHeldItems.Items.Add(i)
                If gbHeldBoxContent.IsEnabled Then
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
            Else
                MessageBox.Show(PluginHelper.GetLanguageItem("Error_TooManyHeldItems", "You are holding the maximum number of items.  To add another, one must be deleted first."))
            End If
            'Me.Header = PluginHelper.GetLanguageItem("Inventory")
        End Sub

        Private Sub cbHeldItems_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbHeldItems.SelectionChanged
            If gbHeldBoxContent.IsEnabled AndAlso cbHeldItems.SelectedIndex > -1 Then
                gbHeldBoxContent.Focusable = storage.IsBox(DirectCast(cbHeldItems.SelectedItem, GenericListItem(Of Integer)).Value) 'slots(lbInventorySlots.SelectedIndex).Creator.Invoke(DirectCast(cbHeldItems.SelectedItem, GenericListItem(Of Integer)).Value, 0).IsBox
            End If
        End Sub

        Private Sub lbHeldItemsDelete_Click(sender As Object, e As RoutedEventArgs) Handles lbHeldItemsDelete.Click
            If lbHeldItems.SelectedItems.Count > 0 Then
                For x As Integer = lbHeldItems.SelectedItems.Count - 1 To 0 Step -1
                    lbHeldItems.Items.Remove(lbHeldItems.SelectedItems(x))
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
        Public Overrides ReadOnly Property SortOrder As Integer
            Get
                Return 25
            End Get
        End Property
    End Class

End Namespace