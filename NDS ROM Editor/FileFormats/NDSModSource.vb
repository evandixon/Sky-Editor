Imports System.Text
Imports SkyEditorBase

Namespace FileFormats
    Public Class NDSModSource
        Inherits GenericFile
        Dim _filename As String
        Public Property Settings As Dictionary(Of String, String)
        Public Property ModName As String
            Get
                If Not Settings.ContainsKey("Name") Then
                    Settings.Add("Name", "")
                End If
                Return Settings("Name")
            End Get
            Set(ByVal value As String)
                If Settings.ContainsKey("Name") Then
                    Settings("Name") = value
                Else
                    Settings.Add("Name", value)
                End If
            End Set
        End Property
        Public Property Author As String
            Get
                If Not Settings.ContainsKey("Author") Then
                    Settings.Add("Author", "")
                End If
                Return Settings("Author")
            End Get
            Set(ByVal value As String)
                If Settings.ContainsKey("Author") Then
                    Settings("Author") = value
                Else
                    Settings.Add("Author", value)
                End If
            End Set
        End Property
        Public Property Description As String
            Get
                If Not Settings.ContainsKey("Description") Then
                    Settings.Add("Description", "")
                End If
                Return Settings("Description")
            End Get
            Set(ByVal value As String)
                If Settings.ContainsKey("Description") Then
                    Settings("Description") = value
                Else
                    Settings.Add("Description", value)
                End If
            End Set
        End Property
        Public Property UpdateURL As String
            Get
                If Not Settings.ContainsKey("UpdateURL") Then
                    Settings.Add("UpdateURL", "")
                End If
                Return Settings("UpdateURL")
            End Get
            Set(ByVal value As String)
                If Settings.ContainsKey("UpdateURL") Then
                    Settings("UpdateURL") = value
                Else
                    Settings.Add("UpdateURL", value)
                End If
            End Set
        End Property
        Public Property DependenciesBefore As String
            Get
                If Not Settings.ContainsKey("DependenciesBefore") Then
                    Settings.Add("DependenciesBefore", "")
                End If
                Return Settings("DependenciesBefore")
            End Get
            Set(ByVal value As String)
                If Settings.ContainsKey("DependenciesBefore") Then
                    Settings("DependenciesBefore") = value
                Else
                    Settings.Add("DependenciesBefore", value)
                End If
            End Set
        End Property
        Public Property DependenciesAfter As String
            Get
                If Not Settings.ContainsKey("DependenciesAfter") Then
                    Settings.Add("DependenciesAfter", "")
                End If
                Return Settings("DependenciesAfter")
            End Get
            Set(ByVal value As String)
                If Settings.ContainsKey("DependenciesAfter") Then
                    Settings("DependenciesAfter") = value
                Else
                    Settings.Add("DependenciesAfter", value)
                End If
            End Set
        End Property

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
            Me.OriginalFilename = Filename
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
