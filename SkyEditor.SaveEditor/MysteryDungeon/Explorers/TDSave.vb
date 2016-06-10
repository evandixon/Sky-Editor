Imports System.Collections.Specialized
Imports SkyEditor.Core
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO
Imports SkyEditor.SaveEditor.Modeling

Namespace MysteryDungeon.Explorers
    Public Class TDSave
        Inherits BinaryFile
        Implements IDetectableFileType
        Implements INotifyPropertyChanged
        Implements INotifyModified
        Implements IInventory
        Implements IPokemonStorage
        Implements IParty

        Public Sub New()
            MyBase.New()
        End Sub

        Public Overrides Async Function OpenFile(Filename As String, Provider As IOProvider) As Task
            Await MyBase.OpenFile(Filename, Provider)

            LoadGeneral()
            LoadItems()
            LoadStoredPokemon()
            LoadActivePokemon
        End Function

        Public Overrides Sub Save(Destination As String, provider As IOProvider)
            SaveGeneral()
            SaveItems()
            SaveStoredPokemon()
            SaveActivePokemon

            MyBase.Save(Destination, provider)
        End Sub

#Region "Events"
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Public Event Modified As INotifyModified.ModifiedEventHandler Implements INotifyModified.Modified
#End Region

#Region "Event Handlers"
        Private Sub TDSave_PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Handles Me.PropertyChanged
            RaiseEvent Modified(Me, e)
        End Sub
        Private Sub On_CollectionChanged(sender As Object, e As NotifyCollectionChangedEventArgs) Handles _heldItems.CollectionChanged
            RaiseEvent Modified(Me, e)
        End Sub
        Private Sub OnModified(sender As Object, e As EventArgs)
            RaiseEvent Modified(Me, e)
        End Sub
#End Region



#Region "Child Classes"
        Friend Class Offsets
            Public Const ChecksumEnd As Integer = &HDC7B
            Public Const BackupSaveStart As Integer = &H10000
            Public Const QuicksaveStart As Integer = &H2E000
            Public Const QuicksaveChecksumStart As Integer = &H2E004
            Public Const QuicksaveChecksumEnd As Integer = &H2E0FF

            Public Const TeamNameStart As Integer = &H96F7 * 8
            Public Const TeamNameLength As Integer = 10

            Public Const HeldItemOffset As Integer = &H8B71 * 8
            Public Const HeldItemNumber As Integer = 48
            Public Const HeldItemLength As Integer = 31

            Public Const StoredPokemonOffset As Integer = &H460 * 8 + 3
            Public Const StoredPokemonLength As Integer = 388
            Public Const StoredPokemonNumber As Integer = 550

            Public Const ActivePokemonOffset As Integer = &H83CB * 8
            Public Const ActivePokemonLength As Integer = 544
            Public Const ActivePokemonNumber As Integer = 4
        End Class
#End Region

#Region "Save Interaction"

#Region "General"

        Sub LoadGeneral()
            TeamName = Bits.StringPMD(0, Offsets.TeamNameStart, Offsets.TeamNameLength)
        End Sub

        Sub SaveGeneral()
            Bits.StringPMD(0, Offsets.TeamNameStart, Offsets.TeamNameLength) = TeamName
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
#End Region

#Region "Items"
        Public Sub LoadItems()
            _heldItems = New ObservableCollection(Of TDHeldItem)
            For count As Integer = 0 To Offsets.HeldItemNumber - 1
                Dim i = TDHeldItem.FromHeldItemBits(Me.Bits.Range(Offsets.HeldItemOffset + count * Offsets.HeldItemLength, Offsets.HeldItemLength))
                If i.IsValid Then
                    _heldItems.Add(i)
                Else
                    Exit For
                End If
            Next

            InitItemSlots()
        End Sub

        Public Sub SaveItems()
            For count As Integer = 0 To Offsets.HeldItemNumber - 1
                If _heldItems.Count > count Then
                    Me.Bits.Range(Offsets.HeldItemOffset + count * Offsets.HeldItemLength, Offsets.HeldItemLength) = _heldItems(count).GetHeldItemBits
                Else
                    Me.Bits.Range(Offsets.HeldItemOffset + count * Offsets.HeldItemLength, Offsets.HeldItemLength) = New Binary(Offsets.HeldItemLength)
                End If
            Next
        End Sub

        Public Property HeldItems As ObservableCollection(Of TDHeldItem)
            Get
                Return _heldItems
            End Get
            Set(value As ObservableCollection(Of TDHeldItem))
                If _heldItems IsNot value Then
                    _heldItems = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HeldItems)))
                End If
            End Set
        End Property
        Private WithEvents _heldItems As ObservableCollection(Of TDHeldItem)

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
            slots.Add(New ItemSlot(Of TDHeldItem)(My.Resources.Language.HeldItemsSlot, HeldItems, Offsets.HeldItemNumber))
            ItemSlots = slots
        End Sub


#End Region

#Region "Stored Pokemon"
        Private Sub LoadStoredPokemon()
            StoredPlayerPartner = New ObservableCollection(Of TDStoredPokemon)
            StoredPokemon = New ObservableCollection(Of TDStoredPokemon)

            For count = 0 To Offsets.StoredPokemonNumber
                Dim pkm As New TDStoredPokemon(Bits.Range(Offsets.StoredPokemonOffset + count * Offsets.StoredPokemonLength, Offsets.StoredPokemonLength))
                AddHandler pkm.Modified, AddressOf OnModified
                AddHandler pkm.PropertyChanged, AddressOf OnModified

                If count < 2 Then 'Player Partner
                    StoredPlayerPartner.Add(pkm)
                Else 'Others
                    StoredPokemon.Add(pkm)
                End If
            Next

            _storage = New ObservableCollection(Of IPokemonBox)
            _storage.Add(New BasicPokemonBox(My.Resources.Language.PlayerPartnerPokemonSlot, StoredPlayerPartner))
            _storage.Add(New BasicPokemonBox(My.Resources.Language.StoredPokemonSlot, StoredPokemon))
        End Sub
        Private Sub SaveStoredPokemon()
            For count = 0 To Offsets.StoredPokemonNumber
                Dim pkm As TDStoredPokemon
                If count < 2 Then 'Player Partner
                    pkm = StoredPlayerPartner(count)
                Else 'Others
                    pkm = StoredPokemon(count - 2)
                End If
                Bits.Range(Offsets.StoredPokemonOffset + count * Offsets.StoredPokemonLength, Offsets.StoredPokemonLength) = pkm.GetStoredPokemonBits
            Next
        End Sub
        Public Property StoredPlayerPartner As ObservableCollection(Of TDStoredPokemon)
        Public Property StoredSpEpisodePokemon As ObservableCollection(Of TDStoredPokemon)
        Public Property StoredPokemon As ObservableCollection(Of TDStoredPokemon)

        Public ReadOnly Property Storage As IEnumerable(Of IPokemonBox) Implements IPokemonStorage.Storage
            Get
                Return _storage
            End Get
        End Property
        Dim _storage As ObservableCollection(Of IPokemonBox)

#End Region

#Region "Active Pokemon"

        Private Sub LoadActivePokemon()
            Dim activePokemon As New ObservableCollection(Of TDActivePokemon)
            Dim spEpisodeActivePokemon As New ObservableCollection(Of TDActivePokemon)
            For count As Integer = 0 To Offsets.ActivePokemonNumber - 1
                Dim main = New TDActivePokemon(Me.Bits.Range(Offsets.ActivePokemonOffset + count * Offsets.ActivePokemonLength, Offsets.ActivePokemonLength))

                AddHandler main.Modified, AddressOf OnModified
                AddHandler main.PropertyChanged, AddressOf OnModified

                activePokemon.Add(main)
            Next

            Me.ActivePokemon = activePokemon
        End Sub

        Private Sub SaveActivePokemon()
            For count As Integer = 0 To Offsets.ActivePokemonNumber - 1
                Me.Bits.Range(Offsets.ActivePokemonOffset + count * Offsets.ActivePokemonLength, Offsets.ActivePokemonLength) = ActivePokemon(count).GetActivePokemonBits
            Next
        End Sub

        Public Property ActivePokemon As ObservableCollection(Of TDActivePokemon)
            Get
                Return _activePokemon
            End Get
            Set(value As ObservableCollection(Of TDActivePokemon))
                If _activePokemon IsNot value Then
                    _activePokemon = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(ActivePokemon)))
                End If
            End Set
        End Property
        Dim _activePokemon As ObservableCollection(Of TDActivePokemon)

        Private Property IParty_Party As IEnumerable Implements IParty.Party
            Get
                Return ActivePokemon
            End Get
            Set(value As IEnumerable)
                ActivePokemon = value
            End Set
        End Property

#End Region

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

            'Quicksave checksum
            buffer = BitConverter.GetBytes(Checksums.Calculate32BitChecksum(Bits, Offsets.QuicksaveChecksumStart, Offsets.QuicksaveChecksumEnd))
            For x As Byte = 0 To 3
                Bits.Int(x + Offsets.QuicksaveStart, 0, 8) = buffer(x)
            Next
        End Sub
        Public Sub CopyToBackup()
            Dim e As Integer = Offsets.BackupSaveStart
            For i As Integer = 4 To e - 1
                Bits.Int(i + e, 0, 8) = Bits.Int(i, 0, 8)
            Next
        End Sub

        'Public Overrides Function DefaultSaveID() As String
        '    Return GameStrings.TDSave
        'End Function

        'Protected Overrides Sub PreSave()
        '    MyBase.PreSave()
        '    For count As Integer = 0 To Math.Ceiling(Bits.Count / 8) - 1
        '        RawData(count) = Bits.Int(count, 0, 8)
        '    Next
        'End Sub
#End Region

        Public Function IsFileOfType(File As GenericFile) As Task(Of Boolean) Implements IDetectableFileType.IsOfType
            If File.Length > Offsets.ChecksumEnd Then
                Dim buffer = BitConverter.GetBytes(Checksums.Calculate32BitChecksum(File, 4, Offsets.ChecksumEnd))
                Return Task.FromResult(File.RawData(0) = buffer(0) AndAlso File.RawData(1) = buffer(1) AndAlso File.RawData(2) = buffer(2) AndAlso File.RawData(3) = buffer(3))
            Else
                Return Task.FromResult(False)
            End If
        End Function

    End Class

End Namespace