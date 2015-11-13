Imports SkyEditorBase.Interfaces

Public Class SettingsContainer
    Public Property CurrentLanguage As String
    Public Property DefaultLanguage As String
    Public Property UpdatePlugins As Boolean
    Public Property DebugLanguagePlaceholders As Boolean
    Public Property PluginUpdateUrl As String
    Public Sub New()
        'Todo: load defaults from somewhere
        CurrentLanguage = "English"
        DefaultLanguage = "English"
        UpdatePlugins = True
        DebugLanguagePlaceholders = False
        PluginUpdateUrl = "http://dl.uniquegeeks.net/SkyEditor4PluginsAlpha/plugins.json"
    End Sub
End Class
Public Class SettingsManager
    Implements Interfaces.iSavable
    Implements iModifiable
    Implements iNamed
    Private Shared _settingsManager As SettingsManager
    Public Event FileSaved(sender As Object, e As EventArgs) Implements iSavable.FileSaved
    Public Event Modified(sender As Object, e As EventArgs) Implements iModifiable.Modified

    Public Shared ReadOnly Property Instance As SettingsManager
        Get
            If _settingsManager Is Nothing Then
                _settingsManager = New SettingsManager
            End If
            Return _settingsManager
        End Get
    End Property

    Private Sub New()
        MyBase.New()
        If IO.File.Exists(SettingsFilename) Then
            Dim f As New ObjectFile(Of SettingsContainer)(SettingsFilename)
            Settings = f.ContainedObject
        Else
            Settings = New SettingsContainer
        End If
    End Sub

    Private Function SettingsFilename() As String
        Return IO.Path.Combine(PluginHelper.RootResourceDirectory, "Settings.json")
    End Function

    Public Property Settings As SettingsContainer
        Get
            Return _settings
        End Get
        Private Set(value As SettingsContainer)
            _settings = value
        End Set
    End Property

    Public Property Name As String Implements iNamed.Name
        Get
            Return PluginHelper.GetLanguageItem("Settings")
        End Get
        Set(value As String)

        End Set
    End Property

    Dim _settings As SettingsContainer

    Public Function DefaultExtension() As String Implements iSavable.DefaultExtension
        Return ".skysettings"
    End Function

    Public Sub Save() Implements iSavable.Save
        Save(SettingsFilename)
        RaiseEvent FileSaved(Me, New EventArgs)
    End Sub

    Public Sub Save(Filename As String) Implements iSavable.Save
        Dim f As New ObjectFile(Of SettingsContainer)
        f.ContainedObject = Settings
        f.Save(Filename)
        RaiseEvent FileSaved(Me, New EventArgs)
    End Sub

    Public Sub RaiseModified() Implements iModifiable.RaiseModified
        RaiseEvent Modified(Me, New EventArgs)
    End Sub
End Class