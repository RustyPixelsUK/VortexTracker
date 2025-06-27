using System.Windows.Forms;
using VortexTracker.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace VortexTracker
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components;

        // Clean up any resources being used.
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            StatusBar = new StatusStrip();
            StatusLabel0 = new ToolStripStatusLabel();
            StatusLabel1 = new ToolStripStatusLabel();
            StatusLabel2 = new ToolStripStatusLabel();
            StatusLabel3 = new ToolStripStatusLabel();
            ToolStrip1 = new MyToolStrip();
            ImageList1 = new ImageList(components);
            NewButton = new ToolStripButton();
            FileOpenButton = new ToolStripButton();
            FileSaveButton = new ToolStripButton();
            ToolButton3 = new ToolStripSeparator();
            UndoButton = new ToolStripButton();
            RedoButton = new ToolStripButton();
            ToolButton8 = new ToolStripSeparator();
            PlayStopButton = new ToolStripButton();
            PlayFromStartButton = new ToolStripButton();
            PlayPatternFromCurrentLineButton = new ToolStripButton();
            PlayPatternFromStartButton = new ToolStripButton();
            ToggleLoopingButton = new ToolStripButton();
            LoopAllFilesButton = new ToolStripButton();
            ToolButton4 = new ToolStripSeparator();
            ToggleSamplesButton = new ToolStripButton();
            TracksManagerButton = new ToolStripButton();
            GlobalTranspositionButton = new ToolStripButton();
            PluginManagerButton = new ToolStripButton();
            ToolButton5 = new ToolStripSeparator();
            ChipButton = new ToolStripButton();
            ChannelsButton = new ToolStripButton();
            OptionsButton = new ToolStripButton();
            VolumeTrackBar = new ToolStripTrackBar();
            MainMenu1 = new MenuStrip();
            File = new ToolStripMenuItem();
            FileNewItem = new ToolStripMenuItem();
            NewTurboSound1 = new ToolStripMenuItem();
            NewTurboSound3 = new ToolStripMenuItem();
            JoinTracks = new ToolStripMenuItem();
            FileOpen = new ToolStripMenuItem();
            OpenDemoSong = new ToolStripMenuItem();
            FileClose = new ToolStripMenuItem();
            FileSave = new ToolStripMenuItem();
            FileSaveAs = new ToolStripMenuItem();
            SaveAsTwoModules = new ToolStripMenuItem();
            SaveAsTemplate = new ToolStripMenuItem();
            Sep15 = new ToolStripSeparator();
            ExportWAV = new ToolStripMenuItem();
            ExportPSG = new ToolStripMenuItem();
            ExportYM = new ToolStripMenuItem();
            Exports = new ToolStripMenuItem();
            SaveSNDH = new ToolStripMenuItem();
            SaveForZX = new ToolStripMenuItem();
            Sep10 = new ToolStripSeparator();
            Options = new ToolStripMenuItem();
            Sep11 = new ToolStripSeparator();
            RFile1 = new ToolStripMenuItem();
            RFile2 = new ToolStripMenuItem();
            RFile3 = new ToolStripMenuItem();
            RFile4 = new ToolStripMenuItem();
            RFile5 = new ToolStripMenuItem();
            RFile6 = new ToolStripMenuItem();
            Sep12 = new ToolStripSeparator();
            FileExit = new ToolStripMenuItem();
            Play = new ToolStripMenuItem();
            PlayStop = new ToolStripMenuItem();
            PlayFromLine = new ToolStripMenuItem();
            PlayFromStart = new ToolStripMenuItem();
            PlayPatternFromCurrentLine = new ToolStripMenuItem();
            PlayPatternFromStart = new ToolStripMenuItem();
            Stop = new ToolStripMenuItem();
            Sep16 = new ToolStripSeparator();
            ToggleLooping = new ToolStripMenuItem();
            ToggleLoopingAll = new ToolStripMenuItem();
            Sep14 = new ToolStripSeparator();
            ToggleSamples = new ToolStripMenuItem();
            Edit = new ToolStripMenuItem();
            Undo1 = new ToolStripMenuItem();
            Redo1 = new ToolStripMenuItem();
            Sep13 = new ToolStripSeparator();
            EditCut1 = new ToolStripMenuItem();
            EditCopy1 = new ToolStripMenuItem();
            EditPaste1 = new ToolStripMenuItem();
            Sep17 = new ToolStripSeparator();
            CopyToModPlug1 = new ToolStripMenuItem();
            CopyToRenoise1 = new ToolStripMenuItem();
            CopyToFami1 = new ToolStripMenuItem();
            Sep9 = new ToolStripSeparator();
            ToggleSamples1 = new ToolStripMenuItem();
            TracksManager = new ToolStripMenuItem();
            GlobalTransposition = new ToolStripMenuItem();
            PluginManager = new ToolStripMenuItem();
            Window = new ToolStripMenuItem();
            Maximize = new ToolStripMenuItem();
            Normal = new ToolStripMenuItem();
            Help1 = new ToolStripMenuItem();
            HelpAbout = new ToolStripMenuItem();
            OpenDialog = new OpenFileDialog();
            SaveDialog1 = new SaveFileDialog();
            PopupMenu1 = new ContextMenuStrip(components);
            Color1 = new ToolStripMenuItem();
            PositionColorL5 = new ToolStripMenuItem();
            PositionColorL4 = new ToolStripMenuItem();
            PositionColorL3 = new ToolStripMenuItem();
            PositionColorL2 = new ToolStripMenuItem();
            PositionColorL1 = new ToolStripMenuItem();
            PositionColorDefault = new ToolStripMenuItem();
            PositionColorRed = new ToolStripMenuItem();
            PositionColorGreen = new ToolStripMenuItem();
            PositionColorBlue = new ToolStripMenuItem();
            PositionColorMaroon = new ToolStripMenuItem();
            PositionColorPurple = new ToolStripMenuItem();
            PositionColorGray = new ToolStripMenuItem();
            PositionColorTeal = new ToolStripMenuItem();
            ResetColors = new ToolStripMenuItem();
            Sep2 = new ToolStripSeparator();
            SetLoopPosition = new ToolStripMenuItem();
            Sep1 = new ToolStripSeparator();
            InsertPosition = new ToolStripMenuItem();
            DeletePosition = new ToolStripMenuItem();
            DuplicatePosition = new ToolStripMenuItem();
            ClonePosition = new ToolStripMenuItem();
            Sep3 = new ToolStripSeparator();
            ChangePatternsLength1 = new ToolStripMenuItem();
            Sep4 = new ToolStripSeparator();
            RenumberPatterns = new ToolStripMenuItem();
            AutoNumeratePatterns = new ToolStripMenuItem();
            CleanSelectedPatterns = new ToolStripMenuItem();
            SaveDialogSNDH = new SaveFileDialog();
            SaveDialogZXAY = new SaveFileDialog();
            PopupMenu2 = new ContextMenuStrip(components);
            Undo2 = new ToolStripMenuItem();
            Redo2 = new ToolStripMenuItem();
            Sep5 = new ToolStripSeparator();
            EditCopy2 = new ToolStripMenuItem();
            EditCut2 = new ToolStripMenuItem();
            EditPaste2 = new ToolStripMenuItem();
            Merge1 = new ToolStripMenuItem();
            N20 = new ToolStripSeparator();
            CopyTo = new ToolStripMenuItem();
            CopyToModPlug2 = new ToolStripMenuItem();
            CopyToRenoise2 = new ToolStripMenuItem();
            CopyToFami2 = new ToolStripMenuItem();
            Sep6 = new ToolStripSeparator();
            TransposeUp1 = new ToolStripMenuItem();
            TransposeDown1 = new ToolStripMenuItem();
            TransposeUp3 = new ToolStripMenuItem();
            TransposeDown3 = new ToolStripMenuItem();
            TransposeUp5 = new ToolStripMenuItem();
            TransposeDown5 = new ToolStripMenuItem();
            TransposeUp12 = new ToolStripMenuItem();
            TransposeDown12 = new ToolStripMenuItem();
            Sep7 = new ToolStripSeparator();
            ExpandPattern = new ToolStripMenuItem();
            CompressPattern = new ToolStripMenuItem();
            SplitPattern = new ToolStripMenuItem();
            PatternPacker = new ToolStripMenuItem();
            Sep8 = new ToolStripSeparator();
            SwapChannelsLeft = new ToolStripMenuItem();
            SwapChannelsRight = new ToolStripMenuItem();
            PopupMenu3 = new ContextMenuStrip(components);
            File2 = new ToolStripMenuItem();
            Play5 = new ToolStripMenuItem();
            TrackTools = new ToolStripMenuItem();
            BackupTimer = new System.Windows.Forms.Timer(components);
            SyncCheckTimer = new System.Windows.Forms.Timer(components);
            SyncFinishTimer = new System.Windows.Forms.Timer(components);
            SyncCopyBuffers = new System.Windows.Forms.Timer(components);
            MIDITimer = new System.Windows.Forms.Timer(components);
            StatusBar.SuspendLayout();
            ToolStrip1.SuspendLayout();
            MainMenu1.SuspendLayout();
            PopupMenu1.SuspendLayout();
            PopupMenu2.SuspendLayout();
            PopupMenu3.SuspendLayout();
            SuspendLayout();
            // 
            // StatusBar
            // 
            StatusBar.AutoSize = false;
            StatusBar.Items.AddRange(new ToolStripItem[] { StatusLabel0, StatusLabel1, StatusLabel2, StatusLabel3 });
            StatusBar.Location = new Point(0, 557);
            StatusBar.Name = "StatusBar";
            StatusBar.Size = new Size(611, 22);
            StatusBar.SizingGrip = false;
            StatusBar.TabIndex = 64;
            StatusBar.DoubleClick += StatusBar_DoubleClick;
            // 
            // StatusLabel0
            // 
            StatusLabel0.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            StatusLabel0.Name = "StatusLabel0";
            StatusLabel0.Size = new Size(540, 17);
            StatusLabel0.Spring = true;
            StatusLabel0.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // StatusLabel1
            // 
            StatusLabel1.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            StatusLabel1.Name = "StatusLabel1";
            StatusLabel1.Size = new Size(4, 17);
            // 
            // StatusLabel2
            // 
            StatusLabel2.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            StatusLabel2.Name = "StatusLabel2";
            StatusLabel2.Size = new Size(4, 17);
            // 
            // StatusLabel3
            // 
            StatusLabel3.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            StatusLabel3.Name = "StatusLabel3";
            StatusLabel3.Size = new Size(48, 17);
            StatusLabel3.Text = "000 ms";
            // 
            // ToolBar2
            // 
            ToolStrip1.ImageList = ImageList1;
            ToolStrip1.Items.AddRange(new ToolStripItem[] { NewButton, FileOpenButton, FileSaveButton, ToolButton3, UndoButton, RedoButton, ToolButton8, PlayStopButton, PlayFromStartButton, PlayPatternFromCurrentLineButton, PlayPatternFromStartButton, ToggleLoopingButton, LoopAllFilesButton, ToolButton4, ToggleSamplesButton, TracksManagerButton, GlobalTranspositionButton, PluginManagerButton, ToolButton5, ChipButton, ChannelsButton, OptionsButton, VolumeTrackBar });
            ToolStrip1.Location = new Point(0, 24);
            ToolStrip1.Name = "ToolBar2";
            ToolStrip1.Size = new Size(611, 25);
            ToolStrip1.TabIndex = 0;
            // 
            // ImageList1
            // 
            ImageList1.ColorDepth = ColorDepth.Depth32Bit;
            ImageList1.ImageStream = (ImageListStreamer)resources.GetObject("ImageList1.ImageStream");
            ImageList1.TransparentColor = Color.Transparent;
            ImageList1.Images.SetKeyName(0, "0.png");
            ImageList1.Images.SetKeyName(1, "1.png");
            ImageList1.Images.SetKeyName(2, "2.png");
            ImageList1.Images.SetKeyName(3, "3.png");
            ImageList1.Images.SetKeyName(4, "4.png");
            ImageList1.Images.SetKeyName(5, "5.png");
            ImageList1.Images.SetKeyName(6, "6.png");
            ImageList1.Images.SetKeyName(7, "7.png");
            ImageList1.Images.SetKeyName(8, "8.png");
            ImageList1.Images.SetKeyName(9, "9.png");
            ImageList1.Images.SetKeyName(10, "10.png");
            ImageList1.Images.SetKeyName(11, "11.png");
            ImageList1.Images.SetKeyName(12, "12.png");
            ImageList1.Images.SetKeyName(13, "13.png");
            ImageList1.Images.SetKeyName(14, "14.png");
            ImageList1.Images.SetKeyName(15, "15.png");
            ImageList1.Images.SetKeyName(16, "16.png");
            ImageList1.Images.SetKeyName(17, "17.png");
            ImageList1.Images.SetKeyName(18, "18.png");
            ImageList1.Images.SetKeyName(19, "19.png");
            ImageList1.Images.SetKeyName(20, "20.png");
            ImageList1.Images.SetKeyName(21, "21.png");
            ImageList1.Images.SetKeyName(22, "22.png");
            ImageList1.Images.SetKeyName(23, "23.png");
            ImageList1.Images.SetKeyName(24, "24.png");
            ImageList1.Images.SetKeyName(25, "25.png");
            ImageList1.Images.SetKeyName(26, "26.png");
            ImageList1.Images.SetKeyName(27, "27.png");
            ImageList1.Images.SetKeyName(28, "28.png");
            ImageList1.Images.SetKeyName(29, "29.png");
            ImageList1.Images.SetKeyName(30, "30.png");
            ImageList1.Images.SetKeyName(31, "31.png");
            ImageList1.Images.SetKeyName(32, "32.png");
            ImageList1.Images.SetKeyName(33, "33.png");
            ImageList1.Images.SetKeyName(34, "34.png");
            ImageList1.Images.SetKeyName(35, "35.png");
            ImageList1.Images.SetKeyName(36, "36.png");
            ImageList1.Images.SetKeyName(37, "37.png");
            ImageList1.Images.SetKeyName(38, "38.png");
            ImageList1.Images.SetKeyName(39, "39.png");
            ImageList1.Images.SetKeyName(40, "40.png");
            ImageList1.Images.SetKeyName(41, "41.png");
            ImageList1.Images.SetKeyName(42, "42.png");
            ImageList1.Images.SetKeyName(43, "43.png");
            ImageList1.Images.SetKeyName(44, "44.png");
            ImageList1.Images.SetKeyName(45, "45.png");
            ImageList1.Images.SetKeyName(46, "46.png");
            ImageList1.Images.SetKeyName(47, "47.png");
            ImageList1.Images.SetKeyName(48, "48.png");
            ImageList1.Images.SetKeyName(49, "49.png");
            ImageList1.Images.SetKeyName(50, "50.png");
            ImageList1.Images.SetKeyName(51, "51.png");
            ImageList1.Images.SetKeyName(52, "52.png");
            ImageList1.Images.SetKeyName(53, "53.png");
            // 
            // NewButton
            // 
            NewButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            NewButton.ImageIndex = 6;
            NewButton.Name = "NewButton";
            NewButton.Size = new Size(23, 22);
            NewButton.Text = "&New";
            NewButton.Click += UIAction_Execute;
            // 
            // FileOpenButton
            // 
            FileOpenButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            FileOpenButton.ImageIndex = 7;
            FileOpenButton.Name = "FileOpenButton";
            FileOpenButton.Size = new Size(23, 22);
            FileOpenButton.Text = "&Open";
            FileOpenButton.Click += UIAction_Execute;
            // 
            // FileSaveButton
            // 
            FileSaveButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            FileSaveButton.ImageIndex = 8;
            FileSaveButton.Name = "FileSaveButton";
            FileSaveButton.Size = new Size(23, 22);
            FileSaveButton.Text = "&Save";
            FileSaveButton.Click += UIAction_Execute;
            // 
            // ToolButton3
            // 
            ToolButton3.Name = "ToolButton3";
            ToolButton3.Size = new Size(6, 25);
            // 
            // UndoButton
            // 
            UndoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            UndoButton.ImageIndex = 3;
            UndoButton.Name = "UndoButton";
            UndoButton.Size = new Size(23, 22);
            UndoButton.Text = "Undo";
            UndoButton.Click += UIAction_Execute;
            // 
            // RedoButton
            // 
            RedoButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            RedoButton.ImageIndex = 4;
            RedoButton.Name = "RedoButton";
            RedoButton.Size = new Size(23, 22);
            RedoButton.Text = "Redo";
            RedoButton.Click += UIAction_Execute;
            // 
            // ToolButton8
            // 
            ToolButton8.Name = "ToolButton8";
            ToolButton8.Size = new Size(6, 25);
            // 
            // PlayStopButton
            // 
            PlayStopButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            PlayStopButton.ImageIndex = 18;
            PlayStopButton.Name = "PlayStopButton";
            PlayStopButton.Size = new Size(23, 22);
            PlayStopButton.Text = "Play/Stop";
            PlayStopButton.Click += UIAction_Execute;
            // 
            // PlayFromStartButton
            // 
            PlayFromStartButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            PlayFromStartButton.ImageIndex = 38;
            PlayFromStartButton.Name = "PlayFromStartButton";
            PlayFromStartButton.Size = new Size(23, 22);
            PlayFromStartButton.Text = "Play From Start";
            PlayFromStartButton.Click += UIAction_Execute;
            // 
            // PlayPatternFromCurrentLineButton
            // 
            PlayPatternFromCurrentLineButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            PlayPatternFromCurrentLineButton.ImageIndex = 40;
            PlayPatternFromCurrentLineButton.Name = "PlayPatternFromCurrentLineButton";
            PlayPatternFromCurrentLineButton.Size = new Size(23, 22);
            PlayPatternFromCurrentLineButton.Text = "Play Pattern From Current Line";
            PlayPatternFromCurrentLineButton.Click += UIAction_Execute;
            // 
            // PlayPatternFromStartButton
            // 
            PlayPatternFromStartButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            PlayPatternFromStartButton.ImageIndex = 41;
            PlayPatternFromStartButton.Name = "PlayPatternFromStartButton";
            PlayPatternFromStartButton.Size = new Size(23, 22);
            PlayPatternFromStartButton.Text = "Play Pattern From Start";
            PlayPatternFromStartButton.Click += UIAction_Execute;
            // 
            // ToggleLoopingButton
            // 
            ToggleLoopingButton.CheckOnClick = true;
            ToggleLoopingButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ToggleLoopingButton.ImageIndex = 37;
            ToggleLoopingButton.Name = "ToggleLoopingButton";
            ToggleLoopingButton.Size = new Size(23, 22);
            ToggleLoopingButton.Text = "Toggle Looping";
            ToggleLoopingButton.Click += UIAction_Execute;
            // 
            // LoopAllFilesButton
            // 
            LoopAllFilesButton.CheckOnClick = true;
            LoopAllFilesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            LoopAllFilesButton.ImageIndex = 39;
            LoopAllFilesButton.Name = "LoopAllFilesButton";
            LoopAllFilesButton.Size = new Size(23, 22);
            LoopAllFilesButton.Text = "Loop All Files";
            LoopAllFilesButton.Click += UIAction_Execute;
            // 
            // ToolButton4
            // 
            ToolButton4.Name = "ToolButton4";
            ToolButton4.Size = new Size(6, 25);
            // 
            // ToggleSamplesButton
            // 
            ToggleSamplesButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ToggleSamplesButton.ImageIndex = 31;
            ToggleSamplesButton.Name = "ToggleSamplesButton";
            ToggleSamplesButton.Size = new Size(23, 22);
            ToggleSamplesButton.Text = "Toggle Samples";
            ToggleSamplesButton.Click += UIAction_Execute;
            // 
            // TracksManagerButton
            // 
            TracksManagerButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            TracksManagerButton.ImageIndex = 42;
            TracksManagerButton.Name = "TracksManagerButton";
            TracksManagerButton.Size = new Size(23, 22);
            TracksManagerButton.Text = "Tracks Manager";
            TracksManagerButton.Click += UIAction_Execute;
            // 
            // GlobalTranspositionButton
            // 
            GlobalTranspositionButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            GlobalTranspositionButton.ImageIndex = 36;
            GlobalTranspositionButton.Name = "GlobalTranspositionButton";
            GlobalTranspositionButton.Size = new Size(23, 22);
            GlobalTranspositionButton.Text = "Global Transposition";
            GlobalTranspositionButton.Click += UIAction_Execute;
            // 
            // PluginManagerButton
            // 
            PluginManagerButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            PluginManagerButton.Image = (Image)resources.GetObject("PluginManagerButton.Image");
            PluginManagerButton.Name = "PluginManagerButton";
            PluginManagerButton.Size = new Size(23, 22);
            PluginManagerButton.Text = "Plugin Manager";
            PluginManagerButton.Click += UIAction_Execute;
            // 
            // ToolButton5
            // 
            ToolButton5.Name = "ToolButton5";
            ToolButton5.Size = new Size(6, 25);
            // 
            // ChipButton
            // 
            ChipButton.Name = "ChipButton";
            ChipButton.Size = new Size(29, 22);
            ChipButton.Text = "YM";
            ChipButton.Click += UIAction_Execute;
            // 
            // ChannelsButton
            // 
            ChannelsButton.Name = "ChannelsButton";
            ChannelsButton.Size = new Size(34, 22);
            ChannelsButton.Text = "ABC";
            ChannelsButton.Click += UIAction_Execute;
            // 
            // OptionsButton
            // 
            OptionsButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            OptionsButton.ImageIndex = 21;
            OptionsButton.Name = "OptionsButton";
            OptionsButton.Size = new Size(23, 22);
            OptionsButton.Text = "Options";
            OptionsButton.Click += UIAction_Execute;
            // 
            // VolumeTrackBar
            // 
            VolumeTrackBar.Left = 464;
            VolumeTrackBar.Maximum = 64;
            VolumeTrackBar.Minimum = 0;
            VolumeTrackBar.Name = "VolumeTrackBar";
            VolumeTrackBar.Orientation = Orientation.Horizontal;
            VolumeTrackBar.Size = new Size(100, 22);
            VolumeTrackBar.Text = "TrackBar1";
            VolumeTrackBar.TickFrequency = 4;
            VolumeTrackBar.Value = 0;
            VolumeTrackBar.ValueChanged += VolumeTrackBar_ValueChanged;
            // 
            // MainMenu1
            // 
            MainMenu1.Items.AddRange(new ToolStripItem[] { File, Play, Edit, Window, Help1 });
            MainMenu1.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            MainMenu1.Location = new Point(0, 0);
            MainMenu1.Name = "MainMenu1";
            MainMenu1.Size = new Size(611, 24);
            MainMenu1.TabIndex = 0;
            // 
            // File
            // 
            File.DropDownItems.AddRange(new ToolStripItem[] { FileNewItem, NewTurboSound1, NewTurboSound3, JoinTracks, FileOpen, OpenDemoSong, FileClose, FileSave, FileSaveAs, SaveAsTwoModules, SaveAsTemplate, Sep15, ExportWAV, ExportPSG, ExportYM, Exports, Sep10, Options, Sep11, RFile1, RFile2, RFile3, RFile4, RFile5, RFile6, Sep12, FileExit });
            File.Name = "File";
            File.Size = new Size(37, 20);
            File.Text = "File";
            // 
            // FileNewItem
            // 
            FileNewItem.Name = "FileNewItem";
            FileNewItem.Size = new Size(199, 22);
            FileNewItem.Text = "&New";
            FileNewItem.Click += UIAction_Execute;
            // 
            // NewTurboSound1
            // 
            NewTurboSound1.Name = "NewTurboSound1";
            NewTurboSound1.Size = new Size(199, 22);
            NewTurboSound1.Text = "New &Turbo Sound";
            NewTurboSound1.Click += UIAction_Execute;
            // 
            // NewTurboSound3
            // 
            NewTurboSound3.Name = "NewTurboSound3";
            NewTurboSound3.Size = new Size(199, 22);
            NewTurboSound3.Text = "New Turbo Sound 3";
            NewTurboSound3.Click += UIAction_Execute;
            // 
            // JoinTracks
            // 
            JoinTracks.Name = "JoinTracks";
            JoinTracks.Size = new Size(199, 22);
            JoinTracks.Text = "Join Tracks...";
            JoinTracks.Click += UIAction_Execute;
            // 
            // FileOpen
            // 
            FileOpen.Name = "FileOpen";
            FileOpen.Size = new Size(199, 22);
            FileOpen.Text = "&Open";
            FileOpen.Click += UIAction_Execute;
            // 
            // OpenDemoSong
            // 
            OpenDemoSong.Name = "OpenDemoSong";
            OpenDemoSong.Size = new Size(199, 22);
            OpenDemoSong.Text = "Open &Demo Song";
            // 
            // FileClose
            // 
            FileClose.Name = "FileClose";
            FileClose.Size = new Size(199, 22);
            FileClose.Text = "&Close";
            FileClose.Click += UIAction_Execute;
            // 
            // FileSave
            // 
            FileSave.Name = "FileSave";
            FileSave.Size = new Size(199, 22);
            FileSave.Text = "&Save";
            FileSave.Click += UIAction_Execute;
            // 
            // FileSaveAs
            // 
            FileSaveAs.Name = "FileSaveAs";
            FileSaveAs.Size = new Size(199, 22);
            FileSaveAs.Text = "Save &As...";
            FileSaveAs.Click += UIAction_Execute;
            // 
            // SaveAsTwoModules
            // 
            SaveAsTwoModules.Name = "SaveAsTwoModules";
            SaveAsTwoModules.Size = new Size(199, 22);
            SaveAsTwoModules.Text = "Save As 2 Modules...";
            SaveAsTwoModules.Click += UIAction_Execute;
            // 
            // SaveAsTemplate
            // 
            SaveAsTemplate.Name = "SaveAsTemplate";
            SaveAsTemplate.Size = new Size(199, 22);
            SaveAsTemplate.Text = "Set As Startup Template";
            SaveAsTemplate.Click += UIAction_Execute;
            // 
            // Sep15
            // 
            Sep15.Name = "Sep15";
            Sep15.Size = new Size(196, 6);
            // 
            // ExportWAV
            // 
            ExportWAV.Name = "ExportWAV";
            ExportWAV.Size = new Size(199, 22);
            ExportWAV.Text = "Export To WAV...";
            ExportWAV.Click += UIAction_Execute;
            // 
            // ExportPSG
            // 
            ExportPSG.Name = "ExportPSG";
            ExportPSG.Size = new Size(199, 22);
            ExportPSG.Text = "Export PSG...";
            ExportPSG.Click += UIAction_Execute;
            // 
            // ExportYM
            // 
            ExportYM.Name = "ExportYM";
            ExportYM.Size = new Size(199, 22);
            ExportYM.Text = "Export YM...";
            ExportYM.Click += UIAction_Execute;
            // 
            // Exports
            // 
            Exports.DropDownItems.AddRange(new ToolStripItem[] { SaveSNDH, SaveForZX });
            Exports.Name = "Exports";
            Exports.Size = new Size(199, 22);
            Exports.Text = "Exports";
            // 
            // SaveSNDH
            // 
            SaveSNDH.Name = "SaveSNDH";
            SaveSNDH.Size = new Size(232, 22);
            SaveSNDH.Text = "Save In SNDH (Atari ST)";
            SaveSNDH.Click += SaveSNDHMenu_Click;
            // 
            // SaveForZX
            // 
            SaveForZX.Name = "SaveForZX";
            SaveForZX.Size = new Size(232, 22);
            SaveForZX.Text = "Save With ZX Spectrum Player";
            SaveForZX.Click += SaveForZXMenu_Click;
            // 
            // Sep10
            // 
            Sep10.Name = "Sep10";
            Sep10.Size = new Size(196, 6);
            // 
            // Options
            // 
            Options.Name = "Options";
            Options.Size = new Size(199, 22);
            Options.Text = "Options...";
            Options.Click += UIAction_Execute;
            // 
            // Sep11
            // 
            Sep11.Name = "Sep11";
            Sep11.Size = new Size(196, 6);
            // 
            // RFile1
            // 
            RFile1.Name = "RFile1";
            RFile1.Size = new Size(199, 22);
            RFile1.Visible = false;
            RFile1.Click += RFile1Click;
            // 
            // RFile2
            // 
            RFile2.Name = "RFile2";
            RFile2.Size = new Size(199, 22);
            RFile2.Visible = false;
            RFile2.Click += RFile2Click;
            // 
            // RFile3
            // 
            RFile3.Name = "RFile3";
            RFile3.Size = new Size(199, 22);
            RFile3.Visible = false;
            RFile3.Click += RFile3Click;
            // 
            // RFile4
            // 
            RFile4.Name = "RFile4";
            RFile4.Size = new Size(199, 22);
            RFile4.Visible = false;
            RFile4.Click += RFile4Click;
            // 
            // RFile5
            // 
            RFile5.Name = "RFile5";
            RFile5.Size = new Size(199, 22);
            RFile5.Visible = false;
            RFile5.Click += RFile5Click;
            // 
            // RFile6
            // 
            RFile6.Name = "RFile6";
            RFile6.Size = new Size(199, 22);
            RFile6.Visible = false;
            RFile6.Click += RFile6Click;
            // 
            // Sep12
            // 
            Sep12.Name = "Sep12";
            Sep12.Size = new Size(196, 6);
            Sep12.Visible = false;
            // 
            // FileExit
            // 
            FileExit.Name = "FileExit";
            FileExit.Size = new Size(199, 22);
            FileExit.Text = "E&xit";
            FileExit.Click += UIAction_Execute;
            // 
            // Play
            // 
            Play.DropDownItems.AddRange(new ToolStripItem[] { PlayStop, PlayFromLine, PlayFromStart, PlayPatternFromCurrentLine, PlayPatternFromStart, Stop, Sep16, ToggleLooping, ToggleLoopingAll, Sep14, ToggleSamples });
            Play.Name = "Play";
            Play.Size = new Size(41, 20);
            Play.Text = "Play";
            // 
            // PlayStop
            // 
            PlayStop.Name = "PlayStop";
            PlayStop.Size = new Size(195, 22);
            PlayStop.Text = "Play/Stop";
            PlayStop.Click += UIAction_Execute;
            // 
            // PlayFromLine
            // 
            PlayFromLine.Name = "PlayFromLine";
            PlayFromLine.Size = new Size(195, 22);
            PlayFromLine.Text = "Play From Line";
            PlayFromLine.Click += UIAction_Execute;
            // 
            // PlayFromStart
            // 
            PlayFromStart.Name = "PlayFromStart";
            PlayFromStart.Size = new Size(195, 22);
            PlayFromStart.Text = "Play From Start";
            PlayFromStart.Click += UIAction_Execute;
            // 
            // PlayPatternFromCurrentLine
            // 
            PlayPatternFromCurrentLine.Name = "PlayPatternFromCurrentLine";
            PlayPatternFromCurrentLine.Size = new Size(195, 22);
            PlayPatternFromCurrentLine.Text = "Play Pattern";
            PlayPatternFromCurrentLine.Click += UIAction_Execute;
            // 
            // PlayPatternFromStart
            // 
            PlayPatternFromStart.Name = "PlayPatternFromStart";
            PlayPatternFromStart.Size = new Size(195, 22);
            PlayPatternFromStart.Text = "Play Pattern From Start";
            PlayPatternFromStart.Click += UIAction_Execute;
            // 
            // Stop
            // 
            Stop.Name = "Stop";
            Stop.Size = new Size(195, 22);
            Stop.Text = "Stop";
            Stop.Click += UIAction_Execute;
            // 
            // Sep16
            // 
            Sep16.Name = "Sep16";
            Sep16.Size = new Size(192, 6);
            // 
            // ToggleLooping
            // 
            ToggleLooping.Name = "ToggleLooping";
            ToggleLooping.Size = new Size(195, 22);
            ToggleLooping.Text = "Toggle Looping";
            ToggleLooping.Click += UIAction_Execute;
            // 
            // ToggleLoopingAll
            // 
            ToggleLoopingAll.Name = "ToggleLoopingAll";
            ToggleLoopingAll.Size = new Size(195, 22);
            ToggleLoopingAll.Text = "Toggle Looping All";
            ToggleLoopingAll.Click += UIAction_Execute;
            // 
            // Sep14
            // 
            Sep14.Name = "Sep14";
            Sep14.Size = new Size(192, 6);
            // 
            // ToggleSamples
            // 
            ToggleSamples.Name = "ToggleSamples";
            ToggleSamples.Size = new Size(195, 22);
            ToggleSamples.Text = "Toggle Samples";
            ToggleSamples.Click += UIAction_Execute;
            // 
            // Edit
            // 
            Edit.DropDownItems.AddRange(new ToolStripItem[] { Undo1, Redo1, Sep13, EditCut1, EditCopy1, EditPaste1, Sep17, CopyToModPlug1, CopyToRenoise1, CopyToFami1, Sep9, ToggleSamples1, TracksManager, GlobalTransposition, PluginManager });
            Edit.Name = "Edit";
            Edit.Size = new Size(39, 20);
            Edit.Text = "&Edit";
            // 
            // Undo1
            // 
            Undo1.Name = "Undo1";
            Undo1.Size = new Size(185, 22);
            Undo1.Text = "Undo";
            Undo1.Click += UIAction_Execute;
            // 
            // Redo1
            // 
            Redo1.Name = "Redo1";
            Redo1.Size = new Size(185, 22);
            Redo1.Text = "Redo";
            Redo1.Click += UIAction_Execute;
            // 
            // Sep13
            // 
            Sep13.Name = "Sep13";
            Sep13.Size = new Size(182, 6);
            // 
            // EditCut1
            // 
            EditCut1.Name = "EditCut1";
            EditCut1.Size = new Size(185, 22);
            EditCut1.Text = "Cu&t";
            EditCut1.Click += UIAction_Execute;
            // 
            // EditCopy1
            // 
            EditCopy1.Name = "EditCopy1";
            EditCopy1.Size = new Size(185, 22);
            EditCopy1.Text = "&Copy";
            EditCopy1.Click += UIAction_Execute;
            // 
            // EditPaste1
            // 
            EditPaste1.Name = "EditPaste1";
            EditPaste1.Size = new Size(185, 22);
            EditPaste1.Text = "&Paste";
            EditPaste1.Click += UIAction_Execute;
            // 
            // Sep17
            // 
            Sep17.Name = "Sep17";
            Sep17.Size = new Size(182, 6);
            // 
            // CopyToModPlug1
            // 
            CopyToModPlug1.Name = "CopyToModPlug1";
            CopyToModPlug1.Size = new Size(185, 22);
            CopyToModPlug1.Text = "Copy To OpenMPT";
            CopyToModPlug1.Click += UIAction_Execute;
            // 
            // CopyToRenoise1
            // 
            CopyToRenoise1.Name = "CopyToRenoise1";
            CopyToRenoise1.Size = new Size(185, 22);
            CopyToRenoise1.Text = "Copy To Renoise";
            CopyToRenoise1.Click += UIAction_Execute;
            // 
            // CopyToFami1
            // 
            CopyToFami1.Name = "CopyToFami1";
            CopyToFami1.Size = new Size(185, 22);
            CopyToFami1.Text = "Copy To FamiTracker";
            CopyToFami1.Click += UIAction_Execute;
            // 
            // Sep9
            // 
            Sep9.Name = "Sep9";
            Sep9.Size = new Size(182, 6);
            // 
            // ToggleSamples1
            // 
            ToggleSamples1.Name = "ToggleSamples1";
            ToggleSamples1.Size = new Size(185, 22);
            ToggleSamples1.Text = "Toggle Samples";
            ToggleSamples1.Click += UIAction_Execute;
            // 
            // TracksManager
            // 
            TracksManager.Name = "TracksManager";
            TracksManager.Size = new Size(185, 22);
            TracksManager.Text = "Tracks Manager";
            TracksManager.Click += UIAction_Execute;
            // 
            // GlobalTransposition
            // 
            GlobalTransposition.Name = "GlobalTransposition";
            GlobalTransposition.Size = new Size(185, 22);
            GlobalTransposition.Text = "Global Transposition";
            GlobalTransposition.Click += UIAction_Execute;
            // 
            // PluginManager
            // 
            PluginManager.Name = "PluginManager";
            PluginManager.Size = new Size(185, 22);
            PluginManager.Text = "Plugin Manager";
            PluginManager.Click += UIAction_Execute;
            // 
            // Window
            // 
            Window.DropDownItems.AddRange(new ToolStripItem[] { Maximize, Normal });
            Window.Name = "Window";
            Window.Size = new Size(63, 20);
            Window.Text = "&Window";
            // 
            // Maximize
            // 
            Maximize.Name = "Maximize";
            Maximize.Size = new Size(124, 22);
            Maximize.Text = "&Maximize";
            Maximize.Click += UIAction_Execute;
            // 
            // Normal
            // 
            Normal.Name = "Normal";
            Normal.Size = new Size(124, 22);
            Normal.Text = "&Normal";
            Normal.Click += UIAction_Execute;
            // 
            // Help1
            // 
            Help1.DropDownItems.AddRange(new ToolStripItem[] { HelpAbout });
            Help1.Name = "Help1";
            Help1.Size = new Size(44, 20);
            Help1.Text = "&Help";
            // 
            // HelpAbout
            // 
            HelpAbout.Name = "HelpAbout";
            HelpAbout.Size = new Size(116, 22);
            HelpAbout.Text = "About...";
            HelpAbout.Click += UIAction_Execute;
            // 
            // OpenDialog
            // 
            OpenDialog.DefaultExt = "PT3";
            OpenDialog.Filter = resources.GetString("OpenDialog.Filter");
            OpenDialog.Title = "Open Module(s):";
            // 
            // SaveDialog1
            // 
            SaveDialog1.DefaultExt = "txt";
            SaveDialog1.Filter = "Vortex Tracker (.vt2)|*.vt2|Pro Tracker 3 (*.pt3)|*.pt3";
            // 
            // PopupMenu1
            // 
            PopupMenu1.Items.AddRange(new ToolStripItem[] { Color1, ResetColors, Sep2, SetLoopPosition, Sep1, InsertPosition, DeletePosition, DuplicatePosition, ClonePosition, Sep3, ChangePatternsLength1, Sep4, RenumberPatterns, AutoNumeratePatterns, CleanSelectedPatterns });
            PopupMenu1.Name = "PopupMenu1";
            PopupMenu1.Size = new Size(211, 270);
            // 
            // Color1
            // 
            Color1.DropDownItems.AddRange(new ToolStripItem[] { PositionColorL5, PositionColorL4, PositionColorL3, PositionColorL2, PositionColorL1, PositionColorDefault, PositionColorRed, PositionColorGreen, PositionColorBlue, PositionColorMaroon, PositionColorPurple, PositionColorGray, PositionColorTeal });
            Color1.Name = "Color1";
            Color1.Size = new Size(210, 22);
            Color1.Text = "Set Color";
            // 
            // PositionColorL5
            // 
            PositionColorL5.Name = "PositionColorL5";
            PositionColorL5.Size = new Size(116, 22);
            PositionColorL5.Text = "Light5";
            PositionColorL5.Click += PositionColorL5_Click;
            PositionColorL5.Paint += PositionColorL5_Paint;
            // 
            // PositionColorL4
            // 
            PositionColorL4.Name = "PositionColorL4";
            PositionColorL4.Size = new Size(116, 22);
            PositionColorL4.Text = "Light4";
            PositionColorL4.Click += PositionColorL4_Click;
            PositionColorL4.Paint += PositionColorL4_Paint;
            // 
            // PositionColorL3
            // 
            PositionColorL3.Name = "PositionColorL3";
            PositionColorL3.Size = new Size(116, 22);
            PositionColorL3.Text = "Light3";
            PositionColorL3.Click += PositionColorL3_Click;
            PositionColorL3.Paint += PositionColorL3_Paint;
            // 
            // PositionColorL2
            // 
            PositionColorL2.Name = "PositionColorL2";
            PositionColorL2.Size = new Size(116, 22);
            PositionColorL2.Text = "Light2";
            PositionColorL2.Click += PositionColorL2_Click;
            PositionColorL2.Paint += PositionColorL2_Paint;
            // 
            // PositionColorL1
            // 
            PositionColorL1.Name = "PositionColorL1";
            PositionColorL1.Size = new Size(116, 22);
            PositionColorL1.Text = "Light1";
            PositionColorL1.Click += PositionColorL1_Click;
            PositionColorL1.Paint += PositionColorL1_Paint;
            // 
            // PositionColorDefault
            // 
            PositionColorDefault.Name = "PositionColorDefault";
            PositionColorDefault.Size = new Size(116, 22);
            PositionColorDefault.Text = "Default";
            PositionColorDefault.Click += PositionColorDefault_Click;
            PositionColorDefault.Paint += PositionColorDefault_Paint;
            // 
            // PositionColorRed
            // 
            PositionColorRed.Name = "PositionColorRed";
            PositionColorRed.Size = new Size(116, 22);
            PositionColorRed.Text = "Red";
            PositionColorRed.Click += PositionColorRed_Click;
            PositionColorRed.Paint += PositionColorRed_Paint;
            // 
            // PositionColorGreen
            // 
            PositionColorGreen.Name = "PositionColorGreen";
            PositionColorGreen.Size = new Size(116, 22);
            PositionColorGreen.Text = "Green";
            PositionColorGreen.Click += PositionColorGreen_Click;
            PositionColorGreen.Paint += PositionColorGreen_Paint;
            // 
            // PositionColorBlue
            // 
            PositionColorBlue.Name = "PositionColorBlue";
            PositionColorBlue.Size = new Size(116, 22);
            PositionColorBlue.Text = "Blue";
            PositionColorBlue.Click += PositionColorBlue_Click;
            PositionColorBlue.Paint += PositionColorBlue_Paint;
            // 
            // PositionColorMaroon
            // 
            PositionColorMaroon.Name = "PositionColorMaroon";
            PositionColorMaroon.Size = new Size(116, 22);
            PositionColorMaroon.Text = "Maroon";
            PositionColorMaroon.Click += PositionColorMaroon_Click;
            PositionColorMaroon.Paint += PositionColorMaroon_Paint;
            // 
            // PositionColorPurple
            // 
            PositionColorPurple.Name = "PositionColorPurple";
            PositionColorPurple.Size = new Size(116, 22);
            PositionColorPurple.Text = "Purple";
            PositionColorPurple.Click += PositionColorPurple_Click;
            PositionColorPurple.Paint += PositionColorPurple_Paint;
            // 
            // PositionColorGray
            // 
            PositionColorGray.Name = "PositionColorGray";
            PositionColorGray.Size = new Size(116, 22);
            PositionColorGray.Text = "Gray";
            PositionColorGray.Click += PositionColorGray_Click;
            PositionColorGray.Paint += PositionColorGray_Paint;
            // 
            // PositionColorTeal
            // 
            PositionColorTeal.Name = "PositionColorTeal";
            PositionColorTeal.Size = new Size(116, 22);
            PositionColorTeal.Text = "Teal";
            PositionColorTeal.Click += PositionColorTeal_Click;
            PositionColorTeal.Paint += PositionColorTeal_Paint;
            // 
            // ResetColors
            // 
            ResetColors.Name = "ResetColors";
            ResetColors.Size = new Size(210, 22);
            ResetColors.Text = "Reset Color";
            ResetColors.Click += ResetColors_Click;
            // 
            // Sep2
            // 
            Sep2.Name = "Sep2";
            Sep2.Size = new Size(207, 6);
            // 
            // SetLoopPosition
            // 
            SetLoopPosition.Name = "SetLoopPosition";
            SetLoopPosition.Size = new Size(210, 22);
            SetLoopPosition.Text = "Set Loop Position";
            SetLoopPosition.Click += UIAction_Execute;
            // 
            // Sep1
            // 
            Sep1.Name = "Sep1";
            Sep1.Size = new Size(207, 6);
            // 
            // InsertPosition
            // 
            InsertPosition.Name = "InsertPosition";
            InsertPosition.ShortcutKeys = Keys.Insert;
            InsertPosition.Size = new Size(210, 22);
            InsertPosition.Text = "Insert Position";
            InsertPosition.Click += UIAction_Execute;
            // 
            // DeletePosition
            // 
            DeletePosition.Name = "DeletePosition";
            DeletePosition.ShortcutKeys = Keys.Delete;
            DeletePosition.Size = new Size(210, 22);
            DeletePosition.Text = "Delete Position";
            DeletePosition.Click += UIAction_Execute;
            // 
            // DuplicatePosition
            // 
            DuplicatePosition.Name = "DuplicatePosition";
            DuplicatePosition.Size = new Size(210, 22);
            DuplicatePosition.Text = "Duplicate Position";
            DuplicatePosition.Click += UIAction_Execute;
            // 
            // ClonePosition
            // 
            ClonePosition.Name = "ClonePosition";
            ClonePosition.Size = new Size(210, 22);
            ClonePosition.Text = "Clone Position";
            ClonePosition.Click += UIAction_Execute;
            // 
            // Sep3
            // 
            Sep3.Name = "Sep3";
            Sep3.Size = new Size(207, 6);
            // 
            // ChangePatternsLength1
            // 
            ChangePatternsLength1.Name = "ChangePatternsLength1";
            ChangePatternsLength1.Size = new Size(210, 22);
            ChangePatternsLength1.Text = "Change Patterns Length...";
            ChangePatternsLength1.Click += ChangePatternsLength1_Click;
            // 
            // Sep4
            // 
            Sep4.Name = "Sep4";
            Sep4.Size = new Size(207, 6);
            // 
            // RenumberPatterns
            // 
            RenumberPatterns.Name = "RenumberPatterns";
            RenumberPatterns.Size = new Size(210, 22);
            RenumberPatterns.Text = "Renumber Patterns";
            RenumberPatterns.Click += RenumberPatterns_Click;
            // 
            // AutoNumeratePatterns
            // 
            AutoNumeratePatterns.Name = "AutoNumeratePatterns";
            AutoNumeratePatterns.Size = new Size(210, 22);
            AutoNumeratePatterns.Text = "Fill Empty Positions";
            AutoNumeratePatterns.Click += AutoNumeratePatternsClick;
            // 
            // CleanSelectedPatterns
            // 
            CleanSelectedPatterns.Name = "CleanSelectedPatterns";
            CleanSelectedPatterns.Size = new Size(210, 22);
            CleanSelectedPatterns.Text = "Clean Selected Pattern(s)";
            CleanSelectedPatterns.Click += CleanSelectedPatterns_Click;
            // 
            // SaveDialogSNDH
            // 
            SaveDialogSNDH.Filter = "Atari ST'#39's SNDH files (SND, SNDH)|*.snd;*.sndh|Any file|*.*";
            // 
            // SaveDialogZXAY
            // 
            SaveDialogZXAY.Filter = "Hobeta With Player ($c)|*.$c|Hobeta without player ($m)|*.$m|.AY-files (AY)|*.ay|SCL-files (SCL)|*.scl|ZX tape files (TAP)|*.tap|Any file|*.*'";
            // 
            // PopupMenu2
            // 
            PopupMenu2.Items.AddRange(new ToolStripItem[] { Undo2, Redo2, Sep5, EditCopy2, EditCut2, EditPaste2, Merge1, N20, CopyTo, Sep6, TransposeUp1, TransposeDown1, TransposeUp3, TransposeDown3, TransposeUp5, TransposeDown5, TransposeUp12, TransposeDown12, Sep7, ExpandPattern, CompressPattern, SplitPattern, PatternPacker, Sep8, SwapChannelsLeft, SwapChannelsRight });
            PopupMenu2.Name = "PopupMenu2";
            PopupMenu2.Size = new Size(186, 496);
            // 
            // Undo2
            // 
            Undo2.Name = "Undo2";
            Undo2.Size = new Size(185, 22);
            Undo2.Text = "Undo";
            Undo2.Click += UIAction_Execute;
            // 
            // Redo2
            // 
            Redo2.Name = "Redo2";
            Redo2.Size = new Size(185, 22);
            Redo2.Text = "Redo";
            Redo2.Click += UIAction_Execute;
            // 
            // Sep5
            // 
            Sep5.Name = "Sep5";
            Sep5.Size = new Size(182, 6);
            // 
            // EditCopy2
            // 
            EditCopy2.Name = "EditCopy2";
            EditCopy2.Size = new Size(185, 22);
            EditCopy2.Text = "Copy";
            EditCopy2.Click += UIAction_Execute;
            // 
            // EditCut2
            // 
            EditCut2.Name = "EditCut2";
            EditCut2.Size = new Size(185, 22);
            EditCut2.Text = "Cut";
            EditCut2.Click += UIAction_Execute;
            // 
            // EditPaste2
            // 
            EditPaste2.Name = "EditPaste2";
            EditPaste2.Size = new Size(185, 22);
            EditPaste2.Text = "Paste";
            EditPaste2.Click += UIAction_Execute;
            // 
            // Merge1
            // 
            Merge1.Name = "Merge1";
            Merge1.Size = new Size(185, 22);
            Merge1.Text = "Merge";
            Merge1.Click += UIAction_Execute;
            // 
            // N20
            // 
            N20.Name = "N20";
            N20.Size = new Size(182, 6);
            // 
            // CopyTo
            // 
            CopyTo.DropDownItems.AddRange(new ToolStripItem[] { CopyToModPlug2, CopyToRenoise2, CopyToFami2 });
            CopyTo.Name = "CopyTo";
            CopyTo.Size = new Size(185, 22);
            CopyTo.Text = "Copy To...";
            // 
            // CopyToModPlug2
            // 
            CopyToModPlug2.Name = "CopyToModPlug2";
            CopyToModPlug2.Size = new Size(185, 22);
            CopyToModPlug2.Text = "Copy To OpenMPT";
            CopyToModPlug2.Click += UIAction_Execute;
            // 
            // CopyToRenoise2
            // 
            CopyToRenoise2.Name = "CopyToRenoise2";
            CopyToRenoise2.Size = new Size(185, 22);
            CopyToRenoise2.Text = "Copy To Renoise";
            CopyToRenoise2.Click += UIAction_Execute;
            // 
            // CopyToFami2
            // 
            CopyToFami2.Name = "CopyToFami2";
            CopyToFami2.Size = new Size(185, 22);
            CopyToFami2.Text = "Copy To FamiTracker";
            CopyToFami2.Click += UIAction_Execute;
            // 
            // Sep6
            // 
            Sep6.Name = "Sep6";
            Sep6.Size = new Size(182, 6);
            // 
            // TransposeUp1
            // 
            TransposeUp1.Name = "TransposeUp1";
            TransposeUp1.Size = new Size(185, 22);
            TransposeUp1.Text = "Transpose +1";
            TransposeUp1.Click += UIAction_Execute;
            // 
            // TransposeDown1
            // 
            TransposeDown1.Name = "TransposeDown1";
            TransposeDown1.Size = new Size(185, 22);
            TransposeDown1.Text = "Transpose -1";
            TransposeDown1.Click += UIAction_Execute;
            // 
            // TransposeUp3
            // 
            TransposeUp3.Name = "TransposeUp3";
            TransposeUp3.Size = new Size(185, 22);
            TransposeUp3.Text = "Transpose +3";
            TransposeUp3.Click += UIAction_Execute;
            // 
            // TransposeDown3
            // 
            TransposeDown3.Name = "TransposeDown3";
            TransposeDown3.Size = new Size(185, 22);
            TransposeDown3.Text = "Transpose -3";
            TransposeDown3.Click += UIAction_Execute;
            // 
            // TransposeUp5
            // 
            TransposeUp5.Name = "TransposeUp5";
            TransposeUp5.Size = new Size(185, 22);
            TransposeUp5.Text = "Tranpose +5";
            TransposeUp5.Click += UIAction_Execute;
            // 
            // TransposeDown5
            // 
            TransposeDown5.Name = "TransposeDown5";
            TransposeDown5.Size = new Size(185, 22);
            TransposeDown5.Text = "Transpose -5";
            TransposeDown5.Click += UIAction_Execute;
            // 
            // TransposeUp12
            // 
            TransposeUp12.Name = "TransposeUp12";
            TransposeUp12.Size = new Size(185, 22);
            TransposeUp12.Text = "Transpose +12";
            TransposeUp12.Click += UIAction_Execute;
            // 
            // TransposeDown12
            // 
            TransposeDown12.Name = "TransposeDown12";
            TransposeDown12.Size = new Size(185, 22);
            TransposeDown12.Text = "Transpose -12";
            TransposeDown12.Click += UIAction_Execute;
            // 
            // Sep7
            // 
            Sep7.Name = "Sep7";
            Sep7.Size = new Size(182, 6);
            // 
            // ExpandPattern
            // 
            ExpandPattern.Name = "ExpandPattern";
            ExpandPattern.Size = new Size(185, 22);
            ExpandPattern.Text = "Expand Pattern";
            ExpandPattern.Click += UIAction_Execute;
            // 
            // CompressPattern
            // 
            CompressPattern.Name = "CompressPattern";
            CompressPattern.Size = new Size(185, 22);
            CompressPattern.Text = "Shrink Pattern";
            CompressPattern.Click += UIAction_Execute;
            // 
            // SplitPattern
            // 
            SplitPattern.Name = "SplitPattern";
            SplitPattern.Size = new Size(185, 22);
            SplitPattern.Text = "Split Pattern";
            SplitPattern.Click += UIAction_Execute;
            // 
            // PatternPacker
            // 
            PatternPacker.Name = "PatternPacker";
            PatternPacker.Size = new Size(185, 22);
            PatternPacker.Text = "Pattern Packer";
            PatternPacker.Click += UIAction_Execute;
            // 
            // Sep8
            // 
            Sep8.Name = "Sep8";
            Sep8.Size = new Size(182, 6);
            // 
            // SwapChannelsLeft
            // 
            SwapChannelsLeft.Name = "SwapChannelsLeft";
            SwapChannelsLeft.Size = new Size(185, 22);
            SwapChannelsLeft.Text = "Swap Channels Left";
            SwapChannelsLeft.Click += UIAction_Execute;
            // 
            // SwapChannelsRight
            // 
            SwapChannelsRight.Name = "SwapChannelsRight";
            SwapChannelsRight.Size = new Size(185, 22);
            SwapChannelsRight.Text = "Swap Channels Right";
            SwapChannelsRight.Click += UIAction_Execute;
            // 
            // PopupMenu3
            // 
            PopupMenu3.Items.AddRange(new ToolStripItem[] { File2, Play5, TrackTools });
            PopupMenu3.Name = "PopupMenu3";
            PopupMenu3.Size = new Size(134, 70);
            // 
            // File2
            // 
            File2.Checked = true;
            File2.CheckState = CheckState.Checked;
            File2.Name = "File2";
            File2.Size = new Size(133, 22);
            File2.Text = "File";
            File2.Click += TrackTools_Click;
            // 
            // Play5
            // 
            Play5.Name = "Play5";
            Play5.Size = new Size(133, 22);
            Play5.Tag = 1;
            Play5.Text = "Play";
            Play5.Click += TrackTools_Click;
            // 
            // TrackTools
            // 
            TrackTools.Checked = true;
            TrackTools.CheckState = CheckState.Checked;
            TrackTools.Name = "TrackTools";
            TrackTools.Size = new Size(133, 22);
            TrackTools.Tag = 2;
            TrackTools.Text = "Track Tools";
            TrackTools.Click += TrackTools_Click;
            // 
            // BackupTimer
            // 
            BackupTimer.Enabled = true;
            BackupTimer.Interval = 1000;
            BackupTimer.Tick += SaveBackups;
            // 
            // SyncCheckTimer
            // 
            SyncCheckTimer.Interval = 500;
            SyncCheckTimer.Tick += SyncCheckTimer_Tick;
            // 
            // SyncFinishTimer
            // 
            SyncFinishTimer.Interval = 800;
            SyncFinishTimer.Tick += SyncFinishTimerTimer;
            // 
            // SyncCopyBuffers
            // 
            SyncCopyBuffers.Enabled = true;
            SyncCopyBuffers.Interval = 1000;
            SyncCopyBuffers.Tick += SyncCopyBuffersTimer;
            // 
            // MIDITimer
            // 
            MIDITimer.Enabled = true;
            MIDITimer.Interval = 1000;
            MIDITimer.Tick += MIDITimer_Tick;
            // 
            // MainForm
            // 
            AllowDrop = true;
            ClientSize = new Size(611, 579);
            Controls.Add(ToolStrip1);
            Controls.Add(StatusBar);
            Controls.Add(MainMenu1);
            Font = new Font("Microsoft Sans Serif", 8.25F);
            ForeColor = Color.Black;
            Icon = (Icon)resources.GetObject("$this.Icon");
            IsMdiContainer = true;
            KeyPreview = true;
            Location = new Point(403, 138);
            MainMenuStrip = MainMenu1;
            MinimumSize = new Size(0, 440);
            Name = "MainForm";
            Text = " ";
            FormClosing += MainForm_FormClosing;
            FormClosed += MainForm_FormClosed;
            Load += MainForm_Load;
            MdiChildActivate += MainForm_MdiChildActivate;
            Shown += MainForm_Shown;
            DragDrop += MainForm_DragDrop;
            DragEnter += MainForm_DragEnter;
            DoubleClick += MainForm_DoubleClick;
            KeyDown += MainForm_KeyDown;
            Resize += MainForm_Resize;
            StatusBar.ResumeLayout(false);
            StatusBar.PerformLayout();
            ToolStrip1.ResumeLayout(false);
            ToolStrip1.PerformLayout();
            MainMenu1.ResumeLayout(false);
            MainMenu1.PerformLayout();
            PopupMenu1.ResumeLayout(false);
            PopupMenu2.ResumeLayout(false);
            PopupMenu3.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        public System.Windows.Forms.StatusStrip StatusBar;
        public System.Windows.Forms.ToolStripStatusLabel StatusLabel0;
        public System.Windows.Forms.ToolStripStatusLabel StatusLabel1;
        public System.Windows.Forms.ToolStripStatusLabel StatusLabel2;
        public System.Windows.Forms.ToolStripStatusLabel StatusLabel3;
        public MyToolStrip ToolStrip1;
        public System.Windows.Forms.ToolStripButton NewButton;
        public System.Windows.Forms.ToolStripButton FileOpenButton;
        public System.Windows.Forms.ToolStripButton FileSaveButton;
        public System.Windows.Forms.ToolStripSeparator ToolButton3;
        public System.Windows.Forms.ToolStripButton UndoButton;
        public System.Windows.Forms.ToolStripButton RedoButton;
        public System.Windows.Forms.ToolStripSeparator ToolButton8;
        public System.Windows.Forms.ToolStripButton PlayStopButton;
        public System.Windows.Forms.ToolStripButton PlayFromStartButton;
        public System.Windows.Forms.ToolStripButton PlayPatternFromCurrentLineButton;
        public System.Windows.Forms.ToolStripButton PlayPatternFromStartButton;
        public System.Windows.Forms.ToolStripButton ToggleLoopingButton;
        public System.Windows.Forms.ToolStripButton LoopAllFilesButton;
        public System.Windows.Forms.ToolStripSeparator ToolButton4;
        public System.Windows.Forms.ToolStripButton ToggleSamplesButton;
        public System.Windows.Forms.ToolStripButton TracksManagerButton;
        public System.Windows.Forms.ToolStripButton GlobalTranspositionButton;
        public System.Windows.Forms.ToolStripSeparator ToolButton5;
        public System.Windows.Forms.ToolStripButton ChipButton;
        public System.Windows.Forms.ToolStripButton ChannelsButton;
        public System.Windows.Forms.ToolStripButton OptionsButton;
        public ToolStripTrackBar VolumeTrackBar;
        public System.Windows.Forms.MenuStrip MainMenu1;
        public System.Windows.Forms.ToolStripMenuItem File;
        public System.Windows.Forms.ToolStripMenuItem FileNewItem;
        public System.Windows.Forms.ToolStripMenuItem NewTurboSound1;
        public System.Windows.Forms.ToolStripMenuItem NewTurboSound3;
        public System.Windows.Forms.ToolStripMenuItem JoinTracks;
        public System.Windows.Forms.ToolStripMenuItem FileOpen;
        public System.Windows.Forms.ToolStripMenuItem OpenDemoSong;
        public System.Windows.Forms.ToolStripMenuItem FileClose;
        public System.Windows.Forms.ToolStripMenuItem FileSave;
        public System.Windows.Forms.ToolStripMenuItem FileSaveAs;
        public System.Windows.Forms.ToolStripMenuItem SaveAsTwoModules;
        public System.Windows.Forms.ToolStripMenuItem SaveAsTemplate;
        public System.Windows.Forms.ToolStripSeparator Sep15;
        public System.Windows.Forms.ToolStripMenuItem ExportWAV;
        public System.Windows.Forms.ToolStripMenuItem ExportPSG;
        public System.Windows.Forms.ToolStripMenuItem ExportYM;
        public System.Windows.Forms.ToolStripMenuItem Exports;
        public System.Windows.Forms.ToolStripMenuItem SaveSNDH;
        public System.Windows.Forms.ToolStripMenuItem SaveForZX;
        public System.Windows.Forms.ToolStripSeparator Sep10;
        public System.Windows.Forms.ToolStripMenuItem Options;
        public System.Windows.Forms.ToolStripSeparator Sep11;
        public System.Windows.Forms.ToolStripMenuItem RFile1;
        public System.Windows.Forms.ToolStripMenuItem RFile2;
        public System.Windows.Forms.ToolStripMenuItem RFile3;
        public System.Windows.Forms.ToolStripMenuItem RFile4;
        public System.Windows.Forms.ToolStripMenuItem RFile5;
        public System.Windows.Forms.ToolStripMenuItem RFile6;
        public System.Windows.Forms.ToolStripSeparator Sep12;
        public System.Windows.Forms.ToolStripMenuItem FileExit;
        public System.Windows.Forms.ToolStripMenuItem Play;
        public System.Windows.Forms.ToolStripMenuItem PlayStop;
        public System.Windows.Forms.ToolStripMenuItem PlayFromLine;
        public System.Windows.Forms.ToolStripMenuItem PlayFromStart;
        public System.Windows.Forms.ToolStripMenuItem PlayPatternFromCurrentLine;
        public System.Windows.Forms.ToolStripMenuItem PlayPatternFromStart;
        public System.Windows.Forms.ToolStripMenuItem Stop;
        public System.Windows.Forms.ToolStripSeparator Sep16;
        public System.Windows.Forms.ToolStripMenuItem ToggleLooping;
        public System.Windows.Forms.ToolStripMenuItem ToggleLoopingAll;
        public System.Windows.Forms.ToolStripSeparator Sep14;
        public System.Windows.Forms.ToolStripMenuItem ToggleSamples;
        public System.Windows.Forms.ToolStripMenuItem Edit;
        public System.Windows.Forms.ToolStripMenuItem Undo1;
        public System.Windows.Forms.ToolStripMenuItem Redo1;
        public System.Windows.Forms.ToolStripSeparator Sep13;
        public System.Windows.Forms.ToolStripMenuItem EditCut1;
        public System.Windows.Forms.ToolStripMenuItem EditCopy1;
        public System.Windows.Forms.ToolStripMenuItem EditPaste1;
        public System.Windows.Forms.ToolStripSeparator Sep17;
        public System.Windows.Forms.ToolStripMenuItem CopyToModPlug1;
        public System.Windows.Forms.ToolStripMenuItem CopyToRenoise1;
        public System.Windows.Forms.ToolStripMenuItem CopyToFami1;
        public System.Windows.Forms.ToolStripSeparator Sep9;
        public System.Windows.Forms.ToolStripMenuItem TracksManager;
        public System.Windows.Forms.ToolStripMenuItem GlobalTransposition;
        public System.Windows.Forms.ToolStripMenuItem Window;
        public System.Windows.Forms.ToolStripMenuItem Maximize;
        public System.Windows.Forms.ToolStripMenuItem Normal;
        public System.Windows.Forms.ToolStripMenuItem Help1;
        public System.Windows.Forms.ToolStripMenuItem HelpAbout;
        public System.Windows.Forms.OpenFileDialog OpenDialog;
        public System.Windows.Forms.ImageList ImageList1;
        public System.Windows.Forms.SaveFileDialog SaveDialog1;
        public System.Windows.Forms.ContextMenuStrip PopupMenu1;
        public System.Windows.Forms.ToolStripMenuItem Color1;
        public System.Windows.Forms.ToolStripMenuItem PositionColorL5;
        public System.Windows.Forms.ToolStripMenuItem PositionColorL4;
        public System.Windows.Forms.ToolStripMenuItem PositionColorL3;
        public System.Windows.Forms.ToolStripMenuItem PositionColorL2;
        public System.Windows.Forms.ToolStripMenuItem PositionColorL1;
        public System.Windows.Forms.ToolStripMenuItem PositionColorDefault;
        public System.Windows.Forms.ToolStripMenuItem PositionColorRed;
        public System.Windows.Forms.ToolStripMenuItem PositionColorGreen;
        public System.Windows.Forms.ToolStripMenuItem PositionColorBlue;
        public System.Windows.Forms.ToolStripMenuItem PositionColorMaroon;
        public System.Windows.Forms.ToolStripMenuItem PositionColorPurple;
        public System.Windows.Forms.ToolStripMenuItem PositionColorGray;
        public System.Windows.Forms.ToolStripMenuItem PositionColorTeal;
        public System.Windows.Forms.ToolStripMenuItem ResetColors;
        public System.Windows.Forms.ToolStripSeparator Sep2;
        public System.Windows.Forms.ToolStripMenuItem SetLoopPosition;
        public System.Windows.Forms.ToolStripSeparator Sep1;
        public System.Windows.Forms.ToolStripMenuItem InsertPosition;
        public System.Windows.Forms.ToolStripMenuItem DeletePosition;
        public System.Windows.Forms.ToolStripMenuItem DuplicatePosition;
        public System.Windows.Forms.ToolStripMenuItem ClonePosition;
        public System.Windows.Forms.ToolStripSeparator Sep3;
        public System.Windows.Forms.ToolStripMenuItem ChangePatternsLength1;
        public System.Windows.Forms.ToolStripSeparator Sep4;
        public System.Windows.Forms.ToolStripMenuItem RenumberPatterns;
        public System.Windows.Forms.ToolStripMenuItem AutoNumeratePatterns;
        public System.Windows.Forms.SaveFileDialog SaveDialogSNDH;
        public System.Windows.Forms.SaveFileDialog SaveDialogZXAY;
        public System.Windows.Forms.ContextMenuStrip PopupMenu2;
        public System.Windows.Forms.ToolStripMenuItem Undo2;
        public System.Windows.Forms.ToolStripMenuItem Redo2;
        public System.Windows.Forms.ToolStripSeparator Sep5;
        public System.Windows.Forms.ToolStripMenuItem EditCopy2;
        public System.Windows.Forms.ToolStripMenuItem EditCut2;
        public System.Windows.Forms.ToolStripMenuItem EditPaste2;
        public System.Windows.Forms.ToolStripMenuItem Merge1;
        public System.Windows.Forms.ToolStripSeparator N20;
        public System.Windows.Forms.ToolStripMenuItem CopyTo;
        public System.Windows.Forms.ToolStripMenuItem CopyToModPlug2;
        public System.Windows.Forms.ToolStripMenuItem CopyToRenoise2;
        public System.Windows.Forms.ToolStripMenuItem CopyToFami2;
        public System.Windows.Forms.ToolStripSeparator Sep6;
        public System.Windows.Forms.ToolStripMenuItem TransposeUp1;
        public System.Windows.Forms.ToolStripMenuItem TransposeDown1;
        public System.Windows.Forms.ToolStripMenuItem TransposeUp3;
        public System.Windows.Forms.ToolStripMenuItem TransposeDown3;
        public System.Windows.Forms.ToolStripMenuItem TransposeUp5;
        public System.Windows.Forms.ToolStripMenuItem TransposeDown5;
        public System.Windows.Forms.ToolStripMenuItem TransposeUp12;
        public System.Windows.Forms.ToolStripMenuItem TransposeDown12;
        public System.Windows.Forms.ToolStripSeparator Sep7;
        public System.Windows.Forms.ToolStripMenuItem ExpandPattern;
        public System.Windows.Forms.ToolStripMenuItem CompressPattern;
        public System.Windows.Forms.ToolStripMenuItem SplitPattern;
        public System.Windows.Forms.ToolStripMenuItem PatternPacker;
        public System.Windows.Forms.ToolStripSeparator Sep8;
        public System.Windows.Forms.ToolStripMenuItem SwapChannelsLeft;
        public System.Windows.Forms.ToolStripMenuItem SwapChannelsRight;
        public System.Windows.Forms.ContextMenuStrip PopupMenu3;
        public System.Windows.Forms.ToolStripMenuItem File2;
        public System.Windows.Forms.ToolStripMenuItem Play5;
        public System.Windows.Forms.ToolStripMenuItem TrackTools;
        public System.Windows.Forms.Timer BackupTimer;
        public System.Windows.Forms.Timer SyncCheckTimer;
        public System.Windows.Forms.Timer SyncFinishTimer;
        public System.Windows.Forms.Timer SyncCopyBuffers;
        public System.Windows.Forms.Timer MIDITimer;
        private System.Windows.Forms.ToolTip toolTip1 = null;
        private ToolStripMenuItem CleanSelectedPatterns;
        private ToolStripButton PluginManagerButton;
        private ToolStripMenuItem ToggleSamples1;
        private ToolStripMenuItem PluginManager;
    }
}
