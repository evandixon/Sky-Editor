Imports System.Collections.Concurrent
Imports System.IO
Imports SkyEditorBase

Namespace Roms
    Public Class GenericNDSRom
        Inherits GenericFile
        Implements SkyEditorBase.Interfaces.iOpenableFile
        Implements SkyEditorBase.Interfaces.iDetectableFileType
        Implements iPackedRom

        Public Overrides Function DefaultExtension() As String
            Return "*.nds"
        End Function

#Region "Constructors"
        Sub New()
            MyBase.New()
            Me.EnableInMemoryLoad = True
        End Sub

        Public Overrides Sub OpenFile(Filename As String) Implements Interfaces.iOpenableFile.OpenFile
            MyBase.OpenFile(Filename)
        End Sub
#End Region


#Region "ROM Stuff"
        Public Async Function RePack(NewFileName As String) As Task Implements iPackedRom.RePack
            Dim romDirectory As String = PluginHelper.GetResourceDirectory
            If Not IO.Directory.Exists(romDirectory) Then
                IO.Directory.CreateDirectory(romDirectory)
            End If
            If Not IO.Directory.Exists(IO.Path.Combine(romDirectory, Name)) Then
                IO.Directory.CreateDirectory(IO.Path.Combine(romDirectory, Name))
            End If
            PluginHelper.Writeline("Repacking ROM...")
            Await PluginHelper.RunProgram(IO.Path.Combine(romDirectory, "ndstool.exe"),
                                                  String.Format("-c ""{0}"" -9 ""{1}/arm9.bin"" -7 ""{1}/arm7.bin"" -y9 ""{1}/y9.bin"" -y7 ""{1}/y7.bin"" -d ""{1}/data"" -y ""{1}/overlay"" -t ""{1}/banner.bin"" -h ""{1}/header.bin""", NewFileName, IO.Path.Combine(romDirectory, Name)))
        End Function

        <Obsolete> Public Overrides Async Sub Save()
            Dim romDirectory As String = PluginHelper.GetResourceDirectory
            Await RePack(romDirectory & "\" & Name & ".nds")
        End Sub
#End Region

#Region "Properties"
        'Credit to http://nocash.emubase.de/gbatek.htm#dscartridgesencryptionfirmware (As of Jan 1 2014) for research
        'Later moved to http://problemkaputt.de/gbatek.htm#dscartridgeheader
        Public Property GameTitle As String
            Get
                Dim e As New System.Text.ASCIIEncoding
                Return e.GetString(RawData(0, 12)).Trim
            End Get
            Set(value As String)
                Dim e As New System.Text.ASCIIEncoding
                Dim buffer = e.GetBytes(value)
                For count = 0 To 11
                    If buffer.Length > count Then
                        RawData(count) = buffer(count)
                    Else
                        RawData(count) = 0
                    End If
                Next
            End Set
        End Property
        Public Property GameCode As String Implements iPackedRom.GameCode
            Get
                Dim e As New System.Text.ASCIIEncoding
                Return e.GetString(RawData(12, 4)).Trim
            End Get
            Set(value As String)
                Dim e As New System.Text.ASCIIEncoding
                Dim buffer = e.GetBytes(value)
                For count = 0 To 3
                    If buffer.Length > count Then
                        RawData(12 + count) = buffer(count)
                    Else
                        RawData(12 + count) = 0
                    End If
                Next
            End Set
        End Property
        Private Property MakerCode As String
            Get
                Dim e As New System.Text.ASCIIEncoding
                Return e.GetString(RawData(16, 2)).Trim
            End Get
            Set(value As String)
                Dim e As New System.Text.ASCIIEncoding
                Dim buffer = e.GetBytes(value)
                For count = 0 To 1
                    If buffer.Length > count Then
                        RawData(16 + count) = buffer(count)
                    Else
                        RawData(16 + count) = 0
                    End If
                Next
            End Set
        End Property
        Private Property UnitCode As Byte
            Get
                Dim e As New System.Text.ASCIIEncoding
                Return RawData(&H12)
            End Get
            Set(value As Byte)
                RawData(&H12) = value
            End Set
        End Property
        Private Property EncryptionSeedSelect As Byte
            Get
                Dim e As New System.Text.ASCIIEncoding
                Return RawData(&H13)
            End Get
            Set(value As Byte)
                RawData(&H13) = value
            End Set
        End Property
        ''' <summary>
        ''' Gets or sets the capacity of the cartridge.  Cartridge size = 128KB * 2 ^ (DeviceCapacity)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property DeviceCapacity As Byte
            Get
                Dim e As New System.Text.ASCIIEncoding
                Return RawData(&H13)
            End Get
            Set(value As Byte)
                RawData(&H13) = value
            End Set
        End Property
        'Reserved: 9 bytes of 0
        Public Property RomVersion As Byte
            Get
                Dim e As New System.Text.ASCIIEncoding
                Return RawData(&H1E)
            End Get
            Set(value As Byte)
                RawData(&H1E) = value
            End Set
        End Property
        'Autostart: bit 2 skips menu
        Private Property Arm9RomOffset As Integer
            Get
                Return BitConverter.ToInt32(RawData(&H20, 4), 0)
            End Get
            Set(value As Integer)
                RawData(&H20, 4) = BitConverter.GetBytes(value)
            End Set
        End Property
        Private Property Arm9REntryAddress As Integer
            Get
                Return BitConverter.ToInt32(RawData(&H24, 4), 0)
            End Get
            Set(value As Integer)
                RawData(&H24, 4) = BitConverter.GetBytes(value)
            End Set
        End Property
        Private Property Arm9RamAddress As Integer
            Get
                Return BitConverter.ToInt32(RawData(&H28, 4), 0)
            End Get
            Set(value As Integer)
                RawData(&H28, 4) = BitConverter.GetBytes(value)
            End Set
        End Property
        Private Property Arm9Size As Integer
            Get
                Return BitConverter.ToInt32(RawData(&H2C, 4), 0)
            End Get
            Set(value As Integer)
                RawData(&H2C, 4) = BitConverter.GetBytes(value)
            End Set
        End Property
        Private Property Arm7RomOffset As Integer
            Get
                Return BitConverter.ToInt32(RawData(&H30, 4), 0)
            End Get
            Set(value As Integer)
                RawData(&H30, 4) = BitConverter.GetBytes(value)
            End Set
        End Property
        Private Property Arm7REntryAddress As Integer
            Get
                Return BitConverter.ToInt32(RawData(&H34, 4), 0)
            End Get
            Set(value As Integer)
                RawData(&H34, 4) = BitConverter.GetBytes(value)
            End Set
        End Property
        Private Property Arm7RamAddress As Integer
            Get
                Return BitConverter.ToInt32(RawData(&H38, 4), 0)
            End Get
            Set(value As Integer)
                RawData(&H38, 4) = BitConverter.GetBytes(value)
            End Set
        End Property
        Private Property Arm7Size As Integer
            Get
                Return BitConverter.ToInt32(RawData(&H3C, 4), 0)
            End Get
            Set(value As Integer)
                RawData(&H3C, 4) = BitConverter.GetBytes(value)
            End Set
        End Property
        Private Property FilenameTableOffset As Integer
            Get
                Return BitConverter.ToInt32(RawData(&H40, 4), 0)
            End Get
            Set(value As Integer)
                RawData(&H40, 4) = BitConverter.GetBytes(value)
            End Set
        End Property
        Private Property FilenameTableSize As Integer
            Get
                Return BitConverter.ToInt32(RawData(&H44, 4), 0)
            End Get
            Set(value As Integer)
                RawData(&H44, 4) = BitConverter.GetBytes(value)
            End Set
        End Property
        Private Property FileAllocationTableOffset As Integer
            Get
                Return BitConverter.ToInt32(RawData(&H48, 4), 0)
            End Get
            Set(value As Integer)
                RawData(&H48, 4) = BitConverter.GetBytes(value)
            End Set
        End Property
        Private Property FileAllocationTableSize As Integer
            Get
                Return BitConverter.ToInt32(RawData(&H4C, 4), 0)
            End Get
            Set(value As Integer)
                RawData(&H4C, 4) = BitConverter.GetBytes(value)
            End Set
        End Property
        Private Property FileArm9OverlayOffset As Integer
            Get
                Return BitConverter.ToInt32(RawData(&H50, 4), 0)
            End Get
            Set(value As Integer)
                RawData(&H50, 4) = BitConverter.GetBytes(value)
            End Set
        End Property
        Private Property FileArm9OverlaySize As Integer
            Get
                Return BitConverter.ToInt32(RawData(&H54, 4), 0)
            End Get
            Set(value As Integer)
                RawData(&H54, 4) = BitConverter.GetBytes(value)
            End Set
        End Property
        Private Property FileArm7OverlayOffset As Integer
            Get
                Return BitConverter.ToInt32(RawData(&H58, 4), 0)
            End Get
            Set(value As Integer)
                RawData(&H58, 4) = BitConverter.GetBytes(value)
            End Set
        End Property
        Private Property FileArm7OverlaySize As Integer
            Get
                Return BitConverter.ToInt32(RawData(&H5C, 4), 0)
            End Get
            Set(value As Integer)
                RawData(&H5C, 4) = BitConverter.GetBytes(value)
            End Set
        End Property
        '060h    4     Port 40001A4h setting for normal commands (usually 00586000h)
        '064h    4     Port 40001A4h setting for KEY1 commands   (usually 001808F8h)
        Private Property IconTitleOffset As Integer
            Get
                Return BitConverter.ToInt32(RawData(&H68, 4), 0)
            End Get
            Set(value As Integer)
                RawData(&H68, 4) = BitConverter.GetBytes(value)
            End Set
        End Property
        '06Ch    2     Secure Area Checksum, CRC-16 of [ [20h]..7FFFh]
        '06Eh    2     Secure Area Loading Timeout (usually 051Eh)
        '070h    4     ARM9 Auto Load List RAM Address (?)
        '074h    4     ARM7 Auto Load List RAM Address (?)
        '078h    8     Secure Area Disable (by encrypted "NmMdOnly") (usually zero)
        '080h    4     Total Used ROM size (remaining/unused bytes usually FFh-padded)
        '084h    4     ROM Header Size (4000h)
        '088h    38h   Reserved (zero filled)
        '0C0h    9Ch   Nintendo Logo (compressed bitmap, same as in GBA Headers)
        '15Ch    2     Nintendo Logo Checksum, CRC-16 of [0C0h-15Bh], fixed CF56h
        '15Eh    2     Header Checksum, CRC-16 of [000h-15Dh]
        '160h    4     Debug rom_offset   (0=none) (8000h and up)       ;only if debug
        '164h    4     Debug size         (0=none) (max 3BFE00h)        ;version with
        '168h    4     Debug ram_address  (0=none) (2400000h..27BFE00h) ;SIO and 8MB
        '16Ch    4     Reserved (zero filled) (transferred, and stored, but not used)
        '170h    90h   Reserved (zero filled) (transferred, but not stored in RAM)
#End Region

#Region "Overlay"
        Private Class OverlayTable
            Public Property OverlayID As Integer
            Public Property RamAddress As Integer 'Point at which to load
            Public Property RamSize As Integer 'mount to load
            Public Property BssSize As Integer 'Size of BSS data region
            Public Property StaticInitStartAddress As Integer
            Public Property StaticInitEndAddress As Integer
            Public Property FileID As Integer
            Public Property Reserved0 As Integer
        End Class
#End Region

#Region "NitroRom Stuff"
        Private Class FileAllocationEntry
            Public Property Offset As Integer
            Public Property EndAddress As Integer
            Public Sub New(Offset As Integer, EndAddress As Integer)
                Me.Offset = Offset
                Me.EndAddress = EndAddress
            End Sub
        End Class

        Private Class DirectoryMainTable
            Public Property SubTableOffset As Integer
            Public Property FirstSubTableFileID As UInt16
            ''' <summary>
            ''' If this is the root directory, will contain the number of child directories.
            ''' Otherwise, the ID of the parent directory.
            ''' </summary>
            ''' <returns></returns>
            Public Property ParentDir As UInt16
            Public Sub New(RawData As Byte())
                SubTableOffset = BitConverter.ToUInt32(RawData, 0)
                FirstSubTableFileID = BitConverter.ToUInt16(RawData, 4)
                ParentDir = BitConverter.ToUInt16(RawData, 6)
            End Sub
        End Class

        Private Class FNTSubTable
            Public Property Length As Byte
            Public Property Name As String
            Public Property SubDirectoryID As UInt16 'Only for directories
            Public Property ParentFileID As UInt16
        End Class

        Private Class FilenameTable
            Public Property Name As String
            Public Property FileIndex As Integer
            Public ReadOnly Property IsDirectory As Boolean
                Get
                    Return FileIndex < 0
                End Get
            End Property
            Public Property Children As List(Of FilenameTable)
            Public Overrides Function ToString() As String
                Return Name
            End Function
            Public Sub New()
                FileIndex = -1
                Children = New List(Of FilenameTable)
            End Sub
        End Class

        Private Function GetFAT() As List(Of FileAllocationEntry)
            Dim out As New List(Of FileAllocationEntry)
            For count = FileAllocationTableOffset To FileAllocationTableOffset + FileAllocationTableSize - 1 Step 8
                Dim entry As New FileAllocationEntry(BitConverter.ToUInt32(RawData(count, 4), 0), BitConverter.ToUInt32(RawData(count + 4, 4), 0))
                If Not entry.Offset = 0 Then
                    out.Add(entry)
                End If
            Next
            Return out
        End Function

        Private Function GetFNT() As FilenameTable
            Dim root As New DirectoryMainTable(RawData(Me.FilenameTableOffset, 8))
            Dim rootDirectories As New List(Of DirectoryMainTable)
            'In the root entry, ParentDir means number of directories
            For count = 8 To root.SubTableOffset - 1 Step 8
                rootDirectories.Add(New DirectoryMainTable(RawData(Me.FilenameTableOffset + count, 8)))
            Next
            'Todo: read the relationship between directories and files
            Dim out As New FilenameTable
            out.Name = "Data"
            BuildFNT(out, root, rootDirectories)
            Return out
        End Function
        Private Sub BuildFNT(ParentFNT As FilenameTable, root As DirectoryMainTable, Directories As List(Of DirectoryMainTable))
            For Each item In ReadFNTSubTable(root.SubTableOffset, root.FirstSubTableFileID)
                Dim child As New FilenameTable With {.Name = item.Name}
                ParentFNT.Children.Add(child)
                If item.Length > 128 Then
                    BuildFNT(child, Directories((item.SubDirectoryID And &HFFF) - 1), Directories)
                Else
                    child.FileIndex = item.ParentFileID
                End If
            Next
        End Sub
        Private Function ReadFNTSubTable(RootSubTableOffset As Integer, ByVal ParentFileID As Integer) As List(Of FNTSubTable)
            Dim subTables As New List(Of FNTSubTable)
            Dim offset = RootSubTableOffset + Me.FilenameTableOffset
            Dim length As Integer = RawData(offset)
            While length > 0
                If length > 128 Then
                    'Then it's a sub directory
                    'Read the string
                    Dim buffer As Byte() = RawData(offset + 1, length - 128)
                    Dim s = Text.Encoding.ASCII.GetString(buffer)
                    'Read sub directory ID
                    Dim subDirID As UInt16 = Me.UInt16(offset + 1 + length - 128)
                    'Add the result to the list
                    subTables.Add(New FNTSubTable With {.Length = length, .Name = s, .SubDirectoryID = subDirID})
                    'Increment the offset
                    offset += length - 128 + 1 + 2
                ElseIf length < 128 Then
                    'Then it's a file
                    'Read the string
                    Dim buffer As Byte() = RawData(offset + 1, length)
                    Dim s = Text.Encoding.ASCII.GetString(buffer)
                    'Add the result to the list
                    subTables.Add(New FNTSubTable With {.Length = length, .Name = s, .ParentFileID = ParentFileID})
                    ParentFileID += 1
                    'Increment the offset
                    offset += length + 1
                Else
                    'Reserved.  I'm not sure what to do here.
                    Throw New NotSupportedException("Subtable length of 0x80 not supported.")
                End If

                length = RawData(offset)
            End While
            Return subTables
        End Function
        ''' <summary>
        ''' Extracts the files contained within the ROMs.
        ''' Extractions either run synchronously or asynchrounously, depending on the value of IsThreadSafe.
        ''' </summary>
        ''' <param name="TargetDir">Directory to store the extracted files.</param>
        ''' <returns></returns>
        <Obsolete("Incomplete.")> Public Async Function Unpack(TargetDir As String) As Task
            Dim fat = GetFAT()

            'Set up extraction dependencies
            CurrentExtractProgress = 0
            CurrentExtractMax = fat.Count
            SetLoadingExtractProgress = True
            ExtractionTasks = New ConcurrentBag(Of Task)

            'Ensure directory exists
            If Not IO.Directory.Exists(TargetDir) Then
                IO.Directory.CreateDirectory(TargetDir)
            End If

            'Start extracting
            '-Header
            Dim headerTask = Task.Run(New Action(Sub()
                                                     IO.File.WriteAllBytes(IO.Path.Combine(TargetDir, "header.bin"), RawData(0, &H200))
                                                 End Sub))
            If Me.IsThreadSafe Then
                ExtractionTasks.Add(headerTask)
            Else
                Await headerTask
            End If

            '-Arm9
            Dim arm9Task = Task.Run(New Action(Sub()
                                                   IO.File.WriteAllBytes(IO.Path.Combine(TargetDir, "arm9.bin"), RawData(Me.Arm9RomOffset, Me.Arm9Size))
                                               End Sub))
            If Me.IsThreadSafe Then
                ExtractionTasks.Add(arm9Task)
            Else
                Await arm9Task
            End If

            '-Arm7
            Dim arm7Task = Task.Run(New Action(Sub()
                                                   IO.File.WriteAllBytes(IO.Path.Combine(TargetDir, "arm7.bin"), RawData(Me.Arm7RomOffset, Me.Arm7Size))
                                               End Sub))
            If Me.IsThreadSafe Then
                ExtractionTasks.Add(arm7Task)
            Else
                Await arm7Task
            End If

            'Todo: extract arm9 overlay table (y9.bin)
            'Todo: extract arm7 overlay table (y7.bin)
            'Todo: extract overlays
            'Todo: extract banner.bin
            Await StartExtractFiles(fat, GetFNT, TargetDir)
            'Wait for everything to finish
            Await Task.WhenAll(ExtractionTasks)
            PluginHelper.SetLoadingStatusFinished()
        End Function
        <Obsolete> Public Async Function UnpackWithNDSTool(TargetDir As String) As Task Implements iPackedRom.Unpack
            Dim romDirectory As String
            If TargetDir Is Nothing Then
                romDirectory = IO.Path.Combine(PluginHelper.GetResourceDirectory, Name)
            Else
                romDirectory = TargetDir
            End If

            If IO.Directory.Exists(romDirectory) Then
                Try
                    IO.Directory.Delete(romDirectory, True)
                Catch ex As IOException
                    PluginHelper.Writeline(ex.ToString)
                End Try
            End If

            IO.Directory.CreateDirectory(romDirectory)
            Await PluginHelper.RunProgram(PluginHelper.GetResourceName("ndstool.exe"),
                                                  String.Format("-v -x ""{0}"" -9 ""{1}/arm9.bin"" -7 ""{1}/arm7.bin"" -y9 ""{1}/y9.bin"" -y7 ""{1}/y7.bin"" -d ""{1}/data"" -y ""{1}/overlay"" -t ""{1}/banner.bin"" -h ""{1}/header.bin""", Filename, romDirectory))
        End Function
        ''' <summary>
        ''' Queues file extraction tasks if the file is thread safe, otherwise, extracts files one at a time.
        ''' </summary>
        ''' <param name="FAT"></param>
        ''' <param name="Root"></param>
        ''' <param name="TargetDir"></param>
        ''' <returns></returns>
        Private Async Function StartExtractFiles(FAT As List(Of FileAllocationEntry), Root As FilenameTable, TargetDir As String) As Task
            Dim dest As String = IO.Path.Combine(TargetDir, Root.Name)
            Dim f As New SkyEditorBase.Utilities.AsyncFor
            Dim task = (f.RunForEach(Async Function(Item As FilenameTable) As Task
                                         If Item.IsDirectory Then
                                             Await StartExtractFiles(FAT, Item, dest)
                                         Else
                                             Dim entry = FAT(Item.FileIndex)
                                             Dim parentDir = IO.Path.GetDirectoryName(IO.Path.Combine(dest, Item.Name))
                                             If Not IO.Directory.Exists(parentDir) Then
                                                 IO.Directory.CreateDirectory(parentDir)
                                             End If
                                             IO.File.WriteAllBytes(IO.Path.Combine(dest, Item.Name), RawData(entry.Offset, entry.EndAddress - entry.Offset))
                                             Console.WriteLine("Extracted " & IO.Path.Combine(dest, Item.Name))
                                             System.Threading.Interlocked.Increment(CurrentExtractProgress)
                                         End If
                                     End Function, Root.Children, Integer.MaxValue, Not Me.IsThreadSafe))
            If Me.IsThreadSafe Then
                'Then let it keep running while we go and add more
                ExtractionTasks.Add(task)
            Else
                'If it's not thread safe, then we must wait until it's finished, in order to avoid weirdness.
                Await task
            End If
        End Function
        Private Property CurrentExtractProgress As Integer
            Get
                Return _extractProgress
            End Get
            Set(value As Integer)
                _extractProgress = value
                If SetLoadingExtractProgress Then
                    PluginHelper.SetLoadingStatus(String.Format(My.Resources.Language.LoadingExtractingNDSRomXofY, value, CurrentExtractMax), GetExtractionProgress)
                End If
            End Set
        End Property
        Dim _extractProgress As Integer
        Private Property CurrentExtractMax As Integer
        Private Property SetLoadingExtractProgress As Boolean
        Private Property ExtractionTasks As ConcurrentBag(Of Task)
        Public Function GetExtractionProgress() As Single
            Return CurrentExtractProgress / CurrentExtractMax
        End Function
#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overrides Sub Dispose(disposing As Boolean)
            MyBase.Dispose(disposing)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If ExtractionTasks IsNot Nothing Then
                        For Each item In ExtractionTasks
                            If item.Status = TaskStatus.RanToCompletion OrElse item.Status = TaskStatus.Faulted OrElse item.Status = TaskStatus.Canceled Then
                                item.Dispose()
                            End If
                        Next
                    End If
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub
#End Region

        Public Overridable Function IsFileOfType(File As GenericFile) As Boolean Implements Interfaces.iDetectableFileType.IsOfType
            Return (File.Length > &H15D AndAlso File.RawData(&H15C) = &H56 AndAlso File.RawData(&H15D) = &HCF)
        End Function

    End Class
End Namespace
