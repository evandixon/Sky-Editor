Imports SkyEditor.Core.Utilities

Namespace MysteryDungeon.Explorers
    Public Class SkyStoredItem
        Implements IClonable
        Implements INotifyPropertyChanged
        Implements IExplorersStoredItem

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Public Property ID As Integer Implements IExplorersStoredItem.ID
            Get
                Return _id
            End Get
            Set(value As Integer)
                If Not _id = value Then
                    _id = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ID)))
                End If
            End Set
        End Property
        Dim _id As Integer

        ''' <summary>
        ''' The ID of the item inside this one, if this item is a box.
        ''' </summary>
        ''' <returns></returns>
        Public Property ContainedItemID As Integer Implements IExplorersStoredItem.ContainedItemID
        Public Property Quantity As Integer Implements IExplorersStoredItem.Quantity
            Get
                Return _quantity
            End Get
            Set(value As Integer)
                If Not _quantity = value Then
                    _quantity = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Quantity)))
                End If
            End Set
        End Property
        Dim _quantity As Integer

        Public ReadOnly Property IsBox As Boolean Implements IExplorersStoredItem.IsBox
            Get
                Return ID > 363 AndAlso ID < 400
            End Get
        End Property

        Public Overrides Function ToString() As String
            If IsBox Then
                Return $"{Lists.GetSkyItemNames(ID)} ({Lists.GetSkyItemNames(ContainedItemID)})"
            Else
                If Quantity > 1 Then
                    Return $"{Lists.GetSkyItemNames(ID)} ({Quantity})"
                Else
                    Return Lists.GetSkyItemNames(ID)
                End If
            End If
        End Function

        Public Function GetParameter() As Integer
            If IsBox Then
                Return ContainedItemID
            Else
                Return Quantity
            End If
        End Function

        Public Function Clone() As Object Implements IClonable.Clone
            Return New SkyStoredItem(Me.ID, Me.GetParameter)
        End Function

        Public Sub New(itemID As Integer, parameter As Integer)
            Me.ID = itemID
            If Me.IsBox Then
                Me.ContainedItemID = parameter
                Me.Quantity = 1
            Else
                Me.ContainedItemID = 0
                Me.Quantity = parameter
            End If
        End Sub

        Public Sub New()
            Me.New(1, 1)
        End Sub
    End Class
End Namespace
