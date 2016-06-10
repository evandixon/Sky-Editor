Imports SkyEditor.Core
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities
Imports SkyEditor.SaveEditor.Interfaces
Imports SkyEditor.SaveEditor.Modeling

Namespace MysteryDungeon.Explorers
    Public Class SkySave
        Inherits BinaryFile
        Implements IDetectableFileType
        Implements IInventory
        Implements IParty
        Implements IPokemonStorage
        Implements INotifyPropertyChanged
        Implements INotifyModified

        Friend Class Offsets
            Public Const BackupSaveStart As Integer = &HC800
            Public Const ChecksumEnd As Integer = &HB65A
            Public Const QuicksaveStart As Integer = &H19000
            Public Const QuicksaveChecksumStart As Integer = &H19004
            Public Const QuicksaveChecksumEnd As Integer = &H1E7FF

            Public Const TeamNameStart As Integer = &H994E * 8
            Public Const TeamNameLength As Integer = 10

            Public Const ExplorerRank As Integer = &H9958 * 8

            Public Const Adventures As Integer = &H8B70 * 8

            Public Const WindowFrameType As Integer = &H995F * 8 + 5

            Public Const HeldMoney As Integer = &H990C * 8 + 6
            Public Const SPHeldMoney As Integer = &H990F * 8 + 6
            Public Const StoredMoney As Integer = &H9915 * 8 + 6

            Public Const StoredPokemonOffset As Integer = &H464 * 8
            Public Const StoredPokemonLength As Integer = 362
            Public Const StoredPokemonNumber As Integer = 720

            Public Const ActivePokemonOffset As Integer = &H83D9 * 8 + 1
            Public Const SpActivePokemonOffset As Integer = &H84F4 * 8 + 2
            Public Const ActivePokemonLength As Integer = 546
            Public Const ActivePokemonNumber As Integer = 4

            Public Const HeldItemOffset As Integer = &H8BA2 * 8
            Public Const HeldItemLength As Integer = 33
            Public Const HeldItemNumber As Integer = 50 '1st 50 are the team's, 2nd 50 are the Sp. Episode

            Public Const StoredItemOffset As Integer = &H8E0C * 8 + 6
            Public Const StoredItemLength As Integer = 2 * 11
            Public Const StoredItemNumber As Integer = 1000

            Public Const ItemShop1Offset As Integer = &H98CA * 8 + 6
            Public Const ItemShopLength As Integer = 22
            Public Const ItemShop1Number As Integer = 8
            Public Const ItemShop2Offset As Integer = &H98E0 * 8 + 6
            Public Const ItemShop2Number As Integer = 4

            Public Const AdventureLogOffset As Integer = &H9958 * 8
            Public Const AdventureLogLength As Integer = 447 'Not tested

            Public Const CroagunkShopOffset As Integer = &HB475 * 8
            Public Const CroagunkShopLength As Integer = 11
            Public Const CroagunkShopNumber As Integer = 8

            Public Const QuicksavePokemonNumber As Integer = 20
            Public Const QuicksavePokemonLength As Integer = 429 * 8
            Public Const QuicksavePokemonOffset As Integer = &H19000 * 8 + (&H3170 * 8)
        End Class

        Public Sub New()
            MyBase.New

            'Init Items
            StoredItems = New ObservableCollection(Of SkyStoredItem)
            HeldItems = New ObservableCollection(Of SkyHeldItem)
            SpEpisodeHeldItems = New ObservableCollection(Of SkyHeldItem)
        End Sub



        Public Overrides Async Function OpenFile(Filename As String, Provider As IOProvider) As Task
            Await MyBase.OpenFile(Filename, Provider)

            LoadGeneral()
            LoadItems()
            LoadActivePokemon()
            LoadStoredPokemon()
            LoadQuicksavePokemon()
            LoadHistory()
            LoadSettings()

        End Function

        Public Overrides Sub Save(Destination As String, provider As IOProvider)

            SaveGeneral()
            SaveItems()
            SaveActivePokemon()
            SaveStoredPokemon()
            SaveQuicksavePokemon()
            SaveHistory()
            SaveSettings()

            MyBase.Save(Destination, provider)
        End Sub

#Region "Events"
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Public Event Modified As INotifyModified.ModifiedEventHandler Implements INotifyModified.Modified
#End Region

#Region "Event Handlers"
        Private Sub OnCollectionChanged(sender As Object, e As EventArgs) Handles _storedItems.CollectionChanged, _heldItems.CollectionChanged, _spEpisodeHeldItems.CollectionChanged
            RaiseEvent Modified(Me, e)
        End Sub

        Private Sub OnModified(sender As Object, e As EventArgs)
            RaiseEvent Modified(Me, e)
        End Sub

        Private Sub Me_OnPropertyChanged(sender As Object, e As EventArgs) Handles Me.PropertyChanged
            RaiseEvent Modified(Me, e)
        End Sub
#End Region

#Region "Save Interaction"

#Region "General"

        Private Sub LoadGeneral()
            TeamName = Bits.StringPMD(0, Offsets.TeamNameStart, Offsets.TeamNameLength)
            HeldMoney = Bits.Int(0, Offsets.HeldMoney, 24)
            SpEpisodeHeldMoney = Bits.Int(0, Offsets.SPHeldMoney, 24)
            StoredMoney = Bits.Int(0, Offsets.StoredMoney, 24)
            Adventures = Bits.Int(0, Offsets.Adventures, 32)
            ExplorerRank = Bits.Int(0, Offsets.ExplorerRank, 32)
        End Sub

        Private Sub SaveGeneral()
            Bits.StringPMD(0, Offsets.TeamNameStart, Offsets.TeamNameLength) = TeamName
            Bits.Int(0, Offsets.HeldMoney, 24) = HeldMoney
            Bits.Int(0, Offsets.SPHeldMoney, 24) = SpEpisodeHeldMoney
            Bits.Int(0, Offsets.StoredMoney, 24) = StoredMoney
            Bits.Int(0, Offsets.Adventures, 32) = Adventures
            Bits.Int(0, Offsets.ExplorerRank, 32) = ExplorerRank
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

        ''' <summary>
        ''' Gets or sets the held money in the main game
        ''' </summary>
        ''' <returns></returns>
        Public Property HeldMoney As Integer
            Get
                Return _heldMoney
            End Get
            Set(value As Integer)
                If Not _heldMoney = value Then
                    _heldMoney = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(_heldMoney))
                End If
            End Set
        End Property
        Dim _heldMoney As Integer

        ''' <summary>
        ''' Gets or sets the held money in the active special episode
        ''' </summary>
        ''' <returns></returns>
        Public Property SpEpisodeHeldMoney As Integer
            Get
                Return _spEpisodeHeldMoney
            End Get
            Set(value As Integer)
                If Not _spEpisodeHeldMoney = value Then
                    _spEpisodeHeldMoney = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SpEpisodeHeldMoney)))
                End If
            End Set
        End Property
        Dim _spEpisodeHeldMoney As Integer

        ''' <summary>
        ''' Gets or sets the money in storage
        ''' </summary>
        ''' <returns></returns>
        Public Property StoredMoney As Integer
            Get
                Return Bits.Int(0, Offsets.StoredMoney, 24)
            End Get
            Set(value As Integer)
                If Not _storedMoney = value Then
                    _storedMoney = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(StoredMoney)))
                End If
            End Set
        End Property
        Dim _storedMoney As Integer

        ''' <summary>
        ''' Gets or sets the number of adventures the team has had.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>This is displayed as a signed integer in-game, so if this is set to a negative number, it will appear negative.</remarks>
        Public Property Adventures As Integer
            Get
                Return Bits.Int(0, Offsets.Adventures, 32)
            End Get
            Set(value As Integer)
                If Not _numAdventures = value Then
                    _numAdventures = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Adventures)))
                End If
            End Set
        End Property
        Dim _numAdventures As Integer

        ''' <summary>
        ''' Gets or sets the team's exploration rank points.
        ''' When set in certain ranges, the rank changes (ex. Silver, Gold, Master, etc).
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ExplorerRank As Integer
            Get
                Return _explorerRank
            End Get
            Set(value As Integer)
                If Not _explorerRank = value Then
                    _explorerRank = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ExplorerRank)))
                End If
            End Set
        End Property
        Dim _explorerRank As Integer

#End Region

#Region "Items"

        Private Sub LoadItems()
            'Stored Items
            StoredItems = New ObservableCollection(Of SkyStoredItem)
            Dim ids = Bits.Range(Offsets.StoredItemOffset, 11 * Offsets.StoredItemNumber)
            Dim params = Bits.Range(Offsets.StoredItemOffset + 11 * Offsets.StoredItemNumber, 11 * Offsets.StoredItemNumber)
            For count As Integer = 0 To 999
                Dim id = ids.NextInt(11)
                Dim p = params.NextInt(11)
                If id > 0 Then
                    StoredItems.Add(New SkyStoredItem(id, p))
                Else
                    Exit For
                End If
            Next

            'Held Items
            HeldItems = New ObservableCollection(Of SkyHeldItem)
            For count As Integer = 0 To Offsets.HeldItemNumber - 1
                Dim item = SkyHeldItem.FromHeldItemBits(Me.Bits.Range(Offsets.HeldItemOffset + count * Offsets.HeldItemLength, Offsets.HeldItemLength))
                If item.IsValid Then
                    HeldItems.Add(item)
                Else
                    Exit For
                End If
            Next

            'Special Episode Held Items
            SpEpisodeHeldItems = New ObservableCollection(Of SkyHeldItem)
            For count As Integer = Offsets.HeldItemNumber To Offsets.HeldItemNumber + Offsets.HeldItemNumber - 1
                Dim item = SkyHeldItem.FromHeldItemBits(Me.Bits.Range(Offsets.HeldItemOffset + count * Offsets.HeldItemLength, Offsets.HeldItemLength))
                If item.IsValid Then
                    SpEpisodeHeldItems.Add(item)
                Else
                    Exit For
                End If
            Next

            'Item slots
            Dim slots As New ObservableCollection(Of IItemSlot)
            slots.Add(New ItemSlot(Of SkyStoredItem)(My.Resources.Language.StoredItemsSlot, StoredItems, Offsets.StoredItemNumber))
            slots.Add(New ItemSlot(Of SkyHeldItem)(My.Resources.Language.HeldItemsSlot, HeldItems, Offsets.HeldItemNumber))
            slots.Add(New ItemSlot(Of SkyHeldItem)(My.Resources.Language.EpisodeHeldItems, SpEpisodeHeldItems, Offsets.HeldItemNumber))
            ItemSlots = slots
        End Sub

        Private Sub SaveItems()
            'Stored Items
            Dim ids As New Binary(11 * Offsets.StoredItemNumber)
            Dim params As New Binary(11 * Offsets.StoredItemNumber)
            For count As Integer = 0 To Offsets.StoredItemNumber - 1
                If StoredItems.Count > count Then
                    ids.NextInt(11) = StoredItems(count).ID
                    params.NextInt(11) = StoredItems(count).GetParameter
                Else
                    ids.NextInt(11) = 0
                    params.NextInt(11) = 0
                End If
            Next
            Bits.Range(Offsets.StoredItemOffset, 11 * Offsets.StoredItemNumber) = ids
            Bits.Range(Offsets.StoredItemOffset + 11 * Offsets.StoredItemNumber, 11 * Offsets.StoredItemNumber) = params

            'Held Items
            For count As Integer = 0 To Offsets.HeldItemNumber - 1
                Dim index = Offsets.HeldItemOffset + count * Offsets.HeldItemLength
                If HeldItems.Count > count Then
                    Me.Bits.Range(index, Offsets.HeldItemLength) = HeldItems(count).GetHeldItemBits
                Else
                    Me.Bits.Range(index, Offsets.HeldItemLength) = New Binary(Offsets.HeldItemLength)
                End If
            Next

            'Special Episode Held Items
            For count As Integer = Offsets.HeldItemNumber To Offsets.HeldItemNumber + Offsets.HeldItemNumber - 1
                Dim index = Offsets.HeldItemOffset + count * Offsets.HeldItemLength
                If SpEpisodeHeldItems.Count > count Then
                    Me.Bits.Range(index, Offsets.HeldItemLength) = SpEpisodeHeldItems(count).GetHeldItemBits
                Else
                    Me.Bits.Range(index, Offsets.HeldItemLength) = New Binary(Offsets.HeldItemLength)
                End If
            Next
        End Sub

        Public Property StoredItems As ObservableCollection(Of SkyStoredItem)
            Get
                Return _storedItems
            End Get
            Set(value As ObservableCollection(Of SkyStoredItem))
                If _storedItems IsNot value Then
                    _storedItems = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(StoredItems)))
                End If
            End Set
        End Property
        Private WithEvents _storedItems As ObservableCollection(Of SkyStoredItem)

        Public Property HeldItems As ObservableCollection(Of SkyHeldItem)
            Get
                Return _heldItems
            End Get
            Set(value As ObservableCollection(Of SkyHeldItem))
                If _heldItems IsNot value Then
                    _heldItems = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HeldItems)))
                End If
            End Set
        End Property
        Private WithEvents _heldItems As ObservableCollection(Of SkyHeldItem)

        Public Property SpEpisodeHeldItems As ObservableCollection(Of SkyHeldItem)
            Get
                Return _spEpisodeHeldItems
            End Get
            Set(value As ObservableCollection(Of SkyHeldItem))
                If _spEpisodeHeldItems IsNot value Then
                    _spEpisodeHeldItems = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SpEpisodeActivePokemon)))
                End If
            End Set
        End Property
        Private WithEvents _spEpisodeHeldItems As ObservableCollection(Of SkyHeldItem)

        Public Property ItemSlots As IEnumerable(Of IItemSlot) Implements IInventory.ItemSlots
            Get
                Return _itemSlots
            End Get
            Private Set(value As IEnumerable(Of IItemSlot))
                _itemSlots = value
            End Set
        End Property
        Dim _itemSlots As ObservableCollection(Of IItemSlot)

#End Region

#Region "Stored Pokemon"
        Private Sub LoadStoredPokemon()
            StoredPlayerPartner = New ObservableCollection(Of SkyStoredPokemon)
            StoredSpEpisodePokemon = New ObservableCollection(Of SkyStoredPokemon)
            StoredPokemon = New ObservableCollection(Of SkyStoredPokemon)

            For count = 0 To Offsets.StoredPokemonNumber
                Dim pkm As New SkyStoredPokemon(Bits.Range(Offsets.StoredPokemonOffset + count * Offsets.StoredPokemonLength, Offsets.StoredPokemonLength))
                AddHandler pkm.Modified, AddressOf OnModified
                AddHandler pkm.PropertyChanged, AddressOf OnModified

                If count < 2 Then 'Player Partner
                    StoredPlayerPartner.Add(pkm)
                ElseIf count < 5 Then 'Sp. Episode
                    StoredSpEpisodePokemon.Add(pkm)
                Else 'Others
                    StoredPokemon.Add(pkm)
                End If
            Next

            _storage = New ObservableCollection(Of IPokemonBox)
            _storage.Add(New BasicPokemonBox(My.Resources.Language.PlayerPartnerPokemonSlot, StoredPlayerPartner))
            _storage.Add(New BasicPokemonBox(My.Resources.Language.SpEpisodePokemonSlot, StoredSpEpisodePokemon))
            _storage.Add(New BasicPokemonBox(My.Resources.Language.StoredPokemonSlot, StoredPokemon))
        End Sub
        Private Sub SaveStoredPokemon()
            For count = 0 To Offsets.StoredPokemonNumber
                Dim pkm As SkyStoredPokemon
                If count < 2 Then 'Player Partner
                    pkm = StoredPlayerPartner(count)
                ElseIf count < 5 Then 'Sp. Episode
                    pkm = StoredSpEpisodePokemon(count - 2)
                Else 'Others
                    pkm = StoredPokemon(count - 5)
                End If
                Bits.Range(Offsets.StoredPokemonOffset + count * Offsets.StoredPokemonLength, Offsets.StoredPokemonLength) = pkm.GetStoredPokemonBits
            Next
        End Sub
        Public Property StoredPlayerPartner As ObservableCollection(Of SkyStoredPokemon)
        Public Property StoredSpEpisodePokemon As ObservableCollection(Of SkyStoredPokemon)
        Public Property StoredPokemon As ObservableCollection(Of SkyStoredPokemon)

        Public ReadOnly Property Storage As IEnumerable(Of IPokemonBox) Implements IPokemonStorage.Storage
            Get
                Return _storage
            End Get
        End Property
        Dim _storage As ObservableCollection(Of IPokemonBox)

#End Region

#Region "Active Pokemon"

        Private Sub LoadActivePokemon()
            Dim activePokemon As New ObservableCollection(Of SkyActivePokemon)
            Dim spEpisodeActivePokemon As New ObservableCollection(Of SkyActivePokemon)
            For count As Integer = 0 To Offsets.ActivePokemonNumber - 1
                Dim main = New SkyActivePokemon(Me.Bits.Range(Offsets.ActivePokemonOffset + count * Offsets.ActivePokemonLength, Offsets.ActivePokemonLength))
                Dim special = New SkyActivePokemon(Me.Bits.Range(Offsets.SpActivePokemonOffset + count * Offsets.ActivePokemonLength, Offsets.ActivePokemonLength))

                AddHandler main.Modified, AddressOf OnModified
                AddHandler main.PropertyChanged, AddressOf OnModified
                AddHandler special.Modified, AddressOf OnModified
                AddHandler special.PropertyChanged, AddressOf OnModified

                activePokemon.Add(main)
                spEpisodeActivePokemon.Add(special)
            Next

            Me.ActivePokemon = activePokemon
            Me.SpEpisodeActivePokemon = spEpisodeActivePokemon
        End Sub

        Private Sub SaveActivePokemon()
            For count As Integer = 0 To Offsets.ActivePokemonNumber - 1
                Me.Bits.Range(Offsets.ActivePokemonOffset + count * Offsets.ActivePokemonLength, Offsets.ActivePokemonLength) = ActivePokemon(count).GetActivePokemonBits
                Me.Bits.Range(Offsets.SpActivePokemonOffset + count * Offsets.ActivePokemonLength, Offsets.ActivePokemonLength) = SpEpisodeActivePokemon(count).GetActivePokemonBits
            Next
        End Sub

        Public Property ActivePokemon As ObservableCollection(Of SkyActivePokemon)
            Get
                Return _activePokemon
            End Get
            Set(value As ObservableCollection(Of SkyActivePokemon))
                If _activePokemon IsNot value Then
                    _activePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ActivePokemon)))
                End If
            End Set
        End Property
        Dim _activePokemon As ObservableCollection(Of SkyActivePokemon)

        Protected Property Party As IEnumerable Implements IParty.Party
            Get
                Return ActivePokemon
            End Get
            Set(value As IEnumerable)
                ActivePokemon = value
            End Set
        End Property

        Public Property SpEpisodeActivePokemon As ObservableCollection(Of SkyActivePokemon)
            Get
                Return _spEpisodeActivePokemon
            End Get
            Set(value As ObservableCollection(Of SkyActivePokemon))
                If _spEpisodeActivePokemon IsNot value Then
                    _spEpisodeActivePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SpEpisodeActivePokemon)))
                End If
            End Set
        End Property
        Dim _spEpisodeActivePokemon As ObservableCollection(Of SkyActivePokemon)

#End Region

#Region "Quicksave Pokemon"
        Private Sub LoadQuicksavePokemon()
            _quicksavePokemon = New ObservableCollection(Of SkyQuicksavePokemon)
            For count = 0 To Offsets.QuicksavePokemonNumber - 1
                Dim quick = New SkyQuicksavePokemon(Bits.Range(Offsets.QuicksavePokemonOffset + Offsets.QuicksavePokemonLength * count, Offsets.QuicksavePokemonLength))
                AddHandler quick.Modified, AddressOf OnModified
                AddHandler quick.PropertyChanged, AddressOf OnModified
                _quicksavePokemon.Add(quick)
            Next
        End Sub
        Private Sub SaveQuicksavePokemon()
            For count = 0 To Offsets.QuicksavePokemonNumber - 1
                If QuicksavePokemon.Count > count Then
                    Bits.Range(Offsets.QuicksavePokemonOffset + Offsets.QuicksavePokemonLength * count, Offsets.QuicksavePokemonLength) = QuicksavePokemon(count).GetQuicksavePokemonBits
                Else
                    QuicksavePokemon(count) = New SkyQuicksavePokemon()
                End If
            Next
        End Sub

        Public Property QuicksavePokemon As ObservableCollection(Of SkyQuicksavePokemon)
            Get
                Return _quicksavePokemon
            End Get
            Set(value As ObservableCollection(Of SkyQuicksavePokemon))
                If _quicksavePokemon IsNot value Then
                    _quicksavePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(QuicksavePokemon)))
                End If
            End Set
        End Property
        Dim _quicksavePokemon As ObservableCollection(Of SkyQuicksavePokemon)

#End Region

#Region "History"
        Private Sub LoadHistory()
            '----------
            '--History
            '----------

            '-----Original Player ID & Gender
            Dim rawOriginalPlayerID = Bits.Int(&HBE, 0, 16)
            If rawOriginalPlayerID > 600 Then
                OriginalPlayerID = rawOriginalPlayerID - 600
                OriginalPlayerIsFemale = True
            Else
                OriginalPlayerID = rawOriginalPlayerID
                OriginalPlayerIsFemale = False
            End If

            '-----Original Partner ID & Gender
            Dim rawOriginalPartnerID = Bits.Int(&HC0, 0, 16)
            If rawOriginalPartnerID > 600 Then
                OriginalPartnerID = rawOriginalPartnerID - 600
                OriginalPartnerIsFemale = True
            Else
                OriginalPartnerID = rawOriginalPartnerID
                OriginalPartnerIsFemale = False
            End If

            '-----Original Names
            OriginalPlayerName = Bits.StringPMD(&H13F, 0, 10)
            OriginalPartnerName = Bits.StringPMD(&H149, 0, 10)
        End Sub

        Private Sub SaveHistory()
            '----------
            '--History
            '----------

            '-----Original Player ID & Gender
            Dim rawOriginalPlayerID = OriginalPlayerID
            If OriginalPlayerIsFemale Then
                rawOriginalPlayerID += 600
            End If
            Bits.Int(&HBE, 0, 16) = rawOriginalPlayerID

            '-----Original Partner ID & Gender
            Dim rawOriginalPartnerID = OriginalPartnerIsFemale
            If OriginalPartnerIsFemale Then
                rawOriginalPartnerID += 600
            End If
            Bits.Int(&HC0, 0, 16) = rawOriginalPlayerID

            '-----Original Names
            Bits.StringPMD(&H13F, 0, 10) = OriginalPlayerName
            Bits.StringPMD(&H149, 0, 10) = OriginalPartnerName
        End Sub

        ''' <summary>
        ''' Gets or sets the original player Pokemon.
        ''' Used in-game for special episodes.
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalPlayerID As Integer
            Get
                Return _originalPlayerID
            End Get
            Set(value As Integer)
                If Not value = _originalPlayerID Then
                    _originalPlayerID = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(OriginalPartnerID)))
                End If
            End Set
        End Property
        Dim _originalPlayerID As Integer

        ''' <summary>
        ''' Gets or sets the original player gender.
        ''' Used in-game for special episodes.
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalPlayerIsFemale As Boolean
            Get
                Return _originalPlayerIsFemale
            End Get
            Set(value As Boolean)
                If Not value = _originalPlayerIsFemale Then
                    _originalPlayerIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(OriginalPlayerIsFemale)))
                End If
            End Set
        End Property
        Dim _originalPlayerIsFemale As Boolean

        ''' <summary>
        ''' Gets or sets the original partner Pokemon.
        ''' Used in-game for special episodes.
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalPartnerID As Integer
            Get
                Return _originalPartnerID
            End Get
            Set(value As Integer)
                If Not _originalPartnerID = value Then
                    _originalPartnerID = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(OriginalPartnerID)))
                End If
            End Set
        End Property
        Dim _originalPartnerID As Integer

        ''' <summary>
        ''' Gets or sets the original partner gender.
        ''' Used in-game for special episodes.
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalPartnerIsFemale As Boolean
            Get
                Return _originalPartnerIsFemale
            End Get
            Set(value As Boolean)
                If Not _originalPartnerIsFemale = value Then
                    _originalPartnerIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(OriginalPartnerIsFemale)))
                End If
            End Set
        End Property
        Dim _originalPartnerIsFemale As Boolean

        ''' <summary>
        ''' Gets or sets the original player name.
        ''' Used in-game for special episodes.
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalPlayerName As String
            Get
                Return _originalPlayerName
            End Get
            Set(value As String)
                If Not _originalPlayerName = value Then
                    _originalPlayerName = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(OriginalPlayerName)))
                End If
            End Set
        End Property
        Dim _originalPlayerName As String

        ''' <summary>
        ''' Gets or sets the original partner name.
        ''' Used in-game for special episodes.
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalPartnerName As String
            Get
                Return _originalPartnerName
            End Get
            Set(value As String)
                If Not _originalPartnerName = value Then
                    _originalPartnerName = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(OriginalPartnerName)))
                End If
            End Set
        End Property
        Dim _originalPartnerName As String
#End Region

#Region "Settings"
        Private Sub LoadSettings()
            WindowFrameType = Bits.Int(0, Offsets.WindowFrameType, 3)
        End Sub
        Private Sub SaveSettings()
            Bits.Int(0, Offsets.WindowFrameType, 3) = WindowFrameType
        End Sub

        ''' <summary>
        ''' Gets or sets the type of window frame used in the game.  Must be 1-5.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property WindowFrameType As Byte
            Get
                Return _windowFrameType
            End Get
            Set(value As Byte)
                If Not _windowFrameType = value Then
                    _windowFrameType = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(WindowFrameType)))
                End If
            End Set
        End Property
        Dim _windowFrameType As Byte

#End Region

#End Region

#Region "Technical Stuff"

        ''' <summary>
        ''' Fixes the save file's checksum to reflect any changes made
        ''' </summary>
        Protected Overrides Sub FixChecksum()
            'Fix the first checksum
            Dim buffer = BitConverter.GetBytes(Checksums.Calculate32BitChecksum(Bits, 4, Offsets.ChecksumEnd))
            For count = 0 To 3
                Bits.Int(count, 0, 8) = buffer(count)
            Next

            'Ensure backup save matches.
            'Not strictly required, as the first one will be loaded if it's valid, but looks nicer.
            CopyToBackup()

            'Quicksave checksum
            buffer = BitConverter.GetBytes(Checksums.Calculate32BitChecksum(Bits, Offsets.QuicksaveChecksumStart, Offsets.QuicksaveChecksumEnd))
            For x As Byte = 0 To 3
                Bits.Int(x + Offsets.QuicksaveStart, 0, 8) = buffer(x)
            Next
        End Sub

        ''' <summary>
        ''' Copies the primary save to the backup save.
        ''' </summary>
        Private Sub CopyToBackup()
            Dim e As Integer = Offsets.BackupSaveStart
            For i As Integer = 4 To e - 1
                Bits.Int(i + e, 0, 8) = Bits.Int(i, 0, 8)
            Next
        End Sub

        ''' <summary>
        ''' Determines whether or not the given file is a SkySave.
        ''' </summary>
        ''' <param name="File">File to determine the type of.</param>
        ''' <returns></returns>
        Public Function IsFileOfType(File As GenericFile) As Task(Of Boolean) Implements IDetectableFileType.IsOfType
            If File.Length > Offsets.ChecksumEnd Then
                Dim buffer = BitConverter.GetBytes(Checksums.Calculate32BitChecksum(File, 4, Offsets.ChecksumEnd))
                Return Task.FromResult(File.RawData(0) = buffer(0) AndAlso File.RawData(1) = buffer(1) AndAlso File.RawData(2) = buffer(2) AndAlso File.RawData(3) = buffer(3))
            Else
                Return Task.FromResult(False)
            End If
        End Function

#End Region

    End Class

End Namespace