Imports System.Web.Script.Serialization

Public Class ObjectFile(Of T)
    Inherits GenericFile
    Implements Interfaces.iOpenableFile

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
        If GetType(T).GetConstructor({}) IsNot Nothing Then
            ContainedObject = GetType(T).GetConstructor({}).Invoke({})
        End If
    End Sub

    Public Overrides Sub Save(Filename As String)
        Dim j As New JavaScriptSerializer
        Dim c As New JsonContainer(Of T)
        c.ContainedObject = Me.ContainedObject
        c.ContainedTypeName = Me.GetType.AssemblyQualifiedName 'GetType(T).AssemblyQualifiedName
        IO.File.WriteAllText(Filename, j.Serialize(c))
        RaiseFileSaved(Me, New EventArgs)
    End Sub

    Public Overrides Sub Save()
        Save(Me.Filename)
    End Sub

    Public Shared Function GetGenericTypeDefinition() As Type
        Return GetType(ObjectFile(Of Object)).GetGenericTypeDefinition
    End Function

    Public Shared Function IsObjectFile(TypeToCheck As Type) As Boolean
        Return TypeToCheck.GetGenericTypeDefinition.IsEquivalentTo(GetGenericTypeDefinition)
    End Function

End Class
