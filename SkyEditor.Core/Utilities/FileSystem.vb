Imports SkyEditor.Core.IO

Namespace Utilities
    Public Class FileSystem
        ''' <summary>
        ''' Asynchronously copies a directory.
        ''' </summary>
        ''' <param name="sourceDirectory">The directory to copy.</param>
        ''' <param name="destinationDirectory">The new destination for the source directory.</param>
        ''' <returns></returns>
        Public Shared Async Function CopyDirectory(sourceDirectory As String, destinationDirectory As String, provider As IOProvider) As Task
            'Get the files/directories to copy
            Dim files = provider.GetFiles(sourceDirectory, "*", False)

            'Create all required directories
            For Each item In files
                Dim dest = item.Replace(sourceDirectory, destinationDirectory)
                If Not provider.DirectoryExists(Path.GetDirectoryName(dest)) Then
                    provider.CreateDirectory(Path.GetDirectoryName(dest))
                End If
            Next
            Dim f As New AsyncFor
            f.RunSynchronously = False
            Await f.RunForEach(Sub(path As String)
                                   Dim dest = path.Replace(sourceDirectory, destinationDirectory)
                                   provider.CopyFile(path, dest)
                               End Sub, files)
        End Function

        Public Shared Async Function DeleteDirectoryContents(DirectoryName As String, provider As IOProvider) As Task
            Dim f As New AsyncFor()

            'Delete the files (because recursive deletes sometimes fail
            Await f.RunForEach(Sub(Item As String)
                                   provider.DeleteFile(Item)
                               End Sub, provider.GetFiles(DirectoryName, "*", False))

            'Delete the main directory (to delete all child directories)
            provider.DeleteDirectory(DirectoryName)

            'Wait until it is fully deleted (because it seems IO.Directory.Delete is asynchronous, and can't be awaited directly)
            Await Task.Run(New Action(Sub()
                                          While provider.DirectoryExists(DirectoryName)
                                              'Block
                                          End While
                                      End Sub))

            'Recreate the main directory
            provider.CreateDirectory(DirectoryName)
        End Function

        ''' <summary>
        ''' Ensures the given diretory exists with nothing inside.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Async Function ReCreateDirectory(DirectoryName As String, provider As IOProvider) As Task
            If provider.DirectoryExists(DirectoryName) Then
                Await DeleteDirectoryContents(DirectoryName, provider)
            Else
                provider.CreateDirectory(DirectoryName)
            End If
        End Function

        ''' <summary>
        ''' Deletes the given directory and all files inside it, if it exists.
        ''' </summary>
        ''' <param name="DirectoryName"></param>
        ''' <returns></returns>
        Public Shared Async Function DeleteDirectory(DirectoryName As String, provider As IOProvider) As Task
            If provider.DirectoryExists(DirectoryName) Then
                Await DeleteDirectoryContents(DirectoryName, provider)
                provider.DeleteDirectory(DirectoryName)
            End If
        End Function

        ''' <summary>
        ''' Deletes the given file if it exists, and does nothing if it does not exist.
        ''' </summary>
        ''' <param name="Filename">Full path of the file to delete.</param>
        Public Shared Sub DeleteFile(Filename As String, provider As IOProvider)
            If provider.FileExists(Filename) Then
                provider.DeleteFile(Filename)
            End If
        End Sub
    End Class
End Namespace

