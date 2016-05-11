Imports SkyEditor.Core

Public Class GenericFile
    Inherits SkyEditor.Core.IO.GenericFile

#Region "Constructors"
    ''' <summary>
    ''' Creates a new instance of GenericFile for use with either GenericFile.OpenFile or GenericFile.CreateFile
    ''' </summary>
    Public Sub New()
        MyBase.New()
        FileProvider = New IOProvider
    End Sub

    ''' <summary>
    ''' Creates a new instance of GenericFile using the given data.
    ''' </summary>
    ''' <param name="RawData"></param>
    Public Sub New(RawData As Byte())
        MyBase.New(New IOProvider, RawData)
    End Sub

    ''' <summary>
    ''' Creates a new instance of GenericFile from the given file.
    ''' </summary>
    ''' <param name="Filename">Full path of the file to load.</param>
    Public Sub New(Filename As String)
        MyBase.New(New IOProvider, Filename)
    End Sub

    ''' <summary>
    ''' Creates a new instance of GenericFile from the given file.
    ''' </summary>
    ''' <param name="Filename">Full path of the file to load.</param>
    ''' <param name="IsReadOnly">Whether or not to allow altering the file.  If True, an IOException will be thrown when attempting to alter the file.</param>
    Public Sub New(Filename As String, IsReadOnly As Boolean)
        MyBase.New(New IOProvider, Filename, IsReadOnly)
    End Sub

    ''' <summary>
    ''' Creates a new instance of GenericFile from the given file.
    ''' </summary>
    ''' <param name="Filename">Full path of the file to load.</param>
    ''' <param name="IsReadOnly">Whether or not to allow altering the file.  If True, an IOException will be thrown when attempting to alter the file, regardless of whether LoadToMemory is true.</param>
    ''' <param name="LoadToMemory">True to load the file into memory, False to use a FileStream.  If loading the file into memory would leave the system with less than 500MB, a FileStream will be used instead.</param>
    Public Sub New(Filename As String, IsReadOnly As Boolean, LoadToMemory As Boolean)
        MyBase.New(New IOProvider, Filename, IsReadOnly, LoadToMemory)
    End Sub

#End Region

End Class
