Imports System.Text
Imports SkyEditorBase

Namespace FileFormats
    Public Class NDSModSource
        Inherits GenericFile
        Dim _filename As String
        Public Property Settings As Dictionary(Of String, String)

        Public Overrides Function DefaultExtension() As String
            Return "*.ndsmodsrc"
        End Function
        Public Sub New()
            MyBase.New()
            Settings = New Dictionary(Of String, String)
            Settings.Add("Name", "")
            Settings.Add("Author", "")
            Settings.Add("Description", "")
            Settings.Add("UpdateUrl", "")
            Settings.Add("DependenciesBefore", "")
            Settings.Add("DependenciesAfter", "")
        End Sub
        Public Sub New(Filename As String)
            Me.New
            _filename = Filename
            Dim lines = IO.File.ReadAllText(Filename).Split(vbLf)
            For Each item In lines
                Dim parts = item.Split("=".ToCharArray, 2)
                If parts.Count > 1 Then
                    If Not Settings.ContainsKey(parts(0).Trim) Then
                        Settings.Add(parts(0).Trim, parts(1).Trim)
                    Else
                        Settings(parts(0).Trim) = parts(1).Trim
                    End If
                End If
            Next
        End Sub
        Public Overrides Sub PreSave()

        End Sub
        Public Overrides Sub Save(Destination As String)
            Dim buffer As New StringBuilder
            For Each item In Settings
                buffer.Append(item.Key)
                buffer.Append("=")
                buffer.Append(item.Value)
                buffer.Append(vbCrLf)
            Next
            IO.File.WriteAllText(Destination, buffer.ToString)
        End Sub
        Public Overrides Sub Save()
            Save(_filename)
        End Sub
    End Class
End Namespace
