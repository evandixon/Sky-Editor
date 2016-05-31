Imports SkyEditor.Core.Utilities

Namespace MysteryDungeon.Rescue
    Public Class RBHeldItem
        Implements IClonable

        Public Const Length As Integer = 23
        Public Const MimeType As String = "application/x-rb-item"

        Public Property IsValid As Boolean
        Private Property Flag1 As Boolean
        Private Property Flag2 As Boolean
        Private Property Flag3 As Boolean
        Private Property Flag4 As Boolean
        Private Property Flag5 As Boolean
        Private Property Flag6 As Boolean
        Private Property Flag7 As Boolean
        Public Property ID As Integer

        ''' <remarks>For sticks and other stackable items, this is the number in the stack.  For used TMs, this is the contained move</remarks>
        Public Property Parameter As Integer

        Public Overrides Function ToString() As String
            Return Lists.RBItems(ID)
        End Function

        Public Function Clone() As Object Implements IClonable.Clone
            Dim out As New RBHeldItem
            With out
                .IsValid = Me.IsValid
                .Flag1 = Me.Flag1
                .Flag2 = Me.Flag2
                .Flag3 = Me.Flag3
                .Flag4 = Me.Flag4
                .Flag5 = Me.Flag5
                .Flag6 = Me.Flag6
                .Flag7 = Me.Flag7

                .Parameter = Me.Parameter

                .ID = Me.ID

            End With
            Return out
        End Function

        Public Function GetParameter() As Integer
            Return Parameter
        End Function

        Public Function GetHeldItemBits() As Binary
            Dim out As New Binary(Length)
            With out
                .Bit(0) = IsValid
                .Bit(1) = Flag1
                .Bit(2) = Flag2
                .Bit(3) = Flag3
                .Bit(4) = Flag4
                .Bit(5) = Flag5
                .Bit(6) = Flag6
                .Bit(7) = Flag7

                .Int(0, 8, 7) = Me.Parameter

                .Int(0, 15, 8) = ID

            End With
            Return out
        End Function

        Public Shared Function FromHeldItemBits(bits As Binary) As RBHeldItem
            Dim out As New RBHeldItem
            With bits
                out.IsValid = .Bit(0)
                out.Flag1 = .Bit(1)
                out.Flag2 = .Bit(2)
                out.Flag3 = .Bit(3)
                out.Flag4 = .Bit(4)
                out.Flag5 = .Bit(5)
                out.Flag6 = .Bit(6)
                out.Flag7 = .Bit(7)
                out.Parameter = .Int(0, 8, 7)
                out.ID = .Int(0, 15, 8)
            End With
            Return out
        End Function

        Public Shared Function FromStoredItemParts(itemID As Integer, parameter As Integer)
            Dim out As New RBHeldItem
            out.IsValid = True
            out.ID = itemID
            out.Parameter = parameter
            Return out
        End Function

        Public Sub New()
            IsValid = True
            ID = 1
            Parameter = 1
        End Sub
    End Class
End Namespace