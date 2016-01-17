Imports SkyEditorBase

Namespace Tabs
    Public Class PKMQuicksaveTab
        Inherits ObjectTab
        Private WriteOnly Property PokemonDictionary As IDictionary(Of Integer, String)
            Set(value As IDictionary(Of Integer, String))
                cbPokemon.Items.Clear()
                cbPokemon2.Items.Clear()
                For Each item In (From v In value Order By v.Value)
                    cbPokemon.Items.Add(New Utilities.GenericListItem(Of Integer)(item.Value, item.Key))
                    cbPokemon2.Items.Add(New Utilities.GenericListItem(Of Integer)(item.Value, item.Key))
                Next
            End Set
        End Property
        Private Property SelectedPokemonID As Integer
            Get
                Return DirectCast(cbPokemon.SelectedItem, Utilities.GenericListItem(Of Integer)).Value
            End Get
            Set(value As Integer)
                For Each item In cbPokemon.Items
                    If DirectCast(item, Utilities.GenericListItem(Of Integer)).Value = value Then
                        cbPokemon.SelectedItem = item
                    End If
                Next
            End Set
        End Property
        Private Property SelectedPokemonID2 As Integer
            Get
                Return DirectCast(cbPokemon2.SelectedItem, Utilities.GenericListItem(Of Integer)).Value
            End Get
            Set(value As Integer)
                For Each item In cbPokemon.Items
                    If DirectCast(item, Utilities.GenericListItem(Of Integer)).Value = value Then
                        cbPokemon2.SelectedItem = item
                    End If
                Next
            End Set
        End Property
        Public Overrides Sub RefreshDisplay()
            Dim _pokemon = DirectCast(Me.EditingObject, Saves.SkySave.QuicksavePkm)
            With _pokemon
                PokemonDictionary = Lists.SkyPokemon

                SelectedPokemonID = .ID
                chbIsFemale.IsChecked = .IsFemale

                SelectedPokemonID2 = .TransformedID
                chbIsFemale2.IsChecked = .TransformedIsFemale

                numLevel.Value = .Level
                numExp.Value = .Exp

                numCurrentHP.Value = _pokemon.CurrentHP
                numMaxHP.Value = .BaseHP
                numAttack.Value = .StatAttack
                numDefense.Value = .StatDefense
                numSpAttack.Value = .StatSpAttack
                numSpDefense.Value = .StatSpDefense
            End With
        End Sub

        Public Overrides Sub UpdateObject()
            Dim _pokemon = DirectCast(Me.EditingObject, Saves.SkySave.QuicksavePkm)
            With _pokemon
                .ID = SelectedPokemonID
                _pokemon.IsFemale = chbIsFemale.IsChecked

                .TransformedID = SelectedPokemonID2
                _pokemon.TransformedIsFemale = chbIsFemale2.IsChecked


                .Level = numLevel.Value
                .Exp = numExp.Value

                _pokemon.CurrentHP = numCurrentHP.Value
                .BaseHP = numMaxHP.Value
                .StatAttack = numAttack.Value
                .StatDefense = numDefense.Value
                .StatSpAttack = numSpAttack.Value
                .StatSpDefense = numSpDefense.Value
            End With
        End Sub
        Public Overrides ReadOnly Property SupportedTypes As Type()
            Get
                Return {GetType(Saves.SkySave.QuicksavePkm)}
            End Get
        End Property

        Private Sub PKMGeneralTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("General")
        End Sub
        Public Overrides ReadOnly Property SortOrder As Integer
            Get
                Return 10
            End Get
        End Property

        Private Sub OnModified(sender As Object, e As EventArgs) Handles cbPokemon.SelectionChanged,
                                                                        cbPokemon2.SelectionChanged,
                                                                        chbIsFemale.Checked, chbIsFemale.Unchecked,
                                                                        chbIsFemale2.Checked, chbIsFemale2.Unchecked,
                                                                        numLevel.ValueChanged,
                                                                        numExp.ValueChanged,
                                                                        numCurrentHP.ValueChanged,
                                                                        numMaxHP.ValueChanged,
                                                                        numAttack.ValueChanged,
                                                                        numSpAttack.ValueChanged,
                                                                        numDefense.ValueChanged,
                                                                        numSpDefense.ValueChanged
            RaiseModified()
        End Sub
    End Class

End Namespace