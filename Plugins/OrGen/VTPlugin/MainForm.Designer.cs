namespace VTPlugin
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            KeyboardPictureBox = new PictureBox();
            NotePanel = new Panel();
            LmbLabel = new Label();
            RmbLabel = new Label();
            BasePanel = new Panel();
            OffsetPanel = new Panel();
            BaseLabel = new Label();
            OffsetLabel = new Label();
            LengthLabel = new Label();
            LoopLabel = new Label();
            LoopPanel = new Panel();
            toolStrip1 = new ToolStrip();
            BackButton = new ToolStripButton();
            ClearButton = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            LoadButton = new ToolStripButton();
            SaveButton = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            PreviousButton = new ToolStripButton();
            PlayButton = new ToolStripButton();
            NextButton = new ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)KeyboardPictureBox).BeginInit();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // KeyboardPictureBox
            // 
            KeyboardPictureBox.Cursor = Cursors.Hand;
            KeyboardPictureBox.Location = new Point(12, 35);
            KeyboardPictureBox.Name = "KeyboardPictureBox";
            KeyboardPictureBox.Size = new Size(560, 80);
            KeyboardPictureBox.TabIndex = 0;
            KeyboardPictureBox.TabStop = false;
            KeyboardPictureBox.Paint += KeyboardPictureBox_Paint;
            KeyboardPictureBox.MouseDown += KeyboardPictureBox_MouseDown;
            KeyboardPictureBox.MouseMove += KeyboardPictureBox_MouseMove;
            KeyboardPictureBox.MouseUp += KeyboardPictureBox_MouseUp;
            // 
            // NotePanel
            // 
            NotePanel.Location = new Point(12, 133);
            NotePanel.Name = "NotePanel";
            NotePanel.Size = new Size(412, 151);
            NotePanel.TabIndex = 0;
            // 
            // LmbLabel
            // 
            LmbLabel.AutoSize = true;
            LmbLabel.Location = new Point(460, 138);
            LmbLabel.Name = "LmbLabel";
            LmbLabel.Size = new Size(117, 15);
            LmbLabel.TabIndex = 1;
            LmbLabel.Text = "LMB: Set Offset Note";
            // 
            // RmbLabel
            // 
            RmbLabel.AutoSize = true;
            RmbLabel.Location = new Point(460, 159);
            RmbLabel.Name = "RmbLabel";
            RmbLabel.Size = new Size(110, 15);
            RmbLabel.TabIndex = 2;
            RmbLabel.Text = "RMB: Set Base Note";
            // 
            // BasePanel
            // 
            BasePanel.BackColor = Color.Cyan;
            BasePanel.BorderStyle = BorderStyle.FixedSingle;
            BasePanel.Location = new Point(465, 188);
            BasePanel.Name = "BasePanel";
            BasePanel.Size = new Size(12, 12);
            BasePanel.TabIndex = 4;
            // 
            // OffsetPanel
            // 
            OffsetPanel.BackColor = Color.Lime;
            OffsetPanel.BorderStyle = BorderStyle.FixedSingle;
            OffsetPanel.Location = new Point(465, 211);
            OffsetPanel.Name = "OffsetPanel";
            OffsetPanel.Size = new Size(12, 12);
            OffsetPanel.TabIndex = 6;
            // 
            // BaseLabel
            // 
            BaseLabel.AutoSize = true;
            BaseLabel.Location = new Point(483, 188);
            BaseLabel.Name = "BaseLabel";
            BaseLabel.Size = new Size(77, 15);
            BaseLabel.TabIndex = 5;
            BaseLabel.Text = "Base key: C-3";
            // 
            // OffsetLabel
            // 
            OffsetLabel.AutoSize = true;
            OffsetLabel.Location = new Point(483, 210);
            OffsetLabel.Name = "OffsetLabel";
            OffsetLabel.Size = new Size(78, 15);
            OffsetLabel.TabIndex = 7;
            OffsetLabel.Text = "Offset to: C-3";
            // 
            // LengthLabel
            // 
            LengthLabel.AutoSize = true;
            LengthLabel.Location = new Point(465, 264);
            LengthLabel.Name = "LengthLabel";
            LengthLabel.Size = new Size(118, 15);
            LengthLabel.TabIndex = 10;
            LengthLabel.Text = "Length: #00 Loop: #0";
            // 
            // LoopLabel
            // 
            LoopLabel.AutoSize = true;
            LoopLabel.Location = new Point(483, 233);
            LoopLabel.Name = "LoopLabel";
            LoopLabel.Size = new Size(34, 15);
            LoopLabel.TabIndex = 9;
            LoopLabel.Text = "Loop";
            // 
            // LoopPanel
            // 
            LoopPanel.BackColor = Color.Plum;
            LoopPanel.BorderStyle = BorderStyle.FixedSingle;
            LoopPanel.Location = new Point(465, 234);
            LoopPanel.Name = "LoopPanel";
            LoopPanel.Size = new Size(12, 12);
            LoopPanel.TabIndex = 8;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { BackButton, ClearButton, toolStripSeparator1, LoadButton, SaveButton, toolStripSeparator2, PreviousButton, PlayButton, NextButton });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(618, 25);
            toolStrip1.TabIndex = 18;
            toolStrip1.Text = "toolStrip1";
            // 
            // BackButton
            // 
            BackButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            BackButton.Image = (Image)resources.GetObject("BackButton.Image");
            BackButton.ImageTransparentColor = Color.Magenta;
            BackButton.Name = "BackButton";
            BackButton.Size = new Size(23, 22);
            BackButton.Text = "Back";
            BackButton.Click += BackButton_Click;
            // 
            // ClearButton
            // 
            ClearButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ClearButton.Image = (Image)resources.GetObject("ClearButton.Image");
            ClearButton.ImageTransparentColor = Color.Magenta;
            ClearButton.Name = "ClearButton";
            ClearButton.Size = new Size(23, 22);
            ClearButton.Text = "Clear";
            ClearButton.Click += ClearButton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // LoadButton
            // 
            LoadButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            LoadButton.Image = (Image)resources.GetObject("LoadButton.Image");
            LoadButton.ImageTransparentColor = Color.Magenta;
            LoadButton.Name = "LoadButton";
            LoadButton.Size = new Size(23, 22);
            LoadButton.Text = "Open";
            LoadButton.Click += LoadButton_Click;
            // 
            // SaveButton
            // 
            SaveButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            SaveButton.Image = (Image)resources.GetObject("SaveButton.Image");
            SaveButton.ImageTransparentColor = Color.Magenta;
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(23, 22);
            SaveButton.Text = "Save";
            SaveButton.Click += SaveButton_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            // 
            // PreviousButton
            // 
            PreviousButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            PreviousButton.Image = (Image)resources.GetObject("PreviousButton.Image");
            PreviousButton.ImageTransparentColor = Color.Magenta;
            PreviousButton.Name = "PreviousButton";
            PreviousButton.Size = new Size(23, 22);
            PreviousButton.Text = "Previous Ornament";
            PreviousButton.Click += PreviousButton_Click;
            // 
            // PlayButton
            // 
            PlayButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            PlayButton.Image = (Image)resources.GetObject("PlayButton.Image");
            PlayButton.ImageTransparentColor = Color.Magenta;
            PlayButton.Name = "PlayButton";
            PlayButton.Size = new Size(23, 22);
            PlayButton.Text = "Play Ornament";
            PlayButton.Click += PlayButton_Click;
            // 
            // NextButton
            // 
            NextButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            NextButton.Image = (Image)resources.GetObject("NextButton.Image");
            NextButton.ImageTransparentColor = Color.Magenta;
            NextButton.Name = "NextButton";
            NextButton.Size = new Size(23, 22);
            NextButton.Text = "Next Ornament";
            NextButton.Click += NextButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(618, 304);
            Controls.Add(toolStrip1);
            Controls.Add(LoopLabel);
            Controls.Add(LoopPanel);
            Controls.Add(LengthLabel);
            Controls.Add(OffsetLabel);
            Controls.Add(BaseLabel);
            Controls.Add(OffsetPanel);
            Controls.Add(BasePanel);
            Controls.Add(RmbLabel);
            Controls.Add(LmbLabel);
            Controls.Add(NotePanel);
            Controls.Add(KeyboardPictureBox);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MainForm";
            Text = "OrGen";
            FormClosing += MainForm_FormClosing;
            ((System.ComponentModel.ISupportInitialize)KeyboardPictureBox).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox KeyboardPictureBox;
        private Panel NotePanel;
        private Label LmbLabel;
        private Label RmbLabel;
        private Panel BasePanel;
        private Panel OffsetPanel;
        private Label BaseLabel;
        private Label OffsetLabel;
        private Label LengthLabel;
        private Label LoopLabel;
        private Panel LoopPanel;
        private ToolStrip toolStrip1;
        private ToolStripButton BackButton;
        private ToolStripButton ClearButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton LoadButton;
        private ToolStripButton SaveButton;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton PreviousButton;
        private ToolStripButton PlayButton;
        private ToolStripButton NextButton;
    }
}
