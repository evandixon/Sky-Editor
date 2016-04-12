Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Timers
Imports System.Windows.Controls
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SkyEditorWPF.UI

Public Class SSBStringDictionaryEditor
    Inherits ObjectControl

    Public Class KVPWrapper
        Implements INotifyPropertyChanged

        Public Property Key As Integer
            Get
                Return _key
            End Get
            Set(value As Integer)
                _key = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Key)))
            End Set
        End Property
        Dim _key As Integer
        Public Property Value As String
            Get
                Return _value
            End Get
            Set(value As String)
                _value = value
                RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Me.Value)))
            End Set
        End Property
        Dim _value As String
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    End Class

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
    Private mainSource As ObservableCollection(Of KVPWrapper)

    Public Overrides Sub RefreshDisplay()
        mainSource = New ObservableCollection(Of KVPWrapper)
        For Each item In GetEditingObject(Of Dictionary(Of Integer, String))()
            Dim n = New KVPWrapper With {.Key = item.Key, .Value = item.Value}
            AddHandler n.PropertyChanged, AddressOf Me.OnObjModified
            mainSource.Add(n)
        Next
        lstEntries.ItemsSource = mainSource
        IsModified = False
    End Sub

    Public Overrides Sub UpdateObject()
        GetEditingObject(Of Dictionary(Of Integer, String)).Clear()
        For Each item In mainSource
            GetEditingObject(Of Dictionary(Of Integer, String)).Add(item.Key, item.Value)
        Next
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
                                  lstEntries.ItemsSource = mainSource
                              End Sub)
        Else
            Dim results As New ObservableCollection(Of KVPWrapper)
            Dispatcher.Invoke(Sub()
                                  lstEntries.ItemsSource = results
                              End Sub)

            Dim searchTerms = SearchText.Split(" ")

            For Each item In mainSource
                If cancelSearch = True Then
                    'If we get here, the search textbox has been changed, so we'll stop searching
                    Exit For
                End If

                Dim isMatch As Boolean
                For Each term In searchTerms
                    isMatch = False 'For every term, we'll set isMatch to false

                    'The entry must match every term
                    If item.Key.ToString.Contains(term) Then
                        isMatch = True
                    ElseIf item.Value.ToLower.Contains(term.ToLower) Then
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

    Private Sub OnLoaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If Not DesignerProperties.GetIsInDesignMode(Me) Then
            Me.Header = EditingLanguage
        End If
    End Sub

    Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
        Return {GetType(Dictionary(Of Integer, String))}
    End Function

    Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
        Return 0
    End Function

    Public Sub AddItem(ID As Integer)
        Dim newItem As New KVPWrapper With {.Key = ID, .Value = ""}
        AddHandler newItem.PropertyChanged, AddressOf Me.OnObjModified
        mainSource.Add(newItem)
        lstEntries.ScrollIntoView(newItem)
    End Sub

    Public Property EditingLanguage As String

End Class
