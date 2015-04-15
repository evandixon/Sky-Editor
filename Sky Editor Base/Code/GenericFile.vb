Public Class GenericFile
    Public Property RawData As Byte()
    Public Property Filename As String
    Public Sub New(Save As Byte())
        Me.RawData = Save
    End Sub
    Public Sub New(Filename As String)
        Me.Filename = Filename
    End Sub
End Class