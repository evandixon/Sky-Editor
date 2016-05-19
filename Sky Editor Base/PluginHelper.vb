Imports System.Reflection
Imports System.Threading.Tasks
Imports System.Runtime.CompilerServices
Imports System.Deployment.Application
Imports System.Resources
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO

''' <summary>
''' A collection of methods that are useful to Sky Editor plugins.
''' </summary>
''' <remarks></remarks>
Public Class PluginHelper

#Region "Resources"

    Public Shared Function GetExtensionDirectory() As String
        Return IO.Path.Combine(RootResourceDirectory, "Extensions")
    End Function
    ''' <summary>
    ''' Combines the given path with your plugin's resource directory.
    ''' </summary>
    ''' <param name="Path"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetResourceName(Path As String) As String
        Return GetResourceName(Path, Assembly.GetCallingAssembly.GetName.Name)
    End Function

    Public Shared Function GetResourceName(Path As String, Plugin As String, Optional ThrowIfCantCreateDirectory As Boolean = False) As String
        Dim fullPath = IO.Path.Combine(GetResourceDirectory(Plugin), Path)
        Dim baseDir = IO.Path.GetDirectoryName(fullPath)
        Try
            If Not IO.Directory.Exists(baseDir) Then
                IO.Directory.CreateDirectory(baseDir)
            End If
        Catch ex As UnauthorizedAccessException
            If ThrowIfCantCreateDirectory Then
                Throw ex
            End If
        End Try
        Return fullPath
    End Function
    ''' <summary>
    ''' Returns your plugin's resource directory as managed by Sky Editor.
    ''' It will be created if it does not exist.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetResourceDirectory() As String
        Return GetResourceDirectory(Assembly.GetCallingAssembly.GetName.Name)
    End Function
    Public Shared Function GetResourceDirectory(AssemblyName As String, Optional ThrowIfCantCreateDirectory As Boolean = False) As String
        Dim baseDir = IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins", AssemblyName)
        If IO.Directory.Exists(baseDir) Then
            Return baseDir
        ElseIf IO.Directory.Exists(IO.Path.Combine(Environment.CurrentDirectory, AssemblyName)) Then
            Return IO.Path.Combine(Environment.CurrentDirectory, AssemblyName)
        Else
            Try
                IO.Directory.CreateDirectory(baseDir)
            Catch ex As UnauthorizedAccessException
                If ThrowIfCantCreateDirectory Then
                    Throw ex
                End If
            End Try
            Return baseDir
        End If
    End Function

    ''' <summary>
    ''' Returns a the path of the root resource directory and creates it if it doesn't exist.
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function RootResourceDirectory(Optional ThrowIfCantCreateDirectory As Boolean = False) As String
        Dim d As String
        If ApplicationDeployment.IsNetworkDeployed Then
            'I'm choosing not to verify if the folder exists because I'm already going to check below.
            d = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create)
        Else
            d = IO.Path.Combine(Environment.CurrentDirectory & "\Resources")
            If Not IO.Directory.Exists(d) Then
                Try
                    IO.Directory.CreateDirectory(d)
                Catch ex As UnauthorizedAccessException
                    If ThrowIfCantCreateDirectory Then
                        Throw ex
                    End If
                End Try
            End If
        End If
        Return d
    End Function

    ''' <summary>
    ''' Gets the directory plugins are stored when in dev mode.
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function DevPluginDirectory() As String
        Return IO.Path.Combine(RootResourceDirectory, "Plugins")
    End Function


    ''' <summary>
    ''' Gets absolute paths of all the assemblies in the plugin directory.
    ''' Not all of these are guarenteed to be supported plugins.
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetPluginAssemblies() As List(Of String)
        Dim FromFolder = DevPluginDirectory()
        Dim assemblyPaths As New List(Of String)
        If IO.Directory.Exists(FromFolder) Then
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
        Else
            'If the "proper" plugin directory doesn't exist, we'll also load from the working directory (because plugins often reference SkyEditor.exe, which will be copied to the same directory).
            For Each item In IO.Directory.GetFiles(Environment.CurrentDirectory, "*.dll")
                If Not assemblyPaths.Contains(item) Then
                    assemblyPaths.Add(item)
                End If
            Next
            For Each item In IO.Directory.GetFiles(Environment.CurrentDirectory, "*.exe")
                If Not assemblyPaths.Contains(item) Then
                    assemblyPaths.Add(item)
                End If
            Next
        End If
        Return assemblyPaths
    End Function
#End Region

#Region "Translation"
    ''' <summary>
    ''' Gets the name of the given type if its contained assembly has its name in its localized resource file, or the full name of the type if it does not.
    ''' </summary>
    ''' <param name="type">Type of which to get the name.</param>
    ''' <returns></returns>
    Public Shared Function GetTypeName(type As Type) As String
        Dim output As String = Nothing
        Dim parent = type.Assembly
        Dim manager As ResourceManager = Nothing
        Dim resxNames As New List(Of String)(parent.GetManifestResourceNames)
        'Dim q = From r In resxNames Where String.Compare(r, "language", True, Globalization.CultureInfo.InvariantCulture) = 0
        'If q.Any Then
        '    'Then look in this one first.
        '    manager = New ResourceManager(q.First, parent)
        'End If

        'If manager IsNot Nothing Then
        '    output = manager.GetString(type.FullName.Replace(".", "_"))
        'End If

        If output Is Nothing Then
            'Then either the language resources doesn't exist, or does not contain what we're looking for.
            'In either case, we'll look at the other resource files.
            For Each item In resxNames
                manager = New ResourceManager(item.Replace(".resources", ""), parent)
                output = manager.GetString(type.FullName.Replace(".", "_"))
                If output IsNot Nothing Then
                    Exit For 'We found something.  Time to return it.
                End If
            Next
        End If

        If output IsNot Nothing Then
            Return output
        Else
            Return type.FullName
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
    ''' 
    ''' </summary>
    ''' <param name="sendingProcess"></param>
    ''' <param name="outLine"></param>
    ''' <remarks>Posted by brendan at http://stackoverflow.com/questions/9996709/read-console-process-output</remarks>
    Private Shared Sub OutputHandler(sendingProcess As Object, outLine As DataReceivedEventArgs)
        ' Collect the sort command output.
        If Not String.IsNullOrEmpty(outLine.Data) Then
            PluginHelper.Writeline(outLine.Data, LineType.ConsoleOutput)
        End If
    End Sub

    ''' <summary>
    ''' Runs the specified program, capturing console output, and waits for it to complete.
    ''' </summary>
    ''' <param name="Filename">Filename of the executable to run.</param>
    ''' <param name="Arguments">Arguments to pass to the process.</param>
    ''' <remarks></remarks>
    Public Shared Async Function RunProgram(Filename As String, Arguments As String, Optional ShowLoadingWindow As Boolean = True) As Task
        Writeline(String.Format(My.Resources.Language.ExecutingProgram, Filename, Arguments))
        'Set up the process
        Dim p As New Process()
        p.StartInfo.FileName = Filename
        p.StartInfo.Arguments = Arguments
        p.StartInfo.RedirectStandardOutput = True
        p.StartInfo.UseShellExecute = False
        p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
        p.StartInfo.CreateNoWindow = True
        p.StartInfo.WorkingDirectory = IO.Path.GetDirectoryName(Filename)
        AddHandler p.OutputDataReceived, AddressOf OutputHandler

        'Start the process
        p.Start()
        p.BeginOutputReadLine()

        'Wait for the process to close
        If ShowLoadingWindow Then
            SetLoadingStatus(String.Format(My.Resources.Language.WaitingOnProgram, IO.Path.GetFileName(Filename)))
            Await WaitForProcess(p)
            SetLoadingStatusFinished()
        Else
            Await WaitForProcess(p)
        End If

        'Clean up
        RemoveHandler p.OutputDataReceived, AddressOf OutputHandler
        p.Dispose()
        Writeline(String.Format(My.Resources.Language.ProgramFinished, p.StartInfo.FileName))
    End Function

    ''' <summary>
    ''' Waits for the given process to exit.
    ''' </summary>
    ''' <param name="p">The process for which to wait.</param>
    ''' <returns></returns>
    Private Shared Async Function WaitForProcess(p As Process) As Task
        Await Task.Run(Sub()
                           p.WaitForExit()
                       End Sub)
    End Function

    ''' <summary>
    ''' Runs the specified program without waiting for it to complete.
    ''' </summary>
    ''' <param name="Filename">Filename of the executable to run.</param>
    ''' <param name="Arguments">Arguments to pass to the process.</param>
    ''' <remarks></remarks>
    Public Shared Sub RunProgramInBackground(Filename As String, Arguments As String)
        Writeline(String.Format(My.Resources.Language.ExecutingProgramAsync, Filename, Arguments))
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

    Public Shared Event LoadingMessageChanged(sender As Object, e As LoadingMessageChangedEventArgs)
    Public Shared Sub SetLoadingStatus(Message As String, Progress As Single)
        RaiseEvent LoadingMessageChanged(Nothing, New LoadingMessageChangedEventArgs(Message, Progress))
    End Sub
    Public Shared Sub SetLoadingStatus(Message As String)
        RaiseEvent LoadingMessageChanged(Nothing, New LoadingMessageChangedEventArgs(Message))
    End Sub
    Public Shared Sub SetLoadingStatusFinished()
        RaiseEvent LoadingMessageChanged(Nothing, New LoadingMessageChangedEventArgs(My.Resources.Language.Ready, 1))
    End Sub
    Public Shared Sub SetLoadingStatusFailed()
        SetLoadingStatus(My.Resources.Language.Failed, 0)
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
    Public Shared Event FileOpenRequested(sender As Object, e As FileOpenedEventArguments)
    Public Shared Event FileClosed(sender As Object, e As FileClosedEventArgs)
    Public Shared Sub RaiseFileClosed(sender As Object, e As FileClosedEventArgs)
        RaiseEvent FileClosed(sender, e)
    End Sub
    Public Shared Sub RequestFileOpen(File As Object, DisposeOnClose As Boolean)
        If File IsNot Nothing Then
            RaiseEvent FileOpenRequested(Nothing, New FileOpenedEventArguments With {.File = File, .DisposeOnExit = DisposeOnClose})
        End If
    End Sub
    Public Shared Sub RequestFileOpen(File As Object, ParentProject As Project)
        If File IsNot Nothing Then
            RaiseEvent FileOpenRequested(Nothing, New FileOpenedEventArguments With {.File = File, .DisposeOnExit = False, .ParentProject = ParentProject})
        End If
    End Sub
#End Region

#Region "Error Reporting"
    Public Shared Event ExceptionThrown(sender As Object, e As EventArguments.ExceptionThrownEventArgs)
    Public Shared Sub ReportExceptionThrown(sender As Object, ex As Exception)
        RaiseEvent ExceptionThrown(sender, New EventArguments.ExceptionThrownEventArgs With {.Exception = ex})
    End Sub
#End Region

    ''' <summary>
    ''' Shows the application's underlying console to the user.
    ''' </summary>
    Public Shared Sub ShowConsole()
        Internal.ConsoleManager.Show()
    End Sub

    ''' <summary>
    ''' Casts the given object to type T, or returns its contained item if it implements the interface iContainer(Of T).
    ''' </summary>
    ''' <typeparam name="T">Type to cast to.</typeparam>
    ''' <param name="ObjectToCast">Object to cast.</param>
    ''' <returns></returns>
    Public Shared Function Cast(Of T)(ObjectToCast As Object) As T
        If TypeOf ObjectToCast Is T Then
            Return DirectCast(ObjectToCast, T)
        ElseIf TypeOf ObjectToCast Is IContainer(Of T) Then
            Return DirectCast(ObjectToCast, IContainer(Of T)).Item
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
        ElseIf TypeOf ObjectToCast Is IContainer(Of T) Then
            DirectCast(ObjectToCast, IContainer(Of T)).Item = NewValue
        Else
            ObjectToCast = NewValue
        End If
    End Sub
End Class