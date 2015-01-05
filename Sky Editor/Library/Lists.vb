Imports SkyEditorBase
Public Class Lists
    Inherits SkyEditorBase.Lists
    Friend Const SubDirectory As String = "SkyEditor"
    Public Shared ReadOnly Property SkyEditorLanguageText As Dictionary(Of String, String)
        Get
            Static _dictionaryCache As New ResourceDictionary("&L;/" & SubDirectory & "/Language.txt")
            Return _dictionaryCache
        End Get
    End Property
    Public Shared ReadOnly Property SkyLocations As Dictionary(Of Integer, String)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/SkyLocations.txt")
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
    Public Shared ReadOnly Property SkyLocationsInverse As Dictionary(Of String, Integer)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/SkyLocations.txt")
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
    Public Shared ReadOnly Property TDLocations As Dictionary(Of Integer, String)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/TDLocations.txt")
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
    Public Shared ReadOnly Property TDLocationsInverse As Dictionary(Of String, Integer)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/TDLocations.txt")
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
    Public Shared ReadOnly Property SkyMoves As Dictionary(Of Integer, String)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/SkyMoves.txt")
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
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/SkyMoves.txt")
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
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/RBMoves.txt")
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
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/RBMoves.txt")
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
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/SkyItems.txt")
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
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/SkyItems.txt")
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
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/RBItems.txt")
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
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/RBItems.txt")
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
    Public Shared ReadOnly Property RBBaseTypes As Dictionary(Of String, Integer)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/RBBaseTypes.txt")
            Static _dictionaryConverted As Dictionary(Of String, Integer) = Nothing
            If _dictionaryConverted Is Nothing Then
                _dictionaryConverted = New Dictionary(Of String, Integer)
                For Each Key As String In _dictionaryOriginal.Keys
                    _dictionaryConverted.Add(Key, _dictionaryOriginal(Key))
                Next
            End If
            Return _dictionaryConverted
        End Get
    End Property
    Public Shared ReadOnly Property RBBaseTypesInverse As Dictionary(Of Integer, String)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/RBBaseTypes.txt")
            Static _dictionaryConverted As Dictionary(Of Integer, String) = Nothing
            If _dictionaryConverted Is Nothing Then
                _dictionaryConverted = New Dictionary(Of Integer, String)
                For Each Key As String In _dictionaryOriginal.Keys
                    If Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then _dictionaryConverted.Add(_dictionaryOriginal(Key), Key)
                Next
            End If
            Return _dictionaryConverted
        End Get
    End Property
    Public Shared ReadOnly Property SkyPokemon As Dictionary(Of Integer, String)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/SkyPokemon.txt")
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
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/SkyPokemon.txt")
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
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/RBPokemon.txt")
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
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/RBPokemon.txt")
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
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/TDItems.txt")
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
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/TDItems.txt")
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
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/StringEncoding.txt")
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
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")> Public Shared ReadOnly Property StringEncodingInverse As Dictionary(Of Char, Byte)
        Get
            Static _dictionaryOriginal As New ResourceDictionary("&L;/" & SubDirectory & "/StringEncoding.txt")
            Static _dictionaryConverted As Dictionary(Of Char, Byte) = Nothing
            If _dictionaryConverted Is Nothing Then
                _dictionaryConverted = New Dictionary(Of Char, Byte)
                For Each Key As String In _dictionaryOriginal.Keys
                    If _dictionaryOriginal(Key).Length = 1 AndAlso Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then
                        _dictionaryConverted.Add(_dictionaryOriginal(Key), Convert.ToByte(Key, 16))
                    End If
                Next
                _dictionaryConverted.Add(vbNullChar, 0)
            End If
            Return _dictionaryConverted
        End Get
    End Property
End Class
Public Class Settings
    Inherits SkyEditorBase.Settings
    Public Shared ReadOnly Property Enable255ItemCount As Boolean
        Get
            Return Lists.Settings.ContainsKey("Enable255ItemCount") AndAlso Lists.Settings.ContainsKey("Enable255ItemCount") = "True"
        End Get
    End Property
End Class
Public Class StringUtilities
    Public Shared Function StringToPMDEncoding(Input As String) As Byte()
        Dim out As New List(Of Byte)
        For Each c As Char In Input
            Try
                out.Add(Lists.StringEncodingInverse(c))
            Catch ex As Exception
                ExceptionManager.LogException(ex, "StringUtilities.StringToPMDEncoding(""" & c & """)")
            End Try
        Next
        Return out.ToArray
    End Function
    Public Shared Function PMDEncodingToString(Input As Byte()) As String
        Dim out As String = ""
        For Each b In Input
            If b > 0 Then
                If Lists.StringEncoding.Keys.Contains(b) Then
                    out = out & Lists.StringEncoding(b)
                Else
                    out = out & "[" & b.ToString & "]"
                End If
            Else
                Exit For
            End If
        Next
        Return out
    End Function
End Class