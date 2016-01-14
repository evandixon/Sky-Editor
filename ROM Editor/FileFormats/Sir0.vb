Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats
    Public Class Sir0
        Inherits GenericFile
        Implements iOpenableFile
        Protected Property HeaderOffset As Integer
        Protected Property PointerOffset As Integer
        Protected ReadOnly Property PointerLength As Integer
            Get
                Return Length - PointerOffset
            End Get
        End Property
        Protected ReadOnly Property HeaderLength As Integer
            Get
                Return PointerOffset - HeaderOffset
            End Get
        End Property
        Protected ReadOnly Property DataLength As Integer
            Get
                Return Length - 16 - HeaderLength - PointerLength
            End Get
        End Property

        Public Overrides Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            MyBase.OpenFile(Filename)
            ProcessData()
        End Sub

        Private Sub ProcessData()
            HeaderOffset = Me.Int(&H4)
            PointerOffset = Me.Int(&H8)
        End Sub

        Public Sub New(RawData As Byte())
            MyBase.New(RawData)
            ProcessData()
        End Sub
    End Class
End Namespace

