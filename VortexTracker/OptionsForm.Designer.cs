using System;
using System.Windows.Forms;
using VortexTracker.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace VortexTracker
{
    partial class OptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            ColBackground = new Panel();
            ColText = new Panel();
            ColSelLineBackground = new Panel();
            ColHighlBackground = new Panel();
            ColHighlText = new Panel();
            ColLineNum = new Panel();
            ColEnvelope = new Panel();
            ColNoise = new Panel();
            ColNote = new Panel();
            ColNoteParams = new Panel();
            ColNoteCommands = new Panel();
            ColOutBackground = new Panel();
            ColOutText = new Panel();
            ColOutHlBackground = new Panel();
            ColSeparators = new Panel();
            ColOutSeparators = new Panel();
            ColSelLineText = new Panel();
            ColSelLineNum = new Panel();
            ColSelEnvelope = new Panel();
            ColSelNoise = new Panel();
            ColSelNote = new Panel();
            ColSelNoteParams = new Panel();
            ColSelNoteCommands = new Panel();
            ColSamOrnBackground = new Panel();
            ColSamOrnText = new Panel();
            ColSamOrnLineNum = new Panel();
            ColSamNoise = new Panel();
            ColSamOrnSeparators = new Panel();
            ColSamOrnTone = new Panel();
            ColFullScreenBackground = new Panel();
            ColSamOrnSelBackground = new Panel();
            ColSamOrnSelText = new Panel();
            ColSamSelNoise = new Panel();
            ColSamOrnSelTone = new Panel();
            ColSamOrnSelLineNum = new Panel();
            ColHighlLineNum = new Panel();
            OptionsTabControl = new TabControl();
            CurWinds = new TabPage();
            InterfaceBox = new GroupBox();
            DecNumbersLines = new CheckBox();
            DecNumbersNoise = new CheckBox();
            HighlightSpeedPosition = new CheckBox();
            DisablePatSeparators = new CheckBox();
            DisableHintsOpt = new CheckBox();
            DisableCtrlClickOpt = new CheckBox();
            DisableInfoWinOpt = new CheckBox();
            BackupBox = new GroupBox();
            BackupEveryMins = new NumericUpDown();
            EveryLabel = new Label();
            MinutesLabel = new Label();
            BackupFilenameLabel = new Label();
            AutoSaveBackups = new CheckBox();
            AppPriorityBox = new RadioGroup();
            StartupBox = new GroupBox();
            WhenVortexStartsLabel = new Label();
            TemplPathLab = new Label();
            StartsAction = new System.Windows.Forms.ComboBox();
            TemplateSong = new System.Windows.Forms.TextBox();
            BrowseTemplate = new System.Windows.Forms.Button();
            FreqTableBox = new GroupBox();
            FrequencyTableLabel = new Label();
            TableName = new Label();
            DefaultFreqTableUpDown = new NumericUpDown();
            ColorThemesTab = new TabPage();
            ThemesBox = new GroupBox();
            ColorThemesList = new ListBox();
            LoadThemeButton = new System.Windows.Forms.Button();
            SaveThemeButton = new System.Windows.Forms.Button();
            DeleteThemeButton = new System.Windows.Forms.Button();
            CopyThemeButton = new System.Windows.Forms.Button();
            RenameThemeButton = new System.Windows.Forms.Button();
            ThemesOptionsBox = new GroupBox();
            Seperator12 = new GroupBox();
            DefinitionLabel = new Label();
            CurrentPatternLabel = new Label();
            NextPrevPatternLabel = new Label();
            BackgroundLabel = new Label();
            TextLabel = new Label();
            LineNumbersLabel = new Label();
            EnvelopeLabel = new Label();
            NoiseLabel = new Label();
            NoteLabel = new Label();
            NoteParamsLabel = new Label();
            NoteCommandsLabel = new Label();
            SeparatorsLabel = new Label();
            Seperator1 = new GroupBox();
            Seperator2 = new GroupBox();
            Seperator3 = new GroupBox();
            Seperator4 = new GroupBox();
            Seperator5 = new GroupBox();
            Seperator6 = new GroupBox();
            Seperator7 = new GroupBox();
            Seperator8 = new GroupBox();
            SampleOrnamentLabel = new Label();
            ToneShiftLabel = new Label();
            FullScreenBackgroundLabel = new Label();
            Seperator14 = new GroupBox();
            WindowThemeLabel = new Label();
            Seperator13 = new GroupBox();
            WinColorsBox = new System.Windows.Forms.ComboBox();
            Background1Panel = new Panel();
            Background2Panel = new Panel();
            Background3Panel = new Panel();
            Background4Panel = new Panel();
            Background5Panel = new Panel();
            Background6Panel = new Panel();
            Background7Panel = new Panel();
            Background8Panel = new Panel();
            Background9Panel = new Panel();
            Background10Panel = new Panel();
            Background11Panel = new Panel();
            TableHeader = new GroupBox();
            TableBottom = new GroupBox();
            FontBox = new GroupBox();
            FontBoldButton = new CheckBox();
            PositionsLabel = new Label();
            FontsList = new ListBox();
            FontSizeUpDown = new NumericUpDown();
            DecPositionsSize = new System.Windows.Forms.Button();
            IncPositionsSize = new System.Windows.Forms.Button();
            AYEmu = new TabPage();
            ChipFreqTextBox = new System.Windows.Forms.TextBox();
            AyumiDCFiltBox = new GroupBox();
            DCCutOffLab = new Label();
            DCCutOffTrackBar = new System.Windows.Forms.TrackBar();
            DCOff = new RadioButton();
            DCAyumi = new RadioButton();
            DCWbcbz7 = new RadioButton();
            HeardAfterLabel = new Label();
            HeardAfterMSLabel = new Label();
            SoundChipBox = new RadioGroup();
            IntFreqBox = new RadioGroup();
            IntFrequencyTextBox = new System.Windows.Forms.TextBox();
            SoundEngineBox = new RadioGroup();
            ChipFreqBox = new RadioGroup();
            DownsamplingBox = new GroupBox();
            LoLabel = new Label();
            HiLabel = new Label();
            FilterCheckBox = new CheckBox();
            FilterNKTrackBar = new System.Windows.Forms.TrackBar();
            PanningBox = new GroupBox();
            APanLabel = new Label();
            BPanLabel = new Label();
            CPanLabel = new Label();
            APanTrackBar = new System.Windows.Forms.TrackBar();
            BPanTrackBar = new System.Windows.Forms.TrackBar();
            CPanTrackBar = new System.Windows.Forms.TrackBar();
            APanTextBox = new System.Windows.Forms.TextBox();
            BPanTextBox = new System.Windows.Forms.TextBox();
            CPanTextBox = new System.Windows.Forms.TextBox();
            ChanMapBox = new RadioGroup();
            WOAPITAB = new TabPage();
            StopButton = new System.Windows.Forms.Button();
            MIDIKeyboardBox = new GroupBox();
            MIDINextDeviceButton = new System.Windows.Forms.Button();
            MIDIPrevDeviceButton = new System.Windows.Forms.Button();
            MIDIStopButton = new System.Windows.Forms.Button();
            MIDIDeviceName = new System.Windows.Forms.TextBox();
            SampleRateBox = new RadioGroup();
            BitRateBox = new RadioGroup();
            ChannelsBox = new RadioGroup();
            BuffersBox = new GroupBox();
            BufferCountLabel = new Label();
            BufferLengthValue = new Label();
            BufferCountValue = new Label();
            TotalLengthLabel = new Label();
            TotalLengthValue = new Label();
            BufferLengthLabel = new Label();
            BufferLengthTrackBar = new System.Windows.Forms.TrackBar();
            BufferCountTrackBar = new System.Windows.Forms.TrackBar();
            WaveOutBox = new GroupBox();
            WaveOutTextBox = new System.Windows.Forms.TextBox();
            WaveOutGetDeviceListButton = new System.Windows.Forms.Button();
            WaveOutDeviceCombo = new System.Windows.Forms.ComboBox();
            HotKeys = new TabPage();
            HotKeysBox = new GroupBox();
            HotKeyList = new System.Windows.Forms.ListView();
            HotKeyHeader1 = new ColumnHeader();
            HotKeyHeader2 = new ColumnHeader();
            OpMod = new TabPage();
            SaveHeadBox = new RadioGroup();
            FeaturesLevelBox = new RadioGroup();
            FileAssocBox = new GroupBox();
            FileAssocList = new System.Windows.Forms.ListView();
            FileAssocHeader1 = new ColumnHeader();
            FileAssocHeader2 = new ColumnHeader();
            AllFileAssoc = new System.Windows.Forms.Button();
            NoneFileAssoc = new System.Windows.Forms.Button();
            _column_3 = new ColumnHeader();
            _column_4 = new ColumnHeader();
            _column_5 = new ColumnHeader();
            _column_6 = new ColumnHeader();
            OkButton = new System.Windows.Forms.Button();
            CancelButton = new System.Windows.Forms.Button();
            SaveThemeDialog = new SaveFileDialog();
            LoadThemeDialog = new OpenFileDialog();
            TemplateDialog = new OpenFileDialog();
            OptionsTabControl.SuspendLayout();
            CurWinds.SuspendLayout();
            InterfaceBox.SuspendLayout();
            BackupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)BackupEveryMins).BeginInit();
            StartupBox.SuspendLayout();
            FreqTableBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DefaultFreqTableUpDown).BeginInit();
            ColorThemesTab.SuspendLayout();
            ThemesBox.SuspendLayout();
            ThemesOptionsBox.SuspendLayout();
            FontBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)FontSizeUpDown).BeginInit();
            AYEmu.SuspendLayout();
            AyumiDCFiltBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DCCutOffTrackBar).BeginInit();
            IntFreqBox.SuspendLayout();
            DownsamplingBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)FilterNKTrackBar).BeginInit();
            PanningBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)APanTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BPanTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)CPanTrackBar).BeginInit();
            WOAPITAB.SuspendLayout();
            MIDIKeyboardBox.SuspendLayout();
            BuffersBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)BufferLengthTrackBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BufferCountTrackBar).BeginInit();
            WaveOutBox.SuspendLayout();
            HotKeys.SuspendLayout();
            HotKeysBox.SuspendLayout();
            OpMod.SuspendLayout();
            FileAssocBox.SuspendLayout();
            SuspendLayout();
            // 
            // ColBackground
            // 
            ColBackground.BorderStyle = BorderStyle.FixedSingle;
            ColBackground.Cursor = Cursors.Hand;
            ColBackground.Location = new Point(154, 59);
            ColBackground.Name = "ColBackground";
            ColBackground.Size = new Size(25, 18);
            ColBackground.TabIndex = 5;
            toolTip1.SetToolTip(ColBackground, "Main Background");
            ColBackground.MouseDown += ColBackground_MouseDown;
            // 
            // ColText
            // 
            ColText.BorderStyle = BorderStyle.FixedSingle;
            ColText.Cursor = Cursors.Hand;
            ColText.Location = new Point(154, 83);
            ColText.Name = "ColText";
            ColText.Size = new Size(25, 18);
            ColText.TabIndex = 13;
            toolTip1.SetToolTip(ColText, "Text (Dots)");
            ColText.MouseDown += ColText_MouseDown;
            // 
            // ColSelLineBackground
            // 
            ColSelLineBackground.BorderStyle = BorderStyle.FixedSingle;
            ColSelLineBackground.Cursor = Cursors.Hand;
            ColSelLineBackground.Location = new Point(186, 59);
            ColSelLineBackground.Name = "ColSelLineBackground";
            ColSelLineBackground.Size = new Size(25, 18);
            ColSelLineBackground.TabIndex = 26;
            toolTip1.SetToolTip(ColSelLineBackground, "Selected Line Background");
            ColSelLineBackground.MouseDown += ColSelLineBackground_MouseDown;
            // 
            // ColHighlBackground
            // 
            ColHighlBackground.BorderStyle = BorderStyle.FixedSingle;
            ColHighlBackground.Cursor = Cursors.Hand;
            ColHighlBackground.Location = new Point(218, 59);
            ColHighlBackground.Name = "ColHighlBackground";
            ColHighlBackground.Size = new Size(25, 18);
            ColHighlBackground.TabIndex = 27;
            toolTip1.SetToolTip(ColHighlBackground, "Highlighted Line Background");
            ColHighlBackground.MouseDown += ColHighlBackground_MouseDown;
            // 
            // ColHighlText
            // 
            ColHighlText.BorderStyle = BorderStyle.FixedSingle;
            ColHighlText.Cursor = Cursors.Hand;
            ColHighlText.Location = new Point(218, 83);
            ColHighlText.Name = "ColHighlText";
            ColHighlText.Size = new Size(25, 18);
            ColHighlText.TabIndex = 28;
            toolTip1.SetToolTip(ColHighlText, "Highlighted Text (Dots)");
            ColHighlText.MouseDown += ColHighlText_MouseDown;
            // 
            // ColLineNum
            // 
            ColLineNum.BorderStyle = BorderStyle.FixedSingle;
            ColLineNum.Cursor = Cursors.Hand;
            ColLineNum.Location = new Point(154, 107);
            ColLineNum.Name = "ColLineNum";
            ColLineNum.Size = new Size(25, 18);
            ColLineNum.TabIndex = 29;
            toolTip1.SetToolTip(ColLineNum, "Line Numbers");
            ColLineNum.MouseDown += ColLineNum_MouseDown;
            // 
            // ColEnvelope
            // 
            ColEnvelope.BorderStyle = BorderStyle.FixedSingle;
            ColEnvelope.Cursor = Cursors.Hand;
            ColEnvelope.Location = new Point(154, 131);
            ColEnvelope.Name = "ColEnvelope";
            ColEnvelope.Size = new Size(25, 18);
            ColEnvelope.TabIndex = 30;
            toolTip1.SetToolTip(ColEnvelope, "Envelope");
            ColEnvelope.MouseDown += ColEnvelope_MouseDown;
            // 
            // ColNoise
            // 
            ColNoise.BorderStyle = BorderStyle.FixedSingle;
            ColNoise.Cursor = Cursors.Hand;
            ColNoise.Location = new Point(154, 155);
            ColNoise.Name = "ColNoise";
            ColNoise.Size = new Size(25, 18);
            ColNoise.TabIndex = 31;
            toolTip1.SetToolTip(ColNoise, "Noise");
            ColNoise.MouseDown += ColNoise_MouseDown;
            // 
            // ColNote
            // 
            ColNote.BorderStyle = BorderStyle.FixedSingle;
            ColNote.Cursor = Cursors.Hand;
            ColNote.Location = new Point(154, 179);
            ColNote.Name = "ColNote";
            ColNote.Size = new Size(25, 18);
            ColNote.TabIndex = 32;
            toolTip1.SetToolTip(ColNote, "Note");
            ColNote.MouseDown += ColNote_MouseDown;
            // 
            // ColNoteParams
            // 
            ColNoteParams.BorderStyle = BorderStyle.FixedSingle;
            ColNoteParams.Cursor = Cursors.Hand;
            ColNoteParams.Location = new Point(154, 203);
            ColNoteParams.Name = "ColNoteParams";
            ColNoteParams.Size = new Size(25, 18);
            ColNoteParams.TabIndex = 33;
            toolTip1.SetToolTip(ColNoteParams, "Note Params (Sample, Envelope, Ornament, Volume)");
            ColNoteParams.MouseDown += ColNoteParams_MouseDown;
            // 
            // ColNoteCommands
            // 
            ColNoteCommands.BorderStyle = BorderStyle.FixedSingle;
            ColNoteCommands.Cursor = Cursors.Hand;
            ColNoteCommands.Location = new Point(154, 227);
            ColNoteCommands.Name = "ColNoteCommands";
            ColNoteCommands.Size = new Size(25, 18);
            ColNoteCommands.TabIndex = 34;
            toolTip1.SetToolTip(ColNoteCommands, "Special Note Commands");
            ColNoteCommands.MouseDown += ColNoteCommands_MouseDown;
            // 
            // ColOutBackground
            // 
            ColOutBackground.BorderStyle = BorderStyle.FixedSingle;
            ColOutBackground.Cursor = Cursors.Hand;
            ColOutBackground.Location = new Point(278, 59);
            ColOutBackground.Name = "ColOutBackground";
            ColOutBackground.Size = new Size(25, 18);
            ColOutBackground.TabIndex = 35;
            toolTip1.SetToolTip(ColOutBackground, "Main Background");
            ColOutBackground.MouseDown += ColOutBackground_MouseDown;
            // 
            // ColOutText
            // 
            ColOutText.BorderStyle = BorderStyle.FixedSingle;
            ColOutText.Cursor = Cursors.Hand;
            ColOutText.Location = new Point(278, 83);
            ColOutText.Name = "ColOutText";
            ColOutText.Size = new Size(25, 18);
            ColOutText.TabIndex = 36;
            toolTip1.SetToolTip(ColOutText, "Text");
            ColOutText.MouseDown += ColOutText_MouseDown;
            // 
            // ColOutHlBackground
            // 
            ColOutHlBackground.BorderStyle = BorderStyle.FixedSingle;
            ColOutHlBackground.Cursor = Cursors.Hand;
            ColOutHlBackground.Location = new Point(310, 59);
            ColOutHlBackground.Name = "ColOutHlBackground";
            ColOutHlBackground.Size = new Size(25, 18);
            ColOutHlBackground.TabIndex = 37;
            toolTip1.SetToolTip(ColOutHlBackground, "Highlighted Line Background");
            ColOutHlBackground.MouseDown += ColOutHlBackground_MouseDown;
            // 
            // ColSeparators
            // 
            ColSeparators.BorderStyle = BorderStyle.FixedSingle;
            ColSeparators.Cursor = Cursors.Hand;
            ColSeparators.Location = new Point(154, 251);
            ColSeparators.Name = "ColSeparators";
            ColSeparators.Size = new Size(25, 18);
            ColSeparators.TabIndex = 39;
            toolTip1.SetToolTip(ColSeparators, "Vertical Separators");
            ColSeparators.MouseDown += ColSeparators_MouseDown;
            // 
            // ColOutSeparators
            // 
            ColOutSeparators.BorderStyle = BorderStyle.FixedSingle;
            ColOutSeparators.Cursor = Cursors.Hand;
            ColOutSeparators.Location = new Point(278, 251);
            ColOutSeparators.Name = "ColOutSeparators";
            ColOutSeparators.Size = new Size(25, 18);
            ColOutSeparators.TabIndex = 46;
            toolTip1.SetToolTip(ColOutSeparators, "Vertical Separators");
            ColOutSeparators.MouseDown += ColOutSeparators_MouseDown;
            // 
            // ColSelLineText
            // 
            ColSelLineText.BorderStyle = BorderStyle.FixedSingle;
            ColSelLineText.Cursor = Cursors.Hand;
            ColSelLineText.Location = new Point(186, 83);
            ColSelLineText.Name = "ColSelLineText";
            ColSelLineText.Size = new Size(25, 18);
            ColSelLineText.TabIndex = 47;
            toolTip1.SetToolTip(ColSelLineText, "Selected Line Text (Dots)");
            ColSelLineText.MouseDown += ColSelLineText_MouseDown;
            // 
            // ColSelLineNum
            // 
            ColSelLineNum.BorderStyle = BorderStyle.FixedSingle;
            ColSelLineNum.Cursor = Cursors.Hand;
            ColSelLineNum.Location = new Point(186, 107);
            ColSelLineNum.Name = "ColSelLineNum";
            ColSelLineNum.Size = new Size(25, 18);
            ColSelLineNum.TabIndex = 48;
            toolTip1.SetToolTip(ColSelLineNum, "Line Numbers Of Selected Line");
            ColSelLineNum.MouseDown += ColSelLineNum_MouseDown;
            // 
            // ColSelEnvelope
            // 
            ColSelEnvelope.BorderStyle = BorderStyle.FixedSingle;
            ColSelEnvelope.Cursor = Cursors.Hand;
            ColSelEnvelope.Location = new Point(186, 131);
            ColSelEnvelope.Name = "ColSelEnvelope";
            ColSelEnvelope.Size = new Size(25, 18);
            ColSelEnvelope.TabIndex = 49;
            toolTip1.SetToolTip(ColSelEnvelope, "Selected Line Envelope");
            ColSelEnvelope.MouseDown += ColSelEnvelope_MouseDown;
            // 
            // ColSelNoise
            // 
            ColSelNoise.BorderStyle = BorderStyle.FixedSingle;
            ColSelNoise.Cursor = Cursors.Hand;
            ColSelNoise.Location = new Point(186, 155);
            ColSelNoise.Name = "ColSelNoise";
            ColSelNoise.Size = new Size(25, 18);
            ColSelNoise.TabIndex = 50;
            toolTip1.SetToolTip(ColSelNoise, "Selected Line Noise");
            ColSelNoise.MouseDown += ColSelNoise_MouseDown;
            // 
            // ColSelNote
            // 
            ColSelNote.BorderStyle = BorderStyle.FixedSingle;
            ColSelNote.Cursor = Cursors.Hand;
            ColSelNote.Location = new Point(186, 179);
            ColSelNote.Name = "ColSelNote";
            ColSelNote.Size = new Size(25, 18);
            ColSelNote.TabIndex = 51;
            toolTip1.SetToolTip(ColSelNote, "Selected Line Note");
            ColSelNote.MouseDown += ColSelNote_MouseDown;
            // 
            // ColSelNoteParams
            // 
            ColSelNoteParams.BorderStyle = BorderStyle.FixedSingle;
            ColSelNoteParams.Cursor = Cursors.Hand;
            ColSelNoteParams.Location = new Point(186, 203);
            ColSelNoteParams.Name = "ColSelNoteParams";
            ColSelNoteParams.Size = new Size(25, 18);
            ColSelNoteParams.TabIndex = 52;
            toolTip1.SetToolTip(ColSelNoteParams, "Selected Line Note Params");
            ColSelNoteParams.MouseDown += ColSelNoteParams_MouseDown;
            // 
            // ColSelNoteCommands
            // 
            ColSelNoteCommands.BorderStyle = BorderStyle.FixedSingle;
            ColSelNoteCommands.Cursor = Cursors.Hand;
            ColSelNoteCommands.Location = new Point(186, 227);
            ColSelNoteCommands.Name = "ColSelNoteCommands";
            ColSelNoteCommands.Size = new Size(25, 18);
            ColSelNoteCommands.TabIndex = 53;
            toolTip1.SetToolTip(ColSelNoteCommands, "Selected Line Special Note Commands");
            ColSelNoteCommands.MouseDown += ColSelNoteCommands_MouseDown;
            // 
            // ColSamOrnBackground
            // 
            ColSamOrnBackground.BorderStyle = BorderStyle.FixedSingle;
            ColSamOrnBackground.Cursor = Cursors.Hand;
            ColSamOrnBackground.Location = new Point(401, 59);
            ColSamOrnBackground.Name = "ColSamOrnBackground";
            ColSamOrnBackground.Size = new Size(25, 18);
            ColSamOrnBackground.TabIndex = 57;
            toolTip1.SetToolTip(ColSamOrnBackground, "Main Background");
            ColSamOrnBackground.MouseDown += ColSamOrnBackground_MouseDown;
            // 
            // ColSamOrnText
            // 
            ColSamOrnText.BorderStyle = BorderStyle.FixedSingle;
            ColSamOrnText.Cursor = Cursors.Hand;
            ColSamOrnText.Location = new Point(401, 83);
            ColSamOrnText.Name = "ColSamOrnText";
            ColSamOrnText.Size = new Size(25, 18);
            ColSamOrnText.TabIndex = 58;
            toolTip1.SetToolTip(ColSamOrnText, "Sample: Text Color");
            ColSamOrnText.MouseDown += ColSamOrnText_MouseDown;
            // 
            // ColSamOrnLineNum
            // 
            ColSamOrnLineNum.BorderStyle = BorderStyle.FixedSingle;
            ColSamOrnLineNum.Cursor = Cursors.Hand;
            ColSamOrnLineNum.Location = new Point(401, 107);
            ColSamOrnLineNum.Name = "ColSamOrnLineNum";
            ColSamOrnLineNum.Size = new Size(25, 18);
            ColSamOrnLineNum.TabIndex = 59;
            toolTip1.SetToolTip(ColSamOrnLineNum, "Line Numbers");
            ColSamOrnLineNum.MouseDown += ColSamOrnLineNum_MouseDown;
            // 
            // ColSamNoise
            // 
            ColSamNoise.BorderStyle = BorderStyle.FixedSingle;
            ColSamNoise.Cursor = Cursors.Hand;
            ColSamNoise.Location = new Point(401, 155);
            ColSamNoise.Name = "ColSamNoise";
            ColSamNoise.Size = new Size(25, 18);
            ColSamNoise.TabIndex = 60;
            toolTip1.SetToolTip(ColSamNoise, "Sample: Noise");
            ColSamNoise.MouseDown += ColSamNoise_MouseDown;
            // 
            // ColSamOrnSeparators
            // 
            ColSamOrnSeparators.BorderStyle = BorderStyle.FixedSingle;
            ColSamOrnSeparators.Cursor = Cursors.Hand;
            ColSamOrnSeparators.Location = new Point(401, 251);
            ColSamOrnSeparators.Name = "ColSamOrnSeparators";
            ColSamOrnSeparators.Size = new Size(25, 18);
            ColSamOrnSeparators.TabIndex = 61;
            toolTip1.SetToolTip(ColSamOrnSeparators, "Sample/Ornament Separators");
            ColSamOrnSeparators.MouseDown += ColSamOrnSeparators_MouseDown;
            // 
            // ColSamOrnTone
            // 
            ColSamOrnTone.BorderStyle = BorderStyle.FixedSingle;
            ColSamOrnTone.Cursor = Cursors.Hand;
            ColSamOrnTone.Location = new Point(401, 275);
            ColSamOrnTone.Name = "ColSamOrnTone";
            ColSamOrnTone.Size = new Size(25, 18);
            ColSamOrnTone.TabIndex = 63;
            toolTip1.SetToolTip(ColSamOrnTone, "Sample/Ornament: Tone Shift");
            ColSamOrnTone.MouseDown += ColSamOrnTone_MouseDown;
            // 
            // ColFullScreenBackground
            // 
            ColFullScreenBackground.BorderStyle = BorderStyle.FixedSingle;
            ColFullScreenBackground.Cursor = Cursors.Hand;
            ColFullScreenBackground.Location = new Point(176, 307);
            ColFullScreenBackground.Name = "ColFullScreenBackground";
            ColFullScreenBackground.Size = new Size(25, 18);
            ColFullScreenBackground.TabIndex = 65;
            toolTip1.SetToolTip(ColFullScreenBackground, "Highlighted Line Background");
            ColFullScreenBackground.MouseDown += ColFullScreenBackground_MouseDown;
            // 
            // ColSamOrnSelBackground
            // 
            ColSamOrnSelBackground.BorderStyle = BorderStyle.FixedSingle;
            ColSamOrnSelBackground.Cursor = Cursors.Hand;
            ColSamOrnSelBackground.Location = new Point(433, 59);
            ColSamOrnSelBackground.Name = "ColSamOrnSelBackground";
            ColSamOrnSelBackground.Size = new Size(25, 18);
            ColSamOrnSelBackground.TabIndex = 67;
            toolTip1.SetToolTip(ColSamOrnSelBackground, "Selected Line Background");
            ColSamOrnSelBackground.MouseDown += ColSamOrnSelBackground_MouseDown;
            // 
            // ColSamOrnSelText
            // 
            ColSamOrnSelText.BorderStyle = BorderStyle.FixedSingle;
            ColSamOrnSelText.Cursor = Cursors.Hand;
            ColSamOrnSelText.Location = new Point(433, 83);
            ColSamOrnSelText.Name = "ColSamOrnSelText";
            ColSamOrnSelText.Size = new Size(25, 18);
            ColSamOrnSelText.TabIndex = 68;
            toolTip1.SetToolTip(ColSamOrnSelText, "Sample: Selected Line Text");
            ColSamOrnSelText.MouseDown += ColSamOrnSelText_MouseDown;
            // 
            // ColSamSelNoise
            // 
            ColSamSelNoise.BorderStyle = BorderStyle.FixedSingle;
            ColSamSelNoise.Cursor = Cursors.Hand;
            ColSamSelNoise.Location = new Point(433, 155);
            ColSamSelNoise.Name = "ColSamSelNoise";
            ColSamSelNoise.Size = new Size(25, 18);
            ColSamSelNoise.TabIndex = 69;
            toolTip1.SetToolTip(ColSamSelNoise, "Sample: Selected Line Noise");
            ColSamSelNoise.MouseDown += ColSamSelNoise_MouseDown;
            // 
            // ColSamOrnSelTone
            // 
            ColSamOrnSelTone.BorderStyle = BorderStyle.FixedSingle;
            ColSamOrnSelTone.Cursor = Cursors.Hand;
            ColSamOrnSelTone.Location = new Point(433, 275);
            ColSamOrnSelTone.Name = "ColSamOrnSelTone";
            ColSamOrnSelTone.Size = new Size(25, 18);
            ColSamOrnSelTone.TabIndex = 70;
            toolTip1.SetToolTip(ColSamOrnSelTone, "Sample/Ornament: Selected line Tone Shift");
            ColSamOrnSelTone.MouseDown += ColSamOrnSelTone_MouseDown;
            // 
            // ColSamOrnSelLineNum
            // 
            ColSamOrnSelLineNum.BorderStyle = BorderStyle.FixedSingle;
            ColSamOrnSelLineNum.Cursor = Cursors.Hand;
            ColSamOrnSelLineNum.Location = new Point(433, 107);
            ColSamOrnSelLineNum.Name = "ColSamOrnSelLineNum";
            ColSamOrnSelLineNum.Size = new Size(25, 18);
            ColSamOrnSelLineNum.TabIndex = 71;
            toolTip1.SetToolTip(ColSamOrnSelLineNum, "Selected Line Numbers");
            ColSamOrnSelLineNum.MouseDown += ColSamOrnSelLineNum_MouseDown;
            // 
            // ColHighlLineNum
            // 
            ColHighlLineNum.BorderStyle = BorderStyle.FixedSingle;
            ColHighlLineNum.Cursor = Cursors.Hand;
            ColHighlLineNum.Location = new Point(218, 107);
            ColHighlLineNum.Name = "ColHighlLineNum";
            ColHighlLineNum.Size = new Size(25, 18);
            ColHighlLineNum.TabIndex = 75;
            toolTip1.SetToolTip(ColHighlLineNum, "Line Numbers Of Highlighted Line");
            ColHighlLineNum.MouseDown += ColHighlLineNum_MouseDown;
            // 
            // OptionsTabControl
            // 
            OptionsTabControl.Controls.Add(CurWinds);
            OptionsTabControl.Controls.Add(ColorThemesTab);
            OptionsTabControl.Controls.Add(AYEmu);
            OptionsTabControl.Controls.Add(WOAPITAB);
            OptionsTabControl.Controls.Add(HotKeys);
            OptionsTabControl.Controls.Add(OpMod);
            OptionsTabControl.Dock = DockStyle.Top;
            OptionsTabControl.Font = new Font("Microsoft Sans Serif", 8.25F);
            OptionsTabControl.Location = new Point(0, 0);
            OptionsTabControl.Name = "OptionsTabControl";
            OptionsTabControl.SelectedIndex = 0;
            OptionsTabControl.Size = new Size(546, 529);
            OptionsTabControl.TabIndex = 0;
            // 
            // CurWinds
            // 
            CurWinds.Controls.Add(InterfaceBox);
            CurWinds.Controls.Add(BackupBox);
            CurWinds.Controls.Add(AppPriorityBox);
            CurWinds.Controls.Add(StartupBox);
            CurWinds.Controls.Add(FreqTableBox);
            CurWinds.ImageIndex = 2;
            CurWinds.Location = new Point(4, 22);
            CurWinds.Name = "CurWinds";
            CurWinds.Size = new Size(538, 503);
            CurWinds.TabIndex = 0;
            CurWinds.Text = "Main";
            // 
            // InterfaceBox
            // 
            InterfaceBox.Controls.Add(DecNumbersLines);
            InterfaceBox.Controls.Add(DecNumbersNoise);
            InterfaceBox.Controls.Add(HighlightSpeedPosition);
            InterfaceBox.Controls.Add(DisablePatSeparators);
            InterfaceBox.Controls.Add(DisableHintsOpt);
            InterfaceBox.Controls.Add(DisableCtrlClickOpt);
            InterfaceBox.Controls.Add(DisableInfoWinOpt);
            InterfaceBox.Location = new Point(8, 8);
            InterfaceBox.Name = "InterfaceBox";
            InterfaceBox.Size = new Size(521, 209);
            InterfaceBox.TabIndex = 0;
            InterfaceBox.TabStop = false;
            InterfaceBox.Text = " Interface Options ";
            // 
            // DecNumbersLines
            // 
            DecNumbersLines.Location = new Point(12, 27);
            DecNumbersLines.Name = "DecNumbersLines";
            DecNumbersLines.Size = new Size(295, 17);
            DecNumbersLines.TabIndex = 0;
            DecNumbersLines.Text = "Line Numbers in Pattern, Sample, Ornament in Decimal";
            DecNumbersLines.Click += DecNumbersLines_Click;
            // 
            // DecNumbersNoise
            // 
            DecNumbersNoise.Location = new Point(12, 52);
            DecNumbersNoise.Name = "DecNumbersNoise";
            DecNumbersNoise.Size = new Size(287, 17);
            DecNumbersNoise.TabIndex = 1;
            DecNumbersNoise.Text = "Noise Level in Pattern and Sample in Decimal";
            DecNumbersNoise.Click += DecNumbersNoise_Click;
            // 
            // HighlightSpeedPosition
            // 
            HighlightSpeedPosition.Location = new Point(12, 78);
            HighlightSpeedPosition.Name = "HighlightSpeedPosition";
            HighlightSpeedPosition.Size = new Size(287, 17);
            HighlightSpeedPosition.TabIndex = 2;
            HighlightSpeedPosition.Text = "Highlight Speed Position for Samples and Ornaments";
            HighlightSpeedPosition.Click += HighlightSpeedPosition_Click;
            // 
            // DisablePatSeparators
            // 
            DisablePatSeparators.Location = new Point(12, 103);
            DisablePatSeparators.Name = "DisablePatSeparators";
            DisablePatSeparators.Size = new Size(273, 17);
            DisablePatSeparators.TabIndex = 3;
            DisablePatSeparators.Text = "Disable Vertical Pattern Separators";
            DisablePatSeparators.Click += DisablePatSeparators_Click;
            // 
            // DisableHintsOpt
            // 
            DisableHintsOpt.Location = new Point(12, 129);
            DisableHintsOpt.Name = "DisableHintsOpt";
            DisableHintsOpt.Size = new Size(279, 17);
            DisableHintsOpt.TabIndex = 4;
            DisableHintsOpt.Text = "Disable Hints in Pattern, Sample and Ornament Editor";
            DisableHintsOpt.Click += DisableHintsOpt_Click;
            // 
            // DisableCtrlClickOpt
            // 
            DisableCtrlClickOpt.Location = new Point(12, 154);
            DisableCtrlClickOpt.Name = "DisableCtrlClickOpt";
            DisableCtrlClickOpt.Size = new Size(279, 17);
            DisableCtrlClickOpt.TabIndex = 5;
            DisableCtrlClickOpt.Text = "Disable Ctrl+Click and Ctrl+Enter on Sample/Ornament";
            DisableCtrlClickOpt.Click += DisableCtrlClickOpt_Click;
            // 
            // DisableInfoWinOpt
            // 
            DisableInfoWinOpt.Location = new Point(12, 180);
            DisableInfoWinOpt.Name = "DisableInfoWinOpt";
            DisableInfoWinOpt.Size = new Size(261, 17);
            DisableInfoWinOpt.TabIndex = 6;
            DisableInfoWinOpt.Text = "Don't Show Info Window When Track is Loaded";
            DisableInfoWinOpt.MouseUp += DisableInfoWinOpt_MouseUp;
            // 
            // BackupBox
            // 
            BackupBox.Controls.Add(BackupEveryMins);
            BackupBox.Controls.Add(EveryLabel);
            BackupBox.Controls.Add(MinutesLabel);
            BackupBox.Controls.Add(BackupFilenameLabel);
            BackupBox.Controls.Add(AutoSaveBackups);
            BackupBox.Location = new Point(277, 232);
            BackupBox.Name = "BackupBox";
            BackupBox.Size = new Size(252, 137);
            BackupBox.TabIndex = 2;
            BackupBox.TabStop = false;
            BackupBox.Text = " Backup Options ";
            // 
            // BackupEveryMins
            // 
            BackupEveryMins.Location = new Point(50, 63);
            BackupEveryMins.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            BackupEveryMins.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            BackupEveryMins.Name = "BackupEveryMins";
            BackupEveryMins.Size = new Size(47, 20);
            BackupEveryMins.TabIndex = 2;
            BackupEveryMins.Value = new decimal(new int[] { 1, 0, 0, 0 });
            BackupEveryMins.ValueChanged += BackupEveryMins_ValueChanged;
            // 
            // EveryLabel
            // 
            EveryLabel.Location = new Point(12, 66);
            EveryLabel.Name = "EveryLabel";
            EveryLabel.Size = new Size(40, 13);
            EveryLabel.TabIndex = 0;
            EveryLabel.Text = "Every:";
            // 
            // MinutesLabel
            // 
            MinutesLabel.Location = new Point(104, 66);
            MinutesLabel.Name = "MinutesLabel";
            MinutesLabel.Size = new Size(53, 13);
            MinutesLabel.TabIndex = 1;
            MinutesLabel.Text = "Minutes";
            // 
            // BackupFilenameLabel
            // 
            BackupFilenameLabel.Font = new Font("Microsoft Sans Serif", 8.25F);
            BackupFilenameLabel.ForeColor = Color.Teal;
            BackupFilenameLabel.Location = new Point(10, 103);
            BackupFilenameLabel.Name = "BackupFilenameLabel";
            BackupFilenameLabel.Size = new Size(205, 13);
            BackupFilenameLabel.TabIndex = 2;
            BackupFilenameLabel.Text = "Backup Filename: \"filename ver 001.vt2\"";
            // 
            // AutoSaveBackups
            // 
            AutoSaveBackups.Location = new Point(12, 30);
            AutoSaveBackups.Name = "AutoSaveBackups";
            AutoSaveBackups.Size = new Size(145, 17);
            AutoSaveBackups.TabIndex = 0;
            AutoSaveBackups.Text = "Autosave Backups";
            AutoSaveBackups.MouseUp += AutoSaveBackups_MouseUp;
            // 
            // AppPriorityBox
            // 
            AppPriorityBox.Items.Add("Normal");
            AppPriorityBox.Items.Add("High");
            AppPriorityBox.Location = new Point(277, 384);
            AppPriorityBox.Name = "AppPriorityBox";
            AppPriorityBox.Size = new Size(252, 100);
            AppPriorityBox.TabIndex = 4;
            AppPriorityBox.TabStop = false;
            AppPriorityBox.Text = " Application Priority ";
            AppPriorityBox.Click += AppPriorityBox_Click;
            // 
            // StartupBox
            // 
            StartupBox.Controls.Add(WhenVortexStartsLabel);
            StartupBox.Controls.Add(TemplPathLab);
            StartupBox.Controls.Add(StartsAction);
            StartupBox.Controls.Add(TemplateSong);
            StartupBox.Controls.Add(BrowseTemplate);
            StartupBox.Location = new Point(8, 232);
            StartupBox.Name = "StartupBox";
            StartupBox.Size = new Size(252, 137);
            StartupBox.TabIndex = 1;
            StartupBox.TabStop = false;
            StartupBox.Text = " Startup ";
            // 
            // WhenVortexStartsLabel
            // 
            WhenVortexStartsLabel.Location = new Point(12, 30);
            WhenVortexStartsLabel.Name = "WhenVortexStartsLabel";
            WhenVortexStartsLabel.Size = new Size(93, 13);
            WhenVortexStartsLabel.TabIndex = 0;
            WhenVortexStartsLabel.Text = "When Vortex Starts:";
            // 
            // TemplPathLab
            // 
            TemplPathLab.Location = new Point(12, 82);
            TemplPathLab.Name = "TemplPathLab";
            TemplPathLab.Size = new Size(91, 13);
            TemplPathLab.TabIndex = 1;
            TemplPathLab.Text = "Use Template Song:";
            // 
            // StartsAction
            // 
            StartsAction.DropDownStyle = ComboBoxStyle.DropDownList;
            StartsAction.ItemHeight = 13;
            StartsAction.Items.AddRange(new object[] { "Open Template", "Open Blank Song", "Do Nothing" });
            StartsAction.Location = new Point(12, 47);
            StartsAction.Name = "StartsAction";
            StartsAction.Size = new Size(221, 21);
            StartsAction.TabIndex = 0;
            StartsAction.SelectedIndexChanged += StartsAction_SelectedIndexChanged;
            // 
            // TemplateSong
            // 
            TemplateSong.Location = new Point(12, 99);
            TemplateSong.Name = "TemplateSong";
            TemplateSong.Size = new Size(192, 20);
            TemplateSong.TabIndex = 1;
            // 
            // BrowseTemplate
            // 
            BrowseTemplate.Location = new Point(206, 99);
            BrowseTemplate.Name = "BrowseTemplate";
            BrowseTemplate.Size = new Size(25, 21);
            BrowseTemplate.TabIndex = 2;
            BrowseTemplate.Text = "...";
            BrowseTemplate.Click += BrowseTemplate_Click;
            // 
            // FreqTableBox
            // 
            FreqTableBox.Controls.Add(FrequencyTableLabel);
            FreqTableBox.Controls.Add(TableName);
            FreqTableBox.Controls.Add(DefaultFreqTableUpDown);
            FreqTableBox.Location = new Point(8, 384);
            FreqTableBox.Name = "FreqTableBox";
            FreqTableBox.Size = new Size(252, 105);
            FreqTableBox.TabIndex = 3;
            FreqTableBox.TabStop = false;
            FreqTableBox.Text = " Frequency Table ";
            // 
            // FrequencyTableLabel
            // 
            FrequencyTableLabel.Location = new Point(12, 30);
            FrequencyTableLabel.Name = "FrequencyTableLabel";
            FrequencyTableLabel.Size = new Size(129, 13);
            FrequencyTableLabel.TabIndex = 0;
            FrequencyTableLabel.Text = "Default Frequency Table:";
            // 
            // TableName
            // 
            TableName.Location = new Point(11, 83);
            TableName.Name = "TableName";
            TableName.Size = new Size(130, 13);
            TableName.TabIndex = 1;
            TableName.Text = "ASM or PSC (1.75 MHz)";
            // 
            // DefaultFreqTableUpDown
            // 
            DefaultFreqTableUpDown.Location = new Point(12, 55);
            DefaultFreqTableUpDown.Name = "DefaultFreqTableUpDown";
            DefaultFreqTableUpDown.Size = new Size(45, 20);
            DefaultFreqTableUpDown.TabIndex = 1;
            DefaultFreqTableUpDown.Value = new decimal(new int[] { 2, 0, 0, 0 });
            DefaultFreqTableUpDown.ValueChanged += DefaultFreqTableUpDown_ValueChanged;
            // 
            // ColorThemesTab
            // 
            ColorThemesTab.Controls.Add(ThemesBox);
            ColorThemesTab.Controls.Add(ThemesOptionsBox);
            ColorThemesTab.Controls.Add(FontBox);
            ColorThemesTab.ImageIndex = 5;
            ColorThemesTab.Location = new Point(4, 22);
            ColorThemesTab.Name = "ColorThemesTab";
            ColorThemesTab.Size = new Size(192, 74);
            ColorThemesTab.TabIndex = 1;
            ColorThemesTab.Text = "Appearance";
            // 
            // ThemesBox
            // 
            ThemesBox.Controls.Add(ColorThemesList);
            ThemesBox.Controls.Add(LoadThemeButton);
            ThemesBox.Controls.Add(SaveThemeButton);
            ThemesBox.Controls.Add(DeleteThemeButton);
            ThemesBox.Controls.Add(CopyThemeButton);
            ThemesBox.Controls.Add(RenameThemeButton);
            ThemesBox.Location = new Point(8, 8);
            ThemesBox.Name = "ThemesBox";
            ThemesBox.Size = new Size(257, 145);
            ThemesBox.TabIndex = 0;
            ThemesBox.TabStop = false;
            ThemesBox.Text = " Color Themes ";
            // 
            // ColorThemesList
            // 
            ColorThemesList.ItemHeight = 13;
            ColorThemesList.Location = new Point(8, 24);
            ColorThemesList.Name = "ColorThemesList";
            ColorThemesList.Size = new Size(172, 108);
            ColorThemesList.TabIndex = 0;
            ColorThemesList.SelectedIndexChanged += ColorThemesList_SelectedIndexChanged;
            // 
            // LoadThemeButton
            // 
            LoadThemeButton.Location = new Point(186, 24);
            LoadThemeButton.Name = "LoadThemeButton";
            LoadThemeButton.Size = new Size(63, 21);
            LoadThemeButton.TabIndex = 1;
            LoadThemeButton.Text = "Load";
            LoadThemeButton.Click += LoadThemeButton_Click;
            // 
            // SaveThemeButton
            // 
            SaveThemeButton.Location = new Point(186, 47);
            SaveThemeButton.Name = "SaveThemeButton";
            SaveThemeButton.Size = new Size(63, 21);
            SaveThemeButton.TabIndex = 2;
            SaveThemeButton.Text = "Save";
            SaveThemeButton.Click += SaveThemeButton_Click;
            // 
            // DeleteThemeButton
            // 
            DeleteThemeButton.Location = new Point(186, 116);
            DeleteThemeButton.Name = "DeleteThemeButton";
            DeleteThemeButton.Size = new Size(63, 21);
            DeleteThemeButton.TabIndex = 5;
            DeleteThemeButton.Text = "Delete";
            DeleteThemeButton.Click += DeleteThemeButton_Click;
            // 
            // CopyThemeButton
            // 
            CopyThemeButton.Location = new Point(186, 70);
            CopyThemeButton.Name = "CopyThemeButton";
            CopyThemeButton.Size = new Size(63, 21);
            CopyThemeButton.TabIndex = 3;
            CopyThemeButton.Text = "Duplicate";
            CopyThemeButton.Click += CopyThemeButton_Click;
            // 
            // RenameThemeButton
            // 
            RenameThemeButton.Location = new Point(186, 93);
            RenameThemeButton.Name = "RenameThemeButton";
            RenameThemeButton.Size = new Size(63, 21);
            RenameThemeButton.TabIndex = 4;
            RenameThemeButton.Text = "Rename";
            RenameThemeButton.Click += RenameThemeButton_Click;
            // 
            // ThemesOptionsBox
            // 
            ThemesOptionsBox.Controls.Add(Seperator12);
            ThemesOptionsBox.Controls.Add(ColBackground);
            ThemesOptionsBox.Controls.Add(DefinitionLabel);
            ThemesOptionsBox.Controls.Add(CurrentPatternLabel);
            ThemesOptionsBox.Controls.Add(NextPrevPatternLabel);
            ThemesOptionsBox.Controls.Add(BackgroundLabel);
            ThemesOptionsBox.Controls.Add(TextLabel);
            ThemesOptionsBox.Controls.Add(ColText);
            ThemesOptionsBox.Controls.Add(LineNumbersLabel);
            ThemesOptionsBox.Controls.Add(EnvelopeLabel);
            ThemesOptionsBox.Controls.Add(NoiseLabel);
            ThemesOptionsBox.Controls.Add(NoteLabel);
            ThemesOptionsBox.Controls.Add(NoteParamsLabel);
            ThemesOptionsBox.Controls.Add(NoteCommandsLabel);
            ThemesOptionsBox.Controls.Add(ColSelLineBackground);
            ThemesOptionsBox.Controls.Add(ColHighlBackground);
            ThemesOptionsBox.Controls.Add(ColHighlText);
            ThemesOptionsBox.Controls.Add(ColLineNum);
            ThemesOptionsBox.Controls.Add(ColEnvelope);
            ThemesOptionsBox.Controls.Add(ColNoise);
            ThemesOptionsBox.Controls.Add(ColNote);
            ThemesOptionsBox.Controls.Add(ColNoteParams);
            ThemesOptionsBox.Controls.Add(ColNoteCommands);
            ThemesOptionsBox.Controls.Add(ColOutBackground);
            ThemesOptionsBox.Controls.Add(ColOutText);
            ThemesOptionsBox.Controls.Add(ColOutHlBackground);
            ThemesOptionsBox.Controls.Add(SeparatorsLabel);
            ThemesOptionsBox.Controls.Add(ColSeparators);
            ThemesOptionsBox.Controls.Add(Seperator1);
            ThemesOptionsBox.Controls.Add(Seperator2);
            ThemesOptionsBox.Controls.Add(Seperator3);
            ThemesOptionsBox.Controls.Add(Seperator4);
            ThemesOptionsBox.Controls.Add(Seperator5);
            ThemesOptionsBox.Controls.Add(Seperator6);
            ThemesOptionsBox.Controls.Add(ColOutSeparators);
            ThemesOptionsBox.Controls.Add(ColSelLineText);
            ThemesOptionsBox.Controls.Add(ColSelLineNum);
            ThemesOptionsBox.Controls.Add(ColSelEnvelope);
            ThemesOptionsBox.Controls.Add(ColSelNoise);
            ThemesOptionsBox.Controls.Add(ColSelNote);
            ThemesOptionsBox.Controls.Add(ColSelNoteParams);
            ThemesOptionsBox.Controls.Add(ColSelNoteCommands);
            ThemesOptionsBox.Controls.Add(Seperator7);
            ThemesOptionsBox.Controls.Add(Seperator8);
            ThemesOptionsBox.Controls.Add(SampleOrnamentLabel);
            ThemesOptionsBox.Controls.Add(ColSamOrnBackground);
            ThemesOptionsBox.Controls.Add(ColSamOrnText);
            ThemesOptionsBox.Controls.Add(ColSamOrnLineNum);
            ThemesOptionsBox.Controls.Add(ColSamNoise);
            ThemesOptionsBox.Controls.Add(ColSamOrnSeparators);
            ThemesOptionsBox.Controls.Add(ToneShiftLabel);
            ThemesOptionsBox.Controls.Add(ColSamOrnTone);
            ThemesOptionsBox.Controls.Add(FullScreenBackgroundLabel);
            ThemesOptionsBox.Controls.Add(ColFullScreenBackground);
            ThemesOptionsBox.Controls.Add(ColSamOrnSelBackground);
            ThemesOptionsBox.Controls.Add(ColSamOrnSelText);
            ThemesOptionsBox.Controls.Add(ColSamSelNoise);
            ThemesOptionsBox.Controls.Add(ColSamOrnSelTone);
            ThemesOptionsBox.Controls.Add(ColSamOrnSelLineNum);
            ThemesOptionsBox.Controls.Add(Seperator14);
            ThemesOptionsBox.Controls.Add(WindowThemeLabel);
            ThemesOptionsBox.Controls.Add(Seperator13);
            ThemesOptionsBox.Controls.Add(ColHighlLineNum);
            ThemesOptionsBox.Controls.Add(WinColorsBox);
            ThemesOptionsBox.Controls.Add(Background1Panel);
            ThemesOptionsBox.Controls.Add(Background2Panel);
            ThemesOptionsBox.Controls.Add(Background3Panel);
            ThemesOptionsBox.Controls.Add(Background4Panel);
            ThemesOptionsBox.Controls.Add(Background5Panel);
            ThemesOptionsBox.Controls.Add(Background6Panel);
            ThemesOptionsBox.Controls.Add(Background7Panel);
            ThemesOptionsBox.Controls.Add(Background8Panel);
            ThemesOptionsBox.Controls.Add(Background9Panel);
            ThemesOptionsBox.Controls.Add(Background10Panel);
            ThemesOptionsBox.Controls.Add(Background11Panel);
            ThemesOptionsBox.Controls.Add(TableHeader);
            ThemesOptionsBox.Controls.Add(TableBottom);
            ThemesOptionsBox.Location = new Point(8, 160);
            ThemesOptionsBox.Name = "ThemesOptionsBox";
            ThemesOptionsBox.Size = new Size(521, 337);
            ThemesOptionsBox.TabIndex = 2;
            ThemesOptionsBox.TabStop = false;
            ThemesOptionsBox.Text = " Color Theme Options  ";
            // 
            // Seperator12
            // 
            Seperator12.Location = new Point(166, 307);
            Seperator12.Name = "Seperator12";
            Seperator12.Size = new Size(1, 19);
            Seperator12.TabIndex = 66;
            Seperator12.TabStop = false;
            // 
            // DefinitionLabel
            // 
            DefinitionLabel.Location = new Point(20, 30);
            DefinitionLabel.Name = "DefinitionLabel";
            DefinitionLabel.Size = new Size(54, 13);
            DefinitionLabel.TabIndex = 6;
            DefinitionLabel.Text = "Definition";
            // 
            // CurrentPatternLabel
            // 
            CurrentPatternLabel.Location = new Point(154, 30);
            CurrentPatternLabel.Name = "CurrentPatternLabel";
            CurrentPatternLabel.Size = new Size(89, 13);
            CurrentPatternLabel.TabIndex = 7;
            CurrentPatternLabel.Text = "Current Pattern";
            // 
            // NextPrevPatternLabel
            // 
            NextPrevPatternLabel.Location = new Point(278, 30);
            NextPrevPatternLabel.Name = "NextPrevPatternLabel";
            NextPrevPatternLabel.Size = new Size(103, 13);
            NextPrevPatternLabel.TabIndex = 8;
            NextPrevPatternLabel.Text = "Next/Prev Pattern";
            // 
            // BackgroundLabel
            // 
            BackgroundLabel.BackColor = Color.FromArgb(235, 235, 235);
            BackgroundLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            BackgroundLabel.Location = new Point(20, 61);
            BackgroundLabel.Name = "BackgroundLabel";
            BackgroundLabel.Size = new Size(76, 13);
            BackgroundLabel.TabIndex = 9;
            BackgroundLabel.Text = "Background";
            // 
            // TextLabel
            // 
            TextLabel.BackColor = Color.FromArgb(224, 224, 224);
            TextLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            TextLabel.Location = new Point(20, 85);
            TextLabel.Name = "TextLabel";
            TextLabel.Size = new Size(33, 13);
            TextLabel.TabIndex = 11;
            TextLabel.Text = "Text";
            // 
            // LineNumbersLabel
            // 
            LineNumbersLabel.BackColor = Color.FromArgb(235, 235, 235);
            LineNumbersLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            LineNumbersLabel.Location = new Point(20, 109);
            LineNumbersLabel.Name = "LineNumbersLabel";
            LineNumbersLabel.Size = new Size(91, 13);
            LineNumbersLabel.TabIndex = 17;
            LineNumbersLabel.Text = "Line Numbers";
            // 
            // EnvelopeLabel
            // 
            EnvelopeLabel.BackColor = Color.FromArgb(224, 224, 224);
            EnvelopeLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            EnvelopeLabel.Location = new Point(20, 133);
            EnvelopeLabel.Name = "EnvelopeLabel";
            EnvelopeLabel.Size = new Size(62, 13);
            EnvelopeLabel.TabIndex = 18;
            EnvelopeLabel.Text = "Envelope";
            // 
            // NoiseLabel
            // 
            NoiseLabel.BackColor = Color.FromArgb(235, 235, 235);
            NoiseLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            NoiseLabel.Location = new Point(20, 157);
            NoiseLabel.Name = "NoiseLabel";
            NoiseLabel.Size = new Size(42, 13);
            NoiseLabel.TabIndex = 19;
            NoiseLabel.Text = "Noise";
            // 
            // NoteLabel
            // 
            NoteLabel.BackColor = Color.FromArgb(224, 224, 224);
            NoteLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            NoteLabel.Location = new Point(20, 181);
            NoteLabel.Name = "NoteLabel";
            NoteLabel.Size = new Size(42, 13);
            NoteLabel.TabIndex = 20;
            NoteLabel.Text = "Note";
            // 
            // NoteParamsLabel
            // 
            NoteParamsLabel.BackColor = Color.FromArgb(235, 235, 235);
            NoteParamsLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            NoteParamsLabel.Location = new Point(20, 205);
            NoteParamsLabel.Name = "NoteParamsLabel";
            NoteParamsLabel.Size = new Size(91, 13);
            NoteParamsLabel.TabIndex = 24;
            NoteParamsLabel.Text = "Note Params";
            // 
            // NoteCommandsLabel
            // 
            NoteCommandsLabel.BackColor = Color.FromArgb(224, 224, 224);
            NoteCommandsLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            NoteCommandsLabel.Location = new Point(20, 229);
            NoteCommandsLabel.Name = "NoteCommandsLabel";
            NoteCommandsLabel.Size = new Size(101, 13);
            NoteCommandsLabel.TabIndex = 25;
            NoteCommandsLabel.Text = "Note Commands";
            // 
            // SeparatorsLabel
            // 
            SeparatorsLabel.BackColor = Color.FromArgb(235, 235, 235);
            SeparatorsLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            SeparatorsLabel.Location = new Point(20, 253);
            SeparatorsLabel.Name = "SeparatorsLabel";
            SeparatorsLabel.Size = new Size(76, 13);
            SeparatorsLabel.TabIndex = 38;
            SeparatorsLabel.Text = "Separators";
            // 
            // Seperator1
            // 
            Seperator1.Location = new Point(140, 51);
            Seperator1.Name = "Seperator1";
            Seperator1.Size = new Size(1, 251);
            Seperator1.TabIndex = 40;
            Seperator1.TabStop = false;
            // 
            // Seperator2
            // 
            Seperator2.Location = new Point(137, 51);
            Seperator2.Name = "Seperator2";
            Seperator2.Size = new Size(1, 251);
            Seperator2.TabIndex = 41;
            Seperator2.TabStop = false;
            // 
            // Seperator3
            // 
            Seperator3.Location = new Point(264, 51);
            Seperator3.Name = "Seperator3";
            Seperator3.Size = new Size(1, 251);
            Seperator3.TabIndex = 42;
            Seperator3.TabStop = false;
            // 
            // Seperator4
            // 
            Seperator4.Location = new Point(264, 27);
            Seperator4.Name = "Seperator4";
            Seperator4.Size = new Size(1, 19);
            Seperator4.TabIndex = 43;
            Seperator4.TabStop = false;
            // 
            // Seperator5
            // 
            Seperator5.Location = new Point(137, 27);
            Seperator5.Name = "Seperator5";
            Seperator5.Size = new Size(1, 19);
            Seperator5.TabIndex = 44;
            Seperator5.TabStop = false;
            // 
            // Seperator6
            // 
            Seperator6.Location = new Point(140, 27);
            Seperator6.Name = "Seperator6";
            Seperator6.Size = new Size(1, 19);
            Seperator6.TabIndex = 45;
            Seperator6.TabStop = false;
            // 
            // Seperator7
            // 
            Seperator7.Location = new Point(387, 51);
            Seperator7.Name = "Seperator7";
            Seperator7.Size = new Size(1, 251);
            Seperator7.TabIndex = 54;
            Seperator7.TabStop = false;
            // 
            // Seperator8
            // 
            Seperator8.Location = new Point(387, 27);
            Seperator8.Name = "Seperator8";
            Seperator8.Size = new Size(1, 19);
            Seperator8.TabIndex = 55;
            Seperator8.TabStop = false;
            // 
            // SampleOrnamentLabel
            // 
            SampleOrnamentLabel.Location = new Point(401, 30);
            SampleOrnamentLabel.Name = "SampleOrnamentLabel";
            SampleOrnamentLabel.Size = new Size(100, 13);
            SampleOrnamentLabel.TabIndex = 56;
            SampleOrnamentLabel.Text = "Sample/Ornament";
            // 
            // ToneShiftLabel
            // 
            ToneShiftLabel.BackColor = Color.FromArgb(224, 224, 224);
            ToneShiftLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            ToneShiftLabel.Location = new Point(20, 277);
            ToneShiftLabel.Name = "ToneShiftLabel";
            ToneShiftLabel.Size = new Size(76, 13);
            ToneShiftLabel.TabIndex = 62;
            ToneShiftLabel.Text = "Tone Shift";
            // 
            // FullScreenBackgroundLabel
            // 
            FullScreenBackgroundLabel.BackColor = Color.FromArgb(235, 235, 235);
            FullScreenBackgroundLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            FullScreenBackgroundLabel.Location = new Point(20, 310);
            FullScreenBackgroundLabel.Name = "FullScreenBackgroundLabel";
            FullScreenBackgroundLabel.Size = new Size(147, 13);
            FullScreenBackgroundLabel.TabIndex = 64;
            FullScreenBackgroundLabel.Text = "Full Screen Background:";
            // 
            // Seperator14
            // 
            Seperator14.Location = new Point(387, 307);
            Seperator14.Name = "Seperator14";
            Seperator14.Size = new Size(1, 19);
            Seperator14.TabIndex = 72;
            Seperator14.TabStop = false;
            // 
            // WindowThemeLabel
            // 
            WindowThemeLabel.BackColor = Color.FromArgb(235, 235, 235);
            WindowThemeLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            WindowThemeLabel.Location = new Point(288, 310);
            WindowThemeLabel.Name = "WindowThemeLabel";
            WindowThemeLabel.Size = new Size(99, 13);
            WindowThemeLabel.TabIndex = 73;
            WindowThemeLabel.Text = "Window Theme:";
            // 
            // Seperator13
            // 
            Seperator13.Location = new Point(264, 307);
            Seperator13.Name = "Seperator13";
            Seperator13.Size = new Size(1, 19);
            Seperator13.TabIndex = 74;
            Seperator13.TabStop = false;
            // 
            // WinColorsBox
            // 
            WinColorsBox.DropDownStyle = ComboBoxStyle.DropDownList;
            WinColorsBox.Items.AddRange(new object[] { "Default", "Crystaline", "Yellow", "Soft", "Relief", "Flatlines", "Light" });
            WinColorsBox.Location = new Point(394, 306);
            WinColorsBox.Name = "WinColorsBox";
            WinColorsBox.Size = new Size(113, 21);
            WinColorsBox.TabIndex = 0;
            WinColorsBox.SelectedIndexChanged += WinColorsBox_SelectedIndexChanged;
            // 
            // Background1Panel
            // 
            Background1Panel.BackColor = Color.FromArgb(235, 235, 235);
            Background1Panel.Location = new Point(9, 56);
            Background1Panel.Name = "Background1Panel";
            Background1Panel.Size = new Size(504, 25);
            Background1Panel.TabIndex = 3;
            // 
            // Background2Panel
            // 
            Background2Panel.BackColor = Color.FromArgb(224, 224, 224);
            Background2Panel.Location = new Point(9, 80);
            Background2Panel.Name = "Background2Panel";
            Background2Panel.Size = new Size(504, 25);
            Background2Panel.TabIndex = 10;
            // 
            // Background3Panel
            // 
            Background3Panel.BackColor = Color.FromArgb(235, 235, 235);
            Background3Panel.Location = new Point(9, 104);
            Background3Panel.Name = "Background3Panel";
            Background3Panel.Size = new Size(504, 25);
            Background3Panel.TabIndex = 12;
            // 
            // Background4Panel
            // 
            Background4Panel.BackColor = Color.FromArgb(224, 224, 224);
            Background4Panel.Location = new Point(9, 128);
            Background4Panel.Name = "Background4Panel";
            Background4Panel.Size = new Size(504, 25);
            Background4Panel.TabIndex = 14;
            // 
            // Background5Panel
            // 
            Background5Panel.BackColor = Color.FromArgb(235, 235, 235);
            Background5Panel.Location = new Point(9, 152);
            Background5Panel.Name = "Background5Panel";
            Background5Panel.Size = new Size(504, 25);
            Background5Panel.TabIndex = 15;
            // 
            // Background6Panel
            // 
            Background6Panel.BackColor = Color.FromArgb(224, 224, 224);
            Background6Panel.Location = new Point(9, 176);
            Background6Panel.Name = "Background6Panel";
            Background6Panel.Size = new Size(504, 25);
            Background6Panel.TabIndex = 16;
            // 
            // Background7Panel
            // 
            Background7Panel.BackColor = Color.FromArgb(235, 235, 235);
            Background7Panel.Location = new Point(9, 200);
            Background7Panel.Name = "Background7Panel";
            Background7Panel.Size = new Size(504, 25);
            Background7Panel.TabIndex = 21;
            // 
            // Background8Panel
            // 
            Background8Panel.BackColor = Color.FromArgb(224, 224, 224);
            Background8Panel.Location = new Point(9, 224);
            Background8Panel.Name = "Background8Panel";
            Background8Panel.Size = new Size(504, 25);
            Background8Panel.TabIndex = 22;
            // 
            // Background9Panel
            // 
            Background9Panel.BackColor = Color.FromArgb(235, 235, 235);
            Background9Panel.Location = new Point(9, 248);
            Background9Panel.Name = "Background9Panel";
            Background9Panel.Size = new Size(504, 25);
            Background9Panel.TabIndex = 23;
            // 
            // Background10Panel
            // 
            Background10Panel.BackColor = Color.FromArgb(224, 224, 224);
            Background10Panel.Location = new Point(9, 272);
            Background10Panel.Name = "Background10Panel";
            Background10Panel.Size = new Size(504, 25);
            Background10Panel.TabIndex = 2;
            // 
            // Background11Panel
            // 
            Background11Panel.BackColor = Color.FromArgb(235, 235, 235);
            Background11Panel.Location = new Point(8, 304);
            Background11Panel.Name = "Background11Panel";
            Background11Panel.Size = new Size(505, 25);
            Background11Panel.TabIndex = 1;
            // 
            // TableHeader
            // 
            TableHeader.Location = new Point(8, 19);
            TableHeader.Name = "TableHeader";
            TableHeader.Size = new Size(505, 30);
            TableHeader.TabIndex = 4;
            TableHeader.TabStop = false;
            // 
            // TableBottom
            // 
            TableBottom.Location = new Point(8, 48);
            TableBottom.Name = "TableBottom";
            TableBottom.Size = new Size(505, 281);
            TableBottom.TabIndex = 0;
            TableBottom.TabStop = false;
            // 
            // FontBox
            // 
            FontBox.Controls.Add(FontBoldButton);
            FontBox.Controls.Add(PositionsLabel);
            FontBox.Controls.Add(FontsList);
            FontBox.Controls.Add(FontSizeUpDown);
            FontBox.Controls.Add(DecPositionsSize);
            FontBox.Controls.Add(IncPositionsSize);
            FontBox.Location = new Point(272, 8);
            FontBox.Name = "FontBox";
            FontBox.Size = new Size(257, 145);
            FontBox.TabIndex = 1;
            FontBox.TabStop = false;
            FontBox.Text = " Font Settings ";
            // 
            // FontBoldButton
            // 
            FontBoldButton.Appearance = Appearance.Button;
            FontBoldButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            FontBoldButton.Location = new Point(192, 52);
            FontBoldButton.Name = "FontBoldButton";
            FontBoldButton.Size = new Size(45, 21);
            FontBoldButton.TabIndex = 0;
            FontBoldButton.Text = "Bold";
            FontBoldButton.Click += FontBoldButton_Click;
            // 
            // PositionsLabel
            // 
            PositionsLabel.Location = new Point(192, 96);
            PositionsLabel.Name = "PositionsLabel";
            PositionsLabel.Size = new Size(59, 13);
            PositionsLabel.TabIndex = 1;
            PositionsLabel.Text = "Positions:";
            // 
            // FontsList
            // 
            FontsList.ItemHeight = 13;
            FontsList.Location = new Point(8, 24);
            FontsList.Name = "FontsList";
            FontsList.Size = new Size(177, 108);
            FontsList.TabIndex = 0;
            FontsList.SelectedIndexChanged += FontsList_SelectedIndexChanged;
            // 
            // FontSizeUpDown
            // 
            FontSizeUpDown.Location = new Point(192, 24);
            FontSizeUpDown.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            FontSizeUpDown.Minimum = new decimal(new int[] { 12, 0, 0, 0 });
            FontSizeUpDown.Name = "FontSizeUpDown";
            FontSizeUpDown.Size = new Size(45, 20);
            FontSizeUpDown.TabIndex = 2;
            FontSizeUpDown.Value = new decimal(new int[] { 12, 0, 0, 0 });
            FontSizeUpDown.ValueChanged += FontSizeUpDown_ValueChanged;
            // 
            // DecPositionsSize
            // 
            DecPositionsSize.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            DecPositionsSize.Location = new Point(192, 115);
            DecPositionsSize.Name = "DecPositionsSize";
            DecPositionsSize.Size = new Size(21, 21);
            DecPositionsSize.TabIndex = 3;
            DecPositionsSize.Text = "-";
            DecPositionsSize.Click += DecPositionsSize_Click;
            // 
            // IncPositionsSize
            // 
            IncPositionsSize.Font = new Font("Microsoft Sans Serif", 8.25F);
            IncPositionsSize.Location = new Point(216, 115);
            IncPositionsSize.Name = "IncPositionsSize";
            IncPositionsSize.Size = new Size(21, 21);
            IncPositionsSize.TabIndex = 4;
            IncPositionsSize.Text = "+";
            IncPositionsSize.Click += IncPositionsSize_Click;
            // 
            // AYEmu
            // 
            AYEmu.Controls.Add(ChipFreqTextBox);
            AYEmu.Controls.Add(AyumiDCFiltBox);
            AYEmu.Controls.Add(HeardAfterLabel);
            AYEmu.Controls.Add(HeardAfterMSLabel);
            AYEmu.Controls.Add(SoundChipBox);
            AYEmu.Controls.Add(IntFreqBox);
            AYEmu.Controls.Add(SoundEngineBox);
            AYEmu.Controls.Add(ChipFreqBox);
            AYEmu.Controls.Add(DownsamplingBox);
            AYEmu.Controls.Add(PanningBox);
            AYEmu.Controls.Add(ChanMapBox);
            AYEmu.ImageIndex = 1;
            AYEmu.Location = new Point(4, 22);
            AYEmu.Name = "AYEmu";
            AYEmu.Size = new Size(192, 74);
            AYEmu.TabIndex = 2;
            AYEmu.Text = "Chip Emulation";
            // 
            // ChipFreqTextBox
            // 
            ChipFreqTextBox.Location = new Point(378, 445);
            ChipFreqTextBox.MaxLength = 7;
            ChipFreqTextBox.Name = "ChipFreqTextBox";
            ChipFreqTextBox.Size = new Size(52, 20);
            ChipFreqTextBox.TabIndex = 8;
            // 
            // AyumiDCFiltBox
            // 
            AyumiDCFiltBox.Controls.Add(DCCutOffLab);
            AyumiDCFiltBox.Controls.Add(DCCutOffTrackBar);
            AyumiDCFiltBox.Controls.Add(DCOff);
            AyumiDCFiltBox.Controls.Add(DCAyumi);
            AyumiDCFiltBox.Controls.Add(DCWbcbz7);
            AyumiDCFiltBox.Location = new Point(184, 280);
            AyumiDCFiltBox.Name = "AyumiDCFiltBox";
            AyumiDCFiltBox.Size = new Size(152, 81);
            AyumiDCFiltBox.TabIndex = 6;
            AyumiDCFiltBox.TabStop = false;
            AyumiDCFiltBox.Text = " Ayumi DC Filter ";
            AyumiDCFiltBox.Visible = false;
            // 
            // DCCutOffLab
            // 
            DCCutOffLab.Location = new Point(74, 38);
            DCCutOffLab.Name = "DCCutOffLab";
            DCCutOffLab.Size = new Size(60, 13);
            DCCutOffLab.TabIndex = 0;
            DCCutOffLab.Text = "DC Cutoff:";
            // 
            // DCCutOffTrackBar
            // 
            DCCutOffTrackBar.AutoSize = false;
            DCCutOffTrackBar.Location = new Point(70, 55);
            DCCutOffTrackBar.Minimum = 3;
            DCCutOffTrackBar.Name = "DCCutOffTrackBar";
            DCCutOffTrackBar.Size = new Size(77, 17);
            DCCutOffTrackBar.TabIndex = 2;
            DCCutOffTrackBar.Value = 5;
            // 
            // DCOff
            // 
            DCOff.Location = new Point(8, 20);
            DCOff.Name = "DCOff";
            DCOff.Size = new Size(41, 17);
            DCOff.TabIndex = 0;
            DCOff.Text = "Off";
            // 
            // DCAyumi
            // 
            DCAyumi.Location = new Point(8, 38);
            DCAyumi.Name = "DCAyumi";
            DCAyumi.Size = new Size(49, 17);
            DCAyumi.TabIndex = 1;
            DCAyumi.Text = "Ayumi";
            // 
            // DCWbcbz7
            // 
            DCWbcbz7.Location = new Point(8, 56);
            DCWbcbz7.Name = "DCWbcbz7";
            DCWbcbz7.Size = new Size(65, 17);
            DCWbcbz7.TabIndex = 3;
            DCWbcbz7.Text = "Wbcbz7";
            // 
            // HeardAfterLabel
            // 
            HeardAfterLabel.Location = new Point(16, 485);
            HeardAfterLabel.Name = "HeardAfterLabel";
            HeardAfterLabel.Size = new Size(176, 13);
            HeardAfterLabel.TabIndex = 0;
            HeardAfterLabel.Text = "Some Changes Will be Heard After:";
            // 
            // HeardAfterMSLabel
            // 
            HeardAfterMSLabel.Location = new Point(189, 485);
            HeardAfterMSLabel.Name = "HeardAfterMSLabel";
            HeardAfterMSLabel.Size = new Size(50, 13);
            HeardAfterMSLabel.TabIndex = 1;
            HeardAfterMSLabel.Text = "2178 ms";
            // 
            // SoundChipBox
            // 
            SoundChipBox.Items.Add("AY-3-8910/12");
            SoundChipBox.Items.Add("YM2149F");
            SoundChipBox.Location = new Point(128, 128);
            SoundChipBox.Name = "SoundChipBox";
            SoundChipBox.Size = new Size(152, 64);
            SoundChipBox.TabIndex = 3;
            SoundChipBox.TabStop = false;
            SoundChipBox.Text = " Sound Chip ";
            SoundChipBox.Click += SoundChipBox_Click;
            // 
            // IntFreqBox
            // 
            IntFreqBox.Controls.Add(IntFrequencyTextBox);
            IntFreqBox.Items.Add("48.828 Hz (Pentagon 128K)");
            IntFreqBox.Items.Add("50 Hz (ZX Spectrum / PAL)");
            IntFreqBox.Items.Add("60 Hz (Atari ST / NTSC)");
            IntFreqBox.Items.Add("100 Hz (Twice per INT)");
            IntFreqBox.Items.Add("200 Hz (Atari ST)");
            IntFreqBox.Items.Add("48 Hz (Non-Fractional BPM)");
            IntFreqBox.Items.Add("Manual (Hz)");
            IntFreqBox.Location = new Point(8, 288);
            IntFreqBox.Name = "IntFreqBox";
            IntFreqBox.Size = new Size(272, 184);
            IntFreqBox.TabIndex = 7;
            IntFreqBox.TabStop = false;
            IntFreqBox.Text = " Default Interrupt Frequency ";
            IntFreqBox.Click += IntFreqBox_Click;
            // 
            // IntFrequencyTextBox
            // 
            IntFrequencyTextBox.Location = new Point(94, 137);
            IntFrequencyTextBox.MaxLength = 8;
            IntFrequencyTextBox.Name = "IntFrequencyTextBox";
            IntFrequencyTextBox.Size = new Size(52, 20);
            IntFrequencyTextBox.TabIndex = 9;
            // 
            // SoundEngineBox
            // 
            SoundEngineBox.Items.Add("VT Quality");
            SoundEngineBox.Items.Add("VT Perfomance");
            SoundEngineBox.Items.Add("Ayumi (Best)");
            SoundEngineBox.Location = new Point(8, 200);
            SoundEngineBox.Name = "SoundEngineBox";
            SoundEngineBox.Size = new Size(113, 81);
            SoundEngineBox.TabIndex = 4;
            SoundEngineBox.TabStop = false;
            SoundEngineBox.Text = " Sound Engine ";
            SoundEngineBox.Click += SoundEngineBox_Click;
            // 
            // ChipFreqBox
            // 
            ChipFreqBox.Items.Add("0.894887 MHz (NES NTSC)");
            ChipFreqBox.Items.Add("0.8313035 MHz (NES PAL)");
            ChipFreqBox.Items.Add("1.7734 MHz (ZX Spectrum)");
            ChipFreqBox.Items.Add("1.75 MHz (Pentagon 128K)");
            ChipFreqBox.Items.Add("1 MHz (Amstrad CPC)");
            ChipFreqBox.Items.Add("1.5 MHz (Vectrex Console)");
            ChipFreqBox.Items.Add("2 MHz (Atari ST)");
            ChipFreqBox.Items.Add("3.5 MHz");
            ChipFreqBox.Items.Add("1520640 Hz (Natural C/Am for 4th Table) ");
            ChipFreqBox.Items.Add("1611062 Hz (Natural C#/A#m for 4th Table)");
            ChipFreqBox.Items.Add("1706861 Hz (Natural D/Bm for 4th Table) ");
            ChipFreqBox.Items.Add("1808356 Hz (Natural D#/Cm for 4th Table)");
            ChipFreqBox.Items.Add("1915886 Hz (Natural E/C#m for 4th Table)");
            ChipFreqBox.Items.Add("2029811 Hz (Natural F/Dm for 4th Table)");
            ChipFreqBox.Items.Add("2150510 Hz (Natural F#/D#m for 4th Table)");
            ChipFreqBox.Items.Add("2278386 Hz (Natural G/Em for 4th Table)");
            ChipFreqBox.Items.Add("2413866 Hz (Natural G#/Fm for 4th Table)");
            ChipFreqBox.Items.Add("2557401 Hz (Natural A/F#m for 4th Table)");
            ChipFreqBox.Items.Add("2709472 Hz (Natural A#/Gm for 4th Table)");
            ChipFreqBox.Items.Add("2870586 Hz (Natural B/G#m for 4th Table)");
            ChipFreqBox.Items.Add("3041280 Hz (Natural C/Am for 4th Table) ");
            ChipFreqBox.Items.Add("Manual (Hz)");
            ChipFreqBox.Location = new Point(288, 8);
            ChipFreqBox.Name = "ChipFreqBox";
            ChipFreqBox.Size = new Size(241, 464);
            ChipFreqBox.TabIndex = 2;
            ChipFreqBox.TabStop = false;
            ChipFreqBox.Text = " Default Chip Frequency ";
            ChipFreqBox.Click += ChipFreqBox_Click;
            // 
            // DownsamplingBox
            // 
            DownsamplingBox.Controls.Add(LoLabel);
            DownsamplingBox.Controls.Add(HiLabel);
            DownsamplingBox.Controls.Add(FilterCheckBox);
            DownsamplingBox.Controls.Add(FilterNKTrackBar);
            DownsamplingBox.Location = new Point(128, 200);
            DownsamplingBox.Name = "DownsamplingBox";
            DownsamplingBox.Size = new Size(152, 81);
            DownsamplingBox.TabIndex = 5;
            DownsamplingBox.TabStop = false;
            DownsamplingBox.Text = " Downsampling ";
            // 
            // LoLabel
            // 
            LoLabel.Location = new Point(7, 61);
            LoLabel.Name = "LoLabel";
            LoLabel.Size = new Size(26, 13);
            LoLabel.TabIndex = 0;
            LoLabel.Text = "Lo";
            // 
            // HiLabel
            // 
            HiLabel.Location = new Point(127, 61);
            HiLabel.Name = "HiLabel";
            HiLabel.Size = new Size(17, 13);
            HiLabel.TabIndex = 1;
            HiLabel.Text = "Hi";
            // 
            // FilterCheckBox
            // 
            FilterCheckBox.Location = new Point(10, 20);
            FilterCheckBox.Name = "FilterCheckBox";
            FilterCheckBox.Size = new Size(77, 17);
            FilterCheckBox.TabIndex = 0;
            FilterCheckBox.Text = "FIR-Filter";
            FilterCheckBox.Click += FiltChk_Click;
            // 
            // FilterNKTrackBar
            // 
            FilterNKTrackBar.AutoSize = false;
            FilterNKTrackBar.Location = new Point(6, 43);
            FilterNKTrackBar.Maximum = 9;
            FilterNKTrackBar.Minimum = 4;
            FilterNKTrackBar.Name = "FilterNKTrackBar";
            FilterNKTrackBar.Size = new Size(139, 17);
            FilterNKTrackBar.TabIndex = 1;
            FilterNKTrackBar.Value = 5;
            FilterNKTrackBar.ValueChanged += FiltNK_ValueChanged;
            // 
            // PanningBox
            // 
            PanningBox.Controls.Add(APanLabel);
            PanningBox.Controls.Add(BPanLabel);
            PanningBox.Controls.Add(CPanLabel);
            PanningBox.Controls.Add(APanTrackBar);
            PanningBox.Controls.Add(BPanTrackBar);
            PanningBox.Controls.Add(CPanTrackBar);
            PanningBox.Controls.Add(APanTextBox);
            PanningBox.Controls.Add(BPanTextBox);
            PanningBox.Controls.Add(CPanTextBox);
            PanningBox.Location = new Point(128, 8);
            PanningBox.Name = "PanningBox";
            PanningBox.Size = new Size(152, 113);
            PanningBox.TabIndex = 1;
            PanningBox.TabStop = false;
            PanningBox.Text = " Panning  ";
            // 
            // APanLabel
            // 
            APanLabel.Location = new Point(11, 25);
            APanLabel.Name = "APanLabel";
            APanLabel.Size = new Size(7, 13);
            APanLabel.TabIndex = 0;
            APanLabel.Text = "A";
            // 
            // BPanLabel
            // 
            BPanLabel.Location = new Point(11, 54);
            BPanLabel.Name = "BPanLabel";
            BPanLabel.Size = new Size(7, 13);
            BPanLabel.TabIndex = 1;
            BPanLabel.Text = "B";
            // 
            // CPanLabel
            // 
            CPanLabel.Location = new Point(11, 83);
            CPanLabel.Name = "CPanLabel";
            CPanLabel.Size = new Size(7, 13);
            CPanLabel.TabIndex = 2;
            CPanLabel.Text = "C";
            // 
            // APanTrackBar
            // 
            APanTrackBar.AutoSize = false;
            APanTrackBar.Location = new Point(20, 24);
            APanTrackBar.Maximum = 255;
            APanTrackBar.Name = "APanTrackBar";
            APanTrackBar.Size = new Size(91, 17);
            APanTrackBar.TabIndex = 0;
            APanTrackBar.TickFrequency = 64;
            APanTrackBar.ValueChanged += APan_ValueChanged;
            // 
            // BPanTrackBar
            // 
            BPanTrackBar.AutoSize = false;
            BPanTrackBar.Location = new Point(20, 53);
            BPanTrackBar.Maximum = 255;
            BPanTrackBar.Name = "BPanTrackBar";
            BPanTrackBar.Size = new Size(91, 17);
            BPanTrackBar.TabIndex = 2;
            BPanTrackBar.TickFrequency = 64;
            BPanTrackBar.ValueChanged += BPan_ValueChanged;
            // 
            // CPanTrackBar
            // 
            CPanTrackBar.AutoSize = false;
            CPanTrackBar.Location = new Point(20, 82);
            CPanTrackBar.Maximum = 255;
            CPanTrackBar.Name = "CPanTrackBar";
            CPanTrackBar.Size = new Size(91, 17);
            CPanTrackBar.TabIndex = 4;
            CPanTrackBar.TickFrequency = 64;
            CPanTrackBar.ValueChanged += CPan_ValueChanged;
            // 
            // APanTextBox
            // 
            APanTextBox.Location = new Point(113, 27);
            APanTextBox.MaxLength = 3;
            APanTextBox.Name = "APanTextBox";
            APanTextBox.Size = new Size(26, 20);
            APanTextBox.TabIndex = 1;
            APanTextBox.Text = "255";
            APanTextBox.KeyUp += APanTextBox_KeyUp;
            // 
            // BPanTextBox
            // 
            BPanTextBox.Location = new Point(113, 56);
            BPanTextBox.MaxLength = 3;
            BPanTextBox.Name = "BPanTextBox";
            BPanTextBox.Size = new Size(26, 20);
            BPanTextBox.TabIndex = 3;
            BPanTextBox.Text = "255";
            BPanTextBox.KeyUp += BPanTextBox_KeyUp;
            // 
            // CPanTextBox
            // 
            CPanTextBox.Location = new Point(113, 85);
            CPanTextBox.MaxLength = 3;
            CPanTextBox.Name = "CPanTextBox";
            CPanTextBox.Size = new Size(26, 20);
            CPanTextBox.TabIndex = 5;
            CPanTextBox.Text = "255";
            CPanTextBox.KeyUp += CPanTextBox_KeyUp;
            // 
            // ChanMapBox
            // 
            ChanMapBox.Items.Add("Mono");
            ChanMapBox.Items.Add("ABC");
            ChanMapBox.Items.Add("ACB");
            ChanMapBox.Items.Add("BAC");
            ChanMapBox.Items.Add("BCA");
            ChanMapBox.Items.Add("CAB");
            ChanMapBox.Items.Add("CBA");
            ChanMapBox.Location = new Point(8, 8);
            ChanMapBox.Name = "ChanMapBox";
            ChanMapBox.Size = new Size(113, 184);
            ChanMapBox.TabIndex = 0;
            ChanMapBox.TabStop = false;
            ChanMapBox.Text = " Channels Mapping ";
            ChanMapBox.Click += ChanMapBox_Click;
            // 
            // WOAPITAB
            // 
            WOAPITAB.Controls.Add(StopButton);
            WOAPITAB.Controls.Add(MIDIKeyboardBox);
            WOAPITAB.Controls.Add(SampleRateBox);
            WOAPITAB.Controls.Add(BitRateBox);
            WOAPITAB.Controls.Add(ChannelsBox);
            WOAPITAB.Controls.Add(BuffersBox);
            WOAPITAB.Controls.Add(WaveOutBox);
            WOAPITAB.ImageIndex = 4;
            WOAPITAB.Location = new Point(4, 22);
            WOAPITAB.Name = "WOAPITAB";
            WOAPITAB.Size = new Size(192, 74);
            WOAPITAB.TabIndex = 3;
            WOAPITAB.Text = "Audio";
            // 
            // StopButton
            // 
            StopButton.Image = (Image)resources.GetObject("StopButton.Image");
            StopButton.ImageAlign = ContentAlignment.MiddleLeft;
            StopButton.Location = new Point(472, 376);
            StopButton.Name = "StopButton";
            StopButton.Size = new Size(55, 25);
            StopButton.TabIndex = 0;
            StopButton.Text = "Stop";
            StopButton.TextAlign = ContentAlignment.MiddleRight;
            // 
            // MIDIKeyboardBox
            // 
            MIDIKeyboardBox.Controls.Add(MIDINextDeviceButton);
            MIDIKeyboardBox.Controls.Add(MIDIPrevDeviceButton);
            MIDIKeyboardBox.Controls.Add(MIDIStopButton);
            MIDIKeyboardBox.Controls.Add(MIDIDeviceName);
            MIDIKeyboardBox.Location = new Point(277, 224);
            MIDIKeyboardBox.Name = "MIDIKeyboardBox";
            MIDIKeyboardBox.Size = new Size(252, 137);
            MIDIKeyboardBox.TabIndex = 5;
            MIDIKeyboardBox.TabStop = false;
            MIDIKeyboardBox.Text = " MIDI Keyboard  ";
            // 
            // MIDINextDeviceButton
            // 
            MIDINextDeviceButton.Location = new Point(136, 64);
            MIDINextDeviceButton.Name = "MIDINextDeviceButton";
            MIDINextDeviceButton.Size = new Size(97, 25);
            MIDINextDeviceButton.TabIndex = 2;
            MIDINextDeviceButton.Text = "Next Device >";
            MIDINextDeviceButton.Click += MIDINextDeviceButton_Click;
            // 
            // MIDIPrevDeviceButton
            // 
            MIDIPrevDeviceButton.Location = new Point(16, 64);
            MIDIPrevDeviceButton.Name = "MIDIPrevDeviceButton";
            MIDIPrevDeviceButton.Size = new Size(105, 25);
            MIDIPrevDeviceButton.TabIndex = 1;
            MIDIPrevDeviceButton.Text = "< Previous Device";
            MIDIPrevDeviceButton.Click += MIDIPrevDeviceButton_Click;
            // 
            // MIDIStopButton
            // 
            MIDIStopButton.Location = new Point(16, 96);
            MIDIStopButton.Name = "MIDIStopButton";
            MIDIStopButton.Size = new Size(217, 25);
            MIDIStopButton.TabIndex = 3;
            MIDIStopButton.Text = "Stop MIDI";
            MIDIStopButton.Visible = false;
            MIDIStopButton.Click += MIDIStopButton_Click;
            // 
            // MIDIDeviceName
            // 
            MIDIDeviceName.Enabled = false;
            MIDIDeviceName.Location = new Point(16, 28);
            MIDIDeviceName.Name = "MIDIDeviceName";
            MIDIDeviceName.Size = new Size(217, 20);
            MIDIDeviceName.TabIndex = 0;
            MIDIDeviceName.Text = "(None)";
            // 
            // SampleRateBox
            // 
            SampleRateBox.Items.Add("11025 Hz");
            SampleRateBox.Items.Add("22050 Hz");
            SampleRateBox.Items.Add("44100 Hz");
            SampleRateBox.Items.Add("48000 Hz");
            SampleRateBox.Items.Add("88200 Hz");
            SampleRateBox.Items.Add("96000 Hz");
            SampleRateBox.Items.Add("192000 Hz");
            SampleRateBox.Location = new Point(8, 8);
            SampleRateBox.Name = "SampleRateBox";
            SampleRateBox.Size = new Size(129, 196);
            SampleRateBox.TabIndex = 0;
            SampleRateBox.TabStop = false;
            SampleRateBox.Text = " Sample Rate ";
            SampleRateBox.Click += SampleRateBox_Click;
            // 
            // BitRateBox
            // 
            BitRateBox.Items.Add("8 bit");
            BitRateBox.Items.Add("16 bit");
            BitRateBox.Items.Add("24 bit");
            BitRateBox.Items.Add("32 bit");
            BitRateBox.Location = new Point(152, 8);
            BitRateBox.Name = "BitRateBox";
            BitRateBox.Size = new Size(108, 112);
            BitRateBox.TabIndex = 1;
            BitRateBox.TabStop = false;
            BitRateBox.Text = " Bit Rate  ";
            BitRateBox.Click += BitRateBox_Click;
            // 
            // ChannelsBox
            // 
            ChannelsBox.Items.Add("Mono");
            ChannelsBox.Items.Add("Stereo");
            ChannelsBox.Location = new Point(152, 140);
            ChannelsBox.Name = "ChannelsBox";
            ChannelsBox.Size = new Size(108, 64);
            ChannelsBox.TabIndex = 3;
            ChannelsBox.TabStop = false;
            ChannelsBox.Text = "Channels";
            ChannelsBox.Click += ChannelsBox_Click;
            // 
            // BuffersBox
            // 
            BuffersBox.Controls.Add(BufferCountLabel);
            BuffersBox.Controls.Add(BufferLengthValue);
            BuffersBox.Controls.Add(BufferCountValue);
            BuffersBox.Controls.Add(TotalLengthLabel);
            BuffersBox.Controls.Add(TotalLengthValue);
            BuffersBox.Controls.Add(BufferLengthLabel);
            BuffersBox.Controls.Add(BufferLengthTrackBar);
            BuffersBox.Controls.Add(BufferCountTrackBar);
            BuffersBox.Location = new Point(277, 8);
            BuffersBox.Name = "BuffersBox";
            BuffersBox.Size = new Size(252, 201);
            BuffersBox.TabIndex = 2;
            BuffersBox.TabStop = false;
            BuffersBox.Text = " Buffers ";
            // 
            // BufferCountLabel
            // 
            BufferCountLabel.Location = new Point(16, 104);
            BufferCountLabel.Name = "BufferCountLabel";
            BufferCountLabel.Size = new Size(97, 13);
            BufferCountLabel.TabIndex = 0;
            BufferCountLabel.Text = "Number Of Buffers:";
            // 
            // BufferLengthValue
            // 
            BufferLengthValue.Location = new Point(90, 32);
            BufferLengthValue.Name = "BufferLengthValue";
            BufferLengthValue.Size = new Size(41, 13);
            BufferLengthValue.TabIndex = 1;
            BufferLengthValue.Text = "726 ms";
            // 
            // BufferCountValue
            // 
            BufferCountValue.Location = new Point(110, 105);
            BufferCountValue.Name = "BufferCountValue";
            BufferCountValue.Size = new Size(21, 13);
            BufferCountValue.TabIndex = 2;
            BufferCountValue.Text = "3";
            // 
            // TotalLengthLabel
            // 
            TotalLengthLabel.Location = new Point(16, 169);
            TotalLengthLabel.Name = "TotalLengthLabel";
            TotalLengthLabel.Size = new Size(74, 13);
            TotalLengthLabel.TabIndex = 3;
            TotalLengthLabel.Text = "Total Length:";
            // 
            // TotalLengthValue
            // 
            TotalLengthValue.Location = new Point(88, 169);
            TotalLengthValue.Name = "TotalLengthValue";
            TotalLengthValue.Size = new Size(52, 13);
            TotalLengthValue.TabIndex = 4;
            TotalLengthValue.Text = "2178 ms";
            // 
            // BufferLengthLabel
            // 
            BufferLengthLabel.Location = new Point(16, 32);
            BufferLengthLabel.Name = "BufferLengthLabel";
            BufferLengthLabel.Size = new Size(74, 13);
            BufferLengthLabel.TabIndex = 5;
            BufferLengthLabel.Text = "Buffer Length:";
            // 
            // BufferLengthTrackBar
            // 
            BufferLengthTrackBar.AutoSize = false;
            BufferLengthTrackBar.Location = new Point(8, 48);
            BufferLengthTrackBar.Maximum = 2000;
            BufferLengthTrackBar.Minimum = 5;
            BufferLengthTrackBar.Name = "BufferLengthTrackBar";
            BufferLengthTrackBar.Size = new Size(233, 33);
            BufferLengthTrackBar.TabIndex = 0;
            BufferLengthTrackBar.TickFrequency = 100;
            BufferLengthTrackBar.Value = 726;
            BufferLengthTrackBar.ValueChanged += BufferLengthTrackBar_ValueChanged;
            // 
            // BufferCountTrackBar
            // 
            BufferCountTrackBar.AutoSize = false;
            BufferCountTrackBar.Location = new Point(8, 119);
            BufferCountTrackBar.Minimum = 2;
            BufferCountTrackBar.Name = "BufferCountTrackBar";
            BufferCountTrackBar.Size = new Size(233, 33);
            BufferCountTrackBar.TabIndex = 1;
            BufferCountTrackBar.Value = 3;
            BufferCountTrackBar.ValueChanged += BufferCountTrackBar_ValueChanged;
            // 
            // WaveOutBox
            // 
            WaveOutBox.Controls.Add(WaveOutTextBox);
            WaveOutBox.Controls.Add(WaveOutGetDeviceListButton);
            WaveOutBox.Controls.Add(WaveOutDeviceCombo);
            WaveOutBox.Location = new Point(8, 224);
            WaveOutBox.Name = "WaveOutBox";
            WaveOutBox.Size = new Size(252, 137);
            WaveOutBox.TabIndex = 4;
            WaveOutBox.TabStop = false;
            WaveOutBox.Text = " Wave Out Device ";
            // 
            // WaveOutTextBox
            // 
            WaveOutTextBox.Location = new Point(16, 28);
            WaveOutTextBox.Name = "WaveOutTextBox";
            WaveOutTextBox.ReadOnly = true;
            WaveOutTextBox.Size = new Size(217, 20);
            WaveOutTextBox.TabIndex = 0;
            WaveOutTextBox.Text = "Wave Mapper";
            // 
            // WaveOutGetDeviceListButton
            // 
            WaveOutGetDeviceListButton.Location = new Point(16, 96);
            WaveOutGetDeviceListButton.Name = "WaveOutGetDeviceListButton";
            WaveOutGetDeviceListButton.Size = new Size(217, 25);
            WaveOutGetDeviceListButton.TabIndex = 2;
            WaveOutGetDeviceListButton.Text = "Get Full List";
            WaveOutGetDeviceListButton.Click += WaveOutGetDeviceListButton_Click;
            // 
            // WaveOutDeviceCombo
            // 
            WaveOutDeviceCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            WaveOutDeviceCombo.ItemHeight = 13;
            WaveOutDeviceCombo.Location = new Point(16, 64);
            WaveOutDeviceCombo.Name = "WaveOutDeviceCombo";
            WaveOutDeviceCombo.Size = new Size(217, 21);
            WaveOutDeviceCombo.TabIndex = 1;
            WaveOutDeviceCombo.Visible = false;
            WaveOutDeviceCombo.SelectedIndexChanged += WaveOutDeviceCombo_SelectedIndexChanged;
            // 
            // HotKeys
            // 
            HotKeys.Controls.Add(HotKeysBox);
            HotKeys.ImageIndex = 6;
            HotKeys.Location = new Point(4, 22);
            HotKeys.Name = "HotKeys";
            HotKeys.Size = new Size(538, 503);
            HotKeys.TabIndex = 4;
            HotKeys.Text = "HotKeys";
            // 
            // HotKeysBox
            // 
            HotKeysBox.Controls.Add(HotKeyList);
            HotKeysBox.Location = new Point(8, 8);
            HotKeysBox.Name = "HotKeysBox";
            HotKeysBox.Size = new Size(521, 481);
            HotKeysBox.TabIndex = 0;
            HotKeysBox.TabStop = false;
            // 
            // HotKeyList
            // 
            HotKeyList.Columns.AddRange(new ColumnHeader[] { HotKeyHeader1, HotKeyHeader2 });
            HotKeyList.Font = new Font("Microsoft Sans Serif", 8.25F);
            HotKeyList.GridLines = true;
            HotKeyList.Location = new Point(8, 14);
            HotKeyList.Name = "HotKeyList";
            HotKeyList.Size = new Size(505, 459);
            HotKeyList.TabIndex = 0;
            HotKeyList.TabStop = false;
            HotKeyList.UseCompatibleStateImageBehavior = false;
            HotKeyList.View = View.Details;
            HotKeyList.KeyDown += HotKeyList_KeyDown;
            HotKeyList.KeyPress += HotKeyList_KeyPress;
            // 
            // HotKeyHeader1
            // 
            HotKeyHeader1.Text = "Name";
            HotKeyHeader1.Width = 240;
            // 
            // HotKeyHeader2
            // 
            HotKeyHeader2.Text = "HotKey";
            HotKeyHeader2.Width = 240;
            // 
            // OpMod
            // 
            OpMod.Controls.Add(SaveHeadBox);
            OpMod.Controls.Add(FeaturesLevelBox);
            OpMod.Controls.Add(FileAssocBox);
            OpMod.ImageIndex = 3;
            OpMod.Location = new Point(4, 22);
            OpMod.Name = "OpMod";
            OpMod.Size = new Size(192, 74);
            OpMod.TabIndex = 5;
            OpMod.Text = "Compatibility";
            // 
            // SaveHeadBox
            // 
            SaveHeadBox.Items.Add("\"Vortex Tracker II 2.0 Module:\" Where Possible");
            SaveHeadBox.Items.Add("ProTracker 3.x Compilation Of Always");
            SaveHeadBox.Items.Add("Try To Detect");
            SaveHeadBox.Location = new Point(8, 264);
            SaveHeadBox.Name = "SaveHeadBox";
            SaveHeadBox.Size = new Size(521, 88);
            SaveHeadBox.TabIndex = 1;
            SaveHeadBox.TabStop = false;
            SaveHeadBox.Text = " Save With Header ";
            SaveHeadBox.Click += SaveHeadBox_Click;
            // 
            // FeaturesLevelBox
            // 
            FeaturesLevelBox.Items.Add("Pro Tracker 3.5");
            FeaturesLevelBox.Items.Add("Vortex Tracker II (PT 3.6)");
            FeaturesLevelBox.Items.Add("Pro Tracker 3.7");
            FeaturesLevelBox.Items.Add("Try to Detect");
            FeaturesLevelBox.Location = new Point(8, 376);
            FeaturesLevelBox.Name = "FeaturesLevelBox";
            FeaturesLevelBox.Size = new Size(521, 112);
            FeaturesLevelBox.TabIndex = 2;
            FeaturesLevelBox.TabStop = false;
            FeaturesLevelBox.Text = " Features Level ";
            FeaturesLevelBox.Click += FeaturesLevelBox_Click;
            // 
            // FileAssocBox
            // 
            FileAssocBox.Controls.Add(FileAssocList);
            FileAssocBox.Controls.Add(AllFileAssoc);
            FileAssocBox.Controls.Add(NoneFileAssoc);
            FileAssocBox.Location = new Point(8, 8);
            FileAssocBox.Name = "FileAssocBox";
            FileAssocBox.Size = new Size(521, 241);
            FileAssocBox.TabIndex = 0;
            FileAssocBox.TabStop = false;
            FileAssocBox.Text = " File Associations ";
            // 
            // FileAssocList
            // 
            FileAssocList.CheckBoxes = true;
            FileAssocList.Columns.AddRange(new ColumnHeader[] { FileAssocHeader1, FileAssocHeader2 });
            FileAssocList.GridLines = true;
            FileAssocList.Location = new Point(16, 48);
            FileAssocList.Name = "FileAssocList";
            FileAssocList.Size = new Size(489, 177);
            FileAssocList.TabIndex = 2;
            FileAssocList.UseCompatibleStateImageBehavior = false;
            FileAssocList.View = View.Details;
            FileAssocList.Click += FileAssocList_Click;
            // 
            // FileAssocHeader1
            // 
            FileAssocHeader1.Text = "Extension";
            FileAssocHeader1.Width = 65;
            // 
            // FileAssocHeader2
            // 
            FileAssocHeader2.Text = "Type";
            FileAssocHeader2.Width = 403;
            // 
            // AllFileAssoc
            // 
            AllFileAssoc.Location = new Point(16, 19);
            AllFileAssoc.Name = "AllFileAssoc";
            AllFileAssoc.Size = new Size(73, 23);
            AllFileAssoc.TabIndex = 0;
            AllFileAssoc.Text = "Check All";
            AllFileAssoc.Click += AllFileAssoc_Click;
            // 
            // NoneFileAssoc
            // 
            NoneFileAssoc.Location = new Point(96, 19);
            NoneFileAssoc.Name = "NoneFileAssoc";
            NoneFileAssoc.Size = new Size(73, 23);
            NoneFileAssoc.TabIndex = 1;
            NoneFileAssoc.Text = "Uncheck All";
            NoneFileAssoc.Click += NoneFileAssoc_Click;
            // 
            // OkButton
            // 
            OkButton.Location = new Point(368, 536);
            OkButton.Name = "OkButton";
            OkButton.Size = new Size(75, 25);
            OkButton.TabIndex = 1;
            OkButton.Text = "OK";
            OkButton.Click += OkButton_Click;
            // 
            // CancelButton
            // 
            CancelButton.Location = new Point(456, 536);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new Size(75, 25);
            CancelButton.TabIndex = 2;
            CancelButton.Text = "Cancel";
            CancelButton.Click += CancelButton_Click;
            // 
            // SaveThemeDialog
            // 
            SaveThemeDialog.DefaultExt = "vtt";
            SaveThemeDialog.Filter = "Vortext Tracker Theme (*.vtt)|*.vtt";
            // 
            // LoadThemeDialog
            // 
            LoadThemeDialog.DefaultExt = "vtt";
            LoadThemeDialog.Filter = "Vortext Tracker Theme (*.vtt)|*.vtt";
            // 
            // TemplateDialog
            // 
            TemplateDialog.DefaultExt = "vt2";
            TemplateDialog.Filter = "VortexTracker 2.0 Module (*.vt2)|*.vt2|VortexTracker 1.0 Module (*.txt)|*.txt'";
            // 
            // OptionsForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(546, 569);
            Controls.Add(OptionsTabControl);
            Controls.Add(OkButton);
            Controls.Add(CancelButton);
            Font = new Font("Microsoft Sans Serif", 8.25F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Location = new Point(1043, 118);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "OptionsForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Options";
            FormClosing += OptionsForm_FormClosing;
            Load += OptionsForm_Load;
            OptionsTabControl.ResumeLayout(false);
            CurWinds.ResumeLayout(false);
            InterfaceBox.ResumeLayout(false);
            BackupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)BackupEveryMins).EndInit();
            StartupBox.ResumeLayout(false);
            StartupBox.PerformLayout();
            FreqTableBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DefaultFreqTableUpDown).EndInit();
            ColorThemesTab.ResumeLayout(false);
            ThemesBox.ResumeLayout(false);
            ThemesOptionsBox.ResumeLayout(false);
            FontBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)FontSizeUpDown).EndInit();
            AYEmu.ResumeLayout(false);
            AYEmu.PerformLayout();
            AyumiDCFiltBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DCCutOffTrackBar).EndInit();
            IntFreqBox.ResumeLayout(false);
            IntFreqBox.PerformLayout();
            DownsamplingBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)FilterNKTrackBar).EndInit();
            PanningBox.ResumeLayout(false);
            PanningBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)APanTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)BPanTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)CPanTrackBar).EndInit();
            WOAPITAB.ResumeLayout(false);
            MIDIKeyboardBox.ResumeLayout(false);
            MIDIKeyboardBox.PerformLayout();
            BuffersBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)BufferLengthTrackBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)BufferCountTrackBar).EndInit();
            WaveOutBox.ResumeLayout(false);
            WaveOutBox.PerformLayout();
            HotKeys.ResumeLayout(false);
            HotKeysBox.ResumeLayout(false);
            OpMod.ResumeLayout(false);
            FileAssocBox.ResumeLayout(false);
            ResumeLayout(false);

        }
        #endregion

        public System.Windows.Forms.TabControl OptionsTabControl;
        public System.Windows.Forms.TabPage CurWinds;
        public System.Windows.Forms.GroupBox InterfaceBox;
        public System.Windows.Forms.CheckBox DecNumbersLines;
        public System.Windows.Forms.CheckBox DecNumbersNoise;
        public System.Windows.Forms.CheckBox HighlightSpeedPosition;
        public System.Windows.Forms.CheckBox DisablePatSeparators;
        public System.Windows.Forms.CheckBox DisableHintsOpt;
        public System.Windows.Forms.CheckBox DisableCtrlClickOpt;
        public System.Windows.Forms.CheckBox DisableInfoWinOpt;
        public System.Windows.Forms.GroupBox BackupBox;
        public System.Windows.Forms.Label EveryLabel;
        public System.Windows.Forms.Label MinutesLabel;
        public System.Windows.Forms.Label BackupFilenameLabel;
        public System.Windows.Forms.CheckBox AutoSaveBackups;
        public System.Windows.Forms.NumericUpDown BackupEveryMins;
        public RadioGroup AppPriorityBox;
        public System.Windows.Forms.GroupBox StartupBox;
        public System.Windows.Forms.Label WhenVortexStartsLabel;
        public System.Windows.Forms.Label TemplPathLab;
        public System.Windows.Forms.ComboBox StartsAction;
        public System.Windows.Forms.TextBox TemplateSong;
        public System.Windows.Forms.Button BrowseTemplate;
        public System.Windows.Forms.GroupBox FreqTableBox;
        public System.Windows.Forms.Label FrequencyTableLabel;
        public System.Windows.Forms.Label TableName;
        public System.Windows.Forms.NumericUpDown DefaultFreqTableUpDown;
        public System.Windows.Forms.TabPage ColorThemesTab;
        public System.Windows.Forms.GroupBox ThemesBox;
        public System.Windows.Forms.ListBox ColorThemesList;
        public System.Windows.Forms.Button LoadThemeButton;
        public System.Windows.Forms.Button SaveThemeButton;
        public System.Windows.Forms.Button DeleteThemeButton;
        public System.Windows.Forms.Button CopyThemeButton;
        public System.Windows.Forms.Button RenameThemeButton;
        public System.Windows.Forms.GroupBox ThemesOptionsBox;
        public System.Windows.Forms.GroupBox TableBottom;
        public System.Windows.Forms.Panel Background11Panel;
        public System.Windows.Forms.Panel Background10Panel;
        public System.Windows.Forms.Panel Background1Panel;
        public System.Windows.Forms.GroupBox TableHeader;
        public System.Windows.Forms.Panel ColBackground;
        public System.Windows.Forms.Label DefinitionLabel;
        public System.Windows.Forms.Label CurrentPatternLabel;
        public System.Windows.Forms.Label NextPrevPatternLabel;
        public System.Windows.Forms.Label BackgroundLabel;
        public System.Windows.Forms.Panel Background2Panel;
        public System.Windows.Forms.Label TextLabel;
        public System.Windows.Forms.Panel Background3Panel;
        public System.Windows.Forms.Panel ColText;
        public System.Windows.Forms.Panel Background4Panel;
        public System.Windows.Forms.Panel Background5Panel;
        public System.Windows.Forms.Panel Background6Panel;
        public System.Windows.Forms.Label LineNumbersLabel;
        public System.Windows.Forms.Label EnvelopeLabel;
        public System.Windows.Forms.Label NoiseLabel;
        public System.Windows.Forms.Label NoteLabel;
        public System.Windows.Forms.Panel Background7Panel;
        public System.Windows.Forms.Panel Background8Panel;
        public System.Windows.Forms.Panel Background9Panel;
        public System.Windows.Forms.Label NoteParamsLabel;
        public System.Windows.Forms.Label NoteCommandsLabel;
        public System.Windows.Forms.Panel ColSelLineBackground;
        public System.Windows.Forms.Panel ColHighlBackground;
        public System.Windows.Forms.Panel ColHighlText;
        public System.Windows.Forms.Panel ColLineNum;
        public System.Windows.Forms.Panel ColEnvelope;
        public System.Windows.Forms.Panel ColNoise;
        public System.Windows.Forms.Panel ColNote;
        public System.Windows.Forms.Panel ColNoteParams;
        public System.Windows.Forms.Panel ColNoteCommands;
        public System.Windows.Forms.Panel ColOutBackground;
        public System.Windows.Forms.Panel ColOutText;
        public System.Windows.Forms.Panel ColOutHlBackground;
        public System.Windows.Forms.Label SeparatorsLabel;
        public System.Windows.Forms.Panel ColSeparators;
        public System.Windows.Forms.GroupBox Seperator1;
        public System.Windows.Forms.GroupBox Seperator2;
        public System.Windows.Forms.GroupBox Seperator3;
        public System.Windows.Forms.GroupBox Seperator4;
        public System.Windows.Forms.GroupBox Seperator5;
        public System.Windows.Forms.GroupBox Seperator6;
        public System.Windows.Forms.Panel ColOutSeparators;
        public System.Windows.Forms.Panel ColSelLineText;
        public System.Windows.Forms.Panel ColSelLineNum;
        public System.Windows.Forms.Panel ColSelEnvelope;
        public System.Windows.Forms.Panel ColSelNoise;
        public System.Windows.Forms.Panel ColSelNote;
        public System.Windows.Forms.Panel ColSelNoteParams;
        public System.Windows.Forms.Panel ColSelNoteCommands;
        public System.Windows.Forms.GroupBox Seperator7;
        public System.Windows.Forms.GroupBox Seperator8;
        public System.Windows.Forms.Label SampleOrnamentLabel;
        public System.Windows.Forms.Panel ColSamOrnBackground;
        public System.Windows.Forms.Panel ColSamOrnText;
        public System.Windows.Forms.Panel ColSamOrnLineNum;
        public System.Windows.Forms.Panel ColSamNoise;
        public System.Windows.Forms.Panel ColSamOrnSeparators;
        public System.Windows.Forms.Label ToneShiftLabel;
        public System.Windows.Forms.Panel ColSamOrnTone;
        public System.Windows.Forms.Label FullScreenBackgroundLabel;
        public System.Windows.Forms.Panel ColFullScreenBackground;
        public System.Windows.Forms.GroupBox Seperator12;
        public System.Windows.Forms.Panel ColSamOrnSelBackground;
        public System.Windows.Forms.Panel ColSamOrnSelText;
        public System.Windows.Forms.Panel ColSamSelNoise;
        public System.Windows.Forms.Panel ColSamOrnSelTone;
        public System.Windows.Forms.Panel ColSamOrnSelLineNum;
        public System.Windows.Forms.GroupBox Seperator14;
        public System.Windows.Forms.Label WindowThemeLabel;
        public System.Windows.Forms.GroupBox Seperator13;
        public System.Windows.Forms.Panel ColHighlLineNum;
        public System.Windows.Forms.ComboBox WinColorsBox;
        public System.Windows.Forms.GroupBox FontBox;
        public System.Windows.Forms.CheckBox FontBoldButton;
        public System.Windows.Forms.Label PositionsLabel;
        public System.Windows.Forms.ListBox FontsList;
        public System.Windows.Forms.NumericUpDown FontSizeUpDown;
        public System.Windows.Forms.Button DecPositionsSize;
        public System.Windows.Forms.Button IncPositionsSize;
        public System.Windows.Forms.TabPage AYEmu;
        public System.Windows.Forms.Label HeardAfterLabel;
        public System.Windows.Forms.Label HeardAfterMSLabel;
        public RadioGroup SoundChipBox;
        public RadioGroup IntFreqBox;
        public RadioGroup SoundEngineBox;
        public RadioGroup ChipFreqBox;
        public System.Windows.Forms.GroupBox DownsamplingBox;
        public System.Windows.Forms.Label LoLabel;
        public System.Windows.Forms.Label HiLabel;
        public System.Windows.Forms.CheckBox FilterCheckBox;
        public System.Windows.Forms.TrackBar FilterNKTrackBar;
        public System.Windows.Forms.TextBox ChipFreqTextBox;
        public System.Windows.Forms.TextBox IntFrequencyTextBox;
        public System.Windows.Forms.GroupBox PanningBox;
        public System.Windows.Forms.Label APanLabel;
        public System.Windows.Forms.Label BPanLabel;
        public System.Windows.Forms.Label CPanLabel;
        public System.Windows.Forms.TrackBar APanTrackBar;
        public System.Windows.Forms.TrackBar BPanTrackBar;
        public System.Windows.Forms.TrackBar CPanTrackBar;
        public System.Windows.Forms.TextBox APanTextBox;
        public System.Windows.Forms.TextBox BPanTextBox;
        public System.Windows.Forms.TextBox CPanTextBox;
        public RadioGroup ChanMapBox;
        public System.Windows.Forms.GroupBox AyumiDCFiltBox;
        public System.Windows.Forms.Label DCCutOffLab;
        public System.Windows.Forms.TrackBar DCCutOffTrackBar;
        public System.Windows.Forms.RadioButton DCOff;
        public System.Windows.Forms.RadioButton DCAyumi;
        public System.Windows.Forms.RadioButton DCWbcbz7;
        public System.Windows.Forms.TabPage WOAPITAB;
        public System.Windows.Forms.Button StopButton;
        public System.Windows.Forms.GroupBox MIDIKeyboardBox;
        public System.Windows.Forms.Button MIDINextDeviceButton;
        public System.Windows.Forms.Button MIDIPrevDeviceButton;
        public System.Windows.Forms.Button MIDIStopButton;
        public System.Windows.Forms.TextBox MIDIDeviceName;
        public RadioGroup SampleRateBox;
        public RadioGroup BitRateBox;
        public RadioGroup ChannelsBox;
        public System.Windows.Forms.GroupBox BuffersBox;
        public System.Windows.Forms.Label BufferCountLabel;
        public System.Windows.Forms.Label BufferLengthValue;
        public System.Windows.Forms.Label BufferCountValue;
        public System.Windows.Forms.Label TotalLengthLabel;
        public System.Windows.Forms.Label TotalLengthValue;
        public System.Windows.Forms.Label BufferLengthLabel;
        public System.Windows.Forms.TrackBar BufferLengthTrackBar;
        public System.Windows.Forms.TrackBar BufferCountTrackBar;
        public System.Windows.Forms.GroupBox WaveOutBox;
        public System.Windows.Forms.TextBox WaveOutTextBox;
        public System.Windows.Forms.Button WaveOutGetDeviceListButton;
        public System.Windows.Forms.ComboBox WaveOutDeviceCombo;
        public System.Windows.Forms.TabPage HotKeys;
        public System.Windows.Forms.GroupBox HotKeysBox;
        public System.Windows.Forms.ListView HotKeyList;
        public System.Windows.Forms.ColumnHeader _column_3;
        public System.Windows.Forms.ColumnHeader _column_4;
        public System.Windows.Forms.TabPage OpMod;
        public RadioGroup SaveHeadBox;
        public RadioGroup FeaturesLevelBox;
        public System.Windows.Forms.GroupBox FileAssocBox;
        public System.Windows.Forms.ListView FileAssocList;
        public System.Windows.Forms.ColumnHeader _column_5;
        public System.Windows.Forms.ColumnHeader _column_6;
        public System.Windows.Forms.Button AllFileAssoc;
        public System.Windows.Forms.Button NoneFileAssoc;
        public System.Windows.Forms.Button OkButton;
        public System.Windows.Forms.Button CancelButton;
        public System.Windows.Forms.SaveFileDialog SaveThemeDialog;
        public System.Windows.Forms.OpenFileDialog LoadThemeDialog;
        public System.Windows.Forms.OpenFileDialog TemplateDialog;
        private System.Windows.Forms.ToolTip toolTip1 = null;
        private ColumnHeader HotKeyHeader1;
        private ColumnHeader HotKeyHeader2;
        private ColumnHeader FileAssocHeader1;
        private ColumnHeader FileAssocHeader2;
    }
}
