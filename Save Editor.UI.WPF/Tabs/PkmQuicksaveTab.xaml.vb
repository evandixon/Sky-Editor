Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace Tabs
    Public Class PKMQuicksaveTab
        Inherits UserControl
        Implements iObjectControl
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
        Public Sub RefreshDisplay()
            Dim _pokemon = GetEditingObject(Of Saves.SkySave.QuicksavePkm)()

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

        Public Sub UpdateObject()
            Dim _pokemon = GetEditingObject(Of Saves.SkySave.QuicksavePkm)()

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

        Private Sub PKMGeneralTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("General")
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

        Public Function GetSupportedTypes() As IEnumerable(Of Type) Implements iObjectControl.GetSupportedTypes
            Return {GetType(Saves.SkySave.QuicksavePkm)}
        End Function

        Public Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer Implements iObjectControl.GetSortOrder
            Return 10
        End Function

#Region "IObjectControl Support"

        Public Function SupportsObject(Obj As Object) As Boolean Implements iObjectControl.SupportsObject
            Return True
        End Function

        Public Function IsBackupControl(Obj As Object) As Boolean Implements iObjectControl.IsBackupControl
            Return False
        End Function
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