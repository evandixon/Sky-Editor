Imports SkyEditor.skyjed.buffer
Imports SkyEditor.skyjed.util
Namespace skyjed.save
    Public Interface iPkmn
        Property isvalid As Boolean
        Property lvl As Integer
        Property no As Integer
        Property isfemale As Boolean
        Property metat As Integer
        Property metfl As Integer
        Property unkdata1() As Boolean()
        Property iq As Integer
        Property hp As Integer
        Property stats() As Integer() ' atk spa def spd
        Property exp As Integer
        Property unkdata2() As Boolean()
        Property attacks() As Attack()
        Property name As String
        Function ToString() As String
        Function GetIsValid() As Boolean
        Function GetBytes() As Byte()
    End Interface
    Public Class SkyPkmn
        Implements iPkmn

        Public Const LENGTH As Integer = 362
        Public Shared ReadOnly MIME_TYPE As String = "application/x-sky-pokemon; class=[Z"

        Private Const FEMALE_ADD As Integer = 600

        Private Const NUM_STATS As Integer = 4
        Private Const NUM_ATTACKS As Integer = 4

        Private Const LEN_LVL As Integer = 7
        Private Const LEN_NO As Integer = 11
        Private Const LEN_METAT As Integer = 8
        Private Const LEN_METFL As Integer = 7
        Public Const LEN_UNK1 As Integer = 15
        Private Const LEN_IQ As Integer = 10
        Private Const LEN_HP As Integer = 10
        Private Const LEN_STAT As Integer = 8
        Private Const LEN_EXP As Integer = 24
        Public Const LEN_UNK2 As Integer = 73
        Private Const LEN_ATTACK As Integer = 21
        Private Const BLEN_NAME As Integer = 10

        Public Property isvalid As Boolean Implements iPkmn.isvalid
        Public Property lvl As Integer Implements iPkmn.lvl
        Public Property no As Integer Implements iPkmn.no
        Public Property isfemale As Boolean Implements iPkmn.isfemale
        Public Property metat As Integer Implements iPkmn.metat
        Public Property metfl As Integer Implements iPkmn.metfl
        Public Property unkdata1() As Boolean() Implements iPkmn.unkdata1
        Public Property iq As Integer Implements iPkmn.iq
        Public Property hp As Integer Implements iPkmn.hp
        Public Property stats() As Integer() Implements iPkmn.stats ' atk spa def spd
        Public Property exp As Integer Implements iPkmn.exp
        Public Property unkdata2() As Boolean() Implements iPkmn.unkdata2
        Public Property attacks() As Attack() Implements iPkmn.attacks
        Public Property name As String Implements iPkmn.name

        Public Sub New()
            unkdata1 = New Boolean(LEN_UNK1 - 1) {}
            stats = New Integer(NUM_STATS - 1) {}
            unkdata2 = New Boolean(LEN_UNK2 - 1) {}
            attacks = New SkyAttack(NUM_ATTACKS - 1) {}
            For i As Integer = 0 To NUM_ATTACKS - 1
                attacks(i) = New SkyAttack()
            Next i
            name = ""
        End Sub

        Public Sub New(ByVal buf As BooleanBuffer)
            Me.New()
            load(buf)
        End Sub

        Public Sub load(ByVal buf As BooleanBuffer)
            isvalid = buf.get()
            lvl = buf.getInt(LEN_LVL)
            Dim mfno As Integer = buf.getInt(LEN_NO)
            no = mfno Mod FEMALE_ADD
            isfemale = mfno >= FEMALE_ADD
            metat = buf.getInt(LEN_METAT)
            metfl = buf.getInt(LEN_METFL)
            unkdata1 = buf.get(LEN_UNK1)
            iq = buf.getInt(LEN_IQ)
            hp = buf.getInt(LEN_HP)
            stats = buf.getInts(NUM_STATS, LEN_STAT)
            exp = buf.getInt(LEN_EXP)
            unkdata2 = buf.get(LEN_UNK2)
            For i As Integer = 0 To NUM_ATTACKS - 1
                attacks(i).load(buf.view(LEN_ATTACK))
            Next i
            name = SkyCharConv.decode(buf.getBytes(BLEN_NAME))
        End Sub

        Public Sub store(ByVal buf As BooleanBuffer)
            buf.put(isvalid)
            buf.putInt(lvl, LEN_LVL)
            Dim mfno As Integer = no + (If(isfemale, FEMALE_ADD, 0))
            buf.putInt(mfno, LEN_NO)
            buf.putInt(metat, LEN_METAT)
            buf.putInt(metfl, LEN_METFL)
            buf.put(unkdata1)
            buf.putInt(iq, LEN_IQ)
            buf.putInt(hp, LEN_HP)
            buf.putInts(stats, LEN_STAT)
            buf.putInt(exp, LEN_EXP)
            buf.put(unkdata2)
            For i As Integer = 0 To NUM_ATTACKS - 1
                attacks(i).store(buf.view(LEN_ATTACK))
            Next i
            buf.putBytes(SkyCharConv.encode(name))
        End Sub

        Public Sub clear()
            Dim arr(LENGTH - 1) As Boolean
            Dim buf As BooleanBuffer = New BooleanBufferArray(arr)
            load(buf)
        End Sub

        'Public Overridable Sub copy()
        'Dim arr(LENGTH - 1) As Boolean
        'Dim buf As BooleanBuffer = New BooleanBufferArray(arr)
        'store(buf)
        'ClipboardHelper.setClipboardContents(arr, MIME_TYPE)
        'End Sub

        'Public Overridable Sub paste()
        'Dim arr() As Boolean = CType(ClipboardHelper.getClipboardContents(MIME_TYPE), Boolean())
        'Dim buf As BooleanBuffer = New BooleanBufferArray(arr)
        'load(buf)
        'End Sub

        Public Overrides Function ToString() As String Implements iPkmn.ToString
            If Not isvalid Then
                Return "----------"
            End If
            Return String.Format("{0} (Lvl. {1} {2})", name, lvl, Lists.SkyPokemon(no))
            ' Return Name &  (Lvl.  & Level &  Pokemon  & ID & )
        End Function

        Public Function GetIsValid() As Boolean Implements iPkmn.GetIsValid
            Return isvalid
        End Function

        Public Function GetBytes() As Byte() Implements iPkmn.GetBytes
            Dim arr(LENGTH - 1) As Boolean
            Dim x1 As New BooleanBufferArray(arr)
            store(x1)
            Dim arr2(LENGTH - 1 + 6) As Boolean
            Dim x2 As New BooleanBufferArray(arr2)
            x2.put(False)
            x2.put(False)
            x2.put(False)
            x2.put(False)
            x2.put(False)
            x2.put(False)
            x2.put(x1.GetSplitBytes)
            Dim lengthBytes As Integer = Math.Ceiling((LENGTH + 6) / 8)
            Dim out As Byte() = BitConverterLE.packBits(x2.GetSplitBytes)
            Return out
        End Function
        Public Shared Function FromBytes(Bytes As Byte()) As SkyPkmn
            Dim out As New SkyPkmn
            Dim x1 As New BooleanBufferArray(BitConverterLE.splitBits(Bytes))
            out.load(x1.seek(6).view(LENGTH))
            Return out
        End Function
    End Class
    Public Class TDPkmn
        Implements iPkmn

        Public Const LENGTH As Integer = 388
        Public Shared ReadOnly MIME_TYPE As String = "application/x-sky-pokemon; class=[Z"

        Private Const FEMALE_ADD As Integer = 600

        Private Const NUM_STATS As Integer = 4
        Private Const NUM_ATTACKS As Integer = 4

        '5 extra
        Private Const LEN_LVL As Integer = 7
        Private Const LEN_NO As Integer = 11
        Private Const LEN_METAT As Integer = 8
        Private Const LEN_METFL As Integer = 7
        Public Const LEN_UNK1 As Integer = 15
        Private Const LEN_IQ As Integer = 10
        Private Const LEN_HP As Integer = 10
        Private Const LEN_STAT As Integer = 8
        Private Const LEN_EXP As Integer = 24
        Public Const LEN_UNK2 As Integer = (73 + 23)
        Private Const LEN_ATTACK As Integer = 21
        Private Const BLEN_NAME As Integer = 10

        Public Property isvalid As Boolean Implements iPkmn.isvalid
        Public Property lvl As Integer Implements iPkmn.lvl
        Public Property no As Integer Implements iPkmn.no
        Public Property isfemale As Boolean Implements iPkmn.isfemale
        Public Property metat As Integer Implements iPkmn.metat
        Public Property metfl As Integer Implements iPkmn.metfl
        Public Property unkdata1() As Boolean() Implements iPkmn.unkdata1
        Public Property iq As Integer Implements iPkmn.iq
        Public Property hp As Integer Implements iPkmn.hp
        Public Property stats() As Integer() Implements iPkmn.stats ' atk spa def spd
        Public Property exp As Integer Implements iPkmn.exp
        Public Property unkdata2() As Boolean() Implements iPkmn.unkdata2
        Public Property attacks() As Attack() Implements iPkmn.attacks
        Public Property name As String Implements iPkmn.name

        Public Sub New()
            unkdata1 = New Boolean(LEN_UNK1 - 1) {}
            stats = New Integer(NUM_STATS - 1) {}
            unkdata2 = New Boolean(LEN_UNK2 - 1) {}
            attacks = New SkyAttack(NUM_ATTACKS - 1) {}
            For i As Integer = 0 To NUM_ATTACKS - 1
                attacks(i) = New SkyAttack()
            Next i
            name = ""
        End Sub

        Public Sub New(ByVal buf As BooleanBuffer)
            Me.New()
            load(buf)
        End Sub

        Public Sub load(ByVal buf As BooleanBuffer)
            isvalid = buf.get()
            lvl = buf.getInt(LEN_LVL)
            Dim mfno As Integer = buf.getInt(LEN_NO)
            no = mfno Mod FEMALE_ADD
            isfemale = mfno >= FEMALE_ADD
            metat = buf.getInt(LEN_METAT)
            metfl = buf.getInt(LEN_METFL)
            unkdata1 = buf.get(LEN_UNK1)
            iq = buf.getInt(LEN_IQ)
            hp = buf.getInt(LEN_HP)
            stats = buf.getInts(NUM_STATS, LEN_STAT)
            exp = buf.getInt(LEN_EXP)
            unkdata2 = buf.get(LEN_UNK2)
            For i As Integer = 0 To NUM_ATTACKS - 1
                attacks(i).load(buf.view(LEN_ATTACK))
            Next i
            name = SkyCharConv.decode(buf.getBytes(BLEN_NAME))
        End Sub

        Public Sub store(ByVal buf As BooleanBuffer)
            buf.put(isvalid)
            buf.putInt(lvl, LEN_LVL)
            Dim mfno As Integer = no + (If(isfemale, FEMALE_ADD, 0))
            buf.putInt(mfno, LEN_NO)
            buf.putInt(metat, LEN_METAT)
            buf.putInt(metfl, LEN_METFL)
            buf.put(unkdata1)
            buf.putInt(iq, LEN_IQ)
            buf.putInt(hp, LEN_HP)
            buf.putInts(stats, LEN_STAT)
            buf.putInt(exp, LEN_EXP)
            buf.put(unkdata2)
            For i As Integer = 0 To NUM_ATTACKS - 1
                attacks(i).store(buf.view(LEN_ATTACK))
            Next i
            buf.putBytes(SkyCharConv.encode(name))
        End Sub

        Public Sub clear()
            Dim arr(LENGTH - 1) As Boolean
            Dim buf As BooleanBuffer = New BooleanBufferArray(arr)
            load(buf)
        End Sub

        'Public Overridable Sub copy()
        'Dim arr(LENGTH - 1) As Boolean
        'Dim buf As BooleanBuffer = New BooleanBufferArray(arr)
        'store(buf)
        'ClipboardHelper.setClipboardContents(arr, MIME_TYPE)
        'End Sub

        'Public Overridable Sub paste()
        'Dim arr() As Boolean = CType(ClipboardHelper.getClipboardContents(MIME_TYPE), Boolean())
        'Dim buf As BooleanBuffer = New BooleanBufferArray(arr)
        'load(buf)
        'End Sub

        Public Overrides Function ToString() As String Implements iPkmn.ToString
            If Not GetIsValid() Then
                Return "----------"
            End If
            Return String.Format("{0} (Lvl. {1} {2})", name, lvl, Lists.SkyPokemon(no))
            ' Return Name &  (Lvl.  & Level &  Pokemon  & ID & )
        End Function

        Public Function GetIsValid() As Boolean Implements iPkmn.GetIsValid
            Return Not (lvl = 0)
        End Function

        Public Function GetBytes() As Byte() Implements iPkmn.GetBytes
            Dim arr(LENGTH - 1) As Boolean
            Dim x1 As New BooleanBufferArray(arr)
            store(x1)
            Dim arr2(LENGTH - 1 + 6) As Boolean
            Dim x2 As New BooleanBufferArray(arr2)
            x2.put(False)
            x2.put(False)
            x2.put(False)
            x2.put(False)
            x2.put(False)
            x2.put(False)
            x2.put(x1.GetSplitBytes)
            Dim lengthBytes As Integer = Math.Ceiling((LENGTH + 6) / 8)
            Dim out As Byte() = BitConverterLE.packBits(x2.GetSplitBytes)
            Return out
        End Function
        Public Shared Function FromBytes(Bytes As Byte()) As SkyPkmn
            Dim out As New SkyPkmn
            Dim x1 As New BooleanBufferArray(BitConverterLE.splitBits(Bytes))
            out.load(x1.seek(6).view(LENGTH))
            Return out
        End Function
    End Class
    Public Class RBPkmn
        Implements iPkmn
        Public Const LENGTH As Integer = 323
        Public Shared ReadOnly MIME_TYPE As String = "application/x-sky-pokemon; class=[Z"

        Private Const FEMALE_ADD As Integer = 0 '1536 '600

        Private Const NUM_STATS As Integer = 4
        Private Const NUM_ATTACKS As Integer = 4

        Private Const LEN_LVL As Integer = 7
        Private Const LEN_NO As Integer = 9
        'Private Const LEN_Unknown As Integer = 1
        'Private Const LEN_UNK0 As Integer = 2
        Private Const LEN_METAT As Integer = 7
        'Private Const LEN_METFL As Integer = 7
        Public Const LEN_UNK1 As Integer = 21
        Private Const LEN_IQ As Integer = 10
        Private Const LEN_HP As Integer = 10
        Private Const LEN_STAT As Integer = 8
        Private Const LEN_EXP As Integer = 24
        Public Const LEN_UNK2 As Integer = 37 + 6 '(73 + 23)
        Private Const LEN_ATTACK As Integer = 20
        Private Const BLEN_NAME As Integer = 10

        Private _originalBuffer As BooleanBuffer

        ''' <summary>
        ''' Depricated; use GetIsValid()
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property isvalid As Boolean Implements iPkmn.isvalid
        Public Property lvl As Integer Implements iPkmn.lvl
        Public Property unknown As Integer
        Public Property no As Integer Implements iPkmn.no
        Public Property isfemale As Boolean Implements iPkmn.isfemale
        Public Property unk0 As Boolean
        Public Property metat As Integer Implements iPkmn.metat
        Public Property metfl As Integer Implements iPkmn.metfl
        Public Property unkdata1() As Boolean() Implements iPkmn.unkdata1
        Public Property iq As Integer Implements iPkmn.iq
        Public Property hp As Integer Implements iPkmn.hp
        Public Property stats() As Integer() Implements iPkmn.stats ' atk spa def spd
        Public Property exp As Integer Implements iPkmn.exp
        Public Property unkdata2() As Boolean() Implements iPkmn.unkdata2
        Public Property attacks() As Attack() Implements iPkmn.attacks
        Public Property name As String Implements iPkmn.name

        Public Sub New()
            unkdata1 = New Boolean(LEN_UNK1 - 1) {}
            stats = New Integer(NUM_STATS - 1) {}
            unkdata2 = New Boolean(LEN_UNK2 - 1) {}
            attacks = New RBAttack(NUM_ATTACKS - 1) {}
            For i As Integer = 0 To NUM_ATTACKS - 1
                attacks(i) = DirectCast(New RBAttack(), Attack)
            Next i
            name = ""
        End Sub

        Public Sub New(ByVal buf As BooleanBuffer)
            Me.New()
            load(buf)
        End Sub
        Public Sub New(Pkm As RBPkmn)
            Me.isvalid = Pkm.isvalid
            Me.lvl = Pkm.lvl
            Me.unknown = Pkm.unknown
            Me.no = Pkm.no
            Me.isfemale = Pkm.isfemale
            Me.metat = Pkm.metat
            Me.unk0 = Pkm.unk0
            Me.metfl = Pkm.metfl
            Me.unkdata1 = Pkm.unkdata1
            Me.iq = Pkm.iq
            Me.hp = Pkm.hp
            Me.stats = Pkm.stats
            Me.exp = Pkm.exp
            Me.unkdata2 = Pkm.unkdata2
            Me.attacks = Pkm.attacks
            Me.name = Pkm.name
        End Sub

        Public Sub load(ByVal buf As BooleanBuffer)
            _originalBuffer = buf
            buf.seek(0)
            'isvalid = buf.get()
            lvl = buf.getInt(LEN_LVL)
            no = buf.getInt(LEN_NO)
            'unknown = buf.getInt(LEN_Unknown)
            isfemale = False
            'unk0 = buf.getInt(LEN_UNK0)
            metat = buf.getInt(LEN_METAT)
            'metfl = buf.getInt(LEN_METFL)
            unkdata1 = buf.get(LEN_UNK1)
            iq = buf.getInt(LEN_IQ)
            hp = buf.getInt(LEN_HP)
            stats = buf.getInts(NUM_STATS, LEN_STAT)
            exp = buf.getInt(LEN_EXP)
            unkdata2 = buf.get(LEN_UNK2)
            For i As Integer = 0 To NUM_ATTACKS - 1
                attacks(i).load(buf.view(LEN_ATTACK))
            Next i
            name = SkyCharConv.decode(buf.getBytes(BLEN_NAME))
        End Sub

        Public Sub store(ByVal buf As BooleanBuffer)
            'buf.put(isvalid)
            buf.putInt(lvl, LEN_LVL)
            buf.putInt(no, LEN_NO)
            'buf.putInt(unknown, LEN_Unknown)
            'buf.putInt(unk0, LEN_UNK0)
            buf.putInt(metat, LEN_METAT)
            'buf.putInt(metfl, LEN_METFL)
            buf.put(unkdata1)
            buf.putInt(iq, LEN_IQ)
            buf.putInt(hp, LEN_HP)
            buf.putInts(stats, LEN_STAT)
            buf.putInt(exp, LEN_EXP)
            buf.put(unkdata2)
            For i As Integer = 0 To NUM_ATTACKS - 1
                attacks(i).store(buf.view(LEN_ATTACK))
            Next i
            buf.putBytes(SkyCharConv.encode(name))
        End Sub

        Public Sub clear()
            Dim arr(LENGTH - 1) As Boolean
            Dim buf As BooleanBuffer = New BooleanBufferArray(arr)
            load(buf)
        End Sub

        'Public Overridable Sub copy()
        'Dim arr(LENGTH - 1) As Boolean
        'Dim buf As BooleanBuffer = New BooleanBufferArray(arr)
        'store(buf)
        'ClipboardHelper.setClipboardContents(arr, MIME_TYPE)
        'End Sub

        'Public Overridable Sub paste()
        'Dim arr() As Boolean = CType(ClipboardHelper.getClipboardContents(MIME_TYPE), Boolean())
        'Dim buf As BooleanBuffer = New BooleanBufferArray(arr)
        'load(buf)
        'End Sub

        Public Overrides Function ToString() As String Implements iPkmn.ToString
            On Error Resume Next
            If Not GetIsValid() Then
                Return "----------"
            End If
            Return String.Format("{0} (Lvl. {1} {2})", name, lvl, Lists.RBPokemon(no))
            ' Return Name &  (Lvl.  & Level &  Pokemon  & ID & )
        End Function
        Public Function GetIsValid() As Boolean Implements iPkmn.GetIsValid
            Return Not (lvl = 0)
        End Function

        Public Function GetBytes() As Byte() Implements iPkmn.GetBytes
            Dim arr(LENGTH - 1) As Boolean
            Dim x1 As New BooleanBufferArray(arr)
            store(x1)
            Dim arr2(LENGTH - 1 + 6) As Boolean
            Dim x2 As New BooleanBufferArray(arr2)
            x2.put(False)
            x2.put(False)
            x2.put(False)
            x2.put(False)
            x2.put(False)
            x2.put(False)
            x2.put(x1.GetSplitBytes)
            Dim lengthBytes As Integer = Math.Ceiling((LENGTH + 6) / 8)
            Dim out As Byte() = BitConverterLE.packBits(x2.GetSplitBytes)
            Return out
        End Function
        Public Function GetBuffer() As BooleanBuffer
            Return _originalBuffer
        End Function
        Public Shared Function FromBytes(Bytes As Byte()) As SkyPkmn
            Dim out As New SkyPkmn
            Dim x1 As New BooleanBufferArray(BitConverterLE.splitBits(Bytes))
            out.load(x1.seek(6).view(LENGTH))
            Return out
        End Function
    End Class

End Namespace