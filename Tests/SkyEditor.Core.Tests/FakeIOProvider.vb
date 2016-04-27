Imports System.IO

Public Class FakeIOProvider
    Inherits IOProvider

    Public Overrides Sub CopyFile(SourceFilename As String, DestinationFilename As String)
    End Sub

    Public Overrides Sub DeleteFile(Filename As String)
    End Sub

    Public Overrides Sub WriteAllBytes(Filename As String, Data() As Byte)
    End Sub

    Public Overrides Sub WriteAllText(Filename As String, Data As String)
    End Sub

    Public Overrides Function CanLoadFileInMemory(FileSize As Long) As Boolean
        Return True
    End Function

    Public Overrides Function FileExists(Filename As String) As Boolean
    End Function

    Public Overrides Function GetFileLength(Filename As String) As Long
    End Function

    Public Overrides Function GetTempFilename() As String
    End Function

    Public Overrides Function OpenFile(Filename As String) As Stream
    End Function

    Public Overrides Function OpenFileReadOnly(Filename As String) As Stream
    End Function

    Public Overrides Function OpenFileWriteOnly(Filename As String) As Stream
    End Function

    Public Overrides Function ReadAllBytes(Filename As String) As Byte()
    End Function

    Public Overrides Function ReadAllText(Filename As String) As String
    End Function
End Class
