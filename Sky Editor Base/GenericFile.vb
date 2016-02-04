Imports System.Text
Imports SkyEditorBase.Interfaces
Public Class GenericFile
    Implements IDisposable
    Implements iNamed
    Implements iModifiable
    'Implements iCreatableFile 'Excluded because this might not apply to children
    'Implements iOpenableFile
    Implements iOnDisk
    Implements ISavableAs
    Private _tempname As String
    Private _tempFilename As String
    Dim _fileReader As IO.FileStream
    Dim _makeTempCopy As Boolean
    Dim _openReadOnly As Boolean

#Region "Constructors"
    <CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")>
    Public Sub New(Filename As String)
        _makeTempCopy = True
        _openReadOnly = False
        OpenFile(Filename)
    End Sub
    <CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")>
    Public Sub New(Filename As String, OpenReadOnly As Boolean)
        _makeTempCopy = Not OpenReadOnly
        _openReadOnly = OpenReadOnly
        OpenFile(Filename)
    End Sub
    Public Sub New()
        _makeTempCopy = True
        _openReadOnly = False
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="MakeTempCopy">Whether or not to make a background copy of a file when opening a file.</param>
    Public Sub New(MakeTempCopy As Boolean)
        _makeTempCopy = MakeTempCopy
    End Sub
    Public Sub New(RawData As Byte())
        Me.New
        CreateFile("")
        Length = RawData.Length
        Me.RawData = RawData
    End Sub
#End Region

    ''' <summary>
    ''' Creates a new file with the given name.
    ''' </summary>
    ''' <param name="Name">Name (not path) of the file.  Include the extension if applicable.</param>
    Public Overridable Sub CreateFile(Name As String) 'Implements iCreatableFile.CreateFile
        _tempname = Guid.NewGuid.ToString()
        IO.File.WriteAllBytes(PluginHelper.GetResourceName(_tempname & ".tmp"), {})
        Me.Filename = PluginHelper.GetResourceName(_tempname & ".tmp")
        _tempFilename = PluginHelper.GetResourceName(_tempname & ".tmp")
        Me.OriginalFilename = String.Empty
        Me.Name = Name
    End Sub

    ''' <summary>
    ''' Opens a file from the given filename.
    ''' </summary>
    ''' <param name="Filename"></param>
    Public Overridable Sub OpenFile(Filename As String) ' Implements iOpenableFile.OpenFile
        If _makeTempCopy Then
            _tempname = Guid.NewGuid.ToString()
            Me.OriginalFilename = Filename
            If IO.File.Exists(Filename) Then
                IO.File.Copy(Filename, PluginHelper.GetResourceName(_tempname & ".tmp"))
            Else
                IO.File.WriteAllText(PluginHelper.GetResourceName(_tempname & ".tmp"), "")
            End If
            _tempFilename = PluginHelper.GetResourceName(_tempname & ".tmp")
            Me.Filename = PluginHelper.GetResourceName(_tempname & ".tmp")
        Else
            Me.OriginalFilename = Filename
            Me.Filename = Filename
        End If
    End Sub

#Region "GenericFile Support"
    Public Property Filename As String
    Public Property OriginalFilename As String Implements iOnDisk.Filename
    Public Property Name As String Implements iNamed.Name
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
    Public Overridable Function DefaultExtension() As String Implements iSavable.DefaultExtension
        Return ""
    End Function
#End Region

#Region "Properties"
    Public ReadOnly Property FileReader As IO.FileStream
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
            FileReader.Seek(Index, IO.SeekOrigin.Begin)
            Dim b = FileReader.ReadByte
            If b > -1 AndAlso b < 256 Then
                Return b
            Else
                Throw New IndexOutOfRangeException("Index " & Index.ToString & " is out of range.  Length of file: " & _fileReader.Length.ToString)
            End If
        End Get
        Set(value As Byte)
            FileReader.Seek(Index, IO.SeekOrigin.Begin)
            FileReader.WriteByte(value)
        End Set
    End Property
    Public Property RawData(Index As Long, Length As Long) As Byte()
        Get
            Dim output(Length - 1) As Byte
            FileReader.Seek(Index, IO.SeekOrigin.Begin)
            FileReader.Read(output, 0, Length)
            Return output
        End Get
        Set(value As Byte())
            FileReader.Seek(Index, IO.SeekOrigin.Begin)
            FileReader.Write(value, 0, Length)
        End Set
    End Property
    Public Property RawData() As Byte()
        Get
            Return RawData(0, Length)
        End Get
        Set(value As Byte())
            RawData(0, Length) = value
        End Set
    End Property

    Public Property Int(Index As Long) As Integer
        Get
            Return BitConverter.ToInt32(RawData(Index, 4), 0)
        End Get
        Set(value As Integer)
            Dim bytes = BitConverter.GetBytes(value)
            RawData(Index, 4) = bytes
        End Set
    End Property

    Public Property UInt(Index As Long) As UInteger
        Get
            Return BitConverter.ToUInt32(RawData(Index, 4), 0)
        End Get
        Set(value As UInteger)
            Dim bytes = BitConverter.GetBytes(value)
            RawData(Index, 4) = bytes
        End Set
    End Property

    Public Property Length As Long
        Get
            Return FileReader.Length
        End Get
        Set(value As Long)
            FileReader.SetLength(value)
        End Set
    End Property
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
    Public Event FileModified(sender As Object, e As EventArgs) Implements iModifiable.Modified
    Public Event FileSaved(sender As Object, e As EventArgs) Implements iSavable.FileSaved

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
    Public Sub RaiseModified() Implements iModifiable.RaiseModified
        RaiseFileModified(Me, New EventArgs)
    End Sub

#End Region

#Region "Methods"
    Protected Overridable Sub PreSave()

    End Sub
    Public Overridable Sub Save(Destination As String) Implements ISavableAs.Save
        PreSave()
        FileReader.Seek(0, IO.SeekOrigin.Begin)
        FileReader.Flush()
        If IO.File.Exists(Filename) Then
            IO.File.Copy(Filename, Destination, True)
        Else
            Using dest = IO.File.Open(Destination, IO.FileMode.OpenOrCreate, IO.FileAccess.Write)
                FileReader.CopyTo(dest)
            End Using
        End If
        If String.IsNullOrEmpty(OriginalFilename) Then
            OriginalFilename = Destination
        End If
        RaiseEvent FileSaved(Me, New EventArgs)
    End Sub
    Public Overridable Sub Save() Implements iSavable.Save
        PreSave()
        FileReader.Seek(0, IO.SeekOrigin.Begin)
        FileReader.Flush()
        If Not Filename = OriginalFilename Then IO.File.Copy(Filename, OriginalFilename, True)
        RaiseEvent FileSaved(Me, New EventArgs)
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