Public Class Settings
    Public Shared Function DebugMode() As Boolean
        Return Lists.Settings.ContainsKey("DebugMode") AndAlso Lists.Settings("DebugMode") = "True"
    End Function
    Public Shared Function ShowConsoleOnStart() As Boolean
        Return Lists.Settings.ContainsKey("ShowConsoleOnStart") AndAlso Lists.Settings("ShowConsoleOnStart") = "True"
    End Function
    Public Shared Function TabStripPlacement() As System.Windows.Controls.Dock
        If Lists.Settings.ContainsKey("TabStripPlacement") Then
            Return Lists.Settings("TabStripPlacement")
        Else
            Return Dock.Top
        End If
    End Function
    Default Public Property Setting(Key As String) As String
        Get
            Dim value As String = ""
            If Lists.Settings.ContainsKey(Key) Then
                value = Lists.Settings(Key)
            End If
            Return value
        End Get
        Set(value As String)
            If Lists.Settings.ContainsKey(Key) Then
                Lists.Settings(Key) = value
            Else
                Lists.Settings.Add(Key, value)
            End If
        End Set
    End Property
    Public Shared Function GetSettings() As Settings
        Return New Settings
    End Function
End Class