''' <summary>
''' A variant of SkyEditorPlugin that is able to host an instance of Sky Editor
''' </summary>
Public MustInherit Class CoreSkyEditorPlugin
    Inherits SkyEditorPlugin

    ''' <summary>
    ''' Creates an instance of an IO Provider.
    ''' </summary>
    ''' <returns></returns>
    Public MustOverride Function GetIOProvider() As IOProvider

End Class

