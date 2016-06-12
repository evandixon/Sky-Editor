Imports System.Windows.Input
Imports SkyEditor.Core.UI

Namespace Modeling
    Public Interface IItemSlot
        ReadOnly Property Name As String
        ReadOnly Property MaxItemCount As Integer
        Property ItemCollection As IList
        Property NewItem As Object
        ReadOnly Property AddCommand As RelayCommand
    End Interface
End Namespace

