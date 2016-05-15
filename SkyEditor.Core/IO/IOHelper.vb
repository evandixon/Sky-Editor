Imports System.Reflection
Imports SkyEditor.Core.Utilities

Namespace IO
    Public Class IOHelper

        Delegate Function DuplicateMatchSelector(Matches As List(Of FileTypeDetectionResult)) As FileTypeDetectionResult

        'Prevent anyone from creating an instance of this static class.
        Private Sub New()
        End Sub

        <Obsolete("Should be replaced by some sort of UI element.")> Public Shared Function PickFirstDuplicateMatchSelector(Matches As List(Of FileTypeDetectionResult)) As FileTypeDetectionResult
            Return Matches.First
        End Function

        ''' <summary>
        ''' Returns an IEnumerable of all the registered types that implement iCreatableFile.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetCreatableFileTypes(Manager As PluginManager) As IEnumerable(Of TypeInfo)
            Return Manager.GetRegisteredTypes(GetType(ICreatableFile).GetTypeInfo)
        End Function

        ''' <summary>
        ''' Returns an IEnumerable of all the registered types that implement iOpenableFile.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetOpenableFileTypes(Manager As PluginManager) As IEnumerable(Of TypeInfo)
            If Manager Is Nothing Then
                Throw New ArgumentNullException(NameOf(Manager))
            End If

            Return Manager.GetRegisteredTypes(GetType(IOpenableFile).GetTypeInfo)
        End Function

        ''' <summary>
        ''' Creates a new instance of the given file type.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function CreateNewFile(NewFileName As String, FileType As TypeInfo) As ICreatableFile
            If String.IsNullOrEmpty(NewFileName) Then
                Throw New ArgumentNullException(NameOf(NewFileName))
            End If

            If FileType Is Nothing Then
                Throw New ArgumentNullException(NameOf(FileType))
            End If

            If Not ReflectionHelpers.IsOfType(FileType, GetType(IOpenableFile).GetTypeInfo) Then
                Throw New ArgumentException(My.Resources.Language.ErrorTypeMustInheritICreatableFile, NameOf(FileType))
            End If

            If Not ReflectionHelpers.CanCreateInstance(FileType) Then
                Throw New ArgumentException(My.Resources.Language.ErrorTypeNoDefaultConstructor, NameOf(FileType))
            End If

            Dim file As ICreatableFile = ReflectionHelpers.CreateInstance(FileType)
            file.CreateFile(NewFileName)
            Return file
        End Function

        ''' <summary>
        ''' Creates a new instance of the given iOpenableFile type using the given filename.
        ''' </summary>
        ''' <param name="Filename">Filename of the file to open.</param>
        ''' <param name="FileType">Type of the class to create an instance of.  Must have a default constructor and implement iOpenableFile.</param>
        ''' <returns></returns>
        Public Shared Async Function OpenFile(Filename As String, FileType As TypeInfo, Manager As PluginManager) As Task(Of Object)
            If String.IsNullOrEmpty(Filename) Then
                Throw New ArgumentNullException(NameOf(Filename))
            End If

            If FileType Is Nothing Then
                Throw New ArgumentNullException(NameOf(FileType))
            End If

            If Manager Is Nothing Then
                Throw New ArgumentNullException(NameOf(Manager))
            End If

            If Not ReflectionHelpers.IsOfType(FileType, GetType(IOpenableFile).GetTypeInfo) Then
                Throw New ArgumentException(My.Resources.Language.ErrorTypeMustInheritIOpenableFile, NameOf(FileType))
            End If

            If ReflectionHelpers.CanCreateInstance(FileType) Then
                Dim f As IOpenableFile = ReflectionHelpers.CreateInstance(FileType)
                Await f.OpenFile(Filename, Manager.CurrentIOProvider)
                Return f
            Else
                Throw New ArgumentException(My.Resources.Language.ErrorTypeNoDefaultConstructor, NameOf(FileType))
            End If
        End Function

        ''' <summary>
        ''' Auto-detects the file/directory type and creates an instance of an appropriate class to model it.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Async Function OpenObject(path As String, duplicateFileTypeSelector As DuplicateMatchSelector, manager As PluginManager) As Task(Of Object)
            If String.IsNullOrEmpty(path) Then
                Throw New ArgumentNullException(NameOf(path))
            End If

            If duplicateFileTypeSelector Is Nothing Then
                Throw New ArgumentNullException(NameOf(duplicateFileTypeSelector))
            End If

            If manager Is Nothing Then
                Throw New ArgumentNullException(NameOf(manager))
            End If

            'Determine whether the path points to a file or a directory.
            If manager.CurrentIOProvider.FileExists(path) Then
                'Open the file for detection
                Dim g As New GenericFile(manager.CurrentIOProvider)
                g.IsReadOnly = True
                Await g.OpenFile(path, manager.CurrentIOProvider)
                Return Await OpenFile(g, duplicateFileTypeSelector, manager)
            Else
                'Open the directory
                If manager.CurrentIOProvider.DirectoryExists(path) Then
                    Return OpenDirectory(path, duplicateFileTypeSelector, manager)
                Else
                    Throw New FileNotFoundException(My.Resources.Language.ErrorFileOrDirDoesntExist)
                End If
            End If
        End Function

        ''' <summary>
        ''' Using the given file, auto-detects the file type and creates an instance of an appropriate class.
        ''' If no appropriate file can be found, will return the given File.
        ''' </summary>
        ''' <returns></returns>
        Protected Shared Async Function OpenFile(File As GenericFile, duplicateFileTypeSelector As DuplicateMatchSelector, manager As PluginManager) As Task(Of Object)
            If File Is Nothing Then
                Throw New ArgumentNullException(NameOf(File))
            End If

            If duplicateFileTypeSelector Is Nothing Then
                Throw New ArgumentNullException(NameOf(duplicateFileTypeSelector))
            End If

            If manager Is Nothing Then
                Throw New ArgumentNullException(NameOf(manager))
            End If

            Dim type = Await GetFileType(File, duplicateFileTypeSelector, manager)
            If type Is Nothing OrElse Not ReflectionHelpers.IsOfType(type, GetType(IOpenableFile).GetTypeInfo) Then
                'There is no class we found that can model this file.
                'We will instead use GenericFile.  We're recreating it so it won't be readonly.
                Dim filename = File.OriginalFilename
                File.Dispose()
                Dim g As New GenericFile
                Await g.OpenFile(File.OriginalFilename, manager.CurrentIOProvider)
                Return g
            Else
                Dim out As IOpenableFile = ReflectionHelpers.CreateInstance(type)
                Await out.OpenFile(File.OriginalFilename, manager.CurrentIOProvider)
                File.Dispose()
                Return out
            End If
        End Function

        ''' <summary>
        ''' Sometimes a "file" actually exists as multiple files in a directory.  This method will open a "file" using the given directory.  Returns Nothing if no class can model the directory at the given path.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Async Function OpenDirectory(path As String, duplicateDirectoryTypeSelector As DuplicateMatchSelector, manager As PluginManager) As Task(Of Object)

            If String.IsNullOrEmpty(path) Then
                Throw New ArgumentNullException(NameOf(path))
            End If

            If duplicateDirectoryTypeSelector Is Nothing Then
                Throw New ArgumentNullException(NameOf(duplicateDirectoryTypeSelector))
            End If

            If manager Is Nothing Then
                Throw New ArgumentNullException(NameOf(manager))
            End If

            Dim type = GetDirectoryType(path, duplicateDirectoryTypeSelector, manager)
            If type Is Nothing OrElse Not ReflectionHelpers.IsOfType(type, GetType(IOpenableFile).GetTypeInfo) Then
                'Nothing models the file.
                Return Nothing
            Else
                Dim out As IOpenableFile = ReflectionHelpers.CreateInstance(type)
                Await out.OpenFile(path, manager.CurrentIOProvider)
                Return out
            End If
        End Function

        ''' <summary>
        ''' Detects the type of the given file, or returns Nothing if no known classes that can model it exist.
        ''' </summary>
        ''' <param name="File">File of which to detect the type.</param>
        ''' <param name="Manager">Instance of the current plugin manager.</param>
        ''' <returns></returns>
        Protected Shared Async Function GetFileType(File As GenericFile, duplicateFileTypeSelector As DuplicateMatchSelector, Manager As PluginManager) As Task(Of TypeInfo)

            If File Is Nothing Then
                Throw New ArgumentNullException(NameOf(File))
            End If

            If Manager Is Nothing Then
                Throw New ArgumentNullException(NameOf(Manager))
            End If

            Dim resultSetTasks As New List(Of Task(Of IEnumerable(Of FileTypeDetectionResult)))
            For Each detector In Manager.GetRegisteredObjects(Of IFileTypeDetector)
                'Start the file type detection
                Dim detectTask = detector.DetectFileType(File, Manager)

                'Add the task to a list of running detection tasks, so we have the option of running them asynchronously.
                resultSetTasks.Add(detectTask)

                'However, the file isn't necessarily thread-safe, so if it isn't, we only want to run one detection task at a time.
                If Not File.IsThreadSafe Then
                    Await detectTask
                End If
            Next

            Dim matches As New List(Of FileTypeDetectionResult)

            'Concatinate all results
            For Each item In resultSetTasks
                matches.AddRange(item.Result)
            Next

            Return HandleFileTypeDetectionResultList(matches, duplicateFileTypeSelector)?.FileType
        End Function

        Public Shared Function GetDirectoryType(path As String, duplicateDirectoryTypeSelector As DuplicateMatchSelector, Manager As PluginManager) As TypeInfo

            If String.IsNullOrEmpty(path) Then
                Throw New ArgumentNullException(NameOf(path))
            End If

            If duplicateDirectoryTypeSelector Is Nothing Then
                Throw New ArgumentNullException(NameOf(duplicateDirectoryTypeSelector))
            End If

            If Manager Is Nothing Then
                Throw New ArgumentNullException(NameOf(Manager))
            End If

            Dim resultSetTasks As New List(Of Task(Of IEnumerable(Of FileTypeDetectionResult)))
            For Each detector In Manager.GetRegisteredObjects(Of IDirectoryTypeDetector)
                'Start the directory type detection
                Dim detectTask = detector.DetectDirectoryType(path)

                'Add the task to a list of running detection tasks, so we have the option of running them asynchronously.
                resultSetTasks.Add(detectTask)
            Next

            Dim matches As New List(Of FileTypeDetectionResult)

            'Concatinate all results
            For Each item In resultSetTasks
                matches.AddRange(item.Result)
            Next

            Return HandleFileTypeDetectionResultList(matches, duplicateDirectoryTypeSelector).FileType
        End Function

        Protected Shared Function HandleFileTypeDetectionResultList(Results As List(Of FileTypeDetectionResult), duplicateFileTypeSelector As DuplicateMatchSelector) As FileTypeDetectionResult
            If Results.Count = 0 Then
                'No classes (besides GenericFile) can model this file.
                Return Nothing
            ElseIf Results.Count = 1 Then
                Return Results(0)
            Else
                'Multiple matches exist.  Find the one with the highest chance of being the correct one.
                Dim maxChance = (From m In Results Select m.MatchChance).Max
                Dim top = (From m In Results Where m.MatchChance = maxChance).ToList
                If top.Count = 1 Then
                    'There's only one, so return the first one
                    Return top.First
                ElseIf top.Count < 1 Then
                    'Better to have an unreachable exception than to some how end up in a handler for more than 1 match, and have 0 matches to work with.
                    Throw New ArgumentOutOfRangeException(My.Resources.Language.ErrorSanityNoMax)
                Else
                    'There's more than one possible match.  Use the given selector to select one.
                    Return duplicateFileTypeSelector(top.ToList)
                End If
            End If
        End Function

    End Class
End Namespace

