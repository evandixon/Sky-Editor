Imports System.Reflection

Namespace IO
    Public Class ObjectFileDetector
        Implements IFileTypeDetector

        Public Function DetectFileType(File As GenericFile, Manager As PluginManager) As Task(Of IEnumerable(Of FileTypeDetectionResult)) Implements IFileTypeDetector.DetectFileType
            Dim out As New List(Of FileTypeDetectionResult)
            If File.Length > 0 AndAlso File.RawData(0) = &H7B Then 'Check to see if the first character is "{".  Otherwise, we could try to open a 500+ MB file which takes much more RAM than we need.
                Dim result = TryGetObjectFileType(File.OriginalFilename, Manager)
                If result IsNot Nothing Then
                    out.Add(New FileTypeDetectionResult With {.FileType = result, .MatchChance = 1})
                End If
            End If
            Return Task.FromResult(DirectCast(out, IEnumerable(Of FileTypeDetectionResult)))
        End Function

        ''' <summary>
        ''' If the given file is of type ObjectFile, returns the contained Type.
        ''' Otherwise, returns Nothing.
        ''' </summary>
        ''' <param name="Filename"></param>
        ''' <returns></returns>
        Private Function TryGetObjectFileType(Filename As String, Manager As PluginManager) As Reflection.TypeInfo
            Try
                Dim f As New ObjectFile(Of Object)(Manager.CurrentIOProvider, Filename)
                'Doesn't work for ObjectFiles
                Return Utilities.ReflectionHelpers.GetTypeByName(f.ContainedTypeName, Manager) 'GetType(ObjectFile(Of Object)).GetGenericTypeDefinition.MakeGenericType({Type.GetType(f.ContainedTypeName, AddressOf AssemblyResolver, AddressOf TypeResolver, False)})
            Catch ex As Exception
                Return Nothing
            End Try
        End Function
    End Class
End Namespace

