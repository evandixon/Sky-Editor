Imports SkyEditorBase
Imports SkyEditorBase.Utilities

Public Class NDSModRegistry
    Private Shared Property Mods As New List(Of Mods.GenericMod)
    Public Shared Sub AddNDSMod(ModType As Type)
        Mods.Add(ModType.GetConstructor({}).Invoke({}))
    End Sub
    Public Shared Function GetMods(GameCode As String) As IEnumerable(Of Type)
        Dim matches As New List(Of Type)
        For Each item In PluginManager.GetInstance.GetRegisteredObjects(GetType(Mods.GenericMod))
            Dim games = item.SupportedGameCodes
            Dim match As Boolean = False
            For Each t In games
                Dim r As New Text.RegularExpressions.Regex(t)
                If r.IsMatch(GameCode) Then
                    matches.Add(item.GetType)
                End If
            Next
        Next
        Return matches
    End Function
    Friend Shared Sub ClearMods()
        Mods.Clear()
        'For Each item In Mods
        '    item.Dispose()
        'Next
    End Sub
End Class
