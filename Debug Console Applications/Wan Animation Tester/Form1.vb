Imports ROMEditor.FileFormats.Animations
Public Class Form1
    Dim index As Integer = 0
    Dim frames As Bitmap()
    WithEvents t As Timer
    Dim a As AnimatedSprite
    Dim o As New FolderBrowserDialog()
    Sub Reload()
        If o.ShowDialog = Windows.Forms.DialogResult.OK Then
            a = New AnimatedSprite(o.SelectedPath)
            t = New Timer
            t.Interval = 100
            NumericUpDown1.Maximum = a.AnimData.AnimSequenceData.Count - 1
            NumericUpDown1.Minimum = 0
            NumericUpDown1.Value = 0
            frames = a.GetAnimationFrames(NumericUpDown1.Value)
            t.Start()
        End If
    End Sub
    Private Sub t_Tick(sender As Object, e As EventArgs) Handles t.Tick
        t.Stop()
        If index >= frames.Length Then
            index = 0
        End If
        PictureBox1.Image = frames(index)
        index += 1
        t.Start()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Reload()
    End Sub

    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        frames = a.GetAnimationFrames(NumericUpDown1.Value)
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            PictureBox1.SizeMode = PictureBoxSizeMode.Zoom
        Else
            PictureBox1.SizeMode = PictureBoxSizeMode.Normal
        End If
    End Sub
End Class