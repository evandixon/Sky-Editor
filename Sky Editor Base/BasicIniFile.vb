Imports SkyEditor.Core.Interfaces

''' <summary>
''' Models a basic file that stores Key/Value pairs.
''' </summary>
Public Class BasicIniFile
    Implements iOpenableFile

    Public Property Entries As Dictionary(Of String, String)

    Public Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
        For Each item In IO.File.ReadAllLines(Filename)
            Dim parts = item.Split("=".ToCharArray, 2)
            If Not Entries.ContainsKey(parts(0)) Then
                Entries.Add(parts(0), parts(1))
            End If
        Next
    End Sub

    Public Sub CreateFile(Contents As String)
        For Each item In Contents.Split(vbLf)
            If Not String.IsNullOrWhiteSpace(item) Then
                Dim parts = item.Trim.Split("=".ToCharArray, 2)
                If Not Entries.ContainsKey(parts(0)) Then
                    Entries.Add(parts(0), parts(1))
                End If
            End If
        Next
    End Sub

    Public Sub New()
        Entries = New Dictionary(Of String, String)
    End Sub
End Class
