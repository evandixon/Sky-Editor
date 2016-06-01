
' Copyright Christophe Bertrand.
' Downloaded from https://chrisbertrandprogramer.wordpress.com/2013/07/22/a-parser-for-assemblyqualifiedname-for-net/
' Slightly modified by Evan Dixon

Imports System.Collections.Generic
Imports System.Linq
Imports System.Reflection
Imports System.Text

Namespace Utilities
    Public Class ParsedAssemblyQualifiedName
        Public AssemblyNameDescriptor As Lazy(Of AssemblyName)
        Public FoundType As Lazy(Of Type)
        Public ReadOnly AssemblyDescriptionString As String
        Public ReadOnly TypeName As String

        ''' <summary>
        ''' The name of the base type, if this is a generic type.  Otherwise, equal to TypeName.
        ''' </summary>
        Public ReadOnly GenericTypeName As String
        Public ReadOnly IsGenericType As Boolean
        Public ReadOnly ShortAssemblyName As String
        Public ReadOnly Version As String
        Public ReadOnly Culture As String
        Public ReadOnly PublicKeyToken As String
        Public ReadOnly GenericParameters As New List(Of ParsedAssemblyQualifiedName)()
        Public ReadOnly CSharpStyleName As Lazy(Of String)
        Public ReadOnly VBNetStyleName As Lazy(Of String)
        Public ReadOnly AssemblyQualifiedName As String

        Public Sub New(AssemblyQualifiedName As String, Assemblies As IEnumerable(Of Assembly), manager As PluginManager)
            Me.AssemblyQualifiedName = AssemblyQualifiedName

            Dim index As Integer = -1
            Dim rootBlock As New block()
            If True Then
                Dim bcount As Integer = 0
                Dim currentBlock As block = rootBlock
                For i As Integer = 0 To AssemblyQualifiedName.Length - 1
                    Dim c As Char = AssemblyQualifiedName(i)
                    If c = "["c Then
                        bcount += 1
                        Dim b = New block() With {
                            .iStart = i + 1,
                            .level = bcount,
                            .parentBlock = currentBlock
                        }
                        currentBlock.innerBlocks.Add(b)
                        currentBlock = b
                    ElseIf c = "]"c Then
                        currentBlock.iEnd = i - 1
                        If AssemblyQualifiedName(currentBlock.iStart) <> "["c Then
                            currentBlock.parsedAssemblyQualifiedName = New ParsedAssemblyQualifiedName(AssemblyQualifiedName.Substring(currentBlock.iStart, i - currentBlock.iStart), Assemblies, manager)
                            If bcount = 2 Then
                                Me.GenericParameters.Add(currentBlock.parsedAssemblyQualifiedName)
                            End If
                        End If
                        currentBlock = currentBlock.parentBlock
                        bcount -= 1
                    ElseIf bcount = 0 AndAlso c = ","c Then
                        index = i
                        Exit For
                    End If
                Next
            End If

            Me.TypeName = AssemblyQualifiedName.Substring(0, index)

            If Me.TypeName.Contains("`") Then
                Me.GenericTypeName = Me.TypeName.Substring(0, Me.TypeName.IndexOf("`"c) + 2)
                IsGenericType = True
            Else
                Me.GenericTypeName = Me.TypeName
                IsGenericType = False
            End If


            Me.CSharpStyleName = New Lazy(Of String)(Function()
                                                         Return Me.LanguageStyle("<", ">")

                                                     End Function)

            Me.VBNetStyleName = New Lazy(Of String)(Function()
                                                        Return Me.LanguageStyle("(Of ", ")")

                                                    End Function)

            Me.AssemblyDescriptionString = AssemblyQualifiedName.Substring(index + 2)

            If True Then
                Dim parts As List(Of String) = AssemblyDescriptionString.Split(","c).[Select](Function(x) x.Trim()).ToList()
                Me.Version = LookForPairThenRemove(parts, "Version")
                Me.Culture = LookForPairThenRemove(parts, "Culture")
                Me.PublicKeyToken = LookForPairThenRemove(parts, "PublicKeyToken")
                If parts.Count > 0 Then
                    Me.ShortAssemblyName = parts(0)
                End If
            End If

            Me.AssemblyNameDescriptor = New Lazy(Of AssemblyName)(Function() New System.Reflection.AssemblyName(Me.AssemblyDescriptionString))

            Me.FoundType = New Lazy(Of Type)(Function() As Type
                                                 Dim searchedType = Type.GetType(TypeName)
                                                 If searchedType IsNot Nothing Then
                                                     Return searchedType
                                                 End If
                                                 For Each assem In Assemblies
                                                     If IsGenericType Then
                                                         searchedType = assem.GetType(GenericTypeName)
                                                         If searchedType IsNot Nothing Then
                                                             Dim types As New List(Of Type)
                                                             For Each item In GenericParameters
                                                                 types.Add(ReflectionHelpers.GetTypeByName(item.AssemblyQualifiedName, manager).AsType)
                                                             Next
                                                             Return searchedType.MakeGenericType(types.ToArray)
                                                         End If
                                                     Else
                                                         searchedType = assem.GetType(GenericTypeName)
                                                         If searchedType IsNot Nothing Then
                                                             Return searchedType
                                                         End If
                                                     End If

                                                 Next
                                                 ' Not found.
                                                 Return Nothing

                                             End Function)
        End Sub

        Friend Function LanguageStyle(prefix As String, suffix As String) As String
            If Me.GenericParameters.Count > 0 Then
                Dim sb As New StringBuilder(Me.TypeName.Substring(0, Me.TypeName.IndexOf("`"c)))
                sb.Append(prefix)
                Dim pendingElement As Boolean = False
                For Each param In Me.GenericParameters
                    If pendingElement Then
                        sb.Append(", ")
                    End If
                    sb.Append(param.LanguageStyle(prefix, suffix))
                    pendingElement = True
                Next
                sb.Append(suffix)
                Return sb.ToString()
            Else
                Return Me.TypeName
            End If
        End Function
        Private Class block
            Friend iStart As Integer
            Friend iEnd As Integer
            Friend level As Integer
            Friend parentBlock As block
            Friend innerBlocks As New List(Of block)()
            Friend parsedAssemblyQualifiedName As ParsedAssemblyQualifiedName
        End Class

        Private Shared Function LookForPairThenRemove(strings As List(Of String), Name As String) As String
            For istr As Integer = 0 To strings.Count - 1
                Dim s As String = strings(istr)
                Dim i As Integer = s.IndexOf(Name)
                If i = 0 Then
                    Dim i2 As Integer = s.IndexOf("="c)
                    If i2 > 0 Then
                        Dim ret As String = s.Substring(i2 + 1)
                        strings.RemoveAt(istr)
                        Return ret
                    End If
                End If
            Next
            Return Nothing
        End Function

#If DEBUG Then
        ' Makes debugging easier.
        Public Overrides Function ToString() As String
            Return Me.CSharpStyleName.ToString()
        End Function
#End If
    End Class



End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
