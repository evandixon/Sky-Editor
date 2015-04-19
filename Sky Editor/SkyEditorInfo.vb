Imports System.Windows.Forms
Imports SkyEditor.skyjed.save
Imports SkyEditorBase
Public Class SkyEditorInfo
    Implements iSkyEditorPlugin
    Public ReadOnly Property PluginAuthor As String Implements SkyEditorBase.iSkyEditorPlugin.PluginAuthor
        Get
            Return "evandixon"
        End Get
    End Property

    Public ReadOnly Property PluginName As String Implements SkyEditorBase.iSkyEditorPlugin.PluginName
        Get
            Return "Sky Editor"
        End Get
    End Property

    Public ReadOnly Property Credits As String Implements iSkyEditorPlugin.Credits
        Get
            Return PluginHelper.GetLanguageItem("SkyEditorPlgCredits", "Sky Editor Credits:\n     evandixon (General Research)\n     matix2267 (Pokemon Stucture, code for interacting with bits)\n     Grovyle91 (Item Structure, IDs of Pokemon/Items/etc)\n     Prof. 9 (Team Name character encoding)\n     Demonic722 (Misc RAM and save addresses)")
        End Get
    End Property

    Public Sub Load(ByRef Manager As PluginManager) Implements iSkyEditorPlugin.Load
        PluginHelper.Writeline(SkyEditorBase.PluginHelper.GetResourceName("Root"))

        Manager.RegisterEditorTab(GetType(SkyGeneralTab))
        Manager.RegisterEditorTab(GetType(TDGeneralTab))
        Manager.RegisterEditorTab(GetType(RBGeneral))
        Manager.RegisterEditorTab(GetType(HeldItemsTab))
        Manager.RegisterEditorTab(GetType(EpisodeHeldItems))
        Manager.RegisterEditorTab(GetType(TDSStoredItems))
        Manager.RegisterEditorTab(GetType(RBStoredItemsTab))
        Manager.RegisterEditorTab(GetType(ActivePokemonTab))
        Manager.RegisterEditorTab(GetType(EpisodeActivePokemon))
        Manager.RegisterEditorTab(GetType(RBStoredPokemonTab))
        Manager.RegisterEditorTab(GetType(QuicksavePokemonTab))

        Manager.RegisterSaveTypeDetector(AddressOf DetectSaveType)
        Manager.RegisterIOFilter("*.sav", PluginHelper.GetLanguageItem("Raw Save File"))
        Manager.RegisterIOFilter("*.dsv", PluginHelper.GetLanguageItem("DeSmuMe Save File"))

        Manager.RegisterSaveGameFormat(GameStrings.BlueGame, GameStrings.RBSave, GetType(RBSave))
        Manager.RegisterSaveGameFormat(GameStrings.RedGame, GameStrings.RBSave, GetType(RBSave))
        Manager.RegisterSaveGameFormat(GameStrings.BlueGameEU, GameStrings.RBSaveEU, GetType(RBSaveEU))
        Manager.RegisterSaveGameFormat(GameStrings.RedGameEU, GameStrings.RBSave, GetType(RBSaveEU))
        Manager.RegisterSaveGameFormat(GameStrings.TimeGame, GameStrings.TDSave, GetType(TDSave))
        Manager.RegisterSaveGameFormat(GameStrings.DarknessGame, GameStrings.TDSave, GetType(TDSave))
        Manager.RegisterSaveGameFormat(GameStrings.SkyGame, GameStrings.SkySave, GetType(SkySave))
        Manager.RegisterSaveGameFormat(GameStrings.GateGame, GameStrings.GateSave, GetType(GateSave))

        Manager.RegisterCodeGenerator(New BlueBaseType)
        Manager.RegisterCodeGenerator(New BlueHeldMoney)
        Manager.RegisterCodeGenerator(New BlueRescuePoints)
        Manager.RegisterCodeGenerator(New BlueStoredMoney)
        Manager.RegisterCodeGenerator(New BlueTeamName)

        Manager.RegisterCodeGenerator(New RedBaseType)
        Manager.RegisterCodeGenerator(New RedHeldMoney)
        Manager.RegisterCodeGenerator(New RedRescuePoints)
        Manager.RegisterCodeGenerator(New RedStoredMoney)
        Manager.RegisterCodeGenerator(New RedTeamName)

        Manager.RegisterResourceFile(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", "xceed.wpf.toolkit.dll"))

        Manager.RegisterConsoleCommand("rbloctest", AddressOf GenerateRBLocationTest)
    End Sub

    Public Function DetectSaveType(File As GenericFile) As String
        Select Case File.RawData(&HD)
            Case &H54
                Return GameStrings.TDSave
            Case &H53
                Return GameStrings.SkySave
            Case Else
                If (File.RawData(&H404) = &H50 AndAlso File.RawData(&H405) = &H4F AndAlso File.RawData(&H406) = &H4B AndAlso File.RawData(&H407) = &H45) Then
                    Return GameStrings.RBSave
                Else
                    Return Nothing
                End If
        End Select
    End Function

    Public Sub UnLoad(ByRef Manager As PluginManager) Implements iSkyEditorPlugin.UnLoad

    End Sub

    Public Sub PrepareForDistribution() Implements iSkyEditorPlugin.PrepareForDistribution

    End Sub
    Public Shared Sub GenerateRBLocationTest(Manager As PluginManager, Parameter As String)
        If TypeOf Manager.Save Is RBSave Then
            Dim p1 As RBPkmn = Nothing
            For Each item In DirectCast(Manager.Save, RBSave).StoredPokemon.pkmns
                If item.GetIsValid() Then
                    p1 = item
                    Exit For
                End If
            Next
            If p1 Is Nothing Then
                PluginHelper.Writeline("No existing Pokemon.")
            Else
                Dim pkmns = DirectCast(Manager.Save, RBSave).StoredPokemon.pkmns
                Dim loc As Integer = 0
                For count As Integer = 0 To DirectCast(Manager.Save, RBSave).StoredPokemon.pkmns.Count - 1
                    If loc > 255 Then Exit For
                    If Not pkmns(count).GetIsValid() Then
                        p1.name = "MetAt " & loc.ToString()
                        p1.metat = loc
                        loc += 1
                        pkmns(count) = New RBPkmn(p1)
                    End If
                Next
                Dim temppkm = DirectCast(Manager.Save, RBSave).StoredPokemon
                temppkm.pkmns = pkmns
                DirectCast(Manager.Save, RBSave).StoredPokemon = temppkm
                Dim f As New SaveFileDialog
                f.Filter = "Raw Save Files|*.sav|All Files|*.*"
                If f.ShowDialog = DialogResult.OK Then
                    IO.File.WriteAllBytes(f.FileName, Manager.Save.RawData)
                End If
            End If
        ElseIf TypeOf Manager.Save Is RBSaveEU Then
            Dim p1 As RBPkmn = Nothing
            For Each item In DirectCast(Manager.Save, RBSaveEU).StoredPokemon.pkmns
                If item.GetIsValid() Then
                    p1 = item
                    Exit For
                End If
            Next
            If p1 Is Nothing Then
                PluginHelper.Writeline("No existing Pokemon.")
            Else
                Dim pkmns = DirectCast(Manager.Save, RBSaveEU).StoredPokemon.pkmns
                Dim loc As Integer = 0
                For count As Integer = 0 To DirectCast(Manager.Save, RBSaveEU).StoredPokemon.pkmns.Count - 1
                    If loc > 255 Then Exit For
                    If Not pkmns(count).GetIsValid() Then
                        p1.name = "MetAt " & loc.ToString()
                        p1.metat = loc
                        loc += 1
                        pkmns(count) = New RBPkmn(p1)
                    End If
                Next
                Dim temppkm = DirectCast(Manager.Save, RBSaveEU).StoredPokemon
                temppkm.pkmns = pkmns
                DirectCast(Manager.Save, RBSaveEU).StoredPokemon = temppkm
                Dim f As New SaveFileDialog
                f.Filter = "Raw Save Files|*.sav|All Files|*.*"
                If f.ShowDialog = DialogResult.OK Then
                    IO.File.WriteAllBytes(f.FileName, Manager.Save.RawData)
                End If
            End If
        End If
    End Sub
End Class