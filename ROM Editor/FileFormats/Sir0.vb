Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats
    Public Class Sir0
        Inherits GenericFile
        Implements iOpenableFile
        Private Property HeaderOffset As Integer
        Private Property PointerOffset As Integer
        Private ReadOnly Property PointerLength As Integer
            Get
                Return Length - PointerOffset
            End Get
        End Property
        Private ReadOnly Property HeaderLength As Integer
            Get
                Return PointerOffset - HeaderOffset
            End Get
        End Property
        Private ReadOnly Property DataLength As Integer
            Get
                Return Length - 16 - HeaderLength - PointerLength
            End Get
        End Property
        Protected Property Header As Byte()

        Public Overrides Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            MyBase.OpenFile(Filename)
            ProcessData()
        End Sub
        Public Overrides Sub Save(Destination As String)
            'Todo: update footer and pointer offsets

            'Update footer
            RawData(HeaderOffset, HeaderLength) = Header

            'Todo: update pointers and padding

            MyBase.Save(Destination)
        End Sub

        Private Sub ProcessData()
            HeaderOffset = Me.Int(&H4)
            PointerOffset = Me.Int(&H8)
            Header = RawData(HeaderOffset, HeaderLength)
        End Sub

        Public Sub New()
            MyBase.New
        End Sub

        Public Sub New(RawData As Byte())
            MyBase.New(RawData)
            ProcessData()
        End Sub
    End Class
End Namespace

