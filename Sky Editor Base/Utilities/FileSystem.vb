Imports System.Threading.Tasks

Namespace Utilities
    Public Class FileSystem

        Public Shared Async Function CopyDirectory(SourceDirectory As String, DestinationDirectory As String, Optional UpdateLoadingStatus As Boolean = False) As Task
            Dim f As New AsyncFileCopier
            f.SetLoadingStatus = UpdateLoadingStatus
            f.SetLoadingStatusOnFinish = UpdateLoadingStatus
            Await f.CopyDirectory(SourceDirectory, DestinationDirectory)
        End Function

        Public Shared Async Function DeleteDirectoryContents(DirectoryName As String, Optional UpdateLoadingStatus As Boolean = False) As Task
            Dim f As New AsyncFor(My.Resources.Language.LoadingDeletingFiles)
            f.SetLoadingStatus = UpdateLoadingStatus
            f.SetLoadingStatusOnFinish = UpdateLoadingStatus

            Await f.RunForEach(Sub(Item As String)
                                   IO.File.Delete(Item)
                               End Sub, IO.Directory.GetFiles(DirectoryName, "*", IO.SearchOption.AllDirectories))

            'Await Task.Run(Sub()
            'For Each item In IO.Directory.GetDirectories(DirectoryName, "*", IO.SearchOption.AllDirectories)
            '    IO.Directory.Delete(item, True)
            'Next
            'End Sub)
            IO.Directory.Delete(DirectoryName, True)

            Await Task.Run(New Action(Sub()
                                          While IO.Directory.Exists(DirectoryName)
                                              'Block
                                          End While
                                      End Sub))

            IO.Directory.CreateDirectory(DirectoryName)

            'Await f.RunForEach(Sub(Item As String)
            '                       IO.Directory.Delete(Item)
            '                   End Sub, IO.Directory.GetDirectories(DirectoryName, "*", IO.SearchOption.AllDirectories))
        End Function

        ''' <summary>
        ''' Ensures the given diretory exists with nothing inside.
        ''' </summary>
        ''' <param name="DirectoryName"></param>
        ''' <param name="UpdateLoadingStatus"></param>
        ''' <returns></returns>
        Public Shared Async Function ReCreateDirectory(DirectoryName As String, Optional UpdateLoadingStatus As Boolean = False) As Task
            If IO.Directory.Exists(DirectoryName) Then
                Await DeleteDirectoryContents(DirectoryName)
            Else
                IO.Directory.CreateDirectory(DirectoryName)
            End If
        End Function

        ''' <summary>
        ''' Deletes the given directory and all files inside it.
        ''' </summary>
        ''' <param name="DirectoryName"></param>
        ''' <returns></returns>
        Public Shared Async Function DeleteDirectory(DirectoryName As String, Optional UpdateLoadingStatus As Boolean = False) As Task
            Await DeleteDirectoryContents(DirectoryName, UpdateLoadingStatus)
            IO.Directory.Delete(DirectoryName, True)
        End Function

        ''' <summary>
        ''' Deletes the given file if it exists, and does nothing if it does not exist.
        ''' </summary>
        ''' <param name="Filename">Full path of the file to delete.</param>
        Public Shared Sub DeleteFile(Filename As String)
            If IO.File.Exists(Filename) Then
                IO.File.Delete(Filename)
            End If
        End Sub
    End Class
End Namespace