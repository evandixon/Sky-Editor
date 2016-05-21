Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities
Imports SkyEditor.Core.Windows
Imports SkyEditorBase

Namespace FileFormats.PSMD
    ''' <summary>
    ''' Models a type 5 FARC file, one that does not contain embedded filenames.
    ''' </summary>
    Public Class FarcF5
        Inherits GenericFile
        Implements IOpenableFile

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
                    Return RawData(info.DataOffset + DataOffset, info.DataLength)
                Else
                    Throw New IndexOutOfRangeException("Unable to find entry with name " & Filename)
                End If
            Else
                Throw New IndexOutOfRangeException("Unable to find entry with name " & Filename)
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
            Dim asyncFor As New AsyncFor(My.Resources.Language.FarcLoadingExtract)
            Dim dic As Dictionary(Of UInteger, String)
            If UseDictionary Then
                dic = GetFileDictionary()
            Else
                dic = New Dictionary(Of UInteger, String)
            End If
            'Extract the files.
            'Async if thread safe, sync otherwise
            asyncFor.RunSynchronously = Not Me.IsThreadSafe
            Await asyncFor.RunFor(Sub(Count As Integer)
                                      Dim filename As String
                                      Dim fileHash As UInteger = Header.FileData(Count).FilenamePointer
                                      If dic.ContainsKey(fileHash) Then
                                          filename = dic(fileHash)
                                      Else
                                          filename = fileHash.ToString 'Count.ToString
                                      End If
                                      IO.File.WriteAllBytes(IO.Path.Combine(Directory, filename), GetFileData(Count))
                                  End Sub, 0, FileCount - 1)
        End Function

        ''' <summary>
        ''' Gets a dictionary matching file indexes to file names.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetFileDictionary() As Dictionary(Of UInteger, String)
            Dim out As New Dictionary(Of UInteger, String)
            Dim resourceFile = My.Resources.FarcFilenames.ResourceManager.GetString(IO.Path.GetFileNameWithoutExtension(Me.OriginalFilename)) ' PluginHelper.GetResourceName(IO.Path.Combine("farc", IO.Path.GetFileNameWithoutExtension(Me.OriginalFilename) & ".txt"))
            If Not String.IsNullOrEmpty(resourceFile) Then
                Dim i As New BasicIniFile
                i.CreateFile(resourceFile)
                For Each item In i.Entries
                    out.Add(CUInt(item.Key), item.Value)
                Next
            End If
            'If IO.File.Exists(resourceFile) Then
            '    Dim i As New BasicIniFile
            '    i.OpenFile(resourceFile)
            '    For Each item In i.Entries
            '        out.Add(CUInt(item.Key), item.Value)
            '    Next
            'End If
            Return out
        End Function

        Public Sub New()
            MyBase.New()
            Me.EnableInMemoryLoad = True
        End Sub

        Public Overrides Async Function OpenFile(Filename As String, Provider As IOProvider) As Task Implements IOpenableFile.OpenFile
            Await MyBase.OpenFile(Filename, Provider)

            Dim sir0Type = Me.Int32(&H20)
            Dim sir0Offset = Me.Int32(&H24)
            Dim sir0Length = Me.Int32(&H28)
            DataOffset = Me.Int32(&H2C)
            Dim datLength = Me.Int32(&H30)

            'Todo: use another class for another sir0 type
            'This code is for sir0 type 5
            Header = New Sir0Fat5
            Header.EnableInMemoryLoad = True
            Header.CreateFile("", Me.RawData(sir0Offset, sir0Length))
        End Function

        Public Shared Function Pack(SourceDirectory As String, DestinationFarcFilename As String, provider As IOProvider) As Task
            If IO.File.Exists(DestinationFarcFilename) Then
                IO.File.Delete(DestinationFarcFilename)
            End If

            'Only works for FARC files that lack filenames
            Dim header As New Sir0Fat5
            header.CreateFile("")
            Dim fileNames = IO.Directory.GetFiles(SourceDirectory)
            'Dim fileData As New GenericFile({})
            Dim fileData As New List(Of Byte)
            Dim filenameDic = GetReverseFileDictionary(DestinationFarcFilename, provider)

            For Each item In From kv In filenameDic Order By kv.Value
                Dim entry As New Sir0Fat5.FileInfo
                entry.DataOffset = fileData.Count
                entry.FilenamePointer = item.Value
                Dim current = IO.File.ReadAllBytes(IO.Path.Combine(SourceDirectory, item.Key))
                entry.DataLength = current.Length
                'Using file As New GenericFile(IO.Path.Combine(SourceDirectory, item.Key), True)
                '    entry.DataLength = file.Length
                '    Await fileData.AppendFile(file)
                'End Using
                fileData.AddRange(current)

                header.FileData.Add(entry)
            Next

            'For Each item In fileNames
            '    If filenameDic.ContainsKey(IO.Path.GetFileNameWithoutExtension(item)) Then

            '        Dim entry As New Sir0Fat5.FileInfo
            '        entry.DataOffset = fileData.Length
            '        entry.FilenamePointer = filenameDic(IO.Path.GetFileNameWithoutExtension(item))

            '        Using file As New GenericFile(item, True)
            '            entry.DataLength = file.Length
            '            Await fileData.AppendFile(file)
            '        End Using

            '        header.FileData.Add(entry)

            '    Else
            '        Throw New IndexOutOfRangeException(String.Format("No file hash can be found for filename ""{0}"".", IO.Path.GetFileNameWithoutExtension(item)))
            '    End If
            'Next

            Dim tempName As String = Guid.NewGuid.ToString
            header.Save(EnvironmentPaths.GetResourceName(tempName & ".tmp"), provider)
            header.Dispose()
            Dim headerData = IO.File.ReadAllBytes(EnvironmentPaths.GetResourceName(tempName & ".tmp"))
            IO.File.Delete(EnvironmentPaths.GetResourceName(tempName & ".tmp"))

            Dim archiveBytes As New List(Of Byte)
            'Dim archive As New FarcF5
            'archive.CreateFile("")
            'archive.Length = &H80 '+ header.Length + fileData.Length
            'archive.RawData(0, 4) = {&H46, &H41, &H52, &H43} 'Magic: FARC
            'archive.Int(4) = 0 'Unknown value
            'archive.Int(8) = 0 'Unknown value
            'archive.Int(&HC) = 2 'Unknown value
            'archive.Int(&H10) = 0 'Unknown value
            'archive.Int(&H14) = 0 'Unknown, usually 0
            'archive.Int(&H18) = 7 'Unknown, usually 7
            'archive.Int(&H1C) = &H77EA3CA4 'Unknown
            'archive.Int(&H20) = 5 'SIR0 version
            'archive.Int(&H24) = &H80 'SIR0 offset, always 0x80
            'archive.Int(&H28) = headerData.Length
            'archive.Int(&H2C) = &H80 + headerData.Length 'Data offset
            'archive.Int(&H30) = fileData.Count

            'archive.Append(headerData)
            'archive.Append(fileData.ToArray)
            'archive.Save(DestinationFarcFilename)

            archiveBytes.AddRange({&H46, &H41, &H52, &H43}) 'Magic: FARC)
            archiveBytes.AddRange(BitConverter.GetBytes(0)) '0x4
            archiveBytes.AddRange(BitConverter.GetBytes(0)) '0x8
            archiveBytes.AddRange(BitConverter.GetBytes(2)) '0xC
            archiveBytes.AddRange(BitConverter.GetBytes(0)) '0x10
            archiveBytes.AddRange(BitConverter.GetBytes(0)) '0x14
            archiveBytes.AddRange(BitConverter.GetBytes(7)) '0x18
            archiveBytes.AddRange(BitConverter.GetBytes(&H77EA3CA4)) '0x1C
            archiveBytes.AddRange(BitConverter.GetBytes(5)) '0x20
            archiveBytes.AddRange(BitConverter.GetBytes(&H80)) '0x24
            archiveBytes.AddRange(BitConverter.GetBytes(headerData.Length)) '0x28
            archiveBytes.AddRange(BitConverter.GetBytes(&H80 + headerData.Length)) '0x2C
            archiveBytes.AddRange(BitConverter.GetBytes(fileData.Count)) '0x30

            For count = 0 To &H4C - 1
                archiveBytes.Add(0)
            Next

            archiveBytes.AddRange(headerData)
            archiveBytes.AddRange(fileData.ToArray)

            IO.File.WriteAllBytes(DestinationFarcFilename, archiveBytes.ToArray)


            'archive.Dispose()
            Return Task.CompletedTask
        End Function

        Private Shared Function GetReverseFileDictionary(Filename As String, provider As IOProvider) As Dictionary(Of String, UInteger)
            Dim out As New Dictionary(Of String, UInteger)
            Dim resourceFile = EnvironmentPaths.GetResourceName(IO.Path.Combine("farc", IO.Path.GetFileNameWithoutExtension(Filename) & ".txt"))
            If IO.File.Exists(resourceFile) Then
                Dim i As New BasicIniFile
                i.OpenFile(resourceFile, provider)
                For Each item In i.Entries
                    out.Add(item.Value, CUInt(item.Key))
                Next
            End If
            Return out
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overrides Sub Dispose(disposing As Boolean)
            MyBase.Dispose(True)

            If Not Me.disposedValue Then
                If disposing AndAlso Header IsNot Nothing Then
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

