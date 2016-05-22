Namespace Modeling
    Public Class ItemSlot
        Implements IItemSlot

        Public Property ItemCollection As Object Implements IItemSlot.ItemCollection

        Public ReadOnly Property MaxItemCount As Integer Implements IItemSlot.MaxItemCount

        Public ReadOnly Property Name As String Implements IItemSlot.Name

        Public Sub New(name As String, items As Object, maxItemCount As Integer)
            Me.Name = name
            Me.ItemCollection = items
            Me.MaxItemCount = maxItemCount
        End Sub
    End Class
End Namespace

