Imports System.Windows.Controls
Imports ROMEditor.PMD_Explorers
Imports SkyEditorBase

Public Class PortraitTab
    Inherits SkyEditorBase.EditorTab

    Public Class Portrait
        Public Property Name As String
        Public Property Filename As String
        Public ReadOnly Property ImageUri As Uri
            Get
                Dim newPath As String = IO.Path.Combine(Environment.CurrentDirectory, "Resources/Plugins/ROMEditor/Temp/", Guid.NewGuid.ToString & ".png")
                IO.File.Copy(Filename, newPath, True)
                Return New Uri(newPath)
            End Get
        End Property
        Public Sub New(Name As String, Filename As String)
            Me.Name = Name
            Me.Filename = Filename
        End Sub
    End Class

    Dim kao As FileFormats.Kaomado

    Public Overrides Async Sub RefreshDisplay(Save As SkyEditorBase.GenericSave)
        If TypeOf Save Is SkyNDSRom Then
            kao = Await DirectCast(Save, SkyNDSRom).GetPortraitsFile
            tvFiles.Items.Clear()
            For Each item In IO.Directory.GetDirectories(kao.UnpackDirectory, "*", IO.SearchOption.TopDirectoryOnly)
                Dim Node As New TreeViewItem()
                Node.Header = IO.Path.GetFileNameWithoutExtension(item)
                Node.Tag = item
                tvFiles.Items.Add(Node)
            Next
        End If
    End Sub

    Public Overrides ReadOnly Property SupportedGames As String()
        Get
            Return {Constants.SkyNDSRom}
        End Get
    End Property

    Public Overrides Function UpdateSave(Save As SkyEditorBase.GenericSave) As GenericSave
        SkyEditorBase.Utilities.AsyncHelpers.RunSync(AddressOf kao.Save)
        Return Save
    End Function

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
        Me.Header = "Pokemon Portraits"
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
End Class
