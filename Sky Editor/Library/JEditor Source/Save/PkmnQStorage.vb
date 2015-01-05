Imports SkyEditor.skyjed.buffer
Namespace skyjed.save
    Public Class PkmnQStorage

        Private Const PKMN_NUM As Integer = 20
        Private Const PKMN_BYTE_SIZE As Integer = 429

        Public pkmns() As SkyPkmnQ

        Public Sub New()
            pkmns = New SkyPkmnQ(PKMN_NUM - 1) {}
            For i As Integer = 0 To PKMN_NUM - 1
                pkmns(i) = New SkyPkmnQ()
            Next i
        End Sub

        Public Sub New(ByVal buf As BooleanBuffer)
            Me.New()
            load(buf)
        End Sub

        Public Overridable Sub load(ByVal buf As BooleanBuffer)
            For i As Integer = 0 To PKMN_NUM - 1
                pkmns(i).load(buf.view(8 * PKMN_BYTE_SIZE))
            Next i
        End Sub

        Public Overridable Sub store(ByVal buf As BooleanBuffer)
            For i As Integer = 0 To PKMN_NUM - 1
                pkmns(i).store(buf.view(8 * PKMN_BYTE_SIZE))
            Next i
        End Sub
    End Class
End Namespace