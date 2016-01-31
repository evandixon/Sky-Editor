Namespace Redistribution
    Public Class PluginInfo
        Public Enum PluginType
            Code
            Language
        End Enum
        Public Property Name As String
        ''' <summary>
        ''' Latest version of the plugin
        ''' </summary>
        ''' <returns></returns>
        Public Property VersionString As String

        ''' <summary>
        ''' Minimum required version of the plugin.
        ''' </summary>
        ''' <returns></returns>
        Public Property MinVersionString As String

        Public Property DownloadUrl As String
        ''' <summary>
        ''' List of plugins required to use this one.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Dependencies As List(Of PluginInfo)
        Public Property Type As PluginType
        Public Function GetVersion() As Version
            Return Version.Parse(VersionString)
        End Function
        Public Function GetMinVersion() As Version
            Return Version.Parse(MinVersionString)
        End Function
        Public Sub New()

        End Sub
    End Class
End Namespace
