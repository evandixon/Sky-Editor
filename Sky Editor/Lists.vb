Imports SkyEditorBase.Utilities
Public Class Lists
    Public Shared Function SkyLocations() As Dictionary(Of Integer, String)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/SkyLocations.txt")
        Static _dictionaryConverted As Dictionary(Of Integer, String) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of Integer, String)
            For Each Key As String In _dictionaryOriginal.Keys
                _dictionaryConverted.Add(Key, _dictionaryOriginal(Key))
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function SkyLocationsInverse() As Dictionary(Of String, Integer)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/SkyLocations.txt")
        Static _dictionaryConverted As Dictionary(Of String, Integer) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of String, Integer)
            For Each Key As String In _dictionaryOriginal.Keys
                If Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then _dictionaryConverted.Add(_dictionaryOriginal(Key), Key)
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function TDLocations() As Dictionary(Of Integer, String)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/TDLocations.txt")
        Static _dictionaryConverted As Dictionary(Of Integer, String) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of Integer, String)
            For Each Key As String In _dictionaryOriginal.Keys
                _dictionaryConverted.Add(Key, _dictionaryOriginal(Key))
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function TDLocationsInverse() As Dictionary(Of String, Integer)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/RBLocations.txt")
        Static _dictionaryConverted As Dictionary(Of String, Integer) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of String, Integer)
            For Each Key As String In _dictionaryOriginal.Keys
                If Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then _dictionaryConverted.Add(_dictionaryOriginal(Key), Key)
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function RBLocations() As Dictionary(Of Integer, String)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/RBLocations.txt")
        Static _dictionaryConverted As Dictionary(Of Integer, String) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of Integer, String)
            For Each Key As String In _dictionaryOriginal.Keys
                _dictionaryConverted.Add(Key, _dictionaryOriginal(Key))
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function SkyMoves() As Dictionary(Of Integer, String)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/SkyMoves.txt")
        Static _dictionaryConverted As Dictionary(Of Integer, String) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of Integer, String)
            For Each Key As String In _dictionaryOriginal.Keys
                _dictionaryConverted.Add(Key, _dictionaryOriginal(Key))
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function SkyMovesInverse() As Dictionary(Of String, Integer)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/SkyMoves.txt")
        Static _dictionaryConverted As Dictionary(Of String, Integer) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of String, Integer)
            For Each Key As String In _dictionaryOriginal.Keys
                If Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then _dictionaryConverted.Add(_dictionaryOriginal(Key), Key)
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function RBMoves() As Dictionary(Of Integer, String)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/RBMoves.txt")
        Static _dictionaryConverted As Dictionary(Of Integer, String) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of Integer, String)
            For Each Key As String In _dictionaryOriginal.Keys
                _dictionaryConverted.Add(Key, _dictionaryOriginal(Key))
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function RBMovesInverse() As Dictionary(Of String, Integer)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/RBMoves.txt")
        Static _dictionaryConverted As Dictionary(Of String, Integer) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of String, Integer)
            For Each Key As String In _dictionaryOriginal.Keys
                If Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then _dictionaryConverted.Add(_dictionaryOriginal(Key), Key)
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function SkyItemNames() As Dictionary(Of Integer, String)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/SkyItems.txt")
        Static _dictionaryConverted As Dictionary(Of Integer, String) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of Integer, String)
            For Each Key As String In _dictionaryOriginal.Keys
                _dictionaryConverted.Add(Key, _dictionaryOriginal(Key))
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function SkyItemNamesInverse() As Dictionary(Of String, Integer)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/SkyItems.txt")
        Static _dictionaryConverted As Dictionary(Of String, Integer) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of String, Integer)
            For Each Key As String In _dictionaryOriginal.Keys
                If Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then _dictionaryConverted.Add(_dictionaryOriginal(Key), Key)
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function RBItemNames() As Dictionary(Of Integer, String)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/RBItems.txt")
        Static _dictionaryConverted As Dictionary(Of Integer, String) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of Integer, String)
            For Each Key As String In _dictionaryOriginal.Keys
                _dictionaryConverted.Add(Key, _dictionaryOriginal(Key))
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function RBItemNamesInverse() As Dictionary(Of String, Integer)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/RBItems.txt")
        Static _dictionaryConverted As Dictionary(Of String, Integer) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of String, Integer)
            For Each Key As String In _dictionaryOriginal.Keys
                If Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then _dictionaryConverted.Add(_dictionaryOriginal(Key), Key)
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function RBBaseTypes() As Dictionary(Of String, Integer)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/RBBaseTypes.txt")
        Static _dictionaryConverted As Dictionary(Of String, Integer) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of String, Integer)
            For Each Key As String In _dictionaryOriginal.Keys
                _dictionaryConverted.Add(Key, _dictionaryOriginal(Key))
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function RBBaseTypesInverse() As Dictionary(Of Integer, String)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/RBBaseTypes.txt")
        Static _dictionaryConverted As Dictionary(Of Integer, String) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of Integer, String)
            For Each Key As String In _dictionaryOriginal.Keys
                If Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then _dictionaryConverted.Add(_dictionaryOriginal(Key), Key)
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function SkyPokemon() As Dictionary(Of Integer, String)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/SkyPokemon.txt")
        Static _dictionaryConverted As Dictionary(Of Integer, String) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of Integer, String)
            For Each Key As String In _dictionaryOriginal.Keys
                _dictionaryConverted.Add(Key, _dictionaryOriginal(Key))
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function SkyPokemonInverse() As Dictionary(Of String, Integer)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/SkyPokemon.txt")
        Static _dictionaryConverted As Dictionary(Of String, Integer) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of String, Integer)
            For Each Key As String In _dictionaryOriginal.Keys
                If Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then _dictionaryConverted.Add(_dictionaryOriginal(Key), Key)
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function RBPokemon() As Dictionary(Of Integer, String)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/RBPokemon.txt")
        Static _dictionaryConverted As Dictionary(Of Integer, String) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of Integer, String)
            For Each Key As String In _dictionaryOriginal.Keys
                _dictionaryConverted.Add(Key, _dictionaryOriginal(Key))
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function RBPokemonInverse() As Dictionary(Of String, Integer)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/RBPokemon.txt")
        Static _dictionaryConverted As Dictionary(Of String, Integer) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of String, Integer)
            For Each Key As String In _dictionaryOriginal.Keys
                If Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then _dictionaryConverted.Add(_dictionaryOriginal(Key), Key)
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function TDItemNames() As Dictionary(Of Integer, String)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/TDItems.txt")
        Static _dictionaryConverted As Dictionary(Of Integer, String) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of Integer, String)
            For Each Key As String In _dictionaryOriginal.Keys
                _dictionaryConverted.Add(Key, _dictionaryOriginal(Key))
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function TDItemNamesInverse() As Dictionary(Of String, Integer)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/TDItems.txt")
        Static _dictionaryConverted As Dictionary(Of String, Integer) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of String, Integer)
            For Each Key As String In _dictionaryOriginal.Keys
                If Not _dictionaryConverted.ContainsKey(_dictionaryOriginal(Key)) Then _dictionaryConverted.Add(_dictionaryOriginal(Key), Key)
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function StringEncoding() As Dictionary(Of Byte, Char)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/StringEncoding.txt")
        Static _dictionaryConverted As Dictionary(Of Byte, Char) = Nothing
        If _dictionaryConverted Is Nothing Then
            _dictionaryConverted = New Dictionary(Of Byte, Char)
            For Each Key As String In _dictionaryOriginal.Keys
                If _dictionaryOriginal(Key).Length <= 1 Then
                    _dictionaryConverted.Add(Convert.ToByte(Key, 16), _dictionaryOriginal(Key))
                End If
            Next
        End If
        Return _dictionaryConverted
    End Function
    Public Shared Function StringEncodingInverse() As Dictionary(Of Char, Byte)
        Static _dictionaryOriginal As New ResourceDictionary("&L;/StringEncoding.txt")
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
    End Function
End Class