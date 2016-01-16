Imports System.Runtime.Serialization
Imports System.Reflection
Imports SkyEditorBase

Namespace Utilities
    <Serializable()>
    Public Class ResourceDictionary
        Inherits Dictionary(Of String, String)
        Public Sub New(ResourceName As String)
            MyBase.New()
            Dim newResourceName As String
            If ResourceName.Contains("&L;") Then
                newResourceName = ResourceName.Replace("&L;", SettingsManager.Instance.Settings.CurrentLanguage)
            Else
                newResourceName = ResourceName
            End If

            Dim lines As String() = Nothing
            If IO.File.Exists(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins\", Assembly.GetCallingAssembly.GetName.Name.Replace("_plg", ""), newResourceName)) Then
                lines = IO.File.ReadAllLines(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins\", Assembly.GetCallingAssembly.GetName.Name.Replace("_plg", ""), newResourceName))
            Else 'Fall back to English
                newResourceName = ResourceName.Replace("&L;", "English")
                If IO.File.Exists(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins\", Assembly.GetCallingAssembly.GetName.Name.Replace("_plg", ""), newResourceName)) Then
                    lines = IO.File.ReadAllLines(IO.Path.Combine(PluginHelper.RootResourceDirectory, "Plugins\", Assembly.GetCallingAssembly.GetName.Name.Replace("_plg", ""), newResourceName))
                End If
            End If
            If lines IsNot Nothing Then
                For Each line In lines
                    If Not String.IsNullOrEmpty(line) AndAlso Not line.StartsWith("#") Then
                        Dim p As String() = line.Split("=".ToCharArray, 2)
                        If Not Me.ContainsKey(FormatString(p(0))) Then
                            Me.Add(FormatString(p(0)), FormatString(p(1)))
                        End If
                    End If
                Next
            End If
        End Sub
        Protected Sub New(info As SerializationInfo, context As StreamingContext)
            MyBase.New(info, context)
        End Sub
        Shared Function FormatString(Input As String) As String
            Return Input.Replace("<br/>", vbCrLf)
        End Function
    End Class
End Namespace