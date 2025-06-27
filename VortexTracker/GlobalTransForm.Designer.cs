namespace VortexTracker
{
  partial class GlobalTransForm
    {
        public System.Windows.Forms.GroupBox ChannelsToTransposeBox;
        public System.Windows.Forms.CheckBox EnvelopeTrack;
        public System.Windows.Forms.CheckBox ChannelATrack;
        public System.Windows.Forms.CheckBox ChannelBTrack;
        public System.Windows.Forms.CheckBox ChannelCTrack;
        public System.Windows.Forms.GroupBox GlobalOptionsBox;
        public System.Windows.Forms.Label NumSemitonesLabel;
        public System.Windows.Forms.NumericUpDown NumSemitonesUpDown;
        public System.Windows.Forms.RadioButton WholeModule;
        public System.Windows.Forms.RadioButton OnlyPatternNum;
        public System.Windows.Forms.NumericUpDown PatternNumUpDown;
        public System.Windows.Forms.Button TransposeButton;
        public System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.ToolTip toolTip1 = null;

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
            ChannelsToTransposeBox = new GroupBox();
            EnvelopeTrack = new CheckBox();
            ChannelATrack = new CheckBox();
            ChannelBTrack = new CheckBox();
            ChannelCTrack = new CheckBox();
            GlobalOptionsBox = new GroupBox();
            PatternNumUpDown = new NumericUpDown();
            NumSemitonesUpDown = new NumericUpDown();
            NumSemitonesLabel = new Label();
            WholeModule = new RadioButton();
            OnlyPatternNum = new RadioButton();
            TransposeButton = new Button();
            CloseButton = new Button();
            ChannelsToTransposeBox.SuspendLayout();
            GlobalOptionsBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PatternNumUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)NumSemitonesUpDown).BeginInit();
            SuspendLayout();
            // 
            // ChannelsToTransposeBox
            // 
            ChannelsToTransposeBox.Controls.Add(EnvelopeTrack);
            ChannelsToTransposeBox.Controls.Add(ChannelATrack);
            ChannelsToTransposeBox.Controls.Add(ChannelBTrack);
            ChannelsToTransposeBox.Controls.Add(ChannelCTrack);
            ChannelsToTransposeBox.Location = new Point(184, 5);
            ChannelsToTransposeBox.Name = "ChannelsToTransposeBox";
            ChannelsToTransposeBox.Size = new Size(134, 134);
            ChannelsToTransposeBox.TabIndex = 1;
            ChannelsToTransposeBox.TabStop = false;
            ChannelsToTransposeBox.Text = "Channels to Transpose";
            // 
            // EnvelopeTrack
            // 
            EnvelopeTrack.Checked = true;
            EnvelopeTrack.CheckState = CheckState.Checked;
            EnvelopeTrack.Location = new Point(10, 104);
            EnvelopeTrack.Name = "EnvelopeTrack";
            EnvelopeTrack.Size = new Size(113, 17);
            EnvelopeTrack.TabIndex = 3;
            EnvelopeTrack.Text = "Envelope Track";
            // 
            // ChannelATrack
            // 
            ChannelATrack.Checked = true;
            ChannelATrack.CheckState = CheckState.Checked;
            ChannelATrack.Location = new Point(10, 24);
            ChannelATrack.Name = "ChannelATrack";
            ChannelATrack.Size = new Size(113, 17);
            ChannelATrack.TabIndex = 0;
            ChannelATrack.Text = "Channel A Track";
            // 
            // ChannelBTrack
            // 
            ChannelBTrack.Checked = true;
            ChannelBTrack.CheckState = CheckState.Checked;
            ChannelBTrack.Location = new Point(10, 50);
            ChannelBTrack.Name = "ChannelBTrack";
            ChannelBTrack.Size = new Size(113, 17);
            ChannelBTrack.TabIndex = 1;
            ChannelBTrack.Text = "Channel B Track";
            // 
            // ChannelCTrack
            // 
            ChannelCTrack.Checked = true;
            ChannelCTrack.CheckState = CheckState.Checked;
            ChannelCTrack.Location = new Point(10, 77);
            ChannelCTrack.Name = "ChannelCTrack";
            ChannelCTrack.Size = new Size(113, 17);
            ChannelCTrack.TabIndex = 2;
            ChannelCTrack.Text = "Channel C Track";
            // 
            // GlobalOptionsBox
            // 
            GlobalOptionsBox.Controls.Add(PatternNumUpDown);
            GlobalOptionsBox.Controls.Add(NumSemitonesUpDown);
            GlobalOptionsBox.Controls.Add(NumSemitonesLabel);
            GlobalOptionsBox.Controls.Add(WholeModule);
            GlobalOptionsBox.Controls.Add(OnlyPatternNum);
            GlobalOptionsBox.Location = new Point(5, 5);
            GlobalOptionsBox.Name = "GlobalOptionsBox";
            GlobalOptionsBox.Size = new Size(172, 103);
            GlobalOptionsBox.TabIndex = 0;
            GlobalOptionsBox.TabStop = false;
            GlobalOptionsBox.Text = "Global Options";
            // 
            // PatternNumUpDown
            // 
            PatternNumUpDown.Location = new Point(117, 67);
            PatternNumUpDown.Maximum = new decimal(new int[] { 84, 0, 0, 0 });
            PatternNumUpDown.Name = "PatternNumUpDown";
            PatternNumUpDown.Size = new Size(40, 20);
            PatternNumUpDown.TabIndex = 4;
            // 
            // NumSemitonesUpDown
            // 
            NumSemitonesUpDown.Location = new Point(117, 19);
            NumSemitonesUpDown.Maximum = new decimal(new int[] { 95, 0, 0, 0 });
            NumSemitonesUpDown.Minimum = new decimal(new int[] { 95, 0, 0, int.MinValue });
            NumSemitonesUpDown.Name = "NumSemitonesUpDown";
            NumSemitonesUpDown.Size = new Size(40, 20);
            NumSemitonesUpDown.TabIndex = 1;
            // 
            // NumSemitonesLabel
            // 
            NumSemitonesLabel.Location = new Point(8, 22);
            NumSemitonesLabel.Name = "NumSemitonesLabel";
            NumSemitonesLabel.Size = new Size(112, 13);
            NumSemitonesLabel.TabIndex = 0;
            NumSemitonesLabel.Text = "Number of Semitones:";
            // 
            // WholeModule
            // 
            WholeModule.Checked = true;
            WholeModule.Location = new Point(8, 50);
            WholeModule.Name = "WholeModule";
            WholeModule.Size = new Size(102, 17);
            WholeModule.TabIndex = 2;
            WholeModule.TabStop = true;
            WholeModule.Text = "Whole Module";
            // 
            // OnlyPatternNum
            // 
            OnlyPatternNum.Location = new Point(8, 69);
            OnlyPatternNum.Name = "OnlyPatternNum";
            OnlyPatternNum.Size = new Size(112, 17);
            OnlyPatternNum.TabIndex = 5;
            OnlyPatternNum.Text = "Only Pattern Num:";
            // 
            // TransposeButton
            // 
            TransposeButton.Location = new Point(5, 117);
            TransposeButton.Name = "TransposeButton";
            TransposeButton.Size = new Size(80, 21);
            TransposeButton.TabIndex = 2;
            TransposeButton.Text = "Transpose";
            TransposeButton.Click += TransposeButton_Click;
            // 
            // CloseButton
            // 
            CloseButton.Location = new Point(96, 117);
            CloseButton.Name = "CloseButton";
            CloseButton.Size = new Size(80, 21);
            CloseButton.TabIndex = 3;
            CloseButton.Text = "Close";
            CloseButton.Click += CloseButton_Click;
            // 
            // GlobalTransForm
            // 
            ClientSize = new Size(325, 147);
            Controls.Add(ChannelsToTransposeBox);
            Controls.Add(GlobalOptionsBox);
            Controls.Add(TransposeButton);
            Controls.Add(CloseButton);
            Font = new Font("Microsoft Sans Serif", 8.25F);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Location = new Point(901, 403);
            Name = "GlobalTransForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Global Transposition";
            FormClosing += GlobalTransForm_FormClosing;
            ChannelsToTransposeBox.ResumeLayout(false);
            GlobalOptionsBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)PatternNumUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)NumSemitonesUpDown).EndInit();
            ResumeLayout(false);

        }
        #endregion

        private System.ComponentModel.IContainer components;
    }
}
