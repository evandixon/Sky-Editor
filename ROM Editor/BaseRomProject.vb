Imports SkyEditorBase
Imports SkyEditorBase.EventArguments

Public Class BaseRomProject
    Inherits SkyEditorBase.Project

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
                Return PluginHelper.GetLanguageItem("Nintendo DS Roms") & " (*.nds)|*.nds|" & PluginHelper.GetLanguageItem("All Files") & " (*.*)|*.*"
            Case "3DS"
                Return PluginHelper.GetLanguageItem("Nintendo 3DS Roms") & " (*.3ds;*.3dz)|*.3ds;*.3dz|" & PluginHelper.GetLanguageItem("All Files") & " (*.*)|*.*"
            Case Else
                Return PluginHelper.GetLanguageItem("All Files") & " (*.*)|*.*"
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
            Using f As New GenericFile(e.FullFilename)
                'Then we have to detect the ROM type
                Dim n As New Roms.GenericNDSRom
                If n.IsFileOfType(f) Then
                    mode = "nds"
                    n.Dispose()
                Else
                    Dim three As New Roms.Generic3DSRom
                    If three.IsOfType(f) Then
                        mode = "3ds"
                        three.Dispose()
                    Else
                        'This file is invalid, and we'll delete it.
                        DeleteFile("/BaseRom")
                        Exit Sub
                    End If
                End If
            End Using
        End If

        Select Case mode
            Case "nds"
                Dim nds As New Roms.GenericNDSRom(e.FullFilename)
                Await nds.Unpack(GetRawFilesDir)
                Setting("System") = "NDS"
                Setting("GameCode") = nds.GameCode
                nds.Dispose()
            Case "3ds"
                Dim threeDS As New Roms.Generic3DSRom(e.FullFilename)
                Await threeDS.Unpack(GetRawFilesDir)
                Setting("System") = "3DS"
                Setting("GameCode") = threeDS.TitleID
                threeDS.Dispose()
        End Select

    End Sub

    Public Overridable Function GetRawFilesDir() As String
        Return IO.Path.Combine(IO.Path.GetDirectoryName(Me.Filename), "Raw Files")
    End Function
End Class
