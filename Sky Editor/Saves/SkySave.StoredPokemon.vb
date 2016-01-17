Imports SaveEditor.Interfaces
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace Saves
    Partial Class SkySave
        Implements iPokemonStorage
        Public Class Attack
            Inherits Binary
            Implements iMDAttack

            Public Const Length = 21
            Public Sub New(Bits As Binary)
                MyBase.New(Bits)
            End Sub
            Public Sub New(ID As Integer, Ginseng As Integer, IsSwitched As Boolean, IsLinked As Boolean, IsSet As Boolean)
                Me.New(New Binary(Length))
                Me.ID = ID
                Me.Ginseng = Ginseng
                Me.IsSwitched = IsSwitched
                Me.IsLinked = IsLinked
                Me.IsSet = IsSet
            End Sub
            Public Property IsValid As Boolean
                Get
                    Return Bits(0)
                End Get
                Set(value As Boolean)
                    Bits(0) = value
                End Set
            End Property
            Public Property IsLinked As Boolean Implements iMDAttack.IsLinked
                Get
                    Return Bits(1)
                End Get
                Set(value As Boolean)
                    Bits(1) = value
                End Set
            End Property
            Public Property IsSwitched As Boolean Implements iMDAttack.IsSwitched
                Get
                    Return Bits(2)
                End Get
                Set(value As Boolean)
                    Bits(2) = value
                End Set
            End Property
            Public Property IsSet As Boolean Implements iMDAttack.IsSet
                Get
                    Return Bits(3)
                End Get
                Set(value As Boolean)
                    Bits(3) = value
                End Set
            End Property
            Public Property ID As Integer Implements iMDAttack.ID
                Get
                    Return Int(0, 4, 10)
                End Get
                Set(value As Integer)
                    Int(0, 4, 10) = value
                End Set
            End Property
            Public Property Ginseng As Integer Implements iMDAttack.Ginseng
                Get
                    Return Int(0, 14, 7)
                End Get
                Set(value As Integer)
                    Int(0, 14, 7) = value
                End Set
            End Property
            Function GetAttackDictionary() As IDictionary(Of Integer, String) Implements iMDAttack.GetAttackDictionary
                Return Lists.SkyMoves
            End Function
        End Class
        Public Class StoredPkm
            Inherits Binary
            Implements iMDPkm
            Implements iMDPkmGender
            Implements iMDPkmMetFloor
            Implements iMDPkmIQ
            Implements iPkmAttack
            Implements iSavable
            Implements iOnDisk
            Implements iOpenableFile
            Public Const Length As Integer = 362
            Public Const MimeType As String = "application/x-sky-pokemon"
            Public Event FileSaved As iSavable.FileSavedEventHandler Implements iSavable.FileSaved

            Public Sub New(Bits As Binary)
                MyBase.New(Bits)
            End Sub
            Public Sub New()
                MyBase.New(Length)
            End Sub
            Public ReadOnly Property IsValid As Boolean Implements iMDPkm.IsValid
                Get
                    Return Bits(0)
                End Get
            End Property
            Public Property Level As Byte Implements iMDPkm.Level
                Get
                    Return Int(0, 1, 7)
                End Get
                Set(value As Byte)
                    Int(0, 1, 7) = value
                End Set
            End Property
            Public Property ID As Integer Implements iMDPkm.ID
                Get
                    Dim i = Int(0, 8, 11)
                    If i > 600 Then
                        i -= 600
                    End If
                    Return i
                End Get
                Set(value As Integer)
                    Dim f = IsFemale
                    Int(0, 8, 11) = value
                    IsFemale = f
                End Set
            End Property
            Public Property IsFemale As Boolean Implements iMDPkmGender.IsFemale
                Get
                    Dim i = Int(0, 8, 11)
                    Return (i > 600)
                End Get
                Set(value As Boolean)
                    Dim i = Int(0, 8, 11)
                    If i > 600 Then
                        If value Then
                            'do nothing
                        Else
                            Int(0, 8, 11) -= 600
                        End If
                    Else
                        If value Then
                            Int(0, 8, 11) += 600
                        Else
                            'do nothing
                        End If
                    End If
                End Set
            End Property
            Public Property MetAt As Integer Implements iMDPkm.MetAt
                Get
                    Return Int(0, 19, 8)
                End Get
                Set(value As Integer)
                    Int(0, 19, 8) = value
                End Set
            End Property
            Public Property MetFloor As Integer Implements iMDPkmMetFloor.MetFloor
                Get
                    Return Int(0, 27, 7)
                End Get
                Set(value As Integer)
                    Int(0, 27, 7) = value
                End Set
            End Property
            'Unknown Data: Length of 15 bits
            Public Property IQ As Integer Implements iMDPkmIQ.IQ
                Get
                    Return Int(0, 49, 10)
                End Get
                Set(value As Integer)
                    Int(0, 49, 10) = value
                End Set
            End Property
            Public Property HP As Integer Implements iMDPkm.MaxHP
                Get
                    Return Int(0, 59, 10)
                End Get
                Set(value As Integer)
                    Int(0, 59, 10) = value
                End Set
            End Property
            Public Property StatAttack As Integer Implements iMDPkm.StatAttack
                Get
                    Return Int(0, 69, 8)
                End Get
                Set(value As Integer)
                    Int(0, 69, 8) = value
                End Set
            End Property
            Public Property StatDefense As Integer Implements iMDPkm.StatDefense
                Get
                    Return Int(0, 77, 8)
                End Get
                Set(value As Integer)
                    Int(0, 77, 8) = value
                End Set
            End Property
            Public Property StatSpAttack As Integer Implements iMDPkm.StatSpAttack
                Get
                    Return Int(0, 85, 8)
                End Get
                Set(value As Integer)
                    Int(0, 85, 8) = value
                End Set
            End Property
            Public Property StatSpDefense As Integer Implements iMDPkm.StatSpDefense
                Get
                    Return Int(0, 93, 8)
                End Get
                Set(value As Integer)
                    Int(0, 93, 8) = value
                End Set
            End Property
            Public Property Exp As Integer Implements iMDPkm.Exp
                Get
                    Return Int(0, 101, 24)
                End Get
                Set(value As Integer)
                    Int(0, 101, 24) = value
                End Set
            End Property
            'Unknown Data: Length of 73 bits
            Public Property Attack1 As iAttack Implements iPkmAttack.Attack1
                Get
                    Return New Attack(Range(198, Attack.Length))
                End Get
                Set(value As iAttack)
                    Range(198, Attack.Length) = value
                End Set
            End Property
            Public Property Attack2 As iAttack Implements iPkmAttack.Attack2
                Get
                    Return New Attack(Range(219, Attack.Length))
                End Get
                Set(value As iAttack)
                    Range(219, Attack.Length) = value
                End Set
            End Property
            Public Property Attack3 As iAttack Implements iPkmAttack.Attack3
                Get
                    Return New Attack(Range(240, Attack.Length))
                End Get
                Set(value As iAttack)
                    Range(240, Attack.Length) = value
                End Set
            End Property
            Public Property Attack4 As iAttack Implements iPkmAttack.Attack4
                Get
                    Return New Attack(Range(261, Attack.Length))
                End Get
                Set(value As iAttack)
                    Range(261, Attack.Length) = value
                End Set
            End Property
            Public Property Name As String Implements iMDPkm.Name
                Get
                    Return StringPMD(0, 282, 10)
                End Get
                Set(value As String)
                    StringPMD(0, 282, 10) = value
                End Set
            End Property

            Public Property Filename As String Implements iOnDisk.Filename

            Public Overrides Function ToString() As String
                If IsValid Then
                    Return String.Format("{0} (Lvl. {1} {2})", Name, Level, Lists.SkyPokemon(ID))
                Else
                    Return "----------"
                End If
            End Function
            Public Function GetPokemonDictionary() As IDictionary(Of Integer, String) Implements iMDPkm.GetPokemonDictionary
                Return Lists.SkyPokemon
            End Function

            Public Function GetMetAtDictionary() As IDictionary(Of Integer, String) Implements iMDPkm.GetMetAtDictionary
                Return Lists.SkyLocations
            End Function

            Public Function DefaultExtension() As String Implements iSavable.DefaultExtension
                Return ".skypkm"
            End Function

            Public Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
                Dim toOpen As New BinaryFile(Filename)
                Me.Bits = toOpen.Bits.Bits
                'matix2267's convention adds 6 bits to the beginning of a file so that the name will be byte-aligned
                For i = 1 To 8 - (Length Mod 8)
                    Me.Bits.RemoveAt(0)
                Next
                toOpen.Dispose()
            End Sub

            Public Sub Save() Implements iSavable.Save
                Save(Filename)
            End Sub

            Public Sub Save(Filename As String) Implements iSavable.Save
                Dim toSave As New BinaryFile()
                toSave.CreateFile(IO.Path.GetFileNameWithoutExtension(Filename))
                'matix2267's convention adds 6 bits to the beginning of a file so that the name will be byte-aligned
                For i = 1 To 8 - (Length Mod 8)
                    toSave.Bits.Bits.Add(0)
                Next
                toSave.Bits.Bits.AddRange(Me.Bits)
                toSave.Save(Filename)
                toSave.Dispose()
            End Sub
        End Class
        Public Property StoredPokemon(Index As Integer) As StoredPkm
            Get
                Return New StoredPkm(Bits.Range(Offsets.StoredPokemonOffset + Index * Offsets.StoredPokemonLength, Offsets.StoredPokemonLength))
            End Get
            Set(value As StoredPkm)
                Bits.Range(Offsets.StoredPokemonOffset + Index * Offsets.StoredPokemonLength, Offsets.StoredPokemonLength) = value
            End Set
        End Property
        Public Property StoredPokemon() As StoredPkm()
            Get
                Dim output As New List(Of StoredPkm)
                For count As Integer = 0 To Offsets.StoredPokemonNumber - 1
                    Dim i = StoredPokemon(count)
                    'If i.IsValid OrElse count < 9 Then 'Excepting when count < 9 because the first 8 pokemon slots are special
                    output.Add(i)
                    'End If
                Next
                Return output.ToArray
            End Get
            Set(value As StoredPkm())
                For count As Integer = 0 To Offsets.StoredPokemonNumber - 1
                    If value.Length > count Then
                        StoredPokemon(count) = value(count)
                    Else
                        StoredPokemon(count) = New StoredPkm(New Binary(Offsets.StoredPokemonLength))
                    End If
                Next
            End Set
        End Property
        Public Function GetPokemon() As iMDPkm() Implements iPokemonStorage.GetPokemon
            Return StoredPokemon
        End Function

        Public Function GetStoredPokemonOffsets() As StoredPokemonSlotDefinition() Implements iPokemonStorage.GetStoredPokemonOffsets
            Return StoredPokemonSlotDefinition.FromLines(IO.File.ReadAllText(PluginHelper.GetResourceName(SettingsManager.Instance.Settings.CurrentLanguage & "\SkyFriendAreaOffsets.txt"))).ToArray
        End Function

        Public Sub SetPokemon(Pokemon() As iMDPkm) Implements iPokemonStorage.SetPokemon
            StoredPokemon = Pokemon
        End Sub
    End Class

End Namespace