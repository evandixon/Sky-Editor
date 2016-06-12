Imports System.Collections.Specialized
Imports System.Reflection
Imports System.Windows.Input
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities

Namespace Modeling
    Public Class ItemSlot(Of T As IClonable)
        Implements IItemSlot

        Public Property ItemCollection As IList Implements IItemSlot.ItemCollection
            Get
                Return _collection
            End Get
            Set(value As IList)
                If _collection IsNot Nothing AndAlso TypeOf _collection Is INotifyCollectionChanged Then
                    RemoveHandler DirectCast(_collection, INotifyCollectionChanged).CollectionChanged, AddressOf OnCollectionChanged
                End If

                _collection = value

                If _collection IsNot Nothing AndAlso TypeOf _collection Is INotifyCollectionChanged Then
                    AddHandler DirectCast(_collection, INotifyCollectionChanged).CollectionChanged, AddressOf OnCollectionChanged
                End If
            End Set
        End Property
        Dim _collection As IList

        Public ReadOnly Property MaxItemCount As Integer Implements IItemSlot.MaxItemCount

        Public ReadOnly Property Name As String Implements IItemSlot.Name

        'Todo: try to make this be of type T
        Public Property NewItem As Object Implements IItemSlot.NewItem

        Public ReadOnly Property AddCommand As RelayCommand Implements IItemSlot.AddCommand

        Private Function CanAdd() As Boolean
            Return ItemCollection.Count < MaxItemCount
        End Function

        Private Function DoAdd() As Task
            Dim cloned As T = DirectCast(NewItem, IClonable).Clone
            ItemCollection.Add(cloned)
            Return Task.FromResult(0)
        End Function

        Private Sub OnCollectionChanged(sender As Object, e As EventArgs)
            AddCommand.IsEnabled = CanAdd()
        End Sub

        Public Sub New(name As String, items As IList(Of T), maxItemCount As Integer)
            Me.Name = name
            Me.ItemCollection = items
            Me.MaxItemCount = maxItemCount
            Me.NewItem = ReflectionHelpers.CreateInstance(GetType(T).GetTypeInfo)

            AddCommand = New RelayCommand(AddressOf DoAdd)
        End Sub
    End Class
End Namespace

