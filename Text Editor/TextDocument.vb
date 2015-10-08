Imports SkyEditorBase

Public Class AsciiTextDocument
    Inherits GenericFile
    Implements Interfaces.iCreatableFile
    Implements Interfaces.iOpenableFile
    Implements Interfaces.iModifiable
    Implements iTextFile

    Public Sub New()
        MyBase.New()
        Me.Text = String.Empty
    End Sub
    Public Sub New(Filename As String)
        MyBase.New(Filename)
        ReadText()
    End Sub
    Public Overrides Sub CreateFile(Name As String) Implements Interfaces.iCreatableFile.CreateFile
        Me.Text = String.Empty
        Me.Name = Name
    End Sub
    Public Overrides Sub OpenFile(Filename As String) Implements Interfaces.iOpenableFile.OpenFile
        MyBase.OpenFile(Filename)
        ReadText()
    End Sub
    Private Sub ReadText()
        Dim e As New Text.ASCIIEncoding
        Text = e.GetString(RawData)
    End Sub

    Public Property Text As String Implements iTextFile.Text
        Get
            Return _text
        End Get
        Set(value As String)
            _text = value
        End Set
    End Property
    Dim _text As String

    Protected Overrides Sub PreSave()
        MyBase.PreSave()
        Dim e As New Text.ASCIIEncoding
        Dim buffer = e.GetBytes(Text)
        Length = buffer.Length
        RawData = buffer
    End Sub

    Public Overrides Function DefaultExtension() As String
        Return ".txt"
    End Function

    Public Shared Function IsFileOfType(File As GenericFile) As Boolean
        Return File.OriginalFilename.ToLower.EndsWith(".txt")
    End Function

End Class
