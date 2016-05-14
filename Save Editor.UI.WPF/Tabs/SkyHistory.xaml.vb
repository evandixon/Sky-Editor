Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SkyEditorWPF.UI

Namespace Tabs
    Public Class SkyHistory
        Inherits ObjectControl

        Private WriteOnly Property PokemonDictionary As IDictionary(Of Integer, String)
            Set(value As IDictionary(Of Integer, String))
                cbPlayer.Items.Clear()
                cbPartner.Items.Clear()

                For Each item In (From v In value Order By v.Value)
                    cbPlayer.Items.Add(New Utilities.GenericListItem(Of Integer)(item.Value, item.Key))
                    cbPartner.Items.Add(New Utilities.GenericListItem(Of Integer)(item.Value, item.Key))
                Next

                cbPlayer.SelectedIndex = 0
                cbPartner.SelectedIndex = 0
            End Set
        End Property

        Private Property SelectedPlayerID As Integer
            Get
                Return DirectCast(cbPlayer.SelectedItem, Utilities.GenericListItem(Of Integer)).Value
            End Get
            Set(value As Integer)
                For Each item In cbPlayer.Items
                    If DirectCast(item, Utilities.GenericListItem(Of Integer)).Value = value Then
                        cbPlayer.SelectedItem = item
                    End If
                Next
            End Set
        End Property

        Private Property SelectedPartnerID As Integer
            Get
                Return DirectCast(cbPartner.SelectedItem, Utilities.GenericListItem(Of Integer)).Value
            End Get
            Set(value As Integer)
                For Each item In cbPartner.Items
                    If DirectCast(item, Utilities.GenericListItem(Of Integer)).Value = value Then
                        cbPartner.SelectedItem = item
                    End If
                Next
            End Set
        End Property

        ''' <summary>
        ''' Updates UI elements to display certain properties.
        ''' </summary>
        Public Overrides Sub RefreshDisplay()
            PokemonDictionary = Lists.GetSkyPokemon
            With GetEditingObject(Of SkySave)()
                txtPlayerName.Text = .OriginalPlayerName
                txtPartnerName.Text = .OriginalPartnerName
                SelectedPlayerID = .OriginalPlayerID
                SelectedPartnerID = .OriginalPartnerID
                chbPlayerGender.IsChecked = .OriginalPlayerIsFemale
                chbPartnerGender.IsChecked = .OriginalPartnerIsFemale
            End With
            IsModified = False
        End Sub

        ''' <summary>
        ''' Updates the EditingObject using data in UI elements.
        ''' </summary>
        Public Overrides Sub UpdateObject()
            With GetEditingObject(Of SkySave)()
                .OriginalPlayerName = txtPlayerName.Text
                .OriginalPartnerName = txtPartnerName.Text
                .OriginalPlayerID = SelectedPlayerID
                .OriginalPartnerID = SelectedPartnerID
                .OriginalPlayerIsFemale = chbPlayerGender.IsChecked
                .OriginalPartnerIsFemale = chbPartnerGender.IsChecked
            End With
        End Sub

        Private Sub GeneralTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = My.Resources.Language.History
        End Sub

        Private Sub OnModified(sender As Object, e As EventArgs) Handles txtPartnerName.TextChanged, txtPlayerName.TextChanged, cbPartner.SelectionChanged, cbPlayer.SelectionChanged
            IsModified = True
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(Saves.SkySave)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 5
        End Function

    End Class
End Namespace