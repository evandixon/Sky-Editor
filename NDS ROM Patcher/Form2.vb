Imports System.Reflection

Public Class Form2
    Private Property is3dsMode As Boolean
    Private Property core As PatcherCore
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles Me.Load
        If IO.File.Exists("Tools/ctrtool.exe") Then
            is3dsMode = True
            Me.Text = String.Format("{0} Patcher v{1}", "3DS", Assembly.GetExecutingAssembly.GetName.Version.ToString)
            'OpenFileDialog1.Filter = "3DS Files (*.3ds)|*.3ds|All Files (*.*)|*.*"
        Else
            is3dsMode = False
            Me.Text = String.Format("{0} Patcher v{1}", "NDS", Assembly.GetExecutingAssembly.GetName.Version.ToString)
            'OpenFileDialog1.Filter = "NDS Files (*.nds)|*.nds|All Files (*.*)|*.*"
        End If


    End Sub
End Class