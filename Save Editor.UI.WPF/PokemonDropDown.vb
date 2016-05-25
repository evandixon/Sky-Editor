Imports SkyEditor.UI.WPF

Public Class PokemonDropDown
    Inherits SearchableDropDown
    Private WriteOnly Property PokemonDictionary As IDictionary(Of Integer, String)
        Set(value As IDictionary(Of Integer, String))
            Items.Clear()

            For Each item In (From v In value Order By v.Value)
                Items.Add(New GenericListItem(Of Integer)(item.Value, item.Key))
            Next

            SelectedIndex = 0
        End Set
    End Property

    Public Property SelectedPokemonID As Integer
        Get
            Return DirectCast(SelectedItem, GenericListItem(Of Integer)).Value
        End Get
        Set(value As Integer)
            For Each item In Items
                If DirectCast(item, GenericListItem(Of Integer)).Value = value Then
                    SelectedItem = item
                End If
            Next
        End Set
    End Property
End Class
