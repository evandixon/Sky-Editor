﻿Imports System.Text
Imports System.Web.Script.Serialization

Public Class Project
    Private Class ProjectFileFormat
        Public Property ProjectType As String
        Public Property Files As List(Of String)
        Public Sub New()
            Files = New List(Of String)
        End Sub
    End Class
    Dim _manager As PluginManager
    Public Property Filename As String
    Public Property Files As Dictionary(Of String, GenericFile)
    Private Property ProjectType As String

#Region "Constructors"
    Public Sub New()
        Files = New Dictionary(Of String, GenericFile)
    End Sub
    Public Sub New(Manager As PluginManager)
        Me.New()
        _manager = Manager
    End Sub
    Public Sub New(Name As String, Location As String, ProjectType As String, Manager As PluginManager)
        Me.New(Manager)
        If Not IO.Directory.Exists(IO.Path.Combine(Location, Name)) Then
            IO.Directory.CreateDirectory(IO.Path.Combine(Location, Name))
        End If
        Me.Filename = IO.Path.Combine(Location, Name, Name & ".skyproj")
        Me.ProjectType = ProjectType
    End Sub
#End Region

    Public Shared Function CreateProject(Name As String, Location As String, ProjectType As String, Manager As PluginManager)
        Dim p As Project
        If Not String.IsNullOrEmpty(ProjectType) Then
            p = Manager.ProjectTypes(ProjectType).GetConstructor({}).Invoke({})
        Else
            p = New Project
        End If
        p.SetManager(Manager)
        If Not IO.Directory.Exists(IO.Path.Combine(Location, Name)) Then
            IO.Directory.CreateDirectory(IO.Path.Combine(Location, Name))
        End If
        p.Filename = IO.Path.Combine(Location, Name, Name & ".skyproj")
        p.ProjectType = ProjectType
        p.Initialize()
        Return p
    End Function

    Public Shared Function OpenProject(Filename As String, Manager As PluginManager) As Project
        Dim j As New JavaScriptSerializer
        Dim f = j.Deserialize(Of ProjectFileFormat)(IO.File.ReadAllText(Filename))
        Dim p As Project
        If Not String.IsNullOrEmpty(f.ProjectType) Then
            p = Manager.ProjectTypes(f.ProjectType).GetConstructor({}).Invoke({})
        Else
            p = New Project
        End If
        p.SetManager(Manager)
        p.Filename = Filename
        p.ProjectType = f.ProjectType
        For Each item In f.Files
            If item.EndsWith("/") OrElse item.EndsWith("\") Then
                p.Files.Add(item, Nothing)
            Else
                p.Files.Add(item, Manager.OpenFile(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), item)))
            End If
        Next
        p.Opened()
        Return p
    End Function

#Region "Save"
    Public Sub SaveProject()
        Dim j As New JavaScriptSerializer
        Dim f As New ProjectFileFormat
        For Each item In Files
            f.Files.Add(item.Key)
        Next
        f.ProjectType = Me.ProjectType
        IO.File.WriteAllText(Filename, j.Serialize(f))
    End Sub
    Public Sub SaveAll()
        For Each item In Files
            item.Value.Save()
        Next
        SaveProject()
    End Sub
#End Region

#Region "Events"
    Public Event FileAdded(sender As Object, File As KeyValuePair(Of String, GenericFile))
    Public Event FileRemoved(sender As Object, File As String)
    Public Event DirectoryCreated(sender As Object, Directory As String)
    Public Event DirectoryRemoved(sender As Object, Directory As String)
#End Region
    Private Function JoinDirs(Dirs As String(), Depth As Integer) As String
        Dim out As New StringBuilder
        For count = 0 To Depth
            out.Append(Dirs(count))
            out.Append("/")
        Next
        Return out.ToString
    End Function
    Public Function GetDirectories(InternalPath As String, Optional Recursive As Boolean = False) As List(Of String)
        Dim currentDepth As Integer = (From part In InternalPath.Replace("\", "/").Split("/") Where Not String.IsNullOrEmpty(part)).Count
        Dim availableDirectories As New List(Of String)
        For Each item In Files
            If item.Key.StartsWith(InternalPath) AndAlso item.Key.EndsWith("/") OrElse item.Key.EndsWith("\") Then
                Dim dirs As String() = item.Key.Trim("/").Replace("\", "/").Split("/")
                If dirs.Length - 1 >= currentDepth Then
                    If Not availableDirectories.Contains(JoinDirs(dirs, currentDepth)) Then
                        availableDirectories.Add(JoinDirs(dirs, currentDepth))
                    End If
                End If
            End If
        Next
        'If Not String.IsNullOrEmpty(InternalPath) Then
        '    availableDirectories = (From file In Files Where file.Key.StartsWith(InternalPath) AndAlso Not String.IsNullOrEmpty(InternalPath) AndAlso (file.Key.EndsWith("/") OrElse file.Key.EndsWith("\")) Select file.Key.Replace(InternalPath, ""))
        'Else
        '    availableDirectories = (From file In Files Where file.Key.StartsWith(InternalPath) AndAlso Not String.IsNullOrEmpty(InternalPath) AndAlso (file.Key.EndsWith("/") OrElse file.Key.EndsWith("\")) Select file.Key)
        'End If

            Return availableDirectories.ToList
    End Function
    Public Function GetFiles(InternalPath As String) As List(Of String)
        Dim currentDepth As Integer = (From part In InternalPath.Replace("\", "/").Split("/") Where Not String.IsNullOrEmpty(part)).Count
        Dim out As New List(Of String)
        For Each item In Files
            If item.Key.StartsWith(InternalPath) AndAlso item.Key.Replace("\", "/").Split("/").Count - 1 = currentDepth AndAlso Not (item.Key.EndsWith("\") OrElse item.Key.EndsWith("/")) Then
                out.Add(item.Key)
            End If
        Next
        Return out
        'If Not String.IsNullOrEmpty(InternalPath) Then
        '    Return (From file In Files Where file.Key.StartsWith(InternalPath.Replace("\", "/").Trim("/")) AndAlso Not String.IsNullOrEmpty(InternalPath) Select file.Key).ToList
        'Else
        '    Return (From file In Files Where file.Key.Contains("/") OrElse file.Key.Contains("\") AndAlso Not String.IsNullOrEmpty(InternalPath) Select file.Key).ToList
        'End If
    End Function
    ''' <summary>
    ''' Determines whether or not a directory can be created inside the given path.
    ''' </summary>
    ''' <param name="InternalPath"></param>
    ''' <returns></returns>
    Public Overridable Function CanCreateDirectory(InternalPath As String) As Boolean
        Return True
    End Function
    ''' <summary>
    ''' Returns File Type IDs of files that can be created inside the given path..
    ''' </summary>
    ''' <param name="InternalPath"></param>
    ''' <returns></returns>
    Public Overridable Function CreatableFiles(InternalPath As String, Manager As PluginManager) As IList(Of String)
        Return Manager.SaveTypes.Keys.ToList
    End Function


    Public Sub CreateDirectory(InternalPath As String, Optional CreateInFileSystem As Boolean = True)
        Dim path As String
        If InternalPath.EndsWith("/") OrElse InternalPath.EndsWith("\") Then
            path = InternalPath
        Else
            path = InternalPath & "/"
        End If
        Files.Add(path, Nothing)
        If CreateInFileSystem Then
            If Not IO.Directory.Exists(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), InternalPath)) Then
                IO.Directory.CreateDirectory(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), InternalPath))
            End If
        End If
        RaiseEvent DirectoryCreated(Me, path)
    End Sub
    Public Sub RemoveDirectory(InternalPath As String)
        Files.Remove(InternalPath)
        Dim toDelete = (From file In Files Where file.Key.StartsWith(InternalPath))
        For Each item In toDelete
            Files.Remove(item.Key)
        Next
        RaiseEvent DirectoryRemoved(Me, InternalPath)
    End Sub
    Public Sub OpenFile(SourceFilename As String, NewInternalPath As String, Optional Copy As Boolean = True)
        Dim newPath = IO.Path.Combine(IO.Path.GetDirectoryName(Me.Filename), NewInternalPath)
        If Copy Then IO.File.Copy(SourceFilename, newPath)
        AddFile(NewInternalPath, _manager.OpenFile(SourceFilename))
    End Sub
    Public Sub AddFile(InternalPath As String, File As GenericFile)
        'Files.Add(File.OriginalFilename.Replace(IO.Path.GetDirectoryName(Me.Filename), ""), File)
        Files.Add(InternalPath, File)
        RaiseEvent FileAdded(Me, New KeyValuePair(Of String, GenericFile)(InternalPath, File))
    End Sub
    Public Sub RemoveFile(InternalPath As String)
        Files.Remove(InternalPath)
        RaiseEvent FileRemoved(Me, InternalPath)
    End Sub


    ''' <summary>
    ''' This is called after the project has been created.
    ''' </summary>
    ''' <remarks></remarks>
    Public Overridable Sub Initialize()

    End Sub

    ''' <summary>
    ''' This is called after the project has been opened.
    ''' Use this to ensure all required assets are present.
    ''' </summary>
    ''' <remarks></remarks>
    Public Overridable Sub Opened()

    End Sub

    Public Overridable Sub Build()

    End Sub

    Public Overridable Sub Run()

    End Sub

    Protected Sub SetManager(Manager As PluginManager)
        _manager = Manager
    End Sub
End Class