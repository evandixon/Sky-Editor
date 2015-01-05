Imports System
Imports SkyEditor.skyjed.buffer
Imports SkyEditor.skyjed.buffer.BooleanBuffer
Namespace skyjed.save
    Public Interface iPkmnQ
        Property no1 As Integer ' TODO don't know what is the difference
        Property no2 As Integer
        Property isfemale1 As Boolean
        Property isfemale2 As Boolean
        Property lvl As Integer
        Property hp1 As Integer ' current hp
        Property hp2 As Integer ' base hp (without bonuses)
        Property hp_boost As Integer ' from IQ / items
        ''' <summary>
        ''' A Pokemon's stats stored in the corresponding index.
        ''' 0: Attack
        ''' 1: Sp. Attack
        ''' 2: Defense
        ''' 3: Sp. Defense
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Property stats As Integer() ' atk spa def spd
        Property exp As Integer
        Property attacks As SkyAttackQ()
    End Interface
    Public Class SkyPkmnQ
        Implements ipkmnq
        Public Const BYTE_LENGTH As Integer = 429
        Public Const MIME_TYPE As String = "application/x-sky-pokemon-q"

        Public Overridable ReadOnly Property ByteLength As Integer
            Get
                Return BYTE_LENGTH
            End Get
        End Property
        Public Overridable ReadOnly Property Length As Integer
            Get
                Return 8 * BYTE_LENGTH
            End Get
        End Property
        Public Overridable ReadOnly Property MimeType As String
            Get
                Return MIME_TYPE
            End Get
        End Property

        Private Const FEMALE_ADD As Integer = 600

        Private Const NUM_STATS As Integer = 4
        Private Const NUM_ATTACKS As Integer = 4

        Private Const BLEN_ATTACK_Q As Integer = 6

        Public Property no1 As Integer Implements iPkmnQ.no1 ' TODO don't know what is the difference
        Public Property no2 As Integer Implements iPkmnQ.no2
        Public Property isfemale1 As Boolean Implements iPkmnQ.isfemale1
        Public Property isfemale2 As Boolean Implements iPkmnQ.isfemale2
        Public Property lvl As Integer Implements iPkmnQ.lvl
        Public Property CurrentHP As Integer Implements iPkmnQ.hp1 ' current hp
        Public Property BaseHPNoBonus As Integer Implements iPkmnQ.hp2 ' base hp (without bonuses)
        Public Property hp_boost As Integer Implements iPkmnQ.hp_boost ' from IQ / items
        Public Property stats As Integer() Implements iPkmnQ.stats ' atk spa def spd
        Public Property exp As Integer Implements iPkmnQ.exp
        Public Property attacks As SkyAttackQ() Implements iPkmnQ.attacks

        Public Sub New()
            stats = New Integer(NUM_STATS - 1) {}
            attacks = New SkyAttackQ(NUM_ATTACKS - 1) {}
            For i As Integer = 0 To NUM_ATTACKS - 1
                attacks(i) = New SkyAttackQ()
            Next i
        End Sub

        Public Sub New(ByVal buf As BooleanBuffer)
            Me.New()
            load(buf)
        End Sub

        Public Overridable Sub load(ByVal buf As BooleanBuffer)
            buf.seek(10 * 8)
            If True Then
                Dim mfno As Integer = buf.getInt(16)
                no1 = mfno Mod FEMALE_ADD
                isfemale1 = mfno >= FEMALE_ADD
            End If
            If True Then
                Dim mfno As Integer = buf.getInt(16)
                no2 = mfno Mod FEMALE_ADD
                isfemale2 = mfno >= FEMALE_ADD
            End If
            buf.seek(18 * 8)
            lvl = buf.getInt(8)
            buf.seek(24 * 8)
            CurrentHP = buf.getInt(16)
            BaseHPNoBonus = buf.getInt(16)
            hp_boost = buf.getInt(16)
            buf.seek(32 * 8)
            stats = buf.getInts(NUM_STATS, 8)
            exp = buf.getInt(32)
            buf.seek(337 * 8)
            For i As Integer = 0 To NUM_ATTACKS - 1
                attacks(i).load(buf.view(8 * BLEN_ATTACK_Q))
            Next i
        End Sub

        Public Overridable Sub store(ByVal buf As BooleanBuffer)
            buf.seek(10 * 8)
            If True Then
                Dim mfno As Integer = no1 + (If(isfemale1, FEMALE_ADD, 0))
                buf.putInt(mfno, 16)
            End If
            If True Then
                Dim mfno As Integer = no2 + (If(isfemale2, FEMALE_ADD, 0))
                buf.putInt(mfno, 16)
            End If
            buf.seek(18 * 8)
            buf.putInt(lvl, 8)
            buf.seek(24 * 8)
            buf.putInt(CurrentHP, 16)
            buf.putInt(BaseHPNoBonus, 16)
            buf.putInt(hp_boost, 16)
            buf.seek(32 * 8)
            buf.putInts(stats, 8)
            buf.putInt(exp, 32)
            buf.seek(337 * 8)
            For i As Integer = 0 To NUM_ATTACKS - 1
                attacks(i).store(buf.view(8 * BLEN_ATTACK_Q))
            Next i
        End Sub

        Public Overrides Function ToString() As String
            If no1 = 0 Then
                Return "----------"
            End If
            Return String.Format("(Lvl. {0} {1})", lvl, Lists.SkyPokemon(no1))
        End Function

        'Public Overridable Sub dump()
        '	Console.Write(String.Format("{0} lvl{1,3:D} | hp: {2:D}/{3:D}+{4:D} | stats: {5:D} {6:D} {7:D} {8:D} | ", SkyPokemonRes.Instance.getValue(no1), lvl, hp1, hp2, hp_boost, stats(0), stats(1), stats(2), stats(3)))
        '	Console.WriteLine(String.Format("id: {0:D}/{1:D} | exp: {2:D}", no1, no2, exp))
        'End Sub

    End Class

End Namespace