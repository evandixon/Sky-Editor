Imports SkyEditor.Interfaces
Imports SkyEditorBase

Namespace Saves
    Partial Class RBSaveEU
        Implements Interfaces.iPokemonStorage
        Public Property StoredPokemon(Index As Integer) As Saves.RBSave.StoredPkm
            Get
                Return New Saves.RBSave.StoredPkm(Me.Bits.Range(Offsets.StoredPokemonOffset + Index * Offsets.StoredPokemonLength, Offsets.StoredPokemonLength))
            End Get
            Set(value As Saves.RBSave.StoredPkm)
                Me.Bits.Range(Offsets.StoredPokemonOffset + Index * Offsets.StoredPokemonLength, Offsets.StoredPokemonLength) = value
            End Set
        End Property
        Public Property StoredPokemon() As Saves.RBSave.StoredPkm()
            Get
                Dim output As New List(Of Saves.RBSave.StoredPkm)
                For count As Integer = 0 To Offsets.StoredPokemonNumber - 1
                    Dim i = StoredPokemon(count)
                    'If i.IsValid OrElse count < 5 Then 'Excepting when count < 5 because the first 4 pokemon slots are special
                    output.Add(i)
                    'End If
                Next
                Return output.ToArray
            End Get
            Set(value As Saves.RBSave.StoredPkm())
                For count As Integer = 0 To Offsets.StoredPokemonNumber - 1
                    If value.Length > count Then
                        StoredPokemon(count) = value(count)
                    Else
                        StoredPokemon(count) = New Saves.RBSave.StoredPkm(New Binary(Offsets.StoredPokemonLength))
                    End If
                Next
            End Set
        End Property

        Public Function GetPokemon() As iMDPkm() Implements iPokemonStorage.GetPokemon
            Return StoredPokemon
        End Function

        Public Sub SetPokemon(Pokemon() As iMDPkm) Implements iPokemonStorage.SetPokemon
            StoredPokemon = Pokemon
        End Sub

        Public Function GetStoredPokemonOffsets() As StoredPokemonSlotDefinition() Implements iPokemonStorage.GetStoredPokemonOffsets
            Return StoredPokemonSlotDefinition.FromLines(IO.File.ReadAllText(SkyEditorBase.PluginHelper.GetResourceName(SettingsManager.Instance.Settings.CurrentLanguage & "\RBFriendAreaOffsets.txt"))).ToArray
        End Function
    End Class

End Namespace
