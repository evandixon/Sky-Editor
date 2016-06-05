Imports System.Windows
Imports System.Windows.Input
Imports CodeFiles
Imports ICSharpCode.AvalonEdit.CodeCompletion
Imports SkyEditor.Core
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.UI
Imports SkyEditorBase

Public Class AvalonEditControl
    Implements IObjectControl

    Dim extraData As CodeExtraData
    Private WithEvents autoComplete As CompletionWindow

    Public Sub SetPluginManager(manager As PluginManager) Implements IObjectControl.SetPluginManager
        'Do nothing, not needed for this control
    End Sub

    Public Sub RefreshDisplay()
        txtCode.ShowLineNumbers = True

        Dim highlighter As New AvalonCodeHighlighter(GetEditingObject(Of CodeFile).GetCodeHighlightRules)

        'Dim p = PluginManager.GetInstance.GetOpenedFileProject(GetEditingObject)
        'If p IsNot Nothing AndAlso TypeOf p Is ICodeProject Then
        '    extraData = DirectCast(p, ICodeProject).GetExtraData(GetEditingObject(Of CodeFile))
        'Else
        extraData = New DebugExtraData
        'End If

        If extraData IsNot Nothing AndAlso extraData.AdditionalHighlightRules IsNot Nothing Then
            highlighter.AddRuleSet("Project Rules", extraData.AdditionalHighlightRules)
        End If

        txtCode.SyntaxHighlighting = highlighter
        txtCode.Text = GetEditingObject(Of CodeFile).Text
        IsModified = False
    End Sub
    Public Sub UpdateObject()
        GetEditingObject(Of CodeFile).Text = txtCode.Text
    End Sub

    Private Sub txtCode_TextChanged(sender As Object, e As EventArgs) Handles txtCode.TextChanged
        IsModified = True
    End Sub

    Public Function GetSupportedTypes() As IEnumerable(Of Type) Implements IObjectControl.GetSupportedTypes
        'Return {GetType(TextFile)}
        Return {GetType(CodeFile)}
    End Function

    Public Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer Implements IObjectControl.GetSortOrder
        Return 0
    End Function

    Private Sub txtCode_TextEntered(sender As Object, e As TextCompositionEventArgs)
        If extraData IsNot Nothing Then
            If extraData.GetAutoCompleteChars.Contains(e.Text) Then
                autoComplete = New CompletionWindow(txtCode.TextArea)
                With autoComplete.CompletionList.CompletionData
                    For Each item In extraData.GetAutoCompleteData(GetLastPart(" "))
                        .Add(New AutoCompleteData(item, extraData.GetAutoCompleteChars))
                    Next
                End With
                autoComplete.Show()
            End If
        End If
    End Sub

    Private Sub txtCode_TextEntering(sender As Object, e As TextCompositionEventArgs)
        If autoComplete IsNot Nothing AndAlso e.Text.Length > 0 Then
            If Not Char.IsLetterOrDigit(e.Text(0)) Then
                autoComplete.CompletionList.RequestInsertion(e)
            End If
        End If
    End Sub

    Private Function GetLastPart(PrecedingChar As String)
        Dim partStart = txtCode.Text.LastIndexOf(PrecedingChar, txtCode.CaretOffset)
        If partStart = -1 Then
            partStart = 0
        End If
        Dim part As String = txtCode.Text.Substring(partStart + PrecedingChar.Length, txtCode.CaretOffset - partStart).Trim
        Return part
    End Function

    Private Sub autoComplete_Closed(sender As Object, e As EventArgs) Handles autoComplete.Closed
        autoComplete = Nothing
    End Sub

    Private Sub AvalonEditControl_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.Header = My.Resources.Language.Code
    End Sub

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        AddHandler txtCode.TextArea.TextEntering, AddressOf txtCode_TextEntering
        AddHandler txtCode.TextArea.TextEntered, AddressOf txtCode_TextEntered
    End Sub

#Region "IObjectControl Support"
    Public Function SupportsObject(Obj As Object) As Boolean Implements IObjectControl.SupportsObject
        Return True
    End Function

    Public Function IsBackupControl(Obj As Object) As Boolean Implements IObjectControl.IsBackupControl
        Return False
    End Function

    ''' <summary>
    ''' Called when Header is changed.
    ''' </summary>
    Public Event HeaderUpdated As IObjectControl.HeaderUpdatedEventHandler Implements IObjectControl.HeaderUpdated

    ''' <summary>
    ''' Called when IsModified is changed.
    ''' </summary>
    Public Event IsModifiedChanged As IObjectControl.IsModifiedChangedEventHandler Implements IObjectControl.IsModifiedChanged

    ''' <summary>
    ''' Returns the value of the Header.  Only used when the iObjectControl is behaving as a tab.
    ''' </summary>
    ''' <returns></returns>
    Public Property Header As String Implements IObjectControl.Header
        Get
            Return _header
        End Get
        Set(value As String)
            Dim oldValue = _header
            _header = value
            RaiseEvent HeaderUpdated(Me, New HeaderUpdatedEventArgs(oldValue, value))
        End Set
    End Property
    Dim _header As String

    ''' <summary>
    ''' Returns the current EditingObject, after casting it to type T.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    Protected Function GetEditingObject(Of T)() As T
        If TypeOf _editingObject Is T Then
            Return DirectCast(_editingObject, T)
        ElseIf TypeOf _editingObject Is IContainer(Of T) Then
            Return DirectCast(_editingObject, IContainer(Of T)).Item
        Else
            'I should probably throw my own exception here, since I'm casting EditingObject to T even though I just found that EditingObject is NOT T, but there will be an exception anyway
            Return DirectCast(_editingObject, T)
        End If
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
    Public Property EditingObject As Object Implements IObjectControl.EditingObject
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
    Public Property IsModified As Boolean Implements IObjectControl.IsModified
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
