Namespace Utilities
    ''' <summary>
    ''' Contains methods to help in JSON serialization.
    ''' Currently, this is a wrapper for the Newtonsoft Json library.
    ''' </summary>
    Public Class Json
        ''' <summary>
        ''' Serializes the specified object into a JSON string.
        ''' </summary>
        ''' <param name="ObjectToSerialize">Object to serialize.</param>
        ''' <returns>Json text that represents the given object.</returns>
        Public Shared Function Serialize(ObjectToSerialize As Object) As String
            Return Newtonsoft.Json.JsonConvert.SerializeObject(ObjectToSerialize)
        End Function

        ''' <summary>
        ''' Deserializes the given json string into a new object of type T.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Json">The JSON to deserialize</param>
        ''' <returns></returns>
        Public Shared Function Deserialize(Of T)(Json As String) As T
            Return Newtonsoft.Json.JsonConvert.DeserializeObject(Of T)(Json)
        End Function

        ''' <summary>
        ''' Serializes the given object into JSON and writes it to disk.
        ''' </summary>
        ''' <param name="Filename">Filename to store the JSON.</param>
        ''' <param name="ObjectToSerialize">Object to serialize.</param>
        Public Shared Sub SerializeToFile(Filename As String, ObjectToSerialize As Object)
            IO.File.WriteAllText(Filename, Serialize(ObjectToSerialize))
        End Sub

        ''' <summary>
        ''' Deserializes JSON stored on disk.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Filename">Path to the text file containing a JSON string.</param>
        ''' <returns></returns>
        Public Shared Function DeserializeFromFile(Of T)(Filename As String) As T
            Return Deserialize(Of T)(IO.File.ReadAllText(Filename))
        End Function
    End Class
End Namespace

