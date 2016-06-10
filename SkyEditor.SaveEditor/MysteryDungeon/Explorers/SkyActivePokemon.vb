Imports SkyEditor.Core.IO

Namespace MysteryDungeon.Explorers
    Public Class SkyActivePokemon
        Implements IExplorersStoredPokemon
        Implements IOpenableFile
        Implements ISavableAs
        Implements IOnDisk
        Implements INotifyPropertyChanged
        Implements INotifyModified

        Public Const Length = 546
        Public Const MimeType As String = "application/x-sky-active-pokemon"

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
                IsValid = .Bit(0)
                Unk1 = .Range(1, 4)
                Level = .Int(0, 5, 7)
                MetAt = .Int(0, 12, 8)
                MetFloor = .Int(0, 20, 7)
                Unk2 = .Range(27, 1)
                IQ = .Int(0, 28, 10)
                RosterNumber = .Int(0, 38, 10)
                Unk3 = .Range(48, 22)
                Dim idRaw As Integer = .Int(0, 70, 11)
                If idRaw > 600 Then
                    IsFemale = True
                    ID = idRaw - 600
                Else
                    IsFemale = False
                    ID = idRaw
                End If
                HP1 = .Int(0, 81, 10)
                HP2 = .Int(0, 91, 10)
                Attack = .Int(0, 101, 8)
                Defense = .Int(0, 109, 8)
                SpAttack = .Int(0, 117, 8)
                SpDefense = .Int(0, 125, 8)
                Exp = .Int(0, 133, 24)
                Attack1 = New SkyActiveAttack(.Range(157, SkyActiveAttack.Length))
                Attack2 = New SkyActiveAttack(.Range(186, SkyActiveAttack.Length))
                Attack3 = New SkyActiveAttack(.Range(215, SkyActiveAttack.Length))
                Attack4 = New SkyActiveAttack(.Range(244, SkyActiveAttack.Length))
                unk4 = .Range(273, 193)
                Name = .StringPMD(0, 466, 10)
            End With
        End Sub

        Public Function GetActivePokemonBits() As Binary
            Dim out As New Binary(Length)
            With out
                .Bit(0) = IsValid
                .Range(1, 4) = Unk1
                .Int(0, 5, 7) = Level
                .Int(0, 12, 8) = MetAt
                .Int(0, 20, 7) = MetFloor
                .Range(27, 1) = Unk2
                .Int(0, 28, 10) = IQ
                .Int(0, 38, 10) = RosterNumber
                .Range(48, 22) = Unk3
                If IsFemale Then
                    .Int(0, 70, 11) = ID + 600
                Else
                    .Int(0, 70, 11) = ID
                End If
                .Int(0, 81, 10) = HP1
                .Int(0, 91, 10) = HP2
                .Int(0, 101, 8) = Attack
                .Int(0, 109, 8) = Defense
                .Int(0, 117, 8) = SpAttack
                .Int(0, 125, 8) = SpDefense
                .Int(0, 133, 24) = Exp
                .Range(157, SkyActiveAttack.Length) = _attack1.GetAttackBits
                .Range(186, SkyActiveAttack.Length) = _attack2.GetAttackBits
                .Range(215, SkyActiveAttack.Length) = _attack3.GetAttackBits
                .Range(244, SkyActiveAttack.Length) = _attack4.GetAttackBits
                .Range(273, 193) = Unk4
                .StringPMD(0, 466, 10) = Name
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
            toSave.Bits.Bits.AddRange(GetActivePokemonBits)
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
        Private Property Unk3 As Binary
        Private Property Unk4 As Binary

        Public Property IsValid As Boolean
            Get
                Return _isValid
            End Get
            Set(value As Boolean)
                If Not _isValid = value Then
                    _isValid = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(IsValid)))
                End If
            End Set
        End Property
        Dim _isValid As Boolean

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

        Public Property HP1 As Integer Implements IExplorersStoredPokemon.HP
            Get
                Return _hp1
            End Get
            Set(value As Integer)
                If Not _hp1 = value Then
                    _hp1 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HP1)))
                End If
            End Set
        End Property
        Dim _hp1 As Integer

        Public Property HP2 As Integer
            Get
                Return _hp2
            End Get
            Set(value As Integer)
                If Not _hp2 = value Then
                    _hp2 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(HP2)))
                End If
            End Set
        End Property
        Dim _hp2 As Integer

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
        Private WithEvents _attack1 As SkyActiveAttack

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
        Private WithEvents _attack2 As SkyActiveAttack

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
        Private WithEvents _attack3 As SkyActiveAttack

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
        Private WithEvents _attack4 As SkyActiveAttack

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

        Private ReadOnly Property PokemonNames As Dictionary(Of Integer, String) Implements IExplorersStoredPokemon.PokemonNames
            Get
                Return Lists.ExplorersPokemon
            End Get
        End Property

        Private ReadOnly Property LocationNames As Dictionary(Of Integer, String) Implements IExplorersStoredPokemon.LocationNames
            Get
                Return Lists.GetSkyLocations
            End Get
        End Property

#End Region
    End Class
End Namespace
