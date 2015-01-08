Imports System.Reflection

''' <summary>
''' A collection of methods that are useful to Sky Editor plugins.
''' </summary>
''' <remarks></remarks>
Public Class PluginHelper
    ''' <summary>
    ''' Gets the name of the assembly of whatever assembly calls this method.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetAssemblyName() As String
        Return Assembly.GetCallingAssembly.GetName.Name
    End Function
    ''' <summary>
    ''' Combines the given path with your plugin's resource directory.
    ''' </summary>
    ''' <param name="Path"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetResourceName(Path As String) As String
        Dim baseDir = IO.Path.Combine(Environment.CurrentDirectory, "Resources/Plugins/", Assembly.GetCallingAssembly.GetName.Name.Replace("_plg", ""))
        If Not IO.Directory.Exists(baseDir) Then
            IO.Directory.CreateDirectory(baseDir)
        End If
        Return IO.Path.Combine(baseDir, Path)
    End Function
    ''' <summary>
    ''' Returns your plugin's resource directory as managed by Sky Editor.
    ''' It will be created if it does not exist.
    ''' </summary>
    ''' <param name="Path"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetResourceDirectory() As String
        Dim baseDir = IO.Path.Combine(Environment.CurrentDirectory, "Resources/Plugins/", Assembly.GetCallingAssembly.GetName.Name.Replace("_plg", ""))
        If Not IO.Directory.Exists(baseDir) Then
            IO.Directory.CreateDirectory(baseDir)
        End If
        Return baseDir
    End Function
    ''' <summary>
    ''' Gets the text specified by the currently loaded language files.
    ''' </summary>
    ''' <param name="Key">The name of the language item to load.</param>
    ''' <param name="DefaultValue">If the currently selected language and the English language files both do not contain the requested Key,
    ''' this value will be returned and written to the English language files in your plugin's resource directory.
    ''' If Nothing (or not provided), will be set to Key in the event it's needed.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetLanguageItem(Key As String, Optional DefaultValue As String = Nothing) As String
        If SkyEditorBase.Settings.GetSettings.Setting("DebugLangaugePlaceholders") = "True" Then
            Return String.Format("[{0}]", Key)
        Else
            Return Internal.LanguageManager.GetLanguageItem(Key, Assembly.GetCallingAssembly.GetName.Name, DefaultValue)
        End If
    End Function
End Class
