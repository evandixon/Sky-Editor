Imports System.Windows
Imports System.Windows.Controls
Imports ROMEditor
Imports ROMEditor.FileFormats
Imports ROMEditor.FileFormats.item_p
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace ObjectControls
    Public Class item_p_Editor
        Inherits UserControl
        Implements iObjectControl

        Public Sub RefreshDisplay()
            With GetEditingObject(Of item_p)()
                Items = .Items
                If IO.File.Exists(.OriginalFilename.Replace("\", "/").Replace("Items/Item Definitions", "Languages/English")) Then
                    Dim englishlanguage = New ObjectFile(Of List(Of String))(.OriginalFilename.Replace("\", "/").Replace("Items/Item Definitions", "Languages/English"))
                    ReDim ItemNames(1959)
                    englishlanguage.ContainedObject.CopyTo(6773, ItemNames, 0, 1959)
                    'Using englishLanguage = New FileFormats.LanguageString(.OriginalFilename.Replace("\", "/").Replace("Items/Item Definitions", "Languages/English"))
                    '    ReDim ItemNames(1959)
                    '    englishLanguage.Items.CopyTo(6773, ItemNames, 0, 1959)
                    'End Using
                Else
                    ItemNames = SaveEditor.Lists.SkyItemNames.Values.ToArray
                End If
            End With
            RefreshItems()
        End Sub
        Private Sub RefreshItems()
            lbItems.Items.Clear()

            For count = 0 To Items.Count - 1
                Dim p As New ItemWrapper() With {.ItemIndex = count, .Name = ItemNames(Items(count).ID)}
                lbItems.Items.Add(p)
            Next
        End Sub

        Public Sub UpdateObject()
            If lbItems.SelectedItem IsNot Nothing Then
                _oldIndex = lbItems.SelectedItem.ItemIndex
                CurrentItem = Items(lbItems.SelectedItem.ItemIndex)
            End If
            With Me.GetEditingObject(Of item_p)()
                .Items = Items
            End With
        End Sub

        Public Property CurrentItem As item_p.Item
            Get
                Dim x As New Item
                x.ID = numID.Value
                x.BuyPrice = numBuyPrice.Value
                x.SellPrice = numSellPrice.Value
                x.Category = cbCategory.SelectedValue
                x.Sprite = cbSprite.SelectedValue
                x.ItemParameter1 = cbMove.SelectedValue
                x.B10 = numUnknown1.Value
                x.B11 = numUnknown2.Value
                x.B12 = numUnknown3.Value
                x.B13 = numUnknown4.Value
                x.B14 = numUnknown5.Value
                ItemNames(x.ID) = txtName.Text
                x.Index = CurrentIndex
                Return x
            End Get
            Set(value As FileFormats.item_p.Item)
                With value
                    numID.Value = .ID
                    numBuyPrice.Value = .BuyPrice
                    numSellPrice.Value = .SellPrice
                    cbCategory.SelectedValue = .Category
                    cbSprite.SelectedValue = .Sprite
                    cbMove.SelectedValue = .ItemParameter1
                    numUnknown1.Value = .B10
                    numUnknown2.Value = .B11
                    numUnknown3.Value = .B12
                    numUnknown4.Value = .B13
                    numUnknown5.Value = .B14
                    txtName.Text = ItemNames(.ID)
                    CurrentIndex = .Index
                End With
            End Set
        End Property
        Private Property CurrentIndex As Integer
        Private Property Items As List(Of item_p.Item)
        Private Property ItemNames As String()

        Dim _oldIndex As Integer = -1
        Private Sub lbItems_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lbItems.SelectionChanged
            If _oldIndex > -1 Then
                Items(_oldIndex) = CurrentItem
            End If
            If lbItems.SelectedItem IsNot Nothing Then
                _oldIndex = lbItems.SelectedItem.ItemIndex
                CurrentItem = Items(lbItems.SelectedItem.ItemIndex)
            End If
        End Sub

        Private Class ItemWrapper
            Public Property ItemIndex As Integer
            Public Property Name As String
            Public Overrides Function ToString() As String
                Return Name
            End Function
        End Class

        Public Function GetSupportedTypes() As IEnumerable(Of Type) Implements iObjectControl.GetSupportedTypes
            Return {GetType(item_p)}
        End Function

        Public Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer Implements iObjectControl.GetSortOrder
            Return 0
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
