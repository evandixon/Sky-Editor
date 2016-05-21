﻿Imports System.Threading.Tasks
Imports SkyEditor.Core.ConsoleCommands
Imports SkyEditor.Core.Extensions
Imports SkyEditor.Core.Windows

Namespace ConsoleCommands
    Public Class InstallExtension
        Inherits ConsoleCommandAsync

        Public Overrides Async Function MainAsync(Arguments() As String) As Task
            If Arguments.Length > 0 Then
                If IO.File.Exists(Arguments(0)) Then
                    Dim result = Await ExtensionHelper.InstallExtension(Arguments(0), EnvironmentPaths.GetExtensionDirectory, CurrentPluginManager)
                    Select Case result
                        Case ExtensionInstallResult.Success
                            Console.WriteLine("Extension install was successful.")
                        Case ExtensionInstallResult.RestartRequired
                            Console.WriteLine("Application must be restarted to complete installation.")
                        Case ExtensionInstallResult.InvalidFormat
                            Console.WriteLine("The provided zip file is not a Sky Editor extension.")
                        Case ExtensionInstallResult.UnsupportedFormat
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

