Imports System.Reflection

Public MustInherit Class MenuAction

    Public ReadOnly Property ActionPath As List(Of String)

    ''' <summary>
    ''' IEnumerable of types the action can be performed with.
    ''' If empty, can be performed on any type.
    ''' </summary>
    ''' <returns></returns>
    Public Overridable Function GetTargetTypes() As IEnumerable(Of Type)
        Return {}
    End Function

    Public MustOverride Sub DoAction(Target As Object)

    Public Sub New(Path As IEnumerable(Of String))
        ActionPath = New List(Of String)
        ActionPath.AddRange(Path)
    End Sub

    Public Sub New(Path As String, Optional SeparatorCharacter As Char = "/"c, Optional TranslateItems As Boolean = False, Optional DefaultTranslationValue As String = Nothing)
        ActionPath = New List(Of String)
        For Each item In Path.Split(SeparatorCharacter)
            If TranslateItems Then
                ActionPath.Add(item)
            Else
                ActionPath.Add(PluginHelper.GetLanguageItem(item, DefaultTranslationValue, Assembly.GetCallingAssembly.GetName.FullName))
            End If
        Next
    End Sub

End Class
