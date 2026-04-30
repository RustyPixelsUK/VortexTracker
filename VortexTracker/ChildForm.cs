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
using Microsoft.VisualBasic.ApplicationServices;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.ES20;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VortexTracker.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static VortexTracker.Win32;

namespace VortexTracker
{
    public partial class ChildForm : Form
    {
        private static ChildForm[] _playingWindow = new ChildForm[VTModule.MaxSoundChipCount];
        public static ChildForm LeadWindow;
        public static int CurrentMidiNote = 0;
        public static int NoteCounter = 0;
        public static int MaxNote = 0;
        public static int[] Arp = new int[97];
        public static Rectangle PatternsOrderSelection;
        public static bool IsPatternsSelected = false;
        public static bool DisableChangingEx = false;
        public const int POS_MOVE = 1;
        public const int POS_COPY = 2;
        public const int POS_DELETE = 3;
        public const int ShowHintDelay = 1100;
        public const int HideHintDelay = 3000;
        public static int[] ColTabs = { 0, 5, 8, 12, 17, 22, 26, 31, 36, 40, 45, 49 };
        public static int[] ColTabsR = { 0, 3, 6, 8, 15, 20, 22, 29, 34, 36, 43, 48 };
        public static int[] ColTabsL = { 0, 5, 8, 12, 17, 22, 26, 31, 36, 40, 45, 46, 49 };
        public static int[] SColTabs = { 0, 5, 11, 14, 19, 20, 21 };
        public static int[] NoteTabs = { 8, 22, 36 };
        public static int[] NotePoses = { 8, 22, 36 };
        public static int[] ChanPoses = { 8, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 26, 27, 28, 29, 30, 31, 32, 33, 34, 36, 40, 41, 42, 43, 44, 45, 46, 47, 48 };
        public static int[] EnvelopePoses = { 0, 1, 2, 3 };
        public static int[] SamTabs = { 12, 26, 40 };
        public static int[] SamPoses = { 12, 26, 40 };
        public static int[] OrnPoses = { 14, 28, 42 };
        public const string ClipHdrPat = "Vortex Tracker II v2.0 Pattern\r\n";

        // Public declarations
        public bool InitFinished = false;
        public Tracks Tracks = null;
        public int LastWidth = 0;
        public int LastHeight = 0;
        public bool WidthChanged = false;
        public bool HeightChanged = false;
        public TestLine SampleTestLine = null;
        public TestLine OrnamentTestLine = null;
        public Samples Samples = null;
        public Ornaments Ornaments = null;
        public DriveSelect SamplesDriveSelect = null;
        public FileBrowser SamplesBrowser = null;
        public DriveSelect OrnamentsDriveSelect = null;
        public FileBrowser OrnamentsBrowser = null;
        public VTM VTM;
        public PlayStopState PlayStopState;
        public int PatternIndex = 0;
        public int SampleIndex = 0;
        public int OrnamentIndex = 0;
        public bool SongChanged = false;
        public bool BackupSongChanged = false;
        public int BackupVersionCounter = 0;
        public bool ShowEnvelopeAsNote = false;
        public int InputPNumber = 0;
        public int PositionIndex = 0;
        public int PosBegin = 0;
        public int PosDelay = 0;
        public int TotInts = 0;
        public int LineInts = 0;
        // InputPNumber, PosBegin, PosDelay, TotInts, LineInts: Integer;
        public ChannelMetrics[] ChannelMetrics = new ChannelMetrics[3];
        public bool AutoEnv = false;
        public bool AutoStep = false;
        public int AutoEnv0 = 0;
        public int AutoEnv1 = 0;
        public int StdAutoEnvIndex = 0;
        public bool OrGenRunning = false;
        public int ChangeCount = 0;
        public int ChangeTop = 0;
        public bool UndoWorking = false;
        public ChangeListItem[] ChangeList = new ChangeListItem[0];
        public ChangePattern[][][] ChangePatternsList = new ChangePattern[0][][];
        public ChangeOnePattern[] ChangeOnePatternList = new ChangeOnePattern[0];
        public ChangeSample[] ChangeSamplesList = new ChangeSample[0];
        public ChangeOrnament[] ChangeOrnamentsList = new ChangeOrnament[0];
        public int[][] ChangeNilPatternsList = new int[0][];
        public int WinNumber = 0;
        public string WinFileName = String.Empty;
        public bool SavedAsText = false;
        public ChildForm[] TSWindow = new ChildForm[2];
        public int NumModule = 0;
        public bool IsClosed = false;
        public bool IsSynchronizing = false;
        public bool BlockRecursion = false;
        public short SamplesClickStartLine = 0;
        public short SamplesClickEndLine = 0;
        public short SamplesLastMouseCursorY = 0;
        public short SamplesLastCursorX = 0;
        public short SamplesLastCursorY = 0;
        public bool SamplesRightMouseButton = false;
        public bool SamplesLeftMouseButton = false;
        public bool DrawOnlyT = false;
        public bool DrawOnlyN = false;
        public bool DrawOnlyE = false;
        public bool TNEValue = false;
        public bool PositiveSign = false;
        public bool DrawOnlyToneSign = false;
        public bool DrawOnlyNoiseSign = false;
        public int StringGridCharWidth = 0;
        public int StringGridCharHeight = 0;
        public int StringGridTextVShift = 0;
        public int StringGridTextHShift = 0;
        public int StringGridAddHeight = 0;
        public int PosArrowSize = 0;
        public int PosArrowVShift = 0;
        public int PosArrowHShift = 0;
        public bool IsTemplate = false;
        public bool IsDemoSong = false;
        public int[] VolumeLBuffer;
        public int[] VolumeRBuffer;
        // Precalculated values for controls
        public int ToolBoxesWidth = 0;
        public string SamplesDir = String.Empty;
        public string OrnamentsDir = String.Empty;
        public ChannelButtons[] ChanButtons = new ChannelButtons[3];
        public ChannelButtonsState[] ChanButtons_s = new ChannelButtonsState[3];
        public bool[] SampUsage = new bool[32];
        public bool[] OrnUsage = new bool[32];
        public int LLTemplate = 0;
        public string[] LLTemplates = new string[] { "+ ", " +", "+  ", " + ", "  +", "+   ", " +  ", "  + ", "   +" };

        public int DestCol;
        public int DestRow;
        public int SourceCol;
        public int SourceColEnd;
        public int NumSelectedCols;
        public int TrackLength;
        public int[] SourceColsContent;
        public int[] SourceColsColors;
        public int OperationType;

        public short[][] ItemsTable;

        public ChildForm(Form parent) :
            base()
        {
            this.MdiParent = parent;

            InitializeComponent();

            UIActionManager.Instance.AddComponents(UIActionType.JoinTracks, JoinTracksButton);
            UIActionManager.Instance.AddEvents(UIActionType.JoinTracks, Globals.MainForm.JoinTracks_Execute, Globals.MainForm.JoinTracks_Update);

            ChannelMetrics[0] = new ChannelMetrics();
            ChannelMetrics[1] = new ChannelMetrics();
            ChannelMetrics[2] = new ChannelMetrics();

            this.TabControl.ImageList = Globals.MainForm.ImageList1;
        }

        public bool TryGetCurrentFileBrowser(out FileBrowser fileBrowser)
        {
            fileBrowser = null;

            if (TabControl.SelectedTab == SamplesTab)
            {
                fileBrowser = SamplesBrowser;
                return true;
            }

            if (TabControl.SelectedTab == OrnamentsTab)
            {
                fileBrowser = OrnamentsBrowser;
                return true;
            }

            return false;
        }

        public void FBRename_Click(object sender, EventArgs e)
        {
            if (!TryGetCurrentFileBrowser(out FileBrowser fileBrowser))
                return;

            string fileName;
            string fullPath;
            string newName;
            string newFullPath;

            fileName = fileBrowser.GetSelectedFileName();
            fullPath = fileBrowser.GetSelectedFullPath();

            if (fileName == "" || fullPath == "")
                return;

            newName = Path.GetFileNameWithoutExtension(fileName);

            do
            {
                if (!Globals.InputQuery(Application.ProductName, "Enter a New File Name", ref newName))
                    return;
            }
            while (!ValidFileName(newName));

            // Cut too long filename
            if (newName.Length > 100)
                newName = newName.Substring(0, 100);

            if (Path.GetExtension(newName) != $".{fileBrowser.FileExt}")
                newName += $".{fileBrowser.FileExt}";

            newFullPath = Path.Combine(Path.GetDirectoryName(fullPath), newName);

            if (File.Exists(newFullPath))
            {
                MessageBox.Show(this, $"File \"{newName}\" Already Exists.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            File.Move(fullPath, newFullPath);

            fileBrowser.ReadDir();

            for (int i = 0; i < fileBrowser.Items.Count; i++)
            {
                if (fileBrowser.Items[i].ToString() == newName)
                {
                    fileBrowser.SetSelected(i, true);
                    break;
                }
            }
        }

        public void FBDelete_Click(object sender, EventArgs e)
        {
            if (!TryGetCurrentFileBrowser(out FileBrowser fileBrowser))
                return;

            int index;
            string fileName;
            string fullPath;

            if (fileBrowser == null)
                return;

            fileName = fileBrowser.GetSelectedFileName();
            fullPath = fileBrowser.GetSelectedFullPath();

            if (fileName == "" || fullPath == "")
                return;

            if (MessageBox.Show(this, $"Are You Sure You Want to Delete \"{fileName}\"?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            index = fileBrowser.GetSelectedIndex();

            File.Delete(fullPath);

            fileBrowser.ReadDir();

            if (index < fileBrowser.Items.Count)
                fileBrowser.SetSelected(index, true);
            else
                fileBrowser.SetSelected(fileBrowser.Items.Count - 1, true);
        }

        public void FBNewFolder_Click(object sender, EventArgs e)
        {
            if (!TryGetCurrentFileBrowser(out FileBrowser fileBrowser))
                return;

            int index;
            string fullPath;
            string newName;

            if (fileBrowser.CurrentDir == "")
                return;

            newName = "";

            do
            {
                if (!Globals.InputQuery(Application.ProductName, "Enter Folder Name", ref newName))
                    return;
            }
            while (!ValidFileName(newName));

            fileBrowser.DontOpenItem = true;

            // Cut too long filename
            if (newName.Length > 100)
                newName = newName.Substring(0, 100);

            fullPath = Path.GetFullPath(Path.Combine(fileBrowser.CurrentDir, newName));

            if (Directory.Exists(fullPath))
            {
                MessageBox.Show(this, $"Folder \"{newName}\" Already Exists.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (fileBrowser.PathNotFound(fileBrowser.CurrentDir, false))
                return;

            if (!IsDirectoryWriteable(fileBrowser.CurrentDir))
            {
                MessageBox.Show(this, $"Folder \"{fileBrowser.CurrentDir}\" Is Not Writeable.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Directory.CreateDirectory(fullPath);

                fileBrowser.ReadDir();
                index = fileBrowser.GetIndex($"[{newName}]");
                fileBrowser.SetSelected(index != -1 ? index : 0, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Cannot Create New Folder. Error: {ex.Message}", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void FBSetQuickAccess_Click(object sender, EventArgs e)
        {
            if (!TryGetCurrentFileBrowser(out FileBrowser fileBrowser))
                return;

            string pathName;

            if (fileBrowser.CurrentDir == "")
                return;

            pathName = fileBrowser.GetSelectedFullPath();

            if (pathName == "")
                return;

            if (fileBrowser.PathNotFound(pathName, false))
                return;

            if (fileBrowser.FileExt == "vts")
                MainForm.SamplesQuickDir = pathName;

            if (fileBrowser.FileExt == "vto")
                MainForm.OrnamentsQuickDir = pathName;

            fileBrowser.DriveSelectBox.FillDiskDrives();
        }

        public void FBSaveInstrument_Click(object sender, EventArgs e)
        {
            if (!TryGetCurrentFileBrowser(out FileBrowser fileBrowser))
                return;

            string newName;
            string fullPath;

            if (fileBrowser.CurrentDir == "")
                return;

            if (fileBrowser.PathNotFound(fileBrowser.CurrentDir, false))
                return;

            if (!IsDirectoryWriteable(fileBrowser.CurrentDir))
            {
                MessageBox.Show(this, $"Folder \"{fileBrowser.CurrentDir}\" Is Not Writeable.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            newName = "";

            do
            {
                if (!Globals.InputQuery(Application.ProductName, "Enter Sample Name", ref newName))
                    return;
            }
            while (!ValidFileName(newName));

            if (fileBrowser.PathNotFound(fileBrowser.CurrentDir, false))
                return;

            // Cut too long filename
            if (newName.Length > 100)
                newName = newName.Substring(0, 100);

            fullPath = Path.Combine(fileBrowser.CurrentDir, $"{newName}.{fileBrowser.FileExt}");

            if (fileBrowser.FileExt == "vts")
                SaveSampleFile(fullPath);

            if (fileBrowser.FileExt == "vto")
                SaveOrnamentFile(fullPath);
        }

        protected override bool ProcessDialogKey(Keys keys)
        {
            if (keys == Keys.Tab)
                return false;

            return base.ProcessDialogKey(keys);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_SYSCHAR:
                    char sysChar = (char)m.WParam;

                    if (sysChar == (char)Keys.Menu)
                        this.OnClick(EventArgs.Empty);
                    break;
                case WM_ERASEBKGND:
                    m.Result = IntPtr.Zero;
                    return;
                case WM_SYSCOMMAND:
                    int cmdType = m.WParam.ToInt32();

                    if (Globals.MainForm.SnappedToRight && cmdType != SC_CLOSE)
                        return;

                    if (Globals.MainForm.MdiChildren.Length == 2 &&
                        Globals.MainForm.MdiChildren[0] is ChildForm child1 &&
                        child1.TSWindow[0] != null &&
                        cmdType != SC_CLOSE)
                        return;

                    if (Globals.MainForm.MdiChildren.Length == 3 &&
                        Globals.MainForm.MdiChildren[0] is ChildForm child2 &&
                        child2.TSWindow[1] != null &&
                        cmdType != SC_CLOSE)
                        return;

                    if (cmdType == SC_RESTORE || cmdType == SC_MAXIMIZE)
                        return;

                    if (cmdType == SC_SIZE_SIDELEFT || cmdType == SC_SIZE_TOPLEFT || cmdType == SC_SIZE_BOTTOMLEFT)
                        return;
                    break;
                case WM_WINDOWPOSCHANGED:
                    WINDOWPOS windowPos = Marshal.PtrToStructure<WINDOWPOS>(m.LParam);

                    if (MainForm.ChildsEventsBlocked || (this.Left == windowPos.x && this.Top == windowPos.y))
                    {
                        base.WndProc(ref m);
                        return;
                    }

                    base.WndProc(ref m);

                    // Drag turbosound window too
                    if (TSWindow[0] != null)
                    {
                        MainForm.ChildsEventsBlocked = true;
                        int numModule = NumModule;

                        TSWindow[0].Top = this.Top;
                        TSWindow[0].Left = this.Left + this.Width * (numModule - TSWindow[0].NumModule);

                        if (TSWindow[1] != null)
                        {
                            TSWindow[1].Top = this.Top;
                            TSWindow[1].Left = this.Left + this.Width * (numModule - TSWindow[1].NumModule);
                        }

                        MainForm.ChildsEventsBlocked = false;
                    }

                    MainForm mainForm = Globals.MainForm;

                    if (mainForm.WindowState == FormWindowState.Normal)
                    {
                        Rectangle newSize = mainForm.GetSizeForChilds(mainForm.WindowState, true);

                        if (newSize.Width != mainForm.ClientSize.Width || newSize.Height != mainForm.ClientSize.Height)
                        {
                            mainForm.RedrawOff();
                            mainForm.SetWindowSize(newSize);
                            mainForm.AutoToolBarPosition(newSize);
                            mainForm.RedrawOn();
                        }
                    }

                    return;
            }

            base.WndProc(ref m);
        }

        public bool IsMouseOverControl(Control ctrl)
        {
            bool result;
            Point point = ctrl.PointToClient(Cursor.Position);

            if (ctrl is Panel)
            {
                Rectangle rect = ctrl.ClientRectangle;
                rect = Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom + MainForm.HScrollbarSize);
                result = rect.Contains(point);
            }
            else
                result = ctrl.ClientRectangle.Contains(point);

            return result;
        }

        public int BorderSize()
        {
            return this.Width - this.ClientSize.Width;
        }

        public int OuterHeight()
        {
            return this.Height - this.ClientSize.Height;
        }

        public void SetWidth(int clientWidth, bool fixedWidth)
        {
            ClientSize = new Size(clientWidth, ClientSize.Height);

            if (!fixedWidth)
                return;

            int borderWidth = Width - ClientSize.Width;
            int outerWidth = clientWidth + borderWidth;

            MinimumSize = new Size(outerWidth, MinimumSize.Height);
            MaximumSize = new Size(outerWidth, int.MaxValue);
        }

        public void RefreshPositionsHScroll()
        {
            //int ScrollPos = PositionsScrollBox.HorizontalScroll.Value;
            //PositionsScrollBox.HorizontalScroll.Visible = false;
            //PositionsScrollBox.HorizontalScroll.Visible = true;
            //PositionsScrollBox.HorizontalScroll.Value = ScrollPos;
        }

        public void RememberChannelsPosition()
        {
            ChannelMetrics[0].BoxLeft = ChannelABox.Left;
            ChannelMetrics[0].BoxWidth = ChannelABox.Width;
            ChannelMetrics[0].ButtonWidth = ChannelAMute.Width;
            ChannelMetrics[0].ToneLeft = ChannelATone.Left;
            ChannelMetrics[0].NoiseLeft = ChannelANoise.Left;
            ChannelMetrics[0].EnvelopeLeft = ChannelAEnvelope.Left;
            ChannelMetrics[0].SoloLeft = ChannelASolo.Left;
            ChannelMetrics[1].BoxLeft = ChannelBBox.Left;
            ChannelMetrics[1].BoxWidth = ChannelBBox.Width;
            ChannelMetrics[1].ButtonWidth = ChannelBMute.Width;
            ChannelMetrics[1].ToneLeft = ChannelBTone.Left;
            ChannelMetrics[1].NoiseLeft = ChannelBNoise.Left;
            ChannelMetrics[1].EnvelopeLeft = ChannelBEnvelope.Left;
            ChannelMetrics[1].SoloLeft = ChannelBSolo.Left;
            ChannelMetrics[2].BoxLeft = ChannelCBox.Left;
            ChannelMetrics[2].BoxWidth = ChannelCBox.Width;
            ChannelMetrics[2].ButtonWidth = ChannelCMute.Width;
            ChannelMetrics[2].ToneLeft = ChannelCTone.Left;
            ChannelMetrics[2].NoiseLeft = ChannelCNoise.Left;
            ChannelMetrics[2].EnvelopeLeft = ChannelCEnvelope.Left;
            ChannelMetrics[2].SoloLeft = ChannelCSolo.Left;
        }

        public int GetBackupVersionCounter()
        {
            string filePath;

            if (IsDemoSong)
                return 0;

            if (WinFileName == "")
                return 1;

            filePath = WinFileName;

            // Is backup file opened?
            if (WinFileName.IndexOf(" ver ") != -1)
            {
                // cut ' ver 001.vt2'
                filePath = WinFileName.Substring(0, WinFileName.IndexOf(" ver ") - 1);
            }

            for (int i = 0; i < 10000; i++)
            {
                string fileName = Path.Combine(Path.GetDirectoryName(filePath), ExtractFileNameEx(filePath)) + $" ver {i:D3}.vt2";

                if (!File.Exists(fileName))
                    return i;
            }

            return 1;
        }

        public bool ModuleInPlayingWindow()
        {
            if (AY.ChipCount >= 3 && PlayingWindow[2] != null)
            {
                return PlayingWindow[2] == this
                    || PlayingWindow[2].TSWindow[0] == this
                    || PlayingWindow[2].TSWindow[1] == this;
            }
            else if (AY.ChipCount >= 2 && PlayingWindow[1] != null)
            {
                return PlayingWindow[1] == this
                    || PlayingWindow[1].TSWindow[0] == this
                    || PlayingWindow[1].TSWindow[1] == this;
            }
            else if (PlayingWindow[0] != null)
            {
                return PlayingWindow[0] == this
                    || PlayingWindow[0].TSWindow[0] == this
                    || PlayingWindow[0].TSWindow[1] == this;
            }

            return false;
        }

        public void SetModuleFreq()
        {
            if (!WaveOutAPI.IsPlaying || ModuleInPlayingWindow())
                return;

            WaveOutAPI.StopPlaying();

            AY.SetAYFreq(VTM.ChipFreq);
            AY.SetIntFreq(VTM.IntFreq);
        }

        public void ChildWin_Load(object sender, EventArgs e)
        {
            MainForm mainForm = (MainForm)this.MdiParent;
            mainForm.MdiChild_Load(this, e);
            InitFinished = false;
            IsTemplate = MainForm.SetChildAsTemplate;
            //this.FormBorderStyle = Globals.MainForm.MdiChildren.Length > 1 ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle;
            SetSizable(this, Globals.MainForm.MdiChildren.Length > 1);

            for (int i = 0; i < ChanButtons.Length; i++)
                ChanButtons[i] = new ChannelButtons();

            for (int i = 0; i < ChanButtons_s.Length; i++)
                ChanButtons_s[i] = new ChannelButtonsState();

            ChanButtons[0].Box = ChannelABox;
            ChanButtons[1].Box = ChannelBBox;
            ChanButtons[2].Box = ChannelCBox;

            ChanButtons[0].Mute_But = ChannelAMute;
            ChanButtons[0].T_But = ChannelATone;
            ChanButtons[0].N_But = ChannelANoise;
            ChanButtons[0].E_But = ChannelAEnvelope;
            ChanButtons[1].Mute_But = ChannelBMute;
            ChanButtons[1].T_But = ChannelBTone;
            ChanButtons[1].N_But = ChannelBNoise;
            ChanButtons[1].E_But = ChannelBEnvelope;
            ChanButtons[2].Mute_But = ChannelCMute;
            ChanButtons[2].T_But = ChannelCTone;
            ChanButtons[2].N_But = ChannelCNoise;
            ChanButtons[2].E_But = ChannelCEnvelope;

            ChanButtons[0].Solo_But = ChannelASolo;
            ChanButtons[1].Solo_But = ChannelBSolo;
            ChanButtons[2].Solo_But = ChannelCSolo;

            ActivateTab(0);
            NumModule = 1;
            this.AutoScroll = false;
            this.AutoSize = false;
            LastWidth = 0;
            LastHeight = 0;
            // Prevent to flickering some controls
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.Opaque, true);
            this.UpdateStyles();
            IsSynchronizing = false;
            TSWindow[0] = null;
            TSWindow[1] = null;
            WinFileName = "";
            SamplesDir = Globals.MainForm.SamplesDir;
            OrnamentsDir = Globals.MainForm.OrnamentsDir;
            SavedAsText = true;
            UndoWorking = true;
            ChangeCount = 0;
            ChangeTop = 0;
            ChangeList = new ChangeListItem[64];
            OrGenRunning = false;
            VTM = new VTM();
            PatternNumUpDown.Maximum = VTModule.MaxPatternIndex;
            PatternLenUpDown.Maximum = VTModule.MaxPatternLength;
            AutoStepUpDown.Maximum = VTModule.MaxPatternLength;
            AutoStepUpDown.Minimum = -VTModule.MaxPatternLength;
            StepHLUpDown.Maximum = VTModule.MaxPatternLength;
            SampleLengthUpDown.Maximum = VTModule.MaxSampleLength;
            SampleLoopUpDown.Maximum = VTModule.MaxSampleLength - 1;
            OrnamentLenUpDown.Maximum = VTModule.MaxOrnamentLength;
            OrnamentLoopUpDown.Maximum = VTModule.MaxOrnamentLength - 1;
            VtmFeaturesBox.SelectedIndex = (int)VTModule.FeaturesLevel;
            SaveHeaderBox.SelectedIndex = !VTModule.VortexModuleHeader ? 1 : 0;
            TrackOptsScrollBox.BorderStyle = BorderStyle.None;

            //PositionsGrid[0, 0].Value = "L0";
            PatternIndex = 0;
            SampleIndex = 1;
            OrnamentIndex = 1;
            PositionIndex = 0;
            PosBegin = 0;
            LineInts = 0;
            PosDelay = VTM.InitialDelay;
            TotInts = 0;
            AutoEnv = false;
            StdAutoEnvIndex = 0;
            AutoEnv0 = 1;
            AutoEnv1 = 1;
            AutoStep = false;

            CreateTracks();
            CreateTestLines();
            CreateSamples();
            CreateOrnaments();

            Tracks.TabIndex = 0;
            Tracks.ContextMenuStrip = Globals.MainForm.PopupMenu2;
            Samples.TabIndex = 0;
            SampleTestLine.TabIndex = 6;
            Ornaments.TabIndex = 0;
            OrnamentTestLine.TabIndex = 6;

            FBRename.Image = Globals.MainForm.ImageList1.Images[48];
            FBSetQuickAccess.Image = Globals.MainForm.ImageList1.Images[49];
            FBNewFolder.Image = Globals.MainForm.ImageList1.Images[50];
            FBDelete.Image = Globals.MainForm.ImageList1.Images[51];
            FBSaveInstrument.Image = Globals.MainForm.ImageList1.Images[52];
            UseLastNoteParamsCheckBox.Checked = MainForm.DupNoteParams;
            MoveBetweenPatternsCheckBox.Checked = MainForm.MoveBetweenPatrns;
            ToneTableUpDown.Value = Globals.MainForm.DefaultTable;
            PatternNumUpDown.Text = PatternIndex.ToString();
            /* if (MainForm.DecBaseLinesOn)
            {
                PatternLenEdit.Text = PatternLenUpDown.Value.ToString();
            }
            else
            {
                PatternLenEdit.Text = ((int)PatternLenUpDown.Value).ToString("X2");
                SampleLenEdit.Text = "01";
                OrnamentLenEdit.Text = "01";
                SampleLoopEdit.Text = "00";
                OrnamentLoopEdit.Text = "00";
            } */
            SampleNumUpDown.Text = SampleIndex.ToString();
            OrnamentNumUpDown.Text = OrnamentIndex.ToString();

            if (MainForm.DrawOffAfterClose)
                Globals.MainForm.RedrawOff();

            WidthChanged = true;
            HeightChanged = true;

            PrepareForm();
            AutoResizeForm();

            UndoWorking = false;
            SongChanged = false;
            BackupSongChanged = false;
            UpdateSpeedBPM();
            ShowStat();

            SamplesGrid.ColumnCount = 31;
            SamplesGrid.RowTemplate.Height = 64;
            SamplesGrid.Columns.Clear();
            SamplesGrid.Rows.Clear();

            for (int i = 0; i < 31; i++)
            {
                SamplesGrid.Columns.Add("", "");
                SamplesGrid.Columns[i].Width = 24;
            }

            SamplesGrid.Rows.Add();

            OrnamentsGrid.ColumnCount = 31;
            OrnamentsGrid.RowTemplate.Height = 64;
            OrnamentsGrid.Columns.Clear();
            OrnamentsGrid.Rows.Clear();

            for (int i = 0; i < 31; i++)
            {
                OrnamentsGrid.Columns.Add("", "");
                OrnamentsGrid.Columns[i].Width = 27;
            }

            OrnamentsGrid.Rows.Add();

            for (int i = 1; i < 32; i++)
            {
                char cs = (char)('0' + i);
                if (i >= 10)
                    cs = (char)(cs + 'A' - '9' - 1);

                SamplesGrid[i - 1, 0].Value = cs.ToString();
                OrnamentsGrid[i - 1, 0].Value = cs.ToString();
            }

            PositionsGrid.Selection = Rectangle.FromLTRB(0, 0, 0, 0);

            SampleToneShiftAsNoteCheckBox.Checked = MainForm.SamToneShiftAsNote;
            OrnamentToneShiftAsNoteCheckBox.Checked = MainForm.OrnToneShiftAsNote;
            UpdateSamToneShiftControls();
            UpdateOrnToneShiftControls();
        }

        public int GetPositionIndex()
        {
            return PositionIndex;
        }

        public void ResizeChannelsBox()
        {
            AutoHLBox.Width = Tracks.SeperatorX[2] - AutoHLBox.Left + 3;
            StepHLUpDown.Left = AutoHLBox.Width - StepHLUpDown.Width - 5;
            //UpDown15.Left = UpDown15.Left - UpDown15.Width;
            AutoHLCheckBox.Left = 2 + AutoLL.Width;
            AutoHLCheckBox.Width = StepHLUpDown.Left - AutoHLCheckBox.Left - 2;

            for (int i = 0; i < 3; i++)
            {
                ChanButtons[i].Box.Left = Tracks.SeperatorX[i + 2] + 4;
                ChanButtons[i].Box.Width = Tracks.SeperatorX[i + 3] - Tracks.SeperatorX[i + 2] - 1;

                ChanButtons[i].Solo_But.Left = ChanButtons[i].Box.Width - ChanButtons[i].Solo_But.Width - 5;
                ChanButtons[i].E_But.Left = ChanButtons[i].Solo_But.Left - ChanButtons[i].E_But.Width + 1;
                ChanButtons[i].N_But.Left = ChanButtons[i].E_But.Left - ChanButtons[i].N_But.Width + 1;
                ChanButtons[i].T_But.Left = ChanButtons[i].N_But.Left - ChanButtons[i].T_But.Width + 1;

                char ch = (char)('A' + i);
                ChanButtons[i].Mute_But.Left = 4;
                ChanButtons[i].Mute_But.Width = ChanButtons[i].T_But.Left - ChanButtons[i].Mute_But.Left + 1;
                ChanButtons[i].Mute_But.Text = ChanButtons[i].Mute_But.Width < 43 ? $"{ch}" : $"Mute {ch}";
            }
        }

        public void ResizeAutoStepEnvBox(float x, float b, byte btnsMargin, short aStepWidth, short aEnvWidth, bool strict)
        {
            return;
            // if strict then
            // begin
            // AutoStepBtn.Width := AStepWidth; // AutoStep
            // AutoEnvBtn.Width := AEnvWidth;  // AutoEnv
            // end
            // else
            // begin
            // AutoStepBtn.Width := Round(AStepWidth * x); // AutoStep
            // AutoEnvBtn.Width := Round(AEnvWidth * x);  // AutoEnv
            // end;
            // 
            // //SpeedButton22.Margin := (SpeedButton22.Width div 8) - 9; // AutoStep
            // //SpeedButton15.Margin := (SpeedButton15.Width div 8) - 9; // AutoEnv
            // 
            // AutoEnvBtn.Left := AutoStepBtn.Left + AutoStepBtn.Width + BtnsMargin;  // AutoEnv
            // 
            // AutoStepUpDown.Left := AutoStepBtn.Left + AutoStepBtn.Width - AutoStepUpDown.Width; // AStep updown
            // AutoStepEdit.Width := AutoStepBtn.Width - AutoStepUpDown.Width; // AStep edit
            // 
            // SpeedButton16.Width := Round(AutoEnvBtn.Width / b); // AEnv 1 btn
            // SpeedButton17.Width := SpeedButton16.Width;          // AEnv 2 btn
            // SpeedButton18.Width := SpeedButton16.Width;          // AEnv 3 btn
            // 
            // SpeedButton16.Left := AutoEnvBtn.Left;            // AEnv 1 btn
            // SpeedButton17.Left := (AutoEnvBtn.Width div 2) + AutoEnvBtn.Left - // AEnv 2 btn
            // (SpeedButton17.Width div 2);
            // SpeedButton18.Left := AutoEnvBtn.Width + AutoEnvBtn.Left - // AEnv 3 btn
            // SpeedButton18.Width;
            // 
            // SpeedButton16.Margin := (SpeedButton16.Width div 2) - 5;
            // SpeedButton18.Margin := (SpeedButton16.Width div 2) - 5;
            // 
            // AutoStepBox.Width := AutoEnvBtn.Width + AutoStepBtn.Width + (AutoStepBtn.Left * 2) + BtnsMargin;
            // 

        }

        public void FitSampleBox()
        {
            int spacer1 = SampleNumUpDown.Top - (SampleNumLabel.Top + SampleNumLabel.Height); // Label -> EditBox vertical space
            int spacer2 = spacer1 * 2; // EditBox -> Next Label vertical space
            int spacer4 = SampleBox.Width / 20;

            // Controls width
            int editWidth = PrevSampleButton.Width - spacer4;
            if (PrevSampleButton.Width < 60)
            {
                spacer4 = spacer4 - 2;
                editWidth = PrevSampleButton.Width - spacer4;
            }

            // Sample number
            //SampleNumUpDown.Width = editWidth - SampleNumUpDown.Width;
            //SampleNumUpDown.Left = SampleNumUpDown.Left + SampleNumUpDown.Width;

            // Length label
            SampleLengthLabel.Left = NextSampleButton.Left + spacer4;
            SampleLengthLabel.Top = SampleNumLabel.Top;

            // Length edit box
            SampleLengthUpDown.Left = SampleLengthLabel.Left;
            SampleLengthUpDown.Top = SampleLengthLabel.Top + SampleLengthLabel.Height + spacer1;
            //SampleLenUpDown.Width = SampleNumUpDown.Width;
            //SampleLenUpDown.Left = SampleLenUpDown.Left + SampleLenUpDown.Width;
            SampleLengthUpDown.Top = SampleLengthUpDown.Top;

            // Loop label
            SampleLoopLabel.Top = SampleNumUpDown.Top + SampleNumUpDown.Height + spacer2;
            SampleLoopLabel.Left = SampleNumLabel.Left;

            // Loop edit box
            SampleLoopUpDown.Top = SampleLoopLabel.Top + SampleLoopLabel.Height + spacer1;
            SampleLoopUpDown.Left = SampleLoopLabel.Left;
            SampleLoopUpDown.Width = SampleNumUpDown.Width;
            SampleLoopUpDown.Top = SampleLoopUpDown.Top;
            SampleLoopUpDown.Left = SampleNumUpDown.Left;

            // Copy button
            CopySampleButton.Left = SampleLengthUpDown.Left;
            CopySampleButton.Top = SampleLengthUpDown.Top + SampleLengthUpDown.Height + spacer2;
            CopySampleButton.Width = editWidth;

            // Paste button
            PasteSampleButton.Left = SampleLengthUpDown.Left;
            PasteSampleButton.Top = CopySampleButton.Top + CopySampleButton.Height + 2;
            PasteSampleButton.Width = editWidth;

            // Unloop button
            UnloopSampleButton.Top = SampleLoopUpDown.Top + SampleLoopUpDown.Height + spacer2;
            UnloopSampleButton.Left = SampleLoopUpDown.Left;
            UnloopSampleButton.Width = editWidth;

            // Clear button
            ClearSampleButton.Top = UnloopSampleButton.Top;
            ClearSampleButton.Left = PasteSampleButton.Left;
            ClearSampleButton.Width = editWidth;

            SampleBox.Height = ClearSampleButton.Top + ClearSampleButton.Height + SampleNumLabel.Top - 3;
        }

        public void PrepareForm()
        {
            InitStringGridMetrics();
            //PatEmptyBox.Width = 2500;
            TopBackgroundPanel.Top = 0;

            // Channels box
            PositionsGrid.ContextMenuStrip = Globals.MainForm.PopupMenu1;
            AutoHLBox.Top = PositionsGrid.Top + PositionsGrid.Height - 3;
            ChannelABox.Top = AutoHLBox.Top;
            ChannelBBox.Top = AutoHLBox.Top;
            ChannelCBox.Top = AutoHLBox.Top;
            Tracks.Top = AutoHLBox.Top + AutoHLBox.Height + 1;
            Tracks.Left = PositionsGrid.Left;

            TrackInfoBox.Left = -2;

            Samples.Left = 7;
            Samples.Top = 13;

            SampleEditBox.Left = 0;
            SampleEditBox.Top = SamplesTestFieldBox.Top + SamplesTestFieldBox.Height - 7;

            // Samples Testline
            LoadSampleButton.Left = SampleTestLine.Left + SampleTestLine.Width + 7;
            SaveSampleButton.Left = LoadSampleButton.Left + LoadSampleButton.Width + 4;
            LoadSampleButton.Height = SampleTestLine.Height;
            SaveSampleButton.Height = SampleTestLine.Height;

            SamplesDriveSelect.Top = 14;
            SamplesDriveSelect.Left = 9;

            SamplesBrowser.Top = SamplesDriveSelect.Top + SamplesDriveSelect.Height;
            SamplesBrowser.Left = SamplesDriveSelect.Left;

            ShowSampleBrowserButton.Top = SamplesDriveSelect.Top;
            ShowSampleBrowserButton.Left = SamplesDriveSelect.Left;
            HideSampleBrowserButton.Left = SamplesBrowser.Left;

            Ornaments.Left = 7;
            Ornaments.Top = 13;
            OrnamentBox.Top = NextPrevOrnBox.Top + NextPrevOrnBox.Height - 7;
            OrnamentsBrowserBox.Top = OrnamentBox.Top + OrnamentBox.Height - 7;

            OrnamentsDriveSelect.Top = 14;
            OrnamentsDriveSelect.Left = 9;

            OrnamentsBrowser.Top = OrnamentsDriveSelect.Top + OrnamentsDriveSelect.Height;
            OrnamentsBrowser.Left = OrnamentsDriveSelect.Left;
            ShowOrnamentBrowserButton.Top = OrnamentsDriveSelect.Top;
            ShowOrnamentBrowserButton.Left = OrnamentsDriveSelect.Left;
            HideOrnamentBrowserButton.Left = OrnamentsBrowser.Left;

            // Testline
            LoadOrnamentButton.Left = OrnamentTestLine.Left + OrnamentTestLine.Width + 7;
            SaveOrnamentButton.Left = LoadOrnamentButton.Left + LoadOrnamentButton.Width + 4;
            LoadOrnamentButton.Height = OrnamentTestLine.Height;
            SaveOrnamentButton.Height = OrnamentTestLine.Height;

            this.ClientSize = new Size(TabControl.Width, TabControl.Height);
        }

        public void AutoResizeForm()
        {
            int editWidth;
            int spacer;
            int mainWidth;
            int halfWidth;
            int sheetWidth;
            int sheetHeight = PatternsTab.Height; // - InterfaceOpts.Height;
            this.SuspendLayout();
            ToolBoxesWidth = JoinTracksBox.Left + JoinTracksBox.Width + 6;

            InitSamplesMetrics();
            Ornaments.InitMetrics();

            // PageControl & tracks width
            TabControl.Width = Tracks.Width + 8;
            if (TabControl.Width < ToolBoxesWidth)
                TabControl.Width = ToolBoxesWidth;

            mainWidth = TabControl.Width;
            halfWidth = mainWidth / 2;
            sheetWidth = PatternsTab.Width;

            if (Tracks.Width < mainWidth)
                Tracks.Width = mainWidth - 8;

            WidthChanged = mainWidth != LastWidth;

            if (HeightChanged)
            {
                AutoHLBox.Top = PositionsGrid.Top + PositionsGrid.Height - 3;
                ChannelABox.Top = AutoHLBox.Top;
                ChannelBBox.Top = AutoHLBox.Top;
                ChannelCBox.Top = AutoHLBox.Top;
                Tracks.Top = AutoHLBox.Top + AutoHLBox.Height + 1;

                // ---- Patterns editor tab -----------
                Tracks.Height = sheetHeight - Tracks.Top - InterfaceBox.Height;
                Tracks.FitNumberOfLines();
            }

            if (WidthChanged)
            {
                TopBackgroundPanel.Left = TabControl.Left;
                TopBackgroundPanel.Width = TabControl.ClientSize.Width;

                // Patterns positions
                //PositionsGrid.Width = mainWidth - 10;
                InitStringGridMetrics();

                // Channels box
                ResizeChannelsBox();
                RememberChannelsPosition();

                // Interface options (Boottom box with checkboxes)
                InterfaceBox.Width = mainWidth + 10;
                MoveBetweenPatternsCheckBox.Left = mainWidth - MoveBetweenPatternsCheckBox.Width;
                UseLastNoteParamsCheckBox.Left = halfWidth - (UseLastNoteParamsCheckBox.Width / 2);

                // Trackname & Author
                TrackInfoBox.Width = mainWidth + 20;
                TitleTextBox.Width = halfWidth - ByTextBox.Width - 2;
                AuthorTextBox.Width = TitleTextBox.Width - 1;
                AuthorTextBox.Left = mainWidth - AuthorTextBox.Width - 6; // Patterns/Author input
                ByTextBox.Left = halfWidth - 8; // by
            }

            if (HeightChanged)
            {
                TopBackgroundPanel.Height = TabControl.Top;

                // Interface options (Boottom box with checkboxes)
                InterfaceBox.Top = Tracks.Top + Tracks.Height;

                // ---- Samples editor tab ------------
                SampleBox.Top = NextPrevSampleBox.Top + NextPrevSampleBox.Height - 7;
                SampleOpts.Top = sheetHeight - SampleOpts.Height - 1;

                SampleEditBox.Height = SampleOpts.Top - SampleEditBox.Top + 7; // 732
                Samples.Height = SampleEditBox.Height - Samples.Top - 8;

                Samples.LineCount = Samples.Height / Samples.CharHeight;
            }

            if (WidthChanged)
            {
                // Samples editor box
                SampleEditBox.Width = Samples.Width + (Samples.Left * 2);
                SamplesTestFieldBox.Width = SampleEditBox.Width;

                // Samples Test Field
                if (SamplesTestFieldBox.Width < SaveSampleButton.Left + SaveSampleButton.Width)
                {
                    SamplesTestFieldBox.Width = SaveSampleButton.Left + SaveSampleButton.Width + 7;
                    SampleEditBox.Width = SamplesTestFieldBox.Width;
                    Samples.Width = SampleEditBox.Width - (Samples.Left * 2);
                }

                // PrevNextSample Box
                NextPrevSampleBox.Left = SamplesTestFieldBox.Left + SamplesTestFieldBox.Width - 2;
                NextPrevSampleBox.Width = sheetWidth - SampleEditBox.Width;
                PrevSampleButton.Left = SampleNumLabel.Left;
                PrevSampleButton.Width = ((NextPrevSampleBox.Width - (PrevSampleButton.Left * 2))) / 2;
                NextSampleButton.Left = PrevSampleButton.Left + PrevSampleButton.Width;
                NextSampleButton.Width = PrevSampleButton.Width;

                // SampleBox: length, loop, copy to, clear
                SampleBox.Left = NextPrevSampleBox.Left;
                SampleBox.Width = NextPrevSampleBox.Width;
                FitSampleBox();

                // Sample browser box
                SampleBrowserBox.Left = SampleBox.Left;
                SampleBrowserBox.Width = SampleBox.Width;

                // Samples Browser
                SamplesBrowser.Width = SampleBrowserBox.Width - 19;

                // Show Sample Browser Button
                ShowSampleBrowserButton.Width = SamplesBrowser.Width;

                // Hide Sample Browser Button
                HideSampleBrowserButton.Width = SamplesBrowser.Width - 1;

                // Disk drive combo box
                SamplesDriveSelect.Width = SamplesBrowser.Width;

                // Bottom options
                SampleOpts.Width = SampleBrowserBox.Left + SampleBrowserBox.Width;
                RecalcTonesButton.Left = SampleEditBox.Width - RecalcTonesButton.Width - 8;
                SampleSeparator2.Left = RecalcTonesButton.Left - 8;
            }

            if (HeightChanged)
            {
                // Sample browser box
                SampleBrowserBox.Top = SampleBox.Top + SampleBox.Height - 7;
                SampleBrowserBox.Height = (SampleOpts.Top + SampleOpts.Height) - SampleBrowserBox.Top;

                // Samples Browser
                SamplesBrowser.Height = SampleBrowserBox.Height - SamplesBrowser.Top - HideSampleBrowserButton.Height - 10;

                // Hide Sample Browser Button
                HideSampleBrowserButton.Top = SamplesBrowser.Top + SamplesBrowser.Height + 2;
            }

            // Visibility of samples browser and buttons
            SamplesBrowser.Visible = Globals.MainForm.SampleBrowserVisible;
            SamplesDriveSelect.Visible = SamplesBrowser.Visible;
            ShowSampleBrowserButton.Visible = !SamplesBrowser.Visible;
            HideSampleBrowserButton.Visible = SamplesBrowser.Visible;

            if (HeightChanged)
            {
                // ---- Ornaments -----
                OrnamentOpts.Top = SampleOpts.Top;
                OrnamentOpts.Height = SampleOpts.Height;

                OrnamentEditBox.Height = SampleEditBox.Height;

                Ornaments.Height = Samples.Height;
                Ornaments.NRaw = Samples.Height / Ornaments.CharHeight;
                Ornaments.LineCount = MainForm.OrnColumnCount * Ornaments.NRaw;
            }

            if (WidthChanged)
            {
                // ---- Ornaments -----
                OrnamentEditBox.Width = Ornaments.Width + Ornaments.Left * 2;
                OrnamentsTestFieldBox.Width = OrnamentEditBox.Width;

                if (OrnamentsTestFieldBox.Width < SaveOrnamentButton.Left + SaveOrnamentButton.Width + 10)
                {
                    OrnamentsTestFieldBox.Width = SaveOrnamentButton.Left + SaveOrnamentButton.Width + 10;
                    OrnamentEditBox.Width = OrnamentsTestFieldBox.Width;
                    Ornaments.Width = OrnamentEditBox.Width - Ornaments.Left - Ornaments.Left;
                }

                // Prev/Next ornament box
                NextPrevOrnBox.Left = OrnamentsTestFieldBox.Width + OrnamentsTestFieldBox.Left - 2;
                NextPrevOrnBox.Width = sheetWidth - OrnamentEditBox.Width;
                PrevOrnamentButton.Left = OrnamentNumLabel.Left;
                PrevOrnamentButton.Width = (NextPrevOrnBox.Width - (PrevOrnamentButton.Left * 2)) / 2;

                if (PrevOrnamentButton.Width > 110)
                    PrevOrnamentButton.Width = 110;

                NextOrnamentButton.Left = PrevOrnamentButton.Left + PrevOrnamentButton.Width;
                NextOrnamentButton.Width = PrevOrnamentButton.Width;

                // Ornament Box
                OrnamentBox.Left = NextPrevOrnBox.Left;
                OrnamentBox.Width = NextPrevOrnBox.Width;

                spacer = (PrevOrnamentButton.Width * 2) / 18;
                editWidth = NextOrnamentButton.Width - spacer;
                //OrnamentNumUpDown.Width = editWidth - OrnamentNumUpDown.Width;
                //OrnamentNumUpDown.Left = OrnamentNumUpDown.Left + OrnamentNumUpDown.Width;

                OrnamentLoopUpDown.Width = OrnamentNumUpDown.Width;
                OrnamentLoopUpDown.Left = OrnamentNumUpDown.Left;

                OrnamentLengthLabel.Left = NextOrnamentButton.Left + spacer;
                OrnamentLenUpDown.Left = OrnamentLengthLabel.Left;
                //OrnamentLenUpDown.Width = OrnamentNumUpDown.Width;
                //OrnamentLenUpDown.Left = OrnamentLenUpDown.Left + OrnamentLenUpDown.Width;

                CopyOrnamentButton.Left = OrnamentLengthLabel.Left;
                CopyOrnamentButton.Width = editWidth;
                PasteOrnamentButton.Left = CopyOrnamentButton.Left;
                PasteOrnamentButton.Width = editWidth;
                ClearOrnamentButton.Width = PasteOrnamentButton.Left + PasteOrnamentButton.Width - OrnamentLoopUpDown.Left;

                OrnamentsBrowserBox.Left = OrnamentBox.Left;
                OrnamentsBrowserBox.Width = OrnamentBox.Width;

                // Ornaments Browser
                OrnamentsBrowser.Width = OrnamentsBrowserBox.Width - 19;
                if (OrnamentsBrowser.Width > ClearOrnamentButton.Left + ClearOrnamentButton.Width)
                {
                    OrnamentsBrowser.Width = ClearOrnamentButton.Width;
                    OrnamentsBrowser.Left = ClearOrnamentButton.Left;
                }
                else
                    OrnamentsBrowser.Left = 9;

                OrnamentsDriveSelect.Width = OrnamentsBrowser.Width;
                OrnamentsDriveSelect.Left = OrnamentsBrowser.Left;

                // Show Ornaments Browser Button
                ShowOrnamentBrowserButton.Width = OrnamentsBrowser.Width;
                ShowOrnamentBrowserButton.Left = OrnamentsBrowser.Left;

                // Hide Ornaments Browser Button
                HideOrnamentBrowserButton.Width = OrnamentsBrowser.Width;
                HideOrnamentBrowserButton.Left = OrnamentsBrowser.Left;
                OrnamentOpts.Width = OrnamentsBrowserBox.Left + OrnamentsBrowserBox.Width;

                // ------- OPTIONS TAB --------
                TrackOptsScrollBox.Width = OptionsTab.Width;

                ChipFreqBox.Width = TrackOptsScrollBox.ClientSize.Width - ChipFreqBox.Left - MainForm.VScrollbarSize - 3;
                IntFreqBox.Width = TrackOptsScrollBox.ClientSize.Width - IntFreqBox.Left - MainForm.VScrollbarSize - 3;

                VtmFeaturesBox.Width = (TrackOptsScrollBox.ClientSize.Width / 2) - 9 - VtmFeaturesBox.Left;

                SaveHeaderBox.Left = VtmFeaturesBox.Left + VtmFeaturesBox.Left + VtmFeaturesBox.Width + 9;
                SaveHeaderBox.Width = TrackOptsScrollBox.ClientSize.Width - SaveHeaderBox.Left - MainForm.VScrollbarSize - 3;

                ManualHz.Left = ChipFreqBox.Buttons[20].Left + 95;
                ManualIntFreq.Left = IntFreqBox.Buttons[6].Left + 95;

                // --- INFO TAB -------
                TrackInfoGB.Width = InfoTab.Width - (TrackInfoGB.Left * 2);
                TrackInfoPanel.Width = TrackInfoGB.ClientSize.Width - (TrackInfoPanel.Left * 2);
                ViewInfoButton.Left = TrackInfoPanel.Left + TrackInfoPanel.Width - ViewInfoButton.Width - 2;
            }

            if (HeightChanged)
            {
                TrackOptsScrollBox.Height = OptionsTab.Height - TrackOptsScrollBox.Top;

                // Ornaments Browser Box
                OrnamentsBrowserBox.Top = OrnamentBox.Top + OrnamentBox.Height - 7;
                OrnamentsBrowserBox.Height = OrnamentOpts.Top + OrnamentOpts.Height - OrnamentsBrowserBox.Top;

                // Ornaments Browser
                OrnamentsBrowser.Top = SamplesBrowser.Top;
                OrnamentsBrowser.Height = SamplesBrowser.Height;

                // Hide Ornaments Browser Button
                HideOrnamentBrowserButton.Top = HideSampleBrowserButton.Top;

                // --- INFO TAB -------
                TrackInfoGB.Height = InfoTab.Height - TrackInfoGB.Top - 1;
                TrackInfoPanel.Height = TrackInfoGB.ClientSize.Height - TrackInfoPanel.Top - TrackInfoPanel.Left;
            }

            // Visibility of ornaments browser and buttons
            OrnamentsBrowser.Visible = Globals.MainForm.OrnamentsBrowserVisible;
            OrnamentsDriveSelect.Visible = OrnamentsBrowser.Visible;
            ShowOrnamentBrowserButton.Visible = !OrnamentsBrowser.Visible;
            HideOrnamentBrowserButton.Visible = OrnamentsBrowser.Visible;

            LastWidth = TabControl.Width;
            LastHeight = TabControl.Height;

            ResetChanAlloc();

            this.ResumeLayout();
        }

        public int WhereIsChannel(int num)
        {
            for (int i = 0; i < 3; i++)
            {
                if (MainForm.ChanAlloc[i] == num)
                    return i;
            }

            return 0;
        }

        public void ResetChanAlloc()
        {
            if (Tracks.Focused)
                Tracks.HideCaret();
            Tracks.RemoveSelection();
            Tracks.RedrawTracks();
            if (Tracks.Focused)
            {
                Tracks.SetCaretPosition();
                Tracks.ShowCaret();
            }

            ChannelMetrics channelMetricsA = ChannelMetrics[WhereIsChannel(0)];
            ChannelABox.Left = channelMetricsA.BoxLeft;
            ChannelABox.Width = channelMetricsA.BoxWidth;
            ChannelAMute.Width = channelMetricsA.ButtonWidth;
            ChannelATone.Left = channelMetricsA.ToneLeft;
            ChannelANoise.Left = channelMetricsA.NoiseLeft;
            ChannelAEnvelope.Left = channelMetricsA.EnvelopeLeft;
            ChannelASolo.Left = channelMetricsA.SoloLeft;

            ChannelMetrics channelMetricsB = ChannelMetrics[WhereIsChannel(1)];
            ChannelBBox.Left = channelMetricsB.BoxLeft;
            ChannelBBox.Width = channelMetricsB.BoxWidth;
            ChannelBMute.Width = channelMetricsB.ButtonWidth;
            ChannelBTone.Left = channelMetricsB.ToneLeft;
            ChannelBNoise.Left = channelMetricsB.NoiseLeft;
            ChannelBEnvelope.Left = channelMetricsB.EnvelopeLeft;
            ChannelBSolo.Left = channelMetricsB.SoloLeft;

            ChannelMetrics channelMetricsC = ChannelMetrics[WhereIsChannel(2)];
            ChannelCBox.Left = channelMetricsC.BoxLeft;
            ChannelCBox.Width = channelMetricsC.BoxWidth;
            ChannelCMute.Width = channelMetricsC.ButtonWidth;
            ChannelCTone.Left = channelMetricsC.ToneLeft;
            ChannelCNoise.Left = channelMetricsC.NoiseLeft;
            ChannelCEnvelope.Left = channelMetricsC.EnvelopeLeft;
            ChannelCSolo.Left = channelMetricsC.SoloLeft;
        }

        public void CreateTracks()
        {
            Tracks = new Tracks(PatternsTab);
            Tracks.InitMetrics();
            Tracks.ParentWin = this;
            Tracks.BackColor = MainForm.ColorTheme.Colors[(int)ThemeColor.Background];

            if (MainForm.DisableHints)
                ToolTip.Hide(Tracks);
            else
                ToolTip.SetToolTip(Tracks, "Tracks");

            //Tracks.ShowHint = !MainForm.DisableHints;
            Tracks.KeyDown += Tracks_KeyDown;
            Tracks.KeyUp += Tracks_KeyUp;
            Tracks.Leave += Tracks_Leave;
            Tracks.MouseDown += Tracks_MouseDown;
            Tracks.MouseMove += Tracks_MouseMove;
            Tracks.MouseWheel += Tracks_MouseWheel;
            Tracks.Show();
        }

        public void CreateTestLines()
        {
            using (Graphics g = Graphics.FromHwnd(this.Handle))
            {
                Size sz;
                SampleTestLine = new TestLine(SamplesTestFieldBox);
                SampleTestLine.BackColor = System.Drawing.Color.White;
                SampleTestLine.ParentWin = this;
                SampleTestLine.TestSample = true;
                SampleTestLine.CharHeight = Math.Abs(SampleTestLine.Font.Height);
                sz = TextRenderer.MeasureText(g, "0", SampleTestLine.Font, Size.Empty, TextFormatFlags.NoPadding);
                SampleTestLine.CharWidth = sz.Width;
                SampleTestLine.CharHeight = sz.Height;

                SampleTestLine.Left = 7;
                SampleTestLine.Top = LoadSampleButton.Top;
                SampleTestLine.ClientSize = new Size(SampleTestLine.CharWidth * 21, SampleTestLine.CharHeight);
                SampleTestLine.KeyDown += SampleTestLine.TestLine_KeyDown;
                SampleTestLine.KeyUp += SampleTestLine.TestLine_KeyUp;
                SampleTestLine.Leave += SampleTestLine.TestLine_Leave;
                SampleTestLine.MouseDown += SampleTestLine.TestLine_MouseDown;

                SampleTestLine.Show();

                OrnamentTestLine = new TestLine(OrnamentsTestFieldBox);
                OrnamentTestLine.BackColor = System.Drawing.Color.White;
                OrnamentTestLine.ParentWin = this;
                OrnamentTestLine.TestSample = false;
                sz = TextRenderer.MeasureText(g, "0", OrnamentTestLine.Font, Size.Empty, TextFormatFlags.NoPadding);
                OrnamentTestLine.CharWidth = sz.Width;
                OrnamentTestLine.CharHeight = sz.Height;

                OrnamentTestLine.Left = 7;
                OrnamentTestLine.Top = LoadOrnamentButton.Top;
                OrnamentTestLine.ClientSize = new Size(OrnamentTestLine.CharWidth * 21, OrnamentTestLine.CharHeight);
                OrnamentTestLine.KeyDown += OrnamentTestLine.TestLine_KeyDown;
                OrnamentTestLine.KeyUp += OrnamentTestLine.TestLine_KeyUp;
                OrnamentTestLine.Leave += OrnamentTestLine.TestLine_Leave;
                OrnamentTestLine.MouseDown += OrnamentTestLine.TestLine_MouseDown;

                OrnamentTestLine.Show();
            }
        }

        public void InitSamplesMetrics()
        {
            string[] specFonts = { "ProTracker 2", "WST_Germ", "ZX Spectrum" };

            using (Graphics g = Graphics.FromHwnd(Samples.Handle))
            {
                int i;
                Size sz;
                Samples.Font = Globals.MainForm.EditorFont;
                sz = TextRenderer.MeasureText(g, "0", Samples.Font, Size.Empty, TextFormatFlags.NoPadding);
                Samples.CharWidth = sz.Width;
                Samples.CharHeight = sz.Height;

                i = 0;
                do
                {
                    if (MainForm.TryGetFontFamily("Arrows", out FontFamily fontFamily))
                        Samples.ArrowsFont = new Font(fontFamily, Samples.Font.Size - i);
                    sz = TextRenderer.MeasureText(g, "0", Samples.ArrowsFont, Size.Empty, TextFormatFlags.NoPadding);
                    Samples.ArrowsFontW = sz.Width;
                    Samples.ArrowsFontH = sz.Height;
                    i++;
                }
                while (Samples.ArrowsFontW >= Samples.CharWidth - 1);

                SamplesBrowser.Font = new Font(SamplesBrowser.Font.Name, Math.Clamp(Globals.MainForm.EditorFont.Size - 9, 10, 12));
                sz = TextRenderer.MeasureText(g, "0", SamplesBrowser.Font, Size.Empty, TextFormatFlags.NoPadding);
                Samples.ClientSize = new Size(Samples.CharWidth * 40, Samples.ClientSize.Height);

                if (Samples.ClientSize.Width < 400)
                    Samples.ClientSize = new Size(400, Samples.ClientSize.Height);
            }
        }

        public void CreateSamples()
        {
            Samples = new Samples(SampleEditBox);
            Samples.BackColor = MainForm.ColorTheme.Colors[(int)ThemeColor.Background];
            Samples.ParentWin = this;
            Samples.UndoSaved = false;

            if (MainForm.DisableHints)
                ToolTip.Hide(Samples);
            else
                ToolTip.SetToolTip(Samples, "Samples");

            //Samples.ShowHint = !MainForm.DisableHints;
            Samples.KeyDown += Samples_KeyDown;
            Samples.KeyUp += Samples_KeyUp;
            Samples.MouseDown += Samples_MouseDown;
            Samples.MouseUp += Samples_MouseUp;
            Samples.MouseMove += Samples_MouseMove;
            Samples.MouseWheel += Samples_MouseWheel;
            //Samples.OnMouseWheelDown += SamplesMouseWheelDown;

            SamplesBrowser = new FileBrowser(SampleBrowserBox);
            SamplesBrowser.ParentWin = this;
            SamplesBrowser.FileExt = "vts";
            SamplesBrowser.CurrentDir = Globals.MainForm.SamplesDir;
            SamplesBrowser.OnFileSelected += SamplesBrowser_OnFileSelected;
            SamplesBrowser.ReadDir();
            //SamplesBrowser.DrawItem += SamplesBrowser.OnDrawItem;
            //SamplesBrowser.MouseDown += SamplesBrowser.OnMouseDown;
            //SamplesBrowser.Click += SamplesBrowser.OnClick;
            //SamplesBrowser.DoubleClick += SamplesBrowser.OnDoubleClick;
            //SamplesBrowser.KeyUp += SamplesBrowser.OnKeyUp;
            //SamplesBrowser.KeyDown += SamplesBrowser.OnKeyDown;
            //SamplesBrowser.MouseMove += SamplesBrowser.OnMouseMove;
            SamplesBrowser.ContextMenuStrip = FileBrowserPopup;
            SamplesDriveSelect = new DriveSelect(SampleBrowserBox);
            SamplesDriveSelect.FileBrowser = SamplesBrowser;

            //SamplesDriveSelect.SelectedIndexChanged += SamplesDriveSelect.OnSelectedIndexChanged;
            //SamplesDriveSelect.DrawItem += SamplesDriveSelect.OnDrawItem;
            SamplesBrowser.DriveSelectBox = SamplesDriveSelect;

            InitSamplesMetrics();

            Samples.Show();
            SamplesBrowser.Show();
        }

        private void SamplesBrowser_OnFileSelected(object? sender, FileSelectedEventArgs e)
        {
            SamplesBrowser.PreviewPlaying = e.IsPreview;

            if (e.IsPreview)
            {
                LoadSample(e.FilePath, VTModule.PreviewSampleIndex);

                SampleTestLine.Preview = e.IsPreview;
                SampleTestLine.PlayCurrentNote();
                PlayStopTimer.Enabled = true;
            }
            else
                LoadSample(e.FilePath);
        }

        public void CreateOrnaments()
        {
            Ornaments = new Ornaments(OrnamentEditBox);
            Ornaments.BackColor = MainForm.ColorTheme.Colors[(int)ThemeColor.Background];
            Ornaments.ParentWin = this;

            if (MainForm.DisableHints)
                ToolTip.Hide(Ornaments);
            else
                ToolTip.SetToolTip(Ornaments, "Ornaments");

            //Ornaments.ShowHint += !MainForm.DisableHints;
            Ornaments.KeyDown += Ornaments_KeyDown;
            Ornaments.KeyUp += Ornaments_KeyUp;
            Ornaments.MouseUp += Ornaments_MouseUp;
            Ornaments.MouseDown += Ornaments_MouseDown;
            Ornaments.MouseMove += Ornaments_MouseMove;
            Ornaments.MouseWheel += Ornaments_MouseWheel;
            //Ornaments.OnMouseWheelDown += OrnamentsMouseWheelDown;
            NoteCounter = 0;
            MaxNote = 0;

            Ornaments.Show();

            OrnamentsBrowser = new FileBrowser(OrnamentsBrowserBox);
            OrnamentsBrowser.ParentWin = this;
            OrnamentsBrowser.FileExt = "vto";
            OrnamentsBrowser.CurrentDir = Globals.MainForm.OrnamentsDir;
            OrnamentsBrowser.OnFileSelected += OrnamentsBrowser_OnFileSelected;
            OrnamentsBrowser.ReadDir();
            //OrnamentsBrowser.DrawItem += OrnamentsBrowser.OnDrawItem;
            //OrnamentsBrowser.MouseDown += OrnamentsBrowser.OnMouseDown;
            //OrnamentsBrowser.Click += OrnamentsBrowser.OnClick;
            //OrnamentsBrowser.DoubleClick += OrnamentsBrowser.OnDoubleClick;
            //OrnamentsBrowser.KeyUp += OrnamentsBrowser.OnKeyUp;
            //OrnamentsBrowser.KeyDown += OrnamentsBrowser.OnKeyDown;
            //OrnamentsBrowser.MouseMove += OrnamentsBrowser.OnMouseMove;
            OrnamentsBrowser.ContextMenuStrip = FileBrowserPopup;

            OrnamentsDriveSelect = new DriveSelect(OrnamentsBrowserBox);
            OrnamentsDriveSelect.FileBrowser = OrnamentsBrowser;
            //OrnamentsDriveSelect.SelectedIndexChanged += OrnamentsDriveSelect.OnSelectedIndexChanged;
            //OrnamentsDriveSelect.DrawItem += OrnamentsDriveSelect.OnDrawItem;
            OrnamentsBrowser.DriveSelectBox = OrnamentsDriveSelect;

            Ornaments.Browser = OrnamentsBrowser;
            Ornaments.InitMetrics();

            Ornaments.Show();
            OrnamentsBrowser.Show();
        }

        private void OrnamentsBrowser_OnFileSelected(object? sender, FileSelectedEventArgs e)
        {
            OrnamentsBrowser.PreviewPlaying = e.IsPreview;

            if (e.IsPreview)
            {
                LoadOrnament(e.FilePath, VTModule.PreviewOrnamentIndex);

                OrnamentTestLine.Preview = e.IsPreview;
                OrnamentTestLine.PlayCurrentNote();
                PlayStopTimer.Enabled = true;
            }
            else
                LoadOrnament(e.FilePath);
        }

        public void ChangeNote(int pattern, int line, int channel, int note)
        {
            bool noteChanged = VTM.Patterns[pattern].Lines[line].Channel[channel].Note != note;

            if (noteChanged)
            {
                SongChanged = true;
                BackupSongChanged = true;
            }

            if (VTM.Patterns[pattern].Lines[line].Channel[channel].Note >= 0)
                VTModule.PlayArgs[0].ChannelParams[channel].Note = (byte)VTM.Patterns[pattern].Lines[line].Channel[channel].Note;

            if (!UndoWorking && noteChanged)
            {
                if (UseLastNoteParamsCheckBox.Checked)
                {
                    AddUndo(TChangeAction.ChangeNoteAndParams, note, 0);
                    ChangeList[ChangeCount - 1].Line = line;
                    ChangeList[ChangeCount - 1].Channel = channel;
                }
                else
                {
                    AddUndo(TChangeAction.ChangeNote, VTM.Patterns[pattern].Lines[line].Channel[channel].Note, note);
                    ChangeList[ChangeCount - 1].Line = line;
                    ChangeList[ChangeCount - 1].Channel = channel;
                }
            }

            VTM.Patterns[pattern].Lines[line].Channel[channel].Note = (sbyte)note;
        }

        public void ChangeTracks(int pattern, int line, int channel, int cursorX, int n, bool keyboard)
        {
            int old = 0;
            int r;
            StringBuilder stringBuilder;
            string newStr;

            switch (cursorX)
            {
                // 0 .. 3
                case 0:
                case 1:
                case 2:
                case 3:
                    old = VTM.Patterns[pattern].Lines[line].Envelope;
                    if (keyboard)
                    {
                        r = 4 * (3 - cursorX);
                        n = (old & (0xFFFF ^ (15 << r))) | ((n & 15) << r);
                    }
                    break;
                // 5 .. 6
                case 5:
                case 6:
                    old = VTM.Patterns[pattern].Lines[line].Noise;
                    if (keyboard)
                    {
                        if (MainForm.DecBaseNoiseOn)
                        {
                            // StrPLCopy(oldArr, Format('%.2d', [old]), High(oldArr));
                            stringBuilder = new StringBuilder(string.Format("%.2d", old));
                            newStr = n.ToString();

                            if (cursorX == 5)
                                stringBuilder[1] = newStr[1];
                            else
                                stringBuilder[2] = newStr[1];

                            n = Convert.ToInt32(stringBuilder.ToString());

                            if (n > 31)
                                n = 31;
                        }
                        else
                        {
                            r = 4 * (6 - cursorX);
                            n = (old & (0xFF ^ (15 << r))) | ((n & 15) << r);
                        }
                    }
                    break;
                // 19 .. 20, 33 .. 34, 47 .. 48
                case 19:
                case 20:
                case 33:
                case 34:
                case 47:
                case 48:
                    old = VTM.Patterns[pattern].Lines[line].Channel[channel].AdditionalCommand.Parameter;

                    if (keyboard)
                        n = (cursorX & 1) != 0 ? (old & 15) | (n << 4) : (old & 0xF0) | n;
                    break;
            }

            if (!UndoWorking)
            {
                switch (cursorX)
                {
                    // 0 .. 3
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        if (old != n)
                            AddUndo(TChangeAction.ChangeEnvelopePeriod, old, n);
                        break;
                    // 5 .. 6
                    case 5:
                    case 6:
                        if (old != n)
                            AddUndo(TChangeAction.ChangeNoise, old, n);
                        break;
                    case 12:
                    case 26:
                    case 40:
                        old = VTM.Patterns[pattern].Lines[line].Channel[channel].Sample;
                        if (old != n)
                            AddUndo(TChangeAction.ChangeSample, old, n);
                        break;
                    case 13:
                    case 27:
                    case 41:
                        old = VTM.Patterns[pattern].Lines[line].Channel[channel].Envelope;
                        if (old != n)
                            AddUndo(TChangeAction.ChangeEnvelopeType, old, n);
                        break;
                    case 14:
                    case 28:
                    case 42:
                        old = VTM.Patterns[pattern].Lines[line].Channel[channel].Ornament;
                        if (old != n)
                            AddUndo(TChangeAction.ChangeOrnament, old, n);
                        break;
                    case 15:
                    case 29:
                    case 43:
                        old = VTM.Patterns[pattern].Lines[line].Channel[channel].Volume;
                        if (old != n)
                            AddUndo(TChangeAction.ChangeVolume, old, n);
                        break;
                    case 17:
                    case 31:
                    case 45:
                        old = VTM.Patterns[pattern].Lines[line].Channel[channel].AdditionalCommand.Number;
                        if (old != n)
                            AddUndo(TChangeAction.ChangeSpecialCommandNumber, old, n);
                        break;
                    case 18:
                    case 32:
                    case 46:
                        old = VTM.Patterns[pattern].Lines[line].Channel[channel].AdditionalCommand.Delay;
                        if (old != n)
                            AddUndo(TChangeAction.ChangeSpecialCommandDelay, old, n);
                        break;
                    // 19 .. 20, 33 .. 34, 47 .. 48
                    case 19:
                    case 20:
                    case 33:
                    case 34:
                    case 47:
                    case 48:
                        if (old != n)
                            AddUndo(TChangeAction.ChangeSpecialCommandParameter, old, n);
                        break;
                }

                if (old != n)
                {
                    if (cursorX > 6)
                        ChangeList[ChangeCount - 1].Channel = channel;

                    ChangeList[ChangeCount - 1].Line = line;
                }
            }

            if (old != n)
            {
                SongChanged = true;
                BackupSongChanged = true;
            }

            switch (cursorX)
            {
                // 0 .. 3
                case 0:
                case 1:
                case 2:
                case 3:
                    VTM.Patterns[pattern].Lines[line].Envelope = (ushort)n;
                    break;
                // 5 .. 6
                case 5:
                case 6:
                    VTM.Patterns[pattern].Lines[line].Noise = (byte)n;
                    break;
                case 12:
                case 26:
                case 40:
                    VTM.Patterns[pattern].Lines[line].Channel[channel].Sample = (byte)n;
                    if (Tracks.LastNoteParams != null && Tracks.LastNoteParams[channel].Line != line)
                        Tracks.ResetLastNoteParams((byte)pattern, (byte)line, (byte)channel);
                    if (Tracks.LastNoteParams != null)
                        Tracks.LastNoteParams[channel].Sample = (byte)n;
                    break;
                case 13:
                case 27:
                case 41:
                    VTM.Patterns[pattern].Lines[line].Channel[channel].Envelope = (byte)n;
                    if (Tracks.LastNoteParams != null && Tracks.LastNoteParams[channel].Line != line)
                        Tracks.ResetLastNoteParams((byte)pattern, (byte)line, (byte)channel);
                    Tracks.LastNoteParams[channel].Envelope = (byte)n;
                    break;
                case 14:
                case 28:
                case 42:
                    VTM.Patterns[pattern].Lines[line].Channel[channel].Ornament = (byte)n;
                    if (Tracks.LastNoteParams[channel].Line != line)
                        Tracks.ResetLastNoteParams((byte)pattern, (byte)line, (byte)channel);
                    Tracks.LastNoteParams[channel].Ornament = (byte)n;
                    break;
                case 15:
                case 29:
                case 43:
                    VTM.Patterns[pattern].Lines[line].Channel[channel].Volume = (sbyte)n;
                    if (Tracks.LastNoteParams[channel].Line != line)
                        Tracks.ResetLastNoteParams((byte)pattern, (byte)line, (byte)channel);
                    Tracks.LastNoteParams[channel].Volume = (sbyte)n;
                    break;
                case 17:
                case 31:
                case 45:
                    if (old != n)
                    {
                        VTM.Patterns[pattern].Lines[line].Channel[channel].AdditionalCommand.Number = (byte)n;
                        CalcTotLen();
                    }
                    break;
                case 18:
                case 32:
                case 46:
                    VTM.Patterns[pattern].Lines[line].Channel[channel].AdditionalCommand.Delay = (byte)n;
                    break;
                // 19 .. 20, 33 .. 34, 47 .. 48
                case 19:
                case 20:
                case 33:
                case 34:
                case 47:
                case 48:
                    if (old != n)
                    {
                        VTM.Patterns[pattern].Lines[line].Channel[channel].AdditionalCommand.Parameter = (byte)n;
                        CalcTotLen();
                    }
                    break;
            }
        }

        public void TLArpMidiOn(int note)
        {
            NoteCounter = NoteCounter + 1;
            Arp[note] = 1;
            OrnamentTestLine.TestLineMidiOn(note);
            if (MaxNote < NoteCounter)
                MaxNote = NoteCounter;
        }

        public void ClearArp(int note)
        {
            for (int f = 0; f < 97; f++)
                Arp[f] = 0;

            MaxNote = 0;
        }

        public void TLArpMidiOff(int note)
        {
            int f;
            int min, len;
            int[] orn = new int[97];

            NoteCounter = NoteCounter - 1;
            OrnamentTestLine.TestLineMidiOff(note);
            if (NoteCounter != 0)
                return;

            if (MaxNote < 3)
            {
                ClearArp(note);
                return;
            }

            min = 96;
            len = 0;

            for (f = 0; f <= 96; f++)
            {
                if (Arp[f] == 1)
                {
                    if (min > f)
                        min = f;

                    orn[len] = f;
                    len = len + 1;
                }
            }
            // if len < 3 then
            // begin
            // ClearArp;
            // Exit;
            // end;

            for (f = 0; f < len; f++)
                orn[f] = orn[f] - min;

            ValidateOrnament(OrnamentIndex);

            ClearShownOrnament();
            Ornaments.ShownOrnament.Length = len;
            Ornaments.ShownOrnament.Loop = 0;

            for (f = 0; f < len; f++)
                Ornaments.ShownOrnament.Offsets[f] = (sbyte)orn[f];

            OrnamentLenUpDown.Value = Ornaments.ShownOrnament.Length;
            OrnamentLoopUpDown.Value = Ornaments.ShownOrnament.Loop;

            Ornaments.HideCaret();
            Ornaments.Redraw();
            Ornaments.ShowCaret();

            // clear Arp when done
            ClearArp(note);
        }

        public void OrnamentsMidiNoteOn(byte note)
        {
            Ornaments.IsLineTesting = true;
            OrnamentTestLine.KeyPressed = 0;
            OrnamentTestLine.CursorX = 8;

            if (VTM.ReservedPattern.Lines[0].Channel[0].Note >= 0)
            {
                if (!WaveOutAPI.IsPlaying || (AY.PlayMode == PlayModes.PlayLine))
                    VTModule.PlayArgs[0].ChannelParams[VTModule.CenterChannel].Note = (byte)VTM.ReservedPattern.Lines[0].Channel[0].Note;
            }

            VTM.ReservedPattern.Lines[0].Channel[0].Note = (sbyte)note;
            DoAutoEnv(-1, 0, 0);

            HideCaret(OrnamentTestLine.Handle);
            OrnamentTestLine.Redraw();
            ShowCaret(OrnamentTestLine.Handle);

            RestartPlayingLine(-(OrnamentTestLine.TestSample ? 1 : 0) - 1);
            Ornaments.CurrentMidiNote = note;
            PlayStopState = PlayStopState.Stop;

            if (Ornaments.ToneShiftAsNote)
            {
                Ornaments.HideCaret();
                Ornaments.Redraw();
                Ornaments.ShowCaret();
            }
        }

        public void OrnamentsMidiNoteOff(byte note)
        {
            if (Ornaments.CurrentMidiNote != note)
                return;

            OrnamentTestLine.TestLine_Leave(this, EventArgs.Empty);
            Ornaments.IsLineTesting = false;

            WaveOutAPI.ResetPlaying();

            PlayStopState = PlayStopState.Play;
        }

        public void SamplesMidiNoteOn(byte note)
        {
            Samples.IsLineTesting = true;
            SampleTestLine.KeyPressed = 0;
            SampleTestLine.CursorX = 8;

            if (VTM.ReservedPattern.Lines[SampleTestLine.TestSample ? 1 : 0].Channel[0].Note >= 0)
            {
                if (!WaveOutAPI.IsPlaying || AY.PlayMode == PlayModes.PlayLine)
                    VTModule.PlayArgs[0].ChannelParams[VTModule.CenterChannel].Note = (byte)VTM.ReservedPattern.Lines[SampleTestLine.TestSample ? 1 : 0].Channel[0].Note;
            }

            VTM.ReservedPattern.Lines[SampleTestLine.TestSample ? 1 : 0].Channel[0].Note = (sbyte)note;

            DoAutoEnv(-1, SampleTestLine.TestSample ? 1 : 0, 0);
            HideCaret(SampleTestLine.Handle);

            SampleTestLine.Redraw();

            ShowCaret(SampleTestLine.Handle);

            Samples.HideCaret();
            Samples.Redraw();
            Samples.ShowCaret();

            RestartPlayingLine(-(SampleTestLine.TestSample ? 1 : 0) - 1);

            PlayStopState = PlayStopState.Stop;

            Samples.CurrentMidiNote = note;
        }

        public void SamplesMidiNoteOff(byte note)
        {
            if (Samples.CurrentMidiNote != note)
                return;

            SampleTestLine.TestLine_Leave(this, EventArgs.Empty);
            Samples.IsLineTesting = false;

            WaveOutAPI.ResetPlaying();

            PlayStopState = PlayStopState.Play;
        }

        public void TracksMidiNoteOn_DuplicateMidiNoteParams(byte pat, byte line, byte chan)
        {
            if (!UseLastNoteParamsCheckBox.Checked)
                return;

            ChannelLine channelLine = VTM.Patterns[pat].Lines[line].Channel[chan];

            if (channelLine.Sample != 0 || channelLine.Envelope != 0 || channelLine.Ornament != 0 || channelLine.Volume != 0)
                return;

            channelLine.Sample = Tracks.LastNoteParams[chan].Sample;
            channelLine.Envelope = Tracks.LastNoteParams[chan].Envelope;
            channelLine.Ornament = Tracks.LastNoteParams[chan].Ornament;
            channelLine.Volume = Tracks.LastNoteParams[chan].Volume;
        }

        public void TracksMidiNoteOn(short note)
        {
            int i, j, n, y, e, old;

            if (note < 0 || note > 96)
                return;

            if (Tracks.CurrentMidiNote == note)
                return;

            if (WaveOutAPI.IsPlaying)
            {
                WaveOutAPI.ResetPlaying();
                VTModule.UnlimitedDelay = false;
            }

            e = 0;

            if (ChanPoses.Contains(Tracks.CursorX))
            {
                ValidatePattern2(PatternIndex);
                i = Tracks.CurrentPatternLine();

                if (i >= 0 && i < Tracks.ShownPattern.Length)
                {
                    j = MainForm.ChanAlloc[(Tracks.CursorX - 8) / 14];
                    Tracks.CurrentMidiNote = note;

                    ChangeNote(PatternIndex, i, j, note);

                    TracksMidiNoteOn_DuplicateMidiNoteParams((byte)PatternIndex, (byte)i, (byte)j);
                    DoAutoEnv(PatternIndex, i, j);

                    Tracks.HideCaret();

                    if (DoStep(i, true, false))
                        ShowStat();

                    Tracks.RedrawTracks();
                    Tracks.ShowCaret();

                    RestartPlayingNote(i);
                }
            }

            if (EnvelopePoses.Contains(Tracks.CursorX))
            {
                n = note;

                if (n < 0)
                    return;

                ValidatePattern2(PatternIndex);
                y = Tracks.CurrentPatternLine();

                if (y >= 0 && y < Tracks.ShownPattern.Length)
                    e = (int)Math.Round(VTModule.GetNoteFreq(VTM.NoteTable, n) * AutoEnv0 / AutoEnv1 / 16.0);

                old = VTM.Patterns[PatternIndex].Lines[y].Envelope;

                if (!UndoWorking)
                {
                    AddUndo(TChangeAction.ChangeEnvelopePeriod, old, e);
                    ChangeList[ChangeCount - 1].Line = y;
                }

                VTM.Patterns[PatternIndex].Lines[y].Envelope = (ushort)e;
                Tracks.CurrentMidiNote = note;
                SongChanged = true;
                BackupSongChanged = true;

                Tracks.HideCaret();

                if (DoStep(y, true, false))
                    ShowStat();

                Tracks.RedrawTracks();
                Tracks.ShowCaret();

                RestartPlayingNote(y);
            }
        }

        public void TracksMidiNoteOff(short note)
        {
            if (Tracks.CurrentMidiNote != note)
                return;

            if (WaveOutAPI.IsPlaying)
            {
                Globals.MainForm.RestoreControls();

                if (AY.PlayMode == PlayModes.PlayLine || AY.PlayMode == PlayModes.PlayPattern)
                    WaveOutAPI.ResetPlaying();

                if (AY.PlayMode == PlayModes.PlayModule)
                    WaveOutAPI.StopPlaying();

                AY.PlayMode = PlayModes.PlayLine;
            }

            Tracks.KeyPressed = 0;
            Tracks.CurrentMidiNote = -1;
            VTModule.UnlimitedDelay = false;

            Tracks.RemoveSelection();
            Tracks.HideCaret();
            Tracks.RecreateCaret();
            Tracks.SetCaretPosition();
            Tracks.ShowCaret();

            PlayStopState = PlayStopState.Play;
        }

        public byte GetCurrentPatternLength()
        {
            return (byte)Tracks.ShownPattern.Length;
        }

        // Tracks MIDI In
        public void OpenSampleOrnament()
        {
            // Calculate line & channel
            int lineIndex = Tracks.CurrentPatternLine();
            int channel = MainForm.ChanAlloc[(Tracks.CursorX - 8) / 14];

            // Get note params
            ChannelLine channelLine = Tracks.ShownPattern.Lines[lineIndex].Channel[channel];
            sbyte note = channelLine.Note;
            sbyte cursorNote = channelLine.Note;
            byte sample = channelLine.Sample;
            byte ornament = channelLine.Ornament;
            byte volume = (byte)channelLine.Volume;
            byte envelope = channelLine.Envelope;
            byte commandNumber = channelLine.AdditionalCommand.Number;
            byte commandDelay = channelLine.AdditionalCommand.Delay;
            byte commandParameter = channelLine.AdditionalCommand.Parameter;

            // Get Envelope & Noise for line
            byte lineEnvelope = (byte)Tracks.ShownPattern.Lines[lineIndex].Envelope;
            byte lineNoise = Tracks.ShownPattern.Lines[lineIndex].Noise;

            // Note not found - find previous note
            if (note == -1)
            {
                for (int i = lineIndex - 1; i >= 0; i--)
                {
                    if (Tracks.ShownPattern.Lines[i].Channel[channel].Note != -1)
                    {
                        note = Tracks.ShownPattern.Lines[i].Channel[channel].Note;
                        break;
                    }
                }
            }

            // Sample not found - find prev sample
            if (sample == 0)
            {
                for (int i = lineIndex - 1; i >= 0; i--)
                {
                    if (Tracks.ShownPattern.Lines[i].Channel[channel].Sample != 0)
                    {
                        sample = Tracks.ShownPattern.Lines[i].Channel[channel].Sample;
                        break;
                    }
                }
            }

            // Ornament not found - find prev ornament
            if (ornament == 0)
            {
                for (int i = lineIndex - 1; i >= 0; i--)
                {
                    if (Tracks.ShownPattern.Lines[i].Channel[channel].Ornament != 0)
                    {
                        ornament = Tracks.ShownPattern.Lines[i].Channel[channel].Ornament;
                        break;
                    }
                }
            }

            // Envelope not found - find prev envelope
            if (envelope == 0)
            {
                for (int i = lineIndex - 1; i >= 0; i--)
                {
                    if (Tracks.ShownPattern.Lines[i].Channel[channel].Envelope != 0)
                    {
                        envelope = Tracks.ShownPattern.Lines[i].Channel[channel].Envelope;
                        break;
                    }
                }
            }

            // Volume not found - find prev volume
            if (volume == 0)
            {
                for (int i = lineIndex - 1; i >= 0; i--)
                {
                    if (Tracks.ShownPattern.Lines[i].Channel[channel].Volume != 0)
                    {
                        volume = (byte)Tracks.ShownPattern.Lines[i].Channel[channel].Volume;
                        break;
                    }
                }
            }

            // Note add. param number not found
            if (cursorNote == -1 && commandNumber == 0)
            {
                for (int i = lineIndex - 1; i >= 0; i--)
                {
                    channelLine = Tracks.ShownPattern.Lines[i].Channel[channel];
                    if (channelLine.Note != -1)
                    {
                        commandNumber = channelLine.AdditionalCommand.Number;
                        break;
                    }
                }
            }

            // Note add. param Delay not found
            if (cursorNote == -1 && commandDelay == 0)
            {
                for (int i = lineIndex - 1; i >= 0; i--)
                {
                    channelLine = Tracks.ShownPattern.Lines[i].Channel[channel];
                    if (channelLine.Note != -1)
                    {
                        commandDelay = channelLine.AdditionalCommand.Delay;
                        break;
                    }
                }
            }

            // Note add. parameter not found
            if (cursorNote == -1 && commandParameter == 0)
            {
                for (int i = lineIndex - 1; i >= 0; i--)
                {
                    channelLine = Tracks.ShownPattern.Lines[i].Channel[channel];
                    if (channelLine.Note != -1)
                    {
                        commandParameter = channelLine.AdditionalCommand.Parameter;
                        break;
                    }
                }
            }

            // Line Envelope not found
            if (lineEnvelope == 0)
            {
                for (int i = lineIndex - 1; i >= 0; i--)
                {
                    Line line = Tracks.ShownPattern.Lines[i];

                    if (line.Envelope != 0)
                    {
                        lineEnvelope = (byte)line.Envelope;
                        break;
                    }
                }
            }

            // Cursor on Sample?
            bool testSample = Tracks.CursorX == 12 || Tracks.CursorX == 26 || Tracks.CursorX == 40; testSample = false;

            // Copy note params to a testline
            channelLine = VTM.ReservedPattern.Lines[testSample ? 1 : 0].Channel[0];
            channelLine.Note = note;
            channelLine.Sample = sample;
            channelLine.Ornament = ornament;
            channelLine.Volume = (sbyte)volume;
            channelLine.Envelope = envelope;
            channelLine.AdditionalCommand.Number = commandNumber;
            channelLine.AdditionalCommand.Delay = commandDelay;
            channelLine.AdditionalCommand.Parameter = commandParameter;

            VTM.ReservedPattern.Lines[testSample ? 1 : 0].Envelope = lineEnvelope;
            VTM.ReservedPattern.Lines[testSample ? 1 : 0].Noise = lineNoise;

            // Open sample
            if (Tracks.CursorX == 12 || Tracks.CursorX == 26 || Tracks.CursorX == 40)
            {
                // Select sample
                SampleNumUpDown.Value = sample;

                // Activate samples tab
                ActivateTab(1);

                // Set focus
                if (SampleTestLine.Enabled && SampleTestLine.CanFocus)
                {
                    SampleTestLine.CursorX = 8;
                    SampleTestLine.Focus();
                }

                HideCaret(SampleTestLine.Handle);
                SampleTestLine.CreateCaret();
                SampleTestLine.SetCaretPosition();
                ShowCaret(SampleTestLine.Handle);
            }

            // Open ornament
            if (Tracks.CursorX == 14 || Tracks.CursorX == 28 || Tracks.CursorX == 42)
            {
                // Select ornament
                OrnamentNumUpDown.Value = ornament;

                // Activate ornaments tab
                ActivateTab(2);

                // Set focus
                if (OrnamentTestLine.Enabled && OrnamentTestLine.CanFocus)
                {
                    OrnamentTestLine.CursorX = 8;
                    OrnamentTestLine.Focus();
                }

                HideCaret(OrnamentTestLine.Handle);
                OrnamentTestLine.CreateCaret();
                OrnamentTestLine.SetCaretPosition();
                ShowCaret(OrnamentTestLine.Handle);
            }
        }

        public void DoSwapChannels(bool rightDirect)
        {
            int i;
            int fromX, toX;
            int numChansSelected;
            int fromChan, toChan;
            int fromLine, toLine;
            int crossModule;
            ChildForm otherModule;
            Tracks otherTracks;
            int lineIndex;
            int[] allocMap = new int[3];
            int copyFromChannel;
            int copyToChannel;
            ChannelLine[] originalChannel = new ChannelLine[3];

            // if not Tracks.IsSelected then Exit;

            // Calculate X coordinates range
            if (Tracks.SelectionX < Tracks.CursorX)
            {
                fromX = Tracks.SelectionX;
                toX = Tracks.CursorX;
            }
            else
            {
                fromX = Tracks.CursorX;
                toX = Tracks.SelectionX;
            }

            // Detect selected channels
            fromChan = 0;

            if (fromX <= 20)
                fromChan = 0;

            if (fromX > 20 && fromX <= 34)
                fromChan = 1;

            if (fromX > 34 && fromX <= 48)
                fromChan = 2;

            toChan = 0;

            if (toX <= 20)
                toChan = 0;

            if (toX > 20 && toX <= 34)
                toChan = 1;

            if (toX > 34 && toX <= 48)
                toChan = 2;

            numChansSelected = toChan - fromChan + 1;

            // Save undo
            SavePatternUndo();

            // All channels selected
            if (numChansSelected == 3 && rightDirect)
            {
                allocMap[0] = 1;
                allocMap[1] = 2;
                allocMap[2] = 0;
            }

            if (numChansSelected == 3 && !rightDirect)
            {
                allocMap[0] = 2;
                allocMap[1] = 0;
                allocMap[2] = 1;
            }

            // Channel0 and Channel1 selected
            if (numChansSelected == 2 && fromChan == 0 && toChan == 1)
            {
                allocMap[0] = 1;
                allocMap[1] = 0;
                allocMap[2] = 2;
            }

            // Channel1 and Channel2 selected
            if (numChansSelected == 2 && fromChan == 1 && toChan == 2)
            {
                allocMap[0] = 0;
                allocMap[1] = 2;
                allocMap[2] = 1;
            }

            crossModule = 0;
            otherModule = this;
            otherTracks = Tracks;

            // Single track selection swaps into left/right
            if (numChansSelected == 1)
            {
                toChan = rightDirect ? fromChan + 1 : fromChan - 1;

                if (toChan == 3)
                {
                    toChan = 0;
                    crossModule = (TSWindow[0] == null) ? 0 : 1;
                }
                else if (toChan == -1)
                {
                    toChan = 2;
                    if (TSWindow[0] == null)
                        crossModule = 0;
                    else if (TSWindow[1] == null)
                        crossModule = 1;
                    else
                        crossModule = -1;
                }

                // Swap from<->to
                allocMap[0] = 0;
                allocMap[1] = 1;
                allocMap[2] = 2;
                int temp = allocMap[toChan];
                allocMap[toChan] = allocMap[fromChan];
                allocMap[fromChan] = temp;

                otherModule = null;
                if (crossModule == -1)
                    otherModule = TSWindow[1];
                else if (crossModule == 1)
                    otherModule = TSWindow[0];

                otherTracks = otherModule.Tracks;

                // Transfer selection to target module
                otherTracks.SelectionY = Tracks.SelectionY;
                otherTracks.SelectionVisible = Tracks.SelectionVisible;
                otherTracks.CursorY = Tracks.CurrentPatternLine() - Tracks.ShownFrom + Tracks.CenterLineIndex;

                if (Tracks.IsSelected())
                {
                    otherTracks.SelectionX = 8 + toChan * 14;
                    otherTracks.CursorX = 20 + toChan * 14;
                }
                else
                {
                    if (otherTracks.CursorX < 8)
                        otherTracks.CursorX = 8;

                    otherTracks.CursorX = 8 + ((otherTracks.CursorX - 8) % 14) + toChan * 14;
                    otherTracks.SelectionX = otherTracks.CursorX;
                }
            }

            // Calculate selected line range
            if (Tracks.SelectionY < Tracks.CurrentPatternLine())
            {
                fromLine = Tracks.SelectionY;
                toLine = Tracks.CurrentPatternLine();
            }
            else
            {
                fromLine = Tracks.CurrentPatternLine();
                toLine = Tracks.SelectionY;
            }

            // Swap channels
            if (ReferenceEquals(otherTracks, Tracks))
            {
                for (lineIndex = fromLine; lineIndex <= toLine; lineIndex++)
                {
                    Line line = Tracks.ShownPattern.Lines[lineIndex];
                    originalChannel = new ChannelLine[3];
                    originalChannel[0] = line.Channel[0];
                    originalChannel[1] = line.Channel[1];
                    originalChannel[2] = line.Channel[2];

                    for (i = 0; i < 3; i++)
                    {
                        if (allocMap[i] == i)
                            continue;

                        copyFromChannel = MainForm.ChanAlloc[i];
                        copyToChannel = MainForm.ChanAlloc[allocMap[i]];
                        line.Channel[copyToChannel] = originalChannel[copyFromChannel];
                    }
                }
            }
            else
            {
                //crossmodule swap
                // Save "paired" undo flag
                ChangeList[ChangeCount - 1].OtherMDI = 1;
                otherModule.SavePatternUndo();
                otherModule.ChangeList[otherModule.ChangeCount - 1].OtherMDI = 2;

                for (lineIndex = fromLine; lineIndex <= toLine; lineIndex++)
                {
                    ChannelLine fromLineChan = Tracks.ShownPattern.Lines[lineIndex].Channel[fromChan];
                    ChannelLine toLineChan = otherTracks.ShownPattern.Lines[lineIndex].Channel[toChan];

                    Tracks.ShownPattern.Lines[lineIndex].Channel[fromChan] = toLineChan;
                    otherTracks.ShownPattern.Lines[lineIndex].Channel[toChan] = fromLineChan;
                }

                otherModule.SavePatternRedo();
            }

            // Save redo
            SavePatternRedo();
            SongChanged = true;
            BackupSongChanged = true;

            if (otherTracks != Tracks)
            {
                otherModule.ActivateTab(0);
                otherModule.Show();
                otherModule.Focus();
                if (otherTracks.CanFocus)
                    otherTracks.Focus();
                Tracks.HideCaret();
                Tracks.RedrawTracks();
            }

            Tracks.HideCaret();
            Tracks.RecreateCaret();
            Tracks.SetCaretPosition();
            Tracks.RedrawTracks();
            Tracks.ShowCaret();
        }

        public void BetweenPatternsUp()
        {
            if (!MoveBetweenPatternsCheckBox.Checked)
                return;

            PositionIndex = PositionIndex > 0 ? PositionIndex - 1 : VTM.Positions.Length - 1;
            Tracks.RedrawDisabled = true;
            IsSynchronizing = true;
            SelectPosition2(PositionIndex);
            IsSynchronizing = false;
            Tracks.RedrawDisabled = false;
            int patternLength = Tracks.ShownPattern == null ? VTModule.DefaultPatternLength : Tracks.ShownPattern.Length;
            Tracks.ShownFrom = patternLength - 1;
            Tracks.CursorY = Tracks.CenterLineIndex;
            Tracks.HideCaret();
            Tracks.RemoveSelection();
            Tracks.RedrawTracks();
            Tracks.SetCaretPosition();
            Tracks.ShowCaret();
        }

        public void BetweenPatternsDown()
        {
            if (!MoveBetweenPatternsCheckBox.Checked)
                return;

            PositionIndex = PositionIndex < VTM.Positions.Length - 1 ? PositionIndex + 1 : VTM.Positions.Loop;
            Tracks.RedrawDisabled = true;
            IsSynchronizing = true;
            SelectPosition2(PositionIndex);
            Tracks.RedrawDisabled = false;
            IsSynchronizing = false;
            Tracks.ShownFrom = 0;
            Tracks.CursorY = Tracks.CenterLineIndex;
            Tracks.HideCaret();
            Tracks.RemoveSelection();
            Tracks.RedrawTracks();
            Tracks.SetCaretPosition();
            Tracks.ShowCaret();
        }

        public void TracksKeyDown_SwitchToNextWindow(bool right, bool tab)
        {
            int curWinCurY = Tracks.CursorY;
            int dir = -1;

            if ((TSWindow[1] == null || !right) && TSWindow[0] != null && TSWindow[0].Tracks.Enabled)
                dir = 0; // 2ts or 3ts =>
            else if (TSWindow[1] != null && TSWindow[1].Tracks.Enabled && right)
                dir = 1; // 3ts <=

            if (dir >= 0)
            {
                ChildForm nextWin = TSWindow[dir];

                // Set cursor X
                nextWin.Tracks.CursorX = right ? (tab ? 36 : 48) : 0;

                // Set cursor Y
                nextWin.Tracks.CursorY = curWinCurY;

                // Adjust CursorY if it's out of bounds
                if (nextWin.Tracks.CurrentPatternLine() < 0)
                    nextWin.Tracks.CursorY = nextWin.Tracks.CenterLineIndex - nextWin.Tracks.ShownFrom;

                // Is CursorY < ShownFrom?
                int patternLength = nextWin.Tracks.ShownPattern.Length;
                if (nextWin.Tracks.CurrentPatternLine() >= patternLength)
                    nextWin.Tracks.CursorY = patternLength - nextWin.Tracks.ShownFrom + nextWin.Tracks.CenterLineIndex - 1;

                nextWin.Tracks.RemoveSelection();
                nextWin.Tracks.RedrawTracks();
                nextWin.ActivateTab(0);
                nextWin.Show();
                nextWin.Focus();

                if (nextWin.Tracks.CanFocus)
                    nextWin.Tracks.Focus();
            }
            else
            {
                // No window switch; just update current caret position
                Tracks.CursorX = right ? (tab ? 36 : 48) : 0;

                Tracks.HideCaret();
                Tracks.RecreateCaret();
                Tracks.SetCaretPosition();
                Tracks.RemoveSelection();
                Tracks.RedrawTracks();
                Tracks.ShowCaret();
            }
        }

        public void TracksKeyDown_RemSel(object sender, KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool isCtrlDown = (e.Modifiers & Keys.Control) == Keys.Control;
            bool isAltDown = (e.Modifiers & Keys.Alt) == Keys.Alt;

            if (!isShiftDown)
            {
                Tracks.HideCaret();
                Tracks.RemoveSelection();
                Tracks.ShowCaret();
            }
        }

        public void TracksKeyDown_DoDiffSlide(object sender, KeyEventArgs e)
        {
            int y, sfreq, efreq, snote, enote, spos, epos;
            int len, diff;
            int chan = MainForm.ChanAlloc[(Tracks.CursorX - 8) / 14];

            y = Tracks.CurrentPatternLine();
            spos = y;
            snote = VTM.Patterns[PatternIndex].Lines[spos].Channel[chan].Note;

            if (snote == 0)
                return;

            epos = y;
            enote = snote;

            for (y = spos + 1; y < Tracks.ShownPattern.Length; y++)
            {
                epos = y;
                enote = VTM.Patterns[PatternIndex].Lines[y].Channel[chan].Note;

                if (enote >= 0)
                    break;
            }

            if (enote < 0 || enote == snote)
                return;

            sfreq = VTModule.GetNoteFreq(VTM.NoteTable, snote);
            efreq = VTModule.GetNoteFreq(VTM.NoteTable, enote);
            len = Math.Abs(epos - spos) * VTM.InitialDelay;
            diff = Math.Abs(efreq - sfreq) / len;

            if (diff == 0)
                diff = 1;

            if (efreq > sfreq)
            {
                VTM.Patterns[PatternIndex].Lines[spos].Channel[chan].AdditionalCommand.Number = 1;
                VTM.Patterns[PatternIndex].Lines[spos].Channel[chan].AdditionalCommand.Delay = 1;
                VTM.Patterns[PatternIndex].Lines[spos].Channel[chan].AdditionalCommand.Parameter = (byte)diff;
            }
            else
            {
                VTM.Patterns[PatternIndex].Lines[spos].Channel[chan].AdditionalCommand.Number = 2;
                VTM.Patterns[PatternIndex].Lines[spos].Channel[chan].AdditionalCommand.Delay = 1;
                VTM.Patterns[PatternIndex].Lines[spos].Channel[chan].AdditionalCommand.Parameter = (byte)diff;
            }

            Tracks.RedrawTracks();
        }

        public void TracksKeyDown_DoNoteInEnvelope(object sender, KeyEventArgs e)
        {
            bool isShiftDown = (GetAsyncKeyState(Keys.ShiftKey) & 0x8000) != 0;
            bool isCtrlDown = (GetAsyncKeyState(Keys.ControlKey) & 0x8000) != 0;
            int note, n, envelope, y, old;

            if (e.KeyValue >= 256)
                return;

            note = MainForm.NoteKeys[e.KeyValue];

            if (note == -3)
                return;

            if (note > 32)
            {
                OctaveUpDown.Value = note & 31;
                return;
            }

            envelope = 0;

            if (note >= 0)
            {
                note += ((int)OctaveUpDown.Value - 1) * 12;

                if (isShiftDown)
                    note += 12;
                else if (isShiftDown || isCtrlDown)
                    note -= 12;

                if ((uint)note >= 96)
                    return;
            }

            // note is defined
            n = note;

            if (n < 0)
                return;

            ValidatePattern2(PatternIndex);
            y = Tracks.CurrentPatternLine();

            if (y >= 0 && y < Tracks.ShownPattern.Length)
                envelope = (int)Math.Round(VTModule.GetNoteFreq(VTM.NoteTable, n) * AutoEnv0 / AutoEnv1 / 16.0);

            old = VTM.Patterns[PatternIndex].Lines[y].Envelope;

            if (!UndoWorking)
            {
                AddUndo(TChangeAction.ChangeEnvelopePeriod, old, envelope);
                ChangeList[ChangeCount - 1].Line = y;
            }

            VTM.Patterns[PatternIndex].Lines[y].Envelope = (ushort)envelope;
            SongChanged = true;
            BackupSongChanged = true;

            TracksKeyDown_RemSel(sender, e);

            Tracks.KeyPressed = Keys.KeyCode;
            Tracks.HideCaret();

            if (DoStep(y, true, false))
                ShowStat();

            Tracks.RecreateCaret();
            Tracks.SetCaretPosition();
            Tracks.RedrawTracks();
            Tracks.ShowCaret();

            RestartPlayingNote(y);
        }

        public void TracksKeyDown_DoDuplicateNoteParams(byte pat, byte line, byte chan)
        {
            if (!UseLastNoteParamsCheckBox.Checked)
                return;

            ChannelLine channelLine = VTM.Patterns[pat].Lines[line].Channel[chan];
            if (channelLine.Sample != 0 || channelLine.Envelope != 0 || channelLine.Ornament != 0 || channelLine.Volume != 0)
                return;

            channelLine.Sample = Tracks.LastNoteParams[chan].Sample;
            channelLine.Envelope = Tracks.LastNoteParams[chan].Envelope;
            channelLine.Ornament = Tracks.LastNoteParams[chan].Ornament;
            channelLine.Volume = Tracks.LastNoteParams[chan].Volume;
        }

        // For patterns editor
        public void TracksKeyDown_DoNoteKey(object sender, KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool isCtrlDown = (e.Modifiers & Keys.Control) == Keys.Control;
            int i, j;

            if (e.KeyValue >= 256)
                return;

            int note = MainForm.NoteKeys[e.KeyValue];

            if (note == -3)
                return;

            if (note > 32)
            {
                OctaveUpDown.Value = note & 31;
                return;
            }

            if (note >= 0)
            {
                note += ((int)OctaveUpDown.Value - 1) * 12;
                if (isShiftDown)
                    note += 12;
                else if (isShiftDown && isCtrlDown)
                    note -= 12;

                if (note >= 96)
                    return;
            }

            Tracks.KeyPressed = e.KeyCode;

            ValidatePattern2(PatternIndex);

            i = Tracks.CurrentPatternLine();

            if (i >= 0 && i < Tracks.ShownPattern.Length)
            {
                TracksKeyDown_RemSel(sender, e);
                j = Tracks.CurrentChannel();
                ChangeNote(PatternIndex, i, j, note);

                // Don't do duplicate note params for R--
                if (e.KeyCode != Keys.A)
                    TracksKeyDown_DoDuplicateNoteParams((byte)PatternIndex, (byte)i, (byte)j);

                DoAutoEnv(PatternIndex, i, j);

                Tracks.HideCaret();
                Tracks.RecreateCaret();
                Tracks.SetCaretPosition();

                if (DoStep(i, true, false))
                    ShowStat();

                Tracks.RedrawTracks();
                Tracks.ShowCaret();

                RestartPlayingNote(i);
            }
        }

        public void TracksKeyDown_DoOtherKeys(object sender, KeyEventArgs e)
        {
            int i, n, c;

            if (SamPoses.Contains(Tracks.CursorX))
                i = 31;
            else if (OrnPoses.Contains(Tracks.CursorX))
                i = 31;
            else if (Tracks.CursorX == 5) // First number of noise (dec/hex) 
                i = MainForm.DecBaseNoiseOn ? 3 : 1;
            else
                i = 15;

            if (e.KeyValue >= (int)'0' && e.KeyValue <= (int)'9')
                n = e.KeyValue - (int)'0';
            else
                n = e.KeyValue - (int)'A' + 10;

            if (n < 0 || n > i)
                return;

            Tracks.KeyPressed = e.KeyCode;

            ValidatePattern2(PatternIndex);

            i = Tracks.CurrentPatternLine();

            if (i >= 0 && i < Tracks.ShownPattern.Length)
            {
                TracksKeyDown_RemSel(sender, e);

                c = (Tracks.CursorX - 8) / 14;

                if (c >= 0)
                    c = MainForm.ChanAlloc[c];

                ChangeTracks(PatternIndex, i, c, Tracks.CursorX, n, true);

                if (Tracks.CursorX == 13 || Tracks.CursorX == 27 || Tracks.CursorX == 41)
                    DoAutoEnv(PatternIndex, i, c);

                Tracks.HideCaret();

                // if Tracks.CursorX in [12,15,26,29,40,43] then //comment -> step in any col
                if (DoStep(i, true, false))
                    ShowStat();

                Tracks.RecreateCaret();
                Tracks.SetCaretPosition();
                Tracks.RedrawTracks();
                Tracks.ShowCaret();

                RestartPlayingNote(i);
            }
        }

        public void TracksKeyDown_RedrawTrs()
        {
            Tracks.HideCaret();
            Tracks.RedrawTracks();
            Tracks.ShowCaret();
        }

        public void TracksKeyDown_DoCursorDown(object sender, KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool isCtrlDown = (e.Modifiers & Keys.Control) == Keys.Control;
            bool isAltDown = (e.Modifiers & Keys.Alt) == Keys.Alt;
            bool isLeftDown = (Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left;
            bool isRightDown = (Control.MouseButtons & MouseButtons.Right) == MouseButtons.Right;
            bool isButtonDown = isLeftDown || isRightDown;
            int to1;
            int patternLength;

            Tracks.ManualBitBlt = true;

            if (TSWindow[0] != null)
                TSWindow[0].Tracks.ManualBitBlt = true;

            if (TSWindow[1] != null)
                TSWindow[1].Tracks.ManualBitBlt = true;

            patternLength = Tracks.ShownPattern == null ? VTModule.DefaultPatternLength : Tracks.ShownPattern.Length;
            to1 = patternLength - Tracks.ShownFrom + Tracks.CenterLineIndex;

            if (to1 > Tracks.VisibleLineCount)
                to1 = Tracks.VisibleLineCount;

            if (Tracks.CursorY < to1 - 1 && Tracks.CursorY != Tracks.CenterLineIndex)
            {
                Tracks.HideCaret();

                if (isAltDown)
                    DoStep(Tracks.CurrentPatternLine(), true, true);
                else
                    Tracks.CursorY++;

                Tracks.SetCaretPosition();

                if (isShiftDown)
                    Tracks.ShowSelection();
                else
                    Tracks.RemoveSelection();

                Tracks.RedrawTracks();
                Tracks.ShowCaret();
            }
            // On selected line
            else if (Tracks.ShownFrom < patternLength - Tracks.CursorY - 1 + Tracks.CenterLineIndex)
            {
                if (isAltDown)
                    DoStep(Tracks.CurrentPatternLine(), true, true);
                else
                    Tracks.ShownFrom++;

                if (isShiftDown)
                    Tracks.ShowSelection();
                else
                    Tracks.RemoveSelection();

                TracksKeyDown_RedrawTrs();
            }
            else if (!isButtonDown)
            {
                if (MoveBetweenPatternsCheckBox.Checked)
                    BetweenPatternsDown();
                else
                {
                    Tracks.ShownFrom = 0;
                    Tracks.CursorY = Tracks.CenterLineIndex;
                    Tracks.RemoveSelection();
                    Tracks.RedrawTracks();
                }
            }

            ShowStat();

            Tracks.HideCaret();
            Tracks.DoBitBlt();
            Tracks.SetCaretPosition();
            Tracks.ShowCaret();
            Tracks.ManualBitBlt = false;

            if (TSWindow[0] != null)
            {
                TSWindow[0].Tracks.ManualBitBlt = false;
                TSWindow[0].Tracks.DoBitBlt();
            }

            if (TSWindow[1] != null)
            {
                TSWindow[1].Tracks.ManualBitBlt = false;
                TSWindow[1].Tracks.DoBitBlt();
            }
        }

        public void TracksKeyDown_DoCursorUp(object sender, KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool isCtrlDown = (e.Modifiers & Keys.Control) == Keys.Control;
            bool isAltDown = (e.Modifiers & Keys.Alt) == Keys.Alt;
            bool isLeftDown = (Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left;
            bool isRightDown = (Control.MouseButtons & MouseButtons.Right) == MouseButtons.Right;
            bool isButtonDown = isLeftDown || isRightDown;
            int from;
            int patternLength;

            Tracks.ManualBitBlt = true;

            if (TSWindow[0] != null)
                TSWindow[0].Tracks.ManualBitBlt = true;

            if (TSWindow[1] != null)
                TSWindow[1].Tracks.ManualBitBlt = true;

            from = (Tracks.CenterLineIndex - Tracks.ShownFrom);

            if (from < 0)
                from = 0;

            if (Tracks.CursorY > from && Tracks.CursorY != Tracks.CenterLineIndex)
            {
                Tracks.HideCaret();

                if (isAltDown)
                    DoStep(Tracks.CurrentPatternLine(), false, true);
                else
                    Tracks.CursorY--;

                Tracks.SetCaretPosition();

                if (isShiftDown)
                    Tracks.ShowSelection();
                else
                    Tracks.RemoveSelection();

                Tracks.RedrawTracks();
                Tracks.ShowCaret();
            }
            else if (Tracks.ShownFrom > Tracks.CenterLineIndex - Tracks.CursorY)
            {
                if (isAltDown)
                    DoStep(Tracks.CurrentPatternLine(), false, true);
                else
                    Tracks.ShownFrom--;

                if (isShiftDown)
                    Tracks.ShowSelection();
                else
                    Tracks.RemoveSelection();

                TracksKeyDown_RedrawTrs();
            }
            else if (!isButtonDown)
            {
                patternLength = Tracks.ShownPattern == null ? VTModule.DefaultPatternLength : Tracks.ShownPattern.Length;

                if (MoveBetweenPatternsCheckBox.Checked)
                    BetweenPatternsUp();
                else
                {
                    Tracks.ShownFrom = patternLength - 1;
                    Tracks.CursorY = Tracks.CenterLineIndex;

                    Tracks.RemoveSelection();
                    Tracks.RedrawTracks();
                }
            }
            ShowStat();

            Tracks.HideCaret();
            Tracks.DoBitBlt();
            Tracks.SetCaretPosition();
            Tracks.ShowCaret();
            Tracks.ManualBitBlt = false;

            if (TSWindow[0] != null)
            {
                TSWindow[0].Tracks.ManualBitBlt = false;
                TSWindow[0].Tracks.DoBitBlt();
            }

            if (TSWindow[1] != null)
            {
                TSWindow[1].Tracks.ManualBitBlt = false;
                TSWindow[1].Tracks.DoBitBlt();
            }
        }

        public void TracksKeyDown_DoCursorLeft(object sender, KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool isCtrlDown = (e.Modifiers & Keys.Control) == Keys.Control;
            bool isAltDown = (e.Modifiers & Keys.Alt) == Keys.Alt;
            int min = 0;

            if (isCtrlDown)
                min = 4;

            if (Tracks.CursorX > min)
            {
                if (isCtrlDown)
                    Tracks.CursorX = ColTabs[ColTab(Tracks.CursorX) - 1];
                else if (isCtrlDown && isShiftDown && (Tracks.CursorX <= Tracks.SelectionX))
                    Tracks.CursorX = ColTabsL[ColTab1(Tracks.CursorX, ref ColTabsL) - 1];
                else if (isCtrlDown && isShiftDown && (Tracks.CursorX > Tracks.SelectionX))
                    Tracks.CursorX = ColTabsR[ColTab1(Tracks.CursorX, ref ColTabsR) - 1];
                else
                {
                    if (Tracks.CursorX == 12 || Tracks.CursorX == 26 || Tracks.CursorX == 40)
                        Tracks.CursorX -= 4;
                    else if (ColSpace(Tracks.CursorX - 1))
                        Tracks.CursorX -= 2;
                    else
                        Tracks.CursorX--;
                }

                Tracks.HideCaret();
                Tracks.RecreateCaret();
                Tracks.SetCaretPosition();

                if (isShiftDown)
                    Tracks.ShowSelection();
                else
                    Tracks.RemoveSelection();

                Tracks.RedrawTracks();
                Tracks.ShowCaret();
            }
            else
                TracksKeyDown_SwitchToNextWindow(true, false);
        }

        public void TracksKeyDown_DoCursorRight(object sender, KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool isCtrlDown = (e.Modifiers & Keys.Control) == Keys.Control;
            bool isAltDown = (e.Modifiers & Keys.Alt) == Keys.Alt;
            int max = 48;

            if (isCtrlDown)
                max = 44;

            if (Tracks.CursorX < max)
            {
                if (isCtrlDown)
                    Tracks.CursorX = ColTabs[ColTab(Tracks.CursorX) + 1];
                else if (isCtrlDown && isShiftDown && (Tracks.CursorX >= Tracks.SelectionX))
                    Tracks.CursorX = ColTabsR[ColTab1(Tracks.CursorX, ref ColTabsR) + 1];
                else if (isCtrlDown && isShiftDown && (Tracks.CursorX < Tracks.SelectionX))
                    Tracks.CursorX = ColTabsL[ColTab1(Tracks.CursorX, ref ColTabsL) + 1];
                else
                {
                    Tracks.CursorX++;
                    if (ColSpace(Tracks.CursorX))
                        Tracks.CursorX++;
                    else if (Tracks.CursorX == 9 || Tracks.CursorX == 23 || Tracks.CursorX == 37)
                        Tracks.CursorX += 3;
                }

                Tracks.HideCaret();
                Tracks.RecreateCaret();
                Tracks.SetCaretPosition();

                if (isShiftDown)
                    Tracks.ShowSelection();
                else
                    Tracks.RemoveSelection();

                Tracks.RedrawTracks();
                Tracks.ShowCaret();
            }
            else
                TracksKeyDown_SwitchToNextWindow(false, false);
        }

        public void TracksKeyDown_GetColsToEdit(ref bool envelope, ref bool noise, ref bool[] tone, bool allPat, object sender, KeyEventArgs e)
        {
            if (allPat)
            {
                envelope = true;
                noise = true;
                tone[0] = true;
                tone[1] = true;
                tone[2] = true;
            }
            else
            {
                envelope = false;
                noise = false;
                tone[0] = false;
                tone[1] = false;
                tone[2] = false;

                if (Tracks.CursorX < 4)
                    envelope = true;
                else if (Tracks.CursorX < 8)
                    noise = true;
                else
                    tone[MainForm.ChanAlloc[(Tracks.CursorX - 8) / 14]] = true;
            }
        }

        public void TracksKeyDown_DoInsertLine(bool allPat, object sender, KeyEventArgs e)
        {
            int i, j, c;
            bool envelope = false;
            bool noise = false;
            bool[] tone = new bool[3];

            TracksKeyDown_RemSel(sender, e);

            if (Tracks.ShownPattern != null)
            {
                i = Tracks.CurrentPatternLine();

                if (i >= 0 && i < Tracks.ShownPattern.Length)
                {
                    SongChanged = true;
                    BackupSongChanged = true;

                    AddUndo(TChangeAction.PatternInsertLine, 0, 0);
                    ChangeList[ChangeCount - 1].Pattern = (Pattern)Tracks.ShownPattern.Clone();

                    TracksKeyDown_GetColsToEdit(ref envelope, ref noise, ref tone, allPat, this, e);

                    if (envelope)
                    {
                        for (j = VTModule.MaxPatternLength - 1; j >= i; j--)
                            Tracks.ShownPattern.Lines[j].Envelope = Tracks.ShownPattern.Lines[j - 1].Envelope;
                        Tracks.ShownPattern.Lines[i].Envelope = 0;
                    }

                    if (noise)
                    {
                        for (j = VTModule.MaxPatternLength - 1; j >= i; j--)
                            Tracks.ShownPattern.Lines[j].Noise = Tracks.ShownPattern.Lines[j - 1].Noise;
                        Tracks.ShownPattern.Lines[i].Noise = 0;
                    }

                    for (c = 0; c < 3; c++)
                    {
                        if (tone[c])
                        {
                            for (j = VTModule.MaxPatternLength - 1; j >= i; j--)
                                Tracks.ShownPattern.Lines[j].Channel[c] = Tracks.ShownPattern.Lines[j - 1].Channel[c];
                            Tracks.ShownPattern.Lines[i].Channel[c] = new ChannelLine();
                        }
                    }

                    CalcTotLen();
                    ShowStat();
                    TracksKeyDown_RedrawTrs();

                    ChangeList[ChangeCount - 1].NewParams.Params.PatternCursorY = Tracks.CursorY;
                    ChangeList[ChangeCount - 1].NewParams.Params.PatternShownFrom = Tracks.ShownFrom;
                }
            }
        }

        public void TracksKeyDown_DoNextColumn(object sender, KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool isCtrlDown = (e.Modifiers & Keys.Control) == Keys.Control;
            bool isAltDown = (e.Modifiers & Keys.Alt) == Keys.Alt;

            if ((Tracks.CursorX >= 36 && Tracks.CursorX <= 48) && !isShiftDown)
            {
                Tracks.DoRefresh();
                TracksKeyDown_SwitchToNextWindow(false, true);
                return;
            }
            if ((Tracks.CursorX >= 0 && Tracks.CursorX <= 6) && isShiftDown)
            {
                Tracks.DoRefresh();
                TracksKeyDown_SwitchToNextWindow(true, true);
                return;
            }
            if (isShiftDown)
            {
                if (Tracks.CursorX >= 8 && Tracks.CursorX <= 20)
                    Tracks.CursorX = 0;
                if (Tracks.CursorX >= 22 && Tracks.CursorX <= 34)
                    Tracks.CursorX = 8;
                if (Tracks.CursorX >= 36 && Tracks.CursorX <= 48)
                    Tracks.CursorX = 22;
            }
            else
            {
                if (Tracks.CursorX >= 22 && Tracks.CursorX <= 34)
                    Tracks.CursorX = 36;
                if (Tracks.CursorX >= 8 && Tracks.CursorX <= 20)
                    Tracks.CursorX = 22;
                if (Tracks.CursorX >= 0 && Tracks.CursorX <= 6)
                    Tracks.CursorX = 8;
            }

            Tracks.DoRefresh();
        }

        public void TracksKeyDown_DoMuteDismuteChannels(object sender, KeyEventArgs e)
        {
            int chanNum;
            ChildForm childForm;
            bool ifSolo;

            if (Tracks.CursorX >= 36)
                chanNum = MainForm.ChanAlloc[2];
            else if (Tracks.CursorX >= 22)
                chanNum = MainForm.ChanAlloc[1];
            else if (Tracks.CursorX >= 8)
                chanNum = MainForm.ChanAlloc[0];
            else
            {
                chanNum = -1;
                return;
            }

            ifSolo = CheckSolo();

            for (int j = 0; j < 3; j++)
            {
                if (j == 1)
                    childForm = TSWindow[0];
                else if (j == 2)
                    childForm = TSWindow[1];
                else
                    childForm = this;

                if (childForm == null)
                    continue;

                for (int i = 0; i < 3; i++)
                {
                    if (ifSolo)
                        childForm.ChanButtons[i].Solo_But_s = 0;
                    else
                        ChanButtons[chanNum].Solo_But_s = 1;
                }
            }

            ApplySolo();
            UpdateChannelsState();
        }

        public void TracksKeyDown_DoRemoveLine(object sender, KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool isCtrlDown = (e.Modifiers & Keys.Control) == Keys.Control;
            bool isAltDown = (e.Modifiers & Keys.Alt) == Keys.Alt;
            int i, j, c;
            bool envelope = false;
            bool noise = false;
            bool[] tone = new bool[3];

            TracksKeyDown_RemSel(sender, e);

            if (Tracks.ShownPattern != null)
            {
                i = Tracks.CurrentPatternLine();

                if (i >= 0 && i < Tracks.ShownPattern.Length)
                {
                    SongChanged = true;
                    BackupSongChanged = true;

                    AddUndo(TChangeAction.PatternDeleteLine, 0, 0);

                    ChangeList[ChangeCount - 1].Pattern = (Pattern)Tracks.ShownPattern.Clone();

                    TracksKeyDown_GetColsToEdit(ref envelope, ref noise, ref tone, isCtrlDown, sender, e);

                    if (envelope)
                    {
                        for (j = i + 1; j < VTModule.MaxPatternLength; j++)
                            Tracks.ShownPattern.Lines[j - 1].Envelope = Tracks.ShownPattern.Lines[j].Envelope;

                        Tracks.ShownPattern.Lines[VTModule.MaxPatternLength - 1].Envelope = 0;
                    }

                    if (noise)
                    {
                        for (j = i + 1; j < VTModule.MaxPatternLength; j++)
                            Tracks.ShownPattern.Lines[j - 1].Noise = Tracks.ShownPattern.Lines[j].Noise;

                        Tracks.ShownPattern.Lines[VTModule.MaxPatternLength - 1].Noise = 0;
                    }

                    for (c = 0; c < 3; c++)
                    {
                        if (tone[c])
                        {
                            for (j = i + 1; j < VTModule.MaxPatternLength; j++)
                                Tracks.ShownPattern.Lines[j - 1].Channel[c] = Tracks.ShownPattern.Lines[j].Channel[c];

                            Tracks.ShownPattern.Lines[VTModule.MaxPatternLength - 1].Channel[c] = new ChannelLine();
                        }
                    }

                    CalcTotLen();
                    ShowStat();
                    TracksKeyDown_RedrawTrs();

                    ChangeList[ChangeCount - 1].NewParams.Params.PatternCursorY = Tracks.CursorY;
                    ChangeList[ChangeCount - 1].NewParams.Params.PatternShownFrom = Tracks.ShownFrom;
                }
            }
        }

        public void TracksKeyDown_DoClearLine(object sender, KeyEventArgs e)
        {
            int i, c;
            bool envelope = false;
            bool noise = false;
            bool[] tone = new bool[3];

            TracksKeyDown_RemSel(sender, e);

            if (Tracks.ShownPattern != null)
            {
                i = Tracks.CurrentPatternLine();

                if (i >= 0 && i < Tracks.ShownPattern.Length)
                {
                    SongChanged = true;
                    BackupSongChanged = true;

                    AddUndo(TChangeAction.PatternClearLine, 0, 0);

                    ChangeList[ChangeCount - 1].Pattern = (Pattern)Tracks.ShownPattern.Clone();

                    TracksKeyDown_GetColsToEdit(ref envelope, ref noise, ref tone, (e.Modifiers & Keys.Control) != 0, sender, e);
                    if (envelope)
                        Tracks.ShownPattern.Lines[i].Envelope = 0;

                    if (noise)
                        Tracks.ShownPattern.Lines[i].Noise = 0;

                    for (c = 0; c < 3; c++)
                    {
                        if (tone[c])
                            Tracks.ShownPattern.Lines[i].Channel[c] = new ChannelLine();
                    }

                    CalcTotLen();

                    if (DoStep(i, true, false))
                        ShowStat();

                    TracksKeyDown_RedrawTrs();

                    ChangeList[ChangeCount - 1].NewParams.Params.PatternCursorY = Tracks.CursorY;
                    ChangeList[ChangeCount - 1].NewParams.Params.PatternShownFrom = Tracks.ShownFrom;
                }
            }
        }

        public void Tracks_KeyDown(object sender, KeyEventArgs e)
        {
            bool isNoneDown = e.Modifiers == Keys.None;
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool isCtrlDown = (e.Modifiers & Keys.Control) == Keys.Control;
            bool isAltDown = (e.Modifiers & Keys.Alt) == Keys.Alt;
            bool isLeftDown = (Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left;
            bool isRightDown = (Control.MouseButtons & MouseButtons.Right) == MouseButtons.Right;
            bool isButtonDown = isLeftDown || isRightDown;
            int patternLength;
            int i, j;
            bool incr;
            bool decr;

            // Ctrl+Enter -> Open selected sample or ornament
            if (isCtrlDown && e.KeyCode == Keys.Enter && (Tracks.CursorX == 12 || Tracks.CursorX == 14 || Tracks.CursorX == 26 || Tracks.CursorX == 28 || Tracks.CursorX == 40 || Tracks.CursorX == 42))
            {
                OpenSampleOrnament();
                return;
            }

            // Ctrl+R -> Toggle autostep
            if (isCtrlDown && (e.KeyValue == (int)'R'))
                return;

            // Change octave: Alt + 1..8
            if (isAltDown && (e.KeyValue >= 49) && (e.KeyValue <= 56))
            {
                OctaveUpDown.Value = e.KeyValue - 48;
                return;
            }

            var shiftlessModifiers = e.Modifiers & ~Keys.Shift;
            var key = e.KeyCode;

            if (HotKeys.MatchesShortcut(HotKeys.AllHotKeys[(int)HotKeyType.JumpPatternStart].ShortcutText, e))
            {
                Tracks.JumpToPatStart(e);
                ShowStat();
                return;
            }

            if (HotKeys.MatchesShortcut(HotKeys.AllHotKeys[(int)HotKeyType.JumpPatternEnd].ShortcutText, e))
            {
                Tracks.JumpToPatEnd(e);
                ShowStat();
                return;
            }

            if (HotKeys.MatchesShortcut(HotKeys.AllHotKeys[(int)HotKeyType.JumpLineStart].ShortcutText, e))
            {
                Tracks.JumpToLineStart(e);
                ShowStat();
                return;
            }

            if (HotKeys.MatchesShortcut(HotKeys.AllHotKeys[(int)HotKeyType.JumpLineEnd].ShortcutText, e))
            {
                Tracks.JumpToLineEnd(e);
                ShowStat();
                return;
            }

            incr = (e.KeyCode == Keys.Oemplus);
            decr = (e.KeyCode == Keys.OemMinus);

            if (Tracks.IsSelected() && (incr || decr))
            {
                if (incr && !isButtonDown)
                    Globals.MainForm.TransposeSelection(1);
                if (decr && !isButtonDown)
                    Globals.MainForm.TransposeSelection(-1);
                if (incr && isShiftDown)
                    Globals.MainForm.TransposeSelection(3);
                if (decr && isShiftDown)
                    Globals.MainForm.TransposeSelection(-3);
                if (incr && isCtrlDown)
                    Globals.MainForm.TransposeSelection(12);
                if (decr && isCtrlDown)
                    Globals.MainForm.TransposeSelection(-12);
                if (incr && (isCtrlDown && isShiftDown))
                    Globals.MainForm.TransposeSelection(5);
                if (decr && (isCtrlDown && isShiftDown))
                    Globals.MainForm.TransposeSelection(-5);
                return;
            }

            switch (e.KeyCode)
            {
                // Cursor key DOWN
                case Keys.Down:
                    if (isCtrlDown)
                    {
                        if (Tracks.ShownPattern == null)
                            patternLength = VTModule.DefaultPatternLength;
                        else
                            patternLength = Tracks.ShownPattern.Length;

                        Tracks.ShownFrom = patternLength - 1;
                        Tracks.CursorY = Tracks.CenterLineIndex;
                        if (isShiftDown)
                            Tracks.ShowSelection();
                        else
                            Tracks.RemoveSelection();
                        Tracks.HideCaret();
                        Tracks.RedrawTracks();
                        Tracks.SetCaretPosition();
                        Tracks.ShowCaret();
                        ShowStat();
                    }
                    else
                        TracksKeyDown_DoCursorDown(sender, e);
                    break;
                // Cursor key UP
                case Keys.Up:
                    if (isCtrlDown)
                    {
                        Tracks.ShownFrom = 0;
                        Tracks.CursorY = Tracks.CenterLineIndex;
                        if (isShiftDown)
                            Tracks.ShowSelection();
                        else
                            Tracks.RemoveSelection();
                        Tracks.HideCaret();
                        Tracks.RedrawTracks();
                        Tracks.SetCaretPosition();
                        Tracks.ShowCaret();
                        ShowStat();
                    }
                    else
                        TracksKeyDown_DoCursorUp(sender, e);
                    break;
                // Cursor key LEFT
                case Keys.Left:
                    TracksKeyDown_DoCursorLeft(sender, e);
                    break;
                // Cursor key RIGHT
                case Keys.Right:
                    TracksKeyDown_DoCursorRight(sender, e);
                    break;
                // TAB
                case Keys.Tab:
                    TracksKeyDown_DoNextColumn(sender, e);
                    break;
                // CAPS LOCK -> Solo Channel, noise, envelope
                case Keys.Capital:
                    TracksKeyDown_DoMuteDismuteChannels(sender, e);
                    break;
                // Numpad * -> Mute channel/channels
                // -> Expand
                case Keys.Multiply:
                    if (isCtrlDown && isShiftDown)
                        Globals.MainForm.ExpandPattern_Execute(sender, e);
                    else if (isShiftDown)
                    {
                    }
                    else if (isCtrlDown)
                    {
                        // next position
                    }
                    else
                        TracksKeyDown_DoMuteDismuteChannels(sender, e);
                    break;
                case Keys.OemPipe:
                    if (isCtrlDown && isShiftDown)
                        TracksKeyDown_DoDiffSlide(sender, e);
                    break;
                case Keys.Divide:
                    if (isCtrlDown && isShiftDown)
                        Globals.MainForm.CompressPattern_Execute(sender, e);
                    else if (isShiftDown)
                    {
                    }
                    else if (isCtrlDown)
                    {
                        // next position
                    }
                    else
                    {
                        // next pattern
                        EnvelopeAsNoteCheckBox.Checked = !EnvelopeAsNoteCheckBox.Checked;
                    }
                    break;
                case Keys.Add:
                    if (isCtrlDown && isShiftDown)
                    {
                        Globals.MainForm.TransposeSelection(12);
                        // TransposeSelection(12);
                    }
                    else if (isShiftDown)
                    {
                        Globals.MainForm.TransposeSelection(1);
                    }
                    else if (isCtrlDown)
                    {
                        // next position
                        // SelectPosition(PositionsGrid.Col+1)
                        int currentCol = PositionsGrid.CurrentCell.ColumnIndex;
                        int currentRow = PositionsGrid.CurrentCell.RowIndex;

                        if (currentCol < PositionsGrid.ColumnCount - 1 && currentCol < VTM.Positions.Length - 1)
                            PositionsGrid.CurrentCell = PositionsGrid[currentCol + 1, currentRow];
                    }
                    else
                    {
                        // next pattern
                        // ChangePattern(PatNum+1);
                        if (PatternIndex <= 83)
                        {
                            PatternNumUpDown.Value = PatternIndex + 1;
                            //PatternNumEdit.Text = (PatNum + 1).ToString();
                        }
                    }
                    break;
                case Keys.Subtract:
                    if (isCtrlDown && isShiftDown)
                    {
                        Globals.MainForm.TransposeSelection(-12);
                        // TransposeSelection(12);
                    }
                    else if (isShiftDown)
                    {
                        Globals.MainForm.TransposeSelection(-1);
                    }
                    else if (isCtrlDown)
                    {
                        if (PositionsGrid.CurrentCell.ColumnIndex >= 1)
                        {
                            int newCol = PositionsGrid.CurrentCell.ColumnIndex - 1;
                            int row = PositionsGrid.CurrentCell.RowIndex;

                            PositionsGrid.CurrentCell = PositionsGrid[newCol, row];
                        }
                    }
                    else
                    {
                        // prev pattern
                        // ChangePattern(PatNum-1);
                        if (PatternIndex >= 1)
                            SampleNumUpDown.Value = PatternIndex - 1;
                    }
                    break;
                // Page UP
                case Keys.Prior:
                    // Ctrl + Page UP -> Up to 8 lines
                    if (isCtrlDown)
                    {
                        Tracks.ShownFrom -= 8;

                        if (Tracks.ShownFrom < 0)
                        {
                            if (MoveBetweenPatternsCheckBox.Checked)
                                BetweenPatternsUp();
                            else
                                Tracks.ShownFrom = 0;
                        }

                        Tracks.CursorY = Tracks.CenterLineIndex;

                        Tracks.HideCaret();
                        Tracks.RedrawTracks();

                        if (isShiftDown)
                            Tracks.ShowSelection();
                        else
                            Tracks.RemoveSelection();

                        Tracks.SetCaretPosition();
                        Tracks.ShowCaret();
                    }
                    else
                    {
                        // cursor points to the first pattern line?
                        if (Tracks.CurrentPatternLine() == 0 && !isShiftDown)
                        {
                            if (MoveBetweenPatternsCheckBox.Checked)
                                BetweenPatternsUp();
                            else
                            {
                                if (Tracks.ShownPattern == null)
                                    patternLength = VTModule.DefaultPatternLength;
                                else
                                    patternLength = Tracks.ShownPattern.Length;

                                Tracks.ShownFrom = patternLength - 1;
                                Tracks.CursorY = Tracks.CenterLineIndex;

                                Tracks.HideCaret();
                                Tracks.RemoveSelection();
                                Tracks.RedrawTracks();
                                Tracks.SetCaretPosition();
                                Tracks.ShowCaret();
                            }
                        }
                        // Page Up -> Up to 16 lines
                        // cursor in the middle or on the first line?
                        else if (Tracks.CursorY == Tracks.CenterLineIndex || Tracks.CursorY == 0)
                        {
                            Tracks.ShownFrom -= 16;

                            if (Tracks.ShownFrom < 0)
                            {
                                if (MoveBetweenPatternsCheckBox.Checked)
                                    BetweenPatternsUp();
                                else
                                    Tracks.ShownFrom = 0;
                            }

                            if (Tracks.CurrentPatternLine() < 0)
                                Tracks.CursorY = Tracks.CenterLineIndex - Tracks.ShownFrom;

                            Tracks.HideCaret();
                            Tracks.SetCaretPosition();

                            if (isShiftDown)
                                Tracks.ShowSelection();
                            else
                                Tracks.RemoveSelection();

                            Tracks.RedrawTracks();
                            Tracks.ShowCaret();
                        }
                        // cursor in other location
                        else
                        {
                            Tracks.CursorY = Tracks.CenterLineIndex - Tracks.ShownFrom;

                            if (Tracks.CursorY < 0)
                                Tracks.CursorY = 0;

                            Tracks.SetCaretPosition();

                            if (isShiftDown)
                                Tracks.ShowSelection();
                            else
                                Tracks.RemoveSelection();

                            Tracks.RedrawTracks();
                        }
                    }
                    ShowStat();
                    break;
                // Page Down
                case Keys.Next:
                    if (Tracks.ShownPattern == null)
                        patternLength = VTModule.DefaultPatternLength;
                    else
                        patternLength = Tracks.ShownPattern.Length;

                    // Ctrl + Page Down -> Down to 8 lines
                    if (isCtrlDown)
                    {
                        Tracks.ShownFrom += 8;
                        if (Tracks.ShownFrom >= patternLength)
                        {
                            if (MoveBetweenPatternsCheckBox.Checked)
                                BetweenPatternsDown();
                            else
                                Tracks.ShownFrom = patternLength - 1;
                        }

                        Tracks.CursorY = Tracks.CenterLineIndex;
                        Tracks.HideCaret();

                        if (isShiftDown)
                            Tracks.ShowSelection();
                        else
                            Tracks.RemoveSelection();

                        Tracks.RedrawTracks();
                        Tracks.SetCaretPosition();
                        Tracks.ShowCaret();
                    }
                    else
                    {
                        // cursor points to the last pattern line?
                        if (Tracks.CurrentPatternLine() == patternLength - 1)
                        {
                            if (!isShiftDown)
                            {
                                if (MoveBetweenPatternsCheckBox.Checked)
                                    BetweenPatternsDown();
                                else
                                {
                                    Tracks.ShownFrom = 0;
                                    Tracks.CursorY = Tracks.CenterLineIndex;
                                    Tracks.HideCaret();
                                    Tracks.RemoveSelection();
                                    Tracks.RedrawTracks();
                                    Tracks.SetCaretPosition();
                                    Tracks.ShowCaret();
                                }
                            }
                        }
                        // Pade Down -> Down to 16 lines
                        // cursor in the middle or in the last line?
                        else if (Tracks.CursorY == Tracks.CenterLineIndex || Tracks.CursorY == Tracks.VisibleLineCount - 1)
                        {
                            Tracks.ShownFrom += 16;
                            if (Tracks.ShownFrom >= patternLength)
                                Tracks.ShownFrom = patternLength - 1;

                            if (Tracks.CurrentPatternLine() >= patternLength)
                                Tracks.CursorY = patternLength - Tracks.ShownFrom + Tracks.CenterLineIndex - 1;

                            Tracks.HideCaret();
                            Tracks.SetCaretPosition();

                            if (isShiftDown)
                                Tracks.ShowSelection();
                            else
                                Tracks.RemoveSelection();

                            Tracks.RedrawTracks();
                            Tracks.ShowCaret();
                        }
                        // cursor in other location
                        else
                        {
                            Tracks.CursorY = patternLength - Tracks.ShownFrom + Tracks.CenterLineIndex - 1;

                            if (Tracks.CursorY >= Tracks.VisibleLineCount)
                                Tracks.CursorY = Tracks.VisibleLineCount - 1;

                            Tracks.SetCaretPosition();

                            if (isShiftDown)
                                Tracks.ShowSelection();
                            else
                                Tracks.RemoveSelection();

                            Tracks.RedrawTracks();
                        }
                    }
                    ShowStat();
                    break;
                case Keys.Insert:
                    if (isNoneDown)
                        TracksKeyDown_DoInsertLine(false, sender, e);
                    else if (isShiftDown)
                        Tracks.PasteFromClipboard(false);
                    else if (isCtrlDown)
                        Tracks.CopyToClipboard();
                    break;
                case Keys.Back:
                    if (isShiftDown)
                    {
                        i = Tracks.CurrentPatternLine();

                        if (i >= 0 && i < Tracks.ShownPattern.Length)
                        {
                            if (DoStep(i, false, false))
                            {
                                ShowStat();
                                TracksKeyDown_RedrawTrs();
                            }
                        }
                    }
                    else
                        TracksKeyDown_DoRemoveLine(sender, e);
                    break;

                case Keys.Delete:
                    if (isShiftDown)
                    {
                        if (UIActionManager.Instance.IsEnabled(UIActionType.EditCut))
                            UIActionManager.Instance.Execute(sender, UIActionType.EditCut);
                    }
                    else if (!isButtonDown)
                    {
                        if (EnvelopePoses.Contains(Tracks.CursorX))
                        {
                            i = Tracks.CurrentPatternLine();

                            if (i >= 0 && i < Tracks.ShownPattern.Length)
                                Tracks.ShownPattern.Lines[i].Envelope = 0;
                        }

                        Tracks.ClearSelection();

                        if (DoStep(Tracks.CurrentPatternLine(), true, false))
                            ShowStat();

                        TracksKeyDown_RedrawTrs();
                    }
                    else
                        TracksKeyDown_DoClearLine(sender, e);
                    break;
                case Keys.Oemtilde:
                    if (isNoneDown)
                    {
                        if (PositionsGrid.CanFocus)
                            PositionsGrid.Focus();
                    }
                    break;
                case Keys.NumPad0:
                    ToggleAutoEnv();
                    break;
                case Keys.Space:
                    // Edit song?
                    ToggleAutoStep();
                    break;
                case Keys.Return:
                    if (Tracks.KeyPressed != Keys.Return)
                    {
                        // Ctrl+Enter Return back after playing
                        if (isCtrlDown)
                        {
                            Tracks.ReturnAfterPlay = true;
                            Tracks.ReturnCursorY = Tracks.CenterLineIndex;
                            Tracks.ReturnShownFrom = Tracks.CurrentPatternLine();
                            Tracks.ReturnPosition = PositionIndex;
                        }
                        else
                            Tracks.ReturnAfterPlay = false;

                        Tracks.KeyPressed = Keys.Return;

                        ValidatePattern2(PatternIndex);

                        Tracks.HideCaret();
                        Tracks.ShownFrom = Tracks.CurrentPatternLine();
                        Tracks.CursorY = Tracks.CenterLineIndex;
                        Tracks.RedrawTracks();

                        ShowStat();

                        if (TSWindow[0] == null)
                            RestartPlaying(true, true);
                        else
                            RestartPlayingTS(true, false);
                    }
                    break;
                default:
                    // Ctrl + Y
                    if (isCtrlDown && e.KeyValue == (int)'Y')
                    {
                        // Ctrl + A or Numpad 5
                        TracksKeyDown_DoRemoveLine(sender, e);
                    }
                    else if (isCtrlDown && (e.KeyValue == (int)'A' || e.KeyCode == Keys.NumPad5))
                    {
                        // Ctrl + I
                        Tracks.SelectAll();
                    }
                    else if (isCtrlDown && e.KeyValue == (int)'I')
                    {
                        // Note, Noise, Envelope keys, etc...
                        TracksKeyDown_DoInsertLine(true, sender, e);
                    }
                    else if (Tracks.KeyPressed != e.KeyCode)
                    {
                        if (NotePoses.Contains(Tracks.CursorX))
                            TracksKeyDown_DoNoteKey(sender, e);
                        else if (EnvelopePoses.Contains(Tracks.CursorX))
                        {
                            if (MainForm.EnvelopeAsNote || isShiftDown)
                                TracksKeyDown_DoNoteInEnvelope(sender, e);
                            else
                                TracksKeyDown_DoOtherKeys(sender, e);
                        }
                        else
                            TracksKeyDown_DoOtherKeys(sender, e);
                    }
                    break;
            }
        }

        public void Samples_KeyUp(object sender, KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool isCtrlDown = (e.Modifiers & Keys.Control) == Keys.Control;
            bool isAltDown = (e.Modifiers & Keys.Alt) == Keys.Alt;

            if (isShiftDown && Samples.IsLineTesting)
                SampleTestLine.TestLine_KeyUp(sender, e);

            if (!isShiftDown && Samples.IsLineTesting)
            {
                if (AY.PlayMode == PlayModes.PlayLine && WaveOutAPI.IsPlaying)
                    WaveOutAPI.ResetPlaying();

                PlayStopState = PlayStopState.Play;
                SampleTestLine.TestLine_Leave(sender, e);
                Samples.IsLineTesting = false;
            }
        }

        public void SaveSyncSample()
        {
            MainForm.SyncBufferBlocked = true;

            using (FileStream fileStream = new FileStream(MainForm.SyncSampleBufferFile, FileMode.Create))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    Sample sample = Main.BuffSample;
                    for (int i = 0; i < sample.Length; i++)
                    {
                        streamWriter.Write(VTModule.GetSampleString(sample.Ticks[i], false, false));
                        if (i == sample.Loop)
                        {
                            streamWriter.Write(" L");
                        }
                    }
                }
            }

            MainForm.SyncSampleBufferFileAge = Globals.FileAge(MainForm.SyncSampleBufferFile);
            MainForm.SyncBufferBlocked = false;
        }

        public void CopySampleToBuffer(bool all)
        {
            int ff;
            int sampleLength;

            if (!all && !Samples.IsSelecting && !Samples.IsColSelecting)
                return;

            MainForm.LastClipboard = LastClipboard.Samples;

            ValidateSample2(SampleIndex);

            if (Samples.IsColSelecting)
            {
                MainForm.SampleCopy.SrcWindow = this;
                MainForm.SampleCopy.Ready = true;

                Samples.IsColSelecting = false;

                Samples.HideCaret();
                Samples.Redraw();
                Samples.ShowCaret();

                MainForm.SyncBufferBlocked = true;

                using (FileStream fileStream = new FileStream(MainForm.SyncSamplePartFile, FileMode.Create))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.WriteLine((MainForm.SampleCopy.FromColumn).ToString());
                        streamWriter.WriteLine((MainForm.SampleCopy.ToColumn).ToString());
                        streamWriter.WriteLine((MainForm.SampleCopy.FromLine).ToString());
                        streamWriter.WriteLine((MainForm.SampleCopy.ToLine).ToString());
                    }
                }

                MainForm.SyncSamplePartFileAge = Globals.FileAge(MainForm.SyncSamplePartFile);
                MainForm.SyncBufferBlocked = false;

                Main.BuffSample.Ticks = Samples.ShownSample.Ticks;
                Main.BuffSample.Loop = 0;
                Main.BuffSample.Length = 63;

                SaveSyncSample();
                return;
            }

            MainForm.SampleCopy.Ready = false;

            if (all)
            {
                sampleLength = Samples.ShownSample.Length;
                Samples.SelectionStart = 0;
            }
            else
                sampleLength = Samples.SelectionEnd - Samples.SelectionStart + 1;

            if (sampleLength > VTModule.MaxSampleLength)
                sampleLength = VTModule.MaxSampleLength;

            for (ff = 0; ff <= sampleLength; ff++)
                Main.BuffSample.Ticks[ff] = Samples.ShownSample.Ticks[ff + Samples.SelectionStart];

            // If copied entire sample (Ctrl+A)
            if (all || (sampleLength == VTModule.MaxSampleLength))
            {
                Main.BuffSample.Loop = Samples.ShownSample.Loop;
                Main.BuffSample.Length = Samples.ShownSample.Length;
            }
            else
            {
                Main.BuffSample.Loop = 0;
                Main.BuffSample.Length = (byte)sampleLength;
            }

            // Save sample to copy/paste buffer file
            SaveSyncSample();
            SamplesSelectionOff();
        }

        public bool PastePatternToSample_Bit(byte aValue, byte bit)
        {
            return (aValue & (1 << bit)) != 0;
        }

        public void PastePatternToSample()
        {
            PlayLineResult result;
            int sampleLine, patternIndex;
            byte channel;
            ushort baseNoteFreq, freqAccum;
            SampleTick sampleTick;
            Sample newSample;
            bool isInitialized;
            ChannelParams channelParams;
            AYRegisters ayRegisters;
            byte curSample, toneBit, noiseBit;
            Pattern savedPattern;
            ChannelLine patChannelLine;
            VTM srcVTM;
            ChildForm srcWindow;

            if (MainForm.LastClipboard != LastClipboard.Tracks)
                return;

            ValidateSample2(SampleIndex);
            SaveSampleUndo(Samples.ShownSample);

            srcWindow = MainForm.TracksCopy.SrcWindow;
            srcVTM = srcWindow.VTM;
            savedPattern = (Pattern)MainForm.TracksCopy.Pattern;

            if (!MainForm.TracksCopy.Command || !MainForm.TracksCopy.Ornament)
            {
                for (int i = 0; i < MainForm.TracksCopy.Pattern.Length; i++)
                {
                    patChannelLine = MainForm.TracksCopy.Pattern.Lines[i].Channel[MainForm.TracksCopy.Channel];

                    // Remove commands from pattern if user doesn't select commands column
                    if (!MainForm.TracksCopy.Command)
                    {
                        // Remove some commands
                        if (patChannelLine.AdditionalCommand.Number == 1 || patChannelLine.AdditionalCommand.Number == 6)
                        {
                            patChannelLine.AdditionalCommand.Number = 0;
                            patChannelLine.AdditionalCommand.Parameter = 0;
                        }

                        // Remove commands delay
                        patChannelLine.AdditionalCommand.Delay = 0;
                    }

                    // Remove ornaments if user doesn't select ornaments column
                    if (!MainForm.TracksCopy.Ornament && patChannelLine.Ornament > 0)
                        patChannelLine.Ornament = 0;
                }
            }

            isInitialized = false;
            newSample = (Sample)Samples.ShownSample;
            sampleLine = Samples.CurrentLine() - 1;
            baseNoteFreq = (ushort)VTModule.GetNoteFreq(VTM.NoteTable, VTM.ReservedPattern.Lines[1].Channel[0].Note);
            channel = MainForm.TracksCopy.Channel;
            patternIndex = MainForm.TracksCopy.FromLine;
            freqAccum = 0;

            AY.ChipCount = 1;
            SetPlayingWindow(0, srcWindow);

            toneBit = 0;
            noiseBit = 0;

            switch (channel)
            {
                case 0:
                    toneBit = 0;
                    noiseBit = 3;
                    break;
                case 1:
                    toneBit = 1;
                    noiseBit = 4;
                    break;
                case 2:
                    toneBit = 2;
                    noiseBit = 5;
                    break;
            }

            RerollToLineNum(0, MainForm.TracksCopy.FromLine, MainForm.TracksCopy.FromLine == 0, srcVTM);
            result = PlayLineResult.Normal;

            do
            {
                sampleLine++;

                if (isInitialized)
                    result = VTModule.Pattern_PlayCurrentLine();

                if (result == PlayLineResult.LineEnded)
                    patternIndex++;

                if (patternIndex > MainForm.TracksCopy.ToLine && result == PlayLineResult.LineEnded)
                    break;

                if (result == PlayLineResult.PatternEnded)
                    break;

                if (sampleLine == VTModule.MaxSampleLength)
                {
                    MessageBox.Show(this, "Maximum Sample Length Reached", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                }

                ayRegisters = AY.SoundChip[0].AYRegisters;
                channelParams = VTModule.PlayArgs[0].ChannelParams[channel];
                curSample = srcVTM.ChannelStates[channel].Sample;

                // Calculate frequency accumulation
                for (int i = 0; i < sampleLine; i++)
                {
                    sampleTick = newSample.Ticks[i];
                    if (sampleTick.Ton_Accumulation)
                        freqAccum += (ushort)sampleTick.AddToTone;
                }

                // Set Tone, Masks and Volume
                newSample.Ticks[sampleLine].AddToTone = (short)(channelParams.Tone - baseNoteFreq - freqAccum);
                newSample.Ticks[sampleLine].Mixer_Ton = !PastePatternToSample_Bit(ayRegisters.Mixer, toneBit);
                newSample.Ticks[sampleLine].Mixer_Noise = !PastePatternToSample_Bit(ayRegisters.Mixer, noiseBit);
                newSample.Ticks[sampleLine].Amplitude = (byte)(channelParams.Amplitude & 0xf);
                newSample.Ticks[sampleLine].Envelope_Enabled = srcVTM.Samples[curSample].Ticks[channelParams.SamplePrevPosition].Envelope_Enabled;

                // Note R--
                if (srcVTM.Patterns[MainForm.TracksCopy.PatNum].Lines[patternIndex].Channel[channel].Note == -2)
                {
                    newSample.Ticks[sampleLine].Mixer_Ton = false;
                    newSample.Ticks[sampleLine].Mixer_Noise = false;
                    newSample.Ticks[sampleLine].AddToTone = 0;
                }

                // Envelope or Noise param
                if (!newSample.Ticks[sampleLine].Mixer_Noise)
                    newSample.Ticks[sampleLine].Add_to_Envelope_or_Noise = VTModule.PlayArgs[0].AddToEnv;
                else
                    newSample.Ticks[sampleLine].Add_to_Envelope_or_Noise = (sbyte)VTModule.PlayArgs[0].PT3Noise;

                isInitialized = true;
            }
            while (true);

            newSample.Length = (byte)sampleLine;

            // Restore pattern
            MainForm.TracksCopy.Pattern.Lines = savedPattern.Lines;

            // Copy sample
            for (int i = 0; i < 64; i++)
                Samples.ShownSample.Ticks[i] = newSample.Ticks[i];

            Samples.ShownSample.Length = newSample.Length;
            Samples.ShownSample.Loop = newSample.Loop;

            // Redraw samples
            SampleLengthUpDown.Value = Samples.ShownSample.Length;
            Samples.HideCaret();
            Samples.Redraw();
            Samples.ShowCaret();

            SongChanged = true;
            BackupSongChanged = true;
            SaveSampleRedo();
        }

        public void PasteOrnamentToSample()
        {
            int ornamentLine, sampleLine;
            sbyte baseNote, note;

            ValidateSample2(SampleIndex);
            SaveSampleUndo(Samples.ShownSample);
            sampleLine = Samples.CurrentLine();
            baseNote = VTM.ReservedPattern.Lines[1].Channel[0].Note;
            for (ornamentLine = 0; ornamentLine < Main.BuffOrnament.Length; ornamentLine++)
            {
                note = (sbyte)(baseNote + Main.BuffOrnament.Offsets[ornamentLine]);
                Samples.SetNote(note, sampleLine, 0xf, false, false, true);
                sampleLine++;
                if (sampleLine == VTModule.MaxSampleLength)
                {
                    MessageBox.Show(this, "Maximum Sample Length Reached", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                }
            }
            Samples.ShownSample.Length = (byte)sampleLine;
            SampleLengthUpDown.Value = sampleLine;
            Samples.HideCaret();
            Samples.Redraw();
            Samples.ShowCaret();
            SongChanged = true;
            BackupSongChanged = true;
            SaveSampleRedo();
        }

        public void PasteSampleFromBuffer(bool all)
        {
            int ff, jj, ll, ii, line, srcLine;

            if (MainForm.LastClipboard == LastClipboard.Tracks)
            {
                PastePatternToSample();
                return;
            }

            if (MainForm.LastClipboard == LastClipboard.Ornaments)
            {
                PasteOrnamentToSample();
                return;
            }

            SongChanged = true;
            BackupSongChanged = true;

            // Paste sample columns
            if (MainForm.SampleCopy.Ready)
            {
                SaveSampleUndo(Samples.ShownSample);
                srcLine = MainForm.SampleCopy.FromLine;
                for (line = Samples.CurrentLine(); line <= Samples.CurrentLine() + MainForm.SampleCopy.ToLine - MainForm.SampleCopy.FromLine; line++)
                {
                    for (jj = MainForm.SampleCopy.FromColumn; jj <= MainForm.SampleCopy.ToColumn; jj++)
                    {
                        switch (jj)
                        {
                            case 1:
                                Samples.ShownSample.Ticks[line].Mixer_Ton = MainForm.SampleCopy.Sample.Ticks[srcLine].Mixer_Ton;
                                break;
                            case 2:
                                Samples.ShownSample.Ticks[line].Mixer_Noise = MainForm.SampleCopy.Sample.Ticks[srcLine].Mixer_Noise;
                                break;
                            case 3:
                                Samples.ShownSample.Ticks[line].Envelope_Enabled = MainForm.SampleCopy.Sample.Ticks[srcLine].Envelope_Enabled;
                                break;
                            case 4:
                                Samples.ShownSample.Ticks[line].AddToTone = MainForm.SampleCopy.Sample.Ticks[srcLine].AddToTone;
                                Samples.ShownSample.Ticks[line].Ton_Accumulation = MainForm.SampleCopy.Sample.Ticks[srcLine].Ton_Accumulation;
                                break;
                            case 5:
                                Samples.ShownSample.Ticks[line].Envelope_or_Noise_Accumulation = MainForm.SampleCopy.Sample.Ticks[srcLine].Envelope_or_Noise_Accumulation;
                                Samples.ShownSample.Ticks[line].Add_to_Envelope_or_Noise = MainForm.SampleCopy.Sample.Ticks[srcLine].Add_to_Envelope_or_Noise;
                                break;
                            case 6:
                                Samples.ShownSample.Ticks[line].Amplitude = MainForm.SampleCopy.Sample.Ticks[srcLine].Amplitude;
                                Samples.ShownSample.Ticks[line].Amplitude_Sliding = MainForm.SampleCopy.Sample.Ticks[srcLine].Amplitude_Sliding;
                                Samples.ShownSample.Ticks[line].Amplitude_Slide_Up = MainForm.SampleCopy.Sample.Ticks[srcLine].Amplitude_Slide_Up;
                                break;
                        }
                    }

                    srcLine++;

                    if (Samples.ShownSample.Length < line + 1)
                        Samples.ShownSample.Length = (byte)(line + 1);
                }

                SampleLengthUpDown.Value = Samples.ShownSample.Length;
                Samples.HideCaret();
                Samples.Redraw();
                Samples.ShowCaret();
                SaveSampleRedo();
                return;
            }

            ValidateSample2(SampleIndex);
            SaveSampleUndo(Samples.ShownSample);
            GetSamParams(out ll, out ii);

            if (all)
            {
                ii = 0;
                ClearShownSample();
            }

            // Paste part of sample
            for (ff = 0; ff < Main.BuffSample.Length; ff++)
            {
                if (ff + ii < 64)
                {
                    Samples.ShownSample.Ticks[ff + ii] = Main.BuffSample.Ticks[ff];
                    if (ff + ii >= Samples.ShownSample.Length)
                        Samples.ShownSample.Length = (byte)(ff + ii + 1);
                }
            }

            if (Main.BuffSample.Loop != 0)
                Samples.ShownSample.Loop = (byte)(ii + Main.BuffSample.Loop);

            SampleLengthUpDown.Value = Samples.ShownSample.Length;
            SampleLoopUpDown.Value = Samples.ShownSample.Loop;

            SamplesSelectionOff();
            SaveSampleRedo();
        }

        public void GetSamParams(out int l, out int i)
        {
            l = Samples.ShownSample == null ? 1 : Samples.ShownSample.Length;
            i = Samples.ShownFrom + Samples.CursorY;
        }

        public void SamplesSelectionOff()
        {
            Samples.IsSelecting = false;
            Samples.IsColSelecting = false;
            Samples.HideCaret();
            Samples.Redraw();
            Samples.ShowCaret();
        }

        public void SamplesKeyDown_DoToggle(TSamToggles n, object sender, KeyEventArgs e)
        {
            int i, l;
            SamplesSelectionOff();
            GetSamParams(out l, out i);

            if (i >= l)
                return;

            SongChanged = true;
            BackupSongChanged = true;

            ValidateSample2(SampleIndex);

            SampleTick st = new SampleTick();
            st = Samples.ShownSample.Ticks[i];
            AddUndo(TChangeAction.ChangeSampleValue, st, i);
            SampleTick sampleTick = Samples.ShownSample.Ticks[i];

            switch (n)
            {
                case TSamToggles.MixTone:
                    sampleTick.Mixer_Ton = !sampleTick.Mixer_Ton;
                    break;
                case TSamToggles.MixNoise:
                    sampleTick.Mixer_Noise = !sampleTick.Mixer_Noise;
                    break;
                case TSamToggles.MaskEnv:
                    sampleTick.Envelope_Enabled = !sampleTick.Envelope_Enabled;
                    break;
                case TSamToggles.SgnTone:
                    if (!Samples.ToneShiftAsNote)
                        sampleTick.AddToTone = (short)-sampleTick.AddToTone;
                    break;
                case TSamToggles.SgnToneP:
                    if (!Samples.ToneShiftAsNote)
                        sampleTick.AddToTone = Math.Abs(sampleTick.AddToTone);
                    break;
                case TSamToggles.SgnToneM:
                    if (!Samples.ToneShiftAsNote)
                        sampleTick.AddToTone = (short)-Math.Abs(sampleTick.AddToTone);
                    break;
                case TSamToggles.SgnNoise:
                    sampleTick.Add_to_Envelope_or_Noise = Ns(-sampleTick.Add_to_Envelope_or_Noise);
                    break;
                case TSamToggles.SgnNoiseP:
                    sampleTick.Add_to_Envelope_or_Noise = Ns(Math.Abs(sampleTick.Add_to_Envelope_or_Noise));
                    break;
                case TSamToggles.SgnNoiseM:
                    sampleTick.Add_to_Envelope_or_Noise = Ns(-Math.Abs(sampleTick.Add_to_Envelope_or_Noise));
                    break;
                case TSamToggles.AccTone:
                    sampleTick.Ton_Accumulation = !sampleTick.Ton_Accumulation;
                    break;
                case TSamToggles.AccNoise:
                    sampleTick.Envelope_or_Noise_Accumulation = !sampleTick.Envelope_or_Noise_Accumulation;
                    break;
                case TSamToggles.AccVol:
                    if (!sampleTick.Amplitude_Sliding)
                    {
                        sampleTick.Amplitude_Sliding = true;
                        sampleTick.Amplitude_Slide_Up = false;
                    }
                    else if (!sampleTick.Amplitude_Slide_Up)
                        sampleTick.Amplitude_Slide_Up = true;
                    else
                        sampleTick.Amplitude_Sliding = false;
                    break;
                case TSamToggles.AccVolP:
                    sampleTick.Amplitude_Sliding = true;
                    sampleTick.Amplitude_Slide_Up = true;
                    break;
                case TSamToggles.AccVolM:
                    sampleTick.Amplitude_Sliding = true;
                    sampleTick.Amplitude_Slide_Up = false;
                    break;
                case TSamToggles.AccVol_:
                    sampleTick.Amplitude_Sliding = false;
                    break;
                case TSamToggles.AccTone_:
                    sampleTick.Ton_Accumulation = false;
                    break;
                case TSamToggles.AccNoise_:
                    sampleTick.Envelope_or_Noise_Accumulation = false;
                    break;
                case TSamToggles.AccToneA:
                    sampleTick.Ton_Accumulation = true;
                    break;
                case TSamToggles.AccNoiseA:
                    sampleTick.Envelope_or_Noise_Accumulation = true;
                    break;
            }

            Samples.HideCaret();
            Samples.Redraw();
            Samples.ShowCaret();
        }

        public void SamplesKeyDown_DoToggleSpace(object sender, KeyEventArgs e)
        {
            SamplesSelectionOff();

            if (Samples.ToneShiftAsNote && (Samples.CursorX >= 4 && Samples.CursorX <= 7))
                return;

            switch (Samples.CursorX)
            {
                // 0 .. 2
                case 0:
                case 1:
                case 2:
                    SamplesKeyDown_DoToggle((TSamToggles)Samples.CursorX, sender, e);
                    break;
                // 4 .. 7
                case 4:
                case 5:
                case 6:
                case 7:
                    SamplesKeyDown_DoToggle(TSamToggles.SgnTone, sender, e);
                    break;
                case 8:
                    SamplesKeyDown_DoToggle(TSamToggles.AccTone, sender, e);
                    break;
                // 10 .. 15
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    SamplesKeyDown_DoToggle(TSamToggles.SgnNoise, sender, e);
                    break;
                case 17:
                    SamplesKeyDown_DoToggle(TSamToggles.AccNoise, sender, e);
                    break;
                case 19:
                case 20:
                    SamplesKeyDown_DoToggle(TSamToggles.AccVol, sender, e);
                    break;
            }
        }

        public void SamplesKeyDown_DoTogglePlus(object sender, KeyEventArgs e)
        {
            SamplesSelectionOff();

            if (Samples.ToneShiftAsNote && (Samples.CursorX >= 4 && Samples.CursorX <= 7))
                return;

            switch (Samples.CursorX)
            {
                // 4 .. 7
                case 4:
                case 5:
                case 6:
                case 7:
                    SamplesKeyDown_DoToggle(TSamToggles.SgnToneP, sender, e);
                    break;
                // 10 .. 15
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    SamplesKeyDown_DoToggle(TSamToggles.SgnNoiseP, sender, e);
                    break;
                case 19:
                case 20:
                    SamplesKeyDown_DoToggle(TSamToggles.AccVolP, sender, e);
                    break;
            }
        }

        public void SamplesKeyDown_DoToggleMinus(object sender, KeyEventArgs e)
        {
            SamplesSelectionOff();

            if (Samples.ToneShiftAsNote && (Samples.CursorX >= 4 && Samples.CursorX <= 7))
                return;

            switch (Samples.CursorX)
            {
                // 4 .. 7
                case 4:
                case 5:
                case 6:
                case 7:
                    SamplesKeyDown_DoToggle(TSamToggles.SgnToneM, sender, e);
                    break;
                // 10 .. 15
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    SamplesKeyDown_DoToggle(TSamToggles.SgnNoiseM, sender, e);
                    break;
                case 19:
                case 20:
                    SamplesKeyDown_DoToggle(TSamToggles.AccVolM, sender, e);
                    break;
            }
        }

        public void SamplesKeyDown_DoToggleAccA(object sender, KeyEventArgs e)
        {
            SamplesSelectionOff();

            if (Samples.ToneShiftAsNote && (Samples.CursorX >= 4 && Samples.CursorX <= 7))
                return;

            switch (Samples.CursorX)
            {
                // 4 .. 8
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    SamplesKeyDown_DoToggle(TSamToggles.AccToneA, sender, e);
                    break;
                // 10 .. 17
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                    SamplesKeyDown_DoToggle(TSamToggles.AccNoiseA, sender, e);
                    break;
            }
        }

        public void SamplesKeyDown_DoToggle_(object sender, KeyEventArgs e)
        {
            SamplesSelectionOff();

            if (Samples.ToneShiftAsNote && (Samples.CursorX >= 4 && Samples.CursorX <= 7))
                return;

            switch (Samples.CursorX)
            {
                // 4 .. 8
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    SamplesKeyDown_DoToggle(TSamToggles.AccTone_, sender, e);
                    break;
                // 10 .. 17
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                case 16:
                case 17:
                    SamplesKeyDown_DoToggle(TSamToggles.AccNoise_, sender, e);
                    break;
                case 19:
                case 20:
                    SamplesKeyDown_DoToggle(TSamToggles.AccVol_, sender, e);
                    break;
            }
        }

        public void SamplesKeyDown_DoNumber(TSamNumbers n, object sender, KeyEventArgs e)
        {
            int i, l;

            if (Samples.ToneShiftAsNote && (Samples.CursorX >= 4 && Samples.CursorX <= 7))
                return;

            SamplesSelectionOff();
            GetSamParams(out l, out i);

            if (i >= l)
                return;

            SongChanged = true;
            BackupSongChanged = true;
            ValidateSample2(SampleIndex);
            SampleTick st = Samples.ShownSample.Ticks[i];
            AddUndo(TChangeAction.ChangeSampleValue, st, i);
            SampleTick sampleTick = Samples.ShownSample.Ticks[i];

            switch (n)
            {
                case TSamNumbers.Tone:
                    if (sampleTick.AddToTone < 0)
                        sampleTick.AddToTone = (short)-Samples.InputSNumber;
                    else
                        sampleTick.AddToTone = (short)Samples.InputSNumber;
                    break;
                case TSamNumbers.Noise:
                    if (sampleTick.Add_to_Envelope_or_Noise < 0)
                        sampleTick.Add_to_Envelope_or_Noise = Ns(-Samples.InputSNumber);
                    else
                        sampleTick.Add_to_Envelope_or_Noise = Ns(Samples.InputSNumber);
                    break;
                case TSamNumbers.NoiseAbs:
                    sampleTick.Add_to_Envelope_or_Noise = Ns(Samples.InputSNumber);
                    break;
                case TSamNumbers.Vol:
                    sampleTick.Amplitude = (byte)Samples.InputSNumber;
                    break;
            }

            Samples.HideCaret();
            Samples.Redraw();
            Samples.ShowCaret();
        }

        public void SamplesKeyDown_DoDigit(int n, object sender, KeyEventArgs e)
        {
            int nm;

            SamplesSelectionOff();

            if (Samples.ToneShiftAsNote && (Samples.CursorX >= 4 && Samples.CursorX <= 8))
                return;

            if (MainForm.DecBaseNoiseOn && (Samples.CursorX == 10 || Samples.CursorX == 11 || Samples.CursorX == 14 || Samples.CursorX == 17))
                nm = Samples.InputSNumber * 10 + n;
            else
                nm = Samples.InputSNumber * 16 + n;

            switch (Samples.CursorX)
            {
                // 4 .. 8
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    if (nm > 0xFFF)
                        nm = n;

                    Samples.InputSNumber = nm;
                    SamplesKeyDown_DoNumber(TSamNumbers.Tone, sender, e);
                    break;
                case 10:
                case 11:
                    if (nm > 0x10)
                        nm = n;

                    Samples.InputSNumber = nm;
                    SamplesKeyDown_DoNumber(TSamNumbers.Noise, sender, e);
                    break;
                case 14:
                case 17:
                    if (nm > 0x1F)
                        nm = n;

                    Samples.InputSNumber = nm;
                    SamplesKeyDown_DoNumber(TSamNumbers.NoiseAbs, sender, e);
                    break;
                case 19:
                case 20:
                    if (nm > 0xF)
                        nm = n;

                    Samples.InputSNumber = nm;
                    ValidateSample2(SampleIndex);
                    if (Samples.ShownSample.Length <= Samples.ShownFrom + Samples.CursorY)
                        Samples.ShownSample.Length = (byte)(Samples.ShownFrom + Samples.CursorY + 1);
                    SamplesKeyDown_DoNumber(TSamNumbers.Vol, sender, e);
                    break;
            }
        }

        public void Samples_KeyDown(object sender, KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool isCtrlDown = (e.Modifiers & Keys.Control) == Keys.Control;
            bool isAltDown = (e.Modifiers & Keys.Alt) == Keys.Alt;
            bool isNoneDown = (e.Modifiers == Keys.None);

            SampleTick sampleTick;
            int ff;
            int ll, ii;
            int i;
            // for, len , position
            NoteTableType currentToneTable;
            int noteFreq1;
            int noteFreq2;
            // envshift: ShortInt;
            int envFreq;
            bool incr = (e.KeyCode == Keys.Add) || (e.KeyCode == Keys.Oemplus);
            bool decr = (e.KeyCode == Keys.Subtract) || (e.KeyCode == Keys.OemMinus);

            // Increase/Descrease selected columns
            if (incr || decr)
            {
                if (Samples.ToneShiftAsNote && (Samples.CursorX >= 4 && Samples.CursorX <= 8))
                    return;

                if (!Samples.IsColSelecting && !Samples.IsSelecting)
                    ResetSampleVolumeBuf();

                if (incr)
                    IncreaseSampleCols();

                if (decr)
                    DecreaseSampleCols();

                return;
            }

            // Delete selected cols
            if (Samples.IsColSelecting && (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete))
            {
                ClearSampleCols();
                return;
            }

            // Exit testline if Shift+Note pressed
            if (!isShiftDown)
            {
                SampleTestLine.KeyPressed = 0;
                SampleTestLine.TestLine_Leave(sender, e);
            }

            // Alt+1..8 - change octave
            if (isAltDown && e.KeyValue >= 49 && e.KeyValue <= 56)
            {
                SampleOctaveNum.Value = (decimal)(e.KeyValue - 48);
                SampleTestLine.TestOct = (int)SampleOctaveNum.Value;
                return;
            }

            // Numpad 1-8 - change octave
            if (e.KeyCode >= Keys.NumPad1 && e.KeyCode <= Keys.NumPad8)
            {
                SampleOctaveNum.Value = e.KeyCode - Keys.NumPad0;
                return;
            }

            // Set note in tone shift position
            if (Samples.ToneShiftAsNote && (Samples.CursorX == 4 || Samples.CursorX == 5) && (MainForm.NoteKeys[e.KeyValue] != -3) && !isShiftDown)
            {
                if (e.KeyValue >= 256)
                    return;

                Samples.SetNote(MainForm.NoteKeys[e.KeyValue], Samples.CurrentLine(), -1, true, true, false);
                return;
            }

            if (!isNoneDown || !((e.KeyValue >= (int)'0' && e.KeyValue <= (int)'9') || (e.KeyValue >= (int)'A' && e.KeyValue <= (int)'F')))
                Samples.InputSNumber = 0;

            ff = (int)'C';
            ll = ff;

            if (isNoneDown || Samples.MouseDoubleClicked)
            {
                switch (e.KeyCode)
                {
                    case Keys.Next:
                        if (Samples.CursorY < Samples.LineCount - 1)
                        {
                            Samples.CursorY = Samples.LineCount - 1;
                            Samples.SetCaretPosition();
                        }
                        else if (Samples.ShownFrom < VTModule.MaxSampleLength - Samples.LineCount)
                        {
                            Samples.ShownFrom += Samples.LineCount;

                            if (Samples.ShownFrom > VTModule.MaxSampleLength - Samples.LineCount)
                                Samples.ShownFrom = VTModule.MaxSampleLength - Samples.LineCount;

                            Samples.HideCaret();
                            Samples.Redraw();
                            Samples.ShowCaret();
                        }
                        break;
                    case Keys.Prior:
                        if (Samples.CursorY > 0)
                        {
                            Samples.CursorY = 0;
                            Samples.SetCaretPosition();
                        }
                        else if (Samples.ShownFrom > 0)
                        {
                            Samples.ShownFrom -= Samples.LineCount;

                            if (Samples.ShownFrom < 0)
                                Samples.ShownFrom = 0;

                            Samples.HideCaret();
                            Samples.Redraw();
                            Samples.ShowCaret();
                        }
                        break;
                    case Keys.Home:
                        if (Samples.CursorX != 0)
                        {
                            Samples.CursorX = 0;
                            Samples.RecreateCaret();
                            Samples.SetCaretPosition();
                        }
                        break;
                    case Keys.End:
                        if (Samples.CursorX != 20)
                        {
                            Samples.CursorX = 20;
                            Samples.RecreateCaret();
                            Samples.SetCaretPosition();
                        }
                        break;
                    case Keys.Down:
                        if (!Samples.MouseDoubleClicked)
                            SamplesSelectionOff();

                        if (Samples.CursorY < Samples.LineCount - 1)
                        {
                            Samples.CursorY++;
                            Samples.SetCaretPosition();
                        }
                        else if (Samples.ShownFrom < VTModule.MaxSampleLength - Samples.LineCount)
                        {
                            Samples.ShownFrom++;
                            Samples.HideCaret();
                            Samples.Redraw();
                            Samples.ShowCaret();
                        }
                        break;
                    case Keys.Up:
                        if (!Samples.MouseDoubleClicked)
                            SamplesSelectionOff();

                        if (Samples.CursorY > 0)
                        {
                            Samples.CursorY--;
                            Samples.SetCaretPosition();
                        }
                        else if (Samples.ShownFrom > 0)
                        {
                            Samples.ShownFrom--;
                            Samples.HideCaret();
                            Samples.Redraw();
                            Samples.ShowCaret();
                        }
                        break;
                    case Keys.Left:
                        if (!Samples.MouseDoubleClicked)
                            SamplesSelectionOff();

                        if (Samples.CursorX > 0)
                        {
                            if (Samples.CursorX == 4 || Samples.CursorX == 10 || Samples.CursorX == 19)
                                Samples.CursorX -= 2;
                            else if (Samples.CursorX == 8 || Samples.CursorX == 14 || Samples.CursorX == 17)
                                Samples.CursorX -= 3;
                            else
                                Samples.CursorX--;
                            Samples.RecreateCaret();
                            Samples.SetCaretPosition();
                        }
                        break;
                    case Keys.Right:
                        if (!Samples.MouseDoubleClicked)
                            SamplesSelectionOff();

                        if (Samples.CursorX < 20)
                        {
                            Samples.CursorX++;
                            if (Samples.CursorX == 3 || Samples.CursorX == 9 || Samples.CursorX == 13 || Samples.CursorX == 16 || Samples.CursorX == 18)
                                Samples.CursorX++;
                            else if (Samples.CursorX == 6)
                                Samples.CursorX = 8;
                            else if (Samples.CursorX == 12)
                                Samples.CursorX = 14;
                            else if (Samples.CursorX == 15)
                                Samples.CursorX = 17;
                            Samples.RecreateCaret();
                            Samples.SetCaretPosition();
                        }
                        break;
                    case Keys.Delete:
                        if (isCtrlDown && isShiftDown)
                        {
                        }
                        else if (isShiftDown)
                        {
                        }
                        else if (isCtrlDown)
                        {
                            // next position
                        }
                        else
                        {
                            // delete position of sample.
                            // Samples.CursorY;
                            ValidateSample2(SampleIndex);
                            GetSamParams(out ll, out ii);

                            if (Samples.ShownSample.Length > 0 && ii < (ll - 1))
                            {
                                Samples.ShownSample.Length = (byte)(Samples.ShownSample.Length - 1);
                                if (Samples.ShownSample.Loop > ii)
                                    Samples.ShownSample.Loop = (byte)(Samples.ShownSample.Loop - 1);
                            }

                            for (ff = ii; ff < 63; ff++)
                                Samples.ShownSample.Ticks[ff] = Samples.ShownSample.Ticks[ff + 1];

                            Samples.ShownSample.Ticks[63].AddToTone = 0;
                            Samples.ShownSample.Ticks[63].AddToTone = 0;
                            Samples.ShownSample.Ticks[63].Ton_Accumulation = false;
                            Samples.ShownSample.Ticks[63].Amplitude = 0;
                            Samples.ShownSample.Ticks[63].Amplitude_Sliding = false;
                            Samples.ShownSample.Ticks[63].Amplitude_Slide_Up = false;
                            Samples.ShownSample.Ticks[63].Envelope_Enabled = false;
                            Samples.ShownSample.Ticks[63].Envelope_or_Noise_Accumulation = false;
                            Samples.ShownSample.Ticks[63].Add_to_Envelope_or_Noise = 0;
                            Samples.ShownSample.Ticks[63].Mixer_Ton = false;
                            Samples.ShownSample.Ticks[63].Mixer_Noise = false;

                            SampleLengthUpDown.Value = Samples.ShownSample.Length;
                            SampleLoopUpDown.Value = Samples.ShownSample.Loop;

                            Samples.HideCaret();
                            Samples.Redraw();
                            Samples.ShowCaret();
                        }
                        break;
                    case Keys.Insert:
                        if (isCtrlDown && isShiftDown)
                        {
                        }
                        else if (isShiftDown)
                        {
                        }
                        else if (isCtrlDown)
                        {
                            // next position
                        }
                        else
                        {
                            ValidateSample2(SampleIndex);
                            GetSamParams(out ll, out ii);

                            if (ll < 64 && ii <= ll)
                            {
                                Samples.ShownSample.Length = (byte)(Samples.ShownSample.Length + 1);
                                if (Samples.ShownSample.Loop >= ii)
                                    Samples.ShownSample.Loop = (byte)(Samples.ShownSample.Loop + 1);
                            }

                            for (ff = 62; ff >= ii; ff--)
                                Samples.ShownSample.Ticks[ff + 1] = Samples.ShownSample.Ticks[ff];

                            SampleLengthUpDown.Value = Samples.ShownSample.Length;
                            SampleLoopUpDown.Value = Samples.ShownSample.Loop;
                            Samples.HideCaret();
                            Samples.Redraw();
                            Samples.ShowCaret();
                        }
                        break;
                    case Keys.T:
                        SamplesKeyDown_DoToggle(TSamToggles.MixTone, sender, e);
                        break;
                    case Keys.N:
                        SamplesKeyDown_DoToggle(TSamToggles.MixNoise, sender, e);
                        break;
                    case Keys.M:
                        SamplesKeyDown_DoToggle(TSamToggles.MaskEnv, sender, e);
                        break;
                    case Keys.Space:
                        SamplesKeyDown_DoToggleSpace(sender, e);
                        break;
                    case Keys.Oemplus:
                    case Keys.Add:
                        SamplesKeyDown_DoTogglePlus(sender, e);
                        break;
                    case Keys.OemMinus:
                    case Keys.Subtract:
                        SamplesKeyDown_DoToggleMinus(sender, e);
                        break;
                    // (int)('0') .. (int)('9')
                    case Keys.D0:
                    case Keys.D1:
                    case Keys.D2:
                    case Keys.D3:
                    case Keys.D4:
                    case Keys.D5:
                    case Keys.D6:
                    case Keys.D7:
                    case Keys.D8:
                    case Keys.D9:
                        SamplesKeyDown_DoDigit(e.KeyValue - (int)'0', sender, e);
                        break;
                    // (int)('A') .. (int)('F')
                    case Keys.A:
                    case Keys.B:
                    case Keys.C:
                    case Keys.D:
                    case Keys.E:
                    case Keys.F:
                        if (!MainForm.DecBaseNoiseOn || !(Samples.CursorX == 10 || Samples.CursorX == 11 || Samples.CursorX == 14 || Samples.CursorX == 17))
                            SamplesKeyDown_DoDigit(e.KeyValue - (int)'A' + 10, sender, e);
                        else
                            return;
                        break;
                    case Keys.Oemtilde:
                        if (SampleTestLine.CanFocus)
                            SampleTestLine.Focus();
                        break;
                }
            }
            else if (isCtrlDown)
            {
                switch (e.KeyCode)
                {
                    // Ctrl + PgDown, Ctrl + End
                    case Keys.Next:
                    case Keys.End:
                        SamplesSelectionOff();
                        if (((e.KeyCode == Keys.End || e.KeyCode == Keys.Down) && Samples.CursorX != 20) || Samples.CursorY < Samples.LineCount - 1)
                        {
                            if (e.KeyCode == Keys.End || e.KeyCode == Keys.Down)
                            {
                                // Samples.CursorX := 20;
                                Samples.RecreateCaret();
                            }
                            Samples.CursorY = Samples.LineCount - 1;
                            Samples.SetCaretPosition();
                        }

                        if (Samples.ShownFrom < VTModule.MaxSampleLength - Samples.LineCount)
                        {
                            Samples.ShownFrom = VTModule.MaxSampleLength - Samples.LineCount;
                            Samples.HideCaret();
                            Samples.Redraw();
                            Samples.ShowCaret();
                        }
                        break;
                    // Ctrl + PgUp, Ctrl + Home
                    case Keys.Prior:
                    case Keys.Home:
                        SamplesSelectionOff();
                        if (((e.KeyCode == Keys.Home || e.KeyCode == Keys.Up) && Samples.CursorX != 0) || Samples.CursorY > 0)
                        {
                            if (e.KeyCode == Keys.Home || e.KeyCode == Keys.Up)
                            {
                                // Samples.CursorX := 0;
                                Samples.RecreateCaret();
                            }
                            Samples.CursorY = 0;
                            Samples.SetCaretPosition();
                        }

                        if (Samples.ShownFrom > 0)
                        {
                            Samples.ShownFrom = 0;
                            Samples.HideCaret();
                            Samples.Redraw();
                            Samples.ShowCaret();
                        }
                        break;
                    // Selecting columns
                    // Ctrl + Arrows
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Right:
                    case Keys.Left:
                        Samples.IsSelecting = false;
                        if (!Samples.IsColSelecting)

                        {
                            ResetSampleVolumeBuf();
                            MainForm.SampleCopy.StartLine = (byte)Samples.CurrentLine();
                            MainForm.SampleCopy.FromLine = MainForm.SampleCopy.StartLine;
                            MainForm.SampleCopy.ToLine = MainForm.SampleCopy.StartLine;
                            MainForm.SampleCopy.StartColumn = DetectSampleColumn(Samples.CursorX);
                            MainForm.SampleCopy.FromColumn = MainForm.SampleCopy.StartColumn;
                            MainForm.SampleCopy.ToColumn = MainForm.SampleCopy.StartColumn;
                            Samples.IsColSelecting = true;
                        }
                        else
                        {
                            // Tone jump
                            if (Samples.CursorX >= 4 && Samples.CursorX <= 8 && e.KeyCode == Keys.Left)
                                Samples.CursorX = 4;
                            else if (Samples.CursorX >= 4 && Samples.CursorX <= 8 && e.KeyCode == Keys.Right)
                                Samples.CursorX = 10;
                            // Noise
                            else if (Samples.CursorX >= 10 && Samples.CursorX <= 17 && e.KeyCode == Keys.Left)
                                Samples.CursorX = 5;
                            else if (Samples.CursorX >= 10 && Samples.CursorX <= 17 && e.KeyCode == Keys.Right)
                                Samples.CursorX = 19;

                            // Amplitude
                            else if (Samples.CursorX >= 19 && Samples.CursorX <= 20 && e.KeyCode == Keys.Right)
                                Samples.CursorX = 19;
                            else if (Samples.CursorX >= 19 && Samples.CursorX <= 20 && e.KeyCode == Keys.Left)
                                Samples.CursorX = 17;

                            // if Key = Keys.Down  then Inc(Samples.CursorY);
                            // if Key = Keys.Up    then Dec(Samples.CursorY);
                            // if Key = Keys.Left  then Inc(Samples.CursorX);
                            // if Key = Keys.Right then Dec(Samples.CursorX);
                            // 
                            // if Samples.CurrentLine <= 0 then Samples.CursorY := 0;
                            // if Samples.CurrentLine >= MaxSamLen-1 then Samples.CursorY := 0;
                            // if Samples.CursorX < 0 then Samples.CursorX := 0;
                            // if Samples.CursorX >= 20 then Samples.CursorX := 20;

                            Samples_KeyDown(sender, e);
                            MainForm.SampleCopy.ToLine = (byte)Samples.CurrentLine();
                            MainForm.SampleCopy.ToColumn = (byte)DetectSampleColumn(Samples.CursorX);

                            if (MainForm.SampleCopy.StartLine >= MainForm.SampleCopy.ToLine)
                            {
                                MainForm.SampleCopy.ToLine = MainForm.SampleCopy.StartLine;
                                MainForm.SampleCopy.FromLine = (byte)Samples.CurrentLine();
                            }

                            if (MainForm.SampleCopy.StartLine < MainForm.SampleCopy.ToLine)
                            {
                                MainForm.SampleCopy.FromLine = MainForm.SampleCopy.StartLine;
                                MainForm.SampleCopy.ToLine = (byte)Samples.CurrentLine();
                            }

                            if (MainForm.SampleCopy.StartColumn >= MainForm.SampleCopy.ToColumn)
                            {
                                i = MainForm.SampleCopy.ToColumn;
                                MainForm.SampleCopy.ToColumn = MainForm.SampleCopy.StartColumn;
                                MainForm.SampleCopy.FromColumn = (byte)i;
                            }

                            if (MainForm.SampleCopy.StartColumn < MainForm.SampleCopy.ToColumn)
                            {
                                i = MainForm.SampleCopy.ToColumn;
                                MainForm.SampleCopy.FromColumn = MainForm.SampleCopy.StartColumn;
                                MainForm.SampleCopy.ToColumn = (byte)i;
                            }
                        }

                        Samples.HideCaret();
                        Samples.Redraw();
                        Samples.ShowCaret();
                        break;
                    // Ctrl + Num+
                    case Keys.Add:
                        // next sample
                        ChangeSample(SampleIndex + 1, true, true);
                        break;
                    // Ctrl + Num-
                    case Keys.Subtract:
                        // previous sample
                        ChangeSample(SampleIndex - 1, true, true);
                        break;
                    // Ctrl + Delete
                    case Keys.Delete:
                        break;
                    // delete sample position
                    // ValidateSample2(Samples.);
                    // if then
                    // begin
                    // ChangeSample(StrToInt(Edit5.Text)-1);
                    // Edit5.Text:= IntToStr((StrToInt(Edit5.Text)-1));
                    // end;
                    // Keys.Insert:
                    // begin
                    // ValidateSample2(SamNum);
                    // GetSamParams(ll,ii);
                    // 
                    // if ii > ll-1 then
                    // begin
                    // for ff:= Samples.ShownSample.loop to ll do
                    // begin
                    // if ii + ff - Samples.ShownSample.loop <=63 then
                    // Samples.ShownSample.Items[ii+ff - Samples.ShownSample.loop ]:= Samples.ShownSample.Items[ff];
                    // end;
                    // 
                    // end
                    // else
                    // begin
                    // 
                    // end;
                    // 
                    // Edit9.Text:= IntToStr(Samples.ShownSample.Length);
                    // Edit10.Text:= IntToStr(Samples.ShownSample.Loop);
                    // Samples.HideCaret;
                    // Samples.Redraw();
                    // Samples.ShowCaret;
                    // end;
                    // Ctrl + Insert - Copy sample part to buffer
                    case Keys.Insert:
                        CopySampleToBuffer(false);
                        break;
                    case Keys.A:
                        // Ctrl + A
                        ValidateSample2(SampleIndex);
                        GetSamParams(out ll, out ii);
                        // Samples.ShownSample.Length := 64;
                        // Samples.ShownSample.Loop := 0;
                        if (!Samples.IsSelecting)
                        {
                            Samples.SelectionStart = 0;
                            Samples.SelectionEnd = 64;
                            Samples.IsSelecting = true;
                        }
                        else
                            Samples.IsSelecting = false;
                        // Edit9.Text := IntToStr(Samples.ShownSample.Length);
                        // Edit10.Text := IntToStr(Samples.ShownSample.Loop);
                        Samples.HideCaret();
                        Samples.Redraw();
                        Samples.ShowCaret();
                        break;
                        // Ord('C'): CTRL + INS
                        // end;
                        // Ord('V'): SHIFT + INS
                        // begin
                        // ll:= ll;
                        // end;
                }
            }
            else if (isShiftDown)
            {
                switch (e.KeyCode)
                {
                    case Keys.D6:
                        SamplesKeyDown_DoToggleAccA(sender, e);
                        break;
                    case Keys.Oemplus:
                        SamplesKeyDown_DoTogglePlus(sender, e);
                        break;
                    case Keys.OemMinus:
                        SamplesKeyDown_DoToggle_(sender, e);
                        break;
                    // Shift + Home
                    case Keys.Home:
                        Samples.IsColSelecting = false;
                        ValidateSample2(SampleIndex);
                        GetSamParams(out ll, out ii);

                        if (Samples.ShownSample.Length > 0 && ii < ll)
                            Samples.ShownSample.Loop = (byte)ii;

                        SampleLengthUpDown.Value = Samples.ShownSample.Length;
                        SampleLoopUpDown.Value = Samples.ShownSample.Loop;

                        Samples.HideCaret();
                        Samples.Redraw();
                        Samples.ShowCaret();
                        break;
                    // Shift + End
                    case Keys.End:
                        Samples.IsColSelecting = false;
                        ValidateSample2(SampleIndex);
                        GetSamParams(out ll, out ii);

                        Samples.ShownSample.Length = (byte)(ii + 1);
                        if (Samples.ShownSample.Loop > Samples.ShownSample.Length)
                            Samples.ShownSample.Loop = (byte)ii;

                        SampleLengthUpDown.Value = Samples.ShownSample.Length;
                        SampleLoopUpDown.Value = Samples.ShownSample.Loop;

                        Samples.HideCaret();
                        Samples.Redraw();
                        Samples.ShowCaret();
                        break;
                    // Shift + Insert
                    case Keys.Insert:
                        PasteSampleFromBuffer(false);
                        break;
                    // Shift + Left/Right
                    case Keys.Left:
                    case Keys.Right:
                        Samples.IsColSelecting = false;

                        GetSamParams(out ll, out ii);
                        Samples.SelectionStart = ii;
                        Samples.SelectionEnd = ii;
                        Samples.IsSelecting = true;

                        Samples.HideCaret();
                        Samples.Redraw();
                        Samples.ShowCaret();
                        break;
                    // Shift + Down
                    case Keys.Down:
                        Samples.IsColSelecting = false;

                        if (!Samples.IsSelecting)
                        {
                            ResetSampleVolumeBuf();
                            GetSamParams(out ll, out ii);
                            Samples.SelectionStart = ii;
                            Samples.SelectionEnd = ii;
                            Samples.IsSelecting = true;
                        }

                        if (Samples.CursorY < Samples.LineCount - 1)
                        {
                            Samples.CursorY++;
                            Samples.SetCaretPosition();
                        }
                        else if (Samples.ShownFrom < VTModule.MaxSampleLength - Samples.LineCount)
                            Samples.ShownFrom++;

                        if (Samples.IsSelecting)
                        {
                            GetSamParams(out ll, out ii);
                            Samples.SelectionEnd = ii;
                        }

                        if (Samples.SelectionEnd < Samples.SelectionStart)
                        {
                            ii = Samples.SelectionEnd;
                            Samples.SelectionEnd = Samples.SelectionStart;
                            Samples.SelectionStart = ii;
                        }

                        Samples.HideCaret();
                        Samples.Redraw();
                        Samples.ShowCaret();
                        break;
                    // Shift + Up
                    case Keys.Up:
                        Samples.IsColSelecting = false;
                        if (!Samples.IsSelecting)
                        {
                            ResetSampleVolumeBuf();
                            GetSamParams(out ll, out ii);
                            Samples.SelectionStart = ii;
                            Samples.SelectionEnd = ii;
                            Samples.IsSelecting = true;
                        }
                        // Samples.selEnd = Samples.selStart then
                        // Inc(Samples.selEnd);
                        if (Samples.CursorY > 0)
                        {
                            Samples.CursorY--;
                            Samples.SetCaretPosition();
                        }
                        else if (Samples.ShownFrom > 0)
                            Samples.ShownFrom--;

                        if (Samples.IsSelecting)
                        {
                            GetSamParams(out ll, out ii);
                            Samples.SelectionStart = ii;
                        }

                        if (Samples.SelectionEnd < Samples.SelectionStart)
                        {
                            ii = Samples.SelectionEnd;
                            Samples.SelectionEnd = Samples.SelectionStart;
                            Samples.SelectionStart = ii;
                        }

                        Samples.HideCaret();
                        Samples.Redraw();
                        Samples.ShowCaret();
                        break;
                    default:
                        if (MainForm.NoteKeys[e.KeyValue] <= -2)
                            return;

                        Samples.IsLineTesting = true;
                        SampleTestLine.CursorX = 8;
                        SampleTestLine.TestLine_KeyDown(sender, e);
                        break;
                }
                // 
                // // Commented block, because users no need templates for samples
                // else if Shift = [ssAlt] then
                // case Key of
                // Keys.Right:
                // AddCurrentToSampTemplate;
                // Keys.Left:
                // CopySampTemplateToCurrent
                // end
                // 
            }
            else if (isShiftDown && isCtrlDown)
            {
                // ValidateSample2(Key);
                GetSamParams(out ll, out ii);

                // tone
                if (Samples.CursorX >= 4 && Samples.CursorX <= 8)
                {
                    // curToneTab:= TMDIChild(ActiveMDIChild).VTM.Ton_Table;
                    currentToneTable = VTM.NoteTable;

                    if (e.KeyValue >= 256)
                        return;

                    if (MainForm.NoteKeys[e.KeyValue] >= 0)
                    {
                        // noteFreq1:= GetNoteFreq(curToneTab,VTM.ReservedPattern.Items[SamNum].Channel[0].Note);
                        noteFreq1 = VTModule.GetNoteFreq(currentToneTable, VTM.ReservedPattern.Lines[1].Channel[0].Note);
                        noteFreq2 = VTModule.GetNoteFreq(currentToneTable, MainForm.NoteKeys[e.KeyValue] + (12 * (SampleTestLine.TestOct - 1)));
                        Samples.ShownSample.Ticks[ii].AddToTone = (short)(noteFreq2 - noteFreq1);
                        Samples.HideCaret();
                        Samples.Redraw();
                        Samples.ShowCaret();
                    }
                }
                // envelope
                else if (Samples.CursorX >= 10 && Samples.CursorX <= 18)
                {
                    // curToneTab:= TMDIChild(ActiveMDIChild).VTM.Ton_Table;
                    currentToneTable = VTM.NoteTable;
                    if (e.KeyValue >= 256)
                        return;

                    if (MainForm.NoteKeys[e.KeyValue] >= 0)
                    {
                        // noteFreq1:= GetNoteFreq(curToneTab,VTM.ReservedPattern.Items[SamNum].Channel[0].Note);
                        noteFreq1 = VTModule.GetNoteFreq(currentToneTable, VTM.ReservedPattern.Lines[1].Channel[0].Note);
                        noteFreq2 = VTModule.GetNoteFreq(currentToneTable, MainForm.NoteKeys[e.KeyValue] + (12 * (SampleTestLine.TestOct - 1)));
                        // envshift:ShortInt;
                        // envFreq: Integer;
                        envFreq = (noteFreq2 - noteFreq1);
                        if (envFreq >= 0)
                        {
                            envFreq = (noteFreq2 - noteFreq1 + 8) / 16;
                            if (envFreq > 15)
                                envFreq = 0;
                        }
                        else
                        {
                            envFreq = (noteFreq2 - noteFreq1 - 8) / 16;
                            if (envFreq < -15)
                                envFreq = 0;
                        }

                        // envshift =
                        Samples.ShownSample.Ticks[ii].Add_to_Envelope_or_Noise = (sbyte)envFreq;
                        Samples.HideCaret();
                        Samples.Redraw();
                        Samples.ShowCaret();
                    }
                }
            }
        }

        public void Ornaments_KeyUp(object sender, KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;

            if (e.KeyCode != Ornaments.KeyPressed)
                return;

            if (isShiftDown && Ornaments.IsLineTesting)
                OrnamentTestLine.TestLine_KeyUp(sender, e);

            if (Samples.IsLineTesting)
            {
                if (AY.PlayMode == PlayModes.PlayLine && WaveOutAPI.IsPlaying)
                    WaveOutAPI.ResetPlaying();

                PlayStopState = PlayStopState.Play;
                SampleTestLine.TestLine_Leave(sender, e);
                Samples.IsLineTesting = false;

                VTM.ReservedPattern.Lines[1].Channel[0] = Ornaments.SavedSampleTestLine;
            }

            if (!isShiftDown && Ornaments.IsLineTesting)
            {
                if (AY.PlayMode == PlayModes.PlayLine && WaveOutAPI.IsPlaying)
                    WaveOutAPI.ResetPlaying();

                PlayStopState = PlayStopState.Play;
                OrnamentTestLine.TestLine_Leave(sender, e);
                Ornaments.IsLineTesting = false;
            }

            Ornaments.KeyPressed = 0;
        }

        public void CopyOrnamentToBuffer(bool all)
        {
            int ff, ornLength, i;

            MainForm.OrnamentCopySrcWindow = this;

            if (!Ornaments.IsSelecting)
            {
                Ornaments.SelectionStart = Ornaments.CurrentLine();
                Ornaments.SelectionEnd = Ornaments.CurrentLine();
                Ornaments.IsSelecting = true;
            }

            if (Ornaments.SelectionStart == 0 && Ornaments.SelectionEnd == VTModule.MaxOrnamentLength)
                all = true;

            MainForm.LastClipboard = LastClipboard.Ornaments;
            ValidateOrnament(OrnamentIndex);

            // Copy entire ornament
            if (all)
            {
                Main.BuffOrnament.Offsets = Ornaments.ShownOrnament.Offsets;
                Main.BuffOrnament.Loop = Ornaments.ShownOrnament.Loop;
                Main.BuffOrnament.Length = Ornaments.ShownOrnament.Length;
            }
            else
            {
                // Copy selected part of ornament
                ornLength = Ornaments.SelectionEnd - Ornaments.SelectionStart;
                for (ff = 0; ff <= ornLength; ff++)
                    Main.BuffOrnament.Offsets[ff] = Ornaments.ShownOrnament.Offsets[ff + Ornaments.SelectionStart];
                Main.BuffOrnament.Loop = 0;
                Main.BuffOrnament.Length = ornLength + 1;
            }

            Main.BuffOrnament.CopyAll = all;

            // Save ornament to copy/paste buffer file
            MainForm.SyncBufferBlocked = true;

            using (FileStream fileStream = new FileStream(MainForm.SyncOrnamentBufferFile, FileMode.Create))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    Ornament ornament = Main.BuffOrnament;
                    for (i = 0; i < ornament.Length; i++)
                    {
                        if (i == ornament.Loop)
                            binaryWriter.Write('L');

                        binaryWriter.Write((ornament.Offsets[i]).ToString());

                        if (i < ornament.Length - 1)
                            binaryWriter.Write(',');
                    }

                    if (all)
                        binaryWriter.Write("\r\nAll\r\n");
                }
            }

            MainForm.SyncOrnamentBufferFileAge = Globals.FileAge(MainForm.SyncOrnamentBufferFile);
            MainForm.SyncBufferBlocked = false;

            OrnamentSelectionOff();
        }

        public void PastePatternToOrnament()
        {
            sbyte i, note, lastNote, tmpNote, baseNote, chan;
            short trackSpeed, j;
            int tracksLine, ornamentLine, srcOrnamentLine;
            bool finish;
            ChannelLine channelLine;
            Pattern pattern;
            Ornament srcOrnament = null;
            byte srcOrnamentNum;
            ChildForm srcWindow;
            VTM srcVTM;

            if (MainForm.LastClipboard != LastClipboard.Tracks)
                return;

            srcWindow = MainForm.TracksCopy.SrcWindow;
            srcVTM = srcWindow.VTM;

            ValidateOrnament(OrnamentIndex);
            SaveOrnamentUndo();

            chan = (sbyte)MainForm.TracksCopy.Channel;
            lastNote = -3;
            baseNote = VTM.ReservedPattern.Lines[0].Channel[0].Note;
            ornamentLine = Ornaments.CurrentLine();
            srcOrnamentNum = 0;
            srcOrnamentLine = 0;
            trackSpeed = 0;
            pattern = MainForm.TracksCopy.Pattern;
            finish = false;

            for (tracksLine = MainForm.TracksCopy.FromLine; tracksLine <= MainForm.TracksCopy.ToLine; tracksLine++)
            {
                channelLine = pattern.Lines[tracksLine].Channel[chan];
                note = channelLine.Note;

                if (lastNote == -3 || note >= 0)
                    lastNote = note;

                if (lastNote == -3 && note < 0)
                    lastNote = baseNote;

                // ---- ORNAMENT ---
                // Remember last ornament
                if (MainForm.TracksCopy.Ornament && (channelLine.Ornament > 0))
                {
                    srcOrnamentNum = channelLine.Ornament;
                    srcOrnament = srcVTM.Ornaments[srcOrnamentNum];
                    srcOrnamentLine = 0;
                }

                // Search last ornament
                if (MainForm.TracksCopy.Ornament && srcOrnamentNum == 0 && tracksLine > 0)
                {
                    for (j = (short)(tracksLine - 1); j >= 0; j--)
                    {
                        if (pattern.Lines[j].Channel[chan].Ornament > 0)
                        {
                            srcOrnamentNum = pattern.Lines[j].Channel[chan].Ornament;
                            srcOrnament = srcVTM.Ornaments[srcOrnamentNum];
                            srcOrnamentLine = 0;
                        }
                    }
                }

                // ---- COMMAND: Ornament offset ---
                if (MainForm.TracksCopy.Ornament && (channelLine.AdditionalCommand.Number == 4))
                {
                    srcOrnamentLine = channelLine.AdditionalCommand.Parameter;
                    if (srcOrnamentLine >= srcOrnament.Length)
                        srcOrnamentLine = srcOrnament.Length - 1;
                }

                // ---- SPEED ---
                // Remember speed
                if (channelLine.AdditionalCommand.Number == 0xb)
                    trackSpeed = channelLine.AdditionalCommand.Parameter;

                // Search last speed command
                if (trackSpeed == 0 && tracksLine > 0)
                {
                    for (j = (short)(tracksLine - 1); j >= 0; j--)
                    {
                        if (pattern.Lines[j].Channel[chan].AdditionalCommand.Parameter == 0xb)
                            trackSpeed = pattern.Lines[j].Channel[chan].AdditionalCommand.Parameter;
                    }
                }

                // Track speed command not found
                if (trackSpeed == 0)
                    trackSpeed = (short)srcWindow.SpeedBpmUpDown.Value;

                // Paste note 'TrackSpeed' times
                for (i = 0; i < trackSpeed; i++)
                {
                    tmpNote = lastNote;

                    // Apply ornament
                    if (MainForm.TracksCopy.Ornament && (srcOrnamentNum != 0))
                        tmpNote += srcOrnament.Offsets[srcOrnamentLine];

                    // Set ornament line note
                    Ornaments.SetNote(tmpNote, ornamentLine, false, false);

                    if (MainForm.TracksCopy.Ornament && (srcOrnamentNum != 0))
                    {
                        srcOrnamentLine++;
                        if (srcOrnamentLine == srcOrnament.Length)
                            srcOrnamentLine = 0;
                    }
                    ornamentLine++;
                    if (ornamentLine == VTModule.MaxOrnamentLength)
                    {
                        MessageBox.Show(this, "Maximum Ornament Length Reached", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        finish = true;
                        break;
                    }
                }

                if (finish)
                    break;
            }

            Ornaments.ShownOrnament.Length = ornamentLine;
            OrnamentLenUpDown.Value = ornamentLine;
            Ornaments.HideCaret();
            Ornaments.Redraw();
            Ornaments.ShowCaret();

            SaveOrnamentRedo();
        }

        public void PasteOrnamentFromBuffer()
        {
            int i, ornCurLine, ornLine;

            if (MainForm.LastClipboard == LastClipboard.Tracks)
            {
                PastePatternToOrnament();
                return;
            }

            ValidateOrnament(OrnamentIndex);
            SaveOrnamentUndo();

            ornCurLine = Ornaments.CurrentLine();

            for (i = 0; i < Main.BuffOrnament.Length; i++)
            {
                ornLine = ornCurLine + i;
                if (ornLine > 254)
                    break;
                Ornaments.ShownOrnament.Offsets[ornLine] = Main.BuffOrnament.Offsets[i];
                if (ornLine >= Ornaments.ShownOrnament.Length)
                    Ornaments.ShownOrnament.Length = ornLine + 1;
            }

            if (Main.BuffOrnament.CopyAll)
            {
                Ornaments.ShownOrnament.Loop = Main.BuffOrnament.Loop + ornCurLine;
                OrnamentLoopUpDown.Value = Main.BuffOrnament.Loop + ornCurLine;
            }
            else
            {
                // OrnamentLoopUpDown.Value := Ornaments.ShownOrnament.Loop;
                OrnamentLenUpDown.Value = Ornaments.ShownOrnament.Length;
            }

            OrnamentSelectionOff();
            SaveOrnamentRedo();

            SongChanged = true;
            BackupSongChanged = true;
        }

        public void OrnamentSelectionOff()
        {
            Ornaments.IsSelecting = false;
            Ornaments.HideCaret();
            Ornaments.Redraw();
            Ornaments.ShowCaret();
        }

        public void GetOrnParams(out int l, out int i, out int c)
        {
            l = Ornaments.ShownOrnament == null ? 1 : Ornaments.ShownOrnament.Length;
            c = Ornaments.CursorY + (Ornaments.CursorX / MainForm.OrnCharCount) * Ornaments.NRaw;
            i = Ornaments.ShownFrom + c;
        }

        public void IncreaseOrnamentValue(int line, KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool isCtrlDown = (e.Modifiers & Keys.Control) == Keys.Control;

            if (Ornaments.ShownOrnament == null)
                return;

            if (e.Modifiers == Keys.None)
                Ornaments.ShownOrnament.Offsets[line]++;

            if (isShiftDown)
                Ornaments.ShownOrnament.Offsets[line] += 3;

            if (isCtrlDown)
                Ornaments.ShownOrnament.Offsets[line] += 12;

            if (isCtrlDown && isShiftDown)
                Ornaments.ShownOrnament.Offsets[line] += 5;

            if (Ornaments.ShownOrnament.Offsets[Ornaments.CurrentLine()] > 96)
                Ornaments.ShownOrnament.Offsets[Ornaments.CurrentLine()] = 96;

            SongChanged = true;
            BackupSongChanged = true;
        }

        public void DecreaseOrnamentValue(int line, KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool isCtrlDown = (e.Modifiers & Keys.Control) == Keys.Control;

            if (Ornaments.ShownOrnament == null)
                return;

            if (e.Modifiers == Keys.None)
                Ornaments.ShownOrnament.Offsets[line]--;

            if (isShiftDown)
                Ornaments.ShownOrnament.Offsets[line] -= 3;

            if (isCtrlDown)
                Ornaments.ShownOrnament.Offsets[line] -= 12;

            if (isCtrlDown && isShiftDown)
                Ornaments.ShownOrnament.Offsets[line] -= 5;

            if (Ornaments.ShownOrnament.Offsets[Ornaments.CurrentLine()] < -96)
                Ornaments.ShownOrnament.Offsets[Ornaments.CurrentLine()] = -96;

            SongChanged = true;
            BackupSongChanged = true;
        }

        public void OrnamentsKeyDown_DoToggles(TOrnToggles n, object sender, KeyEventArgs e)
        {
            int c, i, l, o;

            GetOrnParams(out l, out i, out c);

            if (i >= l)
                return;

            SongChanged = true;
            BackupSongChanged = true;

            ValidateOrnament(OrnamentIndex);

            o = Ornaments.ShownOrnament.Offsets[i];

            switch (n)
            {
                case TOrnToggles.Sgn:
                    Ornaments.ShownOrnament.Offsets[i] = (sbyte)-Ornaments.ShownOrnament.Offsets[i];
                    break;
                case TOrnToggles.SgnP:
                    Ornaments.ShownOrnament.Offsets[i] = Math.Abs(Ornaments.ShownOrnament.Offsets[i]);
                    break;
                case TOrnToggles.SgnM:
                    Ornaments.ShownOrnament.Offsets[i] = (sbyte)-Math.Abs(Ornaments.ShownOrnament.Offsets[i]);
                    break;
            }

            AddUndo(TChangeAction.ChangeOrnamentValue, o, Ornaments.ShownOrnament.Offsets[i]);
            ChangeList[ChangeCount - 1].OldParams.Params.OrnamentCursor = c;
            ChangeList[ChangeCount - 1].OldParams.Params.OrnamentShownFrom = Ornaments.ShownFrom;

            Ornaments.HideCaret();
            Ornaments.Redraw();
            Ornaments.ShowCaret();
        }

        public void OrnamentsKeyDown_DoToggleSpace(object sender, KeyEventArgs e)
        {
            OrnamentsKeyDown_DoToggles(TOrnToggles.Sgn, sender, e);
        }

        public void OrnamentsKeyDown_DoTogglePlus(object sender, KeyEventArgs e)
        {
            OrnamentsKeyDown_DoToggles(TOrnToggles.SgnP, sender, e);
        }

        public void OrnamentsKeyDown_DoToggleMinus(object sender, KeyEventArgs e)
        {
            OrnamentsKeyDown_DoToggles(TOrnToggles.SgnM, sender, e);
        }

        public void OrnamentsKeyDown_DoNumber(object sender, KeyEventArgs e)
        {
            int c, i, l, o;

            GetOrnParams(out l, out i, out c);
            // you can edit everywhere
            // if i >= l then
            // exit;
            SongChanged = true;
            BackupSongChanged = true;

            ValidateOrnament(OrnamentIndex);

            Ornament ornament = Ornaments.ShownOrnament;

            o = ornament.Offsets[i];
            ornament.Offsets[i] = ornament.Offsets[i] < 0 ? (sbyte)-Ornaments.InputONumber : (sbyte)Ornaments.InputONumber;

            AddUndo(TChangeAction.ChangeOrnamentValue, o, ornament.Offsets[i]);

            ChangeList[ChangeCount - 1].OldParams.Params.OrnamentCursor = c;
            ChangeList[ChangeCount - 1].OldParams.Params.OrnamentShownFrom = Ornaments.ShownFrom;

            Ornaments.HideCaret();
            Ornaments.Redraw();
            Ornaments.ShowCaret();
        }

        public void OrnamentsKeyDown_DoDigit(int n, object sender, KeyEventArgs e)
        {
            int nm = Ornaments.InputONumber * 10 + n;

            if (nm > 96)
                nm = n;

            Ornaments.InputONumber = nm;
            OrnamentsKeyDown_DoNumber(sender, e);
        }

        public void Ornaments_KeyDown(object sender, KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool isCtrlDown = (e.Modifiers & Keys.Control) == Keys.Control;
            bool isAltDown = (e.Modifiers & Keys.Alt) == Keys.Alt;
            bool isNoneDown = (e.Modifiers == Keys.None);
            int ff;
            int ii, ll, cc;
            bool incr;
            bool decr;

            incr = e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus;
            decr = e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus;

            // Increase/Decrease
            if (incr || decr)
            {
                // Increase/Decrease selected values
                if (Ornaments.IsSelecting)
                {
                    for (ii = Ornaments.SelectionStart; ii <= Ornaments.SelectionEnd; ii++)
                    {
                        if (incr)
                            IncreaseOrnamentValue(ii, e);
                        else
                            DecreaseOrnamentValue(ii, e);
                    }
                }
                // Increase/Decrease current line only
                else
                {
                    if (incr)
                        IncreaseOrnamentValue(Ornaments.CurrentLine(), e);

                    if (decr)
                        DecreaseOrnamentValue(Ornaments.CurrentLine(), e);
                }

                Ornaments.HideCaret();
                Ornaments.Redraw();
                Ornaments.ShowCaret();

                return;
            }

            if (!isNoneDown || (e.KeyCode < Keys.D0 && e.KeyCode > Keys.D9))
                Ornaments.InputONumber = 0;

            // Alt+1..8 - change octave
            if (isAltDown && e.KeyValue >= 49 && e.KeyValue <= 56)
            {
                OrnamentOctaveNum.Value = e.KeyValue - 48;
                OrnamentTestLine.TestOct = (int)OrnamentOctaveNum.Value;
                return;
            }

            // Numpad 1-8 - change octave
            if (e.KeyCode >= Keys.NumPad1 && e.KeyCode <= Keys.NumPad8)
            {
                OrnamentOctaveNum.Value = e.KeyCode - Keys.NumPad0;
                return;
            }

            // Set note in tone shift position
            if (Ornaments.ToneShiftAsNote && MainForm.NoteKeys[e.KeyValue] != -3 && (isShiftDown))
            {
                if (e.KeyValue >= 256)
                    return;

                if (Ornaments.KeyPressed != e.KeyCode)
                {
                    Ornaments.SetNote(MainForm.NoteKeys[e.KeyValue], Ornaments.CurrentLine(), true, true);

                    // Hack: Temporary replacement of sample testline
                    Ornaments.SavedSampleTestLine = VTM.ReservedPattern.Lines[1].Channel[0];
                    VTM.ReservedPattern.Lines[1].Channel[0] = VTM.ReservedPattern.Lines[0].Channel[0];
                    VTM.ReservedPattern.Lines[1].Channel[0].Ornament = 0;
                    VTM.ReservedPattern.Lines[1].Channel[0].Note = (sbyte)(VTM.ReservedPattern.Lines[0].Channel[0].Note + Ornaments.ShownOrnament.Offsets[Ornaments.CurrentLine()]);

                    Samples.IsLineTesting = true;
                    SampleTestLine.PlayCurrentNote();
                    Ornaments.KeyPressed = e.KeyCode;
                }
                return;
            }

            if (isNoneDown)
            {
                switch (e.KeyCode)
                {
                    case Keys.Next:
                        Ornaments.CursorY = Ornaments.NRaw - 1;
                        Ornaments.SetCaretPosition();
                        break;
                    case Keys.Prior:
                        Ornaments.CursorY = 0;
                        Ornaments.SetCaretPosition();
                        break;
                    case Keys.Home:
                        if (Ornaments.CursorX != 0)
                        {
                            Ornaments.CursorX = 0;
                            Ornaments.SetCaretPosition();
                        }
                        break;
                    case Keys.End:
                        if (Ornaments.CursorX != (MainForm.OrnColumnCount - 1) * MainForm.OrnCharCount)
                        {
                            Ornaments.CursorX = (MainForm.OrnColumnCount - 1) * MainForm.OrnCharCount;
                            Ornaments.SetCaretPosition();
                        }
                        break;
                    case Keys.Down:
                        OrnamentSelectionOff();
                        if (Ornaments.CursorY < Ornaments.NRaw - 1)
                        {
                            Ornaments.CursorY++;
                            Ornaments.SetCaretPosition();
                        }
                        else if (Ornaments.CursorX < (MainForm.OrnColumnCount - 1) * MainForm.OrnCharCount)
                        {
                            Ornaments.CursorY = 0;
                            Ornaments.CursorX += MainForm.OrnCharCount;
                            Ornaments.SetCaretPosition();
                        }
                        else if (Ornaments.ShownFrom < VTModule.MaxOrnamentLength - Ornaments.LineCount)
                        {
                            Ornaments.ShownFrom++;
                            Ornaments.HideCaret();
                            Ornaments.Redraw();
                            Ornaments.ShowCaret();
                        }
                        break;
                    case Keys.Up:
                        OrnamentSelectionOff();
                        if (Ornaments.CursorY > 0)
                        {
                            Ornaments.CursorY--;
                            Ornaments.SetCaretPosition();
                        }
                        else if (Ornaments.CursorX > 0)
                        {
                            Ornaments.CursorY = Ornaments.NRaw - 1;
                            Ornaments.CursorX -= MainForm.OrnCharCount;
                            Ornaments.SetCaretPosition();
                        }
                        else if (Ornaments.ShownFrom > 0)
                        {
                            Ornaments.ShownFrom--;
                            Ornaments.HideCaret();
                            Ornaments.Redraw();
                            Ornaments.ShowCaret();
                        }
                        break;
                    case Keys.Left:
                        if (Ornaments.CursorX > 0)
                        {
                            OrnamentSelectionOff();
                            Ornaments.CursorX -= MainForm.OrnCharCount;
                            Ornaments.SetCaretPosition();
                        }
                        else if (Ornaments.ShownFrom > 0)
                        {
                            OrnamentSelectionOff();
                            Ornaments.ShownFrom -= Ornaments.NRaw;
                            if (Ornaments.ShownFrom < 0)
                                Ornaments.ShownFrom = 0;
                            Ornaments.HideCaret();
                            Ornaments.Redraw();
                            Ornaments.ShowCaret();
                        }
                        break;
                    case Keys.Right:
                        if (Ornaments.CursorX < (MainForm.OrnColumnCount - 1) * MainForm.OrnCharCount)
                        {
                            OrnamentSelectionOff();
                            Ornaments.CursorX += MainForm.OrnCharCount;
                            Ornaments.SetCaretPosition();
                        }
                        else if (Ornaments.ShownFrom < VTModule.MaxOrnamentLength - Ornaments.LineCount)
                        {
                            OrnamentSelectionOff();
                            Ornaments.ShownFrom += Ornaments.NRaw;
                            if (Ornaments.ShownFrom > VTModule.MaxOrnamentLength - Ornaments.LineCount)
                                Ornaments.ShownFrom = VTModule.MaxOrnamentLength - Ornaments.LineCount;
                            Ornaments.HideCaret();
                            Ornaments.Redraw();
                            Ornaments.ShowCaret();
                        }
                        break;
                    case Keys.Space:
                        OrnamentsKeyDown_DoToggleSpace(sender, e);
                        break;
                    case Keys.Oemplus:
                    case Keys.Add:
                        OrnamentsKeyDown_DoTogglePlus(sender, e);
                        break;
                    case Keys.OemMinus:
                    case Keys.Subtract:
                        OrnamentsKeyDown_DoToggleMinus(sender, e);
                        break;
                    // (int)('0') .. (int)('9')
                    case Keys.D0:
                    case Keys.D1:
                    case Keys.D2:
                    case Keys.D3:
                    case Keys.D4:
                    case Keys.D5:
                    case Keys.D6:
                    case Keys.D7:
                    case Keys.D8:
                    case Keys.D9:
                        OrnamentsKeyDown_DoDigit(e.KeyCode - Keys.D0, sender, e);
                        break;
                    case Keys.Oemtilde:
                        if (OrnamentTestLine.CanFocus)
                            OrnamentTestLine.Focus();
                        break;
                    case Keys.Delete:
                        if (isCtrlDown && isShiftDown)
                        {
                        }
                        else if (isShiftDown)
                        {
                        }
                        else if (isCtrlDown)
                        {
                            // next position
                        }
                        else
                        {
                            // If ornament selected, delete selection
                            if (Ornaments.IsSelecting)
                            {
                                Ornaments.ClearSelection();
                                return;
                            }

                            // delete ornament position.
                            ValidateOrnament(OrnamentIndex);
                            GetOrnParams(out ll, out ii, out cc);

                            if (Ornaments.ShownOrnament.Length > 0 && ii < (ll - 1))
                            {
                                Ornaments.ShownOrnament.Length = Ornaments.ShownOrnament.Length - 1;
                                if (Ornaments.ShownOrnament.Loop > ii)
                                    Ornaments.ShownOrnament.Loop = Ornaments.ShownOrnament.Loop - 1;
                            }

                            for (ff = ii; ff < 63; ff++)
                                Ornaments.ShownOrnament.Offsets[ff] = Ornaments.ShownOrnament.Offsets[ff + 1];

                            Ornaments.ShownOrnament.Offsets[254] = 0;
                            OrnamentLenUpDown.Value = Ornaments.ShownOrnament.Length;
                            OrnamentLoopUpDown.Value = Ornaments.ShownOrnament.Loop;

                            Ornaments.HideCaret();
                            Ornaments.Redraw();
                            Ornaments.ShowCaret();
                        }
                        break;
                    case Keys.Insert:
                        if (isCtrlDown && isShiftDown)
                        {
                        }
                        else if (isShiftDown)
                        {
                        }
                        else if (isCtrlDown)
                        {
                            // next position
                        }
                        else
                        {
                            ValidateOrnament(OrnamentIndex);
                            GetOrnParams(out ll, out ii, out cc);
                            if (ll < 255 && ii <= ll)
                            {
                                Ornaments.ShownOrnament.Length = Ornaments.ShownOrnament.Length + 1;
                                if (Ornaments.ShownOrnament.Loop >= ii)
                                    Ornaments.ShownOrnament.Loop = Ornaments.ShownOrnament.Loop + 1;
                            }

                            for (ff = 253; ff >= ii; ff--)
                                Ornaments.ShownOrnament.Offsets[ff + 1] = Ornaments.ShownOrnament.Offsets[ff];

                            OrnamentLenUpDown.Value = Ornaments.ShownOrnament.Length;
                            OrnamentLoopUpDown.Value = Ornaments.ShownOrnament.Loop;

                            Ornaments.HideCaret();
                            Ornaments.Redraw();
                            Ornaments.ShowCaret();
                        }
                        break;
                    default:
                        if (e.KeyValue >= 256 || e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey)
                            return;

                        ValidateOrnament(OrnamentIndex);
                        GetOrnParams(out ll, out ii, out cc);

                        if (MainForm.NoteKeys[e.KeyValue] >= 0)
                        {
                            Ornaments.ShownOrnament.Offsets[ii] = MainForm.NoteKeys[e.KeyValue];
                            Ornaments.HideCaret();
                            Ornaments.Redraw();
                            Ornaments.ShowCaret();
                        }
                        break;
                }
            }
            else if (isCtrlDown)
            {
                switch (e.KeyCode)
                {
                    case Keys.Next:
                    case Keys.End:
                        OrnamentSelectionOff();
                        Ornaments.CursorY = Ornaments.NRaw - 1;
                        Ornaments.CursorX = (MainForm.OrnColumnCount - 1) * MainForm.OrnCharCount;
                        Ornaments.SetCaretPosition();
                        Ornaments.ShownFrom = VTModule.MaxOrnamentLength - Ornaments.LineCount;
                        Ornaments.HideCaret();
                        Ornaments.Redraw();
                        Ornaments.ShowCaret();
                        break;
                    case Keys.Prior:
                    case Keys.Home:
                        OrnamentSelectionOff();
                        Ornaments.CursorY = 0;
                        Ornaments.CursorX = 0;
                        Ornaments.SetCaretPosition();
                        Ornaments.ShownFrom = 0;
                        Ornaments.HideCaret();
                        Ornaments.Redraw();
                        Ornaments.ShowCaret();
                        break;
                    case Keys.Add:
                        // next ornament
                        ChangeOrnament(OrnamentIndex + 1, true, true);
                        break;
                    case Keys.Subtract:
                        // previous ornament
                        ChangeOrnament(OrnamentIndex - 1, true, true);
                        break;
                    // Keys.Insert:
                    // begin
                    // ValidateOrnament(OrnNum);
                    // GetOrnParams(ll,ii,cc);
                    // 
                    // if ii > ll-1 then
                    // begin
                    // for ff:= Ornaments.ShownOrnament.loop to ll do
                    // begin
                    // if ii + ff - Ornaments.ShownOrnament.loop <=254 then
                    // Ornaments.ShownOrnament.Items[ii+ff - Ornaments.ShownOrnament.loop ]:= Ornaments.ShownOrnament.Items[ff];
                    // end;
                    // 
                    // end
                    // else
                    // begin
                    // 
                    // end;
                    // Edit13.Text:= IntToStr(Ornaments.ShownOrnament.Length);
                    // Edit12.Text:= IntToStr(Ornaments.ShownOrnament.Loop);
                    // 
                    // Ornaments.HideCaret;
                    // Ornaments.Redraw();
                    // Ornaments.ShowCaret;
                    // end;
                    case Keys.Insert:
                        CopyOrnamentToBuffer(false);
                        break;
                    case Keys.A:
                        // CTRL+A
                        ValidateOrnament(OrnamentIndex);
                        GetOrnParams(out ll, out ii, out cc);

                        if (!Ornaments.IsSelecting)
                        {
                            Ornaments.SelectionEnd = 255;
                            Ornaments.SelectionStart = 0;
                            Ornaments.IsSelecting = true;
                        }
                        else
                            Ornaments.IsSelecting = false;

                        Ornaments.HideCaret();
                        Ornaments.Redraw();
                        Ornaments.ShowCaret();
                        break;
                }
            }
            else if (isShiftDown)
            {
                switch (e.KeyCode)
                {
                    case Keys.Oemplus:
                        OrnamentsKeyDown_DoTogglePlus(sender, e);
                        break;
                    case Keys.Home:
                        ValidateOrnament(OrnamentIndex);
                        GetOrnParams(out ll, out ii, out cc);

                        if (Ornaments.ShownOrnament.Length > 0 && ii < ll)
                            Ornaments.ShownOrnament.Loop = ii;

                        OrnamentLenUpDown.Value = Ornaments.ShownOrnament.Length;
                        OrnamentLoopUpDown.Value = Ornaments.ShownOrnament.Loop;

                        Ornaments.HideCaret();
                        Ornaments.Redraw();
                        Ornaments.ShowCaret();
                        break;
                    case Keys.End:
                        ValidateOrnament(OrnamentIndex);
                        GetOrnParams(out ll, out ii, out cc);

                        Ornaments.ShownOrnament.Length = ii + 1;
                        if (Ornaments.ShownOrnament.Loop > Ornaments.ShownOrnament.Length)
                            Ornaments.ShownOrnament.Loop = ii;

                        OrnamentLenUpDown.Value = Ornaments.ShownOrnament.Length;
                        OrnamentLoopUpDown.Value = Ornaments.ShownOrnament.Loop;

                        Ornaments.HideCaret();
                        Ornaments.Redraw();
                        Ornaments.ShowCaret();
                        break;
                    case Keys.Insert:
                        PasteOrnamentFromBuffer();
                        break;
                    case Keys.Down:
                        if (Ornaments.IsSelecting == false)
                        {
                            GetOrnParams(out ll, out ii, out cc);
                            Ornaments.SelectionStart = ii;
                            Ornaments.SelectionEnd = ii;
                            Ornaments.IsSelecting = true;
                        }

                        if (Ornaments.CursorY < Ornaments.NRaw - 1)
                        {
                            Ornaments.CursorY++;
                            Ornaments.SetCaretPosition();
                        }
                        else if (Ornaments.CursorX < (MainForm.OrnColumnCount - 1) * MainForm.OrnCharCount)
                        {
                            Ornaments.CursorY = 0;
                            Ornaments.CursorX += MainForm.OrnCharCount;
                            Ornaments.SetCaretPosition();
                        }
                        else if (Ornaments.ShownFrom < VTModule.MaxOrnamentLength - Ornaments.LineCount)
                            Ornaments.ShownFrom++;

                        if (Ornaments.IsSelecting == true)
                        {
                            GetOrnParams(out ll, out ii, out cc);
                            Ornaments.SelectionEnd = ii;
                        }
                        if (Ornaments.SelectionEnd < Ornaments.SelectionStart)
                        {
                            ii = Ornaments.SelectionEnd;
                            Ornaments.SelectionEnd = Ornaments.SelectionStart;
                            Ornaments.SelectionStart = ii;
                        }

                        Ornaments.HideCaret();
                        Ornaments.Redraw();
                        Ornaments.ShowCaret();
                        break;
                    case Keys.Up:
                        if (Ornaments.IsSelecting == false)
                        {
                            GetOrnParams(out ll, out ii, out cc);
                            Ornaments.SelectionStart = ii;
                            Ornaments.SelectionEnd = ii;
                            Ornaments.IsSelecting = true;
                        }

                        if (Ornaments.CursorY > 0)
                        {
                            Ornaments.CursorY--;
                            Ornaments.SetCaretPosition();
                        }
                        else if (Ornaments.CursorX > 0)
                        {
                            Ornaments.CursorY = Ornaments.NRaw - 1;
                            Ornaments.CursorX -= MainForm.OrnCharCount;
                            Ornaments.SetCaretPosition();
                        }
                        else if (Ornaments.ShownFrom > 0)
                            Ornaments.ShownFrom--;

                        if (Ornaments.IsSelecting == true)
                        {
                            GetOrnParams(out ll, out ii, out cc);
                            Ornaments.SelectionStart = ii;
                        }

                        if (Ornaments.SelectionEnd < Ornaments.SelectionStart)
                        {
                            ii = Ornaments.SelectionEnd;
                            Ornaments.SelectionEnd = Ornaments.SelectionStart;
                            Ornaments.SelectionStart = ii;
                        }

                        Ornaments.HideCaret();
                        Ornaments.Redraw();
                        Ornaments.ShowCaret();
                        break;
                    default:
                        if (e.KeyValue == 16 || e.KeyValue == 17)
                            return;

                        Ornaments.IsLineTesting = true;
                        OrnamentTestLine.CursorX = 8;

                        if (e.KeyCode != Keys.Left && e.KeyCode != Keys.Right)
                            OrnamentTestLine.TestLine_KeyDown(sender, e);
                        break;
                }
            }
            else if (isShiftDown && isCtrlDown)
            {
                if (e.KeyValue >= 256 || e.KeyValue == 16 || e.KeyValue == 17)
                    return;

                ValidateOrnament(OrnamentIndex);
                GetOrnParams(out ll, out ii, out cc);

                if (MainForm.NoteKeys[e.KeyValue] + 12 >= 0 && MainForm.NoteKeys[e.KeyValue] + 12 <= 96)
                {
                    Ornaments.ShownOrnament.Offsets[ii] = (sbyte)(MainForm.NoteKeys[e.KeyValue] + 12);
                    Ornaments.HideCaret();
                    Ornaments.Redraw();
                    Ornaments.ShowCaret();
                }
            }
        }

        public void TracksMoveCursorMouse(int x, int y, bool sel, bool mv, bool butRight)
        {
            int x1, y1, i, pLen;
            int sx1, sx2, sy1, sy2;

            if (mv && !Tracks.Clicked)
                return;

            sx2 = Tracks.CursorX;
            sx1 = Tracks.SelectionX;

            if (sx1 > sx2)
            {
                sx1 = sx2;
                sx2 = Tracks.SelectionX;
            }

            sy1 = Tracks.SelectionY;
            sy2 = Tracks.CurrentPatternLine();

            if (sy1 > sy2)
            {
                sy1 = sy2;
                sy2 = Tracks.SelectionY;
            }

            x1 = x / Tracks.CharWidth - MainForm.TracksCursorXLeft;
            y1 = y / Tracks.CharHeight;

            // X out of border
            if (x1 > Tracks.PatternCharCount - 1 - MainForm.TracksCursorXLeft)
                return;

            if (y < 0)
                y1--;

            i = Tracks.CenterLineIndex - Tracks.ShownFrom;
            pLen = Tracks.ShownPattern == null ? VTModule.DefaultPatternLength : Tracks.ShownPattern.Length;

            if (mv)
            {
                if (y1 < i)
                    y1 = i;
                else if (y1 >= i + pLen)
                    y1 = i + pLen - 1;

                if (x1 < 0)
                    x1 = 0;
                else if (x1 > 48)
                    x1 = 48;
            }
            else
                Tracks.Clicked = (y1 >= i) && (y1 < i + pLen) && (x1 >= 0) && !ColSpace(x1);

            if (x1 >= 9 && x1 <= 10)
                x1 = 8;
            else if (x1 >= 23 && x1 <= 24)
                x1 = 22;
            else if (x1 >= 37 && x1 <= 38)
                x1 = 36;

            // Click on a previous pattern
            if (MainForm.MoveBetweenPatrns && y1 < i && x1 >= 0 && !ColSpace(x1) && PositionIndex > 0)
            {
                PositionIndex--;

                Tracks.RedrawDisabled = true;
                IsSynchronizing = true;
                SelectPosition2(PositionIndex);
                IsSynchronizing = false;
                Tracks.RedrawDisabled = false;

                pLen = Tracks.ShownPattern == null ? VTModule.DefaultPatternLength : Tracks.ShownPattern.Length;

                Tracks.ShownFrom = pLen - (i - y1);

                if (Tracks.ShownFrom < 0)
                    Tracks.ShownFrom = 0;

                Tracks.CursorY = Tracks.CenterLineIndex;
                Tracks.CursorX = x1;
                Tracks.RemoveSelection();
            }
            // Click on a next pattern
            else if (MainForm.MoveBetweenPatrns && y1 >= i + pLen && x1 >= 0 && !ColSpace(x1) && PositionIndex + 1 < VTM.Positions.Length)
            {
                PositionIndex++;

                Tracks.RedrawDisabled = true;
                IsSynchronizing = true;
                SelectPosition2(PositionIndex);
                IsSynchronizing = false;
                Tracks.RedrawDisabled = false;

                Tracks.ShownFrom = y1 - (i + pLen);

                pLen = Tracks.ShownPattern == null ? VTModule.DefaultPatternLength : Tracks.ShownPattern.Length;

                if (Tracks.ShownFrom > pLen)
                    Tracks.ShownFrom = pLen - 1;

                Tracks.CursorY = Tracks.CenterLineIndex;
                Tracks.CursorX = x1;
                Tracks.RemoveSelection();
            }
            // Click inside current pattern
            else if (y1 >= i && y1 < i + pLen && x1 >= 0)
            {
                if (butRight && x1 >= sx1 && x1 <= sx2 && y1 >= sy1 + i && y1 <= sy2 + i)
                {
                    if (!mv)
                        Tracks.Clicked = false;

                    return;
                }
                if (ColSpace(x1) && Tracks.IsSelected())
                {
                    if (x1 < sx1)
                        x1++;
                    else
                        x1--;
                }
                if (ColSpace(x1) && !Tracks.IsSelected())
                    x1--;

                if (x1 >= 9 && x1 <= 10)
                    x1 = 8;
                else if (x1 >= 23 && x1 <= 24)
                    x1 = 22;
                else if (x1 >= 37 && x1 <= 38)
                    x1 = 36;

                if (Tracks.CursorX != x1 || Tracks.CursorY != y1)
                {
                    if (Tracks.Focused)
                        Tracks.HideCaret();

                    Tracks.CursorX = x1;
                    Tracks.CursorY = y1;

                    if (Tracks.CursorY >= Tracks.VisibleLineCount)
                    {
                        Tracks.ShownFrom += Tracks.CursorY - Tracks.VisibleLineCount + 1;
                        Tracks.CursorY = Tracks.VisibleLineCount - 1;
                    }
                    else if (Tracks.CursorY < 0)
                    {
                        Tracks.ShownFrom += Tracks.CursorY;
                        Tracks.CursorY = 0;
                    }
                }

                if (sel)
                    Tracks.ShowSelection();
                else
                    Tracks.RemoveSelection();
            }

            if (Tracks.Focused)
            {
                Tracks.HideCaret();
                Tracks.CreateCaret();
                Tracks.SetCaretPosition();
                Tracks.RedrawTracks();
                Tracks.ShowCaret();
            }

            ShowStat();
        }

        public void Tracks_MouseDown(object sender, MouseEventArgs e)
        {
            bool isShiftDown = (GetAsyncKeyState(Keys.ShiftKey) & 0x8000) != 0;
            bool isCtrlDown = (GetAsyncKeyState(Keys.ControlKey) & 0x8000) != 0;
            bool rightButtonPressed = (e.Button & MouseButtons.Right) != 0;

            if (!Tracks.Focused)
                Tracks.Focus();

            if (PlayStopState != PlayStopState.Stop || !(AY.PlayMode == PlayModes.PlayPattern || AY.PlayMode == PlayModes.PlayModule))
                TracksMoveCursorMouse(e.X, e.Y, isShiftDown, false, rightButtonPressed);

            if (isCtrlDown && !MainForm.DisableCtrlClick)
                OpenSampleOrnament();
        }

        public bool SamplesVolMouse(int x, int y)
        {
            bool result = false;

            x -= 21;

            if (x < 0 || x > 15)
                return result;

            int i = Samples.ShownFrom + y;

            if (Samples.ShownSample.Ticks[i].Amplitude != x)
            {
                SongChanged = true;
                BackupSongChanged = true;
                result = true;
                ValidateSample2(SampleIndex);
                Samples.ShownSample.Ticks[i].Amplitude = (byte)x;
            }

            return result;
        }

        public void DrawOnSample(int curX, int curY, int lineNum, bool everywere)
        {
            bool sampleChanged;
            Sample backupSample = new Sample();

            if (SamplesLastCursorX == curX && SamplesLastCursorY == curY)
                return;

            // Make sample backup
            if (!Samples.UndoSaved)
            {
                backupSample.Ticks = Samples.ShownSample.Ticks;
                backupSample.Length = Samples.ShownSample.Length;
                backupSample.Loop = Samples.ShownSample.Loop;
                backupSample.Enabled = Samples.ShownSample.Enabled;
            }
            sampleChanged = SamplesVolMouse(curX, curY);
            // if (CurY <> SamplesLastCursorY) and (CurX > 20) then
            // begin
            // if Samples.ShownSample.Items[LineNum].Amplitude <> 0 then
            // Samples.ShownSample.Items[LineNum].Mixer_Ton := True
            // else
            // Samples.ShownSample.Items[LineNum].Mixer_Ton := False;
            // SampleChanged := True;
            // end;
            SamplesLastCursorX = (short)curX;
            SamplesLastCursorY = (short)curY;

            // Prevent to change tone sign in the Tone Shift as Note mode.
            if (MainForm.SamToneShiftAsNote && curX == 4)
                return;

            if (!everywere && (curX == 8 || curX == 17 || curX == 20))
            {
                if (sampleChanged)
                {
                    if (!Samples.UndoSaved)
                        SaveSampleUndo(backupSample);

                    Samples.HideCaret();
                    Samples.Redraw();
                    Samples.ShowCaret();
                }
                else
                    return;
            }
            // if (LineNum >= Samples.ShownSample.Length) and ((CurX in [0,1,2,8,17,20]) or (CurX > 20)) then
            // begin
            // Samples.ShownSample.Length := LineNum + 1;
            // Edit9.Text := IntToStr(Samples.ShownSample.Length);
            // Edit10.Text := IntToStr(Samples.ShownSample.Loop);
            // SampleChanged := True;
            // end;

            SampleTick sampleTick = Samples.ShownSample.Ticks[lineNum];

            if (DrawOnlyT)
            {
                sampleTick.Mixer_Ton = TNEValue;
                sampleChanged = true;
            }
            else if (DrawOnlyN)
            {
                sampleTick.Mixer_Noise = TNEValue;
                sampleChanged = true;
            }
            else if (DrawOnlyE)
            {
                sampleTick.Envelope_Enabled = TNEValue;
                sampleChanged = true;
            }
            else if (DrawOnlyToneSign)
            {
                if ((PositiveSign && sampleTick.AddToTone < 0) || (!PositiveSign && sampleTick.AddToTone > 0))
                    sampleTick.AddToTone = (short)-sampleTick.AddToTone;
                sampleChanged = true;
            }
            else if (DrawOnlyNoiseSign)
            {
                if ((PositiveSign && sampleTick.Add_to_Envelope_or_Noise < 0) || (!PositiveSign && sampleTick.Add_to_Envelope_or_Noise > 0))
                    sampleTick.Add_to_Envelope_or_Noise = (sbyte)-sampleTick.Add_to_Envelope_or_Noise;
                sampleChanged = true;
            }
            else
            {
                switch (curX)
                {
                    case 0:
                        sampleTick.Mixer_Ton = !sampleTick.Mixer_Ton;
                        sampleChanged = true;
                        break;
                    case 1:
                        sampleTick.Mixer_Noise = !sampleTick.Mixer_Noise;
                        sampleChanged = true;
                        break;
                    case 2:
                        sampleTick.Envelope_Enabled = !sampleTick.Envelope_Enabled;
                        sampleChanged = true;
                        break;
                    case 4:
                        sampleTick.AddToTone = (short)-sampleTick.AddToTone;
                        sampleChanged = true;
                        break;
                    case 8:
                        sampleTick.Ton_Accumulation = !sampleTick.Ton_Accumulation;
                        sampleChanged = true;
                        break;
                    case 10:
                        sampleTick.Add_to_Envelope_or_Noise = Ns(-sampleTick.Add_to_Envelope_or_Noise);
                        sampleChanged = true;
                        break;
                    case 17:
                        sampleTick.Envelope_or_Noise_Accumulation = !sampleTick.Envelope_or_Noise_Accumulation;
                        sampleChanged = true;
                        break;
                    case 20:
                        sampleChanged = true;
                        if (!sampleTick.Amplitude_Sliding)
                        {
                            sampleTick.Amplitude_Sliding = true;
                            sampleTick.Amplitude_Slide_Up = false;
                        }
                        else if (!sampleTick.Amplitude_Slide_Up)
                            sampleTick.Amplitude_Slide_Up = true;
                        else
                            sampleTick.Amplitude_Sliding = false;
                        break;
                }
            }

            if (sampleChanged)
            {
                if (!Samples.UndoSaved)
                    SaveSampleUndo(backupSample);

                Samples.HideCaret();
                Samples.Redraw();
                Samples.ShowCaret();
            }
        }

        public byte DetectSampleColumn(int x)
        {
            byte result = 0;

            if (x == 0)
                result = 1;

            if (x == 1)
                result = 2;

            if (x == 2)
                result = 3;

            if (x >= 4 && x <= 8)
                result = 4;

            if (x >= 10 && x <= 17)
                result = 5;

            if (x >= 19 && x <= 36)
                result = 6;

            return result;
        }

        public void Samples_MouseDown(object sender, MouseEventArgs e)
        {
            int i, x1, y1, lineNum;
            bool isShiftDown = (GetAsyncKeyState(Keys.ShiftKey) & 0x8000) != 0;
            bool isCtrlDown = (GetAsyncKeyState(Keys.ControlKey) & 0x8000) != 0;
            bool isAltDown = (GetAsyncKeyState(Keys.Menu) & 0x8000) != 0;
            bool isLeftDown = (e.Button & MouseButtons.Left) != 0;
            bool isRightDown = (e.Button & MouseButtons.Right) != 0;
            bool isAnyDown = isShiftDown || isCtrlDown || isAltDown || isLeftDown || isRightDown;

            if (isAnyDown)
            {
                //Globals.MainForm.HideHint();
                ToolTip.Hide(this);
                ShowHintTimer.Enabled = false;
                ShowHintTimer.Interval = 9000;
            }

            Samples.InputSNumber = 0;
            ValidateSample2(SampleIndex);
            x1 = e.X / Samples.CharWidth - 3;
            y1 = e.Y / Samples.CharHeight;
            lineNum = Samples.ShownFrom + y1;

            if (y1 >= 0 && y1 < Samples.LineCount && x1 >= 0 && !((x1 == 3 || x1 == 9 || x1 == 18) || (x1 >= 21 && x1 <= 36)))
            {
                if (x1 >= 6 && x1 <= 7)
                    x1 = 5;
                else if (x1 == 12)
                    x1 = 11;
                else if (x1 == 13 || x1 == 16)
                    x1 = 14;
                else if (x1 == 15)
                    x1 = 14;
                Samples.CursorX = x1;
                Samples.CursorY = y1;
                Samples.DoHint(x1, y1);
            }

            // Select lines
            if (isShiftDown && isLeftDown && !Samples.IsSelecting)
            {
                Samples.SelectionStart = lineNum;
                Samples.SelectionEnd = lineNum;
                SamplesClickStartLine = (short)lineNum;
                SamplesLeftMouseButton = true;
                SamplesLastMouseCursorY = (short)e.Y;
            }

            // Select columns
            if (isCtrlDown && isLeftDown && !Samples.IsSelecting)
            {
                MainForm.SampleCopy.FromLine = (byte)lineNum;
                MainForm.SampleCopy.ToLine = (byte)lineNum;
                i = DetectSampleColumn(x1);

                if (i == 0)
                    return;

                MainForm.SampleCopy.FromColumn = (byte)i;
                MainForm.SampleCopy.ToColumn = MainForm.SampleCopy.FromColumn;
                MainForm.SampleCopy.StartColumn = MainForm.SampleCopy.ToColumn;
                SamplesClickStartLine = (short)lineNum;
                SamplesLeftMouseButton = true;
                SamplesLastMouseCursorY = (short)e.Y;
            }

            // Start to set loop position
            if (isRightDown && !SamplesRightMouseButton)
            {
                SamplesClickStartLine = (byte)lineNum;
                SamplesClickEndLine = (short)lineNum;
                SamplesLastMouseCursorY = (short)e.Y;
                SamplesRightMouseButton = true;
            }

            // Left click -> Change sample params
            if (isLeftDown && !Samples.IsSelecting)
            {
                SamplesLastCursorY = (short)y1;
                DrawOnSample(x1, y1, lineNum, true);

                if (x1 == 0)
                {
                    TNEValue = Samples.ShownSample.Ticks[lineNum].Mixer_Ton;
                    DrawOnlyT = true;
                }
                else if (x1 == 1)
                {
                    TNEValue = Samples.ShownSample.Ticks[lineNum].Mixer_Noise;
                    DrawOnlyN = true;
                }
                else if (x1 == 2)
                {
                    TNEValue = Samples.ShownSample.Ticks[lineNum].Envelope_Enabled;
                    DrawOnlyE = true;
                }
                else if (x1 == 4)
                {
                    PositiveSign = Samples.ShownSample.Ticks[lineNum].AddToTone >= 0;
                    DrawOnlyToneSign = true;
                }
                else if (x1 == 10)
                {
                    PositiveSign = Samples.ShownSample.Ticks[lineNum].Add_to_Envelope_or_Noise >= 0;
                    DrawOnlyNoiseSign = true;
                }

                Samples.RecreateCaret();
                Samples.SetCaretPosition();
            }

            // Deselecting
            if (Samples.IsSelecting || Samples.IsColSelecting)
                SamplesSelectionOff();

            // Set focus
            if (!Samples.Focused)
                Samples.Focus();
        }

        public void Samples_MouseUp(object sender, MouseEventArgs e)
        {
            // Release mouse events capture
            ReleaseCapture();

            SamplesRightMouseButton = false;
            SamplesLeftMouseButton = false;
            Samples.SamplesDontScroll = false;
            SamplesLastCursorX = -1;
            SamplesLastCursorY = -1;
            DrawOnlyT = false;
            DrawOnlyN = false;
            DrawOnlyE = false;
            DrawOnlyNoiseSign = false;
            DrawOnlyToneSign = false;

            SaveSampleRedo();
        }

        public void Tracks_MouseMove(object sender, MouseEventArgs e)
        {
            bool isLeftDown = (e.Button & MouseButtons.Left) != 0;
            bool isRightDown = (e.Button & MouseButtons.Right) != 0;

            if ((isLeftDown || isRightDown) && Tracks.Focused)
                TracksMoveCursorMouse(e.X, e.Y, true, true, false);
        }

        public void Samples_MouseMove(object sender, MouseEventArgs e)
        {
            bool isShiftDown = (GetAsyncKeyState(Keys.ShiftKey) & 0x8000) != 0;
            bool isCtrlDown = (GetAsyncKeyState(Keys.ControlKey) & 0x8000) != 0;
            bool isAltDown = (GetAsyncKeyState(Keys.Menu) & 0x8000) != 0;
            bool isLeftDown = (e.Button & MouseButtons.Left) != 0;
            bool isRightDown = (e.Button & MouseButtons.Right) != 0;
            bool isButtonDown = isLeftDown || isRightDown;
            bool isAnyDown = isShiftDown || isCtrlDown || isAltDown || isLeftDown || isRightDown;
            const int MouseShift = 8;
            int i, x1, y1, lineNum;
            bool accept;

            ValidateSample2(SampleIndex);

            x1 = e.X / Samples.CharWidth - 3;
            y1 = e.Y / Samples.CharHeight;
            lineNum = y1 + Samples.ShownFrom;

            if (!isAnyDown)
                Samples.DoHint(x1, y1);
            else
            {
                ToolTip.Hide(this);
                ShowHintTimer.Enabled = false;
                ShowHintTimer.Interval = 9000;
            }

            // Accept means mouse Y coordinate more than Y +- MouseShift value
            accept = (e.Y <= SamplesLastMouseCursorY - MouseShift) || (e.Y >= SamplesLastMouseCursorY + MouseShift);

            // Restart line selecting
            if ((isShiftDown && isLeftDown) && !SamplesLeftMouseButton)
            {
                Samples.SelectionStart = (byte)lineNum;
                Samples.SelectionEnd = (byte)lineNum;
                SamplesClickStartLine = (byte)lineNum;
                SamplesLastMouseCursorY = (byte)e.Y;
                SamplesLeftMouseButton = true;
                accept = false;
            }

            // Restart column selecting
            if ((isCtrlDown && isLeftDown) && !SamplesLeftMouseButton)
            {
                MainForm.SampleCopy.FromLine = (byte)lineNum;
                MainForm.SampleCopy.ToLine = (byte)lineNum;

                i = DetectSampleColumn(x1);

                if (i == 0)
                    return;

                MainForm.SampleCopy.FromColumn = (byte)i;

                MainForm.SampleCopy.ToColumn = MainForm.SampleCopy.FromColumn;
                MainForm.SampleCopy.StartColumn = MainForm.SampleCopy.ToColumn;

                SamplesClickStartLine = (byte)lineNum;
                SamplesLeftMouseButton = true;
                SamplesLastMouseCursorY = (byte)e.Y;
            }

            // Column selecting
            if ((isCtrlDown && isLeftDown) && SamplesLeftMouseButton)
            {
                if (!Samples.IsColSelecting)
                    ResetSampleVolumeBuf();

                Samples.IsSelecting = false;
                Samples.IsColSelecting = true;
                MainForm.SampleCopy.Sample = Samples.ShownSample;

                if (SamplesClickStartLine >= lineNum)
                {
                    MainForm.SampleCopy.ToLine = (byte)SamplesClickStartLine;
                    MainForm.SampleCopy.FromLine = (byte)lineNum;
                }

                if (SamplesClickStartLine < (byte)lineNum)
                {
                    MainForm.SampleCopy.FromLine = (byte)SamplesClickStartLine;
                    MainForm.SampleCopy.ToLine = (byte)lineNum;
                }

                i = DetectSampleColumn(x1);

                if (i == 0)
                    return;

                MainForm.SampleCopy.ToColumn = (byte)i;

                if (MainForm.SampleCopy.StartColumn >= MainForm.SampleCopy.ToColumn)
                {
                    i = MainForm.SampleCopy.ToColumn;
                    MainForm.SampleCopy.ToColumn = MainForm.SampleCopy.StartColumn;
                    MainForm.SampleCopy.FromColumn = (byte)i;
                }

                if (MainForm.SampleCopy.StartColumn < MainForm.SampleCopy.ToColumn)
                {
                    i = MainForm.SampleCopy.ToColumn;
                    MainForm.SampleCopy.FromColumn = MainForm.SampleCopy.StartColumn;
                    MainForm.SampleCopy.ToColumn = (byte)i;
                }

                Samples.ShowCaret();
                Samples.Redraw();
                Samples.HideCaret();
            }

            // Line selecting
            if ((isShiftDown && isLeftDown) && (SamplesClickStartLine == Samples.SelectionStart || SamplesClickStartLine == Samples.SelectionEnd) && accept && SamplesLeftMouseButton)
            {
                if (!Samples.IsSelecting)
                    ResetSampleVolumeBuf();

                Samples.IsSelecting = true;
                Samples.IsColSelecting = false;

                if (SamplesClickStartLine >= lineNum)
                {
                    Samples.SelectionEnd = SamplesClickStartLine;
                    Samples.SelectionStart = lineNum;
                }

                if (SamplesClickStartLine < lineNum)
                {
                    Samples.SelectionStart = SamplesClickStartLine;
                    Samples.SelectionEnd = lineNum;
                }

                Samples.ShowCaret();
                Samples.Redraw();
                Samples.HideCaret();
            }

            // Change sample length & loop
            if (isRightDown && SamplesRightMouseButton && accept)
            {
                if (!Samples.UndoSaved)
                    SaveSampleUndo(Samples.ShownSample);

                if (SamplesClickStartLine >= lineNum)
                {
                    ChangeSampleLength(SamplesClickStartLine + 1, true);
                    ChangeSampleLoop(lineNum, true);
                }

                if (SamplesClickStartLine < lineNum)
                {
                    ChangeSampleLength(lineNum + 1, true);
                    ChangeSampleLoop(SamplesClickStartLine, true);
                }
            }

            // Draw on sample (TNE, Volume, etc)
            if (isLeftDown)
            {
                Samples.InputSNumber = 0;
                DrawOnSample(x1, y1, lineNum, false);
            }

            // Scroll down
            if (isButtonDown && y1 >= Samples.LineCount && Samples.ShownFrom + Samples.LineCount < VTModule.MaxSampleLength)
            {
                Samples.ShownFrom++;
                Samples.Redraw();
            }

            // Scroll up
            if (isButtonDown && y1 == 0 && Samples.ShownFrom > 0)
            {
                Samples.ShownFrom--;
                Samples.Redraw();
            }

            // For catching MouseUp event outside samples control
            if (isButtonDown)
                Samples.Capture = true;
        }

        public void Ornaments_MouseUp(object sender, MouseEventArgs e)
        {
            int x1, x2, y1, i, c, lineNum;

            // Release mouse events capture
            ReleaseCapture();

            x1 = e.X / Ornaments.CharWidth;
            y1 = e.Y / Ornaments.CharHeight;
            x2 = x1 % MainForm.OrnCharCount;

            if (y1 >= 0 && y1 < Ornaments.NRaw && x1 >= 3 + MainForm.OrnXShift && x1 < MainForm.OrnColumnCount * MainForm.OrnCharCount - 1 && !(x2 == 0 || x2 == 7))
            {
                i = x1 / MainForm.OrnCharCount;
                x1 = i * MainForm.OrnCharCount;
                c = i * Ornaments.NRaw + y1;
                lineNum = Ornaments.ShownFrom + c;

                if (Ornaments.RightMouseButton && !Ornaments.LoopStarted && Ornaments.ShownOrnament != null)
                {
                    Ornaments.CursorX = x1;
                    Ornaments.CursorY = y1;
                    Ornaments.SetCaretPosition();

                    // Change sign for ornament item
                    if (lineNum < Ornaments.ShownOrnament.Length && Ornaments.ShownOrnament.Offsets[lineNum] != 0)
                    {
                        SongChanged = true;
                        BackupSongChanged = true;
                        AddUndo(TChangeAction.ChangeOrnamentValue, Ornaments.ShownOrnament.Offsets[lineNum], -Ornaments.ShownOrnament.Offsets[lineNum]);
                        ChangeList[ChangeCount - 1].OldParams.Params.OrnamentCursor = c;
                        ChangeList[ChangeCount - 1].OldParams.Params.OrnamentShownFrom = Ornaments.ShownFrom;
                        Ornaments.ShownOrnament.Offsets[lineNum] = (sbyte)-Ornaments.ShownOrnament.Offsets[lineNum];
                    }

                    if (Ornaments.Focused)
                        Ornaments.HideCaret();

                    Ornaments.Redraw();

                    if (Ornaments.Focused)
                        Ornaments.ShowCaret();
                }
            }

            Ornaments.LeftMouseButton = false;
            Ornaments.RightMouseButton = false;
            Ornaments.LoopStarted = false;
            Ornaments.InputONumber = 0;

            SaveOrnamentRedo();
        }

        public void Ornaments_MouseDown(object sender, MouseEventArgs e)
        {
            bool isShiftDown = (GetAsyncKeyState(Keys.ShiftKey) & 0x8000) != 0;
            bool isCtrlDown = (GetAsyncKeyState(Keys.ControlKey) & 0x8000) != 0;
            bool isAltDown = (GetAsyncKeyState(Keys.Menu) & 0x8000) != 0;
            bool isLeftDown = (e.Button & MouseButtons.Left) != 0;
            bool isRightDown = (e.Button & MouseButtons.Right) != 0;
            bool isButtonDown = isLeftDown || isRightDown;
            bool isNoneDown = !isShiftDown && !isCtrlDown && !isAltDown && !isButtonDown;
            int x1 = e.X / Ornaments.CharWidth;
            int y1 = e.Y / Ornaments.CharHeight;
            int lineNum = Ornaments.ShownFrom + x1 / MainForm.OrnCharCount * Ornaments.NRaw + y1;
            Ornaments.InputONumber = 0;

            // Start to set loop & length
            if (isRightDown && !Ornaments.LoopStarted)
            {
                Ornaments.ClickStartLine = (short)lineNum;
                Ornaments.ClickEndLine = (short)lineNum;
                Ornaments.ClickMouseCursorY = (short)e.Y;
            }

            int x2 = x1 % MainForm.OrnCharCount;

            if (y1 >= 0 && y1 < Ornaments.NRaw && x1 >= 3 + MainForm.OrnXShift && x1 < MainForm.OrnColumnCount * MainForm.OrnCharCount - 1 && !(x2 == 0 || x2 == 7))
            {
                // Set cursor by left mouse click
                if (isLeftDown)
                {
                    Ornaments.CursorX = (x1 / MainForm.OrnCharCount) * MainForm.OrnCharCount;
                    Ornaments.CursorY = y1;
                }

                // Start selecting by Shift
                if ((isShiftDown && isLeftDown) || (isCtrlDown && isLeftDown) && !Ornaments.IsSelecting)
                {
                    Ornaments.SelectionStart = lineNum;
                    Ornaments.SelectionEnd = lineNum;
                    Ornaments.ClickStartLine = (short)lineNum;
                    Ornaments.ClickMouseCursorY = (short)e.Y;
                    Ornaments.LeftMouseButton = true;
                }

                // Set flag for ornament sign change by right mouse button
                if (isRightDown)
                    Ornaments.RightMouseButton = true;
            }

            // Deselecting
            OrnamentSelectionOff();

            if (!Ornaments.Focused)
                Ornaments.Focus();

            // Set cursor position
            Ornaments.HideCaret();
            Ornaments.SetCaretPosition();
            Ornaments.ShowCaret();
            Ornaments.DoHint();
        }

        public void Ornaments_MouseMove(object sender, MouseEventArgs e)
        {
            bool isShiftDown = (GetAsyncKeyState(Keys.ShiftKey) & 0x8000) != 0;
            bool isCtrlDown = (GetAsyncKeyState(Keys.ControlKey) & 0x8000) != 0;
            bool isAltDown = (GetAsyncKeyState(Keys.Menu) & 0x8000) != 0;
            bool isLeftDown = (e.Button & MouseButtons.Left) != 0;
            bool isRightDown = (e.Button & MouseButtons.Right) != 0;
            bool isButtonDown = isLeftDown || isRightDown;
            bool isNoneDown = !isShiftDown && !isCtrlDown && !isAltDown && !isButtonDown;
            const int MouseShift = 8;
            int x1 = e.X / Ornaments.CharWidth;
            int y1 = e.Y / Ornaments.CharHeight;
            int lineNum = Ornaments.ShownFrom + (x1 / MainForm.OrnCharCount) * Ornaments.NRaw + y1;

            // Accept means mouse Y coordinate more than Y +- MouseShift const
            bool accept = e.Y <= Ornaments.ClickMouseCursorY - MouseShift || e.Y >= Ornaments.ClickMouseCursorY + MouseShift;

            // Change ornament length & loop
            if (isRightDown && accept)
            {
                if (!Ornaments.UndoSaved && Ornaments.LoopStarted)
                    SaveOrnamentUndo();

                if (Ornaments.ClickStartLine >= lineNum && Ornaments.LoopStarted)
                {
                    ChangeOrnamentLength(Ornaments.ClickStartLine + 1, true);
                    ChangeOrnamentLoop(lineNum, true);
                }

                if (Ornaments.ClickStartLine < lineNum && Ornaments.LoopStarted)
                {
                    ChangeOrnamentLength(lineNum + 1, true);
                    ChangeOrnamentLoop(Ornaments.ClickStartLine, true);
                }

                Ornaments.LoopStarted = true;
            }

            int x2 = x1 % MainForm.OrnCharCount;

            if (y1 >= 0 && y1 < Ornaments.NRaw && x1 >= 3 + MainForm.OrnXShift && x1 < MainForm.OrnColumnCount * MainForm.OrnCharCount - 1 && !(x2 == 0 || x2 == 7))
            {
                // Restart selecting
                if ((isLeftDown && isShiftDown) || (isLeftDown && isCtrlDown) && !Ornaments.IsSelecting && !Ornaments.LeftMouseButton)
                {
                    Ornaments.SelectionStart = lineNum;
                    Ornaments.SelectionEnd = lineNum;
                    Ornaments.ClickStartLine = (short)lineNum;
                    Ornaments.ClickMouseCursorY = (short)e.Y;
                    Ornaments.LeftMouseButton = true;
                    accept = false;
                }

                // Selecting
                if ((isLeftDown && isShiftDown) && ((Ornaments.ClickStartLine == Ornaments.SelectionStart) || (Ornaments.ClickStartLine == Ornaments.SelectionEnd)) && accept && Ornaments.LeftMouseButton)
                {
                    Ornaments.IsSelecting = true;

                    if (Ornaments.ClickStartLine >= lineNum)
                    {
                        Ornaments.SelectionEnd = Ornaments.ClickStartLine;
                        Ornaments.SelectionStart = lineNum;
                    }

                    if (Ornaments.ClickStartLine < lineNum)
                    {
                        Ornaments.SelectionStart = Ornaments.ClickStartLine;
                        Ornaments.SelectionEnd = lineNum;
                    }

                    Ornaments.HideCaret();
                    Ornaments.Redraw();
                    Ornaments.ShowCaret();
                }
            }

            // For catching MouseUp event outside ornaments control
            if (isButtonDown)
                Ornaments.Capture = true;
        }

        public void Ornaments_MouseWheel(object sender, MouseEventArgs e)
        {
            // Detect mouse wheel up down
            if (e.Delta > 0)
                Ornaments_MouseWheelUp(sender, e);
            else
                Ornaments_MouseWheelDown(sender, e);
        }

        public void Ornaments_MouseWheelUp(object sender, MouseEventArgs e)
        {
            if (Ornaments.ShownFrom == 0)
                return;

            Ornaments.ShownFrom = Ornaments.ShownFrom - 1;
            Ornaments.Redraw();
        }

        public void Ornaments_MouseWheelDown(object sender, MouseEventArgs e)
        {
            if (Ornaments.ShownFrom == VTModule.MaxOrnamentLength - (MainForm.OrnColumnCount * Ornaments.NRaw))
                return;

            Ornaments.ShownFrom = Ornaments.ShownFrom + 1;
            Ornaments.Redraw();
        }

        public void DisposeUndo(bool all)
        {
            int i;

            if (all)
                i = 0;
            else
                i = ChangeCount - 1;

            for (; i < ChangeTop; i++)
            {
                switch (ChangeList[i].Action)
                {
                    case TChangeAction.LoadPattern:
                    case TChangeAction.InsertPatternFromClipboard:
                    case TChangeAction.PatternInsertLine:
                    case TChangeAction.PatternDeleteLine:
                    case TChangeAction.PatternClearLine:
                    case TChangeAction.PatternClearSelection:
                    case TChangeAction.TransposePattern:
                    case TChangeAction.TracksManagerCopy:
                    case TChangeAction.ExpandCompressPattern:
                        //this.Dispose(ChangeList[i].Pattern);
                        break;
                    case TChangeAction.DeletePosition:
                    case TChangeAction.InsertPosition:
                        //this.Dispose(ChangeList[i].PositionList);
                        break;
                    case TChangeAction.LoadOrnament:
                    case TChangeAction.OrGen:
                    case TChangeAction.CopyOrnamentToOrnament:
                        //this.Dispose(ChangeList[i].Ornament);
                        break;
                    case TChangeAction.LoadSample:
                    case TChangeAction.CopySampleToSample:
                        //this.Dispose(ChangeList[i].Sample);
                        break;
                    case TChangeAction.ChangeSampleValue:
                        //this.Dispose(ChangeList[i].SampleLineValues);
                        break;
                    case TChangeAction.ChangePositionsAndPatterns:
                        //this.Dispose(ChangeList[i].PositionList);
                        //this.Dispose(ChangeList[i].ComParams.Patterns);
                        break;
                    case TChangeAction.ChangePatternContent:
                        //this.Dispose(ChangeList[i].ComParams.ChangedPattern);
                        break;
                    case TChangeAction.ChangeEntireSample:
                        //this.Dispose(ChangeList[i].ComParams.EntireSample);
                        break;
                    case TChangeAction.ChangeEntireOrnament:
                        //this.Dispose(ChangeList[i].ComParams.EntireOrnament);
                        break;
                }
            }

            if (all)
                ChangeCount = 0;

            ChangeTop = ChangeCount;
        }

        public void ChildWin_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (WaveOutAPI.IsPlaying && (PlayingWindow[0] == this ||
                (AY.ChipCount >= 2 && PlayingWindow[1] == this) ||
                (AY.ChipCount == 3 && PlayingWindow[2] == this)))
            {
                WaveOutAPI.StopPlaying();
                Globals.MainForm.RestoreControls();
            }

            Globals.MainForm.DeleteWindowListItem(this);
            Globals.MainForm.Text = MainForm.FullVersString;
            Globals.TrackInfoForm.Hide();

            DisposeUndo(true);
            ChangeList = Array.Empty<ChangeListItem>();
            ChangePatternsList = Array.Empty<ChangePattern[][]>();
            ChangeOnePatternList = Array.Empty<ChangeOnePattern>();
            ChangeSamplesList = Array.Empty<ChangeSample>();
            ChangeOrnamentsList = Array.Empty<ChangeOrnament>();
            ChangeNilPatternsList = Array.Empty<int[]>();
            // FreeAndNil(SampleTestLine);
            // FreeAndNil(OrnamentTestLine);
            // FreeAndNil(Samples);
            // FreeAndNil(Ornaments);
            // FreeAndNil(Tracks);

            VTModule.FreeVTM(ref VTM);

            MainForm mainForm = (MainForm)this.MdiParent;
            mainForm.BeginInvoke(new Action(() =>
            {
                mainForm.MdiChild_Closed(this, e);
            }));
        }

        public void SetFileName(string fileName)
        {
            IsDemoSong = fileName.IndexOf(Path.Combine(MainForm.VortexDocumentsDir, MainForm.DemoSongsDefaultDir)) != -1;
            if (!IsDemoSong && !IsTemplate)
                WinFileName = fileName;

            if (fileName.IndexOf("template.vt2") != -1)
                this.Text = "Template Song";
            else
                this.Text = Path.GetFileName(fileName);

            BackupVersionCounter = GetBackupVersionCounter();
        }

        public void Tracks_MouseWheel(object sender, MouseEventArgs e)
        {
            // Detect mouse wheel up down
            if (e.Delta > 0)
                Tracks_MouseWheelUp(sender, e);
            else
                Tracks_MouseWheelDown(sender, e);
        }

        public void Tracks_MouseWheelDown(object sender, MouseEventArgs e)
        {
            int patternLength;

            if (WaveOutAPI.IsPlaying && (AY.PlayMode == PlayModes.PlayModule) || (AY.PlayMode == PlayModes.PlayPattern))
                return;

            if (IsMouseOverControl(PositionsGrid))
            {
                // Scroll positions, not pattern
                PositionsGrid.Focus();
                PositionsGrid_MouseWheelDown(sender, e);
                return;
            }

            // Mouse pointer under another control
            if (!IsMouseOverControl(Tracks))
                return;

            Tracks.ManualBitBlt = true;

            if (TSWindow[0] != null)
                TSWindow[0].Tracks.ManualBitBlt = true;

            if (TSWindow[1] != null)
                TSWindow[1].Tracks.ManualBitBlt = true;

            //e.Handled = true;
            patternLength = Tracks.ShownPattern == null ? VTModule.DefaultPatternLength : Tracks.ShownPattern.Length;

            if (Tracks.ShownFrom < patternLength - 1)
            {
                Tracks.ShownFrom++;
                Tracks.HideCaret();

                if (Tracks.CursorY > 0 && Tracks.CursorY != Tracks.CenterLineIndex)
                {
                    Tracks.CursorY--;
                    Tracks.SetCaretPosition();
                }
                else if ((GetKeyState(Keys.Shift) & 128) == 0)
                    Tracks.RemoveSelection();

                Tracks.RedrawTracks();
                Tracks.ShowCaret();
            }
            else
            {
                if (MoveBetweenPatternsCheckBox.Checked)
                    BetweenPatternsDown();
                else
                {
                    Tracks.ShownFrom = 0;
                    Tracks.CursorY = Tracks.CenterLineIndex;
                    Tracks.RemoveSelection();
                    Tracks.HideCaret();
                    Tracks.RedrawTracks();
                    Tracks.SetCaretPosition();
                    Tracks.ShowCaret();
                }
            }

            ShowStat();

            Tracks.ManualBitBlt = false;
            Tracks.DoBitBlt();

            if (TSWindow[0] != null)
            {
                TSWindow[0].Tracks.ManualBitBlt = false;
                TSWindow[0].Tracks.DoBitBlt();
            }

            if (TSWindow[1] != null)
            {
                TSWindow[1].Tracks.ManualBitBlt = false;
                TSWindow[1].Tracks.DoBitBlt();
            }
        }

        public void Tracks_MouseWheelUp(object sender, MouseEventArgs e)
        {
            int patternLength;

            if (WaveOutAPI.IsPlaying && (AY.PlayMode == PlayModes.PlayModule || AY.PlayMode == PlayModes.PlayPattern))
                return;

            if (IsMouseOverControl(PositionsGrid))
            {
                // Scroll positions, not pattern
                PositionsGrid.Focus();
                PositionsGrid_MouseWheelUp(sender, e);
                return;
            }

            // Mouse pointer under another control
            if (!IsMouseOverControl(Tracks))
                return;

            Tracks.ManualBitBlt = true;

            if (TSWindow[0] != null)
                TSWindow[0].Tracks.ManualBitBlt = true;

            if (TSWindow[1] != null)
                TSWindow[1].Tracks.ManualBitBlt = true;

            //e.Handled = true;
            if (Tracks.ShownFrom > 0)
            {
                Tracks.ShownFrom--;
                Tracks.HideCaret();

                if (Tracks.CursorY < Tracks.VisibleLineCount - 1 && Tracks.CursorY != Tracks.CenterLineIndex)
                {
                    Tracks.CursorY++;
                    Tracks.SetCaretPosition();
                }
                else if ((GetKeyState(Keys.Shift) & 128) == 0)
                    Tracks.RemoveSelection();

                Tracks.RedrawTracks();
                Tracks.ShowCaret();
            }
            else
            {
                patternLength = Tracks.ShownPattern == null ? VTModule.DefaultPatternLength : Tracks.ShownPattern.Length;

                if (MoveBetweenPatternsCheckBox.Checked)
                    BetweenPatternsUp();
                else
                {
                    Tracks.ShownFrom = patternLength - 1;
                    Tracks.CursorY = Tracks.CenterLineIndex;
                    Tracks.RemoveSelection();
                    Tracks.HideCaret();
                    Tracks.RedrawTracks();
                    Tracks.SetCaretPosition();
                    Tracks.ShowCaret();
                }
            }

            ShowStat();

            Tracks.ManualBitBlt = false;
            Tracks.DoBitBlt();

            if (TSWindow[0] != null)
            {
                TSWindow[0].Tracks.ManualBitBlt = false;
                TSWindow[0].Tracks.DoBitBlt();
            }

            if (TSWindow[1] != null)
            {
                TSWindow[1].Tracks.ManualBitBlt = false;
                TSWindow[1].Tracks.DoBitBlt();
            }
        }

        public void ResetSampleVolumeBuf()
        {
            Array.Clear(VolumeLBuffer);
            Array.Clear(VolumeRBuffer);
        }

        public void ClearSampleCols()
        {
            int line, col;
            SampleTick sampleTick;

            if (Samples.ShownSample == null)
                return;

            SaveSampleUndo(Samples.ShownSample);

            for (line = MainForm.SampleCopy.FromLine; line <= MainForm.SampleCopy.ToLine; line++)
            {
                sampleTick = Samples.ShownSample.Ticks[line];

                for (col = MainForm.SampleCopy.FromColumn; col <= MainForm.SampleCopy.ToColumn; col++)
                {
                    switch (col)
                    {
                        case 1:
                            sampleTick.Mixer_Ton = false;
                            break;
                        case 2:
                            sampleTick.Mixer_Noise = false;
                            break;
                        case 3:
                            sampleTick.Envelope_Enabled = false;
                            break;
                        case 4:
                            sampleTick.AddToTone = 0;
                            sampleTick.Ton_Accumulation = false;
                            break;
                        case 5:
                            sampleTick.Add_to_Envelope_or_Noise = 0;
                            sampleTick.Envelope_or_Noise_Accumulation = false;
                            break;
                        case 6:
                            sampleTick.Amplitude = 0;
                            sampleTick.Amplitude_Sliding = false;
                            sampleTick.Amplitude_Slide_Up = false;
                            break;
                    }
                }
            }

            SaveSampleRedo();

            Samples.HideCaret();
            Samples.Redraw();
            Samples.ShowCaret();
        }

        public void IncreaseSampleTone(SampleTick sampleTick)
        {
            bool isShiftDown = (GetAsyncKeyState(Keys.ShiftKey) & 0x8000) != 0;
            bool isCtrlDown = (GetAsyncKeyState(Keys.ControlKey) & 0x8000) != 0;

            if (isShiftDown)
                sampleTick.AddToTone = (short)(sampleTick.AddToTone + 32);
            else if (isCtrlDown)
                sampleTick.AddToTone = (short)(sampleTick.AddToTone + 64);
            else
                sampleTick.AddToTone = (short)(sampleTick.AddToTone + 1);

            if (Math.Abs(sampleTick.AddToTone) > 0xFFF)
                sampleTick.AddToTone = (short)(sampleTick.AddToTone & 0xFFF);
        }

        public void IncreaseSampleNoise(SampleTick sampleTick)
        {
            if (sampleTick.Add_to_Envelope_or_Noise < 31)
                sampleTick.Add_to_Envelope_or_Noise++;
        }

        public void IncreaseSampleAmplitude(SampleTick sampleTick, int line, bool overflow)
        {
            if (sampleTick.Amplitude == 0xF && !overflow)
                return;

            if (sampleTick.Amplitude == 0 && VolumeLBuffer[line] < 0)
                VolumeLBuffer[line]++;
            else if (sampleTick.Amplitude == 0xF)
            {
                if (VolumeRBuffer[line] < 0xF)
                    VolumeRBuffer[line]++;
            }
            else
                sampleTick.Amplitude++;
        }

        public void IncreaseSampleCols()
        {
            int i, line, column;
            SampleTick sampleTick;
            bool limit, shiftVol;

            if (Samples.ShownSample == null)
                return;

            limit = false;
            shiftVol = true;

            if (Samples.IsSelecting)
            {
                MainForm.SampleCopy.FromLine = (byte)Samples.SelectionStart;
                MainForm.SampleCopy.ToLine = (byte)Samples.SelectionEnd;
                MainForm.SampleCopy.FromColumn = 4;
                MainForm.SampleCopy.ToColumn = 6;
            }

            if (!Samples.IsSelecting && !Samples.IsColSelecting)
            {
                MainForm.SampleCopy.FromLine = (byte)Samples.CurrentLine();
                MainForm.SampleCopy.ToLine = MainForm.SampleCopy.FromLine;
                MainForm.SampleCopy.FromColumn = DetectSampleColumn(Samples.CursorX);
                MainForm.SampleCopy.ToColumn = MainForm.SampleCopy.FromColumn;
                shiftVol = false;
            }

            for (i = 0; i < VTModule.MaxSampleLength; i++)
            {
                if (VolumeRBuffer[i] == 0xF)
                {
                    limit = true;
                    break;
                }
            }

            if (limit)
                return;

            for (line = MainForm.SampleCopy.FromLine; line <= MainForm.SampleCopy.ToLine; line++)
            {
                sampleTick = Samples.ShownSample.Ticks[line];
                for (column = MainForm.SampleCopy.FromColumn; column <= MainForm.SampleCopy.ToColumn; column++)
                {
                    switch (column)
                    {
                        case 4:
                            IncreaseSampleTone(sampleTick);
                            break;
                        case 5:
                            IncreaseSampleNoise(sampleTick);
                            break;
                        case 6:
                            IncreaseSampleAmplitude(sampleTick, line, shiftVol);
                            break;
                    }
                }
            }

            SongChanged = true;
            BackupSongChanged = true;

            Samples.HideCaret();
            Samples.Redraw();
            Samples.ShowCaret();
        }

        public void DecreaseSampleTone(SampleTick sampleTick)
        {
            bool isShiftDown = (GetAsyncKeyState(Keys.ShiftKey) & 0x8000) != 0;
            bool isCtrlDown = (GetAsyncKeyState(Keys.ControlKey) & 0x8000) != 0;

            if (isShiftDown)
                sampleTick.AddToTone = (short)(sampleTick.AddToTone - 32);
            else if (isCtrlDown)
                sampleTick.AddToTone = (short)(sampleTick.AddToTone - 64);
            else
                sampleTick.AddToTone = (short)(sampleTick.AddToTone - 1);

            if (Math.Abs(sampleTick.AddToTone) > 0xFFF)
                sampleTick.AddToTone = (short)(sampleTick.AddToTone & 0xFFF);
        }

        public void DecreaseSampleNoise(SampleTick sampleTick)
        {
            if (sampleTick.Add_to_Envelope_or_Noise > -31)
                sampleTick.Add_to_Envelope_or_Noise--;
        }

        public void DecreaseSampleAmplitude(SampleTick sampleTick, int line, bool overflow)
        {
            if (sampleTick.Amplitude == 0 && !overflow)
                return;

            if (sampleTick.Amplitude == 0xF && VolumeRBuffer[line] > 0)
                VolumeRBuffer[line]--;
            else if (sampleTick.Amplitude == 0)
            {
                if (VolumeLBuffer[line] > -0xF)
                    VolumeLBuffer[line]--;
            }
            else
                sampleTick.Amplitude--;
        }

        public void DecreaseSampleCols()
        {
            int i, line, column;
            SampleTick sampleTick;
            bool limit = false;

            if (Samples.ShownSample == null)
                return;

            if (Samples.IsSelecting)
            {
                MainForm.SampleCopy.FromLine = (byte)Samples.SelectionStart;
                MainForm.SampleCopy.ToLine = (byte)Samples.SelectionEnd;
                MainForm.SampleCopy.FromColumn = 4;
                MainForm.SampleCopy.ToColumn = 6;
            }

            if (!Samples.IsSelecting && !Samples.IsColSelecting)
            {
                MainForm.SampleCopy.FromLine = (byte)Samples.CurrentLine();
                MainForm.SampleCopy.ToLine = MainForm.SampleCopy.FromLine;
                MainForm.SampleCopy.FromColumn = DetectSampleColumn(Samples.CursorX);
                MainForm.SampleCopy.ToColumn = MainForm.SampleCopy.FromColumn;
            }

            for (i = 0; i < VTModule.MaxSampleLength; i++)
            {
                if (VolumeLBuffer[i] == -0xF)
                {
                    limit = true;
                    break;
                }
            }

            if (limit)
                return;

            for (line = MainForm.SampleCopy.FromLine; line <= MainForm.SampleCopy.ToLine; line++)
            {
                sampleTick = Samples.ShownSample.Ticks[line];
                for (column = MainForm.SampleCopy.FromColumn; column <= MainForm.SampleCopy.ToColumn; column++)
                {
                    switch (column)
                    {
                        case 4:
                            DecreaseSampleTone(sampleTick);
                            break;
                        case 5:
                            DecreaseSampleNoise(sampleTick);
                            break;
                        case 6:
                            DecreaseSampleAmplitude(sampleTick, line, true);
                            break;
                    }
                }
            }

            SongChanged = true;
            BackupSongChanged = true;

            Samples.HideCaret();
            Samples.Redraw();
            Samples.ShowCaret();
        }

        public void Samples_MouseWheelDown(object sender, MouseEventArgs e)
        {
            bool isLeftDown = (e.Button & MouseButtons.Left) != 0;
            bool isRightDown = (e.Button & MouseButtons.Right) != 0;

            Samples.InputSNumber = 0;
            ValidateSample2(SampleIndex);

            // Decrease selected columns
            if (Samples.IsColSelecting)
            {
                DecreaseSampleCols();
                return;
            }

            if (isRightDown && SamplesRightMouseButton && SamplesClickEndLine < VTModule.MaxSampleLength)
                SamplesClickEndLine++;

            // Scroll sample down
            if (Samples.ShownFrom < VTModule.MaxSampleLength - Samples.LineCount)
            {
                Samples.ShownFrom++;
                SamplesLastCursorY = 255;
                Samples.HideCaret();
                Samples.Redraw();
                if (Samples.CursorY > 0)
                {
                    Samples.CursorY--;
                    Samples.SetCaretPosition();
                }
                Samples.ShowCaret();
            }

            /* if (isRightDown && SamplesRightMouseButton)
            {
                if (SamplesClickStartLine >= SamplesClickEndLine)
                {
                    ChangeSampleLength(SamplesClickStartLine + 1, true);
                    ChangeSampleLoop(SamplesClickEndLine, true);
                    SampleLengthUpDown.Value = SamplesClickStartLine + 1;
                    SampleLoopUpDown.Value = SamplesClickEndLine;
                }
                else
                {
                    ChangeSampleLength(SamplesClickEndLine, true);
                    SampleLengthUpDown.Value = SamplesClickEndLine;
                }
            } */

            // else     // Disabled Jump to samples top when scrolling beyond end of sample
            // begin
            // Samples.ShownFrom := 0;
            // Samples.CursorY := 0;
            // Samples.HideCaret;
            // Samples.Redraw();
            // Samples.SetCaretPosition;
            // Samples.ShowCaret
            // end

        }

        public void Samples_MouseWheel(object sender, MouseEventArgs e)
        {
            // Detect mouse wheel up down
            if (e.Delta > 0)
                Samples_MouseWheelUp(sender, e);
            else
                Samples_MouseWheelDown(sender, e);
        }

        public void Samples_MouseWheelUp(object sender, MouseEventArgs e)
        {
            bool isLeftDown = (e.Button & MouseButtons.Left) != 0;
            bool isRightDown = (e.Button & MouseButtons.Right) != 0;

            Samples.InputSNumber = 0;
            //e.Handled = true;

            // Increase selected columns
            if (Samples.IsColSelecting)
            {
                IncreaseSampleCols();
                return;
            }

            if (isRightDown && SamplesRightMouseButton && SamplesClickEndLine > 0)
                SamplesClickEndLine--;

            // Scroll sample up
            if (Samples.ShownFrom > 0)
            {
                Samples.ShownFrom--;
                SamplesLastCursorY = 255;
                Samples.HideCaret();
                Samples.Redraw();
                if (Samples.CursorY < Samples.LineCount - 1)
                {
                    Samples.CursorY++;
                    Samples.SetCaretPosition();
                }
                Samples.ShowCaret();
            }

            /* if (isRightDown && SamplesRightMouseButton)
            {
                if (SamplesClickStartLine >= SamplesClickEndLine)
                {
                    ChangeSampleLength(SamplesClickStartLine + 1, true);
                    ChangeSampleLoop(SamplesClickEndLine, true);
                    SampleLengthUpDown.Value = SamplesClickStartLine + 1;
                    SampleLoopUpDown.Value = SamplesClickEndLine;
                }
                else
                {
                    ChangeSampleLength(SamplesClickEndLine, true);
                    SampleLengthUpDown.Value = SamplesClickEndLine;
                }
            } */

            // else  // Disabled jump to bottom when scroll beyound start of sample
            // begin
            // Samples.ShownFrom := MaxSamLen - Samples.NOfLines;
            // Samples.CursorY := Samples.NOfLines - 1;
            // Samples.HideCaret;
            // Samples.Redraw();
            // Samples.SetCaretPosition;
            // Samples.ShowCaret
            // end

        }

        public void ValidatePattern2(int pattern)
        {
            VTModule.ValidatePattern(pattern, VTM);
            if (pattern == PatternIndex)
                Tracks.ShownPattern = VTM.Patterns[PatternIndex];
        }

        public void Tracks_KeyUp(object sender, KeyEventArgs e)
        {
            if (Tracks.KeyPressed != e.KeyCode)
                return;

            VTModule.UnlimitedDelay = false;
            Tracks.KeyPressed = 0;

            if (WaveOutAPI.IsPlaying)
            {
                Globals.MainForm.RestoreControls();

                if (AY.PlayMode == PlayModes.PlayLine)
                    WaveOutAPI.ResetPlaying();
                else
                    WaveOutAPI.StopPlaying();

                AY.PlayMode = PlayModes.PlayLine;
                PlayStopState = PlayStopState.Play;

                // Restore checkboxes
                MoveBetweenPatternsCheckBox.Enabled = true;
                UseLastNoteParamsCheckBox.Enabled = true;
                EnvelopeAsNoteCheckBox.Enabled = true;
            }

            // Return back after play (Ctrl+Enter)
            if (Tracks.ReturnAfterPlay)
            {
                Tracks.RedrawDisabled = true;
                IsSynchronizing = true;
                SelectPosition2(Tracks.ReturnPosition);
                Tracks.RedrawDisabled = false;
                IsSynchronizing = false;

                Tracks.ShownFrom = Tracks.ReturnShownFrom;
                Tracks.CursorY = Tracks.ReturnCursorY;

                Tracks.RemoveSelection();
                Tracks.HideCaret();
                Tracks.RedrawTracks();
                Tracks.ShowCaret();

                if (this.TSWindow[0] != null)
                {
                    SynchronizeModules();
                    this.TSWindow[0].Tracks.ShownFrom = Tracks.ReturnShownFrom;
                    this.TSWindow[0].Tracks.CursorY = Tracks.ReturnCursorY;
                    this.TSWindow[0].Tracks.RemoveSelection();
                    Tracks.HideCaret();
                    this.TSWindow[0].Tracks.RedrawTracks();
                    Tracks.ShowCaret();
                }

                if (this.TSWindow[1] != null)
                {
                    this.TSWindow[1].Tracks.ShownFrom = Tracks.ReturnShownFrom;
                    this.TSWindow[1].Tracks.CursorY = Tracks.ReturnCursorY;
                    this.TSWindow[1].Tracks.RemoveSelection();
                    Tracks.HideCaret();
                    this.TSWindow[1].Tracks.RedrawTracks();
                    Tracks.ShowCaret();
                }

                Tracks.ReturnAfterPlay = false;
            }

            // Restore cursor
            Tracks.RemoveSelection();
            Tracks.HideCaret();
            Tracks.RecreateCaret();
            Tracks.SetCaretPosition();
            Tracks.ShowCaret();
        }

        public void RestartPlayingPosition(int position)
        {
            if (!WaveOutAPI.IsPlaying || AY.PlayMode == PlayModes.PlayLine)
                return;

            if (position > VTM.Positions.Length - 1)
                return;

            if (PlayingWindow[0] == this)
            {
                if (!WaveOutAPI.HasReset)
                    WaveOutAPI.ResetPlaying();

                RerollToPos(position, 0);
                WaveOutAPI.UnResetPlaying();
            }
            else if (AY.ChipCount >= 2 && PlayingWindow[1] == this)
            {
                if (!WaveOutAPI.HasReset)
                    WaveOutAPI.ResetPlaying();

                RerollToPos(position, 1);
                WaveOutAPI.UnResetPlaying();
            }
            else if (AY.ChipCount == 3 && PlayingWindow[2] == this)
            {
                if (!WaveOutAPI.HasReset)
                    WaveOutAPI.ResetPlaying();

                RerollToPos(position, 2);
                WaveOutAPI.UnResetPlaying();
            }
        }

        public void RestartPlayingNote(int line)
        {
            if (WaveOutAPI.IsPlaying && VTModule.UnlimitedDelay)
                WaveOutAPI.StopPlaying();

            SetModuleFreq();

            SetPlayingWindow(0, this);
            SetPlayingWindow(1, null);
            SetPlayingWindow(2, null);

            if (TSWindow[0] != null && TSWindow[1] != null)
            {
                PlayingWindow[1] = TSWindow[0];
                PlayingWindow[2] = TSWindow[1];
                AY.ChipCount = 3;
            }
            else if (TSWindow[0] != null)
            {
                PlayingWindow[1] = TSWindow[0];
                AY.ChipCount = 2;
            }
            else
                AY.ChipCount = 1;

            RerollToLineNum(0, Tracks.CurrentPatternLine(), true);

            if (TSWindow[0] == null && TSWindow[1] == null)
                RestartPlayingLine(line);
            else
                RestartPlayingTS(true, true);
        }

        public void RestartPlayingLine(int line)
        {
            int[] nt = new int[3];
            int i;
            int envP, envT;
            bool stopped = false;

            SetModuleFreq();

            if (WaveOutAPI.IsPlaying)
            {
                Globals.MainForm.RestoreControls();

                if (AY.PlayMode == PlayModes.PlayLine)
                    WaveOutAPI.ResetPlaying();
                else
                {
                    WaveOutAPI.StopPlaying();
                    stopped = true;
                }
            }

            VTModule.UnlimitedDelay = false;

            SetPlayingWindow(0, this);
            AY.ChipCount = 1;
            AY.PlayMode = PlayModes.PlayLine;

            envP = VTModule.PlayArgs[0].EnvBase;
            envT = AY.SoundChip[0].AYRegisters.EnvType;

            VTModule.Module_SetPointer(VTM, 0);

            if (line >= 0)
            {
                for (i = 0; i < 3; i++)
                    nt[i] = VTModule.PlayArgs[0].ChannelParams[i].Note;

                WaveOutAPI.InitForAllTypes(false);

                for (i = 0; i < 3; i++)
                    VTModule.PlayArgs[0].ChannelParams[i].Note = (byte)nt[i];

                for (i = 0; i < 3; i++)
                {
                    if (VTM.ChannelStates[i].EnvelopeEnabled)
                    {
                        VTModule.PlayArgs[0].EnvBase = (short)envP;
                        AY.SoundChip[0].SetEnvelopeRegister((byte)envT);
                        break;
                    }
                }

                for (i = 0; i < 3; i++)
                {
                    if (((VTM.Patterns[PatternIndex].Lines[line].Channel[i].Note == -1) && (VTM.Patterns[PatternIndex].Lines[line].Channel[i].Envelope >= 1 && VTM.Patterns[PatternIndex].Lines[line].Channel[i].Envelope <= 14)))
                        VTModule.PlayArgs[0].ChannelParams[i].SoundEnabled = true;
                }

                VTModule.Module_SetCurrentPattern(PatternIndex);
                VTModule.Pattern_SetCurrentLine(line);
            }
            else
            {
                nt[0] = VTModule.PlayArgs[0].ChannelParams[VTModule.CenterChannel].Note;
                WaveOutAPI.InitForAllTypes(false);
                VTModule.PlayArgs[0].ChannelParams[VTModule.CenterChannel].Note = (byte)nt[0];

                if (VTM.ChannelStates[VTModule.CenterChannel].EnvelopeEnabled)
                {
                    VTModule.PlayArgs[0].EnvBase = (short)envP;
                    AY.SoundChip[0].SetEnvelopeRegister((byte)envT);
                }

                if (((VTM.ReservedPattern.Lines[-(line + 1)].Channel[0].Note == -1) && (VTM.ReservedPattern.Lines[-(line + 1)].Channel[0].Envelope >= 1 && VTM.ReservedPattern.Lines[-(line + 1)].Channel[0].Envelope <= 14)))
                    VTModule.PlayArgs[0].ChannelParams[VTModule.CenterChannel].SoundEnabled = true;

                VTModule.Module_SetCurrentPattern(-1);
                VTModule.Pattern_SetCurrentLine(-(line + 1));
            }

            VTModule.Pattern_PlayCurrentLine();
            WaveOutAPI.LineReady = true;

            if (WaveOutAPI.IsPlaying)
            {
                if (stopped)
                    WaveOutAPI.StartWOThread();
                else
                    WaveOutAPI.UnResetPlaying();
            }
            else
                WaveOutAPI.StartWOThread();
        }

        public void RestartPlaying(bool playPat, bool enter)
        {
            bool stopped = false;

            SetModuleFreq();

            if (WaveOutAPI.IsPlaying)
            {
                Globals.MainForm.RestoreControls();

                if (AY.PlayMode == PlayModes.PlayLine)
                    WaveOutAPI.ResetPlaying();
                else
                {
                    WaveOutAPI.StopPlaying();
                    stopped = true;
                }
            }

            VTModule.UnlimitedDelay = false;
            PlayStopState = PlayStopState.Stop;

            if (playPat)
                AY.PlayMode = PlayModes.PlayPattern;
            else
                AY.PlayMode = PlayModes.PlayModule;

            if (enter)
            {
                SetPlayingWindow(0, this);
                // BetweenPatterns.Enabled := False;
                // DuplicateNoteParams.Enabled := False;
                // EnvelopeAsNoteOpt.Enabled := False;
            }
            else
                Globals.MainForm.DisableControls(false);

            AY.ChipCount = 1;
            Tracks.RemoveSelection();

            AY.PlayingModule[0] = VTM;
            VTModule.PlayArgs[0].PositionIndex = PositionIndex;
            VTModule.Module_SetCurrentPattern(PatternIndex);

            // Shift+Enter - infinite play current line
            if (enter && (GetKeyState(Keys.Shift) & 128) != 0)
            {
                RerollToLine0(0);
                VTModule.UnlimitedDelay = true;
                Tracks.HideCaret();
            }
            else
            {
                RerollToLine(0);
                VTModule.UnlimitedDelay = false;
            }

            if (WaveOutAPI.IsPlaying)
            {
                if (stopped)
                    WaveOutAPI.StartWOThread();
                else
                    WaveOutAPI.UnResetPlaying();
            }
            else
                WaveOutAPI.StartWOThread();
        }

        public void RestartPlayingTS(bool playPattern, bool playNote)
        {
            if (WaveOutAPI.IsPlaying)
            {
                Globals.MainForm.RestoreControls();
                WaveOutAPI.StopPlaying();
                VTModule.UnlimitedDelay = false;
            }

            PlayStopState = PlayStopState.Stop;

            if (playPattern)
                AY.PlayMode = PlayModes.PlayPattern;
            else
            {
                AY.PlayMode = PlayModes.PlayModule;
                Globals.MainForm.DisableControls(false);
            }

            SetPlayingWindow(0, this);
            SetPlayingWindow(1, TSWindow[0]);

            PlayingWindow[0].Tracks.RemoveSelection();
            PlayingWindow[1].Tracks.RemoveSelection();

            if (TSWindow[1] != null)
            {
                SetPlayingWindow(2, TSWindow[1]);
                PlayingWindow[2].Tracks.RemoveSelection();
                AY.ChipCount = 3;
            }
            else
                AY.ChipCount = 2;

            for (int _i = 0; _i < AY.ChipCount; _i++)
            {
                if (PlayingWindow[_i] != null)
                    AY.PlayingModule[_i] = PlayingWindow[_i].VTM;
            }

            // Shift+Enter - infinite play current line
            if (playNote || (GetKeyState(Keys.Shift) & 128) != 0)
            {
                VTModule.UnlimitedDelay = true;
                if (!playNote)
                {
                    Tracks.HideCaret();
                    RerollToLine0(0);
                }
            }
            else
                RerollToLine(0);

            WaveOutAPI.StartWOThread();
        }

        public void StopAndRestart()
        {
            if (!WaveOutAPI.IsPlaying)
                return;

            if (WaveOutAPI.HasReset)
                return;

            if (AY.PlayMode != PlayModes.PlayModule)
                return;

            WaveOutAPI.ResetPlaying();
            PlayingWindow[0].RerollToLine(0);
            WaveOutAPI.UnResetPlaying();
        }

        public void RerollToInt(int intIndex, int chipIndex)
        {
            VTModule.Module_SetPointer(VTM, chipIndex);
            VTModule.Module_SetDelay((sbyte)VTM.InitialDelay);
            VTModule.Module_SetCurrentPosition(0);

            if (intIndex > 0)
            {
                do
                {
                    if (VTModule.Module_PlayCurrentLine() == PlayLineResult.AllPatternsEnded)
                    {
                        if (!AY.LoopAllowed && (!Main.LoopAllAllowed || Globals.MainForm.MdiChildren.Length != 1))
                        {
                            AY.RealEnd[chipIndex] = true;
                            AY.SoundChip[chipIndex].SetAmplitudeA(0);
                            AY.SoundChip[chipIndex].SetAmplitudeB(0);
                            AY.SoundChip[chipIndex].SetAmplitudeC(0);
                        }
                    }
                }
                while (VTModule.PlayArgs[chipIndex].IntCount < intIndex && !AY.RealEnd[chipIndex]);

                WaveOutAPI.LineReady = true;
            }
        }

        public void RerollToPos(int position, int chipIndex)
        {
            PlayLineResult result;
            WaveOutAPI.InitForAllTypes(true);
            VTModule.Module_SetPointer(VTM, chipIndex);
            VTModule.Module_SetDelay((sbyte)VTM.InitialDelay);
            VTModule.Module_SetCurrentPosition(0);

            if (position > 0)
            {
                do
                {
                    result = VTModule.Module_PlayCurrentLine();
                }
                while (result != PlayLineResult.PatternEnded && VTModule.PlayArgs[chipIndex].PositionIndex != position);

                WaveOutAPI.LineReady = true;
            }

            if (AY.ChipCount == 2)
                PlayingWindow[1 - chipIndex].RerollToInt(VTModule.PlayArgs[chipIndex].IntCount, 1 - chipIndex);
            else if (AY.ChipCount == 3)
            {
                for (int i = 0; i < AY.ChipCount; i++)
                {
                    if (i == chipIndex)
                        continue;

                    PlayingWindow[i].RerollToInt(VTModule.PlayArgs[chipIndex].IntCount, i);
                }
            }
        }

        public void RerollToLineNum(int chipIndex, int lineIndex, bool zeroLine, VTM srcVTM)
        {
            PlayLineResult result;

            if (srcVTM == null)
                srcVTM = VTM;

            WaveOutAPI.InitForAllTypes(true);

            VTModule.Module_SetPointer(srcVTM, chipIndex);
            VTModule.Module_SetDelay((sbyte)srcVTM.InitialDelay);
            VTModule.Module_SetCurrentPosition(0);

            if (PositionIndex > 0)
            {
                do
                {
                    result = VTModule.Module_PlayCurrentLine();
                }
                while (result != PlayLineResult.AllPatternsEnded && VTModule.PlayArgs[chipIndex].PositionIndex != PositionIndex);

                WaveOutAPI.LineReady = true;
                zeroLine = false;
            }

            if (zeroLine && lineIndex == 0)
            {
                VTModule.Module_PlayCurrentLine();
                WaveOutAPI.LineReady = true;
            }
            else if (lineIndex > 0)
            {
                do
                {
                    result = VTModule.Module_PlayCurrentLine();
                }
                while (result != PlayLineResult.LineEnded && VTModule.PlayArgs[chipIndex].LineIndex != lineIndex + 1);

                WaveOutAPI.LineReady = true;
            }

            if (AY.ChipCount == 2)
                PlayingWindow[1 - chipIndex].RerollToInt(VTModule.PlayArgs[chipIndex].IntCount, 1 - chipIndex);
            else if (AY.ChipCount == 3)
            {
                for (int i = 0; i < AY.ChipCount; i++)
                {
                    if (i == chipIndex)
                        continue;

                    PlayingWindow[i].RerollToInt(VTModule.PlayArgs[chipIndex].IntCount, i);
                }
            }
        }

        public void RerollToLineNum(int chipIndex, int lineIndex, bool zeroLine)
        {
            RerollToLineNum(chipIndex, lineIndex, zeroLine, null);
        }

        public void RerollToLine(int chipIndex)
        {
            PlayLineResult result;

            WaveOutAPI.InitForAllTypes(true);

            VTModule.Module_SetPointer(VTM, chipIndex);
            VTModule.Module_SetDelay((sbyte)VTM.InitialDelay);
            VTModule.Module_SetCurrentPosition(0);

            if (PositionIndex > 0)
            {
                do
                {
                    result = VTModule.Module_PlayCurrentLine();
                }
                while (result != PlayLineResult.AllPatternsEnded && VTModule.PlayArgs[chipIndex].PositionIndex != PositionIndex);

                WaveOutAPI.LineReady = true;
            }

            if (Tracks.ShownFrom > 0)
            {
                do
                {
                    result = VTModule.Module_PlayCurrentLine();
                }
                while (result != PlayLineResult.LineEnded && VTModule.PlayArgs[chipIndex].LineIndex != Tracks.ShownFrom + 1);

                WaveOutAPI.LineReady = true;
            }

            if (AY.ChipCount == 2)
                PlayingWindow[1 - chipIndex].RerollToInt(VTModule.PlayArgs[chipIndex].IntCount, 1 - chipIndex);
            else if (AY.ChipCount == 3)
            {
                for (int i = 0; i < AY.ChipCount; i++)
                {
                    if (i == chipIndex)
                        continue;

                    PlayingWindow[i].RerollToInt(VTModule.PlayArgs[chipIndex].IntCount, i);
                }
            }
        }

        public void RerollToLine0(int chipIndex)
        {
            PlayLineResult result;

            WaveOutAPI.InitForAllTypes(true);

            VTModule.Module_SetPointer(VTM, chipIndex);
            VTModule.Module_SetDelay((sbyte)VTM.InitialDelay);
            VTModule.Module_SetCurrentPosition(0);

            if (PositionIndex > 0)
            {
                do
                {
                    result = VTModule.Module_PlayCurrentLine();
                }
                while (result != PlayLineResult.AllPatternsEnded && VTModule.PlayArgs[chipIndex].PositionIndex != PositionIndex);
                WaveOutAPI.LineReady = true;
            }

            if (Tracks.ShownFrom == 0)
            {
                VTModule.Module_PlayCurrentLine();
                WaveOutAPI.LineReady = true;
            }
            else if (Tracks.ShownFrom > 0)
            {
                do
                {
                    result = VTModule.Module_PlayCurrentLine();
                }
                while (result != PlayLineResult.LineEnded && VTModule.PlayArgs[chipIndex].LineIndex != Tracks.ShownFrom + 1);
                WaveOutAPI.LineReady = true;
            }

            if (AY.ChipCount == 2)
                PlayingWindow[1 - chipIndex].RerollToInt(VTModule.PlayArgs[chipIndex].IntCount, 1 - chipIndex);
            else if (AY.ChipCount == 3)
            {
                for (int i = 0; i < AY.ChipCount; i++)
                {
                    if (i == chipIndex)
                        continue;

                    PlayingWindow[i].RerollToInt(VTModule.PlayArgs[chipIndex].IntCount, i);
                }
            }
        }

        public void RerollToPatternLine(int chipIndex)
        {
            PlayLineResult result;
            WaveOutAPI.LineReady = false;
            int currentLine = Tracks.CurrentPatternLine();

            if (currentLine >= 0 && currentLine < GetCurrentPatternLength())
            {
                do
                {
                    result = VTModule.Pattern_PlayCurrentLine();
                }
                while (result != PlayLineResult.LineEnded && VTModule.PlayArgs[chipIndex].LineIndex != currentLine + 1);
                WaveOutAPI.LineReady = true;
            }
        }

        public void GoToTime(int time)
        {
            int position, lineIndex;

            VTModule.GetTimeParams(VTM, time, out position, out lineIndex);

            if (AY.PlayMode == PlayModes.PlayPattern)
                return;

            // if Pos = -1 then Exit;
            Globals.MainForm.RedrawPlWindow(this, position, VTM.Positions.Value[position], lineIndex);
        }

        public void SynchronizeModules()
        {
            if (IsSynchronizing || (TSWindow[0] == null && TSWindow[1] == null))
                return;

            //if (Tracks.ShownFrom == TSWindow.Tracks.ShownFrom && PositionNumber == TSWindow.PositionNumber)
            //    return;

            if (!WaveOutAPI.IsPlaying || AY.PlayMode != PlayModes.PlayModule)
            {
                TSWindow[0].IsSynchronizing = true;

                try
                {
                    TSWindow[0].GoToTime(PosBegin + LineInts);
                }
                finally
                {
                    TSWindow[0].IsSynchronizing = false;
                }

                if (TSWindow[1] != null)
                {
                    TSWindow[1].IsSynchronizing = true;

                    try
                    {
                        TSWindow[1].GoToTime(PosBegin + LineInts);
                    }
                    finally
                    {
                        TSWindow[1].IsSynchronizing = false;
                    }
                }
            }
        }

        public void SetPositionsGridScroll(int columnIndex)
        {
            int scrollPos, colPos, visibleArea, selRows, visibleColCount;
            bool shift;
            visibleColCount = PositionsGrid.ClientSize.Width / (PositionsGrid.Columns[0].Width + 1);
            selRows = PositionsGrid.Selection.Right - PositionsGrid.Selection.Left + 1;
            shift = false;

            if (columnIndex == -1)
            {
                if (selRows == 1)
                    columnIndex = PositionsGrid.Selection.Left + 1;
                else
                {
                    columnIndex = PositionsGrid.Selection.Left - (visibleColCount / 2) + (selRows / 2);
                    shift = true;
                }
            }

            if (columnIndex >= VTM.Positions.Length)
                columnIndex = VTM.Positions.Length - 1;

            scrollPos = PositionsGrid.HorizontalScrollingOffset;
            colPos = columnIndex * (PositionsGrid.Columns[0].Width + 1);
            visibleArea = scrollPos + PositionsGrid.ClientSize.Width;

            if (colPos < scrollPos || colPos >= visibleArea || shift)
                scrollPos = colPos;

            //PositionsScrollBox.HorizontalScroll.Value = Math.Min(scrollPos, PositionsScrollBox.HorizontalScroll.Maximum);
            /* if (TSWindow[0] != null && !IsSynchronizing)
            {
                TSWindow[0].IsSynchronizing = true;
                TSWindow[0].SelectPosition2(PositionNumber);
                TSWindow[0].IsSynchronizing = false;
            }

            if (TSWindow[1] != null && !IsSynchronizing)
            {
                TSWindow[1].IsSynchronizing = true;
                TSWindow[1].SelectPosition2(PositionNumber);
                TSWindow[1].IsSynchronizing = false;
            } */
        }

        public void SelectPosition(int position)
        {
            int prevPatNum;

            InputPNumber = 0;

            if (position > VTM.Positions.Length)
            {
                PosBegin = TotInts;
                // Label25.Caption := IntsToTime(PosBegin);
                ReCalcTimes(PosBegin);
                UpdateIntsInfo(PosBegin);
                SynchronizeModules();
                return;
            }

            if (position >= VTM.Positions.Length)
                return;

            if (position < 0)
                position = 0;

            if (WaveOutAPI.IsPlaying && AY.PlayMode == PlayModes.PlayModule && (PlayingWindow[0] == this || (AY.ChipCount > 1 && PlayingWindow[1] == this)))
            {
                PositionIndex = position;
                CalculatePos0();
                RestartPlayingPosition(position);
            }
            else if (!WaveOutAPI.IsPlaying || AY.PlayMode != PlayModes.PlayPattern)
            {
                PositionIndex = position;
                CalculatePos0();
                prevPatNum = (int)PatternNumUpDown.Value;
                PatternNumUpDown.Value = VTM.Positions.Value[position];

                if (prevPatNum == VTM.Positions.Value[position])
                {
                    Tracks.ShownFrom = 0;
                    Tracks.CursorY = Tracks.CenterLineIndex;
                    Tracks.RemoveSelection();
                    Tracks.HideCaret();
                    Tracks.RedrawTracks();
                    Tracks.SetCaretPosition();
                    Tracks.ShowCaret();
                }
            }

            SynchronizeModules();
        }

        public void SelectPosition2(int position)
        {
            Rectangle selectionRect;
            int prevPatNum;

            if (VTM == null)
                return;

            if (VTM.Positions.Length == 0)
            {
                selectionRect = Rectangle.Empty;
                SetPositionsGridScroll(0);
                PositionsGrid.Selection = selectionRect;
                PositionIndex = position;
                return;
            }

            if (PositionsGrid.Selection.Left != position)
            {
                position = Math.Clamp(position, 0, VTM.Positions.Length - 1);
                selectionRect = Rectangle.FromLTRB(position, 0, position, 0);
                SetPositionsGridScroll(position);
                PositionsGrid.Selection = selectionRect;
                InputPNumber = 0;
                PositionIndex = position;
                CalculatePos0();
            }

            prevPatNum = (int)PatternNumUpDown.Value;
            PatternNumUpDown.Value = VTM.Positions.Value[position];

            if (prevPatNum == VTM.Positions.Value[position])
            {
                // Tracks.ShownFrom := 0;
                // Tracks.CursorY := Tracks.N1OfLines;
                // Tracks.RemoveSelection;
                Tracks.HideCaret();
                Tracks.RedrawTracks();
                // Tracks.SetCaretPosition;
                Tracks.ShowCaret();
            }
        }

        public void SelectPositions(Rectangle selGrid)
        {
            if (selGrid.Left > 0)
            {
                if (selGrid.Left >= PositionsGrid.LeftCol + PositionsGrid.VisibleColumnCount)
                    PositionsGrid.LeftCol = selGrid.Left + 1 - PositionsGrid.VisibleColumnCount;
                else if (selGrid.Left < PositionsGrid.LeftCol)
                    PositionsGrid.LeftCol = selGrid.Left;
            }

            PositionsGrid.Selection = selGrid;
            InputPNumber = 0;
            CalculatePos0();

            PatternsOrderSelection = PositionsGrid.Selection;
        }

        public void PositionsGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            if (PositionsGrid.CurrentCell == null)
                return;

            if (Tracks == null)
                return;

            Debug.WriteLine("CurrentCellChanged");

            int columnIndex = PositionsGrid.CurrentCell.ColumnIndex;

            if (Tracks.IsTrackPlaying() && columnIndex >= VTM.Positions.Length)
            {
                columnIndex = VTM.Positions.Length - 1;
                SelectPosition2(columnIndex);
            }
            else
            {
                SetPositionsGridScroll(columnIndex);
                SelectPosition(columnIndex);
            }
        }

        public void ChangePositionValue(int position, int value)
        {
            SongChanged = true;
            BackupSongChanged = true;
            PositionIndex = position;

            AddUndo(TChangeAction.ChangePositionValue, VTM.Positions.Value[position], value);

            if (position == VTM.Positions.Length)
                VTM.Positions.Length++;

            if (!UndoWorking)
            {
                ChangeList[ChangeCount - 1].NewParams.Params.CurrentPosition = position;
                ChangeList[ChangeCount - 1].OldParams.Params.CurrentPosition = position;
                ChangeList[ChangeCount - 1].NewParams.Params.PositionListLen = VTM.Positions.Length;
                ChangeList[ChangeCount - 1].OldGridSelection = PositionsGrid.Selection;
                ChangeList[ChangeCount - 1].NewGridSelection = PositionsGrid.Selection;
            }

            VTM.Positions.Value[position] = value;

            string s = value.ToString();

            if (position == VTM.Positions.Loop)
                s = $"L{s}";

            PositionsGrid[position, 0].Value = s;
            CalcTotLen();
            ValidatePattern2(value);
            SelectPosition2(position);
        }

        public void ChangePositionValueNoUndo(int position, int value)
        {
            SongChanged = true;
            BackupSongChanged = true;
            PositionIndex = position;

            if (position == VTM.Positions.Length)
                VTM.Positions.Length++;

            VTM.Positions.Value[position] = value;

            string s = value.ToString();

            if (position == VTM.Positions.Loop)
                s = $"L{s}";

            PositionsGrid[position, 0].Value = s;
            CalcTotLen();
            ValidatePattern2(value);
            if (PositionsGrid.Selection.Left != position)
                SelectPosition2(position);
        }

        public int GetNewPatternNumber()
        {
            int result = VTM.Positions.Value.Max() + 1;

            if (result > VTModule.MaxPatternIndex)
                return -1;

            ValidatePattern2(result);
            return result;
        }

        public int[] GetNewPatternNumbers(int numNewPatterns)
        {
            int newPatternNumber = VTM.Positions.Value.Max() + 1;
            int[] patternsArray = Array.Empty<int>();

            if (newPatternNumber + numNewPatterns - 1 <= VTModule.MaxPatternIndex)
            {
                Array.Resize(ref patternsArray, numNewPatterns);

                for (int i = 0; i < numNewPatterns; i++)
                {
                    ValidatePattern2(newPatternNumber);
                    patternsArray[i] = newPatternNumber;
                    newPatternNumber++;
                }
            }

            return patternsArray;
        }

        public void IncreaseTrackLength(int NumNewPositions)
        {
            Position position = VTM.Positions;
            position.Length += NumNewPositions;

            for (int i = position.Length - 1; i >= position.Length - NumNewPositions; i--)
                position.Value[i] = 0;
        }

        public void RedrawPatternPositions()
        {
            for (int i = 0; i < VTM.Positions.Length; i++)
            {
                string s = VTM.Positions.Value[i].ToString();

                if (i == VTM.Positions.Loop)
                    s = $"L{s}";

                PositionsGrid[i, 0].Value = s;
            }

            for (int i = VTM.Positions.Length; i < PositionsGrid.ColumnCount; i++)
                PositionsGrid[i, 0].Value = "";

            InitStringGridMetrics();
        }

        public void UnselectPositions()
        {
            PositionsGrid.Selection = new Rectangle(-1, -1, -1, -1);
            //PositionsGrid.Repaint;
            PositionsGrid.Invalidate();
            PositionsGrid.Update();
        }

        public void ShiftLoopPosition(int operation, int sourceCol, int destCol, int numChangedPositions)
        {
            int loop = VTM.Positions.Loop;
            int sourceColsRight = sourceCol + numChangedPositions - 1;
            int sourceColsLeft = sourceCol;
            bool loopInsideSelected = sourceColsLeft <= loop && sourceColsRight >= loop;

            if (destCol >= VTM.Positions.Length)
                destCol = VTM.Positions.Length - 1;

            if (operation == POS_MOVE)
            {
                if (!loopInsideSelected && (sourceColsRight > loop) && (destCol <= loop))
                    VTM.Positions.Loop += numChangedPositions;

                if (!loopInsideSelected && (sourceColsRight < loop) && (destCol >= loop))
                    VTM.Positions.Loop -= numChangedPositions;

                if (loopInsideSelected && (destCol > loop))
                    VTM.Positions.Loop += destCol - sourceColsRight;

                if (loopInsideSelected && (destCol < loop))
                    VTM.Positions.Loop -= sourceColsLeft - destCol;
            }
            else if (operation == POS_COPY)
            {
                if (destCol <= loop)
                    VTM.Positions.Loop += numChangedPositions;
            }
            else if (operation == POS_DELETE)
            {
                if (sourceColsRight <= loop)
                    VTM.Positions.Loop -= numChangedPositions;

                if (loopInsideSelected)
                    VTM.Positions.Loop = 0;
            }
        }

        public void SavePositionsUndo()
        {
            // Add undo event 'Insert new position'
            AddUndo(TChangeAction.InsertPosition, 0, 0);

            // Save current selected track position
            ChangeList[ChangeCount - 1].OldGridSelection = PositionsGrid.Selection;

            // Save current selected track position
            ChangeList[ChangeCount - 1].OldParams.Params.CurrentPosition = PositionIndex;

            // Save current pattern number
            ChangeList[ChangeCount - 1].OldParams.Params.CurrentPattern = PatternIndex;

            // Save cursor position
            ChangeList[ChangeCount - 1].OldParams.Params.PatternCursorX = Tracks.CursorX;
            ChangeList[ChangeCount - 1].OldParams.Params.PatternCursorY = Tracks.CursorY;
            ChangeList[ChangeCount - 1].OldParams.Params.PatternShownFrom = Tracks.ShownFrom;

            // Save current positions array
            ChangeList[ChangeCount - 1].PositionList = (Position)VTM.Positions.Clone();
        }

        public void SavePositionsRedo()
        {
            ChangeList[ChangeCount - 1].NewGridSelection = PositionsGrid.Selection;
            ChangeList[ChangeCount - 1].NewParams.Params.PatternCursorX = Tracks.CursorX;
            ChangeList[ChangeCount - 1].NewParams.Params.PatternCursorY = Tracks.CursorY;
            ChangeList[ChangeCount - 1].NewParams.Params.PatternShownFrom = Tracks.ShownFrom;
            ChangeList[ChangeCount - 1].NewParams.Params.CurrentPosition = PositionIndex;
            ChangeList[ChangeCount - 1].NewParams.Params.CurrentPattern = PatternIndex;
        }

        public void SaveTrackUndo()
        {
            // Add undo event 'Change Positions And Patterns'
            AddUndo(TChangeAction.ChangePositionsAndPatterns, 0, 0);

            // Save current selected track position
            ChangeList[ChangeCount - 1].OldParams.Params.CurrentPosition = PositionIndex;

            // Save current pattern number
            ChangeList[ChangeCount - 1].OldParams.Params.CurrentPattern = PatternIndex;

            // Save current selected track position
            ChangeList[ChangeCount - 1].OldGridSelection = PositionsGrid.Selection;

            // Save current positions array
            ChangeList[ChangeCount - 1].PositionList = (Position)VTM.Positions.Clone();

            // Prepare arrays for store previous patterns version
            Array.Resize(ref ChangePatternsList, ChangePatternsList.Length + 1);
            Array.Resize(ref ChangeNilPatternsList, ChangePatternsList.Length);
            int index = ChangePatternsList.Length - 1;
            ChangeNilPatternsList[index] ??= Array.Empty<int>();
            Array.Resize(ref ChangePatternsList[index], 2);

            int lastIndex = 0;

            // Save current patterns data
            for (int i = 0; i < VTM.Patterns.Length; i++)
            {
                // Save number of unused pattern
                if (VTM.Patterns[i] == null)
                {
                    // Increase dynamic array length
                    Array.Resize(ref ChangeNilPatternsList[index], (ChangeNilPatternsList[index]?.Length ?? 0) + 1);

                    // Get last index
                    lastIndex = ChangeNilPatternsList[index].Length - 1;

                    // Save nil-pattern number
                    ChangeNilPatternsList[index][lastIndex] = i;
                }
                // Save used pattern data
                else
                {
                    // Prepare structure
                    var savedPattern = new ChangePattern
                    {
                        Number = i,
                        Pattern = new Pattern
                        {
                            Lines = VTM.Patterns[i].Lines,
                            Length = VTM.Patterns[i].Length
                        }
                    };

                    // Increase array length

                    Array.Resize(ref ChangePatternsList[index][0], (ChangePatternsList[index][0]?.Length ?? 0) + 1);

                    // Get last index
                    lastIndex = ChangePatternsList[index][0].Length - 1;

                    // Save pattern
                    ChangePatternsList[index][0][lastIndex] = savedPattern;
                }
            }

            ChangeList[ChangeCount - 1].ComParams.Patterns = (ChangePattern[][])ChangePatternsList[index].Clone();
            ChangeList[ChangeCount - 1].ComParams.NilPatterns = (int[])(ChangeNilPatternsList[index]?.Clone() ?? Array.Empty<int>());
        }

        public void SaveTrackRedo()
        {
            int index = ChangePatternsList.Length - 1;

            // Save new patterns data
            for (int i = 0; i < VTM.Patterns.Length; i++)
            {
                // Save number of unused pattern
                if (VTM.Patterns[i] != null)
                {
                    // Prepare structure
                    var savedPattern = new ChangePattern
                    {
                        Number = i,
                        Pattern = new Pattern
                        {
                            Lines = VTM.Patterns[i].Lines,
                            Length = VTM.Patterns[i].Length
                        }
                    };

                    // Increase array length
                    Array.Resize(ref ChangePatternsList[index][1], (ChangePatternsList[index][1]?.Length ?? 0) + 1);

                    // Get last index
                    int lastIndex = ChangePatternsList[index][1].Length - 1;

                    // Save pattern
                    ChangePatternsList[index][1][lastIndex] = savedPattern;
                }
            }

            ChangeList[ChangeCount - 1].NewGridSelection = PositionsGrid.Selection;
            ChangeList[ChangeCount - 1].NewParams.Params.CurrentPosition = PositionIndex;
            ChangeList[ChangeCount - 1].NewParams.Params.PatternCursorX = Tracks.CursorX;
            ChangeList[ChangeCount - 1].NewParams.Params.PatternCursorY = Tracks.CursorY;
            ChangeList[ChangeCount - 1].NewParams.Params.PatternShownFrom = Tracks.ShownFrom;
            ChangeList[ChangeCount - 1].NewParams.Params.CurrentPattern = PatternIndex;
        }

        public void SavePatternUndo()
        {
            // Add undo event 'Change Positions And Patterns'
            AddUndo(TChangeAction.ChangePatternContent, 0, 0);

            // Save current selected track position
            ChangeList[ChangeCount - 1].OldParams.Params.CurrentPosition = PositionIndex;
            ChangeList[ChangeCount - 1].NewParams.Params.CurrentPosition = PositionIndex;

            // Save current pattern number
            ChangeList[ChangeCount - 1].OldParams.Params.CurrentPattern = PatternIndex;
            ChangeList[ChangeCount - 1].NewParams.Params.CurrentPattern = PatternIndex;

            // Save current selected track position
            ChangeList[ChangeCount - 1].OldGridSelection = PositionsGrid.Selection;
            ChangeList[ChangeCount - 1].NewGridSelection = PositionsGrid.Selection;

            // Increase array
            Array.Resize(ref ChangeOnePatternList, ChangeOnePatternList.Length + 1);
            int index = ChangeOnePatternList.Length - 1;
            ChangeOnePatternList[index] = new ChangeOnePattern();

            // Save current pattern state
            ChangeOnePatternList[index].OldPattern.Length = Tracks.ShownPattern.Length;
            ChangeOnePatternList[index].OldPattern.Lines = Tracks.ShownPattern.Lines;
        }

        public void SavePatternRedo()
        {
            int index = ChangeOnePatternList.Length - 1;

            // Save result pattern state
            ChangeOnePatternList[index].NewPattern.Length = Tracks.ShownPattern.Length;
            ChangeOnePatternList[index].NewPattern.Lines = Tracks.ShownPattern.Lines;

            ChangeList[ChangeCount - 1].ComParams.ChangedPattern = (ChangeOnePattern)ChangeOnePatternList[index].Clone();
            ChangeList[ChangeCount - 1].NewParams.Params.CurrentPattern = PatternIndex;
            ChangeList[ChangeCount - 1].NewParams.Params.PatternCursorX = Tracks.CursorX;
            ChangeList[ChangeCount - 1].NewParams.Params.PatternCursorY = Tracks.CursorY;
            ChangeList[ChangeCount - 1].NewParams.Params.PatternShownFrom = Tracks.ShownFrom;
        }

        public void SaveSampleUndo(Sample Sample)
        {
            if (Sample == null)
                return;

            if (Samples.UndoSaved)
                return;

            SongChanged = true;
            BackupSongChanged = true;

            AddUndo(TChangeAction.ChangeEntireSample, 0, 0);
            ChangeList[ChangeCount - 1].OldParams.Params.SampleShownFrom = Samples.ShownFrom;
            ChangeList[ChangeCount - 1].OldParams.Params.SampleCursorX = Samples.CursorX;
            ChangeList[ChangeCount - 1].OldParams.Params.SampleCursorY = Samples.CursorY;

            Array.Resize(ref ChangeSamplesList, ChangeSamplesList.Length + 1);
            int index = ChangeSamplesList.Length - 1;

            ChangeSamplesList[index] = new ChangeSample();
            ChangeSamplesList[index].Number = (int)SampleNumUpDown.Value;
            ChangeSamplesList[index].OldSample.Length = Sample.Length;
            ChangeSamplesList[index].OldSample.Loop = Sample.Loop;
            ChangeSamplesList[index].OldSample.Enabled = Sample.Enabled;
            ChangeSamplesList[index].OldSample.Ticks = Sample.Ticks;

            Samples.UndoSaved = true;
        }

        public void SaveSampleRedo()
        {
            if (!Samples.UndoSaved)
                return;

            ChangeList[ChangeCount - 1].NewParams.Params.SampleShownFrom = Samples.ShownFrom;
            ChangeList[ChangeCount - 1].NewParams.Params.SampleCursorX = Samples.CursorX;
            ChangeList[ChangeCount - 1].NewParams.Params.SampleCursorY = Samples.CursorY;

            int index = ChangeSamplesList.Length - 1;

            ChangeSamplesList[index].NewSample.Length = Samples.ShownSample.Length;
            ChangeSamplesList[index].NewSample.Loop = Samples.ShownSample.Loop;
            ChangeSamplesList[index].NewSample.Enabled = Samples.ShownSample.Enabled;
            ChangeSamplesList[index].NewSample.Ticks = Samples.ShownSample.Ticks;

            ChangeList[ChangeCount - 1].ComParams.EntireSample = (ChangeSample)ChangeSamplesList[index].Clone();

            Samples.UndoSaved = false;
        }

        public void SaveOrnamentUndo()
        {
            if (Ornaments.UndoSaved)
                return;

            AddUndo(TChangeAction.ChangeEntireOrnament, 0, 0);
            ChangeList[ChangeCount - 1].OldParams.Params.OrnamentShownFrom = Ornaments.ShownFrom;
            ChangeList[ChangeCount - 1].OldParams.Params.OrnamentCursor = Ornaments.CursorInt;

            Array.Resize(ref ChangeOrnamentsList, ChangeOrnamentsList.Length + 1);

            int index = ChangeOrnamentsList.Length - 1;
            ChangeOrnamentsList[index] = new ChangeOrnament();

            ChangeOrnamentsList[index].Number = (int)SampleNumUpDown.Value;

            if (Ornaments.ShownOrnament != null)
            {
                ChangeOrnamentsList[index].OldOrnament.Length = Ornaments.ShownOrnament.Length;
                ChangeOrnamentsList[index].OldOrnament.Loop = Ornaments.ShownOrnament.Loop;
                ChangeOrnamentsList[index].OldOrnament.Offsets = Ornaments.ShownOrnament.Offsets;
            }
            else
            {
                ChangeOrnamentsList[index].OldOrnament.Length = 1;
                ChangeOrnamentsList[index].OldOrnament.Loop = 0;
            }

            Ornaments.UndoSaved = true;
        }

        public void SaveOrnamentRedo()
        {
            int index;

            if (!Ornaments.UndoSaved)
                return;

            ChangeList[ChangeCount - 1].NewParams.Params.OrnamentShownFrom = Ornaments.ShownFrom;
            ChangeList[ChangeCount - 1].NewParams.Params.OrnamentCursor = Ornaments.CursorInt;

            index = ChangeOrnamentsList.Length - 1;

            ValidateOrnament((int)OrnamentNumUpDown.Value);

            ChangeOrnamentsList[index].NewOrnament.Length = Ornaments.ShownOrnament.Length;
            ChangeOrnamentsList[index].NewOrnament.Loop = Ornaments.ShownOrnament.Loop;
            ChangeOrnamentsList[index].NewOrnament.Offsets = Ornaments.ShownOrnament.Offsets;

            ChangeList[ChangeCount - 1].ComParams.EntireOrnament = (ChangeOrnament)ChangeOrnamentsList[index].Clone();

            Ornaments.UndoSaved = false;
        }

        public void ShiftPositionsToRight(int fromPos, int numNewPositions)
        {
            // Shift positions and colors to the right
            for (int i = VTM.Positions.Length - 1; i >= fromPos; i--)
            {
                VTM.Positions.Value[i] = VTM.Positions.Value[i - numNewPositions];
                VTM.Positions.Colors[i] = VTM.Positions.Colors[i - numNewPositions];
            }
        }

        public void ShiftPositionsToLeft(int fromPos, int toPos)
        {
            int positionShift = fromPos - toPos;

            // Shift positions and colors to the left
            for (int i = toPos; i < VTM.Positions.Length - positionShift; i++)
            {
                VTM.Positions.Value[i] = VTM.Positions.Value[i + positionShift];
                VTM.Positions.Colors[i] = VTM.Positions.Colors[i + positionShift];
            }
        }

        public void InsertPosition(bool duplicate, bool makeUndo, bool changePosition)
        {
            int destCol;
            int numNewPositions;
            int newPatternNumber = 0;
            int lastPatternNumber;
            int patternLength;
            int[] savedPositions = new int[0];
            int[] savedPositionsColors = new int[0];

            // Shortcuts
            int selectLeft = PositionsGrid.Selection.Left;
            int selectRight = PositionsGrid.Selection.Right;
            int trackLength = VTM.Positions.Length;

            // Selected position > Track length?
            if (selectRight > trackLength)
                return;

            if (duplicate)
            {
                // Number of new positions is Right selection pos - Left selection pos + 1
                numNewPositions = selectRight - selectLeft + 1;
            }
            else
            {
                numNewPositions = 1;
                selectLeft = selectRight;
            }

            // Current track length + num new positions > Max track length?
            if (trackLength + numNewPositions - 1 > VTModule.MaxPositionIndex)
                return;

            // Save positions state for undo
            if (makeUndo && duplicate)
                SavePositionsUndo();

            if (makeUndo && !duplicate)
                SaveTrackUndo();

            // Check new pattern number
            if (!duplicate)
            {
                // Get new pattern number
                newPatternNumber = GetNewPatternNumber();

                // Is new pattern number > Max pattern number?
                if (newPatternNumber == -1)
                    return;
            }

            // Get length of last pattern
            if (selectLeft > 0)
                lastPatternNumber = VTM.Positions.Value[selectLeft - 1];
            else
                lastPatternNumber = VTM.Positions.Value[selectLeft];

            patternLength = VTM.Patterns[lastPatternNumber].Length;

            // Change pattern length
            if (!duplicate && makeUndo && changePosition)
                VTM.Patterns[newPatternNumber].Length = patternLength;

            // Index for new position(s) in positions array
            destCol = selectRight + 1;

            // Increase track length
            IncreaseTrackLength(numNewPositions);
            SongChanged = true;
            BackupSongChanged = true;

            // Save position values and colors if duplicate
            if (duplicate)
            {
                Array.Resize(ref savedPositions, numNewPositions);
                Array.Resize(ref savedPositionsColors, numNewPositions);

                for (int i = 0; i < savedPositions.Length; i++)
                {
                    savedPositions[i] = VTM.Positions.Value[selectLeft + i];
                    savedPositionsColors[i] = VTM.Positions.Colors[selectLeft + i];
                }
            }

            // Shift positions and colors to the right
            ShiftPositionsToRight(destCol, numNewPositions);

            // Shift loop
            ShiftLoopPosition(POS_COPY, selectLeft, destCol, numNewPositions);

            // Insert new positions OR duplicete selected positions/colors
            if (duplicate)
            {
                for (int i = 0; i < savedPositions.Length; i++)
                {
                    VTM.Positions.Value[destCol + i] = savedPositions[i];
                    VTM.Positions.Colors[destCol + i] = savedPositionsColors[i];
                }
            }
            else
            {
                VTM.Positions.Value[destCol] = newPatternNumber;
                VTM.Positions.Colors[destCol] = 0;
            }

            // Redraw PositionsGrid positions
            RedrawPatternPositions();

            // Select inserted positions
            if (changePosition)
                PositionMakeSelection((byte)destCol, (byte)(destCol + numNewPositions - 1));

            // Set positions scrollbar
            SetPositionsGridScroll(-1);

            // Recalculate track length
            CalcTotLen();
            InputPNumber = 0;

            // Set pattern editor cursor to the first line and on the channel A note.
            if (duplicate)
            {
                Tracks.ShownFrom = 0;
                Tracks.CursorX = 8;
                Tracks.CursorY = Tracks.CenterLineIndex;
            }

            // Save information for REDO
            if (makeUndo && duplicate)
                SavePositionsRedo();

            if (makeUndo && !duplicate)
                SaveTrackRedo();
        }

        public void ClonePositions()
        {
            int destCol;
            int numNewPositions;
            int[] savedPositions = new int[0];
            int[] savedPositionsColors = new int[0];
            int[] newPatternNumbers = new int[0];

            // Shortcuts
            int selectLeft = PositionsGrid.Selection.Left;
            int selectRight = PositionsGrid.Selection.Right;
            int trackLength = VTM.Positions.Length;

            // Selected position > Track length?
            if (selectRight > trackLength)
                return;

            // Num new positions
            numNewPositions = selectRight - selectLeft + 1;

            // Current track length + num new positions > Max track length?
            if (trackLength + numNewPositions - 1 > VTModule.MaxPositionIndex)
                return;

            // Save positions and patterns state for UNDO
            SaveTrackUndo();

            // Get new pattern numbers
            newPatternNumbers = GetNewPatternNumbers(numNewPositions);

            // Is one of new pattern numbers > Max pattern number?
            if (newPatternNumbers.Length == 0)
                return;

            // Index of new position(s) in positions array
            destCol = selectRight + 1;

            // Increase track length
            IncreaseTrackLength(numNewPositions);
            SongChanged = true;
            BackupSongChanged = true;

            // Save position values and colors
            Array.Resize(ref savedPositions, numNewPositions);
            Array.Resize(ref savedPositionsColors, numNewPositions);

            for (int i = 0; i < savedPositions.Length; i++)
            {
                savedPositions[i] = VTM.Positions.Value[selectLeft + i];
                savedPositionsColors[i] = VTM.Positions.Colors[selectLeft + i];
            }

            // Shift positions and colors to the right
            ShiftPositionsToRight(destCol, numNewPositions);

            // Shift loop
            ShiftLoopPosition(POS_COPY, selectLeft, destCol, numNewPositions);

            // Clone old patterns to new patterns
            for (int i = 0; i < newPatternNumbers.Length; i++)
            {
                CloneAndCopyPattern((byte)savedPositions[i], (byte)newPatternNumbers[i]);
                VTM.Positions.Value[destCol + i] = newPatternNumbers[i];
                VTM.Positions.Colors[destCol + i] = savedPositionsColors[i];
            }

            // Redraw PositionsGrid positions
            RedrawPatternPositions();

            // Select inserted positions
            PositionMakeSelection((byte)destCol, (byte)(destCol + numNewPositions - 1));

            // Set positions scroll
            SetPositionsGridScroll(-1);

            // Recalculate track length
            CalcTotLen();

            InputPNumber = 0;

            // Set pattern editor cursor to the first line and on the channel A note.
            Tracks.ShownFrom = 0;
            Tracks.CursorX = 8;
            Tracks.CursorY = Tracks.CenterLineIndex;

            // Save new patterns state for UNDO
            SaveTrackRedo();
        }

        public void DeletePositions()
        {
            // Shortcuts
            int selectLeft = PositionsGrid.Selection.Left;
            int selectRight = PositionsGrid.Selection.Right;
            int numSelected = selectRight - selectLeft + 1;
            int trackLength = VTM.Positions.Length;

            if (selectLeft < 0 || selectRight < 0 || trackLength == 1)
                return;

            // Prevent to delete all positions.
            // Leave the first position in this case.
            if (numSelected == trackLength)
            {
                selectLeft++;
                numSelected--;
            }

            // Save UNDO information
            SongChanged = true;
            BackupSongChanged = true;
            AddUndo(TChangeAction.DeletePosition, 0, 0);
            ChangeList[ChangeCount - 1].OldParams.Params.CurrentPosition = PositionIndex;
            ChangeList[ChangeCount - 1].OldGridSelection = PositionsGrid.Selection;
            ChangeList[ChangeCount - 1].OldParams.Params.CurrentPattern = PatternIndex;
            ChangeList[ChangeCount - 1].PositionList = new Position();
            ChangeList[ChangeCount - 1].PositionList = VTM.Positions;

            // Shift pattern positions
            ShiftPositionsToLeft(selectRight + 1, selectLeft);

            // Zerofill last positions
            for (int i = trackLength - 1; i >= trackLength - numSelected; i--)
            {
                VTM.Positions.Value[i] = 0;
                VTM.Positions.Colors[i] = 0;
            }

            // Decrease track length
            VTM.Positions.Length -= numSelected;

            // Shift loop
            ShiftLoopPosition(POS_DELETE, selectLeft, selectRight, numSelected);

            UnselectPositions();
            RedrawPatternPositions();
            CalcTotLen();
            InputPNumber = 0;

            // Change position and pattern
            if (selectLeft == trackLength)
                selectLeft--;

            SelectPosition2(selectLeft);

            // Set pattern editor cursor to the first line and on the channel A note.
            Tracks.ShownFrom = 0;
            Tracks.CursorX = 8;
            Tracks.CursorY = Tracks.CenterLineIndex;

            // Save grid selection for REDO
            SavePositionsRedo();
        }

        public void PositionsGrid_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                // '0' .. '9'
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    if (!(WaveOutAPI.IsPlaying && (AY.PlayMode == PlayModes.PlayModule) && (PlayingWindow[0] == this || (AY.ChipCount == 2 && PlayingWindow[1] == this))) && PositionsGrid.Selection.Left <= VTM.Positions.Length)
                    {
                        InputPNumber = InputPNumber * 10 + (int)e.KeyChar - (int)'0';

                        if (InputPNumber > VTModule.MaxPatternIndex)
                            InputPNumber = (int)e.KeyChar - (int)'0';

                        ChangePositionValue(PositionsGrid.Selection.Left, InputPNumber);
                        RedrawPatternPositions();
                        return;
                    }
                    break;
            }

            InputPNumber = 0;
        }

        /* public void PatternNumEditExit(object sender, EventArgs e)
        {
            PatternNumEdit.Text = (PatternNumUpDown.Value).ToString();
        } */

        public void InitStringGridMetrics()
        {
            Size charSize;
            int visibleColCount, colCount;

            PositionsGrid.RowCount = 1;
            PosArrowHShift = -1;
            StringGridTextHShift = 0;

            switch (MainForm.PositionSize)
            {
                case 0:
                    PositionsGrid.Font = new Font(PositionsGrid.Font.FontFamily, 8, PositionsGrid.Font.Style);
                    PosArrowSize = 5;
                    PosArrowVShift = 0;
                    PosArrowHShift = -2;
                    StringGridTextVShift = 0;
                    StringGridTextHShift = 1;
                    StringGridAddHeight = 0;
                    break;
                case 1:
                    PositionsGrid.Font = new Font(PositionsGrid.Font.FontFamily, 10, PositionsGrid.Font.Style);
                    PosArrowSize = 7;
                    StringGridTextVShift = -1;
                    StringGridAddHeight = -1;
                    PosArrowVShift = 1;
                    break;
                case 2:
                    PositionsGrid.Font = new Font(PositionsGrid.Font.FontFamily, 12, PositionsGrid.Font.Style);
                    PosArrowSize = 8;
                    StringGridTextVShift = 1;
                    StringGridAddHeight = 0;
                    PosArrowVShift = 1;
                    break;
                case 3:
                    PositionsGrid.Font = new Font(PositionsGrid.Font.FontFamily, 14, PositionsGrid.Font.Style);
                    PosArrowSize = 9;
                    StringGridTextVShift = 0;
                    StringGridAddHeight = 0;
                    StringGridTextHShift = 1;
                    PosArrowVShift = 1;
                    PosArrowHShift = -2;
                    break;
                case 4:
                    PositionsGrid.Font = new Font(PositionsGrid.Font.FontFamily, 16, PositionsGrid.Font.Style);
                    PosArrowSize = 9;
                    StringGridTextVShift = 0;
                    StringGridAddHeight = -1;
                    PosArrowVShift = 1;
                    break;
                case 5:
                    PositionsGrid.Font = new Font(PositionsGrid.Font.FontFamily, 18, PositionsGrid.Font.Style);
                    PosArrowSize = 12;
                    StringGridTextVShift = 1;
                    StringGridAddHeight = 2;
                    PosArrowVShift = 2;
                    break;
            }

            using (Graphics g = Graphics.FromHwnd(PositionsGrid.Handle))
                charSize = TextRenderer.MeasureText(g, "0", PositionsGrid.Font, Size.Empty, TextFormatFlags.NoPadding);

            StringGridCharWidth = charSize.Width;
            StringGridCharHeight = charSize.Height;

            PositionsGrid.Columns[0].Width = (StringGridCharWidth * 3) + 4;
            int rowPadding = (int)Math.Round(13 * (DeviceDpi / 96.0));
            PositionsGrid.Rows[0].Height = StringGridCharHeight + rowPadding + StringGridAddHeight;
            PositionsGrid.Height = PositionsGrid.Rows[0].Height + SystemInformation.HorizontalScrollBarHeight;

            visibleColCount = PositionsGrid.Width / PositionsGrid.Columns[0].Width;
            colCount = VTM.Positions.Length;

            // Add empty cells
            if (colCount >= visibleColCount)
                colCount++;

            if (colCount < visibleColCount)
                colCount = visibleColCount;

            if (((PositionsGrid.Columns[0].Width + 1) * colCount) < PositionsGrid.ClientSize.Width)
                colCount++;

            PositionsGrid.ColumnCount = colCount;

            for (int i = 0; i < PositionsGrid.ColumnCount; i++)
            {
                var cell = PositionsGrid.Rows[0].Cells[i];
                if (cell.Value == null || cell.Value == DBNull.Value)
                    cell.Value = "...";

                PositionsGrid.Columns[i].Width = PositionsGrid.Columns[0].Width;
            }

            //PositionsGrid.Width = (PositionsGrid.Columns[0].Width + 1) * colCount - 1;
            //PositionsGrid.AutoScrollOffset = false;
            //PositionsScrollBox.HorizontalScroll.Maximum = PositionsGrid.Width + 1;
            //PositionsScrollBox.Height = PositionsGrid.Rows[0].Height + MainForm.HScrollbarSize + 5;
        }

        public void PositionsGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (VTM == null || e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var grid = sender as MyDataGridView;
            bool isSelected = e.State.HasFlag(DataGridViewElementStates.Selected);
            var g = e.Graphics;
            var rect = e.CellBounds;
            var cellValue = grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";

            Color backColor = ColorThemes.StringToColor(ColorThemes.GridColors[VTM.Positions.Colors[e.ColumnIndex]]);
            bool lightBg = (VTM.Positions.Colors[e.ColumnIndex] > 8 || VTM.Positions.Colors[e.ColumnIndex] == 0) && !isSelected;
            Color textColor = lightBg ? Color.Black : Color.White;

            if (isSelected)
            {
                textColor = Color.White;
                backColor = Color.FromArgb(unchecked((int)0xFF062142));
            }

            g.SetClip(e.ClipBounds);

            using (Brush backBrush = new SolidBrush(backColor))
                g.FillRectangle(backBrush, rect);

            // Process cell content
            bool isLoopCell = false;

            if (!string.IsNullOrEmpty(cellValue) && cellValue.StartsWith("L"))
            {
                isLoopCell = true;
                cellValue = cellValue.Substring(1);
            }

            var cellFont = grid.Font;
            var centerX = rect.Left + rect.Width / 2;
            var topY = rect.Top + 5;

            if (cellValue == "...")
            {
                using (Brush dotBrush = new SolidBrush(textColor))
                {
                    Rectangle dot = new Rectangle(centerX - 7, rect.Top + rect.Height - 10, 2, 2);
                    g.FillRectangle(dotBrush, dot);
                    dot.X += 6;
                    g.FillRectangle(dotBrush, dot);
                    dot.X += 6;
                    g.FillRectangle(dotBrush, dot);
                }
            }
            else
            {
                TextRenderer.DrawText(g, cellValue, cellFont, rect, textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            }

            // Draw loop arrow
            if (isLoopCell)
            {
                if (MainForm.TryGetFontFamily("Arrows", out FontFamily fontFamily))
                {
                    using (Font arrowFont = new Font(fontFamily, PosArrowSize, FontStyle.Regular))
                    //using (Brush arrowBrush = new SolidBrush(lightBg ? Color.FromArgb(unchecked((int)0xFFA40D0D)) : textColor))
                    {
                        Point arrowPoint = new Point(centerX - (StringGridCharWidth / 2) + PosArrowHShift, rect.Top);
                        //g.DrawString("4", arrowFont, arrowBrush, arrowPoint);
                        TextRenderer.DrawText(g, "4", arrowFont, arrowPoint, lightBg ? Color.FromArgb(unchecked((int)0xFFA40D0D)) : textColor, TextFormatFlags.NoPadding);
                    }
                }
            }

            // Draw current cell triangle if selected and in the current position
            //if (isSelected && e.ColumnIndex == PositionNumber)
            if (e.ColumnIndex == PositionIndex)
            {
                if (MainForm.TryGetFontFamily("Arrows", out FontFamily fontFamily))
                {
                    using (Font arrowFont = new Font(fontFamily, PosArrowSize, FontStyle.Regular))
                    //using (Brush arrowBrush = new SolidBrush(textColor))
                    {
                        Point arrowPoint = new Point(centerX - (StringGridCharWidth / 2) + PosArrowHShift,
                                                     rect.Bottom - PosArrowSize + PosArrowVShift - 2);
                        //g.DrawString("3", arrowFont, arrowBrush, arrowPoint);
                        TextRenderer.DrawText(g, "3", arrowFont, arrowPoint, textColor, TextFormatFlags.NoPadding);
                    }
                }
            }

            // Draw cell borders
            using (Pen gridLinePen = new Pen(Color.FromArgb(unchecked((int)0xFFC0C0C0))))
            {
                g.DrawLine(gridLinePen, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom);
                g.DrawLine(gridLinePen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
            }

            e.Handled = true;
        }


        public void PositionsGrid_MouseUp(object sender, MouseEventArgs e)
        {
            // If selected range is bigger than track length
            if (PositionsGrid.Selection.Right > PositionsGrid.Selection.Left && PositionsGrid.Selection.Right >= VTM.Positions.Length)
            {
                SelectPosition2(VTM.Positions.Length - 1);
                PositionsGrid.Selection = Rectangle.FromLTRB(PositionsGrid.Selection.Top, PositionsGrid.Selection.Left, PositionsGrid.Selection.Left, PositionsGrid.Selection.Bottom);
                PatternsOrderSelection = PositionsGrid.Selection;
            }

            // Prevent to change position if playing only current pattern
            if (WaveOutAPI.IsPlaying && AY.PlayMode == PlayModes.PlayPattern)
                PositionsGrid.Selection = Rectangle.FromLTRB(PositionIndex, 0, PositionIndex, 0);
        }

        public void PositionsGrid_MouseDown(object sender, MouseEventArgs e)
        {
            int x1, y1;
            int sourceCol;
            bool isShiftDown = (GetAsyncKeyState(Keys.ShiftKey) & 0x8000) != 0;
            bool isLeftDown = (e.Button & MouseButtons.Left) != 0;
            bool isRightDown = (e.Button & MouseButtons.Right) != 0;

            if (isRightDown)
            {
                if (PositionsGrid.Selection.Left == PositionsGrid.Selection.Right && !WaveOutAPI.IsPlaying && AY.PlayMode != PlayModes.PlayPattern)
                {
                    Globals.MouseToCell(PositionsGrid, e.X, e.Y, out x1, out y1);
                    PositionsGrid.Selection = Rectangle.FromLTRB(x1, y1, x1, y1);
                }

                PatternsOrderSelection = PositionsGrid.Selection;
            }

            // Prevent to change position if playing only current pattern
            if (WaveOutAPI.IsPlaying && AY.PlayMode == PlayModes.PlayPattern)
                PositionsGrid.Selection = Rectangle.FromLTRB(PositionIndex, 0, PositionIndex, 0);

            if (isLeftDown)
            {
                // If track is playing, then do nothing
                if (WaveOutAPI.IsPlaying && (AY.PlayMode == PlayModes.PlayModule || AY.PlayMode == PlayModes.PlayPattern))
                    return;

                // If empty pattern selected
                if (PositionsGrid.Selection.Right >= VTM.Positions.Length)
                    return;

                // If Shift Key is pressed, then save selection range in special variable
                if (isShiftDown)
                    PatternsOrderSelection = PositionsGrid.Selection;

                Globals.MouseToCell(PositionsGrid, e.X, e.Y, out sourceCol, out y1);

                if (sourceCol >= 0)
                {
                    string draggedText = PositionsGrid.Rows[y1].Cells[sourceCol].Value.ToString();

                    // Start the drag-and-drop operation
                    PositionsGrid.DoDragDrop(draggedText, DragDropEffects.Move);
                }
            }
        }

        public void PositionMakeSelection(byte fromPos, byte toPos)
        {
            Rectangle selectionRect = Rectangle.FromLTRB(fromPos, 0, toPos, 0);
            PositionsGrid.Selection = selectionRect;
            PatternsOrderSelection = PositionsGrid.Selection;
            PatternIndex = VTM.Positions.Value[selectionRect.Left];
            PositionIndex = selectionRect.Left;
            ChangePattern(PatternIndex);
            PatternNumUpDown.Value = PatternIndex;
        }

        public void CloneAndCopyPattern(byte srcPatternIndex, byte newPatternIndex)
        {
            if (newPatternIndex > VTModule.MaxPatternIndex)
                return;

            // Create pattern and set length
            ValidatePattern2(newPatternIndex);
            VTM.Patterns[newPatternIndex].Length = VTM.Patterns[srcPatternIndex].Length;
            CheckTracksAfterSizeChanged(newPatternIndex); // Copy pattern data from src to dest by Track Manager
            Globals.TracksManagerForm.EnvelopeColumn.Checked = true; // Flag: copy envelope data ON
            Globals.TracksManagerForm.NoiseColumn.Checked = true; // Flag: copy noise data ON
            Globals.TracksManagerForm.TracksOp(srcPatternIndex, 0, 0, newPatternIndex, 0, 0, 0, false); // Copy chan A
            Globals.TracksManagerForm.TracksOp(srcPatternIndex, 0, 1, newPatternIndex, 0, 1, 0, false); // Copy chan B
            Globals.TracksManagerForm.TracksOp(srcPatternIndex, 0, 2, newPatternIndex, 0, 2, 0, false); // Copy chan C
            Globals.TracksManagerForm.EnvelopeColumn.Checked = false;
            Globals.TracksManagerForm.NoiseColumn.Checked = false;
        }

        public void PositionsGridDragDrop_MoveItemsFromLeftToRight()
        {
            // Shift columns to left
            for (int i = SourceCol; i <= DestCol - NumSelectedCols; i++)
            {
                VTM.Positions.Value[i] = VTM.Positions.Value[i + NumSelectedCols];
                VTM.Positions.Colors[i] = VTM.Positions.Colors[i + NumSelectedCols];
            }

            // Copy stored selected columns to dest columns
            for (int i = 0; i < NumSelectedCols; i++)
            {
                VTM.Positions.Value[DestCol + 1 - NumSelectedCols + i] = SourceColsContent[i];
                VTM.Positions.Colors[DestCol + 1 - NumSelectedCols + i] = SourceColsColors[i];
            }

            PositionMakeSelection((byte)(DestCol - NumSelectedCols + 1), (byte)DestCol);
        }

        public void PositionsGridDragDrop_MoveItemsFromLeftToRight2()
        {
            // Destination column = End of soundtrack
            DestCol = VTM.Positions.Length;

            // Shift columns to left
            for (int i = SourceCol; i <= DestCol - NumSelectedCols; i++)
            {
                VTM.Positions.Value[i] = VTM.Positions.Value[i + NumSelectedCols];
                VTM.Positions.Colors[i] = VTM.Positions.Colors[i + NumSelectedCols];
            }

            // Copy stored selected columns to dest columns
            for (int i = 0; i < NumSelectedCols; i++)
            {
                VTM.Positions.Value[DestCol - NumSelectedCols + i] = SourceColsContent[i];
                VTM.Positions.Colors[DestCol - NumSelectedCols + i] = SourceColsColors[i];
            }

            PositionMakeSelection((byte)(DestCol - NumSelectedCols), (byte)(DestCol - 1));
        }

        public void PositionsGridDragDrop_MoveItemsFromRightToLeft()
        {
            // Shift columns to right
            for (int i = SourceCol + NumSelectedCols - 1; i > DestCol + NumSelectedCols; i--)
            {
                VTM.Positions.Value[i] = VTM.Positions.Value[i - NumSelectedCols];
                VTM.Positions.Colors[i] = VTM.Positions.Colors[i - NumSelectedCols];
            }

            // Copy stored selected columns to dest columns
            for (int i = 0; i < NumSelectedCols; i++)
            {
                VTM.Positions.Value[DestCol + i] = SourceColsContent[i];
                VTM.Positions.Colors[DestCol + i] = SourceColsColors[i];
            }

            PositionMakeSelection((byte)DestCol, (byte)(DestCol + NumSelectedCols - 1));
        }

        public void PositionsGridDragDrop_CopyItemsFromLeftToRight()
        {
            // Increase track length
            IncreaseTrackLength(NumSelectedCols);

            // Shift patterns to right FROM DestCol to end
            ShiftPositionsToRight(DestCol + 1, NumSelectedCols);

            // Copy selected patterns to DestCol
            for (int i = 0; i < NumSelectedCols; i++)
            {
                VTM.Positions.Value[DestCol + 1 + i] = SourceColsContent[i];
                VTM.Positions.Colors[DestCol + 1 + i] = SourceColsColors[i];
            }

            PositionMakeSelection((byte)(DestCol + 1), (byte)(DestCol + NumSelectedCols));
        }

        public void PositionsGridDragDrop_CopyItemsFromLeftToRight2()
        {
            // Destination column = End of soundtrack
            DestCol = VTM.Positions.Length;

            // Increase soundtrack length
            IncreaseTrackLength(NumSelectedCols);

            // Copy selected patterns to DestCol
            for (int i = 0; i < NumSelectedCols; i++)
            {
                VTM.Positions.Value[DestCol + i] = SourceColsContent[i];
                VTM.Positions.Colors[DestCol + i] = SourceColsColors[i];
            }

            PositionMakeSelection((byte)DestCol, (byte)(DestCol + NumSelectedCols - 1));
        }

        public void PositionsGridDragDrop_CopyItemsFromRightToLeft()
        {
            // Increase soundtrack length
            IncreaseTrackLength(NumSelectedCols);

            // Shift columns to right
            ShiftPositionsToRight(DestCol + NumSelectedCols, NumSelectedCols);

            // Copy selected patterns to DestCol
            for (int i = 0; i < NumSelectedCols; i++)
            {
                VTM.Positions.Value[DestCol + i] = SourceColsContent[i];
                VTM.Positions.Colors[DestCol + i] = SourceColsColors[i];
            }

            PositionMakeSelection((byte)DestCol, (byte)(DestCol + NumSelectedCols - 1));
        }

        public void PositionsGridDragDrop_CloneItemsFromLeftToRight()
        {
            int[] newPatternNumbers = GetNewPatternNumbers(NumSelectedCols);

            if (newPatternNumbers.Length == 0)
                return;

            // Increase track length
            IncreaseTrackLength(NumSelectedCols);

            // Shift patterns to right FROM DestCol to end
            ShiftPositionsToRight(DestCol + 1, NumSelectedCols);

            // Clone selected patterns to DestCol
            for (int i = 0; i < NumSelectedCols; i++)
            {
                CloneAndCopyPattern((byte)SourceColsContent[i], (byte)newPatternNumbers[i]);
                VTM.Positions.Value[DestCol + 1 + i] = newPatternNumbers[i];
                VTM.Positions.Colors[DestCol + 1 + i] = SourceColsColors[i];
            }

            PositionMakeSelection((byte)(DestCol + 1), (byte)(DestCol + NumSelectedCols));
        }

        public void PositionsGridDragDrop_CloneItemsFromLeftToRight2()
        {
            int[] newPatternNumbers = GetNewPatternNumbers(NumSelectedCols);

            if (newPatternNumbers.Length == 0)
                return;

            // Destination column = End of soundtrack
            DestCol = VTM.Positions.Length;

            // Increase track length
            IncreaseTrackLength(NumSelectedCols);

            // Clone selected patterns to DestCol
            for (int i = 0; i < NumSelectedCols; i++)
            {
                CloneAndCopyPattern((byte)SourceColsContent[i], (byte)newPatternNumbers[i]);
                VTM.Positions.Value[DestCol + i] = newPatternNumbers[i];
                VTM.Positions.Colors[DestCol + i] = SourceColsColors[i];
            }

            PositionMakeSelection((byte)DestCol, (byte)(DestCol + NumSelectedCols - 1));
        }

        public void PositionsGridDragDrop_CloneItemsFromRightToLeft()
        {
            int[] newPatternNumbers = GetNewPatternNumbers(NumSelectedCols);

            if (newPatternNumbers.Length == 0)
                return;

            // Increase soundtrack length
            IncreaseTrackLength(NumSelectedCols);

            // Shift columns to right
            ShiftPositionsToRight(DestCol + NumSelectedCols, NumSelectedCols);

            // Clone selected patterns to DestCol
            for (int i = 0; i < NumSelectedCols; i++)
            {
                CloneAndCopyPattern((byte)SourceColsContent[i], (byte)newPatternNumbers[i]);
                VTM.Positions.Value[DestCol + i] = newPatternNumbers[i];
                VTM.Positions.Colors[DestCol + i] = SourceColsColors[i];
            }

            PositionMakeSelection((byte)DestCol, (byte)(DestCol + NumSelectedCols - 1));
        }

        public void PositionsGrid_DragDrop(object sender, DragEventArgs e)
        {
            Globals.MouseToCell(PositionsGrid, e.X, e.Y, out DestCol, out DestRow);
            SourceCol = PositionsGrid.Selection.Left; // Left index of selection
            SourceColEnd = PositionsGrid.Selection.Right; // Right index of selection
            NumSelectedCols = PositionsGrid.Selection.Right - PositionsGrid.Selection.Left + 1;
            bool isShiftDown = (GetAsyncKeyState(Keys.ShiftKey) & 0x8000) != 0;
            bool isCtrlDown = (GetAsyncKeyState(Keys.ControlKey) & 0x8000) != 0;

            // Is Shift key pressed?
            TrackLength = VTM.Positions.Length;

            // Do nothing if drag to the same column
            if (SourceCol == DestCol)
                return;

            // Detect operation
            if (isShiftDown || isCtrlDown)
                OperationType = POS_COPY;
            else
                OperationType = POS_MOVE;

            // Save information for UNDO operation
            if (!isShiftDown)
            {
                // save positions state only (move and copy)
                SavePositionsUndo();
            }
            else
            {
                SaveTrackUndo();
            }

            // save positions and patterns (clone)
            // Set length of arrays for store selected columns
            SourceColsContent = new int[NumSelectedCols];
            SourceColsColors = new int[NumSelectedCols];

            // Save values of selected columns and colors
            for (int i = 0; i < NumSelectedCols; i++)
            {
                SourceColsContent[i] = VTM.Positions.Value[SourceCol + i];
                SourceColsColors[i] = VTM.Positions.Colors[SourceCol + i];
            }

            // Move patterns
            // If user drag items from left to right
            if (SourceColEnd < DestCol && DestCol < TrackLength && !isCtrlDown && !isShiftDown)
                PositionsGridDragDrop_MoveItemsFromLeftToRight();

            // MOVE patterns
            // If user drag items to the end of track
            if (SourceColEnd < DestCol && DestCol >= TrackLength && !isCtrlDown && !isShiftDown)
                PositionsGridDragDrop_MoveItemsFromLeftToRight2();

            // MOVE patterns
            // If user drags items from right to left
            if (SourceCol > DestCol && !isCtrlDown && !isShiftDown)
                PositionsGridDragDrop_MoveItemsFromRightToLeft();

            // COPY patterns
            // If user drag items from left to right AND press Ctrl key
            if (SourceColEnd < DestCol && DestCol < TrackLength && isCtrlDown && !isShiftDown)
                PositionsGridDragDrop_CopyItemsFromLeftToRight();

            // COPY patterns
            // If user drag items from left to the end of track AND press Ctrl key
            if (SourceColEnd < DestCol && DestCol >= TrackLength && isCtrlDown && !isShiftDown)
                PositionsGridDragDrop_CopyItemsFromLeftToRight2();

            // COPY patterns
            // If user drag items from right to left AND press Ctrl key
            if (SourceCol > DestCol && isCtrlDown && !isShiftDown)
                PositionsGridDragDrop_CopyItemsFromRightToLeft();

            // CLONE patterns
            // If user drag items from left to right AND press Shift key
            if (SourceColEnd < DestCol && DestCol < TrackLength && isShiftDown && !isCtrlDown)
                PositionsGridDragDrop_CloneItemsFromLeftToRight();

            // CLONE patterns
            // If user drag items from left to the end of track AND press Shift key
            if (SourceColEnd < DestCol && DestCol >= TrackLength && isShiftDown && !isCtrlDown)
                PositionsGridDragDrop_CloneItemsFromLeftToRight2();

            // CLONE patterns
            // If user drag items from right to left AND press Shift key
            if (SourceCol > DestCol && isShiftDown && !isCtrlDown)
                PositionsGridDragDrop_CloneItemsFromRightToLeft();

            // Shift loop position
            ShiftLoopPosition(OperationType, SourceCol, DestCol, NumSelectedCols);

            // Redraw stringgrid with pattern positions
            RedrawPatternPositions();

            // Set pattern editor cursor to the first line and on the channel A note.
            if (OperationType == POS_COPY)
            {
                Tracks.ShownFrom = 0;
                Tracks.CursorX = 8;
                Tracks.CursorY = Tracks.CenterLineIndex;
            }

            // Recalculate total track length
            CalcTotLen();
            InputPNumber = 0;

            // If clone, then save new patterns state for REDO
            if (isShiftDown)
                SaveTrackRedo();
            else
                SavePositionsRedo();
        }

        public void PositionsGrid_DragOver(object sender, DragEventArgs e)
        {
            Globals.MouseToCell(PositionsGrid, e.X, e.Y, out int currentCol, out int currentRow);
            object dragSource = (object)e.Data.GetData("DragSource");

            bool accept = sender == dragSource && currentCol >= 0;

            if (accept)
            {
                if (PatternsOrderSelection.Right != PatternsOrderSelection.Left)
                    PositionsGrid.Selection = PatternsOrderSelection;

                if (GetKeyState(Keys.Control) < 0)
                    PositionsGrid.Cursor = Cursors.Default;
                else if (GetKeyState(Keys.Shift) < 0)
                    PositionsGrid.Cursor = Cursors.UpArrow;
                else
                    PositionsGrid.Cursor = Cursors.Default;
            }
        }

        public void PositionsGrid_DragLeave(object sender, EventArgs e)
        {
            PatternsOrderSelection = PositionsGrid.Selection;
        }

        public void TitleTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!InitFinished)
                return;

            SongChanged = true;
            BackupSongChanged = true;
            string text = TitleTextBox.Text;
            AddUndo(TChangeAction.ChangeTitle, VTM.Title, text);
            VTM.Title = text;
        }

        public void AuthorTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!InitFinished)
                return;

            SongChanged = true;
            BackupSongChanged = true;
            string text = AuthorTextBox.Text;
            AddUndo(TChangeAction.ChangeAuthor, VTM.Author, text);
            VTM.Author = text;
        }

        public void ChangePattern(int patternNumber)
        {
            PatternIndex = patternNumber;
            Tracks.ShownPattern = VTM.Patterns[PatternIndex];
            int patternLength = VTM.Patterns[PatternIndex] == null ? VTModule.DefaultPatternLength : VTM.Patterns[PatternIndex].Length;
            PatternLenUpDown.Value = patternLength;

            if (AutoHLCheckBox.Checked)
                CalcHLStep();

            Tracks.ShownFrom = 0;

            if (Tracks.Focused)
                Tracks.HideCaret();

            if (Tracks.CursorY > patternLength - 1 + Tracks.CenterLineIndex)
            {
                Tracks.CursorY = patternLength - 1 + Tracks.CenterLineIndex;

                if (Tracks.Focused)
                    Tracks.SetCaretPosition();
            }
            else if (Tracks.CursorY < Tracks.CenterLineIndex)
            {
                Tracks.CursorY = Tracks.CenterLineIndex;

                if (Tracks.Focused)
                    Tracks.SetCaretPosition();
            }

            Tracks.RemoveSelection();

            Tracks.RedrawTracks();

            if (Tracks.Focused)
                Tracks.ShowCaret();

            if (Globals.MainForm.ActiveMdiChild == this)
                SetToolsPattern();
        }

        public void PatternNumUpDown_ValueChanging(object sender, ValueChangingEventArgs e)
        {
            if (DisableChangingEx)
                return;

            e.Cancel = e.NewValue < 0 || e.NewValue > VTModule.MaxPatternIndex;

            if (e.Cancel)
                return;

            ChangePattern((int)e.NewValue);
        }

        public void PatternNumEditChange(object sender, EventArgs e)
        {
            if (DisableChangingEx)
                return;

            if (PatternIndex != PatternNumUpDown.Value)
                ChangePattern((int)PatternNumUpDown.Value);
        }

        public string GetSpeedBPMString(short trackSpeed)
        {
            string result;
            string trackSpeedStr = trackSpeed.ToString();
            string bpmStr = ((int)Math.Round((WaveOutAPI.InterruptFreq * 60.0) / (trackSpeed * 4.0 * 1000.0))).ToString();

            if (trackSpeedStr.Length + bpmStr.Length <= 4)
                result = $"{trackSpeedStr} / {bpmStr}";
            else
                result = $"{trackSpeedStr}/{bpmStr}";

            return result;
        }

        public void UpdateSpeedBPM()
        {
            SpeedBpmUpDown.Text = GetSpeedBPMString((short)SpeedBpmUpDown.Value);
        }

        public void SpeedBpmUpDown_ValueChanging(object sender, ValueChangingEventArgs e)
        {
            e.Cancel = e.NewValue < 1 || e.NewValue > 255;

            if (e.Cancel)
                return;

            SetInitDelay((int)e.NewValue);

            SpeedBpmUpDown.Text = GetSpeedBPMString((short)e.NewValue);

            // if (!SpeedBpmEdit.Focused)
            //     SpeedBpmEdit.Text = GetSpeedBPMString((short)e.NewValue);
        }

        public void ToneTableUpDown_ValueChanging(object sender, ValueChangingEventArgs e)
        {
            e.Cancel = e.NewValue < 0 || e.NewValue > 4;

            if (e.Cancel)
                return;

            if ((int)VTM.NoteTable == e.NewValue)
                return;

            ChangeEnvelopeWhenToneTableChanged(VTM, VTM.NoteTable, (NoteTableType)e.NewValue);
            SongChanged = true;
            BackupSongChanged = true;
            AddUndo(TChangeAction.ChangeToneTable, VTM.NoteTable, e.NewValue);
            VTM.NoteTable = (NoteTableType)e.NewValue;
            UpdateToneTableHints();

            Tracks.RedrawTracks();

            if (Globals.MainForm.ActiveMdiChild == this)
                SetToolsPattern();

            if (BlockRecursion)
                return;

            if (TSWindow[0] != null)
            {
                TSWindow[0].BlockRecursion = true;
                TSWindow[0].ToneTableUpDown.Value = e.NewValue;
                TSWindow[0].BlockRecursion = false;
            }

            if (TSWindow[1] != null)
            {
                TSWindow[1].BlockRecursion = true;
                TSWindow[1].ToneTableUpDown.Value = e.NewValue;
                TSWindow[1].BlockRecursion = false;
            }
        }

        public void PatternLenUpDown_ValueChanging(object sender, ValueChangingEventArgs e)
        {
            if (DisableChangingEx)
                return;

            e.Cancel = e.NewValue < 1 || e.NewValue > VTModule.MaxPatternLength;

            if (e.Cancel)
                return;

            //PatternLenEdit.Text = MainForm.DecBaseLinesOn ? e.NewValue.ToString() : e.NewValue.ToString("X");
            ChangePatternLength((int)e.NewValue);
        }

        public void CheckTracksAfterSizeChanged(int newLength)
        {
            if (AutoHLCheckBox.Checked)
                CalcHLStep();

            if (!UndoWorking)
            {
                if (Tracks.ShownFrom >= newLength)
                    Tracks.ShownFrom = newLength - 1;

                if (Tracks.Focused)
                    Tracks.HideCaret();

                if (Tracks.CursorY > newLength - Tracks.ShownFrom - 1 + Tracks.CenterLineIndex)
                {
                    Tracks.CursorY = newLength - Tracks.ShownFrom - 1 + Tracks.CenterLineIndex;

                    if (Tracks.Focused)
                        Tracks.SetCaretPosition();
                }

                Tracks.RemoveSelection();
                Tracks.RedrawTracks();

                if (Tracks.Focused)
                    Tracks.ShowCaret();

                ChangeList[ChangeCount - 1].NewParams.Params.PatternShownFrom = Tracks.ShownFrom;
                ChangeList[ChangeCount - 1].NewParams.Params.PatternCursorY = Tracks.CursorY;
            }

            CalcTotLen();
            CalculatePos0();
        }

        public void ChangePatternLength(int newLength)
        {
            ValidatePattern2(PatternIndex);

            if (newLength != VTM.Patterns[PatternIndex].Length)
            {
                SongChanged = true;
                BackupSongChanged = true;
                AddUndo(TChangeAction.ChangePatternSize, VTM.Patterns[PatternIndex].Length, newLength);
                VTM.Patterns[PatternIndex].Length = newLength;
                CheckTracksAfterSizeChanged(newLength);
            }
        }

        private readonly Color _red2 = Color.FromArgb(0x80, 0x80, 0xFF);
        private readonly Color _yellow2 = Color.FromArgb(0x00, 0xE0, 0xFF);

        private void SetButColor(CheckBox checkBox, Color color)
        {
            checkBox.BackColor = color;
            checkBox.ForeColor = (color == SystemColors.Control || color == _yellow2)
                ? SystemColors.WindowText
                : SystemColors.ActiveCaptionText;
        }

        public void UpdateHintsForChannelButtons()
        {
            const string MuteChannel = "Mute Channel";
            const string MuteTone = "Mute Tone";
            const string MuteNoise = "Mute Noise";
            const string MuteEnvelope = "Mute Envelope";
            const string UnmuteChannel = "Unmute Channel";
            const string UnmuteTone = "Unmute Tone";
            const string UnmuteNoise = "Unmute Noise";
            const string UnmuteEnvelope = "Unmute Envelope";
            const string SoloChannel = "Solo Channel";
            const string UnsoloChannel = "Unsolo Channel";

            // Channel A button
            ToolTip.SetToolTip(ChannelAMute, ChannelAMute.Checked ? UnmuteChannel : MuteChannel);
            ToolTip.SetToolTip(ChannelATone, ChannelATone.Checked ? UnmuteTone : MuteTone);
            ToolTip.SetToolTip(ChannelANoise, ChannelANoise.Checked ? UnmuteNoise : MuteNoise);
            ToolTip.SetToolTip(ChannelAEnvelope, ChannelAEnvelope.Checked ? UnmuteEnvelope : MuteEnvelope);
            ToolTip.SetToolTip(ChannelASolo, ChannelASolo.Checked ? UnsoloChannel : SoloChannel);

            // Channel B buttons
            ToolTip.SetToolTip(ChannelBMute, ChannelBMute.Checked ? UnmuteChannel : MuteChannel);
            ToolTip.SetToolTip(ChannelBTone, ChannelBTone.Checked ? UnmuteTone : MuteTone);
            ToolTip.SetToolTip(ChannelBNoise, ChannelBNoise.Checked ? UnmuteNoise : MuteNoise);
            ToolTip.SetToolTip(ChannelBEnvelope, ChannelBEnvelope.Checked ? UnmuteEnvelope : MuteEnvelope);
            ToolTip.SetToolTip(ChannelBSolo, ChannelBSolo.Checked ? UnsoloChannel : SoloChannel);

            // Channel C buttons
            ToolTip.SetToolTip(ChannelCMute, ChannelCMute.Checked ? UnmuteChannel : MuteChannel);
            ToolTip.SetToolTip(ChannelCTone, ChannelCTone.Checked ? UnmuteTone : MuteTone);
            ToolTip.SetToolTip(ChannelCNoise, ChannelCNoise.Checked ? UnmuteNoise : MuteNoise);
            ToolTip.SetToolTip(ChannelCEnvelope, ChannelCEnvelope.Checked ? UnmuteEnvelope : MuteEnvelope);
            ToolTip.SetToolTip(ChannelCSolo, ChannelCSolo.Checked ? UnsoloChannel : SoloChannel);
        }

        public void UpdateChannelsState()
        {
            ApplyChannelsButtons();

            Tracks.HideCaret();
            Tracks.RedrawTracks();
            Tracks.ShowCaret();

            if (TSWindow[0] != null)
            {
                TSWindow[0].ApplyChannelsButtons();
                TSWindow[0].Tracks.HideCaret();
                TSWindow[0].Tracks.RedrawTracks();
                TSWindow[0].Tracks.ShowCaret();
            }

            if (TSWindow[1] != null)
            {
                TSWindow[1].ApplyChannelsButtons();
                TSWindow[1].Tracks.HideCaret();
                TSWindow[1].Tracks.RedrawTracks();
                TSWindow[1].Tracks.ShowCaret();
            }

            //StopAndRestart();
        }

        public bool CheckMute()
        {
            for (int j = 0; j < 3; j++)
            {
                var curWin = this;

                switch (j)
                {
                    case 1:
                        curWin = TSWindow[0];
                        break;
                    case 2:
                        curWin = TSWindow[1];
                        break;
                }

                if (curWin == null)
                    continue;

                for (int i = 0; i < 3; i++)
                {
                    var b = curWin.ChanButtons[i];
                    if (b.Mute_But_s == 1 || b.T_But_s == 1 || b.N_But_s == 1 || b.E_But_s == 1)
                        return true;
                }
            }
            return false;
        }

        public bool CheckSolo()
        {
            for (int j = 0; j < 3; j++)
            {
                var curWin = this;

                switch (j)
                {
                    case 1:
                        curWin = TSWindow[0];
                        break;
                    case 2:
                        curWin = TSWindow[1];
                        break;
                }

                if (curWin == null)
                    continue;

                for (int i = 0; i < 3; i++)
                {
                    if (curWin.ChanButtons[i].Solo_But_s != 0)
                        return true;
                }
            }

            return false;
        }

        public void ApplyChannelsButtons()
        {
            for (int i = 0; i < 3; i++)
            {
                var button = ChanButtons[i];

                VTM.ChannelStates[i].GlobalTone = button.T_But_s == 0;

                var color = SystemColors.Control;

                switch (button.T_But_s)
                {
                    case 1:
                        color = Color.Red;
                        break;
                    case 2:
                        color = _red2;
                        break;
                }

                SetButColor(button.T_But, color);

                VTM.ChannelStates[i].GlobalNoise = button.N_But_s == 0;

                switch (button.T_But_s)
                {
                    case 1:
                        color = Color.Red;
                        break;
                    case 2:
                        color = _red2;
                        break;
                }

                SetButColor(button.N_But, color);

                VTM.ChannelStates[i].GlobalEnvelope = button.E_But_s == 0;

                switch (button.T_But_s)
                {
                    case 1:
                        color = Color.Red;
                        break;
                    case 2:
                        color = _red2;
                        break;
                }

                SetButColor(button.E_But, color);

                Tracks.ChannelState[i].Muted = button.Mute_But_s != 0;

                switch (button.T_But_s)
                {
                    case 1:
                        color = Color.Red;
                        break;
                    case 2:
                        color = _red2;
                        break;
                }

                SetButColor(button.Mute_But, color);
                SetButColor(button.Solo_But, button.Solo_But_s != 0 ? _yellow2 : SystemColors.Control);
            }

            SetButColor(SoloButton, CheckSolo() ? _yellow2 : SystemColors.Control);
            SetButColor(MuteButton, CheckMute() ? Color.Red : SystemColors.Control);

            UpdateHintsForChannelButtons();
        }

        public void ApplySolo()
        {
            if (CheckSolo())
            {
                for (int j = 0; j < 3; j++)
                {
                    var curWin = this;

                    switch (j)
                    {
                        case 1:
                            curWin = TSWindow[0];
                            break;
                        case 2:
                            curWin = TSWindow[1];
                            break;
                    }

                    if (curWin == null)
                        continue;

                    for (int i = 0; i < 3; i++)
                    {
                        var b = curWin.ChanButtons[i];

                        if (b.Mute_But_s == 0 && b.Solo_But_s == 0)
                        {
                            b.Mute_But_s = 2;
                            b.T_But_s = 2;
                            b.N_But_s = 2;
                            b.E_But_s = 2;
                        }
                    }
                }
            }
            else
            {
                // un solo
                for (int j = 0; j < 3; j++)
                {
                    var curWin = this;

                    switch (j)
                    {
                        case 1:
                            curWin = TSWindow[0];
                            break;
                        case 2:
                            curWin = TSWindow[1];
                            break;
                    }

                    if (curWin == null)
                        continue;

                    for (int i = 0; i < 3; i++)
                    {
                        var b = curWin.ChanButtons[i];

                        if (b.Mute_But_s == 2 && b.Mute_But_s != 0) b.Mute_But_s = 0;
                        if (b.T_But_s == 2 && b.T_But_s != 0) b.T_But_s = 0;
                        if (b.N_But_s == 2 && b.N_But_s != 0) b.N_But_s = 0;
                        if (b.E_But_s == 2 && b.E_But_s != 0) b.E_But_s = 0;
                    }
                }
            }
        }

        public void AllMute_Click(object sender, EventArgs e)
        {
            bool isOn = MuteButton.BackColor != SystemColors.Control;

            for (int j = 0; j < 3; j++)
            {
                var curWin = this;

                switch (j)
                {
                    case 1:
                        curWin = TSWindow[0];
                        break;
                    case 2:
                        curWin = TSWindow[1];
                        break;
                }

                if (curWin == null)
                    continue;

                for (int i = 0; i < 3; i++)
                {
                    if (isOn)
                    {
                        curWin.ChanButtons_s[i].Mute_But_s = curWin.ChanButtons[i].Mute_But_s;
                        curWin.ChanButtons_s[i].T_But_s = curWin.ChanButtons[i].T_But_s;
                        curWin.ChanButtons_s[i].N_But_s = curWin.ChanButtons[i].N_But_s;
                        curWin.ChanButtons_s[i].E_But_s = curWin.ChanButtons[i].E_But_s;

                        curWin.ChanButtons[i].Mute_But_s = 0;
                        curWin.ChanButtons[i].T_But_s = 0;
                        curWin.ChanButtons[i].N_But_s = 0;
                        curWin.ChanButtons[i].E_But_s = 0;
                    }
                    else
                    {
                        curWin.ChanButtons[i].Mute_But_s = curWin.ChanButtons_s[i].Mute_But_s;
                        curWin.ChanButtons[i].T_But_s = curWin.ChanButtons_s[i].T_But_s;
                        curWin.ChanButtons[i].N_But_s = curWin.ChanButtons_s[i].N_But_s;
                        curWin.ChanButtons[i].E_But_s = curWin.ChanButtons_s[i].E_But_s;
                    }
                }
            }

            ApplySolo();
            UpdateChannelsState();
        }

        public void AllSolo_Click(object sender, EventArgs e)
        {
            bool isOn = SoloButton.BackColor != SystemColors.Control;

            for (int j = 0; j < 3; j++)
            {
                var curWin = this;

                switch (j)
                {
                    case 1:
                        curWin = TSWindow[0];
                        break;
                    case 2:
                        curWin = TSWindow[1];
                        break;
                }

                if (curWin == null)
                    continue;

                for (int i = 0; i < 3; i++)
                {
                    if (isOn)
                    {
                        curWin.ChanButtons_s[i].Solo_But_s = curWin.ChanButtons[i].Solo_But_s;
                        curWin.ChanButtons[i].Solo_But_s = 0;
                    }
                    else
                    {
                        curWin.ChanButtons[i].Solo_But_s = curWin.ChanButtons_s[i].Solo_But_s;
                    }
                }
            }

            ApplySolo();
            UpdateChannelsState();
        }

        public void MuteOn_Click(object sender, EventArgs e)
        {
            int chan = Array.FindIndex(ChanButtons, b => b.Mute_But == sender);

            if (chan == -1)
                return;

            var button = ChanButtons[chan];

            button.Mute_But_s = button.Mute_But_s == 0 ? 1 : 0;
            button.T_But_s = button.Mute_But_s;
            button.N_But_s = button.Mute_But_s;
            button.E_But_s = button.Mute_But_s;
            button.Solo_But_s = 0;

            // if unmuting and another solo is active, reapply solo
            if (button.Mute_But_s == 0 && CheckSolo())
                button.Solo_But_s = 1;

            ApplySolo();
            UpdateChannelsState();
        }

        public void SoloOn_Click(object sender, EventArgs e)
        {
            int chan = Array.FindIndex(ChanButtons, b => b.Solo_But == sender);

            if (chan == -1)
                return;

            var button = ChanButtons[chan];
            button.Solo_But_s = button.Solo_But_s == 0 ? 1 : 0;

            button.T_But_s = 0;
            button.N_But_s = 0;
            button.E_But_s = 0;
            button.Mute_But_s = 0;

            ApplySolo();
            UpdateChannelsState();
        }

        public void TNEOn_Click(object sender, EventArgs e)
        {
            var checkBox = (CheckBox)sender;

            if (checkBox == null)
                return;

            for (int chan = 0; chan < 3; chan++)
            {
                if (ChanButtons[chan].Box == checkBox.Parent)
                {
                    int valueRef = GetStatusRef(ChanButtons[chan], checkBox.Text);

                    valueRef = valueRef == 0 ? 1 : 0;

                    var btn = ChanButtons[chan];
                    btn.Mute_But_s = (btn.T_But_s != 0 && btn.N_But_s != 0 && btn.E_But_s != 0) ? btn.T_But_s : 0;

                    if (btn.Mute_But_s != 0)
                        btn.Solo_But_s = 0;
                    else if (CheckSolo())
                        btn.Solo_But_s = 1;

                    ApplySolo();
                    UpdateChannelsState();
                    break;
                }
            }
        }

        private int GetStatusRef(ChannelButtons button, string caption)
        {
            switch (caption)
            {
                case "T":
                    return button.T_But_s;
                case "N":
                    return button.N_But_s;
                case "E":
                    return button.E_But_s;
                default:
                    throw new ArgumentException("Invalid panel caption");
            }
        }


        public void VtmFeaturesBox_Click(object sender, EventArgs e)
        {
            if (InitFinished)
            {
                SongChanged = true;
                BackupSongChanged = true;
                AddUndo(TChangeAction.ChangeFeatures, VTM.FeaturesLevel, VtmFeaturesBox.SelectedIndex);
            }

            VTM.FeaturesLevel = (FeaturesLevel)VtmFeaturesBox.SelectedIndex;

            if (BlockRecursion)
                return;

            if (TSWindow[0] != null)
            {
                TSWindow[0].BlockRecursion = true;
                TSWindow[0].VtmFeaturesBox.SelectedIndex = VtmFeaturesBox.SelectedIndex;
                TSWindow[0].BlockRecursion = false;
            }

            if (TSWindow[1] != null)
            {
                TSWindow[1].BlockRecursion = true;
                TSWindow[1].VtmFeaturesBox.SelectedIndex = VtmFeaturesBox.SelectedIndex;
                TSWindow[1].BlockRecursion = false;
            }
        }

        public void SaveHeaderBox_Click(object sender, EventArgs e)
        {
            if (!InitFinished)
                return;

            SongChanged = true;
            BackupSongChanged = true;

            AddUndo(TChangeAction.ChangeHeader, VTM.HasHeader ? 0 : 1, SaveHeaderBox.SelectedIndex);

            VTM.HasHeader = SaveHeaderBox.SelectedIndex == 0;

            if (BlockRecursion)
                return;

            if (TSWindow[0] != null)
            {
                TSWindow[0].BlockRecursion = true;
                TSWindow[0].SaveHeaderBox.SelectedIndex = SaveHeaderBox.SelectedIndex;
                TSWindow[0].BlockRecursion = false;
            }

            if (TSWindow[1] != null)
            {
                TSWindow[1].BlockRecursion = true;
                TSWindow[1].SaveHeaderBox.SelectedIndex = SaveHeaderBox.SelectedIndex;
                TSWindow[1].BlockRecursion = false;
            }
        }

        public void SampleNumEditChange(object sender)
        {
            if (SampleIndex != SampleNumUpDown.Value)
                ChangeSample((int)SampleNumUpDown.Value, true, true);
        }

        public void SampleNumEditExit(object sender)
        {
            SampleNumUpDown.Text = SampleNumUpDown.Value.ToString();
        }

        public void SampleNumUpDown_ValueChanging(object sender, ValueChangingEventArgs e)
        {
            e.Cancel = e.NewValue < 1 || e.NewValue > 31;

            if (e.Cancel)
                return;

            SamplesSelectionOff();
            ChangeSample((int)e.NewValue, false, true);
        }

        /* public void SampleLenEditExit(object sender)
        {
            int newValue = (int)SampleLenUpDown.Value;
            bool allowChange = true;

            if (MainForm.DecBaseLinesOn)
            {
                if (IsDecValid(SampleLenEdit.Text))
                {
                    newValue = Convert.ToInt32(SampleLenEdit.Text);
                }
                else
                {
                    allowChange = false;
                }
            }
            if (!MainForm.DecBaseLinesOn)
            {
                if (IsHexValid(SampleLenEdit.Text))
                {
                    newValue = Convert.ToInt32(SampleLenEdit.Text, 16);
                }
                else
                {
                    allowChange = false;
                }
            }
            if (newValue > TVTM.MaxSamLen)
            {
                allowChange = false;
            }
            if (allowChange)
            {
                SampleLenUpDown.Value = newValue;
            }
            else if (MainForm.DecBaseLinesOn)
            {
                SampleLenEdit.Text = SampleLenUpDown.Value.ToString();
            }
            else
            {
                SampleLenEdit.Text = SampleLenUpDown.Value.ToString("x");
            }
            // ChangeSampleLength(SampleLenUpDown.Value)
        } */

        public void SampleLengthUpDown_ValueChanging(object sender, ValueChangingEventArgs e)
        {
            e.Cancel = e.NewValue < 1 || e.NewValue > VTModule.MaxSampleLength;

            if (e.Cancel)
                return;

            SamplesSelectionOff();
            //SampleLenEdit.Text = MainForm.DecBaseLinesOn ? e.NewValue.ToString() : e.NewValue.ToString("X");
            ChangeSampleLength((int)e.NewValue, false);
        }

        /* public void ChangeSample(int n, bool updateUpDown, bool updateGrid)
        {
            int l;

            if (SamNum != n)
            {
                Samples.IsSelecting = false;
                SamNum = n;
                VTM.ReservedPattern.Items[1].Channel[0].Sample = (byte)n;

                if (SampleTestLine.Focused)
                    HideCaret(SampleTestLine.Handle);

                SampleTestLine.RedrawTestLine();

                if (SampleTestLine.Focused)
                    ShowCaret(SampleTestLine.Handle);

                Samples.ShownSample = VTM.Samples[SamNum];

                if (updateUpDown)
                    SampleNumUpDown.Value = n;

                if (updateGrid)
                    SamplesGrid.CurrentCell = SamplesGrid[SamNum - 1, 0];

                l = VTM.Samples[SamNum] == null ? 1 : VTM.Samples[SamNum].Length;
                SampleLengthUpDown.Value = l;

                l = VTM.Samples[SamNum] == null ? 0 : VTM.Samples[SamNum].Loop;
                SampleLoopUpDown.Value = l;

                if (!UndoWorking)
                {
                    Samples.ShownFrom = 0;
                    Samples.CursorX = 0;
                    Samples.CursorY = 0;
                }
            }

            if (Samples.Focused)
            {
                Samples.RecreateCaret();
                Samples.SetCaretPosition();
                Samples.HideCaret();
            }

            Samples.Redraw();

            if (Samples.Focused)
                Samples.ShowCaret();
        } */

        public void ChangeSample(int sampleIndex, bool updateUpDown, bool updateGrid)
        {
            sampleIndex = Math.Clamp(sampleIndex, 1, 31);

            if (SampleIndex != sampleIndex)
            {
                Samples.IsSelecting = false;
                SampleIndex = sampleIndex;

                // Update SampleTestLine
                var sampleLine = SampleTestLine;
                VTM.ReservedPattern.Lines[1].Channel[0].Sample = (byte)sampleIndex;

                if (sampleLine.Focused)
                    HideCaret(sampleLine.Handle);

                sampleLine.Redraw();

                if (sampleLine.Focused)
                    ShowCaret(sampleLine.Handle);

                // Update OrnamentTestLine sample too
                VTM.ReservedPattern.Lines[0].Channel[0].Sample = (byte)sampleIndex;

                if (OrnamentTestLine.Focused)
                    HideCaret(OrnamentTestLine.Handle);

                OrnamentTestLine.Redraw();

                if (OrnamentTestLine.Focused)
                    ShowCaret(OrnamentTestLine.Handle);

                // Update shown sample
                Samples.ShownSample = VTM.Samples[SampleIndex];

                if (updateUpDown)
                    SampleNumUpDown.Value = sampleIndex;

                if (updateGrid)
                    SamplesGrid.CurrentCell = SamplesGrid[sampleIndex - 1, 0]; // Column select

                int l = VTM.Samples[SampleIndex] == null ? 1 : VTM.Samples[SampleIndex].Length;
                SampleLengthUpDown.Value = l;

                l = VTM.Samples[SampleIndex] == null ? 0 : VTM.Samples[SampleIndex].Loop;
                SampleLoopUpDown.Value = l;

                if (!UndoWorking)
                {
                    Samples.ShownFrom = 0;
                    Samples.CursorX = 0;
                    Samples.CursorY = 0;
                }
            }

            if (Samples.Focused)
            {
                Samples.RecreateCaret();
                Samples.SetCaretPosition();
                Samples.HideCaret();
            }

            Samples.Redraw();

            if (Samples.Focused)
                Samples.ShowCaret();
        }

        public void ChangeSampleLength(int nl, bool updateUpDown)
        {
            if (VTM.Samples[SampleIndex] == null && nl == 1)
                return;

            Samples.IsSelecting = false;
            ValidateSample2(SampleIndex);

            if (nl != VTM.Samples[SampleIndex].Length)
            {
                SongChanged = true;
                BackupSongChanged = true;

                if (!Samples.UndoSaved)
                    AddUndo(TChangeAction.ChangeSampleSize, VTM.Samples[SampleIndex].Length, nl);

                VTM.Samples[SampleIndex].Length = (byte)nl;

                if (!UndoWorking)
                {
                    if (!Samples.UndoSaved)
                        ChangeList[ChangeCount - 1].OldParams.Params.PrevLoop = VTM.Samples[SampleIndex].Loop;

                    // If sample loop > length
                    if (VTM.Samples[SampleIndex].Loop >= VTM.Samples[SampleIndex].Length)
                    {
                        VTM.Samples[SampleIndex].Loop = (byte)(VTM.Samples[SampleIndex].Length - 1);
                        SampleLoopUpDown.Value = VTM.Samples[SampleIndex].Loop;
                    }

                    if (!Samples.UndoSaved)
                        ChangeList[ChangeCount - 1].NewParams.Params.PrevLoop = VTM.Samples[SampleIndex].Loop;

                    if (updateUpDown)
                        SampleLengthUpDown.Value = nl;

                    if (Samples.Focused)
                        Samples.HideCaret();

                    Samples.Redraw();

                    if (Samples.Focused)
                        Samples.ShowCaret();
                }
            }
        }

        public void ChangeSampleLoop(int nl, bool updateUpDown)
        {
            Samples.IsSelecting = false;

            if (VTM.Samples[SampleIndex] == null)
                return;

            if (nl != VTM.Samples[SampleIndex].Loop && nl <= VTM.Samples[SampleIndex].Length)
            {
                SongChanged = true;
                BackupSongChanged = true;

                if (!Samples.UndoSaved)
                    AddUndo(TChangeAction.ChangeSampleLoop, VTM.Samples[SampleIndex].Loop, nl);

                VTM.Samples[SampleIndex].Loop = (byte)nl;

                if (updateUpDown)
                    SampleLoopUpDown.Value = nl;

                if (Samples.Focused)
                    Samples.HideCaret();

                Samples.Redraw();

                if (Samples.Focused)
                    Samples.ShowCaret();
            }
        }

        public void ClearShownOrnament()
        {
            int i;

            if (Ornaments.ShownOrnament == null)
                return;

            for (i = 0; i < VTModule.MaxOrnamentLength; i++)
                Ornaments.ShownOrnament.Offsets[i] = 0;

            Ornaments.CursorY = 0;
            Ornaments.CursorX = 0;
            Ornaments.ShownFrom = 0;
            Ornaments.ShownOrnament.Length = 1;
            Ornaments.ShownOrnament.Loop = 0;
            Ornaments.SetCaretPosition();
            OrnamentLenUpDown.Value = 1;
            OrnamentLoopUpDown.Value = 0;
            SongChanged = true;
            BackupSongChanged = true;
        }

        public void ClearShownSample()
        {
            if (Samples.ShownSample == null)
                return;

            Samples.Sample = new Sample();
            Samples.ShownSample.Ticks = Samples.Sample.Ticks;
            Samples.ShownSample.Length = Samples.Sample.Length;
            Samples.ShownSample.Loop = Samples.Sample.Loop;
            Samples.ShownSample.Enabled = true;
            Samples.CursorY = 0;
            Samples.ShownFrom = 0;
            Samples.SetCaretPosition();
            SampleLengthUpDown.Value = Samples.Sample.Length;
            SampleLoopUpDown.Value = Samples.Sample.Loop;
            SongChanged = true;
            BackupSongChanged = true;
        }

        public void ChangeOrnament(int ornamentIndex, bool updateUpDown, bool updateGrid)
        {
            if (ornamentIndex == 0)
                return;

            ornamentIndex = Math.Clamp(ornamentIndex, 1, 31);

            if (ornamentIndex == OrnamentIndex)
                return;

            Ornaments.IsSelecting = false;
            OrnamentIndex = ornamentIndex;

            // Update test line ornament reference
            VTM.ReservedPattern.Lines[0].Channel[0].Ornament = (byte)ornamentIndex;

            if (OrnamentTestLine.Focused)
                HideCaret(OrnamentTestLine.Handle);

            OrnamentTestLine.Redraw();

            if (OrnamentTestLine.Focused)
                ShowCaret(OrnamentTestLine.Handle);

            // Update shown ornament
            Ornaments.ShownOrnament = VTM.Ornaments[OrnamentIndex];

            int l = VTM.Ornaments[OrnamentIndex] == null ? 1 : VTM.Ornaments[OrnamentIndex].Length;
            OrnamentLenUpDown.Value = l;

            if (updateUpDown)
                OrnamentNumUpDown.Value = ornamentIndex;

            if (updateGrid)
                OrnamentsGrid.CurrentCell = OrnamentsGrid[OrnamentIndex - 1, 0]; // Set column

            l = VTM.Ornaments[OrnamentIndex] == null ? 0 : VTM.Ornaments[OrnamentIndex].Loop;
            OrnamentLoopUpDown.Value = l;

            if (!UndoWorking)
            {
                Ornaments.CursorX = 0;
                Ornaments.CursorY = 0;
                Ornaments.ShownFrom = 0;
            }

            if (Ornaments.Focused)
            {
                Ornaments.SetCaretPosition();
                Ornaments.HideCaret();
            }

            Ornaments.Redraw();

            if (Ornaments.Focused)
                Ornaments.ShowCaret();
        }

        public void ChangeOrnamentLength(int nl, bool updateUpDown)
        {
            Ornaments.IsSelecting = false;

            if (VTM.Ornaments[OrnamentIndex] == null && nl == 1)
                return;

            ValidateOrnament(OrnamentIndex);

            if (nl != VTM.Ornaments[OrnamentIndex].Length)
            {
                SongChanged = true;
                BackupSongChanged = true;

                if (!Ornaments.UndoSaved)
                    AddUndo(TChangeAction.ChangeOrnamentSize, VTM.Ornaments[OrnamentIndex].Length, nl);

                VTM.Ornaments[OrnamentIndex].Length = nl;

                if (!UndoWorking)
                {
                    if (!Ornaments.UndoSaved)
                        ChangeList[ChangeCount - 1].OldParams.Params.PrevLoop = VTM.Ornaments[OrnamentIndex].Loop;

                    // Decrease loop if length < loop
                    if (VTM.Ornaments[OrnamentIndex].Loop >= VTM.Ornaments[OrnamentIndex].Length)
                    {
                        VTM.Ornaments[OrnamentIndex].Loop = VTM.Ornaments[OrnamentIndex].Length - 1;
                        OrnamentLoopUpDown.Value = VTM.Ornaments[OrnamentIndex].Loop;
                    }

                    if (!Ornaments.UndoSaved)
                        ChangeList[ChangeCount - 1].NewParams.Params.PrevLoop = VTM.Ornaments[OrnamentIndex].Loop;

                    if (updateUpDown)
                        OrnamentLenUpDown.Value = nl;

                    if (Ornaments.Focused)
                        Ornaments.HideCaret();

                    Ornaments.Redraw();

                    if (Ornaments.Focused)
                        Ornaments.ShowCaret();
                }
            }
        }

        public void ChangeOrnamentLoop(int nl, bool updateUpDown)
        {
            Ornaments.IsSelecting = false;

            if (VTM.Ornaments[OrnamentIndex] == null)
                return;

            if (nl != VTM.Ornaments[OrnamentIndex].Loop && nl < VTM.Ornaments[OrnamentIndex].Length)
            {
                SongChanged = true;
                BackupSongChanged = true;

                if (!Ornaments.UndoSaved)
                    AddUndo(TChangeAction.ChangeOrnamentLoop, VTM.Ornaments[OrnamentIndex].Loop, nl);

                VTM.Ornaments[OrnamentIndex].Loop = nl;

                if (updateUpDown)
                    OrnamentLoopUpDown.Value = nl;

                if (Ornaments.Focused)
                    Ornaments.HideCaret();

                Ornaments.Redraw();

                if (Ornaments.Focused)
                    Ornaments.ShowCaret();
            }
        }

        public void ValidateSample2(int sam)
        {
            VTModule.ValidateSample(sam, VTM);

            if (sam == SampleIndex)
                Samples.ShownSample = VTM.Samples[SampleIndex];
        }

        public void ValidateOrnament(int orn)
        {
            int i;

            if (VTM.Ornaments[orn] == null)
            {
                VTM.Ornaments[orn] = new Ornament();
                VTM.Ornaments[orn].Loop = 0;
                VTM.Ornaments[orn].Length = 1;

                for (i = 0; i < VTModule.MaxOrnamentLength; i++)
                    VTM.Ornaments[orn].Offsets[i] = 0;

                if (orn == OrnamentIndex)
                    Ornaments.ShownOrnament = VTM.Ornaments[OrnamentIndex];
            }
        }

        public void SampleLoopUpDown_ValueChanging(object sender, ValueChangingEventArgs e)
        {
            int l = VTM.Samples[SampleIndex] == null ? 1 : VTM.Samples[SampleIndex].Length;

            e.Cancel = e.NewValue < 0 || e.NewValue >= l;

            if (e.Cancel)
                return;

            SamplesSelectionOff();
            //SampleLoopEdit.Text = MainForm.DecBaseLinesOn ? e.NewValue.ToString() : e.NewValue.ToString("X");
            ChangeSampleLoop((int)e.NewValue, false);
        }

        public void CalculatePos0()
        {
            PosBegin = VTModule.GetPositionTime(VTM, PositionIndex, ref PosDelay);
            LineInts = 0;
            // Label25.Caption := IntsToTime(PosBegin);
            ReCalcTimes(PosBegin);
            UpdateIntsInfo(PosBegin);
            SynchronizeModules();
        }

        public void CalculatePos(int line)
        {
            if (PositionIndex >= VTM.Positions.Length || VTM.Positions.Value[PositionIndex] != PatternIndex)
                return;

            LineInts = VTModule.GetPositionTimeEx(VTM, PositionIndex, ref PosDelay, line);
            int i = PosBegin + LineInts;
            ReCalcTimes(i);
            UpdateIntsInfo(i);
            Globals.MainForm.StatusBar.Refresh();
            SynchronizeModules();
        }

        public void CheckPositionsGridPosition()
        {
            Rectangle selectionRect;

            if (IsClosed)
                return;

            if (VTM.Positions.Length == 0)
            {
                selectionRect = Rectangle.Empty;
                PositionsGrid.Selection = selectionRect;
                return;
            }

            if (PositionsGrid.Selection.Left != PositionsGrid.Selection.Right)
            {
                selectionRect = Rectangle.FromLTRB(PositionIndex, 0, PositionIndex, 0);
                PositionsGrid.Selection = selectionRect;
            }

            if (PositionsGrid.Selection.Left >= VTM.Positions.Length)
            {
                SelectPosition2(VTM.Positions.Length - 1);
            }

            if (TSWindow[0] == null || TSWindow[0].IsClosed || !TSWindow[0].Visible)
                return;

            if (TSWindow[0].PositionsGrid.Selection.Left > TSWindow[0].VTM.Positions.Length - 1)
                TSWindow[0].SelectPosition2(TSWindow[0].VTM.Positions.Length - 1);

            if (TSWindow[1] == null || TSWindow[1].IsClosed || !TSWindow[1].Visible)
                return;

            if (TSWindow[1].PositionsGrid.Selection.Left > TSWindow[1].VTM.Positions.Length - 1)
                TSWindow[1].SelectPosition2(TSWindow[1].VTM.Positions.Length - 1);
        }

        public void ShowStat()
        {
            if (IsClosed)
                return;

            CheckPositionsGridPosition();

            if (VTM.Positions.Length == 0)
            {
                Globals.MainForm.StatusBar.Items[1].Text = "0:0:0";
                Globals.MainForm.StatusBar.Items[2].Text = "0:00 / 0:04";
                return;
            }

            if (VTM != null && VTM.Positions.Length > 0 && PositionsGrid.Selection.Left < VTM.Positions.Length && VTM.Positions.Value[PositionIndex] == PatternIndex)
            {
                // CalculatePos(Tracks.ShownFrom + Tracks.CursorY - Tracks.N1OfLines)
                CalculatePos(Tracks.ShownFrom);
            }
        }

        public void UpdateIntsInfo(int psBegin)
        {
            Globals.MainForm.StatusBar.Items[1].Text = $"{psBegin}:{LineInts}:{TotInts}";
        }

        public void ShowAllTots()
        {
            // Label20.Caption := IntsToTime(TotInts);
            ReCalcTimes(PosBegin);
            UpdateIntsInfo(PosBegin);
            Globals.MainForm.StatusBar.Refresh();
        }

        public void CalcTotLen()
        {
            TotInts = VTModule.GetModuleTime(VTM);
            ShowAllTots();
        }

        public void ReCalcTimes(int posBegin)
        {
            // Label20.Caption := IntsToTime(TotInts);
            // Label25.Caption := IntsToTime(PosBegin + LineInts)
            Globals.MainForm.StatusBar.Items[2].Text = $"{VTModule.IntsToTime(posBegin)} / {VTModule.IntsToTime(TotInts)}";
        }

        public void SetInitDelay(int initialDelay)
        {
            if (VTM.InitialDelay == initialDelay)
                return;

            SongChanged = true;
            BackupSongChanged = true;

            AddUndo(TChangeAction.ChangeSpeed, VTM.InitialDelay, initialDelay);
            VTM.InitialDelay = (byte)initialDelay;

            CalcTotLen();
            CalculatePos0();

            if (WaveOutAPI.IsPlaying)
                RestartPlayingPosition(PositionIndex);
        }

        // // Templates in samples editor disabled
        // // People really no need this feature
        // 
        // procedure TMDIChild.ListBox1Click(sender: TObject);
        // begin
        // MainForm.SetSampleTemplate(ListBox1.SelectedIndex)
        // end;
        // 
        // procedure TMDIChild.SpeedButton13Click(sender: TObject);
        // begin
        // AddCurrentToSampTemplate
        // end;
        // 
        // procedure TMDIChild.AddCurrentToSampTemplate;
        // var
        // i: Integer;
        // begin
        // with Samples do
        // begin
        // if ShownSample = nil then
        // exit;
        // i := ShownFrom + CursorY;
        // if i >= ShownSample.Length then
        // exit;
        // MainForm.AddToSampTemplate(ShownSample.Items[i])
        // end
        // end;
        // 
        // procedure TMDIChild.SpeedButton14Click(sender: TObject);
        // begin
        // CopySampTemplateToCurrent
        // end;
        // 
        // procedure TMDIChild.SpeedButton23Click(sender: TObject);
        // begin
        // MainForm.ResetSampTemplate
        // end;
        // 
        // procedure TMDIChild.CopySampTemplateToCurrent;
        // var
        // i, l: Integer;
        // ST: PSampleTick;
        // begin
        // with Samples do
        // begin
        // if ShownSample = nil then
        // l := 1
        // else
        // l := ShownSample.Length;
        // i := ShownFrom + CursorY;
        // if i >= l then
        // exit;
        // SongChanged := True;
        // ValidateSample2(SamNum);
        // New(ST);
        // ST^ := ShownSample.Items[i];
        // AddUndo(TChangeAction.CAChangeSampleValue, Integer(ST), i);
        // ShownSample.Items[i] :=
        // MainForm.SampleLineTemplates[MainForm.CurrentSampleLineTemplate];
        // if Focused then
        // HideCaret(Handle);
        // RedrawSamples();
        // if Focused then
        // ShowCaret(Handle)
        // end
        // end;

        public void OrnamentNumUpDown_ValueChanging(object sender, ValueChangingEventArgs e)
        {
            e.Cancel = e.NewValue < 1 || e.NewValue > 15;

            if (e.Cancel)
                return;

            ChangeOrnament((int)e.NewValue, false, true);
        }

        /* public void OrnamentNumEditChange(object sender)
        {
            if (OrnNum != OrnamentNumUpDown.Value)
            {
                ChangeOrnament((int)OrnamentNumUpDown.Value, false, true);
            }
        }

        public void OrnamentNumEditExit(object sender)
        {
            OrnamentNumEdit.Text = (OrnamentNumUpDown.Value).ToString();
        } */

        public void OrnamentLoopUpDown_ValueChanging(object sender, ValueChangingEventArgs e)
        {
            int l = VTM.Ornaments[OrnamentIndex] == null ? 1 : VTM.Ornaments[OrnamentIndex].Length;

            e.Cancel = e.NewValue < 0 || e.NewValue >= l;

            if (e.Cancel)
                return;

            //OrnamentLoopEdit.Text = MainForm.DecBaseLinesOn ? e.NewValue.ToString() : e.NewValue.ToString("X");
            ChangeOrnamentLoop((int)e.NewValue, false);
        }

        /* public void OrnamentLoopEditExit(object sender)
        {
            int newValue = (int)OrnamentLoopUpDown.Value;
            int ornLen;
            bool allowChange = true;

            if (Ornaments.ShownOrnament == null)
            {
                ornLen = 1;
            }
            else
            {
                ornLen = Ornaments.ShownOrnament.Length;
            }
            if (MainForm.DecBaseLinesOn)
            {
                if (IsDecValid(OrnamentLoopEdit.Text))
                {
                    newValue = Convert.ToInt32(OrnamentLoopEdit.Text);
                }
                else
                {
                    allowChange = false;
                }
            }
            if (!MainForm.DecBaseLinesOn)
            {
                if (IsHexValid(OrnamentLoopEdit.Text))
                {
                    newValue = Convert.ToInt32(OrnamentLoopEdit.Text, 16);
                }
                else
                {
                    allowChange = false;
                }
            }
            if ((newValue > TVTM.MaxOrnLen) || (newValue > ornLen))
            {
                allowChange = false;
            }
            if (allowChange)
            {
                OrnamentLoopUpDown.Value = newValue;
            }
            else if (MainForm.DecBaseLinesOn)
            {
                OrnamentLoopEdit.Text = OrnamentLoopUpDown.Value.ToString();
            }
            else
            {
                OrnamentLoopEdit.Text = OrnamentLoopUpDown.Value.ToString("X");
            }
        } */

        public void OrnamentLenUpDown_ValueChanging(object sender, ValueChangingEventArgs e)
        {
            e.Cancel = e.NewValue < 1 || e.NewValue > VTModule.MaxOrnamentLength;

            if (e.Cancel)
                return;

            //OrnamentLenEdit.Text = MainForm.DecBaseLinesOn ? e.NewValue.ToString() : e.NewValue.ToString("X");
            ChangeOrnamentLength((int)e.NewValue, false);
        }

        /* public void OrnamentLenEditExit(object sender)
        {
            int newValue = (int)OrnamentLenUpDown.Value;
            bool allowChange = true;

            if (MainForm.DecBaseLinesOn)
            {
                if (IsDecValid(OrnamentLenEdit.Text))
                {
                    newValue = Convert.ToInt32(OrnamentLenEdit.Text);
                }
                else
                {
                    allowChange = false;
                }
            }
            if (!MainForm.DecBaseLinesOn)
            {
                if (IsHexValid(OrnamentLenEdit.Text))
                {
                    newValue = Convert.ToInt32(OrnamentLenEdit.Text, 16);
                }
                else
                {
                    allowChange = false;
                }
            }
            if (newValue > TVTM.MaxOrnLen)
            {
                allowChange = false;
            }
            if (allowChange)
            {
                OrnamentLenUpDown.Value = newValue;
            }
            else if (MainForm.DecBaseLinesOn)
            {
                OrnamentLenEdit.Text = OrnamentLenUpDown.Value.ToString();
            }
            else
            {
                OrnamentLenEdit.Text = OrnamentLenUpDown.Value.ToString("X");
            }
        } */

        public void ChildWin_KeyDown(object sender, KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool isCtrlDown = (e.Modifiers & Keys.Control) == Keys.Control;
            bool isAltDown = (e.Modifiers & Keys.Alt) == Keys.Alt;

            if (isCtrlDown)
            {
                switch (e.KeyCode)
                {
                    case Keys.E:
                    case Keys.NumPad0:
                        ToggleAutoEnv();
                        break;
                    case Keys.R:
                        ToggleAutoStep();
                        break;
                    case Keys.Oemtilde:
                        // case PageControl1.TabIndex of
                        // 0: PageControl1.TabIndex:=1;
                        // 1: PageControl1.TabIndex:=0;
                        // else ;
                        // PageControl1.TabIndex:=1;
                        // end;
                        TabControl.TabIndex = 0;
                        if (Tracks.CanFocus)
                            Tracks.Focus();
                        break;
                    case Keys.D1:
                        TabControl.TabIndex = 1;
                        if (Samples.CanFocus)
                            Samples.Focus();
                        break;
                    case Keys.D2:
                        TabControl.TabIndex = 2;
                        if (Ornaments.CanFocus)
                            Ornaments.Focus();
                        break;
                }
            }
            else if (isAltDown)
            {
                if (e.KeyCode == Keys.E)
                    ToggleStdAutoEnv();
            }
        }

        public void ToggleAutoEnv()
        {
            AutoEnv = !AutoEnv;
            AutoEnvButton.Checked = AutoEnv;
        }

        public void ToggleStdAutoEnv()
        {
            if (!AutoEnv)
                ToggleAutoEnv();

            if (StdAutoEnvIndex == MainForm.StdAutoEnvMax)
                StdAutoEnvIndex = 0;
            else
                StdAutoEnvIndex++;

            AutoEnv0 = MainForm.StdAutoEnv[StdAutoEnvIndex, 0];
            AutoEnv1 = MainForm.StdAutoEnv[StdAutoEnvIndex, 1];
            AutoEnv0Button.Text = AutoEnv0.ToString();
            AutoEnv1Button.Text = AutoEnv1.ToString();
        }

        public void AutoEnvToggleButton_Click(object sender, EventArgs e)
        {
            ToggleStdAutoEnv();
        }

        public void AutoEnvButton_Click(object sender, EventArgs e)
        {
            ToggleAutoEnv();
        }

        public void AutoEnv0Button_Click(object sender, EventArgs e)
        {
            if (!AutoEnv)
                ToggleAutoEnv();
            StdAutoEnvIndex = -1;

            if (AutoEnv0 == 9)
                AutoEnv0 = 1;
            else
                AutoEnv0++;

            AutoEnv0Button.Text = AutoEnv0.ToString();
        }

        public void AutoEnv1Button_Click(object sender, EventArgs e)
        {
            if (!AutoEnv)
                ToggleAutoEnv();

            StdAutoEnvIndex = -1;

            if (AutoEnv1 == 9)
                AutoEnv1 = 1;
            else
                AutoEnv1++;

            AutoEnv1Button.Text = AutoEnv1.ToString();
        }

        public void DoAutoEnv(int i, int j, int k)
        {
            int n;
            int old;

            if (AutoEnv)
            {
                n = VTM.Patterns[i].Lines[j].Channel[k].Note;

                if (n < 0)
                    return;

                switch (VTM.Patterns[i].Lines[j].Channel[k].Envelope)
                {
                    case 8:
                    case 12:
                        n = (int)Math.Round(VTModule.GetNoteFreq(VTM.NoteTable, n) * AutoEnv0 / AutoEnv1 / 16.0);
                        break;
                    case 10:
                    case 14:
                        n = (int)Math.Round(VTModule.GetNoteFreq(VTM.NoteTable, n) * AutoEnv0 / AutoEnv1 / 32.0);
                        break;
                    default:
                        return;
                }

                old = VTM.Patterns[i].Lines[j].Envelope;

                if (n == old)
                    return;

                if (!UndoWorking)
                {
                    AddUndo(TChangeAction.ChangeEnvelopePeriod, old, n);
                    ChangeList[ChangeCount - 1].Line = j;
                }

                VTM.Patterns[i].Lines[j].Envelope = (ushort)n;

                SongChanged = true;
                BackupSongChanged = true;
            }
        }

        public void PositionsGrid_KeyDown(object sender, KeyEventArgs e)
        {
            bool isLeftDown = (Control.MouseButtons & MouseButtons.Left) == MouseButtons.Left;
            bool isRightDown = (Control.MouseButtons & MouseButtons.Right) == MouseButtons.Right;
            bool isButtonDown = isLeftDown || isRightDown;

            Rectangle selectionRect;

            if (!isButtonDown && e.KeyValue == 192)
            {
                if (Tracks.CanFocus)
                    Tracks.Focus();
            }

            // End pressed
            if (e.KeyCode == Keys.End)
            {
                selectionRect = Rectangle.FromLTRB(VTM.Positions.Length - 1, 0, VTM.Positions.Length - 1, 0);
                PositionsGrid.Selection = selectionRect;
                SelectPosition(selectionRect.Left);
                //Key = 0;
                e.Handled = true;
            }
        }

        public void Tracks_Leave(object sender, EventArgs e)
        {
            if (AY.PlayMode == PlayModes.PlayLine && WaveOutAPI.IsPlaying && PlayingWindow[1] == this)
                WaveOutAPI.ResetPlaying();

            Tracks.KeyPressed = 0;
        }

        /* public void AutoStepEditExit(object sender, EventArgs e)
        {
            AutoStepEdit.Text = (AutoStepUpDown.Value).ToString();
        } */

        public bool DoStep(int i, bool StepForward, bool ForceAutoStep)
        {
            bool result = false;

            if (!AutoStep && !ForceAutoStep)
                return result;

            int t = (int)AutoStepUpDown.Value;

            if (t != 0)
            {
                t = StepForward ? t + i : i - t;

                if (t >= 0 && t < Tracks.ShownPattern.Length)
                {
                    result = true;
                    Tracks.ShownFrom = t;

                    if (Tracks.CursorY != Tracks.CenterLineIndex)
                    {
                        Tracks.CursorY = Tracks.CenterLineIndex;
                        Tracks.SetCaretPosition();
                    }

                    Tracks.RemoveSelection();
                }
            }

            return result;
        }

        public void SaveOrnamentFile(string fullPath)
        {
            int index;

            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine("[Ornament]");
                    VTM.SaveOrnament(streamWriter, VTM, (int)OrnamentNumUpDown.Value);

                    // User save ornament to a current FileBrowser directory
                    if (Path.GetDirectoryName(fullPath) == Path.GetDirectoryName(OrnamentsBrowser.CurrentDir))
                    {
                        OrnamentsBrowser.ReadDir();
                        index = OrnamentsBrowser.GetIndex(Path.GetFileName(fullPath));
                        /* if (index != -1)
                        {
                            OrnamentsBrowser.SetSelected(index, true);
                        }
                        else
                        {
                            OrnamentsBrowser.SetSelected(0, true);
                        } */
                    }
                }
            }
        }

        public void SaveOrnamentButton_Click(object sender, EventArgs e)
        {
            StopAndRestoreControls();

            SaveTextDialog.Title = "Save Ornament";
            SaveTextDialog.Filter = "Ornament Files|*.vto|Text Files|*.txt|All Files|*.*";
            SaveTextDialog.DefaultExt = "vto";
            SaveTextDialog.FileName = $"Ornament {OrnamentNumUpDown.Value}.vto";
            SaveTextDialog.InitialDirectory = OrnamentsDir;

            if (SaveTextDialog.ShowDialog() == DialogResult.OK)
            {
                OrnamentsDir = Path.GetDirectoryName(SaveTextDialog.FileName);
                SaveOrnamentFile(SaveTextDialog.FileName);
            }
        }

        public void LoadOrnamentButton_Click(object sender, EventArgs e)
        {
            StopAndRestoreControls();

            LoadTextDialog.Title = "Load Ornament";
            LoadTextDialog.Filter = "Ornament Files|*.vto|Text Files|*.txt|All Files|*.*";
            LoadTextDialog.DefaultExt = "vto";
            LoadTextDialog.FileName = "";
            LoadTextDialog.InitialDirectory = OrnamentsDir;

            if (LoadTextDialog.ShowDialog() == DialogResult.OK)
            {
                OrnamentsDir = Path.GetDirectoryName(LoadTextDialog.FileName);
                LoadOrnament(LoadTextDialog.FileName);
            }
        }

        public void LoadOrnament(string fileName, int index = -1)
        {
            if (!OrnamentLenUpDown.Enabled)
            {
                MessageBox.Show(this, "Stop Playing Before Loading Ornament", Application.ProductName);
                return;
            }

            string[] lines = File.ReadAllLines(fileName);
            int ornamentIndex = Array.FindIndex(lines, t => t.Equals("[ORNAMENT]", StringComparison.InvariantCultureIgnoreCase));

            if (ornamentIndex == -1)
            {
                MessageBox.Show(this, "Ornament Data Not Found", Application.ProductName);
                return;
            }

            string ornamentString = lines.Skip(ornamentIndex + 1).FirstOrDefault();
            Ornament ornament = new Ornament();

            if (!Ornament.RecognizeOrnamentString(ornamentString, ornament))
            {
                MessageBox.Show(this, "Bad File Structure", Application.ProductName);
            }
            else
            {
                if (index == -1)
                {
                    SongChanged = true;
                    BackupSongChanged = true;
                    AddUndo(TChangeAction.LoadOrnament, 0, 0);
                    ChangeList[ChangeCount - 1].Ornament = VTM.Ornaments[OrnamentIndex];
                    VTM.Ornaments[OrnamentIndex] = ornament;
                    Ornaments.CursorX = 0;
                    Ornaments.CursorY = 0;
                    Ornaments.ShownFrom = 0;
                    int ornamentIndex2 = OrnamentIndex;
                    OrnamentIndex = -1;
                    ChangeOrnament(ornamentIndex2, true, true);
                    ChangeList[ChangeCount - 1].NewParams.Params.OrnamentCursor = Ornaments.CursorY + Ornaments.CursorX / MainForm.OrnCharCount * Ornaments.NRaw;
                    ChangeList[ChangeCount - 1].NewParams.Params.OrnamentShownFrom = 0;
                }
                else
                {
                    if (VTM.Ornaments[index] != null)
                        VTM.Ornaments[index] = null;

                    VTM.Ornaments[index] = ornament;
                }
            }
        }

        public void LoadOrnament(string fileName)
        {
            LoadOrnament(fileName, -1);
        }

        /* public void OrGenButton_Click(object sender, EventArgs e)
        {
            const string fileName = "VTIITempOrnament.txt";

            if (OrGenRunning)
                return;

            string tempPath = Path.GetTempPath();
            string fullPath = Path.Combine(tempPath, fileName);

            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                }
                catch
                {
                    MessageBox.Show(this, "Plug-In Communication Error: Cannot Delete File.", Application.ProductName);
                    return;
                }
            }

            string exeDir = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
            string exePath = Path.Combine(exeDir, "OrGen.exe");

            if (!File.Exists(exePath))
            {
                MessageBox.Show(this, "Plug-In Not Found: OrGen.exe", Application.ProductName);
                return;
            }

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = fileName,
                WorkingDirectory = exeDir,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (Process process = Process.Start(psi))
                {
                    OrGenRunning = true;
                    OrGenButton.Enabled = false;

                    process.WaitForExit();

                    OrGenRunning = false;
                    OrGenButton.Enabled = true;

                    if (File.Exists(fullPath))
                    {
                        LoadOrnament(fullPath);
                        ChangeList[ChangeCount - 1].Action = TChangeAction.OrGen;
                        File.Delete(fullPath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Plug-in Communication Error: {ex.Message}", Application.ProductName);
                OrGenRunning = false;
                OrGenButton.Enabled = true;
            }
        } */

        public void ChildWin_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool canClose = !(SongChanged || (TSWindow[0] != null && TSWindow[0].SongChanged || TSWindow[1] != null && TSWindow[1].SongChanged));

            // Save changes dialog
            if (!canClose)
            {
                DialogResult result = MessageBox.Show(this, $"Edition {this.Text} is Changed. Save it Now?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                canClose = (result == DialogResult.Yes || result == DialogResult.No);

                if (result == DialogResult.Yes)
                    SaveModule();
            }

            if (canClose)
            {
                Globals.MainForm.RedrawOff();
                IsClosed = true;
                SongChanged = false;
                BackupSongChanged = false;

                if (TSWindow[0] != null && !TSWindow[0].IsClosed)
                {
                    TSWindow[0].SongChanged = false;
                    TSWindow[0].BackupSongChanged = false;
                    TSWindow[0].IsClosed = true;
                    MainForm.DrawOffAfterClose = true;

                    // When application exit, we no need to close second child manually.
                    // Otherwise there will be crash
                    if (MainForm.SysCmd != SC_CLOSE)
                        TSWindow[0].Close();
                    // DrawOffAfterClose := False;
                }

                if (TSWindow[1] != null && !TSWindow[1].IsClosed)
                {
                    TSWindow[1].SongChanged = false;
                    TSWindow[1].BackupSongChanged = false;
                    TSWindow[1].IsClosed = true;
                    MainForm.DrawOffAfterClose = true;

                    // When application exit, we no need to close second child manually.
                    // Otherwise there will be crash
                    if (MainForm.SysCmd != SC_CLOSE)
                        TSWindow[1].Close();
                    // DrawOffAfterClose := False;
                }
            }
        }

        public void ToggleAutoStep()
        {
            AutoStep = !AutoStep;
            AutoStepButton.Checked = AutoStep;
        }

        public void AutoStepBtn_Click(object sender, EventArgs e)
        {
            ToggleAutoStep();
        }

        public void OrnamentCopyToUpDownChangingEx(object sender, ValueChangingEventArgs e)
        {
            e.Cancel = e.NewValue < 0 || e.NewValue > 15;
        }

        public void CopyOrnamentButton_Click(object sender, EventArgs e)
        {
            CopyOrnamentToBuffer(true);
        }

        public void SampleCopyToUpDownChangingEx(object sender, ValueChangingEventArgs e)
        {
            e.Cancel = e.NewValue < 0 || e.NewValue > 31;
        }

        public void CopySampleButton_Click(object sender, EventArgs e)
        {
            CopySampleToBuffer(true);
        }

        public void SaveSampleFile(string fullPath)
        {
            int index;

            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine("[Sample]");
                    VTM.SaveSample(streamWriter, VTM, (int)SampleNumUpDown.Value);

                    // User save sample to a current FileBrowser directory
                    if (Path.GetDirectoryName(fullPath) == Path.GetDirectoryName(SamplesBrowser.CurrentDir))
                    {
                        SamplesBrowser.ReadDir();
                        index = SamplesBrowser.GetIndex(Path.GetFileName(fullPath));
                        /* if (index != -1)
                        {
                            SamplesBrowser.SetSelected(index, true);
                        }
                        else
                        {
                            SamplesBrowser.SetSelected(0, true);
                        } */
                    }
                }
            }
        }

        public void SaveSampleButton_Click(object sender, EventArgs e)
        {
            StopAndRestoreControls();

            SaveTextDialog.Title = "Save Sample";
            SaveTextDialog.Filter = "Sample Files|*.vts|Text Files|*.txt|All Files|*.*";
            SaveTextDialog.DefaultExt = "vts";
            SaveTextDialog.FileName = $"Sample {SampleNumUpDown.Value}.vts";
            SaveTextDialog.InitialDirectory = SamplesDir;

            if (SaveTextDialog.ShowDialog() == DialogResult.OK)
            {
                SamplesDir = Path.GetDirectoryName(SaveTextDialog.FileName);
                SaveSampleFile(SaveTextDialog.FileName);
            }
        }

        public void LoadSampleButton_Click(object sender, EventArgs e)
        {
            StopAndRestoreControls();

            LoadTextDialog.Title = "Load Sample";
            LoadTextDialog.Filter = "Sample Files|*.vts|Text Files|*.txt|All Files|*.*";
            LoadTextDialog.DefaultExt = "vts";
            LoadTextDialog.FileName = "";
            LoadTextDialog.InitialDirectory = SamplesDir;

            if (LoadTextDialog.ShowDialog() == DialogResult.OK)
            {
                SamplesDir = Path.GetDirectoryName(LoadTextDialog.FileName);
                LoadSample(LoadTextDialog.FileName);
            }
        }

        public void LoadSample(string fileName, int index = -1)
        {
            if (!SampleLengthUpDown.Enabled)
            {
                MessageBox.Show(this, "Stop Playing Before Loading Sample", Application.ProductName);
                return;
            }

            string[] lines = File.ReadAllLines(fileName);
            int sampleIndex = Array.FindIndex(lines, t => t.Equals("[SAMPLE]", StringComparison.InvariantCultureIgnoreCase));

            if (sampleIndex == -1)
            {
                MessageBox.Show(this, "Sample Data Not Found", Application.ProductName);
                return;
            }

            if (Sample.LoadSampleDataTxt(lines.Skip(sampleIndex + 1).ToArray(), out Sample sample, false) != 0)
            {
                MessageBox.Show(this, "Bad File Structure", Application.ProductName);
                return;
            }

            if (index == -1)
            {
                SongChanged = true;
                BackupSongChanged = true;

                AddUndo(TChangeAction.LoadSample, 0, 0);
                ChangeList[ChangeCount - 1].Sample = VTM.Samples[SampleIndex];

                VTM.Samples[SampleIndex] = sample;
                ValidateSample2(SampleIndex);

                Samples.ShownFrom = 0;
                Samples.CursorX = 0;
                Samples.CursorY = 0;
                ChangeSample(SampleIndex, false, true);

                SampleLengthUpDown.Value = VTM.Samples[SampleIndex].Length;
                SampleLoopUpDown.Value = VTM.Samples[SampleIndex].Loop;

                ChangeList[ChangeCount - 1].NewParams.Params.SampleCursorX = 0;
                ChangeList[ChangeCount - 1].NewParams.Params.SampleCursorY = 0;
                ChangeList[ChangeCount - 1].NewParams.Params.SampleShownFrom = 0;
            }
            else
            {
                if (VTM.Samples[index] != null)
                    VTM.Samples[index] = null;

                VTM.Samples[index] = sample;
                ValidateSample2(index);
            }
        }

        public void LoadSample(string fileName)
        {
            LoadSample(fileName, -1);
        }

        public void LoadPatternButton_Click(object sender, EventArgs e)
        {
            LoadTextDialog.Title = "Load Pattern From Text File";
            LoadTextDialog.Filter = "Pattern Files|*.vtp|Text Files|*.txt|All Files|*.*";
            LoadTextDialog.DefaultExt = "vtp";

            if (LoadTextDialog.ShowDialog() == DialogResult.OK)
            {
                LoadTextDialog.InitialDirectory = Path.GetDirectoryName(LoadTextDialog.FileName);
                SaveTextDialog.InitialDirectory = LoadTextDialog.InitialDirectory;
                LoadPattern(LoadTextDialog.FileName);
            }
        }

        public void LoadPattern(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);
            int patternIndex = Array.FindIndex(lines, t => t.Equals("[PATTERN]", StringComparison.InvariantCultureIgnoreCase));

            if (patternIndex == -1)
            {
                MessageBox.Show(this, "Pattern Data Not Found", Application.ProductName);
                return;
            }

            // Find noise mode (DECNOISE or HEXNOISE)
            int noiseModeIndex = Array.FindIndex(lines, patternIndex + 1, l =>
                l.Equals("DECNOISE", StringComparison.InvariantCultureIgnoreCase) ||
                l.Equals("HEXNOISE", StringComparison.InvariantCultureIgnoreCase));

            if (noiseModeIndex == -1)
            {
                MessageBox.Show(this, "Noise Mode (DECNOISE/HEXNOISE) Not Specified", Application.ProductName);
                return;
            }

            bool decNoise = string.Equals(lines[noiseModeIndex], "DECNOISE", StringComparison.InvariantCultureIgnoreCase);

            // Extract actual pattern data
            string[] patternData = lines.Skip(noiseModeIndex + 1).ToArray();

            // Load pattern data
            if (Pattern.LoadPatternDataTxt(patternData, out Pattern pattern, decNoise) != 0)
            {
                MessageBox.Show(this, "Bad File Structure", Application.ProductName);
                return;
            }

            VTModule.ValidatePattern(PatternIndex, VTM);
            AddUndo(TChangeAction.LoadPattern, 0, 0);
            ChangeList[ChangeCount - 1].Pattern = VTM.Patterns[PatternIndex];
            VTM.Patterns[PatternIndex] = pattern;

            SongChanged = true;
            BackupSongChanged = true;
            ChangePattern(PatternIndex);
            ChangeList[ChangeCount - 1].NewParams.Params.PatternCursorY = Tracks.CursorY;
            ChangeList[ChangeCount - 1].NewParams.Params.PatternShownFrom = Tracks.ShownFrom;
        }

        public void SavePatternButton_Click(object sender, EventArgs e)
        {
            int p = (int)PatternNumUpDown.Value;

            SaveTextDialog.Title = "Save Pattern in Text File";
            SaveTextDialog.Filter = "Pattern Files|*.vtp|Text Files|*.txt|All Files|*.*";
            SaveTextDialog.DefaultExt = "vtp";
            SaveTextDialog.FileName = $"Pattern {p.ToString()}.vtp";

            if (SaveTextDialog.ShowDialog() == DialogResult.OK)
            {
                SaveTextDialog.InitialDirectory = Path.GetDirectoryName(SaveTextDialog.FileName);
                LoadTextDialog.InitialDirectory = SaveTextDialog.InitialDirectory;

                using (FileStream fileStream = new FileStream(SaveTextDialog.FileName, FileMode.Create))
                {
                    using (TextWriter textWriter = new StreamWriter(fileStream))
                    {
                        textWriter.WriteLine("[Pattern]");

                        if (MainForm.DecBaseNoiseOn)
                            textWriter.WriteLine("DecNoise");
                        else
                            textWriter.WriteLine("HexNoise");

                        VTM.SavePattern(textWriter, VTM, p, MainForm.TracksCursorXLeft);
                    }
                }
            }
        }

        public void CopyToFamiTracker()
        {
            if (!Tracks.IsSelected())
                return;

            MainForm.LastClipboard = LastClipboard.None;

            int x1 = Math.Min(Tracks.SelectionX, Tracks.CursorX);
            int x2 = Math.Max(Tracks.SelectionX, Tracks.CursorX);
            int y1 = Math.Min(Tracks.SelectionY, Tracks.CurrentPatternLine());
            int y2 = Math.Max(Tracks.SelectionY, Tracks.CurrentPatternLine());

            int fromChan = GetChannelFromCursorPosition(x1);
            int toChan = GetChannelFromCursorPosition(x2);

            var famiClipboard = new FamiTrackerBuffer
            {
                Header = {
                    Channels = toChan - fromChan + 1,
                    Rows = y2 - y1 + 1,
                    SelectStart = 0,
                    SelectEnd = 3,
                    Undefined1 = 0,
                    Undefined2 = 0,
                },
                Data = new FamiRow[(toChan - fromChan + 1) * (y2 - y1 + 1)]
            };

            int famiRowsCount = 0;

            for (int curChan = fromChan; curChan <= toChan; curChan++)
            {
                for (int patLine = y1; patLine <= y2; patLine++)
                {
                    var channelLine = VTM.Patterns[PatternIndex].Lines[patLine].Channel[MainForm.ChanAlloc[curChan]];
                    var famiRow = new FamiRow();

                    // Note
                    if (channelLine.Note != -1)
                    {
                        string s = VTModule.NoteToStr(channelLine.Note);
                        if (s == "R--")
                        {
                            famiRow.Note = 0x0E;
                        }
                        else
                        {
                            int num = Array.IndexOf(VTModule.FamiNotes, s.Substring(0, 2));
                            famiRow.Note = (byte)((num + 1) / 2);
                            famiRow.Octave = byte.Parse(s.Substring(2, 1));
                        }
                    }

                    // Sample
                    famiRow.Instrument = channelLine.Sample > 0 ? (byte)(channelLine.Sample - 1) : (byte)0x40;

                    // Volume
                    famiRow.NoteVolume = channelLine.Volume > 0 ? (byte)channelLine.Volume : (byte)0x10;

                    // Command
                    switch (channelLine.AdditionalCommand.Number)
                    {
                        case 2: // Tone slide up
                            famiRow.Fx1Cmd = 0x10;
                            famiRow.Fx1Prm = channelLine.AdditionalCommand.Parameter;
                            break;
                        case 1: // Tone slide down
                            famiRow.Fx1Cmd = 0x11;
                            famiRow.Fx1Prm = channelLine.AdditionalCommand.Parameter;
                            break;
                        case 3: // Portamento
                            famiRow.Fx1Cmd = 0x06;
                            famiRow.Fx1Prm = channelLine.AdditionalCommand.Parameter;
                            break;
                        case 6: // Vibrato
                            famiRow.Fx1Cmd = 0x0B;
                            famiRow.Fx1Prm = channelLine.AdditionalCommand.Parameter;
                            break;
                        case 0xB: // Speed
                            famiRow.Fx1Cmd = 0x01;
                            famiRow.Fx1Prm = channelLine.AdditionalCommand.Parameter;
                            break;
                        default:
                            famiRow.Fx1Cmd = 0;
                            famiRow.Fx1Prm = 0;
                            break;
                    }

                    famiClipboard.Data[famiRowsCount++] = famiRow;
                }
            }

            int headerSize = Marshal.SizeOf(typeof(FamiTrackerBufferHeader));
            int rowSize = Marshal.SizeOf(typeof(FamiRow));
            int dataSize = headerSize + famiClipboard.Data.Length * rowSize;

            byte[] buffer = new byte[dataSize];

            // Copy header to byte array
            GCHandle headerHandle = GCHandle.Alloc(famiClipboard.Header, GCHandleType.Pinned);
            try
            {
                IntPtr headerPtr = headerHandle.AddrOfPinnedObject();
                Marshal.Copy(headerPtr, buffer, 0, headerSize);
            }
            finally
            {
                headerHandle.Free();
            }

            // Copy each row to byte array
            for (int i = 0; i < famiClipboard.Data.Length; i++)
            {
                int offset = headerSize + i * rowSize;
                GCHandle rowHandle = GCHandle.Alloc(famiClipboard.Data[i], GCHandleType.Pinned);
                try
                {
                    IntPtr rowPtr = rowHandle.AddrOfPinnedObject();
                    Marshal.Copy(rowPtr, buffer, offset, rowSize);
                }
                finally
                {
                    rowHandle.Free();
                }
            }

            // Place the buffer into the clipboard with your custom format
            DataObject dataObj = new DataObject();
            dataObj.SetData(VTModule.FamiClipboardFormatName, false, buffer);
            Clipboard.SetDataObject(dataObj, true);

            Tracks.RemoveSelection();
            Tracks.HideCaret();
            Tracks.RedrawTracks();
            Tracks.ShowCaret();
        }

        private int GetChannelFromCursorPosition(int cursorX)
        {
            if (cursorX <= 20)
                return 0;
            else if (cursorX > 20 && cursorX <= 34)
                return 1;
            else if (cursorX > 34 && cursorX <= 48)
                return 2;
            else
                return 0;
        }

        public void CopyToModplug()
        {
            string res;
            StringBuilder stringBuilder;
            string s;
            int x1, x2, y1, y2, patLine;
            int fromChan, toChan, curChan;
            ChannelLine channelLine;

            if (!Tracks.IsSelected())
                return;

            MainForm.LastClipboard = LastClipboard.None;
            res = "ModPlug Tracker MOD\r\n";
            Clipboard.Clear();

            x2 = Tracks.CursorX;
            x1 = Tracks.SelectionX;

            if (x1 > x2)
            {
                x1 = x2;
                x2 = Tracks.SelectionX;
            }

            y1 = Tracks.SelectionY;
            y2 = Tracks.CurrentPatternLine();

            if (y1 > y2)
            {
                y1 = y2;
                y2 = Tracks.SelectionY;
            }

            fromChan = 0;

            if (x1 <= 20)
                fromChan = 0;

            if (x1 > 20 && x1 <= 34)
                fromChan = 1;

            if (x1 > 34 && x1 <= 48)
                fromChan = 2;

            toChan = 0;

            if (x2 <= 20)
                toChan = 0;

            if (x2 > 20 && x2 <= 34)
                toChan = 1;

            if (x2 > 34 && x2 <= 48)
                toChan = 2;

            // Note poses [8, 22, 36]
            for (patLine = y1; patLine <= y2; patLine++)
            {
                for (curChan = fromChan; curChan <= toChan; curChan++)
                {
                    stringBuilder = new StringBuilder("|...........");
                    channelLine = VTM.Patterns[PatternIndex].Lines[patLine].Channel[MainForm.ChanAlloc[curChan]];

                    // Note
                    if (channelLine.Note != -1)
                    {
                        s = VTModule.NoteToStr(channelLine.Note);

                        if (s == "R--")
                        {
                            stringBuilder[2] = '^';
                            stringBuilder[3] = '^';
                            stringBuilder[4] = '^';
                        }
                        else
                        {
                            stringBuilder[2] = s[1];
                            stringBuilder[3] = s[2];
                            stringBuilder[4] = s[3];
                        }
                    }

                    // Sample
                    if (channelLine.Sample > 0)
                    {
                        s = string.Format("%.2d", channelLine.Sample);
                        stringBuilder[5] = s[1];
                        stringBuilder[6] = s[2];
                    }

                    // Volume
                    if (channelLine.Volume > 0)
                    {
                        s = string.Format("%.2d", channelLine.Volume * 64 / 0xF);
                        stringBuilder[7] = 'v';
                        stringBuilder[8] = s[1];
                        stringBuilder[9] = s[2];
                    }

                    // Command
                    if (channelLine.AdditionalCommand.Number > 0)
                    {
                        s = string.Format("%.2x", channelLine.AdditionalCommand.Parameter);
                        stringBuilder[11] = s[1];
                        stringBuilder[12] = s[2];

                        switch (channelLine.AdditionalCommand.Number)
                        {
                            case 2:
                                stringBuilder[10] = '1';
                                break;
                            case 1:
                                // Tone Slide Up
                                stringBuilder[10] = '2';
                                break;
                            case 3:
                                // Tone Slide Down
                                stringBuilder[10] = '3';
                                break;
                            case 6:
                                // Tone Portamento
                                stringBuilder[10] = '4';
                                break;
                            case 0xB:
                                // Vibrato
                                stringBuilder[10] = 'F';
                                break;
                                // Set Speed
                        }
                    }
                    res += stringBuilder.ToString();
                }

                res += "\r\n";
            }

            Clipboard.SetText(res);

            Tracks.RemoveSelection();
            Tracks.HideCaret();
            Tracks.RedrawTracks();
            Tracks.ShowCaret();
        }

        public void CopyToRenoise()
        {
            string res;
            int i, x1, x2, y1, y2, patLine, lineIndex;
            int fromChan, toChan, curChan;
            ChannelLine channelLine;

            if (!Tracks.IsSelected())
                return;

            MainForm.LastClipboard = LastClipboard.None;
            res = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n";
            res += "<PatternClipboard.BlockBuffer doc_version=\"0\">\r\n";
            res += "<Columns>\r\n";

            Clipboard.Clear();

            x2 = Tracks.CursorX;
            x1 = Tracks.SelectionX;

            if (x1 > x2)
            {
                x1 = x2;
                x2 = Tracks.SelectionX;
            }

            y1 = Tracks.SelectionY;
            y2 = Tracks.CurrentPatternLine();

            if (y1 > y2)
            {
                y1 = y2;
                y2 = Tracks.SelectionY;
            }

            // Note poses [8, 22, 36]
            fromChan = 0;

            if (x1 <= 20)
                fromChan = 0;

            if (x1 > 20 && x1 <= 34)
                fromChan = 1;

            if (x1 > 34 && x1 <= 48)
                fromChan = 2;

            toChan = 0;

            if (x2 <= 20)
                toChan = 0;

            if (x2 > 20 && x2 <= 34)
                toChan = 1;

            if (x2 > 34 && x2 <= 48)
                toChan = 2;

            for (curChan = fromChan; curChan <= toChan; curChan++)
            {
                res += "<Column><Column>\r\n";
                res += "<Lines>\r\n";

                lineIndex = 0;

                for (patLine = y1; patLine <= y2; patLine++)
                {
                    channelLine = VTM.Patterns[PatternIndex].Lines[patLine].Channel[MainForm.ChanAlloc[curChan]];

                    if (channelLine.Note == -1 && channelLine.Sample == 0 && channelLine.Volume == 0)
                    {
                        res += "<Line/>\r\n";
                        lineIndex++;
                        continue;
                    }

                    res += string.Format("<Line index=\"%d\">\r\n", lineIndex);
                    res += "<NoteColumns><NoteColumn>\r\n";
                    lineIndex++;

                    // Note
                    if (channelLine.Note == -2)
                        res += "<Note>OFF</Note>";
                    else if (channelLine.Note != -1)
                        res += "<Note>" + VTModule.NoteToStr(channelLine.Note) + "</Note>";

                    // Sample
                    if (channelLine.Sample > 0)
                        res += string.Format("<Instrument>%.2d</Instrument>", channelLine.Sample - 1);

                    // Volume
                    if (channelLine.Volume > 0)
                        res += string.Format("<Volume>%.2d</Volume>\r\n", channelLine.Volume * 99 / 0xF);

                    res += "</NoteColumn></NoteColumns></Line>\r\n";
                }

                res += "</Lines>\r\n";
                res += "<ColumnType>NoteColumn</ColumnType>\r\n";
                res += "<SubColumnMask>true true true false false false false false</SubColumnMask>\r\n";
                res += "</Column>\r\n";

                // Effects column
                res += "<Column><Lines>\r\n";

                for (i = 0; i <= lineIndex; i++)
                    res += "<Line/>\r\n";

                res += "</Lines>\r\n";
                res += "<ColumnType>EffectColumn</ColumnType>\r\n";
                res += "<SubColumnMask>false false false false false true true false</SubColumnMask>\r\n";
                res += "</Column>\r\n";
                res += "</Column>\r\n";
            }

            res += "</Columns>\r\n";
            res += "</PatternClipboard.BlockBuffer>\r\n";

            Clipboard.SetText(res);

            Tracks.RemoveSelection();
            Tracks.HideCaret();
            Tracks.RedrawTracks();
            Tracks.ShowCaret();
        }

        public void PasteFamiTrackerPattern()
        {
            if (!Clipboard.ContainsData(VTModule.FamiClipboardFormatName))
                return;

            if (!(Clipboard.GetData(VTModule.FamiClipboardFormatName) is byte[] buffer) || buffer.Length < Marshal.SizeOf<FamiTrackerBufferHeader>())
                return;

            SavePatternUndo();

            FamiTrackerBufferHeader header;
            FamiRow[] rows;

            int headerSize = Marshal.SizeOf<FamiTrackerBufferHeader>();
            int rowSize = Marshal.SizeOf<FamiRow>();
            int rowCount = (buffer.Length - headerSize) / rowSize;

            // --- Deserialize header ---
            GCHandle headerHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                IntPtr headerPtr = headerHandle.AddrOfPinnedObject();
                header = Marshal.PtrToStructure<FamiTrackerBufferHeader>(headerPtr);
            }
            finally
            {
                headerHandle.Free();
            }

            // --- Deserialize rows ---
            rows = new FamiRow[rowCount];
            for (int i = 0; i < rowCount; i++)
            {
                int offset = headerSize + i * rowSize;
                GCHandle rowHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                try
                {
                    IntPtr rowPtr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, offset);
                    rows[i] = Marshal.PtrToStructure<FamiRow>(rowPtr);
                }
                finally
                {
                    rowHandle.Free();
                }
            }

            // Paste to Vortex patterns:
            int startChan = 0;
            if (Tracks.CursorX <= 20)
                startChan = 0;
            else if (Tracks.CursorX > 20 && Tracks.CursorX <= 34)
                startChan = 1;
            else if (Tracks.CursorX > 34 && Tracks.CursorX <= 48)
                startChan = 2;

            int famiLine = 0;
            int curChan = startChan;

            for (int j = 0; j < header.Channels; j++)
            {
                int patLine = Tracks.CurrentPatternLine();

                for (int i = 0; i < header.Rows; i++)
                {
                    var channelLine = VTM.Patterns[PatternIndex].Lines[patLine].Channel[MainForm.ChanAlloc[curChan]];
                    FamiRow famiRow = rows[famiLine++];

                    channelLine.Ornament = 0;
                    channelLine.Envelope = 0;

                    if (famiRow.Note == 0 && famiRow.Octave == 0)
                        channelLine.Note = -1;
                    else if (famiRow.Note == 14)
                        channelLine.Note = -2;
                    else
                    {
                        string str = VTModule.FamiNotes[famiRow.Note * 2 - 1] + VTModule.FamiNotes[famiRow.Note * 2] + famiRow.Octave.ToString();
                        channelLine.Note = (sbyte)VTModule.SGetNote2(str);
                    }

                    // Sample
                    channelLine.Sample = famiRow.Instrument != 64
                        ? (byte)((famiRow.Instrument + 1) & 31)
                        : (byte)0;

                    // Volume
                    channelLine.Volume = famiRow.NoteVolume <= 0xF
                        ? (sbyte)famiRow.NoteVolume
                        : (sbyte)0;

                    channelLine.AdditionalCommand.Number = 0;
                    channelLine.AdditionalCommand.Delay = 0;
                    channelLine.AdditionalCommand.Parameter = 0;

                    // Command
                    switch (famiRow.Fx1Cmd)
                    {
                        // Tone slide up
                        case 0x10:
                            channelLine.AdditionalCommand.Number = 2;
                            channelLine.AdditionalCommand.Parameter = famiRow.Fx1Prm;
                            break;
                        // Tone slide down
                        case 0x11:
                            channelLine.AdditionalCommand.Number = 1;
                            channelLine.AdditionalCommand.Parameter = famiRow.Fx1Prm;
                            break;
                        // Portamento
                        case 0x06:
                            channelLine.AdditionalCommand.Number = 3;
                            channelLine.AdditionalCommand.Parameter = famiRow.Fx1Prm;
                            break;
                        // Vibrato
                        case 0x0B:
                            channelLine.AdditionalCommand.Number = 6;
                            channelLine.AdditionalCommand.Parameter = famiRow.Fx1Prm;
                            break;
                        // Set Volume
                        case 0x05:
                            channelLine.Volume = (sbyte)famiRow.Fx1Prm;
                            break;
                        // Set Speed
                        case 0x01:
                            if (famiRow.Fx1Prm < 0x20)
                            {
                                channelLine.AdditionalCommand.Number = 0xB;
                                channelLine.AdditionalCommand.Parameter = famiRow.Fx1Prm;
                            }
                            break;
                    }

                    patLine++;
                    if (patLine >= VTM.Patterns[PatternIndex].Length)
                        break;
                }

                if (curChan == 2)
                    break;
                curChan++;
            }

            SavePatternRedo();
            SongChanged = true;
            BackupSongChanged = true;

            Tracks.HideCaret();
            Tracks.RedrawTracks();
            Tracks.ShowCaret();
        }

        public void PasteModPlugPattern(string txt)
        {
            int i, j, num, curChan, startChan, patLine;
            sbyte note;
            List<string> lines, chanLines;
            ChannelLine channelLine;
            string modType = "MOD";

            if (txt.IndexOf("MOD") != -1)
                modType = "MOD";

            if (txt.IndexOf("S3M") != -1)
                modType = "S3M";

            if (txt.IndexOf("XM") != -1)
                modType = "XM";

            if (txt.IndexOf("MPT") != -1)
                modType = "MPT";

            SavePatternUndo();

            startChan = 0;

            if (Tracks.CursorX <= 20)
                startChan = 0;

            if (Tracks.CursorX > 20 && Tracks.CursorX <= 34)
                startChan = 1;

            if (Tracks.CursorX > 34 && Tracks.CursorX <= 48)
                startChan = 2;

            patLine = Tracks.CurrentPatternLine();
            lines = new List<string>();
            chanLines = new List<string>();
            lines.AddRange(Regex.Split(txt, @"\r?\n"));

            for (i = 0; i < lines.Count; i++)
            {
                chanLines.Clear();

                // Remove more than 3 channels
                lines[i] = Regex.Replace(lines[i], @"^(\|[^|]+)(\|[^|]+)(\|[^|]+).*$", "$1$2$3", RegexOptions.IgnoreCase);

                // Get parts for each channel
                string[] parts = Regex.Split(lines[i], @"\|");
                chanLines.AddRange(parts.Skip(1));
                curChan = startChan;

                for (j = 0; j < chanLines.Count; j++)
                {
                    channelLine = VTM.Patterns[PatternIndex].Lines[patLine].Channel[MainForm.ChanAlloc[curChan]];
                    channelLine.Ornament = 0;
                    channelLine.Envelope = 0;

                    // Note
                    if (chanLines[j][1] == '\0')
                        break;

                    if (chanLines[j][1] == '.' || chanLines[j][1] == ' ')
                        note = -1;
                    else if (chanLines[j][1] == '^')
                        note = -2;
                    else
                        note = (sbyte)VTModule.SGetNote2(chanLines[j].Substring(0, 3));

                    channelLine.Note = note;

                    // Sample
                    if (chanLines[j][4] == '\0')
                        break;

                    if (!(chanLines[j][4] == '.' || chanLines[j][4] == ' ') && !(chanLines[j][5] == '.' || chanLines[j][5] == ' '))
                    {
                        num = Convert.ToInt32(chanLines[j][4] + chanLines[j][5]) & 31;
                        channelLine.Sample = (byte)num;
                    }
                    else
                        channelLine.Sample = 0;

                    // Volume
                    if (chanLines[j][7] == '\0')
                        break;

                    if (!(chanLines[j][7] == '.' || chanLines[j][7] == ' ') && !(chanLines[j][8] == '.' || chanLines[j][8] == ' '))
                    {
                        num = Convert.ToInt32(chanLines[j][7] + chanLines[j][8]);
                        channelLine.Volume = (sbyte)(0xF * num / 64);
                    }
                    else
                        channelLine.Volume = 0;

                    // Command
                    if (chanLines[j][9] == '\0')
                        break;

                    if (!(chanLines[j][9] == '.' || chanLines[j][9] == ' ') && !(chanLines[j][10] == '.' || chanLines[j][10] == ' ') && !(chanLines[j][11] == '.' || chanLines[j][11] == ' '))
                    {
                        if (modType == "MOD" || modType == "XM")
                        {
                            switch (chanLines[j][9])
                            {
                                case '1':
                                    // Tone Slide Up
                                    channelLine.AdditionalCommand.Number = 2;
                                    break;
                                case '2':
                                    // Tone Slide Down
                                    channelLine.AdditionalCommand.Number = 1;
                                    break;
                                case '3':
                                    // Tone Portamento
                                    channelLine.AdditionalCommand.Number = 3;
                                    break;
                                case '4':
                                    // Vibrato
                                    channelLine.AdditionalCommand.Number = 6;
                                    break;
                                case 'F':
                                    // Speed
                                    channelLine.AdditionalCommand.Number = 0xb;
                                    break;
                            }
                        }

                        if (modType == "S3M" || modType == "IT" || modType == "MPT")
                        {
                            switch (chanLines[j][9])
                            {
                                case 'F':
                                    // Tone Slide Up
                                    channelLine.AdditionalCommand.Number = 2;
                                    break;
                                case 'E':
                                    // Tone Slide Down
                                    channelLine.AdditionalCommand.Number = 1;
                                    break;
                                case 'G':
                                    // Tone Portamento
                                    channelLine.AdditionalCommand.Number = 3;
                                    break;
                                case 'H':
                                    // Vibrato
                                    channelLine.AdditionalCommand.Number = 6;
                                    break;
                                case 'A':
                                    // Speed
                                    channelLine.AdditionalCommand.Number = 0xb;
                                    break;
                            }
                        }

                        if (channelLine.AdditionalCommand.Number != 0)
                            channelLine.AdditionalCommand.Parameter = (byte)Convert.ToInt32(chanLines[j].Substring(10, 2), 16);

                        // Remove Set Tempo command
                        if (channelLine.AdditionalCommand.Number == 0xb && channelLine.AdditionalCommand.Parameter > 0x20)
                        {
                            channelLine.AdditionalCommand.Number = 0;
                            channelLine.AdditionalCommand.Parameter = 0;
                        }
                        // if Ord(ChanLines[j][9]) > Ord('F') then
                        // num := 0
                        // else
                        // num := StrToInt('$' + ChanLines[j][9]) and $F;
                        // ChannelLine.Additional_Command.Delay := num;
                        // num := StrToInt('$' + ChanLines[j][10] + ChanLines[j][11]) and $FF;
                        // ChannelLine.Additional_Command.Parameter := num;
                    }
                    else
                    {
                        channelLine.AdditionalCommand.Number = 0;
                        channelLine.AdditionalCommand.Delay = 0;
                        channelLine.AdditionalCommand.Parameter = 0;
                    }

                    if (curChan == 2)
                        break;

                    curChan++;
                }

                patLine++;

                if (patLine == VTM.Patterns[PatternIndex].Length)
                    break;
            }

            SavePatternRedo();

            SongChanged = true;
            BackupSongChanged = true;

            Tracks.HideCaret();
            Tracks.RedrawTracks();
            Tracks.ShowCaret();
        }

        public string PasteRenoisePattern_Match(string pattern, string input)
        {
            var match = Regex.Match(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (match.Success && match.Groups.Count > 1)
                return match.Groups[1].Value.Trim();

            return "";
        }

        public void PasteRenoisePattern(string txt)
        {
            SavePatternUndo();

            int startChan = 0;

            if (Tracks.CursorX > 20 && Tracks.CursorX <= 34)
                startChan = 1;

            if (Tracks.CursorX > 34 && Tracks.CursorX <= 48)
                startChan = 2;

            // Remove unnecessary tags using Regex
            txt = Regex.Replace(txt, "<\\?xml[^>]+>", "", RegexOptions.IgnoreCase);
            txt = Regex.Replace(txt, "</?PatternClipboard[^>]+>", "", RegexOptions.IgnoreCase);
            txt = Regex.Replace(txt, "</?Columns?>", "", RegexOptions.IgnoreCase);
            txt = Regex.Replace(txt, "</?Lines>", "", RegexOptions.IgnoreCase);
            txt = Regex.Replace(txt, "<SubColumnMask>[^<]+</SubColumnMask>", "", RegexOptions.IgnoreCase);
            txt = Regex.Replace(txt, "\\s+", "", RegexOptions.IgnoreCase);

            // Split into channel data by column
            txt = Regex.Replace(txt, "</ColumnType>", "</ColumnType>####", RegexOptions.IgnoreCase);
            string[] chanLines = txt.Split(new[] { "####" }, StringSplitOptions.RemoveEmptyEntries);

            // Keep only NoteColumn types
            List<string> validChannels = new List<string>();

            foreach (var ch in chanLines)
            {
                if (Regex.IsMatch(ch, "<ColumnType>NoteColumn</ColumnType>", RegexOptions.IgnoreCase))
                    validChannels.Add(ch);
            }

            int curChan = startChan;

            foreach (string chan in validChannels)
            {
                int patLine = Tracks.CurrentPatternLine();
                List<string> lines = Regex.Split(chan, "<Line[^>]+>", RegexOptions.IgnoreCase).ToList();

                if (lines.Count > 0)
                    lines.RemoveAt(0); // First element before the first <Line>

                foreach (string line in lines)
                {
                    ChannelLine channelLine = VTM.Patterns[PatternIndex].Lines[patLine].Channel[MainForm.ChanAlloc[curChan]];
                    channelLine.Ornament = 0;
                    channelLine.Envelope = 0;

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        channelLine.Note = -1;
                        channelLine.Sample = 0;
                        channelLine.Volume = 0;

                        patLine++;

                        if (patLine >= VTM.Patterns[PatternIndex].Length)
                            break;

                        continue;
                    }

                    string str;
                    sbyte note = -1, sample = 0, volume = 0;

                    str = PasteRenoisePattern_Match("<Note>([^<]+)</Note>", line);

                    if (str == "OFF")
                        note = -2;
                    else if (str.Length == 3)
                        note = (sbyte)VTModule.SGetNote2(str);

                    str = PasteRenoisePattern_Match("<Instrument>(\\d+)</Instrument>", line);

                    if (int.TryParse(str, out int sampleVal) && sampleVal >= 0)
                        sample = (sbyte)((sampleVal + 1) & 31);

                    str = PasteRenoisePattern_Match("<Volume>(\\d+)</Volume>", line);

                    if (int.TryParse(str, out int volVal) && volVal >= 0)
                        volume = (sbyte)(0xF * volVal / 99);

                    channelLine.Note = note;
                    channelLine.Sample = (byte)sample;
                    channelLine.Volume = volume;
                    channelLine.AdditionalCommand.Number = 0;
                    channelLine.AdditionalCommand.Delay = 0;
                    channelLine.AdditionalCommand.Parameter = 0;

                    patLine++;

                    if (patLine >= VTM.Patterns[PatternIndex].Length)
                        break;
                }

                if (curChan == 2)
                    break;

                curChan++;
            }

            SavePatternRedo();

            SongChanged = true;
            BackupSongChanged = true;

            Tracks.HideCaret();
            Tracks.RedrawTracks();
            Tracks.ShowCaret();
        }

        public void StepHLUpDown_ValueChanging(object sender, ValueChangingEventArgs e)
        {
            e.Cancel = e.NewValue < 0 || e.NewValue > VTModule.MaxPatternLength;

            if (e.Cancel)
                return;

            ChangeHLStep((int)e.NewValue);
        }

        public void AutoHLCheckBox_Click(object sender, EventArgs e)
        {
            if (AutoHLCheckBox.Checked)
                CalcHLStep();
        }

        public void CalcHLStep()
        {
            int patternLength = Tracks.ShownPattern == null ? VTModule.DefaultPatternLength : Tracks.ShownPattern.Length;
            int newStep;

            if (patternLength % 5 == 0)
                newStep = 5;
            else if (patternLength % 3 == 0)
                newStep = 3;
            else
                newStep = 4;

            if (newStep != Tracks.HLStep)
                StepHLUpDown.Value = newStep;
        }

        /* public void Edit17Exit(object sender, EventArgs e)
        {
            if (!Edit17.Modified)
            {
                return;
            }
            AutoHL.Checked = false;
            Edit17.Text = (UpDown15.Value).ToString();
            ChangeHLStep((int)UpDown15.Value);
        } */

        public void ChangeHLStep(int newStep)
        {
            if (newStep == 0)
                newStep = 256;

            if (Tracks.HLStep != newStep)
            {
                Tracks.HLStep = newStep;
                Tracks.RedrawTracks();
            }
        }

        public void UpDown15Click(object sender, EventArgs e)
        {
            AutoHLCheckBox.Checked = false;
        }

        public void SetLoopPos(int loopPosition)
        {
            SongChanged = true;
            BackupSongChanged = true;

            AddUndo(TChangeAction.ChangePositionListLoop, VTM.Positions.Loop, loopPosition);

            PositionsGrid[VTM.Positions.Loop, 0].Value = (VTM.Positions.Value[VTM.Positions.Loop]).ToString();
            VTM.Positions.Loop = loopPosition;
            PositionsGrid[VTM.Positions.Loop, 0].Value = 'L' + (VTM.Positions.Value[VTM.Positions.Loop]).ToString();

            if (PositionsGrid.Selection.Left != loopPosition)
                SelectPosition2(loopPosition);
        }

        public void AddUndo(TChangeAction changeAction, object par1, object par2)
        {
            int i, curLine, curChannel;

            if (UndoWorking)
                return;

            ChangeCount++;
            DisposeUndo(false);
            i = ChangeList.Length;

            if (ChangeCount > i)
                Array.Resize(ref ChangeList, i + 64);

            if (ChangeList[ChangeCount - 1] == null)
                ChangeList[ChangeCount - 1] = new ChangeListItem();

            ChangeList[ChangeCount - 1].Action = changeAction;

            switch (changeAction)
            {
                case TChangeAction.ChangeSpeed:
                case TChangeAction.ChangeToneTable:
                case TChangeAction.ChangePositionListLoop:
                case TChangeAction.ChangeFeatures:
                case TChangeAction.ChangeHeader:
                    ChangeList[ChangeCount - 1].OldParams.Params.Value = Convert.ToInt32(par1);
                    ChangeList[ChangeCount - 1].NewParams.Params.Value = Convert.ToInt32(par2);
                    break;
                case TChangeAction.ChangeTitle:
                case TChangeAction.ChangeAuthor:
                    ChangeList[ChangeCount - 1].OldParams.Str = Convert.ToString(par1);
                    ChangeList[ChangeCount - 1].NewParams.Str = Convert.ToString(par2);
                    break;
                case TChangeAction.ChangeSampleLoop:
                case TChangeAction.ChangeSampleSize:
                    ChangeList[ChangeCount - 1].OldParams.Params.Value = Convert.ToInt32(par1);
                    ChangeList[ChangeCount - 1].NewParams.Params.Value = Convert.ToInt32(par2);
                    ChangeList[ChangeCount - 1].ComParams.CurrentSample = SampleIndex;
                    break;
                case TChangeAction.ChangeOrnamentLoop:
                case TChangeAction.ChangeOrnamentSize:
                case TChangeAction.ChangeOrnamentValue:
                    ChangeList[ChangeCount - 1].OldParams.Params.Value = Convert.ToInt32(par1);
                    ChangeList[ChangeCount - 1].NewParams.Params.Value = Convert.ToInt32(par2);
                    ChangeList[ChangeCount - 1].ComParams.CurrentOrnament = OrnamentIndex;
                    break;
                case TChangeAction.InsertPosition:
                case TChangeAction.DeletePosition:
                case TChangeAction.ChangePositionsAndPatterns:
                case TChangeAction.ChangePatternSize:
                case TChangeAction.ChangeNote:
                case TChangeAction.ChangeNoteAndParams:
                case TChangeAction.ChangeEnvelopePeriod:
                case TChangeAction.ChangeNoise:
                case TChangeAction.ChangeSample:
                case TChangeAction.ChangeEnvelopeType:
                case TChangeAction.ChangeOrnament:
                case TChangeAction.ChangeVolume:
                case TChangeAction.ChangeSpecialCommandNumber:
                case TChangeAction.ChangeSpecialCommandDelay:
                case TChangeAction.ChangeSpecialCommandParameter:
                case TChangeAction.LoadPattern:
                case TChangeAction.InsertPatternFromClipboard:
                case TChangeAction.PatternInsertLine:
                case TChangeAction.PatternDeleteLine:
                case TChangeAction.PatternClearLine:
                case TChangeAction.PatternClearSelection:
                case TChangeAction.TransposePattern:
                case TChangeAction.TracksManagerCopy:
                case TChangeAction.ExpandCompressPattern:
                case TChangeAction.ChangePatternContent:
                    ChangeList[ChangeCount - 1].OldParams.Params.Value = Convert.ToInt32(par1);
                    ChangeList[ChangeCount - 1].NewParams.Params.Value = Convert.ToInt32(par2);

                    if (changeAction == TChangeAction.ChangeNoteAndParams)
                    {
                        curLine = Tracks.CurrentPatternLine();
                        curChannel = Tracks.CurrentChannel();
                        ChangeList[ChangeCount - 1].OldParams.Params.NoteParam = VTM.Patterns[PatternIndex].Lines[curLine].Channel[curChannel].Note;
                        ChangeList[ChangeCount - 1].OldParams.Params.SampleParam = VTM.Patterns[PatternIndex].Lines[curLine].Channel[curChannel].Sample;
                        ChangeList[ChangeCount - 1].OldParams.Params.OrnamentParam = VTM.Patterns[PatternIndex].Lines[curLine].Channel[curChannel].Ornament;
                        ChangeList[ChangeCount - 1].OldParams.Params.VolumeParam = VTM.Patterns[PatternIndex].Lines[curLine].Channel[curChannel].Volume;
                        ChangeList[ChangeCount - 1].OldParams.Params.EnvelopeParam = VTM.Patterns[PatternIndex].Lines[curLine].Channel[curChannel].Envelope;
                        ChangeList[ChangeCount - 1].NewParams.Params.NoteParam = Convert.ToSByte(par1);
                        ChangeList[ChangeCount - 1].NewParams.Params.SampleParam = Tracks.LastNoteParams[curChannel].Sample;
                        ChangeList[ChangeCount - 1].NewParams.Params.OrnamentParam = Tracks.LastNoteParams[curChannel].Ornament;
                        ChangeList[ChangeCount - 1].NewParams.Params.VolumeParam = Tracks.LastNoteParams[curChannel].Volume;
                        ChangeList[ChangeCount - 1].NewParams.Params.EnvelopeParam = Tracks.LastNoteParams[curChannel].Envelope;
                    }

                    if (changeAction == TChangeAction.TransposePattern || changeAction == TChangeAction.TracksManagerCopy)
                    {
                        ChangeList[ChangeCount - 1].OldParams.Params.CurrentPattern = Convert.ToSByte(par1);
                        ChangeList[ChangeCount - 1].OldParams.Params.PatternCursorX = 0;
                        ChangeList[ChangeCount - 1].OldParams.Params.PatternCursorY = Tracks.CenterLineIndex;
                        ChangeList[ChangeCount - 1].OldParams.Params.PatternShownFrom = 0;
                        ChangeList[ChangeCount - 1].NewParams.Params.CurrentPattern = Convert.ToSByte(par1);
                        ChangeList[ChangeCount - 1].NewParams.Params.PatternCursorX = 0;
                        ChangeList[ChangeCount - 1].NewParams.Params.PatternCursorY = Tracks.CenterLineIndex;
                        ChangeList[ChangeCount - 1].NewParams.Params.PatternShownFrom = 0;
                    }
                    else
                    {
                        ChangeList[ChangeCount - 1].OldParams.Params.CurrentPattern = PatternIndex;
                        ChangeList[ChangeCount - 1].OldParams.Params.PatternCursorX = Tracks.CursorX;
                        ChangeList[ChangeCount - 1].OldParams.Params.PatternCursorY = Tracks.CursorY;
                        ChangeList[ChangeCount - 1].OldParams.Params.PatternShownFrom = Tracks.ShownFrom;
                        ChangeList[ChangeCount - 1].NewParams.Params.CurrentPattern = PatternIndex;
                        ChangeList[ChangeCount - 1].NewParams.Params.PatternCursorX = Tracks.CursorX;
                        ChangeList[ChangeCount - 1].NewParams.Params.PatternCursorY = Tracks.CursorY;
                        ChangeList[ChangeCount - 1].NewParams.Params.PatternShownFrom = Tracks.ShownFrom;
                    }

                    if (VTM.Positions.Value[PositionIndex] == PatternIndex)
                    {
                        ChangeList[ChangeCount - 1].OldParams.Params.CurrentPosition = PositionIndex;
                        ChangeList[ChangeCount - 1].NewParams.Params.CurrentPosition = PositionIndex;
                    }
                    else
                    {
                        ChangeList[ChangeCount - 1].OldParams.Params.CurrentPosition = -1;
                        ChangeList[ChangeCount - 1].NewParams.Params.CurrentPosition = -1;
                    }
                    break;
                case TChangeAction.ChangePositionValue:
                    ChangeList[ChangeCount - 1].OldParams.Params.Value = Convert.ToInt32(par1);
                    ChangeList[ChangeCount - 1].NewParams.Params.Value = Convert.ToInt32(par2);
                    ChangeList[ChangeCount - 1].OldParams.Params.PositionListLen = VTM.Positions.Length;
                    break;
                case TChangeAction.LoadOrnament:
                case TChangeAction.OrGen:
                    ChangeList[ChangeCount - 1].OldParams.Params.OrnamentCursor = Ornaments.CursorY + Ornaments.CursorX / MainForm.OrnCharCount * Ornaments.NRaw;
                    ChangeList[ChangeCount - 1].OldParams.Params.OrnamentShownFrom = Ornaments.ShownFrom;
                    ChangeList[ChangeCount - 1].ComParams.CurrentOrnament = OrnamentIndex;
                    break;
                case TChangeAction.LoadSample:
                case TChangeAction.ChangeSampleValue:
                    if (changeAction == TChangeAction.ChangeSampleValue)
                    {
                        ChangeList[ChangeCount - 1].SampleLineValues = (SampleTick)par1;
                        ChangeList[ChangeCount - 1].Line = Convert.ToInt32(par2);
                    }

                    ChangeList[ChangeCount - 1].OldParams.Params.SampleCursorX = Samples.CursorX;
                    ChangeList[ChangeCount - 1].NewParams.Params.SampleCursorX = Samples.CursorX;
                    ChangeList[ChangeCount - 1].OldParams.Params.SampleCursorY = Samples.CursorY;
                    ChangeList[ChangeCount - 1].OldParams.Params.SampleShownFrom = Samples.ShownFrom;
                    ChangeList[ChangeCount - 1].ComParams.CurrentSample = SampleIndex;
                    break;
            }
        }

        public void DoUndo_SetF(int page, Control ctrl, int steps, bool undo)
        {
            bool focused = TabControl.SelectedIndex == page;
            ActivateTab(page);

            switch (page)
            {
                case 0:
                    if (!focused || !Tracks.Enabled && Tracks.Focused)
                    {
                        if (ctrl.CanFocus)
                            ctrl.Focus();
                    }
                    break;
                case 1:
                    if (!focused || !Samples.Enabled && Samples.Focused)
                    {
                        if (ctrl.CanFocus)
                            ctrl.Focus();
                    }
                    break;
                case 2:
                    if (!focused || !Ornaments.Enabled && Ornaments.Focused)
                    {
                        if (ctrl.CanFocus)
                            ctrl.Focus();
                    }
                    break;
                case 3:
                    if (ctrl.CanFocus)
                        ctrl.Focus();
                    break;
            }
        }

        public void DoUndo_RedrawTracks(int index, ChangeParameters changeParams, bool isSize, int steps, bool undo)
        {
            if (changeParams.Params.CurrentPosition >= 0)
                SelectPosition2(changeParams.Params.CurrentPosition);

            ValidatePattern2(changeParams.Params.CurrentPattern);

            Tracks.ShownPattern = VTM.Patterns[changeParams.Params.CurrentPattern];
            PatternLenUpDown.Value = isSize ? changeParams.Params.Size : Tracks.ShownPattern.Length;

            if (Tracks.Focused)
                Tracks.HideCaret();

            Tracks.ShownFrom = changeParams.Params.PatternShownFrom;
            Tracks.CursorY = changeParams.Params.PatternCursorY;
            Tracks.CursorX = changeParams.Params.PatternCursorX;

            Tracks.RemoveSelection();
            Tracks.RedrawTracks();

            if (Tracks.Focused)
                Tracks.ShowCaret();
            else
            {
                ActivateTab(0);

                if (Tracks.CanFocus)
                    Tracks.Focus();
            }

            Tracks.SetCaretPosition();
            Tracks.RecreateCaret();

            ShowStat();
        }

        public void DoUndo_ShowSmp(int index, ChangeParameters changeParams, int steps, bool undo)
        {
            SongChanged = true;
            BackupSongChanged = true;

            Samples.CursorY = changeParams.Params.SampleCursorY;
            Samples.CursorX = changeParams.Params.SampleCursorX;
            Samples.ShownFrom = changeParams.Params.SampleShownFrom;

            if (SampleNumUpDown.Value == ChangeList[index].ComParams.CurrentSample)
                ChangeSample(ChangeList[index].ComParams.CurrentSample, false, true);
            else
                SampleNumUpDown.Value = ChangeList[index].ComParams.CurrentSample;

            if (!Samples.Focused)
            {
                ActivateTab(1);

                if (Samples.CanFocus)
                    Samples.Focus();
            }
        }

        public void DoUndo(int steps, bool undo)
        {
            ChangeParameters changeParams;
            int index;
            int i, j;
            int patternNumber;
            object pnt;
            Position posLst;
            string s;
            SampleTick sampleTick;
            ChangePattern[][] patternsState = null;
            int[] nilPatternsState = null;
            ChangeOnePattern patternState;
            ChangeSample sampleState;
            ChangeOrnament ornamentState;
            UndoWorking = true;

            try
            {
                for (i = steps; i >= 1; i--)
                {
                    if (undo)
                    {
                        if (ChangeCount == 0)
                            return;

                        ChangeCount--;
                        index = ChangeCount;
                        changeParams = ChangeList[index].OldParams;
                    }
                    else
                    {
                        if (ChangeCount >= ChangeTop)
                            return;

                        index = ChangeCount;
                        ChangeCount++;
                        changeParams = ChangeList[index].NewParams;
                    }

                    switch (ChangeList[index].Action)
                    {
                        case TChangeAction.ChangeSpeed:
                            SpeedBpmUpDown.Value = changeParams.Params.Speed;
                            //SpeedBpmEdit.SelectAll();
                            DoUndo_SetF(0, SpeedBpmUpDown, steps, undo);
                            CalcTotLen();
                            break;
                        case TChangeAction.ChangeTitle:
                            TitleTextBox.Text = changeParams.Str;
                            DoUndo_SetF(0, TitleTextBox, steps, undo);
                            break;
                        case TChangeAction.ChangeAuthor:
                            AuthorTextBox.Text = changeParams.Str;
                            DoUndo_SetF(0, AuthorTextBox, steps, undo);
                            break;
                        case TChangeAction.ChangeToneTable:
                            ToneTableUpDown.Value = changeParams.Params.Table;
                            //Edit7.SelectAll();
                            DoUndo_SetF(0, ToneTableUpDown, steps, undo);
                            break;
                        case TChangeAction.ChangeSampleLoop:
                            SampleNumUpDown.Value = ChangeList[index].ComParams.CurrentSample;
                            SampleLoopUpDown.Value = changeParams.Params.Loop;
                            //SampleLoopEdit.SelectAll();
                            DoUndo_SetF(1, SampleLoopUpDown, steps, undo);
                            break;
                        case TChangeAction.ChangeOrnamentLoop:
                            OrnamentNumUpDown.Value = ChangeList[index].ComParams.CurrentOrnament;
                            OrnamentLoopUpDown.Value = changeParams.Params.Loop;
                            //OrnamentLoopEdit.SelectAll();
                            DoUndo_SetF(2, OrnamentLoopUpDown, steps, undo);
                            break;
                        case TChangeAction.ChangePatternSize:
                            DoUndo_RedrawTracks(index, changeParams, true, steps, undo);
                            CalcTotLen();
                            break;
                        case TChangeAction.ChangeNote:
                            ChangeNote(changeParams.Params.CurrentPattern, ChangeList[index].Line, ChangeList[index].Channel, changeParams.Params.Note);
                            DoUndo_RedrawTracks(index, ChangeList[index].OldParams, false, steps, undo);
                            break;
                        case TChangeAction.ChangeNoteAndParams:
                            ChangeNote(changeParams.Params.CurrentPattern, ChangeList[index].Line, ChangeList[index].Channel, changeParams.Params.NoteParam);
                            ChannelLine channelLine = VTM.Patterns[changeParams.Params.CurrentPattern].Lines[ChangeList[index].Line].Channel[ChangeList[index].Channel];
                            channelLine.Sample = changeParams.Params.SampleParam;
                            channelLine.Ornament = changeParams.Params.OrnamentParam;
                            channelLine.Volume = changeParams.Params.VolumeParam;
                            channelLine.Envelope = changeParams.Params.EnvelopeParam;
                            DoUndo_RedrawTracks(index, ChangeList[index].OldParams, false, steps, undo);
                            break;
                        case TChangeAction.ChangeEnvelopePeriod:
                            j = changeParams.Params.PatternCursorX;
                            if (j > 3)
                                j = 0;
                            ChangeTracks(changeParams.Params.CurrentPattern, ChangeList[index].Line, ChangeList[index].Channel, j, changeParams.Params.Value, false);
                            DoUndo_RedrawTracks(index, ChangeList[index].OldParams, false, steps, undo);
                            break;
                        case TChangeAction.ChangeNoise:
                        case TChangeAction.ChangeSample:
                        case TChangeAction.ChangeEnvelopeType:
                        case TChangeAction.ChangeOrnament:
                        case TChangeAction.ChangeVolume:
                        case TChangeAction.ChangeSpecialCommandDelay:
                            ChangeTracks(changeParams.Params.CurrentPattern, ChangeList[index].Line, ChangeList[index].Channel, changeParams.Params.PatternCursorX, changeParams.Params.Value, false);
                            DoUndo_RedrawTracks(index, ChangeList[index].OldParams, false, steps, undo);
                            break;
                        case TChangeAction.ChangeSpecialCommandNumber:
                        case TChangeAction.ChangeSpecialCommandParameter:
                            ChangeTracks(changeParams.Params.CurrentPattern, ChangeList[index].Line, ChangeList[index].Channel, changeParams.Params.PatternCursorX, changeParams.Params.Value, false);
                            DoUndo_RedrawTracks(index, ChangeList[index].OldParams, false, steps, undo);
                            CalcTotLen();
                            break;
                        case TChangeAction.LoadPattern:
                        case TChangeAction.InsertPatternFromClipboard:
                        case TChangeAction.PatternInsertLine:
                        case TChangeAction.PatternDeleteLine:
                        case TChangeAction.PatternClearLine:
                        case TChangeAction.PatternClearSelection:
                        case TChangeAction.TransposePattern:
                        case TChangeAction.TracksManagerCopy:
                        case TChangeAction.ExpandCompressPattern:
                            SongChanged = true;
                            BackupSongChanged = true;
                            pnt = ChangeList[index].Pattern;
                            ChangeList[index].Pattern = VTM.Patterns[changeParams.Params.CurrentPattern];
                            VTM.Patterns[changeParams.Params.CurrentPattern] = (Pattern)pnt;
                            DoUndo_RedrawTracks(index, changeParams, false, steps, undo);
                            CalcTotLen();
                            break;
                        case TChangeAction.ChangePositionListLoop:
                            SetLoopPos(changeParams.Params.Loop);
                            DoUndo_SetF(0, PositionsGrid, steps, undo);
                            PatternNumUpDown.Value = VTM.Positions.Value[changeParams.Params.Loop];
                            break;
                        case TChangeAction.ChangePositionValue:
                            for (j = changeParams.Params.PositionListLen; j < VTM.Positions.Length; j++)
                                PositionsGrid[j, 0].Value = "...";

                            VTM.Positions.Length = changeParams.Params.PositionListLen;

                            if (changeParams.Params.CurrentPosition < VTM.Positions.Length)
                                ChangePositionValue(changeParams.Params.CurrentPosition, changeParams.Params.Value);

                            if (undo)
                                SelectPositions(ChangeList[index].OldGridSelection);
                            else
                                SelectPositions(ChangeList[index].NewGridSelection);

                            DoUndo_SetF(0, PositionsGrid, steps, undo);
                            CalcTotLen();
                            break;
                        case TChangeAction.DeletePosition:
                        case TChangeAction.InsertPosition:
                            posLst = ChangeList[index].PositionList;
                            ChangeList[index].PositionList = VTM.Positions;
                            VTM.Positions = posLst;
                            for (j = 0; j < 256; j++)
                            {
                                if (j < VTM.Positions.Length)
                                {
                                    s = (VTM.Positions.Value[j]).ToString();

                                    if (j == VTM.Positions.Loop)
                                        s = $"L{s}";

                                    PositionsGrid[j, 0].Value = s;
                                }
                                else
                                {
                                    PositionsGrid[j, 0].Value = "...";
                                }
                            }
                            RedrawPatternPositions();

                            // Restore tracks, position and selection
                            if (undo)
                            {
                                DoUndo_RedrawTracks(index, ChangeList[index].OldParams, false, steps, undo);
                                SelectPositions(ChangeList[index].OldGridSelection);
                            }
                            else
                            {
                                DoUndo_RedrawTracks(index, ChangeList[index].NewParams, false, steps, undo);
                                SelectPositions(ChangeList[index].NewGridSelection);
                            }
                            CalcTotLen();
                            InputPNumber = 0;
                            DoUndo_SetF(0, PositionsGrid, steps, undo);
                            break;
                        case TChangeAction.ChangePositionsAndPatterns:
                            // Undo/Redo positions
                            posLst = ChangeList[index].PositionList;
                            ChangeList[index].PositionList = VTM.Positions;
                            VTM.Positions = posLst;
                            patternsState = ChangeList[index].ComParams.Patterns;
                            nilPatternsState = ChangeList[index].ComParams.NilPatterns;
                            // UNDO patterns
                            if (undo)
                            {
                                // Copy saved patterns back
                                for (j = 0; j < patternsState[0].Length; j++)
                                {
                                    patternNumber = patternsState[0][j].Number;
                                    ValidatePattern2(patternNumber);
                                    VTM.Patterns[patternNumber].Lines = patternsState[0][j].Pattern.Lines;
                                    VTM.Patterns[patternNumber].Length = patternsState[0][j].Pattern.Length;
                                }
                                // Clear unused patterns
                                for (j = 0; j <= nilPatternsState.Length; j++)
                                    VTM.Patterns[nilPatternsState[j]] = null;
                                // REDO patterns
                            }
                            else
                            {
                                // Copy saved patterns back
                                for (j = 0; j < patternsState[1].Length; j++)
                                {
                                    patternNumber = patternsState[1][j].Number;
                                    ValidatePattern2(patternNumber);
                                    VTM.Patterns[patternNumber].Lines = patternsState[1][j].Pattern.Lines;
                                    VTM.Patterns[patternNumber].Length = patternsState[1][j].Pattern.Length;
                                }
                            }
                            RedrawPatternPositions();
                            // Restore tracks, position and selection
                            if (undo)
                            {
                                DoUndo_RedrawTracks(index, ChangeList[index].OldParams, false, steps, undo);
                                SelectPositions(ChangeList[index].OldGridSelection);
                            }
                            else
                            {
                                DoUndo_RedrawTracks(index, ChangeList[index].NewParams, false, steps, undo);
                                SelectPositions(ChangeList[index].NewGridSelection);
                            }
                            CalcTotLen();
                            InputPNumber = 0;
                            DoUndo_SetF(0, PositionsGrid, steps, undo);
                            break;
                        case TChangeAction.ChangePatternContent:
                            patternState = ChangeList[index].ComParams.ChangedPattern;
                            // UNDO pattern
                            if (undo)
                            {
                                PatternIndex = ChangeList[index].OldParams.Params.CurrentPattern;
                                PositionIndex = ChangeList[index].OldParams.Params.CurrentPosition;

                                Tracks.CursorX = ChangeList[index].OldParams.Params.PatternCursorX;
                                Tracks.CursorY = ChangeList[index].OldParams.Params.PatternCursorY;
                                Tracks.ShownFrom = ChangeList[index].OldParams.Params.PatternShownFrom;
                                Tracks.ShownPattern = VTM.Patterns[PatternIndex];

                                Tracks.ShownPattern.Length = patternState.OldPattern.Length;
                                Tracks.ShownPattern.Lines = patternState.OldPattern.Lines;
                                PatternLenUpDown.Value = Tracks.ShownPattern.Length;
                                PatternNumUpDown.Value = PatternIndex;

                                SelectPositions(ChangeList[index].OldGridSelection);
                            }
                            else
                            {
                                // REDO pattern
                                PatternIndex = ChangeList[index].NewParams.Params.CurrentPattern;
                                PositionIndex = ChangeList[index].NewParams.Params.CurrentPosition;

                                Tracks.CursorX = ChangeList[index].NewParams.Params.PatternCursorX;
                                Tracks.CursorY = ChangeList[index].NewParams.Params.PatternCursorY;
                                Tracks.ShownFrom = ChangeList[index].NewParams.Params.PatternShownFrom;
                                Tracks.ShownPattern = VTM.Patterns[PatternIndex];

                                Tracks.ShownPattern.Length = patternState.NewPattern.Length;
                                Tracks.ShownPattern.Lines = patternState.NewPattern.Lines;
                                PatternLenUpDown.Value = Tracks.ShownPattern.Length;
                                PatternNumUpDown.Value = PatternIndex;

                                SelectPositions(ChangeList[index].NewGridSelection);
                            }
                            Tracks.HideCaret();
                            Tracks.SetCaretPosition();
                            Tracks.RemoveSelection();
                            Tracks.RedrawTracks();
                            Tracks.RecreateCaret();
                            Tracks.ShowCaret();
                            Tracks.KeyPressed = 0;

                            CalcTotLen();
                            DoUndo_SetF(0, Tracks, steps, undo);
                            break;
                        case TChangeAction.ChangeEntireSample:
                            sampleState = ChangeList[index].ComParams.EntireSample;
                            // UNDO sample
                            if (undo)
                            {
                                SampleNumUpDown.Value = sampleState.Number;
                                Samples.ShownFrom = ChangeList[index].OldParams.Params.SampleShownFrom;
                                Samples.CursorX = ChangeList[index].OldParams.Params.SampleCursorX;
                                Samples.CursorY = ChangeList[index].OldParams.Params.SampleCursorY;

                                Samples.ShownSample.Length = sampleState.OldSample.Length;
                                Samples.ShownSample.Loop = sampleState.OldSample.Loop;
                                Samples.ShownSample.Enabled = sampleState.OldSample.Enabled;
                                Samples.ShownSample.Ticks = sampleState.OldSample.Ticks;
                                SampleLoopUpDown.Value = Samples.ShownSample.Loop;
                                SampleLengthUpDown.Value = Samples.ShownSample.Length;
                            }
                            else
                            {
                                // REDO sample
                                SampleNumUpDown.Value = sampleState.Number;

                                Samples.ShownFrom = ChangeList[index].NewParams.Params.SampleShownFrom;
                                Samples.CursorX = ChangeList[index].NewParams.Params.SampleCursorX;
                                Samples.CursorY = ChangeList[index].NewParams.Params.SampleCursorY;

                                Samples.ShownSample.Length = sampleState.NewSample.Length;
                                Samples.ShownSample.Loop = sampleState.NewSample.Loop;
                                Samples.ShownSample.Enabled = sampleState.NewSample.Enabled;
                                Samples.ShownSample.Ticks = sampleState.NewSample.Ticks;
                                SampleLoopUpDown.Value = Samples.ShownSample.Loop;
                                SampleLengthUpDown.Value = Samples.ShownSample.Length;
                            }

                            if (Samples.Focused)
                                Samples.HideCaret();

                            Samples.Redraw();

                            if (Samples.Focused)
                                Samples.ShowCaret();

                            DoUndo_SetF(1, Samples, steps, undo);
                            break;

                        case TChangeAction.SwapSamples:
                            SwapSamples(ChangeList[index].NewParams.Params.Value, ChangeList[index].OldParams.Params.Value);

                            if (TabControl.SelectedTab == PatternsTab)
                                Tracks.RedrawTracks();

                            if (TabControl.SelectedTab == SamplesTab)
                                SamplesGrid.Invalidate();

                            if (Samples.Focused)
                                Samples.HideCaret();

                            Samples.Redraw();
                            Samples.SetCaretPosition();

                            if (Samples.Focused)
                                Samples.ShowCaret();
                            break;

                        case TChangeAction.SwapOrnaments:
                            SwapOrnaments(ChangeList[index].NewParams.Params.Value, ChangeList[index].OldParams.Params.Value);

                            if (TabControl.SelectedTab == PatternsTab)
                                Tracks.RedrawTracks();

                            if (TabControl.SelectedTab == OrnamentsTab)
                                OrnamentsGrid.Invalidate();

                            if (Ornaments.Focused)
                                Ornaments.HideCaret();

                            Ornaments.Redraw();
                            Ornaments.SetCaretPosition();

                            if (Ornaments.Focused)
                                Ornaments.ShowCaret();
                            break;

                        case TChangeAction.ChangeEntireOrnament:
                            ornamentState = ChangeList[index].ComParams.EntireOrnament;

                            // UNDO ornament
                            if (undo)
                            {
                                SampleNumUpDown.Value = ornamentState.Number;
                                Ornaments.ShownFrom = ChangeList[index].OldParams.Params.OrnamentShownFrom;
                                Ornaments.CursorInt = ChangeList[index].OldParams.Params.OrnamentCursor;

                                Ornaments.ShownOrnament.Length = ornamentState.OldOrnament.Length;
                                Ornaments.ShownOrnament.Loop = ornamentState.OldOrnament.Loop;
                                Ornaments.ShownOrnament.Offsets = ornamentState.OldOrnament.Offsets;
                                OrnamentLenUpDown.Value = Ornaments.ShownOrnament.Length;
                                OrnamentLoopUpDown.Value = Ornaments.ShownOrnament.Loop;
                            }
                            else
                            {
                                // REDO ornament
                                SampleNumUpDown.Value = ornamentState.Number;
                                Ornaments.ShownFrom = ChangeList[index].NewParams.Params.OrnamentShownFrom;
                                Ornaments.CursorInt = ChangeList[index].NewParams.Params.OrnamentCursor;

                                Ornaments.ShownOrnament.Length = ornamentState.NewOrnament.Length;
                                Ornaments.ShownOrnament.Loop = ornamentState.NewOrnament.Loop;
                                Ornaments.ShownOrnament.Offsets = ornamentState.NewOrnament.Offsets;
                                OrnamentLenUpDown.Value = Ornaments.ShownOrnament.Length;
                                OrnamentLoopUpDown.Value = Ornaments.ShownOrnament.Loop;
                            }
                            if (Ornaments.Focused)
                                Ornaments.HideCaret();
                            Ornaments.Redraw();
                            if (Ornaments.Focused)
                                Ornaments.ShowCaret();
                            DoUndo_SetF(2, Ornaments, steps, undo);
                            break;
                        case TChangeAction.ChangeSampleSize:
                            SampleNumUpDown.Value = ChangeList[index].ComParams.CurrentSample;
                            SampleLengthUpDown.Value = changeParams.Params.Size;
                            SampleLoopUpDown.Value = changeParams.Params.PrevLoop;
                            if (Samples.Focused)
                                Samples.HideCaret();
                            Samples.Redraw();
                            if (Samples.Focused)
                                Samples.ShowCaret();
                            //SampleLenEdit.SelectAll();
                            DoUndo_SetF(1, SampleLengthUpDown, steps, undo);
                            break;
                        case TChangeAction.ChangeOrnamentSize:
                            OrnamentNumUpDown.Value = ChangeList[index].ComParams.CurrentOrnament;
                            OrnamentLenUpDown.Value = changeParams.Params.Size;
                            OrnamentLoopUpDown.Value = changeParams.Params.PrevLoop;
                            if (Ornaments.Focused)
                                Ornaments.HideCaret();
                            Ornaments.Redraw();
                            if (Ornaments.Focused)
                                Ornaments.ShowCaret();
                            //OrnamentLenUpDown.SelectAll();
                            DoUndo_SetF(2, OrnamentLenUpDown, steps, undo);
                            break;
                        case TChangeAction.ChangeFeatures:
                            VtmFeaturesBox.SelectedIndex = changeParams.Params.NewFeatures;
                            DoUndo_SetF(3, VtmFeaturesBox.Buttons[VtmFeaturesBox.SelectedIndex], steps, undo);
                            break;
                        case TChangeAction.ChangeHeader:
                            SaveHeaderBox.SelectedIndex = changeParams.Params.NewHeader;
                            DoUndo_SetF(3, SaveHeaderBox.Buttons[SaveHeaderBox.SelectedIndex], steps, undo);
                            break;
                        case TChangeAction.ChangeOrnamentValue:
                            OrnamentNumUpDown.Value = ChangeList[index].ComParams.CurrentOrnament;
                            VTM.Ornaments[ChangeList[index].ComParams.CurrentOrnament].Offsets[ChangeList[index].OldParams.Params.OrnamentShownFrom + ChangeList[index].OldParams.Params.OrnamentCursor] = (sbyte)changeParams.Params.Value;
                            if (Ornaments.Focused)
                                Ornaments.HideCaret();
                            Ornaments.CursorY = ChangeList[index].OldParams.Params.OrnamentCursor % Ornaments.NRaw;
                            Ornaments.CursorX = ChangeList[index].OldParams.Params.OrnamentCursor / Ornaments.NRaw * MainForm.OrnCharCount;
                            Ornaments.SetCaretPosition();
                            Ornaments.ShownFrom = ChangeList[index].OldParams.Params.OrnamentShownFrom;
                            Ornaments.Redraw();
                            if (Ornaments.Focused)
                                Ornaments.ShowCaret();
                            else
                            {
                                ActivateTab(2);

                                if (Ornaments.CanFocus)
                                    Ornaments.Focus();
                            }
                            break;
                        case TChangeAction.LoadOrnament:
                        case TChangeAction.OrGen:
                        case TChangeAction.CopyOrnamentToOrnament:
                            SongChanged = true;
                            BackupSongChanged = true;
                            pnt = ChangeList[index].Ornament;
                            ChangeList[index].Ornament = VTM.Ornaments[ChangeList[index].ComParams.CurrentOrnament];
                            VTM.Ornaments[ChangeList[index].ComParams.CurrentOrnament] = (Ornament)pnt;
                            Ornaments.CursorY = changeParams.Params.OrnamentCursor % Ornaments.NRaw;
                            Ornaments.CursorX = changeParams.Params.OrnamentCursor / Ornaments.NRaw * MainForm.OrnCharCount;
                            Ornaments.ShownFrom = changeParams.Params.OrnamentShownFrom;

                            if (OrnamentNumUpDown.Value == ChangeList[index].ComParams.CurrentOrnament)
                                ChangeOrnament(ChangeList[index].ComParams.CurrentOrnament, true, true);
                            else
                                OrnamentNumUpDown.Value = ChangeList[index].ComParams.CurrentOrnament;

                            if (!Ornaments.Focused)
                            {
                                ActivateTab(2);

                                if (Ornaments.CanFocus)
                                    Ornaments.Focus();
                            }
                            break;
                        case TChangeAction.LoadSample:
                        case TChangeAction.CopySampleToSample:
                            pnt = ChangeList[index].Sample;
                            ChangeList[index].Sample = VTM.Samples[ChangeList[index].ComParams.CurrentSample];
                            VTM.Samples[ChangeList[index].ComParams.CurrentSample] = (Sample)pnt;
                            Samples.ShownSample = (Sample)pnt;
                            DoUndo_ShowSmp(index, changeParams, steps, undo);
                            break;
                        case TChangeAction.ChangeSampleValue:
                            changeParams = ChangeList[index].OldParams;
                            sampleTick = ChangeList[index].SampleLineValues;
                            ChangeList[index].SampleLineValues = VTM.Samples[ChangeList[index].ComParams.CurrentSample].Ticks[ChangeList[index].Line];
                            VTM.Samples[ChangeList[index].ComParams.CurrentSample].Ticks[ChangeList[index].Line] = sampleTick;
                            DoUndo_ShowSmp(index, changeParams, steps, undo);
                            break;
                    }
                }
            }
            finally
            {
                UndoWorking = false;
            }
        }

        public bool SaveModuleAs()
        {
            MainForm mainForm = Globals.MainForm;

            if (WinFileName != "")
                mainForm.SaveDialog1.FileName = WinFileName;
            else if (TSWindow[0] != null && TSWindow[0].WinFileName != "")
                mainForm.SaveDialog1.FileName = TSWindow[0].WinFileName;
            else if (TSWindow[1] != null && TSWindow[1].WinFileName != "")
                mainForm.SaveDialog1.FileName = TSWindow[1].WinFileName;
            else
                mainForm.SaveDialog1.FileName = $"MyBestTrack{WinNumber}";

            mainForm.SaveDialog1.FilterIndex = (!SavedAsText ? 1 : 0) + 1;

            if (WinFileName.IndexOf(Path.Combine(MainForm.VortexDir, "template.vt2")) != -1)
            {
                mainForm.SaveDialog1.InitialDirectory = Path.GetDirectoryName(mainForm.RecentFiles[0]);
                mainForm.SaveDialog1.FileName = "";
            }
            else if (mainForm.SaveDialog1.FileName.IndexOf(MainForm.VortexDir) != -1 || mainForm.SaveDialog1.FileName.IndexOf(MainForm.VortexDocumentsDir) != -1)
                mainForm.SaveDialog1.FileName = mainForm.SaveDialog1.InitialDirectory + Path.GetFileName(mainForm.SaveDialog1.FileName);

            if (mainForm.SaveDialog1.ShowDialog() == DialogResult.OK)
            {
                IsDemoSong = false;
                IsTemplate = false;
                mainForm.SaveDialog1.FileName = Path.ChangeExtension(mainForm.SaveDialog1.FileName, mainForm.SaveDialog1.FilterIndex == 1 ? ".vt2" : ".pt3");
                mainForm.SaveDialog1.InitialDirectory = mainForm.SaveDialog1.FileName;

                if (mainForm.SavePT3(this, mainForm.SaveDialog1.FileName, mainForm.SaveDialog1.FilterIndex == 1))
                {
                    SetFileName(mainForm.SaveDialog1.FileName);
                    return true;
                }
            }

            return false;
        }

        public void SaveModule()
        {
            string fileName;

            if (!SongChanged && TSWindow[0] == null)
                return;

            if (!SongChanged && TSWindow[0] != null && !TSWindow[0].SongChanged)
                return;

            if (!SongChanged && TSWindow[0] != null && !TSWindow[0].SongChanged &&
                TSWindow[1] != null && !TSWindow[1].SongChanged)
                return;

            if (WinFileName == "")
                SaveModuleAs();
            else
            {
                fileName = Path.ChangeExtension(WinFileName, SavedAsText ? ".vt2" : ".pt3");

                if (fileName != WinFileName)
                    SetFileName(fileName);

                Globals.MainForm.SavePT3(this, WinFileName, SavedAsText);
            }
        }

        public void SaveModuleBackup()
        {
            string filePath;

            if (!SongChanged || WinFileName == "" || IsDemoSong)
                return;

            filePath = WinFileName;

            // Is backup file opened?
            if (WinFileName.IndexOf(" ver ") != -1)
            {
                // cut ' ver 001.vt2'
                filePath = WinFileName.Substring(0, WinFileName.IndexOf(WinFileName) - 1);
            }

            string fileName = Path.Combine(Path.GetDirectoryName(filePath), ExtractFileNameEx(filePath)) + $" ver {BackupVersionCounter:D3}.vt2";

            Globals.MainForm.SavePT3Backup(this, fileName, true);

            BackupSongChanged = false;
        }

        public void ChildWin_Activated(object sender, EventArgs e)
        {
            if (VTM == null)
                return;

            for (int i = 0; i < 32; i++)
            {
                ToggleSamplesForm.ToglSam[i] = new CheckBox();
                ToggleSamplesForm.ToglSam[i].Checked = VTM.Samples[i] == null || VTM.Samples[i].Enabled;
            }

            SetToolsPattern();

            if (Globals.MainForm.MdiChildren.Length > 3 && TSWindow[1] != null)
                SetWindowPos(TSWindow[1].Handle, HWND_TOP, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE);

            else if (Globals.MainForm.MdiChildren.Length > 2 && TSWindow[0] != null)
                SetWindowPos(TSWindow[0].Handle, HWND_TOP, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE);

            if (Tracks.CanFocus)
                Tracks.Focus();
        }

        public string PrepareTSString(Button tsButton, string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            const string ellipsis = "...";

            using (Graphics g = Graphics.FromHwnd(tsButton.Handle))
            {
                Size fullSize = TextRenderer.MeasureText(g, s, tsButton.Font, Size.Empty, TextFormatFlags.NoPadding);

                // If it already fits, return it
                if (fullSize.Width <= tsButton.ClientSize.Width)
                    return s;

                // Start shortening the string from the end until it fits
                StringBuilder stringBuilder = new StringBuilder(s);

                // Ensure we don't go negative or out of range
                for (int i = stringBuilder.Length - 1; i > 0; i--)
                {
                    stringBuilder.Length = i;
                    string trial = stringBuilder.ToString() + ellipsis;
                    Size trialSize = TextRenderer.MeasureText(g, trial, tsButton.Font, Size.Empty, TextFormatFlags.NoPadding);

                    if (trialSize.Width <= tsButton.ClientSize.Width)
                        return trial;
                }
            }

            return ellipsis;
        }

        public void JoinChild(ChildForm childForm)
        {
            ChildForm[] childWins = new ChildForm[3];
            int n = 0;
            string name, fileName;

            if (TSWindow[0] != null && TSWindow[1] == null)
                n = 1;
            else if (TSWindow[0] != null && TSWindow[1] != null)
                return;

            if (childForm.TSWindow[0] != null)
            {
                TSWindow[0] = childForm.TSWindow[0];
                TSWindow[1] = childForm.TSWindow[0].TSWindow[0];
                childForm.TSWindow[1] = this;
                childForm.TSWindow[0].TSWindow[1] = this;
                n = 1;
            }
            else
            {
                TSWindow[n] = childForm;
                childForm.TSWindow[n] = this;

                if (n == 1)
                {
                    childForm.TSWindow[0] = TSWindow[0];
                    TSWindow[0].TSWindow[1] = childForm;
                }
            }

            SongChanged = true; TSWindow[n].SongChanged = true;
            IsTemplate = false; TSWindow[n].IsTemplate = false;
            IsDemoSong = false; TSWindow[n].IsDemoSong = false;

            TSWindow[n].Top = this.Top;
            TSWindow[n].Height = this.Height;
            TSWindow[n].TabControl.Height = this.ClientSize.Height;
            TSWindow[n].Tracks.Height = this.Tracks.Height;
            TSWindow[n].InterfaceBox.Top = this.InterfaceBox.Top;
            TSWindow[n].InterfaceBox.Width = this.InterfaceBox.Width;
            TSWindow[n].MoveBetweenPatternsCheckBox.Left = this.MoveBetweenPatternsCheckBox.Left;
            TSWindow[n].UseLastNoteParamsCheckBox.Left = this.UseLastNoteParamsCheckBox.Left;

            if (TSWindow[0] == null)
                return;

            if (n == 0)
            {
                if (this.Left < TSWindow[0].Left)
                {
                    childWins[0] = this;
                    childWins[1] = TSWindow[0];
                }
                else
                {
                    childWins[0] = TSWindow[0];
                    childWins[1] = this;
                }
                childWins[2] = null;
            }
            else
            {
                // 6 variations
                if (this.Left < TSWindow[0].Left && this.Left < TSWindow[1].Left)
                {
                    // leftmost
                    childWins[0] = this;
                    childWins[1] = TSWindow[0].Left < TSWindow[1].Left ? TSWindow[0] : TSWindow[1];
                    childWins[2] = TSWindow[0].Left < TSWindow[1].Left ? TSWindow[1] : TSWindow[0];
                }
                else if (this.Left > TSWindow[0].Left && this.Left < TSWindow[1].Left)
                {
                    // middle
                    childWins[1] = this;
                    childWins[0] = TSWindow[0].Left < TSWindow[1].Left ? TSWindow[0] : TSWindow[1];
                    childWins[2] = TSWindow[0].Left < TSWindow[1].Left ? TSWindow[1] : TSWindow[0];
                }
                else
                {
                    // rightmost
                    childWins[2] = this;
                    childWins[0] = TSWindow[0].Left < TSWindow[1].Left ? TSWindow[0] : TSWindow[1];
                    childWins[1] = TSWindow[0].Left < TSWindow[1].Left ? TSWindow[1] : TSWindow[0];
                }
            }

            if (!string.IsNullOrEmpty(childWins[0]?.WinFileName))
            {
                fileName = childWins[0].WinFileName;
                name = Path.GetFileName(fileName);
            }
            else if (childWins[1] != null && !string.IsNullOrEmpty(childWins[1].WinFileName))
            {
                fileName = childWins[1].WinFileName;
                name = Path.GetFileName(fileName);
            }
            else if (childWins[2] != null && !string.IsNullOrEmpty(childWins[2].WinFileName))
            {
                fileName = childWins[2].WinFileName;
                name = Path.GetFileName(fileName);
            }
            else
            {
                fileName = string.Empty;
                name = MainForm.WinCount.ToString();
            }

            childWins[0].Text = "Left TS " + name;
            childWins[0].NumModule = 1;
            childWins[0].WinFileName = fileName;

            if (n == 0)
            {
                childWins[1].Text = "Right TS " + name;
                childWins[1].NumModule = 2;
                childWins[1].WinFileName = fileName;
            }
            else
            {
                childWins[1].Text = "Mid TS " + name;
                childWins[1].NumModule = 2;
                childWins[1].WinFileName = fileName;
                childWins[2].Text = "Right TS " + name;
                childWins[2].NumModule = 3;
                childWins[2].WinFileName = fileName;
            }
        }

        public void SetToolsPattern()
        {
            Globals.GlobalTransForm.PatternNumUpDown.Value = PatternIndex;
            Globals.TracksManagerForm.L1LineUpDown.Value = PatternIndex;
            Globals.TracksManagerForm.L2PatternUpDown.Value = PatternIndex;
        }

        public void PatternLenEditKeyDown(object sender, KeyEventArgs e)
        {
            int i;
            switch (e.KeyCode)
            {
                case Keys.Prior:
                    i = (int)(PatternLenUpDown.Value + Tracks.HLStep);
                    if (i > VTModule.MaxPatternLength)
                        i = VTModule.MaxPatternLength;
                    PatternLenUpDown.Value = i;
                    break;
                case Keys.Next:
                    i = (int)(PatternLenUpDown.Value - Tracks.HLStep);
                    if (i <= 0)
                        i = 1;
                    PatternLenUpDown.Value = i;
                    break;
            }
        }

        public void EnvelopeAsNoteCheckBox_MouseUp(object sender, MouseEventArgs e)
        {
            Tracks.Focus();
        }

        public void AutoNumeratePatterns()
        {
            int startPos, destPos, patternNumber;
            int lastPatternLength, lastPatternNumber;

            if (PositionsGrid.Selection.Left < VTM.Positions.Length)
                return;

            SavePositionsUndo();
            SongChanged = true;
            BackupSongChanged = true;

            startPos = VTM.Positions.Length;
            destPos = PositionsGrid.Selection.Left;
            patternNumber = Int32.MaxValue;

            // Get length of last pattern
            if (startPos > 0)
                lastPatternNumber = VTM.Positions.Value[startPos - 1];
            else
                lastPatternNumber = VTM.Positions.Value[startPos];

            lastPatternLength = VTM.Patterns[lastPatternNumber].Length;

            // Increase track length
            IncreaseTrackLength(destPos - startPos + 1);

            // Create new patterns
            for (int i = startPos; i <= destPos; i++)
            {
                patternNumber++;

                if (patternNumber > VTModule.MaxPatternIndex)
                    return;

                ValidatePattern2(patternNumber);

                // The length of new patterns should be the same as the length of last pattern
                VTM.Patterns[patternNumber].Length = lastPatternLength;
                VTM.Positions.Value[i] = patternNumber;
            }

            RedrawPatternPositions();
            SelectPosition(destPos);
            SetPositionsGridScroll(-1);
            CalcTotLen();
        }

        // 
        // procedure TMDIChild.SmartRedraw;
        // begin
        // 
        // case PageControl1.ActivePageIndex of
        // 
        // 0: with Tracks do begin
        // RedrawTracks();
        // RemoveSelection(0, True);
        // HideCaret;
        // CreateMyCaret;
        // SetCaretPosition;
        // ShowCaret;
        // end;
        // 
        // 1: with Samples do begin
        // RedrawSamples();
        // HideCaret;
        // CreateMyCaret;
        // SetCaretPosition;
        // ShowCaret;
        // end;
        // 
        // 2: with Ornaments do
        // begin
        // RedrawOrnaments();
        // HideCaret;
        // CursorX := 0;
        // CreateCaret(Handle, 0, CelW * 3, CelH);
        // SetCaretPosition;
        // ShowCaret;
        // end;
        // end;
        // end;
        /* private void WMEnterSizeMove(ref TWMMove Message)
        {
            this.DisableAlign;
        }

        private void WMExitSizeMove(ref TWMMove Message)
        {
            this.EnableAlign;
        } */

        /* private void MyResizeBegin(object sender, EventArgs e)
        {
            this.SuspendLayout();
        }

        private void MyResizeEnd(object sender, EventArgs e)
        {
            this.ResumeLayout();
        } */

        protected override void OnResizeBegin(EventArgs e)
        {
            base.OnResizeBegin(e);
            SuspendLayout();
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            ResumeLayout(false);
            PerformLayout();
        }

        // procedure SmartRedraw;
        public void RedrawOff()
        {
            SendMessage(this.Handle, WM_SETREDRAW, 0, 0);
        }

        public void RedrawOn()
        {
            SendMessage(this.Handle, WM_SETREDRAW, 1, 0);
        }

        public void InvalidateChild()
        {
            RECT rect;

            // PATTERNS tab
            if (TabControl.SelectedTab == PatternsTab)
            {
                // Invalidate Tracks
                if (!Tracks.IsDisposed)
                    RedrawWindow(Tracks.Handle, IntPtr.Zero, IntPtr.Zero, RDW_INVALIDATE | RDW_NOERASE | RDW_NOINTERNALPAINT | RDW_UPDATENOW);

                // Invalidate child bootom
                rect.Top = PatternsTab.Top + InterfaceBox.Top;
                rect.Left = Tracks.Left;
                rect.Right = this.ClientSize.Width;
                rect.Bottom = this.ClientSize.Height;
                RedrawWindow(this.Handle, ref rect, IntPtr.Zero, RDW_INVALIDATE | RDW_NOERASE | RDW_NOINTERNALPAINT | RDW_ALLCHILDREN | RDW_UPDATENOW);

                // Invalidate left border
                rect.Top = this.ClientSize.Height / 2;
                rect.Left = 0;
                rect.Right = PatternsTab.Left;
                rect.Bottom = this.ClientSize.Height;
                this.Invalidate(Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom));

                // Invalidate right border
                rect.Top = this.ClientSize.Height / 2;
                rect.Left = this.ClientSize.Width - (this.ClientSize.Width - Tracks.Left - Tracks.Width);
                rect.Right = this.ClientSize.Width;
                rect.Bottom = this.ClientSize.Height;
                this.Invalidate(Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom));
            }

            // SAMPLES tab
            if (TabControl.SelectedTab == SamplesTab)
            {
                // Invalidate sample editor
                RedrawWindow(Samples.Handle, IntPtr.Zero, IntPtr.Zero, RDW_NOERASE | RDW_INVALIDATE | RDW_UPDATENOW);

                // Invalidate window boottom
                rect.Top = SampleEditBox.Top + SampleEditBox.Height + 11;
                rect.Left = 0;
                rect.Right = this.ClientSize.Width;
                rect.Bottom = this.ClientSize.Height;
                RedrawWindow(this.Handle, ref rect, IntPtr.Zero, RDW_NOERASE | RDW_INVALIDATE | RDW_ALLCHILDREN | RDW_UPDATENOW);

                // Invalidate left border
                rect.Top = this.ClientSize.Height / 2;
                rect.Left = 0;
                rect.Right = 13;
                rect.Bottom = this.ClientSize.Height;
                RedrawWindow(this.Handle, ref rect, IntPtr.Zero, RDW_NOERASE | RDW_INVALIDATE | RDW_ALLCHILDREN | RDW_UPDATENOW);

                // Invalidate samples browser
                rect.Top = SampleBrowserBox.Top;
                rect.Left = SampleBrowserBox.Left - 3;
                rect.Right = this.ClientSize.Width;
                rect.Bottom = this.ClientSize.Height;
                RedrawWindow(this.Handle, ref rect, IntPtr.Zero, RDW_NOERASE | RDW_NOINTERNALPAINT | RDW_INVALIDATE | RDW_ALLCHILDREN | RDW_UPDATENOW);
            }

            // ORNAMENTS tab
            if (TabControl.SelectedTab == OrnamentsTab)
            {
                // Invalidate ornaments editor
                RedrawWindow(Ornaments.Handle, IntPtr.Zero, IntPtr.Zero, RDW_NOINTERNALPAINT | RDW_INVALIDATE | RDW_UPDATENOW);

                // Invalidate window boottom
                rect.Top = OrnamentEditBox.Top + OrnamentEditBox.Height + 11;
                rect.Left = 0;
                rect.Right = this.ClientSize.Width;
                rect.Bottom = this.ClientSize.Height;
                RedrawWindow(this.Handle, ref rect, IntPtr.Zero, RDW_NOINTERNALPAINT | RDW_INVALIDATE | RDW_ALLCHILDREN | RDW_UPDATENOW);

                // Invalidate left border
                rect.Top = this.ClientSize.Height / 2;
                rect.Left = 0;
                rect.Right = 13;
                rect.Bottom = this.ClientSize.Height;
                RedrawWindow(this.Handle, ref rect, IntPtr.Zero, RDW_NOINTERNALPAINT | RDW_INVALIDATE | RDW_ALLCHILDREN | RDW_UPDATENOW);

                // Invalidate ornaments browser
                rect.Top = OrnamentsBrowserBox.Top;
                rect.Left = OrnamentsBrowserBox.Left - 3;
                rect.Right = this.ClientSize.Width;
                rect.Bottom = this.ClientSize.Height;
                RedrawWindow(this.Handle, ref rect, IntPtr.Zero, RDW_NOINTERNALPAINT | RDW_INVALIDATE | RDW_ALLCHILDREN | RDW_UPDATENOW);
            }

            // OPT / INFO tab
            if (TabControl.SelectedTab == OptionsTab || TabControl.SelectedTab == InfoTab)
            {
                // Invalidate window
                RedrawWindow(this.Handle, IntPtr.Zero, IntPtr.Zero, RDW_FRAME | RDW_INVALIDATE | RDW_ALLCHILDREN | RDW_UPDATENOW);
            }
        }

        public void ChildWin_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
                return;

            if (MainForm.ChildsEventsBlocked)
                return;

            MainForm.ChildsEventsBlocked = true;

            if (TSWindow[0] == null)
                RedrawOff();
            else
                Globals.MainForm.RedrawOff();

            TabControl.Height = this.ClientSize.Height;
            HeightChanged = true;
            LastHeight = TabControl.Height;
            Globals.MainForm.LastChildHeight = this.Height;

            AutoResizeForm();

            if (TSWindow[0] != null)
            {
                TSWindow[0].Top = this.Top;
                TSWindow[0].TabControl.Height = this.ClientSize.Height;
                TSWindow[0].ClientSize = new Size(TSWindow[0].ClientSize.Width, this.ClientSize.Height);
                TSWindow[0].HeightChanged = true;
                TSWindow[0].LastHeight = LastHeight;
                TSWindow[0].AutoResizeForm();
            }

            if (TSWindow[1] != null)
            {
                TSWindow[1].Top = this.Top;
                TSWindow[1].TabControl.Height = this.ClientSize.Height;
                TSWindow[1].ClientSize = new Size(TSWindow[0].ClientSize.Width, this.ClientSize.Height);
                TSWindow[1].HeightChanged = true;
                TSWindow[1].LastHeight = LastHeight;
                TSWindow[1].AutoResizeForm();
            }

            if (TSWindow[0] == null)
            {
                RedrawOn();
                InvalidateChild();
            }
            else
                Globals.MainForm.RedrawOn();

            MainForm.ChildsEventsBlocked = false;
        }

        public void UnloopSampleButton_Click(object sender, EventArgs e)
        {
            int nextLine, lastLine, unloopCount, loopLength;
            bool calcSlides, tilTheEnd, done;
            short lineTone, freqAccum, lineNoise, noiseAccum, lineAmplitude, amplitudeAccum;
            SampleTick sampleTick;
            Sample sample;

            if (Samples.ShownSample == null)
                return;

            Samples.IsSelecting = false;
            sample = Samples.ShownSample;
            nextLine = sample.Length;
            lastLine = VTModule.MaxSampleLength - 1;
            loopLength = sample.Length - sample.Loop;
            done = false;

            // If next line goes beyond the sample, then there is nowhere to unloop
            if (nextLine > lastLine)
            {
                MessageBox.Show(this, 
                    "Unloop is not possible because the sample has a maximum length." +
                    "\r\nThere is nowhere to unloop.",
                    Application.ProductName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            SaveSampleUndo(sample);
            SongChanged = true;
            BackupSongChanged = true;

            using (UnloopForm unloopForm = new UnloopForm())
            {
                if (unloopForm.ShowDialog(Globals.MainForm) == DialogResult.Cancel)
                    return;

                unloopCount = (int)unloopForm.UnloopUpDown.Value;
                calcSlides = unloopForm.CalcSlides.Checked;
            }

            tilTheEnd = unloopCount == 0;

            do
            {
                for (int i = sample.Loop; i < sample.Length; i++)
                {
                    sample.Ticks[nextLine] = sample.Ticks[i];

                    if (nextLine == lastLine)
                    {
                        sample.Length = (byte)(lastLine + 1);
                        sample.Loop = (byte)lastLine;
                        done = true;
                        break;
                    }
                    else
                        nextLine++;
                }

                if (!tilTheEnd && unloopCount == 1)
                    done = true;
                else
                    unloopCount--;
            }
            while (!done);

            if (!tilTheEnd && nextLine != lastLine)
            {
                sample.Length = (byte)nextLine;
                sample.Loop = (byte)(nextLine - loopLength);
            }

            SampleLengthUpDown.Value = sample.Length;
            SampleLoopUpDown.Value = sample.Loop;

            if (!calcSlides)
            {
                Samples.Redraw();
                SaveSampleRedo();
                return;
            }

            // Re-calculate tone, noise and volume accumulation
            freqAccum = 0;
            noiseAccum = 0;
            amplitudeAccum = 0;

            for (int i = 0; i < sample.Length; i++)
            {
                sampleTick = sample.Ticks[i];

                // Re-calculate Tone Shift
                lineTone = (short)(sampleTick.AddToTone + freqAccum);

                if (sampleTick.Ton_Accumulation)
                    freqAccum += sampleTick.AddToTone;

                if (lineTone > 0xFFF || lineTone < -0xFFF)
                    lineTone = (short)(lineTone & 0xFFF);

                sampleTick.AddToTone = lineTone;
                sampleTick.Ton_Accumulation = false;

                // Re-calculate Noise or Envelope
                lineNoise = (short)(sampleTick.Add_to_Envelope_or_Noise + noiseAccum);

                if (sampleTick.Envelope_or_Noise_Accumulation)
                    noiseAccum += sampleTick.Add_to_Envelope_or_Noise;

                sampleTick.Add_to_Envelope_or_Noise = (sbyte)(lineNoise & 31);
                sampleTick.Envelope_or_Noise_Accumulation = false;

                // Re-calculate Amplitude
                lineAmplitude = sampleTick.Amplitude;
                if (sampleTick.Amplitude_Sliding)
                {
                    if (sampleTick.Amplitude_Slide_Up)
                    {
                        if (amplitudeAccum < 15)
                            amplitudeAccum++;
                    }
                    else
                    {
                        if (amplitudeAccum > -15)
                            amplitudeAccum--;
                    }
                }

                lineAmplitude += amplitudeAccum;

                if (lineAmplitude < 0)
                    lineAmplitude = 0;

                if (lineAmplitude > 15)
                    lineAmplitude = 15;

                sampleTick.Amplitude = (byte)lineAmplitude;
                sampleTick.Amplitude_Sliding = false;
                sampleTick.Amplitude_Slide_Up = false;
            }

            Samples.Redraw();
            SaveSampleRedo();
        }

        public void SampleCopyToEditContextPopup(object sender, Point MousePos, ref bool Handled)
        {
            Handled = true;
            Samples.Focus();
        }

        public void ChangePatternsLength(int patternsLength)
        {
            // Shortcuts
            int selectLeft = PositionsGrid.Selection.Left;
            int selectRight = PositionsGrid.Selection.Right;

            // Save positions and patterns for UNDO
            SaveTrackUndo();

            SongChanged = true;
            BackupSongChanged = true;

            for (int i = selectLeft; i <= selectRight; i++)
            {
                int patternNumber = VTM.Positions.Value[i];
                VTM.Patterns[patternNumber].Length = patternsLength;
            }

            // Update current pattern length value
            PatternLenUpDown.Value = patternsLength;

            CalcTotLen();
            Tracks.RedrawTracks();

            // Save new patterns for REDO
            SaveTrackRedo();
        }

        public bool RenumberPatterns_PatternAdded(short patternNumber)
        {
            for (int i = 0; i < ItemsTable.GetLength(0); i++)
            {
                if (patternNumber == ItemsTable[i][0])
                    return true;
            }

            return false;
        }

        public short RenumberPatterns_GetNewPatternNumber1(short OldPatternNumber)
        {
            for (int i = 0; i < ItemsTable.GetLength(0); i++)
            {
                if (OldPatternNumber == ItemsTable[i][0])
                    return ItemsTable[i][1];
            }

            return 0;
        }

        public void RenumberPatterns()
        {
            int newNumber, oldNumber, i, j;

            Pattern[] newPatterns = new Pattern[Convert.ToInt32(VTModule.MaxPatternIndex) + 1];
            Pattern newReservePattern = null;

            for (i = 0; i < newPatterns.Length; i++)
                newPatterns[i] = null;

            newReservePattern = VTM.ReservedPattern;

            // Save undo information
            SavePositionsUndo();

            // Init items dictionary:
            // [i][0] - old pattern number
            // [i][1] - new pattern number
            ItemsTable = new short[VTM.Positions.Length][];
            for (i = 0; i < ItemsTable.GetLength(0); i++)
            {
                Array.Resize(ref ItemsTable[i], 2);
                ItemsTable[i][0] = -1;
                ItemsTable[i][1] = -1;
            }

            // Create new numeration for old paterns numbers
            j = 0;
            newNumber = 0;

            for (i = 0; i < VTM.Positions.Length; i++)
            {
                oldNumber = VTM.Positions.Value[i];

                if (!RenumberPatterns_PatternAdded((short)oldNumber))
                {
                    ItemsTable[j][0] = (short)oldNumber;
                    ItemsTable[j][1] = (short)newNumber;
                    newPatterns[newNumber] = VTM.Patterns[oldNumber];
                    j++;
                    newNumber++;
                }
            }

            // Copy new patterns to old patterns
            for (i = newPatterns.GetLowerBound(0); i <= newPatterns.GetUpperBound(0); i++)
                VTM.Patterns[i] = newPatterns[i];

            // Renumber patterns
            for (i = 0; i < VTM.Positions.Length; i++)
                ChangePositionValueNoUndo(i, RenumberPatterns_GetNewPatternNumber1((short)VTM.Positions.Value[i]));
        }

        public void CleanPatterns()
        {
            int selectLeft = PositionsGrid.Selection.Left;
            int selectRight = PositionsGrid.Selection.Right;

            // Save positions and patterns for UNDO
            SaveTrackUndo();

            SongChanged = true;
            BackupSongChanged = true;

            int[] patterns = new int[256];
            int patternCount = 0;

            // Gather unique pattern numbers
            for (int i = selectLeft; i <= selectRight; i++)
            {
                int position = VTM.Positions.Value[i];
                bool unique = true;

                for (int j = 0; j < patternCount; j++)
                {
                    if (patterns[j] == position)
                    {
                        unique = false;
                        break;
                    }
                }

                if (unique)
                {
                    patterns[patternCount] = position;

                    if (++patternCount > 255)
                        break; // shouldn't happen
                }
            }

            // Clear the unique patterns
            for (int i = 0; i < patternCount; i++)
                VTModule.CleanPattern(VTM.Patterns[patterns[i]]);

            CalcTotLen();
            Tracks.RedrawTracks();

            // Save new patterns for REDO
            SaveTrackRedo();
        }

        public void SplitPattern()
        {
            int i, j;
            int currentPatternPosition;
            int currentPatternNumber;
            int currentPatternLine;
            int currentPatternLength;
            int currentPatternNewLength;
            int newPatternLength;
            int newPatternNumber;
            int newPatternPosition;
            Rectangle selectionRect;

            if (Tracks.SelectionY == 0)
                return;

            // Disable autoupdate UpDown controls
            DisableChangingEx = true;
            selectionRect = Rectangle.FromLTRB(PositionsGrid.Selection.Left, 0, PositionsGrid.Selection.Left, 0);
            PositionsGrid.Selection = selectionRect;

            // Save positions and patterns state for UNDO
            SaveTrackUndo();

            currentPatternPosition = PositionIndex;
            currentPatternNumber = PatternIndex;
            currentPatternLine = Tracks.SelectionY;
            currentPatternLength = VTM.Patterns[currentPatternNumber].Length;
            currentPatternNewLength = currentPatternLine;
            newPatternLength = currentPatternLength - currentPatternNewLength;

            // Insert new position and create new pattern
            InsertPosition(false, false, false);
            newPatternPosition = currentPatternPosition + 1;
            newPatternNumber = VTM.Positions.Value[newPatternPosition];

            // Copy pattern data from current pattern to new by Track Manager
            Globals.TracksManagerForm.EnvelopeColumn.Checked = true; // Flag: copy envelope data ON
            Globals.TracksManagerForm.NoiseColumn.Checked = true; // Flag: copy noise data ON
            Globals.TracksManagerForm.TracksOp(currentPatternNumber, currentPatternLine, 0, newPatternNumber, 0, 0, 0, false); // Copy chan A
            Globals.TracksManagerForm.TracksOp(currentPatternNumber, currentPatternLine, 1, newPatternNumber, 0, 1, 0, false); // Copy chan B
            Globals.TracksManagerForm.TracksOp(currentPatternNumber, currentPatternLine, 2, newPatternNumber, 0, 2, 0, false); // Copy chan C
            Globals.TracksManagerForm.EnvelopeColumn.Checked = false;
            Globals.TracksManagerForm.NoiseColumn.Checked = false;

            // Change current pattern length
            VTM.Patterns[currentPatternNumber].Length = currentPatternNewLength;
            PatternLenUpDown.Value = currentPatternNewLength;

            // Change new pattern length
            VTM.Patterns[newPatternNumber].Length = newPatternLength;

            // Clear current pattern lines > current line
            for (i = currentPatternLine; i < VTModule.MaxPatternLength; i++)
            {
                Line line = VTM.Patterns[currentPatternNumber].Lines[i];
                line.Noise = 0;
                line.Envelope = 0;

                for (j = 0; j < 3; j++)
                {
                    line.Channel[j].Note = -1;
                    line.Channel[j].Sample = 0;
                    line.Channel[j].Ornament = 0;
                    line.Channel[j].Volume = 0;
                    line.Channel[j].Envelope = 0;
                    line.Channel[j].AdditionalCommand.Number = 0;
                    line.Channel[j].AdditionalCommand.Delay = 0;
                    line.Channel[j].AdditionalCommand.Parameter = 0;
                }
            }

            // Enable autoupdate UpDown controls
            DisableChangingEx = false;

            // Set pattern editor cursor to the first line and on the Channel A note
            Tracks.ShownFrom = 0;
            Tracks.CursorY = Tracks.CenterLineIndex;
            Tracks.CursorX = 8;

            // Redraw tracks, cursor and selection
            Tracks.RemoveSelection();
            Tracks.HideCaret();
            Tracks.RedrawTracks();
            Tracks.SetCaretPosition();
            Tracks.ShowCaret();

            // Set position to a new pattern
            SelectPosition2(newPatternPosition);

            // Save new patterns state for REDO
            SaveTrackRedo();
        }

        public void ExpandPattern()
        {
            int patternLength = VTModule.DefaultPatternLength;
            int newPatternLength;
            Pattern oldPattern;

            if (VTM.Patterns[PatternIndex] != null)
                patternLength = VTM.Patterns[PatternIndex].Length;

            newPatternLength = patternLength * 2;

            if (newPatternLength > VTModule.MaxPatternLength)
            {
                MessageBox.Show(this, $"To Expand Pattern Twice the Original Size it Must be {(VTModule.MaxPatternLength / 2)} Lines or Less.", Application.ProductName);
                return;
            }

            SongChanged = true;
            BackupSongChanged = true;
            ValidatePattern2(PatternIndex);

            oldPattern = (Pattern)VTM.Patterns[PatternIndex].Clone();
            AddUndo(TChangeAction.ExpandCompressPattern, 0, 0);
            ChangeList[ChangeCount - 1].Pattern = oldPattern;
            VTM.Patterns[PatternIndex].Length = newPatternLength;
            PatternLenUpDown.Value = newPatternLength;

            for (int i = patternLength - 1; i >= 0; i--)
            {
                Line line = VTM.Patterns[PatternIndex].Lines[i * 2 + 1];
                line.Envelope = 0;
                line.Noise = 0;
                line.Channel[0] = new ChannelLine();
                line.Channel[1] = new ChannelLine();
                line.Channel[2] = new ChannelLine();
                VTM.Patterns[PatternIndex].Lines[i * 2] = VTM.Patterns[PatternIndex].Lines[i];
            }

            CheckTracksAfterSizeChanged(newPatternLength);
        }

        public void CompressPattern()
        {
            int patLen = VTModule.DefaultPatternLength;
            int newPatternLength;
            Pattern oldPattern;

            if (VTM.Patterns[PatternIndex] != null)
                patLen = VTM.Patterns[PatternIndex].Length;

            newPatternLength = patLen / 2;

            if (newPatternLength <= 0)
            {
                MessageBox.Show(this, "To Shrink Pattern by Half it Must be 2 Lines or More.", Application.ProductName);
                return;
            }

            SongChanged = true;
            BackupSongChanged = true;
            ValidatePattern2(PatternIndex);

            oldPattern = (Pattern)VTM.Patterns[PatternIndex].Clone();
            AddUndo(TChangeAction.ExpandCompressPattern, 0, 0);
            ChangeList[ChangeCount - 1].Pattern = oldPattern;
            VTM.Patterns[PatternIndex].Length = newPatternLength;
            PatternLenUpDown.Value = newPatternLength;

            for (int i = 0; i < newPatternLength; i++)
                VTM.Patterns[PatternIndex].Lines[i] = VTM.Patterns[PatternIndex].Lines[i * 2];

            for (int i = newPatternLength; i < VTModule.MaxPatternLength; i++)
            {
                Line line = VTM.Patterns[PatternIndex].Lines[i];
                line.Envelope = 0;
                line.Noise = 0;
                line.Channel[0] = new ChannelLine();
                line.Channel[1] = new ChannelLine();
                line.Channel[2] = new ChannelLine();
            }

            CheckTracksAfterSizeChanged(newPatternLength);
        }

        public void PackPattern()
        {
            PatternsPacker patternsPacker;
            int fromLine;
            int toLine;

            // Calculate from and to lines
            if (!Tracks.IsSelected())
            {
                fromLine = 0;
                toLine = Tracks.ShownPattern.Length - 1;
            }
            else
            {
                fromLine = Tracks.SelectionY;
                toLine = Tracks.CurrentPatternLine();

                if (fromLine > toLine)
                {
                    fromLine = toLine;
                    toLine = Tracks.SelectionY;
                }
            }

            // Too small to pack
            if (toLine - fromLine < 2)
            {
                MessageBox.Show(this, "Pattern Block Too Small to Pack.", Application.ProductName);
                return;
            }

            // Init packer
            patternsPacker = new PatternsPacker();
            patternsPacker.Pattern = Tracks.ShownPattern;
            patternsPacker.FromLine = fromLine;
            patternsPacker.ToLine = toLine;

            // Is pattern packable?
            if (patternsPacker.CantPack())
                return;

            // Pack
            SavePatternUndo();
            patternsPacker.Process();

            // Redraw
            Tracks.HideCaret();
            Tracks.ShownFrom = fromLine;
            Tracks.CursorY = Tracks.CenterLineIndex;
            Tracks.RemoveSelection();
            Tracks.SetCaretPosition();
            Tracks.RedrawTracks();
            Tracks.ShowCaret();

            PatternLenUpDown.Value = Tracks.ShownPattern.Length;

            SavePatternRedo();
        }

        public void ShowHintTimer_Tick(object sender, EventArgs e)
        {
            if (MainForm.DisableHints)
                return;

            ShowHintTimer.Enabled = false;
            HideHintTimer.Interval = HideHintDelay;
            HideHintTimer.Enabled = true;
            Point cursorPos = Cursor.Position;
            //Application.ActivateHint(aPoint);
            ToolTip.Show("Hint", this, cursorPos);
        }

        public void HideHintTimer_Tick(object sender, EventArgs e)
        {
            HideHintTimer.Enabled = false;
            //Application.CancelHint;
            ToolTip.Hide(this);
        }

        public void ChangeBackupVersion_Tick(object sender, EventArgs e)
        {
            if (!BackupSongChanged)
                return;

            BackupVersionCounter++;
        }

        public void PrepareExportDialog(SaveFileDialog saveFileDialog, string ext, string initDir)
        {
            string openPath = "";

            // Prepare filename
            if (IsTemplate || IsDemoSong)
                saveFileDialog.FileName = Path.ChangeExtension(this.Text, ext);
            else if (WinFileName != "")
                saveFileDialog.FileName = Path.ChangeExtension(WinFileName, ext);
            else if (TSWindow[0] != null && TSWindow[0].WinFileName != "")
                saveFileDialog.FileName = Path.ChangeExtension(TSWindow[0].WinFileName, ext);
            else if (TSWindow[1] != null && TSWindow[1].WinFileName != "")
                saveFileDialog.FileName = Path.ChangeExtension(TSWindow[1].WinFileName, ext);
            else
                saveFileDialog.FileName = "MyBestTrack" + ext;

            // Prepare initial dir
            if (saveFileDialog.InitialDirectory == "")
            {
                if (initDir != "")
                    openPath = initDir;
                else
                    openPath = Path.GetDirectoryName(Globals.MainForm.RecentFiles[0]);

                if (Directory.Exists(openPath))
                    saveFileDialog.InitialDirectory = openPath;

                if (string.IsNullOrEmpty(openPath))
                    openPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                saveFileDialog.FileName = Path.GetFileName(saveFileDialog.FileName);
            }
            else
                saveFileDialog.FileName = Path.GetFileName(saveFileDialog.FileName);

            //saveFileDialog.FileName = saveFileDialog.FileName.Replace("\\\\", "\\");
        }

        public void PrepareExportDialog(SaveFileDialog saveFileDialog, string extension)
        {
            PrepareExportDialog(saveFileDialog, extension, "");
        }

        public bool ExportToWavFile_IsOpen(string fileName)
        {
            if (!File.Exists(fileName))
                return false;

            try
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    return false;
            }
            catch (IOException)
            {
                return true;
            }
        }

        public void ExportToWavFile()
        {
            bool trying = true;

            SetPlayingWindow(0, this);

            if (PlayingWindow[0] == PlayingWindow[1])
                SetPlayingWindow(1, null);

            if (PlayingWindow[0] == PlayingWindow[2])
                SetPlayingWindow(2, null);

            if (PlayingWindow[0].TSWindow[0] != null && PlayingWindow[0].TSWindow[0].VTM.Positions.Length != 0 &&
                PlayingWindow[0].TSWindow[1] != null && PlayingWindow[0].TSWindow[1].VTM.Positions.Length != 0)
            {
                SetPlayingWindow(1, PlayingWindow[0].TSWindow[0]);
                SetPlayingWindow(2, PlayingWindow[0].TSWindow[1]);
                AY.ChipCount = 3;
            }
            else if (PlayingWindow[0].TSWindow[0] != null && PlayingWindow[0].TSWindow[0].VTM.Positions.Length != 0)
            {
                SetPlayingWindow(1, PlayingWindow[0].TSWindow[0]);
                SetPlayingWindow(2, null);
                AY.ChipCount = 2;
            }
            else
            {
                SetPlayingWindow(1, null);
                SetPlayingWindow(2, null);
                AY.ChipCount = 1;
            }

            while (trying)
            {
                Globals.ExportWavOptionsForm.ExportSelected.Checked =
                    (PlayingWindow[0].PositionsGrid.Selection.Right > PlayingWindow[0].PositionsGrid.Selection.Left) ||
                    (AY.ChipCount >= 2 && PlayingWindow[1].PositionsGrid.Selection.Right > PlayingWindow[1].PositionsGrid.Selection.Left) ||
                    (AY.ChipCount == 3 && PlayingWindow[2].PositionsGrid.Selection.Right > PlayingWindow[2].PositionsGrid.Selection.Left);

                if (Globals.ExportWavOptionsForm.ShowDialog(this) == DialogResult.Cancel)
                    return;

                PrepareExportDialog(ExportWavDialog, ".wav", MainForm.ExportPath);

                if (ExportWavDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = ExportWavDialog.FileName;

                    if (ExportToWavFile_IsOpen(fileName))
                    {
                        MessageBox.Show(this, "Can\'t Open File. File is Already Opened in Another Application.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    ExportWavDialog.InitialDirectory = Path.GetDirectoryName(fileName);
                    MainForm.ExportPath = ExportWavDialog.InitialDirectory;

                    //Globals.MainForm.DisableControlsForExport();

                    WaveAyumi.IsYm = Globals.ExportWavOptionsForm.GetChip() == ChipType.YM;
                    WaveAyumi.SampleRate = Globals.ExportWavOptionsForm.GetSampleRate();
                    WaveAyumi.BitRate = (short)Globals.ExportWavOptionsForm.GetBitRate();
                    WaveAyumi.NumChannels = (short)Globals.ExportWavOptionsForm.GetNumChannels();
                    WaveAyumi.ExportSelected = Globals.ExportWavOptionsForm.ExportSelected.Checked;
                    WaveAyumi.ExportSeparate = Globals.ExportWavOptionsForm.ExportSeparate.Checked;
                    WaveAyumi.IsrStep = (PlayingWindow[0].VTM.IntFreq / 1000.0) / Globals.ExportWavOptionsForm.GetSampleRate();

                    WaveOutAPI.ExportLoops = Globals.ExportWavOptionsForm.GetRepeats();

                    string created = WaveAyumi.CreateWaveAyumi(fileName);

                    //if (Globals.ExportOptions.OpenFolder.Checked)
                    //    OpenFolderAndSelectFiles(created);

                    trying = false;
                }
            }

            /* PlayingWindow[1] = this;

            if (PlayingWindow[1] == PlayingWindow[2])
                PlayingWindow[2] = null;

            if (PlayingWindow[1].TSWindow[0] != null && PlayingWindow[1].TSWindow.VTM.Positions.Length != 0)
            {
                PlayingWindow[2] = PlayingWindow[1].TSWindow;
                AY.NumberOfSoundChips = 2;
            }
            else
            {
                PlayingWindow[2] = null;
                AY.NumberOfSoundChips = 1;
            }

            Globals.ExportOptions = new TExportWavOptions();
            Globals.ExportOptions.ExportSelected.Checked = (PlayingWindow[1].PositionsGrid.Selection.Right > PlayingWindow[1].PositionsGrid.Selection.Left) || ((AY.NumberOfSoundChips == 2) && (PlayingWindow[2].PositionsGrid.Selection.Right > PlayingWindow[2].PositionsGrid.Selection.Left));
            
            if (Globals.ExportOptions.ShowDialog(Globals.MainForm) == DialogResult.Cancel)
                return;

            PrepareExportDialog(ExportWavDialog, ".wav", MainForm.ExportPath);
            
            if (ExportWavDialog.ShowDialog() == DialogResult.OK)
            {
                if (ExportToWavFile_IsOpen(ExportWavDialog.FileName))
                {
                    MessageBox.Show(this, "Can\'t Open File. File is Already Opened in Another Application.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ExportWavDialog.InitialDirectory = Path.GetDirectoryName(ExportWavDialog.FileName);
                MainForm.ExportPath = ExportWavDialog.InitialDirectory;
                // CreateWave(ExportWavDialog.FileName);
                WaveAyumi.CreateWaveAyumi(ExportWavDialog.FileName);
            }
            else
            {
                this.ChildWinTabControl.Invalidate();
                this.ChildWinTabControl.Update();
            } */
        }

        public void ChildWin_Paint(object sender, PaintEventArgs e)
        {
            //this.Canvas.BackColor = MainForm.CFullScreenBackground;
            // Canvas.BackColor := clRed;
            //this.Canvas.FillRect(new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height));

            //using (SolidBrush brush = new SolidBrush(MainForm.CFullScreenBackground))
            //    e.Graphics.FillRectangle(brush, new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height));
        }

        public void ChildWin_DoubleClick(object sender, EventArgs e)
        {
            //Globals.MainForm.FileOpen1Execute(sender, e);
        }

        public void ClearSampleButton_Click(object sender, EventArgs e)
        {
            StopAndRestoreControls();

            SaveSampleUndo(Samples.ShownSample);
            ClearShownSample();
            SamplesSelectionOff();
            SaveSampleRedo();

            SongChanged = true;
            BackupSongChanged = true;

            Samples.HideCaret();
            Samples.Redraw();
            Samples.ShowCaret();

            if (Samples.CanFocus)
                Samples.Focus();
        }

        public void UnsetFocus(KeyPressEventArgs e, Control control)
        {
            if (e.KeyChar == '\r')
            {
                if (control.CanFocus)
                    control.Focus();
                else
                    this.ActiveControl = null;

                e.Handled = true;
            }
        }

        public void TitleTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            UnsetFocus(e, Tracks);
        }

        public void AuthorTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            UnsetFocus(e, Tracks);
        }

        public void PatternLenEditKeyPress(object sender, KeyPressEventArgs e)
        {
            UnsetFocus(e, Tracks);
        }

        public void SpeedBpmEditKeyPress(object sender, KeyPressEventArgs e)
        {
            UnsetFocus(e, Tracks);
        }

        public void OctaveEditKeyPress(object sender, KeyPressEventArgs e)
        {
            UnsetFocus(e, Tracks);
        }

        public void AutoStepEditKeyPress(object sender, KeyPressEventArgs e)
        {
            UnsetFocus(e, Tracks);
        }

        public void Edit17KeyPress(object sender, KeyPressEventArgs e)
        {
            UnsetFocus(e, Tracks);
        }

        public void PatternNumEditKeyPress(object sender, KeyPressEventArgs e)
        {
            UnsetFocus(e, Tracks);
        }

        public void UseLastNoteParamsCheckBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.DupNoteParams)
            {
                UseLastNoteParamsCheckBox.Checked = false;
                MainForm.DupNoteParams = false;
            }
            else
            {
                UseLastNoteParamsCheckBox.Checked = true;
                MainForm.DupNoteParams = true;
            }

            Globals.MainForm.ChangeDupNoteParams();
            Tracks.Focus();
        }

        public void MoveBetweenPatternsCheckBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (MainForm.MoveBetweenPatrns)
            {
                MoveBetweenPatternsCheckBox.Checked = false;
                MainForm.MoveBetweenPatrns = false;
            }
            else
            {
                MoveBetweenPatternsCheckBox.Checked = true;
                MainForm.MoveBetweenPatrns = true;
            }

            Globals.MainForm.ChangeBetweenPatterns();
            Tracks.Focus();
        }

        public void SampleNumEditKeyPress(object sender, KeyPressEventArgs e)
        {
            UnsetFocus(e, Samples);
        }

        public void SampleLenEditKeyPress(object sender, KeyPressEventArgs e)
        {
            UnsetFocus(e, Samples);
        }

        public void SampleCopyToEditKeyPress(object sender, KeyPressEventArgs e)
        {
            UnsetFocus(e, Samples);
        }

        public void SampleLoopEditKeyPress(object sender, KeyPressEventArgs e)
        {
            UnsetFocus(e, Samples);
        }

        public void OrnamentNumEditKeyPress(object sender, KeyPressEventArgs e)
        {
            UnsetFocus(e, Ornaments);
        }

        public void OrnamentLenEditKeyPress(object sender, KeyPressEventArgs e)
        {
            UnsetFocus(e, Ornaments);
        }

        public void OrnamentCopyToEditKeyPress(object sender, KeyPressEventArgs e)
        {
            UnsetFocus(e, Ornaments);
        }

        public void OrnamentLoopEditKeyPress(object sender, KeyPressEventArgs e)
        {
            UnsetFocus(e, Ornaments);
        }

        public void PositionsGrid_KeyUp(object sender, KeyEventArgs e)
        {
            if (Tracks.IsTrackPlaying() && e.KeyCode == Keys.Right && PositionsGrid.Selection.Left >= VTM.Positions.Length - 1)
            {
                PositionsGrid.Selection = Rectangle.FromLTRB(VTM.Positions.Length - 1, 0, VTM.Positions.Length - 1, 0);
                // SelectPosition(Sel.Left);
            }
        }

        public void EnvelopeAsNoteCheckBox_Click(object sender, EventArgs e)
        {
            MainForm.EnvelopeAsNote = EnvelopeAsNoteCheckBox.Checked;
            Globals.MainForm.UpdateEnvelopeAsNote();
            Tracks.RedrawTracks();

            if (Tracks.Focused)
                Tracks.ShowCaret();

            if (Globals.MainForm.ActiveMdiChild == this)
                SetToolsPattern();
        }

        public void SavePSGRegisterDump(string fileName, VTM vtm, byte chip)
        {
            byte[] currRegs = new byte[14];
            byte[] nextRegs = new byte[14];

            using (FileStream fileStream = File.Open(fileName, FileMode.Create))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream, Encoding.UTF8, false))
                {
                    // Write header
                    binaryWriter.Write(80);
                    binaryWriter.Write(83);
                    binaryWriter.Write(71);
                    binaryWriter.Write(26); // 0x1A

                    // Fill zerobytes
                    for (int i = 0; i < 12; i++)
                        binaryWriter.Write(0);

                    // Zerofill current registers
                    for (int i = 0; i < 14; i++)
                        currRegs[i] = 0;

                    AY.PlayMode = PlayModes.PlayModule;

                    if (WaveOutAPI.IsPlaying)
                        WaveOutAPI.StopPlaying();

                    Globals.MainForm.DisableControlsForExport();
                    WaveOutAPI.InitForAllTypes(true);

                    // Init pointer, position, delay
                    VTModule.Module_SetPointer(vtm, chip);
                    VTModule.Module_SetDelay((sbyte)vtm.InitialDelay);
                    VTModule.Module_SetCurrentPosition(0);

                    WaveOutAPI.ExportLoops = 1;
                    AY.LoopAllowed = false;
                    WaveOutAPI.ExportStarted = true;
                    WaveOutAPI.ExportFinished = false;

                    // Next frame
                    while (VTModule.Module_PlayCurrentLine() != PlayLineResult.AllPatternsEnded)
                    {
                        binaryWriter.Write(255);
                        nextRegs = AY.SoundChip[chip].AYRegisters.Bytes;

                        for (int i = 0; i < 14; i++)
                        {
                            if (currRegs[i] != nextRegs[i])
                            {
                                binaryWriter.Write(i);
                                binaryWriter.Write(nextRegs);
                            }

                            currRegs[i] = nextRegs[i];
                        }
                    }
                }
            }

            WaveOutAPI.ExportStarted = false;
            WaveOutAPI.ExportFinished = true;

            Globals.MainForm.EnableControlsForExport();
        }

        public void ExportPSG()
        {
            string[] chipName = new string[3];

            PrepareExportDialog(ExportPSGDialog, ".psg");

            if (ExportPSGDialog.ShowDialog() == DialogResult.OK)
            {
                ExportPSGDialog.InitialDirectory = Path.GetDirectoryName(ExportPSGDialog.FileName);

                if (TSWindow[0] != null && TSWindow[1] != null)
                {
                    chipName[0] = ExportPSGDialog.FileName.Replace(".psg", ".1.psg");
                    chipName[1] = ExportPSGDialog.FileName.Replace(".psg", ".2.psg");
                    chipName[2] = ExportPSGDialog.FileName.Replace(".psg", ".3.psg");
                    SavePSGRegisterDump(chipName[0], VTM, 0);
                    SavePSGRegisterDump(chipName[1], TSWindow[0].VTM, 1);
                    SavePSGRegisterDump(chipName[2], TSWindow[1].VTM, 2);
                }
                else if (TSWindow[0] != null)
                {
                    chipName[0] = ExportPSGDialog.FileName.Replace(".psg", ".1.psg");
                    chipName[1] = ExportPSGDialog.FileName.Replace(".psg", ".2.psg");
                    SavePSGRegisterDump(chipName[0], VTM, 0);
                    SavePSGRegisterDump(chipName[1], TSWindow[0].VTM, 1);
                }
                else
                {
                    SavePSGRegisterDump(ExportPSGDialog.FileName, VTM, 0);
                }
            }
        }

        public void ExportYM()
        {
            // Prepare the dialog (e.g., set default extension, filters, etc.)
            PrepareExportDialog(ExportYMDialog, ".ym");

            // ShowDialog() returns DialogResult.OK if user clicks 'Save'
            if (ExportYMDialog.ShowDialog() != DialogResult.OK)
                return;

            // Equivalent of "ExportYMDlg.InitialDir := ExtractFilePath(...)"
            ExportYMDialog.InitialDirectory = Path.GetDirectoryName(ExportYMDialog.FileName);

            if (TSWindow[0] != null)
            {
                string fileName = Path.ChangeExtension(ExportYMDialog.FileName, $".1.ym");

                SaveYMRegisterDump(fileName, VTM, 0);

                for (int i = 0; i < TSWindow.Length; i++)
                {
                    if (TSWindow[i] == null)
                        continue;

                    fileName = Path.ChangeExtension(ExportYMDialog.FileName, $".{i + 2}.ym");

                    SaveYMRegisterDump(fileName, TSWindow[i].VTM, (byte)i);
                }
            }
            else
            {
                // Just dump to the chosen file name
                SaveYMRegisterDump(ExportYMDialog.FileName, VTM, 0);
            }
        }

        private void WriteYmHeader(BinaryWriter binaryWriter)
        {
            // 1) "YM6!LeOnArD!"
            string h = "YM6!LeOnArD!";
            // Write ASCII bytes
            binaryWriter.Write(Encoding.ASCII.GetBytes(h));

            // 2) nframes => 0 for now; 4 bytes big-endian
            uint dw = 0;
            binaryWriter.Write((byte)(dw >> 24));
            binaryWriter.Write((byte)(dw >> 16));
            binaryWriter.Write((byte)(dw >> 8));
            binaryWriter.Write((byte)dw);

            // 3) attributes => 0 for now; 4 bytes big-endian
            binaryWriter.Write((byte)0);
            binaryWriter.Write((byte)0);
            binaryWriter.Write((byte)0);
            binaryWriter.Write((byte)0);

            // 4) zero digidrums => 2 bytes (Delphi was blockwrite(dw,2) with dw=0 => little-endian)
            // But it’s 0, so either endianness is effectively the same:
            binaryWriter.Write((byte)0);
            binaryWriter.Write((byte)0);

            // 5) Master clock = 1773400 => big-endian
            dw = 1773400;
            binaryWriter.Write((byte)(dw >> 24));
            binaryWriter.Write((byte)(dw >> 16));
            binaryWriter.Write((byte)(dw >> 8));
            binaryWriter.Write((byte)dw);

            // 6) playrate => 2 bytes: fByte=0, then fByte=50
            binaryWriter.Write((byte)0);
            binaryWriter.Write((byte)50);

            // 7) loop frame => 4 bytes big-endian, zero
            dw = 0;
            binaryWriter.Write((byte)(dw >> 24));
            binaryWriter.Write((byte)(dw >> 16));
            binaryWriter.Write((byte)(dw >> 8));
            binaryWriter.Write((byte)dw);

            // 8) skip => 2 bytes zero
            binaryWriter.Write((byte)0);
            binaryWriter.Write((byte)0);

            // 9) h := 'name'#0 => 5 bytes
            //    "name\0"
            binaryWriter.Write(Encoding.ASCII.GetBytes("name\0"));

            // 10) "author\0" => 7 bytes
            binaryWriter.Write(Encoding.ASCII.GetBytes("author\0"));

            // 11) "comment\0" => 8 bytes
            binaryWriter.Write(Encoding.ASCII.GetBytes("comment\0"));
        }

        public void SaveYMRegisterDump(string fileName, VTM vtm, byte chip)
        {
            // Locals
            byte[] regs = new byte[14]; // AY register states
            ushort fWord = 0;           // always written as 2 bytes (0..65535)
            uint dw = 0;                // used for frame counts and such

            using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                using (var binaryWriter = new BinaryWriter(fileStream, Encoding.ASCII, leaveOpen: false))
                {
                    // Clear current registers
                    Array.Clear(regs);

                    // Music/engine setup
                    AY.PlayMode = PlayModes.PlayModule;
                    if (WaveOutAPI.IsPlaying)
                        WaveOutAPI.StopPlaying();

                    Globals.MainForm.DisableControlsForExport();
                    WaveOutAPI.InitForAllTypes(true);

                    // Initialize module pointer, position, delay
                    VTModule.Module_SetPointer(vtm, chip);
                    VTModule.Module_SetDelay((sbyte)vtm.InitialDelay);
                    VTModule.Module_SetCurrentPosition(0);

                    // Write the YM file header (replicates write_ym_header in Delphi)
                    WriteYmHeader(binaryWriter);

                    // Setup export flags
                    WaveOutAPI.ExportLoops = 1;
                    AY.LoopAllowed = false;
                    WaveOutAPI.ExportStarted = true;
                    WaveOutAPI.ExportFinished = false;

                    dw = 0;
                    fWord = 0;

                    // Loop until
                    while (VTModule.Module_PlayCurrentLine() != PlayLineResult.AllPatternsEnded)
                    {
                        regs = AY.SoundChip[chip].AYRegisters.Bytes;

                        // Write 14 bytes of register data
                        binaryWriter.Write(regs, 0, regs.Length);

                        // Write 2 more bytes (14..15) for effects
                        binaryWriter.Write(fWord);

                        dw++;
                    }

                    // Now we patch the number of frames into offset 12 (the nframes field)
                    // Because we wrote zero initially in big-endian, we have to jump back & overwrite it.
                    fileStream.Seek(12, SeekOrigin.Begin);
                    // The original code writes "dw" in big-endian. We'll do that manually:
                    binaryWriter.Write((byte)(dw >> 24));
                    binaryWriter.Write((byte)(dw >> 16));
                    binaryWriter.Write((byte)(dw >> 8));
                    binaryWriter.Write((byte)dw);
                }
            }

            AY.ClearRegisters();

            WaveOutAPI.ExportStarted = false;
            WaveOutAPI.ExportFinished = true;

            Globals.MainForm.EnableControlsForExport();
        }

        public void StopAndRestoreControls()
        {
            if (!WaveOutAPI.IsPlaying)
                return;

            Globals.MainForm.RestoreControls();

            if (AY.PlayMode == PlayModes.PlayModule)
                WaveOutAPI.StopPlaying();
            else
                WaveOutAPI.ResetPlaying();

            VTModule.UnlimitedDelay = false;
            PlayStopState = PlayStopState.Play;
        }

        public void SamplePreview()
        {
            if (WaveOutAPI.IsPlaying && (AY.PlayMode == PlayModes.PlayPattern || AY.PlayMode == PlayModes.PlayModule))
                return;

            if (Samples.ShownSample == null)
                return;

            if (WaveOutAPI.IsPlaying)
                WaveOutAPI.ResetPlaying();

            PlayStopTimer.Enabled = false;
            SampleTestLine.PlayCurrentNote();
            PlayStopTimer.Enabled = true;
        }

        public void OrnamentPreview()
        {
            if (WaveOutAPI.IsPlaying && (AY.PlayMode == PlayModes.PlayPattern || AY.PlayMode == PlayModes.PlayModule))
                return;

            if (Ornaments.ShownOrnament == null)
                return;

            if (WaveOutAPI.IsPlaying)
                WaveOutAPI.ResetPlaying();

            if (Ornaments.ShownOrnament.Length == 1 && Ornaments.ShownOrnament.Offsets[0] == 0)
                return;

            PlayStopTimer.Enabled = false;
            OrnamentTestLine.PlayCurrentNote();
            PlayStopTimer.Enabled = true;
        }

        public void PrevSampleButton_Click(object sender, EventArgs e)
        {
            ChangeSample(SampleIndex - 1, true, true);
            SamplePreview();
        }

        public void NextSampleButton_Click(object sender, EventArgs e)
        {
            ChangeSample(SampleIndex + 1, true, true);
            SamplePreview();
        }

        public void PrevOrnamentButton_Click(object sender, EventArgs e)
        {
            ChangeOrnament(OrnamentIndex - 1, true, true);
            OrnamentPreview();
        }

        public void NextOrnamentButton_Click(object sender, EventArgs e)
        {
            ChangeOrnament(OrnamentIndex + 1, true, true);
            OrnamentPreview();
        }

        public void ClearOrnamentButton_Click(object sender, EventArgs e)
        {
            StopAndRestoreControls();

            SaveOrnamentUndo();

            ClearShownOrnament();
            SaveOrnamentRedo();

            Ornaments.HideCaret();
            Ornaments.Redraw();
            Ornaments.ShowCaret();

            if (Ornaments.CanFocus)
                Ornaments.Focus();
        }

        public void PasteOrnamentButton_Click(object sender, EventArgs e)
        {
            if (MainForm.LastClipboard == LastClipboard.None || MainForm.LastClipboard == LastClipboard.Samples)
                return;

            StopAndRestoreControls();
            SaveOrnamentUndo();
            ClearShownOrnament();

            switch (MainForm.LastClipboard)
            {
                case LastClipboard.Ornaments:
                    PasteOrnamentFromBuffer();
                    break;
                case LastClipboard.Tracks:
                    PastePatternToOrnament();
                    break;
            }

            SaveOrnamentRedo();

            if (Ornaments.CanFocus)
                Ornaments.Focus();
        }

        public void PasteSampleButton_Click(object sender, EventArgs e)
        {
            if (MainForm.LastClipboard == LastClipboard.None)
                return;

            StopAndRestoreControls();
            SaveSampleUndo(Samples.ShownSample);
            ClearShownSample();

            switch (MainForm.LastClipboard)
            {
                case LastClipboard.Samples:
                    PasteSampleFromBuffer(true);
                    break;
                case LastClipboard.Ornaments:
                    PasteOrnamentToSample();
                    break;
                case LastClipboard.Tracks:
                    PastePatternToSample();
                    break;
            }

            SaveSampleRedo();

            if (Samples.CanFocus)
                Samples.Focus();
        }


        public void RecalcSampOrnUsage()
        {
            for (int i = 0; i < 32; i++)
            {
                SampUsage[i] = false;
                OrnUsage[i] = false;
            }

            if (VTM == null)
                return;

            for (int i = 0; i < VTM.Positions.Length; i++)
            {
                int p = VTM.Positions.Value[i];

                if (VTM.Patterns[p] == null)
                    continue;

                for (int j = 0; j < VTM.Patterns[p].Length; j++)
                {
                    for (int k = 2; k >= 0; k--)
                    {
                        var channel = VTM.Patterns[p].Lines[j].Channel[k];

                        if (channel.Sample != 0)
                            SampUsage[channel.Sample] = true;

                        if (channel.Ornament != 0)
                            OrnUsage[channel.Ornament] = true;
                    }
                }
            }
        }

        private void CopySample1_Click(object sender, EventArgs e)
        {
            CopySampleToBuffer(true);
        }

        private void PasteSample1_Click(object sender, EventArgs e)
        {
            if (MainForm.LastClipboard == LastClipboard.None)
                return;

            SaveSampleUndo(Samples.ShownSample);
            ClearShownSample();

            switch (MainForm.LastClipboard)
            {
                case LastClipboard.Samples:
                    PasteSampleFromBuffer(true);
                    break;
                case LastClipboard.Ornaments:
                    PasteOrnamentToSample();
                    break;
                case LastClipboard.Tracks:
                    PastePatternToSample();
                    break;
            }

            SaveSampleRedo();
        }

        private void CutSample1_Click(object sender, EventArgs e)
        {
            CopySampleToBuffer(true);
            ClearSample1_Click(this, e);
        }

        private void ClearSample1_Click(object sender, EventArgs e)
        {
            SaveSampleUndo(Samples.ShownSample);
            ClearShownSample();
            SamplesSelectionOff();
            SaveSampleRedo();

            SongChanged = true;
            BackupSongChanged = true;

            Samples.HideCaret();
            Samples.Redraw();
            Samples.ShowCaret();
        }

        private void CopyOrnament1_Click(object sender, EventArgs e)
        {
            CopyOrnamentToBuffer(true);
        }

        private void PasteOrnament1_Click(object sender, EventArgs e)
        {
            if (MainForm.LastClipboard == LastClipboard.None || MainForm.LastClipboard == LastClipboard.Samples)
                return;

            SaveOrnamentUndo();
            ClearShownOrnament();

            switch (MainForm.LastClipboard)
            {
                case LastClipboard.Ornaments:
                    PasteOrnamentFromBuffer();
                    break;
                case LastClipboard.Tracks:
                    PastePatternToOrnament();
                    break;
            }

            SaveOrnamentRedo();
        }

        private void CutOrnament1_Click(object sender, EventArgs e)
        {
            CopyOrnamentToBuffer(true);
            ClearOrnament1_Click(this, e);
        }

        private void ClearOrnament1_Click(object sender, EventArgs e)
        {
            SaveOrnamentUndo();
            ClearShownOrnament();
            SaveOrnamentRedo();

            Ornaments.HideCaret();
            Ornaments.Redraw();
            Ornaments.ShowCaret();
        }

        public void SwapSamples(int sample1, int sample2)
        {
            if (sample1 == 0 || sample2 == 0)
                return;

            Sample.ValidateSample(sample1, VTM);
            Sample.ValidateSample(sample2, VTM);

            var t = VTM.Samples[sample1].Length;
            VTM.Samples[sample1].Length = VTM.Samples[sample2].Length;
            VTM.Samples[sample2].Length = t;

            t = VTM.Samples[sample1].Loop;
            VTM.Samples[sample1].Loop = VTM.Samples[sample2].Loop;
            VTM.Samples[sample2].Loop = t;

            var tb = VTM.Samples[sample1].Enabled;
            VTM.Samples[sample1].Enabled = VTM.Samples[sample2].Enabled;
            VTM.Samples[sample2].Enabled = tb;

            for (int i = 0; i < 64; i++)
            {
                var tt = VTM.Samples[sample1].Ticks[i];
                VTM.Samples[sample1].Ticks[i] = VTM.Samples[sample2].Ticks[i];
                VTM.Samples[sample2].Ticks[i] = tt;
            }

            for (int i = 0; i < VTM.Patterns.Length; i++)
            {
                if (VTM.Patterns[i] == null)
                    continue;

                for (int j = 0; j < VTM.Patterns[i].Length; j++)
                {
                    for (int k = 2; k >= 0; k--)
                    {
                        ref var chan = ref VTM.Patterns[i].Lines[j].Channel[k];
                        if (chan.Sample == sample1)
                            chan.Sample = (byte)sample2;
                        else if (chan.Sample == sample2)
                            chan.Sample = (byte)sample1;
                    }
                }
            }

            RecalcSampOrnUsage();
        }

        public void SwapOrnaments(int ornament1, int ornament2)
        {
            if (ornament1 == 0 || ornament2 == 0)
                return;

            ValidateOrnament(ornament1);
            ValidateOrnament(ornament2);

            var t = VTM.Ornaments[ornament1].Length;
            VTM.Ornaments[ornament1].Length = VTM.Ornaments[ornament2].Length;
            VTM.Ornaments[ornament2].Length = t;

            t = VTM.Ornaments[ornament1].Loop;
            VTM.Ornaments[ornament1].Loop = VTM.Ornaments[ornament2].Loop;
            VTM.Ornaments[ornament2].Loop = t;

            var tb = VTM.Ornaments[ornament1].CopyAll;
            VTM.Ornaments[ornament1].CopyAll = VTM.Ornaments[ornament2].CopyAll;
            VTM.Ornaments[ornament2].CopyAll = tb;

            for (int i = 0; i < 255; i++)
            {
                t = VTM.Ornaments[ornament1].Offsets[i];
                VTM.Ornaments[ornament1].Offsets[i] = VTM.Ornaments[ornament2].Offsets[i];
                VTM.Ornaments[ornament2].Offsets[i] = (sbyte)t;
            }

            for (int i = 0; i < VTM.Patterns.Length; i++)
            {
                if (VTM.Patterns[i] == null)
                    continue;

                for (int j = 0; j < VTM.Patterns[i].Length; j++)
                {
                    for (int k = 2; k >= 0; k--)
                    {
                        ref var chan = ref VTM.Patterns[i].Lines[j].Channel[k];
                        if (chan.Ornament == ornament1)
                            chan.Ornament = (byte)ornament2;
                        else if (chan.Ornament == ornament2)
                            chan.Ornament = (byte)ornament1;
                    }
                }
            }

            RecalcSampOrnUsage();
        }

        public void SwapSamples1_Click(object sender, EventArgs e)
        {
            int sample1 = Samples.CopiedSample;
            int sample2 = SampleIndex;

            AddUndo(TChangeAction.SwapSamples, sample1, sample2);
            SwapSamples(sample1, sample2);
            Samples.HideCaret();
            Samples.Redraw();
            SamplesGrid.Invalidate();
            Samples.ShowCaret();
            Samples.CopiedSample = -1;

            if (Globals.ToggleSamplesForm.Visible)
                Globals.ToggleSamplesForm.CheckUsedSamples();
        }

        public void SwapOrnaments1_Click(object sender, EventArgs e)
        {
            int ornament1 = Ornaments.CopiedOrnament;
            int ornament2 = OrnamentIndex;
            AddUndo(TChangeAction.SwapOrnaments, ornament1, ornament2);
            SwapOrnaments(ornament1, ornament2);
            Ornaments.HideCaret();
            Ornaments.Redraw();
            OrnamentsGrid.Invalidate();
            Ornaments.ShowCaret();
            Ornaments.CopiedOrnament = -1;
        }

        public void PackSamples1_Click(object sender, EventArgs e)
        {
            int k = 0;

            while (true)
            {
                int kz = 0;

                for (int i = 1; i < 32; i++)
                {
                    if (VTModule.IsSampleEmpty(VTM, i))
                    {
                        kz = i;
                        break;
                    }
                }

                if (kz == 0)
                    break;

                int knz = 0;
                k = kz + 1;

                for (int i = k; i < 32; i++)
                {
                    if (!VTModule.IsSampleEmpty(VTM, i))
                    {
                        knz = i;
                        break;
                    }
                }

                if (knz == 0)
                    break;

                AddUndo(TChangeAction.SwapSamples, kz, knz);
                SwapSamples(kz, knz);
            }

            Samples.HideCaret();
            Samples.Redraw();
            SamplesGrid.Invalidate();
            Samples.ShowCaret();
            Samples.CopiedSample = -1;

            if (Globals.ToggleSamplesForm.Visible)
                Globals.ToggleSamplesForm.CheckUsedSamples();
        }

        public void PackOrnaments1_Click(object sender, EventArgs e)
        {
            int k = 0;

            while (true)
            {
                int kz = 0;

                for (int i = 1; i <= 31; i++)
                {
                    if (VTModule.IsOrnamentEmpty(VTM, i))
                    {
                        kz = i;
                        break;
                    }
                }

                if (kz == 0)
                    break;

                int knz = 0;
                k = kz + 1;

                for (int i = k; i <= 31; i++)
                {
                    if (!VTModule.IsOrnamentEmpty(VTM, i))
                    {
                        knz = i;
                        break;
                    }
                }

                if (knz == 0)
                    break;

                AddUndo(TChangeAction.SwapOrnaments, kz, knz);
                SwapOrnaments(kz, knz);
            }

            Ornaments.HideCaret();
            Ornaments.Redraw();
            OrnamentsGrid.Invalidate();
            Ornaments.ShowCaret();
            Ornaments.CopiedOrnament = -1;
        }

        private void AutoLL_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var pf = AutoLL.PointToScreen(new Point(e.X, e.Y));
                LowLightMenu1.Show(pf);
            }
            else if (e.Button == MouseButtons.Right)
            {
                AutoLL_Click(Disabled1, EventArgs.Empty);
            }
        }

        public void AutoLL_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            string s = menuItem.Text;
            s = s.Substring(0, s.IndexOf(':'));
            LLTemplate = int.Parse(s);
            AutoLL.Text = LLTemplate.ToString();

            Tracks.HideCaret();
            Tracks.ShowSelection();
            Tracks.RedrawTracks();
            Tracks.RecreateCaret();
            Tracks.SetCaretPosition();
            Tracks.ShowCaret();
        }

        public void HideSampleBrowserButton_Click(object sender, EventArgs e)
        {
            // SamplesBrowser.Visible := False;
            // ShowSamBrowserBtn.Visible := True;
            // HideSamBrowserBtn.Visible := False;
            Globals.MainForm.SampleBrowserVisible = false;
            Globals.MainForm.RedrawAllSamOrnBrowsers();
        }

        public void ShowSampleBrowserButton_Click(object sender, EventArgs e)
        {
            // SamplesBrowser.Visible := True;
            // ShowSamBrowserBtn.Visible := False;
            // HideSamBrowserBtn.Visible := True;
            Globals.MainForm.SampleBrowserVisible = true;
            Globals.MainForm.RedrawAllSamOrnBrowsers();
        }

        public void ActivateTab(int tabIndex)
        {
            if (TabControl.SelectedIndex != tabIndex && tabIndex >= 0 && tabIndex < TabControl.TabPages.Count)
            {
                if (TabControl.SelectedIndex == 0 && (tabIndex == 1 || tabIndex == 2))
                    RecalcSampOrnUsage();

                TabControl.SelectedIndex = tabIndex;

                // Optional: set focus to the new tab page
                // tabControl1.SelectedTab.Focus(); // uncomment if needed
            }
        }

        public void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If CTRL pressed, then change Tab in second turbotrack module
            if (GetKeyState(Keys.Control) < 0 && TSWindow != null)
            {
                TSWindow[0].ActivateTab(TabControl.SelectedIndex);

                if (TabControl.SelectedTab == OptionsTab)
                {
                    TSWindow[0].ManualHz.Left = ChipFreqBox.Buttons[20].Left + 95;
                    TSWindow[0].ManualIntFreq.Left = IntFreqBox.Buttons[6].Left + 95;
                }

                // Change tab in third Turbosound module
                if (TSWindow[1] != null)
                {
                    TSWindow[1].ActivateTab(TabControl.SelectedIndex);

                    if (TabControl.SelectedTab == OptionsTab)
                    {
                        TSWindow[1].ManualHz.Left = ChipFreqBox.Buttons[20].Left + 95; // !!!
                        TSWindow[1].ManualIntFreq.Left = IntFreqBox.Buttons[6].Left + 95;
                    }
                }
            }

            if (WaveOutAPI.IsPlaying && !(AY.PlayMode == PlayModes.PlayPattern || AY.PlayMode == PlayModes.PlayModule))
                StopAndRestoreControls();

            if (TabControl.SelectedTab == PatternsTab)
            {
                RefreshPositionsHScroll();
                PositionsGrid.Focus();
            }

            if (TabControl.SelectedTab == SamplesTab)
            {
                SamplesDriveSelect.FillDiskDrives();
                SampleTestLine.CursorX = 12;
                SampleTestLine.Focus();
                RecalcSampOrnUsage();
            }

            if (TabControl.SelectedTab == OrnamentsTab)
            {
                OrnamentsDriveSelect.FillDiskDrives();
                OrnamentTestLine.CursorX = 14;
                OrnamentTestLine.Focus();
                RecalcSampOrnUsage();
            }

            if (TabControl.SelectedTab == OptionsTab)
            {
                ManualHz.Left = ChipFreqBox.Buttons[20].Left + 95;
                ManualIntFreq.Left = IntFreqBox.Buttons[6].Left + 95;
            }

            if (TabControl.SelectedTab == InfoTab && TrackInfoRTB.CanFocus)
                TrackInfoRTB.Focus();
        }

        public void HideOrnamentBrowserButton_Click(object sender, EventArgs e)
        {
            Globals.MainForm.OrnamentsBrowserVisible = false;
            Globals.MainForm.RedrawAllSamOrnBrowsers();
        }

        public void ShowOrnamentBrowserButton_Click(object sender, EventArgs e)
        {
            Globals.MainForm.OrnamentsBrowserVisible = true;
            Globals.MainForm.RedrawAllSamOrnBrowsers();
        }

        public void PlayStopTimer_Tick(object sender, EventArgs e)
        {
            StopAndRestoreControls();
            PlayStopTimer.Enabled = false;

            if (TabControl.SelectedTab == SamplesTab)
                SamplesBrowser.PreviewPlaying = false;

            if (TabControl.SelectedTab == OrnamentsTab)
                OrnamentsBrowser.PreviewPlaying = false;
        }

        public void UpdateSamToneShiftControls()
        {
            bool value;
            value = SampleToneShiftAsNoteCheckBox.Checked;
            Samples.ToneShiftAsNote = value;
            SampleOctaveLabel.Visible = value;
            SampleSeparator1.Visible = value;
            SampleOctaveLabel.Visible = value;
            SampleOctaveValue.Visible = value;
            SampleOctaveNum.Visible = value;
            SampleOctaveValue.Text = SampleOctaveNum.Value.ToString();
        }

        public void SampleToneShiftAsNoteCheckBox_Click(object sender, EventArgs e)
        {
            MainForm.SamToneShiftAsNote = SampleToneShiftAsNoteCheckBox.Checked;

            UpdateSamToneShiftControls();

            Samples.HideCaret();
            Samples.Redraw();
            Samples.ShowCaret();

            if (InitFinished && Samples.CanFocus)
                Samples.Focus();
        }

        public void SampleOctaveNum_ValueChanging(object sender, ValueChangingEventArgs e)
        {
            e.Cancel = e.NewValue < 1 || e.NewValue > 8;

            if (e.Cancel)
                return;

            SampleOctaveValue.Text = e.NewValue.ToString();
        }

        public void UpdateOrnToneShiftControls()
        {
            bool value = OrnamentToneShiftAsNoteCheckBox.Checked;
            Ornaments.ToneShiftAsNote = value;
            OrnamentOctaveLabel.Visible = value;
            OrnamentSeperator.Visible = value;
            OrnamentOctaveLabel.Visible = value;
            OrnamentOctaveValue.Visible = value;
            OrnamentOctaveNum.Visible = value;
            OrnamentOctaveValue.Text = OrnamentOctaveNum.Value.ToString();
        }

        public void OrnamentToneShiftAsNoteCheckBox_Click(object sender, EventArgs e)
        {
            MainForm.OrnToneShiftAsNote = OrnamentToneShiftAsNoteCheckBox.Checked;
            UpdateOrnToneShiftControls();
            Ornaments.HideCaret();
            Ornaments.Redraw();
            Ornaments.ShowCaret();

            if (InitFinished && Ornaments.CanFocus)
                Ornaments.Focus();
        }

        public void OrnamentOctaveNum_ValueChanging(object sender, ValueChangingEventArgs e)
        {
            e.Cancel = e.NewValue < 1 || e.NewValue > 8;

            if (e.Cancel)
                return;

            OrnamentOctaveValue.Text = e.NewValue.ToString();
        }

        public int GetValue(string s)
        {
            int result;

            if (!int.TryParse(s, out result))
                result = -1;

            return result;
        }

        public double GetValueF(string s)
        {
            double result;
            s = s.Replace(',', '.');

            if (!double.TryParse(s, out result))
                result = -1;

            return result;
        }

        public void UpdateChipFreq()
        {
            if (BlockRecursion || !InitFinished || IsClosed)
                return;

            if (TSWindow[0] != null)
            {
                TSWindow[0].BlockRecursion = true;
                TSWindow[0].SetTrackFreq(VTM.ChipFreq);
                TSWindow[0].BlockRecursion = false;
            }

            if (TSWindow[1] != null)
            {
                TSWindow[1].BlockRecursion = true;
                TSWindow[1].SetTrackFreq(VTM.ChipFreq);
                TSWindow[1].BlockRecursion = false;
            }

            if (!ModuleInPlayingWindow())
                return;

            SongChanged = true;
            BackupSongChanged = true;

            if (VTM.ChipFreq != AY.AYFreq)
                AY.SetAYFreq(VTM.ChipFreq);
        }

        public void UpdateToneTableHints()
        {
            string toolTip = $"Table #{VTM.NoteTable}: {VTModule.NoteTableNames[(int)VTM.NoteTable]}";
            //toolTip1.SetToolTip(UpDown4, toolTip);
            //UpDown4.Hint = Edit7.Hint;
            ToolTip.SetToolTip(ToneTableUpDown, toolTip);
            //ToneTableBox.Hint = Edit7.Hint;
            ToolTip.SetToolTip(ToneTableBox, toolTip);
        }

        public void InitTrack()
        {
            SetTrackFreq(VTM.ChipFreq);
            SetTrackIntFreq(VTM.IntFreq);
            SetRTFText(TrackInfoRTB, VTM.Info);
            UpdateToneTableHints();

            if (NumModule == 1 && !MainForm.DisableInfoWin)
                TrackInfoTimer.Enabled = true;

            ShowInfoOnLoadCheckBox.Checked = VTM.ShowInfo;
        }

        public void SetTrackFreq(int freqValue)
        {
            VTM.ChipFreq = freqValue;

            switch (freqValue)
            {
                case 894887:
                    ChipFreqBox.SelectedIndex = 0;
                    break;
                case 831303:
                    ChipFreqBox.SelectedIndex = 1;
                    break;
                case 1773400:
                    ChipFreqBox.SelectedIndex = 2;
                    break;
                case 1750000:
                    ChipFreqBox.SelectedIndex = 3;
                    break;
                case 1000000:
                    ChipFreqBox.SelectedIndex = 4;
                    break;
                case 1500000:
                    ChipFreqBox.SelectedIndex = 5;
                    break;
                case 2000000:
                    ChipFreqBox.SelectedIndex = 6;
                    break;
                case 3500000:
                    ChipFreqBox.SelectedIndex = 7;
                    break;
                case 1520640:
                    ChipFreqBox.SelectedIndex = 8;
                    break;
                case 1611062:
                    ChipFreqBox.SelectedIndex = 9;
                    break;
                case 1706861:
                    ChipFreqBox.SelectedIndex = 10;
                    break;
                case 1808356:
                    ChipFreqBox.SelectedIndex = 11;
                    break;
                case 1915886:
                    ChipFreqBox.SelectedIndex = 12;
                    break;
                case 2029811:
                    ChipFreqBox.SelectedIndex = 13;
                    break;
                case 2150510:
                    ChipFreqBox.SelectedIndex = 14;
                    break;
                case 2278386:
                    ChipFreqBox.SelectedIndex = 15;
                    break;
                case 2413866:
                    ChipFreqBox.SelectedIndex = 16;
                    break;
                case 2557401:
                    ChipFreqBox.SelectedIndex = 17;
                    break;
                case 2709472:
                    ChipFreqBox.SelectedIndex = 18;
                    break;
                case 2870586:
                    ChipFreqBox.SelectedIndex = 19;
                    break;
                case 3041280:
                    ChipFreqBox.SelectedIndex = 20;
                    break;
                default:
                    ChipFreqBox.SelectedIndex = 21;
                    ManualHz.Text = freqValue.ToString();
                    break;
            }
        }

        public void ChipFreqBox_Click(object sender, EventArgs e)
        {
            int frequency;

            switch (ChipFreqBox.SelectedIndex)
            {
                case 0:
                    VTM.ChipFreq = 894887;
                    break;
                case 1:
                    VTM.ChipFreq = 831303;
                    break;
                case 2:
                    VTM.ChipFreq = 1773400;
                    break;
                case 3:
                    VTM.ChipFreq = 1750000;
                    break;
                case 4:
                    VTM.ChipFreq = 1000000;
                    break;
                case 5:
                    VTM.ChipFreq = 1500000;
                    break;
                case 6:
                    VTM.ChipFreq = 2000000;
                    break;
                case 7:
                    VTM.ChipFreq = 3500000;
                    break;
                case 8:
                    VTM.ChipFreq = 1520640;
                    break;
                case 9:
                    VTM.ChipFreq = 1611062;
                    break;
                case 10:
                    VTM.ChipFreq = 1706861;
                    break;
                case 11:
                    VTM.ChipFreq = 1808356;
                    break;
                case 12:
                    VTM.ChipFreq = 1915886;
                    break;
                case 13:
                    VTM.ChipFreq = 2029811;
                    break;
                case 14:
                    VTM.ChipFreq = 2150510;
                    break;
                case 15:
                    VTM.ChipFreq = 2278386;
                    break;
                case 16:
                    VTM.ChipFreq = 2413866;
                    break;
                case 17:
                    VTM.ChipFreq = 2557401;
                    break;
                case 18:
                    VTM.ChipFreq = 2709472;
                    break;
                case 19:
                    VTM.ChipFreq = 2870586;
                    break;
                case 20:
                    VTM.ChipFreq = 3041280;
                    break;
                case 21:
                    frequency = GetValue(ManualHz.Text);
                    if (frequency < 0 || frequency < 700000 || frequency > 3546800)
                        return;
                    VTM.ChipFreq = frequency;
                    break;
            }
            UpdateChipFreq();
        }

        public void UpdateIntFreq()
        {
            if (BlockRecursion || !InitFinished || IsClosed)
                return;

            if (TSWindow[0] != null)
            {
                TSWindow[0].BlockRecursion = true;
                TSWindow[0].SetTrackIntFreq(VTM.IntFreq);
                TSWindow[0].CalcTotLen();
                TSWindow[0].ReCalcTimes(TSWindow[0].PosBegin + TSWindow[0].LineInts);
                TSWindow[0].UpdateSpeedBPM();
                TSWindow[0].BlockRecursion = false;
            }

            if (TSWindow[1] != null)
            {
                TSWindow[1].BlockRecursion = true;
                TSWindow[1].SetTrackIntFreq(VTM.IntFreq);
                TSWindow[1].CalcTotLen();
                TSWindow[1].ReCalcTimes(TSWindow[1].PosBegin + TSWindow[1].LineInts);
                TSWindow[1].UpdateSpeedBPM();
                TSWindow[1].BlockRecursion = false;
            }

            if (!ModuleInPlayingWindow())
                return;

            if (VTM.IntFreq != WaveOutAPI.InterruptFreq)
                AY.SetIntFreq(VTM.IntFreq);

            SongChanged = true;
            BackupSongChanged = true;

            CalcTotLen();
            ReCalcTimes(PosBegin + LineInts);
            UpdateSpeedBPM();
        }

        public void SetTrackIntFreq(int IntFreqValue)
        {
            double frequency;

            VTM.IntFreq = IntFreqValue;

            switch (IntFreqValue)
            {
                case 48828:
                    IntFreqBox.SelectedIndex = 0;
                    break;
                case 50000:
                    IntFreqBox.SelectedIndex = 1;
                    break;
                case 60000:
                    IntFreqBox.SelectedIndex = 2;
                    break;
                case 100000:
                    IntFreqBox.SelectedIndex = 3;
                    break;
                case 200000:
                    IntFreqBox.SelectedIndex = 4;
                    break;
                case 48000:
                    IntFreqBox.SelectedIndex = 5;
                    break;
                default:
                    IntFreqBox.SelectedIndex = 6;
                    if (!ManualIntFreq.Focused)
                    {
                        frequency = IntFreqValue / 1000;
                        ManualIntFreq.Text = Convert.ToString(frequency);
                    }
                    break;
            }
        }

        public void IntFreqBox_Click(object sender, EventArgs e)
        {
            double frequency;

            switch (IntFreqBox.SelectedIndex)
            {
                case 0:
                    VTM.IntFreq = 48828;
                    break;
                case 1:
                    VTM.IntFreq = 50000;
                    break;
                case 2:
                    VTM.IntFreq = 60000;
                    break;
                case 3:
                    VTM.IntFreq = 100000;
                    break;
                case 4:
                    VTM.IntFreq = 200000;
                    break;
                case 5:
                    VTM.IntFreq = 48000;
                    break;
                case 6:
                    frequency = GetValueF(ManualIntFreq.Text);
                    if (frequency < 0)
                        return;
                    VTM.IntFreq = (int)Math.Round(frequency * 1000);
                    break;
                default:
                    return;
            }
            UpdateIntFreq();
        }

        public void ManualHzKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar >= '0' && e.KeyChar <= '9') && (e.KeyChar != '\b'))
            {
                e.KeyChar = '\0';
                return;
            }
        }

        public void ManualHzKeyUp(object sender, KeyEventArgs e)
        {
            if (ChipFreqBox.SelectedIndex != 21)
                ChipFreqBox.SelectedIndex = 21;

            int newValue = GetValue(ManualHz.Text);

            if (newValue < 0 || newValue < 700000 || newValue > 3546800)
                return;

            SetTrackFreq(newValue);
            UpdateChipFreq();

            SongChanged = true;
            BackupSongChanged = true;
        }

        public void ManualIntFreqKeyPress(object sender, KeyPressEventArgs e)
        {
            bool wrong = !(e.KeyChar == '0' || e.KeyChar == '.' || e.KeyChar == ',') && e.KeyChar != '\b';
            wrong = wrong || ((ManualIntFreq.Text.IndexOf(',') != -1) && (e.KeyChar == '.' || e.KeyChar == ','));
            wrong = wrong || ((ManualIntFreq.Text == "") && (e.KeyChar == '.' || e.KeyChar == ','));

            if (wrong)
            {
                e.KeyChar = '\0';
                return;
            }

            if (e.KeyChar == '.')
            {
                e.KeyChar = ',';
                return;
            }
        }

        public void ManualIntFreqKeyUp(object sender, KeyEventArgs e)
        {
            double f;
            bool wrong;
            double newValue = GetValueF(ManualIntFreq.Text);

            if (newValue < 0)
                return;

            f = Helpers.Frac(Convert.ToSingle(ManualIntFreq.Text));
            wrong = Convert.ToString(f).Length > 5;
            newValue = newValue * 1000;
            wrong = wrong || (newValue < 1000 || newValue > 2000000);

            if (wrong)
            {
                f = VTM.IntFreq / 1000;
                ManualIntFreq.Text = Convert.ToString(f);
                ManualIntFreq.SelectionStart = ManualIntFreq.Text.Length;
                return;
            }

            SetTrackIntFreq((int)Math.Round(newValue));
            UpdateIntFreq();
            SongChanged = true;
            BackupSongChanged = true;
        }

        public void UpdateTrackInfo()
        {
            VTM.Info = GetRTFText(TrackInfoRTB);

            if (TSWindow[0] != null)
            {
                TSWindow[0].VTM.Info = VTM.Info;
                SetRTFText(TSWindow[0].TrackInfoRTB, VTM.Info);
            }

            if (TSWindow[1] != null)
            {
                TSWindow[1].VTM.Info = VTM.Info;
                SetRTFText(TSWindow[1].TrackInfoRTB, VTM.Info);
            }

            SongChanged = true;
            BackupSongChanged = true;
        }

        private void BoldButton_Click(object sender, EventArgs e)
        {
            ToggleFontStyle(FontStyle.Bold);
            UpdateTrackInfo();
        }

        private void ItalicButton_Click(object sender, EventArgs e)
        {
            ToggleFontStyle(FontStyle.Italic);
            UpdateTrackInfo();
        }

        private void UnderlineButton_Click(object sender, EventArgs e)
        {
            ToggleFontStyle(FontStyle.Underline);
            UpdateTrackInfo();
        }

        private void ToggleFontStyle(FontStyle style)
        {
            if (TrackInfoRTB.SelectionFont == null)
                return;

            Font currentFont = TrackInfoRTB.SelectionFont;
            FontStyle newStyle = TrackInfoRTB.SelectionFont.Style;

            if ((currentFont.Style & style) == style)
                newStyle &= ~style;
            else
                newStyle |= style;

            TrackInfoRTB.SelectionFont = new Font(currentFont, newStyle);
        }

        public string GetRTFText(RichTextBox richTextBox)
        {
            return richTextBox.Rtf;
        }

        public void SetRTFText(RichTextBox richTextBox, string rtfText)
        {
            if (String.IsNullOrEmpty(rtfText))
                return;

            byte[] rtfBytes = Encoding.UTF8.GetBytes(rtfText);

            using (MemoryStream rtfStream = new MemoryStream(rtfBytes))
                richTextBox.LoadFile(rtfStream, RichTextBoxStreamType.RichText);
        }

        public void TrackInfoRTB_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateTrackInfo();
        }

        public void TrackInfoRTB_KeyDown(object sender, KeyEventArgs e)
        {
            bool isShiftDown = (e.Modifiers & Keys.Shift) == Keys.Shift;
            bool isCtrlDown = (e.Modifiers & Keys.Control) == Keys.Control;
            bool isAltDown = (e.Modifiers & Keys.Alt) == Keys.Alt;

            if (isShiftDown)
                return;

            switch (e.KeyCode)
            {
                case Keys.B:
                    BoldButton_Click(sender, e);
                    break;
                case Keys.I:
                    ItalicButton_Click(sender, e);
                    break;
                case Keys.U:
                    UnderlineButton_Click(sender, e);
                    break;
            }
        }

        public void ShowInfoOnLoadCheckBox_MouseUp(object sender, MouseEventArgs e)
        {
            VTM.ShowInfo = ShowInfoOnLoadCheckBox.Checked;

            if (TSWindow[0] != null)
            {
                TSWindow[0].ShowInfoOnLoadCheckBox.Checked = VTM.ShowInfo;
                TSWindow[0].VTM.ShowInfo = VTM.ShowInfo;
            }

            if (TSWindow[1] != null)
            {
                TSWindow[1].ShowInfoOnLoadCheckBox.Checked = VTM.ShowInfo;
                TSWindow[1].VTM.ShowInfo = VTM.ShowInfo;
            }

            SongChanged = true;
            BackupSongChanged = true;
        }

        public void TrackInfoTimer_Tick(object sender, EventArgs e)
        {
            TrackInfoTimer.Enabled = false;

            if (!VTM.ShowInfo)
                return;

            Globals.TrackInfoForm.Init(VTM);
            Globals.TrackInfoForm.ShowDialog(this);

            Globals.MainForm.Activate();
        }

        public void ViewInfoButton_Click(object sender, EventArgs e)
        {
            Globals.TrackInfoForm.Init(VTM);
            Globals.TrackInfoForm.ShowDialog(this);

            Globals.MainForm.Activate();
        }

        private void PositionsGrid_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                PositionsGrid_MouseWheelUp(sender, e);
            else if (e.Delta < 0)
                PositionsGrid_MouseWheelDown(sender, e);
        }

        public void PositionsGrid_MouseWheelDown(object sender, MouseEventArgs e)
        {
            int newPos;
            Rectangle selectionRect;

            if (IsMouseOverControl(Tracks))
            {
                // Scroll pattern, not positions
                if (Tracks.CanFocus)
                {
                    Tracks.Focus();
                    Tracks_MouseWheelDown(sender, e);
                }

                return;
            }

            // Mouse pointer under another control
            if (!IsMouseOverControl(PositionsGrid))
                return;

            // If playing current pattern only
            if (WaveOutAPI.IsPlaying && AY.PlayMode == PlayModes.PlayPattern)
                return;

            newPos = PositionIndex + 1;

            if (newPos == VTM.Positions.Length)
                return;

            SetPositionsGridScroll(newPos);
            SelectPosition(newPos);

            selectionRect = Rectangle.FromLTRB(newPos, 0, newPos, 0);
            PositionsGrid.Selection = selectionRect;
        }

        public void PositionsGrid_MouseWheelUp(object sender, MouseEventArgs e)
        {
            int newPos;
            Rectangle selectionRect;

            if (IsMouseOverControl(Tracks))
            {
                // Scroll pattern, not positions
                if (Tracks.CanFocus)
                {
                    Tracks.Focus();
                    Tracks_MouseWheelUp(sender, e);
                }

                return;
            }

            // Mouse pointer under another control
            if (!IsMouseOverControl(PositionsGrid))
                return;

            // If playing current pattern only
            if (WaveOutAPI.IsPlaying && AY.PlayMode == PlayModes.PlayPattern)
                return;

            newPos = PositionIndex - 1;

            if (newPos < 0)
                return;

            SetPositionsGridScroll(newPos);
            SelectPosition(newPos);

            selectionRect = Rectangle.FromLTRB(newPos, 0, newPos, 0);
            PositionsGrid.Selection = selectionRect;
        }

        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are You Sure You Want to Disconnect the Track?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            StopAndRestoreControls();

            var tsWin1 = (ChildForm)Globals.MainForm.ActiveMdiChild;
            var tsWin2 = tsWin1.TSWindow[0];
            var tsWin3 = tsWin1.TSWindow[1];

            tsWin1.TSWindow[0] = null;
            tsWin1.TSWindow[1] = null;
            tsWin1.SongChanged = true;
            tsWin1.Text = tsWin1.WinFileName;

            if (tsWin2 != null)
            {
                if (tsWin2.TSWindow[0] == Globals.MainForm.ActiveMdiChild)
                    tsWin2.TSWindow[0] = null;

                if (tsWin2.TSWindow[1] == Globals.MainForm.ActiveMdiChild)
                    tsWin2.TSWindow[1] = null;

                if (tsWin2.TSWindow[0] == null && tsWin2.TSWindow[1] != null)
                {
                    tsWin2.TSWindow[0] = tsWin2.TSWindow[1];
                    tsWin2.TSWindow[1] = null;
                }

                tsWin2.SongChanged = true;
            }

            if (tsWin3 != null)
            {
                if (tsWin3.TSWindow[0] == Globals.MainForm.ActiveMdiChild)
                    tsWin3.TSWindow[0] = null;

                if (tsWin3.TSWindow[1] == Globals.MainForm.ActiveMdiChild)
                    tsWin3.TSWindow[1] = null;

                if (tsWin3.TSWindow[0] == null && tsWin3.TSWindow[1] != null)
                {
                    tsWin3.TSWindow[0] = tsWin3.TSWindow[1];
                    tsWin3.TSWindow[1] = null;
                }

                tsWin3.SongChanged = true;
            }

            Globals.MainForm.MultitrackReorder();
            Globals.MainForm.JoinTracks_Update(this, EventArgs.Empty);

            SynchronizeModules();
        }

        public void RedrawSamplesCell(int columnIndex, bool active)
        {
            using var bmp = new Bitmap(SamplesGrid.Columns[columnIndex].Width, SamplesGrid.Rows[0].Height);
            using var g = Graphics.FromImage(bmp);
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var args = new DataGridViewCellPaintingEventArgs(
                SamplesGrid, g, rect, rect,
                0, columnIndex,
                active ? DataGridViewElementStates.Selected : DataGridViewElementStates.None,
                SamplesGrid[columnIndex, 0].Value,
                SamplesGrid[columnIndex, 0].FormattedValue,
                "",
                new DataGridViewCellStyle(), null, DataGridViewPaintParts.All);
            SamplesGrid_CellPainting(SamplesGrid, args);
            SamplesGrid.InvalidateCell(columnIndex, 0);
        }

        public static readonly Color[,] Cols = new Color[2, 3]
        {
            {
                Color.FromArgb(unchecked((int)0xFFA0A0A0)),
                Color.FromArgb(unchecked((int)0xFF808080)),
                Color.FromArgb(unchecked((int)0xFFE0E0E0))
            },
            {
                Color.FromArgb(unchecked((int)0xFF808080)),
                Color.FromArgb(unchecked((int)0xFFA0A0A0)),
                Color.FromArgb(unchecked((int)0xFF303030))
            }
        };

        public static readonly Color[] SamplesGridCol = new Color[]
        {
            Color.FromArgb(unchecked((int)0xFF303030)),
            Color.FromArgb(unchecked((int)0xFFD0D0D0))
        };

        // Event handler for SamplesGrid cell painting
        private void SamplesGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            var grid = sender as MyDataGridView;
            bool isSelected = e.State.HasFlag(DataGridViewElementStates.Selected);
            var g = e.Graphics;
            var cellRect = e.CellBounds;
            var textRect = new Rectangle(cellRect.Left, cellRect.Top + 33, cellRect.Width, 15);
            var cellValue = grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";
            Color selectColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelBackground];
            Color backColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnBackground];
            int pal = (backColor.R + backColor.G + backColor.B) > 0x180 ? 1 : 0;
            int pen = 0;

            e.Handled = true;

            using (SolidBrush brush = new SolidBrush(backColor))
                g.FillRectangle(brush, cellRect);

            using (SolidBrush brush = new SolidBrush(isSelected? selectColor : SystemColors.Control))
                g.FillRectangle(brush, textRect);

            var samp = VTM.Samples[e.ColumnIndex + 1];

            if (samp != null)
            {
                int masterVol = 0;
                int masterTon = 0;
                int maxY = cellRect.Height - 31;
                int nx = (cellRect.Width - 21) / 2;
                int x1 = cellRect.Left;
                int y1 = cellRect.Top;
                int mx = nx + x1 + 10;

                for (int y = 0, yy = 0; y < maxY; y++, yy++)
                {
                    if (yy >= samp.Length)
                    {
                        yy = samp.Loop;
                        pen = 1;
                    }

                    var tick = samp.Ticks[yy];

                    if (tick.Amplitude_Sliding)
                    {
                        if (tick.Amplitude_Slide_Up)
                        {
                            if (masterVol < 15)
                                masterVol += 1;
                        }
                        else
                        {
                            if (masterVol > -15)
                                masterVol -= 1;
                        }
                    }

                    int amp = Math.Clamp(tick.Amplitude + masterVol, 0, 15);

                    using (Pen penColor = new Pen(Cols[pal, pen]))
                        g.DrawLine(penColor, nx + x1 + 4, y1 + y, nx + x1 + 4 + amp, y1 + y);

                    int mt = masterTon + tick.AddToTone;

                    if (tick.Ton_Accumulation)
                        masterTon += tick.AddToTone;
                    
                    mt &= 0xFFF;
                    if (mt > 0x400) mt -= 0x1000;
                    if (mt > 3) mt = ((mt - 3) / 48) + 3;
                    if (mt < -3) mt = ((mt + 3) / 48) - 3;

                    mt = Math.Clamp(mt, -mx, mx);

                    if (tick.Mixer_Ton)
                    {
                        using (SolidBrush brush = new SolidBrush(Cols[pal, 2]))
                            g.FillRectangle(brush, nx + x1 + 11 - mt, y1 + y, 1, 1);
                    }

                    if (tick.Mixer_Noise)
                    {
                        using (SolidBrush brush = new SolidBrush(Color.FromArgb(unchecked((int)0xFF40A0FF))))
                            g.FillRectangle(brush, nx + x1 + 2, y1 + y, 1, 1);
                    }

                    if (tick.Envelope_Enabled)
                    {
                        using (SolidBrush brush = new SolidBrush(Color.FromArgb(unchecked((int)0xFF60FF60))))
                            g.FillRectangle(brush, nx + x1 + 3, y1 + y, 1, 1);
                    }
                }
            }

            if (SampUsage[e.ColumnIndex + 1])
                g.FillRectangle(Brushes.Red, cellRect.Right - 6, textRect.Top + 3, 3, 3);

            TextRenderer.DrawText(g, cellValue, grid.Font, textRect, isSelected ? Color.White : Color.Black,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

            using (Pen gridPen = new Pen(Color.FromArgb(unchecked((int)0xFFC0C0C0))))
            {
                g.DrawLine(gridPen, cellRect.Left, cellRect.Top + 33, cellRect.Right, cellRect.Top + 33);
                g.DrawLine(gridPen, cellRect.Right - 1, cellRect.Top, cellRect.Right - 1, cellRect.Bottom);
                g.DrawLine(gridPen, cellRect.Left, cellRect.Bottom - 1, cellRect.Right, cellRect.Bottom - 1);
            }
        }

        public void RedrawOrnamentsCell(int columnIndex, bool active)
        {
            using var bmp = new Bitmap(OrnamentsGrid.Columns[columnIndex].Width, OrnamentsGrid.Rows[0].Height);
            using var g = Graphics.FromImage(bmp);
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var args = new DataGridViewCellPaintingEventArgs(
                OrnamentsGrid, g, rect, rect,
                0, columnIndex,
                active ? DataGridViewElementStates.Selected : DataGridViewElementStates.None,
                OrnamentsGrid[columnIndex, 0].Value,
                OrnamentsGrid[columnIndex, 0].FormattedValue,
                "",
                new DataGridViewCellStyle(), null, DataGridViewPaintParts.All);
            OrnamentsGrid_CellPainting(OrnamentsGrid, args);
            OrnamentsGrid.InvalidateCell(columnIndex, 0);
        }

        public static readonly Color[,] NoteCol = new Color[2, 12]
        {
            {
                Color.FromArgb(unchecked((int)0xFFF8F8F8)),
                Color.FromArgb(unchecked((int)0xFFFF0000)),
                Color.FromArgb(unchecked((int)0xFFFFC000)),
                Color.FromArgb(unchecked((int)0xFF0050FF)),
                Color.FromArgb(unchecked((int)0xFF08B000)),
                Color.FromArgb(unchecked((int)0xFFFF8000)),
                Color.FromArgb(unchecked((int)0xFF909090)),
                Color.FromArgb(unchecked((int)0xFF8000FF)),
                Color.FromArgb(unchecked((int)0xFF00E080)),
                Color.FromArgb(unchecked((int)0xFF00B0FF)),
                Color.FromArgb(unchecked((int)0xFFA0E000)),
                Color.FromArgb(unchecked((int)0xFFFF40A0))
            },
            {
                Color.FromArgb(unchecked((int)0xFF000000)),
                Color.FromArgb(unchecked((int)0xFFFF0000)),
                Color.FromArgb(unchecked((int)0xFFFFC000)),
                Color.FromArgb(unchecked((int)0xFF0060FF)),
                Color.FromArgb(unchecked((int)0xFF10C000)),
                Color.FromArgb(unchecked((int)0xFFFF8000)),
                Color.FromArgb(unchecked((int)0xFF909090)),
                Color.FromArgb(unchecked((int)0xFF8000FF)),
                Color.FromArgb(unchecked((int)0xFF00F090)),
                Color.FromArgb(unchecked((int)0xFF00B0FF)),
                Color.FromArgb(unchecked((int)0xFFB0E000)),
                Color.FromArgb(unchecked((int)0xFFFF00D0))
            }
        };

        public static readonly Color[,] OctCol = new Color[2, 9]
        {
            {
                Color.FromArgb(unchecked((int)0xFFA0A0A0)),
                Color.FromArgb(unchecked((int)0xFF507090)),
                Color.FromArgb(unchecked((int)0xFF406080)),
                Color.FromArgb(unchecked((int)0xFF304050)),
                Color.FromArgb(unchecked((int)0xFF000000)),
                Color.FromArgb(unchecked((int)0xFF504030)),
                Color.FromArgb(unchecked((int)0xFF806040)),
                Color.FromArgb(unchecked((int)0xFF907050)),
                Color.FromArgb(unchecked((int)0xFFA0A0A0))
            },
            {
                Color.FromArgb(unchecked((int)0xFF404040)),
                Color.FromArgb(unchecked((int)0xFFA09060)),
                Color.FromArgb(unchecked((int)0xFFC0A080)),
                Color.FromArgb(unchecked((int)0xFFD0C0B0)),
                Color.FromArgb(unchecked((int)0xFF000000)),
                Color.FromArgb(unchecked((int)0xFFB0C0D0)),
                Color.FromArgb(unchecked((int)0xFF80A0C0)),
                Color.FromArgb(unchecked((int)0xFF6090A0)),
                Color.FromArgb(unchecked((int)0xFF404040))
            }
        };

        public static readonly Color[] OrnamentsGridCol = new Color[]
        {
            Color.FromArgb(unchecked((int)0xFF303030)),
            Color.FromArgb(unchecked((int)0xFFD0D0D0))
        };

        // Event handler for OrnamentsGrid cell painting
        private void OrnamentsGrid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            var grid = sender as MyDataGridView;
            bool isSelected = e.State.HasFlag(DataGridViewElementStates.Selected);
            var g = e.Graphics;
            var cellRect = e.CellBounds;
            var textRect = new Rectangle(cellRect.Left, cellRect.Top + 33, cellRect.Width, 15);
            var cellValue = grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";
            Color selectColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnSelBackground];
            Color backColor = MainForm.ThemeColors[(int)ThemeColor.SamOrnBackground];
            int pal = (backColor.R + backColor.G + backColor.B) > 0x180 ? 1 : 0;

            e.Handled = true;

            using (SolidBrush brush = new SolidBrush(backColor))
                g.FillRectangle(brush, cellRect);

            using (SolidBrush brush = new SolidBrush(isSelected ? selectColor : SystemColors.Control))
                g.FillRectangle(brush, textRect);

            var orn = VTM.Ornaments[e.ColumnIndex + 1];
            int maxY = cellRect.Height - 31;
            int x1 = cellRect.Left;
            int y1 = cellRect.Top + 1;
            int nx = (cellRect.Width - 25) / 2;
            int yy = 0;
            int mx = nx + x1 + 10;

            if (orn != null && (orn.Length > 1 || orn.Offsets[0] != 0))
            {
                for (int y = 0; y < maxY / 2; y++)
                {
                    int x = orn.Offsets[yy];
                    int note = x % 12;
                    int oct = Math.Clamp(x / 12, -4, 4);

                    if (oct != 0)
                    {
                        Color octColor = OctCol[pal, oct + 4];
                        int octLeft = oct > 0 ? nx + x1 + 12 : nx + x1 + 1;
                        int octWidth = oct > 0 ? 13 : 11;

                        Rectangle octRect = new Rectangle(octLeft, y1 + y * 2, octWidth, 2);

                        using (Brush brush = new SolidBrush(octColor))
                            g.FillRectangle(brush, octRect);
                    }

                    Color noteColor = NoteCol[pal, note >= 0 ? note : note + 12];
                    Rectangle noteRect = new Rectangle(nx + x1 + 12 + note, y1 + y * 2, 2, 2);

                    using (Brush brush = new SolidBrush(noteColor))
                        g.FillRectangle(brush, noteRect);

                    if (++yy == orn.Length)
                        yy = orn.Loop;
                }
            }

            if (OrnUsage[e.ColumnIndex + 1])
                g.FillRectangle(Brushes.Red, cellRect.Right - 6, textRect.Top + 3, 3, 3);

            TextRenderer.DrawText(g, cellValue, grid.Font, textRect, isSelected ? Color.White : Color.Black,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

            using (Pen gridPen = new Pen(Color.FromArgb(unchecked((int)0xFFC0C0C0))))
            {
                g.DrawLine(gridPen, cellRect.Left, cellRect.Top + 33, cellRect.Right, cellRect.Top + 33);
                g.DrawLine(gridPen, cellRect.Right - 1, cellRect.Top, cellRect.Right - 1, cellRect.Bottom);
                g.DrawLine(gridPen, cellRect.Left, cellRect.Bottom - 1, cellRect.Right, cellRect.Bottom - 1);
            }
        }

        private void SamplesGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            var grid = sender as MyDataGridView;
            if (grid.CurrentCell?.ColumnIndex is int col && col >= 0 && col < 31)
            {
                ChangeSample(col + 1, true, false);
                // SetSampleGridScroll(col); // SetStringGrid2Scroll(col);
                if (grid.Focused)
                    SamplePreview();
            }
        }

        private void OrnamentsGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            var grid = sender as MyDataGridView;
            if (grid.CurrentCell?.ColumnIndex is int col && col >= 0 && col < 31)
            {
                ChangeOrnament(col + 1, true, false);
                // SetOrnamentGridScroll(col); // SetStringGrid3Scroll(col);
                if (grid.Focused)
                    OrnamentPreview();
            }
        }

        private void SamplesGrid_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                Samples_MouseWheelUp(sender, e);
            else
                Samples_MouseWheelDown(sender, e);
        }

        private void OrnamentsGrid_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                Ornaments_MouseWheelUp(sender, e);
            else
                Ornaments_MouseWheelDown(sender, e);
        }

        private void SamplesGrid_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var grid = sender as MyDataGridView;
                var hit = grid.HitTest(e.X, e.Y);
                int s1 = SampleIndex;
                int s2 = Samples.CopiedSample;
                grid.CurrentCell = grid.Rows[0].Cells[Math.Clamp(hit.ColumnIndex, 0, 30)];

                SwapSamples1.Text = s2 >= 0 ? $"Swap Samples {grid.Rows[0].Cells[s1 - 1].Value}<>{grid.Rows[0].Cells[s2 - 1].Value}" : "Swap Samples ...";
                SwapSamples1.Enabled = s2 >= 0 && s1 != s2;

                SamplesMenu.Show(grid, new Point(e.X, e.Y));
            }
        }

        private void OrnamentsGrid_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var grid = sender as MyDataGridView;
                var hit = grid.HitTest(e.X, e.Y);
                int o1 = OrnamentIndex;
                int o2 = Ornaments.CopiedOrnament;
                grid.CurrentCell = grid.Rows[0].Cells[Math.Clamp(hit.ColumnIndex, 0, 30)];

                SwapOrnaments1.Text = o2 >= 0 ? $"Swap Ornaments {grid.Rows[0].Cells[o1 - 1].Value}<>{grid.Rows[0].Cells[o2 - 1].Value}" : "Swap Ornaments ...";
                SwapOrnaments1.Enabled = o2 >= 0 && o1 != o2;

                OrnamentsMenu.Show(grid, new Point(e.X, e.Y));
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (TabControl.SelectedTab != SamplesTab)
                Samples.CopiedSample = -1;
            if (TabControl.SelectedTab != OrnamentsTab)
                Ornaments.CopiedOrnament = -1;
        }

        /* public void SpeedBpmEditEnter(object sender, EventArgs e)
        {
            SpeedBpmEdit.Text = (VTM.InitialDelay).ToString();
        }

        public void SpeedBpmEditExit(object sender, EventArgs e)
        {
            UpdateSpeedBPM();
        }

        public void SpeedBpmEditKeyUp(object sender, KeyEventArgs e)
        {
            int newValue = GetValue(SpeedBpmEdit.Text);
            if ((NewValue != -1) && (NewValue >= 1 && NewValue <= 255))
            {
                SpeedBpmUpDown.Value = NewValue;
            }
        } */

        public void SpeedBpmUpDownClick(object sender, EventArgs e)
        {
            SpeedBpmUpDown.Focus();
        }

        public void Edit7KeyPress(object sender, KeyPressEventArgs e)
        {
            UnsetFocus(e, Tracks);
        }

        public void VTAction_Execute(object sender, EventArgs e)
        {
            UIActionManager.Instance.Execute(sender, e);
        }

        public void VTAction_Update(object sender, EventArgs e)
        {
            UIActionManager.Instance.Update(sender, e);
        }

        public bool LoadTrackerModule(string fileName, int iter, ref int cnt, ref VTM vtm2, ref VTM vtm3)
        {
            bool result = false;

            SavedAsText = true;
            UndoWorking = true;

            if (vtm2 != null)
            {
                if (Path.GetExtension(fileName).ToLower() == ".pt3")
                    SavedAsText = false;
            }

            result = WaveOutAPI.LoadTrackerModule(fileName, iter, ref cnt, ref VTM, ref vtm2, ref vtm3);

            if (!result)
                return result;

            SetFileName(fileName);

            VtmFeaturesBox.SelectedIndex = (int)VTM.FeaturesLevel;
            SaveHeaderBox.SelectedIndex = VTM.HasHeader ? 0 : 1;
            Globals.MainForm.AddFileName(fileName);

            if (VTM.Positions.Length > 0)
            {
                PatternNumUpDown.Value = VTM.Positions.Value[0];
                Tracks.ShownPattern = VTM.Patterns[VTM.Positions.Value[0]];
                PatternLenUpDown.Value = Tracks.ShownPattern != null ? VTM.Patterns[VTM.Positions.Value[0]].Length : VTModule.DefaultPatternLength;
            }
            else
            {
                Tracks.ShownPattern = VTM.Patterns[0];
                PatternLenUpDown.Value = VTM.Patterns[0] != null ? VTM.Patterns[0].Length : VTModule.DefaultPatternLength;
            }

            if (AutoHLCheckBox.Checked)
                CalcHLStep();

            SpeedBpmUpDown.Value = VTM.InitialDelay;
            ToneTableUpDown.Value = (int)VTM.NoteTable;
            TitleTextBox.Text = VTM.Title;
            AuthorTextBox.Text = VTM.Author;
            PosDelay = VTM.InitialDelay;

            PositionsGrid.ColumnCount = VTM.Positions.Length;
            PositionsGrid.RowTemplate.Height = 30;
            PositionsGrid.Columns.Clear();
            PositionsGrid.Rows.Clear();

            for (int i = 0; i < VTM.Positions.Length; i++)
            {
                PositionsGrid.Columns.Add("", "");
                PositionsGrid.Columns[i].Width = 35;
            }

            PositionsGrid.Rows.Add();

            for (int i = 0; i < VTM.Positions.Length; i++)
                PositionsGrid[i, 0].Value = i == VTM.Positions.Loop ? $"L{VTM.Positions.Value[i]}" : VTM.Positions.Value[i].ToString();

            InitStringGridMetrics();
            //Samples = new TSamples(this);
            Samples.ShownSample = VTM.Samples[1];

            if (VTM.Samples[1] != null)
            {
                SampleLengthUpDown.Value = VTM.Samples[1].Length;
                SampleLoopUpDown.Value = VTM.Samples[1].Loop;
            }

            //Ornaments = new TOrnaments(this);
            //Ornaments.Show();
            Ornaments.ShownOrnament = VTM.Ornaments[1];

            if (VTM.Ornaments[1] != null)
            {
                OrnamentLenUpDown.Value = VTM.Ornaments[1].Length;
                OrnamentLoopUpDown.Value = VTM.Ornaments[1].Loop;
            }

            CalcTotLen();

            for (int i = 1; i < 32; i++)
            {
                if (VTM.Samples[i] == null)
                    continue;

                VTM.Samples[i].Enabled = true;

                for (int j = VTM.Samples[i].Length; j < VTModule.MaxSampleLength; j++)
                    VTM.Samples[i].Ticks[j] = new SampleTick();
            }

            for (int i = 1; i < 32; i++)
            {
                if (VTM.Ornaments[i] == null)
                    continue;

                for (int j = VTM.Ornaments[i].Length; j < VTModule.MaxOrnamentLength; j++)
                    VTM.Ornaments[i].Offsets[j] = 0;
            }

            Tracks.RedrawTracks();

            UndoWorking = false;
            SongChanged = false;
            BackupSongChanged = false;

            return result;
        }

        public static bool ValidFileName(string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();

            if (fileName.Trim() == "")
            {
                MessageBox.Show(Globals.MainForm, "Empty Filename", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            for (int i = 0; i < fileName.Length; i++)
            {
                if (Array.IndexOf(invalidChars, fileName[i]) != -1)
                {
                    MessageBox.Show(Globals.MainForm, "Invalid Filename", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        public static string GetDirName(string FullDirPath)
        {
            return Path.GetFileName(Path.GetDirectoryName(FullDirPath));
        }

        public static bool IsDirectoryWriteable(string path)
        {
            try
            {
                string testFile = Path.Combine(Path.GetFullPath(path), "chk.tmp");

                using (FileStream fs = File.Create(testFile, 1, FileOptions.DeleteOnClose | FileOptions.WriteThrough | FileOptions.SequentialScan))
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsHexValid(string hexValue)
        {
            char[] validChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'a', 'b', 'c', 'd', 'e', 'f' };

            for (int i = 0; i < hexValue.Length; i++)
            {
                if (Array.IndexOf(validChars, hexValue[i]) == -1)
                    return false;
            }

            return true;
        }

        public static bool IsDecValid(string decValue)
        {
            char[] validChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            for (int i = 0; i < decValue.Length; i++)
            {
                if (Array.IndexOf(validChars, decValue[i]) == -1)
                    return false;
            }

            return true;
        }

        public static sbyte Ns(int n)
        {
            return (n & 0x10) == 0 ? (sbyte)(n & 0xF) : (sbyte)(n | 0xF0);
        }

        public static string ExtractFileNameEx(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            string fileName = Path.GetFileName(filePath);
            int lastDot = fileName.LastIndexOf('.');

            if (lastDot > 0)
                return fileName.Substring(0, lastDot);

            return fileName;
        }

        private static int[] _colSpace = new int[] { 4, 7, 11, 16, 21, 25, 30, 35, 39, 44 };

        public static bool ColSpace(int i)
        {
            return _colSpace.Contains(i);
        }

        public static int ColTab(int i)
        {
            int j = 0;

            while (i >= ColTabs[j])
                j++;

            return j - 1;
        }

        public static int ColTab1(int i, ref int[] ColTabs1)
        {
            int j = 0;

            while (i >= ColTabs1[j])
                j++;

            return j - 1;
        }

        public static int SColTab(int i)
        {
            int j = 0;

            while (i >= SColTabs[j])
                j++;

            return j - 1;
        }

        public static void ChangeEnvelopeWhenToneTableChanged(VTM vtm, NoteTableType oldToneTable, NoteTableType newToneTable)
        {
            double coeff;
            int envelopeNote;
            int oldEnvelope;
            int notefreq;

            for (int i = -1; i < 85; i++)
            {
                Pattern pattern = (i == -1 ? vtm.ReservedPattern : vtm.Patterns[i]);

                if (pattern == null)
                    continue;

                for (int j = 0; j < pattern.Length; j++)
                {
                    Line line = pattern.Lines[j];
                    oldEnvelope = line.Envelope;

                    if (oldEnvelope == 0)
                        continue;

                    coeff = VTModule.GetNoteFreq(oldToneTable, 0) / VTModule.GetNoteFreq(newToneTable, 0);
                    envelopeNote = VTModule.GetNoteByEnvelope2(oldToneTable, oldEnvelope);

                    if (envelopeNote == 0)
                        line.Envelope = (ushort)Math.Round(oldEnvelope / coeff);
                    else
                    {
                        notefreq = VTModule.GetNoteFreq(newToneTable, envelopeNote);
                        line.Envelope = (ushort)Math.Round(notefreq / 16.0);
                    }
                }
            }
        }

        public static void SetFromAndToPosition()
        {
            if (WaveAyumi.PlayAll)
            {
                WaveAyumi.FromPosition = 0;
                WaveAyumi.ToPosition = PlayingWindow[0].VTM.Positions.Length - 1;
                LeadWindow = PlayingWindow[0];
            }
            else if (PlayingWindow[0].PositionsGrid.Selection.Right > PlayingWindow[0].PositionsGrid.Selection.Left)
            {
                WaveAyumi.FromPosition = PlayingWindow[0].PositionsGrid.Selection.Left;
                WaveAyumi.ToPosition = PlayingWindow[0].PositionsGrid.Selection.Right;
                LeadWindow = PlayingWindow[0];
            }
            else if (PlayingWindow[1] != null && PlayingWindow[1].PositionsGrid.Selection.Right > PlayingWindow[1].PositionsGrid.Selection.Left)
            {
                WaveAyumi.FromPosition = PlayingWindow[1].PositionsGrid.Selection.Left;
                WaveAyumi.ToPosition = PlayingWindow[1].PositionsGrid.Selection.Right;
                LeadWindow = PlayingWindow[1];
            }
            else if (PlayingWindow[2] != null && PlayingWindow[2].PositionsGrid.Selection.Right > PlayingWindow[2].PositionsGrid.Selection.Left)
            {
                WaveAyumi.FromPosition = PlayingWindow[2].PositionsGrid.Selection.Left;
                WaveAyumi.ToPosition = PlayingWindow[2].PositionsGrid.Selection.Right;
                LeadWindow = PlayingWindow[2];
            }
            else
            {
                WaveAyumi.FromPosition = PlayingWindow[0].PositionIndex;
                WaveAyumi.ToPosition = PlayingWindow[0].PositionIndex;
                LeadWindow = PlayingWindow[0];
            }

            WaveAyumi.PositionCount = WaveAyumi.ToPosition - WaveAyumi.FromPosition + 1;

            WaveAyumi.PrevPosNum[0] = LeadWindow.PositionIndex;
            WaveAyumi.PrevPatNum[0] = LeadWindow.PatternIndex;

            if (LeadWindow.TSWindow[0] != null)
            {
                WaveAyumi.PrevPosNum[1] = LeadWindow.TSWindow[0].PositionIndex;
                WaveAyumi.PrevPatNum[1] = LeadWindow.TSWindow[0].PatternIndex;
            }

            if (LeadWindow.TSWindow[1] != null)
            {
                WaveAyumi.PrevPosNum[2] = LeadWindow.TSWindow[1].PositionIndex;
                WaveAyumi.PrevPatNum[2] = LeadWindow.TSWindow[1].PatternIndex;
            }
        }

        public static void SetChildPositions()
        {
            RestorePositionAndPattern(LeadWindow, WaveAyumi.PrevPosNum[0], WaveAyumi.PrevPatNum[0]);
            if (LeadWindow.TSWindow[0] != null)
                RestorePositionAndPattern(LeadWindow.TSWindow[0], WaveAyumi.PrevPosNum[1], WaveAyumi.PrevPatNum[1]);
            if (LeadWindow.TSWindow[1] != null)
                RestorePositionAndPattern(LeadWindow.TSWindow[1], WaveAyumi.PrevPosNum[2], WaveAyumi.PrevPatNum[2]);
        }

        public static void ChangePositions(ChildForm playWindow)
        {
            if (playWindow != LeadWindow)
            {
                // Non-lead window logic
                if (playWindow.PositionIndex >= playWindow.VTM.Positions.Length - 1)
                    VTModule.Module_SetCurrentPosition(playWindow.VTM.Positions.Loop);
                else
                    VTModule.Module_SetCurrentPosition(VTModule.PlayArgs[VTModule.ChipIndex].PositionIndex + 1);

                playWindow.PositionIndex = VTModule.PlayArgs[VTModule.ChipIndex].PositionIndex;

                return;
            }

            // Lead:
            if (VTModule.PlayArgs[VTModule.ChipIndex].PositionIndex == WaveAyumi.ToPosition)
            {
                if (WaveAyumi.PlayAll)
                    VTModule.Module_SetCurrentPosition(playWindow.VTM.Positions.Loop);
                else
                    VTModule.Module_SetCurrentPosition(WaveAyumi.FromPosition);

                if (WaveOutAPI.ExportLoops > 0)
                    WaveOutAPI.ExportLoops--;
                else
                    WaveOutAPI.ExportFinished = true;
            }
            else
                VTModule.Module_SetCurrentPosition(VTModule.PlayArgs[VTModule.ChipIndex].PositionIndex + 1);

            LeadWindow.PositionIndex = VTModule.PlayArgs[VTModule.ChipIndex].PositionIndex;
        }

        public static void RestorePositionAndPattern(ChildForm childForm, int positionIndex, int patternIndex)
        {
            childForm.PositionIndex = positionIndex;
            childForm.PatternIndex = patternIndex;
            childForm.Tracks.HideCaret();
            childForm.Tracks.RedrawTracks();
            childForm.Tracks.ShowCaret();
        }

        public static void SetPlayingWindow(int index, ChildForm childWindow)
        {
            _playingWindow[index] = childWindow;
            AY.PlayingModule[index] = _playingWindow[index]?.VTM;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Tracks?.Dispose();

            base.OnFormClosing(e);
        }

        public override string ToString()
        {
            if (String.IsNullOrWhiteSpace(Text))
                return "---";

            return Text;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static ChildForm[] PlayingWindow
        {
            get => _playingWindow;
        }
    }

    public enum TDragState
    {
        DragEnter,
        DragLeave,
        DragMove,
        DragCancel
    }

    public enum TChangeAction
    {
        LoadPattern,
        LoadSample,
        LoadOrnament,
        OrGen,
        CopyOrnamentToOrnament,
        CopySampleToSample,
        InsertPatternFromClipboard,
        ChangeNote,
        ChangeNoteAndParams,
        ChangeEnvelopePeriod,
        ChangeNoise,
        ChangeSample,
        ChangeEnvelopeType,
        ChangeOrnament,
        ChangeVolume,
        ChangeSpecialCommandNumber,
        ChangeSpecialCommandDelay,
        ChangeSpecialCommandParameter,
        ChangeSpeed,
        ChangePatternSize,
        ChangePatternsSize,
        ChangeSampleSize,
        ChangeSampleLoop,
        ChangeEntireSample,
        ChangeOrnamentSize,
        ChangeOrnamentLoop,
        ChangeEntireOrnament,
        ChangeOrnamentValue,
        ChangeSampleValue,
        InsertPosition,
        DeletePosition,
        ChangePositionListLoop,
        ChangePositionValue,
        ChangeToneTable,
        ChangeFeatures,
        ChangeHeader,
        ChangeAuthor,
        ChangeTitle,
        PatternInsertLine,
        PatternDeleteLine,
        PatternClearLine,
        PatternClearSelection,
        TransposePattern,
        TracksManagerCopy,
        ExpandCompressPattern,
        ChangePositionsAndPatterns,
        SwapSamples,
        SwapOrnaments,
        ChangePatternContent
    }

    public enum TSamToggles
    {
        MixTone,
        MixNoise,
        MaskEnv,
        SgnTone,
        SgnNoise,
        AccTone,
        AccNoise,
        AccVol,
        SgnToneP,
        SgnToneM,
        SgnNoiseP,
        SgnNoiseM,
        AccVolP,
        AccVolM,
        AccTone_,
        AccNoise_,
        AccVol_,
        AccToneA,
        AccNoiseA
    }

    public enum TSamNumbers
    {
        Tone,
        Noise,
        NoiseAbs,
        Vol
    }
    public enum PlayStopState
    {
        Play,
        Stop
    }

    public class ChangeParams : ICloneable
    {
        public int CurrentPattern;
        public int CurrentPosition;
        public int PatternShownFrom;
        public int PatternCursorX;
        public int PatternCursorY;
        public int SampleShownFrom;
        public int SampleCursorX;
        public int SampleCursorY;
        public int PositionListLen;
        public int OrnamentShownFrom;
        public int OrnamentCursor;
        public int PrevLoop;
        public sbyte NoteParam;
        public byte SampleParam;
        public byte OrnamentParam;
        public sbyte VolumeParam;
        public byte EnvelopeParam;
        public int Note;
        public int NoteP;
        public int EnvelopePeriod;
        public int Noise;
        public int SampleNum;
        public int EnvelopeType;
        public int OrnamentNum;
        public int Volume;
        public int SCNumber;
        public int SCDelay;
        public int SCParameter;
        public int Speed;
        public int Size;
        public int Loop;
        public int Value;
        public int Table;
        public int NewFeatures;
        public int NewHeader;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class ChannelState
    {
        public bool Muted;
    }

    public class ChangeParameters : ICloneable
    {
        public string Str;
        public ChangeParams Params;

        public ChangeParameters()
        {
            Params = new ChangeParams();
        }

        public object Clone()
        {
            ChangeParameters clone = new ChangeParameters
            {
                Params = (ChangeParams)Params.Clone()
            };
            return clone;
        }
    }

    public class ChangePattern : ICloneable
    {
        public int Number;
        public Pattern Pattern;

        public ChangePattern()
        {
            Pattern = new Pattern();
        }

        public object Clone()
        {
            ChangePattern clone = new ChangePattern
            {
                Pattern = (Pattern)Pattern.Clone()
            };
            return clone;
        }
    }

    public class ChangeOnePattern : ICloneable
    {
        public Pattern OldPattern;
        public Pattern NewPattern;

        public ChangeOnePattern()
        {
            OldPattern = new Pattern();
            NewPattern = new Pattern();
        }

        public object Clone()
        {
            ChangeOnePattern clone = new ChangeOnePattern
            {
                OldPattern = (Pattern)OldPattern.Clone(),
                NewPattern = (Pattern)NewPattern.Clone()
            };
            return clone;
        }
    }

    public class ChangeSample : ICloneable
    {
        public int Number;
        public Sample OldSample;
        public Sample NewSample;

        public ChangeSample()
        {
            OldSample = new Sample();
            NewSample = new Sample();
        }

        public object Clone()
        {
            ChangeSample clone = new ChangeSample
            {
                OldSample = (Sample)OldSample.Clone(),
                NewSample = (Sample)NewSample.Clone()
            };
            return clone;
        }
    }

    public class ChangeOrnament : ICloneable
    {
        public int Number;
        public Ornament OldOrnament;
        public Ornament NewOrnament;

        public ChangeOrnament()
        {
            OldOrnament = new Ornament();
            NewOrnament = new Ornament();
        }

        public object Clone()
        {
            ChangeOrnament clone = new ChangeOrnament
            {
                OldOrnament = (Ornament)OldOrnament.Clone(),
                NewOrnament = (Ornament)NewOrnament.Clone()
            };
            return clone;
        }
    }

    public class ComParams
    {
        public int CurrentSample;
        public int CurrentOrnament;
        // Structure:
        //   [0] => OldPatterns - Array of TChangePattern
        //   [1] => NewPatterns - Array of TChangePattern
        public ChangePattern[][] Patterns;
        public int[] NilPatterns;
        public ChangeOnePattern ChangedPattern;
        public ChangeSample EntireSample;
        public ChangeOrnament EntireOrnament;

        public ComParams()
        {
            Patterns = Array.Empty<ChangePattern[]>();
            NilPatterns = Array.Empty<int>();
            ChangedPattern = new ChangeOnePattern();
            EntireSample = new ChangeSample();
            EntireOrnament = new ChangeOrnament();
        }
    }

    public class ChangeListItem
    {
        public TChangeAction Action;
        public int Line;
        public int Channel;
        public Rectangle OldGridSelection;
        public Rectangle NewGridSelection;
        public Pattern Pattern;
        public Position PositionList;
        public Ornament Ornament;
        public Sample Sample;
        public SampleTick SampleLineValues;
        public ComParams ComParams;
        public ChangeParameters OldParams;
        public ChangeParameters NewParams;
        public int OtherMDI;

        public ChangeListItem()
        {
            Pattern = new Pattern();
            PositionList = new Position();
            Ornament = new Ornament();
            Sample = new Sample();
            SampleLineValues = new SampleTick();
            ComParams = new ComParams();
            OldParams = new ChangeParameters();
            NewParams = new ChangeParameters();
        }
    }

    public class ChannelMetrics
    {
        public int BoxLeft;
        public int BoxWidth;
        public int ButtonWidth;
        public int ToneLeft;
        public int NoiseLeft;
        public int EnvelopeLeft;
        public int SoloLeft;
    }

    public class ChannelButtons
    {
        public GroupBox Box;
        public CheckBox Mute_But;
        public int Mute_But_s;
        public CheckBox Solo_But;
        public int Solo_But_s;
        public CheckBox T_But;
        public int T_But_s;
        public CheckBox N_But;
        public int N_But_s;
        public CheckBox E_But;
        public int E_But_s;
    }

    public class ChannelButtonsState
    {
        public int Mute_But_s;
        public int Solo_But_s;
        public int T_But_s;
        public int N_But_s;
        public int E_But_s;
    }
}
