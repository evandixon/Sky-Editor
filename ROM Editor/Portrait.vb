Imports SkyEditor.Core.Windows
Imports SkyEditorBase

Public Class Portrait
    Public Property Name As String
    Public Property Filename As String
    Public ReadOnly Property ImageUri As Uri
        Get
            Dim newPath As String = IO.Path.Combine(EnvironmentPaths.GetResourceName("Temp"), Guid.NewGuid.ToString & ".png")
            IO.File.Copy(Filename, newPath, True)
            Return New Uri(newPath)
        End Get
    End Property
    Public Sub New(Name As String, Filename As String)
        Me.Name = Name
        Me.Filename = Filename
    End Sub
    Public Sub New()
        Me.New("", "")
    End Sub
End Class