Imports System.Reflection
Imports System.Resources
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
                    If IsOfType(item.GetTypeInfo, typeToCheck) Then
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

        Public Shared Function IsIContainerOfType(obj As Object, typeToCheck As TypeInfo, Optional checkContainer As Boolean = False) As Boolean
            Dim Original As TypeInfo = Nothing

            'Check if the object we're checking is itself a TypeInfo
            If TypeOf obj Is TypeInfo Then
                'If so, we'll Obj to compare
                Original = obj
            Else
                'If not, we'll compare the type of the object we're checking
                Original = obj.GetType.GetTypeInfo
            End If
            Return IsOfType(Original,
                            GetType(IContainer(Of Object)).GetGenericTypeDefinition.MakeGenericType(typeToCheck.AsType).GetTypeInfo, 'Get the type definition of "IContainer(Of TypeToCheck)".
                            False 'If this was true, then we'd be in an infinite loop, checking for "IContainer(Of IContainer(Of TypeToCheck)", "IContainer(Of IContainer(Of IContainer(Of TypeToCheck))", and so on.  The plugin management code will only handle IContainer(Of T), so we only want to check one level.
                            )
        End Function

        Public Shared Function GetIContainerContents(container As Object, containedType As Type) As Object
            Dim interfaceType As Type = (From t In container.GetType.GetTypeInfo.ImplementedInterfaces Where t.IsConstructedGenericType AndAlso t.GenericTypeArguments.Length = 1 AndAlso t.GenericTypeArguments(0).Equals(containedType)).FirstOrDefault
            Dim targetProperty = (From p In interfaceType?.GetTypeInfo.DeclaredProperties Where p.Name = NameOf(IContainer(Of Object).Item)).FirstOrDefault

            Return targetProperty.GetValue(container)
        End Function

        Public Shared Sub SetIContainerContents(container As Object, containedType As TypeInfo, value As Object)
            Dim interfaceType As Type = (From t In container.GetType.GetTypeInfo.ImplementedInterfaces Where t.IsConstructedGenericType AndAlso t.GenericTypeArguments.Length = 1 AndAlso t.GenericTypeArguments(0).Equals(containedType)).FirstOrDefault
            Dim targetProperty = (From p In interfaceType?.GetTypeInfo.DeclaredProperties Where p.Name = NameOf(IContainer(Of Object).Item)).FirstOrDefault

            targetProperty.SetValue(container, value)
        End Sub

        Public Shared Function GetTypeByName(AssemblyQualifiedName As String, Manager As PluginManager) As TypeInfo
            Dim t = Type.GetType(AssemblyQualifiedName, False)
            If t Is Nothing Then
                'Can't find the type.  Time to parse it and search the plugin manager's assemblies to find it.
                'We could have done this to begin with, but Type.GetType is probably a little faster.
                Dim name As New ParsedAssemblyQualifiedName(AssemblyQualifiedName, Manager.GetLoadedAssemblies, Manager)
                t = name.FoundType.Value
            End If
            Return t?.GetTypeInfo
        End Function

        ''' <summary>
        ''' Determines whether the given type has a default constructor.
        ''' </summary>
        ''' <param name="Type"></param>
        ''' <returns></returns>
        Public Shared Function CanCreateInstance(Type As TypeInfo) As Boolean
            Return (From c In Type.DeclaredConstructors Where c.GetParameters.Length = 0).Any AndAlso Not Type.IsAbstract
        End Function

        ''' <summary>
        ''' Creates a new instance of the given type.
        ''' </summary>
        ''' <param name="Type">Type to create an instance of.  Must have a default constructor.</param>
        ''' <returns></returns>
        Public Shared Function CreateInstance(Type As TypeInfo) As Object
            Return Activator.CreateInstance(Type.AsType)
        End Function

        ''' <summary>
        ''' Creates a new instance of the given type.
        ''' </summary>
        ''' <param name="Type">Type to create an instance of.  Must have a default constructor.</param>
        ''' <returns></returns>
        Public Shared Function CreateInstance(Type As Type) As Object
            Return Activator.CreateInstance(Type)
        End Function

        ''' <summary>
        ''' Creates a new instance of the type of the given object.
        ''' </summary>
        ''' <param name="target">Instance of the type of which to create a new instance</param>
        ''' <returns></returns>
        Public Shared Function CreateNewInstance(target As Object) As Object
            Return CreateInstance(target.GetType)
        End Function

        ''' <summary>
        ''' Gets the name of the given type if its contained assembly has its name in its localized resource file, or the full name of the type if it does not.
        ''' </summary>
        ''' <param name="type">Type of which to get the name.</param>
        ''' <returns></returns>
        Public Shared Function GetTypeFriendlyName(type As Type) As String
            Dim output As String = Nothing
            Dim parent = type.GetTypeInfo.Assembly
            Dim manager As ResourceManager = Nothing
            Dim resxNames As New List(Of String)(parent.GetManifestResourceNames)
            'Dim q = From r In resxNames Where String.Compare(r, "language", True, Globalization.CultureInfo.InvariantCulture) = 0
            'If q.Any Then
            '    'Then look in this one first.
            '    manager = New ResourceManager(q.First, parent)
            'End If

            'If manager IsNot Nothing Then
            '    output = manager.GetString(type.FullName.Replace(".", "_"))
            'End If

            If output Is Nothing Then
                'Then either the language resources doesn't exist, or does not contain what we're looking for.
                'In either case, we'll look at the other resource files.
                For Each item In resxNames
                    manager = New ResourceManager(item.Replace(".resources", ""), parent)
                    output = manager.GetString(type.FullName.Replace(".", "_"))
                    If output IsNot Nothing Then
                        Exit For 'We found something.  Time to return it.
                    End If
                Next
            End If

            If output IsNot Nothing Then
                Return output
            Else
                Return type.FullName
            End If
        End Function
    End Class
End Namespace

