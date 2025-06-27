namespace VortexTracker
{
  partial class FxmParamsForm
    {
        public System.Windows.Forms.Label LengthLabel;
        public System.Windows.Forms.Label LoopInterruptLabel;
        public System.Windows.Forms.Label InitialTempLabel;
        public System.Windows.Forms.Label PatternSizeLabel;
        public System.Windows.Forms.Label GlobalTransposeLabel;
        public System.Windows.Forms.Label AmadAndSixLabel;
        public System.Windows.Forms.TextBox LengthBox;
        public System.Windows.Forms.TextBox LoopInterruptBox;
        public System.Windows.Forms.TextBox InitialTempoBox;
        public System.Windows.Forms.TextBox PatternSizeBox;
        public System.Windows.Forms.Button StartButton;
        public System.Windows.Forms.Button DefaultButton;
        public System.Windows.Forms.TextBox GlobalTransposeBox;
        public System.Windows.Forms.TextBox AmadAndSixBox;
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
            LengthLabel = new Label();
            LoopInterruptLabel = new Label();
            InitialTempLabel = new Label();
            PatternSizeLabel = new Label();
            GlobalTransposeLabel = new Label();
            AmadAndSixLabel = new Label();
            LengthBox = new TextBox();
            LoopInterruptBox = new TextBox();
            InitialTempoBox = new TextBox();
            PatternSizeBox = new TextBox();
            StartButton = new Button();
            DefaultButton = new Button();
            GlobalTransposeBox = new TextBox();
            AmadAndSixBox = new TextBox();
            SuspendLayout();
            // 
            // LengthLabel
            // 
            LengthLabel.Location = new Point(16, 8);
            LengthLabel.Name = "LengthLabel";
            LengthLabel.Size = new Size(52, 13);
            LengthLabel.TabIndex = 0;
            LengthLabel.Text = "Length:";
            // 
            // LoopInterruptLabel
            // 
            LoopInterruptLabel.Location = new Point(16, 48);
            LoopInterruptLabel.Name = "LoopInterruptLabel";
            LoopInterruptLabel.Size = new Size(87, 13);
            LoopInterruptLabel.TabIndex = 1;
            LoopInterruptLabel.Text = "Loop Interrupt:";
            // 
            // InitialTempLabel
            // 
            InitialTempLabel.Location = new Point(16, 88);
            InitialTempLabel.Name = "InitialTempLabel";
            InitialTempLabel.Size = new Size(76, 13);
            InitialTempLabel.TabIndex = 2;
            InitialTempLabel.Text = "Initial Tempo:";
            // 
            // PatternSizeLabel
            // 
            PatternSizeLabel.Location = new Point(136, 8);
            PatternSizeLabel.Name = "PatternSizeLabel";
            PatternSizeLabel.Size = new Size(76, 13);
            PatternSizeLabel.TabIndex = 3;
            PatternSizeLabel.Text = "Pattern Size:";
            // 
            // GlobalTransposeLabel
            // 
            GlobalTransposeLabel.Location = new Point(136, 48);
            GlobalTransposeLabel.Name = "GlobalTransposeLabel";
            GlobalTransposeLabel.Size = new Size(105, 13);
            GlobalTransposeLabel.TabIndex = 4;
            GlobalTransposeLabel.Text = "Global Transpose:";
            // 
            // AmadAndSixLabel
            // 
            AmadAndSixLabel.Location = new Point(136, 88);
            AmadAndSixLabel.Name = "AmadAndSixLabel";
            AmadAndSixLabel.Size = new Size(76, 13);
            AmadAndSixLabel.TabIndex = 5;
            AmadAndSixLabel.Text = "AmadAndSix:";
            // 
            // LengthBox
            // 
            LengthBox.Location = new Point(16, 24);
            LengthBox.Name = "LengthBox";
            LengthBox.Size = new Size(105, 20);
            LengthBox.TabIndex = 0;
            LengthBox.TextChanged += LengthBox_TextChanged;
            // 
            // LoopInterruptBox
            // 
            LoopInterruptBox.Location = new Point(16, 64);
            LoopInterruptBox.Name = "LoopInterruptBox";
            LoopInterruptBox.Size = new Size(105, 20);
            LoopInterruptBox.TabIndex = 1;
            LoopInterruptBox.TextChanged += LoopInterruptBox_TextChanged;
            // 
            // InitialTempoBox
            // 
            InitialTempoBox.Location = new Point(16, 104);
            InitialTempoBox.Name = "InitialTempoBox";
            InitialTempoBox.Size = new Size(105, 20);
            InitialTempoBox.TabIndex = 2;
            InitialTempoBox.TextChanged += InitialTempoBox_TextChanged;
            // 
            // PatternSizeBox
            // 
            PatternSizeBox.Location = new Point(136, 24);
            PatternSizeBox.Name = "PatternSizeBox";
            PatternSizeBox.Size = new Size(105, 20);
            PatternSizeBox.TabIndex = 3;
            PatternSizeBox.TextChanged += PatternSizeBox_TextChanged;
            // 
            // StartButton
            // 
            StartButton.Location = new Point(16, 136);
            StartButton.Name = "StartButton";
            StartButton.Size = new Size(105, 25);
            StartButton.TabIndex = 6;
            StartButton.Text = "Start Conversion";
            StartButton.Click += StartButton_Click;
            // 
            // DefaultButton
            // 
            DefaultButton.Location = new Point(136, 136);
            DefaultButton.Name = "DefaultButton";
            DefaultButton.Size = new Size(105, 25);
            DefaultButton.TabIndex = 7;
            DefaultButton.Text = "Default";
            DefaultButton.Click += DefaultButton_Click;
            // 
            // GlobalTransposeBox
            // 
            GlobalTransposeBox.Location = new Point(136, 64);
            GlobalTransposeBox.Name = "GlobalTransposeBox";
            GlobalTransposeBox.Size = new Size(105, 20);
            GlobalTransposeBox.TabIndex = 4;
            GlobalTransposeBox.TextChanged += GlobalTransposeBox_TextChanged;
            // 
            // AmadAndSixBox
            // 
            AmadAndSixBox.Location = new Point(136, 104);
            AmadAndSixBox.Name = "AmadAndSixBox";
            AmadAndSixBox.Size = new Size(105, 20);
            AmadAndSixBox.TabIndex = 5;
            AmadAndSixBox.TextChanged += AmadAndSixBox_ValueChanged;
            // 
            // FxmParamsForm
            // 
            ClientSize = new Size(257, 174);
            Controls.Add(LengthLabel);
            Controls.Add(LoopInterruptLabel);
            Controls.Add(InitialTempLabel);
            Controls.Add(PatternSizeLabel);
            Controls.Add(GlobalTransposeLabel);
            Controls.Add(AmadAndSixLabel);
            Controls.Add(LengthBox);
            Controls.Add(LoopInterruptBox);
            Controls.Add(InitialTempoBox);
            Controls.Add(PatternSizeBox);
            Controls.Add(StartButton);
            Controls.Add(DefaultButton);
            Controls.Add(GlobalTransposeBox);
            Controls.Add(AmadAndSixBox);
            Font = new Font("Microsoft Sans Serif", 8.25F);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Location = new Point(192, 114);
            Name = "FxmParamsForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "FXM Importer Parameters";
            FormClosing += FxmParamsForm_FormClosing;
            Load += TFXMParams_Load;
            ResumeLayout(false);
            PerformLayout();

        }
        #endregion

        private System.ComponentModel.IContainer components;
    }
}
