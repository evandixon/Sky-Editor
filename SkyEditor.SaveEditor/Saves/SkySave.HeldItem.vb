Imports SkyEditor.SaveEditor.Interfaces

Namespace Saves
    Partial Class SkySave
        Implements iItemStorage
        Public Class HeldItem
            Inherits Binary
            Implements iItem

            Public Const Length As Integer = 33
            Public Const MimeType As String = "application/x-sky-item"

            Public Sub New(Bits As Binary)
                MyBase.New(Bits)
            End Sub
            Public Sub New(ID As Integer, Parameter As Integer)
                MyBase.New(Length)
                Me.ID = ID
                Me.Parameter = Parameter
                Me.IsValid = True
            End Sub

            ''' <summary>
            ''' The bit the game uses to determine whether this item is valid.
            ''' Must be true for the game to recognize it.
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property IsValid As Boolean
                Get
                    Return Bits(0)
                End Get
                Set(value As Boolean)
                    Bits(0) = value
                End Set
            End Property

            Public Property Flag1 As Boolean
                Get
                    Return Bits(1)
                End Get
                Set(value As Boolean)
                    Bits(1) = value
                End Set
            End Property
            Public Property Flag2 As Boolean
                Get
                    Return Bits(2)
                End Get
                Set(value As Boolean)
                    Bits(2) = value
                End Set
            End Property
            Public Property Flag3 As Boolean
                Get
                    Return Bits(3)
                End Get
                Set(value As Boolean)
                    Bits(3) = value
                End Set
            End Property
            Public Property Flag4 As Boolean
                Get
                    Return Bits(4)
                End Get
                Set(value As Boolean)
                    Bits(4) = value
                End Set
            End Property
            Public Property Flag5 As Boolean
                Get
                    Return Bits(5)
                End Get
                Set(value As Boolean)
                    Bits(5) = value
                End Set
            End Property
            Public Property Flag6 As Boolean
                Get
                    Return Bits(6)
                End Get
                Set(value As Boolean)
                    Bits(6) = value
                End Set
            End Property
            Public Property Flag7 As Boolean
                Get
                    Return Bits(7)
                End Get
                Set(value As Boolean)
                    Bits(7) = value
                End Set
            End Property

            ''' <summary>
            ''' The extra parameter for an item.
            '''
            ''' For boxes, this is the contained item.
            ''' For sticks, rocks, etc., this is the number of items in the stack.
            ''' For the Used TM, this is the contained move.
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Parameter As UInt16 Implements iItem.Parameter
                Get
                    Return Int(0, 8, 11)
                End Get
                Set(value As UInt16)
                    Int(0, 8, 11) = value
                End Set
            End Property

            ''' <summary>
            ''' The ID of the item.
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property ID As UInt16 Implements iItem.ID
                Get
                    Return Int(0, 19, 11)
                End Get
                Set(value As UInt16)
                    Int(0, 19, 11) = value
                End Set
            End Property

            ''' <summary>
            ''' The team member who is holding this item.
            '''
            ''' Must be 0-4.
            ''' 0 means held by no one.
            ''' 1-4 is the number of the active team member holding it.
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property HeldBy As Byte
                Get
                    Return Int(0, 30, 3)
                End Get
                Set(value As Byte)
                    Int(0, 30, 3) = value
                End Set
            End Property

            ''' <summary>
            ''' Determines whether or not the ID is in the range of Box items.
            ''' If this is true, the Parameter is the ID of the contained item.
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public ReadOnly Property IsBox As Boolean Implements iItem.IsBox
                Get
                    Return ID > 363 AndAlso ID < 400
                End Get
            End Property

            Public Overrides Function ToString() As String
                If IsValid Then
                    Dim output As New Text.StringBuilder
                    output.Append(Lists.GetSkyItemNames(ID))
                    If Parameter > 0 Then
                        If IsBox Then
                            output.Append(" (")
                            output.Append(Lists.GetSkyItemNames(Parameter))
                            output.Append(")")
                        Else
                            output.Append(" (")
                            output.Append(Parameter)
                            output.Append(")")
                        End If
                    End If
                    If HeldBy > 0 Then
                        output.Append(" [")
                        output.Append(My.Resources.Language.ItemToStringHeldBy)
                        output.Append(" ")
                        output.Append(HeldBy)
                        output.Append("]")
                    End If
                    Return output.ToString
                Else
                    Return "----------"
                End If
            End Function
        End Class

        Public Class StoredItem
            Implements iItem
            Public Sub New(ID As Integer, Parameter As Integer)
                Me.ID = ID
                Me.Parameter = Parameter
            End Sub

            Public ReadOnly Property IsValid As Boolean
                Get
                    Return ID > 0
                End Get
            End Property

            ''' <summary>
            ''' The extra parameter for an item.
            '''
            ''' For boxes, this is the contained item.
            ''' For sticks, rocks, etc., this is the number of items in the stack.
            ''' For the Used TM, this is the contained move.
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property Parameter As UInt16 Implements iItem.Parameter

            ''' <summary>
            ''' The ID of the item.
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Property ID As UInt16 Implements iItem.ID
            ''' <summary>
            ''' Determines whether or not the ID is in the range of Box items.
            ''' If this is true, the Parameter is the ID of the contained item.
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public ReadOnly Property IsBox As Boolean Implements iItem.IsBox
                Get
                    Return ID > 363 AndAlso ID < 400
                End Get
            End Property

            Public Overrides Function ToString() As String
                If ID > 0 Then
                    Dim output As New Text.StringBuilder
                    output.Append(Lists.GetSkyItemNames(ID))
                    If Parameter > 0 Then
                        If Me.IsBox Then
                            output.Append(" (")
                            output.Append(Lists.GetSkyItemNames(Parameter))
                            output.Append(")")
                        Else
                            output.Append(" (")
                            output.Append(Parameter)
                            output.Append(")")
                        End If
                    End If
                    Return output.ToString
                Else
                    Return "----------"
                End If
            End Function
        End Class

        Public Property HeldItems(Index As Integer) As HeldItem
            Get
                Return New HeldItem(Me.Bits.Range(Offsets.HeldItemOffset + Index * Offsets.HeldItemLength, Offsets.HeldItemLength))
            End Get
            Set(value As HeldItem)
                Me.Bits.Range(Offsets.HeldItemOffset + Index * Offsets.HeldItemLength, Offsets.HeldItemLength) = value
            End Set
        End Property
        Public Property HeldItems() As HeldItem()
            Get
                Return GetHeldItems()
            End Get
            Set(value As HeldItem())
                SetHeldItems(value)
            End Set
        End Property
        Public Function GetHeldItems() As ICollection(Of iItem)
            Dim output As New List(Of HeldItem)
            For count As Integer = 0 To 49
                Dim i = HeldItems(count)
                If i.IsValid Then output.Add(i)
            Next
            Return output.ToArray
        End Function
        Public Sub SetHeldItems(Value As ICollection(Of iItem))
            For count As Integer = 0 To 49
                If Value.Count > count Then
                    HeldItems(count) = Value(count)
                Else
                    HeldItems(count) = New HeldItem(New Binary(Offsets.HeldItemLength))
                End If
            Next
        End Sub
        Public Property SpEpisodeHeldItems(Index As Integer) As HeldItem
            Get
                Return HeldItems(Index + 50)
            End Get
            Set(value As HeldItem)
                HeldItems(Index + 50) = value
            End Set
        End Property
        Public Property SpEpisodeHeldItems() As iItem()
            Get
                Return SpEpisodeGetHeldItems()
            End Get
            Set(value As iItem())
                SpEpisodeSetHeldItems(value)
            End Set
        End Property
        Public Function SpEpisodeGetHeldItems() As ICollection(Of iItem)
            Dim output As New List(Of HeldItem)
            For count As Integer = 0 To 49
                Dim i = SpEpisodeHeldItems(count)
                If i.IsValid Then output.Add(i)
            Next
            Return output.ToArray
        End Function
        Public Sub SpEpisodeSetHeldItems(Value As ICollection(Of iItem))
            For count As Integer = 0 To 49
                If Value.Count > count Then
                    SpEpisodeHeldItems(count) = Value(count)
                Else
                    SpEpisodeHeldItems(count) = New HeldItem(New Binary(Offsets.HeldItemLength))
                End If
            Next
        End Sub
        Public Property StoredItems() As StoredItem()
            Get
                Return GetStoredItems()
            End Get
            Set(value As StoredItem())
                SetStoredItems(value)
            End Set
        End Property
        Public Function GetStoredItems() As iItem()
            Dim ids = Bits.Range(Offsets.StoredItemOffset, 11 * 1000)
            Dim params = Bits.Range(Offsets.StoredItemOffset + 11 * 1000, 11 * 1000)
            Dim items As New List(Of iItem)
            For count As Integer = 0 To 999
                Dim id = ids.NextInt(11)
                Dim p = params.NextInt(11)
                If id > 0 Then
                    items.Add(New StoredItem(id, p))
                Else
                    Exit For
                End If
            Next
            Return items.ToArray
        End Function
        Public Sub SetStoredItems(Value As iItem())
            Dim ids As New Binary(11 * 1000)
            Dim params As New Binary(11 * 1000)
            For count As Integer = 0 To 999
                If Value.Length > count Then
                    ids.NextInt(11) = Value(count).ID
                    params.NextInt(11) = Value(count).Parameter
                Else
                    ids.NextInt(11) = 0
                    params.NextInt(11) = 0
                End If
            Next
            Bits.Range(Offsets.StoredItemOffset, 11 * 1000) = ids
            Bits.Range(Offsets.StoredItemOffset + 11 * 1000, 11 * 1000) = params
        End Sub
        Public Function NewHeldItem(ID As Integer, Parameter As Integer) As iItem
            Return New HeldItem(ID, Parameter)
        End Function
        Public Function NewStoredItem(ID As Integer, Parameter As Integer) As iItem
            Return New StoredItem(ID, Parameter)
        End Function

        Public Function MaxHeldItems() As Integer Implements iItemStorage.MaxHeldItems
            Return Offsets.HeldItemNumber
        End Function

        Public Function GetItemDicitonary() As Dictionary(Of Integer, String) Implements iItemStorage.GetItemDictionary
            Return Lists.GetSkyItemNames()
        End Function

        Public Function IsBox(ItemID As Integer) As Boolean Implements iItemStorage.IsBox
            Return ItemID > 363 AndAlso ItemID < 400
        End Function

        Public Function SupportsBoxes() As Boolean Implements iItemStorage.SupportsBoxes
            Return True
        End Function
        Public Function HeldItemSlots() As ItemSlot() Implements iItemStorage.HeldItemSlots
            Return {New ItemSlot(AddressOf GetHeldItems, AddressOf SetHeldItems, AddressOf NewHeldItem, My.Resources.Language.HeldItemsSlot, GetItemDicitonary, 50),
                    New ItemSlot(AddressOf SpEpisodeGetHeldItems, AddressOf SpEpisodeSetHeldItems, AddressOf NewHeldItem, My.Resources.Language.EpisodeHeldItems, GetItemDicitonary, 50),
                    New ItemSlot(AddressOf GetStoredItems, AddressOf SetStoredItems, AddressOf NewStoredItem, My.Resources.Language.StoredItemsSlot, GetItemDicitonary, 1000)}
        End Function
    End Class
End Namespace