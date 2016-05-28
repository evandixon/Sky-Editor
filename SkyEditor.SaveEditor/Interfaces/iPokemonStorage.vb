Namespace Interfaces
    Public Class StoredPokemonSlotDefinition
        Public Property Index As Integer
        Public Property Name As String
        Public Property Length As Integer
        Public Property CurrentPokemon As Integer
        Public Sub New(Index As Integer, AreaName As String, Length As Integer)
            Me.Index = Index
            Me.Name = AreaName
            Me.Length = Length
            Me.CurrentPokemon = 0
        End Sub
        Public Shared Function FromLine(Line As String, Index As Integer) As StoredPokemonSlotDefinition
            Dim parts = Line.Split(":")
            Return New StoredPokemonSlotDefinition(Index, parts(0).Trim, parts(1).Trim)
        End Function
        Public Shared Function FromLines(Lines As String) As List(Of StoredPokemonSlotDefinition)
            Dim out As New List(Of StoredPokemonSlotDefinition)
            Dim offset As Integer = 0
            For Each Line In Lines.Split(vbCrLf)
                If Not Line.Trim.StartsWith("#") Then
                    out.Add(FromLine(Line.Trim, offset))
                    offset += out.Last.Length
                End If
            Next
            Return out
        End Function
        Public Overrides Function ToString() As String
            Return Name & " (" & CurrentPokemon & "/" & Length & ")"
        End Function
    End Class
    Public Interface iPokemonStorageOld
        Function GetPokemon() As iMDPkm()
        Sub SetPokemon(Pokemon As iMDPkm())

        Function GetStoredPokemonOffsets() As StoredPokemonSlotDefinition()
    End Interface

End Namespace