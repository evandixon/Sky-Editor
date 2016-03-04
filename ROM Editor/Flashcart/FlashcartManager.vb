Imports System.Collections.ObjectModel

Namespace Flashcart
    Public Class FlashcartManager

        'Public Shared Property AvailableFlashcarts As ObservableCollection(Of GenericFlashcart)

        Public Shared Function GetAvailableFlashcarts() As ObservableCollection(Of GenericFlashcart)
            Dim out As New ObservableCollection(Of GenericFlashcart)
            For Each item In My.Computer.FileSystem.Drives
                If item.DriveType = IO.DriveType.Removable AndAlso IO.File.Exists(IO.Path.Combine(item.RootDirectory.FullName, "info.skyfci")) Then
                    Dim f As New GenericFlashcart
                    f.Initialize(item)
                    out.Add(f)
                End If
            Next
            Return out
        End Function
        Public Shared Function OpenFlashcart(Drive As IO.DriveInfo) As GenericFlashcart
            Dim output As GenericFlashcart = Nothing
            Dim infoFilename As String = IO.Path.Combine(Drive.RootDirectory.FullName, "info.skyfci")
            If IO.File.Exists(infoFilename) Then
                Dim info = FlashcartInfoFile.Open(infoFilename)
                Dim cartType As Type = SkyEditorBase.Utilities.ReflectionHelpers.GetTypeFromName(infoFilename)
                If cartType IsNot Nothing AndAlso cartType.GetConstructor({}) IsNot Nothing Then
                    output = cartType.GetConstructor({}).Invoke({})
                    output.Initialize(Drive)
                Else
                    'The type in the info file is invalid.  Maybe a plugin was uninstalled, maybe the type doesn't supply a default constructor.
                    'To load meta data at least, we'll substitute a new GenericFlashcart.
                    output = New GenericFlashcart
                    output.Initialize(Drive)
                End If
            Else
                'The drive isn't actually a flashcart.  We'll substitute a new GenericFlashcart.
                output = New GenericFlashcart
                output.Initialize(Drive)
            End If
            Return output
        End Function

        'Shared Sub New()
        '    AvailableFlashcarts = GetAvailableFlashcarts()
        '    'Todo: register some kind of drive insertion/removal event
        'End Sub
    End Class

End Namespace