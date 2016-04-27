Imports SaveEditor.Interfaces

Namespace Saves
    Partial Class SkySave
        Implements Interfaces.iParty
        Public Class ActiveAttack
            Inherits Binary
            Implements iMDActiveAttack
            Public Const Length = 29
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
            Public Property IsLinked As Boolean Implements iMDActiveAttack.IsLinked
                Get
                    Return Bits(1)
                End Get
                Set(value As Boolean)
                    Bits(1) = value
                End Set
            End Property
            Public Property IsSwitched As Boolean Implements iMDActiveAttack.IsSwitched
                Get
                    Return Bits(2)
                End Get
                Set(value As Boolean)
                    Bits(2) = value
                End Set
            End Property
            Public Property IsSet As Boolean Implements iMDActiveAttack.IsSet
                Get
                    Return Bits(3)
                End Get
                Set(value As Boolean)
                    Bits(3) = value
                End Set
            End Property
            Public Property IsSealed As Boolean Implements iMDActiveAttack.IsSealed
                Get
                    Return Bits(4)
                End Get
                Set(value As Boolean)
                    Bits(4) = value
                End Set
            End Property
            Public Property ID As Integer Implements iAttack.ID
                Get
                    Return Int(0, 5, 10)
                End Get
                Set(value As Integer)
                    Int(0, 5, 10) = value
                End Set
            End Property
            Public Property PP As Integer Implements iMDActiveAttack.PP
                Get
                    Return Int(0, 15, 7)
                End Get
                Set(value As Integer)
                    Int(0, 15, 7) = value
                End Set
            End Property
            Public Property Ginseng As Integer Implements iMDActiveAttack.Ginseng
                Get
                    Return Int(0, 22, 7)
                End Get
                Set(value As Integer)
                    Int(0, 22, 7) = value
                End Set
            End Property
            Function GetAttackDictionary() As IDictionary(Of Integer, String) Implements iAttack.GetAttackDictionary
                Return Lists.GetSkyMoves
            End Function
        End Class
        Public Class ActivePkm
            Inherits Binary
            Implements Interfaces.iMDPkm
            Implements Interfaces.iPkmAttack
            Implements Interfaces.iMDPkmMetFloor
            Implements Interfaces.iMDPkmCurrentHP
            Implements Interfaces.iMDPkmGender
            Implements Interfaces.iMDPkmIQ
            Public Const Length As Integer = 0
            Public Sub New(Bits As Binary)
                MyBase.New(Bits)
            End Sub
            Public ReadOnly Property IsValid As Boolean Implements iMDPkm.IsValid
                Get
                    Return Bits(0)
                End Get
            End Property
            'Unknown data: 5 bits
            Public Property Level As Byte Implements iMDPkm.Level
                Get
                    Return Int(0, 5, 7)
                End Get
                Set(value As Byte)
                    Int(0, 5, 7) = value
                End Set
            End Property
            Public Property MetAt As Integer Implements iMDPkm.MetAt
                Get
                    Return Int(0, 12, 8)
                End Get
                Set(value As Integer)
                    Int(0, 12, 8) = value
                End Set
            End Property
            Public Property MetFloor As Integer Implements iMDPkmMetFloor.MetFloor
                Get
                    Return Int(0, 20, 7)
                End Get
                Set(value As Integer)
                    Int(0, 20, 7) = value
                End Set
            End Property
            'Unknown data: 1 bit
            Public Property IQ As Integer Implements iMDPkmIQ.IQ
                Get
                    Return Int(0, 28, 10)
                End Get
                Set(value As Integer)
                    Int(0, 28, 10) = value
                End Set
            End Property
            Public Property RosterNumber As UInt16
                Get
                    Return Int(0, 38, 10)
                End Get
                Set(value As UInt16)
                    Int(0, 38, 10) = value
                End Set
            End Property
            'Unknown Data: 22 bits
            Public Property ID As Integer Implements iMDPkm.ID
                Get
                    Dim i = Int(0, 70, 11)
                    If i > 600 Then
                        i -= 600
                    End If
                    Return i
                End Get
                Set(value As Integer)
                    Dim f = IsFemale
                    Int(0, 70, 11) = value
                    IsFemale = f
                End Set
            End Property
            Public Property IsFemale As Boolean Implements iMDPkmGender.IsFemale
                Get
                    Dim i = Int(0, 70, 11)
                    Return (i > 600)
                End Get
                Set(value As Boolean)
                    Dim i = Int(0, 70, 11)
                    If i > 600 Then
                        If value Then
                            'do nothing
                        Else
                            Int(0, 70, 11) -= 600
                        End If
                    Else
                        If value Then
                            Int(0, 70, 11) += 600
                        Else
                            'do nothing
                        End If
                    End If
                End Set
            End Property
            Public Property HP1 As Integer Implements iMDPkmCurrentHP.CurrentHP
                Get
                    Return Int(0, 81, 10)
                End Get
                Set(value As Integer)
                    Int(0, 81, 10) = value
                End Set
            End Property
            Public Property HP2 As Integer Implements iMDPkm.MaxHP
                Get
                    Return Int(0, 91, 10)
                End Get
                Set(value As Integer)
                    Int(0, 91, 10) = value
                End Set
            End Property
            Public Property StatAttack As Integer Implements iMDPkm.StatAttack
                Get
                    Return Int(0, 101, 8)
                End Get
                Set(value As Integer)
                    Int(0, 101, 8) = value
                End Set
            End Property
            Public Property StatDefense As Integer Implements iMDPkm.StatDefense
                Get
                    Return Int(0, 109, 8)
                End Get
                Set(value As Integer)
                    Int(0, 109, 8) = value
                End Set
            End Property
            Public Property StatSpAttack As Integer Implements iMDPkm.StatSpAttack
                Get
                    Return Int(0, 117, 8)
                End Get
                Set(value As Integer)
                    Int(0, 117, 8) = value
                End Set
            End Property
            Public Property StatSpDefense As Integer Implements iMDPkm.StatSpDefense
                Get
                    Return Int(0, 125, 8)
                End Get
                Set(value As Integer)
                    Int(0, 125, 8) = value
                End Set
            End Property
            Public Property Exp As Integer Implements iMDPkm.Exp
                Get
                    Return Int(0, 133, 24)
                End Get
                Set(value As Integer)
                    Int(0, 133, 24) = value
                End Set
            End Property
            Public Property Attack1 As Interfaces.iAttack Implements iPkmAttack.Attack1
                Get
                    Return New ActiveAttack(Range(157, ActiveAttack.Length))
                End Get
                Set(value As Interfaces.iAttack)
                    Range(157, ActiveAttack.Length) = value
                End Set
            End Property
            Public Property Attack2 As Interfaces.iAttack Implements iPkmAttack.Attack2
                Get
                    Return New ActiveAttack(Range(186, ActiveAttack.Length))
                End Get
                Set(value As Interfaces.iAttack)
                    Range(186, ActiveAttack.Length) = value
                End Set
            End Property
            Public Property Attack3 As Interfaces.iAttack Implements iPkmAttack.Attack3
                Get
                    Return New ActiveAttack(Range(215, ActiveAttack.Length))
                End Get
                Set(value As Interfaces.iAttack)
                    Range(215, ActiveAttack.Length) = value
                End Set
            End Property
            Public Property Attack4 As Interfaces.iAttack Implements iPkmAttack.Attack4
                Get
                    Return New ActiveAttack(Range(244, ActiveAttack.Length))
                End Get
                Set(value As Interfaces.iAttack)
                    Range(244, ActiveAttack.Length) = value
                End Set
            End Property
            'Unknown data: 193 bits
            Public Property Name As String Implements iMDPkm.Name
                Get
                    Return StringPMD(0, 466, 10)
                End Get
                Set(value As String)
                    StringPMD(0, 466, 10) = value
                End Set
            End Property
            Public Overrides Function ToString() As String
                If IsValid Then
                    Return String.Format("{0} (Lvl. {1} {2})", Name, Level, Lists.GetSkyPokemon(ID))
                Else
                    Return "----------"
                End If
            End Function

            Public Function GetMetAtDictionary() As IDictionary(Of Integer, String) Implements iMDPkm.GetMetAtDictionary
                Return Lists.GetSkyLocations
            End Function

            Public Function GetPokemonDictionary() As IDictionary(Of Integer, String) Implements iMDPkm.GetPokemonDictionary
                Return Lists.GetSkyPokemon
            End Function
        End Class
        Public Property ActivePokemon(Index As Integer) As ActivePkm
            Get
                Return New ActivePkm(Me.Bits.Range(Offsets.ActivePokemonOffset + Index * Offsets.ActivePokemonLength, Offsets.ActivePokemonLength))
            End Get
            Set(value As ActivePkm)
                Me.Bits.Range(Offsets.ActivePokemonOffset + Index * Offsets.ActivePokemonLength, Offsets.ActivePokemonLength) = value
            End Set
        End Property
        Public Property ActivePokemon() As iMDPkm()
            Get
                Dim output As New List(Of ActivePkm)
                For count As Integer = 0 To Offsets.ActivePokemonNumber - 1
                    Dim i = ActivePokemon(count)
                    If i.IsValid OrElse count < 9 Then 'Excepting when count < 9 because the first 8 pokemon slots are special
                        output.Add(i)
                    End If
                Next
                Return output.ToArray
            End Get
            Set(value As iMDPkm())
                For count As Integer = 0 To Offsets.ActivePokemonNumber - 1
                    If value.Length > count Then
                        ActivePokemon(count) = value(count)
                    Else
                        ActivePokemon(count) = New ActivePkm(New Binary(Offsets.ActivePokemonLength))
                    End If
                Next
            End Set
        End Property

        Public Property SpEpisodeActivePokemon(Index As Integer) As ActivePkm
            Get
                Return New ActivePkm(Me.Bits.Range(Offsets.SpActivePokemonOffset + Index * Offsets.ActivePokemonLength, Offsets.ActivePokemonLength))
            End Get
            Set(value As ActivePkm)
                Me.Bits.Range(Offsets.SpActivePokemonOffset + Index * Offsets.ActivePokemonLength, Offsets.ActivePokemonLength) = value
            End Set
        End Property
        Public Property SpEpisodeActivePokemon() As iMDPkm()
            Get
                Dim output As New List(Of ActivePkm)
                For count As Integer = 0 To Offsets.ActivePokemonNumber - 1
                    Dim i = SpEpisodeActivePokemon(count)
                    If i.IsValid OrElse count < 9 Then 'Excepting when count < 9 because the first 8 pokemon slots are special
                        output.Add(i)
                    End If
                Next
                Return output.ToArray
            End Get
            Set(value As iMDPkm())
                For count As Integer = 0 To Offsets.ActivePokemonNumber - 1
                    If value.Length > count Then
                        SpEpisodeActivePokemon(count) = value(count)
                    Else
                        SpEpisodeActivePokemon(count) = New ActivePkm(New Binary(Offsets.ActivePokemonLength))
                    End If
                Next
            End Set
        End Property

        Public Function GetActivePokemon() As Interfaces.iMDPkm() Implements Interfaces.iParty.GetPokemon
            Return ActivePokemon
        End Function

        Public Sub SetActivePokemon(Pokemon() As Interfaces.iMDPkm) Implements Interfaces.iParty.SetPokemon
            ActivePokemon = Pokemon
        End Sub

    End Class

End Namespace