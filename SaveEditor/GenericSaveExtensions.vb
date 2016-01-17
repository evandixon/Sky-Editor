Imports SkyEditorBase
Imports System.Runtime.CompilerServices
Module GenericSaveExtensions
    <Extension()> _
    Public Function IsSkySave(ByVal aGenericSave As GenericSave) As Boolean
        Return (New MDSaveBase(aGenericSave.RawData)).IsSkySave
    End Function
    <Extension()> _
    Public Function IsTDSave(ByVal aGenericSave As GenericSave) As Boolean
        Return (New MDSaveBase(aGenericSave.RawData)).IsTDSave
    End Function
    <Extension()> _
    Public Function IsRBSave(ByVal aGenericSave As GenericSave) As Boolean
        If aGenericSave IsNot Nothing Then
            Return (New MDSaveBase(aGenericSave.RawData)).IsRBSave
        Else
            Return False
        End If
    End Function
End Module
