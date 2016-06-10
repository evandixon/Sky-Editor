Imports SkyEditor.Core
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO
Imports SkyEditor.SaveEditor.Modeling

Namespace MysteryDungeon.Rescue
    Public Class RBSave
        Inherits BinaryFile
        Implements IDetectableFileType
        Implements IInventory
        Implements INotifyPropertyChanged
        Implements IPokemonStorage
        Implements INotifyModified

        Protected Class RBOffsets
            Public Overridable ReadOnly Property BackupSaveStart As Integer = &H6000
            Public Overridable ReadOnly Property ChecksumEnd As Integer = &H57D0
            Public Overridable ReadOnly Property BaseTypeOffset As Integer = &H67 * 8
            Public Overridable ReadOnly Property TeamNameStart As Integer = &H4EC8 * 8
            Public Overridable ReadOnly Property TeamNameLength As Integer = 10
            Public Overridable ReadOnly Property HeldMoneyOffset As Integer = &H4E6C * 8
            Public Overridable ReadOnly Property HeldMoneyLength As Integer = 24
            Public Overridable ReadOnly Property StoredMoneyOffset As Integer = &H4E6F * 8
            Public Overridable ReadOnly Property StoredMoneyLength As Integer = 24
            Public Overridable ReadOnly Property RescuePointsOffset As Integer = &H4ED3 * 8
            Public Overridable ReadOnly Property RescuePointsLength As Integer = 16
            Public Overridable ReadOnly Property HeldItemOffset As Integer = &H4CF0 * 8
            Public Overridable ReadOnly Property HeldItemLength As Integer = 23
            Public Overridable ReadOnly Property HeldItemNumber As Integer = 20
            Public Overridable ReadOnly Property StoredItemOffset As Integer = &H4D2B * 8 - 2
            Public Overridable ReadOnly Property StoredPokemonOffset As Integer = (&H5B3 * 8 + 3) - (323 * 9)
            Public Overridable ReadOnly Property StoredPokemonLength As Integer = 323
            Public Overridable ReadOnly Property StoredPokemonNumber As Integer = 407 + 6
        End Class

        Public Sub New()
            MyBase.New
            Me.Offsets = New RBOffsets
        End Sub

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Public Event Modified As INotifyModified.ModifiedEventHandler Implements INotifyModified.Modified

        Public Overrides Async Function OpenFile(Filename As String, Provider As IOProvider) As Task
            Await MyBase.OpenFile(Filename, Provider)

            LoadGeneral()
            LoadItems()
            LoadStoredPokemon()
        End Function

        Public Overrides Sub Save(Destination As String, provider As IOProvider)
            SaveGeneral()
            SaveItems()
            SaveStoredPokemon()

            MyBase.Save(Destination, provider)
        End Sub

        Protected Overridable ReadOnly Property Offsets As RBOffsets

#Region "Event Handlers"
        Private Sub Me_OnPropertyChanged(sender As Object, e As EventArgs) Handles Me.PropertyChanged
            RaiseEvent Modified(Me, New EventArgs)
        End Sub
        Private Sub OnModified(sender As Object, e As EventArgs)
            RaiseEvent Modified(sender, e)
        End Sub
        Private Sub OnCollectionChanged(sender As Object, e As EventArgs) Handles _heldItems.CollectionChanged
            RaiseEvent Modified(Me, New EventArgs)
        End Sub
#End Region

#Region "Save Interaction"

#Region "General"
        Private Sub LoadGeneral()
            TeamName = Bits.StringPMD(0, Offsets.TeamNameStart, Offsets.TeamNameLength)
            HeldMoney = Bits.Int(0, Offsets.HeldMoneyOffset, Offsets.HeldMoneyLength)
            StoredMoney = Bits.Int(0, Offsets.StoredMoneyOffset, Offsets.StoredMoneyLength)
            RescuePoints = Bits.Int(0, Offsets.RescuePointsOffset, Offsets.RescuePointsLength)
            BaseType = Bits.Int(0, Offsets.BaseTypeOffset, 8)
        End Sub

        Private Sub SaveGeneral()
            Bits.StringPMD(0, Offsets.TeamNameStart, Offsets.TeamNameLength) = TeamName
            Bits.Int(0, Offsets.HeldMoneyOffset, Offsets.HeldMoneyLength) = HeldMoney
            Bits.Int(0, Offsets.StoredMoneyOffset, Offsets.StoredMoneyLength) = StoredMoney
            Bits.Int(0, Offsets.RescuePointsOffset, Offsets.RescuePointsLength) = RescuePoints
            Bits.Int(0, Offsets.BaseTypeOffset, 8) = BaseType
        End Sub

        ''' <summary>
        ''' Gets or sets the save file's Team Name.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property TeamName As String
            Get
                Return _teamName
            End Get
            Set(value As String)
                If Not _teamName = value Then
                    _teamName = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TeamName)))
                End If
            End Set
        End Property
        Dim _teamName As String

        Public Property BaseType As Integer
            Get
                Return _baseType
            End Get
            Set(value As Integer)
                If Not _baseType = value Then
                    _baseType = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(BaseType)))
                End If
            End Set
        End Property
        Dim _baseType As Integer

        Public Property RescuePoints As Integer
            Get
                Return _rescuePoints
            End Get
            Set(value As Integer)
                If Not _rescuePoints = value Then
                    _rescuePoints = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RescuePoints)))
                End If
            End Set
        End Property
        Dim _rescuePoints As Integer

        Public Property HeldMoney As Integer
            Get
                Return _heldMoney
            End Get
            Set(value As Integer)
                If Not _heldMoney = value Then
                    _heldMoney = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HeldMoney)))
                End If
            End Set
        End Property
        Dim _heldMoney As Integer

        Public Property StoredMoney As Integer
            Get
                Return _storedMoney
            End Get
            Set(value As Integer)
                If Not _storedMoney = value Then
                    _storedMoney = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(StoredMoney)))
                End If
            End Set
        End Property
        Dim _storedMoney As Integer

#End Region

#Region "Held Items"
        Private Sub LoadItems()
            HeldItems = New ObservableCollection(Of RBHeldItem)
            For count = 0 To Offsets.HeldItemNumber
                Dim i As RBHeldItem = RBHeldItem.FromHeldItemBits(Me.Bits.Range(Offsets.HeldItemOffset + count * Offsets.HeldItemLength, Offsets.HeldItemLength))
                If i.IsValid Then
                    HeldItems.Add(i)
                Else
                    Exit For
                End If
            Next

            InitItemSlots()
        End Sub

        Private Sub SaveItems()
            For count = 0 To Offsets.HeldItemNumber
                If HeldItems.Count > count Then
                    Me.Bits.Range(Offsets.HeldItemOffset + count * Offsets.HeldItemLength, Offsets.HeldItemLength) = HeldItems(count).GetHeldItemBits
                Else
                    Me.Bits.Range(Offsets.HeldItemOffset + count * Offsets.HeldItemLength, Offsets.HeldItemLength) = New Binary(Offsets.HeldItemLength)
                End If
            Next
        End Sub

        Public Property HeldItems As ObservableCollection(Of RBHeldItem)
            Get
                Return _heldItems
            End Get
            Set(value As ObservableCollection(Of RBHeldItem))
                If _heldItems IsNot value Then
                    _heldItems = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HeldItems)))
                End If
            End Set
        End Property
        Private WithEvents _heldItems As ObservableCollection(Of RBHeldItem)

        Public Property ItemSlots As IEnumerable(Of IItemSlot) Implements IInventory.ItemSlots
            Get
                Return _itemSlots
            End Get
            Private Set(value As IEnumerable(Of IItemSlot))
                _itemSlots = value
            End Set
        End Property
        Dim _itemSlots As ObservableCollection(Of IItemSlot)


        Private Sub InitItemSlots()
            Dim slots As New ObservableCollection(Of IItemSlot)
            slots.Add(New ItemSlot(Of RBHeldItem)(My.Resources.Language.HeldItemsSlot, HeldItems, Offsets.HeldItemNumber))
            ItemSlots = slots
        End Sub


#End Region

#Region "Stored Pokemon"
        Private Sub LoadStoredPokemon()
            _storage = New ObservableCollection(Of IPokemonBox)
            Dim defs = StoredPokemonSlotDefinition.FromLines(My.Resources.ListResources.RBFriendAreaOffsets)
            Dim offset As Integer = 0
            For Each item In defs
                Dim pokemon As New ObservableCollection(Of RBStoredPokemon)
                AddHandler pokemon.CollectionChanged, AddressOf OnCollectionChanged

                For count = offset To offset + item.Length - 1
                    Dim p = RawStoredPokemon(count)
                    AddHandler p.Modified, AddressOf OnModified
                    AddHandler p.PropertyChanged, AddressOf OnModified

                    pokemon.Add(p)
                Next
                offset += item.Length - 1
                _storage.Add(New BasicPokemonBox(item.Name, pokemon))
            Next
        End Sub

        Private Sub SaveStoredPokemon()
            Dim defs = StoredPokemonSlotDefinition.FromLines(My.Resources.ListResources.RBFriendAreaOffsets)
            For i = 0 To defs.Count - 1
                For j = 0 To defs(i).Length - 1
                    RawStoredPokemon(i + j) = _storage(i).ItemCollection(j)
                Next
            Next
        End Sub

        Private Property RawStoredPokemon(Index As Integer) As RBStoredPokemon
            Get
                Return New RBStoredPokemon(Me.Bits.Range(Offsets.StoredPokemonOffset + Index * Offsets.StoredPokemonLength, Offsets.StoredPokemonLength))
            End Get
            Set(value As RBStoredPokemon)
                Me.Bits.Range(Offsets.StoredPokemonOffset + Index * Offsets.StoredPokemonLength, Offsets.StoredPokemonLength) = value.GetStoredPokemonBits
            End Set
        End Property

        Public ReadOnly Property Storage As IEnumerable(Of IPokemonBox) Implements IPokemonStorage.Storage
            Get
                Return _storage
            End Get
        End Property
        Dim _storage As ObservableCollection(Of IPokemonBox)

#End Region

        Public ReadOnly Property StoredItemCounts As Integer()
            Get
                Dim out(239) As Integer
                Dim block = Bits.Range(Offsets.StoredItemOffset, 2400)
                For count As Integer = 0 To 238
                    out(count) = block.NextInt(10)
                Next
                Return out
            End Get
        End Property

#End Region

#Region "Technical Stuff"
        Protected Overrides Sub FixChecksum()
            'Fix the first checksum
            Dim buffer = BitConverter.GetBytes(Checksums.Calculate32BitChecksum(Bits, 4, Offsets.ChecksumEnd))
            For count = 0 To 3
                Bits.Int(count, 0, 8) = buffer(count)
            Next

            'Ensure backup save matches.
            'Not strictly required, as the first one will be loaded if it's valid, but looks nicer.
            CopyToBackup()

            ''Quicksave checksum
            'TODO: Research the quick save
            'buffer = BitConverter.GetBytes(Checksums.Calculate32BitChecksum(Me, Offsets.QuicksaveChecksumStart, Offsets.QuicksaveChecksumEnd))
            'For x As Byte = 0 To 3
            '    RawData(x + Offsets.QuicksaveStart) = buffer(x)
            'Next
        End Sub
        Public Sub CopyToBackup()
            Dim e As Integer = Offsets.BackupSaveStart
            For i As Integer = 4 To e - 1
                Bits.Int(i + e, 0, 8) = Bits.Int(i, 0, 8)
            Next
        End Sub
        'Public Overrides Function DefaultSaveID() As String
        '    Return GameStrings.RBSave
        'End Function
#End Region
        Public Overridable Function IsFileOfType(File As GenericFile) As Task(Of Boolean) Implements IDetectableFileType.IsOfType
            If File.Length > Offsets.ChecksumEnd Then
                Dim buffer = BitConverter.GetBytes(Checksums.Calculate32BitChecksum(File, 4, Offsets.ChecksumEnd))
                Return Task.FromResult(File.RawData(0) = buffer(0) AndAlso File.RawData(1) = buffer(1) AndAlso File.RawData(2) = buffer(2) AndAlso File.RawData(3) = buffer(3))
            Else
                Return Task.FromResult(False)
            End If
        End Function
    End Class

End Namespace