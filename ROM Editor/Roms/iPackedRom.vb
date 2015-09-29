Namespace Roms
    Public Interface iPackedRom
        Function Unpack(Optional DestinationDirectory As String = Nothing) As Task
        Function RePack(NewFileName As String) As Task
    End Interface
End Namespace

