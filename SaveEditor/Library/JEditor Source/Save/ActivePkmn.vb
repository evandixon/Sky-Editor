Imports SkyEditor.skyjed.buffer

Namespace skyjed.save

    Public Class ActivePkmn

        Private Const PKMN_NUM As Integer = 4
        Private Const PKMN_SIZE As Integer = 546

        Public pkmns() As SkyPkmnEx

        Public Sub New()
            MyBase.New()
            pkmns = New SkyPkmnEx(PKMN_NUM - 1) {}
            For i As Integer = 0 To PKMN_NUM - 1
                pkmns(i) = New SkyPkmnEx()
            Next i
        End Sub

        Public Sub New(ByVal buf As BooleanBuffer)
            Me.New()
            load(buf)
        End Sub

        Public Sub load(ByVal buf As BooleanBuffer)
            For i As Integer = 0 To PKMN_NUM - 1
                pkmns(i).load(buf.view(PKMN_SIZE))
            Next i
        End Sub
        Public Sub store(ByVal buf As BooleanBuffer)
            For i As Integer = 0 To PKMN_NUM - 1
                If pkmns.Length > i Then
                    pkmns(i).store(buf.view(PKMN_SIZE))
                Else
                    Dim n As New SkyPkmnEx
                    n.store(buf.view(PKMN_SIZE))
                End If
            Next i
        End Sub

    End Class
    Public Class ActivePkmnTD

        Private Const PKMN_NUM As Integer = 4
        Private Const PKMN_SIZE As Integer = 544

        Public pkmns() As TDPkmnEx

        Public Sub New()
            pkmns = New TDPkmnEx(PKMN_NUM - 1) {}
            For i As Integer = 0 To PKMN_NUM - 1
                pkmns(i) = New TDPkmnEx()
            Next i
        End Sub

        Public Sub New(ByVal buf As BooleanBuffer)
            Me.New()
            load(buf)
        End Sub

        Public Sub load(ByVal buf As BooleanBuffer)
            For i As Integer = 0 To PKMN_NUM - 1
                pkmns(i).load(buf.view(PKMN_SIZE))
            Next i
        End Sub
        Public Sub store(ByVal buf As BooleanBuffer)
            For i As Integer = 0 To PKMN_NUM - 1
                If pkmns.Length > i Then
                    pkmns(i).store(buf.view(PKMN_SIZE))
                Else
                    Dim n As New TDPkmnEx
                    n.store(buf.view(PKMN_SIZE))
                End If
            Next i
        End Sub

    End Class

End Namespace