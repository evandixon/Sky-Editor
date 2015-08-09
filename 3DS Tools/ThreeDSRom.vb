Public Class ThreeDSRom
    Inherits SkyEditorBase.GenericSave
    Public Property PartitionFlags As PartitionFlags

    Public Overrides Function DefaultSaveID() As String
        Return GameConstants.ThreeDSRom
    End Function

    Public Overrides Async Sub Save(Path As String)
        Await Task.Run(Sub()
                           If Not Me.Filename = Path Then
                               IO.File.Copy(Filename, Path)
                           End If
                           Dim file As New IO.FileStream(Path, IO.FileMode.Open)
                           file.Seek(&H188, IO.SeekOrigin.Begin)
                           file.Write(PartitionFlags.RawData, 0, 8)
                           file.Close()
                       End Sub)
    End Sub

    Public Sub New(Filename As String)
        MyBase.New(Filename)

        Dim buffer(7) As Byte
        Dim file As New IO.FileStream(Filename, IO.FileMode.Open)
        file.Seek(&H188, IO.SeekOrigin.Begin)
        file.Read(buffer, 0, 8)
        PartitionFlags = New PartitionFlags(buffer)
        file.Close()

    End Sub
End Class