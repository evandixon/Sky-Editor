Imports System.Collections.Concurrent
Imports System.Reflection
Imports SkyEditorBase.SkyEditorWindows
Imports System.Threading.Tasks
Imports System.Runtime.CompilerServices
Imports System.Deployment.Application
Imports System.Threading

''' <summary>
''' A collection of methods that are useful to Sky Editor plugins.
''' </summary>
''' <remarks></remarks>
Public Class PluginHelper
    'From http://blogs.msdn.com/b/pfxteam/archive/2010/04/07/9990421.aspx
    Public NotInheritable Class StaTaskScheduler
        Inherits TaskScheduler
        Implements IDisposable

        Private ReadOnly _threads As List(Of Thread)

        Private _tasks As BlockingCollection(Of Task)

        Public Sub New(numberOfThreads As Integer)

            If numberOfThreads < 1 Then

                Throw New ArgumentOutOfRangeException("concurrencyLevel")
            End If

            _tasks = New BlockingCollection(Of Task)()

            _threads = Enumerable.Range(0, numberOfThreads).[Select](Function(i)
                                                                         Dim thread = New Thread(New ThreadStart(Sub()
                                                                                                                     For Each t In _tasks.GetConsumingEnumerable()
                                                                                                                         TryExecuteTask(t)
                                                                                                                     Next
                                                                                                                 End Sub))
                                                                         thread.IsBackground = True
                                                                         thread.SetApartmentState(ApartmentState.STA)
                                                                         Return thread
                                                                     End Function).ToList()

            _threads.ForEach(Sub(t) t.Start())
        End Sub

        Protected Overrides Sub QueueTask(task As Task)
            _tasks.Add(task)
        End Sub

        Protected Overrides Function GetScheduledTasks() As IEnumerable(Of Task)
            Return _tasks.ToArray()
        End Function

        Protected Overrides Function TryExecuteTaskInline(task As Task, taskWasPreviouslyQueued As Boolean) As Boolean
            Return Thread.CurrentThread.GetApartmentState() = ApartmentState.STA AndAlso TryExecuteTask(task)
        End Function

        Public Overrides ReadOnly Property MaximumConcurrencyLevel() As Integer
            Get
                Return _threads.Count
            End Get
        End Property

        Public Sub Dispose() Implements IDisposable.Dispose
            If _tasks IsNot Nothing Then
                _tasks.CompleteAdding()

                For Each thread In _threads
                    thread.Join()
                Next

                _tasks.Dispose()

                _tasks = Nothing
            End If
        End Sub
    End Class

    ''' <summary>
    ''' Contains a reference to the last PluginManager created.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property PluginManagerInstance As PluginManager
    ''' <summary>
    ''' Gets the name of the assembly of whatever assembly calls this method.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetAssemblyName() As String
        Return Assembly.GetCallingAssembly.GetName.Name
    End Function
    ''' <summary>
    ''' Combines the given path with your plugin's resource directory.
    ''' </summary>
    ''' <param name="Path"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetResourceName(Path As String) As String
        Dim baseDir = IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins/", Assembly.GetCallingAssembly.GetName.Name.Replace("_plg", ""))
        If Not IO.Directory.Exists(baseDir) Then
            IO.Directory.CreateDirectory(baseDir)
        End If
        Return IO.Path.Combine(baseDir, Path)
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
        If SkyEditorBase.Settings.GetSettings.Setting("DebugLangaugePlaceholders") = "True" Then
            Return String.Format("[{0}]", Key)
        Else
            Return Internal.LanguageManager.GetLanguageItem(Key, CallingAssembly, DefaultValue)
        End If
    End Function
    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="v"></param>
    ''' <param name="SearchLevel">The depth to search for controls.</param>
    ''' <remarks></remarks>
    Public Shared Sub TranslateForm(ByRef v As Visual, Optional SearchLevel As Integer = 5)
        Dim controls = (New ChildControls).GetChildren(v, 10)
        If Not controls.Contains(v) Then controls.Add(v)
        For Each item In controls
            If TypeOf item Is Label Then
                Dim t As String = DirectCast(item, Label).Content
                If t IsNot Nothing Then DirectCast(item, Label).Content = GetLanguageItem(t.Trim("$"), CallingAssembly:=Assembly.GetCallingAssembly.GetName.Name)
            ElseIf TypeOf item Is Controls.Button Then
                Dim t As String = DirectCast(item, Button).Content
                If t IsNot Nothing Then DirectCast(item, Button).Content = GetLanguageItem(t.Trim("$"), CallingAssembly:=Assembly.GetCallingAssembly.GetName.Name)
            ElseIf TypeOf item Is Controls.MenuItem Then
                Dim t As String = DirectCast(item, MenuItem).Header
                If t IsNot Nothing Then DirectCast(item, MenuItem).Header = GetLanguageItem(t.Trim("$"), CallingAssembly:=Assembly.GetCallingAssembly.GetName.Name)
            ElseIf TypeOf item Is Controls.TabItem Then
                Dim t As String = DirectCast(item, TabItem).Header
                If t IsNot Nothing Then DirectCast(item, TabItem).Header = GetLanguageItem(t.Trim("$"), CallingAssembly:=Assembly.GetCallingAssembly.GetName.Name)
            End If
        Next
    End Sub
    ''' <summary>
    ''' Posted by brendan at http://stackoverflow.com/questions/9996709/read-console-process-output
    ''' </summary>
    ''' <param name="sendingProcess"></param>
    ''' <param name="outLine"></param>
    ''' <remarks></remarks>
    Private Shared Sub OutputHandler(sendingProcess As Object, outLine As DataReceivedEventArgs)
        ' Collect the sort command output.
        If Not String.IsNullOrEmpty(outLine.Data) Then
            'Add the text to the collected output.
            'Don't write to dev console, this is a different thread.
            'The only difference is that this output will be shown in the Real console, while DebugConsole.Writeline may be shown in the window
            Console.WriteLine(outLine.Data)
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
        Writeline(String.Format("Executing {0} {1}", Filename, Arguments))
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
            StartLoading(String.Format(PluginHelper.GetLanguageItem("WaitingOnTask", "Waiting on {0}..."), IO.Path.GetFileName(Filename)))

            p.WaitForExit()

            StopLoading()
            RemoveHandler p.OutputDataReceived, AddressOf OutputHandler
            p.Dispose()
            Writeline(String.Format("""{0}"" finished running.", p.StartInfo.FileName))
        Else
            p.WaitForExit()
            p.Dispose()
            Writeline(String.Format("""{0}"" finished running.", p.StartInfo.FileName))
        End If
    End Sub
    ''' <summary>
    ''' Runs the specified program, capturing console output.
    ''' Returns true when the program exits.
    ''' </summary>
    ''' <param name="Filename"></param>
    ''' <param name="Arguments"></param>
    ''' <remarks></remarks>
    Public Shared Async Function RunProgram(Filename As String, Arguments As String, Optional ShowLoadingWindow As Boolean = True) As Task(Of Boolean)
        Writeline(String.Format("Executing {0} {1}", Filename, Arguments))
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
            StartLoading(String.Format(PluginHelper.GetLanguageItem("WaitingOnTask", "Waiting on {0}..."), IO.Path.GetFileName(Filename)))

            Await WaitForProcess(p)

            StopLoading()
            RemoveHandler p.OutputDataReceived, AddressOf OutputHandler
            p.Dispose()
            Writeline(String.Format("""{0}"" finished running.", p.StartInfo.FileName))
        Else
            If Await WaitForProcess(p) Then
                RemoveHandler p.OutputDataReceived, AddressOf OutputHandler
                p.Dispose()
                Writeline(String.Format("""{0}"" finished running.", p.StartInfo.FileName))
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
        Writeline(String.Format("(Async) Executing ""{0}"" ""{1}""", Filename, Arguments))
        Dim p As New Process()
        p.StartInfo.FileName = Filename
        p.StartInfo.Arguments = Arguments
        p.Start()
    End Sub

    Private Shared _loadingShown As Boolean = False
    Private Shared _loadingWindow As BackgroundTaskWait
    Private Shared _loadingDefinitions As New Dictionary(Of String, String)

    ''' <summary>
    ''' Shows a loading window until the same function calls StopLoading.
    ''' </summary>
    ''' <param name="Message">The message to be displayed while loading.  This message will not be translated so you should translate it on your end.</param>
    ''' <param name="CallerName">Name of the calling function.  Do not provide this, it will be filled automatically if you pass Nothing.</param>
    ''' <remarks></remarks>
    Public Shared Sub StartLoading(Message As String, <CallerMemberName> Optional CallerName As String = Nothing)
        If Not _loadingDefinitions.ContainsKey(CallerName) Then
            _loadingDefinitions.Add(CallerName, Message)
            MakeLoadingVisibleorNot()
        End If
    End Sub
    ''' <summary>
    ''' Closes the loading window shown from StartLoading
    ''' </summary>
    ''' <param name="CallerName">Name of the calling function.  Do not provide this, it will be filled automatically if you pass Nothing.</param>
    ''' <remarks></remarks>
    Public Shared Sub StopLoading(<CallerMemberName> Optional CallerName As String = Nothing)
        If _loadingDefinitions.ContainsKey(CallerName) Then
            _loadingDefinitions.Remove(CallerName)
            MakeLoadingVisibleorNot()
        End If
    End Sub
    Private Shared Sub MakeLoadingVisibleorNot()
        If _loadingWindow Is Nothing Then
            'Dim s As New StaTaskScheduler(1)
            'Task.Factory.StartNew(New Action(Sub()
            Try
                _loadingWindow = New BackgroundTaskWait()
            Catch ex As Exception
                PluginHelper.Writeline("Unable to show loading window.  Exception details: " & ex.ToString, LineType.Error)
                Exit Sub
            End Try
            'End Sub), CancellationToken.None, TaskCreationOptions.None, s)
        End If
        Dim shouldShow As Boolean = (_loadingDefinitions.Count > 0)
        Dim isShowing As Boolean = _loadingShown

        If shouldShow AndAlso Not isShowing Then
            If _loadingDefinitions.Count > 1 Then
                _loadingWindow.Show(PluginHelper.GetLanguageItem("Loading", "Loading..."))
            ElseIf _loadingDefinitions.Count = 1 Then
                _loadingWindow.Show(_loadingDefinitions.Values(0))
            End If
            _loadingShown = True
        ElseIf shouldShow AndAlso isShowing Then
            If _loadingDefinitions.Count > 1 Then
                _loadingWindow.ChangeMessage(PluginHelper.GetLanguageItem("Loading", "Loading..."))
            ElseIf _loadingDefinitions.Count = 1 Then
                _loadingWindow.ChangeMessage(_loadingDefinitions.Values(0))
            End If
        ElseIf isShowing AndAlso Not shouldShow Then
            _loadingWindow.DoClose()
            _loadingWindow = Nothing
            _loadingShown = False
        End If
    End Sub
    Public Enum LineType
        Message = 1
        Warning = 2
        [Error] = 3
    End Enum
    Public Shared Sub Writeline(Line As String, Optional type As LineType = LineType.Message, <CallerMemberName> Optional CallerName As String = Nothing)
        Console.WriteLine(String.Format("{0}: {1}", CallerName, Line))
    End Sub
    ''' <summary>
    ''' Starts accepting commands from the console.
    ''' </summary>
    ''' <param name="Manager"></param>
    ''' <param name="ConsoleCommands"></param>
    ''' <remarks></remarks>
    Friend Shared Sub DoCommands(Manager As PluginManager, ConsoleCommands As Dictionary(Of String, PluginManager.ConsoleCommand))
        Writeline("Type ""exit"" to return to Sky Editor.")
        While True
            Console.Write("> ")
            Dim line = Console.ReadLine()
            Dim cmdParts = line.Split(" ".ToCharArray, 2)
            Dim cmd = cmdParts(0).ToLower
            Dim arg = ""
            If cmdParts.Length > 1 Then
                arg = cmdParts(1)
            End If
            If cmd = "exit" Then
                Writeline("You may now use Sky Editor again.")
                Exit While
            ElseIf ConsoleCommands.Keys.Contains(cmd) Then
                ConsoleCommands(cmd).Invoke(Manager, arg)
            Else
                Writeline(String.Format("""{0}"" is not a recognizable command.", cmd))
            End If
        End While
    End Sub

    Public Shared Function RootResourceDirectory() As String
        Dim d As String
        If ApplicationDeployment.IsNetworkDeployed Then
            d = My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData
        Else
            d = IO.Path.Combine(Environment.CurrentDirectory & "\Resources")
        End If
        If Not IO.Directory.Exists(d) Then
            IO.Directory.CreateDirectory(d)
        End If
        Return d
    End Function
    Public Shared Function PluginsToInstallDirectory() As String
        Dim d = IO.Path.Combine(RootResourceDirectory, "ToInstall")
        If Not IO.Directory.Exists(d) Then
            IO.Directory.CreateDirectory(d)
        End If
        Return d
    End Function
    ''' <summary>
    ''' Deletes the given directory and everything inside it.
    ''' </summary>
    ''' <param name="DirectoryPath"></param>
    Public Shared Sub DeleteDirectory(DirectoryPath As String)
        'Delete the files
        For Each item In IO.Directory.GetFiles(DirectoryPath, "*", IO.SearchOption.AllDirectories)
            If IO.File.Exists(item) Then IO.File.Delete(item)
        Next
        'Delete the directories
        For Each item In IO.Directory.GetDirectories(DirectoryPath, "*", IO.SearchOption.AllDirectories)
            If IO.Directory.Exists(item) Then DeleteDirectory(item)
        Next
        'Delete the target directory
        IO.Directory.Delete(DirectoryPath, True)
    End Sub
End Class