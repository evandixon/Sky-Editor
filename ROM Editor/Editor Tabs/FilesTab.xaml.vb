Imports System.Windows.Controls
Imports SkyEditorBase
Public Class FilesTab
    Inherits SkyEditorBase.EditorTab
    Public Overrides Async Sub RefreshDisplay(Save As SkyEditorBase.GenericSave)
        If TypeOf Save Is GenericNDSRom Or TypeOf Save Is PMD_Explorers.SkyNDSRom Then
            Await (DirectCast(Save, GenericNDSRom).EnsureUnpacked)
            PluginHelper.StartLoading(PluginHelper.GetLanguageItem("Reading files..."))
            tvFiles.Items.Add(Await GetDirectoryNode(PluginHelper.GetResourceName(Save.Name & "\")))
            PluginHelper.StopLoading()
        End If
    End Sub
    Private Async Function GetDirectoryNode(Directory As String) As Task(Of TreeViewItem)
        Dim Node As New TreeViewItem()
        Node.Header = GetNodeContents(IO.Path.GetFileName(Directory), SkyEditorBase.PluginHelper.GetLanguageItem("DirPrefix", "[DIR] "))
        Node.Tag = Directory
        For Each d In Await Task.Run(Function()
                                         Return IO.Directory.GetDirectories(Directory)
                                     End Function)
            Node.Items.Add(Await GetDirectoryNode(d))
        Next
        For Each f In Await Task.Run(Function()
                                         Return IO.Directory.GetFiles(Directory)
                                     End Function)
            Node.Items.Add(GetFileNode(f))
        Next
        Return Node
    End Function
    Private Function GetFileNode(Filename As String) As TreeViewItem
        Dim Node As New TreeViewItem()
        Node.Header = GetNodeContents(IO.Path.GetFileName(Filename), SkyEditorBase.PluginHelper.GetLanguageItem("FilePrefix", "[FILE] "))
        Node.Tag = Filename
        Return Node
    End Function
    Private Function GetNodeContents(Name As String, Image As String) As StackPanel
        Dim p As New StackPanel
        Dim l As New Label()
        Dim img As New Label()
        l.Content = Name
        img.Content = Image

        p.Orientation = Orientation.Horizontal
        p.Children.Add(img)
        p.Children.Add(l)
        Return p
    End Function
    Public Overrides ReadOnly Property SupportedGames As String()
        Get
            Return {GameStrings.GenericNDSRom, GameStrings.SkyNDSRom}
        End Get
    End Property

    Public Overrides Function UpdateSave(Save As SkyEditorBase.GenericSave) As SkyEditorBase.GenericSave
        Return Save
    End Function

    Private Sub FilesTab_Loaded(sender As Object, e As Windows.RoutedEventArgs) Handles Me.Loaded
        Me.Header = SkyEditorBase.PluginHelper.GetLanguageItem("Files")
    End Sub

    Private Sub tvFiles_SelectedItemChanged(sender As Object, e As Windows.RoutedPropertyChangedEventArgs(Of Object)) Handles tvFiles.SelectedItemChanged
        Dim filename As String = DirectCast(e.NewValue, TreeViewItem).Tag
        If IO.File.Exists(filename) Then
            If spPlaceholder.Children.Count > 0 AndAlso TypeOf spPlaceholder.Children(0) Is FileFormatControl Then
                DirectCast(spPlaceholder.Children(0), FileFormatControl).UpdateFile()
            End If
            spPlaceholder.Children.Clear()
            Dim extension = IO.Path.GetExtension(filename).ToLower().Trim(".")
            Dim pluginInfo As New PluginDefinition
            If pluginInfo.ROMFileTypes.ContainsKey(extension) Then
                Dim control = pluginInfo.ROMFileTypes(extension)
                control.RefreshDisplay(filename)
                spPlaceholder.Children.Add(control)
            End If
        End If
    End Sub

    Private Sub menuOpenInExplorer_Click(sender As Object, e As Windows.RoutedEventArgs) Handles menuOpenInExplorer.Click
        Dim filename As String = DirectCast(tvFiles.SelectedItem, TreeViewItem).Tag
        If IO.Directory.Exists(filename) Then
            PluginHelper.RunProgramInBackground(filename, "/n/e/root," & filename)
        ElseIf IO.File.Exists(filename) Then
            PluginHelper.RunProgramInBackground(filename, "/n/e/select," & filename)
        End If
    End Sub
End Class