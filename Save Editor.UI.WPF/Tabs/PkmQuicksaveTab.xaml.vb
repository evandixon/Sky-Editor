﻿Imports SkyEditor.SaveEditor.MysteryDungeon.Explorers
Imports SkyEditor.UI.WPF
Imports SkyEditorBase
Imports SkyEditorWPF.UI

Namespace Tabs
    Public Class PKMQuicksaveTab
        Inherits SkyEditorWPF.UI.ObjectControl
        Private WriteOnly Property PokemonDictionary As IDictionary(Of Integer, String)
            Set(value As IDictionary(Of Integer, String))
                cbPokemon.Items.Clear()
                cbPokemon2.Items.Clear()
                For Each item In (From v In value Order By v.Value)
                    cbPokemon.Items.Add(New GenericListItem(Of Integer)(item.Value, item.Key))
                    cbPokemon2.Items.Add(New GenericListItem(Of Integer)(item.Value, item.Key))
                Next
            End Set
        End Property
        Private Property SelectedPokemonID As Integer
            Get
                Return DirectCast(cbPokemon.SelectedItem, GenericListItem(Of Integer)).Value
            End Get
            Set(value As Integer)
                For Each item In cbPokemon.Items
                    If DirectCast(item, GenericListItem(Of Integer)).Value = value Then
                        cbPokemon.SelectedItem = item
                    End If
                Next
            End Set
        End Property
        Private Property SelectedPokemonID2 As Integer
            Get
                Return DirectCast(cbPokemon2.SelectedItem, GenericListItem(Of Integer)).Value
            End Get
            Set(value As Integer)
                For Each item In cbPokemon.Items
                    If DirectCast(item, GenericListItem(Of Integer)).Value = value Then
                        cbPokemon2.SelectedItem = item
                    End If
                Next
            End Set
        End Property
        Public Overrides Sub RefreshDisplay()
            Dim _pokemon = GetEditingObject(Of SkyQuicksavePokemon)()

            With _pokemon
                PokemonDictionary = Lists.ExplorersPokemon

                SelectedPokemonID = .ID
                chbIsFemale.IsChecked = .IsFemale

                SelectedPokemonID2 = .TransformedID
                chbIsFemale2.IsChecked = .TransformedIsFemale

                numLevel.Value = .Level
                numExp.Value = .Exp

                numCurrentHP.Value = _pokemon.CurrentHP
                numMaxHP.Value = .MaxHP
                numAttack.Value = .Attack
                numDefense.Value = .Defense
                numSpAttack.Value = .SpAttack
                numSpDefense.Value = .SpDefense
            End With
        End Sub

        Public Overrides Sub UpdateObject()
            Dim _pokemon = GetEditingObject(Of SkyQuicksavePokemon)()

            With _pokemon
                .ID = SelectedPokemonID
                _pokemon.IsFemale = chbIsFemale.IsChecked

                .TransformedID = SelectedPokemonID2
                _pokemon.TransformedIsFemale = chbIsFemale2.IsChecked


                .Level = numLevel.Value
                .Exp = numExp.Value

                _pokemon.CurrentHP = numCurrentHP.Value
                .MaxHP = numMaxHP.Value
                .Attack = numAttack.Value
                .Defense = numDefense.Value
                .SpAttack = numSpAttack.Value
                .SpDefense = numSpDefense.Value
            End With
        End Sub

        Private Sub PKMGeneralTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = My.Resources.Language.General
        End Sub

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
            IsModified = True
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(SkyQuicksavePokemon)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 10
        End Function

    End Class

End Namespace