
Public MustInherit Class ObjectTab
    Inherits ObjectControl

#Region "Properties"
    ''' <summary>
    ''' Reference to the parent tab item.
    ''' </summary>
    ''' <returns></returns>
    Public Property ParentTabItem As TabItem
        Get
            Return _parentTabItem
        End Get
        Set(value As TabItem)
            _parentTabItem = value
            _parentTabItem.Header = _header
        End Set
    End Property
    Dim _parentTabItem As TabItem

    ''' <summary>
    ''' Gets or sets the header of the parent tab item.
    ''' </summary>
    ''' <returns></returns>
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
    Dim _header As String

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
#End Region

    Public Function GetPluginManager() As PluginManager
        Return PluginHelper.PluginManagerInstance
        'Return DirectCast(DirectCast(DirectCast(Me.ParentTabItem.Parent, TabControl).Parent, Grid).Parent, SkyEditorWindows.MainWindow).GetPluginManager
    End Function
End Class
Public MustInherit Class ObjectTab(Of T)
    Inherits ObjectTab

    Public Sub New()
        MyBase.New()
    End Sub

    Public Property EditingItem As T
        Get
            Return DirectCast(MyBase.EditingObject, T)
        End Get
        Set(value As T)
            MyBase.EditingObject = value
        End Set
    End Property

    Public Overrides ReadOnly Property SupportedTypes As Type()
        Get
            Return {GetType(T)}
        End Get
    End Property
End Class