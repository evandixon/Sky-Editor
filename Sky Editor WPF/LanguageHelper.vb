Imports System.Resources

Public Class LanguageHelper
    Public Shared Function GetLanguageItem(langaugeItem As String) As String
        Return My.Resources.ResourceManager.GetString(langaugeItem)
    End Function
End Class
