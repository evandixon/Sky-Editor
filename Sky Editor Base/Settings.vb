Imports SkyEditorBase.Interfaces

Public Class SettingsSet
    Private Property Settings As Dictionary(Of String, Object)

    Public Property Setting(SettingName As String) As Object
        Get
            If Settings.ContainsKey(SettingName) Then
                Return Settings.Item(SettingName)
            Else
                Return Nothing
            End If
        End Get
        Set(value As Object)
            If Settings.ContainsKey(SettingName) Then
                Settings.Item(SettingName) = value
            Else
                Settings.Add(SettingName, value)
            End If
        End Set
    End Property

    Public Property CurrentLanguage As String
        Get
            Return Setting("CurrentLanguage")
        End Get
        Set(value As String)
            Setting("CurrentLanguage") = value
        End Set
    End Property

    Public Property DefaultLanguage As String
        Get
            Return Setting("DefaultLanguage")
        End Get
        Set(value As String)
            Setting("DefaultLanguage") = value
        End Set
    End Property

    Public Property UpdatePlugins As Boolean
        Get
            Return Setting("UpdatePlugins")
        End Get
        Set(value As Boolean)
            Setting("UpdatePlugins") = value
        End Set
    End Property

    Public Property DebugLanguagePlaceholders As Boolean
        Get
            Return Setting("DebugLanguagePlaceholders")
        End Get
        Set(value As Boolean)
            Setting("DebugLanguagePlaceholders") = value
        End Set
    End Property

    Public Property PluginUpdateUrl As String
        Get
            Return Setting("PluginUpdateUrl")
        End Get
        Set(value As String)
            Setting("PluginUpdateUrl") = value
        End Set
    End Property

    Public Property VerboseOutput As Boolean
        Get
            Return Setting("VerboseOutput")
        End Get
        Set(value As Boolean)
            Setting("VerboseOutput") = value
        End Set
    End Property

    Public Sub New()
        Settings = New Dictionary(Of String, Object)
        'Todo: load defaults from somewhere
        CurrentLanguage = "English"
        DefaultLanguage = "English"
        UpdatePlugins = True
        DebugLanguagePlaceholders = False
        PluginUpdateUrl = "http://dl.uniquegeeks.net/SkyEditor4PluginsAlpha/plugins.json"
        VerboseOutput = False
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
            Dim f As New ObjectFile(Of SettingsSet)(SettingsFilename)
            Settings = f.ContainedObject
        Else
            Settings = New SettingsSet
        End If
    End Sub

    Private Function SettingsFilename() As String
        Return IO.Path.Combine(PluginHelper.RootResourceDirectory, "Settings.json")
    End Function

    Public Property Settings As SettingsSet
        Get
            Return _settings
        End Get
        Private Set(value As SettingsSet)
            _settings = value
        End Set
    End Property
    Dim _settings As SettingsSet

    Public Property Name As String Implements iNamed.Name
        Get
            Return PluginHelper.GetLanguageItem("Settings")
        End Get
        Set(value As String)

        End Set
    End Property

    Public Function DefaultExtension() As String Implements iSavable.DefaultExtension
        Return ".skysettings"
    End Function

    Public Sub Save() Implements iSavable.Save
        Save(SettingsFilename)
        RaiseEvent FileSaved(Me, New EventArgs)
    End Sub

    Public Sub Save(Filename As String) Implements iSavable.Save
        Dim f As New ObjectFile(Of SettingsSet)
        f.ContainedObject = Settings
        f.Save(Filename)
        RaiseEvent FileSaved(Me, New EventArgs)
    End Sub

    Public Sub RaiseModified() Implements iModifiable.RaiseModified
        RaiseEvent Modified(Me, New EventArgs)
    End Sub
End Class