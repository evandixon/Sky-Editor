Imports System.Reflection
Imports System.Threading.Tasks
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities

Namespace IO
    Public Class ObjectFile(Of T)
        Implements iNamed
        Implements IOpenableFile
        Implements ISavableAs
        Implements iOnDisk
        Implements iCreatableFile
        Implements IContainer(Of T)
        Private Class JsonContainer(Of U)
            Public Property ContainedObject As U

            Public Property ContainedTypeName As String
        End Class

        Public Sub New()
        End Sub

        Public Sub New(FileProvider As IOProvider)
            Me.CurrentIOProvider = FileProvider
        End Sub

        Public Sub New(FileProvider As IOProvider, Filename As String)
            Me.CurrentIOProvider = FileProvider
            Me.OpenFileInternal(Filename)
        End Sub

        Public Sub CreateFile(Name As String) Implements iCreatableFile.CreateFile
            If ReflectionHelpers.CanCreateInstance(GetType(T).GetTypeInfo) Then
                ContainedObject = ReflectionHelpers.CreateInstance(GetType(T).GetTypeInfo)
                _name = Name
            End If
        End Sub

        Public Function OpenFile(Filename As String, Provider As IOProvider) As Task Implements IOpenableFile.OpenFile
            Me.CurrentIOProvider = Provider
            OpenFileInternal(Filename)
            Return Task.FromResult(0)
        End Function

        Private Sub OpenFileInternal(Filename As String)
            Me.Filename = Filename

            If CurrentIOProvider.FileExists(Filename) Then
                Dim c = Json.DeserializeFromFile(Of JsonContainer(Of T))(Filename, CurrentIOProvider)
                Me.ContainedObject = c.ContainedObject
                Me.ContainedTypeName = c.ContainedTypeName
            Else
                Me.ContainedObject = ReflectionHelpers.CreateInstance(GetType(T).GetTypeInfo)
                Me.ContainedTypeName = GetType(T).AssemblyQualifiedName
            End If
        End Sub

#Region "Properties"
        Public Property ContainedObject As T Implements IContainer(Of T).Item

        Public Property ContainedTypeName As String

        Public Property Filename As String Implements iOnDisk.Filename

        Public Property CurrentIOProvider As IOProvider

        Public ReadOnly Property Name As String Implements iNamed.Name
            Get
                If String.IsNullOrEmpty(Filename) Then
                    Return _name
                Else
                    Return System.IO.Path.GetFileName(Filename)
                End If
            End Get
        End Property
        Dim _name As String
#End Region

#Region "iSaveableFile support"

        Public Sub Save(Filename As String, provider As IOProvider) Implements ISavableAs.Save
            Dim c As New JsonContainer(Of T)
            c.ContainedObject = Me.ContainedObject
            c.ContainedTypeName = Me.GetType.AssemblyQualifiedName 'GetType(T).AssemblyQualifiedName
            Json.SerializeToFile(Filename, c, provider)
            RaiseFileSaved(Me, New EventArgs)
        End Sub

        Public Sub Save(provider As IOProvider) Implements ISavable.Save
            Save(Me.Filename, provider)
        End Sub

        Public Event FileSaved(sender As Object, e As EventArgs) Implements iSavable.FileSaved
        Protected Sub RaiseFileSaved(sender As Object, e As EventArgs)
            RaiseEvent FileSaved(sender, e)
        End Sub

#End Region

        Public Shared Function GetGenericTypeDefinition() As Type
            Return GetType(ObjectFile(Of Object)).GetGenericTypeDefinition
        End Function

        Public Shared Function IsObjectFile(TypeToCheck As TypeInfo) As Boolean
            Return (TypeToCheck.IsGenericType AndAlso TypeToCheck.GetGenericTypeDefinition.Equals(GetGenericTypeDefinition)) OrElse (Not TypeToCheck.BaseType.Equals(GetType(Object)) AndAlso IsObjectFile(TypeToCheck.BaseType.GetTypeInfo))
        End Function

        Public Overridable Function GetDefaultExtension() As String Implements ISavableAs.GetDefaultExtension
            Return ""
        End Function
    End Class
End Namespace

