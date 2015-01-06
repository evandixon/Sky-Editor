Imports System.Runtime.Serialization

Namespace Utilities
    <Serializable()>
    Public Class ResourceDictionary
        Inherits Dictionary(Of String, String)
        Public Sub New(ResourceName As String)
            MyBase.New()
            If ResourceName.Contains("&L;") Then
                ResourceName = ResourceName.Replace("&L;", Lists.CurrentLanguage)
            End If
            If IO.File.Exists(IO.Path.Combine(Environment.CurrentDirectory, "Resources\" & ResourceName)) Then
                Dim lines As String() = IO.File.ReadAllLines(IO.Path.Combine(Environment.CurrentDirectory, "Resources\" & ResourceName))
                For Each line In lines
                    If Not String.IsNullOrEmpty(line) AndAlso Not line.StartsWith("#") Then
                        Dim p As String() = line.Split("=".ToCharArray, 2)
                        If Not Me.ContainsKey(FormatString(p(0))) Then
                            Me.Add(FormatString(p(0)), FormatString(p(1)))
                        End If
                    End If
                Next
            End If
        End Sub
        Protected Sub New(info As SerializationInfo, context As StreamingContext)
            MyBase.New(info, context)
        End Sub
        Shared Function FormatString(Input As String) As String
            Return Input.Replace("<br/>", vbCrLf)
        End Function
    End Class
End Namespace