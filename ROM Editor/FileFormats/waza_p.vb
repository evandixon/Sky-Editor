Imports ROMEditor.Roms
Imports SkyEditorBase
Namespace FileFormats
    Public Class waza_p
        Inherits GenericFile
        Implements Interfaces.iOpenableFile
        Public Property Pokemon As List(Of PokemonMoves)
        Public Class PokemonMoves
            Public Property LevelUpMoves As Dictionary(Of Byte, UInt16)
            Public Property TMMoves As List(Of UInt16)
            Public Property EggMoves As List(Of UInt16)
            Public Sub New()
                LevelUpMoves = New Dictionary(Of Byte, UInt16)
                TMMoves = New List(Of UInt16)
                EggMoves = New List(Of UInt16)
            End Sub
            Public Function Clone() As PokemonMoves
                Dim m As New PokemonMoves
                For Each item In Me.LevelUpMoves
                    m.LevelUpMoves.Add(item.Key, item.Value)
                Next
                For Each item In Me.TMMoves
                    m.TMMoves.Add(item)
                Next
                For Each item In Me.EggMoves
                    m.EggMoves.Add(item)
                Next
                Return m
            End Function
        End Class
        Public Overrides Sub OpenFile(Filename As String) Implements Interfaces.iOpenableFile.OpenFile
            MyBase.OpenFile(Filename)
            Pokemon = New List(Of PokemonMoves)
            Dim index As Integer = &H13
            Dim section As Byte = 0
            Dim m As New PokemonMoves
            While index < Length
                Select Case section
                    Case 0 'Level up
                        If RawData(index) = 0 Then
                            index += 1
                            section += 1
                        Else
                            Dim moveID As UInt16 = 0
                            If RawData(index) > 128 Then
                                Dim part1 = (RawData(index) - 128) << 7
                                Dim part2 = RawData(index + 1)
                                moveID = part1 Or part2
                                index += 2
                            Else
                                moveID = RawData(index)
                                index += 1
                            End If
                            If Not m.LevelUpMoves.ContainsKey(RawData(index)) Then
                                m.LevelUpMoves.Add(RawData(index), moveID)
                                'else the first one wins
                            End If
                            index += 1
                        End If
                    Case 1 'TMs
                        If RawData(index) = 0 Then
                            index += 1
                            section += 1
                        Else
                            Dim moveID As UInt16 = 0
                            If RawData(index) > 128 Then
                                Dim part1 = (RawData(index) - 128) << 7
                                Dim part2 = RawData(index + 1)
                                moveID = part1 Or part2
                                index += 2
                            Else
                                moveID = RawData(index)
                                index += 1
                            End If
                            m.TMMoves.Add(moveID)
                        End If
                    Case 2 'Egg moves
                        If RawData(index) = 0 Then
                            Pokemon.Add(m.Clone)
                            m = New PokemonMoves
                            section = 0
                            index += 1
                            If RawData(index) = 0 Then
                                Exit While
                            End If
                        Else
                            Dim moveID As UInt16 = 0
                            If RawData(index) > 128 Then
                                Dim part1 = (RawData(index) - 128) << 7
                                Dim part2 = RawData(index + 1)
                                moveID = part1 Or part2
                                index += 2
                            Else
                                moveID = RawData(index)
                                index += 1
                            End If
                            m.EggMoves.Add(moveID)
                        End If
                End Select
            End While
        End Sub
    End Class
End Namespace
