Public Class Settings
    Public Shared Function CurrentLanguage() As String
        Return Settings("Language")
    End Function
    Public Shared Function DebugMode() As Boolean
        Return Settings.ContainsKey("DebugMode") AndAlso Settings("DebugMode") = "True"
    End Function
    Public Shared Function ShowConsoleOnStart() As Boolean
        Return Settings.ContainsKey("ShowConsoleOnStart") AndAlso Settings("ShowConsoleOnStart") = "True"
    End Function
    Public Shared Function TabStripPlacement() As System.Windows.Controls.Dock
        If Settings.ContainsKey("TabStripPlacement") Then
            Return Settings("TabStripPlacement")
        Else
            Return Dock.Top
        End If
    End Function
    Private Shared _settingsCache As Dictionary(Of String, String)
    Protected Shared Property Settings As Dictionary(Of String, String)
        Get
            If _settingsCache Is Nothing Then
                _settingsCache = New Dictionary(Of String, String)
                Dim lines = IO.File.ReadAllLines(IO.Path.Combine(Environment.CurrentDirectory, "Resources/Settings.txt"))
                For Each line In lines
                    If Not String.IsNullOrEmpty(line) AndAlso Not line.StartsWith("#") Then
                        Dim p As String() = line.Split("=".ToCharArray, 2)
                        If Not _settingsCache.ContainsKey(p(0)) Then
                            _settingsCache.Add(p(0), p(1))
                        End If
                    End If
                Next
            End If
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
    Default Public Property Setting(Key As String) As String
        Get
            Dim value As String = ""
            If Settings.ContainsKey(Key) Then
                value = Settings(Key)
            End If
            Return value
        End Get
        Set(value As String)
            If Settings.ContainsKey(Key) Then
                Settings(Key) = value
            Else
                Settings.Add(Key, value)
            End If
        End Set
    End Property
    Public Shared Function GetSettings() As Settings
        Return New Settings
    End Function
End Class