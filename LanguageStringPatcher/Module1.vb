Imports System.Web.Script.Serialization

Module Module1
    Public Class Container
        Public Property Entries As List(Of Entry)
        Public Property SourceLength As Integer 'Used to determine region of source
    End Class
    Public Class Entry
        Public Property Key As Integer
        Public Property Value As String
        Public Sub New()

        End Sub
        Public Sub New(Key As Integer, Value As String)
            Me.Key = Key
            Me.Value = Value
        End Sub
    End Class

    Sub Main()
        Dim Version = Environment.Version.ToString(2)
        Console.WriteLine($"PMD: Explorers of Time/Darkness/Sky text_X.str Patcher v{Version}")
        Console.WriteLine()
        Dim args = Environment.GetCommandLineArgs
        If args.Length >= 4 Then
            Dim mode As String = args(1)
            Select Case mode.ToLower
                Case "-c"
                    Dim inputFile As String = args(2)
                    Dim editedFile As String = args(3)
                    Dim patchFile As String = args(4)

                    Dim inputLS As New LanguageString(inputFile)
                    Dim editedLS As New LanguageString(editedFile)
                    Dim edits As New List(Of Entry)
                    For count As Integer = 0 To Math.Min(inputLS.Items.Count - 1, editedLS.Items.Count - 1)
                        If Not inputLS(count) = editedLS(count) Then
                            edits.Add(New Entry(count, editedLS(count)))
                        End If
                    Next

                    Dim j As New JavaScriptSerializer
                    Dim c As New Container
                    c.Entries = edits
                    c.SourceLength = inputLS.Items.Count
                    IO.File.WriteAllText(patchFile, j.Serialize(c))

                    inputLS.Dispose()
                    editedLS.Dispose()
                Case "-a"
                    Dim inputFile As String = args(2)
                    Dim patchFile As String = args(3)
                    Dim editedFile As String = args(4)

                    Dim j As New JavaScriptSerializer
                    Dim editedLS As New LanguageString(inputFile)
                    Dim c = j.Deserialize(Of Container)(IO.File.ReadAllText(patchFile))
                    Dim edits = c.Entries
                    Dim inputRegion = editedLS.FileRegion
                    Dim patchRegion = LanguageString.GetFileRegion(c.SourceLength)
                    For Each item In edits
                        If inputRegion = LanguageString.Region.US AndAlso patchRegion = LanguageString.Region.Europe Then
                            editedLS(LanguageString.ConvertEUToUS(item.Key)) = item.Value
                        ElseIf inputRegion = LanguageString.Region.Europe AndAlso patchRegion = LanguageString.Region.US
                            editedLS(LanguageString.ConvertUSToEU(item.Key)) = item.Value
                        Else
                            editedLS(item.Key) = item.Value
                        End If
                    Next

                    editedLS.Save(editedFile)

                    editedLS.Dispose()
                Case Else
                    PrintUsage()
            End Select
        Else
            PrintUsage()
        End If
    End Sub

    Sub PrintUsage()
        Console.WriteLine("Usage:")
        Console.WriteLine("To create a patch: LanguageStringPatcher.exe -c <originalFile> <editedFile> <patchFile>")
        Console.WriteLine("To apply a patch:  LanguageStringPatcher.exe -a <originalFile> <patchFile> <editedFile>")
    End Sub
End Module
