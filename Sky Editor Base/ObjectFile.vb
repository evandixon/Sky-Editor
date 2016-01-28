Imports SkyEditorBase.Interfaces

Public Class ObjectFile(Of T)
    'Inherits GenericFile
    Implements iNamed
    Implements iOpenableFile
    Implements ISavableAs
    Implements iOnDisk
    Implements iCreatableFile
    Implements iContainer(Of T)
    Private Class JsonContainer(Of U)
        Public Property ContainedObject As U

        Public Property ContainedTypeName As String
    End Class

    Public Property ContainedObject As T Implements iContainer(Of T).Item

    Public Property ContainedTypeName As String

    Public Property Filename As String Implements iOnDisk.Filename

    Public ReadOnly Property Name As String Implements iNamed.Name
        Get
            If String.IsNullOrEmpty(Filename) Then
                Return _name
            Else
                Return IO.Path.GetFileName(Filename)
            End If
        End Get
    End Property
    Dim _name As String

#Region "Constructors"
    Public Sub New()

    End Sub

    Public Sub New(Filename As String)
        Me.New
        Me.OpenFile(Filename)
    End Sub

    Public Sub CreateFile(Name As String) Implements iCreatableFile.CreateFile
        If GetType(T).GetConstructor({}) IsNot Nothing Then
            ContainedObject = GetType(T).GetConstructor({}).Invoke({})
            _name = Name
        End If
    End Sub

    Public Sub OpenFile(Filename As String) Implements Interfaces.iOpenableFile.OpenFile
        Me.Filename = Filename

        If IO.File.Exists(Filename) Then
            Dim c = Utilities.Json.DeserializeFromFile(Of JsonContainer(Of T))(Filename)
            Me.ContainedObject = c.ContainedObject
            Me.ContainedTypeName = c.ContainedTypeName
        Else
            Me.ContainedObject = GetType(T).GetConstructor({}).Invoke({})
            Me.ContainedTypeName = GetType(T).AssemblyQualifiedName
        End If
    End Sub

#End Region

#Region "iSaveableFile support"

    Public Sub Save(Filename As String) Implements Interfaces.ISavableAs.Save
        Dim c As New JsonContainer(Of T)
        c.ContainedObject = Me.ContainedObject
        c.ContainedTypeName = Me.GetType.AssemblyQualifiedName 'GetType(T).AssemblyQualifiedName
        Utilities.Json.SerializeToFile(Filename, c)
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
        Return (TypeToCheck.IsGenericType AndAlso TypeToCheck.GetGenericTypeDefinition.IsEquivalentTo(GetGenericTypeDefinition)) OrElse (Not TypeToCheck.BaseType = GetType(Object) AndAlso IsObjectFile(TypeToCheck.BaseType))
    End Function

    Public Overridable Function DefaultExtension() As String Implements iSavable.DefaultExtension
        Return ""
    End Function
End Class
