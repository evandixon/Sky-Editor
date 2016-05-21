'From http://www.codeproject.com/Articles/453778/Loading-Assemblies-from-Anywhere-into-a-New-AppDom
Imports System.Reflection

Public Class AssemblyReflectionManager
    Implements IDisposable

    Public Sub New()
        MapDomains = New Dictionary(Of String, AppDomain)
        LoadedAssemblies = New Dictionary(Of String, AppDomain)
        Proxies = New Dictionary(Of String, AssemblyReflectionProxy)
    End Sub

    Private Property MapDomains As Dictionary(Of String, AppDomain)
    Private Property LoadedAssemblies As Dictionary(Of String, AppDomain)
    Private Property Proxies As Dictionary(Of String, AssemblyReflectionProxy)

    Public Sub LoadAssembly(assemblyPath As String, domainName As String)
        If Not LoadedAssemblies.ContainsKey(assemblyPath) Then

            Dim domain As AppDomain = Nothing
            If MapDomains.ContainsKey(domainName) Then
                domain = MapDomains(domainName)
            Else
                domain = CreateChildDomain(AppDomain.CurrentDomain, domainName)
                MapDomains.Add(domainName, domain)
            End If

            Dim proxyType As Type = GetType(AssemblyReflectionProxy)
            If proxyType.Assembly IsNot Nothing Then
                Dim proxy = domain.CreateInstanceFrom(proxyType.Assembly.Location, proxyType.FullName).Unwrap()
                proxy.LoadAssembly(assemblyPath)

                LoadedAssemblies.Add(assemblyPath, domain)
                Proxies.Add(assemblyPath, proxy)
            End If

        End If
    End Sub

    Public Sub UnloadAssembly(assemblyPath As String)
        If LoadedAssemblies.ContainsKey(assemblyPath) AndAlso Proxies.ContainsKey(assemblyPath) Then
            Dim domain = LoadedAssemblies(assemblyPath)
            Dim count = (From a In LoadedAssemblies.Values Where a Is domain).Count

            If count <> 1 Then
                Throw New NotSupportedException("Unable to unload an assembly from an appdomain with multiple loaded assemblies.")
            Else
                MapDomains.Remove(domain.FriendlyName)
                AppDomain.Unload(domain)

                LoadedAssemblies.Remove(assemblyPath)
                Proxies.Remove(assemblyPath)
            End If
        End If
    End Sub

    Public Sub UnloadDomain(DomainName As String)
        If MapDomains.ContainsKey(DomainName) Then
            Dim domain = MapDomains(DomainName)

            Dim assemblies As New List(Of String)
            assemblies.AddRange(From pair In LoadedAssemblies Where pair.Value Is domain)

            For Each item In assemblies
                LoadedAssemblies.Remove(item)
                Proxies.Remove(item)
            Next

            MapDomains.Remove(DomainName)
            AppDomain.Unload(domain)
        End If
    End Sub

    Public Function Reflect(Of T)(AssemblyPath As String, Func As Func(Of Assembly, Object(), T), ParamArray Args() As Object) As T
        If LoadedAssemblies.ContainsKey(AssemblyPath) AndAlso Proxies.ContainsKey(AssemblyPath) Then
            Return Proxies(AssemblyPath).Reflect(Of T)(Func, Args)
        Else
            Return Nothing
        End If
    End Function

    Private Function CreateChildDomain(ParentDomain As AppDomain, DomainName As String) As AppDomain
        Dim evidence As New Security.Policy.Evidence(ParentDomain.Evidence)
        Dim setup As AppDomainSetup = ParentDomain.SetupInformation
        Return AppDomain.CreateDomain(DomainName, evidence, setup)
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                If MapDomains IsNot Nothing Then
                    For Each item In MapDomains.Values
                        AppDomain.Unload(item)
                    Next
                    LoadedAssemblies.Clear()
                    Proxies.Clear()
                    MapDomains.Clear()
                End If

            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
