Imports System.Windows.Controls
Imports ROMEditor.Roms
Imports SkyEditorBase
Public Class FilesTab
    Inherits ObjectTab
    Public Overrides Async Sub RefreshDisplay()
        Dim Save As SkyEditorBase.GenericSave = DirectCast(Me.EditingObject, GenericNDSRom)
        If TypeOf Save Is GenericNDSRom Or TypeOf Save Is SkyNDSRom Then
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
    Public Overrides ReadOnly Property SupportedTypes As Type()
        Get
            Return {GetType(GenericNDSRom), GetType(SkyNDSRom)}
        End Get
    End Property

    Private Sub FilesTab_Loaded(sender As Object, e As Windows.RoutedEventArgs) Handles Me.Loaded
        Me.Header = SkyEditorBase.PluginHelper.GetLanguageItem("Files")
    End Sub

    Private Sub tvFiles_SelectedItemChanged(sender As Object, e As Windows.RoutedPropertyChangedEventArgs(Of Object)) Handles tvFiles.SelectedItemChanged
        Dim filename As String = DirectCast(e.NewValue, TreeViewItem).Tag
        If IO.File.Exists(filename) Then
            If e.OldValue IsNot Nothing Then
                Dim oldFile As String = DirectCast(e.OldValue, TreeViewItem).Tag
                If IO.File.Exists(oldFile) Then
                    DirectCast(spPlaceholder.ObjectToEdit, GenericFile).Save(oldFile)
                End If
            End If
            spPlaceholder.ObjectToEdit = PluginHelper.PluginManagerInstance.OpenFile(New GenericFile(filename))
            'Dim extension = IO.Path.GetExtension(filename).ToLower().Trim(".")
            'Dim pluginInfo As New PluginDefinition
            'If pluginInfo.ROMFileTypes.ContainsKey(extension) Then
            '    Dim control = pluginInfo.ROMFileTypes(extension)
            '    control.RefreshDisplay(filename)
            '    spPlaceholder.Children.Add(control)
            'End If
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