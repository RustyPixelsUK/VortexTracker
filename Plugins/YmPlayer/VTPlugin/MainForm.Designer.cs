namespace VTPlugin
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            ComPortGroup = new GroupBox();
            StopBitsLabel = new Label();
            StopBitsBox = new ComboBox();
            ParityLabel = new Label();
            ParityBox = new ComboBox();
            DataBitsLabel = new Label();
            DataBitsBox = new ComboBox();
            BaudRateLabel = new Label();
            PortLabel = new Label();
            PortBox = new ComboBox();
            BaudRateBox = new ComboBox();
            OKButton = new Button();
            ComPortGroup.SuspendLayout();
            SuspendLayout();
            // 
            // ComPortGroup
            // 
            ComPortGroup.Controls.Add(StopBitsLabel);
            ComPortGroup.Controls.Add(StopBitsBox);
            ComPortGroup.Controls.Add(ParityLabel);
            ComPortGroup.Controls.Add(ParityBox);
            ComPortGroup.Controls.Add(DataBitsLabel);
            ComPortGroup.Controls.Add(DataBitsBox);
            ComPortGroup.Controls.Add(BaudRateLabel);
            ComPortGroup.Controls.Add(PortLabel);
            ComPortGroup.Controls.Add(PortBox);
            ComPortGroup.Controls.Add(BaudRateBox);
            ComPortGroup.Location = new Point(12, 12);
            ComPortGroup.Name = "ComPortGroup";
            ComPortGroup.Size = new Size(345, 148);
            ComPortGroup.TabIndex = 2;
            ComPortGroup.TabStop = false;
            ComPortGroup.Text = "Com Port Settings";
            // 
            // StopBitsLabel
            // 
            StopBitsLabel.AutoSize = true;
            StopBitsLabel.Location = new Point(265, 78);
            StopBitsLabel.Name = "StopBitsLabel";
            StopBitsLabel.Size = new Size(56, 15);
            StopBitsLabel.TabIndex = 6;
            StopBitsLabel.Text = "Stop Bits:";
            // 
            // StopBitsBox
            // 
            StopBitsBox.DropDownStyle = ComboBoxStyle.DropDownList;
            StopBitsBox.Location = new Point(269, 98);
            StopBitsBox.Name = "StopBitsBox";
            StopBitsBox.Size = new Size(57, 23);
            StopBitsBox.TabIndex = 10;
            // 
            // ParityLabel
            // 
            ParityLabel.AutoSize = true;
            ParityLabel.Location = new Point(198, 78);
            ParityLabel.Name = "ParityLabel";
            ParityLabel.Size = new Size(40, 15);
            ParityLabel.TabIndex = 5;
            ParityLabel.Text = "Parity:";
            // 
            // ParityBox
            // 
            ParityBox.DropDownStyle = ComboBoxStyle.DropDownList;
            ParityBox.Location = new Point(202, 98);
            ParityBox.Name = "ParityBox";
            ParityBox.Size = new Size(57, 23);
            ParityBox.TabIndex = 9;
            // 
            // DataBitsLabel
            // 
            DataBitsLabel.AutoSize = true;
            DataBitsLabel.Location = new Point(131, 78);
            DataBitsLabel.Name = "DataBitsLabel";
            DataBitsLabel.Size = new Size(56, 15);
            DataBitsLabel.TabIndex = 4;
            DataBitsLabel.Text = "Data Bits:";
            // 
            // DataBitsBox
            // 
            DataBitsBox.DropDownStyle = ComboBoxStyle.DropDownList;
            DataBitsBox.Location = new Point(134, 98);
            DataBitsBox.Name = "DataBitsBox";
            DataBitsBox.Size = new Size(58, 23);
            DataBitsBox.TabIndex = 8;
            // 
            // BaudRateLabel
            // 
            BaudRateLabel.AutoSize = true;
            BaudRateLabel.Location = new Point(16, 78);
            BaudRateLabel.Name = "BaudRateLabel";
            BaudRateLabel.Size = new Size(63, 15);
            BaudRateLabel.TabIndex = 3;
            BaudRateLabel.Text = "Baud Rate:";
            // 
            // PortLabel
            // 
            PortLabel.AutoSize = true;
            PortLabel.Location = new Point(16, 23);
            PortLabel.Name = "PortLabel";
            PortLabel.Size = new Size(32, 15);
            PortLabel.TabIndex = 0;
            PortLabel.Text = "Port:";
            // 
            // PortBox
            // 
            PortBox.DropDownStyle = ComboBoxStyle.DropDownList;
            PortBox.Location = new Point(19, 44);
            PortBox.Name = "PortBox";
            PortBox.Size = new Size(106, 23);
            PortBox.TabIndex = 1;
            // 
            // BaudRateBox
            // 
            BaudRateBox.DropDownStyle = ComboBoxStyle.DropDownList;
            BaudRateBox.Location = new Point(19, 98);
            BaudRateBox.Name = "BaudRateBox";
            BaudRateBox.Size = new Size(106, 23);
            BaudRateBox.TabIndex = 7;
            // 
            // OKButton
            // 
            OKButton.Location = new Point(126, 174);
            OKButton.Name = "OKButton";
            OKButton.Size = new Size(114, 31);
            OKButton.TabIndex = 11;
            OKButton.Text = "OK";
            OKButton.Click += OKButton_Click;
            // 
            // MainForm
            // 
            AutoScaleBaseSize = new Size(6, 16);
            ClientSize = new Size(373, 227);
            Controls.Add(OKButton);
            Controls.Add(ComPortGroup);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Configuration";
            FormClosing += MainForm_FormClosing;
            ComPortGroup.ResumeLayout(false);
            ComPortGroup.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private GroupBox ComPortGroup;
        private Label StopBitsLabel;
        private ComboBox StopBitsBox;
        private Label ParityLabel;
        private ComboBox ParityBox;
        private Label DataBitsLabel;
        private ComboBox DataBitsBox;
        private Label BaudRateLabel;
        private Label PortLabel;
        private ComboBox PortBox;
        private ComboBox BaudRateBox;
        private Button OKButton;
    }
}
