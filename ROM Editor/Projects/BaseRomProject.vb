Imports System.IO
Imports SkyEditor.Core
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Windows
Imports SkyEditor.ROMEditor

Namespace Projects
    Public Class BaseRomProject
        Inherits Project

        Public Property RomSystem As String
            Get
                Return Setting("System")
            End Get
            Set(value As String)
                Setting("System") = value
            End Set
        End Property

        Public Property GameCode As String
            Get
                Return Setting("GameCode")
            End Get
            Set(value As String)
                Setting("GameCode") = value
            End Set
        End Property

        Public Overrides Function CanBuild() As Boolean
            Return (Me.GetProjectItemByPath("/BaseRom") IsNot Nothing)
        End Function

        Public Overrides Function CanCreateDirectory(Path As String) As Boolean
            Return False
        End Function

        Public Overrides Function CanCreateFile(Path As String) As Boolean
            Return False
        End Function

        Public Overrides Function CanDeleteDirectory(Path As String) As Boolean
            Return False
        End Function

        Public Overrides Function CanAddExistingFile(Path As String) As Boolean
            'Only if it's the root, and there isn't already a file named BaseRom.
            Return (Path.Replace("\", "/").TrimStart("/") = "") AndAlso (Me.GetProjectItemByPath("/BaseRom") Is Nothing)
        End Function

        Public Overrides Function CanDeleteFile(FilePath As String) As Boolean
            Return (FilePath.Replace("\", "/").TrimStart("/").ToLower = "baserom")
        End Function

        Public Overrides Function GetImportIOFilter(ParentProjectPath As String, manager As PluginManager) As String
            Select Case Me.Setting("System")
                Case "NDS"
                    Return $"{My.Resources.Language.NDSRomFile} (*.nds)|*.nds|{My.Resources.Language.AllFiles} (*.*)|*.*"
                Case "3DS"
                    Return $"{My.Resources.Language.ThreeDSRomFile} (*.3ds;*.3dz)|*.3ds;*.3dz|{My.Resources.Language.AllFiles} (*.*)|*.*"
                Case Else
                    Return $"{My.Resources.Language.AllFiles} (*.*)|*.*"
            End Select
        End Function

        Protected Overrides Function GetImportedFilePath(ParentProjectPath As String, FullFilename As String) As Object
            Return "BaseRom"
        End Function

        Private Async Sub BaseRomProject_FileAdded(sender As Object, e As ProjectFileAddedEventArgs) Handles Me.FileAdded
            'Calling DoBuild directly would result in the solution not raising the build event, which is needed for SolutionBuildProgress
            Await ParentSolution.Build({Me})
        End Sub

        Public Overrides Async Function Build() As Task
            Await MyBase.Build
            If CanBuild() Then
                Await DoBuild()
            End If
        End Function

        Private Async Function DoBuild() As Task
            Dim mode As String = Nothing
            Dim fullPath = Me.GetProjectItemByPath("/BaseRom").GetFilename

            If Me.Settings.GetSetting("System") IsNot Nothing Then
                If Me.Setting("System") = "NDS" Then
                    mode = "nds"
                ElseIf Me.Setting("System") = "3DS" Then
                    mode = "3ds"
                End If
            End If

            If mode Is Nothing Then
                Using f As New GenericFile
                    Await f.OpenFile(fullPath, CurrentPluginManager.CurrentIOProvider)
                    'Then we have to detect the ROM type
                    Dim n As New GenericNDSRom
                    If Await n.IsFileOfType(f) Then
                        mode = "nds"
                        n.Dispose()
                    Else
                        Dim three As New Roms.Generic3DSRom
                        Dim cxi As New Roms.Cxi3DSRom
                        If Await three.IsOfType(f) Then
                            mode = "3ds"
                            three.Dispose()
                        ElseIf Await cxi.IsOfType(f) Then
                            mode = "cxi"
                        Else
                            'This file is invalid, and we'll delete it.
                            DeleteFile("/BaseRom")
                            Exit Function
                        End If
                    End If
                End Using
            End If

            Me.BuildProgress = 0
            Me.BuildStatusMessage = My.Resources.Language.LoadingUnpacking
            Select Case mode
                Case "nds"
                    Dim nds As New GenericNDSRom
                    Await nds.OpenFile(fullPath, CurrentPluginManager.CurrentIOProvider)
                    AddHandler nds.UnpackProgress, Sub(sender As Object, e As UnpackProgressEventArgs)
                                                       Me.BuildProgress = e.Progress
                                                   End Sub
                    Await nds.Unpack(GetRawFilesDir, CurrentPluginManager.CurrentIOProvider)
                    Setting("System") = "NDS"
                    Setting("GameCode") = nds.GameCode
                    nds.Dispose()
                Case "3ds"
                    IsBuildProgressIndeterminate = True
                    Dim threeDS As New Roms.Generic3DSRom
                    threeDS.IsReadOnly = True
                    Await threeDS.OpenFile(fullPath, CurrentPluginManager.CurrentIOProvider)
                    Await threeDS.Unpack(GetRawFilesDir, CurrentPluginManager.CurrentIOProvider)
                    Setting("System") = "3DS"
                    Setting("GameCode") = threeDS.TitleID
                    threeDS.Dispose()
                Case "cxi"
                    IsBuildProgressIndeterminate = True
                    Dim threeDS As New Roms.Cxi3DSRom
                    threeDS.IsReadOnly = True
                    Await threeDS.OpenFile(fullPath, CurrentPluginManager.CurrentIOProvider)
                    Await threeDS.Unpack(GetRawFilesDir, CurrentPluginManager.CurrentIOProvider)
                    Setting("System") = "3DS"
                    Setting("GameCode") = threeDS.TitleID
                    threeDS.Dispose()
            End Select

            Dim filename = Me.GetProjectItemByPath("/BaseRom").GetFilename
            DeleteFile("/BaseRom")
            File.Delete(filename)

            Me.IsBuildProgressIndeterminate = False
            Me.BuildProgress = 1
            Me.BuildStatusMessage = My.Resources.Language.Complete
        End Function

        Public Overridable Function GetRawFilesDir() As String
            Return Path.Combine(Path.GetDirectoryName(Me.Filename), "Raw Files")
        End Function

        Public Overridable Function GetIconPath() As String
            Return Path.Combine(GetRawFilesDir, "exefs", "icon.bin")
        End Function
    End Class

End Namespace
