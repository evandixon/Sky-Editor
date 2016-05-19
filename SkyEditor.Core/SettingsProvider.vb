Imports System.Collections.Concurrent
Imports SkyEditor.Core
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.Utilities

Public Class SettingsProvider
    Implements ISettingsProvider

    Protected Class SerializedSettings
        Public Property Settings As Dictionary(Of String, SerializedValue)
        Public Sub New()
            Settings = New Dictionary(Of String, SerializedValue)
        End Sub
    End Class

    Protected Class SerializedValue
        Public Property Value As String
        Public Property TypeName As String
    End Class

    Public Sub New()
        Me.Settings = New ConcurrentDictionary(Of String, Object)
        Me.UnloadableSettings = New ConcurrentDictionary(Of String, SerializedValue)
    End Sub

    ''' <summary>
    ''' Deserializes the given data into a SettingsProvider
    ''' </summary>
    ''' <param name="data">Data to be deserialized</param>
    ''' <param name="manager">Current instance of the plugin manager</param>
    ''' <returns></returns>
    Public Shared Function Deserialize(data As String, manager As PluginManager) As SettingsProvider
        If manager Is Nothing Then
            Throw New ArgumentNullException(NameOf(manager))
        End If

        Dim out As New SettingsProvider
        out.CurrentPluginManager = manager
        Dim s As SerializedSettings
        If String.IsNullOrEmpty(data) Then
            'The file is empty, or data is null.
            s = New SerializedSettings
        Else
            s = Json.Deserialize(Of SerializedSettings)(data)
        End If
        For Each item In s.Settings
            Dim valueType = ReflectionHelpers.GetTypeByName(item.Value.TypeName, manager)
            If valueType Is Nothing Then
                'If we can't load the type, then it's possible that the PluginManager hasn't fully loaded everything yet.
                'We'll store the serialized value and try this part again if anyone requests the property.
                out.UnloadableSettings(item.Key) = item.Value
            Else
                out.Settings(item.Key) = Json.Deserialize(valueType.AsType, item.Value.Value)
            End If
        Next
        Return out
    End Function

    ''' <summary>
    ''' Opens a settings provider stored at the given filename, or creates a new one if it does not exist.
    ''' </summary>
    ''' <param name="filename">Full path of the settings file</param>
    ''' <param name="manager">Current instance of the plugin manager</param>
    ''' <returns></returns>
    Public Shared Function Open(filename As String, manager As PluginManager) As SettingsProvider
        If manager Is Nothing Then
            Throw New ArgumentNullException(NameOf(manager))
        End If

        If manager.CurrentIOProvider.FileExists(filename) Then
            Dim out = Deserialize(manager.CurrentIOProvider.ReadAllText(filename), manager)
            out.Filename = filename
            Return out
        Else
            Dim out As New SettingsProvider
            out.CurrentPluginManager = manager
            out.Filename = filename
            Return out
        End If
    End Function

    Protected Property CurrentPluginManager As PluginManager

    Public Property Filename As String

    ''' <summary>
    ''' The settings contained by the settings provider.
    ''' </summary>
    ''' <returns></returns>
    Protected Property Settings As ConcurrentDictionary(Of String, Object)

    Protected Property UnloadableSettings As ConcurrentDictionary(Of String, SerializedValue)

    ''' <summary>
    ''' Serializes the current SettingsProvider
    ''' </summary>
    ''' <returns></returns>
    Public Function Serialize() As String
        Dim s As New SerializedSettings
        'Save the settings
        For Each item In Settings
            Dim v As New SerializedValue
            v.TypeName = item.Value.GetType.AssemblyQualifiedName
            v.Value = Json.Serialize(item.Value)
            s.Settings.Add(item.Key, v)
        Next
        'Save the settings we couldn't open
        For Each item In UnloadableSettings
            'Make sure we don't have a duplicate.
            'The cases where there could be a duplicate:
            '1. A setting with the same name was set without a successful load beforehand.
            '2. The setting was loaded properly, but not removed from UnloadableSettings due to threading.
            If Not s.Settings.ContainsKey(item.Key) Then
                'This isn't a duplicate
                s.Settings.Add(item.Key, item.Value)
            End If
        Next
        Return Json.Serialize(s)
    End Function

    ''' <summary>
    ''' Saves the SettingsProvider to the filename it was loaded with.
    ''' </summary>
    ''' <exception cref="ArgumentNullException">Thrown if SettingsProvider.Filename is null.</exception>
    Public Sub Save(provider As IOProvider) Implements ISettingsProvider.Save
        If String.IsNullOrEmpty(Filename) Then
            Throw New ArgumentNullException(NameOf(Filename))
        End If

        provider.WriteAllText(Me.Filename, Me.Serialize)
        RaiseEvent FileSaved(Me, New EventArgs)
    End Sub

    ''' <summary>
    ''' Sets a setting.
    ''' </summary>
    ''' <param name="name">Name of the setting.</param>
    ''' <param name="value">Value of the setting.  Must be serializable to JSON.</param>
    Public Sub SetSetting(name As String, value As Object) Implements ISettingsProvider.SetSetting
        If String.IsNullOrEmpty(name) Then
            Throw New ArgumentNullException(name)
        End If

        Settings(name) = value
    End Sub

    Public Event FileSaved(sender As Object, e As EventArgs) Implements iSavable.FileSaved

    ''' <summary>
    ''' Gets a setting with the given name or returns null if it doesn't exist.
    ''' </summary>
    ''' <param name="name">Name of the setting.</param>
    ''' <returns></returns>
    Public Function GetSetting(name As String) As Object Implements ISettingsProvider.GetSetting
        If String.IsNullOrEmpty(name) Then
            Throw New ArgumentNullException(name)
        End If

        If Settings.ContainsKey(name) Then
            Return Settings(name)
        Else
            'If the setting doesn't exist, check to see if we couldn't load it.
            If UnloadableSettings.ContainsKey(name) Then
                'If we couldn't load it, try again
                Dim valueType = ReflectionHelpers.GetTypeByName(UnloadableSettings(name).TypeName, CurrentPluginManager)
                If valueType Is Nothing Then
                    'If we still can't load it, return null as if we don't have it.
                    Return Nothing
                Else
                    'If we can load it, then add it to the main settings
                    Settings(name) = Json.Deserialize(valueType.AsType, UnloadableSettings(name).Value)
                    'Then remove it from the unloadable settings
                    'If it fails, then it doesn't matter too much since it will be ignored.
                    Dim tmp = Nothing
                    UnloadableSettings.TryRemove(name, tmp)

                    Return Settings(name)
                End If
            Else
                'The setting doesn't exist.
                Return Nothing
            End If
        End If
    End Function
End Class
