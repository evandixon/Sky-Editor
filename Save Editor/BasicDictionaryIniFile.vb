Imports SkyEditor.Core.Interfaces

''' <summary>
''' Models a basic file that stores Key/Value pairs.
''' </summary>
Public Class BasicDictionaryIniFile

    Public Property Entries As Dictionary(Of Integer, String)

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
        Entries = New Dictionary(Of Integer, String)
    End Sub
End Class
