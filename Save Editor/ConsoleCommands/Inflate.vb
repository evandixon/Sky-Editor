Namespace ConsoleCommands
    Public Class Inflate
        Inherits SkyEditorBase.ConsoleCommand

        Public Overrides Sub Main(Arguments() As String)
            If IO.File.Exists(Arguments(0)) Then
                Dim f As New IO.FileStream(Arguments(0), IO.FileMode.Open)
                f.Seek(0, IO.SeekOrigin.Begin)
                Dim z As New Ionic.Zlib.DeflateStream(f, Ionic.Zlib.CompressionMode.Decompress)
                Dim d As New IO.FileStream(Arguments(0) & "-deflated", IO.FileMode.OpenOrCreate)
                d.Seek(0, IO.SeekOrigin.Begin)
                z.CopyTo(d)
                f.Dispose()
                z.Dispose()
                d.Dispose()
            End If
        End Sub

        Public Overrides ReadOnly Property CommandName As String
            Get
                Return "inflate"
            End Get
        End Property
    End Class
End Namespace