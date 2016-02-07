Imports SkyEditorBase
Imports SaveEditor.Saves
Imports SkyEditorBase.Interfaces

Namespace Tabs
    Public Class SkyHistory
        Inherits UserControl
        Implements SkyEditorBase.Interfaces.iObjectControl

        Public Sub New()

            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.

        End Sub

        Private WriteOnly Property PokemonDictionary As IDictionary(Of Integer, String)
            Set(value As IDictionary(Of Integer, String))
                cbPlayer.Items.Clear()
                cbPartner.Items.Clear()

                For Each item In (From v In value Order By v.Value)
                    cbPlayer.Items.Add(New Utilities.GenericListItem(Of Integer)(item.Value, item.Key))
                    cbPartner.Items.Add(New Utilities.GenericListItem(Of Integer)(item.Value, item.Key))
                Next

                cbPlayer.SelectedIndex = 0
                cbPartner.SelectedIndex = 0
            End Set
        End Property

        Private Property SelectedPlayerID As Integer
            Get
                Return DirectCast(cbPlayer.SelectedItem, Utilities.GenericListItem(Of Integer)).Value
            End Get
            Set(value As Integer)
                For Each item In cbPlayer.Items
                    If DirectCast(item, Utilities.GenericListItem(Of Integer)).Value = value Then
                        cbPlayer.SelectedItem = item
                    End If
                Next
            End Set
        End Property

        Private Property SelectedPartnerID As Integer
            Get
                Return DirectCast(cbPartner.SelectedItem, Utilities.GenericListItem(Of Integer)).Value
            End Get
            Set(value As Integer)
                For Each item In cbPartner.Items
                    If DirectCast(item, Utilities.GenericListItem(Of Integer)).Value = value Then
                        cbPartner.SelectedItem = item
                    End If
                Next
            End Set
        End Property

        ''' <summary>
        ''' Updates UI elements to display certain properties.
        ''' </summary>
        Public Sub RefreshDisplay()
            PokemonDictionary = Lists.SkyPokemon
            With GetEditingObject(Of SkySave)()
                txtPlayerName.Text = .OriginalPlayerName
                txtPartnerName.Text = .OriginalPartnerName
                SelectedPlayerID = .OriginalPlayerID
                SelectedPartnerID = .OriginalPartnerID
                chbPlayerGender.IsChecked = .OriginalPlayerIsFemale
                chbPartnerGender.IsChecked = .OriginalPartnerIsFemale
                IsModified = False
            End With
        End Sub

        ''' <summary>
        ''' Updates the EditingObject using data in UI elements.
        ''' </summary>
        Public Sub UpdateObject()
            With GetEditingObject(Of SkySave)()
                .OriginalPlayerName = txtPlayerName.Text
                .OriginalPartnerName = txtPartnerName.Text
                .OriginalPlayerID = SelectedPlayerID
                .OriginalPartnerID = SelectedPartnerID
                .OriginalPlayerIsFemale = chbPlayerGender.IsChecked
                .OriginalPartnerIsFemale = chbPartnerGender.IsChecked
            End With
        End Sub

        Private Sub GeneralTab_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Me.Header = PluginHelper.GetLanguageItem("History")
            lblPlayerName.Content = PluginHelper.GetLanguageItem("Original Player Name:")
            lblPlayerKind.Content = PluginHelper.GetLanguageItem("Original Player Pokemon:")
            lblPartnerName.Content = PluginHelper.GetLanguageItem("Original Partner Name:")
            lblPartnerKind.Content = PluginHelper.GetLanguageItem("Original Partner Pokemon:")
            chbPlayerGender.Content = PluginHelper.GetLanguageItem("Is Female")
            chbPartnerGender.Content = PluginHelper.GetLanguageItem("Is Female")
        End Sub

        Private Sub OnModified(sender As Object, e As EventArgs) Handles txtPartnerName.TextChanged, txtPlayerName.TextChanged, cbPartner.SelectionChanged, cbPlayer.SelectionChanged
            IsModified = True
        End Sub

        Public Function GetSupportedTypes() As IEnumerable(Of Type) Implements iObjectControl.GetSupportedTypes
            Return {GetType(Saves.SkySave)}
        End Function

        Public Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer Implements iObjectControl.GetSortOrder
            Return 5
        End Function

#Region "IObjectControl Support"
        Public Function SupportsObject(Obj As Object) As Boolean Implements iObjectControl.SupportsObject
            Return True
        End Function

        Public Function IsBackupControl(Obj As Object) As Boolean Implements iObjectControl.IsBackupControl
            Return False
        End Function

        ''' <summary>
        ''' Called when Header is changed.
        ''' </summary>
        Public Event HeaderUpdated(sender As Object, e As EventArguments.HeaderUpdatedEventArgs) Implements iObjectControl.HeaderUpdated

        ''' <summary>
        ''' Called when IsModified is changed.
        ''' </summary>
        Public Event IsModifiedChanged As iObjectControl.IsModifiedChangedEventHandler Implements iObjectControl.IsModifiedChanged

        ''' <summary>
        ''' Returns the value of the Header.  Only used when the iObjectControl is behaving as a tab.
        ''' </summary>
        ''' <returns></returns>
        Public Property Header As String Implements iObjectControl.Header
            Get
                Return _header
            End Get
            Set(value As String)
                Dim oldValue = _header
                _header = value
                RaiseEvent HeaderUpdated(Me, New EventArguments.HeaderUpdatedEventArgs(oldValue, value))
            End Set
        End Property
        Dim _header As String

        ''' <summary>
        ''' Returns the current EditingObject, after casting it to type T.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <returns></returns>
        Protected Function GetEditingObject(Of T)() As T
            Return PluginHelper.Cast(Of T)(_editingObject)
        End Function

        ''' <summary>
        ''' Returns the current EditingObject.
        ''' It is recommended to use GetEditingObject(Of T), since it returns iContainter(Of T).Item if the EditingObject implements that interface.
        ''' </summary>
        ''' <returns></returns>
        Protected Function GetEditingObject() As Object
            Return _editingObject
        End Function

        ''' <summary>
        ''' The way to get the EditingObject from outside this class.  Refreshes the display on set, and updates the object on get.
        ''' Calling this from inside this class could result in a stack overflow, especially if called from UpdateObject, so use GetEditingObject or GetEditingObject(Of T) instead.
        ''' </summary>
        ''' <returns></returns>
        Public Property EditingObject As Object Implements iObjectControl.EditingObject
            Get
                UpdateObject()
                Return _editingObject
            End Get
            Set(value As Object)
                _editingObject = value
                RefreshDisplay()
            End Set
        End Property
        Dim _editingObject As Object

        ''' <summary>
        ''' Whether or not the EditingObject has been modified without saving.
        ''' Set to true when the user changes anything in the GUI.
        ''' Set to false when the object is saved, or if the user undoes every change.
        ''' </summary>
        ''' <returns></returns>
        Public Property IsModified As Boolean Implements iObjectControl.IsModified
            Get
                Return _isModified
            End Get
            Set(value As Boolean)
                Dim oldValue As Boolean = _isModified
                _isModified = value
                If Not oldValue = _isModified Then
                    RaiseEvent IsModifiedChanged(Me, New EventArgs)
                End If
            End Set
        End Property
        Dim _isModified As Boolean
#End Region

    End Class
End Namespace