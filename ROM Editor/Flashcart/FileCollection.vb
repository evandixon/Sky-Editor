Namespace Flashcart
    Public MustInherit Class FileCollection
        ''' <summary>
        ''' Name of the collection
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String

        ''' <summary>
        ''' Location of the collection, relative to the root of the drive.
        ''' </summary>
        ''' <returns></returns>
        Public Property Directory As String

        ''' <summary>
        ''' Gets the Assembly Qualified Name of the FileCollection type.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property FileCollectionTypeName As String
            Get
                Return Me.GetType.AssemblyQualifiedName
            End Get
        End Property

        Public MustOverride Function GetFiles(CurrentFlashcart As GenericFlashcart) As IEnumerable(Of Object)

        Public Function Serialize() As String
            Dim container As New FileCollectionContainer
            container.CollectionTypeName = Me.GetType.AssemblyQualifiedName
            container.Name = Me.Name
            container.Directory = Directory
            Return SkyEditorBase.Utilities.Json.Serialize(container)
        End Function

        Public Overridable Sub Initialize(Container As FileCollectionContainer)
            Name = Container.Name
            Directory = Container.Directory
        End Sub

        Public Shared Function DeSerialize(Json As String) As FileCollection
            Dim container = SkyEditorBase.Utilities.Json.Deserialize(Of FileCollectionContainer)(Json)
            Dim type = SkyEditorBase.Utilities.ReflectionHelpers.GetTypeFromName(container.CollectionTypeName)
            If type IsNot Nothing AndAlso type.GetConstructor({}) IsNot Nothing Then
                Dim out As FileCollection = type.GetConstructor({}).Invoke({})
                out.Initialize(container)
                Return out
            Else
                Return Nothing
            End If
        End Function

    End Class

End Namespace

