Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows
Imports SkyEditorBase

Public Class DesmumeMenuItem
    Inherits MenuItem
    Dim m As PluginManager
    Async Sub OnKeyPress(sender As Object, e As KeyEventArgs)
        If e.Key = Key.F5 Then
            Await RunRom(True)
        End If
    End Sub

    Private Async Sub menuRunRom_Click(sender As Object, e As System.Windows.RoutedEventArgs) Handles menuRunRom.Click
        Await RunRom()
    End Sub

    Async Function RunRom(Optional SuppressErrorMessage As Boolean = False) As Task
        Dim w As Window = Window.GetWindow(Me)
        If TypeOf w Is SkyEditorBase.SkyEditorWindows.MainWindow Then
            m.UpdateSave()
            If TypeOf m.Save Is ROMEditor.GenericNDSRom Then
                PluginHelper.Writeline("Beginning to run ROM...")

                Dim romDirectory As String = PluginHelper.GetResourceDirectory("ROMEditor_plg")
                Await DirectCast(m.Save, ROMEditor.GenericNDSRom).RePack(romDirectory & "\current.nds")
                PluginHelper.Writeline("Running ROM...")
                Loader.RunDeSmuMe(romDirectory & "\current.nds")

                PluginHelper.Writeline("ROM Launched.")
            Else
                Dim SupportedGames As New List(Of String)
                For Each item In m.GameTypes
                    If m.Save IsNot Nothing AndAlso m.Save.SaveID = item.Value AndAlso item.Key.ToLower.EndsWith(".nds") Then
                        SupportedGames.Add(item.Key)
                    End If
                Next
                If SupportedGames.Count >= 1 Then
                    Dim x As New RomSelector
                    If x.ShowDialog(SupportedGames) Then
                        If Not IO.Directory.Exists(IO.Path.Combine(IO.Path.GetDirectoryName(Loader.GetBestVersionExecutableName), "Battery/")) Then
                            IO.Directory.CreateDirectory(IO.Path.Combine(IO.Path.GetDirectoryName(Loader.GetBestVersionExecutableName), "Battery/"))
                        End If
                        Using dsv As New IO.FileStream(IO.Path.Combine(IO.Path.GetDirectoryName(Loader.GetBestVersionExecutableName), "Battery/" & IO.Path.GetFileNameWithoutExtension(x.RomName) & ".dsv"), IO.FileMode.Create)
                            Dim buffer = Await m.Save.GetBytes
                            dsv.Write(buffer, 0, buffer.Length)
                            Const footer1 As String = "|<--Snip above here to create a raw sav by excluding this DeSmuME savedata footer:"
                            Dim footer2 As Byte() = {1, 0, 1, 0, 0, 0, 4, 0, 5, 0, 0, 0, 3, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0}
                            Const footer3 As String = "|-DESMUME SAVE-|"
                            Dim e As New System.Text.ASCIIEncoding
                            dsv.Write(e.GetBytes(footer1), 0, footer1.Length)
                            dsv.Write(footer2, 0, footer2.Length)
                            dsv.Write(e.GetBytes(footer3), 0, footer3.Length)
                        End Using
                        Loader.RunDeSmuMe(x.RomName)
                    End If
                Else
                    If Not SuppressErrorMessage Then MessageBox.Show("Invalid save.", "Sky Editor")
                End If
            End If
        End If
    End Function

    Public Sub New(Manager As PluginManager)
        InitializeComponent()
        AddHandler Manager.Window.OnKeyPress, AddressOf OnKeyPress
        m = Manager
        Me.Header = PluginHelper.GetLanguageItem("DeSmuMe")
        menuRunRom.Header = PluginHelper.GetLanguageItem("RunRom", "Run Game")
    End Sub
End Class