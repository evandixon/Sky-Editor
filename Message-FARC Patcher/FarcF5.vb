''' <summary>
''' Models a type 5 FARC file, one that does not contain embedded filenames.
''' </summary>
Public Class FarcF5
    Inherits GenericFile

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
    ''' Extracts the FARC to the given directory.
    ''' </summary>
    ''' <param name="Directory">Directory to extract the FARC to.</param>
    Public Sub Extract(Directory As String)
        For count = 0 To FileCount - 1
            Dim filename As String
            Dim fileHash As UInteger = Header.FileData(count).FilenamePointer
            filename = Conversion.Hex(fileHash).PadLeft(8, "0"c)
            IO.File.WriteAllBytes(IO.Path.Combine(Directory, filename), GetFileData(count))
        Next
    End Sub

    Public Sub New()
        EnableInMemoryLoad = True
    End Sub

    Public Overrides Sub OpenFile(Filename As String)
        MyBase.OpenFile(Filename)

        Dim sir0Type = Me.Int32(&H20)
        Dim sir0Offset = Me.Int32(&H24)
        Dim sir0Length = Me.Int32(&H28)
        DataOffset = Me.Int32(&H2C)
        Dim datLength = Me.Int32(&H30)

        'Todo: use another class for another sir0 type
        'This code is for sir0 type 5
        Header = New Sir0Fat5(Me.RawData(sir0Offset, sir0Length))
    End Sub

    Public Shared Sub Pack(SourceDirectory As String, DestinationFarcFilename As String)
        If IO.File.Exists(DestinationFarcFilename) Then
            IO.File.Delete(DestinationFarcFilename)
        End If

        'Only works for FARC files that lack filenames
        Dim header As New Sir0Fat5
        header.CreateFile("")
        Dim fileNames = IO.Directory.GetFiles(SourceDirectory)
        Dim fileData As New List(Of Byte)
        Dim orderedNames = (From f In fileNames Order By System.UInt32.Parse(IO.Path.GetFileNameWithoutExtension(IO.Path.GetFileName(f)), Globalization.NumberStyles.HexNumber) Ascending)
        'While the file names are probably already in order, it's VERY important that the FARC files are in order by hash, ascending.
        For Each item In orderedNames
            Dim entry As New Sir0Fat5.FileInfo
            entry.DataOffset = fileData.Count
            entry.FilenamePointer = System.UInt32.Parse(IO.Path.GetFileNameWithoutExtension(item), Globalization.NumberStyles.HexNumber)
            Dim current = IO.File.ReadAllBytes(item)
            entry.DataLength = current.Length
            fileData.AddRange(current)

            header.FileData.Add(entry)
        Next

        header.Save()
        Dim headerData = header.RawData
        header.Dispose()

        Dim archiveBytes As New List(Of Byte)

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
    End Sub

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

