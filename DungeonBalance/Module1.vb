Module Module1

    Sub Main()
        Dim args = Environment.GetCommandLineArgs
        If args.Length >= 3 Then
            Dim mode As String = args(1)
            Select Case mode.ToLower
                Case "-extract"
                    Dim inputFile As String = args(2)
                    Dim outputPath As String = args(3)

                    If Not IO.Directory.Exists(outputPath) Then
                        IO.Directory.CreateDirectory(outputPath)
                    End If

                    Using bin As New SkyEditorBase.GenericFile
                        bin.EnableInMemoryLoad = True
                        bin.OpenFile(inputFile)

                        Using ent As New SkyEditorBase.GenericFile
                            ent.EnableInMemoryLoad = True
                            ent.OpenFile(IO.Path.Combine(IO.Path.GetDirectoryName(inputFile), IO.Path.GetFileNameWithoutExtension(inputFile) & ".ent"))

                            For count = 0 To (ent.Length / 4 - 2)
                                Dim startIndex = ent.Int32(count * 4)
                                Dim endIndex = ent.Int32((count + 1) * 4)

                                IO.File.WriteAllBytes(IO.Path.Combine(outputPath, count), bin.RawData(startIndex, endIndex - startIndex))
                                Console.WriteLine("Extracted " & count)
                            Next
                        End Using
                    End Using
            End Select
        End If
    End Sub

End Module
