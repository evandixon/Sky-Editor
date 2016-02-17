Namespace UI
    ''' <summary>
    '''
    ''' </summary>
    ''' <remarks>Provided by http://dirk.schuermans.me/?p=585 </remarks>
    Class ChildControls
        Private lstChildren As List(Of Object)

        Public Function GetChildren(p_vParent As Visual, p_nLevel As Integer) As List(Of Object)
            If p_vParent Is Nothing Then
                Throw New ArgumentNullException("Element {0} is null!", p_vParent.ToString())
            End If

            Me.lstChildren = New List(Of Object)()

            Me.GetChildControls(p_vParent, p_nLevel)

            Return Me.lstChildren

        End Function

        Private Sub GetChildControls(p_vParent As Visual, p_nLevel As Integer)
            'Dim nChildCount As Integer = VisualTreeHelper.GetChildrenCount(p_vParent)

            'For i As Integer = 0 To nChildCount - 1
            '    Dim v As Visual = DirectCast(VisualTreeHelper.GetChild(p_vParent, i), Visual)

            '    lstChildren.Add(DirectCast(v, Object))

            '    If VisualTreeHelper.GetChildrenCount(v) > 0 Then
            '        GetChildControls(v, p_nLevel + 1)
            '    End If
            'Next
            For Each v In LogicalTreeHelper.GetChildren(p_vParent)
                lstChildren.Add(v)
                If TypeOf v Is Visual Then
                    GetChildControls(v, p_nLevel + 1)
                End If
            Next
        End Sub
    End Class

    '=======================================================
    'Service provided by Telerik (www.telerik.com)
    'Conversion powered by NRefactory.
    'Twitter: @telerik
    'Facebook: facebook.com/telerik
    '=======================================================
End Namespace
