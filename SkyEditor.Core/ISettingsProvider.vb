Imports SkyEditor.Core.Interfaces

Public Interface ISettingsProvider
    Inherits iSavable
    Function GetSetting(name As String) As Object
    Sub SetSetting(name As String, value As Object)
End Interface
