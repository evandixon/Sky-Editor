Imports System.IO
Imports System.Reflection
Imports System.Windows.Input
Imports SkyEditor.Core
Imports SkyEditor.Core.IO
Imports SkyEditor.Core.UI
Imports SkyEditor.Core.Utilities
Imports SkyEditor.Core.Windows
Imports SkyEditor.UI.WPF.AvalonHelpers
Imports Xceed.Wpf.AvalonDock
Imports Xceed.Wpf.AvalonDock.Layout.Serialization
Namespace AvalonHelpers

    ''' <summary>
    ''' Class implements a viewmodel to support the
    ''' <seealso cref="AvalonDockLayoutSerializer"/>
    ''' attached behavior which is used to implement
    ''' load/save of layout information on application
    ''' start and shut-down.
    ''' </summary>
    Public Class AvalonDockLayoutViewModel
        Private Const AvalonDockLayoutFilename = "AvalonDock.Layout.config"

        Public Sub New(manager As PluginManager)
            Me.CurrentPluginManager = manager
        End Sub
#Region "fields"
        Private mLoadLayoutCommand As RelayCommand = Nothing
        Private mSaveLayoutCommand As RelayCommand = Nothing
        Public Property CurrentPluginManager As PluginManager
#End Region

#Region "command properties"
        ''' <summary>
        ''' Implement a command to load the layout of an AvalonDock-DockingManager instance.
        ''' This layout defines the position and shape of each document and tool window
        ''' displayed in the application.
        ''' 
        ''' Parameter:
        ''' The command expects a reference to a <seealso cref="DockingManager"/> instance to
        ''' work correctly. Not supplying that reference results in not loading a layout (silent return).
        ''' </summary>
        Public ReadOnly Property LoadLayoutCommand() As ICommand
            Get
                If Me.mLoadLayoutCommand Is Nothing Then
                    Me.mLoadLayoutCommand = New RelayCommand(Sub(p As Object)
                                                                 Dim docManager As DockingManager = TryCast(p, DockingManager)

                                                                 If docManager IsNot Nothing Then
                                                                     Me.LoadDockingManagerLayout(docManager)
                                                                 End If

                                                             End Sub)
                End If

                Return Me.mLoadLayoutCommand
            End Get
        End Property

        ''' <summary>
        ''' Implements a command to save the layout of an AvalonDock-DockingManager instance.
        ''' This layout defines the position and shape of each document and tool window
        ''' displayed in the application.
        ''' 
        ''' Parameter:
        ''' The command expects a reference to a <seealso cref="String"/> instance to
        ''' work correctly. The string is supposed to contain the XML layout persisted
        ''' from the DockingManager instance. Not supplying that reference to the string
        ''' results in not saving a layout (silent return).
        ''' </summary>
        Public ReadOnly Property SaveLayoutCommand() As ICommand
            Get
                If Me.mSaveLayoutCommand Is Nothing Then
                    Me.mSaveLayoutCommand = New RelayCommand(Sub(p As Object)
                                                                 Dim xmlLayout As String = TryCast(p, String)

                                                                 If xmlLayout IsNot Nothing Then
                                                                     Me.SaveDockingManagerLayout(xmlLayout)
                                                                 End If

                                                             End Sub)
                End If

                Return Me.mSaveLayoutCommand
            End Get
        End Property
#End Region

#Region "methods"
#Region "LoadLayout"
        ''' <summary>
        ''' Loads the layout of a particular docking manager instance from persistence
        ''' and checks whether a file should really be reloaded (some files may no longer
        ''' be available).
        ''' </summary>
        ''' <param name="docManager"></param>
        Private Sub LoadDockingManagerLayout(docManager As DockingManager)
            Dim layoutFileName As String = Path.Combine(EnvironmentPaths.GetRootResourceDirectory, AvalonDockLayoutFilename)

            If File.Exists(layoutFileName) Then
                Dim LayoutSerializer = New XmlLayoutSerializer(docManager)

                AddHandler LayoutSerializer.LayoutSerializationCallback, Sub(sender As Object, e As LayoutSerializationCallbackEventArgs)
                                                                             ' This can happen if the previous session was loading a file
                                                                             ' but was unable to initialize the view ...
                                                                             If e.Model.ContentId IsNot Nothing Then
                                                                                 ReloadContentOnStartUp(e)
                                                                             Else
                                                                                 e.Cancel = True
                                                                             End If

                                                                         End Sub

                LayoutSerializer.Deserialize(layoutFileName)
            End If

        End Sub

        Private Sub ReloadContentOnStartUp(args As LayoutSerializationCallbackEventArgs)
            Dim sId As String = args.Model.ContentId

            ' Empty Ids are invalid but possible if aaplication is closed with File>New without edits.
            If String.IsNullOrWhiteSpace(sId) = True Then
                args.Cancel = True
                Return
            End If

            If File.Exists(args.Model.ContentId) Then
                'TODO: maybe auto reload previously open files

                'args.Content = AvalonDockLayoutViewModel.ReloadDocument(args.Model.ContentId)

                'If args.Content Is Nothing Then
                '    args.Cancel = True
                'End If
            Else
                Dim targetType = ReflectionHelpers.GetTypeByName(args.Model.ContentId, CurrentPluginManager)
                If targetType IsNot Nothing AndAlso ReflectionHelpers.IsOfType(targetType, GetType(AnchorableViewModel).GetTypeInfo, False) Then
                    Dim model As AnchorableViewModel = ReflectionHelpers.CreateInstance(targetType)
                    model.CurrentIOUIManager = CurrentPluginManager.CurrentIOUIManager
                    args.Content = model
                End If
            End If
        End Sub

        'Private Shared Function ReloadDocument(path As String) As Object
        '    Dim ret As Object = Nothing

        '    If Not String.IsNullOrWhiteSpace(path) Then
        '        Select Case path
        '            Case Else
        '                '**
        '                '          case StartPageViewModel.StartPageContentId: // Re-create start page content
        '                '            if (Workspace.This.GetStartPage(false) == null)
        '                '            {
        '                '              ret = Workspace.This.GetStartPage(true);
        '                '            }
        '                '            break;
        '                '**

        '                ' Re-create text document
        '                ret = Workspace.This.Open(path)
        '                Exit Select
        '        End Select
        '    End If

        '    Return ret
        'End Function
#End Region

#Region "SaveLayout"
        Private Sub SaveDockingManagerLayout(xmlLayout As String)
            ' Create XML Layout file on close application (for re-load on application re-start)
            If xmlLayout IsNot Nothing Then
                Dim fileName As String = Path.Combine(EnvironmentPaths.GetRootResourceDirectory, AvalonDockLayoutFilename)

                File.WriteAllText(fileName, xmlLayout)
            End If
        End Sub
#End Region
#End Region
    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
