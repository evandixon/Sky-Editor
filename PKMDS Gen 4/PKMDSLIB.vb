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

#Region "Imports"
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Drawing.Drawing2D
Imports System
Imports System.Collections.Generic
Imports System.Text
#End Region
''' <summary>
''' A class library for viewing and editing Pokémon DS save data.
''' </summary>
''' <remarks></remarks>
Public Class PokemonLib

#Region "NOTES"
    '[02:36:31] SCV: what's the offset for the dex info in platinum
    '[02:37:06] Jiggy-Ninja: In DP, FE CA EF BE should show up at 0x12DC
    '[02:37:26] SCV: ah ok
    '[02:37:31] Jiggy-Ninja: In Pl, it's at 0x1328
    '
    '        <FieldOffset(&H0)> _
    '
    'PT General footer: located at 0xCF18
    'general size: CF2Ch
    '
    'PT Storage footer: located at 0x1F0FC
    'storage size: 121E4h
    '
    'The 62 bytes at 0x12E0 store which Pokemon you have captured. Each bit represents one Pokemon, in National order. The bit is 0b normally, and set to 1b when the Pokemon is caught.
    '
    'The 62 bytes at 0x1320 store which Pokemon you have seen. Structure is the same as "Captured".
    '
    'The 62 bytes at 0x1360 and 0x13A0 store which genders you have unlocked for each Pokemon. This is used to determine what shows up in the forme viewer for Pokemon that are not among the Special Cases. For the Special cases Burmy, Wormadam, Spinda, Gastrodon, and Shellos, gender information is stored here exactly the same as other Pokemon, even though it isn't used for anything.
    '
    'The first set of 62 bytes determines what the default form is that is shown in the Pokedex for Pokemon that are not among the Special Cases. Structure is the same as "Seen" and "Caught". The bit is set to 0b if Male is the default form, and 1b if Female is the default form.
    '
    'The second set of 62 bytes determines if the other gender has been unlocked. If only the first gender of a Pokemon has been seen, the bit in this section is identical to the corresponding bit in the first section. If both genders have been seen, the bit is inverted.
    '
    'Form Viewer Flag: 0x1404 (Note that this is not in the same place relative to the other flags as it is in Plat.)
    'Language Extension Flag: 0x1413
    'Sinnoh Dex Flag: 0x1414
    'National Dex Flag: 0x1415
    '
    'Captured: 0x12E0
    'Seen: 0x1320
    'First Gender Area: 0x1360
    'Second Gender Area: 0x13A0
    '
    'Special forms:
    'Unown: 0x13E8
    'Spinda: 0x13E0
    'Shellos: 0x13E4
    'Gastrodon: 0x13E5
    'Burmy: 0x13E6
    'Wormadam: 0x13E7
    'Deoxys Byte 1: 0x131F
    'Deoxys Byte 2: 0x135F
    '
    'Languages
    '1 byte per Pokemon, from 0x1405 to 0x1412, in the following order:
    '
    'Ekans, Pikachu, Psyduck, Ponyta, Staryu, Magikarp, Wobbuffet, Heracross, Sneasel, Teddiursa, Houndour, Wingull, Slakoth, Roselia.
    '
    'Language flags are structured the same in the byte as in Plat.
    'Bit 0: Japanese
    'Bit 1: English
    'Bit 2: French
    'Bit 3: German
    'Bit 4: Italian
    'Bit 5: Spanish
    'Bit 6: Unknown/Nothing
    'Bit 7: Unknown/Nothing
#End Region

#Region "Pokémon Database Dictionaries"

    ''' <summary>
    ''' Retrieves a dictionary of Pokémon base stats based on the Diamond and Pearl games.
    ''' </summary>
    ''' <param name="_Species"></param>
    ''' <param name="Parameter">
    ''' Base stats data: 0 HP, 1 ATK, 2 DEF, 3 SPD, 4 SPATK, 5 SPDEF, 6 type 1, 7 type 2, 8 catch rate, 9 base EXP,
    ''' 10 HP EP, 11 ATK EP, 12 DEF EP, 13 SPD EP, 14 SPATK EP, 15 SPDEF EP, 16 gender ratio, 17 base egg step,
    ''' 18 base tameness, 19 growth group, 20 egg group 1, 21 egg group 2, 22 ability 1, 23 ability 2, 24 color,
    ''' 25 D/P item 50%, 26 D/P item 5%, 27 Safari Zone Flag(?)
    ''' </param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property DPBaseStats(ByVal _Species As Species, ByVal Parameter As Byte) As UInt16
        Get
            InitializeDictionaries()
            Return dPKMBaseStats(_Species)(Parameter)
        End Get
    End Property

    Private Shared mDictionariesInitialized As Boolean = False

#Region "Declarations"
    Public Shared dPKMMoves As New Dictionary(Of Moves, Object())
    Public Shared dPKMSpecies As New Dictionary(Of Species, String)
    Public Shared dPKMBaseStats As New Dictionary(Of Species, UInt16())
    Public Shared dPKMItems As New Dictionary(Of Items, String)
    Public Shared dpAvatarNames As New Dictionary(Of Avatars, String)
    Public Shared dpColors As New Dictionary(Of DSColors, Color)
    Public Shared dpWallpapers As New Dictionary(Of Byte, String)
    Public Shared dpNatures As New Dictionary(Of Natures, Decimal())
    Public Shared dpLocations As New Dictionary(Of DSLocations, String)
    Public Shared dpPKMShuffle As New Dictionary(Of Byte, String)
    Public Shared dpPKMEncounters As New Dictionary(Of Byte, String)
    Public Shared dpPKMHometowns As New Dictionary(Of Hometowns, String)
    Public Shared dpPKMCharacterstics As New Dictionary(Of Byte, String())
    Public Shared dpAbilities As New Dictionary(Of Abilities, String())
    Public Shared dpCountries As New Dictionary(Of Countries, String)

    Public Shared dpNMSprites As New Dictionary(Of Species, Bitmap)
    Public Shared dpNFSprites As New Dictionary(Of Species, Bitmap)
    Public Shared dpSMSprites As New Dictionary(Of Species, Bitmap)
    Public Shared dpSFSprites As New Dictionary(Of Species, Bitmap)

    Public Shared dpArceusSprites As New Dictionary(Of ArceusFormes, Bitmap)
    Public Shared dpSArceusSprites As New Dictionary(Of ArceusFormes, Bitmap)
    Public Shared dpBurmySprites As New Dictionary(Of BurmyWormadamFormes, Bitmap)
    Public Shared dpSBurmySprites As New Dictionary(Of BurmyWormadamFormes, Bitmap)
    Public Shared dpWormadamSprites As New Dictionary(Of BurmyWormadamFormes, Bitmap)
    Public Shared dpSWormadamSprites As New Dictionary(Of BurmyWormadamFormes, Bitmap)
    Public Shared dpUnownSprites As New Dictionary(Of UnownFormes, Bitmap)
    Public Shared dpSUnownSprites As New Dictionary(Of UnownFormes, Bitmap)
    Public Shared dpDeoxysSprites As New Dictionary(Of DeoxysFormes, Bitmap)
    Public Shared dpSDeoxysSprites As New Dictionary(Of DeoxysFormes, Bitmap)
    Public Shared dpRotomSprites As New Dictionary(Of RotomFormes, Bitmap)
    Public Shared dpSRotomSprites As New Dictionary(Of RotomFormes, Bitmap)
    Public Shared dpShellosSprites As New Dictionary(Of ShellosGastrodonFormes, Bitmap)
    Public Shared dpSShellosSprites As New Dictionary(Of ShellosGastrodonFormes, Bitmap)
    Public Shared dpGastrodonSprites As New Dictionary(Of ShellosGastrodonFormes, Bitmap)
    Public Shared dpSGastrodonSprites As New Dictionary(Of ShellosGastrodonFormes, Bitmap)
    Public Shared dpGiratinaSprites As New Dictionary(Of GiratinaFormes, Bitmap)
    Public Shared dpSGiratinaSprites As New Dictionary(Of GiratinaFormes, Bitmap)
    Public Shared dpShayminSprites As New Dictionary(Of ShayminFormes, Bitmap)
    Public Shared dpSShayminSprites As New Dictionary(Of ShayminFormes, Bitmap)

    Public Shared dpDeoxysStats As New Dictionary(Of DeoxysFormes, Byte())
    Public Shared dpWormadamStats As New Dictionary(Of BurmyWormadamFormes, Byte())
    Public Shared dpRotomStats As New Dictionary(Of RotomFormes, Byte())
    Public Shared dpGiratinaStats As New Dictionary(Of GiratinaFormes, Byte())
    Public Shared dpShayminStats As New Dictionary(Of ShayminFormes, Byte())

    Public Shared dpBoxIcons As New Dictionary(Of Species, Bitmap)

    Public Shared dpUnownBoxIcons As New Dictionary(Of UnownFormes, Bitmap)
    Public Shared dpDeoxysBoxIcons As New Dictionary(Of DeoxysFormes, Bitmap)
    Public Shared dpBurmyBoxIcons As New Dictionary(Of BurmyWormadamFormes, Bitmap)
    Public Shared dpWormadamBoxIcons As New Dictionary(Of BurmyWormadamFormes, Bitmap)
    Public Shared dpShellosBoxIcons As New Dictionary(Of ShellosGastrodonFormes, Bitmap)
    Public Shared dpGastrodonBoxIcons As New Dictionary(Of ShellosGastrodonFormes, Bitmap)
    Public Shared dpRotomBoxIcons As New Dictionary(Of RotomFormes, Bitmap)
    Public Shared dpGiratinaBoxIcons As New Dictionary(Of GiratinaFormes, Bitmap)
    Public Shared dpShayminBoxIcons As New Dictionary(Of ShayminFormes, Bitmap)

    Public Shared dpTypeIcons As New Dictionary(Of Types, Bitmap)
    Public Shared dpItemImages As New Dictionary(Of Items, Bitmap)
    Public Shared dpPoketchApps As New Dictionary(Of Byte, String)
    Public Shared dpWallpaperImages As New Dictionary(Of Byte, Bitmap)
    'GBA dictionaries
    Public Shared GBAShuffleOrder As New Dictionary(Of Byte, String)
    Public Shared GBASpecies As New Dictionary(Of UInt16, String)
    Public Shared GBAFont As New Dictionary(Of UInt16, String)
    Public Shared GBAItems As New Dictionary(Of UInt16, String)
    Public Shared GBALocations As New Dictionary(Of Byte, String)
    Public Shared LevelTable(,) As Integer
#End Region

    Public Shared Sub InitializeDictionaries()
        If mDictionariesInitialized Then Exit Sub

        'Pokémon Species
        'dPKMSpecies.Add(0, "MISSINGNO")
        dPKMSpecies.Add(1, "Bulbasaur")
        dPKMSpecies.Add(2, "Ivysaur")
        dPKMSpecies.Add(3, "Venusaur")
        dPKMSpecies.Add(4, "Charmander")
        dPKMSpecies.Add(5, "Charmeleon")
        dPKMSpecies.Add(6, "Charizard")
        dPKMSpecies.Add(7, "Squirtle")
        dPKMSpecies.Add(8, "Wartortle")
        dPKMSpecies.Add(9, "Blastoise")
        dPKMSpecies.Add(10, "Caterpie")
        dPKMSpecies.Add(11, "Metapod")
        dPKMSpecies.Add(12, "Butterfree")
        dPKMSpecies.Add(13, "Weedle")
        dPKMSpecies.Add(14, "Kakuna")
        dPKMSpecies.Add(15, "Beedrill")
        dPKMSpecies.Add(16, "Pidgey")
        dPKMSpecies.Add(17, "Pidgeotto")
        dPKMSpecies.Add(18, "Pidgeot")
        dPKMSpecies.Add(19, "Rattata")
        dPKMSpecies.Add(20, "Raticate")
        dPKMSpecies.Add(21, "Spearow")
        dPKMSpecies.Add(22, "Fearow")
        dPKMSpecies.Add(23, "Ekans")
        dPKMSpecies.Add(24, "Arbok")
        dPKMSpecies.Add(25, "Pikachu")
        dPKMSpecies.Add(26, "Raichu")
        dPKMSpecies.Add(27, "Sandshrew")
        dPKMSpecies.Add(28, "Sandslash")
        dPKMSpecies.Add(29, "Nidoran" & Char.ConvertFromUtf32(CNVRT.PKMToUnicode(&H1BC)))
        dPKMSpecies.Add(30, "Nidorina")
        dPKMSpecies.Add(31, "Nidoqueen")
        dPKMSpecies.Add(32, "Nidoran" & Char.ConvertFromUtf32(CNVRT.PKMToUnicode(&H1BB)))
        dPKMSpecies.Add(33, "Nidorino")
        dPKMSpecies.Add(34, "Nidoking")
        dPKMSpecies.Add(35, "Clefairy")
        dPKMSpecies.Add(36, "Clefable")
        dPKMSpecies.Add(37, "Vulpix")
        dPKMSpecies.Add(38, "Ninetales")
        dPKMSpecies.Add(39, "Jigglypuff")
        dPKMSpecies.Add(40, "Wigglytuff")
        dPKMSpecies.Add(41, "Zubat")
        dPKMSpecies.Add(42, "Golbat")
        dPKMSpecies.Add(43, "Oddish")
        dPKMSpecies.Add(44, "Gloom")
        dPKMSpecies.Add(45, "Vileplume")
        dPKMSpecies.Add(46, "Paras")
        dPKMSpecies.Add(47, "Parasect")
        dPKMSpecies.Add(48, "Venonat")
        dPKMSpecies.Add(49, "Venomoth")
        dPKMSpecies.Add(50, "Diglett")
        dPKMSpecies.Add(51, "Dugtrio")
        dPKMSpecies.Add(52, "Meowth")
        dPKMSpecies.Add(53, "Persian")
        dPKMSpecies.Add(54, "Psyduck")
        dPKMSpecies.Add(55, "Golduck")
        dPKMSpecies.Add(56, "Mankey")
        dPKMSpecies.Add(57, "Primeape")
        dPKMSpecies.Add(58, "Growlithe")
        dPKMSpecies.Add(59, "Arcanine")
        dPKMSpecies.Add(60, "Poliwag")
        dPKMSpecies.Add(61, "Poliwhirl")
        dPKMSpecies.Add(62, "Poliwrath")
        dPKMSpecies.Add(63, "Abra")
        dPKMSpecies.Add(64, "Kadabra")
        dPKMSpecies.Add(65, "Alakazam")
        dPKMSpecies.Add(66, "Machop")
        dPKMSpecies.Add(67, "Machoke")
        dPKMSpecies.Add(68, "Machamp")
        dPKMSpecies.Add(69, "Bellsprout")
        dPKMSpecies.Add(70, "Weepinbell")
        dPKMSpecies.Add(71, "Victreebel")
        dPKMSpecies.Add(72, "Tentacool")
        dPKMSpecies.Add(73, "Tentacruel")
        dPKMSpecies.Add(74, "Geodude")
        dPKMSpecies.Add(75, "Graveler")
        dPKMSpecies.Add(76, "Golem")
        dPKMSpecies.Add(77, "Ponyta")
        dPKMSpecies.Add(78, "Rapidash")
        dPKMSpecies.Add(79, "Slowpoke")
        dPKMSpecies.Add(80, "Slowbro")
        dPKMSpecies.Add(81, "Magnemite")
        dPKMSpecies.Add(82, "Magneton")
        dPKMSpecies.Add(83, "Farfetch'd")
        dPKMSpecies.Add(84, "Doduo")
        dPKMSpecies.Add(85, "Dodrio")
        dPKMSpecies.Add(86, "Seel")
        dPKMSpecies.Add(87, "Dewgong")
        dPKMSpecies.Add(88, "Grimer")
        dPKMSpecies.Add(89, "Muk")
        dPKMSpecies.Add(90, "Shellder")
        dPKMSpecies.Add(91, "Cloyster")
        dPKMSpecies.Add(92, "Gastly")
        dPKMSpecies.Add(93, "Haunter")
        dPKMSpecies.Add(94, "Gengar")
        dPKMSpecies.Add(95, "Onix")
        dPKMSpecies.Add(96, "Drowzee")
        dPKMSpecies.Add(97, "Hypno")
        dPKMSpecies.Add(98, "Krabby")
        dPKMSpecies.Add(99, "Kingler")
        dPKMSpecies.Add(100, "Voltorb")
        dPKMSpecies.Add(101, "Electrode")
        dPKMSpecies.Add(102, "Exeggcute")
        dPKMSpecies.Add(103, "Exeggutor")
        dPKMSpecies.Add(104, "Cubone")
        dPKMSpecies.Add(105, "Marowak")
        dPKMSpecies.Add(106, "Hitmonlee")
        dPKMSpecies.Add(107, "Hitmonchan")
        dPKMSpecies.Add(108, "Lickitung")
        dPKMSpecies.Add(109, "Koffing")
        dPKMSpecies.Add(110, "Weezing")
        dPKMSpecies.Add(111, "Rhyhorn")
        dPKMSpecies.Add(112, "Rhydon")
        dPKMSpecies.Add(113, "Chansey")
        dPKMSpecies.Add(114, "Tangela")
        dPKMSpecies.Add(115, "Kangaskhan")
        dPKMSpecies.Add(116, "Horsea")
        dPKMSpecies.Add(117, "Seadra")
        dPKMSpecies.Add(118, "Goldeen")
        dPKMSpecies.Add(119, "Seaking")
        dPKMSpecies.Add(120, "Staryu")
        dPKMSpecies.Add(121, "Starmie")
        dPKMSpecies.Add(122, "Mr. Mime")
        dPKMSpecies.Add(123, "Scyther")
        dPKMSpecies.Add(124, "Jynx")
        dPKMSpecies.Add(125, "Electabuzz")
        dPKMSpecies.Add(126, "Magmar")
        dPKMSpecies.Add(127, "Pinsir")
        dPKMSpecies.Add(128, "Tauros")
        dPKMSpecies.Add(129, "Magikarp")
        dPKMSpecies.Add(130, "Gyarados")
        dPKMSpecies.Add(131, "Lapras")
        dPKMSpecies.Add(132, "Ditto")
        dPKMSpecies.Add(133, "Eevee")
        dPKMSpecies.Add(134, "Vaporeon")
        dPKMSpecies.Add(135, "Jolteon")
        dPKMSpecies.Add(136, "Flareon")
        dPKMSpecies.Add(137, "Porygon")
        dPKMSpecies.Add(138, "Omanyte")
        dPKMSpecies.Add(139, "Omastar")
        dPKMSpecies.Add(140, "Kabuto")
        dPKMSpecies.Add(141, "Kabutops")
        dPKMSpecies.Add(142, "Aerodactyl")
        dPKMSpecies.Add(143, "Snorlax")
        dPKMSpecies.Add(144, "Articuno")
        dPKMSpecies.Add(145, "Zapdos")
        dPKMSpecies.Add(146, "Moltres")
        dPKMSpecies.Add(147, "Dratini")
        dPKMSpecies.Add(148, "Dragonair")
        dPKMSpecies.Add(149, "Dragonite")
        dPKMSpecies.Add(150, "Mewtwo")
        dPKMSpecies.Add(151, "Mew")
        dPKMSpecies.Add(152, "Chikorita")
        dPKMSpecies.Add(153, "Bayleef")
        dPKMSpecies.Add(154, "Meganium")
        dPKMSpecies.Add(155, "Cyndaquil")
        dPKMSpecies.Add(156, "Quilava")
        dPKMSpecies.Add(157, "Typhlosion")
        dPKMSpecies.Add(158, "Totodile")
        dPKMSpecies.Add(159, "Croconaw")
        dPKMSpecies.Add(160, "Feraligatr")
        dPKMSpecies.Add(161, "Sentret")
        dPKMSpecies.Add(162, "Furret")
        dPKMSpecies.Add(163, "Hoothoot")
        dPKMSpecies.Add(164, "Noctowl")
        dPKMSpecies.Add(165, "Ledyba")
        dPKMSpecies.Add(166, "Ledian")
        dPKMSpecies.Add(167, "Spinarak")
        dPKMSpecies.Add(168, "Ariados")
        dPKMSpecies.Add(169, "Crobat")
        dPKMSpecies.Add(170, "Chinchou")
        dPKMSpecies.Add(171, "Lanturn")
        dPKMSpecies.Add(172, "Pichu")
        dPKMSpecies.Add(173, "Cleffa")
        dPKMSpecies.Add(174, "Igglybuff")
        dPKMSpecies.Add(175, "Togepi")
        dPKMSpecies.Add(176, "Togetic")
        dPKMSpecies.Add(177, "Natu")
        dPKMSpecies.Add(178, "Xatu")
        dPKMSpecies.Add(179, "Mareep")
        dPKMSpecies.Add(180, "Flaaffy")
        dPKMSpecies.Add(181, "Ampharos")
        dPKMSpecies.Add(182, "Bellossom")
        dPKMSpecies.Add(183, "Marill")
        dPKMSpecies.Add(184, "Azumarill")
        dPKMSpecies.Add(185, "Sudowoodo")
        dPKMSpecies.Add(186, "Politoed")
        dPKMSpecies.Add(187, "Hoppip")
        dPKMSpecies.Add(188, "Skiploom")
        dPKMSpecies.Add(189, "Jumpluff")
        dPKMSpecies.Add(190, "Aipom")
        dPKMSpecies.Add(191, "Sunkern")
        dPKMSpecies.Add(192, "Sunflora")
        dPKMSpecies.Add(193, "Yanma")
        dPKMSpecies.Add(194, "Wooper")
        dPKMSpecies.Add(195, "Quagsire")
        dPKMSpecies.Add(196, "Espeon")
        dPKMSpecies.Add(197, "Umbreon")
        dPKMSpecies.Add(198, "Murkrow")
        dPKMSpecies.Add(199, "Slowking")
        dPKMSpecies.Add(200, "Misdreavus")
        dPKMSpecies.Add(201, "Unown")
        dPKMSpecies.Add(202, "Wobbuffet")
        dPKMSpecies.Add(203, "Girafarig")
        dPKMSpecies.Add(204, "Pineco")
        dPKMSpecies.Add(205, "Forretress")
        dPKMSpecies.Add(206, "Dunsparce")
        dPKMSpecies.Add(207, "Gligar")
        dPKMSpecies.Add(208, "Steelix")
        dPKMSpecies.Add(209, "Snubbull")
        dPKMSpecies.Add(210, "Granbull")
        dPKMSpecies.Add(211, "Qwilfish")
        dPKMSpecies.Add(212, "Scizor")
        dPKMSpecies.Add(213, "Shuckle")
        dPKMSpecies.Add(214, "Heracross")
        dPKMSpecies.Add(215, "Sneasel")
        dPKMSpecies.Add(216, "Teddiursa")
        dPKMSpecies.Add(217, "Ursaring")
        dPKMSpecies.Add(218, "Slugma")
        dPKMSpecies.Add(219, "Magcargo")
        dPKMSpecies.Add(220, "Swinub")
        dPKMSpecies.Add(221, "Piloswine")
        dPKMSpecies.Add(222, "Corsola")
        dPKMSpecies.Add(223, "Remoraid")
        dPKMSpecies.Add(224, "Octillery")
        dPKMSpecies.Add(225, "Delibird")
        dPKMSpecies.Add(226, "Mantine")
        dPKMSpecies.Add(227, "Skarmory")
        dPKMSpecies.Add(228, "Houndour")
        dPKMSpecies.Add(229, "Houndoom")
        dPKMSpecies.Add(230, "Kingdra")
        dPKMSpecies.Add(231, "Phanpy")
        dPKMSpecies.Add(232, "Donphan")
        dPKMSpecies.Add(233, "Porygon2")
        dPKMSpecies.Add(234, "Stantler")
        dPKMSpecies.Add(235, "Smeargle")
        dPKMSpecies.Add(236, "Tyrogue")
        dPKMSpecies.Add(237, "Hitmontop")
        dPKMSpecies.Add(238, "Smoochum")
        dPKMSpecies.Add(239, "Elekid")
        dPKMSpecies.Add(240, "Magby")
        dPKMSpecies.Add(241, "Miltank")
        dPKMSpecies.Add(242, "Blissey")
        dPKMSpecies.Add(243, "Raikou")
        dPKMSpecies.Add(244, "Entei")
        dPKMSpecies.Add(245, "Suicune")
        dPKMSpecies.Add(246, "Larvitar")
        dPKMSpecies.Add(247, "Pupitar")
        dPKMSpecies.Add(248, "Tyranitar")
        dPKMSpecies.Add(249, "Lugia")
        dPKMSpecies.Add(250, "Ho-Oh")
        dPKMSpecies.Add(251, "Celebi")
        dPKMSpecies.Add(252, "Treecko")
        dPKMSpecies.Add(253, "Grovyle")
        dPKMSpecies.Add(254, "Sceptile")
        dPKMSpecies.Add(255, "Torchic")
        dPKMSpecies.Add(256, "Combusken")
        dPKMSpecies.Add(257, "Blaziken")
        dPKMSpecies.Add(258, "Mudkip")
        dPKMSpecies.Add(259, "Marshtomp")
        dPKMSpecies.Add(260, "Swampert")
        dPKMSpecies.Add(261, "Poochyena")
        dPKMSpecies.Add(262, "Mightyena")
        dPKMSpecies.Add(263, "Zigzagoon")
        dPKMSpecies.Add(264, "Linoone")
        dPKMSpecies.Add(265, "Wurmple")
        dPKMSpecies.Add(266, "Silcoon")
        dPKMSpecies.Add(267, "Beautifly")
        dPKMSpecies.Add(268, "Cascoon")
        dPKMSpecies.Add(269, "Dustox")
        dPKMSpecies.Add(270, "Lotad")
        dPKMSpecies.Add(271, "Lombre")
        dPKMSpecies.Add(272, "Ludicolo")
        dPKMSpecies.Add(273, "Seedot")
        dPKMSpecies.Add(274, "Nuzleaf")
        dPKMSpecies.Add(275, "Shiftry")
        dPKMSpecies.Add(276, "Taillow")
        dPKMSpecies.Add(277, "Swellow")
        dPKMSpecies.Add(278, "Wingull")
        dPKMSpecies.Add(279, "Pelipper")
        dPKMSpecies.Add(280, "Ralts")
        dPKMSpecies.Add(281, "Kirlia")
        dPKMSpecies.Add(282, "Gardevoir")
        dPKMSpecies.Add(283, "Surskit")
        dPKMSpecies.Add(284, "Masquerain")
        dPKMSpecies.Add(285, "Shroomish")
        dPKMSpecies.Add(286, "Breloom")
        dPKMSpecies.Add(287, "Slakoth")
        dPKMSpecies.Add(288, "Vigoroth")
        dPKMSpecies.Add(289, "Slaking")
        dPKMSpecies.Add(290, "Nincada")
        dPKMSpecies.Add(291, "Ninjask")
        dPKMSpecies.Add(292, "Shedinja")
        dPKMSpecies.Add(293, "Whismur")
        dPKMSpecies.Add(294, "Loudred")
        dPKMSpecies.Add(295, "Exploud")
        dPKMSpecies.Add(296, "Makuhita")
        dPKMSpecies.Add(297, "Hariyama")
        dPKMSpecies.Add(298, "Azurill")
        dPKMSpecies.Add(299, "Nosepass")
        dPKMSpecies.Add(300, "Skitty")
        dPKMSpecies.Add(301, "Delcatty")
        dPKMSpecies.Add(302, "Sableye")
        dPKMSpecies.Add(303, "Mawile")
        dPKMSpecies.Add(304, "Aron")
        dPKMSpecies.Add(305, "Lairon")
        dPKMSpecies.Add(306, "Aggron")
        dPKMSpecies.Add(307, "Meditite")
        dPKMSpecies.Add(308, "Medicham")
        dPKMSpecies.Add(309, "Electrike")
        dPKMSpecies.Add(310, "Manectric")
        dPKMSpecies.Add(311, "Plusle")
        dPKMSpecies.Add(312, "Minun")
        dPKMSpecies.Add(313, "Volbeat")
        dPKMSpecies.Add(314, "Illumise")
        dPKMSpecies.Add(315, "Roselia")
        dPKMSpecies.Add(316, "Gulpin")
        dPKMSpecies.Add(317, "Swalot")
        dPKMSpecies.Add(318, "Carvanha")
        dPKMSpecies.Add(319, "Sharpedo")
        dPKMSpecies.Add(320, "Wailmer")
        dPKMSpecies.Add(321, "Wailord")
        dPKMSpecies.Add(322, "Numel")
        dPKMSpecies.Add(323, "Camerupt")
        dPKMSpecies.Add(324, "Torkoal")
        dPKMSpecies.Add(325, "Spoink")
        dPKMSpecies.Add(326, "Grumpig")
        dPKMSpecies.Add(327, "Spinda")
        dPKMSpecies.Add(328, "Trapinch")
        dPKMSpecies.Add(329, "Vibrava")
        dPKMSpecies.Add(330, "Flygon")
        dPKMSpecies.Add(331, "Cacnea")
        dPKMSpecies.Add(332, "Cacturne")
        dPKMSpecies.Add(333, "Swablu")
        dPKMSpecies.Add(334, "Altaria")
        dPKMSpecies.Add(335, "Zangoose")
        dPKMSpecies.Add(336, "Seviper")
        dPKMSpecies.Add(337, "Lunatone")
        dPKMSpecies.Add(338, "Solrock")
        dPKMSpecies.Add(339, "Barboach")
        dPKMSpecies.Add(340, "Whiscash")
        dPKMSpecies.Add(341, "Corphish")
        dPKMSpecies.Add(342, "Crawdaunt")
        dPKMSpecies.Add(343, "Baltoy")
        dPKMSpecies.Add(344, "Claydol")
        dPKMSpecies.Add(345, "Lileep")
        dPKMSpecies.Add(346, "Cradily")
        dPKMSpecies.Add(347, "Anorith")
        dPKMSpecies.Add(348, "Armaldo")
        dPKMSpecies.Add(349, "Feebas")
        dPKMSpecies.Add(350, "Milotic")
        dPKMSpecies.Add(351, "Castform")
        dPKMSpecies.Add(352, "Kecleon")
        dPKMSpecies.Add(353, "Shuppet")
        dPKMSpecies.Add(354, "Banette")
        dPKMSpecies.Add(355, "Duskull")
        dPKMSpecies.Add(356, "Dusclops")
        dPKMSpecies.Add(357, "Tropius")
        dPKMSpecies.Add(358, "Chimecho")
        dPKMSpecies.Add(359, "Absol")
        dPKMSpecies.Add(360, "Wynaut")
        dPKMSpecies.Add(361, "Snorunt")
        dPKMSpecies.Add(362, "Glalie")
        dPKMSpecies.Add(363, "Spheal")
        dPKMSpecies.Add(364, "Sealeo")
        dPKMSpecies.Add(365, "Walrein")
        dPKMSpecies.Add(366, "Clamperl")
        dPKMSpecies.Add(367, "Huntail")
        dPKMSpecies.Add(368, "Gorebyss")
        dPKMSpecies.Add(369, "Relicanth")
        dPKMSpecies.Add(370, "Luvdisc")
        dPKMSpecies.Add(371, "Bagon")
        dPKMSpecies.Add(372, "Shelgon")
        dPKMSpecies.Add(373, "Salamence")
        dPKMSpecies.Add(374, "Beldum")
        dPKMSpecies.Add(375, "Metang")
        dPKMSpecies.Add(376, "Metagross")
        dPKMSpecies.Add(377, "Regirock")
        dPKMSpecies.Add(378, "Regice")
        dPKMSpecies.Add(379, "Registeel")
        dPKMSpecies.Add(380, "Latias")
        dPKMSpecies.Add(381, "Latios")
        dPKMSpecies.Add(382, "Kyogre")
        dPKMSpecies.Add(383, "Groudon")
        dPKMSpecies.Add(384, "Rayquaza")
        dPKMSpecies.Add(385, "Jirachi")
        dPKMSpecies.Add(386, "Deoxys")
        dPKMSpecies.Add(387, "Turtwig")
        dPKMSpecies.Add(388, "Grotle")
        dPKMSpecies.Add(389, "Torterra")
        dPKMSpecies.Add(390, "Chimchar")
        dPKMSpecies.Add(391, "Monferno")
        dPKMSpecies.Add(392, "Infernape")
        dPKMSpecies.Add(393, "Piplup")
        dPKMSpecies.Add(394, "Prinplup")
        dPKMSpecies.Add(395, "Empoleon")
        dPKMSpecies.Add(396, "Starly")
        dPKMSpecies.Add(397, "Staravia")
        dPKMSpecies.Add(398, "Staraptor")
        dPKMSpecies.Add(399, "Bidoof")
        dPKMSpecies.Add(400, "Bibarel")
        dPKMSpecies.Add(401, "Kricketot")
        dPKMSpecies.Add(402, "Kricketune")
        dPKMSpecies.Add(403, "Shinx")
        dPKMSpecies.Add(404, "Luxio")
        dPKMSpecies.Add(405, "Luxray")
        dPKMSpecies.Add(406, "Budew")
        dPKMSpecies.Add(407, "Roserade")
        dPKMSpecies.Add(408, "Cranidos")
        dPKMSpecies.Add(409, "Rampardos")
        dPKMSpecies.Add(410, "Shieldon")
        dPKMSpecies.Add(411, "Bastiodon")
        dPKMSpecies.Add(412, "Burmy")
        dPKMSpecies.Add(413, "Wormadam")
        dPKMSpecies.Add(414, "Mothim")
        dPKMSpecies.Add(415, "Combee")
        dPKMSpecies.Add(416, "Vespiquen")
        dPKMSpecies.Add(417, "Pachirisu")
        dPKMSpecies.Add(418, "Buizel")
        dPKMSpecies.Add(419, "Floatzel")
        dPKMSpecies.Add(420, "Cherubi")
        dPKMSpecies.Add(421, "Cherrim")
        dPKMSpecies.Add(422, "Shellos")
        dPKMSpecies.Add(423, "Gastrodon")
        dPKMSpecies.Add(424, "Ambipom")
        dPKMSpecies.Add(425, "Drifloon")
        dPKMSpecies.Add(426, "Drifblim")
        dPKMSpecies.Add(427, "Buneary")
        dPKMSpecies.Add(428, "Lopunny")
        dPKMSpecies.Add(429, "Mismagius")
        dPKMSpecies.Add(430, "Honchkrow")
        dPKMSpecies.Add(431, "Glameow")
        dPKMSpecies.Add(432, "Purugly")
        dPKMSpecies.Add(433, "Chingling")
        dPKMSpecies.Add(434, "Stunky")
        dPKMSpecies.Add(435, "Skuntank")
        dPKMSpecies.Add(436, "Bronzor")
        dPKMSpecies.Add(437, "Bronzong")
        dPKMSpecies.Add(438, "Bonsly")
        dPKMSpecies.Add(439, "Mime Jr.")
        dPKMSpecies.Add(440, "Happiny")
        dPKMSpecies.Add(441, "Chatot")
        dPKMSpecies.Add(442, "Spiritomb")
        dPKMSpecies.Add(443, "Gible")
        dPKMSpecies.Add(444, "Gabite")
        dPKMSpecies.Add(445, "Garchomp")
        dPKMSpecies.Add(446, "Munchlax")
        dPKMSpecies.Add(447, "Riolu")
        dPKMSpecies.Add(448, "Lucario")
        dPKMSpecies.Add(449, "Hippopotas")
        dPKMSpecies.Add(450, "Hippowdon")
        dPKMSpecies.Add(451, "Skorupi")
        dPKMSpecies.Add(452, "Drapion")
        dPKMSpecies.Add(453, "Croagunk")
        dPKMSpecies.Add(454, "Toxicroak")
        dPKMSpecies.Add(455, "Carnivine")
        dPKMSpecies.Add(456, "Finneon")
        dPKMSpecies.Add(457, "Lumineon")
        dPKMSpecies.Add(458, "Mantyke")
        dPKMSpecies.Add(459, "Snover")
        dPKMSpecies.Add(460, "Abomasnow")
        dPKMSpecies.Add(461, "Weavile")
        dPKMSpecies.Add(462, "Magnezone")
        dPKMSpecies.Add(463, "Lickilicky")
        dPKMSpecies.Add(464, "Rhyperior")
        dPKMSpecies.Add(465, "Tangrowth")
        dPKMSpecies.Add(466, "Electivire")
        dPKMSpecies.Add(467, "Magmortar")
        dPKMSpecies.Add(468, "Togekiss")
        dPKMSpecies.Add(469, "Yanmega")
        dPKMSpecies.Add(470, "Leafeon")
        dPKMSpecies.Add(471, "Glaceon")
        dPKMSpecies.Add(472, "Gliscor")
        dPKMSpecies.Add(473, "Mamoswine")
        dPKMSpecies.Add(474, "Porygon-Z")
        dPKMSpecies.Add(475, "Gallade")
        dPKMSpecies.Add(476, "Probopass")
        dPKMSpecies.Add(477, "Dusknoir")
        dPKMSpecies.Add(478, "Froslass")
        dPKMSpecies.Add(479, "Rotom")
        dPKMSpecies.Add(480, "Uxie")
        dPKMSpecies.Add(481, "Mesprit")
        dPKMSpecies.Add(482, "Azelf")
        dPKMSpecies.Add(483, "Dialga")
        dPKMSpecies.Add(484, "Palkia")
        dPKMSpecies.Add(485, "Heatran")
        dPKMSpecies.Add(486, "Regigigas")
        dPKMSpecies.Add(487, "Giratina")
        dPKMSpecies.Add(488, "Cresselia")
        dPKMSpecies.Add(489, "Phione")
        dPKMSpecies.Add(490, "Manaphy")
        dPKMSpecies.Add(491, "Darkrai")
        dPKMSpecies.Add(492, "Shaymin")
        dPKMSpecies.Add(493, "Arceus")

        'Pokémon Moves
        dPKMMoves.Add(0, New String() {"NOTHING", "NOTHING", "NOTHING", "NOTHING", "NOTHING", "NOTHING", "NOTHING", "NOTHING", "NOTHING", "NOTHING", "NOTHING", "NOTHING", "NOTHING"})
        dPKMMoves.Add(1, New String() {"Pound", "The foe is physically pounded with a long tail or a foreleg, etc.", "0", "40", "0", "100", "35", "0", "0", "0", "115", "0", "4"})
        dPKMMoves.Add(2, New String() {"Karate Chop", "The foe is attacked with a sharp chop. It has a high critical-hit ratio.", "43", "50", "1", "100", "25", "0", "0", "0", "115", "0", "4"})
        dPKMMoves.Add(3, New String() {"DoubleSlap", "The foe is slapped repeatedly, back and forth, two to five times in a row.", "29", "15", "0", "85", "10", "0", "0", "0", "115", "0", "4"})
        dPKMMoves.Add(4, New String() {"Comet Punch", "The foe is hit with a flurry of punches that strike two to five times in a row.", "29", "18", "0", "85", "15", "0", "0", "0", "115", "0", "4"})
        dPKMMoves.Add(5, New String() {"Mega Punch", "The foe is slugged by a punch thrown with muscle-packed power.", "0", "80", "0", "85", "20", "0", "0", "0", "51", "0", "4"})
        dPKMMoves.Add(6, New String() {"Pay Day", "Numerous coins are hurled at the foe to inflict damage. Money is earned after battle.", "34", "40", "0", "100", "20", "0", "0", "0", "50", "0", "3"})
        dPKMMoves.Add(7, New String() {"Fire Punch", "The foe is punched with a fiery fist. It may leave the target with a burn.", "4", "75", "10", "100", "15", "10", "0", "0", "19", "0", "1"})
        dPKMMoves.Add(8, New String() {"Ice Punch", "The foe is punched with an icy fist. It may leave the target frozen.", "5", "75", "15", "100", "15", "10", "0", "0", "19", "0", "1"})
        dPKMMoves.Add(9, New String() {"ThunderPunch", "The foe is punched with an electrified fist. It may leave the target with paralysis.", "6", "75", "13", "100", "15", "10", "0", "0", "19", "0", "0"})
        dPKMMoves.Add(10, New String() {"Scratch", "Hard, pointed, and sharp claws rake the foe to inflict damage.", "0", "40", "0", "100", "35", "0", "0", "0", "115", "0", "4"})
        dPKMMoves.Add(11, New String() {"ViceGrip", "Huge, impressive pincers grip and squeeze the foe.", "0", "55", "0", "100", "30", "0", "0", "0", "51", "0", "4"})
        dPKMMoves.Add(12, New String() {"Guillotine", "A vicious, tearing attack with pincers. The foe will faint instantly if this attack hits.", "38", "1", "0", "30", "5", "0", "0", "0", "19", "0", "0"})
        dPKMMoves.Add(13, New String() {"Razor Wind", "A two-turn attack. Blades of wind hit the foe on the second turn. It has a high critical-hit ratio.", "39", "80", "0", "100", "10", "0", "4", "0", "50", "1", "0"})
        dPKMMoves.Add(14, New String() {"Swords Dance", "A frenetic dance to uplift the fighting spirit. It sharply raises the user's Attack stat.", "50", "0", "0", "0", "30", "0", "16", "0", "72", "2", "1"})
        dPKMMoves.Add(15, New String() {"Cut", "The foe is cut with a scythe or a claw. It can also be used to cut down thin trees.", "0", "50", "0", "95", "30", "0", "0", "0", "115", "0", "0"})
        dPKMMoves.Add(16, New String() {"Gust", "A gust of wind is whipped up by wings and launched at the foe to inflict damage.", "149", "40", "2", "100", "35", "0", "0", "0", "50", "1", "3"})
        dPKMMoves.Add(17, New String() {"Wing Attack", "The foe is struck with large, imposing wings spread wide to inflict damage.", "0", "60", "2", "100", "35", "0", "0", "0", "115", "0", "0"})
        dPKMMoves.Add(18, New String() {"Whirlwind", "The foe is blown away, to be replaced by another Pokémon in its party. In the wild, the battle ends.", "28", "0", "0", "100", "20", "0", "0", "250", "82", "2", "3"})
        dPKMMoves.Add(19, New String() {"Fly", "The user soars, then strikes on the second turn. It can also be used for flying to any familiar town.", "155", "90", "2", "95", "15", "0", "0", "0", "179", "0", "3"})
        dPKMMoves.Add(20, New String() {"Bind", "A long body or tentacles are used to bind and squeeze the foe for two to five turns.", "42", "15", "0", "75", "20", "0", "0", "0", "115", "0", "4"})
        dPKMMoves.Add(21, New String() {"Slam", "The foe is slammed with a long tail, vines, etc., to inflict damage.", "0", "80", "0", "75", "20", "0", "0", "0", "51", "0", "4"})
        dPKMMoves.Add(22, New String() {"Vine Whip", "The foe is struck with slender, whiplike vines to inflict damage.", "0", "35", "12", "100", "15", "0", "0", "0", "115", "0", "0"})
        dPKMMoves.Add(23, New String() {"Stomp", "The foe is stomped with a big foot. It may also make the target flinch.", "150", "65", "0", "100", "20", "30", "0", "0", "83", "0", "4"})
        dPKMMoves.Add(24, New String() {"Double Kick", "The foe is quickly kicked twice in succession using both feet.", "44", "30", "1", "100", "30", "0", "0", "0", "115", "0", "0"})
        dPKMMoves.Add(25, New String() {"Mega Kick", "The foe is attacked by a kick launched with muscle-packed power.", "0", "120", "0", "75", "5", "0", "0", "0", "179", "0", "0"})
        dPKMMoves.Add(26, New String() {"Jump Kick", "The user jumps up high, then strikes with a kick. If the kick misses, the user hurts itself.", "45", "85", "1", "95", "25", "0", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(27, New String() {"Rolling Kick", "The user cuts a quick spin and lashes out with a sharp kick.", "31", "60", "1", "85", "15", "30", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(28, New String() {"Sand-Attack", "Sand is hurled in the foe's face, reducing its accuracy.", "23", "0", "4", "100", "15", "0", "0", "0", "22", "2", "2"})
        dPKMMoves.Add(29, New String() {"Headbutt", "The user sticks its head out and rams straight forward. It may make the foe flinch.", "31", "70", "0", "100", "15", "30", "0", "0", "83", "0", "4"})
        dPKMMoves.Add(30, New String() {"Horn Attack", "The foe is jabbed with a sharply pointed horn to inflict damage.", "0", "65", "0", "100", "25", "0", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(31, New String() {"Fury Attack", "The foe is jabbed repeatedly with a horn or beak two to five times in a row.", "29", "15", "0", "85", "20", "0", "0", "0", "115", "0", "0"})
        dPKMMoves.Add(32, New String() {"Horn Drill", "The foe is stabbed with a horn rotating like a drill. The foe instantly faints if it hits.", "38", "1", "0", "30", "5", "0", "0", "0", "19", "0", "0"})
        dPKMMoves.Add(33, New String() {"Tackle", "A physical attack in which the user charges and slams into the foe with its whole body.", "0", "35", "0", "95", "35", "0", "0", "0", "115", "0", "4"})
        dPKMMoves.Add(34, New String() {"Body Slam", "The user drops onto the foe with its full body weight. It may leave the foe paralyzed.", "6", "85", "0", "100", "15", "30", "0", "0", "83", "0", "4"})
        dPKMMoves.Add(35, New String() {"Wrap", "A long body or vines are used to wrap and squeeze the foe for two to five turns.", "42", "15", "0", "85", "20", "0", "0", "0", "51", "0", "4"})
        dPKMMoves.Add(36, New String() {"Take Down", "A reckless, full-body charge attack for slamming into the foe. It also damages the user a little.", "48", "90", "0", "85", "20", "0", "0", "0", "51", "0", "4"})
        dPKMMoves.Add(37, New String() {"Thrash", "The user rampages and attacks for two to three turns. It then becomes confused, however.", "27", "90", "0", "100", "20", "0", "2", "0", "51", "0", "4"})
        dPKMMoves.Add(38, New String() {"Double-Edge", "A reckless, life- risking tackle. It also damages the user by a fairly large amount, however.", "198", "120", "0", "100", "15", "0", "0", "0", "179", "0", "4"})
        dPKMMoves.Add(39, New String() {"Tail Whip", "The user wags its tail cutely, making the foe less wary. The target's Defense stat is lowered.", "19", "0", "0", "100", "30", "0", "4", "0", "86", "2", "2"})
        dPKMMoves.Add(40, New String() {"Poison Sting", "The foe is stabbed with a poisonous barb of some sort. It may also poison the target.", "2", "15", "3", "100", "35", "30", "0", "0", "18", "0", "3"})
        dPKMMoves.Add(41, New String() {"Twineedle", "The foe is stabbed twice by a pair of stingers. It may also poison the target.", "77", "25", "6", "100", "20", "20", "0", "0", "82", "0", "0"})
        dPKMMoves.Add(42, New String() {"Pin Missile", "Sharp pins are shot at the foe in rapid succession. They hit two to five times in a row.", "29", "14", "6", "85", "20", "0", "0", "0", "114", "0", "0"})
        dPKMMoves.Add(43, New String() {"Leer", "The foe is given an intimidating leer with sharp eyes. The target's Defense stat is reduced.", "19", "0", "0", "100", "30", "0", "4", "0", "22", "2", "0"})
        dPKMMoves.Add(44, New String() {"Bite", "The foe is bitten with viciously sharp fangs. It may make the target flinch.", "31", "60", "17", "100", "25", "30", "0", "0", "19", "0", "4"})
        dPKMMoves.Add(45, New String() {"Growl", "The user growls in an endearing way, making the foe less wary. The target's Attack stat is lowered.", "18", "0", "0", "100", "40", "0", "4", "0", "86", "2", "2"})
        dPKMMoves.Add(46, New String() {"Roar", "The foe is scared off, to be replaced by another Pokémon in its party. In the wild, the battle ends.", "28", "0", "0", "100", "20", "0", "0", "250", "82", "2", "0"})
        dPKMMoves.Add(47, New String() {"Sing", "A soothing lullaby is sung in a calming voice that puts the foe into a deep slumber.", "1", "0", "0", "55", "15", "0", "0", "0", "22", "2", "2"})
        dPKMMoves.Add(48, New String() {"Supersonic", "The user generates odd sound waves from its body. It may confuse the target.", "49", "0", "0", "55", "20", "0", "0", "0", "22", "2", "3"})
        dPKMMoves.Add(49, New String() {"SonicBoom", "The foe is hit with a destructive shock wave that always inflicts 20 HP damage.", "130", "1", "0", "90", "20", "0", "0", "0", "114", "1", "0"})
        dPKMMoves.Add(50, New String() {"Disable", "For several turns, this move prevents the foe from using the move it last used.", "86", "0", "0", "80", "20", "0", "0", "0", "18", "2", "3"})
        dPKMMoves.Add(51, New String() {"Acid", "The foe is attacked with a spray of harsh acid. It may also lower the target's Sp. Def stat.", "72", "40", "3", "100", "30", "10", "4", "0", "18", "1", "3"})
        dPKMMoves.Add(52, New String() {"Ember", "The foe is attacked with small flames. The target may also be left with a burn.", "4", "40", "10", "100", "25", "10", "0", "0", "82", "1", "1"})
        dPKMMoves.Add(53, New String() {"Flamethrower", "The foe is scorched with an intense blast of fire. The target may also be left with a burn.", "4", "95", "10", "100", "15", "10", "0", "0", "18", "1", "1"})
        dPKMMoves.Add(54, New String() {"Mist", "The user cloaks its body with a white mist that prevents any of its stats from being cut for five turns.", "46", "0", "15", "0", "30", "0", "32", "0", "8", "2", "1"})
        dPKMMoves.Add(55, New String() {"Water Gun", "The foe is blasted with a forceful shot of water.", "0", "40", "11", "100", "25", "0", "0", "0", "50", "1", "2"})
        dPKMMoves.Add(56, New String() {"Hydro Pump", "The foe is blasted by a huge volume of water launched under great pressure.", "0", "120", "11", "80", "5", "0", "0", "0", "178", "1", "1"})
        dPKMMoves.Add(57, New String() {"Surf", "It swamps the entire battlefield with a giant wave. It can also be used for crossing water.", "257", "95", "11", "100", "15", "0", "8", "0", "50", "1", "1"})
        dPKMMoves.Add(58, New String() {"Ice Beam", "The foe is struck with an icy-cold beam of energy. It may also freeze the target solid.", "5", "95", "15", "100", "10", "10", "0", "0", "18", "1", "1"})
        dPKMMoves.Add(59, New String() {"Blizzard", "A howling blizzard is summoned to strike the foe. It may also freeze the target solid.", "260", "120", "15", "70", "5", "10", "4", "0", "146", "1", "1"})
        dPKMMoves.Add(60, New String() {"Psybeam", "The foe is attacked with a peculiar ray. It may also leave the target confused.", "76", "65", "14", "100", "20", "10", "0", "0", "18", "1", "1"})
        dPKMMoves.Add(61, New String() {"BubbleBeam", "A spray of bubbles is forcefully ejected at the foe. It may also lower the target's Speed stat.", "70", "65", "11", "100", "20", "10", "0", "0", "18", "1", "1"})
        dPKMMoves.Add(62, New String() {"Aurora Beam", "The foe is hit with a rainbow-colored beam. It may also lower the target's Attack stat.", "68", "65", "15", "100", "20", "10", "0", "0", "18", "1", "1"})
        dPKMMoves.Add(63, New String() {"Hyper Beam", "The foe is attacked with a powerful beam. The user must rest on the next turn to regain its energy.", "80", "150", "0", "90", "5", "0", "0", "0", "178", "1", "0"})
        dPKMMoves.Add(64, New String() {"Peck", "The foe is jabbed with a sharply pointed beak or horn.", "0", "35", "2", "100", "35", "0", "0", "0", "115", "0", "0"})
        dPKMMoves.Add(65, New String() {"Drill Peck", "A corkscrewing attack with the sharp beak acting as a drill.", "0", "80", "2", "100", "20", "0", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(66, New String() {"Submission", "The user grabs the foe and recklessly dives for the ground. It also hurts the user slightly.", "48", "80", "1", "80", "25", "0", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(67, New String() {"Low Kick", "A powerful low kick that makes the foe fall over. It inflicts greater damage on heavier foes.", "196", "1", "1", "100", "20", "0", "0", "0", "115", "0", "4"})
        dPKMMoves.Add(68, New String() {"Counter", "A retaliation move that counters any physical attack, inflicting double the damage taken.", "89", "1", "1", "100", "20", "0", "1", "251", "1", "0", "4"})
        dPKMMoves.Add(69, New String() {"Seismic Toss", "The foe is thrown using the power of gravity. It inflicts damage equal to the user's level.", "87", "1", "1", "100", "20", "0", "0", "0", "179", "0", "4"})
        dPKMMoves.Add(70, New String() {"Strength", "The foe is slugged with a punch thrown at maximum power. It can also be used to move boulders.", "0", "80", "0", "100", "15", "0", "0", "0", "51", "0", "4"})
        dPKMMoves.Add(71, New String() {"Absorb", "A nutrient-draining attack. The user's HP is restored by half the damage taken by the target.", "3", "20", "12", "100", "25", "0", "0", "0", "18", "1", "3"})
        dPKMMoves.Add(72, New String() {"Mega Drain", "A nutrient-draining attack. The user's HP is restored by half the damage taken by the target.", "3", "40", "12", "100", "15", "0", "0", "0", "18", "1", "3"})
        dPKMMoves.Add(73, New String() {"Leech Seed", "A seed is planted on the foe. It steals some HP from the foe to heal the user on every turn.", "84", "0", "12", "90", "10", "0", "0", "0", "22", "2", "3"})
        dPKMMoves.Add(74, New String() {"Growth", "The user's body is forced to grow all at once. It raises the Sp. Atk stat.", "13", "0", "0", "0", "40", "0", "16", "0", "72", "2", "1"})
        dPKMMoves.Add(75, New String() {"Razor Leaf", "A sharp-edged leaf is launched to slash at the foe. It has a high critical-hit ratio.", "43", "55", "12", "95", "25", "0", "4", "0", "50", "0", "0"})
        dPKMMoves.Add(76, New String() {"SolarBeam", "A two-turn attack. The user gathers light, then blasts a bundled beam on the second turn.", "151", "120", "12", "100", "10", "0", "0", "0", "50", "1", "0"})
        dPKMMoves.Add(77, New String() {"PoisonPowder", "A cloud of poisonous dust is scattered on the foe. It may poison the target.", "66", "0", "3", "75", "35", "0", "0", "0", "86", "2", "3"})
        dPKMMoves.Add(78, New String() {"Stun Spore", "The user scatters a cloud of paralyzing powder. It may paralyze the target.", "67", "0", "12", "75", "30", "0", "0", "0", "86", "2", "3"})
        dPKMMoves.Add(79, New String() {"Sleep Powder", "The user scatters a big cloud of sleep- inducing dust around the foe.", "1", "0", "12", "75", "15", "0", "0", "0", "86", "2", "3"})
        dPKMMoves.Add(80, New String() {"Petal Dance", "The user attacks by scattering petals for two to three turns. The user then becomes confused.", "27", "90", "12", "100", "20", "0", "2", "0", "51", "1", "1"})
        dPKMMoves.Add(81, New String() {"String Shot", "The foe is bound with silk blown from the user's mouth. It reduces the target's Speed stat.", "20", "0", "6", "95", "40", "0", "4", "0", "22", "2", "3"})
        dPKMMoves.Add(82, New String() {"Dragon Rage", "The foe is stricken by a shock wave. This attack always inflicts 40 HP damage.", "41", "1", "16", "100", "10", "0", "0", "0", "50", "1", "0"})
        dPKMMoves.Add(83, New String() {"Fire Spin", "The foe becomes trapped within a fierce vortex of fire that rages for two to five turns.", "42", "15", "10", "70", "15", "0", "0", "0", "50", "1", "1"})
        dPKMMoves.Add(84, New String() {"ThunderShock", "A jolt of electricity is hurled at the foe to inflict damage. It may also leave the foe paralyzed.", "6", "40", "13", "100", "30", "10", "0", "0", "18", "1", "0"})
        dPKMMoves.Add(85, New String() {"Thunderbolt", "A strong electric blast is loosed at the foe. It may also leave the foe paralyzed.", "6", "95", "13", "100", "15", "10", "0", "0", "18", "1", "0"})
        dPKMMoves.Add(86, New String() {"Thunder Wave", "A weak electric charge is launched at the foe. It causes paralysis if it hits.", "67", "0", "13", "100", "20", "0", "0", "0", "22", "2", "0"})
        dPKMMoves.Add(87, New String() {"Thunder", "A wicked thunderbolt is dropped on the foe to inflict damage. It may also leave the target paralyzed.", "152", "120", "13", "70", "10", "30", "0", "0", "146", "1", "0"})
        dPKMMoves.Add(88, New String() {"Rock Throw", "The user picks up and throws a small rock at the foe to attack.", "0", "50", "5", "90", "15", "0", "0", "0", "50", "0", "4"})
        dPKMMoves.Add(89, New String() {"Earthquake", "The user sets off an earthquake that hits all the Pokémon in the battle.", "147", "100", "4", "100", "10", "0", "8", "0", "50", "0", "4"})
        dPKMMoves.Add(90, New String() {"Fissure", "The user opens up a fissure in the ground and drops the foe in. The target instantly faints if it hits.", "38", "1", "4", "30", "5", "0", "0", "0", "146", "0", "4"})
        dPKMMoves.Add(91, New String() {"Dig", "The user burrows, then attacks on the second turn. It can also be used to exit dungeons.", "256", "80", "4", "100", "10", "0", "0", "0", "51", "0", "3"})
        dPKMMoves.Add(92, New String() {"Toxic", "A move that leaves the target badly poisoned. Its poison damage worsens every turn.", "33", "0", "3", "85", "10", "0", "0", "0", "22", "2", "3"})
        dPKMMoves.Add(93, New String() {"Confusion", "The foe is hit by a weak telekinetic force. It may also leave the foe confused.", "76", "50", "14", "100", "25", "10", "0", "0", "18", "1", "3"})
        dPKMMoves.Add(94, New String() {"Psychic", "The foe is hit by a strong telekinetic force. It may also reduce the foe's Sp. Def stat.", "72", "90", "14", "100", "10", "10", "0", "0", "18", "1", "3"})
        dPKMMoves.Add(95, New String() {"Hypnosis", "The user employs hypnotic suggestion to make the target fall into a deep sleep.", "1", "0", "14", "70", "20", "0", "0", "0", "22", "2", "3"})
        dPKMMoves.Add(96, New String() {"Meditate", "The user meditates to awaken the power deep within its body and raise its Attack stat.", "10", "0", "14", "0", "40", "0", "16", "0", "8", "2", "1"})
        dPKMMoves.Add(97, New String() {"Agility", "The user relaxes and lightens its body to move faster. It sharply boosts the Speed stat.", "52", "0", "14", "0", "30", "0", "16", "0", "136", "2", "0"})
        dPKMMoves.Add(98, New String() {"Quick Attack", "The user lunges at the foe at a speed that makes it almost invisible. It is sure to strike first.", "103", "40", "0", "100", "30", "0", "0", "1", "115", "0", "0"})
        dPKMMoves.Add(99, New String() {"Rage", "While this move is in use, it gains attack power each time the user is hit in battle.", "81", "20", "0", "100", "20", "0", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(100, New String() {"Teleport", "Use it to flee from any wild Pokémon. It may also be used to warp to the last Poké Center visited.", "153", "0", "14", "0", "20", "0", "16", "0", "0", "2", "0"})
        dPKMMoves.Add(101, New String() {"Night Shade", "The user makes the foe see a mirage. It inflicts damage matching the user's level.", "87", "1", "7", "100", "15", "0", "0", "0", "50", "1", "3"})
        dPKMMoves.Add(102, New String() {"Mimic", "The user copies the move last used by the foe. The move can be used for the rest of the battle.", "82", "0", "0", "0", "10", "0", "0", "0", "2", "2", "2"})
        dPKMMoves.Add(103, New String() {"Screech", "An earsplitting screech is emitted to sharply reduce the foe's Defense stat.", "59", "0", "0", "85", "40", "0", "0", "0", "86", "2", "3"})
        dPKMMoves.Add(104, New String() {"Double Team", "By moving rapidly, the user makes illusory copies of itself to raise its evasiveness.", "16", "0", "0", "0", "15", "0", "16", "0", "72", "2", "0"})
        dPKMMoves.Add(105, New String() {"Recover", "A self-healing move. The user restores its own HP by up to half of its max HP.", "32", "0", "0", "0", "10", "0", "16", "0", "72", "2", "3"})
        dPKMMoves.Add(106, New String() {"Harden", "The user stiffens all the muscles in its body to raise its Defense stat.", "11", "0", "0", "0", "30", "0", "16", "0", "72", "2", "4"})
        dPKMMoves.Add(107, New String() {"Minimize", "The user compresses its body to make itself look smaller. The user's evasion stat is boosted.", "108", "0", "0", "0", "20", "0", "16", "0", "8", "2", "2"})
        dPKMMoves.Add(108, New String() {"SmokeScreen", "The user releases an obscuring cloud of smoke or ink. It reduces the foe's accuracy.", "23", "0", "0", "100", "20", "0", "0", "0", "22", "2", "3"})
        dPKMMoves.Add(109, New String() {"Confuse Ray", "The foe is exposed to a sinister ray that triggers confusion.", "49", "0", "7", "100", "10", "0", "0", "0", "22", "2", "3"})
        dPKMMoves.Add(110, New String() {"Withdraw", "The user withdraws its body into its hard shell, raising its Defense stat.", "11", "0", "11", "0", "40", "0", "16", "0", "72", "2", "2"})
        dPKMMoves.Add(111, New String() {"Defense Curl", "The user curls up to conceal weak spots and raise its Defense stat.", "156", "0", "0", "0", "40", "0", "16", "0", "72", "2", "2"})
        dPKMMoves.Add(112, New String() {"Barrier", "The user throws up a sturdy wall that sharply raises its Defense stat.", "51", "0", "14", "0", "30", "0", "16", "0", "72", "2", "0"})
        dPKMMoves.Add(113, New String() {"Light Screen", "A wondrous wall of light is put up to suppress damage from special attacks for five turns.", "35", "0", "14", "0", "30", "0", "32", "0", "72", "2", "1"})
        dPKMMoves.Add(114, New String() {"Haze", "The user creates a haze that eliminates every stat change among all the Pokémon engaged in battle.", "25", "0", "15", "0", "30", "0", "64", "0", "0", "2", "1"})
        dPKMMoves.Add(115, New String() {"Reflect", "A wondrous wall of light is put up to suppress damage from physical attacks for five turns.", "65", "0", "14", "0", "20", "0", "32", "0", "72", "2", "3"})
        dPKMMoves.Add(116, New String() {"Focus Energy", "The user takes a deep breath and focuses to raise the critical-hit ratio of its attacks.", "47", "0", "0", "0", "30", "0", "16", "0", "72", "2", "0"})
        dPKMMoves.Add(117, New String() {"Bide", "The user endures attacks for two turns, then strikes back to cause double the damage taken.", "26", "1", "0", "0", "10", "0", "16", "1", "99", "0", "4"})
        dPKMMoves.Add(118, New String() {"Metronome", "The user waggles a finger and stimulates the brain into randomly using nearly any move.", "83", "0", "0", "0", "10", "0", "1", "0", "64", "2", "2"})
        dPKMMoves.Add(119, New String() {"Mirror Move", "The user counters the foe by mimicking the move last used by the foe.", "9", "0", "2", "0", "20", "0", "1", "0", "0", "2", "3"})
        dPKMMoves.Add(120, New String() {"Selfdestruct", "The user blows up to inflict damage on all Pokémon in battle. The user faints upon using this move.", "7", "200", "0", "100", "5", "0", "8", "0", "50", "0", "1"})
        dPKMMoves.Add(121, New String() {"Egg Bomb", "A large egg is hurled with maximum force at the foe to inflict damage.", "0", "100", "0", "75", "10", "0", "0", "0", "50", "0", "4"})
        dPKMMoves.Add(122, New String() {"Lick", "The foe is licked with a long tongue, causing damage. It may also paralyze the target.", "6", "20", "7", "100", "30", "30", "0", "0", "83", "0", "4"})
        dPKMMoves.Add(123, New String() {"Smog", "The foe is attacked with a discharge of filthy gases. It may also poison the target.", "2", "20", "3", "70", "20", "40", "0", "0", "18", "1", "4"})
        dPKMMoves.Add(124, New String() {"Sludge", "Unsanitary sludge is hurled at the foe. It may also poison the target.", "2", "65", "3", "100", "20", "30", "0", "0", "18", "1", "4"})
        dPKMMoves.Add(125, New String() {"Bone Club", "The user clubs the foe with a bone. It may also make the target flinch.", "31", "65", "4", "85", "20", "10", "0", "0", "82", "0", "4"})
        dPKMMoves.Add(126, New String() {"Fire Blast", "The foe is attacked with an intense blast of all-consuming fire. It may also leave the target with a burn.", "4", "120", "10", "85", "5", "10", "0", "0", "146", "1", "1"})
        dPKMMoves.Add(127, New String() {"Waterfall", "The user charges the foe at an awesome speed. It can also be used to climb a waterfall.", "31", "80", "11", "100", "15", "20", "0", "0", "179", "0", "4"})
        dPKMMoves.Add(128, New String() {"Clamp", "The foe is clamped and squeezed by the user's very thick and sturdy shell for two to five turns.", "42", "35", "11", "75", "10", "0", "0", "0", "51", "0", "4"})
        dPKMMoves.Add(129, New String() {"Swift", "Star-shaped rays are shot at the foe. This attack never misses.", "17", "60", "0", "0", "20", "0", "4", "0", "50", "1", "0"})
        dPKMMoves.Add(130, New String() {"Skull Bash", "The user tucks in its head to raise its Defense in the first turn, then rams the foe on the next turn.", "145", "100", "0", "100", "15", "100", "0", "0", "51", "0", "4"})
        dPKMMoves.Add(131, New String() {"Spike Cannon", "Sharp spikes are fired at the foe to strike two to five times in rapid succession.", "29", "20", "0", "100", "15", "0", "0", "0", "50", "0", "0"})
        dPKMMoves.Add(132, New String() {"Constrict", "The foe is attacked with long, creeping tentacles or vines. It may also lower the target's Speed.", "70", "10", "0", "100", "35", "10", "0", "0", "19", "0", "4"})
        dPKMMoves.Add(133, New String() {"Amnesia", "The user temporarily empties its mind to forget its concerns. It sharply raises the user's Sp. Def stat.", "54", "0", "14", "0", "20", "0", "16", "0", "8", "2", "2"})
        dPKMMoves.Add(134, New String() {"Kinesis", "The user distracts the foe by bending a spoon. It may lower the target's accuracy.", "23", "0", "14", "80", "15", "0", "0", "0", "86", "2", "3"})
        dPKMMoves.Add(135, New String() {"Softboiled", "The user restores its own HP by up to half of its max HP. May be used in the field to heal HP.", "32", "0", "0", "0", "10", "0", "16", "0", "72", "2", "1"})
        dPKMMoves.Add(136, New String() {"Hi Jump Kick", "The foe is attacked with a knee kick from a jump. If it misses, the user is hurt instead.", "45", "100", "1", "90", "20", "0", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(137, New String() {"Glare", "The user intimidates the foe with the pattern on its belly to cause paralysis.", "67", "0", "0", "75", "30", "0", "0", "0", "22", "2", "4"})
        dPKMMoves.Add(138, New String() {"Dream Eater", "An attack that works only on a sleeping foe. It absorbs half the damage caused to heal the user's HP.", "8", "100", "14", "100", "15", "0", "0", "0", "18", "1", "3"})
        dPKMMoves.Add(139, New String() {"Poison Gas", "A cloud of poison gas is sprayed in the foe's face. It may poison the target.", "66", "0", "3", "55", "40", "0", "0", "0", "22", "2", "3"})
        dPKMMoves.Add(140, New String() {"Barrage", "Round objects are hurled at the foe to strike two to five times in a row.", "29", "15", "0", "85", "20", "0", "0", "0", "114", "0", "4"})
        dPKMMoves.Add(141, New String() {"Leech Life", "A blood-draining attack. The user's HP is restored by half the damage taken by the target.", "3", "20", "6", "100", "15", "0", "0", "0", "19", "0", "3"})
        dPKMMoves.Add(142, New String() {"Lovely Kiss", "With a scary face, the user forces a kiss on the foe. It may make the target fall asleep.", "1", "0", "0", "75", "10", "0", "0", "0", "22", "2", "1"})
        dPKMMoves.Add(143, New String() {"Sky Attack", "A second-turn attack move with a high critical-hit ratio. It may also make the target flinch.", "75", "140", "2", "90", "5", "30", "0", "0", "50", "0", "0"})
        dPKMMoves.Add(144, New String() {"Transform", "The user transforms into a copy of the foe right down to having the same move set.", "57", "0", "0", "0", "10", "0", "0", "0", "64", "2", "3"})
        dPKMMoves.Add(145, New String() {"Bubble", "A spray of countless bubbles is jetted at the foe. It may also lower the target's Speed stat.", "70", "20", "11", "100", "30", "10", "4", "0", "18", "1", "2"})
        dPKMMoves.Add(146, New String() {"Dizzy Punch", "The foe is hit with a rhythmically launched punch that may also leave it confused.", "76", "70", "0", "100", "10", "20", "0", "0", "19", "0", "0"})
        dPKMMoves.Add(147, New String() {"Spore", "The user scatters bursts of spores that induce sleep.", "1", "0", "12", "100", "15", "0", "0", "0", "22", "2", "1"})
        dPKMMoves.Add(148, New String() {"Flash", "The user flashes a light that cuts the foe's accuracy. It can also be used to illuminate caves.", "23", "0", "0", "100", "20", "0", "0", "0", "150", "2", "1"})
        dPKMMoves.Add(149, New String() {"Psywave", "The foe is attacked with an odd, hot energy wave. The attack varies in intensity.", "88", "1", "14", "80", "15", "0", "0", "0", "50", "1", "3"})
        dPKMMoves.Add(150, New String() {"Splash", "The user just flops and splashes around to no effect at all...", "85", "0", "0", "0", "40", "0", "16", "0", "64", "2", "2"})
        dPKMMoves.Add(151, New String() {"Acid Armor", "The user alters its cellular structure to liquefy itself, sharply raising its Defense stat.", "51", "0", "3", "0", "40", "0", "16", "0", "72", "2", "4"})
        dPKMMoves.Add(152, New String() {"Crabhammer", "The foe is hammered with a large pincer. This move has a high critical-hit ratio.", "43", "90", "11", "85", "10", "0", "0", "0", "115", "0", "4"})
        dPKMMoves.Add(153, New String() {"Explosion", "The user explodes to inflict damage on all Pokémon in battle. The user faints upon using this move.", "7", "250", "0", "100", "5", "0", "8", "0", "50", "0", "1"})
        dPKMMoves.Add(154, New String() {"Fury Swipes", "The foe is raked with sharp claws or scythes for two to five times in quick succession.", "29", "18", "0", "80", "15", "0", "0", "0", "115", "0", "4"})
        dPKMMoves.Add(155, New String() {"Bonemerang", "The user throws the bone it holds. The bone loops to hit the foe twice, coming and going.", "44", "50", "4", "90", "10", "0", "0", "0", "114", "0", "4"})
        dPKMMoves.Add(156, New String() {"Rest", "The user goes to sleep for two turns. It fully restores the user's HP and heals any status problem.", "37", "0", "14", "0", "10", "0", "16", "0", "72", "2", "2"})
        dPKMMoves.Add(157, New String() {"Rock Slide", "Large boulders are hurled at the foe to inflict damage. It may also make the target flinch.", "31", "75", "5", "90", "10", "30", "4", "0", "18", "0", "4"})
        dPKMMoves.Add(158, New String() {"Hyper Fang", "The user bites hard on the foe with its sharp front fangs. It may also make the target flinch.", "31", "80", "0", "90", "15", "10", "0", "0", "19", "0", "0"})
        dPKMMoves.Add(159, New String() {"Sharpen", "The user reduces its polygon count to make itself more jagged, raising the Attack stat.", "10", "0", "0", "0", "30", "0", "16", "0", "72", "2", "2"})
        dPKMMoves.Add(160, New String() {"Conversion", "The user changes its type to become the same type as one of its moves.", "30", "0", "0", "0", "30", "0", "16", "0", "64", "2", "1"})
        dPKMMoves.Add(161, New String() {"Tri Attack", "The user strikes with a simultaneous three- beam attack. May also paralyze, burn, or freeze the target.", "36", "80", "0", "100", "10", "20", "0", "0", "18", "1", "1"})
        dPKMMoves.Add(162, New String() {"Super Fang", "The user chomps hard on the foe with its sharp front fangs. It cuts the target's HP to half.", "40", "1", "0", "90", "10", "0", "0", "0", "83", "0", "4"})
        dPKMMoves.Add(163, New String() {"Slash", "The foe is attacked with a slash of claws, etc. It has a high critical-hit ratio.", "43", "70", "0", "100", "20", "0", "0", "0", "115", "0", "0"})
        dPKMMoves.Add(164, New String() {"Substitute", "The user makes a copy of itself using some of its HP. The copy serves as the user's decoy.", "79", "0", "0", "0", "10", "0", "16", "0", "72", "2", "3"})
        dPKMMoves.Add(165, New String() {"Struggle", "An attack that is used in desperation only if the user has no PP. It also hurts the user slightly.", "254", "50", "0", "0", "1", "0", "0", "0", "99", "0", "0"})
        dPKMMoves.Add(166, New String() {"Sketch", "It enables the user to learn a move used by the foe. Once used, the move Sketch disappears.", "95", "0", "0", "0", "1", "0", "0", "0", "0", "2", "3"})
        dPKMMoves.Add(167, New String() {"Triple Kick", "A consecutive three- kick attack that becomes more powerful with each  successive hit.", "104", "10", "1", "90", "10", "0", "0", "0", "115", "0", "0"})
        dPKMMoves.Add(168, New String() {"Thief", "The user attacks and steals the foe's held item simultaneously. It can't steal if the user holds an item.", "105", "40", "17", "100", "10", "0", "0", "0", "83", "0", "4"})
        dPKMMoves.Add(169, New String() {"Spider Web", "The user ensnares the foe with a thin, gooey silk so it can't flee from battle.", "106", "0", "6", "0", "10", "0", "0", "0", "22", "2", "3"})
        dPKMMoves.Add(170, New String() {"Mind Reader", "The user senses the foe's movements with its mind to ensure its next attack does not miss.", "94", "0", "0", "0", "5", "0", "0", "0", "18", "2", "3"})
        dPKMMoves.Add(171, New String() {"Nightmare", "A sleeping foe is shown a nightmare that inflicts some damage every turn.", "107", "0", "7", "100", "15", "0", "0", "0", "18", "2", "3"})
        dPKMMoves.Add(172, New String() {"Flame Wheel", "The user cloaks itself in fire and charges at the foe. It may also leave the target with a burn.", "125", "60", "10", "100", "25", "10", "0", "0", "19", "0", "1"})
        dPKMMoves.Add(173, New String() {"Snore", "An attack that can be used only if the user is asleep. The harsh noise may also make the foe flinch.", "92", "40", "0", "100", "15", "30", "0", "0", "50", "1", "2"})
        dPKMMoves.Add(174, New String() {"Curse", "A move that works differently for the Ghost type than for all the other types.", "109", "0", "9", "0", "10", "0", "0", "0", "64", "2", "4"})
        dPKMMoves.Add(175, New String() {"Flail", "The user flails about aimlessly to attack. It becomes more powerful the less HP the user has.", "99", "1", "0", "100", "15", "0", "0", "0", "115", "0", "2"})
        dPKMMoves.Add(176, New String() {"Conversion 2", "The user changes its type to make itself resistant to the type of the attack it last took.", "93", "0", "0", "0", "30", "0", "16", "0", "0", "2", "1"})
        dPKMMoves.Add(177, New String() {"Aeroblast", "A vortex of air is shot at the foe to inflict damage. It has a high critical-hit ratio.", "43", "100", "2", "95", "5", "0", "0", "0", "178", "1", "0"})
        dPKMMoves.Add(178, New String() {"Cotton Spore", "The user releases cottonlike spores that cling to the foe, sharply reducing its Speed stat.", "60", "0", "12", "85", "40", "0", "0", "0", "86", "2", "1"})
        dPKMMoves.Add(179, New String() {"Reversal", "An all-out attack that becomes more powerful the less HP the user has.", "99", "1", "1", "100", "15", "0", "0", "0", "115", "0", "0"})
        dPKMMoves.Add(180, New String() {"Spite", "The user looses its grudge on the move last used by the foe by cutting 2-5 PP from it.", "100", "0", "7", "100", "10", "0", "0", "0", "18", "2", "4"})
        dPKMMoves.Add(181, New String() {"Powder Snow", "The user attacks with a chilling gust of powdery snow. It may also freeze the target.", "5", "40", "15", "100", "25", "10", "4", "0", "18", "1", "1"})
        dPKMMoves.Add(182, New String() {"Protect", "It enables the user to evade all attacks. Its chance of failing rises if it is used in succession.", "111", "0", "0", "0", "10", "0", "16", "3", "0", "2", "2"})
        dPKMMoves.Add(183, New String() {"Mach Punch", "The user throws a punch at blinding speed. It is certain to strike first.", "103", "40", "1", "100", "30", "0", "0", "1", "115", "0", "0"})
        dPKMMoves.Add(184, New String() {"Scary Face", "The user frightens the foe with a scary face to sharply reduce its Speed stat.", "60", "0", "0", "90", "10", "0", "0", "0", "22", "2", "4"})
        dPKMMoves.Add(185, New String() {"Faint Attack", "The user draws up to the foe disarmingly, then throws a sucker punch. It hits without fail.", "17", "60", "17", "0", "20", "0", "0", "0", "51", "0", "3"})
        dPKMMoves.Add(186, New String() {"Sweet Kiss", "The user kisses the foe with a sweet, angelic cuteness that causes confusion.", "49", "0", "0", "75", "10", "0", "0", "0", "22", "2", "2"})
        dPKMMoves.Add(187, New String() {"Belly Drum", "The user maximizes its Attack stat in exchange for HP equal to half its max HP.", "142", "0", "0", "0", "10", "0", "16", "0", "72", "2", "2"})
        dPKMMoves.Add(188, New String() {"Sludge Bomb", "The user attacks by hurling filthy sludge at the foe. It may also poison the target.", "2", "90", "3", "100", "10", "30", "0", "0", "18", "1", "4"})
        dPKMMoves.Add(189, New String() {"Mud-Slap", "The user hurls mud in the foe's face to inflict damage and lower its accuracy.", "73", "20", "4", "100", "10", "100", "0", "0", "18", "1", "2"})
        dPKMMoves.Add(190, New String() {"Octazooka", "The user attacks by spraying ink in the foe's face or eyes. It may also lower the target's accuracy.", "73", "65", "11", "85", "10", "50", "0", "0", "18", "1", "4"})
        dPKMMoves.Add(191, New String() {"Spikes", "The user lays a trap of spikes at the foe's feet. The trap hurts foes that switch into battle.", "112", "0", "4", "0", "20", "0", "128", "0", "0", "2", "3"})
        dPKMMoves.Add(192, New String() {"Zap Cannon", "The user fires an electric blast like a cannon to inflict damage and cause paralysis.", "6", "120", "13", "50", "5", "100", "0", "0", "146", "1", "0"})
        dPKMMoves.Add(193, New String() {"Foresight", "Enables the user to hit a Ghost type with any type of move. It also enables the user to hit an evasive foe.", "113", "0", "0", "0", "40", "0", "0", "0", "82", "2", "3"})
        dPKMMoves.Add(194, New String() {"Destiny Bond", "When this move is used, if the user faints, the foe that landed the knockout hit also faints.", "98", "0", "7", "0", "5", "0", "16", "0", "0", "2", "3"})
        dPKMMoves.Add(195, New String() {"Perish Song", "Any Pokémon that hears this song faints in three turns unless it switches out of battle.", "114", "0", "0", "0", "5", "0", "64", "0", "128", "2", "1"})
        dPKMMoves.Add(196, New String() {"Icy Wind", "The user attacks with a gust of chilled air. It also lowers the target's Speed stat.", "70", "55", "15", "95", "15", "100", "4", "0", "18", "1", "1"})
        dPKMMoves.Add(197, New String() {"Detect", "It enables the user to evade all attacks. Its chance of failing rises if it is used in succession.", "111", "0", "1", "0", "5", "0", "16", "3", "0", "2", "0"})
        dPKMMoves.Add(198, New String() {"Bone Rush", "The user strikes at the foe with a hard bone two to five times in a row.", "29", "25", "4", "80", "10", "0", "0", "0", "114", "0", "4"})
        dPKMMoves.Add(199, New String() {"Lock-On", "The user takes sure aim at the foe. It ensures the next attack does not fail to hit the target.", "94", "0", "0", "0", "5", "0", "0", "0", "18", "2", "3"})
        dPKMMoves.Add(200, New String() {"Outrage", "The user rampages and attacks for two to three turns. However, it then becomes confused.", "27", "120", "16", "100", "15", "0", "2", "0", "179", "0", "0"})
        dPKMMoves.Add(201, New String() {"Sandstorm", "A five-turn sand- storm is summoned to hurt all combatant types except Rock,  Ground, and Steel.", "115", "0", "5", "0", "10", "0", "64", "0", "0", "2", "4"})
        dPKMMoves.Add(202, New String() {"Giga Drain", "A nutrient-draining attack. The user's HP is restored by half the damage taken by the target.", "3", "60", "12", "100", "10", "0", "0", "0", "18", "1", "3"})
        dPKMMoves.Add(203, New String() {"Endure", "The user endures any attack, leaving 1 HP. Its chance of failing rises if it is used in succession.", "116", "0", "0", "0", "10", "0", "16", "3", "64", "2", "4"})
        dPKMMoves.Add(204, New String() {"Charm", "The user charmingly stares at the foe, making it less wary. The target's Attack is sharply lowered.", "58", "0", "0", "100", "20", "0", "0", "0", "86", "2", "2"})
        dPKMMoves.Add(205, New String() {"Rollout", "The user continually rolls into the foe over five turns. It becomes stronger each time it hits.", "117", "30", "5", "90", "20", "0", "0", "0", "51", "0", "4"})
        dPKMMoves.Add(206, New String() {"False Swipe", "A restrained attack that prevents the foe from fainting. The target is left with at least 1 HP.", "101", "40", "0", "100", "40", "0", "0", "0", "115", "0", "0"})
        dPKMMoves.Add(207, New String() {"Swagger", "The user enrages the foe into confusion. However, it also sharply raises the foe's Attack stat.", "118", "0", "0", "90", "15", "0", "0", "0", "86", "2", "2"})
        dPKMMoves.Add(208, New String() {"Milk Drink", "The user restores its own HP by up to half of its maximum HP. It may also be used to heal an ally's HP.", "32", "0", "0", "0", "10", "0", "16", "0", "8", "2", "2"})
        dPKMMoves.Add(209, New String() {"Spark", "The user throws an electrically charged tackle at the foe. It may also leave the target paralyzed.", "6", "65", "13", "100", "20", "30", "0", "0", "19", "0", "0"})
        dPKMMoves.Add(210, New String() {"Fury Cutter", "The foe is slashed with scythes or claws. Its power increases if it hits in succession.", "119", "10", "6", "95", "20", "0", "0", "0", "115", "0", "0"})
        dPKMMoves.Add(211, New String() {"Steel Wing", "The foe is hit with wings of steel. It may also raise the user's Defense stat.", "138", "70", "8", "90", "25", "10", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(212, New String() {"Mean Look", "The user affixes the foe with a dark, arresting look. The target becomes unable to flee.", "106", "0", "0", "0", "5", "0", "0", "0", "22", "2", "1"})
        dPKMMoves.Add(213, New String() {"Attract", "If it is the opposite gender of the user, the foe becomes infatuated and less likely to attack.", "120", "0", "0", "100", "15", "0", "0", "0", "22", "2", "2"})
        dPKMMoves.Add(214, New String() {"Sleep Talk", "While it is asleep, the user randomly uses one of the moves it knows.", "97", "0", "0", "0", "10", "0", "1", "0", "0", "2", "2"})
        dPKMMoves.Add(215, New String() {"Heal Bell", "The user makes a soothing bell chime to heal the status problems of all the party Pokémon.", "102", "0", "0", "0", "5", "0", "32", "0", "72", "2", "1"})
        dPKMMoves.Add(216, New String() {"Return", "A full-power attack that grows more powerful the more the user likes its Trainer.", "121", "1", "0", "100", "20", "0", "0", "0", "115", "0", "2"})
        dPKMMoves.Add(217, New String() {"Present", "The user attacks by giving the foe a booby-trapped gift. It restores HP sometimes, however.", "122", "1", "0", "90", "15", "0", "0", "0", "18", "0", "2"})
        dPKMMoves.Add(218, New String() {"Frustration", "A full-power attack that grows more powerful the less the user likes its Trainer.", "123", "1", "0", "100", "20", "0", "0", "0", "115", "0", "2"})
        dPKMMoves.Add(219, New String() {"Safeguard", "The user creates a protective field that prevents status problems for five turns.", "124", "0", "0", "0", "25", "0", "32", "0", "72", "2", "1"})
        dPKMMoves.Add(220, New String() {"Pain Split", "The user adds its HP to the foe's HP, then equally shares the combined HP with the foe.", "91", "0", "0", "0", "20", "0", "0", "0", "18", "2", "3"})
        dPKMMoves.Add(221, New String() {"Sacred Fire", "The foe is razed with a mystical fire of great intensity. It may also leave the target with a burn.", "125", "100", "10", "95", "5", "50", "0", "0", "146", "0", "1"})
        dPKMMoves.Add(222, New String() {"Magnitude", "The user looses a ground-shaking quake affecting everyone in battle. Its power varies.", "126", "1", "4", "100", "30", "0", "8", "0", "50", "0", "4"})
        dPKMMoves.Add(223, New String() {"DynamicPunch", "The foe is punched with the user's full, concentrated power. It confuses the foe if it hits.", "76", "100", "1", "50", "5", "100", "0", "0", "147", "0", "0"})
        dPKMMoves.Add(224, New String() {"Megahorn", "Utilizing its tough and impressive horn, the user rams into the foe  with no letup.", "0", "120", "6", "85", "10", "0", "0", "0", "179", "0", "0"})
        dPKMMoves.Add(225, New String() {"DragonBreath", "The user exhales a mighty gust that inflicts damage. It may also paralyze the target.", "6", "60", "16", "100", "20", "30", "0", "0", "50", "1", "0"})
        dPKMMoves.Add(226, New String() {"Baton Pass", "The user switches places with a party Pokémon in waiting, passing along any stat changes.", "127", "0", "0", "0", "40", "0", "16", "0", "0", "2", "2"})
        dPKMMoves.Add(227, New String() {"Encore", "The user compels the foe to keep using only the move it last used for two to six turns.", "90", "0", "0", "100", "5", "0", "0", "0", "18", "2", "2"})
        dPKMMoves.Add(228, New String() {"Pursuit", "An attack move that inflicts double damage if used on a foe that is switching out of battle.", "128", "40", "17", "100", "20", "0", "0", "0", "147", "0", "3"})
        dPKMMoves.Add(229, New String() {"Rapid Spin", "A spin attack that can also eliminate such moves as Bind, Wrap, Leech Seed, and Spikes.", "129", "20", "0", "100", "40", "0", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(230, New String() {"Sweet Scent", "A sweet scent that lowers the foe's evasiveness. It also lures wild Pokémon if used in grass, etc.", "24", "0", "0", "100", "20", "0", "4", "0", "22", "2", "2"})
        dPKMMoves.Add(231, New String() {"Iron Tail", "The foe is slammed with a steel-hard tail. It may also lower the target's Defense stat.", "69", "100", "8", "75", "15", "30", "0", "0", "19", "0", "0"})
        dPKMMoves.Add(232, New String() {"Metal Claw", "The foe is raked with steel claws. It may also raise the user's Attack stat.", "139", "50", "8", "95", "35", "10", "0", "0", "19", "0", "0"})
        dPKMMoves.Add(233, New String() {"Vital Throw", "The user allows the foe to attack first. In return, this throw move is guaranteed not to miss.", "78", "70", "1", "0", "10", "0", "0", "255", "51", "0", "0"})
        dPKMMoves.Add(234, New String() {"Morning Sun", "The user restores its own HP. The amount of HP regained varies with the weather.", "132", "0", "0", "0", "5", "0", "16", "0", "8", "2", "1"})
        dPKMMoves.Add(235, New String() {"Synthesis", "The user restores its own HP. The amount of HP regained varies with the weather.", "132", "0", "12", "0", "5", "0", "16", "0", "72", "2", "3"})
        dPKMMoves.Add(236, New String() {"Moonlight", "The user restores its own HP. The amount of HP regained varies with the weather.", "132", "0", "0", "0", "5", "0", "16", "0", "136", "2", "1"})
        dPKMMoves.Add(237, New String() {"Hidden Power", "A unique attack that varies in type and intensity depending on the Pokémon using it.", "135", "1", "0", "100", "15", "0", "0", "0", "50", "1", "3"})
        dPKMMoves.Add(238, New String() {"Cross Chop", "The user delivers a double chop with its forearms crossed. It has a high critical-hit ratio.", "43", "100", "1", "80", "5", "0", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(239, New String() {"Twister", "The user whips up a vicious twister to tear at the foe. It may also make the foe flinch.", "146", "40", "16", "100", "20", "20", "4", "0", "50", "1", "0"})
        dPKMMoves.Add(240, New String() {"Rain Dance", "The user summons a heavy rain that falls for five turns, powering up Water- type moves.", "136", "0", "11", "0", "5", "0", "64", "0", "0", "2", "4"})
        dPKMMoves.Add(241, New String() {"Sunny Day", "The user intensifies the sun for five turns, powering up Fire-type moves.", "137", "0", "10", "0", "5", "0", "64", "0", "0", "2", "1"})
        dPKMMoves.Add(242, New String() {"Crunch", "The user crunches up the foe with sharp fangs. It may also lower the target's Defense stat.", "69", "80", "17", "100", "15", "20", "0", "0", "19", "0", "4"})
        dPKMMoves.Add(243, New String() {"Mirror Coat", "A retaliation move that counters any special attack, inflicting double the damage taken.", "144", "1", "14", "100", "20", "0", "1", "251", "0", "1", "1"})
        dPKMMoves.Add(244, New String() {"Psych Up", "The user hypnotizes itself into copying any stat change made by the foe.", "143", "0", "0", "0", "10", "0", "0", "0", "72", "2", "3"})
        dPKMMoves.Add(245, New String() {"ExtremeSpeed", "The user charges the foe at blinding speed. This attack always goes before any other move.", "103", "80", "0", "100", "5", "0", "0", "1", "179", "0", "0"})
        dPKMMoves.Add(246, New String() {"AncientPower", "The user attacks with a prehistoric power. It may also raise all the user's stats at once.", "140", "60", "5", "100", "5", "10", "0", "0", "18", "1", "4"})
        dPKMMoves.Add(247, New String() {"Shadow Ball", "The user hurls a shadowy blob at the foe. It may also lower the foe's Sp. Def stat.", "72", "80", "7", "100", "15", "20", "0", "0", "18", "1", "3"})
        dPKMMoves.Add(248, New String() {"Future Sight", "Two turns after this move is used, the foe is attacked with a hunk of psychic energy.", "148", "80", "14", "90", "15", "0", "0", "0", "0", "1", "3"})
        dPKMMoves.Add(249, New String() {"Rock Smash", "The user slugs the foe with a shattering punch. It can also smash cracked boulders.", "69", "40", "1", "100", "15", "50", "0", "0", "19", "0", "4"})
        dPKMMoves.Add(250, New String() {"Whirlpool", "The user traps the foe inside a fast, vicious whirlpool that lasts for two to five turns.", "261", "15", "11", "70", "15", "0", "0", "0", "178", "1", "1"})
        dPKMMoves.Add(251, New String() {"Beat Up", "The user gets all the party Pokémon to attack the foe. The more party Pokémon, the more damage.", "154", "10", "17", "100", "10", "0", "0", "0", "114", "0", "3"})
        dPKMMoves.Add(252, New String() {"Fake Out", "An attack that hits first and makes the target flinch. This move works only on the first turn.", "158", "40", "0", "100", "10", "100", "0", "1", "19", "0", "2"})
        dPKMMoves.Add(253, New String() {"Uproar", "The user attacks in an uproar for two to five turns. Over that time, no one can fall asleep.", "159", "50", "0", "100", "10", "0", "2", "0", "50", "1", "2"})
        dPKMMoves.Add(254, New String() {"Stockpile", "The user charges up power, and raises both its Defense and Sp. Def. The move can be used three times.", "160", "0", "0", "0", "20", "0", "16", "0", "72", "2", "4"})
        dPKMMoves.Add(255, New String() {"Spit Up", "The power stored using the move Stockpile is released at once in an attack.", "161", "1", "0", "100", "10", "0", "0", "0", "34", "1", "4"})
        dPKMMoves.Add(256, New String() {"Swallow", "The power stored using the move Stockpile is absorbed by the user to heal its HP.", "162", "0", "0", "0", "10", "0", "16", "0", "72", "2", "4"})
        dPKMMoves.Add(257, New String() {"Heat Wave", "The user exhales a heated breath on the foe to attack. It may also leave the target with a burn.", "4", "100", "10", "90", "10", "10", "4", "0", "18", "1", "1"})
        dPKMMoves.Add(258, New String() {"Hail", "The user summons a hailstorm lasting five turns. It damages all Pokémon except the Ice type.", "164", "0", "15", "0", "10", "0", "64", "0", "2", "2", "1"})
        dPKMMoves.Add(259, New String() {"Torment", "The user torments and enrages the foe, making it incapable of using the same move twice in a row.", "165", "0", "17", "100", "15", "0", "0", "0", "82", "2", "4"})
        dPKMMoves.Add(260, New String() {"Flatter", "Flattery is used to confuse the foe. However, it also raises the target's Sp. Atk stat.", "166", "0", "17", "100", "15", "0", "0", "0", "22", "2", "3"})
        dPKMMoves.Add(261, New String() {"Will-O-Wisp", "The user shoots a sinister, bluish white flame at the foe to inflict a burn.", "167", "0", "10", "75", "15", "0", "0", "0", "22", "2", "1"})
        dPKMMoves.Add(262, New String() {"Memento", "The user faints upon using this move. In return, it sharply lowers the target's Attack and Sp. Atk.", "168", "0", "17", "100", "10", "0", "0", "0", "18", "2", "4"})
        dPKMMoves.Add(263, New String() {"Facade", "An attack move that doubles its power if the user is poisoned, paralyzed, or has a burn.", "169", "70", "0", "100", "20", "0", "0", "0", "83", "0", "2"})
        dPKMMoves.Add(264, New String() {"Focus Punch", "The user focuses its mind before launching a punch. It will fail if the user is hit before it is used.", "170", "150", "1", "100", "20", "0", "0", "253", "131", "0", "4"})
        dPKMMoves.Add(265, New String() {"SmellingSalt", "This attack inflicts double damage on a paralyzed foe. It also cures the foe's paralysis, however.", "171", "60", "0", "100", "10", "0", "0", "0", "19", "0", "3"})
        dPKMMoves.Add(266, New String() {"Follow Me", "The user draws attention to itself, making all foes take aim only at the user.", "172", "0", "0", "0", "20", "0", "16", "3", "0", "2", "2"})
        dPKMMoves.Add(267, New String() {"Nature Power", "An attack that makes use of nature's power. Its effects vary depending on the user's environment.", "173", "0", "0", "0", "20", "0", "1", "0", "64", "2", "1"})
        dPKMMoves.Add(268, New String() {"Charge", "The user boosts the power of the Electric move it uses next. It also raises the user's Sp. Def stat.", "174", "0", "13", "0", "20", "0", "16", "0", "8", "2", "3"})
        dPKMMoves.Add(269, New String() {"Taunt", "The foe is taunted into a rage that allows it to use only attack moves for two to four turns.", "175", "0", "17", "100", "20", "0", "0", "0", "82", "2", "3"})
        dPKMMoves.Add(270, New String() {"Helping Hand", "A move that boosts the power of the ally's attack in a Double Battle.", "176", "0", "0", "0", "20", "0", "0", "5", "0", "2", "3"})
        dPKMMoves.Add(271, New String() {"Trick", "The user catches the foe off guard and swaps the foe's held item with its own.", "177", "0", "14", "100", "10", "0", "0", "0", "18", "2", "3"})
        dPKMMoves.Add(272, New String() {"Role Play", "The user mimics the foe completely, copying the foe's natural ability.", "178", "0", "14", "0", "10", "0", "0", "0", "0", "2", "2"})
        dPKMMoves.Add(273, New String() {"Wish", "A self-healing move. The user restores its own HP by up to half of its maximum HP in the next turn.", "179", "0", "0", "0", "10", "0", "16", "0", "128", "2", "2"})
        dPKMMoves.Add(274, New String() {"Assist", "The user hurriedly and randomly uses a move among those known by other Pokémon in the party.", "180", "0", "0", "0", "20", "0", "1", "0", "0", "2", "2"})
        dPKMMoves.Add(275, New String() {"Ingrain", "The user lays roots that restore HP on every turn. Because it is rooted, it can't switch out.", "181", "0", "12", "0", "20", "0", "16", "0", "72", "2", "3"})
        dPKMMoves.Add(276, New String() {"Superpower", "The user attacks the foe with great power. However, it also lowers the user's Attack and Defense.", "182", "120", "1", "100", "5", "0", "0", "0", "147", "0", "4"})
        dPKMMoves.Add(277, New String() {"Magic Coat", "A barrier reflects back to the foe moves like Leech Seed and moves that damage status.", "183", "0", "14", "0", "15", "0", "1", "4", "0", "2", "1"})
        dPKMMoves.Add(278, New String() {"Recycle", "The user recycles a single-use item that has been used in battle so it can be used again.", "184", "0", "0", "0", "10", "0", "16", "0", "64", "2", "3"})
        dPKMMoves.Add(279, New String() {"Revenge", "An attack move that inflicts double the damage if the user has been hurt by the foe in the same turn.", "185", "60", "1", "100", "10", "0", "0", "252", "51", "0", "4"})
        dPKMMoves.Add(280, New String() {"Brick Break", "The user attacks with tough fists, etc. It can also break any barrier such as Light Screen and Reflect.", "186", "75", "1", "100", "15", "0", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(281, New String() {"Yawn", "The user lets loose a huge yawn that lulls the foe into falling asleep on the next turn.", "187", "0", "0", "0", "10", "0", "0", "0", "22", "2", "2"})
        dPKMMoves.Add(282, New String() {"Knock Off", "The user slaps down the foe's held item, preventing the item from being used during the battle.", "188", "20", "17", "100", "20", "0", "0", "0", "19", "0", "3"})
        dPKMMoves.Add(283, New String() {"Endeavor", "An attack move that cuts down the foe's HP to equal the user's HP.", "189", "1", "0", "100", "5", "0", "0", "0", "115", "0", "4"})
        dPKMMoves.Add(284, New String() {"Eruption", "The user attacks in an explosive fury. The lower the user's HP, the less powerful this attack becomes.", "190", "150", "10", "100", "5", "0", "4", "0", "50", "1", "1"})
        dPKMMoves.Add(285, New String() {"Skill Swap", "The user employs its psychic power to exchange abilities with the foe.", "191", "0", "14", "0", "10", "0", "0", "0", "18", "2", "3"})
        dPKMMoves.Add(286, New String() {"Imprison", "If the foe knows any move also known by the user, the foe is prevented from using it.", "192", "0", "14", "0", "10", "0", "16", "0", "0", "2", "3"})
        dPKMMoves.Add(287, New String() {"Refresh", "The user rests to cure itself of a poisoning, burn, or paralysis.", "193", "0", "0", "0", "20", "0", "16", "0", "8", "2", "2"})
        dPKMMoves.Add(288, New String() {"Grudge", "If the user faints, the user's grudge fully depletes the PP of the foe's move that knocked it out.", "194", "0", "7", "0", "5", "0", "16", "0", "0", "2", "4"})
        dPKMMoves.Add(289, New String() {"Snatch", "The user steals the effects of any healing or status- changing move the foe attempts to use.", "195", "0", "17", "0", "10", "0", "1", "4", "0", "2", "3"})
        dPKMMoves.Add(290, New String() {"Secret Power", "The user attacks with a secret power. Its added effects vary depending on the user's environment.", "197", "70", "0", "100", "20", "30", "0", "0", "18", "0", "3"})
        dPKMMoves.Add(291, New String() {"Dive", "A two-turn attack. The user dives underwater on the first turn, then hits on the next turn.", "255", "80", "11", "100", "10", "0", "0", "0", "51", "0", "1"})
        dPKMMoves.Add(292, New String() {"Arm Thrust", "The user looses a flurry of open-palmed arm thrusts that hit two to five times in a row.", "29", "15", "1", "100", "20", "0", "0", "0", "115", "0", "4"})
        dPKMMoves.Add(293, New String() {"Camouflage", "The user's type is changed depending on its environment, such as at water's edge, in grass, or in a cave.", "213", "0", "0", "0", "20", "0", "16", "0", "72", "2", "3"})
        dPKMMoves.Add(294, New String() {"Tail Glow", "The user stares at flashing lights to focus its mind, sharply raising its Sp. Atk stat.", "53", "0", "6", "0", "20", "0", "16", "0", "8", "2", "1"})
        dPKMMoves.Add(295, New String() {"Luster Purge", "The user looses a damaging burst of light. It may also reduce the target's Sp. Def stat.", "72", "70", "14", "100", "5", "50", "0", "0", "18", "1", "3"})
        dPKMMoves.Add(296, New String() {"Mist Ball", "A mistlike flurry of down envelops and damages the foe. It may also lower the target's Sp. Atk.", "71", "70", "14", "100", "5", "50", "0", "0", "146", "1", "3"})
        dPKMMoves.Add(297, New String() {"FeatherDance", "The user covers the foe with a mass of down that sharply lowers the Attack stat.", "58", "0", "2", "100", "15", "0", "0", "0", "22", "2", "1"})
        dPKMMoves.Add(298, New String() {"Teeter Dance", "The user performs a wobbly dance that confuses all the Pokémon in battle.", "199", "0", "0", "100", "20", "0", "8", "0", "18", "2", "2"})
        dPKMMoves.Add(299, New String() {"Blaze Kick", "The user launches a kick with a high critical-hit ratio. It may also leave the target with a burn.", "200", "85", "10", "90", "10", "10", "0", "0", "19", "0", "1"})
        dPKMMoves.Add(300, New String() {"Mud Sport", "The user covers itself with mud. It weakens Electric- type moves while the user is in the battle.", "201", "0", "4", "0", "15", "0", "64", "0", "0", "2", "2"})
        dPKMMoves.Add(301, New String() {"Ice Ball", "The user continually rolls into the foe over five turns. It becomes stronger each time it hits.", "117", "30", "15", "90", "20", "0", "0", "0", "51", "0", "1"})
        dPKMMoves.Add(302, New String() {"Needle Arm", "The user attacks by wildly swinging its thorny arms. It may also make the target flinch.", "31", "60", "12", "100", "15", "30", "0", "0", "19", "0", "3"})
        dPKMMoves.Add(303, New String() {"Slack Off", "The user slacks off, restoring its own HP by up to half of its maximum HP.", "32", "0", "0", "0", "10", "0", "16", "0", "72", "2", "2"})
        dPKMMoves.Add(304, New String() {"Hyper Voice", "The user lets loose a horribly echoing shout with the power to inflict damage.", "0", "90", "0", "100", "10", "0", "4", "0", "18", "1", "0"})
        dPKMMoves.Add(305, New String() {"Poison Fang", "The user bites the foe with toxic fangs. It may also leave the foe badly poisoned.", "202", "50", "3", "100", "15", "30", "0", "0", "19", "0", "3"})
        dPKMMoves.Add(306, New String() {"Crush Claw", "The user slashes the foe with hard and sharp claws. It may also lower the target's Defense.", "69", "75", "0", "95", "10", "50", "0", "0", "19", "0", "0"})
        dPKMMoves.Add(307, New String() {"Blast Burn", "The foe is razed by a fiery explosion. The user must rest on the next turn, however.", "80", "150", "10", "90", "5", "0", "0", "0", "50", "1", "1"})
        dPKMMoves.Add(308, New String() {"Hydro Cannon", "The foe is hit with a watery blast. The user must rest on the next turn, however.", "80", "150", "11", "90", "5", "0", "0", "0", "178", "1", "1"})
        dPKMMoves.Add(309, New String() {"Meteor Mash", "The foe is hit with a hard punch fired like a meteor. It may also raise the user's Attack.", "139", "100", "8", "85", "10", "20", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(310, New String() {"Astonish", "The user attacks the foe while shouting in a startling fashion. It may also make the target flinch.", "31", "30", "7", "100", "15", "30", "0", "0", "83", "0", "3"})
        dPKMMoves.Add(311, New String() {"Weather Ball", "An attack move that varies in power and type depending on the weather.", "203", "50", "0", "100", "10", "0", "0", "0", "50", "1", "3"})
        dPKMMoves.Add(312, New String() {"Aromatherapy", "The user releases a soothing scent that heals all status problems affecting the user's party.", "102", "0", "12", "0", "5", "0", "32", "0", "8", "2", "3"})
        dPKMMoves.Add(313, New String() {"Fake Tears", "The user feigns crying to make the foe feel flustered, sharply lowering its Sp. Def stat.", "62", "0", "17", "100", "20", "0", "0", "0", "86", "2", "3"})
        dPKMMoves.Add(314, New String() {"Air Cutter", "The user launches razorlike wind to slash the foe. It has a high critical-hit ratio.", "43", "55", "2", "95", "25", "0", "4", "0", "50", "1", "0"})
        dPKMMoves.Add(315, New String() {"Overheat", "The user attacks the foe at full power. The attack's recoil sharply reduces the user's Sp. Atk stat.", "204", "140", "10", "90", "5", "100", "0", "0", "178", "1", "1"})
        dPKMMoves.Add(316, New String() {"Odor Sleuth", "Enables the user to hit a Ghost type with any type of move. It also enables the user to hit an evasive foe.", "113", "0", "0", "0", "40", "0", "0", "0", "82", "2", "3"})
        dPKMMoves.Add(317, New String() {"Rock Tomb", "Boulders are hurled at the foe. It also lowers the foe's Speed by preventing its movement.", "70", "50", "5", "80", "10", "100", "0", "0", "18", "0", "3"})
        dPKMMoves.Add(318, New String() {"Silver Wind", "The foe is attacked with powdery scales blown by wind. It may also raise all the user's stats.", "140", "60", "6", "100", "5", "10", "0", "0", "50", "1", "1"})
        dPKMMoves.Add(319, New String() {"Metal Sound", "A horrible sound like scraping metal is emitted to sharply reduce the foe's Sp. Def stat.", "62", "0", "8", "85", "40", "0", "0", "0", "22", "2", "3"})
        dPKMMoves.Add(320, New String() {"GrassWhistle", "The user plays a pleasant melody that lulls the foe into a deep sleep.", "1", "0", "12", "55", "15", "0", "0", "0", "22", "2", "3"})
        dPKMMoves.Add(321, New String() {"Tickle", "The user tickles the foe into laughing, reducing its Attack and Defense stats.", "205", "0", "0", "100", "20", "0", "0", "0", "86", "2", "2"})
        dPKMMoves.Add(322, New String() {"Cosmic Power", "The user absorbs a mystical power from space to raise its Defense and Sp. Def stats.", "206", "0", "14", "0", "20", "0", "16", "0", "136", "2", "0"})
        dPKMMoves.Add(323, New String() {"Water Spout", "The user spouts water to damage the foe. The lower the user's HP, the less powerful it becomes.", "190", "150", "11", "100", "5", "0", "4", "0", "18", "1", "1"})
        dPKMMoves.Add(324, New String() {"Signal Beam", "The user attacks with a sinister beam of light. It may also confuse the target.", "76", "75", "6", "100", "15", "10", "0", "0", "50", "1", "1"})
        dPKMMoves.Add(325, New String() {"Shadow Punch", "The user throws a punch at the foe from the shadows. The punch lands without fail.", "17", "60", "7", "0", "20", "0", "0", "0", "51", "0", "3"})
        dPKMMoves.Add(326, New String() {"Extrasensory", "The user attacks with an odd, unseeable power. It may also make the foe flinch.", "31", "80", "14", "100", "30", "10", "0", "0", "18", "1", "0"})
        dPKMMoves.Add(327, New String() {"Sky Uppercut", "The user attacks the foe with an uppercut thrown skyward with force.", "207", "85", "1", "90", "15", "0", "0", "0", "179", "0", "0"})
        dPKMMoves.Add(328, New String() {"Sand Tomb", "The user traps the foe inside a harshly raging sandstorm for two to five turns.", "42", "15", "4", "70", "15", "0", "0", "0", "50", "0", "3"})
        dPKMMoves.Add(329, New String() {"Sheer Cold", "The foe is attacked with a blast of absolute-zero cold. The foe instantly faints if it hits.", "38", "1", "15", "30", "5", "0", "0", "0", "18", "1", "1"})
        dPKMMoves.Add(330, New String() {"Muddy Water", "The user attacks by shooting out muddy water. It may also lower the foe's accuracy.", "73", "95", "11", "85", "10", "30", "4", "0", "50", "1", "4"})
        dPKMMoves.Add(331, New String() {"Bullet Seed", "The user forcefully shoots seeds at the foe. Two to five seeds are shot in rapid succession.", "29", "10", "12", "100", "30", "0", "0", "0", "114", "0", "0"})
        dPKMMoves.Add(332, New String() {"Aerial Ace", "The user confounds the foe with speed, then slashes. The attack lands without fail.", "17", "60", "2", "0", "20", "0", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(333, New String() {"Icicle Spear", "The user launches sharp icicles at the foe. It strikes two to five times in a row.", "29", "10", "15", "100", "30", "0", "0", "0", "114", "0", "1"})
        dPKMMoves.Add(334, New String() {"Iron Defense", "The user hardens its body's surface like iron, sharply raising its Defense stat.", "51", "0", "8", "0", "15", "0", "16", "0", "72", "2", "4"})
        dPKMMoves.Add(335, New String() {"Block", "The user blocks the foe's way with arms spread wide to prevent escape.", "106", "0", "0", "0", "5", "0", "0", "0", "22", "2", "2"})
        dPKMMoves.Add(336, New String() {"Howl", "The user howls loudly to raise its spirit, boosting its Attack stat.", "10", "0", "0", "0", "40", "0", "16", "0", "8", "2", "0"})
        dPKMMoves.Add(337, New String() {"Dragon Claw", "The user slashes the foe with huge, sharp claws.", "0", "80", "16", "100", "15", "0", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(338, New String() {"Frenzy Plant", "The foe is slammed with an enormous tree. The user can't move on the next turn.", "80", "150", "12", "90", "5", "0", "0", "0", "178", "1", "0"})
        dPKMMoves.Add(339, New String() {"Bulk Up", "The user tenses its muscles to bulk up its body, boosting both its Attack and Defense stats.", "208", "0", "1", "0", "20", "0", "16", "0", "72", "2", "1"})
        dPKMMoves.Add(340, New String() {"Bounce", "The user bounces up high, then drops on the foe on the second turn. It may also paralyze the foe.", "263", "85", "2", "85", "5", "30", "0", "0", "51", "0", "2"})
        dPKMMoves.Add(341, New String() {"Mud Shot", "The user attacks by hurling a blob of mud at the foe. It also reduces the target's Speed.", "70", "55", "4", "95", "15", "100", "0", "0", "50", "1", "4"})
        dPKMMoves.Add(342, New String() {"Poison Tail", "An attack with a high critical-hit ratio. This tail  attack may also  poison the foe.", "209", "50", "3", "100", "25", "10", "0", "0", "51", "0", "3"})
        dPKMMoves.Add(343, New String() {"Covet", "The user endearingly approaches the foe, then steals the item the foe is holding.", "105", "40", "0", "100", "40", "0", "0", "0", "19", "0", "2"})
        dPKMMoves.Add(344, New String() {"Volt Tackle", "The user electrifies itself, then charges at the foe. It causes considerable damage to the user as well.", "262", "120", "13", "100", "15", "10", "0", "0", "179", "0", "0"})
        dPKMMoves.Add(345, New String() {"Magical Leaf", "The user scatters curious leaves that chase the foe. This attack will not miss.", "17", "60", "12", "0", "20", "0", "0", "0", "50", "1", "1"})
        dPKMMoves.Add(346, New String() {"Water Sport", "The user soaks itself with water. The move weakens Fire-type moves while the user is in the battle.", "210", "0", "11", "0", "15", "0", "64", "0", "0", "2", "2"})
        dPKMMoves.Add(347, New String() {"Calm Mind", "The user quietly focuses its mind and calms its spirit to raise its Sp. Atk and Sp. Def stats.", "211", "0", "14", "0", "20", "0", "16", "0", "8", "2", "3"})
        dPKMMoves.Add(348, New String() {"Leaf Blade", "The foe is slashed with a sharp leaf. It has a high critical-hit ratio.", "43", "90", "12", "100", "15", "0", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(349, New String() {"Dragon Dance", "The user vigorously performs a mystic, powerful dance that boosts its Attack and Speed stats.", "212", "0", "16", "0", "20", "0", "16", "0", "8", "2", "0"})
        dPKMMoves.Add(350, New String() {"Rock Blast", "The user hurls hard rocks at the foe. Two to five rocks are launched in quick succession.", "29", "25", "5", "80", "10", "0", "0", "0", "50", "0", "4"})
        dPKMMoves.Add(351, New String() {"Shock Wave", "The user strikes the foe with a quick jolt of electricity. This attack cannot be evaded.", "17", "60", "13", "0", "20", "0", "0", "0", "50", "1", "0"})
        dPKMMoves.Add(352, New String() {"Water Pulse", "The user attacks the foe with a pulsing blast of water. It may also confuse the foe.", "76", "60", "11", "100", "20", "20", "0", "0", "178", "1", "1"})
        dPKMMoves.Add(353, New String() {"Doom Desire", "Two turns after this move is used, the user blasts the foe with a concentrated bundle of light.", "148", "120", "8", "85", "5", "0", "0", "0", "128", "1", "0"})
        dPKMMoves.Add(354, New String() {"Psycho Boost", "The user attacks the foe at full power. The attack's recoil sharply reduces the user's Sp. Atk stat.", "204", "140", "14", "90", "5", "100", "0", "0", "178", "1", "3"})
        dPKMMoves.Add(355, New String() {"Roost", "The user lands and rests its body. It restores the user's HP by up to half of its max HP.", "214", "0", "2", "0", "10", "0", "16", "0", "72", "2", "0"})
        dPKMMoves.Add(356, New String() {"Gravity", "Gravity is intensified for five turns, making moves involving flying unusable and negating Levitation.", "215", "0", "14", "0", "5", "0", "64", "0", "0", "2", "1"})
        dPKMMoves.Add(357, New String() {"Miracle Eye", "Enables the user to hit a Ghost type with any type of move. It also enables the user to hit an evasive foe.", "216", "0", "14", "0", "40", "0", "0", "0", "18", "2", "2"})
        dPKMMoves.Add(358, New String() {"Wake-Up Slap", "This attack inflicts high damage on a sleeping foe. It also wakes the foe up, however.", "217", "60", "1", "100", "10", "0", "0", "0", "51", "0", "3"})
        dPKMMoves.Add(359, New String() {"Hammer Arm", "The user swings and hits with its strong and heavy fist. It lowers the user's Speed, however.", "218", "100", "1", "90", "10", "0", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(360, New String() {"Gyro Ball", "The user tackles the foe with a high-speed spin. The slower the user, the greater the damage.", "219", "1", "8", "100", "5", "0", "0", "0", "51", "0", "1"})
        dPKMMoves.Add(361, New String() {"Healing Wish", "The user faints. In return, the Pokémon taking its place will have its HP restored and status cured.", "220", "0", "14", "0", "10", "0", "16", "0", "0", "2", "2"})
        dPKMMoves.Add(362, New String() {"Brine", "If the foe's HP is down to about half, this attack will hit with double the power.", "221", "65", "11", "100", "10", "0", "0", "0", "50", "1", "3"})
        dPKMMoves.Add(363, New String() {"Natural Gift", "The user draws power to attack by using its held Berry. The Berry determines its type and power.", "222", "1", "0", "100", "15", "0", "0", "0", "82", "0", "0"})
        dPKMMoves.Add(364, New String() {"Feint", "An attack that hits a foe using Protect or Detect. It also lifts the effects of those moves.", "223", "50", "0", "100", "10", "0", "0", "2", "0", "0", "1"})
        dPKMMoves.Add(365, New String() {"Pluck", "The user pecks the foe. If the foe is holding a Berry, the user plucks it and gains its effect.", "224", "60", "2", "100", "20", "0", "0", "0", "115", "0", "2"})
        dPKMMoves.Add(366, New String() {"Tailwind", "The user whips up a turbulent whirlwind that ups the Speed of all party Pokémon for three turns.", "225", "0", "2", "0", "30", "0", "32", "0", "8", "2", "3"})
        dPKMMoves.Add(367, New String() {"Acupressure", "The user applies pressure to stress points, sharply boosting one of its stats.", "226", "0", "0", "0", "30", "0", "0", "0", "72", "2", "0"})
        dPKMMoves.Add(368, New String() {"Metal Burst", "The user retaliates against the foe that last inflicted damage on it with much greater power.", "227", "1", "8", "100", "10", "0", "1", "0", "16", "0", "1"})
        dPKMMoves.Add(369, New String() {"U-turn", "After making its attack, the user rushes back to switch places with a party Pokémon in waiting.", "228", "70", "6", "100", "20", "0", "0", "0", "51", "0", "2"})
        dPKMMoves.Add(370, New String() {"Close Combat", "The user fights the foe in close without guarding itself. It also cuts the user's Defense and Sp. Def.", "229", "120", "1", "100", "5", "0", "0", "0", "179", "0", "3"})
        dPKMMoves.Add(371, New String() {"Payback", "If the user can use this attack after the foe attacks, its power is doubled.", "230", "50", "17", "100", "10", "0", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(372, New String() {"Assurance", "If the foe has already taken some damage in the same turn, this attack's power is doubled.", "231", "50", "17", "100", "10", "0", "0", "0", "51", "0", "1"})
        dPKMMoves.Add(373, New String() {"Embargo", "It prevents the foe from using its held item. Its Trainer is also prevented from using items on it.", "232", "0", "17", "100", "15", "0", "0", "0", "18", "2", "2"})
        dPKMMoves.Add(374, New String() {"Fling", "The user flings its held item at the foe to attack. Its power and effects depend on the item.", "233", "1", "17", "100", "10", "0", "0", "0", "18", "0", "4"})
        dPKMMoves.Add(375, New String() {"Psycho Shift", "Using its psychic power of suggestion, the user transfers its status problems to the target.", "234", "0", "14", "90", "10", "0", "0", "0", "18", "2", "0"})
        dPKMMoves.Add(376, New String() {"Trump Card", "The fewer PP this move has, the more power it has for attack.", "235", "1", "0", "0", "5", "0", "0", "0", "51", "1", "0"})
        dPKMMoves.Add(377, New String() {"Heal Block", "The user prevents the foe from using any HP-recovery moves for five turns.", "236", "0", "14", "100", "15", "0", "4", "0", "18", "2", "2"})
        dPKMMoves.Add(378, New String() {"Wring Out", "The user powerfully wrings the foe. The more HP the foe has, the greater this attack's power.", "237", "1", "0", "100", "5", "0", "0", "0", "51", "1", "3"})
        dPKMMoves.Add(379, New String() {"Power Trick", "The user employs its psychic power to switch its Attack with its Defense stat.", "238", "0", "14", "0", "10", "0", "16", "0", "64", "2", "0"})
        dPKMMoves.Add(380, New String() {"Gastro Acid", "The user hurls up its stomach acids on the foe. The fluid eliminates the effect of the foe's ability.", "239", "0", "3", "100", "10", "0", "0", "0", "22", "2", "1"})
        dPKMMoves.Add(381, New String() {"Lucky Chant", "The user chants an incantation toward the sky, preventing the foe from landing critical hits.", "240", "0", "0", "0", "30", "0", "32", "0", "64", "2", "2"})
        dPKMMoves.Add(382, New String() {"Me First", "The user tries to cut ahead of the foe to steal and use the foe's intended move with greater power.", "241", "0", "0", "0", "20", "0", "0", "0", "2", "2", "2"})
        dPKMMoves.Add(383, New String() {"Copycat", "The user mimics the move used immediately before it. The move fails if no other move has been used yet.", "242", "0", "0", "0", "20", "0", "1", "0", "0", "2", "0"})
        dPKMMoves.Add(384, New String() {"Power Swap", "The user employs its psychic power to switch changes to its Attack and Sp. Atk with the foe.", "243", "0", "14", "0", "10", "0", "0", "0", "18", "2", "1"})
        dPKMMoves.Add(385, New String() {"Guard Swap", "The user employs its psychic power to switch changes to its Defense and Sp. Def with the foe.", "244", "0", "14", "0", "10", "0", "0", "0", "18", "2", "2"})
        dPKMMoves.Add(386, New String() {"Punishment", "This attack's power increases the more the foe has powered up with stat changes.", "245", "1", "17", "100", "5", "0", "0", "0", "115", "0", "3"})
        dPKMMoves.Add(387, New String() {"Last Resort", "This move can be used only after the user has used all the other moves it knows in the battle.", "246", "130", "0", "100", "5", "0", "0", "0", "51", "0", "2"})
        dPKMMoves.Add(388, New String() {"Worry Seed", "A seed that causes worry is planted on the foe. It prevents sleep by making its ability Insomnia.", "247", "0", "12", "100", "10", "0", "0", "0", "22", "2", "1"})
        dPKMMoves.Add(389, New String() {"Sucker Punch", "This move enables the user to attack first. It fails if the foe is not readying an attack, however.", "248", "80", "17", "100", "5", "0", "0", "1", "115", "0", "3"})
        dPKMMoves.Add(390, New String() {"Toxic Spikes", "The user lays a trap of poison spikes at the foe's feet. They poison foes that switch into battle.", "249", "0", "3", "0", "20", "0", "128", "0", "64", "2", "3"})
        dPKMMoves.Add(391, New String() {"Heart Swap", "The user employs its psychic power to switch stat changes with the foe.", "250", "0", "14", "0", "10", "0", "0", "0", "18", "2", "0"})
        dPKMMoves.Add(392, New String() {"Aqua Ring", "The user envelops itself in a veil made of water. It regains some HP on every turn.", "251", "0", "11", "0", "20", "0", "16", "0", "0", "2", "1"})
        dPKMMoves.Add(393, New String() {"Magnet Rise", "The user levitates using electrically generated magnetism for five turns.", "252", "0", "13", "0", "10", "0", "16", "0", "0", "2", "2"})
        dPKMMoves.Add(394, New String() {"Flare Blitz", "The user cloaks itself in fire and charges at the foe. The user sustains serious damage, too.", "253", "120", "10", "100", "15", "10", "0", "0", "147", "0", "3"})
        dPKMMoves.Add(395, New String() {"Force Palm", "The foe is attacked with a shock wave. It may also leave the target paralyzed.", "6", "60", "1", "100", "10", "30", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(396, New String() {"Aura Sphere", "The user looses a blast of aura power from deep within its body. This move is certain to hit.", "17", "90", "1", "0", "20", "0", "0", "0", "178", "1", "1"})
        dPKMMoves.Add(397, New String() {"Rock Polish", "The user polishes its body to reduce drag. It can sharply raise the Speed stat.", "52", "0", "5", "0", "20", "0", "16", "0", "8", "2", "4"})
        dPKMMoves.Add(398, New String() {"Poison Jab", "The foe is stabbed with a tentacle or arm steeped in poison. It may also poison the foe.", "2", "80", "3", "100", "20", "30", "0", "0", "51", "0", "3"})
        dPKMMoves.Add(399, New String() {"Dark Pulse", "The user releases a horrible aura imbued with dark thoughts. It may also make the target flinch.", "31", "80", "17", "100", "15", "20", "0", "0", "50", "1", "0"})
        dPKMMoves.Add(400, New String() {"Night Slash", "The user slashes the foe the instant an opportunity arises. It has a high critical-hit ratio.", "43", "70", "17", "100", "15", "0", "0", "0", "51", "0", "1"})
        dPKMMoves.Add(401, New String() {"Aqua Tail", "The user attacks by swinging its tail as if it were a vicious wave in a raging storm.", "0", "90", "11", "90", "10", "0", "0", "0", "51", "0", "2"})
        dPKMMoves.Add(402, New String() {"Seed Bomb", "The user slams a barrage of hard- shelled seeds down on the foe from above.", "0", "80", "12", "100", "15", "0", "0", "0", "50", "0", "3"})
        dPKMMoves.Add(403, New String() {"Air Slash", "The user attacks with a blade of air that slices even the sky. It may also make the target flinch.", "31", "75", "2", "95", "20", "30", "0", "0", "50", "1", "0"})
        dPKMMoves.Add(404, New String() {"X-Scissor", "The user slashes at the foe by crossing its scythes or claws as if they were a pair of scissors.", "0", "80", "6", "100", "15", "0", "0", "0", "51", "0", "1"})
        dPKMMoves.Add(405, New String() {"Bug Buzz", "The user vibrates its wings to generate a damaging sound wave. It may also lower the foe's Sp. Def stat.", "72", "90", "6", "100", "10", "10", "0", "0", "18", "1", "2"})
        dPKMMoves.Add(406, New String() {"Dragon Pulse", "The foe is attacked with a shock wave generated by the user's gaping mouth.", "0", "90", "16", "100", "10", "0", "0", "0", "50", "1", "3"})
        dPKMMoves.Add(407, New String() {"Dragon Rush", "The user tackles the foe while exhibiting overwhelming menace. It may also make the target flinch.", "31", "100", "16", "75", "10", "20", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(408, New String() {"Power Gem", "The user attacks with a ray of light that sparkles as if it were made of gemstones.", "0", "70", "5", "100", "20", "0", "0", "0", "50", "1", "1"})
        dPKMMoves.Add(409, New String() {"Drain Punch", "An energy-draining punch. The user's HP is restored by half the damage taken by the target.", "3", "60", "1", "100", "5", "0", "0", "0", "51", "0", "1"})
        dPKMMoves.Add(410, New String() {"Vacuum Wave", "The user whirls its fists to send a wave of pure vacuum at the foe. This move always goes first.", "103", "40", "1", "100", "30", "0", "0", "1", "50", "1", "3"})
        dPKMMoves.Add(411, New String() {"Focus Blast", "The user heightens its mental focus and unleashes its power. It may also lower the target's Sp. Def.", "72", "120", "1", "70", "5", "10", "0", "0", "146", "1", "0"})
        dPKMMoves.Add(412, New String() {"Energy Ball", "The user draws power from nature and fires it at the foe. It may also lower the target's Sp. Def.", "72", "80", "12", "100", "10", "10", "0", "0", "18", "1", "1"})
        dPKMMoves.Add(413, New String() {"Brave Bird", "The user tucks in its wings and charges from a low altitude. The user also takes serious damage.", "198", "120", "2", "100", "15", "0", "0", "0", "179", "0", "2"})
        dPKMMoves.Add(414, New String() {"Earth Power", "The user makes the ground under the foe erupt with power. It may also lower the target's Sp. Def.", "72", "90", "4", "100", "10", "10", "0", "0", "50", "1", "3"})
        dPKMMoves.Add(415, New String() {"Switcheroo", "The user trades held items with the foe faster than the eye can follow.", "177", "0", "17", "100", "10", "0", "0", "0", "18", "2", "0"})
        dPKMMoves.Add(416, New String() {"Giga Impact", "The user charges at the foe using every bit of its power. The user must rest on the next turn.", "80", "150", "0", "90", "5", "0", "0", "0", "179", "0", "1"})
        dPKMMoves.Add(417, New String() {"Nasty Plot", "The user stimulates its brain by thinking bad thoughts. It sharply raises the user's Sp. Atk.", "53", "0", "17", "0", "20", "0", "16", "0", "72", "2", "2"})
        dPKMMoves.Add(418, New String() {"Bullet Punch", "The user strikes with a tough punch as fast as a bullet. This move always goes first.", "103", "40", "8", "100", "30", "0", "0", "1", "115", "0", "3"})
        dPKMMoves.Add(419, New String() {"Avalanche", "An attack move that inflicts double the damage if the user has been hurt by the foe in the same turn.", "185", "60", "15", "100", "10", "0", "0", "252", "51", "0", "0"})
        dPKMMoves.Add(420, New String() {"Ice Shard", "The user flash freezes chunks of ice and hurls them. This move always goes first.", "103", "40", "15", "100", "30", "0", "0", "1", "50", "0", "1"})
        dPKMMoves.Add(421, New String() {"Shadow Claw", "The user slashes with a sharp claw made from shadows. It has a high critical-hit ratio.", "43", "70", "7", "100", "15", "0", "0", "0", "115", "0", "2"})
        dPKMMoves.Add(422, New String() {"Thunder Fang", "The user bites with electrified fangs. It may also make the foe flinch or become paralyzed.", "275", "65", "13", "95", "15", "10", "0", "0", "51", "0", "3"})
        dPKMMoves.Add(423, New String() {"Ice Fang", "The user bites with cold-infused fangs. It may also make the foe flinch or freeze.", "274", "65", "15", "95", "15", "10", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(424, New String() {"Fire Fang", "The user bites with flame-cloaked fangs. It may also make the foe flinch or sustain a burn.", "273", "65", "10", "95", "15", "10", "0", "0", "51", "0", "1"})
        dPKMMoves.Add(425, New String() {"Shadow Sneak", "The user extends its shadow and attacks the foe from behind. This move always goes first.", "103", "40", "7", "100", "30", "0", "0", "1", "51", "0", "3"})
        dPKMMoves.Add(426, New String() {"Mud Bomb", "The user launches a hard-packed mud ball to attack. It may also lower the target's accuracy.", "73", "65", "4", "85", "10", "30", "0", "0", "50", "1", "3"})
        dPKMMoves.Add(427, New String() {"Psycho Cut", "The user tears at the foe with blades formed by psychic power. It has a high critical-hit ratio.", "43", "70", "14", "100", "20", "0", "0", "0", "50", "0", "0"})
        dPKMMoves.Add(428, New String() {"Zen Headbutt", "The user focuses its willpower to its head and rams the foe. It may also make the target flinch.", "31", "80", "14", "90", "15", "20", "0", "0", "51", "0", "1"})
        dPKMMoves.Add(429, New String() {"Mirror Shot", "The user looses a flash of energy from its polished body. It may also lower the target's accuracy.", "73", "65", "8", "85", "10", "30", "0", "0", "50", "1", "2"})
        dPKMMoves.Add(430, New String() {"Flash Cannon", "The user gathers all its light energy and releases it at once. It may also lower the foe's Sp. Def stat.", "72", "80", "8", "100", "10", "10", "0", "0", "50", "1", "3"})
        dPKMMoves.Add(431, New String() {"Rock Climb", "A charging attack that may also leave the foe confused. It can also be used to scale rocky walls.", "76", "90", "0", "85", "20", "20", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(432, New String() {"Defog", "Obstacles are moved, reducing the foe's evasion stat. It can also be used to clear deep fog, etc.", "258", "0", "2", "0", "15", "0", "0", "0", "18", "2", "1"})
        dPKMMoves.Add(433, New String() {"Trick Room", "The user creates a bizarre area in which slower Pokémon get to move first for five turns.", "259", "0", "14", "0", "5", "0", "64", "249", "16", "2", "2"})
        dPKMMoves.Add(434, New String() {"Draco Meteor", "Comets are summoned down from the sky. The attack's recoil sharply reduces the user's Sp. Atk stat.", "204", "140", "16", "90", "5", "100", "0", "0", "50", "1", "3"})
        dPKMMoves.Add(435, New String() {"Discharge", "A flare of electricity is loosed to strike all Pokémon in battle. It may also cause paralysis.", "6", "80", "13", "100", "15", "30", "8", "0", "50", "1", "0"})
        dPKMMoves.Add(436, New String() {"Lava Plume", "An inferno of scarlet flames washes over all Pokémon in battle. It may also inflict burns.", "4", "80", "10", "100", "15", "30", "8", "0", "50", "1", "4"})
        dPKMMoves.Add(437, New String() {"Leaf Storm", "A storm of sharp leaves is whipped up. The attack's recoil sharply reduces the user's Sp. Atk stat.", "204", "140", "12", "90", "5", "100", "0", "0", "178", "1", "2"})
        dPKMMoves.Add(438, New String() {"Power Whip", "The user violently whirls its vines or tentacles to harshly lash the foe.", "0", "120", "12", "85", "10", "0", "0", "0", "179", "0", "1"})
        dPKMMoves.Add(439, New String() {"Rock Wrecker", "The user launches a huge boulder at the foe to attack. It must rest on the next turn, however.", "80", "150", "5", "90", "5", "0", "0", "0", "178", "0", "4"})
        dPKMMoves.Add(440, New String() {"Cross Poison", "A slashing attack that may also leave the target poisoned. It has a high critical-hit ratio.", "209", "70", "3", "100", "20", "10", "0", "0", "51", "0", "0"})
        dPKMMoves.Add(441, New String() {"Gunk Shot", "The user shoots filthy garbage at the foe to attack. It may also poison the target.", "2", "120", "3", "70", "5", "30", "0", "0", "178", "0", "0"})
        dPKMMoves.Add(442, New String() {"Iron Head", "The foe slams the target with its steel-hard head. It may also make the target flinch.", "31", "80", "8", "100", "15", "30", "0", "0", "51", "0", "4"})
        dPKMMoves.Add(443, New String() {"Magnet Bomb", "The user launches a steel bomb that sticks to the target. This attack will not miss.", "17", "60", "8", "0", "20", "0", "0", "0", "50", "0", "0"})
        dPKMMoves.Add(444, New String() {"Stone Edge", "The user stabs the foe with a sharpened stone. It has a high critical-hit ratio.", "43", "100", "5", "80", "5", "0", "0", "0", "50", "0", "4"})
        dPKMMoves.Add(445, New String() {"Captivate", "If it is the opposite gender of the user, the foe is charmed into sharply lowering its Sp. Atk stat.", "265", "0", "0", "100", "20", "0", "4", "0", "86", "2", "1"})
        dPKMMoves.Add(446, New String() {"Stealth Rock", "The user lays a trap of levitating stones around the foe. The trap hurts foes that switch into battle.", "266", "0", "5", "0", "20", "0", "128", "0", "0", "2", "0"})
        dPKMMoves.Add(447, New String() {"Grass Knot", "The user snares the foe with grass and trips it. The heavier the foe, the greater the damage.", "196", "1", "12", "100", "20", "0", "0", "0", "115", "1", "3"})
        dPKMMoves.Add(448, New String() {"Chatter", "The user attacks using a sound wave based on words it has learned. It may also confuse the foe.", "267", "60", "2", "100", "20", "0", "0", "0", "66", "1", "3"})
        dPKMMoves.Add(449, New String() {"Judgment", "The user releases countless shots of light. Its type varies with the kind of Plate the user is holding.", "268", "100", "0", "100", "10", "0", "0", "0", "50", "1", "3"})
        dPKMMoves.Add(450, New String() {"Bug Bite", "The user bites the foe. If the foe is holding a Berry, the user eats it and gains its effect.", "224", "60", "6", "100", "20", "0", "0", "0", "115", "0", "4"})
        dPKMMoves.Add(451, New String() {"Charge Beam", "The user fires a concentrated bundle of electricity. It may also raise the user's Sp. Atk stat.", "276", "50", "13", "90", "10", "70", "0", "0", "50", "1", "1"})
        dPKMMoves.Add(452, New String() {"Wood Hammer", "The user slams its rugged body into the foe to attack. The user also sustains serious damage.", "198", "120", "12", "100", "15", "0", "0", "0", "51", "0", "4"})
        dPKMMoves.Add(453, New String() {"Aqua Jet", "The user lunges at the foe at a speed that makes it almost invisible. It is sure to strike first.", "103", "40", "11", "100", "20", "0", "0", "1", "51", "0", "1"})
        dPKMMoves.Add(454, New String() {"Attack Order", "The user calls out its underlings to pummel the foe. It has a high critical-hit ratio.", "43", "90", "6", "100", "15", "0", "0", "0", "50", "0", "3"})
        dPKMMoves.Add(455, New String() {"Defend Order", "The user calls out its underlings to make a living shield, raising its Defense and Sp. Def stats.", "206", "0", "6", "0", "10", "0", "16", "0", "8", "2", "3"})
        dPKMMoves.Add(456, New String() {"Heal Order", "The user calls out its underlings to heal it. The user regains up to half of its max HP.", "32", "0", "6", "0", "10", "0", "16", "0", "8", "2", "3"})
        dPKMMoves.Add(457, New String() {"Head Smash", "The user delivers a life-endangering head butt at full power. The user also takes terrible damage.", "269", "150", "5", "80", "5", "0", "0", "0", "179", "0", "4"})
        dPKMMoves.Add(458, New String() {"Double Hit", "The user slams the foe with a tail, etc. The target is hit twice in a row.", "44", "35", "0", "90", "10", "0", "0", "0", "51", "0", "3"})
        dPKMMoves.Add(459, New String() {"Roar of Time", "The user blasts the foe with power that distorts even time. The user must rest on the next turn.", "80", "150", "16", "90", "5", "0", "0", "0", "50", "1", "0"})
        dPKMMoves.Add(460, New String() {"Spacial Rend", "The user tears the foe along with the space around it. This move has a high critical-hit ratio.", "43", "100", "16", "95", "5", "0", "0", "0", "178", "1", "4"})
        dPKMMoves.Add(461, New String() {"Lunar Dance", "The user faints. In return, the Pokémon taking its place will have its status and HP fully restored.", "270", "0", "14", "0", "10", "0", "16", "0", "128", "2", "1"})
        dPKMMoves.Add(462, New String() {"Crush Grip", "The foe is crushed with great force. The attack is more powerful the more HP the foe has left.", "237", "1", "0", "100", "5", "0", "0", "0", "51", "0", "4"})
        dPKMMoves.Add(463, New String() {"Magma Storm", "The foe becomes trapped within a maelstrom of fire that rages for two to five turns.", "42", "120", "10", "70", "5", "0", "0", "0", "178", "1", "4"})
        dPKMMoves.Add(464, New String() {"Dark Void", "The foe is dragged into a world of total darkness that puts it to sleep.", "1", "0", "17", "80", "10", "0", "4", "0", "150", "2", "3"})
        dPKMMoves.Add(465, New String() {"Seed Flare", "The user generates a shock wave from within its body. It may also lower the target's Sp. Def.", "271", "120", "12", "85", "5", "40", "0", "0", "178", "1", "0"})
        dPKMMoves.Add(466, New String() {"Ominous Wind", "The user creates a gust of repulsive wind. It may also raise all the user's stats at once.", "140", "60", "7", "100", "5", "10", "0", "0", "50", "1", "3"})
        dPKMMoves.Add(467, New String() {"Shadow Force", "The user disappears, then strikes the foe on the second turn. It hits even if the foe used Protect.", "272", "120", "7", "100", "5", "0", "0", "0", "49", "0", "3"})

        LevelTable = New Integer(99, 5) {{0, 0, 0, 0, 0, 0}, _
{15, 6, 8, 9, 10, 4}, _
{52, 21, 27, 57, 33, 13}, _
{122, 51, 64, 96, 80, 32}, _
{237, 100, 125, 135, 156, 65}, _
{406, 172, 216, 179, 270, 112}, _
{637, 274, 343, 236, 428, 178}, _
{942, 409, 512, 314, 640, 276}, _
{1326, 583, 729, 419, 911, 393}, _
{1800, 800, 1000, 560, 1250, 540}, _
{2369, 1064, 1331, 742, 1663, 745}, _
{3041, 1382, 1728, 973, 2160, 967}, _
{3822, 1757, 2197, 1261, 2746, 1230}, _
{4719, 2195, 2744, 1612, 3430, 1591}, _
{5737, 2700, 3375, 2035, 4218, 1957}, _
{6881, 3276, 4096, 2535, 5120, 2457}, _
{8155, 3930, 4913, 3120, 6141, 3046}, _
{9564, 4665, 5832, 3798, 7290, 3732}, _
{11111, 5487, 6859, 4575, 8573, 4526}, _
{12800, 6400, 8000, 5460, 10000, 5440}, _
{14632, 7408, 9261, 6458, 11576, 6482}, _
{16610, 8518, 10648, 7577, 13310, 7666}, _
{18737, 9733, 12167, 8825, 15208, 9003}, _
{21012, 11059, 13824, 10208, 17280, 10506}, _
{23437, 12500, 15625, 11735, 19531, 12187}, _
{26012, 14060, 17576, 13411, 21970, 14060}, _
{28737, 15746, 19683, 15244, 24603, 16140}, _
{31610, 17561, 21952, 17242, 27440, 18439}, _
{34632, 19511, 24389, 19411, 30486, 20974}, _
{37800, 21600, 27000, 21760, 33750, 23760}, _
{41111, 23832, 29791, 24294, 37238, 26811}, _
{44564, 26214, 32768, 27021, 40960, 30146}, _
{48155, 28749, 35937, 29949, 44921, 33780}, _
{51881, 31443, 39304, 33084, 49130, 37731}, _
{55737, 34300, 42875, 36435, 53593, 42017}, _
{59719, 37324, 46656, 40007, 58320, 46656}, _
{63822, 40522, 50653, 43808, 63316, 50653}, _
{68041, 43897, 54872, 47846, 68590, 55969}, _
{72369, 47455, 59319, 52127, 74148, 60505}, _
{76800, 51200, 64000, 56660, 80000, 66560}, _
{81326, 55136, 68921, 61450, 86151, 71677}, _
{85942, 59270, 74088, 66505, 92610, 78533}, _
{90637, 63605, 79507, 71833, 99383, 84277}, _
{95406, 68147, 85184, 77440, 106480, 91998}, _
{100237, 72900, 91125, 83335, 113906, 98415}, _
{105122, 77868, 97336, 89523, 121670, 107069}, _
{110052, 83058, 103823, 96012, 129778, 114205}, _
{115015, 88473, 110592, 102810, 138240, 123863}, _
{120001, 94119, 117649, 109923, 147061, 131766}, _
{125000, 100000, 125000, 117360, 156250, 142500}, _
{131324, 106120, 132651, 125126, 165813, 151222}, _
{137795, 112486, 140608, 133229, 175760, 163105}, _
{144410, 119101, 148877, 141677, 186096, 172697}, _
{151165, 125971, 157464, 150476, 196830, 185807}, _
{158056, 133100, 166375, 159635, 207968, 196322}, _
{165079, 140492, 175616, 169159, 219520, 210739}, _
{172229, 148154, 185193, 179056, 231491, 222231}, _
{179503, 156089, 195112, 189334, 243890, 238036}, _
{186894, 164303, 205379, 199999, 256723, 250562}, _
{194400, 172800, 216000, 211060, 270000, 267840}, _
{202013, 181584, 226981, 222522, 283726, 281456}, _
{209728, 190662, 238328, 234393, 297910, 300293}, _
{217540, 200037, 250047, 246681, 312558, 315059}, _
{225443, 209715, 262144, 259392, 327680, 335544}, _
{233431, 219700, 274625, 272535, 343281, 351520}, _
{241496, 229996, 287496, 286115, 359370, 373744}, _
{249633, 240610, 300763, 300140, 375953, 390991}, _
{257834, 251545, 314432, 314618, 393040, 415050}, _
{267406, 262807, 328509, 329555, 410636, 433631}, _
{276458, 274400, 343000, 344960, 428750, 459620}, _
{286328, 286328, 357911, 360838, 447388, 479600}, _
{296358, 298598, 373248, 377197, 466560, 507617}, _
{305767, 311213, 389017, 394045, 486271, 529063}, _
{316074, 324179, 405224, 411388, 506530, 559209}, _
{326531, 337500, 421875, 429235, 527343, 582187}, _
{336255, 351180, 438976, 447591, 548720, 614566}, _
{346965, 365226, 456533, 466464, 570666, 639146}, _
{357812, 379641, 474552, 485862, 593190, 673863}, _
{367807, 394431, 493039, 505791, 616298, 700115}, _
{378880, 409600, 512000, 526260, 640000, 737280}, _
{390077, 425152, 531441, 547274, 664301, 765275}, _
{400293, 441094, 551368, 568841, 689210, 804997}, _
{411686, 457429, 571787, 590969, 714733, 834809}, _
{423190, 474163, 592704, 613664, 740880, 877201}, _
{433572, 491300, 614125, 636935, 767656, 908905}, _
{445239, 508844, 636056, 660787, 795070, 954084}, _
{457001, 526802, 658503, 685228, 823128, 987754}, _
{467489, 545177, 681472, 710266, 851840, 1035837}, _
{479378, 563975, 704969, 735907, 881211, 1071552}, _
{491346, 583200, 729000, 762160, 911250, 1122660}, _
{501878, 602856, 753571, 789030, 941963, 1160499}, _
{513934, 622950, 778688, 816525, 973360, 1214753}, _
{526049, 643485, 804357, 844653, 1005446, 1254796}, _
{536557, 664467, 830584, 873420, 1038230, 1312322}, _
{548720, 685900, 857375, 902835, 1071718, 1354652}, _
{560922, 707788, 884736, 932903, 1105920, 1415577}, _
{571333, 730138, 912673, 963632, 1140841, 1460276}, _
{583539, 752953, 941192, 995030, 1176490, 1524731}, _
{591882, 776239, 970299, 1027103, 1212873, 1571884}, _
{600000, 800000, 1000000, 1059860, 1250000, 1640000}}

        dPKMItems.Add(&H0, "NOTHING")
        dPKMItems.Add(&H1, "Master Ball")
        dPKMItems.Add(&H2, "Ultra Ball")
        dPKMItems.Add(&H3, "Great Ball")
        dPKMItems.Add(&H4, "Pok" & Char.ConvertFromUtf32(CNVRT.PKMToUnicode(&H188)) & " Ball")
        dPKMItems.Add(&H5, "Safari Ball")
        dPKMItems.Add(&H6, "Net Ball")
        dPKMItems.Add(&H7, "Dive Ball")
        dPKMItems.Add(&H8, "Nest Ball")
        dPKMItems.Add(&H9, "Repeat Ball")
        dPKMItems.Add(&HA, "Timer Ball")
        dPKMItems.Add(&HB, "Luxury Ball")
        dPKMItems.Add(&HC, "Premier Ball")
        dPKMItems.Add(&HD, "Dusk Ball")
        dPKMItems.Add(&HE, "Heal Ball")
        dPKMItems.Add(&HF, "Quick Ball")
        dPKMItems.Add(&H10, "Cherish Ball")
        dPKMItems.Add(&H11, "Potion")
        dPKMItems.Add(&H12, "Antidote")
        dPKMItems.Add(&H13, "Burn Heal")
        dPKMItems.Add(&H14, "Ice Heal")
        dPKMItems.Add(&H15, "Awakening")
        dPKMItems.Add(&H16, "Parlyz Heal")
        dPKMItems.Add(&H17, "Full Restore")
        dPKMItems.Add(&H18, "Max Potion")
        dPKMItems.Add(&H19, "Hyper Potion")
        dPKMItems.Add(&H1A, "Super Potion")
        dPKMItems.Add(&H1B, "Full Heal")
        dPKMItems.Add(&H1C, "Revive")
        dPKMItems.Add(&H1D, "Max Revive")
        dPKMItems.Add(&H1E, "Fresh Water")
        dPKMItems.Add(&H1F, "Soda Pop")
        dPKMItems.Add(&H20, "Lemonade")
        dPKMItems.Add(&H21, "Moomoo Milk")
        dPKMItems.Add(&H22, "EnergyPowder")
        dPKMItems.Add(&H23, "Energy Root")
        dPKMItems.Add(&H24, "Heal Powder")
        dPKMItems.Add(&H25, "Revival Herb")
        dPKMItems.Add(&H26, "Ether")
        dPKMItems.Add(&H27, "Max Ether")
        dPKMItems.Add(&H28, "Elixir")
        dPKMItems.Add(&H29, "Max Elixir")
        dPKMItems.Add(&H2A, "Lava Cookie")
        dPKMItems.Add(&H2B, "Berry Juice")
        dPKMItems.Add(&H2C, "Sacred Ash")
        dPKMItems.Add(&H2D, "HP Up")
        dPKMItems.Add(&H2E, "Protein")
        dPKMItems.Add(&H2F, "Iron")
        dPKMItems.Add(&H30, "Carbos")
        dPKMItems.Add(&H31, "Calcium")
        dPKMItems.Add(&H32, "Rare Candy")
        dPKMItems.Add(&H33, "PP Up")
        dPKMItems.Add(&H34, "Zinc")
        dPKMItems.Add(&H35, "PP Max")
        dPKMItems.Add(&H36, "Old Gateau")
        dPKMItems.Add(&H37, "Guard Spec.")
        dPKMItems.Add(&H38, "Dire Hit")
        dPKMItems.Add(&H39, "X Attack")
        dPKMItems.Add(&H3A, "X Defend")
        dPKMItems.Add(&H3B, "X Speed")
        dPKMItems.Add(&H3C, "X Accuracy")
        dPKMItems.Add(&H3D, "X Special")
        dPKMItems.Add(&H3E, "X Sp. Def")
        dPKMItems.Add(&H3F, "Pok" & Char.ConvertFromUtf32(CNVRT.PKMToUnicode(&H188)) & " Doll")
        dPKMItems.Add(&H40, "Fluffy Tail")
        dPKMItems.Add(&H41, "Blue Flute")
        dPKMItems.Add(&H42, "Yellow Flute")
        dPKMItems.Add(&H43, "Red Flute")
        dPKMItems.Add(&H44, "Black Flute")
        dPKMItems.Add(&H45, "White Flute")
        dPKMItems.Add(&H46, "Shoal Salt")
        dPKMItems.Add(&H47, "Shoal Shell")
        dPKMItems.Add(&H48, "Red Shard")
        dPKMItems.Add(&H49, "Blue Shard")
        dPKMItems.Add(&H4A, "Yellow Shard")
        dPKMItems.Add(&H4B, "Green Shard")
        dPKMItems.Add(&H4C, "Super Repel")
        dPKMItems.Add(&H4D, "Max Repel")
        dPKMItems.Add(&H4E, "Escape Rope")
        dPKMItems.Add(&H4F, "Repel")
        dPKMItems.Add(&H50, "Sun Stone")
        dPKMItems.Add(&H51, "Moon Stone")
        dPKMItems.Add(&H52, "Fire Stone")
        dPKMItems.Add(&H53, "Thunderstone")
        dPKMItems.Add(&H54, "Water Stone")
        dPKMItems.Add(&H55, "Leaf Stone")
        dPKMItems.Add(&H56, "TinyMushroom")
        dPKMItems.Add(&H57, "Big Mushroom")
        dPKMItems.Add(&H58, "Pearl")
        dPKMItems.Add(&H59, "Big Pearl")
        dPKMItems.Add(&H5A, "Stardust")
        dPKMItems.Add(&H5B, "Star Piece")
        dPKMItems.Add(&H5C, "Nugget")
        dPKMItems.Add(&H5D, "Heart Scale")
        dPKMItems.Add(&H5E, "Honey")
        dPKMItems.Add(&H5F, "Growth Mulch")
        dPKMItems.Add(&H60, "Damp Mulch")
        dPKMItems.Add(&H61, "Stable Mulch")
        dPKMItems.Add(&H62, "Gooey Mulch")
        dPKMItems.Add(&H63, "Root Fossil")
        dPKMItems.Add(&H64, "Claw Fossil")
        dPKMItems.Add(&H65, "Helix Fossil")
        dPKMItems.Add(&H66, "Dome Fossil")
        dPKMItems.Add(&H67, "Old Amber")
        dPKMItems.Add(&H68, "Armor Fossil")
        dPKMItems.Add(&H69, "Skull Fossil")
        dPKMItems.Add(&H6A, "Rare Bone")
        dPKMItems.Add(&H6B, "Shiny Stone")
        dPKMItems.Add(&H6C, "Dusk Stone")
        dPKMItems.Add(&H6D, "Dawn Stone")
        dPKMItems.Add(&H6E, "Oval Stone")
        dPKMItems.Add(&H6F, "Odd Keystone")
        dPKMItems.Add(&H70, "Griseous Orb")
        dPKMItems.Add(&H71, "???")
        dPKMItems.Add(&H72, "???")
        dPKMItems.Add(&H73, "???")
        dPKMItems.Add(&H74, "???")
        dPKMItems.Add(&H75, "???")
        dPKMItems.Add(&H76, "???")
        dPKMItems.Add(&H77, "???")
        dPKMItems.Add(&H78, "???")
        dPKMItems.Add(&H79, "???")
        dPKMItems.Add(&H7A, "???")
        dPKMItems.Add(&H7B, "???")
        dPKMItems.Add(&H7C, "???")
        dPKMItems.Add(&H7D, "???")
        dPKMItems.Add(&H7E, "???")
        dPKMItems.Add(&H7F, "???")
        dPKMItems.Add(&H80, "???")
        dPKMItems.Add(&H81, "???")
        dPKMItems.Add(&H82, "???")
        dPKMItems.Add(&H83, "???")
        dPKMItems.Add(&H84, "???")
        dPKMItems.Add(&H85, "???")
        dPKMItems.Add(&H86, "???")
        dPKMItems.Add(&H87, "Adamant Orb")
        dPKMItems.Add(&H88, "Lustrous Orb")
        dPKMItems.Add(&H89, "Grass Mail")
        dPKMItems.Add(&H8A, "Flame Mail")
        dPKMItems.Add(&H8B, "Bubble Mail")
        dPKMItems.Add(&H8C, "Bloom Mail")
        dPKMItems.Add(&H8D, "Tunnel Mail")
        dPKMItems.Add(&H8E, "Steel Mail")
        dPKMItems.Add(&H8F, "Heart Mail")
        dPKMItems.Add(&H90, "Snow Mail")
        dPKMItems.Add(&H91, "Space Mail")
        dPKMItems.Add(&H92, "Air Mail")
        dPKMItems.Add(&H93, "Mosaic Mail")
        dPKMItems.Add(&H94, "Brick Mail")
        dPKMItems.Add(&H95, "Cheri Berry")
        dPKMItems.Add(&H96, "Chesto Berry")
        dPKMItems.Add(&H97, "Pecha Berry")
        dPKMItems.Add(&H98, "Rawst Berry")
        dPKMItems.Add(&H99, "Aspear Berry")
        dPKMItems.Add(&H9A, "Leppa Berry")
        dPKMItems.Add(&H9B, "Oran Berry")
        dPKMItems.Add(&H9C, "Persim Berry")
        dPKMItems.Add(&H9D, "Lum Berry")
        dPKMItems.Add(&H9E, "Sitrus Berry")
        dPKMItems.Add(&H9F, "Figy Berry")
        dPKMItems.Add(&HA0, "Wiki Berry")
        dPKMItems.Add(&HA1, "Mago Berry")
        dPKMItems.Add(&HA2, "Aguav Berry")
        dPKMItems.Add(&HA3, "Iapapa Berry")
        dPKMItems.Add(&HA4, "Razz Berry")
        dPKMItems.Add(&HA5, "Bluk Berry")
        dPKMItems.Add(&HA6, "Nanab Berry")
        dPKMItems.Add(&HA7, "Wepear Berry")
        dPKMItems.Add(&HA8, "Pinap Berry")
        dPKMItems.Add(&HA9, "Pomeg Berry")
        dPKMItems.Add(&HAA, "Kelpsy Berry")
        dPKMItems.Add(&HAB, "Qualot Berry")
        dPKMItems.Add(&HAC, "Hondew Berry")
        dPKMItems.Add(&HAD, "Grepa Berry")
        dPKMItems.Add(&HAE, "Tamato Berry")
        dPKMItems.Add(&HAF, "Cornn Berry")
        dPKMItems.Add(&HB0, "Magost Berry")
        dPKMItems.Add(&HB1, "Rabuta Berry")
        dPKMItems.Add(&HB2, "Nomel Berry")
        dPKMItems.Add(&HB3, "Spelon Berry")
        dPKMItems.Add(&HB4, "Pamtre Berry")
        dPKMItems.Add(&HB5, "Watmel Berry")
        dPKMItems.Add(&HB6, "Durin Berry")
        dPKMItems.Add(&HB7, "Belue Berry")
        dPKMItems.Add(&HB8, "Occa Berry")
        dPKMItems.Add(&HB9, "Passho Berry")
        dPKMItems.Add(&HBA, "Wacan Berry")
        dPKMItems.Add(&HBB, "Rindo Berry")
        dPKMItems.Add(&HBC, "Yache Berry")
        dPKMItems.Add(&HBD, "Chople Berry")
        dPKMItems.Add(&HBE, "Kebia Berry")
        dPKMItems.Add(&HBF, "Shuca Berry")
        dPKMItems.Add(&HC0, "Coba Berry")
        dPKMItems.Add(&HC1, "Payapa Berry")
        dPKMItems.Add(&HC2, "Tanga Berry")
        dPKMItems.Add(&HC3, "Charti Berry")
        dPKMItems.Add(&HC4, "Kasib Berry")
        dPKMItems.Add(&HC5, "Haban Berry")
        dPKMItems.Add(&HC6, "Colbur Berry")
        dPKMItems.Add(&HC7, "Babiri Berry")
        dPKMItems.Add(&HC8, "Chilan Berry")
        dPKMItems.Add(&HC9, "Liechi Berry")
        dPKMItems.Add(&HCA, "Ganlon Berry")
        dPKMItems.Add(&HCB, "Salac Berry")
        dPKMItems.Add(&HCC, "Petaya Berry")
        dPKMItems.Add(&HCD, "Apicot Berry")
        dPKMItems.Add(&HCE, "Lansat Berry")
        dPKMItems.Add(&HCF, "Starf Berry")
        dPKMItems.Add(&HD0, "Enigma Berry")
        dPKMItems.Add(&HD1, "Micle Berry")
        dPKMItems.Add(&HD2, "Custap Berry")
        dPKMItems.Add(&HD3, "Jaboca Berry")
        dPKMItems.Add(&HD4, "Rowap Berry")
        dPKMItems.Add(&HD5, "BrightPowder")
        dPKMItems.Add(&HD6, "White Herb")
        dPKMItems.Add(&HD7, "Macho Brace")
        dPKMItems.Add(&HD8, "Exp. Share")
        dPKMItems.Add(&HD9, "Quick Claw")
        dPKMItems.Add(&HDA, "Soothe Bell")
        dPKMItems.Add(&HDB, "Mental Herb")
        dPKMItems.Add(&HDC, "Choice Band")
        dPKMItems.Add(&HDD, "King's Rock")
        dPKMItems.Add(&HDE, "SilverPowder")
        dPKMItems.Add(&HDF, "Amulet Coin")
        dPKMItems.Add(&HE0, "Cleanse Tag")
        dPKMItems.Add(&HE1, "Soul Dew")
        dPKMItems.Add(&HE2, "DeepSeaTooth")
        dPKMItems.Add(&HE3, "DeepSeaScale")
        dPKMItems.Add(&HE4, "Smoke Ball")
        dPKMItems.Add(&HE5, "Everstone")
        dPKMItems.Add(&HE6, "Focus Band")
        dPKMItems.Add(&HE7, "Lucky Egg")
        dPKMItems.Add(&HE8, "Scope Lens")
        dPKMItems.Add(&HE9, "Metal Coat")
        dPKMItems.Add(&HEA, "Leftovers")
        dPKMItems.Add(&HEB, "Dragon Scale")
        dPKMItems.Add(&HEC, "Light Ball")
        dPKMItems.Add(&HED, "Soft Sand")
        dPKMItems.Add(&HEE, "Hard Stone")
        dPKMItems.Add(&HEF, "Miracle Seed")
        dPKMItems.Add(&HF0, "BlackGlasses")
        dPKMItems.Add(&HF1, "Black Belt")
        dPKMItems.Add(&HF2, "Magnet")
        dPKMItems.Add(&HF3, "Mystic Water")
        dPKMItems.Add(&HF4, "Sharp Beak")
        dPKMItems.Add(&HF5, "Poison Barb")
        dPKMItems.Add(&HF6, "NeverMeltIce")
        dPKMItems.Add(&HF7, "Spell Tag")
        dPKMItems.Add(&HF8, "TwistedSpoon")
        dPKMItems.Add(&HF9, "Charcoal")
        dPKMItems.Add(&HFA, "Dragon Fang")
        dPKMItems.Add(&HFB, "Silk Scarf")
        dPKMItems.Add(&HFC, "Up-Grade")
        dPKMItems.Add(&HFD, "Shell Bell")
        dPKMItems.Add(&HFE, "Sea Incense")
        dPKMItems.Add(&HFF, "Lax Incense")
        dPKMItems.Add(&H100, "Lucky Punch")
        dPKMItems.Add(&H101, "Metal Powder")
        dPKMItems.Add(&H102, "Thick Club")
        dPKMItems.Add(&H103, "Stick")
        dPKMItems.Add(&H104, "Red Scarf")
        dPKMItems.Add(&H105, "Blue Scarf")
        dPKMItems.Add(&H106, "Pink Scarf")
        dPKMItems.Add(&H107, "Green Scarf")
        dPKMItems.Add(&H108, "Yellow Scarf")
        dPKMItems.Add(&H109, "Wide Lens")
        dPKMItems.Add(&H10A, "Muscle Band")
        dPKMItems.Add(&H10B, "Wise Glasses")
        dPKMItems.Add(&H10C, "Expert Belt")
        dPKMItems.Add(&H10D, "Light Clay")
        dPKMItems.Add(&H10E, "Life Orb")
        dPKMItems.Add(&H10F, "Power Herb")
        dPKMItems.Add(&H110, "Toxic Orb")
        dPKMItems.Add(&H111, "Flame Orb")
        dPKMItems.Add(&H112, "Quick Powder")
        dPKMItems.Add(&H113, "Focus Sash")
        dPKMItems.Add(&H114, "Zoom Lens")
        dPKMItems.Add(&H115, "Metronome")
        dPKMItems.Add(&H116, "Iron Ball")
        dPKMItems.Add(&H117, "Lagging Tail")
        dPKMItems.Add(&H118, "Destiny Knot")
        dPKMItems.Add(&H119, "Black Sludge")
        dPKMItems.Add(&H11A, "Icy Rock")
        dPKMItems.Add(&H11B, "Smooth Rock")
        dPKMItems.Add(&H11C, "Heat Rock")
        dPKMItems.Add(&H11D, "Damp Rock")
        dPKMItems.Add(&H11E, "Grip Claw")
        dPKMItems.Add(&H11F, "Choice Scarf")
        dPKMItems.Add(&H120, "Sticky Barb")
        dPKMItems.Add(&H121, "Power Bracer")
        dPKMItems.Add(&H122, "Power Belt")
        dPKMItems.Add(&H123, "Power Lens")
        dPKMItems.Add(&H124, "Power Band")
        dPKMItems.Add(&H125, "Power Anklet")
        dPKMItems.Add(&H126, "Power Weight")
        dPKMItems.Add(&H127, "Shed Shell")
        dPKMItems.Add(&H128, "Big Root")
        dPKMItems.Add(&H129, "Choice Specs")
        dPKMItems.Add(&H12A, "Flame Plate")
        dPKMItems.Add(&H12B, "Splash Plate")
        dPKMItems.Add(&H12C, "Zap Plate")
        dPKMItems.Add(&H12D, "Meadow Plate")
        dPKMItems.Add(&H12E, "Icicle Plate")
        dPKMItems.Add(&H12F, "Fist Plate")
        dPKMItems.Add(&H130, "Toxic Plate")
        dPKMItems.Add(&H131, "Earth Plate")
        dPKMItems.Add(&H132, "Sky Plate")
        dPKMItems.Add(&H133, "Mind Plate")
        dPKMItems.Add(&H134, "Insect Plate")
        dPKMItems.Add(&H135, "Stone Plate")
        dPKMItems.Add(&H136, "Spooky Plate")
        dPKMItems.Add(&H137, "Draco Plate")
        dPKMItems.Add(&H138, "Dread Plate")
        dPKMItems.Add(&H139, "Iron Plate")
        dPKMItems.Add(&H13A, "Odd Incense")
        dPKMItems.Add(&H13B, "Rock Incense")
        dPKMItems.Add(&H13C, "Full Incense")
        dPKMItems.Add(&H13D, "Wave Incense")
        dPKMItems.Add(&H13E, "Rose Incense")
        dPKMItems.Add(&H13F, "Luck Incense")
        dPKMItems.Add(&H140, "Pure Incense")
        dPKMItems.Add(&H141, "Protector")
        dPKMItems.Add(&H142, "Electirizer")
        dPKMItems.Add(&H143, "Magmarizer")
        dPKMItems.Add(&H144, "Dubious Disc")
        dPKMItems.Add(&H145, "Reaper Cloth")
        dPKMItems.Add(&H146, "Razor Claw")
        dPKMItems.Add(&H147, "Razor Fang")
        dPKMItems.Add(&H148, "TM01")
        dPKMItems.Add(&H149, "TM02")
        dPKMItems.Add(&H14A, "TM03")
        dPKMItems.Add(&H14B, "TM04")
        dPKMItems.Add(&H14C, "TM05")
        dPKMItems.Add(&H14D, "TM06")
        dPKMItems.Add(&H14E, "TM07")
        dPKMItems.Add(&H14F, "TM08")
        dPKMItems.Add(&H150, "TM09")
        dPKMItems.Add(&H151, "TM10")
        dPKMItems.Add(&H152, "TM11")
        dPKMItems.Add(&H153, "TM12")
        dPKMItems.Add(&H154, "TM13")
        dPKMItems.Add(&H155, "TM14")
        dPKMItems.Add(&H156, "TM15")
        dPKMItems.Add(&H157, "TM16")
        dPKMItems.Add(&H158, "TM17")
        dPKMItems.Add(&H159, "TM18")
        dPKMItems.Add(&H15A, "TM19")
        dPKMItems.Add(&H15B, "TM20")
        dPKMItems.Add(&H15C, "TM21")
        dPKMItems.Add(&H15D, "TM22")
        dPKMItems.Add(&H15E, "TM23")
        dPKMItems.Add(&H15F, "TM24")
        dPKMItems.Add(&H160, "TM25")
        dPKMItems.Add(&H161, "TM26")
        dPKMItems.Add(&H162, "TM27")
        dPKMItems.Add(&H163, "TM28")
        dPKMItems.Add(&H164, "TM29")
        dPKMItems.Add(&H165, "TM30")
        dPKMItems.Add(&H166, "TM31")
        dPKMItems.Add(&H167, "TM32")
        dPKMItems.Add(&H168, "TM33")
        dPKMItems.Add(&H169, "TM34")
        dPKMItems.Add(&H16A, "TM35")
        dPKMItems.Add(&H16B, "TM36")
        dPKMItems.Add(&H16C, "TM37")
        dPKMItems.Add(&H16D, "TM38")
        dPKMItems.Add(&H16E, "TM39")
        dPKMItems.Add(&H16F, "TM40")
        dPKMItems.Add(&H170, "TM41")
        dPKMItems.Add(&H171, "TM42")
        dPKMItems.Add(&H172, "TM43")
        dPKMItems.Add(&H173, "TM44")
        dPKMItems.Add(&H174, "TM45")
        dPKMItems.Add(&H175, "TM46")
        dPKMItems.Add(&H176, "TM47")
        dPKMItems.Add(&H177, "TM48")
        dPKMItems.Add(&H178, "TM49")
        dPKMItems.Add(&H179, "TM50")
        dPKMItems.Add(&H17A, "TM51")
        dPKMItems.Add(&H17B, "TM52")
        dPKMItems.Add(&H17C, "TM53")
        dPKMItems.Add(&H17D, "TM54")
        dPKMItems.Add(&H17E, "TM55")
        dPKMItems.Add(&H17F, "TM56")
        dPKMItems.Add(&H180, "TM57")
        dPKMItems.Add(&H181, "TM58")
        dPKMItems.Add(&H182, "TM59")
        dPKMItems.Add(&H183, "TM60")
        dPKMItems.Add(&H184, "TM61")
        dPKMItems.Add(&H185, "TM62")
        dPKMItems.Add(&H186, "TM63")
        dPKMItems.Add(&H187, "TM64")
        dPKMItems.Add(&H188, "TM65")
        dPKMItems.Add(&H189, "TM66")
        dPKMItems.Add(&H18A, "TM67")
        dPKMItems.Add(&H18B, "TM68")
        dPKMItems.Add(&H18C, "TM69")
        dPKMItems.Add(&H18D, "TM70")
        dPKMItems.Add(&H18E, "TM71")
        dPKMItems.Add(&H18F, "TM72")
        dPKMItems.Add(&H190, "TM73")
        dPKMItems.Add(&H191, "TM74")
        dPKMItems.Add(&H192, "TM75")
        dPKMItems.Add(&H193, "TM76")
        dPKMItems.Add(&H194, "TM77")
        dPKMItems.Add(&H195, "TM78")
        dPKMItems.Add(&H196, "TM79")
        dPKMItems.Add(&H197, "TM80")
        dPKMItems.Add(&H198, "TM81")
        dPKMItems.Add(&H199, "TM82")
        dPKMItems.Add(&H19A, "TM83")
        dPKMItems.Add(&H19B, "TM84")
        dPKMItems.Add(&H19C, "TM85")
        dPKMItems.Add(&H19D, "TM86")
        dPKMItems.Add(&H19E, "TM87")
        dPKMItems.Add(&H19F, "TM88")
        dPKMItems.Add(&H1A0, "TM89")
        dPKMItems.Add(&H1A1, "TM90")
        dPKMItems.Add(&H1A2, "TM91")
        dPKMItems.Add(&H1A3, "TM92")
        dPKMItems.Add(&H1A4, "HM01")
        dPKMItems.Add(&H1A5, "HM02")
        dPKMItems.Add(&H1A6, "HM03")
        dPKMItems.Add(&H1A7, "HM04")
        dPKMItems.Add(&H1A8, "HM05")
        dPKMItems.Add(&H1A9, "HM06")
        dPKMItems.Add(&H1AA, "HM07")
        dPKMItems.Add(&H1AB, "HM08")
        dPKMItems.Add(&H1AC, "Explorer Kit")
        dPKMItems.Add(&H1AD, "Loot Sack")
        dPKMItems.Add(&H1AE, "Rule Book")
        dPKMItems.Add(&H1AF, "Poke" & Char.ConvertFromUtf32(CNVRT.PKMToUnicode(&H188)) & " Radar")
        dPKMItems.Add(&H1B0, "Point Card")
        dPKMItems.Add(&H1B1, "Journal")
        dPKMItems.Add(&H1B2, "Seal Case")
        dPKMItems.Add(&H1B3, "Fashion Case")
        dPKMItems.Add(&H1B4, "Seal Bag")
        dPKMItems.Add(&H1B5, "Pal Pad")
        dPKMItems.Add(&H1B6, "Works Key")
        dPKMItems.Add(&H1B7, "Old Charm")
        dPKMItems.Add(&H1B8, "Galactic Key")
        dPKMItems.Add(&H1B9, "Red Chain")
        dPKMItems.Add(&H1BA, "Town Map")
        dPKMItems.Add(&H1BB, "Vs. Seeker")
        dPKMItems.Add(&H1BC, "Coin Case")
        dPKMItems.Add(&H1BD, "Old Rod")
        dPKMItems.Add(&H1BE, "Good Rod")
        dPKMItems.Add(&H1BF, "Super Rod")
        dPKMItems.Add(&H1C0, "Sprayduck")
        dPKMItems.Add(&H1C1, "Poffin Case")
        dPKMItems.Add(&H1C2, "Bicycle")
        dPKMItems.Add(&H1C3, "Suite Key")
        dPKMItems.Add(&H1C4, "Oak's Letter")
        dPKMItems.Add(&H1C5, "Lunar Wing")
        dPKMItems.Add(&H1C6, "Member Card")
        dPKMItems.Add(&H1C7, "Azure Flute")
        dPKMItems.Add(&H1C8, "S.S. Ticket")
        dPKMItems.Add(&H1C9, "Contest Pass")
        dPKMItems.Add(&H1CA, "Magma Stone")
        dPKMItems.Add(&H1CB, "Parcel")
        dPKMItems.Add(&H1CC, "Coupon 1")
        dPKMItems.Add(&H1CD, "Coupon 2")
        dPKMItems.Add(&H1CE, "Coupon 3")
        dPKMItems.Add(&H1CF, "Storage Key")
        dPKMItems.Add(&H1D0, "SecretPotion")
        dPKMItems.Add(&H1D1, "Vs. Recorder")
        dPKMItems.Add(&H1D2, "Gracidea")
        dPKMItems.Add(&H1D3, "Secret Key")

        'Multiplayer Avatar Names
        With dpAvatarNames
            .Add(&H0, "None")
            .Add(&H3, "School Kid")
            .Add(&H5, "Bug Catcher")
            .Add(&H6, "Lass")
            .Add(&H7, "Battle Girl")
            .Add(&HB, "Ace Trainer " & Char.ConvertFromUtf32(CNVRT.PKMToUnicode(&H1BB)))
            .Add(&HD, "Beauty")
            .Add(&HE, "Ace Trainer " & Char.ConvertFromUtf32(CNVRT.PKMToUnicode(&H1BC)))
            .Add(&H1F, "Roughneck")
            .Add(&H23, "Pop Idol")
            .Add(&H25, "Socialite")
            .Add(&H2A, "Cowgirl")
            .Add(&H32, "Ruin Maniac")
            .Add(&H33, "Black Belt")
            .Add(&H3E, "Rich Boy")
            .Add(&H3F, "Lady")
            .Add(&H46, "Psychic")
        End With

        'Pokémon Base Stats
        '//Base stats data: 1 HP, 2 ATK, 3 DEF, 4 SPD, 5 SPATK, 6 SPDEF, 7 type, 8 type, 9 catch rate, 10 base EXP,
        '// 11 HP EP, 12 ATK EP, 13 DEF EP, 14 SPD EP, 15 SPATK EP, 16 SPDEF EP, 17 gender, 18 base egg step,
        '//19 base tameness, 20 growth group, 21 egg group, 22 egg group, 23 ability, 24 ability, 25 color,
        '//26 D/P item 50%, 27 D/P item 5%, 28 Safari Zone Flag(?)
        dPKMBaseStats.Add(1, New UInt16() {45, 49, 49, 45, 65, 65, 12, 3, 45, 64, 0, 0, 0, 0, 1, 0, 31, 20, 70, 3, 1, 7, 65, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(2, New UInt16() {60, 62, 63, 60, 80, 80, 12, 3, 45, 141, 0, 0, 0, 0, 1, 1, 31, 20, 70, 3, 1, 7, 65, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(3, New UInt16() {80, 82, 83, 80, 100, 100, 12, 3, 45, 208, 0, 0, 0, 0, 2, 1, 31, 20, 70, 3, 1, 7, 65, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(4, New UInt16() {39, 52, 43, 65, 60, 50, 10, 10, 45, 65, 0, 0, 0, 1, 0, 0, 31, 20, 70, 3, 1, 14, 66, 0, 0, 0, 0, 0})
        dPKMBaseStats.Add(5, New UInt16() {58, 64, 58, 80, 80, 65, 10, 10, 45, 142, 0, 0, 0, 1, 1, 0, 31, 20, 70, 3, 1, 14, 66, 0, 0, 0, 0, 0})
        dPKMBaseStats.Add(6, New UInt16() {78, 84, 78, 100, 109, 85, 10, 2, 45, 209, 0, 0, 0, 0, 3, 0, 31, 20, 70, 3, 1, 14, 66, 0, 0, 0, 0, 0})
        dPKMBaseStats.Add(7, New UInt16() {44, 48, 65, 43, 50, 64, 11, 11, 45, 66, 0, 0, 1, 0, 0, 0, 31, 20, 70, 3, 1, 2, 67, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(8, New UInt16() {59, 63, 80, 58, 65, 80, 11, 11, 45, 143, 0, 0, 1, 0, 0, 1, 31, 20, 70, 3, 1, 2, 67, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(9, New UInt16() {79, 83, 100, 78, 85, 105, 11, 11, 45, 210, 0, 0, 0, 0, 0, 3, 31, 20, 70, 3, 1, 2, 67, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(10, New UInt16() {45, 30, 35, 45, 20, 20, 6, 6, 255, 53, 1, 0, 0, 0, 0, 0, 127, 15, 70, 0, 3, 3, 19, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(11, New UInt16() {50, 20, 55, 30, 25, 25, 6, 6, 120, 72, 0, 0, 2, 0, 0, 0, 127, 15, 70, 0, 3, 3, 61, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(12, New UInt16() {60, 45, 50, 70, 80, 80, 6, 2, 45, 160, 0, 0, 0, 0, 2, 1, 127, 15, 70, 0, 3, 3, 14, 0, 8, 0, 222, 0})
        dPKMBaseStats.Add(13, New UInt16() {40, 35, 30, 50, 20, 20, 6, 3, 255, 52, 0, 0, 0, 1, 0, 0, 127, 15, 70, 0, 3, 3, 19, 0, 5, 0, 0, 0})
        dPKMBaseStats.Add(14, New UInt16() {45, 25, 50, 35, 25, 25, 6, 3, 120, 71, 0, 0, 2, 0, 0, 0, 127, 15, 70, 0, 3, 3, 61, 0, 2, 0, 0, 0})
        dPKMBaseStats.Add(15, New UInt16() {65, 80, 40, 75, 45, 80, 6, 3, 45, 159, 0, 2, 0, 0, 0, 1, 127, 15, 70, 0, 3, 3, 68, 0, 2, 0, 245, 0})
        dPKMBaseStats.Add(16, New UInt16() {40, 45, 40, 56, 35, 35, 0, 2, 255, 55, 0, 0, 0, 1, 0, 0, 127, 15, 70, 3, 4, 4, 51, 77, 5, 0, 0, 0})
        dPKMBaseStats.Add(17, New UInt16() {63, 60, 55, 71, 50, 50, 0, 2, 120, 113, 0, 0, 0, 2, 0, 0, 127, 15, 70, 3, 4, 4, 51, 77, 5, 0, 0, 0})
        dPKMBaseStats.Add(18, New UInt16() {83, 80, 75, 91, 70, 70, 0, 2, 45, 172, 0, 0, 0, 3, 0, 0, 127, 15, 70, 3, 4, 4, 51, 77, 5, 0, 0, 0})
        dPKMBaseStats.Add(19, New UInt16() {30, 56, 35, 72, 25, 35, 0, 0, 255, 57, 0, 0, 0, 1, 0, 0, 127, 15, 70, 0, 5, 5, 50, 62, 6, 0, 200, 0})
        dPKMBaseStats.Add(20, New UInt16() {55, 81, 60, 97, 50, 70, 0, 0, 127, 116, 0, 0, 0, 2, 0, 0, 127, 15, 70, 0, 5, 5, 50, 62, 5, 0, 200, 0})
        dPKMBaseStats.Add(21, New UInt16() {40, 60, 30, 70, 31, 31, 0, 2, 255, 58, 0, 0, 0, 1, 0, 0, 127, 15, 70, 0, 4, 4, 51, 0, 5, 0, 0, 0})
        dPKMBaseStats.Add(22, New UInt16() {65, 90, 65, 100, 61, 61, 0, 2, 90, 162, 0, 0, 0, 2, 0, 0, 127, 15, 70, 0, 4, 4, 51, 0, 5, 0, 244, 0})
        dPKMBaseStats.Add(23, New UInt16() {35, 60, 44, 55, 40, 54, 3, 3, 255, 62, 0, 1, 0, 0, 0, 0, 127, 20, 70, 0, 5, 14, 22, 61, 6, 0, 0, 0})
        dPKMBaseStats.Add(24, New UInt16() {60, 85, 69, 80, 65, 79, 3, 3, 90, 147, 0, 2, 0, 0, 0, 0, 127, 20, 70, 0, 5, 14, 22, 61, 6, 0, 0, 60})
        dPKMBaseStats.Add(25, New UInt16() {35, 55, 30, 90, 50, 40, 13, 13, 190, 82, 0, 0, 0, 2, 0, 0, 127, 10, 70, 0, 5, 6, 9, 0, 2, 155, 236, 0})
        dPKMBaseStats.Add(26, New UInt16() {60, 90, 55, 100, 90, 80, 13, 13, 75, 122, 0, 0, 0, 3, 0, 0, 127, 10, 70, 0, 5, 6, 9, 0, 2, 0, 155, 0})
        dPKMBaseStats.Add(27, New UInt16() {50, 75, 85, 40, 20, 30, 4, 4, 255, 93, 0, 0, 1, 0, 0, 0, 127, 20, 70, 0, 5, 5, 8, 0, 2, 0, 217, 0})
        dPKMBaseStats.Add(28, New UInt16() {75, 100, 110, 65, 45, 55, 4, 4, 90, 163, 0, 0, 2, 0, 0, 0, 127, 20, 70, 0, 5, 5, 8, 0, 2, 0, 217, 0})
        dPKMBaseStats.Add(29, New UInt16() {55, 47, 52, 41, 40, 40, 3, 3, 235, 59, 1, 0, 0, 0, 0, 0, 254, 20, 70, 3, 1, 5, 38, 79, 1, 0, 0, 0})
        dPKMBaseStats.Add(30, New UInt16() {70, 62, 67, 56, 55, 55, 3, 3, 120, 117, 2, 0, 0, 0, 0, 0, 254, 20, 70, 3, 15, 15, 38, 79, 1, 0, 0, 0})
        dPKMBaseStats.Add(31, New UInt16() {90, 82, 87, 76, 75, 85, 3, 4, 45, 194, 3, 0, 0, 0, 0, 0, 254, 20, 70, 3, 15, 15, 38, 79, 1, 0, 0, 0})
        dPKMBaseStats.Add(32, New UInt16() {46, 57, 40, 50, 40, 40, 3, 3, 235, 60, 0, 1, 0, 0, 0, 0, 0, 20, 70, 3, 1, 5, 38, 79, 6, 0, 0, 0})
        dPKMBaseStats.Add(33, New UInt16() {61, 72, 57, 65, 55, 55, 3, 3, 120, 118, 0, 2, 0, 0, 0, 0, 0, 20, 70, 3, 1, 5, 38, 79, 6, 0, 0, 0})
        dPKMBaseStats.Add(34, New UInt16() {81, 92, 77, 85, 85, 75, 3, 4, 45, 195, 0, 3, 0, 0, 0, 0, 0, 20, 70, 3, 1, 5, 38, 79, 6, 0, 0, 0})
        dPKMBaseStats.Add(35, New UInt16() {70, 45, 48, 35, 60, 65, 0, 0, 150, 68, 2, 0, 0, 0, 0, 0, 191, 10, 140, 4, 6, 6, 56, 98, 9, 154, 81, 0})
        dPKMBaseStats.Add(36, New UInt16() {95, 70, 73, 60, 85, 90, 0, 0, 25, 129, 3, 0, 0, 0, 0, 0, 191, 10, 140, 4, 6, 6, 56, 98, 9, 154, 81, 0})
        dPKMBaseStats.Add(37, New UInt16() {38, 41, 40, 65, 50, 65, 10, 10, 190, 63, 0, 0, 0, 1, 0, 0, 191, 20, 70, 0, 5, 5, 18, 0, 5, 152, 152, 0})
        dPKMBaseStats.Add(38, New UInt16() {73, 76, 75, 100, 81, 100, 10, 10, 75, 178, 0, 0, 0, 1, 0, 1, 191, 20, 70, 0, 5, 5, 18, 0, 2, 152, 152, 0})
        dPKMBaseStats.Add(39, New UInt16() {115, 45, 20, 20, 45, 25, 0, 0, 170, 76, 2, 0, 0, 0, 0, 0, 191, 10, 70, 4, 6, 6, 56, 0, 9, 0, 0, 0})
        dPKMBaseStats.Add(40, New UInt16() {140, 70, 45, 45, 75, 50, 0, 0, 50, 109, 3, 0, 0, 0, 0, 0, 191, 10, 70, 4, 6, 6, 56, 0, 9, 0, 0, 0})
        dPKMBaseStats.Add(41, New UInt16() {40, 45, 35, 55, 30, 40, 3, 2, 255, 54, 0, 0, 0, 1, 0, 0, 127, 15, 70, 0, 4, 4, 39, 0, 6, 0, 0, 0})
        dPKMBaseStats.Add(42, New UInt16() {75, 80, 70, 90, 65, 75, 3, 2, 90, 171, 0, 0, 0, 2, 0, 0, 127, 15, 70, 0, 4, 4, 39, 0, 6, 0, 0, 0})
        dPKMBaseStats.Add(43, New UInt16() {45, 50, 55, 30, 75, 65, 12, 3, 255, 78, 0, 0, 0, 0, 1, 0, 127, 20, 70, 3, 7, 7, 34, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(44, New UInt16() {60, 65, 70, 40, 85, 75, 12, 3, 120, 132, 0, 0, 0, 0, 2, 0, 127, 20, 70, 3, 7, 7, 34, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(45, New UInt16() {75, 80, 85, 50, 100, 90, 12, 3, 45, 184, 0, 0, 0, 0, 3, 0, 127, 20, 70, 3, 7, 7, 34, 0, 0, 0, 0, 0})
        dPKMBaseStats.Add(46, New UInt16() {35, 70, 55, 25, 45, 55, 6, 12, 190, 70, 0, 1, 0, 0, 0, 0, 127, 20, 70, 0, 3, 7, 27, 87, 0, 86, 87, 120})
        dPKMBaseStats.Add(47, New UInt16() {60, 95, 80, 30, 60, 80, 6, 12, 75, 128, 0, 2, 1, 0, 0, 0, 127, 20, 70, 0, 3, 7, 27, 87, 0, 86, 87, 0})
        dPKMBaseStats.Add(48, New UInt16() {60, 55, 50, 45, 40, 55, 6, 3, 190, 75, 0, 0, 0, 0, 0, 1, 127, 20, 70, 0, 3, 3, 14, 110, 6, 0, 0, 0})
        dPKMBaseStats.Add(49, New UInt16() {70, 65, 60, 90, 90, 75, 6, 3, 75, 138, 0, 0, 0, 1, 1, 0, 127, 20, 70, 0, 3, 3, 19, 110, 6, 0, 295, 0})
        dPKMBaseStats.Add(50, New UInt16() {10, 55, 25, 95, 35, 45, 4, 4, 255, 81, 0, 0, 0, 1, 0, 0, 127, 20, 70, 0, 5, 5, 8, 71, 5, 0, 237, 0})
        dPKMBaseStats.Add(51, New UInt16() {35, 80, 50, 120, 50, 70, 4, 4, 50, 153, 0, 0, 0, 2, 0, 0, 127, 20, 70, 0, 5, 5, 8, 71, 5, 0, 237, 0})
        dPKMBaseStats.Add(52, New UInt16() {40, 45, 35, 90, 40, 40, 0, 0, 255, 69, 0, 0, 0, 1, 0, 0, 127, 20, 70, 0, 5, 5, 53, 101, 2, 0, 217, 0})
        dPKMBaseStats.Add(53, New UInt16() {65, 70, 60, 115, 65, 65, 0, 0, 90, 148, 0, 0, 0, 2, 0, 0, 127, 20, 70, 0, 5, 5, 7, 101, 2, 0, 217, 0})
        dPKMBaseStats.Add(54, New UInt16() {50, 52, 48, 55, 65, 50, 11, 11, 190, 80, 0, 0, 0, 0, 1, 0, 127, 20, 70, 0, 2, 5, 6, 13, 2, 0, 0, 90})
        dPKMBaseStats.Add(55, New UInt16() {80, 82, 78, 85, 95, 80, 11, 11, 75, 174, 0, 0, 0, 0, 2, 0, 127, 20, 70, 0, 2, 5, 6, 13, 1, 0, 0, 60})
        dPKMBaseStats.Add(56, New UInt16() {40, 80, 35, 70, 35, 45, 1, 1, 190, 74, 0, 1, 0, 0, 0, 0, 127, 20, 70, 0, 5, 5, 72, 83, 5, 0, 193, 0})
        dPKMBaseStats.Add(57, New UInt16() {65, 105, 60, 95, 60, 70, 1, 1, 75, 149, 0, 2, 0, 0, 0, 0, 127, 20, 70, 0, 5, 5, 72, 83, 5, 0, 193, 0})
        dPKMBaseStats.Add(58, New UInt16() {55, 70, 45, 60, 70, 50, 10, 10, 190, 91, 0, 1, 0, 0, 0, 0, 63, 20, 70, 5, 5, 5, 22, 18, 5, 152, 152, 0})
        dPKMBaseStats.Add(59, New UInt16() {90, 110, 80, 95, 100, 80, 10, 10, 75, 213, 0, 2, 0, 0, 0, 0, 63, 20, 70, 5, 5, 5, 22, 18, 5, 152, 152, 0})
        dPKMBaseStats.Add(60, New UInt16() {40, 50, 40, 90, 40, 40, 11, 11, 255, 77, 0, 0, 0, 1, 0, 0, 127, 20, 70, 3, 2, 2, 11, 6, 1, 0, 0, 0})
        dPKMBaseStats.Add(61, New UInt16() {65, 65, 65, 90, 50, 50, 11, 11, 120, 131, 0, 0, 0, 2, 0, 0, 127, 20, 70, 3, 2, 2, 11, 6, 1, 0, 221, 0})
        dPKMBaseStats.Add(62, New UInt16() {90, 85, 95, 70, 70, 90, 11, 1, 45, 185, 0, 0, 3, 0, 0, 0, 127, 20, 70, 3, 2, 2, 11, 6, 1, 0, 221, 0})
        dPKMBaseStats.Add(63, New UInt16() {25, 20, 15, 90, 105, 55, 14, 14, 200, 75, 0, 0, 0, 0, 1, 0, 63, 20, 70, 3, 8, 8, 28, 39, 5, 0, 248, 0})
        dPKMBaseStats.Add(64, New UInt16() {40, 35, 30, 105, 120, 70, 14, 14, 100, 145, 0, 0, 0, 0, 2, 0, 63, 20, 70, 3, 8, 8, 28, 39, 5, 0, 248, 0})
        dPKMBaseStats.Add(65, New UInt16() {55, 50, 45, 120, 135, 85, 14, 14, 50, 186, 0, 0, 0, 0, 3, 0, 63, 20, 70, 3, 8, 8, 28, 39, 5, 0, 248, 0})
        dPKMBaseStats.Add(66, New UInt16() {70, 80, 50, 35, 35, 35, 1, 1, 180, 75, 0, 1, 0, 0, 0, 0, 63, 20, 70, 3, 8, 8, 62, 99, 7, 0, 0, 0})
        dPKMBaseStats.Add(67, New UInt16() {80, 100, 70, 45, 50, 60, 1, 1, 90, 146, 0, 2, 0, 0, 0, 0, 63, 20, 70, 3, 8, 8, 62, 99, 7, 0, 0, 0})
        dPKMBaseStats.Add(68, New UInt16() {90, 130, 80, 55, 65, 85, 1, 1, 45, 193, 0, 3, 0, 0, 0, 0, 63, 20, 70, 3, 8, 8, 62, 99, 7, 0, 0, 0})
        dPKMBaseStats.Add(69, New UInt16() {50, 75, 35, 40, 70, 30, 12, 3, 255, 84, 0, 1, 0, 0, 0, 0, 127, 20, 70, 3, 7, 7, 34, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(70, New UInt16() {65, 90, 50, 55, 85, 45, 12, 3, 120, 151, 0, 2, 0, 0, 0, 0, 127, 20, 70, 3, 7, 7, 34, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(71, New UInt16() {80, 105, 65, 70, 100, 60, 12, 3, 45, 191, 0, 3, 0, 0, 0, 0, 127, 20, 70, 3, 7, 7, 34, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(72, New UInt16() {40, 40, 35, 70, 50, 100, 11, 3, 190, 105, 0, 0, 0, 0, 0, 1, 127, 20, 70, 5, 9, 9, 29, 64, 1, 0, 245, 0})
        dPKMBaseStats.Add(73, New UInt16() {80, 70, 65, 100, 80, 120, 11, 3, 60, 205, 0, 0, 0, 0, 0, 2, 127, 20, 70, 5, 9, 9, 29, 64, 1, 0, 245, 0})
        dPKMBaseStats.Add(74, New UInt16() {40, 80, 100, 20, 30, 30, 5, 4, 255, 73, 0, 0, 1, 0, 0, 0, 127, 15, 70, 3, 10, 10, 69, 5, 5, 0, 229, 0})
        dPKMBaseStats.Add(75, New UInt16() {55, 95, 115, 35, 45, 45, 5, 4, 120, 134, 0, 0, 2, 0, 0, 0, 127, 15, 70, 3, 10, 10, 69, 5, 5, 0, 229, 0})
        dPKMBaseStats.Add(76, New UInt16() {80, 110, 130, 45, 55, 65, 5, 4, 45, 177, 0, 0, 3, 0, 0, 0, 127, 15, 70, 3, 10, 10, 69, 5, 5, 0, 229, 0})
        dPKMBaseStats.Add(77, New UInt16() {50, 85, 55, 90, 65, 65, 10, 10, 190, 152, 0, 0, 0, 1, 0, 0, 127, 20, 70, 0, 5, 5, 50, 18, 2, 0, 191, 0})
        dPKMBaseStats.Add(78, New UInt16() {65, 100, 70, 105, 80, 80, 10, 10, 60, 192, 0, 0, 0, 2, 0, 0, 127, 20, 70, 0, 5, 5, 50, 18, 2, 0, 191, 0})
        dPKMBaseStats.Add(79, New UInt16() {90, 65, 65, 15, 40, 40, 11, 14, 190, 99, 1, 0, 0, 0, 0, 0, 127, 20, 70, 0, 1, 2, 12, 20, 9, 0, 279, 0})
        dPKMBaseStats.Add(80, New UInt16() {95, 75, 110, 30, 100, 80, 11, 14, 75, 164, 0, 0, 2, 0, 0, 0, 127, 20, 70, 0, 1, 2, 12, 20, 9, 0, 221, 0})
        dPKMBaseStats.Add(81, New UInt16() {25, 35, 70, 45, 95, 55, 13, 8, 190, 89, 0, 0, 0, 0, 1, 0, 255, 20, 70, 0, 10, 10, 42, 5, 7, 0, 233, 0})
        dPKMBaseStats.Add(82, New UInt16() {50, 60, 95, 70, 120, 70, 13, 8, 60, 161, 0, 0, 0, 0, 2, 0, 255, 20, 70, 0, 10, 10, 42, 5, 7, 0, 233, 0})
        dPKMBaseStats.Add(83, New UInt16() {52, 65, 55, 60, 58, 62, 0, 2, 45, 94, 0, 1, 0, 0, 0, 0, 127, 20, 70, 0, 4, 5, 51, 39, 5, 0, 259, 0})
        dPKMBaseStats.Add(84, New UInt16() {35, 85, 45, 75, 35, 35, 0, 2, 190, 96, 0, 1, 0, 0, 0, 0, 127, 20, 70, 0, 4, 4, 50, 48, 5, 0, 244, 0})
        dPKMBaseStats.Add(85, New UInt16() {60, 110, 70, 100, 60, 60, 0, 2, 45, 158, 0, 2, 0, 0, 0, 0, 127, 20, 70, 0, 4, 4, 50, 48, 5, 0, 244, 0})
        dPKMBaseStats.Add(86, New UInt16() {65, 45, 55, 45, 45, 70, 11, 11, 190, 100, 0, 0, 0, 0, 0, 1, 127, 20, 70, 0, 2, 5, 47, 93, 8, 0, 0, 0})
        dPKMBaseStats.Add(87, New UInt16() {90, 70, 80, 70, 70, 95, 11, 15, 75, 176, 0, 0, 0, 0, 0, 2, 127, 20, 70, 0, 2, 5, 47, 93, 8, 0, 0, 0})
        dPKMBaseStats.Add(88, New UInt16() {80, 80, 50, 25, 40, 50, 3, 3, 190, 90, 1, 0, 0, 0, 0, 0, 127, 20, 70, 0, 11, 11, 1, 60, 6, 0, 92, 0})
        dPKMBaseStats.Add(89, New UInt16() {105, 105, 75, 50, 65, 100, 3, 3, 75, 157, 1, 1, 0, 0, 0, 0, 127, 20, 70, 0, 11, 11, 1, 60, 6, 0, 92, 0})
        dPKMBaseStats.Add(90, New UInt16() {30, 65, 100, 40, 45, 25, 11, 11, 190, 97, 0, 0, 1, 0, 0, 0, 127, 20, 70, 5, 9, 9, 75, 92, 6, 88, 89, 0})
        dPKMBaseStats.Add(91, New UInt16() {50, 95, 180, 70, 85, 45, 11, 15, 60, 203, 0, 0, 2, 0, 0, 0, 127, 20, 70, 5, 9, 9, 75, 92, 6, 88, 89, 0})
        dPKMBaseStats.Add(92, New UInt16() {30, 35, 30, 80, 100, 35, 7, 3, 190, 95, 0, 0, 0, 0, 1, 0, 127, 20, 70, 3, 11, 11, 26, 0, 6, 0, 0, 0})
        dPKMBaseStats.Add(93, New UInt16() {45, 50, 45, 95, 115, 55, 7, 3, 90, 126, 0, 0, 0, 0, 2, 0, 127, 20, 70, 3, 11, 11, 26, 0, 6, 0, 0, 0})
        dPKMBaseStats.Add(94, New UInt16() {60, 65, 60, 110, 130, 75, 7, 3, 45, 190, 0, 0, 0, 0, 3, 0, 127, 20, 70, 3, 11, 11, 26, 0, 6, 0, 0, 0})
        dPKMBaseStats.Add(95, New UInt16() {35, 45, 160, 70, 30, 45, 5, 4, 45, 108, 0, 0, 1, 0, 0, 0, 127, 25, 70, 0, 10, 10, 69, 5, 7, 0, 0, 0})
        dPKMBaseStats.Add(96, New UInt16() {60, 48, 45, 42, 43, 90, 14, 14, 190, 102, 0, 0, 0, 0, 0, 1, 127, 20, 70, 0, 8, 8, 15, 108, 2, 0, 0, 0})
        dPKMBaseStats.Add(97, New UInt16() {85, 73, 70, 67, 73, 115, 14, 14, 75, 165, 0, 0, 0, 0, 0, 2, 127, 20, 70, 0, 8, 8, 15, 108, 2, 0, 0, 0})
        dPKMBaseStats.Add(98, New UInt16() {30, 105, 90, 50, 25, 25, 11, 11, 225, 115, 0, 1, 0, 0, 0, 0, 127, 20, 70, 0, 9, 9, 52, 75, 0, 0, 0, 0})
        dPKMBaseStats.Add(99, New UInt16() {55, 130, 115, 75, 50, 50, 11, 11, 60, 206, 0, 2, 0, 0, 0, 0, 127, 20, 70, 0, 9, 9, 52, 75, 0, 0, 0, 0})
        dPKMBaseStats.Add(100, New UInt16() {40, 30, 50, 100, 55, 55, 13, 13, 190, 103, 0, 0, 0, 1, 0, 0, 255, 20, 70, 0, 10, 10, 43, 9, 0, 0, 0, 0})
        dPKMBaseStats.Add(101, New UInt16() {60, 50, 70, 140, 80, 80, 13, 13, 60, 150, 0, 0, 0, 2, 0, 0, 255, 20, 70, 0, 10, 10, 43, 9, 0, 0, 0, 0})
        dPKMBaseStats.Add(102, New UInt16() {60, 40, 80, 40, 60, 45, 12, 14, 90, 98, 0, 0, 1, 0, 0, 0, 127, 20, 70, 5, 7, 7, 34, 0, 9, 0, 0, 120})
        dPKMBaseStats.Add(103, New UInt16() {95, 95, 85, 55, 125, 65, 12, 14, 45, 212, 0, 0, 0, 0, 2, 0, 127, 20, 70, 5, 7, 7, 34, 0, 2, 0, 0, 0})
        dPKMBaseStats.Add(104, New UInt16() {50, 50, 95, 35, 40, 50, 4, 4, 190, 87, 0, 0, 1, 0, 0, 0, 127, 20, 70, 0, 1, 1, 69, 31, 5, 0, 258, 0})
        dPKMBaseStats.Add(105, New UInt16() {60, 80, 110, 45, 50, 80, 4, 4, 75, 124, 0, 0, 2, 0, 0, 0, 127, 20, 70, 0, 1, 1, 69, 31, 5, 0, 258, 0})
        dPKMBaseStats.Add(106, New UInt16() {50, 120, 53, 87, 35, 110, 1, 1, 45, 139, 0, 2, 0, 0, 0, 0, 0, 25, 70, 0, 8, 8, 7, 120, 5, 0, 0, 0})
        dPKMBaseStats.Add(107, New UInt16() {50, 105, 79, 76, 35, 110, 1, 1, 45, 140, 0, 0, 0, 0, 0, 2, 0, 25, 70, 0, 8, 8, 51, 89, 5, 0, 0, 0})
        dPKMBaseStats.Add(108, New UInt16() {90, 55, 75, 30, 60, 75, 0, 0, 45, 127, 2, 0, 0, 0, 0, 0, 127, 20, 70, 0, 1, 1, 20, 12, 9, 0, 279, 0})
        dPKMBaseStats.Add(109, New UInt16() {40, 65, 95, 35, 60, 45, 3, 3, 190, 114, 0, 0, 1, 0, 0, 0, 127, 20, 70, 0, 11, 11, 26, 0, 6, 0, 228, 0})
        dPKMBaseStats.Add(110, New UInt16() {65, 90, 120, 60, 85, 70, 3, 3, 60, 173, 0, 0, 2, 0, 0, 0, 127, 20, 70, 0, 11, 11, 26, 0, 6, 0, 228, 0})
        dPKMBaseStats.Add(111, New UInt16() {80, 85, 95, 25, 30, 30, 4, 5, 120, 135, 0, 0, 1, 0, 0, 0, 127, 20, 70, 5, 1, 5, 31, 69, 7, 0, 0, 0})
        dPKMBaseStats.Add(112, New UInt16() {105, 130, 120, 40, 45, 45, 4, 5, 60, 204, 0, 2, 0, 0, 0, 0, 127, 20, 70, 5, 1, 5, 31, 69, 7, 0, 0, 0})
        dPKMBaseStats.Add(113, New UInt16() {250, 5, 5, 50, 35, 105, 0, 0, 30, 255, 2, 0, 0, 0, 0, 0, 254, 40, 140, 4, 6, 6, 30, 32, 9, 110, 231, 0})
        dPKMBaseStats.Add(114, New UInt16() {65, 55, 115, 60, 100, 40, 12, 12, 45, 166, 0, 0, 1, 0, 0, 0, 127, 20, 70, 0, 7, 7, 34, 102, 1, 0, 0, 0})
        dPKMBaseStats.Add(115, New UInt16() {105, 95, 80, 90, 40, 80, 0, 0, 45, 175, 2, 0, 0, 0, 0, 0, 254, 20, 70, 0, 1, 1, 48, 113, 5, 0, 0, 150})
        dPKMBaseStats.Add(116, New UInt16() {30, 40, 70, 60, 70, 25, 11, 11, 225, 83, 0, 0, 0, 0, 1, 0, 127, 20, 70, 0, 2, 14, 33, 97, 1, 0, 235, 0})
        dPKMBaseStats.Add(117, New UInt16() {55, 65, 95, 85, 95, 45, 11, 11, 75, 155, 0, 0, 1, 0, 1, 0, 127, 20, 70, 0, 2, 14, 38, 97, 1, 0, 235, 0})
        dPKMBaseStats.Add(118, New UInt16() {45, 67, 60, 63, 35, 50, 11, 11, 225, 111, 0, 1, 0, 0, 0, 0, 127, 20, 70, 0, 12, 12, 33, 41, 0, 0, 0, 0})
        dPKMBaseStats.Add(119, New UInt16() {80, 92, 65, 68, 65, 80, 11, 11, 60, 170, 0, 2, 0, 0, 0, 0, 127, 20, 70, 0, 12, 12, 33, 41, 0, 0, 0, 0})
        dPKMBaseStats.Add(120, New UInt16() {30, 45, 55, 85, 70, 55, 11, 11, 225, 106, 0, 0, 0, 1, 0, 0, 255, 20, 70, 5, 9, 9, 35, 30, 5, 90, 91, 0})
        dPKMBaseStats.Add(121, New UInt16() {60, 75, 85, 115, 100, 85, 11, 14, 60, 207, 0, 0, 0, 2, 0, 0, 255, 20, 70, 5, 9, 9, 35, 30, 6, 90, 91, 0})
        dPKMBaseStats.Add(122, New UInt16() {40, 45, 65, 90, 100, 120, 14, 14, 45, 136, 0, 0, 0, 0, 0, 2, 127, 25, 70, 0, 8, 8, 43, 111, 9, 0, 154, 0})
        dPKMBaseStats.Add(123, New UInt16() {70, 110, 80, 105, 55, 80, 6, 2, 45, 187, 0, 1, 0, 0, 0, 0, 127, 25, 70, 0, 3, 3, 68, 101, 3, 0, 0, 0})
        dPKMBaseStats.Add(124, New UInt16() {65, 50, 35, 95, 115, 95, 15, 14, 45, 137, 0, 0, 0, 0, 2, 0, 254, 25, 70, 0, 8, 8, 12, 108, 0, 153, 153, 0})
        dPKMBaseStats.Add(125, New UInt16() {65, 83, 57, 105, 95, 85, 13, 13, 45, 156, 0, 0, 0, 2, 0, 0, 63, 25, 70, 0, 8, 8, 9, 0, 2, 322, 0, 0})
        dPKMBaseStats.Add(126, New UInt16() {65, 95, 57, 93, 100, 85, 10, 10, 45, 167, 0, 0, 0, 0, 2, 0, 63, 25, 70, 0, 8, 8, 49, 0, 0, 0, 323, 0})
        dPKMBaseStats.Add(127, New UInt16() {65, 125, 100, 85, 55, 70, 6, 6, 45, 200, 0, 2, 0, 0, 0, 0, 127, 25, 70, 5, 3, 3, 52, 104, 5, 0, 0, 0})
        dPKMBaseStats.Add(128, New UInt16() {75, 100, 95, 110, 40, 70, 0, 0, 45, 211, 0, 1, 0, 1, 0, 0, 0, 20, 70, 5, 5, 5, 22, 83, 5, 0, 0, 0})
        dPKMBaseStats.Add(129, New UInt16() {20, 10, 55, 80, 15, 20, 11, 11, 255, 20, 0, 0, 0, 1, 0, 0, 127, 5, 70, 5, 12, 14, 33, 0, 0, 0, 0, 90})
        dPKMBaseStats.Add(130, New UInt16() {95, 125, 79, 81, 60, 100, 11, 2, 45, 214, 0, 2, 0, 0, 0, 0, 127, 5, 70, 5, 12, 14, 22, 0, 1, 0, 0, 60})
        dPKMBaseStats.Add(131, New UInt16() {130, 85, 80, 60, 85, 95, 11, 15, 45, 219, 2, 0, 0, 0, 0, 0, 127, 40, 70, 5, 1, 2, 11, 75, 1, 0, 0, 0})
        dPKMBaseStats.Add(132, New UInt16() {48, 48, 48, 48, 48, 48, 0, 0, 35, 61, 1, 0, 0, 0, 0, 0, 255, 20, 70, 0, 13, 13, 7, 0, 6, 274, 257, 0})
        dPKMBaseStats.Add(133, New UInt16() {55, 55, 50, 55, 45, 65, 0, 0, 45, 92, 0, 0, 0, 0, 0, 1, 31, 35, 70, 0, 5, 5, 50, 91, 5, 0, 0, 0})
        dPKMBaseStats.Add(134, New UInt16() {130, 65, 60, 65, 110, 95, 11, 11, 45, 196, 2, 0, 0, 0, 0, 0, 31, 35, 70, 0, 5, 5, 11, 11, 1, 0, 0, 0})
        dPKMBaseStats.Add(135, New UInt16() {65, 65, 60, 130, 110, 95, 13, 13, 45, 197, 0, 0, 0, 2, 0, 0, 31, 35, 70, 0, 5, 5, 10, 10, 2, 0, 0, 0})
        dPKMBaseStats.Add(136, New UInt16() {65, 130, 60, 65, 95, 110, 10, 10, 45, 198, 0, 2, 0, 0, 0, 0, 31, 35, 70, 0, 5, 5, 18, 18, 0, 0, 0, 0})
        dPKMBaseStats.Add(137, New UInt16() {65, 60, 70, 40, 85, 75, 0, 0, 45, 130, 0, 0, 0, 0, 1, 0, 255, 20, 70, 0, 10, 10, 36, 88, 9, 0, 0, 0})
        dPKMBaseStats.Add(138, New UInt16() {35, 40, 100, 35, 90, 55, 5, 11, 45, 99, 0, 0, 1, 0, 0, 0, 31, 30, 70, 0, 2, 9, 33, 75, 1, 0, 0, 0})
        dPKMBaseStats.Add(139, New UInt16() {70, 60, 125, 55, 115, 70, 5, 11, 45, 199, 0, 0, 2, 0, 0, 0, 31, 30, 70, 0, 2, 9, 33, 75, 1, 0, 0, 0})
        dPKMBaseStats.Add(140, New UInt16() {30, 80, 90, 55, 55, 45, 5, 11, 45, 99, 0, 0, 1, 0, 0, 0, 31, 30, 70, 0, 2, 9, 33, 4, 5, 0, 0, 0})
        dPKMBaseStats.Add(141, New UInt16() {60, 115, 105, 80, 65, 70, 5, 11, 45, 199, 0, 2, 0, 0, 0, 0, 31, 30, 70, 0, 2, 9, 33, 4, 5, 0, 0, 0})
        dPKMBaseStats.Add(142, New UInt16() {80, 105, 65, 130, 60, 75, 5, 2, 45, 202, 0, 0, 0, 2, 0, 0, 31, 35, 70, 5, 4, 4, 69, 46, 6, 0, 0, 0})
        dPKMBaseStats.Add(143, New UInt16() {160, 110, 65, 30, 65, 110, 0, 0, 25, 154, 2, 0, 0, 0, 0, 0, 31, 40, 70, 5, 1, 1, 17, 47, 4, 234, 234, 0})
        dPKMBaseStats.Add(144, New UInt16() {90, 85, 100, 85, 95, 125, 15, 2, 3, 215, 0, 0, 0, 0, 0, 3, 255, 80, 35, 5, 15, 15, 46, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(145, New UInt16() {90, 90, 85, 100, 125, 90, 13, 2, 3, 216, 0, 0, 0, 0, 3, 0, 255, 80, 35, 5, 15, 15, 46, 0, 2, 0, 0, 0})
        dPKMBaseStats.Add(146, New UInt16() {90, 100, 90, 90, 125, 85, 10, 2, 3, 217, 0, 0, 0, 0, 3, 0, 255, 80, 35, 5, 15, 15, 46, 0, 2, 0, 0, 0})
        dPKMBaseStats.Add(147, New UInt16() {41, 64, 45, 50, 50, 50, 16, 16, 45, 67, 0, 1, 0, 0, 0, 0, 127, 40, 35, 5, 2, 14, 61, 0, 1, 0, 235, 0})
        dPKMBaseStats.Add(148, New UInt16() {61, 84, 65, 70, 70, 70, 16, 16, 45, 144, 0, 2, 0, 0, 0, 0, 127, 40, 35, 5, 2, 14, 61, 0, 1, 0, 235, 0})
        dPKMBaseStats.Add(149, New UInt16() {91, 134, 95, 80, 100, 100, 16, 2, 45, 218, 0, 3, 0, 0, 0, 0, 127, 40, 35, 5, 2, 14, 39, 0, 5, 0, 235, 0})
        dPKMBaseStats.Add(150, New UInt16() {106, 110, 90, 130, 154, 90, 14, 14, 3, 220, 0, 0, 0, 0, 3, 0, 255, 120, 0, 5, 15, 15, 46, 0, 6, 0, 0, 0})
        dPKMBaseStats.Add(151, New UInt16() {100, 100, 100, 100, 100, 100, 14, 14, 45, 64, 3, 0, 0, 0, 0, 0, 255, 120, 100, 3, 15, 15, 28, 0, 9, 157, 157, 0})
        dPKMBaseStats.Add(152, New UInt16() {45, 49, 65, 45, 49, 65, 12, 12, 45, 64, 0, 0, 0, 0, 0, 1, 31, 20, 70, 3, 1, 7, 65, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(153, New UInt16() {60, 62, 80, 60, 63, 80, 12, 12, 45, 141, 0, 0, 1, 0, 0, 1, 31, 20, 70, 3, 1, 7, 65, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(154, New UInt16() {80, 82, 100, 80, 83, 100, 12, 12, 45, 208, 0, 0, 1, 0, 0, 2, 31, 20, 70, 3, 1, 7, 65, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(155, New UInt16() {39, 52, 43, 65, 60, 50, 10, 10, 45, 65, 0, 0, 0, 1, 0, 0, 31, 20, 70, 3, 5, 5, 66, 0, 2, 0, 0, 0})
        dPKMBaseStats.Add(156, New UInt16() {58, 64, 58, 80, 80, 65, 10, 10, 45, 142, 0, 0, 0, 1, 1, 0, 31, 20, 70, 3, 5, 5, 66, 0, 2, 0, 0, 0})
        dPKMBaseStats.Add(157, New UInt16() {78, 84, 78, 100, 109, 85, 10, 10, 45, 209, 0, 0, 0, 0, 3, 0, 31, 20, 70, 3, 5, 5, 66, 0, 2, 0, 0, 0})
        dPKMBaseStats.Add(158, New UInt16() {50, 65, 64, 43, 44, 48, 11, 11, 45, 66, 0, 1, 0, 0, 0, 0, 31, 20, 70, 3, 1, 2, 67, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(159, New UInt16() {65, 80, 80, 58, 59, 63, 11, 11, 45, 143, 0, 1, 1, 0, 0, 0, 31, 20, 70, 3, 1, 2, 67, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(160, New UInt16() {85, 105, 100, 78, 79, 83, 11, 11, 45, 210, 0, 2, 1, 0, 0, 0, 31, 20, 70, 3, 1, 2, 67, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(161, New UInt16() {35, 46, 34, 20, 35, 45, 0, 0, 255, 57, 0, 1, 0, 0, 0, 0, 127, 15, 70, 0, 5, 5, 50, 51, 5, 0, 155, 0})
        dPKMBaseStats.Add(162, New UInt16() {85, 76, 64, 90, 45, 55, 0, 0, 90, 116, 0, 0, 0, 2, 0, 0, 127, 15, 70, 0, 5, 5, 50, 51, 5, 155, 158, 0})
        dPKMBaseStats.Add(163, New UInt16() {60, 30, 30, 50, 36, 56, 0, 2, 255, 58, 1, 0, 0, 0, 0, 0, 127, 15, 70, 0, 4, 4, 15, 51, 5, 0, 0, 90})
        dPKMBaseStats.Add(164, New UInt16() {100, 50, 50, 70, 76, 96, 0, 2, 90, 162, 2, 0, 0, 0, 0, 0, 127, 15, 70, 0, 4, 4, 15, 51, 5, 0, 0, 60})
        dPKMBaseStats.Add(165, New UInt16() {40, 20, 30, 55, 40, 80, 6, 2, 255, 54, 0, 0, 0, 0, 0, 1, 127, 15, 70, 4, 3, 3, 68, 48, 0, 0, 0, 0})
        dPKMBaseStats.Add(166, New UInt16() {55, 35, 50, 85, 55, 110, 6, 2, 90, 134, 0, 0, 0, 0, 0, 2, 127, 15, 70, 4, 3, 3, 68, 48, 0, 0, 0, 0})
        dPKMBaseStats.Add(167, New UInt16() {40, 60, 40, 30, 40, 40, 6, 3, 255, 54, 0, 1, 0, 0, 0, 0, 127, 15, 70, 4, 3, 3, 68, 15, 3, 0, 0, 0})
        dPKMBaseStats.Add(168, New UInt16() {70, 90, 70, 40, 60, 60, 6, 3, 90, 134, 0, 2, 0, 0, 0, 0, 127, 15, 70, 4, 3, 3, 68, 15, 0, 0, 0, 0})
        dPKMBaseStats.Add(169, New UInt16() {85, 90, 80, 130, 70, 80, 3, 2, 90, 204, 0, 0, 0, 3, 0, 0, 127, 15, 70, 0, 4, 4, 39, 0, 6, 0, 0, 0})
        dPKMBaseStats.Add(170, New UInt16() {75, 38, 38, 67, 56, 56, 11, 13, 190, 90, 1, 0, 0, 0, 0, 0, 127, 20, 70, 5, 12, 12, 10, 35, 1, 0, 227, 0})
        dPKMBaseStats.Add(171, New UInt16() {125, 58, 58, 67, 76, 76, 11, 13, 75, 156, 2, 0, 0, 0, 0, 0, 127, 20, 70, 5, 12, 12, 10, 35, 1, 0, 227, 0})
        dPKMBaseStats.Add(172, New UInt16() {20, 40, 15, 60, 35, 35, 13, 13, 190, 42, 0, 0, 0, 1, 0, 0, 127, 10, 70, 0, 15, 15, 9, 0, 2, 0, 155, 0})
        dPKMBaseStats.Add(173, New UInt16() {50, 25, 28, 15, 45, 55, 0, 0, 150, 37, 0, 0, 0, 0, 0, 1, 191, 10, 140, 4, 15, 15, 56, 98, 9, 154, 81, 0})
        dPKMBaseStats.Add(174, New UInt16() {90, 30, 15, 15, 40, 20, 0, 0, 170, 39, 1, 0, 0, 0, 0, 0, 191, 10, 70, 4, 15, 15, 56, 0, 9, 0, 0, 0})
        dPKMBaseStats.Add(175, New UInt16() {35, 20, 65, 20, 40, 65, 0, 0, 190, 74, 0, 0, 0, 0, 0, 1, 31, 10, 70, 4, 15, 15, 55, 32, 8, 0, 0, 0})
        dPKMBaseStats.Add(176, New UInt16() {55, 40, 85, 40, 80, 105, 0, 2, 75, 114, 0, 0, 0, 0, 0, 2, 31, 10, 70, 4, 4, 6, 55, 32, 8, 0, 0, 0})
        dPKMBaseStats.Add(177, New UInt16() {40, 50, 45, 70, 70, 45, 14, 2, 190, 73, 0, 0, 0, 0, 1, 0, 127, 20, 70, 0, 4, 4, 28, 48, 3, 0, 0, 0})
        dPKMBaseStats.Add(178, New UInt16() {65, 75, 70, 95, 95, 70, 14, 2, 75, 171, 0, 0, 0, 1, 1, 0, 127, 20, 70, 0, 4, 4, 28, 48, 3, 0, 0, 0})
        dPKMBaseStats.Add(179, New UInt16() {55, 40, 40, 35, 65, 45, 13, 13, 235, 59, 0, 0, 0, 0, 1, 0, 127, 20, 70, 3, 1, 5, 9, 0, 8, 0, 0, 0})
        dPKMBaseStats.Add(180, New UInt16() {70, 55, 55, 45, 80, 60, 13, 13, 120, 117, 0, 0, 0, 0, 2, 0, 127, 20, 70, 3, 1, 5, 9, 0, 9, 0, 0, 0})
        dPKMBaseStats.Add(181, New UInt16() {90, 75, 75, 55, 115, 90, 13, 13, 45, 194, 0, 0, 0, 0, 3, 0, 127, 20, 70, 3, 1, 5, 9, 0, 2, 0, 0, 0})
        dPKMBaseStats.Add(182, New UInt16() {75, 80, 85, 50, 90, 100, 12, 12, 45, 184, 0, 0, 0, 0, 0, 3, 127, 20, 70, 3, 7, 7, 34, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(183, New UInt16() {70, 20, 50, 40, 20, 50, 11, 11, 190, 58, 2, 0, 0, 0, 0, 0, 127, 10, 70, 4, 2, 6, 47, 37, 1, 0, 0, 60})
        dPKMBaseStats.Add(184, New UInt16() {100, 50, 80, 50, 50, 80, 11, 11, 75, 153, 3, 0, 0, 0, 0, 0, 127, 10, 70, 4, 2, 6, 47, 37, 1, 0, 0, 0})
        dPKMBaseStats.Add(185, New UInt16() {70, 100, 115, 30, 30, 65, 5, 5, 65, 135, 0, 0, 2, 0, 0, 0, 127, 20, 70, 0, 10, 10, 5, 69, 5, 0, 0, 0})
        dPKMBaseStats.Add(186, New UInt16() {90, 75, 75, 70, 90, 100, 11, 11, 45, 185, 0, 0, 0, 0, 0, 3, 127, 20, 70, 3, 2, 2, 11, 6, 3, 0, 221, 0})
        dPKMBaseStats.Add(187, New UInt16() {35, 35, 40, 50, 35, 55, 12, 2, 255, 74, 0, 0, 0, 0, 0, 1, 127, 20, 70, 3, 6, 7, 34, 102, 9, 0, 0, 0})
        dPKMBaseStats.Add(188, New UInt16() {55, 45, 50, 80, 45, 65, 12, 2, 120, 136, 0, 0, 0, 2, 0, 0, 127, 20, 70, 3, 6, 7, 34, 102, 3, 0, 0, 0})
        dPKMBaseStats.Add(189, New UInt16() {75, 55, 70, 110, 55, 85, 12, 2, 45, 176, 0, 0, 0, 3, 0, 0, 127, 20, 70, 3, 6, 7, 34, 102, 1, 0, 0, 0})
        dPKMBaseStats.Add(190, New UInt16() {55, 70, 55, 85, 40, 55, 0, 0, 45, 94, 0, 0, 0, 1, 0, 0, 127, 20, 70, 4, 5, 5, 50, 53, 6, 0, 0, 0})
        dPKMBaseStats.Add(191, New UInt16() {30, 30, 30, 30, 30, 30, 12, 12, 235, 52, 0, 0, 0, 0, 1, 0, 127, 20, 70, 3, 7, 7, 34, 94, 2, 0, 192, 0})
        dPKMBaseStats.Add(192, New UInt16() {75, 75, 55, 30, 105, 85, 12, 12, 120, 146, 0, 0, 0, 0, 2, 0, 127, 20, 70, 3, 7, 7, 34, 94, 2, 0, 0, 0})
        dPKMBaseStats.Add(193, New UInt16() {65, 65, 45, 95, 75, 45, 6, 2, 75, 147, 0, 0, 0, 1, 0, 0, 127, 20, 70, 0, 3, 3, 3, 14, 0, 0, 265, 120})
        dPKMBaseStats.Add(194, New UInt16() {55, 45, 45, 15, 25, 25, 11, 4, 255, 52, 1, 0, 0, 0, 0, 0, 127, 20, 70, 0, 2, 5, 6, 11, 1, 0, 0, 120})
        dPKMBaseStats.Add(195, New UInt16() {95, 85, 85, 35, 65, 65, 11, 4, 90, 137, 2, 0, 0, 0, 0, 0, 127, 20, 70, 0, 2, 5, 6, 11, 1, 0, 0, 60})
        dPKMBaseStats.Add(196, New UInt16() {65, 65, 60, 110, 130, 95, 14, 14, 45, 197, 0, 0, 0, 0, 2, 0, 31, 35, 70, 0, 5, 5, 28, 28, 6, 0, 0, 0})
        dPKMBaseStats.Add(197, New UInt16() {95, 65, 110, 65, 60, 130, 17, 17, 45, 197, 0, 0, 0, 0, 0, 2, 31, 35, 35, 0, 5, 5, 28, 28, 4, 0, 0, 0})
        dPKMBaseStats.Add(198, New UInt16() {60, 85, 42, 91, 85, 42, 17, 2, 30, 107, 0, 0, 0, 1, 0, 0, 127, 20, 35, 3, 4, 4, 15, 105, 4, 0, 0, 0})
        dPKMBaseStats.Add(199, New UInt16() {95, 75, 80, 30, 100, 110, 11, 14, 70, 164, 0, 0, 0, 0, 0, 3, 127, 20, 70, 0, 1, 2, 12, 20, 9, 0, 221, 0})
        dPKMBaseStats.Add(200, New UInt16() {60, 60, 60, 85, 85, 85, 7, 7, 45, 147, 0, 0, 0, 0, 0, 1, 127, 25, 35, 4, 11, 11, 26, 0, 7, 0, 0, 0})
        dPKMBaseStats.Add(201, New UInt16() {48, 72, 48, 48, 72, 48, 14, 14, 225, 61, 0, 1, 0, 0, 1, 0, 255, 40, 70, 0, 15, 15, 26, 0, 4, 0, 0, 0})
        dPKMBaseStats.Add(202, New UInt16() {190, 33, 58, 33, 33, 58, 14, 14, 45, 177, 2, 0, 0, 0, 0, 0, 127, 20, 70, 0, 11, 11, 23, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(203, New UInt16() {70, 80, 65, 85, 90, 65, 0, 14, 60, 149, 0, 0, 0, 0, 2, 0, 127, 20, 70, 0, 5, 5, 39, 48, 2, 0, 156, 0})
        dPKMBaseStats.Add(204, New UInt16() {50, 65, 90, 15, 35, 35, 6, 6, 190, 60, 0, 0, 1, 0, 0, 0, 127, 20, 70, 0, 3, 3, 5, 0, 7, 0, 0, 0})
        dPKMBaseStats.Add(205, New UInt16() {75, 90, 140, 40, 60, 60, 6, 8, 75, 118, 0, 0, 2, 0, 0, 0, 127, 20, 70, 0, 3, 3, 5, 0, 6, 0, 0, 0})
        dPKMBaseStats.Add(206, New UInt16() {100, 70, 70, 45, 65, 65, 0, 0, 190, 125, 1, 0, 0, 0, 0, 0, 127, 20, 70, 0, 5, 5, 32, 50, 2, 0, 0, 0})
        dPKMBaseStats.Add(207, New UInt16() {65, 75, 105, 85, 35, 65, 4, 2, 60, 108, 0, 0, 1, 0, 0, 0, 127, 20, 70, 3, 3, 3, 52, 8, 6, 0, 0, 0})
        dPKMBaseStats.Add(208, New UInt16() {75, 85, 200, 30, 55, 65, 8, 4, 25, 196, 0, 0, 2, 0, 0, 0, 127, 25, 70, 0, 10, 10, 69, 5, 7, 0, 233, 0})
        dPKMBaseStats.Add(209, New UInt16() {60, 80, 50, 30, 40, 40, 0, 0, 190, 63, 0, 1, 0, 0, 0, 0, 191, 20, 70, 4, 5, 6, 22, 50, 9, 0, 0, 0})
        dPKMBaseStats.Add(210, New UInt16() {90, 120, 75, 45, 60, 60, 0, 0, 75, 178, 0, 2, 0, 0, 0, 0, 191, 20, 70, 4, 5, 6, 22, 95, 6, 0, 0, 0})
        dPKMBaseStats.Add(211, New UInt16() {65, 95, 75, 85, 55, 55, 11, 3, 45, 100, 0, 1, 0, 0, 0, 0, 127, 20, 70, 0, 12, 12, 38, 33, 7, 0, 245, 0})
        dPKMBaseStats.Add(212, New UInt16() {70, 130, 100, 65, 55, 80, 6, 8, 25, 200, 0, 2, 0, 0, 0, 0, 127, 25, 70, 0, 3, 3, 68, 101, 0, 0, 0, 0})
        dPKMBaseStats.Add(213, New UInt16() {20, 10, 230, 5, 10, 230, 6, 5, 190, 80, 0, 0, 1, 0, 0, 1, 127, 20, 70, 3, 3, 3, 5, 82, 2, 155, 155, 0})
        dPKMBaseStats.Add(214, New UInt16() {80, 125, 75, 85, 40, 95, 6, 1, 45, 200, 0, 2, 0, 0, 0, 0, 127, 25, 70, 5, 3, 3, 68, 62, 1, 0, 0, 0})
        dPKMBaseStats.Add(215, New UInt16() {55, 95, 55, 115, 35, 75, 17, 15, 60, 132, 0, 0, 0, 1, 0, 0, 127, 20, 35, 3, 5, 5, 39, 51, 4, 286, 217, 0})
        dPKMBaseStats.Add(216, New UInt16() {60, 80, 50, 40, 50, 50, 0, 0, 120, 124, 0, 1, 0, 0, 0, 0, 127, 20, 70, 0, 5, 5, 53, 95, 5, 0, 0, 0})
        dPKMBaseStats.Add(217, New UInt16() {90, 130, 75, 55, 75, 75, 0, 0, 60, 189, 0, 2, 0, 0, 0, 0, 127, 20, 70, 0, 5, 5, 62, 95, 5, 0, 0, 0})
        dPKMBaseStats.Add(218, New UInt16() {40, 40, 40, 20, 70, 40, 10, 10, 190, 78, 0, 0, 0, 0, 1, 0, 127, 20, 70, 0, 11, 11, 40, 49, 0, 0, 0, 0})
        dPKMBaseStats.Add(219, New UInt16() {50, 50, 120, 30, 80, 80, 10, 5, 75, 154, 0, 0, 2, 0, 0, 0, 127, 20, 70, 0, 11, 11, 40, 49, 0, 0, 0, 0})
        dPKMBaseStats.Add(220, New UInt16() {50, 50, 40, 50, 30, 30, 15, 4, 225, 78, 0, 1, 0, 0, 0, 0, 127, 20, 70, 5, 5, 5, 12, 81, 5, 0, 0, 0})
        dPKMBaseStats.Add(221, New UInt16() {100, 100, 80, 50, 60, 60, 15, 4, 75, 160, 1, 1, 0, 0, 0, 0, 127, 20, 70, 5, 5, 5, 12, 81, 5, 0, 0, 0})
        dPKMBaseStats.Add(222, New UInt16() {55, 55, 85, 35, 65, 85, 11, 5, 60, 113, 0, 0, 1, 0, 0, 1, 191, 20, 70, 4, 2, 9, 55, 30, 9, 0, 238, 0})
        dPKMBaseStats.Add(223, New UInt16() {35, 65, 35, 65, 65, 35, 11, 11, 190, 78, 0, 0, 0, 0, 1, 0, 127, 20, 70, 0, 2, 12, 55, 97, 7, 0, 0, 0})
        dPKMBaseStats.Add(224, New UInt16() {75, 105, 75, 45, 105, 75, 11, 11, 75, 164, 0, 1, 0, 0, 1, 0, 127, 20, 70, 0, 2, 12, 21, 97, 0, 0, 0, 0})
        dPKMBaseStats.Add(225, New UInt16() {45, 55, 45, 75, 65, 45, 15, 2, 45, 183, 0, 0, 0, 1, 0, 0, 127, 20, 70, 4, 2, 5, 72, 55, 0, 0, 0, 0})
        dPKMBaseStats.Add(226, New UInt16() {65, 40, 70, 70, 80, 140, 11, 2, 25, 168, 0, 0, 0, 0, 0, 2, 127, 25, 70, 5, 2, 2, 33, 11, 6, 0, 0, 0})
        dPKMBaseStats.Add(227, New UInt16() {65, 80, 140, 70, 40, 70, 8, 2, 25, 168, 0, 0, 2, 0, 0, 0, 127, 25, 70, 5, 4, 4, 51, 5, 7, 0, 0, 0})
        dPKMBaseStats.Add(228, New UInt16() {45, 60, 30, 65, 80, 50, 17, 10, 120, 114, 0, 0, 0, 0, 1, 0, 127, 20, 35, 5, 5, 5, 48, 18, 4, 0, 0, 0})
        dPKMBaseStats.Add(229, New UInt16() {75, 90, 50, 95, 110, 80, 17, 10, 45, 204, 0, 0, 0, 0, 2, 0, 127, 20, 35, 5, 5, 5, 48, 18, 4, 0, 0, 0})
        dPKMBaseStats.Add(230, New UInt16() {75, 95, 95, 85, 95, 95, 11, 16, 45, 207, 0, 1, 0, 0, 1, 1, 127, 20, 70, 0, 2, 14, 33, 97, 1, 0, 235, 0})
        dPKMBaseStats.Add(231, New UInt16() {90, 60, 60, 40, 40, 40, 4, 4, 120, 124, 1, 0, 0, 0, 0, 0, 127, 20, 70, 0, 5, 5, 53, 0, 1, 0, 185, 0})
        dPKMBaseStats.Add(232, New UInt16() {90, 120, 120, 50, 60, 60, 4, 4, 60, 189, 0, 1, 1, 0, 0, 0, 127, 20, 70, 0, 5, 5, 5, 0, 7, 0, 185, 0})
        dPKMBaseStats.Add(233, New UInt16() {85, 80, 90, 60, 105, 95, 0, 0, 45, 180, 0, 0, 0, 0, 2, 0, 255, 20, 70, 0, 10, 10, 36, 88, 0, 0, 0, 0})
        dPKMBaseStats.Add(234, New UInt16() {73, 95, 62, 85, 85, 65, 0, 0, 45, 165, 0, 1, 0, 0, 0, 0, 127, 20, 70, 5, 5, 5, 22, 119, 5, 0, 0, 0})
        dPKMBaseStats.Add(235, New UInt16() {55, 20, 35, 75, 20, 45, 0, 0, 45, 106, 0, 0, 0, 1, 0, 0, 127, 20, 70, 4, 5, 5, 20, 101, 8, 0, 0, 0})
        dPKMBaseStats.Add(236, New UInt16() {35, 35, 35, 35, 35, 35, 1, 1, 75, 91, 0, 1, 0, 0, 0, 0, 0, 25, 70, 0, 15, 15, 62, 80, 6, 0, 0, 0})
        dPKMBaseStats.Add(237, New UInt16() {50, 95, 95, 70, 35, 110, 1, 1, 45, 138, 0, 0, 0, 0, 0, 2, 0, 25, 70, 0, 8, 8, 22, 101, 5, 0, 0, 0})
        dPKMBaseStats.Add(238, New UInt16() {45, 30, 15, 65, 85, 65, 15, 14, 45, 87, 0, 0, 0, 0, 1, 0, 254, 25, 70, 0, 15, 15, 12, 108, 9, 153, 153, 0})
        dPKMBaseStats.Add(239, New UInt16() {45, 63, 37, 95, 65, 55, 13, 13, 45, 106, 0, 0, 0, 1, 0, 0, 63, 25, 70, 0, 15, 15, 9, 0, 2, 322, 0, 0})
        dPKMBaseStats.Add(240, New UInt16() {45, 75, 37, 83, 70, 55, 10, 10, 45, 117, 0, 0, 0, 1, 0, 0, 63, 25, 70, 0, 15, 15, 49, 0, 0, 0, 323, 0})
        dPKMBaseStats.Add(241, New UInt16() {95, 80, 105, 100, 40, 70, 0, 0, 45, 200, 0, 0, 2, 0, 0, 0, 254, 20, 70, 5, 5, 5, 47, 113, 9, 33, 33, 0})
        dPKMBaseStats.Add(242, New UInt16() {255, 10, 10, 55, 75, 135, 0, 0, 30, 255, 3, 0, 0, 0, 0, 0, 254, 40, 140, 4, 6, 6, 30, 32, 9, 110, 231, 0})
        dPKMBaseStats.Add(243, New UInt16() {90, 85, 75, 115, 115, 100, 13, 13, 3, 216, 0, 0, 0, 2, 1, 0, 255, 80, 35, 5, 15, 15, 46, 0, 2, 0, 0, 0})
        dPKMBaseStats.Add(244, New UInt16() {115, 115, 85, 100, 90, 75, 10, 10, 3, 217, 1, 2, 0, 0, 0, 0, 255, 80, 35, 5, 15, 15, 46, 0, 5, 0, 0, 0})
        dPKMBaseStats.Add(245, New UInt16() {100, 75, 115, 85, 90, 115, 11, 11, 3, 215, 0, 0, 1, 0, 0, 2, 255, 80, 35, 5, 15, 15, 46, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(246, New UInt16() {50, 64, 50, 41, 45, 50, 5, 4, 45, 67, 0, 1, 0, 0, 0, 0, 127, 40, 35, 5, 1, 1, 62, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(247, New UInt16() {70, 84, 70, 51, 65, 70, 5, 4, 45, 144, 0, 2, 0, 0, 0, 0, 127, 40, 35, 5, 1, 1, 61, 0, 7, 0, 0, 0})
        dPKMBaseStats.Add(248, New UInt16() {100, 134, 110, 61, 95, 100, 5, 17, 45, 218, 0, 3, 0, 0, 0, 0, 127, 40, 35, 5, 1, 1, 45, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(249, New UInt16() {106, 90, 130, 110, 90, 154, 14, 2, 3, 220, 0, 0, 0, 0, 0, 3, 255, 120, 0, 5, 15, 15, 46, 0, 8, 0, 0, 0})
        dPKMBaseStats.Add(250, New UInt16() {106, 130, 90, 90, 110, 154, 10, 2, 3, 220, 0, 0, 0, 0, 0, 3, 255, 120, 0, 5, 15, 15, 46, 0, 0, 44, 44, 0})
        dPKMBaseStats.Add(251, New UInt16() {100, 100, 100, 100, 100, 100, 14, 12, 45, 64, 3, 0, 0, 0, 0, 0, 255, 120, 100, 3, 15, 15, 30, 0, 3, 157, 157, 0})
        dPKMBaseStats.Add(252, New UInt16() {40, 45, 35, 70, 65, 55, 12, 12, 45, 65, 0, 0, 0, 1, 0, 0, 31, 20, 70, 3, 1, 14, 65, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(253, New UInt16() {50, 65, 45, 95, 85, 65, 12, 12, 45, 141, 0, 0, 0, 2, 0, 0, 31, 20, 70, 3, 1, 14, 65, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(254, New UInt16() {70, 85, 65, 120, 105, 85, 12, 12, 45, 208, 0, 0, 0, 3, 0, 0, 31, 20, 70, 3, 1, 14, 65, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(255, New UInt16() {45, 60, 40, 45, 70, 50, 10, 10, 45, 65, 0, 0, 0, 0, 1, 0, 31, 20, 70, 3, 5, 5, 66, 0, 0, 0, 0, 0})
        dPKMBaseStats.Add(256, New UInt16() {60, 85, 60, 55, 85, 60, 10, 1, 45, 142, 0, 1, 0, 0, 1, 0, 31, 20, 70, 3, 5, 5, 66, 0, 0, 0, 0, 0})
        dPKMBaseStats.Add(257, New UInt16() {80, 120, 70, 80, 110, 70, 10, 1, 45, 209, 0, 3, 0, 0, 0, 0, 31, 20, 70, 3, 5, 5, 66, 0, 0, 0, 0, 0})
        dPKMBaseStats.Add(258, New UInt16() {50, 70, 50, 40, 50, 50, 11, 11, 45, 65, 0, 1, 0, 0, 0, 0, 31, 20, 70, 3, 1, 2, 67, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(259, New UInt16() {70, 85, 70, 50, 60, 70, 11, 4, 45, 143, 0, 2, 0, 0, 0, 0, 31, 20, 70, 3, 1, 2, 67, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(260, New UInt16() {100, 110, 90, 60, 85, 90, 11, 4, 45, 210, 0, 3, 0, 0, 0, 0, 31, 20, 70, 3, 1, 2, 67, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(261, New UInt16() {35, 55, 35, 35, 30, 30, 17, 17, 255, 55, 0, 1, 0, 0, 0, 0, 127, 15, 70, 0, 5, 5, 50, 95, 7, 0, 151, 0})
        dPKMBaseStats.Add(262, New UInt16() {70, 90, 70, 70, 60, 60, 17, 17, 127, 128, 0, 2, 0, 0, 0, 0, 127, 15, 70, 0, 5, 5, 22, 95, 7, 0, 151, 0})
        dPKMBaseStats.Add(263, New UInt16() {38, 30, 41, 60, 30, 41, 0, 0, 255, 60, 0, 0, 0, 1, 0, 0, 127, 15, 70, 0, 5, 5, 53, 82, 5, 0, 155, 0})
        dPKMBaseStats.Add(264, New UInt16() {78, 70, 61, 100, 50, 61, 0, 0, 90, 128, 0, 0, 0, 2, 0, 0, 127, 15, 70, 0, 5, 5, 53, 82, 8, 155, 158, 0})
        dPKMBaseStats.Add(265, New UInt16() {45, 45, 35, 20, 20, 30, 6, 6, 255, 54, 1, 0, 0, 0, 0, 0, 127, 15, 70, 0, 3, 3, 19, 0, 0, 0, 0, 0})
        dPKMBaseStats.Add(266, New UInt16() {50, 35, 55, 15, 25, 25, 6, 6, 120, 72, 0, 0, 2, 0, 0, 0, 127, 15, 70, 0, 3, 3, 61, 0, 8, 0, 0, 0})
        dPKMBaseStats.Add(267, New UInt16() {60, 70, 50, 65, 90, 50, 6, 2, 45, 161, 0, 0, 0, 0, 3, 0, 127, 15, 70, 0, 3, 3, 68, 0, 2, 0, 295, 0})
        dPKMBaseStats.Add(268, New UInt16() {50, 35, 55, 15, 25, 25, 6, 6, 120, 72, 0, 0, 2, 0, 0, 0, 127, 15, 70, 0, 3, 3, 61, 0, 6, 0, 0, 0})
        dPKMBaseStats.Add(269, New UInt16() {60, 50, 70, 65, 50, 90, 6, 3, 45, 161, 0, 0, 0, 0, 0, 3, 127, 15, 70, 0, 3, 3, 19, 0, 3, 0, 295, 0})
        dPKMBaseStats.Add(270, New UInt16() {40, 30, 30, 30, 40, 50, 11, 12, 255, 74, 0, 0, 0, 0, 0, 1, 127, 15, 70, 3, 2, 7, 33, 44, 3, 0, 0, 0})
        dPKMBaseStats.Add(271, New UInt16() {60, 50, 50, 50, 60, 70, 11, 12, 120, 141, 0, 0, 0, 0, 0, 2, 127, 15, 70, 3, 2, 7, 33, 44, 3, 0, 0, 0})
        dPKMBaseStats.Add(272, New UInt16() {80, 70, 70, 70, 90, 100, 11, 12, 45, 181, 0, 0, 0, 0, 0, 3, 127, 15, 70, 3, 2, 7, 33, 44, 3, 0, 0, 0})
        dPKMBaseStats.Add(273, New UInt16() {40, 40, 50, 30, 30, 30, 12, 12, 255, 74, 0, 0, 1, 0, 0, 0, 127, 15, 70, 3, 5, 7, 34, 48, 5, 0, 0, 0})
        dPKMBaseStats.Add(274, New UInt16() {70, 70, 40, 60, 60, 40, 12, 17, 120, 141, 0, 2, 0, 0, 0, 0, 127, 15, 70, 3, 5, 7, 34, 48, 5, 0, 0, 0})
        dPKMBaseStats.Add(275, New UInt16() {90, 100, 60, 80, 90, 60, 12, 17, 45, 181, 0, 3, 0, 0, 0, 0, 127, 15, 70, 3, 5, 7, 34, 48, 5, 0, 0, 0})
        dPKMBaseStats.Add(276, New UInt16() {40, 55, 30, 85, 30, 30, 0, 2, 200, 59, 0, 0, 0, 1, 0, 0, 127, 15, 70, 3, 4, 4, 62, 0, 1, 0, 195, 0})
        dPKMBaseStats.Add(277, New UInt16() {60, 85, 60, 125, 50, 50, 0, 2, 45, 162, 0, 0, 0, 2, 0, 0, 127, 15, 70, 3, 4, 4, 62, 0, 1, 0, 195, 0})
        dPKMBaseStats.Add(278, New UInt16() {40, 30, 30, 85, 55, 30, 11, 2, 190, 64, 0, 0, 0, 1, 0, 0, 127, 20, 70, 0, 2, 4, 51, 0, 8, 0, 0, 0})
        dPKMBaseStats.Add(279, New UInt16() {60, 50, 100, 65, 85, 70, 11, 2, 45, 164, 0, 0, 2, 0, 0, 0, 127, 20, 70, 0, 2, 4, 51, 0, 2, 0, 0, 0})
        dPKMBaseStats.Add(280, New UInt16() {28, 25, 25, 40, 45, 35, 14, 14, 235, 70, 0, 0, 0, 0, 1, 0, 127, 20, 35, 5, 11, 11, 28, 36, 8, 0, 0, 0})
        dPKMBaseStats.Add(281, New UInt16() {38, 35, 35, 50, 65, 55, 14, 14, 120, 140, 0, 0, 0, 0, 2, 0, 127, 20, 35, 5, 11, 11, 28, 36, 8, 0, 0, 0})
        dPKMBaseStats.Add(282, New UInt16() {68, 65, 65, 80, 125, 115, 14, 14, 45, 208, 0, 0, 0, 0, 3, 0, 127, 20, 35, 5, 11, 11, 28, 36, 8, 0, 0, 0})
        dPKMBaseStats.Add(283, New UInt16() {40, 30, 32, 65, 50, 52, 6, 11, 200, 63, 0, 0, 0, 1, 0, 0, 127, 15, 70, 0, 2, 3, 33, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(284, New UInt16() {70, 60, 62, 60, 80, 82, 6, 2, 75, 128, 0, 0, 0, 0, 1, 1, 127, 15, 70, 0, 2, 3, 22, 0, 1, 0, 222, 0})
        dPKMBaseStats.Add(285, New UInt16() {60, 40, 60, 35, 40, 60, 12, 12, 255, 65, 1, 0, 0, 0, 0, 0, 127, 15, 70, 2, 6, 7, 27, 90, 5, 0, 190, 120})
        dPKMBaseStats.Add(286, New UInt16() {60, 130, 80, 70, 60, 60, 12, 1, 90, 165, 0, 2, 0, 0, 0, 0, 127, 15, 70, 2, 6, 7, 27, 90, 3, 0, 190, 0})
        dPKMBaseStats.Add(287, New UInt16() {60, 60, 60, 30, 35, 35, 0, 0, 255, 83, 1, 0, 0, 0, 0, 0, 127, 15, 70, 5, 5, 5, 54, 0, 5, 0, 0, 0})
        dPKMBaseStats.Add(288, New UInt16() {80, 80, 80, 90, 55, 55, 0, 0, 120, 126, 0, 0, 0, 2, 0, 0, 127, 15, 70, 5, 5, 5, 72, 0, 8, 0, 0, 0})
        dPKMBaseStats.Add(289, New UInt16() {150, 160, 100, 100, 95, 65, 0, 0, 45, 210, 3, 0, 0, 0, 0, 0, 127, 15, 70, 5, 5, 5, 54, 0, 5, 0, 0, 0})
        dPKMBaseStats.Add(290, New UInt16() {31, 45, 90, 40, 30, 30, 6, 4, 255, 65, 0, 0, 1, 0, 0, 0, 127, 15, 70, 1, 3, 3, 14, 0, 7, 0, 0, 0})
        dPKMBaseStats.Add(291, New UInt16() {61, 90, 45, 160, 50, 50, 6, 2, 120, 155, 0, 0, 0, 2, 0, 0, 127, 15, 70, 1, 3, 3, 3, 0, 2, 0, 0, 0})
        dPKMBaseStats.Add(292, New UInt16() {1, 90, 45, 40, 30, 30, 6, 7, 45, 95, 2, 0, 0, 0, 0, 0, 255, 15, 70, 1, 10, 10, 25, 0, 5, 0, 0, 0})
        dPKMBaseStats.Add(293, New UInt16() {64, 51, 23, 28, 51, 23, 0, 0, 190, 68, 1, 0, 0, 0, 0, 0, 127, 20, 70, 3, 1, 5, 43, 0, 9, 0, 150, 0})
        dPKMBaseStats.Add(294, New UInt16() {84, 71, 43, 48, 71, 43, 0, 0, 120, 126, 2, 0, 0, 0, 0, 0, 127, 20, 70, 3, 1, 5, 43, 0, 1, 0, 150, 0})
        dPKMBaseStats.Add(295, New UInt16() {104, 91, 63, 68, 91, 63, 0, 0, 45, 184, 3, 0, 0, 0, 0, 0, 127, 20, 70, 3, 1, 5, 43, 0, 1, 0, 150, 0})
        dPKMBaseStats.Add(296, New UInt16() {72, 60, 30, 25, 20, 30, 1, 1, 180, 87, 1, 0, 0, 0, 0, 0, 63, 20, 70, 2, 8, 8, 47, 62, 2, 0, 0, 0})
        dPKMBaseStats.Add(297, New UInt16() {144, 120, 60, 50, 40, 60, 1, 1, 200, 184, 2, 0, 0, 0, 0, 0, 63, 20, 70, 2, 8, 8, 47, 62, 5, 0, 221, 0})
        dPKMBaseStats.Add(298, New UInt16() {50, 20, 40, 20, 20, 40, 0, 0, 150, 33, 1, 0, 0, 0, 0, 0, 191, 10, 70, 4, 15, 15, 47, 37, 1, 0, 0, 120})
        dPKMBaseStats.Add(299, New UInt16() {30, 45, 135, 30, 45, 90, 5, 5, 255, 108, 0, 0, 1, 0, 0, 0, 127, 20, 70, 0, 10, 10, 5, 42, 7, 0, 238, 0})
        dPKMBaseStats.Add(300, New UInt16() {50, 45, 45, 50, 35, 35, 0, 0, 255, 65, 0, 0, 0, 1, 0, 0, 191, 15, 70, 4, 5, 6, 56, 96, 9, 0, 154, 0})
        dPKMBaseStats.Add(301, New UInt16() {70, 65, 65, 70, 55, 55, 0, 0, 60, 138, 1, 0, 0, 1, 0, 0, 191, 15, 70, 4, 5, 6, 56, 96, 6, 0, 154, 0})
        dPKMBaseStats.Add(302, New UInt16() {50, 75, 75, 50, 65, 65, 17, 7, 45, 98, 0, 1, 1, 0, 0, 0, 127, 25, 35, 3, 8, 8, 51, 100, 6, 0, 0, 0})
        dPKMBaseStats.Add(303, New UInt16() {50, 85, 85, 50, 55, 55, 8, 8, 45, 98, 0, 1, 1, 0, 0, 0, 127, 20, 70, 4, 5, 6, 52, 22, 4, 0, 184, 0})
        dPKMBaseStats.Add(304, New UInt16() {50, 70, 100, 30, 40, 40, 8, 5, 180, 96, 0, 0, 1, 0, 0, 0, 127, 35, 35, 5, 1, 1, 5, 69, 7, 0, 238, 0})
        dPKMBaseStats.Add(305, New UInt16() {60, 90, 140, 40, 50, 50, 8, 5, 90, 152, 0, 0, 2, 0, 0, 0, 127, 35, 35, 5, 1, 1, 5, 69, 7, 0, 238, 0})
        dPKMBaseStats.Add(306, New UInt16() {70, 110, 180, 50, 60, 60, 8, 5, 45, 205, 0, 0, 3, 0, 0, 0, 127, 35, 35, 5, 1, 1, 5, 69, 7, 0, 238, 0})
        dPKMBaseStats.Add(307, New UInt16() {30, 40, 55, 60, 40, 55, 1, 14, 180, 91, 0, 0, 0, 1, 0, 0, 127, 20, 70, 0, 8, 8, 74, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(308, New UInt16() {60, 60, 75, 80, 60, 75, 1, 14, 90, 153, 0, 0, 0, 2, 0, 0, 127, 20, 70, 0, 8, 8, 74, 0, 0, 0, 0, 0})
        dPKMBaseStats.Add(309, New UInt16() {40, 45, 40, 65, 65, 40, 13, 13, 120, 104, 0, 0, 0, 1, 0, 0, 127, 20, 70, 5, 5, 5, 9, 31, 3, 0, 0, 0})
        dPKMBaseStats.Add(310, New UInt16() {70, 75, 60, 105, 105, 60, 13, 13, 45, 168, 0, 0, 0, 2, 0, 0, 127, 20, 70, 5, 5, 5, 9, 31, 2, 0, 0, 0})
        dPKMBaseStats.Add(311, New UInt16() {60, 50, 40, 95, 85, 75, 13, 13, 200, 120, 0, 0, 0, 1, 0, 0, 127, 20, 70, 0, 6, 6, 57, 0, 2, 0, 0, 0})
        dPKMBaseStats.Add(312, New UInt16() {60, 40, 50, 95, 75, 85, 13, 13, 200, 120, 0, 0, 0, 1, 0, 0, 127, 20, 70, 0, 6, 6, 58, 0, 2, 0, 0, 0})
        dPKMBaseStats.Add(313, New UInt16() {65, 73, 55, 85, 47, 75, 6, 6, 150, 146, 0, 0, 0, 1, 0, 0, 0, 15, 70, 1, 3, 8, 35, 68, 7, 0, 0, 0})
        dPKMBaseStats.Add(314, New UInt16() {65, 47, 55, 85, 73, 75, 6, 6, 150, 146, 0, 0, 0, 1, 0, 0, 254, 15, 70, 2, 3, 8, 12, 110, 6, 0, 0, 0})
        dPKMBaseStats.Add(315, New UInt16() {50, 60, 45, 65, 100, 80, 12, 3, 150, 152, 0, 0, 0, 0, 2, 0, 127, 20, 70, 3, 6, 7, 30, 38, 3, 0, 245, 60})
        dPKMBaseStats.Add(316, New UInt16() {70, 43, 53, 40, 43, 53, 3, 3, 225, 75, 1, 0, 0, 0, 0, 0, 127, 20, 70, 2, 11, 11, 64, 60, 3, 0, 89, 120})
        dPKMBaseStats.Add(317, New UInt16() {100, 73, 83, 55, 73, 83, 3, 3, 75, 168, 2, 0, 0, 0, 0, 0, 127, 20, 70, 2, 11, 11, 64, 60, 6, 0, 89, 0})
        dPKMBaseStats.Add(318, New UInt16() {45, 90, 20, 65, 65, 20, 11, 17, 225, 88, 0, 1, 0, 0, 0, 0, 127, 20, 35, 5, 12, 12, 24, 0, 0, 0, 226, 90})
        dPKMBaseStats.Add(319, New UInt16() {70, 120, 40, 95, 95, 40, 11, 17, 60, 175, 0, 2, 0, 0, 0, 0, 127, 20, 35, 5, 12, 12, 24, 0, 1, 0, 226, 0})
        dPKMBaseStats.Add(320, New UInt16() {130, 70, 35, 60, 70, 35, 11, 11, 125, 137, 1, 0, 0, 0, 0, 0, 127, 40, 70, 2, 5, 12, 41, 12, 1, 0, 0, 0})
        dPKMBaseStats.Add(321, New UInt16() {170, 90, 45, 60, 90, 45, 11, 11, 60, 206, 2, 0, 0, 0, 0, 0, 127, 40, 70, 2, 5, 12, 41, 12, 1, 0, 0, 0})
        dPKMBaseStats.Add(322, New UInt16() {60, 60, 40, 35, 65, 45, 10, 4, 255, 88, 0, 0, 0, 0, 1, 0, 127, 20, 70, 0, 5, 5, 12, 86, 2, 152, 152, 0})
        dPKMBaseStats.Add(323, New UInt16() {70, 100, 70, 40, 105, 75, 10, 4, 150, 175, 0, 1, 0, 0, 1, 0, 127, 20, 70, 0, 5, 5, 40, 116, 0, 152, 152, 0})
        dPKMBaseStats.Add(324, New UInt16() {70, 85, 140, 20, 85, 70, 10, 10, 90, 161, 0, 0, 2, 0, 0, 0, 127, 20, 70, 0, 5, 5, 73, 0, 5, 0, 0, 0})
        dPKMBaseStats.Add(325, New UInt16() {60, 25, 35, 60, 70, 80, 14, 14, 255, 89, 0, 0, 0, 0, 0, 1, 127, 20, 70, 4, 5, 5, 47, 20, 4, 0, 194, 0})
        dPKMBaseStats.Add(326, New UInt16() {80, 45, 65, 80, 90, 110, 14, 14, 60, 164, 0, 0, 0, 0, 0, 2, 127, 20, 70, 4, 5, 5, 47, 20, 6, 0, 194, 0})
        dPKMBaseStats.Add(327, New UInt16() {60, 60, 60, 60, 60, 60, 0, 0, 255, 85, 0, 0, 0, 0, 1, 0, 127, 15, 70, 4, 5, 8, 20, 77, 5, 0, 150, 0})
        dPKMBaseStats.Add(328, New UInt16() {45, 100, 45, 10, 45, 45, 4, 4, 255, 73, 0, 1, 0, 0, 0, 0, 127, 20, 70, 3, 3, 3, 52, 71, 5, 0, 237, 0})
        dPKMBaseStats.Add(329, New UInt16() {50, 70, 50, 70, 50, 50, 4, 16, 120, 126, 0, 1, 0, 1, 0, 0, 127, 20, 70, 3, 3, 3, 26, 26, 3, 0, 0, 0})
        dPKMBaseStats.Add(330, New UInt16() {80, 100, 80, 100, 80, 80, 4, 16, 45, 197, 0, 1, 0, 2, 0, 0, 127, 20, 70, 3, 3, 3, 26, 26, 3, 0, 0, 0})
        dPKMBaseStats.Add(331, New UInt16() {50, 85, 40, 35, 85, 40, 12, 12, 190, 97, 0, 0, 0, 0, 1, 0, 127, 20, 35, 3, 7, 8, 8, 0, 3, 0, 288, 0})
        dPKMBaseStats.Add(332, New UInt16() {70, 115, 60, 55, 115, 60, 12, 17, 60, 177, 0, 1, 0, 0, 1, 0, 127, 20, 35, 3, 7, 8, 8, 0, 3, 0, 288, 0})
        dPKMBaseStats.Add(333, New UInt16() {45, 40, 60, 50, 40, 75, 0, 2, 255, 74, 0, 0, 0, 0, 0, 1, 127, 20, 70, 1, 4, 14, 30, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(334, New UInt16() {75, 70, 90, 80, 70, 105, 16, 2, 45, 188, 0, 0, 0, 0, 0, 2, 127, 20, 70, 1, 4, 14, 30, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(335, New UInt16() {73, 115, 60, 90, 60, 60, 0, 0, 90, 165, 0, 2, 0, 0, 0, 0, 127, 20, 70, 1, 5, 5, 17, 0, 8, 0, 217, 0})
        dPKMBaseStats.Add(336, New UInt16() {73, 100, 60, 65, 100, 60, 3, 3, 90, 165, 0, 1, 0, 0, 1, 0, 127, 20, 70, 2, 5, 14, 61, 0, 4, 0, 0, 0})
        dPKMBaseStats.Add(337, New UInt16() {70, 55, 65, 70, 95, 85, 5, 14, 45, 150, 0, 0, 0, 0, 2, 0, 255, 25, 70, 4, 10, 10, 26, 0, 2, 0, 81, 0})
        dPKMBaseStats.Add(338, New UInt16() {70, 95, 85, 70, 55, 65, 5, 14, 45, 150, 0, 2, 0, 0, 0, 0, 255, 25, 70, 4, 10, 10, 26, 0, 0, 0, 80, 0})
        dPKMBaseStats.Add(339, New UInt16() {50, 48, 43, 60, 46, 41, 11, 4, 190, 92, 1, 0, 0, 0, 0, 0, 127, 20, 70, 0, 12, 12, 12, 107, 7, 0, 0, 120})
        dPKMBaseStats.Add(340, New UInt16() {110, 78, 73, 60, 76, 71, 11, 4, 75, 158, 2, 0, 0, 0, 0, 0, 127, 20, 70, 0, 12, 12, 12, 107, 1, 0, 0, 0})
        dPKMBaseStats.Add(341, New UInt16() {43, 80, 65, 35, 50, 35, 11, 11, 205, 111, 0, 1, 0, 0, 0, 0, 127, 15, 70, 2, 2, 9, 52, 75, 0, 0, 0, 0})
        dPKMBaseStats.Add(342, New UInt16() {63, 120, 85, 55, 90, 55, 11, 17, 155, 161, 0, 2, 0, 0, 0, 0, 127, 15, 70, 2, 2, 9, 52, 75, 0, 0, 0, 0})
        dPKMBaseStats.Add(343, New UInt16() {40, 40, 55, 55, 40, 70, 4, 14, 255, 58, 0, 0, 0, 0, 0, 1, 255, 20, 70, 0, 10, 10, 26, 0, 5, 0, 0, 0})
        dPKMBaseStats.Add(344, New UInt16() {60, 70, 105, 75, 70, 120, 4, 14, 90, 189, 0, 0, 0, 0, 0, 2, 255, 20, 70, 0, 10, 10, 26, 0, 4, 0, 0, 0})
        dPKMBaseStats.Add(345, New UInt16() {66, 41, 77, 23, 61, 87, 5, 12, 45, 99, 0, 0, 0, 0, 0, 1, 31, 30, 70, 1, 9, 9, 21, 0, 6, 0, 296, 0})
        dPKMBaseStats.Add(346, New UInt16() {86, 81, 97, 43, 81, 107, 5, 12, 45, 199, 0, 0, 0, 0, 0, 2, 31, 30, 70, 1, 9, 9, 21, 0, 3, 0, 296, 0})
        dPKMBaseStats.Add(347, New UInt16() {45, 95, 50, 75, 40, 50, 5, 6, 45, 99, 0, 1, 0, 0, 0, 0, 31, 30, 70, 1, 9, 9, 4, 0, 7, 0, 0, 0})
        dPKMBaseStats.Add(348, New UInt16() {75, 125, 100, 45, 70, 80, 5, 6, 45, 199, 0, 2, 0, 0, 0, 0, 31, 30, 70, 1, 9, 9, 4, 0, 7, 0, 0, 0})
        dPKMBaseStats.Add(349, New UInt16() {20, 15, 20, 80, 10, 55, 11, 11, 255, 61, 0, 0, 0, 1, 0, 0, 127, 20, 70, 1, 2, 14, 33, 0, 5, 0, 0, 0})
        dPKMBaseStats.Add(350, New UInt16() {95, 60, 79, 81, 100, 125, 11, 11, 60, 213, 0, 0, 0, 0, 0, 2, 127, 20, 70, 1, 2, 14, 63, 0, 9, 0, 0, 0})
        dPKMBaseStats.Add(351, New UInt16() {70, 70, 70, 70, 70, 70, 0, 0, 45, 145, 1, 0, 0, 0, 0, 0, 127, 25, 70, 0, 6, 11, 59, 0, 8, 243, 243, 0})
        dPKMBaseStats.Add(352, New UInt16() {60, 90, 70, 40, 60, 120, 0, 0, 200, 132, 0, 0, 0, 0, 0, 1, 127, 20, 70, 3, 5, 5, 16, 0, 3, 0, 156, 0})
        dPKMBaseStats.Add(353, New UInt16() {44, 75, 35, 45, 63, 33, 7, 7, 225, 97, 0, 1, 0, 0, 0, 0, 127, 25, 35, 4, 11, 11, 15, 119, 4, 0, 247, 0})
        dPKMBaseStats.Add(354, New UInt16() {64, 115, 65, 65, 83, 63, 7, 7, 45, 179, 0, 2, 0, 0, 0, 0, 127, 25, 35, 4, 11, 11, 15, 119, 4, 0, 247, 0})
        dPKMBaseStats.Add(355, New UInt16() {20, 40, 90, 25, 30, 90, 7, 7, 190, 97, 0, 0, 0, 0, 0, 1, 127, 25, 35, 4, 11, 11, 26, 0, 4, 0, 196, 0})
        dPKMBaseStats.Add(356, New UInt16() {40, 70, 130, 25, 60, 130, 7, 7, 90, 179, 0, 0, 1, 0, 0, 1, 127, 25, 35, 4, 11, 11, 46, 0, 4, 0, 196, 0})
        dPKMBaseStats.Add(357, New UInt16() {99, 68, 83, 51, 72, 87, 12, 2, 200, 169, 2, 0, 0, 0, 0, 0, 127, 25, 70, 5, 1, 7, 34, 94, 3, 0, 0, 0})
        dPKMBaseStats.Add(358, New UInt16() {65, 50, 70, 65, 95, 80, 14, 14, 45, 147, 0, 0, 0, 0, 1, 1, 127, 25, 70, 4, 11, 11, 26, 0, 1, 0, 198, 0})
        dPKMBaseStats.Add(359, New UInt16() {65, 130, 60, 75, 75, 60, 17, 17, 30, 174, 0, 2, 0, 0, 0, 0, 127, 25, 35, 3, 5, 5, 46, 105, 8, 0, 0, 0})
        dPKMBaseStats.Add(360, New UInt16() {95, 23, 48, 23, 23, 48, 14, 14, 125, 44, 1, 0, 0, 0, 0, 0, 127, 20, 70, 0, 15, 15, 23, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(361, New UInt16() {50, 50, 50, 50, 50, 50, 15, 15, 190, 74, 1, 0, 0, 0, 0, 0, 127, 20, 70, 0, 6, 10, 39, 115, 7, 0, 199, 0})
        dPKMBaseStats.Add(362, New UInt16() {80, 80, 80, 80, 80, 80, 15, 15, 75, 187, 2, 0, 0, 0, 0, 0, 127, 20, 70, 0, 6, 10, 39, 115, 7, 0, 199, 0})
        dPKMBaseStats.Add(363, New UInt16() {70, 40, 50, 25, 55, 50, 15, 11, 255, 75, 1, 0, 0, 0, 0, 0, 127, 20, 70, 3, 2, 5, 47, 115, 1, 0, 0, 0})
        dPKMBaseStats.Add(364, New UInt16() {90, 60, 70, 45, 75, 70, 15, 11, 120, 128, 2, 0, 0, 0, 0, 0, 127, 20, 70, 3, 2, 5, 47, 115, 1, 0, 0, 0})
        dPKMBaseStats.Add(365, New UInt16() {110, 80, 90, 65, 95, 90, 15, 11, 45, 192, 3, 0, 0, 0, 0, 0, 127, 20, 70, 3, 2, 5, 47, 115, 1, 0, 0, 0})
        dPKMBaseStats.Add(366, New UInt16() {35, 64, 85, 32, 74, 55, 11, 11, 255, 142, 0, 0, 1, 0, 0, 0, 127, 20, 70, 1, 2, 2, 75, 0, 1, 0, 89, 0})
        dPKMBaseStats.Add(367, New UInt16() {55, 104, 105, 52, 94, 75, 11, 11, 60, 178, 0, 1, 1, 0, 0, 0, 127, 20, 70, 1, 2, 2, 33, 0, 1, 0, 226, 0})
        dPKMBaseStats.Add(368, New UInt16() {55, 84, 105, 52, 114, 75, 11, 11, 60, 178, 0, 0, 0, 0, 2, 0, 127, 20, 70, 1, 2, 2, 33, 0, 9, 0, 227, 0})
        dPKMBaseStats.Add(369, New UInt16() {100, 90, 130, 55, 45, 65, 11, 5, 25, 198, 1, 0, 1, 0, 0, 0, 31, 40, 70, 5, 2, 12, 33, 69, 7, 0, 227, 0})
        dPKMBaseStats.Add(370, New UInt16() {43, 30, 55, 97, 40, 65, 11, 11, 225, 110, 0, 0, 0, 1, 0, 0, 191, 20, 70, 4, 12, 12, 33, 0, 9, 93, 0, 0})
        dPKMBaseStats.Add(371, New UInt16() {45, 75, 60, 50, 40, 30, 16, 16, 45, 89, 0, 1, 0, 0, 0, 0, 127, 40, 35, 5, 14, 14, 69, 0, 1, 0, 250, 0})
        dPKMBaseStats.Add(372, New UInt16() {65, 95, 100, 50, 60, 50, 16, 16, 45, 144, 0, 0, 2, 0, 0, 0, 127, 40, 35, 5, 14, 14, 69, 0, 8, 0, 250, 0})
        dPKMBaseStats.Add(373, New UInt16() {95, 135, 80, 100, 110, 80, 16, 2, 45, 218, 0, 3, 0, 0, 0, 0, 127, 40, 35, 5, 14, 14, 22, 0, 1, 0, 250, 0})
        dPKMBaseStats.Add(374, New UInt16() {40, 55, 80, 30, 35, 60, 8, 14, 3, 103, 0, 0, 1, 0, 0, 0, 255, 40, 35, 5, 10, 10, 29, 0, 1, 0, 233, 0})
        dPKMBaseStats.Add(375, New UInt16() {60, 75, 100, 50, 55, 80, 8, 14, 3, 153, 0, 0, 2, 0, 0, 0, 255, 40, 35, 5, 10, 10, 29, 0, 1, 0, 233, 0})
        dPKMBaseStats.Add(376, New UInt16() {80, 135, 130, 70, 95, 90, 8, 14, 3, 210, 0, 0, 3, 0, 0, 0, 255, 40, 35, 5, 10, 10, 29, 0, 1, 0, 233, 0})
        dPKMBaseStats.Add(377, New UInt16() {80, 100, 200, 50, 50, 100, 5, 5, 3, 217, 0, 0, 3, 0, 0, 0, 255, 80, 35, 5, 15, 15, 29, 0, 5, 0, 0, 0})
        dPKMBaseStats.Add(378, New UInt16() {80, 50, 100, 50, 100, 200, 15, 15, 3, 216, 0, 0, 0, 0, 0, 3, 255, 80, 35, 5, 15, 15, 29, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(379, New UInt16() {80, 75, 150, 50, 75, 150, 8, 8, 3, 215, 0, 0, 2, 0, 0, 1, 255, 80, 35, 5, 15, 15, 29, 0, 7, 0, 0, 0})
        dPKMBaseStats.Add(380, New UInt16() {80, 80, 90, 110, 110, 130, 16, 14, 3, 211, 0, 0, 0, 0, 0, 3, 254, 120, 90, 5, 15, 15, 26, 0, 0, 0, 0, 0})
        dPKMBaseStats.Add(381, New UInt16() {80, 90, 80, 110, 130, 110, 16, 14, 3, 211, 0, 0, 0, 0, 3, 0, 0, 120, 90, 5, 15, 15, 26, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(382, New UInt16() {100, 100, 90, 90, 150, 140, 11, 11, 5, 218, 0, 0, 0, 0, 3, 0, 255, 120, 0, 5, 15, 15, 2, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(383, New UInt16() {100, 150, 140, 90, 100, 90, 4, 4, 5, 218, 0, 3, 0, 0, 0, 0, 255, 120, 0, 5, 15, 15, 70, 0, 0, 0, 0, 0})
        dPKMBaseStats.Add(384, New UInt16() {105, 150, 90, 95, 150, 90, 16, 2, 3, 220, 0, 2, 0, 0, 1, 0, 255, 120, 0, 5, 15, 15, 76, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(385, New UInt16() {100, 100, 100, 100, 100, 100, 8, 14, 3, 215, 3, 0, 0, 0, 0, 0, 255, 120, 100, 5, 15, 15, 32, 0, 2, 91, 91, 0})
        dPKMBaseStats.Add(386, New UInt16() {50, 150, 50, 150, 150, 50, 14, 14, 3, 215, 0, 1, 0, 1, 1, 0, 255, 120, 0, 5, 15, 15, 46, 0, 0, 0, 0, 0})
        dPKMBaseStats.Add(387, New UInt16() {55, 68, 64, 31, 45, 55, 12, 12, 45, 64, 0, 1, 0, 0, 0, 0, 31, 20, 70, 3, 1, 7, 65, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(388, New UInt16() {75, 89, 85, 36, 55, 65, 12, 12, 45, 141, 0, 1, 1, 0, 0, 0, 31, 20, 70, 3, 1, 7, 65, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(389, New UInt16() {95, 109, 105, 56, 75, 85, 12, 4, 45, 208, 0, 2, 1, 0, 0, 0, 31, 20, 70, 3, 1, 7, 65, 0, 3, 0, 0, 0})
        dPKMBaseStats.Add(390, New UInt16() {44, 58, 44, 61, 58, 44, 10, 10, 45, 65, 0, 0, 0, 1, 0, 0, 31, 20, 70, 3, 5, 8, 66, 0, 5, 0, 0, 0})
        dPKMBaseStats.Add(391, New UInt16() {64, 78, 52, 81, 78, 52, 10, 1, 45, 142, 0, 0, 0, 1, 1, 0, 31, 20, 70, 3, 5, 8, 66, 0, 5, 0, 0, 0})
        dPKMBaseStats.Add(392, New UInt16() {76, 104, 71, 108, 104, 71, 10, 1, 45, 209, 0, 1, 0, 1, 1, 0, 31, 20, 70, 3, 5, 8, 66, 0, 5, 0, 0, 0})
        dPKMBaseStats.Add(393, New UInt16() {53, 51, 53, 40, 61, 56, 11, 11, 45, 66, 0, 0, 0, 0, 1, 0, 31, 20, 70, 3, 2, 5, 67, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(394, New UInt16() {64, 66, 68, 50, 81, 76, 11, 11, 45, 143, 0, 0, 0, 0, 2, 0, 31, 20, 70, 3, 2, 5, 67, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(395, New UInt16() {84, 86, 88, 60, 111, 101, 11, 8, 45, 210, 0, 0, 0, 0, 3, 0, 31, 20, 70, 3, 2, 5, 67, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(396, New UInt16() {40, 55, 30, 60, 30, 30, 0, 2, 255, 56, 0, 0, 0, 1, 0, 0, 127, 15, 70, 3, 4, 4, 51, 0, 5, 0, 188, 90})
        dPKMBaseStats.Add(397, New UInt16() {55, 75, 50, 80, 40, 40, 0, 2, 120, 113, 0, 0, 0, 2, 0, 0, 127, 15, 70, 3, 4, 4, 22, 0, 5, 0, 188, 60})
        dPKMBaseStats.Add(398, New UInt16() {85, 120, 70, 100, 50, 50, 0, 2, 45, 172, 0, 3, 0, 0, 0, 0, 127, 15, 70, 3, 4, 4, 22, 0, 5, 0, 188, 0})
        dPKMBaseStats.Add(399, New UInt16() {59, 45, 40, 31, 35, 40, 0, 0, 255, 58, 1, 0, 0, 0, 0, 0, 127, 15, 70, 0, 2, 5, 86, 109, 5, 0, 0, 90})
        dPKMBaseStats.Add(400, New UInt16() {79, 85, 60, 71, 55, 60, 0, 11, 127, 116, 0, 2, 0, 0, 0, 0, 127, 15, 70, 0, 2, 5, 86, 109, 5, 155, 158, 60})
        dPKMBaseStats.Add(401, New UInt16() {37, 25, 41, 25, 25, 41, 6, 6, 255, 54, 0, 0, 1, 0, 0, 0, 127, 15, 70, 3, 3, 3, 61, 0, 0, 0, 277, 0})
        dPKMBaseStats.Add(402, New UInt16() {77, 85, 51, 65, 55, 51, 6, 6, 45, 159, 0, 2, 0, 0, 0, 0, 127, 15, 70, 3, 3, 3, 68, 0, 0, 0, 277, 0})
        dPKMBaseStats.Add(403, New UInt16() {45, 65, 34, 45, 40, 34, 13, 13, 235, 60, 0, 1, 0, 0, 0, 0, 127, 20, 70, 3, 5, 5, 79, 22, 1, 0, 0, 0})
        dPKMBaseStats.Add(404, New UInt16() {60, 85, 49, 60, 60, 49, 13, 13, 120, 117, 0, 2, 0, 0, 0, 0, 127, 20, 100, 3, 5, 5, 79, 22, 1, 0, 0, 0})
        dPKMBaseStats.Add(405, New UInt16() {80, 120, 79, 70, 95, 79, 13, 13, 45, 194, 0, 3, 0, 0, 0, 0, 127, 20, 70, 3, 5, 5, 79, 22, 1, 0, 0, 0})
        dPKMBaseStats.Add(406, New UInt16() {40, 30, 35, 55, 50, 70, 12, 3, 255, 68, 0, 0, 0, 0, 1, 0, 127, 20, 70, 3, 15, 15, 30, 38, 3, 0, 245, 120})
        dPKMBaseStats.Add(407, New UInt16() {60, 70, 55, 90, 125, 105, 12, 3, 75, 204, 0, 0, 0, 0, 3, 0, 127, 20, 70, 3, 6, 7, 30, 38, 3, 0, 245, 0})
        dPKMBaseStats.Add(408, New UInt16() {67, 125, 40, 58, 30, 30, 5, 5, 45, 99, 0, 1, 0, 0, 0, 0, 31, 30, 70, 1, 1, 1, 104, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(409, New UInt16() {97, 165, 60, 58, 65, 50, 5, 5, 45, 199, 0, 2, 0, 0, 0, 0, 31, 30, 70, 1, 1, 1, 104, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(410, New UInt16() {30, 42, 118, 30, 42, 88, 5, 8, 45, 99, 0, 0, 1, 0, 0, 0, 31, 30, 70, 1, 1, 1, 5, 0, 7, 0, 0, 0})
        dPKMBaseStats.Add(411, New UInt16() {60, 52, 168, 30, 47, 138, 5, 8, 45, 199, 0, 0, 2, 0, 0, 0, 31, 30, 70, 1, 1, 1, 5, 0, 7, 0, 0, 0})
        dPKMBaseStats.Add(412, New UInt16() {40, 29, 45, 36, 29, 45, 6, 6, 120, 61, 0, 0, 0, 0, 0, 1, 127, 15, 70, 0, 3, 3, 61, 0, 7, 0, 0, 0})
        dPKMBaseStats.Add(413, New UInt16() {60, 59, 85, 36, 79, 105, 6, 12, 45, 159, 0, 0, 0, 0, 0, 2, 254, 15, 70, 0, 3, 3, 107, 0, 7, 0, 222, 0})
        dPKMBaseStats.Add(414, New UInt16() {70, 94, 50, 66, 94, 50, 6, 2, 45, 159, 0, 1, 0, 0, 1, 0, 0, 15, 70, 0, 3, 3, 68, 0, 2, 0, 222, 0})
        dPKMBaseStats.Add(415, New UInt16() {30, 30, 42, 70, 30, 42, 6, 2, 120, 63, 0, 0, 0, 1, 0, 0, 31, 15, 70, 3, 3, 3, 118, 0, 2, 94, 94, 0})
        dPKMBaseStats.Add(416, New UInt16() {70, 80, 102, 40, 80, 102, 6, 2, 45, 188, 0, 0, 1, 0, 0, 1, 254, 15, 70, 3, 3, 3, 46, 0, 2, 0, 245, 0})
        dPKMBaseStats.Add(417, New UInt16() {60, 45, 70, 95, 45, 90, 13, 13, 200, 120, 0, 0, 0, 1, 0, 0, 127, 10, 100, 0, 5, 6, 50, 53, 8, 0, 0, 0})
        dPKMBaseStats.Add(418, New UInt16() {55, 65, 35, 85, 60, 30, 11, 11, 190, 75, 0, 0, 0, 1, 0, 0, 127, 20, 70, 0, 2, 5, 33, 0, 5, 0, 186, 0})
        dPKMBaseStats.Add(419, New UInt16() {85, 105, 55, 115, 85, 50, 11, 11, 75, 178, 0, 0, 0, 2, 0, 0, 127, 20, 70, 0, 2, 5, 33, 0, 5, 0, 186, 0})
        dPKMBaseStats.Add(420, New UInt16() {45, 35, 45, 35, 62, 53, 12, 12, 190, 68, 0, 0, 0, 0, 1, 0, 127, 20, 70, 0, 6, 7, 34, 0, 9, 0, 239, 0})
        dPKMBaseStats.Add(421, New UInt16() {70, 60, 70, 85, 87, 78, 12, 12, 75, 133, 0, 0, 0, 0, 2, 0, 127, 20, 70, 0, 6, 7, 122, 0, 9, 0, 239, 0})
        dPKMBaseStats.Add(422, New UInt16() {76, 48, 48, 34, 57, 62, 11, 11, 190, 73, 1, 0, 0, 0, 0, 0, 127, 20, 70, 0, 2, 11, 60, 114, 6, 0, 0, 0})
        dPKMBaseStats.Add(423, New UInt16() {111, 83, 68, 39, 92, 82, 11, 4, 75, 176, 2, 0, 0, 0, 0, 0, 127, 20, 70, 0, 2, 11, 60, 114, 6, 0, 0, 0})
        dPKMBaseStats.Add(424, New UInt16() {75, 100, 66, 115, 60, 66, 0, 0, 45, 186, 0, 0, 0, 2, 0, 0, 127, 20, 100, 4, 5, 5, 101, 53, 6, 0, 0, 0})
        dPKMBaseStats.Add(425, New UInt16() {90, 50, 34, 70, 60, 44, 7, 2, 125, 127, 1, 0, 0, 0, 0, 0, 127, 30, 70, 2, 11, 11, 106, 84, 6, 0, 0, 0})
        dPKMBaseStats.Add(426, New UInt16() {150, 80, 44, 80, 90, 54, 7, 2, 60, 204, 2, 0, 0, 0, 0, 0, 127, 30, 70, 2, 11, 11, 106, 84, 6, 0, 0, 0})
        dPKMBaseStats.Add(427, New UInt16() {55, 66, 44, 85, 44, 56, 0, 0, 190, 84, 0, 0, 0, 1, 0, 0, 127, 20, 0, 0, 5, 8, 50, 103, 5, 0, 189, 0})
        dPKMBaseStats.Add(428, New UInt16() {65, 76, 84, 105, 54, 96, 0, 0, 60, 178, 0, 0, 0, 2, 0, 0, 127, 20, 140, 0, 5, 8, 56, 103, 5, 0, 189, 0})
        dPKMBaseStats.Add(429, New UInt16() {60, 60, 60, 105, 105, 105, 7, 7, 45, 187, 0, 0, 0, 0, 1, 1, 127, 25, 35, 4, 11, 11, 26, 0, 6, 0, 0, 0})
        dPKMBaseStats.Add(430, New UInt16() {100, 125, 52, 71, 105, 52, 17, 2, 30, 187, 0, 2, 0, 0, 0, 0, 127, 20, 35, 3, 4, 4, 15, 105, 4, 0, 0, 0})
        dPKMBaseStats.Add(431, New UInt16() {49, 55, 42, 85, 42, 37, 0, 0, 190, 71, 0, 0, 0, 1, 0, 0, 191, 20, 70, 4, 5, 5, 7, 20, 7, 0, 149, 0})
        dPKMBaseStats.Add(432, New UInt16() {71, 82, 64, 112, 64, 59, 0, 0, 75, 183, 0, 0, 0, 2, 0, 0, 191, 20, 70, 4, 5, 5, 47, 20, 7, 0, 149, 0})
        dPKMBaseStats.Add(433, New UInt16() {45, 30, 50, 45, 65, 50, 14, 14, 120, 74, 0, 0, 0, 0, 1, 0, 127, 25, 70, 4, 15, 15, 26, 0, 2, 0, 198, 0})
        dPKMBaseStats.Add(434, New UInt16() {63, 63, 47, 74, 41, 41, 3, 17, 225, 79, 0, 0, 0, 1, 0, 0, 127, 20, 70, 0, 5, 5, 1, 106, 6, 0, 151, 0})
        dPKMBaseStats.Add(435, New UInt16() {103, 93, 67, 84, 71, 61, 3, 17, 60, 209, 2, 0, 0, 0, 0, 0, 127, 20, 70, 0, 5, 5, 1, 106, 6, 0, 151, 0})
        dPKMBaseStats.Add(436, New UInt16() {57, 24, 86, 23, 24, 86, 8, 14, 255, 72, 0, 0, 1, 0, 0, 0, 255, 20, 70, 0, 10, 10, 26, 85, 3, 0, 233, 0})
        dPKMBaseStats.Add(437, New UInt16() {67, 89, 116, 33, 79, 116, 8, 14, 90, 188, 0, 0, 1, 0, 0, 1, 255, 20, 70, 0, 10, 10, 26, 85, 3, 0, 233, 0})
        dPKMBaseStats.Add(438, New UInt16() {50, 80, 95, 10, 10, 45, 5, 5, 255, 68, 0, 0, 1, 0, 0, 0, 127, 20, 70, 0, 15, 15, 5, 69, 5, 0, 0, 0})
        dPKMBaseStats.Add(439, New UInt16() {20, 25, 45, 60, 70, 90, 14, 14, 145, 78, 0, 0, 0, 0, 0, 1, 127, 25, 70, 0, 15, 15, 43, 111, 9, 0, 154, 0})
        dPKMBaseStats.Add(440, New UInt16() {100, 5, 5, 30, 15, 65, 0, 0, 130, 255, 1, 0, 0, 0, 0, 0, 254, 40, 140, 4, 15, 15, 30, 32, 9, 110, 256, 0})
        dPKMBaseStats.Add(441, New UInt16() {76, 65, 45, 91, 92, 42, 0, 2, 30, 107, 0, 1, 0, 0, 0, 0, 127, 20, 35, 3, 4, 4, 51, 77, 4, 0, 277, 0})
        dPKMBaseStats.Add(442, New UInt16() {50, 92, 108, 35, 92, 108, 7, 17, 100, 168, 0, 0, 1, 0, 0, 1, 127, 30, 70, 0, 11, 11, 46, 0, 6, 0, 0, 0})
        dPKMBaseStats.Add(443, New UInt16() {58, 70, 45, 42, 40, 45, 16, 4, 45, 67, 0, 1, 0, 0, 0, 0, 127, 40, 70, 5, 1, 14, 8, 0, 1, 0, 197, 0})
        dPKMBaseStats.Add(444, New UInt16() {68, 90, 65, 82, 50, 55, 16, 4, 45, 144, 0, 2, 0, 0, 0, 0, 127, 40, 70, 5, 1, 14, 8, 0, 1, 0, 197, 0})
        dPKMBaseStats.Add(445, New UInt16() {108, 130, 95, 102, 80, 85, 16, 4, 45, 218, 0, 3, 0, 0, 0, 0, 127, 40, 70, 5, 1, 14, 8, 0, 1, 0, 197, 0})
        dPKMBaseStats.Add(446, New UInt16() {135, 85, 40, 5, 40, 85, 0, 0, 50, 94, 1, 0, 0, 0, 0, 0, 31, 40, 70, 5, 15, 15, 53, 47, 4, 234, 234, 0})
        dPKMBaseStats.Add(447, New UInt16() {40, 70, 40, 60, 35, 40, 1, 1, 75, 72, 0, 1, 0, 0, 0, 0, 31, 25, 70, 3, 15, 15, 80, 39, 1, 0, 0, 0})
        dPKMBaseStats.Add(448, New UInt16() {70, 110, 70, 90, 115, 70, 1, 8, 45, 204, 0, 1, 0, 0, 1, 0, 31, 25, 70, 3, 5, 8, 80, 39, 1, 0, 0, 0})
        dPKMBaseStats.Add(449, New UInt16() {68, 72, 78, 32, 38, 42, 4, 4, 140, 95, 0, 0, 1, 0, 0, 0, 127, 30, 70, 5, 5, 5, 45, 0, 5, 0, 0, 0})
        dPKMBaseStats.Add(450, New UInt16() {108, 112, 118, 47, 68, 72, 4, 4, 60, 198, 0, 0, 2, 0, 0, 0, 127, 30, 70, 5, 5, 5, 45, 0, 5, 0, 0, 0})
        dPKMBaseStats.Add(451, New UInt16() {40, 50, 90, 65, 30, 55, 3, 6, 120, 114, 0, 0, 1, 0, 0, 0, 127, 20, 70, 5, 3, 9, 4, 97, 6, 0, 245, 120})
        dPKMBaseStats.Add(452, New UInt16() {70, 90, 110, 95, 60, 75, 3, 17, 45, 204, 0, 0, 2, 0, 0, 0, 127, 20, 70, 5, 3, 9, 4, 97, 6, 0, 245, 60})
        dPKMBaseStats.Add(453, New UInt16() {48, 61, 40, 50, 61, 40, 3, 1, 140, 83, 0, 1, 0, 0, 0, 0, 127, 10, 100, 0, 8, 8, 107, 87, 1, 0, 281, 150})
        dPKMBaseStats.Add(454, New UInt16() {83, 106, 65, 85, 86, 65, 3, 1, 75, 181, 0, 2, 0, 0, 0, 0, 127, 20, 70, 0, 8, 8, 107, 87, 1, 0, 281, 120})
        dPKMBaseStats.Add(455, New UInt16() {74, 100, 72, 46, 90, 72, 12, 12, 200, 164, 0, 2, 0, 0, 0, 0, 127, 25, 70, 5, 7, 7, 26, 0, 3, 0, 0, 60})
        dPKMBaseStats.Add(456, New UInt16() {49, 49, 56, 66, 49, 61, 11, 11, 190, 90, 0, 0, 0, 1, 0, 0, 127, 20, 70, 1, 12, 12, 33, 114, 1, 0, 187, 0})
        dPKMBaseStats.Add(457, New UInt16() {69, 69, 76, 91, 69, 86, 11, 11, 75, 156, 0, 0, 0, 2, 0, 0, 127, 20, 70, 1, 12, 12, 33, 114, 1, 0, 187, 0})
        dPKMBaseStats.Add(458, New UInt16() {45, 20, 50, 50, 60, 120, 11, 2, 25, 108, 0, 0, 0, 0, 0, 1, 127, 25, 70, 5, 15, 15, 33, 11, 1, 0, 0, 0})
        dPKMBaseStats.Add(459, New UInt16() {60, 62, 50, 40, 62, 60, 12, 15, 120, 131, 0, 1, 0, 0, 0, 0, 127, 20, 70, 5, 1, 7, 117, 0, 8, 0, 246, 0})
        dPKMBaseStats.Add(460, New UInt16() {90, 92, 75, 60, 92, 85, 12, 15, 60, 214, 0, 1, 0, 0, 1, 0, 127, 20, 70, 5, 1, 7, 117, 0, 8, 0, 246, 0})
        dPKMBaseStats.Add(461, New UInt16() {70, 120, 65, 125, 45, 85, 17, 15, 45, 199, 0, 1, 0, 1, 0, 0, 127, 20, 35, 3, 5, 5, 46, 46, 4, 286, 217, 0})
        dPKMBaseStats.Add(462, New UInt16() {70, 70, 115, 60, 130, 90, 13, 8, 30, 211, 0, 0, 0, 0, 3, 0, 255, 20, 70, 0, 10, 10, 42, 5, 7, 0, 233, 0})
        dPKMBaseStats.Add(463, New UInt16() {110, 85, 95, 50, 80, 95, 0, 0, 30, 193, 3, 0, 0, 0, 0, 0, 127, 20, 70, 0, 1, 1, 20, 12, 9, 0, 279, 0})
        dPKMBaseStats.Add(464, New UInt16() {115, 140, 130, 40, 55, 55, 4, 5, 30, 217, 0, 3, 0, 0, 0, 0, 127, 20, 70, 5, 1, 5, 31, 116, 7, 0, 0, 0})
        dPKMBaseStats.Add(465, New UInt16() {100, 100, 125, 50, 110, 50, 12, 12, 30, 211, 0, 0, 2, 0, 0, 0, 127, 20, 70, 0, 7, 7, 34, 102, 1, 0, 0, 0})
        dPKMBaseStats.Add(466, New UInt16() {75, 123, 67, 95, 95, 85, 13, 13, 30, 199, 0, 3, 0, 0, 0, 0, 63, 25, 70, 0, 8, 8, 78, 0, 2, 322, 0, 0})
        dPKMBaseStats.Add(467, New UInt16() {75, 95, 67, 83, 125, 95, 10, 10, 30, 199, 0, 0, 0, 0, 3, 0, 63, 25, 70, 0, 8, 8, 49, 0, 0, 0, 323, 0})
        dPKMBaseStats.Add(468, New UInt16() {85, 50, 95, 80, 120, 115, 0, 2, 30, 220, 0, 0, 0, 0, 2, 1, 31, 10, 70, 4, 4, 6, 55, 32, 8, 0, 0, 0})
        dPKMBaseStats.Add(469, New UInt16() {86, 76, 86, 95, 116, 56, 6, 2, 30, 198, 0, 2, 0, 0, 0, 0, 127, 20, 70, 0, 3, 3, 3, 110, 3, 0, 265, 0})
        dPKMBaseStats.Add(470, New UInt16() {65, 110, 130, 95, 60, 65, 12, 12, 45, 196, 0, 0, 2, 0, 0, 0, 31, 35, 35, 0, 5, 5, 102, 102, 3, 0, 0, 0})
        dPKMBaseStats.Add(471, New UInt16() {65, 60, 110, 65, 130, 95, 15, 15, 45, 196, 0, 0, 0, 0, 2, 0, 31, 35, 35, 0, 5, 5, 81, 81, 1, 0, 0, 0})
        dPKMBaseStats.Add(472, New UInt16() {75, 95, 125, 95, 45, 75, 4, 2, 30, 192, 0, 0, 2, 0, 0, 0, 127, 20, 70, 3, 3, 3, 52, 8, 6, 0, 0, 0})
        dPKMBaseStats.Add(473, New UInt16() {110, 130, 80, 80, 70, 60, 15, 4, 50, 207, 0, 3, 0, 0, 0, 0, 127, 20, 70, 5, 5, 5, 12, 81, 5, 0, 0, 0})
        dPKMBaseStats.Add(474, New UInt16() {85, 80, 70, 90, 135, 75, 0, 0, 30, 185, 0, 0, 0, 0, 3, 0, 255, 20, 70, 0, 10, 10, 91, 88, 0, 0, 0, 0})
        dPKMBaseStats.Add(475, New UInt16() {68, 125, 65, 80, 65, 115, 14, 1, 45, 208, 0, 3, 0, 0, 0, 0, 0, 20, 35, 5, 11, 11, 80, 80, 8, 0, 0, 0})
        dPKMBaseStats.Add(476, New UInt16() {60, 55, 145, 40, 75, 150, 5, 8, 60, 198, 0, 0, 1, 0, 0, 2, 127, 20, 70, 0, 10, 10, 5, 42, 7, 0, 238, 0})
        dPKMBaseStats.Add(477, New UInt16() {45, 100, 135, 45, 65, 135, 7, 7, 45, 210, 0, 0, 1, 0, 0, 2, 127, 25, 35, 4, 11, 11, 46, 0, 4, 0, 196, 0})
        dPKMBaseStats.Add(478, New UInt16() {70, 80, 70, 110, 80, 70, 15, 7, 75, 187, 0, 0, 0, 2, 0, 0, 254, 20, 70, 0, 6, 10, 81, 81, 8, 0, 199, 0})
        dPKMBaseStats.Add(479, New UInt16() {50, 50, 77, 91, 95, 77, 13, 7, 45, 132, 0, 0, 0, 1, 1, 0, 255, 20, 70, 0, 11, 11, 26, 0, 0, 0, 0, 0})
        dPKMBaseStats.Add(480, New UInt16() {75, 75, 130, 95, 75, 130, 14, 14, 3, 210, 0, 0, 2, 0, 0, 1, 255, 80, 140, 5, 15, 15, 26, 0, 2, 0, 0, 0})
        dPKMBaseStats.Add(481, New UInt16() {80, 105, 105, 80, 105, 105, 14, 14, 3, 210, 0, 1, 0, 0, 1, 1, 255, 80, 140, 5, 15, 15, 26, 0, 9, 0, 0, 0})
        dPKMBaseStats.Add(482, New UInt16() {75, 125, 70, 115, 125, 70, 14, 14, 3, 210, 0, 2, 0, 0, 1, 0, 255, 80, 140, 5, 15, 15, 26, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(483, New UInt16() {100, 120, 120, 90, 150, 100, 8, 16, 30, 220, 0, 0, 0, 0, 3, 0, 255, 120, 0, 5, 15, 15, 46, 0, 8, 0, 0, 0})
        dPKMBaseStats.Add(484, New UInt16() {90, 120, 100, 100, 150, 120, 11, 16, 30, 220, 0, 0, 0, 0, 3, 0, 255, 120, 0, 5, 15, 15, 46, 0, 6, 0, 0, 0})
        dPKMBaseStats.Add(485, New UInt16() {91, 90, 106, 77, 130, 106, 10, 8, 3, 215, 0, 0, 0, 0, 3, 0, 127, 10, 100, 5, 15, 15, 18, 0, 5, 0, 0, 0})
        dPKMBaseStats.Add(486, New UInt16() {110, 160, 110, 100, 80, 110, 0, 0, 3, 220, 0, 3, 0, 0, 0, 0, 255, 120, 0, 5, 15, 15, 112, 0, 8, 0, 0, 0})
        dPKMBaseStats.Add(487, New UInt16() {150, 100, 120, 90, 100, 120, 7, 16, 3, 220, 3, 0, 0, 0, 0, 0, 255, 120, 0, 5, 15, 15, 46, 0, 4, 0, 0, 0})
        dPKMBaseStats.Add(488, New UInt16() {120, 70, 120, 85, 75, 130, 14, 14, 3, 210, 0, 0, 0, 0, 0, 3, 254, 120, 100, 5, 15, 15, 26, 0, 2, 0, 0, 0})
        dPKMBaseStats.Add(489, New UInt16() {80, 80, 80, 80, 80, 80, 11, 11, 30, 165, 1, 0, 0, 0, 0, 0, 255, 40, 70, 5, 2, 6, 93, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(490, New UInt16() {100, 100, 100, 100, 100, 100, 11, 11, 3, 215, 3, 0, 0, 0, 0, 0, 255, 10, 70, 5, 2, 6, 93, 0, 1, 0, 0, 0})
        dPKMBaseStats.Add(491, New UInt16() {70, 90, 90, 125, 135, 90, 17, 17, 3, 210, 0, 0, 0, 1, 2, 0, 255, 120, 0, 5, 15, 15, 123, 0, 4, 0, 0, 0})
        dPKMBaseStats.Add(492, New UInt16() {100, 100, 100, 100, 100, 100, 12, 12, 45, 64, 3, 0, 0, 0, 0, 0, 255, 120, 100, 3, 15, 15, 30, 0, 3, 157, 157, 0})
        dPKMBaseStats.Add(493, New UInt16() {120, 120, 120, 120, 120, 120, 0, 0, 3, 255, 3, 0, 0, 0, 0, 0, 255, 120, 0, 5, 15, 15, 121, 0, 7, 0, 0, 0})
        'dPKMBaseStats.Add(494, New UInt16() {10, 10, 10, 10, 10, 10, 0, 0, 3, 255, 0, 0, 0, 0, 0, 0, 255, 120, 0, 5, 15, 15, 0, 0, 10, 0, 0, 0})
        'dPKMBaseStats.Add(495, New UInt16() {10, 10, 10, 10, 10, 10, 0, 0, 3, 255, 0, 0, 0, 0, 0, 0, 255, 120, 0, 5, 15, 15, 0, 0, 10, 0, 0, 0})
        'dPKMBaseStats.Add(496, New UInt16() {50, 180, 20, 150, 180, 20, 14, 14, 3, 215, 0, 2, 0, 0, 1, 0, 255, 120, 0, 5, 15, 15, 46, 0, 0, 0, 0, 0})
        'dPKMBaseStats.Add(497, New UInt16() {50, 70, 160, 90, 70, 160, 14, 14, 3, 215, 0, 0, 2, 0, 0, 1, 255, 120, 0, 5, 15, 15, 46, 0, 0, 0, 0, 0})
        'dPKMBaseStats.Add(498, New UInt16() {50, 95, 90, 180, 95, 90, 14, 14, 3, 215, 0, 0, 0, 3, 0, 0, 255, 120, 0, 5, 15, 15, 46, 0, 0, 0, 0, 0})
        'dPKMBaseStats.Add(499, New UInt16() {60, 79, 105, 36, 59, 85, 6, 4, 45, 159, 0, 0, 2, 0, 0, 0, 254, 15, 70, 0, 3, 3, 107, 0, 7, 0, 222, 0})
        'dPKMBaseStats.Add(500, New UInt16() {60, 69, 95, 36, 69, 95, 6, 8, 45, 159, 0, 0, 1, 0, 0, 1, 254, 15, 70, 0, 3, 3, 107, 0, 7, 0, 222, 0})

        'Colors
        dpColors.Add(0, Color.Red)
        dpColors.Add(1, Color.Blue)
        dpColors.Add(2, Color.Yellow)
        dpColors.Add(3, Color.Green)
        dpColors.Add(4, Color.Black)
        dpColors.Add(5, Color.Brown)
        dpColors.Add(6, Color.Purple)
        dpColors.Add(7, Color.Gray)
        dpColors.Add(8, Color.White)
        dpColors.Add(9, Color.Pink)

        'Wallpapers
        dpWallpapers.Add(0, "Forest")
        dpWallpaperImages.Add(0, My.Resources.WP_Forest)
        dpWallpapers.Add(1, "City")
        dpWallpaperImages.Add(1, My.Resources.WP_City)
        dpWallpapers.Add(2, "Desert")
        dpWallpaperImages.Add(2, My.Resources.WP_Desert)
        dpWallpapers.Add(3, "Savanna")
        dpWallpaperImages.Add(3, My.Resources.WP_Savanna)
        dpWallpapers.Add(4, "Crag")
        dpWallpaperImages.Add(4, My.Resources.WP_Crag)
        dpWallpapers.Add(5, "Volcano")
        dpWallpaperImages.Add(5, My.Resources.WP_Volcano)
        dpWallpapers.Add(6, "Snow")
        dpWallpaperImages.Add(6, My.Resources.WP_Snow)
        dpWallpapers.Add(7, "Cave")
        dpWallpaperImages.Add(7, My.Resources.WP_Cave)
        dpWallpapers.Add(8, "Beach")
        dpWallpaperImages.Add(8, My.Resources.WP_Beach)
        dpWallpapers.Add(9, "Seafloor")
        dpWallpaperImages.Add(9, My.Resources.WP_Seafloor)
        dpWallpapers.Add(10, "River")
        dpWallpaperImages.Add(10, My.Resources.WP_River)
        dpWallpapers.Add(11, "Sky")
        dpWallpaperImages.Add(11, My.Resources.WP_Sky)
        dpWallpapers.Add(12, "Poké Center")
        dpWallpaperImages.Add(12, My.Resources.WP_Poké_Center)
        dpWallpapers.Add(13, "Machine")
        dpWallpaperImages.Add(13, My.Resources.WP_Machine)
        dpWallpapers.Add(14, "Checks")
        dpWallpaperImages.Add(14, My.Resources.WP_Checks)
        dpWallpapers.Add(15, "Simple")
        dpWallpaperImages.Add(15, My.Resources.WP_Simple)
        dpWallpapers.Add(16, "Space")
        dpWallpaperImages.Add(16, My.Resources.WP_Space)
        dpWallpapers.Add(17, "Backyard")
        dpWallpaperImages.Add(17, My.Resources.WP_Backyard)
        dpWallpapers.Add(18, "Nostalgic")
        dpWallpaperImages.Add(18, My.Resources.WP_Nostalgic)
        dpWallpapers.Add(19, "Torchic")
        dpWallpaperImages.Add(19, My.Resources.WP_Torchic)
        dpWallpapers.Add(20, "Trio")
        dpWallpaperImages.Add(20, My.Resources.WP_Trio)
        dpWallpapers.Add(21, "Pikapika")
        dpWallpaperImages.Add(21, My.Resources.WP_Pikapika)
        dpWallpapers.Add(22, "Legend")
        dpWallpaperImages.Add(22, My.Resources.WP_Legend)
        dpWallpapers.Add(23, "Team Galactic")
        dpWallpaperImages.Add(23, My.Resources.WP_Team_Galactic)
        dpWallpapers.Add(24, "Distortion World")
        dpWallpaperImages.Add(24, My.Resources.WP_Platinum_Torn_World)
        dpWallpapers.Add(25, "Contest")
        dpWallpaperImages.Add(25, My.Resources.WP_Platinum_Contest)
        dpWallpapers.Add(26, "Nostalgic")
        dpWallpaperImages.Add(26, My.Resources.WP_Platinum_Red_Blue)
        dpWallpapers.Add(27, "Croagunk")
        dpWallpaperImages.Add(27, My.Resources.WP_Platinum_Croagunk)
        dpWallpapers.Add(28, "Trio")
        dpWallpaperImages.Add(28, My.Resources.WP_Platinum_Starters)
        dpWallpapers.Add(29, "Pikapika")
        dpWallpaperImages.Add(29, My.Resources.WP_Platinum_Pikachu_Family)
        dpWallpapers.Add(30, "Legend")
        dpWallpaperImages.Add(30, My.Resources.WP_Platinum_Giratina_Origin_Forme)
        dpWallpapers.Add(31, "Team Galactic")
        dpWallpaperImages.Add(31, My.Resources.WP_Platinum_Team_Galactic)

        'Natures
        dpNatures.Add(0, New Decimal() {1, 1, 1, 1, 1, 1, 5, 5})
        dpNatures.Add(1, New Decimal() {1, 1.1, 0.9, 1, 1, 1, 0, 4})
        dpNatures.Add(2, New Decimal() {1, 1.1, 1, 0.9, 1, 1, 0, 2})
        dpNatures.Add(3, New Decimal() {1, 1.1, 1, 1, 0.9, 1, 0, 1})
        dpNatures.Add(4, New Decimal() {1, 1.1, 1, 1, 1, 0.9, 0, 3})
        dpNatures.Add(5, New Decimal() {1, 0.9, 1.1, 1, 1, 1, 4, 0})
        dpNatures.Add(6, New Decimal() {1, 1, 1, 1, 1, 1, 5, 5})
        dpNatures.Add(7, New Decimal() {1, 1, 1.1, 0.9, 1, 1, 4, 2})
        dpNatures.Add(8, New Decimal() {1, 1, 1.1, 1, 0.9, 1, 4, 1})
        dpNatures.Add(9, New Decimal() {1, 1, 1.1, 1, 1, 0.9, 4, 3})
        dpNatures.Add(10, New Decimal() {1, 0.9, 1, 1.1, 1, 1, 2, 0})
        dpNatures.Add(11, New Decimal() {1, 1, 0.9, 1.1, 1, 1, 2, 4})
        dpNatures.Add(12, New Decimal() {1, 1, 1, 1, 1, 1, 5, 5})
        dpNatures.Add(13, New Decimal() {1, 1, 1, 1.1, 0.9, 1, 2, 1})
        dpNatures.Add(14, New Decimal() {1, 1, 1, 1.1, 1, 0.9, 2, 3})
        dpNatures.Add(15, New Decimal() {1, 0.9, 1, 1, 1.1, 1, 1, 0})
        dpNatures.Add(16, New Decimal() {1, 1, 0.9, 1, 1.1, 1, 1, 4})
        dpNatures.Add(17, New Decimal() {1, 1, 1, 0.9, 1.1, 1, 1, 2})
        dpNatures.Add(18, New Decimal() {1, 1, 1, 1, 1, 1, 5, 5})
        dpNatures.Add(19, New Decimal() {1, 1, 1, 1, 1.1, 0.9, 1, 3})
        dpNatures.Add(20, New Decimal() {1, 0.9, 1, 1, 1, 1.1, 3, 0})
        dpNatures.Add(21, New Decimal() {1, 1, 0.9, 1, 1, 1.1, 3, 4})
        dpNatures.Add(22, New Decimal() {1, 1, 1, 0.9, 1, 1.1, 3, 2})
        dpNatures.Add(23, New Decimal() {1, 1, 1, 1, 0.9, 1.1, 3, 1})
        dpNatures.Add(24, New Decimal() {1, 1, 1, 1, 1, 1, 5, 5})

        'Locations
        dpLocations.Add(0, " Mystery Zone")
        dpLocations.Add(1, "Twinleaf Town")
        dpLocations.Add(2, "Sandgem Town")
        dpLocations.Add(3, "Floaroma Town")
        dpLocations.Add(4, "Solaceon Town")
        dpLocations.Add(5, "Celestic Town")
        dpLocations.Add(6, "Jubilife City")
        dpLocations.Add(7, "Canalave City")
        dpLocations.Add(8, "Oreburgh City")
        dpLocations.Add(9, "Eterna City")
        dpLocations.Add(10, "Hearthome City")
        dpLocations.Add(11, "Pastoria City")
        dpLocations.Add(12, "Veilstone City")
        dpLocations.Add(13, "Sunyshore City")
        dpLocations.Add(14, "Snowpoint City")
        dpLocations.Add(15, "Pokémon League")
        dpLocations.Add(16, "Route 201")
        dpLocations.Add(17, "Route 202")
        dpLocations.Add(18, "Route 203")
        dpLocations.Add(19, "Route 204")
        dpLocations.Add(20, "Route 205")
        dpLocations.Add(21, "Route 206")
        dpLocations.Add(22, "Route 207")
        dpLocations.Add(23, "Route 208")
        dpLocations.Add(24, "Route 209")
        dpLocations.Add(25, "Route 210")
        dpLocations.Add(26, "Route 211")
        dpLocations.Add(27, "Route 212")
        dpLocations.Add(28, "Route 213")
        dpLocations.Add(29, "Route 214")
        dpLocations.Add(30, "Route 215")
        dpLocations.Add(31, "Route 216")
        dpLocations.Add(32, "Route 217")
        dpLocations.Add(33, "Route 218")
        dpLocations.Add(34, "Route 219")
        dpLocations.Add(35, "Route 220")
        dpLocations.Add(36, "Route 221")
        dpLocations.Add(37, "Route 222")
        dpLocations.Add(38, "Route 223")
        dpLocations.Add(39, "Route 224")
        dpLocations.Add(40, "Route 225")
        dpLocations.Add(41, "Route 226")
        dpLocations.Add(42, "Route 227")
        dpLocations.Add(43, "Route 228")
        dpLocations.Add(44, "Route 229")
        dpLocations.Add(45, "Route 230")
        dpLocations.Add(46, "Oreburgh Mine")
        dpLocations.Add(47, "Valley Windworks")
        dpLocations.Add(48, "Eterna Forest")
        dpLocations.Add(49, "Fuego Ironworks")
        dpLocations.Add(50, "Mt. Coronet")
        dpLocations.Add(51, "Spear Pillar")
        dpLocations.Add(52, "Great Marsh")
        dpLocations.Add(53, "Solaceon Ruins")
        dpLocations.Add(54, "Victory Road")
        dpLocations.Add(55, "Pal Park")
        dpLocations.Add(56, "Amity Square")
        dpLocations.Add(57, "Ravaged Path")
        dpLocations.Add(58, "Floaroma Meadow")
        dpLocations.Add(59, "Oreburgh Gate")
        dpLocations.Add(60, "Fullmoon Island")
        dpLocations.Add(61, "Sendoff Spring")
        dpLocations.Add(62, "Turnback Cave")
        dpLocations.Add(63, "Flower Paradise")
        dpLocations.Add(64, "Snowpoint Temple")
        dpLocations.Add(65, "Wayward Cave")
        dpLocations.Add(66, "Ruin Maniac Cave")
        dpLocations.Add(67, "Maniac Tunnel")
        dpLocations.Add(68, "Trophy Garden")
        dpLocations.Add(69, "Iron Island")
        dpLocations.Add(70, "Old Chateau")
        dpLocations.Add(71, "Galactic HQ")
        dpLocations.Add(72, "Verity Lakefront")
        dpLocations.Add(73, "Valor Lakefront")
        dpLocations.Add(74, "Acuity Lakefront")
        dpLocations.Add(75, "Spring Path")
        dpLocations.Add(76, "Lake Verity")
        dpLocations.Add(77, "Lake Valor")
        dpLocations.Add(78, "Lake Acuity")
        dpLocations.Add(79, "Newmoon Island")
        dpLocations.Add(80, "Battle Tower")
        dpLocations.Add(81, "Fight Area")
        dpLocations.Add(82, "Survival Area")
        dpLocations.Add(83, "Resort Area")
        dpLocations.Add(84, "Stark Mountain")
        dpLocations.Add(85, "Seabreak Path")
        dpLocations.Add(86, "Hall of Origin")
        dpLocations.Add(87, "Verity Cavern")
        dpLocations.Add(88, "Valor Cavern")
        dpLocations.Add(89, "Acuity Cavern")
        dpLocations.Add(90, "Jubilife TV")
        dpLocations.Add(91, "Pokétch Co.")
        dpLocations.Add(92, "GTS")
        dpLocations.Add(93, "Trainer's School")
        dpLocations.Add(94, "Mining Museum")
        dpLocations.Add(95, "Flower Shop")
        dpLocations.Add(96, "Cycle Shop")
        dpLocations.Add(97, "Contest Hall")
        dpLocations.Add(98, "Poffin House")
        dpLocations.Add(99, "Foreign Building")
        dpLocations.Add(100, "Pokémon Day Care")
        dpLocations.Add(101, "Veilstone Store")
        dpLocations.Add(102, "Game Corner")
        dpLocations.Add(103, "Canalave Library")
        dpLocations.Add(104, "Vista Lighthouse")
        dpLocations.Add(105, "Sunyshore Market")
        dpLocations.Add(106, "Pokémon Mansion")
        dpLocations.Add(107, "Footstep House")
        dpLocations.Add(108, "Café")
        dpLocations.Add(109, "Grand Lake")
        dpLocations.Add(110, "Restaurant")
        dpLocations.Add(111, "Battle Park")
        dpLocations.Add(112, "Battle Frontier")
        dpLocations.Add(113, "Battle Factory")
        dpLocations.Add(114, "Battle Castle")
        dpLocations.Add(115, "Battle Arcade")
        dpLocations.Add(116, "Battle Hall")
        dpLocations.Add(117, "Distortion World")
        dpLocations.Add(118, "Global Terminal")
        dpLocations.Add(119, "Villa")
        dpLocations.Add(120, "Battleground")
        dpLocations.Add(121, "ROTOM's Room")
        dpLocations.Add(122, "T.G. Eterna Bldg")
        dpLocations.Add(123, "Iron Ruins")
        dpLocations.Add(124, "Iceburg Ruins")
        dpLocations.Add(125, "Rock Peak Ruins")
        dpLocations.Add(2000, "Day-Care Couple")
        dpLocations.Add(2001, "Link trade") 'arrived
        dpLocations.Add(2002, "Link trade") 'met
        dpLocations.Add(2003, "Kanto")
        dpLocations.Add(2004, "Johto")
        dpLocations.Add(2005, "Hoenn")
        dpLocations.Add(2006, "Sinnoh")
        dpLocations.Add(2007, "----")
        dpLocations.Add(2008, "Distant land")
        dpLocations.Add(2009, "Traveling Man")
        dpLocations.Add(2010, "Riley")
        dpLocations.Add(2011, "Hall of Fame")
        dpLocations.Add(2012, "Mystery Zone")
        dpLocations.Add(3000, "Lovely place")
        dpLocations.Add(3001, "Pokémon Ranger")
        dpLocations.Add(3002, "Faraway place")
        dpLocations.Add(3003, "Pokémon Movie")
        dpLocations.Add(3004, "Pokémon Movie 06")
        dpLocations.Add(3005, "Pokémon Movie 07")
        dpLocations.Add(3006, "Pokémon Movie 08")
        dpLocations.Add(3007, "Pokémon Movie 09")
        dpLocations.Add(3008, "Pokémon Movie 10")
        dpLocations.Add(3009, "Pokémon Movie 11")
        dpLocations.Add(3010, "Pokémon Movie 12")
        dpLocations.Add(3011, "Pokémon Movie 13")
        dpLocations.Add(3012, "Pokémon Movie 14")
        dpLocations.Add(3013, "Pokémon Movie 15")
        dpLocations.Add(3014, "Pokémon Movie 16")
        dpLocations.Add(3015, "Pokémon Cartoon")
        dpLocations.Add(3016, "Space World")
        dpLocations.Add(3017, "Space World 06")
        dpLocations.Add(3018, "Space World 07")
        dpLocations.Add(3019, "Space World 08")
        dpLocations.Add(3020, "Space World 09")
        dpLocations.Add(3021, "Space World 10")
        dpLocations.Add(3022, "Space World 11")
        dpLocations.Add(3023, "Space World 12")
        dpLocations.Add(3024, "Space World 13")
        dpLocations.Add(3025, "Space World 14")
        dpLocations.Add(3026, "Space World 15")
        dpLocations.Add(3027, "Space World 16")
        dpLocations.Add(3028, "Pokémon Festa")
        dpLocations.Add(3029, "Pokémon Festa 06")
        dpLocations.Add(3030, "Pokémon Festa 07")
        dpLocations.Add(3031, "Pokémon Festa 08")
        dpLocations.Add(3032, "Pokémon Festa 09")
        dpLocations.Add(3033, "Pokémon Festa 10")
        dpLocations.Add(3034, "Pokémon Festa 11")
        dpLocations.Add(3035, "Pokémon Festa 12")
        dpLocations.Add(3036, "Pokémon Festa 13")
        dpLocations.Add(3037, "Pokémon Festa 14")
        dpLocations.Add(3038, "Pokémon Festa 15")
        dpLocations.Add(3039, "Pokémon Festa 16")
        dpLocations.Add(3040, "PokéPARK")
        dpLocations.Add(3041, "PokéPARK 06")
        dpLocations.Add(3042, "PokéPARK 07")
        dpLocations.Add(3043, "PokéPARK 08")
        dpLocations.Add(3044, "PokéPARK 09")
        dpLocations.Add(3045, "PokéPARK 10")
        dpLocations.Add(3046, "PokéPARK 11")
        dpLocations.Add(3047, "PokéPARK 12")
        dpLocations.Add(3048, "PokéPARK 13")
        dpLocations.Add(3049, "PokéPARK 14")
        dpLocations.Add(3050, "PokéPARK 15")
        dpLocations.Add(3051, "PokéPARK 16")
        dpLocations.Add(3052, "Pokémon Center")
        dpLocations.Add(3053, "PC Tokyo")
        dpLocations.Add(3054, "PC Osaka")
        dpLocations.Add(3055, "PC Fukuoka")
        dpLocations.Add(3056, "PC Nagoya")
        dpLocations.Add(3057, "PC Sapporo")
        dpLocations.Add(3058, "PC Yokohama")
        dpLocations.Add(3059, "Nintendo World")
        dpLocations.Add(3060, "Pokémon Event")
        dpLocations.Add(3061, "Pokémon Event 06")
        dpLocations.Add(3062, "Pokémon Event 07")
        dpLocations.Add(3063, "Pokémon Event 08")
        dpLocations.Add(3064, "Pokémon Event 09")
        dpLocations.Add(3065, "Pokémon Event 10")
        dpLocations.Add(3066, "Pokémon Event 11")
        dpLocations.Add(3067, "Pokémon Event 12")
        dpLocations.Add(3068, "Pokémon Event 13")
        dpLocations.Add(3069, "Pokémon Event 14")
        dpLocations.Add(3070, "Pokémon Event 15")
        dpLocations.Add(3071, "Pokémon Event 16")
        dpLocations.Add(3072, "Wi-Fi Event")
        dpLocations.Add(3073, "Wi-Fi Gift")
        dpLocations.Add(3074, "Pokémon Fan Club")
        dpLocations.Add(3075, "Event Site")
        dpLocations.Add(3076, "Concert Event")

        'Shuffle order
        dpPKMShuffle.Add(0, "ABCD")
        dpPKMShuffle.Add(1, "ABDC")
        dpPKMShuffle.Add(2, "ACBD")
        dpPKMShuffle.Add(3, "ACDB")
        dpPKMShuffle.Add(4, "ADBC")
        dpPKMShuffle.Add(5, "ADCB")
        dpPKMShuffle.Add(6, "BACD")
        dpPKMShuffle.Add(7, "BADC")
        dpPKMShuffle.Add(8, "BCAD")
        dpPKMShuffle.Add(9, "BCDA")
        dpPKMShuffle.Add(10, "BDAC")
        dpPKMShuffle.Add(11, "BDCA")
        dpPKMShuffle.Add(12, "CABD")
        dpPKMShuffle.Add(13, "CADB")
        dpPKMShuffle.Add(14, "CBAD")
        dpPKMShuffle.Add(15, "CBDA")
        dpPKMShuffle.Add(16, "CDAB")
        dpPKMShuffle.Add(17, "CDBA")
        dpPKMShuffle.Add(18, "DABC")
        dpPKMShuffle.Add(19, "DACB")
        dpPKMShuffle.Add(20, "DBAC")
        dpPKMShuffle.Add(21, "DBCA")
        dpPKMShuffle.Add(22, "DCAB")
        dpPKMShuffle.Add(23, "DCBA")

        'Encounter Type
        dpPKMEncounters.Add(&H0, "Egg/Pal Park/Event/Honey Tree/Shaymin")
        dpPKMEncounters.Add(&H2, "Tall Grass/Darkrai")
        dpPKMEncounters.Add(&H4, "Dialga/Palkia")
        dpPKMEncounters.Add(&H5, "Cave/Hall of Origin/Giratina")
        dpPKMEncounters.Add(&H7, "Water")
        dpPKMEncounters.Add(&H9, "Building")
        dpPKMEncounters.Add(&H17, "Distortion World (Pt)")
        dpPKMEncounters.Add(&H18, "Starter/Bebe's Eevee/Fossil (Pt)")
        dpPKMEncounters.Add(&HA, "Great Marsh/Safari Zone")
        dpPKMEncounters.Add(&HC, "Starter/Fossil (D/P)")

        'Hometown
        dpPKMHometowns.Add(0, "Unknown")
        dpPKMHometowns.Add(1, "Sapphire")
        dpPKMHometowns.Add(2, "Ruby")
        dpPKMHometowns.Add(3, "Emerald")
        dpPKMHometowns.Add(4, "FireRed")
        dpPKMHometowns.Add(5, "LeafGreen")
        dpPKMHometowns.Add(7, "Gold")
        dpPKMHometowns.Add(8, "Silver")
        dpPKMHometowns.Add(10, "Diamond")
        dpPKMHometowns.Add(11, "Pearl")
        dpPKMHometowns.Add(12, "Platinum")
        dpPKMHometowns.Add(15, "Colosseum/XD")

        'Characteristics
        dpPKMCharacterstics.Add(0, New String() {"Loves to eat", "Proud of its power", "Sturdy body", "Likes to run", "Highly curious", "Strong willed"})
        dpPKMCharacterstics.Add(1, New String() {"Often dozes off", "Likes to thrash about", "Capable of taking hits", "Alert to sounds", "Mischievous", "Somewhat vain"})
        dpPKMCharacterstics.Add(2, New String() {"Often scatters things", "A little quick tempered", "Highly persistent", "Impetuous and silly", "Thoroughly cunning", "Strongly defiant"})
        dpPKMCharacterstics.Add(3, New String() {"Scatters things often", "Likes to fight", "Good endurance", "Somewhat of a clown", "Often lost in thought", "Hates to lose"})
        dpPKMCharacterstics.Add(4, New String() {"Likes to relax", "Quick tempered", "Good perserverance", "Quick to flee", "Very finicky", "Somewhat stubborn"})

        'Abilities
        dpAbilities.Add(1, New String() {"Stench", "The stench helps keep wild Pokémon away."})
        dpAbilities.Add(2, New String() {"Drizzle", "The Pokémon makes it rain if it appears in battle."})
        dpAbilities.Add(3, New String() {"Speed Boost", "The Pokémon's Speed stat is gradually boosted."})
        dpAbilities.Add(4, New String() {"Battle Armor", "The Pokémon is protected against critical hits."})
        dpAbilities.Add(5, New String() {"Sturdy", "The Pokémon is protected against 1-hit KO attacks."})
        dpAbilities.Add(6, New String() {"Damp", "Prevents combatants from self destructing."})
        dpAbilities.Add(7, New String() {"Limber", "The Pokémon is protected from paralysis."})
        dpAbilities.Add(8, New String() {"Sand Veil", "Boosts the Pokémon's evasion in a sandstorm."})
        dpAbilities.Add(9, New String() {"Static", "Contact with the Pokémon may cause paralysis."})
        dpAbilities.Add(10, New String() {"Volt Absorb", "Restores HP if hit by an Electric-type move."})
        dpAbilities.Add(11, New String() {"Water Absorb", "Restores HP if hit by a Water-type move."})
        dpAbilities.Add(12, New String() {"Oblivious", "Prevents the Pokémon from becoming infatuated."})
        dpAbilities.Add(13, New String() {"Cloud Nine", "Eliminates the effects of weather."})
        dpAbilities.Add(14, New String() {"Compoundeyes", "The Pokémon's accuracy is boosted."})
        dpAbilities.Add(15, New String() {"Insomnia", "Prevents the Pokémon from falling asleep."})
        dpAbilities.Add(16, New String() {"Color Change", "Changes the Pokémon's type to the foe's move."})
        dpAbilities.Add(17, New String() {"Immunity", "Prevents the Pokémon from getting poisoned."})
        dpAbilities.Add(18, New String() {"Flash Fire", "Powers up Fire-type moves if hit by a fire move."})
        dpAbilities.Add(19, New String() {"Shield Dust", "Blocks the added effects of attacks taken."})
        dpAbilities.Add(20, New String() {"Own Tempo", "Prevents the Pokémon from becoming confused."})
        dpAbilities.Add(21, New String() {"Suction Cups", "Negates moves that force switching out."})
        dpAbilities.Add(22, New String() {"Intimidate", "Lowers the foe's Attack stat."})
        dpAbilities.Add(23, New String() {"Shadow Tag", "Prevents the foe from escaping."})
        dpAbilities.Add(24, New String() {"Rough Skin", "Inflicts damage to the foe on contact."})
        dpAbilities.Add(25, New String() {"Wonder Guard", "Only super effective moves will hit."})
        dpAbilities.Add(26, New String() {"Levitate", "Gives full immunity to all Ground-type moves."})
        dpAbilities.Add(27, New String() {"Effect Spore", "Contact may paralyze, poison, or cause sleep."})
        dpAbilities.Add(28, New String() {"Synchronize", "Passes on a burn, poison, or paralysis to the foe."})
        dpAbilities.Add(29, New String() {"Clear Body", "Prevents the Pokémon's stats from being lowered."})
        dpAbilities.Add(30, New String() {"Natural Cure", "All status problems are healed upon switching out."})
        dpAbilities.Add(31, New String() {"Lightningrod", "The Pokémon draws in all Electric-type moves."})
        dpAbilities.Add(32, New String() {"Serene Grace", "Boosts the likelihood of added effects appearing."})
        dpAbilities.Add(33, New String() {"Swift Swim", "Boosts the Pokémon's Speed in rain."})
        dpAbilities.Add(34, New String() {"Chlorophyll", "Boosts the Pokémon's Speed in sunshine."})
        dpAbilities.Add(35, New String() {"Illuminate", "Raises the likelihood of meeting wild Pokémon."})
        dpAbilities.Add(36, New String() {"Trace", "The Pokémon copies the foe's ability."})
        dpAbilities.Add(37, New String() {"Huge Power", "Raises the Pokémon's Attack stat."})
        dpAbilities.Add(38, New String() {"Poison Point", "Contact with the Pokémon may poison the foe."})
        dpAbilities.Add(39, New String() {"Inner Focus", "The Pokémon is protected from flinching."})
        dpAbilities.Add(40, New String() {"Magma Armor", "Prevents the Pokémon from becoming frozen."})
        dpAbilities.Add(41, New String() {"Water Veil", "Prevents the Pokémon from getting a burn."})
        dpAbilities.Add(42, New String() {"Magnet Pull", "Prevents Steel-type Pokémon from escaping."})
        dpAbilities.Add(43, New String() {"Soundproof", "Gives full immunity to all sound-based moves."})
        dpAbilities.Add(44, New String() {"Rain Dish", "The Pokémon gradually recovers HP in rain."})
        dpAbilities.Add(45, New String() {"Sand Stream", "The Pokémon summons a sandstorm in battle."})
        dpAbilities.Add(46, New String() {"Pressure", "The Pokémon raises the foe's PP usage."})
        dpAbilities.Add(47, New String() {"Thick Fat", "Raises resistance to Fire- and Ice-type moves."})
        dpAbilities.Add(48, New String() {"Early Bird", "The Pokémon awakens quickly from sleep."})
        dpAbilities.Add(49, New String() {"Flame Body", "Contact with the Pokémon may burn the foe."})
        dpAbilities.Add(50, New String() {"Run Away", "Enables sure getaway from wild Pokémon."})
        dpAbilities.Add(51, New String() {"Keen Eye", "Prevents the Pokémon from losing accuracy."})
        dpAbilities.Add(52, New String() {"Hyper Cutter", "Prevents the Attack stat from being lowered."})
        dpAbilities.Add(53, New String() {"Pickup", "The Pokémon may pick up items."})
        dpAbilities.Add(54, New String() {"Truant", "The Pokémon can't attack on consecutive turns."})
        dpAbilities.Add(55, New String() {"Hustle", "Boosts the Attack stat, but lowers accuracy."})
        dpAbilities.Add(56, New String() {"Cute Charm", "Contact with the Pokémon may cause infatuation."})
        dpAbilities.Add(57, New String() {"Plus", "Boosts Sp. Atk if another Pokémon has Minus."})
        dpAbilities.Add(58, New String() {"Minus", "Boosts Sp. Atk if another Pokémon has Plus."})
        dpAbilities.Add(59, New String() {"Forecast", "Transforms with the weather."})
        dpAbilities.Add(60, New String() {"Sticky Hold", "Protects the Pokémon from item theft."})
        dpAbilities.Add(61, New String() {"Shed Skin", "The Pokémon may heal its own status problems."})
        dpAbilities.Add(62, New String() {"Guts", "Boosts Attack if there is a status problem."})
        dpAbilities.Add(63, New String() {"Marvel Scale", "Boosts Defense if there is a status problem."})
        dpAbilities.Add(64, New String() {"Liquid Ooze", "Inflicts damage on foes using any draining move."})
        dpAbilities.Add(65, New String() {"Overgrow", "Powers up Grass-type moves in a pinch."})
        dpAbilities.Add(66, New String() {"Blaze", "Powers up Fire-type moves in a pinch."})
        dpAbilities.Add(67, New String() {"Torrent", "Powers up Water-type moves in a pinch."})
        dpAbilities.Add(68, New String() {"Swarm", "Powers up Bug-type moves in a pinch."})
        dpAbilities.Add(69, New String() {"Rock Head", "Protects the Pokémon from recoil damage."})
        dpAbilities.Add(70, New String() {"Drought", "The Pokémon makes it sunny if it is in battle."})
        dpAbilities.Add(71, New String() {"Arena Trap", "Prevents the foe from fleeing."})
        dpAbilities.Add(72, New String() {"Vital Spirit", "Prevents the Pokémon from falling asleep."})
        dpAbilities.Add(73, New String() {"White Smoke", "Prevents the Pokémon's stats from being lowered."})
        dpAbilities.Add(74, New String() {"Pure Power", "Boosts the power of physical attacks."})
        dpAbilities.Add(75, New String() {"Shell Armor", "The Pokémon is protected against critical hits."})
        dpAbilities.Add(76, New String() {"Air Lock", "Eliminates the effects of weather."})
        dpAbilities.Add(77, New String() {"Tangled Feet", "Raises evasion if the Pokémon is confused."})
        dpAbilities.Add(78, New String() {"Motor Drive", "Raises Speed if hit by an Electric-type move."})
        dpAbilities.Add(79, New String() {"Rivalry", "Raises Attack if the foe is of the same gender."})
        dpAbilities.Add(80, New String() {"Steadfast", "Raises Speed each time the Pokémon flinches."})
        dpAbilities.Add(81, New String() {"Snow Cloak", "Raises evasion in a hailstorm."})
        dpAbilities.Add(82, New String() {"Gluttony", "Encourages the early use of a held Berry."})
        dpAbilities.Add(83, New String() {"Anger Point", "Raises Attack upon taking a critical hit."})
        dpAbilities.Add(84, New String() {"Unburden", "Raises Speed if a held item is used."})
        dpAbilities.Add(85, New String() {"Heatproof", "Weakens the power of Fire-type moves."})
        dpAbilities.Add(86, New String() {"Simple", "The Pokémon is prone to wild stat changes."})
        dpAbilities.Add(87, New String() {"Dry Skin", "Reduces HP if it is hot. Water restores HP."})
        dpAbilities.Add(88, New String() {"Download", "Adjusts power according to the foe's lowest Defense stat."})
        dpAbilities.Add(89, New String() {"Iron Fist", "Boosts the power of punching moves."})
        dpAbilities.Add(90, New String() {"Poison Heal", "Restores HP if the Pokémon is poisoned."})
        dpAbilities.Add(91, New String() {"Adaptability", "Powers up moves of the same type."})
        dpAbilities.Add(92, New String() {"Skill Link", "Increases the frequency of multi-strike moves."})
        dpAbilities.Add(93, New String() {"Hydration", "Heals status problems if it is raining."})
        dpAbilities.Add(94, New String() {"Solar Power", "Boosts Sp. Atk, but lowers HP in sunshine."})
        dpAbilities.Add(95, New String() {"Quick Feet", "Boosts Speed if there is a status problem."})
        dpAbilities.Add(96, New String() {"Normalize", "All the Pokémon's moves become the Normal type."})
        dpAbilities.Add(97, New String() {"Sniper", "Powers up moves if they become critical hits."})
        dpAbilities.Add(98, New String() {"Magic Guard", "The Pokémon only takes damage from attacks."})
        dpAbilities.Add(99, New String() {"No Guard", "Ensures the Pokémon and its foe's attacks land."})
        dpAbilities.Add(100, New String() {"Stall", "The Pokémon moves after even slower foes."})
        dpAbilities.Add(101, New String() {"Technician", "Powers up the Pokémon's weaker moves."})
        dpAbilities.Add(102, New String() {"Leaf Guard", "Prevents status problems in sunny weather."})
        dpAbilities.Add(103, New String() {"Klutz", "The Pokémon can't use any held items."})
        dpAbilities.Add(104, New String() {"Mold Breaker", "Moves can be used regardless of abilities."})
        dpAbilities.Add(105, New String() {"Super Luck", "Heightens the critical-hit ratios of moves."})
        dpAbilities.Add(106, New String() {"Aftermath", "Damages the foe landing the finishing hit."})
        dpAbilities.Add(107, New String() {"Anticipation", "Senses the foe's dangerous moves."})
        dpAbilities.Add(108, New String() {"Forewarn", "Determines what moves the foe has."})
        dpAbilities.Add(109, New String() {"Unaware", "Ignores any change in stats by the foe."})
        dpAbilities.Add(110, New String() {"Tinted Lens", "Powers up " & Chr(34) & "not very effective" & Chr(34) & " moves."})
        dpAbilities.Add(111, New String() {"Filter", "Powers down super- effective moves."})
        dpAbilities.Add(112, New String() {"Slow Start", "Temporarily halves Attack and Speed."})
        dpAbilities.Add(113, New String() {"Scrappy", "Enables moves to hit Ghost-type foes."})
        dpAbilities.Add(114, New String() {"Storm Drain", "The Pokémon draws in all Water-type moves."})
        dpAbilities.Add(115, New String() {"Ice Body", "The Pokémon regains HP in a hailstorm."})
        dpAbilities.Add(116, New String() {"Solid Rock", "Powers down super- effective moves."})
        dpAbilities.Add(117, New String() {"Snow Warning", "The Pokémon summons a hailstorm in battle."})
        dpAbilities.Add(118, New String() {"Honey Gather", "The Pokémon may gather Honey from somewhere."})
        dpAbilities.Add(119, New String() {"Frisk", "The Pokémon can check the foe's held item."})
        dpAbilities.Add(120, New String() {"Reckless", "Powers up moves that have recoil damage."})
        dpAbilities.Add(121, New String() {"Multitype", "Changes type to match the held Plate."})
        dpAbilities.Add(122, New String() {"Flower Gift", "Powers up party Pokémon when it is sunny."})
        dpAbilities.Add(123, New String() {"Bad Dreams", "Reduces a sleeping foe's HP."})

        dpCountries.Add(&H1, "Japan")
        dpCountries.Add(&H2, "United States/United Kingdom/Australia")
        dpCountries.Add(&H3, "France/Québec")
        dpCountries.Add(&H4, "Italy")
        dpCountries.Add(&H5, "Germany")
        dpCountries.Add(&H7, "Spain/Latin America")
        dpCountries.Add(&H8, "South Korea")

        dpNMSprites.Add(0, Nothing)
        dpNMSprites.Add(1, My.Resources._00100)
        dpNMSprites.Add(2, My.Resources._00200)
        dpNMSprites.Add(3, My.Resources._00300)
        dpNMSprites.Add(4, My.Resources._00400)
        dpNMSprites.Add(5, My.Resources._00500)
        dpNMSprites.Add(6, My.Resources._00600)
        dpNMSprites.Add(7, My.Resources._00700)
        dpNMSprites.Add(8, My.Resources._00800)
        dpNMSprites.Add(9, My.Resources._00900)
        dpNMSprites.Add(10, My.Resources._01000)
        dpNMSprites.Add(11, My.Resources._01100)
        dpNMSprites.Add(12, My.Resources._01200)
        dpNMSprites.Add(13, My.Resources._01300)
        dpNMSprites.Add(14, My.Resources._01400)
        dpNMSprites.Add(15, My.Resources._01500)
        dpNMSprites.Add(16, My.Resources._01600)
        dpNMSprites.Add(17, My.Resources._01700)
        dpNMSprites.Add(18, My.Resources._01800)
        dpNMSprites.Add(19, My.Resources._01900)
        dpNMSprites.Add(20, My.Resources._02000)
        dpNMSprites.Add(21, My.Resources._02100)
        dpNMSprites.Add(22, My.Resources._02200)
        dpNMSprites.Add(23, My.Resources._02300)
        dpNMSprites.Add(24, My.Resources._02400)
        dpNMSprites.Add(25, My.Resources._02500)
        dpNMSprites.Add(26, My.Resources._02600)
        dpNMSprites.Add(27, My.Resources._02700)
        dpNMSprites.Add(28, My.Resources._02800)
        dpNMSprites.Add(29, My.Resources._02900)
        dpNMSprites.Add(30, My.Resources._03000)
        dpNMSprites.Add(31, My.Resources._03100)
        dpNMSprites.Add(32, My.Resources._03200)
        dpNMSprites.Add(33, My.Resources._03300)
        dpNMSprites.Add(34, My.Resources._03400)
        dpNMSprites.Add(35, My.Resources._03500)
        dpNMSprites.Add(36, My.Resources._03600)
        dpNMSprites.Add(37, My.Resources._03700)
        dpNMSprites.Add(38, My.Resources._03800)
        dpNMSprites.Add(39, My.Resources._03900)
        dpNMSprites.Add(40, My.Resources._04000)
        dpNMSprites.Add(41, My.Resources._04100)
        dpNMSprites.Add(42, My.Resources._04200)
        dpNMSprites.Add(43, My.Resources._04300)
        dpNMSprites.Add(44, My.Resources._04400)
        dpNMSprites.Add(45, My.Resources._04500)
        dpNMSprites.Add(46, My.Resources._04600)
        dpNMSprites.Add(47, My.Resources._04700)
        dpNMSprites.Add(48, My.Resources._04800)
        dpNMSprites.Add(49, My.Resources._04900)
        dpNMSprites.Add(50, My.Resources._05000)
        dpNMSprites.Add(51, My.Resources._05100)
        dpNMSprites.Add(52, My.Resources._05200)
        dpNMSprites.Add(53, My.Resources._05300)
        dpNMSprites.Add(54, My.Resources._05400)
        dpNMSprites.Add(55, My.Resources._05500)
        dpNMSprites.Add(56, My.Resources._05600)
        dpNMSprites.Add(57, My.Resources._05700)
        dpNMSprites.Add(58, My.Resources._05800)
        dpNMSprites.Add(59, My.Resources._05900)
        dpNMSprites.Add(60, My.Resources._06000)
        dpNMSprites.Add(61, My.Resources._06100)
        dpNMSprites.Add(62, My.Resources._06200)
        dpNMSprites.Add(63, My.Resources._06300)
        dpNMSprites.Add(64, My.Resources._06400)
        dpNMSprites.Add(65, My.Resources._06500)
        dpNMSprites.Add(66, My.Resources._06600)
        dpNMSprites.Add(67, My.Resources._06700)
        dpNMSprites.Add(68, My.Resources._06800)
        dpNMSprites.Add(69, My.Resources._06900)
        dpNMSprites.Add(70, My.Resources._07000)
        dpNMSprites.Add(71, My.Resources._07100)
        dpNMSprites.Add(72, My.Resources._07200)
        dpNMSprites.Add(73, My.Resources._07300)
        dpNMSprites.Add(74, My.Resources._07400)
        dpNMSprites.Add(75, My.Resources._07500)
        dpNMSprites.Add(76, My.Resources._07600)
        dpNMSprites.Add(77, My.Resources._07700)
        dpNMSprites.Add(78, My.Resources._07800)
        dpNMSprites.Add(79, My.Resources._07900)
        dpNMSprites.Add(80, My.Resources._08000)
        dpNMSprites.Add(81, My.Resources._08100)
        dpNMSprites.Add(82, My.Resources._08200)
        dpNMSprites.Add(83, My.Resources._08300)
        dpNMSprites.Add(84, My.Resources._08400)
        dpNMSprites.Add(85, My.Resources._08500)
        dpNMSprites.Add(86, My.Resources._08600)
        dpNMSprites.Add(87, My.Resources._08700)
        dpNMSprites.Add(88, My.Resources._08800)
        dpNMSprites.Add(89, My.Resources._08900)
        dpNMSprites.Add(90, My.Resources._09000)
        dpNMSprites.Add(91, My.Resources._09100)
        dpNMSprites.Add(92, My.Resources._09200)
        dpNMSprites.Add(93, My.Resources._09300)
        dpNMSprites.Add(94, My.Resources._09400)
        dpNMSprites.Add(95, My.Resources._09500)
        dpNMSprites.Add(96, My.Resources._09600)
        dpNMSprites.Add(97, My.Resources._09700)
        dpNMSprites.Add(98, My.Resources._09800)
        dpNMSprites.Add(99, My.Resources._09900)
        dpNMSprites.Add(100, My.Resources._10000)
        dpNMSprites.Add(101, My.Resources._10100)
        dpNMSprites.Add(102, My.Resources._10200)
        dpNMSprites.Add(103, My.Resources._10300)
        dpNMSprites.Add(104, My.Resources._10400)
        dpNMSprites.Add(105, My.Resources._10500)
        dpNMSprites.Add(106, My.Resources._10600)
        dpNMSprites.Add(107, My.Resources._10700)
        dpNMSprites.Add(108, My.Resources._10800)
        dpNMSprites.Add(109, My.Resources._10900)
        dpNMSprites.Add(110, My.Resources._11000)
        dpNMSprites.Add(111, My.Resources._11100)
        dpNMSprites.Add(112, My.Resources._11200)
        dpNMSprites.Add(113, My.Resources._11300)
        dpNMSprites.Add(114, My.Resources._11400)
        dpNMSprites.Add(115, My.Resources._11500)
        dpNMSprites.Add(116, My.Resources._11600)
        dpNMSprites.Add(117, My.Resources._11700)
        dpNMSprites.Add(118, My.Resources._11800)
        dpNMSprites.Add(119, My.Resources._11900)
        dpNMSprites.Add(120, My.Resources._12000)
        dpNMSprites.Add(121, My.Resources._12100)
        dpNMSprites.Add(122, My.Resources._12200)
        dpNMSprites.Add(123, My.Resources._12300)
        dpNMSprites.Add(124, My.Resources._12400)
        dpNMSprites.Add(125, My.Resources._12500)
        dpNMSprites.Add(126, My.Resources._12600)
        dpNMSprites.Add(127, My.Resources._12700)
        dpNMSprites.Add(128, My.Resources._12800)
        dpNMSprites.Add(129, My.Resources._12900)
        dpNMSprites.Add(130, My.Resources._13000)
        dpNMSprites.Add(131, My.Resources._13100)
        dpNMSprites.Add(132, My.Resources._13200)
        dpNMSprites.Add(133, My.Resources._13300)
        dpNMSprites.Add(134, My.Resources._13400)
        dpNMSprites.Add(135, My.Resources._13500)
        dpNMSprites.Add(136, My.Resources._13600)
        dpNMSprites.Add(137, My.Resources._13700)
        dpNMSprites.Add(138, My.Resources._13800)
        dpNMSprites.Add(139, My.Resources._13900)
        dpNMSprites.Add(140, My.Resources._14000)
        dpNMSprites.Add(141, My.Resources._14100)
        dpNMSprites.Add(142, My.Resources._14200)
        dpNMSprites.Add(143, My.Resources._14300)
        dpNMSprites.Add(144, My.Resources._14400)
        dpNMSprites.Add(145, My.Resources._14500)
        dpNMSprites.Add(146, My.Resources._14600)
        dpNMSprites.Add(147, My.Resources._14700)
        dpNMSprites.Add(148, My.Resources._14800)
        dpNMSprites.Add(149, My.Resources._14900)
        dpNMSprites.Add(150, My.Resources._15000)
        dpNMSprites.Add(151, My.Resources._15100)
        dpNMSprites.Add(152, My.Resources._15200)
        dpNMSprites.Add(153, My.Resources._15300)
        dpNMSprites.Add(154, My.Resources._15400)
        dpNMSprites.Add(155, My.Resources._15500)
        dpNMSprites.Add(156, My.Resources._15600)
        dpNMSprites.Add(157, My.Resources._15700)
        dpNMSprites.Add(158, My.Resources._15800)
        dpNMSprites.Add(159, My.Resources._15900)
        dpNMSprites.Add(160, My.Resources._16000)
        dpNMSprites.Add(161, My.Resources._16100)
        dpNMSprites.Add(162, My.Resources._16200)
        dpNMSprites.Add(163, My.Resources._16300)
        dpNMSprites.Add(164, My.Resources._16400)
        dpNMSprites.Add(165, My.Resources._16500)
        dpNMSprites.Add(166, My.Resources._16600)
        dpNMSprites.Add(167, My.Resources._16700)
        dpNMSprites.Add(168, My.Resources._16800)
        dpNMSprites.Add(169, My.Resources._16900)
        dpNMSprites.Add(170, My.Resources._17000)
        dpNMSprites.Add(171, My.Resources._17100)
        dpNMSprites.Add(172, My.Resources._17200)
        dpNMSprites.Add(173, My.Resources._17300)
        dpNMSprites.Add(174, My.Resources._17400)
        dpNMSprites.Add(175, My.Resources._17500)
        dpNMSprites.Add(176, My.Resources._17600)
        dpNMSprites.Add(177, My.Resources._17700)
        dpNMSprites.Add(178, My.Resources._17800)
        dpNMSprites.Add(179, My.Resources._17900)
        dpNMSprites.Add(180, My.Resources._18000)
        dpNMSprites.Add(181, My.Resources._18100)
        dpNMSprites.Add(182, My.Resources._18200)
        dpNMSprites.Add(183, My.Resources._18300)
        dpNMSprites.Add(184, My.Resources._18400)
        dpNMSprites.Add(185, My.Resources._18500)
        dpNMSprites.Add(186, My.Resources._18600)
        dpNMSprites.Add(187, My.Resources._18700)
        dpNMSprites.Add(188, My.Resources._18800)
        dpNMSprites.Add(189, My.Resources._18900)
        dpNMSprites.Add(190, My.Resources._19000)
        dpNMSprites.Add(191, My.Resources._19100)
        dpNMSprites.Add(192, My.Resources._19200)
        dpNMSprites.Add(193, My.Resources._19300)
        dpNMSprites.Add(194, My.Resources._19400)
        dpNMSprites.Add(195, My.Resources._19500)
        dpNMSprites.Add(196, My.Resources._19600)
        dpNMSprites.Add(197, My.Resources._19700)
        dpNMSprites.Add(198, My.Resources._19800)
        dpNMSprites.Add(199, My.Resources._19900)
        dpNMSprites.Add(200, My.Resources._20000)
        dpNMSprites.Add(201, My.Resources._20100)
        dpNMSprites.Add(202, My.Resources._20200)
        dpNMSprites.Add(203, My.Resources._20300)
        dpNMSprites.Add(204, My.Resources._20400)
        dpNMSprites.Add(205, My.Resources._20500)
        dpNMSprites.Add(206, My.Resources._20600)
        dpNMSprites.Add(207, My.Resources._20700)
        dpNMSprites.Add(208, My.Resources._20800)
        dpNMSprites.Add(209, My.Resources._20900)
        dpNMSprites.Add(210, My.Resources._21000)
        dpNMSprites.Add(211, My.Resources._21100)
        dpNMSprites.Add(212, My.Resources._21200)
        dpNMSprites.Add(213, My.Resources._21300)
        dpNMSprites.Add(214, My.Resources._21400)
        dpNMSprites.Add(215, My.Resources._21500)
        dpNMSprites.Add(216, My.Resources._21600)
        dpNMSprites.Add(217, My.Resources._21700)
        dpNMSprites.Add(218, My.Resources._21800)
        dpNMSprites.Add(219, My.Resources._21900)
        dpNMSprites.Add(220, My.Resources._22000)
        dpNMSprites.Add(221, My.Resources._22100)
        dpNMSprites.Add(222, My.Resources._22200)
        dpNMSprites.Add(223, My.Resources._22300)
        dpNMSprites.Add(224, My.Resources._22400)
        dpNMSprites.Add(225, My.Resources._22500)
        dpNMSprites.Add(226, My.Resources._22600)
        dpNMSprites.Add(227, My.Resources._22700)
        dpNMSprites.Add(228, My.Resources._22800)
        dpNMSprites.Add(229, My.Resources._22900)
        dpNMSprites.Add(230, My.Resources._23000)
        dpNMSprites.Add(231, My.Resources._23100)
        dpNMSprites.Add(232, My.Resources._23200)
        dpNMSprites.Add(233, My.Resources._23300)
        dpNMSprites.Add(234, My.Resources._23400)
        dpNMSprites.Add(235, My.Resources._23500)
        dpNMSprites.Add(236, My.Resources._23600)
        dpNMSprites.Add(237, My.Resources._23700)
        dpNMSprites.Add(238, My.Resources._23800)
        dpNMSprites.Add(239, My.Resources._23900)
        dpNMSprites.Add(240, My.Resources._24000)
        dpNMSprites.Add(241, My.Resources._24100)
        dpNMSprites.Add(242, My.Resources._24200)
        dpNMSprites.Add(243, My.Resources._24300)
        dpNMSprites.Add(244, My.Resources._24400)
        dpNMSprites.Add(245, My.Resources._24500)
        dpNMSprites.Add(246, My.Resources._24600)
        dpNMSprites.Add(247, My.Resources._24700)
        dpNMSprites.Add(248, My.Resources._24800)
        dpNMSprites.Add(249, My.Resources._24900)
        dpNMSprites.Add(250, My.Resources._25000)
        dpNMSprites.Add(251, My.Resources._25100)
        dpNMSprites.Add(252, My.Resources._25200)
        dpNMSprites.Add(253, My.Resources._25300)
        dpNMSprites.Add(254, My.Resources._25400)
        dpNMSprites.Add(255, My.Resources._25500)
        dpNMSprites.Add(256, My.Resources._25600)
        dpNMSprites.Add(257, My.Resources._25700)
        dpNMSprites.Add(258, My.Resources._25800)
        dpNMSprites.Add(259, My.Resources._25900)
        dpNMSprites.Add(260, My.Resources._26000)
        dpNMSprites.Add(261, My.Resources._26100)
        dpNMSprites.Add(262, My.Resources._26200)
        dpNMSprites.Add(263, My.Resources._26300)
        dpNMSprites.Add(264, My.Resources._26400)
        dpNMSprites.Add(265, My.Resources._26500)
        dpNMSprites.Add(266, My.Resources._26600)
        dpNMSprites.Add(267, My.Resources._26700)
        dpNMSprites.Add(268, My.Resources._26800)
        dpNMSprites.Add(269, My.Resources._26900)
        dpNMSprites.Add(270, My.Resources._27000)
        dpNMSprites.Add(271, My.Resources._27100)
        dpNMSprites.Add(272, My.Resources._27200)
        dpNMSprites.Add(273, My.Resources._27300)
        dpNMSprites.Add(274, My.Resources._27400)
        dpNMSprites.Add(275, My.Resources._27500)
        dpNMSprites.Add(276, My.Resources._27600)
        dpNMSprites.Add(277, My.Resources._27700)
        dpNMSprites.Add(278, My.Resources._27800)
        dpNMSprites.Add(279, My.Resources._27900)
        dpNMSprites.Add(280, My.Resources._28000)
        dpNMSprites.Add(281, My.Resources._28100)
        dpNMSprites.Add(282, My.Resources._28200)
        dpNMSprites.Add(283, My.Resources._28300)
        dpNMSprites.Add(284, My.Resources._28400)
        dpNMSprites.Add(285, My.Resources._28500)
        dpNMSprites.Add(286, My.Resources._28600)
        dpNMSprites.Add(287, My.Resources._28700)
        dpNMSprites.Add(288, My.Resources._28800)
        dpNMSprites.Add(289, My.Resources._28900)
        dpNMSprites.Add(290, My.Resources._29000)
        dpNMSprites.Add(291, My.Resources._29100)
        dpNMSprites.Add(292, My.Resources._29200)
        dpNMSprites.Add(293, My.Resources._29300)
        dpNMSprites.Add(294, My.Resources._29400)
        dpNMSprites.Add(295, My.Resources._29500)
        dpNMSprites.Add(296, My.Resources._29600)
        dpNMSprites.Add(297, My.Resources._29700)
        dpNMSprites.Add(298, My.Resources._29800)
        dpNMSprites.Add(299, My.Resources._29900)
        dpNMSprites.Add(300, My.Resources._30000)
        dpNMSprites.Add(301, My.Resources._30100)
        dpNMSprites.Add(302, My.Resources._30200)
        dpNMSprites.Add(303, My.Resources._30300)
        dpNMSprites.Add(304, My.Resources._30400)
        dpNMSprites.Add(305, My.Resources._30500)
        dpNMSprites.Add(306, My.Resources._30600)
        dpNMSprites.Add(307, My.Resources._30700)
        dpNMSprites.Add(308, My.Resources._30800)
        dpNMSprites.Add(309, My.Resources._30900)
        dpNMSprites.Add(310, My.Resources._31000)
        dpNMSprites.Add(311, My.Resources._31100)
        dpNMSprites.Add(312, My.Resources._31200)
        dpNMSprites.Add(313, My.Resources._31300)
        dpNMSprites.Add(314, My.Resources._31400)
        dpNMSprites.Add(315, My.Resources._31500)
        dpNMSprites.Add(316, My.Resources._31600)
        dpNMSprites.Add(317, My.Resources._31700)
        dpNMSprites.Add(318, My.Resources._31800)
        dpNMSprites.Add(319, My.Resources._31900)
        dpNMSprites.Add(320, My.Resources._32000)
        dpNMSprites.Add(321, My.Resources._32100)
        dpNMSprites.Add(322, My.Resources._32200)
        dpNMSprites.Add(323, My.Resources._32300)
        dpNMSprites.Add(324, My.Resources._32400)
        dpNMSprites.Add(325, My.Resources._32500)
        dpNMSprites.Add(326, My.Resources._32600)
        dpNMSprites.Add(327, My.Resources._32700)
        dpNMSprites.Add(328, My.Resources._32800)
        dpNMSprites.Add(329, My.Resources._32900)
        dpNMSprites.Add(330, My.Resources._33000)
        dpNMSprites.Add(331, My.Resources._33100)
        dpNMSprites.Add(332, My.Resources._33200)
        dpNMSprites.Add(333, My.Resources._33300)
        dpNMSprites.Add(334, My.Resources._33400)
        dpNMSprites.Add(335, My.Resources._33500)
        dpNMSprites.Add(336, My.Resources._33600)
        dpNMSprites.Add(337, My.Resources._33700)
        dpNMSprites.Add(338, My.Resources._33800)
        dpNMSprites.Add(339, My.Resources._33900)
        dpNMSprites.Add(340, My.Resources._34000)
        dpNMSprites.Add(341, My.Resources._34100)
        dpNMSprites.Add(342, My.Resources._34200)
        dpNMSprites.Add(343, My.Resources._34300)
        dpNMSprites.Add(344, My.Resources._34400)
        dpNMSprites.Add(345, My.Resources._34500)
        dpNMSprites.Add(346, My.Resources._34600)
        dpNMSprites.Add(347, My.Resources._34700)
        dpNMSprites.Add(348, My.Resources._34800)
        dpNMSprites.Add(349, My.Resources._34900)
        dpNMSprites.Add(350, My.Resources._35000)
        dpNMSprites.Add(351, My.Resources._35100)
        dpNMSprites.Add(352, My.Resources._35200)
        dpNMSprites.Add(353, My.Resources._35300)
        dpNMSprites.Add(354, My.Resources._35400)
        dpNMSprites.Add(355, My.Resources._35500)
        dpNMSprites.Add(356, My.Resources._35600)
        dpNMSprites.Add(357, My.Resources._35700)
        dpNMSprites.Add(358, My.Resources._35800)
        dpNMSprites.Add(359, My.Resources._35900)
        dpNMSprites.Add(360, My.Resources._36000)
        dpNMSprites.Add(361, My.Resources._36100)
        dpNMSprites.Add(362, My.Resources._36200)
        dpNMSprites.Add(363, My.Resources._36300)
        dpNMSprites.Add(364, My.Resources._36400)
        dpNMSprites.Add(365, My.Resources._36500)
        dpNMSprites.Add(366, My.Resources._36600)
        dpNMSprites.Add(367, My.Resources._36700)
        dpNMSprites.Add(368, My.Resources._36800)
        dpNMSprites.Add(369, My.Resources._36900)
        dpNMSprites.Add(370, My.Resources._37000)
        dpNMSprites.Add(371, My.Resources._37100)
        dpNMSprites.Add(372, My.Resources._37200)
        dpNMSprites.Add(373, My.Resources._37300)
        dpNMSprites.Add(374, My.Resources._37400)
        dpNMSprites.Add(375, My.Resources._37500)
        dpNMSprites.Add(376, My.Resources._37600)
        dpNMSprites.Add(377, My.Resources._37700)
        dpNMSprites.Add(378, My.Resources._37800)
        dpNMSprites.Add(379, My.Resources._37900)
        dpNMSprites.Add(380, My.Resources._38000)
        dpNMSprites.Add(381, My.Resources._38100)
        dpNMSprites.Add(382, My.Resources._38200)
        dpNMSprites.Add(383, My.Resources._38300)
        dpNMSprites.Add(384, My.Resources._38400)
        dpNMSprites.Add(385, My.Resources._38500)
        dpNMSprites.Add(386, My.Resources._38600)
        dpNMSprites.Add(387, My.Resources._38700)
        dpNMSprites.Add(388, My.Resources._38800)
        dpNMSprites.Add(389, My.Resources._38900)
        dpNMSprites.Add(390, My.Resources._39000)
        dpNMSprites.Add(391, My.Resources._39100)
        dpNMSprites.Add(392, My.Resources._39200)
        dpNMSprites.Add(393, My.Resources._39300)
        dpNMSprites.Add(394, My.Resources._39400)
        dpNMSprites.Add(395, My.Resources._39500)
        dpNMSprites.Add(396, My.Resources._39600)
        dpNMSprites.Add(397, My.Resources._39700)
        dpNMSprites.Add(398, My.Resources._39800)
        dpNMSprites.Add(399, My.Resources._39900)
        dpNMSprites.Add(400, My.Resources._40000)
        dpNMSprites.Add(401, My.Resources._40100)
        dpNMSprites.Add(402, My.Resources._40200)
        dpNMSprites.Add(403, My.Resources._40300)
        dpNMSprites.Add(404, My.Resources._40400)
        dpNMSprites.Add(405, My.Resources._40500)
        dpNMSprites.Add(406, My.Resources._40600)
        dpNMSprites.Add(407, My.Resources._40700)
        dpNMSprites.Add(408, My.Resources._40800)
        dpNMSprites.Add(409, My.Resources._40900)
        dpNMSprites.Add(410, My.Resources._41000)
        dpNMSprites.Add(411, My.Resources._41100)
        dpNMSprites.Add(412, My.Resources._41200)
        dpNMSprites.Add(413, My.Resources._41300)
        dpNMSprites.Add(414, My.Resources._41400)
        dpNMSprites.Add(415, My.Resources._41500)
        dpNMSprites.Add(416, My.Resources._41600)
        dpNMSprites.Add(417, My.Resources._41700)
        dpNMSprites.Add(418, My.Resources._41800)
        dpNMSprites.Add(419, My.Resources._41900)
        dpNMSprites.Add(420, My.Resources._42000)
        dpNMSprites.Add(421, My.Resources._42100)
        dpNMSprites.Add(422, My.Resources._42200)
        dpNMSprites.Add(423, My.Resources._42300)
        dpNMSprites.Add(424, My.Resources._42400)
        dpNMSprites.Add(425, My.Resources._42500)
        dpNMSprites.Add(426, My.Resources._42600)
        dpNMSprites.Add(427, My.Resources._42700)
        dpNMSprites.Add(428, My.Resources._42800)
        dpNMSprites.Add(429, My.Resources._42900)
        dpNMSprites.Add(430, My.Resources._43000)
        dpNMSprites.Add(431, My.Resources._43100)
        dpNMSprites.Add(432, My.Resources._43200)
        dpNMSprites.Add(433, My.Resources._43300)
        dpNMSprites.Add(434, My.Resources._43400)
        dpNMSprites.Add(435, My.Resources._43500)
        dpNMSprites.Add(436, My.Resources._43600)
        dpNMSprites.Add(437, My.Resources._43700)
        dpNMSprites.Add(438, My.Resources._43800)
        dpNMSprites.Add(439, My.Resources._43900)
        dpNMSprites.Add(440, My.Resources._44000)
        dpNMSprites.Add(441, My.Resources._44100)
        dpNMSprites.Add(442, My.Resources._44200)
        dpNMSprites.Add(443, My.Resources._44300)
        dpNMSprites.Add(444, My.Resources._44400)
        dpNMSprites.Add(445, My.Resources._44500)
        dpNMSprites.Add(446, My.Resources._44600)
        dpNMSprites.Add(447, My.Resources._44700)
        dpNMSprites.Add(448, My.Resources._44800)
        dpNMSprites.Add(449, My.Resources._44900)
        dpNMSprites.Add(450, My.Resources._45000)
        dpNMSprites.Add(451, My.Resources._45100)
        dpNMSprites.Add(452, My.Resources._45200)
        dpNMSprites.Add(453, My.Resources._45300)
        dpNMSprites.Add(454, My.Resources._45400)
        dpNMSprites.Add(455, My.Resources._45500)
        dpNMSprites.Add(456, My.Resources._45600)
        dpNMSprites.Add(457, My.Resources._45700)
        dpNMSprites.Add(458, My.Resources._45800)
        dpNMSprites.Add(459, My.Resources._45900)
        dpNMSprites.Add(460, My.Resources._46000)
        dpNMSprites.Add(461, My.Resources._46100)
        dpNMSprites.Add(462, My.Resources._46200)
        dpNMSprites.Add(463, My.Resources._46300)
        dpNMSprites.Add(464, My.Resources._46400)
        dpNMSprites.Add(465, My.Resources._46500)
        dpNMSprites.Add(466, My.Resources._46600)
        dpNMSprites.Add(467, My.Resources._46700)
        dpNMSprites.Add(468, My.Resources._46800)
        dpNMSprites.Add(469, My.Resources._46900)
        dpNMSprites.Add(470, My.Resources._47000)
        dpNMSprites.Add(471, My.Resources._47100)
        dpNMSprites.Add(472, My.Resources._47200)
        dpNMSprites.Add(473, My.Resources._47300)
        dpNMSprites.Add(474, My.Resources._47400)
        dpNMSprites.Add(475, My.Resources._47500)
        dpNMSprites.Add(476, My.Resources._47600)
        dpNMSprites.Add(477, My.Resources._47700)
        dpNMSprites.Add(478, My.Resources._47800)
        dpNMSprites.Add(479, My.Resources._47900)
        dpNMSprites.Add(480, My.Resources._48000)
        dpNMSprites.Add(481, My.Resources._48100)
        dpNMSprites.Add(482, My.Resources._48200)
        dpNMSprites.Add(483, My.Resources._48300)
        dpNMSprites.Add(484, My.Resources._48400)
        dpNMSprites.Add(485, My.Resources._48500)
        dpNMSprites.Add(486, My.Resources._48600)
        dpNMSprites.Add(487, My.Resources._48700)
        dpNMSprites.Add(488, My.Resources._48800)
        dpNMSprites.Add(489, My.Resources._48900)
        dpNMSprites.Add(490, My.Resources._49000)
        dpNMSprites.Add(491, My.Resources._49100)
        dpNMSprites.Add(492, My.Resources._49200)
        dpNMSprites.Add(493, My.Resources._49300)

        dpNFSprites.Add(0, Nothing)
        dpNFSprites.Add(1, My.Resources._00110)
        dpNFSprites.Add(2, My.Resources._00210)
        dpNFSprites.Add(3, My.Resources._00310)
        dpNFSprites.Add(4, My.Resources._00410)
        dpNFSprites.Add(5, My.Resources._00510)
        dpNFSprites.Add(6, My.Resources._00610)
        dpNFSprites.Add(7, My.Resources._00710)
        dpNFSprites.Add(8, My.Resources._00810)
        dpNFSprites.Add(9, My.Resources._00910)
        dpNFSprites.Add(10, My.Resources._01010)
        dpNFSprites.Add(11, My.Resources._01110)
        dpNFSprites.Add(12, My.Resources._01210)
        dpNFSprites.Add(13, My.Resources._01310)
        dpNFSprites.Add(14, My.Resources._01410)
        dpNFSprites.Add(15, My.Resources._01510)
        dpNFSprites.Add(16, My.Resources._01610)
        dpNFSprites.Add(17, My.Resources._01710)
        dpNFSprites.Add(18, My.Resources._01810)
        dpNFSprites.Add(19, My.Resources._01910)
        dpNFSprites.Add(20, My.Resources._02010)
        dpNFSprites.Add(21, My.Resources._02110)
        dpNFSprites.Add(22, My.Resources._02210)
        dpNFSprites.Add(23, My.Resources._02310)
        dpNFSprites.Add(24, My.Resources._02410)
        dpNFSprites.Add(25, My.Resources._02510)
        dpNFSprites.Add(26, My.Resources._02610)
        dpNFSprites.Add(27, My.Resources._02710)
        dpNFSprites.Add(28, My.Resources._02810)
        dpNFSprites.Add(29, My.Resources._02910)
        dpNFSprites.Add(30, My.Resources._03010)
        dpNFSprites.Add(31, My.Resources._03110)
        dpNFSprites.Add(32, My.Resources._03210)
        dpNFSprites.Add(33, My.Resources._03310)
        dpNFSprites.Add(34, My.Resources._03410)
        dpNFSprites.Add(35, My.Resources._03510)
        dpNFSprites.Add(36, My.Resources._03610)
        dpNFSprites.Add(37, My.Resources._03710)
        dpNFSprites.Add(38, My.Resources._03810)
        dpNFSprites.Add(39, My.Resources._03910)
        dpNFSprites.Add(40, My.Resources._04010)
        dpNFSprites.Add(41, My.Resources._04110)
        dpNFSprites.Add(42, My.Resources._04210)
        dpNFSprites.Add(43, My.Resources._04310)
        dpNFSprites.Add(44, My.Resources._04410)
        dpNFSprites.Add(45, My.Resources._04510)
        dpNFSprites.Add(46, My.Resources._04610)
        dpNFSprites.Add(47, My.Resources._04710)
        dpNFSprites.Add(48, My.Resources._04810)
        dpNFSprites.Add(49, My.Resources._04910)
        dpNFSprites.Add(50, My.Resources._05010)
        dpNFSprites.Add(51, My.Resources._05110)
        dpNFSprites.Add(52, My.Resources._05210)
        dpNFSprites.Add(53, My.Resources._05310)
        dpNFSprites.Add(54, My.Resources._05410)
        dpNFSprites.Add(55, My.Resources._05510)
        dpNFSprites.Add(56, My.Resources._05610)
        dpNFSprites.Add(57, My.Resources._05710)
        dpNFSprites.Add(58, My.Resources._05810)
        dpNFSprites.Add(59, My.Resources._05910)
        dpNFSprites.Add(60, My.Resources._06010)
        dpNFSprites.Add(61, My.Resources._06110)
        dpNFSprites.Add(62, My.Resources._06210)
        dpNFSprites.Add(63, My.Resources._06310)
        dpNFSprites.Add(64, My.Resources._06410)
        dpNFSprites.Add(65, My.Resources._06510)
        dpNFSprites.Add(66, My.Resources._06610)
        dpNFSprites.Add(67, My.Resources._06710)
        dpNFSprites.Add(68, My.Resources._06810)
        dpNFSprites.Add(69, My.Resources._06910)
        dpNFSprites.Add(70, My.Resources._07010)
        dpNFSprites.Add(71, My.Resources._07110)
        dpNFSprites.Add(72, My.Resources._07210)
        dpNFSprites.Add(73, My.Resources._07310)
        dpNFSprites.Add(74, My.Resources._07410)
        dpNFSprites.Add(75, My.Resources._07510)
        dpNFSprites.Add(76, My.Resources._07610)
        dpNFSprites.Add(77, My.Resources._07710)
        dpNFSprites.Add(78, My.Resources._07810)
        dpNFSprites.Add(79, My.Resources._07910)
        dpNFSprites.Add(80, My.Resources._08010)
        dpNFSprites.Add(81, My.Resources._08110)
        dpNFSprites.Add(82, My.Resources._08210)
        dpNFSprites.Add(83, My.Resources._08310)
        dpNFSprites.Add(84, My.Resources._08410)
        dpNFSprites.Add(85, My.Resources._08510)
        dpNFSprites.Add(86, My.Resources._08610)
        dpNFSprites.Add(87, My.Resources._08710)
        dpNFSprites.Add(88, My.Resources._08810)
        dpNFSprites.Add(89, My.Resources._08910)
        dpNFSprites.Add(90, My.Resources._09010)
        dpNFSprites.Add(91, My.Resources._09110)
        dpNFSprites.Add(92, My.Resources._09210)
        dpNFSprites.Add(93, My.Resources._09310)
        dpNFSprites.Add(94, My.Resources._09410)
        dpNFSprites.Add(95, My.Resources._09510)
        dpNFSprites.Add(96, My.Resources._09610)
        dpNFSprites.Add(97, My.Resources._09710)
        dpNFSprites.Add(98, My.Resources._09810)
        dpNFSprites.Add(99, My.Resources._09910)
        dpNFSprites.Add(100, My.Resources._10010)
        dpNFSprites.Add(101, My.Resources._10110)
        dpNFSprites.Add(102, My.Resources._10210)
        dpNFSprites.Add(103, My.Resources._10310)
        dpNFSprites.Add(104, My.Resources._10410)
        dpNFSprites.Add(105, My.Resources._10510)
        dpNFSprites.Add(106, My.Resources._10610)
        dpNFSprites.Add(107, My.Resources._10710)
        dpNFSprites.Add(108, My.Resources._10810)
        dpNFSprites.Add(109, My.Resources._10910)
        dpNFSprites.Add(110, My.Resources._11010)
        dpNFSprites.Add(111, My.Resources._11110)
        dpNFSprites.Add(112, My.Resources._11210)
        dpNFSprites.Add(113, My.Resources._11310)
        dpNFSprites.Add(114, My.Resources._11410)
        dpNFSprites.Add(115, My.Resources._11510)
        dpNFSprites.Add(116, My.Resources._11610)
        dpNFSprites.Add(117, My.Resources._11710)
        dpNFSprites.Add(118, My.Resources._11810)
        dpNFSprites.Add(119, My.Resources._11910)
        dpNFSprites.Add(120, My.Resources._12010)
        dpNFSprites.Add(121, My.Resources._12110)
        dpNFSprites.Add(122, My.Resources._12210)
        dpNFSprites.Add(123, My.Resources._12310)
        dpNFSprites.Add(124, My.Resources._12410)
        dpNFSprites.Add(125, My.Resources._12510)
        dpNFSprites.Add(126, My.Resources._12610)
        dpNFSprites.Add(127, My.Resources._12710)
        dpNFSprites.Add(128, My.Resources._12810)
        dpNFSprites.Add(129, My.Resources._12910)
        dpNFSprites.Add(130, My.Resources._13010)
        dpNFSprites.Add(131, My.Resources._13110)
        dpNFSprites.Add(132, My.Resources._13210)
        dpNFSprites.Add(133, My.Resources._13310)
        dpNFSprites.Add(134, My.Resources._13410)
        dpNFSprites.Add(135, My.Resources._13510)
        dpNFSprites.Add(136, My.Resources._13610)
        dpNFSprites.Add(137, My.Resources._13710)
        dpNFSprites.Add(138, My.Resources._13810)
        dpNFSprites.Add(139, My.Resources._13910)
        dpNFSprites.Add(140, My.Resources._14010)
        dpNFSprites.Add(141, My.Resources._14110)
        dpNFSprites.Add(142, My.Resources._14210)
        dpNFSprites.Add(143, My.Resources._14310)
        dpNFSprites.Add(144, My.Resources._14410)
        dpNFSprites.Add(145, My.Resources._14510)
        dpNFSprites.Add(146, My.Resources._14610)
        dpNFSprites.Add(147, My.Resources._14710)
        dpNFSprites.Add(148, My.Resources._14810)
        dpNFSprites.Add(149, My.Resources._14910)
        dpNFSprites.Add(150, My.Resources._15010)
        dpNFSprites.Add(151, My.Resources._15110)
        dpNFSprites.Add(152, My.Resources._15210)
        dpNFSprites.Add(153, My.Resources._15310)
        dpNFSprites.Add(154, My.Resources._15410)
        dpNFSprites.Add(155, My.Resources._15510)
        dpNFSprites.Add(156, My.Resources._15610)
        dpNFSprites.Add(157, My.Resources._15710)
        dpNFSprites.Add(158, My.Resources._15810)
        dpNFSprites.Add(159, My.Resources._15910)
        dpNFSprites.Add(160, My.Resources._16010)
        dpNFSprites.Add(161, My.Resources._16110)
        dpNFSprites.Add(162, My.Resources._16210)
        dpNFSprites.Add(163, My.Resources._16310)
        dpNFSprites.Add(164, My.Resources._16410)
        dpNFSprites.Add(165, My.Resources._16510)
        dpNFSprites.Add(166, My.Resources._16610)
        dpNFSprites.Add(167, My.Resources._16710)
        dpNFSprites.Add(168, My.Resources._16810)
        dpNFSprites.Add(169, My.Resources._16910)
        dpNFSprites.Add(170, My.Resources._17010)
        dpNFSprites.Add(171, My.Resources._17110)
        dpNFSprites.Add(172, My.Resources._17210)
        dpNFSprites.Add(173, My.Resources._17310)
        dpNFSprites.Add(174, My.Resources._17410)
        dpNFSprites.Add(175, My.Resources._17510)
        dpNFSprites.Add(176, My.Resources._17610)
        dpNFSprites.Add(177, My.Resources._17710)
        dpNFSprites.Add(178, My.Resources._17810)
        dpNFSprites.Add(179, My.Resources._17910)
        dpNFSprites.Add(180, My.Resources._18010)
        dpNFSprites.Add(181, My.Resources._18110)
        dpNFSprites.Add(182, My.Resources._18210)
        dpNFSprites.Add(183, My.Resources._18310)
        dpNFSprites.Add(184, My.Resources._18410)
        dpNFSprites.Add(185, My.Resources._18510)
        dpNFSprites.Add(186, My.Resources._18610)
        dpNFSprites.Add(187, My.Resources._18710)
        dpNFSprites.Add(188, My.Resources._18810)
        dpNFSprites.Add(189, My.Resources._18910)
        dpNFSprites.Add(190, My.Resources._19010)
        dpNFSprites.Add(191, My.Resources._19110)
        dpNFSprites.Add(192, My.Resources._19210)
        dpNFSprites.Add(193, My.Resources._19310)
        dpNFSprites.Add(194, My.Resources._19410)
        dpNFSprites.Add(195, My.Resources._19510)
        dpNFSprites.Add(196, My.Resources._19610)
        dpNFSprites.Add(197, My.Resources._19710)
        dpNFSprites.Add(198, My.Resources._19810)
        dpNFSprites.Add(199, My.Resources._19910)
        dpNFSprites.Add(200, My.Resources._20010)
        dpNFSprites.Add(201, My.Resources._20110)
        dpNFSprites.Add(202, My.Resources._20210)
        dpNFSprites.Add(203, My.Resources._20310)
        dpNFSprites.Add(204, My.Resources._20410)
        dpNFSprites.Add(205, My.Resources._20510)
        dpNFSprites.Add(206, My.Resources._20610)
        dpNFSprites.Add(207, My.Resources._20710)
        dpNFSprites.Add(208, My.Resources._20810)
        dpNFSprites.Add(209, My.Resources._20910)
        dpNFSprites.Add(210, My.Resources._21010)
        dpNFSprites.Add(211, My.Resources._21110)
        dpNFSprites.Add(212, My.Resources._21210)
        dpNFSprites.Add(213, My.Resources._21310)
        dpNFSprites.Add(214, My.Resources._21410)
        dpNFSprites.Add(215, My.Resources._21510)
        dpNFSprites.Add(216, My.Resources._21610)
        dpNFSprites.Add(217, My.Resources._21710)
        dpNFSprites.Add(218, My.Resources._21810)
        dpNFSprites.Add(219, My.Resources._21910)
        dpNFSprites.Add(220, My.Resources._22010)
        dpNFSprites.Add(221, My.Resources._22110)
        dpNFSprites.Add(222, My.Resources._22210)
        dpNFSprites.Add(223, My.Resources._22310)
        dpNFSprites.Add(224, My.Resources._22410)
        dpNFSprites.Add(225, My.Resources._22510)
        dpNFSprites.Add(226, My.Resources._22610)
        dpNFSprites.Add(227, My.Resources._22710)
        dpNFSprites.Add(228, My.Resources._22810)
        dpNFSprites.Add(229, My.Resources._22910)
        dpNFSprites.Add(230, My.Resources._23010)
        dpNFSprites.Add(231, My.Resources._23110)
        dpNFSprites.Add(232, My.Resources._23210)
        dpNFSprites.Add(233, My.Resources._23310)
        dpNFSprites.Add(234, My.Resources._23410)
        dpNFSprites.Add(235, My.Resources._23510)
        dpNFSprites.Add(236, My.Resources._23610)
        dpNFSprites.Add(237, My.Resources._23710)
        dpNFSprites.Add(238, My.Resources._23810)
        dpNFSprites.Add(239, My.Resources._23910)
        dpNFSprites.Add(240, My.Resources._24010)
        dpNFSprites.Add(241, My.Resources._24110)
        dpNFSprites.Add(242, My.Resources._24210)
        dpNFSprites.Add(243, My.Resources._24310)
        dpNFSprites.Add(244, My.Resources._24410)
        dpNFSprites.Add(245, My.Resources._24510)
        dpNFSprites.Add(246, My.Resources._24610)
        dpNFSprites.Add(247, My.Resources._24710)
        dpNFSprites.Add(248, My.Resources._24810)
        dpNFSprites.Add(249, My.Resources._24910)
        dpNFSprites.Add(250, My.Resources._25010)
        dpNFSprites.Add(251, My.Resources._25110)
        dpNFSprites.Add(252, My.Resources._25210)
        dpNFSprites.Add(253, My.Resources._25310)
        dpNFSprites.Add(254, My.Resources._25410)
        dpNFSprites.Add(255, My.Resources._25510)
        dpNFSprites.Add(256, My.Resources._25610)
        dpNFSprites.Add(257, My.Resources._25710)
        dpNFSprites.Add(258, My.Resources._25810)
        dpNFSprites.Add(259, My.Resources._25910)
        dpNFSprites.Add(260, My.Resources._26010)
        dpNFSprites.Add(261, My.Resources._26110)
        dpNFSprites.Add(262, My.Resources._26210)
        dpNFSprites.Add(263, My.Resources._26310)
        dpNFSprites.Add(264, My.Resources._26410)
        dpNFSprites.Add(265, My.Resources._26510)
        dpNFSprites.Add(266, My.Resources._26610)
        dpNFSprites.Add(267, My.Resources._26710)
        dpNFSprites.Add(268, My.Resources._26810)
        dpNFSprites.Add(269, My.Resources._26910)
        dpNFSprites.Add(270, My.Resources._27010)
        dpNFSprites.Add(271, My.Resources._27110)
        dpNFSprites.Add(272, My.Resources._27210)
        dpNFSprites.Add(273, My.Resources._27310)
        dpNFSprites.Add(274, My.Resources._27410)
        dpNFSprites.Add(275, My.Resources._27510)
        dpNFSprites.Add(276, My.Resources._27610)
        dpNFSprites.Add(277, My.Resources._27710)
        dpNFSprites.Add(278, My.Resources._27810)
        dpNFSprites.Add(279, My.Resources._27910)
        dpNFSprites.Add(280, My.Resources._28010)
        dpNFSprites.Add(281, My.Resources._28110)
        dpNFSprites.Add(282, My.Resources._28210)
        dpNFSprites.Add(283, My.Resources._28310)
        dpNFSprites.Add(284, My.Resources._28410)
        dpNFSprites.Add(285, My.Resources._28510)
        dpNFSprites.Add(286, My.Resources._28610)
        dpNFSprites.Add(287, My.Resources._28710)
        dpNFSprites.Add(288, My.Resources._28810)
        dpNFSprites.Add(289, My.Resources._28910)
        dpNFSprites.Add(290, My.Resources._29010)
        dpNFSprites.Add(291, My.Resources._29110)
        dpNFSprites.Add(292, My.Resources._29210)
        dpNFSprites.Add(293, My.Resources._29310)
        dpNFSprites.Add(294, My.Resources._29410)
        dpNFSprites.Add(295, My.Resources._29510)
        dpNFSprites.Add(296, My.Resources._29610)
        dpNFSprites.Add(297, My.Resources._29710)
        dpNFSprites.Add(298, My.Resources._29810)
        dpNFSprites.Add(299, My.Resources._29910)
        dpNFSprites.Add(300, My.Resources._30010)
        dpNFSprites.Add(301, My.Resources._30110)
        dpNFSprites.Add(302, My.Resources._30210)
        dpNFSprites.Add(303, My.Resources._30310)
        dpNFSprites.Add(304, My.Resources._30410)
        dpNFSprites.Add(305, My.Resources._30510)
        dpNFSprites.Add(306, My.Resources._30610)
        dpNFSprites.Add(307, My.Resources._30710)
        dpNFSprites.Add(308, My.Resources._30810)
        dpNFSprites.Add(309, My.Resources._30910)
        dpNFSprites.Add(310, My.Resources._31010)
        dpNFSprites.Add(311, My.Resources._31110)
        dpNFSprites.Add(312, My.Resources._31210)
        dpNFSprites.Add(313, My.Resources._31310)
        dpNFSprites.Add(314, My.Resources._31410)
        dpNFSprites.Add(315, My.Resources._31510)
        dpNFSprites.Add(316, My.Resources._31610)
        dpNFSprites.Add(317, My.Resources._31710)
        dpNFSprites.Add(318, My.Resources._31810)
        dpNFSprites.Add(319, My.Resources._31910)
        dpNFSprites.Add(320, My.Resources._32010)
        dpNFSprites.Add(321, My.Resources._32110)
        dpNFSprites.Add(322, My.Resources._32210)
        dpNFSprites.Add(323, My.Resources._32310)
        dpNFSprites.Add(324, My.Resources._32410)
        dpNFSprites.Add(325, My.Resources._32510)
        dpNFSprites.Add(326, My.Resources._32610)
        dpNFSprites.Add(327, My.Resources._32710)
        dpNFSprites.Add(328, My.Resources._32810)
        dpNFSprites.Add(329, My.Resources._32910)
        dpNFSprites.Add(330, My.Resources._33010)
        dpNFSprites.Add(331, My.Resources._33110)
        dpNFSprites.Add(332, My.Resources._33210)
        dpNFSprites.Add(333, My.Resources._33310)
        dpNFSprites.Add(334, My.Resources._33410)
        dpNFSprites.Add(335, My.Resources._33510)
        dpNFSprites.Add(336, My.Resources._33610)
        dpNFSprites.Add(337, My.Resources._33710)
        dpNFSprites.Add(338, My.Resources._33810)
        dpNFSprites.Add(339, My.Resources._33910)
        dpNFSprites.Add(340, My.Resources._34010)
        dpNFSprites.Add(341, My.Resources._34110)
        dpNFSprites.Add(342, My.Resources._34210)
        dpNFSprites.Add(343, My.Resources._34310)
        dpNFSprites.Add(344, My.Resources._34410)
        dpNFSprites.Add(345, My.Resources._34510)
        dpNFSprites.Add(346, My.Resources._34610)
        dpNFSprites.Add(347, My.Resources._34710)
        dpNFSprites.Add(348, My.Resources._34810)
        dpNFSprites.Add(349, My.Resources._34910)
        dpNFSprites.Add(350, My.Resources._35010)
        dpNFSprites.Add(351, My.Resources._35110)
        dpNFSprites.Add(352, My.Resources._35210)
        dpNFSprites.Add(353, My.Resources._35310)
        dpNFSprites.Add(354, My.Resources._35410)
        dpNFSprites.Add(355, My.Resources._35510)
        dpNFSprites.Add(356, My.Resources._35610)
        dpNFSprites.Add(357, My.Resources._35710)
        dpNFSprites.Add(358, My.Resources._35810)
        dpNFSprites.Add(359, My.Resources._35910)
        dpNFSprites.Add(360, My.Resources._36010)
        dpNFSprites.Add(361, My.Resources._36110)
        dpNFSprites.Add(362, My.Resources._36210)
        dpNFSprites.Add(363, My.Resources._36310)
        dpNFSprites.Add(364, My.Resources._36410)
        dpNFSprites.Add(365, My.Resources._36510)
        dpNFSprites.Add(366, My.Resources._36610)
        dpNFSprites.Add(367, My.Resources._36710)
        dpNFSprites.Add(368, My.Resources._36810)
        dpNFSprites.Add(369, My.Resources._36910)
        dpNFSprites.Add(370, My.Resources._37010)
        dpNFSprites.Add(371, My.Resources._37110)
        dpNFSprites.Add(372, My.Resources._37210)
        dpNFSprites.Add(373, My.Resources._37310)
        dpNFSprites.Add(374, My.Resources._37410)
        dpNFSprites.Add(375, My.Resources._37510)
        dpNFSprites.Add(376, My.Resources._37610)
        dpNFSprites.Add(377, My.Resources._37710)
        dpNFSprites.Add(378, My.Resources._37810)
        dpNFSprites.Add(379, My.Resources._37910)
        dpNFSprites.Add(380, My.Resources._38010)
        dpNFSprites.Add(381, My.Resources._38110)
        dpNFSprites.Add(382, My.Resources._38210)
        dpNFSprites.Add(383, My.Resources._38310)
        dpNFSprites.Add(384, My.Resources._38410)
        dpNFSprites.Add(385, My.Resources._38510)
        dpNFSprites.Add(386, My.Resources._38610)
        dpNFSprites.Add(387, My.Resources._38710)
        dpNFSprites.Add(388, My.Resources._38810)
        dpNFSprites.Add(389, My.Resources._38910)
        dpNFSprites.Add(390, My.Resources._39010)
        dpNFSprites.Add(391, My.Resources._39110)
        dpNFSprites.Add(392, My.Resources._39210)
        dpNFSprites.Add(393, My.Resources._39310)
        dpNFSprites.Add(394, My.Resources._39410)
        dpNFSprites.Add(395, My.Resources._39510)
        dpNFSprites.Add(396, My.Resources._39610)
        dpNFSprites.Add(397, My.Resources._39710)
        dpNFSprites.Add(398, My.Resources._39810)
        dpNFSprites.Add(399, My.Resources._39910)
        dpNFSprites.Add(400, My.Resources._40010)
        dpNFSprites.Add(401, My.Resources._40110)
        dpNFSprites.Add(402, My.Resources._40210)
        dpNFSprites.Add(403, My.Resources._40310)
        dpNFSprites.Add(404, My.Resources._40410)
        dpNFSprites.Add(405, My.Resources._40510)
        dpNFSprites.Add(406, My.Resources._40610)
        dpNFSprites.Add(407, My.Resources._40710)
        dpNFSprites.Add(408, My.Resources._40810)
        dpNFSprites.Add(409, My.Resources._40910)
        dpNFSprites.Add(410, My.Resources._41010)
        dpNFSprites.Add(411, My.Resources._41110)
        dpNFSprites.Add(412, My.Resources._41210)
        dpNFSprites.Add(413, My.Resources._41310)
        dpNFSprites.Add(414, My.Resources._41410)
        dpNFSprites.Add(415, My.Resources._41510)
        dpNFSprites.Add(416, My.Resources._41610)
        dpNFSprites.Add(417, My.Resources._41710)
        dpNFSprites.Add(418, My.Resources._41810)
        dpNFSprites.Add(419, My.Resources._41910)
        dpNFSprites.Add(420, My.Resources._42010)
        dpNFSprites.Add(421, My.Resources._42110)
        dpNFSprites.Add(422, My.Resources._42210)
        dpNFSprites.Add(423, My.Resources._42310)
        dpNFSprites.Add(424, My.Resources._42410)
        dpNFSprites.Add(425, My.Resources._42510)
        dpNFSprites.Add(426, My.Resources._42610)
        dpNFSprites.Add(427, My.Resources._42710)
        dpNFSprites.Add(428, My.Resources._42810)
        dpNFSprites.Add(429, My.Resources._42910)
        dpNFSprites.Add(430, My.Resources._43010)
        dpNFSprites.Add(431, My.Resources._43110)
        dpNFSprites.Add(432, My.Resources._43210)
        dpNFSprites.Add(433, My.Resources._43310)
        dpNFSprites.Add(434, My.Resources._43410)
        dpNFSprites.Add(435, My.Resources._43510)
        dpNFSprites.Add(436, My.Resources._43610)
        dpNFSprites.Add(437, My.Resources._43710)
        dpNFSprites.Add(438, My.Resources._43810)
        dpNFSprites.Add(439, My.Resources._43910)
        dpNFSprites.Add(440, My.Resources._44010)
        dpNFSprites.Add(441, My.Resources._44110)
        dpNFSprites.Add(442, My.Resources._44210)
        dpNFSprites.Add(443, My.Resources._44310)
        dpNFSprites.Add(444, My.Resources._44410)
        dpNFSprites.Add(445, My.Resources._44510)
        dpNFSprites.Add(446, My.Resources._44610)
        dpNFSprites.Add(447, My.Resources._44710)
        dpNFSprites.Add(448, My.Resources._44810)
        dpNFSprites.Add(449, My.Resources._44910)
        dpNFSprites.Add(450, My.Resources._45010)
        dpNFSprites.Add(451, My.Resources._45110)
        dpNFSprites.Add(452, My.Resources._45210)
        dpNFSprites.Add(453, My.Resources._45310)
        dpNFSprites.Add(454, My.Resources._45410)
        dpNFSprites.Add(455, My.Resources._45510)
        dpNFSprites.Add(456, My.Resources._45610)
        dpNFSprites.Add(457, My.Resources._45710)
        dpNFSprites.Add(458, My.Resources._45810)
        dpNFSprites.Add(459, My.Resources._45910)
        dpNFSprites.Add(460, My.Resources._46010)
        dpNFSprites.Add(461, My.Resources._46110)
        dpNFSprites.Add(462, My.Resources._46210)
        dpNFSprites.Add(463, My.Resources._46310)
        dpNFSprites.Add(464, My.Resources._46410)
        dpNFSprites.Add(465, My.Resources._46510)
        dpNFSprites.Add(466, My.Resources._46610)
        dpNFSprites.Add(467, My.Resources._46710)
        dpNFSprites.Add(468, My.Resources._46810)
        dpNFSprites.Add(469, My.Resources._46910)
        dpNFSprites.Add(470, My.Resources._47010)
        dpNFSprites.Add(471, My.Resources._47110)
        dpNFSprites.Add(472, My.Resources._47210)
        dpNFSprites.Add(473, My.Resources._47310)
        dpNFSprites.Add(474, My.Resources._47410)
        dpNFSprites.Add(475, My.Resources._47510)
        dpNFSprites.Add(476, My.Resources._47610)
        dpNFSprites.Add(477, My.Resources._47710)
        dpNFSprites.Add(478, My.Resources._47810)
        dpNFSprites.Add(479, My.Resources._47910)
        dpNFSprites.Add(480, My.Resources._48010)
        dpNFSprites.Add(481, My.Resources._48110)
        dpNFSprites.Add(482, My.Resources._48210)
        dpNFSprites.Add(483, My.Resources._48310)
        dpNFSprites.Add(484, My.Resources._48410)
        dpNFSprites.Add(485, My.Resources._48510)
        dpNFSprites.Add(486, My.Resources._48610)
        dpNFSprites.Add(487, My.Resources._48710)
        dpNFSprites.Add(488, My.Resources._48810)
        dpNFSprites.Add(489, My.Resources._48910)
        dpNFSprites.Add(490, My.Resources._49010)
        dpNFSprites.Add(491, My.Resources._49110)
        dpNFSprites.Add(492, My.Resources._49210)
        dpNFSprites.Add(493, My.Resources._49310)

        dpSMSprites.Add(0, Nothing)
        dpSMSprites.Add(1, My.Resources._00101)
        dpSMSprites.Add(2, My.Resources._00201)
        dpSMSprites.Add(3, My.Resources._00301)
        dpSMSprites.Add(4, My.Resources._00401)
        dpSMSprites.Add(5, My.Resources._00501)
        dpSMSprites.Add(6, My.Resources._00601)
        dpSMSprites.Add(7, My.Resources._00701)
        dpSMSprites.Add(8, My.Resources._00801)
        dpSMSprites.Add(9, My.Resources._00901)
        dpSMSprites.Add(10, My.Resources._01001)
        dpSMSprites.Add(11, My.Resources._01101)
        dpSMSprites.Add(12, My.Resources._01201)
        dpSMSprites.Add(13, My.Resources._01301)
        dpSMSprites.Add(14, My.Resources._01401)
        dpSMSprites.Add(15, My.Resources._01501)
        dpSMSprites.Add(16, My.Resources._01601)
        dpSMSprites.Add(17, My.Resources._01701)
        dpSMSprites.Add(18, My.Resources._01801)
        dpSMSprites.Add(19, My.Resources._01901)
        dpSMSprites.Add(20, My.Resources._02001)
        dpSMSprites.Add(21, My.Resources._02101)
        dpSMSprites.Add(22, My.Resources._02201)
        dpSMSprites.Add(23, My.Resources._02301)
        dpSMSprites.Add(24, My.Resources._02401)
        dpSMSprites.Add(25, My.Resources._02501)
        dpSMSprites.Add(26, My.Resources._02601)
        dpSMSprites.Add(27, My.Resources._02701)
        dpSMSprites.Add(28, My.Resources._02801)
        dpSMSprites.Add(29, My.Resources._02901)
        dpSMSprites.Add(30, My.Resources._03001)
        dpSMSprites.Add(31, My.Resources._03101)
        dpSMSprites.Add(32, My.Resources._03201)
        dpSMSprites.Add(33, My.Resources._03301)
        dpSMSprites.Add(34, My.Resources._03401)
        dpSMSprites.Add(35, My.Resources._03501)
        dpSMSprites.Add(36, My.Resources._03601)
        dpSMSprites.Add(37, My.Resources._03701)
        dpSMSprites.Add(38, My.Resources._03801)
        dpSMSprites.Add(39, My.Resources._03901)
        dpSMSprites.Add(40, My.Resources._04001)
        dpSMSprites.Add(41, My.Resources._04101)
        dpSMSprites.Add(42, My.Resources._04201)
        dpSMSprites.Add(43, My.Resources._04301)
        dpSMSprites.Add(44, My.Resources._04401)
        dpSMSprites.Add(45, My.Resources._04501)
        dpSMSprites.Add(46, My.Resources._04601)
        dpSMSprites.Add(47, My.Resources._04701)
        dpSMSprites.Add(48, My.Resources._04801)
        dpSMSprites.Add(49, My.Resources._04901)
        dpSMSprites.Add(50, My.Resources._05001)
        dpSMSprites.Add(51, My.Resources._05101)
        dpSMSprites.Add(52, My.Resources._05201)
        dpSMSprites.Add(53, My.Resources._05301)
        dpSMSprites.Add(54, My.Resources._05401)
        dpSMSprites.Add(55, My.Resources._05501)
        dpSMSprites.Add(56, My.Resources._05601)
        dpSMSprites.Add(57, My.Resources._05701)
        dpSMSprites.Add(58, My.Resources._05801)
        dpSMSprites.Add(59, My.Resources._05901)
        dpSMSprites.Add(60, My.Resources._06001)
        dpSMSprites.Add(61, My.Resources._06101)
        dpSMSprites.Add(62, My.Resources._06201)
        dpSMSprites.Add(63, My.Resources._06301)
        dpSMSprites.Add(64, My.Resources._06401)
        dpSMSprites.Add(65, My.Resources._06501)
        dpSMSprites.Add(66, My.Resources._06601)
        dpSMSprites.Add(67, My.Resources._06701)
        dpSMSprites.Add(68, My.Resources._06801)
        dpSMSprites.Add(69, My.Resources._06901)
        dpSMSprites.Add(70, My.Resources._07001)
        dpSMSprites.Add(71, My.Resources._07101)
        dpSMSprites.Add(72, My.Resources._07201)
        dpSMSprites.Add(73, My.Resources._07301)
        dpSMSprites.Add(74, My.Resources._07401)
        dpSMSprites.Add(75, My.Resources._07501)
        dpSMSprites.Add(76, My.Resources._07601)
        dpSMSprites.Add(77, My.Resources._07701)
        dpSMSprites.Add(78, My.Resources._07801)
        dpSMSprites.Add(79, My.Resources._07901)
        dpSMSprites.Add(80, My.Resources._08001)
        dpSMSprites.Add(81, My.Resources._08101)
        dpSMSprites.Add(82, My.Resources._08201)
        dpSMSprites.Add(83, My.Resources._08301)
        dpSMSprites.Add(84, My.Resources._08401)
        dpSMSprites.Add(85, My.Resources._08501)
        dpSMSprites.Add(86, My.Resources._08601)
        dpSMSprites.Add(87, My.Resources._08701)
        dpSMSprites.Add(88, My.Resources._08801)
        dpSMSprites.Add(89, My.Resources._08901)
        dpSMSprites.Add(90, My.Resources._09001)
        dpSMSprites.Add(91, My.Resources._09101)
        dpSMSprites.Add(92, My.Resources._09201)
        dpSMSprites.Add(93, My.Resources._09301)
        dpSMSprites.Add(94, My.Resources._09401)
        dpSMSprites.Add(95, My.Resources._09501)
        dpSMSprites.Add(96, My.Resources._09601)
        dpSMSprites.Add(97, My.Resources._09701)
        dpSMSprites.Add(98, My.Resources._09801)
        dpSMSprites.Add(99, My.Resources._09901)
        dpSMSprites.Add(100, My.Resources._10001)
        dpSMSprites.Add(101, My.Resources._10101)
        dpSMSprites.Add(102, My.Resources._10201)
        dpSMSprites.Add(103, My.Resources._10301)
        dpSMSprites.Add(104, My.Resources._10401)
        dpSMSprites.Add(105, My.Resources._10501)
        dpSMSprites.Add(106, My.Resources._10601)
        dpSMSprites.Add(107, My.Resources._10701)
        dpSMSprites.Add(108, My.Resources._10801)
        dpSMSprites.Add(109, My.Resources._10901)
        dpSMSprites.Add(110, My.Resources._11001)
        dpSMSprites.Add(111, My.Resources._11101)
        dpSMSprites.Add(112, My.Resources._11201)
        dpSMSprites.Add(113, My.Resources._11301)
        dpSMSprites.Add(114, My.Resources._11401)
        dpSMSprites.Add(115, My.Resources._11501)
        dpSMSprites.Add(116, My.Resources._11601)
        dpSMSprites.Add(117, My.Resources._11701)
        dpSMSprites.Add(118, My.Resources._11801)
        dpSMSprites.Add(119, My.Resources._11901)
        dpSMSprites.Add(120, My.Resources._12001)
        dpSMSprites.Add(121, My.Resources._12101)
        dpSMSprites.Add(122, My.Resources._12201)
        dpSMSprites.Add(123, My.Resources._12301)
        dpSMSprites.Add(124, My.Resources._12401)
        dpSMSprites.Add(125, My.Resources._12501)
        dpSMSprites.Add(126, My.Resources._12601)
        dpSMSprites.Add(127, My.Resources._12701)
        dpSMSprites.Add(128, My.Resources._12801)
        dpSMSprites.Add(129, My.Resources._12901)
        dpSMSprites.Add(130, My.Resources._13001)
        dpSMSprites.Add(131, My.Resources._13101)
        dpSMSprites.Add(132, My.Resources._13201)
        dpSMSprites.Add(133, My.Resources._13301)
        dpSMSprites.Add(134, My.Resources._13401)
        dpSMSprites.Add(135, My.Resources._13501)
        dpSMSprites.Add(136, My.Resources._13601)
        dpSMSprites.Add(137, My.Resources._13701)
        dpSMSprites.Add(138, My.Resources._13801)
        dpSMSprites.Add(139, My.Resources._13901)
        dpSMSprites.Add(140, My.Resources._14001)
        dpSMSprites.Add(141, My.Resources._14101)
        dpSMSprites.Add(142, My.Resources._14201)
        dpSMSprites.Add(143, My.Resources._14301)
        dpSMSprites.Add(144, My.Resources._14401)
        dpSMSprites.Add(145, My.Resources._14501)
        dpSMSprites.Add(146, My.Resources._14601)
        dpSMSprites.Add(147, My.Resources._14701)
        dpSMSprites.Add(148, My.Resources._14801)
        dpSMSprites.Add(149, My.Resources._14901)
        dpSMSprites.Add(150, My.Resources._15001)
        dpSMSprites.Add(151, My.Resources._15101)
        dpSMSprites.Add(152, My.Resources._15201)
        dpSMSprites.Add(153, My.Resources._15301)
        dpSMSprites.Add(154, My.Resources._15401)
        dpSMSprites.Add(155, My.Resources._15501)
        dpSMSprites.Add(156, My.Resources._15601)
        dpSMSprites.Add(157, My.Resources._15701)
        dpSMSprites.Add(158, My.Resources._15801)
        dpSMSprites.Add(159, My.Resources._15901)
        dpSMSprites.Add(160, My.Resources._16001)
        dpSMSprites.Add(161, My.Resources._16101)
        dpSMSprites.Add(162, My.Resources._16201)
        dpSMSprites.Add(163, My.Resources._16301)
        dpSMSprites.Add(164, My.Resources._16401)
        dpSMSprites.Add(165, My.Resources._16501)
        dpSMSprites.Add(166, My.Resources._16601)
        dpSMSprites.Add(167, My.Resources._16701)
        dpSMSprites.Add(168, My.Resources._16801)
        dpSMSprites.Add(169, My.Resources._16901)
        dpSMSprites.Add(170, My.Resources._17001)
        dpSMSprites.Add(171, My.Resources._17101)
        dpSMSprites.Add(172, My.Resources._17201)
        dpSMSprites.Add(173, My.Resources._17301)
        dpSMSprites.Add(174, My.Resources._17401)
        dpSMSprites.Add(175, My.Resources._17501)
        dpSMSprites.Add(176, My.Resources._17601)
        dpSMSprites.Add(177, My.Resources._17701)
        dpSMSprites.Add(178, My.Resources._17801)
        dpSMSprites.Add(179, My.Resources._17901)
        dpSMSprites.Add(180, My.Resources._18001)
        dpSMSprites.Add(181, My.Resources._18101)
        dpSMSprites.Add(182, My.Resources._18201)
        dpSMSprites.Add(183, My.Resources._18301)
        dpSMSprites.Add(184, My.Resources._18401)
        dpSMSprites.Add(185, My.Resources._18501)
        dpSMSprites.Add(186, My.Resources._18601)
        dpSMSprites.Add(187, My.Resources._18701)
        dpSMSprites.Add(188, My.Resources._18801)
        dpSMSprites.Add(189, My.Resources._18901)
        dpSMSprites.Add(190, My.Resources._19001)
        dpSMSprites.Add(191, My.Resources._19101)
        dpSMSprites.Add(192, My.Resources._19201)
        dpSMSprites.Add(193, My.Resources._19301)
        dpSMSprites.Add(194, My.Resources._19401)
        dpSMSprites.Add(195, My.Resources._19501)
        dpSMSprites.Add(196, My.Resources._19601)
        dpSMSprites.Add(197, My.Resources._19701)
        dpSMSprites.Add(198, My.Resources._19801)
        dpSMSprites.Add(199, My.Resources._19901)
        dpSMSprites.Add(200, My.Resources._20001)
        dpSMSprites.Add(201, My.Resources._20101)
        dpSMSprites.Add(202, My.Resources._20201)
        dpSMSprites.Add(203, My.Resources._20301)
        dpSMSprites.Add(204, My.Resources._20401)
        dpSMSprites.Add(205, My.Resources._20501)
        dpSMSprites.Add(206, My.Resources._20601)
        dpSMSprites.Add(207, My.Resources._20701)
        dpSMSprites.Add(208, My.Resources._20801)
        dpSMSprites.Add(209, My.Resources._20901)
        dpSMSprites.Add(210, My.Resources._21001)
        dpSMSprites.Add(211, My.Resources._21101)
        dpSMSprites.Add(212, My.Resources._21201)
        dpSMSprites.Add(213, My.Resources._21301)
        dpSMSprites.Add(214, My.Resources._21401)
        dpSMSprites.Add(215, My.Resources._21501)
        dpSMSprites.Add(216, My.Resources._21601)
        dpSMSprites.Add(217, My.Resources._21701)
        dpSMSprites.Add(218, My.Resources._21801)
        dpSMSprites.Add(219, My.Resources._21901)
        dpSMSprites.Add(220, My.Resources._22001)
        dpSMSprites.Add(221, My.Resources._22101)
        dpSMSprites.Add(222, My.Resources._22201)
        dpSMSprites.Add(223, My.Resources._22301)
        dpSMSprites.Add(224, My.Resources._22401)
        dpSMSprites.Add(225, My.Resources._22501)
        dpSMSprites.Add(226, My.Resources._22601)
        dpSMSprites.Add(227, My.Resources._22701)
        dpSMSprites.Add(228, My.Resources._22801)
        dpSMSprites.Add(229, My.Resources._22901)
        dpSMSprites.Add(230, My.Resources._23001)
        dpSMSprites.Add(231, My.Resources._23101)
        dpSMSprites.Add(232, My.Resources._23201)
        dpSMSprites.Add(233, My.Resources._23301)
        dpSMSprites.Add(234, My.Resources._23401)
        dpSMSprites.Add(235, My.Resources._23501)
        dpSMSprites.Add(236, My.Resources._23601)
        dpSMSprites.Add(237, My.Resources._23701)
        dpSMSprites.Add(238, My.Resources._23801)
        dpSMSprites.Add(239, My.Resources._23901)
        dpSMSprites.Add(240, My.Resources._24001)
        dpSMSprites.Add(241, My.Resources._24101)
        dpSMSprites.Add(242, My.Resources._24201)
        dpSMSprites.Add(243, My.Resources._24301)
        dpSMSprites.Add(244, My.Resources._24401)
        dpSMSprites.Add(245, My.Resources._24501)
        dpSMSprites.Add(246, My.Resources._24601)
        dpSMSprites.Add(247, My.Resources._24701)
        dpSMSprites.Add(248, My.Resources._24801)
        dpSMSprites.Add(249, My.Resources._24901)
        dpSMSprites.Add(250, My.Resources._25001)
        dpSMSprites.Add(251, My.Resources._25101)
        dpSMSprites.Add(252, My.Resources._25201)
        dpSMSprites.Add(253, My.Resources._25301)
        dpSMSprites.Add(254, My.Resources._25401)
        dpSMSprites.Add(255, My.Resources._25501)
        dpSMSprites.Add(256, My.Resources._25601)
        dpSMSprites.Add(257, My.Resources._25701)
        dpSMSprites.Add(258, My.Resources._25801)
        dpSMSprites.Add(259, My.Resources._25901)
        dpSMSprites.Add(260, My.Resources._26001)
        dpSMSprites.Add(261, My.Resources._26101)
        dpSMSprites.Add(262, My.Resources._26201)
        dpSMSprites.Add(263, My.Resources._26301)
        dpSMSprites.Add(264, My.Resources._26401)
        dpSMSprites.Add(265, My.Resources._26501)
        dpSMSprites.Add(266, My.Resources._26601)
        dpSMSprites.Add(267, My.Resources._26701)
        dpSMSprites.Add(268, My.Resources._26801)
        dpSMSprites.Add(269, My.Resources._26901)
        dpSMSprites.Add(270, My.Resources._27001)
        dpSMSprites.Add(271, My.Resources._27101)
        dpSMSprites.Add(272, My.Resources._27201)
        dpSMSprites.Add(273, My.Resources._27301)
        dpSMSprites.Add(274, My.Resources._27401)
        dpSMSprites.Add(275, My.Resources._27501)
        dpSMSprites.Add(276, My.Resources._27601)
        dpSMSprites.Add(277, My.Resources._27701)
        dpSMSprites.Add(278, My.Resources._27801)
        dpSMSprites.Add(279, My.Resources._27901)
        dpSMSprites.Add(280, My.Resources._28001)
        dpSMSprites.Add(281, My.Resources._28101)
        dpSMSprites.Add(282, My.Resources._28201)
        dpSMSprites.Add(283, My.Resources._28301)
        dpSMSprites.Add(284, My.Resources._28401)
        dpSMSprites.Add(285, My.Resources._28501)
        dpSMSprites.Add(286, My.Resources._28601)
        dpSMSprites.Add(287, My.Resources._28701)
        dpSMSprites.Add(288, My.Resources._28801)
        dpSMSprites.Add(289, My.Resources._28901)
        dpSMSprites.Add(290, My.Resources._29001)
        dpSMSprites.Add(291, My.Resources._29101)
        dpSMSprites.Add(292, My.Resources._29201)
        dpSMSprites.Add(293, My.Resources._29301)
        dpSMSprites.Add(294, My.Resources._29401)
        dpSMSprites.Add(295, My.Resources._29501)
        dpSMSprites.Add(296, My.Resources._29601)
        dpSMSprites.Add(297, My.Resources._29701)
        dpSMSprites.Add(298, My.Resources._29801)
        dpSMSprites.Add(299, My.Resources._29901)
        dpSMSprites.Add(300, My.Resources._30001)
        dpSMSprites.Add(301, My.Resources._30101)
        dpSMSprites.Add(302, My.Resources._30201)
        dpSMSprites.Add(303, My.Resources._30301)
        dpSMSprites.Add(304, My.Resources._30401)
        dpSMSprites.Add(305, My.Resources._30501)
        dpSMSprites.Add(306, My.Resources._30601)
        dpSMSprites.Add(307, My.Resources._30701)
        dpSMSprites.Add(308, My.Resources._30801)
        dpSMSprites.Add(309, My.Resources._30901)
        dpSMSprites.Add(310, My.Resources._31001)
        dpSMSprites.Add(311, My.Resources._31101)
        dpSMSprites.Add(312, My.Resources._31201)
        dpSMSprites.Add(313, My.Resources._31301)
        dpSMSprites.Add(314, My.Resources._31401)
        dpSMSprites.Add(315, My.Resources._31501)
        dpSMSprites.Add(316, My.Resources._31601)
        dpSMSprites.Add(317, My.Resources._31701)
        dpSMSprites.Add(318, My.Resources._31801)
        dpSMSprites.Add(319, My.Resources._31901)
        dpSMSprites.Add(320, My.Resources._32001)
        dpSMSprites.Add(321, My.Resources._32101)
        dpSMSprites.Add(322, My.Resources._32201)
        dpSMSprites.Add(323, My.Resources._32301)
        dpSMSprites.Add(324, My.Resources._32401)
        dpSMSprites.Add(325, My.Resources._32501)
        dpSMSprites.Add(326, My.Resources._32601)
        dpSMSprites.Add(327, My.Resources._32701)
        dpSMSprites.Add(328, My.Resources._32801)
        dpSMSprites.Add(329, My.Resources._32901)
        dpSMSprites.Add(330, My.Resources._33001)
        dpSMSprites.Add(331, My.Resources._33101)
        dpSMSprites.Add(332, My.Resources._33201)
        dpSMSprites.Add(333, My.Resources._33301)
        dpSMSprites.Add(334, My.Resources._33401)
        dpSMSprites.Add(335, My.Resources._33501)
        dpSMSprites.Add(336, My.Resources._33601)
        dpSMSprites.Add(337, My.Resources._33701)
        dpSMSprites.Add(338, My.Resources._33801)
        dpSMSprites.Add(339, My.Resources._33901)
        dpSMSprites.Add(340, My.Resources._34001)
        dpSMSprites.Add(341, My.Resources._34101)
        dpSMSprites.Add(342, My.Resources._34201)
        dpSMSprites.Add(343, My.Resources._34301)
        dpSMSprites.Add(344, My.Resources._34401)
        dpSMSprites.Add(345, My.Resources._34501)
        dpSMSprites.Add(346, My.Resources._34601)
        dpSMSprites.Add(347, My.Resources._34701)
        dpSMSprites.Add(348, My.Resources._34801)
        dpSMSprites.Add(349, My.Resources._34901)
        dpSMSprites.Add(350, My.Resources._35001)
        dpSMSprites.Add(351, My.Resources._35101)
        dpSMSprites.Add(352, My.Resources._35201)
        dpSMSprites.Add(353, My.Resources._35301)
        dpSMSprites.Add(354, My.Resources._35401)
        dpSMSprites.Add(355, My.Resources._35501)
        dpSMSprites.Add(356, My.Resources._35601)
        dpSMSprites.Add(357, My.Resources._35701)
        dpSMSprites.Add(358, My.Resources._35801)
        dpSMSprites.Add(359, My.Resources._35901)
        dpSMSprites.Add(360, My.Resources._36001)
        dpSMSprites.Add(361, My.Resources._36101)
        dpSMSprites.Add(362, My.Resources._36201)
        dpSMSprites.Add(363, My.Resources._36301)
        dpSMSprites.Add(364, My.Resources._36401)
        dpSMSprites.Add(365, My.Resources._36501)
        dpSMSprites.Add(366, My.Resources._36601)
        dpSMSprites.Add(367, My.Resources._36701)
        dpSMSprites.Add(368, My.Resources._36801)
        dpSMSprites.Add(369, My.Resources._36901)
        dpSMSprites.Add(370, My.Resources._37001)
        dpSMSprites.Add(371, My.Resources._37101)
        dpSMSprites.Add(372, My.Resources._37201)
        dpSMSprites.Add(373, My.Resources._37301)
        dpSMSprites.Add(374, My.Resources._37401)
        dpSMSprites.Add(375, My.Resources._37501)
        dpSMSprites.Add(376, My.Resources._37601)
        dpSMSprites.Add(377, My.Resources._37701)
        dpSMSprites.Add(378, My.Resources._37801)
        dpSMSprites.Add(379, My.Resources._37901)
        dpSMSprites.Add(380, My.Resources._38001)
        dpSMSprites.Add(381, My.Resources._38101)
        dpSMSprites.Add(382, My.Resources._38201)
        dpSMSprites.Add(383, My.Resources._38301)
        dpSMSprites.Add(384, My.Resources._38401)
        dpSMSprites.Add(385, My.Resources._38501)
        dpSMSprites.Add(386, My.Resources._38601)
        dpSMSprites.Add(387, My.Resources._38701)
        dpSMSprites.Add(388, My.Resources._38801)
        dpSMSprites.Add(389, My.Resources._38901)
        dpSMSprites.Add(390, My.Resources._39001)
        dpSMSprites.Add(391, My.Resources._39101)
        dpSMSprites.Add(392, My.Resources._39201)
        dpSMSprites.Add(393, My.Resources._39301)
        dpSMSprites.Add(394, My.Resources._39401)
        dpSMSprites.Add(395, My.Resources._39501)
        dpSMSprites.Add(396, My.Resources._39601)
        dpSMSprites.Add(397, My.Resources._39701)
        dpSMSprites.Add(398, My.Resources._39801)
        dpSMSprites.Add(399, My.Resources._39901)
        dpSMSprites.Add(400, My.Resources._40001)
        dpSMSprites.Add(401, My.Resources._40101)
        dpSMSprites.Add(402, My.Resources._40201)
        dpSMSprites.Add(403, My.Resources._40301)
        dpSMSprites.Add(404, My.Resources._40401)
        dpSMSprites.Add(405, My.Resources._40501)
        dpSMSprites.Add(406, My.Resources._40601)
        dpSMSprites.Add(407, My.Resources._40701)
        dpSMSprites.Add(408, My.Resources._40801)
        dpSMSprites.Add(409, My.Resources._40901)
        dpSMSprites.Add(410, My.Resources._41001)
        dpSMSprites.Add(411, My.Resources._41101)
        dpSMSprites.Add(412, My.Resources._41201)
        dpSMSprites.Add(413, My.Resources._41301)
        dpSMSprites.Add(414, My.Resources._41401)
        dpSMSprites.Add(415, My.Resources._41501)
        dpSMSprites.Add(416, My.Resources._41601)
        dpSMSprites.Add(417, My.Resources._41701)
        dpSMSprites.Add(418, My.Resources._41801)
        dpSMSprites.Add(419, My.Resources._41901)
        dpSMSprites.Add(420, My.Resources._42001)
        dpSMSprites.Add(421, My.Resources._42101)
        dpSMSprites.Add(422, My.Resources._42201)
        dpSMSprites.Add(423, My.Resources._42301)
        dpSMSprites.Add(424, My.Resources._42401)
        dpSMSprites.Add(425, My.Resources._42501)
        dpSMSprites.Add(426, My.Resources._42601)
        dpSMSprites.Add(427, My.Resources._42701)
        dpSMSprites.Add(428, My.Resources._42801)
        dpSMSprites.Add(429, My.Resources._42901)
        dpSMSprites.Add(430, My.Resources._43001)
        dpSMSprites.Add(431, My.Resources._43101)
        dpSMSprites.Add(432, My.Resources._43201)
        dpSMSprites.Add(433, My.Resources._43301)
        dpSMSprites.Add(434, My.Resources._43401)
        dpSMSprites.Add(435, My.Resources._43501)
        dpSMSprites.Add(436, My.Resources._43601)
        dpSMSprites.Add(437, My.Resources._43701)
        dpSMSprites.Add(438, My.Resources._43801)
        dpSMSprites.Add(439, My.Resources._43901)
        dpSMSprites.Add(440, My.Resources._44001)
        dpSMSprites.Add(441, My.Resources._44101)
        dpSMSprites.Add(442, My.Resources._44201)
        dpSMSprites.Add(443, My.Resources._44301)
        dpSMSprites.Add(444, My.Resources._44401)
        dpSMSprites.Add(445, My.Resources._44501)
        dpSMSprites.Add(446, My.Resources._44601)
        dpSMSprites.Add(447, My.Resources._44701)
        dpSMSprites.Add(448, My.Resources._44801)
        dpSMSprites.Add(449, My.Resources._44901)
        dpSMSprites.Add(450, My.Resources._45001)
        dpSMSprites.Add(451, My.Resources._45101)
        dpSMSprites.Add(452, My.Resources._45201)
        dpSMSprites.Add(453, My.Resources._45301)
        dpSMSprites.Add(454, My.Resources._45401)
        dpSMSprites.Add(455, My.Resources._45501)
        dpSMSprites.Add(456, My.Resources._45601)
        dpSMSprites.Add(457, My.Resources._45701)
        dpSMSprites.Add(458, My.Resources._45801)
        dpSMSprites.Add(459, My.Resources._45901)
        dpSMSprites.Add(460, My.Resources._46001)
        dpSMSprites.Add(461, My.Resources._46101)
        dpSMSprites.Add(462, My.Resources._46201)
        dpSMSprites.Add(463, My.Resources._46301)
        dpSMSprites.Add(464, My.Resources._46401)
        dpSMSprites.Add(465, My.Resources._46501)
        dpSMSprites.Add(466, My.Resources._46601)
        dpSMSprites.Add(467, My.Resources._46701)
        dpSMSprites.Add(468, My.Resources._46801)
        dpSMSprites.Add(469, My.Resources._46901)
        dpSMSprites.Add(470, My.Resources._47001)
        dpSMSprites.Add(471, My.Resources._47101)
        dpSMSprites.Add(472, My.Resources._47201)
        dpSMSprites.Add(473, My.Resources._47301)
        dpSMSprites.Add(474, My.Resources._47401)
        dpSMSprites.Add(475, My.Resources._47501)
        dpSMSprites.Add(476, My.Resources._47601)
        dpSMSprites.Add(477, My.Resources._47701)
        dpSMSprites.Add(478, My.Resources._47801)
        dpSMSprites.Add(479, My.Resources._47901)
        dpSMSprites.Add(480, My.Resources._48001)
        dpSMSprites.Add(481, My.Resources._48101)
        dpSMSprites.Add(482, My.Resources._48201)
        dpSMSprites.Add(483, My.Resources._48301)
        dpSMSprites.Add(484, My.Resources._48401)
        dpSMSprites.Add(485, My.Resources._48501)
        dpSMSprites.Add(486, My.Resources._48601)
        dpSMSprites.Add(487, My.Resources._48701)
        dpSMSprites.Add(488, My.Resources._48801)
        dpSMSprites.Add(489, My.Resources._48901)
        dpSMSprites.Add(490, My.Resources._49001)
        dpSMSprites.Add(491, My.Resources._49101)
        dpSMSprites.Add(492, My.Resources._49201)
        dpSMSprites.Add(493, My.Resources._49301)

        dpSFSprites.Add(0, Nothing)
        dpSFSprites.Add(1, My.Resources._00111)
        dpSFSprites.Add(2, My.Resources._00211)
        dpSFSprites.Add(3, My.Resources._00311)
        dpSFSprites.Add(4, My.Resources._00411)
        dpSFSprites.Add(5, My.Resources._00511)
        dpSFSprites.Add(6, My.Resources._00611)
        dpSFSprites.Add(7, My.Resources._00711)
        dpSFSprites.Add(8, My.Resources._00811)
        dpSFSprites.Add(9, My.Resources._00911)
        dpSFSprites.Add(10, My.Resources._01011)
        dpSFSprites.Add(11, My.Resources._01111)
        dpSFSprites.Add(12, My.Resources._01211)
        dpSFSprites.Add(13, My.Resources._01311)
        dpSFSprites.Add(14, My.Resources._01411)
        dpSFSprites.Add(15, My.Resources._01511)
        dpSFSprites.Add(16, My.Resources._01611)
        dpSFSprites.Add(17, My.Resources._01711)
        dpSFSprites.Add(18, My.Resources._01811)
        dpSFSprites.Add(19, My.Resources._01911)
        dpSFSprites.Add(20, My.Resources._02011)
        dpSFSprites.Add(21, My.Resources._02111)
        dpSFSprites.Add(22, My.Resources._02211)
        dpSFSprites.Add(23, My.Resources._02311)
        dpSFSprites.Add(24, My.Resources._02411)
        dpSFSprites.Add(25, My.Resources._02511)
        dpSFSprites.Add(26, My.Resources._02611)
        dpSFSprites.Add(27, My.Resources._02711)
        dpSFSprites.Add(28, My.Resources._02811)
        dpSFSprites.Add(29, My.Resources._02911)
        dpSFSprites.Add(30, My.Resources._03011)
        dpSFSprites.Add(31, My.Resources._03111)
        dpSFSprites.Add(32, My.Resources._03211)
        dpSFSprites.Add(33, My.Resources._03311)
        dpSFSprites.Add(34, My.Resources._03411)
        dpSFSprites.Add(35, My.Resources._03511)
        dpSFSprites.Add(36, My.Resources._03611)
        dpSFSprites.Add(37, My.Resources._03711)
        dpSFSprites.Add(38, My.Resources._03811)
        dpSFSprites.Add(39, My.Resources._03911)
        dpSFSprites.Add(40, My.Resources._04011)
        dpSFSprites.Add(41, My.Resources._04111)
        dpSFSprites.Add(42, My.Resources._04211)
        dpSFSprites.Add(43, My.Resources._04311)
        dpSFSprites.Add(44, My.Resources._04411)
        dpSFSprites.Add(45, My.Resources._04511)
        dpSFSprites.Add(46, My.Resources._04611)
        dpSFSprites.Add(47, My.Resources._04711)
        dpSFSprites.Add(48, My.Resources._04811)
        dpSFSprites.Add(49, My.Resources._04911)
        dpSFSprites.Add(50, My.Resources._05011)
        dpSFSprites.Add(51, My.Resources._05111)
        dpSFSprites.Add(52, My.Resources._05211)
        dpSFSprites.Add(53, My.Resources._05311)
        dpSFSprites.Add(54, My.Resources._05411)
        dpSFSprites.Add(55, My.Resources._05511)
        dpSFSprites.Add(56, My.Resources._05611)
        dpSFSprites.Add(57, My.Resources._05711)
        dpSFSprites.Add(58, My.Resources._05811)
        dpSFSprites.Add(59, My.Resources._05911)
        dpSFSprites.Add(60, My.Resources._06011)
        dpSFSprites.Add(61, My.Resources._06111)
        dpSFSprites.Add(62, My.Resources._06211)
        dpSFSprites.Add(63, My.Resources._06311)
        dpSFSprites.Add(64, My.Resources._06411)
        dpSFSprites.Add(65, My.Resources._06511)
        dpSFSprites.Add(66, My.Resources._06611)
        dpSFSprites.Add(67, My.Resources._06711)
        dpSFSprites.Add(68, My.Resources._06811)
        dpSFSprites.Add(69, My.Resources._06911)
        dpSFSprites.Add(70, My.Resources._07011)
        dpSFSprites.Add(71, My.Resources._07111)
        dpSFSprites.Add(72, My.Resources._07211)
        dpSFSprites.Add(73, My.Resources._07311)
        dpSFSprites.Add(74, My.Resources._07411)
        dpSFSprites.Add(75, My.Resources._07511)
        dpSFSprites.Add(76, My.Resources._07611)
        dpSFSprites.Add(77, My.Resources._07711)
        dpSFSprites.Add(78, My.Resources._07811)
        dpSFSprites.Add(79, My.Resources._07911)
        dpSFSprites.Add(80, My.Resources._08011)
        dpSFSprites.Add(81, My.Resources._08111)
        dpSFSprites.Add(82, My.Resources._08211)
        dpSFSprites.Add(83, My.Resources._08311)
        dpSFSprites.Add(84, My.Resources._08411)
        dpSFSprites.Add(85, My.Resources._08511)
        dpSFSprites.Add(86, My.Resources._08611)
        dpSFSprites.Add(87, My.Resources._08711)
        dpSFSprites.Add(88, My.Resources._08811)
        dpSFSprites.Add(89, My.Resources._08911)
        dpSFSprites.Add(90, My.Resources._09011)
        dpSFSprites.Add(91, My.Resources._09111)
        dpSFSprites.Add(92, My.Resources._09211)
        dpSFSprites.Add(93, My.Resources._09311)
        dpSFSprites.Add(94, My.Resources._09411)
        dpSFSprites.Add(95, My.Resources._09511)
        dpSFSprites.Add(96, My.Resources._09611)
        dpSFSprites.Add(97, My.Resources._09711)
        dpSFSprites.Add(98, My.Resources._09811)
        dpSFSprites.Add(99, My.Resources._09911)
        dpSFSprites.Add(100, My.Resources._10011)
        dpSFSprites.Add(101, My.Resources._10111)
        dpSFSprites.Add(102, My.Resources._10211)
        dpSFSprites.Add(103, My.Resources._10311)
        dpSFSprites.Add(104, My.Resources._10411)
        dpSFSprites.Add(105, My.Resources._10511)
        dpSFSprites.Add(106, My.Resources._10611)
        dpSFSprites.Add(107, My.Resources._10711)
        dpSFSprites.Add(108, My.Resources._10811)
        dpSFSprites.Add(109, My.Resources._10911)
        dpSFSprites.Add(110, My.Resources._11011)
        dpSFSprites.Add(111, My.Resources._11111)
        dpSFSprites.Add(112, My.Resources._11211)
        dpSFSprites.Add(113, My.Resources._11311)
        dpSFSprites.Add(114, My.Resources._11411)
        dpSFSprites.Add(115, My.Resources._11511)
        dpSFSprites.Add(116, My.Resources._11611)
        dpSFSprites.Add(117, My.Resources._11711)
        dpSFSprites.Add(118, My.Resources._11811)
        dpSFSprites.Add(119, My.Resources._11911)
        dpSFSprites.Add(120, My.Resources._12011)
        dpSFSprites.Add(121, My.Resources._12111)
        dpSFSprites.Add(122, My.Resources._12211)
        dpSFSprites.Add(123, My.Resources._12311)
        dpSFSprites.Add(124, My.Resources._12411)
        dpSFSprites.Add(125, My.Resources._12511)
        dpSFSprites.Add(126, My.Resources._12611)
        dpSFSprites.Add(127, My.Resources._12711)
        dpSFSprites.Add(128, My.Resources._12811)
        dpSFSprites.Add(129, My.Resources._12911)
        dpSFSprites.Add(130, My.Resources._13011)
        dpSFSprites.Add(131, My.Resources._13111)
        dpSFSprites.Add(132, My.Resources._13211)
        dpSFSprites.Add(133, My.Resources._13311)
        dpSFSprites.Add(134, My.Resources._13411)
        dpSFSprites.Add(135, My.Resources._13511)
        dpSFSprites.Add(136, My.Resources._13611)
        dpSFSprites.Add(137, My.Resources._13711)
        dpSFSprites.Add(138, My.Resources._13811)
        dpSFSprites.Add(139, My.Resources._13911)
        dpSFSprites.Add(140, My.Resources._14011)
        dpSFSprites.Add(141, My.Resources._14111)
        dpSFSprites.Add(142, My.Resources._14211)
        dpSFSprites.Add(143, My.Resources._14311)
        dpSFSprites.Add(144, My.Resources._14411)
        dpSFSprites.Add(145, My.Resources._14511)
        dpSFSprites.Add(146, My.Resources._14611)
        dpSFSprites.Add(147, My.Resources._14711)
        dpSFSprites.Add(148, My.Resources._14811)
        dpSFSprites.Add(149, My.Resources._14911)
        dpSFSprites.Add(150, My.Resources._15011)
        dpSFSprites.Add(151, My.Resources._15111)
        dpSFSprites.Add(152, My.Resources._15211)
        dpSFSprites.Add(153, My.Resources._15311)
        dpSFSprites.Add(154, My.Resources._15411)
        dpSFSprites.Add(155, My.Resources._15511)
        dpSFSprites.Add(156, My.Resources._15611)
        dpSFSprites.Add(157, My.Resources._15711)
        dpSFSprites.Add(158, My.Resources._15811)
        dpSFSprites.Add(159, My.Resources._15911)
        dpSFSprites.Add(160, My.Resources._16011)
        dpSFSprites.Add(161, My.Resources._16111)
        dpSFSprites.Add(162, My.Resources._16211)
        dpSFSprites.Add(163, My.Resources._16311)
        dpSFSprites.Add(164, My.Resources._16411)
        dpSFSprites.Add(165, My.Resources._16511)
        dpSFSprites.Add(166, My.Resources._16611)
        dpSFSprites.Add(167, My.Resources._16711)
        dpSFSprites.Add(168, My.Resources._16811)
        dpSFSprites.Add(169, My.Resources._16911)
        dpSFSprites.Add(170, My.Resources._17011)
        dpSFSprites.Add(171, My.Resources._17111)
        dpSFSprites.Add(172, My.Resources._17211)
        dpSFSprites.Add(173, My.Resources._17311)
        dpSFSprites.Add(174, My.Resources._17411)
        dpSFSprites.Add(175, My.Resources._17511)
        dpSFSprites.Add(176, My.Resources._17611)
        dpSFSprites.Add(177, My.Resources._17711)
        dpSFSprites.Add(178, My.Resources._17811)
        dpSFSprites.Add(179, My.Resources._17911)
        dpSFSprites.Add(180, My.Resources._18011)
        dpSFSprites.Add(181, My.Resources._18111)
        dpSFSprites.Add(182, My.Resources._18211)
        dpSFSprites.Add(183, My.Resources._18311)
        dpSFSprites.Add(184, My.Resources._18411)
        dpSFSprites.Add(185, My.Resources._18511)
        dpSFSprites.Add(186, My.Resources._18611)
        dpSFSprites.Add(187, My.Resources._18711)
        dpSFSprites.Add(188, My.Resources._18811)
        dpSFSprites.Add(189, My.Resources._18911)
        dpSFSprites.Add(190, My.Resources._19011)
        dpSFSprites.Add(191, My.Resources._19111)
        dpSFSprites.Add(192, My.Resources._19211)
        dpSFSprites.Add(193, My.Resources._19311)
        dpSFSprites.Add(194, My.Resources._19411)
        dpSFSprites.Add(195, My.Resources._19511)
        dpSFSprites.Add(196, My.Resources._19611)
        dpSFSprites.Add(197, My.Resources._19711)
        dpSFSprites.Add(198, My.Resources._19811)
        dpSFSprites.Add(199, My.Resources._19911)
        dpSFSprites.Add(200, My.Resources._20011)
        dpSFSprites.Add(201, My.Resources._20111)
        dpSFSprites.Add(202, My.Resources._20211)
        dpSFSprites.Add(203, My.Resources._20311)
        dpSFSprites.Add(204, My.Resources._20411)
        dpSFSprites.Add(205, My.Resources._20511)
        dpSFSprites.Add(206, My.Resources._20611)
        dpSFSprites.Add(207, My.Resources._20711)
        dpSFSprites.Add(208, My.Resources._20811)
        dpSFSprites.Add(209, My.Resources._20911)
        dpSFSprites.Add(210, My.Resources._21011)
        dpSFSprites.Add(211, My.Resources._21111)
        dpSFSprites.Add(212, My.Resources._21211)
        dpSFSprites.Add(213, My.Resources._21311)
        dpSFSprites.Add(214, My.Resources._21411)
        dpSFSprites.Add(215, My.Resources._21511)
        dpSFSprites.Add(216, My.Resources._21611)
        dpSFSprites.Add(217, My.Resources._21711)
        dpSFSprites.Add(218, My.Resources._21811)
        dpSFSprites.Add(219, My.Resources._21911)
        dpSFSprites.Add(220, My.Resources._22011)
        dpSFSprites.Add(221, My.Resources._22111)
        dpSFSprites.Add(222, My.Resources._22211)
        dpSFSprites.Add(223, My.Resources._22311)
        dpSFSprites.Add(224, My.Resources._22411)
        dpSFSprites.Add(225, My.Resources._22511)
        dpSFSprites.Add(226, My.Resources._22611)
        dpSFSprites.Add(227, My.Resources._22711)
        dpSFSprites.Add(228, My.Resources._22811)
        dpSFSprites.Add(229, My.Resources._22911)
        dpSFSprites.Add(230, My.Resources._23011)
        dpSFSprites.Add(231, My.Resources._23111)
        dpSFSprites.Add(232, My.Resources._23211)
        dpSFSprites.Add(233, My.Resources._23311)
        dpSFSprites.Add(234, My.Resources._23411)
        dpSFSprites.Add(235, My.Resources._23511)
        dpSFSprites.Add(236, My.Resources._23611)
        dpSFSprites.Add(237, My.Resources._23711)
        dpSFSprites.Add(238, My.Resources._23811)
        dpSFSprites.Add(239, My.Resources._23911)
        dpSFSprites.Add(240, My.Resources._24011)
        dpSFSprites.Add(241, My.Resources._24111)
        dpSFSprites.Add(242, My.Resources._24211)
        dpSFSprites.Add(243, My.Resources._24311)
        dpSFSprites.Add(244, My.Resources._24411)
        dpSFSprites.Add(245, My.Resources._24511)
        dpSFSprites.Add(246, My.Resources._24611)
        dpSFSprites.Add(247, My.Resources._24711)
        dpSFSprites.Add(248, My.Resources._24811)
        dpSFSprites.Add(249, My.Resources._24911)
        dpSFSprites.Add(250, My.Resources._25011)
        dpSFSprites.Add(251, My.Resources._25111)
        dpSFSprites.Add(252, My.Resources._25211)
        dpSFSprites.Add(253, My.Resources._25311)
        dpSFSprites.Add(254, My.Resources._25411)
        dpSFSprites.Add(255, My.Resources._25511)
        dpSFSprites.Add(256, My.Resources._25611)
        dpSFSprites.Add(257, My.Resources._25711)
        dpSFSprites.Add(258, My.Resources._25811)
        dpSFSprites.Add(259, My.Resources._25911)
        dpSFSprites.Add(260, My.Resources._26011)
        dpSFSprites.Add(261, My.Resources._26111)
        dpSFSprites.Add(262, My.Resources._26211)
        dpSFSprites.Add(263, My.Resources._26311)
        dpSFSprites.Add(264, My.Resources._26411)
        dpSFSprites.Add(265, My.Resources._26511)
        dpSFSprites.Add(266, My.Resources._26611)
        dpSFSprites.Add(267, My.Resources._26711)
        dpSFSprites.Add(268, My.Resources._26811)
        dpSFSprites.Add(269, My.Resources._26911)
        dpSFSprites.Add(270, My.Resources._27011)
        dpSFSprites.Add(271, My.Resources._27111)
        dpSFSprites.Add(272, My.Resources._27211)
        dpSFSprites.Add(273, My.Resources._27311)
        dpSFSprites.Add(274, My.Resources._27411)
        dpSFSprites.Add(275, My.Resources._27511)
        dpSFSprites.Add(276, My.Resources._27611)
        dpSFSprites.Add(277, My.Resources._27711)
        dpSFSprites.Add(278, My.Resources._27811)
        dpSFSprites.Add(279, My.Resources._27911)
        dpSFSprites.Add(280, My.Resources._28011)
        dpSFSprites.Add(281, My.Resources._28111)
        dpSFSprites.Add(282, My.Resources._28211)
        dpSFSprites.Add(283, My.Resources._28311)
        dpSFSprites.Add(284, My.Resources._28411)
        dpSFSprites.Add(285, My.Resources._28511)
        dpSFSprites.Add(286, My.Resources._28611)
        dpSFSprites.Add(287, My.Resources._28711)
        dpSFSprites.Add(288, My.Resources._28811)
        dpSFSprites.Add(289, My.Resources._28911)
        dpSFSprites.Add(290, My.Resources._29011)
        dpSFSprites.Add(291, My.Resources._29111)
        dpSFSprites.Add(292, My.Resources._29211)
        dpSFSprites.Add(293, My.Resources._29311)
        dpSFSprites.Add(294, My.Resources._29411)
        dpSFSprites.Add(295, My.Resources._29511)
        dpSFSprites.Add(296, My.Resources._29611)
        dpSFSprites.Add(297, My.Resources._29711)
        dpSFSprites.Add(298, My.Resources._29811)
        dpSFSprites.Add(299, My.Resources._29911)
        dpSFSprites.Add(300, My.Resources._30011)
        dpSFSprites.Add(301, My.Resources._30111)
        dpSFSprites.Add(302, My.Resources._30211)
        dpSFSprites.Add(303, My.Resources._30311)
        dpSFSprites.Add(304, My.Resources._30411)
        dpSFSprites.Add(305, My.Resources._30511)
        dpSFSprites.Add(306, My.Resources._30611)
        dpSFSprites.Add(307, My.Resources._30711)
        dpSFSprites.Add(308, My.Resources._30811)
        dpSFSprites.Add(309, My.Resources._30911)
        dpSFSprites.Add(310, My.Resources._31011)
        dpSFSprites.Add(311, My.Resources._31111)
        dpSFSprites.Add(312, My.Resources._31211)
        dpSFSprites.Add(313, My.Resources._31311)
        dpSFSprites.Add(314, My.Resources._31411)
        dpSFSprites.Add(315, My.Resources._31511)
        dpSFSprites.Add(316, My.Resources._31611)
        dpSFSprites.Add(317, My.Resources._31711)
        dpSFSprites.Add(318, My.Resources._31811)
        dpSFSprites.Add(319, My.Resources._31911)
        dpSFSprites.Add(320, My.Resources._32011)
        dpSFSprites.Add(321, My.Resources._32111)
        dpSFSprites.Add(322, My.Resources._32211)
        dpSFSprites.Add(323, My.Resources._32311)
        dpSFSprites.Add(324, My.Resources._32411)
        dpSFSprites.Add(325, My.Resources._32511)
        dpSFSprites.Add(326, My.Resources._32611)
        dpSFSprites.Add(327, My.Resources._32711)
        dpSFSprites.Add(328, My.Resources._32811)
        dpSFSprites.Add(329, My.Resources._32911)
        dpSFSprites.Add(330, My.Resources._33011)
        dpSFSprites.Add(331, My.Resources._33111)
        dpSFSprites.Add(332, My.Resources._33211)
        dpSFSprites.Add(333, My.Resources._33311)
        dpSFSprites.Add(334, My.Resources._33411)
        dpSFSprites.Add(335, My.Resources._33511)
        dpSFSprites.Add(336, My.Resources._33611)
        dpSFSprites.Add(337, My.Resources._33711)
        dpSFSprites.Add(338, My.Resources._33811)
        dpSFSprites.Add(339, My.Resources._33911)
        dpSFSprites.Add(340, My.Resources._34011)
        dpSFSprites.Add(341, My.Resources._34111)
        dpSFSprites.Add(342, My.Resources._34211)
        dpSFSprites.Add(343, My.Resources._34311)
        dpSFSprites.Add(344, My.Resources._34411)
        dpSFSprites.Add(345, My.Resources._34511)
        dpSFSprites.Add(346, My.Resources._34611)
        dpSFSprites.Add(347, My.Resources._34711)
        dpSFSprites.Add(348, My.Resources._34811)
        dpSFSprites.Add(349, My.Resources._34911)
        dpSFSprites.Add(350, My.Resources._35011)
        dpSFSprites.Add(351, My.Resources._35111)
        dpSFSprites.Add(352, My.Resources._35211)
        dpSFSprites.Add(353, My.Resources._35311)
        dpSFSprites.Add(354, My.Resources._35411)
        dpSFSprites.Add(355, My.Resources._35511)
        dpSFSprites.Add(356, My.Resources._35611)
        dpSFSprites.Add(357, My.Resources._35711)
        dpSFSprites.Add(358, My.Resources._35811)
        dpSFSprites.Add(359, My.Resources._35911)
        dpSFSprites.Add(360, My.Resources._36011)
        dpSFSprites.Add(361, My.Resources._36111)
        dpSFSprites.Add(362, My.Resources._36211)
        dpSFSprites.Add(363, My.Resources._36311)
        dpSFSprites.Add(364, My.Resources._36411)
        dpSFSprites.Add(365, My.Resources._36511)
        dpSFSprites.Add(366, My.Resources._36611)
        dpSFSprites.Add(367, My.Resources._36711)
        dpSFSprites.Add(368, My.Resources._36811)
        dpSFSprites.Add(369, My.Resources._36911)
        dpSFSprites.Add(370, My.Resources._37011)
        dpSFSprites.Add(371, My.Resources._37111)
        dpSFSprites.Add(372, My.Resources._37211)
        dpSFSprites.Add(373, My.Resources._37311)
        dpSFSprites.Add(374, My.Resources._37411)
        dpSFSprites.Add(375, My.Resources._37511)
        dpSFSprites.Add(376, My.Resources._37611)
        dpSFSprites.Add(377, My.Resources._37711)
        dpSFSprites.Add(378, My.Resources._37811)
        dpSFSprites.Add(379, My.Resources._37911)
        dpSFSprites.Add(380, My.Resources._38011)
        dpSFSprites.Add(381, My.Resources._38111)
        dpSFSprites.Add(382, My.Resources._38211)
        dpSFSprites.Add(383, My.Resources._38311)
        dpSFSprites.Add(384, My.Resources._38411)
        dpSFSprites.Add(385, My.Resources._38511)
        dpSFSprites.Add(386, My.Resources._38611)
        dpSFSprites.Add(387, My.Resources._38711)
        dpSFSprites.Add(388, My.Resources._38811)
        dpSFSprites.Add(389, My.Resources._38911)
        dpSFSprites.Add(390, My.Resources._39011)
        dpSFSprites.Add(391, My.Resources._39111)
        dpSFSprites.Add(392, My.Resources._39211)
        dpSFSprites.Add(393, My.Resources._39311)
        dpSFSprites.Add(394, My.Resources._39411)
        dpSFSprites.Add(395, My.Resources._39511)
        dpSFSprites.Add(396, My.Resources._39611)
        dpSFSprites.Add(397, My.Resources._39711)
        dpSFSprites.Add(398, My.Resources._39811)
        dpSFSprites.Add(399, My.Resources._39911)
        dpSFSprites.Add(400, My.Resources._40011)
        dpSFSprites.Add(401, My.Resources._40111)
        dpSFSprites.Add(402, My.Resources._40211)
        dpSFSprites.Add(403, My.Resources._40311)
        dpSFSprites.Add(404, My.Resources._40411)
        dpSFSprites.Add(405, My.Resources._40511)
        dpSFSprites.Add(406, My.Resources._40611)
        dpSFSprites.Add(407, My.Resources._40711)
        dpSFSprites.Add(408, My.Resources._40811)
        dpSFSprites.Add(409, My.Resources._40911)
        dpSFSprites.Add(410, My.Resources._41011)
        dpSFSprites.Add(411, My.Resources._41111)
        dpSFSprites.Add(412, My.Resources._41211)
        dpSFSprites.Add(413, My.Resources._41311)
        dpSFSprites.Add(414, My.Resources._41411)
        dpSFSprites.Add(415, My.Resources._41511)
        dpSFSprites.Add(416, My.Resources._41611)
        dpSFSprites.Add(417, My.Resources._41711)
        dpSFSprites.Add(418, My.Resources._41811)
        dpSFSprites.Add(419, My.Resources._41911)
        dpSFSprites.Add(420, My.Resources._42011)
        dpSFSprites.Add(421, My.Resources._42111)
        dpSFSprites.Add(422, My.Resources._42211)
        dpSFSprites.Add(423, My.Resources._42311)
        dpSFSprites.Add(424, My.Resources._42411)
        dpSFSprites.Add(425, My.Resources._42511)
        dpSFSprites.Add(426, My.Resources._42611)
        dpSFSprites.Add(427, My.Resources._42711)
        dpSFSprites.Add(428, My.Resources._42811)
        dpSFSprites.Add(429, My.Resources._42911)
        dpSFSprites.Add(430, My.Resources._43011)
        dpSFSprites.Add(431, My.Resources._43111)
        dpSFSprites.Add(432, My.Resources._43211)
        dpSFSprites.Add(433, My.Resources._43311)
        dpSFSprites.Add(434, My.Resources._43411)
        dpSFSprites.Add(435, My.Resources._43511)
        dpSFSprites.Add(436, My.Resources._43611)
        dpSFSprites.Add(437, My.Resources._43711)
        dpSFSprites.Add(438, My.Resources._43811)
        dpSFSprites.Add(439, My.Resources._43911)
        dpSFSprites.Add(440, My.Resources._44011)
        dpSFSprites.Add(441, My.Resources._44111)
        dpSFSprites.Add(442, My.Resources._44211)
        dpSFSprites.Add(443, My.Resources._44311)
        dpSFSprites.Add(444, My.Resources._44411)
        dpSFSprites.Add(445, My.Resources._44511)
        dpSFSprites.Add(446, My.Resources._44611)
        dpSFSprites.Add(447, My.Resources._44711)
        dpSFSprites.Add(448, My.Resources._44811)
        dpSFSprites.Add(449, My.Resources._44911)
        dpSFSprites.Add(450, My.Resources._45011)
        dpSFSprites.Add(451, My.Resources._45111)
        dpSFSprites.Add(452, My.Resources._45211)
        dpSFSprites.Add(453, My.Resources._45311)
        dpSFSprites.Add(454, My.Resources._45411)
        dpSFSprites.Add(455, My.Resources._45511)
        dpSFSprites.Add(456, My.Resources._45611)
        dpSFSprites.Add(457, My.Resources._45711)
        dpSFSprites.Add(458, My.Resources._45811)
        dpSFSprites.Add(459, My.Resources._45911)
        dpSFSprites.Add(460, My.Resources._46011)
        dpSFSprites.Add(461, My.Resources._46111)
        dpSFSprites.Add(462, My.Resources._46211)
        dpSFSprites.Add(463, My.Resources._46311)
        dpSFSprites.Add(464, My.Resources._46411)
        dpSFSprites.Add(465, My.Resources._46511)
        dpSFSprites.Add(466, My.Resources._46611)
        dpSFSprites.Add(467, My.Resources._46711)
        dpSFSprites.Add(468, My.Resources._46811)
        dpSFSprites.Add(469, My.Resources._46911)
        dpSFSprites.Add(470, My.Resources._47011)
        dpSFSprites.Add(471, My.Resources._47111)
        dpSFSprites.Add(472, My.Resources._47211)
        dpSFSprites.Add(473, My.Resources._47311)
        dpSFSprites.Add(474, My.Resources._47411)
        dpSFSprites.Add(475, My.Resources._47511)
        dpSFSprites.Add(476, My.Resources._47611)
        dpSFSprites.Add(477, My.Resources._47711)
        dpSFSprites.Add(478, My.Resources._47811)
        dpSFSprites.Add(479, My.Resources._47911)
        dpSFSprites.Add(480, My.Resources._48011)
        dpSFSprites.Add(481, My.Resources._48111)
        dpSFSprites.Add(482, My.Resources._48211)
        dpSFSprites.Add(483, My.Resources._48311)
        dpSFSprites.Add(484, My.Resources._48411)
        dpSFSprites.Add(485, My.Resources._48511)
        dpSFSprites.Add(486, My.Resources._48611)
        dpSFSprites.Add(487, My.Resources._48711)
        dpSFSprites.Add(488, My.Resources._48811)
        dpSFSprites.Add(489, My.Resources._48911)
        dpSFSprites.Add(490, My.Resources._49011)
        dpSFSprites.Add(491, My.Resources._49111)
        dpSFSprites.Add(492, My.Resources._49211)
        dpSFSprites.Add(493, My.Resources._49311)

        dpArceusSprites.Add(0, My.Resources._493000)
        dpArceusSprites.Add(1, My.Resources._493001)
        dpArceusSprites.Add(2, My.Resources._493002)
        dpArceusSprites.Add(3, My.Resources._493003)
        dpArceusSprites.Add(4, My.Resources._493004)
        dpArceusSprites.Add(5, My.Resources._493005)
        dpArceusSprites.Add(6, My.Resources._493006)
        dpArceusSprites.Add(7, My.Resources._493007)
        dpArceusSprites.Add(8, My.Resources._493008)
        dpArceusSprites.Add(9, My.Resources._493009)
        dpArceusSprites.Add(10, My.Resources._4930010)
        dpArceusSprites.Add(11, My.Resources._4930011)
        dpArceusSprites.Add(12, My.Resources._4930012)
        dpArceusSprites.Add(13, My.Resources._4930013)
        dpArceusSprites.Add(14, My.Resources._4930014)
        dpArceusSprites.Add(15, My.Resources._4930015)
        dpArceusSprites.Add(16, My.Resources._4930016)
        dpArceusSprites.Add(17, My.Resources._4930017)
        dpSArceusSprites.Add(0, My.Resources._493010)
        dpSArceusSprites.Add(1, My.Resources._493011)
        dpSArceusSprites.Add(2, My.Resources._493012)
        dpSArceusSprites.Add(3, My.Resources._493013)
        dpSArceusSprites.Add(4, My.Resources._493014)
        dpSArceusSprites.Add(5, My.Resources._493015)
        dpSArceusSprites.Add(6, My.Resources._493016)
        dpSArceusSprites.Add(7, My.Resources._493017)
        dpSArceusSprites.Add(8, My.Resources._493018)
        dpSArceusSprites.Add(9, My.Resources._493019)
        dpSArceusSprites.Add(10, My.Resources._4930110)
        dpSArceusSprites.Add(11, My.Resources._4930111)
        dpSArceusSprites.Add(12, My.Resources._4930112)
        dpSArceusSprites.Add(13, My.Resources._4930113)
        dpSArceusSprites.Add(14, My.Resources._4930114)
        dpSArceusSprites.Add(15, My.Resources._4930115)
        dpSArceusSprites.Add(16, My.Resources._4930116)
        dpSArceusSprites.Add(17, My.Resources._4930117)

        dpDeoxysSprites.Add(0, My.Resources._386000)
        dpDeoxysSprites.Add(1, My.Resources._386001)
        dpDeoxysSprites.Add(2, My.Resources._386002)
        dpDeoxysSprites.Add(3, My.Resources._386003)
        dpSDeoxysSprites.Add(0, My.Resources._386010)
        dpSDeoxysSprites.Add(1, My.Resources._386011)
        dpSDeoxysSprites.Add(2, My.Resources._386012)
        dpSDeoxysSprites.Add(3, My.Resources._386013)

        dpBurmySprites.Add(0, My.Resources._412000)
        dpBurmySprites.Add(1, My.Resources._412001)
        dpBurmySprites.Add(2, My.Resources._412002)
        dpSBurmySprites.Add(0, My.Resources._412010)
        dpSBurmySprites.Add(1, My.Resources._412011)
        dpSBurmySprites.Add(2, My.Resources._412012)

        dpWormadamSprites.Add(0, My.Resources._413000)
        dpWormadamSprites.Add(1, My.Resources._413001)
        dpWormadamSprites.Add(2, My.Resources._413002)
        dpSWormadamSprites.Add(0, My.Resources._413010)
        dpSWormadamSprites.Add(1, My.Resources._413011)
        dpSWormadamSprites.Add(2, My.Resources._413012)

        dpUnownSprites.Add(0, My.Resources._201000)
        dpUnownSprites.Add(1, My.Resources._201001)
        dpUnownSprites.Add(2, My.Resources._201002)
        dpUnownSprites.Add(3, My.Resources._201003)
        dpUnownSprites.Add(4, My.Resources._201004)
        dpUnownSprites.Add(5, My.Resources._201005)
        dpUnownSprites.Add(6, My.Resources._201006)
        dpUnownSprites.Add(7, My.Resources._201007)
        dpUnownSprites.Add(8, My.Resources._201008)
        dpUnownSprites.Add(9, My.Resources._201009)
        dpUnownSprites.Add(10, My.Resources._2010010)
        dpUnownSprites.Add(11, My.Resources._2010011)
        dpUnownSprites.Add(12, My.Resources._2010012)
        dpUnownSprites.Add(13, My.Resources._2010013)
        dpUnownSprites.Add(14, My.Resources._2010014)
        dpUnownSprites.Add(15, My.Resources._2010015)
        dpUnownSprites.Add(16, My.Resources._2010016)
        dpUnownSprites.Add(17, My.Resources._2010017)
        dpUnownSprites.Add(18, My.Resources._2010018)
        dpUnownSprites.Add(19, My.Resources._2010019)
        dpUnownSprites.Add(20, My.Resources._2010020)
        dpUnownSprites.Add(21, My.Resources._2010021)
        dpUnownSprites.Add(22, My.Resources._2010022)
        dpUnownSprites.Add(23, My.Resources._2010023)
        dpUnownSprites.Add(24, My.Resources._2010024)
        dpUnownSprites.Add(25, My.Resources._2010025)
        dpUnownSprites.Add(26, My.Resources._2010026)
        dpUnownSprites.Add(27, My.Resources._2010027)
        dpSUnownSprites.Add(0, My.Resources._201010)
        dpSUnownSprites.Add(1, My.Resources._201011)
        dpSUnownSprites.Add(2, My.Resources._201012)
        dpSUnownSprites.Add(3, My.Resources._201013)
        dpSUnownSprites.Add(4, My.Resources._201014)
        dpSUnownSprites.Add(5, My.Resources._201015)
        dpSUnownSprites.Add(6, My.Resources._201016)
        dpSUnownSprites.Add(7, My.Resources._201017)
        dpSUnownSprites.Add(8, My.Resources._201018)
        dpSUnownSprites.Add(9, My.Resources._201019)
        dpSUnownSprites.Add(10, My.Resources._2010110)
        dpSUnownSprites.Add(11, My.Resources._2010111)
        dpSUnownSprites.Add(12, My.Resources._2010112)
        dpSUnownSprites.Add(13, My.Resources._2010113)
        dpSUnownSprites.Add(14, My.Resources._2010114)
        dpSUnownSprites.Add(15, My.Resources._2010115)
        dpSUnownSprites.Add(16, My.Resources._2010116)
        dpSUnownSprites.Add(17, My.Resources._2010117)
        dpSUnownSprites.Add(18, My.Resources._2010118)
        dpSUnownSprites.Add(19, My.Resources._2010119)
        dpSUnownSprites.Add(20, My.Resources._2010120)
        dpSUnownSprites.Add(21, My.Resources._2010121)
        dpSUnownSprites.Add(22, My.Resources._2010122)
        dpSUnownSprites.Add(23, My.Resources._2010123)
        dpSUnownSprites.Add(24, My.Resources._2010124)
        dpSUnownSprites.Add(25, My.Resources._2010125)
        dpSUnownSprites.Add(26, My.Resources._2010126)
        dpSUnownSprites.Add(27, My.Resources._2010127)

        dpRotomSprites.Add(0, My.Resources._479000)
        dpRotomSprites.Add(1, My.Resources._479001)
        dpRotomSprites.Add(2, My.Resources._479002)
        dpRotomSprites.Add(3, My.Resources._479003)
        dpRotomSprites.Add(4, My.Resources._479004)
        dpRotomSprites.Add(5, My.Resources._479005)
        dpSRotomSprites.Add(0, My.Resources._479010)
        dpSRotomSprites.Add(1, My.Resources._479011)
        dpSRotomSprites.Add(2, My.Resources._479012)
        dpSRotomSprites.Add(3, My.Resources._479013)
        dpSRotomSprites.Add(4, My.Resources._479014)
        dpSRotomSprites.Add(5, My.Resources._479015)

        dpShellosSprites.Add(0, My.Resources._422000)
        dpShellosSprites.Add(1, My.Resources._422001)
        dpSShellosSprites.Add(0, My.Resources._422010)
        dpSShellosSprites.Add(1, My.Resources._422011)

        dpGastrodonSprites.Add(0, My.Resources._423000)
        dpGastrodonSprites.Add(1, My.Resources._423001)
        dpSGastrodonSprites.Add(0, My.Resources._423010)
        dpSGastrodonSprites.Add(1, My.Resources._423011)

        dpGiratinaSprites.Add(0, My.Resources._487000)
        dpGiratinaSprites.Add(1, My.Resources._487001)
        dpSGiratinaSprites.Add(0, My.Resources._487010)
        dpSGiratinaSprites.Add(1, My.Resources._487011)

        dpShayminSprites.Add(0, My.Resources._492000)
        dpShayminSprites.Add(1, My.Resources._492001)
        dpSShayminSprites.Add(0, My.Resources._492010)
        dpSShayminSprites.Add(1, My.Resources._492011)

        dpDeoxysStats.Add(0, New Byte() {50, 150, 50, 150, 150, 50})
        dpDeoxysStats.Add(1, New Byte() {50, 180, 20, 150, 180, 20})
        dpDeoxysStats.Add(2, New Byte() {50, 70, 160, 90, 70, 160})
        dpDeoxysStats.Add(3, New Byte() {50, 95, 90, 180, 95, 90})

        dpWormadamStats.Add(0, New Byte() {60, 59, 85, 36, 79, 105, Types.Bug, Types.Grass})
        dpWormadamStats.Add(1, New Byte() {60, 79, 105, 36, 59, 85, Types.Bug, Types.Ground})
        dpWormadamStats.Add(2, New Byte() {60, 69, 95, 36, 69, 95, Types.Bug, Types.Steel})

        dpRotomStats.Add(0, New Byte() {50, 50, 77, 91, 95, 77})
        dpRotomStats.Add(1, New Byte() {50, 65, 107, 86, 105, 107})
        dpRotomStats.Add(2, New Byte() {50, 65, 107, 86, 105, 107})
        dpRotomStats.Add(3, New Byte() {50, 65, 107, 86, 105, 107})
        dpRotomStats.Add(4, New Byte() {50, 65, 107, 86, 105, 107})
        dpRotomStats.Add(5, New Byte() {50, 65, 107, 86, 105, 107})

        dpGiratinaStats.Add(0, New Byte() {150, 100, 120, 90, 100, 120})
        dpGiratinaStats.Add(1, New Byte() {150, 120, 100, 90, 120, 100})

        dpShayminStats.Add(0, New Byte() {100, 100, 100, 100, 100, 100, Types.Grass, Types.Grass})
        dpShayminStats.Add(1, New Byte() {100, 103, 75, 127, 120, 75, Types.Grass, Types.Flying})

        dpBoxIcons.Add(0, Nothing)
        dpBoxIcons.Add(1, My.Resources.Box0010)
        dpBoxIcons.Add(2, My.Resources.Box0020)
        dpBoxIcons.Add(3, My.Resources.Box0030)
        dpBoxIcons.Add(4, My.Resources.Box0040)
        dpBoxIcons.Add(5, My.Resources.Box0050)
        dpBoxIcons.Add(6, My.Resources.Box0060)
        dpBoxIcons.Add(7, My.Resources.Box0070)
        dpBoxIcons.Add(8, My.Resources.Box0080)
        dpBoxIcons.Add(9, My.Resources.Box0090)
        dpBoxIcons.Add(10, My.Resources.Box0100)
        dpBoxIcons.Add(11, My.Resources.Box0110)
        dpBoxIcons.Add(12, My.Resources.Box0120)
        dpBoxIcons.Add(13, My.Resources.Box0130)
        dpBoxIcons.Add(14, My.Resources.Box0140)
        dpBoxIcons.Add(15, My.Resources.Box0150)
        dpBoxIcons.Add(16, My.Resources.Box0160)
        dpBoxIcons.Add(17, My.Resources.Box0170)
        dpBoxIcons.Add(18, My.Resources.Box0180)
        dpBoxIcons.Add(19, My.Resources.Box0190)
        dpBoxIcons.Add(20, My.Resources.Box0200)
        dpBoxIcons.Add(21, My.Resources.Box0210)
        dpBoxIcons.Add(22, My.Resources.Box0220)
        dpBoxIcons.Add(23, My.Resources.Box0230)
        dpBoxIcons.Add(24, My.Resources.Box0240)
        dpBoxIcons.Add(25, My.Resources.Box0250)
        dpBoxIcons.Add(26, My.Resources.Box0260)
        dpBoxIcons.Add(27, My.Resources.Box0270)
        dpBoxIcons.Add(28, My.Resources.Box0280)
        dpBoxIcons.Add(29, My.Resources.Box0290)
        dpBoxIcons.Add(30, My.Resources.Box0300)
        dpBoxIcons.Add(31, My.Resources.Box0310)
        dpBoxIcons.Add(32, My.Resources.Box0320)
        dpBoxIcons.Add(33, My.Resources.Box0330)
        dpBoxIcons.Add(34, My.Resources.Box0340)
        dpBoxIcons.Add(35, My.Resources.Box0350)
        dpBoxIcons.Add(36, My.Resources.Box0360)
        dpBoxIcons.Add(37, My.Resources.Box0370)
        dpBoxIcons.Add(38, My.Resources.Box0380)
        dpBoxIcons.Add(39, My.Resources.Box0390)
        dpBoxIcons.Add(40, My.Resources.Box0400)
        dpBoxIcons.Add(41, My.Resources.Box0410)
        dpBoxIcons.Add(42, My.Resources.Box0420)
        dpBoxIcons.Add(43, My.Resources.Box0430)
        dpBoxIcons.Add(44, My.Resources.Box0440)
        dpBoxIcons.Add(45, My.Resources.Box0450)
        dpBoxIcons.Add(46, My.Resources.Box0460)
        dpBoxIcons.Add(47, My.Resources.Box0470)
        dpBoxIcons.Add(48, My.Resources.Box0480)
        dpBoxIcons.Add(49, My.Resources.Box0490)
        dpBoxIcons.Add(50, My.Resources.Box0500)
        dpBoxIcons.Add(51, My.Resources.Box0510)
        dpBoxIcons.Add(52, My.Resources.Box0520)
        dpBoxIcons.Add(53, My.Resources.Box0530)
        dpBoxIcons.Add(54, My.Resources.Box0540)
        dpBoxIcons.Add(55, My.Resources.Box0550)
        dpBoxIcons.Add(56, My.Resources.Box0560)
        dpBoxIcons.Add(57, My.Resources.Box0570)
        dpBoxIcons.Add(58, My.Resources.Box0580)
        dpBoxIcons.Add(59, My.Resources.Box0590)
        dpBoxIcons.Add(60, My.Resources.Box0600)
        dpBoxIcons.Add(61, My.Resources.Box0610)
        dpBoxIcons.Add(62, My.Resources.Box0620)
        dpBoxIcons.Add(63, My.Resources.Box0630)
        dpBoxIcons.Add(64, My.Resources.Box0640)
        dpBoxIcons.Add(65, My.Resources.Box0650)
        dpBoxIcons.Add(66, My.Resources.Box0660)
        dpBoxIcons.Add(67, My.Resources.Box0670)
        dpBoxIcons.Add(68, My.Resources.Box0680)
        dpBoxIcons.Add(69, My.Resources.Box0690)
        dpBoxIcons.Add(70, My.Resources.Box0700)
        dpBoxIcons.Add(71, My.Resources.Box0710)
        dpBoxIcons.Add(72, My.Resources.Box0720)
        dpBoxIcons.Add(73, My.Resources.Box0730)
        dpBoxIcons.Add(74, My.Resources.Box0740)
        dpBoxIcons.Add(75, My.Resources.Box0750)
        dpBoxIcons.Add(76, My.Resources.Box0760)
        dpBoxIcons.Add(77, My.Resources.Box0770)
        dpBoxIcons.Add(78, My.Resources.Box0780)
        dpBoxIcons.Add(79, My.Resources.Box0790)
        dpBoxIcons.Add(80, My.Resources.Box0800)
        dpBoxIcons.Add(81, My.Resources.Box0810)
        dpBoxIcons.Add(82, My.Resources.Box0820)
        dpBoxIcons.Add(83, My.Resources.Box0830)
        dpBoxIcons.Add(84, My.Resources.Box0840)
        dpBoxIcons.Add(85, My.Resources.Box0850)
        dpBoxIcons.Add(86, My.Resources.Box0860)
        dpBoxIcons.Add(87, My.Resources.Box0870)
        dpBoxIcons.Add(88, My.Resources.Box0880)
        dpBoxIcons.Add(89, My.Resources.Box0890)
        dpBoxIcons.Add(90, My.Resources.Box0900)
        dpBoxIcons.Add(91, My.Resources.Box0910)
        dpBoxIcons.Add(92, My.Resources.Box0920)
        dpBoxIcons.Add(93, My.Resources.Box0930)
        dpBoxIcons.Add(94, My.Resources.Box0940)
        dpBoxIcons.Add(95, My.Resources.Box0950)
        dpBoxIcons.Add(96, My.Resources.Box0960)
        dpBoxIcons.Add(97, My.Resources.Box0970)
        dpBoxIcons.Add(98, My.Resources.Box0980)
        dpBoxIcons.Add(99, My.Resources.Box0990)
        dpBoxIcons.Add(100, My.Resources.Box1000)
        dpBoxIcons.Add(101, My.Resources.Box1010)
        dpBoxIcons.Add(102, My.Resources.Box1020)
        dpBoxIcons.Add(103, My.Resources.Box1030)
        dpBoxIcons.Add(104, My.Resources.Box1040)
        dpBoxIcons.Add(105, My.Resources.Box1050)
        dpBoxIcons.Add(106, My.Resources.Box1060)
        dpBoxIcons.Add(107, My.Resources.Box1070)
        dpBoxIcons.Add(108, My.Resources.Box1080)
        dpBoxIcons.Add(109, My.Resources.Box1090)
        dpBoxIcons.Add(110, My.Resources.Box1100)
        dpBoxIcons.Add(111, My.Resources.Box1110)
        dpBoxIcons.Add(112, My.Resources.Box1120)
        dpBoxIcons.Add(113, My.Resources.Box1130)
        dpBoxIcons.Add(114, My.Resources.Box1140)
        dpBoxIcons.Add(115, My.Resources.Box1150)
        dpBoxIcons.Add(116, My.Resources.Box1160)
        dpBoxIcons.Add(117, My.Resources.Box1170)
        dpBoxIcons.Add(118, My.Resources.Box1180)
        dpBoxIcons.Add(119, My.Resources.Box1190)
        dpBoxIcons.Add(120, My.Resources.Box1200)
        dpBoxIcons.Add(121, My.Resources.Box1210)
        dpBoxIcons.Add(122, My.Resources.Box1220)
        dpBoxIcons.Add(123, My.Resources.Box1230)
        dpBoxIcons.Add(124, My.Resources.Box1240)
        dpBoxIcons.Add(125, My.Resources.Box1250)
        dpBoxIcons.Add(126, My.Resources.Box1260)
        dpBoxIcons.Add(127, My.Resources.Box1270)
        dpBoxIcons.Add(128, My.Resources.Box1280)
        dpBoxIcons.Add(129, My.Resources.Box1290)
        dpBoxIcons.Add(130, My.Resources.Box1300)
        dpBoxIcons.Add(131, My.Resources.Box1310)
        dpBoxIcons.Add(132, My.Resources.Box1320)
        dpBoxIcons.Add(133, My.Resources.Box1330)
        dpBoxIcons.Add(134, My.Resources.Box1340)
        dpBoxIcons.Add(135, My.Resources.Box1350)
        dpBoxIcons.Add(136, My.Resources.Box1360)
        dpBoxIcons.Add(137, My.Resources.Box1370)
        dpBoxIcons.Add(138, My.Resources.Box1380)
        dpBoxIcons.Add(139, My.Resources.Box1390)
        dpBoxIcons.Add(140, My.Resources.Box1400)
        dpBoxIcons.Add(141, My.Resources.Box1410)
        dpBoxIcons.Add(142, My.Resources.Box1420)
        dpBoxIcons.Add(143, My.Resources.Box1430)
        dpBoxIcons.Add(144, My.Resources.Box1440)
        dpBoxIcons.Add(145, My.Resources.Box1450)
        dpBoxIcons.Add(146, My.Resources.Box1460)
        dpBoxIcons.Add(147, My.Resources.Box1470)
        dpBoxIcons.Add(148, My.Resources.Box1480)
        dpBoxIcons.Add(149, My.Resources.Box1490)
        dpBoxIcons.Add(150, My.Resources.Box1500)
        dpBoxIcons.Add(151, My.Resources.Box1510)
        dpBoxIcons.Add(152, My.Resources.Box1520)
        dpBoxIcons.Add(153, My.Resources.Box1530)
        dpBoxIcons.Add(154, My.Resources.Box1540)
        dpBoxIcons.Add(155, My.Resources.Box1550)
        dpBoxIcons.Add(156, My.Resources.Box1560)
        dpBoxIcons.Add(157, My.Resources.Box1570)
        dpBoxIcons.Add(158, My.Resources.Box1580)
        dpBoxIcons.Add(159, My.Resources.Box1590)
        dpBoxIcons.Add(160, My.Resources.Box1600)
        dpBoxIcons.Add(161, My.Resources.Box1610)
        dpBoxIcons.Add(162, My.Resources.Box1620)
        dpBoxIcons.Add(163, My.Resources.Box1630)
        dpBoxIcons.Add(164, My.Resources.Box1640)
        dpBoxIcons.Add(165, My.Resources.Box1650)
        dpBoxIcons.Add(166, My.Resources.Box1660)
        dpBoxIcons.Add(167, My.Resources.Box1670)
        dpBoxIcons.Add(168, My.Resources.Box1680)
        dpBoxIcons.Add(169, My.Resources.Box1690)
        dpBoxIcons.Add(170, My.Resources.Box1700)
        dpBoxIcons.Add(171, My.Resources.Box1710)
        dpBoxIcons.Add(172, My.Resources.Box1720)
        dpBoxIcons.Add(173, My.Resources.Box1730)
        dpBoxIcons.Add(174, My.Resources.Box1740)
        dpBoxIcons.Add(175, My.Resources.Box1750)
        dpBoxIcons.Add(176, My.Resources.Box1760)
        dpBoxIcons.Add(177, My.Resources.Box1770)
        dpBoxIcons.Add(178, My.Resources.Box1780)
        dpBoxIcons.Add(179, My.Resources.Box1790)
        dpBoxIcons.Add(180, My.Resources.Box1800)
        dpBoxIcons.Add(181, My.Resources.Box1810)
        dpBoxIcons.Add(182, My.Resources.Box1820)
        dpBoxIcons.Add(183, My.Resources.Box1830)
        dpBoxIcons.Add(184, My.Resources.Box1840)
        dpBoxIcons.Add(185, My.Resources.Box1850)
        dpBoxIcons.Add(186, My.Resources.Box1860)
        dpBoxIcons.Add(187, My.Resources.Box1870)
        dpBoxIcons.Add(188, My.Resources.Box1880)
        dpBoxIcons.Add(189, My.Resources.Box1890)
        dpBoxIcons.Add(190, My.Resources.Box1900)
        dpBoxIcons.Add(191, My.Resources.Box1910)
        dpBoxIcons.Add(192, My.Resources.Box1920)
        dpBoxIcons.Add(193, My.Resources.Box1930)
        dpBoxIcons.Add(194, My.Resources.Box1940)
        dpBoxIcons.Add(195, My.Resources.Box1950)
        dpBoxIcons.Add(196, My.Resources.Box1960)
        dpBoxIcons.Add(197, My.Resources.Box1970)
        dpBoxIcons.Add(198, My.Resources.Box1980)
        dpBoxIcons.Add(199, My.Resources.Box1990)
        dpBoxIcons.Add(200, My.Resources.Box2000)
        dpBoxIcons.Add(201, My.Resources.Box2010)
        dpBoxIcons.Add(202, My.Resources.Box2020)
        dpBoxIcons.Add(203, My.Resources.Box2030)
        dpBoxIcons.Add(204, My.Resources.Box2040)
        dpBoxIcons.Add(205, My.Resources.Box2050)
        dpBoxIcons.Add(206, My.Resources.Box2060)
        dpBoxIcons.Add(207, My.Resources.Box2070)
        dpBoxIcons.Add(208, My.Resources.Box2080)
        dpBoxIcons.Add(209, My.Resources.Box2090)
        dpBoxIcons.Add(210, My.Resources.Box2100)
        dpBoxIcons.Add(211, My.Resources.Box2110)
        dpBoxIcons.Add(212, My.Resources.Box2120)
        dpBoxIcons.Add(213, My.Resources.Box2130)
        dpBoxIcons.Add(214, My.Resources.Box2140)
        dpBoxIcons.Add(215, My.Resources.Box2150)
        dpBoxIcons.Add(216, My.Resources.Box2160)
        dpBoxIcons.Add(217, My.Resources.Box2170)
        dpBoxIcons.Add(218, My.Resources.Box2180)
        dpBoxIcons.Add(219, My.Resources.Box2190)
        dpBoxIcons.Add(220, My.Resources.Box2200)
        dpBoxIcons.Add(221, My.Resources.Box2210)
        dpBoxIcons.Add(222, My.Resources.Box2220)
        dpBoxIcons.Add(223, My.Resources.Box2230)
        dpBoxIcons.Add(224, My.Resources.Box2240)
        dpBoxIcons.Add(225, My.Resources.Box2250)
        dpBoxIcons.Add(226, My.Resources.Box2260)
        dpBoxIcons.Add(227, My.Resources.Box2270)
        dpBoxIcons.Add(228, My.Resources.Box2280)
        dpBoxIcons.Add(229, My.Resources.Box2290)
        dpBoxIcons.Add(230, My.Resources.Box2300)
        dpBoxIcons.Add(231, My.Resources.Box2310)
        dpBoxIcons.Add(232, My.Resources.Box2320)
        dpBoxIcons.Add(233, My.Resources.Box2330)
        dpBoxIcons.Add(234, My.Resources.Box2340)
        dpBoxIcons.Add(235, My.Resources.Box2350)
        dpBoxIcons.Add(236, My.Resources.Box2360)
        dpBoxIcons.Add(237, My.Resources.Box2370)
        dpBoxIcons.Add(238, My.Resources.Box2380)
        dpBoxIcons.Add(239, My.Resources.Box2390)
        dpBoxIcons.Add(240, My.Resources.Box2400)
        dpBoxIcons.Add(241, My.Resources.Box2410)
        dpBoxIcons.Add(242, My.Resources.Box2420)
        dpBoxIcons.Add(243, My.Resources.Box2430)
        dpBoxIcons.Add(244, My.Resources.Box2440)
        dpBoxIcons.Add(245, My.Resources.Box2450)
        dpBoxIcons.Add(246, My.Resources.Box2460)
        dpBoxIcons.Add(247, My.Resources.Box2470)
        dpBoxIcons.Add(248, My.Resources.Box2480)
        dpBoxIcons.Add(249, My.Resources.Box2490)
        dpBoxIcons.Add(250, My.Resources.Box2500)
        dpBoxIcons.Add(251, My.Resources.Box2510)
        dpBoxIcons.Add(252, My.Resources.Box2520)
        dpBoxIcons.Add(253, My.Resources.Box2530)
        dpBoxIcons.Add(254, My.Resources.Box2540)
        dpBoxIcons.Add(255, My.Resources.Box2550)
        dpBoxIcons.Add(256, My.Resources.Box2560)
        dpBoxIcons.Add(257, My.Resources.Box2570)
        dpBoxIcons.Add(258, My.Resources.Box2580)
        dpBoxIcons.Add(259, My.Resources.Box2590)
        dpBoxIcons.Add(260, My.Resources.Box2600)
        dpBoxIcons.Add(261, My.Resources.Box2610)
        dpBoxIcons.Add(262, My.Resources.Box2620)
        dpBoxIcons.Add(263, My.Resources.Box2630)
        dpBoxIcons.Add(264, My.Resources.Box2640)
        dpBoxIcons.Add(265, My.Resources.Box2650)
        dpBoxIcons.Add(266, My.Resources.Box2660)
        dpBoxIcons.Add(267, My.Resources.Box2670)
        dpBoxIcons.Add(268, My.Resources.Box2680)
        dpBoxIcons.Add(269, My.Resources.Box2690)
        dpBoxIcons.Add(270, My.Resources.Box2700)
        dpBoxIcons.Add(271, My.Resources.Box2710)
        dpBoxIcons.Add(272, My.Resources.Box2720)
        dpBoxIcons.Add(273, My.Resources.Box2730)
        dpBoxIcons.Add(274, My.Resources.Box2740)
        dpBoxIcons.Add(275, My.Resources.Box2750)
        dpBoxIcons.Add(276, My.Resources.Box2760)
        dpBoxIcons.Add(277, My.Resources.Box2770)
        dpBoxIcons.Add(278, My.Resources.Box2780)
        dpBoxIcons.Add(279, My.Resources.Box2790)
        dpBoxIcons.Add(280, My.Resources.Box2800)
        dpBoxIcons.Add(281, My.Resources.Box2810)
        dpBoxIcons.Add(282, My.Resources.Box2820)
        dpBoxIcons.Add(283, My.Resources.Box2830)
        dpBoxIcons.Add(284, My.Resources.Box2840)
        dpBoxIcons.Add(285, My.Resources.Box2850)
        dpBoxIcons.Add(286, My.Resources.Box2860)
        dpBoxIcons.Add(287, My.Resources.Box2870)
        dpBoxIcons.Add(288, My.Resources.Box2880)
        dpBoxIcons.Add(289, My.Resources.Box2890)
        dpBoxIcons.Add(290, My.Resources.Box2900)
        dpBoxIcons.Add(291, My.Resources.Box2910)
        dpBoxIcons.Add(292, My.Resources.Box2920)
        dpBoxIcons.Add(293, My.Resources.Box2930)
        dpBoxIcons.Add(294, My.Resources.Box2940)
        dpBoxIcons.Add(295, My.Resources.Box2950)
        dpBoxIcons.Add(296, My.Resources.Box2960)
        dpBoxIcons.Add(297, My.Resources.Box2970)
        dpBoxIcons.Add(298, My.Resources.Box2980)
        dpBoxIcons.Add(299, My.Resources.Box2990)
        dpBoxIcons.Add(300, My.Resources.Box3000)
        dpBoxIcons.Add(301, My.Resources.Box3010)
        dpBoxIcons.Add(302, My.Resources.Box3020)
        dpBoxIcons.Add(303, My.Resources.Box3030)
        dpBoxIcons.Add(304, My.Resources.Box3040)
        dpBoxIcons.Add(305, My.Resources.Box3050)
        dpBoxIcons.Add(306, My.Resources.Box3060)
        dpBoxIcons.Add(307, My.Resources.Box3070)
        dpBoxIcons.Add(308, My.Resources.Box3080)
        dpBoxIcons.Add(309, My.Resources.Box3090)
        dpBoxIcons.Add(310, My.Resources.Box3100)
        dpBoxIcons.Add(311, My.Resources.Box3110)
        dpBoxIcons.Add(312, My.Resources.Box3120)
        dpBoxIcons.Add(313, My.Resources.Box3130)
        dpBoxIcons.Add(314, My.Resources.Box3140)
        dpBoxIcons.Add(315, My.Resources.Box3150)
        dpBoxIcons.Add(316, My.Resources.Box3160)
        dpBoxIcons.Add(317, My.Resources.Box3170)
        dpBoxIcons.Add(318, My.Resources.Box3180)
        dpBoxIcons.Add(319, My.Resources.Box3190)
        dpBoxIcons.Add(320, My.Resources.Box3200)
        dpBoxIcons.Add(321, My.Resources.Box3210)
        dpBoxIcons.Add(322, My.Resources.Box3220)
        dpBoxIcons.Add(323, My.Resources.Box3230)
        dpBoxIcons.Add(324, My.Resources.Box3240)
        dpBoxIcons.Add(325, My.Resources.Box3250)
        dpBoxIcons.Add(326, My.Resources.Box3260)
        dpBoxIcons.Add(327, My.Resources.Box3270)
        dpBoxIcons.Add(328, My.Resources.Box3280)
        dpBoxIcons.Add(329, My.Resources.Box3290)
        dpBoxIcons.Add(330, My.Resources.Box3300)
        dpBoxIcons.Add(331, My.Resources.Box3310)
        dpBoxIcons.Add(332, My.Resources.Box3320)
        dpBoxIcons.Add(333, My.Resources.Box3330)
        dpBoxIcons.Add(334, My.Resources.Box3340)
        dpBoxIcons.Add(335, My.Resources.Box3350)
        dpBoxIcons.Add(336, My.Resources.Box3360)
        dpBoxIcons.Add(337, My.Resources.Box3370)
        dpBoxIcons.Add(338, My.Resources.Box3380)
        dpBoxIcons.Add(339, My.Resources.Box3390)
        dpBoxIcons.Add(340, My.Resources.Box3400)
        dpBoxIcons.Add(341, My.Resources.Box3410)
        dpBoxIcons.Add(342, My.Resources.Box3420)
        dpBoxIcons.Add(343, My.Resources.Box3430)
        dpBoxIcons.Add(344, My.Resources.Box3440)
        dpBoxIcons.Add(345, My.Resources.Box3450)
        dpBoxIcons.Add(346, My.Resources.Box3460)
        dpBoxIcons.Add(347, My.Resources.Box3470)
        dpBoxIcons.Add(348, My.Resources.Box3480)
        dpBoxIcons.Add(349, My.Resources.Box3490)
        dpBoxIcons.Add(350, My.Resources.Box3500)
        dpBoxIcons.Add(351, My.Resources.Box3510)
        dpBoxIcons.Add(352, My.Resources.Box3520)
        dpBoxIcons.Add(353, My.Resources.Box3530)
        dpBoxIcons.Add(354, My.Resources.Box3540)
        dpBoxIcons.Add(355, My.Resources.Box3550)
        dpBoxIcons.Add(356, My.Resources.Box3560)
        dpBoxIcons.Add(357, My.Resources.Box3570)
        dpBoxIcons.Add(358, My.Resources.Box3580)
        dpBoxIcons.Add(359, My.Resources.Box3590)
        dpBoxIcons.Add(360, My.Resources.Box3600)
        dpBoxIcons.Add(361, My.Resources.Box3610)
        dpBoxIcons.Add(362, My.Resources.Box3620)
        dpBoxIcons.Add(363, My.Resources.Box3630)
        dpBoxIcons.Add(364, My.Resources.Box3640)
        dpBoxIcons.Add(365, My.Resources.Box3650)
        dpBoxIcons.Add(366, My.Resources.Box3660)
        dpBoxIcons.Add(367, My.Resources.Box3670)
        dpBoxIcons.Add(368, My.Resources.Box3680)
        dpBoxIcons.Add(369, My.Resources.Box3690)
        dpBoxIcons.Add(370, My.Resources.Box3700)
        dpBoxIcons.Add(371, My.Resources.Box3710)
        dpBoxIcons.Add(372, My.Resources.Box3720)
        dpBoxIcons.Add(373, My.Resources.Box3730)
        dpBoxIcons.Add(374, My.Resources.Box3740)
        dpBoxIcons.Add(375, My.Resources.Box3750)
        dpBoxIcons.Add(376, My.Resources.Box3760)
        dpBoxIcons.Add(377, My.Resources.Box3770)
        dpBoxIcons.Add(378, My.Resources.Box3780)
        dpBoxIcons.Add(379, My.Resources.Box3790)
        dpBoxIcons.Add(380, My.Resources.Box3800)
        dpBoxIcons.Add(381, My.Resources.Box3810)
        dpBoxIcons.Add(382, My.Resources.Box3820)
        dpBoxIcons.Add(383, My.Resources.Box3830)
        dpBoxIcons.Add(384, My.Resources.Box3840)
        dpBoxIcons.Add(385, My.Resources.Box3850)
        dpBoxIcons.Add(386, My.Resources.Box3860)
        dpBoxIcons.Add(387, My.Resources.Box3870)
        dpBoxIcons.Add(388, My.Resources.Box3880)
        dpBoxIcons.Add(389, My.Resources.Box3890)
        dpBoxIcons.Add(390, My.Resources.Box3900)
        dpBoxIcons.Add(391, My.Resources.Box3910)
        dpBoxIcons.Add(392, My.Resources.Box3920)
        dpBoxIcons.Add(393, My.Resources.Box3930)
        dpBoxIcons.Add(394, My.Resources.Box3940)
        dpBoxIcons.Add(395, My.Resources.Box3950)
        dpBoxIcons.Add(396, My.Resources.Box3960)
        dpBoxIcons.Add(397, My.Resources.Box3970)
        dpBoxIcons.Add(398, My.Resources.Box3980)
        dpBoxIcons.Add(399, My.Resources.Box3990)
        dpBoxIcons.Add(400, My.Resources.Box4000)
        dpBoxIcons.Add(401, My.Resources.Box4010)
        dpBoxIcons.Add(402, My.Resources.Box4020)
        dpBoxIcons.Add(403, My.Resources.Box4030)
        dpBoxIcons.Add(404, My.Resources.Box4040)
        dpBoxIcons.Add(405, My.Resources.Box4050)
        dpBoxIcons.Add(406, My.Resources.Box4060)
        dpBoxIcons.Add(407, My.Resources.Box4070)
        dpBoxIcons.Add(408, My.Resources.Box4080)
        dpBoxIcons.Add(409, My.Resources.Box4090)
        dpBoxIcons.Add(410, My.Resources.Box4100)
        dpBoxIcons.Add(411, My.Resources.Box4110)
        dpBoxIcons.Add(412, My.Resources.Box4120)
        dpBoxIcons.Add(413, My.Resources.Box4130)
        dpBoxIcons.Add(414, My.Resources.Box4140)
        dpBoxIcons.Add(415, My.Resources.Box4150)
        dpBoxIcons.Add(416, My.Resources.Box4160)
        dpBoxIcons.Add(417, My.Resources.Box4170)
        dpBoxIcons.Add(418, My.Resources.Box4180)
        dpBoxIcons.Add(419, My.Resources.Box4190)
        dpBoxIcons.Add(420, My.Resources.Box4200)
        dpBoxIcons.Add(421, My.Resources.Box4210)
        dpBoxIcons.Add(422, My.Resources.Box4220)
        dpBoxIcons.Add(423, My.Resources.Box4230)
        dpBoxIcons.Add(424, My.Resources.Box4240)
        dpBoxIcons.Add(425, My.Resources.Box4250)
        dpBoxIcons.Add(426, My.Resources.Box4260)
        dpBoxIcons.Add(427, My.Resources.Box4270)
        dpBoxIcons.Add(428, My.Resources.Box4280)
        dpBoxIcons.Add(429, My.Resources.Box4290)
        dpBoxIcons.Add(430, My.Resources.Box4300)
        dpBoxIcons.Add(431, My.Resources.Box4310)
        dpBoxIcons.Add(432, My.Resources.Box4320)
        dpBoxIcons.Add(433, My.Resources.Box4330)
        dpBoxIcons.Add(434, My.Resources.Box4340)
        dpBoxIcons.Add(435, My.Resources.Box4350)
        dpBoxIcons.Add(436, My.Resources.Box4360)
        dpBoxIcons.Add(437, My.Resources.Box4370)
        dpBoxIcons.Add(438, My.Resources.Box4380)
        dpBoxIcons.Add(439, My.Resources.Box4390)
        dpBoxIcons.Add(440, My.Resources.Box4400)
        dpBoxIcons.Add(441, My.Resources.Box4410)
        dpBoxIcons.Add(442, My.Resources.Box4420)
        dpBoxIcons.Add(443, My.Resources.Box4430)
        dpBoxIcons.Add(444, My.Resources.Box4440)
        dpBoxIcons.Add(445, My.Resources.Box4450)
        dpBoxIcons.Add(446, My.Resources.Box4460)
        dpBoxIcons.Add(447, My.Resources.Box4470)
        dpBoxIcons.Add(448, My.Resources.Box4480)
        dpBoxIcons.Add(449, My.Resources.Box4490)
        dpBoxIcons.Add(450, My.Resources.Box4500)
        dpBoxIcons.Add(451, My.Resources.Box4510)
        dpBoxIcons.Add(452, My.Resources.Box4520)
        dpBoxIcons.Add(453, My.Resources.Box4530)
        dpBoxIcons.Add(454, My.Resources.Box4540)
        dpBoxIcons.Add(455, My.Resources.Box4550)
        dpBoxIcons.Add(456, My.Resources.Box4560)
        dpBoxIcons.Add(457, My.Resources.Box4570)
        dpBoxIcons.Add(458, My.Resources.Box4580)
        dpBoxIcons.Add(459, My.Resources.Box4590)
        dpBoxIcons.Add(460, My.Resources.Box4600)
        dpBoxIcons.Add(461, My.Resources.Box4610)
        dpBoxIcons.Add(462, My.Resources.Box4620)
        dpBoxIcons.Add(463, My.Resources.Box4630)
        dpBoxIcons.Add(464, My.Resources.Box4640)
        dpBoxIcons.Add(465, My.Resources.Box4650)
        dpBoxIcons.Add(466, My.Resources.Box4660)
        dpBoxIcons.Add(467, My.Resources.Box4670)
        dpBoxIcons.Add(468, My.Resources.Box4680)
        dpBoxIcons.Add(469, My.Resources.Box4690)
        dpBoxIcons.Add(470, My.Resources.Box4700)
        dpBoxIcons.Add(471, My.Resources.Box4710)
        dpBoxIcons.Add(472, My.Resources.Box4720)
        dpBoxIcons.Add(473, My.Resources.Box4730)
        dpBoxIcons.Add(474, My.Resources.Box4740)
        dpBoxIcons.Add(475, My.Resources.Box4750)
        dpBoxIcons.Add(476, My.Resources.Box4760)
        dpBoxIcons.Add(477, My.Resources.Box4770)
        dpBoxIcons.Add(478, My.Resources.Box4780)
        dpBoxIcons.Add(479, My.Resources.Box4790)
        dpBoxIcons.Add(480, My.Resources.Box4800)
        dpBoxIcons.Add(481, My.Resources.Box4810)
        dpBoxIcons.Add(482, My.Resources.Box4820)
        dpBoxIcons.Add(483, My.Resources.Box4830)
        dpBoxIcons.Add(484, My.Resources.Box4840)
        dpBoxIcons.Add(485, My.Resources.Box4850)
        dpBoxIcons.Add(486, My.Resources.Box4860)
        dpBoxIcons.Add(487, My.Resources.Box4870)
        dpBoxIcons.Add(488, My.Resources.Box4880)
        dpBoxIcons.Add(489, My.Resources.Box4890)
        dpBoxIcons.Add(490, My.Resources.Box4900)
        dpBoxIcons.Add(491, My.Resources.Box4910)
        dpBoxIcons.Add(492, My.Resources.Box4920)
        dpBoxIcons.Add(493, My.Resources.Box4930)

        dpUnownBoxIcons.Add(0, My.Resources.Box2010)
        dpUnownBoxIcons.Add(1, My.Resources.Box2011)
        dpUnownBoxIcons.Add(2, My.Resources.Box2012)
        dpUnownBoxIcons.Add(3, My.Resources.Box2013)
        dpUnownBoxIcons.Add(4, My.Resources.Box2014)
        dpUnownBoxIcons.Add(5, My.Resources.Box2015)
        dpUnownBoxIcons.Add(6, My.Resources.Box2016)
        dpUnownBoxIcons.Add(7, My.Resources.Box2017)
        dpUnownBoxIcons.Add(8, My.Resources.Box2018)
        dpUnownBoxIcons.Add(9, My.Resources.Box2019)
        dpUnownBoxIcons.Add(10, My.Resources.Box20110)
        dpUnownBoxIcons.Add(11, My.Resources.Box20111)
        dpUnownBoxIcons.Add(12, My.Resources.Box20112)
        dpUnownBoxIcons.Add(13, My.Resources.Box20113)
        dpUnownBoxIcons.Add(14, My.Resources.Box20114)
        dpUnownBoxIcons.Add(15, My.Resources.Box20115)
        dpUnownBoxIcons.Add(16, My.Resources.Box20116)
        dpUnownBoxIcons.Add(17, My.Resources.Box20117)
        dpUnownBoxIcons.Add(18, My.Resources.Box20118)
        dpUnownBoxIcons.Add(19, My.Resources.Box20119)
        dpUnownBoxIcons.Add(20, My.Resources.Box20120)
        dpUnownBoxIcons.Add(21, My.Resources.Box20121)
        dpUnownBoxIcons.Add(22, My.Resources.Box20122)
        dpUnownBoxIcons.Add(23, My.Resources.Box20123)
        dpUnownBoxIcons.Add(24, My.Resources.Box20124)
        dpUnownBoxIcons.Add(25, My.Resources.Box20125)
        dpUnownBoxIcons.Add(26, My.Resources.Box20126)
        dpUnownBoxIcons.Add(27, My.Resources.Box20127)

        dpDeoxysBoxIcons.Add(0, My.Resources.Box3860)
        dpDeoxysBoxIcons.Add(1, My.Resources.Box3861)
        dpDeoxysBoxIcons.Add(2, My.Resources.Box3862)
        dpDeoxysBoxIcons.Add(3, My.Resources.Box3863)

        dpBurmyBoxIcons.Add(0, My.Resources.Box4120)
        dpBurmyBoxIcons.Add(1, My.Resources.Box4121)
        dpBurmyBoxIcons.Add(2, My.Resources.Box4122)

        dpWormadamBoxIcons.Add(0, My.Resources.Box4130)
        dpWormadamBoxIcons.Add(1, My.Resources.Box4131)
        dpWormadamBoxIcons.Add(2, My.Resources.Box4132)

        dpShellosBoxIcons.Add(0, My.Resources.Box4220)
        dpShellosBoxIcons.Add(1, My.Resources.Box4221)

        dpGastrodonBoxIcons.Add(0, My.Resources.Box4230)
        dpGastrodonBoxIcons.Add(1, My.Resources.Box4231)

        dpRotomBoxIcons.Add(0, My.Resources.Box4790)
        dpRotomBoxIcons.Add(1, My.Resources.Box4791)
        dpRotomBoxIcons.Add(2, My.Resources.Box4792)
        dpRotomBoxIcons.Add(3, My.Resources.Box4793)
        dpRotomBoxIcons.Add(4, My.Resources.Box4794)
        dpRotomBoxIcons.Add(5, My.Resources.Box4795)

        dpGiratinaBoxIcons.Add(0, My.Resources.Box4870)
        dpGiratinaBoxIcons.Add(1, My.Resources.Box4871)

        dpShayminBoxIcons.Add(0, My.Resources.Box4920)
        dpShayminBoxIcons.Add(1, My.Resources.Box4921)

        dpTypeIcons.Add(0, My.Resources.Type00)
        dpTypeIcons.Add(1, My.Resources.Type01)
        dpTypeIcons.Add(2, My.Resources.Type02)
        dpTypeIcons.Add(3, My.Resources.Type03)
        dpTypeIcons.Add(4, My.Resources.Type04)
        dpTypeIcons.Add(5, My.Resources.Type05)
        dpTypeIcons.Add(6, My.Resources.Type06)
        dpTypeIcons.Add(7, My.Resources.Type07)
        dpTypeIcons.Add(8, My.Resources.Type08)
        dpTypeIcons.Add(9, My.Resources.Type09)
        dpTypeIcons.Add(10, My.Resources.Type10)
        dpTypeIcons.Add(11, My.Resources.Type11)
        dpTypeIcons.Add(12, My.Resources.Type12)
        dpTypeIcons.Add(13, My.Resources.Type13)
        dpTypeIcons.Add(14, My.Resources.Type14)
        dpTypeIcons.Add(15, My.Resources.Type15)
        dpTypeIcons.Add(16, My.Resources.Type16)
        dpTypeIcons.Add(17, My.Resources.Type17)

        dpItemImages.Add(0, Nothing)
        dpItemImages.Add(1, My.Resources.Item001)
        dpItemImages.Add(2, My.Resources.Item002)
        dpItemImages.Add(3, My.Resources.Item003)
        dpItemImages.Add(4, My.Resources.Item004)
        dpItemImages.Add(5, My.Resources.Item005)
        dpItemImages.Add(6, My.Resources.Item006)
        dpItemImages.Add(7, My.Resources.Item007)
        dpItemImages.Add(8, My.Resources.Item008)
        dpItemImages.Add(9, My.Resources.Item009)
        dpItemImages.Add(10, My.Resources.Item010)
        dpItemImages.Add(11, My.Resources.Item011)
        dpItemImages.Add(12, My.Resources.Item012)
        dpItemImages.Add(13, My.Resources.Item013)
        dpItemImages.Add(14, My.Resources.Item014)
        dpItemImages.Add(15, My.Resources.Item015)
        dpItemImages.Add(16, My.Resources.Item016)
        dpItemImages.Add(17, My.Resources.Item017)
        dpItemImages.Add(18, My.Resources.Item018)
        dpItemImages.Add(19, My.Resources.Item019)
        dpItemImages.Add(20, My.Resources.Item020)
        dpItemImages.Add(21, My.Resources.Item021)
        dpItemImages.Add(22, My.Resources.Item022)
        dpItemImages.Add(23, My.Resources.Item023)
        dpItemImages.Add(24, My.Resources.Item024)
        dpItemImages.Add(25, My.Resources.Item025)
        dpItemImages.Add(26, My.Resources.Item026)
        dpItemImages.Add(27, My.Resources.Item027)
        dpItemImages.Add(28, My.Resources.Item028)
        dpItemImages.Add(29, My.Resources.Item029)
        dpItemImages.Add(30, My.Resources.Item030)
        dpItemImages.Add(31, My.Resources.Item031)
        dpItemImages.Add(32, My.Resources.Item032)
        dpItemImages.Add(33, My.Resources.Item033)
        dpItemImages.Add(34, My.Resources.Item034)
        dpItemImages.Add(35, My.Resources.Item035)
        dpItemImages.Add(36, My.Resources.Item036)
        dpItemImages.Add(37, My.Resources.Item037)
        dpItemImages.Add(38, My.Resources.Item038)
        dpItemImages.Add(39, My.Resources.Item039)
        dpItemImages.Add(40, My.Resources.Item040)
        dpItemImages.Add(41, My.Resources.Item041)
        dpItemImages.Add(42, My.Resources.Item042)
        dpItemImages.Add(43, My.Resources.Item043)
        dpItemImages.Add(44, My.Resources.Item044)
        dpItemImages.Add(45, My.Resources.Item045)
        dpItemImages.Add(46, My.Resources.Item046)
        dpItemImages.Add(47, My.Resources.Item047)
        dpItemImages.Add(48, My.Resources.Item048)
        dpItemImages.Add(49, My.Resources.Item049)
        dpItemImages.Add(50, My.Resources.Item050)
        dpItemImages.Add(51, My.Resources.Item051)
        dpItemImages.Add(52, My.Resources.Item052)
        dpItemImages.Add(53, My.Resources.Item053)
        dpItemImages.Add(54, My.Resources.Item054)
        dpItemImages.Add(55, My.Resources.Item055)
        dpItemImages.Add(56, My.Resources.Item056)
        dpItemImages.Add(57, My.Resources.Item057)
        dpItemImages.Add(58, My.Resources.Item058)
        dpItemImages.Add(59, My.Resources.Item059)
        dpItemImages.Add(60, My.Resources.Item060)
        dpItemImages.Add(61, My.Resources.Item061)
        dpItemImages.Add(62, My.Resources.Item062)
        dpItemImages.Add(63, My.Resources.Item063)
        dpItemImages.Add(64, My.Resources.Item064)
        dpItemImages.Add(65, My.Resources.Item065)
        dpItemImages.Add(66, My.Resources.Item066)
        dpItemImages.Add(67, My.Resources.Item067)
        dpItemImages.Add(68, My.Resources.Item068)
        dpItemImages.Add(69, My.Resources.Item069)
        dpItemImages.Add(70, My.Resources.Item070)
        dpItemImages.Add(71, My.Resources.Item071)
        dpItemImages.Add(72, My.Resources.Item072)
        dpItemImages.Add(73, My.Resources.Item073)
        dpItemImages.Add(74, My.Resources.Item074)
        dpItemImages.Add(75, My.Resources.Item075)
        dpItemImages.Add(76, My.Resources.Item076)
        dpItemImages.Add(77, My.Resources.Item077)
        dpItemImages.Add(78, My.Resources.Item078)
        dpItemImages.Add(79, My.Resources.Item079)
        dpItemImages.Add(80, My.Resources.Item080)
        dpItemImages.Add(81, My.Resources.Item081)
        dpItemImages.Add(82, My.Resources.Item082)
        dpItemImages.Add(83, My.Resources.Item083)
        dpItemImages.Add(84, My.Resources.Item084)
        dpItemImages.Add(85, My.Resources.Item085)
        dpItemImages.Add(86, My.Resources.Item086)
        dpItemImages.Add(87, My.Resources.Item087)
        dpItemImages.Add(88, My.Resources.Item088)
        dpItemImages.Add(89, My.Resources.Item089)
        dpItemImages.Add(90, My.Resources.Item090)
        dpItemImages.Add(91, My.Resources.Item091)
        dpItemImages.Add(92, My.Resources.Item092)
        dpItemImages.Add(93, My.Resources.Item093)
        dpItemImages.Add(94, My.Resources.Item094)
        dpItemImages.Add(95, My.Resources.Item095)
        dpItemImages.Add(96, My.Resources.Item096)
        dpItemImages.Add(97, My.Resources.Item097)
        dpItemImages.Add(98, My.Resources.Item098)
        dpItemImages.Add(99, My.Resources.Item099)
        dpItemImages.Add(100, My.Resources.Item100)
        dpItemImages.Add(101, My.Resources.Item101)
        dpItemImages.Add(102, My.Resources.Item102)
        dpItemImages.Add(103, My.Resources.Item103)
        dpItemImages.Add(104, My.Resources.Item104)
        dpItemImages.Add(105, My.Resources.Item105)
        dpItemImages.Add(106, My.Resources.Item106)
        dpItemImages.Add(107, My.Resources.Item107)
        dpItemImages.Add(108, My.Resources.Item108)
        dpItemImages.Add(109, My.Resources.Item109)
        dpItemImages.Add(110, My.Resources.Item110)
        dpItemImages.Add(111, My.Resources.Item111)
        dpItemImages.Add(112, My.Resources.Item112)
        dpItemImages.Add(&H71, Nothing)
        dpItemImages.Add(&H72, Nothing)
        dpItemImages.Add(&H73, Nothing)
        dpItemImages.Add(&H74, Nothing)
        dpItemImages.Add(&H75, Nothing)
        dpItemImages.Add(&H76, Nothing)
        dpItemImages.Add(&H77, Nothing)
        dpItemImages.Add(&H78, Nothing)
        dpItemImages.Add(&H79, Nothing)
        dpItemImages.Add(&H7A, Nothing)
        dpItemImages.Add(&H7B, Nothing)
        dpItemImages.Add(&H7C, Nothing)
        dpItemImages.Add(&H7D, Nothing)
        dpItemImages.Add(&H7E, Nothing)
        dpItemImages.Add(&H7F, Nothing)
        dpItemImages.Add(&H80, Nothing)
        dpItemImages.Add(&H81, Nothing)
        dpItemImages.Add(&H82, Nothing)
        dpItemImages.Add(&H83, Nothing)
        dpItemImages.Add(&H84, Nothing)
        dpItemImages.Add(&H85, Nothing)
        dpItemImages.Add(&H86, Nothing)
        dpItemImages.Add(135, My.Resources.Item135)
        dpItemImages.Add(136, My.Resources.Item136)
        dpItemImages.Add(137, My.Resources.Item137)
        dpItemImages.Add(138, My.Resources.Item138)
        dpItemImages.Add(139, My.Resources.Item139)
        dpItemImages.Add(140, My.Resources.Item140)
        dpItemImages.Add(141, My.Resources.Item141)
        dpItemImages.Add(142, My.Resources.Item142)
        dpItemImages.Add(143, My.Resources.Item143)
        dpItemImages.Add(144, My.Resources.Item144)
        dpItemImages.Add(145, My.Resources.Item145)
        dpItemImages.Add(146, My.Resources.Item146)
        dpItemImages.Add(147, My.Resources.Item147)
        dpItemImages.Add(148, My.Resources.Item148)
        dpItemImages.Add(149, My.Resources.Item149)
        dpItemImages.Add(150, My.Resources.Item150)
        dpItemImages.Add(151, My.Resources.Item151)
        dpItemImages.Add(152, My.Resources.Item152)
        dpItemImages.Add(153, My.Resources.Item153)
        dpItemImages.Add(154, My.Resources.Item154)
        dpItemImages.Add(155, My.Resources.Item155)
        dpItemImages.Add(156, My.Resources.Item156)
        dpItemImages.Add(157, My.Resources.Item157)
        dpItemImages.Add(158, My.Resources.Item158)
        dpItemImages.Add(159, My.Resources.Item159)
        dpItemImages.Add(160, My.Resources.Item160)
        dpItemImages.Add(161, My.Resources.Item161)
        dpItemImages.Add(162, My.Resources.Item162)
        dpItemImages.Add(163, My.Resources.Item163)
        dpItemImages.Add(164, My.Resources.Item164)
        dpItemImages.Add(165, My.Resources.Item165)
        dpItemImages.Add(166, My.Resources.Item166)
        dpItemImages.Add(167, My.Resources.Item167)
        dpItemImages.Add(168, My.Resources.Item168)
        dpItemImages.Add(169, My.Resources.Item169)
        dpItemImages.Add(170, My.Resources.Item170)
        dpItemImages.Add(171, My.Resources.Item171)
        dpItemImages.Add(172, My.Resources.Item172)
        dpItemImages.Add(173, My.Resources.Item173)
        dpItemImages.Add(174, My.Resources.Item174)
        dpItemImages.Add(175, My.Resources.Item175)
        dpItemImages.Add(176, My.Resources.Item176)
        dpItemImages.Add(177, My.Resources.Item177)
        dpItemImages.Add(178, My.Resources.Item178)
        dpItemImages.Add(179, My.Resources.Item179)
        dpItemImages.Add(180, My.Resources.Item180)
        dpItemImages.Add(181, My.Resources.Item181)
        dpItemImages.Add(182, My.Resources.Item182)
        dpItemImages.Add(183, My.Resources.Item183)
        dpItemImages.Add(184, My.Resources.Item184)
        dpItemImages.Add(185, My.Resources.Item185)
        dpItemImages.Add(186, My.Resources.Item186)
        dpItemImages.Add(187, My.Resources.Item187)
        dpItemImages.Add(188, My.Resources.Item188)
        dpItemImages.Add(189, My.Resources.Item189)
        dpItemImages.Add(190, My.Resources.Item190)
        dpItemImages.Add(191, My.Resources.Item191)
        dpItemImages.Add(192, My.Resources.Item192)
        dpItemImages.Add(193, My.Resources.Item193)
        dpItemImages.Add(194, My.Resources.Item194)
        dpItemImages.Add(195, My.Resources.Item195)
        dpItemImages.Add(196, My.Resources.Item196)
        dpItemImages.Add(197, My.Resources.Item197)
        dpItemImages.Add(198, My.Resources.Item198)
        dpItemImages.Add(199, My.Resources.Item199)
        dpItemImages.Add(200, My.Resources.Item200)
        dpItemImages.Add(201, My.Resources.Item201)
        dpItemImages.Add(202, My.Resources.Item202)
        dpItemImages.Add(203, My.Resources.Item203)
        dpItemImages.Add(204, My.Resources.Item204)
        dpItemImages.Add(205, My.Resources.Item205)
        dpItemImages.Add(206, My.Resources.Item206)
        dpItemImages.Add(207, My.Resources.Item207)
        dpItemImages.Add(208, My.Resources.Item208)
        dpItemImages.Add(209, My.Resources.Item209)
        dpItemImages.Add(210, My.Resources.Item210)
        dpItemImages.Add(211, My.Resources.Item211)
        dpItemImages.Add(212, My.Resources.Item212)
        dpItemImages.Add(213, My.Resources.Item213)
        dpItemImages.Add(214, My.Resources.Item214)
        dpItemImages.Add(215, My.Resources.Item215)
        dpItemImages.Add(216, My.Resources.Item216)
        dpItemImages.Add(217, My.Resources.Item217)
        dpItemImages.Add(218, My.Resources.Item218)
        dpItemImages.Add(219, My.Resources.Item219)
        dpItemImages.Add(220, My.Resources.Item220)
        dpItemImages.Add(221, My.Resources.Item221)
        dpItemImages.Add(222, My.Resources.Item222)
        dpItemImages.Add(223, My.Resources.Item223)
        dpItemImages.Add(224, My.Resources.Item224)
        dpItemImages.Add(225, My.Resources.Item225)
        dpItemImages.Add(226, My.Resources.Item226)
        dpItemImages.Add(227, My.Resources.Item227)
        dpItemImages.Add(228, My.Resources.Item228)
        dpItemImages.Add(229, My.Resources.Item229)
        dpItemImages.Add(230, My.Resources.Item230)
        dpItemImages.Add(231, My.Resources.Item231)
        dpItemImages.Add(232, My.Resources.Item232)
        dpItemImages.Add(233, My.Resources.Item233)
        dpItemImages.Add(234, My.Resources.Item234)
        dpItemImages.Add(235, My.Resources.Item235)
        dpItemImages.Add(236, My.Resources.Item236)
        dpItemImages.Add(237, My.Resources.Item237)
        dpItemImages.Add(238, My.Resources.Item238)
        dpItemImages.Add(239, My.Resources.Item239)
        dpItemImages.Add(240, My.Resources.Item240)
        dpItemImages.Add(241, My.Resources.Item241)
        dpItemImages.Add(242, My.Resources.Item242)
        dpItemImages.Add(243, My.Resources.Item243)
        dpItemImages.Add(244, My.Resources.Item244)
        dpItemImages.Add(245, My.Resources.Item245)
        dpItemImages.Add(246, My.Resources.Item246)
        dpItemImages.Add(247, My.Resources.Item247)
        dpItemImages.Add(248, My.Resources.Item248)
        dpItemImages.Add(249, My.Resources.Item249)
        dpItemImages.Add(250, My.Resources.Item250)
        dpItemImages.Add(251, My.Resources.Item251)
        dpItemImages.Add(252, My.Resources.Item252)
        dpItemImages.Add(253, My.Resources.Item253)
        dpItemImages.Add(254, My.Resources.Item254)
        dpItemImages.Add(255, My.Resources.Item255)
        dpItemImages.Add(256, My.Resources.Item256)
        dpItemImages.Add(257, My.Resources.Item257)
        dpItemImages.Add(258, My.Resources.Item258)
        dpItemImages.Add(259, My.Resources.Item259)
        dpItemImages.Add(260, My.Resources.Item260)
        dpItemImages.Add(261, My.Resources.Item261)
        dpItemImages.Add(262, My.Resources.Item262)
        dpItemImages.Add(263, My.Resources.Item263)
        dpItemImages.Add(264, My.Resources.Item264)
        dpItemImages.Add(265, My.Resources.Item265)
        dpItemImages.Add(266, My.Resources.Item266)
        dpItemImages.Add(267, My.Resources.Item267)
        dpItemImages.Add(268, My.Resources.Item268)
        dpItemImages.Add(269, My.Resources.Item269)
        dpItemImages.Add(270, My.Resources.Item270)
        dpItemImages.Add(271, My.Resources.Item271)
        dpItemImages.Add(272, My.Resources.Item272)
        dpItemImages.Add(273, My.Resources.Item273)
        dpItemImages.Add(274, My.Resources.Item274)
        dpItemImages.Add(275, My.Resources.Item275)
        dpItemImages.Add(276, My.Resources.Item276)
        dpItemImages.Add(277, My.Resources.Item277)
        dpItemImages.Add(278, My.Resources.Item278)
        dpItemImages.Add(279, My.Resources.Item279)
        dpItemImages.Add(280, My.Resources.Item280)
        dpItemImages.Add(281, My.Resources.Item281)
        dpItemImages.Add(282, My.Resources.Item282)
        dpItemImages.Add(283, My.Resources.Item283)
        dpItemImages.Add(284, My.Resources.Item284)
        dpItemImages.Add(285, My.Resources.Item285)
        dpItemImages.Add(286, My.Resources.Item286)
        dpItemImages.Add(287, My.Resources.Item287)
        dpItemImages.Add(288, My.Resources.Item288)
        dpItemImages.Add(289, My.Resources.Item289)
        dpItemImages.Add(290, My.Resources.Item290)
        dpItemImages.Add(291, My.Resources.Item291)
        dpItemImages.Add(292, My.Resources.Item292)
        dpItemImages.Add(293, My.Resources.Item293)
        dpItemImages.Add(294, My.Resources.Item294)
        dpItemImages.Add(295, My.Resources.Item295)
        dpItemImages.Add(296, My.Resources.Item296)
        dpItemImages.Add(297, My.Resources.Item297)
        dpItemImages.Add(298, My.Resources.Item298)
        dpItemImages.Add(299, My.Resources.Item299)
        dpItemImages.Add(300, My.Resources.Item300)
        dpItemImages.Add(301, My.Resources.Item301)
        dpItemImages.Add(302, My.Resources.Item302)
        dpItemImages.Add(303, My.Resources.Item303)
        dpItemImages.Add(304, My.Resources.Item304)
        dpItemImages.Add(305, My.Resources.Item305)
        dpItemImages.Add(306, My.Resources.Item306)
        dpItemImages.Add(307, My.Resources.Item307)
        dpItemImages.Add(308, My.Resources.Item308)
        dpItemImages.Add(309, My.Resources.Item309)
        dpItemImages.Add(310, My.Resources.Item310)
        dpItemImages.Add(311, My.Resources.Item311)
        dpItemImages.Add(312, My.Resources.Item312)
        dpItemImages.Add(313, My.Resources.Item313)
        dpItemImages.Add(314, My.Resources.Item314)
        dpItemImages.Add(315, My.Resources.Item315)
        dpItemImages.Add(316, My.Resources.Item316)
        dpItemImages.Add(317, My.Resources.Item317)
        dpItemImages.Add(318, My.Resources.Item318)
        dpItemImages.Add(319, My.Resources.Item319)
        dpItemImages.Add(320, My.Resources.Item320)
        dpItemImages.Add(321, My.Resources.Item321)
        dpItemImages.Add(322, My.Resources.Item322)
        dpItemImages.Add(323, My.Resources.Item323)
        dpItemImages.Add(324, My.Resources.Item324)
        dpItemImages.Add(325, My.Resources.Item325)
        dpItemImages.Add(326, My.Resources.Item326)
        dpItemImages.Add(327, My.Resources.Item327)
        dpItemImages.Add(328, My.Resources.Item328)
        dpItemImages.Add(329, My.Resources.Item329)
        dpItemImages.Add(330, My.Resources.Item330)
        dpItemImages.Add(331, My.Resources.Item331)
        dpItemImages.Add(332, My.Resources.Item332)
        dpItemImages.Add(333, My.Resources.Item333)
        dpItemImages.Add(334, My.Resources.Item334)
        dpItemImages.Add(335, My.Resources.Item335)
        dpItemImages.Add(336, My.Resources.Item336)
        dpItemImages.Add(337, My.Resources.Item337)
        dpItemImages.Add(338, My.Resources.Item338)
        dpItemImages.Add(339, My.Resources.Item339)
        dpItemImages.Add(340, My.Resources.Item340)
        dpItemImages.Add(341, My.Resources.Item341)
        dpItemImages.Add(342, My.Resources.Item342)
        dpItemImages.Add(343, My.Resources.Item343)
        dpItemImages.Add(344, My.Resources.Item344)
        dpItemImages.Add(345, My.Resources.Item345)
        dpItemImages.Add(346, My.Resources.Item346)
        dpItemImages.Add(347, My.Resources.Item347)
        dpItemImages.Add(348, My.Resources.Item348)
        dpItemImages.Add(349, My.Resources.Item349)
        dpItemImages.Add(350, My.Resources.Item350)
        dpItemImages.Add(351, My.Resources.Item351)
        dpItemImages.Add(352, My.Resources.Item352)
        dpItemImages.Add(353, My.Resources.Item353)
        dpItemImages.Add(354, My.Resources.Item354)
        dpItemImages.Add(355, My.Resources.Item355)
        dpItemImages.Add(356, My.Resources.Item356)
        dpItemImages.Add(357, My.Resources.Item357)
        dpItemImages.Add(358, My.Resources.Item358)
        dpItemImages.Add(359, My.Resources.Item359)
        dpItemImages.Add(360, My.Resources.Item360)
        dpItemImages.Add(361, My.Resources.Item361)
        dpItemImages.Add(362, My.Resources.Item362)
        dpItemImages.Add(363, My.Resources.Item363)
        dpItemImages.Add(364, My.Resources.Item364)
        dpItemImages.Add(365, My.Resources.Item365)
        dpItemImages.Add(366, My.Resources.Item366)
        dpItemImages.Add(367, My.Resources.Item367)
        dpItemImages.Add(368, My.Resources.Item368)
        dpItemImages.Add(369, My.Resources.Item369)
        dpItemImages.Add(370, My.Resources.Item370)
        dpItemImages.Add(371, My.Resources.Item371)
        dpItemImages.Add(372, My.Resources.Item372)
        dpItemImages.Add(373, My.Resources.Item373)
        dpItemImages.Add(374, My.Resources.Item374)
        dpItemImages.Add(375, My.Resources.Item375)
        dpItemImages.Add(376, My.Resources.Item376)
        dpItemImages.Add(377, My.Resources.Item377)
        dpItemImages.Add(378, My.Resources.Item378)
        dpItemImages.Add(379, My.Resources.Item379)
        dpItemImages.Add(380, My.Resources.Item380)
        dpItemImages.Add(381, My.Resources.Item381)
        dpItemImages.Add(382, My.Resources.Item382)
        dpItemImages.Add(383, My.Resources.Item383)
        dpItemImages.Add(384, My.Resources.Item384)
        dpItemImages.Add(385, My.Resources.Item385)
        dpItemImages.Add(386, My.Resources.Item386)
        dpItemImages.Add(387, My.Resources.Item387)
        dpItemImages.Add(388, My.Resources.Item388)
        dpItemImages.Add(389, My.Resources.Item389)
        dpItemImages.Add(390, My.Resources.Item390)
        dpItemImages.Add(391, My.Resources.Item391)
        dpItemImages.Add(392, My.Resources.Item392)
        dpItemImages.Add(393, My.Resources.Item393)
        dpItemImages.Add(394, My.Resources.Item394)
        dpItemImages.Add(395, My.Resources.Item395)
        dpItemImages.Add(396, My.Resources.Item396)
        dpItemImages.Add(397, My.Resources.Item397)
        dpItemImages.Add(398, My.Resources.Item398)
        dpItemImages.Add(399, My.Resources.Item399)
        dpItemImages.Add(400, My.Resources.Item400)
        dpItemImages.Add(401, My.Resources.Item401)
        dpItemImages.Add(402, My.Resources.Item402)
        dpItemImages.Add(403, My.Resources.Item403)
        dpItemImages.Add(404, My.Resources.Item404)
        dpItemImages.Add(405, My.Resources.Item405)
        dpItemImages.Add(406, My.Resources.Item406)
        dpItemImages.Add(407, My.Resources.Item407)
        dpItemImages.Add(408, My.Resources.Item408)
        dpItemImages.Add(409, My.Resources.Item409)
        dpItemImages.Add(410, My.Resources.Item410)
        dpItemImages.Add(411, My.Resources.Item411)
        dpItemImages.Add(412, My.Resources.Item412)
        dpItemImages.Add(413, My.Resources.Item413)
        dpItemImages.Add(414, My.Resources.Item414)
        dpItemImages.Add(415, My.Resources.Item415)
        dpItemImages.Add(416, My.Resources.Item416)
        dpItemImages.Add(417, My.Resources.Item417)
        dpItemImages.Add(418, My.Resources.Item418)
        dpItemImages.Add(419, My.Resources.Item419)
        dpItemImages.Add(420, My.Resources.Item420)
        dpItemImages.Add(421, My.Resources.Item421)
        dpItemImages.Add(422, My.Resources.Item422)
        dpItemImages.Add(423, My.Resources.Item423)
        dpItemImages.Add(424, My.Resources.Item424)
        dpItemImages.Add(425, My.Resources.Item425)
        dpItemImages.Add(426, My.Resources.Item426)
        dpItemImages.Add(427, My.Resources.Item427)
        dpItemImages.Add(428, My.Resources.Item428)
        dpItemImages.Add(429, My.Resources.Item429)
        dpItemImages.Add(430, My.Resources.Item430)
        dpItemImages.Add(431, My.Resources.Item431)
        dpItemImages.Add(432, My.Resources.Item432)
        dpItemImages.Add(433, My.Resources.Item433)
        dpItemImages.Add(434, My.Resources.Item434)
        dpItemImages.Add(435, My.Resources.Item435)
        dpItemImages.Add(436, My.Resources.Item436)
        dpItemImages.Add(437, My.Resources.Item437)
        dpItemImages.Add(438, My.Resources.Item438)
        dpItemImages.Add(439, My.Resources.Item439)
        dpItemImages.Add(440, My.Resources.Item440)
        dpItemImages.Add(441, My.Resources.Item441)
        dpItemImages.Add(442, My.Resources.Item442)
        dpItemImages.Add(443, My.Resources.Item443)
        dpItemImages.Add(444, My.Resources.Item444)
        dpItemImages.Add(445, My.Resources.Item445)
        dpItemImages.Add(446, My.Resources.Item446)
        dpItemImages.Add(447, My.Resources.Item447)
        dpItemImages.Add(448, My.Resources.Item448)
        dpItemImages.Add(449, My.Resources.Item449)
        dpItemImages.Add(450, My.Resources.Item450)
        dpItemImages.Add(451, My.Resources.Item451)
        dpItemImages.Add(452, My.Resources.Item452)
        dpItemImages.Add(453, My.Resources.Item453)
        dpItemImages.Add(454, My.Resources.Item454)
        dpItemImages.Add(455, My.Resources.Item455)
        dpItemImages.Add(456, My.Resources.Item456)
        dpItemImages.Add(457, My.Resources.Item457)
        dpItemImages.Add(458, My.Resources.Item458)
        dpItemImages.Add(459, My.Resources.Item459)
        dpItemImages.Add(460, My.Resources.Item460)
        dpItemImages.Add(461, My.Resources.Item461)
        dpItemImages.Add(462, My.Resources.Item462)
        dpItemImages.Add(463, My.Resources.Item463)
        dpItemImages.Add(464, My.Resources.Item464)
        dpItemImages.Add(465, My.Resources.Item465)
        dpItemImages.Add(466, My.Resources.Item466)
        dpItemImages.Add(467, My.Resources.Item467)

        dpPoketchApps.Add(&H0, "Digital Watch")
        dpPoketchApps.Add(&H1, "Calculator")
        dpPoketchApps.Add(&H2, "Memo Pad")
        dpPoketchApps.Add(&H3, "Pedometer")
        dpPoketchApps.Add(&H4, "Pokémon List")
        dpPoketchApps.Add(&H5, "Friendship Checker")
        dpPoketchApps.Add(&H6, "Dowsing Machine")
        dpPoketchApps.Add(&H7, "Berry Searcher")
        dpPoketchApps.Add(&H8, "Day-Care Checker")
        dpPoketchApps.Add(&H9, "Pokémon History")
        dpPoketchApps.Add(&HA, "Counter")
        dpPoketchApps.Add(&HB, "Analog Watch")
        dpPoketchApps.Add(&HC, "Marking Map")
        dpPoketchApps.Add(&HD, "Link Searcher")
        dpPoketchApps.Add(&HE, "Coin Toss")
        dpPoketchApps.Add(&HF, "Move Tester")
        dpPoketchApps.Add(&H10, "Calendar")
        dpPoketchApps.Add(&H11, "Dot Artist")
        dpPoketchApps.Add(&H12, "Roulette")
        dpPoketchApps.Add(&H13, "Trainer Counter")
        dpPoketchApps.Add(&H14, "Kitchen Timer")
        dpPoketchApps.Add(&H15, "Color Changer")
        dpPoketchApps.Add(&H16, "Matchup Checker")
        dpPoketchApps.Add(&H17, "Stopwatch")
        dpPoketchApps.Add(&H18, "Alarm Clock")

        GBAShuffleOrder.Add(0, "GAEM")
        GBAShuffleOrder.Add(1, "GAME")
        GBAShuffleOrder.Add(2, "GEAM")
        GBAShuffleOrder.Add(3, "GEMA")
        GBAShuffleOrder.Add(4, "GMAE")
        GBAShuffleOrder.Add(5, "GMEA")
        GBAShuffleOrder.Add(6, "AGEM")
        GBAShuffleOrder.Add(7, "AGME")
        GBAShuffleOrder.Add(8, "AEGM")
        GBAShuffleOrder.Add(9, "AEMG")
        GBAShuffleOrder.Add(10, "AMGE")
        GBAShuffleOrder.Add(11, "AMEG")
        GBAShuffleOrder.Add(12, "EGAM")
        GBAShuffleOrder.Add(13, "EGMA")
        GBAShuffleOrder.Add(14, "EAGM")
        GBAShuffleOrder.Add(15, "EAMG")
        GBAShuffleOrder.Add(16, "EMGA")
        GBAShuffleOrder.Add(17, "EMAG")
        GBAShuffleOrder.Add(18, "MGAE")
        GBAShuffleOrder.Add(19, "MGEA")
        GBAShuffleOrder.Add(20, "MAGE")
        GBAShuffleOrder.Add(21, "MAEG")
        GBAShuffleOrder.Add(22, "MEGA")
        GBAShuffleOrder.Add(23, "MEAG")

        GBASpecies.Add(0, "900")
        GBASpecies.Add(1, "001")
        GBASpecies.Add(2, "002")
        GBASpecies.Add(3, "003")
        GBASpecies.Add(4, "004")
        GBASpecies.Add(5, "005")
        GBASpecies.Add(6, "006")
        GBASpecies.Add(7, "007")
        GBASpecies.Add(8, "008")
        GBASpecies.Add(9, "009")
        GBASpecies.Add(10, "010")
        GBASpecies.Add(11, "011")
        GBASpecies.Add(12, "012")
        GBASpecies.Add(13, "013")
        GBASpecies.Add(14, "014")
        GBASpecies.Add(15, "015")
        GBASpecies.Add(16, "016")
        GBASpecies.Add(17, "017")
        GBASpecies.Add(18, "018")
        GBASpecies.Add(19, "019")
        GBASpecies.Add(20, "020")
        GBASpecies.Add(21, "021")
        GBASpecies.Add(22, "022")
        GBASpecies.Add(23, "023")
        GBASpecies.Add(24, "024")
        GBASpecies.Add(25, "025")
        GBASpecies.Add(26, "026")
        GBASpecies.Add(27, "027")
        GBASpecies.Add(28, "028")
        GBASpecies.Add(29, "029")
        GBASpecies.Add(30, "030")
        GBASpecies.Add(31, "031")
        GBASpecies.Add(32, "032")
        GBASpecies.Add(33, "033")
        GBASpecies.Add(34, "034")
        GBASpecies.Add(35, "035")
        GBASpecies.Add(36, "036")
        GBASpecies.Add(37, "037")
        GBASpecies.Add(38, "038")
        GBASpecies.Add(39, "039")
        GBASpecies.Add(40, "040")
        GBASpecies.Add(41, "041")
        GBASpecies.Add(42, "042")
        GBASpecies.Add(43, "043")
        GBASpecies.Add(44, "044")
        GBASpecies.Add(45, "045")
        GBASpecies.Add(46, "046")
        GBASpecies.Add(47, "047")
        GBASpecies.Add(48, "048")
        GBASpecies.Add(49, "049")
        GBASpecies.Add(50, "050")
        GBASpecies.Add(51, "051")
        GBASpecies.Add(52, "052")
        GBASpecies.Add(53, "053")
        GBASpecies.Add(54, "054")
        GBASpecies.Add(55, "055")
        GBASpecies.Add(56, "056")
        GBASpecies.Add(57, "057")
        GBASpecies.Add(58, "058")
        GBASpecies.Add(59, "059")
        GBASpecies.Add(60, "060")
        GBASpecies.Add(61, "061")
        GBASpecies.Add(62, "062")
        GBASpecies.Add(63, "063")
        GBASpecies.Add(64, "064")
        GBASpecies.Add(65, "065")
        GBASpecies.Add(66, "066")
        GBASpecies.Add(67, "067")
        GBASpecies.Add(68, "068")
        GBASpecies.Add(69, "069")
        GBASpecies.Add(70, "070")
        GBASpecies.Add(71, "071")
        GBASpecies.Add(72, "072")
        GBASpecies.Add(73, "073")
        GBASpecies.Add(74, "074")
        GBASpecies.Add(75, "075")
        GBASpecies.Add(76, "076")
        GBASpecies.Add(77, "077")
        GBASpecies.Add(78, "077")
        GBASpecies.Add(79, "079")
        GBASpecies.Add(80, "080")
        GBASpecies.Add(81, "081")
        GBASpecies.Add(82, "082")
        GBASpecies.Add(83, "083")
        GBASpecies.Add(84, "084")
        GBASpecies.Add(85, "085")
        GBASpecies.Add(86, "086")
        GBASpecies.Add(87, "087")
        GBASpecies.Add(88, "088")
        GBASpecies.Add(89, "089")
        GBASpecies.Add(90, "090")
        GBASpecies.Add(91, "091")
        GBASpecies.Add(92, "092")
        GBASpecies.Add(93, "093")
        GBASpecies.Add(94, "094")
        GBASpecies.Add(95, "095")
        GBASpecies.Add(96, "096")
        GBASpecies.Add(97, "097")
        GBASpecies.Add(98, "098")
        GBASpecies.Add(99, "099")
        GBASpecies.Add(100, "100")
        GBASpecies.Add(101, "101")
        GBASpecies.Add(102, "102")
        GBASpecies.Add(103, "103")
        GBASpecies.Add(104, "104")
        GBASpecies.Add(105, "105")
        GBASpecies.Add(106, "106")
        GBASpecies.Add(107, "107")
        GBASpecies.Add(108, "108")
        GBASpecies.Add(109, "109")
        GBASpecies.Add(110, "110")
        GBASpecies.Add(111, "111")
        GBASpecies.Add(112, "112")
        GBASpecies.Add(113, "113")
        GBASpecies.Add(114, "114")
        GBASpecies.Add(115, "115")
        GBASpecies.Add(116, "116")
        GBASpecies.Add(117, "117")
        GBASpecies.Add(118, "118")
        GBASpecies.Add(119, "119")
        GBASpecies.Add(120, "120")
        GBASpecies.Add(121, "121")
        GBASpecies.Add(122, "122")
        GBASpecies.Add(123, "123")
        GBASpecies.Add(124, "124")
        GBASpecies.Add(125, "125")
        GBASpecies.Add(126, "126")
        GBASpecies.Add(127, "127")
        GBASpecies.Add(128, "128")
        GBASpecies.Add(129, "129")
        GBASpecies.Add(130, "130")
        GBASpecies.Add(131, "131")
        GBASpecies.Add(132, "132")
        GBASpecies.Add(133, "133")
        GBASpecies.Add(134, "134")
        GBASpecies.Add(135, "135")
        GBASpecies.Add(136, "136")
        GBASpecies.Add(137, "137")
        GBASpecies.Add(138, "138")
        GBASpecies.Add(139, "139")
        GBASpecies.Add(140, "140")
        GBASpecies.Add(141, "141")
        GBASpecies.Add(142, "142")
        GBASpecies.Add(143, "143")
        GBASpecies.Add(144, "144")
        GBASpecies.Add(145, "145")
        GBASpecies.Add(146, "146")
        GBASpecies.Add(147, "147")
        GBASpecies.Add(148, "148")
        GBASpecies.Add(149, "149")
        GBASpecies.Add(150, "150")
        GBASpecies.Add(151, "151")
        GBASpecies.Add(152, "152")
        GBASpecies.Add(153, "153")
        GBASpecies.Add(154, "154")
        GBASpecies.Add(155, "155")
        GBASpecies.Add(156, "156")
        GBASpecies.Add(157, "157")
        GBASpecies.Add(158, "158")
        GBASpecies.Add(159, "159")
        GBASpecies.Add(160, "160")
        GBASpecies.Add(161, "161")
        GBASpecies.Add(162, "162")
        GBASpecies.Add(163, "163")
        GBASpecies.Add(164, "164")
        GBASpecies.Add(165, "165")
        GBASpecies.Add(166, "166")
        GBASpecies.Add(167, "167")
        GBASpecies.Add(168, "168")
        GBASpecies.Add(169, "169")
        GBASpecies.Add(170, "170")
        GBASpecies.Add(171, "171")
        GBASpecies.Add(172, "172")
        GBASpecies.Add(173, "173")
        GBASpecies.Add(174, "174")
        GBASpecies.Add(175, "175")
        GBASpecies.Add(176, "176")
        GBASpecies.Add(177, "177")
        GBASpecies.Add(178, "178")
        GBASpecies.Add(179, "179")
        GBASpecies.Add(180, "180")
        GBASpecies.Add(181, "181")
        GBASpecies.Add(182, "182")
        GBASpecies.Add(183, "183")
        GBASpecies.Add(184, "184")
        GBASpecies.Add(185, "185")
        GBASpecies.Add(186, "186")
        GBASpecies.Add(187, "187")
        GBASpecies.Add(188, "188")
        GBASpecies.Add(189, "189")
        GBASpecies.Add(190, "190")
        GBASpecies.Add(191, "191")
        GBASpecies.Add(192, "192")
        GBASpecies.Add(193, "193")
        GBASpecies.Add(194, "194")
        GBASpecies.Add(195, "195")
        GBASpecies.Add(196, "196")
        GBASpecies.Add(197, "197")
        GBASpecies.Add(198, "198")
        GBASpecies.Add(199, "199")
        GBASpecies.Add(200, "200")
        GBASpecies.Add(201, "201")
        GBASpecies.Add(202, "202")
        GBASpecies.Add(203, "203")
        GBASpecies.Add(204, "204")
        GBASpecies.Add(205, "205")
        GBASpecies.Add(206, "206")
        GBASpecies.Add(207, "207")
        GBASpecies.Add(208, "208")
        GBASpecies.Add(209, "209")
        GBASpecies.Add(210, "210")
        GBASpecies.Add(211, "211")
        GBASpecies.Add(212, "212")
        GBASpecies.Add(213, "213")
        GBASpecies.Add(214, "214")
        GBASpecies.Add(215, "215")
        GBASpecies.Add(216, "216")
        GBASpecies.Add(217, "217")
        GBASpecies.Add(218, "218")
        GBASpecies.Add(219, "219")
        GBASpecies.Add(220, "220")
        GBASpecies.Add(221, "221")
        GBASpecies.Add(222, "222")
        GBASpecies.Add(223, "223")
        GBASpecies.Add(224, "224")
        GBASpecies.Add(225, "225")
        GBASpecies.Add(226, "226")
        GBASpecies.Add(227, "227")
        GBASpecies.Add(228, "228")
        GBASpecies.Add(229, "229")
        GBASpecies.Add(230, "230")
        GBASpecies.Add(231, "231")
        GBASpecies.Add(232, "232")
        GBASpecies.Add(233, "233")
        GBASpecies.Add(234, "234")
        GBASpecies.Add(235, "235")
        GBASpecies.Add(236, "236")
        GBASpecies.Add(237, "237")
        GBASpecies.Add(238, "238")
        GBASpecies.Add(239, "239")
        GBASpecies.Add(240, "240")
        GBASpecies.Add(241, "241")
        GBASpecies.Add(242, "242")
        GBASpecies.Add(243, "243")
        GBASpecies.Add(244, "244")
        GBASpecies.Add(245, "245")
        GBASpecies.Add(246, "246")
        GBASpecies.Add(247, "247")
        GBASpecies.Add(248, "248")
        GBASpecies.Add(249, "249")
        GBASpecies.Add(250, "250")
        GBASpecies.Add(251, "251")
        GBASpecies.Add(252, "000")
        GBASpecies.Add(253, "000")
        GBASpecies.Add(254, "000")
        GBASpecies.Add(255, "000")
        GBASpecies.Add(256, "000")
        GBASpecies.Add(257, "000")
        GBASpecies.Add(258, "000")
        GBASpecies.Add(259, "000")
        GBASpecies.Add(260, "000")
        GBASpecies.Add(261, "000")
        GBASpecies.Add(262, "000")
        GBASpecies.Add(263, "000")
        GBASpecies.Add(264, "000")
        GBASpecies.Add(265, "000")
        GBASpecies.Add(266, "000")
        GBASpecies.Add(267, "000")
        GBASpecies.Add(268, "000")
        GBASpecies.Add(269, "000")
        GBASpecies.Add(270, "000")
        GBASpecies.Add(271, "000")
        GBASpecies.Add(272, "000")
        GBASpecies.Add(273, "000")
        GBASpecies.Add(274, "000")
        GBASpecies.Add(275, "000")
        GBASpecies.Add(276, "000")
        GBASpecies.Add(277, "252")
        GBASpecies.Add(278, "253")
        GBASpecies.Add(279, "254")
        GBASpecies.Add(280, "255")
        GBASpecies.Add(281, "256")
        GBASpecies.Add(282, "257")
        GBASpecies.Add(283, "258")
        GBASpecies.Add(284, "259")
        GBASpecies.Add(285, "260")
        GBASpecies.Add(286, "261")
        GBASpecies.Add(287, "262")
        GBASpecies.Add(288, "263")
        GBASpecies.Add(289, "264")
        GBASpecies.Add(290, "265")
        GBASpecies.Add(291, "266")
        GBASpecies.Add(292, "267")
        GBASpecies.Add(293, "268")
        GBASpecies.Add(294, "269")
        GBASpecies.Add(295, "270")
        GBASpecies.Add(296, "271")
        GBASpecies.Add(297, "272")
        GBASpecies.Add(298, "273")
        GBASpecies.Add(299, "274")
        GBASpecies.Add(300, "275")
        GBASpecies.Add(301, "290")
        GBASpecies.Add(302, "291")
        GBASpecies.Add(303, "292")
        GBASpecies.Add(304, "276")
        GBASpecies.Add(305, "277")
        GBASpecies.Add(306, "285")
        GBASpecies.Add(307, "286")
        GBASpecies.Add(308, "327")
        GBASpecies.Add(309, "278")
        GBASpecies.Add(310, "279")
        GBASpecies.Add(311, "283")
        GBASpecies.Add(312, "284")
        GBASpecies.Add(313, "320")
        GBASpecies.Add(314, "321")
        GBASpecies.Add(315, "300")
        GBASpecies.Add(316, "301")
        GBASpecies.Add(317, "352")
        GBASpecies.Add(318, "343")
        GBASpecies.Add(319, "344")
        GBASpecies.Add(320, "299")
        GBASpecies.Add(321, "324")
        GBASpecies.Add(322, "302")
        GBASpecies.Add(323, "339")
        GBASpecies.Add(324, "340")
        GBASpecies.Add(325, "370")
        GBASpecies.Add(326, "341")
        GBASpecies.Add(327, "342")
        GBASpecies.Add(328, "349")
        GBASpecies.Add(329, "350")
        GBASpecies.Add(330, "318")
        GBASpecies.Add(331, "319")
        GBASpecies.Add(332, "328")
        GBASpecies.Add(333, "329")
        GBASpecies.Add(334, "330")
        GBASpecies.Add(335, "296")
        GBASpecies.Add(336, "297")
        GBASpecies.Add(337, "309")
        GBASpecies.Add(338, "310")
        GBASpecies.Add(339, "322")
        GBASpecies.Add(340, "323")
        GBASpecies.Add(341, "363")
        GBASpecies.Add(342, "364")
        GBASpecies.Add(343, "365")
        GBASpecies.Add(344, "331")
        GBASpecies.Add(345, "332")
        GBASpecies.Add(346, "361")
        GBASpecies.Add(347, "362")
        GBASpecies.Add(348, "337")
        GBASpecies.Add(349, "338")
        GBASpecies.Add(350, "298")
        GBASpecies.Add(351, "325")
        GBASpecies.Add(352, "326")
        GBASpecies.Add(353, "311")
        GBASpecies.Add(354, "312")
        GBASpecies.Add(355, "303")
        GBASpecies.Add(356, "307")
        GBASpecies.Add(357, "308")
        GBASpecies.Add(358, "333")
        GBASpecies.Add(359, "334")
        GBASpecies.Add(360, "360")
        GBASpecies.Add(361, "355")
        GBASpecies.Add(362, "356")
        GBASpecies.Add(363, "315")
        GBASpecies.Add(364, "287")
        GBASpecies.Add(365, "288")
        GBASpecies.Add(366, "289")
        GBASpecies.Add(367, "316")
        GBASpecies.Add(368, "317")
        GBASpecies.Add(369, "357")
        GBASpecies.Add(370, "293")
        GBASpecies.Add(371, "294")
        GBASpecies.Add(372, "295")
        GBASpecies.Add(373, "366")
        GBASpecies.Add(374, "367")
        GBASpecies.Add(375, "368")
        GBASpecies.Add(376, "359")
        GBASpecies.Add(377, "353")
        GBASpecies.Add(378, "354")
        GBASpecies.Add(379, "336")
        GBASpecies.Add(380, "335")
        GBASpecies.Add(381, "369")
        GBASpecies.Add(382, "304")
        GBASpecies.Add(383, "305")
        GBASpecies.Add(384, "306")
        GBASpecies.Add(385, "351")
        GBASpecies.Add(386, "313")
        GBASpecies.Add(387, "314")
        GBASpecies.Add(388, "345")
        GBASpecies.Add(389, "346")
        GBASpecies.Add(390, "347")
        GBASpecies.Add(391, "348")
        GBASpecies.Add(392, "280")
        GBASpecies.Add(393, "281")
        GBASpecies.Add(394, "282")
        GBASpecies.Add(395, "371")
        GBASpecies.Add(396, "372")
        GBASpecies.Add(397, "373")
        GBASpecies.Add(398, "374")
        GBASpecies.Add(399, "375")
        GBASpecies.Add(400, "376")
        GBASpecies.Add(401, "377")
        GBASpecies.Add(402, "378")
        GBASpecies.Add(403, "379")
        GBASpecies.Add(404, "382")
        GBASpecies.Add(405, "383")
        GBASpecies.Add(406, "384")
        GBASpecies.Add(407, "380")
        GBASpecies.Add(408, "381")
        GBASpecies.Add(409, "385")
        GBASpecies.Add(410, "386")
        GBASpecies.Add(411, "358")
        GBASpecies.Add(412, "Egg")
        GBASpecies.Add(413, "201B")
        GBASpecies.Add(414, "201C")
        GBASpecies.Add(415, "201D")
        GBASpecies.Add(416, "201E")
        GBASpecies.Add(417, "201F")
        GBASpecies.Add(418, "201G")
        GBASpecies.Add(419, "201H")
        GBASpecies.Add(420, "201I")
        GBASpecies.Add(421, "201J")
        GBASpecies.Add(422, "201K")
        GBASpecies.Add(423, "201L")
        GBASpecies.Add(424, "201M")
        GBASpecies.Add(425, "201N")
        GBASpecies.Add(426, "201O")
        GBASpecies.Add(427, "201P")
        GBASpecies.Add(428, "201Q")
        GBASpecies.Add(429, "201R")
        GBASpecies.Add(430, "201S")
        GBASpecies.Add(431, "201T")
        GBASpecies.Add(432, "201U")
        GBASpecies.Add(433, "201V")
        GBASpecies.Add(434, "201W")
        GBASpecies.Add(435, "201X")
        GBASpecies.Add(436, "201Y")
        GBASpecies.Add(437, "201Z")
        GBASpecies.Add(438, "201!")
        GBASpecies.Add(439, "201?")

        GBAFont.Add(&H0, " ")
        GBAFont.Add(&H1, "HERO")
        GBAFont.Add(&H1B, "?")
        GBAFont.Add(&H2D, "&")
        GBAFont.Add(&H5C, "(")
        GBAFont.Add(&H5D, ")")
        GBAFont.Add(&H79, "-UP")
        GBAFont.Add(&H7A, "-DOWN")
        GBAFont.Add(&H7B, "-LEFT")
        GBAFont.Add(&H7C, "-RIGHT")
        GBAFont.Add(&HA1, "0")
        GBAFont.Add(&HA2, "1")
        GBAFont.Add(&HA3, "2")
        GBAFont.Add(&HA4, "3")
        GBAFont.Add(&HA5, "4")
        GBAFont.Add(&HA6, "5")
        GBAFont.Add(&HA7, "6")
        GBAFont.Add(&HA8, "7")
        GBAFont.Add(&HA9, "8")
        GBAFont.Add(&HAA, "9")
        GBAFont.Add(&HAB, "!")
        GBAFont.Add(&HAC, "?")
        GBAFont.Add(&HAD, ".")
        GBAFont.Add(&HAE, "-")
        GBAFont.Add(&HB0, "..")
        GBAFont.Add(&HB1, Chr(34))
        GBAFont.Add(&HB2, Chr(34) & "2")
        GBAFont.Add(&HB3, "'2")
        GBAFont.Add(&HB4, "'")
        GBAFont.Add(&HB5, "mA")
        GBAFont.Add(&HB6, "fE")
        GBAFont.Add(&HB7, "$")
        GBAFont.Add(&HB8, ",")
        GBAFont.Add(&HB9, "x-")
        GBAFont.Add(&HBA, "/")
        GBAFont.Add(&HBB, "A")
        GBAFont.Add(&HBC, "B")
        GBAFont.Add(&HBD, "C")
        GBAFont.Add(&HBE, "D")
        GBAFont.Add(&HBF, "E")
        GBAFont.Add(&HC0, "F")
        GBAFont.Add(&HC1, "G")
        GBAFont.Add(&HC2, "H")
        GBAFont.Add(&HC3, "I")
        GBAFont.Add(&HC4, "J")
        GBAFont.Add(&HC5, "K")
        GBAFont.Add(&HC6, "L")
        GBAFont.Add(&HC7, "M")
        GBAFont.Add(&HC8, "N")
        GBAFont.Add(&HC9, "O")
        GBAFont.Add(&HCA, "P")
        GBAFont.Add(&HCB, "Q")
        GBAFont.Add(&HCC, "R")
        GBAFont.Add(&HCD, "S")
        GBAFont.Add(&HCE, "T")
        GBAFont.Add(&HCF, "U")
        GBAFont.Add(&HD0, "V")
        GBAFont.Add(&HD1, "W")
        GBAFont.Add(&HD2, "X")
        GBAFont.Add(&HD3, "Y")
        GBAFont.Add(&HD4, "Z")
        GBAFont.Add(&HD5, "a")
        GBAFont.Add(&HD6, "b")
        GBAFont.Add(&HD7, "c")
        GBAFont.Add(&HD8, "d")
        GBAFont.Add(&HD9, "e")
        GBAFont.Add(&HDA, "f")
        GBAFont.Add(&HDB, "g")
        GBAFont.Add(&HDC, "h")
        GBAFont.Add(&HDD, "i")
        GBAFont.Add(&HDE, "j")
        GBAFont.Add(&HDF, "k")
        GBAFont.Add(&HE0, "l")
        GBAFont.Add(&HE1, "m")
        GBAFont.Add(&HE2, "n")
        GBAFont.Add(&HE3, "o")
        GBAFont.Add(&HE4, "p")
        GBAFont.Add(&HE5, "q")
        GBAFont.Add(&HE6, "r")
        GBAFont.Add(&HE7, "s")
        GBAFont.Add(&HE8, "t")
        GBAFont.Add(&HE9, "u")
        GBAFont.Add(&HEA, "v")
        GBAFont.Add(&HEB, "w")
        GBAFont.Add(&HEC, "x")
        GBAFont.Add(&HED, "y")
        GBAFont.Add(&HEE, "z")
        GBAFont.Add(&HF0, ":")
        GBAFont.Add(&HFA, "=")
        GBAFont.Add(&HFB, "*")
        GBAFont.Add(&HFC, "=2")
        GBAFont.Add(&HFD, "@")
        GBAFont.Add(&HFE, "+")
        GBAFont.Add(&HFF, "?")

        GBAItems.Add(0, "NOTHING")
        GBAItems.Add(1, "Master Ball")
        GBAItems.Add(2, "Ultra Ball")
        GBAItems.Add(3, "Great Ball")
        GBAItems.Add(4, "Poké Ball")
        GBAItems.Add(5, "Safari Ball")
        GBAItems.Add(6, "Net Ball")
        GBAItems.Add(7, "Dive Ball")
        GBAItems.Add(8, "Nest Ball")
        GBAItems.Add(9, "Repeat Ball")
        GBAItems.Add(10, "Timer Ball")
        GBAItems.Add(11, "Luxury Ball")
        GBAItems.Add(12, "Premier Ball")
        GBAItems.Add(13, "Potion")
        GBAItems.Add(14, "Antidote")
        GBAItems.Add(15, "Burn Heal")
        GBAItems.Add(16, "Ice Heal")
        GBAItems.Add(17, "Awakening")
        GBAItems.Add(18, "Parlyz Heal")
        GBAItems.Add(19, "Full Restore")
        GBAItems.Add(20, "Max Potion")
        GBAItems.Add(21, "Hyper Potion")
        GBAItems.Add(22, "Super Potion")
        GBAItems.Add(23, "Full Heal")
        GBAItems.Add(24, "Revive")
        GBAItems.Add(25, "Max Revive")
        GBAItems.Add(26, "Fresh Water")
        GBAItems.Add(27, "Soda Pop")
        GBAItems.Add(28, "Lemonade")
        GBAItems.Add(29, "Moomoo Milk")
        GBAItems.Add(30, "Energypowder")
        GBAItems.Add(31, "Energy Root")
        GBAItems.Add(32, "Heal Powder")
        GBAItems.Add(33, "Revival Herb")
        GBAItems.Add(34, "Ether")
        GBAItems.Add(35, "Max Ether")
        GBAItems.Add(36, "Elixir")
        GBAItems.Add(37, "Max Elixir")
        GBAItems.Add(38, "Lava Cookie")
        GBAItems.Add(39, "Blue Flute")
        GBAItems.Add(40, "Yellow Flute")
        GBAItems.Add(41, "Red Flute")
        GBAItems.Add(42, "Black Flute")
        GBAItems.Add(43, "White Flute")
        GBAItems.Add(44, "Berry Juice")
        GBAItems.Add(45, "Sacred Ash")
        GBAItems.Add(46, "Shoal Salt")
        GBAItems.Add(47, "Shoal Shell")
        GBAItems.Add(48, "Red Shard")
        GBAItems.Add(49, "Blue Shard")
        GBAItems.Add(50, "Yellow Shard")
        GBAItems.Add(51, "Green Shard")
        GBAItems.Add(52, "????????")
        GBAItems.Add(53, "????????")
        GBAItems.Add(54, "????????")
        GBAItems.Add(55, "????????")
        GBAItems.Add(56, "????????")
        GBAItems.Add(57, "????????")
        GBAItems.Add(58, "????????")
        GBAItems.Add(59, "????????")
        GBAItems.Add(60, "????????")
        GBAItems.Add(61, "????????")
        GBAItems.Add(62, "????????")
        GBAItems.Add(63, "HP Up")
        GBAItems.Add(64, "Protein")
        GBAItems.Add(65, "Iron")
        GBAItems.Add(66, "Carbos")
        GBAItems.Add(67, "Calcium")
        GBAItems.Add(68, "Rare Candy")
        GBAItems.Add(69, "PP Up")
        GBAItems.Add(70, "Zinc")
        GBAItems.Add(71, "PP Max")
        GBAItems.Add(72, "????????")
        GBAItems.Add(73, "Guard Spec.")
        GBAItems.Add(74, "Dire Hit")
        GBAItems.Add(75, "X Attack")
        GBAItems.Add(76, "X Defend")
        GBAItems.Add(77, "X Speed")
        GBAItems.Add(78, "X Accuracy")
        GBAItems.Add(79, "X Special")
        GBAItems.Add(80, "Poké Doll")
        GBAItems.Add(81, "Fluffy Tail")
        GBAItems.Add(82, "????????")
        GBAItems.Add(83, "Super Repel")
        GBAItems.Add(84, "Max Repel")
        GBAItems.Add(85, "Escape Rope")
        GBAItems.Add(86, "Repel")
        GBAItems.Add(87, "????????")
        GBAItems.Add(88, "????????")
        GBAItems.Add(89, "????????")
        GBAItems.Add(90, "????????")
        GBAItems.Add(91, "????????")
        GBAItems.Add(92, "????????")
        GBAItems.Add(93, "Sun Stone")
        GBAItems.Add(94, "Moon Stone")
        GBAItems.Add(95, "Fire Stone")
        GBAItems.Add(96, "Thunderstone")
        GBAItems.Add(97, "Water Stone")
        GBAItems.Add(98, "Leaf Stone")
        GBAItems.Add(99, " ????????")
        GBAItems.Add(100, " ????????")
        GBAItems.Add(101, " ????????")
        GBAItems.Add(102, " ????????")
        GBAItems.Add(103, "Tinymushroom")
        GBAItems.Add(104, "Big Mushroom")
        GBAItems.Add(105, " ????????")
        GBAItems.Add(106, "Pearl")
        GBAItems.Add(107, "Big Pearl")
        GBAItems.Add(108, "Stardust")
        GBAItems.Add(109, "Star Piece")
        GBAItems.Add(110, "Nugget")
        GBAItems.Add(111, "Heart Scale")
        GBAItems.Add(112, " ????????")
        GBAItems.Add(113, " ????????")
        GBAItems.Add(114, " ????????")
        GBAItems.Add(115, " ????????")
        GBAItems.Add(116, " ????????")
        GBAItems.Add(117, " ????????")
        GBAItems.Add(118, " ????????")
        GBAItems.Add(119, " ????????")
        GBAItems.Add(120, " ????????")
        GBAItems.Add(121, "Orange Mail")
        GBAItems.Add(122, "Harbor Mail")
        GBAItems.Add(123, "Glitter Mail")
        GBAItems.Add(124, "Mech Mail")
        GBAItems.Add(125, "Wood Mail")
        GBAItems.Add(126, "Wave Mail")
        GBAItems.Add(127, "Bead Mail")
        GBAItems.Add(128, "Shadow Mail")
        GBAItems.Add(129, "Tropic Mail")
        GBAItems.Add(130, "Dream Mail")
        GBAItems.Add(131, "Fab Mail")
        GBAItems.Add(132, "Retro Mail")
        GBAItems.Add(133, "Cheri Berry")
        GBAItems.Add(134, "Chesto Berry")
        GBAItems.Add(135, "Pecha Berry")
        GBAItems.Add(136, "Rawst Berry")
        GBAItems.Add(137, "Aspear Berry")
        GBAItems.Add(138, "Leppa Berry")
        GBAItems.Add(139, "Oran Berry")
        GBAItems.Add(140, "Persim Berry")
        GBAItems.Add(141, "Lum Berry")
        GBAItems.Add(142, "Sitrus Berry")
        GBAItems.Add(143, "Figy Berry")
        GBAItems.Add(144, "Wiki Berry")
        GBAItems.Add(145, "Mago Berry")
        GBAItems.Add(146, "Aguav Berry")
        GBAItems.Add(147, "Iapapa Berry")
        GBAItems.Add(148, "Razz Berry")
        GBAItems.Add(149, "Bluk Berry")
        GBAItems.Add(150, "Nanab Berry")
        GBAItems.Add(151, "Wepear Berry")
        GBAItems.Add(152, "Pinap Berry")
        GBAItems.Add(153, "Pomeg Berry")
        GBAItems.Add(154, "Kelpsy Berry")
        GBAItems.Add(155, "Qualot Berry")
        GBAItems.Add(156, "Hondew Berry")
        GBAItems.Add(157, "Grepa Berry")
        GBAItems.Add(158, "Tamato Berry")
        GBAItems.Add(159, "Cornn Berry")
        GBAItems.Add(160, "Magost Berry")
        GBAItems.Add(161, "Rabuta Berry")
        GBAItems.Add(162, "Nomel Berry")
        GBAItems.Add(163, "Spelon Berry")
        GBAItems.Add(164, "Pamtre Berry")
        GBAItems.Add(165, "Watmel Berry")
        GBAItems.Add(166, "Durin Berry")
        GBAItems.Add(167, "Belue Berry")
        GBAItems.Add(168, "Liechi Berry")
        GBAItems.Add(169, "Ganlon Berry")
        GBAItems.Add(170, "Salac Berry")
        GBAItems.Add(171, "Petaya Berry")
        GBAItems.Add(172, "Apicot Berry")
        GBAItems.Add(173, "Lansat Berry")
        GBAItems.Add(174, "Starf Berry")
        GBAItems.Add(175, "Enigma Berry")
        GBAItems.Add(176, " ????????")
        GBAItems.Add(177, " ????????")
        GBAItems.Add(178, " ????????")
        GBAItems.Add(179, "Brightpowder")
        GBAItems.Add(180, "White Herb")
        GBAItems.Add(181, "Macho Brace")
        GBAItems.Add(182, "Exp. Share")
        GBAItems.Add(183, "Quick Claw")
        GBAItems.Add(184, "Soothe Bell")
        GBAItems.Add(185, "Mental Herb")
        GBAItems.Add(186, "Choice Band")
        GBAItems.Add(187, "King's Rock")
        GBAItems.Add(188, "Silverpowder")
        GBAItems.Add(189, "Amulet Coin")
        GBAItems.Add(190, "Cleanse Tag")
        GBAItems.Add(191, "Soul Dew")
        GBAItems.Add(192, "Deepseatooth")
        GBAItems.Add(193, "Deepseascale")
        GBAItems.Add(194, "Smoke Ball")
        GBAItems.Add(195, "Everstone")
        GBAItems.Add(196, "Focus Band")
        GBAItems.Add(197, "Lucky Egg")
        GBAItems.Add(198, "Scope Lens")
        GBAItems.Add(199, "Metal Coat")
        GBAItems.Add(200, "Leftovers")
        GBAItems.Add(201, "Dragon Scale")
        GBAItems.Add(202, "Light Ball")
        GBAItems.Add(203, "Soft Sand")
        GBAItems.Add(204, "Hard Stone")
        GBAItems.Add(205, "Miracle Seed")
        GBAItems.Add(206, "Blackglasses")
        GBAItems.Add(207, "Black Belt")
        GBAItems.Add(208, "Magnet")
        GBAItems.Add(209, "Mystic Water")
        GBAItems.Add(210, "Sharp Beak")
        GBAItems.Add(211, "Poison Barb")
        GBAItems.Add(212, "Nevermeltice")
        GBAItems.Add(213, "Spell Tag")
        GBAItems.Add(214, "Twistedspoon")
        GBAItems.Add(215, "Charcoal")
        GBAItems.Add(216, "Dragon Fang")
        GBAItems.Add(217, "Silk Scarf")
        GBAItems.Add(218, "Up-Grade")
        GBAItems.Add(219, "Shell Bell")
        GBAItems.Add(220, "Sea Incense")
        GBAItems.Add(221, "Lax Incense")
        GBAItems.Add(222, "Lucky Punch")
        GBAItems.Add(223, "Metal Powder")
        GBAItems.Add(224, "Thick Club")
        GBAItems.Add(225, "Stick")
        GBAItems.Add(226, " ????????")
        GBAItems.Add(227, " ????????")
        GBAItems.Add(228, " ????????")
        GBAItems.Add(229, " ????????")
        GBAItems.Add(230, " ????????")
        GBAItems.Add(231, " ????????")
        GBAItems.Add(232, " ????????")
        GBAItems.Add(233, " ????????")
        GBAItems.Add(234, " ????????")
        GBAItems.Add(235, " ????????")
        GBAItems.Add(236, " ????????")
        GBAItems.Add(237, " ????????")
        GBAItems.Add(238, " ????????")
        GBAItems.Add(239, " ????????")
        GBAItems.Add(240, " ????????")
        GBAItems.Add(241, " ????????")
        GBAItems.Add(242, " ????????")
        GBAItems.Add(243, " ????????")
        GBAItems.Add(244, " ????????")
        GBAItems.Add(245, " ????????")
        GBAItems.Add(246, " ????????")
        GBAItems.Add(247, " ????????")
        GBAItems.Add(248, " ????????")
        GBAItems.Add(249, " ????????")
        GBAItems.Add(250, " ????????")
        GBAItems.Add(251, " ????????")
        GBAItems.Add(252, " ????????")
        GBAItems.Add(253, " ????????")
        GBAItems.Add(254, "Red Scarf")
        GBAItems.Add(255, "Blue Scarf")
        GBAItems.Add(256, "Pink Scarf")
        GBAItems.Add(257, "Green Scarf")
        GBAItems.Add(258, "Yellow Scarf")
        GBAItems.Add(259, "Mach Bike")
        GBAItems.Add(260, "Coin Case")
        GBAItems.Add(261, "Itemfinder")
        GBAItems.Add(262, "Old Rod")
        GBAItems.Add(263, "Good Rod")
        GBAItems.Add(264, "Super Rod")
        GBAItems.Add(265, "S. S. Ticket")
        GBAItems.Add(266, "Contest Pass")
        GBAItems.Add(267, " ????????")
        GBAItems.Add(268, "Wailmer Pail")
        GBAItems.Add(269, "Devon Goods")
        GBAItems.Add(270, "Soot Sack")
        GBAItems.Add(271, "Basement Key")
        GBAItems.Add(272, "Acro Bike")
        GBAItems.Add(273, "PkBlock Case")
        GBAItems.Add(274, "Letter")
        GBAItems.Add(275, "Eon Ticket")
        GBAItems.Add(276, "Red Orb")
        GBAItems.Add(277, "Blue Orb")
        GBAItems.Add(278, "Scanner")
        GBAItems.Add(279, "Go-Goggles")
        GBAItems.Add(280, "Meteorite")
        GBAItems.Add(281, "Rm. 1 Key")
        GBAItems.Add(282, "Rm. 2 Key")
        GBAItems.Add(283, "Rm. 4 Key")
        GBAItems.Add(284, "Rm. 6 Key")
        GBAItems.Add(285, "Storage Key")
        GBAItems.Add(286, "Root Fossil")
        GBAItems.Add(287, "Claw Fossil")
        GBAItems.Add(288, "Devon Scope")
        GBAItems.Add(289, "TM01")
        GBAItems.Add(290, "TM02")
        GBAItems.Add(291, "TM03")
        GBAItems.Add(292, "TM04")
        GBAItems.Add(293, "TM05")
        GBAItems.Add(294, "TM06")
        GBAItems.Add(295, "TM07")
        GBAItems.Add(296, "TM08")
        GBAItems.Add(297, "TM09")
        GBAItems.Add(298, "TM10")
        GBAItems.Add(299, "TM11")
        GBAItems.Add(300, "TM12")
        GBAItems.Add(301, "TM13")
        GBAItems.Add(302, "TM14")
        GBAItems.Add(303, "TM15")
        GBAItems.Add(304, "TM16")
        GBAItems.Add(305, "TM17")
        GBAItems.Add(306, "TM18")
        GBAItems.Add(307, "TM19")
        GBAItems.Add(308, "TM20")
        GBAItems.Add(309, "TM21")
        GBAItems.Add(310, "TM22")
        GBAItems.Add(311, "TM23")
        GBAItems.Add(312, "TM24")
        GBAItems.Add(313, "TM25")
        GBAItems.Add(314, "TM26")
        GBAItems.Add(315, "TM27")
        GBAItems.Add(316, "TM28")
        GBAItems.Add(317, "TM29")
        GBAItems.Add(318, "TM30")
        GBAItems.Add(319, "TM31")
        GBAItems.Add(320, "TM32")
        GBAItems.Add(321, "TM33")
        GBAItems.Add(322, "TM34")
        GBAItems.Add(323, "TM35")
        GBAItems.Add(324, "TM36")
        GBAItems.Add(325, "TM37")
        GBAItems.Add(326, "TM38")
        GBAItems.Add(327, "TM39")
        GBAItems.Add(328, "TM40")
        GBAItems.Add(329, "TM41")
        GBAItems.Add(330, "TM42")
        GBAItems.Add(331, "TM43")
        GBAItems.Add(332, "TM44")
        GBAItems.Add(333, "TM45")
        GBAItems.Add(334, "TM46")
        GBAItems.Add(335, "TM47")
        GBAItems.Add(336, "TM48")
        GBAItems.Add(337, "TM49")
        GBAItems.Add(338, "TM50")
        GBAItems.Add(339, "HM01")
        GBAItems.Add(340, "HM02")
        GBAItems.Add(341, "HM03")
        GBAItems.Add(342, "HM04")
        GBAItems.Add(343, "HM05")
        GBAItems.Add(344, "HM06")
        GBAItems.Add(345, "HM07")
        GBAItems.Add(346, "HM08")
        GBAItems.Add(347, " ????????")
        GBAItems.Add(348, " ????????")
        GBAItems.Add(349, "Oak's Parcel")
        GBAItems.Add(350, "Poké Flute")
        GBAItems.Add(351, "Secret Key")
        GBAItems.Add(352, "Bike Voucher")
        GBAItems.Add(353, "Gold Teeth")
        GBAItems.Add(354, "Old Amber")
        GBAItems.Add(355, "Card Key")
        GBAItems.Add(356, "Elevator Key")
        GBAItems.Add(357, "Dome Fossil")
        GBAItems.Add(358, "Helix Fossil")
        GBAItems.Add(359, "Silph Scope")
        GBAItems.Add(360, "Bicycle")
        GBAItems.Add(361, "Town Map")
        GBAItems.Add(362, "VS Seeker")
        GBAItems.Add(363, "Fame Checker")
        GBAItems.Add(364, "TM Case")
        GBAItems.Add(365, "Berry Bag")
        GBAItems.Add(366, "Teachy TV")
        GBAItems.Add(367, "Tri-Pass")
        GBAItems.Add(368, "Rainbow Pass")
        GBAItems.Add(369, "Tea")
        GBAItems.Add(370, "Mysticticket")
        GBAItems.Add(371, "Auroraticket")
        GBAItems.Add(372, "Powder Jar")
        GBAItems.Add(373, "Ruby")
        GBAItems.Add(374, "Sapphire")
        GBAItems.Add(375, "Magma Emblem")
        GBAItems.Add(376, "Old Sea Map")

        GBALocations.Add(0, "Littleroot Town")
        GBALocations.Add(1, "Oldale Town")
        GBALocations.Add(2, "Dewford Town")
        GBALocations.Add(3, "Lavaridge Town")
        GBALocations.Add(4, "Fallarbor Town")
        GBALocations.Add(5, "Verdanturf Town")
        GBALocations.Add(6, "Pacifidlog Town")
        GBALocations.Add(7, "Petalburg City")
        GBALocations.Add(8, "Slateport City")
        GBALocations.Add(9, "Mauville City")
        GBALocations.Add(10, "Rustboro City")
        GBALocations.Add(11, "Fortree City")
        GBALocations.Add(12, "Lilycove City")
        GBALocations.Add(13, "Mossdeep City")
        GBALocations.Add(14, "Sootopolis City")
        GBALocations.Add(15, "Ever Grande City")
        GBALocations.Add(16, "Route 101")
        GBALocations.Add(17, "Route 102")
        GBALocations.Add(18, "Route 103")
        GBALocations.Add(19, "Route 104")
        GBALocations.Add(20, "Route 105")
        GBALocations.Add(21, "Route 106")
        GBALocations.Add(22, "Route 107")
        GBALocations.Add(23, "Route 108")
        GBALocations.Add(24, "Route 109")
        GBALocations.Add(25, "Route 110")
        GBALocations.Add(26, "Route 111")
        GBALocations.Add(27, "Route 112")
        GBALocations.Add(28, "Route 113")
        GBALocations.Add(29, "Route 114")
        GBALocations.Add(30, "Route 115")
        GBALocations.Add(31, "Route 116")
        GBALocations.Add(32, "Route 117")
        GBALocations.Add(33, "Route 118")
        GBALocations.Add(34, "Route 119")
        GBALocations.Add(35, "Route 120")
        GBALocations.Add(36, "Route 121")
        GBALocations.Add(37, "Route 122")
        GBALocations.Add(38, "Route 123")
        GBALocations.Add(39, "Route 124")
        GBALocations.Add(40, "Route 125")
        GBALocations.Add(41, "Route 126")
        GBALocations.Add(42, "Route 127")
        GBALocations.Add(43, "Route 128")
        GBALocations.Add(44, "Route 129")
        GBALocations.Add(45, "Route 130")
        GBALocations.Add(46, "Route 131")
        GBALocations.Add(47, "Route 132")
        GBALocations.Add(48, "Route 133")
        GBALocations.Add(49, "Route 134")
        GBALocations.Add(50, "Underwater (Route 124)")
        GBALocations.Add(51, "Underwater (Route 126)")
        GBALocations.Add(52, "Underwater (Route 127)")
        GBALocations.Add(53, "Underwater (Route 128)")
        GBALocations.Add(54, "Underwater (Sootopolis City)")
        GBALocations.Add(55, "Battle Tower")
        GBALocations.Add(56, "Mt. Chimney")
        GBALocations.Add(57, "Safari Zone")
        GBALocations.Add(58, "Battle Frontier")
        GBALocations.Add(59, "Petalburg Woods")
        GBALocations.Add(60, "Rusturf Tunnel")
        GBALocations.Add(61, "Abandoned Ship")
        GBALocations.Add(62, "New Mauville")
        GBALocations.Add(63, "Meteor Falls")
        GBALocations.Add(64, "Meteor Falls")
        GBALocations.Add(65, "Mt. Pyre")
        GBALocations.Add(66, "Aqua/Magma Hideout")
        GBALocations.Add(67, "Shoal Cave")
        GBALocations.Add(68, "Seafloor Cavern")
        GBALocations.Add(69, "Underwater (Seafloor Cavern)")
        GBALocations.Add(70, "Victory Road")
        GBALocations.Add(71, "Mirage Island")
        GBALocations.Add(72, "Cave of Origin")
        GBALocations.Add(73, "Southern Island")
        GBALocations.Add(74, "Fiery Path")
        GBALocations.Add(75, "Fiery Path")
        GBALocations.Add(76, "Jagged Pass")
        GBALocations.Add(77, "Jagged Pass")
        GBALocations.Add(78, "Sealed Chamber")
        GBALocations.Add(79, "Underwater (Route 134)")
        GBALocations.Add(80, "Scorched Slab")
        GBALocations.Add(81, "Island Cave")
        GBALocations.Add(82, "Desert Ruins")
        GBALocations.Add(83, "Ancient Tomb")
        GBALocations.Add(84, "Inside of Truck")
        GBALocations.Add(85, "Sky Pillar")
        GBALocations.Add(86, "Secret Base")
        GBALocations.Add(87, "Ferry")
        GBALocations.Add(88, "Pallet Town")
        GBALocations.Add(89, "Viridian City")
        GBALocations.Add(90, "Pewter City")
        GBALocations.Add(91, "Cerulean City")
        GBALocations.Add(92, "Lavender Town")
        GBALocations.Add(93, "Vermilion City")
        GBALocations.Add(94, "Celadon City")
        GBALocations.Add(95, "Fuchsia City")
        GBALocations.Add(96, "Cinnabar Island")
        GBALocations.Add(97, "Indigo Plateau")
        GBALocations.Add(98, "Saffron City")
        GBALocations.Add(99, "Route 4")
        GBALocations.Add(100, "Route 10")
        GBALocations.Add(101, "Route 1")
        GBALocations.Add(102, "Route 2")
        GBALocations.Add(103, "Route 3")
        GBALocations.Add(104, "Route 4")
        GBALocations.Add(105, "Route 5")
        GBALocations.Add(106, "Route 6")
        GBALocations.Add(107, "Route 7")
        GBALocations.Add(108, "Route 8")
        GBALocations.Add(109, "Route 9")
        GBALocations.Add(110, "Route 10")
        GBALocations.Add(111, "Route 11")
        GBALocations.Add(112, "Route 12")
        GBALocations.Add(113, "Route 13")
        GBALocations.Add(114, "Route 14")
        GBALocations.Add(115, "Route 15")
        GBALocations.Add(116, "Route 16")
        GBALocations.Add(117, "Route 17")
        GBALocations.Add(118, "Route 18")
        GBALocations.Add(119, "Route 19")
        GBALocations.Add(120, "Route 20")
        GBALocations.Add(121, "Route 21")
        GBALocations.Add(122, "Route 22")
        GBALocations.Add(123, "Route 23")
        GBALocations.Add(124, "Route 24")
        GBALocations.Add(125, "Route 25")
        GBALocations.Add(126, "Viridian Forest")
        GBALocations.Add(127, "Mt. Moon")
        GBALocations.Add(128, "S.S. Anne")
        GBALocations.Add(129, "Underground Path (Routes 5-6)")
        GBALocations.Add(130, "Underground Path (Routes 7-8)")
        GBALocations.Add(131, "Diglett's Cave")
        GBALocations.Add(132, "Victory Road")
        GBALocations.Add(133, "Rocket Hideout")
        GBALocations.Add(134, "Silph Co.")
        GBALocations.Add(135, "Pokémon Mansion")
        GBALocations.Add(136, "Safari Zone")
        GBALocations.Add(137, "Pokémon League")
        GBALocations.Add(138, "Rock Tunnel")
        GBALocations.Add(139, "Seafoam Islands")
        GBALocations.Add(140, "Pokémon Tower")
        GBALocations.Add(141, "Cerulean Cave")
        GBALocations.Add(142, "Power Plant")
        GBALocations.Add(143, "One Island")
        GBALocations.Add(144, "Two Island")
        GBALocations.Add(145, "Three Island")
        GBALocations.Add(146, "Four Island")
        GBALocations.Add(147, "Five Island")
        GBALocations.Add(148, "Seven Island")
        GBALocations.Add(149, "Six Island")
        GBALocations.Add(150, "Kindle Road")
        GBALocations.Add(151, "Treasure Beach")
        GBALocations.Add(152, "Cape Brink")
        GBALocations.Add(153, "Bond Bridge")
        GBALocations.Add(154, "Three Isle Port")
        GBALocations.Add(155, "Sevii Isle 6")
        GBALocations.Add(156, "Sevii Isle 7")
        GBALocations.Add(157, "Sevii Isle 8")
        GBALocations.Add(158, "Sevii Isle 9")
        GBALocations.Add(159, "Resort Gorgeous")
        GBALocations.Add(160, "Water Labyrinth")
        GBALocations.Add(161, "Five Isle Meadow")
        GBALocations.Add(162, "Memorial Pillar")
        GBALocations.Add(163, "Outcast Island")
        GBALocations.Add(164, "Green Path")
        GBALocations.Add(165, "Water Path")
        GBALocations.Add(166, "Ruin Valley")
        GBALocations.Add(167, "Trainer Tower")
        GBALocations.Add(168, "Canyon Entrance")
        GBALocations.Add(169, "Sevault Canyon")
        GBALocations.Add(170, "Tanoby Ruins")
        GBALocations.Add(171, "Sevii Isle 22")
        GBALocations.Add(172, "Sevii Isle 23")
        GBALocations.Add(173, "Sevii Isle 24")
        GBALocations.Add(174, "Navel Rock")
        GBALocations.Add(175, "Mt. Ember")
        GBALocations.Add(176, "Berry Forest")
        GBALocations.Add(177, "Icefall Cave")
        GBALocations.Add(178, "Rocket Warehouse")
        GBALocations.Add(179, "Trainer Tower")
        GBALocations.Add(180, "Dotted Hole")
        GBALocations.Add(181, "Lost Cave")
        GBALocations.Add(182, "Pattern Bush")
        GBALocations.Add(183, "Altering Cave")
        GBALocations.Add(184, "Tanoby Chambers")
        GBALocations.Add(185, "Three Isle Path")
        GBALocations.Add(186, "Tanoby Key")
        GBALocations.Add(187, "Birth Island")
        GBALocations.Add(188, "Monean Chamber")
        GBALocations.Add(189, "Liptoo Chamber")
        GBALocations.Add(190, "Weepth Chamber")
        GBALocations.Add(191, "Dilford Chamber")
        GBALocations.Add(192, "Scufib Chamber")
        GBALocations.Add(193, "Rixy Chamber")
        GBALocations.Add(194, "Viapois Chamber")
        GBALocations.Add(195, "Ember Spa")
        GBALocations.Add(196, "Special Area")
        GBALocations.Add(197, "Aqua Hideout")
        GBALocations.Add(198, "Magma Hideout")
        GBALocations.Add(199, "Mirage Tower")
        GBALocations.Add(200, "Birth Island")
        GBALocations.Add(201, "Faraway Island")
        GBALocations.Add(202, "Artisan Cave")
        GBALocations.Add(203, "Marine Cave")
        GBALocations.Add(204, "Underwater (Marine Cave)")
        GBALocations.Add(205, "Terra Cave")
        GBALocations.Add(206, "Underwater (Marine Cave)")
        GBALocations.Add(207, "Underwater (Marine Cave)")
        GBALocations.Add(208, "Underwater (Marine Cave)")
        GBALocations.Add(209, "Desert Underpass")
        GBALocations.Add(210, "Altering Cave")
        GBALocations.Add(211, "Navel Rock")
        GBALocations.Add(212, "Trainer Hill")
        GBALocations.Add(254, "In-game Trade")
        GBALocations.Add(255, "Fateful encounter")

        mDictionariesInitialized = True
    End Sub

#End Region

#Region "Public Variables"

    Public Shared PKRSCured As Bitmap = My.Resources.PKRS_CURED
    Public Shared ShinyStar As Bitmap = My.Resources.SHINY_PNG
    Public Shared MarkOriginal As Color = My.Resources.Square.GetPixel(0, 0)
    Public Shared _Hand1 As Icon = My.Resources.Box_Hand_1
    Public Shared _Hand2 As Icon = My.Resources.Box_Hand_2
    Public Shared _Hand3 As Icon = My.Resources.Box_Hand_3
    Public Shared _DPMaleTrainer As Bitmap = My.Resources.DPMaleTrainer
    Public Shared _DPFemaleTrainer As Bitmap = My.Resources.DPFemaleTrainer
    Public Shared _PtMaleTrainer As Bitmap = My.Resources.PtMaleTrainer
    Public Shared _PtFemaleTrainer As Bitmap = My.Resources.PtFemaleTrainer
    Public Shared DSBadgeNames As String() = New String() {"Coal Badge", _
                                                                                 "Forest Badge", _
                                                                                 "Cobble Badge", _
                                                                                 "Fen Badge", _
                                                                                 "Relic Badge", _
                                                                                 "Mine Badge", _
                                                                                 "Icicle Badge", _
                                                                                 "Beacon Badge"}

#End Region

#Region "Private Variables"

    Public Shared CNVRT As New PKMFontConverter

    Friend Shared BlankParty As Byte() = { _
     &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H0, &H7E, &HE9, _
     &H71, &H52, &HB0, &H31, &H42, &H8E, &HCC, &HE2, &HC5, &HAF, &HDB, &H67, _
     &H33, &HFC, &H2C, &HEF, &H5E, &HFC, &HC5, &HCA, &HD6, &HEB, &H3D, &H99, _
     &HBC, &H7A, &HA7, &HCB, &HD6, &H5D, &H78, &H91, &HA6, &H27, &H8D, &H61, _
     &H92, &H16, &HB8, &HCF, &H5D, &H37, &H80, &H30, &H7C, &H40, &HFB, &H48, _
     &H13, &H32, &HE7, &HFE, &HA3, &HDF, &H69, &H3D, &H9E, &H63, &H29, &H1D, _
     &H8D, &HEA, &H96, &H62, &H68, &H92, &H97, &HA3, &H49, &H1C, &H3, &H6E, _
     &HAA, &H31, &H89, &HAA, &HC5, &HD3, &HEA, &HC3, &HD9, &H82, &HC6, &HE0, _
     &H5C, &H94, &H3B, &H4E, &H5F, &H5A, &H28, &H24, &HB3, &HFB, &HE1, &HBF, _
     &H8E, &H7B, &H7F, &H0, &HC4, &H40, &H48, &HC8, &HD1, &HBF, &HB6, &H38, _
     &H3B, &H90, &H23, &HFB, &H23, &H7D, &H34, &HBE, &H0, &HDA, &H6A, &H70, _
     &HC5, &HDF, &H84, &HBA, &H0, &H0, &H7E, &HE9, &H71, &H52, &HB0, &H31, _
     &H42, &H8E, &HCC, &HE2, &HC5, &HAF, &HDB, &H67, &H33, &HFC, &H2C, &HEF, _
     &H5E, &HFC, &HC5, &HCA, &HD6, &HEB, &H3D, &H99, &HBC, &H7A, &HA7, &HCB, _
     &HD6, &H5D, &H78, &H91, &HA6, &H27, &H8D, &H61, &H92, &H16, &HB8, &HCF, _
     &H5D, &H37, &H80, &H30, &H7C, &H40, &HFB, &H48, &H13, &H32, &HE7, &HFE, _
     &HA3, &HDF, &H69, &H3D, &H9E, &H63, &H29, &H1D, &H8D, &HEA, &H96, &H62, _
     &H68, &H92, &H97, &HA3, &H49, &H1C, &H3, &H6E, &HAA, &H31, &H89, &HAA, _
     &HC5, &HD3, &HEA, &HC3, &HD9, &H82, &HC6, &HE0, &H5C, &H94, &H3B, &H4E, _
     &H5F, &H5A, &H28, &H24, &HB3, &HFB, &HE1, &HBF _
    }

    Public Shared SpriteBack As Color = Color.FromArgb(255, 240, 240, 240)

#End Region

    Friend Shared out(1) As Byte

#Region "Functions"

    ''' <summary>
    ''' Convert the given byte array into a Unicode string based on the Pokémon DS character table.
    ''' </summary>
    ''' <param name="Byte_Array"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function PKMBytesToString(ByVal Byte_Array() As Byte) _
As String

        PokemonLib.InitializeDictionaries()

        If Byte_Array.Length = 1 Then
            Dim buf As Byte = Byte_Array(0)
            ReDim Byte_Array(1)
            Byte_Array(0) = buf
        End If

        Dim strName As String = ""
        For iCount As Integer = 0 To Byte_Array.Length - 1 Step 2
            Dim iByte As UInt16 = BitConverter.ToUInt16(New Byte() {Byte_Array(iCount), Byte_Array(iCount + 1)}, 0)
            If iByte >= 0 Then
                If iByte <> &HFFFF Then strName &= Char.ConvertFromUtf32(CNVRT.PKMToUnicode(iByte))
            End If
            If Byte_Array(iCount + 0) = &HFF And Byte_Array(iCount + 1) = &HFF _
             Then Return strName
        Next
        Return strName

    End Function

    ''' <summary>
    ''' Convert the given Unicode string into a byte array based on the Pokémon DS character table.
    ''' </summary>
    ''' <param name="mString"></param>
    ''' <param name="Terminator"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function StringToPKMBytes(ByVal mString As String, Optional ByVal Terminator As Boolean = False) As Byte()
        If mString = "" Then Return New Byte() {}
        Dim Data((mString.Length * 2) + 1) As Byte
        Dim strData As String = ""
        For i As Integer = 0 To mString.Length - 1
            Dim _i As Byte() = BitConverter.GetBytes(CNVRT.UnicodeToPKM(Char.ConvertToUtf32(mString, i)))
            strData &= _i(0) & "," & _i(1) & ","
        Next
        strData = strData.Substring(0, strData.LastIndexOf(","))
        Dim _strData() As String = strData.Split(",")
        CNVRT.DictionaryInitialize()
        For i As Integer = 0 To _strData.Length - 1
            'Try
            Data(i) = Integer.Parse(_strData(i))
            'Catch ex As Exception
            '    Console.WriteLine(Err.Number & ": " & ex.Message)
            '    Stop
            'End Try
        Next
        If Terminator Then
            If _strData.Length = Data.Length Then
                Data(Data.Length - 2) = &HFF
                Data(Data.Length - 1) = &HFF
            Else
                Data(_strData.Length) = &HFF
                Data(_strData.Length + 1) = &HFF
            End If
        End If
        strData = ""
        For i As Integer = 0 To Data.Length - 1
            strData &= Hex(Data(i)) & ", "
        Next
        If Terminator Then Return Data
        Dim dataout(Data.Length - 3) As Byte
        Array.Copy(Data, 0, dataout, 0, dataout.Length)
        Return dataout
    End Function

    ''' <summary>
    ''' Converts Unix's epoch time to VB DateTime value
    ''' </summary>
    ''' <param name="EpochValue">Epoch time (seconds)</param>
    ''' <returns>VB Date</returns>
    ''' <remarks></remarks>
    Friend Shared Function EpochToDateTime(ByVal EpochValue As Integer) As Date

        If EpochValue >= 0 Then
            Return CDate("1.1.2000 00:00:00").AddSeconds(EpochValue)
        Else
            Return CDate("1.1.2000 00:00:00")
        End If

    End Function

    ''' <summary>
    ''' Decrypt raw Pokémon data.
    ''' </summary>
    ''' <param name="Data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Shared Function Decrypt_Data(ByVal Data() As Byte) As Byte()

        Dim Checksum As UInt16 = BitConverter.ToUInt16(Data, &H6)

        Dim ucheck As UInt16 = Checksum
        Dim prng As New PokemonLib.PokePRNG '(ucheck)
        prng.Seed = ucheck

        For i As Integer = 8 To 135 Step 2
            Dim bef As UShort = BitConverter.ToUInt16(Data, i)
            Dim aft As UShort = CUShort((bef Xor (prng.NextNum() >> &H10)))
            Data(i + 1) = CByte((aft >> &H8))
            Data(i) = CByte((aft And &HFF))
        Next

        If Data.Length = 136 Then Return Data

        prng.Seed = BitConverter.ToUInt32(Data, 0)

        For i As Integer = 136 To 235 Step 2
            Dim bef As UShort = BitConverter.ToUInt16(Data, i)
            Dim aft As UShort = CUShort((bef Xor (prng.NextNum() >> &H10)))
            Data(i + 1) = CByte((aft >> &H8))
            Data(i) = CByte((aft And &HFF))
        Next

        Return Data

    End Function

    ''' <summary>
    ''' Decrypt and unshuffle the given raw Pokémon data.
    ''' </summary>
    ''' <param name="PKM"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function DecryptPokemon(ByVal PKM() As Byte) As Byte()
        Return UnShuffleBytes(Decrypt_Data(PKM))
    End Function

    ''' <summary>
    ''' Encrypt and shuffle the given raw Pokémon data.
    ''' </summary>
    ''' <param name="PKM"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function EncryptPokemon(ByVal PKM() As Byte) As Byte()
        Dim checksum As UInt16 = Calculate_Checksum(PKM)
        Dim chkBytes() As Byte = BitConverter.GetBytes(checksum)

        PKM(6) = chkBytes(0)
        PKM(7) = chkBytes(1)

        Dim Encrypted() As Byte = ShuffleBytes(PKM)

        Encrypted = Decrypt_Data(Encrypted)

        Return Encrypted

        Encrypted = Nothing
        PKM = Nothing
    End Function

    ''' <summary>
    ''' Calculate the checksum of the given Pokémon data.
    ''' </summary>
    ''' <param name="PKM"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Calculate_Checksum(ByVal PKM() As Byte) As UInt16

        Dim Data(PKM.Length - 1) As Byte
        For i As Integer = 0 To Data.Length - 1
            Data(i) = PKM(i)
        Next

        Dim index As UInteger = 0

        For i As Integer = &H8 To &H86 Step 2
            index += Data(i) + (Data(i + 1) * 256)
        Next

        Dim bin As String = DecToBin(index, 32)
        bin = bin.Substring(16, 16)
        Return Convert.ToUInt16(bin, 2)

    End Function

    ''' <summary>
    ''' Shuffle the bytes in the given .pkm byte array.
    ''' </summary>
    ''' <param name="UnencryptedData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ShuffleBytes(ByVal UnencryptedData() As Byte) As Byte()
        InitializeDictionaries()
        Dim PID As UInt32 = BitConverter.ToUInt32(UnencryptedData, 0)
        Dim UnShuffleIndex As UInt16 = ((PID >> &HD) And &H1F) Mod 24
        Dim ShuffleOrder As String = dpPKMShuffle(UnShuffleIndex)

        Dim Block1(31) As Byte
        Dim Block2(31) As Byte
        Dim Block3(31) As Byte
        Dim Block4(31) As Byte

        Select Case ShuffleOrder.Substring(0, 1)
            Case "A"
                For i As Integer = &H8 To &H27
                    Block1(i - &H8) = UnencryptedData(i)
                Next
            Case "B"
                For i As Integer = &H28 To &H47
                    Block1(i - &H28) = UnencryptedData(i)
                Next
            Case "C"
                For i As Integer = &H48 To &H67
                    Block1(i - &H48) = UnencryptedData(i)
                Next
            Case "D"
                For i As Integer = &H68 To &H87
                    Block1(i - &H68) = UnencryptedData(i)
                Next
        End Select

        Select Case ShuffleOrder.Substring(1, 1)
            Case "A"
                For i As Integer = &H8 To &H27
                    Block2(i - &H8) = UnencryptedData(i)
                Next
            Case "B"
                For i As Integer = &H28 To &H47
                    Block2(i - &H28) = UnencryptedData(i)
                Next
            Case "C"
                For i As Integer = &H48 To &H67
                    Block2(i - &H48) = UnencryptedData(i)
                Next
            Case "D"
                For i As Integer = &H68 To &H87
                    Block2(i - &H68) = UnencryptedData(i)
                Next
        End Select

        Select Case ShuffleOrder.Substring(2, 1)
            Case "A"
                For i As Integer = &H8 To &H27
                    Block3(i - &H8) = UnencryptedData(i)
                Next
            Case "B"
                For i As Integer = &H28 To &H47
                    Block3(i - &H28) = UnencryptedData(i)
                Next
            Case "C"
                For i As Integer = &H48 To &H67
                    Block3(i - &H48) = UnencryptedData(i)
                Next
            Case "D"
                For i As Integer = &H68 To &H87
                    Block3(i - &H68) = UnencryptedData(i)
                Next
        End Select

        Select Case ShuffleOrder.Substring(3, 1)
            Case "A"
                For i As Integer = &H8 To &H27
                    Block4(i - &H8) = UnencryptedData(i)
                Next
            Case "B"
                For i As Integer = &H28 To &H47
                    Block4(i - &H28) = UnencryptedData(i)
                Next
            Case "C"
                For i As Integer = &H48 To &H67
                    Block4(i - &H48) = UnencryptedData(i)
                Next
            Case "D"
                For i As Integer = &H68 To &H87
                    Block4(i - &H68) = UnencryptedData(i)
                Next
        End Select

        For i As Integer = &H8 To &H27

            UnencryptedData(i) = Block1(i - &H8)

        Next

        For i As Integer = &H28 To &H47

            UnencryptedData(i) = Block2(i - &H28)

        Next

        For i As Integer = &H48 To &H67

            UnencryptedData(i) = Block3(i - &H48)

        Next

        For i As Integer = &H68 To &H87

            UnencryptedData(i) = Block4(i - &H68)

        Next

        Return UnencryptedData
    End Function

    ''' <summary>
    ''' Unshuffle the bytes in the given .pkm byte array.
    ''' </summary>
    ''' <param name="UnencryptedData"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Friend Shared Function UnShuffleBytes(ByVal UnencryptedData() As Byte) As Byte()
        InitializeDictionaries()
        Dim PID As UInt32 = BitConverter.ToUInt32(UnencryptedData, 0)
        Dim UnShuffleIndex As UInt16 = ((PID >> &HD) And &H1F) Mod 24
        Dim ShuffleOrder As String = dpPKMShuffle(UnShuffleIndex)

        Dim Block1(31) As Byte
        Dim Block2(31) As Byte
        Dim Block3(31) As Byte
        Dim Block4(31) As Byte

        Select Case ShuffleOrder.Substring(0, 1)
            Case "A"
                For i As Integer = &H8 To &H27
                    Block1(i - &H8) = UnencryptedData(i)
                Next
            Case "B"
                For i As Integer = &H8 To &H27
                    Block2(i - &H8) = UnencryptedData(i)
                Next
            Case "C"
                For i As Integer = &H8 To &H27
                    Block3(i - &H8) = UnencryptedData(i)
                Next
            Case "D"
                For i As Integer = &H8 To &H27
                    Block4(i - &H8) = UnencryptedData(i)
                Next
        End Select

        Select Case ShuffleOrder.Substring(1, 1)
            Case "A"
                For i As Integer = &H28 To &H47
                    Block1(i - &H28) = UnencryptedData(i)
                Next
            Case "B"
                For i As Integer = &H28 To &H47
                    Block2(i - &H28) = UnencryptedData(i)
                Next
            Case "C"
                For i As Integer = &H28 To &H47
                    Block3(i - &H28) = UnencryptedData(i)
                Next
            Case "D"
                For i As Integer = &H28 To &H47
                    Block4(i - &H28) = UnencryptedData(i)
                Next
        End Select

        Select Case ShuffleOrder.Substring(2, 1)
            Case "A"
                For i As Integer = &H48 To &H67
                    Block1(i - &H48) = UnencryptedData(i)
                Next
            Case "B"
                For i As Integer = &H48 To &H67
                    Block2(i - &H48) = UnencryptedData(i)
                Next
            Case "C"
                For i As Integer = &H48 To &H67
                    Block3(i - &H48) = UnencryptedData(i)
                Next
            Case "D"
                For i As Integer = &H48 To &H67
                    Block4(i - &H48) = UnencryptedData(i)
                Next
        End Select

        Select Case ShuffleOrder.Substring(3, 1)
            Case "A"
                For i As Integer = &H68 To &H87
                    Block1(i - &H68) = UnencryptedData(i)
                Next
            Case "B"
                For i As Integer = &H68 To &H87
                    Block2(i - &H68) = UnencryptedData(i)
                Next
            Case "C"
                For i As Integer = &H68 To &H87
                    Block3(i - &H68) = UnencryptedData(i)
                Next
            Case "D"
                For i As Integer = &H68 To &H87
                    Block4(i - &H68) = UnencryptedData(i)
                Next
        End Select

        For i As Integer = &H8 To &H27

            UnencryptedData(i) = Block1(i - &H8)

        Next

        For i As Integer = &H28 To &H47

            UnencryptedData(i) = Block2(i - &H28)

        Next

        For i As Integer = &H48 To &H67

            UnencryptedData(i) = Block3(i - &H48)

        Next

        For i As Integer = &H68 To &H87

            UnencryptedData(i) = Block4(i - &H68)

        Next

        Return UnencryptedData
    End Function

    ''' <summary>
    ''' Convert given binary data into an object.
    ''' </summary>
    ''' <param name="rawData"></param>
    ''' <param name="anyType"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RawDeserialize(ByVal rawData As Byte(), ByVal anyType As Type) As Object
        Dim rawsize As Integer = Marshal.SizeOf(anyType)
        rawsize = rawData.Length
        Dim buffer As IntPtr = Marshal.AllocHGlobal(rawsize)
        Marshal.Copy(rawData, 0, buffer, rawsize)
        Dim retobj As Object = Marshal.PtrToStructure(buffer, anyType)
        Marshal.FreeHGlobal(buffer)
        Return retobj
    End Function

    ''' <summary>
    ''' Convert a given object into raw binary data.
    ''' </summary>
    ''' <param name="anything"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RawSerialize(ByVal anything As Object) As Byte()
        Dim rawSize As Integer = Marshal.SizeOf(anything)
        Dim buffer As IntPtr = Marshal.AllocHGlobal(rawSize)
        Marshal.StructureToPtr(anything, buffer, False)
        Dim rawDatas As Byte() = New Byte(rawSize - 1) {}
        Marshal.Copy(buffer, rawDatas, 0, rawSize)
        Marshal.FreeHGlobal(buffer)
        Return rawDatas
    End Function

    ''' <summary>
    ''' Convert a Decimal number to a binary String.
    ''' </summary>
    ''' <param name="DeciValue"></param>
    ''' <param name="NoOfBits"></param>
    ''' <returns></returns>
    ''' <remarks>Alex Etchells, 2003, http://www.developerfusion.com/code/5430/convert-decimal-integer-values-to-binary-string-in-vb6/</remarks>
    Private Shared Function DecToBin(ByVal DeciValue As Long, Optional _
    ByVal NoOfBits As Integer = 8) As String

        Dim i As Integer
        'make sure there are enough bits to contain the number
        Do While DeciValue > (2 ^ NoOfBits) - 1
            NoOfBits = NoOfBits + 8
        Loop
        DecToBin = vbNullString
        'build the string
        For i = 0 To (NoOfBits - 1)
            DecToBin = CStr((DeciValue And 2 ^ i) / 2 ^ i) & DecToBin
        Next i
    End Function

    ''' <summary>
    ''' The ClearBit Sub clears the 1 based, nth bit (MyBit) of an integer (MyByte).
    ''' </summary>
    ''' <param name="MyByte"></param>
    ''' <param name="MyBit"></param>
    ''' <remarks></remarks>
    Public Shared Sub ClearBit(ByRef MyByte, ByVal MyBit)
        Dim BitMask As UInt16
        ' Create a bitmask with the 2 to the nth power bit set:
        BitMask = 2 ^ (MyBit - 1)
        ' Clear the nth Bit:
        MyByte = MyByte And Not BitMask
    End Sub

    ''' <summary>
    ''' The ExamineBit function will return True or False depending on the value of the 1 based, nth bit (MyBit) of an integer (MyByte).
    ''' </summary>
    ''' <param name="MyByte"></param>
    ''' <param name="MyBit"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function ExamineBit(ByVal MyByte, ByVal MyBit) As Boolean
        Dim BitMask As UInt16
        BitMask = 2 ^ (MyBit - 1)
        ExamineBit = ((MyByte And BitMask) > 0)
    End Function

    ''' <summary>
    ''' The SetBit Sub will set the 1 based, nth bit (MyBit) of an integer (MyByte).
    ''' </summary>
    ''' <param name="MyByte"></param>
    ''' <param name="MyBit"></param>
    ''' <remarks></remarks>
    Public Shared Sub SetBit(ByRef MyByte, ByVal MyBit)
        Dim BitMask As UInt16
        BitMask = 2 ^ (MyBit - 1)
        MyByte = MyByte Or BitMask
    End Sub

    Public Shared Sub SetBit(ByRef MyByte As Object, ByVal MyBit As Object, ByVal State As Boolean)
        If State Then
            SetBit(MyByte, MyBit)
        Else
            ClearBit(MyByte, MyBit)
        End If
    End Sub

    ''' <summary>
    ''' The ToggleBit Sub will change the state of the 1 based, nth bit (MyBit) of an integer (MyByte).
    ''' </summary>
    ''' <param name="MyByte"></param>
    ''' <param name="MyBit"></param>
    ''' <remarks></remarks>
    Public Shared Sub ToggleBit(ByRef MyByte, ByVal MyBit)
        Dim BitMask As UInt16
        BitMask = 2 ^ (MyBit - 1)
        MyByte = MyByte Xor BitMask
    End Sub

    ''' <summary>
    ''' Get the coordinates of the upper left corner of the spots on the given Spinda.
    ''' </summary>
    ''' <param name="PID"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GetPKMSpindaSpots(ByVal PID As UInt32) As Integer(,)

        Dim Coordinates(3, 1) As Integer

        Dim PIDBin As String = DecToBin(PID, 32)
        PIDBin.PadLeft(32, "0")

        Coordinates(0, 0) = _
        Convert.ToInt16(PIDBin.Substring(28, 4), 2)

        Coordinates(0, 1) = _
        Convert.ToInt16(PIDBin.Substring(24, 4), 2)

        Coordinates(1, 0) = _
        Convert.ToInt16(PIDBin.Substring(20, 4), 2) + 24

        Coordinates(1, 1) = _
        Convert.ToInt16(PIDBin.Substring(16, 4), 2) + 2

        Coordinates(2, 0) = _
        Convert.ToInt16(PIDBin.Substring(12, 4), 2) + 3

        Coordinates(2, 1) = _
        Convert.ToInt16(PIDBin.Substring(8, 4), 2) + 18

        Coordinates(3, 0) = _
        Convert.ToInt16(PIDBin.Substring(4, 4), 2) + 15

        Coordinates(3, 1) = _
        Convert.ToInt16(PIDBin.Substring(0, 4), 2) + 18

        Return Coordinates

    End Function

    ''' <summary>
    ''' Render the spots of a given Spinda sprite with a given Spinda PID.
    ''' </summary>
    ''' <param name="PID"></param>
    ''' <param name="_Shiny"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function RenderSpindaSprite(ByVal PID As UInt32, Optional ByVal _Shiny As Boolean = False) As Bitmap

        Dim BlankSprite, Spot1, Spot2, Spot3, Spot4 As Bitmap

        If _Shiny Then
            BlankSprite = My.Resources.SpindaMS
            Spot1 = My.Resources.Spinda_Spot_1_Shiny
            Spot2 = My.Resources.Spinda_Spot_2_Shiny
            Spot3 = My.Resources.Spinda_Spot_3_Shiny
            Spot4 = My.Resources.Spinda_Spot_4_Shiny
        Else
            BlankSprite = My.Resources.SpindaM
            Spot1 = My.Resources.Spinda_Spot_1
            Spot2 = My.Resources.Spinda_Spot_2
            Spot3 = My.Resources.Spinda_Spot_3
            Spot4 = My.Resources.Spinda_Spot_4
        End If

        Dim mBLACK As Color = Color.FromArgb(255, 16, 16, 16)
        Dim mLBROWN As Color = Color.FromArgb(255, 0, 0, 0)
        Dim mMBROWN As Color = Color.FromArgb(255, 115, 82, 65)
        Dim mDBROWN As Color = Color.FromArgb(255, 123, 65, 49)
        Dim mMRED As Color = Color.FromArgb(255, 238, 82, 74)
        Dim mMBEIG As Color = Color.FromArgb(255, 230, 213, 164)
        Dim mDBEIG As Color = Color.FromArgb(255, 205, 164, 115)
        Dim mPINK As Color = Color.FromArgb(255, 230, 98, 115)
        Dim mDSPOT As Color = Color.FromArgb(255, 176, 64, 40)
        Dim mGREEN As Color = Color.FromArgb(255, 162, 203, 16)
        Dim mDGREEN As Color = Color.FromArgb(255, 123, 156, 0)

        Dim NormColor As Color = BlankSprite.GetPixel(7, 37)
        Dim DarkColor As Color = BlankSprite.GetPixel(30, 45)

        Dim Coordinates(,) As Integer = GetPKMSpindaSpots(PID.ToString)

        Dim outImage As Bitmap = BlankSprite

        'SPOT 1

        For x As Integer = 0 To Spot1.Width - 1
            For y As Integer = 0 To Spot1.Height - 1

                Select Case BlankSprite.GetPixel(x + Coordinates(0, 0), y + _
                                                 Coordinates(0, 1))
                    Case mBLACK
                    Case mLBROWN
                    Case mMBROWN
                    Case mDBROWN
                    Case NormColor
                    Case mMBEIG
                        If _
                        Spot1.GetPixel(x, y) = NormColor Then _
                        outImage.SetPixel(x + Coordinates(0, 0), y + _
                                                 Coordinates(0, 1), NormColor)
                    Case mDBEIG
                        If _
                        Spot1.GetPixel(x, y) = NormColor Then _
                            outImage.SetPixel(x + Coordinates(0, 0), y + _
                                                     Coordinates(0, 1), DarkColor)
                    Case mPINK
                    Case Else
                End Select
            Next
        Next

        'SPOT 2

        For x As Integer = 0 To Spot2.Width - 1
            For y As Integer = 0 To Spot2.Height - 1

                Select Case BlankSprite.GetPixel(x + Coordinates(1, 0), y + _
                                                 Coordinates(1, 1))
                    Case mBLACK
                    Case mLBROWN
                    Case mMBROWN
                    Case mDBROWN
                    Case NormColor
                    Case mMBEIG
                        If _
                        Spot2.GetPixel(x, y) = NormColor Then _
                        outImage.SetPixel(x + Coordinates(1, 0), y + _
                                                 Coordinates(1, 1), NormColor)
                    Case mDBEIG
                        If _
                        Spot2.GetPixel(x, y) = NormColor Then _
                        outImage.SetPixel(x + Coordinates(1, 0), y + _
                                                 Coordinates(1, 1), DarkColor)
                    Case mPINK
                    Case Else
                End Select
            Next
        Next

        'SPOT 3

        For x As Integer = 0 To Spot3.Width - 1
            For y As Integer = 0 To Spot3.Height - 1

                Select Case BlankSprite.GetPixel(x + Coordinates(2, 0), y + _
                                                 Coordinates(2, 1))
                    Case mBLACK
                    Case mLBROWN
                    Case mMBROWN
                    Case mDBROWN
                    Case NormColor
                    Case mMBEIG

                        If y + Coordinates(2, 1) <= 40 Then
                            If y + Coordinates(2, 1) = 40 And x + Coordinates(2, 0) <= 17 Then
                            Else
                                If _
                                Spot3.GetPixel(x, y) = NormColor Then _
                                outImage.SetPixel(x + Coordinates(2, 0), y + _
                                                         Coordinates(2, 1), NormColor)
                            End If
                        End If

                    Case mDBEIG

                        If y + Coordinates(2, 1) <= 40 Then
                            If y + Coordinates(2, 1) = 40 And x + Coordinates(2, 0) <= 17 Then
                            Else
                                If _
                                Spot3.GetPixel(x, y) = NormColor Then _
                                outImage.SetPixel(x + Coordinates(2, 0), y + _
                                                         Coordinates(2, 1), DarkColor)
                            End If
                        End If

                    Case mPINK
                    Case Else
                End Select
            Next
        Next

        'SPOT 4

        For x As Integer = 0 To Spot3.Width - 1
            For y As Integer = 0 To Spot3.Height - 1

                Select Case BlankSprite.GetPixel(x + Coordinates(3, 0), y + _
                                                 Coordinates(3, 1))
                    Case mBLACK
                    Case mLBROWN
                    Case mMBROWN
                    Case mDBROWN
                    Case NormColor
                    Case mMBEIG

                        If y + Coordinates(3, 1) <= 40 Then
                            If y + Coordinates(3, 1) = 40 And x + Coordinates(2, 0) <= 17 Then
                            Else
                                If _
                                Spot4.GetPixel(x, y) = NormColor Then _
                                outImage.SetPixel(x + Coordinates(3, 0), y + _
                                                         Coordinates(3, 1), NormColor)
                            End If
                        End If

                    Case mDBEIG

                        If y + Coordinates(3, 1) <= 40 Then
                            If y + Coordinates(3, 1) = 40 And x + Coordinates(2, 0) <= 17 Then
                            Else
                                If _
                                Spot4.GetPixel(x, y) = NormColor Then _
                                outImage.SetPixel(x + Coordinates(3, 0), y + _
                                                         Coordinates(3, 1), DarkColor)
                            End If
                        End If

                    Case mPINK
                    Case Else
                End Select
            Next
        Next

        mBLACK = Nothing
        mLBROWN = Nothing
        mMBROWN = Nothing
        mDBROWN = Nothing
        mMRED = Nothing
        mMBEIG = Nothing
        mDBEIG = Nothing
        mPINK = Nothing
        mDSPOT = Nothing
        NormColor = Nothing
        DarkColor = Nothing

        Return outImage

    End Function

    Public Shared Function OpenSaveFile(ByVal FileName As String, Optional ByVal IgnoreErr As Boolean = False) As Object

        Dim fo As New System.IO.FileInfo(FileName)
        If fo.Length <> &H80000 Then Throw New Exception("Incorrect filesize!")
        If Not fo.Exists Then Throw New Exception("File does not exist!")

        Dim datBUF(3) As Byte
        Using fs As New FileStream(FileName, FileMode.Open, FileAccess.Read)
            fs.Seek(&H12DC, SeekOrigin.Begin)
            fs.Read(datBUF, 0, 4)
        End Using

        Dim CmpData() As Byte = New Byte() {&HFE, &HCA, &HEF, &HBE}

        Dim isPlat As Boolean = False

        For i As Integer = 0 To 3
            isPlat = Not (datBUF(i) = CmpData(i))
            If isPlat Then Exit For
        Next

        If isPlat Then
            Return OpenPtSaveFile(FileName, IgnoreErr)
        Else
            Return OpenDPSaveFile(FileName, IgnoreErr)
        End If

    End Function

    ''' <summary>
    ''' Opens and reads the given file, and returns a DPSaveFile object.
    ''' </summary>
    ''' <param name="FileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function OpenDPSaveFile(ByVal FileName As String, Optional ByVal IgnoreErr As Boolean = False) As DPSaveFile
        Dim s As New DPSaveFile
        Using fs As New FileStream(FileName, FileMode.Open, FileAccess.Read)
            Using bR As New BinaryReader(fs)
                If fs.Length = &H800A4 Then
                    fs.Seek(&HA4, SeekOrigin.Begin)
                    s = RawDeserialize(bR.ReadBytes(fs.Length - &HA4), s.GetType)
                Else
                    s = RawDeserialize(bR.ReadBytes(fs.Length), s.GetType)
                End If
                bR.Close()
                fs.Close()
            End Using
        End Using
        s.GetBlocks(True, IgnoreErr)
        Return s
    End Function

    ''' <summary>
    ''' Opens and reads the given file, and returns a PtSaveFile object.
    ''' </summary>
    ''' <param name="FileName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function OpenPtSaveFile(ByVal FileName As String, Optional ByVal IgnoreErr As Boolean = False) As PtSaveFile
        Dim s As New PtSaveFile
        Using fs As New FileStream(FileName, FileMode.Open, FileAccess.Read)
            Using bR As New BinaryReader(fs)
                If fs.Length = &H800A4 Then
                    fs.Seek(&HA4, SeekOrigin.Begin)
                    s = RawDeserialize(bR.ReadBytes(fs.Length - &HA4), s.GetType)
                Else
                    s = RawDeserialize(bR.ReadBytes(fs.Length), s.GetType)
                End If
            End Using
        End Using
        s.GetBlocks(True, IgnoreErr)
        Return s
    End Function

    Public Shared Function OpenPKMFile(ByVal FileName As String, Optional ByVal Encrypted As Boolean = False) As Pokemon
        Dim PKM As New Pokemon
        Dim _Convert As Boolean = False
        Using fs As New FileStream(FileName, FileMode.Open, FileAccess.Read)
            If fs.Length < 236 Then _Convert = True
            Using bR As New BinaryReader(fs)
                If Encrypted Then
                    PKM = RawDeserialize(DecryptPokemon(bR.ReadBytes(fs.Length)), PKM.GetType)
                Else
                    PKM = RawDeserialize(bR.ReadBytes(fs.Length), PKM.GetType)
                End If
            End Using
        End Using
        If _Convert Then _
        PKM = PC_to_Party(PKM)
        Return PKM
    End Function

    Public Shared Function PC_to_Party(ByVal PKM As Pokemon) As Pokemon
        Dim Data() As Byte = RawSerialize(PKM)
        Dim BLANKDATA(99) As Byte
        Array.Copy(BLANKDATA, 0, Data, &H88, 100)
        PKM = RawDeserialize(Data, PKM.GetType)

        Dim BS As New mBattleStats
        Dim theCalculated As New mCalculated
        Dim theCurrent As New mCurrent

        theCalculated = PKM.BattleStats.Calculated

        With PKM.BattleStats
            theCurrent.MaxHP = .Calculated.MaxHP
            theCurrent.CurrentHP = .Calculated.CurrentHP
            theCurrent.Attack = .Calculated.Attack
            theCurrent.Defense = .Calculated.Defense
            theCurrent.Speed = .Calculated.Speed
            theCurrent.SpAttack = .Calculated.SpAttack
            theCurrent.SpDefense = .Calculated.SpDefense
        End With

        BS.Calculated = theCalculated
        BS.Current = theCurrent

        PKM.BattleStats = BS

        PKM.UpdateLevel()
        Return PKM
    End Function

    Public Shared Function GetPKMImage(ByVal SpeciesID As Species, ByVal Gender As Genders, ByVal Forme As Byte) As Bitmap
        Dim pkm As New Pokemon
        pkm.Species = New mSpecies(SpeciesID)
        pkm.Gender = New mGender(Gender)
        pkm.Forme = Forme
        Return pkm.Sprite
    End Function

#End Region

#Region "Enumerations"

    ''' <summary>
    ''' The different Pokémon types; characteristics that determine, among other things, strengths and weaknesses in battle.
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum Types As Byte
        Normal
        Fighting
        Flying
        Poison
        Ground
        Rock
        Bug
        Ghost
        Steel
        CURSETYPE
        Fire
        Water
        Grass
        Electric
        Psychic
        Ice
        Dragon
        Dark
    End Enum

    Public Enum Natures As Byte
        Hardy
        Lonely
        Brave
        Adamant
        Naughty
        Bold
        Docile
        Relaxed
        Impish
        Lax
        Timid
        Hasty
        Serious
        Jolly
        Naive
        Modest
        Mild
        Quiet
        Bashful
        Rash
        Calm
        Gentle
        Sassy
        Careful
        Quirky
    End Enum

    Public Enum Genders As Byte
        Male
        Female
        Genderless
    End Enum

    Public Enum GenderRatios As Byte
        Male = 0
        Female = 254
        Genderless = 255
    End Enum

    Public Enum Stats As Byte
        HP
        Attack
        Defense
        Speed
        SpAttack
        SpDefense
    End Enum

    Public Enum ContestStats As Byte
        Cool
        Beauty
        Cute
        Smart
        Tough
        Sheen
    End Enum

    Public Enum Flavors As Byte
        Spicy
        Dry
        Sweet
        Bitter
        Sour
        [NOTHING]
    End Enum

    Public Enum PoketchApps As Byte
        Digital_Watch
        Calculator
        Memo_Pad
        Pedometer
        Pokemon_List
        Friendship_Checker
        Dowsing_Machine
        Berry_Searcher
        Day_Care_Checker
        Pokemon_History
        Counter
        Analog_Watch
        Marking_Map
        Link_Searcher
        Coin_Toss
        Move_Tester
        Calendar
        Dot_Artist
        Roulette
        Trainer_Counter
        Kitchen_Timer
        Color_Changer
        Matchup_Checker
        Stopwatch
        Alarm_Clock
    End Enum

    Public Enum Countries As Byte
        Japanese = 1
        English
        French
        Italian
        German
        Spanish = 7
        Korean
    End Enum

    Public Enum Marks As Byte
        Circle = 1
        Triangle
        Square
        Heart
        Star
        Diamond
    End Enum

    Public Enum Abilities As Byte
        [NOTHING]
        Stench
        Drizzle
        Speed_Boost
        Battle_Armor
        Sturdy
        Damp
        Limber
        Sand_Veil
        [Static]
        Volt_Absorb
        Water_Absorb
        Oblivious
        Cloud_Nine
        Compoundeyes
        Insomnia
        Color_Change
        Immunity
        Flash_Fire
        Shield_Dust
        Own_Tempo
        Suction_Cups
        Intimidate
        Shadow_Tag
        Rough_Skin
        Wonder_Guard
        Levitate
        Effect_Spore
        Synchronize
        Clear_Body
        Natural_Cure
        Lightningrod
        Serene_Grace
        Swift_Swim
        Chlorophyll
        Illuminate
        Trace
        Huge_Power
        Poison_Point
        Inner_Focus
        Magma_Armor
        Water_Veil
        Magnet_Pull
        Soundproof
        Rain_Dish
        Sand_Stream
        Pressure
        Thick_Fat
        Early_Bird
        Flame_Body
        Run_Away
        Keen_Eye
        Hyper_Cutter
        Pickup
        Truant
        Hustle
        Cute_Charm
        Plus
        Minus
        Forecast
        Sticky_Hold
        Shed_Skin
        Guts
        Marvel_Scale
        Liquid_Ooze
        Overgrow
        Blaze
        Torrent
        Swarm
        Rock_Head
        Drought
        Arena_Trap
        Vital_Spirit
        White_Smoke
        Pure_Power
        Shell_Armor
        Air_Lock
        Tangled_Feet
        Motor_Drive
        Rivalry
        Steadfast
        Snow_Cloak
        Gluttony
        Anger_Point
        Unburden
        Heatproof
        Simple
        Dry_Skin
        Download
        Iron_Fist
        Poison_Heal
        Adaptability
        Skill_Link
        Hydration
        Solar_Power
        Quick_Feet
        Normalize
        Sniper
        Magic_Guard
        No_Guard
        Stall
        Technician
        Leaf_Guard
        Klutz
        Mold_Breaker
        Super_Luck
        Aftermath
        Anticipation
        Forewarn
        Unaware
        Tinted_Lens
        Filter
        Slow_Start
        Scrappy
        Storm_Drain
        Ice_Body
        Solid_Rock
        Snow_Warning
        Honey_Gather
        Frisk
        Reckless
        Multitype
        Flower_Gift
    End Enum

    Public Enum Moves As UInt16
        [NOTHING]
        Pound
        Karate_Chop
        DoubleSlap
        Comet_Punch
        Mega_Punch
        Pay_Day
        Fire_Punch
        Ice_Punch
        ThunderPunch
        Scratch
        ViceGrip
        Guillotine
        Razor_Wind
        Swords_Dance
        Cut
        Gust
        Wing_Attack
        Whirlwind
        Fly
        Bind
        Slam
        Vine_Whip
        Stomp
        Double_Kick
        Mega_Kick
        Jump_Kick
        Rolling_Kick
        Sand_Attack
        Headbutt
        Horn_Attack
        Fury_Attack
        Horn_Drill
        Tackle
        Body_Slam
        Wrap
        Take_Down
        Thrash
        Double_Edge
        Tail_Whip
        Poison_Sting
        Twineedle
        Pin_Missile
        Leer
        Bite
        Growl
        Roar
        Sing
        Supersonic
        SonicBoom
        Disable
        Acid
        Ember
        Flamethrower
        Mist
        Water_Gun
        Hydro_Pump
        Surf
        Ice_Beam
        Blizzard
        Psybeam
        BubbleBeam
        Aurora_Beam
        Hyper_Beam
        Peck
        Drill_Peck
        Submission
        Low_Kick
        Counter
        Seismic_Toss
        Strength
        Absorb
        Mega_Drain
        Leech_Seed
        Growth
        Razor_Leaf
        SolarBeam
        PoisonPowder
        Stun_Spore
        Sleep_Powder
        Petal_Dance
        String_Shot
        Dragon_Rage
        Fire_Spin
        ThunderShock
        Thunderbolt
        Thunder_Wave
        Thunder
        Rock_Throw
        Earthquake
        Fissure
        Dig
        Toxic
        Confusion
        Psychic
        Hypnosis
        Meditate
        Agility
        Quick_Attack
        Rage
        Teleport
        Night_Shade
        Mimic
        Screech
        Double_Team
        Recover
        Harden
        Minimize
        SmokeScreen
        Confuse_Ray
        Withdraw
        Defense_Curl
        Barrier
        Light_Screen
        Haze
        Reflect
        Focus_Energy
        Bide
        Metronome
        Mirror_Move
        Selfdestruct
        Egg_Bomb
        Lick
        Smog
        Sludge
        Bone_Club
        Fire_Blast
        Waterfall
        Clamp
        Swift
        Skull_Bash
        Spike_Cannon
        Constrict
        Amnesia
        Kinesis
        Softboiled
        Hi_Jump_Kick
        Glare
        Dream_Eater
        Poison_Gas
        Barrage
        Leech_Life
        Lovely_Kiss
        Sky_Attack
        Transform
        Bubble
        Dizzy_Punch
        Spore
        Flash
        Psywave
        Splash
        Acid_Armor
        Crabhammer
        Explosion
        Fury_Swipes
        Bonemerang
        Rest
        Rock_Slide
        Hyper_Fang
        Sharpen
        Conversion
        Tri_Attack
        Super_Fang
        Slash
        Substitute
        Struggle
        Sketch
        Triple_Kick
        Thief
        Spider_Web
        Mind_Reader
        Nightmare
        Flame_Wheel
        Snore
        Curse
        Flail
        Conversion_2
        Aeroblast
        Cotton_Spore
        Reversal
        Spite
        Powder_Snow
        Protect
        Mach_Punch
        Scary_Face
        Faint_Attack
        Sweet_Kiss
        Belly_Drum
        Sludge_Bomb
        Mud_Slap
        Octazooka
        Spikes
        Zap_Cannon
        Foresight
        Destiny_Bond
        Perish_Song
        Icy_Wind
        Detect
        Bone_Rush
        Lock_On
        Outrage
        Sandstorm
        Giga_Drain
        Endure
        Charm
        Rollout
        False_Swipe
        Swagger
        Milk_Drink
        Spark
        Fury_Cutter
        Steel_Wing
        Mean_Look
        Attract
        Sleep_Talk
        Heal_Bell
        [Return]
        Present
        Frustration
        Safeguard
        Pain_Split
        Sacred_Fire
        Magnitude
        DynamicPunch
        Megahorn
        DragonBreath
        Baton_Pass
        Encore
        Pursuit
        Rapid_Spin
        Sweet_Scent
        Iron_Tail
        Metal_Claw
        Vital_Throw
        Morning_Sun
        Synthesis
        Moonlight
        Hidden_Power
        Cross_Chop
        Twister
        Rain_Dance
        Sunny_Day
        Crunch
        Mirror_Coat
        Psych_Up
        ExtremeSpeed
        AncientPower
        Shadow_Ball
        Future_Sight
        Rock_Smash
        Whirlpool
        Beat_Up
        Fake_Out
        Uproar
        Stockpile
        Spit_Up
        Swallow
        Heat_Wave
        Hail
        Torment
        Flatter
        Will_O_Wisp
        Memento
        Facade
        Focus_Punch
        SmellingSalt
        Follow_Me
        Nature_Power
        Charge
        Taunt
        Helping_Hand
        Trick
        Role_Play
        Wish
        Assist
        Ingrain
        Superpower
        Magic_Coat
        Recycle
        Revenge
        Brick_Break
        Yawn
        Knock_Off
        Endeavor
        Eruption
        Skill_Swap
        Imprison
        Refresh
        Grudge
        Snatch
        Secret_Power
        Dive
        Arm_Thrust
        Camouflage
        Tail_Glow
        Luster_Purge
        Mist_Ball
        FeatherDance
        Teeter_Dance
        Blaze_Kick
        Mud_Sport
        Ice_Ball
        Needle_Arm
        Slack_Off
        Hyper_Voice
        Poison_Fang
        Crush_Claw
        Blast_Burn
        Hydro_Cannon
        Meteor_Mash
        Astonish
        Weather_Ball
        Aromatherapy
        Fake_Tears
        Air_Cutter
        Overheat
        Odor_Sleuth
        Rock_Tomb
        Silver_Wind
        Metal_Sound
        GrassWhistle
        Tickle
        Cosmic_Power
        Water_Spout
        Signal_Beam
        Shadow_Punch
        Extrasensory
        Sky_Uppercut
        Sand_Tomb
        Sheer_Cold
        Muddy_Water
        Bullet_Seed
        Aerial_Ace
        Icicle_Spear
        Iron_Defense
        Block
        Howl
        Dragon_Claw
        Frenzy_Plant
        Bulk_Up
        Bounce
        Mud_Shot
        Poison_Tail
        Covet
        Volt_Tackle
        Magical_Leaf
        Water_Sport
        Calm_Mind
        Leaf_Blade
        Dragon_Dance
        Rock_Blast
        Shock_Wave
        Water_Pulse
        Doom_Desire
        Psycho_Boost
        Roost
        Gravity
        Miracle_Eye
        Wake_Up_Slap
        Hammer_Arm
        Gyro_Ball
        Healing_Wish
        Brine
        Natural_Gift
        Feint
        Pluck
        Tailwind
        Acupressure
        Metal_Burst
        U_turn
        Close_Combat
        Payback
        Assurance
        Embargo
        Fling
        Psycho_Shift
        Trump_Card
        Heal_Block
        Wring_Out
        Power_Trick
        Gastro_Acid
        Lucky_Chant
        Me_First
        Copycat
        Power_Swap
        Guard_Swap
        Punishment
        Last_Resort
        Worry_Seed
        Sucker_Punch
        Toxic_Spikes
        Heart_Swap
        Aqua_Ring
        Magnet_Rise
        Flare_Blitz
        Force_Palm
        Aura_Sphere
        Rock_Polish
        Poison_Jab
        Dark_Pulse
        Night_Slash
        Aqua_Tail
        Seed_Bomb
        Air_Slash
        X_Scissor
        Bug_Buzz
        Dragon_Pulse
        Dragon_Rush
        Power_Gem
        Drain_Punch
        Vacuum_Wave
        Focus_Blast
        Energy_Ball
        Brave_Bird
        Earth_Power
        Switcheroo
        Giga_Impact
        Nasty_Plot
        Bullet_Punch
        Avalanche
        Ice_Shard
        Shadow_Claw
        Thunder_Fang
        Ice_Fang
        Fire_Fang
        Shadow_Sneak
        Mud_Bomb
        Psycho_Cut
        Zen_Headbutt
        Mirror_Shot
        Flash_Cannon
        Rock_Climb
        Defog
        Trick_Room
        Draco_Meteor
        Discharge
        Lava_Plume
        Leaf_Storm
        Power_Whip
        Rock_Wrecker
        Cross_Poison
        Gunk_Shot
        Iron_Head
        Magnet_Bomb
        Stone_Edge
        Captivate
        Stealth_Rock
        Grass_Knot
        Chatter
        Judgment
        Bug_Bite
        Charge_Beam
        Wood_Hammer
        Aqua_Jet
        Attack_Order
        Defend_Order
        Heal_Order
        Head_Smash
        Double_Hit
        Roar_of_Time
        Spacial_Rend
        Lunar_Dance
        Crush_Grip
        Magma_Storm
        Dark_Void
        Seed_Flare
        Ominous_Wind
        Shadow_Force
    End Enum

    Public Enum Species As UInt16
        [NOTHING]
        Bulbasaur
        Ivysaur
        Venusaur
        Charmander
        Charmeleon
        Charizard
        Squirtle
        Wartortle
        Blastoise
        Caterpie
        Metapod
        Butterfree
        Weedle
        Kakuna
        Beedrill
        Pidgey
        Pidgeotto
        Pidgeot
        Rattata
        Raticate
        Spearow
        Fearow
        Ekans
        Arbok
        Pikachu
        Raichu
        Sandshrew
        Sandslash
        Nidoran_F
        Nidorina
        Nidoqueen
        Nidoran_M
        Nidorino
        Nidoking
        Clefairy
        Clefable
        Vulpix
        Ninetales
        Jigglypuff
        Wigglytuff
        Zubat
        Golbat
        Oddish
        Gloom
        Vileplume
        Paras
        Parasect
        Venonat
        Venomoth
        Diglett
        Dugtrio
        Meowth
        Persian
        Psyduck
        Golduck
        Mankey
        Primeape
        Growlithe
        Arcanine
        Poliwag
        Poliwhirl
        Poliwrath
        Abra
        Kadabra
        Alakazam
        Machop
        Machoke
        Machamp
        Bellsprout
        Weepinbell
        Victreebel
        Tentacool
        Tentacruel
        Geodude
        Graveler
        Golem
        Ponyta
        Rapidash
        Slowpoke
        Slowbro
        Magnemite
        Magneton
        Farfetchd
        Doduo
        Dodrio
        Seel
        Dewgong
        Grimer
        Muk
        Shellder
        Cloyster
        Gastly
        Haunter
        Gengar
        Onix
        Drowzee
        Hypno
        Krabby
        Kingler
        Voltorb
        Electrode
        Exeggcute
        Exeggutor
        Cubone
        Marowak
        Hitmonlee
        Hitmonchan
        Lickitung
        Koffing
        Weezing
        Rhyhorn
        Rhydon
        Chansey
        Tangela
        Kangaskhan
        Horsea
        Seadra
        Goldeen
        Seaking
        Staryu
        Starmie
        Mr_Mime
        Scyther
        Jynx
        Electabuzz
        Magmar
        Pinsir
        Tauros
        Magikarp
        Gyarados
        Lapras
        Ditto
        Eevee
        Vaporeon
        Jolteon
        Flareon
        Porygon
        Omanyte
        Omastar
        Kabuto
        Kabutops
        Aerodactyl
        Snorlax
        Articuno
        Zapdos
        Moltres
        Dratini
        Dragonair
        Dragonite
        Mewtwo
        Mew
        Chikorita
        Bayleef
        Meganium
        Cyndaquil
        Quilava
        Typhlosion
        Totodile
        Croconaw
        Feraligatr
        Sentret
        Furret
        Hoothoot
        Noctowl
        Ledyba
        Ledian
        Spinarak
        Ariados
        Crobat
        Chinchou
        Lanturn
        Pichu
        Cleffa
        Igglybuff
        Togepi
        Togetic
        Natu
        Xatu
        Mareep
        Flaaffy
        Ampharos
        Bellossom
        Marill
        Azumarill
        Sudowoodo
        Politoed
        Hoppip
        Skiploom
        Jumpluff
        Aipom
        Sunkern
        Sunflora
        Yanma
        Wooper
        Quagsire
        Espeon
        Umbreon
        Murkrow
        Slowking
        Misdreavus
        Unown
        Wobbuffet
        Girafarig
        Pineco
        Forretress
        Dunsparce
        Gligar
        Steelix
        Snubbull
        Granbull
        Qwilfish
        Scizor
        Shuckle
        Heracross
        Sneasel
        Teddiursa
        Ursaring
        Slugma
        Magcargo
        Swinub
        Piloswine
        Corsola
        Remoraid
        Octillery
        Delibird
        Mantine
        Skarmory
        Houndour
        Houndoom
        Kingdra
        Phanpy
        Donphan
        Porygon2
        Stantler
        Smeargle
        Tyrogue
        Hitmontop
        Smoochum
        Elekid
        Magby
        Miltank
        Blissey
        Raikou
        Entei
        Suicune
        Larvitar
        Pupitar
        Tyranitar
        Lugia
        Ho_Oh
        Celebi
        Treecko
        Grovyle
        Sceptile
        Torchic
        Combusken
        Blaziken
        Mudkip
        Marshtomp
        Swampert
        Poochyena
        Mightyena
        Zigzagoon
        Linoone
        Wurmple
        Silcoon
        Beautifly
        Cascoon
        Dustox
        Lotad
        Lombre
        Ludicolo
        Seedot
        Nuzleaf
        Shiftry
        Taillow
        Swellow
        Wingull
        Pelipper
        Ralts
        Kirlia
        Gardevoir
        Surskit
        Masquerain
        Shroomish
        Breloom
        Slakoth
        Vigoroth
        Slaking
        Nincada
        Ninjask
        Shedinja
        Whismur
        Loudred
        Exploud
        Makuhita
        Hariyama
        Azurill
        Nosepass
        Skitty
        Delcatty
        Sableye
        Mawile
        Aron
        Lairon
        Aggron
        Meditite
        Medicham
        Electrike
        Manectric
        Plusle
        Minun
        Volbeat
        Illumise
        Roselia
        Gulpin
        Swalot
        Carvanha
        Sharpedo
        Wailmer
        Wailord
        Numel
        Camerupt
        Torkoal
        Spoink
        Grumpig
        Spinda
        Trapinch
        Vibrava
        Flygon
        Cacnea
        Cacturne
        Swablu
        Altaria
        Zangoose
        Seviper
        Lunatone
        Solrock
        Barboach
        Whiscash
        Corphish
        Crawdaunt
        Baltoy
        Claydol
        Lileep
        Cradily
        Anorith
        Armaldo
        Feebas
        Milotic
        Castform
        Kecleon
        Shuppet
        Banette
        Duskull
        Dusclops
        Tropius
        Chimecho
        Absol
        Wynaut
        Snorunt
        Glalie
        Spheal
        Sealeo
        Walrein
        Clamperl
        Huntail
        Gorebyss
        Relicanth
        Luvdisc
        Bagon
        Shelgon
        Salamence
        Beldum
        Metang
        Metagross
        Regirock
        Regice
        Registeel
        Latias
        Latios
        Kyogre
        Groudon
        Rayquaza
        Jirachi
        Deoxys
        Turtwig
        Grotle
        Torterra
        Chimchar
        Monferno
        Infernape
        Piplup
        Prinplup
        Empoleon
        Starly
        Staravia
        Staraptor
        Bidoof
        Bibarel
        Kricketot
        Kricketune
        Shinx
        Luxio
        Luxray
        Budew
        Roserade
        Cranidos
        Rampardos
        Shieldon
        Bastiodon
        Burmy
        Wormadam
        Mothim
        Combee
        Vespiquen
        Pachirisu
        Buizel
        Floatzel
        Cherubi
        Cherrim
        Shellos
        Gastrodon
        Ambipom
        Drifloon
        Drifblim
        Buneary
        Lopunny
        Mismagius
        Honchkrow
        Glameow
        Purugly
        Chingling
        Stunky
        Skuntank
        Bronzor
        Bronzong
        Bonsly
        Mime_Jr
        Happiny
        Chatot
        Spiritomb
        Gible
        Gabite
        Garchomp
        Munchlax
        Riolu
        Lucario
        Hippopotas
        Hippowdon
        Skorupi
        Drapion
        Croagunk
        Toxicroak
        Carnivine
        Finneon
        Lumineon
        Mantyke
        Snover
        Abomasnow
        Weavile
        Magnezone
        Lickilicky
        Rhyperior
        Tangrowth
        Electivire
        Magmortar
        Togekiss
        Yanmega
        Leafeon
        Glaceon
        Gliscor
        Mamoswine
        Porygon_Z
        Gallade
        Probopass
        Dusknoir
        Froslass
        Rotom
        Uxie
        Mesprit
        Azelf
        Dialga
        Palkia
        Heatran
        Regigigas
        Giratina
        Cresselia
        Phione
        Manaphy
        Darkrai
        Shaymin
        Arceus
    End Enum

    Public Enum Items As UInt16
        [NOTHING]
        Master_Ball
        Ultra_Ball
        Great_Ball
        Poke_Ball
        Safari_Ball
        Net_Ball
        Dive_Ball
        Nest_Ball
        Repeat_Ball
        Timer_Ball
        Luxury_Ball
        Premier_Ball
        Dusk_Ball
        Heal_Ball
        Quick_Ball
        Cherish_Ball
        Potion
        Antidote
        Burn_Heal
        Ice_Heal
        Awakening
        Parlyz_Heal
        Full_Restore
        Max_Potion
        Hyper_Potion
        Super_Potion
        Full_Heal
        Revive
        Max_Revive
        Fresh_Water
        Soda_Pop
        Lemonade
        Moomoo_Milk
        EnergyPowder
        Energy_Root
        Heal_Powder
        Revival_Herb
        Ether
        Max_Ether
        Elixir
        Max_Elixir
        Lava_Cookie
        Berry_Juice
        Sacred_Ash
        HP_Up
        Protein
        Iron
        Carbos
        Calcium
        Rare_Candy
        PP_Up
        Zinc
        PP_Max
        Old_Gateau
        Guard_Spec
        Dire_Hit
        X_Attack
        X_Defend
        X_Speed
        X_Accuracy
        X_Special
        X_Sp_Def
        Poke_Doll
        Fluffy_Tail
        Blue_Flute
        Yellow_Flute
        Red_Flute
        Black_Flute
        White_Flute
        Shoal_Salt
        Shoal_Shell
        Red_Shard
        Blue_Shard
        Yellow_Shard
        Green_Shard
        Super_Repel
        Max_Repel
        Escape_Rope
        Repel
        Sun_Stone
        Moon_Stone
        Fire_Stone
        Thunderstone
        Water_Stone
        Leaf_Stone
        TinyMushroom
        Big_Mushroom
        Pearl
        Big_Pearl
        Stardust
        Star_Piece
        Nugget
        Heart_Scale
        Honey
        Growth_Mulch
        Damp_Mulch
        Stable_Mulch
        Gooey_Mulch
        Root_Fossil
        Claw_Fossil
        Helix_Fossil
        Dome_Fossil
        Old_Amber
        Armor_Fossil
        Skull_Fossil
        Rare_Bone
        Shiny_Stone
        Dusk_Stone
        Dawn_Stone
        Oval_Stone
        Odd_Keystone
        Griseous_Orb
        Adamant_Orb = 135US
        Lustrous_Orb
        Grass_Mail
        Flame_Mail
        Bubble_Mail
        Bloom_Mail
        Tunnel_Mail
        Steel_Mail
        Heart_Mail
        Snow_Mail
        Space_Mail
        Air_Mail
        Mosaic_Mail
        Brick_Mail
        Cheri_Berry
        Chesto_Berry
        Pecha_Berry
        Rawst_Berry
        Aspear_Berry
        Leppa_Berry
        Oran_Berry
        Persim_Berry
        Lum_Berry
        Sitrus_Berry
        Figy_Berry
        Wiki_Berry
        Mago_Berry
        Aguav_Berry
        Iapapa_Berry
        Razz_Berry
        Bluk_Berry
        Nanab_Berry
        Wepear_Berry
        Pinap_Berry
        Pomeg_Berry
        Kelpsy_Berry
        Qualot_Berry
        Hondew_Berry
        Grepa_Berry
        Tamato_Berry
        Cornn_Berry
        Magost_Berry
        Rabuta_Berry
        Nomel_Berry
        Spelon_Berry
        Pamtre_Berry
        Watmel_Berry
        Durin_Berry
        Belue_Berry
        Occa_Berry
        Passho_Berry
        Wacan_Berry
        Rindo_Berry
        Yache_Berry
        Chople_Berry
        Kebia_Berry
        Shuca_Berry
        Coba_Berry
        Payapa_Berry
        Tanga_Berry
        Charti_Berry
        Kasib_Berry
        Haban_Berry
        Colbur_Berry
        Babiri_Berry
        Chilan_Berry
        Liechi_Berry
        Ganlon_Berry
        Salac_Berry
        Petaya_Berry
        Apicot_Berry
        Lansat_Berry
        Starf_Berry
        Enigma_Berry
        Micle_Berry
        Custap_Berry
        Jaboca_Berry
        Rowap_Berry
        BrightPowder
        White_Herb
        Macho_Brace
        Exp_Share
        Quick_Claw
        Soothe_Bell
        Mental_Herb
        Choice_Band
        Kings_Rock
        SilverPowder
        Amulet_Coin
        Cleanse_Tag
        Soul_Dew
        DeepSeaTooth
        DeepSeaScale
        Smoke_Ball
        Everstone
        Focus_Band
        Lucky_Egg
        Scope_Lens
        Metal_Coat
        Leftovers
        Dragon_Scale
        Light_Ball
        Soft_Sand
        Hard_Stone
        Miracle_Seed
        BlackGlasses
        Black_Belt
        Magnet
        Mystic_Water
        Sharp_Beak
        Poison_Barb
        NeverMeltIce
        Spell_Tag
        TwistedSpoon
        Charcoal
        Dragon_Fang
        Silk_Scarf
        Up_Grade
        Shell_Bell
        Sea_Incense
        Lax_Incense
        Lucky_Punch
        Metal_Powder
        Thick_Club
        Stick
        Red_Scarf
        Blue_Scarf
        Pink_Scarf
        Green_Scarf
        Yellow_Scarf
        Wide_Lens
        Muscle_Band
        Wise_Glasses
        Expert_Belt
        Light_Clay
        Life_Orb
        Power_Herb
        Toxic_Orb
        Flame_Orb
        Quick_Powder
        Focus_Sash
        Zoom_Lens
        Metronome
        Iron_Ball
        Lagging_Tail
        Destiny_Knot
        Black_Sludge
        Icy_Rock
        Smooth_Rock
        Heat_Rock
        Damp_Rock
        Grip_Claw
        Choice_Scarf
        Sticky_Barb
        Power_Bracer
        Power_Belt
        Power_Lens
        Power_Band
        Power_Anklet
        Power_Weight
        Shed_Shell
        Big_Root
        Choice_Specs
        Flame_Plate
        Splash_Plate
        Zap_Plate
        Meadow_Plate
        Icicle_Plate
        Fist_Plate
        Toxic_Plate
        Earth_Plate
        Sky_Plate
        Mind_Plate
        Insect_Plate
        Stone_Plate
        Spooky_Plate
        Draco_Plate
        Dread_Plate
        Iron_Plate
        Odd_Incense
        Rock_Incense
        Full_Incense
        Wave_Incense
        Rose_Incense
        Luck_Incense
        Pure_Incense
        Protector
        Electirizer
        Magmarizer
        Dubious_Disc
        Reaper_Cloth
        Razor_Claw
        Razor_Fang
        TM01
        TM02
        TM03
        TM04
        TM05
        TM06
        TM07
        TM08
        TM09
        TM10
        TM11
        TM12
        TM13
        TM14
        TM15
        TM16
        TM17
        TM18
        TM19
        TM20
        TM21
        TM22
        TM23
        TM24
        TM25
        TM26
        TM27
        TM28
        TM29
        TM30
        TM31
        TM32
        TM33
        TM34
        TM35
        TM36
        TM37
        TM38
        TM39
        TM40
        TM41
        TM42
        TM43
        TM44
        TM45
        TM46
        TM47
        TM48
        TM49
        TM50
        TM51
        TM52
        TM53
        TM54
        TM55
        TM56
        TM57
        TM58
        TM59
        TM60
        TM61
        TM62
        TM63
        TM64
        TM65
        TM66
        TM67
        TM68
        TM69
        TM70
        TM71
        TM72
        TM73
        TM74
        TM75
        TM76
        TM77
        TM78
        TM79
        TM80
        TM81
        TM82
        TM83
        TM84
        TM85
        TM86
        TM87
        TM88
        TM89
        TM90
        TM91
        TM92
        HM01
        HM02
        HM03
        HM04
        HM05
        HM06
        HM07
        HM08
        Explorer_Kit
        Loot_Sack
        Rule_Book
        Poke_Radar
        Point_Card
        Journal
        Seal_Case
        Fashion_Case
        Seal_Bag
        Pal_Pad
        Works_Key
        Old_Charm
        Galactic_Key
        Red_Chain
        Town_Map
        Vs_Seeker
        Coin_Case
        Old_Rod
        Good_Rod
        Super_Rod
        Sprayduck
        Poffin_Case
        Bicycle
        Suite_Key
        Oaks_Letter
        Lunar_Wing
        Member_Card
        Azure_Flute
        SS_Ticket
        Contest_Pass
        Magma_Stone
        Parcel
        Coupon_1
        Coupon_2
        Coupon_3
        Storage_Key
        SecretPotion
        Vs_Recorder
        Gracidea
        Secret_Key
    End Enum

    Public Enum Balls As Byte
        [NOTHING]
        Master_Ball
        Ultra_Ball
        Great_Ball
        Poke_Ball
        Safari_Ball
        Net_Ball
        Dive_Ball
        Nest_Ball
        Repeat_Ball
        Timer_Ball
        Luxury_Ball
        Premier_Ball
        Dusk_Ball
        Heal_Ball
        Quick_Ball
        Cherish_Ball
    End Enum

    Public Enum DSLocations As UInt16
        _Mystery_Zone
        Twinleaf_Town
        Sandgem_Town
        Floaroma_Town
        Solaceon_Town
        Celestic_Town
        Jubilife_City
        Canalave_City
        Oreburgh_City
        Eterna_City
        Hearthome_City
        Pastoria_City
        Veilstone_City
        Sunyshore_City
        Snowpoint_City
        Pokemon_League
        Route_201
        Route_202
        Route_203
        Route_204
        Route_205
        Route_206
        Route_207
        Route_208
        Route_209
        Route_210
        Route_211
        Route_212
        Route_213
        Route_214
        Route_215
        Route_216
        Route_217
        Route_218
        Route_219
        Route_220
        Route_221
        Route_222
        Route_223
        Route_224
        Route_225
        Route_226
        Route_227
        Route_228
        Route_229
        Route_230
        Oreburgh_Mine
        Valley_Windworks
        Eterna_Forest
        Fuego_Ironworks
        Mt_Coronet
        Spear_Pillar
        Great_Marsh
        Solaceon_Ruins
        Victory_Road
        Pal_Park
        Amity_Square
        Ravaged_Path
        Floaroma_Meadow
        Oreburgh_Gate
        Fullmoon_Island
        Sendoff_Spring
        Turnback_Cave
        Flower_Paradise
        Snowpoint_Temple
        Wayward_Cave
        Ruin_Maniac_Cave
        Maniac_Tunnel
        Trophy_Garden
        Iron_Island
        Old_Chateau
        Galactic_HQ
        Verity_Lakefront
        Valor_Lakefront
        Acuity_Lakefront
        Spring_Path
        Lake_Verity
        Lake_Valor
        Lake_Acuity
        Newmoon_Island
        Battle_Tower
        Fight_Area
        Survival_Area
        Resort_Area
        Stark_Mountain
        Seabreak_Path
        Hall_of_Origin
        Verity_Cavern
        Valor_Cavern
        Acuity_Cavern
        Jubilife_TV
        Poketch_Co
        GTS
        Trainers_School
        Mining_Museum
        Flower_Shop
        Cycle_Shop
        Contest_Hall
        Poffin_House
        Foreign_Building
        Pokemon_Day_Care
        Veilstone_Store
        Game_Corner
        Canalave_Library
        Vista_Lighthouse
        Sunyshore_Market
        Pokemon_Mansion
        Footstep_House
        Cafe
        Grand_Lake
        Restaurant
        Battle_Park
        Battle_Frontier
        Battle_Factory
        Battle_Castle
        Battle_Arcade
        Battle_Hall
        Distortion_World
        Global_Terminal
        Villa
        Battleground
        ROTOMs_Room
        TG_Eterna_Bldg
        Iron_Ruins
        Iceburg_Ruins
        Rock_Peak_Ruins
        Day_Care_Couple = 2000US
        Link_trade_Arrived
        Link_trade_Met
        Kanto
        Johto
        Hoenn
        Sinnoh
        Distant_land = 2008US
        Traveling_Man
        Riley
        Hall_of_Fame
        Mystery_Zone
        Lovely_place = 3000US
        Pokemon_Ranger
        Faraway_place
        Pokemon_Movie
        Pokemon_Movie_06
        Pokemon_Movie_07
        Pokemon_Movie_08
        Pokemon_Movie_09
        Pokemon_Movie_10
        Pokemon_Movie_11
        Pokemon_Movie_12
        Pokemon_Movie_13
        Pokemon_Movie_14
        Pokemon_Movie_15
        Pokemon_Movie_16
        Pokemon_Cartoon
        Space_World
        Space_World_06
        Space_World_07
        Space_World_08
        Space_World_09
        Space_World_10
        Space_World_11
        Space_World_12
        Space_World_13
        Space_World_14
        Space_World_15
        Space_World_16
        Pokemon_Festa
        Pokemon_Festa_06
        Pokemon_Festa_07
        Pokemon_Festa_08
        Pokemon_Festa_09
        Pokemon_Festa_10
        Pokemon_Festa_11
        Pokemon_Festa_12
        Pokemon_Festa_13
        Pokemon_Festa_14
        Pokemon_Festa_15
        Pokemon_Festa_16
        PokePARK
        PokePARK_06
        PokePARK_07
        PokePARK_08
        PokePARK_09
        PokePARK_10
        PokePARK_11
        PokePARK_12
        PokePARK_13
        PokePARK_14
        PokePARK_15
        PokePARK_16
        Pokemon_Center
        PC_Tokyo
        PC_Osaka
        PC_Fukuoka
        PC_Nagoya
        PC_Sapporo
        PC_Yokohama
        Nintendo_World
        Pokemon_Event
        Pokemon_Event_06
        Pokemon_Event_07
        Pokemon_Event_08
        Pokemon_Event_09
        Pokemon_Event_10
        Pokemon_Event_11
        Pokemon_Event_12
        Pokemon_Event_13
        Pokemon_Event_14
        Pokemon_Event_15
        Pokemon_Event_16
        Wi_Fi_Event
        Wi_Fi_Gift
        Pokemon_Fan_Club
        Event_Site
        Concert_Event
    End Enum

    Public Enum Hometowns As Byte
        Unknown
        Sapphire
        Ruby
        Emerald
        FireRed
        LeafGreen
        Gold = 7
        Silver
        Diamond = 10
        Pearl
        Platinum
        Colosseum_XD = 15
    End Enum

    Public Enum Avatars As Byte
        [NOTHING]
        School_Kid = &H3
        Bug_Catcher = &H5
        Lass = &H6
        Battle_Girl = &H7
        Ace_Trainer_M = &HB
        Beauty = &HD
        Ace_Trainer_F = &HE
        Roughneck = &H1F
        Pop_Idol = &H23
        Socialite = &H25
        Cowgirl = &H2A
        Ruin_Maniac = &H32
        Black_Belt = &H33
        Rich_Boy = &H3E
        Lady = &H3F
        Psychic = &H46
    End Enum

    Public Enum Directions As Byte
        Up
        Down
        Left
        Right
    End Enum

    Public Enum DSColors As Byte
        Red
        Blue
        Yellow
        Green
        Black
        Brown
        Purple
        Gray
        White
        Pink
    End Enum

    Public Enum BattleCategories As Byte
        Physical
        Special
        Other
    End Enum

    Public Enum PCBoxes As Byte
        Box1
        Box2
        Box3
        Box4
        Box5
        Box6
        Box7
        Box8
        Box9
        Box10
        Box11
        Box12
        Box13
        Box14
        Box15
        Box16
        Box17
        Box18
    End Enum

    Public Enum PCSlots As Byte
        Slot1
        Slot2
        Slot3
        Slot4
        Slot5
        Slot6
        Slot7
        Slot8
        Slot9
        Slot10
        Slot11
        Slot12
        Slot13
        Slot14
        Slot15
        Slot16
        Slot17
        Slot18
        Slot19
        Slot20
        Slot21
        Slot22
        Slot23
        Slot24
        Slot25
        Slot26
        Slot27
        Slot28
        Slot29
        Slot30
    End Enum

    Public Enum PartySlots As Byte
        Slot1
        Slot2
        Slot3
        Slot4
        Slot5
        Slot6
    End Enum

    Public Enum ArceusFormes As Byte
        Normal
        Fist
        Sky
        Toxic
        Earth
        Stone
        Insect
        Spooky
        Iron
        Curse
        Flame
        Splash
        Meadow
        Zap
        Mind
        Icicle
        Draco
        Dread
    End Enum

    Public Enum ShayminFormes As Byte
        Land
        Sky
    End Enum

    Public Enum GiratinaFormes As Byte
        Altered
        Origin
    End Enum

    Public Enum RotomFormes As Byte
        Normal
        Heat
        Cut
        Wash
        Frost
        Spin
    End Enum

    Public Enum ShellosGastrodonFormes As Byte
        West
        East
    End Enum

    Public Enum BurmyWormadamFormes As Byte
        Plant
        Sandy
        Trash
    End Enum

    Public Enum DeoxysFormes As Byte
        Normal
        Attack
        Defense
        Speed
    End Enum

    Public Enum UnownFormes As Byte
        A
        B
        C
        D
        E
        F
        G
        H
        I
        J
        K
        L
        M
        N
        O
        P
        Q
        R
        S
        T
        U
        V
        W
        X
        Y
        Z
        Exclamation
        Question
    End Enum

    Public Enum DSBadges As Byte
        Coal_Badge
        Forest_Badge
        Cobble_Badge
        Fen_Badge
        Relic_Badge
        Mine_Badge
        Icicle_Badge
        Beacon_Badge
    End Enum

    Public Enum PKM_Size_Formats As Byte
        PC_Storage = 136
        Party = 236
    End Enum

    Public Enum Encounters As Byte
        Egg_PalPark_Event_HoneyTree_Shaymin
        TallGrass_Darkrai = &H2
        Dialga_Palkia = &H4
        Cave_HallofOrigin_Giratina = &H5
        Water = &H7
        Building = &H9
        DistortionWorld = &H17
        Starter_BebeEevee_Fossil_PT = &H18
        GreatMarsh_SafariZone = &HA
        Starter_Fossil_DP = &HC
    End Enum

    Public Enum StatusAilments As Byte
        Sleep = 1
        Poison = 4
        Burn
        Freeze
        Paralysis
        Toxic
    End Enum

#End Region

#Region "Subclasses"

    Class Badge
        Public Name As String
        Public Obtained As Boolean
        Public Shine As Byte
        Public Sub New()
            Name = ""
            Obtained = False
            Shine = 0
        End Sub
    End Class

    Class _pktchPKMHistory
        Public Enabled As Boolean
        Public Index() As UInt16
        Public Name() As String
        Public Sprite() As Bitmap
        Public Sub New()
            Enabled = False
            Index = Nothing
            Name = Nothing
            Sprite = Nothing
        End Sub
        Public Sub New(ByVal DATA() As Byte)
            InitializeDictionaries()
            ReDim Index(11)
            ReDim Name(11)
            ReDim Sprite(11)
            Dim Formes(11) As UInt16
            If DATA.Length = 48 Then
                For i As Integer = 0 To DATA.Length - 1 Step 4
                    Index(i / 4) = BitConverter.ToUInt16(DATA, i)
                    Formes(i / 4) = BitConverter.ToUInt16(DATA, i + 2)
                Next
            Else
                For i As Integer = 0 To DATA.Length - 1 Step 8
                    Index(i / 8) = BitConverter.ToUInt16(DATA, i)
                    Formes(i / 8) = BitConverter.ToUInt16(DATA, i + 2)
                Next
            End If
            For i As Integer = 0 To 11
                Name(i) = dPKMSpecies(Index(i))
                'If IsEgg Then
                '    If mSpeciesID = 490 Then
                '        Return My.Resources.BoxManaphyEgg
                '    Else
                '        Return My.Resources.BoxEgg
                '    End If
                'Else
                Select Case Index(i)
                    Case 201
                        Sprite(i) = dpUnownBoxIcons(Formes(i))
                    Case 386
                        Sprite(i) = dpDeoxysBoxIcons(Formes(i))
                    Case 412
                        Sprite(i) = dpBurmyBoxIcons(Formes(i))
                    Case 413
                        Sprite(i) = dpWormadamBoxIcons(Formes(i))
                    Case 422
                        Sprite(i) = dpShellosBoxIcons(Formes(i))
                    Case 423
                        Sprite(i) = dpGastrodonBoxIcons(Formes(i))
                    Case 479
                        Sprite(i) = dpRotomBoxIcons(Formes(i))
                    Case 487
                        Sprite(i) = dpGiratinaBoxIcons(Formes(i))
                    Case 492
                        Sprite(i) = dpShayminBoxIcons(Formes(i))
                    Case Else
                        Sprite(i) = dpBoxIcons(Index(i))
                End Select
                'End If
            Next
        End Sub
    End Class

    Class _pktchTHistory
        Public Enabled As Boolean
        Public Index() As UInt16
        Public Name() As String
        Public Sprite() As Bitmap
        Public HighChain() As UInt16
        Public Sub New()
            Enabled = False
            Index = Nothing
            Name = Nothing
            Sprite = Nothing
            HighChain = Nothing
        End Sub
        Public Sub New(ByVal DATA() As Byte)
            InitializeDictionaries()
            ReDim Index(2)
            ReDim Name(2)
            ReDim Sprite(2)
            ReDim HighChain(2)
            For i As Integer = 0 To 11 Step 4

                Index(i / 4) = BitConverter.ToUInt16(DATA, i)
                Name(i / 4) = dPKMSpecies(BitConverter.ToUInt16(DATA, i))
                Sprite(i / 4) = dpBoxIcons(BitConverter.ToUInt16(DATA, i))
                HighChain(i / 4) = BitConverter.ToUInt16(DATA, i + 2)

            Next
        End Sub
    End Class

    Class _pktchStepCounter
        Public Steps As UInt32
        Public Enabled As Boolean
        Public Sub New()
            Enabled = False
            Steps = 0UI
        End Sub
        Public Sub New(ByVal DATA As UInt32)
            Steps = DATA
        End Sub
    End Class

    Class mPoketch
        'TODO: Poketch
        Public ActiveAppValue As PoketchApps
        Public ActiveAppName As String '[Enum].GetName(GetType(PokemonLib._PKTCHApps), ActiveAppValue)
        Public Enabled As Boolean
        Public Obtained As Boolean
        Public PokemonHistory As _pktchPKMHistory
        Public TrainerHistory As _pktchTHistory
        Public StepCounter As _pktchStepCounter
        Public Sub New()
            ActiveAppValue = New PoketchApps
            ActiveAppName = String.Empty
            PokemonHistory = New _pktchPKMHistory
            TrainerHistory = New _pktchTHistory
            StepCounter = New _pktchStepCounter
        End Sub
    End Class

    ''' <summary>
    ''' A single marking.
    ''' </summary>
    ''' <remarks></remarks>
    Class _mMarking
        Public Name As String
        Public Value As Boolean
        Public Image As Bitmap
        Public Sub New()
            Name = String.Empty
            Value = False
            Image = Nothing
        End Sub
    End Class

    ''' <summary>
    ''' A group of markings.
    ''' </summary>
    ''' <remarks></remarks>
    Class _mMarkings
        Public Circle As _mMarking
        Public Triangle As _mMarking
        Public Square As _mMarking
        Public Heart As _mMarking
        Public Star As _mMarking
        Public Diamond As _mMarking
        Public Sub New()
            Circle = New _mMarking
            Triangle = New _mMarking
            Square = New _mMarking
            Heart = New _mMarking
            Star = New _mMarking
            Diamond = New _mMarking
        End Sub
    End Class

    Class mRibbons
        Public Names As List(Of String)
        Public Images As List(Of Bitmap)
        Public Sub New()
            Names = New List(Of String)
            Images = New List(Of Bitmap)
        End Sub
    End Class

    Class mAbility
        Public Value As Abilities
        Public Name As String
        Public Description As String
        Public Sub New()
            Value = 0US
            Name = String.Empty
            Description = String.Empty
        End Sub
        Public Sub New(ByVal index As Abilities)
            Value = index
            Name = dpAbilities(index)(0)
            Description = dpAbilities(index)(1)
        End Sub
    End Class

    Class mBattleCategory
        Public Value As BattleCategories
        Public Name As String
        Public Image As Bitmap
        Public Sub New()
            Value = 0
            Name = String.Empty
            Image = Nothing
        End Sub
        Public Sub New(ByVal index As BattleCategories)
            Value = index
            Select Case index
                Case BattleCategories.Physical
                    Name = "Physical"
                    Image = My.Resources.physical
                Case BattleCategories.Special
                    Name = "Special"
                    Image = My.Resources.special
                Case BattleCategories.Other
                    Name = "Other"
                    Image = My.Resources.other
            End Select
            Image.MakeTransparent(Color.Black)
        End Sub
    End Class

    Class mContestCategory
        Public Value As ContestStats
        Public Name As String
        Public Image As Bitmap
        Public Sub New()
            Value = 0
            Name = String.Empty
            Image = Nothing
        End Sub
        Public Sub New(ByVal index As ContestStats)
            Value = index
            Name = [Enum].GetName(GetType(PokemonLib.ContestStats), index)
            Select Case index
                Case ContestStats.Cool
                    Image = My.Resources.cool
                Case ContestStats.Beauty
                    Image = My.Resources.beauty
                Case ContestStats.Cute
                    Image = My.Resources.cute
                Case ContestStats.Smart
                    Image = My.Resources.smart
                Case ContestStats.Tough
                    Image = My.Resources.tough
            End Select
            Image.MakeTransparent(Color.Black)
        End Sub
    End Class

    Class mType
        Public Name As String
        Public Value As Types
        Public Image As Bitmap
        Public Sub New()
            Value = 0
            Name = String.Empty
            Image = Nothing
        End Sub
        Public Sub New(ByVal index As Types)
            Value = index
            Name = [Enum].GetName(GetType(PokemonLib.Types), index)
            Image = dpTypeIcons(index)
            Image.MakeTransparent(Color.Black)
        End Sub
    End Class

    Class mNatures
        Private mValue As Natures

        Public Sub New()
            mValue = 0UI
        End Sub

        Public Sub New(ByVal PID As UInt32)
            mValue = PID Mod 25
        End Sub

        Public ReadOnly Property Boost() As Stats
            Get
                Dim out As Stats = Nothing
                For i As Integer = 0 To 5
                    If dpNatures(mValue)(i) = 1.1D Then
                        out = i
                        Exit For
                    End If
                Next
                Return out
            End Get
        End Property

        Public ReadOnly Property Detract() As Stats
            Get
                Dim out As Stats = Nothing
                For i As Integer = 0 To 5
                    If dpNatures(mValue)(i) = 0.9D Then
                        out = i
                        Exit For
                    End If
                Next
                Return out
            End Get
        End Property

        Public ReadOnly Property Value() As Natures
            Get
                Return mValue
            End Get
        End Property

        Public ReadOnly Property Name() As String
            Get
                Return [Enum].GetName(GetType(PokemonLib.Natures), mValue)
            End Get
        End Property

    End Class

    Class mSpecies
        Private mID As Species

        Public Sub New()
            mID = 0
        End Sub

        Public Sub New(ByVal NationalID As Species)
            mID = NationalID
        End Sub

        Public Property ID() As Species
            Get
                Return mID
            End Get
            Set(ByVal value As Species)
                mID = value
            End Set
        End Property

        Public ReadOnly Property Name() As String
            Get
                PokemonLib.InitializeDictionaries()
                If mID < 1 Or mID > 493 Then Return ""
                Return dPKMSpecies(ID)
            End Get
        End Property

    End Class

    Class mItems
        Private mValue As Items
        Private mIsDSItem As Boolean

        Public Sub New()
            mValue = 0US
            mIsDSItem = False
        End Sub

        Public Sub New(ByVal Index As Items, Optional ByVal IsDSItem As Boolean = True)
            mValue = Index
            mIsDSItem = IsDSItem
        End Sub

        Public Property Value() As Items
            Get
                Return mValue
            End Get
            Set(ByVal value As Items)
                mValue = value
            End Set
        End Property

        Public ReadOnly Property Name() As String
            Get
                PokemonLib.InitializeDictionaries()
                If mIsDSItem Then Return dPKMItems(Value)
                Return GBAItems(Value)
            End Get
        End Property

        Public ReadOnly Property Image() As Bitmap
            Get
                Return dpItemImages(mValue)
            End Get
        End Property

    End Class

    Class mGender
        Private mName As String
        Private mValue As Genders

        Public Sub New()
            mName = String.Empty
            mValue = 0
        End Sub

        Public Sub New(ByVal Index As Genders)
            mValue = Index
        End Sub

        Public ReadOnly Property Value() As Genders
            Get
                Return mValue
            End Get
        End Property

        Public ReadOnly Property Name() As String
            Get
                Return [Enum].GetName(GetType(PokemonLib.Genders), Value)
            End Get
        End Property

        Public ReadOnly Property Image() As Bitmap
            Get
                Dim ImgOUT As Bitmap
                Select Case mValue
                    Case Genders.Male
                        ImgOUT = My.Resources.PKM_Male
                    Case Genders.Female
                        ImgOUT = My.Resources.PKM_Female
                    Case Else
                        Return Nothing
                End Select
                ImgOUT.MakeTransparent(Color.White)
                Return ImgOUT
            End Get
        End Property

    End Class

    Class mTrainer
        Private mTrName As Byte()
        Private mTrGender As mGender
        Private mTrID, mTrSID As UInt16
        Private mTrImage As Bitmap

        Public Sub New()
            'mTrName = Nothing
            ReDim mTrName(15)
            mTrGender = New mGender
            mTrID = 0US
            mTrSID = 0US
            mTrImage = Nothing
        End Sub

        Public Sub New(ByVal TrNameData As Byte(), ByVal TrGender As mGender, ByVal TrID As UInt16, ByVal TrSID As UInt16, Optional ByVal Platinum As Boolean = True)
            ReDim mTrName(15)
            If TrNameData Is Nothing Then TrNameData = New Byte() {&HFF, &HFF}
            If TrNameData.Length < 16 Then
                Array.Copy(TrNameData, 0, mTrName, 0, TrNameData.Length)
            Else
                Array.Copy(TrNameData, 0, mTrName, 0, mTrName.Length)
            End If
            mTrGender = TrGender
            mTrID = TrID
            mTrSID = TrSID
            If Platinum Then
                If mTrGender.Value = Genders.Male Then
                    mTrImage = My.Resources.PtMaleTrainer
                Else
                    mTrImage = My.Resources.PtFemaleTrainer
                End If
            Else
                If mTrGender.Value = Genders.Male Then
                    mTrImage = My.Resources.DPMaleTrainer
                Else
                    mTrImage = My.Resources.DPFemaleTrainer
                End If
            End If
        End Sub

        Public Sub New(ByVal TrName As String, ByVal TrGender As mGender, ByVal TrID As UInt16, ByVal TrSID As UInt16, Optional ByVal Platinum As Boolean = True)
            ReDim mTrName(15)
            Dim theBytes() As Byte = StringToPKMBytes(TrName, True)
            If theBytes.Length < 16 Then
                Array.Copy(theBytes, 0, mTrName, 0, theBytes.Length)
            Else
                Array.Copy(theBytes, 0, mTrName, 0, mTrName.Length)
            End If
            mTrGender = TrGender
            mTrID = TrID
            mTrSID = TrSID
            If Platinum Then
                If mTrGender.Value = Genders.Male Then
                    mTrImage = My.Resources.PtMaleTrainer
                Else
                    mTrImage = My.Resources.PtFemaleTrainer
                End If
            Else
                If mTrGender.Value = Genders.Male Then
                    mTrImage = My.Resources.DPMaleTrainer
                Else
                    mTrImage = My.Resources.DPFemaleTrainer
                End If
            End If
        End Sub

        Public Property Name() As String
            Get
                Return PokemonLib.PKMBytesToString(mTrName)
            End Get
            Set(ByVal value As String)
                If value.Length > 10 Then value = value.Substring(0, 10)
                Dim out() As Byte
                out = StringToPKMBytes(value, True)
                Array.Copy(out, mTrName, out.Length)
            End Set
        End Property

        Public Property Gender() As mGender
            Get
                Return mTrGender
            End Get
            Set(ByVal value As mGender)
                mTrGender = New mGender(value.Value)
            End Set
        End Property

        Public Property ID() As UInt16
            Get
                Return mTrID
            End Get
            Set(ByVal value As UInt16)
                mTrID = value
            End Set
        End Property

        Public Property SID() As UInt16
            Get
                Return mTrSID
            End Get
            Set(ByVal value As UInt16)
                mTrSID = value
            End Set
        End Property

        Public ReadOnly Property Image() As Bitmap
            Get
                Return mTrImage
            End Get
        End Property

    End Class

    Class mIVs
        Public HP As Byte
        Public Attack As Byte
        Public Defense As Byte
        Public Speed As Byte
        Public SpAttack As Byte
        Public SpDefense As Byte
        Public Sub New()
            HP = 0
            Attack = 0
            Defense = 0
            Speed = 0
            SpAttack = 0
            SpDefense = 0
        End Sub
        Public Sub New(ByVal _HP As Byte, ByVal _Attack As Byte, ByVal _Defense As Byte, ByVal _Speed As Byte, ByVal _SpAttack As Byte, ByVal _SpDefense As Byte)
            HP = _HP
            Attack = _Attack
            Defense = _Defense
            Speed = _Speed
            SpAttack = _SpAttack
            SpDefense = _SpDefense
        End Sub
    End Class

    Class mBattleStats
        Public Current As New mCurrent
        Public Calculated As New mCalculated
        Public Sub New()
            Current = New mCurrent
            Calculated = New mCalculated
        End Sub
    End Class

    Class mCurrent
        Public MaxHP As UInt16
        Public CurrentHP As UInt16
        Public Attack As UInt16
        Public Defense As UInt16
        Public Speed As UInt16
        Public SpAttack As UInt16
        Public SpDefense As UInt16
        Public Sub New()
            MaxHP = 0
            CurrentHP = 0
            Attack = 0
            Defense = 0
            Speed = 0
            SpAttack = 0
            SpDefense = 0
        End Sub
    End Class

    Class mCalculated
        Public MaxHP As UInt16
        Public CurrentHP As UInt16
        Public Attack As UInt16
        Public Defense As UInt16
        Public Speed As UInt16
        Public SpAttack As UInt16
        Public SpDefense As UInt16
        Public Sub New()
            MaxHP = 0
            CurrentHP = 0
            Attack = 0
            Defense = 0
            Speed = 0
            SpAttack = 0
            SpDefense = 0
        End Sub
    End Class

    Class mContestStats
        Public Cool As Byte
        Public Beauty As Byte
        Public Cute As Byte
        Public Smart As Byte
        Public Tough As Byte
        Public Sheen As Byte

        Public Sub New()
            Cool = 0
            Beauty = 0
            Cute = 0
            Smart = 0
            Tough = 0
            Sheen = 0
        End Sub

        Public Sub New(ByVal mCool As Byte, ByVal mBeauty As Byte, _
                       ByVal mCute As Byte, ByVal mSmart As Byte, _
                       ByVal mTough As Byte, ByVal mSheen As Byte)
            Cool = mCool
            Beauty = mBeauty
            Cute = mCute
            Smart = mSmart
            Tough = mTough
            Sheen = mSheen
        End Sub

    End Class

    Public Class mEncounters
        Public Value As Encounters
        Public Sub New()
            Value = 0
        End Sub
        Public Sub New(ByVal _Encounter As Encounters)
            Value = _Encounter
        End Sub
        Public ReadOnly Property Name() As String
            Get
                Name = "UNKNOWN"
                dpPKMEncounters.TryGetValue(Value, Name)
            End Get
        End Property
    End Class

    Public Class mWallpaper
        Public Name As String
        Public Value As Byte
        Public Image As Bitmap
        Public Sub New()
            Name = String.Empty
            Value = 0
            Image = Nothing
        End Sub
        Public Sub New(ByVal _Value As Byte)
            InitializeDictionaries()
            Value = _Value
            Name = dpWallpapers(_Value)
            Image = dpWallpaperImages(_Value)
        End Sub
    End Class

    <Serializable()> _
    <StructLayout(LayoutKind.Sequential)> _
    Public Class StorageBox

        Public Number As PCBoxes
        Public Name As String
        Public StoredPokemon() As Pokemon
        Public Wallpaper As mWallpaper

        Public Sub New()
            Number = 0
            Name = String.Empty
            StoredPokemon = Nothing
            Wallpaper = New mWallpaper
        End Sub

        Public Sub New(ByVal mNumber As PCBoxes, ByVal mName As String, ByVal mPKM() As Pokemon, ByVal _Wallpaper As Byte)
            Number = mNumber
            Name = mName
            StoredPokemon = mPKM
            Wallpaper = New mWallpaper(_Wallpaper)
        End Sub

        Public ReadOnly Property PokemonCount() As Byte
            Get
                PokemonCount = 0
                For Each pkm As Pokemon In StoredPokemon
                    If pkm.Species.ID > 0 And pkm.Species.ID < 494 Then PokemonCount += 1
                Next
            End Get
        End Property

        Public Sub Grid(ByVal g As Graphics, Optional ByVal Scale As Integer = 1)

            Dim IMG As Bitmap = New Bitmap(6, 5)
            Dim pkm As New Pokemon
            For i As Integer = 0 To 29
                pkm = StoredPokemon(i)
                With pkm.Species
                    If .ID > 0 And .ID < 494 Then
                        IMG.SetPixel(i Mod 6, Math.Floor(i / 6), pkm.BaseStats.Color)
                    Else
                        IMG.SetPixel(i Mod 6, Math.Floor(i / 6), Color.Transparent)
                    End If
                End With
            Next

            ' destination rectangle
            Dim rectDst As New Rectangle()

            rectDst.X = 0
            rectDst.Y = 0
            rectDst.Width = IMG.Width * Scale
            rectDst.Height = IMG.Height * Scale

            ' source rectangle
            Dim rectSrc As New Rectangle()

            rectSrc.X = 0
            rectSrc.Y = 0
            rectSrc.Width = IMG.Width
            rectSrc.Height = IMG.Height

            ' draw (part of the image)
            'Dim g As Graphics = PB.CreateGraphics 'e.Graphics
            g.InterpolationMode = InterpolationMode.NearestNeighbor
            g.PixelOffsetMode = PixelOffsetMode.Half
            g.DrawImage(IMG, rectDst, rectSrc, GraphicsUnit.Pixel)

        End Sub

    End Class

    Public Class mMap
        Public ID As Integer
        Public Name As String
        Public X As Integer
        Public Y As Integer
        Public Z As Integer

        Public Sub New()
            ID = 0
            Name = String.Empty
            X = 0
            Y = 0
            Z = 0
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _X As Integer, ByVal _Y As Integer, ByVal _Z As Integer)
            ID = _ID
            X = _X
            Y = _Y
            Z = _Z
        End Sub

    End Class

    Class mBaseStats

        Public HP, Attack, Defense, Speed, SpAttack, SpDefense, _
        CatchRate, BaseEXP, HPEffort, AtkEffort, DefEffort, _
        SpdEffort, SpAtkEffort, SpDefEffort, Gender, BaseEggSteps, _
        BaseTameness, GrowthGroup, EggGroup1, EggGroup2, _
        Ability1, Ability2 As UInt16
        Private mColor As UInt16

        Public Type() As mType

        Public Color As System.Drawing.Color

        Private DP, DPITEM5, _
        SAFARIFLAG As UInt16

        Public Sub New()
            HP = 0US
            Attack = 0US
            Defense = 0US
            Speed = 0US
            SpAttack = 0US
            SpDefense = 0US
            CatchRate = 0US
            BaseEXP = 0US
            HPEffort = 0US
            AtkEffort = 0US
            DefEffort = 0US
            SpdEffort = 0US
            SpAtkEffort = 0US
            SpDefEffort = 0US
            Gender = 0US
            BaseEggSteps = 0US
            BaseTameness = 0US
            GrowthGroup = 0US
            EggGroup1 = 0US
            EggGroup2 = 0US
            Ability1 = 0US
            Ability2 = 0US
            mColor = 0US
            Type = Nothing
            Color = Nothing
            DP = 0US
            DPITEM5 = 0US
            SAFARIFLAG = 0US
        End Sub

        Public Sub New(ByVal DexNum As Species, ByVal Forme As Byte)
            InitializeDictionaries()
            If DexNum < 1 Or DexNum > 493 Then Exit Sub
            Dim DicBase() As UInt16 = dPKMBaseStats(DexNum)

            ReDim Type(1)

            HP = DicBase(0)
            Attack = DicBase(1)
            Defense = DicBase(2)
            Speed = DicBase(3)
            SpAttack = DicBase(4)
            SpDefense = DicBase(5)
            Type(0) = New mType(DicBase(6))
            Type(1) = New mType(DicBase(7))
            CatchRate = DicBase(8)
            BaseEXP = DicBase(9)
            HPEffort = DicBase(10)
            AtkEffort = DicBase(11)
            DefEffort = DicBase(12)
            SpdEffort = DicBase(13)
            SpAtkEffort = DicBase(14)
            SpDefEffort = DicBase(15)
            Gender = DicBase(16)
            BaseEggSteps = DicBase(17)
            BaseTameness = DicBase(18)
            Select Case DicBase(19)
                Case 0
                    GrowthGroup = 2
                Case 1
                    GrowthGroup = 0
                Case 2
                    GrowthGroup = 5
                Case 3
                    GrowthGroup = 3
                Case 4
                    GrowthGroup = 1
                Case 5
                    GrowthGroup = 4
            End Select
            EggGroup1 = DicBase(20)
            EggGroup2 = DicBase(21)
            Ability1 = DicBase(22)
            Ability2 = DicBase(23)
            mColor = DicBase(24)
            Color = dpColors(mColor)

            Select Case DexNum
                Case Species.Deoxys
                    HP = dpDeoxysStats(Forme)(0)
                    Attack = dpDeoxysStats(Forme)(1)
                    Defense = dpDeoxysStats(Forme)(2)
                    Speed = dpDeoxysStats(Forme)(3)
                    SpAttack = dpDeoxysStats(Forme)(4)
                    SpDefense = dpDeoxysStats(Forme)(5)
                Case Species.Wormadam
                    HP = dpWormadamStats(Forme)(0)
                    Attack = dpWormadamStats(Forme)(1)
                    Defense = dpWormadamStats(Forme)(2)
                    Speed = dpWormadamStats(Forme)(3)
                    SpAttack = dpWormadamStats(Forme)(4)
                    SpDefense = dpWormadamStats(Forme)(5)
                    Type(0) = New mType(dpWormadamStats(Forme)(6))
                    Type(1) = New mType(dpWormadamStats(Forme)(7))
                Case Species.Rotom
                    HP = dpRotomStats(Forme)(0)
                    Attack = dpRotomStats(Forme)(1)
                    Defense = dpRotomStats(Forme)(2)
                    Speed = dpRotomStats(Forme)(3)
                    SpAttack = dpRotomStats(Forme)(4)
                    SpDefense = dpRotomStats(Forme)(5)
                Case Species.Giratina
                    HP = dpGiratinaStats(Forme)(0)
                    Attack = dpGiratinaStats(Forme)(1)
                    Defense = dpGiratinaStats(Forme)(2)
                    Speed = dpGiratinaStats(Forme)(3)
                    SpAttack = dpGiratinaStats(Forme)(4)
                    SpDefense = dpGiratinaStats(Forme)(5)
                Case Species.Shaymin
                    HP = dpShayminStats(Forme)(0)
                    Attack = dpShayminStats(Forme)(1)
                    Defense = dpShayminStats(Forme)(2)
                    Speed = dpShayminStats(Forme)(3)
                    SpAttack = dpShayminStats(Forme)(4)
                    SpDefense = dpShayminStats(Forme)(5)
                    Type(0) = New mType(dpShayminStats(Forme)(6))
                    Type(1) = New mType(dpShayminStats(Forme)(7))
                Case Species.Arceus
                    Type(0) = New mType(Forme)
                    Type(1) = Type(0)
            End Select

        End Sub

        Public Sub New(ByVal DexNum As Species)
            InitializeDictionaries()
            If DexNum < 1 Or DexNum > 493 Then Exit Sub
            Dim DicBase() As UInt16 = dPKMBaseStats(DexNum)

            ReDim Type(1)

            HP = DicBase(0)
            Attack = DicBase(1)
            Defense = DicBase(2)
            Speed = DicBase(3)
            SpAttack = DicBase(4)
            SpDefense = DicBase(5)
            Type(0) = New mType(DicBase(6))
            Type(1) = New mType(DicBase(7))
            CatchRate = DicBase(8)
            BaseEXP = DicBase(9)
            HPEffort = DicBase(10)
            AtkEffort = DicBase(11)
            DefEffort = DicBase(12)
            SpdEffort = DicBase(13)
            SpAtkEffort = DicBase(14)
            SpDefEffort = DicBase(15)
            Gender = DicBase(16)
            BaseEggSteps = DicBase(17)
            BaseTameness = DicBase(18)
            Select Case DicBase(19)
                Case 0
                    GrowthGroup = 2
                Case 1
                    GrowthGroup = 0
                Case 2
                    GrowthGroup = 5
                Case 3
                    GrowthGroup = 3
                Case 4
                    GrowthGroup = 1
                Case 5
                    GrowthGroup = 4
            End Select
            EggGroup1 = DicBase(20)
            EggGroup2 = DicBase(21)
            Ability1 = DicBase(22)
            Ability2 = DicBase(23)
            mColor = DicBase(24)
            Color = dpColors(mColor)

            If DexNum = 386 Then
                'HP = dpDeoxysStats(Forme)(0)
                'Attack = dpDeoxysStats(Forme)(1)
                'Defense = dpDeoxysStats(Forme)(2)
                'Speed = dpDeoxysStats(Forme)(3)
                'SpAttack = dpDeoxysStats(Forme)(4)
                'SpDefense = dpDeoxysStats(Forme)(5)

            End If

        End Sub

    End Class

    Class mFlavors
        Public Likes As String
        Public Dislikes As String
        Public Sub New()
            Likes = String.Empty
            Dislikes = String.Empty
        End Sub
        Public Sub New(ByVal __Nature As mNatures)
            Likes = [Enum].GetName(GetType(PokemonLib.Flavors), Convert.ToUInt16(dpNatures(__Nature.Value)(6)))
            Dislikes = [Enum].GetName(GetType(PokemonLib.Flavors), Convert.ToUInt16(dpNatures(__Nature.Value)(7)))
        End Sub
    End Class

    Class mOrigins
        Public Level As Byte
        Public Egg As mEgg
        Public Location As mMet
        Public DateMet As Date
        Public Hometown As mmHometown
        Public Country As mmCountry
        Public Sub New()
            Level = 0
            Egg = New mEgg
            Location = New mMet
            DateMet = Nothing
            Hometown = New mmHometown
            Country = New mmCountry
        End Sub
        Public Sub New(ByVal _Level As Byte, ByVal _Egg As mEgg, _
                       ByVal _Location As mMet, ByVal _DateMet As Date, _
                       ByVal _Hometown As mmHometown, ByVal _Country As mmCountry)
            Level = _Level
            Egg = _Egg
            Location = _Location
            DateMet = _DateMet
            Hometown = _Hometown
            Country = _Country
        End Sub
    End Class

    Class mmCountry
        Public Value As Countries
        Public Sub New()
            Value = 0
        End Sub
        Public Sub New(ByVal CountryID As Countries)
            Value = CountryID
        End Sub
        Public ReadOnly Property Name() As String
            Get
                Return dpCountries(Value)
            End Get
        End Property
    End Class

    Class mmHometown
        Public Value As Hometowns
        Public Sub New()
            Value = 0
        End Sub
        Public Sub New(ByVal HometownID As Hometowns)
            Value = HometownID
        End Sub
        Public ReadOnly Property Name() As String
            Get
                Return dpPKMHometowns(Value)
            End Get
        End Property
    End Class

    Class mMet
        Public DiamondPearl As mDSLocation
        Public Platinum As mDSLocation
        Public Sub New()
            DiamondPearl = New mDSLocation
            Platinum = New mDSLocation
        End Sub
        Public Sub New(ByVal DPLocation As DSLocations, ByVal PtLocation As DSLocations)
            DiamondPearl = New mDSLocation(DPLocation)
            Platinum = New mDSLocation(PtLocation)
        End Sub
    End Class

    Class mEgg
        Public EggDate As Date
        Public Location As mMet
        Public Sub New()
            EggDate = Nothing
            Location = New mMet
        End Sub
        Public Sub New(ByVal _EggDate As Date, ByVal _Location As mMet)
            EggDate = _EggDate
            Location = _Location
        End Sub
    End Class

    Class mDSLocation
        Public Value As DSLocations
        Public Sub New()
            Value = 0US
        End Sub
        Public Sub New(ByVal LocationID As DSLocations)
            Value = LocationID
        End Sub
        Public ReadOnly Property Name() As String
            Get
                Return dpLocations(Value)
            End Get
        End Property
    End Class

    Class mEVs
        Public HP As Byte
        Public Attack As Byte
        Public Defense As Byte
        Public Speed As Byte
        Public SpAttack As Byte
        Public SpDefense As Byte
        Public Sub New()
            HP = 0
            Attack = 0
            Defense = 0
            Speed = 0
            SpAttack = 0
            SpDefense = 0
        End Sub
        Public Sub New(ByVal _HP As Byte, ByVal _Attack As Byte, ByVal _
                       _Defense As Byte, ByVal _Speed As Byte, ByVal _
                       _SpAttack As Byte, ByVal _SpDefense As Byte)
            HP = _HP
            Attack = _Attack
            Defense = _Defense
            Speed = _Speed
            SpAttack = _SpAttack
            SpDefense = SpDefense
        End Sub
    End Class

    Class mStatus
        Public Asleep As Boolean
        Public SleepRounds As Byte
        Public Poisoned As Boolean
        Public Burned As Boolean
        Public Frozen As Boolean
        Public Paralyzed As Boolean
        Public Toxic As Boolean
        Public Image As Bitmap
        Public Sub New()
            Asleep = False
            SleepRounds = 0
            Poisoned = False
            Burned = False
            Frozen = False
            Paralyzed = False
            Toxic = False
            Image = Nothing
        End Sub
        Public Sub New(ByVal _Ailments As Byte)
            Dim BA As BitArray = New BitArray(New Byte() {_Ailments})
            If (BA(0) Or BA(1) Or BA(2)) Then
                Asleep = True
                Select Case True
                    Case BA(0)
                        SleepRounds += 1
                    Case BA(1)
                        SleepRounds += 2
                    Case BA(2)
                        SleepRounds += 4
                End Select
            End If
            Poisoned = BA(3)
            Burned = BA(4)
            Frozen = BA(5)
            Paralyzed = BA(6)
            Toxic = BA(7)
            Dim IMGOUT As Bitmap = Nothing
            Select Case True
                Case Asleep
                    IMGOUT = My.Resources.stSleep
                    IMGOUT.MakeTransparent(Color.Black)
                Case Poisoned Or Toxic
                    IMGOUT = My.Resources.stPoison
                    IMGOUT.MakeTransparent(Color.Black)
                Case Burned
                    IMGOUT = My.Resources.stBurn
                    IMGOUT.MakeTransparent(Color.Black)
                Case Frozen
                    IMGOUT = My.Resources.stFrozen
                    IMGOUT.MakeTransparent(Color.Black)
                Case Paralyzed
                    IMGOUT = My.Resources.stParalysis
                    IMGOUT.MakeTransparent(Color.Black)
            End Select
            Image = IMGOUT
        End Sub
    End Class

    <Serializable()> _
    <StructLayout(LayoutKind.Sequential)> _
    Public Class mPalPadEntry

        'For i As Integer = 0 To &H180 - &HC Step &HC
        '    Dim theFC As UInt32 = BitConverter.ToUInt32(DATA, i + 4)
        '    If theFC = 0 Then
        '        Exit For
        '    End If
        '    PalPadList.Add(New String() {FCM.GetFC(theFC).ToString("0000-0000-0000"), ""})
        'Next

        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H10)> _
        Private _groupNameBytes() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H10)> _
        Private _PlayerNameBytes() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H8)> _
        Private _DPData() As Byte
        Private _pkmTrades As UInt16
        Private _yearLastPlayed As UInt16
        Private _monthLastPlayed As Byte
        Private _dayLastPlayed As Byte
        Private _gender As Byte
        Private _avatar As Avatars
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H8)> _
        Private PtData() As Byte

        Private _friendCode As UInt64

        Public Sub New()
            _groupNameBytes = Nothing
            _PlayerNameBytes = Nothing
            _DPData = Nothing
            _pkmTrades = 0US
            _yearLastPlayed = 0US
            _monthLastPlayed = 0
            _dayLastPlayed = 0
            _gender = 0
            _avatar = Avatars.NOTHING
            PtData = Nothing
            _friendCode = 0UL
        End Sub

        Public Property FriendCode() As UInt64
            Get
                Return _friendCode
            End Get
            Set(ByVal value As UInt64)
                _friendCode = value
            End Set
        End Property

        Public Property GroupName() As String
            Get
                Dim strOUT As String = ""
                'strOUT = CNVRT.BytesToString(_groupNameBytes)
                strOUT = PKMBytesToString(_groupNameBytes)
                Return strOUT
            End Get
            Set(ByVal value As String)
                Dim datain() As Byte = StringToPKMBytes(value)
                Dim dataout(&H10 - 1) As Byte
                Array.Copy(datain, 0, dataout, 0, datain.Length)
                _groupNameBytes = dataout
            End Set
        End Property

        Public Property PlayerName() As String
            Get
                'Return CNVRT.BytesToString(_PlayerNameBytes)
                Return PKMBytesToString(_PlayerNameBytes)
            End Get
            Set(ByVal value As String)
                Dim datain() As Byte = StringToPKMBytes(value)
                Dim dataout(&H10 - 1) As Byte
                Array.Copy(datain, 0, dataout, 0, datain.Length)
                _PlayerNameBytes = dataout
            End Set
        End Property

    End Class

    'TODO: GBA SAVE STRUCTURE

#Region "GBA Save Files"

#Region "GBA SAVE NOTES"
    'Each block of 4KB has 12 bytes at the end of it that you can identify it by:
    '
    '0x0FF4 - byte - Block ID (0x00-0x0D)
    '0x0FF5 - byte - Unknown
    '0x0FF6 - word - Checksum (total value of first 3968 bytes of block, Modulus 0xFFFF)
    '0x0FF8 - long word - validation code 0x08012025
    '0x0FFC - long word - save ID - (highest value is most recent save)
    '
    '(this is stored in little endian so in a hex editor you'll see the bytes in reverse order - the validation code will show as 25 20 01 08 for example.)
    '
    'You have to process the blocks and rebuild the whole file before you can really start looking at the data as certain structures, such as PC boxes,
    'cross block boundaries. When you're rebuilding the file, you just take the first 3968 (0x0F80) bytes of each block and put them all together in order.
    '
    'In the rebuilt file, party pokemon are at 0x11B8 in RSE and 0x0FB8 in FrLg and box pokemon are at 0x4D84 in all versions.
#End Region

    Class mGBALocation
        Public Value As UInteger
        Public Sub New()
            Value = 0UI
        End Sub
        Public Property Name() As String
            Get
                Return GBALocations(Value)
            End Get
            Set(ByVal _value As String)
                If dpLocations.Values.Contains(_value) Then _
                Value = Array.IndexOf(GBALocations.Values.ToArray, _value)
            End Set
        End Property
    End Class

    <Serializable()> _
    <StructLayout(LayoutKind.Sequential, Size:=&HC)> _
    Public Class GBASaveFooter
        Public BlockID As Byte
        Public Unknown As Byte
        Public Checksum As UInt16
        Public Validation As UInt32
        Public SaveID As UInt32
    End Class

    <Serializable()> _
    <StructLayout(LayoutKind.Sequential, Size:=&H1000)> _
    Public Class GBASaveBlock
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HF80)> _
        Public Data() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H74)> _
        Public Data2() As Byte
        Public Footer As GBASaveFooter
    End Class

    <Serializable()> _
    <StructLayout(LayoutKind.Sequential, Size:=&H8340)> _
    Public Class GBAPCStorageSystem
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H8340)> _
        Private Data() As Byte

        Public Sub New()
            Data = Nothing
        End Sub

        Public ReadOnly Property Boxes() As List(Of GBAPCBox)
            Get
                Dim BoxesOut As New List(Of GBAPCBox)
                Dim BoxData(&H95F) As Byte
                For i As Integer = 0 To 13
                    Array.Copy(Data, (i * &H960), BoxData, 0, &H960)
                    BoxesOut.Add(RawDeserialize(BoxData, (New GBAPCBox).GetType))
                Next
                Return BoxesOut
            End Get
        End Property

    End Class

    <Serializable()> _
    <StructLayout(LayoutKind.Sequential, Size:=&H960)> _
    Public Class GBAPCBox
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H960)> _
        Private Data() As Byte

        Public Sub New()
            Data = Nothing
        End Sub

        Public ReadOnly Property StoredPokemon() As GBAPokemon()
            Get
                Dim PKMOUT(29) As GBAPokemon
                Dim thePKM(79) As Byte
                Dim PKM As New GBAPokemon
                For i As Integer = 0 To Data.Length - 1 Step 80
                    Array.Copy(Data, i, thePKM, 0, 80)
                    PKM = RawDeserialize(thePKM, PKM.GetType)
                    PKMOUT(i / 80) = PKM
                Next
                Return PKMOUT
            End Get
        End Property

    End Class

    Public Shared Function OpenRSESaveFile(ByVal FileName As String) As RSESaveFile
        OpenRSESaveFile = New RSESaveFile
        Dim Blocks As New List(Of GBASaveBlock)
        Dim Data() As Byte
        Using fs As New FileStream(FileName, FileMode.Open, FileAccess.Read)
            Using bR As New BinaryReader(fs)
                Data = bR.ReadBytes(fs.Length)
            End Using
        End Using
        Dim BlockData(&HFFF) As Byte
        Dim theBlock As New GBASaveBlock
        For i As Integer = 0 To &H1BFFF Step &H1000
            Array.Copy(Data, i, BlockData, 0, &H1000)
            theBlock = RawDeserialize(BlockData, theBlock.GetType)
            If theBlock.Footer.Validation <> &H8012025 Then Throw New Exception("Corrupt data block!")
            If Blocks.Count <> 28 Then Blocks.Add(theBlock)
        Next

        Dim CurSave As UInt32 = 0UI
        For i As Integer = 0 To Blocks.Count - 1
            If Blocks(i).Footer.SaveID > CurSave Then CurSave = Blocks(i).Footer.SaveID
        Next

        Dim BlockArray(27) As GBASaveBlock

        For Each BLK As GBASaveBlock In Blocks
            If BLK.Footer.SaveID = CurSave Then
                BlockArray(BLK.Footer.BlockID) = BLK
            Else
                BlockArray(BLK.Footer.BlockID + 14) = BLK
            End If
        Next

        Dim SavData(&H20000) As Byte

        For k As Integer = 0 To BlockArray.Count - 1
            Array.Copy(BlockArray(k).Data, 0, SavData, (&HF80 * k), &HF80)
        Next

        Return RawDeserialize(SavData, OpenRSESaveFile.GetType)

    End Function

    Public Shared Function OpenFRLGSaveFile(ByVal FileName As String) As FRLGSaveFile
        OpenFRLGSaveFile = New FRLGSaveFile
        Dim Blocks As New List(Of GBASaveBlock)
        Dim Data() As Byte
        Using fs As New FileStream(FileName, FileMode.Open, FileAccess.Read)
            Using bR As New BinaryReader(fs)
                Data = bR.ReadBytes(fs.Length)
                bR.Close()
                fs.Close()
            End Using
        End Using
        Dim BlockData(&HFFF) As Byte
        Dim theBlock As New GBASaveBlock
        For i As Integer = 0 To &H1BFFF Step &H1000
            Array.Copy(Data, i, BlockData, 0, &H1000)
            theBlock = RawDeserialize(BlockData, theBlock.GetType)
            If theBlock.Footer.Validation <> &H8012025 Then Throw New Exception("Corrupt data block!")
            If Blocks.Count <> 28 Then Blocks.Add(theBlock)
        Next

        Dim CurSave As UInt32 = 0UI
        For i As Integer = 0 To Blocks.Count - 1
            If Blocks(i).Footer.SaveID > CurSave Then CurSave = Blocks(i).Footer.SaveID
        Next

        Dim BlockArray(27) As GBASaveBlock

        For Each BLK As GBASaveBlock In Blocks
            If BLK.Footer.SaveID = CurSave Then
                BlockArray(BLK.Footer.BlockID) = BLK
            Else
                BlockArray(BLK.Footer.BlockID + 14) = BLK
            End If
        Next

        Dim SavData(&H20000) As Byte

        For k As Integer = 0 To BlockArray.Count - 1
            Array.Copy(BlockArray(k).Data, 0, SavData, (&HF80 * k), &HF80)
        Next

        Return RawDeserialize(SavData, OpenFRLGSaveFile.GetType)

    End Function

    <Serializable()> _
    <StructLayout(LayoutKind.Sequential, Size:=&H20000)> _
    Public Class RSESaveFile
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H4D84)> _
        Public Data1() As Byte
        Public PC_Storage1 As GBAPCStorageSystem
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H83C)> _
        Public Data2() As Byte
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H4D84)> _
        Public Data3() As Byte
        Public PC_Storage2 As GBAPCStorageSystem
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H83C)> _
        Public Data4() As Byte
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H4E00)> _
        Public Data5() As Byte
    End Class

    <Serializable()> _
    <StructLayout(LayoutKind.Sequential, Size:=&H20000)> _
    Public Class FRLGSaveFile
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H4D84)> _
        Public Data1() As Byte
        Public PC_Storage1 As GBAPCStorageSystem
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H83C)> _
        Public Data2() As Byte
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H4D84)> _
        Public Data3() As Byte
        Public PC_Storage2 As GBAPCStorageSystem
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H83C)> _
        Public Data4() As Byte
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H4E00)> _
        Public Data5() As Byte
    End Class

    <Serializable()> _
    <StructLayout(LayoutKind.Sequential, Size:=48)> _
    Public Class GBAPKMSubStruct

        'G
        Public _Species As UInt16
        Public _Item As UInt16
        Public EXP As UInt32
        Public _PPBonus As Byte
        Public Tameness As Byte
        Public Unknown As UInt16

        'A
        Public _Move1 As UInt16
        Public _Move2 As UInt16
        Public _Move3 As UInt16
        Public _Move4 As UInt16
        Public _PP1 As Byte
        Public _PP2 As Byte
        Public _PP3 As Byte
        Public _PP4 As Byte

        'E
        Public _HPEV As Byte
        Public _ATKEV As Byte
        Public _DEFEV As Byte
        Public _SPDEV As Byte
        Public _SPATKEV As Byte
        Public _SPDEFEV As Byte
        Public _Cool As Byte
        Public _Beauty As Byte
        Public _Cute As Byte
        Public _Smart As Byte
        Public _Tough As Byte
        Public _Feel As Byte

        'M
        Public _PKRS As Byte
        Public _MetLoc As Byte
        Public _LevelGameBallTrGender As UInt16
        Public _IVs As UInt32
        Public _Ribbons As UInt32

        '0 	 1 	 2 	 3 	 4 	 5 	 6 	| 7 	 0 	 1 	 2 	| 3 	 4 	 5 	 6 	| 7
        'Level met               |Original game |	Pokeball      |	Trainer gender
        '
        'Or, in C speak:
        '
        'u32 Level:7
        'u32 OriginGame:4;
        'u32 Pokeball:4;
        'u32 TrainerGender:1;

        Public Sub New()
            _Species = 0US
            _Item = 0US
            EXP = 0UI
            _PPBonus = 0
            Tameness = 0
            Unknown = 0US
            _Move1 = 0US
            _Move2 = 0US
            _Move3 = 0US
            _Move4 = 0US
            _PP1 = 0
            _PP2 = 0
            _PP3 = 0
            _PP4 = 0
            _HPEV = 0
            _ATKEV = 0
            _DEFEV = 0
            _SPDEV = 0
            _SPATKEV = 0
            _SPDEFEV = 0
            _Cool = 0
            _Beauty = 0
            _Cute = 0
            _Smart = 0
            _Tough = 0
            _Feel = 0
            _PKRS = 0
            _MetLoc = 0
            _LevelGameBallTrGender = 0US
            _IVs = 0UI
            _Ribbons = 0UI
        End Sub

    End Class

    <Serializable()> _
    <StructLayout(LayoutKind.Sequential, Size:=100)> _
    Public Class GBAPokemon

#Region "Members"
        Public PID As UInt32
        Public OTID As UInt32
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=10)> _
        Private _NicknameData() As Byte
        Public Font As Byte
        Public Sanity As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=7)> _
        Private _TrainerNameData() As Byte
        Private _Marks As Byte
        Private Checksum As UInt16
        Private Unknown1 As UInt16
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=12)> _
        Private Data1() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=12)> _
        Private Data2() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=12)> _
        Private Data3() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=12)> _
        Private Data4() As Byte
        Private Status As UInt32
        Public _Level As Byte
        Private Unknown2 As Byte
        Public CurHP As UInt16
        Public MaxHP As UInt16
        Public Attack As UInt16
        Public Defense As UInt16
        Public Speed As UInt16
        Public SpAtk As UInt16
        Public SpDef As UInt16
#End Region

#Region "Constructors"

        Public Sub New()
            PID = 0UI
            OTID = 0UI
            _NicknameData = Nothing
            Font = 0
            Sanity = 0
            _TrainerNameData = Nothing
            _Marks = 0
            Checksum = 0US
            Unknown1 = 0US
            Data1 = Nothing
            Data2 = Nothing
            Data3 = Nothing
            Data4 = Nothing
            Status = 0UI
            _Level = 0
            Unknown2 = 0
            CurHP = 0
            MaxHP = 0
            Attack = 0
            Defense = 0
            Speed = 0
            SpAtk = 0
            SpDef = 0
        End Sub

#End Region

#Region "Properties"

        'TODO: Build GBA PKM structure properties

        Private ReadOnly Property EncryptedStructure() As GBAPKMSubStruct
            Get
                EncryptedStructure = New GBAPKMSubStruct
                EncryptedStructure = RawDeserialize(UnshuffleData, EncryptedStructure.GetType)
            End Get
        End Property

        Public ReadOnly Property BaseStats() As mBaseStats
            Get
                Return New mBaseStats(Species.ID)
            End Get
        End Property

        Public ReadOnly Property Level() As Byte
            Get
                InitializeDictionaries()
                For i As Integer = 0 To 99
                    If EXP < LevelTable(i, BaseStats.GrowthGroup) Then
                        Return i
                    End If
                Next
                Return 100
            End Get
        End Property

        Public ReadOnly Property Species() As mSpecies
            Get
                If GBASpecies(EncryptedStructure._Species).Length = 3 Then
                    If LCase(GBASpecies(EncryptedStructure._Species)) = "egg" Then
                        Return New mSpecies
                    Else
                        Return New mSpecies(UInt16.Parse(GBASpecies(EncryptedStructure._Species)))
                    End If
                Else
                    Return New mSpecies(201)
                End If
                Return New mSpecies
            End Get
        End Property

        Public ReadOnly Property EXP() As UInt32
            Get
                Return EncryptedStructure.EXP
            End Get
        End Property

        Public Property TID() As UInt16
            Get
                Return BitConverter.ToUInt16(BitConverter.GetBytes(OTID), 0)
            End Get
            Set(ByVal value As UInt16)
                OTID = (UInt32.Parse(SID) << 16) + value
            End Set
        End Property

        Public Property SID() As UInt16
            Get
                Return BitConverter.ToUInt16(BitConverter.GetBytes(OTID), 2)
            End Get
            Set(ByVal value As UInt16)
                OTID = (UInt32.Parse(value) << 16) + TID
            End Set
        End Property

        Private ReadOnly Property Forme() As Byte
            Get
                If EncryptedStructure._Species = 201 Then
                    Return Convert.ToUInt16(DecToBin(PID, 32).Substring(6, 2) & _
                                                      DecToBin(PID, 32).Substring(14, 2) & _
                                                      DecToBin(PID, 32).Substring(22, 2) & _
                                                      DecToBin(PID, 32).Substring(30, 2), 2) Mod 28
                ElseIf EncryptedStructure._Species <> 201 Then
                    Return 0
                Else

                    Select Case GBASpecies(EncryptedStructure._Species).Substring(3, 1)
                        Case "A"
                            Return 0
                        Case "B"
                            Return 1
                        Case "C"
                            Return 2
                        Case "D"
                            Return 3
                        Case "E"
                            Return 4
                        Case "F"
                            Return 5
                        Case "G"
                            Return 6
                        Case "H"
                            Return 7
                        Case "I"
                            Return 8
                        Case "J"
                            Return 9
                        Case "K"
                            Return 10
                        Case "L"
                            Return 11
                        Case "M"
                            Return 12
                        Case "N"
                            Return 13
                        Case "O"
                            Return 14
                        Case "P"
                            Return 15
                        Case "Q"
                            Return 16
                        Case "R"
                            Return 17
                        Case "S"
                            Return 18
                        Case "T"
                            Return 19
                        Case "U"
                            Return 20
                        Case "V"
                            Return 21
                        Case "W"
                            Return 22
                        Case "X"
                            Return 23
                        Case "Y"
                            Return 24
                        Case "Z"
                            Return 25
                        Case "!"
                            Return 26
                        Case "?"
                            Return 27
                        Case Else
                            Return 0
                    End Select
                End If
            End Get
        End Property

        Public Property Nickname() As String
            Get
                Nickname = ""
                For i As Integer = 0 To _NicknameData.Length - 1
                    If _NicknameData(i) = &HFF Then Exit For
                    If GBAFont.Keys.Contains(_NicknameData(i)) Then _
                    Nickname &= GBAFont(_NicknameData(i))
                Next
            End Get
            Set(ByVal value As String)
                Dim fntArray() As String = GBAFont.Values.ToArray
                For i As Integer = 0 To 9
                    If i >= value.Length Then
                        _NicknameData(i) = &HFF
                    Else
                        _NicknameData(i) = Array.IndexOf(fntArray, value.Substring(i, 1))
                    End If
                Next
            End Set
        End Property

        Public ReadOnly Property Moves() As mMoves()
            Get
                InitializeDictionaries()
                Dim retMoves(3) As mMoves
                retMoves(0) = New mMoves(EncryptedStructure._Move1)
                retMoves(1) = New mMoves(EncryptedStructure._Move2)
                retMoves(2) = New mMoves(EncryptedStructure._Move3)
                retMoves(3) = New mMoves(EncryptedStructure._Move4)
                retMoves(0).CurrentPP = EncryptedStructure._PP1
                retMoves(0).TotalPP = retMoves(0).BasePP + (retMoves(0).BasePP * 0.2 * (EncryptedStructure._PPBonus And &H3))
                retMoves(1).CurrentPP = EncryptedStructure._PP2
                retMoves(1).TotalPP = retMoves(1).BasePP + (retMoves(1).BasePP * 0.2 * (EncryptedStructure._PPBonus And &HC))
                retMoves(2).CurrentPP = EncryptedStructure._PP3
                retMoves(2).TotalPP = retMoves(2).BasePP + (retMoves(2).BasePP * 0.2 * (EncryptedStructure._PPBonus And &H30))
                retMoves(3).CurrentPP = EncryptedStructure._PP4
                retMoves(3).TotalPP = retMoves(3).BasePP + (retMoves(3).BasePP * 0.2 * (EncryptedStructure._PPBonus And &HC0))
                Return retMoves
            End Get
            'Set(ByVal value As mMoves())
            '    'mMove1 = value(0).Value
            '    'mMove2 = value(1).Value
            '    'mMove3 = value(2).Value
            '    'mMove4 = value(3).Value
            '    'mM1PP = value(0).CurrentPP
            '    'mM2PP = value(1).CurrentPP
            '    'mM3PP = value(2).CurrentPP
            '    'mM4PP = value(3).CurrentPP
            '    'mM1PPUp = (value(0).TotalPP - value(0).BasePP) / (value(0).BasePP * 0.2)
            '    'mM2PPUp = (value(1).TotalPP - value(1).BasePP) / (value(1).BasePP * 0.2)
            '    'mM3PPUp = (value(2).TotalPP - value(2).BasePP) / (value(2).BasePP * 0.2)
            '    'mM4PPUp = (value(3).TotalPP - value(3).BasePP) / (value(3).BasePP * 0.2)
            'End Set
        End Property

        Public Property IVs() As mIVs
            Get
                IVs = New mIVs
                With IVs
                    .HP = (EncryptedStructure._IVs >> (0)) And &H1F
                    .Attack = (EncryptedStructure._IVs >> (5)) And &H1F
                    .Defense = (EncryptedStructure._IVs >> (10)) And &H1F
                    .Speed = (EncryptedStructure._IVs >> (15)) And &H1F
                    .SpAttack = (EncryptedStructure._IVs >> (20)) And &H1F
                    .SpDefense = (EncryptedStructure._IVs >> (25)) And &H1F
                End With
            End Get
            Set(ByVal newIVs As mIVs)
                Dim retString As String = ""
                If IsEgg Then
                    retString &= "1"
                Else
                    retString &= "0"
                End If
                If Nicknamed Then
                    retString &= "1"
                Else
                    retString &= "0"
                End If
                With newIVs
                    retString &= DecToBin(.SpDefense, 5).PadLeft(5, "0")
                    retString &= DecToBin(.SpAttack, 5).PadLeft(5, "0")
                    retString &= DecToBin(.Speed, 5).PadLeft(5, "0")
                    retString &= DecToBin(.Defense, 5).PadLeft(5, "0")
                    retString &= DecToBin(.Attack, 5).PadLeft(5, "0")
                    retString &= DecToBin(.HP, 5).PadLeft(5, "0")
                End With
                EncryptedStructure._IVs = Convert.ToUInt32(retString, 2)
            End Set
        End Property

        Public Property FatefulEncounter() As Boolean
            Get
                Dim ba As New BitArray(BitConverter.GetBytes(Unknown1))
                Return ba(7)
            End Get
            Set(ByVal value As Boolean)
                Dim OBED As String = DecToBin(Unknown1, 16)
                If value Then
                    Unknown1 = Convert.ToUInt16(OBED.Substring(0, 8) & "1" & OBED.Substring(8, 7), 2)
                Else
                    Unknown1 = Convert.ToUInt16(OBED.Substring(0, 8) & "0" & OBED.Substring(8, 7), 2)
                End If
            End Set
        End Property

        Public Property IsEgg() As Boolean
            Get
                Dim ba As New BitArray(BitConverter.GetBytes(EncryptedStructure._IVs))
                Return ba(31)
            End Get
            Set(ByVal value As Boolean)
                Dim mIVsAndEtcBIN As String = DecToBin(EncryptedStructure._IVs, 32)
                If value Then
                    mIVsAndEtcBIN = "1" & mIVsAndEtcBIN.Substring(1, 31)
                Else
                    mIVsAndEtcBIN = "0" & mIVsAndEtcBIN.Substring(1, 31)
                End If
                EncryptedStructure._IVs = Convert.ToUInt32(mIVsAndEtcBIN, 2)
            End Set
        End Property

        Public Property Nicknamed() As Boolean
            Get
                Dim ba As New BitArray(BitConverter.GetBytes(EncryptedStructure._IVs))
                Return ba(30)
            End Get
            Set(ByVal value As Boolean)
                Dim mIVsAndEtcBIN As String = DecToBin(EncryptedStructure._IVs, 32)
                If value Then
                    mIVsAndEtcBIN = mIVsAndEtcBIN.Substring(0, 1) & "1" & mIVsAndEtcBIN.Substring(2, 30)
                Else
                    mIVsAndEtcBIN = mIVsAndEtcBIN.Substring(0, 1) & "0" & mIVsAndEtcBIN.Substring(2, 30)
                End If
                EncryptedStructure._IVs = Convert.ToUInt32(mIVsAndEtcBIN, 2)
            End Set
        End Property

        Public Property TrainerName() As String
            Get
                TrainerName = ""

                For i As Integer = 0 To _TrainerNameData.Length - 1
                    If _TrainerNameData(i) = &HFF Then Exit For
                    If GBAFont.Keys.Contains(_TrainerNameData(i)) Then _
                    TrainerName &= GBAFont(_TrainerNameData(i))
                Next
            End Get
            Set(ByVal value As String)
                Dim fntArray() As String = GBAFont.Values.ToArray
                For i As Integer = 0 To 6
                    If i >= value.Length Then
                        _TrainerNameData(i) = &HFF
                        Exit For
                    Else
                        _TrainerNameData(i) = Array.IndexOf(fntArray, value.Substring(i, 1))
                    End If
                Next
            End Set
        End Property

        Public Property TrainerGender() As mGender
            Get
                Return New mGender(Integer.Parse(DecToBin(EncryptedStructure._LevelGameBallTrGender, 16).Last))
            End Get
            Set(ByVal value As mGender)
                If value.Value = Genders.Female Then
                    EncryptedStructure._LevelGameBallTrGender = _
                    Convert.ToUInt16(DecToBin(EncryptedStructure._LevelGameBallTrGender, 16).Substring(0, 15) & "1", 2)
                Else
                    EncryptedStructure._LevelGameBallTrGender = _
                    Convert.ToUInt16(DecToBin(EncryptedStructure._LevelGameBallTrGender, 16).Substring(0, 15) & "0", 2)
                End If
            End Set
        End Property

        Public Property Item() As mItems
            Get
                PokemonLib.InitializeDictionaries()
                Dim _item As New mItems(EncryptedStructure._Item, False)
                Return _item
            End Get
            Set(ByVal value As mItems)
                EncryptedStructure._Item = value.Value
            End Set
        End Property

        Public Property Ball() As mItems
            Get
                PokemonLib.InitializeDictionaries()
                Return New mItems(Convert.ToUInt16(DecToBin(EncryptedStructure._LevelGameBallTrGender, 16).Substring(1, 4), 2), False)
            End Get
            Set(ByVal value As mItems)
                EncryptedStructure._LevelGameBallTrGender = Convert.ToUInt16( _
                                                                             DecToBin(EncryptedStructure._LevelGameBallTrGender, 16).Substring(0, 1)) & _
                                                                             DecToBin(value.Value, 4).PadLeft(4, "0") & _
                                                                             DecToBin(EncryptedStructure._LevelGameBallTrGender, 16).Substring(5, 11)
            End Set
        End Property

        Public Property MetGame() As mGBALocation
            Get
                PokemonLib.InitializeDictionaries()
                MetGame = New mGBALocation
                MetGame.Value = Convert.ToUInt16(DecToBin(EncryptedStructure._LevelGameBallTrGender, 16).Substring(5, 4), 2)
            End Get
            Set(ByVal value As mGBALocation)
                EncryptedStructure._LevelGameBallTrGender = Convert.ToUInt16( _
                                                                             DecToBin(EncryptedStructure._LevelGameBallTrGender, 16).Substring(0, 5)) & _
                                                                             DecToBin(value.Value, 4).PadLeft(4, "0") & _
                                                                             DecToBin(EncryptedStructure._LevelGameBallTrGender, 16).Substring(10, 6)
            End Set
        End Property

        'TODO: Hometown
        'Locations with index numbers up to 087 appeared in Pokémon Ruby and Sapphire,
        'hose with index numbers between 088 and 195 appeared in Pokémon FireRed and LeafGreen,
        'and those with index numbers beyond 197 are exclusive to Pokémon Emerald.
        'All games, however, have the locations with index numbers 254 and 255,
        'which correspond to in-game trades and "fateful encounters".

#End Region

#Region "Functions"

        Private Function UnshuffleData() As Byte()

            If Data1 Is Nothing Or _
               Data2 Is Nothing Or _
               Data3 Is Nothing Or _
               Data4 Is Nothing _
               Then Return Nothing

            Dim DataUNSHUFFLED(47) As Byte
            Dim DataOUT(47) As Byte
            Dim ShuffleOrder As String = ""
            GBAShuffleOrder.TryGetValue(PID Mod 24, ShuffleOrder)
            Dim EncKey As UInt32 = PID Xor OTID

            Select Case ShuffleOrder.Substring(0, 1)
                Case "G"
                    Array.Copy(Data1, 0, DataUNSHUFFLED, 0, 12)
                Case "A"
                    Array.Copy(Data1, 0, DataUNSHUFFLED, 12, 12)
                Case "E"
                    Array.Copy(Data1, 0, DataUNSHUFFLED, 24, 12)
                Case "M"
                    Array.Copy(Data1, 0, DataUNSHUFFLED, 36, 12)
            End Select

            Select Case ShuffleOrder.Substring(1, 1)
                Case "G"
                    Array.Copy(Data2, 0, DataUNSHUFFLED, 0, 12)
                Case "A"
                    Array.Copy(Data2, 0, DataUNSHUFFLED, 12, 12)
                Case "E"
                    Array.Copy(Data2, 0, DataUNSHUFFLED, 24, 12)
                Case "M"
                    Array.Copy(Data2, 0, DataUNSHUFFLED, 36, 12)
            End Select

            Select Case ShuffleOrder.Substring(2, 1)
                Case "G"
                    Array.Copy(Data3, 0, DataUNSHUFFLED, 0, 12)
                Case "A"
                    Array.Copy(Data3, 0, DataUNSHUFFLED, 12, 12)
                Case "E"
                    Array.Copy(Data3, 0, DataUNSHUFFLED, 24, 12)
                Case "M"
                    Array.Copy(Data3, 0, DataUNSHUFFLED, 36, 12)
            End Select

            Select Case ShuffleOrder.Substring(3, 1)
                Case "G"
                    Array.Copy(Data4, 0, DataUNSHUFFLED, 0, 12)
                Case "A"
                    Array.Copy(Data4, 0, DataUNSHUFFLED, 12, 12)
                Case "E"
                    Array.Copy(Data4, 0, DataUNSHUFFLED, 24, 12)
                Case "M"
                    Array.Copy(Data4, 0, DataUNSHUFFLED, 36, 12)
            End Select

            For i As Integer = 0 To 44 Step 4
                Dim bef As UInt32 = BitConverter.ToUInt32(DataUNSHUFFLED, i)
                Dim aft As UInt32 = bef Xor EncKey

                Array.Copy(BitConverter.GetBytes(aft), 0, DataOUT, i, 4)
            Next

            Return DataOUT

        End Function

        Public Function ConvertToDS(ByVal Destination_Country As Byte, Optional ByVal Destination_Platinum As Boolean = True) As Pokemon
            ConvertToDS = New Pokemon
            With ConvertToDS
                .Species = New mSpecies(Species.ID)
                .Forme = Forme
                .Ability = New mAbility(BaseStats.Ability1)
                If BaseStats.Ability2 < 77 Then
                    If BaseStats.Ability2 <> 0 Then
                        If PID Mod 2 Then
                            .Ability = New mAbility(BaseStats.Ability2)
                        End If
                    End If
                End If
                If dPKMItems.Values.Contains(Ball.Name) And Ball.Value <> 0 Then
                    .Ball = New mItems(Array.IndexOf(dPKMItems.Values.ToArray, Ball.Name))
                Else
                    .Ball = New mItems(4)
                End If
                .Condition = New mContestStats(EncryptedStructure._Cool, _
                 EncryptedStructure._Beauty, EncryptedStructure._Cute, _
                 EncryptedStructure._Smart, EncryptedStructure._Tough, _
                 EncryptedStructure._Feel)
                .EVs = New mEVs(EncryptedStructure._HPEV, EncryptedStructure._ATKEV, _
                 EncryptedStructure._DEFEV, EncryptedStructure._SPDEV, EncryptedStructure._SPATKEV, _
                 EncryptedStructure._SPDEFEV)
                .EXP = EncryptedStructure.EXP
                .FatefulEncounter = FatefulEncounter
                If .BaseStats.Gender = 255 Then
                    .Gender = New mGender(Genders.Genderless)
                ElseIf .BaseStats.Gender = 254 Then
                    .Gender = New mGender(Genders.Female)
                ElseIf .BaseStats.Gender = 0 Then
                    .Gender = New mGender(Genders.Male)
                Else
                    If (.PID Mod 256) > .BaseStats.Gender Then
                        .Gender = New mGender(Genders.Male)
                    Else
                        .Gender = New mGender(Genders.Female)
                    End If
                End If
                .IsEgg = IsEgg
                If dPKMItems.Values.Contains(Item.Name) Then _
                .Item = New mItems(Array.IndexOf(dPKMItems.Values.ToArray, Item.Name))
                .IVs = IVs
                .SetMarks(ExamineBit(_Marks, 1), False, _
                          ExamineBit(_Marks, 4), ExamineBit(_Marks, 2), _
                          False, ExamineBit(_Marks, 3))
                .Moves = Moves
                .Nickname = Nickname
                .Nicknamed = Nicknamed

                Dim theOrigins As New mOrigins

                With theOrigins
                    .Country.Value = Destination_Country
                    .DateMet = Today
                    .Level = Level
                    .Location.DiamondPearl.Value = 55
                    .Egg.Location.DiamondPearl.Value = 0
                    .Egg.Location.Platinum.Value = 0
                    If Destination_Platinum Then
                        .Location.Platinum.Value = 55
                    Else
                        .Location.Platinum.Value = 0
                    End If
                    .Hometown.Value = 0 'TODO: Hometown
                End With

                .Origins = theOrigins

                ConvertToDS.SetEggDateBytes(New Byte() {0, 0, 0})

                .PID = PID
                'TODO: PokéRus
                '.Ribbons = New mRibbons 'TODO: Ribbons
                .Status = New mStatus
                .Tameness = EncryptedStructure.Tameness
                .Trainer = New mTrainer(TrainerName, TrainerGender, TID, SID, Destination_Platinum)
            End With
        End Function

#End Region

    End Class

#End Region

    'TODO: mDPGeneralBlock default constructor

    <Serializable()> _
<StructLayout(LayoutKind.Sequential, Size:=&HC100)> _
    Public Class mDPGeneralBlock '&HC100 bytes
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H34)> _
        Private DATA1() As Byte
        Public adStartDate As UInt32 '0x34
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H4)> _
        Private DATA2() As Byte
        Public leagueBeatDate As UInt32 '0x3C
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HC)> _
        Private DATA3() As Byte
        Public mFC_1 As UInt32 '0x4C
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H14)> _
        Private _DATA3() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H10)> _
        Public trName() As Byte '0x64
        Public trID As UInt16 '0x74
        Public trSID As UInt16 '0x76
        Public trMoney As UInt32 '0x78
        Public trGender As Genders '0x7C
        Public trCountry As Countries '0x7D
        Public trBadges As Byte '0x7E
        Public trAvatar As Avatars '0x7F
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H6)> _
        Private DATA5() As Byte
        Public mPlayHrs As UInt16 '0x86
        Public mPlayMin As Byte '0x88
        Public mPlaySec As Byte '0x89
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HA)> _
        Private DATA6() As Byte
        Public numPtPKM As Byte '0x94
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H3)> _
        Private DATA7() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H588)> _
        Public ptPKM() As Byte '0x98
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H4)> _
        Private DATA8() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H294)> _
        Public trGenItems() As Byte '0x624
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HC8)> _
        Public trKeyItems() As Byte '0x8B8
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H190)> _
        Public trTMs() As Byte '0x980
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H30)> _
        Public trMail() As Byte '0xB10
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HA0)> _
        Public trMedicine() As Byte '0xB40
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H100)> _
        Public trBerries() As Byte '0xBE0
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H3C)> _
        Public trBalls() As Byte '0xCE0
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HE0)> _
        Public trBattleItems() As Byte '0xD1C
        Public trStarter As Species '0xDFC
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H314)> _
        Private DATA9() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H9)> _
        Public trFly() As Byte '0x1112
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H31)> _
        Private DATA10() As Byte
        Public trPktchEnabled As Byte '0x114C
        Public trPktchAppsCount As Byte '0x114D
        Public trCurPktchApp As Byte '0x114E
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H19)> _
        Public trPktchApps() As Byte '0x114F
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H8)> _
        Private DATA111() As Byte
        Public trPktchSteps As UInt32 '0x1170
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H8E)> _
        Private DATA231() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H30)> _
        Public trPktchPKMHist() As Byte '0x1202
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H6)> _
        Private DATA11() As Byte
        Public trMapID As UInt16 '0x1238
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HE)> _
        Private DATA_321() As Byte
        Public trDirection As Directions '0x1248
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H97)> _
        Private DATA321() As Byte
        'TODO: Pokédex Class
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H100)> _
        Private trDexData() As Byte '0x12E0
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H24)> _
        Private _DATA111() As Byte
        Public trDexFormeViewer As Byte '0x1404
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HE)> _
        Private trDexForeignEntries() As Byte
        Public trDexLangViewer As Byte '0x1413
        Public trSDexUnlocked As Byte '0x1414
        Public trNDexUnlocked As Byte '0x1415
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HA6E)> _
        Private DATA12() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H700)> _
        Public trBerryTrees() As Byte 'Berry trees - 0x1E84 (128 x 14 bytes = &H700)
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H24)> _
        Private _DATA12() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H10)> _
        Public trRivalName() As Byte '0x25A8
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H42)> _
        Private DATA13() As Byte
        Public trMapX As UInt16 '0x25FA
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H2)> _
        Private DATA14() As Byte
        Public trMapY As UInt16 '0x25FE
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H2)> _
        Private DATA15() As Byte
        Public trMapZ As UInt16 '0x2602
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H1428)> _
        Private DATA16() As Byte
        Public trPlysMetUndgd As UInt32 '0x3A2C
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H8)> _
        Private _DATA16() As Byte
        Public trSphrsObtnd As UInt32 '0x3A38
        Public trFossObtnd As UInt32 '0x3A3C
        Public trGiftsGvn As UInt32 '0x3A40
        Public trTrapsHit As UInt32 '0x3A44
        Public trTrapsTrgr As UInt32 '0x3A48
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H14)> _
        Private DATA17() As Byte
        Public trFlagsCapt As UInt32 '0x3A60
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H15EC)> _
        Private _DATA19() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H310)> _
        Public trPoffins() As Byte '0x5050
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H5A4)> _
        Private _DATA17() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H600)> _
        Public TCardSig() As Byte '0x5904
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H4)> _
        Private _DATA21() As Byte
        Public trScore As UInt32 '0x5F08
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H20)> _
        Private _DATA22() As Byte
        Public trPKMCght As UInt32 '0x5F2C
        Public trPKMFished As UInt32 '0x5F30
        Public trPKMEggs As UInt32 '0x5F34
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H74)> _
        Private _DATA23() As Byte
        Public trPKMDftd As UInt32 '0x5FAC
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H1A)> _
        Private _DATA200() As Byte
        Public trSnglBatWins As UInt16 '0x5FCA
        Public trCnsSnglBatWins As UInt16 '0x5FCC
        Public trDblBatWins As UInt16 '0x5FCE
        Public trCnsDblBatWins As UInt16 '0x5FD0
        Public trMultBatWins As UInt16 '0x5FD2
        Public trCnsMultBatWins As UInt16 '0x5FD4
        Public trLnkMultBatWins As UInt16 '0x5FD6
        Public trCnsLnkMultBatWins As UInt16 '0x5FD8
        Public trWFBatWins As UInt16 '0x5FDA
        Public trCnsWFBatWins As UInt16 '0x5FDC
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H4)> _
        Private DATA20() As Byte
        Public trCntstWon As UInt16 '0x5FE2
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H2)> _
        Private DATA201() As Byte
        Public trRibbons As UInt16 '0x5FE6
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H68)> _
        Private DATA221() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H120)> _
        Public trCapsules() As Byte '0x6050
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H488)> _
        Private _DATA221() As Byte
        Public trBatPoints As UInt16 '0x65F8
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HCD6)> _
        Private _DATA20() As Byte
        Public trSafariZone As UInt32 '0x72D0
        Public trSwarm As UInt32 '0x72D4
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H4)> _
        Private _DATA18() As Byte
        Public trTrophyToday As UInt16 '0x72DC
        Public trTrophyYesterday As UInt16 '0x72DE
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H4)> _
        Private DATA19() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HA8)> _
        Public trHoneyTrees() As Byte 'Honey Trees - 0x72E4 (21 x 8 bytes = &HA8)
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HC)> _
        Public trPktchTHistory() As Byte '0x738C
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H50)> _
        Private DATA203() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HEC)> _
        Public trGTSPKM() As Byte '0x73E8
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H1A7C)> _
        Private DATA21() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H180)> _
        Public PalPadFCs() As Byte '0x8F50
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H600)> _
        Public PalPadNames() As Byte '0x90D0
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H2A1C)> _
        Private DATA212() As Byte
        Public Footer As GeneralFooter '0xC0EC

        Public Sub SetPartyCount(ByVal num As Byte)
            numPtPKM = num
        End Sub

    End Class

    <Serializable()> _
    <StructLayout(LayoutKind.Sequential, Size:=&H121E0)> _
    Public Class mDPStorageBlock '&H121E0 bytes
        Private mCurBox As Byte
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H3)> _
        Private Data1() As Byte
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H121B0)> _
        Private StorageBlockData() As Byte
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H18)> _
        Private WallpaperData() As Byte
        Public Footer As GeneralFooter

        Public Sub New()
            mCurBox = 0
            Data1 = Nothing
            StorageBlockData = Nothing
            WallpaperData = Nothing
            Footer = New GeneralFooter
        End Sub

        Public Function AddPCPokemon(ByVal BoxNum As Byte, ByVal PKM As Pokemon, Optional ByVal Slot As Byte = 0) As Boolean
            If Slot < 1 Or Slot > 30 Then Slot = 1
            Dim offset As Integer = ((BoxNum - 1) * 30) * 136
            Dim PArray(135) As Byte
            Dim _pkm As New Pokemon
            If Slot = 0 Then
                For i As Integer = offset To offset + (30 * 136) Step 136
                    Array.Copy(StorageBlockData, i, PArray, 0, 136)
                    _pkm = RawDeserialize(UnShuffleBytes(Decrypt_Data(PArray)), _pkm.GetType)
                    If _pkm.Species.ID <= 0 Or _pkm.Species.ID > 493 Then
                        PArray = EncryptPokemon(RawSerialize(PKM))
                        Array.Copy(PArray, 0, StorageBlockData, i, 136)
                        Return True
                    End If
                Next
            Else
                PArray = EncryptPokemon(RawSerialize(PKM))
                offset += ((Slot - 1) * 136)
                Array.Copy(PArray, 0, StorageBlockData, offset, 136)
                Return True
            End If
            Return False
        End Function

        Public Function RemovePCPokemon(ByVal BoxNum As Byte, ByVal SlotNum As Byte) As Pokemon
            RemovePCPokemon = New Pokemon
            Dim offset As Integer = (((BoxNum - 1) * 30) * 136) + ((SlotNum - 1) * 136)
            Dim PArray(135) As Byte
            Array.Copy(StorageBlockData, offset, PArray, 0, 136)
            For i As Integer = 0 To 135
                StorageBlockData(offset + i) = 0
            Next
            Return PC_to_Party(RawDeserialize(DecryptPokemon(PArray), RemovePCPokemon.GetType))
        End Function

        Public ReadOnly Property Boxes() As StorageBox()
            Get
                Dim mBoxes(17) As StorageBox
                For BoxNum As Integer = 0 To 17
                    Dim PKM(29) As Pokemon
                    Dim Offset As Integer = BoxNum * 30 * 136
                    For i As Integer = 0 To 29
                        Dim p(135) As Byte
                        Array.Copy(StorageBlockData, Offset, p, 0, 136)
                        p = UnShuffleBytes(Decrypt_Data(p))
                        PKM(i) = RawDeserialize(p, (New Pokemon).GetType)
                        Offset += 136
                    Next
                    Offset = BoxNum * 40
                    Dim n(39) As Byte
                    Array.Copy(StorageBlockData, Offset + &H11EE0, n, 0, 40)
                    mBoxes(BoxNum) = New StorageBox(BoxNum, PKMBytesToString(n), PKM, WallpaperData(BoxNum))
                Next
                Return mBoxes
            End Get
        End Property

        ''' <summary>
        ''' Gets the zero-based currently open box number.
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns an integer from 0 to 17.</returns>
        ''' <remarks></remarks>
        Public Property CurrentBoxNumber() As Byte
            Get
                Return mCurBox
            End Get
            Set(ByVal value As Byte)
                mCurBox = value
            End Set
        End Property

    End Class

    <Serializable()> _
    <StructLayout(LayoutKind.Sequential, Size:=&H2AC0)> _
    Public Class mDPHallOfFameBlock '&H2AC0 bytes (?)
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H2AB0)> _
        Public DATA() As Byte
        Public Footer As HallOfFameFooter

        Public Sub New()
            DATA = Nothing
            Footer = New HallOfFameFooter
        End Sub
    End Class

    'TODO: mPTGeneralBlock default constructor

    <Serializable()> _
<StructLayout(LayoutKind.Sequential, Size:=&HCF2C)> _
    Public Class mPtGeneralBlock '&HCF2C bytes
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H34)> _
        Public DATA1() As Byte
        Public adStartDate As UInt32 '0x34
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H4)> _
        Public DATA2() As Byte
        Public leagueBeatDate As UInt32 '0x3C
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HC)> _
        Public DATA3() As Byte
        Public mFC_1 As UInt32 '0x4C
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H18)> _
        Public DATA30() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H10)> _
        Public trName() As Byte '0x68
        Public trID As UInt16 '0x78
        Public trSID As UInt16 '0x80
        Public trMoney As UInt32 '0x8B
        Public trGender As Genders '0x80
        Public trCountry As Countries '0x81
        Public trBadges As Byte '0x82
        Public trAvatar As Avatars '0x83
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H6)> _
        Public DATA5() As Byte
        Public mPlayHrs As UInt16 '0x8A
        Public mPlayMin As Byte '0x8C
        Public mPlaySec As Byte '0x8D
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HA)> _
        Public DATA6() As Byte
        Public numPtPKM As Byte '0x98
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H7)> _
        Public DATA7() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H588)> _
        Public ptPKM() As Byte '0xA0
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H8)> _
        Private DATA8() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H294)> _
        Public trGenItems() As Byte '0x630
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HC8)> _
        Public trKeyItems() As Byte '0x8C4
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H190)> _
        Public trTMs() As Byte '0x98C
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H30)> _
        Public trMail() As Byte '0xB1C
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HA0)> _
        Public trMedicine() As Byte '0xB4C
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H100)> _
        Public trBerries() As Byte '0xBEC
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H3C)> _
        Public trBalls() As Byte '0xCEC
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HE4)> _
        Public trBattleItems() As Byte '0xD28
        Public trStarter As Species '0xE0C
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H314)> _
        Private DATA9() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H9)> _
        Public trFly() As Byte '0x1122
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H35)> _
        Private DATA10() As Byte
        Public trPktchEnabled As Byte '0x1160
        Public trPktchAppsCount As Byte '0x1161
        Public trCurPktchApp As Byte '0x1162
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H19)> _
        Public trPktchApps() As Byte '0x1163
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H8)> _
        Private DATA111() As Byte
        Public trPktchSteps As UInt32 '0x1184
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H90)> _
        Private DATA231() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H60)> _
        Public trPktchPKMHist() As Byte '0x1218
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H8)> _
        Private DATA11() As Byte
        Public trMapID As UInt16 '0x1280
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HE)> _
        Private DATA_321() As Byte
        Public trDirection As Directions '0x1290
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H9B)> _
        Private DATA321() As Byte
        'TODO: Platinum Pokédex Structure
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H314)> _
        Private trDexData() As Byte '0x132C
        Public trDexFormeViewer As Byte '0x1640
        Public trDexLangViewer As Byte '0x1641
        Public trSDexUnlocked As Byte '0x1642
        Public trNDexUnlocked As Byte '0x1643
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HA80)> _
        Private DATA12() As Byte

        '0x1650: Unknown Pokémon
        '0x1740: Unknown Pokémon

        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H700)> _
        Public trBerryTrees() As Byte 'Berry trees - 0x20C4 (128 x 14 bytes = &H700)
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H24)> _
        Private _DATA12() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H10)> _
        Public trRivalName() As Byte '0x27E8
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H86)> _
        Private DATA13() As Byte
        Public trMapX As UInt16 '0x287E
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H2)> _
        Private DATA14() As Byte
        Public trMapY As UInt16 '0x2882
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H2)> _
        Private DATA15() As Byte
        Public trMapZ As UInt16 '0x2886
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H142C)> _
        Private DATA16() As Byte
        Public trPlysMetUndgd As UInt32 '0x3CB4
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H8)> _
        Private _DATA16() As Byte
        Public trSphrsObtnd As UInt32 '0x3CC0
        Public trFossObtnd As UInt32 '0x3CC4
        Public trGiftsGvn As UInt32 '0x3CC8
        Public trTrapsHit As UInt32 '0x3CCC
        Public trTrapsTrgr As UInt32 '0x3CD0
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H14)> _
        Private DATA17() As Byte
        Public trFlagsCapt As UInt32 '0x3CE8
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H15FC)> _
        Private _DATA19() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H320)> _
        Public trPoffins() As Byte '0x52E8
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H5A0)> _
        Private _DATA17() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H600)> _
        Public TCardSig() As Byte '0x5BA8
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H738)> _
        Private _DATA21() As Byte
        'Public trScore As UInt32 '0x61B0
        '<MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H20)> _
        'Private _DATA22() As Byte
        'Public trPKMCght As UInt32 '0x5F2C
        'Public trPKMFished As UInt32 '0x5F30
        'Public trPKMEggs As UInt32 '0x5F34
        '<MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H74)> _
        'Private _DATA23() As Byte
        'Public trPKMDftd As UInt32 '0x5FAC
        '<MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H1A)> _
        'Private _DATA200() As Byte
        Public trSnglBatWins As UInt16 '0x68E0
        Public trCnsSnglBatWins As UInt16 '0x68E2
        Public trDblBatWins As UInt16 '0x68E4
        Public trCnsDblBatWins As UInt16 '0x68E6
        Public trMultBatWins As UInt16 '0x68E8
        Public trCnsMultBatWins As UInt16 '0x68EA
        Public trLnkMultBatWins As UInt16 '0x68EC
        Public trCnsLnkMultBatWins As UInt16 '0x68EE
        Public trWFBatWins As UInt16 '0x68F0
        Public trCnsWFBatWins As UInt16 '0x68F2
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H4)> _
        Private DATA20() As Byte
        Public trCntstWon As UInt16 '0x68F8
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H2)> _
        Private DATA201() As Byte
        Public trRibbons As UInt16 '0x68FC
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H64)> _
        Private DATA221() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H120)> _
        Public trCapsules() As Byte '0x6962
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H7B2)> _
        Private _DATA221() As Byte
        Public trBatPoints As UInt16 '0x7234
        '<MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H5CE2)> _
        'Private _DATA20() As Byte
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H2A02)> _
        Private _DATA20() As Byte
        'Public trSafariZone As UInt32 '0x72D0
        'Public trSwarm As UInt32 '0x72D4
        '<MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H4)> _
        'Private _DATA18() As Byte
        'Public trTrophyToday As UInt16 '0x72DC
        'Public trTrophyYesterday As UInt16 '0x72DE
        '<MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H4)> _
        'Private DATA19() As Byte
        '<MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HA8)> _
        'Public trHoneyTrees() As Byte 'Honey Trees - 0x72E4 (21 x 8 bytes = &HA8)
        '<MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HC)> _
        'Public trPktchTHistory() As Byte '0x738C
        '<MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H50)> _
        'Private DATA203() As Byte
        '<MarshalAs(UnmanagedType.ByValArray, SizeConst:=&HEC)> _
        'Public trGTSPKM() As Byte '0x73E8
        '<MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H4C18)> _
        'Private DATA21() As Byte

        '2A01 bytes between last and next

        '0x9C37, &H180 for Pal Pad FCs

        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H180)> _
        Public PalPadFCs() As Byte '0x9C38

        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H700)> _
        Public PalPadNames() As Byte '0x9DB8

        ' &H500

        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H2A60)> _
        Private _DATA220() As Byte

        '0x9DC8 (Pal Pad names)

        '0xC7F0: Unknown Pokémon

        '0xC8DC: Unknown Pokémon

        '0xC9C8: Unknown Pokémon

        '0xCAB4: Unknown Pokémon

        '0xCBA0: Unknown Pokémon

        '0xCC8C: Unknown Pokémon

        Public Footer As GeneralFooter '0xCF18

        Public Sub SetPartyCount(ByVal num As Byte)
            numPtPKM = num
        End Sub

    End Class

    <Serializable()> _
    <StructLayout(LayoutKind.Sequential, Size:=&H121E4)> _
    Public Class mPtStorageBlock '&H121E4 bytes
        Private mCurBox As Byte
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H3)> _
        Private Data1() As Byte
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H121B0)> _
        Private StorageBlockData() As Byte
        <MarshalAs(UnmanagedType.ByValArray, sizeconst:=&H1C)> _
        Private WallpaperData() As Byte
        Public Footer As GeneralFooter

        Public Sub New()
            mCurBox = 0
            Data1 = Nothing
            StorageBlockData = Nothing
            WallpaperData = Nothing
            Footer = New GeneralFooter
        End Sub

        Public Function AddPCPokemon(ByVal BoxNum As Byte, ByVal PKM As Pokemon, Optional ByVal Slot As Byte = 0) As Boolean
            If Slot < 1 Or Slot > 30 Then Slot = 1
            Dim offset As Integer = ((BoxNum - 1) * 30) * 136
            Dim PArray(135) As Byte
            Dim _pkm As New Pokemon
            If Slot = 0 Then
                For i As Integer = offset To offset + (30 * 136) Step 136
                    Array.Copy(StorageBlockData, i, PArray, 0, 136)
                    _pkm = RawDeserialize(UnShuffleBytes(Decrypt_Data(PArray)), _pkm.GetType)
                    If _pkm.Species.ID <= 0 Or _pkm.Species.ID > 493 Then
                        PArray = EncryptPokemon(RawSerialize(PKM))
                        Array.Copy(PArray, 0, StorageBlockData, i, 136)
                        Return True
                    End If
                Next
            Else
                PArray = EncryptPokemon(RawSerialize(PKM))
                offset += ((Slot - 1) * 136)
                Array.Copy(PArray, 0, StorageBlockData, offset, 136)
                Return True
            End If
            Return False
        End Function

        Public Function RemovePCPokemon(ByVal BoxNum As Byte, ByVal SlotNum As Byte) As Pokemon
            RemovePCPokemon = New Pokemon
            Dim offset As Integer = (((BoxNum - 1) * 30) * 136) + ((SlotNum - 1) * 136)
            Dim PArray(135) As Byte
            Array.Copy(StorageBlockData, offset, PArray, 0, 136)
            For i As Integer = 0 To 135
                StorageBlockData(offset + i) = 0
            Next
            Return PC_to_Party(RawDeserialize(DecryptPokemon(PArray), RemovePCPokemon.GetType))
        End Function

        Public ReadOnly Property Boxes() As StorageBox()
            Get
                Dim mBoxes(17) As StorageBox
                For BoxNum As Integer = 0 To 17
                    Dim PKM(29) As Pokemon
                    Dim Offset As Integer = BoxNum * 30 * 136
                    For i As Integer = 0 To 29
                        Dim p(135) As Byte
                        Array.Copy(StorageBlockData, Offset, p, 0, 136)
                        p = UnShuffleBytes(Decrypt_Data(p))
                        PKM(i) = RawDeserialize(p, (New Pokemon).GetType)
                        Offset += 136
                    Next
                    Offset = BoxNum * 40
                    Dim n(39) As Byte
                    Array.Copy(StorageBlockData, Offset + &H11EE0, n, 0, 40)
                    mBoxes(BoxNum) = New StorageBox(BoxNum, PKMBytesToString(n), PKM, WallpaperData(BoxNum))
                Next
                Return mBoxes
            End Get
        End Property

        ''' <summary>
        ''' Gets or sets the zero-based currently open box number.
        ''' </summary>
        ''' <value></value>
        ''' <returns>Returns an integer from 0 to 17.</returns>
        ''' <remarks></remarks>
        Public Property CurrentBoxNumber() As Byte
            Get
                Return mCurBox
            End Get
            Set(ByVal value As Byte)
                mCurBox = value
            End Set
        End Property

    End Class

    <Serializable()> _
    <StructLayout(LayoutKind.Sequential, Size:=&H20000)> _
    Public Class mPtHallOfFameBlock '&H20000 bytes (?)
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=&H1FFF0)> _
        Public DATA() As Byte
        Public Footer As HallOfFameFooter

        Public Sub New()
            DATA = Nothing
            Footer = New HallOfFameFooter
        End Sub
    End Class

    <Serializable()> _
    <StructLayout(LayoutKind.Sequential)> _
    Public Class GeneralFooter
        'First set:
        'Starts at 0x0C0EC
        'Storage starts at 0x1FFEC
        'Second set:
        'Starts at 0x4C0EC
        'Storage starts at 0x5FFEC
        Private mStoCount As UInt32
        Private mGenCount As UInt32
        Public Size As UInt32
        Private RunTime1 As UInt32
        Private RunTime2 As UInt16
        Private mChecksum As UInt16

        Public Sub New()
            mStoCount = 0UI
            mGenCount = 0UI
            Size = 0UI
            RunTime1 = 0UI
            RunTime2 = 0US
            mChecksum = 0US
        End Sub

        Public Property StoCount() As UInt32
            Get
                Return mStoCount
            End Get
            Set(ByVal value As UInt32)
                mStoCount = value
            End Set
        End Property

        Public Property GenCount() As UInt32
            Get
                Return mGenCount
            End Get
            Set(ByVal value As UInt32)
                mGenCount = value
            End Set
        End Property

        Public Property Checksum() As UInt16
            Get
                Return mChecksum
            End Get
            Set(ByVal value As UInt16)
                mChecksum = value
            End Set
        End Property

    End Class

    <Serializable()> _
    <StructLayout(LayoutKind.Sequential)> _
    Public Class HallOfFameFooter
        'First set:
        'Starts at 0x3FFE8
        'Second set:
        'Starts at 0x7FFE8
        'TODO: Figure out Hall of Fame footer structure.
        'Private mGenCount As UInt32
        'Private mStoCount As UInt32
        Private RunTime1 As UInt32
        Private RunTime2 As UInt32
        Public Size As UInt32
        Private RunTime3 As UInt16
        Public mChecksum As UInt16

        Public Sub New()
            RunTime1 = 0UI
            RunTime2 = 0US
            Size = 0UI
            RunTime3 = 0US
            mChecksum = 0US
        End Sub

    End Class

    Public Class SaveFile

#Region "Variables"

        Private PtSav As PtSaveFile
        Private DPSav As DPSaveFile
        Private _SaveType As SaveTypes

#End Region

#Region "Enums"

        Private Enum SaveTypes
            Diamond_Pearl
            Platinum
            HeartGold_SoulSilver
        End Enum

#End Region

#Region "Constructors"

        Public Sub New()
            PtSav = New PtSaveFile
            DPSav = New DPSaveFile
            _SaveType = 0
        End Sub

        Public Sub New(ByVal FileName As String)
            Dim o As New Object
            o = OpenSaveFile(FileName)
            If o.GetType Is PtSav.GetType Then
                PtSav = o
                SaveType = SaveTypes.Platinum
            ElseIf o.GetType Is DPSav.GetType Then
                DPSav = o
                SaveType = SaveTypes.Diamond_Pearl
            Else
                Throw New Exception("Incorrect file type!")
            End If
        End Sub

#End Region

#Region "Properties"

        Private Property SaveType() As SaveTypes
            Get
                Return _SaveType
            End Get
            Set(ByVal value As SaveTypes)
                _SaveType = value
            End Set
        End Property

        Private Property Trainer() As mTrainer
            Get
                Return PtSav.Trainer
            End Get
            Set(ByVal value As mTrainer)
                PtSav.Trainer = value
            End Set
        End Property

#End Region

    End Class

    Public Class PKMFontConverter

        Public Function UnicodeToPKM(ByVal _Unicode As Integer) As Integer
            Dim rt As Integer
            mRevPKMFont.TryGetValue(_Unicode, rt)
            Return rt
        End Function

        Public Function PKMToUnicode(ByVal _PKM As Integer) As Integer
            Dim rt As Integer
            mPKMFontConverter.TryGetValue(_PKM, rt)
            Return rt
        End Function

        Public Function StringtoBytes(ByVal _String As String) As Byte()
            Dim dataOUT(_String.Length * 2 - 1) As Byte
            For i As Integer = 0 To dataOUT.Length - 2 Step 2
                dataOUT(i) = BitConverter.GetBytes(UnicodeToPKM(Char.ConvertToUtf32(_String, i / 2)))(0)
                dataOUT(i + 1) = BitConverter.GetBytes(UnicodeToPKM(Char.ConvertToUtf32(_String, i / 2)))(1)
            Next
            Return dataOUT
        End Function

        'Public Function BytesToString(ByVal _Bytes() As Byte) As String
        '    Dim strOUT As String = ""
        '    For i As Integer = 0 To _Bytes.Length - 2 Step 2
        '        'TODO: Fix conversion of byte array to string
        '        Dim strLetter As String
        '        strLetter = Char.ConvertFromUtf32(PKMToUnicode(BitConverter.ToUInt16(_Bytes, i))).ToString
        '        strOUT &= strLetter
        '    Next
        '    Return strOUT
        'End Function

        Private Shared mPKMFontConverter As New Dictionary(Of Integer, Integer)
        Private Shared mRevPKMFont As New Dictionary(Of Integer, Integer)

        Public Sub New()
            DictionaryInitialize()
        End Sub

        Public Sub DictionaryInitialize()

            Try

                mPKMFontConverter.Add(&H1, &H3000)
                mPKMFontConverter.Add(&H2, &H3041)
                mPKMFontConverter.Add(&H3, &H3042)
                mPKMFontConverter.Add(&H4, &H3043)
                mPKMFontConverter.Add(&H5, &H3044)
                mPKMFontConverter.Add(&H6, &H3045)
                mPKMFontConverter.Add(&H7, &H3046)
                mPKMFontConverter.Add(&H8, &H3047)
                mPKMFontConverter.Add(&H9, &H3048)
                mPKMFontConverter.Add(&HA, &H3049)
                mPKMFontConverter.Add(&HB, &H304A)
                mPKMFontConverter.Add(&HC, &H304B)
                mPKMFontConverter.Add(&HD, &H304C)
                mPKMFontConverter.Add(&HE, &H304D)
                mPKMFontConverter.Add(&HF, &H304E)
                mPKMFontConverter.Add(&H10, &H304F)
                mPKMFontConverter.Add(&H11, &H3050)
                mPKMFontConverter.Add(&H12, &H3051)
                mPKMFontConverter.Add(&H13, &H3052)
                mPKMFontConverter.Add(&H14, &H3053)
                mPKMFontConverter.Add(&H15, &H3054)
                mPKMFontConverter.Add(&H16, &H3055)
                mPKMFontConverter.Add(&H17, &H3056)
                mPKMFontConverter.Add(&H18, &H3057)
                mPKMFontConverter.Add(&H19, &H3058)
                mPKMFontConverter.Add(&H1A, &H3059)
                mPKMFontConverter.Add(&H1B, &H305A)
                mPKMFontConverter.Add(&H1C, &H305B)
                mPKMFontConverter.Add(&H1D, &H305C)
                mPKMFontConverter.Add(&H1E, &H305D)
                mPKMFontConverter.Add(&H1F, &H305E)
                mPKMFontConverter.Add(&H20, &H305F)
                mPKMFontConverter.Add(&H21, &H3060)
                mPKMFontConverter.Add(&H22, &H3061)
                mPKMFontConverter.Add(&H23, &H3062)
                mPKMFontConverter.Add(&H24, &H3063)
                mPKMFontConverter.Add(&H25, &H3064)
                mPKMFontConverter.Add(&H26, &H3065)
                mPKMFontConverter.Add(&H27, &H3066)
                mPKMFontConverter.Add(&H28, &H3067)
                mPKMFontConverter.Add(&H29, &H3068)
                mPKMFontConverter.Add(&H2A, &H3069)
                mPKMFontConverter.Add(&H2B, &H306A)
                mPKMFontConverter.Add(&H2C, &H306B)
                mPKMFontConverter.Add(&H2D, &H306C)
                mPKMFontConverter.Add(&H2E, &H306D)
                mPKMFontConverter.Add(&H2F, &H306E)
                mPKMFontConverter.Add(&H30, &H306F)
                mPKMFontConverter.Add(&H31, &H3070)
                mPKMFontConverter.Add(&H32, &H3071)
                mPKMFontConverter.Add(&H33, &H3072)
                mPKMFontConverter.Add(&H34, &H3073)
                mPKMFontConverter.Add(&H35, &H3074)
                mPKMFontConverter.Add(&H36, &H3075)
                mPKMFontConverter.Add(&H37, &H3076)
                mPKMFontConverter.Add(&H38, &H3077)
                mPKMFontConverter.Add(&H39, &H3078)
                mPKMFontConverter.Add(&H3A, &H3079)
                mPKMFontConverter.Add(&H3B, &H307A)
                mPKMFontConverter.Add(&H3C, &H307B)
                mPKMFontConverter.Add(&H3D, &H307C)
                mPKMFontConverter.Add(&H3E, &H307D)
                mPKMFontConverter.Add(&H3F, &H307E)
                mPKMFontConverter.Add(&H40, &H307F)
                mPKMFontConverter.Add(&H41, &H3080)
                mPKMFontConverter.Add(&H42, &H3081)
                mPKMFontConverter.Add(&H43, &H3082)
                mPKMFontConverter.Add(&H44, &H3083)
                mPKMFontConverter.Add(&H45, &H3084)
                mPKMFontConverter.Add(&H46, &H3085)
                mPKMFontConverter.Add(&H47, &H3086)
                mPKMFontConverter.Add(&H48, &H3087)
                mPKMFontConverter.Add(&H49, &H3088)
                mPKMFontConverter.Add(&H4A, &H3089)
                mPKMFontConverter.Add(&H4B, &H308A)
                mPKMFontConverter.Add(&H4C, &H308B)
                mPKMFontConverter.Add(&H4D, &H308C)
                mPKMFontConverter.Add(&H4E, &H308D)
                mPKMFontConverter.Add(&H4F, &H308F)
                mPKMFontConverter.Add(&H50, &H3092)
                mPKMFontConverter.Add(&H51, &H3093)
                mPKMFontConverter.Add(&H52, &H30A1)
                mPKMFontConverter.Add(&H53, &H30A2)
                mPKMFontConverter.Add(&H54, &H30A3)
                mPKMFontConverter.Add(&H55, &H30A4)
                mPKMFontConverter.Add(&H56, &H30A5)
                mPKMFontConverter.Add(&H57, &H30A6)
                mPKMFontConverter.Add(&H58, &H30A7)
                mPKMFontConverter.Add(&H59, &H30A8)
                mPKMFontConverter.Add(&H5A, &H30A9)
                mPKMFontConverter.Add(&H5B, &H30AA)
                mPKMFontConverter.Add(&H5C, &H30AB)
                mPKMFontConverter.Add(&H5D, &H30AC)
                mPKMFontConverter.Add(&H5E, &H30AD)
                mPKMFontConverter.Add(&H5F, &H30AE)
                mPKMFontConverter.Add(&H60, &H30AF)
                mPKMFontConverter.Add(&H61, &H30B0)
                mPKMFontConverter.Add(&H62, &H30B1)
                mPKMFontConverter.Add(&H63, &H30B2)
                mPKMFontConverter.Add(&H64, &H30B3)
                mPKMFontConverter.Add(&H65, &H30B4)
                mPKMFontConverter.Add(&H66, &H30B5)
                mPKMFontConverter.Add(&H67, &H30B6)
                mPKMFontConverter.Add(&H68, &H30B7)
                mPKMFontConverter.Add(&H69, &H30B8)
                mPKMFontConverter.Add(&H6A, &H30B9)
                mPKMFontConverter.Add(&H6B, &H30BA)
                mPKMFontConverter.Add(&H6C, &H30BB)
                mPKMFontConverter.Add(&H6D, &H30BC)
                mPKMFontConverter.Add(&H6E, &H30BD)
                mPKMFontConverter.Add(&H6F, &H30BE)
                mPKMFontConverter.Add(&H70, &H30BF)
                mPKMFontConverter.Add(&H71, &H30C0)
                mPKMFontConverter.Add(&H72, &H30C1)
                mPKMFontConverter.Add(&H73, &H30C2)
                mPKMFontConverter.Add(&H74, &H30C3)
                mPKMFontConverter.Add(&H75, &H30C4)
                mPKMFontConverter.Add(&H76, &H30C5)
                mPKMFontConverter.Add(&H77, &H30C6)
                mPKMFontConverter.Add(&H78, &H30C7)
                mPKMFontConverter.Add(&H79, &H30C8)
                mPKMFontConverter.Add(&H7A, &H30C9)
                mPKMFontConverter.Add(&H7B, &H30CA)
                mPKMFontConverter.Add(&H7C, &H30CB)
                mPKMFontConverter.Add(&H7D, &H30CC)
                mPKMFontConverter.Add(&H7E, &H30CD)
                mPKMFontConverter.Add(&H7F, &H30CE)
                mPKMFontConverter.Add(&H80, &H30CF)
                mPKMFontConverter.Add(&H81, &H30D0)
                mPKMFontConverter.Add(&H82, &H30D1)
                mPKMFontConverter.Add(&H83, &H30D2)
                mPKMFontConverter.Add(&H84, &H30D3)
                mPKMFontConverter.Add(&H85, &H30D4)
                mPKMFontConverter.Add(&H86, &H30D5)
                mPKMFontConverter.Add(&H87, &H30D6)
                mPKMFontConverter.Add(&H88, &H30D7)
                mPKMFontConverter.Add(&H89, &H30D8)
                mPKMFontConverter.Add(&H8A, &H30D9)
                mPKMFontConverter.Add(&H8B, &H30DA)
                mPKMFontConverter.Add(&H8C, &H30DB)
                mPKMFontConverter.Add(&H8D, &H30DC)
                mPKMFontConverter.Add(&H8E, &H30DD)
                mPKMFontConverter.Add(&H8F, &H30DE)
                mPKMFontConverter.Add(&H90, &H30DF)
                mPKMFontConverter.Add(&H91, &H30E0)
                mPKMFontConverter.Add(&H92, &H30E1)
                mPKMFontConverter.Add(&H93, &H30E2)
                mPKMFontConverter.Add(&H94, &H30E3)
                mPKMFontConverter.Add(&H95, &H30E4)
                mPKMFontConverter.Add(&H96, &H30E5)
                mPKMFontConverter.Add(&H97, &H30E6)
                mPKMFontConverter.Add(&H98, &H30E7)
                mPKMFontConverter.Add(&H99, &H30E8)
                mPKMFontConverter.Add(&H9A, &H30E9)
                mPKMFontConverter.Add(&H9B, &H30EA)
                mPKMFontConverter.Add(&H9C, &H30EB)
                mPKMFontConverter.Add(&H9D, &H30EC)
                mPKMFontConverter.Add(&H9E, &H30ED)
                mPKMFontConverter.Add(&H9F, &H30EF)
                mPKMFontConverter.Add(&HA0, &H30F2)
                mPKMFontConverter.Add(&HA1, &H30F3)
                mPKMFontConverter.Add(&HA2, &HFF10)
                mPKMFontConverter.Add(&HA3, &HFF11)
                mPKMFontConverter.Add(&HA4, &HFF12)
                mPKMFontConverter.Add(&HA5, &HFF13)
                mPKMFontConverter.Add(&HA6, &HFF14)
                mPKMFontConverter.Add(&HA7, &HFF15)
                mPKMFontConverter.Add(&HA8, &HFF16)
                mPKMFontConverter.Add(&HA9, &HFF17)
                mPKMFontConverter.Add(&HAA, &HFF18)
                mPKMFontConverter.Add(&HAB, &HFF19)
                mPKMFontConverter.Add(&HAC, &HFF21)
                mPKMFontConverter.Add(&HAD, &HFF22)
                mPKMFontConverter.Add(&HAE, &HFF23)
                mPKMFontConverter.Add(&HAF, &HFF24)
                mPKMFontConverter.Add(&HB0, &HFF25)
                mPKMFontConverter.Add(&HB1, &HFF26)
                mPKMFontConverter.Add(&HB2, &HFF27)
                mPKMFontConverter.Add(&HB3, &HFF28)
                mPKMFontConverter.Add(&HB4, &HFF29)
                mPKMFontConverter.Add(&HB5, &HFF2A)
                mPKMFontConverter.Add(&HB6, &HFF2B)
                mPKMFontConverter.Add(&HB7, &HFF2C)
                mPKMFontConverter.Add(&HB8, &HFF2D)
                mPKMFontConverter.Add(&HB9, &HFF2E)
                mPKMFontConverter.Add(&HBA, &HFF2F)
                mPKMFontConverter.Add(&HBB, &HFF30)
                mPKMFontConverter.Add(&HBC, &HFF31)
                mPKMFontConverter.Add(&HBD, &HFF32)
                mPKMFontConverter.Add(&HBE, &HFF33)
                mPKMFontConverter.Add(&HBF, &HFF34)
                mPKMFontConverter.Add(&HC0, &HFF35)
                mPKMFontConverter.Add(&HC1, &HFF36)
                mPKMFontConverter.Add(&HC2, &HFF37)
                mPKMFontConverter.Add(&HC3, &HFF38)
                mPKMFontConverter.Add(&HC4, &HFF39)
                mPKMFontConverter.Add(&HC5, &HFF3A)
                mPKMFontConverter.Add(&HC6, &HFF41)
                mPKMFontConverter.Add(&HC7, &HFF42)
                mPKMFontConverter.Add(&HC8, &HFF43)
                mPKMFontConverter.Add(&HC9, &HFF44)
                mPKMFontConverter.Add(&HCA, &HFF45)
                mPKMFontConverter.Add(&HCB, &HFF46)
                mPKMFontConverter.Add(&HCC, &HFF47)
                mPKMFontConverter.Add(&HCD, &HFF48)
                mPKMFontConverter.Add(&HCE, &HFF49)
                mPKMFontConverter.Add(&HCF, &HFF4A)
                mPKMFontConverter.Add(&HD0, &HFF4B)
                mPKMFontConverter.Add(&HD1, &HFF4C)
                mPKMFontConverter.Add(&HD2, &HFF4D)
                mPKMFontConverter.Add(&HD3, &HFF4E)
                mPKMFontConverter.Add(&HD4, &HFF4F)
                mPKMFontConverter.Add(&HD5, &HFF50)
                mPKMFontConverter.Add(&HD6, &HFF51)
                mPKMFontConverter.Add(&HD7, &HFF52)
                mPKMFontConverter.Add(&HD8, &HFF53)
                mPKMFontConverter.Add(&HD9, &HFF54)
                mPKMFontConverter.Add(&HDA, &HFF55)
                mPKMFontConverter.Add(&HDB, &HFF56)
                mPKMFontConverter.Add(&HDC, &HFF57)
                mPKMFontConverter.Add(&HDD, &HFF58)
                mPKMFontConverter.Add(&HDE, &HFF59)
                mPKMFontConverter.Add(&HDF, &HFF5A)
                mPKMFontConverter.Add(&HE1, &HFF01)
                mPKMFontConverter.Add(&HE2, &HFF1F)
                mPKMFontConverter.Add(&HE3, &H3001)
                mPKMFontConverter.Add(&HE4, &H3002)
                mPKMFontConverter.Add(&HE5, &H22EF)
                mPKMFontConverter.Add(&HE6, &H30FB)
                mPKMFontConverter.Add(&HE7, &HFF0F)
                mPKMFontConverter.Add(&HE8, &H300C)
                mPKMFontConverter.Add(&HE9, &H300D)
                mPKMFontConverter.Add(&HEA, &H300E)
                mPKMFontConverter.Add(&HEB, &H300F)
                mPKMFontConverter.Add(&HEC, &HFF08)
                mPKMFontConverter.Add(&HED, &HFF09)
                mPKMFontConverter.Add(&HEE, &H329A)
                mPKMFontConverter.Add(&HEF, &H329B)
                mPKMFontConverter.Add(&HF0, &HFF0B)
                mPKMFontConverter.Add(&HF1, &HFF0D)
                mPKMFontConverter.Add(&HF2, &H2297)
                mPKMFontConverter.Add(&HF3, &H2298)
                mPKMFontConverter.Add(&HF4, &HFF1D)
                mPKMFontConverter.Add(&HF5, &HFF5A)
                mPKMFontConverter.Add(&HF6, &HFF1A)
                mPKMFontConverter.Add(&HF7, &HFF1B)
                mPKMFontConverter.Add(&HF8, &HFF0E)
                mPKMFontConverter.Add(&HF9, &HFF0C)
                mPKMFontConverter.Add(&HFA, &H2664)
                mPKMFontConverter.Add(&HFB, &H2667)
                mPKMFontConverter.Add(&HFC, &H2661)
                mPKMFontConverter.Add(&HFD, &H2662)
                mPKMFontConverter.Add(&HFE, &H2606)
                mPKMFontConverter.Add(&HFF, &H25CE)
                mPKMFontConverter.Add(&H100, &H25CB)
                mPKMFontConverter.Add(&H101, &H25A1)
                mPKMFontConverter.Add(&H102, &H25B3)
                mPKMFontConverter.Add(&H103, &H25C7)
                mPKMFontConverter.Add(&H104, &HFF20)
                mPKMFontConverter.Add(&H105, &H266B)
                mPKMFontConverter.Add(&H106, &HFF05)
                mPKMFontConverter.Add(&H107, &H263C)
                mPKMFontConverter.Add(&H108, &H2614)
                mPKMFontConverter.Add(&H109, &H2630)
                mPKMFontConverter.Add(&H10A, &H2744)
                mPKMFontConverter.Add(&H10B, &H260B)
                mPKMFontConverter.Add(&H10C, &H2654)
                mPKMFontConverter.Add(&H10D, &H2655)
                mPKMFontConverter.Add(&H10E, &H260A)
                mPKMFontConverter.Add(&H10F, &H21D7)
                mPKMFontConverter.Add(&H110, &H21D8)
                mPKMFontConverter.Add(&H111, &H263E)
                mPKMFontConverter.Add(&H112, &HA5)
                mPKMFontConverter.Add(&H113, &H2648)
                mPKMFontConverter.Add(&H114, &H2649)
                mPKMFontConverter.Add(&H115, &H264A)
                mPKMFontConverter.Add(&H116, &H264B)
                mPKMFontConverter.Add(&H117, &H264C)
                mPKMFontConverter.Add(&H118, &H264D)
                mPKMFontConverter.Add(&H119, &H264E)
                mPKMFontConverter.Add(&H11A, &H264F)
                mPKMFontConverter.Add(&H11B, &H2190)
                mPKMFontConverter.Add(&H11C, &H2191)
                mPKMFontConverter.Add(&H11D, &H2193)
                mPKMFontConverter.Add(&H11E, &H2192)
                mPKMFontConverter.Add(&H11F, &H2023)
                mPKMFontConverter.Add(&H120, &HFF06)
                mPKMFontConverter.Add(&H121, &H30)
                mPKMFontConverter.Add(&H122, &H31)
                mPKMFontConverter.Add(&H123, &H32)
                mPKMFontConverter.Add(&H124, &H33)
                mPKMFontConverter.Add(&H125, &H34)
                mPKMFontConverter.Add(&H126, &H35)
                mPKMFontConverter.Add(&H127, &H36)
                mPKMFontConverter.Add(&H128, &H37)
                mPKMFontConverter.Add(&H129, &H38)
                mPKMFontConverter.Add(&H12A, &H39)
                mPKMFontConverter.Add(&H12B, &H41)
                mPKMFontConverter.Add(&H12C, &H42)
                mPKMFontConverter.Add(&H12D, &H43)
                mPKMFontConverter.Add(&H12E, &H44)
                mPKMFontConverter.Add(&H12F, &H45)
                mPKMFontConverter.Add(&H130, &H46)
                mPKMFontConverter.Add(&H131, &H47)
                mPKMFontConverter.Add(&H132, &H48)
                mPKMFontConverter.Add(&H133, &H49)
                mPKMFontConverter.Add(&H134, &H4A)
                mPKMFontConverter.Add(&H135, &H4B)
                mPKMFontConverter.Add(&H136, &H4C)
                mPKMFontConverter.Add(&H137, &H4D)
                mPKMFontConverter.Add(&H138, &H4E)
                mPKMFontConverter.Add(&H139, &H4F)
                mPKMFontConverter.Add(&H13A, &H50)
                mPKMFontConverter.Add(&H13B, &H51)
                mPKMFontConverter.Add(&H13C, &H52)
                mPKMFontConverter.Add(&H13D, &H53)
                mPKMFontConverter.Add(&H13E, &H54)
                mPKMFontConverter.Add(&H13F, &H55)
                mPKMFontConverter.Add(&H140, &H56)
                mPKMFontConverter.Add(&H141, &H57)
                mPKMFontConverter.Add(&H142, &H58)
                mPKMFontConverter.Add(&H143, &H59)
                mPKMFontConverter.Add(&H144, &H5A)
                mPKMFontConverter.Add(&H145, &H61)
                mPKMFontConverter.Add(&H146, &H62)
                mPKMFontConverter.Add(&H147, &H63)
                mPKMFontConverter.Add(&H148, &H64)
                mPKMFontConverter.Add(&H149, &H65)
                mPKMFontConverter.Add(&H14A, &H66)
                mPKMFontConverter.Add(&H14B, &H67)
                mPKMFontConverter.Add(&H14C, &H68)
                mPKMFontConverter.Add(&H14D, &H69)
                mPKMFontConverter.Add(&H14E, &H6A)
                mPKMFontConverter.Add(&H14F, &H6B)
                mPKMFontConverter.Add(&H150, &H6C)
                mPKMFontConverter.Add(&H151, &H6D)
                mPKMFontConverter.Add(&H152, &H6E)
                mPKMFontConverter.Add(&H153, &H6F)
                mPKMFontConverter.Add(&H154, &H70)
                mPKMFontConverter.Add(&H155, &H71)
                mPKMFontConverter.Add(&H156, &H72)
                mPKMFontConverter.Add(&H157, &H73)
                mPKMFontConverter.Add(&H158, &H74)
                mPKMFontConverter.Add(&H159, &H75)
                mPKMFontConverter.Add(&H15A, &H76)
                mPKMFontConverter.Add(&H15B, &H77)
                mPKMFontConverter.Add(&H15C, &H78)
                mPKMFontConverter.Add(&H15D, &H79)
                mPKMFontConverter.Add(&H15E, &H7A)
                mPKMFontConverter.Add(&H15F, &HC0)
                mPKMFontConverter.Add(&H160, &HC1)
                mPKMFontConverter.Add(&H161, &HC2)
                mPKMFontConverter.Add(&H162, &HC3)
                mPKMFontConverter.Add(&H163, &HC4)
                mPKMFontConverter.Add(&H164, &HC5)
                mPKMFontConverter.Add(&H165, &HC6)
                mPKMFontConverter.Add(&H166, &HC7)
                mPKMFontConverter.Add(&H167, &HC8)
                mPKMFontConverter.Add(&H168, &HC9)
                mPKMFontConverter.Add(&H169, &HCA)
                mPKMFontConverter.Add(&H16A, &HCB)
                mPKMFontConverter.Add(&H16B, &HCC)
                mPKMFontConverter.Add(&H16C, &HCD)
                mPKMFontConverter.Add(&H16D, &HCE)
                mPKMFontConverter.Add(&H16E, &HCF)
                mPKMFontConverter.Add(&H16F, &HD0)
                mPKMFontConverter.Add(&H170, &HD1)
                mPKMFontConverter.Add(&H171, &HD2)
                mPKMFontConverter.Add(&H172, &HD3)
                mPKMFontConverter.Add(&H173, &HD4)
                mPKMFontConverter.Add(&H174, &HD5)
                mPKMFontConverter.Add(&H175, &HD6)
                mPKMFontConverter.Add(&H176, &HD7)
                mPKMFontConverter.Add(&H177, &HD8)
                mPKMFontConverter.Add(&H178, &HD9)
                mPKMFontConverter.Add(&H179, &HDA)
                mPKMFontConverter.Add(&H17A, &HDB)
                mPKMFontConverter.Add(&H17B, &HDC)
                mPKMFontConverter.Add(&H17C, &HDD)
                mPKMFontConverter.Add(&H17D, &HDE)
                mPKMFontConverter.Add(&H17E, &HDF)
                mPKMFontConverter.Add(&H17F, &HE0)
                mPKMFontConverter.Add(&H180, &HE1)
                mPKMFontConverter.Add(&H181, &HE2)
                mPKMFontConverter.Add(&H182, &HE3)
                mPKMFontConverter.Add(&H183, &HE4)
                mPKMFontConverter.Add(&H184, &HE5)
                mPKMFontConverter.Add(&H185, &HE6)
                mPKMFontConverter.Add(&H186, &HE7)
                mPKMFontConverter.Add(&H187, &HE8)
                mPKMFontConverter.Add(&H188, &HE9)
                mPKMFontConverter.Add(&H189, &HEA)
                mPKMFontConverter.Add(&H18A, &HEB)
                mPKMFontConverter.Add(&H18B, &HEC)
                mPKMFontConverter.Add(&H18C, &HED)
                mPKMFontConverter.Add(&H18D, &HEE)
                mPKMFontConverter.Add(&H18E, &HEF)
                mPKMFontConverter.Add(&H18F, &HF0)
                mPKMFontConverter.Add(&H190, &HF1)
                mPKMFontConverter.Add(&H191, &HF2)
                mPKMFontConverter.Add(&H192, &HF3)
                mPKMFontConverter.Add(&H193, &HF4)
                mPKMFontConverter.Add(&H194, &HF5)
                mPKMFontConverter.Add(&H195, &HF6)
                mPKMFontConverter.Add(&H196, &HF7)
                mPKMFontConverter.Add(&H197, &HF8)
                mPKMFontConverter.Add(&H198, &HF9)
                mPKMFontConverter.Add(&H199, &HFA)
                mPKMFontConverter.Add(&H19A, &HFB)
                mPKMFontConverter.Add(&H19B, &HFC)
                mPKMFontConverter.Add(&H19C, &HFD)
                mPKMFontConverter.Add(&H19D, &HFE)
                mPKMFontConverter.Add(&H19E, &HFF)
                mPKMFontConverter.Add(&H19F, &H152)
                mPKMFontConverter.Add(&H1A0, &H153)
                mPKMFontConverter.Add(&H1A1, &H15E)
                mPKMFontConverter.Add(&H1A2, &H15F)
                mPKMFontConverter.Add(&H1A3, &HAA)
                mPKMFontConverter.Add(&H1A4, &HBA)
                mPKMFontConverter.Add(&H1A5, &HB9)
                mPKMFontConverter.Add(&H1A6, &HB2)
                mPKMFontConverter.Add(&H1A7, &HB3)
                mPKMFontConverter.Add(&H1A8, &H24)
                mPKMFontConverter.Add(&H1A9, &HA1)
                mPKMFontConverter.Add(&H1AA, &HBF)
                mPKMFontConverter.Add(&H1AB, &H21)
                mPKMFontConverter.Add(&H1AC, &H3F)
                mPKMFontConverter.Add(&H1AD, &H2C)
                mPKMFontConverter.Add(&H1AE, &H2E)
                mPKMFontConverter.Add(&H1AF, &H2026)
                mPKMFontConverter.Add(&H1B0, &HB7)
                mPKMFontConverter.Add(&H1B1, &H2F)
                mPKMFontConverter.Add(&H1B2, &H2018)
                mPKMFontConverter.Add(&H1B3, &H2019)
                mPKMFontConverter.Add(&H1B4, &H201C)
                mPKMFontConverter.Add(&H1B5, &H201D)
                mPKMFontConverter.Add(&H1B6, &H201E)
                mPKMFontConverter.Add(&H1B7, &H300A)
                mPKMFontConverter.Add(&H1B8, &H300B)
                mPKMFontConverter.Add(&H1B9, &H28)
                mPKMFontConverter.Add(&H1BA, &H29)
                mPKMFontConverter.Add(&H1BB, &H2642)
                mPKMFontConverter.Add(&H1BC, &H2640)
                mPKMFontConverter.Add(&H1BD, &H2B)
                mPKMFontConverter.Add(&H1BE, &H2D)
                mPKMFontConverter.Add(&H1BF, &H2A)
                mPKMFontConverter.Add(&H1C0, &H23)
                mPKMFontConverter.Add(&H1C1, &H3D)
                mPKMFontConverter.Add(&H1C2, &H26)
                mPKMFontConverter.Add(&H1C3, &H7E)
                mPKMFontConverter.Add(&H1C4, &H3A)
                mPKMFontConverter.Add(&H1C5, &H3B)
                mPKMFontConverter.Add(&H1C6, &H2660)
                mPKMFontConverter.Add(&H1C7, &H2663)
                mPKMFontConverter.Add(&H1C8, &H2665)
                mPKMFontConverter.Add(&H1C9, &H2666)
                mPKMFontConverter.Add(&H1CA, &H2605)
                mPKMFontConverter.Add(&H1CB, &H25C9)
                mPKMFontConverter.Add(&H1CC, &H25CF)
                mPKMFontConverter.Add(&H1CD, &H25A0)
                mPKMFontConverter.Add(&H1CE, &H25B2)
                mPKMFontConverter.Add(&H1CF, &H25C6)
                mPKMFontConverter.Add(&H1D0, &H40)
                mPKMFontConverter.Add(&H1D1, &H266A)
                mPKMFontConverter.Add(&H1D2, &H25)
                mPKMFontConverter.Add(&H1D3, &H2600)
                mPKMFontConverter.Add(&H1D4, &H2601)
                mPKMFontConverter.Add(&H1D5, &H2602)
                mPKMFontConverter.Add(&H1D6, &H2603)
                mPKMFontConverter.Add(&H1D7, &H263A)
                mPKMFontConverter.Add(&H1D8, &H265A)
                mPKMFontConverter.Add(&H1D9, &H265B)
                mPKMFontConverter.Add(&H1DA, &H2639)
                mPKMFontConverter.Add(&H1DB, &H2197)
                mPKMFontConverter.Add(&H1DC, &H2198)
                mPKMFontConverter.Add(&H1DD, &H263D)
                mPKMFontConverter.Add(&H1DE, &H20)
                mPKMFontConverter.Add(&H1DF, &H2074)
                mPKMFontConverter.Add(&H1E0, &H20A7)
                mPKMFontConverter.Add(&H1E1, &H20A6)
                mPKMFontConverter.Add(&H1E8, &HB0)
                mPKMFontConverter.Add(&H1E9, &H5F)
                mPKMFontConverter.Add(&H1EA, &HFF3F)
                mPKMFontConverter.Add(&H400, &HAC00)
                mPKMFontConverter.Add(&H401, &HAC01)
                mPKMFontConverter.Add(&H402, &HAC04)
                mPKMFontConverter.Add(&H403, &HAC07)
                mPKMFontConverter.Add(&H404, &HAC08)
                mPKMFontConverter.Add(&H405, &HAC09)
                mPKMFontConverter.Add(&H406, &HAC0A)
                mPKMFontConverter.Add(&H407, &HAC10)
                mPKMFontConverter.Add(&H408, &HAC11)
                mPKMFontConverter.Add(&H409, &HAC12)
                mPKMFontConverter.Add(&H40A, &HAC13)
                mPKMFontConverter.Add(&H40B, &HAC14)
                mPKMFontConverter.Add(&H40C, &HAC15)
                mPKMFontConverter.Add(&H40D, &HAC16)
                mPKMFontConverter.Add(&H40E, &HAC17)
                mPKMFontConverter.Add(&H410, &HAC19)
                mPKMFontConverter.Add(&H411, &HAC1A)
                mPKMFontConverter.Add(&H412, &HAC1B)
                mPKMFontConverter.Add(&H413, &HAC1C)
                mPKMFontConverter.Add(&H414, &HAC1D)
                mPKMFontConverter.Add(&H415, &HAC20)
                mPKMFontConverter.Add(&H416, &HAC24)
                mPKMFontConverter.Add(&H417, &HAC2C)
                mPKMFontConverter.Add(&H418, &HAC2D)
                mPKMFontConverter.Add(&H419, &HAC2F)
                mPKMFontConverter.Add(&H41A, &HAC30)
                mPKMFontConverter.Add(&H41B, &HAC31)
                mPKMFontConverter.Add(&H41C, &HAC38)
                mPKMFontConverter.Add(&H41D, &HAC39)
                mPKMFontConverter.Add(&H41E, &HAC3C)
                mPKMFontConverter.Add(&H41F, &HAC40)
                mPKMFontConverter.Add(&H420, &HAC4B)
                mPKMFontConverter.Add(&H421, &HAC4D)
                mPKMFontConverter.Add(&H422, &HAC54)
                mPKMFontConverter.Add(&H423, &HAC58)
                mPKMFontConverter.Add(&H424, &HAC5C)
                mPKMFontConverter.Add(&H425, &HAC70)
                mPKMFontConverter.Add(&H426, &HAC71)
                mPKMFontConverter.Add(&H427, &HAC74)
                mPKMFontConverter.Add(&H428, &HAC77)
                mPKMFontConverter.Add(&H429, &HAC78)
                mPKMFontConverter.Add(&H42A, &HAC7A)
                mPKMFontConverter.Add(&H42B, &HAC80)
                mPKMFontConverter.Add(&H42C, &HAC81)
                mPKMFontConverter.Add(&H42D, &HAC83)
                mPKMFontConverter.Add(&H42E, &HAC84)
                mPKMFontConverter.Add(&H42F, &HAC85)
                mPKMFontConverter.Add(&H430, &HAC86)
                mPKMFontConverter.Add(&H431, &HAC89)
                mPKMFontConverter.Add(&H432, &HAC8A)
                mPKMFontConverter.Add(&H433, &HAC8B)
                mPKMFontConverter.Add(&H434, &HAC8C)
                mPKMFontConverter.Add(&H435, &HAC90)
                mPKMFontConverter.Add(&H436, &HAC94)
                mPKMFontConverter.Add(&H437, &HAC9C)
                mPKMFontConverter.Add(&H438, &HAC9D)
                mPKMFontConverter.Add(&H439, &HAC9F)
                mPKMFontConverter.Add(&H43A, &HACA0)
                mPKMFontConverter.Add(&H43B, &HACA1)
                mPKMFontConverter.Add(&H43C, &HACA8)
                mPKMFontConverter.Add(&H43D, &HACA9)
                mPKMFontConverter.Add(&H43E, &HACAA)
                mPKMFontConverter.Add(&H43F, &HACAC)
                mPKMFontConverter.Add(&H440, &HACAF)
                mPKMFontConverter.Add(&H441, &HACB0)
                mPKMFontConverter.Add(&H442, &HACB8)
                mPKMFontConverter.Add(&H443, &HACB9)
                mPKMFontConverter.Add(&H444, &HACBB)
                mPKMFontConverter.Add(&H445, &HACBC)
                mPKMFontConverter.Add(&H446, &HACBD)
                mPKMFontConverter.Add(&H447, &HACC1)
                mPKMFontConverter.Add(&H448, &HACC4)
                mPKMFontConverter.Add(&H449, &HACC8)
                mPKMFontConverter.Add(&H44A, &HACCC)
                mPKMFontConverter.Add(&H44B, &HACD5)
                mPKMFontConverter.Add(&H44C, &HACD7)
                mPKMFontConverter.Add(&H44D, &HACE0)
                mPKMFontConverter.Add(&H44E, &HACE1)
                mPKMFontConverter.Add(&H44F, &HACE4)
                mPKMFontConverter.Add(&H450, &HACE7)
                mPKMFontConverter.Add(&H451, &HACE8)
                mPKMFontConverter.Add(&H452, &HACEA)
                mPKMFontConverter.Add(&H453, &HACEC)
                mPKMFontConverter.Add(&H454, &HACEF)
                mPKMFontConverter.Add(&H455, &HACF0)
                mPKMFontConverter.Add(&H456, &HACF1)
                mPKMFontConverter.Add(&H457, &HACF3)
                mPKMFontConverter.Add(&H458, &HACF5)
                mPKMFontConverter.Add(&H459, &HACF6)
                mPKMFontConverter.Add(&H45A, &HACFC)
                mPKMFontConverter.Add(&H45B, &HACFD)
                mPKMFontConverter.Add(&H45C, &HAD00)
                mPKMFontConverter.Add(&H45D, &HAD04)
                mPKMFontConverter.Add(&H45E, &HAD06)
                mPKMFontConverter.Add(&H45F, &HAD0C)
                mPKMFontConverter.Add(&H460, &HAD0D)
                mPKMFontConverter.Add(&H461, &HAD0F)
                mPKMFontConverter.Add(&H462, &HAD11)
                mPKMFontConverter.Add(&H463, &HAD18)
                mPKMFontConverter.Add(&H464, &HAD1C)
                mPKMFontConverter.Add(&H465, &HAD20)
                mPKMFontConverter.Add(&H466, &HAD29)
                mPKMFontConverter.Add(&H467, &HAD2C)
                mPKMFontConverter.Add(&H468, &HAD2D)
                mPKMFontConverter.Add(&H469, &HAD34)
                mPKMFontConverter.Add(&H46A, &HAD35)
                mPKMFontConverter.Add(&H46B, &HAD38)
                mPKMFontConverter.Add(&H46C, &HAD3C)
                mPKMFontConverter.Add(&H46D, &HAD44)
                mPKMFontConverter.Add(&H46E, &HAD45)
                mPKMFontConverter.Add(&H46F, &HAD47)
                mPKMFontConverter.Add(&H470, &HAD49)
                mPKMFontConverter.Add(&H471, &HAD50)
                mPKMFontConverter.Add(&H472, &HAD54)
                mPKMFontConverter.Add(&H473, &HAD58)
                mPKMFontConverter.Add(&H474, &HAD61)
                mPKMFontConverter.Add(&H475, &HAD63)
                mPKMFontConverter.Add(&H476, &HAD6C)
                mPKMFontConverter.Add(&H477, &HAD6D)
                mPKMFontConverter.Add(&H478, &HAD70)
                mPKMFontConverter.Add(&H479, &HAD73)
                mPKMFontConverter.Add(&H47A, &HAD74)
                mPKMFontConverter.Add(&H47B, &HAD75)
                mPKMFontConverter.Add(&H47C, &HAD76)
                mPKMFontConverter.Add(&H47D, &HAD7B)
                mPKMFontConverter.Add(&H47E, &HAD7C)
                mPKMFontConverter.Add(&H47F, &HAD7D)
                mPKMFontConverter.Add(&H480, &HAD7F)
                mPKMFontConverter.Add(&H481, &HAD81)
                mPKMFontConverter.Add(&H482, &HAD82)
                mPKMFontConverter.Add(&H483, &HAD88)
                mPKMFontConverter.Add(&H484, &HAD89)
                mPKMFontConverter.Add(&H485, &HAD8C)
                mPKMFontConverter.Add(&H486, &HAD90)
                mPKMFontConverter.Add(&H487, &HAD9C)
                mPKMFontConverter.Add(&H488, &HAD9D)
                mPKMFontConverter.Add(&H489, &HADA4)
                mPKMFontConverter.Add(&H48A, &HADB7)
                mPKMFontConverter.Add(&H48B, &HADC0)
                mPKMFontConverter.Add(&H48C, &HADC1)
                mPKMFontConverter.Add(&H48D, &HADC4)
                mPKMFontConverter.Add(&H48E, &HADC8)
                mPKMFontConverter.Add(&H48F, &HADD0)
                mPKMFontConverter.Add(&H490, &HADD1)
                mPKMFontConverter.Add(&H491, &HADD3)
                mPKMFontConverter.Add(&H492, &HADDC)
                mPKMFontConverter.Add(&H493, &HADE0)
                mPKMFontConverter.Add(&H494, &HADE4)
                mPKMFontConverter.Add(&H495, &HADF8)
                mPKMFontConverter.Add(&H496, &HADF9)
                mPKMFontConverter.Add(&H497, &HADFC)
                mPKMFontConverter.Add(&H498, &HADFF)
                mPKMFontConverter.Add(&H499, &HAE00)
                mPKMFontConverter.Add(&H49A, &HAE01)
                mPKMFontConverter.Add(&H49B, &HAE08)
                mPKMFontConverter.Add(&H49C, &HAE09)
                mPKMFontConverter.Add(&H49D, &HAE0B)
                mPKMFontConverter.Add(&H49E, &HAE0D)
                mPKMFontConverter.Add(&H49F, &HAE14)
                mPKMFontConverter.Add(&H4A0, &HAE30)
                mPKMFontConverter.Add(&H4A1, &HAE31)
                mPKMFontConverter.Add(&H4A2, &HAE34)
                mPKMFontConverter.Add(&H4A3, &HAE37)
                mPKMFontConverter.Add(&H4A4, &HAE38)
                mPKMFontConverter.Add(&H4A5, &HAE3A)
                mPKMFontConverter.Add(&H4A6, &HAE40)
                mPKMFontConverter.Add(&H4A7, &HAE41)
                mPKMFontConverter.Add(&H4A8, &HAE43)
                mPKMFontConverter.Add(&H4A9, &HAE45)
                mPKMFontConverter.Add(&H4AA, &HAE46)
                mPKMFontConverter.Add(&H4AB, &HAE4A)
                mPKMFontConverter.Add(&H4AC, &HAE4C)
                mPKMFontConverter.Add(&H4AD, &HAE4D)
                mPKMFontConverter.Add(&H4AE, &HAE4E)
                mPKMFontConverter.Add(&H4AF, &HAE50)
                mPKMFontConverter.Add(&H4B0, &HAE54)
                mPKMFontConverter.Add(&H4B1, &HAE56)
                mPKMFontConverter.Add(&H4B2, &HAE5C)
                mPKMFontConverter.Add(&H4B3, &HAE5D)
                mPKMFontConverter.Add(&H4B4, &HAE5F)
                mPKMFontConverter.Add(&H4B5, &HAE60)
                mPKMFontConverter.Add(&H4B6, &HAE61)
                mPKMFontConverter.Add(&H4B7, &HAE65)
                mPKMFontConverter.Add(&H4B8, &HAE68)
                mPKMFontConverter.Add(&H4B9, &HAE69)
                mPKMFontConverter.Add(&H4BA, &HAE6C)
                mPKMFontConverter.Add(&H4BB, &HAE70)
                mPKMFontConverter.Add(&H4BC, &HAE78)
                mPKMFontConverter.Add(&H4BD, &HAE79)
                mPKMFontConverter.Add(&H4BE, &HAE7B)
                mPKMFontConverter.Add(&H4BF, &HAE7C)
                mPKMFontConverter.Add(&H4C0, &HAE7D)
                mPKMFontConverter.Add(&H4C1, &HAE84)
                mPKMFontConverter.Add(&H4C2, &HAE85)
                mPKMFontConverter.Add(&H4C3, &HAE8C)
                mPKMFontConverter.Add(&H4C4, &HAEBC)
                mPKMFontConverter.Add(&H4C5, &HAEBD)
                mPKMFontConverter.Add(&H4C6, &HAEBE)
                mPKMFontConverter.Add(&H4C7, &HAEC0)
                mPKMFontConverter.Add(&H4C8, &HAEC4)
                mPKMFontConverter.Add(&H4C9, &HAECC)
                mPKMFontConverter.Add(&H4CA, &HAECD)
                mPKMFontConverter.Add(&H4CB, &HAECF)
                mPKMFontConverter.Add(&H4CC, &HAED0)
                mPKMFontConverter.Add(&H4CD, &HAED1)
                mPKMFontConverter.Add(&H4CE, &HAED8)
                mPKMFontConverter.Add(&H4CF, &HAED9)
                mPKMFontConverter.Add(&H4D0, &HAEDC)
                mPKMFontConverter.Add(&H4D1, &HAEE8)
                mPKMFontConverter.Add(&H4D2, &HAEEB)
                mPKMFontConverter.Add(&H4D3, &HAEED)
                mPKMFontConverter.Add(&H4D4, &HAEF4)
                mPKMFontConverter.Add(&H4D5, &HAEF8)
                mPKMFontConverter.Add(&H4D6, &HAEFC)
                mPKMFontConverter.Add(&H4D7, &HAF07)
                mPKMFontConverter.Add(&H4D8, &HAF08)
                mPKMFontConverter.Add(&H4D9, &HAF0D)
                mPKMFontConverter.Add(&H4DA, &HAF10)
                mPKMFontConverter.Add(&H4DB, &HAF2C)
                mPKMFontConverter.Add(&H4DC, &HAF2D)
                mPKMFontConverter.Add(&H4DD, &HAF30)
                mPKMFontConverter.Add(&H4DE, &HAF32)
                mPKMFontConverter.Add(&H4DF, &HAF34)
                mPKMFontConverter.Add(&H4E0, &HAF3C)
                mPKMFontConverter.Add(&H4E1, &HAF3D)
                mPKMFontConverter.Add(&H4E2, &HAF3F)
                mPKMFontConverter.Add(&H4E3, &HAF41)
                mPKMFontConverter.Add(&H4E4, &HAF42)
                mPKMFontConverter.Add(&H4E5, &HAF43)
                mPKMFontConverter.Add(&H4E6, &HAF48)
                mPKMFontConverter.Add(&H4E7, &HAF49)
                mPKMFontConverter.Add(&H4E8, &HAF50)
                mPKMFontConverter.Add(&H4E9, &HAF5C)
                mPKMFontConverter.Add(&H4EA, &HAF5D)
                mPKMFontConverter.Add(&H4EB, &HAF64)
                mPKMFontConverter.Add(&H4EC, &HAF65)
                mPKMFontConverter.Add(&H4ED, &HAF79)
                mPKMFontConverter.Add(&H4EE, &HAF80)
                mPKMFontConverter.Add(&H4EF, &HAF84)
                mPKMFontConverter.Add(&H4F0, &HAF88)
                mPKMFontConverter.Add(&H4F1, &HAF90)
                mPKMFontConverter.Add(&H4F2, &HAF91)
                mPKMFontConverter.Add(&H4F3, &HAF95)
                mPKMFontConverter.Add(&H4F4, &HAF9C)
                mPKMFontConverter.Add(&H4F5, &HAFB8)
                mPKMFontConverter.Add(&H4F6, &HAFB9)
                mPKMFontConverter.Add(&H4F7, &HAFBC)
                mPKMFontConverter.Add(&H4F8, &HAFC0)
                mPKMFontConverter.Add(&H4F9, &HAFC7)
                mPKMFontConverter.Add(&H4FA, &HAFC8)
                mPKMFontConverter.Add(&H4FB, &HAFC9)
                mPKMFontConverter.Add(&H4FC, &HAFCB)
                mPKMFontConverter.Add(&H4FD, &HAFCD)
                mPKMFontConverter.Add(&H4FE, &HAFCE)
                mPKMFontConverter.Add(&H4FF, &HAFD4)
                mPKMFontConverter.Add(&H500, &HAFDC)
                mPKMFontConverter.Add(&H501, &HAFE8)
                mPKMFontConverter.Add(&H502, &HAFE9)
                mPKMFontConverter.Add(&H503, &HAFF0)
                mPKMFontConverter.Add(&H504, &HAFF1)
                mPKMFontConverter.Add(&H505, &HAFF4)
                mPKMFontConverter.Add(&H506, &HAFF8)
                mPKMFontConverter.Add(&H507, &HB000)
                mPKMFontConverter.Add(&H508, &HB001)
                mPKMFontConverter.Add(&H509, &HB004)
                mPKMFontConverter.Add(&H50A, &HB00C)
                mPKMFontConverter.Add(&H50B, &HB010)
                mPKMFontConverter.Add(&H50C, &HB014)
                mPKMFontConverter.Add(&H50D, &HB01C)
                mPKMFontConverter.Add(&H50E, &HB01D)
                mPKMFontConverter.Add(&H50F, &HB028)
                mPKMFontConverter.Add(&H510, &HB044)
                mPKMFontConverter.Add(&H511, &HB045)
                mPKMFontConverter.Add(&H512, &HB048)
                mPKMFontConverter.Add(&H513, &HB04A)
                mPKMFontConverter.Add(&H514, &HB04C)
                mPKMFontConverter.Add(&H515, &HB04E)
                mPKMFontConverter.Add(&H516, &HB053)
                mPKMFontConverter.Add(&H517, &HB054)
                mPKMFontConverter.Add(&H518, &HB055)
                mPKMFontConverter.Add(&H519, &HB057)
                mPKMFontConverter.Add(&H51A, &HB059)
                mPKMFontConverter.Add(&H51B, &HB05D)
                mPKMFontConverter.Add(&H51C, &HB07C)
                mPKMFontConverter.Add(&H51D, &HB07D)
                mPKMFontConverter.Add(&H51E, &HB080)
                mPKMFontConverter.Add(&H51F, &HB084)
                mPKMFontConverter.Add(&H520, &HB08C)
                mPKMFontConverter.Add(&H521, &HB08D)
                mPKMFontConverter.Add(&H522, &HB08F)
                mPKMFontConverter.Add(&H523, &HB091)
                mPKMFontConverter.Add(&H524, &HB098)
                mPKMFontConverter.Add(&H525, &HB099)
                mPKMFontConverter.Add(&H526, &HB09A)
                mPKMFontConverter.Add(&H527, &HB09C)
                mPKMFontConverter.Add(&H528, &HB09F)
                mPKMFontConverter.Add(&H529, &HB0A0)
                mPKMFontConverter.Add(&H52A, &HB0A1)
                mPKMFontConverter.Add(&H52B, &HB0A2)
                mPKMFontConverter.Add(&H52C, &HB0A8)
                mPKMFontConverter.Add(&H52D, &HB0A9)
                mPKMFontConverter.Add(&H52E, &HB0AB)
                mPKMFontConverter.Add(&H52F, &HB0AC)
                mPKMFontConverter.Add(&H530, &HB0AD)
                mPKMFontConverter.Add(&H531, &HB0AE)
                mPKMFontConverter.Add(&H532, &HB0AF)
                mPKMFontConverter.Add(&H533, &HB0B1)
                mPKMFontConverter.Add(&H534, &HB0B3)
                mPKMFontConverter.Add(&H535, &HB0B4)
                mPKMFontConverter.Add(&H536, &HB0B5)
                mPKMFontConverter.Add(&H537, &HB0B8)
                mPKMFontConverter.Add(&H538, &HB0BC)
                mPKMFontConverter.Add(&H539, &HB0C4)
                mPKMFontConverter.Add(&H53A, &HB0C5)
                mPKMFontConverter.Add(&H53B, &HB0C7)
                mPKMFontConverter.Add(&H53C, &HB0C8)
                mPKMFontConverter.Add(&H53D, &HB0C9)
                mPKMFontConverter.Add(&H53E, &HB0D0)
                mPKMFontConverter.Add(&H53F, &HB0D1)
                mPKMFontConverter.Add(&H540, &HB0D4)
                mPKMFontConverter.Add(&H541, &HB0D8)
                mPKMFontConverter.Add(&H542, &HB0E0)
                mPKMFontConverter.Add(&H543, &HB0E5)
                mPKMFontConverter.Add(&H544, &HB108)
                mPKMFontConverter.Add(&H545, &HB109)
                mPKMFontConverter.Add(&H546, &HB10B)
                mPKMFontConverter.Add(&H547, &HB10C)
                mPKMFontConverter.Add(&H548, &HB110)
                mPKMFontConverter.Add(&H549, &HB112)
                mPKMFontConverter.Add(&H54A, &HB113)
                mPKMFontConverter.Add(&H54B, &HB118)
                mPKMFontConverter.Add(&H54C, &HB119)
                mPKMFontConverter.Add(&H54D, &HB11B)
                mPKMFontConverter.Add(&H54E, &HB11C)
                mPKMFontConverter.Add(&H54F, &HB11D)
                mPKMFontConverter.Add(&H550, &HB123)
                mPKMFontConverter.Add(&H551, &HB124)
                mPKMFontConverter.Add(&H552, &HB125)
                mPKMFontConverter.Add(&H553, &HB128)
                mPKMFontConverter.Add(&H554, &HB12C)
                mPKMFontConverter.Add(&H555, &HB134)
                mPKMFontConverter.Add(&H556, &HB135)
                mPKMFontConverter.Add(&H557, &HB137)
                mPKMFontConverter.Add(&H558, &HB138)
                mPKMFontConverter.Add(&H559, &HB139)
                mPKMFontConverter.Add(&H55A, &HB140)
                mPKMFontConverter.Add(&H55B, &HB141)
                mPKMFontConverter.Add(&H55C, &HB144)
                mPKMFontConverter.Add(&H55D, &HB148)
                mPKMFontConverter.Add(&H55E, &HB150)
                mPKMFontConverter.Add(&H55F, &HB151)
                mPKMFontConverter.Add(&H560, &HB154)
                mPKMFontConverter.Add(&H561, &HB155)
                mPKMFontConverter.Add(&H562, &HB158)
                mPKMFontConverter.Add(&H563, &HB15C)
                mPKMFontConverter.Add(&H564, &HB160)
                mPKMFontConverter.Add(&H565, &HB178)
                mPKMFontConverter.Add(&H566, &HB179)
                mPKMFontConverter.Add(&H567, &HB17C)
                mPKMFontConverter.Add(&H568, &HB180)
                mPKMFontConverter.Add(&H569, &HB182)
                mPKMFontConverter.Add(&H56A, &HB188)
                mPKMFontConverter.Add(&H56B, &HB189)
                mPKMFontConverter.Add(&H56C, &HB18B)
                mPKMFontConverter.Add(&H56D, &HB18D)
                mPKMFontConverter.Add(&H56E, &HB192)
                mPKMFontConverter.Add(&H56F, &HB193)
                mPKMFontConverter.Add(&H570, &HB194)
                mPKMFontConverter.Add(&H571, &HB198)
                mPKMFontConverter.Add(&H572, &HB19C)
                mPKMFontConverter.Add(&H573, &HB1A8)
                mPKMFontConverter.Add(&H574, &HB1CC)
                mPKMFontConverter.Add(&H575, &HB1D0)
                mPKMFontConverter.Add(&H576, &HB1D4)
                mPKMFontConverter.Add(&H577, &HB1DC)
                mPKMFontConverter.Add(&H578, &HB1DD)
                mPKMFontConverter.Add(&H579, &HB1DF)
                mPKMFontConverter.Add(&H57A, &HB1E8)
                mPKMFontConverter.Add(&H57B, &HB1E9)
                mPKMFontConverter.Add(&H57C, &HB1EC)
                mPKMFontConverter.Add(&H57D, &HB1F0)
                mPKMFontConverter.Add(&H57E, &HB1F9)
                mPKMFontConverter.Add(&H57F, &HB1FB)
                mPKMFontConverter.Add(&H580, &HB1FD)
                mPKMFontConverter.Add(&H581, &HB204)
                mPKMFontConverter.Add(&H582, &HB205)
                mPKMFontConverter.Add(&H583, &HB208)
                mPKMFontConverter.Add(&H584, &HB20B)
                mPKMFontConverter.Add(&H585, &HB20C)
                mPKMFontConverter.Add(&H586, &HB214)
                mPKMFontConverter.Add(&H587, &HB215)
                mPKMFontConverter.Add(&H588, &HB217)
                mPKMFontConverter.Add(&H589, &HB219)
                mPKMFontConverter.Add(&H58A, &HB220)
                mPKMFontConverter.Add(&H58B, &HB234)
                mPKMFontConverter.Add(&H58C, &HB23C)
                mPKMFontConverter.Add(&H58D, &HB258)
                mPKMFontConverter.Add(&H58E, &HB25C)
                mPKMFontConverter.Add(&H58F, &HB260)
                mPKMFontConverter.Add(&H590, &HB268)
                mPKMFontConverter.Add(&H591, &HB269)
                mPKMFontConverter.Add(&H592, &HB274)
                mPKMFontConverter.Add(&H593, &HB275)
                mPKMFontConverter.Add(&H594, &HB27C)
                mPKMFontConverter.Add(&H595, &HB284)
                mPKMFontConverter.Add(&H596, &HB285)
                mPKMFontConverter.Add(&H597, &HB289)
                mPKMFontConverter.Add(&H598, &HB290)
                mPKMFontConverter.Add(&H599, &HB291)
                mPKMFontConverter.Add(&H59A, &HB294)
                mPKMFontConverter.Add(&H59B, &HB298)
                mPKMFontConverter.Add(&H59C, &HB299)
                mPKMFontConverter.Add(&H59D, &HB29A)
                mPKMFontConverter.Add(&H59E, &HB2A0)
                mPKMFontConverter.Add(&H59F, &HB2A1)
                mPKMFontConverter.Add(&H5A0, &HB2A3)
                mPKMFontConverter.Add(&H5A1, &HB2A5)
                mPKMFontConverter.Add(&H5A2, &HB2A6)
                mPKMFontConverter.Add(&H5A3, &HB2AA)
                mPKMFontConverter.Add(&H5A4, &HB2AC)
                mPKMFontConverter.Add(&H5A5, &HB2B0)
                mPKMFontConverter.Add(&H5A6, &HB2B4)
                mPKMFontConverter.Add(&H5A7, &HB2C8)
                mPKMFontConverter.Add(&H5A8, &HB2C9)
                mPKMFontConverter.Add(&H5A9, &HB2CC)
                mPKMFontConverter.Add(&H5AA, &HB2D0)
                mPKMFontConverter.Add(&H5AB, &HB2D2)
                mPKMFontConverter.Add(&H5AC, &HB2D8)
                mPKMFontConverter.Add(&H5AD, &HB2D9)
                mPKMFontConverter.Add(&H5AE, &HB2DB)
                mPKMFontConverter.Add(&H5AF, &HB2DD)
                mPKMFontConverter.Add(&H5B0, &HB2E2)
                mPKMFontConverter.Add(&H5B1, &HB2E4)
                mPKMFontConverter.Add(&H5B2, &HB2E5)
                mPKMFontConverter.Add(&H5B3, &HB2E6)
                mPKMFontConverter.Add(&H5B4, &HB2E8)
                mPKMFontConverter.Add(&H5B5, &HB2EB)
                mPKMFontConverter.Add(&H5B6, &HB2EC)
                mPKMFontConverter.Add(&H5B7, &HB2ED)
                mPKMFontConverter.Add(&H5B8, &HB2EE)
                mPKMFontConverter.Add(&H5B9, &HB2EF)
                mPKMFontConverter.Add(&H5BA, &HB2F3)
                mPKMFontConverter.Add(&H5BB, &HB2F4)
                mPKMFontConverter.Add(&H5BC, &HB2F5)
                mPKMFontConverter.Add(&H5BD, &HB2F7)
                mPKMFontConverter.Add(&H5BE, &HB2F8)
                mPKMFontConverter.Add(&H5BF, &HB2F9)
                mPKMFontConverter.Add(&H5C0, &HB2FA)
                mPKMFontConverter.Add(&H5C1, &HB2FB)
                mPKMFontConverter.Add(&H5C2, &HB2FF)
                mPKMFontConverter.Add(&H5C3, &HB300)
                mPKMFontConverter.Add(&H5C4, &HB301)
                mPKMFontConverter.Add(&H5C5, &HB304)
                mPKMFontConverter.Add(&H5C6, &HB308)
                mPKMFontConverter.Add(&H5C7, &HB310)
                mPKMFontConverter.Add(&H5C8, &HB311)
                mPKMFontConverter.Add(&H5C9, &HB313)
                mPKMFontConverter.Add(&H5CA, &HB314)
                mPKMFontConverter.Add(&H5CB, &HB315)
                mPKMFontConverter.Add(&H5CC, &HB31C)
                mPKMFontConverter.Add(&H5CD, &HB354)
                mPKMFontConverter.Add(&H5CE, &HB355)
                mPKMFontConverter.Add(&H5CF, &HB356)
                mPKMFontConverter.Add(&H5D0, &HB358)
                mPKMFontConverter.Add(&H5D1, &HB35B)
                mPKMFontConverter.Add(&H5D2, &HB35C)
                mPKMFontConverter.Add(&H5D3, &HB35E)
                mPKMFontConverter.Add(&H5D4, &HB35F)
                mPKMFontConverter.Add(&H5D5, &HB364)
                mPKMFontConverter.Add(&H5D6, &HB365)
                mPKMFontConverter.Add(&H5D7, &HB367)
                mPKMFontConverter.Add(&H5D8, &HB369)
                mPKMFontConverter.Add(&H5D9, &HB36B)
                mPKMFontConverter.Add(&H5DA, &HB36E)
                mPKMFontConverter.Add(&H5DB, &HB370)
                mPKMFontConverter.Add(&H5DC, &HB371)
                mPKMFontConverter.Add(&H5DD, &HB374)
                mPKMFontConverter.Add(&H5DE, &HB378)
                mPKMFontConverter.Add(&H5DF, &HB380)
                mPKMFontConverter.Add(&H5E0, &HB381)
                mPKMFontConverter.Add(&H5E1, &HB383)
                mPKMFontConverter.Add(&H5E2, &HB384)
                mPKMFontConverter.Add(&H5E3, &HB385)
                mPKMFontConverter.Add(&H5E4, &HB38C)
                mPKMFontConverter.Add(&H5E5, &HB390)
                mPKMFontConverter.Add(&H5E6, &HB394)
                mPKMFontConverter.Add(&H5E7, &HB3A0)
                mPKMFontConverter.Add(&H5E8, &HB3A1)
                mPKMFontConverter.Add(&H5E9, &HB3A8)
                mPKMFontConverter.Add(&H5EA, &HB3AC)
                mPKMFontConverter.Add(&H5EB, &HB3C4)
                mPKMFontConverter.Add(&H5EC, &HB3C5)
                mPKMFontConverter.Add(&H5ED, &HB3C8)
                mPKMFontConverter.Add(&H5EE, &HB3CB)
                mPKMFontConverter.Add(&H5EF, &HB3CC)
                mPKMFontConverter.Add(&H5F0, &HB3CE)
                mPKMFontConverter.Add(&H5F1, &HB3D0)
                mPKMFontConverter.Add(&H5F2, &HB3D4)
                mPKMFontConverter.Add(&H5F3, &HB3D5)
                mPKMFontConverter.Add(&H5F4, &HB3D7)
                mPKMFontConverter.Add(&H5F5, &HB3D9)
                mPKMFontConverter.Add(&H5F6, &HB3DB)
                mPKMFontConverter.Add(&H5F7, &HB3DD)
                mPKMFontConverter.Add(&H5F8, &HB3E0)
                mPKMFontConverter.Add(&H5F9, &HB3E4)
                mPKMFontConverter.Add(&H5FA, &HB3E8)
                mPKMFontConverter.Add(&H5FB, &HB3FC)
                mPKMFontConverter.Add(&H5FC, &HB410)
                mPKMFontConverter.Add(&H5FD, &HB418)
                mPKMFontConverter.Add(&H5FE, &HB41C)
                mPKMFontConverter.Add(&H5FF, &HB420)
                mPKMFontConverter.Add(&H600, &HB428)
                mPKMFontConverter.Add(&H601, &HB429)
                mPKMFontConverter.Add(&H602, &HB42B)
                mPKMFontConverter.Add(&H603, &HB434)
                mPKMFontConverter.Add(&H604, &HB450)
                mPKMFontConverter.Add(&H605, &HB451)
                mPKMFontConverter.Add(&H606, &HB454)
                mPKMFontConverter.Add(&H607, &HB458)
                mPKMFontConverter.Add(&H608, &HB460)
                mPKMFontConverter.Add(&H609, &HB461)
                mPKMFontConverter.Add(&H60A, &HB463)
                mPKMFontConverter.Add(&H60B, &HB465)
                mPKMFontConverter.Add(&H60C, &HB46C)
                mPKMFontConverter.Add(&H60D, &HB480)
                mPKMFontConverter.Add(&H60E, &HB488)
                mPKMFontConverter.Add(&H60F, &HB49D)
                mPKMFontConverter.Add(&H610, &HB4A4)
                mPKMFontConverter.Add(&H611, &HB4A8)
                mPKMFontConverter.Add(&H612, &HB4AC)
                mPKMFontConverter.Add(&H613, &HB4B5)
                mPKMFontConverter.Add(&H614, &HB4B7)
                mPKMFontConverter.Add(&H615, &HB4B9)
                mPKMFontConverter.Add(&H616, &HB4C0)
                mPKMFontConverter.Add(&H617, &HB4C4)
                mPKMFontConverter.Add(&H618, &HB4C8)
                mPKMFontConverter.Add(&H619, &HB4D0)
                mPKMFontConverter.Add(&H61A, &HB4D5)
                mPKMFontConverter.Add(&H61B, &HB4DC)
                mPKMFontConverter.Add(&H61C, &HB4DD)
                mPKMFontConverter.Add(&H61D, &HB4E0)
                mPKMFontConverter.Add(&H61E, &HB4E3)
                mPKMFontConverter.Add(&H61F, &HB4E4)
                mPKMFontConverter.Add(&H620, &HB4E6)
                mPKMFontConverter.Add(&H621, &HB4EC)
                mPKMFontConverter.Add(&H622, &HB4ED)
                mPKMFontConverter.Add(&H623, &HB4EF)
                mPKMFontConverter.Add(&H624, &HB4F1)
                mPKMFontConverter.Add(&H625, &HB4F8)
                mPKMFontConverter.Add(&H626, &HB514)
                mPKMFontConverter.Add(&H627, &HB515)
                mPKMFontConverter.Add(&H628, &HB518)
                mPKMFontConverter.Add(&H629, &HB51B)
                mPKMFontConverter.Add(&H62A, &HB51C)
                mPKMFontConverter.Add(&H62B, &HB524)
                mPKMFontConverter.Add(&H62C, &HB525)
                mPKMFontConverter.Add(&H62D, &HB527)
                mPKMFontConverter.Add(&H62E, &HB528)
                mPKMFontConverter.Add(&H62F, &HB529)
                mPKMFontConverter.Add(&H630, &HB52A)
                mPKMFontConverter.Add(&H631, &HB530)
                mPKMFontConverter.Add(&H632, &HB531)
                mPKMFontConverter.Add(&H633, &HB534)
                mPKMFontConverter.Add(&H634, &HB538)
                mPKMFontConverter.Add(&H635, &HB540)
                mPKMFontConverter.Add(&H636, &HB541)
                mPKMFontConverter.Add(&H637, &HB543)
                mPKMFontConverter.Add(&H638, &HB544)
                mPKMFontConverter.Add(&H639, &HB545)
                mPKMFontConverter.Add(&H63A, &HB54B)
                mPKMFontConverter.Add(&H63B, &HB54C)
                mPKMFontConverter.Add(&H63C, &HB54D)
                mPKMFontConverter.Add(&H63D, &HB550)
                mPKMFontConverter.Add(&H63E, &HB554)
                mPKMFontConverter.Add(&H63F, &HB55C)
                mPKMFontConverter.Add(&H640, &HB55D)
                mPKMFontConverter.Add(&H641, &HB55F)
                mPKMFontConverter.Add(&H642, &HB560)
                mPKMFontConverter.Add(&H643, &HB561)
                mPKMFontConverter.Add(&H644, &HB5A0)
                mPKMFontConverter.Add(&H645, &HB5A1)
                mPKMFontConverter.Add(&H646, &HB5A4)
                mPKMFontConverter.Add(&H647, &HB5A8)
                mPKMFontConverter.Add(&H648, &HB5AA)
                mPKMFontConverter.Add(&H649, &HB5AB)
                mPKMFontConverter.Add(&H64A, &HB5B0)
                mPKMFontConverter.Add(&H64B, &HB5B1)
                mPKMFontConverter.Add(&H64C, &HB5B3)
                mPKMFontConverter.Add(&H64D, &HB5B4)
                mPKMFontConverter.Add(&H64E, &HB5B5)
                mPKMFontConverter.Add(&H64F, &HB5BB)
                mPKMFontConverter.Add(&H650, &HB5BC)
                mPKMFontConverter.Add(&H651, &HB5BD)
                mPKMFontConverter.Add(&H652, &HB5C0)
                mPKMFontConverter.Add(&H653, &HB5C4)
                mPKMFontConverter.Add(&H654, &HB5CC)
                mPKMFontConverter.Add(&H655, &HB5CD)
                mPKMFontConverter.Add(&H656, &HB5CF)
                mPKMFontConverter.Add(&H657, &HB5D0)
                mPKMFontConverter.Add(&H658, &HB5D1)
                mPKMFontConverter.Add(&H659, &HB5D8)
                mPKMFontConverter.Add(&H65A, &HB5EC)
                mPKMFontConverter.Add(&H65B, &HB610)
                mPKMFontConverter.Add(&H65C, &HB611)
                mPKMFontConverter.Add(&H65D, &HB614)
                mPKMFontConverter.Add(&H65E, &HB618)
                mPKMFontConverter.Add(&H65F, &HB625)
                mPKMFontConverter.Add(&H660, &HB62C)
                mPKMFontConverter.Add(&H661, &HB634)
                mPKMFontConverter.Add(&H662, &HB648)
                mPKMFontConverter.Add(&H663, &HB664)
                mPKMFontConverter.Add(&H664, &HB668)
                mPKMFontConverter.Add(&H665, &HB69C)
                mPKMFontConverter.Add(&H666, &HB69D)
                mPKMFontConverter.Add(&H667, &HB6A0)
                mPKMFontConverter.Add(&H668, &HB6A4)
                mPKMFontConverter.Add(&H669, &HB6AB)
                mPKMFontConverter.Add(&H66A, &HB6AC)
                mPKMFontConverter.Add(&H66B, &HB6B1)
                mPKMFontConverter.Add(&H66C, &HB6D4)
                mPKMFontConverter.Add(&H66D, &HB6F0)
                mPKMFontConverter.Add(&H66E, &HB6F4)
                mPKMFontConverter.Add(&H66F, &HB6F8)
                mPKMFontConverter.Add(&H670, &HB700)
                mPKMFontConverter.Add(&H671, &HB701)
                mPKMFontConverter.Add(&H672, &HB705)
                mPKMFontConverter.Add(&H673, &HB728)
                mPKMFontConverter.Add(&H674, &HB729)
                mPKMFontConverter.Add(&H675, &HB72C)
                mPKMFontConverter.Add(&H676, &HB72F)
                mPKMFontConverter.Add(&H677, &HB730)
                mPKMFontConverter.Add(&H678, &HB738)
                mPKMFontConverter.Add(&H679, &HB739)
                mPKMFontConverter.Add(&H67A, &HB73B)
                mPKMFontConverter.Add(&H67B, &HB744)
                mPKMFontConverter.Add(&H67C, &HB748)
                mPKMFontConverter.Add(&H67D, &HB74C)
                mPKMFontConverter.Add(&H67E, &HB754)
                mPKMFontConverter.Add(&H67F, &HB755)
                mPKMFontConverter.Add(&H680, &HB760)
                mPKMFontConverter.Add(&H681, &HB764)
                mPKMFontConverter.Add(&H682, &HB768)
                mPKMFontConverter.Add(&H683, &HB770)
                mPKMFontConverter.Add(&H684, &HB771)
                mPKMFontConverter.Add(&H685, &HB773)
                mPKMFontConverter.Add(&H686, &HB775)
                mPKMFontConverter.Add(&H687, &HB77C)
                mPKMFontConverter.Add(&H688, &HB77D)
                mPKMFontConverter.Add(&H689, &HB780)
                mPKMFontConverter.Add(&H68A, &HB784)
                mPKMFontConverter.Add(&H68B, &HB78C)
                mPKMFontConverter.Add(&H68C, &HB78D)
                mPKMFontConverter.Add(&H68D, &HB78F)
                mPKMFontConverter.Add(&H68E, &HB790)
                mPKMFontConverter.Add(&H68F, &HB791)
                mPKMFontConverter.Add(&H690, &HB792)
                mPKMFontConverter.Add(&H691, &HB796)
                mPKMFontConverter.Add(&H692, &HB797)
                mPKMFontConverter.Add(&H693, &HB798)
                mPKMFontConverter.Add(&H694, &HB799)
                mPKMFontConverter.Add(&H695, &HB79C)
                mPKMFontConverter.Add(&H696, &HB7A0)
                mPKMFontConverter.Add(&H697, &HB7A8)
                mPKMFontConverter.Add(&H698, &HB7A9)
                mPKMFontConverter.Add(&H699, &HB7AB)
                mPKMFontConverter.Add(&H69A, &HB7AC)
                mPKMFontConverter.Add(&H69B, &HB7AD)
                mPKMFontConverter.Add(&H69C, &HB7B4)
                mPKMFontConverter.Add(&H69D, &HB7B5)
                mPKMFontConverter.Add(&H69E, &HB7B8)
                mPKMFontConverter.Add(&H69F, &HB7C7)
                mPKMFontConverter.Add(&H6A0, &HB7C9)
                mPKMFontConverter.Add(&H6A1, &HB7EC)
                mPKMFontConverter.Add(&H6A2, &HB7ED)
                mPKMFontConverter.Add(&H6A3, &HB7F0)
                mPKMFontConverter.Add(&H6A4, &HB7F4)
                mPKMFontConverter.Add(&H6A5, &HB7FC)
                mPKMFontConverter.Add(&H6A6, &HB7FD)
                mPKMFontConverter.Add(&H6A7, &HB7FF)
                mPKMFontConverter.Add(&H6A8, &HB800)
                mPKMFontConverter.Add(&H6A9, &HB801)
                mPKMFontConverter.Add(&H6AA, &HB807)
                mPKMFontConverter.Add(&H6AB, &HB808)
                mPKMFontConverter.Add(&H6AC, &HB809)
                mPKMFontConverter.Add(&H6AD, &HB80C)
                mPKMFontConverter.Add(&H6AE, &HB810)
                mPKMFontConverter.Add(&H6AF, &HB818)
                mPKMFontConverter.Add(&H6B0, &HB819)
                mPKMFontConverter.Add(&H6B1, &HB81B)
                mPKMFontConverter.Add(&H6B2, &HB81D)
                mPKMFontConverter.Add(&H6B3, &HB824)
                mPKMFontConverter.Add(&H6B4, &HB825)
                mPKMFontConverter.Add(&H6B5, &HB828)
                mPKMFontConverter.Add(&H6B6, &HB82C)
                mPKMFontConverter.Add(&H6B7, &HB834)
                mPKMFontConverter.Add(&H6B8, &HB835)
                mPKMFontConverter.Add(&H6B9, &HB837)
                mPKMFontConverter.Add(&H6BA, &HB838)
                mPKMFontConverter.Add(&H6BB, &HB839)
                mPKMFontConverter.Add(&H6BC, &HB840)
                mPKMFontConverter.Add(&H6BD, &HB844)
                mPKMFontConverter.Add(&H6BE, &HB851)
                mPKMFontConverter.Add(&H6BF, &HB853)
                mPKMFontConverter.Add(&H6C0, &HB85C)
                mPKMFontConverter.Add(&H6C1, &HB85D)
                mPKMFontConverter.Add(&H6C2, &HB860)
                mPKMFontConverter.Add(&H6C3, &HB864)
                mPKMFontConverter.Add(&H6C4, &HB86C)
                mPKMFontConverter.Add(&H6C5, &HB86D)
                mPKMFontConverter.Add(&H6C6, &HB86F)
                mPKMFontConverter.Add(&H6C7, &HB871)
                mPKMFontConverter.Add(&H6C8, &HB878)
                mPKMFontConverter.Add(&H6C9, &HB87C)
                mPKMFontConverter.Add(&H6CA, &HB88D)
                mPKMFontConverter.Add(&H6CB, &HB8A8)
                mPKMFontConverter.Add(&H6CC, &HB8B0)
                mPKMFontConverter.Add(&H6CD, &HB8B4)
                mPKMFontConverter.Add(&H6CE, &HB8B8)
                mPKMFontConverter.Add(&H6CF, &HB8C0)
                mPKMFontConverter.Add(&H6D0, &HB8C1)
                mPKMFontConverter.Add(&H6D1, &HB8C3)
                mPKMFontConverter.Add(&H6D2, &HB8C5)
                mPKMFontConverter.Add(&H6D3, &HB8CC)
                mPKMFontConverter.Add(&H6D4, &HB8D0)
                mPKMFontConverter.Add(&H6D5, &HB8D4)
                mPKMFontConverter.Add(&H6D6, &HB8DD)
                mPKMFontConverter.Add(&H6D7, &HB8DF)
                mPKMFontConverter.Add(&H6D8, &HB8E1)
                mPKMFontConverter.Add(&H6D9, &HB8E8)
                mPKMFontConverter.Add(&H6DA, &HB8E9)
                mPKMFontConverter.Add(&H6DB, &HB8EC)
                mPKMFontConverter.Add(&H6DC, &HB8F0)
                mPKMFontConverter.Add(&H6DD, &HB8F8)
                mPKMFontConverter.Add(&H6DE, &HB8F9)
                mPKMFontConverter.Add(&H6DF, &HB8FB)
                mPKMFontConverter.Add(&H6E0, &HB8FD)
                mPKMFontConverter.Add(&H6E1, &HB904)
                mPKMFontConverter.Add(&H6E2, &HB918)
                mPKMFontConverter.Add(&H6E3, &HB920)
                mPKMFontConverter.Add(&H6E4, &HB93C)
                mPKMFontConverter.Add(&H6E5, &HB93D)
                mPKMFontConverter.Add(&H6E6, &HB940)
                mPKMFontConverter.Add(&H6E7, &HB944)
                mPKMFontConverter.Add(&H6E8, &HB94C)
                mPKMFontConverter.Add(&H6E9, &HB94F)
                mPKMFontConverter.Add(&H6EA, &HB951)
                mPKMFontConverter.Add(&H6EB, &HB958)
                mPKMFontConverter.Add(&H6EC, &HB959)
                mPKMFontConverter.Add(&H6ED, &HB95C)
                mPKMFontConverter.Add(&H6EE, &HB960)
                mPKMFontConverter.Add(&H6EF, &HB968)
                mPKMFontConverter.Add(&H6F0, &HB969)
                mPKMFontConverter.Add(&H6F1, &HB96B)
                mPKMFontConverter.Add(&H6F2, &HB96D)
                mPKMFontConverter.Add(&H6F3, &HB974)
                mPKMFontConverter.Add(&H6F4, &HB975)
                mPKMFontConverter.Add(&H6F5, &HB978)
                mPKMFontConverter.Add(&H6F6, &HB97C)
                mPKMFontConverter.Add(&H6F7, &HB984)
                mPKMFontConverter.Add(&H6F8, &HB985)
                mPKMFontConverter.Add(&H6F9, &HB987)
                mPKMFontConverter.Add(&H6FA, &HB989)
                mPKMFontConverter.Add(&H6FB, &HB98A)
                mPKMFontConverter.Add(&H6FC, &HB98D)
                mPKMFontConverter.Add(&H6FD, &HB98E)
                mPKMFontConverter.Add(&H6FE, &HB9AC)
                mPKMFontConverter.Add(&H6FF, &HB9AD)
                mPKMFontConverter.Add(&H700, &HB9B0)
                mPKMFontConverter.Add(&H701, &HB9B4)
                mPKMFontConverter.Add(&H702, &HB9BC)
                mPKMFontConverter.Add(&H703, &HB9BD)
                mPKMFontConverter.Add(&H704, &HB9BF)
                mPKMFontConverter.Add(&H705, &HB9C1)
                mPKMFontConverter.Add(&H706, &HB9C8)
                mPKMFontConverter.Add(&H707, &HB9C9)
                mPKMFontConverter.Add(&H708, &HB9CC)
                mPKMFontConverter.Add(&H709, &HB9CE)
                mPKMFontConverter.Add(&H70A, &HB9CF)
                mPKMFontConverter.Add(&H70B, &HB9D0)
                mPKMFontConverter.Add(&H70C, &HB9D1)
                mPKMFontConverter.Add(&H70D, &HB9D2)
                mPKMFontConverter.Add(&H70E, &HB9D8)
                mPKMFontConverter.Add(&H70F, &HB9D9)
                mPKMFontConverter.Add(&H710, &HB9DB)
                mPKMFontConverter.Add(&H711, &HB9DD)
                mPKMFontConverter.Add(&H712, &HB9DE)
                mPKMFontConverter.Add(&H713, &HB9E1)
                mPKMFontConverter.Add(&H714, &HB9E3)
                mPKMFontConverter.Add(&H715, &HB9E4)
                mPKMFontConverter.Add(&H716, &HB9E5)
                mPKMFontConverter.Add(&H717, &HB9E8)
                mPKMFontConverter.Add(&H718, &HB9EC)
                mPKMFontConverter.Add(&H719, &HB9F4)
                mPKMFontConverter.Add(&H71A, &HB9F5)
                mPKMFontConverter.Add(&H71B, &HB9F7)
                mPKMFontConverter.Add(&H71C, &HB9F8)
                mPKMFontConverter.Add(&H71D, &HB9F9)
                mPKMFontConverter.Add(&H71E, &HB9FA)
                mPKMFontConverter.Add(&H71F, &HBA00)
                mPKMFontConverter.Add(&H720, &HBA01)
                mPKMFontConverter.Add(&H721, &HBA08)
                mPKMFontConverter.Add(&H722, &HBA15)
                mPKMFontConverter.Add(&H723, &HBA38)
                mPKMFontConverter.Add(&H724, &HBA39)
                mPKMFontConverter.Add(&H725, &HBA3C)
                mPKMFontConverter.Add(&H726, &HBA40)
                mPKMFontConverter.Add(&H727, &HBA42)
                mPKMFontConverter.Add(&H728, &HBA48)
                mPKMFontConverter.Add(&H729, &HBA49)
                mPKMFontConverter.Add(&H72A, &HBA4B)
                mPKMFontConverter.Add(&H72B, &HBA4D)
                mPKMFontConverter.Add(&H72C, &HBA4E)
                mPKMFontConverter.Add(&H72D, &HBA53)
                mPKMFontConverter.Add(&H72E, &HBA54)
                mPKMFontConverter.Add(&H72F, &HBA55)
                mPKMFontConverter.Add(&H730, &HBA58)
                mPKMFontConverter.Add(&H731, &HBA5C)
                mPKMFontConverter.Add(&H732, &HBA64)
                mPKMFontConverter.Add(&H733, &HBA65)
                mPKMFontConverter.Add(&H734, &HBA67)
                mPKMFontConverter.Add(&H735, &HBA68)
                mPKMFontConverter.Add(&H736, &HBA69)
                mPKMFontConverter.Add(&H737, &HBA70)
                mPKMFontConverter.Add(&H738, &HBA71)
                mPKMFontConverter.Add(&H739, &HBA74)
                mPKMFontConverter.Add(&H73A, &HBA78)
                mPKMFontConverter.Add(&H73B, &HBA83)
                mPKMFontConverter.Add(&H73C, &HBA84)
                mPKMFontConverter.Add(&H73D, &HBA85)
                mPKMFontConverter.Add(&H73E, &HBA87)
                mPKMFontConverter.Add(&H73F, &HBA8C)
                mPKMFontConverter.Add(&H740, &HBAA8)
                mPKMFontConverter.Add(&H741, &HBAA9)
                mPKMFontConverter.Add(&H742, &HBAAB)
                mPKMFontConverter.Add(&H743, &HBAAC)
                mPKMFontConverter.Add(&H744, &HBAB0)
                mPKMFontConverter.Add(&H745, &HBAB2)
                mPKMFontConverter.Add(&H746, &HBAB8)
                mPKMFontConverter.Add(&H747, &HBAB9)
                mPKMFontConverter.Add(&H748, &HBABB)
                mPKMFontConverter.Add(&H749, &HBABD)
                mPKMFontConverter.Add(&H74A, &HBAC4)
                mPKMFontConverter.Add(&H74B, &HBAC8)
                mPKMFontConverter.Add(&H74C, &HBAD8)
                mPKMFontConverter.Add(&H74D, &HBAD9)
                mPKMFontConverter.Add(&H74E, &HBAFC)
                mPKMFontConverter.Add(&H74F, &HBB00)
                mPKMFontConverter.Add(&H750, &HBB04)
                mPKMFontConverter.Add(&H751, &HBB0D)
                mPKMFontConverter.Add(&H752, &HBB0F)
                mPKMFontConverter.Add(&H753, &HBB11)
                mPKMFontConverter.Add(&H754, &HBB18)
                mPKMFontConverter.Add(&H755, &HBB1C)
                mPKMFontConverter.Add(&H756, &HBB20)
                mPKMFontConverter.Add(&H757, &HBB29)
                mPKMFontConverter.Add(&H758, &HBB2B)
                mPKMFontConverter.Add(&H759, &HBB34)
                mPKMFontConverter.Add(&H75A, &HBB35)
                mPKMFontConverter.Add(&H75B, &HBB36)
                mPKMFontConverter.Add(&H75C, &HBB38)
                mPKMFontConverter.Add(&H75D, &HBB3B)
                mPKMFontConverter.Add(&H75E, &HBB3C)
                mPKMFontConverter.Add(&H75F, &HBB3D)
                mPKMFontConverter.Add(&H760, &HBB3E)
                mPKMFontConverter.Add(&H761, &HBB44)
                mPKMFontConverter.Add(&H762, &HBB45)
                mPKMFontConverter.Add(&H763, &HBB47)
                mPKMFontConverter.Add(&H764, &HBB49)
                mPKMFontConverter.Add(&H765, &HBB4D)
                mPKMFontConverter.Add(&H766, &HBB4F)
                mPKMFontConverter.Add(&H767, &HBB50)
                mPKMFontConverter.Add(&H768, &HBB54)
                mPKMFontConverter.Add(&H769, &HBB58)
                mPKMFontConverter.Add(&H76A, &HBB61)
                mPKMFontConverter.Add(&H76B, &HBB63)
                mPKMFontConverter.Add(&H76C, &HBB6C)
                mPKMFontConverter.Add(&H76D, &HBB88)
                mPKMFontConverter.Add(&H76E, &HBB8C)
                mPKMFontConverter.Add(&H76F, &HBB90)
                mPKMFontConverter.Add(&H770, &HBBA4)
                mPKMFontConverter.Add(&H771, &HBBA8)
                mPKMFontConverter.Add(&H772, &HBBAC)
                mPKMFontConverter.Add(&H773, &HBBB4)
                mPKMFontConverter.Add(&H774, &HBBB7)
                mPKMFontConverter.Add(&H775, &HBBC0)
                mPKMFontConverter.Add(&H776, &HBBC4)
                mPKMFontConverter.Add(&H777, &HBBC8)
                mPKMFontConverter.Add(&H778, &HBBD0)
                mPKMFontConverter.Add(&H779, &HBBD3)
                mPKMFontConverter.Add(&H77A, &HBBF8)
                mPKMFontConverter.Add(&H77B, &HBBF9)
                mPKMFontConverter.Add(&H77C, &HBBFC)
                mPKMFontConverter.Add(&H77D, &HBBFF)
                mPKMFontConverter.Add(&H77E, &HBC00)
                mPKMFontConverter.Add(&H77F, &HBC02)
                mPKMFontConverter.Add(&H780, &HBC08)
                mPKMFontConverter.Add(&H781, &HBC09)
                mPKMFontConverter.Add(&H782, &HBC0B)
                mPKMFontConverter.Add(&H783, &HBC0C)
                mPKMFontConverter.Add(&H784, &HBC0D)
                mPKMFontConverter.Add(&H785, &HBC0F)
                mPKMFontConverter.Add(&H786, &HBC11)
                mPKMFontConverter.Add(&H787, &HBC14)
                mPKMFontConverter.Add(&H788, &HBC15)
                mPKMFontConverter.Add(&H789, &HBC16)
                mPKMFontConverter.Add(&H78A, &HBC17)
                mPKMFontConverter.Add(&H78B, &HBC18)
                mPKMFontConverter.Add(&H78C, &HBC1B)
                mPKMFontConverter.Add(&H78D, &HBC1C)
                mPKMFontConverter.Add(&H78E, &HBC1D)
                mPKMFontConverter.Add(&H78F, &HBC1E)
                mPKMFontConverter.Add(&H790, &HBC1F)
                mPKMFontConverter.Add(&H791, &HBC24)
                mPKMFontConverter.Add(&H792, &HBC25)
                mPKMFontConverter.Add(&H793, &HBC27)
                mPKMFontConverter.Add(&H794, &HBC29)
                mPKMFontConverter.Add(&H795, &HBC2D)
                mPKMFontConverter.Add(&H796, &HBC30)
                mPKMFontConverter.Add(&H797, &HBC31)
                mPKMFontConverter.Add(&H798, &HBC34)
                mPKMFontConverter.Add(&H799, &HBC38)
                mPKMFontConverter.Add(&H79A, &HBC40)
                mPKMFontConverter.Add(&H79B, &HBC41)
                mPKMFontConverter.Add(&H79C, &HBC43)
                mPKMFontConverter.Add(&H79D, &HBC44)
                mPKMFontConverter.Add(&H79E, &HBC45)
                mPKMFontConverter.Add(&H79F, &HBC49)
                mPKMFontConverter.Add(&H7A0, &HBC4C)
                mPKMFontConverter.Add(&H7A1, &HBC4D)
                mPKMFontConverter.Add(&H7A2, &HBC50)
                mPKMFontConverter.Add(&H7A3, &HBC5D)
                mPKMFontConverter.Add(&H7A4, &HBC84)
                mPKMFontConverter.Add(&H7A5, &HBC85)
                mPKMFontConverter.Add(&H7A6, &HBC88)
                mPKMFontConverter.Add(&H7A7, &HBC8B)
                mPKMFontConverter.Add(&H7A8, &HBC8C)
                mPKMFontConverter.Add(&H7A9, &HBC8E)
                mPKMFontConverter.Add(&H7AA, &HBC94)
                mPKMFontConverter.Add(&H7AB, &HBC95)
                mPKMFontConverter.Add(&H7AC, &HBC97)
                mPKMFontConverter.Add(&H7AD, &HBC99)
                mPKMFontConverter.Add(&H7AE, &HBC9A)
                mPKMFontConverter.Add(&H7AF, &HBCA0)
                mPKMFontConverter.Add(&H7B0, &HBCA1)
                mPKMFontConverter.Add(&H7B1, &HBCA4)
                mPKMFontConverter.Add(&H7B2, &HBCA7)
                mPKMFontConverter.Add(&H7B3, &HBCA8)
                mPKMFontConverter.Add(&H7B4, &HBCB0)
                mPKMFontConverter.Add(&H7B5, &HBCB1)
                mPKMFontConverter.Add(&H7B6, &HBCB3)
                mPKMFontConverter.Add(&H7B7, &HBCB4)
                mPKMFontConverter.Add(&H7B8, &HBCB5)
                mPKMFontConverter.Add(&H7B9, &HBCBC)
                mPKMFontConverter.Add(&H7BA, &HBCBD)
                mPKMFontConverter.Add(&H7BB, &HBCC0)
                mPKMFontConverter.Add(&H7BC, &HBCC4)
                mPKMFontConverter.Add(&H7BD, &HBCCD)
                mPKMFontConverter.Add(&H7BE, &HBCCF)
                mPKMFontConverter.Add(&H7BF, &HBCD0)
                mPKMFontConverter.Add(&H7C0, &HBCD1)
                mPKMFontConverter.Add(&H7C1, &HBCD5)
                mPKMFontConverter.Add(&H7C2, &HBCD8)
                mPKMFontConverter.Add(&H7C3, &HBCDC)
                mPKMFontConverter.Add(&H7C4, &HBCF4)
                mPKMFontConverter.Add(&H7C5, &HBCF5)
                mPKMFontConverter.Add(&H7C6, &HBCF6)
                mPKMFontConverter.Add(&H7C7, &HBCF8)
                mPKMFontConverter.Add(&H7C8, &HBCFC)
                mPKMFontConverter.Add(&H7C9, &HBD04)
                mPKMFontConverter.Add(&H7CA, &HBD05)
                mPKMFontConverter.Add(&H7CB, &HBD07)
                mPKMFontConverter.Add(&H7CC, &HBD09)
                mPKMFontConverter.Add(&H7CD, &HBD10)
                mPKMFontConverter.Add(&H7CE, &HBD14)
                mPKMFontConverter.Add(&H7CF, &HBD24)
                mPKMFontConverter.Add(&H7D0, &HBD2C)
                mPKMFontConverter.Add(&H7D1, &HBD40)
                mPKMFontConverter.Add(&H7D2, &HBD48)
                mPKMFontConverter.Add(&H7D3, &HBD49)
                mPKMFontConverter.Add(&H7D4, &HBD4C)
                mPKMFontConverter.Add(&H7D5, &HBD50)
                mPKMFontConverter.Add(&H7D6, &HBD58)
                mPKMFontConverter.Add(&H7D7, &HBD59)
                mPKMFontConverter.Add(&H7D8, &HBD64)
                mPKMFontConverter.Add(&H7D9, &HBD68)
                mPKMFontConverter.Add(&H7DA, &HBD80)
                mPKMFontConverter.Add(&H7DB, &HBD81)
                mPKMFontConverter.Add(&H7DC, &HBD84)
                mPKMFontConverter.Add(&H7DD, &HBD87)
                mPKMFontConverter.Add(&H7DE, &HBD88)
                mPKMFontConverter.Add(&H7DF, &HBD89)
                mPKMFontConverter.Add(&H7E0, &HBD8A)
                mPKMFontConverter.Add(&H7E1, &HBD90)
                mPKMFontConverter.Add(&H7E2, &HBD91)
                mPKMFontConverter.Add(&H7E3, &HBD93)
                mPKMFontConverter.Add(&H7E4, &HBD95)
                mPKMFontConverter.Add(&H7E5, &HBD99)
                mPKMFontConverter.Add(&H7E6, &HBD9A)
                mPKMFontConverter.Add(&H7E7, &HBD9C)
                mPKMFontConverter.Add(&H7E8, &HBDA4)
                mPKMFontConverter.Add(&H7E9, &HBDB0)
                mPKMFontConverter.Add(&H7EA, &HBDB8)
                mPKMFontConverter.Add(&H7EB, &HBDD4)
                mPKMFontConverter.Add(&H7EC, &HBDD5)
                mPKMFontConverter.Add(&H7ED, &HBDD8)
                mPKMFontConverter.Add(&H7EE, &HBDDC)
                mPKMFontConverter.Add(&H7EF, &HBDE9)
                mPKMFontConverter.Add(&H7F0, &HBDF0)
                mPKMFontConverter.Add(&H7F1, &HBDF4)
                mPKMFontConverter.Add(&H7F2, &HBDF8)
                mPKMFontConverter.Add(&H7F3, &HBE00)
                mPKMFontConverter.Add(&H7F4, &HBE03)
                mPKMFontConverter.Add(&H7F5, &HBE05)
                mPKMFontConverter.Add(&H7F6, &HBE0C)
                mPKMFontConverter.Add(&H7F7, &HBE0D)
                mPKMFontConverter.Add(&H7F8, &HBE10)
                mPKMFontConverter.Add(&H7F9, &HBE14)
                mPKMFontConverter.Add(&H7FA, &HBE1C)
                mPKMFontConverter.Add(&H7FB, &HBE1D)
                mPKMFontConverter.Add(&H7FC, &HBE1F)
                mPKMFontConverter.Add(&H7FD, &HBE44)
                mPKMFontConverter.Add(&H7FE, &HBE45)
                mPKMFontConverter.Add(&H7FF, &HBE48)
                mPKMFontConverter.Add(&H800, &HBE4C)
                mPKMFontConverter.Add(&H801, &HBE4E)
                mPKMFontConverter.Add(&H802, &HBE54)
                mPKMFontConverter.Add(&H803, &HBE55)
                mPKMFontConverter.Add(&H804, &HBE57)
                mPKMFontConverter.Add(&H805, &HBE59)
                mPKMFontConverter.Add(&H806, &HBE5A)
                mPKMFontConverter.Add(&H807, &HBE5B)
                mPKMFontConverter.Add(&H808, &HBE60)
                mPKMFontConverter.Add(&H809, &HBE61)
                mPKMFontConverter.Add(&H80A, &HBE64)
                mPKMFontConverter.Add(&H80B, &HBE68)
                mPKMFontConverter.Add(&H80C, &HBE6A)
                mPKMFontConverter.Add(&H80D, &HBE70)
                mPKMFontConverter.Add(&H80E, &HBE71)
                mPKMFontConverter.Add(&H80F, &HBE73)
                mPKMFontConverter.Add(&H810, &HBE74)
                mPKMFontConverter.Add(&H811, &HBE75)
                mPKMFontConverter.Add(&H812, &HBE7B)
                mPKMFontConverter.Add(&H813, &HBE7C)
                mPKMFontConverter.Add(&H814, &HBE7D)
                mPKMFontConverter.Add(&H815, &HBE80)
                mPKMFontConverter.Add(&H816, &HBE84)
                mPKMFontConverter.Add(&H817, &HBE8C)
                mPKMFontConverter.Add(&H818, &HBE8D)
                mPKMFontConverter.Add(&H819, &HBE8F)
                mPKMFontConverter.Add(&H81A, &HBE90)
                mPKMFontConverter.Add(&H81B, &HBE91)
                mPKMFontConverter.Add(&H81C, &HBE98)
                mPKMFontConverter.Add(&H81D, &HBE99)
                mPKMFontConverter.Add(&H81E, &HBEA8)
                mPKMFontConverter.Add(&H81F, &HBED0)
                mPKMFontConverter.Add(&H820, &HBED1)
                mPKMFontConverter.Add(&H821, &HBED4)
                mPKMFontConverter.Add(&H822, &HBED7)
                mPKMFontConverter.Add(&H823, &HBED8)
                mPKMFontConverter.Add(&H824, &HBEE0)
                mPKMFontConverter.Add(&H825, &HBEE3)
                mPKMFontConverter.Add(&H826, &HBEE4)
                mPKMFontConverter.Add(&H827, &HBEE5)
                mPKMFontConverter.Add(&H828, &HBEEC)
                mPKMFontConverter.Add(&H829, &HBF01)
                mPKMFontConverter.Add(&H82A, &HBF08)
                mPKMFontConverter.Add(&H82B, &HBF09)
                mPKMFontConverter.Add(&H82C, &HBF18)
                mPKMFontConverter.Add(&H82D, &HBF19)
                mPKMFontConverter.Add(&H82E, &HBF1B)
                mPKMFontConverter.Add(&H82F, &HBF1C)
                mPKMFontConverter.Add(&H830, &HBF1D)
                mPKMFontConverter.Add(&H831, &HBF40)
                mPKMFontConverter.Add(&H832, &HBF41)
                mPKMFontConverter.Add(&H833, &HBF44)
                mPKMFontConverter.Add(&H834, &HBF48)
                mPKMFontConverter.Add(&H835, &HBF50)
                mPKMFontConverter.Add(&H836, &HBF51)
                mPKMFontConverter.Add(&H837, &HBF55)
                mPKMFontConverter.Add(&H838, &HBF94)
                mPKMFontConverter.Add(&H839, &HBFB0)
                mPKMFontConverter.Add(&H83A, &HBFC5)
                mPKMFontConverter.Add(&H83B, &HBFCC)
                mPKMFontConverter.Add(&H83C, &HBFCD)
                mPKMFontConverter.Add(&H83D, &HBFD0)
                mPKMFontConverter.Add(&H83E, &HBFD4)
                mPKMFontConverter.Add(&H83F, &HBFDC)
                mPKMFontConverter.Add(&H840, &HBFDF)
                mPKMFontConverter.Add(&H841, &HBFE1)
                mPKMFontConverter.Add(&H842, &HC03C)
                mPKMFontConverter.Add(&H843, &HC051)
                mPKMFontConverter.Add(&H844, &HC058)
                mPKMFontConverter.Add(&H845, &HC05C)
                mPKMFontConverter.Add(&H846, &HC060)
                mPKMFontConverter.Add(&H847, &HC068)
                mPKMFontConverter.Add(&H848, &HC069)
                mPKMFontConverter.Add(&H849, &HC090)
                mPKMFontConverter.Add(&H84A, &HC091)
                mPKMFontConverter.Add(&H84B, &HC094)
                mPKMFontConverter.Add(&H84C, &HC098)
                mPKMFontConverter.Add(&H84D, &HC0A0)
                mPKMFontConverter.Add(&H84E, &HC0A1)
                mPKMFontConverter.Add(&H84F, &HC0A3)
                mPKMFontConverter.Add(&H850, &HC0A5)
                mPKMFontConverter.Add(&H851, &HC0AC)
                mPKMFontConverter.Add(&H852, &HC0AD)
                mPKMFontConverter.Add(&H853, &HC0AF)
                mPKMFontConverter.Add(&H854, &HC0B0)
                mPKMFontConverter.Add(&H855, &HC0B3)
                mPKMFontConverter.Add(&H856, &HC0B4)
                mPKMFontConverter.Add(&H857, &HC0B5)
                mPKMFontConverter.Add(&H858, &HC0B6)
                mPKMFontConverter.Add(&H859, &HC0BC)
                mPKMFontConverter.Add(&H85A, &HC0BD)
                mPKMFontConverter.Add(&H85B, &HC0BF)
                mPKMFontConverter.Add(&H85C, &HC0C0)
                mPKMFontConverter.Add(&H85D, &HC0C1)
                mPKMFontConverter.Add(&H85E, &HC0C5)
                mPKMFontConverter.Add(&H85F, &HC0C8)
                mPKMFontConverter.Add(&H860, &HC0C9)
                mPKMFontConverter.Add(&H861, &HC0CC)
                mPKMFontConverter.Add(&H862, &HC0D0)
                mPKMFontConverter.Add(&H863, &HC0D8)
                mPKMFontConverter.Add(&H864, &HC0D9)
                mPKMFontConverter.Add(&H865, &HC0DB)
                mPKMFontConverter.Add(&H866, &HC0DC)
                mPKMFontConverter.Add(&H867, &HC0DD)
                mPKMFontConverter.Add(&H868, &HC0E4)
                mPKMFontConverter.Add(&H869, &HC0E5)
                mPKMFontConverter.Add(&H86A, &HC0E8)
                mPKMFontConverter.Add(&H86B, &HC0EC)
                mPKMFontConverter.Add(&H86C, &HC0F4)
                mPKMFontConverter.Add(&H86D, &HC0F5)
                mPKMFontConverter.Add(&H86E, &HC0F7)
                mPKMFontConverter.Add(&H86F, &HC0F9)
                mPKMFontConverter.Add(&H870, &HC100)
                mPKMFontConverter.Add(&H871, &HC104)
                mPKMFontConverter.Add(&H872, &HC108)
                mPKMFontConverter.Add(&H873, &HC110)
                mPKMFontConverter.Add(&H874, &HC115)
                mPKMFontConverter.Add(&H875, &HC11C)
                mPKMFontConverter.Add(&H876, &HC11D)
                mPKMFontConverter.Add(&H877, &HC11E)
                mPKMFontConverter.Add(&H878, &HC11F)
                mPKMFontConverter.Add(&H879, &HC120)
                mPKMFontConverter.Add(&H87A, &HC123)
                mPKMFontConverter.Add(&H87B, &HC124)
                mPKMFontConverter.Add(&H87C, &HC126)
                mPKMFontConverter.Add(&H87D, &HC127)
                mPKMFontConverter.Add(&H87E, &HC12C)
                mPKMFontConverter.Add(&H87F, &HC12D)
                mPKMFontConverter.Add(&H880, &HC12F)
                mPKMFontConverter.Add(&H881, &HC130)
                mPKMFontConverter.Add(&H882, &HC131)
                mPKMFontConverter.Add(&H883, &HC136)
                mPKMFontConverter.Add(&H884, &HC138)
                mPKMFontConverter.Add(&H885, &HC139)
                mPKMFontConverter.Add(&H886, &HC13C)
                mPKMFontConverter.Add(&H887, &HC140)
                mPKMFontConverter.Add(&H888, &HC148)
                mPKMFontConverter.Add(&H889, &HC149)
                mPKMFontConverter.Add(&H88A, &HC14B)
                mPKMFontConverter.Add(&H88B, &HC14C)
                mPKMFontConverter.Add(&H88C, &HC14D)
                mPKMFontConverter.Add(&H88D, &HC154)
                mPKMFontConverter.Add(&H88E, &HC155)
                mPKMFontConverter.Add(&H88F, &HC158)
                mPKMFontConverter.Add(&H890, &HC15C)
                mPKMFontConverter.Add(&H891, &HC164)
                mPKMFontConverter.Add(&H892, &HC165)
                mPKMFontConverter.Add(&H893, &HC167)
                mPKMFontConverter.Add(&H894, &HC168)
                mPKMFontConverter.Add(&H895, &HC169)
                mPKMFontConverter.Add(&H896, &HC170)
                mPKMFontConverter.Add(&H897, &HC174)
                mPKMFontConverter.Add(&H898, &HC178)
                mPKMFontConverter.Add(&H899, &HC185)
                mPKMFontConverter.Add(&H89A, &HC18C)
                mPKMFontConverter.Add(&H89B, &HC18D)
                mPKMFontConverter.Add(&H89C, &HC18E)
                mPKMFontConverter.Add(&H89D, &HC190)
                mPKMFontConverter.Add(&H89E, &HC194)
                mPKMFontConverter.Add(&H89F, &HC196)
                mPKMFontConverter.Add(&H8A0, &HC19C)
                mPKMFontConverter.Add(&H8A1, &HC19D)
                mPKMFontConverter.Add(&H8A2, &HC19F)
                mPKMFontConverter.Add(&H8A3, &HC1A1)
                mPKMFontConverter.Add(&H8A4, &HC1A5)
                mPKMFontConverter.Add(&H8A5, &HC1A8)
                mPKMFontConverter.Add(&H8A6, &HC1A9)
                mPKMFontConverter.Add(&H8A7, &HC1AC)
                mPKMFontConverter.Add(&H8A8, &HC1B0)
                mPKMFontConverter.Add(&H8A9, &HC1BD)
                mPKMFontConverter.Add(&H8AA, &HC1C4)
                mPKMFontConverter.Add(&H8AB, &HC1C8)
                mPKMFontConverter.Add(&H8AC, &HC1CC)
                mPKMFontConverter.Add(&H8AD, &HC1D4)
                mPKMFontConverter.Add(&H8AE, &HC1D7)
                mPKMFontConverter.Add(&H8AF, &HC1D8)
                mPKMFontConverter.Add(&H8B0, &HC1E0)
                mPKMFontConverter.Add(&H8B1, &HC1E4)
                mPKMFontConverter.Add(&H8B2, &HC1E8)
                mPKMFontConverter.Add(&H8B3, &HC1F0)
                mPKMFontConverter.Add(&H8B4, &HC1F1)
                mPKMFontConverter.Add(&H8B5, &HC1F3)
                mPKMFontConverter.Add(&H8B6, &HC1FC)
                mPKMFontConverter.Add(&H8B7, &HC1FD)
                mPKMFontConverter.Add(&H8B8, &HC200)
                mPKMFontConverter.Add(&H8B9, &HC204)
                mPKMFontConverter.Add(&H8BA, &HC20C)
                mPKMFontConverter.Add(&H8BB, &HC20D)
                mPKMFontConverter.Add(&H8BC, &HC20F)
                mPKMFontConverter.Add(&H8BD, &HC211)
                mPKMFontConverter.Add(&H8BE, &HC218)
                mPKMFontConverter.Add(&H8BF, &HC219)
                mPKMFontConverter.Add(&H8C0, &HC21C)
                mPKMFontConverter.Add(&H8C1, &HC21F)
                mPKMFontConverter.Add(&H8C2, &HC220)
                mPKMFontConverter.Add(&H8C3, &HC228)
                mPKMFontConverter.Add(&H8C4, &HC229)
                mPKMFontConverter.Add(&H8C5, &HC22B)
                mPKMFontConverter.Add(&H8C6, &HC22D)
                mPKMFontConverter.Add(&H8C7, &HC22F)
                mPKMFontConverter.Add(&H8C8, &HC231)
                mPKMFontConverter.Add(&H8C9, &HC232)
                mPKMFontConverter.Add(&H8CA, &HC234)
                mPKMFontConverter.Add(&H8CB, &HC248)
                mPKMFontConverter.Add(&H8CC, &HC250)
                mPKMFontConverter.Add(&H8CD, &HC251)
                mPKMFontConverter.Add(&H8CE, &HC254)
                mPKMFontConverter.Add(&H8CF, &HC258)
                mPKMFontConverter.Add(&H8D0, &HC260)
                mPKMFontConverter.Add(&H8D1, &HC265)
                mPKMFontConverter.Add(&H8D2, &HC26C)
                mPKMFontConverter.Add(&H8D3, &HC26D)
                mPKMFontConverter.Add(&H8D4, &HC270)
                mPKMFontConverter.Add(&H8D5, &HC274)
                mPKMFontConverter.Add(&H8D6, &HC27C)
                mPKMFontConverter.Add(&H8D7, &HC27D)
                mPKMFontConverter.Add(&H8D8, &HC27F)
                mPKMFontConverter.Add(&H8D9, &HC281)
                mPKMFontConverter.Add(&H8DA, &HC288)
                mPKMFontConverter.Add(&H8DB, &HC289)
                mPKMFontConverter.Add(&H8DC, &HC290)
                mPKMFontConverter.Add(&H8DD, &HC298)
                mPKMFontConverter.Add(&H8DE, &HC29B)
                mPKMFontConverter.Add(&H8DF, &HC29D)
                mPKMFontConverter.Add(&H8E0, &HC2A4)
                mPKMFontConverter.Add(&H8E1, &HC2A5)
                mPKMFontConverter.Add(&H8E2, &HC2A8)
                mPKMFontConverter.Add(&H8E3, &HC2AC)
                mPKMFontConverter.Add(&H8E4, &HC2AD)
                mPKMFontConverter.Add(&H8E5, &HC2B4)
                mPKMFontConverter.Add(&H8E6, &HC2B5)
                mPKMFontConverter.Add(&H8E7, &HC2B7)
                mPKMFontConverter.Add(&H8E8, &HC2B9)
                mPKMFontConverter.Add(&H8E9, &HC2DC)
                mPKMFontConverter.Add(&H8EA, &HC2DD)
                mPKMFontConverter.Add(&H8EB, &HC2E0)
                mPKMFontConverter.Add(&H8EC, &HC2E3)
                mPKMFontConverter.Add(&H8ED, &HC2E4)
                mPKMFontConverter.Add(&H8EE, &HC2EB)
                mPKMFontConverter.Add(&H8EF, &HC2EC)
                mPKMFontConverter.Add(&H8F0, &HC2ED)
                mPKMFontConverter.Add(&H8F1, &HC2EF)
                mPKMFontConverter.Add(&H8F2, &HC2F1)
                mPKMFontConverter.Add(&H8F3, &HC2F6)
                mPKMFontConverter.Add(&H8F4, &HC2F8)
                mPKMFontConverter.Add(&H8F5, &HC2F9)
                mPKMFontConverter.Add(&H8F6, &HC2FB)
                mPKMFontConverter.Add(&H8F7, &HC2FC)
                mPKMFontConverter.Add(&H8F8, &HC300)
                mPKMFontConverter.Add(&H8F9, &HC308)
                mPKMFontConverter.Add(&H8FA, &HC309)
                mPKMFontConverter.Add(&H8FB, &HC30C)
                mPKMFontConverter.Add(&H8FC, &HC30D)
                mPKMFontConverter.Add(&H8FD, &HC313)
                mPKMFontConverter.Add(&H8FE, &HC314)
                mPKMFontConverter.Add(&H8FF, &HC315)
                mPKMFontConverter.Add(&H900, &HC318)
                mPKMFontConverter.Add(&H901, &HC31C)
                mPKMFontConverter.Add(&H902, &HC324)
                mPKMFontConverter.Add(&H903, &HC325)
                mPKMFontConverter.Add(&H904, &HC328)
                mPKMFontConverter.Add(&H905, &HC329)
                mPKMFontConverter.Add(&H906, &HC345)
                mPKMFontConverter.Add(&H907, &HC368)
                mPKMFontConverter.Add(&H908, &HC369)
                mPKMFontConverter.Add(&H909, &HC36C)
                mPKMFontConverter.Add(&H90A, &HC370)
                mPKMFontConverter.Add(&H90B, &HC372)
                mPKMFontConverter.Add(&H90C, &HC378)
                mPKMFontConverter.Add(&H90D, &HC379)
                mPKMFontConverter.Add(&H90E, &HC37C)
                mPKMFontConverter.Add(&H90F, &HC37D)
                mPKMFontConverter.Add(&H910, &HC384)
                mPKMFontConverter.Add(&H911, &HC388)
                mPKMFontConverter.Add(&H912, &HC38C)
                mPKMFontConverter.Add(&H913, &HC3C0)
                mPKMFontConverter.Add(&H914, &HC3D8)
                mPKMFontConverter.Add(&H915, &HC3D9)
                mPKMFontConverter.Add(&H916, &HC3DC)
                mPKMFontConverter.Add(&H917, &HC3DF)
                mPKMFontConverter.Add(&H918, &HC3E0)
                mPKMFontConverter.Add(&H919, &HC3E2)
                mPKMFontConverter.Add(&H91A, &HC3E8)
                mPKMFontConverter.Add(&H91B, &HC3E9)
                mPKMFontConverter.Add(&H91C, &HC3ED)
                mPKMFontConverter.Add(&H91D, &HC3F4)
                mPKMFontConverter.Add(&H91E, &HC3F5)
                mPKMFontConverter.Add(&H91F, &HC3F8)
                mPKMFontConverter.Add(&H920, &HC408)
                mPKMFontConverter.Add(&H921, &HC410)
                mPKMFontConverter.Add(&H922, &HC424)
                mPKMFontConverter.Add(&H923, &HC42C)
                mPKMFontConverter.Add(&H924, &HC430)
                mPKMFontConverter.Add(&H925, &HC434)
                mPKMFontConverter.Add(&H926, &HC43C)
                mPKMFontConverter.Add(&H927, &HC43D)
                mPKMFontConverter.Add(&H928, &HC448)
                mPKMFontConverter.Add(&H929, &HC464)
                mPKMFontConverter.Add(&H92A, &HC465)
                mPKMFontConverter.Add(&H92B, &HC468)
                mPKMFontConverter.Add(&H92C, &HC46C)
                mPKMFontConverter.Add(&H92D, &HC474)
                mPKMFontConverter.Add(&H92E, &HC475)
                mPKMFontConverter.Add(&H92F, &HC479)
                mPKMFontConverter.Add(&H930, &HC480)
                mPKMFontConverter.Add(&H931, &HC494)
                mPKMFontConverter.Add(&H932, &HC49C)
                mPKMFontConverter.Add(&H933, &HC4B8)
                mPKMFontConverter.Add(&H934, &HC4BC)
                mPKMFontConverter.Add(&H935, &HC4E9)
                mPKMFontConverter.Add(&H936, &HC4F0)
                mPKMFontConverter.Add(&H937, &HC4F1)
                mPKMFontConverter.Add(&H938, &HC4F4)
                mPKMFontConverter.Add(&H939, &HC4F8)
                mPKMFontConverter.Add(&H93A, &HC4FA)
                mPKMFontConverter.Add(&H93B, &HC4FF)
                mPKMFontConverter.Add(&H93C, &HC500)
                mPKMFontConverter.Add(&H93D, &HC501)
                mPKMFontConverter.Add(&H93E, &HC50C)
                mPKMFontConverter.Add(&H93F, &HC510)
                mPKMFontConverter.Add(&H940, &HC514)
                mPKMFontConverter.Add(&H941, &HC51C)
                mPKMFontConverter.Add(&H942, &HC528)
                mPKMFontConverter.Add(&H943, &HC529)
                mPKMFontConverter.Add(&H944, &HC52C)
                mPKMFontConverter.Add(&H945, &HC530)
                mPKMFontConverter.Add(&H946, &HC538)
                mPKMFontConverter.Add(&H947, &HC539)
                mPKMFontConverter.Add(&H948, &HC53B)
                mPKMFontConverter.Add(&H949, &HC53D)
                mPKMFontConverter.Add(&H94A, &HC544)
                mPKMFontConverter.Add(&H94B, &HC545)
                mPKMFontConverter.Add(&H94C, &HC548)
                mPKMFontConverter.Add(&H94D, &HC549)
                mPKMFontConverter.Add(&H94E, &HC54A)
                mPKMFontConverter.Add(&H94F, &HC54C)
                mPKMFontConverter.Add(&H950, &HC54D)
                mPKMFontConverter.Add(&H951, &HC54E)
                mPKMFontConverter.Add(&H952, &HC553)
                mPKMFontConverter.Add(&H953, &HC554)
                mPKMFontConverter.Add(&H954, &HC555)
                mPKMFontConverter.Add(&H955, &HC557)
                mPKMFontConverter.Add(&H956, &HC558)
                mPKMFontConverter.Add(&H957, &HC559)
                mPKMFontConverter.Add(&H958, &HC55D)
                mPKMFontConverter.Add(&H959, &HC55E)
                mPKMFontConverter.Add(&H95A, &HC560)
                mPKMFontConverter.Add(&H95B, &HC561)
                mPKMFontConverter.Add(&H95C, &HC564)
                mPKMFontConverter.Add(&H95D, &HC568)
                mPKMFontConverter.Add(&H95E, &HC570)
                mPKMFontConverter.Add(&H95F, &HC571)
                mPKMFontConverter.Add(&H960, &HC573)
                mPKMFontConverter.Add(&H961, &HC574)
                mPKMFontConverter.Add(&H962, &HC575)
                mPKMFontConverter.Add(&H963, &HC57C)
                mPKMFontConverter.Add(&H964, &HC57D)
                mPKMFontConverter.Add(&H965, &HC580)
                mPKMFontConverter.Add(&H966, &HC584)
                mPKMFontConverter.Add(&H967, &HC587)
                mPKMFontConverter.Add(&H968, &HC58C)
                mPKMFontConverter.Add(&H969, &HC58D)
                mPKMFontConverter.Add(&H96A, &HC58F)
                mPKMFontConverter.Add(&H96B, &HC591)
                mPKMFontConverter.Add(&H96C, &HC595)
                mPKMFontConverter.Add(&H96D, &HC597)
                mPKMFontConverter.Add(&H96E, &HC598)
                mPKMFontConverter.Add(&H96F, &HC59C)
                mPKMFontConverter.Add(&H970, &HC5A0)
                mPKMFontConverter.Add(&H971, &HC5A9)
                mPKMFontConverter.Add(&H972, &HC5B4)
                mPKMFontConverter.Add(&H973, &HC5B5)
                mPKMFontConverter.Add(&H974, &HC5B8)
                mPKMFontConverter.Add(&H975, &HC5B9)
                mPKMFontConverter.Add(&H976, &HC5BB)
                mPKMFontConverter.Add(&H977, &HC5BC)
                mPKMFontConverter.Add(&H978, &HC5BD)
                mPKMFontConverter.Add(&H979, &HC5BE)
                mPKMFontConverter.Add(&H97A, &HC5C4)
                mPKMFontConverter.Add(&H97B, &HC5C5)
                mPKMFontConverter.Add(&H97C, &HC5C6)
                mPKMFontConverter.Add(&H97D, &HC5C7)
                mPKMFontConverter.Add(&H97E, &HC5C8)
                mPKMFontConverter.Add(&H97F, &HC5C9)
                mPKMFontConverter.Add(&H980, &HC5CA)
                mPKMFontConverter.Add(&H981, &HC5CC)
                mPKMFontConverter.Add(&H982, &HC5CE)
                mPKMFontConverter.Add(&H983, &HC5D0)
                mPKMFontConverter.Add(&H984, &HC5D1)
                mPKMFontConverter.Add(&H985, &HC5D4)
                mPKMFontConverter.Add(&H986, &HC5D8)
                mPKMFontConverter.Add(&H987, &HC5E0)
                mPKMFontConverter.Add(&H988, &HC5E1)
                mPKMFontConverter.Add(&H989, &HC5E3)
                mPKMFontConverter.Add(&H98A, &HC5E5)
                mPKMFontConverter.Add(&H98B, &HC5EC)
                mPKMFontConverter.Add(&H98C, &HC5ED)
                mPKMFontConverter.Add(&H98D, &HC5EE)
                mPKMFontConverter.Add(&H98E, &HC5F0)
                mPKMFontConverter.Add(&H98F, &HC5F4)
                mPKMFontConverter.Add(&H990, &HC5F6)
                mPKMFontConverter.Add(&H991, &HC5F7)
                mPKMFontConverter.Add(&H992, &HC5FC)
                mPKMFontConverter.Add(&H993, &HC5FD)
                mPKMFontConverter.Add(&H994, &HC5FE)
                mPKMFontConverter.Add(&H995, &HC5FF)
                mPKMFontConverter.Add(&H996, &HC600)
                mPKMFontConverter.Add(&H997, &HC601)
                mPKMFontConverter.Add(&H998, &HC605)
                mPKMFontConverter.Add(&H999, &HC606)
                mPKMFontConverter.Add(&H99A, &HC607)
                mPKMFontConverter.Add(&H99B, &HC608)
                mPKMFontConverter.Add(&H99C, &HC60C)
                mPKMFontConverter.Add(&H99D, &HC610)
                mPKMFontConverter.Add(&H99E, &HC618)
                mPKMFontConverter.Add(&H99F, &HC619)
                mPKMFontConverter.Add(&H9A0, &HC61B)
                mPKMFontConverter.Add(&H9A1, &HC61C)
                mPKMFontConverter.Add(&H9A2, &HC624)
                mPKMFontConverter.Add(&H9A3, &HC625)
                mPKMFontConverter.Add(&H9A4, &HC628)
                mPKMFontConverter.Add(&H9A5, &HC62C)
                mPKMFontConverter.Add(&H9A6, &HC62D)
                mPKMFontConverter.Add(&H9A7, &HC62E)
                mPKMFontConverter.Add(&H9A8, &HC630)
                mPKMFontConverter.Add(&H9A9, &HC633)
                mPKMFontConverter.Add(&H9AA, &HC634)
                mPKMFontConverter.Add(&H9AB, &HC635)
                mPKMFontConverter.Add(&H9AC, &HC637)
                mPKMFontConverter.Add(&H9AD, &HC639)
                mPKMFontConverter.Add(&H9AE, &HC63B)
                mPKMFontConverter.Add(&H9AF, &HC640)
                mPKMFontConverter.Add(&H9B0, &HC641)
                mPKMFontConverter.Add(&H9B1, &HC644)
                mPKMFontConverter.Add(&H9B2, &HC648)
                mPKMFontConverter.Add(&H9B3, &HC650)
                mPKMFontConverter.Add(&H9B4, &HC651)
                mPKMFontConverter.Add(&H9B5, &HC653)
                mPKMFontConverter.Add(&H9B6, &HC654)
                mPKMFontConverter.Add(&H9B7, &HC655)
                mPKMFontConverter.Add(&H9B8, &HC65C)
                mPKMFontConverter.Add(&H9B9, &HC65D)
                mPKMFontConverter.Add(&H9BA, &HC660)
                mPKMFontConverter.Add(&H9BB, &HC66C)
                mPKMFontConverter.Add(&H9BC, &HC66F)
                mPKMFontConverter.Add(&H9BD, &HC671)
                mPKMFontConverter.Add(&H9BE, &HC678)
                mPKMFontConverter.Add(&H9BF, &HC679)
                mPKMFontConverter.Add(&H9C0, &HC67C)
                mPKMFontConverter.Add(&H9C1, &HC680)
                mPKMFontConverter.Add(&H9C2, &HC688)
                mPKMFontConverter.Add(&H9C3, &HC689)
                mPKMFontConverter.Add(&H9C4, &HC68B)
                mPKMFontConverter.Add(&H9C5, &HC68D)
                mPKMFontConverter.Add(&H9C6, &HC694)
                mPKMFontConverter.Add(&H9C7, &HC695)
                mPKMFontConverter.Add(&H9C8, &HC698)
                mPKMFontConverter.Add(&H9C9, &HC69C)
                mPKMFontConverter.Add(&H9CA, &HC6A4)
                mPKMFontConverter.Add(&H9CB, &HC6A5)
                mPKMFontConverter.Add(&H9CC, &HC6A7)
                mPKMFontConverter.Add(&H9CD, &HC6A9)
                mPKMFontConverter.Add(&H9CE, &HC6B0)
                mPKMFontConverter.Add(&H9CF, &HC6B1)
                mPKMFontConverter.Add(&H9D0, &HC6B4)
                mPKMFontConverter.Add(&H9D1, &HC6B8)
                mPKMFontConverter.Add(&H9D2, &HC6B9)
                mPKMFontConverter.Add(&H9D3, &HC6BA)
                mPKMFontConverter.Add(&H9D4, &HC6C0)
                mPKMFontConverter.Add(&H9D5, &HC6C1)
                mPKMFontConverter.Add(&H9D6, &HC6C3)
                mPKMFontConverter.Add(&H9D7, &HC6C5)
                mPKMFontConverter.Add(&H9D8, &HC6CC)
                mPKMFontConverter.Add(&H9D9, &HC6CD)
                mPKMFontConverter.Add(&H9DA, &HC6D0)
                mPKMFontConverter.Add(&H9DB, &HC6D4)
                mPKMFontConverter.Add(&H9DC, &HC6DC)
                mPKMFontConverter.Add(&H9DD, &HC6DD)
                mPKMFontConverter.Add(&H9DE, &HC6E0)
                mPKMFontConverter.Add(&H9DF, &HC6E1)
                mPKMFontConverter.Add(&H9E0, &HC6E8)
                mPKMFontConverter.Add(&H9E1, &HC6E9)
                mPKMFontConverter.Add(&H9E2, &HC6EC)
                mPKMFontConverter.Add(&H9E3, &HC6F0)
                mPKMFontConverter.Add(&H9E4, &HC6F8)
                mPKMFontConverter.Add(&H9E5, &HC6F9)
                mPKMFontConverter.Add(&H9E6, &HC6FD)
                mPKMFontConverter.Add(&H9E7, &HC704)
                mPKMFontConverter.Add(&H9E8, &HC705)
                mPKMFontConverter.Add(&H9E9, &HC708)
                mPKMFontConverter.Add(&H9EA, &HC70C)
                mPKMFontConverter.Add(&H9EB, &HC714)
                mPKMFontConverter.Add(&H9EC, &HC715)
                mPKMFontConverter.Add(&H9ED, &HC717)
                mPKMFontConverter.Add(&H9EE, &HC719)
                mPKMFontConverter.Add(&H9EF, &HC720)
                mPKMFontConverter.Add(&H9F0, &HC721)
                mPKMFontConverter.Add(&H9F1, &HC724)
                mPKMFontConverter.Add(&H9F2, &HC728)
                mPKMFontConverter.Add(&H9F3, &HC730)
                mPKMFontConverter.Add(&H9F4, &HC731)
                mPKMFontConverter.Add(&H9F5, &HC733)
                mPKMFontConverter.Add(&H9F6, &HC735)
                mPKMFontConverter.Add(&H9F7, &HC737)
                mPKMFontConverter.Add(&H9F8, &HC73C)
                mPKMFontConverter.Add(&H9F9, &HC73D)
                mPKMFontConverter.Add(&H9FA, &HC740)
                mPKMFontConverter.Add(&H9FB, &HC744)
                mPKMFontConverter.Add(&H9FC, &HC74A)
                mPKMFontConverter.Add(&H9FD, &HC74C)
                mPKMFontConverter.Add(&H9FE, &HC74D)
                mPKMFontConverter.Add(&H9FF, &HC74F)
                mPKMFontConverter.Add(&HA00, &HC751)
                mPKMFontConverter.Add(&HA01, &HC752)
                mPKMFontConverter.Add(&HA02, &HC753)
                mPKMFontConverter.Add(&HA03, &HC754)
                mPKMFontConverter.Add(&HA04, &HC755)
                mPKMFontConverter.Add(&HA05, &HC756)
                mPKMFontConverter.Add(&HA06, &HC757)
                mPKMFontConverter.Add(&HA07, &HC758)
                mPKMFontConverter.Add(&HA08, &HC75C)
                mPKMFontConverter.Add(&HA09, &HC760)
                mPKMFontConverter.Add(&HA0A, &HC768)
                mPKMFontConverter.Add(&HA0B, &HC76B)
                mPKMFontConverter.Add(&HA0C, &HC774)
                mPKMFontConverter.Add(&HA0D, &HC775)
                mPKMFontConverter.Add(&HA0E, &HC778)
                mPKMFontConverter.Add(&HA0F, &HC77C)
                mPKMFontConverter.Add(&HA10, &HC77D)
                mPKMFontConverter.Add(&HA11, &HC77E)
                mPKMFontConverter.Add(&HA12, &HC783)
                mPKMFontConverter.Add(&HA13, &HC784)
                mPKMFontConverter.Add(&HA14, &HC785)
                mPKMFontConverter.Add(&HA15, &HC787)
                mPKMFontConverter.Add(&HA16, &HC788)
                mPKMFontConverter.Add(&HA17, &HC789)
                mPKMFontConverter.Add(&HA18, &HC78A)
                mPKMFontConverter.Add(&HA19, &HC78E)
                mPKMFontConverter.Add(&HA1A, &HC790)
                mPKMFontConverter.Add(&HA1B, &HC791)
                mPKMFontConverter.Add(&HA1C, &HC794)
                mPKMFontConverter.Add(&HA1D, &HC796)
                mPKMFontConverter.Add(&HA1E, &HC797)
                mPKMFontConverter.Add(&HA1F, &HC798)
                mPKMFontConverter.Add(&HA20, &HC79A)
                mPKMFontConverter.Add(&HA21, &HC7A0)
                mPKMFontConverter.Add(&HA22, &HC7A1)
                mPKMFontConverter.Add(&HA23, &HC7A3)
                mPKMFontConverter.Add(&HA24, &HC7A4)
                mPKMFontConverter.Add(&HA25, &HC7A5)
                mPKMFontConverter.Add(&HA26, &HC7A6)
                mPKMFontConverter.Add(&HA27, &HC7AC)
                mPKMFontConverter.Add(&HA28, &HC7AD)
                mPKMFontConverter.Add(&HA29, &HC7B0)
                mPKMFontConverter.Add(&HA2A, &HC7B4)
                mPKMFontConverter.Add(&HA2B, &HC7BC)
                mPKMFontConverter.Add(&HA2C, &HC7BD)
                mPKMFontConverter.Add(&HA2D, &HC7BF)
                mPKMFontConverter.Add(&HA2E, &HC7C0)
                mPKMFontConverter.Add(&HA2F, &HC7C1)
                mPKMFontConverter.Add(&HA30, &HC7C8)
                mPKMFontConverter.Add(&HA31, &HC7C9)
                mPKMFontConverter.Add(&HA32, &HC7CC)
                mPKMFontConverter.Add(&HA33, &HC7CE)
                mPKMFontConverter.Add(&HA34, &HC7D0)
                mPKMFontConverter.Add(&HA35, &HC7D8)
                mPKMFontConverter.Add(&HA36, &HC7DD)
                mPKMFontConverter.Add(&HA37, &HC7E4)
                mPKMFontConverter.Add(&HA38, &HC7E8)
                mPKMFontConverter.Add(&HA39, &HC7EC)
                mPKMFontConverter.Add(&HA3A, &HC800)
                mPKMFontConverter.Add(&HA3B, &HC801)
                mPKMFontConverter.Add(&HA3C, &HC804)
                mPKMFontConverter.Add(&HA3D, &HC808)
                mPKMFontConverter.Add(&HA3E, &HC80A)
                mPKMFontConverter.Add(&HA3F, &HC810)
                mPKMFontConverter.Add(&HA40, &HC811)
                mPKMFontConverter.Add(&HA41, &HC813)
                mPKMFontConverter.Add(&HA42, &HC815)
                mPKMFontConverter.Add(&HA43, &HC816)
                mPKMFontConverter.Add(&HA44, &HC81C)
                mPKMFontConverter.Add(&HA45, &HC81D)
                mPKMFontConverter.Add(&HA46, &HC820)
                mPKMFontConverter.Add(&HA47, &HC824)
                mPKMFontConverter.Add(&HA48, &HC82C)
                mPKMFontConverter.Add(&HA49, &HC82D)
                mPKMFontConverter.Add(&HA4A, &HC82F)
                mPKMFontConverter.Add(&HA4B, &HC831)
                mPKMFontConverter.Add(&HA4C, &HC838)
                mPKMFontConverter.Add(&HA4D, &HC83C)
                mPKMFontConverter.Add(&HA4E, &HC840)
                mPKMFontConverter.Add(&HA4F, &HC848)
                mPKMFontConverter.Add(&HA50, &HC849)
                mPKMFontConverter.Add(&HA51, &HC84C)
                mPKMFontConverter.Add(&HA52, &HC84D)
                mPKMFontConverter.Add(&HA53, &HC854)
                mPKMFontConverter.Add(&HA54, &HC870)
                mPKMFontConverter.Add(&HA55, &HC871)
                mPKMFontConverter.Add(&HA56, &HC874)
                mPKMFontConverter.Add(&HA57, &HC878)
                mPKMFontConverter.Add(&HA58, &HC87A)
                mPKMFontConverter.Add(&HA59, &HC880)
                mPKMFontConverter.Add(&HA5A, &HC881)
                mPKMFontConverter.Add(&HA5B, &HC883)
                mPKMFontConverter.Add(&HA5C, &HC885)
                mPKMFontConverter.Add(&HA5D, &HC886)
                mPKMFontConverter.Add(&HA5E, &HC887)
                mPKMFontConverter.Add(&HA5F, &HC88B)
                mPKMFontConverter.Add(&HA60, &HC88C)
                mPKMFontConverter.Add(&HA61, &HC88D)
                mPKMFontConverter.Add(&HA62, &HC894)
                mPKMFontConverter.Add(&HA63, &HC89D)
                mPKMFontConverter.Add(&HA64, &HC89F)
                mPKMFontConverter.Add(&HA65, &HC8A1)
                mPKMFontConverter.Add(&HA66, &HC8A8)
                mPKMFontConverter.Add(&HA67, &HC8BC)
                mPKMFontConverter.Add(&HA68, &HC8BD)
                mPKMFontConverter.Add(&HA69, &HC8C4)
                mPKMFontConverter.Add(&HA6A, &HC8C8)
                mPKMFontConverter.Add(&HA6B, &HC8CC)
                mPKMFontConverter.Add(&HA6C, &HC8D4)
                mPKMFontConverter.Add(&HA6D, &HC8D5)
                mPKMFontConverter.Add(&HA6E, &HC8D7)
                mPKMFontConverter.Add(&HA6F, &HC8D9)
                mPKMFontConverter.Add(&HA70, &HC8E0)
                mPKMFontConverter.Add(&HA71, &HC8E1)
                mPKMFontConverter.Add(&HA72, &HC8E4)
                mPKMFontConverter.Add(&HA73, &HC8F5)
                mPKMFontConverter.Add(&HA74, &HC8FC)
                mPKMFontConverter.Add(&HA75, &HC8FD)
                mPKMFontConverter.Add(&HA76, &HC900)
                mPKMFontConverter.Add(&HA77, &HC904)
                mPKMFontConverter.Add(&HA78, &HC905)
                mPKMFontConverter.Add(&HA79, &HC906)
                mPKMFontConverter.Add(&HA7A, &HC90C)
                mPKMFontConverter.Add(&HA7B, &HC90D)
                mPKMFontConverter.Add(&HA7C, &HC90F)
                mPKMFontConverter.Add(&HA7D, &HC911)
                mPKMFontConverter.Add(&HA7E, &HC918)
                mPKMFontConverter.Add(&HA7F, &HC92C)
                mPKMFontConverter.Add(&HA80, &HC934)
                mPKMFontConverter.Add(&HA81, &HC950)
                mPKMFontConverter.Add(&HA82, &HC951)
                mPKMFontConverter.Add(&HA83, &HC954)
                mPKMFontConverter.Add(&HA84, &HC958)
                mPKMFontConverter.Add(&HA85, &HC960)
                mPKMFontConverter.Add(&HA86, &HC961)
                mPKMFontConverter.Add(&HA87, &HC963)
                mPKMFontConverter.Add(&HA88, &HC96C)
                mPKMFontConverter.Add(&HA89, &HC970)
                mPKMFontConverter.Add(&HA8A, &HC974)
                mPKMFontConverter.Add(&HA8B, &HC97C)
                mPKMFontConverter.Add(&HA8C, &HC988)
                mPKMFontConverter.Add(&HA8D, &HC989)
                mPKMFontConverter.Add(&HA8E, &HC98C)
                mPKMFontConverter.Add(&HA8F, &HC990)
                mPKMFontConverter.Add(&HA90, &HC998)
                mPKMFontConverter.Add(&HA91, &HC999)
                mPKMFontConverter.Add(&HA92, &HC99B)
                mPKMFontConverter.Add(&HA93, &HC99D)
                mPKMFontConverter.Add(&HA94, &HC9C0)
                mPKMFontConverter.Add(&HA95, &HC9C1)
                mPKMFontConverter.Add(&HA96, &HC9C4)
                mPKMFontConverter.Add(&HA97, &HC9C7)
                mPKMFontConverter.Add(&HA98, &HC9C8)
                mPKMFontConverter.Add(&HA99, &HC9CA)
                mPKMFontConverter.Add(&HA9A, &HC9D0)
                mPKMFontConverter.Add(&HA9B, &HC9D1)
                mPKMFontConverter.Add(&HA9C, &HC9D3)
                mPKMFontConverter.Add(&HA9D, &HC9D5)
                mPKMFontConverter.Add(&HA9E, &HC9D6)
                mPKMFontConverter.Add(&HA9F, &HC9D9)
                mPKMFontConverter.Add(&HAA0, &HC9DA)
                mPKMFontConverter.Add(&HAA1, &HC9DC)
                mPKMFontConverter.Add(&HAA2, &HC9DD)
                mPKMFontConverter.Add(&HAA3, &HC9E0)
                mPKMFontConverter.Add(&HAA4, &HC9E2)
                mPKMFontConverter.Add(&HAA5, &HC9E4)
                mPKMFontConverter.Add(&HAA6, &HC9E7)
                mPKMFontConverter.Add(&HAA7, &HC9EC)
                mPKMFontConverter.Add(&HAA8, &HC9ED)
                mPKMFontConverter.Add(&HAA9, &HC9EF)
                mPKMFontConverter.Add(&HAAA, &HC9F0)
                mPKMFontConverter.Add(&HAAB, &HC9F1)
                mPKMFontConverter.Add(&HAAC, &HC9F8)
                mPKMFontConverter.Add(&HAAD, &HC9F9)
                mPKMFontConverter.Add(&HAAE, &HC9FC)
                mPKMFontConverter.Add(&HAAF, &HCA00)
                mPKMFontConverter.Add(&HAB0, &HCA08)
                mPKMFontConverter.Add(&HAB1, &HCA09)
                mPKMFontConverter.Add(&HAB2, &HCA0B)
                mPKMFontConverter.Add(&HAB3, &HCA0C)
                mPKMFontConverter.Add(&HAB4, &HCA0D)
                mPKMFontConverter.Add(&HAB5, &HCA14)
                mPKMFontConverter.Add(&HAB6, &HCA18)
                mPKMFontConverter.Add(&HAB7, &HCA29)
                mPKMFontConverter.Add(&HAB8, &HCA4C)
                mPKMFontConverter.Add(&HAB9, &HCA4D)
                mPKMFontConverter.Add(&HABA, &HCA50)
                mPKMFontConverter.Add(&HABB, &HCA54)
                mPKMFontConverter.Add(&HABC, &HCA5C)
                mPKMFontConverter.Add(&HABD, &HCA5D)
                mPKMFontConverter.Add(&HABE, &HCA5F)
                mPKMFontConverter.Add(&HABF, &HCA60)
                mPKMFontConverter.Add(&HAC0, &HCA61)
                mPKMFontConverter.Add(&HAC1, &HCA68)
                mPKMFontConverter.Add(&HAC2, &HCA7D)
                mPKMFontConverter.Add(&HAC3, &HCA84)
                mPKMFontConverter.Add(&HAC4, &HCA98)
                mPKMFontConverter.Add(&HAC5, &HCABC)
                mPKMFontConverter.Add(&HAC6, &HCABD)
                mPKMFontConverter.Add(&HAC7, &HCAC0)
                mPKMFontConverter.Add(&HAC8, &HCAC4)
                mPKMFontConverter.Add(&HAC9, &HCACC)
                mPKMFontConverter.Add(&HACA, &HCACD)
                mPKMFontConverter.Add(&HACB, &HCACF)
                mPKMFontConverter.Add(&HACC, &HCAD1)
                mPKMFontConverter.Add(&HACD, &HCAD3)
                mPKMFontConverter.Add(&HACE, &HCAD8)
                mPKMFontConverter.Add(&HACF, &HCAD9)
                mPKMFontConverter.Add(&HAD0, &HCAE0)
                mPKMFontConverter.Add(&HAD1, &HCAEC)
                mPKMFontConverter.Add(&HAD2, &HCAF4)
                mPKMFontConverter.Add(&HAD3, &HCB08)
                mPKMFontConverter.Add(&HAD4, &HCB10)
                mPKMFontConverter.Add(&HAD5, &HCB14)
                mPKMFontConverter.Add(&HAD6, &HCB18)
                mPKMFontConverter.Add(&HAD7, &HCB20)
                mPKMFontConverter.Add(&HAD8, &HCB21)
                mPKMFontConverter.Add(&HAD9, &HCB41)
                mPKMFontConverter.Add(&HADA, &HCB48)
                mPKMFontConverter.Add(&HADB, &HCB49)
                mPKMFontConverter.Add(&HADC, &HCB4C)
                mPKMFontConverter.Add(&HADD, &HCB50)
                mPKMFontConverter.Add(&HADE, &HCB58)
                mPKMFontConverter.Add(&HADF, &HCB59)
                mPKMFontConverter.Add(&HAE0, &HCB5D)
                mPKMFontConverter.Add(&HAE1, &HCB64)
                mPKMFontConverter.Add(&HAE2, &HCB78)
                mPKMFontConverter.Add(&HAE3, &HCB79)
                mPKMFontConverter.Add(&HAE4, &HCB9C)
                mPKMFontConverter.Add(&HAE5, &HCBB8)
                mPKMFontConverter.Add(&HAE6, &HCBD4)
                mPKMFontConverter.Add(&HAE7, &HCBE4)
                mPKMFontConverter.Add(&HAE8, &HCBE7)
                mPKMFontConverter.Add(&HAE9, &HCBE9)
                mPKMFontConverter.Add(&HAEA, &HCC0C)
                mPKMFontConverter.Add(&HAEB, &HCC0D)
                mPKMFontConverter.Add(&HAEC, &HCC10)
                mPKMFontConverter.Add(&HAED, &HCC14)
                mPKMFontConverter.Add(&HAEE, &HCC1C)
                mPKMFontConverter.Add(&HAEF, &HCC1D)
                mPKMFontConverter.Add(&HAF0, &HCC21)
                mPKMFontConverter.Add(&HAF1, &HCC22)
                mPKMFontConverter.Add(&HAF2, &HCC27)
                mPKMFontConverter.Add(&HAF3, &HCC28)
                mPKMFontConverter.Add(&HAF4, &HCC29)
                mPKMFontConverter.Add(&HAF5, &HCC2C)
                mPKMFontConverter.Add(&HAF6, &HCC2E)
                mPKMFontConverter.Add(&HAF7, &HCC30)
                mPKMFontConverter.Add(&HAF8, &HCC38)
                mPKMFontConverter.Add(&HAF9, &HCC39)
                mPKMFontConverter.Add(&HAFA, &HCC3B)
                mPKMFontConverter.Add(&HAFB, &HCC3C)
                mPKMFontConverter.Add(&HAFC, &HCC3D)
                mPKMFontConverter.Add(&HAFD, &HCC3E)
                mPKMFontConverter.Add(&HAFE, &HCC44)
                mPKMFontConverter.Add(&HAFF, &HCC45)
                mPKMFontConverter.Add(&HB00, &HCC48)
                mPKMFontConverter.Add(&HB01, &HCC4C)
                mPKMFontConverter.Add(&HB02, &HCC54)
                mPKMFontConverter.Add(&HB03, &HCC55)
                mPKMFontConverter.Add(&HB04, &HCC57)
                mPKMFontConverter.Add(&HB05, &HCC58)
                mPKMFontConverter.Add(&HB06, &HCC59)
                mPKMFontConverter.Add(&HB07, &HCC60)
                mPKMFontConverter.Add(&HB08, &HCC64)
                mPKMFontConverter.Add(&HB09, &HCC66)
                mPKMFontConverter.Add(&HB0A, &HCC68)
                mPKMFontConverter.Add(&HB0B, &HCC70)
                mPKMFontConverter.Add(&HB0C, &HCC75)
                mPKMFontConverter.Add(&HB0D, &HCC98)
                mPKMFontConverter.Add(&HB0E, &HCC99)
                mPKMFontConverter.Add(&HB0F, &HCC9C)
                mPKMFontConverter.Add(&HB10, &HCCA0)
                mPKMFontConverter.Add(&HB11, &HCCA8)
                mPKMFontConverter.Add(&HB12, &HCCA9)
                mPKMFontConverter.Add(&HB13, &HCCAB)
                mPKMFontConverter.Add(&HB14, &HCCAC)
                mPKMFontConverter.Add(&HB15, &HCCAD)
                mPKMFontConverter.Add(&HB16, &HCCB4)
                mPKMFontConverter.Add(&HB17, &HCCB5)
                mPKMFontConverter.Add(&HB18, &HCCB8)
                mPKMFontConverter.Add(&HB19, &HCCBC)
                mPKMFontConverter.Add(&HB1A, &HCCC4)
                mPKMFontConverter.Add(&HB1B, &HCCC5)
                mPKMFontConverter.Add(&HB1C, &HCCC7)
                mPKMFontConverter.Add(&HB1D, &HCCC9)
                mPKMFontConverter.Add(&HB1E, &HCCD0)
                mPKMFontConverter.Add(&HB1F, &HCCD4)
                mPKMFontConverter.Add(&HB20, &HCCE4)
                mPKMFontConverter.Add(&HB21, &HCCEC)
                mPKMFontConverter.Add(&HB22, &HCCF0)
                mPKMFontConverter.Add(&HB23, &HCD01)
                mPKMFontConverter.Add(&HB24, &HCD08)
                mPKMFontConverter.Add(&HB25, &HCD09)
                mPKMFontConverter.Add(&HB26, &HCD0C)
                mPKMFontConverter.Add(&HB27, &HCD10)
                mPKMFontConverter.Add(&HB28, &HCD18)
                mPKMFontConverter.Add(&HB29, &HCD19)
                mPKMFontConverter.Add(&HB2A, &HCD1B)
                mPKMFontConverter.Add(&HB2B, &HCD1D)
                mPKMFontConverter.Add(&HB2C, &HCD24)
                mPKMFontConverter.Add(&HB2D, &HCD28)
                mPKMFontConverter.Add(&HB2E, &HCD2C)
                mPKMFontConverter.Add(&HB2F, &HCD39)
                mPKMFontConverter.Add(&HB30, &HCD5C)
                mPKMFontConverter.Add(&HB31, &HCD60)
                mPKMFontConverter.Add(&HB32, &HCD64)
                mPKMFontConverter.Add(&HB33, &HCD6C)
                mPKMFontConverter.Add(&HB34, &HCD6D)
                mPKMFontConverter.Add(&HB35, &HCD6F)
                mPKMFontConverter.Add(&HB36, &HCD71)
                mPKMFontConverter.Add(&HB37, &HCD78)
                mPKMFontConverter.Add(&HB38, &HCD88)
                mPKMFontConverter.Add(&HB39, &HCD94)
                mPKMFontConverter.Add(&HB3A, &HCD95)
                mPKMFontConverter.Add(&HB3B, &HCD98)
                mPKMFontConverter.Add(&HB3C, &HCD9C)
                mPKMFontConverter.Add(&HB3D, &HCDA4)
                mPKMFontConverter.Add(&HB3E, &HCDA5)
                mPKMFontConverter.Add(&HB3F, &HCDA7)
                mPKMFontConverter.Add(&HB40, &HCDA9)
                mPKMFontConverter.Add(&HB41, &HCDB0)
                mPKMFontConverter.Add(&HB42, &HCDC4)
                mPKMFontConverter.Add(&HB43, &HCDCC)
                mPKMFontConverter.Add(&HB44, &HCDD0)
                mPKMFontConverter.Add(&HB45, &HCDE8)
                mPKMFontConverter.Add(&HB46, &HCDEC)
                mPKMFontConverter.Add(&HB47, &HCDF0)
                mPKMFontConverter.Add(&HB48, &HCDF8)
                mPKMFontConverter.Add(&HB49, &HCDF9)
                mPKMFontConverter.Add(&HB4A, &HCDFB)
                mPKMFontConverter.Add(&HB4B, &HCDFD)
                mPKMFontConverter.Add(&HB4C, &HCE04)
                mPKMFontConverter.Add(&HB4D, &HCE08)
                mPKMFontConverter.Add(&HB4E, &HCE0C)
                mPKMFontConverter.Add(&HB4F, &HCE14)
                mPKMFontConverter.Add(&HB50, &HCE19)
                mPKMFontConverter.Add(&HB51, &HCE20)
                mPKMFontConverter.Add(&HB52, &HCE21)
                mPKMFontConverter.Add(&HB53, &HCE24)
                mPKMFontConverter.Add(&HB54, &HCE28)
                mPKMFontConverter.Add(&HB55, &HCE30)
                mPKMFontConverter.Add(&HB56, &HCE31)
                mPKMFontConverter.Add(&HB57, &HCE33)
                mPKMFontConverter.Add(&HB58, &HCE35)
                mPKMFontConverter.Add(&HB59, &HCE58)
                mPKMFontConverter.Add(&HB5A, &HCE59)
                mPKMFontConverter.Add(&HB5B, &HCE5C)
                mPKMFontConverter.Add(&HB5C, &HCE5F)
                mPKMFontConverter.Add(&HB5D, &HCE60)
                mPKMFontConverter.Add(&HB5E, &HCE61)
                mPKMFontConverter.Add(&HB5F, &HCE68)
                mPKMFontConverter.Add(&HB60, &HCE69)
                mPKMFontConverter.Add(&HB61, &HCE6B)
                mPKMFontConverter.Add(&HB62, &HCE6D)
                mPKMFontConverter.Add(&HB63, &HCE74)
                mPKMFontConverter.Add(&HB64, &HCE75)
                mPKMFontConverter.Add(&HB65, &HCE78)
                mPKMFontConverter.Add(&HB66, &HCE7C)
                mPKMFontConverter.Add(&HB67, &HCE84)
                mPKMFontConverter.Add(&HB68, &HCE85)
                mPKMFontConverter.Add(&HB69, &HCE87)
                mPKMFontConverter.Add(&HB6A, &HCE89)
                mPKMFontConverter.Add(&HB6B, &HCE90)
                mPKMFontConverter.Add(&HB6C, &HCE91)
                mPKMFontConverter.Add(&HB6D, &HCE94)
                mPKMFontConverter.Add(&HB6E, &HCE98)
                mPKMFontConverter.Add(&HB6F, &HCEA0)
                mPKMFontConverter.Add(&HB70, &HCEA1)
                mPKMFontConverter.Add(&HB71, &HCEA3)
                mPKMFontConverter.Add(&HB72, &HCEA4)
                mPKMFontConverter.Add(&HB73, &HCEA5)
                mPKMFontConverter.Add(&HB74, &HCEAC)
                mPKMFontConverter.Add(&HB75, &HCEAD)
                mPKMFontConverter.Add(&HB76, &HCEC1)
                mPKMFontConverter.Add(&HB77, &HCEE4)
                mPKMFontConverter.Add(&HB78, &HCEE5)
                mPKMFontConverter.Add(&HB79, &HCEE8)
                mPKMFontConverter.Add(&HB7A, &HCEEB)
                mPKMFontConverter.Add(&HB7B, &HCEEC)
                mPKMFontConverter.Add(&HB7C, &HCEF4)
                mPKMFontConverter.Add(&HB7D, &HCEF5)
                mPKMFontConverter.Add(&HB7E, &HCEF7)
                mPKMFontConverter.Add(&HB7F, &HCEF8)
                mPKMFontConverter.Add(&HB80, &HCEF9)
                mPKMFontConverter.Add(&HB81, &HCF00)
                mPKMFontConverter.Add(&HB82, &HCF01)
                mPKMFontConverter.Add(&HB83, &HCF04)
                mPKMFontConverter.Add(&HB84, &HCF08)
                mPKMFontConverter.Add(&HB85, &HCF10)
                mPKMFontConverter.Add(&HB86, &HCF11)
                mPKMFontConverter.Add(&HB87, &HCF13)
                mPKMFontConverter.Add(&HB88, &HCF15)
                mPKMFontConverter.Add(&HB89, &HCF1C)
                mPKMFontConverter.Add(&HB8A, &HCF20)
                mPKMFontConverter.Add(&HB8B, &HCF24)
                mPKMFontConverter.Add(&HB8C, &HCF2C)
                mPKMFontConverter.Add(&HB8D, &HCF2D)
                mPKMFontConverter.Add(&HB8E, &HCF2F)
                mPKMFontConverter.Add(&HB8F, &HCF30)
                mPKMFontConverter.Add(&HB90, &HCF31)
                mPKMFontConverter.Add(&HB91, &HCF38)
                mPKMFontConverter.Add(&HB92, &HCF54)
                mPKMFontConverter.Add(&HB93, &HCF55)
                mPKMFontConverter.Add(&HB94, &HCF58)
                mPKMFontConverter.Add(&HB95, &HCF5C)
                mPKMFontConverter.Add(&HB96, &HCF64)
                mPKMFontConverter.Add(&HB97, &HCF65)
                mPKMFontConverter.Add(&HB98, &HCF67)
                mPKMFontConverter.Add(&HB99, &HCF69)
                mPKMFontConverter.Add(&HB9A, &HCF70)
                mPKMFontConverter.Add(&HB9B, &HCF71)
                mPKMFontConverter.Add(&HB9C, &HCF74)
                mPKMFontConverter.Add(&HB9D, &HCF78)
                mPKMFontConverter.Add(&HB9E, &HCF80)
                mPKMFontConverter.Add(&HB9F, &HCF85)
                mPKMFontConverter.Add(&HBA0, &HCF8C)
                mPKMFontConverter.Add(&HBA1, &HCFA1)
                mPKMFontConverter.Add(&HBA2, &HCFA8)
                mPKMFontConverter.Add(&HBA3, &HCFB0)
                mPKMFontConverter.Add(&HBA4, &HCFC4)
                mPKMFontConverter.Add(&HBA5, &HCFE0)
                mPKMFontConverter.Add(&HBA6, &HCFE1)
                mPKMFontConverter.Add(&HBA7, &HCFE4)
                mPKMFontConverter.Add(&HBA8, &HCFE8)
                mPKMFontConverter.Add(&HBA9, &HCFF0)
                mPKMFontConverter.Add(&HBAA, &HCFF1)
                mPKMFontConverter.Add(&HBAB, &HCFF3)
                mPKMFontConverter.Add(&HBAC, &HCFF5)
                mPKMFontConverter.Add(&HBAD, &HCFFC)
                mPKMFontConverter.Add(&HBAE, &HD000)
                mPKMFontConverter.Add(&HBAF, &HD004)
                mPKMFontConverter.Add(&HBB0, &HD011)
                mPKMFontConverter.Add(&HBB1, &HD018)
                mPKMFontConverter.Add(&HBB2, &HD02D)
                mPKMFontConverter.Add(&HBB3, &HD034)
                mPKMFontConverter.Add(&HBB4, &HD035)
                mPKMFontConverter.Add(&HBB5, &HD038)
                mPKMFontConverter.Add(&HBB6, &HD03C)
                mPKMFontConverter.Add(&HBB7, &HD044)
                mPKMFontConverter.Add(&HBB8, &HD045)
                mPKMFontConverter.Add(&HBB9, &HD047)
                mPKMFontConverter.Add(&HBBA, &HD049)
                mPKMFontConverter.Add(&HBBB, &HD050)
                mPKMFontConverter.Add(&HBBC, &HD054)
                mPKMFontConverter.Add(&HBBD, &HD058)
                mPKMFontConverter.Add(&HBBE, &HD060)
                mPKMFontConverter.Add(&HBBF, &HD06C)
                mPKMFontConverter.Add(&HBC0, &HD06D)
                mPKMFontConverter.Add(&HBC1, &HD070)
                mPKMFontConverter.Add(&HBC2, &HD074)
                mPKMFontConverter.Add(&HBC3, &HD07C)
                mPKMFontConverter.Add(&HBC4, &HD07D)
                mPKMFontConverter.Add(&HBC5, &HD081)
                mPKMFontConverter.Add(&HBC6, &HD0A4)
                mPKMFontConverter.Add(&HBC7, &HD0A5)
                mPKMFontConverter.Add(&HBC8, &HD0A8)
                mPKMFontConverter.Add(&HBC9, &HD0AC)
                mPKMFontConverter.Add(&HBCA, &HD0B4)
                mPKMFontConverter.Add(&HBCB, &HD0B5)
                mPKMFontConverter.Add(&HBCC, &HD0B7)
                mPKMFontConverter.Add(&HBCD, &HD0B9)
                mPKMFontConverter.Add(&HBCE, &HD0C0)
                mPKMFontConverter.Add(&HBCF, &HD0C1)
                mPKMFontConverter.Add(&HBD0, &HD0C4)
                mPKMFontConverter.Add(&HBD1, &HD0C8)
                mPKMFontConverter.Add(&HBD2, &HD0C9)
                mPKMFontConverter.Add(&HBD3, &HD0D0)
                mPKMFontConverter.Add(&HBD4, &HD0D1)
                mPKMFontConverter.Add(&HBD5, &HD0D3)
                mPKMFontConverter.Add(&HBD6, &HD0D4)
                mPKMFontConverter.Add(&HBD7, &HD0D5)
                mPKMFontConverter.Add(&HBD8, &HD0DC)
                mPKMFontConverter.Add(&HBD9, &HD0DD)
                mPKMFontConverter.Add(&HBDA, &HD0E0)
                mPKMFontConverter.Add(&HBDB, &HD0E4)
                mPKMFontConverter.Add(&HBDC, &HD0EC)
                mPKMFontConverter.Add(&HBDD, &HD0ED)
                mPKMFontConverter.Add(&HBDE, &HD0EF)
                mPKMFontConverter.Add(&HBDF, &HD0F0)
                mPKMFontConverter.Add(&HBE0, &HD0F1)
                mPKMFontConverter.Add(&HBE1, &HD0F8)
                mPKMFontConverter.Add(&HBE2, &HD10D)
                mPKMFontConverter.Add(&HBE3, &HD130)
                mPKMFontConverter.Add(&HBE4, &HD131)
                mPKMFontConverter.Add(&HBE5, &HD134)
                mPKMFontConverter.Add(&HBE6, &HD138)
                mPKMFontConverter.Add(&HBE7, &HD13A)
                mPKMFontConverter.Add(&HBE8, &HD140)
                mPKMFontConverter.Add(&HBE9, &HD141)
                mPKMFontConverter.Add(&HBEA, &HD143)
                mPKMFontConverter.Add(&HBEB, &HD144)
                mPKMFontConverter.Add(&HBEC, &HD145)
                mPKMFontConverter.Add(&HBED, &HD14C)
                mPKMFontConverter.Add(&HBEE, &HD14D)
                mPKMFontConverter.Add(&HBEF, &HD150)
                mPKMFontConverter.Add(&HBF0, &HD154)
                mPKMFontConverter.Add(&HBF1, &HD15C)
                mPKMFontConverter.Add(&HBF2, &HD15D)
                mPKMFontConverter.Add(&HBF3, &HD15F)
                mPKMFontConverter.Add(&HBF4, &HD161)
                mPKMFontConverter.Add(&HBF5, &HD168)
                mPKMFontConverter.Add(&HBF6, &HD16C)
                mPKMFontConverter.Add(&HBF7, &HD17C)
                mPKMFontConverter.Add(&HBF8, &HD184)
                mPKMFontConverter.Add(&HBF9, &HD188)
                mPKMFontConverter.Add(&HBFA, &HD1A0)
                mPKMFontConverter.Add(&HBFB, &HD1A1)
                mPKMFontConverter.Add(&HBFC, &HD1A4)
                mPKMFontConverter.Add(&HBFD, &HD1A8)
                mPKMFontConverter.Add(&HBFE, &HD1B0)
                mPKMFontConverter.Add(&HBFF, &HD1B1)
                mPKMFontConverter.Add(&HC00, &HD1B3)
                mPKMFontConverter.Add(&HC01, &HD1B5)
                mPKMFontConverter.Add(&HC02, &HD1BA)
                mPKMFontConverter.Add(&HC03, &HD1BC)
                mPKMFontConverter.Add(&HC04, &HD1C0)
                mPKMFontConverter.Add(&HC05, &HD1D8)
                mPKMFontConverter.Add(&HC06, &HD1F4)
                mPKMFontConverter.Add(&HC07, &HD1F8)
                mPKMFontConverter.Add(&HC08, &HD207)
                mPKMFontConverter.Add(&HC09, &HD209)
                mPKMFontConverter.Add(&HC0A, &HD210)
                mPKMFontConverter.Add(&HC0B, &HD22C)
                mPKMFontConverter.Add(&HC0C, &HD22D)
                mPKMFontConverter.Add(&HC0D, &HD230)
                mPKMFontConverter.Add(&HC0E, &HD234)
                mPKMFontConverter.Add(&HC0F, &HD23C)
                mPKMFontConverter.Add(&HC10, &HD23D)
                mPKMFontConverter.Add(&HC11, &HD23F)
                mPKMFontConverter.Add(&HC12, &HD241)
                mPKMFontConverter.Add(&HC13, &HD248)
                mPKMFontConverter.Add(&HC14, &HD25C)
                mPKMFontConverter.Add(&HC15, &HD264)
                mPKMFontConverter.Add(&HC16, &HD280)
                mPKMFontConverter.Add(&HC17, &HD281)
                mPKMFontConverter.Add(&HC18, &HD284)
                mPKMFontConverter.Add(&HC19, &HD288)
                mPKMFontConverter.Add(&HC1A, &HD290)
                mPKMFontConverter.Add(&HC1B, &HD291)
                mPKMFontConverter.Add(&HC1C, &HD295)
                mPKMFontConverter.Add(&HC1D, &HD29C)
                mPKMFontConverter.Add(&HC1E, &HD2A0)
                mPKMFontConverter.Add(&HC1F, &HD2A4)
                mPKMFontConverter.Add(&HC20, &HD2AC)
                mPKMFontConverter.Add(&HC21, &HD2B1)
                mPKMFontConverter.Add(&HC22, &HD2B8)
                mPKMFontConverter.Add(&HC23, &HD2B9)
                mPKMFontConverter.Add(&HC24, &HD2BC)
                mPKMFontConverter.Add(&HC25, &HD2BF)
                mPKMFontConverter.Add(&HC26, &HD2C0)
                mPKMFontConverter.Add(&HC27, &HD2C2)
                mPKMFontConverter.Add(&HC28, &HD2C8)
                mPKMFontConverter.Add(&HC29, &HD2C9)
                mPKMFontConverter.Add(&HC2A, &HD2CB)
                mPKMFontConverter.Add(&HC2B, &HD2D4)
                mPKMFontConverter.Add(&HC2C, &HD2D8)
                mPKMFontConverter.Add(&HC2D, &HD2DC)
                mPKMFontConverter.Add(&HC2E, &HD2E4)
                mPKMFontConverter.Add(&HC2F, &HD2E5)
                mPKMFontConverter.Add(&HC30, &HD2F0)
                mPKMFontConverter.Add(&HC31, &HD2F1)
                mPKMFontConverter.Add(&HC32, &HD2F4)
                mPKMFontConverter.Add(&HC33, &HD2F8)
                mPKMFontConverter.Add(&HC34, &HD300)
                mPKMFontConverter.Add(&HC35, &HD301)
                mPKMFontConverter.Add(&HC36, &HD303)
                mPKMFontConverter.Add(&HC37, &HD305)
                mPKMFontConverter.Add(&HC38, &HD30C)
                mPKMFontConverter.Add(&HC39, &HD30D)
                mPKMFontConverter.Add(&HC3A, &HD30E)
                mPKMFontConverter.Add(&HC3B, &HD310)
                mPKMFontConverter.Add(&HC3C, &HD314)
                mPKMFontConverter.Add(&HC3D, &HD316)
                mPKMFontConverter.Add(&HC3E, &HD31C)
                mPKMFontConverter.Add(&HC3F, &HD31D)
                mPKMFontConverter.Add(&HC40, &HD31F)
                mPKMFontConverter.Add(&HC41, &HD320)
                mPKMFontConverter.Add(&HC42, &HD321)
                mPKMFontConverter.Add(&HC43, &HD325)
                mPKMFontConverter.Add(&HC44, &HD328)
                mPKMFontConverter.Add(&HC45, &HD329)
                mPKMFontConverter.Add(&HC46, &HD32C)
                mPKMFontConverter.Add(&HC47, &HD330)
                mPKMFontConverter.Add(&HC48, &HD338)
                mPKMFontConverter.Add(&HC49, &HD339)
                mPKMFontConverter.Add(&HC4A, &HD33B)
                mPKMFontConverter.Add(&HC4B, &HD33C)
                mPKMFontConverter.Add(&HC4C, &HD33D)
                mPKMFontConverter.Add(&HC4D, &HD344)
                mPKMFontConverter.Add(&HC4E, &HD345)
                mPKMFontConverter.Add(&HC4F, &HD37C)
                mPKMFontConverter.Add(&HC50, &HD37D)
                mPKMFontConverter.Add(&HC51, &HD380)
                mPKMFontConverter.Add(&HC52, &HD384)
                mPKMFontConverter.Add(&HC53, &HD38C)
                mPKMFontConverter.Add(&HC54, &HD38D)
                mPKMFontConverter.Add(&HC55, &HD38F)
                mPKMFontConverter.Add(&HC56, &HD390)
                mPKMFontConverter.Add(&HC57, &HD391)
                mPKMFontConverter.Add(&HC58, &HD398)
                mPKMFontConverter.Add(&HC59, &HD399)
                mPKMFontConverter.Add(&HC5A, &HD39C)
                mPKMFontConverter.Add(&HC5B, &HD3A0)
                mPKMFontConverter.Add(&HC5C, &HD3A8)
                mPKMFontConverter.Add(&HC5D, &HD3A9)
                mPKMFontConverter.Add(&HC5E, &HD3AB)
                mPKMFontConverter.Add(&HC5F, &HD3AD)
                mPKMFontConverter.Add(&HC60, &HD3B4)
                mPKMFontConverter.Add(&HC61, &HD3B8)
                mPKMFontConverter.Add(&HC62, &HD3BC)
                mPKMFontConverter.Add(&HC63, &HD3C4)
                mPKMFontConverter.Add(&HC64, &HD3C5)
                mPKMFontConverter.Add(&HC65, &HD3C8)
                mPKMFontConverter.Add(&HC66, &HD3C9)
                mPKMFontConverter.Add(&HC67, &HD3D0)
                mPKMFontConverter.Add(&HC68, &HD3D8)
                mPKMFontConverter.Add(&HC69, &HD3E1)
                mPKMFontConverter.Add(&HC6A, &HD3E3)
                mPKMFontConverter.Add(&HC6B, &HD3EC)
                mPKMFontConverter.Add(&HC6C, &HD3ED)
                mPKMFontConverter.Add(&HC6D, &HD3F0)
                mPKMFontConverter.Add(&HC6E, &HD3F4)
                mPKMFontConverter.Add(&HC6F, &HD3FC)
                mPKMFontConverter.Add(&HC70, &HD3FD)
                mPKMFontConverter.Add(&HC71, &HD3FF)
                mPKMFontConverter.Add(&HC72, &HD401)
                mPKMFontConverter.Add(&HC73, &HD408)
                mPKMFontConverter.Add(&HC74, &HD41D)
                mPKMFontConverter.Add(&HC75, &HD440)
                mPKMFontConverter.Add(&HC76, &HD444)
                mPKMFontConverter.Add(&HC77, &HD45C)
                mPKMFontConverter.Add(&HC78, &HD460)
                mPKMFontConverter.Add(&HC79, &HD464)
                mPKMFontConverter.Add(&HC7A, &HD46D)
                mPKMFontConverter.Add(&HC7B, &HD46F)
                mPKMFontConverter.Add(&HC7C, &HD478)
                mPKMFontConverter.Add(&HC7D, &HD479)
                mPKMFontConverter.Add(&HC7E, &HD47C)
                mPKMFontConverter.Add(&HC7F, &HD47F)
                mPKMFontConverter.Add(&HC80, &HD480)
                mPKMFontConverter.Add(&HC81, &HD482)
                mPKMFontConverter.Add(&HC82, &HD488)
                mPKMFontConverter.Add(&HC83, &HD489)
                mPKMFontConverter.Add(&HC84, &HD48B)
                mPKMFontConverter.Add(&HC85, &HD48D)
                mPKMFontConverter.Add(&HC86, &HD494)
                mPKMFontConverter.Add(&HC87, &HD4A9)
                mPKMFontConverter.Add(&HC88, &HD4CC)
                mPKMFontConverter.Add(&HC89, &HD4D0)
                mPKMFontConverter.Add(&HC8A, &HD4D4)
                mPKMFontConverter.Add(&HC8B, &HD4DC)
                mPKMFontConverter.Add(&HC8C, &HD4DF)
                mPKMFontConverter.Add(&HC8D, &HD4E8)
                mPKMFontConverter.Add(&HC8E, &HD4EC)
                mPKMFontConverter.Add(&HC8F, &HD4F0)
                mPKMFontConverter.Add(&HC90, &HD4F8)
                mPKMFontConverter.Add(&HC91, &HD4FB)
                mPKMFontConverter.Add(&HC92, &HD4FD)
                mPKMFontConverter.Add(&HC93, &HD504)
                mPKMFontConverter.Add(&HC94, &HD508)
                mPKMFontConverter.Add(&HC95, &HD50C)
                mPKMFontConverter.Add(&HC96, &HD514)
                mPKMFontConverter.Add(&HC97, &HD515)
                mPKMFontConverter.Add(&HC98, &HD517)
                mPKMFontConverter.Add(&HC99, &HD53C)
                mPKMFontConverter.Add(&HC9A, &HD53D)
                mPKMFontConverter.Add(&HC9B, &HD540)
                mPKMFontConverter.Add(&HC9C, &HD544)
                mPKMFontConverter.Add(&HC9D, &HD54C)
                mPKMFontConverter.Add(&HC9E, &HD54D)
                mPKMFontConverter.Add(&HC9F, &HD54F)
                mPKMFontConverter.Add(&HCA0, &HD551)
                mPKMFontConverter.Add(&HCA1, &HD558)
                mPKMFontConverter.Add(&HCA2, &HD559)
                mPKMFontConverter.Add(&HCA3, &HD55C)
                mPKMFontConverter.Add(&HCA4, &HD560)
                mPKMFontConverter.Add(&HCA5, &HD565)
                mPKMFontConverter.Add(&HCA6, &HD568)
                mPKMFontConverter.Add(&HCA7, &HD569)
                mPKMFontConverter.Add(&HCA8, &HD56B)
                mPKMFontConverter.Add(&HCA9, &HD56D)
                mPKMFontConverter.Add(&HCAA, &HD574)
                mPKMFontConverter.Add(&HCAB, &HD575)
                mPKMFontConverter.Add(&HCAC, &HD578)
                mPKMFontConverter.Add(&HCAD, &HD57C)
                mPKMFontConverter.Add(&HCAE, &HD584)
                mPKMFontConverter.Add(&HCAF, &HD585)
                mPKMFontConverter.Add(&HCB0, &HD587)
                mPKMFontConverter.Add(&HCB1, &HD588)
                mPKMFontConverter.Add(&HCB2, &HD589)
                mPKMFontConverter.Add(&HCB3, &HD590)
                mPKMFontConverter.Add(&HCB4, &HD5A5)
                mPKMFontConverter.Add(&HCB5, &HD5C8)
                mPKMFontConverter.Add(&HCB6, &HD5C9)
                mPKMFontConverter.Add(&HCB7, &HD5CC)
                mPKMFontConverter.Add(&HCB8, &HD5D0)
                mPKMFontConverter.Add(&HCB9, &HD5D2)
                mPKMFontConverter.Add(&HCBA, &HD5D8)
                mPKMFontConverter.Add(&HCBB, &HD5D9)
                mPKMFontConverter.Add(&HCBC, &HD5DB)
                mPKMFontConverter.Add(&HCBD, &HD5DD)
                mPKMFontConverter.Add(&HCBE, &HD5E4)
                mPKMFontConverter.Add(&HCBF, &HD5E5)
                mPKMFontConverter.Add(&HCC0, &HD5E8)
                mPKMFontConverter.Add(&HCC1, &HD5EC)
                mPKMFontConverter.Add(&HCC2, &HD5F4)
                mPKMFontConverter.Add(&HCC3, &HD5F5)
                mPKMFontConverter.Add(&HCC4, &HD5F7)
                mPKMFontConverter.Add(&HCC5, &HD5F9)
                mPKMFontConverter.Add(&HCC6, &HD600)
                mPKMFontConverter.Add(&HCC7, &HD601)
                mPKMFontConverter.Add(&HCC8, &HD604)
                mPKMFontConverter.Add(&HCC9, &HD608)
                mPKMFontConverter.Add(&HCCA, &HD610)
                mPKMFontConverter.Add(&HCCB, &HD611)
                mPKMFontConverter.Add(&HCCC, &HD613)
                mPKMFontConverter.Add(&HCCD, &HD614)
                mPKMFontConverter.Add(&HCCE, &HD615)
                mPKMFontConverter.Add(&HCCF, &HD61C)
                mPKMFontConverter.Add(&HCD0, &HD620)
                mPKMFontConverter.Add(&HCD1, &HD624)
                mPKMFontConverter.Add(&HCD2, &HD62D)
                mPKMFontConverter.Add(&HCD3, &HD638)
                mPKMFontConverter.Add(&HCD4, &HD639)
                mPKMFontConverter.Add(&HCD5, &HD63C)
                mPKMFontConverter.Add(&HCD6, &HD640)
                mPKMFontConverter.Add(&HCD7, &HD645)
                mPKMFontConverter.Add(&HCD8, &HD648)
                mPKMFontConverter.Add(&HCD9, &HD649)
                mPKMFontConverter.Add(&HCDA, &HD64B)
                mPKMFontConverter.Add(&HCDB, &HD64D)
                mPKMFontConverter.Add(&HCDC, &HD651)
                mPKMFontConverter.Add(&HCDD, &HD654)
                mPKMFontConverter.Add(&HCDE, &HD655)
                mPKMFontConverter.Add(&HCDF, &HD658)
                mPKMFontConverter.Add(&HCE0, &HD65C)
                mPKMFontConverter.Add(&HCE1, &HD667)
                mPKMFontConverter.Add(&HCE2, &HD669)
                mPKMFontConverter.Add(&HCE3, &HD670)
                mPKMFontConverter.Add(&HCE4, &HD671)
                mPKMFontConverter.Add(&HCE5, &HD674)
                mPKMFontConverter.Add(&HCE6, &HD683)
                mPKMFontConverter.Add(&HCE7, &HD685)
                mPKMFontConverter.Add(&HCE8, &HD68C)
                mPKMFontConverter.Add(&HCE9, &HD68D)
                mPKMFontConverter.Add(&HCEA, &HD690)
                mPKMFontConverter.Add(&HCEB, &HD694)
                mPKMFontConverter.Add(&HCEC, &HD69D)
                mPKMFontConverter.Add(&HCED, &HD69F)
                mPKMFontConverter.Add(&HCEE, &HD6A1)
                mPKMFontConverter.Add(&HCEF, &HD6A8)
                mPKMFontConverter.Add(&HCF0, &HD6AC)
                mPKMFontConverter.Add(&HCF1, &HD6B0)
                mPKMFontConverter.Add(&HCF2, &HD6B9)
                mPKMFontConverter.Add(&HCF3, &HD6BB)
                mPKMFontConverter.Add(&HCF4, &HD6C4)
                mPKMFontConverter.Add(&HCF5, &HD6C5)
                mPKMFontConverter.Add(&HCF6, &HD6C8)
                mPKMFontConverter.Add(&HCF7, &HD6CC)
                mPKMFontConverter.Add(&HCF8, &HD6D1)
                mPKMFontConverter.Add(&HCF9, &HD6D4)
                mPKMFontConverter.Add(&HCFA, &HD6D7)
                mPKMFontConverter.Add(&HCFB, &HD6D9)
                mPKMFontConverter.Add(&HCFC, &HD6E0)
                mPKMFontConverter.Add(&HCFD, &HD6E4)
                mPKMFontConverter.Add(&HCFE, &HD6E8)
                mPKMFontConverter.Add(&HCFF, &HD6F0)
                mPKMFontConverter.Add(&HD00, &HD6F5)
                mPKMFontConverter.Add(&HD01, &HD6FC)
                mPKMFontConverter.Add(&HD02, &HD6FD)
                mPKMFontConverter.Add(&HD03, &HD700)
                mPKMFontConverter.Add(&HD04, &HD704)
                mPKMFontConverter.Add(&HD05, &HD711)
                mPKMFontConverter.Add(&HD06, &HD718)
                mPKMFontConverter.Add(&HD07, &HD719)
                mPKMFontConverter.Add(&HD08, &HD71C)
                mPKMFontConverter.Add(&HD09, &HD720)
                mPKMFontConverter.Add(&HD0A, &HD728)
                mPKMFontConverter.Add(&HD0B, &HD729)
                mPKMFontConverter.Add(&HD0C, &HD72B)
                mPKMFontConverter.Add(&HD0D, &HD72D)
                mPKMFontConverter.Add(&HD0E, &HD734)
                mPKMFontConverter.Add(&HD0F, &HD735)
                mPKMFontConverter.Add(&HD10, &HD738)
                mPKMFontConverter.Add(&HD11, &HD73C)
                mPKMFontConverter.Add(&HD12, &HD744)
                mPKMFontConverter.Add(&HD13, &HD747)
                mPKMFontConverter.Add(&HD14, &HD749)
                mPKMFontConverter.Add(&HD15, &HD750)
                mPKMFontConverter.Add(&HD16, &HD751)
                mPKMFontConverter.Add(&HD17, &HD754)
                mPKMFontConverter.Add(&HD18, &HD756)
                mPKMFontConverter.Add(&HD19, &HD757)
                mPKMFontConverter.Add(&HD1A, &HD758)
                mPKMFontConverter.Add(&HD1B, &HD759)
                mPKMFontConverter.Add(&HD1C, &HD760)
                mPKMFontConverter.Add(&HD1D, &HD761)
                mPKMFontConverter.Add(&HD1E, &HD763)
                mPKMFontConverter.Add(&HD1F, &HD765)
                mPKMFontConverter.Add(&HD20, &HD769)
                mPKMFontConverter.Add(&HD21, &HD76C)
                mPKMFontConverter.Add(&HD22, &HD770)
                mPKMFontConverter.Add(&HD23, &HD774)
                mPKMFontConverter.Add(&HD24, &HD77C)
                mPKMFontConverter.Add(&HD25, &HD77D)
                mPKMFontConverter.Add(&HD26, &HD781)
                mPKMFontConverter.Add(&HD27, &HD788)
                mPKMFontConverter.Add(&HD28, &HD789)
                mPKMFontConverter.Add(&HD29, &HD78C)
                mPKMFontConverter.Add(&HD2A, &HD790)
                mPKMFontConverter.Add(&HD2B, &HD798)
                mPKMFontConverter.Add(&HD2C, &HD799)
                mPKMFontConverter.Add(&HD2D, &HD79B)
                mPKMFontConverter.Add(&HD2E, &HD79D)
                mPKMFontConverter.Add(&HD31, &H1100)
                mPKMFontConverter.Add(&HD32, &H1101)
                mPKMFontConverter.Add(&HD33, &H1102)
                mPKMFontConverter.Add(&HD34, &H1103)
                mPKMFontConverter.Add(&HD35, &H1104)
                mPKMFontConverter.Add(&HD36, &H1105)
                mPKMFontConverter.Add(&HD37, &H1106)
                mPKMFontConverter.Add(&HD38, &H1107)
                mPKMFontConverter.Add(&HD39, &H1108)
                mPKMFontConverter.Add(&HD3A, &H1109)
                mPKMFontConverter.Add(&HD3B, &H110A)
                mPKMFontConverter.Add(&HD3C, &H110B)
                mPKMFontConverter.Add(&HD3D, &H110C)
                mPKMFontConverter.Add(&HD3E, &H110D)
                mPKMFontConverter.Add(&HD3F, &H110E)
                mPKMFontConverter.Add(&HD40, &H110F)
                mPKMFontConverter.Add(&HD41, &H1110)
                mPKMFontConverter.Add(&HD42, &H1111)
                mPKMFontConverter.Add(&HD43, &H1112)
                mPKMFontConverter.Add(&HD44, &H1161)
                mPKMFontConverter.Add(&HD45, &H1162)
                mPKMFontConverter.Add(&HD46, &H1163)
                mPKMFontConverter.Add(&HD47, &H1164)
                mPKMFontConverter.Add(&HD48, &H1165)
                mPKMFontConverter.Add(&HD49, &H1166)
                mPKMFontConverter.Add(&HD4A, &H1167)
                mPKMFontConverter.Add(&HD4B, &H1168)
                mPKMFontConverter.Add(&HD4C, &H1169)
                mPKMFontConverter.Add(&HD4D, &H116D)
                mPKMFontConverter.Add(&HD4E, &H116E)
                mPKMFontConverter.Add(&HD4F, &H1172)
                mPKMFontConverter.Add(&HD50, &H1173)
                mPKMFontConverter.Add(&HD51, &H1175)
                mPKMFontConverter.Add(&HD61, &HB894)
                mPKMFontConverter.Add(&HD62, &HC330)
                mPKMFontConverter.Add(&HD63, &HC3BC)
                mPKMFontConverter.Add(&HD64, &HC4D4)
                mPKMFontConverter.Add(&HD65, &HCB2C)

                mRevPKMFont.Add(&H3000, &H1)
                mRevPKMFont.Add(&H3041, &H2)
                mRevPKMFont.Add(&H3042, &H3)
                mRevPKMFont.Add(&H3043, &H4)
                mRevPKMFont.Add(&H3044, &H5)
                mRevPKMFont.Add(&H3045, &H6)
                mRevPKMFont.Add(&H3046, &H7)
                mRevPKMFont.Add(&H3047, &H8)
                mRevPKMFont.Add(&H3048, &H9)
                mRevPKMFont.Add(&H3049, &HA)
                mRevPKMFont.Add(&H304A, &HB)
                mRevPKMFont.Add(&H304B, &HC)
                mRevPKMFont.Add(&H304C, &HD)
                mRevPKMFont.Add(&H304D, &HE)
                mRevPKMFont.Add(&H304E, &HF)
                mRevPKMFont.Add(&H304F, &H10)
                mRevPKMFont.Add(&H3050, &H11)
                mRevPKMFont.Add(&H3051, &H12)
                mRevPKMFont.Add(&H3052, &H13)
                mRevPKMFont.Add(&H3053, &H14)
                mRevPKMFont.Add(&H3054, &H15)
                mRevPKMFont.Add(&H3055, &H16)
                mRevPKMFont.Add(&H3056, &H17)
                mRevPKMFont.Add(&H3057, &H18)
                mRevPKMFont.Add(&H3058, &H19)
                mRevPKMFont.Add(&H3059, &H1A)
                mRevPKMFont.Add(&H305A, &H1B)
                mRevPKMFont.Add(&H305B, &H1C)
                mRevPKMFont.Add(&H305C, &H1D)
                mRevPKMFont.Add(&H305D, &H1E)
                mRevPKMFont.Add(&H305E, &H1F)
                mRevPKMFont.Add(&H305F, &H20)
                mRevPKMFont.Add(&H3060, &H21)
                mRevPKMFont.Add(&H3061, &H22)
                mRevPKMFont.Add(&H3062, &H23)
                mRevPKMFont.Add(&H3063, &H24)
                mRevPKMFont.Add(&H3064, &H25)
                mRevPKMFont.Add(&H3065, &H26)
                mRevPKMFont.Add(&H3066, &H27)
                mRevPKMFont.Add(&H3067, &H28)
                mRevPKMFont.Add(&H3068, &H29)
                mRevPKMFont.Add(&H3069, &H2A)
                mRevPKMFont.Add(&H306A, &H2B)
                mRevPKMFont.Add(&H306B, &H2C)
                mRevPKMFont.Add(&H306C, &H2D)
                mRevPKMFont.Add(&H306D, &H2E)
                mRevPKMFont.Add(&H306E, &H2F)
                mRevPKMFont.Add(&H306F, &H30)
                mRevPKMFont.Add(&H3070, &H31)
                mRevPKMFont.Add(&H3071, &H32)
                mRevPKMFont.Add(&H3072, &H33)
                mRevPKMFont.Add(&H3073, &H34)
                mRevPKMFont.Add(&H3074, &H35)
                mRevPKMFont.Add(&H3075, &H36)
                mRevPKMFont.Add(&H3076, &H37)
                mRevPKMFont.Add(&H3077, &H38)
                mRevPKMFont.Add(&H3078, &H39)
                mRevPKMFont.Add(&H3079, &H3A)
                mRevPKMFont.Add(&H307A, &H3B)
                mRevPKMFont.Add(&H307B, &H3C)
                mRevPKMFont.Add(&H307C, &H3D)
                mRevPKMFont.Add(&H307D, &H3E)
                mRevPKMFont.Add(&H307E, &H3F)
                mRevPKMFont.Add(&H307F, &H40)
                mRevPKMFont.Add(&H3080, &H41)
                mRevPKMFont.Add(&H3081, &H42)
                mRevPKMFont.Add(&H3082, &H43)
                mRevPKMFont.Add(&H3083, &H44)
                mRevPKMFont.Add(&H3084, &H45)
                mRevPKMFont.Add(&H3085, &H46)
                mRevPKMFont.Add(&H3086, &H47)
                mRevPKMFont.Add(&H3087, &H48)
                mRevPKMFont.Add(&H3088, &H49)
                mRevPKMFont.Add(&H3089, &H4A)
                mRevPKMFont.Add(&H308A, &H4B)
                mRevPKMFont.Add(&H308B, &H4C)
                mRevPKMFont.Add(&H308C, &H4D)
                mRevPKMFont.Add(&H308D, &H4E)
                mRevPKMFont.Add(&H308F, &H4F)
                mRevPKMFont.Add(&H3092, &H50)
                mRevPKMFont.Add(&H3093, &H51)
                mRevPKMFont.Add(&H30A1, &H52)
                mRevPKMFont.Add(&H30A2, &H53)
                mRevPKMFont.Add(&H30A3, &H54)
                mRevPKMFont.Add(&H30A4, &H55)
                mRevPKMFont.Add(&H30A5, &H56)
                mRevPKMFont.Add(&H30A6, &H57)
                mRevPKMFont.Add(&H30A7, &H58)
                mRevPKMFont.Add(&H30A8, &H59)
                mRevPKMFont.Add(&H30A9, &H5A)
                mRevPKMFont.Add(&H30AA, &H5B)
                mRevPKMFont.Add(&H30AB, &H5C)
                mRevPKMFont.Add(&H30AC, &H5D)
                mRevPKMFont.Add(&H30AD, &H5E)
                mRevPKMFont.Add(&H30AE, &H5F)
                mRevPKMFont.Add(&H30AF, &H60)
                mRevPKMFont.Add(&H30B0, &H61)
                mRevPKMFont.Add(&H30B1, &H62)
                mRevPKMFont.Add(&H30B2, &H63)
                mRevPKMFont.Add(&H30B3, &H64)
                mRevPKMFont.Add(&H30B4, &H65)
                mRevPKMFont.Add(&H30B5, &H66)
                mRevPKMFont.Add(&H30B6, &H67)
                mRevPKMFont.Add(&H30B7, &H68)
                mRevPKMFont.Add(&H30B8, &H69)
                mRevPKMFont.Add(&H30B9, &H6A)
                mRevPKMFont.Add(&H30BA, &H6B)
                mRevPKMFont.Add(&H30BB, &H6C)
                mRevPKMFont.Add(&H30BC, &H6D)
                mRevPKMFont.Add(&H30BD, &H6E)
                mRevPKMFont.Add(&H30BE, &H6F)
                mRevPKMFont.Add(&H30BF, &H70)
                mRevPKMFont.Add(&H30C0, &H71)
                mRevPKMFont.Add(&H30C1, &H72)
                mRevPKMFont.Add(&H30C2, &H73)
                mRevPKMFont.Add(&H30C3, &H74)
                mRevPKMFont.Add(&H30C4, &H75)
                mRevPKMFont.Add(&H30C5, &H76)
                mRevPKMFont.Add(&H30C6, &H77)
                mRevPKMFont.Add(&H30C7, &H78)
                mRevPKMFont.Add(&H30C8, &H79)
                mRevPKMFont.Add(&H30C9, &H7A)
                mRevPKMFont.Add(&H30CA, &H7B)
                mRevPKMFont.Add(&H30CB, &H7C)
                mRevPKMFont.Add(&H30CC, &H7D)
                mRevPKMFont.Add(&H30CD, &H7E)
                mRevPKMFont.Add(&H30CE, &H7F)
                mRevPKMFont.Add(&H30CF, &H80)
                mRevPKMFont.Add(&H30D0, &H81)
                mRevPKMFont.Add(&H30D1, &H82)
                mRevPKMFont.Add(&H30D2, &H83)
                mRevPKMFont.Add(&H30D3, &H84)
                mRevPKMFont.Add(&H30D4, &H85)
                mRevPKMFont.Add(&H30D5, &H86)
                mRevPKMFont.Add(&H30D6, &H87)
                mRevPKMFont.Add(&H30D7, &H88)
                mRevPKMFont.Add(&H30D8, &H89)
                mRevPKMFont.Add(&H30D9, &H8A)
                mRevPKMFont.Add(&H30DA, &H8B)
                mRevPKMFont.Add(&H30DB, &H8C)
                mRevPKMFont.Add(&H30DC, &H8D)
                mRevPKMFont.Add(&H30DD, &H8E)
                mRevPKMFont.Add(&H30DE, &H8F)
                mRevPKMFont.Add(&H30DF, &H90)
                mRevPKMFont.Add(&H30E0, &H91)
                mRevPKMFont.Add(&H30E1, &H92)
                mRevPKMFont.Add(&H30E2, &H93)
                mRevPKMFont.Add(&H30E3, &H94)
                mRevPKMFont.Add(&H30E4, &H95)
                mRevPKMFont.Add(&H30E5, &H96)
                mRevPKMFont.Add(&H30E6, &H97)
                mRevPKMFont.Add(&H30E7, &H98)
                mRevPKMFont.Add(&H30E8, &H99)
                mRevPKMFont.Add(&H30E9, &H9A)
                mRevPKMFont.Add(&H30EA, &H9B)
                mRevPKMFont.Add(&H30EB, &H9C)
                mRevPKMFont.Add(&H30EC, &H9D)
                mRevPKMFont.Add(&H30ED, &H9E)
                mRevPKMFont.Add(&H30EF, &H9F)
                mRevPKMFont.Add(&H30F2, &HA0)
                mRevPKMFont.Add(&H30F3, &HA1)
                mRevPKMFont.Add(&HFF10, &HA2)
                mRevPKMFont.Add(&HFF11, &HA3)
                mRevPKMFont.Add(&HFF12, &HA4)
                mRevPKMFont.Add(&HFF13, &HA5)
                mRevPKMFont.Add(&HFF14, &HA6)
                mRevPKMFont.Add(&HFF15, &HA7)
                mRevPKMFont.Add(&HFF16, &HA8)
                mRevPKMFont.Add(&HFF17, &HA9)
                mRevPKMFont.Add(&HFF18, &HAA)
                mRevPKMFont.Add(&HFF19, &HAB)
                mRevPKMFont.Add(&HFF21, &HAC)
                mRevPKMFont.Add(&HFF22, &HAD)
                mRevPKMFont.Add(&HFF23, &HAE)
                mRevPKMFont.Add(&HFF24, &HAF)
                mRevPKMFont.Add(&HFF25, &HB0)
                mRevPKMFont.Add(&HFF26, &HB1)
                mRevPKMFont.Add(&HFF27, &HB2)
                mRevPKMFont.Add(&HFF28, &HB3)
                mRevPKMFont.Add(&HFF29, &HB4)
                mRevPKMFont.Add(&HFF2A, &HB5)
                mRevPKMFont.Add(&HFF2B, &HB6)
                mRevPKMFont.Add(&HFF2C, &HB7)
                mRevPKMFont.Add(&HFF2D, &HB8)
                mRevPKMFont.Add(&HFF2E, &HB9)
                mRevPKMFont.Add(&HFF2F, &HBA)
                mRevPKMFont.Add(&HFF30, &HBB)
                mRevPKMFont.Add(&HFF31, &HBC)
                mRevPKMFont.Add(&HFF32, &HBD)
                mRevPKMFont.Add(&HFF33, &HBE)
                mRevPKMFont.Add(&HFF34, &HBF)
                mRevPKMFont.Add(&HFF35, &HC0)
                mRevPKMFont.Add(&HFF36, &HC1)
                mRevPKMFont.Add(&HFF37, &HC2)
                mRevPKMFont.Add(&HFF38, &HC3)
                mRevPKMFont.Add(&HFF39, &HC4)
                mRevPKMFont.Add(&HFF3A, &HC5)
                mRevPKMFont.Add(&HFF41, &HC6)
                mRevPKMFont.Add(&HFF42, &HC7)
                mRevPKMFont.Add(&HFF43, &HC8)
                mRevPKMFont.Add(&HFF44, &HC9)
                mRevPKMFont.Add(&HFF45, &HCA)
                mRevPKMFont.Add(&HFF46, &HCB)
                mRevPKMFont.Add(&HFF47, &HCC)
                mRevPKMFont.Add(&HFF48, &HCD)
                mRevPKMFont.Add(&HFF49, &HCE)
                mRevPKMFont.Add(&HFF4A, &HCF)
                mRevPKMFont.Add(&HFF4B, &HD0)
                mRevPKMFont.Add(&HFF4C, &HD1)
                mRevPKMFont.Add(&HFF4D, &HD2)
                mRevPKMFont.Add(&HFF4E, &HD3)
                mRevPKMFont.Add(&HFF4F, &HD4)
                mRevPKMFont.Add(&HFF50, &HD5)
                mRevPKMFont.Add(&HFF51, &HD6)
                mRevPKMFont.Add(&HFF52, &HD7)
                mRevPKMFont.Add(&HFF53, &HD8)
                mRevPKMFont.Add(&HFF54, &HD9)
                mRevPKMFont.Add(&HFF55, &HDA)
                mRevPKMFont.Add(&HFF56, &HDB)
                mRevPKMFont.Add(&HFF57, &HDC)
                mRevPKMFont.Add(&HFF58, &HDD)
                mRevPKMFont.Add(&HFF59, &HDE)
                mRevPKMFont.Add(&HFF5A, &HDF)
                mRevPKMFont.Add(&HFF01, &HE1)
                mRevPKMFont.Add(&HFF1F, &HE2)
                mRevPKMFont.Add(&H3001, &HE3)
                mRevPKMFont.Add(&H3002, &HE4)
                mRevPKMFont.Add(&H22EF, &HE5)
                mRevPKMFont.Add(&H30FB, &HE6)
                mRevPKMFont.Add(&HFF0F, &HE7)
                mRevPKMFont.Add(&H300C, &HE8)
                mRevPKMFont.Add(&H300D, &HE9)
                mRevPKMFont.Add(&H300E, &HEA)
                mRevPKMFont.Add(&H300F, &HEB)
                mRevPKMFont.Add(&HFF08, &HEC)
                mRevPKMFont.Add(&HFF09, &HED)
                mRevPKMFont.Add(&H329A, &HEE)
                mRevPKMFont.Add(&H329B, &HEF)
                mRevPKMFont.Add(&HFF0B, &HF0)
                mRevPKMFont.Add(&HFF0D, &HF1)
                mRevPKMFont.Add(&H2297, &HF2)
                mRevPKMFont.Add(&H2298, &HF3)
                mRevPKMFont.Add(&HFF1D, &HF4)
                'mRevPKMFont.Add(&HFF5A, &HF5)
                mRevPKMFont.Add(&HFF1A, &HF6)
                mRevPKMFont.Add(&HFF1B, &HF7)
                mRevPKMFont.Add(&HFF0E, &HF8)
                mRevPKMFont.Add(&HFF0C, &HF9)
                mRevPKMFont.Add(&H2664, &HFA)
                mRevPKMFont.Add(&H2667, &HFB)
                mRevPKMFont.Add(&H2661, &HFC)
                mRevPKMFont.Add(&H2662, &HFD)
                mRevPKMFont.Add(&H2606, &HFE)
                mRevPKMFont.Add(&H25CE, &HFF)
                mRevPKMFont.Add(&H25CB, &H100)
                mRevPKMFont.Add(&H25A1, &H101)
                mRevPKMFont.Add(&H25B3, &H102)
                mRevPKMFont.Add(&H25C7, &H103)
                mRevPKMFont.Add(&HFF20, &H104)
                mRevPKMFont.Add(&H266B, &H105)
                mRevPKMFont.Add(&HFF05, &H106)
                mRevPKMFont.Add(&H263C, &H107)
                mRevPKMFont.Add(&H2614, &H108)
                mRevPKMFont.Add(&H2630, &H109)
                mRevPKMFont.Add(&H2744, &H10A)
                mRevPKMFont.Add(&H260B, &H10B)
                mRevPKMFont.Add(&H2654, &H10C)
                mRevPKMFont.Add(&H2655, &H10D)
                mRevPKMFont.Add(&H260A, &H10E)
                mRevPKMFont.Add(&H21D7, &H10F)
                mRevPKMFont.Add(&H21D8, &H110)
                mRevPKMFont.Add(&H263E, &H111)
                mRevPKMFont.Add(&HA5, &H112)
                mRevPKMFont.Add(&H2648, &H113)
                mRevPKMFont.Add(&H2649, &H114)
                mRevPKMFont.Add(&H264A, &H115)
                mRevPKMFont.Add(&H264B, &H116)
                mRevPKMFont.Add(&H264C, &H117)
                mRevPKMFont.Add(&H264D, &H118)
                mRevPKMFont.Add(&H264E, &H119)
                mRevPKMFont.Add(&H264F, &H11A)
                mRevPKMFont.Add(&H2190, &H11B)
                mRevPKMFont.Add(&H2191, &H11C)
                mRevPKMFont.Add(&H2193, &H11D)
                mRevPKMFont.Add(&H2192, &H11E)
                mRevPKMFont.Add(&H2023, &H11F)
                mRevPKMFont.Add(&HFF06, &H120)
                mRevPKMFont.Add(&H30, &H121)
                mRevPKMFont.Add(&H31, &H122)
                mRevPKMFont.Add(&H32, &H123)
                mRevPKMFont.Add(&H33, &H124)
                mRevPKMFont.Add(&H34, &H125)
                mRevPKMFont.Add(&H35, &H126)
                mRevPKMFont.Add(&H36, &H127)
                mRevPKMFont.Add(&H37, &H128)
                mRevPKMFont.Add(&H38, &H129)
                mRevPKMFont.Add(&H39, &H12A)
                mRevPKMFont.Add(&H41, &H12B)
                mRevPKMFont.Add(&H42, &H12C)
                mRevPKMFont.Add(&H43, &H12D)
                mRevPKMFont.Add(&H44, &H12E)
                mRevPKMFont.Add(&H45, &H12F)
                mRevPKMFont.Add(&H46, &H130)
                mRevPKMFont.Add(&H47, &H131)
                mRevPKMFont.Add(&H48, &H132)
                mRevPKMFont.Add(&H49, &H133)
                mRevPKMFont.Add(&H4A, &H134)
                mRevPKMFont.Add(&H4B, &H135)
                mRevPKMFont.Add(&H4C, &H136)
                mRevPKMFont.Add(&H4D, &H137)
                mRevPKMFont.Add(&H4E, &H138)
                mRevPKMFont.Add(&H4F, &H139)
                mRevPKMFont.Add(&H50, &H13A)
                mRevPKMFont.Add(&H51, &H13B)
                mRevPKMFont.Add(&H52, &H13C)
                mRevPKMFont.Add(&H53, &H13D)
                mRevPKMFont.Add(&H54, &H13E)
                mRevPKMFont.Add(&H55, &H13F)
                mRevPKMFont.Add(&H56, &H140)
                mRevPKMFont.Add(&H57, &H141)
                mRevPKMFont.Add(&H58, &H142)
                mRevPKMFont.Add(&H59, &H143)
                mRevPKMFont.Add(&H5A, &H144)
                mRevPKMFont.Add(&H61, &H145)
                mRevPKMFont.Add(&H62, &H146)
                mRevPKMFont.Add(&H63, &H147)
                mRevPKMFont.Add(&H64, &H148)
                mRevPKMFont.Add(&H65, &H149)
                mRevPKMFont.Add(&H66, &H14A)
                mRevPKMFont.Add(&H67, &H14B)
                mRevPKMFont.Add(&H68, &H14C)
                mRevPKMFont.Add(&H69, &H14D)
                mRevPKMFont.Add(&H6A, &H14E)
                mRevPKMFont.Add(&H6B, &H14F)
                mRevPKMFont.Add(&H6C, &H150)
                mRevPKMFont.Add(&H6D, &H151)
                mRevPKMFont.Add(&H6E, &H152)
                mRevPKMFont.Add(&H6F, &H153)
                mRevPKMFont.Add(&H70, &H154)
                mRevPKMFont.Add(&H71, &H155)
                mRevPKMFont.Add(&H72, &H156)
                mRevPKMFont.Add(&H73, &H157)
                mRevPKMFont.Add(&H74, &H158)
                mRevPKMFont.Add(&H75, &H159)
                mRevPKMFont.Add(&H76, &H15A)
                mRevPKMFont.Add(&H77, &H15B)
                mRevPKMFont.Add(&H78, &H15C)
                mRevPKMFont.Add(&H79, &H15D)
                mRevPKMFont.Add(&H7A, &H15E)
                mRevPKMFont.Add(&HC0, &H15F)
                mRevPKMFont.Add(&HC1, &H160)
                mRevPKMFont.Add(&HC2, &H161)
                mRevPKMFont.Add(&HC3, &H162)
                mRevPKMFont.Add(&HC4, &H163)
                mRevPKMFont.Add(&HC5, &H164)
                mRevPKMFont.Add(&HC6, &H165)
                mRevPKMFont.Add(&HC7, &H166)
                mRevPKMFont.Add(&HC8, &H167)
                mRevPKMFont.Add(&HC9, &H168)
                mRevPKMFont.Add(&HCA, &H169)
                mRevPKMFont.Add(&HCB, &H16A)
                mRevPKMFont.Add(&HCC, &H16B)
                mRevPKMFont.Add(&HCD, &H16C)
                mRevPKMFont.Add(&HCE, &H16D)
                mRevPKMFont.Add(&HCF, &H16E)
                mRevPKMFont.Add(&HD0, &H16F)
                mRevPKMFont.Add(&HD1, &H170)
                mRevPKMFont.Add(&HD2, &H171)
                mRevPKMFont.Add(&HD3, &H172)
                mRevPKMFont.Add(&HD4, &H173)
                mRevPKMFont.Add(&HD5, &H174)
                mRevPKMFont.Add(&HD6, &H175)
                mRevPKMFont.Add(&HD7, &H176)
                mRevPKMFont.Add(&HD8, &H177)
                mRevPKMFont.Add(&HD9, &H178)
                mRevPKMFont.Add(&HDA, &H179)
                mRevPKMFont.Add(&HDB, &H17A)
                mRevPKMFont.Add(&HDC, &H17B)
                mRevPKMFont.Add(&HDD, &H17C)
                mRevPKMFont.Add(&HDE, &H17D)
                mRevPKMFont.Add(&HDF, &H17E)
                mRevPKMFont.Add(&HE0, &H17F)
                mRevPKMFont.Add(&HE1, &H180)
                mRevPKMFont.Add(&HE2, &H181)
                mRevPKMFont.Add(&HE3, &H182)
                mRevPKMFont.Add(&HE4, &H183)
                mRevPKMFont.Add(&HE5, &H184)
                mRevPKMFont.Add(&HE6, &H185)
                mRevPKMFont.Add(&HE7, &H186)
                mRevPKMFont.Add(&HE8, &H187)
                mRevPKMFont.Add(&HE9, &H188)
                mRevPKMFont.Add(&HEA, &H189)
                mRevPKMFont.Add(&HEB, &H18A)
                mRevPKMFont.Add(&HEC, &H18B)
                mRevPKMFont.Add(&HED, &H18C)
                mRevPKMFont.Add(&HEE, &H18D)
                mRevPKMFont.Add(&HEF, &H18E)
                mRevPKMFont.Add(&HF0, &H18F)
                mRevPKMFont.Add(&HF1, &H190)
                mRevPKMFont.Add(&HF2, &H191)
                mRevPKMFont.Add(&HF3, &H192)
                mRevPKMFont.Add(&HF4, &H193)
                mRevPKMFont.Add(&HF5, &H194)
                mRevPKMFont.Add(&HF6, &H195)
                mRevPKMFont.Add(&HF7, &H196)
                mRevPKMFont.Add(&HF8, &H197)
                mRevPKMFont.Add(&HF9, &H198)
                mRevPKMFont.Add(&HFA, &H199)
                mRevPKMFont.Add(&HFB, &H19A)
                mRevPKMFont.Add(&HFC, &H19B)
                mRevPKMFont.Add(&HFD, &H19C)
                mRevPKMFont.Add(&HFE, &H19D)
                mRevPKMFont.Add(&HFF, &H19E)
                mRevPKMFont.Add(&H152, &H19F)
                mRevPKMFont.Add(&H153, &H1A0)
                mRevPKMFont.Add(&H15E, &H1A1)
                mRevPKMFont.Add(&H15F, &H1A2)
                mRevPKMFont.Add(&HAA, &H1A3)
                mRevPKMFont.Add(&HBA, &H1A4)
                mRevPKMFont.Add(&HB9, &H1A5)
                mRevPKMFont.Add(&HB2, &H1A6)
                mRevPKMFont.Add(&HB3, &H1A7)
                mRevPKMFont.Add(&H24, &H1A8)
                mRevPKMFont.Add(&HA1, &H1A9)
                mRevPKMFont.Add(&HBF, &H1AA)
                mRevPKMFont.Add(&H21, &H1AB)
                mRevPKMFont.Add(&H3F, &H1AC)
                mRevPKMFont.Add(&H2C, &H1AD)
                mRevPKMFont.Add(&H2E, &H1AE)
                mRevPKMFont.Add(&H2026, &H1AF)
                mRevPKMFont.Add(&HB7, &H1B0)
                mRevPKMFont.Add(&H2F, &H1B1)
                mRevPKMFont.Add(&H2018, &H1B2)
                mRevPKMFont.Add(&H2019, &H1B3)
                mRevPKMFont.Add(&H201C, &H1B4)
                mRevPKMFont.Add(&H201D, &H1B5)
                mRevPKMFont.Add(&H201E, &H1B6)
                mRevPKMFont.Add(&H300A, &H1B7)
                mRevPKMFont.Add(&H300B, &H1B8)
                mRevPKMFont.Add(&H28, &H1B9)
                mRevPKMFont.Add(&H29, &H1BA)
                mRevPKMFont.Add(&H2642, &H1BB)
                mRevPKMFont.Add(&H2640, &H1BC)
                mRevPKMFont.Add(&H2B, &H1BD)
                mRevPKMFont.Add(&H2D, &H1BE)
                mRevPKMFont.Add(&H2A, &H1BF)
                mRevPKMFont.Add(&H23, &H1C0)
                mRevPKMFont.Add(&H3D, &H1C1)
                mRevPKMFont.Add(&H26, &H1C2)
                mRevPKMFont.Add(&H7E, &H1C3)
                mRevPKMFont.Add(&H3A, &H1C4)
                mRevPKMFont.Add(&H3B, &H1C5)
                mRevPKMFont.Add(&H2660, &H1C6)
                mRevPKMFont.Add(&H2663, &H1C7)
                mRevPKMFont.Add(&H2665, &H1C8)
                mRevPKMFont.Add(&H2666, &H1C9)
                mRevPKMFont.Add(&H2605, &H1CA)
                mRevPKMFont.Add(&H25C9, &H1CB)
                mRevPKMFont.Add(&H25CF, &H1CC)
                mRevPKMFont.Add(&H25A0, &H1CD)
                mRevPKMFont.Add(&H25B2, &H1CE)
                mRevPKMFont.Add(&H25C6, &H1CF)
                mRevPKMFont.Add(&H40, &H1D0)
                mRevPKMFont.Add(&H266A, &H1D1)
                mRevPKMFont.Add(&H25, &H1D2)
                mRevPKMFont.Add(&H2600, &H1D3)
                mRevPKMFont.Add(&H2601, &H1D4)
                mRevPKMFont.Add(&H2602, &H1D5)
                mRevPKMFont.Add(&H2603, &H1D6)
                mRevPKMFont.Add(&H263A, &H1D7)
                mRevPKMFont.Add(&H265A, &H1D8)
                mRevPKMFont.Add(&H265B, &H1D9)
                mRevPKMFont.Add(&H2639, &H1DA)
                mRevPKMFont.Add(&H2197, &H1DB)
                mRevPKMFont.Add(&H2198, &H1DC)
                mRevPKMFont.Add(&H263D, &H1DD)
                mRevPKMFont.Add(&H20, &H1DE)
                mRevPKMFont.Add(&H2074, &H1DF)
                mRevPKMFont.Add(&H20A7, &H1E0)
                mRevPKMFont.Add(&H20A6, &H1E1)
                mRevPKMFont.Add(&HB0, &H1E8)
                mRevPKMFont.Add(&H5F, &H1E9)
                mRevPKMFont.Add(&HFF3F, &H1EA)
                mRevPKMFont.Add(&HAC00, &H400)
                mRevPKMFont.Add(&HAC01, &H401)
                mRevPKMFont.Add(&HAC04, &H402)
                mRevPKMFont.Add(&HAC07, &H403)
                mRevPKMFont.Add(&HAC08, &H404)
                mRevPKMFont.Add(&HAC09, &H405)
                mRevPKMFont.Add(&HAC0A, &H406)
                mRevPKMFont.Add(&HAC10, &H407)
                mRevPKMFont.Add(&HAC11, &H408)
                mRevPKMFont.Add(&HAC12, &H409)
                mRevPKMFont.Add(&HAC13, &H40A)
                mRevPKMFont.Add(&HAC14, &H40B)
                mRevPKMFont.Add(&HAC15, &H40C)
                mRevPKMFont.Add(&HAC16, &H40D)
                mRevPKMFont.Add(&HAC17, &H40E)
                mRevPKMFont.Add(&HAC19, &H410)
                mRevPKMFont.Add(&HAC1A, &H411)
                mRevPKMFont.Add(&HAC1B, &H412)
                mRevPKMFont.Add(&HAC1C, &H413)
                mRevPKMFont.Add(&HAC1D, &H414)
                mRevPKMFont.Add(&HAC20, &H415)
                mRevPKMFont.Add(&HAC24, &H416)
                mRevPKMFont.Add(&HAC2C, &H417)
                mRevPKMFont.Add(&HAC2D, &H418)
                mRevPKMFont.Add(&HAC2F, &H419)
                mRevPKMFont.Add(&HAC30, &H41A)
                mRevPKMFont.Add(&HAC31, &H41B)
                mRevPKMFont.Add(&HAC38, &H41C)
                mRevPKMFont.Add(&HAC39, &H41D)
                mRevPKMFont.Add(&HAC3C, &H41E)
                mRevPKMFont.Add(&HAC40, &H41F)
                mRevPKMFont.Add(&HAC4B, &H420)
                mRevPKMFont.Add(&HAC4D, &H421)
                mRevPKMFont.Add(&HAC54, &H422)
                mRevPKMFont.Add(&HAC58, &H423)
                mRevPKMFont.Add(&HAC5C, &H424)
                mRevPKMFont.Add(&HAC70, &H425)
                mRevPKMFont.Add(&HAC71, &H426)
                mRevPKMFont.Add(&HAC74, &H427)
                mRevPKMFont.Add(&HAC77, &H428)
                mRevPKMFont.Add(&HAC78, &H429)
                mRevPKMFont.Add(&HAC7A, &H42A)
                mRevPKMFont.Add(&HAC80, &H42B)
                mRevPKMFont.Add(&HAC81, &H42C)
                mRevPKMFont.Add(&HAC83, &H42D)
                mRevPKMFont.Add(&HAC84, &H42E)
                mRevPKMFont.Add(&HAC85, &H42F)
                mRevPKMFont.Add(&HAC86, &H430)
                mRevPKMFont.Add(&HAC89, &H431)
                mRevPKMFont.Add(&HAC8A, &H432)
                mRevPKMFont.Add(&HAC8B, &H433)
                mRevPKMFont.Add(&HAC8C, &H434)
                mRevPKMFont.Add(&HAC90, &H435)
                mRevPKMFont.Add(&HAC94, &H436)
                mRevPKMFont.Add(&HAC9C, &H437)
                mRevPKMFont.Add(&HAC9D, &H438)
                mRevPKMFont.Add(&HAC9F, &H439)
                mRevPKMFont.Add(&HACA0, &H43A)
                mRevPKMFont.Add(&HACA1, &H43B)
                mRevPKMFont.Add(&HACA8, &H43C)
                mRevPKMFont.Add(&HACA9, &H43D)
                mRevPKMFont.Add(&HACAA, &H43E)
                mRevPKMFont.Add(&HACAC, &H43F)
                mRevPKMFont.Add(&HACAF, &H440)
                mRevPKMFont.Add(&HACB0, &H441)
                mRevPKMFont.Add(&HACB8, &H442)
                mRevPKMFont.Add(&HACB9, &H443)
                mRevPKMFont.Add(&HACBB, &H444)
                mRevPKMFont.Add(&HACBC, &H445)
                mRevPKMFont.Add(&HACBD, &H446)
                mRevPKMFont.Add(&HACC1, &H447)
                mRevPKMFont.Add(&HACC4, &H448)
                mRevPKMFont.Add(&HACC8, &H449)
                mRevPKMFont.Add(&HACCC, &H44A)
                mRevPKMFont.Add(&HACD5, &H44B)
                mRevPKMFont.Add(&HACD7, &H44C)
                mRevPKMFont.Add(&HACE0, &H44D)
                mRevPKMFont.Add(&HACE1, &H44E)
                mRevPKMFont.Add(&HACE4, &H44F)
                mRevPKMFont.Add(&HACE7, &H450)
                mRevPKMFont.Add(&HACE8, &H451)
                mRevPKMFont.Add(&HACEA, &H452)
                mRevPKMFont.Add(&HACEC, &H453)
                mRevPKMFont.Add(&HACEF, &H454)
                mRevPKMFont.Add(&HACF0, &H455)
                mRevPKMFont.Add(&HACF1, &H456)
                mRevPKMFont.Add(&HACF3, &H457)
                mRevPKMFont.Add(&HACF5, &H458)
                mRevPKMFont.Add(&HACF6, &H459)
                mRevPKMFont.Add(&HACFC, &H45A)
                mRevPKMFont.Add(&HACFD, &H45B)
                mRevPKMFont.Add(&HAD00, &H45C)
                mRevPKMFont.Add(&HAD04, &H45D)
                mRevPKMFont.Add(&HAD06, &H45E)
                mRevPKMFont.Add(&HAD0C, &H45F)
                mRevPKMFont.Add(&HAD0D, &H460)
                mRevPKMFont.Add(&HAD0F, &H461)
                mRevPKMFont.Add(&HAD11, &H462)
                mRevPKMFont.Add(&HAD18, &H463)
                mRevPKMFont.Add(&HAD1C, &H464)
                mRevPKMFont.Add(&HAD20, &H465)
                mRevPKMFont.Add(&HAD29, &H466)
                mRevPKMFont.Add(&HAD2C, &H467)
                mRevPKMFont.Add(&HAD2D, &H468)
                mRevPKMFont.Add(&HAD34, &H469)
                mRevPKMFont.Add(&HAD35, &H46A)
                mRevPKMFont.Add(&HAD38, &H46B)
                mRevPKMFont.Add(&HAD3C, &H46C)
                mRevPKMFont.Add(&HAD44, &H46D)
                mRevPKMFont.Add(&HAD45, &H46E)
                mRevPKMFont.Add(&HAD47, &H46F)
                mRevPKMFont.Add(&HAD49, &H470)
                mRevPKMFont.Add(&HAD50, &H471)
                mRevPKMFont.Add(&HAD54, &H472)
                mRevPKMFont.Add(&HAD58, &H473)
                mRevPKMFont.Add(&HAD61, &H474)
                mRevPKMFont.Add(&HAD63, &H475)
                mRevPKMFont.Add(&HAD6C, &H476)
                mRevPKMFont.Add(&HAD6D, &H477)
                mRevPKMFont.Add(&HAD70, &H478)
                mRevPKMFont.Add(&HAD73, &H479)
                mRevPKMFont.Add(&HAD74, &H47A)
                mRevPKMFont.Add(&HAD75, &H47B)
                mRevPKMFont.Add(&HAD76, &H47C)
                mRevPKMFont.Add(&HAD7B, &H47D)
                mRevPKMFont.Add(&HAD7C, &H47E)
                mRevPKMFont.Add(&HAD7D, &H47F)
                mRevPKMFont.Add(&HAD7F, &H480)
                mRevPKMFont.Add(&HAD81, &H481)
                mRevPKMFont.Add(&HAD82, &H482)
                mRevPKMFont.Add(&HAD88, &H483)
                mRevPKMFont.Add(&HAD89, &H484)
                mRevPKMFont.Add(&HAD8C, &H485)
                mRevPKMFont.Add(&HAD90, &H486)
                mRevPKMFont.Add(&HAD9C, &H487)
                mRevPKMFont.Add(&HAD9D, &H488)
                mRevPKMFont.Add(&HADA4, &H489)
                mRevPKMFont.Add(&HADB7, &H48A)
                mRevPKMFont.Add(&HADC0, &H48B)
                mRevPKMFont.Add(&HADC1, &H48C)
                mRevPKMFont.Add(&HADC4, &H48D)
                mRevPKMFont.Add(&HADC8, &H48E)
                mRevPKMFont.Add(&HADD0, &H48F)
                mRevPKMFont.Add(&HADD1, &H490)
                mRevPKMFont.Add(&HADD3, &H491)
                mRevPKMFont.Add(&HADDC, &H492)
                mRevPKMFont.Add(&HADE0, &H493)
                mRevPKMFont.Add(&HADE4, &H494)
                mRevPKMFont.Add(&HADF8, &H495)
                mRevPKMFont.Add(&HADF9, &H496)
                mRevPKMFont.Add(&HADFC, &H497)
                mRevPKMFont.Add(&HADFF, &H498)
                mRevPKMFont.Add(&HAE00, &H499)
                mRevPKMFont.Add(&HAE01, &H49A)
                mRevPKMFont.Add(&HAE08, &H49B)
                mRevPKMFont.Add(&HAE09, &H49C)
                mRevPKMFont.Add(&HAE0B, &H49D)
                mRevPKMFont.Add(&HAE0D, &H49E)
                mRevPKMFont.Add(&HAE14, &H49F)
                mRevPKMFont.Add(&HAE30, &H4A0)
                mRevPKMFont.Add(&HAE31, &H4A1)
                mRevPKMFont.Add(&HAE34, &H4A2)
                mRevPKMFont.Add(&HAE37, &H4A3)
                mRevPKMFont.Add(&HAE38, &H4A4)
                mRevPKMFont.Add(&HAE3A, &H4A5)
                mRevPKMFont.Add(&HAE40, &H4A6)
                mRevPKMFont.Add(&HAE41, &H4A7)
                mRevPKMFont.Add(&HAE43, &H4A8)
                mRevPKMFont.Add(&HAE45, &H4A9)
                mRevPKMFont.Add(&HAE46, &H4AA)
                mRevPKMFont.Add(&HAE4A, &H4AB)
                mRevPKMFont.Add(&HAE4C, &H4AC)
                mRevPKMFont.Add(&HAE4D, &H4AD)
                mRevPKMFont.Add(&HAE4E, &H4AE)
                mRevPKMFont.Add(&HAE50, &H4AF)
                mRevPKMFont.Add(&HAE54, &H4B0)
                mRevPKMFont.Add(&HAE56, &H4B1)
                mRevPKMFont.Add(&HAE5C, &H4B2)
                mRevPKMFont.Add(&HAE5D, &H4B3)
                mRevPKMFont.Add(&HAE5F, &H4B4)
                mRevPKMFont.Add(&HAE60, &H4B5)
                mRevPKMFont.Add(&HAE61, &H4B6)
                mRevPKMFont.Add(&HAE65, &H4B7)
                mRevPKMFont.Add(&HAE68, &H4B8)
                mRevPKMFont.Add(&HAE69, &H4B9)
                mRevPKMFont.Add(&HAE6C, &H4BA)
                mRevPKMFont.Add(&HAE70, &H4BB)
                mRevPKMFont.Add(&HAE78, &H4BC)
                mRevPKMFont.Add(&HAE79, &H4BD)
                mRevPKMFont.Add(&HAE7B, &H4BE)
                mRevPKMFont.Add(&HAE7C, &H4BF)
                mRevPKMFont.Add(&HAE7D, &H4C0)
                mRevPKMFont.Add(&HAE84, &H4C1)
                mRevPKMFont.Add(&HAE85, &H4C2)
                mRevPKMFont.Add(&HAE8C, &H4C3)
                mRevPKMFont.Add(&HAEBC, &H4C4)
                mRevPKMFont.Add(&HAEBD, &H4C5)
                mRevPKMFont.Add(&HAEBE, &H4C6)
                mRevPKMFont.Add(&HAEC0, &H4C7)
                mRevPKMFont.Add(&HAEC4, &H4C8)
                mRevPKMFont.Add(&HAECC, &H4C9)
                mRevPKMFont.Add(&HAECD, &H4CA)
                mRevPKMFont.Add(&HAECF, &H4CB)
                mRevPKMFont.Add(&HAED0, &H4CC)
                mRevPKMFont.Add(&HAED1, &H4CD)
                mRevPKMFont.Add(&HAED8, &H4CE)
                mRevPKMFont.Add(&HAED9, &H4CF)
                mRevPKMFont.Add(&HAEDC, &H4D0)
                mRevPKMFont.Add(&HAEE8, &H4D1)
                mRevPKMFont.Add(&HAEEB, &H4D2)
                mRevPKMFont.Add(&HAEED, &H4D3)
                mRevPKMFont.Add(&HAEF4, &H4D4)
                mRevPKMFont.Add(&HAEF8, &H4D5)
                mRevPKMFont.Add(&HAEFC, &H4D6)
                mRevPKMFont.Add(&HAF07, &H4D7)
                mRevPKMFont.Add(&HAF08, &H4D8)
                mRevPKMFont.Add(&HAF0D, &H4D9)
                mRevPKMFont.Add(&HAF10, &H4DA)
                mRevPKMFont.Add(&HAF2C, &H4DB)
                mRevPKMFont.Add(&HAF2D, &H4DC)
                mRevPKMFont.Add(&HAF30, &H4DD)
                mRevPKMFont.Add(&HAF32, &H4DE)
                mRevPKMFont.Add(&HAF34, &H4DF)
                mRevPKMFont.Add(&HAF3C, &H4E0)
                mRevPKMFont.Add(&HAF3D, &H4E1)
                mRevPKMFont.Add(&HAF3F, &H4E2)
                mRevPKMFont.Add(&HAF41, &H4E3)
                mRevPKMFont.Add(&HAF42, &H4E4)
                mRevPKMFont.Add(&HAF43, &H4E5)
                mRevPKMFont.Add(&HAF48, &H4E6)
                mRevPKMFont.Add(&HAF49, &H4E7)
                mRevPKMFont.Add(&HAF50, &H4E8)
                mRevPKMFont.Add(&HAF5C, &H4E9)
                mRevPKMFont.Add(&HAF5D, &H4EA)
                mRevPKMFont.Add(&HAF64, &H4EB)
                mRevPKMFont.Add(&HAF65, &H4EC)
                mRevPKMFont.Add(&HAF79, &H4ED)
                mRevPKMFont.Add(&HAF80, &H4EE)
                mRevPKMFont.Add(&HAF84, &H4EF)
                mRevPKMFont.Add(&HAF88, &H4F0)
                mRevPKMFont.Add(&HAF90, &H4F1)
                mRevPKMFont.Add(&HAF91, &H4F2)
                mRevPKMFont.Add(&HAF95, &H4F3)
                mRevPKMFont.Add(&HAF9C, &H4F4)
                mRevPKMFont.Add(&HAFB8, &H4F5)
                mRevPKMFont.Add(&HAFB9, &H4F6)
                mRevPKMFont.Add(&HAFBC, &H4F7)
                mRevPKMFont.Add(&HAFC0, &H4F8)
                mRevPKMFont.Add(&HAFC7, &H4F9)
                mRevPKMFont.Add(&HAFC8, &H4FA)
                mRevPKMFont.Add(&HAFC9, &H4FB)
                mRevPKMFont.Add(&HAFCB, &H4FC)
                mRevPKMFont.Add(&HAFCD, &H4FD)
                mRevPKMFont.Add(&HAFCE, &H4FE)
                mRevPKMFont.Add(&HAFD4, &H4FF)
                mRevPKMFont.Add(&HAFDC, &H500)
                mRevPKMFont.Add(&HAFE8, &H501)
                mRevPKMFont.Add(&HAFE9, &H502)
                mRevPKMFont.Add(&HAFF0, &H503)
                mRevPKMFont.Add(&HAFF1, &H504)
                mRevPKMFont.Add(&HAFF4, &H505)
                mRevPKMFont.Add(&HAFF8, &H506)
                mRevPKMFont.Add(&HB000, &H507)
                mRevPKMFont.Add(&HB001, &H508)
                mRevPKMFont.Add(&HB004, &H509)
                mRevPKMFont.Add(&HB00C, &H50A)
                mRevPKMFont.Add(&HB010, &H50B)
                mRevPKMFont.Add(&HB014, &H50C)
                mRevPKMFont.Add(&HB01C, &H50D)
                mRevPKMFont.Add(&HB01D, &H50E)
                mRevPKMFont.Add(&HB028, &H50F)
                mRevPKMFont.Add(&HB044, &H510)
                mRevPKMFont.Add(&HB045, &H511)
                mRevPKMFont.Add(&HB048, &H512)
                mRevPKMFont.Add(&HB04A, &H513)
                mRevPKMFont.Add(&HB04C, &H514)
                mRevPKMFont.Add(&HB04E, &H515)
                mRevPKMFont.Add(&HB053, &H516)
                mRevPKMFont.Add(&HB054, &H517)
                mRevPKMFont.Add(&HB055, &H518)
                mRevPKMFont.Add(&HB057, &H519)
                mRevPKMFont.Add(&HB059, &H51A)
                mRevPKMFont.Add(&HB05D, &H51B)
                mRevPKMFont.Add(&HB07C, &H51C)
                mRevPKMFont.Add(&HB07D, &H51D)
                mRevPKMFont.Add(&HB080, &H51E)
                mRevPKMFont.Add(&HB084, &H51F)
                mRevPKMFont.Add(&HB08C, &H520)
                mRevPKMFont.Add(&HB08D, &H521)
                mRevPKMFont.Add(&HB08F, &H522)
                mRevPKMFont.Add(&HB091, &H523)
                mRevPKMFont.Add(&HB098, &H524)
                mRevPKMFont.Add(&HB099, &H525)
                mRevPKMFont.Add(&HB09A, &H526)
                mRevPKMFont.Add(&HB09C, &H527)
                mRevPKMFont.Add(&HB09F, &H528)
                mRevPKMFont.Add(&HB0A0, &H529)
                mRevPKMFont.Add(&HB0A1, &H52A)
                mRevPKMFont.Add(&HB0A2, &H52B)
                mRevPKMFont.Add(&HB0A8, &H52C)
                mRevPKMFont.Add(&HB0A9, &H52D)
                mRevPKMFont.Add(&HB0AB, &H52E)
                mRevPKMFont.Add(&HB0AC, &H52F)
                mRevPKMFont.Add(&HB0AD, &H530)
                mRevPKMFont.Add(&HB0AE, &H531)
                mRevPKMFont.Add(&HB0AF, &H532)
                mRevPKMFont.Add(&HB0B1, &H533)
                mRevPKMFont.Add(&HB0B3, &H534)
                mRevPKMFont.Add(&HB0B4, &H535)
                mRevPKMFont.Add(&HB0B5, &H536)
                mRevPKMFont.Add(&HB0B8, &H537)
                mRevPKMFont.Add(&HB0BC, &H538)
                mRevPKMFont.Add(&HB0C4, &H539)
                mRevPKMFont.Add(&HB0C5, &H53A)
                mRevPKMFont.Add(&HB0C7, &H53B)
                mRevPKMFont.Add(&HB0C8, &H53C)
                mRevPKMFont.Add(&HB0C9, &H53D)
                mRevPKMFont.Add(&HB0D0, &H53E)
                mRevPKMFont.Add(&HB0D1, &H53F)
                mRevPKMFont.Add(&HB0D4, &H540)
                mRevPKMFont.Add(&HB0D8, &H541)
                mRevPKMFont.Add(&HB0E0, &H542)
                mRevPKMFont.Add(&HB0E5, &H543)
                mRevPKMFont.Add(&HB108, &H544)
                mRevPKMFont.Add(&HB109, &H545)
                mRevPKMFont.Add(&HB10B, &H546)
                mRevPKMFont.Add(&HB10C, &H547)
                mRevPKMFont.Add(&HB110, &H548)
                mRevPKMFont.Add(&HB112, &H549)
                mRevPKMFont.Add(&HB113, &H54A)
                mRevPKMFont.Add(&HB118, &H54B)
                mRevPKMFont.Add(&HB119, &H54C)
                mRevPKMFont.Add(&HB11B, &H54D)
                mRevPKMFont.Add(&HB11C, &H54E)
                mRevPKMFont.Add(&HB11D, &H54F)
                mRevPKMFont.Add(&HB123, &H550)
                mRevPKMFont.Add(&HB124, &H551)
                mRevPKMFont.Add(&HB125, &H552)
                mRevPKMFont.Add(&HB128, &H553)
                mRevPKMFont.Add(&HB12C, &H554)
                mRevPKMFont.Add(&HB134, &H555)
                mRevPKMFont.Add(&HB135, &H556)
                mRevPKMFont.Add(&HB137, &H557)
                mRevPKMFont.Add(&HB138, &H558)
                mRevPKMFont.Add(&HB139, &H559)
                mRevPKMFont.Add(&HB140, &H55A)
                mRevPKMFont.Add(&HB141, &H55B)
                mRevPKMFont.Add(&HB144, &H55C)
                mRevPKMFont.Add(&HB148, &H55D)
                mRevPKMFont.Add(&HB150, &H55E)
                mRevPKMFont.Add(&HB151, &H55F)
                mRevPKMFont.Add(&HB154, &H560)
                mRevPKMFont.Add(&HB155, &H561)
                mRevPKMFont.Add(&HB158, &H562)
                mRevPKMFont.Add(&HB15C, &H563)
                mRevPKMFont.Add(&HB160, &H564)
                mRevPKMFont.Add(&HB178, &H565)
                mRevPKMFont.Add(&HB179, &H566)
                mRevPKMFont.Add(&HB17C, &H567)
                mRevPKMFont.Add(&HB180, &H568)
                mRevPKMFont.Add(&HB182, &H569)
                mRevPKMFont.Add(&HB188, &H56A)
                mRevPKMFont.Add(&HB189, &H56B)
                mRevPKMFont.Add(&HB18B, &H56C)
                mRevPKMFont.Add(&HB18D, &H56D)
                mRevPKMFont.Add(&HB192, &H56E)
                mRevPKMFont.Add(&HB193, &H56F)
                mRevPKMFont.Add(&HB194, &H570)
                mRevPKMFont.Add(&HB198, &H571)
                mRevPKMFont.Add(&HB19C, &H572)
                mRevPKMFont.Add(&HB1A8, &H573)
                mRevPKMFont.Add(&HB1CC, &H574)
                mRevPKMFont.Add(&HB1D0, &H575)
                mRevPKMFont.Add(&HB1D4, &H576)
                mRevPKMFont.Add(&HB1DC, &H577)
                mRevPKMFont.Add(&HB1DD, &H578)
                mRevPKMFont.Add(&HB1DF, &H579)
                mRevPKMFont.Add(&HB1E8, &H57A)
                mRevPKMFont.Add(&HB1E9, &H57B)
                mRevPKMFont.Add(&HB1EC, &H57C)
                mRevPKMFont.Add(&HB1F0, &H57D)
                mRevPKMFont.Add(&HB1F9, &H57E)
                mRevPKMFont.Add(&HB1FB, &H57F)
                mRevPKMFont.Add(&HB1FD, &H580)
                mRevPKMFont.Add(&HB204, &H581)
                mRevPKMFont.Add(&HB205, &H582)
                mRevPKMFont.Add(&HB208, &H583)
                mRevPKMFont.Add(&HB20B, &H584)
                mRevPKMFont.Add(&HB20C, &H585)
                mRevPKMFont.Add(&HB214, &H586)
                mRevPKMFont.Add(&HB215, &H587)
                mRevPKMFont.Add(&HB217, &H588)
                mRevPKMFont.Add(&HB219, &H589)
                mRevPKMFont.Add(&HB220, &H58A)
                mRevPKMFont.Add(&HB234, &H58B)
                mRevPKMFont.Add(&HB23C, &H58C)
                mRevPKMFont.Add(&HB258, &H58D)
                mRevPKMFont.Add(&HB25C, &H58E)
                mRevPKMFont.Add(&HB260, &H58F)
                mRevPKMFont.Add(&HB268, &H590)
                mRevPKMFont.Add(&HB269, &H591)
                mRevPKMFont.Add(&HB274, &H592)
                mRevPKMFont.Add(&HB275, &H593)
                mRevPKMFont.Add(&HB27C, &H594)
                mRevPKMFont.Add(&HB284, &H595)
                mRevPKMFont.Add(&HB285, &H596)
                mRevPKMFont.Add(&HB289, &H597)
                mRevPKMFont.Add(&HB290, &H598)
                mRevPKMFont.Add(&HB291, &H599)
                mRevPKMFont.Add(&HB294, &H59A)
                mRevPKMFont.Add(&HB298, &H59B)
                mRevPKMFont.Add(&HB299, &H59C)
                mRevPKMFont.Add(&HB29A, &H59D)
                mRevPKMFont.Add(&HB2A0, &H59E)
                mRevPKMFont.Add(&HB2A1, &H59F)
                mRevPKMFont.Add(&HB2A3, &H5A0)
                mRevPKMFont.Add(&HB2A5, &H5A1)
                mRevPKMFont.Add(&HB2A6, &H5A2)
                mRevPKMFont.Add(&HB2AA, &H5A3)
                mRevPKMFont.Add(&HB2AC, &H5A4)
                mRevPKMFont.Add(&HB2B0, &H5A5)
                mRevPKMFont.Add(&HB2B4, &H5A6)
                mRevPKMFont.Add(&HB2C8, &H5A7)
                mRevPKMFont.Add(&HB2C9, &H5A8)
                mRevPKMFont.Add(&HB2CC, &H5A9)
                mRevPKMFont.Add(&HB2D0, &H5AA)
                mRevPKMFont.Add(&HB2D2, &H5AB)
                mRevPKMFont.Add(&HB2D8, &H5AC)
                mRevPKMFont.Add(&HB2D9, &H5AD)
                mRevPKMFont.Add(&HB2DB, &H5AE)
                mRevPKMFont.Add(&HB2DD, &H5AF)
                mRevPKMFont.Add(&HB2E2, &H5B0)
                mRevPKMFont.Add(&HB2E4, &H5B1)
                mRevPKMFont.Add(&HB2E5, &H5B2)
                mRevPKMFont.Add(&HB2E6, &H5B3)
                mRevPKMFont.Add(&HB2E8, &H5B4)
                mRevPKMFont.Add(&HB2EB, &H5B5)
                mRevPKMFont.Add(&HB2EC, &H5B6)
                mRevPKMFont.Add(&HB2ED, &H5B7)
                mRevPKMFont.Add(&HB2EE, &H5B8)
                mRevPKMFont.Add(&HB2EF, &H5B9)
                mRevPKMFont.Add(&HB2F3, &H5BA)
                mRevPKMFont.Add(&HB2F4, &H5BB)
                mRevPKMFont.Add(&HB2F5, &H5BC)
                mRevPKMFont.Add(&HB2F7, &H5BD)
                mRevPKMFont.Add(&HB2F8, &H5BE)
                mRevPKMFont.Add(&HB2F9, &H5BF)
                mRevPKMFont.Add(&HB2FA, &H5C0)
                mRevPKMFont.Add(&HB2FB, &H5C1)
                mRevPKMFont.Add(&HB2FF, &H5C2)
                mRevPKMFont.Add(&HB300, &H5C3)
                mRevPKMFont.Add(&HB301, &H5C4)
                mRevPKMFont.Add(&HB304, &H5C5)
                mRevPKMFont.Add(&HB308, &H5C6)
                mRevPKMFont.Add(&HB310, &H5C7)
                mRevPKMFont.Add(&HB311, &H5C8)
                mRevPKMFont.Add(&HB313, &H5C9)
                mRevPKMFont.Add(&HB314, &H5CA)
                mRevPKMFont.Add(&HB315, &H5CB)
                mRevPKMFont.Add(&HB31C, &H5CC)
                mRevPKMFont.Add(&HB354, &H5CD)
                mRevPKMFont.Add(&HB355, &H5CE)
                mRevPKMFont.Add(&HB356, &H5CF)
                mRevPKMFont.Add(&HB358, &H5D0)
                mRevPKMFont.Add(&HB35B, &H5D1)
                mRevPKMFont.Add(&HB35C, &H5D2)
                mRevPKMFont.Add(&HB35E, &H5D3)
                mRevPKMFont.Add(&HB35F, &H5D4)
                mRevPKMFont.Add(&HB364, &H5D5)
                mRevPKMFont.Add(&HB365, &H5D6)
                mRevPKMFont.Add(&HB367, &H5D7)
                mRevPKMFont.Add(&HB369, &H5D8)
                mRevPKMFont.Add(&HB36B, &H5D9)
                mRevPKMFont.Add(&HB36E, &H5DA)
                mRevPKMFont.Add(&HB370, &H5DB)
                mRevPKMFont.Add(&HB371, &H5DC)
                mRevPKMFont.Add(&HB374, &H5DD)
                mRevPKMFont.Add(&HB378, &H5DE)
                mRevPKMFont.Add(&HB380, &H5DF)
                mRevPKMFont.Add(&HB381, &H5E0)
                mRevPKMFont.Add(&HB383, &H5E1)
                mRevPKMFont.Add(&HB384, &H5E2)
                mRevPKMFont.Add(&HB385, &H5E3)
                mRevPKMFont.Add(&HB38C, &H5E4)
                mRevPKMFont.Add(&HB390, &H5E5)
                mRevPKMFont.Add(&HB394, &H5E6)
                mRevPKMFont.Add(&HB3A0, &H5E7)
                mRevPKMFont.Add(&HB3A1, &H5E8)
                mRevPKMFont.Add(&HB3A8, &H5E9)
                mRevPKMFont.Add(&HB3AC, &H5EA)
                mRevPKMFont.Add(&HB3C4, &H5EB)
                mRevPKMFont.Add(&HB3C5, &H5EC)
                mRevPKMFont.Add(&HB3C8, &H5ED)
                mRevPKMFont.Add(&HB3CB, &H5EE)
                mRevPKMFont.Add(&HB3CC, &H5EF)
                mRevPKMFont.Add(&HB3CE, &H5F0)
                mRevPKMFont.Add(&HB3D0, &H5F1)
                mRevPKMFont.Add(&HB3D4, &H5F2)
                mRevPKMFont.Add(&HB3D5, &H5F3)
                mRevPKMFont.Add(&HB3D7, &H5F4)
                mRevPKMFont.Add(&HB3D9, &H5F5)
                mRevPKMFont.Add(&HB3DB, &H5F6)
                mRevPKMFont.Add(&HB3DD, &H5F7)
                mRevPKMFont.Add(&HB3E0, &H5F8)
                mRevPKMFont.Add(&HB3E4, &H5F9)
                mRevPKMFont.Add(&HB3E8, &H5FA)
                mRevPKMFont.Add(&HB3FC, &H5FB)
                mRevPKMFont.Add(&HB410, &H5FC)
                mRevPKMFont.Add(&HB418, &H5FD)
                mRevPKMFont.Add(&HB41C, &H5FE)
                mRevPKMFont.Add(&HB420, &H5FF)
                mRevPKMFont.Add(&HB428, &H600)
                mRevPKMFont.Add(&HB429, &H601)
                mRevPKMFont.Add(&HB42B, &H602)
                mRevPKMFont.Add(&HB434, &H603)
                mRevPKMFont.Add(&HB450, &H604)
                mRevPKMFont.Add(&HB451, &H605)
                mRevPKMFont.Add(&HB454, &H606)
                mRevPKMFont.Add(&HB458, &H607)
                mRevPKMFont.Add(&HB460, &H608)
                mRevPKMFont.Add(&HB461, &H609)
                mRevPKMFont.Add(&HB463, &H60A)
                mRevPKMFont.Add(&HB465, &H60B)
                mRevPKMFont.Add(&HB46C, &H60C)
                mRevPKMFont.Add(&HB480, &H60D)
                mRevPKMFont.Add(&HB488, &H60E)
                mRevPKMFont.Add(&HB49D, &H60F)
                mRevPKMFont.Add(&HB4A4, &H610)
                mRevPKMFont.Add(&HB4A8, &H611)
                mRevPKMFont.Add(&HB4AC, &H612)
                mRevPKMFont.Add(&HB4B5, &H613)
                mRevPKMFont.Add(&HB4B7, &H614)
                mRevPKMFont.Add(&HB4B9, &H615)
                mRevPKMFont.Add(&HB4C0, &H616)
                mRevPKMFont.Add(&HB4C4, &H617)
                mRevPKMFont.Add(&HB4C8, &H618)
                mRevPKMFont.Add(&HB4D0, &H619)
                mRevPKMFont.Add(&HB4D5, &H61A)
                mRevPKMFont.Add(&HB4DC, &H61B)
                mRevPKMFont.Add(&HB4DD, &H61C)
                mRevPKMFont.Add(&HB4E0, &H61D)
                mRevPKMFont.Add(&HB4E3, &H61E)
                mRevPKMFont.Add(&HB4E4, &H61F)
                mRevPKMFont.Add(&HB4E6, &H620)
                mRevPKMFont.Add(&HB4EC, &H621)
                mRevPKMFont.Add(&HB4ED, &H622)
                mRevPKMFont.Add(&HB4EF, &H623)
                mRevPKMFont.Add(&HB4F1, &H624)
                mRevPKMFont.Add(&HB4F8, &H625)
                mRevPKMFont.Add(&HB514, &H626)
                mRevPKMFont.Add(&HB515, &H627)
                mRevPKMFont.Add(&HB518, &H628)
                mRevPKMFont.Add(&HB51B, &H629)
                mRevPKMFont.Add(&HB51C, &H62A)
                mRevPKMFont.Add(&HB524, &H62B)
                mRevPKMFont.Add(&HB525, &H62C)
                mRevPKMFont.Add(&HB527, &H62D)
                mRevPKMFont.Add(&HB528, &H62E)
                mRevPKMFont.Add(&HB529, &H62F)
                mRevPKMFont.Add(&HB52A, &H630)
                mRevPKMFont.Add(&HB530, &H631)
                mRevPKMFont.Add(&HB531, &H632)
                mRevPKMFont.Add(&HB534, &H633)
                mRevPKMFont.Add(&HB538, &H634)
                mRevPKMFont.Add(&HB540, &H635)
                mRevPKMFont.Add(&HB541, &H636)
                mRevPKMFont.Add(&HB543, &H637)
                mRevPKMFont.Add(&HB544, &H638)
                mRevPKMFont.Add(&HB545, &H639)
                mRevPKMFont.Add(&HB54B, &H63A)
                mRevPKMFont.Add(&HB54C, &H63B)
                mRevPKMFont.Add(&HB54D, &H63C)
                mRevPKMFont.Add(&HB550, &H63D)
                mRevPKMFont.Add(&HB554, &H63E)
                mRevPKMFont.Add(&HB55C, &H63F)
                mRevPKMFont.Add(&HB55D, &H640)
                mRevPKMFont.Add(&HB55F, &H641)
                mRevPKMFont.Add(&HB560, &H642)
                mRevPKMFont.Add(&HB561, &H643)
                mRevPKMFont.Add(&HB5A0, &H644)
                mRevPKMFont.Add(&HB5A1, &H645)
                mRevPKMFont.Add(&HB5A4, &H646)
                mRevPKMFont.Add(&HB5A8, &H647)
                mRevPKMFont.Add(&HB5AA, &H648)
                mRevPKMFont.Add(&HB5AB, &H649)
                mRevPKMFont.Add(&HB5B0, &H64A)
                mRevPKMFont.Add(&HB5B1, &H64B)
                mRevPKMFont.Add(&HB5B3, &H64C)
                mRevPKMFont.Add(&HB5B4, &H64D)
                mRevPKMFont.Add(&HB5B5, &H64E)
                mRevPKMFont.Add(&HB5BB, &H64F)
                mRevPKMFont.Add(&HB5BC, &H650)
                mRevPKMFont.Add(&HB5BD, &H651)
                mRevPKMFont.Add(&HB5C0, &H652)
                mRevPKMFont.Add(&HB5C4, &H653)
                mRevPKMFont.Add(&HB5CC, &H654)
                mRevPKMFont.Add(&HB5CD, &H655)
                mRevPKMFont.Add(&HB5CF, &H656)
                mRevPKMFont.Add(&HB5D0, &H657)
                mRevPKMFont.Add(&HB5D1, &H658)
                mRevPKMFont.Add(&HB5D8, &H659)
                mRevPKMFont.Add(&HB5EC, &H65A)
                mRevPKMFont.Add(&HB610, &H65B)
                mRevPKMFont.Add(&HB611, &H65C)
                mRevPKMFont.Add(&HB614, &H65D)
                mRevPKMFont.Add(&HB618, &H65E)
                mRevPKMFont.Add(&HB625, &H65F)
                mRevPKMFont.Add(&HB62C, &H660)
                mRevPKMFont.Add(&HB634, &H661)
                mRevPKMFont.Add(&HB648, &H662)
                mRevPKMFont.Add(&HB664, &H663)
                mRevPKMFont.Add(&HB668, &H664)
                mRevPKMFont.Add(&HB69C, &H665)
                mRevPKMFont.Add(&HB69D, &H666)
                mRevPKMFont.Add(&HB6A0, &H667)
                mRevPKMFont.Add(&HB6A4, &H668)
                mRevPKMFont.Add(&HB6AB, &H669)
                mRevPKMFont.Add(&HB6AC, &H66A)
                mRevPKMFont.Add(&HB6B1, &H66B)
                mRevPKMFont.Add(&HB6D4, &H66C)
                mRevPKMFont.Add(&HB6F0, &H66D)
                mRevPKMFont.Add(&HB6F4, &H66E)
                mRevPKMFont.Add(&HB6F8, &H66F)
                mRevPKMFont.Add(&HB700, &H670)
                mRevPKMFont.Add(&HB701, &H671)
                mRevPKMFont.Add(&HB705, &H672)
                mRevPKMFont.Add(&HB728, &H673)
                mRevPKMFont.Add(&HB729, &H674)
                mRevPKMFont.Add(&HB72C, &H675)
                mRevPKMFont.Add(&HB72F, &H676)
                mRevPKMFont.Add(&HB730, &H677)
                mRevPKMFont.Add(&HB738, &H678)
                mRevPKMFont.Add(&HB739, &H679)
                mRevPKMFont.Add(&HB73B, &H67A)
                mRevPKMFont.Add(&HB744, &H67B)
                mRevPKMFont.Add(&HB748, &H67C)
                mRevPKMFont.Add(&HB74C, &H67D)
                mRevPKMFont.Add(&HB754, &H67E)
                mRevPKMFont.Add(&HB755, &H67F)
                mRevPKMFont.Add(&HB760, &H680)
                mRevPKMFont.Add(&HB764, &H681)
                mRevPKMFont.Add(&HB768, &H682)
                mRevPKMFont.Add(&HB770, &H683)
                mRevPKMFont.Add(&HB771, &H684)
                mRevPKMFont.Add(&HB773, &H685)
                mRevPKMFont.Add(&HB775, &H686)
                mRevPKMFont.Add(&HB77C, &H687)
                mRevPKMFont.Add(&HB77D, &H688)
                mRevPKMFont.Add(&HB780, &H689)
                mRevPKMFont.Add(&HB784, &H68A)
                mRevPKMFont.Add(&HB78C, &H68B)
                mRevPKMFont.Add(&HB78D, &H68C)
                mRevPKMFont.Add(&HB78F, &H68D)
                mRevPKMFont.Add(&HB790, &H68E)
                mRevPKMFont.Add(&HB791, &H68F)
                mRevPKMFont.Add(&HB792, &H690)
                mRevPKMFont.Add(&HB796, &H691)
                mRevPKMFont.Add(&HB797, &H692)
                mRevPKMFont.Add(&HB798, &H693)
                mRevPKMFont.Add(&HB799, &H694)
                mRevPKMFont.Add(&HB79C, &H695)
                mRevPKMFont.Add(&HB7A0, &H696)
                mRevPKMFont.Add(&HB7A8, &H697)
                mRevPKMFont.Add(&HB7A9, &H698)
                mRevPKMFont.Add(&HB7AB, &H699)
                mRevPKMFont.Add(&HB7AC, &H69A)
                mRevPKMFont.Add(&HB7AD, &H69B)
                mRevPKMFont.Add(&HB7B4, &H69C)
                mRevPKMFont.Add(&HB7B5, &H69D)
                mRevPKMFont.Add(&HB7B8, &H69E)
                mRevPKMFont.Add(&HB7C7, &H69F)
                mRevPKMFont.Add(&HB7C9, &H6A0)
                mRevPKMFont.Add(&HB7EC, &H6A1)
                mRevPKMFont.Add(&HB7ED, &H6A2)
                mRevPKMFont.Add(&HB7F0, &H6A3)
                mRevPKMFont.Add(&HB7F4, &H6A4)
                mRevPKMFont.Add(&HB7FC, &H6A5)
                mRevPKMFont.Add(&HB7FD, &H6A6)
                mRevPKMFont.Add(&HB7FF, &H6A7)
                mRevPKMFont.Add(&HB800, &H6A8)
                mRevPKMFont.Add(&HB801, &H6A9)
                mRevPKMFont.Add(&HB807, &H6AA)
                mRevPKMFont.Add(&HB808, &H6AB)
                mRevPKMFont.Add(&HB809, &H6AC)
                mRevPKMFont.Add(&HB80C, &H6AD)
                mRevPKMFont.Add(&HB810, &H6AE)
                mRevPKMFont.Add(&HB818, &H6AF)
                mRevPKMFont.Add(&HB819, &H6B0)
                mRevPKMFont.Add(&HB81B, &H6B1)
                mRevPKMFont.Add(&HB81D, &H6B2)
                mRevPKMFont.Add(&HB824, &H6B3)
                mRevPKMFont.Add(&HB825, &H6B4)
                mRevPKMFont.Add(&HB828, &H6B5)
                mRevPKMFont.Add(&HB82C, &H6B6)
                mRevPKMFont.Add(&HB834, &H6B7)
                mRevPKMFont.Add(&HB835, &H6B8)
                mRevPKMFont.Add(&HB837, &H6B9)
                mRevPKMFont.Add(&HB838, &H6BA)
                mRevPKMFont.Add(&HB839, &H6BB)
                mRevPKMFont.Add(&HB840, &H6BC)
                mRevPKMFont.Add(&HB844, &H6BD)
                mRevPKMFont.Add(&HB851, &H6BE)
                mRevPKMFont.Add(&HB853, &H6BF)
                mRevPKMFont.Add(&HB85C, &H6C0)
                mRevPKMFont.Add(&HB85D, &H6C1)
                mRevPKMFont.Add(&HB860, &H6C2)
                mRevPKMFont.Add(&HB864, &H6C3)
                mRevPKMFont.Add(&HB86C, &H6C4)
                mRevPKMFont.Add(&HB86D, &H6C5)
                mRevPKMFont.Add(&HB86F, &H6C6)
                mRevPKMFont.Add(&HB871, &H6C7)
                mRevPKMFont.Add(&HB878, &H6C8)
                mRevPKMFont.Add(&HB87C, &H6C9)
                mRevPKMFont.Add(&HB88D, &H6CA)
                mRevPKMFont.Add(&HB8A8, &H6CB)
                mRevPKMFont.Add(&HB8B0, &H6CC)
                mRevPKMFont.Add(&HB8B4, &H6CD)
                mRevPKMFont.Add(&HB8B8, &H6CE)
                mRevPKMFont.Add(&HB8C0, &H6CF)
                mRevPKMFont.Add(&HB8C1, &H6D0)
                mRevPKMFont.Add(&HB8C3, &H6D1)
                mRevPKMFont.Add(&HB8C5, &H6D2)
                mRevPKMFont.Add(&HB8CC, &H6D3)
                mRevPKMFont.Add(&HB8D0, &H6D4)
                mRevPKMFont.Add(&HB8D4, &H6D5)
                mRevPKMFont.Add(&HB8DD, &H6D6)
                mRevPKMFont.Add(&HB8DF, &H6D7)
                mRevPKMFont.Add(&HB8E1, &H6D8)
                mRevPKMFont.Add(&HB8E8, &H6D9)
                mRevPKMFont.Add(&HB8E9, &H6DA)
                mRevPKMFont.Add(&HB8EC, &H6DB)
                mRevPKMFont.Add(&HB8F0, &H6DC)
                mRevPKMFont.Add(&HB8F8, &H6DD)
                mRevPKMFont.Add(&HB8F9, &H6DE)
                mRevPKMFont.Add(&HB8FB, &H6DF)
                mRevPKMFont.Add(&HB8FD, &H6E0)
                mRevPKMFont.Add(&HB904, &H6E1)
                mRevPKMFont.Add(&HB918, &H6E2)
                mRevPKMFont.Add(&HB920, &H6E3)
                mRevPKMFont.Add(&HB93C, &H6E4)
                mRevPKMFont.Add(&HB93D, &H6E5)
                mRevPKMFont.Add(&HB940, &H6E6)
                mRevPKMFont.Add(&HB944, &H6E7)
                mRevPKMFont.Add(&HB94C, &H6E8)
                mRevPKMFont.Add(&HB94F, &H6E9)
                mRevPKMFont.Add(&HB951, &H6EA)
                mRevPKMFont.Add(&HB958, &H6EB)
                mRevPKMFont.Add(&HB959, &H6EC)
                mRevPKMFont.Add(&HB95C, &H6ED)
                mRevPKMFont.Add(&HB960, &H6EE)
                mRevPKMFont.Add(&HB968, &H6EF)
                mRevPKMFont.Add(&HB969, &H6F0)
                mRevPKMFont.Add(&HB96B, &H6F1)
                mRevPKMFont.Add(&HB96D, &H6F2)
                mRevPKMFont.Add(&HB974, &H6F3)
                mRevPKMFont.Add(&HB975, &H6F4)
                mRevPKMFont.Add(&HB978, &H6F5)
                mRevPKMFont.Add(&HB97C, &H6F6)
                mRevPKMFont.Add(&HB984, &H6F7)
                mRevPKMFont.Add(&HB985, &H6F8)
                mRevPKMFont.Add(&HB987, &H6F9)
                mRevPKMFont.Add(&HB989, &H6FA)
                mRevPKMFont.Add(&HB98A, &H6FB)
                mRevPKMFont.Add(&HB98D, &H6FC)
                mRevPKMFont.Add(&HB98E, &H6FD)
                mRevPKMFont.Add(&HB9AC, &H6FE)
                mRevPKMFont.Add(&HB9AD, &H6FF)
                mRevPKMFont.Add(&HB9B0, &H700)
                mRevPKMFont.Add(&HB9B4, &H701)
                mRevPKMFont.Add(&HB9BC, &H702)
                mRevPKMFont.Add(&HB9BD, &H703)
                mRevPKMFont.Add(&HB9BF, &H704)
                mRevPKMFont.Add(&HB9C1, &H705)
                mRevPKMFont.Add(&HB9C8, &H706)
                mRevPKMFont.Add(&HB9C9, &H707)
                mRevPKMFont.Add(&HB9CC, &H708)
                mRevPKMFont.Add(&HB9CE, &H709)
                mRevPKMFont.Add(&HB9CF, &H70A)
                mRevPKMFont.Add(&HB9D0, &H70B)
                mRevPKMFont.Add(&HB9D1, &H70C)
                mRevPKMFont.Add(&HB9D2, &H70D)
                mRevPKMFont.Add(&HB9D8, &H70E)
                mRevPKMFont.Add(&HB9D9, &H70F)
                mRevPKMFont.Add(&HB9DB, &H710)
                mRevPKMFont.Add(&HB9DD, &H711)
                mRevPKMFont.Add(&HB9DE, &H712)
                mRevPKMFont.Add(&HB9E1, &H713)
                mRevPKMFont.Add(&HB9E3, &H714)
                mRevPKMFont.Add(&HB9E4, &H715)
                mRevPKMFont.Add(&HB9E5, &H716)
                mRevPKMFont.Add(&HB9E8, &H717)
                mRevPKMFont.Add(&HB9EC, &H718)
                mRevPKMFont.Add(&HB9F4, &H719)
                mRevPKMFont.Add(&HB9F5, &H71A)
                mRevPKMFont.Add(&HB9F7, &H71B)
                mRevPKMFont.Add(&HB9F8, &H71C)
                mRevPKMFont.Add(&HB9F9, &H71D)
                mRevPKMFont.Add(&HB9FA, &H71E)
                mRevPKMFont.Add(&HBA00, &H71F)
                mRevPKMFont.Add(&HBA01, &H720)
                mRevPKMFont.Add(&HBA08, &H721)
                mRevPKMFont.Add(&HBA15, &H722)
                mRevPKMFont.Add(&HBA38, &H723)
                mRevPKMFont.Add(&HBA39, &H724)
                mRevPKMFont.Add(&HBA3C, &H725)
                mRevPKMFont.Add(&HBA40, &H726)
                mRevPKMFont.Add(&HBA42, &H727)
                mRevPKMFont.Add(&HBA48, &H728)
                mRevPKMFont.Add(&HBA49, &H729)
                mRevPKMFont.Add(&HBA4B, &H72A)
                mRevPKMFont.Add(&HBA4D, &H72B)
                mRevPKMFont.Add(&HBA4E, &H72C)
                mRevPKMFont.Add(&HBA53, &H72D)
                mRevPKMFont.Add(&HBA54, &H72E)
                mRevPKMFont.Add(&HBA55, &H72F)
                mRevPKMFont.Add(&HBA58, &H730)
                mRevPKMFont.Add(&HBA5C, &H731)
                mRevPKMFont.Add(&HBA64, &H732)
                mRevPKMFont.Add(&HBA65, &H733)
                mRevPKMFont.Add(&HBA67, &H734)
                mRevPKMFont.Add(&HBA68, &H735)
                mRevPKMFont.Add(&HBA69, &H736)
                mRevPKMFont.Add(&HBA70, &H737)
                mRevPKMFont.Add(&HBA71, &H738)
                mRevPKMFont.Add(&HBA74, &H739)
                mRevPKMFont.Add(&HBA78, &H73A)
                mRevPKMFont.Add(&HBA83, &H73B)
                mRevPKMFont.Add(&HBA84, &H73C)
                mRevPKMFont.Add(&HBA85, &H73D)
                mRevPKMFont.Add(&HBA87, &H73E)
                mRevPKMFont.Add(&HBA8C, &H73F)
                mRevPKMFont.Add(&HBAA8, &H740)
                mRevPKMFont.Add(&HBAA9, &H741)
                mRevPKMFont.Add(&HBAAB, &H742)
                mRevPKMFont.Add(&HBAAC, &H743)
                mRevPKMFont.Add(&HBAB0, &H744)
                mRevPKMFont.Add(&HBAB2, &H745)
                mRevPKMFont.Add(&HBAB8, &H746)
                mRevPKMFont.Add(&HBAB9, &H747)
                mRevPKMFont.Add(&HBABB, &H748)
                mRevPKMFont.Add(&HBABD, &H749)
                mRevPKMFont.Add(&HBAC4, &H74A)
                mRevPKMFont.Add(&HBAC8, &H74B)
                mRevPKMFont.Add(&HBAD8, &H74C)
                mRevPKMFont.Add(&HBAD9, &H74D)
                mRevPKMFont.Add(&HBAFC, &H74E)
                mRevPKMFont.Add(&HBB00, &H74F)
                mRevPKMFont.Add(&HBB04, &H750)
                mRevPKMFont.Add(&HBB0D, &H751)
                mRevPKMFont.Add(&HBB0F, &H752)
                mRevPKMFont.Add(&HBB11, &H753)
                mRevPKMFont.Add(&HBB18, &H754)
                mRevPKMFont.Add(&HBB1C, &H755)
                mRevPKMFont.Add(&HBB20, &H756)
                mRevPKMFont.Add(&HBB29, &H757)
                mRevPKMFont.Add(&HBB2B, &H758)
                mRevPKMFont.Add(&HBB34, &H759)
                mRevPKMFont.Add(&HBB35, &H75A)
                mRevPKMFont.Add(&HBB36, &H75B)
                mRevPKMFont.Add(&HBB38, &H75C)
                mRevPKMFont.Add(&HBB3B, &H75D)
                mRevPKMFont.Add(&HBB3C, &H75E)
                mRevPKMFont.Add(&HBB3D, &H75F)
                mRevPKMFont.Add(&HBB3E, &H760)
                mRevPKMFont.Add(&HBB44, &H761)
                mRevPKMFont.Add(&HBB45, &H762)
                mRevPKMFont.Add(&HBB47, &H763)
                mRevPKMFont.Add(&HBB49, &H764)
                mRevPKMFont.Add(&HBB4D, &H765)
                mRevPKMFont.Add(&HBB4F, &H766)
                mRevPKMFont.Add(&HBB50, &H767)
                mRevPKMFont.Add(&HBB54, &H768)
                mRevPKMFont.Add(&HBB58, &H769)
                mRevPKMFont.Add(&HBB61, &H76A)
                mRevPKMFont.Add(&HBB63, &H76B)
                mRevPKMFont.Add(&HBB6C, &H76C)
                mRevPKMFont.Add(&HBB88, &H76D)
                mRevPKMFont.Add(&HBB8C, &H76E)
                mRevPKMFont.Add(&HBB90, &H76F)
                mRevPKMFont.Add(&HBBA4, &H770)
                mRevPKMFont.Add(&HBBA8, &H771)
                mRevPKMFont.Add(&HBBAC, &H772)
                mRevPKMFont.Add(&HBBB4, &H773)
                mRevPKMFont.Add(&HBBB7, &H774)
                mRevPKMFont.Add(&HBBC0, &H775)
                mRevPKMFont.Add(&HBBC4, &H776)
                mRevPKMFont.Add(&HBBC8, &H777)
                mRevPKMFont.Add(&HBBD0, &H778)
                mRevPKMFont.Add(&HBBD3, &H779)
                mRevPKMFont.Add(&HBBF8, &H77A)
                mRevPKMFont.Add(&HBBF9, &H77B)
                mRevPKMFont.Add(&HBBFC, &H77C)
                mRevPKMFont.Add(&HBBFF, &H77D)
                mRevPKMFont.Add(&HBC00, &H77E)
                mRevPKMFont.Add(&HBC02, &H77F)
                mRevPKMFont.Add(&HBC08, &H780)
                mRevPKMFont.Add(&HBC09, &H781)
                mRevPKMFont.Add(&HBC0B, &H782)
                mRevPKMFont.Add(&HBC0C, &H783)
                mRevPKMFont.Add(&HBC0D, &H784)
                mRevPKMFont.Add(&HBC0F, &H785)
                mRevPKMFont.Add(&HBC11, &H786)
                mRevPKMFont.Add(&HBC14, &H787)
                mRevPKMFont.Add(&HBC15, &H788)
                mRevPKMFont.Add(&HBC16, &H789)
                mRevPKMFont.Add(&HBC17, &H78A)
                mRevPKMFont.Add(&HBC18, &H78B)
                mRevPKMFont.Add(&HBC1B, &H78C)
                mRevPKMFont.Add(&HBC1C, &H78D)
                mRevPKMFont.Add(&HBC1D, &H78E)
                mRevPKMFont.Add(&HBC1E, &H78F)
                mRevPKMFont.Add(&HBC1F, &H790)
                mRevPKMFont.Add(&HBC24, &H791)
                mRevPKMFont.Add(&HBC25, &H792)
                mRevPKMFont.Add(&HBC27, &H793)
                mRevPKMFont.Add(&HBC29, &H794)
                mRevPKMFont.Add(&HBC2D, &H795)
                mRevPKMFont.Add(&HBC30, &H796)
                mRevPKMFont.Add(&HBC31, &H797)
                mRevPKMFont.Add(&HBC34, &H798)
                mRevPKMFont.Add(&HBC38, &H799)
                mRevPKMFont.Add(&HBC40, &H79A)
                mRevPKMFont.Add(&HBC41, &H79B)
                mRevPKMFont.Add(&HBC43, &H79C)
                mRevPKMFont.Add(&HBC44, &H79D)
                mRevPKMFont.Add(&HBC45, &H79E)
                mRevPKMFont.Add(&HBC49, &H79F)
                mRevPKMFont.Add(&HBC4C, &H7A0)
                mRevPKMFont.Add(&HBC4D, &H7A1)
                mRevPKMFont.Add(&HBC50, &H7A2)
                mRevPKMFont.Add(&HBC5D, &H7A3)
                mRevPKMFont.Add(&HBC84, &H7A4)
                mRevPKMFont.Add(&HBC85, &H7A5)
                mRevPKMFont.Add(&HBC88, &H7A6)
                mRevPKMFont.Add(&HBC8B, &H7A7)
                mRevPKMFont.Add(&HBC8C, &H7A8)
                mRevPKMFont.Add(&HBC8E, &H7A9)
                mRevPKMFont.Add(&HBC94, &H7AA)
                mRevPKMFont.Add(&HBC95, &H7AB)
                mRevPKMFont.Add(&HBC97, &H7AC)
                mRevPKMFont.Add(&HBC99, &H7AD)
                mRevPKMFont.Add(&HBC9A, &H7AE)
                mRevPKMFont.Add(&HBCA0, &H7AF)
                mRevPKMFont.Add(&HBCA1, &H7B0)
                mRevPKMFont.Add(&HBCA4, &H7B1)
                mRevPKMFont.Add(&HBCA7, &H7B2)
                mRevPKMFont.Add(&HBCA8, &H7B3)
                mRevPKMFont.Add(&HBCB0, &H7B4)
                mRevPKMFont.Add(&HBCB1, &H7B5)
                mRevPKMFont.Add(&HBCB3, &H7B6)
                mRevPKMFont.Add(&HBCB4, &H7B7)
                mRevPKMFont.Add(&HBCB5, &H7B8)
                mRevPKMFont.Add(&HBCBC, &H7B9)
                mRevPKMFont.Add(&HBCBD, &H7BA)
                mRevPKMFont.Add(&HBCC0, &H7BB)
                mRevPKMFont.Add(&HBCC4, &H7BC)
                mRevPKMFont.Add(&HBCCD, &H7BD)
                mRevPKMFont.Add(&HBCCF, &H7BE)
                mRevPKMFont.Add(&HBCD0, &H7BF)
                mRevPKMFont.Add(&HBCD1, &H7C0)
                mRevPKMFont.Add(&HBCD5, &H7C1)
                mRevPKMFont.Add(&HBCD8, &H7C2)
                mRevPKMFont.Add(&HBCDC, &H7C3)
                mRevPKMFont.Add(&HBCF4, &H7C4)
                mRevPKMFont.Add(&HBCF5, &H7C5)
                mRevPKMFont.Add(&HBCF6, &H7C6)
                mRevPKMFont.Add(&HBCF8, &H7C7)
                mRevPKMFont.Add(&HBCFC, &H7C8)
                mRevPKMFont.Add(&HBD04, &H7C9)
                mRevPKMFont.Add(&HBD05, &H7CA)
                mRevPKMFont.Add(&HBD07, &H7CB)
                mRevPKMFont.Add(&HBD09, &H7CC)
                mRevPKMFont.Add(&HBD10, &H7CD)
                mRevPKMFont.Add(&HBD14, &H7CE)
                mRevPKMFont.Add(&HBD24, &H7CF)
                mRevPKMFont.Add(&HBD2C, &H7D0)
                mRevPKMFont.Add(&HBD40, &H7D1)
                mRevPKMFont.Add(&HBD48, &H7D2)
                mRevPKMFont.Add(&HBD49, &H7D3)
                mRevPKMFont.Add(&HBD4C, &H7D4)
                mRevPKMFont.Add(&HBD50, &H7D5)
                mRevPKMFont.Add(&HBD58, &H7D6)
                mRevPKMFont.Add(&HBD59, &H7D7)
                mRevPKMFont.Add(&HBD64, &H7D8)
                mRevPKMFont.Add(&HBD68, &H7D9)
                mRevPKMFont.Add(&HBD80, &H7DA)
                mRevPKMFont.Add(&HBD81, &H7DB)
                mRevPKMFont.Add(&HBD84, &H7DC)
                mRevPKMFont.Add(&HBD87, &H7DD)
                mRevPKMFont.Add(&HBD88, &H7DE)
                mRevPKMFont.Add(&HBD89, &H7DF)
                mRevPKMFont.Add(&HBD8A, &H7E0)
                mRevPKMFont.Add(&HBD90, &H7E1)
                mRevPKMFont.Add(&HBD91, &H7E2)
                mRevPKMFont.Add(&HBD93, &H7E3)
                mRevPKMFont.Add(&HBD95, &H7E4)
                mRevPKMFont.Add(&HBD99, &H7E5)
                mRevPKMFont.Add(&HBD9A, &H7E6)
                mRevPKMFont.Add(&HBD9C, &H7E7)
                mRevPKMFont.Add(&HBDA4, &H7E8)
                mRevPKMFont.Add(&HBDB0, &H7E9)
                mRevPKMFont.Add(&HBDB8, &H7EA)
                mRevPKMFont.Add(&HBDD4, &H7EB)
                mRevPKMFont.Add(&HBDD5, &H7EC)
                mRevPKMFont.Add(&HBDD8, &H7ED)
                mRevPKMFont.Add(&HBDDC, &H7EE)
                mRevPKMFont.Add(&HBDE9, &H7EF)
                mRevPKMFont.Add(&HBDF0, &H7F0)
                mRevPKMFont.Add(&HBDF4, &H7F1)
                mRevPKMFont.Add(&HBDF8, &H7F2)
                mRevPKMFont.Add(&HBE00, &H7F3)
                mRevPKMFont.Add(&HBE03, &H7F4)
                mRevPKMFont.Add(&HBE05, &H7F5)
                mRevPKMFont.Add(&HBE0C, &H7F6)
                mRevPKMFont.Add(&HBE0D, &H7F7)
                mRevPKMFont.Add(&HBE10, &H7F8)
                mRevPKMFont.Add(&HBE14, &H7F9)
                mRevPKMFont.Add(&HBE1C, &H7FA)
                mRevPKMFont.Add(&HBE1D, &H7FB)
                mRevPKMFont.Add(&HBE1F, &H7FC)
                mRevPKMFont.Add(&HBE44, &H7FD)
                mRevPKMFont.Add(&HBE45, &H7FE)
                mRevPKMFont.Add(&HBE48, &H7FF)
                mRevPKMFont.Add(&HBE4C, &H800)
                mRevPKMFont.Add(&HBE4E, &H801)
                mRevPKMFont.Add(&HBE54, &H802)
                mRevPKMFont.Add(&HBE55, &H803)
                mRevPKMFont.Add(&HBE57, &H804)
                mRevPKMFont.Add(&HBE59, &H805)
                mRevPKMFont.Add(&HBE5A, &H806)
                mRevPKMFont.Add(&HBE5B, &H807)
                mRevPKMFont.Add(&HBE60, &H808)
                mRevPKMFont.Add(&HBE61, &H809)
                mRevPKMFont.Add(&HBE64, &H80A)
                mRevPKMFont.Add(&HBE68, &H80B)
                mRevPKMFont.Add(&HBE6A, &H80C)
                mRevPKMFont.Add(&HBE70, &H80D)
                mRevPKMFont.Add(&HBE71, &H80E)
                mRevPKMFont.Add(&HBE73, &H80F)
                mRevPKMFont.Add(&HBE74, &H810)
                mRevPKMFont.Add(&HBE75, &H811)
                mRevPKMFont.Add(&HBE7B, &H812)
                mRevPKMFont.Add(&HBE7C, &H813)
                mRevPKMFont.Add(&HBE7D, &H814)
                mRevPKMFont.Add(&HBE80, &H815)
                mRevPKMFont.Add(&HBE84, &H816)
                mRevPKMFont.Add(&HBE8C, &H817)
                mRevPKMFont.Add(&HBE8D, &H818)
                mRevPKMFont.Add(&HBE8F, &H819)
                mRevPKMFont.Add(&HBE90, &H81A)
                mRevPKMFont.Add(&HBE91, &H81B)
                mRevPKMFont.Add(&HBE98, &H81C)
                mRevPKMFont.Add(&HBE99, &H81D)
                mRevPKMFont.Add(&HBEA8, &H81E)
                mRevPKMFont.Add(&HBED0, &H81F)
                mRevPKMFont.Add(&HBED1, &H820)
                mRevPKMFont.Add(&HBED4, &H821)
                mRevPKMFont.Add(&HBED7, &H822)
                mRevPKMFont.Add(&HBED8, &H823)
                mRevPKMFont.Add(&HBEE0, &H824)
                mRevPKMFont.Add(&HBEE3, &H825)
                mRevPKMFont.Add(&HBEE4, &H826)
                mRevPKMFont.Add(&HBEE5, &H827)
                mRevPKMFont.Add(&HBEEC, &H828)
                mRevPKMFont.Add(&HBF01, &H829)
                mRevPKMFont.Add(&HBF08, &H82A)
                mRevPKMFont.Add(&HBF09, &H82B)
                mRevPKMFont.Add(&HBF18, &H82C)
                mRevPKMFont.Add(&HBF19, &H82D)
                mRevPKMFont.Add(&HBF1B, &H82E)
                mRevPKMFont.Add(&HBF1C, &H82F)
                mRevPKMFont.Add(&HBF1D, &H830)
                mRevPKMFont.Add(&HBF40, &H831)
                mRevPKMFont.Add(&HBF41, &H832)
                mRevPKMFont.Add(&HBF44, &H833)
                mRevPKMFont.Add(&HBF48, &H834)
                mRevPKMFont.Add(&HBF50, &H835)
                mRevPKMFont.Add(&HBF51, &H836)
                mRevPKMFont.Add(&HBF55, &H837)
                mRevPKMFont.Add(&HBF94, &H838)
                mRevPKMFont.Add(&HBFB0, &H839)
                mRevPKMFont.Add(&HBFC5, &H83A)
                mRevPKMFont.Add(&HBFCC, &H83B)
                mRevPKMFont.Add(&HBFCD, &H83C)
                mRevPKMFont.Add(&HBFD0, &H83D)
                mRevPKMFont.Add(&HBFD4, &H83E)
                mRevPKMFont.Add(&HBFDC, &H83F)
                mRevPKMFont.Add(&HBFDF, &H840)
                mRevPKMFont.Add(&HBFE1, &H841)
                mRevPKMFont.Add(&HC03C, &H842)
                mRevPKMFont.Add(&HC051, &H843)
                mRevPKMFont.Add(&HC058, &H844)
                mRevPKMFont.Add(&HC05C, &H845)
                mRevPKMFont.Add(&HC060, &H846)
                mRevPKMFont.Add(&HC068, &H847)
                mRevPKMFont.Add(&HC069, &H848)
                mRevPKMFont.Add(&HC090, &H849)
                mRevPKMFont.Add(&HC091, &H84A)
                mRevPKMFont.Add(&HC094, &H84B)
                mRevPKMFont.Add(&HC098, &H84C)
                mRevPKMFont.Add(&HC0A0, &H84D)
                mRevPKMFont.Add(&HC0A1, &H84E)
                mRevPKMFont.Add(&HC0A3, &H84F)
                mRevPKMFont.Add(&HC0A5, &H850)
                mRevPKMFont.Add(&HC0AC, &H851)
                mRevPKMFont.Add(&HC0AD, &H852)
                mRevPKMFont.Add(&HC0AF, &H853)
                mRevPKMFont.Add(&HC0B0, &H854)
                mRevPKMFont.Add(&HC0B3, &H855)
                mRevPKMFont.Add(&HC0B4, &H856)
                mRevPKMFont.Add(&HC0B5, &H857)
                mRevPKMFont.Add(&HC0B6, &H858)
                mRevPKMFont.Add(&HC0BC, &H859)
                mRevPKMFont.Add(&HC0BD, &H85A)
                mRevPKMFont.Add(&HC0BF, &H85B)
                mRevPKMFont.Add(&HC0C0, &H85C)
                mRevPKMFont.Add(&HC0C1, &H85D)
                mRevPKMFont.Add(&HC0C5, &H85E)
                mRevPKMFont.Add(&HC0C8, &H85F)
                mRevPKMFont.Add(&HC0C9, &H860)
                mRevPKMFont.Add(&HC0CC, &H861)
                mRevPKMFont.Add(&HC0D0, &H862)
                mRevPKMFont.Add(&HC0D8, &H863)
                mRevPKMFont.Add(&HC0D9, &H864)
                mRevPKMFont.Add(&HC0DB, &H865)
                mRevPKMFont.Add(&HC0DC, &H866)
                mRevPKMFont.Add(&HC0DD, &H867)
                mRevPKMFont.Add(&HC0E4, &H868)
                mRevPKMFont.Add(&HC0E5, &H869)
                mRevPKMFont.Add(&HC0E8, &H86A)
                mRevPKMFont.Add(&HC0EC, &H86B)
                mRevPKMFont.Add(&HC0F4, &H86C)
                mRevPKMFont.Add(&HC0F5, &H86D)
                mRevPKMFont.Add(&HC0F7, &H86E)
                mRevPKMFont.Add(&HC0F9, &H86F)
                mRevPKMFont.Add(&HC100, &H870)
                mRevPKMFont.Add(&HC104, &H871)
                mRevPKMFont.Add(&HC108, &H872)
                mRevPKMFont.Add(&HC110, &H873)
                mRevPKMFont.Add(&HC115, &H874)
                mRevPKMFont.Add(&HC11C, &H875)
                mRevPKMFont.Add(&HC11D, &H876)
                mRevPKMFont.Add(&HC11E, &H877)
                mRevPKMFont.Add(&HC11F, &H878)
                mRevPKMFont.Add(&HC120, &H879)
                mRevPKMFont.Add(&HC123, &H87A)
                mRevPKMFont.Add(&HC124, &H87B)
                mRevPKMFont.Add(&HC126, &H87C)
                mRevPKMFont.Add(&HC127, &H87D)
                mRevPKMFont.Add(&HC12C, &H87E)
                mRevPKMFont.Add(&HC12D, &H87F)
                mRevPKMFont.Add(&HC12F, &H880)
                mRevPKMFont.Add(&HC130, &H881)
                mRevPKMFont.Add(&HC131, &H882)
                mRevPKMFont.Add(&HC136, &H883)
                mRevPKMFont.Add(&HC138, &H884)
                mRevPKMFont.Add(&HC139, &H885)
                mRevPKMFont.Add(&HC13C, &H886)
                mRevPKMFont.Add(&HC140, &H887)
                mRevPKMFont.Add(&HC148, &H888)
                mRevPKMFont.Add(&HC149, &H889)
                mRevPKMFont.Add(&HC14B, &H88A)
                mRevPKMFont.Add(&HC14C, &H88B)
                mRevPKMFont.Add(&HC14D, &H88C)
                mRevPKMFont.Add(&HC154, &H88D)
                mRevPKMFont.Add(&HC155, &H88E)
                mRevPKMFont.Add(&HC158, &H88F)
                mRevPKMFont.Add(&HC15C, &H890)
                mRevPKMFont.Add(&HC164, &H891)
                mRevPKMFont.Add(&HC165, &H892)
                mRevPKMFont.Add(&HC167, &H893)
                mRevPKMFont.Add(&HC168, &H894)
                mRevPKMFont.Add(&HC169, &H895)
                mRevPKMFont.Add(&HC170, &H896)
                mRevPKMFont.Add(&HC174, &H897)
                mRevPKMFont.Add(&HC178, &H898)
                mRevPKMFont.Add(&HC185, &H899)
                mRevPKMFont.Add(&HC18C, &H89A)
                mRevPKMFont.Add(&HC18D, &H89B)
                mRevPKMFont.Add(&HC18E, &H89C)
                mRevPKMFont.Add(&HC190, &H89D)
                mRevPKMFont.Add(&HC194, &H89E)
                mRevPKMFont.Add(&HC196, &H89F)
                mRevPKMFont.Add(&HC19C, &H8A0)
                mRevPKMFont.Add(&HC19D, &H8A1)
                mRevPKMFont.Add(&HC19F, &H8A2)
                mRevPKMFont.Add(&HC1A1, &H8A3)
                mRevPKMFont.Add(&HC1A5, &H8A4)
                mRevPKMFont.Add(&HC1A8, &H8A5)
                mRevPKMFont.Add(&HC1A9, &H8A6)
                mRevPKMFont.Add(&HC1AC, &H8A7)
                mRevPKMFont.Add(&HC1B0, &H8A8)
                mRevPKMFont.Add(&HC1BD, &H8A9)
                mRevPKMFont.Add(&HC1C4, &H8AA)
                mRevPKMFont.Add(&HC1C8, &H8AB)
                mRevPKMFont.Add(&HC1CC, &H8AC)
                mRevPKMFont.Add(&HC1D4, &H8AD)
                mRevPKMFont.Add(&HC1D7, &H8AE)
                mRevPKMFont.Add(&HC1D8, &H8AF)
                mRevPKMFont.Add(&HC1E0, &H8B0)
                mRevPKMFont.Add(&HC1E4, &H8B1)
                mRevPKMFont.Add(&HC1E8, &H8B2)
                mRevPKMFont.Add(&HC1F0, &H8B3)
                mRevPKMFont.Add(&HC1F1, &H8B4)
                mRevPKMFont.Add(&HC1F3, &H8B5)
                mRevPKMFont.Add(&HC1FC, &H8B6)
                mRevPKMFont.Add(&HC1FD, &H8B7)
                mRevPKMFont.Add(&HC200, &H8B8)
                mRevPKMFont.Add(&HC204, &H8B9)
                mRevPKMFont.Add(&HC20C, &H8BA)
                mRevPKMFont.Add(&HC20D, &H8BB)
                mRevPKMFont.Add(&HC20F, &H8BC)
                mRevPKMFont.Add(&HC211, &H8BD)
                mRevPKMFont.Add(&HC218, &H8BE)
                mRevPKMFont.Add(&HC219, &H8BF)
                mRevPKMFont.Add(&HC21C, &H8C0)
                mRevPKMFont.Add(&HC21F, &H8C1)
                mRevPKMFont.Add(&HC220, &H8C2)
                mRevPKMFont.Add(&HC228, &H8C3)
                mRevPKMFont.Add(&HC229, &H8C4)
                mRevPKMFont.Add(&HC22B, &H8C5)
                mRevPKMFont.Add(&HC22D, &H8C6)
                mRevPKMFont.Add(&HC22F, &H8C7)
                mRevPKMFont.Add(&HC231, &H8C8)
                mRevPKMFont.Add(&HC232, &H8C9)
                mRevPKMFont.Add(&HC234, &H8CA)
                mRevPKMFont.Add(&HC248, &H8CB)
                mRevPKMFont.Add(&HC250, &H8CC)
                mRevPKMFont.Add(&HC251, &H8CD)
                mRevPKMFont.Add(&HC254, &H8CE)
                mRevPKMFont.Add(&HC258, &H8CF)
                mRevPKMFont.Add(&HC260, &H8D0)
                mRevPKMFont.Add(&HC265, &H8D1)
                mRevPKMFont.Add(&HC26C, &H8D2)
                mRevPKMFont.Add(&HC26D, &H8D3)
                mRevPKMFont.Add(&HC270, &H8D4)
                mRevPKMFont.Add(&HC274, &H8D5)
                mRevPKMFont.Add(&HC27C, &H8D6)
                mRevPKMFont.Add(&HC27D, &H8D7)
                mRevPKMFont.Add(&HC27F, &H8D8)
                mRevPKMFont.Add(&HC281, &H8D9)
                mRevPKMFont.Add(&HC288, &H8DA)
                mRevPKMFont.Add(&HC289, &H8DB)
                mRevPKMFont.Add(&HC290, &H8DC)
                mRevPKMFont.Add(&HC298, &H8DD)
                mRevPKMFont.Add(&HC29B, &H8DE)
                mRevPKMFont.Add(&HC29D, &H8DF)
                mRevPKMFont.Add(&HC2A4, &H8E0)
                mRevPKMFont.Add(&HC2A5, &H8E1)
                mRevPKMFont.Add(&HC2A8, &H8E2)
                mRevPKMFont.Add(&HC2AC, &H8E3)
                mRevPKMFont.Add(&HC2AD, &H8E4)
                mRevPKMFont.Add(&HC2B4, &H8E5)
                mRevPKMFont.Add(&HC2B5, &H8E6)
                mRevPKMFont.Add(&HC2B7, &H8E7)
                mRevPKMFont.Add(&HC2B9, &H8E8)
                mRevPKMFont.Add(&HC2DC, &H8E9)
                mRevPKMFont.Add(&HC2DD, &H8EA)
                mRevPKMFont.Add(&HC2E0, &H8EB)
                mRevPKMFont.Add(&HC2E3, &H8EC)
                mRevPKMFont.Add(&HC2E4, &H8ED)
                mRevPKMFont.Add(&HC2EB, &H8EE)
                mRevPKMFont.Add(&HC2EC, &H8EF)
                mRevPKMFont.Add(&HC2ED, &H8F0)
                mRevPKMFont.Add(&HC2EF, &H8F1)
                mRevPKMFont.Add(&HC2F1, &H8F2)
                mRevPKMFont.Add(&HC2F6, &H8F3)
                mRevPKMFont.Add(&HC2F8, &H8F4)
                mRevPKMFont.Add(&HC2F9, &H8F5)
                mRevPKMFont.Add(&HC2FB, &H8F6)
                mRevPKMFont.Add(&HC2FC, &H8F7)
                mRevPKMFont.Add(&HC300, &H8F8)
                mRevPKMFont.Add(&HC308, &H8F9)
                mRevPKMFont.Add(&HC309, &H8FA)
                mRevPKMFont.Add(&HC30C, &H8FB)
                mRevPKMFont.Add(&HC30D, &H8FC)
                mRevPKMFont.Add(&HC313, &H8FD)
                mRevPKMFont.Add(&HC314, &H8FE)
                mRevPKMFont.Add(&HC315, &H8FF)
                mRevPKMFont.Add(&HC318, &H900)
                mRevPKMFont.Add(&HC31C, &H901)
                mRevPKMFont.Add(&HC324, &H902)
                mRevPKMFont.Add(&HC325, &H903)
                mRevPKMFont.Add(&HC328, &H904)
                mRevPKMFont.Add(&HC329, &H905)
                mRevPKMFont.Add(&HC345, &H906)
                mRevPKMFont.Add(&HC368, &H907)
                mRevPKMFont.Add(&HC369, &H908)
                mRevPKMFont.Add(&HC36C, &H909)
                mRevPKMFont.Add(&HC370, &H90A)
                mRevPKMFont.Add(&HC372, &H90B)
                mRevPKMFont.Add(&HC378, &H90C)
                mRevPKMFont.Add(&HC379, &H90D)
                mRevPKMFont.Add(&HC37C, &H90E)
                mRevPKMFont.Add(&HC37D, &H90F)
                mRevPKMFont.Add(&HC384, &H910)
                mRevPKMFont.Add(&HC388, &H911)
                mRevPKMFont.Add(&HC38C, &H912)
                mRevPKMFont.Add(&HC3C0, &H913)
                mRevPKMFont.Add(&HC3D8, &H914)
                mRevPKMFont.Add(&HC3D9, &H915)
                mRevPKMFont.Add(&HC3DC, &H916)
                mRevPKMFont.Add(&HC3DF, &H917)
                mRevPKMFont.Add(&HC3E0, &H918)
                mRevPKMFont.Add(&HC3E2, &H919)
                mRevPKMFont.Add(&HC3E8, &H91A)
                mRevPKMFont.Add(&HC3E9, &H91B)
                mRevPKMFont.Add(&HC3ED, &H91C)
                mRevPKMFont.Add(&HC3F4, &H91D)
                mRevPKMFont.Add(&HC3F5, &H91E)
                mRevPKMFont.Add(&HC3F8, &H91F)
                mRevPKMFont.Add(&HC408, &H920)
                mRevPKMFont.Add(&HC410, &H921)
                mRevPKMFont.Add(&HC424, &H922)
                mRevPKMFont.Add(&HC42C, &H923)
                mRevPKMFont.Add(&HC430, &H924)
                mRevPKMFont.Add(&HC434, &H925)
                mRevPKMFont.Add(&HC43C, &H926)
                mRevPKMFont.Add(&HC43D, &H927)
                mRevPKMFont.Add(&HC448, &H928)
                mRevPKMFont.Add(&HC464, &H929)
                mRevPKMFont.Add(&HC465, &H92A)
                mRevPKMFont.Add(&HC468, &H92B)
                mRevPKMFont.Add(&HC46C, &H92C)
                mRevPKMFont.Add(&HC474, &H92D)
                mRevPKMFont.Add(&HC475, &H92E)
                mRevPKMFont.Add(&HC479, &H92F)
                mRevPKMFont.Add(&HC480, &H930)
                mRevPKMFont.Add(&HC494, &H931)
                mRevPKMFont.Add(&HC49C, &H932)
                mRevPKMFont.Add(&HC4B8, &H933)
                mRevPKMFont.Add(&HC4BC, &H934)
                mRevPKMFont.Add(&HC4E9, &H935)
                mRevPKMFont.Add(&HC4F0, &H936)
                mRevPKMFont.Add(&HC4F1, &H937)
                mRevPKMFont.Add(&HC4F4, &H938)
                mRevPKMFont.Add(&HC4F8, &H939)
                mRevPKMFont.Add(&HC4FA, &H93A)
                mRevPKMFont.Add(&HC4FF, &H93B)
                mRevPKMFont.Add(&HC500, &H93C)
                mRevPKMFont.Add(&HC501, &H93D)
                mRevPKMFont.Add(&HC50C, &H93E)
                mRevPKMFont.Add(&HC510, &H93F)
                mRevPKMFont.Add(&HC514, &H940)
                mRevPKMFont.Add(&HC51C, &H941)
                mRevPKMFont.Add(&HC528, &H942)
                mRevPKMFont.Add(&HC529, &H943)
                mRevPKMFont.Add(&HC52C, &H944)
                mRevPKMFont.Add(&HC530, &H945)
                mRevPKMFont.Add(&HC538, &H946)
                mRevPKMFont.Add(&HC539, &H947)
                mRevPKMFont.Add(&HC53B, &H948)
                mRevPKMFont.Add(&HC53D, &H949)
                mRevPKMFont.Add(&HC544, &H94A)
                mRevPKMFont.Add(&HC545, &H94B)
                mRevPKMFont.Add(&HC548, &H94C)
                mRevPKMFont.Add(&HC549, &H94D)
                mRevPKMFont.Add(&HC54A, &H94E)
                mRevPKMFont.Add(&HC54C, &H94F)
                mRevPKMFont.Add(&HC54D, &H950)
                mRevPKMFont.Add(&HC54E, &H951)
                mRevPKMFont.Add(&HC553, &H952)
                mRevPKMFont.Add(&HC554, &H953)
                mRevPKMFont.Add(&HC555, &H954)
                mRevPKMFont.Add(&HC557, &H955)
                mRevPKMFont.Add(&HC558, &H956)
                mRevPKMFont.Add(&HC559, &H957)
                mRevPKMFont.Add(&HC55D, &H958)
                mRevPKMFont.Add(&HC55E, &H959)
                mRevPKMFont.Add(&HC560, &H95A)
                mRevPKMFont.Add(&HC561, &H95B)
                mRevPKMFont.Add(&HC564, &H95C)
                mRevPKMFont.Add(&HC568, &H95D)
                mRevPKMFont.Add(&HC570, &H95E)
                mRevPKMFont.Add(&HC571, &H95F)
                mRevPKMFont.Add(&HC573, &H960)
                mRevPKMFont.Add(&HC574, &H961)
                mRevPKMFont.Add(&HC575, &H962)
                mRevPKMFont.Add(&HC57C, &H963)
                mRevPKMFont.Add(&HC57D, &H964)
                mRevPKMFont.Add(&HC580, &H965)
                mRevPKMFont.Add(&HC584, &H966)
                mRevPKMFont.Add(&HC587, &H967)
                mRevPKMFont.Add(&HC58C, &H968)
                mRevPKMFont.Add(&HC58D, &H969)
                mRevPKMFont.Add(&HC58F, &H96A)
                mRevPKMFont.Add(&HC591, &H96B)
                mRevPKMFont.Add(&HC595, &H96C)
                mRevPKMFont.Add(&HC597, &H96D)
                mRevPKMFont.Add(&HC598, &H96E)
                mRevPKMFont.Add(&HC59C, &H96F)
                mRevPKMFont.Add(&HC5A0, &H970)
                mRevPKMFont.Add(&HC5A9, &H971)
                mRevPKMFont.Add(&HC5B4, &H972)
                mRevPKMFont.Add(&HC5B5, &H973)
                mRevPKMFont.Add(&HC5B8, &H974)
                mRevPKMFont.Add(&HC5B9, &H975)
                mRevPKMFont.Add(&HC5BB, &H976)
                mRevPKMFont.Add(&HC5BC, &H977)
                mRevPKMFont.Add(&HC5BD, &H978)
                mRevPKMFont.Add(&HC5BE, &H979)
                mRevPKMFont.Add(&HC5C4, &H97A)
                mRevPKMFont.Add(&HC5C5, &H97B)
                mRevPKMFont.Add(&HC5C6, &H97C)
                mRevPKMFont.Add(&HC5C7, &H97D)
                mRevPKMFont.Add(&HC5C8, &H97E)
                mRevPKMFont.Add(&HC5C9, &H97F)
                mRevPKMFont.Add(&HC5CA, &H980)
                mRevPKMFont.Add(&HC5CC, &H981)
                mRevPKMFont.Add(&HC5CE, &H982)
                mRevPKMFont.Add(&HC5D0, &H983)
                mRevPKMFont.Add(&HC5D1, &H984)
                mRevPKMFont.Add(&HC5D4, &H985)
                mRevPKMFont.Add(&HC5D8, &H986)
                mRevPKMFont.Add(&HC5E0, &H987)
                mRevPKMFont.Add(&HC5E1, &H988)
                mRevPKMFont.Add(&HC5E3, &H989)
                mRevPKMFont.Add(&HC5E5, &H98A)
                mRevPKMFont.Add(&HC5EC, &H98B)
                mRevPKMFont.Add(&HC5ED, &H98C)
                mRevPKMFont.Add(&HC5EE, &H98D)
                mRevPKMFont.Add(&HC5F0, &H98E)
                mRevPKMFont.Add(&HC5F4, &H98F)
                mRevPKMFont.Add(&HC5F6, &H990)
                mRevPKMFont.Add(&HC5F7, &H991)
                mRevPKMFont.Add(&HC5FC, &H992)
                mRevPKMFont.Add(&HC5FD, &H993)
                mRevPKMFont.Add(&HC5FE, &H994)
                mRevPKMFont.Add(&HC5FF, &H995)
                mRevPKMFont.Add(&HC600, &H996)
                mRevPKMFont.Add(&HC601, &H997)
                mRevPKMFont.Add(&HC605, &H998)
                mRevPKMFont.Add(&HC606, &H999)
                mRevPKMFont.Add(&HC607, &H99A)
                mRevPKMFont.Add(&HC608, &H99B)
                mRevPKMFont.Add(&HC60C, &H99C)
                mRevPKMFont.Add(&HC610, &H99D)
                mRevPKMFont.Add(&HC618, &H99E)
                mRevPKMFont.Add(&HC619, &H99F)
                mRevPKMFont.Add(&HC61B, &H9A0)
                mRevPKMFont.Add(&HC61C, &H9A1)
                mRevPKMFont.Add(&HC624, &H9A2)
                mRevPKMFont.Add(&HC625, &H9A3)
                mRevPKMFont.Add(&HC628, &H9A4)
                mRevPKMFont.Add(&HC62C, &H9A5)
                mRevPKMFont.Add(&HC62D, &H9A6)
                mRevPKMFont.Add(&HC62E, &H9A7)
                mRevPKMFont.Add(&HC630, &H9A8)
                mRevPKMFont.Add(&HC633, &H9A9)
                mRevPKMFont.Add(&HC634, &H9AA)
                mRevPKMFont.Add(&HC635, &H9AB)
                mRevPKMFont.Add(&HC637, &H9AC)
                mRevPKMFont.Add(&HC639, &H9AD)
                mRevPKMFont.Add(&HC63B, &H9AE)
                mRevPKMFont.Add(&HC640, &H9AF)
                mRevPKMFont.Add(&HC641, &H9B0)
                mRevPKMFont.Add(&HC644, &H9B1)
                mRevPKMFont.Add(&HC648, &H9B2)
                mRevPKMFont.Add(&HC650, &H9B3)
                mRevPKMFont.Add(&HC651, &H9B4)
                mRevPKMFont.Add(&HC653, &H9B5)
                mRevPKMFont.Add(&HC654, &H9B6)
                mRevPKMFont.Add(&HC655, &H9B7)
                mRevPKMFont.Add(&HC65C, &H9B8)
                mRevPKMFont.Add(&HC65D, &H9B9)
                mRevPKMFont.Add(&HC660, &H9BA)
                mRevPKMFont.Add(&HC66C, &H9BB)
                mRevPKMFont.Add(&HC66F, &H9BC)
                mRevPKMFont.Add(&HC671, &H9BD)
                mRevPKMFont.Add(&HC678, &H9BE)
                mRevPKMFont.Add(&HC679, &H9BF)
                mRevPKMFont.Add(&HC67C, &H9C0)
                mRevPKMFont.Add(&HC680, &H9C1)
                mRevPKMFont.Add(&HC688, &H9C2)
                mRevPKMFont.Add(&HC689, &H9C3)
                mRevPKMFont.Add(&HC68B, &H9C4)
                mRevPKMFont.Add(&HC68D, &H9C5)
                mRevPKMFont.Add(&HC694, &H9C6)
                mRevPKMFont.Add(&HC695, &H9C7)
                mRevPKMFont.Add(&HC698, &H9C8)
                mRevPKMFont.Add(&HC69C, &H9C9)
                mRevPKMFont.Add(&HC6A4, &H9CA)
                mRevPKMFont.Add(&HC6A5, &H9CB)
                mRevPKMFont.Add(&HC6A7, &H9CC)
                mRevPKMFont.Add(&HC6A9, &H9CD)
                mRevPKMFont.Add(&HC6B0, &H9CE)
                mRevPKMFont.Add(&HC6B1, &H9CF)
                mRevPKMFont.Add(&HC6B4, &H9D0)
                mRevPKMFont.Add(&HC6B8, &H9D1)
                mRevPKMFont.Add(&HC6B9, &H9D2)
                mRevPKMFont.Add(&HC6BA, &H9D3)
                mRevPKMFont.Add(&HC6C0, &H9D4)
                mRevPKMFont.Add(&HC6C1, &H9D5)
                mRevPKMFont.Add(&HC6C3, &H9D6)
                mRevPKMFont.Add(&HC6C5, &H9D7)
                mRevPKMFont.Add(&HC6CC, &H9D8)
                mRevPKMFont.Add(&HC6CD, &H9D9)
                mRevPKMFont.Add(&HC6D0, &H9DA)
                mRevPKMFont.Add(&HC6D4, &H9DB)
                mRevPKMFont.Add(&HC6DC, &H9DC)
                mRevPKMFont.Add(&HC6DD, &H9DD)
                mRevPKMFont.Add(&HC6E0, &H9DE)
                mRevPKMFont.Add(&HC6E1, &H9DF)
                mRevPKMFont.Add(&HC6E8, &H9E0)
                mRevPKMFont.Add(&HC6E9, &H9E1)
                mRevPKMFont.Add(&HC6EC, &H9E2)
                mRevPKMFont.Add(&HC6F0, &H9E3)
                mRevPKMFont.Add(&HC6F8, &H9E4)
                mRevPKMFont.Add(&HC6F9, &H9E5)
                mRevPKMFont.Add(&HC6FD, &H9E6)
                mRevPKMFont.Add(&HC704, &H9E7)
                mRevPKMFont.Add(&HC705, &H9E8)
                mRevPKMFont.Add(&HC708, &H9E9)
                mRevPKMFont.Add(&HC70C, &H9EA)
                mRevPKMFont.Add(&HC714, &H9EB)
                mRevPKMFont.Add(&HC715, &H9EC)
                mRevPKMFont.Add(&HC717, &H9ED)
                mRevPKMFont.Add(&HC719, &H9EE)
                mRevPKMFont.Add(&HC720, &H9EF)
                mRevPKMFont.Add(&HC721, &H9F0)
                mRevPKMFont.Add(&HC724, &H9F1)
                mRevPKMFont.Add(&HC728, &H9F2)
                mRevPKMFont.Add(&HC730, &H9F3)
                mRevPKMFont.Add(&HC731, &H9F4)
                mRevPKMFont.Add(&HC733, &H9F5)
                mRevPKMFont.Add(&HC735, &H9F6)
                mRevPKMFont.Add(&HC737, &H9F7)
                mRevPKMFont.Add(&HC73C, &H9F8)
                mRevPKMFont.Add(&HC73D, &H9F9)
                mRevPKMFont.Add(&HC740, &H9FA)
                mRevPKMFont.Add(&HC744, &H9FB)
                mRevPKMFont.Add(&HC74A, &H9FC)
                mRevPKMFont.Add(&HC74C, &H9FD)
                mRevPKMFont.Add(&HC74D, &H9FE)
                mRevPKMFont.Add(&HC74F, &H9FF)
                mRevPKMFont.Add(&HC751, &HA00)
                mRevPKMFont.Add(&HC752, &HA01)
                mRevPKMFont.Add(&HC753, &HA02)
                mRevPKMFont.Add(&HC754, &HA03)
                mRevPKMFont.Add(&HC755, &HA04)
                mRevPKMFont.Add(&HC756, &HA05)
                mRevPKMFont.Add(&HC757, &HA06)
                mRevPKMFont.Add(&HC758, &HA07)
                mRevPKMFont.Add(&HC75C, &HA08)
                mRevPKMFont.Add(&HC760, &HA09)
                mRevPKMFont.Add(&HC768, &HA0A)
                mRevPKMFont.Add(&HC76B, &HA0B)
                mRevPKMFont.Add(&HC774, &HA0C)
                mRevPKMFont.Add(&HC775, &HA0D)
                mRevPKMFont.Add(&HC778, &HA0E)
                mRevPKMFont.Add(&HC77C, &HA0F)
                mRevPKMFont.Add(&HC77D, &HA10)
                mRevPKMFont.Add(&HC77E, &HA11)
                mRevPKMFont.Add(&HC783, &HA12)
                mRevPKMFont.Add(&HC784, &HA13)
                mRevPKMFont.Add(&HC785, &HA14)
                mRevPKMFont.Add(&HC787, &HA15)
                mRevPKMFont.Add(&HC788, &HA16)
                mRevPKMFont.Add(&HC789, &HA17)
                mRevPKMFont.Add(&HC78A, &HA18)
                mRevPKMFont.Add(&HC78E, &HA19)
                mRevPKMFont.Add(&HC790, &HA1A)
                mRevPKMFont.Add(&HC791, &HA1B)
                mRevPKMFont.Add(&HC794, &HA1C)
                mRevPKMFont.Add(&HC796, &HA1D)
                mRevPKMFont.Add(&HC797, &HA1E)
                mRevPKMFont.Add(&HC798, &HA1F)
                mRevPKMFont.Add(&HC79A, &HA20)
                mRevPKMFont.Add(&HC7A0, &HA21)
                mRevPKMFont.Add(&HC7A1, &HA22)
                mRevPKMFont.Add(&HC7A3, &HA23)
                mRevPKMFont.Add(&HC7A4, &HA24)
                mRevPKMFont.Add(&HC7A5, &HA25)
                mRevPKMFont.Add(&HC7A6, &HA26)
                mRevPKMFont.Add(&HC7AC, &HA27)
                mRevPKMFont.Add(&HC7AD, &HA28)
                mRevPKMFont.Add(&HC7B0, &HA29)
                mRevPKMFont.Add(&HC7B4, &HA2A)
                mRevPKMFont.Add(&HC7BC, &HA2B)
                mRevPKMFont.Add(&HC7BD, &HA2C)
                mRevPKMFont.Add(&HC7BF, &HA2D)
                mRevPKMFont.Add(&HC7C0, &HA2E)
                mRevPKMFont.Add(&HC7C1, &HA2F)
                mRevPKMFont.Add(&HC7C8, &HA30)
                mRevPKMFont.Add(&HC7C9, &HA31)
                mRevPKMFont.Add(&HC7CC, &HA32)
                mRevPKMFont.Add(&HC7CE, &HA33)
                mRevPKMFont.Add(&HC7D0, &HA34)
                mRevPKMFont.Add(&HC7D8, &HA35)
                mRevPKMFont.Add(&HC7DD, &HA36)
                mRevPKMFont.Add(&HC7E4, &HA37)
                mRevPKMFont.Add(&HC7E8, &HA38)
                mRevPKMFont.Add(&HC7EC, &HA39)
                mRevPKMFont.Add(&HC800, &HA3A)
                mRevPKMFont.Add(&HC801, &HA3B)
                mRevPKMFont.Add(&HC804, &HA3C)
                mRevPKMFont.Add(&HC808, &HA3D)
                mRevPKMFont.Add(&HC80A, &HA3E)
                mRevPKMFont.Add(&HC810, &HA3F)
                mRevPKMFont.Add(&HC811, &HA40)
                mRevPKMFont.Add(&HC813, &HA41)
                mRevPKMFont.Add(&HC815, &HA42)
                mRevPKMFont.Add(&HC816, &HA43)
                mRevPKMFont.Add(&HC81C, &HA44)
                mRevPKMFont.Add(&HC81D, &HA45)
                mRevPKMFont.Add(&HC820, &HA46)
                mRevPKMFont.Add(&HC824, &HA47)
                mRevPKMFont.Add(&HC82C, &HA48)
                mRevPKMFont.Add(&HC82D, &HA49)
                mRevPKMFont.Add(&HC82F, &HA4A)
                mRevPKMFont.Add(&HC831, &HA4B)
                mRevPKMFont.Add(&HC838, &HA4C)
                mRevPKMFont.Add(&HC83C, &HA4D)
                mRevPKMFont.Add(&HC840, &HA4E)
                mRevPKMFont.Add(&HC848, &HA4F)
                mRevPKMFont.Add(&HC849, &HA50)
                mRevPKMFont.Add(&HC84C, &HA51)
                mRevPKMFont.Add(&HC84D, &HA52)
                mRevPKMFont.Add(&HC854, &HA53)
                mRevPKMFont.Add(&HC870, &HA54)
                mRevPKMFont.Add(&HC871, &HA55)
                mRevPKMFont.Add(&HC874, &HA56)
                mRevPKMFont.Add(&HC878, &HA57)
                mRevPKMFont.Add(&HC87A, &HA58)
                mRevPKMFont.Add(&HC880, &HA59)
                mRevPKMFont.Add(&HC881, &HA5A)
                mRevPKMFont.Add(&HC883, &HA5B)
                mRevPKMFont.Add(&HC885, &HA5C)
                mRevPKMFont.Add(&HC886, &HA5D)
                mRevPKMFont.Add(&HC887, &HA5E)
                mRevPKMFont.Add(&HC88B, &HA5F)
                mRevPKMFont.Add(&HC88C, &HA60)
                mRevPKMFont.Add(&HC88D, &HA61)
                mRevPKMFont.Add(&HC894, &HA62)
                mRevPKMFont.Add(&HC89D, &HA63)
                mRevPKMFont.Add(&HC89F, &HA64)
                mRevPKMFont.Add(&HC8A1, &HA65)
                mRevPKMFont.Add(&HC8A8, &HA66)
                mRevPKMFont.Add(&HC8BC, &HA67)
                mRevPKMFont.Add(&HC8BD, &HA68)
                mRevPKMFont.Add(&HC8C4, &HA69)
                mRevPKMFont.Add(&HC8C8, &HA6A)
                mRevPKMFont.Add(&HC8CC, &HA6B)
                mRevPKMFont.Add(&HC8D4, &HA6C)
                mRevPKMFont.Add(&HC8D5, &HA6D)
                mRevPKMFont.Add(&HC8D7, &HA6E)
                mRevPKMFont.Add(&HC8D9, &HA6F)
                mRevPKMFont.Add(&HC8E0, &HA70)
                mRevPKMFont.Add(&HC8E1, &HA71)
                mRevPKMFont.Add(&HC8E4, &HA72)
                mRevPKMFont.Add(&HC8F5, &HA73)
                mRevPKMFont.Add(&HC8FC, &HA74)
                mRevPKMFont.Add(&HC8FD, &HA75)
                mRevPKMFont.Add(&HC900, &HA76)
                mRevPKMFont.Add(&HC904, &HA77)
                mRevPKMFont.Add(&HC905, &HA78)
                mRevPKMFont.Add(&HC906, &HA79)
                mRevPKMFont.Add(&HC90C, &HA7A)
                mRevPKMFont.Add(&HC90D, &HA7B)
                mRevPKMFont.Add(&HC90F, &HA7C)
                mRevPKMFont.Add(&HC911, &HA7D)
                mRevPKMFont.Add(&HC918, &HA7E)
                mRevPKMFont.Add(&HC92C, &HA7F)
                mRevPKMFont.Add(&HC934, &HA80)
                mRevPKMFont.Add(&HC950, &HA81)
                mRevPKMFont.Add(&HC951, &HA82)
                mRevPKMFont.Add(&HC954, &HA83)
                mRevPKMFont.Add(&HC958, &HA84)
                mRevPKMFont.Add(&HC960, &HA85)
                mRevPKMFont.Add(&HC961, &HA86)
                mRevPKMFont.Add(&HC963, &HA87)
                mRevPKMFont.Add(&HC96C, &HA88)
                mRevPKMFont.Add(&HC970, &HA89)
                mRevPKMFont.Add(&HC974, &HA8A)
                mRevPKMFont.Add(&HC97C, &HA8B)
                mRevPKMFont.Add(&HC988, &HA8C)
                mRevPKMFont.Add(&HC989, &HA8D)
                mRevPKMFont.Add(&HC98C, &HA8E)
                mRevPKMFont.Add(&HC990, &HA8F)
                mRevPKMFont.Add(&HC998, &HA90)
                mRevPKMFont.Add(&HC999, &HA91)
                mRevPKMFont.Add(&HC99B, &HA92)
                mRevPKMFont.Add(&HC99D, &HA93)
                mRevPKMFont.Add(&HC9C0, &HA94)
                mRevPKMFont.Add(&HC9C1, &HA95)
                mRevPKMFont.Add(&HC9C4, &HA96)
                mRevPKMFont.Add(&HC9C7, &HA97)
                mRevPKMFont.Add(&HC9C8, &HA98)
                mRevPKMFont.Add(&HC9CA, &HA99)
                mRevPKMFont.Add(&HC9D0, &HA9A)
                mRevPKMFont.Add(&HC9D1, &HA9B)
                mRevPKMFont.Add(&HC9D3, &HA9C)
                mRevPKMFont.Add(&HC9D5, &HA9D)
                mRevPKMFont.Add(&HC9D6, &HA9E)
                mRevPKMFont.Add(&HC9D9, &HA9F)
                mRevPKMFont.Add(&HC9DA, &HAA0)
                mRevPKMFont.Add(&HC9DC, &HAA1)
                mRevPKMFont.Add(&HC9DD, &HAA2)
                mRevPKMFont.Add(&HC9E0, &HAA3)
                mRevPKMFont.Add(&HC9E2, &HAA4)
                mRevPKMFont.Add(&HC9E4, &HAA5)
                mRevPKMFont.Add(&HC9E7, &HAA6)
                mRevPKMFont.Add(&HC9EC, &HAA7)
                mRevPKMFont.Add(&HC9ED, &HAA8)
                mRevPKMFont.Add(&HC9EF, &HAA9)
                mRevPKMFont.Add(&HC9F0, &HAAA)
                mRevPKMFont.Add(&HC9F1, &HAAB)
                mRevPKMFont.Add(&HC9F8, &HAAC)
                mRevPKMFont.Add(&HC9F9, &HAAD)
                mRevPKMFont.Add(&HC9FC, &HAAE)
                mRevPKMFont.Add(&HCA00, &HAAF)
                mRevPKMFont.Add(&HCA08, &HAB0)
                mRevPKMFont.Add(&HCA09, &HAB1)
                mRevPKMFont.Add(&HCA0B, &HAB2)
                mRevPKMFont.Add(&HCA0C, &HAB3)
                mRevPKMFont.Add(&HCA0D, &HAB4)
                mRevPKMFont.Add(&HCA14, &HAB5)
                mRevPKMFont.Add(&HCA18, &HAB6)
                mRevPKMFont.Add(&HCA29, &HAB7)
                mRevPKMFont.Add(&HCA4C, &HAB8)
                mRevPKMFont.Add(&HCA4D, &HAB9)
                mRevPKMFont.Add(&HCA50, &HABA)
                mRevPKMFont.Add(&HCA54, &HABB)
                mRevPKMFont.Add(&HCA5C, &HABC)
                mRevPKMFont.Add(&HCA5D, &HABD)
                mRevPKMFont.Add(&HCA5F, &HABE)
                mRevPKMFont.Add(&HCA60, &HABF)
                mRevPKMFont.Add(&HCA61, &HAC0)
                mRevPKMFont.Add(&HCA68, &HAC1)
                mRevPKMFont.Add(&HCA7D, &HAC2)
                mRevPKMFont.Add(&HCA84, &HAC3)
                mRevPKMFont.Add(&HCA98, &HAC4)
                mRevPKMFont.Add(&HCABC, &HAC5)
                mRevPKMFont.Add(&HCABD, &HAC6)
                mRevPKMFont.Add(&HCAC0, &HAC7)
                mRevPKMFont.Add(&HCAC4, &HAC8)
                mRevPKMFont.Add(&HCACC, &HAC9)
                mRevPKMFont.Add(&HCACD, &HACA)
                mRevPKMFont.Add(&HCACF, &HACB)
                mRevPKMFont.Add(&HCAD1, &HACC)
                mRevPKMFont.Add(&HCAD3, &HACD)
                mRevPKMFont.Add(&HCAD8, &HACE)
                mRevPKMFont.Add(&HCAD9, &HACF)
                mRevPKMFont.Add(&HCAE0, &HAD0)
                mRevPKMFont.Add(&HCAEC, &HAD1)
                mRevPKMFont.Add(&HCAF4, &HAD2)
                mRevPKMFont.Add(&HCB08, &HAD3)
                mRevPKMFont.Add(&HCB10, &HAD4)
                mRevPKMFont.Add(&HCB14, &HAD5)
                mRevPKMFont.Add(&HCB18, &HAD6)
                mRevPKMFont.Add(&HCB20, &HAD7)
                mRevPKMFont.Add(&HCB21, &HAD8)
                mRevPKMFont.Add(&HCB41, &HAD9)
                mRevPKMFont.Add(&HCB48, &HADA)
                mRevPKMFont.Add(&HCB49, &HADB)
                mRevPKMFont.Add(&HCB4C, &HADC)
                mRevPKMFont.Add(&HCB50, &HADD)
                mRevPKMFont.Add(&HCB58, &HADE)
                mRevPKMFont.Add(&HCB59, &HADF)
                mRevPKMFont.Add(&HCB5D, &HAE0)
                mRevPKMFont.Add(&HCB64, &HAE1)
                mRevPKMFont.Add(&HCB78, &HAE2)
                mRevPKMFont.Add(&HCB79, &HAE3)
                mRevPKMFont.Add(&HCB9C, &HAE4)
                mRevPKMFont.Add(&HCBB8, &HAE5)
                mRevPKMFont.Add(&HCBD4, &HAE6)
                mRevPKMFont.Add(&HCBE4, &HAE7)
                mRevPKMFont.Add(&HCBE7, &HAE8)
                mRevPKMFont.Add(&HCBE9, &HAE9)
                mRevPKMFont.Add(&HCC0C, &HAEA)
                mRevPKMFont.Add(&HCC0D, &HAEB)
                mRevPKMFont.Add(&HCC10, &HAEC)
                mRevPKMFont.Add(&HCC14, &HAED)
                mRevPKMFont.Add(&HCC1C, &HAEE)
                mRevPKMFont.Add(&HCC1D, &HAEF)
                mRevPKMFont.Add(&HCC21, &HAF0)
                mRevPKMFont.Add(&HCC22, &HAF1)
                mRevPKMFont.Add(&HCC27, &HAF2)
                mRevPKMFont.Add(&HCC28, &HAF3)
                mRevPKMFont.Add(&HCC29, &HAF4)
                mRevPKMFont.Add(&HCC2C, &HAF5)
                mRevPKMFont.Add(&HCC2E, &HAF6)
                mRevPKMFont.Add(&HCC30, &HAF7)
                mRevPKMFont.Add(&HCC38, &HAF8)
                mRevPKMFont.Add(&HCC39, &HAF9)
                mRevPKMFont.Add(&HCC3B, &HAFA)
                mRevPKMFont.Add(&HCC3C, &HAFB)
                mRevPKMFont.Add(&HCC3D, &HAFC)
                mRevPKMFont.Add(&HCC3E, &HAFD)
                mRevPKMFont.Add(&HCC44, &HAFE)
                mRevPKMFont.Add(&HCC45, &HAFF)
                mRevPKMFont.Add(&HCC48, &HB00)
                mRevPKMFont.Add(&HCC4C, &HB01)
                mRevPKMFont.Add(&HCC54, &HB02)
                mRevPKMFont.Add(&HCC55, &HB03)
                mRevPKMFont.Add(&HCC57, &HB04)
                mRevPKMFont.Add(&HCC58, &HB05)
                mRevPKMFont.Add(&HCC59, &HB06)
                mRevPKMFont.Add(&HCC60, &HB07)
                mRevPKMFont.Add(&HCC64, &HB08)
                mRevPKMFont.Add(&HCC66, &HB09)
                mRevPKMFont.Add(&HCC68, &HB0A)
                mRevPKMFont.Add(&HCC70, &HB0B)
                mRevPKMFont.Add(&HCC75, &HB0C)
                mRevPKMFont.Add(&HCC98, &HB0D)
                mRevPKMFont.Add(&HCC99, &HB0E)
                mRevPKMFont.Add(&HCC9C, &HB0F)
                mRevPKMFont.Add(&HCCA0, &HB10)
                mRevPKMFont.Add(&HCCA8, &HB11)
                mRevPKMFont.Add(&HCCA9, &HB12)
                mRevPKMFont.Add(&HCCAB, &HB13)
                mRevPKMFont.Add(&HCCAC, &HB14)
                mRevPKMFont.Add(&HCCAD, &HB15)
                mRevPKMFont.Add(&HCCB4, &HB16)
                mRevPKMFont.Add(&HCCB5, &HB17)
                mRevPKMFont.Add(&HCCB8, &HB18)
                mRevPKMFont.Add(&HCCBC, &HB19)
                mRevPKMFont.Add(&HCCC4, &HB1A)
                mRevPKMFont.Add(&HCCC5, &HB1B)
                mRevPKMFont.Add(&HCCC7, &HB1C)
                mRevPKMFont.Add(&HCCC9, &HB1D)
                mRevPKMFont.Add(&HCCD0, &HB1E)
                mRevPKMFont.Add(&HCCD4, &HB1F)
                mRevPKMFont.Add(&HCCE4, &HB20)
                mRevPKMFont.Add(&HCCEC, &HB21)
                mRevPKMFont.Add(&HCCF0, &HB22)
                mRevPKMFont.Add(&HCD01, &HB23)
                mRevPKMFont.Add(&HCD08, &HB24)
                mRevPKMFont.Add(&HCD09, &HB25)
                mRevPKMFont.Add(&HCD0C, &HB26)
                mRevPKMFont.Add(&HCD10, &HB27)
                mRevPKMFont.Add(&HCD18, &HB28)
                mRevPKMFont.Add(&HCD19, &HB29)
                mRevPKMFont.Add(&HCD1B, &HB2A)
                mRevPKMFont.Add(&HCD1D, &HB2B)
                mRevPKMFont.Add(&HCD24, &HB2C)
                mRevPKMFont.Add(&HCD28, &HB2D)
                mRevPKMFont.Add(&HCD2C, &HB2E)
                mRevPKMFont.Add(&HCD39, &HB2F)
                mRevPKMFont.Add(&HCD5C, &HB30)
                mRevPKMFont.Add(&HCD60, &HB31)
                mRevPKMFont.Add(&HCD64, &HB32)
                mRevPKMFont.Add(&HCD6C, &HB33)
                mRevPKMFont.Add(&HCD6D, &HB34)
                mRevPKMFont.Add(&HCD6F, &HB35)
                mRevPKMFont.Add(&HCD71, &HB36)
                mRevPKMFont.Add(&HCD78, &HB37)
                mRevPKMFont.Add(&HCD88, &HB38)
                mRevPKMFont.Add(&HCD94, &HB39)
                mRevPKMFont.Add(&HCD95, &HB3A)
                mRevPKMFont.Add(&HCD98, &HB3B)
                mRevPKMFont.Add(&HCD9C, &HB3C)
                mRevPKMFont.Add(&HCDA4, &HB3D)
                mRevPKMFont.Add(&HCDA5, &HB3E)
                mRevPKMFont.Add(&HCDA7, &HB3F)
                mRevPKMFont.Add(&HCDA9, &HB40)
                mRevPKMFont.Add(&HCDB0, &HB41)
                mRevPKMFont.Add(&HCDC4, &HB42)
                mRevPKMFont.Add(&HCDCC, &HB43)
                mRevPKMFont.Add(&HCDD0, &HB44)
                mRevPKMFont.Add(&HCDE8, &HB45)
                mRevPKMFont.Add(&HCDEC, &HB46)
                mRevPKMFont.Add(&HCDF0, &HB47)
                mRevPKMFont.Add(&HCDF8, &HB48)
                mRevPKMFont.Add(&HCDF9, &HB49)
                mRevPKMFont.Add(&HCDFB, &HB4A)
                mRevPKMFont.Add(&HCDFD, &HB4B)
                mRevPKMFont.Add(&HCE04, &HB4C)
                mRevPKMFont.Add(&HCE08, &HB4D)
                mRevPKMFont.Add(&HCE0C, &HB4E)
                mRevPKMFont.Add(&HCE14, &HB4F)
                mRevPKMFont.Add(&HCE19, &HB50)
                mRevPKMFont.Add(&HCE20, &HB51)
                mRevPKMFont.Add(&HCE21, &HB52)
                mRevPKMFont.Add(&HCE24, &HB53)
                mRevPKMFont.Add(&HCE28, &HB54)
                mRevPKMFont.Add(&HCE30, &HB55)
                mRevPKMFont.Add(&HCE31, &HB56)
                mRevPKMFont.Add(&HCE33, &HB57)
                mRevPKMFont.Add(&HCE35, &HB58)
                mRevPKMFont.Add(&HCE58, &HB59)
                mRevPKMFont.Add(&HCE59, &HB5A)
                mRevPKMFont.Add(&HCE5C, &HB5B)
                mRevPKMFont.Add(&HCE5F, &HB5C)
                mRevPKMFont.Add(&HCE60, &HB5D)
                mRevPKMFont.Add(&HCE61, &HB5E)
                mRevPKMFont.Add(&HCE68, &HB5F)
                mRevPKMFont.Add(&HCE69, &HB60)
                mRevPKMFont.Add(&HCE6B, &HB61)
                mRevPKMFont.Add(&HCE6D, &HB62)
                mRevPKMFont.Add(&HCE74, &HB63)
                mRevPKMFont.Add(&HCE75, &HB64)
                mRevPKMFont.Add(&HCE78, &HB65)
                mRevPKMFont.Add(&HCE7C, &HB66)
                mRevPKMFont.Add(&HCE84, &HB67)
                mRevPKMFont.Add(&HCE85, &HB68)
                mRevPKMFont.Add(&HCE87, &HB69)
                mRevPKMFont.Add(&HCE89, &HB6A)
                mRevPKMFont.Add(&HCE90, &HB6B)
                mRevPKMFont.Add(&HCE91, &HB6C)
                mRevPKMFont.Add(&HCE94, &HB6D)
                mRevPKMFont.Add(&HCE98, &HB6E)
                mRevPKMFont.Add(&HCEA0, &HB6F)
                mRevPKMFont.Add(&HCEA1, &HB70)
                mRevPKMFont.Add(&HCEA3, &HB71)
                mRevPKMFont.Add(&HCEA4, &HB72)
                mRevPKMFont.Add(&HCEA5, &HB73)
                mRevPKMFont.Add(&HCEAC, &HB74)
                mRevPKMFont.Add(&HCEAD, &HB75)
                mRevPKMFont.Add(&HCEC1, &HB76)
                mRevPKMFont.Add(&HCEE4, &HB77)
                mRevPKMFont.Add(&HCEE5, &HB78)
                mRevPKMFont.Add(&HCEE8, &HB79)
                mRevPKMFont.Add(&HCEEB, &HB7A)
                mRevPKMFont.Add(&HCEEC, &HB7B)
                mRevPKMFont.Add(&HCEF4, &HB7C)
                mRevPKMFont.Add(&HCEF5, &HB7D)
                mRevPKMFont.Add(&HCEF7, &HB7E)
                mRevPKMFont.Add(&HCEF8, &HB7F)
                mRevPKMFont.Add(&HCEF9, &HB80)
                mRevPKMFont.Add(&HCF00, &HB81)
                mRevPKMFont.Add(&HCF01, &HB82)
                mRevPKMFont.Add(&HCF04, &HB83)
                mRevPKMFont.Add(&HCF08, &HB84)
                mRevPKMFont.Add(&HCF10, &HB85)
                mRevPKMFont.Add(&HCF11, &HB86)
                mRevPKMFont.Add(&HCF13, &HB87)
                mRevPKMFont.Add(&HCF15, &HB88)
                mRevPKMFont.Add(&HCF1C, &HB89)
                mRevPKMFont.Add(&HCF20, &HB8A)
                mRevPKMFont.Add(&HCF24, &HB8B)
                mRevPKMFont.Add(&HCF2C, &HB8C)
                mRevPKMFont.Add(&HCF2D, &HB8D)
                mRevPKMFont.Add(&HCF2F, &HB8E)
                mRevPKMFont.Add(&HCF30, &HB8F)
                mRevPKMFont.Add(&HCF31, &HB90)
                mRevPKMFont.Add(&HCF38, &HB91)
                mRevPKMFont.Add(&HCF54, &HB92)
                mRevPKMFont.Add(&HCF55, &HB93)
                mRevPKMFont.Add(&HCF58, &HB94)
                mRevPKMFont.Add(&HCF5C, &HB95)
                mRevPKMFont.Add(&HCF64, &HB96)
                mRevPKMFont.Add(&HCF65, &HB97)
                mRevPKMFont.Add(&HCF67, &HB98)
                mRevPKMFont.Add(&HCF69, &HB99)
                mRevPKMFont.Add(&HCF70, &HB9A)
                mRevPKMFont.Add(&HCF71, &HB9B)
                mRevPKMFont.Add(&HCF74, &HB9C)
                mRevPKMFont.Add(&HCF78, &HB9D)
                mRevPKMFont.Add(&HCF80, &HB9E)
                mRevPKMFont.Add(&HCF85, &HB9F)
                mRevPKMFont.Add(&HCF8C, &HBA0)
                mRevPKMFont.Add(&HCFA1, &HBA1)
                mRevPKMFont.Add(&HCFA8, &HBA2)
                mRevPKMFont.Add(&HCFB0, &HBA3)
                mRevPKMFont.Add(&HCFC4, &HBA4)
                mRevPKMFont.Add(&HCFE0, &HBA5)
                mRevPKMFont.Add(&HCFE1, &HBA6)
                mRevPKMFont.Add(&HCFE4, &HBA7)
                mRevPKMFont.Add(&HCFE8, &HBA8)
                mRevPKMFont.Add(&HCFF0, &HBA9)
                mRevPKMFont.Add(&HCFF1, &HBAA)
                mRevPKMFont.Add(&HCFF3, &HBAB)
                mRevPKMFont.Add(&HCFF5, &HBAC)
                mRevPKMFont.Add(&HCFFC, &HBAD)
                mRevPKMFont.Add(&HD000, &HBAE)
                mRevPKMFont.Add(&HD004, &HBAF)
                mRevPKMFont.Add(&HD011, &HBB0)
                mRevPKMFont.Add(&HD018, &HBB1)
                mRevPKMFont.Add(&HD02D, &HBB2)
                mRevPKMFont.Add(&HD034, &HBB3)
                mRevPKMFont.Add(&HD035, &HBB4)
                mRevPKMFont.Add(&HD038, &HBB5)
                mRevPKMFont.Add(&HD03C, &HBB6)
                mRevPKMFont.Add(&HD044, &HBB7)
                mRevPKMFont.Add(&HD045, &HBB8)
                mRevPKMFont.Add(&HD047, &HBB9)
                mRevPKMFont.Add(&HD049, &HBBA)
                mRevPKMFont.Add(&HD050, &HBBB)
                mRevPKMFont.Add(&HD054, &HBBC)
                mRevPKMFont.Add(&HD058, &HBBD)
                mRevPKMFont.Add(&HD060, &HBBE)
                mRevPKMFont.Add(&HD06C, &HBBF)
                mRevPKMFont.Add(&HD06D, &HBC0)
                mRevPKMFont.Add(&HD070, &HBC1)
                mRevPKMFont.Add(&HD074, &HBC2)
                mRevPKMFont.Add(&HD07C, &HBC3)
                mRevPKMFont.Add(&HD07D, &HBC4)
                mRevPKMFont.Add(&HD081, &HBC5)
                mRevPKMFont.Add(&HD0A4, &HBC6)
                mRevPKMFont.Add(&HD0A5, &HBC7)
                mRevPKMFont.Add(&HD0A8, &HBC8)
                mRevPKMFont.Add(&HD0AC, &HBC9)
                mRevPKMFont.Add(&HD0B4, &HBCA)
                mRevPKMFont.Add(&HD0B5, &HBCB)
                mRevPKMFont.Add(&HD0B7, &HBCC)
                mRevPKMFont.Add(&HD0B9, &HBCD)
                mRevPKMFont.Add(&HD0C0, &HBCE)
                mRevPKMFont.Add(&HD0C1, &HBCF)
                mRevPKMFont.Add(&HD0C4, &HBD0)
                mRevPKMFont.Add(&HD0C8, &HBD1)
                mRevPKMFont.Add(&HD0C9, &HBD2)
                mRevPKMFont.Add(&HD0D0, &HBD3)
                mRevPKMFont.Add(&HD0D1, &HBD4)
                mRevPKMFont.Add(&HD0D3, &HBD5)
                mRevPKMFont.Add(&HD0D4, &HBD6)
                mRevPKMFont.Add(&HD0D5, &HBD7)
                mRevPKMFont.Add(&HD0DC, &HBD8)
                mRevPKMFont.Add(&HD0DD, &HBD9)
                mRevPKMFont.Add(&HD0E0, &HBDA)
                mRevPKMFont.Add(&HD0E4, &HBDB)
                mRevPKMFont.Add(&HD0EC, &HBDC)
                mRevPKMFont.Add(&HD0ED, &HBDD)
                mRevPKMFont.Add(&HD0EF, &HBDE)
                mRevPKMFont.Add(&HD0F0, &HBDF)
                mRevPKMFont.Add(&HD0F1, &HBE0)
                mRevPKMFont.Add(&HD0F8, &HBE1)
                mRevPKMFont.Add(&HD10D, &HBE2)
                mRevPKMFont.Add(&HD130, &HBE3)
                mRevPKMFont.Add(&HD131, &HBE4)
                mRevPKMFont.Add(&HD134, &HBE5)
                mRevPKMFont.Add(&HD138, &HBE6)
                mRevPKMFont.Add(&HD13A, &HBE7)
                mRevPKMFont.Add(&HD140, &HBE8)
                mRevPKMFont.Add(&HD141, &HBE9)
                mRevPKMFont.Add(&HD143, &HBEA)
                mRevPKMFont.Add(&HD144, &HBEB)
                mRevPKMFont.Add(&HD145, &HBEC)
                mRevPKMFont.Add(&HD14C, &HBED)
                mRevPKMFont.Add(&HD14D, &HBEE)
                mRevPKMFont.Add(&HD150, &HBEF)
                mRevPKMFont.Add(&HD154, &HBF0)
                mRevPKMFont.Add(&HD15C, &HBF1)
                mRevPKMFont.Add(&HD15D, &HBF2)
                mRevPKMFont.Add(&HD15F, &HBF3)
                mRevPKMFont.Add(&HD161, &HBF4)
                mRevPKMFont.Add(&HD168, &HBF5)
                mRevPKMFont.Add(&HD16C, &HBF6)
                mRevPKMFont.Add(&HD17C, &HBF7)
                mRevPKMFont.Add(&HD184, &HBF8)
                mRevPKMFont.Add(&HD188, &HBF9)
                mRevPKMFont.Add(&HD1A0, &HBFA)
                mRevPKMFont.Add(&HD1A1, &HBFB)
                mRevPKMFont.Add(&HD1A4, &HBFC)
                mRevPKMFont.Add(&HD1A8, &HBFD)
                mRevPKMFont.Add(&HD1B0, &HBFE)
                mRevPKMFont.Add(&HD1B1, &HBFF)
                mRevPKMFont.Add(&HD1B3, &HC00)
                mRevPKMFont.Add(&HD1B5, &HC01)
                mRevPKMFont.Add(&HD1BA, &HC02)
                mRevPKMFont.Add(&HD1BC, &HC03)
                mRevPKMFont.Add(&HD1C0, &HC04)
                mRevPKMFont.Add(&HD1D8, &HC05)
                mRevPKMFont.Add(&HD1F4, &HC06)
                mRevPKMFont.Add(&HD1F8, &HC07)
                mRevPKMFont.Add(&HD207, &HC08)
                mRevPKMFont.Add(&HD209, &HC09)
                mRevPKMFont.Add(&HD210, &HC0A)
                mRevPKMFont.Add(&HD22C, &HC0B)
                mRevPKMFont.Add(&HD22D, &HC0C)
                mRevPKMFont.Add(&HD230, &HC0D)
                mRevPKMFont.Add(&HD234, &HC0E)
                mRevPKMFont.Add(&HD23C, &HC0F)
                mRevPKMFont.Add(&HD23D, &HC10)
                mRevPKMFont.Add(&HD23F, &HC11)
                mRevPKMFont.Add(&HD241, &HC12)
                mRevPKMFont.Add(&HD248, &HC13)
                mRevPKMFont.Add(&HD25C, &HC14)
                mRevPKMFont.Add(&HD264, &HC15)
                mRevPKMFont.Add(&HD280, &HC16)
                mRevPKMFont.Add(&HD281, &HC17)
                mRevPKMFont.Add(&HD284, &HC18)
                mRevPKMFont.Add(&HD288, &HC19)
                mRevPKMFont.Add(&HD290, &HC1A)
                mRevPKMFont.Add(&HD291, &HC1B)
                mRevPKMFont.Add(&HD295, &HC1C)
                mRevPKMFont.Add(&HD29C, &HC1D)
                mRevPKMFont.Add(&HD2A0, &HC1E)
                mRevPKMFont.Add(&HD2A4, &HC1F)
                mRevPKMFont.Add(&HD2AC, &HC20)
                mRevPKMFont.Add(&HD2B1, &HC21)
                mRevPKMFont.Add(&HD2B8, &HC22)
                mRevPKMFont.Add(&HD2B9, &HC23)
                mRevPKMFont.Add(&HD2BC, &HC24)
                mRevPKMFont.Add(&HD2BF, &HC25)
                mRevPKMFont.Add(&HD2C0, &HC26)
                mRevPKMFont.Add(&HD2C2, &HC27)
                mRevPKMFont.Add(&HD2C8, &HC28)
                mRevPKMFont.Add(&HD2C9, &HC29)
                mRevPKMFont.Add(&HD2CB, &HC2A)
                mRevPKMFont.Add(&HD2D4, &HC2B)
                mRevPKMFont.Add(&HD2D8, &HC2C)
                mRevPKMFont.Add(&HD2DC, &HC2D)
                mRevPKMFont.Add(&HD2E4, &HC2E)
                mRevPKMFont.Add(&HD2E5, &HC2F)
                mRevPKMFont.Add(&HD2F0, &HC30)
                mRevPKMFont.Add(&HD2F1, &HC31)
                mRevPKMFont.Add(&HD2F4, &HC32)
                mRevPKMFont.Add(&HD2F8, &HC33)
                mRevPKMFont.Add(&HD300, &HC34)
                mRevPKMFont.Add(&HD301, &HC35)
                mRevPKMFont.Add(&HD303, &HC36)
                mRevPKMFont.Add(&HD305, &HC37)
                mRevPKMFont.Add(&HD30C, &HC38)
                mRevPKMFont.Add(&HD30D, &HC39)
                mRevPKMFont.Add(&HD30E, &HC3A)
                mRevPKMFont.Add(&HD310, &HC3B)
                mRevPKMFont.Add(&HD314, &HC3C)
                mRevPKMFont.Add(&HD316, &HC3D)
                mRevPKMFont.Add(&HD31C, &HC3E)
                mRevPKMFont.Add(&HD31D, &HC3F)
                mRevPKMFont.Add(&HD31F, &HC40)
                mRevPKMFont.Add(&HD320, &HC41)
                mRevPKMFont.Add(&HD321, &HC42)
                mRevPKMFont.Add(&HD325, &HC43)
                mRevPKMFont.Add(&HD328, &HC44)
                mRevPKMFont.Add(&HD329, &HC45)
                mRevPKMFont.Add(&HD32C, &HC46)
                mRevPKMFont.Add(&HD330, &HC47)
                mRevPKMFont.Add(&HD338, &HC48)
                mRevPKMFont.Add(&HD339, &HC49)
                mRevPKMFont.Add(&HD33B, &HC4A)
                mRevPKMFont.Add(&HD33C, &HC4B)
                mRevPKMFont.Add(&HD33D, &HC4C)
                mRevPKMFont.Add(&HD344, &HC4D)
                mRevPKMFont.Add(&HD345, &HC4E)
                mRevPKMFont.Add(&HD37C, &HC4F)
                mRevPKMFont.Add(&HD37D, &HC50)
                mRevPKMFont.Add(&HD380, &HC51)
                mRevPKMFont.Add(&HD384, &HC52)
                mRevPKMFont.Add(&HD38C, &HC53)
                mRevPKMFont.Add(&HD38D, &HC54)
                mRevPKMFont.Add(&HD38F, &HC55)
                mRevPKMFont.Add(&HD390, &HC56)
                mRevPKMFont.Add(&HD391, &HC57)
                mRevPKMFont.Add(&HD398, &HC58)
                mRevPKMFont.Add(&HD399, &HC59)
                mRevPKMFont.Add(&HD39C, &HC5A)
                mRevPKMFont.Add(&HD3A0, &HC5B)
                mRevPKMFont.Add(&HD3A8, &HC5C)
                mRevPKMFont.Add(&HD3A9, &HC5D)
                mRevPKMFont.Add(&HD3AB, &HC5E)
                mRevPKMFont.Add(&HD3AD, &HC5F)
                mRevPKMFont.Add(&HD3B4, &HC60)
                mRevPKMFont.Add(&HD3B8, &HC61)
                mRevPKMFont.Add(&HD3BC, &HC62)
                mRevPKMFont.Add(&HD3C4, &HC63)
                mRevPKMFont.Add(&HD3C5, &HC64)
                mRevPKMFont.Add(&HD3C8, &HC65)
                mRevPKMFont.Add(&HD3C9, &HC66)
                mRevPKMFont.Add(&HD3D0, &HC67)
                mRevPKMFont.Add(&HD3D8, &HC68)
                mRevPKMFont.Add(&HD3E1, &HC69)
                mRevPKMFont.Add(&HD3E3, &HC6A)
                mRevPKMFont.Add(&HD3EC, &HC6B)
                mRevPKMFont.Add(&HD3ED, &HC6C)
                mRevPKMFont.Add(&HD3F0, &HC6D)
                mRevPKMFont.Add(&HD3F4, &HC6E)
                mRevPKMFont.Add(&HD3FC, &HC6F)
                mRevPKMFont.Add(&HD3FD, &HC70)
                mRevPKMFont.Add(&HD3FF, &HC71)
                mRevPKMFont.Add(&HD401, &HC72)
                mRevPKMFont.Add(&HD408, &HC73)
                mRevPKMFont.Add(&HD41D, &HC74)
                mRevPKMFont.Add(&HD440, &HC75)
                mRevPKMFont.Add(&HD444, &HC76)
                mRevPKMFont.Add(&HD45C, &HC77)
                mRevPKMFont.Add(&HD460, &HC78)
                mRevPKMFont.Add(&HD464, &HC79)
                mRevPKMFont.Add(&HD46D, &HC7A)
                mRevPKMFont.Add(&HD46F, &HC7B)
                mRevPKMFont.Add(&HD478, &HC7C)
                mRevPKMFont.Add(&HD479, &HC7D)
                mRevPKMFont.Add(&HD47C, &HC7E)
                mRevPKMFont.Add(&HD47F, &HC7F)
                mRevPKMFont.Add(&HD480, &HC80)
                mRevPKMFont.Add(&HD482, &HC81)
                mRevPKMFont.Add(&HD488, &HC82)
                mRevPKMFont.Add(&HD489, &HC83)
                mRevPKMFont.Add(&HD48B, &HC84)
                mRevPKMFont.Add(&HD48D, &HC85)
                mRevPKMFont.Add(&HD494, &HC86)
                mRevPKMFont.Add(&HD4A9, &HC87)
                mRevPKMFont.Add(&HD4CC, &HC88)
                mRevPKMFont.Add(&HD4D0, &HC89)
                mRevPKMFont.Add(&HD4D4, &HC8A)
                mRevPKMFont.Add(&HD4DC, &HC8B)
                mRevPKMFont.Add(&HD4DF, &HC8C)
                mRevPKMFont.Add(&HD4E8, &HC8D)
                mRevPKMFont.Add(&HD4EC, &HC8E)
                mRevPKMFont.Add(&HD4F0, &HC8F)
                mRevPKMFont.Add(&HD4F8, &HC90)
                mRevPKMFont.Add(&HD4FB, &HC91)
                mRevPKMFont.Add(&HD4FD, &HC92)
                mRevPKMFont.Add(&HD504, &HC93)
                mRevPKMFont.Add(&HD508, &HC94)
                mRevPKMFont.Add(&HD50C, &HC95)
                mRevPKMFont.Add(&HD514, &HC96)
                mRevPKMFont.Add(&HD515, &HC97)
                mRevPKMFont.Add(&HD517, &HC98)
                mRevPKMFont.Add(&HD53C, &HC99)
                mRevPKMFont.Add(&HD53D, &HC9A)
                mRevPKMFont.Add(&HD540, &HC9B)
                mRevPKMFont.Add(&HD544, &HC9C)
                mRevPKMFont.Add(&HD54C, &HC9D)
                mRevPKMFont.Add(&HD54D, &HC9E)
                mRevPKMFont.Add(&HD54F, &HC9F)
                mRevPKMFont.Add(&HD551, &HCA0)
                mRevPKMFont.Add(&HD558, &HCA1)
                mRevPKMFont.Add(&HD559, &HCA2)
                mRevPKMFont.Add(&HD55C, &HCA3)
                mRevPKMFont.Add(&HD560, &HCA4)
                mRevPKMFont.Add(&HD565, &HCA5)
                mRevPKMFont.Add(&HD568, &HCA6)
                mRevPKMFont.Add(&HD569, &HCA7)
                mRevPKMFont.Add(&HD56B, &HCA8)
                mRevPKMFont.Add(&HD56D, &HCA9)
                mRevPKMFont.Add(&HD574, &HCAA)
                mRevPKMFont.Add(&HD575, &HCAB)
                mRevPKMFont.Add(&HD578, &HCAC)
                mRevPKMFont.Add(&HD57C, &HCAD)
                mRevPKMFont.Add(&HD584, &HCAE)
                mRevPKMFont.Add(&HD585, &HCAF)
                mRevPKMFont.Add(&HD587, &HCB0)
                mRevPKMFont.Add(&HD588, &HCB1)
                mRevPKMFont.Add(&HD589, &HCB2)
                mRevPKMFont.Add(&HD590, &HCB3)
                mRevPKMFont.Add(&HD5A5, &HCB4)
                mRevPKMFont.Add(&HD5C8, &HCB5)
                mRevPKMFont.Add(&HD5C9, &HCB6)
                mRevPKMFont.Add(&HD5CC, &HCB7)
                mRevPKMFont.Add(&HD5D0, &HCB8)
                mRevPKMFont.Add(&HD5D2, &HCB9)
                mRevPKMFont.Add(&HD5D8, &HCBA)
                mRevPKMFont.Add(&HD5D9, &HCBB)
                mRevPKMFont.Add(&HD5DB, &HCBC)
                mRevPKMFont.Add(&HD5DD, &HCBD)
                mRevPKMFont.Add(&HD5E4, &HCBE)
                mRevPKMFont.Add(&HD5E5, &HCBF)
                mRevPKMFont.Add(&HD5E8, &HCC0)
                mRevPKMFont.Add(&HD5EC, &HCC1)
                mRevPKMFont.Add(&HD5F4, &HCC2)
                mRevPKMFont.Add(&HD5F5, &HCC3)
                mRevPKMFont.Add(&HD5F7, &HCC4)
                mRevPKMFont.Add(&HD5F9, &HCC5)
                mRevPKMFont.Add(&HD600, &HCC6)
                mRevPKMFont.Add(&HD601, &HCC7)
                mRevPKMFont.Add(&HD604, &HCC8)
                mRevPKMFont.Add(&HD608, &HCC9)
                mRevPKMFont.Add(&HD610, &HCCA)
                mRevPKMFont.Add(&HD611, &HCCB)
                mRevPKMFont.Add(&HD613, &HCCC)
                mRevPKMFont.Add(&HD614, &HCCD)
                mRevPKMFont.Add(&HD615, &HCCE)
                mRevPKMFont.Add(&HD61C, &HCCF)
                mRevPKMFont.Add(&HD620, &HCD0)
                mRevPKMFont.Add(&HD624, &HCD1)
                mRevPKMFont.Add(&HD62D, &HCD2)
                mRevPKMFont.Add(&HD638, &HCD3)
                mRevPKMFont.Add(&HD639, &HCD4)
                mRevPKMFont.Add(&HD63C, &HCD5)
                mRevPKMFont.Add(&HD640, &HCD6)
                mRevPKMFont.Add(&HD645, &HCD7)
                mRevPKMFont.Add(&HD648, &HCD8)
                mRevPKMFont.Add(&HD649, &HCD9)
                mRevPKMFont.Add(&HD64B, &HCDA)
                mRevPKMFont.Add(&HD64D, &HCDB)
                mRevPKMFont.Add(&HD651, &HCDC)
                mRevPKMFont.Add(&HD654, &HCDD)
                mRevPKMFont.Add(&HD655, &HCDE)
                mRevPKMFont.Add(&HD658, &HCDF)
                mRevPKMFont.Add(&HD65C, &HCE0)
                mRevPKMFont.Add(&HD667, &HCE1)
                mRevPKMFont.Add(&HD669, &HCE2)
                mRevPKMFont.Add(&HD670, &HCE3)
                mRevPKMFont.Add(&HD671, &HCE4)
                mRevPKMFont.Add(&HD674, &HCE5)
                mRevPKMFont.Add(&HD683, &HCE6)
                mRevPKMFont.Add(&HD685, &HCE7)
                mRevPKMFont.Add(&HD68C, &HCE8)
                mRevPKMFont.Add(&HD68D, &HCE9)
                mRevPKMFont.Add(&HD690, &HCEA)
                mRevPKMFont.Add(&HD694, &HCEB)
                mRevPKMFont.Add(&HD69D, &HCEC)
                mRevPKMFont.Add(&HD69F, &HCED)
                mRevPKMFont.Add(&HD6A1, &HCEE)
                mRevPKMFont.Add(&HD6A8, &HCEF)
                mRevPKMFont.Add(&HD6AC, &HCF0)
                mRevPKMFont.Add(&HD6B0, &HCF1)
                mRevPKMFont.Add(&HD6B9, &HCF2)
                mRevPKMFont.Add(&HD6BB, &HCF3)
                mRevPKMFont.Add(&HD6C4, &HCF4)
                mRevPKMFont.Add(&HD6C5, &HCF5)
                mRevPKMFont.Add(&HD6C8, &HCF6)
                mRevPKMFont.Add(&HD6CC, &HCF7)
                mRevPKMFont.Add(&HD6D1, &HCF8)
                mRevPKMFont.Add(&HD6D4, &HCF9)
                mRevPKMFont.Add(&HD6D7, &HCFA)
                mRevPKMFont.Add(&HD6D9, &HCFB)
                mRevPKMFont.Add(&HD6E0, &HCFC)
                mRevPKMFont.Add(&HD6E4, &HCFD)
                mRevPKMFont.Add(&HD6E8, &HCFE)
                mRevPKMFont.Add(&HD6F0, &HCFF)
                mRevPKMFont.Add(&HD6F5, &HD00)
                mRevPKMFont.Add(&HD6FC, &HD01)
                mRevPKMFont.Add(&HD6FD, &HD02)
                mRevPKMFont.Add(&HD700, &HD03)
                mRevPKMFont.Add(&HD704, &HD04)
                mRevPKMFont.Add(&HD711, &HD05)
                mRevPKMFont.Add(&HD718, &HD06)
                mRevPKMFont.Add(&HD719, &HD07)
                mRevPKMFont.Add(&HD71C, &HD08)
                mRevPKMFont.Add(&HD720, &HD09)
                mRevPKMFont.Add(&HD728, &HD0A)
                mRevPKMFont.Add(&HD729, &HD0B)
                mRevPKMFont.Add(&HD72B, &HD0C)
                mRevPKMFont.Add(&HD72D, &HD0D)
                mRevPKMFont.Add(&HD734, &HD0E)
                mRevPKMFont.Add(&HD735, &HD0F)
                mRevPKMFont.Add(&HD738, &HD10)
                mRevPKMFont.Add(&HD73C, &HD11)
                mRevPKMFont.Add(&HD744, &HD12)
                mRevPKMFont.Add(&HD747, &HD13)
                mRevPKMFont.Add(&HD749, &HD14)
                mRevPKMFont.Add(&HD750, &HD15)
                mRevPKMFont.Add(&HD751, &HD16)
                mRevPKMFont.Add(&HD754, &HD17)
                mRevPKMFont.Add(&HD756, &HD18)
                mRevPKMFont.Add(&HD757, &HD19)
                mRevPKMFont.Add(&HD758, &HD1A)
                mRevPKMFont.Add(&HD759, &HD1B)
                mRevPKMFont.Add(&HD760, &HD1C)
                mRevPKMFont.Add(&HD761, &HD1D)
                mRevPKMFont.Add(&HD763, &HD1E)
                mRevPKMFont.Add(&HD765, &HD1F)
                mRevPKMFont.Add(&HD769, &HD20)
                mRevPKMFont.Add(&HD76C, &HD21)
                mRevPKMFont.Add(&HD770, &HD22)
                mRevPKMFont.Add(&HD774, &HD23)
                mRevPKMFont.Add(&HD77C, &HD24)
                mRevPKMFont.Add(&HD77D, &HD25)
                mRevPKMFont.Add(&HD781, &HD26)
                mRevPKMFont.Add(&HD788, &HD27)
                mRevPKMFont.Add(&HD789, &HD28)
                mRevPKMFont.Add(&HD78C, &HD29)
                mRevPKMFont.Add(&HD790, &HD2A)
                mRevPKMFont.Add(&HD798, &HD2B)
                mRevPKMFont.Add(&HD799, &HD2C)
                mRevPKMFont.Add(&HD79B, &HD2D)
                mRevPKMFont.Add(&HD79D, &HD2E)
                mRevPKMFont.Add(&H1100, &HD31)
                mRevPKMFont.Add(&H1101, &HD32)
                mRevPKMFont.Add(&H1102, &HD33)
                mRevPKMFont.Add(&H1103, &HD34)
                mRevPKMFont.Add(&H1104, &HD35)
                mRevPKMFont.Add(&H1105, &HD36)
                mRevPKMFont.Add(&H1106, &HD37)
                mRevPKMFont.Add(&H1107, &HD38)
                mRevPKMFont.Add(&H1108, &HD39)
                mRevPKMFont.Add(&H1109, &HD3A)
                mRevPKMFont.Add(&H110A, &HD3B)
                mRevPKMFont.Add(&H110B, &HD3C)
                mRevPKMFont.Add(&H110C, &HD3D)
                mRevPKMFont.Add(&H110D, &HD3E)
                mRevPKMFont.Add(&H110E, &HD3F)
                mRevPKMFont.Add(&H110F, &HD40)
                mRevPKMFont.Add(&H1110, &HD41)
                mRevPKMFont.Add(&H1111, &HD42)
                mRevPKMFont.Add(&H1112, &HD43)
                mRevPKMFont.Add(&H1161, &HD44)
                mRevPKMFont.Add(&H1162, &HD45)
                mRevPKMFont.Add(&H1163, &HD46)
                mRevPKMFont.Add(&H1164, &HD47)
                mRevPKMFont.Add(&H1165, &HD48)
                mRevPKMFont.Add(&H1166, &HD49)
                mRevPKMFont.Add(&H1167, &HD4A)
                mRevPKMFont.Add(&H1168, &HD4B)
                mRevPKMFont.Add(&H1169, &HD4C)
                mRevPKMFont.Add(&H116D, &HD4D)
                mRevPKMFont.Add(&H116E, &HD4E)
                mRevPKMFont.Add(&H1172, &HD4F)
                mRevPKMFont.Add(&H1173, &HD50)
                mRevPKMFont.Add(&H1175, &HD51)
                mRevPKMFont.Add(&HB894, &HD61)
                mRevPKMFont.Add(&HC330, &HD62)
                mRevPKMFont.Add(&HC3BC, &HD63)
                mRevPKMFont.Add(&HC4D4, &HD64)
                mRevPKMFont.Add(&HCB2C, &HD65)

            Catch ex As Exception 'When 5
                'If Err.Number <> 5 Then Console.WriteLine(Err.Number)
            End Try
        End Sub

        Private Sub DictionaryDispose()
            For Each dPair As KeyValuePair(Of Integer, Integer) In mPKMFontConverter
                mPKMFontConverter.Remove(dPair.Key)
            Next

            For Each dPair As KeyValuePair(Of Integer, Integer) In mRevPKMFont
                mRevPKMFont.Remove(dPair.Key)
            Next

        End Sub

    End Class

    Public Class PokePRNG

        Public Sub New()
            m_seed = 0UI
        End Sub

        Public Sub New(ByVal _SEED As UInt32)
            m_seed = _SEED
        End Sub

        Private m_seed As UInt32

        Public Property Seed() As UInt32
            Get
                Return m_seed
            End Get
            Set(ByVal value As UInt32)
                m_seed = value
            End Set
        End Property

        Public Function Previous() As UInt32
            Return &HEEB9EB65 * m_seed + &HA3561A1
        End Function

        Public Function PreviousNum() As UInt32
            m_seed = Previous()
            Return m_seed
        End Function

        Public Function [Next]() As UInt32
            Return (&H41C64E6D * m_seed) + &H6073
        End Function

        Public Function NextNum() As UInt32
            m_seed = [Next]()
            Return m_seed
        End Function

    End Class

    Public Class FriendCodeManager

        'Credit to the following people:
        'http://www.caitsith2.com/ds/fc.php
        'SCV
        'damio

        Private InputVar As ULong
        Private magic As UInt32
        Private CRC_Table(255) As Byte
        Private game_code() As UInt32 = New UInt32() {&H41504100, &H41444100, &H43505500}
        Private lang() As Byte = New Byte() {&H44, &H45, &H46, &H49, &H4A, &H4B}

        Public Function ValidateFC(ByVal FriendCode As ULong) As Boolean
            InputVar = FriendCode
            If validate_code(game_code(1) Or lang(4)) Then
                Return True
            Else
                Return False
            End If
        End Function

        Private Function validate_code(ByVal magic) As Boolean
            Dim crc_data(7) As UShort
            crc_data(0) = InputVar >> 0 And &HFF
            crc_data(1) = InputVar >> 8 And &HFF
            crc_data(2) = InputVar >> 16 And &HFF
            crc_data(3) = InputVar >> 24 And &HFF
            crc_data(4) = magic >> 0 And &HFF
            crc_data(5) = magic >> 8 And &HFF
            crc_data(6) = magic >> 16 And &HFF
            crc_data(7) = magic >> 24 And &HFF

            Return (InputVar >> 32 = (calc_crc(0, crc_data, 8)) And &H7F)

        End Function

        Private Function calc_crc(ByVal initial As UShort, ByVal data() As UShort, ByVal length As UShort) As UShort
            Dim crc As UShort = initial
            For i As UInt32 = 0 To length - 1
                crc = crc Xor data(i)
                crc = crc And &HFF
                crc = CRC_Table(crc)
            Next
            Return crc
        End Function

        Private Sub gen_crc()
            Dim crctab As UInt32
            For i As UInt32 = 0 To 255
                crctab = i
                For j As UInt32 = 0 To 7
                    If crctab And &H80 Then
                        crctab = crctab << 1
                        crctab = crctab Xor 7
                    Else
                        crctab = crctab << 1
                    End If
                Next
                CRC_Table(i) = crctab And &HFF
            Next
        End Sub

        Public Sub New()
            gen_crc()
            magic = game_code(1) Or lang(4)
        End Sub

        Public Function FC_Checksum(ByVal data() As Byte) As Byte
            Dim crc_data(7) As UShort
            Dim _IPV(7) As Byte
            Array.Copy(data, 0, _IPV, 0, 4)
            InputVar = BitConverter.ToUInt64(_IPV, 0)
            crc_data(0) = InputVar >> 0 And &HFF
            crc_data(1) = InputVar >> 8 And &HFF
            crc_data(2) = InputVar >> 16 And &HFF
            crc_data(3) = InputVar >> 24 And &HFF
            crc_data(4) = magic >> 0 And &HFF
            crc_data(5) = magic >> 8 And &HFF
            crc_data(6) = magic >> 16 And &HFF
            crc_data(7) = magic >> 24 And &HFF
            Return (calc_crc(0, crc_data, 8)) And &H7F
        End Function

        Public Function FC_Checksum(ByVal data As UInt32) As Byte
            Return FC_Checksum(BitConverter.GetBytes(data))
        End Function

        Public Function GetFC(ByVal data As UInt32) As UInt64
            If data = 0UI Then Return 0UI
            Dim fcdata() As Byte = BitConverter.GetBytes(data)
            Dim fcOut(7) As Byte
            Array.Copy(fcdata, 0, fcOut, 0, 4)
            Dim FCM As New FriendCodeManager
            fcOut(4) = FCM.FC_Checksum(fcdata)
            Return BitConverter.ToUInt64(fcOut, 0)
        End Function

        Public Function GetFC(ByVal data() As Byte) As UInt64
            Return GetFC(BitConverter.ToUInt64(data, 0))
        End Function

    End Class

    Public Class PKMGenerator

        Public Enum Methods
            Method1 = 1
            Method2
            Method3
            Method4
        End Enum

        Public PRNG As New PokePRNG

        Public Sub New(ByVal Delay As UInt32)
            PRNG.Seed = GenerateSeed(Delay)
        End Sub

        Public Sub New(ByVal Delay As UInt32, ByVal _Date As Date)
            PRNG.Seed = GenerateSeed(Delay, _Date)
        End Sub

        Public Function GenerateSeed(ByVal Delay As UInt32) As UInt32
            With Now
                Return (((.Month() * .Day + .Minute() + .Second()) Mod &H100) * &H1000000) + (.Hour() * &H10000) + (.Year() - 2000 + Delay)
            End With
        End Function

        Public Function GenerateSeed(ByVal Delay As UInt32, ByVal _Date As Date) As UInt32
            With _Date
                Return (((.Month() * .Day + .Minute() + .Second()) Mod &H100) * &H1000000) + (.Hour() * &H10000) + (.Year() - 2000 + Delay)
            End With
        End Function

        Public Function GeneratePID(Optional ByVal Method3 As Boolean = False) As UInt32
            Dim RESULT1, RESULT2 As UInt32
            RESULT1 = PRNG.NextNum >> 16 And &HFFFF
            If Method3 Then PRNG.NextNum()
            RESULT2 = PRNG.NextNum >> 16 And &HFFFF
            'Return Convert.ToUInt32(Hex(RESULT2) & Hex(RESULT1), 16)
            Return (RESULT2 << 16) + RESULT1
        End Function

        Public Function GenerateIVs(Optional ByVal Method4 As Boolean = False) As UInt32 'As Byte()
            Dim RESULT1, RESULT2 As UInt32
            RESULT1 = PRNG.NextNum >> 16 And &HFFFF
            If Method4 Then PRNG.NextNum()
            RESULT2 = PRNG.NextNum >> 16 And &HFFFF
            'Return Convert.ToUInt32((DecToBin(RESULT2, 15) & DecToBin(RESULT1, 15)), 2)
            Return ((RESULT2 And &H7FFF) << 15) + (RESULT1 And &H7FFF)
        End Function

        Public Function Generate(ByVal SpeciesID As Species, _
ByVal Method As PKMGenerator.Methods, _
ByVal OTName As String, _
ByVal OTGender As Genders, _
ByVal OTID As UInt16, _
ByVal OTSID As UInt16, _
ByVal MetLevel As Byte, _
ByVal EggDate As Date, _
ByVal DPEggMet As DSLocations, _
ByVal PtEggMet As DSLocations, _
ByVal DPMet As DSLocations, _
ByVal PtMet As DSLocations, _
ByVal MetDate As Date, _
ByVal Hometown As Hometowns, _
ByVal Encounter As Encounters, _
ByVal Country As Countries, _
ByVal BallCaught As Balls, _
ByVal Move1 As Moves, _
ByVal Move2 As Moves, _
ByVal Move3 As Moves, _
ByVal Move4 As Moves) As Pokemon

            Dim theTrainer As New mTrainer(OTName, New mGender(OTGender), OTID, OTSID)
            Dim theOrigins As New mOrigins(MetLevel, New  _
                                           mEgg(EggDate, New mMet(DPEggMet, PtEggMet)), _
                                           New mMet(DPMet, PtMet), MetDate, New mmHometown(Hometown), _
                                           New mmCountry(Country))
            Dim theMoves(3) As mMoves
            theMoves(0) = New mMoves(Move1)
            theMoves(1) = New mMoves(Move2)
            theMoves(2) = New mMoves(Move3)
            theMoves(3) = New mMoves(Move4)

            For i As Integer = 0 To 3
                If theMoves(i).Value <> Moves.NOTHING Then
                    theMoves(i).TotalPP = theMoves(i).BasePP
                    theMoves(i).CurrentPP = theMoves(i).BasePP
                End If
            Next

            Return Generate(SpeciesID, Method, theTrainer, theOrigins, New mEncounters(Encounter), theMoves)

        End Function

        Public Function Generate(ByVal SpeciesID As Species, ByVal Method As Methods, _
                                 ByVal _Trainer As mTrainer, ByVal _Origins As mOrigins, _
                                 ByVal Encounter As mEncounters, ByVal _Moves As mMoves()) As Pokemon
            Dim pkmOUT As New Pokemon
            Select Case Method
                Case Methods.Method1
                    pkmOUT = Method1(SpeciesID, _Trainer, _Origins, _Moves)
                Case Methods.Method2
                    pkmOUT = Method2(SpeciesID, _Trainer, _Origins, _Moves)
                Case Methods.Method3
                    pkmOUT = Method3(SpeciesID, _Trainer, _Origins, _Moves)
                Case Methods.Method4
                    pkmOUT = Method4(SpeciesID, _Trainer, _Origins, _Moves)
                Case Else
                    Return New Pokemon
            End Select
            pkmOUT.Encounter = Encounter
            Return pkmOUT
        End Function

        Private Function Method1(ByVal SpeciesID As Species, ByVal _Trainer As mTrainer, ByVal _Origins As mOrigins, ByVal _Moves As mMoves()) As Pokemon
            Method1 = New Pokemon
            Method1.Species = New mSpecies(SpeciesID)
            Method1.PID = GeneratePID()
            Dim mIVsAndEtc As UInt32 = GenerateIVs()
            'Dim theIVs As New mIVs
            'With theIVs
            '    .HP = (mIVsAndEtc >> (0)) And &H1F
            '    .Attack = (mIVsAndEtc >> (5)) And &H1F
            '    .Defense = (mIVsAndEtc >> (10)) And &H1F
            '    .Speed = (mIVsAndEtc >> (15)) And &H1F
            '    .SpAttack = (mIVsAndEtc >> (20)) And &H1F
            '    .SpDefense = (mIVsAndEtc >> (25)) And &H1F
            'End With
            'Method1.IVs = theIVs
            Method1.mIVsAndEtc = mIVsAndEtc
            Return SetValues(Method1, _Trainer, _Origins, _Moves)
        End Function

        Private Function Method2(ByVal SpeciesID As Species, ByVal _Trainer As mTrainer, ByVal _Origins As mOrigins, ByVal _Moves As mMoves()) As Pokemon
            Method2 = New Pokemon
            Method2.Species = New mSpecies(SpeciesID)
            Method2.PID = GeneratePID()
            PRNG.NextNum()
            Dim mIVsAndEtc As UInt32 = GenerateIVs()
            'Dim theIVs As New mIVs
            'With theIVs
            '    .HP = (mIVsAndEtc >> (0)) And &H1F
            '    .Attack = (mIVsAndEtc >> (5)) And &H1F
            '    .Defense = (mIVsAndEtc >> (10)) And &H1F
            '    .Speed = (mIVsAndEtc >> (15)) And &H1F
            '    .SpAttack = (mIVsAndEtc >> (20)) And &H1F
            '    .SpDefense = (mIVsAndEtc >> (25)) And &H1F
            'End With
            'Method2.IVs = theIVs
            Method2.mIVsAndEtc = mIVsAndEtc
            Return SetValues(Method2, _Trainer, _Origins, _Moves)
        End Function

        Private Function Method3(ByVal SpeciesID As Species, ByVal _Trainer As mTrainer, ByVal _Origins As mOrigins, ByVal _Moves As mMoves()) As Pokemon
            Method3 = New Pokemon
            Method3.Species = New mSpecies(SpeciesID)
            Method3.PID = GeneratePID(True)
            Dim mIVsAndEtc As UInt32 = GenerateIVs()
            'Dim theIVs As New mIVs
            'With theIVs
            '    .HP = (mIVsAndEtc >> (0)) And &H1F
            '    .Attack = (mIVsAndEtc >> (5)) And &H1F
            '    .Defense = (mIVsAndEtc >> (10)) And &H1F
            '    .Speed = (mIVsAndEtc >> (15)) And &H1F
            '    .SpAttack = (mIVsAndEtc >> (20)) And &H1F
            '    .SpDefense = (mIVsAndEtc >> (25)) And &H1F
            'End With
            'Method3.IVs = theIVs
            Method3.mIVsAndEtc = mIVsAndEtc
            Return SetValues(Method3, _Trainer, _Origins, _Moves)
        End Function

        Private Function Method4(ByVal SpeciesID As Species, ByVal _Trainer As mTrainer, ByVal _Origins As mOrigins, ByVal _Moves As mMoves()) As Pokemon
            Method4 = New Pokemon
            Method4.Species = New mSpecies(SpeciesID)
            Method4.PID = GeneratePID()
            Dim mIVsAndEtc As UInt32 = GenerateIVs(True)
            'Dim theIVs As New mIVs
            'With theIVs
            '    .HP = (mIVsAndEtc >> (0)) And &H1F
            '    .Attack = (mIVsAndEtc >> (5)) And &H1F
            '    .Defense = (mIVsAndEtc >> (10)) And &H1F
            '    .Speed = (mIVsAndEtc >> (15)) And &H1F
            '    .SpAttack = (mIVsAndEtc >> (20)) And &H1F
            '    .SpDefense = (mIVsAndEtc >> (25)) And &H1F
            'End With
            'Method4.IVs = theIVs
            Method4.mIVsAndEtc = mIVsAndEtc
            Return SetValues(Method4, _Trainer, _Origins, _Moves)
        End Function

        Public Function SetValues(ByVal PKM As Pokemon, ByVal _Trainer As mTrainer, ByVal _Origins As mOrigins, ByVal _Moves As mMoves()) As Pokemon
            With PKM
                Try
                    'TODO: Set values for generated PKM.
                    If .Class = 0 Or .BaseStats.Ability2 = 0 Then
                        .Ability = New mAbility(.BaseStats.Ability1)
                    Else
                        .Ability = New mAbility(.BaseStats.Ability2)
                    End If

                    If .BaseStats.Gender = 255 Then
                        .Gender = New mGender(Genders.Genderless)
                    ElseIf .BaseStats.Gender = 254 Then
                        .Gender = New mGender(Genders.Female)
                    ElseIf .BaseStats.Gender = 0 Then
                        .Gender = New mGender(Genders.Male)
                    Else

                        If (.PID Mod 256) > .BaseStats.Gender Then
                            .Gender = New mGender(Genders.Male)
                        Else
                            .Gender = New mGender(Genders.Female)
                        End If
                    End If

                    .ClearMarks()
                    .EXP = 0
                    .FatefulEncounter = False
                    .Forme = 0
                    .IsEgg = False
                    .Item = New mItems(Items.NOTHING)

                    'Dim theMoves(3) As mMoves
                    'theMoves(0) = New mMoves(Moves.NOTHING)
                    'theMoves(1) = New mMoves(Moves.NOTHING)
                    'theMoves(2) = New mMoves(Moves.NOTHING)
                    'theMoves(3) = New mMoves(Moves.NOTHING)
                    .Moves = _Moves

                    .Nickname = UCase(.Species.Name)
                    .Nicknamed = False

                    .Origins = _Origins
                    PKM.SetEggDateBytes(New Byte() {0, 0, 0})

                    .Tameness = 70
                    .Trainer = _Trainer

                    .Recalculate()

                Catch ex As Exception

                End Try
            End With
            Return PKM
        End Function

#Region "Copyright Notice"
        '
        ' * This file is part of RNG Reporter
        ' * Copyright (C) 2009 by Bill Young
        ' *
        ' * This program is free software; you can redistribute it and/or
        ' * modify it under the terms of the GNU General Public License
        ' * as published by the Free Software Foundation; either version 2
        ' * of the License, or (at your option) any later version.
        ' *
        ' * This program is distributed in the hope that it will be useful,
        ' * but WITHOUT ANY WARRANTY; without even the implied warranty of
        ' * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
        ' * GNU General Public License for more details.
        ' *
        ' * You should have received a copy of the GNU General Public License
        ' * along with this program; if not, write to the Free Software
        ' * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
        '
#End Region

        Public Enum FrameTypes
            Method1 = 1
            Method2
            Method3
            Method4
        End Enum

        Public Class seed
            Private mframe As FrameTypes
            Public PID As UInt32
            Public MonsterSeed As UInt32
            Public SID As UInt16

            Public Property FrameType() As FrameTypes
                Get
                    Return mframe
                End Get
                Set(ByVal value As FrameTypes)
                    mframe = value
                End Set
            End Property

        End Class

        ''' <summary>
        ''' This class is going to do an IV/PID/Seed calculation given a particular
        ''' method (1, 2 or 3, or 4).  Should use the same code to develop canidate IVs.
        ''' </summary>
        Class IVtoSeed
            '  We need a function to return a list of monster seeds,
            '  which will be updated to include a method.

            Public Shared Function GetSeeds(ByVal IVs As PokemonLib.mIVs, ByVal Nature As PokemonLib.Natures, ByVal ID As UInt16) As List(Of seed)

                Dim hp, atk, def, spa, spd, spe As UInteger

                With IVs
                    hp = .HP
                    atk = .Attack
                    def = .Defense
                    spa = .SpAttack
                    spd = .SpDefense
                    spe = .Speed
                End With

                Dim seeds As New List(Of seed)()

                Dim x4 As UInteger = 0
                Dim x4_2 As UInteger = 0

                x4 = spe + (spa << 5) + (spd << 10)
                x4_2 = x4 Xor &H8000

                '  Now we want to start with IV2 and call the RNG for
                '  values between 0 and FFFF in the low order bits.
                For cnt As UInteger = 0 To &H1FFFE
                    Dim x_test As UInteger

                    '  We want to test with the high bit
                    '  both set and not set, so we're going
                    '  to sneakily do them both.  god help
                    '  me if i ever have to figure this out
                    '  in the future.
                    If cnt Mod 2 = 0 Then
                        x_test = x4
                    Else
                        x_test = x4_2
                    End If

                    '  Set our test seed here so we can start
                    '  working backwards to see if the rest
                    '  of the information we were provided
                    '  is a match.

                    Dim seed As UInteger = (x_test << 16) + (cnt Mod &HFFFF)
                    Dim rng As New PokemonDSLib.PokemonLib.PokePRNG 'LCRNGR(seed)
                    rng.Seed = seed

                    '  Right now, this simply assumes method
                    '  1 and gets the value previous to check
                    '  for  match.  We need a clean way to do
                    '  this for all of our methods.

                    '  We have a max of 5 total RNG calls
                    '  to make a pokemon and we already have
                    '  one so lets go ahead and get 4 more.
                    Dim rng1 As UShort = rng.PreviousNum >> 16 And &HFFFF 'rng.GetPrevious16BitNumber()
                    Dim rng2 As UShort = rng.PreviousNum >> 16 And &HFFFF 'rng.GetPrevious16BitNumber()
                    Dim rng3 As UShort = rng.PreviousNum >> 16 And &HFFFF 'rng.GetPrevious16BitNumber()
                    Dim rng4 As UShort = rng.PreviousNum >> 16 And &HFFFF 'rng.GetPrevious16BitNumber()

                    Dim method1Seed As UInteger = rng.Seed
                    rng.PreviousNum()
                    Dim method234Seed As UInteger = rng.Seed

                    '  Check Method 1
                    ' [PID] [PID] [IVs] [IVs]
                    ' [rng3] [rng2] [rng1] [START]
                    If Check(rng1, rng2, rng3, hp, atk, def, _
                     Nature) Then
                        '  Build a seed to add to our collection
                        Dim newSeed As New seed()
                        newSeed.FrameType = FrameTypes.Method1
                        newSeed.PID = (CUInt(rng2) << 16) + CUInt(rng3)
                        newSeed.MonsterSeed = method1Seed
                        newSeed.SID = (CUInt(rng2) Xor CUInt(rng3) Xor ID) And &HFFF8

                        seeds.Add(newSeed)
                    End If

                    '  Check Method 2
                    ' [PID] [PID] [xxxx] [IVs] [IVs]
                    ' [rng4] [rng3] [xxxx] [rng1] [START]
                    If Check(rng1, rng3, rng4, hp, atk, def, _
                     Nature) Then
                        '  Build a seed to add to our collection
                        Dim newSeed As New seed()
                        newSeed.FrameType = FrameTypes.Method2
                        newSeed.PID = (CUInt(rng3) << 16) + CUInt(rng4)
                        newSeed.MonsterSeed = method234Seed
                        newSeed.SID = (CUInt(rng3) Xor CUInt(rng4) Xor ID) And &HFFF8

                        seeds.Add(newSeed)
                    End If

                    '  Check Method 3
                    '  [PID] [xxxx] [PID] [IVs] [IVs]
                    '  [rng4] [xxxx] [rng2] [rng1] [START]
                    If Check(rng1, rng2, rng4, hp, atk, def, _
                     Nature) Then
                        '  Build a seed to add to our collection
                        Dim newSeed As New seed()
                        newSeed.FrameType = FrameTypes.Method3
                        newSeed.PID = (CUInt(rng2) << 16) + CUInt(rng4)
                        newSeed.MonsterSeed = method234Seed
                        newSeed.SID = (CUInt(rng2) Xor CUInt(rng4) Xor ID) And &HFFF8

                        seeds.Add(newSeed)
                    End If

                    '  Check Method 4
                    '  [PID] [PID] [IVs] [xxxx] [IVs]
                    '  [rng4] [rng3] [rng2] [xxxx] [START]
                    If Check(rng2, rng3, rng4, hp, atk, def, _
                     Nature) Then
                        '  Build a seed to add to our collection
                        Dim newSeed As New seed()
                        newSeed.FrameType = FrameTypes.Method4
                        newSeed.PID = (CUInt(rng3) << 16) + CUInt(rng4)
                        newSeed.MonsterSeed = method234Seed
                        newSeed.SID = (CUInt(rng3) Xor CUInt(rng4) Xor ID) And &HFFF8

                        seeds.Add(newSeed)
                    End If
                Next

                Return seeds
            End Function

            Public Shared Function GetSeeds(ByVal hp As UInteger, ByVal atk As UInteger, ByVal def As UInteger, _
                                            ByVal spa As UInteger, ByVal spd As UInteger, ByVal spe As UInteger, _
                                            ByVal nature As UInteger, ByVal id As UInteger) As List(Of seed)
                Dim seeds As New List(Of seed)()

                Dim x4 As UInteger = 0
                Dim x4_2 As UInteger = 0

                x4 = spe + (spa << 5) + (spd << 10)
                x4_2 = x4 Xor &H8000

                '  Now we want to start with IV2 and call the RNG for
                '  values between 0 and FFFF in the low order bits.
                For cnt As UInteger = 0 To &H1FFFE
                    Dim x_test As UInteger

                    '  We want to test with the high bit
                    '  both set and not set, so we're going
                    '  to sneakily do them both.  god help
                    '  me if i ever have to figure this out
                    '  in the future.
                    If cnt Mod 2 = 0 Then
                        x_test = x4
                    Else
                        x_test = x4_2
                    End If

                    '  Set our test seed here so we can start
                    '  working backwards to see if the rest
                    '  of the information we were provided
                    '  is a match.

                    Dim seed As UInteger = (x_test << 16) + (cnt Mod &HFFFF)
                    Dim rng As New PokemonDSLib.PokemonLib.PokePRNG 'LCRNGR(seed)
                    rng.Seed = seed

                    '  Right now, this simply assumes method
                    '  1 and gets the value previous to check
                    '  for  match.  We need a clean way to do
                    '  this for all of our methods.

                    '  We have a max of 5 total RNG calls
                    '  to make a pokemon and we already have
                    '  one so lets go ahead and get 4 more.
                    Dim rng1 As UShort = rng.PreviousNum >> 16 And &HFFFF 'rng.GetPrevious16BitNumber()
                    Dim rng2 As UShort = rng.PreviousNum >> 16 And &HFFFF 'rng.GetPrevious16BitNumber()
                    Dim rng3 As UShort = rng.PreviousNum >> 16 And &HFFFF 'rng.GetPrevious16BitNumber()
                    Dim rng4 As UShort = rng.PreviousNum >> 16 And &HFFFF 'rng.GetPrevious16BitNumber()

                    Dim method1Seed As UInteger = rng.Seed
                    rng.PreviousNum()
                    Dim method234Seed As UInteger = rng.Seed

                    '  Check Method 1
                    ' [PID] [PID] [IVs] [IVs]
                    ' [rng3] [rng2] [rng1] [START]
                    If Check(rng1, rng2, rng3, hp, atk, def, _
                     nature) Then
                        '  Build a seed to add to our collection
                        Dim newSeed As New seed()
                        newSeed.FrameType = FrameTypes.Method1
                        newSeed.PID = (CUInt(rng2) << 16) + CUInt(rng3)
                        newSeed.MonsterSeed = method1Seed
                        newSeed.SID = (CUInt(rng2) Xor CUInt(rng3) Xor id) And &HFFF8

                        seeds.Add(newSeed)
                    End If

                    '  Check Method 2
                    ' [PID] [PID] [xxxx] [IVs] [IVs]
                    ' [rng4] [rng3] [xxxx] [rng1] [START]
                    If Check(rng1, rng3, rng4, hp, atk, def, _
                     nature) Then
                        '  Build a seed to add to our collection
                        Dim newSeed As New seed()
                        newSeed.FrameType = FrameTypes.Method2
                        newSeed.PID = (CUInt(rng3) << 16) + CUInt(rng4)
                        newSeed.MonsterSeed = method234Seed
                        newSeed.SID = (CUInt(rng3) Xor CUInt(rng4) Xor id) And &HFFF8

                        seeds.Add(newSeed)
                    End If

                    '  Check Method 3
                    '  [PID] [xxxx] [PID] [IVs] [IVs]
                    '  [rng4] [xxxx] [rng2] [rng1] [START]
                    If Check(rng1, rng2, rng4, hp, atk, def, _
                     nature) Then
                        '  Build a seed to add to our collection
                        Dim newSeed As New seed()
                        newSeed.FrameType = FrameTypes.Method3
                        newSeed.PID = (CUInt(rng2) << 16) + CUInt(rng4)
                        newSeed.MonsterSeed = method234Seed
                        newSeed.SID = (CUInt(rng2) Xor CUInt(rng4) Xor id) And &HFFF8

                        seeds.Add(newSeed)
                    End If

                    '  Check Method 4
                    '  [PID] [PID] [IVs] [xxxx] [IVs]
                    '  [rng4] [rng3] [rng2] [xxxx] [START]
                    If Check(rng2, rng3, rng4, hp, atk, def, _
                     nature) Then
                        '  Build a seed to add to our collection
                        Dim newSeed As New seed()
                        newSeed.FrameType = FrameTypes.Method4
                        newSeed.PID = (CUInt(rng3) << 16) + CUInt(rng4)
                        newSeed.MonsterSeed = method234Seed
                        newSeed.SID = (CUInt(rng3) Xor CUInt(rng4) Xor id) And &HFFF8

                        seeds.Add(newSeed)
                    End If
                Next

                Return seeds
            End Function

            Public Shared Function Check(ByVal iv As UShort, ByVal pid2 As UShort, ByVal pid1 As UShort, ByVal hp As UInteger, ByVal atk As UInteger, ByVal def As UInteger, _
             ByVal nature As UInteger) As Boolean
                Dim ret As Boolean = False

                Dim test_hp As UInteger = CUInt(iv) And &H1F
                Dim test_atk As UInteger = (CUInt(iv) And &H3E0) >> 5
                Dim test_def As UInteger = (CUInt(iv) And &H7C00) >> 10

                If test_hp = hp AndAlso test_atk = atk AndAlso test_def = def Then

                    '  Use these two values to see if we have a possible
                    '  match for the nature of this pokemon.  Also, if
                    '  we have a match then the RNG will contain a
                    '  seeding possibility.

                    Dim pid As UInteger = (CUInt(pid2) << 16) + CUInt(pid1)

                    Dim pidNature As UInteger = pid Mod 25

                    '  Do a nature comparison with what we have selected
                    '  in the dropdown and if we have a good match we can
                    '  go ahead and add this to our starting seeds.
                    If nature = pidNature Then
                        ret = True
                    End If
                End If

                Return ret
            End Function
            ' bool Check(ushort iv, short pid2, ushort pid1)
        End Class

    End Class

#End Region

End Class