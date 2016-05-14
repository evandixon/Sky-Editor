Imports System.IO
Imports SkyEditor.Core
''' <summary>
''' A library that contains NDS Roms, as stored by most slot-1 NDS flashcarts.
''' </summary>
Public Class NDSFlashcartLibrary
    Inherits Library

    Public Sub New()
        MyBase.New
    End Sub

    Public Sub New(name As String, relativePath As String, rootPath As String)
        MyBase.New(name, relativePath, rootPath)
    End Sub

    Public Overrides Async Function GetContents(manager As PluginManager) As Task(Of IEnumerable(Of Object))
        Dim out As New List(Of Object)
        For Each item In manager.CurrentIOProvider.GetFiles(Path.Combine(Me.RootPath, Me.RelativePath), "*.nds", True)
            out.Add(Await FlashcartNDSRom.GetRom(item, manager))
        Next
        Return out
    End Function

    Public Overrides Function GetSupportedContentTypes() As IEnumerable(Of Type)
        Return {GetType(FlashcartNDSRom)}
    End Function

    Public Overrides Function AddContent(content As Object, manager As PluginManager) As Task
        'Todo: copy the ROM
        'Todo: copy the saves
        Throw New NotImplementedException
    End Function
End Class
