Imports System.Collections.Generic

Namespace skyjed.util



	Public Class ArrayListModel(Of E)
		Inherits AbstractListModel(Of E)
		Implements ComboBoxModel(Of E)

		Friend values As IList(Of E)
		Private selectedItem As E

		Public Sub New(ByVal values As IList(Of E))
			Me.values = values
		End Sub

		Public Sub New(ByVal values() As E)
			Me.values = Arrays.asList(values)
		End Sub

		Public Overridable Function getElementAt(ByVal index As Integer) As E
			Return values(index)
		End Function

		Public Overridable Property Size As Integer
			Get
				Return values.Count
			End Get
		End Property

		Public Overridable Property SelectedItem As Object
			Get
				Return selectedItem
			End Get
			Set(ByVal anItem As Object)
				selectedItem = DirectCast(anItem, E)
				Me.fireContentsChanged(Me, -1, -1)
			End Set
		End Property


	End Class

End Namespace