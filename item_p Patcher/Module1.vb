Imports System.Web.Script.Serialization

Module Module1
    Sub Main()
        'Dim Version = Environment.Version.ToString(2)
        'Console.WriteLine($"PMD: Explorers of Time/Darkness/Sky item_p.bin Patcher v{Version}")
        'Console.WriteLine()
        'Dim args = Environment.GetCommandLineArgs
        'If args.Length >= 4 Then
        '    Dim mode As String = args(1)
        '    Select Case mode.ToLower
        '        Case "-c"
        '            Dim inputFile As String = args(2)
        '            Dim editedFile As String = args(3)
        '            Dim patchFile As String = args(4)

        '            Dim inputLS As New item_p(inputFile)
        '            Dim editedLS As New item_p(editedFile)
        '            Dim edits As New List(Of MyKeyValuePair)
        '            For count As Integer = 0 To Math.Min(inputLS.Items.Count - 1, editedLS.Items.Count - 1)
        '                If Not inputLS(count) = editedLS(count) Then
        '                    edits.Add(New MyKeyValuePair(count, editedLS(count)))
        '                End If
        '            Next

        '            Dim j As New JavaScriptSerializer
        '            IO.File.WriteAllText(patchFile, j.Serialize(edits))

        '            inputLS.Dispose()
        '            editedLS.Dispose()
        '        Case "-a"
        '            Dim inputFile As String = args(2)
        '            Dim patchFile As String = args(3)
        '            Dim editedFile As String = args(4)

        '            Dim j As New JavaScriptSerializer
        '            Dim editedLS As New item_p(inputFile)
        '            Dim edits = j.Deserialize(Of List(Of MyKeyValuePair))(IO.File.ReadAllText(patchFile))
        '            For Each item In edits
        '                editedLS(item.Key) = item.Value
        '            Next

        '            editedLS.Save(editedFile)

        '            editedLS.Dispose()
        '        Case Else
        '            PrintUsage()
        '    End Select
        'Else
        '    PrintUsage()
        'End If
    End Sub

    Sub PrintUsage()
        Console.WriteLine("Usage")
        Console.WriteLine("To create a patch itemppatcher.exe -c <originalFile> <editedFile> <patchFile>")
        Console.WriteLine("To apply a patch  itemppatcher.exe -a <originalFile> <patchFile> <editedFile>")
    End Sub

End Module
