Imports System.Runtime.CompilerServices
Imports SkyEditorBase
Imports SkyEditorBase.Utilities

Module ByteExtensions
    <Extension()> _
    Public Function Bit1(ByVal aByte As Byte) As Boolean
        Return CType(aByte, Bits8).Bit1
    End Function
    <Extension()> _
    Public Function Bit2(ByVal aByte As Byte) As Boolean
        Return CType(aByte, Bits8).Bit2
    End Function
    <Extension()> _
    Public Function Bit3(ByVal aByte As Byte) As Boolean
        Return CType(aByte, Bits8).Bit3
    End Function
    <Extension()> _
    Public Function Bit4(ByVal aByte As Byte) As Boolean
        Return CType(aByte, Bits8).Bit4
    End Function
    <Extension()> _
    Public Function Bit5(ByVal aByte As Byte) As Boolean
        Return CType(aByte, Bits8).Bit5
    End Function
    <Extension()> _
    Public Function Bit6(ByVal aByte As Byte) As Boolean
        Return CType(aByte, Bits8).Bit6
    End Function
    <Extension()> _
    Public Function Bit7(ByVal aByte As Byte) As Boolean
        Return CType(aByte, Bits8).Bit7
    End Function
    <Extension()> _
    Public Function Bit8(ByVal aByte As Byte) As Boolean
        Return CType(aByte, Bits8).Bit8
    End Function
End Module