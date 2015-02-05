Imports System.Reflection
Imports System.Text.RegularExpressions

Namespace Internal
    Friend Class LanguageManager
        Friend Class LanguageItem
            Public Property Key As String
            Public Property Value As String
            Public Property PluginName As String
            Public Property Accessed As Boolean
            Public Sub New(Key As String, Value As String, PluginName As String, Accessed As Boolean)
                Me.Key = Key
                Me.Value = Value
                Me.PluginName = PluginName
                Me.Accessed = Accessed
            End Sub
            Public Overrides Function ToString() As String
                Return Value
            End Function
        End Class
        Friend _LanguageDictionary As Dictionary(Of String, LanguageItem)
        Friend _LanguageDictionaryEnglish As Dictionary(Of String, LanguageItem)

        Private Sub EnsureLanguageLoaded()
            If _LanguageDictionary Is Nothing Then
                _LanguageDictionary = New Dictionary(Of String, LanguageItem)
                Dim language As String = Settings.CurrentLanguage
                Dim files As New Dictionary(Of String, String)
                files.Add(IO.Path.Combine(Environment.CurrentDirectory, String.Format("Resources/{0}/Language.txt", language)), "{Default}")
                For Each directory In IO.Directory.GetDirectories(IO.Path.Combine(Environment.CurrentDirectory, "Resources", language), "*", IO.SearchOption.TopDirectoryOnly)
                    files.Add(IO.Path.Combine(directory, "Language.txt"), IO.Path.GetDirectoryName(directory))
                Next
                For Each file In files
                    If IO.File.Exists(file.Key) Then
                        Dim lines = IO.File.ReadAllLines(file.Key)
                        For Each Line In lines
                            Dim parts As String() = Line.Split("=".ToCharArray, 2)
                            If parts.Count = 2 Then
                                If Not _LanguageDictionary.Keys.Contains(parts(0)) Then
                                    _LanguageDictionary.Add(parts(0), New LanguageItem(parts(0), parts(1), file.Value, False))
                                End If
                            End If
                        Next
                    End If
                Next
            End If
        End Sub
        Private Sub EnsureEnglishLanguageLoaded()
            If _LanguageDictionaryEnglish Is Nothing Then
                _LanguageDictionaryEnglish = New Dictionary(Of String, LanguageItem)
                Dim files As New Dictionary(Of String, String)
                files.Add(IO.Path.Combine(Environment.CurrentDirectory, String.Format("Resources/{0}/Language.txt", "English")), "{Default}")
                For Each directory In IO.Directory.GetDirectories(IO.Path.Combine(Environment.CurrentDirectory, "Resources"), "*", IO.SearchOption.TopDirectoryOnly)
                    files.Add(IO.Path.Combine(directory, "English/Language.txt"), IO.Path.GetDirectoryName(directory))
                Next
                For Each file In files
                    If IO.File.Exists(file.Key) Then
                        Dim lines = IO.File.ReadAllLines(file.Key)
                        For Each Line In lines
                            Dim parts As String() = Line.Split("=".ToCharArray, 2)
                            If parts.Count = 2 Then
                                If Not _LanguageDictionaryEnglish.Keys.Contains(parts(0)) Then
                                    _LanguageDictionaryEnglish.Add(parts(0), New LanguageItem(parts(0), parts(1), file.Value, False))
                                End If
                            End If
                        Next
                    End If
                Next
            End If
        End Sub
        Public Shared Function GetLanguageItem(Key As String, AssemblyName As String, Optional DefaultValue As String = Nothing) As String
            If Key.Contains("=") Then Key = Key.Replace("=", "_")
            Instance.EnsureLanguageLoaded()
            If Instance._LanguageDictionary.ContainsKey(Key) Then
                Instance._LanguageDictionary(Key).Accessed = True
                Return FormatString(Instance._LanguageDictionary(Key).Value)
            Else
                Instance.EnsureEnglishLanguageLoaded()
                If Instance._LanguageDictionaryEnglish.ContainsKey(Key) Then
                    DeveloperConsole.Writeline(String.Format("Could not find key ""{0}"" in language ""{1}"" in assembly ""{2}""", Key, Settings.CurrentLanguage, AssemblyName))
                    DeveloperConsole.Writeline("English value substituted.")
                    Instance._LanguageDictionaryEnglish(Key).Accessed = True
                    Return FormatString(Instance._LanguageDictionaryEnglish(Key).Value)
                Else
                    If DefaultValue Is Nothing Then DefaultValue = Key
                    DeveloperConsole.Writeline(String.Format("Could not find key ""{0}"" in language ""{1}"" in assembly ""{2}""", Key, Settings.CurrentLanguage, AssemblyName))
                    DeveloperConsole.Writeline("Default value used.")
                    Instance._LanguageDictionaryEnglish.Add(Key, New LanguageItem(Key, DefaultValue, AssemblyName, True))
                    Dim filename As String
                    If AssemblyName = IO.Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs(0)).Replace(".vshost", "") Then
                        'It's not a plugin, it's this assembly (or something external that's not a plugin, which means something is using Sky Editor Base itself), so don't put it in the Plugin Directory
                        filename = String.Format("Resources/{0}/Language.txt", "English")
                    Else
                        filename = String.Format("Resources/{1}/{0}/language.txt", AssemblyName.Replace("_plg", ""), "English")
                    End If

                    Dim fileContents As String
                    If IO.File.Exists(filename) Then
                        fileContents = IO.File.ReadAllText(filename)
                    Else
                        fileContents = ""
                    End If

                    fileContents &= vbCrLf & Key & "=" & DefaultValue

                    If Not IO.Directory.Exists(IO.Path.GetDirectoryName(filename)) Then
                        IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(filename))
                    End If
                    IO.File.WriteAllText(filename, fileContents.Trim)
                    DeveloperConsole.Writeline("Default value written to language file " & filename)
                    Return FormatString(DefaultValue)
                End If
            End If
        End Function
        Private Shared Function FormatString(Input As String) As String
            Dim nl As New Regex("(?<!\\)\\n")
            Input = nl.Replace(Input, vbCrLf)
            Input = Input.Replace("\\", "\")
            Return Input
        End Function
        Private Shared _manager As LanguageManager
        Public Shared ReadOnly Property Instance As LanguageManager
            Get
                If _manager Is Nothing Then
                    _manager = New LanguageManager
                End If
                Return _manager
            End Get
        End Property
        Private Sub New()
            MyBase.New()
        End Sub
    End Class
End Namespace
