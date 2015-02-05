Imports SkyEditorBase
Public Class ActivePokemonTab
    Inherits EditorTab

    Public Overrides Sub RefreshDisplay(Save As GenericSave)
        If TypeOf Save Is SkySave Then
            'Load Active Pokemon
            lbActivePokemon.Items.Clear()
            For Each apkm In DirectCast(Save, SkySave).JSave.activePkmn.pkmns
                If apkm.no > 0 Then
                    lbActivePokemon.Items.Add(apkm)
                End If
            Next
            Me.Header = String.Format(PluginHelper.GetLanguageItem("Category_ActivePokemon", "Active Pokemon ({0})"), lbActivePokemon.Items.Count)

        ElseIf TypeOf Save Is TDSave Then
            'Load Active Pokemon
            lbActivePokemon.Items.Clear()
            For Each apkm In DirectCast(Save, TDSave).ActivePokemon.pkmns
                If apkm.no > 0 Then
                    lbActivePokemon.Items.Add(apkm)
                End If
            Next
            Me.Header = String.Format(PluginHelper.GetLanguageItem("Category_ActivePokemon", "Active Pokemon ({0})"), lbActivePokemon.Items.Count)
        End If
    End Sub

    Public Overrides ReadOnly Property SupportedGames As String()
        Get
            Return {GameStrings.SkySave, GameStrings.TDSave}
        End Get
    End Property

    Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
        Dim out As GenericSave = Nothing
        If TypeOf Save Is SkySave Then
            Dim sky = DirectCast(Save, SkySave)
            'Update things using code from Sky JEditor
            Dim JSave = sky.JSave
            'update active pokemon
            Dim apkms As New List(Of skyjed.save.SkyPkmnEx)
            For Each p In lbActivePokemon.Items
                apkms.Add(p)
            Next
            JSave.activePkmn.pkmns = apkms.ToArray
            'Update JSave
            sky.JSave = JSave
            out = sky
        ElseIf TypeOf Save Is TDSave Then
            Dim td = DirectCast(Save, TDSave)
            'update active pokemon
            Dim apkms As New List(Of skyjed.save.TDPkmnEx)
            For Each p In lbActivePokemon.Items
                apkms.Add(p)
            Next
            Dim tempActivePokemon = td.ActivePokemon
            tempActivePokemon.pkmns = apkms.ToArray
            td.ActivePokemon = tempActivePokemon
            out = td
        End If
        Return out
    End Function
    Sub RefreshActivePKMDisplay()
        Dim pkms As New List(Of skyjed.save.iPkmnEx)
        For Each p In lbActivePokemon.Items
            pkms.Add(p)
        Next
        lbActivePokemon.Items.Clear()
        For count As Integer = 0 To pkms.Count - 1
            If pkms(count).no > 0 Then
                lbActivePokemon.Items.Add(pkms(count))
            End If
        Next
    End Sub
    Sub ShowActivePkmEditDialog()
        If lbActivePokemon.SelectedIndex > -1 Then
            'If save.IsSkySave Then
            Dim x As New ActivePkmWindow
            x.JSkyPokemonEx = lbActivePokemon.SelectedItem
            x.ShowDialog()
            lbActivePokemon.SelectedItem = x.JSkyPokemonEx
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
        Me.Header = String.Format(PluginHelper.GetLanguageItem("Category_ActivePokemon", "Active Pokemon ({0})"), lbActivePokemon.Items.Count)
        btnEditActivePokemon.Content = PluginHelper.GetLanguageItem("Edit")
    End Sub
End Class
