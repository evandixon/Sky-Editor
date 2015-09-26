Imports System.Web.Script.Serialization

Public Class ObjectFile(Of T)
    Inherits GenericFile

    Private Class JsonContainer(Of T)
        Public Property ContainedObject As T

        Public Property ContainedTypeName As String
    End Class

    Public Property ContainedObject As T

    Public Property ContainedTypeName As String

    Public Sub New(Filename As String)
        Me.OriginalFilename = Filename
        Me.Filename = Filename

        Dim j As New JavaScriptSerializer
        Dim c = j.Deserialize(Of JsonContainer(Of T))(IO.File.ReadAllText(Filename))
        Me.ContainedObject = c.ContainedObject
        Me.ContainedTypeName = c.ContainedTypeName
    End Sub

    Public Sub New()

    End Sub

    Public Overrides Sub Save(Filename As String)
        Dim j As New JavaScriptSerializer
        Dim c As New JsonContainer(Of T)
        c.ContainedObject = Me.ContainedObject
        c.ContainedTypeName = GetType(T).FullName
        IO.File.WriteAllText(Filename, j.Serialize(c))
        RaiseFileSaved(Me, New EventArgs)
    End Sub

    Public Overrides Sub Save()
        Save(Me.Filename)
    End Sub

    ''' <summary>
    ''' If the given file is of type ObjectFile, returns the contained Type.
    ''' Otherwise, returns Nothing.
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <returns></returns>
    Public Shared Function TryGetType(Filename As String) As Type
        Try
            Dim f As New ObjectFile(Of Object)(Filename)
            Return Type.GetType(f.ContainedTypeName, False)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    ''' <summary>
    ''' If the given file is of type ObjectFile, returns the contained Type.
    ''' Otherwise, returns Nothing.
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function TryGetType(File As GenericFile) As Type
        Return TryGetType(File.OriginalFilename)
    End Function

End Class
