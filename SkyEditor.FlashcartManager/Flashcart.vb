Imports SkyEditor.Core
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities

Public Class Flashcart

    Private Class FlashcartFile
        Public Property ID As Guid
        Public Property Name As String
        Public Property Libraries As List(Of String)
        Public Sub New()
            Libraries = New List(Of String)
        End Sub
    End Class

    Public Shared Function CreateFlashcart(filename As String, flashcartName As String, provider As IOProvider) As Task(Of Flashcart)
        Dim cart As New Flashcart
        cart.Name = flashcartName
        cart.Filename = filename

        Return Task.FromResult(cart)
    End Function

    Public Shared Async Function OpenFlashcart(filename As String, manager As PluginManager) As Task(Of Flashcart)
        Dim file = Json.DeserializeFromFile(Of FlashcartFile)(filename, manager.CurrentIOProvider)
        Dim cart As New Flashcart
        cart.ID = file.ID
        cart.Name = file.Name
        cart.Filename = filename

        'I would opt to let each OpenLibrary task run asynchronously, but I don't know how the list cart.Libraries would behave, so it's safer to do it this way.
        'Todo: in the future, figure out a safe way to let each OpenLibrary task run asynchronously.
        For Each item In file.Libraries
            cart.Libraries.Add(Await Library.OpenLibrary(item, Path.GetDirectoryName(filename), manager))
        Next

        Return cart
    End Function

    Public Property ID As Guid
    Public Property Name As String
    Public Property Filename As String
    Public Property Libraries As List(Of Library)

    Public Async Function Save(provider As IOProvider) As Task
        Dim file As New FlashcartFile
        file.Name = Me.Name
        file.ID = Me.ID

        For Each item In Libraries
            file.Libraries.Add(Await item.Save)
        Next

        Json.SerializeToFile(Me.Filename, file, provider)
    End Function

    Public Function GetRootPath() As String
        Return Path.GetDirectoryName(Me.Filename)
    End Function

    Protected Sub New()
        Libraries = New List(Of Library)
    End Sub

End Class
