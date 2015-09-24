Imports SkyEditorBase
Imports System.IO

Public Class GenericNDSRom
    Inherits GenericSave
    Implements IDisposable
#Region "General"
    Public Overrides Sub FixChecksum()
        'do nothing
    End Sub

    Sub New(save As Byte())
        MyBase.New(save)
        _unpackTask = New Task(Of Boolean)(Function()
                                               Return True
                                           End Function)
        _unpackTask.Start()
    End Sub

    Sub New(Filename As String)
        MyBase.New(Filename)
        Me.Filename = Filename
        _unpackTask = Nothing
    End Sub

    Public Overrides Function DefaultSaveID() As String
        Return GameStrings.GenericNDSRom
    End Function

    Public ReadOnly Property IsUnpacked
        Get
            Return _unpackTask IsNot Nothing AndAlso _unpackTask.IsCompleted
        End Get
    End Property
    Public ReadOnly Property IsUnpacking
        Get
            Return _unpackTask IsNot Nothing
        End Get
    End Property
#End Region

#Region "ROM Stuff"
    Private _unpackTask As Task(Of Boolean)
    Public Async Function EnsureUnpacked() As Task(Of Boolean)
        If Not IsUnpacked Then
            If Not IsUnpacking Then
                _unpackTask = Unpack()
            End If
            Return Await _unpackTask
        Else
            Return True
        End If
    End Function
    Public Async Function Unpack() As Task(Of Boolean)
        If IsUnpacking Then
            PluginHelper.Writeline("Something failed, unpack called when currently unpacking.")
        End If
        Dim romDirectory As String = PluginHelper.GetResourceDirectory

        If IO.Directory.Exists(IO.Path.Combine(romDirectory, Name)) Then
            Try
                IO.Directory.Delete(IO.Path.Combine(romDirectory, Name), True)
            Catch ex As IOException
                PluginHelper.Writeline(ex.ToString)
            End Try
        End If

        IO.Directory.CreateDirectory(IO.Path.Combine(romDirectory, Name))
        Return Await PluginHelper.RunProgram(IO.Path.Combine(romDirectory, "ndstool.exe"),
                                              String.Format("-v -x ""{0}"" -9 ""{1}/arm9.bin"" -7 ""{1}/arm7.bin"" -y9 ""{1}/y9.bin"" -y7 ""{1}/y7.bin"" -d ""{1}/data"" -y ""{1}/overlay"" -t ""{1}/banner.bin"" -h ""{1}/header.bin""", Filename, IO.Path.Combine(romDirectory, Name)))
    End Function
    Public Async Function RePack(NewFileName As String) As Task(Of Boolean)
        Dim romDirectory As String = PluginHelper.GetResourceDirectory
        If Not IO.Directory.Exists(romDirectory) Then
            IO.Directory.CreateDirectory(romDirectory)
        End If
        If Not IO.Directory.Exists(IO.Path.Combine(romDirectory, Name)) Then
            IO.Directory.CreateDirectory(IO.Path.Combine(romDirectory, Name))
        End If
        PluginHelper.Writeline("Repacking ROM...")
        Return Await PluginHelper.RunProgram(IO.Path.Combine(romDirectory, "ndstool.exe"),
                                              String.Format("-c ""{0}"" -9 ""{1}/arm9.bin"" -7 ""{1}/arm7.bin"" -y9 ""{1}/y9.bin"" -y7 ""{1}/y7.bin"" -d ""{1}/data"" -y ""{1}/overlay"" -t ""{1}/banner.bin"" -h ""{1}/header.bin""", NewFileName, IO.Path.Combine(romDirectory, Name)))
        'DeveloperConsole.Writeline("Repack complete.")
    End Function
    ''' <summary>
    ''' Repacks the rom and runs it.  Running only works if .nds files are associated with an emulator.
    ''' </summary>
    ''' <remarks></remarks>
    Public Async Function RunRom() As Task
        Dim romDirectory As String = PluginHelper.GetResourceDirectory
        Await RePack(romDirectory & "\" & Name & ".nds")
        PluginHelper.Writeline("Running ROM...")
        Process.Start(romDirectory & Name & ".nds")
    End Function
    Public Overrides Async Function GetBytes() As Task(Of Byte())
        Dim romDirectory As String = PluginHelper.GetResourceDirectory
        Await RePack(romDirectory & "\" & Name & ".nds")
        Return IO.File.ReadAllBytes(romDirectory & "\" & Name & ".nds")
    End Function
#End Region

    Public Overrides Sub DebugInfo()
        MyBase.DebugInfo()
    End Sub

    Public ReadOnly Property ROMHeader As String
        Get
            Dim e As New System.Text.ASCIIEncoding
            Return e.GetString(RawData, 0, 12).Trim
        End Get
    End Property
    Public ReadOnly Property ROMID As String
        Get
            Dim e As New System.Text.ASCIIEncoding
            Return e.GetString(RawData, 12, 4).Trim
        End Get
    End Property

    Public Overrides Function DefaultExtension() As String
        Return "*.nds"
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                _unpackTask.Dispose()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class