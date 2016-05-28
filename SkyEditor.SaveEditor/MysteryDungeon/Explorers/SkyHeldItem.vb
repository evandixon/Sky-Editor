Imports SkyEditor.Core.Utilities

Namespace MysteryDungeon.Explorers
    Public Class SkyHeldItem
        Implements IClonable

        Public Property IsValid As Boolean
        Private Property Flag1 As Boolean
        Private Property Flag2 As Boolean
        Private Property Flag3 As Boolean
        Private Property Flag4 As Boolean
        Private Property Flag5 As Boolean
        Private Property Flag6 As Boolean
        Private Property Flag7 As Boolean
        Public Property ID As Integer

        ''' <summary>
        ''' The ID of the item inside this one, if this item is a box.
        ''' </summary>
        ''' <returns></returns>
        Public Property ContainedItemID As Integer?
        Public Property Quantity As Integer
        Public Property HeldBy As Byte
        Public ReadOnly Property IsBox As Boolean
            Get
                Return ID > 363 AndAlso ID < 400
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Lists.GetSkyItemNames(ID)
        End Function

        Public Function Clone() As Object Implements IClonable.Clone
            Dim out As New SkyHeldItem
            With out
                .IsValid = Me.IsValid
                .Flag1 = Me.Flag1
                .Flag2 = Me.Flag2
                .Flag3 = Me.Flag3
                .Flag4 = Me.Flag4
                .Flag5 = Me.Flag5
                .Flag6 = Me.Flag6
                .Flag7 = Me.Flag7

                .ContainedItemID = Me.ContainedItemID
                .Quantity = Me.Quantity

                .ID = Me.ID
                .HeldBy = Me.HeldBy

            End With
            Return out
        End Function

        Public Function GetParameter() As Integer
            If IsBox Then
                Return ContainedItemID
            Else
                Return Quantity
            End If
        End Function

        Public Function GetHeldItemBits() As Binary
            Dim out As New Binary(SkySave.Offsets.HeldItemLength)
            With out
                .Bit(0) = IsValid
                .Bit(1) = Flag1
                .Bit(2) = Flag2
                .Bit(3) = Flag3
                .Bit(4) = Flag4
                .Bit(5) = Flag5
                .Bit(6) = Flag6
                .Bit(7) = Flag7

                Dim parameter As Integer
                If IsBox Then
                    parameter = ContainedItemID
                Else
                    parameter = Quantity
                End If
                .Int(0, 8, 11) = parameter

                .Int(0, 19, 11) = ID
                .Int(0, 30, 3) = HeldBy

            End With
            Return out
        End Function

        Public Shared Function FromHeldItemBits(bits As Binary) As SkyHeldItem
            Dim out As New SkyHeldItem
            With bits
                out.IsValid = .Bit(0)
                out.Flag1 = .Bit(1)
                out.Flag2 = .Bit(2)
                out.Flag3 = .Bit(3)
                out.Flag4 = .Bit(4)
                out.Flag5 = .Bit(5)
                out.Flag6 = .Bit(6)
                out.Flag7 = .Bit(7)
                Dim parameter = .Int(0, 8, 11)
                out.ID = .Int(0, 19, 11)
                out.HeldBy = .Int(0, 30, 3)
                If out.IsBox Then
                    out.ContainedItemID = parameter
                    out.Quantity = 1
                Else
                    out.ContainedItemID = Nothing
                    out.Quantity = parameter
                End If
            End With
            Return out
        End Function

        Public Shared Function FromStoredItemParts(itemID As Integer, parameter As Integer)
            Dim out As New SkyHeldItem
            out.IsValid = True
            out.ID = itemID
            If out.IsBox Then
                out.ContainedItemID = parameter
                out.Quantity = 1
            Else
                out.ContainedItemID = Nothing
                out.Quantity = parameter
            End If
            Return out
        End Function

        Public Sub New()
            IsValid = True
            ID = 1
            Quantity = 1
            ContainedItemID = 0
        End Sub
    End Class
End Namespace