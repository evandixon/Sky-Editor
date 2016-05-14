Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO
Namespace IO
    Public Interface ICreatableFile
        Inherits iGenericFile
        Inherits IOpenableFile
        Sub CreateFile(Name As String)
    End Interface
End Namespace


