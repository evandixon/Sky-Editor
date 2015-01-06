Imports SkyEditor.skyjed.buffer
Namespace skyjed.save

    Public Class PkmnStorage

        Private Const PKMN_NUM As Integer = 720
        Private Const PKMN_SIZE As Integer = 362

        Public pkmns() As SkyPkmn

        Public Sub New()
            pkmns = New SkyPkmn(PKMN_NUM - 1) {}
            For i As Integer = 0 To PKMN_NUM - 1
                pkmns(i) = New SkyPkmn()
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
                    Dim x As New SkyPkmn
                    x.isvalid = False
                    x.store(buf.view(PKMN_SIZE))
                End If
            Next i
        End Sub

    End Class
    Public Class PkmnStorageTD

        Private Const PKMN_NUM As Integer = 550
        Private Const PKMN_SIZE As Integer = 388

        Public pkmns() As TDPkmn

        Public Sub New()
            pkmns = New TDPkmn(PKMN_NUM - 1) {}
            For i As Integer = 0 To PKMN_NUM - 1
                pkmns(i) = New TDPkmn()
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
                    Dim x As New TDPkmn
                    x.isvalid = False
                    x.store(buf.view(PKMN_SIZE))
                End If
            Next i
        End Sub

    End Class
    Public Class PkmnStorageRB

        Private PKMN_NUM As Integer
        Private Const PKMN_SIZE As Integer = 323

        Public pkmns() As RBPkmn

        Public Sub New(PokemonNumber As Integer)
            PKMN_NUM = PokemonNumber
            pkmns = New RBPkmn(PKMN_NUM - 1) {}
            For i As Integer = 0 To PKMN_NUM - 1
                pkmns(i) = New RBPkmn()
            Next i
        End Sub

        Public Sub New(ByVal buf As BooleanBuffer, PokemonNumber As Integer)
            Me.New(PokemonNumber)
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
                    Dim x As New RBPkmn
                    x.isvalid = False
                    x.store(buf.view(PKMN_SIZE))
                End If
            Next i
        End Sub

    End Class

End Namespace