Imports System.Reflection
Imports System.Text.RegularExpressions
Imports SkyEditorBase.Interfaces

Namespace Language
    Public Class LanguageManager
        Implements iNamed
        Implements iSavable
        Implements iModifiable

#Region "Constructor"
        Private Shared _manager As LanguageManager
        Public Event FileSaved(sender As Object, e As EventArgs) Implements iSavable.FileSaved
        Public Event Modified(sender As Object, e As EventArgs) Implements iModifiable.Modified

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
            Languages = New Dictionary(Of String, LanguageFile)
        End Sub

#End Region

        Public Sub ImportFile(Filename As String)
            Dim f As New ObjectFile(Of Dictionary(Of String, LanguageFile))(Filename)
            For Each lang In f.ContainedObject.Keys
                EnsureLanguageLoaded(lang)
                For Each item In f.ContainedObject(lang).ContainedObject
                    AddLanguageItem(lang, item)
                Next
            Next
        End Sub

#Region "iSavable support"
        Public Sub SaveAll() Implements iSavable.Save
            For Each item In Languages.Values
                item.Save()
            Next
            RaiseEvent FileSaved(Me, New EventArgs)
        End Sub

        Public Sub SaveAll(Filename As String) Implements iSavable.Save
            Dim f As New ObjectFile(Of Dictionary(Of String, LanguageFile))
            f.ContainedObject = Languages
            f.Save(Filename)
            RaiseEvent FileSaved(Me, New EventArgs)
        End Sub
        Public Function DefaultExtension() As String Implements iSavable.DefaultExtension
            Return ".skylangpack"
        End Function
#End Region

        Public Property Languages As Dictionary(Of String, LanguageFile)

        Public Property AdditionsMade As Boolean

        Public Property Name As String Implements iNamed.Name
            Get
                Return GetLanguageItem("Language Manager", Assembly.GetExecutingAssembly.GetName.Name)
            End Get
            Set(value As String)

            End Set
        End Property

        Public Sub LoadAllLanguages()
            Dim languageDir = IO.Path.Combine(PluginHelper.RootResourceDirectory, "Languages")
            For Each item In IO.Directory.GetFiles(languageDir, "*.json", IO.SearchOption.TopDirectoryOnly)
                EnsureLanguageLoaded(IO.Path.GetFileNameWithoutExtension(item))
            Next
        End Sub

        Public Sub EnsureLanguageLoaded(Language As String)
            If Not Languages.ContainsKey(Language) Then
                Dim languageDir = IO.Path.Combine(PluginHelper.RootResourceDirectory, "Languages")
                If Not IO.Directory.Exists(languageDir) Then
                    IO.Directory.CreateDirectory(languageDir)
                End If
                Languages.Add(Language, New LanguageFile(IO.Path.Combine(languageDir, String.Format("{0}.json", Language))))
            End If
        End Sub

        Public Sub EnsureCurrentLanguageLoaded()
            EnsureLanguageLoaded(SettingsManager.Instance.Settings.CurrentLanguage)
        End Sub

        Public Sub EnsureDefaultLanguageLoaded()
            EnsureLanguageLoaded(SettingsManager.Instance.Settings.DefaultLanguage)
        End Sub

        Public Sub AddLanguageItem(Language As String, Item As LanguageItem)
            EnsureLanguageLoaded(Language)
            If Not Languages(Language).ContainedObject.Contains(Item) Then
                Languages(Language).ContainedObject.Add(Item)
                Instance.AdditionsMade = True
            End If
        End Sub

        Private Function SearchLanguageItem(Language As String, Key As String, AssemblyName As String) As LanguageItem
            If Languages.ContainsKey(Language) Then
                Dim q = From l In Languages(Language).ContainedObject Where l.Key = Key AndAlso l.PluginName = AssemblyName Select l

                If q.Any Then
                    Return q.First
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' Gets the language item from the given key.
        ''' </summary>
        ''' <param name="Key">Key used to find the language item.</param>
        ''' <param name="AssemblyName">Assembly name that needs the language item.</param>
        ''' <param name="DefaultValue">English value to use if the language item is not found.</param>
        ''' <returns></returns>
        Public Shared Function GetLanguageItem(Key As String, AssemblyName As String, Optional DefaultValue As String = Nothing) As String
            Dim language = SettingsManager.Instance.Settings.CurrentLanguage
            'Get rid of invalid characters
            If Key.Contains("=") Then Key = Key.Replace("=", "_")

            'Ensure the language is actually loaded
            Instance.EnsureCurrentLanguageLoaded()

            'Check for the language item
            Dim wantedItem = Instance.SearchLanguageItem(language, Key, AssemblyName)
            If wantedItem IsNot Nothing Then
                wantedItem.Accessed = True
                Return FormatString(wantedItem.Value)
            Else
                'If it doesn't exist, we'll try the value in the Default language (which is probably English).
                Dim defaultLanguage = SettingsManager.Instance.Settings.DefaultLanguage
                Instance.EnsureDefaultLanguageLoaded()
                Dim defaultItem = Instance.SearchLanguageItem(defaultLanguage, Key, AssemblyName)
                If defaultItem IsNot Nothing Then
                    defaultItem.Accessed = True
                    Return FormatString(defaultItem.Value)
                Else
                    'If the English value doesn't exist, we'll use the default value

                    'If no default value is given, we'll use the key.
                    If DefaultValue Is Nothing Then
                        DefaultValue = Key
                    End If

                    'Let's add the default language item to the current language, so it'll be easier to use a tool to translate.
                    Dim newItem As New LanguageItem(Key, DefaultValue, AssemblyName, True)
                    Instance.AddLanguageItem(defaultLanguage, newItem)

                    Return FormatString(DefaultValue)
                End If
            End If
        End Function

        ''' <summary>
        ''' Imports language items from the given file.
        ''' Input file format requires the key to be on the left of an equals sign (=) and the value to be on the right; one language item per line.
        ''' </summary>
        ''' <param name="Filename">Filename of the language file.</param>
        ''' <param name="PluginName">Plugin that the language file corresponds to.</param>
        Public Shared Sub ImportLanguageFile(Filename As String, PluginName As String)
            Dim lines = IO.File.ReadAllLines(Filename)
            Dim language = IO.Path.GetFileNameWithoutExtension(Filename)
            Instance.EnsureLanguageLoaded(language)
            For Each Line In lines
                Dim parts As String() = Line.Split("=".ToCharArray, 2)
                If parts.Count = 2 Then
                    Instance.AddLanguageItem(language, New LanguageItem(parts(0), parts(1), PluginName, False))
                End If
            Next
            Instance.Languages(language).Save()
        End Sub
        Private Shared Function FormatString(Input As String) As String
            Dim nl As New Regex("(?<!\\)\\n")
            Input = nl.Replace(Input, vbCrLf)
            Input = Input.Replace("\\", "\")
            Return Input
        End Function



        Public Sub RaiseModified() Implements iModifiable.RaiseModified
            RaiseEvent Modified(Me, New EventArgs)
        End Sub
    End Class
End Namespace