Imports System.Windows.Media.Imaging
Imports SkyEditorBase

Public Class ROM
    Public Property Name As String
    Public ReadOnly Property Filename As String
        Get
            Dim romDirectory As String = PluginHelper.GetResourceName("Roms/NDS/")
            Return IO.Path.Combine(romDirectory, Name.Replace(":", ""))
        End Get
    End Property
    Public ReadOnly Property ImageUri As Uri
        Get
            If IO.File.Exists(Filename) Then
                Dim newpath = IO.Path.Combine(PluginHelper.GetResourceName("Temp"), IO.Path.GetFileNameWithoutExtension(Name.Replace(":", "")) & ".bmp")
                If Not IO.File.Exists(newpath) Then
                    If Not IO.Directory.Exists(IO.Path.GetDirectoryName(newpath)) Then
                        IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(newpath))
                    End If
                    ExtractIcon(Filename, newpath)
                End If
                Return New Uri(newpath)
            Else
                Return Nothing
            End If
        End Get
    End Property
    Public ReadOnly Property ImageSource As BitmapImage
        Get
            Dim u = ImageUri
            If u IsNot Nothing AndAlso IO.File.Exists(u.AbsolutePath) Then
                Return New BitmapImage(u)
            Else
                Return New BitmapImage
            End If
        End Get
    End Property
    Public Sub New(Name As String)
        Me.Name = Name
    End Sub
    Public Sub New()
        Me.New("")
    End Sub
End Class