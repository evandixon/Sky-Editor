Imports SkyEditor.SaveEditor.Interfaces

Namespace Saves
    Partial Class SkySave
        Public Class QuicksaveAttack
            Inherits Binary
            Implements iMDActiveAttack
            Public Const Length = 48
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
            'Unknown flags: 3 bits
            'Unknown data: 8 bits
            Public Property ID As Integer Implements iMDActiveAttack.ID
                Get
                    Return Int(0, 16, 16)
                End Get
                Set(value As Integer)
                    Int(0, 16, 16) = value
                End Set
            End Property
            Public Property PP As Integer Implements iMDActiveAttack.PP
                Get
                    Return Int(0, 32, 8)
                End Get
                Set(value As Integer)
                    Int(0, 32, 8) = value
                End Set
            End Property
            Public Property Ginseng As Integer Implements iMDActiveAttack.Ginseng
                Get
                    Return Int(0, 40, 8)
                End Get
                Set(value As Integer)
                    Int(0, 40, 8) = value
                End Set
            End Property

            Public Function GetAttackDictionary() As IDictionary(Of Integer, String) Implements iAttack.GetAttackDictionary
                Return Lists.GetSkyMoves
            End Function
        End Class
        Public Class QuicksavePkm
            Inherits Binary
            Implements iPkmAttack
            Public Const Length As Integer = 429 * 8
            Public Sub New(Bits As Binary)
                MyBase.New(Bits)
            End Sub
            Public Sub New()
                MyBase.New(New Binary(Length))
            End Sub
            Public ReadOnly Property IsValid As Boolean
                Get
                    Return ID > 0
                End Get
            End Property
            'Unknown data: 80 bits
            Public Property TransformedID As Integer
                Get
                    Dim i = Int(0, 80, 16)
                    If i > 600 Then
                        i -= 600
                    End If
                    Return i
                End Get
                Set(value As Integer)
                    Dim f = IsFemale
                    Int(0, 80, 16) = value
                    IsFemale = f
                End Set
            End Property
            Public Property TransformedIsFemale As Boolean
                Get
                    Dim i = Int(0, 80, 16)
                    Return (i > 600)
                End Get
                Set(value As Boolean)
                    Dim i = Int(0, 80, 16)
                    If i > 600 Then
                        If value Then
                            'do nothing
                        Else
                            Int(0, 80, 16) -= 600
                        End If
                    Else
                        If value Then
                            Int(0, 80, 16) += 600
                        Else
                            'do nothing
                        End If
                    End If
                End Set
            End Property
            Public Property ID As Integer
                Get
                    Dim i = Int(0, 96, 16)
                    If i > 600 Then
                        i -= 600
                    End If
                    Return i
                End Get
                Set(value As Integer)
                    Dim f = IsFemale
                    Int(0, 96, 16) = value
                    IsFemale = f
                End Set
            End Property
            Public Property IsFemale As Boolean
                Get
                    Dim i = Int(0, 96, 16)
                    Return (i > 600)
                End Get
                Set(value As Boolean)
                    Dim i = Int(0, 96, 16)
                    If i > 600 Then
                        If value Then
                            'do nothing
                        Else
                            Int(0, 96, 16) -= 600
                        End If
                    Else
                        If value Then
                            Int(0, 96, 16) += 600
                        Else
                            'do nothing
                        End If
                    End If
                End Set
            End Property
            'Unknown data
            Public Property Level As Byte
                Get
                    Return Int(0, 144, 8)
                End Get
                Set(value As Byte)
                    Int(0, 144, 8) = value
                End Set
            End Property
            'Unknown data
            Public Property CurrentHP As UInt16
                Get
                    Return Int(0, 192, 16)
                End Get
                Set(value As UInt16)
                    Int(0, 192, 16) = value
                End Set
            End Property
            Public Property BaseHP As UInt16
                Get
                    Return Int(0, 208, 16)
                End Get
                Set(value As UInt16)
                    Int(0, 208, 16) = value
                End Set
            End Property
            Public Property HPBoost As UInt16
                Get
                    Return Int(0, 224, 16)
                End Get
                Set(value As UInt16)
                    Int(0, 224, 16) = value
                End Set
            End Property
            'Unknown Data:
            Public Property StatAttack As Byte
                Get
                    Return Int(0, 256, 8)
                End Get
                Set(value As Byte)
                    Int(0, 256, 8) = value
                End Set
            End Property
            Public Property StatDefense As Byte
                Get
                    Return Int(0, 264, 8)
                End Get
                Set(value As Byte)
                    Int(0, 264, 8) = value
                End Set
            End Property
            Public Property StatSpAttack As Byte
                Get
                    Return Int(0, 272, 8)
                End Get
                Set(value As Byte)
                    Int(0, 272, 8) = value
                End Set
            End Property
            Public Property StatSpDefense As Byte
                Get
                    Return Int(0, 280, 8)
                End Get
                Set(value As Byte)
                    Int(0, 280, 8) = value
                End Set
            End Property
            Public Property Exp As Integer
                Get
                    Return Int(0, 288, 32)
                End Get
                Set(value As Integer)
                    Int(0, 288, 32) = value
                End Set
            End Property
            'Unknown Data
            Public Property Attack1 As iAttack Implements iPkmAttack.Attack1
                Get
                    Return New QuicksaveAttack(Range(2696, QuicksaveAttack.Length))
                End Get
                Set(value As iAttack)
                    Range(2696, QuicksaveAttack.Length) = value
                End Set
            End Property
            Public Property Attack2 As iAttack Implements iPkmAttack.Attack2
                Get
                    Return New QuicksaveAttack(Range(2696 + 1 * QuicksaveAttack.Length, QuicksaveAttack.Length))
                End Get
                Set(value As iAttack)
                    Range(2696 + 1 * QuicksaveAttack.Length, QuicksaveAttack.Length) = value
                End Set
            End Property
            Public Property Attack3 As iAttack Implements iPkmAttack.Attack3
                Get
                    Return New QuicksaveAttack(Range(2696 + 2 * QuicksaveAttack.Length, QuicksaveAttack.Length))
                End Get
                Set(value As iAttack)
                    Range(2696 + 2 * QuicksaveAttack.Length, QuicksaveAttack.Length) = value
                End Set
            End Property
            Public Property Attack4 As iAttack Implements iPkmAttack.Attack4
                Get
                    Return New QuicksaveAttack(Range(2696 + 3 * QuicksaveAttack.Length, QuicksaveAttack.Length))
                End Get
                Set(value As iAttack)
                    Range(2696 + 3 * QuicksaveAttack.Length, QuicksaveAttack.Length) = value
                End Set
            End Property
            Public Overrides Function ToString() As String
                If IsValid Then
                    Return String.Format("Lvl. {0} {1}", Level, Lists.GetSkyPokemon(ID))
                Else
                    Return "----------"
                End If
            End Function
        End Class

        Public Property QuicksavePokemon(Index As Integer) As QuicksavePkm
            Get
                Return New QuicksavePkm(Bits.Range(Offsets.QuicksavePokemonOffset + Offsets.QuicksavePokemonLength * Index, Offsets.QuicksavePokemonLength))
            End Get
            Set(value As QuicksavePkm)
                Bits.Range(Offsets.QuicksavePokemonOffset + Offsets.QuicksavePokemonLength * Index, Offsets.QuicksavePokemonLength) = value
            End Set
        End Property

        Public Property QuicksavePokemon As QuicksavePkm()
            Get
                Dim out As New List(Of QuicksavePkm)
                For count = 0 To Offsets.QuicksavePokemonNumber - 1
                    out.Add(QuicksavePokemon(count))
                Next
                Return out.ToArray
            End Get
            Set(value As QuicksavePkm())
                For count = 0 To Offsets.QuicksavePokemonNumber - 1
                    If value.Length > count Then
                        QuicksavePokemon(count) = value(count)
                    Else
                        QuicksavePokemon(count) = New QuicksavePkm()
                    End If
                Next
            End Set
        End Property

    End Class

End Namespace