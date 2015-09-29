Public Class NDSModRegistry
    Private Shared Property Mods As New List(Of Mods.GenericNDSMod)
    Public Shared Sub AddNDSMod(ModType As Type)
        Mods.Add(ModType.GetConstructor({}).Invoke({}))
    End Sub
    Public Shared Function GetMods(GameCode As String) As IEnumerable(Of Type)
        Dim matches As New List(Of Type)
        For Each item In Mods
            Dim games = item.SupportedGameCodes
            If games.Count = 0 OrElse games.Contains(GameCode) Then
                matches.Add(item.GetType)
            End If
        Next
        Return matches
    End Function
    Friend Shared Sub ClearMods()
        For Each item In Mods
            item.Dispose()
        Next
    End Sub
End Class
