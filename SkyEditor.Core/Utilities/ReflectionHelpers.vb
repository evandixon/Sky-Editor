Imports System.Reflection
Imports SkyEditor.Core.Interfaces

Namespace Utilities
    Public Class ReflectionHelpers
        ''' <summary>
        ''' Determines whether the given Object is, inherits, or implements the given type.
        ''' </summary>
        ''' <param name="obj">The object to check type equality, inheritance, or implementation.  If Obj is itself a TypeInfo, it will be checked directly, instead of checking against the type of Obj.</param>
        ''' <param name="typeToCheck">The type to check for.</param>
        ''' <param name="checkContainer">Whether or not to check if Obj implements IContainer(Of TypeToCheck).</param>
        ''' <returns></returns>
        Public Shared Function IsOfType(obj As Object, typeToCheck As TypeInfo, Optional checkContainer As Boolean = False) As Boolean
            Dim Original As TypeInfo = Nothing

            'Check if the object we're checking is itself a TypeInfo
            If TypeOf obj Is TypeInfo Then
                'If so, we'll Obj to compare
                Original = obj
            Else
                'If not, we'll compare the type of the object we're checking
                Original = obj.GetType.GetTypeInfo
            End If

            'If the original and reference types are the same, or if its base type and TypeToCheck are the same
            Dim match = Original.Equals(typeToCheck) OrElse
                                    (Original.BaseType IsNot Nothing AndAlso
                                            IsOfType(Original.BaseType, typeToCheck, checkContainer))

            'If the above isn't true, let's check the interfaces
            If Not match Then
                'For every interface the type we're checking defines, we'll check to see if that interface is TypeToCheck.
                For Each item In Original.ImplementedInterfaces
                    If item.Equals(typeToCheck) Then
                        match = True
                        Exit For
                    End If
                Next
            End If

            'If it's still not a match, then we know that the original type does not inherit or implement TypeToCheck.
            'If CheckContainer, we'll check to see if the original type implements IContainer(Of TypeToCheck).
            If Not match AndAlso checkContainer AndAlso Not Original.Equals(GetType(Object)) Then
                If IsOfType(Original,
                            GetType(IContainer(Of Object)).GetGenericTypeDefinition.MakeGenericType(typeToCheck.AsType).GetTypeInfo, 'Get the type definition of "IContainer(Of TypeToCheck)".
                            False 'If this was true, then we'd be in an infinite loop, checking for "IContainer(Of IContainer(Of TypeToCheck)", "IContainer(Of IContainer(Of IContainer(Of TypeToCheck))", and so on.  The plugin management code will only handle IContainer(Of T), so we only want to check one level.
                            ) Then
                    match = True
                End If
            End If
            Return match
        End Function

        ''' <summary>
        ''' Determines whether the given type has a default constructor.
        ''' </summary>
        ''' <param name="Type"></param>
        ''' <returns></returns>
        Public Shared Function HasDefaultConstructor(Type As TypeInfo) As Boolean
            Return (From c In Type.DeclaredConstructors Where c.GetParameters.Length = 0).Any
        End Function

        ''' <summary>
        ''' Creates a new instance of the given type.
        ''' </summary>
        ''' <param name="Type">Type to create an instance of.  Must have a default constructor.</param>
        ''' <returns></returns>
        Public Shared Function CreateInstance(Type As TypeInfo) As Object
            Return Activator.CreateInstance(Type.AsType)
        End Function
    End Class
End Namespace

