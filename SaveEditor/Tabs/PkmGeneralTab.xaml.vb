Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace Tabs
    Public Class PKMGeneralTab
        Inherits UserControl
        Implements iObjectControl
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
        Public Sub RefreshDisplay()
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
        End Sub

        Public Sub UpdateObject()
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
            Me.Header = PluginHelper.GetLanguageItem("General")
            lblPkm.Content = PluginHelper.GetLanguageItem("Pokémon")
            lblIsFemale.Content = PluginHelper.GetLanguageItem("Is Female")
            lblName.Content = PluginHelper.GetLanguageItem("Name")
            lblLevel.Content = PluginHelper.GetLanguageItem("Level")
            lblExp.Content = PluginHelper.GetLanguageItem("Experience")
            lblMetAt.Content = PluginHelper.GetLanguageItem("Met At")
            lblMetFloor.Content = PluginHelper.GetLanguageItem("Met Floor")
            lblIQ.Content = PluginHelper.GetLanguageItem("IQ")
            lblCurrentHP.Content = PluginHelper.GetLanguageItem("Current HP")
            lblMaxHP.Content = PluginHelper.GetLanguageItem("Max HP")
            lblAttack.Content = PluginHelper.GetLanguageItem("Attack")
            lblSpAttack.Content = PluginHelper.GetLanguageItem("Sp. Attack")
            lblDefense.Content = PluginHelper.GetLanguageItem("Defense")
            lblSpDefense.Content = PluginHelper.GetLanguageItem("Sp. Defense")
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

        Public Function GetSupportedTypes() As IEnumerable(Of Type) Implements iObjectControl.GetSupportedTypes
            Return {GetType(Interfaces.iMDPkm)}
        End Function

        Public Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer Implements iObjectControl.GetSortOrder
            Return 0
        End Function

#Region "IObjectControl Support"
        ''' <summary>
        ''' Called when Header is changed.
        ''' </summary>
        Public Event HeaderUpdated As iObjectControl.HeaderUpdatedEventHandler Implements iObjectControl.HeaderUpdated

        ''' <summary>
        ''' Called when IsModified is changed.
        ''' </summary>
        Public Event IsModifiedChanged As iObjectControl.IsModifiedChangedEventHandler Implements iObjectControl.IsModifiedChanged

        ''' <summary>
        ''' Returns the value of the Header.  Only used when the iObjectControl is behaving as a tab.
        ''' </summary>
        ''' <returns></returns>
        Public Property Header As String Implements iObjectControl.Header
            Get
                Return _header
            End Get
            Set(value As String)
                Dim oldValue = _header
                _header = value
                RaiseEvent HeaderUpdated(Me, New EventArguments.HeaderUpdatedEventArgs(oldValue, value))
            End Set
        End Property
        Dim _header As String

        ''' <summary>
        ''' Returns the current EditingObject, after casting it to type T.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <returns></returns>
        Protected Function GetEditingObject(Of T)() As T
            Return PluginHelper.Cast(Of T)(_editingObject)
        End Function

        ''' <summary>
        ''' Returns the current EditingObject.
        ''' It is recommended to use GetEditingObject(Of T), since it returns iContainter(Of T).Item if the EditingObject implements that interface.
        ''' </summary>
        ''' <returns></returns>
        Protected Function GetEditingObject() As Object
            Return _editingObject
        End Function

        ''' <summary>
        ''' The way to get the EditingObject from outside this class.  Refreshes the display on set, and updates the object on get.
        ''' Calling this from inside this class could result in a stack overflow, especially if called from UpdateObject, so use GetEditingObject or GetEditingObject(Of T) instead.
        ''' </summary>
        ''' <returns></returns>
        Public Property EditingObject As Object Implements iObjectControl.EditingObject
            Get
                UpdateObject()
                Return _editingObject
            End Get
            Set(value As Object)
                _editingObject = value
                RefreshDisplay()
            End Set
        End Property
        Dim _editingObject As Object

        ''' <summary>
        ''' Whether or not the EditingObject has been modified without saving.
        ''' Set to true when the user changes anything in the GUI.
        ''' Set to false when the object is saved, or if the user undoes every change.
        ''' </summary>
        ''' <returns></returns>
        Public Property IsModified As Boolean Implements iObjectControl.IsModified
            Get
                Return _isModified
            End Get
            Set(value As Boolean)
                Dim oldValue As Boolean = _isModified
                _isModified = value
                If Not oldValue = _isModified Then
                    RaiseEvent IsModifiedChanged(Me, New EventArgs)
                End If
            End Set
        End Property
        Dim _isModified As Boolean
#End Region

    End Class

End Namespace