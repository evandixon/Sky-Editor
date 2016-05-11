Imports System.Windows.Controls
Imports ROMEditor.FileFormats.PSMD
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SkyEditorWPF
Imports SkyEditorWPF.UI

Public Class PsmdLuaLangIntegration
    Inherits ObjectControl
    Implements IDisposable

    Private Sub PsmdLuaLangIntegration_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Me.Header = My.Resources.Language.Message
    End Sub

    Public Overrides Async Sub RefreshDisplay()
        With PluginManager.GetInstance.GetOpenedFileProject(GetEditingObject) 'GetEditingObject(Of CodeFiles.LuaCodeFile)()
            Dim messageFiles As New Dictionary(Of String, MessageBin)
            For Each item In IO.Directory.GetDirectories(IO.Path.Combine(.GetRootDirectory, "Languages"), "*", IO.SearchOption.TopDirectoryOnly)
                Dim msgfile = New MessageBin
                Dim filename = IO.Path.Combine(item, IO.Path.GetFileNameWithoutExtension(GetEditingObject(Of CodeFiles.LuaCodeFile).Filename))

                Dim exists As Boolean = False
                If IO.File.Exists(filename) Then
                    exists = True
                ElseIf IO.File.Exists(filename & ".bin") Then
                    filename &= ".bin"
                    exists = True
                End If

                If exists Then
                    Await msgfile.OpenFile(filename, New SkyEditor.Core.Windows.IOProvider)
                    messageFiles.Add(IO.Path.GetFileName(item), msgfile)
                End If
            Next

            tcTabs.Items.Clear()
            For Each item In messageFiles
                Dim t As New TabItem
                t.Header = item.Key
                Dim p As New MessageBinEditor
                AddHandler p.IsModifiedChanged, AddressOf Me.OnModified
                t.Content = p
                p.EditingObject = item.Value
                tcTabs.Items.Add(t)
            Next
        End With
        IsModified = False
    End Sub

    Public Overrides Sub UpdateObject()
        For Each item As TabItem In tcTabs.Items
            DirectCast(DirectCast(item.Content, MessageBinEditor).EditingObject, MessageBin).Save()
        Next
    End Sub

    Public Overrides Function GetSupportedTypes() As IEnumerable(Of Type)
        Return {GetType(CodeFiles.LuaCodeFile)} '{GetType(Mods.ModSourceContainer)}
    End Function

    Public Overrides Function GetSortOrder(CurrentType As Type, IsTab As Boolean) As Integer
        Return 1
    End Function

    Public Overrides Function SupportsObject(Obj As Object) As Boolean
        Return PluginManager.GetInstance.GetOpenedFileProject(Obj) IsNot Nothing
    End Function

    Private Sub OnModified(sender As Object, e As EventArgs)
        IsModified = True
    End Sub

    Private Async Sub btnAdd_Click(sender As Object, e As RoutedEventArgs) Handles btnAdd.Click
        Dim p As Projects.PsmdLuaProject = PluginManager.GetInstance.GetOpenedFileProject(GetEditingObject)
        Dim oldText As String = btnAdd.Content
        If Not p.IsLanguageLoaded Then
            btnAdd.IsEnabled = False
            btnAdd.Content = String.Format(My.Resources.Language.GenericLoading, oldText)
        End If
        Dim id As UInteger = Await p.GetNewLanguageID
        For Each item As TabItem In tcTabs.Items
            DirectCast(DirectCast(item.Content, MessageBinEditor).EditingObject, MessageBin).AddBlankEntry(id)
        Next
        btnAdd.IsEnabled = True
        btnAdd.Content = oldText
    End Sub

    Private Sub btnSciptSort_Click(sender As Object, e As RoutedEventArgs) Handles btnSciptSort.Click
        Dim numberRegex As New Text.RegularExpressions.Regex("\-?[0-9]+")
        Dim matches As New List(Of Integer)
        For Each item As Text.RegularExpressions.Match In numberRegex.Matches(GetEditingObject(Of CodeFiles.LuaCodeFile).Text)
            matches.Add(CInt(item.Value))
        Next
        For Each item As TabItem In tcTabs.Items
            DirectCast(item.Content, MessageBinEditor).Sort(matches)
        Next
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If tcTabs IsNot Nothing Then
                    For Each item As TabItem In tcTabs.Items
                        If item.Content IsNot Nothing AndAlso TypeOf item.Content Is ObjectControlPlaceholder Then
                            DirectCast(item.Content, ObjectControlPlaceholder).Dispose()
                        ElseIf item.Content IsNot Nothing AndAlso TypeOf item.Content Is MessageBinEditor AndAlso DirectCast(item.Content, MessageBinEditor).EditingObject IsNot Nothing AndAlso TypeOf DirectCast(item.Content, MessageBinEditor).EditingObject Is IDisposable Then
                            DirectCast(item.Content, MessageBinEditor).EditingObject.Dispose()
                        End If
                    Next
                End If
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub

#End Region

End Class
