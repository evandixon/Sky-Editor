Imports SkyEditorBase

Public Class RomSelector
    Public Class ROM
        Public Property Name As String
        Public ReadOnly Property Filename As String
            Get
                Dim romDirectory As String = PluginHelper.GetResourceName("Roms/NDS/")
                Return IO.Path.Combine(romDirectory, Name.Replace(":", ""))
            End Get
        End Property
        Public ReadOnly Property ImageUri As Uri
            Get
                If IO.File.Exists(Filename) Then
                    Dim newpath = IO.Path.Combine(PluginHelper.GetResourceName("Temp"), IO.Path.GetFileNameWithoutExtension(Name.Replace(":", "")) & ".bmp")
                    If Not IO.File.Exists(newpath) Then
                        If Not IO.Directory.Exists(IO.Path.GetDirectoryName(newpath)) Then
                            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(newpath))
                        End If
                        DSIconTool.ExtractIcon(Filename, newpath)
                    End If
                    Return New Uri(newpath)
                Else
                    Return New Uri("")
                End If
            End Get
        End Property
        Public Sub New(Name As String)
            Me.Name = Name
        End Sub
    End Class
    Public Overloads Function ShowDialog(Roms As List(Of String)) As Boolean
        For Each item In Roms
            lvRoms.Items.Add(New ROM(item))
        Next
        Return Me.ShowDialog
    End Function
    Public Property RomName As String
    Sub OnOK()
        Dim romDirectory As String = PluginHelper.GetResourceName("Roms/NDS/")
        If Not IO.Directory.Exists(romDirectory) Then
            IO.Directory.CreateDirectory(romDirectory)
        End If
        Dim file As String = IO.Path.Combine(romDirectory, DirectCast(lvRoms.SelectedItem, ROM).Name.Replace(":", ""))
        If IO.File.Exists(file) Then
            RomName = file
            DialogResult = True
            Me.Close()
        Else
            Dim x As New Windows.Forms.OpenFileDialog
            x.Filter = "NDS Roms (*.nds)|*.nds|All Files (*.*)|*.*"
            If x.ShowDialog = Windows.Forms.DialogResult.OK Then
                IO.File.Copy(x.FileName, file)
                RomName = file
                DialogResult = True
                Me.Close()
            End If
        End If
    End Sub

    Private Sub btnOK_Click(sender As Object, e As Windows.RoutedEventArgs) Handles btnOK.Click
        OnOK()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As Windows.RoutedEventArgs) Handles btnCancel.Click
        DialogResult = False
        Me.Close()
    End Sub

    Private Sub RomSelector_Loaded(sender As Object, e As Windows.RoutedEventArgs) Handles Me.Loaded
        SkyEditorBase.PluginHelper.TranslateForm(Me)
        If lvRoms.Items.Count = 1 Then
            lvRoms.SelectedIndex = 0
            OnOK()
        End If
    End Sub

    Private Sub lvRoms_MouseDoubleClick(sender As Object, e As Windows.Input.MouseButtonEventArgs) Handles lvRoms.MouseDoubleClick
        If lvRoms.SelectedIndex > -1 Then
            OnOK()
        End If
    End Sub
End Class
