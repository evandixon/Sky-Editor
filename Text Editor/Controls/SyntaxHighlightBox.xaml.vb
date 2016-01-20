Imports System.Windows.Controls
Imports System.Windows.Input
Imports AurelienRibon.Ui.SyntaxHighlightBox
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace Controls
    Public Class SyntaxHighlightBox
        Inherits UserControl
        Implements iObjectControl

        Private WithEvents AutoComplete As AutoCompletePopup

        Public Sub RefreshDisplay()
            txtCode.CurrentHighlighter = GetEditingObject(Of CodeFile).CodeHighlighter
            txtCode.Text = GetEditingObject(Of CodeFile).Text
            IsModified = False
        End Sub
        Public Sub UpdateObject()
            GetEditingObject(Of CodeFile).Text = txtCode.Text
        End Sub

        Private Sub txtCode_TextChanged(sender As Object, e As TextChangedEventArgs) Handles txtCode.TextChanged
            IsModified = True
        End Sub

        Public Function GetSupportedTypes() As IEnumerable(Of Type) Implements iObjectControl.GetSupportedTypes
            Return {GetType(CodeFile)}
        End Function

        Public Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer Implements iObjectControl.GetSortOrder
            Return 0
        End Function
        Private Sub txtCode_KeyUp(sender As Object, e As KeyEventArgs) Handles txtCode.KeyUp
            If Not e.Key = Key.RightShift AndAlso Not e.Key = Key.LeftShift Then
                Select Case e.Key
                    Case Else
                        If IsAutoCompleteOpen() Then
                            Dim partStart = txtCode.Text.LastIndexOf(".", txtCode.CaretIndex)
                            If partStart = -1 Then
                                partStart = 0
                            End If
                            Dim part As String = txtCode.Text.Substring(partStart, txtCode.CaretIndex - partStart).Trim.Trim(".")
                            FilterAutoComplete(part)
                        End If
                End Select
            End If
        End Sub
        Private Sub txtCode_KeyDown(sender As Object, e As KeyEventArgs) Handles txtCode.KeyDown
            If Not e.Key = Key.RightShift AndAlso Not e.Key = Key.LeftShift Then
                Select Case e.Key
                    Case Key.OemPeriod
                        'Get the last word, preceding the period
                        Dim wordStart = txtCode.Text.LastIndexOf(" ", txtCode.CaretIndex - 1)
                        If wordStart = -1 Then
                            wordStart = 0
                        End If
                        Dim lastWord As String = txtCode.Text.Substring(wordStart, txtCode.CaretIndex - wordStart).Trim.Trim(".").ToLower
                        If lastWord = "testc" Then
                            ShowAutoComplete(txtCode.GetRectFromCharacterIndex(txtCode.CaretIndex, True), lastWord)
                        End If
                End Select
            End If
        End Sub
        Private Sub txtCode_PreviewKeyDown(sender As Object, e As KeyEventArgs) Handles txtCode.PreviewKeyDown
            Select Case e.Key
                Case Key.Tab
                    If IsAutoCompleteOpen() Then
                        Hide()
                        AutoFill()
                    End If
                Case Key.Enter
                    If IsAutoCompleteOpen() Then
                        Hide()
                        AutoFill()
                    End If
                Case Key.Escape
                    Hide()
            End Select
        End Sub

        Sub ShowAutoComplete(Position As Windows.Rect, PreviousWord As String)
            AutoComplete.PlacementRectangle = Position
            AutoComplete.ItemsSource = GetItemsSource(PreviousWord)
            AutoComplete.IsOpen = True
            AutoComplete.Focus()
        End Sub
        Function GetItemsSource(PreviousWord As String)
            Return From s In {"Alpha", "Beta", "Others", "Omega"} Select s
        End Function
        Sub FilterAutoComplete(CurrentText As String)
            AutoComplete.ApplyFilter(CurrentText)
        End Sub
        Function IsAutoCompleteOpen()
            Return AutoComplete.IsOpen
        End Function
        Sub Hide()
            AutoComplete.IsOpen = False
        End Sub
        Sub AutoFill()
            Dim index = txtCode.CaretIndex
            Dim text = AutoComplete.SelectedItem
            If text IsNot Nothing Then
                'We're going to insert a whole word, but there might already be part of one.
                'First we remove the part
                Dim partStart = txtCode.Text.LastIndexOf(".", txtCode.CaretIndex)
                If partStart = -1 Then
                    partStart = 0
                End If
                Dim part As String = txtCode.Text.Substring(partStart, txtCode.CaretIndex - partStart).Trim.Trim(".")
                index -= part.Length
                txtCode.Text = txtCode.Text.Remove(index, part.Length)
                'Then we add the new word
                txtCode.Text = txtCode.Text.Insert(index, text)
                txtCode.CaretIndex = index + text.Length
                txtCode.Focus()
            End If
        End Sub

        Public Sub New()
            ' This call is required by the designer.
            InitializeComponent()

            ' Add any initialization after the InitializeComponent() call.
            AutoComplete = New AutoCompletePopup
            AutoComplete.PlacementTarget = txtCode
        End Sub


#Region "IObjectControl Support"
        ''' <summary>
        ''' Called when Header is changed.
        ''' </summary>
        Public Event HeaderUpdated As iObjectControl.HeaderUpdatedEventHandler Implements iObjectControl.HeaderUpdated

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

