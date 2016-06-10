Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Timers
Imports System.Windows.Controls
Imports System.Windows.Forms
Imports ROMEditor.FileFormats
Imports ROMEditor.FileFormats.PSMD
Imports SkyEditorWPF.UI

Public Class MessageBinEditor
    Inherits ObjectControl

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        searchTimer = New Timers.Timer(500)
        cancelSearch = False
    End Sub

    Private WithEvents searchTimer As Timers.Timer
    Private cancelSearch As Boolean
    Private searchTask As Task

    Public Overrides Sub RefreshDisplay()
        With GetEditingObject(Of MessageBin)()
            AddHandler .EntryAdded, AddressOf OnMsgItemAdded
            AddHandler .Modified, AddressOf OnObjModified
            lstEntries.ItemsSource = .Strings
            If lstEntries.Items.Count > 0 Then
                lstEntries.SelectedIndex = 0
            End If
        End With
        IsModified = False
    End Sub

    Public Sub Sort(Keys As List(Of Integer))
        Task.Run(New Action(Sub()
                                DoSort(Keys)
                            End Sub))
    End Sub

    Private Sub DoSort(Keys As List(Of Integer))
        Dim results As New ObservableCollection(Of MessageBinStringEntry)
        Dispatcher.Invoke(Sub()
                              lstEntries.ItemsSource = results
                          End Sub)
        For Each item In Keys
            Dim entry = (From s In GetEditingObject(Of MessageBin)().Strings Where s.HashSigned = item).FirstOrDefault
            If entry IsNot Nothing Then
                Dispatcher.Invoke(Sub()
                                      results.Add(entry)
                                  End Sub)
            End If
        Next
    End Sub

    Private Sub OnMsgItemAdded(sender As Object, e As MessageBin.EntryAddedEventArgs)
        Dim addedEntry = (From i As MessageBinStringEntry In lstEntries.ItemsSource Where i.Hash = e.NewID).FirstOrDefault
        If addedEntry IsNot Nothing Then
            lstEntries.SelectedIndex = lstEntries.Items.IndexOf(addedEntry)
            lstEntries.ScrollIntoView(addedEntry)
        End If
    End Sub

    Private Sub txtSearch_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtSearch.TextChanged
        cancelSearch = True
        searchTimer.Stop()
        searchTimer.Start()
    End Sub


    Private Async Sub searchTimer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles searchTimer.Elapsed
        searchTimer.Stop()
        Dim searchText As String = ""
        Dispatcher.Invoke(Sub()
                              searchText = txtSearch.Text
                          End Sub)
        'Wait for the current task to stop itself
        If searchTask IsNot Nothing Then
            Await searchTask
        End If
        'Start a new async task
        searchTask = Task.Run(New Action(Sub()
                                             RunSearch(searchText)
                                         End Sub))
    End Sub

    Private Sub RunSearch(SearchText As String)
        cancelSearch = False
        If String.IsNullOrEmpty(SearchText) Then
            Dispatcher.Invoke(Sub()
                                  lstEntries.ItemsSource = GetEditingObject(Of MessageBin)().Strings
                              End Sub)
        Else
            Dim results As New ObservableCollection(Of MessageBinStringEntry)
            Dispatcher.Invoke(Sub()
                                  lstEntries.ItemsSource = results
                              End Sub)

            Dim searchTerms = SearchText.Split(" ")

            For Each item In GetEditingObject(Of MessageBin)().Strings
                If cancelSearch = True Then
                    'If we get here, the search textbox has been changed, so we'll stop searching
                    Exit For
                End If

                Dim isMatch As Boolean
                For Each term In searchTerms
                    isMatch = False 'For every term, we'll set isMatch to false

                    'The entry must match every term
                    If item.Hash.ToString.Contains(term) Then
                        isMatch = True
                    ElseIf item.HashSigned.ToString.Contains(term) Then
                        isMatch = True
                    ElseIf item.Entry.ToString.ToLower.Contains(term.ToLower) Then
                        isMatch = True
                    End If

                    'If any terms aren't a match, then we don't use this entry
                    If Not isMatch Then
                        Exit For
                    End If
                Next

                If isMatch Then
                    Dispatcher.Invoke(Sub()
                                          results.Add(item)
                                      End Sub)
                End If
            Next
        End If
    End Sub

    Private Sub OnObjModified(sender As Object, e As EventArgs)
        IsModified = True
    End Sub

    Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
        Return {GetType(MessageBin)}
    End Function

    Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
        Return 1
    End Function

    Private Sub NDSModSrcEditor_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If Not DesignerProperties.GetIsInDesignMode(Me) Then
            Me.Header = My.Resources.Language.Message
            columnID.Header = My.Resources.Language.ID
            columnEntry.Header = My.Resources.Language.Entry
            lblSearch.Content = My.Resources.Language.Search
        End If
    End Sub

    Private Sub btnExport_Click(sender As Object, e As RoutedEventArgs) Handles btnExport.Click
        Dim f As New SaveFileDialog
        f.Filter = $"{My.Resources.Language.HTMLFiles} (*.htm)|*.htm|{My.Resources.Language.AllFiles} (*.*)|*.*"
        'If f.ShowDialog = DialogResult.OK Then

        'End If
    End Sub
End Class
