Imports SkyEditorBase

Public Class EpisodeHeldItems
    Inherits EditorTab
    Public Overrides Sub RefreshDisplay(Save As GenericSave)
        lbSpEpisodeHeldItems.Items.Clear()
        For Each i In DirectCast(Save, SkySave).SpEpisode_HeldItems
            lbSpEpisodeHeldItems.Items.Add(i)
        Next
        Me.Header = String.Format(Lists.SkyEditorLanguageText("Category_SpEpisodeHeldItems"), lbSpEpisodeHeldItems.Items.Count)
    End Sub

    Public Overrides ReadOnly Property SupportedGames As String()
        Get
            Return {GameConstants.SkySave}
        End Get
    End Property

    Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
        Dim sky = DirectCast(Save, SkySave)
        Dim SpEpisodeHeldItems As New List(Of SkySave.SkyItem)
        For Each item In lbSpEpisodeHeldItems.Items
            SpEpisodeHeldItems.Add(item)
        Next
        Sky.SpEpisode_HeldItems = SpEpisodeHeldItems.ToArray
        Return sky
    End Function

    Private Sub EpisodeHeldItems_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'load language
        btnSpEpisodeHeldItemsAdd.Content = Lists.SkyEditorLanguageText("Add")
        lbSpEpisodeHeldItemsDelete.Header = Lists.SkyEditorLanguageText("Delete")
        gbSpEpisodeHeldBoxContent.Header = Lists.SkyEditorLanguageText("BoxContents")

        cbSpEpisodeHeldItems.Items.Clear()
        cbSpEpisodeHeldItemsBoxContents.Items.Clear()
        Dim keys As New Generic.List(Of String)
        For Each item In Lists.SkyItemNames.Keys
            keys.Add(Lists.SkyItemNames(item))
        Next
        keys.Sort(StringComparer.CurrentCultureIgnoreCase)
        For Each itemname As String In keys
            If Not itemname = "$$$" Then
                Dim i As New SkySave.SkyItem(Lists.SkyItemNamesInverse(itemname), 0)
                cbSpEpisodeHeldItems.Items.Add(i)
                cbSpEpisodeHeldItemsBoxContents.Items.Add(i)
            End If
        Next
        cbSpEpisodeHeldItems.SelectedIndex = 0
        cbSpEpisodeHeldItemsBoxContents.SelectedIndex = 0
        Me.Header = String.Format(Lists.SkyEditorLanguageText("Category_SpEpisodeHeldItems"), lbSpEpisodeHeldItems.Items.Count)
    End Sub
    Private Sub btnSpEpisodeHeldItemsAdd_Click(sender As Object, e As RoutedEventArgs) Handles btnSpEpisodeHeldItemsAdd.Click
        If lbSpEpisodeHeldItems.Items.Count < 50 Then
            Dim i As SkySave.SkyItem = New SkySave.SkyItem(cbSpEpisodeHeldItems.SelectedItem.ItemID, numSpEpisodeHeldItemsAddCount.Value)
            If i.IsBox Then
                i.BoxContents = New SkySave.SkyItem(cbSpEpisodeHeldItemsBoxContents.SelectedItem.ItemID, numSpEpisodeHeldItemsBoxContentsAddCount.Value)
            End If
            lbSpEpisodeHeldItems.Items.Add(i)
        Else
            MessageBox.Show(Lists.SkyEditorLanguageText("Error_TooManyHeldItems"))
        End If
        Me.Header = String.Format(Lists.SkyEditorLanguageText("Category_SpEpisodeHeldItems"), lbSpEpisodeHeldItems.Items.Count)
    End Sub
    Private Sub lbSpEpisodeHeldItemsDelete_Click(sender As Object, e As RoutedEventArgs) Handles lbSpEpisodeHeldItemsDelete.Click
        If lbSpEpisodeHeldItems.SelectedItems.Count > 0 Then
            For x As Integer = lbSpEpisodeHeldItems.SelectedItems.Count - 1 To 0 Step -1
                lbSpEpisodeHeldItems.Items.Remove(lbSpEpisodeHeldItems.SelectedItems(x))
            Next
            Me.Header = String.Format(Lists.SkyEditorLanguageText("Category_SpEpisodeHeldItems"), lbSpEpisodeHeldItems.Items.Count)
        End If
    End Sub
End Class
