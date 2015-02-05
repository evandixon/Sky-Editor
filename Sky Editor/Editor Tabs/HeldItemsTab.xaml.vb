Imports SkyEditor.skyjed.save
Imports SkyEditorBase

Public Class HeldItemsTab
    Inherits EditorTab
    Dim IsSkySave As Boolean = False
    Dim IsTDSave As Boolean = False
    Dim IsRBSave As Boolean = False

    Public Overrides Sub RefreshDisplay(Save As GenericSave)
        If TypeOf Save Is SkySave Then
            LoadSkyHeldItemsDropDowns()
            IsSkySave = True
            IsTDSave = False
            IsRBSave = False
            'Load held items
            lbHeldItems.Items.Clear()
            For Each i In DirectCast(Save, SkySave).HeldItems
                lbHeldItems.Items.Add(i)
            Next
        ElseIf TypeOf Save Is TDSave Then
            IsSkySave = False
            IsTDSave = True
            IsRBSave = False
            LoadTDHeldItemsDropDowns()
            'Load Items
            lbHeldItems.Items.Clear()
            For Each i In DirectCast(Save, TDSave).HeldItems.items
                If i.isvalid Then lbHeldItems.Items.Add(i)
            Next
        ElseIf TypeOf Save Is RBSave Then
            LoadRBHeldItemsDropDowns()
            IsSkySave = False
            IsTDSave = False
            IsRBSave = True
            'Load Items
            lbHeldItems.Items.Clear()
            For Each i In DirectCast(Save, RBSave).HeldItems.items
                If i.isvalid Then lbHeldItems.Items.Add(i)
            Next
        End If
        Me.Header = String.Format(PluginHelper.GetLanguageItem("Category_HeldItems", "Held Items ({0})"), lbHeldItems.Items.Count)
    End Sub
    Public Overrides ReadOnly Property SupportedGames As String()
        Get
            Return {GameStrings.SkySave, GameStrings.TDSave, GameStrings.RBSave}
        End Get
    End Property

    Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
        Dim out As GenericSave = Nothing
        If TypeOf Save Is SkySave Then
            Dim sky = DirectCast(Save, SkySave)
            Dim HeldItems As New List(Of SkySave.SkyItem)
            For Each item In lbHeldItems.Items
                HeldItems.Add(item)
            Next
            sky.HeldItems = HeldItems.ToArray
            out = sky
        ElseIf TypeOf Save Is TDSave Then
            Dim td = DirectCast(Save, TDSave)
            Dim helditems As New Generic.List(Of TDItem)
            For Each item In lbHeldItems.Items
                helditems.Add(item)
            Next
            Dim tempHeldItems = td.HeldItems
            tempHeldItems.items = helditems.ToArray
            td.HeldItems = tempHeldItems
            out = td
        ElseIf TypeOf Save Is RBSave Then
            Dim rb = DirectCast(Save, RBSave)
            'Held Items
            Dim helditems As New Generic.List(Of RBItem)
            For Each item In lbHeldItems.Items
                helditems.Add(item)
            Next
            Dim tempHeldItems = rb.HeldItems
            tempHeldItems.items = helditems.ToArray
            rb.HeldItems = tempHeldItems
            out = rb
        End If
        Return out
    End Function
    Sub LoadSkyHeldItemsDropDowns()
        'Load Items
        gbHeldBoxContent.IsEnabled = True
        cbHeldItems.Items.Clear()
        cbHeldItemsBoxContents.Items.Clear()
        Dim keys As New Generic.List(Of String)
        For Each item In Lists.SkyItemNames.Keys
            keys.Add(Lists.SkyItemNames(item))
        Next
        keys.Sort(StringComparer.CurrentCultureIgnoreCase)
        For Each itemname As String In keys
            If Not itemname = "$$$" Then
                Dim i As New SkySave.SkyItem(Lists.SkyItemNamesInverse(itemname), 0)
                cbHeldItems.Items.Add(i)
                cbHeldItemsBoxContents.Items.Add(i)
                End If
        Next
        cbHeldItems.SelectedIndex = 0
        cbHeldItemsBoxContents.SelectedIndex = 0
        End Sub
    Sub LoadTDHeldItemsDropDowns()
        'Load Items
        gbHeldBoxContent.IsEnabled = False
        cbHeldItems.Items.Clear()
        Dim keys As New Generic.List(Of String)
        For Each item In Lists.SkyItemNames.Keys
            keys.Add(Lists.SkyItemNames(item))
        Next
        keys.Sort(StringComparer.CurrentCultureIgnoreCase)
        For Each itemname As String In keys
            If Not itemname = "$$$" Then
                Dim i As New TDItem(Lists.SkyItemNamesInverse(itemname), 0)
                cbHeldItems.Items.Add(i)
                cbHeldItemsBoxContents.Items.Add(i)
            End If
        Next
        cbHeldItems.SelectedIndex = 0
        cbHeldItemsBoxContents.SelectedIndex = 0
    End Sub
    Sub LoadRBHeldItemsDropDowns()
        'Load Items
        gbHeldBoxContent.IsEnabled = False
        cbHeldItems.Items.Clear()
        Dim keys As New Generic.List(Of String)
        For Each item In Lists.RBItemNames.Keys
            keys.Add(Lists.RBItemNames(item))
        Next
        keys.Sort(StringComparer.CurrentCultureIgnoreCase)
        For Each itemname As String In keys
            If Not itemname = "$$$" Then
                Dim i As New RBItem(Lists.RBItemNamesInverse(itemname), 0)
                cbHeldItems.Items.Add(i)
                cbHeldItemsBoxContents.Items.Add(i)
            End If
        Next
        cbHeldItems.SelectedIndex = 0
        cbHeldItemsBoxContents.SelectedIndex = 0
    End Sub
    Private Sub btnHeldItemsAdd_Click(sender As Object, e As RoutedEventArgs) Handles btnHeldItemsAdd.Click
        If IsSkySave Then
            If lbHeldItems.Items.Count < 50 Then
                Dim i As SkySave.SkyItem = New SkySave.SkyItem(cbHeldItems.SelectedItem.ItemID, numHeldItemsAddCount.Value)
                If i.IsBox Then
                    i.BoxContents = New SkySave.SkyItem(cbHeldItemsBoxContents.SelectedItem.ItemID, numHeldItemsBoxContentsAddCount.Value)
                End If
                lbHeldItems.Items.Add(i)
            Else
                MessageBox.Show(PluginHelper.GetLanguageItem("Error_TooManyHeldItems", "You have too many items.  You can only hold 50 items.  To add another, one must be deleted first."))
            End If
        ElseIf IsTDSave Then
            If lbHeldItems.Items.Count < 48 Then
                Dim i As TDItem = New TDItem(cbHeldItems.SelectedItem.id, numHeldItemsAddCount.Value)
                'If i.IsBox Then
                '    i.BoxContents = New SkySave.SkyItem(cbHeldItemsBoxContents.SelectedItem.ItemID, numHeldItemsBoxContentsAddCount.Value)
                'End If
                lbHeldItems.Items.Add(i)
            Else
                MessageBox.Show(PluginHelper.GetLanguageItem("Error_TooManyHeldItems", "You have too many items.  You can only hold 50 items.  To add another, one must be deleted first."))
            End If
        ElseIf IsRBSave Then
            If lbHeldItems.Items.Count < 24 Then
                Dim i As RBItem = New RBItem(cbHeldItems.SelectedItem.id, numHeldItemsAddCount.Value)
                lbHeldItems.Items.Add(i)
            Else
                MessageBox.Show(PluginHelper.GetLanguageItem("Error_TooManyHeldItems", "You have too many items.  You can only hold 50 items.  To add another, one must be deleted first."))
            End If
        End If
        Me.Header = String.Format(PluginHelper.GetLanguageItem("Category_HeldItems", "Held Items {0}"), lbHeldItems.Items.Count)
    End Sub

    Private Sub cbHeldItems_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbHeldItems.SelectionChanged
        If IsSkySave AndAlso cbHeldItems.SelectedIndex > -1 Then
            gbHeldBoxContent.Focusable = DirectCast(cbHeldItems.SelectedItem, SkySave.SkyItem).IsBox
        End If
    End Sub

    Private Sub lbHeldItemsDelete_Click(sender As Object, e As RoutedEventArgs) Handles lbHeldItemsDelete.Click
        If lbHeldItems.SelectedItems.Count > 0 Then
            For x As Integer = lbHeldItems.SelectedItems.Count - 1 To 0 Step -1
                lbHeldItems.Items.Remove(lbHeldItems.SelectedItems(x))
            Next
            Me.Header = String.Format(PluginHelper.GetLanguageItem("Category_HeldItems", "Held Items ({0})"), lbHeldItems.Items.Count)
        End If
    End Sub



    Private Sub HeldItemsTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.Header = String.Format(PluginHelper.GetLanguageItem("Category_HeldItems", "Held Items ({0})"), lbHeldItems.Items.Count)
        btnHeldItemsAdd.Content = PluginHelper.GetLanguageItem("Add")
        lbHeldItemsDelete.Header = PluginHelper.GetLanguageItem("Delete")
        gbHeldBoxContent.Header = PluginHelper.GetLanguageItem("BoxContents")
    End Sub
End Class
