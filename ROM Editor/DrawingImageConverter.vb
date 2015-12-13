Imports System.Drawing.Imaging
Imports System.Globalization
Imports System.IO
Imports System.Windows.Data

'''
''' One-way converter from System.Drawing.Image to System.Windows.Media.ImageSource
''' 
''' Copied from http://stevecooper.org/2010/08/06/databinding-a-system-drawing-image-into-a-wpf-system-windows-image/
''' Written by Matt Galbraith of Microsoft
''' Date copied: 12-13-2015
'''
<ValueConversion(GetType(System.Drawing.Image), GetType(System.Windows.Media.ImageSource))>
Public Class ImageConverter
    Implements IValueConverter
    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        ' empty images are empty…
        If value Is Nothing Then
            Return Nothing
        End If

        Dim image = DirectCast(value, System.Drawing.Image)
        ' Winforms Image we want to get the WPF Image from…
        Dim bitmap = New System.Windows.Media.Imaging.BitmapImage()
        bitmap.BeginInit()
        Dim memoryStream As New MemoryStream()
        ' Save to a memory stream…
        image.Save(memoryStream, ImageFormat.Png)
        ' Rewind the stream…
        memoryStream.Seek(0, System.IO.SeekOrigin.Begin)
        bitmap.StreamSource = memoryStream
        bitmap.EndInit()
        Return bitmap
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException
    End Function
End Class

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
