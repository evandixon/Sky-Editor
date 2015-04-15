Imports SkyEditorBase
Public Module Loader
    Public Class DeSmuMeVersion
        Implements IComparable(Of DeSmuMeVersion)
        Public Property Path As String
        Public Property Version As Version
        Public Property Is64Bit As Boolean
        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="Filename">The path of the file.</param>
        ''' <remarks></remarks>
        Public Sub New(Filename As String)
            Dim r As New Text.RegularExpressions.Regex("(.*desmume\-)(([0-9]|\.)*)(\-win)(32|64)(\.zip)?")
            Dim m = r.Match(Filename)
            Me.Version = System.Version.Parse(m.Groups(2).Value)
            Me.Is64Bit = (m.Groups(5).Value = "64")
            Me.Path = Filename
        End Sub

        Public Function CompareTo1(other As DeSmuMeVersion) As Integer Implements IComparable(Of DeSmuMeVersion).CompareTo
            Return other.Version.CompareTo(Me.Version)
        End Function
    End Class
    Function GetBestVersionName(Optional SearchOnly32Bit As Boolean = False) As DeSmuMeVersion
        Dim searchFilter As String
        If Environment.Is64BitProcess AndAlso Not SearchOnly32Bit Then
            searchFilter = "*64.zip"
        Else
            searchFilter = "*32.zip"
        End If
        Dim files As New List(Of DeSmuMeVersion)
        For Each item In (IO.Directory.GetFiles(PluginHelper.GetResourceDirectory, searchFilter, IO.SearchOption.TopDirectoryOnly))
            files.Add(New DeSmuMeVersion(item))
        Next
        If files.Count > 0 Then
            files.Sort()
            Return files(0)
        ElseIf SearchOnly32Bit = False Then
            Return GetBestVersionName(True)
        Else
            Return Nothing
        End If
    End Function
    Sub EnsureVersionExtracted(FilePath As String)
        If IO.Directory.Exists(IO.Path.GetFileNameWithoutExtension(FilePath)) Then
            'Everything's fine then
        Else
            SkyEditorBase.Utilities.Zip.UnZip(FilePath, FilePath.Replace(".zip", ""))
        End If
    End Sub
    Function GetBestVersionExecutableName() As String
        Dim version As DeSmuMeVersion = GetBestVersionName()
        EnsureVersionExtracted(version.Path)
        If version.Is64Bit Then
            Return version.Path.Replace(".zip", ("/DeSmuMe-" & version.Version.ToString(3) & "-x64.exe").Replace("-", "_"))
        Else
            Return version.Path.Replace(".zip", ("/DeSmuMe-" & version.Version.ToString(3) & "-x86.exe").Replace("-", "_"))
        End If
    End Function
    Sub RunDeSmuMe(romPath As String)
        SkyEditorBase.PluginHelper.RunProgramInBackground(GetBestVersionExecutableName, String.Format("""{0}""", romPath))
    End Sub
End Module