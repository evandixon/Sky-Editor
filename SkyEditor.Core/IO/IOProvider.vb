Namespace IO
    Public MustInherit Class IOProvider
        ''' <summary>
        ''' Gets the length, in bytes, of the file at the given path.
        ''' </summary>
        ''' <param name="filename">Full path of the file.</param>
        ''' <returns>The length, in bytes, of the file</returns>
        Public MustOverride Function GetFileLength(filename As String) As Long

        ''' <summary>
        ''' Determines whether the specified file exists.
        ''' </summary>
        ''' <param name="filename">Full path of the file.</param>
        ''' <returns></returns>
        Public MustOverride Function FileExists(filename As String) As Boolean

        ''' <summary>
        ''' Determines whether the specified directory exists.
        ''' </summary>
        ''' <param name="directory">Full path of the directory.</param>
        ''' <returns></returns>
        Public MustOverride Function DirectoryExists(directory As String) As Boolean

        ''' <summary>
        ''' Reads a file from disk, and returns its contents as a byte array.
        ''' </summary>
        ''' <param name="filename">Full path of the file.</param>
        ''' <returns></returns>
        Public MustOverride Function ReadAllBytes(filename As String) As Byte()

        ''' <summary>
        ''' Writes the given text to disk.
        ''' </summary>
        ''' <param name="filename">Full path of the file.</param>
        ''' <param name="data">File contents to be written.</param>
        Public MustOverride Sub WriteAllText(filename As String, Data As String)

        ''' <summary>
        ''' Reads a file from disk, and returns its contents as a string.
        ''' </summary>
        ''' <param name="filename">Full path of the file.</param>
        ''' <returns></returns>
        Public MustOverride Function ReadAllText(filename As String) As String

        ''' <summary>
        ''' Writes the given byte array to disk.
        ''' </summary>
        ''' <param name="filename">Full path of the file.</param>
        ''' <param name="data">File contents to be written.</param>
        Public MustOverride Sub WriteAllBytes(filename As String, data As Byte())

        ''' <summary>
        ''' Copies a file, overwriting the destination file if it exists.
        ''' </summary>
        ''' <param name="sourceFilename"></param>
        ''' <param name="destinationFilename"></param>
        Public MustOverride Sub CopyFile(sourceFilename As String, destinationFilename As String)

        ''' <summary>
        ''' Deletes the file at the given path.
        ''' </summary>
        ''' <param name="filename">Full path of the file.</param>
        Public MustOverride Sub DeleteFile(filename As String)

        ''' <summary>
        ''' Creates a temporary, blank file, and returns the filename.
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride Function GetTempFilename() As String

        ''' <summary>
        ''' Determines whether or not a file of the given size will fit in memory.
        ''' </summary>
        ''' <param name="fileSize">Full path of the file.</param>
        ''' <returns></returns>
        Public MustOverride Function CanLoadFileInMemory(fileSize As Long) As Boolean

        ''' <summary>
        ''' Opens a file stream with Read/Write privilages.
        ''' </summary>
        ''' <param name="filename">Full path of the file.</param>
        ''' <returns></returns>
        Public MustOverride Function OpenFile(filename As String) As Stream

        ''' <summary>
        ''' Opens a file stream with Read privilages.
        ''' </summary>
        ''' <param name="filename">Full path of the file.</param>
        ''' <returns></returns>
        Public MustOverride Function OpenFileReadOnly(filename As String) As Stream

        ''' <summary>
        ''' Opens a file stream with Write privilages.
        ''' </summary>
        ''' <param name="filename">Full path of the file.</param>
        ''' <returns></returns>
        Public MustOverride Function OpenFileWriteOnly(filename As String) As Stream
    End Class

End Namespace
