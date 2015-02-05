Imports SkyEditorBase
Public Class QuicksavePokemonTab
    Inherits EditorTab

    Public Overrides Sub RefreshDisplay(Save As GenericSave)
        If TypeOf Save Is SkySave Then
            Dim QSave = DirectCast(Save, SkySave).QSave
            'If QSave Is Nothing Then
            '    Me.Visibility = Windows.Visibility.Hidden
            '    Exit Sub
            'End If
            'Load Active Pokemon
            lbActivePokemon.Items.Clear()
            For Each apkm In QSave.pkmnQStorage.pkmns
                'If apkm.no1 > 0 Then
                lbActivePokemon.Items.Add(apkm)
                'End If
            Next
            Me.Header = String.Format(PluginHelper.GetLanguageItem("Category_QuicksavePokemon", "Quicksave Pokemon ({0})"), lbActivePokemon.Items.Count)
        End If
    End Sub

    Public Overrides ReadOnly Property SupportedGames As String()
        Get
            Return {GameStrings.SkySave}
        End Get
    End Property

    Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
        If Me.Visibility = System.Windows.Visibility.Hidden Then
            Return Save
        End If
        Dim out As GenericSave = Nothing
        If TypeOf Save Is SkySave Then
            Dim sky = DirectCast(Save, SkySave)
            'Update things using code from Sky JEditor
            Dim QSave = sky.QSave
            'update active pokemon
            Dim apkms As New List(Of skyjed.save.SkyPkmnQ)
            For Each p In lbActivePokemon.Items
                apkms.Add(p)
            Next
            QSave.pkmnQStorage.pkmns = apkms.ToArray
            'Update JSave
            sky.QSave = QSave
            out = sky
        End If
        Return out
    End Function
    Sub RefreshActivePKMDisplay()
        Dim pkms As New List(Of skyjed.save.iPkmnQ)
        For Each p In lbActivePokemon.Items
            pkms.Add(p)
        Next
        lbActivePokemon.Items.Clear()
        For count As Integer = 0 To pkms.Count - 1
            lbActivePokemon.Items.Add(pkms(count))
        Next
    End Sub
    Sub ShowActivePkmEditDialog()
        If lbActivePokemon.SelectedIndex > -1 Then
            'If save.IsSkySave Then
            Dim x As New QPkmWindow
            x.JSkyPokemonQ = lbActivePokemon.SelectedItem
            x.ShowDialog()
            lbActivePokemon.SelectedItem = x.JSkyPokemonQ
            RefreshActivePKMDisplay()
            'End If
        End If
    End Sub
    Private Sub btnEditActivePokemon_Click(sender As Object, e As RoutedEventArgs) Handles btnEditActivePokemon.Click
        ShowActivePkmEditDialog()
    End Sub

    Private Sub lbActivePokemon_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles lbActivePokemon.MouseDoubleClick
        ShowActivePkmEditDialog()
    End Sub

    Private Sub ActivePokemonTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.Header = String.Format(PluginHelper.GetLanguageItem("Category_QuicksavePokemon", "Quicksave Pokemon ({0})"), lbActivePokemon.Items.Count)
        btnEditActivePokemon.Content = PluginHelper.GetLanguageItem("Edit")
    End Sub
End Class
