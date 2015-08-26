Imports System.Web.Script.Serialization

Public Class ObjectFile(Of T)
    Inherits GenericFile

    Public Property ContainedObject As T

    Public Sub New(Filename As String)
        Me.OriginalFilename = Filename
        Me.Filename = Filename

        Dim j As New JavaScriptSerializer
        ContainedObject = j.Deserialize(Of T)(IO.File.ReadAllText(Filename))
    End Sub

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Sub Save(Filename As String)
        Dim j As New JavaScriptSerializer
        IO.File.WriteAllText(Filename, j.Serialize(ContainedObject))
    End Sub

    Public Overrides Sub Save()
        Save(Me.Filename)
    End Sub

End Class
