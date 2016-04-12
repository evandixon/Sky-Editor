Imports SkyEditorBase.Interfaces

Namespace Flashcart
    Public Class GenericFlashcart
        Implements SkyEditorBase.Interfaces.iSavable

        Public Event FileSaved As iSavable.FileSavedEventHandler Implements iSavable.FileSaved

        Public Property Drive As IO.DriveInfo
        Public Property Name As String
        Public Property ID As Guid
        Public Property Collections As List(Of FileCollection)

        ''' <summary>
        ''' Gets the collection types that this flashcart supports.
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function GetSupportedCollectionTypes() As IEnumerable(Of Type)
            Return {}
        End Function

        Public Overridable Sub Initialize(Drive As IO.DriveInfo)
            Me.Drive = Drive
            Me.Name = Drive.VolumeLabel
            Me.ID = Guid.NewGuid
        End Sub

        Public Overridable Sub Initialize(Drive As IO.DriveInfo, InfoFile As FlashcartInfoFile)
            Me.Drive = Drive
            Me.Name = InfoFile.Name
            Me.ID = Guid.Parse(InfoFile.ID)
            For Each item In InfoFile.Collections
                Dim c = FileCollection.DeSerialize(item)
                'Todo: for data integrity, ensure that c.GetType is inherits or implements something in GetSupportedCollectionTypes
                If c IsNot Nothing Then
                    Collections.Add(c)
                End If
            Next
        End Sub

        Public Sub Save() Implements iSavable.Save
            Dim info As New FlashcartInfoFile
            info.FlashcartAssemblyQualifiedTypeName = Me.GetType.AssemblyQualifiedName
            info.Name = Me.Name
            info.ID = Me.ID.ToString
            info.Save(IO.Path.Combine(Drive.RootDirectory.FullName, "info.skyfci"))
        End Sub

        Public Sub New()
            Collections = New List(Of FileCollection)
        End Sub
    End Class
End Namespace