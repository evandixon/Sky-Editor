﻿Imports System.IO
Imports Microsoft.VisualBasic.Devices

Public Class IOProvider
    Inherits SkyEditor.Core.IO.IOProvider

    Public Overrides Sub CopyFile(sourceFilename As String, destinationFilename As String)
        File.Copy(sourceFilename, destinationFilename, True)
    End Sub

    Public Overrides Sub DeleteFile(filename As String)
        File.Delete(filename)
    End Sub

    Public Overrides Sub WriteAllBytes(filename As String, data() As Byte)
        File.WriteAllBytes(filename, data)
    End Sub

    Public Overrides Sub WriteAllText(filename As String, data As String)
        File.WriteAllText(filename, data)
    End Sub

    Public Overrides Function CanLoadFileInMemory(fileSize As Long) As Boolean
        Return (New ComputerInfo).AvailablePhysicalMemory > (fileSize + 500 * 1024 * 1024)
    End Function

    Public Overrides Function DirectoryExists(directoryPath As String) As Boolean
        Return Directory.Exists(directoryPath)
    End Function

    Public Overrides Function fileExists(Filename As String) As Boolean
        Return File.Exists(Filename)
    End Function

    Public Overrides Function GetFileLength(filename As String) As Long
        Return (New FileInfo(filename)).Length
    End Function

    Public Overrides Function GetTempFilename() As String
        Return Path.GetTempFileName
    End Function

    Public Overrides Function OpenFile(filename As String) As Stream
        Return File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read)
    End Function

    Public Overrides Function OpenFileReadOnly(filename As String) As Stream
        Return File.Open(filename, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite)
    End Function

    Public Overrides Function OpenFileWriteOnly(filename As String) As Stream
        Return File.Open(filename, FileMode.OpenOrCreate, FileAccess.Write)
    End Function

    Public Overrides Function ReadAllBytes(filename As String) As Byte()
        Return File.ReadAllBytes(filename)
    End Function

    Public Overrides Function ReadAllText(filename As String) As String
        Return File.ReadAllText(filename)
    End Function
End Class