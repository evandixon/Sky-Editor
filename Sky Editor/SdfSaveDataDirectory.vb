Public Class SdfSaveDataDirectory
    Private Property Saves As List(Of SdfSave)
    Public Property Path As String
    Public Sub DeleteSave(Save As SdfSave)
        IO.Directory.Delete(Save.Path, True)
        If Saves.Contains(Save) Then
            Saves.Remove(Save)
        End If
    End Sub
    Public Sub MoveSaves(Destination As SdfSaveDataDirectory)
        If Destination Is Me Then
            Throw New ArgumentException("Destination cannot be the same as the source.")
        End If
        For count = Saves.Count - 1 To 0 Step -1
            Dim item = Saves(count)
            item.CopyTo(Destination)
            DeleteSave(item)
        Next
    End Sub
    Public Sub New(Path As String)
        Me.Path = Path
        Saves = New List(Of SdfSave)
        If Not IO.Directory.Exists(Path) Then
            IO.Directory.CreateDirectory(Path)
        End If
        For Each item In IO.Directory.GetDirectories(Path, "*", IO.SearchOption.TopDirectoryOnly)
            Dim dirName = IO.Path.GetFileNameWithoutExtension(item)
            If IsNumeric(dirName) AndAlso dirName.Length = 14 Then
                Dim save As New SdfSave(item)
                Saves.Add(save)
            End If
        Next
    End Sub
End Class
