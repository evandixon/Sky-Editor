Imports SkyEditor.skyjed.buffer

Namespace skyjed.save

	Public Class ItemShop

		Private Const LEN_ID As Integer = 11
		Private Const LEN_PARAM As Integer = 11

		Public items() As SkyItem

		Public Sub New(ByVal numItems As Integer)
			items = New SkyItem(numItems - 1){}
			For i As Integer = 0 To items.Length - 1
				items(i) = New SkyItem()
			Next i
		End Sub

		Public Sub New(ByVal buf As BooleanBuffer)
			Me.New(buf.remaining()/(LEN_ID+LEN_PARAM))
			load(buf)
		End Sub

        Public Sub load(ByVal buf As BooleanBuffer)
            For i As Integer = 0 To items.Length - 1
                Dim id As Integer = buf.getInt(LEN_ID)
                Dim param As Integer = buf.getInt(LEN_PARAM)
                items(i) = New SkyItem(id, param)
            Next i
        End Sub

        Public Sub store(ByVal buf As BooleanBuffer)
            For i As Integer = 0 To items.Length - 1
                buf.putInt(items(i).id, LEN_ID)
                buf.putInt(items(i).param, LEN_PARAM)
            Next i
        End Sub

	End Class

End Namespace