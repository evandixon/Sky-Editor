Public MustInherit Class ObjectTab
    Inherits ObjectControl
    ''' <summary>
    ''' The order in which tabs update their saves.  Higher values update first, lower values update later.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable ReadOnly Property UpdatePriority As Integer
        Get
            Return 0
        End Get
    End Property

    Dim _header As String
    Public Property Header As String
        Get
            If ParentTabItem Is Nothing Then
                Return _header
            Else
                Return ParentTabItem.Header
            End If
        End Get
        Set(value As String)
            If ParentTabItem Is Nothing Then
                _header = value
            Else
                ParentTabItem.Header = value
            End If
        End Set
    End Property

    Dim _parentTabItem As TabItem
    Public Property ParentTabItem As TabItem
        Get
            Return _parentTabItem
        End Get
        Set(value As TabItem)
            _parentTabItem = value
            _parentTabItem.Header = _header
        End Set
    End Property

    ''' <summary>
    ''' Order tabs will appear in tab controls.
    ''' Higher values appear first.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable ReadOnly Property SortOrder As Integer
        Get
            Return 0
        End Get
    End Property

    Public Function GetPluginManager() As PluginManager
        Return PluginHelper.PluginManagerInstance
        'Return DirectCast(DirectCast(DirectCast(Me.ParentTabItem.Parent, TabControl).Parent, Grid).Parent, SkyEditorWindows.MainWindow).GetPluginManager
    End Function
End Class