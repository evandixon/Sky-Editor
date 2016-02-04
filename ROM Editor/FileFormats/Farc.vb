Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats
    ''' <summary>
    ''' Models a type 5 FARC file, one that does not contain embedded filenames.
    ''' </summary>
    Public Class FarcF5
        Inherits SkyEditorBase.GenericFile
        Implements iOpenableFile

        Public Property Header As Sir0Fat5
        Protected Property DataOffset As Integer

        Public ReadOnly Property FileCount As Integer
            Get
                Return Header.FileData.Count
            End Get
        End Property

        Public Function GetFileData(FileIndex As Integer) As Byte()
            Return RawData(Header.FileData(FileIndex).DataOffset + DataOffset, Header.FileData(FileIndex).DataLength)
        End Function

        ''' <summary>
        ''' Gets the FARC file with the given filename, if it exists.
        ''' Otherwise, returns nothing.
        ''' </summary>
        ''' <param name="Filename">Name of the file to look for.</param>
        ''' <returns></returns>
        Public Function GetFileData(Filename As String) As Byte()
            'Only works on Farc files without filenames.
            Dim dic = GetFileDictionary()
            Dim hash As UInteger? = (From kv In dic Where String.Compare(Filename, kv.Value, True, Globalization.CultureInfo.InvariantCulture) = 0 Select kv.Key).FirstOrDefault
            If hash IsNot Nothing Then
                Dim info = (From i In Header.FileData Where i.FilenamePointer = hash).FirstOrDefault
                If info IsNot Nothing Then
                    Return RawData(info.DataOffset, info.DataLength)
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        End Function

        '''' <summary>
        '''' Copies the file at FileIndex into the given Stream at its current position.
        '''' </summary>
        '''' <param name="FileIndex"></param>
        '''' <param name="Stream"></param>
        '''' <param name="FullCopy">True to write at the beginning of the target stream and set the length to the proper length.</param>
        'Public Sub ReadData(FileIndex As Integer, Stream As IO.Stream, FullCopy As Boolean)
        '    Me.FileReader.Seek(Header.FileData(FileIndex).DataOffset + DataOffset, IO.SeekOrigin.Begin)
        '    If FullCopy Then
        '        Stream.Seek(0, IO.SeekOrigin.Begin)
        '        Stream.SetLength(Header.FileData(FileIndex).DataLength)
        '    End If
        '    For count = 0 To Header.FileData(FileIndex).DataLength - 1
        '        Stream.WriteByte(FileReader.ReadByte)
        '    Next
        'End Sub

        ''' <summary>
        ''' Extracts the FARC to the given directory.
        ''' </summary>
        ''' <param name="Directory">Directory to extract the FARC to.</param>
        Public Async Function Extract(Directory As String, Optional UseDictionary As Boolean = True) As Task
            Dim asyncFor As New Utilities.AsyncFor(PluginHelper.GetLanguageItem("Extracting files..."))
            Dim dic As Dictionary(Of UInteger, String)
            If UseDictionary Then
                dic = GetFileDictionary()
            Else
                dic = New Dictionary(Of UInteger, String)
            End If
            Await asyncFor.RunFor(Sub(Count As Integer)
                                      Dim filename As String
                                      Dim fileHash As UInteger = Header.FileData(Count).FilenamePointer
                                      If dic.ContainsKey(fileHash) Then
                                          filename = dic(fileHash)
                                      Else
                                          filename = fileHash.ToString 'Count.ToString
                                      End If
                                      IO.File.WriteAllBytes(IO.Path.Combine(Directory, filename), GetFileData(Count))
                                      'Using f As New IO.FileStream(IO.Path.Combine(Directory, filename), IO.FileMode.OpenOrCreate)
                                      '    ReadData(Count, f, True)
                                      'End Using
                                  End Sub, 0, FileCount - 1, 1)
        End Function

        ''' <summary>
        ''' Gets a dictionary matching file indexes to file names.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetFileDictionary() As Dictionary(Of UInteger, String)
            Dim out As New Dictionary(Of UInteger, String)
            Dim resourceFile = PluginHelper.GetResourceName(IO.Path.Combine("farc", IO.Path.GetFileNameWithoutExtension(Me.OriginalFilename) & ".txt"))
            If IO.File.Exists(resourceFile) Then
                Dim i As New BasicIniFile
                i.OpenFile(resourceFile)
                For Each item In i.Entries
                    out.Add(CUInt(item.Key), item.Value)
                Next
            End If
            Return out
        End Function

        Public Sub New()
            MyBase.New
        End Sub

        Public Overrides Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            MyBase.OpenFile(Filename)

            Dim sir0Type = Me.Int(&H20)
            Dim sir0Offset = Me.Int(&H24)
            Dim sir0Length = Me.Int(&H28)
            DataOffset = Me.Int(&H2C)
            Dim datLength = Me.Int(&H30)

            'Todo: use another class for another sir0 type
            'This code is for sir0 type 5
            Header = New Sir0Fat5(Me.RawData(sir0Offset, sir0Length))
        End Sub

        Public Shared Async Function Pack(SourceDirectory As String, DestinationFarcFilename As String) As Task
            Dim header As New Sir0Fat5
            Dim files = IO.Directory.GetFiles(SourceDirectory)
            Throw New NotImplementedException
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overrides Sub Dispose(disposing As Boolean)
            MyBase.Dispose(True)

            If Not Me.disposedValue Then
                If disposing Then
                    Header.Dispose()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub
#End Region

    End Class
End Namespace

