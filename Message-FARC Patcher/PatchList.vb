Public Class PatchList
    Public Class PatchItem
        Public Property FarcFileHash As UInteger
        Public Property StringID As UInteger
        Public Property StringData As Byte()
    End Class
    Public Property Patches As List(Of PatchItem)
    Public Sub New()
        Patches = New List(Of PatchItem)
    End Sub
End Class
