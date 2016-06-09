Imports SkyEditor.Core.IO

Namespace MysteryDungeon.Rescue
    Public Class RBStoredPokemon
        Implements IOpenableFile
        Implements ISavableAs
        Implements IOnDisk
        Implements INotifyPropertyChanged
        Implements INotifyModified

        Public Const Length = 362
        Public Const MimeType As String = "application/x-rb-pokemon"

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
                Level = .Int(0, 0, 7)
                ID = .Int(0, 7, 9)
                MetAt = .Int(0, 16, 7)
                Unk1 = .Range(23, 21)
                IQ = .Int(0, 44, 10)
                HP = .Int(0, 54, 10)
                Attack = .Int(0, 64, 8)
                Defense = .Int(0, 72, 8)
                SpAttack = .Int(0, 80, 8)
                SpDefense = .Int(0, 88, 8)
                Exp = .Int(0, 96, 24)
                Unk2 = .Range(120, 43)
                Attack1 = New RBAttack(.Range(163, RBAttack.Length))
                Attack2 = New RBAttack(.Range(183, RBAttack.Length))
                Attack3 = New RBAttack(.Range(203, RBAttack.Length))
                Attack4 = New RBAttack(.Range(223, RBAttack.Length))
                Name = .StringPMD(0, 243, 10)
            End With
        End Sub

        Public Function GetStoredPokemonBits() As Binary
            Dim out As New Binary(Length)
            With out
                .Int(0, 0, 7) = Level
                .Int(0, 7, 9) = ID
                .Int(0, 16, 7) = MetAt
                .Range(23, 21) = Unk1
                .Int(0, 44, 10) = IQ
                .Int(0, 54, 10) = HP
                .Int(0, 64, 8) = Attack
                .Int(0, 72, 8) = Defense
                .Int(0, 80, 8) = SpAttack
                .Int(0, 88, 8) = SpDefense
                .Int(0, 96, 24) = Exp
                .Range(120, 43) = Unk2
                .Range(163, RBAttack.Length) = _attack1.GetAttackBits
                .Range(183, RBAttack.Length) = _attack2.GetAttackBits
                .Range(203, RBAttack.Length) = _attack3.GetAttackBits
                .Range(223, RBAttack.Length) = _attack4.GetAttackBits
                .StringPMD(0, 243, 10) = Name
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
            Return ".skypkm"
        End Function

        Public Sub Save(provider As IOProvider) Implements ISavable.Save
            Save(Filename, provider)
        End Sub

        Public Property Filename As String Implements IOnDisk.Filename

        Public Overrides Function ToString() As String
            If ID > 0 Then
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

        Public Property MetAt As Integer
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

        'Public Property MetFloor As Integer
        '    Get
        '        Return _metFloor
        '    End Get
        '    Set(value As Integer)
        '        If Not _metFloor = value Then
        '            _metFloor = value
        '        End If
        '    End Set
        'End Property
        'Dim _metFloor As Integer

        Public Property IQ As Integer
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

        Public Property HP As Integer
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

        Public Property Attack1 As RBAttack
            Get
                Return _attack1
            End Get
            Set(value As RBAttack)
                If _attack1 IsNot value Then
                    _attack1 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Attack1)))
                End If
            End Set
        End Property
        Private WithEvents _attack1 As RBAttack

        Public Property Attack2 As RBAttack
            Get
                Return _attack2
            End Get
            Set(value As RBAttack)
                If _attack2 IsNot value Then
                    _attack2 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Attack2)))
                End If
            End Set
        End Property
        Private WithEvents _attack2 As RBAttack

        Public Property Attack3 As RBAttack
            Get
                Return _attack3
            End Get
            Set(value As RBAttack)
                If _attack3 IsNot value Then
                    _attack3 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Attack3)))
                End If
            End Set
        End Property
        Private WithEvents _attack3 As RBAttack

        Public Property Attack4 As RBAttack
            Get
                Return _attack4
            End Get
            Set(value As RBAttack)
                If _attack4 IsNot value Then
                    _attack4 = value
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Attack4)))
                End If
            End Set
        End Property
        Private WithEvents _attack4 As RBAttack

        Public Property Name As String
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

        Public ReadOnly Property PokemonNames As Dictionary(Of Integer, String)
            Get
                Return Lists.RBPokemon
            End Get
        End Property

        Public ReadOnly Property LocationNames As Dictionary(Of Integer, String)
            Get
                Return Lists.RBLocations
            End Get
        End Property

#End Region
    End Class
End Namespace

