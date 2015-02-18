Imports SkyEditorBase

Namespace FileFormats
    Public Class Kaomado
        Public Property Filename As String
        Public ReadOnly Property UnpackDirectory As String
            Get
                Return PluginHelper.GetResourceName("current\data\font\kaomado_unpack").Replace("\current\", "\temp\")
            End Get
        End Property
        Public Shared Async Function RunUnpack(Filename As String) As Task(Of Boolean)
            Dim romDirectory As String = PluginHelper.GetResourceDirectory
            If Not IO.Directory.Exists(IO.Path.GetDirectoryName(Filename).Replace("\current\", "\temp\") & "\kaomado_unpack") Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(Filename).Replace("\current\", "\temp\") & "\kaomado_unpack")
            End If
            Return Await SkyEditorBase.PluginHelper.RunProgram(IO.Path.Combine(romDirectory, "ppmd_kaoutil.exe"),
                                                  String.Format("-fn ""{0}"" -pn ""{1}"" ""{2}"" ""{3}""", IO.Path.Combine(romDirectory, "facenames.txt"), IO.Path.Combine(romDirectory, "pokenames.txt"), Filename, IO.Path.GetDirectoryName(Filename).Replace("\current\", "\temp\") & "\kaomado_unpack"))
        End Function
        Public Shared Async Function RunPack(Filename As String) As Task(Of Boolean)
            Dim romDirectory As String = PluginHelper.GetResourceDirectory
            Return Await SkyEditorBase.PluginHelper.RunProgram(IO.Path.Combine(romDirectory, "ppmd_kaoutil.exe"),
                                                  String.Format("-fn ""{0}"" -pn ""{1}"" ""{2}"" ""{3}""", IO.Path.Combine(romDirectory, "facenames.txt"), IO.Path.Combine(romDirectory, "pokenames.txt"), IO.Path.GetDirectoryName(Filename).Replace("\current\", "\temp\") & "\kaomado_unpack", Filename))
        End Function
        Public Async Function Save(Filename As String) As Task
            Await RunPack(Filename)
        End Function
        Public Async Function Save() As Task(Of Boolean)
            Return Await RunPack(Me.Filename)
        End Function
        Public Sub New(OriginalFilename As String)
            Me.Filename = OriginalFilename
            _unpackTask = Nothing
        End Sub
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
    End Class
End Namespace
