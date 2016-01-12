Imports System.Reflection
Imports System.Text.RegularExpressions
Imports SkyEditorBase.Interfaces

Namespace Utilities

    Public Class ReflectionHelpers
        Public Class TypeInheritanceDepthComparer
            Implements IComparer(Of Type)

            Public Function Compare(x As Type, y As Type) As Integer Implements IComparer(Of Type).Compare
                Return GetInheritanceDepth(x).CompareTo(GetInheritanceDepth(y))
            End Function
            Public Function GetInheritanceDepth(T As Type) As Integer
                If Not T = GetType(Object) Then
                    Return 1 + GetInheritanceDepth(T.BaseType)
                Else
                    Return 0
                End If
            End Function
        End Class
        Private Shared Function CustomAssemblyResolver(Name As AssemblyName) As Assembly
            Dim results = From a In AppDomain.CurrentDomain.GetAssemblies Where a.GetName.Name = Name.Name

            If results.Count > 0 Then
                Return results.First
            Else
                Return Nothing
            End If
        End Function
        Private Shared Function TypeResolver(A As Assembly, TypeName As String, ignoreCase As Boolean) As Type
            If TypeName = "System.Object" Then
                Return GetType(Object)
            Else
                Return A.GetType(TypeName, False, True)
            End If
        End Function

        ''' <summary>
        ''' Gets the Type from the given type name.
        ''' </summary>
        ''' <param name="TypeName"></param>
        ''' <returns></returns>
        Public Shared Function GetTypeFromName(TypeName As String) As Type
            Return Type.GetType(TypeName, AddressOf CustomAssemblyResolver, AddressOf TypeResolver, False)
        End Function

        Public Shared Function GetAssemblyVersion(Assembly As Assembly) As Version
            Return Assembly.GetName.Version
        End Function

        Public Shared Function GetAssemblyFileName(Assembly As Assembly, PluginFolder As String) As String
            Dim n = Assembly.GetName.Name
            If IO.File.Exists(IO.Path.Combine(PluginFolder, n & ".dll")) Then
                Return n & ".dll"
            ElseIf IO.File.Exists(IO.Path.Combine(PluginFolder, n & ".exe")) Then
                Return n & ".exe"
            Else
                Return n & ".dll"
            End If
        End Function

        Public Shared Function IsOfType(Obj As Object, TypeToCheck As Type, Optional CheckContainer As Boolean = True) As Boolean
            Dim match = False
            Dim Original As Type = Nothing
            If TypeOf Obj Is Type Then
                If TypeToCheck.IsEquivalentTo(GetType(Type)) Then
                    match = True
                Else
                    Original = Obj
                End If
            Else
                Original = Obj.GetType
            End If
            If Not match Then
                match = Original.IsEquivalentTo(TypeToCheck) OrElse (Original.BaseType IsNot Nothing AndAlso IsOfType(Original.BaseType, TypeToCheck, CheckContainer))
            End If
            If Not match Then
                For Each item In Original.GetInterfaces
                    If item.IsEquivalentTo(TypeToCheck) Then
                        match = True
                        Exit For
                    End If
                Next
            End If
            If Not match AndAlso CheckContainer AndAlso Not Original.IsEquivalentTo(GetType(Object)) Then
                'Check to see if this is an object file of the type we're looking for
                If IsOfType(Original, GetType(iContainer(Of Object)).GetGenericTypeDefinition.MakeGenericType(TypeToCheck), False) Then
                    match = True
                End If
            End If
            Return match
        End Function

    End Class

End Namespace

