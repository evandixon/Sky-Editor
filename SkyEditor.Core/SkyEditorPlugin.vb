''' <summary>
''' Defines a plugin for Sky Editor.
''' </summary>
Public MustInherit Class SkyEditorPlugin

    ''' <summary>
    ''' Name of the plugin.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride ReadOnly Property PluginName As String

    ''' <summary>
    ''' Name of the person or group who authored the plugin.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride ReadOnly Property PluginAuthor As String

    ''' <summary>
    ''' Credits for the plugin.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride ReadOnly Property Credits As String

    ''' <summary>
    ''' Registers types in the PluginManager and loads any required resources.
    ''' </summary>
    ''' <param name="manager"></param>
    ''' <remarks></remarks>
    Public Overridable Sub Load(manager As PluginManager)
    End Sub

    ''' <summary>
    ''' Disposes of any resources.
    ''' </summary>
    ''' <param name="manager"></param>
    ''' <remarks></remarks>
    Public Overridable Sub UnLoad(manager As PluginManager)
    End Sub

    ''' <summary>
    ''' This should delete all temporary and user-specific files that are not required to distribute Sky Editor.
    ''' </summary>
    ''' <remarks></remarks>
    Public Overridable Sub PrepareForDistribution()
    End Sub

End Class