Imports System.IO
Imports System.IO.Compression
Imports SkyEditorBase

Namespace ConsoleCommands
    Public Class Gzip
        Inherits SkyEditorBase.ConsoleCommand

        Public Overrides Sub Main(Arguments() As String)
            Throw New NotImplementedException
            'Dim s As New GenericFile(System.Text.UnicodeEncoding.Unicode.GetBytes(Arguments(0)))
            'Dim m As New MemoryStream(s.Length)
            'Dim g As New GZipStream(m, CompressionMode.Compress)
            's.FileReader.Position = 0
            's.FileReader.CopyTo(g)
            'm.Position = 0
            'Dim b = m.ReadByte
            'While b > -1
            '    Console.Write(Hex(b).PadLeft(2, "0"c))
            '    b = m.ReadByte
            'End While
        End Sub
    End Class

End Namespace
