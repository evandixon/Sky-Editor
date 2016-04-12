Imports SkyEditorBase
Imports SaveEditor.Interfaces
Imports SkyEditorBase.Utilities
Imports SkyEditorBase.Interfaces
Imports SkyEditorWPF.UI

Namespace Tabs
    Public Class HeldItemsTab
        Inherits ObjectControl
        Dim slots As ItemSlot()
        Dim storage As iItemStorage

        Public Overrides Sub RefreshDisplay()
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
        Public Overrides Sub UpdateObject()
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
                MessageBox.Show(My.Resources.Language.ErrorTooManyItems)
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
            Me.Header = My.Resources.Language.HeldItemsTabHeader
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

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(Interfaces.iItemStorage)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 1
        End Function

    End Class

End Namespace