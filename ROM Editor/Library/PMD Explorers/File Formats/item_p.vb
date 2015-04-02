Imports SkyEditorBase

Public Class item_p
    Inherits GenericFile
    Public Property Items As List(Of Item)
    Public Class Item
        Inherits GenericFile
        Public Property Index As Integer
        Public Property BuyPrice As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, 0)
            End Get
            Set(value As UInt16)
                Dim b = BitConverter.GetBytes(value)
                RawData(0) = b(0)
                RawData(1) = b(1)
            End Set
        End Property
        Public Property SellPrice As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, 2)
            End Get
            Set(value As UInt16)
                Dim b = BitConverter.GetBytes(value)
                RawData(2) = b(0)
                RawData(3) = b(1)
            End Set
        End Property
        Public Property Category As Byte
            Get
                Return RawData(4)
            End Get
            Set(value As Byte)
                RawData(4) = value
            End Set
        End Property
        Public Property Sprite As Byte
            Get
                Return RawData(5)
            End Get
            Set(value As Byte)
                RawData(5) = value
            End Set
        End Property
        Public Property ID As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, 6)
            End Get
            Set(value As UInt16)
                Dim b = BitConverter.GetBytes(value)
                RawData(6) = b(0)
                RawData(7) = b(1)
            End Set
        End Property
        Public Property MoveID As UInt16
            Get
                Return BitConverter.ToUInt16(RawData, 8)
            End Get
            Set(value As UInt16)
                Dim b = BitConverter.GetBytes(value)
                RawData(8) = b(0)
                RawData(9) = b(1)
            End Set
        End Property
        Public Property B10 As Byte
            Get
                Return RawData(10)
            End Get
            Set(value As Byte)
                RawData(10) = value
            End Set
        End Property
        Public Property B11 As Byte
            Get
                Return RawData(11)
            End Get
            Set(value As Byte)
                RawData(11) = value
            End Set
        End Property
        Public Property B12 As Byte
            Get
                Return RawData(12)
            End Get
            Set(value As Byte)
                RawData(12) = value
            End Set
        End Property
        Public Property B13 As Byte
            Get
                Return RawData(13)
            End Get
            Set(value As Byte)
                RawData(13) = value
            End Set
        End Property
        Public Property B14 As Byte
            Get
                Return RawData(14)
            End Get
            Set(value As Byte)
                RawData(14) = value
            End Set
        End Property
        Public Property B15 As Byte
            Get
                Return RawData(15)
            End Get
            Set(value As Byte)
                RawData(15) = value
            End Set
        End Property
        Public Sub New(Data As Byte())
            MyBase.New(Data)
        End Sub
    End Class
    Public Sub New(Rom As GenericNDSRom)
        MyBase.New(IO.File.ReadAllBytes(PluginHelper.GetResourceName(Rom.Name & "/data/balance/item_p.bin")))
        Items = New List(Of Item)
        For count As Integer = &H20 To Math.Floor(Me.RawData.Length / 16 - 1) * 16 Step 16
            Items.Add(New Item(SkyEditorBase.Utilities.GenericArrayOperations(Of Byte).CopyOfRange(Me.RawData, count, count + 15)))
        Next
    End Sub
    Public Function GetBytes() As Byte()
        Dim out As New List(Of Byte)
        For count As Integer = 0 To 15
            out.Add(RawData(count))
        Next
        For Each item In Items
            For count As Integer = 0 To 15
                out.Add(item.RawData(count))
            Next
        Next
        out.Add(4)
        out.Add(4)
        out.Add(0)
        out.Add(&HAA)
        out.Add(&HAA)
        out.Add(&HAA)
        out.Add(&HAA)
        out.Add(&HAA)
        out.Add(&HAA)
        out.Add(&HAA)
        out.Add(&HAA)
        out.Add(&HAA)
        out.Add(&HAA)
        out.Add(&HAA)
        out.Add(&HAA)
        out.Add(&HAA)

        Return out.ToArray
    End Function
    Public Sub Save()
        IO.File.WriteAllBytes(Filename, GetBytes)
    End Sub
End Class
