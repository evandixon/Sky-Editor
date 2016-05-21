Namespace Settings
    Public Module ISettingsProviderExtensions

        ''' <summary>
        ''' Gets whether or not Development Mode is enabled.
        ''' </summary>
        ''' <returns></returns>
        <Extension> Function GetIsDevMode(provider As ISettingsProvider) As Boolean
            Dim setting As Boolean? = provider.GetSetting(My.Resources.SettingNames.DevMode)
            If setting.HasValue Then
                Return setting.Value
            Else
#If DEBUG Then
                Return True
#Else
                Return False
#End If
            End If
        End Function

        ''' <summary>
        ''' Sets whether or not Development Mode is enabled.
        ''' </summary>
        <Extension> Sub SetIsDevMode(provider As ISettingsProvider, value As Boolean)
            provider.SetSetting(My.Resources.SettingNames.DevMode, value)
        End Sub

        ''' <summary>
        ''' Gets the full paths of all files that are scheduled for deletion.
        ''' </summary>
        ''' <returns></returns>
        <Extension> Function GetFilesScheduledForDeletion(provider As ISettingsProvider) As IList(Of String)
            Dim setting As IList(Of String) = provider.GetSetting(My.Resources.SettingNames.FilesForDeletion)

            If setting Is Nothing OrElse TypeOf setting IsNot IList(Of String) Then
                setting = New List(Of String)
            End If

            Return setting
        End Function

        ''' <summary>
        ''' Schedules a file for deletion upon the next PluginManager load.
        ''' </summary>
        ''' <param name="path">Full path of the file to be deleted.</param>
        <Extension> Sub ScheduleFileForDeletion(provider As ISettingsProvider, path As String)
            Dim setting As IList(Of String) = provider.GetSetting(My.Resources.SettingNames.FilesForDeletion)

            If setting Is Nothing OrElse TypeOf setting IsNot IList(Of String) Then
                setting = New List(Of String)
            End If

            setting.Add(path)
            provider.SetSetting(My.Resources.SettingNames.FilesForDeletion, setting)
        End Sub

        ''' <summary>
        ''' Unschedules a file for deletion upon the next PluginManager load.
        ''' </summary>
        ''' <param name="path">Full path of the file to unschedule.</param>
        <Extension> Sub UncheduleFileForDeletion(provider As ISettingsProvider, path As String)
            Dim setting As IList(Of String) = provider.GetSetting(My.Resources.SettingNames.FilesForDeletion)

            If setting Is Nothing OrElse TypeOf setting IsNot IList(Of String) Then
                setting = New List(Of String)
            End If

            setting.Remove(path)
            provider.SetSetting(My.Resources.SettingNames.FilesForDeletion, setting)
        End Sub

        ''' <summary>
        ''' Gets the full paths of all directories that are scheduled for deletion.
        ''' </summary>
        ''' <returns></returns>
        <Extension> Function GetDirectoriesScheduledForDeletion(provider As ISettingsProvider) As IList(Of String)
            Dim setting As IList(Of String) = provider.GetSetting(My.Resources.SettingNames.DirectoriesForDeletion)

            If setting Is Nothing OrElse TypeOf setting IsNot IList(Of String) Then
                setting = New List(Of String)
            End If

            Return setting
        End Function

        ''' <summary>
        ''' Schedules a directory for deletion upon the next PluginManager load.
        ''' </summary>
        ''' <param name="path">Full path of the directory to be deleted.</param>
        <Extension> Sub ScheduleDirectoryForDeletion(provider As ISettingsProvider, path As String)
            Dim setting As IList(Of String) = provider.GetSetting(My.Resources.SettingNames.DirectoriesForDeletion)

            If setting Is Nothing OrElse TypeOf setting IsNot IList(Of String) Then
                setting = New List(Of String)
            End If

            setting.Add(path)
            provider.SetSetting(My.Resources.SettingNames.DirectoriesForDeletion, setting)
        End Sub

        ''' <summary>
        ''' Unschedules a directory for deletion upon the next PluginManager load.
        ''' </summary>
        ''' <param name="path">Full path of the directory to unschedule.</param>
        <Extension> Sub UncheduleDirectoryForDeletion(provider As ISettingsProvider, path As String)
            Dim setting As IList(Of String) = provider.GetSetting(My.Resources.SettingNames.DirectoriesForDeletion)

            If setting Is Nothing OrElse TypeOf setting IsNot IList(Of String) Then
                setting = New List(Of String)
            End If

            setting.Remove(path)
            provider.SetSetting(My.Resources.SettingNames.DirectoriesForDeletion, setting)
        End Sub
    End Module
End Namespace

