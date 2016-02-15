Imports System.Reflection
Imports System.Threading.Tasks
Imports System.Runtime.CompilerServices
Imports System.Deployment.Application
Imports SkyEditorBase.Interfaces

''' <summary>
''' A collection of methods that are useful to Sky Editor plugins.
''' </summary>
''' <remarks></remarks>
Public Class PluginHelper

    ''' <summary>
    ''' Contains a reference to the last PluginManager created.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property PluginManagerInstance As PluginManager

#Region "Resources"
    ''' <summary>
    ''' Combines the given path with your plugin's resource directory.
    ''' </summary>
    ''' <param name="Path"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetResourceName(Path As String) As String
        Dim fullPath = IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins/", Assembly.GetCallingAssembly.GetName.Name, Path)
        Dim baseDir = IO.Path.GetDirectoryName(fullPath)
        If Not IO.Directory.Exists(baseDir) Then
            IO.Directory.CreateDirectory(baseDir)
        End If
        Return fullPath
    End Function

    Public Shared Function GetResourceName(Path As String, Plugin As String) As String
        Dim fullPath = IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins/", Plugin, Path)
        Dim baseDir = IO.Path.GetDirectoryName(fullPath)
        If Not IO.Directory.Exists(baseDir) Then
            IO.Directory.CreateDirectory(baseDir)
        End If
        Return fullPath
    End Function
    ''' <summary>
    ''' Returns your plugin's resource directory as managed by Sky Editor.
    ''' It will be created if it does not exist.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetResourceDirectory() As String
        Dim baseDir = IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", Assembly.GetCallingAssembly.GetName.Name.Replace("_plg", ""))
        If Not IO.Directory.Exists(baseDir) Then
            IO.Directory.CreateDirectory(baseDir)
        End If
        Return baseDir
    End Function
    Public Shared Function GetResourceDirectory(AssemblyName As String) As String
        Dim baseDir = IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", AssemblyName.Replace("_plg", ""))
        If Not IO.Directory.Exists(baseDir) Then
            IO.Directory.CreateDirectory(baseDir)
        End If
        Return baseDir
    End Function

    ''' <summary>
    ''' Returns a the path of the root resource directory and creates it if it doesn't exist.
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function RootResourceDirectory() As String
        Dim d As String
        If ApplicationDeployment.IsNetworkDeployed Then
            'I'm choosing not to verify if the folder exists because I'm already going to check below.
            d = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create)
        Else
            d = IO.Path.Combine(Environment.CurrentDirectory & "\Resources")
            If Not IO.Directory.Exists(d) Then
                IO.Directory.CreateDirectory(d)
            End If
        End If
        Return d
    End Function

    ''' <summary>
    ''' Gets absolute paths of all the assemblies in the plugin directory.
    ''' Not all of these are guarenteed to be supported plugins.
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetPluginAssemblies() As List(Of String)
        Dim FromFolder = IO.Path.Combine(RootResourceDirectory, "Plugins")
        Dim assemblyPaths As New List(Of String)
        For Each item In IO.Directory.GetFiles(FromFolder, "*.dll")
            If Not assemblyPaths.Contains(item) Then
                assemblyPaths.Add(item)
            End If
        Next
        For Each item In IO.Directory.GetFiles(FromFolder, "*.exe")
            If Not assemblyPaths.Contains(item) Then
                assemblyPaths.Add(item)
            End If
        Next
        Return assemblyPaths
    End Function
#End Region

#Region "Translation"
    ''' <summary>
    ''' Gets the text specified by the currently loaded language files.
    ''' </summary>
    ''' <param name="Key">The name of the language item to load.</param>
    ''' <param name="DefaultValue">If the currently selected language and the English language files both do not contain the requested Key,
    ''' this value will be returned and written to the English language files in your plugin's resource directory.
    ''' If Nothing (or not provided), will be set to Key in the event it's needed.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetLanguageItem(Key As String, Optional DefaultValue As String = Nothing, Optional CallingAssembly As String = Nothing) As String
        If CallingAssembly Is Nothing Then
            CallingAssembly = Assembly.GetCallingAssembly.GetName.Name
        End If
        If SettingsManager.Instance.Settings.DebugLanguagePlaceholders Then
            Return String.Format("[{0}]", Key)
        Else
            Return Language.LanguageManager.GetLanguageItem(Key, CallingAssembly, DefaultValue)
        End If
    End Function

    Public Shared Function PluginsToInstallDirectory() As String
        Dim d = IO.Path.Combine(RootResourceDirectory, "ToInstall")
        If Not IO.Directory.Exists(d) Then
            IO.Directory.CreateDirectory(d)
        End If
        Return d
    End Function
#End Region

#Region "Program Running"
    ''' <summary>
    ''' Posted by brendan at http://stackoverflow.com/questions/9996709/read-console-process-output
    ''' </summary>
    ''' <param name="sendingProcess"></param>
    ''' <param name="outLine"></param>
    ''' <remarks></remarks>
    Private Shared Sub OutputHandler(sendingProcess As Object, outLine As DataReceivedEventArgs)
        ' Collect the sort command output.
        If Not String.IsNullOrEmpty(outLine.Data) Then
            PluginHelper.Writeline(outLine.Data, LineType.ConsoleOutput)
        End If
    End Sub
    ''' <summary>
    ''' Runs the specified program synchronously, capturing console output.
    ''' Returns true when the program exits.
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <param name="Arguments"></param>
    ''' <remarks></remarks>
    Public Shared Sub RunProgramSync(Filename As String, Arguments As String, Optional ShowLoadingWindow As Boolean = True)
        Writeline(String.Format(PluginHelper.GetLanguageItem("Executing {0} {1}", "Executing {0} {1}"), Filename, Arguments))
        Dim p As New Process()
        p.StartInfo.FileName = Filename
        p.StartInfo.Arguments = Arguments
        p.StartInfo.RedirectStandardOutput = True
        p.StartInfo.UseShellExecute = False
        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        p.StartInfo.CreateNoWindow = True
        AddHandler p.OutputDataReceived, AddressOf OutputHandler
        p.Start()
        p.BeginOutputReadLine()

        If ShowLoadingWindow Then
            SetLoadingStatus(String.Format(PluginHelper.GetLanguageItem("WaitingOnTask", "Waiting on {0}..."), IO.Path.GetFileName(Filename)))

            p.WaitForExit()


            RemoveHandler p.OutputDataReceived, AddressOf OutputHandler
        Else
            p.WaitForExit()
        End If
        p.Dispose()
        Writeline(String.Format(PluginHelper.GetLanguageItem("""{0}"" finished running."), p.StartInfo.FileName))
    End Sub
    ''' <summary>
    ''' Runs the specified program, capturing console output.
    ''' Returns true when the program exits.
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <param name="Arguments"></param>
    ''' <remarks></remarks>
    Public Shared Async Function RunProgram(Filename As String, Arguments As String, Optional ShowLoadingWindow As Boolean = True) As Task(Of Boolean)
        Writeline(String.Format(PluginHelper.GetLanguageItem("Executing {0} {1}", "Executing {0} {1}"), Filename, Arguments))
        Dim p As New Process()
        p.StartInfo.FileName = Filename
        p.StartInfo.Arguments = Arguments
        p.StartInfo.RedirectStandardOutput = True
        p.StartInfo.UseShellExecute = False
        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        p.StartInfo.CreateNoWindow = True
        p.StartInfo.WorkingDirectory = IO.Path.GetDirectoryName(Filename)
        AddHandler p.OutputDataReceived, AddressOf OutputHandler
        p.Start()
        p.BeginOutputReadLine()

        If ShowLoadingWindow Then
            SetLoadingStatus(String.Format(PluginHelper.GetLanguageItem("WaitingOnTask", "Waiting on {0}..."), IO.Path.GetFileName(Filename)))

            Await WaitForProcess(p)

            SetLoadingStatusFinished()
            RemoveHandler p.OutputDataReceived, AddressOf OutputHandler
            p.Dispose()
            Writeline(String.Format(PluginHelper.GetLanguageItem("""{0}"" finished running."), p.StartInfo.FileName))
        Else
            If Await WaitForProcess(p) Then
                RemoveHandler p.OutputDataReceived, AddressOf OutputHandler
                p.Dispose()
                Writeline(String.Format(PluginHelper.GetLanguageItem("""{0}"" finished running."), p.StartInfo.FileName))
            End If
        End If
        Return True
    End Function

    Private Shared Async Function WaitForProcess(p As Process) As Task(Of Boolean)
        Return Await Task.Run(Function()
                                  p.WaitForExit()
                                  Return True
                              End Function)
    End Function
    ''' <summary>
    ''' Runs the specified program without waiting for it to complete.
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <param name="Arguments"></param>
    ''' <remarks></remarks>
    Public Shared Sub RunProgramInBackground(Filename As String, Arguments As String)
        Writeline(String.Format(PluginHelper.GetLanguageItem("(Async) Executing ""{0}"" ""{1}"""), Filename, Arguments))
        Dim p As New Process()
        p.StartInfo.FileName = Filename
        p.StartInfo.Arguments = Arguments
        p.Start()
    End Sub
#End Region

#Region "Loading"
    Public Class LoadingMessageChangedEventArgs
        Inherits EventArgs
        Public Property NewMessage As String
        Public Property Progress As Single
        Public Property IsIndeterminate As Boolean
        Public Sub New()
            MyBase.New()
            Progress = 1
            IsIndeterminate = True
        End Sub
        Public Sub New(Message As String)
            Me.New()
            NewMessage = Message
        End Sub
        Public Sub New(Message As String, Progress As Single)
            Me.New(Message)
            Me.Progress = Progress
            Me.IsIndeterminate = False
        End Sub
    End Class
    Private Shared _loadingShown As Boolean = False
    Private Shared _loadingDefinitions As New Dictionary(Of String, LoadingMessageChangedEventArgs)

    '''' <summary>
    '''' Shows a loading window until the same function calls StopLoading.
    '''' </summary>
    '''' <param name="Message">The message to be displayed while loading.  This message will not be translated so you should translate it on your end.</param>
    '''' <param name="CallerName">Name of the calling function.  Do not provide this, it will be filled automatically if you pass Nothing.</param>
    '''' <remarks></remarks>
    'Public Shared Sub StartLoading(Message As String, Optional Progress As Single? = Nothing, <CallerMemberName> Optional CallerName As String = Nothing)
    '    If _loadingDefinitions.ContainsKey(CallerName) Then
    '        If Progress.HasValue Then
    '            _loadingDefinitions(CallerName) = New LoadingMessageChangedEventArgs(Message, Progress)
    '        Else
    '            _loadingDefinitions(CallerName) = New LoadingMessageChangedEventArgs(Message)
    '        End If
    '    Else
    '        If Progress.HasValue Then
    '            _loadingDefinitions.Add(CallerName, New LoadingMessageChangedEventArgs(Message, Progress))
    '        Else
    '            _loadingDefinitions.Add(CallerName, New LoadingMessageChangedEventArgs(Message))
    '        End If
    '    End If
    '    MakeLoadingVisibleorNot()
    'End Sub
    '''' <summary>
    '''' Closes the loading window shown from StartLoading
    '''' </summary>
    '''' <param name="CallerName">Name of the calling function.  Do not provide this, it will be filled automatically if you pass Nothing.</param>
    '''' <remarks></remarks>
    'Public Shared Sub StopLoading(<CallerMemberName> Optional CallerName As String = Nothing)
    '    If _loadingDefinitions.ContainsKey(CallerName) Then
    '        _loadingDefinitions.Remove(CallerName)
    '        MakeLoadingVisibleorNot()
    '    End If
    'End Sub
    Private Shared Sub MakeLoadingVisibleorNot()
        Dim shouldShow As Boolean = (_loadingDefinitions.Count > 0)
        Dim isShowing As Boolean = _loadingShown

        If shouldShow Then
            If _loadingDefinitions.Count > 1 Then
                RaiseEvent LoadingMessageChanged(Nothing, New LoadingMessageChangedEventArgs(PluginHelper.GetLanguageItem("Loading", "Loading...")))
            ElseIf _loadingDefinitions.Count = 1 Then
                RaiseEvent LoadingMessageChanged(Nothing, _loadingDefinitions.Values(0))
            End If
        Else
            RaiseEvent LoadingMessageChanged(Nothing, New LoadingMessageChangedEventArgs(PluginHelper.GetLanguageItem("Ready"), 1))
        End If
    End Sub
    Public Shared Event LoadingMessageChanged(sender As Object, e As LoadingMessageChangedEventArgs)
    Public Shared Sub SetLoadingStatus(Message As String, Progress As Single)
        RaiseEvent LoadingMessageChanged(Nothing, New LoadingMessageChangedEventArgs(Message, Progress))
    End Sub
    Public Shared Sub SetLoadingStatus(Message As String)
        RaiseEvent LoadingMessageChanged(Nothing, New LoadingMessageChangedEventArgs(Message))
    End Sub
    Public Shared Sub SetLoadingStatusFinished()
        RaiseEvent LoadingMessageChanged(Nothing, New LoadingMessageChangedEventArgs(PluginHelper.GetLanguageItem("Ready"), 1))
    End Sub
    Public Shared Sub SetLoadingStatusFailed()
        SetLoadingStatus(PluginHelper.GetLanguageItem("Failed"), 0)
    End Sub
#End Region

#Region "Console Writeline"
    Public Class ConsoleLineWrittenEventArgs
        Inherits EventArgs
        Public Property Line As String
        Public Property Type As LineType
        Public Property CallerName As String
    End Class
    Public Enum LineType
        Message = 1
        Warning = 2
        [Error] = 3
        ConsoleOutput = 4
    End Enum
    Public Shared Sub Writeline(Line As String, Optional type As LineType = LineType.Message, <CallerMemberName> Optional CallerName As String = Nothing)
        Console.WriteLine(Line) 'String.Format("{1}", CallerName, Line)) 'Old format: "{0}: {1}"

        Dim e As New ConsoleLineWrittenEventArgs
        e.Line = Line
        e.Type = type
        e.CallerName = CallerName
        RaiseEvent ConsoleLineWritten(Nothing, e)
    End Sub
    Public Shared Event ConsoleLineWritten(sender As Object, e As ConsoleLineWrittenEventArgs)
#End Region

#Region "Open/Close File"
    Public Shared Event FileOpenRequested(sender As Object, e As EventArguments.FileOpenedEventArguments)
    Public Shared Event FileClosed(sender As Object, e As EventArguments.FileClosedEventArgs)
    Public Shared Sub RaiseFileClosed(sender As Object, e As EventArguments.FileClosedEventArgs)
        RaiseEvent FileClosed(sender, e)
    End Sub
    Public Shared Sub RequestFileOpen(File As Object, DisposeOnClose As Boolean)
        If File IsNot Nothing Then
            RaiseEvent FileOpenRequested(Nothing, New EventArguments.FileOpenedEventArguments With {.File = File, .DisposeOnExit = DisposeOnClose})
        End If
    End Sub
    Public Shared Sub RequestFileOpen(File As Object, ParentProject As Project)
        If File IsNot Nothing Then
            RaiseEvent FileOpenRequested(Nothing, New EventArguments.FileOpenedEventArguments With {.File = File, .DisposeOnExit = False, .ParentProject = ParentProject})
        End If
    End Sub
#End Region

#Region "Error Reporting"
    Public Shared Event ExceptionThrown(sender As Object, e As EventArguments.ExceptionThrownEventArgs)
    Public Shared Sub ReportExceptionThrown(sender As Object, ex As Exception)
        RaiseEvent ExceptionThrown(sender, New EventArguments.ExceptionThrownEventArgs With {.Exception = ex})
    End Sub
#End Region

    Public Shared Async Function CopyDirectory(SourceDirectory As String, DestinationDirectory As String) As Task
        Dim f As New Utilities.AsyncFileCopier
        Await f.CopyDirectory(SourceDirectory, DestinationDirectory)
    End Function

    ''' <summary>
    ''' Shows the application's underlying console to the user.
    ''' </summary>
    Public Shared Sub ShowConsole()
        Internal.ConsoleManager.Show()
    End Sub

    ''' <summary>
    ''' Returns whether or not ObjectToCheck is of type T.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="ObjectToCheck"></param>
    ''' <returns></returns>
    Public Shared Function IsTypeOf(Of T)(ObjectToCheck As Object) As Boolean
        Return Utilities.ReflectionHelpers.IsOfType(ObjectToCheck, GetType(T))
    End Function

    ''' <summary>
    ''' Casts the given object to type T, or returns its contained item if it implements the interface iContainer(Of T).
    ''' </summary>
    ''' <typeparam name="T">Type to cast to.</typeparam>
    ''' <param name="ObjectToCast">Object to cast.</param>
    ''' <returns></returns>
    Public Shared Function Cast(Of T)(ObjectToCast As Object) As T
        If TypeOf ObjectToCast Is T Then
            Return DirectCast(ObjectToCast, T)
        ElseIf TypeOf ObjectToCast Is iContainer(Of T) Then
            Return DirectCast(ObjectToCast, iContainer(Of T)).Item
        Else
            'I should probably throw my own exception here, since I'm casting EditingObject to T even though I just found that EditingObject is NOT T, but there will be an exception anyway
            Return DirectCast(ObjectToCast, T)
        End If
    End Function

    ''' <summary>
    ''' If ObjectToCast implements iContainer(Of T), sets iContainer(Of T).Item to NewValue.
    ''' Otherwise, sets ObjectToCast to NewValue.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="ObjectToCast"></param>
    ''' <param name="NewValue"></param>
    Public Shared Sub CastUpdate(Of T)(ByRef ObjectToCast As Object, ByVal NewValue As T)
        If TypeOf ObjectToCast Is T Then
            ObjectToCast = NewValue
        ElseIf TypeOf ObjectToCast Is iContainer(Of T) Then
            DirectCast(ObjectToCast, iContainer(Of T)).Item = NewValue
        Else
            ObjectToCast = NewValue
        End If
    End Sub
End Class