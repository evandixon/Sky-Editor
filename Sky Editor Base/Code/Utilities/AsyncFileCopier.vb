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

        Public Async Function CopyDirectory(SourceDirectory As String, DestinationDirectory As String) As Task
            Dim files = IO.Directory.GetFiles(SourceDirectory, "*", IO.SearchOption.AllDirectories)
            Dim tasks As New List(Of Task)
            FileCopyCompleted = 0
            _fileCopyMax = files.Length
            For count = 0 To files.Count - 1
                Dim fileIndex As Integer = count
                Dim t As New Task(New Action(Sub()
                                                 Dim item = files(fileIndex)
                                                 If Not IO.Directory.Exists(IO.Path.GetDirectoryName(item.Replace(SourceDirectory, DestinationDirectory))) Then
                                                     IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(item.Replace(SourceDirectory, DestinationDirectory)))
                                                 End If
                                                 IO.File.Copy(item, item.Replace(SourceDirectory, DestinationDirectory))
                                                 FileCopyCompleted += 1
                                             End Sub))
                t.Start()
                tasks.Add(t)
            Next
            Await Task.Run(New Action(Sub()
                                          Task.WaitAll(tasks.ToArray)
                                      End Sub))
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
                If SetLoadingStatus Then PluginHelper.SetLoadingStatus(String.Format(PluginHelper.GetLanguageItem("CopyingFilesStatus", "Copying files... ({0} of {1})"), Completed, Max), Completed / Max)
            Else
                If SetLoadingStatusOnFinish Then PluginHelper.SetLoadingStatusFinished()
            End If
        End Sub
    End Class

End Namespace
