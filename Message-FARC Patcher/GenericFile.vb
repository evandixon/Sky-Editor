Imports System.Text
Imports Microsoft.VisualBasic.Devices

Public Class GenericFile
    Implements IDisposable
    Private _tempname As String
    Private _tempFilename As String
    Dim _fileReader As IO.Stream

    Public Property IsReadOnly As Boolean
        Get
            Return _openReadOnly
        End Get
        Set(ByVal value As Boolean)
            _openReadOnly = value
        End Set
    End Property
    Dim _openReadOnly As Boolean

    ''' <summary>
    ''' Determines whether or not the file will be loaded into memory completely, or located on disk.
    ''' </summary>
    ''' <returns></returns>
    Public Property EnableInMemoryLoad As Boolean

    Public Property EnableShadowCopy As Boolean
        Get
            If _enableShadowCopy.HasValue Then
                Return _enableShadowCopy
            Else
                Return Not IsReadOnly
            End If
        End Get
        Set(value As Boolean)
            _enableShadowCopy = value
        End Set
    End Property
    Dim _enableShadowCopy As Boolean?

    Public ReadOnly Property IsThreadSafe As Boolean
        Get
            Return InMemoryFile IsNot Nothing
        End Get
    End Property


    Private Property InMemoryFile As Byte()

    Public Sub New()
        _openReadOnly = False
        EnableInMemoryLoad = False 'This is an opt-in setting
        _enableShadowCopy = Nothing
    End Sub


    ''' <summary>
    ''' Creates a new file with the given name.
    ''' </summary>
    ''' <param name="Name">Name (not path) of the file.  Include the extension if applicable.</param>
    Public Overridable Sub CreateFile(Name As String) 'Implements iCreatableFile.CreateFile
        CreateFile(Name, {})
    End Sub

    Public Overridable Sub CreateFile(Name As String, FileContents As Byte())
        'Generate a temporary filename
        _tempname = Guid.NewGuid.ToString()
        _tempFilename = (_tempname & ".tmp")
        'Load the file if applicable
        If EnableInMemoryLoad Then
            Me.InMemoryFile = FileContents
        Else
            IO.File.WriteAllBytes(_tempFilename, FileContents)
            'The file reader will be initialized when it's first needed
        End If
        Me.Filename = _tempFilename
        Me.OriginalFilename = _tempFilename
        Me.Name = Name
    End Sub

    ''' <summary>
    ''' Opens a file from the given filename.
    ''' </summary>
    ''' <param name="Filename"></param>
    Public Overridable Sub OpenFile(Filename As String) ' Implements iOpenableFile.OpenFile
        Dim info As New IO.FileInfo(Filename)
        If (EnableInMemoryLoad AndAlso (New ComputerInfo).AvailablePhysicalMemory > (info.Length + 500 * 1024 * 1024)) Then
            'Load the file into memory if it's enabled and it will fit into RAM, with 500MB left over, just in case.
            Me.OriginalFilename = Filename
            Me.Filename = Filename
            InMemoryFile = IO.File.ReadAllBytes(Filename)
        Else
            'The file will be read from disk.  The only concern is whether or not we want to make a shadow copy.
            If EnableShadowCopy Then
                _tempname = Guid.NewGuid.ToString()
                _tempFilename = (_tempname & ".tmp")
                Me.OriginalFilename = Filename
                If IO.File.Exists(Filename) Then
                    IO.File.Copy(Filename, _tempFilename)
                Else
                    'If the file doesn't exist, we'll create a file.
                    IO.File.WriteAllBytes(_tempFilename, {})
                End If
                Me.Filename = _tempFilename
            Else
                Me.OriginalFilename = Filename
                Me.Filename = Filename
            End If
            'The file stream will be initialized when it's needed.
        End If
    End Sub

    Public Property Filename As String
    Public Property OriginalFilename As String
    Public Property Name As String
        Get
            If _name Is Nothing Then
                Return IO.Path.GetFileName(OriginalFilename)
            Else
                Return _name
            End If
        End Get
        Set(value As String)
            _name = value
        End Set
    End Property
    Dim _name As String = Nothing
    Public Overridable Function DefaultExtension() As String
        Return ""
    End Function

#Region "Properties"
    Protected ReadOnly Property FileReader As IO.Stream
        Get
            If _fileReader Is Nothing Then
                If _openReadOnly Then
                    _fileReader = IO.File.Open(Filename, IO.FileMode.OpenOrCreate, IO.FileAccess.Read, IO.FileShare.ReadWrite)
                Else
                    _fileReader = IO.File.Open(Filename, IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite, IO.FileShare.Read)
                End If
            End If

            Return _fileReader
        End Get
    End Property
    Public Property RawData(Index As Long) As Byte
        Get
            If InMemoryFile IsNot Nothing Then
                Return InMemoryFile(Index)
            Else
                FileReader.Seek(Index, IO.SeekOrigin.Begin)
                Dim b = FileReader.ReadByte
                If b > -1 AndAlso b < 256 Then
                    Return b
                Else
                    Throw New IndexOutOfRangeException("Index " & Index.ToString & " is out of range.  Length of file: " & _fileReader.Length.ToString)
                End If
            End If
        End Get
        Set(value As Byte)
            If InMemoryFile IsNot Nothing Then
                InMemoryFile(Index) = value
            Else
                FileReader.Seek(Index, IO.SeekOrigin.Begin)
                FileReader.WriteByte(value)
            End If
        End Set
    End Property
    Public Property RawData(Index As Long, Length As Long) As Byte()
        Get
            Dim output(Length - 1) As Byte
            If InMemoryFile IsNot Nothing Then
                For i = 0 To Length - 1
                    output(i) = RawData(Index + i)
                Next
            Else
                FileReader.Seek(Index, IO.SeekOrigin.Begin)
                FileReader.Read(output, 0, Length)
            End If
            Return output
        End Get
        Set(value As Byte())
            If InMemoryFile IsNot Nothing Then
                For i = 0 To Length - 1
                    RawData(Index + i) = value(i)
                Next
            Else
                FileReader.Seek(Index, IO.SeekOrigin.Begin)
                FileReader.Write(value, 0, Length)
            End If
        End Set
    End Property
    Public Property RawData() As Byte()
        Get
            If InMemoryFile IsNot Nothing Then
                Return InMemoryFile
            Else
                Return RawData(0, Length)
            End If
        End Get
        Set(value As Byte())
            If InMemoryFile IsNot Nothing Then
                InMemoryFile = value
            Else
                RawData(0, Length) = value
            End If
        End Set
    End Property

    ''' <summary>
    ''' Gets a 16 bit signed little endian int starting at the given index.
    ''' </summary>
    ''' <param name="Index"></param>
    ''' <returns></returns>
    Public Property Int16(Index As Long) As Short
        Get
            Return BitConverter.ToInt16(RawData(Index, 2), 0)
        End Get
        Set(value As Short)
            Dim bytes = BitConverter.GetBytes(value)
            RawData(Index, 2) = bytes
        End Set
    End Property

    ''' <summary>
    ''' Gets a 16 bit unsigned little endian int starting at the given index.
    ''' </summary>
    ''' <param name="Index"></param>
    ''' <returns></returns>
    Public Property UInt16(Index As Long) As UShort
        Get
            Return BitConverter.ToUInt16(RawData(Index, 2), 0)
        End Get
        Set(value As UShort)
            Dim bytes = BitConverter.GetBytes(value)
            RawData(Index, 2) = bytes
        End Set
    End Property

    ''' <summary>
    ''' Gets a 32 bit signed little endian int starting at the given index.
    ''' </summary>
    ''' <param name="Index"></param>
    ''' <returns></returns>
    Public Property Int32(Index As Long) As Integer
        Get
            Return BitConverter.ToInt32(RawData(Index, 4), 0)
        End Get
        Set(value As Integer)
            Dim bytes = BitConverter.GetBytes(value)
            RawData(Index, 4) = bytes
        End Set
    End Property

    ''' <summary>
    ''' Gets a 32 bit unsingned little endian int starting at the given index.
    ''' </summary>
    ''' <param name="Index"></param>
    ''' <returns></returns>
    Public Property UInt32(Index As Long) As UInteger
        Get
            Return BitConverter.ToUInt32(RawData(Index, 4), 0)
        End Get
        Set(value As UInteger)
            Dim bytes = BitConverter.GetBytes(value)
            RawData(Index, 4) = bytes
        End Set
    End Property

    ''' <summary>
    ''' Gets a 64 bit signed little endian int starting at the given index.
    ''' </summary>
    ''' <param name="Index"></param>
    ''' <returns></returns>
    Public Property Int64(Index As Long) As Long
        Get
            Return BitConverter.ToInt64(RawData(Index, 8), 0)
        End Get
        Set(value As Long)
            Dim bytes = BitConverter.GetBytes(value)
            RawData(Index, 8) = bytes
        End Set
    End Property

    ''' <summary>
    ''' Gets a 64 bit unsingned little endian int starting at the given index.
    ''' </summary>
    ''' <param name="Index"></param>
    ''' <returns></returns>
    Public Property UInt64(Index As Long) As ULong
        Get
            Return BitConverter.ToUInt64(RawData(Index, 8), 0)
        End Get
        Set(value As ULong)
            Dim bytes = BitConverter.GetBytes(value)
            RawData(Index, 8) = bytes
        End Set
    End Property

    Public Property Length As Long
        Get
            If InMemoryFile IsNot Nothing Then
                Return InMemoryFile.Length
            Else
                Return FileReader.Length
            End If
        End Get
        Set(value As Long)
            If InMemoryFile IsNot Nothing Then
                ReDim Preserve InMemoryFile(value - 1)
            Else
                FileReader.SetLength(value)
            End If
        End Set
    End Property

    Public Property Position As ULong
    '''' <summary>
    '''' Gets or sets a string representation of the file.
    '''' When setting, will overwrite all data in the file.
    '''' </summary>
    '''' <returns></returns>
    '<Obsolete("Awkward code.  Will drastically change in the future.")> Protected Property RawText As String
    '    Get
    '        Return IO.File.ReadAllText(Filename)
    '    End Get
    '    Set(value As String)
    '        Dim buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(value)
    '        If _fileReader Is Nothing Then
    '            _fileReader = IO.File.Open(Filename, IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite, IO.FileShare.Read)
    '        End If
    '        _fileReader.SetLength(buffer.Length)
    '        _fileReader.Seek(0, IO.SeekOrigin.Begin)
    '        _fileReader.Write(buffer, 0, buffer.Count)
    '    End Set
    'End Property
#End Region

#Region "Events"
    Public Event FileModified(sender As Object, e As EventArgs)
    Public Event FileSaved(sender As Object, e As EventArgs)

    ''' <summary>
    ''' Raises the FileModified event, marking the file as having been modified.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub RaiseFileModified(sender As Object, e As EventArgs)
        RaiseEvent FileModified(sender, e)
    End Sub
    Protected Sub RaiseFileSaved(sender As Object, e As EventArgs)
        RaiseEvent FileSaved(sender, e)
    End Sub
    Public Sub RaiseModified()
        RaiseFileModified(Me, New EventArgs)
    End Sub

#End Region

#Region "Functions"
    Protected Overridable Sub PreSave()

    End Sub
    Public Overridable Sub Save(Destination As String)
        PreSave()
        If InMemoryFile IsNot Nothing Then
            IO.File.WriteAllBytes(Destination, InMemoryFile)
        Else
            FileReader.Seek(0, IO.SeekOrigin.Begin)
            FileReader.Flush()
            If Not String.IsNullOrEmpty(Destination) Then
                If IO.File.Exists(Filename) Then
                    IO.File.Copy(Filename, Destination, True)
                Else
                    Using dest = IO.File.Open(Destination, IO.FileMode.OpenOrCreate, IO.FileAccess.Write)
                        FileReader.CopyTo(dest)
                    End Using
                End If
            End If
        End If

        If String.IsNullOrEmpty(OriginalFilename) Then
            OriginalFilename = Destination
        End If
        RaiseEvent FileSaved(Me, New EventArgs)
    End Sub
    Public Overridable Sub Save()
        Save(Me.OriginalFilename)
        'PreSave()
        'FileReader.Seek(0, IO.SeekOrigin.Begin)
        'FileReader.Flush()
        'If Not Filename = OriginalFilename AndAlso Not String.IsNullOrEmpty(OriginalFilename) Then
        '    IO.File.Copy(Filename, OriginalFilename, True)
        'End If
        'RaiseEvent FileSaved(Me, New EventArgs)
    End Sub

    ''' <summary>
    ''' Reads a unicode string from the file.
    ''' </summary>
    ''' <param name="Offset">Location of the string in the file.</param>
    ''' <param name="Length">Length, in characters, of the string.</param>
    ''' <returns></returns>
    Public Function ReadUnicodeString(Offset As Integer, Length As Integer) As String
        Dim u = Text.Encoding.Unicode
        Return u.GetString(RawData(Offset, Length * 2))
    End Function

    ''' <summary>
    ''' Reads a null-terminated string from the file.
    ''' </summary>
    ''' <param name="Offset">Location of the string in the file.</param>
    ''' <returns></returns>
    Public Function ReadUnicodeString(Offset As Integer) As String
        'Parse the null-terminated UTF-16 string
        Dim s As New StringBuilder
        Dim e = Text.Encoding.Unicode
        Dim j As Integer = 0
        Dim cRaw As Byte()
        Dim c As String
        Do
            cRaw = RawData(Offset + j * 2, 2)
            c = e.GetString(cRaw)

            If Not c = vbNullChar Then
                s.Append(c)
            End If

            j += 1
        Loop Until c = vbNullChar
        Return s.ToString
    End Function

    Public Function ReadNullTerminatedString(Offset As Integer, e As Text.Encoding) As String
        Dim out As New Text.StringBuilder
        Dim pos = Offset
        Dim c As Byte
        Do
            c = RawData(pos)
            If Not c = 0 Then
                out.Append(e.GetString({c}))
            End If
            pos += 1
        Loop Until c = 0
        Return out.ToString
    End Function

    ''' <summary>
    ''' Reads an unsigned 16 bit integer at the current position, and increments the current position by 2.
    ''' </summary>
    ''' <returns></returns>
    Public Function NextUInt16() As UInt16
        Dim out = UInt16(Position)
        Position += 2
        Return out
    End Function
#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If _fileReader IsNot Nothing Then
                    _fileReader.Dispose()
                    _fileReader = Nothing
                End If
                If IO.File.Exists(_tempFilename) Then IO.File.Delete(_tempFilename)
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class