Imports SkyEditor.Core.IO

Namespace MysteryDungeon.Explorers
    Public Class SkyQuicksavePokemon
        Implements IOpenableFile
        Implements ISavableAs
        Implements IOnDisk
        Implements INotifyPropertyChanged
        Implements INotifyModified

        Public Const Length = 429 * 8
        Public Const MimeType As String = "application/x-sky-quicksave-pokemon"

        Public Event FileSaved As ISavable.FileSavedEventHandler Implements ISavable.FileSaved
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Public Event Modified As INotifyModified.ModifiedEventHandler Implements INotifyModified.Modified

        Public Sub New()
            Unk1 = New Binary(15)
            Unk2 = New Binary(73)
        End Sub

        Public Sub New(bits As Binary)
            Initialize(bits)
        End Sub

        Private Sub Initialize(bits As Binary)
            With bits
                Unk1 = .Range(0, 80)
                Dim transIdRaw As Integer = .Int(0, 80, 16)
                If transIdRaw > 600 Then
                    TransformedIsFemale = True
                    TransformedID = transIdRaw - 600
                Else
                    TransformedIsFemale = False
                    TransformedID = transIdRaw
                End If

                Dim idRaw As Integer = .Int(0, 96, 16)
                If idRaw > 600 Then
                    IsFemale = True
                    ID = idRaw - 600
                Else
                    IsFemale = False
                    ID = idRaw
                End If
                Unk2 = .Range(112, 48)
                Level = .Int(0, 144, 8)
                Unk3 = .Range(152, 48)
                CurrentHP = .Int(0, 192, 16)
                MaxHP = .Int(0, 208, 16)
                HPBoost = .Int(0, 224, 16)
                Unk4 = .Range(240, 32)
                Attack = .Int(0, 256, 8)
                Defense = .Int(0, 264, 8)
                SpAttack = .Int(0, 272, 8)
                SpDefense = .Int(0, 280, 8)
                Exp = .Int(0, 288, 32)
                Unk5 = .Range(320, 2408)
                Attack1 = New SkyQuicksaveAttack(.Range(2696 + 0 * SkyQuicksaveAttack.Length, SkyQuicksaveAttack.Length))
                Attack2 = New SkyQuicksaveAttack(.Range(2696 + 1 * SkyQuicksaveAttack.Length, SkyQuicksaveAttack.Length))
                Attack3 = New SkyQuicksaveAttack(.Range(2696 + 2 * SkyQuicksaveAttack.Length, SkyQuicksaveAttack.Length))
                Attack4 = New SkyQuicksaveAttack(.Range(2696 + 3 * SkyQuicksaveAttack.Length, SkyQuicksaveAttack.Length))
            End With
        End Sub

        Public Function GetQuicksavePokemonBits() As Binary
            Dim out As New Binary(Length)
            With out
                .Range(0, 80) = Unk1

                If TransformedIsFemale Then
                    .Int(0, 80, 16) = TransformedID + 600
                Else
                    .Int(0, 80, 16) = TransformedID
                End If

                If IsFemale Then
                    .Int(0, 96, 16) = ID + 600
                Else
                    .Int(0, 96, 16) = ID
                End If

                .Range(112, 48) = Unk2
                .Int(0, 144, 8) = Level
                .Range(152, 48) = Unk3
                .Int(0, 192, 16) = CurrentHP
                .Int(0, 208, 16) = MaxHP
                .Int(0, 224, 16) = HPBoost
                .Range(240, 32) = Unk4
                .Int(0, 256, 8) = Attack
                .Int(0, 264, 8) = Defense
                .Int(0, 272, 8) = SpAttack
                .Int(0, 280, 8) = SpDefense
                .Int(0, 288, 32) = Exp
                .Range(320, 2408) = Unk5

                .Range(2696 + 0 * SkyQuicksaveAttack.Length, SkyQuicksaveAttack.Length) = _attack1.GetAttackBits
                .Range(2696 + 1 * SkyQuicksaveAttack.Length, SkyQuicksaveAttack.Length) = _attack2.GetAttackBits
                .Range(2696 + 2 * SkyQuicksaveAttack.Length, SkyQuicksaveAttack.Length) = _attack3.GetAttackBits
                .Range(2696 + 3 * SkyQuicksaveAttack.Length, SkyQuicksaveAttack.Length) = _attack4.GetAttackBits
            End With
            Return out
        End Function

        Public Async Function OpenFile(Filename As String, Provider As IOProvider) As Task Implements IOpenableFile.OpenFile
            Dim toOpen As New BinaryFile
            Await toOpen.OpenFile(Filename, Provider)

            'matix2267's convention adds 6 bits to the beginning of a file so that the name will be byte-aligned
            For i = 1 To 8 - (Length Mod 8)
                toOpen.Bits.Bits.RemoveAt(0)
            Next

            Initialize(toOpen.Bits)
        End Function

        Public Sub Save(Filename As String, provider As IOProvider) Implements ISavableAs.Save
            Dim toSave As New BinaryFile()
            toSave.CreateFile(Path.GetFileNameWithoutExtension(Filename))
            'matix2267's convention adds 6 bits to the beginning of a file so that the name will be byte-aligned
            For i = 1 To 8 - (Length Mod 8)
                toSave.Bits.Bits.Add(0)
            Next
            toSave.Bits.Bits.AddRange(GetQuicksavePokemonBits)
            toSave.Save(Filename, provider)
            RaiseEvent FileSaved(Me, New EventArgs)
        End Sub

        Public Function GetDefaultExtension() As String Implements ISavableAs.GetDefaultExtension
            Return ".skypkm"
        End Function

        Public Sub Save(provider As IOProvider) Implements ISavable.Save
            Save(Filename, provider)
        End Sub

        Public Property Filename As String Implements IOnDisk.Filename

        Private Sub OnAttackModified(sender As Object, e As PropertyChangedEventArgs) Handles _attack1.PropertyChanged, _attack2.PropertyChanged, _attack3.PropertyChanged, _attack4.PropertyChanged
            RaiseEvent Modified(Me, e)
        End Sub

#Region "Properties"
        Private Property Unk1 As Binary
        Private Property Unk2 As Binary
        Private Property Unk3 As Binary
        Private Property Unk4 As Binary
        Private Property Unk5 As Binary

        Public ReadOnly Property IsValid As Boolean
            Get
                Return ID > 0
            End Get
        End Property

        Public Property Level As Byte
            Get
                Return _level
            End Get
            Set(value As Byte)
                If Not _level = value Then
                    _level = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Level)))
                End If
            End Set
        End Property
        Dim _level As Byte

        Public Property ID As Integer
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

        Public Property IsFemale As Boolean
            Get
                Return _isFemale
            End Get
            Set(value As Boolean)
                If Not _isFemale = value Then
                    _isFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(IsFemale)))
                End If
            End Set
        End Property
        Dim _isFemale As Boolean

        Public Property TransformedID As Integer
            Get
                Return _transformedID
            End Get
            Set(value As Integer)
                If Not _transformedID = value Then
                    _transformedID = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TransformedID)))
                End If
            End Set
        End Property
        Dim _transformedID As Integer

        Public Property TransformedIsFemale As Boolean
            Get
                Return _transformedIsFemale
            End Get
            Set(value As Boolean)
                If Not _transformedIsFemale = value Then
                    _transformedIsFemale = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(TransformedIsFemale)))
                End If
            End Set
        End Property
        Dim _transformedIsFemale As Boolean

        ''' <summary>
        ''' The index of the Pokemon in storage as stored in the save file.
        ''' </summary>
        ''' <returns></returns>
        Public Property RosterNumber As Integer
            Get
                Return _rosterNumber
            End Get
            Set(value As Integer)
                If Not _rosterNumber = value Then
                    _rosterNumber = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(RosterNumber)))
                End If
            End Set
        End Property
        Dim _rosterNumber As Integer

        Public Property CurrentHP As Integer
            Get
                Return _hp1
            End Get
            Set(value As Integer)
                If Not _hp1 = value Then
                    _hp1 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(CurrentHP)))
                End If
            End Set
        End Property
        Dim _hp1 As Integer

        Public Property MaxHP As Integer
            Get
                Return _hp2
            End Get
            Set(value As Integer)
                If Not _hp2 = value Then
                    _hp2 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(MaxHP)))
                End If
            End Set
        End Property
        Dim _hp2 As Integer

        Public Property HPBoost As Integer
            Get
                Return _hpBoost
            End Get
            Set(value As Integer)
                If Not _hpBoost = value Then
                    _hpBoost = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HPBoost)))
                End If
            End Set
        End Property
        Dim _hpBoost As Integer

        Public Property Attack As Byte
            Get
                Return _attack
            End Get
            Set(value As Byte)
                If Not _attack = value Then
                    _attack = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Attack)))
                End If
            End Set
        End Property
        Dim _attack As Byte

        Public Property Defense As Byte
            Get
                Return _defense
            End Get
            Set(value As Byte)
                If Not _defense = value Then
                    _defense = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Defense)))
                End If
            End Set
        End Property
        Dim _defense As Byte

        Public Property SpAttack As Byte
            Get
                Return _spAttack
            End Get
            Set(value As Byte)
                If Not _spAttack = value Then
                    _spAttack = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SpAttack)))
                End If
            End Set
        End Property
        Dim _spAttack As Byte

        Public Property SpDefense As Byte
            Get
                Return _spDefense
            End Get
            Set(value As Byte)
                If Not _spDefense = value Then
                    _spDefense = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(SpDefense)))
                End If
            End Set
        End Property
        Dim _spDefense As Byte

        Public Property Exp As Integer
            Get
                Return _exp
            End Get
            Set(value As Integer)
                If Not _exp = value Then
                    _exp = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Exp)))
                End If
            End Set
        End Property
        Dim _exp As Integer

        Public Property Attack1 As IExplorersAttack
            Get
                Return _attack1
            End Get
            Set(value As IExplorersAttack)
                If _attack1 IsNot value Then
                    _attack1 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Attack1)))
                End If
            End Set
        End Property
        Private WithEvents _attack1 As SkyQuicksaveAttack

        Public Property Attack2 As IExplorersAttack
            Get
                Return _attack2
            End Get
            Set(value As IExplorersAttack)
                If _attack2 IsNot value Then
                    _attack2 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Attack2)))
                End If
            End Set
        End Property
        Private WithEvents _attack2 As SkyQuicksaveAttack

        Public Property Attack3 As IExplorersAttack
            Get
                Return _attack3
            End Get
            Set(value As IExplorersAttack)
                If _attack3 IsNot value Then
                    _attack3 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Attack3)))
                End If
            End Set
        End Property
        Private WithEvents _attack3 As SkyQuicksaveAttack

        Public Property Attack4 As IExplorersAttack
            Get
                Return _attack4
            End Get
            Set(value As IExplorersAttack)
                If _attack4 IsNot value Then
                    _attack4 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Attack4)))
                End If
            End Set
        End Property
        Private WithEvents _attack4 As SkyQuicksaveAttack


        Public ReadOnly Property PokemonNames As Dictionary(Of Integer, String)
            Get
                Return Lists.ExplorersPokemon
            End Get
        End Property

        Private ReadOnly Property LocationNames As Dictionary(Of Integer, String)
            Get
                Return Lists.GetSkyLocations
            End Get
        End Property

#End Region

        Public Overrides Function ToString() As String
            If IsValid Then
                Return String.Format("Lvl. {0} {1}", Level, Lists.ExplorersPokemon(ID))
            Else
                Return "----------"
            End If
        End Function
    End Class
End Namespace

