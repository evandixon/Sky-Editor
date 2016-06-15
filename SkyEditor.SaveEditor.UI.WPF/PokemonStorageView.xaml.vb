Imports System.ComponentModel
Imports SkyEditor.SaveEditor.Modeling

Public Class PokemonStorageView
    Implements INotifyPropertyChanged
    Public Shared ReadOnly ObjectToEditProperty As DependencyProperty = DependencyProperty.Register(NameOf(SelectedPokemon), GetType(Object), GetType(PokemonStorageView), New FrameworkPropertyMetadata(AddressOf OnSelectedPokemonChanged))

    Private Shared Sub OnSelectedPokemonChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        DirectCast(d, PokemonStorageView).SelectedPokemon = e.NewValue
    End Sub

    Public Property SelectedPokemon As Object

End Class
