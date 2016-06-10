Imports SkyEditor.Core.IO

Namespace FileFormats.Explorers
    Public Class TableDatItemList
        Inherits GenericFile
        Public Structure TableDatItem
            Public Property ObtainPercentage As Single
            Public Property ItemID As UInt16
            Public Sub New(ItemID As UInt16, ObtainPercentage As Single)
                Me.ItemID = ItemID
                Me.ObtainPercentage = ObtainPercentage
            End Sub
        End Structure
        Public Property Items As List(Of TableDatItem)
        Private Sub InitItems()
            Items = New List(Of TableDatItem)
            If Length >= 2 Then
                Dim itemCount = BitConverter.ToUInt16(RawData(0, 2), 0)
                For count As Integer = 2 To (itemCount - 1) * 4
                    Dim percentage As Single
                    If count = 0 Then
                        percentage = BitConverter.ToUInt16(RawData(count, 2), 0) / 1024
                    Else
                        percentage = (BitConverter.ToUInt16(RawData(count, 2), 0) - BitConverter.ToUInt16(RawData(count - 4, 2), 0)) / 1024
                    End If
                    Items.Add(New TableDatItem(percentage, BitConverter.ToUInt16(RawData(count + 2, 2), 0)))
                Next
            End If
        End Sub
        Public Overrides Sub Save(Destination As String, provider As IOProvider)
            Me.Length = 2 + (Items.Count * 4)
            RawData(0, 2) = BitConverter.GetBytes(Items.Count)
            For count As Integer = 0 To Items.Count - 1
                If count = 0 Then
                    RawData(count * 4, 2) = BitConverter.GetBytes(CInt(Items(count).ObtainPercentage * 1024))
                Else
                    RawData(count * 4, 2) = BitConverter.GetBytes(CInt((Items(count).ObtainPercentage + Items(count + 1).ObtainPercentage) * 1024))
                End If
                RawData(count * 4 + 2, 2) = BitConverter.GetBytes(Items(count).ItemID)
            Next
            MyBase.Save(Destination, provider)
        End Sub
    End Class

End Namespace
