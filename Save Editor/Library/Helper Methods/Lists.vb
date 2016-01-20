Public Class Lists
    Public Shared ReadOnly Property CurrentLanguage As String
        Get
            Return Settings("Language")
        End Get
    End Property
    Public Shared ReadOnly Property SkyMoves As Dictionary(Of Integer, String)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/SkyMoves.txt")
            Static _dictionaryConverted As Dictionary(Of Integer, String) = Nothing
            If _dictionaryConverted Is Nothing Then
                _dictionaryConverted = New Dictionary(Of Integer, String)
                For Each Key As String In _dictionaryOriginal.Keys
                    _dictionaryConverted.Add(Key, _dictionaryOriginal(Key))
                Next
            End If
            Return _dictionaryConverted
        End Get
    End Property
    Public Shared ReadOnly Property SkyMovesInverse As Dictionary(Of String, Integer)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/SkyMoves.txt")
            Static _dictionaryConverted As Dictionary(Of String, Integer) = Nothing
            If _dictionaryConverted Is Nothing Then
                _dictionaryConverted = New Dictionary(Of String, Integer)
                For Each Key As String In _dictionaryOriginal.Keys
                    If Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then _dictionaryConverted.Add(_dictionaryOriginal(Key), Key)
                Next
            End If
            Return _dictionaryConverted
        End Get
    End Property
    Public Shared ReadOnly Property RBMoves As Dictionary(Of Integer, String)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/RBMoves.txt")
            Static _dictionaryConverted As Dictionary(Of Integer, String) = Nothing
            If _dictionaryConverted Is Nothing Then
                _dictionaryConverted = New Dictionary(Of Integer, String)
                For Each Key As String In _dictionaryOriginal.Keys
                    _dictionaryConverted.Add(Key, _dictionaryOriginal(Key))
                Next
            End If
            Return _dictionaryConverted
        End Get
    End Property
    Public Shared ReadOnly Property RBMovesInverse As Dictionary(Of String, Integer)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/RBMoves.txt")
            Static _dictionaryConverted As Dictionary(Of String, Integer) = Nothing
            If _dictionaryConverted Is Nothing Then
                _dictionaryConverted = New Dictionary(Of String, Integer)
                For Each Key As String In _dictionaryOriginal.Keys
                    If Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then _dictionaryConverted.Add(_dictionaryOriginal(Key), Key)
                Next
            End If
            Return _dictionaryConverted
        End Get
    End Property
    Public Shared ReadOnly Property SkyItemNames As Dictionary(Of Integer, String)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/SkyItems.txt")
            Static _dictionaryConverted As Dictionary(Of Integer, String) = Nothing
            If _dictionaryConverted Is Nothing Then
                _dictionaryConverted = New Dictionary(Of Integer, String)
                For Each Key As String In _dictionaryOriginal.Keys
                    _dictionaryConverted.Add(Key, _dictionaryOriginal(Key))
                Next
            End If
            Return _dictionaryConverted
        End Get
    End Property
    Public Shared ReadOnly Property SkyItemNamesInverse As Dictionary(Of String, Integer)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/SkyItems.txt")
            Static _dictionaryConverted As Dictionary(Of String, Integer) = Nothing
            If _dictionaryConverted Is Nothing Then
                _dictionaryConverted = New Dictionary(Of String, Integer)
                For Each Key As String In _dictionaryOriginal.Keys
                    If Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then _dictionaryConverted.Add(_dictionaryOriginal(Key), Key)
                Next
            End If
            Return _dictionaryConverted
        End Get
    End Property
    Public Shared ReadOnly Property RBItemNames As Dictionary(Of Integer, String)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/RBItems.txt")
            Static _dictionaryConverted As Dictionary(Of Integer, String) = Nothing
            If _dictionaryConverted Is Nothing Then
                _dictionaryConverted = New Dictionary(Of Integer, String)
                For Each Key As String In _dictionaryOriginal.Keys
                    _dictionaryConverted.Add(Key, _dictionaryOriginal(Key))
                Next
            End If
            Return _dictionaryConverted
        End Get
    End Property
    Public Shared ReadOnly Property RBItemNamesInverse As Dictionary(Of String, Integer)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/RBItems.txt")
            Static _dictionaryConverted As Dictionary(Of String, Integer) = Nothing
            If _dictionaryConverted Is Nothing Then
                _dictionaryConverted = New Dictionary(Of String, Integer)
                For Each Key As String In _dictionaryOriginal.Keys
                    If Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then _dictionaryConverted.Add(_dictionaryOriginal(Key), Key)
                Next
            End If
            Return _dictionaryConverted
        End Get
    End Property
    Public Shared ReadOnly Property SkyPokemon As Dictionary(Of Integer, String)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/SkyPokemon.txt")
            Static _dictionaryConverted As Dictionary(Of Integer, String) = Nothing
            If _dictionaryConverted Is Nothing Then
                _dictionaryConverted = New Dictionary(Of Integer, String)
                For Each Key As String In _dictionaryOriginal.Keys
                    _dictionaryConverted.Add(Key, _dictionaryOriginal(Key))
                Next
            End If
            Return _dictionaryConverted
        End Get
    End Property
    Public Shared ReadOnly Property SkyPokemonInverse As Dictionary(Of String, Integer)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/SkyPokemon.txt")
            Static _dictionaryConverted As Dictionary(Of String, Integer) = Nothing
            If _dictionaryConverted Is Nothing Then
                _dictionaryConverted = New Dictionary(Of String, Integer)
                For Each Key As String In _dictionaryOriginal.Keys
                    If Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then _dictionaryConverted.Add(_dictionaryOriginal(Key), Key)
                Next
            End If
            Return _dictionaryConverted
        End Get
    End Property
    Public Shared ReadOnly Property RBPokemon As Dictionary(Of Integer, String)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/RBPokemon.txt")
            Static _dictionaryConverted As Dictionary(Of Integer, String) = Nothing
            If _dictionaryConverted Is Nothing Then
                _dictionaryConverted = New Dictionary(Of Integer, String)
                For Each Key As String In _dictionaryOriginal.Keys
                    _dictionaryConverted.Add(Key, _dictionaryOriginal(Key))
                Next
            End If
            Return _dictionaryConverted
        End Get
    End Property
    Public Shared ReadOnly Property RBPokemonInverse As Dictionary(Of String, Integer)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/RBPokemon.txt")
            Static _dictionaryConverted As Dictionary(Of String, Integer) = Nothing
            If _dictionaryConverted Is Nothing Then
                _dictionaryConverted = New Dictionary(Of String, Integer)
                For Each Key As String In _dictionaryOriginal.Keys
                    If Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then _dictionaryConverted.Add(_dictionaryOriginal(Key), Key)
                Next
            End If
            Return _dictionaryConverted
        End Get
    End Property
    Public Shared ReadOnly Property TDItemNames As Dictionary(Of Integer, String)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/TDItems.txt")
            Static _dictionaryConverted As Dictionary(Of Integer, String) = Nothing
            If _dictionaryConverted Is Nothing Then
                _dictionaryConverted = New Dictionary(Of Integer, String)
                For Each Key As String In _dictionaryOriginal.Keys
                    _dictionaryConverted.Add(Key, _dictionaryOriginal(Key))
                Next
            End If
            Return _dictionaryConverted
        End Get
    End Property
    Public Shared ReadOnly Property TDItemNamesInverse As Dictionary(Of String, Integer)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/TDItems.txt")
            Static _dictionaryConverted As Dictionary(Of String, Integer) = Nothing
            If _dictionaryConverted Is Nothing Then
                _dictionaryConverted = New Dictionary(Of String, Integer)
                For Each Key As String In _dictionaryOriginal.Keys
                    If Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then _dictionaryConverted.Add(_dictionaryOriginal(Key), Key)
                Next
            End If
            Return _dictionaryConverted
        End Get
    End Property
    Public Shared ReadOnly Property StringEncoding As Dictionary(Of Byte, Char)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/StringEncoding.txt")
            Static _dictionaryConverted As Dictionary(Of Byte, Char) = Nothing
            If _dictionaryConverted Is Nothing Then
                _dictionaryConverted = New Dictionary(Of Byte, Char)
                For Each Key As String In _dictionaryOriginal.Keys
                    If _dictionaryOriginal(Key).Length = 1 Then
                        _dictionaryConverted.Add(Convert.ToByte(Key, 16), _dictionaryOriginal(Key)(0))
                    End If
                Next
            End If
            Return _dictionaryConverted
        End Get
    End Property
    Public Shared ReadOnly Property StringEncodingInverse As Dictionary(Of Char, Byte)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/StringEncoding.txt")
            Static _dictionaryConverted As Dictionary(Of Char, Byte) = Nothing
            If _dictionaryConverted Is Nothing Then
                _dictionaryConverted = New Dictionary(Of Char, Byte)
                For Each Key As String In _dictionaryOriginal.Keys
                    If _dictionaryOriginal(Key).Length = 1 AndAlso Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then
                        _dictionaryConverted.Add(_dictionaryOriginal(Key), Convert.ToByte(Key, 16))
                    End If
                Next
            End If
            Return _dictionaryConverted
        End Get
    End Property
    Public Shared ReadOnly Property LanguageText As Dictionary(Of String, String)
        Get
            Static _dictionaryCache As New ResourceDictionary("&L;/Language.txt")
            Return _dictionaryCache
        End Get
    End Property
    Public Shared ReadOnly Property Settings As Dictionary(Of String, String)
        Get
            Static _dictionaryCache As New ResourceDictionary("Settings.txt")
            Return _dictionaryCache
        End Get
    End Property
End Class
Public Class Settings
    Public Shared ReadOnly Property DebugMode As Boolean
        Get
            Return Lists.Settings.ContainsKey("DebugMode") AndAlso Lists.Settings("DebugMode") = "True"
        End Get
    End Property
    Public Shared ReadOnly Property Enable255ItemCount As Boolean
        Get
            Return Lists.Settings.ContainsKey("Enable255ItemCount") AndAlso Lists.Settings.ContainsKey("Enable255ItemCount") = "True"
        End Get
    End Property
End Class
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
                Dim p As String() = line.Split("=".ToCharArray, 2)
                If Not Me.ContainsKey(FormatString(p(0))) Then
                    Me.Add(FormatString(p(0)), FormatString(p(1)))
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