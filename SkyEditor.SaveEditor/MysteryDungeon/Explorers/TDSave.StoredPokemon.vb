Imports SkyEditor.SaveEditor.Interfaces
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO

Namespace MysteryDungeon.Explorers
    Partial Class TDSave
        Implements iPokemonStorageOld

        Public Property StoredPokemon(Index As Integer) As TDStoredPokemon
            Get
                Return New TDStoredPokemon(Me.Bits.Range(Offsets.StoredPokemonOffset + Index * Offsets.StoredPokemonLength, Offsets.StoredPokemonLength))
            End Get
            Set(value As TDStoredPokemon)
                Me.Bits.Range(Offsets.StoredPokemonOffset + Index * Offsets.StoredPokemonLength, Offsets.StoredPokemonLength) = value.GetStoredPokemonBits
            End Set
        End Property
        Public Property StoredPokemon() As TDStoredPokemon()
            Get
                Dim output As New List(Of TDStoredPokemon)
                For count As Integer = 0 To Offsets.StoredPokemonNumber - 1
                    Dim i = StoredPokemon(count)
                    'If i.IsValid OrElse count < 9 Then 'Excepting when count < 9 because the first 8 pokemon slots are special
                    output.Add(i)
                    'End If
                Next
                Return output.ToArray
            End Get
            Set(value As TDStoredPokemon())
                For count As Integer = 0 To Offsets.StoredPokemonNumber - 1
                    If value.Length > count Then
                        StoredPokemon(count) = value(count)
                    Else
                        StoredPokemon(count) = New TDStoredPokemon(New Binary(Offsets.StoredPokemonLength))
                    End If
                Next
            End Set
        End Property

        Public Function GetPokemon() As iMDPkm() Implements iPokemonStorageOld.GetPokemon
            Return StoredPokemon
        End Function

        Public Function GetStoredPokemonOffsets() As StoredPokemonSlotDefinition() Implements iPokemonStorageOld.GetStoredPokemonOffsets
            Return StoredPokemonSlotDefinition.FromLines(My.Resources.ListResources.TDFriendAreaOffsets).ToArray
        End Function

        Public Sub SetPokemon(Pokemon() As iMDPkm) Implements iPokemonStorageOld.SetPokemon
            StoredPokemon = Pokemon
        End Sub
    End Class

End Namespace