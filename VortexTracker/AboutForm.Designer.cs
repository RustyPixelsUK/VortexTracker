using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace VortexTracker
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            toolTip1 = new ToolTip(components);
            Panel1 = new Panel();
            AboutText = new TextBox();
            BuildDateLabel = new Label();
            ProgramIcon = new PictureBox();
            MyProductName = new Label();
            Version = new Label();
            OkButton = new Button();
            Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ProgramIcon).BeginInit();
            SuspendLayout();
            // 
            // Panel1
            // 
            Panel1.BackColor = SystemColors.Control;
            Panel1.Controls.Add(AboutText);
            Panel1.Controls.Add(BuildDateLabel);
            Panel1.Controls.Add(ProgramIcon);
            Panel1.Controls.Add(MyProductName);
            Panel1.Controls.Add(Version);
            Panel1.Location = new Point(8, 8);
            Panel1.Name = "Panel1";
            Panel1.Size = new Size(338, 424);
            Panel1.TabIndex = 0;
            // 
            // AboutText
            // 
            AboutText.BackColor = SystemColors.Control;
            AboutText.BorderStyle = BorderStyle.None;
            AboutText.Enabled = false;
            AboutText.Location = new Point(13, 119);
            AboutText.Multiline = true;
            AboutText.Name = "AboutText";
            AboutText.ReadOnly = true;
            AboutText.Size = new Size(312, 257);
            AboutText.TabIndex = 14;
            AboutText.Text = resources.GetString("AboutText.Text");
            // 
            // BuildDateLabel
            // 
            BuildDateLabel.Location = new Point(9, 395);
            BuildDateLabel.Name = "BuildDateLabel";
            BuildDateLabel.Size = new Size(252, 19);
            BuildDateLabel.TabIndex = 13;
            BuildDateLabel.Text = "Date of build: \r\n";
            // 
            // ProgramIcon
            // 
            ProgramIcon.ErrorImage = null;
            ProgramIcon.InitialImage = Properties.Resources.Vortex3;
            ProgramIcon.Location = new Point(41, 9);
            ProgramIcon.Name = "ProgramIcon";
            ProgramIcon.Size = new Size(256, 93);
            ProgramIcon.TabIndex = 0;
            ProgramIcon.TabStop = false;
            // 
            // MyProductName
            // 
            MyProductName.Location = new Point(9, 64);
            MyProductName.Name = "MyProductName";
            MyProductName.Size = new Size(264, 13);
            MyProductName.TabIndex = 1;
            MyProductName.TextAlign = ContentAlignment.TopCenter;
            // 
            // Version
            // 
            Version.Location = new Point(8, 80);
            Version.Name = "Version";
            Version.Size = new Size(265, 13);
            Version.TabIndex = 2;
            Version.TextAlign = ContentAlignment.TopCenter;
            // 
            // OkButton
            // 
            OkButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            OkButton.ForeColor = Color.Black;
            OkButton.Location = new Point(8, 438);
            OkButton.Name = "OkButton";
            OkButton.Size = new Size(338, 33);
            OkButton.TabIndex = 1;
            OkButton.Text = "OK";
            OkButton.Click += OkButton_Click;
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(354, 478);
            Controls.Add(Panel1);
            Controls.Add(OkButton);
            Font = new Font("Microsoft Sans Serif", 8.25F);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Location = new Point(617, 250);
            Name = "AboutForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "About Vortex Tracker III";
            FormClosing += AboutForm_FormClosing;
            Panel1.ResumeLayout(false);
            Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ProgramIcon).EndInit();
            ResumeLayout(false);

        }
        #endregion

        public System.Windows.Forms.Panel Panel1;
        public System.Windows.Forms.PictureBox ProgramIcon;
        public System.Windows.Forms.Label MyProductName;
        public System.Windows.Forms.Label Version;
        public System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.ToolTip toolTip1 = null;
        public Label BuildDateLabel;
        private TextBox AboutText;
    }
}
