Imports SkyEditor.skyjed.buffer
Imports SkyEditor.skyjed.util

Namespace skyjed.save
    Public Interface iPkmnEx
        Property unksp1() As Boolean()
        Property lvl As Integer
        Property metat As Integer
        Property metfl As Integer
        Property unksp2() As Boolean()
        Property iq As Integer
        Property roster_no As Integer
        Property unksp3() As Boolean()
        Property no As Integer
        Property isfemale As Boolean
        Property hp1 As Integer
        Property hp2 As Integer
        Property stats() As Integer() ' atk spa def spd
        Property exp As Integer
        Property attacks() As SkyAttackEx()
        Property unksp4() As Boolean()
        Property unkdata2() As Boolean()
        Property unksp5() As Boolean()
        Property name As String
        Function Tostring() As String
    End Interface
    Public Class SkyPkmnEx
        Implements iPkmnEx
        Private Const FEMALE_ADD As Integer = 600

        Private Const NUM_STATS As Integer = 4
        Private Const NUM_ATTACKS As Integer = 4

        Private Const LEN_UNKS1 As Integer = 5
        Private Const LEN_LVL As Integer = 7
        Private Const LEN_METAT As Integer = 8
        Private Const LEN_METFL As Integer = 7
        Private Const LEN_UNKS2 As Integer = 1
        Private Const LEN_IQ As Integer = 10
        Private Const LEN_ROSTER_NO As Integer = 10
        Private Const LEN_UNKS3 As Integer = 22
        Private Const LEN_NO As Integer = 11
        Private Const LEN_HP1 As Integer = 10
        Private Const LEN_HP2 As Integer = 10
        Private Const LEN_STAT As Integer = 8
        Private Const LEN_EXP As Integer = 24
        Private Const LEN_ATTACK_EX As Integer = 29
        Private Const LEN_UNKS4 As Integer = 105
        Private Const LEN_UNK2 As Integer = 73
        Private Const LEN_UNKS5 As Integer = 15
        Private Const BLEN_NAME As Integer = 10

        Public Property unksp1() As Boolean() Implements iPkmnEx.unksp1
        Public Property lvl As Integer Implements iPkmnEx.lvl
        Public Property metat As Integer Implements iPkmnEx.metat
        Public Property metfl As Integer Implements iPkmnEx.metfl
        Public Property unksp2() As Boolean() Implements iPkmnEx.unksp2
        Public Property iq As Integer Implements iPkmnEx.iq
        Public Property roster_no As Integer Implements iPkmnEx.roster_no
        Public Property unksp3() As Boolean() Implements iPkmnEx.unksp3
        Public Property no As Integer Implements iPkmnEx.no
        Public Property isfemale As Boolean Implements iPkmnEx.isfemale
        Public Property hp1 As Integer Implements iPkmnEx.hp1
        Public Property hp2 As Integer Implements iPkmnEx.hp2
        Public Property stats() As Integer() Implements iPkmnEx.stats ' atk spa def spd
        Public Property exp As Integer Implements iPkmnEx.exp
        Public Property attacks() As SkyAttackEx() Implements iPkmnEx.attacks
        Public Property unksp4() As Boolean() Implements iPkmnEx.unksp4
        Public Property unkdata2() As Boolean() Implements iPkmnEx.unkdata2
        Public Property unksp5() As Boolean() Implements iPkmnEx.unksp5
        Public Property name As String Implements iPkmnEx.name

        Public Sub New()
            unksp1 = New Boolean(LEN_UNKS1 - 1) {}
            unksp2 = New Boolean(LEN_UNKS2 - 1) {}
            unksp3 = New Boolean(LEN_UNKS3 - 1) {}
            stats = New Integer(NUM_STATS - 1) {}
            attacks = New SkyAttackEx(NUM_ATTACKS - 1) {}
            For i As Integer = 0 To NUM_ATTACKS - 1
                attacks(i) = New SkyAttackEx()
            Next i
            unksp4 = New Boolean(LEN_UNKS4 - 1) {}
            unkdata2 = New Boolean(LEN_UNK2 - 1) {}
            unksp5 = New Boolean(LEN_UNKS5 - 1) {}
            name = ""

        End Sub

        Public Sub New(ByVal buf As BooleanBuffer)
            Me.New()
            load(buf)
        End Sub

        Public Overridable Sub load(ByVal buf As BooleanBuffer)
            unksp1 = buf.get(LEN_UNKS1)
            lvl = buf.getInt(LEN_LVL)
            metat = buf.getInt(LEN_METAT)
            metfl = buf.getInt(LEN_METFL)
            unksp2 = buf.get(LEN_UNKS2)
            iq = buf.getInt(LEN_IQ)
            roster_no = buf.getInt(LEN_ROSTER_NO)
            unksp3 = buf.get(LEN_UNKS3)
            Dim mfno As Integer = buf.getInt(LEN_NO)
            no = mfno Mod FEMALE_ADD
            isfemale = mfno >= FEMALE_ADD
            hp1 = buf.getInt(LEN_HP1)
            hp2 = buf.getInt(LEN_HP2)
            stats = buf.getInts(NUM_STATS, LEN_STAT)
            exp = buf.getInt(LEN_EXP)
            For i As Integer = 0 To NUM_ATTACKS - 1
                attacks(i).load(buf.view(LEN_ATTACK_EX))
            Next i
            unksp4 = buf.get(LEN_UNKS4)
            unkdata2 = buf.get(LEN_UNK2)
            unksp5 = buf.get(LEN_UNKS5)
            name = SkyCharConv.decode(buf.getBytes(BLEN_NAME))
        End Sub

        Public Overridable Sub store(ByVal buf As BooleanBuffer)
            buf.put(unksp1)
            buf.putInt(lvl, LEN_LVL)
            buf.putInt(metat, LEN_METAT)
            buf.putInt(metfl, LEN_METFL)
            buf.put(unksp2)
            buf.putInt(iq, LEN_IQ)
            buf.putInt(roster_no, LEN_ROSTER_NO)
            buf.put(unksp3)
            Dim mfno As Integer = no + (If(isfemale, FEMALE_ADD, 0))
            buf.putInt(mfno, LEN_NO)
            buf.putInt(hp1, LEN_HP1)
            buf.putInt(hp2, LEN_HP2)
            buf.putInts(stats, LEN_STAT)
            buf.putInt(exp, LEN_EXP)
            For i As Integer = 0 To NUM_ATTACKS - 1
                attacks(i).store(buf.view(LEN_ATTACK_EX))
            Next i
            buf.put(unksp4)
            buf.put(unkdata2)
            buf.put(unksp5)
            buf.putBytes(SkyCharConv.encode(name))
        End Sub

        Public Overrides Function ToString() As String Implements iPkmnEx.Tostring
            Return String.Format("{0} (Lvl. {1} {2})", name, lvl, Lists.SkyPokemon(no))
            ' Return Name & " (Lvl. " & Level & " Pokemon " & ID & ")"
        End Function

    End Class
    Public Class TDPkmnEx
        Implements iPkmnEx
        Private Const FEMALE_ADD As Integer = 600

        Private Const NUM_STATS As Integer = 4
        Private Const NUM_ATTACKS As Integer = 4

        Private Const LEN_UNKS1 As Integer = 5
        Private Const LEN_LVL As Integer = 7
        Private Const LEN_METAT As Integer = 8
        Private Const LEN_METFL As Integer = 7
        Private Const LEN_UNKS2 As Integer = 1
        Private Const LEN_IQ As Integer = 10
        Private Const LEN_ROSTER_NO As Integer = 10
        Private Const LEN_UNKS3 As Integer = 22
        Private Const LEN_NO As Integer = 11
        Private Const LEN_HP1 As Integer = 10
        Private Const LEN_HP2 As Integer = 10
        Private Const LEN_STAT As Integer = 8
        Private Const LEN_EXP As Integer = 24
        Private Const LEN_ATTACK_EX As Integer = 29
        Private Const LEN_UNKS4 As Integer = 105
        Private Const LEN_UNK2 As Integer = 73
        Private Const LEN_UNKS5 As Integer = 13
        Private Const BLEN_NAME As Integer = 10
        '0 free

        Public Property unksp1() As Boolean() Implements iPkmnEx.unksp1
        Public Property lvl As Integer Implements iPkmnEx.lvl
        Public Property metat As Integer Implements iPkmnEx.metat
        Public Property metfl As Integer Implements iPkmnEx.metfl
        Public Property unksp2() As Boolean() Implements iPkmnEx.unksp2
        Public Property iq As Integer Implements iPkmnEx.iq
        Public Property roster_no As Integer Implements iPkmnEx.roster_no
        Public Property unksp3() As Boolean() Implements iPkmnEx.unksp3
        Public Property no As Integer Implements iPkmnEx.no
        Public Property isfemale As Boolean Implements iPkmnEx.isfemale
        Public Property hp1 As Integer Implements iPkmnEx.hp1
        Public Property hp2 As Integer Implements iPkmnEx.hp2
        Public Property stats() As Integer() Implements iPkmnEx.stats ' atk spa def spd
        Public Property exp As Integer Implements iPkmnEx.exp
        Public Property attacks() As SkyAttackEx() Implements iPkmnEx.attacks
        Public Property unksp4() As Boolean() Implements iPkmnEx.unksp4
        Public Property unkdata2() As Boolean() Implements iPkmnEx.unkdata2
        Public Property unksp5() As Boolean() Implements iPkmnEx.unksp5
        Public Property name As String Implements iPkmnEx.name

        Public Sub New()
            unksp1 = New Boolean(LEN_UNKS1 - 1) {}
            unksp2 = New Boolean(LEN_UNKS2 - 1) {}
            unksp3 = New Boolean(LEN_UNKS3 - 1) {}
            stats = New Integer(NUM_STATS - 1) {}
            attacks = New SkyAttackEx(NUM_ATTACKS - 1) {}
            For i As Integer = 0 To NUM_ATTACKS - 1
                attacks(i) = New SkyAttackEx()
            Next i
            unksp4 = New Boolean(LEN_UNKS4 - 1) {}
            unkdata2 = New Boolean(LEN_UNK2 - 1) {}
            unksp5 = New Boolean(LEN_UNKS5 - 1) {}
            name = ""

        End Sub

        Public Sub New(ByVal buf As BooleanBuffer)
            Me.New()
            load(buf)
        End Sub

        Public Overridable Sub load(ByVal buf As BooleanBuffer)
            unksp1 = buf.get(LEN_UNKS1)
            lvl = buf.getInt(LEN_LVL)
            metat = buf.getInt(LEN_METAT)
            metfl = buf.getInt(LEN_METFL)
            unksp2 = buf.get(LEN_UNKS2)
            iq = buf.getInt(LEN_IQ)
            roster_no = buf.getInt(LEN_ROSTER_NO)
            unksp3 = buf.get(LEN_UNKS3)
            Dim mfno As Integer = buf.getInt(LEN_NO)
            no = mfno Mod FEMALE_ADD
            isfemale = mfno >= FEMALE_ADD
            hp1 = buf.getInt(LEN_HP1)
            hp2 = buf.getInt(LEN_HP2)
            stats = buf.getInts(NUM_STATS, LEN_STAT)
            exp = buf.getInt(LEN_EXP)
            For i As Integer = 0 To NUM_ATTACKS - 1
                attacks(i).load(buf.view(LEN_ATTACK_EX))
            Next i
            unksp4 = buf.get(LEN_UNKS4)
            unkdata2 = buf.get(LEN_UNK2)
            unksp5 = buf.get(LEN_UNKS5)
            name = SkyCharConv.decode(buf.getBytes(BLEN_NAME - 1))
        End Sub

        Public Overridable Sub store(ByVal buf As BooleanBuffer)
            buf.put(unksp1)
            buf.putInt(lvl, LEN_LVL)
            buf.putInt(metat, LEN_METAT)
            buf.putInt(metfl, LEN_METFL)
            buf.put(unksp2)
            buf.putInt(iq, LEN_IQ)
            buf.putInt(roster_no, LEN_ROSTER_NO)
            buf.put(unksp3)
            Dim mfno As Integer = no + (If(isfemale, FEMALE_ADD, 0))
            buf.putInt(mfno, LEN_NO)
            buf.putInt(hp1, LEN_HP1)
            buf.putInt(hp2, LEN_HP2)
            buf.putInts(stats, LEN_STAT)
            buf.putInt(exp, LEN_EXP)
            For i As Integer = 0 To NUM_ATTACKS - 1
                attacks(i).store(buf.view(LEN_ATTACK_EX))
            Next i
            buf.put(unksp4)
            buf.put(unkdata2)
            buf.put(unksp5)
            buf.putBytes(SkyCharConv.encode(name))
        End Sub

        Public Overrides Function ToString() As String Implements iPkmnEx.Tostring
            Return String.Format("{0} (Lvl. {1} {2})", name, lvl, Lists.SkyPokemon(no))
            ' Return Name & " (Lvl. " & Level & " Pokemon " & ID & ")"
        End Function

    End Class

End Namespace