//#define NOREDRAW
//#define DEBUG
//#define LOGGER

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

using LibVT;
using LibVT.Plugins;
using Microsoft.Win32;
using OpenTK.Audio.OpenAL;
using RtMidi.Net;
using RtMidi.Net.Clients;
using RtMidi.Net.Enums;
using RtMidi.Net.Events;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Text;
using System.Globalization;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using static VortexTracker.Win32;

namespace VortexTracker
{
    public partial class MainForm : Form
    {
        public static ProcessPriorityClass Priority = ProcessPriorityClass.Normal;
        public static int TracksCursorXLeft = 0;
        public static int OrnXShift = 0;
        public static int OrnColumnCount = 4;
        public static int OrnCharCount = 9;
        public static bool DisableUpdateChilds = false;
        public static string SyncMessageFile = String.Empty;
        public static bool SyncVTInstances = false;
        public static string SyncSampleBufferFile = String.Empty;
        public static string SyncOrnamentBufferFile = String.Empty;
        public static string SyncSamplePartFile = String.Empty;
        public static long SyncSampleBufferFileAge = 0;
        public static long SyncOrnamentBufferFileAge = 0;
        public static long SyncSamplePartFileAge = 0;
        public static bool SyncBufferBlocked = false;
        public static bool ChildsEventsBlocked = false;
        public static bool EditorFontChanged = false;
        public static bool DisplayChanged = false;
        public static ColorTheme ColorTheme;
        public static string ColorThemeName = String.Empty;
        public static Color[] ThemeColors = new Color[(int)ThemeColor.Count];
        public static sbyte[] NoteKeys = new sbyte[256];
        public static byte[] Panoram = new byte[3];
        public static string VortexDir = String.Empty;
        public static string VortexDocumentsDir = String.Empty;
        public static string ConfigFilePath = String.Empty;
        public static string PluginsPath = String.Empty;
        public static bool EnvelopeAsNote = false;
        public static bool DupNoteParams = false;
        public static bool MoveBetweenPatrns = false;
        public static bool SamToneShiftAsNote = false;
        public static bool OrnToneShiftAsNote = false;
        public static int PositionSize = 0;
        public static bool DecBaseLinesOn = false;
        public static bool DecBaseNoiseOn = false;
        public static bool HighlightSpeedOn = false;
        public static bool DisableSeparators = false;
        public static bool AutoBackupsOn = false;
        public static byte AutoBackupsMins = 0;
        public static bool DisableHints = false;
        public static bool DisableCtrlClick = false;
        public static bool DisableInfoWin = false;
        public static bool VortexFirstStart = false;
        //public static int ManualChipFreq = 0;
        //public static int DefaultChipFreq = 0;
        //public static int DefaultIntFreq = 0;
        public static int ExportSampleRate = 0;
        public static int ExportBitRate = 0;
        public static int ExportChannels = 0;
        public static int ExportChip = 0;
        public static int ExportRepeats = 0;
        public static string ExportPath = String.Empty;
        public static int[] ChanAlloc;
        public static int ChanAllocIndex = 0;
        public static int SysCmd = 0;
        public static bool WindowSnap = false;
        public static bool WindowUnsnap = false;
        public static int VScrollbarSize = 0;
        public static int HScrollbarSize = 0;
        public static bool DrawOffAfterClose = false;
        public static int MoveShift = 0;
        public static int WinCount = 0;
        public static bool FileAssocChanged = false;
        public static bool SetChildAsTemplate = false;
        public static LastClipboard LastClipboard;
        public static TracksCopy TracksCopy;
        public static SampleCopy SampleCopy;
        public static ChildForm OrnamentCopySrcWindow = null;
        public static string SamplesQuickDir = String.Empty;
        public static string OrnamentsQuickDir = String.Empty;

#if LOGGER
        public static Logger Logger = null;
#endif
        public static string[,] FileAssociations = { { "1", ".vt2", "VortexTracker2", "VortexTracker 2 Module" }, { "1", ".vtt", "VortexTracker2Theme", "VortexTracker Color Theme" }, { "0", ".pt1", "ProTracker1", "ProTracker 1 Module" }, { "0", ".pt2", "ProTracker2", "ProTracker 2 Module" }, { "0", ".pt3", "ProTracker3", "ProTracker 3 Module" }, { "0", ".ftc", "FastTracker", "Fast Tracker Module" }, { "0", ".stc", "SoundTracker1", "SoundTracker 1.X Module" }, { "0", ".stp", "SoundTrackerPro", "SoundTracker Pro Module" }, { "0", ".asc", "ASCSoundMaster", "ASC Sound Master Module" }, { "0", ".fls", "FlashTracker", "Flash Tracker Module" }, { "0", ".gtr", "GlobalTracker", "Global Tracker Module" }, { "0", ".psc", "ProSoundCreator", "Pro Sound Creator Module" }, { "0", ".psm", "ProSoundMaker", "Pro Sound Maker Module" }, { "0", ".sqt", "SQTracker", "SQ-Tracker Module" } };
        public static long LibHandle = 0;
        public static uint AddFontMemResource = 0;
        public const int StdAutoEnvMax = 7;
        public static int[,] StdAutoEnv = { { 1, 1 }, { 3, 4 }, { 1, 2 }, { 1, 4 }, { 3, 1 }, { 5, 2 }, { 2, 1 }, { 3, 2 } };
        public static string VersionString = null;
        public static DateTime BuildDateTime;
        public static string FullVersString = null;
        public static string HalfVersString = null;
        public static string VortexDirName = null;
        public const string InstrumentsDefaultDir = "Instruments";
        public const string SamplesDefaultDir = @"Instruments\Samples";
        public const string OrnamentsDefaultDir = @"Instruments\Ornaments";
        public const string DemoSongsDefaultDir = "DemoSongs";
        public const string FontsDefaultDir = "Fonts";
        public const string PluginsDefaultDir = "Plugins";
        public const string FontsDir = "Fonts";
        public static string[] ModuleExtensions = { ".vt2", ".pt1", ".pt2", ".pt3", ".stc", ".stp", ".sqt", ".asc", ".psc", ".fls", ".gtr", ".ftc", ".psm", ".fxm", ".ay" };

        public MidiDeviceInfo MidiInputDeviceInfo;
        public MidiInputClient MidiInputClient;

        public bool EnableMidiIn = true;

        public PluginHost PluginHost = new PluginHost();

        public static InternalFont[] InternalFonts =
        {
            new InternalFont("Arrows.ttf", "Arrows", FontStyle.Regular),
            new InternalFont("Consola Mono Bold.ttf", "Consola Mono", FontStyle.Bold),
            new InternalFont("Consola Mono.ttf", "Consola Mono", FontStyle.Regular),
            new InternalFont("Consolas Bold.ttf", "Consolas", FontStyle.Bold),
            new InternalFont("Consolas.ttf", "Consolas", FontStyle.Regular),
            new InternalFont("Courier New Bold.ttf", "Courier New", FontStyle.Bold),
            new InternalFont("Courier New.ttf", "Courier New", FontStyle.Regular),
            new InternalFont("CQ Mono.ttf", "CQ Mono", FontStyle.Bold),
            new InternalFont("CQ Mono.ttf", "CQ Mono", FontStyle.Regular),
            new InternalFont("Droid Sans Mono.ttf", "Droid Sans Mono", FontStyle.Bold),
            new InternalFont("Droid Sans Mono.ttf", "Droid Sans Mono", FontStyle.Regular),
            new InternalFont("Etelka Monospace Pro Bold.ttf", "Etelka Monospace Pro", FontStyle.Bold),
            new InternalFont("Etelka Monospace Pro.ttf", "Etelka Monospace Pro", FontStyle.Regular),
            new InternalFont("Hack Bold.ttf", "Hack", FontStyle.Bold),
            new InternalFont("Hack Regular.ttf", "Hack", FontStyle.Regular),
            new InternalFont("JackInput.ttf", "JackInput", FontStyle.Bold),
            new InternalFont("JackInput.ttf", "JackInput", FontStyle.Regular),
            new InternalFont("Kongtext Regular.ttf", "Kongtext", FontStyle.Bold),
            new InternalFont("Kongtext Regular.ttf", "Kongtext", FontStyle.Regular),
            new InternalFont("Liberation Mono Bold.ttf", "Liberation Mono", FontStyle.Bold),
            new InternalFont("Liberation Mono.ttf", "Liberation Mono", FontStyle.Regular),
            new InternalFont("ProFontWindows.ttf", "ProFontWindows", FontStyle.Bold),
            new InternalFont("ProFontWindows.ttf", "ProFontWindows", FontStyle.Regular),
            new InternalFont("ProTracker2.ttf", "ProTracker2", FontStyle.Bold),
            new InternalFont("ProTracker2.ttf", "ProTracker2", FontStyle.Regular),
            new InternalFont("Roboto Mono Bold.ttf", "Roboto Mono", FontStyle.Bold),
            new InternalFont("Roboto Mono.ttf", "Roboto Mono", FontStyle.Regular),
            new InternalFont("Segoe VT.ttf", "Segoe VT", FontStyle.Bold),
            new InternalFont("Segoe VT.ttf", "Segoe VT", FontStyle.Regular),
            new InternalFont("Share Tech Mono.ttf", "Share Tech Mono", FontStyle.Bold),
            new InternalFont("Share Tech Mono.ttf", "Share Tech Mono", FontStyle.Regular),
            new InternalFont("ZX Spectrum.ttf", "ZX Spectrum", FontStyle.Bold),
            new InternalFont("ZX Spectrum.ttf", "ZX Spectrum", FontStyle.Regular),
        };

        public UIAction[] ActionList =
        {
            // File
            new UIAction(UIActionType.FileNew, "File", "&New", "New|Create a New File", 6, (Keys)16462),
            new UIAction(UIActionType.NewTurboSound, "File", "New &Turbo Sound", "", 6, (Keys)24654),
            new UIAction(UIActionType.NewTurboSound3, "File", "New Turbo Sound 3", "", 6, 0),
            new UIAction(UIActionType.JoinTracks, "File", "Join|Join Tracks...", "", -1, Keys.None),
            new UIAction(UIActionType.FileOpen, "File", "&Open", "Open|Open a File", 7, (Keys)16463),
            // OpenDemosong
            new UIAction(UIActionType.FileClose, "File", "&Close", "Close|Close Current File", -1, (Keys)16471),
            new UIAction(UIActionType.FileSave, "File", "&Save", "Save|Save Current File", 8, (Keys)16467),
            new UIAction(UIActionType.FileSaveAs, "File", "Save &As...", "Save As|Save Current File With Different Name", -1, Keys.None),
            new UIAction(UIActionType.SaveAsTwoModules, "File", "Save as 2 Modules...", "", -1, Keys.None),
            new UIAction(UIActionType.SaveAsTemplate, "File", "Save as Template", "", -1, Keys.None),
            // ---
            new UIAction(UIActionType.ExportToWAV, "File", "Export to WAV...", "", -1, Keys.None),
            new UIAction(UIActionType.ExportPSG, "File", "Export PSG...", "", -1, Keys.None),
            new UIAction(UIActionType.ExportYM, "File", "Export YM...", "", -1, Keys.None),
            //new TAction(ActionType.SaveSNDH, "File", "Save In SNDH (Atari ST)", "", -1, Keys.None),
            //new TAction(ActionType.SaveForZX, "File", "Save With ZX Spectrum Player", "", -1, Keys.None),
            // ---
            new UIAction(UIActionType.Options, "None", "Options", "", 21, Keys.None),
            // ---
            new UIAction(UIActionType.FileExit, "File", "E&xit", "Exit|Exit Application", -1, Keys.None),
            // Play
            new UIAction(UIActionType.PlayStop, "Play", "Play/Stop", "", 18, (Keys)32),
            new UIAction(UIActionType.PlayFromLine, "Play", "Play From Line", "", 18, Keys.None),
            new UIAction(UIActionType.PlayFromStart, "Play", "Play From Start", "Play From Start|Play Module From Start", 38, Keys.None),
            new UIAction(UIActionType.PlayPatternFromLine, "Play", "Play Pattern", "Play Pattern|Play Current Pattern From Current Line", 40, Keys.None),
            new UIAction(UIActionType.PlayPatternFromStart, "Play", "Play Pattern From Start", "Play Pattern From Start|Play Current Pattern From Start", 41, Keys.None),
            new UIAction(UIActionType.Stop, "Play", "Stop", "", 20, Keys.None),
            // ---
            new UIAction(UIActionType.ToggleLooping, "Play", "Toggle Looping", "Loop|Toggle Looping", 37, Keys.None),
            new UIAction(UIActionType.ToggleLoopingAll, "Play", "Toggle Looping All", "Loop All|Toggle Looping Among All Opened Files", 39, Keys.None),
            // Edit
            new UIAction(UIActionType.Undo, "Edit", "Undo", "Undo|Undo Last Change", 3, (Keys)16474),
            new UIAction(UIActionType.Redo, "Edit", "Redo", "Redo|Redo Last Undo", 4, (Keys)24666),
            // ---
            new UIAction(UIActionType.EditCut, "Edit", "Cu&t", "Cut|Cuts The Selection And Puts It On The Clipboard", 0, (Keys)16472),
            new UIAction(UIActionType.EditCopy, "Edit", "&Copy", "Copy|Copies The Selection And Puts It On The Clipboard", 1, (Keys)16451),
            new UIAction(UIActionType.EditPaste, "Edit", "&Paste", "Paste|Inserts Clipboard Contents", 2, (Keys)16470),
            // Merge
            // ---
            new UIAction(UIActionType.CopyToModPlug, "Edit", "Copy to OpenMPT", "", 44, Keys.None),
            new UIAction(UIActionType.CopyToRenoise, "Edit", "Copy to Renoise", "", 44, Keys.None),
            new UIAction(UIActionType.CopyToFami, "Edit", "Copy to FamiTracker", "", 44, Keys.None),
            // ---
            new UIAction(UIActionType.TransposeUp1, "Edit", "Transpose +1", "", -1, Keys.None),
            new UIAction(UIActionType.TransposeDown1, "Edit", "Transpose -1", "", -1, Keys.None),
            new UIAction(UIActionType.TransposeUp3, "Edit", "Transpose +3", "", -1, Keys.None),
            new UIAction(UIActionType.TransposeDown3, "Edit", "Transpose -3", "", -1, Keys.None),
            new UIAction(UIActionType.TransposeUp5, "Edit", "Transpose +5", "", -1, Keys.None),
            new UIAction(UIActionType.TransposeDown5, "Edit", "Transpose -5", "", -1, Keys.None),
            new UIAction(UIActionType.TransposeUp12, "Edit", "Transpose +12", "", -1, Keys.None),
            new UIAction(UIActionType.TransposeDown12, "Edit", "Transpose -12", "", -1, Keys.None),
            // ---
            new UIAction(UIActionType.ExpandPattern, "Edit", "Expand Pattern", "", -1, Keys.None),
            new UIAction(UIActionType.CompressPattern, "Edit", "Compress Pattern", "", -1, Keys.None),
            new UIAction(UIActionType.SplitPattern, "None", "Split Pattern", "", -1, Keys.None),
            new UIAction(UIActionType.PatternPacker, "Edit", "Pattern Packer", "", -1, Keys.None),
            // ---
            new UIAction(UIActionType.SwapChannelsLeft, "Edit", "Swap Channels Left", "", -1, (Keys)16421),
            new UIAction(UIActionType.SwapChannelsRight, "Edit", "Swap Channels Right", "", -1, (Keys)16423),
            // ---
            new UIAction(UIActionType.ToggleSamples, "None", "Toggle Samples", "Toggle Samples|Switch On/Off Standalone Samples", 31, Keys.None),
            new UIAction(UIActionType.TracksManager, "None", "Tracks Manager", "Tracks Manager|Copy/Move/Swap/Transpose Tracks", 42, Keys.None),
            new UIAction(UIActionType.GlobalTransposition, "None", "Global Transposition", "Global Transposition|Transpose Channel/Pattern/Module", 36, Keys.None),
            new UIAction(UIActionType.PluginManager, "None", "Plugin Manager", "Plugin Manager", 53, Keys.None),
            // Window
            new UIAction(UIActionType.Maximize, "Window", "&Maximize", "", -1, Keys.None),
            new UIAction(UIActionType.Normal, "Window", "&Normal", "", -1, (Keys)49223),
            // Help
            new UIAction(UIActionType.HelpAbout, "Help", "&About...", "About|Displays Program Information, Version Number, And Copyright", -1, Keys.None),
            // SetColor
            // ResetColors
            // ---
            new UIAction(UIActionType.SetLoopPosition, "Edit", "Set Loop Position", "Loop|Set Loop Position", -1, (Keys)76),
            // ---
            new UIAction(UIActionType.InsertPositions, "Edit", "Insert Position", "Insert Position", -1, (Keys)45),
            new UIAction(UIActionType.DeletePositions, "Edit", "Delete Position", "Delete Position", -1, (Keys)46),
            new UIAction(UIActionType.DuplicatePositions, "Edit", "Duplicate Positions", "Duplicate Positions", -1, Keys.None),
            new UIAction(UIActionType.ClonePositions, "Edit", "Clone Positions", "Clone Positions", -1, Keys.None),
            // ---
            // ChangePatternsLength
            // ---
            // RenumberPatternsLength
            // FillEmptyPositions
            new UIAction(UIActionType.ToggleChip, "None", "YM", "Chip|Toggle sound chip", -1, Keys.None),
            new UIAction(UIActionType.ToggleChannels, "None", "ABC", "Channels|Toggle channel allocation", -1, Keys.None),
            // ---
            new UIAction(UIActionType.UseLastNoteParams, "Edit", "Duplicate Last Note Params", "", -1, (Keys)8219),
            new UIAction(UIActionType.MoveBetweenPatterns, "Edit", "Move Between Patterns", "", -1, (Keys)8384),
            // ---
            new UIAction(UIActionType.AutoStep0, "None", "Set Autostep to 0", "", -1, (Keys)16432),
            new UIAction(UIActionType.AutoStep1, "None", "AutoStep1", "", -1, (Keys)16433),
            new UIAction(UIActionType.AutoStep2, "None", "AutoStep2", "", -1, (Keys)16434),
            new UIAction(UIActionType.AutoStep3, "None", "AutoStep3", "", -1, (Keys)16435),
            new UIAction(UIActionType.AutoStep4, "None", "AutoStep4", "", -1, (Keys)16436),
            new UIAction(UIActionType.AutoStep5, "None", "AutoStep5", "", -1, (Keys)16437),
            new UIAction(UIActionType.AutoStep6, "None", "AutoStep6", "", -1, (Keys)16438),
            new UIAction(UIActionType.AutoStep7, "None", "AutoStep7", "", -1, (Keys)16439),
            new UIAction(UIActionType.AutoStep8, "None", "AutoStep8", "", -1, (Keys)16440),
            new UIAction(UIActionType.AutoStep9, "None", "AutoStep9", "", -1, (Keys)16441),
            // ---
            new UIAction(UIActionType.JumpPatternStart, "Edit", "HK_JMP_PAT_START", "", -1, Keys.None),
            new UIAction(UIActionType.JumpPatternEnd, "Edit", "JmpPatEndAct", "", -1, Keys.None),
            new UIAction(UIActionType.JumpLineStart, "Edit", "JmpLineStartAct", "", -1, Keys.None),
            new UIAction(UIActionType.JumpLineEnd, "Edit", "JmpLineEndAct", "", -1, Keys.None),
            // ---
        };

        public static bool IsWine = false;
        public static bool Detected = false;

        private static readonly List<IntPtr> _allocatedFontPtrs = new();
        public static PrivateFontCollection PrivateFontCollection = null;

        // Public declarations
        public int PrevTop = 0;
        public int WidthFix = 0;
        public int HeightFix = 0;
        public bool SizeFixed = false;
        public bool SnappedToRight = false;
        public bool Snaped = false;
        public bool MaximizeChilds = false;
        //public int Tone_Table_On_Load = 0;
        public Font EditorFont = null;
        public Font TestLineFont = null;
        public string[] RecentFiles;
        //public bool LoopAllAllowed = false;
        // 
        // // Sample templates is disabled now
        // 
        // SampleLineTemplates: array of TSampleTick;
        // CurrentSampleLineTemplate: integer;
        // 
        //public int GlobalVolume = 0;
        //public int GlobalVolumeMax = 0;
        public short DefaultTable = 0;
        public byte StartupAction = 0;
        public bool StartupOpenModule = false;
        public bool StartupOpenTheme = false;
        public string TemplateSongPath = String.Empty;
        public int WinThemeIndex = 0;
        // samples and ornaments for global buffer, for copy/paste
        //public TSample BuffSample = null;
        //public TOrnament BuffOrnament = null;
        public bool NumberOfLinesChanged = false;
        public bool ResizeActionBlocked = false;
        public bool SampleBrowserVisible = false;
        public bool OrnamentsBrowserVisible = false;
        public bool VTExit = false;
        public int LastChildWidth = 0;
        public int LastChildHeight = 0;
        public string SamplesDir = String.Empty;
        public string OrnamentsDir = String.Empty;
        public bool RedrawEnabled = false;
        public ChildForm[] ChildsTable = null;
        public bool DontAddToRecent = false;

        private readonly System.Windows.Forms.Timer _appEventTimer = new();
        private readonly System.Windows.Forms.Timer _renderTimer = new();

        public MainForm()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.Opaque, true);
            this.UpdateStyles();

            TracksCopy = new TracksCopy();
            SampleCopy = new SampleCopy();
            ChildsTable = new ChildForm[0];

            this.Text = Application.ProductName;

            MainMenu1.ImageList = ImageList1;
            PopupMenu1.ImageList = ImageList1;
            PopupMenu2.ImageList = ImageList1;
            PopupMenu3.ImageList = ImageList1;

            UIActionManager.Instance.AddActions(ActionList);

            // File
            UIActionManager.Instance.AddEvents(UIActionType.FileNew, FileNew_Execute, FileNew_Update);
            UIActionManager.Instance.AddEvents(UIActionType.NewTurboSound, NewTurboSound_Execute, NewTurboSound_Update);
            UIActionManager.Instance.AddEvents(UIActionType.NewTurboSound3, NewTurboSound3_Execute, NewTurboSound3_Update);
            UIActionManager.Instance.AddEvents(UIActionType.JoinTracks, JoinTracks_Execute, JoinTracks_Update);
            UIActionManager.Instance.AddEvents(UIActionType.FileOpen, FileOpen_Execute, FileOpen_Update);
            // OpenDemosong
            UIActionManager.Instance.AddEvents(UIActionType.FileClose, FileClose_Execute, FileClose_Update);
            UIActionManager.Instance.AddEvents(UIActionType.FileSave, FileSave_Execute, FileSave_Update);
            UIActionManager.Instance.AddEvents(UIActionType.FileSaveAs, FileSaveAs_Execute, FileSaveAs_Update);
            UIActionManager.Instance.AddEvents(UIActionType.SaveAsTwoModules, SaveAsTwoModules_Execute, SaveAsTwoModules_Update);
            UIActionManager.Instance.AddEvents(UIActionType.SaveAsTemplate, SaveAsTemplate_Execute, SaveAsTemplate_Update);
            // ---
            UIActionManager.Instance.AddEvents(UIActionType.ExportToWAV, ExportToWAV_Execute, ExportToWAV_Update);
            UIActionManager.Instance.AddEvents(UIActionType.ExportPSG, ExportPSG_Execute, ExportPSG_Update);
            UIActionManager.Instance.AddEvents(UIActionType.ExportYM, ExportYM_Execute, ExportYM_Update);
            // SaveSNDH
            // SaveForZX
            // ---
            UIActionManager.Instance.AddEvents(UIActionType.Options, Options_Execute, null);
            // ---
            UIActionManager.Instance.AddEvents(UIActionType.FileExit, FileExit_Execute, null);
            // Play
            UIActionManager.Instance.AddEvents(UIActionType.PlayStop, PlayStop_Execute, PlayStop_Update);
            UIActionManager.Instance.AddEvents(UIActionType.PlayFromLine, PlayFromLine_Execute, PlayFromLine_Update);
            UIActionManager.Instance.AddEvents(UIActionType.PlayFromStart, PlayFrom_Execute, PlayFrom_Update);
            UIActionManager.Instance.AddEvents(UIActionType.PlayPatternFromLine, PlayPatternFromLine_Execute, PlayPatternFromLine_Update);
            UIActionManager.Instance.AddEvents(UIActionType.PlayPatternFromStart, PlayPatternFromStart_Execute, PlayPatternFromStart_Update);
            UIActionManager.Instance.AddEvents(UIActionType.Stop, Stop_Execute, Stop_Update);
            // ---
            UIActionManager.Instance.AddEvents(UIActionType.ToggleLooping, ToggleLooping_Execute, ToggleLooping_Update);
            UIActionManager.Instance.AddEvents(UIActionType.ToggleLoopingAll, ToggleLoopingAll_Execute, ToggleLoopingAll_Update);
            // Edit
            UIActionManager.Instance.AddEvents(UIActionType.Undo, Undo_Execute, Undo_Update);
            UIActionManager.Instance.AddEvents(UIActionType.Redo, Redo_Execute, Redo_Update);
            // ---
            UIActionManager.Instance.AddEvents(UIActionType.EditCut, EditCut_Execute, EditCut_Update);
            UIActionManager.Instance.AddEvents(UIActionType.EditCopy, EditCopy_Execute, EditCopy_Update);
            UIActionManager.Instance.AddEvents(UIActionType.EditPaste, EditPaste_Execute, EditPaste_Update);
            // Merge
            // ---
            UIActionManager.Instance.AddEvents(UIActionType.CopyToModPlug, CopyToModPlug_Execute, CopyToModPlug_Update);
            UIActionManager.Instance.AddEvents(UIActionType.CopyToRenoise, CopyToRenoise_Execute, CopyToRenoise_Update);
            UIActionManager.Instance.AddEvents(UIActionType.CopyToFami, CopyToFami_Execute, CopyToFami_Update);
            // ---
            UIActionManager.Instance.AddEvents(UIActionType.TransposeUp1, TransposeUp1_Execute, TransposeUp1_Update);
            UIActionManager.Instance.AddEvents(UIActionType.TransposeDown1, TransposeDown1_Execute, TransposeDown1_Update);
            UIActionManager.Instance.AddEvents(UIActionType.TransposeUp3, TransposeUp3_Execute, TransposeUp3_Update);
            UIActionManager.Instance.AddEvents(UIActionType.TransposeDown3, TransposeDown3_Execute, TransposeDown3_Update);
            UIActionManager.Instance.AddEvents(UIActionType.TransposeUp5, TransposeUp5_Execute, TransposeUp5_Update);
            UIActionManager.Instance.AddEvents(UIActionType.TransposeDown5, TransposeDown5_Execute, TransposeDown5_Update);
            UIActionManager.Instance.AddEvents(UIActionType.TransposeUp12, TransposeUp12_Execute, TransposeUp12_Update);
            UIActionManager.Instance.AddEvents(UIActionType.TransposeDown12, TransposeDown12_Execute, TransposeDown12_Update);
            // ---
            UIActionManager.Instance.AddEvents(UIActionType.ExpandPattern, ExpandPattern_Execute, null);
            UIActionManager.Instance.AddEvents(UIActionType.CompressPattern, CompressPattern_Execute, null);
            UIActionManager.Instance.AddEvents(UIActionType.SplitPattern, SplitPattern_Execute, null);
            UIActionManager.Instance.AddEvents(UIActionType.PatternPacker, PatternPacker_Execute, null);
            // ---
            UIActionManager.Instance.AddEvents(UIActionType.SwapChannelsLeft, SwapChannelsLeft_Execute, SwapChannelsLeft_Update);
            UIActionManager.Instance.AddEvents(UIActionType.SwapChannelsRight, SwapChannelsRight_Execute, SwapChannelsRight_Update);
            // ---
            UIActionManager.Instance.AddEvents(UIActionType.ToggleSamples, ToggleSamples_Execute, ToggleSamples_Update);
            UIActionManager.Instance.AddEvents(UIActionType.TracksManager, TracksManager_Execute, TracksManager_Update);
            UIActionManager.Instance.AddEvents(UIActionType.GlobalTransposition, GlobalTransposition_Execute, GlobalTransposition_Update);
            UIActionManager.Instance.AddEvents(UIActionType.PluginManager, PluginManager_Execute, PluginManager_Update);
            // Window
            UIActionManager.Instance.AddEvents(UIActionType.Maximize, Maximize_Execute, null);
            UIActionManager.Instance.AddEvents(UIActionType.Normal, Normal_Execute, null);
            // Help
            UIActionManager.Instance.AddEvents(UIActionType.HelpAbout, HelpAbout_Execute, null);
            // SetColor
            // ResetColors
            // ---
            UIActionManager.Instance.AddEvents(UIActionType.SetLoopPosition, SetLoopPosition_Execute, SetLoopPosition_Update);
            // ---
            UIActionManager.Instance.AddEvents(UIActionType.InsertPositions, InsertPositions_Execute, InsertPositions_Update);
            UIActionManager.Instance.AddEvents(UIActionType.DeletePositions, DeletePositions_Execute, DeletePositions_Update);
            UIActionManager.Instance.AddEvents(UIActionType.DuplicatePositions, DuplicatePositions_Execute, null);
            UIActionManager.Instance.AddEvents(UIActionType.ClonePositions, ClonePositions_Execute, null);
            // ---
            // ChangePatternsLength
            // ---
            // RenumberPatternsLength
            // FillEmptyPositions
            UIActionManager.Instance.AddEvents(UIActionType.ToggleChip, ToggleChip_Execute, null);
            UIActionManager.Instance.AddEvents(UIActionType.ToggleChannels, ToggleChanAlloc_Execute, null);
            // ---
            UIActionManager.Instance.AddEvents(UIActionType.UseLastNoteParams, DuplicateLastNoteParams_Execute, null);
            UIActionManager.Instance.AddEvents(UIActionType.MoveBetweenPatterns, MoveBetweenPatterns_Execute, null);
            // ---
            UIActionManager.Instance.AddEvents(UIActionType.AutoStep0, AutoStep0_Execute, null);
            UIActionManager.Instance.AddEvents(UIActionType.AutoStep1, AutoStep1_Execute, null);
            UIActionManager.Instance.AddEvents(UIActionType.AutoStep2, AutoStep2_Execute, null);
            UIActionManager.Instance.AddEvents(UIActionType.AutoStep3, AutoStep3_Execute, null);
            UIActionManager.Instance.AddEvents(UIActionType.AutoStep4, AutoStep4_Execute, null);
            UIActionManager.Instance.AddEvents(UIActionType.AutoStep5, AutoStep5_Execute, null);
            UIActionManager.Instance.AddEvents(UIActionType.AutoStep6, AutoStep6_Execute, null);
            UIActionManager.Instance.AddEvents(UIActionType.AutoStep7, AutoStep7_Execute, null);
            UIActionManager.Instance.AddEvents(UIActionType.AutoStep8, AutoStep8_Execute, null);
            UIActionManager.Instance.AddEvents(UIActionType.AutoStep9, AutoStep9_Execute, null);
            // ---
            UIActionManager.Instance.AddEvents(UIActionType.JumpPatternStart, null, null);
            UIActionManager.Instance.AddEvents(UIActionType.JumpPatternEnd, null, null);
            UIActionManager.Instance.AddEvents(UIActionType.JumpLineStart, null, null);
            UIActionManager.Instance.AddEvents(UIActionType.JumpLineEnd, null, null);

            // ============================

            // File
            UIActionManager.Instance.AddComponents(UIActionType.FileNew, FileNewItem, NewButton);
            UIActionManager.Instance.AddComponents(UIActionType.NewTurboSound, NewTurboSound1);
            UIActionManager.Instance.AddComponents(UIActionType.NewTurboSound3, NewTurboSound3);
            UIActionManager.Instance.AddComponents(UIActionType.JoinTracks, JoinTracks);
            UIActionManager.Instance.AddComponents(UIActionType.FileOpen, FileOpen, FileOpenButton);
            // OpenDemosong
            UIActionManager.Instance.AddComponents(UIActionType.FileClose, FileClose);
            UIActionManager.Instance.AddComponents(UIActionType.FileSave, FileSave, FileSaveButton);
            UIActionManager.Instance.AddComponents(UIActionType.FileSaveAs, FileSaveAs);
            UIActionManager.Instance.AddComponents(UIActionType.SaveAsTwoModules, SaveAsTwoModules);
            UIActionManager.Instance.AddComponents(UIActionType.SaveAsTemplate, SaveAsTemplate);
            // ---
            UIActionManager.Instance.AddComponents(UIActionType.ExportToWAV, ExportWAV);
            UIActionManager.Instance.AddComponents(UIActionType.ExportPSG, ExportPSG);
            UIActionManager.Instance.AddComponents(UIActionType.ExportYM, ExportYM);
            // SaveSNDH
            // SaveForZX
            // ---
            UIActionManager.Instance.AddComponents(UIActionType.Options, Options, OptionsButton);
            // ---
            UIActionManager.Instance.AddComponents(UIActionType.FileExit, FileExit);
            // Play
            UIActionManager.Instance.AddComponents(UIActionType.PlayStop, PlayStop, PlayStopButton);
            UIActionManager.Instance.AddComponents(UIActionType.PlayFromLine, PlayFromLine);
            UIActionManager.Instance.AddComponents(UIActionType.PlayFromStart, PlayFromStart, PlayFromStartButton);
            UIActionManager.Instance.AddComponents(UIActionType.PlayPatternFromLine, PlayPatternFromCurrentLine, PlayPatternFromCurrentLineButton);
            UIActionManager.Instance.AddComponents(UIActionType.PlayPatternFromStart, PlayPatternFromStart, PlayPatternFromStartButton);
            UIActionManager.Instance.AddComponents(UIActionType.Stop, Stop);
            // ---
            UIActionManager.Instance.AddComponents(UIActionType.ToggleLooping, ToggleLooping, ToggleLoopingButton);
            UIActionManager.Instance.AddComponents(UIActionType.ToggleLoopingAll, ToggleLoopingAll, LoopAllFilesButton);
            // Edit
            UIActionManager.Instance.AddComponents(UIActionType.Undo, Undo1, Undo2, UndoButton);
            UIActionManager.Instance.AddComponents(UIActionType.Redo, Redo1, Redo2, RedoButton);
            // ---
            UIActionManager.Instance.AddComponents(UIActionType.EditCut, EditCut1, EditCut2);
            UIActionManager.Instance.AddComponents(UIActionType.EditCopy, EditCopy1, EditCopy2);
            UIActionManager.Instance.AddComponents(UIActionType.EditPaste, EditPaste1, EditPaste2);
            // Merge
            // ---
            UIActionManager.Instance.AddComponents(UIActionType.CopyToModPlug, CopyToModPlug1, CopyToModPlug2);
            UIActionManager.Instance.AddComponents(UIActionType.CopyToRenoise, CopyToRenoise1, CopyToRenoise2);
            UIActionManager.Instance.AddComponents(UIActionType.CopyToFami, CopyToFami1, CopyToFami2);
            // ---
            UIActionManager.Instance.AddComponents(UIActionType.TransposeUp1, TransposeUp1);
            UIActionManager.Instance.AddComponents(UIActionType.TransposeDown1, TransposeDown1);
            UIActionManager.Instance.AddComponents(UIActionType.TransposeUp3, TransposeUp3);
            UIActionManager.Instance.AddComponents(UIActionType.TransposeDown3, TransposeDown3);
            UIActionManager.Instance.AddComponents(UIActionType.TransposeUp5, TransposeUp5);
            UIActionManager.Instance.AddComponents(UIActionType.TransposeDown5, TransposeDown5);
            UIActionManager.Instance.AddComponents(UIActionType.TransposeUp12, TransposeUp12);
            UIActionManager.Instance.AddComponents(UIActionType.TransposeDown12, TransposeDown12);
            // ---
            UIActionManager.Instance.AddComponents(UIActionType.ExpandPattern, ExpandPattern);
            UIActionManager.Instance.AddComponents(UIActionType.CompressPattern, CompressPattern);
            UIActionManager.Instance.AddComponents(UIActionType.SplitPattern, SplitPattern);
            UIActionManager.Instance.AddComponents(UIActionType.PatternPacker, PatternPacker);
            // ---
            UIActionManager.Instance.AddComponents(UIActionType.SwapChannelsLeft, SwapChannelsLeft);
            UIActionManager.Instance.AddComponents(UIActionType.SwapChannelsRight, SwapChannelsRight);
            // ---
            UIActionManager.Instance.AddComponents(UIActionType.ToggleSamples, ToggleSamples, ToggleSamples1, ToggleSamplesButton);
            UIActionManager.Instance.AddComponents(UIActionType.TracksManager, TracksManager, TracksManagerButton);
            UIActionManager.Instance.AddComponents(UIActionType.GlobalTransposition, GlobalTransposition, GlobalTranspositionButton);
            UIActionManager.Instance.AddComponents(UIActionType.PluginManager, PluginManager, PluginManagerButton);
            // Window
            UIActionManager.Instance.AddComponents(UIActionType.Maximize, Maximize);
            UIActionManager.Instance.AddComponents(UIActionType.Normal, Normal);
            // Help
            UIActionManager.Instance.AddComponents(UIActionType.HelpAbout, HelpAbout);
            // SetColor
            // ResetColors
            // ---
            UIActionManager.Instance.AddComponents(UIActionType.SetLoopPosition, SetLoopPosition);
            // ---
            UIActionManager.Instance.AddComponents(UIActionType.InsertPositions, InsertPosition);
            UIActionManager.Instance.AddComponents(UIActionType.DeletePositions, DeletePosition);
            UIActionManager.Instance.AddComponents(UIActionType.DuplicatePositions, DuplicatePosition);
            UIActionManager.Instance.AddComponents(UIActionType.ClonePositions, ClonePosition);
            // ---
            // ChangePatternsLength
            // ---
            // RenumberPatternsLength
            // FillEmptyPositions
            UIActionManager.Instance.AddComponents(UIActionType.ToggleChip, ChipButton);
            UIActionManager.Instance.AddComponents(UIActionType.ToggleChannels, ChannelsButton);
            // ---
            //ActionManager.Instance.AddComponents(ActionType.UseLastNoteParams, null);
            //ActionManager.Instance.AddComponents(ActionType.MoveBetweenPatterns, null);
            // AutoStep0
            // AutoStep1
            // AutoStep2
            // AutoStep3
            // AutoStep4
            // AutoStep5
            // AutoStep6
            // AutoStep7
            // AutoStep8
            // AutoStep9
            // ---
            // JumpPatternStart
            // JumpPatternEnd
            // JumpLineStart
            // JumpLineEnd
            // ---

            AppEvents.IsOnUIThread = () => !InvokeRequired;
            AppEvents.Dispatcher = OnAppEvent;

            /* Application.Idle += (s, e) =>
            {
                //foreach (var action in ActionList1)
                //    action.Update(s, e);

                //while (AppEvents.TryDequeue(out var evt))
                //    OnAppEvent(evt);
            }; */

            AY.RegisterEvent += OnRegisterEvent;
            VTModule.PlaybackEvent += OnPlaybackEvent;

            SetupAppEventTimer();
            SetupRenderTimer();
            //SerialOut.StartPump();

            //AppEvents.MessageBox += OnMessageBox;
            //AppEvents.PostMessage += OnPostMessage;
            //AppEvents.PeekMessage += OnPeekMessage;
            //AppEvents.AppEvent += OnAppEvent;
            //AppEvents.FXMDialog += OnFXMDialog;
            //Application.ThreadException += AppException;
            SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;

            
            //Application.ThreadException += (s, e) => MessageBox.Show(e.Exception.ToString());
            //AppDomain.CurrentDomain.UnhandledException += (s, e) => MessageBox.Show(e.ToString());
        }

        private void SetupAppEventTimer()
        {
            _appEventTimer.Interval = 1;
            _appEventTimer.Tick += (s, e) =>
            {
                while (AppEvents.TryDequeue(out var evt))
                    OnAppEvent(evt);
            };
            _appEventTimer.Start();
        }

        private void SetupRenderTimer()
        {
            _renderTimer.Interval = 1;
            _renderTimer.Tick += (s, e) =>
            {
                if (WaveOutAPI.ExportStarted)
                    return;

                if (WaveOutAPI.IsPlaying && (AY.PlayMode == PlayModes.PlayModule || AY.PlayMode == PlayModes.PlayPattern))
                {
                    AL.GetSource(WaveOutAPI.ALSource, ALGetSourcei.SampleOffset, out int offsetSamples);
                    uint totalSamplePos = (uint)(WaveOutAPI.TotalSamplesPlayed + offsetSamples);
                    uint curVisPos = totalSamplePos % WaveOutAPI.PlayGridLength;
                    UmRedrawTracks(curVisPos);
                }
                else
                    UmRedrawTracks();
            };

            _renderTimer.Start();
        }

        private void OnFXMDialog(FxmParamsEventArgs e)
        {
            using (FxmParamsForm fxmParamsForm = new FxmParamsForm())
            {
                fxmParamsForm.LengthBox.Text = e.Length.ToString();
                fxmParamsForm.LoopInterruptBox.Text = e.LoopInterrupt.ToString();
                fxmParamsForm.InitialTempoBox.Text = e.InitialTempo.ToString();
                fxmParamsForm.PatternSizeBox.Text = e.PatternSize.ToString();
                fxmParamsForm.GlobalTransposeBox.Text = e.GlobalTranspose.ToString();
                fxmParamsForm.AmadAndSixBox.Text = e.AmadAndSix.ToString();

                if (fxmParamsForm.ShowDialog(this) == DialogResult.OK)
                {
                    e.Length = Convert.ToInt32(fxmParamsForm.LengthBox.Text.Trim());
                    e.LoopInterrupt = Convert.ToInt32(fxmParamsForm.LoopInterruptBox.Text.Trim());
                    e.InitialTempo = Convert.ToInt32(fxmParamsForm.InitialTempoBox.Text.Trim());
                    e.PatternSize = Convert.ToInt32(fxmParamsForm.PatternSizeBox.Text.Trim());
                    e.GlobalTranspose = Convert.ToInt32(fxmParamsForm.GlobalTransposeBox.Text.Trim());
                    e.AmadAndSix = Convert.ToInt32(fxmParamsForm.AmadAndSixBox.Text.Trim());
                }
            }
        }

        public bool VScrollVisible(int newHeight)
        {
            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[i];

                if (childForm.Top < 0)
                    return true;
            }

            return this.Top + ChildsBottom() + OuterHeight() + ToolStrip1.Height + StatusBar.Height - 6 > Screen.FromControl(this).WorkingArea.Bottom;
        }

        public IntPtr ClientHandle()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is MdiClient client)
                    return client.Handle;
            }

            return IntPtr.Zero;
        }

        public bool HScrollVisible(int newLeft)
        {
            if (this.MdiChildren.Length < 2)
                return false;

            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[i];

                if (childForm.Left < 0)
                    return true;
            }

            // + BorderSize
            return newLeft + ChildsRight() > Screen.FromControl(this).WorkingArea.Right;
        }

        public int VScrollSize(int newHeight)
        {
            if (VScrollVisible(newHeight))
                return VScrollbarSize;

            return 0;
        }

        public int HScrollSize(int newLeft)
        {
            if (HScrollVisible(newLeft))
                return HScrollbarSize;

            return 0;
        }

        public int BorderSize()
        {
            return (this.Width - this.ClientSize.Width) / 2;
        }

        public int DoubleBorderSize()
        {
            return this.Width - this.ClientSize.Width;
        }

        public int AbsTop()
        {
            return this.Top - Screen.FromControl(this).WorkingArea.Top;
        }

        public int OuterHeight()
        {
            return this.Height - this.ClientSize.Height;
        }

        public int MonitorWorkAreaWidth()
        {
            return Screen.FromControl(this).WorkingArea.Width;
        }

        public int MonitorWorkAreaHeight()
        {
            return Screen.FromControl(this).WorkingArea.Height;
        }

        public int WorkAreaHeight(int newHeight)
        {
            //return newHeight - ToolStrip1.Height - StatusBar.Height - 5;
            return newHeight - MainMenu1.Height - ToolStrip1.Height - StatusBar.Height - 5;
        }

        private bool OneTrack()
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            return (MdiChildren.Length == 1)
                || (MdiChildren.Length == 2 && activeForm.TSWindow[0] != null)
                || (MdiChildren.Length == 3
                    && activeForm.TSWindow[0] != null
                    && activeForm.TSWindow[1] != null);
        }

        public bool ResizeChildsHeight()
        {
            bool result = OneTrack();

            if (result)
                return result;

            result = true;

            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                if (this.MdiChildren[i].Top != 0)
                    return false;
            }

            return result;
        }

        public int ChildsWidth()
        {
            int result;
            int childCount = this.MdiChildren.Length;

            if (childCount == 0)
                result = LastChildWidth;
            else if (childCount == 1)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[0];
                result = childForm.TabControl.Width;
            }
            else
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[0];
                result = childForm.Width * childCount + 5;
            }

            return result;
        }

        public int ChildsRight()
        {
            int result;
            int childsCount = this.MdiChildren.Length;
            int right;

            if (childsCount == 0)
                result = LastChildWidth;
            else if (childsCount == 1)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[0];
                result = childForm.TabControl.Width;
            }
            else
            {
                result = 0;

                for (int i = 0; i < childsCount; i++)
                {
                    right = this.MdiChildren[i].Left + this.MdiChildren[i].Width;

                    if (right > result)
                        result = right;
                }

                result += 5;
            }

            return result;
        }

        public int ChildsBottom()
        {
            int result;
            int childCount = this.MdiChildren.Length;
            int bottom;

            if (childCount == 0)
            {
                result = LastChildHeight;
            }
            else if (childCount == 1)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[0];
                result = childForm.TabControl.Height;
            }
            else
            {
                result = 0;

                for (int i = 0; i < childCount; i++)
                {
                    bottom = this.MdiChildren[i].Top + this.MdiChildren[i].Height;

                    if (bottom > result)
                        result = bottom;
                }

                result += 5;
            }

            return result;
        }

        public int ChildsMaxHeight()
        {
            if (this.MdiChildren.Length == 1)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[0];
                return childForm.TabControl.Height;
            }

            int result = 0;

            for (int i = 0; i < this.MdiChildren.Length; i++)
                result = Math.Max(result, this.MdiChildren[i].Height);

            return result;
        }

        public void SetWidth(int clientWidth, bool fixedWidth)
        {
            bool oldBlock = ResizeActionBlocked;   // Flag := ResizeActionBlocked;
            ResizeActionBlocked = true;

            ResetConstraints(false);
            ClientSize = new Size(clientWidth, ClientSize.Height);

            if (fixedWidth)
            {
                MinimumSize = new Size(clientWidth + DoubleBorderSize(), MinimumSize.Height);
                MaximumSize = new Size(clientWidth + DoubleBorderSize(), int.MaxValue);
            }

            ResizeActionBlocked = oldBlock;
        }

        public void FixWidth()
        {
            int width = Width;
            MinimumSize = new Size(width, MinimumSize.Height);
            MaximumSize = new Size(width, int.MaxValue);
        }

        public void ResetConstraints(bool calculateMinWidth)
        {
            this.MaximumSize = Size.Empty;
            this.MinimumSize = new Size(0, 570);

            if (this.MdiChildren.Length < 2)
                return;

            if (!calculateMinWidth)
                return;

            if (ChildsWidth() > this.ClientSize.Width)
                return;

            int rightX = ChildsRight() - 5;
            int minWidth = rightX + DoubleBorderSize() + 5;

            if (minWidth <= MonitorWorkAreaWidth())
                this.MinimumSize = new Size(minWidth, 570);
            else
                this.MinimumSize = new Size(MonitorWorkAreaWidth(), 570);
        }

        /* public override bool IsShortCut(ref TWMKey Message)
        {
            bool result;
            if ((this.MdiChildren.Length != 0) && (((TChildWin)this.ActiveMdiChild).ActiveControl is TextBox) && (Message.CharCode != 27))
            {
                result = false;
            }
            else
            {
                result = HotKeys.IsShortcut(ref Message);
            }
            return result;
        } */

        public bool NoPatterns()
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            if (activeForm.VTM.Positions.Length == 0)
                return true;

            if (activeForm.TSWindow[0] != null && activeForm.TSWindow[0].VTM.Positions.Length == 0)
                return true;

            if (activeForm.TSWindow[1] != null && activeForm.TSWindow[1].VTM.Positions.Length == 0)
                return true;

            return false;
        }

        public void SetChildsTab(int tab)
        {
            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[i];

                if (childForm.TabControl.SelectedTab == childForm.OrnamentsTab && tab == 2)
                    childForm.TabControl.SelectedIndex = 2;
                else
                    childForm.TabControl.SelectedIndex = tab - 1;
            }

            // for i := 0 to MDIChildCount-1 do
            // with TMDIChild(MDIChildren[i]) do
            // if (PageControl1.ActivePage = PatternsSheet) and (Tab <> 1) then
            // PageControl1.ActivePage := SamplesSheet
            // else
            // PageControl1.ActivePage := PatternsSheet;
            // case Tab of
            // 1: PageControl1.ActivePage := PatternsSheet;
            // 2: PageControl1.ActivePage := SamplesSheet;
            // end;
        }

        public void AlignRightSnapped(Rectangle newSize, FormWindowState mainWindowState, bool moveChild)
        {
            Rectangle monitorRect = Screen.FromControl(this).WorkingArea;

            if (SnappedToRight && (newSize.Left + newSize.Width + DoubleBorderSize() < monitorRect.Left))
                newSize.X = monitorRect.Left - newSize.Width - BorderSize() - 1;
        }

        public Rectangle GetSizeForChilds(FormWindowState mainWindowState, bool moveChild)
        {
            Rectangle result = Rectangle.Empty;
            int childCount = this.MdiChildren.Length;
            Rectangle monitorRect = Screen.FromControl(this).WorkingArea;
            result.Y = this.Top;
            result.X = this.Left;
            result.Width = this.ClientSize.Width;
            result.Height = this.ClientSize.Height;

            if ((childCount == 0 && LastChildHeight == 0) || mainWindowState == FormWindowState.Maximized)
                return result;

            if (childCount == 0 && LastChildWidth != 0)
                result.Width = LastChildWidth;

            if (childCount == 1)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[0];
                result.Width = childForm.TabControl.Width;
            }

            AlignRightSnapped(result, mainWindowState, moveChild);

            if (childCount < 2)
                return result;

            // ChildCount > 1
            if (moveChild)
            {
                result.Width = ChildsRight();
                result.Height = ChildsBottom() + ToolStrip1.Height + StatusBar.Height;
            }
            else
                result.Width = ChildsWidth();

            if (!moveChild && !WindowUnsnap && SysCmd != SC_RESTORE)
            {
                if (this.Left + result.Width + DoubleBorderSize() > monitorRect.Right)
                {
                    if (result.Width + DoubleBorderSize() > MonitorWorkAreaWidth())
                    {
                        result.X = monitorRect.Left - BorderSize() + 1;
                        result.Width = MonitorWorkAreaWidth();
                    }
                    else
                        result.X = monitorRect.Right - result.Width - BorderSize() - 1;
                }
            }

            AlignRightSnapped(result, mainWindowState, moveChild);

            if (moveChild)
            {
                result.Width += VScrollSize(result.Height);
                result.Height += HScrollSize(result.Left);

                if (result.Left + result.Width + BorderSize() > monitorRect.Right - 2)
                    result.Width = monitorRect.Right - result.Left - BorderSize() - 2;

                if (result.Top + result.Height + OuterHeight() > monitorRect.Bottom - 2)
                    result.Height = monitorRect.Bottom - result.Top - OuterHeight() + BorderSize() - 2;

                if (!EditorFontChanged && (result.Width < this.ClientSize.Width))
                    result.Width = this.ClientSize.Width;

                if (!EditorFontChanged && (result.Height < this.ClientSize.Height))
                    result.Height = this.ClientSize.Height;
            }

            return result;
        }

        public void SetWindowSize(Rectangle newSize)
        {
            bool flag = ResizeActionBlocked;
            WINDOWPLACEMENT windowPlacement = new WINDOWPLACEMENT();
            ResizeActionBlocked = true;
            ResetConstraints(false);

            if (this.Left != newSize.Left || this.Top != newSize.Top)
            {
                this.Left = newSize.Left;
                this.Top = newSize.Top;
            }

            if (SizeFixed)
            {
                WidthFix = newSize.Width + DoubleBorderSize();
                HeightFix = newSize.Height + OuterHeight();
            }

            if (this.ClientSize.Width != newSize.Width || this.ClientSize.Height != newSize.Height)
            {
                if (this.MdiChildren.Length == 1)
                {
                    SetWidth(newSize.Width, true);
                    this.ClientSize = new Size(this.ClientSize.Width, newSize.Height);
                }
                else
                    this.ClientSize = new Size(newSize.Width, newSize.Height);
            }

            if (WindowSnap)
            {
                windowPlacement.Length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
                Win32.GetWindowPlacement(this.Handle, ref windowPlacement);
                RECT rect = new RECT();
                rect.Left = this.Bounds.Left;
                rect.Top = this.Bounds.Top;
                rect.Right = this.Bounds.Right;
                rect.Bottom = this.Bounds.Bottom;
                windowPlacement.NormalPosition = rect;
                Win32.SetWindowPlacement(this.Handle, ref windowPlacement);
            }

            StatusBar.Items[0].Width = this.ClientSize.Width - StatusBar.Items[1].Width - StatusBar.Items[2].Width - 8;
            ResizeActionBlocked = flag;
        }

        public void AddWindowListItem(ChildForm childForm)
        {
            Array.Resize(ref ChildsTable, ChildsTable.Length + 1);
            ChildsTable[ChildsTable.Length - 1] = childForm;

            Debug.WriteLine($"AddWindowListItem ChildsTable.Length: {ChildsTable.Length}");
        }

        public void DeleteWindowListItem(ChildForm childForm)
        {
            int i, j, k;

            // Remove child and its TurboSound windows from ChildsTable
            for (i = 0; i < ChildsTable.Length; i++)
            {
                if (ChildsTable[i] == childForm)
                {
                    // Remove TurboTrack window 0
                    for (j = 0; j < ChildsTable.Length; j++)
                    {
                        if (ChildsTable[j] == childForm.TSWindow[0])
                        {
                            ChildsTable[j] = null;
                            break;
                        }
                    }

                    // Remove TurboTrack window 1
                    for (j = 0; j < ChildsTable.Length; j++)
                    {
                        if (ChildsTable[j] == childForm.TSWindow[1])
                        {
                            ChildsTable[j] = null;
                            break;
                        }
                    }

                    ChildsTable[i] = null;
                    break;
                }
            }

            // Compact the array to remove nulls
            k = ChildsTable.Length;
            i = 0;
            j = 0;

            while (i < k)
            {
                // Find next non-null entry
                while (j < k && ChildsTable[j] == null)
                    j++;

                // Shift if needed
                if (j < k && j > i)
                    ChildsTable[i] = ChildsTable[j];

                i++;
                j++;
            }

            // Resize array to remove trailing nulls
            Array.Resize(ref ChildsTable, ChildsTable.Length - (j - i));
        }

        public void AutoMetricsForChilds(FormWindowState mainWindowState)
        {
            int childCount = this.MdiChildren.Length;

            if (childCount == 0)
                return;

            ChildForm childForm = null;
            bool prevEventsFlag = ChildsEventsBlocked;
            ChildsEventsBlocked = true;
            Rectangle monitorRect = Screen.FromControl(this).WorkingArea;

            // Just one child window - forced maximization & no sizeable
            if (childCount == 1)
            {
                childForm = (ChildForm)this.MdiChildren[0];

                if (childForm.IsClosed)
                    return;

                //activeForm.Constraints.MaxWidth = 0;
                //activeForm.Constraints.MinWidth = 0;
                childForm.MinimumSize = new Size(0, childForm.MinimumSize.Height);
                childForm.MaximumSize = new Size(0, childForm.MaximumSize.Height);

                childForm.WindowState = FormWindowState.Maximized;
                //activeForm.FormBorderStyle = FormBorderStyle.FixedSingle;
                SetSizable(childForm, false);

                // If num childs > 1, then set FormWindowState.Normal state & sizeable
            }
            else
            {
                for (int i = 0; i < childCount; i++)
                {
                    childForm = (ChildForm)this.MdiChildren[i];

                    if (childForm.IsClosed)
                        continue;

                    childForm.WindowState = FormWindowState.Normal;
                    //activeForm.FormBorderStyle = FormBorderStyle.Sizable;
                    SetSizable(childForm, true);
                }
            }

            // Just one child
            if (childCount == 1)
            {
                // Just one child & main window in maximized state:
                if (mainWindowState == FormWindowState.Maximized)
                {
                    childForm = (ChildForm)this.MdiChildren[0];

                    if (childForm.IsClosed)
                        return;

                    childForm.SuspendLayout();

                    childForm.TabControl.Top = 10;
                    childForm.TabControl.Left = (this.Width / 2) - (childForm.TabControl.Width / 2);
                    childForm.TabControl.Height = WorkAreaHeight(this.ClientSize.Height) - childForm.TabControl.Top - 7;

                    childForm.TopBackgroundPanel.Left = childForm.TabControl.Left;
                    childForm.TopBackgroundPanel.Width = childForm.TabControl.Width;
                    childForm.TopBackgroundPanel.Visible = true;

                    childForm.HeightChanged = childForm.LastHeight != childForm.TabControl.Height;
                    childForm.LastHeight = childForm.TabControl.Height;
                    childForm.ResumeLayout();
                }
                else
                {
                    // Just one child & window state = normal
                    childForm = (ChildForm)this.MdiChildren[0];

                    if (childForm.IsClosed)
                        return;

                    childForm.SuspendLayout();
                    childForm.TabControl.Left = 0;
                    childForm.TabControl.Top = 10;
                    childForm.TabControl.Height = WorkAreaHeight(this.ClientSize.Height) - childForm.TabControl.Top + 5;
                    childForm.HeightChanged = childForm.LastHeight != childForm.TabControl.Height;
                    childForm.LastHeight = childForm.TabControl.Height;
                    childForm.TopBackgroundPanel.Left = 0;
                    childForm.TopBackgroundPanel.Width = childForm.TabControl.Width;
                    childForm.TopBackgroundPanel.Visible = true;
                    childForm.ResumeLayout();
                }

                childForm.MuteButton.Left = childForm.TabControl.Left + childForm.TabControl.Width - childForm.MuteButton.Width * 2 - 16;
                childForm.SoloButton.Left = childForm.MuteButton.Left + childForm.MuteButton.Width + 2;
            }
            else
            {
                // Childs count > 1
                for (int i = 0; i < this.MdiChildren.Length; i++)
                {
                    childForm = (ChildForm)this.MdiChildren[i];

                    if (childForm.IsClosed)
                        continue;

                    ChildsEventsBlocked = true;

                    childForm.SuspendLayout();
                    childForm.TabControl.Left = 0;
                    childForm.TabControl.Top = 0;

                    childForm.MuteButton.Left = childForm.TabControl.Left + childForm.TabControl.Width - childForm.MuteButton.Width * 2 - 16;
                    childForm.SoloButton.Left = childForm.MuteButton.Left + childForm.MuteButton.Width + 2;

                    childForm.TopBackgroundPanel.Visible = false;
                    childForm.SetWidth(childForm.TabControl.Width, true);

                    if (MaximizeChilds)
                        childForm.Height = WorkAreaHeight(this.ClientSize.Height) - childForm.Top;

                    childForm.TabControl.Height = childForm.ClientSize.Height;
                    childForm.HeightChanged = childForm.LastHeight != childForm.TabControl.Height;
                    childForm.LastHeight = childForm.TabControl.Height;
                    childForm.ResumeLayout();
                    ChildsEventsBlocked = false;
                }
            }

            childForm = (ChildForm)this.MdiChildren[0];
            LastChildWidth = childForm.TabControl.Width;
            LastChildHeight = ChildsMaxHeight();

            ChildsEventsBlocked = prevEventsFlag;
        }

        public void AutoCutChilds(Rectangle newSize)
        {
            if (this.MdiChildren.Length < 2)
                return;

            if (!HScrollVisible(newSize.Left))
                return;

            if (ChildsWidth() <= newSize.Width)
                return;

            // Fix childs extra height
            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[i];

                if (childForm.Top + childForm.Height > WorkAreaHeight(newSize.Height) - HScrollbarSize)
                {
                    childForm.Height = WorkAreaHeight(newSize.Height) - HScrollbarSize - childForm.Top;
                    childForm.TabControl.Height = childForm.ClientSize.Height;
                    childForm.HeightChanged = true;

                    if (EditorFontChanged)
                        childForm.AutoResizeForm();
                }
            }
        }

        public void AutoToolBarPosition(Rectangle newSize)
        {
            const int MaxTrackBarWidth = 250;
            int toolBarWidth;
            int trackBarWidth;

            // Volume trackbar width
            if (this.MdiChildren.Length < 2 && this.WindowState != FormWindowState.Maximized)
            {
                //ToolStrip1.Indent = 0;
                trackBarWidth = newSize.Width - VolumeTrackBar.Left;

                if (trackBarWidth > MaxTrackBarWidth)
                    trackBarWidth = MaxTrackBarWidth;

                VolumeTrackBar.Width = trackBarWidth;
                return;
            }

            if (this.MdiChildren.Length == 1)
            {
                ChildForm activeForm = (ChildForm)this.MdiChildren[0];
                //ToolStrip1.Indent = c.PageControl1.Left - 2;
                trackBarWidth = activeForm.TabControl.Left + activeForm.TabControl.Width - VolumeTrackBar.Left + 3;

                if (trackBarWidth > MaxTrackBarWidth)
                    trackBarWidth = MaxTrackBarWidth;

                VolumeTrackBar.Width = trackBarWidth;
                return;
            }

            // Set toolbar to main window center
            VolumeTrackBar.Width = MaxTrackBarWidth;
            toolBarWidth = VolumeTrackBar.Left + VolumeTrackBar.Width; // - ToolStrip1.Indent;
            //ToolStrip1.Indent = (NewSize.Width / 2) - (ToolBarWidth / 2);
        }

        public void SetChildsPosition(FormWindowState mainWindowState)
        {
            if (this.MdiChildren.Length < 2)
                return;

            int prevChildRightCorner = 0;
            bool prevEventsFlag = ChildsEventsBlocked;
            ChildsEventsBlocked = true;

            if (mainWindowState == FormWindowState.Maximized && ChildsWidth() < this.ClientSize.Width)
                prevChildRightCorner = (this.ClientSize.Width / 2) - (ChildsWidth() / 2);

            for (int i = 0; i < ChildsTable.Length; i++)
            {
                if (ChildsTable[i] == null)
                    continue;

                if (ChildsTable[i].IsClosed)
                    continue;

                ChildsTable[i].Top = 0;
                ChildsTable[i].Left = prevChildRightCorner;
                prevChildRightCorner = prevChildRightCorner + ChildsTable[i].Width;
            }

            ChildsEventsBlocked = prevEventsFlag;
        }

        public void CreateChildWrapper(string fileName)
        {
            if (fileName != "" && !System.IO.File.Exists(fileName))
            {
                MessageBox.Show(this, $"File Not Found: \"{fileName}\"", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RedrawOff();
            CreateMDIChild(fileName, 1);
            RedrawOn();
            Activate();
        }

        // Private declarations
        private void CloseTemplateModule()
        {
            if (ChildsTable.Length == 0)
                return;

            if (ChildsTable[0] == null)
                return;

            if (!ChildsTable[0].IsTemplate)
                return;

            ChildForm activeForm = ChildsTable[0];

            // Don't kill window if SongChanged OR playing right now
            activeForm.IsTemplate = !(activeForm.SongChanged || activeForm.Tracks.IsTrackPlaying());

            if (activeForm.IsTemplate)
            {
                // Otherwise... say bye-bye to window
                if (activeForm.TSWindow[1] != null)
                {
                    DrawOffAfterClose = true;
                    activeForm.TSWindow[1].Dispose();
                    ChildsTable[2] = null;
                }

                if (activeForm.TSWindow[0] != null)
                {
                    DrawOffAfterClose = true;
                    activeForm.TSWindow[0].Dispose();
                    ChildsTable[1] = null;
                }

                DrawOffAfterClose = true;
                activeForm.Dispose();
                ChildsTable[0] = null;
                RedrawOff();
            }
        }

        private void CreateMDIChild(string fileName, int turboSoundCount)
        {
            ChildForm childForm = null;
            LibVT.VTM vtm2 = null;
            LibVT.VTM vtm3 = null;
            int initLeft = 0, numb = 0;
            Rectangle newSize;

            // --- Check if module is already opened

            // Create list of opened files
            var openedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                if (this.MdiChildren[i] is ChildForm child)
                {
                    if (!string.IsNullOrEmpty(child.WinFileName))
                        openedFiles.Add(child.WinFileName);
                }
            }

            // Check if file already opened
            if (openedFiles.Contains(Name))
            {
                MessageBox.Show($"Module \"{Path.GetFileName(Name)}\" is Already Opened.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Create MDIChild
            WinCount++;
            ChildsEventsBlocked = true;
            ResizeActionBlocked = true;
            vtm2 = null;
            vtm3 = null;

            if (this.MdiChildren.Length == 1 && turboSoundCount == 1)
            {
                childForm = (ChildForm)this.MdiChildren[0];

                childForm.WindowState = FormWindowState.Normal;
                //activeForm.FormBorderStyle = FormBorderStyle.Sizable;
                SetSizable(childForm, true);

                childForm.TabControl.Left = 0;
                childForm.TabControl.Top = 0;

                childForm.Height = WorkAreaHeight(this.ClientSize.Height);
                childForm.TabControl.Height = childForm.ClientSize.Height;
                childForm.ClientSize = new Size(childForm.TabControl.Width, childForm.ClientSize.Height);
                LastChildHeight = childForm.Height;

                ResetConstraints(false);
            }

            // Close unchanged & unplaying template song
            CloseTemplateModule();

            for (int i = 0; i < 3; i++)
            {
                // Create a new child
                childForm = new ChildForm(this);

                if (i == 0)
                    initLeft = childForm.Left;
                else
                    childForm.Left = initLeft + i * 20;

                if (this.WindowState == FormWindowState.Normal && LastChildHeight != 0 && this.MdiChildren.Length > 1)
                {
                    childForm.Height = LastChildHeight;
                    childForm.TabControl.Height = childForm.ClientSize.Height;
                }
                else if (this.WindowState == FormWindowState.Normal && this.MdiChildren.Length == 1)
                {
                    childForm.Height = WorkAreaHeight(this.ClientSize.Height); // + 5;
                    childForm.TabControl.Height = childForm.ClientSize.Height;
                    LastChildHeight = childForm.Height;
                }

                // Some initial shit
                childForm.WinNumber = this.MdiChildren.Length;
                childForm.Text = $"New Module {WinCount}";
                childForm.EnvelopeAsNoteCheckBox.Checked = EnvelopeAsNote;
                DrawOffAfterClose = false;
                AddWindowListItem(childForm);

                childForm.Show();
                childForm.Top = 0;

                // Load module
                bool ok = true;

                if (fileName == "")
                {
                    childForm.VTM.Positions.Length = 1;
                    childForm.ValidatePattern2(0);
                }

                if (fileName != "")
                {
                    if (System.IO.File.Exists(fileName))
                        ok = childForm.LoadTrackerModule(fileName, i, ref numb, ref vtm2, ref vtm3);
                }
                else
                    numb = turboSoundCount;

                // Shit happens
                if (!ok)
                {
                    DeleteWindowListItem(childForm);
                    childForm.Close();
                    WinCount--;
                    ChildsEventsBlocked = false;
                    ResizeActionBlocked = false;
                    return;
                }

                if (i + 1 >= numb)
                    break;
            }

            if (fileName == "")
            {
                childForm.VTM.Positions.Length = 1;

                if (childForm.TSWindow[0] != null)
                    childForm.TSWindow[0].VTM.Positions.Length = 1;

                if (childForm.TSWindow[1] != null)
                    childForm.TSWindow[1].VTM.Positions.Length = 1;
            }

            if (vtm2 != null || vtm3 != null || turboSoundCount > 1)
            {
                childForm.NumModule = 1;
                childForm.InitTrack();
                childForm.TSWindow[0] = ChildsTable[ChildsTable.Length - 2];
                childForm.TSWindow[0].TSWindow[0] = childForm;
                childForm.TSWindow[0].TSWindow[1] = null;
                childForm.TSWindow[0].InitTrack();
                childForm.TSWindow[0].NumModule = 2;

                if (vtm3 != null || turboSoundCount == 3)
                {
                    childForm.TSWindow[1] = ChildsTable[ChildsTable.Length - 3];
                    childForm.TSWindow[1].TSWindow[0] = childForm;
                    childForm.TSWindow[1].TSWindow[1] = childForm.TSWindow[0];
                    childForm.TSWindow[0].TSWindow[1] = childForm.TSWindow[1];
                    childForm.TSWindow[1].InitTrack();
                    childForm.TSWindow[1].NumModule = 3;
                }
            }
            else
            {
                childForm.InitTrack();
            }

            // Accept to show new child
            AutoMetricsForChilds(this.WindowState);
            SetChildsPosition(this.WindowState);
            newSize = GetSizeForChilds(this.WindowState, false);
            AutoToolBarPosition(newSize);
            AutoCutChilds(newSize);
            RedrawChilds();
            SetWindowSize(newSize);

            if (HScrollVisible(this.Left))
                PostMessage(ClientHandle(), WM_HSCROLL, SB_RIGHT, 0);

            if (this.MdiChildren.Length > 0)
            {
                childForm.InitFinished = true;

                if (childForm.TSWindow[0] != null)
                    childForm.TSWindow[0].InitFinished = true;

                if (childForm.TSWindow[1] != null)
                    childForm.TSWindow[1].InitFinished = true;

                AY.ActiveModule = childForm.VTM;
            }

            ChildsEventsBlocked = false;
            ResizeActionBlocked = false;

            JoinTracks_Update(this, EventArgs.Empty);
        }

        public void FileNew_Execute(object sender, EventArgs e)
        {
            if (WaveOutAPI.ExportStarted)
                return;

            if (ChildsTable.Length > 0 && ChildsTable[0] != null && ChildsTable[0].IsTemplate)
                ChildsTable[0].IsTemplate = false;

            CreateChildWrapper("");
        }

        public void FileOpen_Execute(object sender, EventArgs e)
        {
            if (WaveOutAPI.ExportStarted)
                return;

            OpenDialog.Multiselect = true;

            if (RecentFiles[0] != "")
            {
                string openPath = Path.GetDirectoryName(RecentFiles[0]);

                if (Directory.Exists(openPath))
                    OpenDialog.InitialDirectory = openPath;
            }

            if (OpenDialog.ShowDialog() == DialogResult.OK)
            {
                RedrawOff();
                ChildsEventsBlocked = true;
                int i = Math.Min(OpenDialog.FileNames.Length - 1, 16);

                for (; i >= 0; i--)
                    CreateMDIChild(OpenDialog.FileNames[i], 1);

                ChildsEventsBlocked = false;
                RedrawOn();
            }
        }

        public void FileClose_Execute(object sender, EventArgs e)
        {
            if (WaveOutAPI.ExportStarted)
                return;

            if (this.MdiChildren.Length == 0)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.Close();
        }

        public void HelpAbout_Execute(object sender, EventArgs e)
        {
            using (AboutForm aboutForm = new AboutForm())
                aboutForm.ShowDialog(this);
        }

        public void FileExit_Execute(object sender, EventArgs e)
        {
            this.Close();
        }

        private static bool TryGetBuildDateTime()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using Stream? stream = assembly.GetManifestResourceStream("VortexTracker.Resources.BuildDate.txt");

            if (stream == null)
                return false;

            using StreamReader streamReader = new StreamReader(stream);
            string? line = streamReader.ReadLine();

            if (string.IsNullOrWhiteSpace(line))
                return false;

            return DateTime.TryParse(line, null, DateTimeStyles.AdjustToUniversal, out BuildDateTime);
        }

        public static bool TryGetAppInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var appName = assembly.GetName().Name;
            var version = assembly.GetName().Version;

            VersionString = version.ToString(3);
            FullVersString = $"{Application.ProductName} {VersionString}";
            HalfVersString = $"Version {VersionString}";
            VortexDirName = $"{Application.ProductName} {version.ToString(1)}";

            return true;
        }

        private static bool TryGetEmbeddedResourceTimestamp(string manifestName, out DateTime timeStamp)
        {
            timeStamp = DateTime.MinValue;
            
            var assembly = Assembly.GetExecutingAssembly();
            
            using Stream? stream = assembly.GetManifestResourceStream(manifestName);

            if (stream == null)
                return false;

            using var streamReader = new StreamReader(stream);
            var line = streamReader.ReadLine();
            
            if (string.IsNullOrWhiteSpace(line))
                return false;

            return DateTime.TryParseExact(
                line,
                "yyyy-MM-dd HH:mm:ss.FFFFFFF",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out timeStamp);
        }

        private void ExtractResourceZips(string category, string outDirectory, bool skipIfUnchanged = true)
        {
            var asm = Assembly.GetExecutingAssembly();
            var ini = GetConfigIniFile();

            // manifest prefix: "VortexTracker.Resources.Plugins."
            string prefix = $"VortexTracker.Resources.{category}.";
            const string zipExt = ".zip", txtExt = ".txt";

            // 1) find all ZIPs for this category
            var zips = asm.GetManifestResourceNames()
                          .Where(r => r.StartsWith(prefix, StringComparison.Ordinal)
                                   && r.EndsWith(zipExt, StringComparison.Ordinal));

            foreach (var zipRes in zips)
            {
                // 2) derive the bundle name: strip prefix & ".zip"
                string bundle = zipRes[prefix.Length..^zipExt.Length];

                // 3) load embedded timestamp resource "<prefix><bundle>.txt"
                string manifestName = prefix + bundle + txtExt;
                if (!TryGetEmbeddedResourceTimestamp(manifestName, out DateTime buildStamp))
                    buildStamp = BuildDateTime;  // fallback

                // 4) check if we should skip extraction
                string iniKey = $"{bundle}Stamp";
                string stored = ini.GetValue("Resources", iniKey, "");
                string buildIso = buildStamp.ToString("O", CultureInfo.InvariantCulture);

                if (skipIfUnchanged
                    && Directory.Exists(outDirectory)
                    && string.Equals(stored, buildIso, StringComparison.Ordinal))
                {
                    continue;
                }

                // 5) (re)create the output folder
                // Don't delete the directory so user files are maintained
                //if (Directory.Exists(outDir))
                //    Directory.Delete(outDir, recursive: true);
                Directory.CreateDirectory(outDirectory);

                // 6) extract ZIP into that sub-folder
                using var zipStream = asm.GetManifestResourceStream(zipRes)
                                  ?? throw new InvalidOperationException($"Missing resource {zipRes}");
                using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
                archive.ExtractToDirectory(outDirectory, overwriteFiles: true);

                // 7) persist the new timestamp
                ini.SetValue("Resources", iniKey, buildIso);
            }

            ini.Save();
        }

        public void UnpackInstruments()
        {
            string instrumentsDir = Path.Combine(VortexDocumentsDir, InstrumentsDefaultDir);
            ExtractResourceZips("Instruments", instrumentsDir);
        }

        public void UnpackDemoSongs()
        {
            string songsDir = Path.Combine(VortexDocumentsDir, DemoSongsDefaultDir);

            if (!Directory.Exists(songsDir))
                Directory.CreateDirectory(songsDir);

            ExtractResourceZips("DemoSongs", songsDir);

            OpenDemoSong.DropDownItems.Clear();

            var files = Directory.GetFiles(songsDir, "*.vt2", SearchOption.AllDirectories)
                                 .OrderBy(f => f, StringComparer.OrdinalIgnoreCase);

            Regex regex = new Regex(@"^([0-9]+)_.*$", RegexOptions.IgnoreCase);
            int prevYear = 0;

            foreach (string filePath in files)
            {
                string name = Path.GetFileName(filePath);
                Match match = regex.Match(name);
                int year = match.Success ? int.Parse(match.Groups[1].Value) : 0;

                if (prevYear != 0 && year != prevYear)
                    OpenDemoSong.DropDownItems.Add(new ToolStripSeparator());

                var item = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(name))
                {
                    Tag = filePath
                };
                item.Click += OpenDemo_Click;

                OpenDemoSong.DropDownItems.Add(item);
                prevYear = year;
            }
        }

        public void UnpackFonts()
        {
            string fontsDir = Path.Combine(VortexDocumentsDir, FontsDefaultDir);
            ExtractResourceZips("Fonts", fontsDir);
        }

        public void UnpackPlugins()
        {
            string pluginsDir = Path.Combine(VortexDocumentsDir, PluginsDefaultDir);
            ExtractResourceZips("Plugins", pluginsDir);
        }

        public void OpenDemo_Click(object sender, EventArgs e)
        {
            string filePath;

            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
                DontAddToRecent = true;
                filePath = (string)menuItem.Tag;
                CreateChildWrapper(filePath);
                DontAddToRecent = false;
            }
        }

        public void AppException(object sender, ThreadExceptionEventArgs e)
        {
            if (WaveOutAPI.IsPlaying)
                WaveOutAPI.StopPlaying();

            for (int i = 0; i < this.MdiChildren.Length; i++)
                this.MdiChildren[i].Dispose();

            WaveOutAPI.Shutdown();
            ColorThemes.RestoreSystemColors();
            SaveBackups(this, EventArgs.Empty);
            MessageBox.Show(this, "Application Error Occured. Backup File Saved.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            Application.Exit();
        }

        public void MainForm_Load(object sender, EventArgs e)
        {
            int i;
            //Bitmap bitmap;
#if LOGGER
            Logger = new Logger("out.txt");
#endif

            TryGetBuildDateTime();
            TryGetAppInfo();

            MaximizeChilds = true;
            RedrawEnabled = true;
            PrevTop = 0;

            this.Text = FullVersString;

            // Save system colors for window decoration
            ColorThemes.SaveSystemColors();

            // Set VortexDir variable
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (String.IsNullOrEmpty(appData))
            {
                VortexDir = Path.Combine("C:\\", VortexDirName);
                VortexDocumentsDir = VortexDir;
            }
            else
            {
                VortexDir = Path.Combine(appData, VortexDirName);
                VortexDocumentsDir = Path.Combine(GetUserDocumentsDir(), VortexDirName);
            }

            ConfigFilePath = Path.Combine(VortexDir, "Config.ini");
            PluginsPath = Path.Combine(VortexDir, "Plugins");

            //Win32.DragAcceptFiles(this.Handle, true); // TODO: Change to built in (DragEnter, DragDrop)
            CreateMidiInputClient();
            MidiOut.CreateClient(this);
            MidiOut.InitializeMidiChannel(0);
            MidiOut.InitializeMidiChannel(1);
            MidiOut.InitializeMidiChannel(2);

            // Init syncronization
            SyncMessageFile = Path.Combine(VortexDir, "Sync");

            if (System.IO.File.Exists(SyncMessageFile))
                System.IO.File.Delete(SyncMessageFile);

            SyncCheckTimer.Enabled = true;
            SyncFinishTimer.Enabled = false;
            SyncSampleBufferFile = Path.Combine(VortexDir, "Sample");
            SyncSamplePartFile = Path.Combine(VortexDir, "SamplePart");
            SyncOrnamentBufferFile = Path.Combine(VortexDir, "Ornament");
            // Init FamiTracker clipboard format
            // Unpack instruments & demosongs
            UnpackInstruments();
            UnpackDemoSongs();
            UnpackFonts();
            UnpackPlugins();

            // Load fonts
            LoadAllResourceFonts();

            WinCount = 0;
            LastChildWidth = 0;
            LastChildHeight = 0;
            SetChildAsTemplate = false;
            RecentFiles = new string[6];
            for (i = 0; i < 6; i++)
                RecentFiles[i] = "";
            Helpers.FillChar<sbyte>(NoteKeys, NoteKeys.Length, -3);
            NoteKeys[(int)Keys.A] = -2;
            NoteKeys[(int)Keys.K] = -1;
            NoteKeys[(int)Keys.Z] = 0;
            NoteKeys[(int)Keys.S] = 1;
            NoteKeys[(int)Keys.X] = 2;
            NoteKeys[(int)Keys.D] = 3;
            NoteKeys[(int)Keys.C] = 4;
            NoteKeys[(int)Keys.V] = 5;
            NoteKeys[(int)Keys.G] = 6;
            NoteKeys[(int)Keys.B] = 7;
            NoteKeys[(int)Keys.H] = 8;
            NoteKeys[(int)Keys.N] = 9;
            NoteKeys[(int)Keys.J] = 10;
            NoteKeys[(int)Keys.M] = 11;
            NoteKeys[(int)Keys.Oemcomma] = 12;
            NoteKeys[(int)Keys.L] = 13;
            NoteKeys[(int)Keys.OemPeriod] = 14;
            NoteKeys[(int)Keys.OemSemicolon] = 15;
            NoteKeys[(int)Keys.OemQuestion] = 16;
            NoteKeys[(int)Keys.Q] = 12;
            NoteKeys[(int)Keys.D2] = 13;
            NoteKeys[(int)Keys.W] = 14;
            NoteKeys[(int)Keys.D3] = 15;
            NoteKeys[(int)Keys.E] = 16;
            NoteKeys[(int)Keys.R] = 17;
            NoteKeys[(int)Keys.D5] = 18;
            NoteKeys[(int)Keys.T] = 19;
            NoteKeys[(int)Keys.D6] = 20;
            NoteKeys[(int)Keys.Y] = 21;
            NoteKeys[(int)Keys.D7] = 22;
            NoteKeys[(int)Keys.U] = 23;
            NoteKeys[(int)Keys.I] = 24;
            NoteKeys[(int)Keys.D9] = 25;
            NoteKeys[(int)Keys.O] = 26;
            NoteKeys[(int)Keys.D0] = 27;
            NoteKeys[(int)Keys.P] = 28;
            NoteKeys[(int)Keys.OemOpenBrackets] = 29;
            NoteKeys[(int)Keys.Oemplus] = 30;
            NoteKeys[(int)Keys.OemCloseBrackets] = 31;
            NoteKeys[(int)Keys.NumPad1] = 33;
            NoteKeys[(int)Keys.NumPad2] = 34;
            NoteKeys[(int)Keys.NumPad3] = 35;
            NoteKeys[(int)Keys.NumPad4] = 36;
            NoteKeys[(int)Keys.NumPad5] = 37;
            NoteKeys[(int)Keys.NumPad6] = 38;
            NoteKeys[(int)Keys.NumPad7] = 39;
            NoteKeys[(int)Keys.NumPad8] = 40;
            ChanAlloc = new int[3];
            ChanAlloc[0] = 0;
            ChanAlloc[1] = 1;
            ChanAlloc[2] = 2;
            MainForm.ChanAllocIndex = 0;
            this.Enabled = true;
            //FileMode = 0;
            OpenDialog.InitialDirectory = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            if (MainForm.TryGetFontFamily("Consolas", out FontFamily fontFamily))
            {
                EditorFont = new Font(fontFamily, 15, FontStyle.Bold);
                TestLineFont = new Font(fontFamily, 15, FontStyle.Bold);
            }
            else
            {
                MessageBox.Show(this, "Error Loading Consolas Font", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //EditorFont = new Font("Courier New", 15, FontStyle.Bold);
                //TestLineFont = new Font("Courier New", 15, FontStyle.Bold);
            }
            AY.LoopAllowed = false;
            Main.LoopAllAllowed = false;
            Main.GlobalVolume = VolumeTrackBar.Value;
            Main.GlobalVolumeMax = VolumeTrackBar.Maximum;
            AY.SetDefault(AY.SampleRateDefault, AY.NumberOfChannelsDefault, AY.SampleBitDefault);
            // Moved to WaveOutAPI
            //WaveOutAPI.ResetMutex = Win32.CreateMutex(IntPtr.Zero, false, ("VTII_Reset" + Process.GetCurrentProcess().Id.ToString()));
            AY.Synthesizer = AY.Synthesizer_Stereo16;
            WaveOutAPI.ExportStarted = false;
            DrawOffAfterClose = false;
            VScrollbarSize = SystemInformation.VerticalScrollBarWidth;
            HScrollbarSize = SystemInformation.VerticalScrollBarArrowHeight;
            ReadConfig();
            SetFileAssociations();
            ColorThemes.InitColorThemes();
            Main.InitBuffSample();
            Main.InitBuffOrnament();
            InitBuffTracks();
            LastClipboard = LastClipboard.None;
            ChangeBackupTimer();
            StatusBar.Items[0].Width = this.ClientSize.Width - StatusBar.Items[1].Width - StatusBar.Items[2].Width - 8;
        }

        public void RedrawPlWindow(ChildForm playingWindow, int position, int pattern, int line)
        {
            if (playingWindow == null)
                return;

            if (pattern >= playingWindow.VTM.Patterns.Length)
                return;

            // PW.Tracks.RedrawDisabled := True;
#if LOGGER
            Logger.Add(line.ToString());
            //Logger.Add(String.Format($"Pos: {position}, Pat: {pattern}, Line: {line}"));
#endif

            if (position < 256 && position != playingWindow.PositionIndex)
            {
                playingWindow.IsSynchronizing = true;
                playingWindow.SelectPosition2(position);
                playingWindow.IsSynchronizing = false;
            }

            if (playingWindow.Tracks.ShownPattern != playingWindow.VTM.Patterns[pattern] || playingWindow.Tracks.ShownFrom != line)
            {
                playingWindow.PatternIndex = pattern;

                if (AY.PlayMode != PlayModes.PlayPattern)
                    playingWindow.PatternNumUpDown.Value = pattern;

                playingWindow.Tracks.ShownPattern = playingWindow.VTM.Patterns[pattern];
                playingWindow.Tracks.ShownFrom = line;

                if (playingWindow.Tracks.Enabled)
                    playingWindow.Tracks.HideCaret();

                playingWindow.Tracks.CursorY = playingWindow.Tracks.CenterLineIndex;
                playingWindow.Tracks.RemoveSelection();

                if (playingWindow.Tracks.Enabled)
                    playingWindow.Tracks.ShowCaret();

                playingWindow.IsSynchronizing = true;
                playingWindow.CalculatePos(line);
                playingWindow.IsSynchronizing = false;
            }

            if (!playingWindow.Tracks.RedrawDisabled)
                playingWindow.Tracks.RedrawTracks();
        }

        public void RedrawAllSamOrnBrowsers()
        {
            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[i];
                childForm.SamplesBrowser.Visible = SampleBrowserVisible;
                childForm.SamplesDriveSelect.Visible = SampleBrowserVisible;
                childForm.ShowSampleBrowserButton.Visible = !SampleBrowserVisible;
                childForm.HideSampleBrowserButton.Visible = SampleBrowserVisible;
                childForm.OrnamentsBrowser.Visible = OrnamentsBrowserVisible;
                childForm.OrnamentsDriveSelect.Visible = OrnamentsBrowserVisible;
                childForm.ShowOrnamentBrowserButton.Visible = !OrnamentsBrowserVisible;
                childForm.HideOrnamentBrowserButton.Visible = OrnamentsBrowserVisible;
            }
        }

        public void RedrawChilds()
        {
            if (this.MdiChildren.Length == 0)
                return;

            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[i];

                if (childForm.IsClosed)
                    continue;

                childForm.InitStringGridMetrics();
                childForm.Tracks.RedrawDisabled = true;
                childForm.Tracks.InitMetrics();
                if (EditorFontChanged)
                {
                    childForm.LastWidth--;

                    if (childForm.WindowState != FormWindowState.Maximized)
                    {
                        childForm.Height = WorkAreaHeight(this.ClientSize.Height) - childForm.Top;
                        childForm.TabControl.Height = childForm.ClientSize.Height;
                    }

                    childForm.HeightChanged = true;
                }

                //activeForm.SamplesDriveSelect = new TDriveSelect(this);
                //activeForm.OrnamentsBrowser = new TFileBrowser(this);
                //activeForm.OrnamentsDriveSelect = new TDriveSelect(this);

                childForm.AutoResizeForm();
                childForm.SamplesBrowser.BackColor = ThemeColors[(int)ThemeColor.SamOrnBackground];
                childForm.OrnamentsBrowser.BackColor = ThemeColors[(int)ThemeColor.SamOrnBackground];
                childForm.SamplesDriveSelect.BackColor = ThemeColors[(int)ThemeColor.SamOrnBackground];
                childForm.OrnamentsDriveSelect.BackColor = ThemeColors[(int)ThemeColor.SamOrnBackground];
                childForm.UpdateSpeedBPM();
                childForm.Tracks.RedrawDisabled = false;

                // Fix scrollbar repaint bug
                childForm.RefreshPositionsHScroll();
            }
        }

        public void RedrawTracks(int chip, PlayInfo module)
        {
            if (WaveOutAPI.IsPlaying && AY.PlayMode == PlayModes.PlayLine)
                return;

            if (!WaveOutAPI.IsPlaying || VTModule.UnlimitedDelay)
                return;

            if (ChildForm.PlayingWindow[chip] == null)
                return;

            var childForm = ChildForm.PlayingWindow[chip];
            childForm.Tracks.RedrawDisabled = true;

            // Extract line, pattern, and position from WParam
            int position = module.Position;
            int pattern = module.Pattern;
            int line = module.Line;

            if (line < 0)
                line = 0;

            // Check for changes
            //bool pwChanged = (line != childForm.Tracks.ShownFrom || pattern != childForm.PatternNumber || position != childForm.PositionNumber);

            // We draw even when there's no change to update the vu and spectrum bars
            //if (pwChanged)
            Globals.MainForm.RedrawPlWindow(childForm, position, pattern, line);

            childForm.Tracks.RedrawDisabled = false;

            //if (!pwChanged)
            //    return;

            childForm.Tracks.ManualBitBlt = true;

            if (childForm.TabControl.SelectedIndex == 0)
            {
                childForm.Tracks.RedrawTracks();
                childForm.Tracks.DoBitBlt();
            }

            childForm.Tracks.ManualBitBlt = false;
        }

        public void UmRedrawTracks(uint curVisPos)
        {
            for (int i = 0; i < AY.ChipCount; i++)
            {
                PlayInfo module = WaveOutAPI.PlayGrid[curVisPos].Module[i];

                if (ChildForm.PlayingWindow[i] != null && ChildForm.PlayingWindow[i].Tracks != null)
                    RedrawTracks(i, module);
            }
        }

        public void UmRedrawTracks()
        {
            for (int i = 0; i < AY.ChipCount; i++)
            {
                if (ChildForm.PlayingWindow[i] == null)
                    continue;

                var childForm = ChildForm.PlayingWindow[i];

                //childForm.Tracks.DrawBars();
                AY.UpdateSpec(i);
                childForm.Tracks.RedrawTracks();
            }
        }

        public void Options_Execute(object sender, EventArgs e)
        {
            Globals.OptionsForm.Visible = !Globals.OptionsForm.Visible;
        }

        public ChildForm GetFirstModule(ChildForm childForm)
        {
            int left = childForm.Left;
            int index = 0;

            if (childForm.TSWindow[0] != null)
            {
                if (childForm.TSWindow[0].Left < left)
                {
                    left = childForm.TSWindow[0].Left;
                    index = 1;
                }
            }

            if (childForm.TSWindow[1] != null)
            {
                if (childForm.TSWindow[1].Left < left)
                {
                    left = childForm.TSWindow[1].Left;
                    index = 2;
                }
            }

            if (index == 0)
                return childForm;

            if (index == 1)
                return childForm.TSWindow[0];

            return childForm.TSWindow[1];
        }

        public bool SavePT3(ChildForm activeForm, string fileName, bool asText)
        {
            const string errMsg = "Cannot Compile Module Due to 65536 Size Limit For PT3-Modules. You Can Save it as Text Still.";
            PT3 pt3 = new PT3();
            int moduleSize;
            byte[] data = new byte[65536];
            VTM2PT3 vtm2PT3 = new VTM2PT3();

            if (!Globals.IsFileWritable(fileName))
                return false;

            activeForm = GetFirstModule(activeForm);

            if (!asText)
            {
                if (!vtm2PT3.Initialize(data, pt3, activeForm.VTM, out moduleSize))
                {
                    MessageBox.Show(errMsg, fileName);
                    return false;
                }

                using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                using (BinaryWriter writer = new BinaryWriter(fileStream))
                {
                    writer.Write(data, 0, moduleSize);

                    if (activeForm.TSWindow[0] != null && activeForm.TSWindow[1] != null)
                    {
                        TSData3 tsData3 = new TSData3("PT3!", "PT3!", "02TS", "03TS");
                        tsData3.Size0 = (ushort)moduleSize;

                        if (!vtm2PT3.Initialize(data, pt3, activeForm.TSWindow[0].VTM, out moduleSize))
                        {
                            MessageBox.Show(errMsg, fileName);
                            return false;
                        }

                        writer.Write(data, 0, moduleSize);
                        tsData3.Size1 = (ushort)moduleSize;

                        if (!vtm2PT3.Initialize(data, pt3, activeForm.TSWindow[1].VTM, out moduleSize))
                        {
                            MessageBox.Show(errMsg, fileName);
                            return false;
                        }

                        writer.Write(data, 0, moduleSize);
                        tsData3.Size2 = (ushort)moduleSize;

                        writer.Write(Helpers.CastToArray(tsData3), 0, Marshal.SizeOf(typeof(TSData3)));
                    }
                    else if (activeForm.TSWindow[0] != null)
                    {
                        TSData2 tsData2 = new TSData2("PT3!", "PT3!", "02TS");
                        tsData2.Size1 = (ushort)moduleSize;

                        if (!vtm2PT3.Initialize(data, pt3, activeForm.TSWindow[0].VTM, out moduleSize))
                        {
                            MessageBox.Show(errMsg, fileName);
                            return false;
                        }

                        writer.Write(data, 0, moduleSize);
                        tsData2.Size2 = (ushort)moduleSize;

                        writer.Write(Helpers.CastToArray(tsData2), 0, Marshal.SizeOf(typeof(TSData2)));
                    }
                }
            }
            else
            {
                // Swap left and right module, if need
                //if (activeForm.TSWindow[0] != null && !activeForm.LeftModule)
                //    activeWin = activeForm.TSWindow;

                // Save left module
                VTModule.VTM2TextFile(fileName, activeForm.VTM, false, TracksCursorXLeft);

                // Save right module
                if (activeForm.TSWindow[0] != null)
                    VTModule.VTM2TextFile(fileName, activeForm.TSWindow[0].VTM, true, TracksCursorXLeft);

                // Save middle module
                if (activeForm.TSWindow[1] != null)
                    VTModule.VTM2TextFile(fileName, activeForm.TSWindow[1].VTM, true, TracksCursorXLeft);
            }

            activeForm.SavedAsText = asText;
            activeForm.SongChanged = false;
            activeForm.BackupSongChanged = false;

            if (activeForm.TSWindow[0] != null)
            {
                activeForm.TSWindow[0].SavedAsText = asText;
                activeForm.TSWindow[0].SongChanged = false;
                activeForm.TSWindow[0].BackupSongChanged = false;
                activeForm.TSWindow[0].SetFileName(fileName);
            }

            if (activeForm.TSWindow[1] != null)
            {
                activeForm.TSWindow[1].SavedAsText = asText;
                activeForm.TSWindow[1].SongChanged = false;
                activeForm.TSWindow[1].BackupSongChanged = false;
                activeForm.TSWindow[1].SetFileName(fileName);
            }

            AddFileName(fileName);

            return true;
        }

        public void SavePT3Backup(ChildForm activeForm, string fileName, bool asText)
        {
            const string errMsg = "Cannot Compile Module Due 65536 Size Limit For PT3-Modules. You Can Save It In Text Still.";
            PT3 pt3 = new PT3();
            int moduleSize;
            byte[] data = new byte[65536];
            VTM2PT3 vtm2PT3 = new VTM2PT3();

            if (!Globals.IsFileWritable(fileName))
                return;

            activeForm = GetFirstModule(activeForm);

            if (!asText)
            {
                if (!vtm2PT3.Initialize(data, pt3, activeForm.VTM, out moduleSize))
                {
                    MessageBox.Show(errMsg, fileName);
                    return;
                }

                using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
                using (BinaryWriter writer = new BinaryWriter(fileStream))
                {
                    writer.Write(data, 0, moduleSize);

                    if (activeForm.TSWindow[0] != null && activeForm.TSWindow[1] != null)
                    {
                        TSData3 tsData3 = new TSData3("PT3!", "PT3!", "02TS", "03TS");

                        tsData3.Size0 = (ushort)moduleSize;

                        if (!vtm2PT3.Initialize(data, pt3, activeForm.TSWindow[0].VTM, out moduleSize))
                        {
                            MessageBox.Show(errMsg, fileName);
                            return;
                        }

                        writer.Write(data, 0, moduleSize);
                        tsData3.Size1 = (ushort)moduleSize;

                        if (!vtm2PT3.Initialize(data, pt3, activeForm.TSWindow[1].VTM, out moduleSize))
                        {
                            MessageBox.Show(errMsg, fileName);
                            return;
                        }

                        writer.Write(data, 0, moduleSize);
                        tsData3.Size2 = (ushort)moduleSize;

                        writer.Write(Helpers.CastToArray(tsData3), 0, Marshal.SizeOf(typeof(TSData3)));
                    }
                    else if (activeForm.TSWindow[0] != null)
                    {
                        TSData2 tsData2 = new TSData2("PT3!", "PT3!", "02TS");
                        tsData2.Size1 = (ushort)moduleSize;

                        if (!vtm2PT3.Initialize(data, pt3, activeForm.TSWindow[0].VTM, out moduleSize))
                        {
                            MessageBox.Show(errMsg, fileName);
                            return;
                        }

                        writer.Write(data, 0, moduleSize);
                        tsData2.Size2 = (ushort)moduleSize;

                        writer.Write(Helpers.CastToArray(tsData2), 0, Marshal.SizeOf(typeof(TSData2)));
                    }
                }
            }
            else
            {
                VTModule.VTM2TextFile(fileName, activeForm.VTM, false, TracksCursorXLeft);

                if (activeForm.TSWindow[0] != null)
                    VTModule.VTM2TextFile(fileName, activeForm.TSWindow[0].VTM, true, TracksCursorXLeft);

                if (activeForm.TSWindow[1] != null)
                    VTModule.VTM2TextFile(fileName, activeForm.TSWindow[1].VTM, true, TracksCursorXLeft);
            }

            activeForm.BackupSongChanged = false;

            if (activeForm.TSWindow[0] != null)
                activeForm.TSWindow[0].BackupSongChanged = false;

            if (activeForm.TSWindow[1] != null)
                activeForm.TSWindow[1].BackupSongChanged = false;
        }

        public ChildForm GetMainModule()
        {
            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;

            if (activeForm.TSWindow[0] != null && activeForm.TSWindow[0].NumModule == 1)
                return activeForm.TSWindow[0];
            else if (activeForm.TSWindow[1] != null && activeForm.TSWindow[1].NumModule == 1)
                return activeForm.TSWindow[1];
            else
                return activeForm;
        }

        public void FileSave_Execute(object sender, EventArgs e)
        {
            GetMainModule().SaveModule();
        }

        public void FileSaveAs_Execute(object sender, EventArgs e)
        {
            GetMainModule().SaveModuleAs();
        }

        public void FileSave_Update(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
            {
                UIActionManager.Instance.SetEnabled(UIActionType.FileSave, false);
                return;
            }

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            UIActionManager.Instance.SetEnabled(UIActionType.FileSave,
                !WaveOutAPI.ExportStarted &&
                this.MdiChildren.Length != 0 &&
                (activeForm.SongChanged == true ||
                (activeForm.TSWindow[0] != null &&
                 activeForm.TSWindow[0].SongChanged == true)) ||
                (activeForm.SongChanged == true ||
                (activeForm.TSWindow[1] != null &&
                 activeForm.TSWindow[1].SongChanged == true)));
        }

        public void FileSaveAs_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetEnabled(UIActionType.FileSaveAs, this.MdiChildren.Length != 0 && !WaveOutAPI.ExportStarted);
        }

        public void SaveDialog1TypeChange(object sender, EventArgs e)
        {
            SaveDialog1.DefaultExt = SaveDialog1.FilterIndex == 1 ? "txt" : "pt3";
        }

        public void PlayFrom_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetEnabled(UIActionType.PlayFromStart, this.MdiChildren.Length != 0 && !WaveOutAPI.ExportStarted);
        }

        public void PlayPatternFromStart_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetEnabled(UIActionType.PlayPatternFromStart, this.MdiChildren.Length != 0 && !WaveOutAPI.ExportStarted);
        }

        public void PlayPatternFromLine_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetEnabled(UIActionType.PlayPatternFromLine, this.MdiChildren.Length != 0 && !WaveOutAPI.ExportStarted);
        }

        public void PlayFrom_Execute(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            if (activeForm.VTM.Positions.Length <= 0)
                return;

            if (WaveOutAPI.IsPlaying)
            {
                WaveOutAPI.StopPlaying();
                RestoreControls();
            }

            if (NoPatterns())
                return;

            DisableControls(true);

            AY.PlayMode = PlayModes.PlayModule;
            activeForm.PlayStopState = PlayStopState.Stop;
            activeForm.Tracks.RemoveSelection();
            activeForm.CheckPositionsGridPosition();
            ScrollToPlayingWindow();

            for (int i = 0; i < AY.ChipCount; i++)
            {
                if (ChildForm.PlayingWindow[i] == null)
                    continue;

                AY.PlayingModule[i] = ChildForm.PlayingWindow[i].VTM;
                VTModule.Module_SetPointer(ChildForm.PlayingWindow[i].VTM, i);
                VTModule.Module_SetDelay((sbyte)ChildForm.PlayingWindow[i].VTM.InitialDelay);
                VTModule.Module_SetCurrentPosition(0);
            }

            WaveOutAPI.InitForAllTypes(true);
            WaveOutAPI.StartWOThread();
            PlayStop_Update(null, EventArgs.Empty);
        }

        public void PlayPatternFromStart_Execute(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            if (WaveOutAPI.IsPlaying)
            {
                WaveOutAPI.StopPlaying();
                RestoreControls();
            }

            if (NoPatterns())
                return;

            activeForm.PlayStopState = PlayStopState.Stop;
            AY.PlayMode = PlayModes.PlayPattern;

            DisableControls(true);

            ChildForm.PlayingWindow[0].ValidatePattern2(ChildForm.PlayingWindow[0].PatternIndex);
            ChildForm.PlayingWindow[0].Tracks.RemoveSelection();
            ChildForm.PlayingWindow[0].CheckPositionsGridPosition();
            ScrollToPlayingWindow();

            for (int i = 0; i < AY.ChipCount; i++)
            {
                if (ChildForm.PlayingWindow[i] == null)
                    continue;

                AY.PlayingModule[i] = ChildForm.PlayingWindow[i].VTM;
                VTModule.Module_SetPointer(ChildForm.PlayingWindow[i].VTM, i);
                VTModule.Module_SetDelay((sbyte)ChildForm.PlayingWindow[i].VTM.InitialDelay);
                VTModule.Module_SetCurrentPosition(ChildForm.PlayingWindow[i].PositionIndex);
                VTModule.Module_SetCurrentPattern(ChildForm.PlayingWindow[i].PatternIndex);
            }

            WaveOutAPI.InitForAllTypes(false);
            WaveOutAPI.StartWOThread();
            PlayStop_Update(null, EventArgs.Empty);
        }

        public void PlayPatternFromLine_Execute(object sender, EventArgs e)
        {
            // Current line already playing
            if (WaveOutAPI.IsPlaying && (AY.PlayMode == PlayModes.PlayPattern || AY.PlayMode == PlayModes.PlayModule))
                return;

            if (this.MdiChildren.Length == 0)
                return;

            if (WaveOutAPI.IsPlaying)
            {
                WaveOutAPI.StopPlaying();
                RestoreControls();
            }

            if (NoPatterns())
                return;

            DisableControls(true);

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.PlayStopState = PlayStopState.Stop;
            activeForm.ValidatePattern2(activeForm.PatternIndex);
            activeForm.Tracks.RemoveSelection();
            ScrollToPlayingWindow();

            if (activeForm.TSWindow[0] == null)
                activeForm.RestartPlaying(true, false);
            else
                activeForm.RestartPlayingTS(true, false);

            activeForm.CheckPositionsGridPosition();
            PlayStop_Update(null, EventArgs.Empty);
        }

        private void RestoreTracksFocus()
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            for (int i = 0; i < AY.ChipCount; i++)
            {
                if (this.ActiveMdiChild == ChildForm.PlayingWindow[i] && ChildForm.PlayingWindow[i].TabControl.SelectedIndex == 0)
                    ChildForm.PlayingWindow[i].Tracks.Focus();
            }
        }

        public void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            VTExit = true;
            WaveOutAPI.StopPlaying();
            WriteConfig();
            BeginInvoke(() => ColorThemes.RestoreSystemColors());

            try
            {
                if (System.IO.File.Exists(SyncSampleBufferFile))
                    System.IO.File.Delete(SyncSampleBufferFile);

                if (System.IO.File.Exists(SyncOrnamentBufferFile))
                    System.IO.File.Delete(SyncOrnamentBufferFile);

                if (System.IO.File.Exists(SyncSamplePartFile))
                    System.IO.File.Delete(SyncSamplePartFile);
            }
            catch
            {
            }
        }

        public void SetLoopPosition_Execute(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            if (activeForm.PositionsGrid.Selection.Left < activeForm.VTM.Positions.Length && activeForm.PositionsGrid.Selection.Left != activeForm.VTM.Positions.Loop)
                activeForm.SetLoopPos(activeForm.PositionsGrid.Selection.Left);

            activeForm.InputPNumber = 0;
        }

        public void SetLoopPosition_Update(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            UIActionManager.Instance.SetChecked(UIActionType.SetLoopPosition, this.MdiChildren.Length != 0 && activeForm.PositionsGrid.Focused && (activeForm.VTM.Positions.Length > activeForm.PositionsGrid.Selection.Left));
        }

        public void InsertPositions_Execute(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
                return;

            if (WaveOutAPI.IsPlaying && AY.PlayMode == PlayModes.PlayModule)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.InsertPosition(false, true, true);
            // Duplicate - false

        }

        public void DuplicatePositions_Execute(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
                return;

            if (WaveOutAPI.IsPlaying && AY.PlayMode == PlayModes.PlayModule)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.InsertPosition(true, true, true);
            // Duplicate - true
        }

        public void ClonePositions_Execute(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
                return;

            if (WaveOutAPI.IsPlaying && AY.PlayMode == PlayModes.PlayModule)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.ClonePositions();
        }

        public void DeletePositions_Execute(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
                return;

            if (WaveOutAPI.IsPlaying && AY.PlayMode == PlayModes.PlayModule)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.DeletePositions();
        }

        public void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            DragAcceptFiles(this.Handle, false);
            //CloseHandle(WaveOutAPI.ResetMutex);
            WaveOutAPI.ResetMutex.Dispose();
            EditorFont.Dispose();
            TestLineFont.Dispose();
        }

        public void ToggleLooping_Execute(object sender, EventArgs e)
        {
            AY.LoopAllowed = !AY.LoopAllowed;

            if (AY.LoopAllowed)
            {
                Main.LoopAllAllowed = false;
                UIActionManager.Instance.SetChecked(UIActionType.ToggleLoopingAll, false);
            }

            UIActionManager.Instance.SetChecked(UIActionType.ToggleLooping, AY.LoopAllowed);
        }

        public void ToggleLoopingAll_Execute(object sender, EventArgs e)
        {
            Main.LoopAllAllowed = !Main.LoopAllAllowed;

            if (Main.LoopAllAllowed)
            {
                AY.LoopAllowed = false;
                UIActionManager.Instance.SetChecked(UIActionType.ToggleLooping, false);
            }

            UIActionManager.Instance.SetChecked(UIActionType.ToggleLoopingAll, Main.LoopAllAllowed);
        }

        public void AddFileName(string fileName)
        {
            int i, j;

            if (DontAddToRecent)
                return;

            string fileName1 = fileName.ToUpper();

            for (i = 0; i < 5; i++)
            {
                if (RecentFiles[i].ToUpper() == fileName1)
                {
                    for (j = i; j < 5; j++)
                        RecentFiles[j] = RecentFiles[j + 1];

                    break;
                }
            }
            for (i = 4; i >= 0; i--)
                RecentFiles[i + 1] = RecentFiles[i];

            RecentFiles[0] = fileName;
            j = ((ToolStripMenuItem)MainMenu1.Items[0]).DropDownItems.IndexOf(RFile1);

            if (j == -1)
                return;

            for (i = 0; i < 6; i++)
            {
                if (RecentFiles[i] != "")
                {
                    ((ToolStripMenuItem)MainMenu1.Items[0]).DropDownItems[j + i].Text = $"{(i + 1)}: {Path.GetFileName(RecentFiles[i])}";
                    ((ToolStripMenuItem)MainMenu1.Items[0]).DropDownItems[j + i].Visible = true;
                }
                else
                {
                    ((ToolStripMenuItem)MainMenu1.Items[0]).DropDownItems[j + i].Visible = false;
                }
            }

            ((ToolStripMenuItem)MainMenu1.Items[0]).DropDownItems[j + 6].Visible = ((ToolStripMenuItem)MainMenu1.Items[0]).DropDownItems[j].Visible;
        }

        public void OpenRecent(int n)
        {
            if (RecentFiles[n] != "" && System.IO.File.Exists(RecentFiles[n]))
            {
                OpenDialog.InitialDirectory = Path.GetDirectoryName(RecentFiles[n]);
                OpenDialog.FileName = RecentFiles[n];
                CreateChildWrapper(RecentFiles[n]);
            }
        }

        public void RFile1Click(object sender, EventArgs e)
        {
            OpenRecent(0);
        }

        public void RFile2Click(object sender, EventArgs e)
        {
            OpenRecent(1);
        }

        public void RFile3Click(object sender, EventArgs e)
        {
            OpenRecent(2);
        }

        public void RFile4Click(object sender, EventArgs e)
        {
            OpenRecent(3);
        }

        public void RFile5Click(object sender, EventArgs e)
        {
            OpenRecent(4);
        }

        public void RFile6Click(object sender, EventArgs e)
        {
            OpenRecent(5);
        }

        public void UmPlayingOff()
        {
            RestoreControls();
        }

        /* public void UmFinalizeWO()
        {
            // if TChildWin(ActiveMDIChild).BeetweenPatterns.Checked then exit;
            WaveOutAPI.WOThreadFinalization();
            WaveOutAPI.WaitForWOThreadExit();

            RestoreControls();
            if (Main.LoopAllAllowed && this.MdiChildren.Length > 1)
            {
                this.Next();
                Play1Execute(null, EventArgs.Empty);
            }
        }

        public void UmFinalizeWO()
        {
            // Finalize and join the WOThread safely on the UI thread
            WaveOutAPI.WOThreadFinalization();
            WaveOutAPI.WaitForWOThreadExit(); // Only safe here, on UI thread

            RestoreControls();

            if (Main.LoopAllAllowed && this.MdiChildren.Length > 1)
            {
                this.Next();
                Play1Execute(null, EventArgs.Empty);
            }
        }

        public void UmFinalizeWO()
        {
            // Call finalization logic that stops posting messages
            WaveOutAPI.WOThreadFinalization();

            // Important: RestoreControls and trigger any UI updates BEFORE join
            RestoreControls();

            if (Main.LoopAllAllowed && this.MdiChildren.Length > 1)
            {
                this.Next();
                Play1Execute(null, EventArgs.Empty);
            }

            // Now that nothing else is required from WOThread, join safely
            // WaveOutAPI.WaitForWOThreadExit();
        } */

        public void UmFinalizeWO()
        {
            // STEP 1: Safely wait for WOThread to exit
            WaveOutAPI.WaitForWOThreadExit();

            // STEP 2: Now finalize state cleanly
            WaveOutAPI.WOThreadFinalization();

            // STEP 3: Restore UI
            RestoreControls();

            // STEP 4: Auto-loop next track if enabled
            if (Main.LoopAllAllowed && this.MdiChildren.Length > 1)
            {
                this.Next();
                PlayFrom_Execute(null, EventArgs.Empty);
            }
        }

        private void Next()
        {
            Form[] children = this.MdiChildren;
            int currentIndex = Array.IndexOf(children, this.ActiveMdiChild);
            if (currentIndex == -1)
                return;

            int nextIndex = (currentIndex + 1) % children.Length;
            children[nextIndex].Activate();
        }

        public void ToggleChip_Execute(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetText(UIActionType.ToggleChip, AY.EmulatingChip == ChipType.AY ? "YM" : "AY");

            if (AY.StdChannelsAllocation >= 0 && AY.StdChannelsAllocation <= 6)
                AY.SetStdChannelsAllocation(AY.StdChannelsAllocation);
            else
                AY.CalculateLevelTables();

            if (AY.RenderEngine == 2 && AY.AyumiChip[0] != null)
            {
                for (int i = 0; i < AY.ChipCount; i++)
                {
                    if (AY.AyumiChip[i] == null)
                        continue;

                    AY.AyumiChip[i].SetChipType(AY.EmulatingChip == ChipType.YM);
                }
            }
            else if (WaveOutAPI.IsPlaying)
            {
                // TODO
                for (int i = 0; i < AY.ChipCount; i++)
                {
                    if (ChildForm.PlayingWindow[i] == null)
                        continue;

                    ChildForm.PlayingWindow[i].StopAndRestart();
                }
            }
        }

        public void ToggleChanAlloc_Execute(object sender, EventArgs e)
        {
            if (++MainForm.ChanAllocIndex > 5)
                MainForm.ChanAllocIndex = 0;

            RedrawOff();
            SetChannelsAllocation(MainForm.ChanAllocIndex);
            RedrawOn();

            if (AY.RenderEngine == 2)
                AY.UpdatePanoram();
            else if (WaveOutAPI.IsPlaying)
                ChildForm.PlayingWindow[0].StopAndRestart();
        }

        public void SetChannelsAllocation(int chanAllocIndex)
        {
            int i, c, p, n;
            int[] PrevAlloc = new int[3];
            string Caption = "";
            Helpers.Move(ChanAlloc, PrevAlloc, PrevAlloc.Length);
            MainForm.ChanAllocIndex = chanAllocIndex;

            switch (chanAllocIndex)
            {
                case 0:
                    ChanAlloc[0] = 0;
                    ChanAlloc[1] = 1;
                    ChanAlloc[2] = 2;
                    Caption = "Mono";
                    break;
                case 1:
                    ChanAlloc[0] = 0;
                    ChanAlloc[1] = 1;
                    ChanAlloc[2] = 2;
                    Caption = "ABC";
                    break;
                case 2:
                    ChanAlloc[0] = 0;
                    ChanAlloc[1] = 2;
                    ChanAlloc[2] = 1;
                    Caption = "ACB";
                    break;
                case 3:
                    ChanAlloc[0] = 1;
                    ChanAlloc[1] = 0;
                    ChanAlloc[2] = 2;
                    Caption = "BAC";
                    break;
                case 4:
                    ChanAlloc[0] = 1;
                    ChanAlloc[1] = 2;
                    ChanAlloc[2] = 0;
                    Caption = "BCA";
                    break;
                case 5:
                    ChanAlloc[0] = 2;
                    ChanAlloc[1] = 0;
                    ChanAlloc[2] = 1;
                    Caption = "CAB";
                    break;
                case 6:
                    ChanAlloc[0] = 2;
                    ChanAlloc[1] = 1;
                    ChanAlloc[2] = 0;
                    Caption = "CBA";
                    break;
            }

            UIActionManager.Instance.SetText(UIActionType.ToggleChannels, Caption);

            for (i = 0; i < this.MdiChildren.Length; i++)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[i];
                c = (childForm.Tracks.CursorX - 8) / 14;

                if (c >= 0)
                {
                    p = PrevAlloc[c];
                    n = 0;

                    while (n < 2 && ChanAlloc[n] != p)
                        n++;

                    childForm.Tracks.CursorX += (n - c) * 14;
                }

                childForm.ResetChanAlloc();
            }

            AY.SetStdChannelsAllocation(chanAllocIndex);
        }

        public void DisableControls(bool DisableTracks)
        {
            int i;

            // Disable hints, because they makes 'lags' in pattern scrolling
            //Application.ShowHint = false;

            // Setup playing windows and number of chips
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            ChildForm.SetPlayingWindow(0, activeForm);

            if (ChildForm.PlayingWindow[0] == ChildForm.PlayingWindow[1])
                ChildForm.SetPlayingWindow(1, null);

            if (ChildForm.PlayingWindow[0] == ChildForm.PlayingWindow[2])
                ChildForm.SetPlayingWindow(2, null);

            // Is second playing window present? And its positions length <> 0?
            if (ChildForm.PlayingWindow[0].TSWindow[1] != null && ChildForm.PlayingWindow[0].TSWindow[1].VTM.Positions.Length != 0 &&
                ChildForm.PlayingWindow[0].TSWindow[0] != null && ChildForm.PlayingWindow[0].TSWindow[0].VTM.Positions.Length != 0)
            {
                ChildForm.SetPlayingWindow(1, ChildForm.PlayingWindow[0].TSWindow[0]);
                ChildForm.SetPlayingWindow(2, ChildForm.PlayingWindow[0].TSWindow[1]);
                AY.ChipCount = 3;
            }
            else if (ChildForm.PlayingWindow[0].TSWindow[0] != null && ChildForm.PlayingWindow[0].TSWindow[0].VTM.Positions.Length != 0)
            {
                ChildForm.SetPlayingWindow(1, ChildForm.PlayingWindow[0].TSWindow[0]);
                ChildForm.SetPlayingWindow(2, null);
                AY.ChipCount = 2;
            }
            else
            {
                ChildForm.SetPlayingWindow(1, null);
                ChildForm.SetPlayingWindow(2, null);
                AY.ChipCount = 1;
            }

            // Disable controls for playing windows
            for (i = 0; i < AY.ChipCount; i++)
            {
                if (ChildForm.PlayingWindow[i] == null)
                    continue;

                // Change Play/Stop button state
                ChildForm.PlayingWindow[i].PlayStopState = PlayStopState.Stop;
                // Disable pattern number changing
                ChildForm.PlayingWindow[i].PatternNumUpDown.Enabled = false;
                // Disable pattern length changing
                ChildForm.PlayingWindow[i].PatternLenUpDown.Enabled = false;
                // Disable move between patterns checkbox
                ChildForm.PlayingWindow[i].MoveBetweenPatternsCheckBox.Enabled = false;
                // Disable duplicate note params checkbox
                ChildForm.PlayingWindow[i].UseLastNoteParamsCheckBox.Enabled = false;
                // Disable Envelope As Note checkbox
                ChildForm.PlayingWindow[i].EnvelopeAsNoteCheckBox.Enabled = false;
                // Disable pattern editor
                if (DisableTracks)
                    ChildForm.PlayingWindow[i].Tracks.Enabled = false;
                // Disable range selection for positions list
                //TChildWin.PlayingWindow[i].StringGrid1.Options = TChildWin.PlayingWindow[i].StringGrid1.Options - new object[] { goRangeSelect };
                // If playing only current pattern, then disable change position
                // if PlayMode = PMPlayPattern then
                // PlayingWindow[i].StringGrid1.Options := PlayingWindow[i].StringGrid1.Options - [goRowSelect, goDrawFocusSelected];
            }

            // Disable some context menu items
            RenumberPatterns.Enabled = false;
            AutoNumeratePatterns.Enabled = false;
        }

        public void RestoreControls()
        {
            //Application.ShowHint = true;
            // Restore controls for childs
            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[i];
                // Set Play/Stop button state
                childForm.PlayStopState = PlayStopState.Play;
                // Enable pattern number edit
                childForm.PatternNumUpDown.Enabled = true;
                // Enable pattern length changing
                childForm.PatternLenUpDown.Enabled = true;
                // Set patterns editor cursor position
                // if PlayMode in [PMPlayModule, PMPlayPattern] then
                // Tracks.CursorY := Tracks.N1OfLines;
                // Enable pattern editor
                childForm.Tracks.Enabled = true;
                // Enable range selection for positions list
                //activeForm.StringGrid1.Options = activeForm.StringGrid1.Options + new object[] { activeForm.goRangeSelect };
                // Enable Move Between Patterns checkbox
                childForm.MoveBetweenPatternsCheckBox.Enabled = true;
                // Enable Duplicate Note Params checkbox
                childForm.UseLastNoteParamsCheckBox.Enabled = true;
                // Enable Envelope As Note checkbox
                childForm.EnvelopeAsNoteCheckBox.Enabled = true;
                // Set play/stop button state
                childForm.PlayStopState = PlayStopState.Play;
                RestoreTracksFocus();
                childForm.Tracks.RemoveSelection();
            }
            // Enable some context menu items
            RenumberPatterns.Enabled = true;
            AutoNumeratePatterns.Enabled = true;
            PlayStop_Update(null, EventArgs.Empty);
        }

        public void ScrollToPlayingWindow()
        {
            if (this.MdiChildren.Length == 1)
                return;

            if (ChildsWidth() <= this.ClientSize.Width)
                return;

            var activateForm = (ChildForm)this.ActiveMdiChild;

            // Check if already fully visible
            var winRect = activateForm.Bounds;
            if (winRect.Left > 0 && winRect.Right < this.ClientSize.Width)
                return;

            // Let WinForms scroll it into view
            this.ScrollControlIntoView(activateForm);

            activateForm.Activate();
        }

        /* public void ScrollToPlayingWindow()
        {
            int childIndex;
            bool firstChild;
            bool lastChild;

            if (this.MdiChildren.Length == 1)
                return;

            if (ChildsWidth() <= this.ClientSize.Width)
                return;

            TChildWin activeWin = (TChildWin)this.ActiveMdiChild;
            Rectangle winRect = activeForm.Bounds;

            if (winRect.Left > 0 && winRect.Right < this.ClientSize.Width)
                return;

            childIndex = 0;

            for (int i = ChildsTable.Length - 1; i >= 0; i--)
            {
                if (ChildsTable[i] != null)
                {
                    childIndex = i;
                    break;
                }
            }

            lastChild = ChildsTable[childIndex] == activeWin;
            childIndex = 0;

            for (int i = 0; i < ChildsTable.Length; i++)
            {
                if (ChildsTable[i] != null)
                {
                    childIndex = i;
                    break;
                }
            }

            firstChild = ChildsTable[childIndex] == activeWin;

            IntPtr clientHandle = ClientHandle();

            if (firstChild)
                PostMessage(clientHandle, WM_HSCROLL, SB_LEFT, 0);
            else if (lastChild || (Math.Abs(winRect.Left - ChildsWidth()) < this.ClientSize.Width))
                PostMessage(clientHandle, WM_HSCROLL, SB_RIGHT, 0);
            else
            {
                int offset = winRect.Left - (this.ClientSize.Width / 2) + (LastChildWidth / 2);
                ScrollWindow(clientHandle, -offset, 0, IntPtr.Zero, IntPtr.Zero);
                PostMessage(clientHandle, WM_HSCROLL, SB_LINELEFT, 0);
                PostMessage(clientHandle, WM_HSCROLL, SB_LINERIGHT, 0);
            }

            activeForm.BringToFront();
        } */

        // procedure TCheckSecondWindow(DisableTracks: Boolean);
        // begin
        // if PlayingWindow[1].TSWindow <> nil then
        // begin
        // PlayingWindow[2] := PlayingWindow[1].TSWindow;
        // if (PlayingWindow[1] <> PlayingWindow[2]) and (PlayingWindow[2].VTM.Positions.Length <> 0) then
        // begin
        // NumberOfSoundChips := 2;
        // PlayingWindow[2].PlayStopState := TPlayStopState.BStop;
        // PlayingWindow[2].PatternNumEdit.Enabled := False;
        // PlayingWindow[2].PatternNumUpDown.Enabled := False;
        // // Disable pattern length changing
        // PlayingWindow[2].PatternLenEdit.Enabled   := False;
        // PlayingWindow[2].PatternLenUpDown.Enabled := False;
        // if DisableTracks then
        // PlayingWindow[2].Tracks.Enabled := False;
        // PlayingWindow[2].TSBut.Enabled := False;
        // PlayingWindow[2].StringGrid1.Options := PlayingWindow[2].StringGrid1.Options - [goRangeSelect];
        // PlayingWindow[2].BeetweenPatterns.Enabled := False;
        // PlayingWindow[2].DuplicateNoteParams.Enabled := False;
        // PlayingWindow[2].EnvelopeAsNote.Enabled := False;
        // RenumberPatterns.Enabled := False;
        // AutoNumeratePatterns.Enabled := False;
        // end;
        // end;
        // PlayingWindow[1].TSBut.Enabled := False;
        // end;
        public void ToggleSamples_Execute(object sender, EventArgs e)
        {
            Globals.ToggleSamplesForm.Visible = !Globals.ToggleSamplesForm.Visible;
        }

        public void TracksManager_Execute(object sender, EventArgs e)
        {
            Globals.TracksManagerForm.Visible = !Globals.TracksManagerForm.Visible;
        }

        public void GlobalTransposition_Execute(object sender, EventArgs e)
        {
            Globals.GlobalTransForm.Visible = !Globals.GlobalTransForm.Visible;
        }

        public void PluginManager_Execute(object sender, EventArgs e)
        {
            Globals.PluginManagerForm.Visible = !Globals.PluginManagerForm.Visible;
        }

        public void VolumeTrackBar_ValueChanged(object sender, EventArgs e)
        {
            Main.GlobalVolume = VolumeTrackBar.Value;
            AY.CalculateLevelTables();
        }

        // procedure CheckSecondWindow(DisableTracks: Boolean);
        // 
        // 
        // // Commented code block, because users really
        // // don't need templates for samples
        // 
        // procedure SetSampleTemplate(Tmp: integer);
        // procedure AddToSampTemplate(const SamTik: TSampleTick);
        // procedure ResetSampTemplate;
        // 
        // 
        public void SetEmulatingChip(ChipType chipType)
        {
            if (AY.EmulatingChip != chipType)
            {
                AY.EmulatingChip = chipType;
                UIActionManager.Instance.SetText(UIActionType.ToggleChip, AY.EmulatingChip == ChipType.AY ? "AY" : "YM");
                AY.CalculateLevelTables();
            }
        }

        public void ResetConfig()
        {
            SetPriority(0);
            IniFile iniFile = GetConfigIniFile();

            iniFile.SetValue("General", "SampleRate", 44100);
            iniFile.SetValue("General", "SampleBit", 16);
            iniFile.SetValue("General", "BufLen_ms", 100);
            iniFile.SetValue("General", "NumberOfBuffers", 3);
            iniFile.SetValue("General", "WODevice", 0);
            iniFile.SetValue("General", "Optimization", 1);
            iniFile.SetValue("General", "Filtering", 1);
            iniFile.SetValue("General", "FilterQ", 64);
            iniFile.SetValue("General", "Priority", 32);
            iniFile.SetValue("General", "AY_Freq", 1750000);
            iniFile.SetValue("General", "Interrupt_Freq", 48828);
            iniFile.SetValue("General", "NumberOfChannels", 2);
            iniFile.Save();
        }

        public void GetFileAssocFromText(string fileAssocText)
        {
            if (fileAssocText.Length != FileAssociations.Length)
                return;

            for (int i = 0; i < fileAssocText.Length; i++)
                FileAssociations[i, 0] = fileAssocText[i].ToString();
        }

        public string FileAssocToText()
        {
            string result = "";

            for (int i = 0; i < FileAssociations.GetLowerBound(0); i++)
                result += FileAssociations[i, 0];

            return result;
        }

        public void WriteConfig()
        {
            int i;
            SetPriority(ProcessPriorityClass.Normal);
            IniFile iniFile = GetConfigIniFile();
            iniFile.SetValue("General", "ConfigInitialized", true);
            iniFile.SetValue("General", "Version", VersionString);
            iniFile.SetValue("General", "StartupAction", StartupAction);
            iniFile.SetValue("General", "TemplateSongPath", TemplateSongPath);
            iniFile.SetValue("General", "SamplesDir", SamplesDir);
            iniFile.SetValue("General", "OrnamentsDir", OrnamentsDir);
            iniFile.SetValue("General", "SamplesQuickDir", SamplesQuickDir);
            iniFile.SetValue("General", "OrnamentsQuickDir", OrnamentsQuickDir);
            iniFile.SetValue("General", "AutoBackups", AutoBackupsOn);
            iniFile.SetValue("General", "AutoBackupsMins", AutoBackupsMins);
            iniFile.SetValue("General", "Priority", Priority);
            iniFile.SetValue("General", "ChanAllocIndex", MainForm.ChanAllocIndex);
            iniFile.SetValue("General", "PanoramA", Panoram[0]);
            iniFile.SetValue("General", "PanoramB", Panoram[1]);
            iniFile.SetValue("General", "PanoramC", Panoram[2]);
            iniFile.SetValue("General", "DefaultChipFreq", Main.DefaultChipFreq);
            iniFile.SetValue("General", "ManualChipFreq", Main.ManualChipFreq);
            iniFile.SetValue("General", "DefaultIntFreq", Main.DefaultIntFreq);
            iniFile.SetValue("General", "SampleRate", WaveOutAPI.SampleRate);
            iniFile.SetValue("General", "SampleBit", WaveOutAPI.SampleBit);
            iniFile.SetValue("General", "NumberOfChannels", WaveOutAPI.NumberOfChannels);
            iniFile.SetValue("General", "BufLen_ms", WaveOutAPI.BufferLengthMs);
            iniFile.SetValue("General", "NumberOfBuffers", WaveOutAPI.BufferCount);
            iniFile.SetValue("General", "WODevice", WaveOutAPI.WODevice);
            iniFile.SetValue("General", "ChipType", (int)(AY.EmulatingChip));
            iniFile.SetValue("General", "RenderEngine", AY.RenderEngine);
            iniFile.SetValue("General", "FeaturesLevel", VTModule.FeaturesLevel);
            iniFile.SetValue("General", "DetectFeaturesLevel", VTModule.DetectFeaturesLevel);
            iniFile.SetValue("General", "VortexModuleHeader", VTModule.VortexModuleHeader);
            iniFile.SetValue("General", "DetectModuleHeader", VTModule.DetectModuleHeader);
            iniFile.SetValue("General", "ExportSampleRate", ExportSampleRate);
            iniFile.SetValue("General", "ExportBitRate", ExportBitRate);
            iniFile.SetValue("General", "ExportChannels", ExportChannels);
            iniFile.SetValue("General", "ExportChip", ExportChip);
            iniFile.SetValue("General", "ExportRepeats", ExportRepeats);
            iniFile.SetValue("General", "ExportPath", ExportPath);
            for (i = 0; i < 6; i++)
                iniFile.SetValue("General", "Recent" + i.ToString(), RecentFiles[i]);
            i = AY.LoopAllowed ? 1 : Main.LoopAllAllowed ? 2 : 0;
            iniFile.SetValue("General", "LoopMode", i);
            iniFile.SetValue("General", "GlobalVolume", Main.GlobalVolume);
            iniFile.SetValue("General", "TrackFontName", EditorFont.Name);
            iniFile.SetValue("General", "FileAssoc", FileAssocToText());
            //iniFile.SetValue("General", "Shortcuts", HotKeys.AllHotKeysToText());

            HotKeys.WriteConfig(iniFile);
            
            iniFile.SetValue("General", "TrackFontSize", EditorFont.Size);
            iniFile.SetValue("General", "PositionSize", PositionSize);
            iniFile.SetValue("General", "TrackFontBold", (EditorFont.Style & FontStyle.Bold) != 0);
            iniFile.SetValue("General", "WindowMaximized", this.WindowState == FormWindowState.Maximized);
            iniFile.SetValue("General", "DefaultTable", DefaultTable);
            if (this.WindowState != FormWindowState.Maximized)
            {
                iniFile.SetValue("General", "WindowX", this.Left);
                iniFile.SetValue("General", "WindowY", this.Top);
                iniFile.SetValue("General", "WindowWidth", LastChildWidth + DoubleBorderSize());
                iniFile.SetValue("General", "WindowHeight", this.Height);
            }
            iniFile.SetValue("General", "Filtering", AY.FilterEnabled);
            iniFile.SetValue("General", "FilterQ", AY.FilterLength);
            iniFile.SetValue("General", "DCType", AY.DCType);
            iniFile.SetValue("General", "DCCutOff", AY.DCCutOff);
            iniFile.SetValue("General", "ColorThemeName", ColorThemeName);
            iniFile.SetValue("General", "EnvelopeAsNote", EnvelopeAsNote);
            iniFile.SetValue("General", "SamToneShiftAsNote", SamToneShiftAsNote);
            iniFile.SetValue("General", "OrnToneShiftAsNote", OrnToneShiftAsNote);
            iniFile.SetValue("General", "DecBaseLines", MainForm.DecBaseLinesOn);
            iniFile.SetValue("General", "DecBaseNoise", DecBaseNoiseOn);
            iniFile.SetValue("General", "HighlightSpeed", HighlightSpeedOn);
            iniFile.SetValue("General", "DupNoteParams", DupNoteParams);
            iniFile.SetValue("General", "MoveBetweenPatrns", MoveBetweenPatrns);
            iniFile.SetValue("General", "DisableSeparators", DisableSeparators);
            int numThemes = ColorThemes.VTColorThemes?.Length ?? 0;
            iniFile.SetValue("General", "NumThemes", 0);
            for (i = 0; i < numThemes; i++)
            {
                string section = $"Theme{i + 1}";
                ColorTheme colorTheme = ColorThemes.VTColorThemes[i];
                string colors = string.Join(",", colorTheme.Colors.Take((int)ThemeColor.Count).Select(ColorThemes.ColorToString));
                iniFile.SetValue(section, "Name", colorTheme.Name);
                iniFile.SetValue(section, "Colors", colors);
            }
            iniFile.SetValue("General", "WinThemeIndex", WinThemeIndex);
            for (i = 0; i < 3; i++)
                iniFile.SetValue("General", "ToolBar" + i.ToString(), ((ToolStripMenuItem)PopupMenu3.Items[i]).Checked);
            iniFile.SetValue("General", "DisableHints", DisableHints);
            iniFile.SetValue("General", "DisableCtrlClick", DisableCtrlClick);
            iniFile.SetValue("General", "DisableInfoWin", DisableInfoWin);
            iniFile.SetValue("General", "SampleBrowserVisible", SampleBrowserVisible);
            iniFile.SetValue("General", "OrnamentsBrowserVisible", OrnamentsBrowserVisible);
            iniFile.Save();
            if (!VTExit)
                SendSyncMessage();
        }

        public bool IsFontValid(string fontName)
        {
            bool result;
            int celW, celH;
            Size sz;

            using (Graphics g = Graphics.FromHwnd(this.Handle))
            {
                using (Font testFont = new Font(fontName, 12))
                {
                    sz = TextRenderer.MeasureText(g, "0", testFont, Size.Empty, TextFormatFlags.NoPadding);
                    celW = sz.Width;
                    celH = sz.Height;
                    result = (celW > 6) && (celH > 6) && (celH < 60) && (celW < 60);
                }
            }

            return result;
        }

        /* public string LoadOptions_LoadTheme(int ThemeNum)
        {
            string result;
            result = ini.Read("Themes", ThemeNum.ToString(), "");
            return result;
        }

        public string iniFile.GetValue("General", string ParamName, string DefaultValue)
        {
            string result;
            result = ini.Read("General", ParamName, DefaultValue);
            return result;
        }

        public int iniFile.GetValue<int>("General", string ParamName, int DefaultValue)
        {
            int result;
            result = ini.ReadInteger("General", ParamName, DefaultValue);
            return result;
        }

        public bool iniFile.GetValue<bool>("General", string ParamName, bool DefaultValue)
        {
            bool result;
            int def;
            if (DefaultValue)
            {
                def = 1;
            }
            else
            {
                def = 0;
            }
            result = ini.ReadInteger("General", ParamName, def) == 1;
            return result;
        } */

        public void ReadConfig()
        {
            string s;
            int i;
            int defFont;
            bool b;

            // Remove config if version changed
            IniFile iniFile = GetConfigIniFile();

            //if (iniFile.GetValue("General", "Version", "") != VersionString)
            //    System.IO.File.Delete(ConfigFilePath);

            // Vortex started first time with clean config
            if (!iniFile.GetValue<bool>("General", "ConfigInitialized", false))
                VortexFirstStart = true;

            StartupAction = iniFile.GetValue<byte>("General", "StartupAction", 1);
            TemplateSongPath = iniFile.GetValue("General", "TemplateSongPath", "");
            AutoBackupsOn = iniFile.GetValue<bool>("General", "AutoBackups", true);
            AutoBackupsMins = iniFile.GetValue<byte>("General", "AutoBackupsMins", 1);

            if (VortexFirstStart)
                CheckFileAssociations();
            else
            {
                s = iniFile.GetValue("General", "FileAssoc", "");
                if (s != "")
                    GetFileAssocFromText(s);
            }
            /* s = iniFile.GetValue("General", "Shortcuts", "");
            if (s != "")
                HotKeys.LoadHotKeysFromText(s);
            else
                HotKeys.SetDefaultHotKeys(); */
            
            HotKeys.ReadConfig(iniFile);

            ProcessPriorityClass priority = iniFile.GetValue("General", "Priority", ProcessPriorityClass.Normal);
            SetPriority(priority);
            SamplesDir = iniFile.GetValue("General", "SamplesDir", Path.Combine(VortexDocumentsDir, SamplesDefaultDir));
            OrnamentsDir = iniFile.GetValue("General", "OrnamentsDir", Path.Combine(VortexDocumentsDir, OrnamentsDefaultDir));

            if (!Directory.Exists(SamplesDir))
                SamplesDir = "C:\\";

            if (!Directory.Exists(OrnamentsDir))
                OrnamentsDir = "C:\\";

            SamplesQuickDir = iniFile.GetValue("General", "SamplesQuickDir", "");
            OrnamentsQuickDir = iniFile.GetValue("General", "OrnamentsQuickDir", "");

            if (!Directory.Exists(SamplesQuickDir))
                SamplesQuickDir = "";

            if (!Directory.Exists(OrnamentsQuickDir))
                OrnamentsQuickDir = "";

            if (!SyncVTInstances)
            {
                EnvelopeAsNote = iniFile.GetValue<bool>("General", "EnvelopeAsNote", false);
                SamToneShiftAsNote = iniFile.GetValue<bool>("General", "SamToneShiftAsNote", false);
                OrnToneShiftAsNote = iniFile.GetValue<bool>("General", "OrnToneShiftAsNote", false);
                MoveBetweenPatrns = iniFile.GetValue<bool>("General", "MoveBetweenPatrns", true);
                DupNoteParams = iniFile.GetValue<bool>("General", "DupNoteParams", false);
                VolumeTrackBar.Value = iniFile.GetValue<int>("General", "GlobalVolume", 56);
                MainForm.ChanAllocIndex = iniFile.GetValue<int>("General", "ChanAllocIndex", 1);
                SetChannelsAllocation(MainForm.ChanAllocIndex);
                Panoram[0] = iniFile.GetValue<byte>("General", "PanoramA", 64);
                Panoram[1] = iniFile.GetValue<byte>("General", "PanoramB", 128);
                Panoram[2] = iniFile.GetValue<byte>("General", "PanoramC", 192);
                int loopMode = iniFile.GetValue<int>("General", "LoopMode", 1);

                switch (loopMode)
                {
                    case 1:
                        UIActionManager.Instance.Execute(this, UIActionType.ToggleLooping);
                        break;
                    case 2:
                        UIActionManager.Instance.Execute(this, UIActionType.ToggleLoopingAll);
                        break;
                }
            }
            if (!WaveOutAPI.IsPlaying)
            {
                WaveOutAPI.SampleRate = iniFile.GetValue<int>("General", "SampleRate", 44100);
                AY.SetSampleRate(WaveOutAPI.SampleRate);
                WaveOutAPI.SampleBit = iniFile.GetValue<int>("General", "SampleBit", 16);
                AY.SetBitRate(WaveOutAPI.SampleBit);
                WaveOutAPI.NumberOfChannels = iniFile.GetValue<int>("General", "NumberOfChannels", 2);
                AY.SetNChans(WaveOutAPI.NumberOfChannels);
                WaveOutAPI.BufferCount = iniFile.GetValue<int>("General", "NumberOfBuffers", 3);
                WaveOutAPI.BufferLengthMs = iniFile.GetValue<int>("General", "BufLen_ms", 100);
                AY.SetBuffers(WaveOutAPI.BufferLengthMs, WaveOutAPI.BufferCount);
                WaveOutAPI.WODevice = iniFile.GetValue("General", "WODevice", null);
            }
            if (!WaveOutAPI.IsPlaying || (AY.PlayMode == PlayModes.PlayLine))
            {
                AY.DCType = iniFile.GetValue<int>("General", "DCType", 1);
                AY.DCCutOff = iniFile.GetValue<int>("General", "DCCutOff", 3);
                int chipType = iniFile.GetValue<int>("General", "ChipType", 2); // YM by default
                SetEmulatingChip((ChipType)(chipType == 1 || chipType == 2 ? chipType : 2));
                int renderEngine = iniFile.GetValue<int>("General", "RenderEngine", 2); // Ayumi render by default
                AY.Set_Engine(renderEngine);
            }

            ExportSampleRate = iniFile.GetValue<int>("General", "ExportSampleRate", 1);
            ExportBitRate = iniFile.GetValue<int>("General", "ExportBitRate", 0);
            ExportChannels = iniFile.GetValue<int>("General", "ExportChannels", 2);
            ExportChip = iniFile.GetValue<int>("General", "ExportChip", 1);
            ExportRepeats = iniFile.GetValue<int>("General", "ExportRepeats", 0);
            ExportPath = iniFile.GetValue("General", "ExportPath", "");
            VTModule.FeaturesLevel = iniFile.GetValue<FeaturesLevel>("General", "FeaturesLevel", FeaturesLevel.AutoDetect);
            VTModule.DetectFeaturesLevel = iniFile.GetValue<bool>("General", "DetectFeaturesLevel", true);
            VTModule.VortexModuleHeader = iniFile.GetValue<bool>("General", "VortexModuleHeader", true);
            VTModule.DetectModuleHeader = iniFile.GetValue<bool>("General", "DetectModuleHeader", true);
            Main.DefaultChipFreq = iniFile.GetValue<int>("General", "DefaultChipFreq", 1750000);
            Main.ManualChipFreq = iniFile.GetValue<int>("General", "ManualChipFreq", 0);
            Main.DefaultIntFreq = iniFile.GetValue<int>("General", "DefaultIntFreq", 48828);
            bool filtering = iniFile.GetValue<bool>("General", "Filtering", true);
            AY.SetFilter(filtering, AY.FilterLength);
            int filterQ = iniFile.GetValue<int>("General", "FilterQ", 64);
            AY.SetFilter(AY.FilterEnabled, filterQ);
            DefaultTable = (short)iniFile.GetValue<int>("General", "DefaultTable", 2);
            for (i = 5; i >= 0; i--)
            {
                s = iniFile.GetValue("General", ("Recent" + i.ToString() as string), "");
                if (s != "" && System.IO.File.Exists(s))
                {
                    AddFileName(s);
                }
            }
            defFont = 16;
            if (VortexFirstStart)
            {
                if (MonitorWorkAreaHeight() <= 600)
                {
                    this.Top = 0;
                }
                else if (MonitorWorkAreaHeight() <= 768)
                {
                    this.Top = 5;
                }
                else if (MonitorWorkAreaHeight() <= 840)
                {
                    this.Top = 10;
                }
                else if (MonitorWorkAreaHeight() <= 900)
                {
                    this.Top = 15;
                }
                else if (MonitorWorkAreaHeight() <= 1024)
                {
                    this.Top = 20;
                }
                else if (MonitorWorkAreaHeight() <= 1200)
                {
                }
                else if (MonitorWorkAreaHeight() <= 1400)
                {
                    defFont = 17;
                }
                else if (MonitorWorkAreaHeight() <= 1600)
                {
                    defFont = 18;
                }
                else
                {
                    defFont = 20;
                }
            }
            PositionSize = iniFile.GetValue<int>("General", "PositionSize", 2);
            if (!SyncVTInstances)
            {
                this.Width = iniFile.GetValue<int>("General", "WindowWidth", (defFont - 4) * 52);
                this.Height = iniFile.GetValue<int>("General", "WindowHeight", MonitorWorkAreaHeight() - 60);
                this.Left = iniFile.GetValue<int>("General", "WindowX", (MonitorWorkAreaWidth() / 2) - (this.Width / 2) - 200);
                this.Top = iniFile.GetValue<int>("General", "WindowY", (MonitorWorkAreaHeight() / 2) - (this.Height / 2));
                // Window on a second monitor, but monitor turned off.
                Rectangle workingArea = Screen.FromControl(this).WorkingArea;

                if (this.Left < workingArea.Left || this.Left >= workingArea.Width)
                {
                    this.Left = (MonitorWorkAreaWidth() / 2) - (this.Width / 2) - 200;
                }
                if (this.Top < workingArea.Top || this.Top >= workingArea.Height)
                {
                    this.Top = (MonitorWorkAreaHeight() / 2) - (this.Height / 2);
                }
                if (iniFile.GetValue<bool>("General", "WindowMaximized", false))
                {
                    this.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    this.WindowState = FormWindowState.Normal;
                }
            }
            string editorFontName = iniFile.GetValue("General", "TrackFontName", "Consolas");
            float editorFontSize;
            FontStyle editorFontStyle = FontStyle.Regular;
            if (IsFontExists(editorFontName))
            {
            }
            else if (IsFontExists("Consolas"))
            {
                editorFontName = "Consolas";
            }
            else if (IsFontExists("Courier New"))
            {
                editorFontName = "Courier New";
            }
            if (!IsFontValid(editorFontName))
            {
                editorFontName = "Consolas";
                editorFontSize = defFont;
                if (!IsFontValid(editorFontName))
                {
                    editorFontName = "Lucida Console";
                    editorFontSize = defFont;
                    if (!IsFontValid(editorFontName))
                    {
                        editorFontName = "Courier New";
                        editorFontSize = defFont;
                    }
                }
            }
            editorFontSize = iniFile.GetValue<int>("General", "TrackFontSize", defFont);
            if (editorFontSize < 12)
            {
                editorFontSize = 12;
            }
            if (iniFile.GetValue<bool>("General", "TrackFontBold", false))
            {
                editorFontStyle |= FontStyle.Bold;
            }
            else
            {
                editorFontStyle &= ~FontStyle.Bold;
            }

            if (MainForm.TryGetFontFamily(editorFontName, out FontFamily fontFamily))
                EditorFont = new Font(fontFamily, editorFontSize, editorFontStyle);

            var themes = new List<ColorTheme>();
            for (i = 1; ; i++)
            {
                string sec = $"Theme{i}";
                string name = iniFile.GetValue(sec, "Name", null);
                string colors = iniFile.GetValue(sec, "Colors", null);

                if (name == null || colors == null)
                    break;

                ColorTheme colorTheme = ColorThemes.LoadColorThemeFromStr(name, colors);
                themes.Add(colorTheme);
            }
            ColorThemes.VTColorThemes = themes.ToArray();
            ColorThemeName = iniFile.GetValue("General", "ColorThemeName", "Default");
            ColorThemes.InitColorThemes();
            WinThemeIndex = iniFile.GetValue<int>("General", "WinThemeIndex", 0);
            ColorThemes.SetWindowColors(WinThemeIndex);
            MainForm.DecBaseLinesOn = iniFile.GetValue<bool>("General", "DecBaseLines", false);
            DecBaseNoiseOn = iniFile.GetValue<bool>("General", "DecBaseNoise", false);
            DisableSeparators = iniFile.GetValue<bool>("General", "DisableSeparators", false);
            HighlightSpeedOn = iniFile.GetValue<bool>("General", "HighlightSpeed", false);
            TracksCursorXLeft = MainForm.DecBaseLinesOn ? 4 : 3;
            for (i = 0; i < 3; i++)
            {
                b = iniFile.GetValue<bool>("General", "ToolBar" + i.ToString(), true);
                SetBar(i, b);
            }
            DisableHints = iniFile.GetValue<bool>("General", "DisableHints", false);
            DisableCtrlClick = iniFile.GetValue<bool>("General", "DisableCtrlClick", false);
            DisableInfoWin = iniFile.GetValue<bool>("General", "DisableInfoWin", false);
            SampleBrowserVisible = iniFile.GetValue<bool>("General", "SampleBrowserVisible", true);
            OrnamentsBrowserVisible = iniFile.GetValue<bool>("General", "OrnamentsBrowserVisible", true);

            if (VortexFirstStart)
                WriteConfig();
        }

        public static bool IsFileAssociationExists(string fileExt)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey($@"Software\Classes\{fileExt}", writable: false))
            {
                return key != null;
            }
        }

        public bool IsVortexFileAssociation(string fileExt, string assocName)
        {
            bool extOk = false;
            bool pathOk = false;
            bool iconOk = false;

            string exePath = $"\"{Process.GetCurrentProcess().MainModule.FileName}\"";
            string expectedCommand = exePath + " \"%1\"";
            string expectedIcon = exePath + ",0";

            using (RegistryKey classesRoot = Registry.CurrentUser.OpenSubKey(@"Software\Classes", writable: false))
            {
                if (classesRoot != null)
                {
                    using (RegistryKey extKey = classesRoot.OpenSubKey(fileExt))
                    {
                        if (extKey != null)
                        {
                            string defaultVal = extKey.GetValue("") as string;
                            if (defaultVal == "VortexTracker2")
                                extOk = true;
                        }
                    }

                    using (RegistryKey commandKey = classesRoot.OpenSubKey($@"{assocName}\shell\open\command"))
                    {
                        if (commandKey != null)
                        {
                            string command = commandKey.GetValue("") as string;
                            if (command == expectedCommand)
                                pathOk = true;
                        }
                    }

                    using (RegistryKey iconKey = classesRoot.OpenSubKey($@"{assocName}\DefaultIcon"))
                    {
                        if (iconKey != null)
                        {
                            string icon = iconKey.GetValue("") as string;
                            if (icon == expectedIcon)
                                iconOk = true;
                        }
                    }
                }
            }

            return extOk && pathOk && iconOk;
        }

        public void CheckFileAssociations()
        {
            for (int i = 0; i < FileAssociations.GetLength(0); i++)
            {
                string fileExt = FileAssociations[i, 1];
                string assocName = FileAssociations[i, 2];

                // Unckeck file association if already taken by another application
                if (IsFileAssociationExists(fileExt) && !IsVortexFileAssociation(fileExt, assocName))
                    FileAssociations[i, 0] = "0"; // Uncheck if already taken
            }
        }

        public void CreateAssociation(string fileExt, string name, string description)
        {
            string exePath = $"\"{Process.GetCurrentProcess().MainModule.FileName}\"";

            using (RegistryKey classes = Registry.CurrentUser.OpenSubKey("Software\\Classes", writable: true))
            {
                if (classes == null) return;

                using (RegistryKey extKey = classes.CreateSubKey(fileExt))
                {
                    extKey?.SetValue("", "VortexTracker2");
                }

                using (RegistryKey nameKey = classes.CreateSubKey(name))
                {
                    nameKey?.SetValue("", description);
                }

                using (RegistryKey iconKey = classes.CreateSubKey(name + "\\DefaultIcon"))
                {
                    iconKey?.SetValue("", exePath + ",0");
                }

                using (RegistryKey commandKey = classes.CreateSubKey(name + "\\shell\\open\\command"))
                {
                    commandKey?.SetValue("", exePath + " \"%1\"");
                }
            }
        }

        void RemoveAssociation(string fileExt, string name)
        {
            using (RegistryKey classes = Registry.CurrentUser.OpenSubKey("Software\\Classes", writable: true))
            {
                classes?.DeleteSubKeyTree(fileExt, false);
                classes?.DeleteSubKeyTree(name, false);
            }
        }

        public void SetFileAssociations()
        {
            FileAssocChanged = false;
            int count = FileAssociations.GetLength(0);

            for (int i = 0; i < count; i++)
            {
                string state = FileAssociations[i, 0];
                string fileExt = FileAssociations[i, 1];
                string assocName = FileAssociations[i, 2];

                if ((state == "1" && (!IsFileAssociationExists(fileExt) || !IsVortexFileAssociation(fileExt, assocName))) ||
                    (state == "0" && IsFileAssociationExists(fileExt) && IsVortexFileAssociation(fileExt, assocName)))
                {
                    FileAssocChanged = true;
                    break;
                }
            }

            if (!FileAssocChanged)
                return;

            for (int i = 0; i < count; i++)
            {
                string state = FileAssociations[i, 0];
                string fileExt = FileAssociations[i, 1];
                string assocName = FileAssociations[i, 2];
                string description = FileAssociations[i, 3];

                RemoveAssociation(fileExt, assocName);

                if (state == "1")
                    CreateAssociation(fileExt, assocName, description);
            }

            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
            FileAssocChanged = false;
        }

        public void SaveSNDHMenu_Click(object sender, EventArgs e)
        {
            if (MdiChildren.Length == 0)
                return;

            var activeForm = (ChildForm)ActiveMdiChild;
            SaveDialogSNDH.InitialDirectory ??= OpenDialog.InitialDirectory;
            SaveDialogSNDH.FileName = string.IsNullOrEmpty(activeForm.WinFileName)
                ? $"VTIIModule{activeForm.WinNumber}"
                : Path.ChangeExtension(activeForm.WinFileName, null);

            string filename;
            do
            {
                if (SaveDialogSNDH.ShowDialog() != DialogResult.OK)
                    return;

                filename = SaveDialogSNDH.FileName;
                var ext = Path.GetExtension(filename)?.ToLower();

                if (ext == ".snd")
                    filename += "h";
                else if (ext != ".sndh")
                    filename += ".sndh";

            }
            while (!AllowSave(filename));

            SaveDialogSNDH.InitialDirectory = Path.GetDirectoryName(filename);

            // Load embedded resource
            byte[] playerData;
            string resourceName = "VortexTracker.Resources.Players.SNDHPLAYER.bin";
            Assembly asm = Assembly.GetExecutingAssembly();
            using (Stream resource = asm.GetManifestResourceStream(resourceName))
            {
                if (resource == null)
                {
                    MessageBox.Show("Embedded Resource Not Found: " + resourceName);
                    return;
                }

                playerData = new byte[resource.Length];
                resource.ReadExactly(playerData, 0, (int)resource.Length);
            }

            byte[] data = new byte[65536];
            var pt3 = new PT3();
            VTM2PT3 vtm2PT3 = new VTM2PT3();

            if (!vtm2PT3.Initialize(data, pt3, activeForm.VTM, out int pt3Size))
            {
                MessageBox.Show("Cannot Compile Module Due 65536 Size Limit For PT3-Modules. You Can Save It In Text Yet.", filename);
                return;
            }

            using var f = new BinaryWriter(System.IO.File.Create(filename));

            f.Write(playerData, 0, 16);
            int sndhhdrsz = 10;

            void WriteTag(string tag, string value)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var bytes = Encoding.ASCII.GetBytes(value + '\0');
                    f.Write(Encoding.ASCII.GetBytes(tag));
                    f.Write(bytes);
                    sndhhdrsz += tag.Length + bytes.Length;
                }
            }

            WriteTag("TITL", activeForm.VTM.Title);
            WriteTag("COMM", activeForm.VTM.Author);
            WriteTag("CONV", MainForm.FullVersString);

            string year = "";
            if (Globals.InputQuery("SNDHv2 Extra TAG", "Year Of Release (Empty If No):", ref year))
                WriteTag("YEAR", year.Trim());

            int j = (int)Math.Round(WaveOutAPI.InterruptFreq / 1000.0);
            string tc = "TC" + j.ToString();
            f.Write(Encoding.ASCII.GetBytes(tc + '\0'));
            sndhhdrsz += tc.Length + 1;

            f.Write(Encoding.ASCII.GetBytes("TIME"));
            int duration = (int)Math.Round((double)activeForm.TotInts / j);
            if (duration > 65535) duration = 65535;
            f.Write(Helpers.IntelWord((ushort)duration));
            sndhhdrsz += 6;

            if ((sndhhdrsz & 1) != 0)
            {
                f.Write((byte)0);
                sndhhdrsz++;
            }

            f.Write(playerData, 16, playerData.Length - 16);
            f.Write(data, 0, pt3Size);

            // Patch offsets
            int patchBase = 0;
            for (int i = 0; i < 3; i++)
            {
                patchBase += 4;
                int patchOffset = Helpers.IntelWord(BitConverter.ToUInt16(playerData, patchBase)) + sndhhdrsz;
                f.BaseStream.Seek(2 + i * 4, SeekOrigin.Begin);
                f.Write(Helpers.IntelWord((ushort)patchOffset));
            }
        }

        public void SaveForZXMenu_Click(object sender, EventArgs e)
        {
            const string errMsg = "Cannot Compile Module Due 65536 Size Limit For PT3-Modules. You Can Save it in Text.";

            if (MdiChildren.Length == 0)
                return;

            ChildForm currentWindow = (ChildForm)ActiveMdiChild;

            PT3 PT3_1 = new PT3();
            PT3 PT3_2 = new PT3();
            byte[] data = new byte[65536];

            VTM2PT3 vtm2Pt3 = new VTM2PT3();

            // TODO: out errMsg
            if (!vtm2Pt3.Initialize(data, PT3_1, currentWindow.VTM, out ExportZXForm.ZXModSize1))
            {
                MessageBox.Show(errMsg, currentWindow.Text);
                return;
            }

            ExportZXForm.ZXModSize2 = 0;

            // TODO: out errMsg
            if (currentWindow.TSWindow[0] != null)
            {
                if (!vtm2Pt3.Initialize(data, PT3_2, currentWindow.TSWindow[0].VTM, out ExportZXForm.ZXModSize2))
                {
                    MessageBox.Show(errMsg, currentWindow.TSWindow[0].Text);
                    return;
                }
            }

            string resourceName = currentWindow.TSWindow[0] == null ? "VortexTracker.Resources.Players.ZXAYPLAYER.bin" : "VortexTracker.Resources.Players.ZXTSPLAYER.bin";
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream resource = assembly.GetManifestResourceStream(resourceName))
            {
                if (resource == null)
                {
                    MessageBox.Show($"Embedded Resource Not Found: {resourceName}", Application.ProductName);
                    return;
                }

                using (BinaryReader reader = new BinaryReader(resource))
                {
                    ExportZXForm.ZxPlayerSize = reader.ReadUInt16();
                    ExportZXForm.ZxDataSize = reader.ReadUInt16();

                    if (Globals.ExportZXForm.ShowDialog(this) != DialogResult.OK)
                        return;

                    SaveDialogZXAY.FilterIndex = Globals.ExportZXForm.FormatGroup.SelectedIndex + 1;
                    SetDialogZXAYExt();

                    string baseName = !string.IsNullOrEmpty(currentWindow.WinFileName) ? currentWindow.WinFileName :
                                      (currentWindow.TSWindow[0] != null && !string.IsNullOrEmpty(currentWindow.TSWindow[0].WinFileName) ?
                                       currentWindow.TSWindow[0].WinFileName :
                                       $"VTIIModule{currentWindow.WinNumber}");

                    SaveDialogZXAY.FileName = Path.ChangeExtension(baseName, null);

                    int formatIndex;
                    do
                    {
                        if (SaveDialogZXAY.ShowDialog() != DialogResult.OK)
                            return;

                        formatIndex = SaveDialogZXAY.FilterIndex - 1;
                        if (formatIndex < 0 || formatIndex > 4)
                            formatIndex = Globals.ExportZXForm.FormatGroup.SelectedIndex;

                        string[] exts = { ".$c", ".$m", ".ay", ".scl", ".tap" };
                        SaveDialogZXAY.FileName = Path.ChangeExtension(SaveDialogZXAY.FileName, exts[formatIndex]);

                    }
                    while (!AllowSave(SaveDialogZXAY.FileName));

                    SaveDialogZXAY.InitialDirectory = Path.GetDirectoryName(SaveDialogZXAY.FileName);
                    if (SaveDialogZXAY.FilterIndex >= 1 && SaveDialogZXAY.FilterIndex <= 5)
                        Globals.ExportZXForm.FormatGroup.SelectedIndex = SaveDialogZXAY.FilterIndex - 1;

                    int format = Globals.ExportZXForm.FormatGroup.SelectedIndex;

                    if (format != 1 && ExportZXForm.ZXModSize1 + ExportZXForm.ZXModSize2 + ExportZXForm.ZxPlayerSize + ExportZXForm.ZxDataSize > 65536)
                    {
                        MessageBox.Show("Size Of Mmodule With Player Exceeds 65536 Bytes.", "Cannot Export");
                        return;
                    }

                    byte[] pl = reader.ReadBytes(ExportZXForm.ZxPlayerSize);

                    using (BinaryWriter writer = new BinaryWriter(System.IO.File.Open(SaveDialogZXAY.FileName, FileMode.Create)))
                    {
                        int i = ExportZXForm.ZXModSize1;

                        if (format == 0 || format == 1)
                        {
                            i += ExportZXForm.ZXModSize2;
                            if (format == 0)
                                i += ExportZXForm.ZxPlayerSize + ExportZXForm.ZxDataSize;
                            else
                                i += 16;

                            HOBETAHeader hobeta = new HOBETAHeader()
                            {
                                Name = new byte[8],
                                Typ = (byte)(format == 0 ? 'C' : 'm'),
                                Start = (ushort)ExportZXForm.ZXCompAddr,
                                Leng = (ushort)i,
                                SectLeng = (ushort)((i + 255) & 0xFF00)
                            };
                            string fileName = Path.GetFileName(SaveDialogZXAY.FileName);
                            byte[] nameBytes = Encoding.ASCII.GetBytes(fileName);
                            Array.Copy(nameBytes, hobeta.Name, Math.Min(8, nameBytes.Length));

                            if (hobeta.SectLeng == 0)
                            {
                                MessageBox.Show("Size Of Hobeta File Exceeds 255 Sectors.", "Cannot Export");
                                return;
                            }

                            ushort k = 0;
                            for (int j = 0; j < 15; j++)
                                k += hobeta.Ind[j];

                            hobeta.CheckSum = (ushort)(k * 257 + 105);

                            byte[] hobetaBytes = Helpers.CastToArray(hobeta);
                            writer.Write(hobetaBytes);
                        }
                        else if (format == 2)
                        {
                            WriteAYFile(writer, currentWindow);
                        }
                        else if (format == 3)
                        {
                            WriteSCLHeader(writer, currentWindow, pl);
                        }
                        else if (format == 4)
                        {
                            WriteTAPHeader(writer, currentWindow, pl, data);
                            return; // TAP writing includes everything
                        }

                        if (format != 1)
                            writer.Write(pl);

                        int offset1 = Marshal.SizeOf(typeof(PT3));
                        writer.Write(data, offset1, ExportZXForm.ZXModSize1);

                        if (currentWindow.TSWindow[0] != null)
                        {
                            int offset2 = offset1 + ExportZXForm.ZXModSize1;
                            writer.Write(data, offset2, ExportZXForm.ZXModSize2);
                        }
                    }
                }
            }
        }

        private void WriteAYFile(BinaryWriter writer, ChildForm window)
        {
            AYFileHeader header = new AYFileHeader
            {
                FileID = 0x5941585A,
                TypeID = 0x4C554D45,
                FileVersion = 0,
                PlayerVersion = 0,
                PSpecialPlayer = 0,
                PAuthor = (short)Helpers.IntelWord((ushort)(8 + Marshal.SizeOf(typeof(SongStructure)) + Marshal.SizeOf(typeof(SongData)) + Marshal.SizeOf(typeof(Points)))),
                PMisc = (short)Helpers.IntelWord((ushort)(8 + Marshal.SizeOf(typeof(SongStructure)) + Marshal.SizeOf(typeof(SongData)) + Marshal.SizeOf(typeof(Points)) + window.VTM.Author.Length - 1)),
                NumOfSongs = 0,
                FirstSong = 0,
                PSongsStructure = 0x200
            };

            writer.Write(Helpers.CastToArray(header));

            SongStructure structure = new SongStructure
            {
                PSongName = (short)Helpers.IntelWord((ushort)(4 + Marshal.SizeOf(typeof(SongData)) + Marshal.SizeOf(typeof(Points)))),
                PSongData = 0x200
            };

            writer.Write(Helpers.CastToArray(structure));

            SongData songData = new SongData
            {
                ChanA = 0,
                ChanB = 1,
                ChanC = 2,
                Noise = 3,
                FadeLength = 0,
                SongLength = (ushort)Math.Min(window.TotInts, 65535),
                HiReg = 0,
                LoReg = 0,
                PPoints = 0x400,
                PAddresses = 0x800
            };

            if (window.TSWindow[0] != null)
            {
                int regOffset = ExportZXForm.ZXCompAddr + ExportZXForm.ZxPlayerSize + ExportZXForm.ZxDataSize + ExportZXForm.ZXModSize1;
                songData.HiReg = (byte)(regOffset >> 8);
                songData.LoReg = (byte)(regOffset & 0xFF);
            }

            writer.Write(Helpers.CastToArray(songData));

            Points points = new Points
            {
                Stek = Helpers.IntelWord((ushort)ExportZXForm.ZXCompAddr),
                Init = Helpers.IntelWord((ushort)ExportZXForm.ZXCompAddr),
                Inter = Helpers.IntelWord((ushort)(ExportZXForm.ZXCompAddr + 5)),
                Adr1 = Helpers.IntelWord((ushort)ExportZXForm.ZXCompAddr),
                Len1 = Helpers.IntelWord((ushort)ExportZXForm.ZxPlayerSize),
                Offs1 = Helpers.IntelWord((ushort)(10 + window.VTM.Title.Length + window.VTM.Author.Length + FullVersString.Length + 3)),
                Adr2 = Helpers.IntelWord((ushort)(ExportZXForm.ZXCompAddr + ExportZXForm.ZxPlayerSize + ExportZXForm.ZxDataSize)),
                Len2 = Helpers.IntelWord((ushort)(ExportZXForm.ZXModSize1 + ExportZXForm.ZXModSize2)),
                Offs2 = Helpers.IntelWord((ushort)(4 + window.VTM.Title.Length + window.VTM.Author.Length + FullVersString.Length + 3 + ExportZXForm.ZxPlayerSize - 6)),
                Zero = 0
            };

            writer.Write(Helpers.CastToArray(points));

            writer.Write(Encoding.ASCII.GetBytes(window.VTM.Title + "\0"));
            writer.Write(Encoding.ASCII.GetBytes(window.VTM.Author + "\0"));
            writer.Write(Encoding.ASCII.GetBytes(FullVersString + "\0"));
        }

        private void WriteSCLHeader(BinaryWriter writer, ChildForm window, byte[] pl)
        {
            SCLHeader header = new SCLHeader();
            header.SCL = Encoding.ASCII.GetBytes("SINCLAIR");
            header.NBlk = 2;
            header.Name1 = Encoding.ASCII.GetBytes(window.TSWindow[0] != null ? "tsplayer" : "vtplayer");
            header.Typ1 = (byte)'C';
            header.Start1 = (ushort)ExportZXForm.ZXCompAddr;
            header.Leng1 = (ushort)ExportZXForm.ZxPlayerSize;
            header.Sect1 = (byte)((ExportZXForm.ZxPlayerSize + 255) / 256);

            header.Name2 = new byte[8];
            string name2 = Path.GetFileNameWithoutExtension(SaveDialogZXAY.FileName);
            byte[] name2Bytes = Encoding.ASCII.GetBytes(name2);
            Array.Copy(name2Bytes, header.Name2, Math.Min(8, name2Bytes.Length));

            header.Typ2 = (byte)'C';
            header.Start2 = (ushort)(ExportZXForm.ZXCompAddr + ExportZXForm.ZxPlayerSize + ExportZXForm.ZxDataSize);
            header.Leng2 = (ushort)(ExportZXForm.ZXModSize1 + ExportZXForm.ZXModSize2);
            header.Sect2 = (byte)((header.Leng2 + 255) / 256);

            writer.Write(Helpers.CastToArray(header));
        }

        private void WriteTAPHeader(BinaryWriter writer, ChildForm window, byte[] pl, byte[] data)
        {
            TAPHeader hdr = new TAPHeader();
            hdr.Sz = 19;
            hdr.Flag = 0;
            hdr.Typ = 3;
            hdr.Name = Encoding.ASCII.GetBytes(window.TSWindow[0] != null ? "tsplayer  " : "vtplayer  ");
            hdr.Leng = (ushort)ExportZXForm.ZxPlayerSize;
            hdr.Start = (ushort)ExportZXForm.ZXCompAddr;
            hdr.Trash = 32768;

            hdr.Sum = 0;
            for (int i = 2; i < 20; i++)
                hdr.Sum ^= hdr.Ind[i];

            writer.Write(Helpers.CastToArray(hdr));
            writer.Write((ushort)(ExportZXForm.ZxPlayerSize + 2));
            writer.Write((byte)255);
            writer.Write(pl);

            byte chk = 255;
            for (int i = 0; i < ExportZXForm.ZxPlayerSize; i++)
                chk ^= pl[i];
            writer.Write(chk);

            hdr.Name = new byte[10];
            string name = Path.GetFileNameWithoutExtension(SaveDialogZXAY.FileName);
            byte[] nameBytes = Encoding.ASCII.GetBytes(name);
            Array.Copy(nameBytes, hdr.Name, Math.Min(10, nameBytes.Length));
            hdr.Leng = (ushort)(ExportZXForm.ZXModSize1 + ExportZXForm.ZXModSize2);
            hdr.Start = (ushort)(ExportZXForm.ZXCompAddr + ExportZXForm.ZxPlayerSize + ExportZXForm.ZxDataSize);
            hdr.Sum = 0;
            for (int i = 2; i < 20; i++)
                hdr.Sum ^= hdr.Ind[i];

            writer.Write(Helpers.CastToArray(hdr));
            writer.Write((ushort)(hdr.Leng + 2));
            writer.Write((byte)255);

            int offset1 = Marshal.SizeOf(typeof(PT3));
            writer.Write(data, offset1, ExportZXForm.ZXModSize1);
            if (window.TSWindow[0] != null)
            {
                int offset2 = offset1 + ExportZXForm.ZXModSize1;
                writer.Write(data, offset2, ExportZXForm.ZXModSize2);
            }

            chk = 255;
            for (int i = 0; i < ExportZXForm.ZXModSize1; i++) chk ^= data[Marshal.SizeOf(typeof(PT3)) + i];
            if (window.TSWindow[0] != null)
                for (int i = 0; i < ExportZXForm.ZXModSize2; i++) chk ^= data[Marshal.SizeOf(typeof(PT3)) + ExportZXForm.ZXModSize1 + i];
            writer.Write(chk);
        }

        public void SetDialogZXAYExt()
        {
            int i = SaveDialogZXAY.FilterIndex - 1;
            
            if (i < 0 || i > 4)
                i = Globals.ExportZXForm.FormatGroup.SelectedIndex;

            switch (i)
            {
                case 0:
                    SaveDialogZXAY.DefaultExt = "$c";
                    break;
                case 1:
                    SaveDialogZXAY.DefaultExt = "$m";
                    break;
                case 2:
                    SaveDialogZXAY.DefaultExt = "ay";
                    break;
                case 3:
                    SaveDialogZXAY.DefaultExt = "scl";
                    break;
                case 4:
                    SaveDialogZXAY.DefaultExt = "tap";
                    break;
            }
        }

        public void SaveDialogZXAYTypeChange(object sender)
        {
            SetDialogZXAYExt();
        }

        public void SetPriority(ProcessPriorityClass priority)
        {
            Process currentProcess = Process.GetCurrentProcess();
            currentProcess.PriorityClass = priority;
        }

        public void EditCopy_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetEnabled(UIActionType.EditCopy, CanCopy());
        }

        public void EditCut_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetEnabled(UIActionType.EditCut, CanCopy());
        }

        public void EditPaste_Update(object sender, EventArgs e)
        {
            Control control;
            bool enabled = Globals.MainForm.MdiChildren.Length != 0;

            if (!enabled)
            {
                UIActionManager.Instance.SetEnabled(UIActionType.EditPaste, false);
                return;
            }

            if (WaveOutAPI.ExportStarted)
            {
                UIActionManager.Instance.SetEnabled(UIActionType.EditPaste, false);
                return;
            }

            control = Globals.MainForm.ActiveMdiChild.ActiveControl;

            if (control is TextBox)
            {
                enabled = true;
            }
            else if (control is ChildForm)
            {
                ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
                if (activeForm.Tracks == control)
                    enabled = true;
                else if (activeForm.Samples == control)
                    enabled = true;
                else if (activeForm.Ornaments == control)
                    enabled = true;
                else
                    enabled = false;
            }
            else
            {
                enabled = false;
            }

            UIActionManager.Instance.SetEnabled(UIActionType.EditPaste, enabled);
        }

        public void EditCut_Execute(object sender, EventArgs e)
        {
            int controlType = 0;
            Control control = null;

            if (GetCopyControl(ref controlType, ref control))
            {
                switch (controlType)
                {
                    case 0:
                        ((TextBox)control).Cut();
                        break;
                    case 1:
                        ((Tracks)control).CutToClipboard();
                        break;
                    case 3:
                        ((Ornaments)control).CutToClipBoard();
                        break;
                }
            }
        }

        public void EditCopy_Execute(object sender, EventArgs e)
        {
            int controlType = 0;
            Control control = null;

            if (GetCopyControl(ref controlType, ref control))
            {
                switch (controlType)
                {
                    case 0:
                        {
                            TextBox textBox = (TextBox)control;
                            textBox.Copy();
                        }
                        break;
                    case 1:
                        {
                            Tracks tracks = (Tracks)control;
                            tracks.CopyToClipboard();
                            LastClipboard = LastClipboard.Tracks;
                        }
                        break;
                    case 2:
                        {
                            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
                            activeForm.CopySampleToBuffer(false);
                            LastClipboard = LastClipboard.Samples;
                        }
                        break;
                    case 3:
                        {
                            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
                            activeForm.CopyOrnamentToBuffer(false);
                            LastClipboard = LastClipboard.Ornaments;
                        }
                        break;
                }
            }
        }

        public void EditPaste_Execute(object sender, EventArgs e)
        {
            int controlType = 0;
            Control control = null;

            if (GetCopyControl(ref controlType, ref control))
            {
                switch (controlType)
                {
                    case 0:
                        {
                            TextBox textBox = (TextBox)control;
                            textBox.Paste();
                        }
                        break;
                    case 1:
                        {
                            Tracks tracks = (Tracks)control;
                            tracks.PasteFromClipboard(false);
                        }
                        break;
                    case 2:
                        {
                            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
                            activeForm.PasteSampleFromBuffer(false);
                        }
                        break;
                    case 3:
                        {
                            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
                            activeForm.PasteOrnamentFromBuffer();
                        }
                        break;
                }
            }
        }

        public void Undo_Update(object sender, EventArgs e)
        {
            if (WaveOutAPI.ExportStarted)
            {
                UIActionManager.Instance.SetEnabled(UIActionType.Undo, false);
                return;
            }

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            if (activeForm == null || activeForm.IsClosed || activeForm.ChangeList == null)
            {
                UIActionManager.Instance.SetEnabled(UIActionType.Undo, false);
                return;
            }

            UIActionManager.Instance.SetEnabled(UIActionType.Undo, this.MdiChildren.Length != 0 && (activeForm.ChangeCount > 0) || (activeForm.TabControl.SelectedIndex == 4));
        }

        public void Undo_Execute(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            if (activeForm.TabControl.SelectedIndex == 4 && activeForm.TrackInfoRTB.Focused)
            {
                activeForm.TrackInfoRTB.Undo();
                return;
            }

            activeForm.DoUndo(1, true);
        }

        public void Redo_Update(object sender, EventArgs e)
        {
            if (WaveOutAPI.ExportStarted)
            {
                UIActionManager.Instance.SetEnabled(UIActionType.Redo, false);
                return;
            }

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            UIActionManager.Instance.SetEnabled(UIActionType.Redo, this.MdiChildren.Length != 0 && (activeForm.ChangeCount < activeForm.ChangeTop) || (activeForm.TabControl.SelectedIndex == 4));
        }

        public void Redo_Execute(object sender, EventArgs e)
        {
            const uint EM_REDO = WM_USER + 84;

            if (this.MdiChildren.Length == 0)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            if (activeForm.TabControl.SelectedIndex == 4 && activeForm.TrackInfoRTB.Focused)
            {
                SendMessage(activeForm.TrackInfoRTB.Handle, EM_REDO, 0, 0);
                return;
            }

            activeForm.DoUndo(1, false);
        }

        public void CheckCommandLine()
        {
            string fileExt;
            string[] cmdLine = Environment.GetCommandLineArgs();
            int i = Environment.GetCommandLineArgs().Length;

            if (i == 1)
                return;

            fileExt = Path.GetExtension(cmdLine[1]);

            if (i == 2)
            {
                StartupOpenModule = Array.IndexOf(ModuleExtensions, fileExt) != -1;
                StartupOpenTheme = fileExt == ".vtt";
                return;
            }

            StartupOpenTheme = false;
            StartupOpenModule = true;

            for (; i >= 1; i--)
                CreateMDIChild(Path.GetFullPath(cmdLine[i]), 1);
        }

        public bool AllowSave(string fileName)
        {
            return !System.IO.File.Exists(fileName) || (MessageBox.Show(this, $"File \"{fileName}\" Exists. Overwrite?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes);
        }

        public void TransposeChannel(ChildForm childForm, int pat, int chn, int i, int semitones)
        {
            int j;

            if (childForm.VTM.Patterns[pat].Lines[i].Channel[chn].Note >= 0)
            {
                j = childForm.VTM.Patterns[pat].Lines[i].Channel[chn].Note + semitones;

                if (j >= 96 || j < 0)
                    return;

                childForm.VTM.Patterns[pat].Lines[i].Channel[chn].Note = (sbyte)j;
            }
        }

        public void TransposeColumns(ChildForm childForm, int patternIndex, bool isEnvelope, bool[] channelTrans, int fromLine, int toLine, int semiTones, bool makeUndo)
        {
            double envelopeFreqScale;
            int scaledEnvelope;
            int oldEnvelope;
            int envelopeNote;
            bool needsTranspose;
            Pattern oldPattern = null;

            if (semiTones == 0)
                return;

            if (childForm.VTM.Patterns[patternIndex] == null)
                return;

            needsTranspose = isEnvelope || channelTrans[0] || channelTrans[1] || channelTrans[2];

            if (!needsTranspose)
                return;

            // Work with all pattern lines even if it greater then pattern length
            if (toLine >= VTModule.MaxPatternLength)
                toLine = VTModule.MaxPatternLength - 1;

            if (fromLine > toLine)
                return;

            childForm.SongChanged = true;
            childForm.BackupSongChanged = true;

            if (makeUndo)
                oldPattern = (Pattern)childForm.VTM.Patterns[patternIndex].Clone();

            if (channelTrans[0])
            {
                for (int i = fromLine; i <= toLine; i++)
                    TransposeChannel(childForm, patternIndex, 0, i, semiTones);
            }

            if (channelTrans[1])
            {
                for (int i = fromLine; i <= toLine; i++)
                    TransposeChannel(childForm, patternIndex, 1, i, semiTones);
            }

            if (channelTrans[2])
            {
                for (int i = fromLine; i <= toLine; i++)
                    TransposeChannel(childForm, patternIndex, 2, i, semiTones);
            }

            if (isEnvelope)
            {
                envelopeFreqScale = Math.Exp(-semiTones / 12 * Math.Log(2.0));
                for (int i = fromLine; i <= toLine; i++)
                {
                    oldEnvelope = childForm.VTM.Patterns[patternIndex].Lines[i].Envelope;

                    if (oldEnvelope == 0)
                        continue;

                    envelopeNote = VTModule.GetNoteByEnvelope(oldEnvelope);
                    scaledEnvelope = envelopeNote > 0 ? (int)Math.Round(VTModule.GetNoteFreq(childForm.VTM.NoteTable, envelopeNote + semiTones) / 16.0) : (int)Math.Round(oldEnvelope * envelopeFreqScale);

                    if (scaledEnvelope >= 0 && scaledEnvelope < 0x10000)
                        childForm.VTM.Patterns[patternIndex].Lines[i].Envelope = (ushort)scaledEnvelope;
                }
            }

            if (makeUndo)
            {
                childForm.AddUndo(TChangeAction.TransposePattern, patternIndex, 0);
                childForm.ChangeList[childForm.ChangeCount - 1].Pattern = oldPattern;
            }

            if (childForm.PatternIndex == patternIndex)
            {
                if (childForm.Tracks.Focused)
                    childForm.Tracks.HideCaret();

                childForm.Tracks.RedrawTracks();

                if (childForm.Tracks.Focused)
                    childForm.Tracks.ShowCaret();
            }
        }

        public void TransposeSelection(int semiTones)
        {
            int fromX, toX, fromY, toY, i;
            int evenVol, tempVol, volChan;

            // channel for volume transposition
            bool[] channelTrans = new bool[3];

            if (semiTones == 0)
                return;

            if (this.MdiChildren.Length == 0)
                return;

            volChan = 0;
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            Tracks tracks = activeForm.Tracks;
            fromX = tracks.SelectionX;
            toX = tracks.CursorX;

            if (fromX > toX)
            {
                fromX = toX;
                toX = tracks.SelectionX;
            }

            fromY = tracks.SelectionY;
            toY = tracks.ShownFrom - tracks.CenterLineIndex + tracks.CursorY;

            if (fromY > toY)
            {
                fromY = toY;
                toY = tracks.SelectionY;
            }

            // Volume
            if (toX == fromX && (fromX == 15 || fromX == 29 || fromX == 43))
            {
                activeForm.SavePatternUndo();

                switch (fromX)
                {
                    case 15:
                        volChan = 0;
                        break;
                    case 29:
                        volChan = 1;
                        break;
                    case 43:
                        volChan = 2;
                        break;
                }

                evenVol = 0;

                for (i = fromY; i <= toY; i++)
                {
                    tempVol = activeForm.VTM.Patterns[activeForm.PatternIndex].Lines[i].Channel[volChan].Volume;

                    if (Math.Abs(semiTones) == 1 && (tempVol + semiTones >= 1 && tempVol + semiTones <= 15) && tempVol != 0)
                        activeForm.VTM.Patterns[activeForm.PatternIndex].Lines[i].Channel[volChan].Volume = (sbyte)(tempVol + semiTones);
                    else if (Math.Abs(semiTones) == 12)
                    {
                        if (tempVol != 0)
                            evenVol = evenVol + 1;

                        if (tempVol != 0 && (evenVol % 2) == 0)
                        {
                            if (semiTones > 0 && (tempVol + 1 >= 1 && tempVol + 1 <= 15))
                                activeForm.VTM.Patterns[activeForm.PatternIndex].Lines[i].Channel[volChan].Volume = (sbyte)(tempVol + 1);

                            if (semiTones < 0 && (tempVol - 1 >= 1 && tempVol - 1 <= 15))
                                activeForm.VTM.Patterns[activeForm.PatternIndex].Lines[i].Channel[volChan].Volume = (sbyte)(tempVol - 1);
                        }
                    }
                }

                activeForm = (ChildForm)this.ActiveMdiChild;
                activeForm.Tracks.HideCaret();
                activeForm.Tracks.RedrawTracks();
                activeForm.Tracks.ShowCaret();
                activeForm.SavePatternRedo();
            }
            // Noise
            else if (fromX == 6 || toX == 6)
            {
                activeForm.SavePatternUndo();

                for (i = fromY; i <= toY; i++)
                {
                    if (semiTones > 0 && activeForm.VTM.Patterns[activeForm.PatternIndex].Lines[i].Noise < 31 && activeForm.VTM.Patterns[activeForm.PatternIndex].Lines[i].Noise > 0)
                        activeForm.VTM.Patterns[activeForm.PatternIndex].Lines[i].Noise++;

                    if (semiTones < 0 && activeForm.VTM.Patterns[activeForm.PatternIndex].Lines[i].Noise > 1)
                        activeForm.VTM.Patterns[activeForm.PatternIndex].Lines[i].Noise--;
                }

                activeForm.Tracks.HideCaret();
                activeForm.Tracks.RedrawTracks();
                activeForm.Tracks.ShowCaret();
                activeForm.SavePatternRedo();
            }
            else
            {
                channelTrans[ChanAlloc[0]] = (fromX <= 8) && (toX >= 8);
                channelTrans[ChanAlloc[1]] = (fromX <= 22) && (toX >= 22);
                channelTrans[ChanAlloc[2]] = (fromX <= 36) && (toX >= 36);

                TransposeColumns(activeForm, activeForm.PatternIndex, fromX <= 3, channelTrans, fromY, toY, semiTones, true);
            }
        }

        public void TransposeUp1_Update(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            UIActionManager.Instance.SetEnabled(UIActionType.TransposeUp1, this.MdiChildren.Length != 0 && activeForm.Tracks.Focused);
        }

        public void TransposeDown1_Update(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            UIActionManager.Instance.SetEnabled(UIActionType.TransposeDown1, this.MdiChildren.Length != 0 && activeForm.Tracks.Focused);
        }

        public void TransposeUp12_Update(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            UIActionManager.Instance.SetEnabled(UIActionType.TransposeUp12, this.MdiChildren.Length != 0 && activeForm.Tracks.Focused);
        }

        public void TransposeDown12_Update(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            UIActionManager.Instance.SetEnabled(UIActionType.TransposeDown12, this.MdiChildren.Length != 0 && activeForm.Tracks.Focused);
        }

        public void TransposeUp1_Execute(object sender, EventArgs e)
        {
            TransposeSelection(1);
        }

        public void TransposeDown1_Execute(object sender, EventArgs e)
        {
            TransposeSelection(-1);
        }

        public void TransposeUp12_Execute(object sender, EventArgs e)
        {
            TransposeSelection(12);
        }

        public void TransposeDown12_Execute(object sender, EventArgs e)
        {
            TransposeSelection(-12);
        }

        // Specially for Znahar
        public void SetBar(int barNum, bool value)
        {
            ((ToolStripMenuItem)PopupMenu3.Items[barNum]).Checked = value;
            switch (barNum)
            {
                case 0:
                    NewButton.Visible = value;
                    FileOpenButton.Visible = value;
                    FileSaveButton.Visible = value;
                    break;
                case 1:
                    // ToolButton3.Visible := Value;
                    PlayStopButton.Visible = value;
                    PlayFromStartButton.Visible = value;
                    PlayPatternFromCurrentLineButton.Visible = value;
                    PlayPatternFromStartButton.Visible = value;
                    ToggleLoopingButton.Visible = value;
                    LoopAllFilesButton.Visible = value;
                    // ToolButton16.Visible := Value;
                    ToggleSamplesButton.Visible = value;
                    break;
                case 2:
                    TracksManagerButton.Visible = value;
                    GlobalTranspositionButton.Visible = value;
                    break;
                    // ToolButton28.Visible := Value;
                    // 6:
                    // begin
                    // SpeedButton1.Visible := Value;
                    // SpeedButton2.Visible := Value;
                    // ToolButton19.Visible := Value;
                    // end;
                    // 7:
                    // begin
                    // TrackBar1.Visible := Value;
                    // ToolButton25.Visible := Value;
                    // end;
                    // 8:
                    // ComboBox1.Visible := Value;
            }
        }

        public void TrackTools_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            SetBar((int)menuItem.Tag, !menuItem.Checked);
        }

        public void ExpandPattern_Execute(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.ExpandPattern();
        }

        public void CompressPattern_Execute(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.CompressPattern();
        }

        public void Merge1Click(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.Tracks.PasteFromClipboard(true);
        }

        public void CreateMidiInputClient(uint devicePort = 0)
        {
           var devices = MidiManager.GetAvailableDevices();

            if (devices.Count > 0)
            {
                try
                {
                    MidiInputDeviceInfo = MidiManager.GetDeviceInfo(devicePort, MidiDeviceType.Input);

                    // Create and open the MIDI input client
                    MidiInputClient = new MidiInputClient(MidiInputDeviceInfo);
                    MidiInputClient.OnMessageReceived += MidiInputClient_OnMessageReceived;
                    MidiInputClient.ActivateMessageReceivedEvent();
                    MidiInputClient.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"MIDI Input Error: {ex.Message}", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public void DestroyMidiInputClient()
        {
            MidiInputClient?.Close();
            MidiInputClient = null;
            MidiInputDeviceInfo = null;
        }

        public static bool TryConvertMidiMessage(int device, RtMidi.Net.MidiMessage raw, TimeSpan timestamp, out LibVT.Plugins.MidiMessage? midiMessage)
        {
            midiMessage = raw switch
            {
                MidiMessageNote { Type: RtMidi.Net.Enums.MidiMessageType.NoteOn } m => new LibVT.Plugins.MidiMessage(LibVT.Plugins.MidiMessageType.NoteOn, (int)m.Channel, m.Note.GetByteRepresentation(), m.Velocity, timestamp, device),
                MidiMessageNote { Type: RtMidi.Net.Enums.MidiMessageType.NoteOff } m => new LibVT.Plugins.MidiMessage(LibVT.Plugins.MidiMessageType.NoteOff, (int)m.Channel, m.Note.GetByteRepresentation(), m.Velocity, timestamp, device),
                MidiMessageControlChange m => new LibVT.Plugins.MidiMessage(LibVT.Plugins.MidiMessageType.ControlChange, (int)m.Channel, m.ControlFunction, m.Value, timestamp, device),
                MidiMessageProgramChange m => new LibVT.Plugins.MidiMessage(LibVT.Plugins.MidiMessageType.ProgramChange, (int)m.Channel, m.Program, 0, timestamp, device),
                MidiMessagePitchBendChange m => new LibVT.Plugins.MidiMessage(LibVT.Plugins.MidiMessageType.PitchBendChange, (int)m.Chanel, m.Lsb, m.Msb, timestamp, device),
                MidiMessageChannelAfterTouch m => new LibVT.Plugins.MidiMessage(LibVT.Plugins.MidiMessageType.ChannelPressure, (int)m.Channel, m.Pressure, 0, timestamp, device),
                MidiMessageSystemExclusive m => new LibVT.Plugins.MidiMessage(LibVT.Plugins.MidiMessageType.SystemExclusive, 0, m.Data.FirstOrDefault(), m.Data.Skip(1).FirstOrDefault(), timestamp, device),
                _ => null
            };

            return false;
        }

        private void MidiInputClient_OnMessageReceived(object sender, MidiMessageReceivedEventArgs e)
        {
            if (TryConvertMidiMessage((int)MidiInputDeviceInfo.Port, e.Message, e.Timestamp, out LibVT.Plugins.MidiMessage? midiMessage))
            {
                MidiMessageEventArgs midiMessageEventArgs = new LibVT.Plugins.MidiMessageEventArgs(midiMessage.Value);

                VortexTracker.PluginManager.RaiseMidiMessageEvent(this, midiMessageEventArgs);

                if (midiMessageEventArgs.Handled)
                    return;
            }

            if (e.Message is MidiMessageNote { Type: RtMidi.Net.Enums.MidiMessageType.NoteOn or RtMidi.Net.Enums.MidiMessageType.NoteOff } message)
            {
                const int offset = 21;
                var note = message.Note.GetByteRepresentation() - offset;

                // Determine the active window and handle the note accordingly
                var activeForm = (ChildForm)this.ActiveMdiChild;

                if (activeForm == null)
                    return;

                // NOTE ON
                if (message.Type == RtMidi.Net.Enums.MidiMessageType.NoteOn)
                {
                    if (activeForm.Tracks.Focused)
                        activeForm.TracksMidiNoteOn((short)note);
                    else if (activeForm.SampleTestLine.Focused)
                        activeForm.SampleTestLine.TestLineMidiOn(note);
                    else if (activeForm.TabControl.SelectedTab == activeForm.SamplesTab)
                        activeForm.SamplesMidiNoteOn((byte)note);
                    else if (activeForm.TabControl.SelectedTab == activeForm.OrnamentsTab)
                        activeForm.OrnamentsMidiNoteOn((byte)note);
                }
                // NOTE OFF
                else if (message.Type == RtMidi.Net.Enums.MidiMessageType.NoteOff)
                {
                    if (activeForm.Tracks.Focused)
                        activeForm.TracksMidiNoteOff((short)note);
                    else if (activeForm.SampleTestLine.Focused)
                        activeForm.SampleTestLine.TestLineMidiOff(note);
                    else if (activeForm.TabControl.SelectedTab == activeForm.SamplesTab)
                        activeForm.SamplesMidiNoteOff((byte)note);
                    else if (activeForm.TabControl.SelectedTab == activeForm.OrnamentsTab)
                        activeForm.OrnamentsMidiNoteOff((byte)note);
                }
            }
        }

        public void RenumberPatterns_Click(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
                return;

            if (WaveOutAPI.IsPlaying && AY.PlayMode == PlayModes.PlayModule)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            activeForm.RenumberPatterns();
        }

        public void UpdateEnvelopeAsNote()
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[i];

                if (childForm != activeForm)
                {
                    childForm.EnvelopeAsNoteCheckBox.Checked = EnvelopeAsNote;
                    childForm.Tracks.RedrawTracks();
                }
            }
        }

        public void ChangeDupNoteParams()
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[i];

                if (childForm != activeForm)
                    childForm.UseLastNoteParamsCheckBox.Checked = DupNoteParams;
            }
        }

        public void DuplicateLastNoteParams_Execute(object sender, EventArgs e)
        {
            DupNoteParams = !DupNoteParams;

            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[i];
                childForm.UseLastNoteParamsCheckBox.Checked = DupNoteParams;
            }
        }

        public void ChangeBetweenPatterns()
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[i];

                if (childForm != activeForm)
                    childForm.MoveBetweenPatternsCheckBox.Checked = MoveBetweenPatrns;
            }
        }

        public void MoveBetweenPatterns_Execute(object sender, EventArgs e)
        {
            MoveBetweenPatrns = !MoveBetweenPatrns;

            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[i];

                childForm.MoveBetweenPatternsCheckBox.Checked = MoveBetweenPatrns;
            }
        }

        public void AutoNumeratePatternsClick(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            activeForm.AutoNumeratePatterns();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_SYSCOMMAND:
                    int cmd = m.WParam.ToInt32();
                    SysCmd = cmd & 0xFFF0;
                    PrevTop = this.Top;
                    MaximizeChilds = (cmd != SC_SIZE_SIDELEFT) && (cmd != SC_SIZE_SIDERIGHT);

                    if (SysCmd == SC_SIZE && OneTrack())
                        FixWidth();

                    else
                        ResetConstraints(true);

                    if (OneTrack() && (cmd == SC_SIZE_SIDELEFT || cmd == SC_SIZE_TOPLEFT || cmd == SC_SIZE_BOTTOMLEFT))
                        return; // skip further handling

                    if (SysCmd == SC_MAXIMIZE || SysCmd == SC_RESTORE)
                        RedrawOff();

                    // base.WMSysCommand();
                    RedrawOn();
                    MaximizeChilds = true;
                    WindowUnsnap = false;
                    SizeFixed = false;
                    MoveShift = 0;
                    break;
                case WM_WINDOWPOSCHANGING:
                    WINDOWPOS windowPos = Marshal.PtrToStructure<WINDOWPOS>(m.LParam);

                    if (Snaped && !WindowSnap && (windowPos.x != this.Left || windowPos.y != this.Top))
                    {
                        Snaped = false;
                        SnappedToRight = false;
                    }

                    if (MoveShift != 0 && (windowPos.flags & SWP_NOMOVE) == 0)
                    {
                        windowPos.x += MoveShift;
                        Marshal.StructureToPtr(windowPos, m.LParam, true);
                        m.Result = IntPtr.Zero;
                        return;
                    }

                    if (SizeFixed && (windowPos.flags & SWP_NOSIZE) == 0)
                    {
                        windowPos.cx = WidthFix;
                        windowPos.cy = HeightFix;
                        Marshal.StructureToPtr(windowPos, m.LParam, true);
                        m.Result = IntPtr.Zero;
                        return;
                    }

                    break;
            }

            base.WndProc(ref m);
        }

        // procedure WMSize(var Msg: TWMSize); message WM_SIZE;
        // procedure WMSizing(var Msg: TWMSize); message WM_SIZING;
        /* private void WMPosChanging(ref Message m)
        {
            if (m.Msg == WM_WINDOWPOSCHANGING)
            {
                WINDOWPOS pos = Marshal.PtrToStructure<WINDOWPOS>(m.LParam);

                if (Snaped && !WindowSnap && (pos.x != this.Left || pos.y != this.Top))
                {
                    Snaped = false;
                    SnappedToRight = false;
                }

                if (MoveShift != 0 && (pos.flags & SWP_NOMOVE) == 0)
                {
                    pos.x += MoveShift;
                    Marshal.StructureToPtr(pos, m.LParam, true);
                    m.Result = IntPtr.Zero;
                    return;
                }

                if (SizeFixed && (pos.flags & SWP_NOSIZE) == 0)
                {
                    pos.cx = WidthFix;
                    pos.cy = HeightFix;
                    Marshal.StructureToPtr(pos, m.LParam, true);
                    m.Result = IntPtr.Zero;
                    return;
                }
            }
        } */

        private void OnDisplaySettingsChanged(object sender, EventArgs e)
        {
            Rectangle newSize;

            // Update constants
            DisplayChanged = true;
            MonitorWorkAreaWidth();
            MonitorWorkAreaHeight();
            DisplayChanged = false;

            if (this.WindowState == FormWindowState.Normal)
            {
                Normal_Execute(this, EventArgs.Empty);
                return;
            }

            newSize = new Rectangle(this.Left, this.Top, this.ClientSize.Width, this.ClientSize.Height);

            RedrawOff();
            SetChildsPosition(FormWindowState.Maximized);
            AutoMetricsForChilds(FormWindowState.Maximized);
            AutoCutChilds(newSize);
            RedrawChilds();
            AutoToolBarPosition(newSize);
            RedrawOn();
        }

        // procedure WMWindowPosChanged(var Msg: TWMWindowPosChanged); message WM_WINDOWPOSCHANGED;
        /* private void WMDropFiles(ref Message m)
        {
            IntPtr dropH;
            // drop handle
            int DroppedFileCount;
            // number of files dropped
            int FileNameLength;
            // length of a dropped file name
            string FileName;
            string FileExt;
            // a dropped file name
            int I;
            // loops thru all dropped files
            POINT DropPoint;
            // point where files dropped
            // base.WMDropFiles();
            // Store drop handle from the message
            dropH = m.WParam;
            try
            {
                // Get count of files dropped
                DroppedFileCount = (int)DragQueryFile(dropH, 0xFFFFFFFF, null, 0);
                // Get name of each file dropped and process it
                for (I = 0; I < DroppedFileCount; I++)
                {
                    // get length of file name
                    FileNameLength = (int)DragQueryFile(dropH, (uint)I, null, 0);
                    // create string large enough to store file
                    // (Delphi allows for #0 terminating character automatically)
                    //FileName.Length = FileNameLength;
                    // get the file name
                    StringBuilder fileNameBuffer = new StringBuilder(MAX_PATH);
                    DragQueryFile(dropH, (uint)I, fileNameBuffer, MAX_PATH);
                    // process file name
                    FileName = fileNameBuffer.ToString();

                    FileExt = Path.GetExtension(FileName);
                    if (FileExt == ".vtt")
                    {
                        if (ColorThemes.LoadColorTheme(FileName))
                        {
                            MessageBox.Show(this, "Done. Color theme successfully imported!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show(this, "Invalid color theme file", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        }
                        return;
                    }
                    if (Array.IndexOf(ModuleExtensions, FileExt) == -1)
                    {
                        return;
                    }
                    ChildsEventsBlocked = true;
                    RedrawOff();
                    CreateMDIChild(FileName, false);
                    RedrawOn();
                    ChildsEventsBlocked = false;
                }
                // Optional: Get point at which files were dropped
                DragQueryPoint(dropH, out DropPoint);
                // ... do something with drop point here
            }
            finally
            {
                // Tidy up - release the drop handle
                // don't use DropH again after this
                DragFinish(dropH);
            }
            // Note we handled message
            m.Result = IntPtr.Zero;
        } */

        public void SaveBackups(object sender, EventArgs e)
        {
            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[i];

                if (childForm.BackupSongChanged)
                    childForm.SaveModuleBackup();
            }
        }

        public void ChangeBackupTimer()
        {
            if (AutoBackupsOn)
            {
                BackupTimer.Enabled = true;
                BackupTimer.Interval = AutoBackupsMins * 60000;
            }
            else
            {
                BackupTimer.Enabled = false;
            }
        }

        public void SetPositionColor(byte numColor)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            for (int i = ChildForm.PatternsOrderSelection.Left; i <= ChildForm.PatternsOrderSelection.Right; i++)
                activeForm.VTM.Positions.Colors[i] = numColor;

            activeForm.SongChanged = true;
            activeForm.BackupSongChanged = true;
            activeForm.UnselectPositions();
        }

        public void PositionColorDefault_Click(object sender, EventArgs e)
        {
            SetPositionColor(0);
        }

        public void PositionColorRed_Click(object sender, EventArgs e)
        {
            SetPositionColor(1);
        }

        public void PositionColorGreen_Click(object sender, EventArgs e)
        {
            SetPositionColor(2);
        }

        public void PositionColorBlue_Click(object sender, EventArgs e)
        {
            SetPositionColor(3);
        }

        public void PositionColorMaroon_Click(object sender, EventArgs e)
        {
            SetPositionColor(4);
        }

        public void PositionColorPurple_Click(object sender, EventArgs e)
        {
            SetPositionColor(5);
        }

        public void PositionColorGray_Click(object sender, EventArgs e)
        {
            SetPositionColor(6);
        }

        public void PositionColorTeal_Click(object sender, EventArgs e)
        {
            SetPositionColor(7);
        }

        public void PositionColorBlackClick(object sender, EventArgs e)
        {
            SetPositionColor(8);
        }

        public void PositionColorL1_Click(object sender, EventArgs e)
        {
            SetPositionColor(9);
        }

        public void PositionColorL2_Click(object sender, EventArgs e)
        {
            SetPositionColor(10);
        }

        public void PositionColorL3_Click(object sender, EventArgs e)
        {
            SetPositionColor(11);
        }

        public void PositionColorL4_Click(object sender, EventArgs e)
        {
            SetPositionColor(12);
        }

        public void PositionColorL5_Click(object sender, EventArgs e)
        {
            SetPositionColor(13);
        }

        public void PositionColorRed_Paint(object sender, PaintEventArgs e)
        {
            DrawSubmenuColor(sender, e, ColorThemes.StringToColor(ColorThemes.GridColors[(int)GridColor.Red]));
        }

        public void PositionColorGreen_Paint(object sender, PaintEventArgs e)
        {
            DrawSubmenuColor(sender, e, ColorThemes.StringToColor(ColorThemes.GridColors[(int)GridColor.Green]));
        }

        public void PositionColorBlue_Paint(object sender, PaintEventArgs e)
        {
            DrawSubmenuColor(sender, e, ColorThemes.StringToColor(ColorThemes.GridColors[(int)GridColor.Blue]));
        }

        public void PositionColorMaroon_Paint(object sender, PaintEventArgs e)
        {
            DrawSubmenuColor(sender, e, ColorThemes.StringToColor(ColorThemes.GridColors[(int)GridColor.Maroon]));
        }

        public void PositionColorPurple_Paint(object sender, PaintEventArgs e)
        {
            DrawSubmenuColor(sender, e, ColorThemes.StringToColor(ColorThemes.GridColors[(int)GridColor.Purple]));
        }

        public void PositionColorGray_Paint(object sender, PaintEventArgs e)
        {
            DrawSubmenuColor(sender, e, ColorThemes.StringToColor(ColorThemes.GridColors[(int)GridColor.Gray]));
        }

        public void PositionColorTeal_Paint(object sender, PaintEventArgs e)
        {
            DrawSubmenuColor(sender, e, ColorThemes.StringToColor(ColorThemes.GridColors[(int)GridColor.Teal]));
        }

        public void PositionColorDefault_Paint(object sender, PaintEventArgs e)
        {
            DrawSubmenuColor(sender, e, ColorThemes.StringToColor(ColorThemes.GridColors[(int)GridColor.White]));
        }

        public void PositionColorL1_Paint(object sender, PaintEventArgs e)
        {
            DrawSubmenuColor(sender, e, ColorThemes.StringToColor(ColorThemes.GridColors[(int)GridColor.Light1]));
        }

        public void PositionColorL2_Paint(object sender, PaintEventArgs e)
        {
            DrawSubmenuColor(sender, e, ColorThemes.StringToColor(ColorThemes.GridColors[(int)GridColor.Light2]));
        }

        public void PositionColorL3_Paint(object sender, PaintEventArgs e)
        {
            DrawSubmenuColor(sender, e, ColorThemes.StringToColor(ColorThemes.GridColors[(int)GridColor.Light3]));
        }

        public void PositionColorL4_Paint(object sender, PaintEventArgs e)
        {
            DrawSubmenuColor(sender, e, ColorThemes.StringToColor(ColorThemes.GridColors[(int)GridColor.Light4]));
        }

        public void PositionColorL5_Paint(object sender, PaintEventArgs e)
        {
            DrawSubmenuColor(sender, e, ColorThemes.StringToColor(ColorThemes.GridColors[(int)GridColor.Light5]));
        }

        public void PopupMenu1Popup(object sender, EventArgs e)
        {
            int numSelected;
            bool canChangePositions;
            bool canChangeColors;
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            canChangeColors = (this.MdiChildren.Length != 0) && activeForm.PositionsGrid.Focused && (activeForm.VTM.Positions.Length > activeForm.PositionsGrid.Selection.Right);
            canChangePositions = canChangeColors && !(WaveOutAPI.IsPlaying && (AY.PlayMode == PlayModes.PlayModule));
            numSelected = activeForm.PositionsGrid.Selection.Right - activeForm.PositionsGrid.Selection.Left + 1;

            Color1.Enabled = canChangeColors;
            ResetColors.Enabled = canChangeColors;

            InsertPosition.Enabled = canChangePositions;
            DeletePosition.Enabled = canChangePositions;
            DuplicatePosition.Enabled = canChangePositions;
            ClonePosition.Enabled = canChangePositions;

            if (numSelected > 1)
            {
                RenumberPatterns.Visible = true;
                AutoNumeratePatterns.Visible = true;
                DuplicatePosition.Text = "Duplicate positions";
                DeletePosition.Text = "Delete positions";
                ClonePosition.Text = "Clone positions";
            }
            else
            {
                RenumberPatterns.Visible = false;
                AutoNumeratePatterns.Visible = false;
                DuplicatePosition.Text = "Duplicate position";
                DeletePosition.Text = "Delete position";
                ClonePosition.Text = "Clone position";
            }
        }

        public void DrawSubmenuColor(object sender, PaintEventArgs e, Color color)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            Graphics g = e.Graphics;
            Rectangle rect = menuItem.ContentRectangle;
            bool selected = menuItem.Selected;

            using (SolidBrush bgBrush = new SolidBrush(color))
                g.FillRectangle(bgBrush, rect);

            if (selected)
            {
                using (Pen borderPen = new Pen(SystemColors.MenuHighlight))
                    g.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }
            else
            {
                using (Pen borderPen = new Pen(SystemColors.ButtonFace))
                    g.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);
            }
        }

        public void SetLoopPosition1MeasureItem(object sender, MeasureItemEventArgs e)
        {
            Width = 155;
        }

        public void PrepareColors()
        {
            for (int i = 0; i < (int)ThemeColor.Count; i++)
                ThemeColors[i] = ColorTheme.Colors[i];

            Controls.OfType<MdiClient>().FirstOrDefault().BackColor = MainForm.ColorTheme.Colors[(int)ThemeColor.FullScreenBackground];
        }

        public void ResetColors_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Delete All Colors?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            for (int i = activeForm.VTM.Positions.Colors.GetLowerBound(0); i <= activeForm.VTM.Positions.Colors.GetUpperBound(0); i++)
                activeForm.VTM.Positions.Colors[i] = 0;

            activeForm.PositionsGrid.Invalidate();
            activeForm.PositionsGrid.Update();
        }

        public void SendSyncMessage()
        {
            using (FileStream fileStream = new FileStream(SyncMessageFile, FileMode.OpenOrCreate))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    streamWriter.WriteLine(this.Handle.ToString());
            }
        }

        public void SyncCheckTimer_Tick(object sender, EventArgs e)
        {
            return;

            if (!System.IO.File.Exists(SyncMessageFile))
                return;

            SyncCheckTimer.Enabled = false;
            SyncFinishTimer.Enabled = true;

            string res;

            using (FileStream fileStream = new FileStream(SyncMessageFile, FileMode.Open))
            {
                using (StreamReader streamReader = new StreamReader(fileStream))
                    res = streamReader.ReadLine();
            }

            if (res == this.Handle.ToString())
                return;

            SyncVTInstances = true;
            EditorFontChanged = true;
            NumberOfLinesChanged = true;
            ReadConfig();
            RedrawOff();
            ChildsEventsBlocked = true;
            RedrawChilds();
            AutoMetricsForChilds(this.WindowState);
            SetChildsPosition(this.WindowState);
            Rectangle newSize = GetSizeForChilds(this.WindowState, false);
            AutoCutChilds(newSize);
            AutoToolBarPosition(newSize);
            SetWindowSize(newSize);
            ChildsEventsBlocked = false;
            RedrawOn();
            SyncVTInstances = false;
            EditorFontChanged = false;
            NumberOfLinesChanged = false;
            SyncFinishTimer.Enabled = true;
        }

        public void SyncFinishTimerTimer(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(SyncMessageFile))
            {
                try
                {
                    System.IO.File.Delete(SyncMessageFile);
                }
                catch
                {
                }
            }
            SyncFinishTimer.Enabled = false;
            SyncCheckTimer.Enabled = true;
        }

        public void SyncCopyBuffersTimer(object sender, EventArgs e)
        {
            string s;
            Sample sample;
            Ornament ornament;
            long sampleBufferAge;
            long ornamenBufferAge;
            long samplePartBufferAge;
            bool sampleBufferReady;
            bool ornamentBufferReady;
            bool samplePartBufferReady;

            if (SyncBufferBlocked)
                return;

            if (this.MdiChildren.Length == 0)
                return;

            sampleBufferAge = Globals.FileAge(SyncSampleBufferFile);
            ornamenBufferAge = Globals.FileAge(SyncOrnamentBufferFile);
            samplePartBufferAge = Globals.FileAge(SyncSamplePartFile);
            sampleBufferReady = (sampleBufferAge != -1) && (sampleBufferAge != SyncSampleBufferFileAge);
            ornamentBufferReady = (ornamenBufferAge != -1) && (ornamenBufferAge != SyncOrnamentBufferFileAge);
            samplePartBufferReady = (samplePartBufferAge != -1) && (samplePartBufferAge != SyncSamplePartFileAge);

            if (!sampleBufferReady && !ornamentBufferReady && !samplePartBufferReady)
                return;

            // Sync sample copy/paste buffer
            if (sampleBufferReady)
            {
                SyncSampleBufferFileAge = sampleBufferAge;
                string[] sampleLines = System.IO.File.ReadAllLines(SyncSampleBufferFile);
                if (Sample.LoadSampleDataTxt(sampleLines, out sample, false) != 0)
                    return;
                Main.BuffSample.Length = sample.Length;
                Main.BuffSample.Loop = sample.Loop;
                Main.BuffSample.Enabled = sample.Enabled;
                Main.BuffSample.Ticks = sample.Ticks;
                MainForm.LastClipboard = LastClipboard.Samples;
            }

            if (samplePartBufferReady)
            {
                SyncSamplePartFileAge = samplePartBufferAge;
                using (StreamReader streamReader = new StreamReader(SyncSamplePartFile))
                {
                    s = streamReader.ReadLine();
                    SampleCopy.FromColumn = (byte)Convert.ToInt32(s);
                    s = streamReader.ReadLine();
                    SampleCopy.ToColumn = (byte)Convert.ToInt32(s);
                    s = streamReader.ReadLine();
                    SampleCopy.FromLine = (byte)Convert.ToInt32(s);
                    s = streamReader.ReadLine();
                    SampleCopy.ToLine = (byte)Convert.ToInt32(s);
                    SampleCopy.Ready = true;
                    SampleCopy.Sample = Main.BuffSample;
                    MainForm.LastClipboard = LastClipboard.Samples;
                }
            }
            // Sync ornament copy/paste buffer
            if (ornamentBufferReady)
            {
                SyncOrnamentBufferFileAge = ornamenBufferAge;
                using (StreamReader streamReader = new StreamReader(SyncOrnamentBufferFile))
                {
                    s = streamReader.ReadLine();
                    ornament = new Ornament();
                    if (Ornament.RecognizeOrnamentString(s, ornament))
                    {
                        Main.BuffOrnament.Loop = ornament.Loop;
                        Main.BuffOrnament.Length = ornament.Length;
                        Main.BuffOrnament.Offsets = ornament.Offsets;
                    }
                    s = streamReader.ReadLine();

                    if (s == "All")
                        Main.BuffOrnament.CopyAll = true;

                    MainForm.LastClipboard = LastClipboard.Ornaments;
                }
            }
        }

        public void ChangePatternsLength1_Click(object sender, EventArgs e)
        {
            int patternsLength = 0;
            string patternsLengthStr = "";

            do
            {
                if (!Globals.InputQuery(Application.ProductName, $"Enter New Length For Selected Patterns From 1 to {VTModule.MaxPatternLength}", ref patternsLengthStr))
                    return;

                // Check is number entered
                //val(PatternsLengthStr, PatternsLength, i);
                bool success = int.TryParse(patternsLengthStr, out patternsLength);

                if (success && (patternsLength >= 1) && (patternsLength <= VTModule.MaxPatternLength))
                    break;
            }
            while (true);

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.ChangePatternsLength(patternsLength);
        }

        public void SplitPattern_Execute(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.SplitPattern();
        }

        public void RedrawOff()
        {
#if NOREDRAW
            return;
#endif
            if (!RedrawEnabled)
                return;

            SendMessage(ClientHandle(), WM_SETREDRAW, 0, 0);
            RedrawEnabled = false;
        }

        public void RedrawOn()
        {
#if NOREDRAW
            return;
#endif
            if (RedrawEnabled)
                return;

            IntPtr clientHandle = ClientHandle();

            SendMessage(clientHandle, WM_SETREDRAW, 1, 0);
            RedrawWindow(clientHandle, IntPtr.Zero, IntPtr.Zero, RDW_NOERASE | RDW_NOFRAME | RDW_NOINTERNALPAINT | RDW_INVALIDATE | RDW_ALLCHILDREN | RDW_UPDATENOW);
            RedrawEnabled = true;
        }

        public void InsertPositions_Update(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            bool res = (this.MdiChildren.Length != 0) && !(WaveOutAPI.IsPlaying && (AY.PlayMode == PlayModes.PlayModule)) && activeForm.PositionsGrid.Focused && ((activeForm.VTM.Positions.Length > activeForm.PositionsGrid.Selection.Left) || ((activeForm.VTM.Positions.Length == 0) && (activeForm.PositionsGrid.Selection.Left == 0)));
            InsertPosition.Enabled = res;
            UIActionManager.Instance.SetEnabled(UIActionType.InsertPositions, res);
        }

        public void DeletePositions_Update(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            UIActionManager.Instance.SetEnabled(UIActionType.DeletePositions, this.MdiChildren.Length != 0 && !(WaveOutAPI.IsPlaying && (AY.PlayMode == PlayModes.PlayModule)) && activeForm.PositionsGrid.Focused && (activeForm.VTM.Positions.Length > activeForm.PositionsGrid.Selection.Left));
        }

        public void PlayStop_Execute(object sender, EventArgs e)
        {
            bool patternsTabActive;
            bool samplesTabActive;
            bool ornamentsTabActive;
            bool currentWindowPlaying;

            if (this.MdiChildren.Length == 0)
                return;

            // Detect active tab
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            patternsTabActive = (activeForm.TabControl.SelectedTab == activeForm.PatternsTab) || (activeForm.TabControl.SelectedTab == activeForm.OptionsTab) || (activeForm.TabControl.SelectedTab == activeForm.InfoTab);
            samplesTabActive = activeForm.TabControl.SelectedTab == activeForm.SamplesTab;
            ornamentsTabActive = activeForm.TabControl.SelectedTab == activeForm.OrnamentsTab;

            if (NoPatterns() && patternsTabActive && !WaveOutAPI.IsPlaying)
                return;

            // Is current window playing?
            currentWindowPlaying = (activeForm.PlayStopState == PlayStopState.Stop);

            // Stop playing all
            if (WaveOutAPI.IsPlaying)
            {
                WaveOutAPI.StopPlaying();
                RestoreControls();
            }

            if (currentWindowPlaying)
                return;

            // Play track if patterns editor tab is active
            if (patternsTabActive && activeForm.PlayStopState == PlayStopState.Play)
            {
                AY.PlayMode = PlayModes.PlayModule;
                DisableControls(true);

                // CheckSecondWindow(True);

                activeForm.CheckPositionsGridPosition();
                activeForm.PlayStopState = PlayStopState.Stop;
                activeForm.Tracks.RemoveSelection();
                activeForm.RerollToPos(activeForm.PositionIndex, 0);

                ScrollToPlayingWindow();
                WaveOutAPI.StartWOThread();
                PlayStop_Update(null, EventArgs.Empty);
            }

            // Play sample, if samples tab is active
            if (samplesTabActive && activeForm.PlayStopState == PlayStopState.Play)
                activeForm.SampleTestLine.PlayCurrentNote();

            // Play ornament, if samples tab is active
            if (ornamentsTabActive && activeForm.PlayStopState == PlayStopState.Play)
                activeForm.OrnamentTestLine.PlayCurrentNote();
        }

        public void PlayStop_Update(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0 || WaveOutAPI.ExportStarted)
            {
                UIActionManager.Instance.SetEnabled(UIActionType.PlayStop, false);
                UIActionManager.Instance.SetEnabled(UIActionType.Stop, false);
                return;
            }
            else
            {
                UIActionManager.Instance.SetEnabled(UIActionType.PlayStop, true);
            }

            UIActionManager.Instance.SetEnabled(UIActionType.Stop, WaveOutAPI.IsPlaying);

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            if (activeForm == null)
                return;

            UIActionManager.Instance.SetImageIndex(UIActionType.PlayStop, activeForm.PlayStopState == PlayStopState.Stop ? 20 : 18);
        }

        public void MainForm_Paint(object sender, PaintEventArgs e)
        {
            //this.BackColor = CFullScreenBackground;
            //this.Canvas.BackColor = CFullScreenBackground;
            //this.Canvas.FillRect(new Rectangle(0, ToolStrip1.Height, this.ClientSize.Width, this.ClientSize.Height));
        }

        public void AutoStep0_Execute(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.AutoStepUpDown.Value = 0;
        }

        public void AutoStep1_Execute(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.AutoStepUpDown.Value = 1;
        }

        public void AutoStep2_Execute(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.AutoStepUpDown.Value = 2;
        }

        public void AutoStep3_Execute(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.AutoStepUpDown.Value = 3;
        }

        public void AutoStep4_Execute(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.AutoStepUpDown.Value = 4;
        }

        public void AutoStep5_Execute(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.AutoStepUpDown.Value = 5;
        }

        public void AutoStep6_Execute(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.AutoStepUpDown.Value = 6;
        }

        public void AutoStep7_Execute(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.AutoStepUpDown.Value = 7;
        }

        public void AutoStep8_Execute(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.AutoStepUpDown.Value = 8;
        }

        public void AutoStep9_Execute(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.AutoStepUpDown.Value = 9;
        }

        public void ExportToWAV_Execute(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.ExportToWavFile();
        }

        public void ExportToWAV_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetEnabled(UIActionType.ExportToWAV, this.MdiChildren.Length != 0 && !WaveOutAPI.ExportStarted);
        }

        public void NewTurboSound(int turboSoundCount = 2)
        {
            ChildForm[] tsWindow = new ChildForm[3];

            RedrawOff();

            CreateMDIChild(string.Empty, turboSoundCount);

            int first = ChildsTable.Length - turboSoundCount;
            tsWindow[0] = ChildsTable[first];

            if (turboSoundCount == 3)
            {
                tsWindow[1] = ChildsTable[first + 1];
                tsWindow[2] = ChildsTable[first + 2];

                tsWindow[1].Text = $"Mid TS {WinCount}";
                tsWindow[2].Text = $"Right TS {WinCount}";
                tsWindow[2].NumModule = 3;
            }
            else
            {
                tsWindow[1] = ChildsTable[first + 1];
                tsWindow[1].Text = $"Right TS {WinCount}";
            }

            tsWindow[0].Text = $"Left TS {WinCount}";
            tsWindow[0].NumModule = 1;
            tsWindow[1].NumModule = 2;

            RedrawOn();

            tsWindow[0].Activate();
            if (tsWindow[0].Tracks.CanFocus)
                tsWindow[0].Tracks.Focus();
        }

        public void NewTurboSound_Execute(object sender, EventArgs e)
        {
            NewTurboSound();
        }

        public void NewTurboSound3_Execute(object sender, EventArgs e)
        {
            NewTurboSound(3);
        }

        public void SaveAsTwoModules_Update(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            UIActionManager.Instance.SetEnabled(UIActionType.SaveAsTwoModules, this.MdiChildren.Length != 0 && activeForm.TSWindow[0] != null && !WaveOutAPI.ExportStarted);

            if (this.MdiChildren.Length != 0 &&
                activeForm.TSWindow[0] != null &&
                activeForm.TSWindow[1] != null &&
                !WaveOutAPI.ExportStarted)
                SaveAsTwoModules.Text = "Save As 3 modules..";
            else
                SaveAsTwoModules.Text = "Save As 2 modules..";

            SaveAsTwoModules.Visible = UIActionManager.Instance.IsEnabled(UIActionType.SaveAsTwoModules);
        }

        public void MainForm_DoubleClick(object sender, EventArgs e)
        {
            FileOpen_Execute(sender, e);
        }

        public void Stop_Execute(object sender, EventArgs e)
        {
            // Esc for hide active windows
            if (Globals.GlobalTransForm.Visible)
            {
                Globals.GlobalTransForm.Hide();
                return;
            }

            if (Globals.ToggleSamplesForm.Visible)
            {
                Globals.ToggleSamplesForm.Hide();
                return;
            }

            if (Globals.TracksManagerForm.Visible)
            {
                Globals.TracksManagerForm.Hide();
                return;
            }

            // Stop playing all
            if (WaveOutAPI.IsPlaying)
            {
                WaveOutAPI.StopPlaying();
                RestoreControls();
            }

            Globals.OptionsForm.UpdateAudioSettings();
        }

        public void Maximize_Execute(object sender, EventArgs e)
        {
            ChildsEventsBlocked = true;
            SysCmd = SC_MAXIMIZE;
            this.WindowState = FormWindowState.Maximized;
            ChildsEventsBlocked = false;
        }

        public void Normal_Execute(object sender, EventArgs e)
        {
            ChildsEventsBlocked = true;
            SysCmd = SC_RESTORE;
            this.WindowState = FormWindowState.Normal;
            ChildsEventsBlocked = false;
        }

        public void PlayFromLine_Execute(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            // Already playing
            if (WaveOutAPI.IsPlaying && (AY.PlayMode == PlayModes.PlayModule || AY.PlayMode == PlayModes.PlayPattern) && (activeForm.PlayStopState == PlayStopState.Stop))
                return;

            // Stop playing all
            if (WaveOutAPI.IsPlaying)
            {
                WaveOutAPI.StopPlaying();
                RestoreControls();
            }

            if (NoPatterns())
                return;

            DisableControls(true);
            AY.PlayMode = PlayModes.PlayModule;
            activeForm.CheckPositionsGridPosition();
            activeForm.PlayStopState = PlayStopState.Stop;
            activeForm.ValidatePattern2(activeForm.PatternIndex);
            activeForm.Tracks.RemoveSelection();
            ScrollToPlayingWindow();

            if (activeForm.TSWindow[0] == null)
                activeForm.RestartPlaying(false, false);
            else
                activeForm.RestartPlayingTS(false, false);

            WaveOutAPI.StartWOThread();
            PlayStop_Update(null, EventArgs.Empty);
        }

        public void SaveAsTwoModules_Execute(object sender, EventArgs e)
        {
            if (WaveOutAPI.IsPlaying)
            {
                WaveOutAPI.StopPlaying();
                RestoreControls();
            }

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            ChildForm[] tsWindow = new ChildForm[3];

            // Save turbosound windows state
            tsWindow[0] = activeForm;
            tsWindow[1] = activeForm.TSWindow[0];
            tsWindow[2] = activeForm.TSWindow[1];
            string fileName = tsWindow[0].WinFileName;

            // Split turbotrack
            tsWindow[0].TSWindow[0] = null;
            tsWindow[1].TSWindow[0] = null;

            if (tsWindow[2] != null)
                tsWindow[2].TSWindow[0] = null;

            // Save modules
            bool saved1 = tsWindow[0].SaveModuleAs();
            bool saved2 = tsWindow[1].SaveModuleAs();
            bool saved3 = false;

            if (tsWindow[2] != null)
                saved3 = tsWindow[2].SaveModuleAs();

            // Merge turbotrack back
            tsWindow[0].TSWindow[0] = tsWindow[1];
            tsWindow[0].TSWindow[1] = tsWindow[2];
            tsWindow[1].TSWindow[0] = tsWindow[0];
            tsWindow[1].TSWindow[1] = tsWindow[2];

            if (saved1)
                tsWindow[0].SetFileName(fileName);

            if (saved2)
                tsWindow[1].SetFileName(fileName);

            if (saved3 && tsWindow[2] != null)
                tsWindow[2].SetFileName(fileName);
        }

        public void PlayFromLine_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetEnabled(UIActionType.PlayFromLine, this.MdiChildren.Length != 0 && !WaveOutAPI.ExportStarted);
        }

        public void ToggleLoopingAll_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetEnabled(UIActionType.ToggleLoopingAll, !WaveOutAPI.ExportStarted);
        }

        public void ToggleLooping_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetEnabled(UIActionType.ToggleLooping, !WaveOutAPI.ExportStarted);
        }

        public void Stop_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetEnabled(UIActionType.Stop, this.MdiChildren.Length != 0 && !WaveOutAPI.ExportStarted);
        }

        public void FileNew_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetEnabled(UIActionType.FileNew, !WaveOutAPI.ExportStarted);
        }

        public void FileOpen_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetEnabled(UIActionType.FileOpen, !WaveOutAPI.ExportStarted);
        }

        public void FileClose_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetEnabled(UIActionType.FileClose, !WaveOutAPI.ExportStarted && this.MdiChildren.Length > 0);
        }

        public void NewTurboSound_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetEnabled(UIActionType.NewTurboSound, !WaveOutAPI.ExportStarted);
        }

        public void NewTurboSound3_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetEnabled(UIActionType.NewTurboSound3, !WaveOutAPI.ExportStarted);
        }

        public void SwapChannelsLeft_Execute(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.DoSwapChannels(false);
        }

        public void SwapChannelsRight_Execute(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.DoSwapChannels(true);
        }

        public void SwapChannelsLeft_Update(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
            {
                UIActionManager.Instance.SetEnabled(UIActionType.SwapChannelsLeft, false);
                return;
            }

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            UIActionManager.Instance.SetEnabled(UIActionType.SwapChannelsLeft, activeForm.TabControl.SelectedIndex == 0);
            SwapChannelsLeft.Visible = UIActionManager.Instance.IsEnabled(UIActionType.SwapChannelsLeft);
        }

        public void SwapChannelsRight_Update(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
            {
                UIActionManager.Instance.SetEnabled(UIActionType.SwapChannelsRight, false);
                return;
            }

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            UIActionManager.Instance.SetEnabled(UIActionType.SwapChannelsRight, activeForm.TabControl.SelectedIndex == 0);
            SwapChannelsRight.Visible = UIActionManager.Instance.IsEnabled(UIActionType.SwapChannelsRight);
            Sep8.Visible = SwapChannelsRight.Visible;
        }

        public void MainForm_Resize(object sender, EventArgs e)
        {
            Rectangle newSize;
            bool maximize;
            bool resizeOneChild;
            bool restore;
            bool resize;
            bool stateNormal;
            bool stateMaximized;
            bool moveCmd;
            Rectangle monitorRect;
            int mouseX;

            if (this.MdiChildren.Length == 0)
                return;

            if (ResizeActionBlocked)
                return;

            if (WindowUnsnap)
                return;

            if (SysCmd == -1 && !UnderWine())
                return;

            mouseX = Cursor.Position.X;
            ResizeActionBlocked = true;

            newSize = new Rectangle(Left, Top, this.ClientSize.Width, this.ClientSize.Height);

            stateNormal = this.WindowState == FormWindowState.Normal;
            stateMaximized = this.WindowState == FormWindowState.Maximized;
            moveCmd = SysCmd == SC_MOVE;

            maximize = SysCmd == SC_MAXIMIZE;
            restore = SysCmd == SC_RESTORE;
            maximize = maximize || (stateNormal && moveCmd && (this.ClientSize.Width == MonitorWorkAreaWidth()) && (this.Height >= MonitorWorkAreaHeight()));
            restore = restore || stateMaximized && moveCmd;

            resize = !maximize && !restore;
            resizeOneChild = resize && (this.MdiChildren.Length == 1);

            WindowSnap = resize && stateNormal && moveCmd && ((this.Top == Screen.FromControl(this).WorkingArea.Top) || (AbsTop() == MonitorWorkAreaHeight() / 2)); // and (Top <> PrevTop);
            WindowSnap = WindowSnap || resize && stateNormal && moveCmd && (this.ClientSize.Width + 2 == MonitorWorkAreaWidth() / 2);
            WindowUnsnap = !restore && !WindowSnap && stateNormal && moveCmd && Snaped;

            // Event: window snap to the left/right screen border.
            if (WindowSnap || WindowUnsnap)
            {
                resizeOneChild = false;
                resize = false;
            }

            if (maximize || restore || WindowSnap)
                RedrawOff();

            // maximize event
            if (maximize)
            {
                SysCmd = SC_MAXIMIZE;
                this.WindowState = FormWindowState.Maximized;
                SnappedToRight = false;
                Snaped = false;
                ResetConstraints(false);
                SetChildsPosition(FormWindowState.Maximized);
                AutoMetricsForChilds(FormWindowState.Maximized);
                AutoCutChilds(newSize);
                RedrawChilds();
                AutoToolBarPosition(newSize);
            }

            // Set normal window size
            else if (restore)
            {
                SysCmd = SC_RESTORE;
                this.WindowState = FormWindowState.Normal;
                SnappedToRight = false;
                Snaped = false;

                SetChildsPosition(FormWindowState.Normal);
                AutoMetricsForChilds(FormWindowState.Normal);
                newSize = GetSizeForChilds(FormWindowState.Normal, false);

                if (newSize.Width > MonitorWorkAreaWidth())
                    newSize.Width = MonitorWorkAreaWidth() - DoubleBorderSize();

                if (moveCmd)
                {
                    SizeFixed = true;

                    if (mouseX < this.Left || mouseX > this.Left + newSize.Width)
                        MoveShift = (mouseX - (newSize.Width / 2)) - this.Left;
                }

                AutoCutChilds(newSize);
                RedrawChilds();
                SetWindowSize(newSize);
                AutoToolBarPosition(newSize);
            }

            // Resize one child
            else if (resizeOneChild)
            {
                ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
                activeForm.TabControl.Height = WorkAreaHeight(this.ClientSize.Height) - activeForm.TabControl.Top + 5;
                activeForm.HeightChanged = activeForm.LastHeight != activeForm.TabControl.Height;
                activeForm.LastHeight = activeForm.TabControl.Height;
                activeForm.Tracks.RedrawDisabled = true;
                activeForm.AutoResizeForm();
                activeForm.Tracks.RedrawDisabled = false;
            }
            else if (resize)
            {
                AutoToolBarPosition(newSize);
                AutoMetricsForChilds(this.WindowState);
                AutoCutChilds(newSize);
                RedrawChilds();
            }
            // Aero snap
            else if (WindowSnap)
            {
                Snaped = true;
                newSize = GetSizeForChilds(FormWindowState.Normal, false);
                monitorRect = Screen.FromControl(this).WorkingArea;
                if (this.Left - monitorRect.Left > 0)
                {
                    newSize = new Rectangle(monitorRect.Right - newSize.Width - BorderSize() - 1, newSize.Y, newSize.Width, newSize.Height);
                    SnappedToRight = true;
                }
                else
                    SnappedToRight = false;

                newSize = new Rectangle(monitorRect.Right - newSize.Width - BorderSize() - 1, monitorRect.Top, newSize.Width, monitorRect.Bottom - OuterHeight() + BorderSize() - 1 - monitorRect.Top);

                SizeFixed = true;
                SetWindowSize(newSize);

                SetChildsPosition(FormWindowState.Normal);
                AutoMetricsForChilds(FormWindowState.Normal);
                AutoToolBarPosition(newSize);
                AutoCutChilds(newSize);
                RedrawChilds();
            }
            // Unsnap
            else if (WindowUnsnap)
            {
                SizeFixed = true;
                Snaped = false;
                SnappedToRight = false;

                SetChildsPosition(FormWindowState.Normal);
                AutoMetricsForChilds(FormWindowState.Normal);

                newSize.Width = ChildsWidth();
                if (newSize.Width > MonitorWorkAreaWidth())
                    newSize.Width = MonitorWorkAreaWidth() - DoubleBorderSize();

                // if (MouseX < Left) or (MouseX > Left + NewSize.Width) then
                MoveShift = (mouseX - (newSize.Width / 2)) - this.Left;

                AutoCutChilds(newSize);
                RedrawChilds();
                this.WindowState = FormWindowState.Normal;
                AutoToolBarPosition(newSize);
                SetWindowSize(newSize);
            }

            StatusBar.Items[0].Width = this.ClientSize.Width - StatusBar.Items[1].Width - StatusBar.Items[2].Width - 8;
#if DEBUG
            // StatusStrip.Panels[0].Text := Format(
            // 'Maximize: %d, Restore: %d, Snap: %d, Unsnap: %d, Resize: %d, OneChRes: %d',
            // [Ord(Maximize), Ord(Restore), Ord(WindowSnap), Ord(WindowUnsnap), Ord(Resize), Ord(ResizeOneChild)]
            // );
#endif // DEBUG

            ResizeActionBlocked = false;
            WindowSnap = false;

            if (maximize || restore || WindowSnap || WindowUnsnap)
                SysCmd = -1;

            if (maximize || restore || WindowSnap)
                RedrawOn();
        }

        public void JoinTracks_Update(object sender, EventArgs e)
        {
            if (MdiChildren.Length <= 1 || WaveOutAPI.ExportStarted)
            {
                UIActionManager.Instance.SetEnabled(UIActionType.JoinTracks, false);
                return;
            }

            MultitrackReorder();

            foreach (Form form in MdiChildren)
            {
                ChildForm childForm = (ChildForm)form;

                bool hasTSWindow = childForm.TSWindow[0] != null;

                childForm.DisconnectButton.Enabled = hasTSWindow;
                childForm.DisconnectButton.ImageIndex = hasTSWindow ? 1 : 0;

                string s1 = childForm.TSWindow[0] != null ? $"T{childForm.TSWindow[0].WinNumber}" : "--";
                string s2 = childForm.TSWindow[1] != null ? $"T{childForm.TSWindow[1].WinNumber}" : "--";

                childForm.JoinLabel.Text = $"{s1} - {s2}";
            }

            UIActionManager.Instance.SetEnabled(UIActionType.JoinTracks, true);
        }

        public void MultitrackReorder()
        {
            foreach (Form mdiForm in MdiChildren)
            {
                var childWin1 = (ChildForm)mdiForm;
                var childWin2 = childWin1.TSWindow[0];
                var childWin3 = childWin1.TSWindow[1];

                int x1 = childWin1.Left;
                int x2 = childWin2 != null ? childWin2.Left : int.MinValue;
                int x3 = childWin3 != null ? childWin3.Left : int.MinValue;

                // single
                if (childWin3 == null && childWin2 == null)
                    continue;

                // 2ts
                if (childWin3 != null && childWin2 == null)
                {
                    if (x1 < x2)
                    {
                        childWin2.NumModule = 1;
                        childWin1.NumModule = 2;
                    }
                }

                if (childWin2 == null || childWin3 == null)
                    continue;

                // 3ts swap
                if ((x1 < x3 && x3 < x2) || // 1 3 2 => 1 2 3
                    (x2 < x1 && x1 < x3) || // 2 1 3 => 3 1 2
                    (x3 < x2 && x2 < x1))   // 3 2 1 => 2 3 1
                {
                    childWin1.TSWindow[0] = childWin3;
                    childWin1.TSWindow[1] = childWin2;
                }

                if (x1 < x2 && x2 < x3)
                {
                    childWin1.NumModule = 1;
                    childWin2.NumModule = 2;
                    childWin3.NumModule = 3;
                }
            }
        }

        public void JoinTracks_Execute(object sender, EventArgs e)
        {
            var activeForm = (ChildForm)this.ActiveMdiChild;

            Globals.TurboSoundForm.ListBox1.Items.Clear();

            for (int i = 0; i < this.MdiChildren.Length; i++)
            {
                ChildForm childForm = (ChildForm)this.MdiChildren[i];

                if (((childForm.TSWindow[0] == null) || (childForm.TSWindow[1] == null)) &&
                    (childForm.TSWindow[0] != activeForm) &&
                    (childForm.TSWindow[1] != activeForm) &&
                    (childForm != activeForm))
                {
                    Globals.TurboSoundForm.ListBox1.Items.Add(childForm);
                }
            }

            if (Globals.TurboSoundForm.ListBox1.Items.Count > 0 &&
                Globals.TurboSoundForm.ShowDialog() == DialogResult.OK &&
                Globals.TurboSoundForm.ListBox1.SelectedIndex >= 0)
            {
                WinCount--;

                if (this.MdiChildren.Length >= 2)
                {
                    SetChildsPosition(this.WindowState);
                    AutoMetricsForChilds(this.WindowState);
                    RedrawChilds();
                    var newSize = GetSizeForChilds(this.WindowState, false);
                    AutoToolBarPosition(newSize);
                    SetWindowSize(newSize);
                }

                var childToJoin = (ChildForm)Globals.TurboSoundForm.ListBox1.SelectedItem;

                if (childToJoin != null)
                {
                    activeForm.JoinChild(childToJoin);
                    JoinTracks_Update(this, e);
                    activeForm.SynchronizeModules();
                }
            }
        }

        public void MainForm_Shown(object sender, EventArgs e)
        {
            //PostMessage(this.Handle, WM_USER, 0, 0);
            if (StartupOpenModule)
            {
                CreateChildWrapper(Environment.GetCommandLineArgs()[1]);
                return;
            }

            if (StartupOpenTheme)
            {
                ColorThemes.LoadColorTheme(Environment.GetCommandLineArgs()[1]);
                MessageBox.Show(this, "Done. Color Theme Successfully Imported!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // Vortex first start - open demosong
            if (VortexFirstStart)
            {
                DontAddToRecent = true;
                CreateChildWrapper(Path.Combine(VortexDocumentsDir, DemoSongsDefaultDir, "2019_MmcM_Conversions.vt2"));
                DontAddToRecent = false;
                return;
            }

            // Do startup action
            DontAddToRecent = true;
            SetChildAsTemplate = true;

            switch (StartupAction)
            {
                case 0:
                    if (TemplateSongPath != "")
                    {
                        CreateChildWrapper(TemplateSongPath);

                        if (!System.IO.File.Exists(TemplateSongPath))
                            TemplateSongPath = "";
                    }
                    break;
                case 1:
                    FileNew_Execute(this, EventArgs.Empty);
                    break;
            }

            DontAddToRecent = false;
            SetChildAsTemplate = false;
        }

        public void SaveAsTemplate_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetEnabled(UIActionType.SaveAsTemplate, this.MdiChildren.Length != 0 && !WaveOutAPI.ExportStarted);
            SaveAsTemplate.Visible = UIActionManager.Instance.IsEnabled(UIActionType.SaveAsTemplate);
        }

        public void SaveAsTemplate_Execute(object sender, EventArgs e)
        {
            StartupAction = 0;
            TemplateSongPath = Path.Combine(VortexDocumentsDir, "template.vt2");
            DontAddToRecent = true;

            SavePT3(GetMainModule(), TemplateSongPath, true);

            DontAddToRecent = false;

            MessageBox.Show(this, "Done. Song Is Successfully Saved as a Startup Template.", "Save as Template", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void DisableControlsForExport()
        {
            DisableControls(true);

            ToolStrip1.Enabled = false;
            RFile1.Enabled = false;
            RFile2.Enabled = false;
            RFile3.Enabled = false;
            RFile4.Enabled = false;
            RFile5.Enabled = false;
            RFile6.Enabled = false;
            OpenDemoSong.Enabled = false;
            Options.Enabled = false;
            Exports.Enabled = false;
            ToggleSamples.Enabled = false;
            TracksManager.Enabled = false;
            GlobalTransposition.Enabled = false;
            ExportPSG.Enabled = false;
            ExportYM.Enabled = false;
            ChildForm.PlayingWindow[0].TabControl.Enabled = false;

            if (ChildForm.PlayingWindow[1] != null)
                ChildForm.PlayingWindow[1].TabControl.Enabled = false;

            if (ChildForm.PlayingWindow[2] != null)
                ChildForm.PlayingWindow[2].TabControl.Enabled = false;
        }

        public void EnableControlsForExport()
        {
            ToolStrip1.Enabled = true;
            RFile1.Enabled = true;
            RFile2.Enabled = true;
            RFile3.Enabled = true;
            RFile4.Enabled = true;
            RFile5.Enabled = true;
            RFile6.Enabled = true;
            OpenDemoSong.Enabled = true;
            Options.Enabled = true;
            Exports.Enabled = true;
            ToggleSamples.Enabled = true;
            TracksManager.Enabled = true;
            GlobalTransposition.Enabled = true;
            ChildForm.PlayingWindow[0].TabControl.Enabled = true;
            ExportPSG.Enabled = true;
            ExportYM.Enabled = true;

            if (ChildForm.PlayingWindow[1] != null)
                ChildForm.PlayingWindow[1].TabControl.Enabled = true;

            if (ChildForm.PlayingWindow[2] != null)
                ChildForm.PlayingWindow[2].TabControl.Enabled = true;

            RestoreControls();
        }

        private void MIDITimer_Tick(object sender, EventArgs e)
        {
            if (MidiInputClient != null)
            {
                var inputDevices = MidiManager.GetAvailableDevices();

                if (inputDevices.Count == 0)
                    DestroyMidiInputClient();
            }
            else
                CreateMidiInputClient();
        }

        public void TransposeUp3_Execute(object sender, EventArgs e)
        {
            TransposeSelection(3);
        }

        public void TransposeDown3_Execute(object sender, EventArgs e)
        {
            TransposeSelection(-3);
        }

        public void TransposeUp5_Execute(object sender, EventArgs e)
        {
            TransposeSelection(5);
        }

        public void TransposeDown5_Execute(object sender, EventArgs e)
        {
            TransposeSelection(-5);
        }

        public void TransposeUp3_Update(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            UIActionManager.Instance.SetEnabled(UIActionType.TransposeUp3, this.MdiChildren.Length != 0 && activeForm.Tracks.Focused);
        }

        public void TransposeDown3_Update(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            UIActionManager.Instance.SetEnabled(UIActionType.TransposeDown3, this.MdiChildren.Length != 0 && activeForm.Tracks.Focused);
        }

        public void TransposeUp5_Update(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            UIActionManager.Instance.SetEnabled(UIActionType.TransposeUp5, this.MdiChildren.Length != 0 && activeForm.Tracks.Focused);
        }

        public void TransposeDown5_Update(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            UIActionManager.Instance.SetEnabled(UIActionType.TransposeDown5, this.MdiChildren.Length != 0 && activeForm.Tracks.Focused);
        }

        public void CopyToModPlug_Update(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            UIActionManager.Instance.SetEnabled(UIActionType.CopyToModPlug, this.MdiChildren.Length != 0 && activeForm.Tracks.Focused && activeForm.Tracks.IsSelected());
        }

        public void CopyToRenoise_Update(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            UIActionManager.Instance.SetEnabled(UIActionType.CopyToRenoise, this.MdiChildren.Length != 0 && activeForm.Tracks.Focused && activeForm.Tracks.IsSelected());
        }

        public void CopyToModPlug_Execute(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.CopyToModplug();
        }

        public void CopyToRenoise_Execute(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.CopyToRenoise();
        }

        public void CopyToFami_Update(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            UIActionManager.Instance.SetEnabled(UIActionType.CopyToFami, this.MdiChildren.Length != 0 && activeForm.Tracks.Focused && activeForm.Tracks.IsSelected());
        }

        public void CopyToFami_Execute(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.CopyToFamiTracker();
        }

        public void PatternPacker_Execute(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;
            activeForm.PackPattern();
        }

        public void ExportPSG_Execute(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            if (activeForm == null)
                return;

            if (this.MdiChildren.Length == 0)
                return;

            if (WaveOutAPI.ExportStarted)
                return;

            if (NoPatterns())
                return;

            // Stop playing all
            if (WaveOutAPI.IsPlaying)
            {
                WaveOutAPI.StopPlaying();
                RestoreControls();
            }

            activeForm.ExportPSG();
        }

        public void ExportPSG_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetEnabled(UIActionType.ExportPSG, this.MdiChildren.Length > 0 && !WaveOutAPI.ExportStarted);
            Exports.Enabled = UIActionManager.Instance.IsEnabled(UIActionType.ExportPSG);
        }

        public void ExportYM_Execute(object sender, EventArgs e)
        {
            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            if (activeForm == null)
                return;

            if (this.MdiChildren.Length == 0)
                return;

            if (WaveOutAPI.ExportStarted)
                return;

            if (NoPatterns())
                return;

            // Stop playing all
            if (WaveOutAPI.IsPlaying)
            {
                WaveOutAPI.StopPlaying();
                RestoreControls();
            }

            // Cast ActiveMdiChild to your custom form class, then call ExportYM
            activeForm.ExportYM();
        }

        public void ExportYM_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.SetEnabled(UIActionType.ExportYM, this.MdiChildren.Length > 0 && !WaveOutAPI.ExportStarted);
            Exports.Enabled = UIActionManager.Instance.IsEnabled(UIActionType.ExportYM);
        }

        public void ToggleSamples_Update(object sender, EventArgs e)
        {
            ToggleSamplesButton.Enabled = (this.MdiChildren.Length != 0) && !WaveOutAPI.ExportStarted;
            ToggleSamples.Enabled = ToggleSamplesButton.Enabled;
        }

        public void TracksManager_Update(object sender, EventArgs e)
        {
            TracksManagerButton.Enabled = (this.MdiChildren.Length != 0) && !WaveOutAPI.ExportStarted;
            TracksManager.Enabled = TracksManagerButton.Enabled;
        }

        public void GlobalTransposition_Update(object sender, EventArgs e)
        {
            GlobalTranspositionButton.Enabled = (this.MdiChildren.Length != 0) && !WaveOutAPI.ExportStarted;
            GlobalTransposition.Enabled = GlobalTranspositionButton.Enabled;
        }

        public void PluginManager_Update(object sender, EventArgs e)
        {
        }

        private void StatusBar_DoubleClick(object sender, EventArgs e)
        {
            Point mouseClickCoord = StatusBar.PointToClient(Control.MousePosition);
            int width = 0;
            int panelNum = -1;

            for (int i = 0; i < StatusBar.Items.Count; i++)
            {
                width += StatusBar.Items[i].Width;
                if (mouseClickCoord.X < width)
                {
                    panelNum = i;
                    break;
                }
            }

            if (panelNum == -1 || panelNum != 1)
                return;

            Clipboard.SetText(StatusBar.Items[1].Text);
        }

        private void CleanSelectedPatterns_Click(object sender, EventArgs e)
        {
            if (this.MdiChildren.Length == 0)
                return;

            if (WaveOutAPI.IsPlaying && AY.PlayMode == PlayModes.PlayModule)
                return;

            ChildForm activeForm = (ChildForm)this.ActiveMdiChild;

            activeForm.CleanPatterns();
        }

        private IntPtr _cachedHandle;

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            _cachedHandle = this.Handle;
        }

        public void UIAction_Execute(object sender, EventArgs e)
        {
            Debug.WriteLine($"VTAction_Execute: {sender}");

            UIActionManager.Instance.Execute(sender, e);
        }

        public void UIAction_Update(object sender, EventArgs e)
        {
            Debug.WriteLine($"VTAction_Update: {sender}");

            UIActionManager.Instance.Update(sender, e);
        }

        private void OnAppEvent(AppEventArgs e)
        {
            VortexTracker.PluginManager.RaiseAppEvent(this, e);

            switch (e.EventType)
            {
                case EventType.MessageBox:
                    {
                        string text = (string)e.Params[0];
                        string caption = (string)e.Params[1];
                        MessageBoxButtons buttons = (MessageBoxButtons)e.Params[2];
                        MessageBoxIcon icon = (MessageBoxIcon)e.Params[3];
                        e.Result = (int)MessageBox.Show(this, text, caption, buttons, icon);
                        break;
                    }

                case EventType.RedrawTracks:
                    {
                        uint curVisPos = (uint)e.Params[0];
                        UmRedrawTracks(curVisPos);
                        break;
                    }

                case EventType.PlayingOff:
                    UmPlayingOff();
                    break;

                case EventType.FinalizeWO:
                    UmFinalizeWO();
                    break;

                case EventType.SetControlsForExport:
                    {
                        bool enable = (bool)e.Params[0];

                        if (enable)
                            EnableControlsForExport();
                        else
                            DisableControlsForExport();
                        break;
                    }

                case EventType.SetChannelsAllocation:
                    {
                        int chanAllocIndex = (int)e.Params[0];
                        SetChannelsAllocation(chanAllocIndex);
                        break;
                    }

                case EventType.SetFromAndToPosition:
                    ChildForm.SetFromAndToPosition();
                    break;

                case EventType.SetChildPositions:
                    ChildForm.SetChildPositions();
                    break;

                case EventType.ChangePositions:
                    {
                        int chip = (int)e.Params[0];
                        if (ChildForm.PlayingWindow[chip] == null)
                            break;
                        ChildForm.ChangePositions(ChildForm.PlayingWindow[chip]);
                        break;
                    }

                case EventType.RestartPlayingPosition:
                    {
                        int fromPosition = (int)e.Params[0];
                        ChildForm.LeadWindow.RestartPlayingPosition(fromPosition);
                        break;
                    }

                case EventType.RestorePositionAndPattern:
                    {
                        int chip = (int)e.Params[0];
                        int posNum = (int)e.Params[1];
                        int patNum = (int)e.Params[2];
                        if (ChildForm.PlayingWindow[chip] == null)
                            break;
                        ChildForm.RestorePositionAndPattern(ChildForm.PlayingWindow[chip], posNum, patNum);
                        break;
                    }

                case EventType.RerollToLine:
                    {
                        int chip = (int)e.Params[0];
                        if (ChildForm.PlayingWindow[chip] == null)
                            break;
                        ChildForm.PlayingWindow[chip]?.RerollToLine(chip);
                        break;
                    }

                case EventType.RerollToLineNum:
                    {
                        int chip = (int)e.Params[0];
                        int line = (int)e.Params[1];
                        bool zeroLine = (bool)e.Params[2];
                        if (ChildForm.PlayingWindow[chip] == null)
                            break;
                        ChildForm.PlayingWindow[chip]?.RerollToLineNum(chip, line, zeroLine);
                        break;
                    }

                case EventType.StopAndRestart:
                    {
                        int chip = (int)e.Params[0];
                        if (ChildForm.PlayingWindow[chip] == null)
                            break;
                        ChildForm.PlayingWindow[chip]?.StopAndRestart();
                        break;
                    }

                case EventType.FXMDialog:
                    FxmParamsEventArgs args = (FxmParamsEventArgs)e.Params[0];
                    OnFXMDialog(args);
                    break;

                case EventType.UpdatePerformanceStats:
                    StatusLabel3.Text = (string)e.Params[0];
                    break;

                case EventType.ProgressBar:
                    {
                        ProgressBarEventArgs progressArgs = (ProgressBarEventArgs)e.Params[0];
                        if (progressArgs.Type == ProgressType.Initialize)
                        {
                            // Show export modal
                            Globals.ProgressBarForm.Text = progressArgs.Text;
                            Globals.ProgressBarForm.ExportProgress.Minimum = progressArgs.Minimum;
                            Globals.ProgressBarForm.ExportProgress.Maximum = progressArgs.Maximum;
                            Globals.ProgressBarForm.ExportProgress.Step = progressArgs.Step;
                            Globals.ProgressBarForm.ExportProgress.Value = progressArgs.Value;

                            Globals.ProgressBarForm.Show(Globals.MainForm);
                        }
                        else if (progressArgs.Type == ProgressType.Update)
                        {
                            Globals.ProgressBarForm.ExportProgress.Value = progressArgs.Value;
                        }
                        else if (progressArgs.Type == ProgressType.Complete)
                        {
                            Globals.ProgressBarForm.Hide();
                        }
                        break;
                    }

                case EventType.DoEvents:
                    Application.DoEvents();
                    break;
            }

            e.Complete();
        }

        private static void OnRegisterEvent(object? sender, RegisterEventArgs e)
        {
            VortexTracker.PluginManager.RaiseRegisterEvent(sender, e);

            //SerialOut.RegisterEvent(sender, e);
        }

        private static void OnPlaybackEvent(object? sender, PlaybackEventArgs e)
        {
            VortexTracker.PluginManager.RaisePlaybackEvent(sender, e);
            
            MidiOut.PlaybackEvent(sender, e);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            HotKeys.HandleHotKey(e.KeyData);
        }

        public static bool UnderWine()
        {
            bool result;

            IntPtr handle;
            if (Detected)
            {
                result = IsWine;
                return result;
            }

            result = false;
            handle = Win32.LoadLibrary("ntdll.dll");

            if (handle != IntPtr.Zero)
            {
                result = (Win32.GetProcAddress(handle, "wine_get_version") != IntPtr.Zero);
                Win32.FreeLibrary(handle);
            }

            IsWine = result;
            Detected = true;

            return result;
        }

        public static string GetUserDocumentsDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        /* public static bool IsFontExists(string fontName)
        {
            bool result;
            int i;
            if (Environment.OSVersion.Version.Major > 4)
            {
                for (i = 0; i <= InternalFonts.GetUpperBound(0); i++)
                {
                    if (InternalFonts[i, 1] == fontName)
                    {
                        result = true;
                        return result;
                    }
                }
            }
            result = Screen.PrimaryScreen.Fonts.IndexOf(fontName) != -1;
            return result;
        } */

        public static bool IsFontExists(string fontName)
        {
            using (InstalledFontCollection fontCollection = new InstalledFontCollection())
            {
                foreach (FontFamily fontFamily in fontCollection.Families)
                {
                    if (fontFamily.Name.Equals(fontName, StringComparison.CurrentCultureIgnoreCase))
                        return true;
                }
            }

            foreach (FontFamily family in PrivateFontCollection.Families)
            {
                if (family.Name.Equals(fontName, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        /* public static bool LoadAllResourceFonts()
        {
            PrivateFontCollection = new PrivateFontCollection();

            if (!TryGetAllResourceNames("Fonts", "*.ttf", out string[] resourceNames))
                return false;

            foreach (string resourceName in resourceNames)
                LoadResourceFileFont(resourceName);

            return true;
        }

        public static bool LoadResourceFont(string resourceName)
        {
            byte[] data = null;

            if (!TryLoadResourceBytes(resourceName, out data))
                return false;

            IntPtr fontPtr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, fontPtr, data.Length);

            PrivateFontCollection.AddMemoryFont(fontPtr, data.Length);

            return true;
        } */


        /* public static bool LoadResourceMemoryFont(string resourceName)
        {
            if (!TryLoadResourceBytes(resourceName, out byte[] data))
                return false;

            IntPtr fontPtr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, fontPtr, data.Length);

            // Register font globally in GDI so TextRenderer can see it
            uint cFonts = 0;
            AddFontMemResourceEx(fontPtr, (uint)data.Length, IntPtr.Zero, ref cFonts);

            // Add to private collection (optional if you still use FontFamily references)
            PrivateFontCollection.AddMemoryFont(fontPtr, data.Length);

            _allocatedFontPtrs.Add(fontPtr); // Prevent GC or releasing the memory

            return true;
        }

        public static bool LoadResourceFileFont(string resourceName)
        {
            if (!TryLoadResourceBytes(resourceName, out byte[] data))
                return false;

            IntPtr fontPtr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, fontPtr, data.Length);

            // Register font globally in GDI so TextRenderer can see it
            uint cFonts = 0;
            AddFontMemResourceEx(fontPtr, (uint)data.Length, IntPtr.Zero, ref cFonts);

            if (!TryGetResourceFileName(resourceName, out string fileName))
                return false;

            string filePath = Path.Combine(Path.GetTempPath(), fileName);

            System.IO.File.WriteAllBytes(filePath, data);

            // Add to private collection (optional if you still use FontFamily references)
            PrivateFontCollection.AddFontFile(filePath);

            _allocatedFontPtrs.Add(fontPtr); // Prevent GC or releasing the memory

            return true;
        }

        private static bool LoadFontFileFromDisk(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                return false;

            byte[] data = System.IO.File.ReadAllBytes(filePath);

            IntPtr fontPtr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, fontPtr, data.Length);

            uint cFonts = 0;
            AddFontMemResourceEx(fontPtr, (uint)data.Length, IntPtr.Zero, ref cFonts);

            PrivateFontCollection.AddFontFile(filePath);

            _allocatedFontPtrs.Add(fontPtr);
            return true;
        } */

        private static bool LoadFontFileFromDisk(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                return false;

            if (AddFontResourceEx(filePath, FR_PRIVATE | FR_NOT_ENUM, IntPtr.Zero) == 0)
                return false;

            PrivateFontCollection.AddFontFile(filePath);
            return true;
        }

        public static bool LoadAllResourceFonts()
        {
            PrivateFontCollection = new PrivateFontCollection();

            string fontsDir = Path.Combine(VortexDocumentsDir, FontsDefaultDir);

            if (!Directory.Exists(fontsDir))
                return false;

            string[] fontFiles = Directory.GetFiles(fontsDir, "*.ttf", SearchOption.AllDirectories);

            foreach (string filePath in fontFiles)
                LoadFontFileFromDisk(filePath);

            return fontFiles.Length > 0;
        }

        public static bool TryGetInternalFont(Font font, out InternalFont internalFont)
        {
            internalFont = null;

            foreach (InternalFont tFont in InternalFonts)
            {
                if (tFont.Name.Equals(font.Name) && tFont.Style.Equals(font.Style))
                {
                    internalFont = tFont;
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetFontFamily(string fontName, out FontFamily fontFamily)
        {
            fontFamily = null;

            foreach (FontFamily family in MainForm.PrivateFontCollection.Families)
            {
                if (!family.Name.Equals(fontName, StringComparison.CurrentCultureIgnoreCase))
                    continue;

                fontFamily = family;
                return true;
            }

            using (InstalledFontCollection fontCollection = new InstalledFontCollection())
            {
                foreach (FontFamily family in fontCollection.Families)
                {
                    if (!family.Name.Equals(fontName, StringComparison.CurrentCultureIgnoreCase))
                        continue;

                    fontFamily = family;
                    return true;
                }
            }

            return false;
        }

        public static bool TryGetResourceFileName(string resourceName, out string fileName, int pathDepth = 0)
        {
            fileName = null;

            if (string.IsNullOrEmpty(resourceName))
                return false;

            string[] parts = resourceName.Split('.');

            if (parts.Length < 2)
                return false;

            string fileBaseName = parts[parts.Length - 2];
            string extension = parts[parts.Length - 1];

            int nameStart = Math.Max(0, parts.Length - 2 - pathDepth);
            int nameLength = parts.Length - 2 - nameStart;

            string folderPath = nameLength > 0 ? string.Join(Path.DirectorySeparatorChar.ToString(), parts, nameStart, nameLength) : null;

            if (folderPath != null)
                fileName = Path.Combine(folderPath, fileBaseName + "." + extension);
            else
                fileName = fileBaseName + "." + extension;

            return true;
        }

        public static bool TryGetAllResourceNames(string pathPrefix, string fileName, out string[] resourceNames)
        {
            resourceNames = null;
            const string ResourcesPrefix = "VortexTracker.Resources.";

            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] manifestResourceNames = assembly.GetManifestResourceNames();
            List<string> resourceList = new List<string>();

            bool matchAll = string.IsNullOrEmpty(fileName) || fileName == "*" || fileName == "*.*";
            bool hasWildcard = !matchAll && fileName.IndexOf('*') >= 0;
            Regex wildcardRegex = null;

            if (hasWildcard)
            {
                string pattern = "^" + Regex.Escape(fileName).Replace("\\*", ".*") + "$";
                wildcardRegex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }

            foreach (string full in manifestResourceNames)
            {
                if (!full.StartsWith(ResourcesPrefix, StringComparison.Ordinal))
                    continue;

                string resourceName = full.Substring(ResourcesPrefix.Length);

                if (!resourceName.StartsWith(pathPrefix, StringComparison.Ordinal))
                    continue;

                string shortName = resourceName.Substring(pathPrefix.Length).TrimStart('.');

                bool isMatch = matchAll ? true : hasWildcard ? wildcardRegex!.IsMatch(shortName) : shortName.Equals(fileName, StringComparison.OrdinalIgnoreCase);

                if (isMatch)
                    resourceList.Add(resourceName);
            }

            resourceNames = resourceList.ToArray();
            return true;
        }

        public static bool TryLoadResourceBytes(string resourceName, out byte[] data)
        {
            data = null;

            const string ResourcesPrefix = "VortexTracker.Resources.";
            Assembly asm = Assembly.GetExecutingAssembly();

            string fullName = resourceName.StartsWith(ResourcesPrefix, StringComparison.Ordinal) ? resourceName : ResourcesPrefix + resourceName;

            using Stream stream = asm.GetManifestResourceStream(fullName);
            if (stream == null)
                return false;

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            data = memoryStream.ToArray();
            return true;
        }

        public static string GetWin(string command)
        {
            char[] buff = new char[0xFF + 1];
            StringBuilder stringBuilder = new StringBuilder(0xFF + 1);
            Win32.ExpandEnvironmentStrings(command, stringBuilder, (uint)stringBuilder.Length);
            return stringBuilder.ToString();
        }

        /* public void CheckPlayingModules()
        {
            Debug.Assert(AY.ActiveModule == ((TChildWin)ActiveMdiChild)?.VTM);

            for (int i = 0; i < TChildWin.PlayingWindow.Length; i++)
                Debug.Assert(AY.PlayingModule[i] == TChildWin.PlayingWindow[i]?.VTM);

            Debug.Assert(AY.PlayingModuleCount == MdiChildren.Length);
        }

        public void UpdatePlayingModules()
        {
            AY.ActiveModule = ((TChildWin)ActiveMdiChild)?.VTM;

            for (int i = 0; i < TChildWin.PlayingWindow.Length; i++)
                AY.PlayingModule[i] = TChildWin.PlayingWindow[i]?.VTM;

            AY.PlayingModuleCount = MdiChildren.Length;
        } */

        // Moved to Main
        /* public static void InitBuffSample()
        {
            Main.BuffSample.Length = 1;
            Main.BuffSample.Loop = 0;
            Main.BuffSample.Items[0] = new TSampleTick();
        }

        // Moved to Main
        public static void InitBuffOrnament()
        {
            Main.BuffOrnament.Length = 1;
            Main.BuffOrnament.Loop = 0;
            Main.BuffOrnament.Items[0] = 0;
        } */

        public static void InitBuffTracks()
        {
            TracksCopy.Pattern = null;
            TracksCopy.Channel = 0;
            TracksCopy.FromLine = 0;
            TracksCopy.ToLine = 0;
            SampleCopy.Ready = false;
        }

        public static IniFile GetConfigIniFile()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigFilePath));
            return new IniFile(ConfigFilePath);
        }

        public static bool CanCopy()
        {
            bool result = Globals.MainForm.MdiChildren.Length != 0;
            Control activeControl;

            if (!result)
                return result;

            if (WaveOutAPI.ExportStarted)
            {
                result = false;
                return result;
            }

            ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
            activeControl = activeForm.ActiveControl;

            if (activeControl is TextBox && ((TextBox)activeControl).SelectionLength > 0)
                result = true;
            else if (activeForm.Tracks == activeControl)
                result = true;
            else if (activeForm.Samples == activeControl)
                result = true;
            else if (activeForm.Ornaments == activeControl)
                result = true;
            else
                result = false;

            return result;
        }

        public static bool GetCopyControl(ref int controlType, ref Control control)
        {
            bool result = Globals.MainForm.MdiChildren.Length != 0;

            if (result)
            {
                ChildForm activeForm = (ChildForm)Globals.MainForm.ActiveMdiChild;
                controlType = -1;
                control = activeForm.ActiveControl;

                if (control is TextBox)
                {
                    controlType = 0;
                    result = true;
                    return result;
                }

                if (controlType < 0)
                {
                    result = activeForm.Tracks == control;

                    if (result)
                    {
                        controlType = 1;
                        return result;
                    }

                    result = activeForm.Samples == control;

                    if (result)
                    {
                        controlType = 2;
                        return result;
                    }

                    result = activeForm.Ornaments == control;

                    if (result)
                        controlType = 3;
                }
            }
            return result;
        }

        private void MainForm_MdiChildActivate(object sender, EventArgs e)
        {
            ChildForm activeChild = (ChildForm)this.ActiveMdiChild;
            AY.ActiveModule = activeChild?.VTM;
        }

        public void MdiChild_Load(object sender, EventArgs e)
        {
            AY.PlayingModuleCount = this.MdiChildren.Length;
        }

        public void MdiChild_Closed(object sender, FormClosedEventArgs e)
        {
            Rectangle newSize;

            // Child closed
            if (DrawOffAfterClose)
            {
                DrawOffAfterClose = false;
                return;
            }

            RedrawOff();
            // Dec(WinCount);

            ChildsEventsBlocked = true;

            AutoMetricsForChilds(this.WindowState);
            SetChildsPosition(this.WindowState);
            newSize = GetSizeForChilds(this.WindowState, false);
            AutoCutChilds(newSize);
            AutoToolBarPosition(newSize);
            RedrawChilds();

            RedrawOn();
            SetWindowSize(newSize);

            ChildsEventsBlocked = false;

            Globals.MainForm.StatusBar.Items[1].Text = "";
            Globals.MainForm.StatusBar.Items[2].Text = "";

            AY.PlayingModuleCount = MdiChildren.Length;
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            // Get all dropped file paths
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            // If you need the exact drop coordinates in client coordinates:
            Point dropPoint = this.PointToClient(new Point(e.X, e.Y));
            // (dropPoint.X, dropPoint.Y) now has the local position on this form

            foreach (var fileName in files)
            {
                string fileExt = Path.GetExtension(fileName);

                // Handle color theme (.vtt) files
                if (fileExt.Equals(".vtt", StringComparison.OrdinalIgnoreCase))
                {
                    if (ColorThemes.LoadColorTheme(fileName))
                        MessageBox.Show(this, "Done. Color Theme Successfully Imported!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show(this, "Invalid Color Theme File", "Fail", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    // Original code seems to return after the first .vtt
                    return;
                }

                // If this isn't a module extension, ignore it
                if (Array.IndexOf(ModuleExtensions, fileExt) == -1)
                    return;

                // Original code toggles "ChildsEventsBlocked" and does "RedrawOff" before loading
                ChildsEventsBlocked = true;
                RedrawOff();

                CreateMDIChild(fileName, 1);

                RedrawOn();
                ChildsEventsBlocked = false;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Debug.WriteLine("MainForm.OnFormClosing");

            if (WaveOutAPI.IsPlaying)
                WaveOutAPI.StopPlaying();

            UmFinalizeWO();

            for (int i = 0; i < this.MdiChildren.Length; i++)
                this.MdiChildren[i].Dispose();

            WaveOutAPI.Shutdown();
            SaveBackups(this, EventArgs.Empty);

            SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;

            AppEvents.ClearAllEvents();
            AppEvents.Dispatcher = null;

            _appEventTimer?.Dispose();
            _renderTimer?.Dispose();

            base.OnFormClosing(e);
        }
    }

    public class InternalFont
    {
        public string FileName;
        public string Name;
        public FontStyle Style;

        public InternalFont(string fileName, string name, FontStyle style)
        {
            FileName = fileName;
            Name = name;
            Style = style;
        }
    }

    public class TracksCopy
    {
        public ChildForm SrcWindow;
        public Pattern Pattern;
        public int PatNum;
        public byte Channel;
        public byte FromLine;
        public byte ToLine;
        public bool Ornament;
        public bool Command;
    }

    public class SampleCopy
    {
        public ChildForm SrcWindow;
        public Sample Sample;
        public byte FromColumn;
        public byte ToColumn;
        public byte FromLine;
        public byte ToLine;
        public byte StartColumn;
        public byte StartLine;
        public bool Ready;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct HOBETAHeader
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Name;
        [FieldOffset(8)]
        public byte Typ;
        [FieldOffset(9)]
        public ushort Start;
        [FieldOffset(11)]
        public ushort Leng;
        [FieldOffset(13)]
        public ushort SectLeng;
        [FieldOffset(15)]
        public ushort CheckSum;
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
        public byte[] Ind;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct SCLHeader
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] SCL;
        [FieldOffset(8)]
        public byte NBlk;
        [FieldOffset(9)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Name1;
        [FieldOffset(17)]
        public byte Typ1;
        [FieldOffset(18)]
        public ushort Start1;
        [FieldOffset(20)]
        public ushort Leng1;
        [FieldOffset(22)]
        public byte Sect1;
        [FieldOffset(23)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Name2;
        [FieldOffset(31)]
        public byte Typ2;
        [FieldOffset(32)]
        public ushort Start2;
        [FieldOffset(34)]
        public ushort Leng2;
        [FieldOffset(36)]
        public byte Sect2;
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 37)]
        public byte[] Ind;
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct TAPHeader
    {
        [FieldOffset(0)]
        public ushort Sz;
        [FieldOffset(2)]
        public byte Flag;
        [FieldOffset(3)]
        public byte Typ;
        [FieldOffset(4)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] Name;
        [FieldOffset(14)]
        public ushort Leng;
        [FieldOffset(16)]
        public ushort Start;
        [FieldOffset(18)]
        public ushort Trash;
        [FieldOffset(20)]
        public byte Sum;
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
        public byte[] Ind;
    }

    public enum LastClipboard
    {
        None,
        Tracks,
        Samples,
        Ornaments
    }
}
