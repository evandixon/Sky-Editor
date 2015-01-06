Imports SkyEditor.skyjed.buffer

Namespace skyjed.save
    Public Interface Attack
        Sub load(ByVal buf As BooleanBuffer)
        Sub store(ByVal buf As BooleanBuffer)
        Property isvalid As Boolean
        Property islinked As Boolean
        Property isswitched As Boolean
        Property isset As Boolean
        Property no As Integer
        Property ginseng As Integer
    End Interface
    Public Class SkyAttack
        Implements Attack
        Private Const LEN_NO As Integer = 10
        Private Const LEN_GINSENG As Integer = 7

        Public Property isvalid As Boolean Implements Attack.isvalid
        Public Property islinked As Boolean Implements Attack.islinked
        Public Property isswitched As Boolean Implements Attack.isswitched
        Public Property isset As Boolean Implements Attack.isset
        Public Property no As Integer Implements Attack.no
        Public Property ginseng As Integer Implements Attack.ginseng

        Public Sub New()
        End Sub

        Public Sub New(ByVal buf As BooleanBuffer)
            Me.New()
            load(buf)
        End Sub

        Public Sub load(ByVal buf As BooleanBuffer) Implements Attack.load
            isvalid = buf.get()
            islinked = buf.get()
            isswitched = buf.get()
            isset = buf.get()
            no = buf.getInt(LEN_NO)
            ginseng = buf.getInt(LEN_GINSENG)
        End Sub

        Public Sub store(ByVal buf As BooleanBuffer) Implements Attack.store
            buf.put(isvalid)
            buf.put(islinked)
            buf.put(isswitched)
            buf.put(isset)
            buf.putInt(no, LEN_NO)
            buf.putInt(ginseng, LEN_GINSENG)
        End Sub

    End Class
    Public Class RBAttack
        Implements Attack
        Private Const LEN_NO As Integer = 9
        Private Const LEN_GINSENG As Integer = 7

        Public Property isvalid As Boolean Implements Attack.isvalid
        Public Property islinked As Boolean Implements Attack.islinked
        Public Property isswitched As Boolean Implements Attack.isswitched
        Public Property isset As Boolean Implements Attack.isset
        Public Property no As Integer Implements Attack.no
        Public Property ginseng As Integer Implements Attack.ginseng

        Public Sub New()
        End Sub

        Public Sub New(ByVal buf As BooleanBuffer)
            Me.New()
            load(buf)
        End Sub

        Public Sub load(ByVal buf As BooleanBuffer) Implements Attack.load
            isvalid = buf.get()
            islinked = buf.get()
            isswitched = buf.get()
            isset = buf.get()
            no = buf.getInt(LEN_NO)
            ginseng = buf.getInt(LEN_GINSENG)
        End Sub

        Public Sub store(ByVal buf As BooleanBuffer) Implements Attack.store
            buf.put(isvalid)
            buf.put(islinked)
            buf.put(isswitched)
            buf.put(isset)
            buf.putInt(no, LEN_NO)
            buf.putInt(ginseng, LEN_GINSENG)
        End Sub

    End Class

End Namespace