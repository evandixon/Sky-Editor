Public MustInherit Class PatcherCore

#Region "Progress Changed"
    Public Class ProgressChangedEventArgs
        Inherits EventArgs
        Public Property Progress As Single
        Public Property Message As String
    End Class
    Public Event ProgressChanged(sender As Object, e As ProgressChangedEventArgs)
    Protected Sub RaiseProgressChanged(sender As Object, e As ProgressChangedEventArgs)
        RaiseEvent ProgressChanged(sender, e)
    End Sub
    Protected Sub RaiseProgressChanged(Progress As Single, Message As String)
        Dim e As New ProgressChangedEventArgs
        e.Progress = Progress
        e.Message = Message
        RaiseProgressChanged(Me, e)
    End Sub
#End Region

    Public Property SelectedFilename As String

    ''' <summary>
    ''' Prompts the user for a file, then stores the filename in PatcherCore.SelectedFilename
    ''' </summary>
    Public MustOverride Sub PromptFilePath()

    ''' <summary>
    ''' Determines whether or not the given mod supports the file chosen with SelectFilePath.
    ''' </summary>
    ''' <param name="ModToCheck"></param>
    ''' <returns></returns>
    Public MustOverride Function SupportsMod(ModToCheck As ModJson) As Boolean
    Public MustOverride Function RunPatch(Mods As IEnumerable(Of ModJson), Optional DestinationPath As String = Nothing) As Task
End Class
