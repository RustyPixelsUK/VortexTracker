using System.Windows.Forms;

namespace VortexTracker
{
    partial class TrackInfoForm
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
            Info = new RichTextBox();
            OkButton = new Button();
            SuspendLayout();
            // 
            // Info
            // 
            Info.Location = new Point(10, 10);
            Info.Name = "Info";
            Info.ReadOnly = true;
            Info.Size = new Size(599, 423);
            Info.TabIndex = 0;
            Info.Text = "";
            // 
            // OkButton
            // 
            OkButton.BackColor = SystemColors.Control;
            OkButton.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            OkButton.Location = new Point(10, 443);
            OkButton.Name = "OkButton";
            OkButton.Size = new Size(599, 33);
            OkButton.TabIndex = 1;
            OkButton.Text = "OK";
            OkButton.UseVisualStyleBackColor = false;
            OkButton.Click += OkButton_Click;
            // 
            // TrackInfoForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.Black;
            ClientSize = new Size(619, 490);
            Controls.Add(Info);
            Controls.Add(OkButton);
            Font = new Font("Microsoft Sans Serif", 8.25F);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Location = new Point(645, 245);
            Name = "TrackInfoForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Track Info";
            FormClosing += TrackInfoForm_FormClosing;
            ResumeLayout(false);

        }
        #endregion

        public System.Windows.Forms.RichTextBox Info;
        public System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.ToolTip toolTip1 = null;
    }
}
