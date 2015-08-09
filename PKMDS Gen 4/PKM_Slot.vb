#Region "LICENSE"
'Copyright 2009 by Codemonkey85
'
'This library is free software: you can redistribute it and/or modify
'it under the terms of the GNU General Public License as published by
'the Free Software Foundation, either version 3 of the License, or
'(at your option) any later version.
'
'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License for more details.
'
'You should have received a copy of the GNU General Public License
'along with this program.  If not, see <http://www.gnu.org/licenses/>.
#End Region

Imports PokemonDSLib.PokemonLib
Imports System.Windows.Forms

Public Class PKM_Slot
    Inherits PictureBox

    Private PKM As New Pokemon

    Public Property _Pokemon() As Pokemon
        Get
            InitializeDictionaries()
            Return PKM
        End Get
        Set(ByVal value As Pokemon)
            PKM = value
            Image = PKM.BoxIcon
        End Set
    End Property

End Class