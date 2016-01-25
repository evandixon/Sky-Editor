Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats
    ''' <summary>
    ''' Models a type 5 FARC file, one that does not contain embedded filenames.
    ''' </summary>
    Public Class FarcF5
        Inherits SkyEditorBase.GenericFile
        Implements iOpenableFile

        Protected Property Header As Sir0Fat5
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
        ''' Copies the file at FileIndex into the given Stream at its current position.
        ''' </summary>
        ''' <param name="FileIndex"></param>
        ''' <param name="Stream"></param>
        Public Sub ReadData(FileIndex As Integer, Stream As IO.Stream)
            Me.FileReader.Seek(Header.FileData(FileIndex).DataOffset + DataOffset, IO.SeekOrigin.Begin)
            For count = 0 To Header.FileData(FileIndex).DataLength
                Stream.WriteByte(FileReader.ReadByte)
            Next
        End Sub

        ''' <summary>
        ''' Extracts the FARC to the given directory.
        ''' </summary>
        ''' <param name="Directory">Directory to extract the FARC to.</param>
        Public Async Function Extract(Directory As String) As Task
            Dim asyncFor As New Utilities.AsyncFor(PluginHelper.GetLanguageItem("Extracting files..."))
            Dim dic = GetFileDictionary()
            Await asyncFor.RunFor(Sub(Count As Integer)
                                      Dim filename As String
                                      If dic.ContainsKey(Count) Then
                                          filename = dic(Count)
                                      Else
                                          filename = Count.ToString
                                      End If
                                      Using f As New IO.FileStream(IO.Path.Combine(Directory, filename), IO.FileMode.OpenOrCreate)
                                          ReadData(Count, f)
                                      End Using
                                  End Sub, 0, FileCount - 1, 1)
        End Function

        ''' <summary>
        ''' Gets a dictionary matching file indexes to file names.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetFileDictionary() As Dictionary(Of Integer, String)
            Dim out As New Dictionary(Of Integer, String)
            Dim resourceFile = PluginHelper.GetResourceName(IO.Path.Combine("farc", IO.Path.GetFileNameWithoutExtension(Me.Filename) & ".txt"))
            If IO.File.Exists(resourceFile) Then
                Dim i As New BasicIniFile
                i.OpenFile(resourceFile)
                For Each item In i.Entries
                    out.Add(CInt(item.Key), item.Value)
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

