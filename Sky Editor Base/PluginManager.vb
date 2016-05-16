Imports System.Reflection
Imports System.Text
Imports System.Threading.Tasks
Imports SkyEditor.Core.Interfaces
Imports SkyEditorBase.EventArguments
Imports SkyEditor.Core.Utilities
Imports SkyEditor.Core
Imports System.IO
Imports SkyEditor.Core.Extensions

Public Class PluginManager
    Inherits SkyEditor.Core.PluginManager
    Implements IDisposable

#Region "Constructors"
    ''' <summary>
    ''' Returns an instance of PluginManager, or returns nothing if an instance has not been created.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetInstance() As PluginManager
        If _instance Is Nothing Then
            _instance = New PluginManager
        End If
        Return _instance
    End Function
    Private Shared _instance As PluginManager

    ''' <summary>
    ''' Creates a new PluginManager, using the default storage location for plugins, which is Resources/Plugins, stored in the current working directory.
    ''' Plugins should end in _plg.dll or _plg.exe,  Ex. MyPlugin_plg.dll
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub New()
        MyBase.New

        Me.CurrentSolution = Nothing
        Me.OpenedFiles = New Dictionary(Of Object, ProjectOld)

        AddHandler PluginHelper.FileOpenRequested, AddressOf _pluginHelper_FileOpened
        AddHandler PluginHelper.FileClosed, AddressOf _pluginHelper_FileClosed
    End Sub
#End Region

#Region "Properties"



    ''' <summary>
    ''' Matches opened files to their parent projects
    ''' </summary>
    ''' <returns></returns>
    Private Property OpenedFiles As Dictionary(Of Object, ProjectOld)



    Public Property CurrentSolution As SolutionOld
        Get
            Return _currentSolutoin
        End Get
        Set(value As SolutionOld)
            If _currentSolutoin IsNot Nothing Then _currentSolutoin.Dispose()
            _currentSolutoin = value
            RaiseEvent SolutionChanged(Me, New EventArgs)
        End Set
    End Property
    Private WithEvents _currentSolutoin As SolutionOld

    Public Property CurrentProject As ProjectOld
        Get
            Return _currentProject
        End Get
        Set(value As ProjectOld)
            _currentProject = value
            RaiseEvent CurrentProjectChanged(Me, New EventArgs)
        End Set
    End Property
    Private WithEvents _currentProject As ProjectOld

#End Region

#Region "Functions"

#Region "IO Filters"
    ''' <summary>
    ''' Gets the IO filters in a form Open and Save file dialogs can use.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IOFiltersString(Optional Filters As Dictionary(Of String, String) = Nothing, Optional IsSaveAs As Boolean = False) As String
        If Filters Is Nothing Then
            Filters = IOFilters
        End If
        Dim listFilter As New StringBuilder
        Dim supportedFilterName As String = ""
        Dim supportedFilterExt As String = ""
        If Filters IsNot Nothing Then
            For Each item In Filters
                listFilter.Append(String.Format("{0} ({1})|{1}|", item.Value, item.Key))
                supportedFilterName &= item.Value & ", "
                supportedFilterExt &= "" & item.Key & ";"
            Next
            Dim out = ""
            If Not IsSaveAs Then
                out &= String.Format("{0} ({1})|{1}", supportedFilterName.Trim(";"), supportedFilterExt.Trim(";"))
            End If
            out &= "|" & listFilter.ToString & "All Files (*.*)|*.*"
            Return out.Trim("|")
        Else
            Return "All Files (*.*)|*.*"
        End If
    End Function

    ''' <summary>
    ''' Gets the IO filters in a form the Open and Save file dialogs can use, optimized for the Save file dialog.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IOFiltersStringSaveAs(DefaultExtension As String) As String
        Dim ext As String = DefaultExtension
        If IOFilters.ContainsKey(ext) Then
            Dim filters As New Dictionary(Of String, String)
            filters.Add(ext, IOFilters(ext))
            For Each item In IOFilters
                If Not filters.ContainsKey(item.Key) Then
                    filters.Add(item.Key, item.Value)
                End If
            Next
            Return IOFiltersString(filters, True)
        Else
            Return IOFiltersString()
        End If
    End Function
#End Region

    ''' <summary>
    ''' Returns the file's parent project, if it exists.
    ''' </summary>
    ''' <param name="File">File of which to get the parent project.  Must be an open file, otherwise the function will return Nothing.</param>
    ''' <returns></returns>
    Public Function GetOpenedFileProject(File As Object) As ProjectOld
        If Me.OpenedFiles.ContainsKey(File) Then
            Return Me.OpenedFiles(File)
        Else
            Return Nothing
        End If
    End Function

#End Region

#Region "Events"

    Public Event ProjectFileAdded(sender As Object, e As FileAddedEventArguments)
    Public Event ProjectFileRemoved(sender As Object, e As FileRemovedEventArgs)
    Public Event ProjectDirectoryCreated(sender As Object, e As ProjectDirectoryCreatedEventArgs)
    Public Event ProjectModified(sender As Object, e As EventArgs)
    Public Event MenuActionAdded(sender As Object, e As MenuActionAddedEventArgs)
    Public Event SolutionChanged(sender As Object, e As EventArgs)
    Public Event CurrentProjectChanged(sender As Object, e As EventArgs)

#End Region

#Region "Event Handlers"
    Private Sub _currentProject_FileAdded(sender As Object, e As EventArguments.FileAddedEventArguments) ' Handles _currentProject.FileAdded
        RaiseEvent ProjectFileAdded(sender, e)
    End Sub

    Private Sub _currentProject_FileRemoved(sender As Object, File As String) ' Handles _currentProject.FileRemoved
        RaiseEvent ProjectFileRemoved(sender, New FileRemovedEventArgs With {.File = File})
    End Sub

    Private Sub _currentProject_DirectoryCreated(sender As Object, Directory As String) ' Handles _currentProject.DirectoryCreated
        RaiseEvent ProjectDirectoryCreated(sender, New EventArguments.ProjectDirectoryCreatedEventArgs With {.Directory = Directory})
    End Sub

    Private Sub _currentProject_Modified(sender As Object, e As EventArgs) ' Handles _currentProject.Modified
        RaiseEvent ProjectModified(sender, e)
    End Sub

    Private Sub _pluginHelper_FileOpened(sender As Object, e As EventArguments.FileOpenedEventArguments)
        If Not Me.OpenedFiles.ContainsKey(e.File) Then
            Me.OpenedFiles.Add(e.File, e.ParentProject)
        End If
    End Sub

    Private Sub _pluginHelper_FileClosed(sender As Object, e As EventArguments.FileClosedEventArgs)
        If Me.OpenedFiles.ContainsKey(e.File) Then
            Me.OpenedFiles.Remove(e.File)
        End If
    End Sub

#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If CurrentSolution IsNot Nothing Then
                    CurrentSolution.Dispose()
                End If

                For Each item In Plugins
                    item.UnLoad(Me)
                Next
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
