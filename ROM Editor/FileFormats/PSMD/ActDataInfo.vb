Imports SkyEditorBase
Imports SkyEditorBase.Interfaces

Namespace FileFormats.PSMD
    Public Class ActDataInfo
        Implements iOpenableFile
        Public Class ActDataInfoEntry
            Public Property EffectRate As UInt16
            Public Property HPBellyChangeValue As UInt16
            Public Property TrapFlag As Boolean
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks>Status list:
            ''' 0    None
            '''         1    nightmare
            '''         2    sleepless
            '''         3    sleep
            '''         4    yawning
            '''         5    napping
            '''         6    
            '''         7    burn
            '''         8    poisoned
            '''         9    badly poisoned
            '''         10    paralysis
            '''         11    frozen
            '''         12    stuck
            '''         13    Ingrain
            '''         14    petrified
            '''         15    bound
            '''         16    Fire Spin
            '''         17    Sand Tomb
            '''         18    Clamp
            '''         19    Whirlpool
            '''         20    Magma Storm
            '''         21    Infestation
            '''         22    flinch
            '''         23    truant
            '''         24    confused
            '''         25    recoil
            '''         26    lethargic
            '''         27    infatuated
            '''         28    taunted
            '''         29    encore
            '''         30    discord
            '''         31    Bide
            '''         32    Solar Beam
            '''         33    Focus Punch
            '''         34    flying
            '''         35    bouncing
            '''         36    digging
            '''         37    Future Sight
            '''         38    Sky Drop
            '''         39    Suspended
            '''         40    charging
            '''         41    enraged
            '''         42    Round
            '''         43    Razor Wind
            '''         44    Sky Attack
            '''         45    Skull Bash
            '''         46    Doom Desire
            '''         47    Dive
            '''         48    Shadow Force
            '''         49    Phantom Force
            '''         50    Ice Burn
            '''         51    Freeze Shock
            '''         52    Geomancy
            '''         53    Reflect
            '''         54    Safeguard
            '''         55    Light Screen
            '''         56    Counter
            '''         57    protect
            '''         58    Quick Guard
            '''         59    Wide Guard
            '''         60    Mat Block
            '''         61    Crafty Shield
            '''         62    King's Shield
            '''         63    Spiky Shield
            '''         64    Mirror Coat
            '''         65    enduring
            '''         66    mist
            '''         67    healthy
            '''         68    Aqua Ring
            '''         69    Lucky Chant
            '''         70    Metal Burst
            '''         71    Magic Coat
            '''         72    Snatch
            '''         73    cursed
            '''         74    substitute
            '''         75    decoy
            '''         76    Gastro Acid
            '''         77    Heal Block
            '''         78    embargo
            '''         79    Leech Seed
            '''         80    Destiny Bond
            '''         81    Sure Shot
            '''         82    Focus Energy
            '''         83    encouraged
            '''         84    pierce
            '''         85    blinker
            '''         86    Eyedrops
            '''         87    mobile
            '''         88    trained
            '''         89    stockpiling
            '''         90    radar
            '''         91    scanning
            '''         92    grudge
            '''         93    exposed
            '''         94    Miracle Eye
            '''         95    terrified
            '''         96    grounded
            '''         97    Magnet Rise
            '''         98    telekinesis
            '''         99    autotomize
            '''         100    sealed
            '''         101    Perish Song
            '''         102    wish
            '''         103    transform
            '''         104    electrify
            '''         105    powder
            '''         106    puppet
            '''         107    awakened
            '''         108    berserk
            '''         109    
            '''         110    self-destruct
            '''         111    explosion
            '''         112    
            '''         113    
            '''         114    
            '''         115    
            '''         116    
            '''         117    
            '''         118    
            '''         119    
            '''         120    
            '''         121    
            '''         122    
            '''         123    
            '''         124    
            '''         125    
            '''         126    
            '''         127    
            '''         128    fainted
            '''         129    
            '''         130    
            '''         131    
            '''         132    
            '''         133    
            '''         134    
            '''         135    
            '''         136    
            '''         137    
            '''         138    
            '''         139    
            '''         140    
            '''         141    
            '''         142    
            '''         143    
            '''         144    
            '''         145    
            '''         146    
            '''         147    
            '''         148    
            '''         149    
            '''         150    
            '''         151    
            '''         152    
            '''         153    
            '''         154    
            '''         155    
            '''         156    
            '''         157    
            '''         158    
            '''         159     
            '''         160    rainbow
            '''         161    swamp
            '''         162    sea of fire</remarks>
            Public Property StatusChange As UInt16
            Public Property StatChangeIndex As UInt16
            Public Property TypeChange As UInt16
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks>Terrain List:
            '''         0    None
            '''         1    Clear
            '''         2    Sunny
            '''         3    Rain
            '''         4    Hail
            '''         5    Sandstorm
            '''         6    Mud Sport
            '''         7    Water Sport
            '''         8    Defused
            '''         9    Thief Alert
            '''         10    Trick Room
            '''         11    Magic Room
            '''         12    Monster House
            '''         13    Shop
            '''         14    Nullified
            '''         15    Luminous
            '''         16     
            '''         17    heavy rain
            '''         18    harsh sunlight
            '''         19    strong winds
            '''         20    ineffective weather
            '''         21    gravity
            '''         22    Wonder Room
            '''         23    Ion Deluge
            '''         24    Grassy Terrain
            '''         25    Misty Terrain
            '''         26    Electric Terrain
            '''         27    Happy Hour
            '''         28    apathetic
            '''         29    Enemy Discord
            '''         30    disabled</remarks>
            Public Property TerrainChange As UInt16

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks>Value should be a percentage from 0 to 100, but 101 is a sure shot</remarks>
            Public Property BaseAccuracy As Byte
            ''' <summary>
            ''' 
            ''' </summary>
            ''' <returns></returns>
            ''' <remarks>Value should be a percentage from 0 to 100, but 101 is a sure shot</remarks>
            Public Property MaxAccuracy As Byte

            Public Property SizeTypeMove As Byte
            ''' <summary>
            ''' The type of the move.  (1: Normal, 19: No Type)
            ''' </summary>
            ''' <returns></returns>
            Public Property TypeID As Byte
            Public Property Attribute As Byte
            Public Property BaseDamage As Byte
            Public Property MaxDamage As Byte
            Public Property BasePP As Byte
            Public Property MaxPP As Byte
            Public Property CutsCorners As Boolean
            Public Property MoreTimeToAttack As Boolean
            Public Property TilesCount As Byte
            Public Property Range As Byte
            Public Property Target As Byte
            Public Property PiercingAttack As Boolean
            Public Property SleepAttack As Boolean
            Public Property FaintAttack As Boolean
            Public Property NearbyDamage As Boolean
            Public Property HitCounterIndex As Byte

            Public Sub New(RawData As Byte())
                EffectRate = BitConverter.ToUInt16(RawData, &H28)
                HPBellyChangeValue = BitConverter.ToUInt16(RawData, &H2A)
                TrapFlag = BitConverter.ToUInt16(RawData, &H2E)
                StatusChange = BitConverter.ToUInt16(RawData, &H30)
                StatChangeIndex = BitConverter.ToUInt16(RawData, &H32)
                TypeChange = BitConverter.ToUInt16(RawData, &H34)
                TerrainChange = BitConverter.ToUInt16(RawData, &H36)

                BaseAccuracy = RawData(&H58)
                MaxAccuracy = RawData(&H5A)

                SizeTypeMove = RawData(&H7C)
                TypeID = RawData(&H7D)
                Attribute = RawData(&H7E)
                BaseDamage = RawData(&H7F)
                MaxDamage = RawData(&H80)
                BasePP = RawData(&H81)
                MaxPP = RawData(&H82)
                CutsCorners = (RawData(&H84 = 1))
                MoreTimeToAttack = (RawData(&H85) = 1)
                TilesCount = RawData(&H88)
                Range = RawData(&H8C)
                Target = RawData(&H8D)
                PiercingAttack = (RawData(&H8E) = 1)
                SleepAttack = (RawData(&H8F) = 1)
                FaintAttack = (RawData(&H90) = 1)
                NearbyDamage = (RawData(&H92) = 1)
                HitCounterIndex = RawData(&H93)
            End Sub
        End Class

        Public Property Entries As List(Of ActDataInfoEntry)

        Public Sub OpenFile(Filename As String) Implements iOpenableFile.OpenFile
            Const entryLength = 160
            Using f As New GenericFile
                f.EnableInMemoryLoad = True
                f.OpenFile(Filename)

                For count = 0 To ((f.Length / entryLength) - 1)
                    Entries.Add(New ActDataInfoEntry(f.RawData(count * entryLength, entryLength)))
                Next
            End Using
        End Sub

        Public Sub New()
            Entries = New List(Of ActDataInfoEntry)
        End Sub
    End Class
End Namespace

