Imports SkyEditorBase
Imports SkyEditor.skyjed.save

Public Class TDSStoredItems
    Inherits EditorTab
    Dim IsSkySave As Boolean = False
    Dim IsTDSave As Boolean = False
    Dim IsRBSave As Boolean = False
    Public Overrides Sub RefreshDisplay(Save As GenericSave)
        If TypeOf Save Is SkySave Then
            IsSkySave = True
            IsTDSave = False
            IsRBSave = False

            LoadSkyHeldItemsDropDowns()
            'Load stored items
            lbStoredItems.Items.Clear()
            For Each item In DirectCast(Save, SkySave).JSave.itemStorage.items
                If item.isvalid Then
                    lbStoredItems.Items.Add(item)
                End If
            Next
            Me.Header = String.Format(PluginHelper.GetLanguageItem("Category_StoredItems", "Stored Items ({0})"), lbStoredItems.Items.Count)
        End If
    End Sub
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
                Dim i As New SkyItem(Lists.SkyItemNamesInverse(itemname), 0)
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
            If lbStoredItems.Items.Count < 1000 Then
                Dim i As New SkyItem(DirectCast(cbHeldItems.SelectedItem, SkyItem).id, numHeldItemsAddCount.Value)
                If i.Box Then
                    i.param = DirectCast(cbHeldItemsBoxContents.SelectedItem, SkyItem).id
                End If
                lbStoredItems.Items.Add(i)
            Else
                MessageBox.Show(PluginHelper.GetLanguageItem("Error_FullStorage", "You can only have 1000 items in storage.  You must remove one to add another.")) 'Lists.SkyEditorLanguageText("Error_TooManyHeldItems"))
            End If
            'ElseIf IsTDSave Then
            '    If lbHeldItems.Items.Count < 48 Then
            '        Dim i As TDItem = New TDItem(cbHeldItems.SelectedItem.id, numHeldItemsAddCount.Value)
            '        'If i.IsBox Then
            '        '    i.BoxContents = New SkySave.SkyItem(cbHeldItemsBoxContents.SelectedItem.ItemID, numHeldItemsBoxContentsAddCount.Value)
            '        'End If
            '        lbHeldItems.Items.Add(i)
            '    Else
            '        MessageBox.Show(Lists.SkyEditorLanguageText("Error_TooManyHeldItems"))
            '    End If
            'ElseIf IsRBSave Then
            '    If lbHeldItems.Items.Count < 24 Then
            '        Dim i As RBItem = New RBItem(cbHeldItems.SelectedItem.id, numHeldItemsAddCount.Value)
            '        lbHeldItems.Items.Add(i)
            '    Else
            '        MessageBox.Show(Lists.SkyEditorLanguageText("Error_TooManyHeldItems"))
            '    End If
        End If
        Me.Header = String.Format(PluginHelper.GetLanguageItem("Category_StoredItems", "Stored Items ({0})"), lbStoredItems.Items.Count)
    End Sub

    Public Overrides ReadOnly Property SupportedGames As String()
        Get
            Return {GameStrings.SkySave}
        End Get
    End Property

    Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
        If TypeOf Save Is SkySave Then
            Dim j = DirectCast(Save, SkySave).JSave
            Dim s As New List(Of SkyItem)
            For count As Integer = 0 To 999
                If lbStoredItems.Items.Count > count Then
                    s.Add(lbStoredItems.Items(count))
                Else
                    s.Add(New SkyItem(0, 0))
                End If
            Next
            j.itemStorage.items = s.ToArray
            DirectCast(Save, SkySave).JSave = j
        End If
        Return Save
    End Function
    Private Sub lbStoredItemsItems_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbHeldItems.SelectionChanged
        If IsSkySave AndAlso cbHeldItems.SelectedIndex > -1 Then
            gbHeldBoxContent.Focusable = DirectCast(cbHeldItems.SelectedItem, SkyItem).Box
        End If
    End Sub

    Private Sub lbStoredItemsDelete_Click(sender As Object, e As RoutedEventArgs) Handles lbHeldItemsDelete.Click
        If lbStoredItems.SelectedItems.Count > 0 Then
            For x As Integer = lbStoredItems.SelectedItems.Count - 1 To 0 Step -1
                lbStoredItems.Items.Remove(lbStoredItems.SelectedItems(x))
            Next
            Me.Header = String.Format(PluginHelper.GetLanguageItem("Category_StoredItems", "Stored Items ({0})"), lbStoredItems.Items.Count)
        End If
    End Sub

    Private Sub TDSStoredItems_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If String.IsNullOrEmpty(Me.Header) Then Me.Header = String.Format(PluginHelper.GetLanguageItem("Category_StoredItems", "Stored Items ({0})"), "0")
        btnHeldItemsAdd.Content = PluginHelper.GetLanguageItem("Add")
        lbHeldItemsDelete.Header = PluginHelper.GetLanguageItem("Delete")
        gbHeldBoxContent.Header = PluginHelper.GetLanguageItem("BoxContents")
    End Sub
End Class