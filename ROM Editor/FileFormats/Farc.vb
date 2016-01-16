Imports SkyEditorBase.Interfaces

Namespace FileFormats
    Public Class Farc
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

