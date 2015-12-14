Namespace Roms
    Public Interface iPackedRom
        Function Unpack(Optional DestinationDirectory As String = Nothing) As Task
        Function RePack(NewFileName As String) As Task
        ReadOnly Property GameCode As String
    End Interface
End Namespace

