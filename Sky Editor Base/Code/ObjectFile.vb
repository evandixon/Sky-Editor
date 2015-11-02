Imports System.Web.Script.Serialization
Imports SkyEditorBase.Interfaces

Public Class ObjectFile(Of T)
    'Inherits GenericFile
    Implements iNamed
    Implements iOpenableFile
    Implements iSavable
    Implements iOnDisk
    Private Class JsonContainer
        Public Property ContainedObject As T

        Public Property ContainedTypeName As String
    End Class

    Public Property ContainedObject As T

    Public Property ContainedTypeName As String

    Public Property Filename As String

    Public Property OriginalFilename As String Implements iOnDisk.Filename

    Public Property Name As String Implements iNamed.Name

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

        If IO.File.Exists(Filename) Then
            Dim j As New JavaScriptSerializer
            Dim c = j.Deserialize(Of JsonContainer)(IO.File.ReadAllText(Filename))
            Me.ContainedObject = c.ContainedObject
            Me.ContainedTypeName = c.ContainedTypeName
        Else
            Me.ContainedObject = GetType(T).GetConstructor({}).Invoke({})
            Me.ContainedTypeName = GetType(T).AssemblyQualifiedName
        End If
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
        Return TypeToCheck.IsGenericType AndAlso TypeToCheck.GetGenericTypeDefinition.IsEquivalentTo(GetGenericTypeDefinition)
    End Function

    Public Overridable Function DefaultExtension() As String Implements iSavable.DefaultExtension
        Return ""
    End Function
End Class
