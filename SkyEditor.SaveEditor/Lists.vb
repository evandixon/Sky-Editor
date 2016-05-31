Public Class Lists

    Private Shared Property SkyLocations As Dictionary(Of Integer, String)
    Private Shared Property TDLocations As Dictionary(Of Integer, String)
    Private Shared Property RBLocations As Dictionary(Of Integer, String)
    Public Shared ReadOnly Property SkyMoves As Dictionary(Of Integer, String)
        Get
            If _skyMoves Is Nothing Then
                Dim i As New BasicDictionaryIniFile
                i.CreateFile(My.Resources.ListResources.SkyMoves)
                _skyMoves = i.Entries
            End If
            Return _skyMoves
        End Get
    End Property
    Private Shared _skyMoves As Dictionary(Of Integer, String)

    Private Shared Property TDMoves As Dictionary(Of Integer, String)
    Private Shared Property RBMoves As Dictionary(Of Integer, String)

    Public Shared ReadOnly Property SkyItems As Dictionary(Of Integer, String)
        Get
            If _skyItems Is Nothing Then
                Dim i As New BasicDictionaryIniFile
                i.CreateFile(My.Resources.ListResources.SkyItems)
                _skyItems = i.Entries
            End If
            Return _skyItems
        End Get
    End Property
    Private Shared _skyItems As Dictionary(Of Integer, String)

    Public Shared ReadOnly Property TDItems As Dictionary(Of Integer, String)
        Get
            If _tdItems Is Nothing Then
                Dim i As New BasicDictionaryIniFile
                i.CreateFile(My.Resources.ListResources.TDItems)
                _tdItems = i.Entries
            End If
            Return _tdItems
        End Get
    End Property
    Private Shared _tdItems As Dictionary(Of Integer, String)

    Public Shared ReadOnly Property RBItems As Dictionary(Of Integer, String)
        Get
            If _rbItems Is Nothing Then
                Dim i As New BasicDictionaryIniFile
                i.CreateFile(My.Resources.ListResources.RBItems)
                _rbItems = i.Entries
            End If
            Return _rbItems
        End Get
    End Property
    Private Shared _rbItems As Dictionary(Of Integer, String)

    Public Shared ReadOnly Property SkyPokemon As Dictionary(Of Integer, String)
        Get
            If _skyPokemon Is Nothing Then
                Dim i As New BasicDictionaryIniFile
                i.CreateFile(My.Resources.ListResources.SkyPokemon)
                _skyPokemon = i.Entries
            End If
            Return _skyPokemon
        End Get
    End Property
    Private Shared _skyPokemon As Dictionary(Of Integer, String)

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

    Public Shared Function GetRBMoves() As Dictionary(Of Integer, String)
        If RBMoves Is Nothing Then
            Dim i As New BasicDictionaryIniFile
            i.CreateFile(My.Resources.ListResources.RBMoves)
            RBMoves = i.Entries
        End If
        Return RBMoves
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