namespace VortexTracker
{
    partial class TracksManagerForm
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
            PatternLabel = new Label();
            LineLabel = new Label();
            ChannelLabel = new Label();
            Location1Box = new GroupBox();
            L1PatternUpDown = new NumericUpDown();
            L1LineUpDown = new NumericUpDown();
            L1ChannelUpDown = new NumericUpDown();
            Location2Box = new GroupBox();
            L2PatternUpDown = new NumericUpDown();
            L2LineUpDown = new NumericUpDown();
            L2ChannelUpDown = new NumericUpDown();
            CopyBox = new GroupBox();
            CopyLeftButton = new Button();
            CopyRightButton = new Button();
            MoveBox = new GroupBox();
            MoveLeftButton = new Button();
            MoveRightButton = new Button();
            SwapBox = new GroupBox();
            SwapButton = new Button();
            AreaBox = new GroupBox();
            LinesUpDown = new NumericUpDown();
            LinesLabel = new Label();
            EnvelopeColumn = new CheckBox();
            NoiseColumn = new CheckBox();
            CloseButton = new Button();
            TranspositionBox = new GroupBox();
            SemitonesUpDown = new NumericUpDown();
            SemitonesLabel = new Label();
            Location1Button = new Button();
            Location2Button = new Button();
            Location1Box.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)L1PatternUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)L1LineUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)L1ChannelUpDown).BeginInit();
            Location2Box.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)L2PatternUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)L2LineUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)L2ChannelUpDown).BeginInit();
            CopyBox.SuspendLayout();
            MoveBox.SuspendLayout();
            SwapBox.SuspendLayout();
            AreaBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)LinesUpDown).BeginInit();
            TranspositionBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)SemitonesUpDown).BeginInit();
            SuspendLayout();
            // 
            // PatternLabel
            // 
            PatternLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            PatternLabel.Location = new Point(83, 36);
            PatternLabel.Name = "PatternLabel";
            PatternLabel.Size = new Size(108, 13);
            PatternLabel.TabIndex = 0;
            PatternLabel.Text = "<---  Pattern  --->";
            // 
            // LineLabel
            // 
            LineLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            LineLabel.Location = new Point(83, 63);
            LineLabel.Name = "LineLabel";
            LineLabel.Size = new Size(110, 13);
            LineLabel.TabIndex = 1;
            LineLabel.Text = "<---    Line    --->";
            // 
            // ChannelLabel
            // 
            ChannelLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            ChannelLabel.Location = new Point(83, 94);
            ChannelLabel.Name = "ChannelLabel";
            ChannelLabel.Size = new Size(110, 13);
            ChannelLabel.TabIndex = 2;
            ChannelLabel.Text = "<--- Channel --->";
            // 
            // Location1Box
            // 
            Location1Box.Controls.Add(L1PatternUpDown);
            Location1Box.Controls.Add(L1LineUpDown);
            Location1Box.Controls.Add(L1ChannelUpDown);
            Location1Box.Location = new Point(8, 8);
            Location1Box.Name = "Location1Box";
            Location1Box.Size = new Size(74, 113);
            Location1Box.TabIndex = 0;
            Location1Box.TabStop = false;
            Location1Box.Text = " Location 1 ";
            // 
            // L1PatternUpDown
            // 
            L1PatternUpDown.Location = new Point(10, 25);
            L1PatternUpDown.Maximum = new decimal(new int[] { 84, 0, 0, 0 });
            L1PatternUpDown.Name = "L1PatternUpDown";
            L1PatternUpDown.Size = new Size(48, 20);
            L1PatternUpDown.TabIndex = 1;
            // 
            // L1LineUpDown
            // 
            L1LineUpDown.Location = new Point(10, 53);
            L1LineUpDown.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            L1LineUpDown.Name = "L1LineUpDown";
            L1LineUpDown.Size = new Size(48, 20);
            L1LineUpDown.TabIndex = 3;
            // 
            // L1ChannelUpDown
            // 
            L1ChannelUpDown.Location = new Point(10, 81);
            L1ChannelUpDown.Maximum = new decimal(new int[] { 2, 0, 0, 0 });
            L1ChannelUpDown.Name = "L1ChannelUpDown";
            L1ChannelUpDown.Size = new Size(48, 20);
            L1ChannelUpDown.TabIndex = 5;
            // 
            // Location2Box
            // 
            Location2Box.Controls.Add(L2PatternUpDown);
            Location2Box.Controls.Add(L2LineUpDown);
            Location2Box.Controls.Add(L2ChannelUpDown);
            Location2Box.Location = new Point(187, 8);
            Location2Box.Name = "Location2Box";
            Location2Box.Size = new Size(78, 113);
            Location2Box.TabIndex = 1;
            Location2Box.TabStop = false;
            Location2Box.Text = " Location 2 ";
            // 
            // L2PatternUpDown
            // 
            L2PatternUpDown.Location = new Point(10, 25);
            L2PatternUpDown.Maximum = new decimal(new int[] { 84, 0, 0, 0 });
            L2PatternUpDown.Name = "L2PatternUpDown";
            L2PatternUpDown.Size = new Size(48, 20);
            L2PatternUpDown.TabIndex = 1;
            // 
            // L2LineUpDown
            // 
            L2LineUpDown.Location = new Point(10, 53);
            L2LineUpDown.Maximum = new decimal(new int[] { 63, 0, 0, 0 });
            L2LineUpDown.Name = "L2LineUpDown";
            L2LineUpDown.Size = new Size(48, 20);
            L2LineUpDown.TabIndex = 3;
            // 
            // L2ChannelUpDown
            // 
            L2ChannelUpDown.Location = new Point(10, 81);
            L2ChannelUpDown.Maximum = new decimal(new int[] { 2, 0, 0, 0 });
            L2ChannelUpDown.Name = "L2ChannelUpDown";
            L2ChannelUpDown.Size = new Size(48, 20);
            L2ChannelUpDown.TabIndex = 5;
            // 
            // CopyBox
            // 
            CopyBox.Controls.Add(CopyLeftButton);
            CopyBox.Controls.Add(CopyRightButton);
            CopyBox.Location = new Point(8, 138);
            CopyBox.Name = "CopyBox";
            CopyBox.Size = new Size(81, 55);
            CopyBox.TabIndex = 2;
            CopyBox.TabStop = false;
            CopyBox.Text = " Copy ";
            // 
            // CopyLeftButton
            // 
            CopyLeftButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            CopyLeftButton.Location = new Point(4, 20);
            CopyLeftButton.Name = "CopyLeftButton";
            CopyLeftButton.Size = new Size(34, 21);
            CopyLeftButton.TabIndex = 0;
            CopyLeftButton.Text = "<<";
            CopyLeftButton.Click += CopyLeftButton_Click;
            // 
            // CopyRightButton
            // 
            CopyRightButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            CopyRightButton.Location = new Point(42, 20);
            CopyRightButton.Name = "CopyRightButton";
            CopyRightButton.Size = new Size(32, 21);
            CopyRightButton.TabIndex = 1;
            CopyRightButton.Text = ">>";
            CopyRightButton.Click += CopyRightButton_Click;
            // 
            // MoveBox
            // 
            MoveBox.Controls.Add(MoveLeftButton);
            MoveBox.Controls.Add(MoveRightButton);
            MoveBox.Location = new Point(96, 138);
            MoveBox.Name = "MoveBox";
            MoveBox.Size = new Size(81, 55);
            MoveBox.TabIndex = 3;
            MoveBox.TabStop = false;
            MoveBox.Text = " Move ";
            // 
            // MoveLeftButton
            // 
            MoveLeftButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            MoveLeftButton.Location = new Point(6, 20);
            MoveLeftButton.Name = "MoveLeftButton";
            MoveLeftButton.Size = new Size(32, 21);
            MoveLeftButton.TabIndex = 0;
            MoveLeftButton.Text = "<<";
            MoveLeftButton.Click += MoveLeftButton_Click;
            // 
            // MoveRightButton
            // 
            MoveRightButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            MoveRightButton.Location = new Point(42, 20);
            MoveRightButton.Name = "MoveRightButton";
            MoveRightButton.Size = new Size(33, 21);
            MoveRightButton.TabIndex = 1;
            MoveRightButton.Text = ">>";
            MoveRightButton.Click += MoveRightButton_Click;
            // 
            // SwapBox
            // 
            SwapBox.Controls.Add(SwapButton);
            SwapBox.Location = new Point(184, 138);
            SwapBox.Name = "SwapBox";
            SwapBox.Size = new Size(81, 55);
            SwapBox.TabIndex = 4;
            SwapBox.TabStop = false;
            SwapBox.Text = " Swap ";
            // 
            // SwapButton
            // 
            SwapButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            SwapButton.Location = new Point(16, 20);
            SwapButton.Name = "SwapButton";
            SwapButton.Size = new Size(54, 21);
            SwapButton.TabIndex = 0;
            SwapButton.Text = "<<  >>";
            SwapButton.Click += SwapButton_Click;
            // 
            // AreaBox
            // 
            AreaBox.Controls.Add(LinesUpDown);
            AreaBox.Controls.Add(LinesLabel);
            AreaBox.Controls.Add(EnvelopeColumn);
            AreaBox.Controls.Add(NoiseColumn);
            AreaBox.Location = new Point(8, 216);
            AreaBox.Name = "AreaBox";
            AreaBox.Size = new Size(125, 113);
            AreaBox.TabIndex = 5;
            AreaBox.TabStop = false;
            AreaBox.Text = " Area ";
            // 
            // LinesUpDown
            // 
            LinesUpDown.Location = new Point(42, 25);
            LinesUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            LinesUpDown.Name = "LinesUpDown";
            LinesUpDown.Size = new Size(47, 20);
            LinesUpDown.TabIndex = 1;
            LinesUpDown.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // LinesLabel
            // 
            LinesLabel.Location = new Point(8, 27);
            LinesLabel.Name = "LinesLabel";
            LinesLabel.Size = new Size(38, 13);
            LinesLabel.TabIndex = 0;
            LinesLabel.Text = "Lines:";
            // 
            // EnvelopeColumn
            // 
            EnvelopeColumn.Location = new Point(10, 60);
            EnvelopeColumn.Name = "EnvelopeColumn";
            EnvelopeColumn.Size = new Size(115, 17);
            EnvelopeColumn.TabIndex = 2;
            EnvelopeColumn.Text = "Envelope Column";
            // 
            // NoiseColumn
            // 
            NoiseColumn.Location = new Point(10, 84);
            NoiseColumn.Name = "NoiseColumn";
            NoiseColumn.Size = new Size(94, 17);
            NoiseColumn.TabIndex = 3;
            NoiseColumn.Text = "Noise Column";
            // 
            // CloseButton
            // 
            CloseButton.Location = new Point(187, 340);
            CloseButton.Name = "CloseButton";
            CloseButton.Size = new Size(77, 29);
            CloseButton.TabIndex = 7;
            CloseButton.Text = "Close";
            CloseButton.Click += CloseButton_Click;
            // 
            // TranspositionBox
            // 
            TranspositionBox.Controls.Add(SemitonesUpDown);
            TranspositionBox.Controls.Add(SemitonesLabel);
            TranspositionBox.Controls.Add(Location1Button);
            TranspositionBox.Controls.Add(Location2Button);
            TranspositionBox.Location = new Point(144, 216);
            TranspositionBox.Name = "TranspositionBox";
            TranspositionBox.Size = new Size(121, 113);
            TranspositionBox.TabIndex = 6;
            TranspositionBox.TabStop = false;
            TranspositionBox.Text = " Transposition ";
            // 
            // SemitonesUpDown
            // 
            SemitonesUpDown.Location = new Point(68, 25);
            SemitonesUpDown.Maximum = new decimal(new int[] { 95, 0, 0, 0 });
            SemitonesUpDown.Minimum = new decimal(new int[] { 95, 0, 0, int.MinValue });
            SemitonesUpDown.Name = "SemitonesUpDown";
            SemitonesUpDown.Size = new Size(41, 20);
            SemitonesUpDown.TabIndex = 1;
            // 
            // SemitonesLabel
            // 
            SemitonesLabel.Location = new Point(10, 27);
            SemitonesLabel.Name = "SemitonesLabel";
            SemitonesLabel.Size = new Size(62, 13);
            SemitonesLabel.TabIndex = 0;
            SemitonesLabel.Text = "Semitones:";
            // 
            // Location1Button
            // 
            Location1Button.Location = new Point(10, 56);
            Location1Button.Name = "Location1Button";
            Location1Button.Size = new Size(100, 21);
            Location1Button.TabIndex = 1;
            Location1Button.Text = "Location 1";
            Location1Button.Click += Location1Button_Click;
            // 
            // Location2Button
            // 
            Location2Button.Location = new Point(10, 80);
            Location2Button.Name = "Location2Button";
            Location2Button.Size = new Size(100, 21);
            Location2Button.TabIndex = 2;
            Location2Button.Text = "Location 2";
            Location2Button.Click += Location2Button_Click;
            // 
            // TracksManagerForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(274, 377);
            Controls.Add(Location2Box);
            Controls.Add(PatternLabel);
            Controls.Add(LineLabel);
            Controls.Add(ChannelLabel);
            Controls.Add(Location1Box);
            Controls.Add(CopyBox);
            Controls.Add(MoveBox);
            Controls.Add(SwapBox);
            Controls.Add(AreaBox);
            Controls.Add(CloseButton);
            Controls.Add(TranspositionBox);
            Font = new Font("Microsoft Sans Serif", 8.25F);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Location = new Point(888, 212);
            Name = "TracksManagerForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Tracks Manager";
            FormClosing += TracksManagerForm_FormClosing;
            Location1Box.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)L1PatternUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)L1LineUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)L1ChannelUpDown).EndInit();
            Location2Box.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)L2PatternUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)L2LineUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)L2ChannelUpDown).EndInit();
            CopyBox.ResumeLayout(false);
            MoveBox.ResumeLayout(false);
            SwapBox.ResumeLayout(false);
            AreaBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)LinesUpDown).EndInit();
            TranspositionBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)SemitonesUpDown).EndInit();
            ResumeLayout(false);

        }
        #endregion

        public System.Windows.Forms.Label PatternLabel;
        public System.Windows.Forms.Label LineLabel;
        public System.Windows.Forms.Label ChannelLabel;
        public System.Windows.Forms.GroupBox Location1Box;
        public System.Windows.Forms.NumericUpDown L1PatternUpDown;
        public System.Windows.Forms.NumericUpDown L1LineUpDown;
        public System.Windows.Forms.NumericUpDown L1ChannelUpDown;
        public System.Windows.Forms.GroupBox Location2Box;
        public System.Windows.Forms.NumericUpDown L2PatternUpDown;
        public System.Windows.Forms.NumericUpDown L2LineUpDown;
        public System.Windows.Forms.NumericUpDown L2ChannelUpDown;
        public System.Windows.Forms.GroupBox CopyBox;
        public System.Windows.Forms.Button CopyLeftButton;
        public System.Windows.Forms.Button CopyRightButton;
        public System.Windows.Forms.GroupBox MoveBox;
        public System.Windows.Forms.Button MoveLeftButton;
        public System.Windows.Forms.Button MoveRightButton;
        public System.Windows.Forms.GroupBox SwapBox;
        public System.Windows.Forms.Button SwapButton;
        public System.Windows.Forms.GroupBox AreaBox;
        public System.Windows.Forms.Label LinesLabel;
        public System.Windows.Forms.CheckBox EnvelopeColumn;
        public System.Windows.Forms.CheckBox NoiseColumn;
        public System.Windows.Forms.NumericUpDown LinesUpDown;
        public System.Windows.Forms.Button CloseButton;
        public System.Windows.Forms.GroupBox TranspositionBox;
        public System.Windows.Forms.Label SemitonesLabel;
        public System.Windows.Forms.Button Location1Button;
        public System.Windows.Forms.Button Location2Button;
        public System.Windows.Forms.NumericUpDown SemitonesUpDown;
        private System.Windows.Forms.ToolTip toolTip1 = null;
    }
}
