Imports SkyEditor.skyjed.buffer

Namespace skyjed.save

    Public Class SkyHeldItemStorage

        Private Const NUM_ITEMS As Integer = 50
        Private Const LEN_ITEM As Integer = 33

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
            For i As Integer = 0 To NUM_ITEMS - 1
                items(i).load(buf.view(LEN_ITEM))
            Next i
        End Sub

        Public Sub store(ByVal buf As BooleanBuffer)
            For i As Integer = 0 To NUM_ITEMS - 1
                items(i).store(buf.view(LEN_ITEM))
            Next i
        End Sub

    End Class
    Public Class TDHeldItemStorage

        Private Const NUM_ITEMS As Integer = 48
        Private Const LEN_ITEM As Integer = 31

        Public items() As TDItem

        Public Sub New()
            items = New TDItem(NUM_ITEMS - 1) {}
            For i As Integer = 0 To NUM_ITEMS - 1
                items(i) = New TDItem()
            Next i
        End Sub

        Public Sub New(ByVal buf As BooleanBuffer)
            Me.New()
            load(buf)
        End Sub

        Public Sub load(ByVal buf As BooleanBuffer)
            For i As Integer = 0 To NUM_ITEMS - 1
                items(i).load(buf.view(LEN_ITEM))
            Next i
        End Sub

        Public Sub store(ByVal buf As BooleanBuffer)
            For i As Integer = 0 To NUM_ITEMS - 1
                If items.Count > i AndAlso items(i).isvalid Then
                    items(i).store(buf.view(LEN_ITEM))
                Else
                    Dim x As New TDItem
                    x.store(buf.view(LEN_ITEM))
                End If
            Next i
        End Sub

    End Class
    Public Class RBHeldItemStorage

        Private Const NUM_ITEMS As Integer = 20
        Private Const LEN_ITEM As Integer = 23

        Public items() As RBItem

        Public Sub New()
            items = New RBItem(NUM_ITEMS - 1) {}
            For i As Integer = 0 To NUM_ITEMS - 1
                items(i) = New RBItem()
            Next i
        End Sub

        Public Sub New(ByVal buf As BooleanBuffer)
            Me.New()
            load(buf)
        End Sub

        Public Sub load(ByVal buf As BooleanBuffer)
            For i As Integer = 0 To NUM_ITEMS - 1
                items(i).load(buf.view(LEN_ITEM))
            Next i
        End Sub

        Public Sub store(ByVal buf As BooleanBuffer)
            For i As Integer = 0 To NUM_ITEMS - 1
                If items.Count > i AndAlso items(i).isvalid Then
                    items(i).store(buf.view(LEN_ITEM))
                Else
                    Dim x As New RBItem
                    x.store(buf.view(LEN_ITEM))
                End If
            Next i
        End Sub

    End Class
End Namespace