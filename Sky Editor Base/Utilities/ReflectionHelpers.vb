Imports System.Reflection
Imports System.Text.RegularExpressions
Imports SkyEditorBase.Interfaces
Imports SkyEditorBase.Internal

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

        Public Shared Function IsMethodOverridden(Method As MethodInfo) As Boolean
            Return Not (Method.GetBaseDefinition = Method)
        End Function

        ''' <summary>
        ''' Returns a list of the plugin paths that are valid .Net assemblies that contain an iPlugin.
        ''' </summary>
        ''' <param name="PluginPaths">Full paths of the plugin assemblies to analyse.</param>
        ''' <param name="CoreAssemblyName">Name of the core assembly, usually the Entry assembly.  Assemblies with this name are not supported, to avoid loading duplicates.</param>
        ''' <returns></returns>
        Public Shared Function GetSupportedPlugins(PluginPaths As IEnumerable(Of String), Optional CoreAssemblyName As String = Nothing) As List(Of String)
            Dim supportedList As New List(Of String)
            'We're going to load these assemblies into another appdomain, so we don't accidentally create duplicates, and so we don't keep any unneeded assemblies loaded for the life of the application.
            Using reflectionManager As New AssemblyReflectionManager
                For Each item In PluginPaths
                    reflectionManager.LoadAssembly(item, "PluginManagerAnalysis")

                    Dim pluginInfoNames As New List(Of String)

                    Try
                        pluginInfoNames =
                            reflectionManager.Reflect(item,
                                                      Function(a As Assembly, Args() As Object) As List(Of String)
                                                          Dim out As New List(Of String)

                                                          If a IsNot Nothing AndAlso
                                                            Not (a.FullName = Assembly.GetCallingAssembly.FullName OrElse
                                                                    (Assembly.GetEntryAssembly IsNot Nothing AndAlso a.FullName = Assembly.GetEntryAssembly.FullName) OrElse
                                                                    a.FullName = Assembly.GetExecutingAssembly.FullName OrElse
                                                                    (Args(0) IsNot Nothing AndAlso a.FullName = Args(0))
                                                                    ) Then
                                                              For Each t As Type In a.GetTypes
                                                                  Dim isPlg As Boolean = (From i In t.GetInterfaces Where ReflectionHelpers.IsOfType(i, GetType(iSkyEditorPlugin))).Any
                                                                  If isPlg Then
                                                                      out.Add(t.FullName)
                                                                  End If
                                                              Next
                                                          End If

                                                          Return out
                                                      End Function, CoreAssemblyName)
                    Catch ex As Reflection.ReflectionTypeLoadException
                        'If we fail here, then the assembly is NOT a valid plugin, so we won't load it.
                        Console.WriteLine(ex.ToString)
                    Catch ex As IO.FileNotFoundException
                        'If we fail here, then the assembly is missing some of its references, meaning it's not a valid plugin.
                        Console.WriteLine(ex.ToString)
                    End Try

                    If pluginInfoNames.Count > 0 Then
                        'Then we want to keep this assembly
                        supportedList.Add(item)
                    End If
                Next
            End Using 'The reflection appdomain will be unloaded on dispose
            Return supportedList
        End Function

    End Class

End Namespace

