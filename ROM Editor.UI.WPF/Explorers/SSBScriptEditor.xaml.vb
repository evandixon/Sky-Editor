Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Windows.Controls
Imports System.Windows.Input
Imports SkyEditor.ROMEditor.Windows.FileFormats.Explorers.Script
Imports SkyEditor.ROMEditor.Windows.FileFormats.Explorers.Script.Commands
Imports SkyEditor.UI.WPF
Imports WPF.JoshSmith.ServiceProviders.UI

Namespace Explorers
    Public Class SSBScriptEditor
        Inherits DataBoundObjectControl

#Region "Child Classes"
        ''' <summary>
        ''' Container class that's used to bypass a bug in ListViewDragDropManager.
        ''' </summary>
        Public Class ObjectContainer
            Implements INotifyPropertyChanged

            Public Property Item As LogicalCommand
                Get
                    Return _item
                End Get
                Set(value As LogicalCommand)
                    _item = value

                    'Normally we want to avoid raising this property if nothing has changed,
                    'but in this case this serves to refresh the AsString display.
                    'When the ObjectWindow in lvScript_MouseDoubleClick below exits, it will set Item, which may be the same reference,
                    'and we need the ListView display to update
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(Item)))
                    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(NameOf(AsString)))
                End Set
            End Property
            Dim _item As LogicalCommand

            Public ReadOnly Property AsString As String
                Get
                    Return Item.ToString
                End Get
            End Property

            Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        End Class
#End Region

        Private enableRaiseModified As Boolean
        Private WithEvents dragManager As ListViewDragDropManager(Of ObjectContainer)
        Protected WithEvents Items As ObjectModel.ObservableCollection(Of ObjectContainer)

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 1
        End Function

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(SSB)}
        End Function

        Public Overrides Property ObjectToEdit As Object
            Get
                Return _editing
            End Get
            Set(value As Object)
                enableRaiseModified = False
                _editing = value
                If value IsNot Nothing Then
                    With DirectCast(value, SSB)
                        Items.Clear()
                        For Each item In .Commands
                            Items.Add(New ObjectContainer With {.Item = item})
                        Next
                    End With
                End If
                scriptToolbar.ScriptFile = value
                enableRaiseModified = True
            End Set
        End Property
        Private WithEvents _editing As SSB

        Public Sub New()
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Items = New ObjectModel.ObservableCollection(Of ObjectContainer)
            lvScript.DataContext = Items
            dragManager = New ListViewDragDropManager(Of ObjectContainer)(lvScript)
            scriptToolbar.Target = lvScript
            enableRaiseModified = False
        End Sub

        Private Sub lvScript_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lvScript.SelectionChanged

        End Sub

        Private Sub Items_CollectionChanged(sender As Object, e As NotifyCollectionChangedEventArgs) Handles Items.CollectionChanged
            If enableRaiseModified AndAlso TypeOf _editing Is SSB Then
                DirectCast(_editing, SSB).RaiseModified()
            End If
        End Sub

        Private Sub lvScript_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs) Handles lvScript.MouseDoubleClick
            If lvScript.SelectedItem IsNot Nothing Then
                Dim item As ObjectContainer = lvScript.SelectedItem
                Dim command = item.Item

                Dim window As New ObjectWindow(CurrentPluginManager)
                window.ObjectToEdit = command
                'Todo: use "If window.ShowDialog() Then" when ObjectWindow gives a better result
                window.ShowDialog()
                item.Item = window.ObjectToEdit
            End If
        End Sub

        Private Sub _editing_FileSaving(sender As Object, e As EventArgs) Handles _editing.FileSaving
            With DirectCast(_editing, SSB)
                .Commands.Clear()
                For Each item In Items
                    .Commands.Add(item.Item)
                Next
            End With
        End Sub
    End Class
End Namespace

