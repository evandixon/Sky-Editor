Imports SkyEditor.Core.Interfaces
Imports SkyEditor.Core.IO

Public Interface ISettingsProvider
    Inherits ISavable
    Function GetSetting(name As String) As Object
    Sub SetSetting(name As String, value As Object)
End Interface
