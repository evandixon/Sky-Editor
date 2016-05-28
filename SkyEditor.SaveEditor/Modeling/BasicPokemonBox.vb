Namespace Modeling
    Public Class BasicPokemonBox
        Implements IPokemonBox

        Public Property ItemCollection As IEnumerable Implements IPokemonBox.ItemCollection

        Public Property Name As String Implements IPokemonBox.Name
        Public Property SelectedPokemon As Object

        Public Sub New(name As String, itemCollection As IEnumerable)
            Me.Name = name
            Me.ItemCollection = itemCollection
        End Sub
    End Class
End Namespace

