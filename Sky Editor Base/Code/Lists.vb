Imports SkyEditorBase.Utilities
Partial Public Class Lists
    Public Shared Function CurrentLanguage() As String
        Return Settings("Language")
    End Function
    Public Shared Function LanguageText() As Dictionary(Of String, String)
        Static _dictionaryCache As New ResourceDictionary("&L;/Language.txt")
        Return _dictionaryCache
    End Function
    Private Shared _settingsCache As ResourceDictionary
    Public Shared Property Settings As Dictionary(Of String, String)
        Get
            If _settingsCache Is Nothing Then _settingsCache = New ResourceDictionary("Settings.txt")
            Return _settingsCache
        End Get
        Set(value As Dictionary(Of String, String))
            Dim settingsText As String = ""
            For Each item In value
                settingsText &= item.Key & "=" & item.Value & vbCrLf
            Next
            IO.File.WriteAllText(IO.Path.Combine(Environment.CurrentDirectory, "Resources\Settings.txt"), settingsText.Trim)
        End Set
    End Property
End Class


