Imports System.Threading.Tasks

Namespace ConsoleCommands
    Public Class InstallExtension
        Inherits ConsoleCommandAsync

        Public Overrides Async Function MainAsync(Arguments() As String) As Task
            If Arguments.Length > 0 Then
                If IO.File.Exists(Arguments(0)) Then
                    Dim result = Await Extensions.ExtensionHelper.InstallExtension(Arguments(0))
                    Select Case result
                        Case Extensions.ExtensionInstallResult.Success
                            Console.WriteLine("Extension install was successful.")
                        Case Extensions.ExtensionInstallResult.RestartRequired
                            Console.WriteLine("Application must be restarted to complete installation.")
                        Case Extensions.ExtensionInstallResult.InvalidFormat
                            Console.WriteLine("The provided zip file is not a Sky Editor extension.")
                        Case Extensions.ExtensionInstallResult.UnsupportedFormat
                            Console.WriteLine("The provided extension is not supported.  Is this an extension to an extension that's not currently installed?")
                        Case Else
                            Console.WriteLine("Unknown error.")
                    End Select
                Else
                    Console.WriteLine("File doesn't exist.")
                End If
            Else
                Console.WriteLine("Usage: InstallExtension <Filename>")
            End If
        End Function
    End Class
End Namespace

