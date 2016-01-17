Public Class CodeGenerator
    Dim s As GenericSave
    Private Sub CodeGenerator_Loaded(sender As Object, e As EventArgs) Handles Me.ContentRendered
        'Load buttons
        listActivators.Items.Add(ARDS.DSButton.L)
        listActivators.Items.Add(ARDS.DSButton.R)
        listActivators.Items.Add(ARDS.DSButton.A)
        listActivators.Items.Add(ARDS.DSButton.B)
        listActivators.Items.Add(ARDS.DSButton.X)
        listActivators.Items.Add(ARDS.DSButton.Y)
        listActivators.Items.Add(ARDS.DSButton.Up)
        listActivators.Items.Add(ARDS.DSButton.Down)
        listActivators.Items.Add(ARDS.DSButton.Left)
        listActivators.Items.Add(ARDS.DSButton.Right)
        listActivators.Items.Add(ARDS.DSButton.Start)
        listActivators.Items.Add(ARDS.DSButton.Select)
        'listActivators.Items.Add(ARDS.DSButton.NDS_Not_Folded)
        listActivators.SelectedItems.Add(listActivators.Items(10))
        listActivators.SelectedItems.Add(listActivators.Items(11))
        If ARDS.ManagerV1.CodeDefinitions.Count > 0 Then
            'Load Regions
            For Each r In ARDS.ManagerV1.Regions
                cbRegion.Items.Add(r)
            Next
            If cbRegion.Items.Count > 0 Then cbRegion.SelectedIndex = 0
        Else
            MessageBox.Show(Lists.LanguageText("Error_NoCheats"))
            Me.Close()
        End If
    End Sub

    Private Sub cbRegion_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbRegion.SelectionChanged
        cbCategory.Items.Clear()
        For Each c In ARDS.ManagerV1.GetCategoriesForRegion(cbRegion.SelectedItem)
            cbCategory.Items.Add(c)
        Next
        If cbCategory.Items.Count > 0 Then cbCategory.SelectedIndex = 0
    End Sub

    Private Sub cbCategory_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbCategory.SelectionChanged
        cbProperty.Items.Clear()
        For Each p In ARDS.ManagerV1.GetDefinitionsForCategory(cbCategory.SelectedItem, cbRegion.SelectedItem)
            cbProperty.Items.Add(p)
        Next
        If cbProperty.Items.Count > 0 Then cbProperty.SelectedIndex = 0
    End Sub

    Private Sub btnGenerate_Click(sender As Object, e As RoutedEventArgs) Handles btnGenerate.Click
        If cbProperty.SelectedIndex > -1 Then
            Dim activator As UInt16 = 0
            For Each a In listActivators.SelectedItems
                activator = activator Or (DirectCast(a, ARDS.DSButton))
            Next
            txtOut.Text = DirectCast(cbProperty.SelectedItem, ARDS.CodeDefinition).GenerateCode(s, cbRegion.SelectedItem, activator).ToString
        End If
    End Sub

    Private Sub cbProperty_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cbProperty.SelectionChanged
        If cbProperty.SelectedIndex > -1 Then
            lbAuthorDynamic.Content = cbProperty.SelectedItem.Author
        End If
    End Sub
    Public Overloads Sub Show(ByRef SaveFile As GenericSave)
        MyBase.Show()
        s = SaveFile
    End Sub
End Class
