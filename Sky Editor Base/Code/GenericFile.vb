Imports SkyEditorBase.Interfaces
Public Class GenericFile
    Implements IDisposable
    Implements iModifiable
    Implements iCreatableFile
    Implements iOpenableFile
    Implements iSavable
    Protected _tempname As String
    Dim _fileReader As IO.FileStream

#Region "Constructors"
    Public Sub New(Filename As String)
        _tempname = Guid.NewGuid.ToString()
        Me.OriginalFilename = Filename
        If IO.File.Exists(Filename) Then
            IO.File.Copy(Filename, PluginHelper.GetResourceName(_tempname & ".tmp"))
        Else
            IO.File.WriteAllText(PluginHelper.GetResourceName(_tempname & ".tmp"), "")
        End If
        Me.Filename = PluginHelper.GetResourceName(_tempname & ".tmp")
    End Sub
    Public Sub New(RawData As Byte())
        _tempname = Guid.NewGuid.ToString()
        IO.File.WriteAllBytes(PluginHelper.GetResourceName(_tempname & ".tmp"), RawData)
        Me.Filename = PluginHelper.GetResourceName(_tempname & ".tmp")
        Me.OriginalFilename = Filename
    End Sub
    Public Sub New()
        _tempname = Guid.NewGuid.ToString()
        IO.File.WriteAllBytes(PluginHelper.GetResourceName(_tempname & ".tmp"), {})
        Me.Filename = PluginHelper.GetResourceName(_tempname & ".tmp")
        Me.OriginalFilename = Filename
    End Sub
#End Region

#Region "Properties"
    Public Property Filename As String
    Public Property OriginalFilename As String
    Protected ReadOnly Property FileReader As IO.FileStream
        Get
            If _fileReader Is Nothing Then
                _fileReader = IO.File.Open(Filename, IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite, IO.FileShare.Read)
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
            'End If
        End Get
        Set(value As Byte())
            FileReader.Seek(Index, IO.SeekOrigin.Begin)
            FileReader.Write(value, 0, Length)
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
    Public Sub RaiseModified() Implements iModifiable.RaiseModified
        RaiseFileModified(Me, New EventArgs)
    End Sub

#End Region

#Region "Methods"
    Public Overridable Function DefaultExtension() As String
        Return ""
    End Function
    Protected Overridable Sub PreSave()

    End Sub
    Public Overridable Sub Save(Destination As String) Implements iSavable.Save
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
    Public Overridable Sub Save() Implements iSavable.Save
        PreSave()
        _fileReader.Seek(0, IO.SeekOrigin.Begin)
        _fileReader.Flush()
        If Not Filename = OriginalFilename Then IO.File.Copy(Filename, OriginalFilename, True)
    End Sub
#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If _fileReader IsNot Nothing Then _fileReader.Dispose()
                If IO.File.Exists(_tempname) Then IO.File.Delete(_tempname)
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