Namespace UI
    Public Class ExtensionManager
        Inherits ObjectControl

        Private Function GetTVItem(Collection As SkyEditorBase.Extensions.IExtensionCollection)
            Dim item As New TreeViewItem
            item.Header = Collection.Name
            item.Tag = Collection
            For Each child In Collection.GetChildCollections
                item.Items.Add(GetTVItem(child))
            Next
            Return item
        End Function

        Private Sub ExtensionManager_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("Extension Manager")
            tvCategories.Items.Clear()
            tvCategories.Items.Add(GetTVItem(New SkyEditorBase.Extensions.InstalledExtensionCollection))
        End Sub

        Private Sub tvCategories_SelectedItemChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Object)) Handles tvCategories.SelectedItemChanged
            If tvCategories.SelectedItem IsNot Nothing Then
                lvExtensions.ItemsSource = DirectCast(tvCategories.SelectedItem.Tag, SkyEditorBase.Extensions.IExtensionCollection).GetExtensions
            End If
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(SkyEditorBase.Extensions.ExtensionHelper)}
        End Function

    End Class
End Namespace

