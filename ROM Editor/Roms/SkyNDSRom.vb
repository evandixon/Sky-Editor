Imports ROMEditor.FileFormats
Imports SkyEditorBase

Namespace Roms
    Public Class SkyNDSRom
        Inherits GenericNDSRom

#Region "Constructors"
        Public Sub New(Filename As String)
            MyBase.New(Filename)
        End Sub
        Public Sub New()
            MyBase.New()
        End Sub
#End Region

#Region "Functions"
        Public Async Function GetPortraitsFile() As Task(Of Kaomado)
            Await EnsureUnpacked()
            Dim x = New Kaomado(PluginHelper.GetResourceName(Name & "\data\font\kaomado.kao"))
            Await x.EnsureUnpacked
            Return x
        End Function
        Public Async Function GetPersonalityTestOverlay() As Task(Of Overlay13)
            Await EnsureUnpacked()
            Return New Overlay13(PluginHelper.GetResourceName(Name & "\overlay\overlay_0013.bin"))
        End Function
        Public Async Function GetLanguageString() As Task(Of LanguageString)
            Await EnsureUnpacked()
            Return New LanguageString(PluginHelper.GetResourceName(Name & "\data\message\text_e.str"))
        End Function
        Public Async Function GetItemP() As Task(Of item_p)
            Await EnsureUnpacked()
            Return New item_p(PluginHelper.GetResourceName(Name & "\data\balance\item_p.bin"))
        End Function
        Public Async Function GetWazaP() As Task(Of waza_p)
            Await EnsureUnpacked()
            Return New waza_p(PluginHelper.GetResourceName(Name & "/data/balance/waza_p.bin"))
        End Function
        Public Async Function GetBackgrounds() As Task(Of List(Of BGP))
            Await EnsureUnpacked()
            Dim images As New List(Of BGP)
            For Each item In IO.Directory.GetFiles(PluginHelper.GetResourceName(Name & "\data\back\"), "*.bgp")
                images.Add(New BGP(item))
            Next
            Return images
        End Function
        Public Sub SetBackgrounds(value As List(Of BGP))
            For Each item In value
                item.Save()
            Next
        End Sub
#End Region

#Region "Properties"
 
#End Region

#Region "GenericSave Stuff"
        Public Overrides Function DefaultSaveID() As String
            Return GameStrings.SkyNDSRom
        End Function
#End Region

        Public Shared Function IsSkyNDSRom(File As GenericFile) As Boolean
            Dim e As New System.Text.ASCIIEncoding
            Return (e.GetString(File.RawData(0, 12)).Trim) = GameStrings.SkyROMHeader
        End Function

    End Class

End Namespace
