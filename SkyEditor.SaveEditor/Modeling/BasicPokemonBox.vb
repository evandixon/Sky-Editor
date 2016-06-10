Imports System.Text

Namespace Modeling
    Public Class BasicPokemonBox
        Implements IPokemonBox

        Public Property ItemCollection As IEnumerable Implements IPokemonBox.ItemCollection

        Public Property Name As String Implements IPokemonBox.Name
        Public Property SelectedPokemon As Object

        Public Overrides Function ToString() As String
            Dim out As New StringBuilder
            out.Append(Name)

            If TypeOf ItemCollection Is IList Then
                out.Append(" (")
                out.Append(DirectCast(ItemCollection, IList).Count.ToString)
                out.Append(")")
            End If

            Return out.ToString
        End Function

        Public Sub New(name As String, itemCollection As IEnumerable)
            Me.Name = name
            Me.ItemCollection = itemCollection
        End Sub
    End Class
End Namespace

