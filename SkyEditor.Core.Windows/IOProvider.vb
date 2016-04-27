Imports System.IO
Imports Microsoft.VisualBasic.Devices

Public Class IOProvider
    Inherits SkyEditor.Core.IOProvider
    Public Overrides Sub CopyFile(SourceFilename As String, DestinationFilename As String)
        IO.File.Copy(SourceFilename, DestinationFilename, True)
    End Sub

    Public Overrides Sub DeleteFile(Filename As String)
        IO.File.Delete(Filename)
    End Sub

    Public Overrides Sub WriteAllBytes(Filename As String, Data() As Byte)
        IO.File.WriteAllBytes(Filename, Data)
    End Sub

    Public Overrides Sub WriteAllText(Filename As String, Data As String)
        IO.File.WriteAllText(Filename, Data)
    End Sub

    Public Overrides Function CanLoadFileInMemory(FileSize As Long) As Boolean
        Return (New ComputerInfo).AvailablePhysicalMemory > (FileSize + 500 * 1024 * 1024)
    End Function

    Public Overrides Function FileExists(Filename As String) As Boolean
        Return IO.File.Exists(Filename)
    End Function

    Public Overrides Function GetFileLength(Filename As String) As Long
        Return (New IO.FileInfo(Filename)).Length
    End Function

    Public Overrides Function GetTempFilename() As String
        Return IO.Path.GetTempFileName
    End Function

    Public Overrides Function OpenFile(Filename As String) As Stream
        Return IO.File.Open(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read)
    End Function

    Public Overrides Function OpenFileReadOnly(Filename As String) As Stream
        Return IO.File.Open(Filename, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite)
    End Function

    Public Overrides Function OpenFileWriteOnly(Filename As String) As Stream
        Return IO.File.Open(Filename, FileMode.OpenOrCreate, FileAccess.Write)
    End Function

    Public Overrides Function ReadAllBytes(Filename As String) As Byte()
        Return IO.File.ReadAllBytes(Filename)
    End Function

    Public Overrides Function ReadAllText(Filename As String) As String
        Return IO.File.ReadAllText(Filename)
    End Function
End Class
