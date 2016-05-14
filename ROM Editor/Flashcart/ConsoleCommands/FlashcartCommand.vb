Imports SkyEditor.Core.ConsoleCommands
Imports SkyEditorBase

Namespace Flashcart.ConsoleCommands
    Public Class FlashcartCommand
        Inherits ConsoleCommand

        Public Overrides Sub Main(Arguments() As String)
            Dim doExit As Boolean = False
            Dim selectedDisk As IO.DriveInfo = Nothing
            While Not doExit
                Dim commandList = Console.ReadLine.Split(" ".ToCharArray, 2)
                Select Case commandList(0).ToLower
                    Case "list-cart"
                        Dim carts = FlashcartManager.GetAvailableFlashcarts
                        For Each item In carts
                            Console.WriteLine($"{item.Drive.RootDirectory} ({item.Name})")
                        Next
                    Case "list-drive"
                        Dim disks = GetDisks()
                        For Each item In disks
                            Console.WriteLine($"{item.Name} ({item.VolumeLabel})")
                        Next
                    Case "select-drive"
                        Dim disks = GetDisks()
                        For Each item In disks
                            If item.Name.ToLower = commandList(1).ToLower Then
                                selectedDisk = item
                                Exit For
                            End If
                        Next
                        If selectedDisk Is Nothing Then
                            Console.WriteLine("Disk not found")
                        End If
                    Case "unselect-drive"
                        selectedDisk = Nothing
                        Console.WriteLine("Disk unselected")
                    Case "list-type"
                        For Each item In PluginManager.GetInstance.GetRegisteredTypes(GetType(GenericFlashcart))
                            Console.WriteLine(item.AssemblyQualifiedName)
                        Next
                    Case "create-cart"
                        If selectedDisk IsNot Nothing Then
                            Dim t = Utilities.ReflectionHelpers.GetTypeFromName(commandList(1))
                            If t IsNot Nothing Then
                                FlashcartManager.CreateFlashcart(selectedDisk, t)
                            Else
                                Console.WriteLine("Type not found")
                            End If
                        Else
                            Console.WriteLine("No disk selected")
                        End If
                    Case "exit"
                        doExit = True
                End Select
            End While
        End Sub

        Private Function GetDisks() As List(Of IO.DriveInfo)
            Dim disks As New List(Of IO.DriveInfo)
            For Each item In My.Computer.FileSystem.Drives
                If item.DriveType = IO.DriveType.Removable Then
                    disks.Add(item)
                End If
            Next
            Return disks
        End Function

        Public Overrides ReadOnly Property CommandName As String
            Get
                Return "flashcarts"
            End Get
        End Property
    End Class
End Namespace

