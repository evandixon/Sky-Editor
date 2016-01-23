Imports System.Reflection
Imports System.Threading.Tasks

Public MustInherit Class ContextMenuAction

    Public Property Header As String

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

    Public MustOverride Function DoAction(Targets As IEnumerable(Of Object)) As Task

End Class
