Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Public Class BinaryFile
    Inherits GenericFile
    Implements iOpenableFile
    Implements iModifiable

#Region "Constructors"
    Public Sub New()
        Bits = New Binary(0)
    End Sub

    Public Sub New(Filename As String)
        MyBase.New()
        OpenFile(Filename)
    End Sub

    Public Overrides Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
        MyBase.OpenFile(Filename)
        Bits = New Binary(0)
        ProcessRawData()
    End Sub

    Private Sub ProcessRawData()
        For count As Integer = 0 To Length - 1
            Bits.AppendByte(RawData(count))
        Next
    End Sub
#End Region

    Public Property Bits As Binary

    Protected Overridable Sub FixChecksum()

    End Sub

    Protected Overrides Sub PreSave()
        MyBase.PreSave()
        FixChecksum()
        For count As Integer = 0 To Math.Ceiling(Bits.Count / 8) - 1
            RawData(count) = Bits.Int(count, 0, 8)
        Next
    End Sub
End Class
