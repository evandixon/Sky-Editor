Imports SkyEditor.Core.IO

Namespace Flashcart
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
    ''' Represents an NDSRom that exists on a flashcart, including saves.
    ''' </summary>
    Public Class FlashcartNDSRom
        Public Property ROM As Roms.GenericNDSRom
        Public Property Saves As Dictionary(Of String, Object)
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Filename">The full file path of the NDS ROM.</param>
        <Obsolete("Needs refactoring")> Public Sub New(Filename As String)
            Me.ROM = ROM
            Me.Saves = New Dictionary(Of String, Object)
            'Detect saves
            Dim possibleFilenames As New List(Of String)
            'Look at the possible filenames for the ROM's saves
            Dim form1Base As String = IO.Path.Combine(IO.Path.GetDirectoryName(Filename), IO.Path.GetFileNameWithoutExtension(Filename))
            possibleFilenames.Add(form1Base & ".sav")
            For count = 0 To 9
                possibleFilenames.Add(form1Base & "." & count.ToString & ".sav")
            Next
            possibleFilenames.Add(form1Base & ".nds.sav")
            For count = 0 To 9
                possibleFilenames.Add(form1Base & ".nds." & count.ToString & ".sav")
            Next
            possibleFilenames.Add(IO.Path.Combine(IO.Path.GetPathRoot(Filename), "saves", IO.Path.GetFileNameWithoutExtension(Filename) & ".sav"))
            'See if they actually exist.
            For Each item In possibleFilenames
                If IO.File.Exists(item) Then
                    'Load the save
                    'Todo: properly wait for the async task
                    Saves.Add(item, IOHelper.OpenObject(item, AddressOf IOHelper.PickFirstDuplicateMatchSelector, SkyEditorBase.PluginManager.GetInstance).Result)
                End If
            Next
        End Sub
    End Class
End Namespace

