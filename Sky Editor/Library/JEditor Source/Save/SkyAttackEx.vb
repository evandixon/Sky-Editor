Imports SkyEditor.skyjed.buffer
Namespace skyjed.save

    Public Class SkyAttackEx

        Private Const LEN_NO As Integer = 10
        Private Const LEN_PP As Integer = 7
        Private Const LEN_GINSENG As Integer = 7

        Public isvalid As Boolean
        Public islinked As Boolean
        Public isswitched As Boolean
        Public isset As Boolean
        Public unkflag As Boolean
        Public no As Integer
        Public pp As Integer
        Public ginseng As Integer

        Public Sub New()
        End Sub

        Public Sub New(ByVal buf As BooleanBuffer)
            Me.New()
            load(buf)
        End Sub

        Public Sub load(ByVal buf As BooleanBuffer)
            isvalid = buf.get()
            islinked = buf.get()
            isswitched = buf.get()
            isset = buf.get()
            unkflag = buf.get()
            no = buf.getInt(LEN_NO)
            pp = buf.getInt(LEN_PP)
            ginseng = buf.getInt(LEN_GINSENG)
        End Sub

        Public Sub store(ByVal buf As BooleanBuffer)
            buf.put(isvalid)
            buf.put(islinked)
            buf.put(isswitched)
            buf.put(isset)
            buf.put(unkflag)
            buf.putInt(no, LEN_NO)
            buf.putInt(pp, LEN_PP)
            buf.putInt(ginseng, LEN_GINSENG)
        End Sub

    End Class

End Namespace