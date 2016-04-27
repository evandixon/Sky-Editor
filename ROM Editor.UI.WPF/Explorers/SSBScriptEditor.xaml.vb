Imports System.Windows.Controls
Imports ROMEditor.FileFormats.Explorers.Script
Imports ROMEditor.FileFormats.Explorers.Script.Commands
Imports SkyEditorWPF.UI
Imports WPF.JoshSmith.ServiceProviders.UI

Namespace Explorers
    Public Class SSBScriptEditor
        Inherits DataBoundObjectControl

#Region "Child Classes"
        ''' <summary>
        ''' Container class that's used to bypass a bug in ListViewDragDropManager.
        ''' </summary>
        Public Class ObjectContainer
            Public Property Item As Commands.RawCommand
        End Class
#End Region

        Private WithEvents dragManager As ListViewDragDropManager(Of ObjectContainer)
        Private Property Items As ObjectModel.ObservableCollection(Of ObjectContainer)

        Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
            Return 1
        End Function

        Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
            Return {GetType(SSB)}
        End Function

        Public Overrides Property EditingObject As Object
            Get
                If _editing IsNot Nothing Then
                    With DirectCast(_editing, SSB)
                        .Commands.Clear()
                        For Each item In Items
                            .Commands.Add(item.Item)
                        Next
                    End With
                End If

                Return _editing
            End Get
            Set(value As Object)
                _editing = value
                If value IsNot Nothing Then
                    With DirectCast(value, SSB)
                        Items.Clear()
                        For Each item In .Commands
                            Items.Add(New ObjectContainer With {.Item = item})
                        Next
                    End With
                End If
            End Set
        End Property
        Dim _editing As Object

        Public Sub New()
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            Items = New ObjectModel.ObservableCollection(Of ObjectContainer)
            lvScript.ItemsSource = Items
            dragManager = New ListViewDragDropManager(Of ObjectContainer)(lvScript)
        End Sub

        Private Sub lvScript_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles lvScript.SelectionChanged

        End Sub
    End Class
End Namespace

