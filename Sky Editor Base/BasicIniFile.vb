Imports System.Threading.Tasks
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO

''' <summary>
''' Models a basic file that stores Key/Value pairs.
''' </summary>
Public Class BasicIniFile
    Implements IOpenableFile

    Public Property Entries As Dictionary(Of String, String)

    Public Function OpenFile(Filename As String, Provider As IOProvider) As Task Implements IOpenableFile.OpenFile
        For Each item In IO.File.ReadAllLines(Filename)
            Dim parts = item.Split("=".ToCharArray, 2)
            If Not Entries.ContainsKey(parts(0)) Then
                Entries.Add(parts(0), parts(1))
            End If
        Next
        Return Task.CompletedTask
    End Function

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
