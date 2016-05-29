Imports System.Reflection
Imports System.Threading.Tasks
Imports SkyEditor.Core.Utilities

Namespace UI
    Public MustInherit Class MenuAction

        ''' <summary>
        ''' Whether or not the menu item appears in context menus.
        ''' </summary>
        ''' <returns></returns>
        Public Property IsContextBased As Boolean

        ''' <summary>
        ''' Names representing the action's location in a heiarchy of menu items.
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ActionPath As List(Of String)

        ''' <summary>
        ''' The current instance of the plugin manager
        ''' </summary>
        ''' <returns></returns>
        Public Property CurrentPluginManager As PluginManager

        '''' <summary>
        '''' True to target all open files and the current project.
        '''' False to target only the selected file and the current project.
        '''' </summary>
        '''' <returns></returns>
        'Public Property TargetAll As Boolean

        ''' <summary>
        ''' True to be visible regardless of current targets.
        ''' False to be dependant on MenuAction.SupportsObjects.
        ''' </summary>
        ''' <returns></returns>
        Public Property AlwaysVisible As Boolean
            Get
                Return _alwaysVisible
            End Get
            Protected Set(value As Boolean)
                _alwaysVisible = value
            End Set
        End Property
        Dim _alwaysVisible As Boolean

        ''' <summary>
        ''' True to be visible only when Development Mode is enabled and normal visibility conditions are satisfied.
        ''' False to be visibile as normal.
        ''' </summary>
        ''' <returns></returns>
        Public Property DevOnly As Boolean
            Get
                Return _devOnly
            End Get
            Protected Set(value As Boolean)
                _devOnly = value
            End Set
        End Property
        Dim _devOnly As Boolean

        ''' <summary>
        ''' Order in which menu items are sorted
        ''' </summary>
        ''' <returns></returns>
        Public Property SortOrder As Decimal
            Get
                Return _sortOrder
            End Get
            Protected Set(value As Decimal)
                _sortOrder = value
            End Set
        End Property
        Dim _sortOrder As Decimal

        ''' <summary>
        ''' IEnumerable of types the action can be performed with.
        ''' If empty, can be performed on any type.
        ''' </summary>
        ''' <returns></returns>
        Public Overridable Function SupportedTypes() As IEnumerable(Of TypeInfo)
            Return {}
        End Function

        Public Overridable Function SupportsObject(Obj As Object) As Boolean
            If Obj Is Nothing Then
                Return AlwaysVisible
            Else
                Dim q = From t In SupportedTypes() Where ReflectionHelpers.IsOfType(Obj.GetType, t)

                Return q.Any
            End If
        End Function

        Public Overridable Function SupportsObjects(Objects As IEnumerable(Of Object)) As Boolean
            Return (From o In Objects Where SupportsObject(o)).Any
        End Function

        Public MustOverride Function DoAction(Targets As IEnumerable(Of Object)) As Task

        Public Sub New(Path As IEnumerable(Of String))
            _alwaysVisible = False
            ActionPath = New List(Of String)
            ActionPath.AddRange(Path)
            DevOnly = False
            SortOrder = Integer.MaxValue
        End Sub

    End Class

End Namespace
