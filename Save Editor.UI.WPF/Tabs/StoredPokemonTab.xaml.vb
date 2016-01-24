Imports SkyEditorBase
Imports SaveEditor.Interfaces
Imports SkyEditorBase.Interfaces

Namespace Tabs
    Public Class StoredPokemonTab
        Inherits UserControl
        Implements iObjectControl
        Dim storage As iPokemonStorage
        Dim pokemon As iMDPkm()
        Dim slots As StoredPokemonSlotDefinition()
        Private Sub gbPokemon_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles lbPokemon.MouseDoubleClick
            ShowPkmEditDialog()
        End Sub
        Private Sub btnEditPokemon_Click(sender As Object, e As RoutedEventArgs) Handles btnEditPokemon.Click
            ShowPkmEditDialog()
        End Sub
        Sub ShowPkmEditDialog()
            If lbPokemon.SelectedIndex > -1 Then
                Dim w As New SkyEditorWPF.ObjectWindow
                w.ObjectToEdit = lbPokemon.SelectedItem
                w.ShowDialog()
                lbPokemon.SelectedItem = w.ObjectToEdit
                IsModified = True
                RefreshPKMDisplay()
            End If
        End Sub
        Sub RefreshPKMDisplay()
            Dim pkms As New List(Of Object)
            For Each p In lbPokemon.Items
                pkms.Add(p)
            Next
            lbPokemon.Items.Clear()
            For count As Integer = 0 To pkms.Count - 1
                'If pkms(count).isvalid Then
                lbPokemon.Items.Add(pkms(count))
                'End If
            Next
            'ChangeHeader()
        End Sub

        Public Sub RefreshDisplay()
            storage = GetEditingObject(Of iPokemonStorage)()
            pokemon = storage.GetPokemon
            slots = storage.GetStoredPokemonOffsets
            RefreshSlotDisplay()
            ShowSlot(-1)
            isRefresh = False
        End Sub
        Private Sub ShowSlot(Index As Integer)
            lbPokemon.Items.Clear()
            If Index > -1 Then
                For count As Integer = slots(Index).Index To slots(Index).Index + slots(Index).Length - 1
                    lbPokemon.Items.Add(pokemon(count))
                Next
            Else
                For count As Integer = 0 To pokemon.Length - 1
                    lbPokemon.Items.Add(pokemon(count))
                Next
            End If
        End Sub
        Private Sub SaveSlot(Index As Integer)
            If Index > -1 Then
                For count As Integer = 0 To slots(Index).Length - 1
                    pokemon(slots(Index).Index + count) = lbPokemon.Items(count)
                Next
            Else
                For count As Integer = 0 To lbPokemon.Items.Count - 1
                    pokemon(count) = lbPokemon.Items(count)
                Next
            End If
        End Sub
        Private Sub RefreshSlotDisplay()
            Dim index As Integer = lbFriendArea.SelectedIndex
            isRefresh = True
            lbFriendArea.Items.Clear()
            For Each item In slots
                Dim number As Integer = 0
                For count As Integer = item.Index To item.Index + item.Length - 1
                    If pokemon(count).IsValid Then
                        number += 1
                    End If
                Next
                lbFriendArea.Items.Add(item.Name & String.Format(" ({0}/{1})", number, item.Length))
            Next
            isRefresh = True
            lbFriendArea.SelectedIndex = index
        End Sub
        Dim oldSlot As Integer = -1
        Dim isRefresh As Boolean = False
        Private Sub lbFriendArea_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lbFriendArea.SelectionChanged
            If Not isRefresh Then
                If oldSlot > -1 Then
                    SaveSlot(oldSlot)
                End If
                ShowSlot(lbFriendArea.SelectedIndex)
                oldSlot = lbFriendArea.SelectedIndex
                isRefresh = True
                RefreshSlotDisplay()
            Else
                isRefresh = False
            End If
        End Sub

        Public Sub UpdateObject()
            SaveSlot(lbFriendArea.SelectedIndex)
            GetEditingObject(Of iPokemonStorage).SetPokemon(pokemon)
        End Sub

        Private Sub StoredPokemonTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("Stored Pokemon")
            btnShowAll.Content = PluginHelper.GetLanguageItem("Show All")
            btnEditPokemon.Content = PluginHelper.GetLanguageItem("Edit")
        End Sub
        'Sub ChangeHeader(Optional RefreshFriendArea As Boolean = True)
        '    'load language
        '    btnEditPokemon.Content = PluginHelper.GetLanguageItem("Edit")
        '    btnShowAll.Content = PluginHelper.GetLanguageItem("ShowAll")

        '    If RefreshFriendArea Then
        '        Dim selectedFriendArea As Integer = lbFriendArea.SelectedIndex
        '        If selectedFriendArea > -1 Then
        '            Dim olditem = DirectCast(lbFriendArea.SelectedItem, RBSave.RBFriendAreaOffsetDefinition)
        '            For count As Integer = olditem.Index To (olditem.Index + olditem.Length) - 1
        '                Storage(count) = lbPokemon.Items(count - olditem.Index)
        '            Next
        '        End If
        '        lbFriendArea.Items.Clear()
        '        Dim friendAreas As List(Of RBSave.RBFriendAreaOffsetDefinition)
        '        If isSkySave Then
        '            friendAreas = RBSave.RBFriendAreaOffsetDefinition.FromLines(IO.File.ReadAllText(PluginHelper.GetResourceName(Settings.CurrentLanguage & "\SkyFriendAreaOffsets.txt")))
        '        ElseIf isTDSave Then
        '            friendAreas = RBSave.RBFriendAreaOffsetDefinition.FromLines(IO.File.ReadAllText(PluginHelper.GetResourceName(Settings.CurrentLanguage & "\TDFriendAreaOffsets.txt")))
        '        ElseIf isRBSave Then
        '            friendAreas = RBSave.RBFriendAreaOffsetDefinition.FromLines(IO.File.ReadAllText(PluginHelper.GetResourceName(Settings.CurrentLanguage & "\RBFriendAreaOffsets.txt")))
        '        Else
        '            friendAreas = RBSave.RBFriendAreaOffsetDefinition.FromLines(IO.File.ReadAllText(PluginHelper.GetResourceName(Settings.CurrentLanguage & "\TDFriendAreaOffsets.txt")))
        '        End If
        '        For Each item In friendAreas
        '            Dim pkm As Integer = 0
        '            For count As Integer = item.Index To item.Index + item.Length - 1
        '                If Storage.Count > count AndAlso Storage(count).GetIsValid Then pkm += 1
        '            Next
        '            item.CurrentPokemon = pkm
        '            lbFriendArea.Items.Add(item)
        '        Next
        '        lbFriendArea.SelectedIndex = selectedFriendArea
        '    End If
        '    If lbFriendArea.SelectedIndex > -1 Then
        '        Me.Header = String.Format(PluginHelper.GetLanguageItem("RBStoredPokemon", "Stored Pokemon ({0})"), lbFriendArea.SelectedItem)
        '    Else
        '        Me.Header = String.Format(PluginHelper.GetLanguageItem("RBStoredPokemon", "Stored Pokemon ({0})"), "All")
        '    End If
        'End Sub

        'Private Sub lbFriendArea_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lbFriendArea.SelectionChanged
        '    'save the old ones
        '    If e.RemovedItems.Count > 0 Then
        '        Dim olditem = DirectCast(e.RemovedItems(0), RBSave.RBFriendAreaOffsetDefinition)
        '        For count As Integer = olditem.Index To (olditem.Index + olditem.Length) - 1
        '            Storage(count) = lbPokemon.Items(count - olditem.Index)
        '        Next
        '    Else
        '        For count As Integer = 0 To Storage.Count - 1
        '            Storage(count) = lbPokemon.Items(count)
        '        Next
        '    End If
        '    'update the new ones
        '    lbPokemon.Items.Clear()
        '    If e.AddedItems.Count > 0 Then
        '        Dim newitem = DirectCast(e.AddedItems(0), RBSave.RBFriendAreaOffsetDefinition)
        '        For count As Integer = newitem.Index To (newitem.Index + newitem.Length) - 1
        '            lbPokemon.Items.Add(Storage(count))
        '        Next
        '    Else
        '        For count As Integer = 0 To Storage.Count - 1
        '            lbPokemon.Items.Add(Storage(count))
        '        Next
        '    End If
        '    ChangeHeader(False)
        'End Sub

        'Private Sub btnShowAll_Click(sender As Object, e As RoutedEventArgs) Handles btnShowAll.Click
        '    lbFriendArea.SelectedIndex = -1
        'End Sub

        Private Sub btnShowAll_Click(sender As Object, e As RoutedEventArgs) Handles btnShowAll.Click
            lbFriendArea.SelectedIndex = -1
        End Sub

        Public Function GetSupportedTypes() As IEnumerable(Of Type) Implements iObjectControl.GetSupportedTypes
            Return {GetType(iPokemonStorage)}
        End Function

        Public Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer Implements iObjectControl.GetSortOrder
            Return 4
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