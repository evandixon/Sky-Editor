Public Class CiaConversion
    Public Shared Async Function ConvertToCia(Input3DSFilename As String, OutputCiaFilename As String) As Task
        Dim tempDir As String = IO.Path.Combine(IO.Path.GetDirectoryName(Input3DSFilename), "temp")
        If Not IO.Directory.Exists(tempDir) Then
            IO.Directory.CreateDirectory(tempDir)
        End If
        Await ExtractNCCH(Input3DSFilename, tempDir)

        'We're going to fix the exheader before packing as a 3ds
        'FixCxi(tempDir)

        Await MakeCia(tempDir, OutputCiaFilename)

        'If the input 3DS file is encrypted, an extra step is needed.
        'FixCia(OutputCiaFilename)

        If IO.Directory.Exists(tempDir) Then
            IO.Directory.Delete(tempDir, True)
        End If
    End Function

    Private Shared Async Function ExtractNCCH(Input3DSFilename As String, tempDir As String) As Task
        Dim romTool = IO.Path.Combine(Environment.CurrentDirectory, "Tools/rom_tool.exe")
        Await ProcessHelper.RunProgram(romtool, $"-x ""{tempDir}"" ""{Input3DSFilename}""")
    End Function

    Private Shared Sub FixCxi(tempDir As String)
        For Each item In IO.Directory.GetFiles(tempDir, "*.cxi")
            Using f As New IO.FileStream(item, IO.FileMode.Open, IO.FileAccess.ReadWrite)
                Dim exheaderBuffer(&H400 - 1) As Byte

                'read exHeader
                f.Seek(&H200, IO.SeekOrigin.Begin)
                f.Read(exheaderBuffer, 0, exheaderBuffer.Length)

                'Todo: apply xorpad here if supported in the future

                'set SD flag in exheader
                exheaderBuffer(&HD) = exheaderBuffer(&HD) Or 2

                'write exHeader
                f.Seek(&H200, IO.SeekOrigin.Begin)
                f.Write(exheaderBuffer, 0, exheaderBuffer.Length)

                'write exHeader hash
                Dim sha = System.Security.Cryptography.SHA256.Create
                Dim shaBytes = sha.ComputeHash(exheaderBuffer)
                f.Seek(&H160, IO.SeekOrigin.Begin)
                f.Write(shaBytes, 0, shaBytes.Length)
            End Using
        Next
    End Sub

    Private Shared Async Function MakeCia(tempDir As String, OutputCiaFilename As String) As Task
        Dim makeRom = IO.Path.Combine(Environment.CurrentDirectory, "Tools/makerom.exe")
        Dim content As New List(Of String())
        Dim cxiRegex As New Text.RegularExpressions.Regex(".*_(\d)_.*?(?:cxi|cfa)", System.Text.RegularExpressions.RegexOptions.IgnoreCase) 'Original: ^.*?_(\d)_.*?(?:cxi|cfa)$
        For Each item In IO.Directory.GetFiles(tempDir)
            Dim match = cxiRegex.Match(item)
            If match IsNot Nothing AndAlso match.Success Then
                Dim index = match.Groups(1).Value
                If IsNumeric(index) AndAlso CInt(index) < 6 Then
                    content.Add({item.Replace(IO.Path.Combine(Environment.CurrentDirectory, "Tools"), "").TrimStart("\"), index})
                End If
            End If
        Next

        Dim args As New Text.StringBuilder
        args.Append("-f cia -o """)
        args.Append(OutputCiaFilename)
        args.Append("""")
        For Each item In content
            args.Append(" -content """)
            args.Append(item(0))
            args.Append(""":")
            args.Append(item(1))
            args.Append(":")
            args.Append(item(1))
        Next

        Await ProcessHelper.RunProgram(makeRom, args.ToString)
    End Function

    'Private Shared Sub FixCia(OutputCiaFilename As String)
    '
    'End Sub
End Class
