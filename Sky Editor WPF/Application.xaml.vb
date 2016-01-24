Imports System.IO
Imports System.Reflection
Imports SkyEditorBase.Redistribution

Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.
    Private Sub Application_Exit(sender As Object, e As ExitEventArgs) Handles Me.Exit
        StartupHelpers.RunExitSequence()
    End Sub

    Private Async Sub Application_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
        Await StartupHelpers.RunWPFStartupSequence
    End Sub
End Class
