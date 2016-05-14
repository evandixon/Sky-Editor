Imports SkyEditor.Core
Imports SkyEditor.Core.IO
Imports SkyEditor.ROMEditor

'Public Enum NDSSaveSlot
'    DefaultSlot 'ROM.sav
'    Slot0 'ROM.0.sav
'    Slot1 'ROM.1.sav
'    Slot2 'ROM.2.sav
'    Slot3 'ROM.3.sav
'    Slot4 'ROM.4.sav
'    Slot5 'ROM.5.sav
'    Slot6 'ROM.6.sav
'    Slot7 'ROM.7.sav
'    Slot8 'ROM.8.sav
'    Slot9 'ROM.9.sav
'    DefaultSlotAlt 'ROM.nds.sav
'    Slot0Alt 'ROM.nds.0.sav
'    Slot1Alt 'ROM.nds.1.sav
'    Slot2Alt 'ROM.nds.2.sav
'    Slot3Alt 'ROM.nds.3.sav
'    Slot4Alt 'ROM.nds.4.sav
'    Slot5Alt 'ROM.nds.5.sav
'    Slot6Alt 'ROM.nds.6.sav
'    Slot7Alt 'ROM.nds.7.sav
'    Slot8Alt 'ROM.nds.8.sav
'    Slot9Alt 'ROM.nds.9.sav
'    EZFlash '~/saves/ROM.sav
'End Enum

''' <summary>
''' Represents an NDS Rom that exists on a flashcart, including saves.
''' </summary>
Public Class FlashcartNDSRom
    Public Property ROM As GenericNDSRom
    Protected Property Saves As Dictionary(Of String, Object)
    Public Function GetSaves() As IEnumerable(Of Object)
        Return Saves.Values
    End Function

    Public Shared Async Function GetRom(filename As String, manager As PluginManager) As Task(Of FlashcartNDSRom)
        Dim r As New FlashcartNDSRom
        r.ROM = New GenericNDSRom(filename, True, False, manager.CurrentIOProvider)
        r.Saves = New Dictionary(Of String, Object)

        'Detect saves
        Dim possibleFilenames As New List(Of String)

        'Look at the possible filenames for the ROM's saves
        'form 1: RomName.sav or RomName.X.sav, where X is 0-9
        Dim form1Base As String = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename))
        possibleFilenames.Add(form1Base & ".sav")
        For count = 0 To 9
            possibleFilenames.Add(form1Base & "." & count.ToString & ".sav")
        Next

        'form 2: RomName.nds.sav, or RomName.nds.X.sav, where X is 0-9
        possibleFilenames.Add(form1Base & ".nds.sav")
        For count = 0 To 9
            possibleFilenames.Add(form1Base & ".nds." & count.ToString & ".sav")
        Next
        possibleFilenames.Add(Path.Combine(Path.GetPathRoot(filename), "saves", Path.GetFileNameWithoutExtension(filename) & ".sav"))

        'See if they actually exist.
        For Each item In possibleFilenames
            If manager.CurrentIOProvider.FileExists(item) Then
                'Load the save
                r.Saves.Add(item, Await IOHelper.OpenObject(item, AddressOf IOHelper.PickFirstDuplicateMatchSelector, manager))
            End If
        Next

        Return r
    End Function

    Public Overrides Function ToString() As String
        If ROM IsNot Nothing Then
            Return String.Format("{0} ({1} saves).", ROM.ToString, Saves.Count)
        Else
            Return MyBase.ToString
        End If
    End Function
    Private Sub New()
    End Sub
End Class

