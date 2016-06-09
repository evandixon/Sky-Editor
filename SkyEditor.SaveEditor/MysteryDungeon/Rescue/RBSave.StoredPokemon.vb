Imports SkyEditor.SaveEditor.Interfaces
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO

Namespace MysteryDungeon.Rescue
    Partial Class RBSave
        Implements iPokemonStorageOld

        Public Property StoredPokemon(Index As Integer) As rbstoredpokemon
            Get
                Return New RBStoredPokemon(Me.Bits.Range(Offsets.StoredPokemonOffset + Index * Offsets.StoredPokemonLength, Offsets.StoredPokemonLength))
            End Get
            Set(value As RBStoredPokemon)
                Me.Bits.Range(Offsets.StoredPokemonOffset + Index * Offsets.StoredPokemonLength, Offsets.StoredPokemonLength) = value.GetStoredPokemonBits
            End Set
        End Property
        Public Property StoredPokemon() As RBStoredPokemon()
            Get
                Dim output As New List(Of RBStoredPokemon)
                For count As Integer = 0 To Offsets.StoredPokemonNumber - 1
                    Dim i = StoredPokemon(count)
                    'If i.IsValid OrElse count < 5 Then 'Excepting when count < 5 because the first 4 pokemon slots are special
                    output.Add(i)
                    'End If
                Next
                Return output.ToArray
            End Get
            Set(value As RBStoredPokemon())
                For count As Integer = 0 To Offsets.StoredPokemonNumber - 1
                    If value.Length > count Then
                        StoredPokemon(count) = value(count)
                    Else
                        StoredPokemon(count) = New RBStoredPokemon(New Binary(Offsets.StoredPokemonLength))
                    End If
                Next
            End Set
        End Property

        Public Function GetPokemon() As iMDPkm() Implements iPokemonStorageOld.GetPokemon
            Return StoredPokemon
        End Function

        Public Sub SetPokemon(Pokemon() As iMDPkm) Implements iPokemonStorageOld.SetPokemon
            StoredPokemon = Pokemon
        End Sub

        Public Function GetStoredPokemonOffsets() As StoredPokemonSlotDefinition() Implements iPokemonStorageOld.GetStoredPokemonOffsets
            Return StoredPokemonSlotDefinition.FromLines(My.Resources.ListResources.RBFriendAreaOffsets).ToArray
        End Function
    End Class

End Namespace