Imports System.Reflection
Imports System.Threading.Tasks

Public MustInherit Class MenuAction

    ''' <summary>
    ''' Names representing the action's location in a heiarchy of menu items.
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ActionPath As List(Of String)

    ''' <summary>
    ''' True to target all open files and the current project.
    ''' False to target only the selected file and the current project.
    ''' </summary>
    ''' <returns></returns>
    Public Property TargetAll As Boolean

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
    ''' IEnumerable of types the action can be performed with.
    ''' If empty, can be performed on any type.
    ''' </summary>
    ''' <returns></returns>
    Public Overridable Function SupportedTypes() As IEnumerable(Of Type)
        Return {}
    End Function

    Public Overridable Function SupportsObject(Obj As Object) As Boolean
        Dim q = From t In SupportedTypes() Where Utilities.ReflectionHelpers.IsOfType(Obj.GetType, t)

        Return q.Any
    End Function

    Public Overridable Function SupportsObjects(Objects As IEnumerable(Of Object)) As Boolean
        Return (From o In Objects Where SupportsObject(o)).Any
    End Function

    Public MustOverride Function DoAction(Targets As IEnumerable(Of Object)) As Task

    Public Sub New(Path As IEnumerable(Of String))
        _alwaysVisible = False
        ActionPath = New List(Of String)
        ActionPath.AddRange(Path)
    End Sub

    Public Sub New(Path As String, Optional SeparatorCharacter As Char = "/"c, Optional TranslateItems As Boolean = False, Optional DefaultTranslationValue As String = Nothing)
        _alwaysVisible = False
        ActionPath = New List(Of String)
        For Each item In Path.Split(SeparatorCharacter)
            If Not TranslateItems Then
                ActionPath.Add(item)
            Else
                ActionPath.Add(PluginHelper.GetLanguageItem(item, DefaultTranslationValue, Assembly.GetCallingAssembly.GetName.FullName))
            End If
        Next
    End Sub

End Class
