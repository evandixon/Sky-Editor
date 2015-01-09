Public Interface iSkyEditorPlugin
    ReadOnly Property PluginName As String
    ReadOnly Property PluginAuthor As String
    ReadOnly Property Credits As String
    ''' <summary>
    ''' Called when the form is loaded.  Use this to register save types and other resources your plugin may add to the form.
    ''' </summary>
    ''' <param name="Window"></param>
    ''' <remarks></remarks>
    Sub Load(ByRef Manager As PluginManager)
    ''' <summary>
    ''' Called on form close.  Use this to free any resources in need of disposal or delete temporary files, if applicable.
    ''' </summary>
    ''' <param name="Window"></param>
    ''' <remarks></remarks>
    Sub UnLoad(ByRef Manager As PluginManager)
    ''' <summary>
    ''' This should delete all temporary and user-specific files that are not required to distribute Sky Editor.
    ''' Should delete any copyrighted data such as ROMs.
    ''' </summary>
    ''' <remarks></remarks>
    Sub PrepareForDistribution()


End Interface
