'Credit to Marius Bancila for figuring this out
'http://www.codeproject.com/Articles/453778/Loading-Assemblies-from-Anywhere-into-a-New-AppDom
Imports System.IO
Imports System.Reflection
Public Class AssemblyReflectionProxy
    Inherits MarshalByRefObject

    Private Property AssemblyPath As String
    Private Property DirectoryInfo As DirectoryInfo

    Public Sub LoadAssembly(AssemblyPath As String)
        Me.AssemblyPath = AssemblyPath
        Me.DirectoryInfo = New FileInfo(AssemblyPath).Directory
        Assembly.LoadFrom(AssemblyPath)
        'Assembly.ReflectionOnlyLoadFrom(AssemblyPath)
    End Sub

    Public Function Reflect(Of T)(Func As Func(Of Assembly, Object(), T), Args() As Object) As T
        'AddHandler AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve, AddressOf OnReflectionOnlyResolve

        'Dim assembly = (From a In AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies Where a.Location = AssemblyPath).FirstOrDefault
        Dim assembly = (From a In AppDomain.CurrentDomain.GetAssemblies Where a.Location = AssemblyPath).FirstOrDefault
        Dim result = Func(assembly, Args)

        'RemoveHandler AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve, AddressOf OnReflectionOnlyResolve

        Return result
    End Function

    Private Function OnReflectionOnlyResolve(sender As Object, e As ResolveEventArgs) As Assembly
        Dim loadedAssembly = (From a In AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies Where String.Equals(a.FullName, e.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault

        If loadedAssembly IsNot Nothing Then
            Return loadedAssembly
        Else
            Dim name As New AssemblyName(e.Name)
            Dim dependentAssemblyFilenameDll = Path.Combine(DirectoryInfo.FullName, name.Name & ".dll")
            Dim dependentAssemblyFilenameExe = Path.Combine(DirectoryInfo.FullName, name.Name & ".exe")

            If File.Exists(dependentAssemblyFilenameDll) Then
                Return Assembly.ReflectionOnlyLoadFrom(dependentAssemblyFilenameDll)
            ElseIf File.Exists(dependentAssemblyFilenameExe) Then
                Return Assembly.ReflectionOnlyLoadFrom(dependentAssemblyFilenameExe)
            Else
                Return Assembly.ReflectionOnlyLoadFrom(e.Name)
            End If
        End If
    End Function
End Class

