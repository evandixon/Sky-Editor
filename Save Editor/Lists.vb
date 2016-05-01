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
    Private Shared Property RBBaseTypes As Dictionary(Of Integer, String)


    Public Shared Function GetSkyLocations() As Dictionary(Of Integer, String)
        If SkyLocations Is Nothing Then
            Dim i As New BasicDictionaryIniFile
            i.CreateFile(My.Resources.ListResources.SkyLocations)
            SkyLocations = i.Entries
        End If
        Return SkyLocations
    End Function

    Public Shared Function GetTDLocations() As Dictionary(Of Integer, String)
        If TDLocations Is Nothing Then
            Dim i As New BasicDictionaryIniFile
            i.CreateFile(My.Resources.ListResources.TDLocations)
            TDLocations = i.Entries
        End If
        Return TDLocations
    End Function

    Public Shared Function GetRBLocations() As Dictionary(Of Integer, String)
        If RBLocations Is Nothing Then
            Dim i As New BasicDictionaryIniFile
            i.CreateFile(My.Resources.ListResources.RBLocations)
            RBLocations = i.Entries
        End If
        Return RBLocations
    End Function

    Public Shared Function GetSkyMoves() As Dictionary(Of Integer, String)
        If SkyMoves Is Nothing Then
            Dim i As New BasicDictionaryIniFile
            i.CreateFile(My.Resources.ListResources.SkyMoves)
            SkyMoves = i.Entries
        End If
        Return SkyMoves
    End Function

    Public Shared Function GetRBMoves() As Dictionary(Of Integer, String)
        If RBMoves Is Nothing Then
            Dim i As New BasicDictionaryIniFile
            i.CreateFile(My.Resources.ListResources.RBMoves)
            RBMoves = i.Entries
        End If
        Return RBMoves
    End Function

    Public Shared Function GetSkyItemNames() As Dictionary(Of Integer, String)
        If SkyItems Is Nothing Then
            Dim i As New BasicDictionaryIniFile
            i.CreateFile(My.Resources.ListResources.SkyItems)
            SkyItems = i.Entries
        End If
        Return SkyItems
    End Function

    Public Shared Function GetTDItemNames() As Dictionary(Of Integer, String)
        If TDItems Is Nothing Then
            Dim i As New BasicDictionaryIniFile
            i.CreateFile(My.Resources.ListResources.TDItems)
            TDItems = i.Entries
        End If
        Return TDItems
    End Function

    Public Shared Function RBItemNames() As Dictionary(Of Integer, String)
        If RBItems Is Nothing Then
            Dim i As New BasicDictionaryIniFile
            i.CreateFile(My.Resources.ListResources.RBItems)
            RBItems = i.Entries
        End If
        Return RBItems
    End Function

    Public Shared Function GetSkyPokemon() As Dictionary(Of Integer, String)
        If SkyPokemon Is Nothing Then
            Dim i As New BasicDictionaryIniFile
            i.CreateFile(My.Resources.ListResources.SkyPokemon)
            SkyPokemon = i.Entries
        End If
        Return SkyPokemon
    End Function

    Public Shared Function GetRBPokemon() As Dictionary(Of Integer, String)
        If RBPokemon Is Nothing Then
            Dim i As New BasicDictionaryIniFile
            i.CreateFile(My.Resources.ListResources.RBPokemon)
            RBPokemon = i.Entries
        End If
        Return RBPokemon
    End Function

    Public Shared Function GetRBBaseTypes() As Dictionary(Of Integer, String)
        If RBBaseTypes Is Nothing Then
            Dim i As New BasicDictionaryIniFile
            i.CreateFile(My.Resources.ListResources.RBBaseTypes)
            RBBaseTypes = i.Entries
        End If
        Return RBBaseTypes
    End Function

End Class