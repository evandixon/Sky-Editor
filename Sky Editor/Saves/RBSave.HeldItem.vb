Imports SkyEditor.Interfaces

Namespace Saves
    Partial Class RBSave
        Implements iItemStorage
        Public Class HeldItem
            Inherits Binary
            Implements iItem

            Public Const Length As Integer = 23
            Public Const MimeType As String = "application/x-rb-item"

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
                    Return Int(0, 8, 7)
                End Get
                Set(value As UInt16)
                    Int(0, 8, 7) = value
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
                    Return Int(0, 15, 8)
                End Get
                Set(value As UInt16)
                    Int(0, 15, 8) = value
                End Set
            End Property

            ' ''' <summary>
            ' ''' The team member who is holding this item.
            ' '''
            ' ''' Must be 0-4.
            ' ''' 0 means held by no one.
            ' ''' 1-4 is the number of the active team member holding it.
            ' ''' </summary>
            ' ''' <value></value>
            ' ''' <returns></returns>
            ' ''' <remarks></remarks>
            '<Obsolete("Not implemented")> Public Property HeldBy As Byte
            '    Get
            '        Return 0
            '    End Get
            '    Set(value As Byte)

            '    End Set
            'End Property

            ''' <summary>
            ''' Determines whether or not the ID is in the range of Box items.
            ''' If this is true, the Parameter is the ID of the contained item.
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            <Obsolete("Not implemented")> Public ReadOnly Property IsBox As Boolean Implements iItem.IsBox
                Get
                    Return False
                End Get
            End Property

            Public Overrides Function ToString() As String
                If IsValid Then
                    Dim output As New Text.StringBuilder
                    output.Append(Lists.RBItemNames(ID))
                    If Parameter > 0 Then
                        'If Me.IsBox Then
                        '    output.Append(" (")
                        '    output.Append(Lists.SkyItemNames(Parameter))
                        '    output.Append(")")
                        'Else
                        output.Append(" (")
                        output.Append(Parameter)
                        output.Append(")")
                        'End If
                    End If
                    'If HeldBy > 0 Then
                    'output.Append(" [")
                    'output.Append(PluginHelper.GetLanguageItem("Held by"))
                    'output.Append(" ")
                    'output.Append(HeldBy)
                    'output.Append("]")
                    'End If
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
            For count As Integer = 0 To Offsets.HeldItemNumber - 1
                Dim i = HeldItems(count)
                If i.IsValid Then output.Add(i)
            Next
            Return output.ToArray
        End Function
        Public Sub SetHeldItems(Value As ICollection(Of iItem))
            For count As Integer = 0 To Offsets.HeldItemNumber - 1
                If Value.Count > count Then
                    HeldItems(count) = Value(count)
                Else
                    HeldItems(count) = New HeldItem(New Binary(Offsets.HeldItemLength))
                End If
            Next
        End Sub
        Public Function NewHeldItem(ID As Integer, Parameter As Integer) As iItem
            Return New HeldItem(ID, Parameter)
        End Function

        Public Function MaxHeldItems() As Integer Implements iItemStorage.MaxHeldItems
            Return Offsets.HeldItemNumber
        End Function

        Public Function GetItemDicitonary() As Dictionary(Of Integer, String) Implements iItemStorage.GetItemDictionary
            Return Lists.RBItemNames
        End Function

        Public Function IsBox(ItemID As Integer) As Boolean Implements iItemStorage.IsBox
            Return False
        End Function

        Public Function SupportsBoxes() As Boolean Implements iItemStorage.SupportsBoxes
            Return False
        End Function

        Public Function HeldItemSlots() As ItemSlot() Implements iItemStorage.HeldItemSlots
            Return {New ItemSlot(AddressOf GetHeldItems, AddressOf SetHeldItems, AddressOf NewHeldItem, SkyEditorBase.PluginHelper.GetLanguageItem("Toolbox"), GetItemDicitonary, Offsets.HeldItemNumber)}
        End Function
    End Class

End Namespace