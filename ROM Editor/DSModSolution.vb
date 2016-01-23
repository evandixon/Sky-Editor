Imports SkyEditorBase
Imports SkyEditorBase.EventArguments

Public Class DSModSolution
    Inherits SkyEditorBase.Solution

    Public Overrides Function CanCreateDirectory(Path As String) As Boolean
        Return False
    End Function

    Public Overrides Function CanCreateProject(Path As String) As Boolean
        Return (Path.Replace("\", "/").TrimStart("/") = "")
    End Function

    Public Overrides Function GetSupportedProjectTypes(Path As String) As IEnumerable(Of Type)
        Dim baseRomProject As BaseRomProject = GetProjectsByName(Me.Setting("BaseRomProject")).FirstOrDefault
        If baseRomProject Is Nothing OrElse baseRomProject.Setting("System") Is Nothing OrElse baseRomProject.Setting("GameCode") Is Nothing Then
            Return {}
        Else
            Dim matches As New List(Of Type)
            For Each item In PluginManager.GetInstance.GetRegisteredObjects(GetType(GenericModProject))
                Dim games = item.GetSupportedGameCodes
                Dim match As Boolean = False
                For Each t In games
                    Dim r As New Text.RegularExpressions.Regex(t)
                    If r.IsMatch(baseRomProject.Setting("GameCode")) Then
                        matches.Add(item.GetType)
                    End If
                Next
            Next
            Return matches
        End If
    End Function

    Private Sub DSModSolution_Created(sender As Object, e As EventArgs) Handles Me.Created
        Me.Setting("BaseRomProject") = "BaseRom"
        Me.Setting("ModPackProject") = "ModPack"
        CreateProject("", "BaseRom", GetType(BaseRomProject))
        CreateProject("", "ModPack", GetType(DSModPackProject))
    End Sub

    Private Async Sub DSModSolution_ProjectAdded(sender As Object, e As ProjectAddedEventArgs) Handles Me.ProjectAdded
        If TypeOf e.Project Is GenericModProject Then
            Dim m = DirectCast(e.Project, GenericModProject)
            m.Setting("SourceProject") = Me.Setting("BaseRomProject")
            m.Setting("TargetProject") = Me.Setting("ModPackProject")
            m.Setting("ModDependenciesAfter") = ""
            m.Setting("ModDependenciesAfter") = ""
            m.Setting("ModName") = e.Project.Name
            m.Setting("ModVersion") = "1.0.0"
            m.Setting("ModAuthor") = "Unknown"
            m.Setting("ModDescription") = "A generic mod"
            m.Setting("ModUpdateUrl") = ""
            Try
                Await m.Initialize(Me)
            Catch ex As Exception
                PluginHelper.ReportExceptionThrown(Me, ex)
                PluginHelper.SetLoadingStatusFailed()
            End Try
        ElseIf TypeOf e.Project Is DSModPackProject Then
            Dim m = DirectCast(e.Project, DSModPackProject)
            m.Setting("ModPackName") = Me.Name
            m.Setting("ModPackVersion") = "1.0.0"
            m.Setting("ModPackAuthor") = "Unknown"
            m.Setting("ModPackDescription") = "A generic modpack"
            m.Setting("ModPackUpdateUrl") = ""
            m.Setting("ModpackInfo") = New ModpackInfo With {.Name = Me.Name}
        End If
    End Sub

    Public Overrides Async Function Build() As Task
        Dim info As ModpackInfo = Me.Setting("ModpackInfo")
        If info Is Nothing Then
            info = New ModpackInfo
            Me.Setting("ModpackInfo") = info
        End If
        Dim baseRomProject As BaseRomProject = GetProjectsByName(Me.Setting("BaseRomProject")).FirstOrDefault
        If baseRomProject IsNot Nothing Then
            info.System = baseRomProject.Setting("System")
            info.GameCode = baseRomProject.Setting("GameCode")
            Me.Setting("System") = info.System
            Me.Setting("GameCode") = info.GameCode
        End If
        Dim modPacks As New List(Of DSModPackProject)
        Dim allProjects As New List(Of Project)(Me.GetAllProjects)
        Dim built As Integer = 0
        For Each item In allProjects
            PluginHelper.SetLoadingStatus(PluginHelper.GetLanguageItem("Building projects..."), built / allProjects.Count)
            If TypeOf item Is DSModPackProject Then
                modPacks.Add(item)
            Else
                Await item.Build(Me)
                built += 1
            End If
        Next
        For Each item In modPacks
            PluginHelper.SetLoadingStatus(PluginHelper.GetLanguageItem("Building projects..."), built / allProjects.Count)
            Await item.Build(Me)
            built += 1
        Next
        PluginHelper.SetLoadingStatusFinished()
    End Function
End Class
