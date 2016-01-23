Public Class FilePatcher
    ''' <summary>
    ''' Regular expression used to identify which file paths this FilePatcher will create and apply patches for.
    ''' </summary>
    ''' <returns></returns>
    Public Property FilePath As String
    ''' <summary>
    ''' Path of the program to create a patch, relative to ROMEditor's plugin directory.
    ''' </summary>
    ''' <returns></returns>
    Public Property CreatePatchProgram As String
    ''' <summary>
    ''' Arguments for the CreatePatchProgram.
    ''' {0} is a placeholder for the original file, {1} is a placeholder for the updated file, and {2} is a placeholder for the output patch file.
    ''' </summary>
    ''' <returns></returns>
    Public Property CreatePatchArguments As String
    ''' <summary>
    ''' Path of the patcher program, relative to ROMEditor's plugin directory.
    ''' Will be placed in the Tools directory of a mod pack, and any dependencies should be supplied by FilePatcher.ApplyPatchDependencies.
    ''' </summary>
    ''' <returns></returns>
    Public Property ApplyPatchProgram As String
    ''' <summary>
    ''' Arguments for the ApplyPatchProgram.
    ''' {0} is a placeholder for the original file, {1} is a placeholder for the patch file, and {2} is a placeholder for the output file.
    ''' </summary>
    ''' <returns></returns>
    Public Property ApplyPatchArguments As String
    ''' <summary>
    ''' Extension of the patch file.
    ''' While the FilePath regex will likely be used to identify which patcher to use to apply the patch, the extension should be unique to the patcher.
    ''' </summary>
    ''' <returns></returns>
    Public Property PatchExtension As String
    ''' <summary>
    ''' Specifies whether or not multple patches can be applied to the same file.
    ''' If false, any two mods that patch the same file will be incompatible.
    ''' </summary>
    ''' <returns></returns>
    Public Property MergeSafe As Boolean
    ''' <summary>
    ''' A dictionary of paths of files needed by the patcher.
    ''' The key is the path of the file relative to ROMEditor's plugin directory.  The value is the path it should be placed in, relative to the mod pack's Tools directory (where the patcher will be copied).
    ''' </summary>
    ''' <returns></returns>
    Public Property ApplyPatchDependencies As IDictionary(Of String, String)
End Class