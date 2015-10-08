Imports System.Web.Script.Serialization
Imports SkyEditorBase.Interfaces

Public Class ObjectFile(Of T)
    'Inherits GenericFile
    Implements Interfaces.iOpenableFile
    Implements Interfaces.iSavable
    Implements Interfaces.iGenericFile
    Private Class JsonContainer
        Public Property ContainedObject As T

        Public Property ContainedTypeName As String
    End Class

    Public Property ContainedObject As T

    Public Property ContainedTypeName As String

    Public Property Filename As String Implements Interfaces.iGenericFile.Filename

    Public Property OriginalFilename As String Implements Interfaces.iGenericFile.OriginalFilename

    Public Property Name As String Implements Interfaces.iGenericFile.Name

#Region "Constructors"
    Public Sub New()

    End Sub

    Public Sub New(Filename As String)
        Me.New
        Me.OpenFile(Filename)
    End Sub

    Public Sub CreateFile(Name As String)
        If GetType(T).GetConstructor({}) IsNot Nothing Then
            ContainedObject = GetType(T).GetConstructor({}).Invoke({})
        End If
    End Sub

    Public Sub OpenFile(Filename As String) Implements Interfaces.iOpenableFile.OpenFile
        Me.OriginalFilename = Filename
        Me.Filename = Filename

        Dim j As New JavaScriptSerializer
        Dim c = j.Deserialize(Of JsonContainer)(IO.File.ReadAllText(Filename))
        Me.ContainedObject = c.ContainedObject
        Me.ContainedTypeName = c.ContainedTypeName
    End Sub

#End Region

#Region "iSaveableFile support"

    Public Sub Save(Filename As String) Implements Interfaces.iSavable.Save
        Dim j As New JavaScriptSerializer
        Dim c As New JsonContainer
        c.ContainedObject = Me.ContainedObject
        c.ContainedTypeName = Me.GetType.AssemblyQualifiedName 'GetType(T).AssemblyQualifiedName
        IO.File.WriteAllText(Filename, j.Serialize(c))
        RaiseFileSaved(Me, New EventArgs)
    End Sub

    Public Sub Save() Implements Interfaces.iSavable.Save
        Save(Me.Filename)
    End Sub

    Public Event FileSaved(sender As Object, e As EventArgs) Implements iSavable.FileSaved
    Protected Sub RaiseFileSaved(sender As Object, e As EventArgs)
        RaiseEvent FileSaved(sender, e)
    End Sub

#End Region

    Public Shared Function GetGenericTypeDefinition() As Type
        Return GetType(ObjectFile(Of Object)).GetGenericTypeDefinition
    End Function

    Public Shared Function IsObjectFile(TypeToCheck As Type) As Boolean
        Return TypeToCheck.GetGenericTypeDefinition.IsEquivalentTo(GetGenericTypeDefinition)
    End Function

    Public Overridable Function DefaultExtension() As String Implements Interfaces.iGenericFile.DefaultExtension
        Return ""
    End Function

End Class
