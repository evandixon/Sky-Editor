Imports System.Text
'Yes, I know it's a bad idea to have multiple copies of this class (The other one's in ROMEditor.FileFormats), but this executable is redistributable, so if you know of a better way, please contact me on Github.
''' <summary>
''' 
''' </summary>
Public Class LanguageString
    Implements IDisposable
    Dim _fileReader As IO.FileStream
    Public Enum Region
        US
        Europe
    End Enum

    Public Shared Function ConvertEUToUS(Offset As Integer) As Integer
        If Offset = 3938 OrElse Offset = 3939 Then
            Throw New IndexOutOfRangeException("Indexes 3938 and 3939 do not have a US equivalent.")
        ElseIf Offset < 3938
            Return Offset
        ElseIf 3938 < Offset AndAlso Offset < 17600
            Return Offset - 2
        ElseIf 17600 <= Offset AndAlso Offset < 17812
            Throw New NotImplementedException("Conversion in the Staff Credits is currently not supported.")
        Else 'If   17812<=Offset
            Return Offset - 31
        End If
    End Function
    Public Shared Function ConvertUSToEU(Offset As Integer) As Integer
        If Offset < 3938 Then
            Return Offset
        ElseIf 3938 <= Offset AndAlso Offset < 17598
            Return Offset + 2
        ElseIf 17598 <= Offset AndAlso Offset < 17781
            Throw New NotImplementedException("Conversion in the Staff Credits is currently not supported.")
        Else ' offset <= 17781
            Return Offset + 31
        End If
    End Function

    Public ReadOnly Property FileRegion As Region
        Get
            Return GetFileRegion(Length)
        End Get
    End Property
    Public Shared Function GetFileRegion(Length As Integer) As Region
        If Length = 18451 Then
            Return Region.US
        ElseIf Length = 18482
            Return Region.Europe
        Else
            Return Nothing
        End If
    End Function
    Public Property Items As List(Of String)
    Public Property Filename As String
    Public Property OriginalFilename As String
    Private Property tempPath As String

    Public Sub New()
        MyBase.New()
        Items = New List(Of String)
    End Sub
    Public Sub New(Filename As String)
        Dim tempName = Guid.NewGuid.ToString()
        tempPath = IO.Path.Combine(Environment.CurrentDirectory, "lsp", tempName & ".tmp")
        If Not IO.Directory.Exists(IO.Path.GetDirectoryName(tempPath)) Then
            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(tempPath))
        End If
        Me.OriginalFilename = Filename
        If IO.File.Exists(Filename) Then
            IO.File.Copy(Filename, tempPath)
        Else
            IO.File.WriteAllText(tempPath, "")
        End If
        Me.Filename = tempPath

        Items = New List(Of String)

        Dim offset1 As UInteger = BitConverter.ToUInt32(RawData(0, 4), 0)
        Dim e = Encoding.GetEncoding("Windows-1252")
        'Loop through each entry
        For count = 0 To offset1 - 5 Step 4
            Dim startOffset As UInteger = BitConverter.ToUInt32(RawData(count, 4), 0)
            Items.Add("")
            Dim endOffset As UInteger = startOffset
            Dim s As New StringBuilder
            'Read the null-terminated string
            While RawData(endOffset) <> 0
                s.Append(e.GetString({RawData(endOffset)}))
                endOffset += 1
            End While
            Items(count \ 4) = s.ToString
        Next
    End Sub

    Public Sub PreSave()
        'Generate File
        Dim e = Encoding.GetEncoding("Windows-1252")
        Dim offsets As New List(Of UInt32)
        For i = 0 To Items.Count - 1
            offsets.Add(0)
        Next
        Dim stringdataBytes As New List(Of Byte)
        For count As Integer = 0 To Items.Count - 1
            Dim offset As UInt32 = offsets.Count * 4 + stringdataBytes.Count
            offsets(count) = offset
            Dim strBytes = e.GetBytes(Item(count).Replace(vbCrLf, vbCr))
            For Each s In strBytes
                stringdataBytes.Add(s)
            Next
            stringdataBytes.Add(0)
        Next
        Dim offsetBytes As New List(Of Byte)
        For Each offset In offsets
            Dim t = BitConverter.GetBytes(offset)
            offsetBytes.Add(t(0))
            offsetBytes.Add(t(1))
            offsetBytes.Add(t(2))
            offsetBytes.Add(t(3))
        Next

        Dim totalData As New List(Of Byte)
        For Each b In offsetBytes
            totalData.Add(b)
        Next
        For Each b In stringdataBytes
            totalData.Add(b)
        Next
        'Write buffer to stream
        Length = totalData.Count
        RawData(0, totalData.Count) = totalData.ToArray
    End Sub
    Public Sub Save(Destination As String)
        PreSave()
        If _fileReader Is Nothing Then
            _fileReader = IO.File.Open(Filename, IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite, IO.FileShare.Read)
        End If
        _fileReader.Seek(0, IO.SeekOrigin.Begin)
        _fileReader.Flush()
        If IO.File.Exists(Filename) Then
            IO.File.Copy(Filename, Destination, True)
        Else
            Using dest = IO.File.Open(Destination, IO.FileMode.OpenOrCreate, IO.FileAccess.Write)
                _fileReader.CopyTo(dest)
            End Using
        End If
    End Sub
    Default Public Property Item(Index As UInteger) As String
        Get
            Return Items(Index)
        End Get
        Set(value As String)
            Items(Index) = value
        End Set
    End Property
    Public Property RawData(Index As Long) As Byte
        Get
            If _fileReader Is Nothing Then
                _fileReader = IO.File.Open(Filename, IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite, IO.FileShare.Read)
            End If
            _fileReader.Seek(Index, IO.SeekOrigin.Begin)
            Dim b = _fileReader.ReadByte
            If b > -1 AndAlso b < 256 Then
                Return b
            Else
                Throw New IndexOutOfRangeException("Index " & Index.ToString & " is out of range.  Length of file: " & _fileReader.Length.ToString)
            End If
        End Get
        Set(value As Byte)
            If _fileReader Is Nothing Then
                _fileReader = IO.File.Open(Filename, IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite, IO.FileShare.Read)
            End If
            _fileReader.Seek(Index, IO.SeekOrigin.Begin)
            _fileReader.WriteByte(value)
        End Set
    End Property
    Public Property RawData(Index As Long, Length As Integer) As Byte()
        Get
            Dim output(Length - 1) As Byte
            If _fileReader Is Nothing Then
                _fileReader = IO.File.Open(Filename, IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite, IO.FileShare.Read)
            End If
            _fileReader.Seek(Index, IO.SeekOrigin.Begin)
            _fileReader.Read(output, 0, Length)
            Return output
            'End If
        End Get
        Set(value As Byte())
            If _fileReader Is Nothing Then
                _fileReader = IO.File.Open(Filename, IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite, IO.FileShare.Read)
            End If
            _fileReader.Seek(Index, IO.SeekOrigin.Begin)
            _fileReader.Write(value, 0, Length)
        End Set
    End Property
    Public Property Length As Long
        Get
            If _fileReader Is Nothing Then
                _fileReader = IO.File.Open(Filename, IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite, IO.FileShare.Read)
            End If
            Return _fileReader.Length
        End Get
        Set(value As Long)
            If _fileReader IsNot Nothing Then
                _fileReader.SetLength(value)
            End If
        End Set
    End Property
    Public Const ItemNameStartUS As Integer = 6773

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                _fileReader?.Dispose()
                If IO.File.Exists(tempPath) Then
                    IO.File.Delete(tempPath)
                End If
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class