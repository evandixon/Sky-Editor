Imports SkyEditor.skyjed.buffer

Namespace skyjed.save

    Public Class ItemStorage

        Private Const NUM_ITEMS As Integer = 1000
        Private Const LEN_ID As Integer = 11
        Private Const LEN_PARAM As Integer = 11

        Public items() As SkyItem

        Public Sub New()
            items = New SkyItem(NUM_ITEMS - 1) {}
            For i As Integer = 0 To NUM_ITEMS - 1
                items(i) = New SkyItem()
            Next i
        End Sub

        Public Sub New(ByVal buf As BooleanBuffer)
            Me.New()
            load(buf)
        End Sub

        Public Sub load(ByVal buf As BooleanBuffer)
            Dim buf_ids As BooleanBuffer = buf.view(NUM_ITEMS * LEN_ID)
            Dim buf_params As BooleanBuffer = buf.view(NUM_ITEMS * LEN_PARAM)
            For i As Integer = 0 To NUM_ITEMS - 1
                items(i) = New SkyItem(buf_ids.getInt(LEN_ID), buf_params.getInt(LEN_PARAM))
            Next i
        End Sub

        Public Sub store(ByVal buf As BooleanBuffer)
            Dim buf_ids As BooleanBuffer = buf.view(NUM_ITEMS * LEN_ID)
            Dim buf_params As BooleanBuffer = buf.view(NUM_ITEMS * LEN_PARAM)
            For i As Integer = 0 To NUM_ITEMS - 1
                buf_ids.putInt(items(i).id, LEN_ID)
                buf_params.putInt(items(i).param, LEN_PARAM)
            Next i
        End Sub

    End Class

End Namespace