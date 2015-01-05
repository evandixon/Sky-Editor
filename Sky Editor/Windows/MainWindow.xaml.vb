Imports SkyEditor.skyjed.save

Class MainWindow
    Private WithEvents OpenFileDialog1 As Windows.Forms.OpenFileDialog
    Private WithEvents SaveFileDialog1 As Windows.Forms.SaveFileDialog
    Property Sky As SkySave
        Get
            Return DirectCast(save, SkySave)
        End Get
        Set(value As SkySave)
            save = DirectCast(value, GenericSave)
        End Set
    End Property
    Property TD As TDSave
        Get
            Return DirectCast(save, TDSave)
        End Get
        Set(value As TDSave)
            save = DirectCast(value, GenericSave)
        End Set
    End Property
    Property RB As RBSave
        Get
            Return DirectCast(save, RBSave)
        End Get
        Set(value As RBSave)
            save = DirectCast(value, GenericSave)
        End Set
    End Property
    Friend save As GenericSave
    Private Sub menuFileOpen_Click(sender As Object, e As RoutedEventArgs) Handles menuFileOpen.Click
        If OpenFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            LoadSave(OpenFileDialog1.FileName)
        End If
    End Sub
    Sub LoadSave(Filename As String)
        Dim d() As Byte = IO.File.ReadAllBytes(Filename)
        save = New GenericSave(d)
        If save.IsTDSave Then
            save = New TDSave(d)
        ElseIf save.IsSkySave Then
            save = New SkySave(d)
        ElseIf save.IsRBSave Then
            save = New RBSave(d)
        End If
        RefreshDisplay()
    End Sub
    Private Sub menuFileSaveAs_Click(sender As Object, e As RoutedEventArgs) Handles menuFileSaveAs.Click
        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            'TestSub()
            UpdateSave()
            IO.File.WriteAllBytes(SaveFileDialog1.FileName, save.GetBytes)
        End If
    End Sub
#Region "RefreshDisplay and Update Save"

    Private Sub RefreshDisplay()
        'While True
        '    Dim x = RB.StoredPokemon '.pkmns(10)
        'End While

        RefreshTabs()
    End Sub
    Private Sub UpdateSave()
        UpdateFromTabs()
    End Sub
#End Region

    Sub RefreshTabs()
        'For Each item In tcTabs.Items
        '    If TypeOf item Is EditorTab Then
        '        tcTabs.Items.Remove(item)
        '    End If
        'Next
        tcTabs.Items.Clear()
        For Each item In EditorTabManager.GetEditorTabs
            If (CByte(item.SupportedGames).Bit1 = save.IsRBSave AndAlso save.IsRBSave) OrElse (CByte(item.SupportedGames).Bit2 = save.IsTDSave AndAlso save.IsTDSave) OrElse (CByte(item.SupportedGames).Bit3 = save.IsSkySave AndAlso save.IsSkySave) Then
                Dim t As TabItem = item
                't.Header = item.Header
                't.Content = item
                item.RefreshDisplay(save)
                tcTabs.Items.Add(t)
            End If
        Next
    End Sub
    Sub UpdateFromTabs()
        For Each item In tcTabs.Items
            'Try
            save = DirectCast(item, EditorTab).UpdateSave(save) 'DirectCast(item.content, EditorTab).UpdateSave(save)
            'Catch ex As Exception
            'do nothing
            'End Try
        Next
    End Sub
    
  
    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        'Check command line arguments to see if a file needs to be loaded
        Dim fileToLoad As String = ""
        Dim args() As String = Environment.GetCommandLineArgs
        If args.Length > 1 Then
            'Code commented out should work if published with ClickOnce.  Should, so test before checking this in.
            'If args(1).ToLower.EndsWith(".application") Then
            '    'This is running in ClickOnce, which has a special way of loading files
            '    'TODO: use clickonce classes to get ileToLoad
            'Else
            fileToLoad = args(1)
            'Reset Current Directory. Without doing this, resources might not be read properly
            Environment.CurrentDirectory = args(0).Replace(IO.Path.GetFileName(args(0)), "")
            'End If
        End If

        'Initialize Dialogs
        OpenFileDialog1 = New Forms.OpenFileDialog
        OpenFileDialog1.Filter = "Supported Files (*.sav;*.dsv)|*.sav;*.dsv|Save Files (*.sav)|*.sav|DeSmuMe Save Files (*.dsv)|*.dsv|All Files (*.*)|*.*"
        SaveFileDialog1 = New Forms.SaveFileDialog
        SaveFileDialog1.Filter = "Supported Files (*.sav;*.dsv)|*.sav;*.dsv|Save Files (*.sav)|*.sav|DeSmuMe Save Files (*.dsv)|*.dsv|All Files (*.*)|*.*"
        'Load Language
        Try
            SetValue(MainWindow.TitleProperty, String.Format(Lists.LanguageText("Title"), My.Application.Info.Version.ToString(2)))
            menuFileOpen.Header = Lists.LanguageText("Open")
            menuFileSaveAs.Header = Lists.LanguageText("SaveAs")
            ' btnHeldItemsAdd.Content = Lists.LanguageText("Add")
            'btnSpEpisodeHeldItemsAdd.Content = Lists.LanguageText("Add")
            'DeleteToolStripMenuItem.Text = Lists.LanguageText("Delete")

            'lblGeneral_TeamName.Content = Lists.LanguageText("General_TeamName")
            'lblGeneral_HeldMoney.Content = Lists.LanguageText("General_HeldMoney")
            'lblGeneral_SpEpisodeHeldMoney.Content = Lists.LanguageText("General_SpEpisodeHeldMoney")
            'lblGeneral_StoredMoney.Content = Lists.LanguageText("General_StoredMoney")
            ''lblGeneral_PlayerName.Content = Lists.LanguageText("General_PlayerName")
            'lblGeneral_Adventures.Content = Lists.LanguageText("Adventures")

            'gbStoredItems.Header = String.Format(Lists.LanguageText("Category_StoredItems"), "0")
            'gbHeldItems.Header = String.Format(Lists.LanguageText("Category_HeldItems"), "0")
            'gbSpEpisodeHeldItems.Header = String.Format(Lists.LanguageText("Category_SpEpisodeHeldItems"), "0")
            'gbActivePokemon.Header = String.Format(Lists.LanguageText("Category_ActivePokemon"), "0")

            menuCredits.Header = Lists.LanguageText("CreditsLabel")
            'gbHeldBoxContent.Header = Lists.LanguageText("BoxContents")
        Catch ex As Exception
            Try
                If Settings.DebugMode Then
                    MessageBox.Show(ex.ToString)
                Else
                    MessageBox.Show("Loading language failed.  Remaining text will be loaded as English.")
                End If
            Catch ex2 As Exception
                MessageBox.Show("Loading language and settings failed.  Remaining text will be loaded as English.")
            End Try
        End Try

        'If Settings.Enable255ItemCount Then
        '    numHeldItemsAddCount.Maximum = 255
        '    numSpEpisodeHeldItemsAddCount.Maximum = 255
        'End If
        'Add or remove cheats menu if there's plugins
        If ARDS.ManagerV1.CodeDefinitions.Count = 0 Then
            menuCheats.Visibility = Windows.Visibility.Collapsed
        End If
        'LoadSkyHeldItemsDropDowns()
        
        'Ensure save is big enough for code generation
        Dim b(128 * 1024) As Byte
        save = New GenericSave(b)
        'Load fileToLoad
        If fileToLoad.Length > 0 Then
            LoadSave(fileToLoad)
        End If

    End Sub
    

#Region "Event Handlers"
    Private Sub menuCredits_Click(sender As Object, e As RoutedEventArgs) Handles menuCredits.Click
        MessageBox.Show(Lists.LanguageText("Credits"))
    End Sub

    Private Sub menuCheats_Click(sender As Object, e As RoutedEventArgs) Handles menuCheats.Click
        Dim x As New CodeGenerator
        x.Show(Sky)
    End Sub
#End Region
    
    

End Class
