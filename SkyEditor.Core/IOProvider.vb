Public MustInherit Class IOProvider
    ''' <summary>
    ''' Gets the length, in bytes, of the file at the given path.
    ''' </summary>
    ''' <param name="Filename">Full path of the file.</param>
    ''' <returns>The length, in bytes, of the file</returns>
    Public MustOverride Function GetFileLength(Filename As String) As Long

    ''' <summary>
    ''' Determines whether the specified file exists.
    ''' </summary>
    ''' <param name="Filename">Full path of the file.</param>
    ''' <returns></returns>
    Public MustOverride Function FileExists(Filename As String) As Boolean

    ''' <summary>
    ''' Reads a file from disk, and returns its contents as a byte array.
    ''' </summary>
    ''' <param name="Filename">Full path of the file.</param>
    ''' <returns></returns>
    Public MustOverride Function ReadAllBytes(Filename As String) As Byte()

    ''' <summary>
    ''' Writes the given text to disk.
    ''' </summary>
    ''' <param name="Filename">Full path of the file.</param>
    ''' <param name="Data">File contents to be written.</param>
    Public MustOverride Sub WriteAllText(Filename As String, Data As String)

    ''' <summary>
    ''' Reads a file from disk, and returns its contents as a string.
    ''' </summary>
    ''' <param name="Filename">Full path of the file.</param>
    ''' <returns></returns>
    Public MustOverride Function ReadAllText(Filename As String) As String

    ''' <summary>
    ''' Writes the given byte array to disk.
    ''' </summary>
    ''' <param name="Filename">Full path of the file.</param>
    ''' <param name="Data">File contents to be written.</param>
    Public MustOverride Sub WriteAllBytes(Filename As String, Data As Byte())

    ''' <summary>
    ''' Copies a file, overwriting the destination file if it exists.
    ''' </summary>
    ''' <param name="SourceFilename"></param>
    ''' <param name="DestinationFilename"></param>
    Public MustOverride Sub CopyFile(SourceFilename As String, DestinationFilename As String)

    ''' <summary>
    ''' Deletes the file at the given path.
    ''' </summary>
    ''' <param name="Filename">Full path of the file.</param>
    Public MustOverride Sub DeleteFile(Filename As String)

    ''' <summary>
    ''' Creates a temporary, blank file, and returns the filename.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function GetTempFilename() As String

    ''' <summary>
    ''' Determines whether or not a file of the given size will fit in memory.
    ''' </summary>
    ''' <param name="FileSize">Full path of the file.</param>
    ''' <returns></returns>
    Public MustOverride Function CanLoadFileInMemory(FileSize As Long) As Boolean

    ''' <summary>
    ''' Opens a file stream with Read/Write privilages.
    ''' </summary>
    ''' <param name="Filename">Full path of the file.</param>
    ''' <returns></returns>
    Public MustOverride Function OpenFile(Filename As String) As IO.Stream

    ''' <summary>
    ''' Opens a file stream with Read privilages.
    ''' </summary>
    ''' <param name="Filename">Full path of the file.</param>
    ''' <returns></returns>
    Public MustOverride Function OpenFileReadOnly(Filename As String) As IO.Stream

    ''' <summary>
    ''' Opens a file stream with Write privilages.
    ''' </summary>
    ''' <param name="Filename">Full path of the file.</param>
    ''' <returns></returns>
    Public MustOverride Function OpenFileWriteOnly(Filename As String) As IO.Stream
End Class
