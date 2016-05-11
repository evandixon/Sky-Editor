﻿Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO

Namespace FileFormats.Explorers.Script
    Public Class SSA
        Implements IOpenableFile
        Implements iOnDisk
        Implements iNamed

        Public Property Filename As String Implements iOnDisk.Filename

        Public ReadOnly Property Name As String Implements iNamed.Name
            Get
                Return IO.Path.GetFileName(Filename)
            End Get
        End Property

        Public Async Function OpenFile(Filename As String, Provider As IOProvider) As Task Implements IOpenableFile.OpenFile
            Me.Filename = Filename

            Using f As New GenericFile
                f.IsReadOnly = True
                Await f.OpenFile(Filename, Provider)

                f.Position = 0
                Dim numGroups = f.NextUInt16
                Dim dataOffset = f.NextUInt16 'Length of non-groups/start of groups (Z)
                Dim unkA = f.NextUInt16 'Start of something, only in enter.sse
                Dim pokePos = f.NextUInt16
                Dim objPos = f.NextUInt16
                Dim backPos = f.NextUInt16
                Dim unkE = f.NextUInt16
                Dim movements = f.NextUInt16
                Dim wordsGStart = f.NextUInt16
            End Using
        End Function
    End Class

End Namespace