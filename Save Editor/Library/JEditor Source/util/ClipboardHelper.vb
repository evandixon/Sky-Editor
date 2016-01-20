Namespace skyjed.util


	Public NotInheritable Class ClipboardHelper

		Public Property Shared AvailableDataFlavors As DataFlavor()
			Get
				' the requestor param of getContents is not currently used
				Dim content As Transferable = SystemClipboard.getContents(Nothing)
				If content IsNot Nothing Then
					Return content.TransferDataFlavors
				End If
				Return Nothing
			End Get
		End Property

		Public Property Shared PreferredMimeType As String
			Get
				Dim arr() As DataFlavor = AvailableDataFlavors
				If arr IsNot Nothing AndAlso arr.Length > 0 Then
					Return arr(0).MimeType
				End If
				Return Nothing
			End Get
		End Property

		''' <summary>
		''' Place an object on the clipboard with given mime-type.
		''' Example mime: text/plain; class=java.lang.String; charset=Unicode
		''' Example mime: text/plain; class="[B"; charset=UTF-8
		''' Example mime: application/x-java-serialized-object; class=java.lang.String
		''' </summary>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not allowed in .NET:
'ORIGINAL LINE: public static void setClipboardContents(final Object obj, final String mime)
		Public Shared Sub setClipboardContents(ByVal obj As Object, ByVal mime As String)
			Try
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.awt.datatransfer.DataFlavor dFlavor = new java.awt.datatransfer.DataFlavor(mime);
				Dim dFlavor As New DataFlavor(mime)
				Dim contents As Transferable = New TransferableAnonymousInnerClassHelper(obj, dFlavor)
				SystemClipboard.setContents(contents, New ClipboardOwnerAnonymousInnerClassHelper(contents))
			Catch e As ClassNotFoundException
				' only if not valid class specified in mime
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
			End Try
		End Sub

		Private Class TransferableAnonymousInnerClassHelper
			Inherits Transferable

			Private obj As Object
			Private dFlavor As DataFlavor

			Public Sub New(ByVal obj As Object, ByVal dFlavor As DataFlavor)
				Me.obj = obj
				Me.dFlavor = dFlavor
			End Sub

			Public Overridable Function isDataFlavorSupported(ByVal flavor As DataFlavor) As Boolean
				Return flavor.Equals(dFlavor)
			End Function
			Public Overridable Property TransferDataFlavors As DataFlavor()
				Get
					Return New DataFlavor() { dFlavor }
				End Get
			End Property
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
'ORIGINAL LINE: public Object getTransferData(java.awt.datatransfer.DataFlavor flavor) throws java.awt.datatransfer.UnsupportedFlavorException, java.io.IOException
			Public Overridable Function getTransferData(ByVal flavor As DataFlavor) As Object
				Return obj
			End Function
		End Class

		Private Class ClipboardOwnerAnonymousInnerClassHelper
			Inherits ClipboardOwner

			Private contents As Transferable

			Public Sub New(ByVal contents As Transferable)
				Me.contents = contents
			End Sub

			Public Overridable Sub lostOwnership(ByVal clipboard As Clipboard, ByVal contents As Transferable)
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: The following line could not be converted:
        Public Shared Sub setClipboardString(ByVal src As String) 'JavaToDotNetTempPropertySetClipboardString
		Public Shared Property ClipboardString As String
			Set(ByVal src As String)
				setClipboardContents(src, "text/plain; class=java.lang.String; charset=Unicode")
			End Set
			Get
		End Property

		''' <summary>
		''' Get the object of specified mime-type residing on the clipboard; if none found return null.
		''' Example mime: text/plain; class=java.lang.String; charset=Unicode
		''' Example mime: text/plain; class="[B"; charset=UTF-8
		''' </summary>
		Public Shared Function getClipboardContents(ByVal mime As String) As Object
			Try
				Dim dFlavor As New DataFlavor(mime)
				' the requestor param of getContents is not currently used
				Dim contents As Transferable = SystemClipboard.getContents(Nothing)
				If contents IsNot Nothing AndAlso contents.isDataFlavorSupported(dFlavor) Then
					Return contents.getTransferData(dFlavor)
				End If
			Catch e As UnsupportedFlavorException
				' highly unlikely since we checked it is supported
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
			Catch e As ClassNotFoundException
				' only if not valid class specified in mime
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
			Catch e As IOException
				' we will return null in that case
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
			End Try
			Return Nothing
		End Function

			Return DirectCast(getClipboardContents("text/plain; class=java.lang.String; charset=Unicode"), String)
		End Function

		Private Property Shared SystemClipboard As Clipboard
			Get
				Return Toolkit.DefaultToolkit.SystemClipboard
			End Get
		End Property

	End Class

End Namespace