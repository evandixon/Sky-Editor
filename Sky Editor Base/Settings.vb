Imports SkyEditorBase.Interfaces
Public Class SettingsManager
    Implements Interfaces.ISavableAs
    Implements iModifiable
    Implements iNamed
    Friend Class SettingsValue
        Public Property TypeName As String
        Public Property Value As String
    End Class
    Friend Class SettingsFile
        Public Property Settings As Dictionary(Of String, SettingsValue)
    End Class

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
                If Setting("CurrentLanguage") Is Nothing Then
                    Setting("CurrentLanguage") = "English"
                End If
                Return Setting("CurrentLanguage")
            End Get
            Set(value As String)
                Setting("CurrentLanguage") = value
            End Set
        End Property

        Public Property DefaultLanguage As String
            Get
                If Setting("DefaultLanguage") Is Nothing Then
                    Setting("DefaultLanguage") = "English"
                End If
                Return Setting("DefaultLanguage")
            End Get
            Set(value As String)
                Setting("DefaultLanguage") = value
            End Set
        End Property

        Public Property UpdatePlugins As Boolean
            Get
                If Setting("UpdatePlugins") Is Nothing Then
                    Setting("UpdatePlugins") = True
                End If
                Return Setting("UpdatePlugins")
            End Get
            Set(value As Boolean)
                Setting("UpdatePlugins") = value
            End Set
        End Property

        Public Property DebugLanguagePlaceholders As Boolean
            Get
                If Setting("DebugLanguagePlaceholders") Is Nothing Then
                    Setting("DebugLanguagePlaceholders") = False
                End If
                Return Setting("DebugLanguagePlaceholders")
            End Get
            Set(value As Boolean)
                Setting("DebugLanguagePlaceholders") = value
            End Set
        End Property

        Public Property PluginUpdateUrl As String
            Get
                If Setting("PluginUpdateUrl") Is Nothing Then
                    Setting("PluginUpdateUrl") = ""
                End If
                Return Setting("PluginUpdateUrl")
            End Get
            Set(value As String)
                Setting("PluginUpdateUrl") = value
            End Set
        End Property

        Public Property VerboseOutput As Boolean
            Get
                If Setting("VerboseOutput") Is Nothing Then
                    Setting("VerboseOutput") = False
                End If
                Return Setting("VerboseOutput")
            End Get
            Set(value As Boolean)
                Setting("VerboseOutput") = value
            End Set
        End Property

        ''' <summary>
        ''' Activates certain features for developer use.  See remarks for details.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>Development Mode does the following:
        ''' •Load all plugins in plugin directory.  Usually only plugins defined in SettingsSet.Plugins are defined.  Any plugin loaded with this override will be added to the plugins settings.
        ''' •Enables use of Redistribution Helpers, which helps with packaging plugins, or the application as a whole.</remarks>
        Public Property DevelopmentMode As Boolean
            Get
                If Setting("DevMode") Is Nothing Then
                    Setting("DevMode") = True
                End If
                Return Setting("DevMode")
            End Get
            Set(value As Boolean)
                Setting("DevMode") = value
            End Set
        End Property

        ''' <summary>
        ''' List of assembly paths, relative to the application's plugin directory, of plugins to be loaded.
        ''' </summary>
        ''' <returns></returns>
        Public Property Plugins As List(Of String)
            Get
                If Setting("Plugins") Is Nothing Then
                    Setting("Plugins") = New List(Of String)
                End If
                Return Setting("Plugins")
            End Get
            Set(value As List(Of String))
                Setting("Plugins") = value
            End Set
        End Property

        Friend Function Serialize() As String
            Dim settingsItems As New Dictionary(Of String, SettingsValue)
            For Each item In Settings
                Dim v As New SettingsValue() With {.TypeName = item.Value.GetType.AssemblyQualifiedName, .Value = Utilities.Json.Serialize(item.Value)}
                settingsItems.Add(item.Key, v)
            Next
            Dim f As New SettingsFile
            f.Settings = settingsItems
            Return Utilities.Json.Serialize(f)
        End Function

        Friend Sub New()
            Settings = New Dictionary(Of String, Object)
            'Todo: load defaults from somewhere
            CurrentLanguage = "English"
            DefaultLanguage = "English"
            UpdatePlugins = True
            DebugLanguagePlaceholders = False
            PluginUpdateUrl = "http://dl.uniquegeeks.net/SkyEditor4BetaPlugins/plugins.json"
            VerboseOutput = False
        End Sub
        Friend Sub New(File As SettingsFile)
            Settings = New Dictionary(Of String, Object)
            If File.Settings IsNot Nothing Then
                For Each item In File.Settings
                    Dim t = Utilities.ReflectionHelpers.GetTypeFromName(item.Value.TypeName)
                    If t IsNot Nothing Then
                        Dim obj = Utilities.Json.Deserialize(t, item.Value.Value)
                        Settings.Add(item.Key, obj)
                    Else
                        Dim obj = item.Value.Value
                        Settings.Add(item.Key, obj)
                    End If
                Next
            Else
                Settings = New Dictionary(Of String, Object)
                CurrentLanguage = "English"
                DefaultLanguage = "English"
                UpdatePlugins = True
                DebugLanguagePlaceholders = False
                PluginUpdateUrl = "http://dl.uniquegeeks.net/SkyEditor4BetaPlugins/plugins.json"
                VerboseOutput = False
            End If
        End Sub
    End Class

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
            Dim file = Utilities.Json.DeserializeFromFile(Of SettingsFile)(SettingsFilename)
            Settings = New SettingsSet(file)
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
    End Sub

    Public Sub Save(Filename As String) Implements ISavableAs.Save
        IO.File.WriteAllText(Filename, Settings.Serialize)
        RaiseEvent FileSaved(Me, New EventArgs)
    End Sub

    Public Sub RaiseModified() Implements iModifiable.RaiseModified
        RaiseEvent Modified(Me, New EventArgs)
    End Sub
End Class