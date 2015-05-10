' Tamir Khason http://khason.net/
'
' Released under MS-PL : 6-Apr-09

Imports System.Collections
Imports System.IO
Imports System.Security.Cryptography
Imports System.Text

''' <summary>Implements a 16-bits cyclic redundancy check (CRC) hash algorithm.</summary>
''' <remarks>This class is not intended to be used for security purposes. For security applications use MD5, SHA1, SHA256, SHA384,
''' or SHA512 in the System.Security.Cryptography namespace.</remarks>
Public Class CRC16
    Inherits HashAlgorithm

#Region "CONSTRUCTORS"
    ''' <summary>Creates a CRC16 object using the <see cref="DefaultPolynomial"/>.</summary>
    Public Sub New()
        Me.New(DefaultPolynomial)
    End Sub

    ''' <summary>Creates a CRC16 object using the specified polynomial.</summary>
    <CLSCompliant(False)> _
    Public Sub New(polynomial As UShort)
        HashSizeValue = 16
        _crc16Table = DirectCast(_crc16TablesCache(polynomial), UShort())
        If _crc16Table Is Nothing Then
            _crc16Table = CRC16._buildCRC16Table(polynomial)
            _crc16TablesCache.Add(polynomial, _crc16Table)
        End If
        Initialize()
    End Sub

    ' static constructor
    Shared Sub New()
        _crc16TablesCache = Hashtable.Synchronized(New Hashtable())
        _defaultCRC = New CRC16()
    End Sub
#End Region

#Region "PROPERTIES"
    ''' <summary>Gets the default polynomial.</summary>
    <CLSCompliant(False)> _
    Public Shared ReadOnly DefaultPolynomial As UShort = &H8408 '&H1021 '&H8408
    ' Bit reversion of 0xA001;
#End Region
#Region "METHODS"
    ''' <summary>Initializes an implementation of HashAlgorithm.</summary>
    Public Overrides Sub Initialize()
        _crc = 0 '&HFFFF '0
    End Sub

    ''' <summary>Routes data written to the object into the hash algorithm for computing the hash.</summary>
    Protected Overrides Sub HashCore(buffer As Byte(), offset As Integer, count As Integer)
        For i As Integer = 0 To buffer.Length - 1
            Dim index As Byte = CByte(_crc Xor buffer(i))
            _crc = CUShort((_crc >> 8) Xor _crc16Table(index))
        Next
    End Sub

    ''' <summary>Finalizes the hash computation after the last data is processed by the cryptographic stream object.</summary>
    Protected Overrides Function HashFinal() As Byte()
        Dim finalHash As Byte() = New Byte(1) {}
        Dim finalCRC As UShort = CUShort(_crc Xor _allOnes)

        finalHash(0) = CByte((finalCRC >> 0) And &HFF)
        finalHash(1) = CByte((finalCRC >> 8) And &HFF)

        Return finalHash
    End Function

    ''' <summary>Computes the CRC16 value for the given ASCII string using the <see cref="DefaultPolynomial"/>.</summary>
    Public Shared Function Compute(asciiString As String) As Short
        _defaultCRC.Initialize()
        Return ToInt16(_defaultCRC.ComputeHash(asciiString))
    End Function

    ''' <summary>Computes the CRC16 value for the given input stream using the <see cref="DefaultPolynomial"/>.</summary>
    Public Shared Function Compute(inputStream As Stream) As Short
        _defaultCRC.Initialize()
        Return ToInt16(_defaultCRC.ComputeHash(inputStream))
    End Function

    ''' <summary>Computes the CRC16 value for the input data using the <see cref="DefaultPolynomial"/>.</summary>
    Public Shared Function Compute(buffer As Byte()) As Short
        _defaultCRC.Initialize()
        Return ToInt16(_defaultCRC.ComputeHash(buffer))
    End Function

    ''' <summary>Computes the hash value for the input data using the <see cref="DefaultPolynomial"/>.</summary>
    Public Shared Function Compute(buffer As Byte(), offset As Integer, count As Integer) As Short
        _defaultCRC.Initialize()
        Return ToInt16(_defaultCRC.ComputeHash(buffer, offset, count))
    End Function

    ''' <summary>Computes the hash value for the given ASCII string.</summary>
    ''' <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
    Public Shadows Function ComputeHash(asciiString As String) As Byte()
        Dim rawBytes As Byte() = ASCIIEncoding.ASCII.GetBytes(asciiString)
        Return ComputeHash(rawBytes)
    End Function

    ''' <summary>Computes the hash value for the given input stream.</summary>
    ''' <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
    Public Shadows Function ComputeHash(inputStream As Stream) As Byte()
        Dim buffer As Byte() = New Byte(4095) {}
        Dim bytesRead As Integer
        While (InlineAssignHelper(bytesRead, inputStream.Read(buffer, 0, 4096))) > 0
            HashCore(buffer, 0, bytesRead)
        End While
        Return HashFinal()
    End Function

    ''' <summary>Computes the hash value for the input data.</summary>
    ''' <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
    Public Shadows Function ComputeHash(buffer As Byte()) As Byte()
        Return ComputeHash(buffer, 0, buffer.Length)
    End Function

    ''' <summary>Computes the hash value for the input data.</summary>
    ''' <remarks>The computation preserves the internal state between the calls, so it can be used for computation of a stream data.</remarks>
    Public Shadows Function ComputeHash(buffer As Byte(), offset As Integer, count As Integer) As Byte()
        HashCore(buffer, offset, count)
        Return HashFinal()
    End Function
#End Region

#Region "PRIVATE SECTION"
    Private Shared _allOnes As UShort = &HFFFF
    Private Shared _defaultCRC As CRC16
    Private Shared _crc16TablesCache As Hashtable
    Private _crc16Table As UShort()
    Private _crc As UShort

    ' Builds a crc16 table given a polynomial
    Private Shared Function _buildCRC16Table(polynomial As UShort) As UShort()
        ' 256 values representing ASCII character codes.
        Dim table As UShort() = New UShort(255) {}
        For i As UShort = 0 To table.Length - 1
            Dim value As UShort = 0
            Dim temp As UShort = i
            For j As Byte = 0 To 7
                If ((value Xor temp) And &H1) <> 0 Then
                    value = CUShort((value >> 1) Xor polynomial)
                Else
                    value >>= 1
                End If
                temp >>= 1
            Next
            table(i) = value
        Next
        Return table
    End Function

    Private Shared Function ToInt16(buffer As Byte()) As Short
        Return BitConverter.ToInt16(buffer, 0)
    End Function
    Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
        target = value
        Return value
    End Function
#End Region

End Class

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================