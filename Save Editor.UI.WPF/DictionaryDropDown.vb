Imports System.ComponentModel
Imports SkyEditor.UI.WPF

Public Class DictionaryDropDown
    Inherits SearchableDropDown
    Implements INotifyPropertyChanged

    Public Shared ReadOnly PokemonDictionaryProperty As DependencyProperty = DependencyProperty.Register(NameOf(ItemDictionary), GetType(IDictionary(Of Integer, String)), GetType(DictionaryDropDown), New FrameworkPropertyMetadata(AddressOf OnPokemonDictionaryChanged))
    Public Shared ReadOnly SelectedPokemonIDProperty As DependencyProperty = DependencyProperty.Register(NameOf(SelectedItemID), GetType(Integer), GetType(DictionaryDropDown), New FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, AddressOf OnSelectedPokemonIDChanged))

    Private Shared Sub OnPokemonDictionaryChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        DirectCast(d, DictionaryDropDown).ItemDictionary = e.NewValue
    End Sub
    Private Shared Sub OnSelectedPokemonIDChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
        DirectCast(d, DictionaryDropDown).SelectedItemID = e.NewValue
    End Sub

    Private Sub DictionaryDropDown_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles Me.SelectionChanged
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SelectedItemID)))
    End Sub

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property ItemDictionary As IDictionary(Of Integer, String)
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

    Public Property SelectedItemID As Integer
        Get
            Return DirectCast(SelectedItem, GenericListItem(Of Integer)).Value
        End Get
        Set(value As Integer)
            For Each item In Items
                If DirectCast(item, GenericListItem(Of Integer)).Value = value Then
                    SelectedItem = item
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(SelectedItemID))
                    Exit For
                End If
            Next
        End Set
    End Property
End Class
