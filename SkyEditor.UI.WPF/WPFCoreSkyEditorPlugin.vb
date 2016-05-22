Imports System.Reflection
Imports SkyEditor.Core
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Windows
Imports SkyEditor.UI.WPF.ObjectControls

Public MustInherit Class WPFCoreSkyEditorPlugin
    Inherits WindowsCoreSkyEditorPlugin

    Public Overrides Sub Load(manager As PluginManager)
        MyBase.Load(manager)

        manager.RegisterType(GetType(IObjectControl).GetTypeInfo, GetType(GenericEnumerable))
    End Sub

End Class
