Public Class Lists

    Private Shared Property SkyLocations As Dictionary(Of Integer, String)
    Public Shared ReadOnly Property TDLocations As Dictionary(Of Integer, String)
        Get
            If _tdLocations Is Nothing Then
                Dim i As New BasicDictionaryIniFile
                i.CreateFile(My.Resources.ListResources.TDLocations)
                _tdLocations = i.Entries
            End If
            Return _tdLocations
        End Get
    End Property
    Private Shared _tdLocations As Dictionary(Of Integer, String)

    Public Shared ReadOnly Property RBLocations As Dictionary(Of Integer, String)
        Get
            If _rbLocations Is Nothing Then
                Dim i As New BasicDictionaryIniFile
                i.CreateFile(My.Resources.ListResources.RBLocations)
                _rbLocations = i.Entries
            End If
            Return _rbLocations
        End Get
    End Property
    Private Shared _rbLocations As Dictionary(Of Integer, String)

    Public Shared ReadOnly Property ExplorersMoves As Dictionary(Of Integer, String)
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

    Public Shared ReadOnly Property RBMoves As Dictionary(Of Integer, String)
        Get
            If _rbMoves Is Nothing Then
                Dim i As New BasicDictionaryIniFile
                i.CreateFile(My.Resources.ListResources.RBMoves)
                _rbMoves = i.Entries
            End If
            Return _rbMoves
        End Get
    End Property
    Private Shared _rbMoves As Dictionary(Of Integer, String)

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

    Public Shared ReadOnly Property ExplorersPokemon As Dictionary(Of Integer, String)
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

    Public Shared ReadOnly Property RBPokemon As Dictionary(Of Integer, String)
        Get
            If _rbPokemon Is Nothing Then
                Dim i As New BasicDictionaryIniFile
                i.CreateFile(My.Resources.ListResources.RBPokemon)
                _rbPokemon = i.Entries
            End If
            Return _rbPokemon
        End Get
    End Property
    Private Shared _rbPokemon As Dictionary(Of Integer, String)

    Public Shared ReadOnly Property RBBaseTypes As Dictionary(Of Integer, String)
        Get
            If _rbBaseTypes Is Nothing Then
                Dim i As New BasicDictionaryIniFile
                i.CreateFile(My.Resources.ListResources.RBBaseTypes)
                _rbBaseTypes = i.Entries
            End If
            Return _rbBaseTypes
        End Get
    End Property
    Private Shared _rbBaseTypes As Dictionary(Of Integer, String)


    Public Shared Function GetSkyLocations() As Dictionary(Of Integer, String)
        If SkyLocations Is Nothing Then
            Dim i As New BasicDictionaryIniFile
            i.CreateFile(My.Resources.ListResources.SkyLocations)
            SkyLocations = i.Entries
        End If
        Return SkyLocations
    End Function

End Class