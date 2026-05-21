// 
// This is part of Vortex Tracker II project
// 
// (c)2000-2009 S.V.Bulba
// Author: Sergey Bulba, vorobey@mail.khstu.ru
// Support page: http://bulba.untergrund.net/
// 
// Version 1.5 - 2.6
// (c)2017-2021 Ivan Pirog, ivan.pirog@gmail.com
// 
// Version 2.6.1
// (c)2022-2025 Dexus (Volutar), https://github.com/Volutar
// 
// Version 3.0+ (C# port)
// (c)2025 Ben Baker, https://github.com/benbaker76

using System.Runtime.InteropServices;
using System.Text;

namespace LibVT
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FamiRow
    {
        public byte Note;
        public byte Octave;
        public byte NoteVolume;
        public byte Instrument;
        public byte Fx1Cmd;
        public byte Fx2Cmd;
        public byte Fx3Cmd;
        public byte Fx4Cmd;
        public byte Fx1Prm;
        public byte Fx2Prm;
        public byte Fx3Prm;
        public byte Fx4Prm;

        public static FamiRow Read(BinaryReader reader)
        {
            FamiRow row = new FamiRow
            {
                Note = reader.ReadByte(),
                Octave = reader.ReadByte(),
                NoteVolume = reader.ReadByte(),
                Instrument = reader.ReadByte(),
                Fx1Cmd = reader.ReadByte(),
                Fx2Cmd = reader.ReadByte(),
                Fx3Cmd = reader.ReadByte(),
                Fx4Cmd = reader.ReadByte(),
                Fx1Prm = reader.ReadByte(),
                Fx2Prm = reader.ReadByte(),
                Fx3Prm = reader.ReadByte(),
                Fx4Prm = reader.ReadByte()
            };
            return row;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FamiTrackerBufferHeader
    {
        public int Channels;
        public int Rows;
        public int SelectStart;
        public int SelectEnd;
        public int Undefined1;
        public int Undefined2;
    }

    // FamiTracker Clipboard
    public class FamiTrackerBuffer
    {
        public FamiTrackerBufferHeader Header;
        public FamiRow[] Data;
    }

    public class LastNoteParams
    {
        public byte Line =0;     // 0..255
        public byte Sample=0;   // 0..31
        public byte Ornament=0; // 0..15
        public sbyte Volume=0;  // 1-15 - vol, 0 - prev vol
        public byte Envelope=0;
    }

    public class PatternTriple
    {
        public ushort PatternA;
        public ushort PatternB;
        public ushort PatternC;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SongData
    {
        public byte ChanA;
        public byte ChanB;
        public byte ChanC;
        public byte Noise;
        public ushort SongLength;
        public ushort FadeLength;
        public byte HiReg;
        public byte LoReg;
        public short PPoints;
        public short PAddresses;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Points
    {
        public ushort Stek;
        public ushort Init;
        public ushort Inter;
        public ushort Adr1;
        public ushort Len1;
        public ushort Offs1;
        public ushort Adr2;
        public ushort Len2;
        public ushort Offs2;
        public ushort Zero;
    }

    public class ChannelParams
    {
        public byte SamplePosition;
        public byte SamplePrevPosition;
        public byte OrnamentPosition;
        public bool SoundEnabled;
        public byte SlideToNote;
        public byte Note;
        public sbyte ToneSlideDelay;
        public sbyte ToneSlideCount;
        public short ToneSlideStep;
        public short ToneSlideDelta;
        public int ToneSlideType;
        public short CurrentToneSliding;
        public sbyte OnOffDelay;
        public sbyte OffOnDelay;
        public sbyte CurrentOnOff;
        public ushort Tone;
        public ushort ToneAccumulator;
        public byte Amplitude;
        public sbyte CurrentAmplitudeSliding;
        public sbyte CurrentEnvelopeSliding;
        public sbyte CurrentNoiseSliding;
    }

    public class PlayArgs
    {
        public int PositionIndex;
        public int PatternIndex;
        public int LineIndex;
        public short EnvBase;
        public ChannelParams[] ChannelParams = new ChannelParams[3];
        public sbyte Delay;
        public sbyte DelayCount;
        public short CurEnvSlide;
        public sbyte CurEnvDelay;
        public sbyte EnvDelay;
        public short EnvSlideAdd;
        public sbyte AddToEnv;
        public sbyte AddToNoise;
        public byte PT3Noise;
        public int IntCount;
        // MIDI output
        public int[] LastNoteSent = new int[3];     // Store MIDI note numbers
        public bool[] NoteOnState = new bool[3];    // Whether note is currently on

        public PlayArgs()
        {
            Array.Fill(LastNoteSent, -1);
            Array.Fill(NoteOnState, false);

            for (int j = 0; j < 3; j++)
                ChannelParams[j] = new ChannelParams();
        }
    }

    public class PlaybackEventArgs : EventArgs
    {
        public int ChipIndex { get; }
        
        // Immutable snapshots (copied by value)
        public AYRegisters AYRegisters { get; }
        public byte[] ChannelNotes { get; }
        public ushort[] ChannelTones { get; }
        public bool[] ChannelSoundEnabled { get; }
        

        public PlaybackEventArgs(int chipIndex, SoundChip soundChip, PlayArgs playArgs)
        {
            ChipIndex = chipIndex;
            
            // Copy AY registers by value (struct copy)
            AYRegisters = new AYRegisters
            {
                ToneA = soundChip.AYRegisters.ToneA,
                ToneB = soundChip.AYRegisters.ToneB,
                ToneC = soundChip.AYRegisters.ToneC,
                Noise = soundChip.AYRegisters.Noise,
                Mixer = soundChip.AYRegisters.Mixer,
                AmplitudeA = soundChip.AYRegisters.AmplitudeA,
                AmplitudeB = soundChip.AYRegisters.AmplitudeB,
                AmplitudeC = soundChip.AYRegisters.AmplitudeC,
                Envelope = soundChip.AYRegisters.Envelope,
                EnvType = soundChip.AYRegisters.EnvType
            };
            
            // Copy channel data (small arrays, fast copy)
            ChannelNotes = new byte[3];
            ChannelTones = new ushort[3];
            ChannelSoundEnabled = new bool[3];
            
            for (int i = 0; i < 3; i++)
            {
                ChannelNotes[i] = playArgs.ChannelParams[i].Note;
                ChannelTones[i] = playArgs.ChannelParams[i].Tone;
                ChannelSoundEnabled[i] = playArgs.ChannelParams[i].SoundEnabled;
            }
        }

        public override string ToString() => $"ChipIndex={ChipIndex}";
    }

    public enum PlayLineResult : int
    {
        Normal,
        LineEnded,
        PatternEnded,
        AllPatternsEnded
    }

    public enum FeaturesLevel : int
    {
        PT35 = 0,      // Pro Tracker 3.5 and older
        VTII_PT36 = 1, // Vortex Tracker II and Pro Tracker 3.6 (Correct3xxxInterpretation)
        PT37 = 2,      // Pro Tracker 3.7 (New1xxx2xxxInterpretation)
        AutoDetect,
    }

    public enum ModuleType
    {
        Unknown,
        STCFile,
        ASCFile,
        ASC0File,
        STPFile,
        PSCFile,
        FLSFile,
        FTCFile,
        PT1File,
        PT2File,
        PT3File,
        SQTFile,
        GTRFile,
        FXMFile,
        PSMFile
    }

    public partial class VTModule
    {
        //public static int FamiClipboardType = 0;
        public static string FamiClipboardFormatName = "FamiTracker Pattern";
        public static bool UnlimitedDelay = false;
        public static int CenterChannel = 1;
        public static int ChipIndex = 0;
        public static PlayArgs[] PlayArgs = new PlayArgs[MaxSoundChipCount];
        public static VTM VTM;
        public static int[] StdChns = {0, 1, 2}; // ABC (for export to text/clipboard)
        public const int MaxPatternLength = 256; // Pro Tracker 3.xx allows 64 max :(
        public const int DefaultPatternLength = 64;
        public const int MaxPatternIndex = 84; // Alone Coder had made Pro Tracker 3.6x+ with 48 patterns
                                               // and with player, which supports up to 85 patterns
        public const int MaxPatternCount = MaxPatternIndex + 1;
        public const int MaxOrnamentLength = 255; // can be up to 255; 64 in ZX version of PT3-editor
        public const int MaxSampleLength = 64;    // not bigger than 64 (players limitation)
        public const int MaxPositionIndex = 255;  // max positions in track
        public const int MaxPositionCount = MaxPositionIndex + 1;
        public const int NoteCount = 96;          // 8 octaves * 12 semitones
        public const int PreviewSampleIndex = 32;
        public const int PreviewOrnamentIndex = 32;
        public static FeaturesLevel FeaturesLevel = FeaturesLevel.VTII_PT36;
        // 0 for PT 3.5 and older
        // 1 for VT II and PT3.6 (Correct3xxxInterpretation - Allows Vortex Tracker II rightly do the situation like
        // A-4 .... 11.1
        // --- .... ....
        // A-4 .... 31.1
        // An ASC modules conversion also depends on this switch)
        // 2 for PT 3.7 (New1xxx2xxxInterpretation - PT 3.7 1.xx and 2.xx allows single tone offset without glissando)
        public static bool DetectFeaturesLevel = true;
        public static bool VortexModuleHeader = true;
        public static bool DetectModuleHeader = true;
        public const int MaxSoundChipCount = 3;
        
                public static string[] FamiNotes = { "C-", "C#", "D-", "D#", "E-", "F-", "F#", "G-", "G#", "A-", "A#", "B-" };
        public const string KsaId = "KSA SOFTWARE COMPILATION OF ";
        public static string[] Notes = { "C-", "C#", "D-", "D#", "E-", "F-", "F#", "G-", "G#", "A-", "A#", "B-" };
        public static string[] NoteTableNames =
        {
            "ProTracker 3.3",
            "Sound Tracker",
            "ASM or PSC (1.75 MHz)",
            "RealSound",
            "IvanRochin NATURAL Cmaj/Am",
            "Custom"
        };
        
        // Table #0 of Pro Tracker 3.4x - 3.5x
        public static ushort[] PT3NoteTable_PT =
        {
            0x0C22, 0x0B73, 0x0ACF, 0x0A33, 0x09A1, 0x0917, 0x0894, 0x0819, 0x07A4, 0x0737, 0x06CF, 0x066D,
            0x0611, 0x05BA, 0x0567, 0x051A, 0x04D0, 0x048B, 0x044A, 0x040C, 0x03D2, 0x039B, 0x0367, 0x0337,
            0x0308, 0x02DD, 0x02B4, 0x028D, 0x0268, 0x0246, 0x0225, 0x0206, 0x01E9, 0x01CE, 0x01B4, 0x019B,
            0x0184, 0x016E, 0x015A, 0x0146, 0x0134, 0x0123, 0x0112, 0x0103, 0x00F5, 0x00E7, 0x00DA, 0x00CE,
            0x00C2, 0x00B7, 0x00AD, 0x00A3, 0x009A, 0x0091, 0x0089, 0x0082, 0x007A, 0x0073, 0x006D, 0x0067,
            0x0061, 0x005C, 0x0056, 0x0052, 0x004D, 0x0049, 0x0045, 0x0041, 0x003D, 0x003A, 0x0036, 0x0033,
            0x0031, 0x002E, 0x002B, 0x0029, 0x0027, 0x0024, 0x0022, 0x0020, 0x001F, 0x001D, 0x001B, 0x001A,
            0x0018, 0x0017, 0x0016, 0x0014, 0x0013, 0x0012, 0x0011, 0x0010, 0x000F, 0x000E, 0x000D, 0x000C
        };
        
        // Table #1 of Pro Tracker 3.3x - 3.5x
        public static ushort[] PT3NoteTable_ST =
        {
            0x0EF8, 0x0E10, 0x0D60, 0x0C80, 0x0BD8, 0x0B28, 0x0A88, 0x09F0, 0x0960, 0x08E0, 0x0858, 0x07E0,
            0x077C, 0x0708, 0x06B0, 0x0640, 0x05EC, 0x0594, 0x0544, 0x04F8, 0x04B0, 0x0470, 0x042C, 0x03FD,
            0x03BE, 0x0384, 0x0358, 0x0320, 0x02F6, 0x02CA, 0x02A2, 0x027C, 0x0258, 0x0238, 0x0216, 0x01F8,
            0x01DF, 0x01C2, 0x01AC, 0x0190, 0x017B, 0x0165, 0x0151, 0x013E, 0x012C, 0x011C, 0x010A, 0x00FC,
            0x00EF, 0x00E1, 0x00D6, 0x00C8, 0x00BD, 0x00B2, 0x00A8, 0x009F, 0x0096, 0x008E, 0x0085, 0x007E,
            0x0077, 0x0070, 0x006B, 0x0064, 0x005E, 0x0059, 0x0054, 0x004F, 0x004B, 0x0047, 0x0042, 0x003F,
            0x003B, 0x0038, 0x0035, 0x0032, 0x002F, 0x002C, 0x002A, 0x0027, 0x0025, 0x0023, 0x0021, 0x001F,
            0x001D, 0x001C, 0x001A, 0x0019, 0x0017, 0x0016, 0x0015, 0x0013, 0x0012, 0x0011, 0x0010, 0x000F
        };
        
        // Table #2 of Pro Tracker 3.4x - 3.5x
        public static ushort[] PT3NoteTable_ASM =
        {
            0x0D10, 0x0C55, 0x0BA4, 0x0AFC, 0x0A5F, 0x09CA, 0x093D, 0x08B8, 0x083B, 0x07C5, 0x0755, 0x06EC,
            0x0688, 0x062A, 0x05D2, 0x057E, 0x052F, 0x04E5, 0x049E, 0x045C, 0x041D, 0x03E2, 0x03AB, 0x0376,
            0x0344, 0x0315, 0x02E9, 0x02BF, 0x0298, 0x0272, 0x024F, 0x022E, 0x020F, 0x01F1, 0x01D5, 0x01BB,
            0x01A2, 0x018B, 0x0174, 0x0160, 0x014C, 0x0139, 0x0128, 0x0117, 0x0107, 0x00F9, 0x00EB, 0x00DD,
            0x00D1, 0x00C5, 0x00BA, 0x00B0, 0x00A6, 0x009D, 0x0094, 0x008C, 0x0084, 0x007C, 0x0075, 0x006F,
            0x0069, 0x0063, 0x005D, 0x0058, 0x0053, 0x004E, 0x004A, 0x0046, 0x0042, 0x003E, 0x003B, 0x0037,
            0x0034, 0x0031, 0x002F, 0x002C, 0x0029, 0x0027, 0x0025, 0x0023, 0x0021, 0x001F, 0x001D, 0x001C,
            0x001A, 0x0019, 0x0017, 0x0016, 0x0015, 0x0014, 0x0012, 0x0011, 0x0010, 0x000F, 0x000E, 0x000D
        };
        
        // Table #3 of Pro Tracker 3.4x - 3.5x
        public static ushort[] PT3NoteTable_Real =
        {
            0x0CDA, 0x0C22, 0x0B73, 0x0ACF, 0x0A33, 0x09A1, 0x0917, 0x0894, 0x0819, 0x07A4, 0x0737, 0x06CF,
            0x066D, 0x0611, 0x05BA, 0x0567, 0x051A, 0x04D0, 0x048B, 0x044A, 0x040C, 0x03D2, 0x039B, 0x0367,
            0x0337, 0x0308, 0x02DD, 0x02B4, 0x028D, 0x0268, 0x0246, 0x0225, 0x0206, 0x01E9, 0x01CE, 0x01B4,
            0x019B, 0x0184, 0x016E, 0x015A, 0x0146, 0x0134, 0x0123, 0x0112, 0x0103, 0x00F5, 0x00E7, 0x00DA,
            0x00CE, 0x00C2, 0x00B7, 0x00AD, 0x00A3, 0x009A, 0x0091, 0x0089, 0x0082, 0x007A, 0x0073, 0x006D,
            0x0067, 0x0061, 0x005C, 0x0056, 0x0052, 0x004D, 0x0049, 0x0045, 0x0041, 0x003D, 0x003A, 0x0036,
            0x0033, 0x0031, 0x002E, 0x002B, 0x0029, 0x0027, 0x0024, 0x0022, 0x0020, 0x001F, 0x001D, 0x001B,
            0x001A, 0x0018, 0x0017, 0x0016, 0x0014, 0x0013, 0x0012, 0x0011, 0x0010, 0x000F, 0x000E, 0x000D
        };

        // Table #4 of IvanRochin NATURAL Cmaj/Am
        // 5760, 5400, 5120, 4800, 4608, 4320, 4050, 3840, 3600, 3456, 3240, 3072, //3041280
        // 1520640 MHz
        public static ushort[] PT3NoteTable_Natural = new ushort[]
        {
            0x0B40, 0x0A8C, 0x0A00, 0x0960, 0x0900, 0x0870, 0x07E9, 0x0780, 0x0708, 0x06C0, 0x0654, 0x0600,
            0x05A0, 0x0546, 0x0500, 0x04B0, 0x0480, 0x0438, 0x03F5, 0x03C0, 0x0384, 0x0360, 0x032A, 0x0300,
            0x02D0, 0x02A3, 0x0280, 0x0258, 0x0240, 0x021C, 0x01FA, 0x01E0, 0x01C2, 0x01B0, 0x0195, 0x0180,
            0x0168, 0x0152, 0x0140, 0x012C, 0x0120, 0x010E, 0x00FD, 0x00F0, 0x00E1, 0x00D8, 0x00CB, 0x00C0,
            0x00B4, 0x00A9, 0x00A0, 0x0096, 0x0090, 0x0087, 0x007F, 0x0078, 0x0071, 0x006C, 0x0065, 0x0060,
            0x005A, 0x0054, 0x0050, 0x004B, 0x0048, 0x0044, 0x003F, 0x003C, 0x0038, 0x0036, 0x0033, 0x0030,
            0x002D, 0x002A, 0x0028, 0x0026, 0x0024, 0x0022, 0x0020, 0x001E, 0x001C, 0x001B, 0x0019, 0x0018,
            0x0017, 0x0015, 0x0014, 0x0013, 0x0012, 0x0011, 0x0010, 0x000F, 0x000E, 0x000E, 0x000D, 0x000C,
        };

        public static ushort[] CustomNoteTable =
        {
            0x0D10, 0x0C55, 0x0BA4, 0x0AFC, 0x0A5F, 0x09CA, 0x093D, 0x08B8, 0x083B, 0x07C5, 0x0755, 0x06EC,
            0x0688, 0x062A, 0x05D2, 0x057E, 0x052F, 0x04E5, 0x049E, 0x045C, 0x041D, 0x03E2, 0x03AB, 0x0376,
            0x0344, 0x0315, 0x02E9, 0x02BF, 0x0298, 0x0272, 0x024F, 0x022E, 0x020F, 0x01F1, 0x01D5, 0x01BB,
            0x01A2, 0x018B, 0x0174, 0x0160, 0x014C, 0x0139, 0x0128, 0x0117, 0x0107, 0x00F9, 0x00EB, 0x00DD,
            0x00D1, 0x00C5, 0x00BA, 0x00B0, 0x00A6, 0x009D, 0x0094, 0x008C, 0x0084, 0x007C, 0x0075, 0x006F,
            0x0069, 0x0063, 0x005D, 0x0058, 0x0053, 0x004E, 0x004A, 0x0046, 0x0042, 0x003E, 0x003B, 0x0037,
            0x0034, 0x0031, 0x002F, 0x002C, 0x0029, 0x0027, 0x0025, 0x0023, 0x0021, 0x001F, 0x001D, 0x001C,
            0x001A, 0x0019, 0x0017, 0x0016, 0x0015, 0x0014, 0x0012, 0x0011, 0x0010, 0x000F, 0x000E, 0x000D
        };

        // Volume table of Pro Tracker 3.5x
        public static byte[, ] PT3VolumeTable =
        {
            { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
            { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01 },
            { 0x00, 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x02, 0x02, 0x02, 0x02 },
            { 0x00, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x02, 0x02, 0x02, 0x02, 0x02, 0x03, 0x03, 0x03 },
            { 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x02, 0x02, 0x02, 0x02, 0x03, 0x03, 0x03, 0x03, 0x04, 0x04 },
            { 0x00, 0x00, 0x01, 0x01, 0x01, 0x02, 0x02, 0x02, 0x03, 0x03, 0x03, 0x04, 0x04, 0x04, 0x05, 0x05 },
            { 0x00, 0x00, 0x01, 0x01, 0x02, 0x02, 0x02, 0x03, 0x03, 0x04, 0x04, 0x04, 0x05, 0x05, 0x06, 0x06 },
            { 0x00, 0x00, 0x01, 0x01, 0x02, 0x02, 0x03, 0x03, 0x04, 0x04, 0x05, 0x05, 0x06, 0x06, 0x07, 0x07 },
            { 0x00, 0x01, 0x01, 0x02, 0x02, 0x03, 0x03, 0x04, 0x04, 0x05, 0x05, 0x06, 0x06, 0x07, 0x07, 0x08 },
            { 0x00, 0x01, 0x01, 0x02, 0x02, 0x03, 0x04, 0x04, 0x05, 0x05, 0x06, 0x07, 0x07, 0x08, 0x08, 0x09 },
            { 0x00, 0x01, 0x01, 0x02, 0x03, 0x03, 0x04, 0x05, 0x05, 0x06, 0x07, 0x07, 0x08, 0x09, 0x09, 0x0A },
            { 0x00, 0x01, 0x01, 0x02, 0x03, 0x04, 0x04, 0x05, 0x06, 0x07, 0x07, 0x08, 0x09, 0x0A, 0x0A, 0x0B },
            { 0x00, 0x01, 0x02, 0x02, 0x03, 0x04, 0x05, 0x06, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0A, 0x0B, 0x0C },
            { 0x00, 0x01, 0x02, 0x03, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0A, 0x0B, 0x0C, 0x0D },
            { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E },
            { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F }
        };

        //public static event EventHandler<MidiEventArgs> MidiEvent;
        public static event EventHandler<PlaybackEventArgs> PlaybackEvent;

        static VTModule()
        {
            for (int i = 0; i < MaxSoundChipCount; i++)
                PlayArgs[i] = new PlayArgs();

            // Start dedicated MIDI event worker thread
            StartEventWorker();
        }

        // Sets the pointer to the module structure
        // Must be called at least once before using other procedures
        public static void Module_SetPointer(VTM vtm, int chipIndex)
        {
            VTM = vtm;
            ChipIndex = chipIndex;
        }

        // Sets the current delay
        // During playback, delay may be changed by a special command
        // Must be called each time before playback begins using
        // Pattern_PlayCurrentLine or Module_PlayCurrentLine,
        // otherwise the last used delay value will be used, which may be invalid
        // Valid range: 3..255 (3 for PT3 compatibility)
        public static void Module_SetDelay(sbyte delay)
        {
            PlayArgs[ChipIndex].Delay = delay;
        }

        // Initializes internal tracker variables
        // If all == true => Sam=1, Env=15, Orn=0, Vol=15
        public static void InitTrackerParameters(bool all)
        {
            WaveOutAPI.ResetAYChipEmulation(ChipIndex);

            PlayArgs[ChipIndex].DelayCount = 1;
            PlayArgs[ChipIndex].PT3Noise = 0;
            PlayArgs[ChipIndex].EnvBase = 0;
            PlayArgs[ChipIndex].IntCount = 0;

            if (all && VTM != null)
            {
                for (int k = 0; k < 3; k ++ )
                {
                    VTM.ChannelStates[k].Sample = 1;
                    VTM.ChannelStates[k].EnvelopeEnabled = false;
                    VTM.ChannelStates[k].Ornament = 0;
                    VTM.ChannelStates[k].Volume = 15;
                }
            }

            for (int k = 0; k < 3; k ++ )
            {
                PlayArgs[ChipIndex].ChannelParams[k].SamplePosition = 0;
                PlayArgs[ChipIndex].ChannelParams[k].OrnamentPosition = 0;
                PlayArgs[ChipIndex].ChannelParams[k].SoundEnabled = false;
                PlayArgs[ChipIndex].ChannelParams[k].Note = 0;
                PlayArgs[ChipIndex].ChannelParams[k].ToneSlideCount = 0;
                PlayArgs[ChipIndex].ChannelParams[k].CurrentToneSliding = 0;
                PlayArgs[ChipIndex].ChannelParams[k].CurrentOnOff = 0;
                PlayArgs[ChipIndex].ChannelParams[k].ToneAccumulator = 0;
                PlayArgs[ChipIndex].ChannelParams[k].Tone = 0;
                PlayArgs[ChipIndex].ChannelParams[k].CurrentAmplitudeSliding = 0;
                PlayArgs[ChipIndex].ChannelParams[k].CurrentEnvelopeSliding = 0;
                PlayArgs[ChipIndex].ChannelParams[k].CurrentNoiseSliding = 0;
            }

            PlayArgs[ChipIndex].LineIndex = 0;
        }

        // Sets the current line within the pattern
        // Valid values: 0 to PatternLength - 1
        // Playback by Module_PlayCurrentLine or Pattern_PlayCurrentLine
        // will start from this line
        public static void Pattern_SetCurrentLine(int line)
        {
            PlayArgs[ChipIndex].LineIndex = line;
        }

        // Sets the current pattern
        // Valid values: 0..MaxPatNum
        public static void Module_SetCurrentPattern(int pattern)
        {
            PlayArgs[ChipIndex].PatternIndex = pattern;

            Pattern_SetCurrentLine(0);
        }

        // Sets the current position
        // Valid values: 0..255
        // This value is used as an index to select the pattern number
        // from VTM.Position_List
        public static void Module_SetCurrentPosition(int position)
        {
            if (VTM.Positions.Length == 0)
                return;
            
            PlayArgs[ChipIndex].PositionIndex = position;

            Module_SetCurrentPattern(VTM.Positions.Value[position]);
        }

        // Moves to the pattern at the next position, first line
        public static void Module_GoNextPosition()
        {
            byte nextPosition = (byte)(PlayArgs[ChipIndex].PositionIndex + 1);
            byte lastPosition = (byte)(VTM.Positions.Length - 1);

            if (nextPosition > lastPosition)
            {
                if (AY.LoopAllowed || Main.LoopAllAllowed)
                    Module_SetCurrentPosition(VTM.Positions.Loop);
                else
                    Pattern_SetCurrentLine(0);
            }
            else
                Module_SetCurrentPosition(nextPosition);
            
            Pattern_PlayCurrentLine();
        }

        public static int GetNoteFreq(NoteTableType noteTable, int note)
        {
            int result;

            switch(noteTable)
            {
                case NoteTableType.ProTracker:
                    result = PT3NoteTable_PT[note];
                    break;
                case NoteTableType.SoundTracker:
                    result = PT3NoteTable_ST[note];
                    break;
                case NoteTableType.ASM:
                    result = PT3NoteTable_ASM[note];
                    break;
                case NoteTableType.Real:
                    result = PT3NoteTable_Real[note];
                    break;
                case NoteTableType.Natural:
                    result = PT3NoteTable_Natural[note];
                    break;
                default:
                    result = CustomNoteTable[note];
                    break;
            }
            return result;
        }

        public static int GetNoteByEnvelope2(NoteTableType noteTableType, int e)
        {
            int result;
            ushort[] noteTable;
            int n, d;
            double f;
            int nearestNote;
            ushort bestDistanceFoundYet;

            switch(noteTableType)
            {
                case NoteTableType.ProTracker:
                    noteTable = PT3NoteTable_PT;
                    break;
                case NoteTableType.SoundTracker:
                    noteTable = PT3NoteTable_ST;
                    break;
                case NoteTableType.ASM:
                    noteTable = PT3NoteTable_ASM;
                    break;
                case NoteTableType.Real:
                    noteTable = PT3NoteTable_Real;
                    break;
                case NoteTableType.Natural:
                    noteTable = PT3NoteTable_Natural;
                    break;
                default:
                    noteTable = CustomNoteTable;
                    break;
            }

            result =  -1;

            for (int i = 0; i < noteTable.Length; i++)
            {
                f = noteTable[i] / 16.0;
                n = (int)Math.Round(f);

                if (n == e)
                {
                    result = i;
                    return result;
                }
            }

            if (result >= 0)
                return result;

            // Search nearest note
            nearestNote =  -1;
            bestDistanceFoundYet = 0xFFFF;

            for (int i = 0; i < noteTable.Length; i++)
            {
                n = (int)Math.Round(noteTable[i] / 16.0);
                d = Math.Abs(e - n);
                
                if (d < bestDistanceFoundYet)
                {
                    bestDistanceFoundYet = (ushort)d;
                    nearestNote = i;
                }
            }

            result = nearestNote;

            return result;
        }

        public static int GetNoteByEnvelope(int e)
        {
            if (VTM == null)
                return 0;

            return GetNoteByEnvelope2(VTM.NoteTable, e);
        }
		

        public static int GetNoteByTone(NoteTableType noteTableType, int tone)
        {
            ushort[] noteTable;

            switch (noteTableType)
            {
                case NoteTableType.ProTracker: noteTable = PT3NoteTable_PT; break;
                case NoteTableType.SoundTracker: noteTable = PT3NoteTable_ST; break;
                case NoteTableType.ASM: noteTable = PT3NoteTable_ASM; break;
                case NoteTableType.Real: noteTable = PT3NoteTable_Real; break;
                case NoteTableType.Natural: noteTable = PT3NoteTable_Natural; break;
                default: noteTable = CustomNoteTable; break;
            }

            int nearestNote = -1;
            int bestDistance = int.MaxValue;

            for (int i = 0; i < noteTable.Length; i++)
            {
                int d = Math.Abs(tone - noteTable[i]);

                if (d < bestDistance)
                {
                    bestDistance = d;
                    nearestNote = i;

                    if (d == 0)
                        break;
                }
            }

            return nearestNote;
        }

        public static int GetNoteByTone(int tone)
        {
            if (VTM == null)
                return -1;

            return GetNoteByTone(VTM.NoteTable, tone);
        }

        public static void Pattern_PlayOnlyCurrentLine_GetRegisters(ref int tempMixer, int chNum)
        {
            byte j;
            ushort w;
            bool globalTone, globalNoise, globalEnvelope;

            //Console.WriteLine($"Pattern_PlayOnlyCurrentLine_GetRegisters (chNum: {chNum})");

            ChannelParams channelParams = PlayArgs[ChipIndex].ChannelParams[chNum];
            ChannelState channelState = VTM.ChannelStates[chNum];

            if (channelParams.SoundEnabled)
            {
                if ((VTM.Samples[channelState.Sample] == null) ||
                    (channelParams.SamplePosition >= VTM.Samples[channelState.Sample].Length))
                    channelParams.Tone = 0;
                else
                {
                    channelParams.Tone = (ushort)(channelParams.ToneAccumulator + VTM.Samples[channelState.Sample].Ticks[channelParams.SamplePosition].AddToTone);
                    
                    if (VTM.Samples[channelState.Sample].Ticks[channelParams.SamplePosition].Ton_Accumulation)
                        channelParams.ToneAccumulator = channelParams.Tone;
                }
                if ((VTM.Ornaments[channelState.Ornament] == null) ||
                    (channelParams.OrnamentPosition >= VTM.Ornaments[channelState.Ornament].Length))
                    j = channelParams.Note;
                else
                    j = (byte)(channelParams.Note + VTM.Ornaments[channelState.Ornament].Offsets[channelParams.OrnamentPosition]);

                if ((sbyte)j < 0)
                    j = 0;
                else if (j > 95)
                    j = 95;

                w = (ushort)GetNoteFreq(VTM.NoteTable, j);
                channelParams.Tone = (ushort)((channelParams.Tone + channelParams.CurrentToneSliding + w) & 0xFFF);

                //Console.WriteLine($"w: {w} j: {j} Ton: {channelParams.Ton} Current_Ton_Sliding: {channelParams.Current_Ton_Sliding}");

                if (channelParams.ToneSlideCount > 0)
                {
                    channelParams.ToneSlideCount--;
                    
                    if (channelParams.ToneSlideCount == 0)
                    {
                        channelParams.CurrentToneSliding += channelParams.ToneSlideStep;
                        channelParams.ToneSlideCount = channelParams.ToneSlideDelay;
                        
                        if (channelParams.ToneSlideType == 1)
                        {
                            if ((channelParams.ToneSlideStep < 0 && channelParams.CurrentToneSliding <= channelParams.ToneSlideDelta) ||
                                (channelParams.ToneSlideStep >= 0 && channelParams.CurrentToneSliding >= channelParams.ToneSlideDelta))
                            {
                                channelParams.Note = channelParams.SlideToNote;
                                channelParams.ToneSlideCount = 0;
                                channelParams.CurrentToneSliding = 0;
                            }
                        }
                    }
                }

                if (VTM.Samples[channelState.Sample] == null ||
                    channelParams.SamplePosition >= VTM.Samples[channelState.Sample].Length)
                    channelParams.Amplitude = 0;
                else
                {
                    channelParams.Amplitude = VTM.Samples[channelState.Sample].Ticks[channelParams.SamplePosition].Amplitude;
                    
                    if (VTM.Samples[channelState.Sample].Ticks[channelParams.SamplePosition].Amplitude_Sliding)
                    {
                        if (VTM.Samples[channelState.Sample].Ticks[channelParams.SamplePosition].Amplitude_Slide_Up)
                        {
                            if (channelParams.CurrentAmplitudeSliding < 15)
                                channelParams.CurrentAmplitudeSliding++;
                        }
                        else if (channelParams.CurrentAmplitudeSliding > -15)
                            channelParams.CurrentAmplitudeSliding--;
                    }

                    channelParams.Amplitude += (byte)channelParams.CurrentAmplitudeSliding;

                    if (((sbyte)channelParams.Amplitude) < 0)
                        channelParams.Amplitude = 0;
                    else if (channelParams.Amplitude > 15)
                        channelParams.Amplitude = 15;

                    channelParams.Amplitude = PT3VolumeTable[channelState.Volume, channelParams.Amplitude];
                    
                    if (VTM.Samples[channelState.Sample].Ticks[channelParams.SamplePosition].Envelope_Enabled && channelState.EnvelopeEnabled)
                        channelParams.Amplitude = (byte)(channelParams.Amplitude | 16);

                    if (!VTM.Samples[channelState.Sample].Ticks[channelParams.SamplePosition].Mixer_Noise)
                    {
                        j = (byte)(channelParams.CurrentEnvelopeSliding + VTM.Samples[channelState.Sample].Ticks[channelParams.SamplePosition].Add_to_Envelope_or_Noise);
                        
                        if (VTM.Samples[channelState.Sample].Ticks[channelParams.SamplePosition].Envelope_or_Noise_Accumulation)
                            channelParams.CurrentEnvelopeSliding = (sbyte)j;

                        PlayArgs[ChipIndex].AddToEnv += (sbyte)j;
                    }
                    else
                    {
                        PlayArgs[ChipIndex].PT3Noise = (byte)(channelParams.CurrentNoiseSliding +
                            VTM.Samples[channelState.Sample].Ticks[channelParams.SamplePosition].Add_to_Envelope_or_Noise);
                        
                        if (VTM.Samples[channelState.Sample].Ticks[channelParams.SamplePosition].Envelope_or_Noise_Accumulation)
                            channelParams.CurrentNoiseSliding = (sbyte)PlayArgs[ChipIndex].PT3Noise;
                    }
                    if (!VTM.Samples[channelState.Sample].Ticks[channelParams.SamplePosition].Mixer_Ton)
                        tempMixer |= 8;

                    if (!VTM.Samples[channelState.Sample].Ticks[channelParams.SamplePosition].Mixer_Noise)
                        tempMixer |= 0x40;
                }

                if (VTM.Samples[channelState.Sample] != null)
                {
                    channelParams.SamplePrevPosition = channelParams.SamplePosition;
                    channelParams.SamplePosition++;

                    if (channelParams.SamplePosition >= VTM.Samples[channelState.Sample].Length)
                        channelParams.SamplePosition = VTM.Samples[channelState.Sample].Loop;
                }

                if (VTM.Ornaments[channelState.Ornament] != null)
                {
                    channelParams.OrnamentPosition++;

                    if (channelParams.OrnamentPosition >= VTM.Ornaments[channelState.Ornament].Length)
                        channelParams.OrnamentPosition = (byte)VTM.Ornaments[channelState.Ornament].Loop;
                }
            }
            else
                channelParams.Amplitude = 0;

            tempMixer >>= 1;

            if (channelParams.CurrentOnOff > 0)
            {
                channelParams.CurrentOnOff--;

                if (channelParams.CurrentOnOff == 0)
                {
                    channelParams.SoundEnabled = !channelParams.SoundEnabled;
                    channelParams.CurrentOnOff = channelParams.SoundEnabled ? channelParams.OnOffDelay : channelParams.OffOnDelay;
                }
            }

            if (PlayArgs[ChipIndex].PatternIndex == -1)
                return;

            globalTone = VTM.ChannelStates[chNum].GlobalTone;
            globalNoise = VTM.ChannelStates[chNum].GlobalNoise;
            globalEnvelope = VTM.ChannelStates[chNum].GlobalEnvelope;
            
            if (VTM.Samples[channelState.Sample] != null && !VTM.Samples[channelState.Sample].Enabled)
            {
                globalTone = false;
                globalNoise = false;
                globalEnvelope = false;
            }

            if (!globalTone)
                tempMixer |= 4;

            if (!globalNoise)
                tempMixer |= 32;

            if (!globalEnvelope)
                channelParams.Amplitude &= 15;

            if ((!globalTone || !globalNoise) && ((channelParams.Amplitude & 16) == 0) && ((tempMixer & 36) == 36))
                channelParams.Amplitude = 0;
        }

        // Returns AY register values for the current pattern line
        // Does not advance to the next line
        public static void Pattern_PlayOnlyCurrentLine()
        {
            //Console.WriteLine("Pattern_PlayOnlyCurrentLine");

            PlayArgs[ChipIndex].IntCount++;
            PlayArgs[ChipIndex].AddToEnv = 0;
            int tempMixer = 0;

            for (int k = 0; k < 3; k ++ )
                Pattern_PlayOnlyCurrentLine_GetRegisters(ref tempMixer, k);
            
            SoundChip soundChip = AY.SoundChip[ChipIndex];
            PlayArgs playArgs = PlayArgs[ChipIndex];

            soundChip.SetMixerRegister((byte)tempMixer);

            soundChip.AYRegisters.ToneA = playArgs.ChannelParams[0].Tone;
            soundChip.AYRegisters.ToneB = playArgs.ChannelParams[1].Tone;
            soundChip.AYRegisters.ToneC = playArgs.ChannelParams[2].Tone;

            soundChip.SetAmplitudeA(playArgs.ChannelParams[0].Amplitude);
            soundChip.SetAmplitudeB(playArgs.ChannelParams[1].Amplitude);
            soundChip.SetAmplitudeC(playArgs.ChannelParams[2].Amplitude);
 
            soundChip.AYRegisters.Noise = (byte)((playArgs.PT3Noise + playArgs.AddToNoise) & 31);
            soundChip.AYRegisters.Envelope = (ushort)(playArgs.AddToEnv + playArgs.CurEnvSlide + playArgs.EnvBase);

            QueuePlaybackEvent(ChipIndex, soundChip, playArgs);

            if (playArgs.CurEnvDelay > 0)
            {
                playArgs.CurEnvDelay--;
                
                if (playArgs.CurEnvDelay == 0)
                {
                    playArgs.CurEnvDelay = playArgs.EnvDelay;
                    playArgs.CurEnvSlide += playArgs.EnvSlideAdd;
                }
            }
        }

        public static void Pattern_PlayCurrentLine_PatternInterpreter(int channelIndex)
        {
            //Console.WriteLine($"Pattern_PlayCurrentLine_PatternInterpreter (chNum: {chNum})");

            int toneSliding, prNote, gls;
            int channel = channelIndex;

            if (PlayArgs[ChipIndex].PatternIndex == -1)
                channel = CenterChannel;

            Pattern pattern = (PlayArgs[ChipIndex].PatternIndex == -1 ? VTM.ReservedPattern : VTM.Patterns[PlayArgs[ChipIndex].PatternIndex]);
            ChannelLine channelLine = pattern.Lines[PlayArgs[ChipIndex].LineIndex].Channel[channelIndex];

            toneSliding = PlayArgs[ChipIndex].ChannelParams[channel].CurrentToneSliding;
            prNote = PlayArgs[ChipIndex].ChannelParams[channel].Note;
            
            if (channelLine.Note == -2)
            {
                PlayArgs[ChipIndex].ChannelParams[channel].SoundEnabled = false;
                PlayArgs[ChipIndex].ChannelParams[channel].CurrentEnvelopeSliding = 0;
                PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideCount = 0;
                PlayArgs[ChipIndex].ChannelParams[channel].SamplePosition = 0;
                PlayArgs[ChipIndex].ChannelParams[channel].OrnamentPosition = 0;
                PlayArgs[ChipIndex].ChannelParams[channel].CurrentNoiseSliding = 0;
                PlayArgs[ChipIndex].ChannelParams[channel].CurrentAmplitudeSliding = 0;
                PlayArgs[ChipIndex].ChannelParams[channel].CurrentOnOff = 0;
                PlayArgs[ChipIndex].ChannelParams[channel].CurrentToneSliding = 0;
                PlayArgs[ChipIndex].ChannelParams[channel].ToneAccumulator = 0;
            }
            else if (channelLine.Note != -1)
            {
                PlayArgs[ChipIndex].ChannelParams[channel].SoundEnabled = true;
                PlayArgs[ChipIndex].ChannelParams[channel].Note = (byte)channelLine.Note;
                PlayArgs[ChipIndex].ChannelParams[channel].CurrentEnvelopeSliding = 0;
                PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideCount = 0;
                PlayArgs[ChipIndex].ChannelParams[channel].SamplePosition = 0;
                PlayArgs[ChipIndex].ChannelParams[channel].OrnamentPosition = 0;
                PlayArgs[ChipIndex].ChannelParams[channel].CurrentNoiseSliding = 0;
                PlayArgs[ChipIndex].ChannelParams[channel].CurrentAmplitudeSliding = 0;
                PlayArgs[ChipIndex].ChannelParams[channel].CurrentOnOff = 0;
                PlayArgs[ChipIndex].ChannelParams[channel].CurrentToneSliding = 0;
                PlayArgs[ChipIndex].ChannelParams[channel].ToneAccumulator = 0;
            }

            if (channelLine.Note != -1 && channelLine.Sample != 0)
                VTM.ChannelStates[channel].Sample = channelLine.Sample;

            if (channelLine.Envelope != 0 && channelLine.Envelope != 15)
            {
                VTM.ChannelStates[channel].EnvelopeEnabled = true;
                PlayArgs[ChipIndex].EnvBase = (short)VTM.Patterns[PlayArgs[ChipIndex].PatternIndex].Lines[PlayArgs[ChipIndex].LineIndex].Envelope;
                
                AY.SoundChip[ChipIndex].SetEnvelopeRegister(channelLine.Envelope);
                
                VTM.ChannelStates[channel].Ornament = channelLine.Ornament;
                PlayArgs[ChipIndex].ChannelParams[channel].OrnamentPosition = 0;
                PlayArgs[ChipIndex].CurEnvSlide = 0;
                PlayArgs[ChipIndex].CurEnvDelay = 0;
            }
            else if (channelLine.Envelope == 15)
            {
                VTM.ChannelStates[channel].EnvelopeEnabled = false;
                VTM.ChannelStates[channel].Ornament = channelLine.Ornament;
                PlayArgs[ChipIndex].ChannelParams[channel].OrnamentPosition = 0;
            }
            else if (channelLine.Ornament != 0)
            {
                VTM.ChannelStates[channel].Ornament = channelLine.Ornament;
                PlayArgs[ChipIndex].ChannelParams[channel].OrnamentPosition = 0;
            }

            if (channelLine.Volume > 0)
                VTM.ChannelStates[channel].Volume = (byte)channelLine.Volume;

            switch(channelLine.AdditionalCommand.Number)
            {
                case 0x1:
                    gls = channelLine.AdditionalCommand.Delay;
                    
                    PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideDelay = (sbyte)gls;

                    if (gls == 0 && VTM.FeaturesLevel >= FeaturesLevel.PT37)
                        gls++;

                    PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideCount = (sbyte)gls;
                    PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideStep = channelLine.AdditionalCommand.Parameter;
                    PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideType = 0;
                    PlayArgs[ChipIndex].ChannelParams[channel].CurrentOnOff = 0;
                    break;
                case 0x2:
                    gls = channelLine.AdditionalCommand.Delay;
                    
                    PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideDelay = (sbyte)gls;

                    if (gls == 0 && VTM.FeaturesLevel >= FeaturesLevel.PT37)
                        gls++;

                    PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideCount = (sbyte)gls;
                    PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideStep =  (short)-channelLine.AdditionalCommand.Parameter;
                    PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideType = 0;
                    PlayArgs[ChipIndex].ChannelParams[channel].CurrentOnOff = 0;
                    break;
                case 0x3:
                    if (channelLine.Note >= 0 || (channelLine.Note != -2 && VTM.FeaturesLevel >= FeaturesLevel.VTII_PT36))
                    {
                        PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideDelay = (sbyte)channelLine.AdditionalCommand.Delay;
                        PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideCount = PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideDelay;
                        PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideStep = channelLine.AdditionalCommand.Parameter;
                        PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideDelta = (short)(GetNoteFreq(VTM.NoteTable, PlayArgs[ChipIndex].ChannelParams[channel].Note) - GetNoteFreq(VTM.NoteTable, prNote));
                        PlayArgs[ChipIndex].ChannelParams[channel].SlideToNote = PlayArgs[ChipIndex].ChannelParams[channel].Note;
                        PlayArgs[ChipIndex].ChannelParams[channel].Note = (byte)prNote;
                        
                        if (VTM.FeaturesLevel >= FeaturesLevel.VTII_PT36)
                            PlayArgs[ChipIndex].ChannelParams[channel].CurrentToneSliding = (short)toneSliding;

                        if (PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideDelta - PlayArgs[ChipIndex].ChannelParams[channel].CurrentToneSliding < 0)
                            PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideStep =  (short)-PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideStep;

                        PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideType = 1;
                        PlayArgs[ChipIndex].ChannelParams[channel].CurrentOnOff = 0;
                    }
                    break;
                case 0x4:
                    PlayArgs[ChipIndex].ChannelParams[channel].SamplePosition = channelLine.AdditionalCommand.Parameter;
                    break;
                case 0x5:
                    PlayArgs[ChipIndex].ChannelParams[channel].OrnamentPosition = channelLine.AdditionalCommand.Parameter;
                    break;
                case 0x6:
                    PlayArgs[ChipIndex].ChannelParams[channel].OffOnDelay = (sbyte)(channelLine.AdditionalCommand.Parameter & 15);
                    PlayArgs[ChipIndex].ChannelParams[channel].OnOffDelay = (sbyte)(channelLine.AdditionalCommand.Parameter >> 4);
                    PlayArgs[ChipIndex].ChannelParams[channel].CurrentOnOff = PlayArgs[ChipIndex].ChannelParams[channel].OnOffDelay;
                    PlayArgs[ChipIndex].ChannelParams[channel].ToneSlideCount = 0;
                    PlayArgs[ChipIndex].ChannelParams[channel].CurrentToneSliding = 0;
                    break;
                case 0x9:
                    PlayArgs[ChipIndex].EnvDelay = (sbyte)channelLine.AdditionalCommand.Delay;
                    PlayArgs[ChipIndex].CurEnvDelay = PlayArgs[ChipIndex].EnvDelay;
                    PlayArgs[ChipIndex].EnvSlideAdd = channelLine.AdditionalCommand.Parameter;
                    break;
                case 0xA:
                    PlayArgs[ChipIndex].EnvDelay = (sbyte)channelLine.AdditionalCommand.Delay;
                    PlayArgs[ChipIndex].CurEnvDelay = PlayArgs[ChipIndex].EnvDelay;
                    PlayArgs[ChipIndex].EnvSlideAdd = (short)-channelLine.AdditionalCommand.Parameter;
                    break;
                case 0xB:
                    if (channelLine.AdditionalCommand.Parameter == 0)
                        break;

                    PlayArgs[ChipIndex].Delay = (sbyte)channelLine.AdditionalCommand.Parameter;
                    break;
            }
        }

        // Returns AY register values for the current pattern line
        // and advances to the next line
        // Return values:
        // 0 - Normal processing
        // 1 - Line ended and AYRegisters are from the new line
        // 2 - No more lines; AYRegisters remain unchanged
        public static PlayLineResult Pattern_PlayCurrentLine()
        {
            //Console.WriteLine("Pattern_PlayCurrentLine");

            if (PlayArgs[ChipIndex].PatternIndex == -1)
            {
                PlayArgs[ChipIndex].AddToNoise = (sbyte)VTM.ReservedPattern.Lines[PlayArgs[ChipIndex].LineIndex].Noise;
                Pattern_PlayCurrentLine_PatternInterpreter(0);
            }
            else
            {
                if (!UnlimitedDelay || !WaveOutAPI.IsPlaying)
                    PlayArgs[ChipIndex].DelayCount--;

                if (PlayArgs[ChipIndex].DelayCount == 0)
                {
                    if (VTM.Patterns[PlayArgs[ChipIndex].PatternIndex] == null)
                        return PlayLineResult.LineEnded;

                    if (PlayArgs[ChipIndex].LineIndex >= VTM.Patterns[PlayArgs[ChipIndex].PatternIndex].Length)
                    {
                        PlayArgs[ChipIndex].DelayCount++;

                        return PlayLineResult.PatternEnded;
                    }

                    PlayArgs[ChipIndex].AddToNoise = (sbyte)VTM.Patterns[PlayArgs[ChipIndex].PatternIndex].Lines[PlayArgs[ChipIndex].LineIndex].Noise;
            
                    for (int k = 0; k < 3; k ++)
                        Pattern_PlayCurrentLine_PatternInterpreter(k);

                    PlayArgs[ChipIndex].LineIndex++;
                    PlayArgs[ChipIndex].DelayCount = PlayArgs[ChipIndex].Delay;                    
                }
            }

            Pattern_PlayOnlyCurrentLine();

            return PlayLineResult.Normal;
        }

        // Returns AY register values for the current pattern line
        // and advances to the next line
        // If end of pattern is reached, moves automatically to the
        // next pattern from the Position List
        // Return values:
        // 0 - Normal processing
        // 1 - Line ended; AYRegisters are from the new line
        // 2 - Pattern ended; AYRegisters are from the first line of the next pattern
        // 3 - All patterns ended; AYRegisters are from the first line of the first pattern
        public static PlayLineResult Module_PlayCurrentLine()
        {
            // Console.WriteLine("Module_PlayCurrentLine");

            if (VTM.Positions.Length == 0)
                return PlayLineResult.AllPatternsEnded;

            PlayLineResult result = Pattern_PlayCurrentLine();

            if (result == PlayLineResult.PatternEnded)
            {
                PlayArgs[ChipIndex].PositionIndex++;
                
                if (PlayArgs[ChipIndex].PositionIndex >= VTM.Positions.Length)
                {
                    PlayArgs[ChipIndex].PositionIndex = VTM.Positions.Loop;
                    result = PlayLineResult.AllPatternsEnded;
                }

                PlayArgs[ChipIndex].PatternIndex = VTM.Positions.Value[PlayArgs[ChipIndex].PositionIndex];
                PlayArgs[ChipIndex].LineIndex = 0;

                Pattern_PlayCurrentLine();
            }

            return result;
        }

        public static void CleanPattern(Pattern pattern)
        {
            if (pattern == null)
                return;

            for (int i = 0; i < VTModule.MaxPatternLength; i++)
            {
                Line line = pattern.Lines[i];
                line.Envelope = 0;
                line.Noise = 0;
                line.Channel[0] = new ChannelLine();
                line.Channel[1] = new ChannelLine();
                line.Channel[2] = new ChannelLine();
                pattern.Lines[i] = line;
            }

            pattern.Length = VTModule.DefaultPatternLength;
        }

        public static void ValidateSample(int index, VTM vtm)
        {
            if (vtm.Samples[index] != null)
                return;

            vtm.Samples[index] = new Sample();
            vtm.Samples[index].Loop = 0;
            vtm.Samples[index].Length = 1;
            vtm.Samples[index].Enabled = true;
            vtm.Samples[index].Ticks[0] = new SampleTick();

            for (int i = 1; i < MaxSampleLength; i++)
                vtm.Samples[index].Ticks[i] = new SampleTick();
        }

        public static void ValidatePattern(int index, VTM vtm)
        {
            if (vtm.Patterns[index] != null)
                return;

            vtm.Patterns[index] = new Pattern();
        }

        public static bool SGetNumber(string s, int max, out int res)
        {
            res = 0;
            s = s.ToUpper();
            char[] chars = s.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '.')
                    chars[i] = '0';

                if (char.IsDigit(chars[i]))
                    res = res * 16 + (chars[i] - '0');
                else if (chars[i] >= 'A' && chars[i] <= 'V')
                    res = res * 16 + (chars[i] - 'A' + 10);
                else
                    return false;
            }
            return res <= max;
        }


        public static bool SGetDecNumber(string s, int max, out int res)
        {
            res = 0;
            s = s.Replace('.', '0');

            foreach (char c in s)
            {
                if (!char.IsDigit(c))
                    return false;
                res = res * 10 + (c - '0');
            }

            return res < max;
        }

        public static bool SGetNote(string s, out int res)
        {
            s = s.ToUpper();
            res = -2;
            if (s == "R--")
                return true;
            res++;
            if (s == "---")
                return true;

            if (s.Length != 3)
                return false;
            int d = s[1] == '#' ? 1 : (s[1] == '-' ? 0 : -1);
            if (d == -1)
                return false;

            int o = s[2] - '1';
            if (o < 0 || o > 7)
                return false;

            int n;
            switch (s[0])
            {
                case 'C': n = 0;
                    break;
                case 'D': n = 2;
                    break;
                case 'E': n = 4;
                    break;
                case 'F': n = 5;
                    break;
                case 'G': n = 7;
                    break;
                case 'A': n = 9;
                    break;
                case 'B': n = 11;
                    break;
                default:
                    return false;
            }

            res = n + d + o * 12;
            return (res < 96);
        }

        public static int SGetNote2(string s)
        {
            int res;
            return SGetNote(s, out res) ? res : -2;
        }

        public static string GetSampleString(SampleTick sl, bool vol, bool ns)
        {
            string result = "";

            result = sl.Mixer_Ton ? "T" : ".";
            result += sl.Mixer_Noise ? "N" : ".";
            result += sl.Envelope_Enabled ? "E" : ".";
            result += " ";

            result += sl.AddToTone >= 0 ? $"+{sl.AddToTone:X3}" : $"-{-sl.AddToTone:X3}";
            result += sl.Ton_Accumulation ? "^ " : "_ ";
            result += sl.Add_to_Envelope_or_Noise >= 0 ? $"+{sl.Add_to_Envelope_or_Noise:X2}" : $"-{-sl.Add_to_Envelope_or_Noise:X2}";

            if (ns)
                result += $"({(sl.Add_to_Envelope_or_Noise & 31):X2})";

            result += sl.Envelope_or_Noise_Accumulation ? "^ " : "_ ";
            result += $"{sl.Amplitude:X}";
            result += sl.Amplitude_Sliding ? (sl.Amplitude_Slide_Up ? "+" : "-") : "_";

            if (vol)
            {
                result += "+";
                for (int j = 1; j < 16; j++)
                    result += j <= sl.Amplitude ? (char)149 : ' ';
            }

            return result;
        }

        public static string GetSampleStringForRedraw(SampleTick sl)
        {
            string result = "";

            result += sl.Mixer_Ton ? "T" : ".";
            result += sl.Mixer_Noise ? "N" : ".";
            result += sl.Envelope_Enabled ? "E" : ".";
            result += " ";

            result += sl.AddToTone >= 0
                ? $"+{sl.AddToTone:X3}"
                : $"-{-sl.AddToTone:X3}";

            result += sl.Ton_Accumulation ? "  " : ". ";

            if (sl.Add_to_Envelope_or_Noise >= 0)
            {
                result += Main.DecBaseNoiseOn
                    ? $"+{sl.Add_to_Envelope_or_Noise:00}"
                    : $"+{sl.Add_to_Envelope_or_Noise:X2}";
            }
            else
            {
                result += Main.DecBaseNoiseOn
                    ? $"-{-sl.Add_to_Envelope_or_Noise:00}"
                    : $"-{-sl.Add_to_Envelope_or_Noise:X2}";
            }

            result += Main.DecBaseNoiseOn
                ? $"({sl.Add_to_Envelope_or_Noise & 31:00})"
                : $"({sl.Add_to_Envelope_or_Noise & 31:X2})";

            result += sl.Envelope_or_Noise_Accumulation ? "  " : ". ";
            result += $"{sl.Amplitude:X}";
            result += sl.Amplitude_Sliding ? " " : ".";
            result += new string(' ', 16);

            return result;
        }

        public static string Int4DToStr(int i)
        {
            string result;

            if (i == 0)
                result = "....";
            else if (i < 0x10)
                result = $"...{i:X}";
            else if (i < 0x100)
                result = $"..{i:X}";
            else if (i < 0x1000)
                result = $".{i:X}";
            else
                result = $"{i:X}";

            return result;
        }

        public static string Int2DToStr(int i)
        {
            string result;

            if (i == 0)
                result = "..";
            else if (i < 0x10)
                result = $".{i:X}";
            else
                result = $"{i:X}";

            return result;
        }

        public static string Int2DToDecStr(int i)
        {
            string result;

            if (i == 0)
                result = "..";
            else if (i < 10)
                result = $".{i}";
            else
                result = $"{i}";

            return result;
        }

        public static string Int1DToStr(int i)
        {
            return i == 0 ? "." : $"{i:X}";
        }

        public static string NoteToStr(int i)
        {
            string result;
            int octave;
            
            if (i ==  -1)
                result = "---";
            else if (i ==  -2)
                result = "R--";
            else
            {
                octave = i / 12 + 1;

                if (octave < 1)
                    result = "C-1";
                else if (octave > 8)
                    result = "B-8";
                else
                    result = Notes[Math.Abs(i) % 12] + octave.ToString();
            }

            return result;
        }

        public static string NoteToStr2(int i)
        {
            string result;

            if (i ==  -1)
                result = "---";
            else if (i ==  -2)
                result = "R--";
            else
                result = Notes[i % 12] + (i / 12 + 1).ToString();

            return result;
        }

        public static string SampToStr(int i)
        {
            if (i == 0)
                return ".";
            else if (i < 16)
                return $"{i:X}";
            else
                return ((char)(i + 'A' - 10)).ToString();
        }

        public static string IntsToTime(int i)
        {
            string result;
            int min, sec;

            if (i == 0)
            {
                result = "0:04";
                return result;
            }

            sec = (int)Math.Round(i * 1000.0 / WaveOutAPI.InterruptFreq);
            min = sec / 60;
            sec = sec % 60;
            result = $"{min}:{sec:D2}";
            return result;
        }

        public static string GetOutPatternLineString(int patNum, Pattern patPtr, int lineNum, int[] chn, bool previous, bool decBaseLinesOn, bool decBaseNoiseOn)
        {
            string result;

            bool isEmpty = false;

            if (previous && (patNum < 0 || patPtr.Length - lineNum < 0))
                isEmpty = true;

            if (!previous && (patNum < 0 || lineNum > patPtr.Length - 1))
                isEmpty = true;

            if (isEmpty)
            {
                if (decBaseLinesOn)
                    result = new String(' ', 53);
                else
                    result = new String(' ', 52);
            }
            else if (previous)
                result = GetPatternLineString(patPtr, patPtr.Length - lineNum, chn, false, false, decBaseLinesOn, decBaseNoiseOn);
            else
                result = GetPatternLineString(patPtr, lineNum, chn, false, false, decBaseLinesOn, decBaseNoiseOn);

            return result;
        }

        public static string GetPatternLineString_Envelope2NoteText(int e)
        {
            string result = Int4DToStr(e);

            if (!Main.EnvelopeAsNote || e == 0)
                return result;

            int note = GetNoteByEnvelope(e);

            if (note >= 0 && note <= 60)
                result = ' ' + NoteToStr(note);

            return result;
        }

        public static string GetPatternLineString(Pattern patPtr, int line, int[] chn, bool lineNums, bool separators, bool decBaseLinesOn, bool decBaseNoiseOn)
        {
            string result;
            int j1;
            int chan;
            string sep = (separators ? "|" : " ");

            if (lineNums)
            {
                if (decBaseLinesOn)
                    result = $"{line:D2}{sep}";
                else
                    result = $"{line:X2}{sep}";
            }
            else if (decBaseLinesOn)
                result = "   " + sep;
            else
                result = "  " + sep;

            if (patPtr == null)
            {
                if (separators)
                    result += "....|..|--- .... ....|--- .... ....|--- .... ....";
                else
                    result += ".... .. --- .... .... --- .... .... --- .... ....";
            }
            else
            {
                result += GetPatternLineString_Envelope2NoteText(patPtr.Lines[line].Envelope) + sep;
 
                if (decBaseNoiseOn)
                    result += Int2DToDecStr(patPtr.Lines[line].Noise);
                else
                    result += Int2DToStr(patPtr.Lines[line].Noise);

                for (j1 = 0; j1 < 3; j1 ++ )
                {
                    chan = chn[j1];
                    result += sep + NoteToStr(patPtr.Lines[line].Channel[chan].Note) + ' ';
                    result += SampToStr(patPtr.Lines[line].Channel[chan].Sample);
                    result += Int1DToStr(patPtr.Lines[line].Channel[chan].Envelope);
                    result += Int1DToStr(patPtr.Lines[line].Channel[chan].Ornament);
                    result += Int1DToStr(patPtr.Lines[line].Channel[chan].Volume) + ' ';
                    result += Int1DToStr(patPtr.Lines[line].Channel[chan].AdditionalCommand.Number);
                    result += Int1DToStr(patPtr.Lines[line].Channel[chan].AdditionalCommand.Delay);
                    result += Int2DToStr(patPtr.Lines[line].Channel[chan].AdditionalCommand.Parameter);
                }
            }

            return result;
        }

        public static int GetModuleTime(VTM vtm)
        {
            int result = 0;
            int i, j, k, d, p;

            d = vtm.InitialDelay;

            for (i = 0; i < vtm.Positions.Length; i++)
            {
                p = vtm.Positions.Value[i];
                if (vtm.Patterns[p] == null)
                {
                    result += d * DefaultPatternLength;
                }
                else
                {
                    for (j = 0; j < vtm.Patterns[p].Length; j ++ )
                    {
                        for (k = 2; k >= 0; k--)
                        {
                            AdditionalCommand additionalCommand = vtm.Patterns[p].Lines[j].Channel[k].AdditionalCommand;
                            if (additionalCommand.Number == 11 && additionalCommand.Parameter != 0)
                            {
                                d = additionalCommand.Parameter;
                                break;
                            }
                        }
                        result += d;
                    }
                }
            }
            return result;
        }

        public static int GetPositionTime(VTM vtm, int pos, ref int posDelay)
        {
            int result = 0;
            int i, j, k, d, p;
            
            d = vtm.InitialDelay;

            for (i = 0; i < pos; i++)
            {
                p = vtm.Positions.Value[i];
                if (vtm.Patterns[p] == null)
                {
                    result += d * DefaultPatternLength;
                }
                else
                {
                    for (j = 0; j < vtm.Patterns[p].Length; j++)
                    {
                        for (k = 2; k >= 0; k--)
                        {
                            AdditionalCommand additionalCommand = vtm.Patterns[p].Lines[j].Channel[k].AdditionalCommand;
                            if (additionalCommand.Number == 11 && additionalCommand.Parameter != 0)
                            {
                                d = additionalCommand.Parameter;
                                break;
                            }
                        }
                        result += d;
                    }
                }
            }
            posDelay = d;
            return result;
        }

        public static int GetPositionTimeEx(VTM vtm, int position, ref int positionDelay, int lineIndex)
        {
            int result = 0;
            int j, k, p;
            p = vtm.Positions.Value[position];
            if (vtm.Patterns[p] == null)
            {
                result += positionDelay * lineIndex;
            }
            else
            {
                for (j = 0; j < lineIndex; j++)
                {
                    if (j > 255)
                        return result;

                    for (k = 2; k >= 0; k--)
                    {
                        AdditionalCommand additionalCommand = vtm.Patterns[p].Lines[j].Channel[k].AdditionalCommand;
                        if (additionalCommand.Number == 11 && additionalCommand.Parameter != 0)
                        {
                            positionDelay = additionalCommand.Parameter;
                            break;
                        }
                    }
                    result += positionDelay;
                }
            }
            return result;
        }

        public static void GetTimeParams(VTM vtm, int time, out int position, out int lineIndex)
        {
            int delay, p, ct, tmp;
            position =  -1;
            lineIndex = 0;

            delay = vtm.InitialDelay;

            ct = 0;
            for (int i = 0; i < vtm.Positions.Length; i++)
            {
                p = vtm.Positions.Value[i];
                if (vtm.Patterns[p] == null)
                {
                    tmp = delay * DefaultPatternLength;
                    if (ct + tmp < time)
                    {
                        ct += tmp;
                    }
                    else
                    {
                        position = i;
                        lineIndex = (time - ct) / delay;
                        return;
                    }
                }
                else
                {
                    for (int j = 0; j < vtm.Patterns[p].Length; j++)
                    {
                        if (ct >= time)
                        {
                            position = i;
                            lineIndex = j;
                            return;
                        }
                        for (int k = 2; k >= 0; k--)
                        {
                            AdditionalCommand additionalCommand = vtm.Patterns[p].Lines[j].Channel[k].AdditionalCommand;
                            if (additionalCommand.Number == 11 && additionalCommand.Parameter != 0)
                            {
                                delay = additionalCommand.Parameter;
                                break;
                            }
                        }
                        ct += delay;
                    }
                }
            }
        }

        public static bool IsSampleEmpty(VTM vtm, int i)
        {
            Sample sample = vtm.Samples[i];

            if (sample != null)
            {
                if (sample.Length != 1)
                    return false;
                if (sample.Loop != 0)
                    return false;

                for (int j = 0; j < 64; j++)
                {
                    SampleTick tick = sample.Ticks[j];
                    if (tick.AddToTone != 0)
                        return false;
                    if (tick.Ton_Accumulation)
                        return false;
                    if (tick.Amplitude != 0)
                        return false;
                    if (tick.Amplitude_Sliding)
                        return false;
                    if (tick.Amplitude_Slide_Up)
                        return false;
                    if (tick.Envelope_Enabled)
                        return false;
                    if (tick.Envelope_or_Noise_Accumulation)
                        return false;
                    if (tick.Add_to_Envelope_or_Noise != 0)
                        return false;
                    if (tick.Mixer_Ton)
                        return false;
                    if (tick.Mixer_Noise)
                        return false;
                }
            }

            return true;
        }

        public static bool IsOrnamentEmpty(VTM vtm, int i)
        {
            var ornament = vtm.Ornaments[i];

            if (ornament != null)
            {
                if (ornament.Length != 1)
                    return false;
                if (ornament.Loop != 0)
                    return false;

                for (int j = 0; j < 255; j++)
                {
                    if (ornament.Offsets[j] != 0)
                        return false;
                }
            }

            return true;
        }

        public static void FreeVTM(ref VTM vtm)
        {
            if (vtm == null)
                return;

            for (int i = 0; i < vtm.Samples.Length; i++)
            {
                if (vtm.Samples[i] != null)
                {
                    //Dispose(VTM.Samples[i]);
                    vtm.Samples[i] = null;
                }
            }
            for (int i = 0; i < vtm.Ornaments.Length; i++)
            {
                if (vtm.Ornaments[i] != null)
                {
                    //Dispose(VTM.Ornaments[i]);
                    vtm.Ornaments[i] = null;
                }
            }
            for (int i = 0; i <= MaxPatternIndex; i++)
            {
                if (vtm.Patterns[i] != null)
                {
                    //Dispose(VTM.Patterns[i]);
                    vtm.Patterns[i] = null;
                }
            }
            //Dispose(VTM);
            vtm = null;
            //VTM = null;
        }



        public static void VTM2TextFile(string fileName, VTM vtm, bool append, int tracksCursorXLeft)
        {
            int i, j = 0;
            string colors;
            bool decBaseNoiseOn = Main.DecBaseNoiseOn;
            bool envelopeAsNode = Main.EnvelopeAsNote;

            using (FileStream fileStream = new FileStream(fileName, append ? FileMode.OpenOrCreate : FileMode.Create, FileAccess.ReadWrite))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream))
                {
                    Main.DecBaseNoiseOn = false;
                    Main.EnvelopeAsNote = false;

                    textWriter.WriteLine("[Module]");
                    textWriter.WriteLine($"VortexTrackerII={(vtm.HasHeader ? 1 : 0)}");
                    textWriter.WriteLine($"Version=3.{(5 + vtm.FeaturesLevel)}");
                    textWriter.WriteLine($"Title={vtm.Title}");
                    textWriter.WriteLine($"Author={vtm.Author}");

                    if (!append)
                        textWriter.WriteLine($"ShowInfo={(vtm.ShowInfo ? 1 : 0)}");

                    textWriter.WriteLine($"NoteTable={vtm.NoteTable}");
                    textWriter.WriteLine($"ChipFreq={vtm.ChipFreq}");
                    textWriter.WriteLine($"IntFreq={vtm.IntFreq}");
                    textWriter.WriteLine($"Speed={vtm.InitialDelay}");
                    textWriter.WriteLine("Noise=HEX");
                    textWriter.Write("PlayOrder=");

                    Position position = vtm.Positions;

                    for (i = 0; i < position.Length; i++)
                    {
                        if (i == position.Loop)
                            textWriter.Write('L');

                        textWriter.Write(position.Value[i]);

                        if (i != position.Length - 1)
                            textWriter.Write(',');
                    }

                    textWriter.WriteLine();

                    if (Int32.MaxValue != 0)
                    {
                        colors = "";
                        for (i = 0; i < vtm.Positions.Length - 1; i++)
                        {
                            colors += "${vtm.Positions.Colors[i]},";
                            j = i;
                        }

                        colors += $"{vtm.Positions.Colors[j + 1]}";
                        textWriter.WriteLine($"Colors={colors}");
                    }

                    textWriter.WriteLine();
                    textWriter.WriteLine();

                    // is it 32-ornament version?
                    int ornamentCount = 16;

                    for (i = 16; i < 32; i++)
                    {
                        if (!IsOrnamentEmpty(VTM, i))
                        {
                            ornamentCount = 32;
                            break;
                        }
                    }

                    for (i = 0; i < ornamentCount; i++)
                    {
                        textWriter.WriteLine($"[Ornament{i}]");
                        VTM.SaveOrnament(textWriter, vtm, i);
                        textWriter.WriteLine();
                    }

                    for (i = 0; i < 32; i++)
                    {
                        textWriter.WriteLine($"[Sample{i}]");
                        VTM.SaveSample(textWriter, vtm, i);
                        textWriter.WriteLine();
                    }

                    for (i = 0; i < VTModule.MaxPatternIndex; i++)
                    {
                        if (vtm.Patterns[i] != null)
                        {
                            textWriter.WriteLine($"[Pattern{i}]");
                            VTM.SavePattern(textWriter, vtm, i, tracksCursorXLeft);
                            textWriter.WriteLine();
                        }
                    }

                    if (vtm.Info.Trim() != "" && !append)
                    {
                        textWriter.WriteLine("[Info]");
                        textWriter.WriteLine(vtm.Info);
                        textWriter.WriteLine("[/Info]");
                        textWriter.WriteLine();
                        textWriter.WriteLine();
                    }

                    // colors := MainForm.GetPositionsColors;
                    // if colors <> '' then
                    // begin
                    // Writeln(TxtFile, '[Colors]');
                    // Writeln(TxtFile, colors);
                    // end;
                }
            }

            Main.DecBaseNoiseOn = decBaseNoiseOn;
            Main.EnvelopeAsNote = envelopeAsNode;
        }

        private static void QueuePlaybackEvent(int chipIndex, SoundChip soundChip, PlayArgs playArgs)
        {
            // Check if anyone is listening first (fast check)
            if (PlaybackEvent == null)
                return;

            // Create snapshot - fast, no blocking
            var args = new PlaybackEventArgs(chipIndex, soundChip, playArgs);
            
            // Try to add to queue with zero timeout (never blocks audio thread)
            // If queue is full, this returns false and event is dropped
            if (!_eventQueue.TryAdd(args, millisecondsTimeout: 0))
            {
                // Queue full - drop event (audio thread never blocks)
                System.Diagnostics.Debug.WriteLine("MIDI event queue full - dropping event");
            }
        }

    }
}



