Imports SkyEditorBase

Public Class item_p
    Inherits GenericFile
    Public Class Item
        Inherits GenericFile
        Public Sub New(Data As Byte())
            MyBase.New(Data)
        End Sub
    End Class
    Public Sub New()
        MyBase.New(IO.File.ReadAllBytes(IO.Path.Combine(PluginDefinition.GetResourceDirectory, "current/data/balance/item_p.bin")))
    End Sub
End Class
