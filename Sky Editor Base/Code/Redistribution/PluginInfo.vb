Public Class PluginInfo
    Public Property Name As String
    Public Property VersionString As String
    Public Function GetVersion() As Version
        Return Version.Parse(VersionString)
    End Function
    Public Property DownloadUrl As String
    ''' <summary>
    ''' List of plugins required to use this one.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Dependencies As List(Of PluginInfo)
    Public Property Type As PluginType
    Public Enum PluginType
        Code
        Language
    End Enum
    Public Sub New()

    End Sub
End Class