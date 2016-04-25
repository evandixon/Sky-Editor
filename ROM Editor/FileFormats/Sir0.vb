Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats
    Public Class Sir0
        Inherits GenericFile
        Implements iOpenableFile

        ''' <summary>
        ''' The byte used to pad blocks that aren't divisible by 0x10.
        ''' </summary>
        ''' <returns></returns>
        Protected Property PaddingByte As Byte

        ''' <summary>
        ''' Offset of the sub header
        ''' </summary>
        ''' <returns></returns>
        Protected Property HeaderOffset As Integer

        ''' <summary>
        ''' Offset of the pointers block
        ''' </summary>
        ''' <returns></returns>
        Protected Property PointerOffset As Integer

        ''' <summary>
        ''' Length of the pointers block
        ''' </summary>
        ''' <returns></returns>
        Private ReadOnly Property PointerLength As Integer
            Get
                Return Length - PointerOffset
            End Get
        End Property

        ''' <summary>
        ''' Length of the sub header
        ''' </summary>
        ''' <returns></returns>
        Private ReadOnly Property HeaderLength As Integer
            Get
                Return PointerOffset - HeaderOffset
            End Get
        End Property

        ''' <summary>
        ''' Length of the data block
        ''' </summary>
        ''' <returns></returns>
        Private ReadOnly Property DataLength As Integer
            Get
                Return Length - 16 - HeaderLength - PointerLength
            End Get
        End Property

        ''' <summary>
        ''' Contents of the sub header
        ''' </summary>
        ''' <returns></returns>
        Protected Property Header As Byte()

        ''' <summary>
        ''' The decoded pointers in the pointers block.
        ''' Each number is the number of bytes after the previous pointer in the file.
        ''' </summary>
        ''' <returns></returns>
        Public Property RelativePointers As List(Of Integer)

        ''' <summary>
        ''' Whether or not to trim the pointers and sub header blocks after the file is loaded.
        ''' If true, it is easier to append data to the data block.
        ''' </summary>
        ''' <returns></returns>
        Protected Property ResizeFileOnLoad As Boolean

        Public Overrides Sub CreateFile(Name As String, FileContents() As Byte)
            MyBase.CreateFile(Name, FileContents)

            If FileContents.Length > 0 Then
                ProcessData()
            End If
        End Sub

        Public Overrides Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            MyBase.OpenFile(Filename)
            ProcessData()
        End Sub

        Public Overrides Sub Save(Destination As String)
            'The header and relative pointers must be set by child classes

            Me.RawData(0, 4) = {&H53, &H49, &H52, &H30}

            'Update subheader length
            Dim oldLength = Me.Length 'the new header offset
            Me.Length += Me.Header.Length 'Change the file length
            Me.Int32(&H4) = oldLength 'Update the header pointer
            Me.HeaderOffset = oldLength

            'Update subHeader
            RawData(HeaderOffset, Header.Length) = Header

            'Pad the footer
            While Not Length Mod 16 = 0
                Length += 1
            End While

            'Write the pointers
            Dim pointerSection As New List(Of Byte)
            For Each item In RelativePointers
                If item < 128 Then 'If the most significant bit is not 1
                    pointerSection.Add(CByte(item))
                Else
                    Dim workingBytes As New List(Of Byte)
                    Dim workingItem = item

                    workingBytes.Add(workingItem And &H7F)
                    workingItem = workingItem >> 7

                    While workingItem > 0
                        workingBytes.Add((workingItem And &H7F) Or &H80)
                        workingItem = workingItem >> 7
                    End While

                    For count = workingBytes.Count - 1 To 0 Step -1
                        pointerSection.Add(workingBytes(count))
                    Next
                End If
            Next

            oldLength = Me.Length
            Me.Length += pointerSection.Count
            Me.Int32(&H8) = oldLength
            Me.PointerOffset = oldLength
            Me.RawData(oldLength, pointerSection.Count) = pointerSection.ToArray

            While Not Length Mod 16 = 0
                Length += 1
            End While

            MyBase.Save(Destination)

            'Saving multiple times like this will make the second time fail, because the file length is changing.  
            'To change it back to a good working size, we'll reload the SIR0 portions.
            ProcessData()
        End Sub

        Private Sub ProcessData()
            RelativePointers = New List(Of Integer)
            HeaderOffset = Me.Int32(&H4)
            PointerOffset = Me.Int32(&H8)
            Header = RawData(HeaderOffset, HeaderLength)

            Dim isConstructing As Boolean = False
            Dim constructedPointer As Integer = 0
            For count = PointerOffset To Length - 1
                Dim current = RawData(count)
                If current >= 128 Then 'if the most significant bit is 1
                    isConstructing = True
                    constructedPointer = constructedPointer << 7 Or (current And &H7F)
                Else
                    If isConstructing Then
                        constructedPointer = constructedPointer << 7 Or (current And &H7F)
                        RelativePointers.Add(constructedPointer)
                        isConstructing = False
                        constructedPointer = 0
                    Else
                        If current = 0 Then
                            Exit For
                        Else
                            RelativePointers.Add(current)
                        End If
                    End If
                End If
            Next

            'Remove the header and pointer sections, because it will be reconstructed on save
            If Not Me.IsReadOnly AndAlso ResizeFileOnLoad Then Me.Length = Me.Length - Me.PointerLength - Me.HeaderLength
        End Sub

        Public Sub New()
            MyBase.New
            PaddingByte = &H0
            ResizeFileOnLoad = True
            RelativePointers = New List(Of Integer)
        End Sub
    End Class
End Namespace

