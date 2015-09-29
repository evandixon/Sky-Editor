Imports System.Reflection
Imports System.Text.RegularExpressions

Namespace Utilities

    Public Class ReflectionHelpers
        Private Shared Function CustomAssemblyResolver(Name As AssemblyName) As Assembly
            Dim results = From a In AppDomain.CurrentDomain.GetAssemblies Where a.FullName = Name.FullName

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

    End Class

End Namespace

