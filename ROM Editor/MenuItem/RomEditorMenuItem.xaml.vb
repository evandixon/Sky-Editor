Imports System.Windows
Imports SkyEditorBase
Imports System.Windows.Input

Public Class RomEditorMenuItem
    Async Sub OnKeyPress(sender As Object, e As KeyEventArgs)
        If e.Key = Key.F5 Then
            Await RunRom(True)
        End If
    End Sub

    Private Async Sub menuRunRom_Click(sender As Object, e As Windows.RoutedEventArgs) Handles menuRunRom.Click
        Await RunRom()
    End Sub

    Async Function RunRom(Optional SuppressErrorMessage As Boolean = False) As Task
        Dim w As Window = Window.GetWindow(Me)
        If TypeOf w Is SkyEditorBase.MainWindow Then
            Dim m As SkyEditorBase.MainWindow = DirectCast(w, SkyEditorBase.MainWindow)
            m.UpdateFromTabs()
            If TypeOf m.Save Is GenericNDSRom Then
                DeveloperConsole.Writeline("Beginning to run ROM...")
                Await DirectCast(m.Save, GenericNDSRom).RunRom()
                DeveloperConsole.Writeline("ROM Launched.")
            Else
                If Not SuppressErrorMessage Then MessageBox.Show("You do not currently have a NDS ROM loaded.", "Sky Editor ROM Editor")
            End If
        End If
    End Function

    Public Sub New(Window As iMainWindow)
        InitializeComponent()
        AddHandler Window.OnKeyUp, AddressOf OnKeyPress
    End Sub
End Class
