Namespace MysteryDungeon.Explorers
    Partial Class TDSave
        Public Class ActiveAttack
            Inherits Binary
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
            Public Property IsSealed As Boolean
                Get
                    Return Bits(4)
                End Get
                Set(value As Boolean)
                    Bits(4) = value
                End Set
            End Property
            Public Property ID As UInt16
                Get
                    Return Int(0, 5, 10)
                End Get
                Set(value As UInt16)
                    Int(0, 5, 10) = value
                End Set
            End Property
            Public Property PP As UInt16
                Get
                    Return Int(0, 15, 7)
                End Get
                Set(value As UInt16)
                    Int(0, 15, 7) = value
                End Set
            End Property
            Public Property Ginseng As Integer
                Get
                    Return Int(0, 22, 7)
                End Get
                Set(value As Integer)
                    Int(0, 22, 7) = value
                End Set
            End Property
        End Class
        Public Class ActivePkm
            Inherits Binary
            Public Const Length As Integer = 0
            Public Sub New(Bits As Binary)
                MyBase.New(Bits)
            End Sub
            Public ReadOnly Property IsValid As Boolean
                Get
                    Return ID > 0
                End Get
            End Property
            'Unknown data: 5 bits
            Public Property Level As Byte
                Get
                    Return Int(0, 5, 7)
                End Get
                Set(value As Byte)
                    Int(0, 5, 7) = value
                End Set
            End Property
            Public Property MetAt As Byte
                Get
                    Return Int(0, 12, 8)
                End Get
                Set(value As Byte)
                    Int(0, 12, 8) = value
                End Set
            End Property
            Public Property MetFloor As Byte
                Get
                    Return Int(0, 20, 7)
                End Get
                Set(value As Byte)
                    Int(0, 20, 7) = value
                End Set
            End Property
            'Unknown data: 1 bit
            Public Property IQ As UInt16
                Get
                    Return Int(0, 27, 10)
                End Get
                Set(value As UInt16)
                    Int(0, 27, 10) = value
                End Set
            End Property
            Public Property RosterNumber As UInt16
                Get
                    Return Int(0, 37, 10)
                End Get
                Set(value As UInt16)
                    Int(0, 37, 10) = value
                End Set
            End Property
            'Unknown Data: 22 bits
            Public Property ID As Integer
                Get
                    Dim i = Int(0, 69, 11)
                    If i > 600 Then
                        i -= 600
                    End If
                    Return i
                End Get
                Set(value As Integer)
                    Dim f = IsFemale
                    Int(0, 69, 11) = value
                    IsFemale = f
                End Set
            End Property
            Public Property IsFemale As Boolean
                Get
                    Dim i = Int(0, 69, 11)
                    Return (i > 600)
                End Get
                Set(value As Boolean)
                    Dim i = Int(0, 69, 11)
                    If i > 600 Then
                        If value Then
                            'do nothing
                        Else
                            Int(0, 69, 11) -= 600
                        End If
                    Else
                        If value Then
                            Int(0, 69, 11) += 600
                        Else
                            'do nothing
                        End If
                    End If
                End Set
            End Property
            Public Property HP1 As UInt16
                Get
                    Return Int(0, 80, 10)
                End Get
                Set(value As UInt16)
                    Int(0, 80, 10) = value
                End Set
            End Property
            Public Property HP2 As UInt16
                Get
                    Return Int(0, 90, 10)
                End Get
                Set(value As UInt16)
                    Int(0, 90, 10) = value
                End Set
            End Property
            Public Property StatAttack As Byte
                Get
                    Return Int(0, 100, 8)
                End Get
                Set(value As Byte)
                    Int(0, 100, 8) = value
                End Set
            End Property
            Public Property StatDefense As Byte
                Get
                    Return Int(0, 108, 8)
                End Get
                Set(value As Byte)
                    Int(0, 108, 8) = value
                End Set
            End Property
            Public Property StatSpAttack As Byte
                Get
                    Return Int(0, 116, 8)
                End Get
                Set(value As Byte)
                    Int(0, 116, 8) = value
                End Set
            End Property
            Public Property StatSpDefense As Byte
                Get
                    Return Int(0, 124, 8)
                End Get
                Set(value As Byte)
                    Int(0, 124, 8) = value
                End Set
            End Property
            Public Property Exp As Integer
                Get
                    Return Int(0, 132, 24)
                End Get
                Set(value As Integer)
                    Int(0, 132, 24) = value
                End Set
            End Property
            'Unknown Data: Length of 105 bits
            Public Property Attack1 As ActiveAttack
                Get
                    Return New ActiveAttack(Range(261, ActiveAttack.Length))
                End Get
                Set(value As ActiveAttack)
                    Range(261, ActiveAttack.Length) = value
                End Set
            End Property
            Public Property Attack2 As ActiveAttack
                Get
                    Return New ActiveAttack(Range(290, ActiveAttack.Length))
                End Get
                Set(value As ActiveAttack)
                    Range(290, ActiveAttack.Length) = value
                End Set
            End Property
            Public Property Attack3 As ActiveAttack
                Get
                    Return New ActiveAttack(Range(311, ActiveAttack.Length))
                End Get
                Set(value As ActiveAttack)
                    Range(311, ActiveAttack.Length) = value
                End Set
            End Property
            Public Property Attack4 As ActiveAttack
                Get
                    Return New ActiveAttack(Range(332, ActiveAttack.Length))
                End Get
                Set(value As ActiveAttack)
                    Range(332, ActiveAttack.Length) = value
                End Set
            End Property
            'Unknown data: 191 bits
            Public Property Name As String
                Get
                    Return StringPMD(0, 343, 10)
                End Get
                Set(value As String)
                    StringPMD(0, 343, 10) = value
                End Set
            End Property
            Public Overrides Function ToString() As String
                If IsValid Then
                    Return String.Format("{0} (Lvl. {1} {2})", Name, Level, Lists.ExplorersPokemon(ID))
                Else
                    Return "----------"
                End If
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
        Public Property ActivePokemon() As ActivePkm()
            Get
                Dim output As New List(Of ActivePkm)
                For count As Integer = 0 To Offsets.ActivePokemonNumber - 1
                    Dim i = ActivePokemon(count)
                    If i.IsValid OrElse count < 5 Then 'Excepting when count < 5 because the first 4 pokemon slots are special
                        output.Add(i)
                    End If
                Next
                Return output.ToArray
            End Get
            Set(value As ActivePkm())
                For count As Integer = 0 To Offsets.ActivePokemonNumber - 1
                    If value.Length > count Then
                        ActivePokemon(count) = value(count)
                    Else
                        ActivePokemon(count) = New ActivePkm(New Binary(Offsets.ActivePokemonLength))
                    End If
                Next
            End Set
        End Property
    End Class
End Namespace