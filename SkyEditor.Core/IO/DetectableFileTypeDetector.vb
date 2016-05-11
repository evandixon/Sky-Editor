Imports System.Reflection

Namespace IO
    ''' <summary>
    ''' Detects the type of a file using all registered IDetectableFileTypes.
    ''' </summary>
    Public Class DetectableFileTypeDetector
        Implements IFileTypeDetector

        Public Async Function DetectFileType(File As GenericFile, Manager As PluginManager) As Task(Of IEnumerable(Of FileTypeDetectionResult)) Implements IFileTypeDetector.DetectFileType
            Dim matches As New Concurrent.ConcurrentQueue(Of FileTypeDetectionResult)
            Dim f As New Utilities.AsyncFor
            f.RunSynchronously = Not File.IsThreadSafe
            Await f.RunForEach(Async Function(detectable As IDetectableFileType) As Task
                                   If Await detectable.IsOfType(File) Then
                                       matches.Enqueue(New FileTypeDetectionResult With {.FileType = detectable.GetType.GetTypeInfo, .MatchChance = 0.5})
                                   End If
                               End Function, Manager.GetRegisteredObjects(Of IDetectableFileType))
            Return matches
        End Function
    End Class
End Namespace

