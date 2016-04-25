Imports SkyEditorBase

Namespace FileFormats.Explorers
    Public Class Kaomado
        Public Property Filename As String
        Public ReadOnly Property UnpackDirectory As String
            Get
                Return PluginHelper.GetResourceName("temp\" & IO.Path.GetFileName(Filename) & "\data\font\kaomado_unpack")
            End Get
        End Property
        Public Async Function RunUnpack(Filename As String) As Task
            Dim romDirectory As String = PluginHelper.GetResourceDirectory
            If Not IO.Directory.Exists(UnpackDirectory) Then
                IO.Directory.CreateDirectory(UnpackDirectory)
            End If
            Await SkyEditorBase.PluginHelper.RunProgram(IO.Path.Combine(romDirectory, "ppmd_kaoutil.exe"),
                                                  String.Format("-fn ""{0}"" -pn ""{1}"" ""{2}"" ""{3}""", IO.Path.Combine(romDirectory, "facenames.txt"), IO.Path.Combine(romDirectory, "pokenames.txt"), Filename, UnpackDirectory))
        End Function
        Public Async Function RunPack(Filename As String) As Task
            Dim romDirectory As String = PluginHelper.GetResourceDirectory
            Await SkyEditorBase.PluginHelper.RunProgram(IO.Path.Combine(romDirectory, "ppmd_kaoutil.exe"),
                                                  String.Format("-fn ""{0}"" -pn ""{1}"" ""{2}"" ""{3}""", IO.Path.Combine(romDirectory, "facenames.txt"), IO.Path.Combine(romDirectory, "pokenames.txt"), UnpackDirectory, Filename))
        End Function
        Public Shared Async Function RunUnpack(Filename As String, UnpackDirectory As String) As Task
            Dim romDirectory As String = PluginHelper.GetResourceDirectory
            If Not IO.Directory.Exists(UnpackDirectory) Then
                IO.Directory.CreateDirectory(UnpackDirectory)
            End If
            Await SkyEditorBase.PluginHelper.RunProgram(IO.Path.Combine(romDirectory, "ppmd_kaoutil.exe"),
                                                  String.Format("-fn ""{0}"" -pn ""{1}"" ""{2}"" ""{3}""", IO.Path.Combine(romDirectory, "facenames.txt"), IO.Path.Combine(romDirectory, "pokenames.txt"), Filename, UnpackDirectory))
        End Function
        Public Shared Async Function RunPack(Filename As String, UnpackDirectory As String) As Task
            Dim romDirectory As String = PluginHelper.GetResourceDirectory
            Await SkyEditorBase.PluginHelper.RunProgram(IO.Path.Combine(romDirectory, "ppmd_kaoutil.exe"),
                                                  String.Format("-fn ""{0}"" -pn ""{1}"" ""{2}"" ""{3}""", IO.Path.Combine(romDirectory, "facenames.txt"), IO.Path.Combine(romDirectory, "pokenames.txt"), UnpackDirectory, Filename))
        End Function
        Public Async Function Save(Filename As String) As Task
            Await RunPack(Filename)
        End Function
        Public Async Function Save() As Task
            Await RunPack(Me.Filename)
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
        Private _unpackTask As Task(Of Boolean)
        Public Async Function EnsureUnpacked() As Task(Of Boolean)
            If Not IsUnpacked Then
                If Not IsUnpacking Then
                    _unpackTask = RunUnpack(Me.Filename)
                End If
                Return Await _unpackTask
            Else
                Return True
            End If
        End Function
        Public Async Function ApplyMissingPortraitFix(UnpackDirectory As String) As Task
            Dim faces = {"0000_STANDARD.png",
                                                               "0002_GRIN.png",
                                                               "0004_PAINED.png",
                                                               "0006_ANGRY.png",
                                                               "0008_WORRIED.png",
                                                               "0010_SAD.png",
                                                               "0012_CRYING.png",
                                                               "0014_SHOUTING.png",
                                                               "0016_TEARY_EYED.png",
                                                               "0018_DETERMINED.png",
                                                               "0020_JOYOUS.png",
                                                               "0022_INSPIRED.png",
                                                               "0024_SURPRISED.png",
                                                               "0026_DIZZY.png",
                                                               "0032_SIGH.png",
                                                               "0034_STUNNED.png"}

            Dim runner As New SkyEditorBase.Utilities.AsyncFor("Fixing missing portraits...")
            Dim directories = IO.Directory.GetDirectories(UnpackDirectory)
            Dim delegateAction As New Utilities.AsyncFor.ForEachItem(Of String)(Sub(directory As String)
                                                                                    For j As Integer = 1 To faces.Length - 1
                                                                                        If Not IO.File.Exists(IO.Path.Combine(directory, faces(j))) Then
                                                                                            IO.File.Copy(IO.Path.Combine(directory, faces(0)), IO.Path.Combine(directory, faces(j)))
                                                                                        End If
                                                                                    Next
                                                                                End Sub)
            Await runner.RunForEach(delegateAction, directories)
        End Function
        Public Async Function ApplyMissingPortraitFix() As Task
            Await ApplyMissingPortraitFix(Me.UnpackDirectory)
        End Function

        Public Sub New(OriginalFilename As String)
            Me.Filename = OriginalFilename
            _unpackTask = Nothing
        End Sub
    End Class
End Namespace