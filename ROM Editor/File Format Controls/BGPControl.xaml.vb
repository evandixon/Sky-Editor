Imports System.Windows.Media.Imaging
Imports SkyEditorBase
Public Class BGPControl
    Inherits FileFormatControl
    Dim _bgp As FileFormats.BGP
    Dim _filename As String
    Public Overrides Async Sub RefreshDisplay(Filename As String)
        _bgp = Await FileFormats.BGP.FromFilename(Filename)
        _filename = Filename
        img.Source = New BitmapImage(New Uri(Await _bgp.GetTempImageURI(IO.Path.GetFileNameWithoutExtension(Filename))))
    End Sub

    Public Overrides Sub UpdateFile()

    End Sub

    Private Async Sub btnSave_Click(sender As Object, e As Windows.RoutedEventArgs) Handles btnSave.Click
        Dim s As New Windows.Forms.SaveFileDialog()
        s.Filter = "PNG Files (*.png)|*.png"
        If s.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim i = Await _bgp.GetImage
            i.Save(s.FileName)
        End If
    End Sub

    Private Async Sub btnImport_Click(sender As Object, e As Windows.RoutedEventArgs) Handles btnImport.Click
        Dim s As New Windows.Forms.OpenFileDialog()
        s.Filter = "PNG Files (*.png)|*.png|All Files (*.*)|*.*"
        If s.ShowDialog = Windows.Forms.DialogResult.OK Then
            Try
                _bgp = FileFormats.BGP.ConvertFromBitmap(New Drawing.Bitmap(s.FileName))
                _bgp.Save(_filename)
                img.Source = New BitmapImage(New Uri(Await _bgp.GetTempImageURI(IO.Path.GetFileNameWithoutExtension(_filename))))

            Catch ex As BadImageFormatException
                Windows.Forms.MessageBox.Show(PluginHelper.GetLanguageItem("BGPFormatError", "There was not enough room in the palette for all the colors in this image.\nEach 8x8 tile can only use one of 16 palettes, each with 16 colors.\nTry to avoid lots of different colors in the same area."))
            End Try
        End If
    End Sub

    Private Sub BGPControl_Loaded(sender As Object, e As Windows.RoutedEventArgs) Handles Me.Loaded
        PluginHelper.TranslateForm(Me)
    End Sub
End Class