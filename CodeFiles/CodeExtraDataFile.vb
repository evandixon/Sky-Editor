Imports CodeFiles
Imports SkyEditorBase
Imports SkyEditorBase.Interfaces
Imports SkyEditorBase.Utilities
''' <summary>
''' An instance of CodeExtraData that reads its information from disk.
''' </summary>
Public Class CodeExtraDataFile
    Inherits CodeExtraData
    Implements iCreatableFile
    Implements iOpenableFile
    Implements iOnDisk
    Implements ISavableAs

    Private Class JsonStructure
        Public Property Database As List(Of FunctionDocumentation)
        Public Property AutoCompleteChars As List(Of Char)
    End Class

    Public Property Database As List(Of FunctionDocumentation)
    Public Property AutoCompleteChars As List(Of Char)
    Public Property Filename As String Implements iOnDisk.Filename

    Public ReadOnly Property Name As String Implements iNamed.Name
        Get
            If _name Is Nothing Then
                Return IO.Path.GetFileNameWithoutExtension(Filename)
            Else
                Return _name
            End If
        End Get
    End Property
    Dim _name As String

    Public Overrides Function GetAutoCompleteChars() As IEnumerable(Of Char)
        Return Me.AutoCompleteChars
    End Function

    Public Overrides Function GetAutoCompleteData(CurrentWord As String) As IEnumerable(Of FunctionDocumentation)
        Return (From d In Database Where d.FunctionName.ToLowerInvariant.StartsWith(CurrentWord.ToLowerInvariant) Select d)
    End Function

    Public Overrides Function GetDocumentation(FunctionName As String) As FunctionDocumentation
        Return (From d In Database Where String.Compare(d.FunctionName, FunctionName, True, Globalization.CultureInfo.InvariantCulture) = 0 Select d).FirstOrDefault
    End Function

    Public Overrides Function AdditionalHighlightRules() As HighlightDefinition
        Dim out As New HighlightDefinition
        out.Name = "Function Names"
        out.NamedHighlightColors.Add(New HighlightingStyle() With {.Name = "FunctionName", .Foreground = "#009999"})
        For Each item In GetAutoCompleteData("")
            Dim rule As New HighlightRule
            rule.CaseSensitive = True
            rule.ColorName = "FunctionName"
            rule.Regex = item.FunctionName
            out.Rules.Add(rule)
        Next
        Return out
    End Function

    Public Sub Save() Implements iSavable.Save
        Save(Filename)
    End Sub

    Public Sub Save(Filename As String) Implements ISavableAs.Save
        Dim j As New JsonStructure With {.Database = Me.Database, .AutoCompleteChars = Me.AutoCompleteChars}
        Json.SerializeToFile(Filename, j)
    End Sub

    Public Sub New(Filename As String)
        Me.New
        OpenFile(Filename)
    End Sub
    Public Sub New()
        MyBase.New()
        Database = New List(Of FunctionDocumentation)
        AutoCompleteChars = New List(Of Char)
    End Sub

    Public Sub CreateFile(Name As String) Implements iCreatableFile.CreateFile
        Database = New List(Of FunctionDocumentation)
        AutoCompleteChars = New List(Of Char)
    End Sub

    Public Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
        Dim j = Json.DeserializeFromFile(Of JsonStructure)(Filename)
        Me.Database = j.Database
        Me.AutoCompleteChars = j.AutoCompleteChars
    End Sub

    Public Event FileSaved As iSavable.FileSavedEventHandler Implements iSavable.FileSaved

    Public Function DefaultExtension() As String Implements iSavable.DefaultExtension
        Return ".fdd"
    End Function

End Class
