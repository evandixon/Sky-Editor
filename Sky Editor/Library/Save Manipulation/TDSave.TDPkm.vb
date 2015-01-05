Imports SkyEditorBase

Partial Class TDSave
    Public Class TDPkm
        Public Property RawData As Byte()
        Public Sub New(RawData As Byte())
            Me.RawData = RawData
        End Sub
        Public ReadOnly Property Name As String
            Get
                Const offset As Integer = 58
                Dim out As String = ""
                For count As Integer = 0 To 9
                    If RawData(offset + count) = 0 Then Exit For
                    out += Lists.StringEncoding(RawData(offset + count))
                Next
                Return out
            End Get
        End Property
        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class
    Public Property Pokemon As TDPkm()
        Get
            Const StartOffset As Integer = &H83CB
            Const PkmLength As Integer = &H44
            Dim out As New List(Of TDPkm)
            For count As Integer = 0 To 720
                If RawData(StartOffset + (count * PkmLength)) = 0 Then Exit For
                out.Add(New TDPkm(GenericArrayOperations(Of Byte).CopyOfRange(RawData, StartOffset + (count * PkmLength), StartOffset + ((count + 1) * PkmLength) - 1)))
            Next
            Return out.ToArray
        End Get
        Set(value As TDPkm())
            Throw New NotImplementedException
        End Set
    End Property
End Class
