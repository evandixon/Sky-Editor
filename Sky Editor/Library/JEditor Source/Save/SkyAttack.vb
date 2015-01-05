Imports SkyEditor.skyjed.buffer

Namespace skyjed.save
    Public MustInherit Class Attack
        MustOverride Sub load(ByVal buf As BooleanBuffer)
        MustOverride Sub store(ByVal buf As BooleanBuffer)
        MustOverride Property isvalid As Boolean
        MustOverride Property islinked As Boolean
        MustOverride Property isswitched As Boolean
        MustOverride Property isset As Boolean
        MustOverride Property no As Integer
        MustOverride Property ginseng As Integer
    End Class
	Public Class SkyAttack
        Inherits Attack
		Private Const LEN_NO As Integer = 10
		Private Const LEN_GINSENG As Integer = 7

        Public Overrides Property isvalid As Boolean
        Public Overrides Property islinked As Boolean
        Public Overrides Property isswitched As Boolean
        Public Overrides Property isset As Boolean
        Public Overrides Property no As Integer
        Public Overrides Property ginseng As Integer

		Public Sub New()
		End Sub

		Public Sub New(ByVal buf As BooleanBuffer)
			Me.New()
			load(buf)
		End Sub

        Public Overrides Sub load(ByVal buf As BooleanBuffer)
            isvalid = buf.get()
            islinked = buf.get()
            isswitched = buf.get()
            isset = buf.get()
            no = buf.getInt(LEN_NO)
            ginseng = buf.getInt(LEN_GINSENG)
        End Sub

        Public Overrides Sub store(ByVal buf As BooleanBuffer)
            buf.put(isvalid)
            buf.put(islinked)
            buf.put(isswitched)
            buf.put(isset)
            buf.putInt(no, LEN_NO)
            buf.putInt(ginseng, LEN_GINSENG)
        End Sub

    End Class
    Public Class RBAttack
        Inherits Attack
        Private Const LEN_NO As Integer = 9
        Private Const LEN_GINSENG As Integer = 7

        Public Overrides Property isvalid As Boolean
        Public Overrides Property islinked As Boolean
        Public Overrides Property isswitched As Boolean
        Public Overrides Property isset As Boolean
        Public Overrides Property no As Integer
        Public Overrides Property ginseng As Integer

        Public Sub New()
        End Sub

        Public Sub New(ByVal buf As BooleanBuffer)
            Me.New()
            load(buf)
        End Sub

        Public Overrides Sub load(ByVal buf As BooleanBuffer)
            isvalid = buf.get()
            islinked = buf.get()
            isswitched = buf.get()
            isset = buf.get()
            no = buf.getInt(LEN_NO)
            ginseng = buf.getInt(LEN_GINSENG)
        End Sub

        Public Overrides Sub store(ByVal buf As BooleanBuffer)
            buf.put(isvalid)
            buf.put(islinked)
            buf.put(isswitched)
            buf.put(isset)
            buf.putInt(no, LEN_NO)
            buf.putInt(ginseng, LEN_GINSENG)
        End Sub

    End Class

End Namespace