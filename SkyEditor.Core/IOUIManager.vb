Imports System.Text
Imports SkyEditor.Core.IO
''' <summary>
''' Class that manages open files, solutions, and projects, and helps with the UI display them.
''' </summary>
Public Class IOUIManager
    Implements IDisposable

    Public Sub New()
        Me.CurrentSolution = Nothing
        Me.OpenedFiles = New Dictionary(Of Object, Project)
    End Sub

#Region "Events"
    Public Event SolutionChanged(sender As Object, e As EventArgs)
    Public Event CurrentProjectChanged(sender As Object, e As EventArgs)
#End Region

#Region "Event Handlers"

    '<Obsolete> Private Sub _pluginHelper_FileOpened(sender As Object, e As FileOpenedEventArguments)
    '    If Not Me.OpenedFiles.ContainsKey(e.File) Then
    '        Me.OpenedFiles.Add(e.File, e.ParentProject)
    '    End If
    'End Sub

    '<Obsolete> Private Sub _pluginHelper_FileClosed(sender As Object, e As FileClosedEventArgs)
    '    If Me.OpenedFiles.ContainsKey(e.File) Then
    '        Me.OpenedFiles.Remove(e.File)
    '    End If
    'End Sub

#End Region

    ''' <summary>
    ''' Matches opened files to their parent projects
    ''' </summary>
    ''' <returns></returns>
    Private Property OpenedFiles As Dictionary(Of Object, Project)

    ''' <summary>
    ''' Dictionary of (Extension, Friendly Name) used in the Open and Save file dialogs.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property IOFilters As New Dictionary(Of String, String)

    Public Property CurrentSolution As Solution
        Get
            Return _currentSolution
        End Get
        Set(value As Solution)
            If _currentSolution IsNot Nothing Then _currentSolution.Dispose()
            _currentSolution = value
            RaiseEvent SolutionChanged(Me, New EventArgs)
        End Set
    End Property
    Private WithEvents _currentSolution As Solution

    Public Property CurrentProject As Project
        Get
            Return _currentProject
        End Get
        Set(value As Project)
            _currentProject = value
            RaiseEvent CurrentProjectChanged(Me, New EventArgs)
        End Set
    End Property
    Private WithEvents _currentProject As Project

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

    ''' <summary>
    ''' Registers a filter for use in open and save file dialogs.
    ''' </summary>
    ''' <param name="FileExtension">Filter for the dialog.  If this is by extension, should be *.extension</param>
    ''' <param name="FileFormatName">Name of the file format</param>
    Public Sub RegisterIOFilter(FileExtension As String, FileFormatName As String)
        Dim TempIOFilters As Dictionary(Of String, String) = IOFilters
        If TempIOFilters Is Nothing Then
            TempIOFilters = New Dictionary(Of String, String)
        End If
        If Not TempIOFilters.ContainsKey(FileExtension) Then
            TempIOFilters.Add(FileExtension, FileFormatName)
        End If
        IOFilters = TempIOFilters
    End Sub
#End Region

    ''' <summary>
    ''' Returns the file's parent project, if it exists.
    ''' </summary>
    ''' <param name="File">File of which to get the parent project.  Must be an open file, otherwise the function will return Nothing.</param>
    ''' <returns></returns>
    Public Function GetOpenedFileProject(File As Object) As Project
        If Me.OpenedFiles.ContainsKey(File) Then
            Return Me.OpenedFiles(File)
        Else
            Return Nothing
        End If
    End Function

#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If CurrentSolution IsNot Nothing Then
                    CurrentSolution.Dispose()
                End If
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
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
