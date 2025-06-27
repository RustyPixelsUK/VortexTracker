using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibVT
{
    /* public struct TSize
    {
        public int Width;
        public int Height;
        public int Left;
        public int Top;
    } // end TSize

    public class ERegistryError : Exception
    {
    } // end ERegistryError

    public struct TTracksCopy
    {
        //public TChildWin SrcWindow;
        public TPattern Pattern;
        public int PatNum;
        public byte Channel;
        public byte FromLine;
        public byte ToLine;
        public bool Ornament;
        public bool Command;
    } // end TTracksCopy

    public struct TSampleCopy
    {
        //public TChildWin SrcWindow;
        public TSample Sample;
        public byte FromColumn;
        public byte ToColumn;
        public byte FromLine;
        public byte ToLine;
        public byte StartColumn;
        public byte StartLine;
        public bool Ready;
    } // end TSampleCopy

    public struct THOBETAHeader
    {
        public char[] Name;
        public char Typ;
        public ushort Start;
        public ushort Leng;
        public ushort SectLeng;
        public ushort CheckSum;
        public byte[] Ind;
    } // end Record_2

    public struct TTSCLHeader
    {
        public char[] SCL;
        public byte NBlk;
        public char[] Name1;
        public char Typ1;
        public ushort Start1;
        public ushort Leng1;
        public byte Sect1;
        public char[] Name2;
        public char Typ2;
        public ushort Start2;
        public ushort Leng2;
        public byte Sect2;
        public byte[] Ind;
    } // end Record_3

    public struct TTAPHeader
    {
        public ushort Sz;
        public byte Flag;
        public byte Typ;
        public char[] Name;
        public ushort Leng;
        public ushort Start;
        public ushort Trash;
        public byte Sum;
        public byte[] Ind;
    } // end Record_4

    public enum TLastClipboard
    {
        LCNone,
        LCTracks,
        LCSamples,
        LCOrnaments
    } */

    public class Main
    {
        public static string ProductName = "Vortex Tracker";
        // Public declarations
        //public static int PrevTop = 0;
        //public static int WidthFix = 0;
        //public static int HeightFix = 0;
        //public static bool SizeFixed = false;
        //public static bool SnapedToRight = false;
        //public static bool Snaped = false;
        //public static bool MaximizeChilds = false;
        public static NoteTableType NoteTableOnLoad = 0;
        //public Font EditorFont = null;
        //public Font TestLineFont = null;
        //public static string[] RecentFiles;
        public static bool LoopAllAllowed = false;
        // 
        // // Sample templates is disabled now
        // 
        // SampleLineTemplates: array of TSampleTick;
        // CurrentSampleLineTemplate: integer;
        public static int GlobalVolume = 56;
        public static int GlobalVolumeMax = 64;
        //public static short DefaultTable = 0;
        //public static byte StartupAction = 0;
        //public static bool StartupOpenModule = false;
        //public static bool StartupOpenTheme = false;
        //public static string TemplateSongPath = String.Empty;
        //public static int WinThemeIndex = 0;
        // samples and ornaments for global buffer, for copy/paste
        public static Sample BuffSample = null;
        public static Ornament BuffOrnament = null;
        //public static bool NumberOfLinesChanged = false;
        //public static bool ResizeActionBlocked = false;
        //public static bool SampleBrowserVisible = false;
        //public static bool OrnamentsBrowserVisible = false;
        public static bool VTExit = false;
        //public static int LastChildWidth = 0;
        //public static int LastChildHeight = 0;
        //public static string SamplesDir = String.Empty;
        //public static string OrnamentsDir = String.Empty;
        //public static bool RedrawEnabled = false;
        //public TChildWin[] ChildsTable;
        //public static bool DontAddToRecent = false;

        // ------------------------

        //public const int UM_REDRAWTRACKS = Win32.WM_USER + 1;
        //public const int UM_PLAYINGOFF = Win32.WM_USER + 2;
        //public const int UM_FINALIZEWO = Win32.WM_USER + 3;
        //public static TMainForm MainForm = null;
        //public static uint Priority = Win32.NORMAL_PRIORITY_CLASS;
        //public static int TracksCursorXLeft = 0;
        //public static int OrnXShift = 0;
        //public static int OrnNCol = 4;
        //public static int OrnNChars = 9;
        //public static bool DisableUpdateChilds = false;
        //public static string SyncMessageFile = String.Empty;
        //public static bool SyncVTInstanses = false;
        //public static string SyncSampleBufferFile = String.Empty;
        //public static string SyncOrnamentBufferFile = String.Empty;
        //public static string SyncSamplePartFile = String.Empty;
        //public static int SyncSampleBufferFileAge = 0;
        //public static int SyncOrnamentBufferFileAge = 0;
        //public static int SyncSamplePartFileAge = 0;
        //public static bool SyncBufferBlocked = false;
        //public static bool ChildsEventsBlocked = false;
        //public static bool EditorFontChanged = false;
        //public static bool DisplayChanged = false;
        //public static TColorTheme ColorTheme;
        //public static string ColorThemeName = String.Empty;
        //public static sbyte[] NoteKeys = new sbyte[255 + 1];
        public static byte[] Panoram = new byte[3];
        //public static string VortexDir = String.Empty;
        //public static string VortexDocumentsDir = String.Empty;
        //public static string ConfigFilePath = String.Empty;
        public static bool EnvelopeAsNote = false;
        //public static bool DupNoteParams = false;
        public static bool MoveBetweenPatterns = true;
        //public static bool SamToneShiftAsNote = false;
        //public static bool OrnToneShiftAsNote = false;
        //public static int PositionSize = 0;
        public static bool DecBaseLinesOn = false;
        public static bool DecBaseNoiseOn = false;
        //public static bool HighlightSpeedOn = false;
        //public static bool DisableSeparators = false;
        //public static bool AutoBackupsOn = false;
        //public static byte AutoBackupsMins = 0;
        //public static bool DisableHints = false;
        //public static bool DisableCtrlClick = false;
        //public static bool DisableInfoWin = false;
        //public static bool VortexFirstStart = false;
        public static int ManualChipFreq = 0;
        public static int DefaultChipFreq = 1750000;
        public static int DefaultIntFreq = 48828;
        //public static int ExportSampleRate = 0;
        //public static int ExportBitRate = 0;
        //public static int ExportChannels = 0;
        //public static int ExportChip = 0;
        //public static int ExportRepeats = 0;
        //public static string ExportPath = String.Empty;
        //public static int[] ChanAlloc;
        public static int ChanAllocIndex = 1;
        //public static int SysCmd = 0;
        //public static bool WindowSnap = false;
        //public static bool WindowUnsnap = false;
        //public static int VScrollbarSize = 0;
        //public static int HScrollbarSize = 0;
        //public static bool DrawOffAfterClose = false;
        //public static int MoveShift = 0;
        //public static int WinCount = 0;
        //public static bool FileAssocChanged = false;
        //public static bool SetChildAsTemplate = false;
        //public static TLastClipboard LastClipboard;
        //public static TTracksCopy TracksCopy;
        //public static TSampleCopy SampleCopy;
        //public static TChildWin OrnamentCopySrcWindow = null;
        //public static string SamplesQuickDir = String.Empty;
        //public static string OrnamentsQuickDir = String.Empty;
#if LOGGER
        public static TLogger Logger = null;
#endif
        //public static string[,] FileAssociations = { { "1", ".vt2", "VortexTracker2", "VortexTracker 2 module" }, { "1", ".vtt", "VortexTracker2Theme", "VortexTracker Color Theme" }, { "0", ".pt1", "ProTracker1", "ProTracker 1 module" }, { "0", ".pt2", "ProTracker2", "ProTracker 2 module" }, { "0", ".pt3", "ProTracker3", "ProTracker 3 module" }, { "0", ".ftc", "FastTracker", "Fast Tracker module" }, { "0", ".stc", "SoundTracker1", "SoundTracker 1.X module" }, { "0", ".stp", "SoundTrackerPro", "SoundTracker Pro module" }, { "0", ".asc", "ASCSoundMaster", "ASC Sound Master module" }, { "0", ".fls", "FlashTracker", "Flash Tracker module" }, { "0", ".gtr", "GlobalTracker", "Global Tracker module" }, { "0", ".psc", "ProSoundCreator", "Pro Sound Creator module" }, { "0", ".psm", "ProSoundMaker", "Pro Sound Maker module" }, { "0", ".sqt", "SQTracker", "SQ-Tracker module" } };
        //public static long LibHandle = 0;
        //public static uint AddFontMemResource = 0;
        //public const int StdAutoEnvMax = 7;
        //public static int[,] StdAutoEnv = { { 1, 1 }, { 3, 4 }, { 1, 2 }, { 1, 4 }, { 3, 1 }, { 5, 2 }, { 2, 1 }, { 3, 2 } };
        //public const string AppName = "Vortex Tracker";
        //public const string VersionString = "2.6";
        //public const string IsBeta = " dev";
        //public const string BetaNumber = " 17";
        //public const string VersionFullString = VersionString + IsBeta + BetaNumber;
        //public const string FullVersString = AppName + " " + VersionFullString;
        //public const string HalfVersString = "Version " + VersionFullString;
        //public const string VortexDirName = AppName + " " + VersionString + IsBeta;
        //public const string SamplesDefaultDir = "\\Instruments\\Samples";
        //public const string OrnamentsDefaultDir = "\\Instruments\\Ornaments";
        //public const string DemosongsDefaultDir = "\\Demosongs";
        //public const string FontsDir = "\\Fonts";
        //public static string[,] InternalFonts = new string[,] { { "SegoeVT", "Segoe VT" }, { "ArrowsFont", "Arrows" }, { "ConsolasFont", "Consolas" }, { "ConsolasBFont", "Consolas" }, { "CQMono", "CQ Mono" }, { "DroidSansFont", "Droid Sans Mono" }, { "ConsolaMono", "Consola Mono" }, { "ConsolaMonoBold", "Consola Mono" }, { "EtelkaMono", "Etelka Monospace Pro" }, { "EtelkaMonoBold", "Etelka Monospace Pro" }, { "RobotoMono", "Roboto Mono" }, { "RobotoMonoBold", "Roboto Mono" }, { "HackFont", "Hack" }, { "HackBFont", "Hack" }, { "LiberationFont", "Liberation Mono" }, { "LiberationBFont", "Liberation Mono" }, { "ShareTechFont", "Share Tech Mono" }, { "ZXSpectrumFont", "ZX Spectrum" }, { "Kongtext", "Kongtext" }, { "ProFontWindows", "ProFontWindows" }, { "JackInput", "JackInput" }, { "CourierNew", "Courier New" }, { "CourierNewB", "Courier New" }, { "CourierNewI", "Courier New" }, { "wstgerm", "WST_Germ" } };
        //public static string[] ModuleExtensions = { ".vt2", ".pt1", ".pt2", ".pt3", ".stc", ".stp", ".sqt", ".asc", ".psc", ".fls", ".gtr", ".ftc", ".psm", ".fxm", ".ay" };
        //public TTS TSData = new TTS("PT3!", "PT3!", "02TS");

        //public static bool IsWine = false;
        //public static bool Detected = false;

        static Main()
        {
            Panoram[0] = 64;
            Panoram[1] = 128;
            Panoram[2] = 192;
        }

        public static void InitBuffSample()
        {
            BuffSample = new Sample();
            BuffSample.Length = 1;
            BuffSample.Loop = 0;
            BuffSample.Ticks = new SampleTick[VTModule.MaxSampleLength];
            BuffSample.Ticks[0] = new SampleTick();
        }

        public static void InitBuffOrnament()
        {
            BuffOrnament = new Ornament();
            BuffOrnament.Length = 1;
            BuffOrnament.Loop = 0;
            BuffOrnament.Offsets[0] = 0;
        }

        /* public static void InitBuffTracks()
        {
            TracksCopy.Pattern = null;
            TracksCopy.Channel = 0;
            TracksCopy.FromLine = 0;
            TracksCopy.ToLine = 0;
            SampleCopy.Ready = false;
        } */
    }
}
