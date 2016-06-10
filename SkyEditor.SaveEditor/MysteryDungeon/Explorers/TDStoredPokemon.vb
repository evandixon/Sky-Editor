Imports SkyEditor.Core.IO

Namespace MysteryDungeon.Explorers
    Public Class TDStoredPokemon
        Implements IExplorersStoredPokemon
        Implements IOpenableFile
        Implements ISavableAs
        Implements IOnDisk
        Implements INotifyPropertyChanged
        Implements INotifyModified

        Public Const Length = 388
        Public Const MimeType As String = "application/x-td-pokemon"

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
                'IsValid = .Bit(0) 'Not 100% sure about this
                Level = .Int(0, 1, 7)

                Dim idRaw As Integer = .Int(0, 8, 11)
                If idRaw > 600 Then
                    IsFemale = True
                    ID = idRaw - 600
                Else
                    IsFemale = False
                    ID = idRaw
                End If

                MetAt = .Int(0, 19, 8)
                MetFloor = .Int(0, 27, 7)

                Unk1 = .Range(34, 15)

                IQ = .Int(0, 49, 10)
                HP = .Int(0, 59, 10)
                Attack = .Int(0, 69, 8)
                Defense = .Int(0, 77, 8)
                SpAttack = .Int(0, 85, 8)
                SpDefense = .Int(0, 93, 8)
                Exp = .Int(0, 101, 24)

                Unk2 = .Range(125, 96)

                Attack1 = New ExplorersAttack(.Range(221, ExplorersAttack.Length))
                Attack2 = New ExplorersAttack(.Range(242, ExplorersAttack.Length))
                Attack3 = New ExplorersAttack(.Range(263, ExplorersAttack.Length))
                Attack4 = New ExplorersAttack(.Range(284, ExplorersAttack.Length))
                Name = .StringPMD(0, 305, 10)
            End With
        End Sub

        Public Function GetStoredPokemonBits() As Binary
            Dim out As New Binary(Length)
            With out
                '.Bit(0) = IsValid
                .Int(0, 1, 7) = Level

                If IsFemale Then
                    .Int(0, 8, 11) = ID + 600
                Else
                    .Int(0, 8, 11) = ID
                End If

                .Int(0, 19, 8) = MetAt
                .Int(0, 27, 7) = MetFloor

                .Range(34, 15) = Unk1

                .Int(0, 49, 10) = IQ
                .Int(0, 59, 10) = HP
                .Int(0, 69, 8) = Attack
                .Int(0, 77, 8) = Defense
                .Int(0, 85, 8) = SpAttack
                .Int(0, 93, 8) = SpDefense
                .Int(0, 101, 24) = Exp

                .Range(125, 96) = Unk2

                .Range(221, ExplorersAttack.Length) = _attack1.GetAttackBits
                .Range(242, ExplorersAttack.Length) = _attack2.GetAttackBits
                .Range(263, ExplorersAttack.Length) = _attack3.GetAttackBits
                .Range(284, ExplorersAttack.Length) = _attack4.GetAttackBits
                .StringPMD(0, 305, 10) = Name
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
            toSave.Bits.Bits.AddRange(GetStoredPokemonBits)
            toSave.Save(Filename, provider)
            RaiseEvent FileSaved(Me, New EventArgs)
        End Sub

        Public Function GetDefaultExtension() As String Implements ISavableAs.GetDefaultExtension
            Return ".tdpkm"
        End Function

        Public Sub Save(provider As IOProvider) Implements ISavable.Save
            Save(Filename, provider)
        End Sub

        Public Property Filename As String Implements IOnDisk.Filename

        Public Overrides Function ToString() As String
            If IsValid Then
                Return String.Format(My.Resources.Language.SkyStoredPokemonToString, Name, Level, Lists.ExplorersPokemon(ID))
            Else
                Return My.Resources.Language.BlankPokemon
            End If
        End Function

        Private Sub OnAttackModified(sender As Object, e As PropertyChangedEventArgs) Handles _attack1.PropertyChanged, _attack2.PropertyChanged, _attack3.PropertyChanged, _attack4.PropertyChanged
            RaiseEvent Modified(Me, e)
        End Sub

#Region "Properties"
        Private Property Unk1 As Binary
        Private Property Unk2 As Binary

        Public ReadOnly Property IsValid As Boolean
            Get
                Return ID > 0
            End Get
        End Property

        Public Property Level As Byte Implements IExplorersStoredPokemon.Level
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

        Public Property ID As Integer Implements IExplorersStoredPokemon.ID
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

        Public Property IsFemale As Boolean Implements IExplorersStoredPokemon.IsFemale
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

        Public Property MetAt As Integer Implements IExplorersStoredPokemon.MetAt
            Get
                Return _metAt
            End Get
            Set(value As Integer)
                If Not _metAt = value Then
                    _metAt = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(MetAt)))
                End If
            End Set
        End Property
        Dim _metAt As Integer

        Public Property MetFloor As Integer Implements IExplorersStoredPokemon.MetFloor
            Get
                Return _metFloor
            End Get
            Set(value As Integer)
                If Not _metFloor = value Then
                    _metFloor = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(MetFloor)))
                End If
            End Set
        End Property
        Dim _metFloor As Integer

        Public Property IQ As Integer Implements IExplorersStoredPokemon.IQ
            Get
                Return _iq
            End Get
            Set(value As Integer)
                If Not _iq = value Then
                    _iq = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(IQ)))
                End If
            End Set
        End Property
        Dim _iq As Integer

        Public Property HP As Integer Implements IExplorersStoredPokemon.HP
            Get
                Return _hp
            End Get
            Set(value As Integer)
                If Not _hp = value Then
                    _hp = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HP)))
                End If
            End Set
        End Property
        Dim _hp As Integer

        Public Property Attack As Byte Implements IExplorersStoredPokemon.Attack
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

        Public Property Defense As Byte Implements IExplorersStoredPokemon.Defense
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

        Public Property SpAttack As Byte Implements IExplorersStoredPokemon.SpAttack
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

        Public Property SpDefense As Byte Implements IExplorersStoredPokemon.SpDefense
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

        Public Property Exp As Integer Implements IExplorersStoredPokemon.Exp
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

        Public Property Attack1 As IMDAttack Implements IExplorersStoredPokemon.Attack1
            Get
                Return _attack1
            End Get
            Set(value As IMDAttack)
                If _attack1 IsNot value Then
                    _attack1 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Attack1)))
                End If
            End Set
        End Property
        Private WithEvents _attack1 As ExplorersAttack

        Public Property Attack2 As IMDAttack Implements IExplorersStoredPokemon.Attack2
            Get
                Return _attack2
            End Get
            Set(value As IMDAttack)
                If _attack2 IsNot value Then
                    _attack2 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Attack2)))
                End If
            End Set
        End Property
        Private WithEvents _attack2 As ExplorersAttack

        Public Property Attack3 As IMDAttack Implements IExplorersStoredPokemon.Attack3
            Get
                Return _attack3
            End Get
            Set(value As IMDAttack)
                If _attack3 IsNot value Then
                    _attack3 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Attack3)))
                End If
            End Set
        End Property
        Private WithEvents _attack3 As ExplorersAttack

        Public Property Attack4 As IMDAttack Implements IExplorersStoredPokemon.Attack4
            Get
                Return _attack4
            End Get
            Set(value As IMDAttack)
                If _attack4 IsNot value Then
                    _attack4 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Attack4)))
                End If
            End Set
        End Property
        Private WithEvents _attack4 As ExplorersAttack

        Public Property Name As String Implements IExplorersStoredPokemon.Name
            Get
                Return _name
            End Get
            Set(value As String)
                If Not _name = value Then
                    _name = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Name)))
                End If
            End Set
        End Property
        Dim _name As String

        Public ReadOnly Property PokemonNames As Dictionary(Of Integer, String) Implements IExplorersStoredPokemon.PokemonNames
            Get
                Return Lists.ExplorersPokemon
            End Get
        End Property

        Public ReadOnly Property LocationNames As Dictionary(Of Integer, String) Implements IExplorersStoredPokemon.LocationNames
            Get
                Return Lists.TDLocations
            End Get
        End Property

#End Region
    End Class

End Namespace
