Namespace ConsoleCommands
    Public Class BitSum
        Inherits SkyEditorBase.ConsoleCommand

        Public Overrides Sub Main(Arguments() As String)
            If IO.File.Exists(Arguments(0)) Then
                Dim sum As UInteger = 0
                Dim b As New Binary(IO.File.ReadAllBytes(Arguments(0)))
                For Each item In b.Bits
                    If item Then
                        sum += 1
                    End If
                Next
                Console.WriteLine(sum)
            End If
        End Sub
    End Class
End Namespace

