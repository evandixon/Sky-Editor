Imports System.IO
Imports SkyEditor.Core
Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO
Imports SkyEditor.ROMEditor
Imports SkyEditorBase

Namespace Roms
    Public Class Generic3DSRom
        Inherits GenericFile
        Implements IOpenableFile
        Implements IDetectableFileType
        Public Overrides Function GetDefaultExtension() As String
            Return "*.3ds"
        End Function

        Public Overridable Function IsOfType(File As GenericFile) As Task(Of Boolean) Implements IDetectableFileType.IsOfType
            If File.Length > 104 Then
                Dim e As New System.Text.ASCIIEncoding
                Return Task.FromResult(e.GetString(File.RawData(&H100, 4)) = "NCSD")
            Else
                Return Task.FromResult(False)
            End If
        End Function
        Public ReadOnly Property GameCode As String
            Get
                Return Text.Encoding.UTF8.GetString(RawData(&H1156, 4), 0, 4).Trim
            End Get
        End Property

        Public Overridable ReadOnly Property TitleID As String
            Get
                Return Conversion.Hex(BitConverter.ToUInt64(RawData(&H108, 8), 0)).PadLeft(16, "0"c)
            End Get
        End Property

        Public Property OnlineHeaderBinary As Byte()
            Get
                Return RawData(&H1200, &H2E00)
            End Get
            Set(value As Byte())
                RawData(&H1200, &H2E00) = value
            End Set
        End Property

        Public Async Function Unpack(DestinationDirectory As String) As Task
            PluginHelper.SetLoadingStatus(My.Resources.Language.LoadingUnpacking)
            If DestinationDirectory Is Nothing Then
                DestinationDirectory = Path.Combine(PluginHelper.GetResourceName(Path.GetFileNameWithoutExtension(Me.PhysicalFilename)))
            End If
            If Not Directory.Exists(DestinationDirectory) Then
                Directory.CreateDirectory(DestinationDirectory)
            End If
            '(Basically these variables are formatted as so: [item][bin/dir][path/task])
            'Define paths
            Dim exHeaderPath = Path.Combine(DestinationDirectory, "DecryptedExHeader.bin")
            Dim exefsBinPath = Path.Combine(DestinationDirectory, "DecryptedExeFS.bin")
            Dim romfsBinPath = Path.Combine(DestinationDirectory, "DecryptedRomFS.bin")
            Dim romfsDirPath = Path.Combine(DestinationDirectory, "Romfs")
            Dim exefsDirPath = Path.Combine(DestinationDirectory, "Exefs")
            Dim manualBinPath = Path.Combine(DestinationDirectory, "DecryptedManual.bin")
            Dim manualDirPath = Path.Combine(DestinationDirectory, "Manual")
            Dim dlPlayBinPath = Path.Combine(DestinationDirectory, "DecryptedDownloadPlay.bin")
            Dim dlPlayDirPath = Path.Combine(DestinationDirectory, "DownloadPlay")
            Dim n3dsUpdateBinPath = Path.Combine(DestinationDirectory, "DecryptedN3DSUpdate.bin")
            Dim n3dsUpdateDirPath = Path.Combine(DestinationDirectory, "N3DSUpdate")
            Dim o3dsUpdateBinPath = Path.Combine(DestinationDirectory, "DecryptedO3DSUpdate.bin")
            Dim o3dsUpdateDirPath = Path.Combine(DestinationDirectory, "O3DSUpdate")
            Dim onlineHeaderBinPath = Path.Combine(DestinationDirectory, "OnlineHeader.bin")

            'Unpack portions
            Dim exheaderTask = RunCtrTool($"-p --ncch=0 --exheader=""{exHeaderPath}"" ""{PhysicalFilename}""")
            Dim exefsBinTask = RunCtrTool($"-p --ncch=0 --exefs=""{exefsBinPath}"" ""{PhysicalFilename}""")
            Dim romfsBinTask = RunCtrTool($"-p --ncch=0 --romfs=""{romfsBinPath}"" ""{PhysicalFilename}""")
            Dim manualBinTask = RunCtrTool($"-p --ncch=1 --romfs=""{manualBinPath}"" ""{PhysicalFilename}"" --decompresscode")
            Dim dlPlayBinTask = RunCtrTool($"-p --ncch=2 --romfs=""{dlPlayBinPath}"" ""{PhysicalFilename}"" --decompresscode")
            Dim n3dsUpdateBinTask = RunCtrTool($"-p --ncch=6 --romfs=""{n3dsUpdateBinPath}"" ""{PhysicalFilename}"" --decompresscode")
            Dim o3dsUpdateBinTask = RunCtrTool($"-p --ncch=7 --romfs=""{o3dsUpdateBinPath}"" ""{PhysicalFilename}"" --decompresscode")

            'Save online header
            File.WriteAllBytes(onlineHeaderBinPath, OnlineHeaderBinary)

            'Unpack romfs
            Await romfsBinTask
            Dim romfsDirTask = RunCtrTool($"-t romfs --romfsdir=""{romfsDirPath}"" ""{romfsBinPath}""")

            'Unpack exefs
            Await exefsBinTask
            Dim exefsDirTask = RunCtrTool($"-t exefs --exefsdir=""{exefsDirPath}"" ""{exefsBinPath}"" --decompresscode")

            'Unpack manual
            Await manualBinTask
            Dim manualDirTask = RunCtrTool($"-t romfs --romfsdir=""{manualDirPath}"" ""{manualBinPath}""")

            'Unpack n3ds update
            Await n3dsUpdateBinTask
            Dim n3dsUpdateDirTask = RunCtrTool($"-t romfs --romfsdir=""{n3dsUpdateDirPath}"" ""{n3dsUpdateBinPath}""")

            'Unpack o3ds update
            Await o3dsUpdateBinTask
            Dim o3dsUpdateDirTask = RunCtrTool($"-t romfs --romfsdir=""{o3dsUpdateDirPath}"" ""{o3dsUpdateBinPath}""")

            'Unpack download play
            Await dlPlayBinTask
            Dim dlPlayDirTask = RunCtrTool($"-t romfs --romfsdir=""{dlPlayDirPath}"" ""{dlPlayBinPath}""")


            Await romfsDirTask
            Await exefsDirTask
            Await manualDirTask
            Await n3dsUpdateDirTask
            Await dlPlayDirTask

            Utilities.FileSystem.DeleteFile(exefsBinPath, SkyEditorBase.PluginManager.GetInstance.CurrentIOProvider)
            Utilities.FileSystem.DeleteFile(romfsBinPath, SkyEditorBase.PluginManager.GetInstance.CurrentIOProvider)
            Utilities.FileSystem.DeleteFile(manualBinPath, SkyEditorBase.PluginManager.GetInstance.CurrentIOProvider)
            Utilities.FileSystem.DeleteFile(dlPlayBinPath, SkyEditorBase.PluginManager.GetInstance.CurrentIOProvider)
            Utilities.FileSystem.DeleteFile(n3dsUpdateBinPath, SkyEditorBase.PluginManager.GetInstance.CurrentIOProvider)
            Utilities.FileSystem.DeleteFile(o3dsUpdateBinPath, SkyEditorBase.PluginManager.GetInstance.CurrentIOProvider)
            PluginHelper.SetLoadingStatusFinished()
        End Function

        Private Shared Async Function RunCtrTool(Arguments As String) As Task
            Await PluginHelper.RunProgram(PluginHelper.GetResourceName("ctrtool.exe"), Arguments, False)
        End Function

        Public Sub New()
            MyBase.New()
        End Sub

    End Class

End Namespace
