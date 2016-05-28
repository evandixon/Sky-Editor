Imports System.ComponentModel

Public Class IPokemonStorageView
    Implements INotifyPropertyChanged
    Public Shared ReadOnly ObjectToEditProperty As DependencyProperty = DependencyProperty.Register(NameOf(SelectedPokemon), GetType(Object), GetType(IPokemonStorageView), New FrameworkPropertyMetadata(AddressOf OnSelectedPokemonChanged))

    Private Shared Sub OnSelectedPokemonChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        DirectCast(d, IPokemonStorageView).SelectedPokemon = e.NewValue
    End Sub

    Public Property SelectedPokemon As Object

End Class
