Partial Public Class Lists
    Public Shared ReadOnly Property CurrentLanguage As String
        Get
            Return Settings("Language")
        End Get
    End Property
    Public Shared ReadOnly Property LanguageText As Dictionary(Of String, String)
        Get
            Static _dictionaryCache As New ResourceDictionary("&L;/Language.txt")
            Return _dictionaryCache
        End Get
    End Property
    Public Shared Property Settings As Dictionary(Of String, String)
        Get
            Static _dictionaryCache As New ResourceDictionary("Settings.txt")
            Return _dictionaryCache
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
Public Class Settings
    Public Shared ReadOnly Property DebugMode As Boolean
        Get
            Return Lists.Settings.ContainsKey("DebugMode") AndAlso Lists.Settings("DebugMode") = "True"
        End Get
    End Property
    Public Shared ReadOnly Property ShowConsoleOnStart As Boolean
        Get
            Return Lists.Settings.ContainsKey("ShowConsoleOnStart") AndAlso Lists.Settings("ShowConsoleOnStart") = "True"
        End Get
    End Property
    Public Shared ReadOnly Property TabStripPlacement As Windows.Controls.Dock
        Get
            If Lists.Settings.ContainsKey("TabStripPlacement") Then
                Return Lists.Settings("TabStripPlacement")
            Else
                Return Dock.Top
            End If
        End Get
    End Property
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
<Serializable()>
Public Class ResourceDictionary
    Inherits Dictionary(Of String, String)
    Public Sub New(ResourceName As String)
        MyBase.New()
        If ResourceName.Contains("&L;") Then
            ResourceName = ResourceName.Replace("&L;", Lists.CurrentLanguage)
        End If
        If IO.File.Exists(IO.Path.Combine(Environment.CurrentDirectory, "Resources\" & ResourceName)) Then
            Dim lines As String() = IO.File.ReadAllLines(IO.Path.Combine(Environment.CurrentDirectory, "Resources\" & ResourceName))
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
    Shared Function FormatString(Input As String) As String
        Return Input.Replace("<br/>", vbCrLf)
    End Function
End Class
Public Class GenericListItem(Of T)
    Implements IComparable

    Public Property Text As String
    Public Property Value As T
    Public Sub New(Text As String, Value As T)
        Me.Text = Text
        Me.Value = Value
    End Sub
    Public Overrides Function ToString() As String
        Return Text
    End Function
    Public Overrides Function Equals(obj As Object) As Boolean
        If TypeOf obj Is GenericListItem(Of T) Then
            Return DirectCast(obj, GenericListItem(Of T)).Text = Me.Text
        Else
            Return False
        End If
    End Function

    Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
        If TypeOf obj Is GenericListItem(Of T) Then
            Return Me.Text.CompareTo(DirectCast(obj, GenericListItem(Of T)).Text)
        Else
            Return 0
        End If
    End Function
End Class