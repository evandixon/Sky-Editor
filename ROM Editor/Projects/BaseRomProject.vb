Imports SkyEditorBase
Imports SkyEditorBase.EventArguments

Namespace Projects
    Public Class BaseRomProject
        Inherits SkyEditorBase.Project

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

        Public Overrides Function GetImportIOFilter(ParentProjectPath As String) As String
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
            Dim mode As String = Nothing

            If Me.Settings.ContainsKey("System") Then
                If Me.Settings("System") = "NDS" Then
                    mode = "nds"
                ElseIf Me.Settings("System") = "3DS" Then
                    mode = "3ds"
                End If
            End If

            If mode Is Nothing Then
                Using f As New GenericFile
                    f.OpenFile(e.FullFilename)
                    'Then we have to detect the ROM type
                    Dim n As New Roms.GenericNDSRom
                    If n.IsFileOfType(f) Then
                        mode = "nds"
                        n.Dispose()
                    Else
                        Dim three As New Roms.Generic3DSRom
                        Dim cxi As New Roms.Cxi3DSRom
                        If three.IsOfType(f) Then
                            mode = "3ds"
                            three.Dispose()
                        ElseIf cxi.IsOfType(f) Then
                            mode = "cxi"
                        Else
                            'This file is invalid, and we'll delete it.
                            DeleteFile("/BaseRom")
                            Exit Sub
                        End If
                    End If
                End Using
            End If

            PluginHelper.SetLoadingStatus(My.Resources.Language.LoadingUnpacking)
            Select Case mode
                Case "nds"
                    Dim nds As New Roms.GenericNDSRom()
                    nds.OpenFile(e.FullFilename)
                    Await nds.UnpackWithNDSTool(GetRawFilesDir)
                    'Await nds.Unpack(GetRawFilesDir)
                    Setting("System") = "NDS"
                    Setting("GameCode") = nds.GameCode
                    nds.Dispose()
                Case "3ds"
                    Dim threeDS As New Roms.Generic3DSRom
                    threeDS.IsReadOnly = True
                    threeDS.OpenFile(e.FullFilename)
                    Await threeDS.Unpack(GetRawFilesDir)
                    Setting("System") = "3DS"
                    Setting("GameCode") = threeDS.TitleID
                    threeDS.Dispose()
                Case "cxi"
                    Dim threeDS As New Roms.Cxi3DSRom
                    threeDS.IsReadOnly = True
                    threeDS.OpenFile(e.FullFilename)
                    Await threeDS.Unpack(GetRawFilesDir)
                    Setting("System") = "3DS"
                    Setting("GameCode") = threeDS.TitleID
                    threeDS.Dispose()
            End Select

            Dim filename = Me.GetProjectItemByPath("/BaseRom").GetFilename
            DeleteFile("/BaseRom")
            IO.File.Delete(filename)

            PluginHelper.SetLoadingStatusFinished()
        End Sub

        Public Overridable Function GetRawFilesDir() As String
            Return IO.Path.Combine(IO.Path.GetDirectoryName(Me.Filename), "Raw Files")
        End Function

        Public Overridable Function GetIconPath() As String
            Return IO.Path.Combine(GetRawFilesDir, "exefs", "icon.bin")
        End Function
    End Class

End Namespace
