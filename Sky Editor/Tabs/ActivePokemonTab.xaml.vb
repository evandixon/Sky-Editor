Imports SkyEditorBase
Imports SkyEditor.Interfaces
Namespace Tabs
    Public Class ActivePokemonTab
        Inherits EditorTab
        Public Overloads Overrides Sub RefreshDisplay(Save As GenericSave)
            If Save.IsOfType(GetType(iParty)) Then
                Dim p = Save.Convert(Of iParty)()
                lbActivePokemon.Items.Clear()
                For Each apkm In p.GetPokemon
                    If apkm.IsValid Then
                        lbActivePokemon.Items.Add(apkm)
                    End If
                Next
            End If
        End Sub

        Public Overrides Function UpdateSave(Save As GenericSave) As GenericSave
            If Save.IsOfType(GetType(iParty)) Then
                Dim p = Save.Convert(Of iParty)()
                Dim apkms As New List(Of iMDPkm)
                For Each item In lbActivePokemon.Items
                    apkms.Add(item)
                Next
                p.SetPokemon(apkms.ToArray)
                Save = Save.Convert(Of iParty)(p)
            End If
            Return Save
        End Function
        Public Overrides ReadOnly Property SupportedTypes As Type()
            Get
                Return {GetType(iParty)}
            End Get
        End Property

        Private Sub ActivePokemonTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("Party")
        End Sub
        Sub RefreshActivePKMDisplay()
            Dim pkms As New List(Of iMDPkm)
            For Each p In lbActivePokemon.Items
                pkms.Add(p)
            Next
            lbActivePokemon.Items.Clear()
            For count As Integer = 0 To pkms.Count - 1
                If pkms(count).ID > 0 Then
                    lbActivePokemon.Items.Add(pkms(count))
                End If
            Next
        End Sub
        Sub ShowActivePkmEditDialog()
            If lbActivePokemon.SelectedIndex > -1 Then
                Dim w As New SkyEditorBase.ObjectWindow(Me.GetPluginManager)
                w.ObjectToEdit = lbActivePokemon.SelectedItem
                w.ShowDialog()
                lbActivePokemon.SelectedItem = w.ObjectToEdit
                RefreshActivePKMDisplay()
            End If
        End Sub
        Private Sub btnEditActivePokemon_Click(sender As Object, e As RoutedEventArgs) Handles btnEditActivePokemon.Click
            ShowActivePkmEditDialog()
        End Sub

        Private Sub lbActivePokemon_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles lbActivePokemon.MouseDoubleClick
            ShowActivePkmEditDialog()
        End Sub
        Public Overrides ReadOnly Property SortOrder As Integer
            Get
                Return 23
            End Get
        End Property
    End Class

End Namespace