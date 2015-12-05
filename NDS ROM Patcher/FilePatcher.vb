Public Class FilePatcher
    Public Property FilePath As String
    Public Property CreatePatchProgram As String
    ''' <summary>
    ''' Arguments for the CreatePatchProgram.
    ''' {0} is a placeholder for the original file, {1} is a placeholder for the updated file, and {2} is a placeholder for the output patch file.
    ''' </summary>
    ''' <returns></returns>
    Public Property CreatePatchArguments As String
    Public Property ApplyPatchProgram As String
    ''' <summary>
    ''' Arguments for the ApplyPatchProgram.
    ''' {0} is a placeholder for the original file, {1} is a placeholder for the patch file, and {2} is a placeholder for the output file.
    ''' </summary>
    ''' <returns></returns>
    Public Property ApplyPatchArguments As String
    Public Property PatchExtension As String
End Class