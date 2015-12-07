Imports System.Web.Script.Serialization

Public Class ModFile
    Public Property ModDetails As ModJson
    Public Property Name As String
    Public Property Patched As Boolean
    Public Property Filename As String
    Public Async Function ApplyPatch(currentDirectory As String, ROMDirectory As String, patchers As List(Of FilePatcher)) As Task
        Dim renameTemp = IO.Path.Combine(currentDirectory, "Tools/renametemp")
        If ModDetails.ToAdd IsNot Nothing Then
            For Each file In ModDetails.ToAdd
                IO.File.Copy(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Files", file.Trim("\")), IO.Path.Combine(ROMDirectory, file.Trim("\")), True)
            Next
        End If

        If ModDetails.ToUpdate IsNot Nothing Then
            For Each file In ModDetails.ToUpdate
                If IO.File.Exists(IO.Path.Combine(ROMDirectory, file.TrimStart("\"))) Then
                    Dim patches = IO.Directory.GetFiles(IO.Path.GetDirectoryName(IO.Path.Combine(IO.Path.GetDirectoryName(Filename), "Files", file.Trim("\"))), IO.Path.GetFileName(file.Trim("\")) & "*")
                    'Hopefully we only have 1 patch, but if there's more than 1 patch, apply them all.
                    For Each patchFile In patches
                        Dim possiblePatchers As New List(Of FilePatcher) ' = (From p In patchers Where p.PatchExtension = IO.Path.GetExtension(patchFile) Select p).ToList
                        For Each p As FilePatcher In patchers
                            If "." & p.PatchExtension = IO.Path.GetExtension(patchFile) Then
                                possiblePatchers.Add(p)
                            End If
                        Next
                        'If possiblePatchers.Count = 0 Then
                        '   Do nothing, we don't have the tools to deal with this patch
                        If possiblePatchers.Count >= 1 Then
                            Dim tempFilename As String = IO.Path.Combine(currentDirectory, "Tools", "tempFile")
                            'If there's 1 possible patcher, great.  If there's more than one, then multiple programs have the same extension, which is their fault.  Only using the first one because we don't need to apply the same patch multiple times.
                            Await ProcessHelper.RunProgram(IO.Path.Combine(currentDirectory, "Tools", "Patchers", possiblePatchers(0).ApplyPatchProgram), String.Format(possiblePatchers(0).ApplyPatchArguments, IO.Path.Combine(ROMDirectory, file.TrimStart("\")), patchFile, tempFilename))

                            If Not IO.File.Exists(tempFilename) Then
                                MessageBox.Show("Unable to patch file """ & file & """.  Please ensure you're using a supported ROM.  If you sure you are, report this to the mod author.")
                            Else
                                IO.File.Copy(tempFilename, IO.Path.Combine(ROMDirectory, file.TrimStart("\")), True)
                                IO.File.Delete(tempFilename)
                            End If
                        End If
                    Next
                End If
            Next
        End If

        If ModDetails.ToRename IsNot Nothing Then
            'Create temporary directory
            If Not IO.Directory.Exists(renameTemp) Then
                IO.Directory.CreateDirectory(renameTemp)
            End If

            'Move to a temporary directory (so swapping files works)
            For Each file In ModDetails.ToRename
                CopyFile(IO.Path.Combine(ROMDirectory, file.Key.Trim("\")), IO.Path.Combine(renameTemp, file.Key.Trim("\")), True)
            Next

            'Rename the things
            For Each file In ModDetails.ToRename
                CopyFile(IO.Path.Combine(renameTemp, file.Key.Trim("\")), IO.Path.Combine(ROMDirectory, file.Value.Trim("\")), True)
            Next
        End If

        If ModDetails.ToDelete IsNot Nothing Then
            For Each file In ModDetails.ToDelete
                If IO.File.Exists(IO.Path.Combine(ROMDirectory, file.Trim("\"))) Then
                    IO.File.Delete(IO.Path.Combine(ROMDirectory, file.Trim("\")))
                End If
            Next
        End If

        If IO.Directory.Exists(renameTemp) Then IO.Directory.Delete(renameTemp, True)

        Patched = True
    End Function
    Public Shared Async Function ApplyPatch(Mods As List(Of ModFile), ModFile As ModFile, currentDirectory As String, ROMDirectory As String, patchers As List(Of FilePatcher)) As Task
        If Not ModFile.Patched Then
            'Patch depencencies
            If ModFile.ModDetails.DependenciesBefore IsNot Nothing Then
                For Each item In ModFile.ModDetails.DependenciesBefore
                    Dim q = From m In Mods Where m.Name = item AndAlso Not String.IsNullOrEmpty(m.Name)

                    For Each d In q
                        Await ApplyPatch(Mods, d, currentDirectory, ROMDirectory, patchers)
                    Next
                Next
            End If
            Await ModFile.ApplyPatch(currentDirectory, ROMDirectory, patchers)
            'Patch dependencies
            If ModFile.ModDetails.DependenciesBefore IsNot Nothing Then
                For Each item In ModFile.ModDetails.DependenciesAfter
                    Dim q = From m In Mods Where m.Name = item AndAlso Not String.IsNullOrEmpty(m.Name)

                    For Each d In q
                        Await ApplyPatch(Mods, d, currentDirectory, ROMDirectory, patchers)
                    Next
                Next
            End If
        End If
    End Function
    Public Sub New(Filename As String)
        Dim j As New JavaScriptSerializer
        Me.ModDetails = j.Deserialize(Of ModJson)(IO.File.ReadAllText(Filename))
        Me.Name = Me.ModDetails.Name
        Me.Patched = False
        Me.Filename = Filename
    End Sub
    Public Shared Sub CopyFile(OriginalFilename As String, NewFilename As String, Overwrite As Boolean)
        If Not IO.Directory.Exists(IO.Path.GetDirectoryName(NewFilename)) Then
            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(NewFilename))
        End If
        IO.File.Copy(OriginalFilename, NewFilename, Overwrite)
    End Sub
End Class