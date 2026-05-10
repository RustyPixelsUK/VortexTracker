using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Threading;
using System.Windows.Forms;
using VortexTracker.Controls;

namespace VortexTracker
{
    partial class ChildForm
    {
        private System.ComponentModel.IContainer components;

        // Clean up any resources being used.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChildForm));
            ToolTip = new ToolTip(components);
            TopBackgroundPanel = new Panel();
            TabControl = new MyTabControl();
            PatternsTab = new TabPage();
            PositionsGrid = new MyDataGridView();
            InterfaceBox = new GroupBox();
            EnvelopeAsNoteCheckBox = new CheckBox();
            UseLastNoteParamsCheckBox = new CheckBox();
            MoveBetweenPatternsCheckBox = new CheckBox();
            AutoHLBox = new GroupBox();
            AutoLL = new Button();
            AutoHLCheckBox = new CheckBox();
            StepHLUpDown = new MyNumericUpDown();
            ChannelABox = new GroupBox();
            ChannelAMute = new CheckBox();
            ChannelATone = new CheckBox();
            ChannelANoise = new CheckBox();
            ChannelAEnvelope = new CheckBox();
            ChannelASolo = new CheckBox();
            ChannelBBox = new GroupBox();
            ChannelBMute = new CheckBox();
            ChannelBTone = new CheckBox();
            ChannelBNoise = new CheckBox();
            ChannelBEnvelope = new CheckBox();
            ChannelBSolo = new CheckBox();
            ChannelCBox = new GroupBox();
            ChannelCMute = new CheckBox();
            ChannelCTone = new CheckBox();
            ChannelCNoise = new CheckBox();
            ChannelCEnvelope = new CheckBox();
            ChannelCSolo = new CheckBox();
            PatternBox = new GroupBox();
            PatternNumberLabel = new Label();
            PatternLengthLabel = new Label();
            LoadPatternButton = new Button();
            SavePatternButton = new Button();
            PatternNumUpDown = new MyNumericUpDown();
            PatternLenUpDown = new MyNumericUpDown();
            SpeedBox = new GroupBox();
            SpeedBPMLabel = new Label();
            SpeedBpmUpDown = new MyNumericUpDown();
            OctaveBox = new GroupBox();
            OctaveUpDown = new MyNumericUpDown();
            OctaveLabel = new Label();
            AutoStepBox = new GroupBox();
            AutoStepUpDown = new MyNumericUpDown();
            AutoStepButton = new CheckBox();
            AutoEnvBox = new GroupBox();
            AutoEnvButton = new CheckBox();
            AutoEnv0Button = new Button();
            AutoEnvToggleButton = new Button();
            AutoEnv1Button = new Button();
            ToneTableBox = new GroupBox();
            ToneTableLabel = new Label();
            ToneTableUpDown = new MyNumericUpDown();
            JoinTracksBox = new GroupBox();
            JoinLabel = new Label();
            DisconnectButton = new Button();
            ImageList1 = new ImageList(components);
            JoinTracksButton = new Button();
            TrackInfoBox = new GroupBox();
            TitleTextBox = new TextBox();
            AuthorTextBox = new TextBox();
            ByTextBox = new Label();
            SamplesTab = new TabPage();
            SamplesGrid = new MyDataGridView();
            SamplesTestFieldBox = new GroupBox();
            SaveSampleButton = new Button();
            LoadSampleButton = new Button();
            SampleEditBox = new GroupBox();
            NextPrevSampleBox = new GroupBox();
            NextSampleButton = new Button();
            PrevSampleButton = new Button();
            SampleBox = new GroupBox();
            SampleNumLabel = new Label();
            SampleLoopLabel = new Label();
            SampleLengthLabel = new Label();
            CopySampleButton = new Button();
            PasteSampleButton = new Button();
            SampleNumUpDown = new MyNumericUpDown();
            SampleLoopUpDown = new MyNumericUpDown();
            SampleLengthUpDown = new MyNumericUpDown();
            UnloopSampleButton = new Button();
            ClearSampleButton = new Button();
            SampleBrowserBox = new GroupBox();
            HideSampleBrowserButton = new Button();
            ShowSampleBrowserButton = new Button();
            SampleOpts = new GroupBox();
            SampleOctaveValue = new Label();
            SampleOctaveLabel = new Label();
            SampleSeparator1 = new GroupBox();
            RecalcTonesButton = new Button();
            SampleSeparator2 = new GroupBox();
            SampleToneShiftAsNoteCheckBox = new CheckBox();
            SampleOctaveNum = new MyNumericUpDown();
            OrnamentsTab = new TabPage();
            OrnamentsGrid = new MyDataGridView();
            OrnamentsTestFieldBox = new GroupBox();
            LoadOrnamentButton = new Button();
            SaveOrnamentButton = new Button();
            OrnamentEditBox = new GroupBox();
            NextPrevOrnBox = new GroupBox();
            NextOrnamentButton = new Button();
            PrevOrnamentButton = new Button();
            OrnamentBox = new GroupBox();
            OrnamentNumLabel = new Label();
            OrnamentLoopLabel = new Label();
            OrnamentLengthLabel = new Label();
            CopyOrnamentButton = new Button();
            PasteOrnamentButton = new Button();
            OrnamentNumUpDown = new MyNumericUpDown();
            OrnamentLoopUpDown = new MyNumericUpDown();
            OrnamentLenUpDown = new MyNumericUpDown();
            ClearOrnamentButton = new Button();
            OrnamentOpts = new GroupBox();
            OrnamentOctaveLabel = new Label();
            OrnamentOctaveValue = new Label();
            OrnamentSeperator = new GroupBox();
            OrnamentToneShiftAsNoteCheckBox = new CheckBox();
            OrnamentOctaveNum = new MyNumericUpDown();
            OrnamentsBrowserBox = new GroupBox();
            HideOrnamentBrowserButton = new Button();
            ShowOrnamentBrowserButton = new Button();
            OptionsTab = new TabPage();
            TrackOptsScrollBox = new Panel();
            ManualIntFreq = new TextBox();
            HelpBox = new GroupBox();
            ChipFreqBox = new RadioGroup();
            ManualHz = new TextBox();
            IntFreqBox = new RadioGroup();
            SaveHeaderBox = new RadioGroup();
            VtmFeaturesBox = new RadioGroup();
            InfoTab = new TabPage();
            TrackInfoGB = new GroupBox();
            BoldButton = new Button();
            ItalicButton = new Button();
            UnderlineButton = new Button();
            ShowInfoOnLoadCheckBox = new CheckBox();
            TrackInfoPanel = new Panel();
            TrackInfoRTB = new RichTextBox();
            ViewInfoButton = new Button();
            MuteButton = new CheckBox();
            SoloButton = new CheckBox();
            SaveTextDialog = new SaveFileDialog();
            LoadTextDialog = new OpenFileDialog();
            ShowHintTimer = new System.Windows.Forms.Timer(components);
            HideHintTimer = new System.Windows.Forms.Timer(components);
            ChangeBackupVersion = new System.Windows.Forms.Timer(components);
            ExportWavDialog = new SaveFileDialog();
            ExportPSGDialog = new SaveFileDialog();
            ExportYMDialog = new SaveFileDialog();
            PlayStopTimer = new System.Windows.Forms.Timer(components);
            TrackInfoTimer = new System.Windows.Forms.Timer(components);
            FileBrowserPopup = new ContextMenuStrip(components);
            FBSaveInstrument = new ToolStripMenuItem();
            N1 = new ToolStripSeparator();
            FBRename = new ToolStripMenuItem();
            FBDelete = new ToolStripMenuItem();
            FBNewFolder = new ToolStripMenuItem();
            FBSetQuickAccess = new ToolStripMenuItem();
            LowLightMenu1 = new ContextMenuStrip(components);
            Disabled1 = new ToolStripMenuItem();
            N6 = new ToolStripMenuItem();
            N7 = new ToolStripMenuItem();
            N31 = new ToolStripMenuItem();
            N431 = new ToolStripMenuItem();
            N51 = new ToolStripMenuItem();
            N61 = new ToolStripMenuItem();
            N71 = new ToolStripMenuItem();
            N81 = new ToolStripMenuItem();
            N91 = new ToolStripMenuItem();
            UpdateTimer = new System.Windows.Forms.Timer(components);
            OrnamentsMenu = new ContextMenuStrip(components);
            CutOrnament1 = new ToolStripMenuItem();
            CopyOrnament1 = new ToolStripMenuItem();
            PasteOrnament1 = new ToolStripMenuItem();
            N2 = new ToolStripSeparator();
            ClearOrnament1 = new ToolStripMenuItem();
            N5 = new ToolStripSeparator();
            SwapOrnaments1 = new ToolStripMenuItem();
            PackOrnaments1 = new ToolStripMenuItem();
            SamplesMenu = new ContextMenuStrip(components);
            CutSample1 = new ToolStripMenuItem();
            CopySample1 = new ToolStripMenuItem();
            PasteSample1 = new ToolStripMenuItem();
            N3 = new ToolStripSeparator();
            ClearSample1 = new ToolStripMenuItem();
            N4 = new ToolStripSeparator();
            SwapSamples1 = new ToolStripMenuItem();
            PackSamples1 = new ToolStripMenuItem();
            TabControl.SuspendLayout();
            PatternsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PositionsGrid).BeginInit();
            InterfaceBox.SuspendLayout();
            AutoHLBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)StepHLUpDown).BeginInit();
            ChannelABox.SuspendLayout();
            ChannelBBox.SuspendLayout();
            ChannelCBox.SuspendLayout();
            PatternBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PatternNumUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PatternLenUpDown).BeginInit();
            SpeedBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SpeedBpmUpDown).BeginInit();
            OctaveBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)OctaveUpDown).BeginInit();
            AutoStepBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)AutoStepUpDown).BeginInit();
            AutoEnvBox.SuspendLayout();
            ToneTableBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ToneTableUpDown).BeginInit();
            JoinTracksBox.SuspendLayout();
            TrackInfoBox.SuspendLayout();
            SamplesTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SamplesGrid).BeginInit();
            SamplesTestFieldBox.SuspendLayout();
            NextPrevSampleBox.SuspendLayout();
            SampleBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SampleNumUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SampleLoopUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)SampleLengthUpDown).BeginInit();
            SampleBrowserBox.SuspendLayout();
            SampleOpts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SampleOctaveNum).BeginInit();
            OrnamentsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)OrnamentsGrid).BeginInit();
            OrnamentsTestFieldBox.SuspendLayout();
            NextPrevOrnBox.SuspendLayout();
            OrnamentBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)OrnamentNumUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)OrnamentLoopUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)OrnamentLenUpDown).BeginInit();
            OrnamentOpts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)OrnamentOctaveNum).BeginInit();
            OrnamentsBrowserBox.SuspendLayout();
            OptionsTab.SuspendLayout();
            TrackOptsScrollBox.SuspendLayout();
            ChipFreqBox.SuspendLayout();
            InfoTab.SuspendLayout();
            TrackInfoGB.SuspendLayout();
            TrackInfoPanel.SuspendLayout();
            FileBrowserPopup.SuspendLayout();
            LowLightMenu1.SuspendLayout();
            OrnamentsMenu.SuspendLayout();
            SamplesMenu.SuspendLayout();
            SuspendLayout();
            // 
            // TopBackgroundPanel
            // 
            TopBackgroundPanel.Location = new Point(7, 619);
            TopBackgroundPanel.Name = "TopBackgroundPanel";
            TopBackgroundPanel.Size = new Size(620, 9);
            TopBackgroundPanel.TabIndex = 0;
            TopBackgroundPanel.Visible = false;
            // 
            // TabControl
            // 
            TabControl.Controls.Add(PatternsTab);
            TabControl.Controls.Add(SamplesTab);
            TabControl.Controls.Add(OrnamentsTab);
            TabControl.Controls.Add(OptionsTab);
            TabControl.Controls.Add(InfoTab);
            TabControl.Location = new Point(0, 0);
            TabControl.Name = "TabControl";
            TabControl.SelectedIndex = 0;
            TabControl.Size = new Size(634, 665);
            TabControl.TabIndex = 0;
            TabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
            // 
            // PatternsTab
            // 
            PatternsTab.Controls.Add(PositionsGrid);
            PatternsTab.Controls.Add(InterfaceBox);
            PatternsTab.Controls.Add(AutoHLBox);
            PatternsTab.Controls.Add(ChannelABox);
            PatternsTab.Controls.Add(ChannelBBox);
            PatternsTab.Controls.Add(ChannelCBox);
            PatternsTab.Controls.Add(PatternBox);
            PatternsTab.Controls.Add(SpeedBox);
            PatternsTab.Controls.Add(OctaveBox);
            PatternsTab.Controls.Add(AutoStepBox);
            PatternsTab.Controls.Add(AutoEnvBox);
            PatternsTab.Controls.Add(ToneTableBox);
            PatternsTab.Controls.Add(JoinTracksBox);
            PatternsTab.Controls.Add(TrackInfoBox);
            PatternsTab.Font = new Font("Microsoft Sans Serif", 8.25F);
            PatternsTab.ImageIndex = 29;
            PatternsTab.Location = new Point(4, 24);
            PatternsTab.Name = "PatternsTab";
            PatternsTab.Size = new Size(626, 637);
            PatternsTab.TabIndex = 0;
            PatternsTab.Text = "Patterns";
            // 
            // PositionsGrid
            // 
            PositionsGrid.AllowDrop = true;
            PositionsGrid.AllowUserToAddRows = false;
            PositionsGrid.AllowUserToDeleteRows = false;
            PositionsGrid.AllowUserToResizeColumns = false;
            PositionsGrid.AllowUserToResizeRows = false;
            PositionsGrid.BackgroundColor = Color.White;
            PositionsGrid.BorderStyle = BorderStyle.None;
            PositionsGrid.CellBorderStyle = DataGridViewCellBorderStyle.None;
            PositionsGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            PositionsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            PositionsGrid.ColumnHeadersVisible = false;
            PositionsGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
            PositionsGrid.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            PositionsGrid.Location = new Point(0, 85);
            PositionsGrid.MultiSelect = false;
            PositionsGrid.Name = "PositionsGrid";
            PositionsGrid.RowHeadersVisible = false;
            PositionsGrid.ScrollBars = ScrollBars.Horizontal;
            PositionsGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            PositionsGrid.Size = new Size(626, 49);
            PositionsGrid.TabIndex = 0;
            PositionsGrid.AllowDrop = true;
            PositionsGrid.MouseWheel += PositionsGrid_MouseWheel;
            PositionsGrid.CellPainting += PositionsGrid_CellPainting;
            PositionsGrid.CurrentCellChanged += PositionsGrid_CurrentCellChanged;
            PositionsGrid.DragDrop += PositionsGrid_DragDrop;
            PositionsGrid.DragOver += PositionsGrid_DragOver;
            PositionsGrid.DragLeave += PositionsGrid_DragLeave;
            PositionsGrid.KeyDown += PositionsGrid_KeyDown;
            PositionsGrid.KeyPress += PositionsGrid_KeyPress;
            PositionsGrid.KeyUp += PositionsGrid_KeyUp;
            PositionsGrid.MouseDown += PositionsGrid_MouseDown;
            PositionsGrid.MouseUp += PositionsGrid_MouseUp;
            // 
            // InterfaceBox
            // 
            InterfaceBox.Controls.Add(EnvelopeAsNoteCheckBox);
            InterfaceBox.Controls.Add(UseLastNoteParamsCheckBox);
            InterfaceBox.Controls.Add(MoveBetweenPatternsCheckBox);
            InterfaceBox.Location = new Point(0, 604);
            InterfaceBox.Name = "InterfaceBox";
            InterfaceBox.Size = new Size(626, 33);
            InterfaceBox.TabIndex = 14;
            InterfaceBox.TabStop = false;
            // 
            // EnvelopeAsNoteCheckBox
            // 
            EnvelopeAsNoteCheckBox.Font = new Font("Microsoft Sans Serif", 8.25F);
            EnvelopeAsNoteCheckBox.Location = new Point(12, 13);
            EnvelopeAsNoteCheckBox.Name = "EnvelopeAsNoteCheckBox";
            EnvelopeAsNoteCheckBox.Size = new Size(117, 17);
            EnvelopeAsNoteCheckBox.TabIndex = 0;
            EnvelopeAsNoteCheckBox.Text = "Envelope as Note";
            EnvelopeAsNoteCheckBox.Click += EnvelopeAsNoteCheckBox_Click;
            EnvelopeAsNoteCheckBox.MouseUp += EnvelopeAsNoteCheckBox_MouseUp;
            // 
            // UseLastNoteParamsCheckBox
            // 
            UseLastNoteParamsCheckBox.Font = new Font("Microsoft Sans Serif", 8.25F);
            UseLastNoteParamsCheckBox.Location = new Point(220, 13);
            UseLastNoteParamsCheckBox.Name = "UseLastNoteParamsCheckBox";
            UseLastNoteParamsCheckBox.Size = new Size(137, 17);
            UseLastNoteParamsCheckBox.TabIndex = 1;
            UseLastNoteParamsCheckBox.Text = "Use Last Note Params";
            UseLastNoteParamsCheckBox.MouseDown += UseLastNoteParamsCheckBox_MouseDown;
            // 
            // MoveBetweenPatternsCheckBox
            // 
            MoveBetweenPatternsCheckBox.Font = new Font("Microsoft Sans Serif", 8.25F);
            MoveBetweenPatternsCheckBox.Location = new Point(398, 13);
            MoveBetweenPatternsCheckBox.Name = "MoveBetweenPatternsCheckBox";
            MoveBetweenPatternsCheckBox.Size = new Size(146, 17);
            MoveBetweenPatternsCheckBox.TabIndex = 2;
            MoveBetweenPatternsCheckBox.Text = "Move Between Patterns";
            MoveBetweenPatternsCheckBox.MouseDown += MoveBetweenPatternsCheckBox_MouseDown;
            // 
            // AutoHLBox
            // 
            AutoHLBox.Controls.Add(AutoLL);
            AutoHLBox.Controls.Add(AutoHLCheckBox);
            AutoHLBox.Controls.Add(StepHLUpDown);
            AutoHLBox.Location = new Point(0, 131);
            AutoHLBox.Name = "AutoHLBox";
            AutoHLBox.Size = new Size(97, 33);
            AutoHLBox.TabIndex = 10;
            AutoHLBox.TabStop = false;
            // 
            // AutoLL
            // 
            AutoLL.Location = new Point(1, 9);
            AutoLL.Name = "AutoLL";
            AutoLL.Size = new Size(20, 20);
            AutoLL.TabIndex = 5;
            AutoLL.Text = "0";
            AutoLL.MouseDown += AutoLL_MouseDown;
            // 
            // AutoHLCheckBox
            // 
            AutoHLCheckBox.Appearance = Appearance.Button;
            AutoHLCheckBox.Location = new Point(20, 9);
            AutoHLCheckBox.Name = "AutoHLCheckBox";
            AutoHLCheckBox.Size = new Size(37, 20);
            AutoHLCheckBox.TabIndex = 0;
            AutoHLCheckBox.Text = "Auto";
            AutoHLCheckBox.TextAlign = ContentAlignment.MiddleCenter;
            AutoHLCheckBox.Click += AutoHLCheckBox_Click;
            // 
            // StepHLUpDown
            // 
            StepHLUpDown.Location = new Point(58, 9);
            StepHLUpDown.Name = "StepHLUpDown";
            StepHLUpDown.Size = new Size(34, 20);
            StepHLUpDown.TabIndex = 1;
            StepHLUpDown.Value = new decimal(new int[] { 0, 0, 0, 0 });
            StepHLUpDown.ValueChanging += StepHLUpDown_ValueChanging;
            // 
            // ChannelABox
            // 
            ChannelABox.Controls.Add(ChannelAMute);
            ChannelABox.Controls.Add(ChannelATone);
            ChannelABox.Controls.Add(ChannelANoise);
            ChannelABox.Controls.Add(ChannelAEnvelope);
            ChannelABox.Controls.Add(ChannelASolo);
            ChannelABox.Location = new Point(96, 131);
            ChannelABox.Name = "ChannelABox";
            ChannelABox.Size = new Size(142, 33);
            ChannelABox.TabIndex = 11;
            ChannelABox.TabStop = false;
            // 
            // ChannelAMute
            // 
            ChannelAMute.Appearance = Appearance.Button;
            ChannelAMute.Location = new Point(5, 9);
            ChannelAMute.Name = "ChannelAMute";
            ChannelAMute.Size = new Size(50, 20);
            ChannelAMute.TabIndex = 0;
            ChannelAMute.Text = "Mute A";
            ChannelAMute.TextAlign = ContentAlignment.MiddleCenter;
            ChannelAMute.Click += MuteOn_Click;
            // 
            // ChannelATone
            // 
            ChannelATone.Appearance = Appearance.Button;
            ChannelATone.Location = new Point(76, 9);
            ChannelATone.Name = "ChannelATone";
            ChannelATone.Size = new Size(20, 20);
            ChannelATone.TabIndex = 1;
            ChannelATone.Text = "T";
            ChannelATone.Click += TNEOn_Click;
            // 
            // ChannelANoise
            // 
            ChannelANoise.Appearance = Appearance.Button;
            ChannelANoise.Location = new Point(96, 9);
            ChannelANoise.Name = "ChannelANoise";
            ChannelANoise.Size = new Size(20, 20);
            ChannelANoise.TabIndex = 2;
            ChannelANoise.Text = "N";
            ChannelANoise.Click += TNEOn_Click;
            // 
            // ChannelAEnvelope
            // 
            ChannelAEnvelope.Appearance = Appearance.Button;
            ChannelAEnvelope.Location = new Point(117, 9);
            ChannelAEnvelope.Name = "ChannelAEnvelope";
            ChannelAEnvelope.Size = new Size(20, 20);
            ChannelAEnvelope.TabIndex = 3;
            ChannelAEnvelope.Text = "E";
            ChannelAEnvelope.Click += TNEOn_Click;
            // 
            // ChannelASolo
            // 
            ChannelASolo.Appearance = Appearance.Button;
            ChannelASolo.Location = new Point(55, 9);
            ChannelASolo.Name = "ChannelASolo";
            ChannelASolo.Size = new Size(20, 20);
            ChannelASolo.TabIndex = 4;
            ChannelASolo.Text = "S";
            ChannelASolo.Click += SoloOn_Click;
            // 
            // ChannelBBox
            // 
            ChannelBBox.Controls.Add(ChannelBMute);
            ChannelBBox.Controls.Add(ChannelBTone);
            ChannelBBox.Controls.Add(ChannelBNoise);
            ChannelBBox.Controls.Add(ChannelBEnvelope);
            ChannelBBox.Controls.Add(ChannelBSolo);
            ChannelBBox.Location = new Point(240, 131);
            ChannelBBox.Name = "ChannelBBox";
            ChannelBBox.Size = new Size(145, 33);
            ChannelBBox.TabIndex = 12;
            ChannelBBox.TabStop = false;
            // 
            // ChannelBMute
            // 
            ChannelBMute.Appearance = Appearance.Button;
            ChannelBMute.Location = new Point(5, 9);
            ChannelBMute.Name = "ChannelBMute";
            ChannelBMute.Size = new Size(50, 20);
            ChannelBMute.TabIndex = 0;
            ChannelBMute.Text = "Mute B";
            ChannelBMute.TextAlign = ContentAlignment.MiddleCenter;
            ChannelBMute.Click += MuteOn_Click;
            // 
            // ChannelBTone
            // 
            ChannelBTone.Appearance = Appearance.Button;
            ChannelBTone.Location = new Point(77, 9);
            ChannelBTone.Name = "ChannelBTone";
            ChannelBTone.Size = new Size(20, 20);
            ChannelBTone.TabIndex = 1;
            ChannelBTone.Text = "T";
            ChannelBTone.Click += TNEOn_Click;
            // 
            // ChannelBNoise
            // 
            ChannelBNoise.Appearance = Appearance.Button;
            ChannelBNoise.Location = new Point(97, 9);
            ChannelBNoise.Name = "ChannelBNoise";
            ChannelBNoise.Size = new Size(20, 20);
            ChannelBNoise.TabIndex = 2;
            ChannelBNoise.Text = "N";
            ChannelBNoise.Click += TNEOn_Click;
            // 
            // ChannelBEnvelope
            // 
            ChannelBEnvelope.Appearance = Appearance.Button;
            ChannelBEnvelope.Location = new Point(117, 9);
            ChannelBEnvelope.Name = "ChannelBEnvelope";
            ChannelBEnvelope.Size = new Size(20, 20);
            ChannelBEnvelope.TabIndex = 3;
            ChannelBEnvelope.Text = "E";
            ChannelBEnvelope.Click += TNEOn_Click;
            // 
            // ChannelBSolo
            // 
            ChannelBSolo.Appearance = Appearance.Button;
            ChannelBSolo.Location = new Point(57, 9);
            ChannelBSolo.Name = "ChannelBSolo";
            ChannelBSolo.Size = new Size(20, 20);
            ChannelBSolo.TabIndex = 4;
            ChannelBSolo.Text = "S";
            ChannelBSolo.Click += SoloOn_Click;
            // 
            // ChannelCBox
            // 
            ChannelCBox.Controls.Add(ChannelCMute);
            ChannelCBox.Controls.Add(ChannelCTone);
            ChannelCBox.Controls.Add(ChannelCNoise);
            ChannelCBox.Controls.Add(ChannelCEnvelope);
            ChannelCBox.Controls.Add(ChannelCSolo);
            ChannelCBox.Location = new Point(384, 131);
            ChannelCBox.Name = "ChannelCBox";
            ChannelCBox.Size = new Size(145, 33);
            ChannelCBox.TabIndex = 13;
            ChannelCBox.TabStop = false;
            // 
            // ChannelCMute
            // 
            ChannelCMute.Appearance = Appearance.Button;
            ChannelCMute.Location = new Point(7, 9);
            ChannelCMute.Name = "ChannelCMute";
            ChannelCMute.Size = new Size(50, 20);
            ChannelCMute.TabIndex = 0;
            ChannelCMute.Text = "Mute C";
            ChannelCMute.TextAlign = ContentAlignment.MiddleCenter;
            ChannelCMute.Click += MuteOn_Click;
            // 
            // ChannelCTone
            // 
            ChannelCTone.Appearance = Appearance.Button;
            ChannelCTone.Location = new Point(79, 9);
            ChannelCTone.Name = "ChannelCTone";
            ChannelCTone.Size = new Size(20, 20);
            ChannelCTone.TabIndex = 1;
            ChannelCTone.Text = "T";
            ChannelCTone.Click += TNEOn_Click;
            // 
            // ChannelCNoise
            // 
            ChannelCNoise.Appearance = Appearance.Button;
            ChannelCNoise.Location = new Point(99, 9);
            ChannelCNoise.Name = "ChannelCNoise";
            ChannelCNoise.Size = new Size(20, 20);
            ChannelCNoise.TabIndex = 2;
            ChannelCNoise.Text = "N";
            ChannelCNoise.Click += TNEOn_Click;
            // 
            // ChannelCEnvelope
            // 
            ChannelCEnvelope.Appearance = Appearance.Button;
            ChannelCEnvelope.Location = new Point(121, 9);
            ChannelCEnvelope.Name = "ChannelCEnvelope";
            ChannelCEnvelope.Size = new Size(20, 20);
            ChannelCEnvelope.TabIndex = 3;
            ChannelCEnvelope.Text = "E";
            ChannelCEnvelope.Click += TNEOn_Click;
            // 
            // ChannelCSolo
            // 
            ChannelCSolo.Appearance = Appearance.Button;
            ChannelCSolo.Location = new Point(59, 9);
            ChannelCSolo.Name = "ChannelCSolo";
            ChannelCSolo.Size = new Size(20, 20);
            ChannelCSolo.TabIndex = 4;
            ChannelCSolo.Text = "S";
            ChannelCSolo.Click += SoloOn_Click;
            // 
            // PatternBox
            // 
            PatternBox.Controls.Add(PatternNumberLabel);
            PatternBox.Controls.Add(PatternLengthLabel);
            PatternBox.Controls.Add(LoadPatternButton);
            PatternBox.Controls.Add(SavePatternButton);
            PatternBox.Controls.Add(PatternNumUpDown);
            PatternBox.Controls.Add(PatternLenUpDown);
            PatternBox.Location = new Point(-2, -2);
            PatternBox.Name = "PatternBox";
            PatternBox.Size = new Size(168, 58);
            PatternBox.TabIndex = 0;
            PatternBox.TabStop = false;
            // 
            // PatternNumberLabel
            // 
            PatternNumberLabel.Location = new Point(60, 15);
            PatternNumberLabel.Name = "PatternNumberLabel";
            PatternNumberLabel.Size = new Size(47, 14);
            PatternNumberLabel.TabIndex = 0;
            PatternNumberLabel.Text = "Pattern";
            // 
            // PatternLengthLabel
            // 
            PatternLengthLabel.Location = new Point(113, 15);
            PatternLengthLabel.Name = "PatternLengthLabel";
            PatternLengthLabel.Size = new Size(47, 16);
            PatternLengthLabel.TabIndex = 1;
            PatternLengthLabel.Text = "Length";
            // 
            // LoadPatternButton
            // 
            LoadPatternButton.Font = new Font("Microsoft Sans Serif", 8.25F);
            LoadPatternButton.Location = new Point(2, 10);
            LoadPatternButton.Name = "LoadPatternButton";
            LoadPatternButton.Size = new Size(45, 21);
            LoadPatternButton.TabIndex = 2;
            LoadPatternButton.Text = "Load";
            LoadPatternButton.Click += LoadPatternButton_Click;
            // 
            // SavePatternButton
            // 
            SavePatternButton.Font = new Font("Microsoft Sans Serif", 8.25F);
            SavePatternButton.Location = new Point(2, 32);
            SavePatternButton.Name = "SavePatternButton";
            SavePatternButton.Size = new Size(45, 21);
            SavePatternButton.TabIndex = 3;
            SavePatternButton.Text = "Save";
            SavePatternButton.Click += SavePatternButton_Click;
            // 
            // PatternNumUpDown
            // 
            PatternNumUpDown.Location = new Point(60, 32);
            PatternNumUpDown.Name = "PatternNumUpDown";
            PatternNumUpDown.Size = new Size(40, 20);
            PatternNumUpDown.TabIndex = 1;
            PatternNumUpDown.Value = new decimal(new int[] { 0, 0, 0, 0 });
            PatternNumUpDown.ValueChanging += PatternNumUpDown_ValueChanging;
            // 
            // PatternLenUpDown
            // 
            PatternLenUpDown.Location = new Point(113, 32);
            PatternLenUpDown.Name = "PatternLenUpDown";
            PatternLenUpDown.Size = new Size(40, 20);
            PatternLenUpDown.TabIndex = 3;
            PatternLenUpDown.Value = new decimal(new int[] { 0, 0, 0, 0 });
            PatternLenUpDown.ValueChanging += PatternLenUpDown_ValueChanging;
            // 
            // SpeedBox
            // 
            SpeedBox.Controls.Add(SpeedBPMLabel);
            SpeedBox.Controls.Add(SpeedBpmUpDown);
            SpeedBox.Location = new Point(164, -2);
            SpeedBox.Name = "SpeedBox";
            SpeedBox.Size = new Size(86, 58);
            SpeedBox.TabIndex = 2;
            SpeedBox.TabStop = false;
            // 
            // SpeedBPMLabel
            // 
            SpeedBPMLabel.Location = new Point(14, 15);
            SpeedBPMLabel.Name = "SpeedBPMLabel";
            SpeedBPMLabel.Size = new Size(66, 16);
            SpeedBPMLabel.TabIndex = 0;
            SpeedBPMLabel.Text = "Speed/BPM";
            // 
            // SpeedBpmUpDown
            // 
            SpeedBpmUpDown.Location = new Point(14, 32);
            SpeedBpmUpDown.Name = "SpeedBpmUpDown";
            SpeedBpmUpDown.Size = new Size(58, 20);
            SpeedBpmUpDown.TabIndex = 1;
            SpeedBpmUpDown.ValidateValue = false;
            SpeedBpmUpDown.Value = new decimal(new int[] { 3, 0, 0, 0 });
            SpeedBpmUpDown.ValueChanging += SpeedBpmUpDown_ValueChanging;
            // 
            // OctaveBox
            // 
            OctaveBox.Controls.Add(OctaveUpDown);
            OctaveBox.Controls.Add(OctaveLabel);
            OctaveBox.Location = new Point(248, -2);
            OctaveBox.Name = "OctaveBox";
            OctaveBox.Size = new Size(68, 58);
            OctaveBox.TabIndex = 3;
            OctaveBox.TabStop = false;
            // 
            // OctaveUpDown
            // 
            OctaveUpDown.Location = new Point(14, 32);
            OctaveUpDown.Maximum = new decimal(new int[] { 8, 0, 0, 0 });
            OctaveUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            OctaveUpDown.Name = "OctaveUpDown";
            OctaveUpDown.Size = new Size(41, 20);
            OctaveUpDown.TabIndex = 1;
            OctaveUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // OctaveLabel
            // 
            OctaveLabel.Location = new Point(14, 15);
            OctaveLabel.Name = "OctaveLabel";
            OctaveLabel.Size = new Size(46, 14);
            OctaveLabel.TabIndex = 0;
            OctaveLabel.Text = "Octave";
            // 
            // AutoStepBox
            // 
            AutoStepBox.Controls.Add(AutoStepUpDown);
            AutoStepBox.Controls.Add(AutoStepButton);
            AutoStepBox.Location = new Point(314, -2);
            AutoStepBox.Name = "AutoStepBox";
            AutoStepBox.Size = new Size(90, 58);
            AutoStepBox.TabIndex = 4;
            AutoStepBox.TabStop = false;
            // 
            // AutoStepUpDown
            // 
            AutoStepUpDown.Location = new Point(14, 32);
            AutoStepUpDown.Maximum = new decimal(new int[] { 64, 0, 0, 0 });
            AutoStepUpDown.Minimum = new decimal(new int[] { 64, 0, 0, int.MinValue });
            AutoStepUpDown.Name = "AutoStepUpDown";
            AutoStepUpDown.Size = new Size(67, 20);
            AutoStepUpDown.TabIndex = 1;
            AutoStepUpDown.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // AutoStepButton
            // 
            AutoStepButton.Appearance = Appearance.Button;
            AutoStepButton.Font = new Font("Microsoft Sans Serif", 8.25F);
            AutoStepButton.Location = new Point(14, 11);
            AutoStepButton.Name = "AutoStepButton";
            AutoStepButton.Size = new Size(68, 20);
            AutoStepButton.TabIndex = 0;
            AutoStepButton.Text = "Auto Step";
            AutoStepButton.Click += AutoStepBtn_Click;
            // 
            // AutoEnvBox
            // 
            AutoEnvBox.Controls.Add(AutoEnvButton);
            AutoEnvBox.Controls.Add(AutoEnv0Button);
            AutoEnvBox.Controls.Add(AutoEnvToggleButton);
            AutoEnvBox.Controls.Add(AutoEnv1Button);
            AutoEnvBox.Location = new Point(402, -2);
            AutoEnvBox.Name = "AutoEnvBox";
            AutoEnvBox.Size = new Size(94, 58);
            AutoEnvBox.TabIndex = 5;
            AutoEnvBox.TabStop = false;
            // 
            // AutoEnvButton
            // 
            AutoEnvButton.Appearance = Appearance.Button;
            AutoEnvButton.Location = new Point(14, 11);
            AutoEnvButton.Name = "AutoEnvButton";
            AutoEnvButton.Size = new Size(66, 20);
            AutoEnvButton.TabIndex = 0;
            AutoEnvButton.Text = "Auto Env";
            AutoEnvButton.Click += AutoEnvButton_Click;
            // 
            // AutoEnv0Button
            // 
            AutoEnv0Button.Location = new Point(14, 32);
            AutoEnv0Button.Name = "AutoEnv0Button";
            AutoEnv0Button.Size = new Size(22, 20);
            AutoEnv0Button.TabIndex = 1;
            AutoEnv0Button.Text = "1";
            AutoEnv0Button.Click += AutoEnv0Button_Click;
            // 
            // AutoEnvToggleButton
            // 
            AutoEnvToggleButton.Location = new Point(36, 32);
            AutoEnvToggleButton.Name = "AutoEnvToggleButton";
            AutoEnvToggleButton.Size = new Size(22, 20);
            AutoEnvToggleButton.TabIndex = 2;
            AutoEnvToggleButton.Text = ":";
            AutoEnvToggleButton.Click += AutoEnvToggleButton_Click;
            // 
            // AutoEnv1Button
            // 
            AutoEnv1Button.Location = new Point(58, 32);
            AutoEnv1Button.Name = "AutoEnv1Button";
            AutoEnv1Button.Size = new Size(22, 20);
            AutoEnv1Button.TabIndex = 3;
            AutoEnv1Button.Text = "1";
            AutoEnv1Button.Click += AutoEnv1Button_Click;
            // 
            // ToneTableBox
            // 
            ToneTableBox.Controls.Add(ToneTableLabel);
            ToneTableBox.Controls.Add(ToneTableUpDown);
            ToneTableBox.Location = new Point(494, -2);
            ToneTableBox.Name = "ToneTableBox";
            ToneTableBox.Size = new Size(81, 58);
            ToneTableBox.TabIndex = 6;
            ToneTableBox.TabStop = false;
            // 
            // ToneTableLabel
            // 
            ToneTableLabel.Location = new Point(12, 15);
            ToneTableLabel.Name = "ToneTableLabel";
            ToneTableLabel.Size = new Size(63, 14);
            ToneTableLabel.TabIndex = 0;
            ToneTableLabel.Text = "Tone Table";
            // 
            // ToneTableUpDown
            // 
            ToneTableUpDown.Location = new Point(13, 32);
            ToneTableUpDown.Maximum = new decimal(new int[] { 4, 0, 0, 0 });
            ToneTableUpDown.Name = "ToneTableUpDown";
            ToneTableUpDown.Size = new Size(55, 20);
            ToneTableUpDown.TabIndex = 1;
            ToneTableUpDown.Value = new decimal(new int[] { 0, 0, 0, 0 });
            ToneTableUpDown.ValueChanging += ToneTableUpDown_ValueChanging;
            // 
            // JoinTracksBox
            // 
            JoinTracksBox.Controls.Add(JoinLabel);
            JoinTracksBox.Controls.Add(DisconnectButton);
            JoinTracksBox.Controls.Add(JoinTracksButton);
            JoinTracksBox.Location = new Point(573, -2);
            JoinTracksBox.Name = "JoinTracksBox";
            JoinTracksBox.Size = new Size(55, 58);
            JoinTracksBox.TabIndex = 7;
            JoinTracksBox.TabStop = false;
            // 
            // JoinLabel
            // 
            JoinLabel.Location = new Point(6, 37);
            JoinLabel.Name = "JoinLabel";
            JoinLabel.Size = new Size(43, 14);
            JoinLabel.TabIndex = 2;
            JoinLabel.Text = "-- - --";
            JoinLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DisconnectButton
            // 
            DisconnectButton.BackgroundImageLayout = ImageLayout.None;
            DisconnectButton.Enabled = false;
            DisconnectButton.FlatAppearance.BorderSize = 0;
            DisconnectButton.FlatStyle = FlatStyle.Flat;
            DisconnectButton.ImageIndex = 0;
            DisconnectButton.ImageList = ImageList1;
            DisconnectButton.Location = new Point(37, 9);
            DisconnectButton.Margin = new Padding(0);
            DisconnectButton.Name = "DisconnectButton";
            DisconnectButton.Size = new Size(9, 9);
            DisconnectButton.TabIndex = 4;
            DisconnectButton.TabStop = false;
            DisconnectButton.Click += DisconnectButton_Click;
            // 
            // ImageList1
            // 
            ImageList1.ColorDepth = ColorDepth.Depth32Bit;
            ImageList1.ImageStream = (ImageListStreamer)resources.GetObject("ImageList1.ImageStream");
            ImageList1.TransparentColor = Color.Transparent;
            ImageList1.Images.SetKeyName(0, "Close_Disabled.png");
            ImageList1.Images.SetKeyName(1, "Close.png");
            // 
            // JoinTracksButton
            // 
            JoinTracksButton.Location = new Point(6, 17);
            JoinTracksButton.Name = "JoinTracksButton";
            JoinTracksButton.Size = new Size(43, 20);
            JoinTracksButton.TabIndex = 0;
            JoinTracksButton.Text = "Join";
            JoinTracksButton.Click += VTAction_Execute;
            // 
            // TrackInfoBox
            // 
            TrackInfoBox.Controls.Add(TitleTextBox);
            TrackInfoBox.Controls.Add(AuthorTextBox);
            TrackInfoBox.Controls.Add(ByTextBox);
            TrackInfoBox.Location = new Point(-2, 49);
            TrackInfoBox.Name = "TrackInfoBox";
            TrackInfoBox.Size = new Size(628, 34);
            TrackInfoBox.TabIndex = 8;
            TrackInfoBox.TabStop = false;
            // 
            // TitleTextBox
            // 
            TitleTextBox.Location = new Point(2, 9);
            TitleTextBox.MaxLength = 32;
            TitleTextBox.Name = "TitleTextBox";
            TitleTextBox.Size = new Size(231, 20);
            TitleTextBox.TabIndex = 0;
            TitleTextBox.TextChanged += TitleTextBox_TextChanged;
            TitleTextBox.KeyPress += TitleTextBox_KeyPress;
            // 
            // AuthorTextBox
            // 
            AuthorTextBox.Location = new Point(256, 9);
            AuthorTextBox.MaxLength = 32;
            AuthorTextBox.Name = "AuthorTextBox";
            AuthorTextBox.Size = new Size(233, 20);
            AuthorTextBox.TabIndex = 1;
            AuthorTextBox.TextChanged += AuthorTextBox_TextChanged;
            AuthorTextBox.KeyPress += AuthorTextBox_KeyPress;
            // 
            // ByTextBox
            // 
            ByTextBox.Location = new Point(239, 13);
            ByTextBox.Name = "ByTextBox";
            ByTextBox.Size = new Size(22, 13);
            ByTextBox.TabIndex = 0;
            ByTextBox.Text = "by";
            // 
            // SamplesTab
            // 
            SamplesTab.Controls.Add(SamplesGrid);
            SamplesTab.Controls.Add(SamplesTestFieldBox);
            SamplesTab.Controls.Add(SampleEditBox);
            SamplesTab.Controls.Add(NextPrevSampleBox);
            SamplesTab.Controls.Add(SampleBox);
            SamplesTab.Controls.Add(SampleBrowserBox);
            SamplesTab.Controls.Add(SampleOpts);
            SamplesTab.ImageIndex = 31;
            SamplesTab.Location = new Point(4, 24);
            SamplesTab.Name = "SamplesTab";
            SamplesTab.Size = new Size(626, 637);
            SamplesTab.TabIndex = 1;
            SamplesTab.Text = "Samples";
            // 
            // SamplesGrid
            // 
            SamplesGrid.AllowUserToAddRows = false;
            SamplesGrid.AllowUserToDeleteRows = false;
            SamplesGrid.AllowUserToResizeColumns = false;
            SamplesGrid.AllowUserToResizeRows = false;
            SamplesGrid.BackgroundColor = Color.White;
            SamplesGrid.BorderStyle = BorderStyle.None;
            SamplesGrid.CellBorderStyle = DataGridViewCellBorderStyle.None;
            SamplesGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            SamplesGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            SamplesGrid.ColumnHeadersVisible = false;
            SamplesGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
            SamplesGrid.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            SamplesGrid.Location = new Point(0, 0);
            SamplesGrid.MultiSelect = false;
            SamplesGrid.Name = "SamplesGrid";
            SamplesGrid.RowHeadersVisible = false;
            SamplesGrid.ScrollBars = ScrollBars.Horizontal;
            SamplesGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            SamplesGrid.Size = new Size(626, 64);
            SamplesGrid.TabIndex = 0;
            SamplesGrid.MouseWheel += SamplesGrid_MouseWheel;
            SamplesGrid.CellPainting += SamplesGrid_CellPainting;
            SamplesGrid.CurrentCellChanged += SamplesGrid_CurrentCellChanged;
            SamplesGrid.MouseUp += SamplesGrid_MouseUp;
            // 
            // SamplesTestFieldBox
            // 
            SamplesTestFieldBox.Controls.Add(SaveSampleButton);
            SamplesTestFieldBox.Controls.Add(LoadSampleButton);
            SamplesTestFieldBox.Location = new Point(0, 63);
            SamplesTestFieldBox.Name = "SamplesTestFieldBox";
            SamplesTestFieldBox.Size = new Size(345, 51);
            SamplesTestFieldBox.TabIndex = 0;
            SamplesTestFieldBox.TabStop = false;
            // 
            // SaveSampleButton
            // 
            SaveSampleButton.Location = new Point(282, 14);
            SaveSampleButton.Name = "SaveSampleButton";
            SaveSampleButton.Size = new Size(49, 27);
            SaveSampleButton.TabIndex = 1;
            SaveSampleButton.Text = "Save";
            SaveSampleButton.Click += SaveSampleButton_Click;
            // 
            // LoadSampleButton
            // 
            LoadSampleButton.Font = new Font("Microsoft Sans Serif", 8.25F);
            LoadSampleButton.Location = new Point(229, 14);
            LoadSampleButton.Name = "LoadSampleButton";
            LoadSampleButton.Size = new Size(49, 27);
            LoadSampleButton.TabIndex = 0;
            LoadSampleButton.Text = "Load";
            LoadSampleButton.Click += LoadSampleButton_Click;
            // 
            // SampleEditBox
            // 
            SampleEditBox.Location = new Point(0, 108);
            SampleEditBox.Name = "SampleEditBox";
            SampleEditBox.Size = new Size(345, 329);
            SampleEditBox.TabIndex = 2;
            SampleEditBox.TabStop = false;
            // 
            // NextPrevSampleBox
            // 
            NextPrevSampleBox.Controls.Add(NextSampleButton);
            NextPrevSampleBox.Controls.Add(PrevSampleButton);
            NextPrevSampleBox.Location = new Point(343, 63);
            NextPrevSampleBox.Name = "NextPrevSampleBox";
            NextPrevSampleBox.Size = new Size(183, 51);
            NextPrevSampleBox.TabIndex = 1;
            NextPrevSampleBox.TabStop = false;
            // 
            // NextSampleButton
            // 
            NextSampleButton.Location = new Point(104, 16);
            NextSampleButton.Name = "NextSampleButton";
            NextSampleButton.Size = new Size(65, 25);
            NextSampleButton.TabIndex = 1;
            NextSampleButton.Text = ">>";
            NextSampleButton.Click += NextSampleButton_Click;
            // 
            // PrevSampleButton
            // 
            PrevSampleButton.Location = new Point(8, 16);
            PrevSampleButton.Name = "PrevSampleButton";
            PrevSampleButton.Size = new Size(65, 25);
            PrevSampleButton.TabIndex = 0;
            PrevSampleButton.Text = "<<";
            PrevSampleButton.Click += PrevSampleButton_Click;
            // 
            // SampleBox
            // 
            SampleBox.Controls.Add(SampleNumLabel);
            SampleBox.Controls.Add(SampleLoopLabel);
            SampleBox.Controls.Add(SampleLengthLabel);
            SampleBox.Controls.Add(CopySampleButton);
            SampleBox.Controls.Add(PasteSampleButton);
            SampleBox.Controls.Add(SampleNumUpDown);
            SampleBox.Controls.Add(SampleLoopUpDown);
            SampleBox.Controls.Add(SampleLengthUpDown);
            SampleBox.Controls.Add(UnloopSampleButton);
            SampleBox.Controls.Add(ClearSampleButton);
            SampleBox.Location = new Point(343, 108);
            SampleBox.Name = "SampleBox";
            SampleBox.Size = new Size(183, 166);
            SampleBox.TabIndex = 3;
            SampleBox.TabStop = false;
            // 
            // SampleNumLabel
            // 
            SampleNumLabel.Location = new Point(16, 18);
            SampleNumLabel.Name = "SampleNumLabel";
            SampleNumLabel.Size = new Size(48, 13);
            SampleNumLabel.TabIndex = 0;
            SampleNumLabel.Text = "Sample";
            // 
            // SampleLoopLabel
            // 
            SampleLoopLabel.Location = new Point(16, 70);
            SampleLoopLabel.Name = "SampleLoopLabel";
            SampleLoopLabel.Size = new Size(35, 13);
            SampleLoopLabel.TabIndex = 1;
            SampleLoopLabel.Text = "Loop";
            // 
            // SampleLengthLabel
            // 
            SampleLengthLabel.Location = new Point(85, 18);
            SampleLengthLabel.Name = "SampleLengthLabel";
            SampleLengthLabel.Size = new Size(48, 13);
            SampleLengthLabel.TabIndex = 2;
            SampleLengthLabel.Text = "Length";
            // 
            // CopySampleButton
            // 
            CopySampleButton.Location = new Point(85, 70);
            CopySampleButton.Name = "CopySampleButton";
            CopySampleButton.Size = new Size(48, 21);
            CopySampleButton.TabIndex = 3;
            CopySampleButton.Text = "Copy";
            CopySampleButton.Click += CopySampleButton_Click;
            // 
            // PasteSampleButton
            // 
            PasteSampleButton.Location = new Point(85, 91);
            PasteSampleButton.Name = "PasteSampleButton";
            PasteSampleButton.Size = new Size(48, 21);
            PasteSampleButton.TabIndex = 4;
            PasteSampleButton.Text = "Paste";
            PasteSampleButton.Click += PasteSampleButton_Click;
            // 
            // SampleNumUpDown
            // 
            SampleNumUpDown.Location = new Point(16, 37);
            SampleNumUpDown.Maximum = new decimal(new int[] { 31, 0, 0, 0 });
            SampleNumUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            SampleNumUpDown.Name = "SampleNumUpDown";
            SampleNumUpDown.Size = new Size(48, 23);
            SampleNumUpDown.TabIndex = 1;
            SampleNumUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            SampleNumUpDown.ValueChanging += SampleNumUpDown_ValueChanging;
            // 
            // SampleLoopUpDown
            // 
            SampleLoopUpDown.Hexadecimal = true;
            SampleLoopUpDown.LeadingZeros = 2;
            SampleLoopUpDown.Location = new Point(16, 89);
            SampleLoopUpDown.Maximum = new decimal(new int[] { 63, 0, 0, 0 });
            SampleLoopUpDown.Name = "SampleLoopUpDown";
            SampleLoopUpDown.Size = new Size(48, 23);
            SampleLoopUpDown.TabIndex = 5;
            SampleLoopUpDown.Value = new decimal(new int[] { 0, 0, 0, 0 });
            SampleLoopUpDown.ValueChanging += SampleLoopUpDown_ValueChanging;
            // 
            // SampleLengthUpDown
            // 
            SampleLengthUpDown.Hexadecimal = true;
            SampleLengthUpDown.LeadingZeros = 2;
            SampleLengthUpDown.Location = new Point(85, 37);
            SampleLengthUpDown.Maximum = new decimal(new int[] { 64, 0, 0, 0 });
            SampleLengthUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            SampleLengthUpDown.Name = "SampleLengthUpDown";
            SampleLengthUpDown.Size = new Size(48, 23);
            SampleLengthUpDown.TabIndex = 3;
            SampleLengthUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            SampleLengthUpDown.ValueChanging += SampleLengthUpDown_ValueChanging;
            // 
            // UnloopSampleButton
            // 
            UnloopSampleButton.Location = new Point(16, 128);
            UnloopSampleButton.Name = "UnloopSampleButton";
            UnloopSampleButton.Size = new Size(48, 21);
            UnloopSampleButton.TabIndex = 6;
            UnloopSampleButton.Text = "Unloop";
            UnloopSampleButton.Click += UnloopSampleButton_Click;
            // 
            // ClearSampleButton
            // 
            ClearSampleButton.Location = new Point(85, 128);
            ClearSampleButton.Name = "ClearSampleButton";
            ClearSampleButton.Size = new Size(48, 21);
            ClearSampleButton.TabIndex = 7;
            ClearSampleButton.Text = "Clear";
            ClearSampleButton.Click += ClearSampleButton_Click;
            // 
            // SampleBrowserBox
            // 
            SampleBrowserBox.Controls.Add(HideSampleBrowserButton);
            SampleBrowserBox.Controls.Add(ShowSampleBrowserButton);
            SampleBrowserBox.Font = new Font("Microsoft Sans Serif", 8.25F);
            SampleBrowserBox.Location = new Point(343, 270);
            SampleBrowserBox.Name = "SampleBrowserBox";
            SampleBrowserBox.Size = new Size(183, 239);
            SampleBrowserBox.TabIndex = 4;
            SampleBrowserBox.TabStop = false;
            // 
            // HideSampleBrowserButton
            // 
            HideSampleBrowserButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            HideSampleBrowserButton.Location = new Point(16, 216);
            HideSampleBrowserButton.Name = "HideSampleBrowserButton";
            HideSampleBrowserButton.Size = new Size(153, 12);
            HideSampleBrowserButton.TabIndex = 1;
            HideSampleBrowserButton.Text = "- - -";
            HideSampleBrowserButton.Click += HideSampleBrowserButton_Click;
            // 
            // ShowSampleBrowserButton
            // 
            ShowSampleBrowserButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            ShowSampleBrowserButton.Location = new Point(16, 32);
            ShowSampleBrowserButton.Name = "ShowSampleBrowserButton";
            ShowSampleBrowserButton.Size = new Size(153, 12);
            ShowSampleBrowserButton.TabIndex = 0;
            ShowSampleBrowserButton.Text = "- - -";
            ShowSampleBrowserButton.Click += ShowSampleBrowserButton_Click;
            // 
            // SampleOpts
            // 
            SampleOpts.Controls.Add(SampleOctaveValue);
            SampleOpts.Controls.Add(SampleOctaveLabel);
            SampleOpts.Controls.Add(SampleSeparator1);
            SampleOpts.Controls.Add(RecalcTonesButton);
            SampleOpts.Controls.Add(SampleSeparator2);
            SampleOpts.Controls.Add(SampleToneShiftAsNoteCheckBox);
            SampleOpts.Controls.Add(SampleOctaveNum);
            SampleOpts.Location = new Point(0, 504);
            SampleOpts.Name = "SampleOpts";
            SampleOpts.Size = new Size(394, 38);
            SampleOpts.TabIndex = 5;
            SampleOpts.TabStop = false;
            // 
            // SampleOctaveValue
            // 
            SampleOctaveValue.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            SampleOctaveValue.Location = new Point(221, 16);
            SampleOctaveValue.Name = "SampleOctaveValue";
            SampleOctaveValue.Size = new Size(15, 15);
            SampleOctaveValue.TabIndex = 1;
            SampleOctaveValue.Text = "3";
            // 
            // SampleOctaveLabel
            // 
            SampleOctaveLabel.Location = new Point(135, 15);
            SampleOctaveLabel.Name = "SampleOctaveLabel";
            SampleOctaveLabel.Size = new Size(97, 17);
            SampleOctaveLabel.TabIndex = 0;
            SampleOctaveLabel.Text = "Editor Octave:";
            // 
            // SampleSeparator1
            // 
            SampleSeparator1.Location = new Point(130, 9);
            SampleSeparator1.Name = "SampleSeparator1";
            SampleSeparator1.Size = new Size(1, 20);
            SampleSeparator1.TabIndex = 2;
            SampleSeparator1.TabStop = false;
            // 
            // RecalcTonesButton
            // 
            RecalcTonesButton.Location = new Point(295, 11);
            RecalcTonesButton.Name = "RecalcTonesButton";
            RecalcTonesButton.Size = new Size(91, 23);
            RecalcTonesButton.TabIndex = 3;
            RecalcTonesButton.Text = "Recalc Tones";
            // 
            // SampleSeparator2
            // 
            SampleSeparator2.Location = new Point(286, 10);
            SampleSeparator2.Name = "SampleSeparator2";
            SampleSeparator2.Size = new Size(1, 20);
            SampleSeparator2.TabIndex = 4;
            SampleSeparator2.TabStop = false;
            // 
            // SampleToneShiftAsNoteCheckBox
            // 
            SampleToneShiftAsNoteCheckBox.Location = new Point(8, 14);
            SampleToneShiftAsNoteCheckBox.Name = "SampleToneShiftAsNoteCheckBox";
            SampleToneShiftAsNoteCheckBox.Size = new Size(122, 17);
            SampleToneShiftAsNoteCheckBox.TabIndex = 1;
            SampleToneShiftAsNoteCheckBox.Text = "Tone Shift as Note";
            SampleToneShiftAsNoteCheckBox.Click += SampleToneShiftAsNoteCheckBox_Click;
            // 
            // SampleOctaveNum
            // 
            SampleOctaveNum.Location = new Point(238, 11);
            SampleOctaveNum.Maximum = new decimal(new int[] { 8, 0, 0, 0 });
            SampleOctaveNum.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            SampleOctaveNum.Name = "SampleOctaveNum";
            SampleOctaveNum.Size = new Size(41, 23);
            SampleOctaveNum.TabIndex = 0;
            SampleOctaveNum.Value = new decimal(new int[] { 1, 0, 0, 0 });
            SampleOctaveNum.ValueChanging += SampleOctaveNum_ValueChanging;
            // 
            // OrnamentsTab
            // 
            OrnamentsTab.Controls.Add(OrnamentsGrid);
            OrnamentsTab.Controls.Add(OrnamentsTestFieldBox);
            OrnamentsTab.Controls.Add(OrnamentEditBox);
            OrnamentsTab.Controls.Add(NextPrevOrnBox);
            OrnamentsTab.Controls.Add(OrnamentBox);
            OrnamentsTab.Controls.Add(OrnamentOpts);
            OrnamentsTab.Controls.Add(OrnamentsBrowserBox);
            OrnamentsTab.ImageIndex = 30;
            OrnamentsTab.Location = new Point(4, 24);
            OrnamentsTab.Name = "OrnamentsTab";
            OrnamentsTab.Size = new Size(626, 637);
            OrnamentsTab.TabIndex = 2;
            OrnamentsTab.Text = "Ornaments";
            // 
            // OrnamentsGrid
            // 
            OrnamentsGrid.AllowUserToAddRows = false;
            OrnamentsGrid.AllowUserToDeleteRows = false;
            OrnamentsGrid.AllowUserToResizeColumns = false;
            OrnamentsGrid.AllowUserToResizeRows = false;
            OrnamentsGrid.BackgroundColor = Color.White;
            OrnamentsGrid.BorderStyle = BorderStyle.None;
            OrnamentsGrid.CellBorderStyle = DataGridViewCellBorderStyle.None;
            OrnamentsGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            OrnamentsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            OrnamentsGrid.ColumnHeadersVisible = false;
            OrnamentsGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
            OrnamentsGrid.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            OrnamentsGrid.Location = new Point(0, 0);
            OrnamentsGrid.MultiSelect = false;
            OrnamentsGrid.Name = "OrnamentsGrid";
            OrnamentsGrid.RowHeadersVisible = false;
            OrnamentsGrid.ScrollBars = ScrollBars.Horizontal;
            OrnamentsGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
            OrnamentsGrid.Size = new Size(626, 64);
            OrnamentsGrid.TabIndex = 0;
            OrnamentsGrid.MouseWheel += OrnamentsGrid_MouseWheel;
            OrnamentsGrid.CellPainting += OrnamentsGrid_CellPainting;
            OrnamentsGrid.CurrentCellChanged += OrnamentsGrid_CurrentCellChanged;
            OrnamentsGrid.MouseUp += OrnamentsGrid_MouseUp;
            // 
            // OrnamentsTestFieldBox
            // 
            OrnamentsTestFieldBox.Controls.Add(LoadOrnamentButton);
            OrnamentsTestFieldBox.Controls.Add(SaveOrnamentButton);
            OrnamentsTestFieldBox.Location = new Point(0, 63);
            OrnamentsTestFieldBox.Name = "OrnamentsTestFieldBox";
            OrnamentsTestFieldBox.Size = new Size(369, 51);
            OrnamentsTestFieldBox.TabIndex = 0;
            OrnamentsTestFieldBox.TabStop = false;
            // 
            // LoadOrnamentButton
            // 
            LoadOrnamentButton.Location = new Point(245, 14);
            LoadOrnamentButton.Name = "LoadOrnamentButton";
            LoadOrnamentButton.Size = new Size(49, 27);
            LoadOrnamentButton.TabIndex = 0;
            LoadOrnamentButton.Text = "Load";
            LoadOrnamentButton.Click += LoadOrnamentButton_Click;
            // 
            // SaveOrnamentButton
            // 
            SaveOrnamentButton.Location = new Point(306, 14);
            SaveOrnamentButton.Name = "SaveOrnamentButton";
            SaveOrnamentButton.Size = new Size(49, 27);
            SaveOrnamentButton.TabIndex = 1;
            SaveOrnamentButton.Text = "Save";
            SaveOrnamentButton.Click += SaveOrnamentButton_Click;
            // 
            // OrnamentEditBox
            // 
            OrnamentEditBox.Location = new Point(0, 108);
            OrnamentEditBox.Name = "OrnamentEditBox";
            OrnamentEditBox.Size = new Size(369, 355);
            OrnamentEditBox.TabIndex = 2;
            OrnamentEditBox.TabStop = false;
            // 
            // NextPrevOrnBox
            // 
            NextPrevOrnBox.Controls.Add(NextOrnamentButton);
            NextPrevOrnBox.Controls.Add(PrevOrnamentButton);
            NextPrevOrnBox.Location = new Point(367, 63);
            NextPrevOrnBox.Name = "NextPrevOrnBox";
            NextPrevOrnBox.Size = new Size(154, 51);
            NextPrevOrnBox.TabIndex = 1;
            NextPrevOrnBox.TabStop = false;
            // 
            // NextOrnamentButton
            // 
            NextOrnamentButton.Location = new Point(64, 16);
            NextOrnamentButton.Name = "NextOrnamentButton";
            NextOrnamentButton.Size = new Size(49, 25);
            NextOrnamentButton.TabIndex = 1;
            NextOrnamentButton.Text = ">>";
            NextOrnamentButton.Click += NextOrnamentButton_Click;
            // 
            // PrevOrnamentButton
            // 
            PrevOrnamentButton.Location = new Point(8, 16);
            PrevOrnamentButton.Name = "PrevOrnamentButton";
            PrevOrnamentButton.Size = new Size(49, 25);
            PrevOrnamentButton.TabIndex = 0;
            PrevOrnamentButton.Text = "<<";
            PrevOrnamentButton.Click += PrevOrnamentButton_Click;
            // 
            // OrnamentBox
            // 
            OrnamentBox.Controls.Add(OrnamentNumLabel);
            OrnamentBox.Controls.Add(OrnamentLoopLabel);
            OrnamentBox.Controls.Add(OrnamentLengthLabel);
            OrnamentBox.Controls.Add(CopyOrnamentButton);
            OrnamentBox.Controls.Add(PasteOrnamentButton);
            OrnamentBox.Controls.Add(OrnamentNumUpDown);
            OrnamentBox.Controls.Add(OrnamentLoopUpDown);
            OrnamentBox.Controls.Add(OrnamentLenUpDown);
            OrnamentBox.Controls.Add(ClearOrnamentButton);
            OrnamentBox.Location = new Point(367, 108);
            OrnamentBox.Name = "OrnamentBox";
            OrnamentBox.Size = new Size(154, 158);
            OrnamentBox.TabIndex = 3;
            OrnamentBox.TabStop = false;
            // 
            // OrnamentNumLabel
            // 
            OrnamentNumLabel.Location = new Point(16, 18);
            OrnamentNumLabel.Name = "OrnamentNumLabel";
            OrnamentNumLabel.Size = new Size(63, 13);
            OrnamentNumLabel.TabIndex = 0;
            OrnamentNumLabel.Text = "Ornament";
            // 
            // OrnamentLoopLabel
            // 
            OrnamentLoopLabel.Location = new Point(16, 70);
            OrnamentLoopLabel.Name = "OrnamentLoopLabel";
            OrnamentLoopLabel.Size = new Size(41, 13);
            OrnamentLoopLabel.TabIndex = 1;
            OrnamentLoopLabel.Text = "Loop";
            // 
            // OrnamentLengthLabel
            // 
            OrnamentLengthLabel.Location = new Point(85, 18);
            OrnamentLengthLabel.Name = "OrnamentLengthLabel";
            OrnamentLengthLabel.Size = new Size(48, 13);
            OrnamentLengthLabel.TabIndex = 2;
            OrnamentLengthLabel.Text = "Length";
            // 
            // CopyOrnamentButton
            // 
            CopyOrnamentButton.Location = new Point(85, 70);
            CopyOrnamentButton.Name = "CopyOrnamentButton";
            CopyOrnamentButton.Size = new Size(49, 19);
            CopyOrnamentButton.TabIndex = 3;
            CopyOrnamentButton.Text = "Copy";
            CopyOrnamentButton.Click += CopyOrnamentButton_Click;
            // 
            // PasteOrnamentButton
            // 
            PasteOrnamentButton.Location = new Point(85, 91);
            PasteOrnamentButton.Name = "PasteOrnamentButton";
            PasteOrnamentButton.Size = new Size(49, 19);
            PasteOrnamentButton.TabIndex = 4;
            PasteOrnamentButton.Text = "Paste";
            PasteOrnamentButton.Click += PasteOrnamentButton_Click;
            // 
            // OrnamentNumUpDown
            // 
            OrnamentNumUpDown.Location = new Point(16, 37);
            OrnamentNumUpDown.Maximum = new decimal(new int[] { 15, 0, 0, 0 });
            OrnamentNumUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            OrnamentNumUpDown.Name = "OrnamentNumUpDown";
            OrnamentNumUpDown.Size = new Size(48, 23);
            OrnamentNumUpDown.TabIndex = 1;
            OrnamentNumUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            OrnamentNumUpDown.ValueChanging += OrnamentNumUpDown_ValueChanging;
            // 
            // OrnamentLoopUpDown
            // 
            OrnamentLoopUpDown.Hexadecimal = true;
            OrnamentLoopUpDown.LeadingZeros = 2;
            OrnamentLoopUpDown.Location = new Point(16, 89);
            OrnamentLoopUpDown.Maximum = new decimal(new int[] { 63, 0, 0, 0 });
            OrnamentLoopUpDown.Name = "OrnamentLoopUpDown";
            OrnamentLoopUpDown.Size = new Size(48, 23);
            OrnamentLoopUpDown.TabIndex = 5;
            OrnamentLoopUpDown.Value = new decimal(new int[] { 0, 0, 0, 0 });
            OrnamentLoopUpDown.ValueChanging += OrnamentLoopUpDown_ValueChanging;
            // 
            // OrnamentLenUpDown
            // 
            OrnamentLenUpDown.Hexadecimal = true;
            OrnamentLenUpDown.LeadingZeros = 2;
            OrnamentLenUpDown.Location = new Point(85, 37);
            OrnamentLenUpDown.Maximum = new decimal(new int[] { 64, 0, 0, 0 });
            OrnamentLenUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            OrnamentLenUpDown.Name = "OrnamentLenUpDown";
            OrnamentLenUpDown.Size = new Size(48, 23);
            OrnamentLenUpDown.TabIndex = 3;
            OrnamentLenUpDown.Value = new decimal(new int[] { 1, 0, 0, 0 });
            OrnamentLenUpDown.ValueChanging += OrnamentLenUpDown_ValueChanging;
            // 
            // ClearOrnamentButton
            // 
            ClearOrnamentButton.Location = new Point(16, 122);
            ClearOrnamentButton.Name = "ClearOrnamentButton";
            ClearOrnamentButton.Size = new Size(121, 21);
            ClearOrnamentButton.TabIndex = 6;
            ClearOrnamentButton.Text = "Clear Ornament";
            ClearOrnamentButton.Click += ClearOrnamentButton_Click;
            // 
            // OrnamentOpts
            // 
            OrnamentOpts.Controls.Add(OrnamentOctaveLabel);
            OrnamentOpts.Controls.Add(OrnamentOctaveValue);
            OrnamentOpts.Controls.Add(OrnamentSeperator);
            OrnamentOpts.Controls.Add(OrnamentToneShiftAsNoteCheckBox);
            OrnamentOpts.Controls.Add(OrnamentOctaveNum);
            OrnamentOpts.Location = new Point(0, 472);
            OrnamentOpts.Name = "OrnamentOpts";
            OrnamentOpts.Size = new Size(505, 38);
            OrnamentOpts.TabIndex = 5;
            OrnamentOpts.TabStop = false;
            // 
            // OrnamentOctaveLabel
            // 
            OrnamentOctaveLabel.Location = new Point(137, 16);
            OrnamentOctaveLabel.Name = "OrnamentOctaveLabel";
            OrnamentOctaveLabel.Size = new Size(80, 13);
            OrnamentOctaveLabel.TabIndex = 0;
            OrnamentOctaveLabel.Text = "Editor Octave:";
            // 
            // OrnamentOctaveValue
            // 
            OrnamentOctaveValue.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            OrnamentOctaveValue.Location = new Point(212, 18);
            OrnamentOctaveValue.Name = "OrnamentOctaveValue";
            OrnamentOctaveValue.Size = new Size(19, 13);
            OrnamentOctaveValue.TabIndex = 1;
            OrnamentOctaveValue.Text = "3";
            // 
            // OrnamentSeperator
            // 
            OrnamentSeperator.Location = new Point(130, 11);
            OrnamentSeperator.Name = "OrnamentSeperator";
            OrnamentSeperator.Size = new Size(1, 20);
            OrnamentSeperator.TabIndex = 2;
            OrnamentSeperator.TabStop = false;
            // 
            // OrnamentToneShiftAsNoteCheckBox
            // 
            OrnamentToneShiftAsNoteCheckBox.Location = new Point(8, 14);
            OrnamentToneShiftAsNoteCheckBox.Name = "OrnamentToneShiftAsNoteCheckBox";
            OrnamentToneShiftAsNoteCheckBox.Size = new Size(122, 17);
            OrnamentToneShiftAsNoteCheckBox.TabIndex = 1;
            OrnamentToneShiftAsNoteCheckBox.Text = "Tone Shift as Note";
            OrnamentToneShiftAsNoteCheckBox.Click += OrnamentToneShiftAsNoteCheckBox_Click;
            // 
            // OrnamentOctaveNum
            // 
            OrnamentOctaveNum.Location = new Point(232, 10);
            OrnamentOctaveNum.Maximum = new decimal(new int[] { 8, 0, 0, 0 });
            OrnamentOctaveNum.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            OrnamentOctaveNum.Name = "OrnamentOctaveNum";
            OrnamentOctaveNum.Size = new Size(41, 23);
            OrnamentOctaveNum.TabIndex = 0;
            OrnamentOctaveNum.Value = new decimal(new int[] { 1, 0, 0, 0 });
            OrnamentOctaveNum.ValueChanging += OrnamentOctaveNum_ValueChanging;
            // 
            // OrnamentsBrowserBox
            // 
            OrnamentsBrowserBox.Controls.Add(HideOrnamentBrowserButton);
            OrnamentsBrowserBox.Controls.Add(ShowOrnamentBrowserButton);
            OrnamentsBrowserBox.Location = new Point(367, 260);
            OrnamentsBrowserBox.Name = "OrnamentsBrowserBox";
            OrnamentsBrowserBox.Size = new Size(154, 203);
            OrnamentsBrowserBox.TabIndex = 4;
            OrnamentsBrowserBox.TabStop = false;
            // 
            // HideOrnamentBrowserButton
            // 
            HideOrnamentBrowserButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            HideOrnamentBrowserButton.Location = new Point(9, 175);
            HideOrnamentBrowserButton.Name = "HideOrnamentBrowserButton";
            HideOrnamentBrowserButton.Size = new Size(136, 12);
            HideOrnamentBrowserButton.TabIndex = 1;
            HideOrnamentBrowserButton.Text = "- - -";
            HideOrnamentBrowserButton.Click += HideOrnamentBrowserButton_Click;
            // 
            // ShowOrnamentBrowserButton
            // 
            ShowOrnamentBrowserButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            ShowOrnamentBrowserButton.Location = new Point(9, 19);
            ShowOrnamentBrowserButton.Name = "ShowOrnamentBrowserButton";
            ShowOrnamentBrowserButton.Size = new Size(136, 12);
            ShowOrnamentBrowserButton.TabIndex = 0;
            ShowOrnamentBrowserButton.Text = "- - -";
            ShowOrnamentBrowserButton.Click += ShowOrnamentBrowserButton_Click;
            // 
            // OptionsTab
            // 
            OptionsTab.Controls.Add(TrackOptsScrollBox);
            OptionsTab.ImageIndex = 21;
            OptionsTab.Location = new Point(4, 24);
            OptionsTab.Name = "OptionsTab";
            OptionsTab.Size = new Size(626, 637);
            OptionsTab.TabIndex = 3;
            OptionsTab.Text = "Options";
            // 
            // TrackOptsScrollBox
            // 
            TrackOptsScrollBox.Controls.Add(ManualIntFreq);
            TrackOptsScrollBox.Controls.Add(HelpBox);
            TrackOptsScrollBox.Controls.Add(ChipFreqBox);
            TrackOptsScrollBox.Controls.Add(IntFreqBox);
            TrackOptsScrollBox.Controls.Add(SaveHeaderBox);
            TrackOptsScrollBox.Controls.Add(VtmFeaturesBox);
            TrackOptsScrollBox.Location = new Point(0, 2);
            TrackOptsScrollBox.Name = "TrackOptsScrollBox";
            TrackOptsScrollBox.Size = new Size(545, 623);
            TrackOptsScrollBox.TabIndex = 0;
            // 
            // ManualIntFreq
            // 
            ManualIntFreq.Location = new Point(347, 380);
            ManualIntFreq.MaxLength = 8;
            ManualIntFreq.Name = "ManualIntFreq";
            ManualIntFreq.Size = new Size(52, 23);
            ManualIntFreq.TabIndex = 3;
            // 
            // HelpBox
            // 
            HelpBox.Location = new Point(8, 25);
            HelpBox.Name = "HelpBox";
            HelpBox.Size = new Size(489, 40);
            HelpBox.TabIndex = 0;
            HelpBox.TabStop = false;
            HelpBox.Visible = false;
            // 
            // ChipFreqBox
            // 
            ChipFreqBox.Columns = 2;
            ChipFreqBox.Controls.Add(ManualHz);
            ChipFreqBox.Items.Add("0.894887 MHz (NES NTSC)");
            ChipFreqBox.Items.Add("0.8313035 MHz (NES PAL)");
            ChipFreqBox.Items.Add("1.7734 MHz (ZX Spectrum)");
            ChipFreqBox.Items.Add("1.75 MHz (Pentagon 128K)");
            ChipFreqBox.Items.Add("1 MHz (Amstrad CPC)");
            ChipFreqBox.Items.Add("1.5 MHz (Vectrex console)");
            ChipFreqBox.Items.Add("2 MHz (Atari ST)");
            ChipFreqBox.Items.Add("3.5 MHz");
            ChipFreqBox.Items.Add("1520640 Hz (Natural C/Am for 4th table)");
            ChipFreqBox.Items.Add("1611062 Hz (Natural C#/A#m for 4th table)");
            ChipFreqBox.Items.Add("1706861 Hz (Natural D/Bm for 4th table)");
            ChipFreqBox.Items.Add("1808356 Hz (Natural D#/Cm for 4th table)");
            ChipFreqBox.Items.Add("1915886 Hz (Natural E/C#m for 4th table)");
            ChipFreqBox.Items.Add("2029811 Hz (Natural F/Dm for 4th table)");
            ChipFreqBox.Items.Add("2150510 Hz (Natural F#/D#m for 4th table)");
            ChipFreqBox.Items.Add("2278386 Hz (Natural G/Em for 4th table)");
            ChipFreqBox.Items.Add("2413866 Hz (Natural G#/Fm for 4th table)");
            ChipFreqBox.Items.Add("2557401 Hz (Natural A/F#m for 4th table)");
            ChipFreqBox.Items.Add("2709472 Hz (Natural A#/Gm for 4th table)");
            ChipFreqBox.Items.Add("2870586 Hz (Natural B/G#m for 4th table)");
            ChipFreqBox.Items.Add("3041280 Hz (Natural C/Am for 4th table)");
            ChipFreqBox.Items.Add("Manual (Hz)");
            ChipFreqBox.Location = new Point(8, 14);
            ChipFreqBox.Name = "ChipFreqBox";
            ChipFreqBox.Size = new Size(489, 283);
            ChipFreqBox.TabIndex = 0;
            ChipFreqBox.TabStop = false;
            ChipFreqBox.Text = " Chip Frequency For Track ";
            ChipFreqBox.Click += ChipFreqBox_Click;
            // 
            // ManualHz
            // 
            ManualHz.Location = new Point(338, 240);
            ManualHz.MaxLength = 7;
            ManualHz.Name = "ManualHz";
            ManualHz.Size = new Size(52, 23);
            ManualHz.TabIndex = 1;
            // 
            // IntFreqBox
            // 
            IntFreqBox.Columns = 2;
            IntFreqBox.Items.Add("48.828 Hz (Pentagon 128K)");
            IntFreqBox.Items.Add("50 Hz (ZX Spectrum / PAL)");
            IntFreqBox.Items.Add("60 Hz (Atari ST / NTSC)");
            IntFreqBox.Items.Add("100 Hz (Twice per INT)");
            IntFreqBox.Items.Add("200 Hz (Atari ST)");
            IntFreqBox.Items.Add("48 Hz (Non-fractional BPM)");
            IntFreqBox.Items.Add("Manual (Hz)");
            IntFreqBox.Location = new Point(8, 316);
            IntFreqBox.Name = "IntFreqBox";
            IntFreqBox.Size = new Size(489, 118);
            IntFreqBox.TabIndex = 2;
            IntFreqBox.TabStop = false;
            IntFreqBox.Text = " Interrupt Frequency For Track";
            IntFreqBox.Click += IntFreqBox_Click;
            // 
            // SaveHeadBox
            // 
            SaveHeaderBox.Items.Add("\"Vortex Tracker II 2.0 module:\" Where Possible");
            SaveHeaderBox.Items.Add("\"ProTracker 3.x Compilation Of\" Always");
            SaveHeaderBox.Location = new Point(232, 453);
            SaveHeaderBox.Name = "SaveHeaderBox";
            SaveHeaderBox.Size = new Size(265, 91);
            SaveHeaderBox.TabIndex = 5;
            SaveHeaderBox.TabStop = false;
            SaveHeaderBox.Text = " Save With Header ";
            SaveHeaderBox.Click += SaveHeaderBox_Click;
            // 
            // VtmFeaturesBox
            // 
            VtmFeaturesBox.Items.Add("Pro Tracker 3.5");
            VtmFeaturesBox.Items.Add("Vortex Tracker II (PT 3.6)");
            VtmFeaturesBox.Items.Add("Pro Tracker 3.7");
            VtmFeaturesBox.Location = new Point(8, 453);
            VtmFeaturesBox.Name = "VtmFeaturesBox";
            VtmFeaturesBox.Size = new Size(212, 91);
            VtmFeaturesBox.TabIndex = 4;
            VtmFeaturesBox.TabStop = false;
            VtmFeaturesBox.Text = " Features level ";
            VtmFeaturesBox.Click += VtmFeaturesBox_Click;
            // 
            // InfoTab
            // 
            InfoTab.Controls.Add(TrackInfoGB);
            InfoTab.ImageIndex = 43;
            InfoTab.Location = new Point(4, 24);
            InfoTab.Name = "InfoTab";
            InfoTab.Size = new Size(626, 637);
            InfoTab.TabIndex = 4;
            InfoTab.Text = "Info";
            // 
            // TrackInfoGB
            // 
            TrackInfoGB.Controls.Add(BoldButton);
            TrackInfoGB.Controls.Add(ItalicButton);
            TrackInfoGB.Controls.Add(UnderlineButton);
            TrackInfoGB.Controls.Add(ShowInfoOnLoadCheckBox);
            TrackInfoGB.Controls.Add(TrackInfoPanel);
            TrackInfoGB.Controls.Add(ViewInfoButton);
            TrackInfoGB.Location = new Point(0, -2);
            TrackInfoGB.Name = "TrackInfoGB";
            TrackInfoGB.Size = new Size(529, 545);
            TrackInfoGB.TabIndex = 0;
            TrackInfoGB.TabStop = false;
            // 
            // BoldButton
            // 
            BoldButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            BoldButton.Location = new Point(8, 16);
            BoldButton.Name = "BoldButton";
            BoldButton.Size = new Size(25, 23);
            BoldButton.TabIndex = 0;
            BoldButton.Text = "B";
            BoldButton.Click += BoldButton_Click;
            // 
            // ItalicButton
            // 
            ItalicButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            ItalicButton.Location = new Point(40, 16);
            ItalicButton.Name = "ItalicButton";
            ItalicButton.Size = new Size(25, 23);
            ItalicButton.TabIndex = 1;
            ItalicButton.Text = "I";
            ItalicButton.Click += ItalicButton_Click;
            // 
            // UnderlineButton
            // 
            UnderlineButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold | FontStyle.Underline);
            UnderlineButton.Location = new Point(72, 16);
            UnderlineButton.Name = "UnderlineButton";
            UnderlineButton.Size = new Size(25, 23);
            UnderlineButton.TabIndex = 2;
            UnderlineButton.Text = "U";
            UnderlineButton.Click += UnderlineButton_Click;
            // 
            // ShowInfoOnLoadCheckBox
            // 
            ShowInfoOnLoadCheckBox.Location = new Point(112, 20);
            ShowInfoOnLoadCheckBox.Name = "ShowInfoOnLoadCheckBox";
            ShowInfoOnLoadCheckBox.Size = new Size(217, 17);
            ShowInfoOnLoadCheckBox.TabIndex = 1;
            ShowInfoOnLoadCheckBox.Text = "Show Info When Track Is Loaded";
            ShowInfoOnLoadCheckBox.MouseUp += ShowInfoOnLoadCheckBox_MouseUp;
            // 
            // TrackInfoPanel
            // 
            TrackInfoPanel.Controls.Add(TrackInfoRTB);
            TrackInfoPanel.Location = new Point(8, 48);
            TrackInfoPanel.Name = "TrackInfoPanel";
            TrackInfoPanel.Size = new Size(513, 481);
            TrackInfoPanel.TabIndex = 2;
            TrackInfoPanel.Text = "EditPanel";
            // 
            // TrackInfoRTB
            // 
            TrackInfoRTB.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            TrackInfoRTB.BackColor = Color.FromArgb(255, 251, 240);
            TrackInfoRTB.BorderStyle = BorderStyle.None;
            TrackInfoRTB.Font = new Font("Courier New", 12F);
            TrackInfoRTB.Location = new Point(0, 0);
            TrackInfoRTB.Name = "TrackInfoRTB";
            TrackInfoRTB.ReadOnly = true;
            TrackInfoRTB.ScrollBars = RichTextBoxScrollBars.Vertical;
            TrackInfoRTB.Size = new Size(597, 481);
            TrackInfoRTB.TabIndex = 0;
            TrackInfoRTB.Text = "";
            TrackInfoRTB.KeyDown += TrackInfoRTB_KeyDown;
            TrackInfoRTB.KeyUp += TrackInfoRTB_KeyUp;
            // 
            // ViewInfoButton
            // 
            ViewInfoButton.Location = new Point(456, 16);
            ViewInfoButton.Name = "ViewInfoButton";
            ViewInfoButton.Size = new Size(57, 23);
            ViewInfoButton.TabIndex = 0;
            ViewInfoButton.Text = "View";
            ViewInfoButton.Click += ViewInfoButton_Click;
            // 
            // MuteButton
            // 
            MuteButton.Appearance = Appearance.Button;
            MuteButton.Location = new Point(456, 4);
            MuteButton.Name = "MuteButton";
            MuteButton.Size = new Size(45, 20);
            MuteButton.TabIndex = 0;
            MuteButton.Text = "Mute";
            MuteButton.TextAlign = ContentAlignment.MiddleCenter;
            MuteButton.Click += AllMute_Click;
            // 
            // SoloButton
            // 
            SoloButton.Appearance = Appearance.Button;
            SoloButton.Location = new Point(501, 4);
            SoloButton.Name = "SoloButton";
            SoloButton.Size = new Size(45, 20);
            SoloButton.TabIndex = 0;
            SoloButton.Text = "Solo";
            SoloButton.TextAlign = ContentAlignment.MiddleCenter;
            SoloButton.Click += AllSolo_Click;
            // 
            // SaveTextDialog
            // 
            SaveTextDialog.DefaultExt = "TXT";
            SaveTextDialog.Filter = "Pattern Files|*.vtp|Sample Files|*.vts|Text Files|*.txt|All Files|*.*'";
            // 
            // LoadTextDialog
            // 
            LoadTextDialog.DefaultExt = "TXT";
            LoadTextDialog.Filter = "Text Files|*.txt|All Files|*.*";
            // 
            // ShowHintTimer
            // 
            ShowHintTimer.Tick += ShowHintTimer_Tick;
            // 
            // HideHintTimer
            // 
            HideHintTimer.Tick += HideHintTimer_Tick;
            // 
            // ChangeBackupVersion
            // 
            ChangeBackupVersion.Enabled = true;
            ChangeBackupVersion.Interval = 1500000;
            ChangeBackupVersion.Tick += ChangeBackupVersion_Tick;
            // 
            // ExportWavDialog
            // 
            ExportWavDialog.DefaultExt = "wav";
            ExportWavDialog.Filter = "WAV|*.wav";
            // 
            // ExportPSGDialog
            // 
            ExportPSGDialog.DefaultExt = "psg";
            ExportPSGDialog.Filter = "PSG|*.psg";
            // 
            // ExportYMDialog
            // 
            ExportYMDialog.DefaultExt = "ym";
            ExportYMDialog.Filter = "YM|*.ym";
            // 
            // PlayStopTimer
            // 
            PlayStopTimer.Tick += PlayStopTimer_Tick;
            // 
            // TrackInfoTimer
            // 
            TrackInfoTimer.Interval = 300;
            TrackInfoTimer.Tick += TrackInfoTimer_Tick;
            // 
            // FileBrowserPopup
            // 
            FileBrowserPopup.Items.AddRange(new ToolStripItem[] { FBSaveInstrument, N1, FBRename, FBDelete, FBNewFolder, FBSetQuickAccess });
            FileBrowserPopup.Name = "FileBrowserPopup";
            FileBrowserPopup.Size = new Size(167, 120);
            // 
            // FBSaveInstrument
            // 
            FBSaveInstrument.Name = "FBSaveInstrument";
            FBSaveInstrument.Size = new Size(166, 22);
            FBSaveInstrument.Text = "Save sample here";
            FBSaveInstrument.Click += FBSaveInstrument_Click;
            // 
            // N1
            // 
            N1.Name = "N1";
            N1.Size = new Size(163, 6);
            // 
            // FBRename
            // 
            FBRename.Name = "FBRename";
            FBRename.Size = new Size(166, 22);
            FBRename.Text = "Rename";
            FBRename.Click += FBRename_Click;
            // 
            // FBDelete
            // 
            FBDelete.Name = "FBDelete";
            FBDelete.Size = new Size(166, 22);
            FBDelete.Text = "Delete";
            FBDelete.Click += FBDelete_Click;
            // 
            // FBNewFolder
            // 
            FBNewFolder.Name = "FBNewFolder";
            FBNewFolder.Size = new Size(166, 22);
            FBNewFolder.Text = "New Folder";
            FBNewFolder.Click += FBNewFolder_Click;
            // 
            // FBSetQuickAccess
            // 
            FBSetQuickAccess.Name = "FBSetQuickAccess";
            FBSetQuickAccess.Size = new Size(166, 22);
            FBSetQuickAccess.Text = "Set as User Folder";
            FBSetQuickAccess.Click += FBSetQuickAccess_Click;
            // 
            // LowLightMenu1
            // 
            LowLightMenu1.Items.AddRange(new ToolStripItem[] { Disabled1, N6, N7, N31, N431, N51, N61, N71, N81, N91 });
            LowLightMenu1.Name = "LowLightMenu1";
            LowLightMenu1.Size = new Size(133, 224);
            // 
            // Disabled1
            // 
            Disabled1.Name = "Disabled1";
            Disabled1.Size = new Size(132, 22);
            Disabled1.Text = "0: Disabled";
            Disabled1.Click += AutoLL_Click;
            // 
            // N6
            // 
            N6.Name = "N6";
            N6.Size = new Size(132, 22);
            N6.Text = "1: [x][ ]";
            N6.Click += AutoLL_Click;
            // 
            // N7
            // 
            N7.Name = "N7";
            N7.Size = new Size(132, 22);
            N7.Text = "2: [ ][x]";
            N7.Click += AutoLL_Click;
            // 
            // N31
            // 
            N31.Name = "N31";
            N31.Size = new Size(132, 22);
            N31.Text = "3: [x][ ][ ]";
            N31.Click += AutoLL_Click;
            // 
            // N431
            // 
            N431.Name = "N431";
            N431.Size = new Size(132, 22);
            N431.Text = "4: [ ][x][ ]";
            N431.Click += AutoLL_Click;
            // 
            // N51
            // 
            N51.Name = "N51";
            N51.Size = new Size(132, 22);
            N51.Text = "5: [ ][ ][x]";
            N51.Click += AutoLL_Click;
            // 
            // N61
            // 
            N61.Name = "N61";
            N61.Size = new Size(132, 22);
            N61.Text = "6: [x][ ][ ][ ]";
            N61.Click += AutoLL_Click;
            // 
            // N71
            // 
            N71.Name = "N71";
            N71.Size = new Size(132, 22);
            N71.Text = "7: [ ][x][ ][ ]";
            N71.Click += AutoLL_Click;
            // 
            // N81
            // 
            N81.Name = "N81";
            N81.Size = new Size(132, 22);
            N81.Text = "8: [ ][ ][x][ ]";
            N81.Click += AutoLL_Click;
            // 
            // N91
            // 
            N91.Name = "N91";
            N91.Size = new Size(132, 22);
            N91.Text = "9: [ ][ ][ ][x]";
            N91.Click += AutoLL_Click;
            // 
            // UpdateTimer
            // 
            UpdateTimer.Tick += UpdateTimer_Tick;
            // 
            // OrnamentsMenu
            // 
            OrnamentsMenu.Items.AddRange(new ToolStripItem[] { CutOrnament1, CopyOrnament1, PasteOrnament1, N2, ClearOrnament1, N5, SwapOrnaments1, PackOrnaments1 });
            OrnamentsMenu.Name = "OrnamentsMenu";
            OrnamentsMenu.Size = new Size(165, 148);
            // 
            // CutOrnament1
            // 
            CutOrnament1.Name = "CutOrnament1";
            CutOrnament1.Size = new Size(164, 22);
            CutOrnament1.Text = "Cut Ornament";
            CutOrnament1.Click += CutOrnament1_Click;
            // 
            // CopyOrnament1
            // 
            CopyOrnament1.Name = "CopyOrnament1";
            CopyOrnament1.Size = new Size(164, 22);
            CopyOrnament1.Text = "Copy Ornament";
            CopyOrnament1.Click += CopyOrnament1_Click;
            // 
            // PasteOrnament1
            // 
            PasteOrnament1.Name = "PasteOrnament1";
            PasteOrnament1.Size = new Size(164, 22);
            PasteOrnament1.Text = "Paste Ornament";
            PasteOrnament1.Click += PasteOrnament1_Click;
            // 
            // N2
            // 
            N2.Name = "N2";
            N2.Size = new Size(161, 6);
            // 
            // ClearOrnament1
            // 
            ClearOrnament1.Name = "ClearOrnament1";
            ClearOrnament1.Size = new Size(164, 22);
            ClearOrnament1.Text = "Clear Ornament";
            ClearOrnament1.Click += ClearOrnament1_Click;
            // 
            // N5
            // 
            N5.Name = "N5";
            N5.Size = new Size(161, 6);
            // 
            // SwapOrnaments1
            // 
            SwapOrnaments1.Name = "SwapOrnaments1";
            SwapOrnaments1.Size = new Size(164, 22);
            SwapOrnaments1.Text = "Swap Ornaments";
            SwapOrnaments1.Click += SwapOrnaments1_Click;
            // 
            // PackOrnaments1
            // 
            PackOrnaments1.Name = "PackOrnaments1";
            PackOrnaments1.Size = new Size(164, 22);
            PackOrnaments1.Text = "Pack Ornaments";
            PackOrnaments1.Click += PackOrnaments1_Click;
            // 
            // SamplesMenu
            // 
            SamplesMenu.Items.AddRange(new ToolStripItem[] { CutSample1, CopySample1, PasteSample1, N3, ClearSample1, N4, SwapSamples1, PackSamples1 });
            SamplesMenu.Name = "SamplesMenu";
            SamplesMenu.Size = new Size(150, 148);
            // 
            // CutSample1
            // 
            CutSample1.Name = "CutSample1";
            CutSample1.Size = new Size(149, 22);
            CutSample1.Text = "Cut Sample";
            CutSample1.Click += CutSample1_Click;
            // 
            // CopySample1
            // 
            CopySample1.Name = "CopySample1";
            CopySample1.Size = new Size(149, 22);
            CopySample1.Text = "Copy Sample";
            CopySample1.Click += CopySample1_Click;
            // 
            // PasteSample1
            // 
            PasteSample1.Name = "PasteSample1";
            PasteSample1.Size = new Size(149, 22);
            PasteSample1.Text = "Paste Sample";
            PasteSample1.Click += PasteSample1_Click;
            // 
            // N3
            // 
            N3.Name = "N3";
            N3.Size = new Size(146, 6);
            // 
            // ClearSample1
            // 
            ClearSample1.Name = "ClearSample1";
            ClearSample1.Size = new Size(149, 22);
            ClearSample1.Text = "Clear Sample";
            ClearSample1.Click += ClearSample1_Click;
            // 
            // N4
            // 
            N4.Name = "N4";
            N4.Size = new Size(146, 6);
            // 
            // SwapSamples1
            // 
            SwapSamples1.Name = "SwapSamples1";
            SwapSamples1.Size = new Size(149, 22);
            SwapSamples1.Text = "Swap Samples";
            SwapSamples1.Click += SwapSamples1_Click;
            // 
            // PackSamples1
            // 
            PackSamples1.Name = "PackSamples1";
            PackSamples1.Size = new Size(149, 22);
            PackSamples1.Text = "Pack Samples";
            PackSamples1.Click += PackSamples1_Click;
            // 
            // ChildForm
            // 
            AutoScroll = true;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(634, 665);
            Controls.Add(SoloButton);
            Controls.Add(MuteButton);
            Controls.Add(TopBackgroundPanel);
            Controls.Add(TabControl);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Location = new Point(826, 63);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(634, 0);
            Name = "ChildForm";
            Text = "Module";
            Activated += ChildWin_Activated;
            FormClosing += ChildWin_FormClosing;
            FormClosed += ChildWin_FormClosed;
            Load += ChildWin_Load;
            Paint += ChildWin_Paint;
            DoubleClick += ChildWin_DoubleClick;
            KeyDown += ChildWin_KeyDown;
            Resize += ChildWin_Resize;
            TabControl.ResumeLayout(false);
            PatternsTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PositionsGrid).EndInit();
            InterfaceBox.ResumeLayout(false);
            AutoHLBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)StepHLUpDown).EndInit();
            ChannelABox.ResumeLayout(false);
            ChannelBBox.ResumeLayout(false);
            ChannelCBox.ResumeLayout(false);
            PatternBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PatternNumUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)PatternLenUpDown).EndInit();
            SpeedBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)SpeedBpmUpDown).EndInit();
            OctaveBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)OctaveUpDown).EndInit();
            AutoStepBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)AutoStepUpDown).EndInit();
            AutoEnvBox.ResumeLayout(false);
            ToneTableBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)ToneTableUpDown).EndInit();
            JoinTracksBox.ResumeLayout(false);
            TrackInfoBox.ResumeLayout(false);
            TrackInfoBox.PerformLayout();
            SamplesTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)SamplesGrid).EndInit();
            SamplesTestFieldBox.ResumeLayout(false);
            NextPrevSampleBox.ResumeLayout(false);
            SampleBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)SampleNumUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)SampleLoopUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)SampleLengthUpDown).EndInit();
            SampleBrowserBox.ResumeLayout(false);
            SampleOpts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)SampleOctaveNum).EndInit();
            OrnamentsTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)OrnamentsGrid).EndInit();
            OrnamentsTestFieldBox.ResumeLayout(false);
            NextPrevOrnBox.ResumeLayout(false);
            OrnamentBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)OrnamentNumUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)OrnamentLoopUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)OrnamentLenUpDown).EndInit();
            OrnamentOpts.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)OrnamentOctaveNum).EndInit();
            OrnamentsBrowserBox.ResumeLayout(false);
            OptionsTab.ResumeLayout(false);
            TrackOptsScrollBox.ResumeLayout(false);
            TrackOptsScrollBox.PerformLayout();
            ChipFreqBox.ResumeLayout(false);
            ChipFreqBox.PerformLayout();
            InfoTab.ResumeLayout(false);
            TrackInfoGB.ResumeLayout(false);
            TrackInfoPanel.ResumeLayout(false);
            FileBrowserPopup.ResumeLayout(false);
            LowLightMenu1.ResumeLayout(false);
            OrnamentsMenu.ResumeLayout(false);
            SamplesMenu.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        public MyDataGridView PositionsGrid;
        public GroupBox HelpBox;
        public System.Windows.Forms.Panel TopBackgroundPanel;
        public MyTabControl TabControl;
        public System.Windows.Forms.TabPage PatternsTab;
        public System.Windows.Forms.CheckBox MuteButton;
        public System.Windows.Forms.CheckBox SoloButton;
        public System.Windows.Forms.GroupBox AutoHLBox;
        public System.Windows.Forms.CheckBox AutoHLCheckBox;
        public MyNumericUpDown StepHLUpDown;
        public System.Windows.Forms.GroupBox ChannelABox;
        public System.Windows.Forms.CheckBox ChannelAMute;
        public System.Windows.Forms.CheckBox ChannelATone;
        public System.Windows.Forms.CheckBox ChannelANoise;
        public System.Windows.Forms.CheckBox ChannelAEnvelope;
        public System.Windows.Forms.CheckBox ChannelASolo;
        public System.Windows.Forms.GroupBox ChannelBBox;
        public System.Windows.Forms.CheckBox ChannelBMute;
        public System.Windows.Forms.CheckBox ChannelBTone;
        public System.Windows.Forms.CheckBox ChannelBNoise;
        public System.Windows.Forms.CheckBox ChannelBEnvelope;
        public System.Windows.Forms.CheckBox ChannelBSolo;
        public System.Windows.Forms.GroupBox ChannelCBox;
        public System.Windows.Forms.CheckBox ChannelCMute;
        public System.Windows.Forms.CheckBox ChannelCTone;
        public System.Windows.Forms.CheckBox ChannelCNoise;
        public System.Windows.Forms.CheckBox ChannelCEnvelope;
        public System.Windows.Forms.CheckBox ChannelCSolo;
        public System.Windows.Forms.GroupBox TrackInfoBox;
        public System.Windows.Forms.Label ByTextBox;
        public System.Windows.Forms.TextBox TitleTextBox;
        public System.Windows.Forms.TextBox AuthorTextBox;
        public System.Windows.Forms.GroupBox PatternBox;
        public System.Windows.Forms.Label PatternNumberLabel;
        public System.Windows.Forms.Label PatternLengthLabel;
        public System.Windows.Forms.Button LoadPatternButton;
        public System.Windows.Forms.Button SavePatternButton;
        public MyNumericUpDown PatternNumUpDown;
        public MyNumericUpDown PatternLenUpDown;
        public System.Windows.Forms.GroupBox SpeedBox;
        public System.Windows.Forms.Label SpeedBPMLabel;
        public MyNumericUpDown SpeedBpmUpDown;
        public System.Windows.Forms.GroupBox OctaveBox;
        public System.Windows.Forms.Label OctaveLabel;
        public MyNumericUpDown OctaveUpDown;
        public System.Windows.Forms.GroupBox AutoStepBox;
        public System.Windows.Forms.CheckBox AutoStepButton;
        public MyNumericUpDown AutoStepUpDown;
        public System.Windows.Forms.GroupBox AutoEnvBox;
        public System.Windows.Forms.CheckBox AutoEnvButton;
        public System.Windows.Forms.Button AutoEnv0Button;
        public System.Windows.Forms.Button AutoEnvToggleButton;
        public System.Windows.Forms.Button AutoEnv1Button;
        public System.Windows.Forms.GroupBox InterfaceBox;
        public System.Windows.Forms.CheckBox EnvelopeAsNoteCheckBox;
        public System.Windows.Forms.CheckBox UseLastNoteParamsCheckBox;
        public System.Windows.Forms.CheckBox MoveBetweenPatternsCheckBox;
        public System.Windows.Forms.GroupBox ToneTableBox;
        public System.Windows.Forms.Label ToneTableLabel;
        public MyNumericUpDown ToneTableUpDown;
        public System.Windows.Forms.GroupBox JoinTracksBox;
        public System.Windows.Forms.Button JoinTracksButton;
        public System.Windows.Forms.TabPage SamplesTab;
        public System.Windows.Forms.GroupBox SampleOpts;
        public System.Windows.Forms.Label SampleOctaveLabel;
        public System.Windows.Forms.Label SampleOctaveValue;
        public System.Windows.Forms.GroupBox SampleSeparator1;
        public System.Windows.Forms.Button RecalcTonesButton;
        public System.Windows.Forms.GroupBox SampleSeparator2;
        public System.Windows.Forms.CheckBox SampleToneShiftAsNoteCheckBox;
        public MyNumericUpDown SampleOctaveNum;
        public System.Windows.Forms.GroupBox SampleBrowserBox;
        public System.Windows.Forms.Button HideSampleBrowserButton;
        public System.Windows.Forms.Button ShowSampleBrowserButton;
        public System.Windows.Forms.GroupBox SampleBox;
        public System.Windows.Forms.Label SampleNumLabel;
        public System.Windows.Forms.Label SampleLoopLabel;
        public System.Windows.Forms.Label SampleLengthLabel;
        public System.Windows.Forms.Button CopySampleButton;
        public System.Windows.Forms.Button PasteSampleButton;
        public MyNumericUpDown SampleNumUpDown;
        public MyNumericUpDown SampleLoopUpDown;
        public MyNumericUpDown SampleLengthUpDown;
        public System.Windows.Forms.Button UnloopSampleButton;
        public System.Windows.Forms.Button ClearSampleButton;
        public System.Windows.Forms.GroupBox NextPrevSampleBox;
        public System.Windows.Forms.Button NextSampleButton;
        public System.Windows.Forms.Button PrevSampleButton;
        public System.Windows.Forms.GroupBox SampleEditBox;
        public System.Windows.Forms.GroupBox SamplesTestFieldBox;
        public System.Windows.Forms.Button SaveSampleButton;
        public System.Windows.Forms.Button LoadSampleButton;
        public System.Windows.Forms.TabPage OrnamentsTab;
        public System.Windows.Forms.GroupBox OrnamentOpts;
        public System.Windows.Forms.Label OrnamentOctaveLabel;
        public System.Windows.Forms.Label OrnamentOctaveValue;
        public System.Windows.Forms.GroupBox OrnamentSeperator;
        public System.Windows.Forms.CheckBox OrnamentToneShiftAsNoteCheckBox;
        public MyNumericUpDown OrnamentOctaveNum;
        public System.Windows.Forms.GroupBox OrnamentsBrowserBox;
        public System.Windows.Forms.Button HideOrnamentBrowserButton;
        public System.Windows.Forms.Button ShowOrnamentBrowserButton;
        public System.Windows.Forms.GroupBox OrnamentEditBox;
        public System.Windows.Forms.GroupBox OrnamentsTestFieldBox;
        public System.Windows.Forms.Button LoadOrnamentButton;
        public System.Windows.Forms.Button SaveOrnamentButton;
        public System.Windows.Forms.GroupBox OrnamentBox;
        public System.Windows.Forms.Label OrnamentNumLabel;
        public System.Windows.Forms.Label OrnamentLoopLabel;
        public System.Windows.Forms.Label OrnamentLengthLabel;
        public System.Windows.Forms.Button CopyOrnamentButton;
        public System.Windows.Forms.Button PasteOrnamentButton;
        public MyNumericUpDown OrnamentNumUpDown;
        public MyNumericUpDown OrnamentLoopUpDown;
        public MyNumericUpDown OrnamentLenUpDown;
        public System.Windows.Forms.Button ClearOrnamentButton;
        public System.Windows.Forms.GroupBox NextPrevOrnBox;
        public System.Windows.Forms.Button NextOrnamentButton;
        public System.Windows.Forms.Button PrevOrnamentButton;
        public System.Windows.Forms.TabPage OptionsTab;
        public System.Windows.Forms.Panel TrackOptsScrollBox;
        public RadioGroup ChipFreqBox;
        public RadioGroup IntFreqBox;
        public RadioGroup SaveHeaderBox;
        public RadioGroup VtmFeaturesBox;
        public System.Windows.Forms.TextBox ManualHz;
        public System.Windows.Forms.TextBox ManualIntFreq;
        public System.Windows.Forms.TabPage InfoTab;
        public System.Windows.Forms.GroupBox TrackInfoGB;
        public System.Windows.Forms.Button BoldButton;
        public System.Windows.Forms.Button ItalicButton;
        public System.Windows.Forms.Button UnderlineButton;
        public System.Windows.Forms.CheckBox ShowInfoOnLoadCheckBox;
        public System.Windows.Forms.Panel TrackInfoPanel;
        public System.Windows.Forms.RichTextBox TrackInfoRTB;
        public System.Windows.Forms.Button ViewInfoButton;
        public System.Windows.Forms.SaveFileDialog SaveTextDialog;
        public System.Windows.Forms.OpenFileDialog LoadTextDialog;
        public System.Windows.Forms.Timer ShowHintTimer;
        public System.Windows.Forms.Timer HideHintTimer;
        public System.Windows.Forms.Timer ChangeBackupVersion;
        public System.Windows.Forms.SaveFileDialog ExportWavDialog;
        public System.Windows.Forms.SaveFileDialog ExportPSGDialog;
        public System.Windows.Forms.SaveFileDialog ExportYMDialog;
        public System.Windows.Forms.Timer PlayStopTimer;
        public System.Windows.Forms.Timer TrackInfoTimer;
        public System.Windows.Forms.ContextMenuStrip FileBrowserPopup;
        public System.Windows.Forms.ToolStripMenuItem FBSaveInstrument;
        public System.Windows.Forms.ToolStripSeparator N1;
        public System.Windows.Forms.ToolStripMenuItem FBRename;
        public System.Windows.Forms.ToolStripMenuItem FBDelete;
        public System.Windows.Forms.ToolStripMenuItem FBNewFolder;
        public System.Windows.Forms.ToolStripMenuItem FBSetQuickAccess;
        public System.Windows.Forms.ToolTip ToolTip = null;
        public Button DisconnectButton;
        public Label JoinLabel;
        private ContextMenuStrip LowLightMenu1;
        private ToolStripMenuItem Disabled1;
        private ToolStripMenuItem N6;
        private ToolStripMenuItem N7;
        private ToolStripMenuItem N31;
        private ToolStripMenuItem N431;
        private ToolStripMenuItem N51;
        private ToolStripMenuItem N61;
        private ToolStripMenuItem N71;
        private ToolStripMenuItem N81;
        private ToolStripMenuItem N91;
        public MyDataGridView SamplesGrid;
        public MyDataGridView OrnamentsGrid;
        public Button AutoLL;
        private System.Windows.Forms.Timer UpdateTimer;
        private ContextMenuStrip OrnamentsMenu;
        private ToolStripMenuItem CutOrnament1;
        private ToolStripMenuItem CopyOrnament1;
        private ToolStripMenuItem PasteOrnament1;
        private ToolStripSeparator N2;
        private ToolStripMenuItem ClearOrnament1;
        private ToolStripSeparator N5;
        private ToolStripMenuItem SwapOrnaments1;
        private ToolStripMenuItem PackOrnaments1;
        private ContextMenuStrip SamplesMenu;
        private ToolStripMenuItem CutSample1;
        private ToolStripMenuItem CopySample1;
        private ToolStripMenuItem PasteSample1;
        private ToolStripSeparator N3;
        private ToolStripMenuItem ClearSample1;
        private ToolStripSeparator N4;
        private ToolStripMenuItem SwapSamples1;
        private ToolStripMenuItem PackSamples1;
        private ImageList ImageList1;
    }
}
