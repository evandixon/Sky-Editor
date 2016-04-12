Imports System.Threading.Tasks
Namespace Utilities
    Public Class AsyncFileCopier
        Public Sub New()
            SetLoadingStatus = True
            SetLoadingStatusOnFinish = True
        End Sub
        ''' <summary>
        ''' Gets or sets whether or not progress will be reported to PluginHelper.SetLoadingStatus after each file copy.
        ''' </summary>
        ''' <returns></returns>
        Public Property SetLoadingStatus As Boolean

        ''' <summary>
        ''' Gets or sets whether or not progress will be set to "Ready" once the file copy is complete.
        ''' </summary>
        ''' <returns></returns>
        Public Property SetLoadingStatusOnFinish As Boolean

        ''' <summary>
        ''' Asynchronously copies a directory.
        ''' </summary>
        ''' <param name="sourceDirectory">The directory to copy.</param>
        ''' <param name="destinationDirectory">The new destination for the source directory.</param>
        ''' <returns></returns>
        Public Async Function CopyDirectory(sourceDirectory As String, destinationDirectory As String) As Task
            Dim files = IO.Directory.GetFiles(sourceDirectory, "*", IO.SearchOption.AllDirectories)
            Dim tasks As New List(Of Task)
            FileCopyCompleted = 0
            _fileCopyMax = files.Length
            For Each item In files
                Dim dest = item.Replace(sourceDirectory, destinationDirectory)
                If Not IO.Directory.Exists(IO.Path.GetDirectoryName(dest)) Then
                    IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(dest))
                End If
            Next
            For count = 0 To files.Count - 1
                Dim fileIndex As Integer = count
                Dim t = Task.Run(New Action(Sub()
                                                Dim item = files(fileIndex)
                                                Dim dest = item.Replace(sourceDirectory, destinationDirectory)
                                                IO.File.Copy(item, dest, True)
                                                FileCopyCompleted += 1
                                            End Sub))
                tasks.Add(t)
            Next
            Await Task.WhenAll(tasks)
        End Function
        Private Property FileCopyCompleted As Integer
            Get
                Return _fileCopyCompleted
            End Get
            Set(value As Integer)
                _fileCopyCompleted = value
                UpdatePortraitFixLoading(value, _fileCopyMax)
            End Set
        End Property
        Private _fileCopyCompleted As Integer
        Private _fileCopyMax As Integer
        Private Sub UpdatePortraitFixLoading(Completed As Integer, Max As Integer)
            If Completed < Max Then
                If SetLoadingStatus Then
                    PluginHelper.SetLoadingStatus(String.Format(My.Resources.Language.GenericLoadingXofY, My.Resources.Language.LoadingCopyingFiles, Completed, Max), Completed / Max)
                End If
            Else
                If SetLoadingStatusOnFinish Then
                    PluginHelper.SetLoadingStatusFinished()
                End If
            End If
        End Sub
    End Class

End Namespace
