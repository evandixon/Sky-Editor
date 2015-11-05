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
        Dim baseDir = IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins/", Assembly.GetCallingAssembly.GetName.Name.Replace("_plg", ""))
        If Not IO.Directory.Exists(baseDir) Then
            IO.Directory.CreateDirectory(baseDir)
        End If
        Return IO.Path.Combine(baseDir, Path)
    End Function

    Public Shared Function GetResourceName(Path As String, Plugin As String) As String
        Dim baseDir = IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins/", Plugin.Replace("_plg", ""))
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
                If t IsNot Nothing AndAlso Not String.IsNullOrEmpty(t) Then DirectCast(item, Label).Content = GetLanguageItem(t.Trim("$"), CallingAssembly:=Assembly.GetCallingAssembly.GetName.Name)

            ElseIf TypeOf item Is Button Then
                Dim t As String = DirectCast(item, Button).Content
                If t IsNot Nothing AndAlso Not String.IsNullOrEmpty(t) Then DirectCast(item, Button).Content = GetLanguageItem(t.Trim("$"), CallingAssembly:=Assembly.GetCallingAssembly.GetName.Name)

            ElseIf TypeOf item Is CheckBox Then
                Dim t As String = DirectCast(item, CheckBox).Content
                If t IsNot Nothing AndAlso Not String.IsNullOrEmpty(t) Then DirectCast(item, CheckBox).Content = GetLanguageItem(t.Trim("$"), CallingAssembly:=Assembly.GetCallingAssembly.GetName.Name)

            ElseIf TypeOf item Is MenuItem Then
                Dim t As String = DirectCast(item, MenuItem).Header
                If t IsNot Nothing AndAlso Not String.IsNullOrEmpty(t) Then DirectCast(item, MenuItem).Header = GetLanguageItem(t.Trim("$"), CallingAssembly:=Assembly.GetCallingAssembly.GetName.Name)

            ElseIf TypeOf item Is TabItem Then
                Dim t As String = DirectCast(item, TabItem).Header
                If t IsNot Nothing AndAlso Not String.IsNullOrEmpty(t) Then DirectCast(item, TabItem).Header = GetLanguageItem(t.Trim("$"), CallingAssembly:=Assembly.GetCallingAssembly.GetName.Name)
            End If
        Next
    End Sub
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
            StartLoading(String.Format(PluginHelper.GetLanguageItem("WaitingOnTask", "Waiting on {0}..."), IO.Path.GetFileName(Filename)))

            p.WaitForExit()

            StopLoading()
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
            StartLoading(String.Format(PluginHelper.GetLanguageItem("WaitingOnTask", "Waiting on {0}..."), IO.Path.GetFileName(Filename)))

            Await WaitForProcess(p)

            StopLoading()
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
    Private Shared _loadingWindow As BackgroundTaskWait
    Private Shared _loadingDefinitions As New Dictionary(Of String, LoadingMessageChangedEventArgs)

    ''' <summary>
    ''' Shows a loading window until the same function calls StopLoading.
    ''' </summary>
    ''' <param name="Message">The message to be displayed while loading.  This message will not be translated so you should translate it on your end.</param>
    ''' <param name="CallerName">Name of the calling function.  Do not provide this, it will be filled automatically if you pass Nothing.</param>
    ''' <remarks></remarks>
    Public Shared Sub StartLoading(Message As String, Optional Progress As Single? = Nothing, <CallerMemberName> Optional CallerName As String = Nothing)
        If _loadingDefinitions.ContainsKey(CallerName) Then
            If Progress.HasValue Then
                _loadingDefinitions(CallerName) = New LoadingMessageChangedEventArgs(Message, Progress)
            Else
                _loadingDefinitions(CallerName) = New LoadingMessageChangedEventArgs(Message)
            End If
        Else
            If Progress.HasValue Then
                    _loadingDefinitions.Add(CallerName, New LoadingMessageChangedEventArgs(Message, Progress))
                Else
                    _loadingDefinitions.Add(CallerName, New LoadingMessageChangedEventArgs(Message))
                End If
            End If
        MakeLoadingVisibleorNot()
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
            'Try
            '    ' _loadingWindow = New BackgroundTaskWait()
            'Catch ex As Exception
            '    PluginHelper.Writeline("Unable to show loading window.  Exception details: " & ex.ToString, LineType.Error)
            '    Exit Sub
            'End Try
            'End Sub), CancellationToken.None, TaskCreationOptions.None, s)
        End If
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

        'If shouldShow AndAlso Not isShowing Then
        '    If _loadingDefinitions.Count > 1 Then
        '        ' _loadingWindow.Show(PluginHelper.GetLanguageItem("Loading", "Loading..."))
        '        RaiseEvent LoadingMessageChanged(Nothing, New LoadingMessageChangedEventArgs(PluginHelper.GetLanguageItem("Loading", "Loading...")))
        '    ElseIf _loadingDefinitions.Count = 1 Then
        '        ' _loadingWindow.Show(_loadingDefinitions.Values(0))
        '        RaiseEvent LoadingMessageChanged(Nothing, _loadingDefinitions.Values(0))
        '    End If
        '    _loadingShown = True
        'ElseIf shouldShow AndAlso isShowing Then
        '    If _loadingDefinitions.Count > 1 Then
        '        ' _loadingWindow.ChangeMessage(PluginHelper.GetLanguageItem("Loading", "Loading..."))
        '        RaiseEvent LoadingMessageChanged(Nothing, New LoadingMessageChangedEventArgs(PluginHelper.GetLanguageItem("Loading", "Loading...")))
        '    ElseIf _loadingDefinitions.Count = 1 Then
        '        '  _loadingWindow.ChangeMessage(_loadingDefinitions.Values(0))
        '        RaiseEvent LoadingMessageChanged(Nothing, _loadingDefinitions.Values(0))
        '    End If
        'ElseIf isShowing AndAlso Not shouldShow Then
        '    ' _loadingWindow.DoClose()
        '    _loadingWindow = Nothing
        '    _loadingShown = False
        '    RaiseEvent LoadingMessageChanged(Nothing, New LoadingMessageChangedEventArgs(PluginHelper.GetLanguageItem("Ready"), 1))
        'End If
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

    Public Shared Async Function CopyDirectory(SourceDirectory As String, DestinationDirectory As String) As Task
        Dim f As New Utilities.AsyncFileCopier
        Await f.CopyDirectory(SourceDirectory, DestinationDirectory)
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

#Region "Reflection Helpers"
    Public Shared Function IsMethodOverridden(Method As MethodInfo) As Boolean
        Return Not (Method.GetBaseDefinition = Method)
    End Function
#End Region

End Class