Imports System.Drawing

Module Module1

    Sub Main()
        Const dir = "C:\TFS\Sky Editor\skyeditor\Sky Editor Base\bin\Debug\Resources\Plugins\ROMEditor\Current\data\BACK\Decompressed"
        If Not IO.Directory.Exists("C:\TFS\Sky Editor\skyeditor\Sky Editor Base\bin\Debug\Resources\Plugins\ROMEditor\Current\data\BACK\Decompressed\Converted") Then
            IO.Directory.CreateDirectory("C:\TFS\Sky Editor\skyeditor\Sky Editor Base\bin\Debug\Resources\Plugins\ROMEditor\Current\data\BACK\Decompressed\Converted")
        End If
        For Each file In IO.Directory.GetFiles(dir, "*.png")
            Console.WriteLine("Converting " & file)
            Dim i As New Bitmap(file)
            Dim b As ROMEditor.FileFormats.BGP = ROMEditor.FileFormats.BGP.ConvertFromBitmap(i)
            IO.File.WriteAllBytes(file.Replace(dir, dir & "\Converted").Replace(".png", ".rawimg"), b.RawData)
        Next
    End Sub

End Module
