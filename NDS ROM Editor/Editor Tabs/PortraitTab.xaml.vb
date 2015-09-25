Imports System.Windows.Controls
Imports ROMEditor.Roms
Imports SkyEditorBase

Public Class PortraitTab
    Inherits ObjectTab
    Dim kao As FileFormats.Kaomado

    Public Overrides Async Sub RefreshDisplay()
        Dim Save As GenericSave = DirectCast(Me.EditingObject, GenericSave)
        If TypeOf Save Is SkyNDSRom Then
            kao = Await DirectCast(Save, SkyNDSRom).GetPortraitsFile
            PluginHelper.StartLoading(PluginHelper.GetLanguageItem("Filling Pokemon Portraits..."))
            tvFiles.Items.Clear()
            For Each item In IO.Directory.GetDirectories(kao.UnpackDirectory, "*", IO.SearchOption.TopDirectoryOnly)
                Dim Node As New TreeViewItem()
                Node.Header = IO.Path.GetFileNameWithoutExtension(item)
                Node.Tag = item
                tvFiles.Items.Add(Node)
            Next
            PluginHelper.StopLoading()
        End If
    End Sub

    Public Overrides ReadOnly Property SupportedTypes As Type()
        Get
            Return {GetType(SkyNDSRom)}
        End Get
    End Property

    Public Overrides Sub UpdateObject()
        If kao IsNot Nothing Then
            SkyEditorBase.Utilities.AsyncHelpers.RunSync(AddressOf kao.Save)
        End If
    End Sub

    Private Sub tvFiles_SelectedItemChanged(sender As Object, e As System.Windows.RoutedPropertyChangedEventArgs(Of Object)) Handles tvFiles.SelectedItemChanged
        RefreshPortraits()
    End Sub
    Private Sub RefreshPortraits()
        Dim filename As String = DirectCast(tvFiles.SelectedItem, TreeViewItem).Tag
        lvPortraits.Items.Clear()
        For Each item In IO.Directory.GetFiles(filename)
            lvPortraits.Items.Add(New Portrait(IO.Path.GetFileNameWithoutExtension(item), item))
        Next
    End Sub
    Private Sub PortraitTab_Loaded(sender As Object, e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Me.Header = PluginHelper.GetLanguageItem("PokemonPortraits", "Pokemon Portraits")
    End Sub

    Protected Sub btnExport_Click(sender As Object, e As System.Windows.RoutedEventArgs)
        Dim o As New System.Windows.Forms.SaveFileDialog()
        o.Filter = "PNG Files (*.png)|*.png|All Files (*.*)|*.*"
        If o.ShowDialog = System.Windows.Forms.DialogResult.OK Then
            IO.File.Copy(DirectCast(sender, Button).Tag, o.FileName, True)
        End If
    End Sub
    Protected Sub btnImport_Click(sender As Object, e As System.Windows.RoutedEventArgs)
        Dim s As New System.Windows.Forms.OpenFileDialog
        s.Filter = "PNG Files (*.png)|*.png|All Files (*.*)|*.*"
        If s.ShowDialog = System.Windows.Forms.DialogResult.OK Then
            IO.File.Copy(s.FileName, DirectCast(sender, Button).Tag, True)
            'Dim input = FreeImageAPI.FreeImage.LoadEx(DirectCast(sender, Button).Tag)
            'input = FreeImageAPI.FreeImage.ConvertColorDepth(input, FreeImageAPI.FREE_IMAGE_COLOR_DEPTH.FICD_04_BPP)
            'FreeImageAPI.FreeImage.UnloadEx(input)
            RefreshPortraits()
        End If
    End Sub

    Private Async Sub btnPortraitFix_Click(sender As Object, e As Windows.RoutedEventArgs) Handles btnPortraitFix.Click
        PluginHelper.StartLoading(PluginHelper.GetLanguageItem("ApplyingKaomadoFix", "Applying the missing portrait fix."))
        Await kao.ApplyMissingPortraitFix()
        PluginHelper.StopLoading()
    End Sub
End Class