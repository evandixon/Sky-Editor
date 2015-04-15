Imports SkyEditor.skyjed.buffer

Namespace skyjed.save

    Public Class SkyAttackQ

        Public isvalid As Boolean
        Public islinked As Boolean
        Public isswitched As Boolean
        Public isset As Boolean
        Public issealed As Boolean
        Public unkn As Integer
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
            issealed = buf.get()
            buf.skip(3) ' remainder of flags
            unkn = buf.getInt(8)
            no = buf.getInt(16)
            pp = buf.getInt(8)
            ginseng = buf.getInt(8)
        End Sub

        Public Sub store(ByVal buf As BooleanBuffer)
            buf.put(isvalid)
            buf.put(islinked)
            buf.put(isswitched)
            buf.put(isset)
            buf.put(issealed)
            buf.skip(3) ' remainder of flags
            buf.putInt(unkn, 8)
            buf.putInt(no, 16)
            buf.putInt(pp, 8)
            buf.putInt(ginseng, 8)
        End Sub

    End Class

End Namespace