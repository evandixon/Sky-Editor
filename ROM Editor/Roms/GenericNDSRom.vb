Imports System.IO
Imports SkyEditorBase

Namespace Roms
    Public Class GenericNDSRom
        Inherits GenericFile
        Implements SkyEditorBase.Interfaces.iOpenableFile
        Implements iPackedRom

#Region "Constructors"
        Sub New()
            MyBase.New()
            _unpackTask = New Task(Of Boolean)(Function()
                                                   Return True
                                               End Function)
            _unpackTask.Start()
        End Sub

        Sub New(Filename As String)
            MyBase.New(Filename, True)
            _unpackTask = Nothing
        End Sub
        Public Overrides Sub OpenFile(Filename As String) Implements Interfaces.iOpenableFile.OpenFile
            MyBase.OpenFile(Filename)
            _unpackTask = Nothing
        End Sub
#End Region

#Region "GenericSave Stuff"
        'Public Overrides Function DefaultSaveID() As String
        '    Return GameStrings.GenericNDSRom
        'End Function
        Public Overrides Function DefaultExtension() As String
            Return "*.nds"
        End Function
#End Region

#Region "ROM Stuff"
        Private _unpackTask As Task(Of Boolean)
        Public ReadOnly Property IsUnpacked
            Get
                Return _unpackTask IsNot Nothing AndAlso _unpackTask.IsCompleted
            End Get
        End Property
        Public ReadOnly Property IsUnpacking
            Get
                Return _unpackTask IsNot Nothing
            End Get
        End Property
        Public Async Function EnsureUnpacked() As Task(Of Boolean)
            If Not IsUnpacked Then
                If Not IsUnpacking Then
                    _unpackTask = Unpack()
                End If
                Return Await _unpackTask
            Else
                Return True
            End If
        End Function
        Public Async Function Unpack(Optional DestinationDirectory As String = Nothing) As Task Implements iPackedRom.Unpack
            If IsUnpacking Then
                PluginHelper.Writeline("Something failed, unpack called when currently unpacking.")
            End If
            Dim romDirectory As String
            If DestinationDirectory Is Nothing Then
                romDirectory = IO.Path.Combine(PluginHelper.GetResourceDirectory, Name)
            Else
                romDirectory = DestinationDirectory
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
        ''' <summary>
        ''' Repacks the rom and runs it.  Running only works if .nds files are associated with an emulator.
        ''' </summary>
        ''' <remarks></remarks>
        Public Async Function RunRom() As Task
            Dim romDirectory As String = PluginHelper.GetResourceDirectory
            Await RePack(romDirectory & "\" & Name & ".nds")
            PluginHelper.Writeline("Running ROM...")
            Process.Start(romDirectory & Name & ".nds")
        End Function
        Public Overrides Async Sub Save()
            Dim romDirectory As String = PluginHelper.GetResourceDirectory
            Await RePack(romDirectory & "\" & Name & ".nds")
        End Sub
#End Region

#Region "Properties"
        'Credit to http://nocash.emubase.de/gbatek.htm#dscartridgesencryptionfirmware (As of Jan 1 2014) for research
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
        Public Property GameCode As String
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

#Region "NitroRom Stuff"
        Private Class FileAllocationEntry
            Public Property Offset As Integer
            Public Property EndAddress As Integer
            Public Sub New(Offset As Integer, EndAddress As Integer)
                Me.Offset = Offset
                Me.EndAddress = EndAddress
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

#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overrides Sub Dispose(disposing As Boolean)
            MyBase.Dispose(disposing)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    If _unpackTask IsNot Nothing Then _unpackTask.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub
#End Region

        Public Shared Function IsFileOfType(File As GenericFile) As Boolean
            Return (File.Length > &H15D AndAlso File.RawData(&H15C) = &H56 AndAlso File.RawData(&H15D) = &HCF)
        End Function

    End Class
End Namespace
