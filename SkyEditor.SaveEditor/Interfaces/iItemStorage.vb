﻿Namespace Interfaces
    <Obsolete> Public Class ItemSlotOld
        Public Property Getter As iItemStorage.GetHeldItems
        Public Property Setter As iItemStorage.SetHeldItems
        Public Property Creator As iItemStorage.NewHeldItem
        Public Property SlotName As String
        Public Property ItemDictionary As IDictionary(Of Integer, String)
        Public Property MaxItemCount As Integer
        Public Sub New(Getter As iItemStorage.GetHeldItems, Setter As iItemStorage.SetHeldItems, Creator As iItemStorage.NewHeldItem, SlotName As String, ItemDictionary As IDictionary(Of Integer, String), MaxItemCount As Integer)
            Me.Getter = Getter
            Me.Setter = Setter
            Me.Creator = Creator
            Me.SlotName = SlotName
            Me.ItemDictionary = ItemDictionary
            Me.MaxItemCount = MaxItemCount
        End Sub
    End Class
    <Obsolete> Public Interface iItemStorage
        Delegate Function GetHeldItems() As iItemOld()
        Delegate Sub SetHeldItems(Items As iItemOld())
        Delegate Function NewHeldItem(ID As Integer, Parameter As Integer) As iItemOld

        Function HeldItemSlots() As ItemSlotOld()
        Function MaxHeldItems() As Integer
        Function GetItemDictionary() As Dictionary(Of Integer, String)
        Function IsBox(ItemID As Integer) As Boolean
        Function SupportsBoxes() As Boolean
    End Interface

End Namespace