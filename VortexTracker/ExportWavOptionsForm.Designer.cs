using VortexTracker.Controls;

namespace VortexTracker
{
    partial class ExportWavOptionsForm
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
            toolTip1 = new ToolTip(components);
            Chip = new RadioGroup();
            SampleRate = new RadioGroup();
            GroupBox1 = new GroupBox();
            ExportNumLoops = new Label();
            TimesLabel = new Label();
            LoopRepeats = new NumericUpDown();
            ExportSelected = new CheckBox();
            ExportSeparate = new CheckBox();
            OpenFolder = new CheckBox();
            ExportButton = new Button();
            CancelButton = new Button();
            BitRate = new RadioGroup();
            Channels = new RadioGroup();
            GroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)LoopRepeats).BeginInit();
            SuspendLayout();
            // 
            // Chip
            // 
            Chip.Items.Add("AY");
            Chip.Items.Add("YM");
            Chip.Location = new Point(9, 158);
            Chip.Name = "Chip";
            Chip.Size = new Size(218, 59);
            Chip.TabIndex = 3;
            Chip.TabStop = false;
            // 
            // SampleRate
            // 
            SampleRate.Items.Add("22050 Hz");
            SampleRate.Items.Add("44100 Hz");
            SampleRate.Items.Add("48000 Hz");
            SampleRate.Items.Add("88200 Hz");
            SampleRate.Items.Add("96000 Hz");
            SampleRate.Items.Add("192000 Hz");
            SampleRate.Location = new Point(9, 4);
            SampleRate.Name = "SampleRate";
            SampleRate.Size = new Size(105, 148);
            SampleRate.TabIndex = 0;
            SampleRate.TabStop = false;
            // 
            // GroupBox1
            // 
            GroupBox1.Controls.Add(ExportNumLoops);
            GroupBox1.Controls.Add(TimesLabel);
            GroupBox1.Controls.Add(LoopRepeats);
            GroupBox1.Controls.Add(ExportSelected);
            GroupBox1.Controls.Add(ExportSeparate);
            GroupBox1.Controls.Add(OpenFolder);
            GroupBox1.Location = new Point(9, 222);
            GroupBox1.Name = "GroupBox1";
            GroupBox1.Size = new Size(218, 104);
            GroupBox1.TabIndex = 4;
            GroupBox1.TabStop = false;
            // 
            // ExportNumLoops
            // 
            ExportNumLoops.Location = new Point(9, 20);
            ExportNumLoops.Name = "ExportNumLoops";
            ExportNumLoops.Size = new Size(73, 13);
            ExportNumLoops.TabIndex = 0;
            ExportNumLoops.Text = "Repeat Loop:";
            // 
            // TimesLabel
            // 
            TimesLabel.Location = new Point(144, 20);
            TimesLabel.Name = "TimesLabel";
            TimesLabel.Size = new Size(37, 13);
            TimesLabel.TabIndex = 1;
            TimesLabel.Text = "Times";
            // 
            // LoopRepeats
            // 
            LoopRepeats.Location = new Point(82, 16);
            LoopRepeats.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            LoopRepeats.Name = "LoopRepeats";
            LoopRepeats.Size = new Size(53, 20);
            LoopRepeats.TabIndex = 1;
            // 
            // ExportSelected
            // 
            ExportSelected.Location = new Point(9, 41);
            ExportSelected.Name = "ExportSelected";
            ExportSelected.Size = new Size(153, 17);
            ExportSelected.TabIndex = 2;
            ExportSelected.Text = "Export Selected Positions";
            // 
            // ExportSeparate
            // 
            ExportSeparate.Location = new Point(9, 61);
            ExportSeparate.Name = "ExportSeparate";
            ExportSeparate.Size = new Size(200, 17);
            ExportSeparate.TabIndex = 3;
            ExportSeparate.Text = "Export Each Channel to Separate File";
            // 
            // OpenFolder
            // 
            OpenFolder.Location = new Point(9, 81);
            OpenFolder.Name = "OpenFolder";
            OpenFolder.Size = new Size(200, 17);
            OpenFolder.TabIndex = 4;
            OpenFolder.Text = "Open Folder";
            // 
            // ExportButton
            // 
            ExportButton.Location = new Point(152, 332);
            ExportButton.Name = "ExportButton";
            ExportButton.Size = new Size(75, 25);
            ExportButton.TabIndex = 6;
            ExportButton.Text = "Export";
            ExportButton.Click += ExportButton_Click;
            // 
            // CancelButton
            // 
            CancelButton.Location = new Point(65, 332);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new Size(75, 25);
            CancelButton.TabIndex = 5;
            CancelButton.Text = "Cancel";
            CancelButton.Click += CancelButton_Click;
            // 
            // BitRate
            // 
            BitRate.Items.Add("16 bit");
            BitRate.Items.Add("24 bit");
            BitRate.Items.Add("32 bit");
            BitRate.Location = new Point(122, 4);
            BitRate.Name = "BitRate";
            BitRate.Size = new Size(105, 88);
            BitRate.TabIndex = 1;
            BitRate.TabStop = false;
            // 
            // Channels
            // 
            Channels.Items.Add("Mono");
            Channels.Items.Add("Stereo");
            Channels.Location = new Point(122, 98);
            Channels.Name = "Channels";
            Channels.Size = new Size(105, 54);
            Channels.TabIndex = 2;
            Channels.TabStop = false;
            // 
            // ExportWavOptionsForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(236, 366);
            Controls.Add(Chip);
            Controls.Add(SampleRate);
            Controls.Add(BitRate);
            Controls.Add(Channels);
            Controls.Add(GroupBox1);
            Controls.Add(ExportButton);
            Controls.Add(CancelButton);
            Font = new Font("Microsoft Sans Serif", 8.25F);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Location = new Point(987, 299);
            Name = "ExportWavOptionsForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Export Options";
            GroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)LoopRepeats).EndInit();
            ResumeLayout(false);

        }
        #endregion

        public RadioGroup Chip;
        public RadioGroup SampleRate;
        public System.Windows.Forms.GroupBox GroupBox1;
        public System.Windows.Forms.Label ExportNumLoops;
        public System.Windows.Forms.Label TimesLabel;
        public System.Windows.Forms.NumericUpDown LoopRepeats;
        public System.Windows.Forms.CheckBox ExportSelected;
        public System.Windows.Forms.CheckBox ExportSeparate;
        public System.Windows.Forms.CheckBox OpenFolder;
        public System.Windows.Forms.Button ExportButton;
        public System.Windows.Forms.Button CancelButton;
        public RadioGroup BitRate;
        public RadioGroup Channels;
        private System.Windows.Forms.ToolTip toolTip1 = null;
    }
}
