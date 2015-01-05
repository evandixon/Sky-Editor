'
'This code was provided by John Leidegren for the Stack Overflow thread located at http://stackoverflow.com/questions/160587/no-output-to-console-from-a-wpf-application
'
Imports System.IO
Imports System.Security
Imports System.Runtime.InteropServices

<SuppressUnmanagedCodeSecurity> _
Public NotInheritable Class ConsoleManager
    Private Sub New()
    End Sub
    Private Const Kernel32_DllName As String = "kernel32.dll"

    <DllImport(Kernel32_DllName)> _
    Private Shared Function AllocConsole() As Boolean
    End Function

    <DllImport(Kernel32_DllName)> _
    Private Shared Function FreeConsole() As Boolean
    End Function

    <DllImport(Kernel32_DllName)> _
    Private Shared Function GetConsoleWindow() As IntPtr
    End Function

    <DllImport(Kernel32_DllName)> _
    Private Shared Function GetConsoleOutputCP() As Integer
    End Function

    Public Shared ReadOnly Property HasConsole() As Boolean
        Get
            Return GetConsoleWindow() <> IntPtr.Zero
        End Get
    End Property

    ''' <summary>
    ''' Creates a new console instance if the process is not attached to a console already.
    ''' </summary>
    Public Shared Sub Show()
        '#if DEBUG
        If Not HasConsole Then
            AllocConsole()
            InvalidateOutAndError()
        End If
        '#endif
    End Sub

    ''' <summary>
    ''' If the process has a console attached to it, it will be detached and no longer visible. Writing to the System.Console is still possible, but no output will be shown.
    ''' </summary>
    Public Shared Sub Hide()
        '#if DEBUG
        If HasConsole Then
            SetOutAndErrorNull()
            FreeConsole()
        End If
        '#endif
    End Sub

    Public Shared Sub Toggle()
        If HasConsole Then
            Hide()
        Else
            Show()
        End If
    End Sub

    Private Shared Sub InvalidateOutAndError()
        Dim type As Type = GetType(System.Console)

        Dim _out As System.Reflection.FieldInfo = type.GetField("_out", System.Reflection.BindingFlags.[Static] Or System.Reflection.BindingFlags.NonPublic)

        Dim _error As System.Reflection.FieldInfo = type.GetField("_error", System.Reflection.BindingFlags.[Static] Or System.Reflection.BindingFlags.NonPublic)

        Dim _InitializeStdOutError As System.Reflection.MethodInfo = type.GetMethod("InitializeStdOutError", System.Reflection.BindingFlags.[Static] Or System.Reflection.BindingFlags.NonPublic)

        Debug.Assert(_out IsNot Nothing)
        Debug.Assert(_error IsNot Nothing)

        Debug.Assert(_InitializeStdOutError IsNot Nothing)

        _out.SetValue(Nothing, Nothing)
        _error.SetValue(Nothing, Nothing)

        _InitializeStdOutError.Invoke(Nothing, New Object() {True})
    End Sub

    Private Shared Sub SetOutAndErrorNull()
        Console.SetOut(TextWriter.Null)
        Console.SetError(TextWriter.Null)
    End Sub
End Class
