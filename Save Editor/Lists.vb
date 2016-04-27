Imports SkyEditorBase.Utilities
Public Class Lists

    Private Shared Property SkyLocations As Dictionary(Of Integer, String)
    Private Shared Property TDLocations As Dictionary(Of Integer, String)
    Private Shared Property RBLocations As Dictionary(Of Integer, String)
    Private Shared Property SkyMoves As Dictionary(Of Integer, String)
    Private Shared Property TDMoves As Dictionary(Of Integer, String)
    Private Shared Property RBMoves As Dictionary(Of Integer, String)
    Private Shared Property SkyItems As Dictionary(Of Integer, String)
    Private Shared Property TDItems As Dictionary(Of Integer, String)
    Private Shared Property RBItems As Dictionary(Of Integer, String)
    Private Shared Property SkyPokemon As Dictionary(Of Integer, String)
    Private Shared Property TDPokemon As Dictionary(Of Integer, String)
    Private Shared Property RBPokemon As Dictionary(Of Integer, String)


    <Obsolete> Public Shared Function GetSkyLocations() As Dictionary(Of Integer, String)
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
    <Obsolete> Public Shared Function GetSkyLocationsInverse() As Dictionary(Of String, Integer)
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
    <Obsolete> Public Shared Function GetTDLocations() As Dictionary(Of Integer, String)
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
    <Obsolete> Public Shared Function GetTDLocationsInverse() As Dictionary(Of String, Integer)
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
    <Obsolete> Public Shared Function GetRBLocations() As Dictionary(Of Integer, String)
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
    <Obsolete> Public Shared Function GetSkyMoves() As Dictionary(Of Integer, String)
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
    <Obsolete> Public Shared Function GetSkyMovesInverse() As Dictionary(Of String, Integer)
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
    <Obsolete> Public Shared Function GetRBMoves() As Dictionary(Of Integer, String)
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
    <Obsolete> Public Shared Function GetRBMovesInverse() As Dictionary(Of String, Integer)
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
    <Obsolete> Public Shared Function GetSkyItemNames() As Dictionary(Of Integer, String)
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
    <Obsolete> Public Shared Function SkyItemNamesInverse() As Dictionary(Of String, Integer)
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
    <Obsolete> Public Shared Function RBItemNames() As Dictionary(Of Integer, String)
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
    <Obsolete> Public Shared Function GetRBItemNamesInverse() As Dictionary(Of String, Integer)
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
    <Obsolete> Public Shared Function GetRBBaseTypes() As Dictionary(Of String, Integer)
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
    <Obsolete> Public Shared Function GetRBBaseTypesInverse() As Dictionary(Of Integer, String)
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
    <Obsolete> Public Shared Function GetSkyPokemon() As Dictionary(Of Integer, String)
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
    <Obsolete> Public Shared Function GetSkyPokemonInverse() As Dictionary(Of String, Integer)
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
    <Obsolete> Public Shared Function GetRBPokemon() As Dictionary(Of Integer, String)
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
    <Obsolete> Public Shared Function GetRBPokemonInverse() As Dictionary(Of String, Integer)
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
    <Obsolete> Public Shared Function GetTDItemNames() As Dictionary(Of Integer, String)
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
    <Obsolete> Public Shared Function GetTDItemNamesInverse() As Dictionary(Of String, Integer)
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
End Class