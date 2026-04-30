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
            toolTip1 = new ToolTip(components);
            Panel1 = new Panel();
            BuildDateLabel = new Label();
            label8 = new Label();
            label7 = new Label();
            ProgramIcon = new PictureBox();
            MyProductName = new Label();
            Version = new Label();
            Copyright = new Label();
            Comments = new Label();
            Label1 = new Label();
            Label3 = new Label();
            Label5 = new Label();
            Label4 = new Label();
            Label2 = new Label();
            Label6 = new Label();
            line1 = new Panel();
            line2 = new Panel();
            OkButton = new Button();
            Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ProgramIcon).BeginInit();
            SuspendLayout();
            // 
            // Panel1
            // 
            Panel1.BackColor = SystemColors.Control;
            Panel1.Controls.Add(BuildDateLabel);
            Panel1.Controls.Add(label8);
            Panel1.Controls.Add(label7);
            Panel1.Controls.Add(ProgramIcon);
            Panel1.Controls.Add(MyProductName);
            Panel1.Controls.Add(Version);
            Panel1.Controls.Add(Copyright);
            Panel1.Controls.Add(Comments);
            Panel1.Controls.Add(Label1);
            Panel1.Controls.Add(Label3);
            Panel1.Controls.Add(Label5);
            Panel1.Controls.Add(Label4);
            Panel1.Controls.Add(Label2);
            Panel1.Controls.Add(Label6);
            Panel1.Controls.Add(line1);
            Panel1.Controls.Add(line2);
            Panel1.Location = new Point(8, 8);
            Panel1.Name = "Panel1";
            Panel1.Size = new Size(281, 450);
            Panel1.TabIndex = 0;
            // 
            // BuildDateLabel
            // 
            BuildDateLabel.Location = new Point(9, 431);
            BuildDateLabel.Name = "BuildDateLabel";
            BuildDateLabel.Size = new Size(252, 19);
            BuildDateLabel.TabIndex = 13;
            BuildDateLabel.Text = "Date of build: \r\n";
            // 
            // label8
            // 
            label8.Location = new Point(8, 353);
            label8.Name = "label8";
            label8.Size = new Size(252, 31);
            label8.TabIndex = 12;
            label8.Text = "(c) Dexus (Volutar) 2022-2025\nVersion 2.6.1";
            // 
            // label7
            // 
            label7.Location = new Point(9, 392);
            label7.Name = "label7";
            label7.Size = new Size(252, 30);
            label7.TabIndex = 11;
            label7.Text = "(c) Ben Baker (Rusty Pixels) 2025\r\nVersion 3.0+";
            // 
            // ProgramIcon
            // 
            ProgramIcon.ErrorImage = null;
            ProgramIcon.Location = new Point(40, 8);
            ProgramIcon.Name = "ProgramIcon";
            ProgramIcon.Size = new Size(200, 50);
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
            // Copyright
            // 
            Copyright.Location = new Point(8, 120);
            Copyright.Name = "Copyright";
            Copyright.Size = new Size(190, 13);
            Copyright.TabIndex = 3;
            Copyright.Text = "Copyright (c) 2000-2009 S.V. Bulba";
            // 
            // Comments
            // 
            Comments.Location = new Point(8, 136);
            Comments.Name = "Comments";
            Comments.Size = new Size(265, 13);
            Comments.TabIndex = 4;
            Comments.Text = "Thanks to Roman Scherbakov for idea and graphics";
            // 
            // Label1
            // 
            Label1.Location = new Point(8, 152);
            Label1.Name = "Label1";
            Label1.Size = new Size(202, 13);
            Label1.TabIndex = 5;
            Label1.Text = "Thanks to Shiru Otaku for OrGen plug-in";
            // 
            // Label3
            // 
            Label3.Location = new Point(8, 168);
            Label3.Name = "Label3";
            Label3.Size = new Size(190, 13);
            Label3.TabIndex = 6;
            Label3.Text = "Thanks to Denis Seleznev for icons";
            // 
            // Label5
            // 
            Label5.Location = new Point(9, 213);
            Label5.Name = "Label5";
            Label5.Size = new Size(251, 26);
            Label5.TabIndex = 7;
            Label5.Text = "(c) Oisee (Siril/4D) 2010\r\nNatural 4th table added, control improvement, MIDI";
            // 
            // Label4
            // 
            Label4.Location = new Point(8, 320);
            Label4.Name = "Label4";
            Label4.Size = new Size(252, 31);
            Label4.TabIndex = 8;
            Label4.Text = "(c) Ivan Pirog (Flexx/Enhancers) 2017-2021\r\nVersion 1.5 - 2.6";
            // 
            // Label2
            // 
            Label2.Location = new Point(8, 184);
            Label2.Name = "Label2";
            Label2.Size = new Size(252, 13);
            Label2.TabIndex = 9;
            Label2.Text = "Thanks to MmcM, nq, bfox, mr287cc, Oisee, Flexx";
            // 
            // Label6
            // 
            Label6.Location = new Point(9, 255);
            Label6.Name = "Label6";
            Label6.Size = new Size(251, 60);
            Label6.TabIndex = 10;
            Label6.Text = "Ayumi Engine:\r\nAuthor: Peter Sovietov (True Grue)\r\nVortex Tracker adaptation by Ivan Pirog (Flexx)\r\nSound improvements by Artem Vasilev (wbcbz7)";
            // 
            // line1
            // 
            line1.BackColor = SystemColors.ControlDark;
            line1.Location = new Point(8, 110);
            line1.Name = "line1";
            line1.Size = new Size(265, 1);
            line1.TabIndex = 0;
            // 
            // line2
            // 
            line2.BackColor = SystemColors.ControlDark;
            line2.Location = new Point(8, 386);
            line2.Name = "line2";
            line2.Size = new Size(265, 1);
            line2.TabIndex = 1;
            // 
            // OkButton
            // 
            OkButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            OkButton.ForeColor = Color.Black;
            OkButton.Location = new Point(8, 464);
            OkButton.Name = "OkButton";
            OkButton.Size = new Size(281, 33);
            OkButton.TabIndex = 1;
            OkButton.Text = "OK";
            OkButton.Click += OkButton_Click;
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(298, 509);
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
            ((System.ComponentModel.ISupportInitialize)ProgramIcon).EndInit();
            ResumeLayout(false);

        }
        #endregion

        public System.Windows.Forms.Panel Panel1;
        public System.Windows.Forms.PictureBox ProgramIcon;
        public System.Windows.Forms.Label MyProductName;
        public System.Windows.Forms.Label Version;
        public System.Windows.Forms.Label Copyright;
        public System.Windows.Forms.Label Comments;
        public System.Windows.Forms.Label Label1;
        public System.Windows.Forms.Label Label3;
        public System.Windows.Forms.Label Label5;
        public System.Windows.Forms.Label Label4;
        public System.Windows.Forms.Label Label2;
        public System.Windows.Forms.Label Label6;
        public System.Windows.Forms.Panel line1;
        public System.Windows.Forms.Panel line2;
        public System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.ToolTip toolTip1 = null;
        public System.Windows.Forms.Label label7;
        public Label label8;
        public Label BuildDateLabel;
    }
}
