Imports SkyEditorBase
Imports SkyEditorBase.Utilities

Public Class NDSModRegistry
    Private Shared Property Mods As New List(Of Mods.GenericMod)
    Public Shared Sub AddNDSMod(ModType As Type)
        Mods.Add(ModType.GetConstructor({}).Invoke({}))
    End Sub
    Public Shared Function GetMods(GameType As Type) As IEnumerable(Of Type)
        Dim matches As New List(Of Type)
        For Each item In Mods
            Dim games = item.SupportedGameTypes
            Dim match As Boolean = False
            For Each t In games
                If games.Count = 0 OrElse ReflectionHelpers.IsOfType(GameType, t) Then
                    matches.Add(item.GetType)
                End If
            Next
        Next
        Return matches
    End Function
    Public Shared Function GetMods(GameCode As String) As IEnumerable(Of Type)
        If GameCodeRegistry.GameTypes.ContainsKey(GameCode) Then
            Dim t = GameCodeRegistry.GameTypes(GameCode)
            Return GetMods(t)
        Else
            Return GetMods(GetType(Roms.iPackedRom))
        End If
    End Function
    Friend Shared Sub ClearMods()
        Mods.Clear()
        'For Each item In Mods
        '    item.Dispose()
        'Next
    End Sub
End Class
