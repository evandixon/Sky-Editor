Imports SkyEditorBase
Imports SkyEditor.SaveEditor
Imports SkyEditorWPF.UI

Namespace Tabs
    Public Class PKMGeneralTab
        Inherits ObjectControl
        Private WriteOnly Property PokemonDictionary As IDictionary(Of Integer, String)
            Set(value As IDictionary(Of Integer, String))
                cbPokemon.Items.Clear()
                For Each item In (From v In value Order By v.Value)
                    cbPokemon.Items.Add(New Utilities.GenericListItem(Of Integer)(item.Value, item.Key))
                Next
            End Set
        End Property
        Private WriteOnly Property MetAtDictionary As IDictionary(Of Integer, String)
            Set(value As IDictionary(Of Integer, String))
                cbMetAt.Items.Clear()
                For Each item In (From v In value Order By v.Value)
                    cbMetAt.Items.Add(New Utilities.GenericListItem(Of Integer)(item.Value, item.Key))
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
        Private Property SelectedMetAtID As Integer
            Get
                Return DirectCast(cbMetAt.SelectedItem, Utilities.GenericListItem(Of Integer)).Value
            End Get
            Set(value As Integer)
                For Each item In cbMetAt.Items
                    If DirectCast(item, Utilities.GenericListItem(Of Integer)).Value = value Then
                        cbMetAt.SelectedItem = item
                    End If
                Next
            End Set
        End Property
        Public Overrides Sub RefreshDisplay()
            Dim _pokemon = GetEditingObject(Of Interfaces.iMDPkm)()

            With _pokemon
                PokemonDictionary = .GetPokemonDictionary
                MetAtDictionary = .GetMetAtDictionary

                SelectedPokemonID = .ID

                If TypeOf _pokemon Is Interfaces.iMDPkmGender Then
                    lblIsFemale.Visibility = Windows.Visibility.Visible
                    chbIsFemale.Visibility = Windows.Visibility.Visible
                    chbIsFemale.IsChecked = DirectCast(_pokemon, Interfaces.iMDPkmGender).IsFemale
                Else
                    lblIsFemale.Visibility = Windows.Visibility.Collapsed
                    chbIsFemale.Visibility = Windows.Visibility.Collapsed
                End If

                txtName.Text = .Name
                numLevel.Value = .Level
                numExp.Value = .Exp
                SelectedMetAtID = .MetAt

                If TypeOf _pokemon Is Interfaces.iMDPkmMetFloor Then
                    lblMetFloor.Visibility = Windows.Visibility.Visible
                    numMetFloor.Visibility = Windows.Visibility.Visible
                    numMetFloor.Value = DirectCast(_pokemon, Interfaces.iMDPkmMetFloor).MetFloor
                Else
                    lblMetFloor.Visibility = Windows.Visibility.Collapsed
                    numMetFloor.Visibility = Windows.Visibility.Collapsed
                End If

                If TypeOf _pokemon Is Interfaces.iMDPkmIQ Then
                    lblIQ.Visibility = Windows.Visibility.Visible
                    numIQ.Visibility = Windows.Visibility.Visible
                    numIQ.Value = DirectCast(_pokemon, Interfaces.iMDPkmIQ).IQ
                Else
                    lblIQ.Visibility = Windows.Visibility.Collapsed
                    numIQ.Visibility = Windows.Visibility.Collapsed
                End If

                If TypeOf _pokemon Is Interfaces.iMDPkmCurrentHP Then
                    lblCurrentHP.Visibility = Windows.Visibility.Visible
                    numCurrentHP.Visibility = Windows.Visibility.Visible
                    numCurrentHP.Value = DirectCast(_pokemon, Interfaces.iMDPkmCurrentHP).CurrentHP
                Else
                    lblCurrentHP.Visibility = Windows.Visibility.Collapsed
                    numCurrentHP.Visibility = Windows.Visibility.Collapsed
                End If

                numMaxHP.Value = .MaxHP
                numAttack.Value = .StatAttack
                numDefense.Value = .StatDefense
                numSpAttack.Value = .StatSpAttack
                numSpDefense.Value = .StatSpDefense

                'If TypeOf _pokemon Is Interfaces.iMDPkmSpeed Then
                '    lblSpeed.Visibility = Windows.Visibility.Visible
                '    numSpeed.Visibility = Windows.Visibility.Visible
                '    numSpeed.Value = DirectCast(_pokemon, Interfaces.iMDPkmSpeed).Speed
                'Else
                '    lblSpeed.Visibility = Windows.Visibility.Collapsed
                '    numSpeed.Visibility = Windows.Visibility.Collapsed
                'End If

            End With
            IsModified = False
        End Sub

        Public Overrides Sub UpdateObject()
            Dim _pokemon = GetEditingObject(Of Interfaces.iMDPkm)()

            With _pokemon
                .ID = SelectedPokemonID

                If TypeOf _pokemon Is Interfaces.iMDPkmGender Then
                    DirectCast(_pokemon, Interfaces.iMDPkmGender).IsFemale = chbIsFemale.IsChecked
                End If

                .Name = txtName.Text
                .Level = numLevel.Value
                .Exp = numExp.Value
                .MetAt = SelectedMetAtID

                If TypeOf _pokemon Is Interfaces.iMDPkmMetFloor Then
                    DirectCast(_pokemon, Interfaces.iMDPkmMetFloor).MetFloor = numMetFloor.Value
                End If

                If TypeOf _pokemon Is Interfaces.iMDPkmIQ Then
                    DirectCast(_pokemon, Interfaces.iMDPkmIQ).IQ = numIQ.Value
                End If

                If TypeOf _pokemon Is Interfaces.iMDPkmCurrentHP Then
                    DirectCast(_pokemon, Interfaces.iMDPkmCurrentHP).CurrentHP = numCurrentHP.Value
                End If

                .MaxHP = numMaxHP.Value
                .StatAttack = numAttack.Value
                .StatDefense = numDefense.Value
                .StatSpAttack = numSpAttack.Value
                .StatSpDefense = numSpDefense.Value
            End With
            Me.EditingObject = _pokemon
        End Sub

        Private Sub PKMGeneralTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = My.Resources.Language.General
        End Sub

        Private Sub OnModified(sender As Object, e As EventArgs) Handles cbPokemon.SelectionChanged,
                                                                           chbIsFemale.Checked,
                                                                           chbIsFemale.Unchecked,
                                                                           txtName.TextChanged,
                                                                           numLevel.ValueChanged,
                                                                           numExp.ValueChanged,
                                                                           cbMetAt.SelectionChanged,
                                                                           numMetFloor.ValueChanged,
                                                                           numIQ.ValueChanged,
                                                                           numCurrentHP.ValueChanged,
                                                                           numMaxHP.ValueChanged,
                                                                           numAttack.ValueChanged,
                                                                           numSpAttack.ValueChanged,
                                                                           numDefense.ValueChanged,
                                                                           numSpDefense.ValueChanged
            IsModified = True
        End Sub

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(Interfaces.iMDPkm)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 0
        End Function

    End Class

End Namespace