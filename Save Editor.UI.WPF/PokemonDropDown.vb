Imports SkyEditor.UI.WPF

Public Class PokemonDropDown
    Inherits SearchableDropDown

    Public Shared ReadOnly PokemonDictionaryProperty As DependencyProperty = DependencyProperty.Register("PokemonDictionary", GetType(IDictionary(Of Integer, String)), GetType(PokemonDropDown), New FrameworkPropertyMetadata(AddressOf OnPokemonDictionaryChanged))
    Public Shared ReadOnly SelectedPokemonIDProperty As DependencyProperty = DependencyProperty.Register("SelectedPokemonID", GetType(Integer), GetType(PokemonDropDown), New FrameworkPropertyMetadata(AddressOf OnSelectedPokemonIDChanged))

    Private Shared Sub OnPokemonDictionaryChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        DirectCast(d, PokemonDropDown).PokemonDictionary = e.NewValue
    End Sub
    Private Shared Sub OnSelectedPokemonIDChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        DirectCast(d, PokemonDropDown).SelectedPokemonID = e.NewValue
    End Sub

    Public Property PokemonDictionary As IDictionary(Of Integer, String)
        Get
            Return _pkmDictionary
        End Get
        Set(value As IDictionary(Of Integer, String))
            _pkmDictionary = value

            If _pkmDictionary IsNot Nothing Then
                Items.Clear()

                For Each item In (From v In value Order By v.Value)
                    Items.Add(New GenericListItem(Of Integer)(item.Value, item.Key))
                Next

                SelectedIndex = 0
            End If
        End Set
    End Property
    Dim _pkmDictionary As Dictionary(Of Integer, String)

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
