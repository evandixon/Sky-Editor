Imports System.Windows.Controls
Imports ROMEditor.FileFormats.Explorers
Imports SkyEditorBase
Imports SkyEditorWPF.UI

Namespace Explorers
    Public Class item_p_Editor
        Inherits ObjectControl

        Public Overrides Sub RefreshDisplay()
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

        Public Overrides Sub UpdateObject()
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
                Dim x As New item_p.Item
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
            Set(value As item_p.Item)
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

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(item_p)}
        End Function

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 0
        End Function

    End Class
End Namespace
