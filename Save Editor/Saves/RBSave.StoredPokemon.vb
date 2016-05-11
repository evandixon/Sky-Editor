Imports SaveEditor.Interfaces
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace Saves
    Partial Class RBSave
        Implements iPokemonStorage
        Public Class Attack
            Inherits Binary
            Implements iAttack
            Public Const Length = 20
            Public Sub New(Bits As Binary)
                MyBase.New(Bits)
            End Sub
            Public Property IsValid As Boolean
                Get
                    Return Bits(0)
                End Get
                Set(value As Boolean)
                    Bits(0) = value
                End Set
            End Property
            Public Property IsLinked As Boolean
                Get
                    Return Bits(1)
                End Get
                Set(value As Boolean)
                    Bits(1) = value
                End Set
            End Property
            Public Property IsSwitched As Boolean
                Get
                    Return Bits(2)
                End Get
                Set(value As Boolean)
                    Bits(2) = value
                End Set
            End Property
            Public Property IsSet As Boolean
                Get
                    Return Bits(3)
                End Get
                Set(value As Boolean)
                    Bits(3) = value
                End Set
            End Property
            Public Property ID As Integer Implements iAttack.ID
                Get
                    Return Int(0, 4, 10)
                End Get
                Set(value As Integer)
                    Int(0, 4, 10) = value
                End Set
            End Property
            Public Property Ginseng As Integer
                Get
                    Return Int(0, 14, 7)
                End Get
                Set(value As Integer)
                    Int(0, 14, 7) = value
                End Set
            End Property
            Public Function GetAttackDictionary() As IDictionary(Of Integer, String) Implements iAttack.GetAttackDictionary
                Return Lists.GetRBMoves
            End Function
        End Class
        Public Class StoredPkm
            Inherits Binary
            Implements iMDPkm
            Implements iPkmAttack
            Implements ISavableAs
            Implements iOnDisk
            Implements IOpenableFile
            Public Const Length As Integer = 323
            Public Const MimeType As String = "application/x-rb-pokemon"
            Public Event FileSaved As iSavable.FileSavedEventHandler Implements iSavable.FileSaved

            Public Sub New(Bits As Binary)
                MyBase.New(Bits)
            End Sub
            Public ReadOnly Property IsValid As Boolean Implements iMDPkm.IsValid
                Get
                    Return ID > 0 'Bits(0)
                End Get
            End Property
            Public Property Level As Byte Implements iMDPkm.Level
                Get
                    Return Int(0, 0, 7)
                End Get
                Set(value As Byte)
                    Int(0, 0, 7) = value
                End Set
            End Property
            Public Property ID As Integer Implements iMDPkm.ID
                Get
                    Return Int(0, 7, 9)
                End Get
                Set(value As Integer)
                    Int(0, 7, 9) = value
                End Set
            End Property
            Public Property MetAt As Integer Implements iMDPkm.MetAt
                Get
                    Return Int(0, 16, 7)
                End Get
                Set(value As Integer)
                    Int(0, 16, 7) = value
                End Set
            End Property
            'Unknown Data: Length of 21 bits
            Public Property IQ As UInt16
                Get
                    Return Int(0, 44, 10)
                End Get
                Set(value As UInt16)
                    Int(0, 44, 10) = value
                End Set
            End Property
            Public Property HP As Integer Implements iMDPkm.MaxHP
                Get
                    Return Int(0, 54, 10)
                End Get
                Set(value As Integer)
                    Int(0, 54, 10) = value
                End Set
            End Property
            Public Property StatAttack As Integer Implements iMDPkm.StatAttack
                Get
                    Return Int(0, 64, 8)
                End Get
                Set(value As Integer)
                    Int(0, 64, 8) = value
                End Set
            End Property
            Public Property StatDefense As Integer Implements iMDPkm.StatDefense
                Get
                    Return Int(0, 72, 8)
                End Get
                Set(value As Integer)
                    Int(0, 72, 8) = value
                End Set
            End Property
            Public Property StatSpAttack As Integer Implements iMDPkm.StatSpAttack
                Get
                    Return Int(0, 80, 8)
                End Get
                Set(value As Integer)
                    Int(0, 80, 8) = value
                End Set
            End Property
            Public Property StatSpDefense As Integer Implements iMDPkm.StatSpDefense
                Get
                    Return Int(0, 88, 8)
                End Get
                Set(value As Integer)
                    Int(0, 88, 8) = value
                End Set
            End Property
            Public Property Exp As Integer Implements iMDPkm.Exp
                Get
                    Return Int(0, 96, 24)
                End Get
                Set(value As Integer)
                    Int(0, 96, 24) = value
                End Set
            End Property
            'Unknown Data: Length of 43 bits
            Public Property Attack1 As iAttack Implements iPkmAttack.Attack1
                Get
                    Return New Attack(Range(163, Attack.Length))
                End Get
                Set(value As iAttack)
                    Range(163, Attack.Length) = value
                End Set
            End Property
            Public Property Attack2 As iAttack Implements iPkmAttack.Attack2
                Get
                    Return New Attack(Range(183, Attack.Length))
                End Get
                Set(value As iAttack)
                    Range(183, Attack.Length) = value
                End Set
            End Property
            Public Property Attack3 As iAttack Implements iPkmAttack.Attack3
                Get
                    Return New Attack(Range(203, Attack.Length))
                End Get
                Set(value As iAttack)
                    Range(203, Attack.Length) = value
                End Set
            End Property
            Public Property Attack4 As iAttack Implements iPkmAttack.Attack4
                Get
                    Return New Attack(Range(223, Attack.Length))
                End Get
                Set(value As iAttack)
                    Range(223, Attack.Length) = value
                End Set
            End Property
            Public Property Name As String Implements iMDPkm.Name
                Get
                    Return StringPMD(0, 243, 10)
                End Get
                Set(value As String)
                    StringPMD(0, 243, 10) = value
                End Set
            End Property
            Public Overrides Function ToString() As String
                If IsValid Then
                    Return String.Format("{0} (Lvl. {1} {2})", Name, Level, Lists.GetRBPokemon(ID))
                Else
                    Return "----------"
                End If
            End Function
            Public Function GetPokemonDictionary() As IDictionary(Of Integer, String) Implements iMDPkm.GetPokemonDictionary
                Return Lists.GetRBPokemon
            End Function

            Public Function GetMetAtDictionary() As IDictionary(Of Integer, String) Implements iMDPkm.GetMetAtDictionary
                Return Lists.GetRBLocations
            End Function

            Public Property Filename As String Implements iOnDisk.Filename

            Public Function GetDefaultExtension() As String Implements ISavableAs.GetDefaultExtension
                Return ".rbpkm"
            End Function

            Public Async Function OpenFile(Filename As String, Provider As IOProvider) As Task Implements IOpenableFile.OpenFile
                Dim toOpen As New BinaryFile
                Await toOpen.OpenFile(Filename, Provider)
                Me.Bits = toOpen.Bits.Bits
                For i = 1 To 8 - (Length Mod 8)
                    Me.Bits.RemoveAt(Me.Bits.Count - 1)
                Next
            End Function

            Public Sub Save() Implements iSavable.Save
                Save(Filename)
            End Sub

            Public Sub Save(Filename As String) Implements ISavableAs.Save
                Dim toSave As New BinaryFile()
                toSave.CreateFile(IO.Path.GetFileNameWithoutExtension(Filename))
                toSave.Bits.Bits.AddRange(Me.Bits)
                For i = 1 To 8 - (Length Mod 8)
                    toSave.Bits.Bits.Add(0)
                Next
                toSave.Save(Filename)
            End Sub
        End Class
        Public Property StoredPokemon(Index As Integer) As StoredPkm
            Get
                Return New StoredPkm(Me.Bits.Range(Offsets.StoredPokemonOffset + Index * Offsets.StoredPokemonLength, Offsets.StoredPokemonLength))
            End Get
            Set(value As StoredPkm)
                Me.Bits.Range(Offsets.StoredPokemonOffset + Index * Offsets.StoredPokemonLength, Offsets.StoredPokemonLength) = value
            End Set
        End Property
        Public Property StoredPokemon() As StoredPkm()
            Get
                Dim output As New List(Of StoredPkm)
                For count As Integer = 0 To Offsets.StoredPokemonNumber - 1
                    Dim i = StoredPokemon(count)
                    'If i.IsValid OrElse count < 5 Then 'Excepting when count < 5 because the first 4 pokemon slots are special
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

        Public Sub SetPokemon(Pokemon() As iMDPkm) Implements iPokemonStorage.SetPokemon
            StoredPokemon = Pokemon
        End Sub

        Public Function GetStoredPokemonOffsets() As StoredPokemonSlotDefinition() Implements iPokemonStorage.GetStoredPokemonOffsets
            Return StoredPokemonSlotDefinition.FromLines(My.Resources.ListResources.RBFriendAreaOffsets).ToArray
        End Function
    End Class

End Namespace