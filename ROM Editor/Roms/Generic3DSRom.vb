Imports SkyEditorBase

Namespace Roms
    Public Class Generic3DSRom
        Inherits GenericFile
        Implements Interfaces.iOpenableFile
        Implements iPackedRom
        Public Overrides Function DefaultExtension() As String
            Return "*.3ds"
        End Function

        Public Shared Shadows Function IsFileOfType(File As GenericFile) As Boolean
            If File.Length > 104 Then
                Dim e As New System.Text.ASCIIEncoding
                Return e.GetString(File.RawData(&H100, 4)) = "NCSD"
            Else
                Return False
            End If
        End Function

        Public Async Function Unpack(Optional DestinationDirectory As String = Nothing) As Task Implements iPackedRom.Unpack
            If DestinationDirectory Is Nothing Then
                DestinationDirectory = IO.Path.Combine(PluginHelper.GetResourceName(IO.Path.GetFileNameWithoutExtension(Me.Filename)))
            End If
            If Not IO.Directory.Exists(DestinationDirectory) Then
                IO.Directory.CreateDirectory(DestinationDirectory)
            End If
            Dim exHeaderPath = IO.Path.Combine(DestinationDirectory, "DecryptedExHeader.bin")
            Dim exefsPath = IO.Path.Combine(DestinationDirectory, "DecryptedExeFS.bin")
            Dim romfsPath = IO.Path.Combine(DestinationDirectory, "DecryptedRomFS.bin")
            Dim romfsDir = IO.Path.Combine(DestinationDirectory, "romfs")
            Dim exefsDir = IO.Path.Combine(DestinationDirectory, "exefs")
            'Unpack portions
            Await RunCtrTool($"-p --exheader=""{exHeaderPath}"" ""{Filename}""")
            Await RunCtrTool($"-p --exefs=""{exefsPath}"" ""{Filename}""")
            Await RunCtrTool($"-p --romfs=""{romfsPath}"" ""{Filename}""")
            'Unpack romfs
            Await RunCtrTool($"-t romfs --romfsdir=""{romfsDir}"" ""{romfsPath}""")
            'Unpack exefs
            Await RunCtrTool($"-t exefs --exefsdir=""{exefsDir}"" ""{exefsPath}""")
            IO.File.Delete(exefsPath)
            IO.File.Delete(romfsPath)
        End Function

        Public Function RePack(NewFileName As String) As Task Implements iPackedRom.RePack
            Throw New NotImplementedException()
        End Function

        Public Shared Async Function RunCtrTool(Arguments As String) As Task
            Await PluginHelper.RunProgram(PluginHelper.GetResourceName("ctrtool.exe"), Arguments)
        End Function

        Public Sub New(Filename As String)
            MyBase.New(Filename)
        End Sub
    End Class

End Namespace
